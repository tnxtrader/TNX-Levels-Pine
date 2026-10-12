# Luo, Klein, Ji, Hou (2019) — Forecasting RV of agricultural commodity futures with infinite Hidden Markov HAR models.
Int. J. Forecasting (QMS WP 2019/10). 43 pp incl. 13 tables, 7 figs. Read in full; equations transcribed from page renders.

## Hypothesis
Futures volatility has long memory + structural breaks; HAR with (infinite) hidden-Markov regime switching in coefficients/variance
forecasts better than plain HAR, statistically and economically.

## Formulas (exact)
(1) r_{t,j} = log P_{t,j} − log P_{t,j−1}   (5-min)            (2) RV_t = Σ_j r²_{t,j}
BPV_t = µ1^{−2} Σ_{j=1}^{M−1} |r_{t,j}||r_{t,j+1}|,  µ1 = sqrt(2/π)             (jump-robust)
(3) J_{t,α} = I{Z_t > Φ_α} (RV_t − BPV_t)^+
(4) Z_t = sqrt(M) (1 − BPV_t/RV_t) / sqrt( (µ1^−4 + 2µ1^−2 − 5) max(1, TQ_t/BPV_t²) )
TQ_t = M µ_{4/3}^{−3} Σ |r_{t,j}|^{4/3}|r_{t,j+1}|^{4/3}|r_{t,j+2}|^{4/3},  µ_p = 2^{p/2} Γ((p+1)/2)/Γ(1/2)
TBPV (Corsi-Pirino-Renò 2010) with threshold ϑ = c_ϑ² V̂, c_ϑ = 3; C_t = RV_t − J_t (continuous part)
Realized semivariances RS+ = Σ r² I(r>0), RS− = Σ r² I(r<0)   (Barndorff-Nielsen, Kinnebrock, Shephard 2010)
RM̄_{h,t−1} = h^{−1} Σ_{j=1}^{h} RM_{t−j}
(9)  HAR:      RV_t = a0 + a_d RV̄_{1,t−1} + a_w RV̄_{5,t−1} + a_m RV̄_{22,t−1} + u_t        (Corsi 2009)
(10) HAR-CJ:   + continuous (C̄) and jump (J̄) parts at 1/5/22
(11) HAR-TCJ:  threshold versions
(12) HAR-ΔJ:   RV_t = a0 + a_{dΔJ} ΔJ̄_{1,t−1} + a_d BPV̄_{1,t−1} + a_w RV̄_5 + a_m RV̄_22 + u_t   (signed jump, Patton–Sheppard 2015)
(13) X_spec = Vol / OI     (14) HAR-spec: HAR + a_s X_spec,t
(15) Centralised HAR (Bollerslev et al. 2018): intercept replaced by expanding long-run mean RV_t^{LR}
IHM / DIHM / IHMC: sticky hierarchical Dirichlet-process HMM on coefficients and/or variance, beam sampler MCMC.
Evaluation: Mincer–Zarnowitz RV_{t+1:t+h} = β0 + β1 R̂V + ε (R²), MAFE, Model Confidence Set (Hansen-Lunde-Nason 2011,
stationary bootstrap block L=16), economic value under mean-variance utility U = E(Rp) − (γ/2)V(Rp), γ ∈ {1, 10};
w_t = r̂_{t+1}/(γ σ̂²_{t+1}).  Robustness: realized kernel (Hansen–Lunde), medRV (Andersen-Dobrev-Schaumburg 2012).

## Data/results
Corn, Cotton, Indica Rice, Palm oil, Soybean (DCE/ZCE), 5-min, 2011-2015, ~1,200 days; 2/3 IS, 1/3 OOS recursive; h=1,5,22 direct.
- All RV series fat-tailed (Jarque-Bera). Posterior regime counts large (~30 coefficient regimes) -> parameters are NOT stable.
- DIHM-HAR best; speculation ratio Vol/OI strongly positive on future vol; regime-switching HARs survive MCS at 25%.
- Economic value positive for short horizon; for h=5/22 only IHMC (variance-switching) helps.

## Relevance to T3 Lab (OHLC only)
- Volatility is forecastable (HAR is the workhorse); ATR(14) is a crude, lagging, single-horizon estimator. A HAR on OHLC range-based
  variance (Parkinson / Garman-Klass / Rogers-Satchell / Yang-Zhang) gives a better forward σ for barrier width and position size.
- Barrier width should use FORECAST vol for the holding horizon, not the past-14-bar average.
- Jumps (gaps, news bars) inflate RV/ATR; a jump-robust estimator (bipower-like on bar ranges, or median-range) keeps barriers sane after
  a spike. ATR at signal bar includes the signal bar's own big range -> stop widens exactly on breakout bars (pro-cyclical).
- Regime switching in vol is the norm -> condition entry stats on vol regime (e.g. ATR percentile / HAR forecast vs long-run mean).
- Vol/OI unavailable (no OI on TV OHLC); volume alone is available on CME futures in TradingView if wanted.
