# Bag & Goel — Weak form of call auction prices: simulation using Monte Carlo variants. Capital Markets Review 31(1):59-71
# (published 2023; the zip's _SOURCES.md lists it as 2025). 13 pp. Read in full.

## Hypotheses
H1 NSE pre-open equilibrium prices follow a random walk; H2 no day-to-day correlation of residuals; H3 no cross-stock correlation;
H4 multivariate log-normal MC is more accurate than univariate MC.
## Methods / formulas
GBM: ΔP/P = µΔt + σ ε sqrt(Δt);  residual ε_t = Δ ln P_t − µ̂ (prediction error);  t = mean(ε)/SE(ε)
Multivariate MC: Σ_ij = Cov(X_i, X_j) of log prices; Cholesky Σ = L D Lᵀ = CᵀC, C = sqrt(D)Lᵀ; Y ~ MN(µ, Σ), X = exp(Y).
"Significant" correlation threshold |ρ| ≥ 0.3 (Ratner 2009 rule of thumb — not a statistical test).
Data: NIFTY-50 pre-open prices, only 30 consecutive trading days of 2019; 500 MC trials.
## Results
Mean residuals insignificant (e.g. ACC t = 0.21) -> "random walk"; lag-1 residual corr e.g. ACC ρ = −0.31; cross-stock ρ up to 0.67.
Univariate MC simulated SD 0.10-1.42% vs actual 0.82-3.53% (under-dispersed by ~3/4); multivariate MC 0.68-3.33% (closer).
## Critical assessment
Very weak evidence: 30 observations per stock, ad-hoc |ρ|≥0.3 "significance", hypothesis logic inverted (cross-correlation is not
evidence for/against weak-form efficiency), no proper RW tests (variance ratio / runs) despite citing them. Use for nothing but:
## Relevance to T3 Lab
- Negative lag-1 correlation of opening/auction residuals (ρ ≈ −0.3 example) is consistent with Camilleri's overnight reversal and Park's
  overshoot: the open overshoots then partly reverts.
- Monte Carlo with Cholesky-correlated shocks is the right tool to build a NULL distribution for multi-instrument strategy tests, and
  univariate i.i.d. MC under-states dispersion -> any MC/bootstrap of the Lab must keep volatility clustering & cross-correlation
  (block bootstrap / stationary bootstrap), otherwise significance is overstated.
