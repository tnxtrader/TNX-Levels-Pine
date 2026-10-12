"""Synthetic data + engine/signal self-tests. Run by Phase 1 and by pytest (tests/test_selftest.py).
Nothing here touches real data."""
from __future__ import annotations
import numpy as np, pandas as pd
from . import core
from .engine import Market, _run

def synthetic(n_sessions=300, bars=46, seed=0, tick=0.01, start="2015-01-04 17:00", roll_every=60, px0=None):
    """Random-walk 30-min OHLC with CME-style sessions (17:00-16:00 CT, 1h halt), weekends,
    contract rolls with an additive back-adjustment gap. Has NO edge by construction."""
    rng = np.random.default_rng(seed)
    ts, rows, px, k = [], [], (10000 * tick if px0 is None else px0), 0
    day = pd.Timestamp(start)
    while k < n_sessions:
        if day.weekday() in (4, 5):            # sessions open Sun-Thu 17:00
            day += pd.Timedelta(days=1); continue
        sig = 0.002 * px * np.exp(rng.normal(0, 0.3))
        for j in range(bars):
            t = day + pd.Timedelta(minutes=30 * j)
            path = px + np.cumsum(rng.normal(0, sig / 3, 4))
            o = px; c = path[-1]
            h = max(o, c, path.max()) + abs(rng.normal(0, sig / 4)); l = min(o, c, path.min()) - abs(rng.normal(0, sig / 4))
            q = lambda x: np.round(x / tick) * tick
            rows.append((t, q(o), q(h), q(l), q(c), int(rng.integers(1, 500)), f"C{k // roll_every:03d}"))
            px = c + rng.normal(0, sig / 6)        # small gap to next open
        day += pd.Timedelta(days=1); k += 1
    df = pd.DataFrame(rows, columns=["timestamp", "open", "high", "low", "close", "volume", "contract"])
    df["timestamp"] = df["timestamp"].dt.strftime("%Y-%m-%d %H:%M")
    return df

def _prep(df):
    d = core.load_symbol_df(df.copy())
    d = core.add_sessions(d, 17, 120, {"ALL": (0, 24)})
    sl, rl = core.last_index_arrays(d)
    b = core.Bars(d)
    return d, b, sl, rl

def _one(o, h, l, c, sig, atr, sess_last=None, roll_last=None, **kw):
    n = len(o)
    sl = np.full(n, n - 1) if sess_last is None else sess_last
    rl = np.full(n, n - 1) if roll_last is None else roll_last
    p = dict(sl_atr=1.0, tp_r=1.0, max_bars=5, tick=0.01, cost_ticks=0.0, stop_slip=0.0, delay=0, pessimistic=True)
    p.update(kw)
    r = _run(np.array(o, float), np.array(h, float), np.array(l, float), np.array(c, float), np.array(atr, float),
             np.array(sig, bool), sl, rl, p["sl_atr"], p["tp_r"], p["max_bars"], p["tick"], p["cost_ticks"],
             p["stop_slip"], p["delay"], p["pessimistic"], 0, n)
    return r

def run_all():
    out = []
    def check(name, cond, detail=""):
        out.append((name, bool(cond), detail))
    A = [1.0] * 8
    # 1 TP hit: entry 100 at bar1 open, TP 101
    r = _one([100, 100, 100.5, 100, 100, 100, 100, 100], [100, 100.2, 101.2, 100, 100, 100, 100, 100],
             [100, 99.8, 100.3, 100, 100, 100, 100, 100], [100, 100, 101, 100, 100, 100, 100, 100], [1, 0, 0, 0, 0, 0, 0, 0], A)
    check("TP fills at target", r[7][0] == 1 and abs(r[3][0] - 100) < 1e-6 and r[2][0] == 2, str(r))
    # 2 both barriers inside one bar -> pessimistic SL
    r = _one([100, 100, 100] + [100] * 5, [100, 101.5, 100] + [100] * 5, [100, 98.5, 100] + [100] * 5, [100] * 8, [1] + [0] * 7, A)
    check("SL+TP same bar -> SL (pessimistic)", r[7][0] == 0 and abs(r[3][0] + 100) < 1e-6, str(r))
    r = _one([100, 100.9, 100] + [100] * 5, [100, 102.5, 100] + [100] * 5, [100, 98.5, 100] + [100] * 5, [100] * 8, [1] + [0] * 7, A,
             pessimistic=False)
    check("SL+TP same bar, open nearer high -> TP (TV path)", r[7][0] == 1, str(r))
    # 3 gap through stop fills at open
    r = _one([100, 100, 98] + [98] * 5, [100, 100.2, 98.2] + [98] * 5, [100, 99.8, 97.5] + [98] * 5, [100, 100, 98] + [98] * 5, [1] + [0] * 7, A)
    check("gap through stop fills at open", r[7][0] == 0 and abs(r[3][0] + 200) < 1e-6, str(r))
    # 4 time exit at open of entry+max_bars
    o = [100, 100, 100.1, 100.2, 100.3, 100.4, 100.5, 100.6]
    r = _one(o, [x + 0.05 for x in o], [x - 0.05 for x in o], o, [1] + [0] * 7, A, max_bars=3)
    check("time exit at open of entry+max_bars", r[7][0] == 2 and r[2][0] == 4 and abs(r[3][0] - 30) < 1e-6, str(r))
    # 5 session end closes at last bar close, no entry on last session bar
    sl = np.array([3, 3, 3, 3, 7, 7, 7, 7])
    r = _one(o, [x + 0.05 for x in o], [x - 0.05 for x in o], o, [1, 0, 0, 1, 0, 0, 0, 0], A, sess_last=sl, max_bars=10)
    check("session flatten at close; no entry from last session bar", len(r[0]) == 1 and r[7][0] == 3 and r[2][0] == 3, str(r))
    # 6 pyramiding=0
    r = _one(o, [x + 0.05 for x in o], [x - 0.05 for x in o], o, [1, 1, 1, 0, 0, 0, 0, 0], A, max_bars=3)
    check("pyramiding=0 (signals in trade ignored)", len(r[0]) == 1, str(r))
    # 7 costs
    r = _one(o, [x + 0.05 for x in o], [x - 0.05 for x in o], o, [1] + [0] * 7, A, max_bars=3, cost_ticks=3)
    check("round-trip cost deducted", abs(r[3][0] - 27) < 1e-6, str(r))
    # signal-level tests on a synthetic market
    df = synthetic(120, seed=1)
    d, b, sl, rl = _prep(df)
    names_t = [t for t in core.TRIGGER_NAMES]; names_f = [f for f in core.FILTER_NAMES]
    # mirror: short signal on price == long signal on negated price
    dneg = d.copy()
    dneg["open"], dneg["high"], dneg["low"], dneg["close"] = -d["open"], -d["low"], -d["high"], -d["close"]
    bneg = core.Bars(dneg)
    bad = [x for x in names_t if not np.array_equal(core.trigger(b, x, -1, 5, 0.7), core.trigger(bneg, x, 1, 5, 0.7))]
    bad += [x for x in names_f if not np.array_equal(core.filt(b, x, -1, 5, 0.7), core.filt(bneg, x, 1, 5, 0.7))]
    check("short rules are exact mirrors of long", not bad, ",".join(bad))
    # causality: changing bars after t never changes signals at <= t
    cut = 3000
    d2 = d.copy(); rng = np.random.default_rng(9)
    noise = rng.normal(0, 1, len(d2) - cut)
    for col in ("open", "high", "low", "close"):
        d2.loc[cut:, col] = d2.loc[cut:, col].to_numpy() + noise
    d2["high"] = d2[["open", "high", "low", "close"]].max(1); d2["low"] = d2[["open", "high", "low", "close"]].min(1)
    b2 = core.Bars(d2)
    bad = []
    for s in (1, -1):
        bad += [x for x in names_t if not np.array_equal(core.trigger(b, x, s, 5, 0.7)[:cut], core.trigger(b2, x, s, 5, 0.7)[:cut])]
        bad += [x for x in names_f if not np.array_equal(core.filt(b, x, s, 5, 0.7)[:cut], core.filt(b2, x, s, 5, 0.7)[:cut])]
    check("no look-ahead in any trigger/filter", not bad, ",".join(bad))
    # T3 sanity
    v = 0.7; cs = [-v**3, 3*v*v + 3*v**3, -6*v*v - 3*v - 3*v**3, 1 + 3*v + v**3 + 3*v*v]
    check("T3 coefficients sum to 1", abs(sum(cs) - 1) < 1e-12)
    check("T3 of a constant is the constant", np.allclose(core.t3(np.full(200, 5.0), 8, 0.7), 5.0))
    # every trigger/filter fires sometimes on random data (definitions are reachable)
    dead = [x for x in names_t if core.trigger(b, x, 1, 5, 0.7).sum() == 0] + \
           [x for x in names_f if core.filt(b, x, 1, 5, 0.7).sum() == 0]
    check("every trigger/filter fires on random data", not dead, ",".join(dead))
    # engine on real session bookkeeping: no trade crosses a session or a roll
    m = Market(b, sl, rl, 0.01, 3, 1)
    tr = m.trades(core.trigger(b, "PREVBAR_BREAK", 1), 1, 1.0, 1.5, 40)
    ok = np.all(d["sess"].to_numpy()[tr["entry_bar"]] == d["sess"].to_numpy()[np.minimum(tr["exit_bar"], len(d) - 1)])
    ok &= np.all(tr["exit_bar"] <= rl[tr["entry_bar"]])
    check("no trade crosses a session end or a roll", ok)
    return out
