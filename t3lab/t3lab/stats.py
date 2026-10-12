"""Statistics used by the gates. All inputs are per-session net R series (zeros on no-trade sessions)
unless stated otherwise. Libraries: arch (stationary bootstrap, Hansen SPA), scipy (clustering, normal)."""
from __future__ import annotations
from itertools import combinations
import numpy as np
from scipy import stats as st
from scipy.cluster.hierarchy import linkage, fcluster
from scipy.spatial.distance import squareform
from arch.bootstrap import StationaryBootstrap, SPA, optimal_block_length

EULER = 0.5772156649015329

def sharpe(x):
    s = np.std(x, ddof=1)
    return float(np.mean(x) / s) if s > 0 else 0.0

def nw_tstat(x, lags=None):
    """Newey-West (Bartlett) HAC t-stat of the mean."""
    x = np.asarray(x, float); T = len(x)
    if T < 3:
        return 0.0
    u = x - x.mean()
    L = int(np.floor(4 * (T / 100) ** (2 / 9))) if lags is None else lags
    v = u @ u / T
    for k in range(1, L + 1):
        v += 2 * (1 - k / (L + 1)) * (u[k:] @ u[:-k]) / T
    return float(x.mean() / np.sqrt(v / T)) if v > 0 else 0.0

def block_len(x):
    try:
        b = float(optimal_block_length(np.asarray(x, float))["stationary"].iloc[0])
        return max(1.0, min(b, len(x) / 10))
    except Exception:
        return 5.0

def boot_mean(x, reps=2000, seed=7, alpha=0.05):
    """Stationary bootstrap of the mean: two-sided (1-alpha) percentile CI and one-sided p for mean<=0."""
    x = np.asarray(x, float)
    bs = StationaryBootstrap(block_len(x), x, seed=seed)
    m = np.array([d[0][0].mean() for d in bs.bootstrap(reps)])
    lo, hi = np.quantile(m, [alpha / 2, 1 - alpha / 2])
    p = (1 + np.sum(m - x.mean() >= x.mean())) / (reps + 1)       # centred: null mean = 0
    return dict(mean=float(x.mean()), lo=float(lo), hi=float(hi), p=float(p))

def boot_upper(x, reps=1000, seed=11, q=0.975):
    """Upper q-quantile of the bootstrapped mean (used for 'significantly negative' bucket checks)."""
    x = np.asarray(x, float)
    if len(x) < 3:
        return np.inf
    bs = StationaryBootstrap(block_len(x), x, seed=seed)
    return float(np.quantile([d[0][0].mean() for d in bs.bootstrap(reps)], q))

def bh(p, q):
    """Benjamini-Hochberg: boolean mask of discoveries and adjusted q-values."""
    p = np.asarray(p, float); n = len(p)
    o = np.argsort(p); r = p[o] * n / np.arange(1, n + 1)
    adj = np.minimum.accumulate(r[::-1])[::-1].clip(0, 1)
    out = np.empty(n); out[o] = adj
    return out <= q, out

def k_eff(M, cutoff=0.70):
    """Effective number of independent trials: clusters of columns (average linkage, distance 1-|corr|)."""
    M = np.asarray(M, float)
    keep = M.std(0) > 0
    M = M[:, keep]
    if M.shape[1] < 2:
        return max(1, M.shape[1])
    C = np.nan_to_num(np.corrcoef(M.T))
    D = 1 - np.abs(C); np.fill_diagonal(D, 0)
    Z = linkage(squareform(D, checks=False), "average")
    return int(fcluster(Z, cutoff, "distance").max())

def expected_max_sr(var_sr, N):
    N = max(int(N), 2)
    return float(np.sqrt(var_sr) * ((1 - EULER) * st.norm.ppf(1 - 1 / N) + EULER * st.norm.ppf(1 - 1 / (N * np.e))))

def psr(x, sr_star=0.0):
    """Probabilistic Sharpe ratio (Bailey & Lopez de Prado), per-observation SR."""
    x = np.asarray(x, float); T = len(x); sr = sharpe(x)
    g3 = st.skew(x); g4 = st.kurtosis(x, fisher=False)
    den = np.sqrt(max(1e-12, 1 - g3 * sr + (g4 - 1) / 4 * sr ** 2))
    return float(st.norm.cdf((sr - sr_star) * np.sqrt(T - 1) / den))

def dsr(x, var_sr, N):
    """Deflated Sharpe ratio: PSR against the expected max SR of N independent trials."""
    return psr(x, expected_max_sr(var_sr, N))

def pbo_cscv(M, S=16):
    """Probability of backtest overfitting (Bailey et al. CSCV). M: T x N (sessions x configs)."""
    M = np.asarray(M, float); T, N = M.shape
    if N < 2:
        return np.nan
    if T < 10 * S:
        S = max(2, (T // 10) // 2 * 2)
    blocks = np.array_split(np.arange(T), S)
    s1 = np.array([M[b].sum(0) for b in blocks]); s2 = np.array([(M[b] ** 2).sum(0) for b in blocks])
    cnt = np.array([len(b) for b in blocks])
    def sr(idx):
        n = cnt[idx].sum(); m = s1[idx].sum(0) / n
        v = s2[idx].sum(0) / n - m ** 2
        return m / np.sqrt(np.where(v > 0, v, np.inf))
    lam = []
    for tr in combinations(range(S), S // 2):
        tr = list(tr); te = [i for i in range(S) if i not in tr]
        best = int(np.argmax(sr(tr)))
        oos = sr(te)
        w = (st.rankdata(oos)[best]) / (N + 1)
        lam.append(np.log(w / (1 - w)))
    return float(np.mean(np.array(lam) <= 0))

def spa_pvalue(M, reps=500, seed=3):
    """Hansen SPA test that NO column of M (sessions x models, net R) beats zero. Returns consistent p."""
    M = np.asarray(M, float)
    M = M[:, M.std(0) > 0]
    if M.shape[1] == 0:
        return 1.0
    spa = SPA(np.zeros(len(M)), -M, reps=reps, block_size=int(round(block_len(M.mean(1)))), bootstrap="stationary", seed=seed)
    spa.compute()
    return float(spa.pvalues["consistent"])

def emp_p(null, obs):
    null = np.asarray(null)
    return float((1 + np.sum(null >= obs)) / (len(null) + 1))
