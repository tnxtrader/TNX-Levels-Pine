"""Data, sessions, indicators, the 32 triggers + 28 filters, and the candidate registry.

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

def last_index_arrays(df: pd.DataFrame, tradable: np.ndarray | None = None):
    """Per bar: index of the last TRADABLE bar of its session (entries need e <= it, positions close at its close),
    and of the last bar before the next contract roll."""
    n = len(df)
    s = df["sess"].to_numpy()
    tr = np.ones(n, bool) if tradable is None else np.asarray(tradable, bool)
    sess_last = np.empty(n, np.int64)
    ends = np.r_[np.nonzero(np.diff(s))[0], n - 1]
    starts = np.r_[0, ends[:-1] + 1]
    for a, b in zip(starts, ends):
        idx = np.nonzero(tr[a:b + 1])[0]
        sess_last[a:b + 1] = a + idx[-1] if len(idx) else a - 1
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

# ----------------------------------------------------------------------------- registry
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
