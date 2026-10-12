# Docherty, Dong, Song, Tippett (2017) — The Feller diffusion, filter rules and abnormal stock returns
European J. Finance. doi:10.1080/1351847x.2017.1309328. 26 pp. Read in full (equations from page renders).

## Hypothesis
Expected log-return depends on the assumed price process. Under the Feller diffusion (variance ∝ price), the expected log
return varies with price level (prob. of extinction); assuming GBM when the truth is Feller creates FAKE "abnormal returns"
(apparent drift). => You must test the return distribution before measuring "edge".

## Core formulas
Feller SDE:     dx = µ x dt + σ sqrt(x) dW            (variance of increment = σ² x dt)
GBM (eq 23):    E[log(x_t/x_0)] = µ t − ½ σ² t          (volatility drag, constant)
Feller (eq 27): E[log] = µ t − E[ ∫_{x0}^{x_t} exp(−2µ(x_t−y)/σ²)/y dy ]   (drag depends on x0)
Prob. ultimate extinction: exp(−2µ x0/σ²)
Instantaneous log-return under Feller: E[dr] = (µ − σ²/(2x)) dt, Var[dr] = σ²/x dt

## Filter-rule / two-barrier appendix  (= TP/SL barrier maths — directly the T3 Lab's exit)
Buy at x, take profit at b ≥ x, stop at a ≤ x.
(B1) ½ σ² x u'' + µ x u' = 0, u(a)=0, u(b)=1
(B2) P(hit b before a) = u(x) = [exp(−2µx/σ²) − exp(−2µa/σ²)] / [exp(−2µb/σ²) − exp(−2µa/σ²)]
(B3) P(hit a first) = 1 − u(x)
(B4) ½ σ² x υ'' + µ x υ' = −1, υ(a)=υ(b)=0  -> mean exit time υ(x)
(B5) υ(x) = { E[log(x_t/x0)] − E[∫ exp(−2µ(x_t−y)/σ²)/y dy] } / µ,
     E[log] = u log(b/x) + (1−u) log(a/x)
Result: expected ABNORMAL log return over the barrier holding time is ZERO for a random process — barriers do not create edge;
but footnote 14: expected abnormal *buy-and-hold* return under the filter rule is NOT zero (measurement artefact) ->
how you measure returns (log vs simple, per-trade vs per-time) can manufacture edge.

## Table 1 (µ=0.15, σ=1): E[log] by x0: 3.33:-0.0105 | 5:0.0465 | 10:0.1012 | 20:0.1262 | 45:0.1396 | 90:0.1448 ; P(bankrupt) 0.3679 -> 0.

## Derived for T3 Lab (arithmetic Brownian motion limit, standard gambler's-ruin results; to be cross-checked by simulation)
Driftless: P(TP first) = SL/(SL+TP); E[τ] = SL·TP/σ_bar²  (in bars)
With drift µ per bar: P(TP first) = (1 − e^{−2µ·SL/σ²}) / (1 − e^{−2µ(SL+TP)/σ²})
1:1 ATR barrier, ATR ≈ 1.6σ_bar (Parkinson E[H−L] = sqrt(8/π)σ ≈ 1.596σ) -> E[τ] ≈ 2.5 bars -> maxBars=16 rarely binds.
Consistent with Vanstone 2008: 1×ATR stop -> trades last 2.74 days.
=> The Lab's "neutral" triple barrier is really a 1:1 two-barrier coin-flip test; win-rate under zero edge = 50% minus cost/slippage drag.
   Required break-even win-rate with costs c (in ATR units): p* = (SL + c)/(SL + TP).
