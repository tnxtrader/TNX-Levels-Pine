"""Data, sessions, indicators, every candidate rule of FRAMEWORK.html Part C, and the candidate registries.

Pool L (legacy, Programs 1-2): 36 triggers x 30 filters (TRIGGERS, FILTERS; trigger(), filt()).
Pool N (Program 3): 25 triggers x 10 T3 contexts x 5 market contexts (N_TRIGGERS, T3_CONTEXTS, MARKET_CONTEXTS;
ntrigger(), t3ctx(), mctx()); signal3() executes any row of registry/program3_candidates.csv.

Every rule is causal (reads bars <= t only; x[k] = k bars back) and is written for the long side.
The short side is the same rule on mirrored prices (o, h, l, c) -> (-o, -l, -h, -c) and T -> -T.
atr1 = ATR(14) of the previous bar sizes every threshold, so the signal bar never sizes itself.
"""
from __future__ import annotations
import hashlib, json
import numpy as np, pandas as pd
from numba import njit

DEF_VERSION = "t3lab-rules-2"

# ----------------------------------------------------------------------------- data and sessions
def load_symbol(path: str) -> pd.DataFrame:
    return load_symbol_df(pd.read_csv(path))

def load_symbol_df(df: pd.DataFrame) -> pd.DataFrame:
    df["ts"] = pd.to_datetime(df["timestamp"])
    df = df.drop(columns=["timestamp"]).sort_values("ts", kind="stable").reset_index(drop=True)
    for c in ("open", "high", "low", "close", "volume"):
        df[c] = df[c].astype(float)
    return df

def add_sessions(df: pd.DataFrame, session_start_hour: int, gap_minutes: int, buckets: dict) -> pd.DataFrame:
    """CME Globex day: the session dated D starts at session_start_hour (17:00 CT) of day D-1.
    Any time gap >= gap_minutes also starts a new session (halts, holidays)."""
    ts = df["ts"]
    sess_date = (ts + pd.Timedelta(hours=24 - session_start_hour)).dt.normalize()
    gap = ts.diff().dt.total_seconds().div(60).fillna(1e9)
    new = (sess_date != sess_date.shift()) | (gap >= gap_minutes)
    df["sess"] = new.cumsum().astype(np.int64) - 1
    df["sess_date"] = sess_date
    roll = (df["contract"] != df["contract"].shift()).to_numpy().copy()
    roll[0] = False
    df["roll"] = roll
    hr = ts.dt.hour + ts.dt.minute / 60.0
    df["bucket"] = "OTHER"
    for name, (a, b) in buckets.items():
        m = (hr >= a) & (hr < b) if a < b else (hr >= a) | (hr < b)
        df.loc[m, "bucket"] = name
    return df

def tradable_mask(df: pd.DataFrame, start_hour: int, bar_minutes: int, flat_by: str | None) -> np.ndarray:
    """Bars that may hold a position: with a flat-by time (e.g. Topstep 15:08 CT), a bar that closes after it and opens
    before the next session start (17:00 CT) is not tradable."""
    if not flat_by:
        return np.ones(len(df), bool)
    hh, mm = (int(x) for x in flat_by.split(":"))
    tod = (df["ts"].dt.hour * 60 + df["ts"].dt.minute).to_numpy()
    return ~((tod < start_hour * 60) & (tod + bar_minutes > hh * 60 + mm))

def clock_masks(df: pd.DataFrame, start_hour: int, bar_minutes: int, flat_by: str | None):
    """(tradable, next_tradable): whether each bar may hold a position, and whether the NEXT SCHEDULED bar (bar time +
    bar_minutes) could. Both come from the clock alone, so the last tradable bar is known at its close without reading
    the next bar (the same rule as T3_Edge_Lab.pine: lastTradable / afterFlat)."""
    nxt = df[["ts"]].copy()
    nxt["ts"] = nxt["ts"] + pd.Timedelta(minutes=bar_minutes)
    return tradable_mask(df, start_hour, bar_minutes, flat_by), tradable_mask(nxt, start_hour, bar_minutes, flat_by)

def last_index_arrays(df: pd.DataFrame, tradable: np.ndarray | None = None, next_tradable: np.ndarray | None = None):
    """Per bar i: the session's end marker at or after i, and the last bar before the next contract roll.
    An end marker is a bar that may not hold a position, a bar whose next scheduled bar may not, or the session's last bar
    (sessions end on the exchange calendar). A position closes at the marker's close; no entry is made from a marker.
    Everything is known at the close of the bar, as in T3_Edge_Lab.pine (session.islastbar, lastTradable, afterFlat)."""
    n = len(df)
    s = df["sess"].to_numpy()
    tr = np.ones(n, bool) if tradable is None else np.asarray(tradable, bool)
    trn = np.ones(n, bool) if next_tradable is None else np.asarray(next_tradable, bool)
    marker = ~tr | ~trn | np.r_[s[1:] != s[:-1], True]
    sess_last = np.empty(n, np.int64)
    nxt = n - 1
    for i in range(n - 1, -1, -1):
        if marker[i]:
            nxt = i
        sess_last[i] = nxt
    roll = df["roll"].to_numpy()
    roll_last = np.empty(n, np.int64)
    nxt = n - 1
    for i in range(n - 1, -1, -1):
        roll_last[i] = nxt
        if roll[i] and i > 0:
            nxt = i - 1
    return sess_last, roll_last

# ----------------------------------------------------------------------------- indicators
@njit(cache=True)
def _ema(x, n):
    a = 2.0 / (n + 1.0)
    out = np.empty_like(x)
    out[0] = x[0]
    for i in range(1, len(x)):
        out[i] = a * x[i] + (1 - a) * out[i - 1]
    return out

def t3(close: np.ndarray, length: int, vf: float) -> np.ndarray:
    """Tillson T3: GD(x) = (1+v)EMA(x) - v EMA(EMA(x)); T3 = GD(GD(GD(close)))."""
    e1 = _ema(close, length); e2 = _ema(e1, length); e3 = _ema(e2, length)
    e4 = _ema(e3, length); e5 = _ema(e4, length); e6 = _ema(e5, length)
    v = vf
    c1 = -v ** 3; c2 = 3 * v * v + 3 * v ** 3; c3 = -6 * v * v - 3 * v - 3 * v ** 3; c4 = 1 + 3 * v + v ** 3 + 3 * v * v
    return c1 * e6 + c2 * e5 + c3 * e4 + c4 * e3

def atr_wilder(h, l, c, n=14):
    """Wilder ATR: RMA of true range, seeded with the mean of the first n true ranges (same as Pine ta.atr)."""
    pc = np.r_[np.nan, c[:-1]]
    tr = np.where(np.isnan(pc), h - l, np.maximum(h - l, np.maximum(np.abs(h - pc), np.abs(l - pc))))
    out = np.full(len(c), np.nan)
    if len(c) >= n:
        out[n - 1] = tr[:n].mean()
        for i in range(n, len(c)):
            out[i] = (out[i - 1] * (n - 1) + tr[i]) / n
    return out

def lag(x, k):
    out = np.full(len(x), np.nan)
    if k == 0:
        return np.asarray(x, float).copy()
    out[k:] = x[:-k]
    return out

def roll_max(x, n):
    return pd.Series(x).rolling(n, min_periods=n).max().to_numpy()

def roll_min(x, n):
    return pd.Series(x).rolling(n, min_periods=n).min().to_numpy()

def roll_mean(x, n):
    return pd.Series(x).rolling(n, min_periods=n).mean().to_numpy()

def roll_median(x, n):
    return pd.Series(x).rolling(n, min_periods=n).median().to_numpy()

def hhmm(x: str) -> int:
    h, m = (int(v) for v in str(x).split(":"))
    return h * 60 + m

class Bars:
    """Arrays of one instrument. T3 lines are cached per (length, vf); non-T3 rule arrays are cached by name.
    Session clock (CT minutes): rth_open = day-session open, rth_close = daily settlement time of the instrument."""
    def __init__(self, df: pd.DataFrame, atr_len: int = 14, rth_open: str = "08:30", rth_close: str = "15:00",
                 bar_minutes: int = 30):
        self.df = df
        self.o, self.h, self.l, self.c = (df[k].to_numpy(float) for k in ("open", "high", "low", "close"))
        self.v = df["volume"].to_numpy(float) if "volume" in df else np.ones(len(df))
        self.n = len(df)
        self.atr = atr_wilder(self.h, self.l, self.c, atr_len)
        self.atr1 = lag(self.atr, 1)
        self._t3, self._cache = {}, {}
        self.tod = (df["ts"].dt.hour * 60 + df["ts"].dt.minute).to_numpy()
        self.sess = df["sess"].to_numpy() if "sess" in df else np.zeros(self.n, np.int64)
        self.bar = bar_minutes
        self.or_start = int(np.ceil(hhmm(rth_open) / bar_minutes) * bar_minutes)          # first full bar of the day session
        self.settle_start = int(np.floor((hhmm(rth_close) - 1) / bar_minutes) * bar_minutes)  # bar that ends at settlement
        self.imom_start = self.settle_start - bar_minutes                                 # bar before the last half-hour
        self.or2_start = self.or_start + bar_minutes                                      # the bar after the opening bar
        self.nsess = int(self.sess.max()) + 1 if self.n else 0
        self._anch, self.pair_d = {}, None

    def by_session(self, x, at):
        """x at the bar of each session where `at` is true, carried forward inside that session (NaN before it)."""
        v = pd.Series(np.where(at, x, np.nan))
        return v.groupby(self.sess).ffill().to_numpy()

    def prev_settle(self, c):
        """Close of the previous session's settlement bar (NaN if that session has none)."""
        st = pd.Series(np.where(self.tod == self.settle_start, c, np.nan)).groupby(self.sess).max()
        prev = st.reindex(np.arange(self.sess.max() + 1)).shift(1)
        return prev.to_numpy()[self.sess]

    def t3(self, length, vf):
        k = (int(length), round(float(vf), 3))
        if k not in self._t3:
            self._t3[k] = t3(self.c, *k)
        return self._t3[k]

    def first(self, cond):
        """first(cond): cond true for the first time in the current session."""
        cond = np.asarray(cond, bool)
        return cond & (pd.Series(cond.astype(np.int64)).groupby(self.sess).cumsum().to_numpy() == 1)

    def set_pair(self, pb: "Bars"):
        """MARKET_AGREE input: the paired market's close minus its own previous settlement close, aligned by timestamp
        (NaN where the paired market has no bar at that time)."""
        d = pd.Series(pb.c - pb.prev_settle(pb.c), index=pb.df["ts"].to_numpy())
        d = d[~d.index.duplicated(keep="last")]
        self.pair_d = d.reindex(self.df["ts"].to_numpy()).to_numpy(float)
        self._cache = {k: v for k, v in self._cache.items() if not (k[0] == "M" and k[1] == "MARKET_AGREE")}

    def anchors(self, side: int) -> dict:
        """Session anchors of FRAMEWORK.html B.8 on the side's (mirrored) prices, per bar. Causal: every value exists only
        from the bar at which it is known; session statistics use sessions strictly before the current one."""
        if side in self._anch:
            return self._anch[side]
        o, h, l, c = _mirror(self, side)
        tod, sess, S = self.tod, self.sess, self.nsess
        isOB, isOB2, isSB = tod == self.or_start, tod == self.or2_start, tod == self.settle_start
        inday = (tod >= self.or_start) & (tod <= self.settle_start)
        hasOB = np.zeros(S, bool); hasOB[sess[isOB]] = True
        hasSB = np.zeros(S, bool); hasSB[sess[isSB]] = True
        grp = lambda x, f: getattr(pd.Series(x).groupby(sess), f)().reindex(np.arange(S)).to_numpy(float)
        valid = hasOB & hasSB
        Hs = np.where(valid, grp(np.where(inday, h, np.nan), "max"), np.nan)
        Ls = np.where(valid, grp(np.where(inday, l, np.nan), "min"), np.nan)
        Rs = Hs - Ls
        St = grp(np.where(isSB, c, np.nan), "max")
        prev = lambda x: np.r_[np.nan, x[:-1]]
        R_prev, H_prev, L_prev, S_prev_s = prev(Rs), prev(Hs), prev(Ls), prev(St)
        med250 = _prior(Rs, 250, np.median)
        x = R_prev / med250
        vol_cnt = _prior_rank(x, 250)
        first21 = _prior(St, 21, lambda w: w[0])
        A = dict(
            O_day=self.by_session(o, isOB), hOB=self.by_session(h, isOB), cOB=self.by_session(c, isOB),
            hOB2=self.by_session(h, isOB2), S_prev=S_prev_s[sess], H_prev=H_prev[sess], L_prev=L_prev[sess],
            R_prev=R_prev[sess], meanR20=_prior(Rs, 20, np.mean)[sess],
            nr4=(R_prev <= _prior(Rs, 4, np.min))[sess], nr7=(R_prev <= _prior(Rs, 7, np.min))[sess],
            vol_cnt=vol_cnt[sess], dtrend=((S_prev_s - first21) > 0)[sess])
        after = ~np.isnan(self.by_session(np.ones(self.n), isOB))
        tp = (h + l + c) / 3.0
        cpv = pd.Series(np.where(after, tp * self.v, 0.0)).groupby(sess).cumsum().to_numpy()
        cv = pd.Series(np.where(after, self.v, 0.0)).groupby(sess).cumsum().to_numpy()
        with np.errstate(invalid="ignore", divide="ignore"):
            A["vwap"] = np.where(after & (cv > 0), cpv / np.where(cv > 0, cv, 1.0), np.nan)
        dev = np.abs(c - A["O_day"])
        A["sigma"] = self._slot_prior(dev, 14, tod, "mean")
        ret = c - o
        nxt = tod + self.bar
        for N in (20, 40):
            mu = self._slot_prior(ret, N, nxt, "mean")
            sd = self._slot_prior(ret, N, nxt, "std")
            with np.errstate(invalid="ignore", divide="ignore"):
                A[f"z{N}"] = np.where(sd > 0, mu / np.where(sd > 0, sd, 1.0) * np.sqrt(N), np.nan)
        self._anch[side] = A
        return A

    def _slot_prior(self, vals, n, query_tod, how):
        """Per bar t: rolling `how` (mean | std with ddof 1) over the last n non-NaN values of time slot query_tod[t]
        from sessions strictly before the session of bar t; NaN with fewer than n."""
        out = np.full(self.n, np.nan)
        ok = ~np.isnan(vals)
        for tau in np.unique(self.tod[ok]):
            m = ok & (self.tod == tau)
            q = np.nonzero(query_tod == tau)[0]
            if not len(q):
                continue
            ss = self.sess[m]
            r = pd.Series(vals[m]).rolling(n, min_periods=n)
            st = (r.mean() if how == "mean" else r.std(ddof=1)).to_numpy()
            pos = np.searchsorted(ss, self.sess[q], side="left") - 1
            good = pos >= 0
            out[q[good]] = st[pos[good]]
        return out

def _prior(vals, n, fn):
    """Per session s: fn over the last n non-NaN values of sessions strictly before s; NaN with fewer than n."""
    vals = np.asarray(vals, float)
    out = np.full(len(vals), np.nan)
    idx = np.nonzero(~np.isnan(vals))[0]
    v = vals[idx]
    st = np.full(len(idx), np.nan)
    for k in range(n - 1, len(idx)):
        st[k] = fn(v[k - n + 1:k + 1])
    pos = np.searchsorted(idx, np.arange(len(vals)), side="left") - 1
    good = pos >= 0
    out[good] = st[pos[good]]
    return out

def _prior_rank(x, n):
    """Per session s with a value: how many of the last n non-NaN values of sessions before s are strictly below x[s]."""
    out = np.full(len(x), np.nan)
    idx = np.nonzero(~np.isnan(x))[0]
    for j, s in enumerate(idx):
        if j >= n:
            out[s] = float((x[idx[j - n:j]] < x[s]).sum())
    return out

def _mirror(b: Bars, side: int):
    if side == 1:
        return b.o, b.h, b.l, b.c
    return -b.o, -b.l, -b.h, -b.c

# ----------------------------------------------------------------------------- rule definitions (long form)
TRIGGERS = {
    "T3_BODY_CROSS":          "o <= T and c > T",
    "T3_CLOSE_CROSS":         "c[1] <= T[1] and c > T",
    "T3_TWO_CLOSE":           "c > T and c[1] > T[1] and c[2] <= T[2]",
    "T3_THREE_CLOSE":         "c > T and c[1] > T[1] and c[2] > T[2] and c[3] <= T[3]",
    "T3_WICK_RECLAIM":        "o > T and l <= T and c > T",
    "T3_CLEAN_RECLAIM":       "l > T and l[1] <= T[1] <= h[1]",
    "T3_CROSS_RANGE_BREAK":   "c[1] > T[1] and c[2] <= T[2] and c > h[1]",
    "T3_PULLBACK_BREAK":      "T > T[1] and l[1] <= T[1] < c[1] and c > h[1]",
    "T3_TOUCH_3BAR_TREND":    "T > T[1] and (l[1] <= T[1] or l[2] <= T[2] or l[3] <= T[3]) and c > T",
    "T3_SLOPE_TURN":          "T > T[1] and T[1] <= T[2] and c > T",
    "T3_DISTANCE_RELEASE":    "d > 0 and d > d[1]",
    "DONCHIAN_3":             "c > HH(3)",
    "DONCHIAN_5":             "c > HH(5)",
    "DONCHIAN_8":             "c > HH(8)",
    "DONCHIAN_12":            "c > HH(12)",
    "DONCHIAN_20":            "c > HH(20)",
    "PREVBAR_BREAK":          "c > h[1]",
    "INSIDE_BAR_BREAK":       "h[1] <= h[2] and l[1] >= l[2] and c > h[1]",
    "COMPRESSION_BREAK_3":    "rng[1] == min(rng[1..3]) and c > HH(3)",
    "COMPRESSION_BREAK_5":    "rng[1] == min(rng[1..5]) and c > HH(5)",
    "COMPRESSION_BREAK_8":    "rng[1] == min(rng[1..8]) and c > HH(8)",
    "RANGE_EXPANSION_CLOSE":  "rng > atr1 and CLV >= 0.75",
    "MOMENTUM_3":             "c > c[3]",
    "MOMENTUM_6":             "c > c[6]",
    "THREE_BAR_CONTINUATION": "c[1] > c[2] and c[2] > c[3] and c > h[1]",
    "GAP_CONTINUATION":       "o > c[1] and c > o",
    "TWO_BAR_REVERSAL":       "c[1] < o[1] and c > h[1]",
    "OUTSIDE_BAR_REVERSAL":   "h > h[1] and l < l[1] and rng > 0 and CLV >= 0.75",
    "SWEEP_RECLAIM_5":        "lvl = min(l[2..6]); l[1] < lvl and c > lvl",
    "SWEEP_RECLAIM_10":       "lvl = min(l[2..11]); l[1] < lvl and c > lvl",
    "MEAN_REVERSION_2":       "l <= SMA20(c)[1] - 2*atr1 and c > o",
    "MEAN_REVERSION_3":       "l <= SMA20(c)[1] - 3*atr1 and c > o",
    "ORB_OPEN_BAR":           "bar = opening bar of the day session and c > o",
    "ORB_BREAK":              "opening bar < bar < settlement bar, c > high of the opening bar, first such close of the session",
    "IMOM_DAY":               "bar = the bar before the last half-hour and c > previous settlement close",
    "IMOM_FIRST":             "bar = the bar before the last half-hour and close of the opening bar > previous settlement close",
}
FILTERS = {
    "NONE":                 "true",
    "BODY_35":              "rng > 0 and |c - o| >= 0.35*rng",
    "BODY_55":              "rng > 0 and |c - o| >= 0.55*rng",
    "BODY_70":              "rng > 0 and |c - o| >= 0.70*rng",
    "CLV_55":               "rng > 0 and CLV >= 0.55",
    "CLV_65":               "rng > 0 and CLV >= 0.65",
    "CLV_75":               "rng > 0 and CLV >= 0.75",
    "DIRECTIONAL_BODY":     "c > o",
    "FAVOURABLE_WICK_30":   "rng > 0 and min(o, c) - l >= 0.30*rng",
    "NO_ADVERSE_WICK_20":   "rng > 0 and h - max(o, c) <= 0.20*rng",
    "GAP_ALIGNED_010":      "o - c[1] >= 0.10*atr1",
    "MOMENTUM_2":           "c > c[2]",
    "PREV_OPPOSITE_CANDLE": "c[1] < o[1]",
    "RANGE_COMPRESS_060":   "rng <= 0.60*median(rng[1..20])",
    "RANGE_EXPAND_125":     "rng >= 1.25*atr1",
    "RANGE_EXPAND_150":     "rng >= 1.50*atr1",
    "STRUCTURE_STEP":       "l > l[1]",
    "T3_DIST_FAR_025":      "d >= 0.25*atr1",
    "T3_DIST_FAR_050":      "d >= 0.50*atr1",
    "T3_DIST_NEAR_025":     "d > 0 and d <= 0.25*atr1",
    "T3_DIST_NEAR_050":     "d > 0 and d <= 0.50*atr1",
    "T3_DIST_NEAR_100":     "d > 0 and d <= 1.00*atr1",
    "T3_NOT_EXTENDED_075":  "d <= 0.75*atr1",
    "T3_SLOPE_NONNEG":      "T - T[3] >= 0",
    "T3_SLOPE_STRONG_015":  "T - T[3] >= 0.15*atr1",
    "T3_SLOPE_STRONG_030":  "T - T[3] >= 0.30*atr1",
    "VOL_HIGH_120":         "atr1 >= 1.20*median(atr1[0..49])",
    "VOL_LOW_080":          "atr1 <= 0.80*median(atr1[0..49])",
    "ABOVE_PREV_SETTLE":    "c > previous settlement close",
    "RTH_OPEN_90":          "bar starts within 90 minutes after the opening bar",
}
TRIGGER_NAMES = list(TRIGGERS)
FILTER_NAMES = list(FILTERS)

def uses_t3(trigger_name: str, filter_name: str) -> bool:
    return trigger_name.startswith("T3_") or filter_name.startswith("T3_")

def _done(b, key, r, cache):
    r = np.nan_to_num(np.asarray(r, float), nan=0.0).astype(bool)
    if cache:
        b._cache[key] = r
    return r

def trigger(b: Bars, name: str, side: int, L=None, V=None) -> np.ndarray:
    is_t3 = name.startswith("T3_")
    key = ("T", name, side)
    if not is_t3 and key in b._cache:
        return b._cache[key]
    o, h, l, c = _mirror(b, side)
    S = lag; a1 = b.atr1; rng = h - l
    with np.errstate(invalid="ignore", divide="ignore"):
        if is_t3:
            T = b.t3(L, V) * side
        if name == "T3_BODY_CROSS":          r = (o <= T) & (c > T)
        elif name == "T3_CLOSE_CROSS":       r = (S(c, 1) <= S(T, 1)) & (c > T)
        elif name == "T3_TWO_CLOSE":         r = (c > T) & (S(c, 1) > S(T, 1)) & (S(c, 2) <= S(T, 2))
        elif name == "T3_THREE_CLOSE":       r = (c > T) & (S(c, 1) > S(T, 1)) & (S(c, 2) > S(T, 2)) & (S(c, 3) <= S(T, 3))
        elif name == "T3_WICK_RECLAIM":      r = (o > T) & (l <= T) & (c > T)
        elif name == "T3_CLEAN_RECLAIM":     r = (l > T) & (S(l, 1) <= S(T, 1)) & (S(T, 1) <= S(h, 1))
        elif name == "T3_CROSS_RANGE_BREAK": r = (S(c, 1) > S(T, 1)) & (S(c, 2) <= S(T, 2)) & (c > S(h, 1))
        elif name == "T3_PULLBACK_BREAK":    r = (T > S(T, 1)) & (S(l, 1) <= S(T, 1)) & (S(T, 1) < S(c, 1)) & (c > S(h, 1))
        elif name == "T3_TOUCH_3BAR_TREND":
            touch = (S(l, 1) <= S(T, 1)) | (S(l, 2) <= S(T, 2)) | (S(l, 3) <= S(T, 3))
            r = (T > S(T, 1)) & touch & (c > T)
        elif name == "T3_SLOPE_TURN":        r = (T > S(T, 1)) & (S(T, 1) <= S(T, 2)) & (c > T)
        elif name == "T3_DISTANCE_RELEASE":
            d = c - T; r = (d > 0) & (d > S(d, 1))
        elif name.startswith("DONCHIAN_"):
            N = int(name.split("_")[1]); r = c > S(roll_max(h, N), 1)
        elif name == "PREVBAR_BREAK":        r = c > S(h, 1)
        elif name == "INSIDE_BAR_BREAK":     r = (S(h, 1) <= S(h, 2)) & (S(l, 1) >= S(l, 2)) & (c > S(h, 1))
        elif name.startswith("COMPRESSION_BREAK_"):
            N = int(name.rsplit("_", 1)[1])
            r = (S(rng, 1) <= S(roll_min(rng, N), 1)) & (c > S(roll_max(h, N), 1))
        elif name == "RANGE_EXPANSION_CLOSE": r = (rng > a1) & ((c - l) >= 0.75 * rng)
        elif name.startswith("MOMENTUM_"):
            N = int(name.split("_")[1]); r = c > S(c, N)
        elif name == "THREE_BAR_CONTINUATION": r = (S(c, 1) > S(c, 2)) & (S(c, 2) > S(c, 3)) & (c > S(h, 1))
        elif name == "GAP_CONTINUATION":     r = (o > S(c, 1)) & (c > o)
        elif name == "TWO_BAR_REVERSAL":     r = (S(c, 1) < S(o, 1)) & (c > S(h, 1))
        elif name == "OUTSIDE_BAR_REVERSAL": r = (h > S(h, 1)) & (l < S(l, 1)) & (rng > 0) & ((c - l) >= 0.75 * rng)
        elif name.startswith("SWEEP_RECLAIM_"):
            N = int(name.rsplit("_", 1)[1]); lvl = S(roll_min(l, N), 2)
            r = (S(l, 1) < lvl) & (c > lvl)
        elif name.startswith("MEAN_REVERSION_"):
            k = float(name.rsplit("_", 1)[1]); mu = S(roll_mean(c, 20), 1)
            r = (l <= mu - k * a1) & (c > o)
        elif name == "ORB_OPEN_BAR":
            r = (b.tod == b.or_start) & (c > o)
        elif name == "ORB_BREAK":
            orh = b.by_session(h, b.tod == b.or_start)
            raw = (b.tod > b.or_start) & (b.tod < b.settle_start) & (c > orh)
            r = raw & (pd.Series(raw.astype(int)).groupby(b.sess).cumsum().to_numpy() == 1)
        elif name == "IMOM_DAY":
            r = (b.tod == b.imom_start) & (c > b.prev_settle(c))
        elif name == "IMOM_FIRST":
            orc = b.by_session(c, b.tod == b.or_start)
            r = (b.tod == b.imom_start) & (orc > b.prev_settle(c))
        else:
            raise KeyError(name)
    return _done(b, key, r, not is_t3)

def filt(b: Bars, name: str, side: int, L=None, V=None) -> np.ndarray:
    is_t3 = name.startswith("T3_")
    key = ("F", name, side)
    if not is_t3 and key in b._cache:
        return b._cache[key]
    o, h, l, c = _mirror(b, side)
    S = lag; a1 = b.atr1; rng = h - l
    with np.errstate(invalid="ignore", divide="ignore"):
        if is_t3:
            T = b.t3(L, V) * side; d = c - T
        if name == "NONE":                    r = np.ones(b.n, bool)
        elif name.startswith("BODY_"):        r = (rng > 0) & (np.abs(c - o) >= int(name[5:]) / 100 * rng)
        elif name.startswith("CLV_"):         r = (rng > 0) & ((c - l) >= int(name[4:]) / 100 * rng)
        elif name == "DIRECTIONAL_BODY":      r = c > o
        elif name == "FAVOURABLE_WICK_30":    r = (rng > 0) & ((np.minimum(o, c) - l) >= 0.30 * rng)
        elif name == "NO_ADVERSE_WICK_20":    r = (rng > 0) & ((h - np.maximum(o, c)) <= 0.20 * rng)
        elif name == "GAP_ALIGNED_010":       r = (o - S(c, 1)) >= 0.10 * a1
        elif name == "MOMENTUM_2":            r = c > S(c, 2)
        elif name == "PREV_OPPOSITE_CANDLE":  r = S(c, 1) < S(o, 1)
        elif name == "RANGE_COMPRESS_060":    r = rng <= 0.60 * S(roll_median(rng, 20), 1)
        elif name == "RANGE_EXPAND_125":      r = rng >= 1.25 * a1
        elif name == "RANGE_EXPAND_150":      r = rng >= 1.50 * a1
        elif name == "STRUCTURE_STEP":        r = l > S(l, 1)
        elif name.startswith("T3_DIST_FAR_"):  r = d >= int(name[-3:]) / 100 * a1
        elif name.startswith("T3_DIST_NEAR_"): r = (d > 0) & (d <= int(name[-3:]) / 100 * a1)
        elif name == "T3_NOT_EXTENDED_075":   r = d <= 0.75 * a1
        elif name == "T3_SLOPE_NONNEG":       r = (T - S(T, 3)) >= 0
        elif name.startswith("T3_SLOPE_STRONG_"): r = (T - S(T, 3)) >= int(name[-3:]) / 100 * a1
        elif name == "VOL_HIGH_120":          r = a1 >= 1.20 * roll_median(a1, 50)
        elif name == "VOL_LOW_080":           r = a1 <= 0.80 * roll_median(a1, 50)
        elif name == "ABOVE_PREV_SETTLE":     r = c > b.prev_settle(c)
        elif name == "RTH_OPEN_90":           r = (b.tod >= b.or_start) & (b.tod < b.or_start + 90)
        else:
            raise KeyError(name)
    return _done(b, key, r, not is_t3)

def candidate_signal(b: Bars, row) -> np.ndarray:
    side = 1 if row["side"] == "long" else -1
    if uses_t3(row["trigger"], row["filter"]):
        L, V = int(row["t3_length"]), float(row["t3_vf"])
    else:
        L = V = None
    return trigger(b, row["trigger"], side, L, V) & filt(b, row["filter"], side, L, V)

# ============================================================================= Program 3, Pool N (FRAMEWORK.html C.6-C.10)
FAST = (5, 0.7)        # Tillson's published setting
SLOW = (13, 0.7)
DAY_BARS = 48          # a DAY hold: more bars than a session has, so the trade ends at the last tradable bar (flat-by)
VOL_HIGH_MIN = 167     # x above at least 2/3 of the previous 250 sessions' x (upper tercile)
VOL_NOT_LOW_MIN = 84   # x above at least 1/3 of them (not the lower tercile)

N_TRIGGERS = {
    "ORB_DIR_30":          "bar = OB and c > o",
    "ORB_BREAK_30":        "first(OB < bar < SB and c > h(OB))",
    "ORB_BREAK_60":        "first(OB2 < bar < SB and c > max(h(OB), h(OB2)))",
    "ORB_RETEST_30":       "first(OB < bar < SB, an ORB_BREAK_30 on an earlier bar of this session, l <= h(OB) and c > h(OB))",
    "IMOM_DAY":            "bar = IB and c > S_prev",
    "IMOM_FIRST":          "bar = IB and c(OB) > S_prev",
    "IMOM_LARGE":          "bar = IB and c - S_prev >= 0.5 * mean(R of the last 20 valid sessions)",
    "NB_10":               "first(OB < bar < SB and c > max(O_day, S_prev) + 1.0*sigma_tau)",
    "NB_15":               "first(OB < bar < SB and c > max(O_day, S_prev) + 1.5*sigma_tau)",
    "VBO_025":             "first(DAY and c > O_day + 0.25*R_prev)",
    "VBO_050":             "first(DAY and c > O_day + 0.50*R_prev)",
    "GAP_CONTINUE":        "bar = OB, O_day - S_prev >= 0.25*R_prev and c > o",
    "GAP_FADE":            "bar = OB, O_day - S_prev <= -0.25*R_prev and c > o",
    "VWAP_RECLAIM":        "OB < bar < SB, c[1] <= VWAP[1] and c > VWAP",
    "VWAP_PULLBACK":       "OB < bar < SB, c[1] > VWAP[1], l <= VWAP, c > VWAP and c > o",
    "SLOT_20":             "OB <= time of bar t+1 < SB and z(c - o of that time slot, previous 20 sessions) >= 2",
    "SLOT_40":             "OB <= time of bar t+1 < SB and z(c - o of that time slot, previous 40 sessions) >= 2",
    "NR4_DAY_BREAK":       "R_prev <= min(R of the last 4 valid sessions) and first(DAY and c > H_prev)",
    "NR7_DAY_BREAK":       "R_prev <= min(R of the last 7 valid sessions) and first(DAY and c > H_prev)",
    "PDH_BREAK":           "first(DAY and c > H_prev)",
    "PDL_RECLAIM":         "first(DAY and l[1] < L_prev and c > L_prev)",
    "T3_PULLBACK_BREAK_D": "DAY and T > T[1] and l[1] <= T[1] < c[1] and c > h[1]",
    "T3_TOUCH_3BAR_D":     "DAY and T > T[1] and (l[1] <= T[1] or l[2] <= T[2] or l[3] <= T[3]) and c > T",
    "MR_VWAP_2":           "OB < bar < SB, l <= VWAP - 2*atr1, c > o and CLV >= 0.5",
    "MR_SMA20_2_D":        "DAY and l <= SMA20(c)[1] - 2*atr1 and c > o",
}
T3_CONTEXTS = {
    "NONE":          "true",
    "T3_SLOPE_UP_F": "T - T[3] >= 0 (FAST)",
    "T3_SLOPE_UP_S": "T - T[3] >= 0 (SLOW)",
    "T3_SIDE_F":     "c > T (FAST)",
    "T3_SIDE_S":     "c > T (SLOW)",
    "T3_NEAR_F":     "0 < c - T <= 1.0*atr1 (FAST)",
    "T3_NEAR_S":     "0 < c - T <= 1.0*atr1 (SLOW)",
    "T3_STRONG_F":   "T - T[3] >= 0.15*atr1 (FAST)",
    "T3_STRONG_S":   "T - T[3] >= 0.15*atr1 (SLOW)",
    "T3_STACK":      "T_FAST > T_SLOW and T_SLOW - T_SLOW[3] >= 0",
}
MARKET_CONTEXTS = {
    "NONE":         "true",
    "VOL_HIGH":     "x = R_prev / median(R of the last 250 valid sessions) is above >= 167 of the previous 250 sessions' x",
    "VOL_NOT_LOW":  "x is above >= 84 of the previous 250 sessions' x",
    "DAILY_TREND":  "S_prev - (the valid settlement 20 before S_prev) > 0",
    "MARKET_AGREE": "paired market: its close - its own S_prev > 0 at the same bar time (NQ-YM, GC-SI, CL-NQ)",
}
N_TRIGGER_NAMES, T3_CONTEXT_NAMES, MARKET_CONTEXT_NAMES = list(N_TRIGGERS), list(T3_CONTEXTS), list(MARKET_CONTEXTS)
PAIRS = {"NQ": "YM", "YM": "NQ", "GC": "SI", "SI": "GC", "CL": "NQ"}

def n_speed(t3_context: str):
    """T3 speed of a Pool N row: SLOW for *_S contexts, FAST otherwise (T3 triggers with NONE / T3_STACK use FAST)."""
    return SLOW if t3_context.endswith("_S") else FAST

def ntrigger(b: Bars, name: str, side: int, L=None, V=None) -> np.ndarray:
    """Pool N trigger, long form on the side's mirrored prices. L, V: T3 speed (T3 triggers only)."""
    key = ("N", name, side) if not name.startswith("T3_") else ("N", name, side, L, V)
    if key in b._cache:
        return b._cache[key]
    if name in ("IMOM_DAY", "IMOM_FIRST"):
        return _done(b, key, trigger(b, name, side), True)
    if name == "ORB_DIR_30":
        return _done(b, key, trigger(b, "ORB_OPEN_BAR", side), True)
    if name == "ORB_BREAK_30":
        return _done(b, key, trigger(b, "ORB_BREAK", side), True)
    A = b.anchors(side)
    o, h, l, c = _mirror(b, side)
    S = lag; a1 = b.atr1; rng = h - l; tod = b.tod
    OB, OB2, SB, IB = b.or_start, b.or2_start, b.settle_start, b.imom_start
    day = (tod >= OB) & (tod < SB)
    mid = (tod > OB) & (tod < SB)
    vw = A["vwap"]
    with np.errstate(invalid="ignore", divide="ignore"):
        if name == "ORB_BREAK_60":
            r = b.first((tod > OB2) & (tod < SB) & (c > np.maximum(A["hOB"], A["hOB2"])))
        elif name == "ORB_RETEST_30":
            brk = trigger(b, "ORB_BREAK", side).astype(np.int64)
            before = (pd.Series(brk).groupby(b.sess).cumsum().to_numpy() - brk) > 0
            r = b.first(mid & before & (l <= A["hOB"]) & (c > A["hOB"]))
        elif name == "IMOM_LARGE":
            r = (tod == IB) & ((c - A["S_prev"]) >= 0.5 * A["meanR20"])
        elif name in ("NB_10", "NB_15"):
            k = 1.0 if name == "NB_10" else 1.5
            r = b.first(mid & (c > np.maximum(A["O_day"], A["S_prev"]) + k * A["sigma"]))
        elif name in ("VBO_025", "VBO_050"):
            k = 0.25 if name == "VBO_025" else 0.50
            r = b.first(day & (c > A["O_day"] + k * A["R_prev"]))
        elif name == "GAP_CONTINUE":
            r = (tod == OB) & ((A["O_day"] - A["S_prev"]) >= 0.25 * A["R_prev"]) & (c > o)
        elif name == "GAP_FADE":
            r = (tod == OB) & ((A["O_day"] - A["S_prev"]) <= -0.25 * A["R_prev"]) & (c > o)
        elif name == "VWAP_RECLAIM":
            r = mid & (S(c, 1) <= S(vw, 1)) & (c > vw)
        elif name == "VWAP_PULLBACK":
            r = mid & (S(c, 1) > S(vw, 1)) & (l <= vw) & (c > vw) & (c > o)
        elif name in ("SLOT_20", "SLOT_40"):
            nxt = tod + b.bar
            r = (nxt >= OB) & (nxt < SB) & (A["z" + name[-2:]] >= 2)
        elif name in ("NR4_DAY_BREAK", "NR7_DAY_BREAK", "PDH_BREAK"):
            r = b.first(day & (c > A["H_prev"]))
            if name != "PDH_BREAK":
                r = r & A["nr4" if name.startswith("NR4") else "nr7"]
        elif name == "PDL_RECLAIM":
            r = b.first(day & (S(l, 1) < A["L_prev"]) & (c > A["L_prev"]))
        elif name == "T3_PULLBACK_BREAK_D":
            r = day & trigger(b, "T3_PULLBACK_BREAK", side, L, V)
        elif name == "T3_TOUCH_3BAR_D":
            r = day & trigger(b, "T3_TOUCH_3BAR_TREND", side, L, V)
        elif name == "MR_VWAP_2":
            r = mid & (l <= vw - 2 * a1) & (c > o) & (rng > 0) & ((c - l) >= 0.5 * rng)
        elif name == "MR_SMA20_2_D":
            r = day & trigger(b, "MEAN_REVERSION_2", side)
        else:
            raise KeyError(name)
    return _done(b, key, r, True)

def t3ctx(b: Bars, name: str, side: int, fast=FAST, slow=SLOW) -> np.ndarray:
    key = ("C", name, side, fast, slow)
    if key in b._cache:
        return b._cache[key]
    c = b.c * side
    a1 = b.atr1
    with np.errstate(invalid="ignore"):
        if name == "NONE":
            r = np.ones(b.n, bool)
        elif name == "T3_STACK":
            Tf, Ts = b.t3(*fast) * side, b.t3(*slow) * side
            r = (Tf > Ts) & ((Ts - lag(Ts, 3)) >= 0)
        else:
            base, sp = name.rsplit("_", 1)
            T = b.t3(*(fast if sp == "F" else slow)) * side
            d = c - T
            if base == "T3_SLOPE_UP":  r = (T - lag(T, 3)) >= 0
            elif base == "T3_SIDE":    r = c > T
            elif base == "T3_NEAR":    r = (d > 0) & (d <= 1.0 * a1)
            elif base == "T3_STRONG":  r = (T - lag(T, 3)) >= 0.15 * a1
            else:
                raise KeyError(name)
    return _done(b, key, r, True)

def mctx(b: Bars, name: str, side: int) -> np.ndarray:
    key = ("M", name, side)
    if key in b._cache:
        return b._cache[key]
    A = b.anchors(side)
    with np.errstate(invalid="ignore"):
        if name == "NONE":           r = np.ones(b.n, bool)
        elif name == "VOL_HIGH":     r = A["vol_cnt"] >= VOL_HIGH_MIN
        elif name == "VOL_NOT_LOW":  r = A["vol_cnt"] >= VOL_NOT_LOW_MIN
        elif name == "DAILY_TREND":  r = A["dtrend"]
        elif name == "MARKET_AGREE":
            if b.pair_d is None:
                raise ValueError("MARKET_AGREE needs the paired market: call Bars.set_pair(paired_bars) first")
            r = side * b.pair_d > 0
        else:
            raise KeyError(name)
    return _done(b, key, r, True)

def parse_t3(x):
    """'12/0.6' -> (12, 0.6); '' / NaN -> (None, None)."""
    if x is None or (isinstance(x, float) and np.isnan(x)) or str(x).strip() == "":
        return None, None
    a, v = str(x).split("+")[0].strip().split("/")
    return int(a), float(v)

def hold_bars(screen_hold: str) -> list:
    """'8 bars | DAY' -> [8, 48]; 'SHORT 4 bars' -> [4]; 'SHORT 1 bar' -> [1]; 'DAY' -> [48]."""
    out = []
    for part in str(screen_hold).split("|"):
        part = part.strip()
        out.append(DAY_BARS if part == "DAY" else int(part.split()[-2]))
    return out

def signal3(b: Bars, row, fast=FAST, slow=SLOW) -> np.ndarray:
    """Signal of one row of registry/program3_candidates.csv (pool L or N)."""
    side = 1 if row["side"] == "long" else -1
    if row["pool"] == "L":
        L, V = parse_t3(row["t3"])
        return trigger(b, row["trigger"], side, L, V) & filt(b, row["context_or_filter"], side, L, V)
    ctx = row["context_or_filter"]
    L, V = (slow if ctx.endswith("_S") else fast)
    return ntrigger(b, row["trigger"], side, L, V) & t3ctx(b, ctx, side, fast, slow) & mctx(b, row["market_context"], side)

REGISTRY3_COLUMNS = ["candidate_id", "pool", "program", "family", "trigger", "context_or_filter", "market_context", "side",
                     "t3", "screen_hold", "dev_informed"]

def registry3_problems(reg: pd.DataFrame) -> dict:
    """Counts of Program 3 rows that cannot be executed exactly as written."""
    L, N = reg[reg["pool"] == "L"], reg[reg["pool"] == "N"]
    def bad_hold(x):
        try:
            return not hold_bars(x)
        except Exception:
            return True
    def expect_t3(r):
        ctx, trg = r["context_or_filter"], r["trigger"]
        if ctx == "T3_STACK":
            return "5/0.7 + 13/0.7"
        if ctx.startswith("T3_"):
            return "13/0.7" if ctx.endswith("_S") else "5/0.7"
        return "5/0.7" if trg.startswith("T3_") else ""
    t3L = L["trigger"].str.startswith("T3_") | L["context_or_filter"].str.startswith("T3_")
    return dict(
        missing_columns=len(set(REGISTRY3_COLUMNS) - set(reg.columns)),
        duplicate_ids=int(reg["candidate_id"].duplicated().sum()),
        bad_pool=int((~reg["pool"].isin(["L", "N"])).sum()),
        bad_side=int((~reg["side"].isin(["long", "short"])).sum()),
        unknown_trigger=int((~L["trigger"].isin(TRIGGER_NAMES)).sum() + (~N["trigger"].isin(N_TRIGGER_NAMES)).sum()),
        unknown_context_or_filter=int((~L["context_or_filter"].isin(FILTER_NAMES)).sum()
                                      + (~N["context_or_filter"].isin(T3_CONTEXT_NAMES)).sum()),
        unknown_market_context=int((~reg["market_context"].isin(MARKET_CONTEXT_NAMES)).sum()
                                   + (L["market_context"] != "NONE").sum()),
        bad_hold=int(sum(bad_hold(x) for x in reg["screen_hold"])),
        t3_missing_or_wrong=int((t3L & (L["t3"].fillna("") == "")).sum()
                                + sum((("" if pd.isna(r["t3"]) else str(r["t3"])) != expect_t3(r)) for _, r in N.iterrows())),
        duplicate_specs=int(reg.duplicated(["pool", "trigger", "context_or_filter", "market_context", "side", "t3",
                                            "screen_hold"]).sum()),
    )

# ----------------------------------------------------------------------------- registry (Programs 1-2)
REGISTRY_COLUMNS = ["candidate_id", "family", "trigger", "filter", "side", "t3_length", "t3_vf"]

def spec_hash(trigger_name, filter_name, side, L, V) -> str:
    spec = dict(t=trigger_name, f=filter_name, s=side, L=None if pd.isna(L) else int(L),
                V=None if pd.isna(V) else round(float(V), 3), v=DEF_VERSION)
    return hashlib.sha1(json.dumps(spec, sort_keys=True).encode()).hexdigest()[:16]

def build_registry(reg: pd.DataFrame) -> pd.DataFrame:
    """Executable registry: adds uses_t3, cell_id (trigger|filter|side), the exact rule text and the spec hash.
    Optional column screen_bars: the bars held by the P2 screen exit for that row (48 = until the session flat-by)."""
    r = reg[REGISTRY_COLUMNS + [c for c in ("screen_bars",) if c in reg.columns]].copy()
    r["uses_t3"] = [uses_t3(t, f) for t, f in zip(r["trigger"], r["filter"])]
    r["cell_id"] = r["trigger"] + "|" + r["filter"] + "|" + r["side"]
    r["trigger_def"] = r["trigger"].map(TRIGGERS)
    r["filter_def"] = r["filter"].map(FILTERS)
    r["spec_hash"] = [spec_hash(*v) for v in r[["trigger", "filter", "side", "t3_length", "t3_vf"]].itertuples(index=False)]
    return r

def registry_problems(reg: pd.DataFrame) -> dict:
    """Counts of rows that cannot be executed exactly as written."""
    t3 = reg["trigger"].str.startswith("T3_") | reg["filter"].str.startswith("T3_")
    return dict(
        missing_columns=len(set(REGISTRY_COLUMNS) - set(reg.columns)),
        duplicate_ids=int(reg["candidate_id"].duplicated().sum()),
        unknown_trigger=int((~reg["trigger"].isin(TRIGGER_NAMES)).sum()),
        unknown_filter=int((~reg["filter"].isin(FILTER_NAMES)).sum()),
        bad_side=int((~reg["side"].isin(["long", "short"])).sum()),
        t3_rows_without_params=int((t3 & (reg["t3_length"].isna() | reg["t3_vf"].isna())).sum()),
        t3_params_out_of_range=int((t3 & ((reg["t3_length"] < 1) | (reg["t3_vf"] < 0) | (reg["t3_vf"] > 1))).sum()),
        duplicate_specs=int(reg.duplicated(["trigger", "filter", "side", "t3_length", "t3_vf"]).sum()),
    )
