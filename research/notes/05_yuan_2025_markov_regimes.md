# Yuan (2025) — Short-Term Returns of Chinese Asset Portfolios Based on Markov Regimes. Series of Conferences J. 1(1):183-191. 9 pp. Read in full.

## Hypothesis
Multi-asset returns switch between latent regimes; weighting by filtered regime probabilities + CVaR budget + max-DD stop
improves OOS Sharpe and tail loss.

## Model / formulas
MS-VAR(1): r_t = µ_{S_t} + A_{S_t} r_{t−1} + ε_t, ε_t ~ N(0, Σ_{S_t}); S_t first-order Markov with transition matrix P = [p_ij].
EM (multiple random starts) for MLE; Kim–Hamilton filter -> smoothed P(S_t=s|F_T) in-sample, 1-step predicted P(S_t=s|F_{t−1}) OOS.
Expected 5-day return: r̂_{t+5} = Σ_s P(S_t=s|F_t) µ_s ; Σ_{t+5} = Σ_s P(S_t=s)(Σ_s + A_s Σ_t A_sᵀ)
Objective: max wᵀ r̂ − (λ/2) wᵀ Σ w,  1ᵀw = 1, w ≥ 0, w_i ≤ 30%.
CVaR (Rockafellar–Uryasev): CVaR_α(w) = min_η { η + 1/((1−α)M) Σ_i max(0, L_i(w) − η) },  L_i = −wᵀR_i
   linearised: u_i ≥ L_i − η, u_i ≥ 0, η + Σu_i/((1−α)M) ≤ κ   (α=0.95, M=10,000 scenarios, κ=2.5% 5-day)
Normal closed form (warned against for fat tails): CVaR_α = µ_L + σ_L φ(z_α)/(1−α)
Shrinkage: Σ_ρ = (1−ρ)Σ + ρ diag(Σ), ρ=0.05. Inputs winsorised at 1/99 pct. Costs 10 bps one way.

## Results (as reported)
States: bull 28.3%, bear 19.7%, volatile/range 52.0%; self-transition p: bull 0.89, bear 0.81, range 0.94.
Ann. return 11.8% vs EW 8.2%, momentum 7.6%, min-var 6.9%; vol 18.3% vs 23.7%; Sharpe 0.64 vs 0.35/0.29; MaxDD 12.8% vs 28.4/35.2/21.6%.
Stress: 2016 circuit breaker −8.2% vs −15.3% EW; 2020 COVID −10.8% vs −19.4%; DD stop triggered 18 times, mean recovery 22 days.

## Critical assessment (do not take at face value)
- Internally inconsistent: method says TWO-state MS-VAR, results report THREE states.
- λ∈[0,5], γ∈[5,50] chosen by grid search to improve "IS Sharpe and OOS drawdown" -> selection on OOS = leakage (Bailey 2014).
- Low-tier venue, no code, no trial count. Use only the *ideas*: regime persistence, regime-conditioned exposure, CVaR sizing.

## Relevance to T3 Lab (OHLC-only futures)
- Range-bound regime dominates (52%, p=0.94): a T3 cross rule is a trend tool; in range regimes it whipsaws. A regime filter
  (HMM on returns/realised range, or simpler proxies: ADX, efficiency ratio, variance ratio) is the missing gate.
- Regime persistence (p_ii 0.8-0.94) means a filtered state probability is a usable *slow* filter — computable from OHLC.
- Equity-curve/max-DD circuit breaker = portfolio-level stop, which Vanstone suggests is where stops should live (not per trade).
