# Liu & Maheu (2009) — Forecasting realized volatility: a Bayesian model-averaging approach. J. Applied Econometrics 24:709-733.
U. Toronto WP 313. 33 pp. Read in full (equations are images; standard forms recorded).

## Hypothesis
No single volatility model dominates across markets/horizons (model risk); averaging 72 HAR/AR specs by predictive likelihood
(BMA) beats picking one. Power variation (|r|^p, p≈1-1.5) is a better predictor than squared returns.

## Formulas
Price: dp(t) = µ(t)dt + σ(t)dW(t) + κ(t)dq(t);  QV_{t+1} = ∫σ²(s)ds + Σ κ²(s) (jumps)
RPV_{t+1}(p) = m^{p/2−1} µ_p^{−1} Σ_j |r_{t,j}|^p,  µ_p = E|Z|^p = 2^{p/2} Γ((p+1)/2)/Γ(1/2);  RV = RPV(2)
RBP_{t+1} = µ1^{−2} Σ |r_{t,j}||r_{t,j−1}|;  jump term J_t = log(RV_t − RBP_t + 1) (≥0)
Range proxy (Alizadeh-Brandt-Diebold 2002): range_t = log(H_t/L_t)  -> log-range ≈ Gaussian, far more efficient than r²
Hansen–Lunde kernel RV with Bartlett weights, q=3 (microstructure-noise correction); staggered bipower for noise.
HAR-log: log RV_{t,h} = β0 + β1 log RV_{t−1,1} + β2 log RV_{t−5,5} + β3 log RV_{t−22,22} + β4 J_{t−1} (+ L_{t−1} asym for equity) + ε
Predictive likelihood p(y_s..y_t | Y_{s−1}, M_k); BMA weight update p(M_k|Y_T) ∝ p(y_T|Y_{T−1},M_k) p(M_k|Y_{T−1});
Kass–Raftery: 2 log BF 0-2 weak, 2-6 positive, 6-10 strong, >10 very strong.

## Results
- corr(log RV_t, log RPV_{t−i}(p)) maximised at p ≈ 1.0-1.4, not 2 (JPY: 0.673 @ i=1, p=1.39).
- Range beats squared returns; RV/RPV/RBP beat range (but range is what OHLC users have).
- BMA best or 2nd-best in 9/9 market×horizon density forecasts; point-forecast gains modest. Jump term matters everywhere.
- Equal-weight average (SMA) is dominated; weights must be earned out-of-sample (training sample 500 obs, then recursive).

## Relevance to T3 Lab (OHLC)
- With OHLC only, use RANGE-based vol (log(H/L), Parkinson/GK/RS/YZ); ATR is a range measure in price units including gaps — fine,
  but an absolute-value (p≈1) based measure like mean |range| is more persistent/predictable than squared-return σ.
- Model uncertainty lesson applies to entry selection: rather than crowning one of 103 entries, average/ensemble the robust family
  weighted by OOS record (or simply equal-weight the survivors after DSR) — a direct anti-overfitting device.
