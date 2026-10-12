"""The seven phases. Each one: steps -> gates -> out/<P>/report.md + gates.json + plots + tables.
A phase refuses to start unless the previous phase status is PASS."""
from __future__ import annotations
import hashlib, itertools, json, warnings
from pathlib import Path
import numpy as np, pandas as pd, yaml
import matplotlib.pyplot as plt
from scipy import stats as sst
from scipy.ndimage import generic_filter
from . import core, stats as S, selftest
from .engine import Market
from .report import Report, require as _require

# ----------------------------------------------------------------------------- context / helpers
class Ctx:
    def __init__(self, cfg_path):
        self.path = Path(cfg_path).resolve()
        self.base = self.path.parent
        self.cfg = yaml.safe_load(open(self.path))
        self.out = self.base / self.cfg["out_dir"]
        self.data = self.base / self.cfg["data_dir"]
        self.syms = list(self.cfg["instruments"])
        self.demo = bool(self.cfg.get("demo_ignore_gates", False))

    def ins(self, s):
        return self.cfg["instruments"][s]

def require(ctx, phase):
    if ctx.demo:
        print(f"[demo] gates of {phase} ignored (code test only)"); return
    _require(ctx.out, phase)

def market(ctx, sym, upto="val"):
    df = pd.read_pickle(ctx.out / "cache" / f"{sym}.pkl")
    if upto == "val":                      # the sealed test rows are not even loaded before P6
        df = df[df["seg"] != "test"].reset_index(drop=True)
    sl, rl = core.last_index_arrays(df)
    b = core.Bars(df, ctx.cfg["atr_len"])
    i = ctx.ins(sym)
    m = Market(b, sl, rl, i["tick"], i["cost_ticks_rt"], i["stop_slip_ticks"], ctx.cfg["intrabar"] == "pessimistic")
    seg, sess = {}, df["sess"].to_numpy()
    for name in ("dev", "val", "test"):
        idx = np.nonzero(df["seg"].to_numpy() == name)[0]
        if len(idx):
            seg[name] = (int(idx[0]), int(idx[-1]) + 1, int(sess[idx[0]]), int(sess[idx[-1]]) + 1)
    blk = df.groupby("sess")["blk"].first().to_numpy()
    return df, b, m, seg, blk

def side_of(rec):
    return 1 if rec["side"] == "long" else -1

def with_params(rec, L, V):
    r = dict(rec)
    if r["uses_t3_recomputed"]:
        r["t3_length"], r["t3_vf"] = L, V
    return r

def regime(b):
    ratio = b.atr1 / pd.Series(b.atr1).rolling(500, min_periods=100).median().to_numpy()
    return np.where(ratio < 0.85, "lowvol", np.where(ratio > 1.15, "highvol", "midvol"))

def load_registry(ctx):
    return pd.read_csv(ctx.out / "P1" / "candidate_registry_fixed.csv")

def unit_key(cid, sym):
    return f"{cid}__{sym}"

def _empty(r):
    r.add("found", "No unit reached this phase: the previous phase rejected everything. That is a valid research result "
                   "(no edge that survives the gates), not a bug.")
    r.gate("units entering this phase", 0, ">= 1", False)
    return r.save()

# ============================================================================= P0
def P0(ctx):
    r = Report(ctx.out, "P0", "Data, sessions, QA and the sealed split")
    (ctx.out / "cache").mkdir(parents=True, exist_ok=True)
    cs, sp = ctx.cfg["session"], ctx.cfg["split"]
    r.add("did", "Loaded each raw 30-min CSV, removed duplicate timestamps, repaired OHLC ordering, built CME sessions "
          f"(start {cs['start_hour']}:00, new session after any gap >= {cs['gap_minutes']} min), flagged contract rolls, "
          f"and cut a chronological split by session: Dev {sp['dev']:.0%} / Val {sp['val']:.0%} / Test {sp['test']:.0%} with a "
          f"{sp['embargo_sessions']}-session embargo between segments. Dev is cut into {sp['dev_blocks']} blocks for walk-forward. "
          "The test segment is hashed here and is not loaded again until P6.")
    r.add("tried", "Nothing is tuned in P0.")
    qa, spl, hashes = [], [], {}
    fig, ax = plt.subplots(len(ctx.syms), 1, figsize=(11, 2.1 * len(ctx.syms)), squeeze=False)
    fig2, ax2 = plt.subplots(1, 2, figsize=(11, 3.4))
    for k, sym in enumerate(ctx.syms):
        i = ctx.ins(sym)
        p = ctx.data / i["file"]
        if not p.exists():
            hits = sorted(ctx.data.glob(f"*{sym}_30min*.csv"))
            if not hits:
                raise SystemExit(f"missing data file for {sym} in {ctx.data}")
            p = hits[0]
        df = core.load_symbol(str(p))
        dup = int(df["ts"].duplicated(keep="last").sum())
        df = df[~df["ts"].duplicated(keep="last")].reset_index(drop=True)
        bad = int(((df["high"] < df[["open", "close"]].max(1)) | (df["low"] > df[["open", "close"]].min(1))).sum())
        df["high"] = df[["open", "high", "low", "close"]].max(1); df["low"] = df[["open", "high", "low", "close"]].min(1)
        offgrid = float(((df["ts"].dt.minute % 30) != 0).mean())
        offtick = float((np.abs(df["close"] / i["tick"] - np.round(df["close"] / i["tick"])) > 1e-6).mean())
        df = core.add_sessions(df, cs["start_hour"], cs["gap_minutes"], {k_: tuple(v) for k_, v in cs["buckets"].items()})
        bps = df.groupby("sess").size()
        atr = core.atr_wilder(*(df[c].to_numpy() for c in ("high", "low", "close")))
        g = np.abs(df["open"] - df["close"].shift()) / pd.Series(atr).shift()
        first = df["sess"].diff().fillna(1).to_numpy() > 0
        roll_gap = float(g[df["roll"] & first].median()) if (df["roll"] & first).any() else np.nan
        sess_gap = float(g[first & ~df["roll"]].median())
        n = int(df["sess"].max()) + 1; e = sp["embargo_sessions"]
        nd, nv = int(n * sp["dev"]), int(n * sp["val"])
        seg = np.full(n, "emb", dtype=object)
        seg[:nd] = "dev"; seg[nd + e:nd + e + nv] = "val"; seg[nd + 2 * e + nv:] = "test"
        blk = np.full(n, -1); blk[:nd] = np.concatenate([np.full(len(a), j) for j, a in enumerate(np.array_split(np.arange(nd), sp["dev_blocks"]))])
        df["seg"] = seg[df["sess"].to_numpy()]; df["blk"] = blk[df["sess"].to_numpy()]
        df.to_pickle(ctx.out / "cache" / f"{sym}.pkl")
        hashes[sym] = hashlib.sha256(df[df["seg"] == "test"][["ts", "open", "high", "low", "close"]].to_csv().encode()).hexdigest()[:16]
        qa.append(dict(symbol=sym, file=p.name, bars=len(df), first=df["ts"].iloc[0], last=df["ts"].iloc[-1], sessions=n,
                       dup_removed=dup, ohlc_repaired=bad, offgrid_ts=offgrid, offtick_close=offtick,
                       median_bars_per_session=float(bps.median()), short_sessions=int((bps < 0.5 * bps.median()).sum()),
                       rolls=int(df["roll"].sum()), roll_open_gap_atr=roll_gap, normal_open_gap_atr=sess_gap,
                       min_price=float(df["low"].min())))
        for name in ("dev", "val", "test"):
            d = df[df["seg"] == name]
            spl.append(dict(symbol=sym, segment=name, start=d["ts"].iloc[0], end=d["ts"].iloc[-1],
                            sessions=d["sess"].nunique(), bars=len(d)))
        a = ax[k, 0]; a.plot(df["ts"], df["close"], lw=0.5, color="k"); a.set_ylabel(sym)
        for name, col in (("val", "tab:orange"), ("test", "tab:red")):
            d = df[df["seg"] == name]; a.axvspan(d["ts"].iloc[0], d["ts"].iloc[-1], color=col, alpha=0.15)
        ax2[0].hist(bps.to_numpy(), bins=50, histtype="step", label=sym)
        ax2[1].hist(df["ts"].dt.hour + df["ts"].dt.minute / 60, bins=48, histtype="step", label=sym)
    ax2[0].set_title("bars per session"); ax2[1].set_title("bar timestamp hour (check the timezone)"); ax2[0].legend()
    r.fig(fig, "split_timeline", "Close price per instrument; orange = Validation, red = sealed Test, rest = Dev")
    r.fig(fig2, "sessions", "Session lengths and time-of-day coverage")
    Q = pd.DataFrame(qa); SP = pd.DataFrame(spl)
    r.table("changed", Q[["symbol", "dup_removed", "ohlc_repaired"]], "repairs")
    r.table("found", Q, "data_qa")
    r.table("found", SP, "splits")
    if (Q["min_price"] <= 0).any():
        r.add("found", "Non-positive prices exist (additive back-adjustment). The framework only uses price differences in "
                       "ticks and ATR units; never percentage returns.")
    r.add("found", "`roll_open_gap_atr` close to `normal_open_gap_atr` means the back-adjustment removed the roll gaps.")
    r.gate("OHLC repairs <= 0.1% of bars", f"{(Q.ohlc_repaired / Q.bars).max():.4%}", "<= 0.1%", (Q.ohlc_repaired / Q.bars).max() <= 0.001)
    r.gate("timestamps on the 30-min grid", f"{1 - Q.offgrid_ts.max():.3%}", ">= 99%", Q.offgrid_ts.max() <= 0.01)
    r.gate("prices on the tick grid", f"{1 - Q.offtick_close.max():.3%}", ">= 99% (else fix tick in config)", Q.offtick_close.max() <= 0.01)
    r.gate("sessions per segment", int(SP.sessions.min()), ">= 250", SP.sessions.min() >= 250)
    return r.save(dict(test_hash=hashes))

# ============================================================================= P1
def P1(ctx):
    require(ctx, "P0")
    r = Report(ctx.out, "P1", "Registry fix and engine verification")
    r.add("did", "1) Ran the engine/signal self-tests on synthetic data (execution rules, costs, session/roll handling, "
                 "short = exact mirror of long, no look-ahead in any of the 32 triggers / 28 filters). "
                 "2) Rebuilt the candidate registry with executable definitions and a deterministic spec hash. "
                 "3) Measured how much the triggers overlap on Dev bars (no returns are looked at in P1).")
    T = pd.DataFrame(selftest.run_all(), columns=["test", "passed", "detail"])
    r.table("found", T[["test", "passed"]], "selftests", 50)
    raw = pd.read_csv((ctx.base / ctx.cfg["registry"]).resolve())
    if ctx.cfg.get("registry_limit"):                      # demo only
        raw = raw.sample(ctx.cfg["registry_limit"], random_state=ctx.cfg["seed"]).reset_index(drop=True)
    unknown = int((~raw["trigger"].isin(core.TRIGGER_NAMES) | ~raw["filter"].isin(core.FILTER_NAMES)).sum())
    fx = core.canonical_registry(raw)
    fx.to_csv(r.dir / "candidate_registry_fixed.csv", index=False)
    fl = fx["registry_flags"].str.split(";").explode().loc[lambda s: s != ""].value_counts().rename_axis("flag").reset_index(name="rows")
    r.add("changed", "Wrote `candidate_registry_fixed.csv`: added `trigger_def`/`filter_def` (exact formulas used by the code "
                     "and by the Pine script), `cell_id` = trigger|filter|side, `spec_hash` (sha1 of the executable spec), "
                     "`uses_t3_recomputed`, and `registry_flags`. T3 length/vf are treated as a tunable axis of the cell "
                     "(tuned in P3) instead of 5000 frozen random draws.")
    r.table("changed", fl, "registry_flags")
    cells = fx.groupby("cell_id").size()
    r.add("found", f"{len(fx)} rows = {cells.size} distinct trigger x filter x side cells "
                   f"({(cells > 1).sum()} cells appear with several T3 settings; max {cells.max()}). "
                   f"{int((~fx.uses_t3_recomputed).sum())} rows do not use T3 at all.")
    comp = fx.pivot_table(index="trigger", columns="filter", values="candidate_id", aggfunc="count").fillna(0)
    fig, a = plt.subplots(figsize=(12, 9)); im = a.imshow(comp.to_numpy(), cmap="viridis", aspect="auto")
    a.set_xticks(range(comp.shape[1]), comp.columns, rotation=90, fontsize=7); a.set_yticks(range(comp.shape[0]), comp.index, fontsize=7)
    fig.colorbar(im); r.fig(fig, "registry_composition", "Registry rows per trigger x filter")
    # trigger overlap on Dev bars (long side, reference T3 = 3 / 0.9 = the original script default)
    J = np.zeros((len(core.TRIGGER_NAMES),) * 2)
    for sym in ctx.syms:
        df, b, m, seg, blk = market(ctx, sym)
        bs, be = seg["dev"][:2]
        sigs = [core.trigger(b, t, 1, 3, 0.9)[bs:be] for t in core.TRIGGER_NAMES]
        for i_, j_ in itertools.product(range(len(sigs)), repeat=2):
            u = (sigs[i_] | sigs[j_]).sum(); J[i_, j_] += ((sigs[i_] & sigs[j_]).sum() / u if u else 0) / len(ctx.syms)
    fig, a = plt.subplots(figsize=(10, 9)); im = a.imshow(J, cmap="magma", vmin=0, vmax=1)
    a.set_xticks(range(len(J)), core.TRIGGER_NAMES, rotation=90, fontsize=7); a.set_yticks(range(len(J)), core.TRIGGER_NAMES, fontsize=7)
    fig.colorbar(im); r.fig(fig, "trigger_overlap", "Jaccard overlap of trigger signals on Dev (1 = identical entries)")
    hi = [(core.TRIGGER_NAMES[i_], core.TRIGGER_NAMES[j_], J[i_, j_]) for i_ in range(len(J)) for j_ in range(i_ + 1, len(J)) if J[i_, j_] >= 0.5]
    if hi:
        r.table("found", pd.DataFrame(hi, columns=["trigger_a", "trigger_b", "jaccard"]).sort_values("jaccard", ascending=False),
                "near_duplicate_triggers")
    r.gate("engine/signal self-tests", f"{T.passed.sum()}/{len(T)}", "all pass", T.passed.all())
    r.gate("registry rows with unknown trigger/filter", unknown, "0", unknown == 0)
    r.gate("unique executable specs", fx.spec_hash.nunique(), f"== {len(fx)}", fx.spec_hash.nunique() == len(fx))
    r.gate("side values valid", int((~fx.side.isin(["long", "short"])).sum()), "0", fx.side.isin(["long", "short"]).all())
    return r.save()

# ============================================================================= P2
def _metrics(tr, ps, blk, nb):
    x = tr["r_atr"]; n = len(x)
    if n == 0:
        return dict(n=0, mean_r=0.0, win=0.0, pf=0.0, mfe_med=0.0, mae_med=0.0, sess_mean=0.0, sr_sess=0.0, t_nw=0.0, p=1.0, pos_blocks=0)
    neg = -x[x < 0].sum()
    t = S.nw_tstat(ps)
    return dict(n=n, mean_r=float(x.mean()), win=float((x > 0).mean()), pf=float(x[x > 0].sum() / neg) if neg > 0 else np.inf,
                mfe_med=float(np.median(tr["mfe_atr"])), mae_med=float(np.median(tr["mae_atr"])), sess_mean=float(ps.mean()),
                sr_sess=S.sharpe(ps), t_nw=t, p=float(1 - sst.norm.cdf(t)),
                pos_blocks=int((np.bincount(blk, weights=ps, minlength=nb) > 0).sum()))

def P2(ctx):
    require(ctx, "P1")
    c = ctx.cfg["p2_screen"]; nb = ctx.cfg["split"]["dev_blocks"]
    r = Report(ctx.out, "P2", "Screen every candidate on every instrument (Dev only)")
    reg = load_registry(ctx); recs = reg.to_dict("records")
    r.add("did", f"Ran all {len(recs)} registry rows on all {len(ctx.syms)} instruments = {len(recs) * len(ctx.syms)} units, "
                 f"Dev segment only, with ONE fixed exit for everybody (pure time exit after {c['horizon']} bars, no TP/SL, "
                 "full costs). A fixed exit means the screen ranks ENTRIES, not exit tuning. Per unit: trades, net mean R "
                 "(ATR units), win rate, profit factor, median MFE/MAE, Newey-West t of the per-session series "
                 "(zeros on no-trade sessions), and positive Dev blocks. Per instrument: K_eff (effective number of "
                 "independent candidates), variance of Sharpe across candidates (feeds the DSR in P4), Hansen SPA p "
                 "(does ANY candidate beat zero after data-snooping?) and PBO of picking the Dev winner.")
    r.add("tried", f"{len(recs) * len(ctx.syms)} units, 1 exit. Every unit counts as a trial.")
    rows, uni, entries = [], {}, {}
    for sym in ctx.syms:
        df, b, m, seg, blk_s = market(ctx, sym)
        bs, be, ss, se = seg["dev"]; blk = blk_s[ss:se]
        M = np.zeros((se - ss, len(recs)), np.float32)
        for j, rec in enumerate(recs):
            tr = m.trades(core.candidate_signal(b, rec), side_of(rec), 0, 0, c["horizon"], bs, be)
            ps = m.per_session(tr)[ss:se]; M[:, j] = ps
            met = _metrics(tr, ps, blk, nb)
            met.update(candidate_id=rec["candidate_id"], symbol=sym, family=rec["family"], trigger=rec["trigger"],
                       filter=rec["filter"], side=rec["side"], t3_length=rec["t3_length"], t3_vf=rec["t3_vf"])
            rows.append(met)
            if met["n"] >= c["min_trades"] and ((met["t_nw"] >= c["t_min"] and met["mean_r"] > 0) or ctx.demo):
                entries[unit_key(rec["candidate_id"], sym)] = set(tr["entry_bar"].tolist())
        M = M.astype(float)
        sd = M.std(0, ddof=1); srs = np.where(sd > 0, M.mean(0) / np.where(sd > 0, sd, 1), 0)
        uni[sym] = dict(N=len(recs), K_eff=S.k_eff(M), var_sr=float(np.var(srs)), spa_p=S.spa_pvalue(M, c["spa_reps"]),
                        pbo=S.pbo_cscv(M, c["pbo_blocks"]), dev_sessions=int(se - ss), best_sr=float(srs.max()))
        print(f"  P2 {sym}: {uni[sym]}")
    U = pd.DataFrame(rows)
    disc, q = S.bh(U["p"].to_numpy(), c["bh_q"]); U["q_bh"] = q
    U["g_trades"] = U.n >= c["min_trades"]; U["g_mean"] = U.mean_r > 0; U["g_t"] = U.t_nw >= c["t_min"]
    U["g_bh"] = disc; U["g_blocks"] = U.pos_blocks >= c["min_pos_blocks"]; U["g_mfe_mae"] = U.mfe_med > U.mae_med
    gcols = ["g_trades", "g_mean", "g_t", "g_bh", "g_blocks", "g_mfe_mae"]
    U["pass"] = U[gcols].all(1)
    if ctx.demo and not U["pass"].any():
        U["pass"] = U.n >= c["min_trades"]
    cand = U[U["pass"]].sort_values("t_nw", ascending=False)
    kept = []
    for sym in ctx.syms:
        sets = []
        for idx, row in cand[cand.symbol == sym].iterrows():
            e = entries.get(unit_key(row.candidate_id, sym))
            if e is None:
                continue
            if all(len(e & k) / max(1, len(e | k)) <= c["jaccard_max"] for k in sets):
                sets.append(e); kept.append(idx)
    SLt = U.loc[kept].sort_values("t_nw", ascending=False).head(c["shortlist_max"])
    U.to_csv(r.dir / "units.csv", index=False)
    UN = pd.DataFrame(uni).T.rename_axis("symbol").reset_index()
    exp = len(U) * (1 - sst.norm.cdf(c["t_min"]))
    r.add("found", f"Units with t >= {c['t_min']}: {int((U.t_nw >= c['t_min']).sum())} (pure luck would give ~{exp:.0f}). "
                   f"Passing every P2 gate: {int(U['pass'].sum())}. After removing overlapping entries (Jaccard > "
                   f"{c['jaccard_max']}): {len(kept)}. Shortlist kept: {len(SLt)}.")
    r.table("found", UN, "universe")
    r.table("found", pd.DataFrame({"gate": gcols, "units_passing": [int(U[g].sum()) for g in gcols]}), "gate_funnel")
    r.table("found", SLt[["candidate_id", "symbol", "family", "trigger", "filter", "side", "t3_length", "t3_vf", "n",
                          "mean_r", "win", "pf", "t_nw", "q_bh", "pos_blocks"]], "shortlist", 50)
    # plots
    fig, a = plt.subplots(1, 2, figsize=(12, 4))
    xs = np.linspace(-6, 6, 300)
    for sym in ctx.syms:
        a[0].hist(U[U.symbol == sym].t_nw.clip(-8, 8), bins=80, histtype="step", density=True, label=sym)
    a[0].plot(xs, sst.norm.pdf(xs), "k--", label="N(0,1) = no edge"); a[0].legend(fontsize=7); a[0].set_title("t-stat of every unit")
    a[1].scatter(U.mean_r, -np.log10(U.p.clip(1e-12)), s=2, c=np.where(U["pass"], "tab:red", "0.7"))
    a[1].set_xlabel("net mean R per trade (ATR)"); a[1].set_ylabel("-log10 p"); a[1].set_title("volcano (red = passed P2)")
    r.fig(fig, "screen_distribution", "If the t histogram looks like N(0,1) shifted left by costs, the universe has no edge")
    hm = U.assign(sig=U.t_nw >= c["t_min"]).pivot_table(index="family", columns="symbol", values="sig", aggfunc="mean")
    fig, a = plt.subplots(figsize=(7, 4.5)); im = a.imshow(hm.to_numpy(), cmap="Reds", vmin=0)
    a.set_xticks(range(hm.shape[1]), hm.columns); a.set_yticks(range(hm.shape[0]), hm.index, fontsize=7); fig.colorbar(im)
    r.fig(fig, "family_heatmap", f"Share of units with t >= {c['t_min']} by family and instrument (luck ~ {1 - sst.norm.cdf(c['t_min']):.3f})")
    fig, a = plt.subplots(1, 2, figsize=(12, 4))
    a[0].scatter(U.mae_med, U.mfe_med, s=2, c=np.where(U["pass"], "tab:red", "0.7")); a[0].plot([0, 3], [0, 3], "k--")
    a[0].set_xlabel("median MAE (ATR)"); a[0].set_ylabel("median MFE (ATR)"); a[0].set_title(f"MFE vs MAE over {c['horizon']} bars")
    for sym, g in SLt.head(12).groupby("symbol"):
        df, b, m, seg, _ = market(ctx, sym); bs, be = seg["dev"][:2]
        for _, row in g.iterrows():
            rec = reg[reg.candidate_id == row.candidate_id].iloc[0].to_dict()
            sig = core.candidate_signal(b, rec)
            prof = [m.trades(sig, side_of(rec), 0, 0, h, bs, be)["r_atr"].mean() for h in c["horizons"]]
            a[1].plot(c["horizons"], prof, marker="o", ms=3, label=f"{row.candidate_id} {sym}")
    a[1].axhline(0, color="k", lw=0.5); a[1].set_xlabel("bars held"); a[1].set_ylabel("net mean R"); a[1].set_title("horizon profile (shortlist)")
    a[1].legend(fontsize=6)
    r.fig(fig, "mfe_mae_horizons", "Left: excursions of every unit. Right: does the edge build with time or decay?")
    r.gate("shortlist not empty", len(SLt), ">= 1", len(SLt) >= 1)
    return r.save(dict(universe=uni))

# ============================================================================= P3
class Tuner:
    """Evaluates (L, V, sl_atr, tp_r, max_bars) configs on Dev; caches per-session series for the WF folds and PBO."""
    def __init__(self, ctx, b, m, rec, seg, blk):
        self.c = ctx.cfg["p3_refine"]; self.h = ctx.cfg["p2_screen"]["horizon"]; self.seed = ctx.cfg["seed"]
        self.b, self.m, self.rec, self.side = b, m, rec, side_of(rec)
        self.bs, self.be, self.ss, self.se = seg["dev"]; self.blk = blk[self.ss:self.se]
        self.t3 = bool(rec["uses_t3_recomputed"]); self.cache = {}
        self.LV = list(itertools.product(self.c["t3_length"], self.c["t3_vf"])) if self.t3 else [(rec["t3_length"], rec["t3_vf"])]
        self.shapeA = (len(self.c["t3_length"]), len(self.c["t3_vf"])) if self.t3 else (1, 1)
        self.EX = list(itertools.product(self.c["sl_atr"], self.c["tp_r"], self.c["max_bars"]))
        self.shapeB = (len(self.c["sl_atr"]), len(self.c["tp_r"]), len(self.c["max_bars"]))

    def ev(self, key):
        if key not in self.cache:
            L, V, sl, tp, hb = key
            tr = self.m.trades(core.candidate_signal(self.b, with_params(self.rec, L, V)), self.side, sl, tp, hb, self.bs, self.be)
            ps = self.m.per_session(tr)[self.ss:self.se]
            cnt = np.bincount(self.m.sess[tr["entry_bar"]] - self.ss, minlength=self.se - self.ss)
            self.cache[key] = (ps, cnt)
        return self.cache[key]

    def score(self, key, train):
        """25th percentile over the training blocks of the block's mean net R per session."""
        ps, cnt = self.ev(key)
        if cnt[np.isin(self.blk, train)].sum() < self.c["min_trades_train"]:
            return -np.inf
        means = [ps[self.blk == k].mean() for k in train]
        return float(np.quantile(means, 0.25)) if len(means) > 1 else float(means[0])

    @staticmethod
    def plateau(sc):
        x = np.where(np.isfinite(sc), sc, np.nan)
        with warnings.catch_warnings():
            warnings.simplefilter("ignore", RuntimeWarning)
            sm = generic_filter(x, np.nanmean, size=3, mode="constant", cval=np.nan)
        return np.where(np.isfinite(x), sm, -np.inf)

    def choose_grid(self, train):
        A = [(L, V, 0.0, 0.0, self.h) for L, V in self.LV]
        sa = np.array([self.score(k, train) for k in A]).reshape(self.shapeA)
        L, V = A[int(np.argmax(self.plateau(sa)))][:2]
        B = [(L, V, s, t, hb) for s, t, hb in self.EX]
        sb = np.array([self.score(k, train) for k in B]).reshape(self.shapeB)
        return B[int(np.argmax(self.plateau(sb)))], sb

    def choose_optuna(self, train):
        import optuna
        optuna.logging.set_verbosity(optuna.logging.WARNING)
        c = self.c
        def obj(t):
            L, V = t.suggest_categorical("LV", [f"{a}|{b}" for a, b in self.LV]).split("|")
            key = (float(L), float(V), t.suggest_categorical("sl", c["sl_atr"]), t.suggest_categorical("tp", c["tp_r"]),
                   t.suggest_categorical("hb", c["max_bars"]))
            v = self.score(key, train)
            return v if np.isfinite(v) else -1e9
        st_ = optuna.create_study(direction="maximize", sampler=optuna.samplers.TPESampler(seed=self.seed))
        st_.optimize(obj, n_trials=c["optuna_trials"])
        p = st_.best_params; L, V = p["LV"].split("|")
        return (float(L), float(V), p["sl"], p["tp"], p["hb"]), None

    def choose(self, train):
        return self.choose_optuna(train) if self.c["search"] == "optuna" else self.choose_grid(train)

def P3(ctx):
    require(ctx, "P2")
    c = ctx.cfg["p3_refine"]; nb = ctx.cfg["split"]["dev_blocks"]
    r = Report(ctx.out, "P3", "Refine the shortlist: T3 settings and exits, walk-forward inside Dev")
    SLt = pd.read_csv(ctx.out / "P2" / "shortlist.csv"); reg = load_registry(ctx).set_index("candidate_id")
    (r.dir / "units").mkdir(exist_ok=True)
    if SLt.empty:
        return _empty(r)
    r.add("did", f"For each shortlisted unit: {nb - 1} expanding walk-forward folds inside Dev (train = blocks before k, "
                 "test = block k). In each fold the parameters are chosen on the train blocks only, then applied untouched to "
                 "block k; the stitched block-k results are the walk-forward OOS series used by P4. Search "
                 f"({c['search']}): stage A = T3 length x vf on the fixed P2 exit (T3 units only), stage B = SL (ATR) x TP (R) x "
                 "max bars on the chosen T3. Objective = 25th percentile of per-block mean net R (robust to one lucky block). "
                 "Grid choices are plateau-smoothed (3x3(x3) neighbourhood mean) so an isolated spike cannot win. "
                 "Final parameters = same rule on all Dev blocks. Every evaluated config is counted and fed to PBO.")
    r.add("tried", f"T3 grid {len(c['t3_length'])}x{len(c['t3_vf'])}, exit grid {len(c['sl_atr'])}x{len(c['tp_r'])}x{len(c['max_bars'])}"
                   f" = {len(c['sl_atr']) * len(c['tp_r']) * len(c['max_bars'])}.")
    out, folds_all, heat = [], [], []
    for sym, grp in SLt.groupby("symbol"):
        df, b, m, seg, blk = market(ctx, sym)
        for _, u in grp.iterrows():
            rec = reg.loc[u.candidate_id].to_dict(); rec["candidate_id"] = u.candidate_id
            tu = Tuner(ctx, b, m, rec, seg, blk)
            oos = np.full(len(tu.blk), np.nan); fsum = []
            for k in range(1, nb):
                key, _ = tu.choose(list(range(k)))
                ps, cnt = tu.ev(key); mk = tu.blk == k
                oos[mk] = ps[mk]; fsum.append(ps[mk].sum())
                folds_all.append(dict(candidate_id=u.candidate_id, symbol=sym, fold=k, t3_length=key[0], t3_vf=key[1],
                                      sl_atr=key[2], tp_r=key[3], max_bars=key[4], oos_sum_r=ps[mk].sum(), oos_trades=int(cnt[mk].sum())))
            final, sb = tu.choose(list(range(nb)))
            Mx = np.column_stack([v[0] for v in tu.cache.values()])
            pbo = S.pbo_cscv(Mx, 16)
            x = oos[tu.blk >= 1]; ntr = int(sum(f["oos_trades"] for f in folds_all[-(nb - 1):]))
            np.savez(r.dir / "units" / f"{unit_key(u.candidate_id, sym)}.npz", oos=oos, fold_sums=np.array(fsum))
            out.append(dict(candidate_id=u.candidate_id, symbol=sym, uses_t3=tu.t3, t3_length=final[0], t3_vf=final[1],
                            sl_atr=final[2], tp_r=final[3], max_bars=final[4], n_configs=len(tu.cache), pbo=pbo,
                            oos_mean_sess=float(np.mean(x)), oos_trades=ntr, oos_pos_folds=int((np.array(fsum) > 0).sum())))
            if sb is not None and len(heat) < 12:
                heat.append((f"{u.candidate_id} {sym}", sb[:, :, ctx.cfg["p3_refine"]["max_bars"].index(final[4])], final))
            print(f"  P3 {u.candidate_id} {sym}: {out[-1]}")
    R = pd.DataFrame(out); F = pd.DataFrame(folds_all)
    R["pass"] = (R.oos_trades >= c["min_oos_trades"]) & (R.oos_mean_sess > 0)
    if ctx.demo and not R["pass"].any():
        R["pass"] = True
    r.table("found", R, "refined", 60)
    r.table("found", F, "wf_folds", 60)
    r.add("changed", "Parameters per unit replaced the registry's random T3 draw and the fixed P2 exit (see `refined.csv`). "
                     "`wf_folds.csv` shows whether the chosen parameters are stable from fold to fold.")
    if heat:
        n = len(heat); fig, ax = plt.subplots((n + 3) // 4, 4, figsize=(14, 3.2 * ((n + 3) // 4)), squeeze=False)
        for a, (name, sc, fin) in zip(ax.flat, heat):
            im = a.imshow(np.where(np.isfinite(sc), sc, np.nan), cmap="RdYlGn", origin="lower", aspect="auto")
            a.set_xticks(range(len(c["tp_r"])), c["tp_r"], fontsize=6); a.set_yticks(range(len(c["sl_atr"])), c["sl_atr"], fontsize=6)
            a.set_xlabel("TP (R)", fontsize=7); a.set_ylabel("SL (ATR)", fontsize=7); a.set_title(f"{name} hold={fin[4]}", fontsize=7)
            a.plot(c["tp_r"].index(fin[3]), c["sl_atr"].index(fin[2]), "k*", ms=10); fig.colorbar(im, ax=a)
        for a in list(ax.flat)[n:]:
            a.axis("off")
        r.fig(fig, "exit_surfaces", "Dev score surface SL x TP at the chosen hold (star = chosen). Broad green plateau = good; a lone green cell = overfit")
    if len(F):
        fig, a = plt.subplots(figsize=(12, 3.8))
        piv = F.pivot_table(index=F.candidate_id + " " + F.symbol, columns="fold", values="oos_sum_r")
        piv.head(20).plot.bar(ax=a, width=0.8); a.axhline(0, color="k", lw=0.5); a.set_ylabel("OOS net R in fold")
        a.tick_params(axis="x", labelsize=6)
        r.fig(fig, "wf_folds", "Walk-forward OOS result per fold (each bar = parameters chosen only on earlier blocks)")
    r.gate("units with >= min OOS trades and positive WF-OOS mean", int(R["pass"].sum()), ">= 1", R["pass"].sum() >= 1)
    return r.save()

# ============================================================================= P4
def _bucket_check(trades_r, labels, min_n, reps):
    """Per bucket with >= min_n trades: mean and the 97.5% bootstrap upper bound of the mean."""
    res = []
    for lab in pd.unique(labels):
        x = trades_r[labels == lab]
        if len(x) >= min_n:
            res.append((lab, len(x), float(x.mean()), S.boot_upper(x, reps)))
    return res

def P4(ctx):
    require(ctx, "P3")
    c = ctx.cfg["p4_evidence"]; nb = ctx.cfg["split"]["dev_blocks"]; c3 = ctx.cfg["p3_refine"]
    r = Report(ctx.out, "P4", "Evidence on Dev: selection bias, null tests, robustness, stability")
    R = pd.read_csv(ctx.out / "P3" / "refined.csv"); R = R[R["pass"]]
    if R.empty:
        return _empty(r)
    uni = json.load(open(ctx.out / "P2" / "gates.json"))["universe"]
    reg = load_registry(ctx).set_index("candidate_id")
    r.add("did", "On the walk-forward OOS series from P3: stationary-bootstrap CI of the per-session mean, Deflated Sharpe "
                 "(N = K_eff of the instrument's universe + configs tried in P3), PBO of the P3 search, Hansen SPA of the "
                 "universe, fold dispersion. With the final parameters on Dev: circular-session-shift null (same signals, "
                 "random timing, same time of day), cost x1.5, +1 tick slippage per side, +1 bar entry delay, parameter "
                 "neighbours, top-5% trades removed, best year removed, and stability by year / volatility regime / session "
                 "bucket (no bucket with >= 30 trades may be significantly negative).")
    rows, curves, nulls = [], {}, {}
    rng = np.random.default_rng(ctx.cfg["seed"])
    for sym, grp in R.groupby("symbol"):
        df, b, m, seg, blk_s = market(ctx, sym)
        bs, be, ss, se = seg["dev"]; blk = blk_s[ss:se]
        reg_lab = regime(b); bucket = df["bucket"].to_numpy(); year = df["ts"].dt.year.to_numpy()
        bps = int(df[df.seg == "dev"].groupby("sess").size().median())
        for _, u in grp.iterrows():
            rec = reg.loc[u.candidate_id].to_dict(); side = side_of(rec)
            z = np.load(ctx.out / "P3" / "units" / f"{unit_key(u.candidate_id, sym)}.npz")
            x = z["oos"][blk >= 1]; fs = z["fold_sums"]
            bt = S.boot_mean(x, c["boot_reps"], ctx.cfg["seed"])
            N = int(uni[sym]["K_eff"]) + int(u.n_configs)
            dsr = S.dsr(x, uni[sym]["var_sr"], N)
            sig = core.candidate_signal(b, with_params(rec, u.t3_length, u.t3_vf))
            P = dict(sl_atr=u.sl_atr, tp_r=u.tp_r, max_bars=int(u.max_bars), start=bs, end=be)
            tr = m.trades(sig, side, **P)
            mean_dev = lambda t: float(m.per_session(t)[ss:se].mean())
            obs = mean_dev(tr)
            nd = []
            for _ in range(c["null_reps"]):
                sh = int(rng.integers(5, max(6, se - ss - 5))) * bps
                s2 = sig.copy(); s2[bs:be] = np.roll(sig[bs:be], sh)
                nd.append(mean_dev(m.trades(s2, side, **P)))
            p_null = S.emp_p(nd, obs)
            nb_vals = []
            for ax_name, grid in (("sl_atr", c3["sl_atr"]), ("tp_r", c3["tp_r"]), ("max_bars", c3["max_bars"]),
                                  ("t3_length", c3["t3_length"] if u.uses_t3 else None), ("t3_vf", c3["t3_vf"] if u.uses_t3 else None)):
                if grid is None:
                    continue
                i0 = int(np.argmin(np.abs(np.array(grid, float) - float(u[ax_name]))))
                for j in (i0 - 1, i0 + 1):
                    if 0 <= j < len(grid):
                        q = dict(t3_length=u.t3_length, t3_vf=u.t3_vf, sl_atr=u.sl_atr, tp_r=u.tp_r, max_bars=u.max_bars)
                        q[ax_name] = grid[j]
                        sg = core.candidate_signal(b, with_params(rec, q["t3_length"], q["t3_vf"]))
                        nb_vals.append(mean_dev(m.trades(sg, side, q["sl_atr"], q["tp_r"], int(q["max_bars"]), bs, be)))
            rr = tr["r_atr"]; srt = np.sort(rr)[::-1]
            trim = srt[int(np.ceil(c["top_trim"] * len(srt))):].mean() if len(srt) > 20 else -np.inf
            yr = year[tr["entry_bar"]]; ysum = pd.Series(rr).groupby(yr).sum()
            stab = {}
            for dim, lab in (("year", yr), ("regime", reg_lab[tr["sig_bar"]]), ("session", bucket[tr["entry_bar"]])):
                bk = _bucket_check(rr, lab, c["min_bucket_trades"], 500)
                stab[dim] = dict(sig_negative=sum(1 for q_ in bk if q_[3] < 0), negative=sum(1 for q_ in bk if q_[2] < 0), n=len(bk))
            pos_f = fs > 0; tot_pos = fs[pos_f].sum()
            g = dict(
                g_ci_lo=bt["lo"] > 0, g_dsr=dsr >= c["dsr_min"], g_null=p_null <= c["null_p_max"], g_pbo=u.pbo <= c["pbo_max"],
                g_spa=uni[sym]["spa_p"] <= c["spa_p_max"], g_wf_pos=pos_f.mean() >= c["wf_pos_frac"],
                g_wf_share=(fs.max() / tot_pos if tot_pos > 0 else 1) <= c["fold_share_max"],
                g_cost=mean_dev(m.trades(sig, side, cost_mult=c["cost_mult"], **P)) > 0,
                g_slip=mean_dev(m.trades(sig, side, extra_slip=c["extra_slip_ticks"], **P)) > 0,
                g_delay=mean_dev(m.trades(sig, side, delay=c["delay_bars"], **P)) > 0,
                g_neighbours=(np.median(nb_vals) > 0) if nb_vals else False, g_top_trim=trim > 0,
                g_best_year=(ysum.sum() - ysum.max()) > 0,
                g_years=(ysum > 0).mean() >= c["pos_years_frac"],
                g_stable=all(v["sig_negative"] == 0 for v in stab.values()) and stab["regime"]["negative"] <= 1 and stab["session"]["negative"] <= 1)
            row = dict(candidate_id=u.candidate_id, symbol=sym, wf_mean=bt["mean"], ci_lo=bt["lo"], ci_hi=bt["hi"],
                       p_boot=bt["p"], t_nw=S.nw_tstat(x), sr_sess=S.sharpe(x), dsr=dsr, N_trials=N, p_null=p_null,
                       pbo=u.pbo, spa_p=uni[sym]["spa_p"], dev_trades=len(rr), neighbours_median=float(np.median(nb_vals)) if nb_vals else np.nan,
                       **{k: bool(v) for k, v in g.items()})
            row["pass"] = all(g.values()); rows.append(row)
            curves[f"{u.candidate_id} {sym}"] = np.cumsum(x); nulls[f"{u.candidate_id} {sym}"] = (np.array(nd), obs)
            print(f"  P4 {u.candidate_id} {sym}: pass={row['pass']}")
    E = pd.DataFrame(rows)
    gcols = [k for k in E.columns if k.startswith("g_")]
    if ctx.demo and not E["pass"].any():
        E["pass"] = True
    r.table("found", E, "evidence", 60)
    E[E["pass"]].to_csv(r.dir / "survivors.csv", index=False)
    r.table("found", pd.DataFrame({"gate": gcols, "units_passing": [int(E[g].sum()) for g in gcols]}), "gate_funnel")
    fig, a = plt.subplots(1, 2, figsize=(13, 4))
    for k, v in list(curves.items())[:15]:
        a[0].plot(v, lw=0.8, label=k)
    a[0].axhline(0, color="k", lw=0.5); a[0].set_title("walk-forward OOS equity (net R)"); a[0].legend(fontsize=6)
    a[1].errorbar(E.wf_mean, range(len(E)), xerr=[E.wf_mean - E.ci_lo, E.ci_hi - E.wf_mean], fmt="o", ms=3)
    a[1].axvline(0, color="k", lw=0.5); a[1].set_yticks(range(len(E)), E.candidate_id + " " + E.symbol, fontsize=6)
    a[1].set_title("95% stationary-bootstrap CI of WF-OOS mean per session")
    r.fig(fig, "wf_equity_ci", "Walk-forward OOS equity and confidence intervals")
    fig, a = plt.subplots(figsize=(10, 0.3 * len(E) + 2))
    a.imshow(E[gcols].to_numpy(float), cmap="RdYlGn", vmin=0, vmax=1, aspect="auto")
    a.set_xticks(range(len(gcols)), [g[2:] for g in gcols], rotation=60, fontsize=7)
    a.set_yticks(range(len(E)), E.candidate_id + " " + E.symbol, fontsize=6)
    r.fig(fig, "gate_matrix", "Gate matrix (green = pass). A unit continues only if its whole row is green")
    k6 = list(nulls.items())[:6]
    if k6:
        fig, ax = plt.subplots(1, len(k6), figsize=(3 * len(k6), 2.8), squeeze=False)
        for a, (k, (nd, ob)) in zip(ax[0], k6):
            a.hist(nd, bins=40, color="0.6"); a.axvline(ob, color="r"); a.set_title(k, fontsize=7)
        r.fig(fig, "null_tests", "Circular-shift null (grey) vs actual (red): same signals, random timing")
    r.gate("units passing all evidence gates", int(E["pass"].sum()), ">= 1", E["pass"].sum() >= 1)
    return r.save()

# ============================================================================= P5 / P6
def _frozen(ctx, segname, units, reps, null_reps=0, upto="val"):
    reg = load_registry(ctx).set_index("candidate_id"); out = {}
    rng = np.random.default_rng(ctx.cfg["seed"] + 1)
    for sym, grp in units.groupby("symbol"):
        df, b, m, seg, _ = market(ctx, sym, upto)
        s0, s1, ss, se = seg[segname]
        bps = int(df[df.seg == segname].groupby("sess").size().median())
        for _, u in grp.iterrows():
            rec = reg.loc[u.candidate_id].to_dict(); side = side_of(rec)
            sig = core.candidate_signal(b, with_params(rec, u.t3_length, u.t3_vf))
            P = dict(sl_atr=u.sl_atr, tp_r=u.tp_r, max_bars=int(u.max_bars), start=s0, end=s1)
            tr = m.trades(sig, side, **P); ps = m.per_session(tr)[ss:se]
            bt = S.boot_mean(ps, reps, ctx.cfg["seed"])
            cost = float(m.per_session(m.trades(sig, side, cost_mult=1.5, **P))[ss:se].mean())
            p_null = np.nan
            if null_reps:
                nd = []
                for _ in range(null_reps):
                    s2 = sig.copy(); s2[s0:s1] = np.roll(sig[s0:s1], int(rng.integers(5, max(6, se - ss - 5))) * bps)
                    nd.append(m.per_session(m.trades(s2, side, **P))[ss:se].mean())
                p_null = S.emp_p(nd, ps.mean())
            full = m.trades(sig, side, u.sl_atr, u.tp_r, int(u.max_bars))
            out[(u.candidate_id, sym)] = dict(ps=ps, trades=len(tr["r_atr"]), mean_r=float(tr["r_atr"].mean()) if len(tr["r_atr"]) else 0.0,
                                               boot=bt, cost15=cost, p_null=p_null,
                                               curve=(df["ts"].to_numpy()[full["exit_bar"]], np.cumsum(full["r_atr"])),
                                               seg_ts={k: (df["ts"].iloc[v[0]], df["ts"].iloc[v[1] - 1]) for k, v in seg.items()})
    return out

def P5(ctx):
    require(ctx, "P4")
    c = ctx.cfg["p5_validation"]
    r = Report(ctx.out, "P5", "Validation segment (opened once, parameters frozen)")
    units = pd.merge(pd.read_csv(ctx.out / "P4" / "survivors.csv")[["candidate_id", "symbol", "wf_mean"]],
                     pd.read_csv(ctx.out / "P3" / "refined.csv").drop(columns="pass"), on=["candidate_id", "symbol"])
    r.add("did", f"Ran the {len(units)} P4 survivors with their frozen final parameters on the Validation segment. No "
                 "parameter is changed here. Tests: stationary bootstrap of the per-session mean (one-sided p, "
                 f"Benjamini-Hochberg q <= {c['bh_q']} across survivors), circular-shift null, cost x1.5.")
    if units.empty:
        return _empty(r)
    res = _frozen(ctx, "val", units, c["boot_reps"], c["null_reps"])
    rows = []
    for (cid, sym), v in res.items():
        rows.append(dict(candidate_id=cid, symbol=sym, val_trades=v["trades"], val_mean_r=v["mean_r"], val_sess_mean=v["boot"]["mean"],
                         ci_lo=v["boot"]["lo"], ci_hi=v["boot"]["hi"], p_boot=v["boot"]["p"], p_null=v["p_null"], cost15=v["cost15"],
                         sr_sess=S.sharpe(v["ps"])))
    V = pd.DataFrame(rows)
    disc, q = S.bh(V.p_boot.to_numpy(), c["bh_q"]); V["q_bh"] = q
    V["g_trades"] = V.val_trades >= c["min_trades"]; V["g_mean"] = V.val_sess_mean > 0; V["g_bh"] = disc
    V["g_null"] = V.p_null <= 0.05; V["g_cost"] = V.cost15 > 0
    V["pass"] = V[["g_trades", "g_mean", "g_bh", "g_null", "g_cost"]].all(1)
    if ctx.demo and not V["pass"].any():
        V["pass"] = True
    V = V.merge(units, on=["candidate_id", "symbol"])
    r.table("found", V, "validation", 60)
    V[V["pass"]].to_csv(r.dir / "survivors.csv", index=False)
    fig, a = plt.subplots(1, 2, figsize=(13, 4))
    for (cid, sym), v in list(res.items())[:12]:
        t, y = v["curve"]; a[0].plot(t, y, lw=0.8, label=f"{cid} {sym}")
    if res:
        v = next(iter(res.values())); a[0].axvspan(*v["seg_ts"]["val"], color="tab:orange", alpha=0.15)
    a[0].set_title("frozen parameters: Dev (in-sample) then Validation (orange)"); a[0].legend(fontsize=6)
    a[1].errorbar(V.val_sess_mean, range(len(V)), xerr=[V.val_sess_mean - V.ci_lo, V.ci_hi - V.val_sess_mean], fmt="o", ms=3)
    a[1].axvline(0, color="k", lw=0.5); a[1].set_yticks(range(len(V)), V.candidate_id + " " + V.symbol, fontsize=6)
    a[1].set_title("Validation 95% CI (per session)")
    r.fig(fig, "validation", "Equity through Validation and the Validation confidence intervals")
    r.gate("units passing validation", int(V["pass"].sum()), ">= 1", V["pass"].sum() >= 1)
    return r.save()

def P6(ctx, force=False):
    require(ctx, "P5")
    c = ctx.cfg["p6_test"]
    r = Report(ctx.out, "P6", "Sealed test (opened once) and deployment cards")
    units = pd.read_csv(ctx.out / "P5" / "survivors.csv")
    if units.empty:
        return _empty(r)
    lock = r.dir / "LOCK"; sig_ = hashlib.sha1(units[["candidate_id", "symbol"]].to_csv().encode()).hexdigest()
    if lock.exists() and lock.read_text().strip() != sig_ and not force:
        raise SystemExit("The sealed test was already opened for a DIFFERENT set of units. Re-running on new choices is "
                         "data snooping on the test. Use --force only if you accept the test is contaminated.")
    contaminated = lock.exists() and lock.read_text().strip() != sig_
    lock.write_text(sig_)
    r.add("did", f"Ran the {len(units)} P5 survivors once on the sealed Test segment with frozen parameters. "
                 "Gates: >= min trades, 95% bootstrap CI lower bound > 0, Deflated Sharpe vs the number of units that "
                 "reached the test, cost x1.5 still positive.")
    if contaminated:
        r.add("found", "**WARNING: test re-opened for a different unit set — results are contaminated.**")
    res = _frozen(ctx, "test", units, c["boot_reps"], 0, upto="all")
    srs = [S.sharpe(v["ps"]) for v in res.values()]
    rows = []
    for (cid, sym), v in res.items():
        N = max(1, len(res))
        d = S.dsr(v["ps"], float(np.var(srs)) if N > 1 else 0.0, N) if N > 1 else S.psr(v["ps"], 0.0)
        rows.append(dict(candidate_id=cid, symbol=sym, test_trades=v["trades"], test_mean_r=v["mean_r"], test_sess_mean=v["boot"]["mean"],
                         ci_lo=v["boot"]["lo"], ci_hi=v["boot"]["hi"], p_boot=v["boot"]["p"], dsr=d, cost15=v["cost15"]))
    T = pd.DataFrame(rows)
    T["g_trades"] = T.test_trades >= c["min_trades"]; T["g_ci_lo"] = T.ci_lo > 0; T["g_dsr"] = T.dsr >= c["dsr_min"]; T["g_cost"] = T.cost15 > 0
    T["pass"] = T[["g_trades", "g_ci_lo", "g_dsr", "g_cost"]].all(1) & (not contaminated)
    T = T.merge(units[["candidate_id", "symbol", "t3_length", "t3_vf", "sl_atr", "tp_r", "max_bars"]], on=["candidate_id", "symbol"])
    r.table("found", T, "test", 60)
    reg = load_registry(ctx).set_index("candidate_id")
    cards = []
    for _, u in T[T["pass"]].iterrows():
        rec = reg.loc[u.candidate_id]; i = ctx.ins(u.symbol)
        cards.append(dict(candidate_id=u.candidate_id, symbol=u.symbol, Trigger=rec.trigger, Filter=rec["filter"], Side=rec.side.capitalize(),
                          T3_length=int(u.t3_length) if rec.uses_t3_recomputed else "-", T3_vf=u.t3_vf if rec.uses_t3_recomputed else "-",
                          ATR_length=ctx.cfg["atr_len"], SL_ATR=u.sl_atr, TP_R=u.tp_r, Max_bars=int(u.max_bars),
                          Commission_ticks_rt=i["cost_ticks_rt"]))
    if cards:
        r.table("found", pd.DataFrame(cards), "deployment_cards")
        r.add("found", "Put each card's values into `T3_Entry_Lab_v2.pine` (same names). Then paper/sim trade it: "
                       "incubation of at least 30 trades; stop it if the live mean R falls below the lower bound of the Test CI "
                       "or the live drawdown exceeds the worst Dev+Val+Test drawdown.")
    fig, a = plt.subplots(1, 2, figsize=(13, 4))
    for (cid, sym), v in list(res.items())[:12]:
        t, y = v["curve"]; a[0].plot(t, y, lw=0.8, label=f"{cid} {sym}")
    if res:
        v = next(iter(res.values()))
        a[0].axvspan(*v["seg_ts"]["val"], color="tab:orange", alpha=0.15); a[0].axvspan(*v["seg_ts"]["test"], color="tab:red", alpha=0.15)
    a[0].set_title("frozen parameters over the whole history (orange Val, red Test)"); a[0].legend(fontsize=6)
    a[1].errorbar(T.test_sess_mean, range(len(T)), xerr=[T.test_sess_mean - T.ci_lo, T.ci_hi - T.test_sess_mean], fmt="o", ms=3)
    a[1].axvline(0, color="k", lw=0.5); a[1].set_yticks(range(len(T)), T.candidate_id + " " + T.symbol, fontsize=6)
    a[1].set_title("Test 95% CI (per session)")
    r.fig(fig, "test", "Sealed test")
    r.gate("units passing the sealed test", int(T["pass"].sum()), ">= 1", T["pass"].sum() >= 1)
    return r.save()

PHASES = dict(P0=P0, P1=P1, P2=P2, P3=P3, P4=P4, P5=P5, P6=P6)
