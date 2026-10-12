# Guo & Zervos (2015) — Optimal execution with multiplicative price impact. SIAM J. Financial Math 6:281-306. 27 pp. Read (model,
# propositions, results; long proofs skimmed).
## Hypothesis / model
GBM price with log-linear (multiplicative) impact of the large trader's cumulative sales ξ^s and purchases ξ^b:
  dX_t = µ X_t dt + σ X_t dW_t − λ X_t ∘ dξ^s_t + λ X_t ∘ dξ^b_t   (impact is additive in log X; block sale of size Δ multiplies X by e^{−λΔ})
Objective: maximise E[ ∫ e^{−δt} (X_t − C_s) ∘ dξ^s_t − ∫ e^{−δt}(X_t + C_b) ∘ dξ^b_t ]  (C_s,C_b = half-spread / proportional costs)
Results: asymmetric buy/sell impact -> arbitrage; symmetric -> none. No price manipulation iff µ = 0. Need δ ≥ max(µ,0).
With C_s = 0: sell everything at t=0, v = (x/λ)(1 − e^{−λy}). T=∞: explicit solution with a critical price F°: wait below F°,
sell to push price to F° and keep it there ("waiting" vs "selling" regions) — a free-boundary / optimal-stopping structure.
## Relevance: LOW for a 1-lot retail OHLC system (impact λ ≈ 0). Conceptual takeaways only: (i) exits are an optimal-stopping problem
with a state-dependent boundary (not fixed ATR multiples); (ii) any edge must survive the bid/ask C_s, C_b; (iii) "no free lunch"
(no manipulation) holds only when the price is a martingale, i.e. edge = drift conditional on signal — that is what must be measured.

# Horst & Kivman (2024) — Optimal trade execution under small market impact and portfolio liquidation with semimartingale strategies.
# Finance & Stochastics 28:759-812. 55 pp. Read intro, model and main theorems; BSDE convergence proofs skimmed.
## Model
Inventory X, trading rate ξ; transient impact (Obizhaeva-Wang type) dY_t = −ρ_t Y_t dt + γ dX_t with stochastic resilience ρ_t;
cost = instantaneous impact η ξ² + transient impact Y + risk/penalty terms; liquidation constraint X_T = 0.
As η -> 0 the optimal inventory converges (Skorokhod M2) to an infinite-variation semimartingale with block trades at start/end;
cost characterised by a 1-D quadratic BSDE (independent of liquidation constraint).
## Relevance: LOW (institutional liquidation). Takeaway: impact is TRANSIENT with stochastic resilience -> displacement from aggressive
flow decays (mean-reverts) at a rate that itself varies; consistent with Tóth's partial impact reversion -> supports pullback-type
entries over chasing impulse bars, and supports regime-dependent behaviour.
