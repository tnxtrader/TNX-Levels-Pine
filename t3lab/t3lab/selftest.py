"""Acceptance tests run by P1 and by pytest. Synthetic data only.
Fast code is checked against independent slow reference implementations written from the framework text,
and the statistics are checked against statsmodels and Monte Carlo."""
from __future__ import annotations
import math
import numpy as np, pandas as pd
from . import core, stats as S
from .engine import Market, _run

# ----------------------------------------------------------------------------- synthetic market
def synthetic(n_sessions=300, bars=46, seed=0, tick=0.01, start="2015-01-04 17:00", roll_every=60, px0=None, drift=0.0, plant=0.0):
    """Random-walk 30-min OHLC with CME-style sessions (17:00-15:30 CT bars), weekends and contract rolls.
    No edge exists by construction. `drift` adds a constant drift per bar. `plant` adds a timing edge on purpose: after a
    close above the highest high of the previous 20 bars the next 8 bars drift up by `plant` x volatility per bar, and
    after a close below the lowest low of the previous 20 bars they drift down by the same amount (no net drift)."""
    rng = np.random.default_rng(seed)
    rows, k, up, dn, hist_h, hist_l = [], 0, 0, 0, [], []
    px = 10000 * tick if px0 is None else px0
    day = pd.Timestamp(start)
    q = lambda x: np.round(x / tick) * tick
    while k < n_sessions:
        if day.weekday() in (4, 5):
            day += pd.Timedelta(days=1); continue
        sig = 0.002 * px * np.exp(rng.normal(0, 0.3))
        for j in range(bars):
            t = day + pd.Timedelta(minutes=30 * j)
            push = drift * sig + plant * sig * ((up > 0) - (dn > 0))
            up = max(0, up - 1); dn = max(0, dn - 1)
            path = px + push + np.cumsum(rng.normal(0, sig / 3, 4))
            o = px; c = path[-1]
            h = max(o, c, path.max()) + abs(rng.normal(0, sig / 4)); l = min(o, c, path.min()) - abs(rng.normal(0, sig / 4))
            rows.append((t, q(o), q(h), q(l), q(c), int(rng.integers(1, 500)), f"C{k // roll_every:03d}"))
            if len(hist_h) == 20:
                if q(c) > max(hist_h):
                    up = 8
                elif q(c) < min(hist_l):
                    dn = 8
            hist_h = (hist_h + [q(h)])[-20:]; hist_l = (hist_l + [q(l)])[-20:]
            px = c + rng.normal(0, sig / 6)
        day += pd.Timedelta(days=1); k += 1
    df = pd.DataFrame(rows, columns=["timestamp", "open", "high", "low", "close", "volume", "contract"])
    df["timestamp"] = df["timestamp"].dt.strftime("%Y-%m-%d %H:%M")
    return df

def _prep(df):
    d = core.load_symbol_df(df.copy())
    d = core.add_sessions(d, 17, 120, {"ALL": (0, 24)})
    sl, rl = core.last_index_arrays(d)
    return d, core.Bars(d), sl, rl

# ----------------------------------------------------------------------------- slow reference engine (B.2 / B.3)
def ref_engine(o, h, l, c, atr, sig, sess_last, roll_last, sl_atr, tp_r, max_bars, tick, cost, slip, delay, pess, start, end):
    out = []
    t = start
    while t < end:
        if not sig[t]:
            t += 1; continue
        e = t + 1 + delay
        if e >= end or e > sess_last[t] or e > roll_last[t] or not atr[t] > 0:
            t += 1; continue
        entry = o[e]
        if sl_atr > 0:
            slt = max(1, round(sl_atr * atr[t] / tick))
            stop = entry - slt * tick
            tgt = entry + max(1, round(tp_r * slt)) * tick if tp_r > 0 else math.inf
        else:
            stop, tgt = -math.inf, math.inf
        last = min(e + max_bars - 1, sess_last[e], roll_last[e], end - 1)
        res = None
        for j in range(e, last + 1):
            if j > e and o[j] <= stop:
                res = (o[j], "SL", j); break
            if j > e and o[j] >= tgt:
                res = (o[j], "TP", j); break
            hit_s, hit_t = l[j] <= stop, h[j] >= tgt
            if hit_s and hit_t:
                first_high = (h[j] - o[j]) < (o[j] - l[j])
                res = (tgt, "TP", j) if (not pess and first_high) else (stop, "SL", j); break
            if hit_s:
                res = (stop, "SL", j); break
            if hit_t:
                res = (tgt, "TP", j); break
        if res is None:
            x = e + max_bars
            if last == e + max_bars - 1 and x <= sess_last[e] and x <= roll_last[e] and x < end:
                res = (o[x], "TIME", x)
            else:
                res = (c[last], "EOS", last)
        px, kind, xb = res
        gross = (px - entry) / tick
        net = gross - cost - (slip if kind == "SL" else 0)
        out.append((t, e, xb, round(net, 6), kind))
        t = xb if xb > t else t + 1
    return out

def ref_baseline(o, c, atr, sess_last, roll_last, H, start, end):
    vals = []
    for t in range(start, end):
        e = t + 1
        if e >= end or e > sess_last[t] or e > roll_last[t] or not atr[t] > 0:
            continue
        last = min(e + H - 1, sess_last[e], roll_last[e], end - 1)
        x = e + H
        px = o[x] if (last == e + H - 1 and x <= sess_last[e] and x <= roll_last[e] and x < end) else c[last]
        vals.append((px - o[e]) / atr[t])
    return float(np.mean(vals))

# ----------------------------------------------------------------------------- slow reference rules (B.7 / B.8)
def ref_rules(o, h, l, c, T, a1, t):
    """All 60 rules at bar t, long form, scalar code. Any missing history -> NaN -> comparison is False."""
    nan = float("nan")
    g = lambda x, k: x[t - k] if t - k >= 0 else nan
    def win(x, k0, k1):
        if t - k1 < 0:
            return None
        w = [x[t - k] for k in range(k0, k1 + 1)]
        return None if any(v != v for v in w) else w
    hh = lambda N: max(win(h, 1, N)) if win(h, 1, N) else nan
    rng = [hi - lo for hi, lo in zip(h, l)]
    clv = (c[t] - l[t]) / rng[t] if rng[t] > 0 else nan
    d = lambda k: g(c, k) - g(T, k)
    a = a1[t]
    R = {}
    R["T3_BODY_CROSS"] = o[t] <= T[t] and c[t] > T[t]
    R["T3_CLOSE_CROSS"] = g(c, 1) <= g(T, 1) and c[t] > T[t]
    R["T3_TWO_CLOSE"] = c[t] > T[t] and g(c, 1) > g(T, 1) and g(c, 2) <= g(T, 2)
    R["T3_THREE_CLOSE"] = c[t] > T[t] and g(c, 1) > g(T, 1) and g(c, 2) > g(T, 2) and g(c, 3) <= g(T, 3)
    R["T3_WICK_RECLAIM"] = o[t] > T[t] and l[t] <= T[t] and c[t] > T[t]
    R["T3_CLEAN_RECLAIM"] = l[t] > T[t] and g(l, 1) <= g(T, 1) <= g(h, 1)
    R["T3_CROSS_RANGE_BREAK"] = g(c, 1) > g(T, 1) and g(c, 2) <= g(T, 2) and c[t] > g(h, 1)
    R["T3_PULLBACK_BREAK"] = T[t] > g(T, 1) and g(l, 1) <= g(T, 1) < g(c, 1) and c[t] > g(h, 1)
    R["T3_TOUCH_3BAR_TREND"] = T[t] > g(T, 1) and any(g(l, k) <= g(T, k) for k in (1, 2, 3)) and c[t] > T[t]
    R["T3_SLOPE_TURN"] = T[t] > g(T, 1) and g(T, 1) <= g(T, 2) and c[t] > T[t]
    R["T3_DISTANCE_RELEASE"] = d(0) > 0 and d(0) > d(1)
    for N in (3, 5, 8, 12, 20):
        R[f"DONCHIAN_{N}"] = c[t] > hh(N)
    R["PREVBAR_BREAK"] = c[t] > g(h, 1)
    R["INSIDE_BAR_BREAK"] = g(h, 1) <= g(h, 2) and g(l, 1) >= g(l, 2) and c[t] > g(h, 1)
    for N in (3, 5, 8):
        w = win(rng, 1, N)
        R[f"COMPRESSION_BREAK_{N}"] = bool(w) and rng[t - 1] <= min(w) and c[t] > hh(N)
    R["RANGE_EXPANSION_CLOSE"] = rng[t] > a and clv >= 0.75
    R["MOMENTUM_3"] = c[t] > g(c, 3)
    R["MOMENTUM_6"] = c[t] > g(c, 6)
    R["THREE_BAR_CONTINUATION"] = g(c, 1) > g(c, 2) and g(c, 2) > g(c, 3) and c[t] > g(h, 1)
    R["GAP_CONTINUATION"] = o[t] > g(c, 1) and c[t] > o[t]
    R["TWO_BAR_REVERSAL"] = g(c, 1) < g(o, 1) and c[t] > g(h, 1)
    R["OUTSIDE_BAR_REVERSAL"] = h[t] > g(h, 1) and l[t] < g(l, 1) and rng[t] > 0 and clv >= 0.75
    for N in (5, 10):
        w = win(l, 2, N + 1)
        lvl = min(w) if w else nan
        R[f"SWEEP_RECLAIM_{N}"] = g(l, 1) < lvl and c[t] > lvl
    w = win(c, 1, 20)
    mu = sum(w) / 20 if w else nan
    for k in (2, 3):
        R[f"MEAN_REVERSION_{k}"] = l[t] <= mu - k * a and c[t] > o[t]
    F = {"NONE": True}
    for x in (35, 55, 70):
        F[f"BODY_{x}"] = rng[t] > 0 and abs(c[t] - o[t]) >= x / 100 * rng[t]
    for x in (55, 65, 75):
        F[f"CLV_{x}"] = rng[t] > 0 and clv >= x / 100
    F["DIRECTIONAL_BODY"] = c[t] > o[t]
    F["FAVOURABLE_WICK_30"] = rng[t] > 0 and min(o[t], c[t]) - l[t] >= 0.30 * rng[t]
    F["NO_ADVERSE_WICK_20"] = rng[t] > 0 and h[t] - max(o[t], c[t]) <= 0.20 * rng[t]
    F["GAP_ALIGNED_010"] = o[t] - g(c, 1) >= 0.10 * a
    F["MOMENTUM_2"] = c[t] > g(c, 2)
    F["PREV_OPPOSITE_CANDLE"] = g(c, 1) < g(o, 1)
    w = win(rng, 1, 20)
    F["RANGE_COMPRESS_060"] = rng[t] <= 0.60 * float(np.median(w)) if w else False
    F["RANGE_EXPAND_125"] = rng[t] >= 1.25 * a
    F["RANGE_EXPAND_150"] = rng[t] >= 1.50 * a
    F["STRUCTURE_STEP"] = l[t] > g(l, 1)
    for x in ("025", "050"):
        F[f"T3_DIST_FAR_{x}"] = d(0) >= int(x) / 100 * a
    for x in ("025", "050", "100"):
        F[f"T3_DIST_NEAR_{x}"] = d(0) > 0 and d(0) <= int(x) / 100 * a
    F["T3_NOT_EXTENDED_075"] = d(0) <= 0.75 * a
    F["T3_SLOPE_NONNEG"] = T[t] - g(T, 3) >= 0
    for x in ("015", "030"):
        F[f"T3_SLOPE_STRONG_{x}"] = T[t] - g(T, 3) >= int(x) / 100 * a
    w = win(a1, 0, 49)
    med = float(np.median(w)) if w else nan
    F["VOL_HIGH_120"] = a >= 1.20 * med
    F["VOL_LOW_080"] = a <= 0.80 * med
    return R, F

def ref_t3(close, L, v):
    a = 2 / (L + 1)
    def ema(x):
        out = [x[0]]
        for xi in x[1:]:
            out.append(a * xi + (1 - a) * out[-1])
        return out
    e = [list(close)]
    for _ in range(6):
        e.append(ema(e[-1]))
    c1, c2, c3, c4 = -v ** 3, 3 * v ** 2 + 3 * v ** 3, -6 * v ** 2 - 3 * v - 3 * v ** 3, 1 + 3 * v + v ** 3 + 3 * v ** 2
    return np.array([c1 * e[6][i] + c2 * e[5][i] + c3 * e[4][i] + c4 * e[3][i] for i in range(len(close))])

def ref_atr(h, l, c, n=14):
    tr = [h[0] - l[0]] + [max(h[i] - l[i], abs(h[i] - c[i - 1]), abs(l[i] - c[i - 1])) for i in range(1, len(c))]
    out = [math.nan] * len(c)
    out[n - 1] = sum(tr[:n]) / n
    for i in range(n, len(c)):
        out[i] = (out[i - 1] * (n - 1) + tr[i]) / n
    return np.array(out)

# ----------------------------------------------------------------------------- the tests
def _one(o, h, l, c, sig, atr, sess_last=None, roll_last=None, **kw):
    n = len(o)
    sl = np.full(n, n - 1) if sess_last is None else np.asarray(sess_last)
    rl = np.full(n, n - 1) if roll_last is None else np.asarray(roll_last)
    p = dict(sl_atr=1.0, tp_r=1.0, max_bars=5, tick=0.01, cost_ticks=0.0, stop_slip=0.0, delay=0, pessimistic=True)
    p.update(kw)
    return _run(np.array(o, float), np.array(h, float), np.array(l, float), np.array(c, float), np.array(atr, float),
                np.array(sig, bool), sl, rl, p["sl_atr"], p["tp_r"], p["max_bars"], p["tick"], p["cost_ticks"],
                p["stop_slip"], p["delay"], p["pessimistic"], 0, n)

def run_all():
    out = []
    def check(name, cond, detail=""):
        out.append((name, bool(cond), str(detail)[:300]))

    # --- engine: hand-made cases (B.3)
    A = [1.0] * 8
    r = _one([100, 100, 100.5, 100, 100, 100, 100, 100], [100, 100.2, 101.2, 100, 100, 100, 100, 100],
             [100, 99.8, 100.3, 100, 100, 100, 100, 100], [100, 100, 101, 100, 100, 100, 100, 100], [1, 0, 0, 0, 0, 0, 0, 0], A)
    check("engine: target fills at the target", r[8][0] == 1 and abs(r[3][0] - 100) < 1e-6 and r[2][0] == 2, r)
    r = _one([100, 100, 100] + [100] * 5, [100, 101.5, 100] + [100] * 5, [100, 98.5, 100] + [100] * 5, [100] * 8, [1] + [0] * 7, A)
    check("engine: stop and target in one bar -> stop (pessimistic)", r[8][0] == 0 and abs(r[3][0] + 100) < 1e-6, r)
    r = _one([100, 100.9, 100] + [100] * 5, [100, 102.5, 100] + [100] * 5, [100, 98.5, 100] + [100] * 5, [100] * 8, [1] + [0] * 7, A,
             pessimistic=False)
    check("engine: same bar, open nearer the high -> target (TradingView path)", r[8][0] == 1, r)
    r = _one([100, 100, 98] + [98] * 5, [100, 100.2, 98.2] + [98] * 5, [100, 99.8, 97.5] + [98] * 5, [100, 100, 98] + [98] * 5, [1] + [0] * 7, A)
    check("engine: gap through the stop fills at the open", r[8][0] == 0 and abs(r[3][0] + 200) < 1e-6, r)
    o = [100, 100, 100.1, 100.2, 100.3, 100.4, 100.5, 100.6]
    hi = [x + 0.05 for x in o]; lo = [x - 0.05 for x in o]
    r = _one(o, hi, lo, o, [1] + [0] * 7, A, max_bars=3)
    check("engine: time exit at the open of entry + max_bars", r[8][0] == 2 and r[2][0] == 4 and abs(r[3][0] - 30) < 1e-6, r)
    r = _one(o, hi, lo, o, [1, 0, 0, 1, 0, 0, 0, 0], A, sess_last=[3, 3, 3, 3, 7, 7, 7, 7], max_bars=10)
    check("engine: session end closes at the last close; no entry from a session's last bar",
          len(r[0]) == 1 and r[8][0] == 3 and r[2][0] == 3, r)
    r = _one(o, hi, lo, o, [1, 1, 1, 0, 0, 0, 0, 0], A, max_bars=3)
    check("engine: one position at a time", len(r[0]) == 1, r)
    r = _one(o, hi, lo, o, [1] + [0] * 7, A, max_bars=3, cost_ticks=3)
    check("engine: round-trip cost deducted, gross kept", abs(r[3][0] - 27) < 1e-6 and abs(r[5][0] - 0.30) < 1e-9, r)
    r = _one(o, hi, lo, o, [1, 0, 0, 0, 0, 0, 0, 0], A, max_bars=3, delay=1)
    check("engine: one-bar delay enters at the open of t + 2", len(r[1]) == 1 and r[1][0] == 2, r)

    # --- engine and baseline vs the slow reference on random markets
    df = synthetic(160, seed=1)
    d, b, sl, rl = _prep(df)
    mism = []
    rs = np.random.default_rng(5)
    for side in (1, -1):
        O, H, L, C = (b.o, b.h, b.l, b.c) if side == 1 else (-b.o, -b.l, -b.h, -b.c)
        for sl_atr, tp_r, mb, pess, delay in ((0.0, 0.0, 8, True, 0), (1.0, 1.5, 16, True, 0), (0.5, 3.0, 4, False, 0),
                                              (2.0, 0.5, 32, True, 1), (0.75, 1.0, 12, False, 0), (1.25, 0.0, 6, True, 0)):
            sig = rs.random(b.n) < 0.08
            fast = _run(O, H, L, C, b.atr, sig, sl, rl, sl_atr, tp_r, mb, 0.01, 3.0, 1.0, delay, pess, 100, b.n - 50)
            slow = ref_engine(O, H, L, C, b.atr, sig, sl, rl, sl_atr, tp_r, mb, 0.01, 3.0, 1.0, delay, pess, 100, b.n - 50)
            f = [(int(fast[0][i]), int(fast[1][i]), int(fast[2][i]), round(float(fast[3][i]), 6), ("SL", "TP", "TIME", "EOS")[fast[8][i]])
                 for i in range(len(fast[0]))]
            if f != slow:
                mism.append((side, sl_atr, tp_r, mb, pess, delay))
    check("engine == slow reference engine (12 random configurations, both sides)", not mism, mism)
    bl = []
    for side in (1, -1):
        O, C = (b.o, b.c) if side == 1 else (-b.o, -b.c)
        m = Market(b, sl, rl, 0.01, 3, 1)
        bl.append(abs(m.baseline(side, 8, 100, b.n - 50) - ref_baseline(O, C, b.atr, sl, rl, 8, 100, b.n - 50)) < 1e-9)
    check("drift baseline == slow reference", all(bl))
    m = Market(b, sl, rl, 0.01, 3, 1)
    tr = m.trades(core.trigger(b, "PREVBAR_BREAK", 1), 1, 1.0, 1.5, 40)
    ok = np.all(d["sess"].to_numpy()[tr["entry_bar"]] == d["sess"].to_numpy()[tr["exit_bar"]]) and np.all(tr["exit_bar"] <= rl[tr["entry_bar"]])
    check("no trade crosses a session end or a roll", ok)
    check("sessions: one session per synthetic trading day, rolls detected",
          d["sess"].nunique() == 160 and int(d["roll"].sum()) == 2)

    # --- indicators vs slow reference
    check("T3 == slow reference (6 cascaded EMAs)", np.allclose(core.t3(b.c[:600], 5, 0.7), ref_t3(b.c[:600], 5, 0.7), rtol=0, atol=1e-9))
    check("ATR == slow Wilder reference", np.allclose(core.atr_wilder(b.h, b.l, b.c)[13:], ref_atr(b.h, b.l, b.c)[13:], atol=1e-12))
    v = 0.7
    check("T3 coefficients sum to 1", abs((-v ** 3) + (3 * v * v + 3 * v ** 3) + (-6 * v * v - 3 * v - 3 * v ** 3) + (1 + 3 * v + v ** 3 + 3 * v * v) - 1) < 1e-12)

    # --- every rule vs the slow reference (long), mirror (short), causality, reachability
    n = 1500
    T = b.t3(5, 0.7)
    lists = [x[:n].tolist() for x in (b.o, b.h, b.l, b.c, T, b.atr1)]
    refR = {k: [] for k in core.TRIGGER_NAMES}; refF = {k: [] for k in core.FILTER_NAMES}
    for t in range(n):
        Rr, Ff = ref_rules(*lists, t)
        for k in refR: refR[k].append(bool(Rr[k]))
        for k in refF: refF[k].append(bool(Ff[k]))
    bad = [k for k in core.TRIGGER_NAMES if not np.array_equal(core.trigger(b, k, 1, 5, 0.7)[:n], np.array(refR[k]))]
    bad += [k for k in core.FILTER_NAMES if not np.array_equal(core.filt(b, k, 1, 5, 0.7)[:n], np.array(refF[k]))]
    check("all 32 triggers + 28 filters == slow reference (1500 bars)", not bad, bad)
    dn = d.copy()
    dn["open"], dn["high"], dn["low"], dn["close"] = -d["open"], -d["low"], -d["high"], -d["close"]
    bn = core.Bars(dn)
    bad = [k for k in core.TRIGGER_NAMES if not np.array_equal(core.trigger(b, k, -1, 5, 0.7), core.trigger(bn, k, 1, 5, 0.7))]
    bad += [k for k in core.FILTER_NAMES if not np.array_equal(core.filt(b, k, -1, 5, 0.7), core.filt(bn, k, 1, 5, 0.7))]
    check("short rules are exact mirrors of long rules", not bad, bad)
    cut = 3000
    d2 = d.copy(); noise = np.random.default_rng(9).normal(0, 1, len(d2) - cut)
    for col in ("open", "high", "low", "close"):
        d2.loc[cut:, col] = d2.loc[cut:, col].to_numpy() + noise
    d2["high"] = d2[["open", "high", "low", "close"]].max(axis=1); d2["low"] = d2[["open", "high", "low", "close"]].min(axis=1)
    b2 = core.Bars(d2)
    bad = []
    for s in (1, -1):
        bad += [k for k in core.TRIGGER_NAMES if not np.array_equal(core.trigger(b, k, s, 5, 0.7)[:cut], core.trigger(b2, k, s, 5, 0.7)[:cut])]
        bad += [k for k in core.FILTER_NAMES if not np.array_equal(core.filt(b, k, s, 5, 0.7)[:cut], core.filt(b2, k, s, 5, 0.7)[:cut])]
    check("no look-ahead: future bars never change past signals", not bad, bad)
    dead = [k for k in core.TRIGGER_NAMES if core.trigger(b, k, 1, 5, 0.7).sum() == 0] + \
           [k for k in core.FILTER_NAMES if core.filt(b, k, 1, 5, 0.7).sum() == 0]
    check("every trigger and filter fires on random data", not dead, dead)

    # --- statistics vs independent references
    import statsmodels.api as sm
    from statsmodels.stats.multitest import multipletests
    rg = np.random.default_rng(1)
    x = rg.normal(0.05, 1, 800) + 0.3 * np.r_[0, rg.normal(0, 1, 799)]
    L = int(np.floor(4 * (800 / 100) ** (2 / 9)))
    f = sm.OLS(x, np.ones(800)).fit(cov_type="HAC", cov_kwds={"maxlags": L, "use_correction": False})
    check("Newey-West t == statsmodels HAC", abs(S.nw_tstat(x) - f.params[0] / f.bse[0]) < 1e-8, (S.nw_tstat(x), f.params[0] / f.bse[0]))
    p = rg.uniform(0, 1, 300) ** 2
    check("Benjamini-Hochberg == statsmodels fdr_bh", np.allclose(S.bh(p, 0.1)[1], multipletests(p, 0.1, "fdr_bh")[1]))
    mc = np.mean(np.max(rg.normal(0, 0.1, (20000, 50)), axis=1))
    check("expected max SR formula within 3% of Monte Carlo (N=50)", abs(S.expected_max_sr(0.01, 50) / mc - 1) < 0.03, (S.expected_max_sr(0.01, 50), mc))
    y = rg.normal(0.08, 1, 500)
    srm, g3, g4 = S._sr_moments(y); mt = S.min_trl(y)
    z = srm * np.sqrt(mt - 1) / np.sqrt(1 - g3 * srm + (g4 - 1) / 4 * srm ** 2)
    check("MinTRL is where PSR reaches 95%", abs(z - 1.6448536) < 1e-6, z)
    check("PSR is 0.5 at SR = SR*", abs(S.psr(y, S.sharpe(y)) - 0.5) < 1e-9)
    sr_null = [S.sharpe(rg.standard_t(5, 400)) for _ in range(4000)]
    check("null SR variance 1/(T-1) matches simulation (fat tails, T=400)", abs(np.var(sr_null) / S.null_sr_var(400) - 1) < 0.08,
          (np.var(sr_null), S.null_sr_var(400)))
    best = []
    for _ in range(300):
        Mn = rg.normal(0, 1, (300, 40)); best.append(S.dsr(Mn[:, np.argmax(Mn.mean(0))], 40))
    check("DSR of the best of 40 pure-noise trials exceeds 0.95 in about 5% of cases or fewer", np.mean(np.array(best) >= 0.95) <= 0.08,
          np.mean(np.array(best) >= 0.95))
    pn = S.pbo_cscv(rg.normal(0, 1, (1600, 40)), 16)
    Md = rg.normal(0, 1, (1600, 40)); Md[:, 7] += 0.4
    pd_ = S.pbo_cscv(Md, 16)
    check("PBO: pure noise near 0.5, one real winner near 0", 0.3 < pn < 0.7 and pd_ < 0.05, (pn, pd_))
    bt1 = S.boot_mean(rg.normal(0.3, 1, 600), 999, 3); bt0 = S.boot_mean(rg.normal(0, 1, 600), 999, 3)
    check("stationary bootstrap: real mean detected, CI covers it; noise not detected",
          bt1["p"] < 0.01 and bt1["lo"] < 0.3 < bt1["hi"] and bt0["p"] > 0.01, (bt1, bt0))
    Ms = rg.normal(0, 1, (700, 25)); p0 = S.spa_pvalue(Ms, 300); Ms[:, 3] += 0.25; p1 = S.spa_pvalue(Ms, 300)
    check("Hansen SPA: noise not significant, a real winner significant", p0 > 0.05 and p1 < 0.05, (p0, p1))
    base = rg.normal(0, 1, (900, 3))
    Mk = np.column_stack([base[:, g] + 0.2 * rg.normal(0, 1, 900) for g in range(3) for _ in range(5)])
    check("K_eff finds 3 clusters in 3 groups of 5 correlated series", S.k_eff(Mk) == 3, S.k_eff(Mk))
    check("randomization p-value (1 + #null >= obs) / (B + 1)", S.emp_p([1, 2, 3, 4], 3.5) == 2 / 5)
    return out
