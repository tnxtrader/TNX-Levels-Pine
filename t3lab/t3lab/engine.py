"""Bar-level execution engine. Identical rules to T3_Entry_Lab_v2.pine.

Signal at the CLOSE of bar t -> market entry at the OPEN of bar t+1(+delay).
Short trades are run on mirrored prices (o,h,l,c) -> (-o,-l,-h,-c), so one long-only engine serves both.
Barriers are fixed at the fill: SL = round(sl_atr * ATR[t] / tick) ticks, TP = round(tp_r * SL) ticks.
Exit priority inside a bar: gap through a barrier fills at the open; if SL and TP are both inside the
bar range -> SL (pessimistic, default) or TradingView's O->H->L->C / O->L->H->C path (intrabar='tv').
Time exit: order at the close of bar entry+max_bars-1 -> fills at the next open.
No position crosses a session end or a contract roll: it is closed at that bar's close.
pyramiding = 0: signals while a trade is open are ignored (a signal on the exit bar is allowed).
"""
from __future__ import annotations
import numpy as np
from numba import njit

EXIT_NAMES = np.array(["SL", "TP", "TIME", "EOS"])

@njit(cache=True)
def _run(o, h, l, c, atr, sig, sess_last, roll_last, sl_atr, tp_r, max_bars, tick,
         cost_ticks, stop_slip, delay, pessimistic, start, end):
    n_cap = int(sig[start:end].sum()) + 1
    eb = np.empty(n_cap, np.int64); xb = np.empty(n_cap, np.int64); sb = np.empty(n_cap, np.int64)
    pnl = np.empty(n_cap); ratr = np.empty(n_cap); mfe = np.empty(n_cap); mae = np.empty(n_cap)
    et = np.empty(n_cap, np.int64)
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
        if sl_atr > 0:
            sl_t = max(1.0, np.round(sl_atr * a / tick))
            stop = entry - sl_t * tick
        else:
            stop = -np.inf
        if tp_r > 0 and sl_atr > 0:
            tgt = entry + max(1.0, np.round(tp_r * sl_t)) * tick
        else:
            tgt = np.inf
        last = min(e + max_bars - 1, sess_last[e], roll_last[e], end - 1)
        up = 0.0; dn = 0.0; px = np.nan; typ = -1; xbar = last
        for j in range(e, last + 1):
            if j > e:
                if o[j] <= stop:
                    px = o[j]; typ = 0; xbar = j; break
                if o[j] >= tgt:
                    px = o[j]; typ = 1; xbar = j; break
            hs = l[j] <= stop
            ht = h[j] >= tgt
            if hs and ht:
                if pessimistic or (h[j] - o[j]) >= (o[j] - l[j]):   # TV: low first when open is nearer the low
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
        ticks = (px - entry) / tick - cost_ticks - (stop_slip if typ == 0 else 0.0)
        eb[k] = e; xb[k] = xbar; sb[k] = i; pnl[k] = ticks; ratr[k] = ticks * tick / a
        mfe[k] = up / a; mae[k] = dn / a; et[k] = typ
        k += 1
        i = xbar if xbar > i else i + 1
    return sb[:k], eb[:k], xb[:k], pnl[:k], ratr[:k], mfe[:k], mae[:k], et[:k]


class Market:
    """Holds one symbol's arrays (both sides) and session bookkeeping for the engine."""
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
        r = _run(o, h, l, c, self.b.atr, sig.astype(np.bool_), self.sess_last, self.roll_last,
                 float(sl_atr), float(tp_r), int(max_bars), float(self.tick),
                 float(self.cost * cost_mult + 2 * extra_slip), float(self.slip + extra_slip),
                 int(delay), bool(self.pess), int(start), int(end))
        keys = ("sig_bar", "entry_bar", "exit_bar", "pnl_ticks", "r_atr", "mfe_atr", "mae_atr", "exit_type")
        return dict(zip(keys, r))

    def per_session(self, tr):
        """Net R (ATR units) summed by entry session; zeros on sessions without a trade."""
        out = np.zeros(self.n_sess)
        np.add.at(out, self.sess[tr["entry_bar"]], tr["r_atr"])
        return out
