"""Data loading, sessions, indicators and the executable candidate registry.

Every trigger/filter below is the canonical, causal definition of the registry's English text
(only bars <= the signal bar are read; [k] = k bars back). Short = exact mirror of long.
Normalisation uses atr1 = ATR(14) of the PREVIOUS bar, so the signal bar never sizes itself.
"""
from __future__ import annotations
import hashlib, json
import numpy as np, pandas as pd
from numba import njit

DEF_VERSION = "2026-10-12.v1"

# ----------------------------------------------------------------------------- data
def load_symbol(path: str) -> pd.DataFrame:
    return load_symbol_df(pd.read_csv(path))

def load_symbol_df(df: pd.DataFrame) -> pd.DataFrame:
    df["ts"] = pd.to_datetime(df["timestamp"])
    df = df.drop(columns=["timestamp"]).sort_values("ts", kind="stable").reset_index(drop=True)
    for c in ("open", "high", "low", "close", "volume"):
        df[c] = df[c].astype(float)
    return df

def add_sessions(df: pd.DataFrame, session_start_hour: int, gap_minutes: int, buckets: dict) -> pd.DataFrame:
    """CME Globex day: a session starts at `session_start_hour` (17 CT) of the previous calendar day.
    A new session also starts after any gap >= gap_minutes (daily halt, weekend, holiday)."""
    ts = df["ts"]
    sess_date = (ts + pd.Timedelta(hours=24 - session_start_hour)).dt.normalize()
    gap = ts.diff().dt.total_seconds().div(60).fillna(1e9)
    new = (sess_date != sess_date.shift()) | (gap >= gap_minutes)
    df["sess"] = new.cumsum().astype(np.int64) - 1
    df["sess_date"] = sess_date
    df["roll"] = (df["contract"] != df["contract"].shift()).fillna(False)
    df.loc[0, "roll"] = False
    hr = ts.dt.hour + ts.dt.minute / 60.0
    df["bucket"] = "OTHER"
    for name, (a, b) in buckets.items():
        m = (hr >= a) & (hr < b) if a < b else (hr >= a) | (hr < b)
        df.loc[m, "bucket"] = name
    return df

def last_index_arrays(df: pd.DataFrame):
    """For each bar: index of the last bar of its session, and of the last bar before the next roll."""
    n = len(df)
    s = df["sess"].to_numpy()
    sess_last = np.empty(n, np.int64)
    ends = np.r_[np.nonzero(np.diff(s))[0], n - 1]
    starts = np.r_[0, ends[:-1] + 1]
    for a, b in zip(starts, ends):
        sess_last[a:b + 1] = b
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
    a = 2.0 / (n + 1.0); out = np.empty_like(x); out[0] = x[0]
    for i in range(1, len(x)):
        out[i] = a * x[i] + (1 - a) * out[i - 1]
    return out

def t3(close: np.ndarray, length: int, vf: float) -> np.ndarray:
    """Tillson T3 = GD(GD(GD)), GD = (1+v)EMA - v EMA(EMA). Same as ta.ema cascade in Pine."""
    e1 = _ema(close, length); e2 = _ema(e1, length); e3 = _ema(e2, length)
    e4 = _ema(e3, length); e5 = _ema(e4, length); e6 = _ema(e5, length)
    v = vf
    c1 = -v**3; c2 = 3*v*v + 3*v**3; c3 = -6*v*v - 3*v - 3*v**3; c4 = 1 + 3*v + v**3 + 3*v*v
    return c1*e6 + c2*e5 + c3*e4 + c4*e3

def atr_wilder(h, l, c, n=14):
    pc = np.r_[np.nan, c[:-1]]
    tr = np.where(np.isnan(pc), h - l, np.maximum(h - l, np.maximum(np.abs(h - pc), np.abs(l - pc))))
    out = np.full(len(c), np.nan)
    if len(c) > n:
        out[n - 1] = tr[:n].mean()
        for i in range(n, len(c)):
            out[i] = (out[i - 1] * (n - 1) + tr[i]) / n
    return out

def lag(x, k):
    out = np.full(len(x), np.nan); out[k:] = x[:-k] if k else x; return out

def roll_max(x, n):   # max over x[t-n+1..t]
    return pd.Series(x).rolling(n, min_periods=n).max().to_numpy()

def roll_min(x, n):
    return pd.Series(x).rolling(n, min_periods=n).min().to_numpy()

def roll_median(x, n):
    return pd.Series(x).rolling(n, min_periods=n).median().to_numpy()

class Bars:
    """Cached arrays for one symbol. t3 lines are computed lazily per (length, vf)."""
    def __init__(self, df: pd.DataFrame, atr_len: int = 14):
        self.df = df
        self.o, self.h, self.l, self.c = (df[k].to_numpy(float) for k in ("open", "high", "low", "close"))
        self.n = len(df)
        self.atr = atr_wilder(self.h, self.l, self.c, atr_len)
        self.atr1 = lag(self.atr, 1)
        self.rng = self.h - self.l
        self._t3, self._cache = {}, {}

    def t3(self, length, vf):
        k = (int(length), round(float(vf), 3))
        if k not in self._t3:
            self._t3[k] = t3(self.c, *k)
        return self._t3[k]

    def S(self, x, k):  # shorthand for x[k]
        return lag(x, k)

# ----------------------------------------------------------------------------- triggers / filters
def _mirror(b: Bars, side: int):
    """Return (o,h,l,c) as seen from the side's point of view: short = price negated, high<->low swapped."""
    if side == 1:
        return b.o, b.h, b.l, b.c
    return -b.o, -b.l, -b.h, -b.c

def trigger(b: Bars, name: str, side: int, L=None, V=None) -> np.ndarray:
    if not name.startswith("T3_"):
        L = V = None
    key = ("T", name, side)
    if L is None and key in b._cache:
        return b._cache[key]
    o, h, l, c = _mirror(b, side)
    S = lambda x, k: lag(x, k)
    a1 = b.atr1
    with np.errstate(invalid="ignore", divide="ignore"):
        if name.startswith("T3_"):
            T = b.t3(L, V) * (1 if side == 1 else -1)
        if name == "T3_BODY_CROSS":       r = (o <= T) & (c > T)
        elif name == "T3_CLOSE_CROSS":    r = (S(c, 1) <= S(T, 1)) & (c > T)
        elif name == "T3_TWO_CLOSE":      r = (c > T) & (S(c, 1) > S(T, 1)) & (S(c, 2) <= S(T, 2))
        elif name == "T3_THREE_CLOSE":    r = (c > T) & (S(c, 1) > S(T, 1)) & (S(c, 2) > S(T, 2)) & (S(c, 3) <= S(T, 3))
        elif name == "T3_WICK_RECLAIM":   r = (o > T) & (l <= T) & (c > T)
        elif name == "T3_CLEAN_RECLAIM":  r = (l > T) & (S(l, 1) <= S(T, 1))
        elif name == "T3_CROSS_RANGE_BREAK": r = (S(c, 1) > S(T, 1)) & (S(c, 2) <= S(T, 2)) & (c > S(h, 1))
        elif name == "T3_PULLBACK_BREAK": r = (T > S(T, 1)) & (S(l, 1) <= S(T, 1)) & (S(c, 1) > S(T, 1)) & (c > S(h, 1))
        elif name == "T3_TOUCH_3BAR_TREND":
            touch = (S(l, 1) <= S(T, 1)) | (S(l, 2) <= S(T, 2)) | (S(l, 3) <= S(T, 3))
            r = (T > S(T, 3)) & touch & (c > T)
        elif name == "T3_SLOPE_TURN":     r = (T > S(T, 1)) & (S(T, 1) <= S(T, 2)) & (c > T)
        elif name == "T3_DISTANCE_RELEASE":
            d = c - T; r = (d > 0) & (d > S(d, 1))
        elif name.startswith("DONCHIAN_"):
            N = int(name.split("_")[1]); r = c > S(roll_max(h, N), 1)
        elif name == "PREVBAR_BREAK":     r = c > S(h, 1)
        elif name == "INSIDE_BAR_BREAK":  r = (S(h, 1) <= S(h, 2)) & (S(l, 1) >= S(l, 2)) & (c > S(h, 1))
        elif name.startswith("COMPRESSION_BREAK_"):
            N = int(name.rsplit("_", 1)[1])
            hiN, loN = S(roll_max(h, N), 1), S(roll_min(l, N), 1)
            r = ((hiN - loN) <= 0.75 * a1 * np.sqrt(N)) & (c > hiN)
        elif name == "RANGE_EXPANSION_CLOSE":
            rng = h - l; r = (rng > a1) & ((c - l) >= 0.75 * rng)
        elif name.startswith("MOMENTUM_"):
            N = int(name.split("_")[1]); r = (c - S(c, N)) >= 0.5 * a1 * np.sqrt(N)
        elif name == "THREE_BAR_CONTINUATION": r = (S(c, 1) > S(c, 2)) & (S(c, 2) > S(c, 3)) & (c > S(h, 1))
        elif name == "GAP_CONTINUATION":  r = ((o - S(c, 1)) >= 0.05 * a1) & (c > o)
        elif name == "TWO_BAR_REVERSAL":  r = (S(c, 1) < S(o, 1)) & (c > S(h, 1))
        elif name == "OUTSIDE_BAR_REVERSAL":
            rng = h - l; r = (h > S(h, 1)) & (l < S(l, 1)) & ((c - l) >= 0.75 * rng) & (rng > 0)
        elif name.startswith("SWEEP_RECLAIM_"):
            N = int(name.rsplit("_", 1)[1]); lvl = S(roll_min(l, N), 2)        # extreme of bars t-2..t-N-1
            r = (S(l, 1) < lvl) & (c > lvl) & (c > S(c, 1))
        elif name.startswith("MEAN_REVERSION_"):
            k = float(name.rsplit("_", 1)[1]); mu = S(pd.Series(c).rolling(20).mean().to_numpy(), 1)
            rng = h - l
            r = (np.minimum(l, S(l, 1)) <= mu - k * a1) & (c > o) & ((c - l) >= 0.5 * rng)
        else:
            raise KeyError(name)
    r = np.nan_to_num(r.astype(float), nan=0).astype(bool)
    if L is None:
        b._cache[key] = r
    return r

def filt(b: Bars, name: str, side: int, L=None, V=None) -> np.ndarray:
    if not name.startswith("T3_"):
        L = V = None
    key = ("F", name, side)
    if L is None and key in b._cache:
        return b._cache[key]
    o, h, l, c = _mirror(b, side)
    S = lambda x, k: lag(x, k)
    a1 = b.atr1; rng = h - l
    with np.errstate(invalid="ignore", divide="ignore"):
        if name.startswith("T3_"):
            T = b.t3(L, V) * (1 if side == 1 else -1); d = c - T
        if name == "NONE":                r = np.ones(b.n, bool)
        elif name.startswith("BODY_"):    r = (np.abs(c - o) >= int(name[5:]) / 100 * rng) & (rng > 0)
        elif name.startswith("CLV_"):     r = ((c - l) >= int(name[4:]) / 100 * rng) & (rng > 0)
        elif name == "DIRECTIONAL_BODY":  r = c > o
        elif name == "FAVOURABLE_WICK_30": r = ((np.minimum(o, c) - l) >= 0.30 * rng) & (rng > 0)
        elif name == "NO_ADVERSE_WICK_20": r = ((h - np.maximum(o, c)) <= 0.20 * rng) & (rng > 0)
        elif name == "GAP_ALIGNED_010":   r = (o - S(c, 1)) >= 0.10 * a1
        elif name == "MOMENTUM_2":        r = c > S(c, 2)
        elif name == "PREV_OPPOSITE_CANDLE": r = S(c, 1) < S(o, 1)
        elif name == "RANGE_COMPRESS_060": r = rng <= 0.60 * S(roll_median(b.rng, 20), 1)
        elif name == "RANGE_EXPAND_125":  r = rng >= 1.25 * a1
        elif name == "RANGE_EXPAND_150":  r = rng >= 1.50 * a1
        elif name == "STRUCTURE_STEP":    r = l > S(l, 1)
        elif name.startswith("T3_DIST_FAR_"):  r = d >= int(name[-3:]) / 100 * a1
        elif name.startswith("T3_DIST_NEAR_"): r = (d > 0) & (d <= int(name[-3:]) / 100 * a1)
        elif name == "T3_NOT_EXTENDED_075": r = d <= 0.75 * a1
        elif name == "T3_SLOPE_NONNEG":   r = (T - S(T, 3)) >= 0
        elif name.startswith("T3_SLOPE_STRONG_"): r = (T - S(T, 3)) >= int(name[-3:]) / 100 * a1
        elif name == "VOL_HIGH_120":      r = a1 >= 1.20 * roll_median(a1, 50)
        elif name == "VOL_LOW_080":       r = a1 <= 0.80 * roll_median(a1, 50)
        else:
            raise KeyError(name)
    r = np.nan_to_num(np.asarray(r, float), nan=0).astype(bool)
    if L is None:
        b._cache[key] = r
    return r

def candidate_signal(b: Bars, row) -> np.ndarray:
    side = 1 if row["side"] == "long" else -1
    L = None if pd.isna(row["t3_length"]) else int(row["t3_length"])
    V = None if pd.isna(row["t3_vf"]) else float(row["t3_vf"])
    return trigger(b, row["trigger"], side, L, V) & filt(b, row["filter"], side, L, V)

TRIGGER_NAMES = ["T3_BODY_CROSS", "T3_CLOSE_CROSS", "T3_TWO_CLOSE", "T3_THREE_CLOSE", "T3_WICK_RECLAIM",
    "T3_CLEAN_RECLAIM", "T3_CROSS_RANGE_BREAK", "T3_PULLBACK_BREAK", "T3_TOUCH_3BAR_TREND", "T3_SLOPE_TURN",
    "T3_DISTANCE_RELEASE", "DONCHIAN_3", "DONCHIAN_5", "DONCHIAN_8", "DONCHIAN_12", "DONCHIAN_20", "PREVBAR_BREAK",
    "INSIDE_BAR_BREAK", "COMPRESSION_BREAK_3", "COMPRESSION_BREAK_5", "COMPRESSION_BREAK_8", "RANGE_EXPANSION_CLOSE",
    "MOMENTUM_3", "MOMENTUM_6", "THREE_BAR_CONTINUATION", "GAP_CONTINUATION", "TWO_BAR_REVERSAL",
    "OUTSIDE_BAR_REVERSAL", "SWEEP_RECLAIM_5", "SWEEP_RECLAIM_10", "MEAN_REVERSION_2", "MEAN_REVERSION_3"]
FILTER_NAMES = ["NONE", "BODY_35", "BODY_55", "BODY_70", "CLV_55", "CLV_65", "CLV_75", "DIRECTIONAL_BODY",
    "FAVOURABLE_WICK_30", "NO_ADVERSE_WICK_20", "GAP_ALIGNED_010", "MOMENTUM_2", "PREV_OPPOSITE_CANDLE",
    "RANGE_COMPRESS_060", "RANGE_EXPAND_125", "RANGE_EXPAND_150", "STRUCTURE_STEP", "T3_DIST_FAR_025",
    "T3_DIST_FAR_050", "T3_DIST_NEAR_025", "T3_DIST_NEAR_050", "T3_DIST_NEAR_100", "T3_NOT_EXTENDED_075",
    "T3_SLOPE_NONNEG", "T3_SLOPE_STRONG_015", "T3_SLOPE_STRONG_030", "VOL_HIGH_120", "VOL_LOW_080"]

# ----------------------------------------------------------------------------- registry
TRIGGER_DEF = {
 "T3_BODY_CROSS": "o<=T3 and c>T3", "T3_CLOSE_CROSS": "c[1]<=T3[1] and c>T3",
 "T3_TWO_CLOSE": "c>T3, c[1]>T3[1], c[2]<=T3[2]", "T3_THREE_CLOSE": "c,c[1],c[2]>T3 and c[3]<=T3[3]",
 "T3_WICK_RECLAIM": "o>T3 and l<=T3 and c>T3", "T3_CLEAN_RECLAIM": "l>T3 and l[1]<=T3[1]",
 "T3_CROSS_RANGE_BREAK": "c[1]>T3[1], c[2]<=T3[2], c>h[1]",
 "T3_PULLBACK_BREAK": "T3>T3[1], l[1]<=T3[1]<c[1], c>h[1]",
 "T3_TOUCH_3BAR_TREND": "T3>T3[3], any l[k]<=T3[k] k=1..3, c>T3",
 "T3_SLOPE_TURN": "T3>T3[1], T3[1]<=T3[2], c>T3", "T3_DISTANCE_RELEASE": "d=c-T3>0 and d>d[1]",
 "DONCHIAN_N": "c > max(h[1..N])", "PREVBAR_BREAK": "c>h[1]",
 "INSIDE_BAR_BREAK": "h[1]<=h[2], l[1]>=l[2], c>h[1]",
 "COMPRESSION_BREAK_N": "range(h,l over bars 1..N) <= 0.75*atr1*sqrt(N) and c > max(h[1..N])",
 "RANGE_EXPANSION_CLOSE": "(h-l)>atr1 and CLV>=0.75",
 "MOMENTUM_N": "c-c[N] >= 0.5*atr1*sqrt(N)", "THREE_BAR_CONTINUATION": "c[1]>c[2]>c[3] and c>h[1]",
 "GAP_CONTINUATION": "o-c[1] >= 0.05*atr1 and c>o", "TWO_BAR_REVERSAL": "c[1]<o[1] and c>h[1]",
 "OUTSIDE_BAR_REVERSAL": "h>h[1], l<l[1], CLV>=0.75",
 "SWEEP_RECLAIM_N": "lvl=min(l[2..N+1]); l[1]<lvl and c>lvl and c>c[1]",
 "MEAN_REVERSION_k": "mu=SMA(c,20)[1]; min(l,l[1])<=mu-k*atr1 and c>o and CLV>=0.5",
}
FILTER_DEF = {
 "NONE": "true", "BODY_x": "|c-o|>=x*range", "CLV_x": "(c-l)/range>=x (short: (h-c)/range)",
 "DIRECTIONAL_BODY": "c>o", "FAVOURABLE_WICK_30": "lower wick>=0.30*range (short: upper)",
 "NO_ADVERSE_WICK_20": "upper wick<=0.20*range (short: lower)", "GAP_ALIGNED_010": "o-c[1]>=0.10*atr1",
 "MOMENTUM_2": "c>c[2]", "PREV_OPPOSITE_CANDLE": "c[1]<o[1]",
 "RANGE_COMPRESS_060": "range<=0.60*median(range[1..20])", "RANGE_EXPAND_125": "range>=1.25*atr1",
 "RANGE_EXPAND_150": "range>=1.50*atr1", "STRUCTURE_STEP": "l>l[1] (short: h<h[1])",
 "T3_DIST_FAR_x": "c-T3>=x*atr1", "T3_DIST_NEAR_x": "0<c-T3<=x*atr1", "T3_NOT_EXTENDED_075": "c-T3<=0.75*atr1",
 "T3_SLOPE_NONNEG": "T3-T3[3]>=0", "T3_SLOPE_STRONG_x": "T3-T3[3]>=x*atr1",
 "VOL_HIGH_120": "atr1>=1.20*median(atr1, 50)", "VOL_LOW_080": "atr1<=0.80*median(atr1, 50)",
}

def _defkey(name, table):
    if name in table:
        return table[name]
    for k, v in table.items():
        stem = k.rsplit("_", 1)[0]
        if k.endswith(("_N", "_x", "_k")) and name.startswith(stem + "_"):
            return v.replace("N", name.rsplit("_", 1)[1]) if k.endswith("_N") else v + f"  [{name}]"
    raise KeyError(name)

NESTED = [("T3_DIST_NEAR_025", "T3_DIST_NEAR_050", "T3_DIST_NEAR_100"), ("CLV_75", "CLV_65", "CLV_55"),
          ("BODY_70", "BODY_55", "BODY_35"), ("RANGE_EXPAND_150", "RANGE_EXPAND_125"),
          ("T3_SLOPE_STRONG_030", "T3_SLOPE_STRONG_015", "T3_SLOPE_NONNEG"), ("T3_DIST_FAR_050", "T3_DIST_FAR_025")]

def canonical_registry(raw: pd.DataFrame) -> pd.DataFrame:
    """Fix the supplied registry: executable definitions, recomputed uses_t3, deterministic spec hash,
    cell id (trigger x filter x side) so T3 settings become a separate, testable axis."""
    r = raw.copy()
    r["cell_id"] = r["trigger"] + "|" + r["filter"] + "|" + r["side"]
    r["trigger_def"] = r["trigger"].map(lambda x: _defkey(x, TRIGGER_DEF))
    r["filter_def"] = r["filter"].map(lambda x: _defkey(x, FILTER_DEF))
    r["uses_t3_recomputed"] = r["trigger"].str.startswith("T3_") | r["filter"].str.startswith("T3_")
    def h(row):
        spec = dict(t=row.trigger, f=row["filter"], s=row.side,
                    L=None if pd.isna(row.t3_length) else int(row.t3_length),
                    V=None if pd.isna(row.t3_vf) else round(float(row.t3_vf), 3), v=DEF_VERSION)
        return hashlib.sha1(json.dumps(spec, sort_keys=True).encode()).hexdigest()[:16]
    r["spec_hash"] = r.apply(h, axis=1)
    r["t3_settings_in_cell"] = r.groupby("cell_id")["candidate_id"].transform("count")
    r["registry_flags"] = ""
    r.loc[r["uses_t3"] != r["uses_t3_recomputed"], "registry_flags"] += "uses_t3_mismatch;"
    r.loc[r["uses_t3_recomputed"] & (r["t3_settings_in_cell"] == 1), "registry_flags"] += "no_t3_neighbour;"
    r.loc[(r["trigger"] == "RANGE_EXPANSION_CLOSE") & (r["filter"] == "RANGE_COMPRESS_060"), "registry_flags"] += "contradictory_pair;"
    r.loc[(r["trigger"] == "GAP_CONTINUATION") & (r["filter"] == "GAP_ALIGNED_010"), "registry_flags"] += "redundant_pair;"
    r.loc[r["trigger"].isin(["DONCHIAN_3", "PREVBAR_BREAK"]), "registry_flags"] += "overlaps_DONCHIAN3_PREVBAR;"
    r.loc[r["trigger"].str.startswith("RANGE_EXPANSION") & r["filter"].str.startswith("RANGE_EXPAND"), "registry_flags"] += "redundant_pair;"
    r.loc[r["trigger"].str.startswith("RANGE_EXPANSION") & r["filter"].str.startswith("CLV_"), "registry_flags"] += "redundant_pair;"
    return r
