"""Faithful Python replica of T3_Entry_Lab_Phase0_AUDITED.pine for research (NOT a trading system).

Fidelity choices (match TradingView semantics):
  * Rule expressions are translated mechanically from the Pine source (no hand re-typing).
  * Signal on bar t close (flat at close of t) -> market entry at open of t+1.
  * TP/SL distances = round(mult * ATR_t / mintick) ticks, measured from the fill price.
  * Broker-emulator intrabar path: open nearer high -> O,H,L,C else O,L,H,C; a bar opening beyond a level fills at the open.
  * Exit orders are live on the fill bar itself.
  * Time barrier: close_all issued at close of bar fill+maxBars-1 -> fills at open of bar fill+maxBars.
  * ta.ema seeded with first source value (Pine reference implementation); ta.atr = RMA(TR) seeded with SMA.
"""
import ast, re, pathlib
import numpy as np
from numba import njit

PINE = pathlib.Path(__file__).resolve().parents[2] / "T3_Entry_Lab_Phase0_AUDITED.pine"

# ---------------------------------------------------------------- indicators
@njit(cache=True)
def ema_pine(x, n):
    a = 2.0 / (n + 1.0)
    out = np.empty_like(x)
    out[0] = x[0]
    for i in range(1, len(x)):
        out[i] = a * x[i] + (1 - a) * out[i - 1]
    return out

def t3(src, n, v):
    e1 = ema_pine(src, n); e2 = ema_pine(e1, n); e3 = ema_pine(e2, n)
    e4 = ema_pine(e3, n); e5 = ema_pine(e4, n); e6 = ema_pine(e5, n)
    c1 = -v**3; c2 = 3*v*v + 3*v**3; c3 = -6*v*v - 3*v - 3*v**3; c4 = 1 + 3*v + v**3 + 3*v*v
    return c1*e6 + c2*e5 + c3*e4 + c4*e3

def atr_pine(h, l, c, n):
    pc = np.r_[np.nan, c[:-1]]
    tr = np.where(np.isnan(pc), h - l, np.maximum(h - l, np.maximum(np.abs(h - pc), np.abs(l - pc))))
    out = np.full_like(c, np.nan)
    if len(c) < n:
        return out
    out[n - 1] = tr[:n].mean()
    for i in range(n, len(c)):
        out[i] = (out[i - 1] * (n - 1) + tr[i]) / n
    return out

def rolling_max(x, n):
    out = np.full_like(x, np.nan)
    for i in range(n - 1, len(x)):
        out[i] = x[i - n + 1:i + 1].max()
    return out

def rolling_min(x, n):
    out = np.full_like(x, np.nan)
    for i in range(n - 1, len(x)):
        out[i] = x[i - n + 1:i + 1].min()
    return out

# ---------------------------------------------------------------- rule compiler
class _Eval(ast.NodeVisitor):
    def __init__(self, env):
        self.env = env
    def visit_Expression(self, n): return self.visit(n.body)
    def visit_BoolOp(self, n):
        vals = [self.visit(v) for v in n.values]
        f = np.logical_and if isinstance(n.op, ast.And) else np.logical_or
        out = vals[0]
        for v in vals[1:]:
            out = f(out, v)
        return out
    def visit_Compare(self, n):
        left = self.visit(n.left); out = None
        for op, comp in zip(n.ops, n.comparators):
            right = self.visit(comp)
            r = {ast.Lt: np.less, ast.LtE: np.less_equal, ast.Gt: np.greater, ast.GtE: np.greater_equal,
                 ast.Eq: np.equal, ast.NotEq: np.not_equal}[type(op)](left, right)
            out = r if out is None else np.logical_and(out, r)
            left = right
        return out
    def visit_BinOp(self, n):
        a, b = self.visit(n.left), self.visit(n.right)
        return {ast.Add: np.add, ast.Sub: np.subtract, ast.Mult: np.multiply, ast.Div: np.divide}[type(n.op)](a, b)
    def visit_UnaryOp(self, n):
        v = self.visit(n.operand)
        return -v if isinstance(n.op, ast.USub) else np.logical_not(v)
    def visit_Constant(self, n): return n.value
    def visit_Name(self, n): return self.env[n.id]
    def visit_Subscript(self, n):
        arr = self.visit(n.value); k = self.visit(n.slice)
        out = np.full_like(arr, np.nan, dtype=float)
        out[k:] = arr[:-k] if k else arr
        return out
    def visit_Call(self, n):
        args = [self.visit(a) for a in n.args]
        return {"mx": np.maximum, "mn": np.minimum, "ab": np.abs}[n.func.id](*args)

def parse_rules(path=PINE):
    src = path.read_text()
    longpart, shortpart = src.split("bool shortCond")
    shortpart = shortpart.split("// ---------- Execution")[0]
    pat = re.compile(r"^\s+(\d+) => (.*?)\s*//\s*(\S+)\s*$", re.M)
    rules = {}
    for side, part in (("long", longpart), ("short", shortpart)):
        for k, expr, name in pat.findall(part):
            e = (expr.replace("math.max", "mx").replace("math.min", "mn").replace("math.abs", "ab")
                     .replace("syminfo.mintick", "mintick"))
            rules.setdefault(int(k), {})[side] = (e, name)
    return rules

def compile_signals(o, h, l, c, t3v, atrv, mintick, rules=None):
    rules = rules or parse_rules()
    env = dict(open=o, high=h, low=l, close=c, t3=t3v, atrv=atrv, hh3=rolling_max(h, 3), ll3=rolling_min(l, 3),
               mintick=mintick)
    ev = _Eval(env)
    sig = {}
    with np.errstate(invalid="ignore", divide="ignore"):
        for k, d in rules.items():
            for side, (e, name) in d.items():
                sig[(k, side)] = np.asarray(ev.visit(ast.parse(e, mode="eval")), dtype=bool)
    return sig, rules

# ---------------------------------------------------------------- backtester (exact Lab semantics)
@njit(cache=True)
def backtest(o, h, l, c, sig, atrv, direction, tp_mult, sl_mult, max_bars, mintick,
             cost_ticks_rt, stop_slip_ticks, start):
    """Returns per-trade arrays: entry bar, exit bar, pnl_ticks (net), R (pnl/ATR_signal), exit type (1 TP, -1 SL, 0 time),
    MFE and MAE in ATR units."""
    n = len(c)
    eb = np.empty(n, np.int64); xb = np.empty(n, np.int64); pnl = np.empty(n); rr = np.empty(n)
    et = np.empty(n, np.int64); mfe = np.empty(n); mae = np.empty(n)
    k = 0
    t = start
    while t < n - 1:
        if not sig[t] or not (atrv[t] > 0):
            t += 1
            continue
        tp_t = max(1.0, np.round(tp_mult * atrv[t] / mintick)); sl_t = max(1.0, np.round(sl_mult * atrv[t] / mintick))
        f = t + 1
        entry = o[f]
        tp = entry + direction * tp_t * mintick
        sl = entry - direction * sl_t * mintick
        exit_px = np.nan; typ = 0; xbar = -1
        best = 0.0; worst = 0.0
        last = min(f + max_bars - 1, n - 1)
        for b in range(f, last + 1):
            if b > f:   # gap through a level at the open
                if direction == 1:
                    if o[b] >= tp: exit_px = o[b]; typ = 1; xbar = b; break
                    if o[b] <= sl: exit_px = o[b]; typ = -1; xbar = b; break
                else:
                    if o[b] <= tp: exit_px = o[b]; typ = 1; xbar = b; break
                    if o[b] >= sl: exit_px = o[b]; typ = -1; xbar = b; break
            hi_first = (h[b] - o[b]) < (o[b] - l[b])
            # walk the two legs of the assumed path
            if hi_first:
                legs = (h[b], l[b])
            else:
                legs = (l[b], h[b])
            hit = False
            for p in legs:
                if direction == 1:
                    if p >= tp and p == h[b]: exit_px = tp; typ = 1; hit = True
                    elif p <= sl and p == l[b]: exit_px = sl; typ = -1; hit = True
                else:
                    if p <= tp and p == l[b]: exit_px = tp; typ = 1; hit = True
                    elif p >= sl and p == h[b]: exit_px = sl; typ = -1; hit = True
                if hit:
                    break
            fav = (h[b] - entry) if direction == 1 else (entry - l[b])
            adv = (entry - l[b]) if direction == 1 else (h[b] - entry)
            if fav > best: best = fav
            if adv > worst: worst = adv
            if hit:
                xbar = b
                break
        if xbar == -1:      # time exit at open of f+max_bars
            xbar = f + max_bars
            if xbar >= n:
                break
            exit_px = o[xbar]; typ = 0
        gross_ticks = direction * (exit_px - entry) / mintick
        net = gross_ticks - cost_ticks_rt - (stop_slip_ticks if typ == -1 else 0.0)
        eb[k] = f; xb[k] = xbar; pnl[k] = net; rr[k] = net * mintick / atrv[t]; et[k] = typ
        mfe[k] = best / atrv[t]; mae[k] = worst / atrv[t]
        k += 1
        # next signal can only be evaluated at a bar where we are flat at its close
        t = xbar if typ != 0 else xbar  # time exit fills at open of xbar -> flat at close of xbar
    return eb[:k], xb[:k], pnl[:k], rr[:k], et[:k], mfe[:k], mae[:k]

def stats(rr, et=None):
    n = len(rr)
    if n < 2:
        return dict(n=n, win=np.nan, avgR=np.nan, tR=np.nan, pf=np.nan, srT=np.nan)
    w = (rr > 0).mean()
    gp = rr[rr > 0].sum(); gl = -rr[rr < 0].sum()
    sd = rr.std(ddof=1)
    out = dict(n=n, win=w, avgR=rr.mean(), tR=rr.mean() / sd * np.sqrt(n) if sd > 0 else np.nan,
               pf=gp / gl if gl > 0 else np.inf, srT=rr.mean() / sd if sd > 0 else np.nan)
    if et is not None:
        out.update(tp=(et == 1).mean(), sl=(et == -1).mean(), time=(et == 0).mean())
    return out
