"""Phases P0-P6. Each phase: steps -> gates -> out/<P>/report.md + gates.json + plots + tables.
A phase refuses to start unless the previous phase closed with PASS."""
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

# ----------------------------------------------------------------------------- context and helpers
class Ctx:
    def __init__(self, cfg_path):
        self.path = Path(cfg_path).resolve()
        self.base = self.path.parent
        self.cfg = yaml.safe_load(open(self.path))
        self.out = self.base / self.cfg["out_dir"]
        self.data = self.base / self.cfg["data_dir"]
        self.syms = list(self.cfg["instruments"])

    def ins(self, s):
        """Instrument settings with the traded contract's costs converted to ticks:
        cost_ticks_rt = fees_rt_usd / tick_value + 2 x slippage_ticks_per_side."""
        i = dict(self.cfg["instruments"][s])
        k = i[i.get("trade", "micro")]
        i.update(contract=k["symbol"], point_value=k["point_value"], tick_value=k["tick_value"], fees_rt_usd=k["fees_rt_usd"],
                 cost_ticks_rt=k["fees_rt_usd"] / k["tick_value"] + 2 * self.cfg["slippage_ticks_per_side"],
                 stop_slip_ticks=self.cfg["stop_slip_ticks"])
        return i

def require(ctx, phase):
    _require(ctx.out, phase)

def market(ctx, sym, upto="val"):
    """Load one instrument. Before P6 (upto='val') the Test rows are dropped and never seen."""
    df = pd.read_pickle(ctx.out / "cache" / f"{sym}.pkl")
    if upto == "val":
        df = df[df["seg"] != "test"].reset_index(drop=True)
    cs = ctx.cfg["session"]
    sl, rl = core.last_index_arrays(df, core.tradable_mask(df, cs["start_hour"], cs["bar_minutes"], ctx.cfg.get("flat_by")))
    i = ctx.ins(sym)
    b = core.Bars(df, ctx.cfg["atr_len"], i.get("rth_open", "08:30"), i.get("rth_close", "15:00"), cs["bar_minutes"])
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
    if r["uses_t3"]:
        r["t3_length"], r["t3_vf"] = L, V
    return r

def screen_bars(rec, default):
    v = rec.get("screen_bars") if hasattr(rec, "get") else None
    return int(v) if v is not None and v == v else int(default)

def load_registry(ctx):
    return pd.read_csv(ctx.out / "P1" / "registry.csv")

def unit_key(cid, sym):
    return f"{cid}__{sym}"

def segment_sessions(ctx):
    sp = pd.read_csv(ctx.out / "P0" / "splits.csv")
    return {(r.symbol, r.segment): int(r.sessions) for r in sp.itertuples()}

def regime_labels(b, dev_end, window):
    """Volatility regime of each bar: atr1 / median(atr1 over `window` bars), cut at the Dev terciles of that ratio."""
    ratio = b.atr1 / pd.Series(b.atr1).rolling(window, min_periods=window // 5).median().to_numpy()
    q1, q2 = np.nanquantile(ratio[:dev_end], [1 / 3, 2 / 3])
    lab = np.where(ratio < q1, "lowvol", np.where(ratio > q2, "highvol", "midvol"))
    return np.where(np.isnan(ratio), "na", lab)

def shift_null(m, sig, side, P, s0, s1, ss, se, bps, reps, rng, key):
    """Circular-shift randomization null: the signal series rolled by a random whole number of sessions inside
    [s0, s1); same parameters, same prices. Returns the null distribution of the per-session mean of `key`."""
    out = []
    n_s = se - ss
    for _ in range(reps):
        sh = int(rng.integers(5, max(6, n_s - 5))) * bps
        s2 = sig.copy(); s2[s0:s1] = np.roll(sig[s0:s1], sh)
        out.append(float(m.per_session(m.trades(s2, side, **P), key)[ss:se].mean()))
    return np.array(out)

def _empty(r):
    r.add("found", "No unit reached this phase: the previous phase rejected every unit.")
    r.gate("units entering this phase", 0, ">= 1", False)
    return r.save()

def _forest(a, mean, lo, hi, labels, title):
    a.errorbar(mean, range(len(mean)), xerr=[np.asarray(mean) - np.asarray(lo), np.asarray(hi) - np.asarray(mean)], fmt="o", ms=3)
    a.axvline(0, color="k", lw=0.5); a.set_yticks(range(len(mean)), labels, fontsize=6); a.set_title(title)

# ============================================================================= P0
def P0(ctx):
    r = Report(ctx.out, "P0", "Data, sessions and windows")
    (ctx.out / "cache").mkdir(parents=True, exist_ok=True)
    cs, sp = ctx.cfg["session"], ctx.cfg["split"]
    r.add("did", "Loaded each 30-minute file, removed duplicate timestamps, enforced OHLC containment, checked the time and tick "
                 f"grids, built CME sessions (start {cs['start_hour']}:00 CT; any gap >= {cs['gap_minutes']} min also starts a "
                 "session), flagged contract rolls, cut the windows by session (Dev / embargo / Validation / embargo / Test), "
                 f"split Dev into {sp['dev_blocks']} blocks, and hashed the Test rows.")
    r.add("tried", "Nothing is tested in P0.")
    qa, spl, hashes = [], [], {}
    fig, ax = plt.subplots(len(ctx.syms), 1, figsize=(11, 2.1 * len(ctx.syms)), squeeze=False)
    fig2, ax2 = plt.subplots(1, 2, figsize=(11, 3.4))
    for k, sym in enumerate(ctx.syms):
        i = ctx.ins(sym)
        p = ctx.data / i["file"]
        if not p.exists():
            hits = sorted(ctx.data.glob(f"*{sym}_30min*.csv"))
            if len(hits) != 1:
                raise SystemExit(f"expected exactly one data file for {sym} in {ctx.data}, found {len(hits)}")
            p = hits[0]
        df = core.load_symbol(str(p))
        dup = int(df["ts"].duplicated(keep="last").sum())
        df = df[~df["ts"].duplicated(keep="last")].reset_index(drop=True)
        bad = int(((df["high"] < df[["open", "close"]].max(axis=1)) | (df["low"] > df[["open", "close"]].min(axis=1))).sum())
        df["high"] = df[["open", "high", "low", "close"]].max(axis=1); df["low"] = df[["open", "high", "low", "close"]].min(axis=1)
        offgrid = float(((df["ts"].dt.minute % 30) != 0).mean())
        offtick = float(np.mean([(np.abs(df[c] / i["tick"] - np.round(df[c] / i["tick"])) > 1e-6).mean()
                                 for c in ("open", "high", "low", "close")]))
        df = core.add_sessions(df, cs["start_hour"], cs["gap_minutes"], {k_: tuple(v) for k_, v in cs["buckets"].items()})
        bps = df.groupby("sess").size()
        atr = core.atr_wilder(*(df[c].to_numpy() for c in ("high", "low", "close")))
        g = np.abs(df["open"] - df["close"].shift()) / pd.Series(atr).shift()
        first = df["sess"].diff().fillna(1).to_numpy() > 0
        roll_first = df["roll"].to_numpy() & first
        roll_gap = float(g[roll_first].median()) if roll_first.any() else np.nan
        sess_gap = float(g[first & ~df["roll"].to_numpy()].median())
        n = int(df["sess"].max()) + 1; e = sp["embargo_sessions"]
        nd, nv = int(n * sp["dev"]), int(n * sp["val"])
        seg = np.full(n, "emb", dtype=object)
        seg[:nd] = "dev"; seg[nd + e:nd + e + nv] = "val"; seg[nd + 2 * e + nv:] = "test"
        blk = np.full(n, -1)
        blk[:nd] = np.concatenate([np.full(len(a), j) for j, a in enumerate(np.array_split(np.arange(nd), sp["dev_blocks"]))])
        df["seg"] = seg[df["sess"].to_numpy()]; df["blk"] = blk[df["sess"].to_numpy()]
        df.to_pickle(ctx.out / "cache" / f"{sym}.pkl")
        hashes[sym] = hashlib.sha256(df[df["seg"] == "test"][["ts", "open", "high", "low", "close"]].to_csv().encode()).hexdigest()[:16]
        qa.append(dict(symbol=sym, file=p.name, bars=len(df), first=df["ts"].iloc[0], last=df["ts"].iloc[-1], sessions=n,
                       dup_removed=dup, ohlc_repaired=bad, offgrid_ts=offgrid, offtick=offtick,
                       median_bars_per_session=float(bps.median()), short_sessions=int((bps < 0.5 * bps.median()).sum()),
                       rolls=int(df["roll"].sum()), roll_open_gap_atr=roll_gap, normal_open_gap_atr=sess_gap,
                       min_price=float(df["low"].min())))
        for name in ("dev", "val", "test"):
            dd = df[df["seg"] == name]
            spl.append(dict(symbol=sym, segment=name, start=dd["ts"].iloc[0], end=dd["ts"].iloc[-1],
                            sessions=int(dd["sess"].nunique()), bars=len(dd)))
        a = ax[k, 0]; a.plot(df["ts"], df["close"], lw=0.5, color="k"); a.set_ylabel(sym)
        for name, col in (("val", "tab:orange"), ("test", "tab:red")):
            dd = df[df["seg"] == name]; a.axvspan(dd["ts"].iloc[0], dd["ts"].iloc[-1], color=col, alpha=0.15)
        ax2[0].hist(bps.to_numpy(), bins=50, histtype="step", label=sym)
        ax2[1].hist(df["ts"].dt.hour + df["ts"].dt.minute / 60, bins=48, histtype="step", label=sym)
    ax2[0].set_title("bars per session"); ax2[1].set_title("bar time of day (CT)"); ax2[0].legend()
    r.fig(fig, "split_timeline", "Close per instrument; orange = Validation, red = Test, the rest = Dev")
    r.fig(fig2, "sessions", "Session lengths and time-of-day coverage")
    Q = pd.DataFrame(qa); SP = pd.DataFrame(spl)
    r.table("changed", Q[["symbol", "dup_removed", "ohlc_repaired"]], "repairs")
    r.table("found", Q, "data_qa")
    r.table("found", SP, "splits")
    r.table("found", pd.DataFrame([dict(symbol=s_, contract=ctx.ins(s_)["contract"], tick=ctx.ins(s_)["tick"],
                                        tick_value=ctx.ins(s_)["tick_value"], fees_rt_usd=ctx.ins(s_)["fees_rt_usd"],
                                        cost_ticks_rt=round(ctx.ins(s_)["cost_ticks_rt"], 3),
                                        cost_rt_usd=round(ctx.ins(s_)["cost_ticks_rt"] * ctx.ins(s_)["tick_value"], 2),
                                        stop_slip_ticks=ctx.ins(s_)["stop_slip_ticks"], flat_by=ctx.cfg.get("flat_by"))
                                   for s_ in ctx.syms]), "costs")
    r.gate("OHLC repairs", f"{(Q.ohlc_repaired / Q.bars).max():.4%}", "<= 0.1% of bars", (Q.ohlc_repaired / Q.bars).max() <= 0.001)
    r.gate("timestamps on the 30-minute grid", f"{1 - Q.offgrid_ts.max():.3%}", ">= 99%", Q.offgrid_ts.max() <= 0.01)
    r.gate("prices on the tick grid", f"{1 - Q.offtick.max():.3%}", ">= 99%", Q.offtick.max() <= 0.01)
    r.gate("sessions in every window", int(SP.sessions.min()), ">= 250", SP.sessions.min() >= 250)
    return r.save(dict(test_hash=hashes))

# ============================================================================= P1
def P1(ctx):
    require(ctx, "P0")
    r = Report(ctx.out, "P1", "Registry and engine acceptance")
    r.add("did", "Ran the acceptance tests (engine and drift baseline against slow reference implementations, every trigger "
                 "and filter against a slow reference, mirror and look-ahead tests, statistics against statsmodels and Monte "
                 "Carlo), validated the candidate registry and made every row executable (exact rule text, cell id, spec "
                 "hash), and measured trigger overlap on Dev bars. No return is computed in P1.")
    T = pd.DataFrame(selftest.run_all(), columns=["test", "passed", "detail"])
    r.table("found", T, "acceptance_tests", 60)
    raw = pd.read_csv((ctx.base / ctx.cfg["registry"]).resolve())
    prob = core.registry_problems(raw)
    reg = core.build_registry(raw)
    reg.to_csv(r.dir / "registry.csv", index=False)
    cells = reg.groupby("cell_id").size()
    r.add("changed", "Wrote `registry.csv`: the candidates with `uses_t3`, `cell_id`, `trigger_def`, `filter_def`, `spec_hash`.")
    r.add("found", f"{len(reg)} candidates = {cells.size} trigger x filter x side cells; {int(reg.uses_t3.sum())} use T3, "
                   f"{int((~reg.uses_t3).sum())} do not.")
    r.table("found", pd.DataFrame([prob]).T.reset_index().rename(columns={"index": "check", 0: "rows"}), "registry_checks")
    comp = reg.pivot_table(index="trigger", columns="filter", values="candidate_id", aggfunc="count").fillna(0)
    fig, a = plt.subplots(figsize=(12, 9)); im = a.imshow(comp.to_numpy(), cmap="viridis", aspect="auto")
    a.set_xticks(range(comp.shape[1]), comp.columns, rotation=90, fontsize=7); a.set_yticks(range(comp.shape[0]), comp.index, fontsize=7)
    fig.colorbar(im); r.fig(fig, "registry_composition", "Candidates per trigger x filter")
    t3r = reg[reg.uses_t3]
    L0, V0 = int(t3r.t3_length.median()), float(t3r.t3_vf.median())
    J = np.zeros((len(core.TRIGGER_NAMES),) * 2)
    for sym in ctx.syms:
        df, b, m, seg, blk = market(ctx, sym)
        bs, be = seg["dev"][:2]
        sigs = [core.trigger(b, t, 1, L0, V0)[bs:be] for t in core.TRIGGER_NAMES]
        for i_, j_ in itertools.product(range(len(sigs)), repeat=2):
            u = (sigs[i_] | sigs[j_]).sum(); J[i_, j_] += ((sigs[i_] & sigs[j_]).sum() / u if u else 0) / len(ctx.syms)
    fig, a = plt.subplots(figsize=(10, 9)); im = a.imshow(J, cmap="magma", vmin=0, vmax=1)
    a.set_xticks(range(len(J)), core.TRIGGER_NAMES, rotation=90, fontsize=7); a.set_yticks(range(len(J)), core.TRIGGER_NAMES, fontsize=7)
    fig.colorbar(im); r.fig(fig, "trigger_overlap", f"Jaccard overlap of trigger entries on Dev, long side, T3 {L0} / {V0}")
    hi = [(core.TRIGGER_NAMES[i_], core.TRIGGER_NAMES[j_], J[i_, j_]) for i_ in range(len(J)) for j_ in range(i_ + 1, len(J)) if J[i_, j_] >= 0.5]
    if hi:
        r.table("found", pd.DataFrame(hi, columns=["trigger_a", "trigger_b", "jaccard"]).sort_values("jaccard", ascending=False), "trigger_overlap")
    r.gate("acceptance tests", f"{int(T.passed.sum())}/{len(T)}", "all pass", T.passed.all())
    for k, v in prob.items():
        r.gate(f"registry: {k}", v, "0", v == 0)
    r.gate("unique spec hashes", int(reg.spec_hash.nunique()), f"== {len(reg)}", reg.spec_hash.nunique() == len(reg))
    return r.save()

# ============================================================================= P2
def P2(ctx):
    require(ctx, "P1")
    c = ctx.cfg["p2_screen"]; nb = ctx.cfg["split"]["dev_blocks"]; H = c["horizon"]
    r = Report(ctx.out, "P2", "Screen: edge before costs, then after costs (Dev)")
    reg = load_registry(ctx); recs = reg.to_dict("records")
    r.add("did", f"All {len(recs)} candidates x {len(ctx.syms)} instruments = {len(recs) * len(ctx.syms)} units on Dev with one "
                 f"screen exit for every unit (pure time exit after the row's screen_bars, default {H}; 48 = held until the flat-by time). Layer A, edge before costs: gross R per trade "
                 "minus the drift baseline (the gross R of the same exit entered after every eligible Dev bar at the same time of day, "
                 "same side), its "
                 "Newey-West t on the per-session series, Benjamini-Hochberg over all units, edge ratio (mean MFE / mean MAE in "
                 "ATR) and persistence across the Dev blocks. Layer B, after costs: net R per trade and its Newey-West t. Per "
                 "instrument, on the net per-session series: K_eff (effective number of independent units), the spread of SR across "
                 "units, Hansen SPA p, PBO.")
    r.add("tried", f"{len(recs) * len(ctx.syms)} units, one exit. Every unit counts as a trial.")
    rows, uni, entries = [], {}, {}
    for sym in ctx.syms:
        df, b, m, seg, blk_s = market(ctx, sym)
        bs, be, ss, se = seg["dev"]; blk = blk_s[ss:se]
        bcache = {}
        M = np.zeros((se - ss, len(recs)), np.float32)
        for j, rec in enumerate(recs):
            side = side_of(rec); Hr = screen_bars(rec, H)
            tr = m.trades(core.candidate_signal(b, rec), side, 0, 0, Hr, bs, be)
            n = len(tr["r_atr"])
            ex = m.excess(tr, side, Hr, bs, be, bcache)
            ps_ex = m.per_session(tr, values=ex)[ss:se]
            ps_net = m.per_session(tr)[ss:se]
            M[:, j] = ps_net
            t_g, t_n = S.nw_tstat(ps_ex), S.nw_tstat(ps_net)
            mae = tr["mae_atr"].mean() if n else 0.0
            row = dict(candidate_id=rec["candidate_id"], symbol=sym, family=rec["family"], trigger=rec["trigger"],
                       filter=rec["filter"], side=rec["side"], t3_length=rec["t3_length"], t3_vf=rec["t3_vf"], screen_bars=Hr, n=n,
                       gross_mean=float(tr["g_atr"].mean()) if n else 0.0, excess_mean=float(ex.mean()) if n else 0.0,
                       t_gross=t_g, p_gross=S.p_one_sided(t_g),
                       e_ratio=float(tr["mfe_atr"].mean() / mae) if n and mae > 0 else 0.0,
                       pos_blocks=int((np.bincount(blk, weights=ps_ex, minlength=nb) > 0).sum()),
                       net_mean=float(tr["r_atr"].mean()) if n else 0.0, t_net=t_n,
                       win=float((tr["r_atr"] > 0).mean()) if n else 0.0, sr_net=S.sharpe(ps_net))
            rows.append(row)
            if n >= c["min_trades"] and t_g >= c["t_min"] and row["excess_mean"] > 0:
                entries[unit_key(rec["candidate_id"], sym)] = set(tr["entry_bar"].tolist())
        M = M.astype(float)
        live = M.std(0) > 0
        srs = M[:, live].mean(0) / M[:, live].std(0, ddof=1)
        uni[sym] = dict(N=len(recs), K_eff=S.k_eff(M, c["keff_cutoff"]), sr_cross_sd=float(np.std(srs)) if live.any() else 0.0,
                        spa_p=S.spa_pvalue(M, c["spa_reps"]), pbo=S.pbo_cscv(M, c["pbo_blocks"]), dev_sessions=int(se - ss),
                        drift_long=m.baseline(1, H, bs, be), drift_short=m.baseline(-1, H, bs, be))
        print(f"  P2 {sym}: {uni[sym]}")
    U = pd.DataFrame(rows)
    disc, q = S.bh(U["p_gross"].to_numpy(), c["bh_q"]); U["q_gross"] = q
    U["g_trades"] = U.n >= c["min_trades"]
    U["g_edge"] = U.excess_mean > 0
    U["g_t_gross"] = U.t_gross >= c["t_min"]
    U["g_bh_gross"] = disc
    U["g_e_ratio"] = U.e_ratio > c["e_ratio_min"]
    U["g_blocks"] = U.pos_blocks >= c["min_pos_blocks"]
    U["g_net"] = U.net_mean > 0
    U["g_t_net"] = U.t_net >= c["t_min"]
    gA = ["g_trades", "g_edge", "g_t_gross", "g_bh_gross", "g_e_ratio", "g_blocks"]; gB = ["g_net", "g_t_net"]
    U["edge_before_costs"] = U[gA].all(axis=1)
    U["pass"] = U[gA + gB].all(axis=1)
    cand = U[U["pass"]].sort_values("t_net", ascending=False)
    kept = []
    for sym in ctx.syms:
        sets = []
        for idx, row in cand[cand.symbol == sym].iterrows():
            e = entries.get(unit_key(row.candidate_id, sym))
            if e is not None and all(len(e & k) / max(1, len(e | k)) <= c["jaccard_max"] for k in sets):
                sets.append(e); kept.append(idx)
    SLt = U.loc[kept].sort_values("t_net", ascending=False).head(c["shortlist_max"])
    U.to_csv(r.dir / "units.csv", index=False)
    exp = len(U) * (1 - sst.norm.cdf(c["t_min"]))
    r.add("found", f"Units with gross t >= {c['t_min']}: {int(U.g_t_gross.sum())} (chance alone gives about {exp:.0f}). "
                   f"Edge before costs (all layer-A gates): {int(U.edge_before_costs.sum())}. Also positive and significant after "
                   f"costs: {int(U['pass'].sum())}. After removing overlapping entries (Jaccard > {c['jaccard_max']}): "
                   f"{len(kept)}. Shortlist: {len(SLt)}.")
    r.table("found", pd.DataFrame(uni).T.rename_axis("symbol").reset_index(), "universe")
    r.table("found", pd.DataFrame({"gate": gA + gB, "units_passing": [int(U[g].sum()) for g in gA + gB]}), "gate_funnel")
    r.table("found", SLt[["candidate_id", "symbol", "family", "trigger", "filter", "side", "t3_length", "t3_vf", "n",
                          "excess_mean", "t_gross", "q_gross", "e_ratio", "pos_blocks", "net_mean", "t_net"]], "shortlist", 50)
    xs = np.linspace(-6, 6, 300)
    fig, a = plt.subplots(1, 3, figsize=(16, 4))
    for sym in ctx.syms:
        a[0].hist(U[U.symbol == sym].t_gross.clip(-8, 8), bins=80, histtype="step", density=True, label=sym)
        a[1].hist(U[U.symbol == sym].t_net.clip(-8, 8), bins=80, histtype="step", density=True, label=sym)
    for k, ttl in ((0, "gross excess t (before costs)"), (1, "net t (after costs)")):
        a[k].plot(xs, sst.norm.pdf(xs), "k--", label="N(0,1): no edge"); a[k].set_title(ttl); a[k].legend(fontsize=6)
    a[2].scatter(U.excess_mean, U.net_mean, s=2, c=np.where(U["pass"], "tab:red", np.where(U.edge_before_costs, "tab:orange", "0.75")))
    a[2].axhline(0, color="k", lw=0.5); a[2].axvline(0, color="k", lw=0.5)
    a[2].set_xlabel("gross excess R per trade (ATR)"); a[2].set_ylabel("net R per trade (ATR)")
    a[2].set_title("orange = edge before costs, red = also after costs")
    r.fig(fig, "screen_distribution", "Distribution of edges before and after costs against the no-edge curve")
    hm = U.pivot_table(index="family", columns="symbol", values="edge_before_costs", aggfunc="mean")
    fig, a = plt.subplots(figsize=(7, 4.5)); im = a.imshow(hm.to_numpy(), cmap="Reds", vmin=0)
    a.set_xticks(range(hm.shape[1]), hm.columns); a.set_yticks(range(hm.shape[0]), hm.index, fontsize=7); fig.colorbar(im)
    r.fig(fig, "family_heatmap", "Share of units with an edge before costs, by family and instrument")
    fig, a = plt.subplots(1, 2, figsize=(12, 4))
    a[0].scatter(U.e_ratio.clip(0, 3), U.excess_mean, s=2, c=np.where(U["pass"], "tab:red", "0.75"))
    a[0].axvline(1, color="k", lw=0.5); a[0].axhline(0, color="k", lw=0.5)
    a[0].set_xlabel("edge ratio (mean MFE / mean MAE)"); a[0].set_ylabel("gross excess R"); a[0].set_title(f"entry quality over {H} bars")
    for sym, g in SLt.head(12).groupby("symbol"):
        df, b, m, seg, _ = market(ctx, sym); bs, be = seg["dev"][:2]
        for _, row in g.iterrows():
            rec = reg[reg.candidate_id == row.candidate_id].iloc[0].to_dict(); side = side_of(rec)
            sig = core.candidate_signal(b, rec)
            prof = [m.excess(m.trades(sig, side, 0, 0, h, bs, be), side, h, bs, be).mean() for h in c["horizons"]]
            a[1].plot(c["horizons"], prof, marker="o", ms=3, label=f"{row.candidate_id} {sym}")
    a[1].axhline(0, color="k", lw=0.5); a[1].set_xlabel("bars held"); a[1].set_ylabel("gross excess R")
    if len(SLt):
        a[1].legend(fontsize=6)
    a[1].set_title("horizon profile of the shortlist (before costs)")
    r.fig(fig, "entry_quality", "Edge ratio vs excess return, and how the shortlist's edge builds or decays with time")
    r.gate("shortlist", len(SLt), ">= 1 unit", len(SLt) >= 1)
    return r.save(dict(universe=uni))

# ============================================================================= P3
class Tuner:
    """Evaluates (L, V, sl_atr, tp_r, max_bars) on Dev and caches the per-session net series of every config."""
    def __init__(self, ctx, b, m, rec, seg, blk):
        self.c = ctx.cfg["p3_refine"]; self.h = screen_bars(rec, ctx.cfg["p2_screen"]["horizon"]); self.seed = ctx.cfg["seed"]
        self.b, self.m, self.rec, self.side = b, m, rec, side_of(rec)
        self.bs, self.be, self.ss, self.se = seg["dev"]; self.blk = blk[self.ss:self.se]
        self.t3 = bool(rec["uses_t3"]); self.cache = {}
        self.LV = list(itertools.product(self.c["t3_length"], self.c["t3_vf"])) if self.t3 else [(0, 0.0)]
        self.shapeA = (len(self.c["t3_length"]), len(self.c["t3_vf"])) if self.t3 else (1, 1)
        self.EX = list(itertools.product(self.c["sl_atr"], self.c["tp_r"], self.c["max_bars"]))
        self.shapeB = (len(self.c["sl_atr"]), len(self.c["tp_r"]), len(self.c["max_bars"]))

    def signal(self, L, V):
        return core.candidate_signal(self.b, with_params(self.rec, L, V))

    def ev(self, key):
        if key not in self.cache:
            L, V, sl, tp, hb = key
            tr = self.m.trades(self.signal(L, V), self.side, sl, tp, hb, self.bs, self.be)
            ps = self.m.per_session(tr)[self.ss:self.se]
            cnt = np.bincount(self.m.sess[tr["entry_bar"]] - self.ss, minlength=self.se - self.ss)
            self.cache[key] = (ps, cnt)
        return self.cache[key]

    def score(self, key, train):
        """25th percentile, over the training blocks, of the block's mean net R per session; -inf below the trade floor."""
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
            i = t.suggest_categorical("lv", list(range(len(self.LV))))
            key = (*self.LV[i], t.suggest_categorical("sl", c["sl_atr"]), t.suggest_categorical("tp", c["tp_r"]),
                   t.suggest_categorical("hb", c["max_bars"]))
            v = self.score(key, train)
            return v if np.isfinite(v) else -1e9
        study = optuna.create_study(direction="maximize", sampler=optuna.samplers.TPESampler(seed=self.seed))
        study.optimize(obj, n_trials=c["optuna_trials"])
        p = study.best_params
        return (*self.LV[p["lv"]], p["sl"], p["tp"], p["hb"]), None

    def choose(self, train):
        return self.choose_optuna(train) if self.c["search"] == "optuna" else self.choose_grid(train)

def P3(ctx):
    require(ctx, "P2")
    c = ctx.cfg["p3_refine"]; nb = ctx.cfg["split"]["dev_blocks"]
    r = Report(ctx.out, "P3", "T3 settings, stop, target and hold by walk-forward (Dev)")
    SLt = pd.read_csv(ctx.out / "P2" / "shortlist.csv"); reg = load_registry(ctx).set_index("candidate_id")
    (r.dir / "units").mkdir(exist_ok=True)
    if SLt.empty:
        return _empty(r)
    r.add("did", f"For every shortlisted unit, {nb - 1} expanding walk-forward folds inside Dev (fold k trains on blocks 0..k-1 and "
                 "is scored on block k). In each fold the parameters are chosen on the training blocks only and applied unchanged "
                 f"to block k. Search: {c['search']}. Stage A chooses the T3 length and volume factor with the screen exit (T3 units "
                 "only); stage B chooses stop (x ATR), target (x stop) and maximum bars on that T3 setting. Objective: 25th "
                 "percentile of the training blocks' mean net R per session. Grid scores are plateau-smoothed before the choice. "
                 "Final parameters: the same procedure on all Dev blocks. Every evaluated configuration enters the PBO and the "
                 "effective-trial count (K_eff of the configurations).")
    r.add("tried", f"T3 grid {len(c['t3_length'])} x {len(c['t3_vf'])} = {len(c['t3_length']) * len(c['t3_vf'])}; exit grid "
                   f"{len(c['sl_atr'])} x {len(c['tp_r'])} x {len(c['max_bars'])} = {len(c['sl_atr']) * len(c['tp_r']) * len(c['max_bars'])}.")
    out, folds_all, heat = [], [], []
    for sym, grp in SLt.groupby("symbol"):
        df, b, m, seg, blk = market(ctx, sym)
        for _, u in grp.iterrows():
            rec = reg.loc[u.candidate_id].to_dict(); rec["candidate_id"] = u.candidate_id
            tu = Tuner(ctx, b, m, rec, seg, blk)
            oos = np.full(len(tu.blk), np.nan); fsum, ftr = [], []
            for k in range(1, nb):
                key, _ = tu.choose(list(range(k)))
                ps, cnt = tu.ev(key); mk = tu.blk == k
                oos[mk] = ps[mk]; fsum.append(float(ps[mk].sum())); ftr.append(int(cnt[mk].sum()))
                folds_all.append(dict(candidate_id=u.candidate_id, symbol=sym, fold=k, t3_length=key[0] if tu.t3 else np.nan,
                                      t3_vf=key[1] if tu.t3 else np.nan, sl_atr=key[2], tp_r=key[3], max_bars=key[4],
                                      oos_sum_r=fsum[-1], oos_trades=ftr[-1]))
            final, sb = tu.choose(list(range(nb)))
            Mc = np.column_stack([v[0] for v in tu.cache.values()])
            pbo = S.pbo_cscv(Mc, c["pbo_blocks"]); keff_c = S.k_eff(Mc, ctx.cfg["p2_screen"]["keff_cutoff"])
            x = oos[tu.blk >= 1]
            np.savez(r.dir / "units" / f"{unit_key(u.candidate_id, sym)}.npz", oos=oos, fold_sums=np.array(fsum))
            out.append(dict(candidate_id=u.candidate_id, symbol=sym, uses_t3=tu.t3,
                            t3_length=final[0] if tu.t3 else np.nan, t3_vf=final[1] if tu.t3 else np.nan,
                            sl_atr=final[2], tp_r=final[3], max_bars=final[4], n_configs=len(tu.cache), keff_configs=keff_c, pbo=pbo,
                            oos_mean_sess=float(np.mean(x)), oos_trades=int(sum(ftr)), oos_pos_folds=int((np.array(fsum) > 0).sum())))
            if sb is not None and len(heat) < 12:
                heat.append((f"{u.candidate_id} {sym}", sb[:, :, c["max_bars"].index(final[4])], final))
            print(f"  P3 {u.candidate_id} {sym}: {out[-1]}")
    R = pd.DataFrame(out); F = pd.DataFrame(folds_all)
    R["g_oos_trades"] = R.oos_trades >= c["min_oos_trades"]; R["g_oos_mean"] = R.oos_mean_sess > 0
    R["pass"] = R.g_oos_trades & R.g_oos_mean
    r.table("found", R, "refined", 60)
    r.table("found", F, "wf_folds", 60)
    r.add("changed", "Each unit now carries its own T3 setting (T3 units) and exit; `wf_folds.csv` shows the choice of every fold.")
    if heat:
        n = len(heat); fig, ax = plt.subplots((n + 3) // 4, 4, figsize=(14, 3.2 * ((n + 3) // 4)), squeeze=False)
        for a, (name, sc, fin) in zip(ax.flat, heat):
            im = a.imshow(np.where(np.isfinite(sc), sc, np.nan), cmap="RdYlGn", origin="lower", aspect="auto")
            a.set_xticks(range(len(c["tp_r"])), c["tp_r"], fontsize=6); a.set_yticks(range(len(c["sl_atr"])), c["sl_atr"], fontsize=6)
            a.set_xlabel("TP (x stop)", fontsize=7); a.set_ylabel("SL (x ATR)", fontsize=7); a.set_title(f"{name} hold={fin[4]}", fontsize=7)
            a.plot(c["tp_r"].index(fin[3]), c["sl_atr"].index(fin[2]), "k*", ms=10); fig.colorbar(im, ax=a)
        for a in list(ax.flat)[n:]:
            a.axis("off")
        r.fig(fig, "exit_surfaces", "Dev score over stop x target at the chosen hold (star = chosen)")
    fig, a = plt.subplots(figsize=(12, 3.8))
    F.pivot_table(index=F.candidate_id + " " + F.symbol, columns="fold", values="oos_sum_r").head(20).plot.bar(ax=a, width=0.8)
    a.axhline(0, color="k", lw=0.5); a.set_ylabel("out-of-sample net R in the fold"); a.tick_params(axis="x", labelsize=6)
    r.fig(fig, "wf_folds", "Walk-forward result per fold (parameters chosen only on earlier blocks)")
    r.gate("units with >= min WF-OOS trades and a positive WF-OOS mean", int(R["pass"].sum()), ">= 1", R["pass"].sum() >= 1)
    return r.save()

# ============================================================================= P4
def _buckets(x, labels, min_n, reps):
    res = []
    for lab in pd.unique(labels):
        if lab == "na":
            continue
        v = x[labels == lab]
        if len(v) >= min_n:
            res.append((lab, len(v), float(v.mean()), S.boot_upper(v, reps)))
    return res

def P4(ctx):
    require(ctx, "P3")
    c = ctx.cfg["p4_evidence"]; c3 = ctx.cfg["p3_refine"]
    r = Report(ctx.out, "P4", "Evidence: timing edge before costs, selection bias, robustness, stability, power (Dev)")
    R = pd.read_csv(ctx.out / "P3" / "refined.csv"); R = R[R["pass"]]
    if R.empty:
        return _empty(r)
    uni = json.load(open(ctx.out / "P2" / "gates.json"))["universe"]
    reg = load_registry(ctx).set_index("candidate_id"); nses = segment_sessions(ctx)
    r.add("did", "Timing edge before costs: circular-shift randomization null on GROSS R with the final parameters on Dev. "
                 "On the walk-forward out-of-sample net series from P3: stationary-bootstrap CI, Deflated Sharpe (N = effective "
                 "number of independent trials = K_eff of the instrument's universe + K_eff of the unit's P3 configurations + the "
                 "effective trials of earlier programs on the same Dev data), PBO of the P3 search, Hansen SPA of the instrument's universe, fold "
                 "dispersion, and the minimum track record length compared with the length of the Validation window. With the "
                 "final parameters on Dev: cost x1.5, +1 tick per side, one-bar delay, grid neighbours, best 5% of trades removed, "
                 "best year removed, positive-year share, and stability by year, volatility tercile and session bucket.")
    rows, curves, nulls = [], {}, {}
    rng = np.random.default_rng(ctx.cfg["seed"])
    for sym, grp in R.groupby("symbol"):
        df, b, m, seg, blk_s = market(ctx, sym)
        bs, be, ss, se = seg["dev"]; blk = blk_s[ss:se]
        reg_lab = regime_labels(b, be, c["regime_window"]); bucket = df["bucket"].to_numpy(); year = df["ts"].dt.year.to_numpy()
        bps = int(df[df.seg == "dev"].groupby("sess").size().median())
        for _, u in grp.iterrows():
            rec = reg.loc[u.candidate_id].to_dict(); side = side_of(rec)
            z = np.load(ctx.out / "P3" / "units" / f"{unit_key(u.candidate_id, sym)}.npz")
            x = z["oos"][blk >= 1]; fs = z["fold_sums"]
            bt = S.boot_mean(x, c["boot_reps"], ctx.cfg["seed"])
            N = int(uni[sym]["K_eff"]) + int(u.keff_configs) + int(ctx.cfg.get("prior_trials", {}).get(sym, 0))
            dsr = S.dsr(x, N)
            mtrl = S.min_trl(x, 0.0, c["mintrl_alpha"])
            sig = core.candidate_signal(b, with_params(rec, u.t3_length, u.t3_vf))
            P = dict(sl_atr=u.sl_atr, tp_r=u.tp_r, max_bars=int(u.max_bars), start=bs, end=be)
            tr = m.trades(sig, side, **P)
            mean_dev = lambda t, key="r_atr": float(m.per_session(t, key)[ss:se].mean())
            obs_g = mean_dev(tr, "g_atr")
            nd = shift_null(m, sig, side, P, bs, be, ss, se, bps, c["null_reps"], rng, "g_atr")
            p_null = S.emp_p(nd, obs_g)
            nb_vals = []
            axes = [("sl_atr", c3["sl_atr"]), ("tp_r", c3["tp_r"]), ("max_bars", c3["max_bars"])]
            if u.uses_t3:
                axes += [("t3_length", c3["t3_length"]), ("t3_vf", c3["t3_vf"])]
            for ax_name, grid in axes:
                i0 = int(np.argmin(np.abs(np.array(grid, float) - float(u[ax_name]))))
                for j in (i0 - 1, i0 + 1):
                    if 0 <= j < len(grid):
                        q_ = dict(t3_length=u.t3_length, t3_vf=u.t3_vf, sl_atr=u.sl_atr, tp_r=u.tp_r, max_bars=u.max_bars)
                        q_[ax_name] = grid[j]
                        sg = core.candidate_signal(b, with_params(rec, q_["t3_length"], q_["t3_vf"]))
                        nb_vals.append(mean_dev(m.trades(sg, side, q_["sl_atr"], q_["tp_r"], int(q_["max_bars"]), bs, be)))
            rr = tr["r_atr"]; srt = np.sort(rr)[::-1]
            trim = float(srt[int(np.ceil(c["top_trim"] * len(srt))):].mean()) if len(srt) >= 20 else -np.inf
            yr = year[tr["entry_bar"]]; ysum = pd.Series(rr).groupby(yr).sum()
            stab = {}
            for dim, lab in (("year", yr), ("regime", reg_lab[tr["sig_bar"]]), ("session", bucket[tr["entry_bar"]])):
                bk = _buckets(rr, lab, c["min_bucket_trades"], c["stability_boot_reps"])
                stab[dim] = dict(sig_negative=sum(1 for q_ in bk if q_[3] < 0), negative=sum(1 for q_ in bk if q_[2] < 0))
            pos_f = fs > 0; tot_pos = fs[pos_f].sum()
            g = dict(
                g_null_gross=p_null <= c["null_p_max"],
                g_ci_lo=bt["lo"] > 0,
                g_dsr=dsr >= c["dsr_min"],
                g_pbo=u.pbo <= c["pbo_max"],
                g_spa=uni[sym]["spa_p"] <= c["spa_p_max"],
                g_wf_pos=pos_f.mean() >= c["wf_pos_frac"],
                g_wf_share=(fs.max() / tot_pos if tot_pos > 0 else 1.0) <= c["fold_share_max"],
                g_cost=mean_dev(m.trades(sig, side, cost_mult=c["cost_mult"], **P)) > 0,
                g_slip=mean_dev(m.trades(sig, side, extra_slip=c["extra_slip_ticks"], **P)) > 0,
                g_delay=mean_dev(m.trades(sig, side, delay=c["delay_bars"], **P)) > 0,
                g_neighbours=bool(nb_vals) and float(np.median(nb_vals)) > 0,
                g_top_trim=trim > 0,
                g_best_year=(ysum.sum() - ysum.max()) > 0,
                g_years=(ysum > 0).mean() >= c["pos_years_frac"],
                g_stable=all(v["sig_negative"] == 0 for v in stab.values()) and stab["regime"]["negative"] <= 1
                         and stab["session"]["negative"] <= 1,
                g_power=mtrl <= nses[(sym, "val")])
            row = dict(candidate_id=u.candidate_id, symbol=sym, wf_mean=bt["mean"], ci_lo=bt["lo"], ci_hi=bt["hi"], p_boot=bt["p"],
                       t_nw=S.nw_tstat(x), sr_sess=S.sharpe(x), dsr=dsr, N_trials=N, gross_mean_dev=obs_g, p_null_gross=p_null,
                       pbo=u.pbo, spa_p=uni[sym]["spa_p"], min_trl=mtrl, val_sessions=nses[(sym, "val")], dev_trades=len(rr),
                       neighbours_median=float(np.median(nb_vals)) if nb_vals else np.nan, **{k: bool(v) for k, v in g.items()})
            row["pass"] = all(g.values()); rows.append(row)
            curves[f"{u.candidate_id} {sym}"] = np.cumsum(x); nulls[f"{u.candidate_id} {sym}"] = (nd, obs_g)
            print(f"  P4 {u.candidate_id} {sym}: pass={row['pass']}")
    E = pd.DataFrame(rows)
    gcols = [k for k in E.columns if k.startswith("g_")]
    r.table("found", E, "evidence", 60)
    E[E["pass"]].to_csv(r.dir / "survivors.csv", index=False)
    r.table("found", pd.DataFrame({"gate": gcols, "units_passing": [int(E[g].sum()) for g in gcols]}), "gate_funnel")
    fig, a = plt.subplots(1, 2, figsize=(13, 4))
    for k, v in list(curves.items())[:15]:
        a[0].plot(v, lw=0.8, label=k)
    a[0].axhline(0, color="k", lw=0.5); a[0].set_title("walk-forward out-of-sample equity (net R)"); a[0].legend(fontsize=6)
    _forest(a[1], E.wf_mean, E.ci_lo, E.ci_hi, E.candidate_id + " " + E.symbol, "95% CI of the WF-OOS mean per session")
    r.fig(fig, "wf_equity_ci", "Walk-forward out-of-sample equity and confidence intervals")
    fig, a = plt.subplots(figsize=(11, 0.3 * len(E) + 2))
    a.imshow(E[gcols].to_numpy(float), cmap="RdYlGn", vmin=0, vmax=1, aspect="auto")
    a.set_xticks(range(len(gcols)), [g_[2:] for g_ in gcols], rotation=60, fontsize=7)
    a.set_yticks(range(len(E)), E.candidate_id + " " + E.symbol, fontsize=6)
    r.fig(fig, "gate_matrix", "Gate matrix (green = pass); a unit continues only if its whole row is green")
    k6 = list(nulls.items())[:6]
    fig, ax = plt.subplots(1, max(1, len(k6)), figsize=(3 * max(1, len(k6)), 2.8), squeeze=False)
    for a, (k, (nd, ob)) in zip(ax[0], k6):
        a.hist(nd, bins=40, color="0.6"); a.axvline(ob, color="r"); a.set_title(k, fontsize=7)
    r.fig(fig, "null_tests", "Gross R: same signals at random times (grey) vs actual timing (red)")
    r.gate("units passing every evidence gate", int(E["pass"].sum()), ">= 1", E["pass"].sum() >= 1)
    return r.save()

# ============================================================================= P5 / P6
def _frozen(ctx, segname, units, reps, null_reps, cost_mult, upto):
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
            p_null = np.nan
            if null_reps:
                obs_g = float(m.per_session(tr, "g_atr")[ss:se].mean())
                p_null = S.emp_p(shift_null(m, sig, side, P, s0, s1, ss, se, bps, null_reps, rng, "g_atr"), obs_g)
            full = m.trades(sig, side, u.sl_atr, u.tp_r, int(u.max_bars))
            out[(u.candidate_id, sym)] = dict(
                ps=ps, trades=len(tr["r_atr"]), mean_r=float(tr["r_atr"].mean()) if len(tr["r_atr"]) else 0.0,
                boot=S.boot_mean(ps, reps, ctx.cfg["seed"]), p_null=p_null,
                cost=float(m.per_session(m.trades(sig, side, cost_mult=cost_mult, **P))[ss:se].mean()),
                curve=(df["ts"].to_numpy()[full["exit_bar"]], np.cumsum(full["r_atr"])),
                seg_ts={k: (df["ts"].iloc[v[0]], df["ts"].iloc[v[1] - 1]) for k, v in seg.items()})
    return out

def _equity(a, res, shade):
    for (cid, sym), v in list(res.items())[:12]:
        t, y = v["curve"]; a.plot(t, y, lw=0.8, label=f"{cid} {sym}")
    if res:
        v = next(iter(res.values()))
        for name, col in shade:
            a.axvspan(*v["seg_ts"][name], color=col, alpha=0.15)
        a.legend(fontsize=6)

def P5(ctx):
    require(ctx, "P4")
    c = ctx.cfg["p5_validation"]
    r = Report(ctx.out, "P5", "Validation (opened once, parameters frozen)")
    units = pd.merge(pd.read_csv(ctx.out / "P4" / "survivors.csv")[["candidate_id", "symbol", "wf_mean"]],
                     pd.read_csv(ctx.out / "P3" / "refined.csv").drop(columns=["pass", "g_oos_trades", "g_oos_mean"]),
                     on=["candidate_id", "symbol"])
    if units.empty:
        return _empty(r)
    nses = segment_sessions(ctx)
    r.add("did", f"Ran the {len(units)} P4 survivors once on the Validation window with frozen parameters: trade count, net mean, "
                 f"stationary bootstrap (one-sided p, Benjamini-Hochberg q <= {c['bh_q']} across these units), the gross-R "
                 f"circular-shift null, cost x{c['cost_mult']}, and the minimum track record length of the Validation series "
                 "compared with the length of the Test window.")
    res = _frozen(ctx, "val", units, c["boot_reps"], c["null_reps"], c["cost_mult"], "val")
    rows = []
    for (cid, sym), v in res.items():
        rows.append(dict(candidate_id=cid, symbol=sym, val_trades=v["trades"], val_mean_r=v["mean_r"], val_sess_mean=v["boot"]["mean"],
                         ci_lo=v["boot"]["lo"], ci_hi=v["boot"]["hi"], p_boot=v["boot"]["p"], p_null_gross=v["p_null"],
                         cost_stress=v["cost"], sr_sess=S.sharpe(v["ps"]), min_trl=S.min_trl(v["ps"], 0.0, c["mintrl_alpha"]),
                         test_sessions=nses[(sym, "test")]))
    V = pd.DataFrame(rows)
    disc, q = S.bh(V.p_boot.to_numpy(), c["bh_q"]); V["q_bh"] = q
    V["g_trades"] = V.val_trades >= c["min_trades"]; V["g_mean"] = V.val_sess_mean > 0; V["g_bh"] = disc
    V["g_null_gross"] = V.p_null_gross <= c["null_p_max"]; V["g_cost"] = V.cost_stress > 0
    V["g_power"] = V.min_trl <= V.test_sessions
    gcols = ["g_trades", "g_mean", "g_bh", "g_null_gross", "g_cost", "g_power"]
    V["pass"] = V[gcols].all(axis=1)
    V = V.merge(units, on=["candidate_id", "symbol"])
    r.table("found", V, "validation", 60)
    V[V["pass"]].to_csv(r.dir / "survivors.csv", index=False)
    fig, a = plt.subplots(1, 2, figsize=(13, 4))
    _equity(a[0], res, (("val", "tab:orange"),)); a[0].set_title("frozen parameters: Dev, then Validation (orange)")
    _forest(a[1], V.val_sess_mean, V.ci_lo, V.ci_hi, V.candidate_id + " " + V.symbol, "Validation 95% CI (per session)")
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
    lock = r.dir / "LOCK"; key = hashlib.sha1(units[["candidate_id", "symbol"]].to_csv().encode()).hexdigest()
    if lock.exists() and lock.read_text().strip() != key and not force:
        raise SystemExit("The Test window was already opened for a different set of units. Opening it again for new choices "
                         "is data snooping. --force marks every Test result as contaminated and fails it.")
    contaminated = lock.exists() and lock.read_text().strip() != key
    lock.write_text(key)
    r.add("did", f"Ran the {len(units)} P5 survivors once on the Test window with frozen parameters: trade count, 95% "
                 f"stationary-bootstrap CI, Deflated Sharpe against the number of units tested here, cost x{c['cost_mult']}.")
    if contaminated:
        r.add("found", "The Test window had already been opened for a different unit set: every result below is contaminated.")
    res = _frozen(ctx, "test", units, c["boot_reps"], 0, c["cost_mult"], "all")
    N = len(res)
    rows = []
    for (cid, sym), v in res.items():
        d = S.dsr(v["ps"], N) if N > 1 else S.psr(v["ps"], 0.0)
        rows.append(dict(candidate_id=cid, symbol=sym, test_trades=v["trades"], test_mean_r=v["mean_r"], test_sess_mean=v["boot"]["mean"],
                         ci_lo=v["boot"]["lo"], ci_hi=v["boot"]["hi"], p_boot=v["boot"]["p"], dsr=d, cost_stress=v["cost"]))
    T = pd.DataFrame(rows)
    T["g_trades"] = T.test_trades >= c["min_trades"]; T["g_ci_lo"] = T.ci_lo > 0; T["g_dsr"] = T.dsr >= c["dsr_min"]
    T["g_cost"] = T.cost_stress > 0; T["g_sealed"] = not contaminated
    T["pass"] = T[["g_trades", "g_ci_lo", "g_dsr", "g_cost", "g_sealed"]].all(axis=1)
    T = T.merge(units[["candidate_id", "symbol", "uses_t3", "t3_length", "t3_vf", "sl_atr", "tp_r", "max_bars"]], on=["candidate_id", "symbol"])
    r.table("found", T, "test", 60)
    reg = load_registry(ctx).set_index("candidate_id")
    cards = []
    for _, u in T[T["pass"]].iterrows():
        rec = reg.loc[u.candidate_id]; i = ctx.ins(u.symbol)
        cards.append(dict(candidate_id=u.candidate_id, symbol=u.symbol, Trigger=rec.trigger, Filter=rec["filter"], Side=rec.side.capitalize(),
                          T3_length=int(u.t3_length) if rec.uses_t3 else "-", T3_vf=u.t3_vf if rec.uses_t3 else "-",
                          ATR_length=ctx.cfg["atr_len"], SL_ATR=u.sl_atr, TP_R=u.tp_r, Max_bars=int(u.max_bars),
                          Contract=i["contract"], Fees_round_trip_usd=i["fees_rt_usd"], Slippage_ticks_per_side=ctx.cfg["slippage_ticks_per_side"],
                          Cost_ticks_round_trip=round(i["cost_ticks_rt"], 3), Stop_slip_ticks=i["stop_slip_ticks"], Flat_by_CT=ctx.cfg.get("flat_by"),
                          Test_CI_low=u.ci_lo, Test_CI_high=u.ci_hi))
    if cards:
        r.table("found", pd.DataFrame(cards), "deployment_cards")
    fig, a = plt.subplots(1, 2, figsize=(13, 4))
    _equity(a[0], res, (("val", "tab:orange"), ("test", "tab:red"))); a[0].set_title("whole history, frozen parameters (orange Val, red Test)")
    _forest(a[1], T.test_sess_mean, T.ci_lo, T.ci_hi, T.candidate_id + " " + T.symbol, "Test 95% CI (per session)")
    r.fig(fig, "test", "Sealed test")
    r.gate("units passing the sealed test", int(T["pass"].sum()), ">= 1", T["pass"].sum() >= 1)
    return r.save()

PHASES = dict(P0=P0, P1=P1, P2=P2, P3=P3, P4=P4, P5=P5, P6=P6)
