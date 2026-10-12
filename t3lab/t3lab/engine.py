"""Bar-level execution engine (same rules as T3_Entry_Lab_v2.pine).

Signal at the CLOSE of bar t -> market entry at the OPEN of bar e = t + 1 (+ delay).
Shorts run on mirrored prices (o, h, l, c) -> (-o, -l, -h, -c), so one long-only engine serves both sides.
Stop = max(1, round(sl_atr * ATR[t] / tick)) ticks, target = max(1, round(tp_r * stop ticks)) ticks, fixed at the fill.
Inside a bar: gap through a level fills at the open; stop and target both inside the bar -> stop (pessimistic) or the
TradingView path (open nearer the high -> high first). Time exit: order at the close of bar e + max_bars - 1, filled at
the next open. No position crosses a session end or a contract roll: it is closed at that bar's close.
One position at a time; a signal on the exit bar is allowed.
"""
from __future__ import annotations
import numpy as np
from numba import njit

EXIT_NAMES = ("SL", "TP", "TIME", "EOS")

@njit(cache=True)
def _run(o, h, l, c, atr, sig, sess_last, roll_last, sl_atr, tp_r, max_bars, tick,
         cost_ticks, stop_slip, delay, pessimistic, start, end):
    n_cap = int(sig[start:end].sum()) + 1
    eb = np.empty(n_cap, np.int64); xb = np.empty(n_cap, np.int64); sb = np.empty(n_cap, np.int64)
    net = np.empty(n_cap); ratr = np.empty(n_cap); gatr = np.empty(n_cap)
    mfe = np.empty(n_cap); mae = np.empty(n_cap); et = np.empty(n_cap, np.int64)
    k = 0
    i = start
    while i < end:
        if not sig[i]:
            i += 1
            continue
        e = i + 1 + delay
        a = atr[i]
        if e >= end or e > sess_last[i] or e > roll_last[i] or not (a > 0):
            i += 1
            continue
        entry = o[e]
        sl_t = 0.0
        stop = -np.inf
        tgt = np.inf
        if sl_atr > 0:
            sl_t = max(1.0, np.round(sl_atr * a / tick))
            stop = entry - sl_t * tick
            if tp_r > 0:
                tgt = entry + max(1.0, np.round(tp_r * sl_t)) * tick
        last = min(e + max_bars - 1, sess_last[e], roll_last[e], end - 1)
        up = 0.0; dn = 0.0; px = np.nan; typ = -1; xbar = last
        for j in range(e, last + 1):
            if j > e:
                if o[j] <= stop:
                    px = o[j]; typ = 0; xbar = j
                    break
                if o[j] >= tgt:
                    px = o[j]; typ = 1; xbar = j
                    break
            hs = l[j] <= stop
            ht = h[j] >= tgt
            if hs and ht:
                if pessimistic or (h[j] - o[j]) >= (o[j] - l[j]):
                    px = stop; typ = 0
                else:
                    px = tgt; typ = 1
            elif hs:
                px = stop; typ = 0
            elif ht:
                px = tgt; typ = 1
            up = max(up, min(h[j], tgt) - entry)
            dn = max(dn, entry - max(l[j], stop))
            if typ >= 0:
                xbar = j
                break
        if typ < 0:
            nxt = e + max_bars
            if last == e + max_bars - 1 and nxt <= sess_last[e] and nxt <= roll_last[e] and nxt < end:
                px = o[nxt]; typ = 2; xbar = nxt
            else:
                px = c[last]; typ = 3; xbar = last
        gross_ticks = (px - entry) / tick
        ticks = gross_ticks - cost_ticks - (stop_slip if typ == 0 else 0.0)
        eb[k] = e; xb[k] = xbar; sb[k] = i; net[k] = ticks
        ratr[k] = ticks * tick / a; gatr[k] = gross_ticks * tick / a
        mfe[k] = up / a; mae[k] = dn / a; et[k] = typ
        k += 1
        i = xbar if xbar > i else i + 1
    return sb[:k], eb[:k], xb[:k], net[:k], ratr[:k], gatr[:k], mfe[:k], mae[:k], et[:k]

@njit(cache=True)
def _baseline(o, c, atr, sess_last, roll_last, max_bars, start, end):
    """Mean gross R (ATR units) of the screen exit (pure time exit) entered after EVERY eligible bar t in [start, end):
    the return of simply being in the market for that long (drift)."""
    tot = 0.0; n = 0
    for i in range(start, end):
        e = i + 1
        a = atr[i]
        if e >= end or e > sess_last[i] or e > roll_last[i] or not (a > 0):
            continue
        last = min(e + max_bars - 1, sess_last[e], roll_last[e], end - 1)
        nxt = e + max_bars
        if last == e + max_bars - 1 and nxt <= sess_last[e] and nxt <= roll_last[e] and nxt < end:
            px = o[nxt]
        else:
            px = c[last]
        tot += (px - o[e]) / a
        n += 1
    return tot / n if n > 0 else 0.0


class Market:
    """One instrument's arrays (both sides) and session bookkeeping for the engine."""
    def __init__(self, bars, sess_last, roll_last, tick, cost_ticks, stop_slip, pessimistic=True):
        self.b = bars
        self.side = {1: (bars.o, bars.h, bars.l, bars.c), -1: (-bars.o, -bars.l, -bars.h, -bars.c)}
        self.sess = bars.df["sess"].to_numpy()
        self.n_sess = int(self.sess.max()) + 1
        self.sess_last, self.roll_last = sess_last, roll_last
        self.tick, self.cost, self.slip, self.pess = tick, cost_ticks, stop_slip, pessimistic

    def trades(self, sig, side, sl_atr=0.0, tp_r=0.0, max_bars=8, start=0, end=None,
               cost_mult=1.0, extra_slip=0.0, delay=0):
        o, h, l, c = self.side[side]
        end = self.b.n if end is None else end
        r = _run(o, h, l, c, self.b.atr, np.asarray(sig, np.bool_), self.sess_last, self.roll_last,
                 float(sl_atr), float(tp_r), int(max_bars), float(self.tick),
                 float(self.cost * cost_mult + 2 * extra_slip), float(self.slip + extra_slip),
                 int(delay), bool(self.pess), int(start), int(end))
        keys = ("sig_bar", "entry_bar", "exit_bar", "net_ticks", "r_atr", "g_atr", "mfe_atr", "mae_atr", "exit_type")
        return dict(zip(keys, r))

    def baseline(self, side, max_bars, start, end):
        o, h, l, c = self.side[side]
        return float(_baseline(o, c, self.b.atr, self.sess_last, self.roll_last, int(max_bars), int(start), int(end)))

    def per_session(self, tr, key="r_atr", values=None):
        """Sum of a per-trade value by entry session; zeros on sessions without a trade."""
        out = np.zeros(self.n_sess)
        np.add.at(out, self.sess[tr["entry_bar"]], tr[key] if values is None else values)
        return out
