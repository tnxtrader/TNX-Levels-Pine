"""Statistics behind the gates. Inputs are per-session series (zeros on sessions without a trade) unless stated.
Libraries: arch (stationary bootstrap, Hansen SPA), scipy (normal, clustering)."""
from __future__ import annotations
from itertools import combinations
import numpy as np
from scipy import stats as st
from scipy.cluster.hierarchy import linkage, fcluster
from scipy.spatial.distance import squareform
from arch.bootstrap import StationaryBootstrap, SPA, optimal_block_length

EULER = 0.5772156649015329

def sharpe(x):
    """Per-observation Sharpe ratio (not annualised)."""
    x = np.asarray(x, float)
    s = x.std(ddof=1) if len(x) > 1 else 0.0
    return float(x.mean() / s) if s > 0 else 0.0

def nw_tstat(x, lags=None):
    """Newey-West (1987) HAC t-statistic of the mean, Bartlett kernel, lag L = floor(4 (T/100)^(2/9))."""
    x = np.asarray(x, float); T = len(x)
    if T < 3:
        return 0.0
    u = x - x.mean()
    L = int(np.floor(4 * (T / 100) ** (2 / 9))) if lags is None else lags
    v = u @ u / T
    for k in range(1, L + 1):
        v += 2 * (1 - k / (L + 1)) * (u[k:] @ u[:-k]) / T
    return float(x.mean() / np.sqrt(v / T)) if v > 0 else 0.0

def p_one_sided(t):
    return float(1 - st.norm.cdf(t))

def block_len(x):
    """Politis-White (2004, corrected 2009) optimal block length for the stationary bootstrap."""
    try:
        b = float(optimal_block_length(np.asarray(x, float))["stationary"].iloc[0])
        return max(1.0, min(b, len(x) / 10))
    except Exception:
        return max(1.0, len(x) ** (1 / 3))

def boot_mean(x, reps=2000, seed=7, alpha=0.05):
    """Politis-Romano (1994) stationary bootstrap of the mean: percentile CI and one-sided p for H0: mean <= 0
    (p = (1 + #{m*_b - mean >= mean}) / (B + 1))."""
    x = np.asarray(x, float)
    bs = StationaryBootstrap(block_len(x), x, seed=seed)
    m = np.array([d[0][0].mean() for d in bs.bootstrap(reps)])
    lo, hi = np.quantile(m, [alpha / 2, 1 - alpha / 2])
    p = (1 + np.sum(m - x.mean() >= x.mean())) / (reps + 1)
    return dict(mean=float(x.mean()), lo=float(lo), hi=float(hi), p=float(p))

def boot_upper(x, reps=1000, seed=11, q=0.975):
    """Upper q-quantile of the stationary-bootstrapped mean (a bucket is 'significantly negative' if this is < 0)."""
    x = np.asarray(x, float)
    if len(x) < 3:
        return np.inf
    bs = StationaryBootstrap(block_len(x), x, seed=seed)
    return float(np.quantile([d[0][0].mean() for d in bs.bootstrap(reps)], q))

def bh(p, q):
    """Benjamini-Hochberg (1995): discoveries at FDR level q and adjusted q-values."""
    p = np.asarray(p, float); n = len(p)
    if n == 0:
        return np.zeros(0, bool), np.zeros(0)
    o = np.argsort(p); r = p[o] * n / np.arange(1, n + 1)
    adj = np.minimum.accumulate(r[::-1])[::-1].clip(0, 1)
    out = np.empty(n); out[o] = adj
    return out <= q, out

def k_eff(M, cutoff=0.70):
    """Effective number of independent trials: average-linkage clusters of the columns of M on distance 1 - |corr|,
    cut at `cutoff` (trial clustering in the spirit of Lopez de Prado 2019)."""
    M = np.asarray(M, float)
    M = M[:, M.std(0) > 0]
    if M.shape[1] < 2:
        return max(1, M.shape[1])
    C = np.nan_to_num(np.corrcoef(M.T))
    D = 1 - np.abs(C); np.fill_diagonal(D, 0); D = np.clip(D, 0, None)
    Z = linkage(squareform(D, checks=False), "average")
    return int(fcluster(Z, cutoff, "distance").max())

def expected_max_sr(var_sr, N):
    """Expected maximum Sharpe of N independent trials whose estimated SRs have variance var_sr (Bailey & Lopez de Prado 2014)."""
    N = max(int(N), 2)
    return float(np.sqrt(var_sr) * ((1 - EULER) * st.norm.ppf(1 - 1 / N) + EULER * st.norm.ppf(1 - 1 / (N * np.e))))

def _sr_moments(x):
    x = np.asarray(x, float)
    return sharpe(x), float(st.skew(x)), float(st.kurtosis(x, fisher=False))

def psr(x, sr_star=0.0):
    """Probabilistic Sharpe ratio (Bailey & Lopez de Prado 2012): P(true SR > sr_star)."""
    x = np.asarray(x, float); T = len(x)
    if T < 3:
        return 0.0
    sr, g3, g4 = _sr_moments(x)
    den = np.sqrt(max(1e-12, 1 - g3 * sr + (g4 - 1) / 4 * sr ** 2))
    return float(st.norm.cdf((sr - sr_star) * np.sqrt(T - 1) / den))

def null_sr_var(T):
    """Variance of the estimated Sharpe ratio of a series of length T whose true SR is 0: 1/(T-1)
    (Mertens 2002, Opdyke 2007: the skewness and kurtosis terms vanish at SR = 0)."""
    return 1.0 / max(T - 1, 1)

def dsr(x, N, var_sr=None):
    """Deflated Sharpe ratio (Bailey & Lopez de Prado 2014): PSR of x against the expected maximum SR of N independent
    trials with no edge. var_sr defaults to the null sampling variance 1/(T-1) of the False Strategy Theorem."""
    x = np.asarray(x, float)
    return psr(x, expected_max_sr(null_sr_var(len(x)) if var_sr is None else var_sr, N))

def min_trl(x, sr_star=0.0, alpha=0.05):
    """Minimum track record length (Bailey & Lopez de Prado 2012): observations needed for PSR(sr_star) >= 1 - alpha,
    given the SR, skewness and kurtosis of x. inf when SR <= sr_star."""
    sr, g3, g4 = _sr_moments(x)
    if sr <= sr_star:
        return np.inf
    return float(1 + (1 - g3 * sr + (g4 - 1) / 4 * sr ** 2) * (st.norm.ppf(1 - alpha) / (sr - sr_star)) ** 2)

def pbo_cscv(M, S=16):
    """Probability of backtest overfitting (Bailey, Borwein, Lopez de Prado & Zhu 2017) by CSCV.
    M: T x N (sessions x configs). For every split of S blocks into halves: the in-sample best config, its out-of-sample
    relative rank w = rank/(N+1), logit = ln(w/(1-w)); PBO = share of logit <= 0."""
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
        w = st.rankdata(sr(te))[best] / (N + 1)
        lam.append(np.log(w / (1 - w)))
    return float(np.mean(np.array(lam) <= 0))

def spa_pvalue(M, reps=500, seed=3):
    """Hansen (2005) SPA test, consistent p-value, of H0: no column of M (sessions x models, net R) beats zero.
    Losses = -M, benchmark loss = 0, stationary bootstrap, studentized."""
    M = np.asarray(M, float)
    M = M[:, M.std(0) > 0]
    if M.shape[1] == 0:
        return 1.0
    spa = SPA(np.zeros(len(M)), -M, reps=reps, block_size=int(round(block_len(M.mean(1)))), bootstrap="stationary", seed=seed)
    spa.compute()
    return float(spa.pvalues["consistent"])

def emp_p(null, obs):
    """Randomization-test p-value: (1 + #{null >= observed}) / (B + 1)."""
    null = np.asarray(null)
    return float((1 + np.sum(null >= obs)) / (len(null) + 1))
