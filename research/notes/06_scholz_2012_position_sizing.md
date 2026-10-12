# Scholz (2012) — Size Matters! How position sizing determines risk and return of technical timing strategies.
CPQF Working Paper 31, Frankfurt School. 60 pp (main text 25 pp + 21 appendix tables). Read in full.

## Hypothesis
Position sizing (erratic / fixed-unit vs relative "Kelly-style" fractions) changes the risk-return profile of an SMA timing system as
much as the signal does; an "optimal f" found in a backtest is a single-path artefact.

## Simulation design
- GBM (Δt=1/250): S_t = S_{t−1} exp((µ − σ²/2)Δt + σ sqrt(Δt) ε_t); AR(1): r_t = µ_a + ϕ r_{t−1} + ε_t.
- Drift correction: µ_a = µ_e + ½σ² (vol drag); for AR(1): µ_a = µ_e (1−ϕ), σ_a² = σ_e² (1−ϕ²).
- Params calibrated on 35 equity indices 2000-2009: µ ∈ {−11.6%, 5.2%, 22.6%}, σ ∈ {16%, 26%, 39%}, ϕ ∈ {−0.103, 0.025, 0.201}.
- 10,000 paths × 2,500 days (100,000 paths verified stability). SMA(d), d ∈ {5,10,20,38,50,100,200}; long/cash, no stops, no costs.
- Sizing: erratic (1 unit) vs fractions f ∈ {25,50,75,100,125}% of remaining capital.
- Kelly (continuous, normal): x_K = (µ_r − r_f)/σ_r²  -> table 1: 334% (bull), 77% (mean), −118% (bear); 203% (σ=16%), 34% (σ=39%).
- Metrics: TWR = final/initial, total net result, hit ratio, max/mean drawdown, longest loss streak, moments, VaR 5%, Sharpe, excess vs B&H.

## Findings
1. No optimal fraction exists ex-ante: averaged over 10,000 paths, TWR vs f is MONOTONE; concave single-path curves (~38% of paths for SMA38)
   are artefacts -> Anderson & Faff (2004) "optimal f" in futures was sample luck. (Same lesson as Bailey: a single historical path lies.)
2. f = 25% has the HIGHEST Sharpe in every scenario; leverage earns no risk premium. Over-betting hurts more than under-betting.
3. Relative sizing cuts volatility, MaxDD, VaR and improves skew vs erratic sizing, especially with high vol / negative autocorrelation.
4. Short SMAs (5-20) are whipsaw-prone; they win only when lag-1 autocorrelation ϕ > 0; with ϕ < 0 long SMAs are better.
   SMA(38) is the pivot. Timing success is determined by the underlying's (µ, σ, ϕ), not by the rule itself (companion paper
   Scholz & Walther 2011 "The trend is not your friend!").

## Relevance to T3 Lab
- T3(3) is an ultra-short smoother (fast like SMA(3-5)): it is only expected to work if the instrument/timeframe shows POSITIVE short-lag
  autocorrelation (momentum). Intraday index futures at 1-5m usually show slight NEGATIVE lag-1 autocorrelation (bid-ask bounce / mean
  reversion) -> measure ϕ (and variance ratio) per symbol/TF BEFORE testing entries. This is a cheap pre-filter on where T3 can work.
- Lab uses default_qty_value=1 fixed contract = "erratic/absolute" sizing. Fine for Phase 0 signal study (per-trade R-multiple), but results
  must be expressed in R (risk units) or ATR units, not $; otherwise vol regimes dominate the P&L.
- Do NOT pick a position-size or "optimal f" from the backtest; use fractional (≤ ½) Kelly computed from out-of-sample edge, or vol-target.
