# Tóth, Lempérière, Deremble, de Lataillade, Kockelkoren, Bouchaud (2011) — Anomalous price impact and the critical nature of liquidity.
Phys. Rev. X 1, 021006. 11 pp. Read in full.

## Hypothesis
Because prices are diffusive, the latent (intended) supply/demand profile is V-shaped and VANISHES at the current price
("self-organised criticality"). Consequences: (1) metaorders must be split -> long memory in order-flow sign; (2) square-root impact.

## Formulas
(1) Square-root law: Δ = Y σ_daily sqrt(Q / V_daily),  Y ~ O(1); generally Δ ∝ Q^δ, δ ∈ [0.4, 0.7]
    CFM futures data (~500k metaorders, 2007-2010): δ ≈ 0.5 small-tick, ≈ 0.6 large-tick contracts.
(2) Linear latent book: V^±(p) = b|p − p0|  ->  Δ = sqrt(2Q/b), with b ≈ J/σ² (J = trading rate)  => Δ ∝ σ sqrt(Q/J)
Latent book diffusion eq. for ρ(u,t); stationary near price: ρ_st(u) ≈ b u (u->0), b = 2J/D(0); uniform deposition λ, cancel ν∞:
    ρ_st(u) = ρ∞(1 − e^{−u/u*}), u* = sqrt(D/(2ν∞)) ≈ daily vol.
Best-quote volume q_best ≈ b w²/2;  q_best / V_daily ~ (w/u*)² ~ 10^−3 (w = tick)
Sign autocorrelation of market orders C(ℓ) ~ ℓ^−γ, γ ≈ 0.5 stocks, ≈ 0.8 futures (Lillo-Mike-Farmer: run length P(L) ~ L^−(α+1), γ = α−1)
Diffusivity measure: σ²(ℓ) = ⟨(p_{t+ℓ} − p_t)²⟩/ℓ;  ratio σ(ℓ1)/σ(ℓ2) < 1 subdiffusive (mean-reverting), > 1 superdiffusive (trending)
Impact decay after metaorder: plateau Δ∞/ΔT ≈ 0.75 (γ=0.5); avg execution price ≈ 0.6 ΔT; Farmer "fair price": Δ∞/ΔT = 1/(1+δ) = 2/3.
Impact becomes linear when execution time T ≫ τ_life (book memory).

## Relevance to T3 Lab (1-lot retail, OHLC only)
- 1 contract on ES/NQ/CL is ≪ q_best: own impact ≈ 0; cost = half-spread + queue/stop slippage. Stops are market orders into a
  thin book -> slippage on stops > on entries, especially in fast bars. Model cost asymmetrically (stop slippage ≥ 1 tick).
- Diffusivity / variance-ratio σ²(ℓ)/σ²(1) is the OHLC-computable regime gauge: VR > 1 trend (T3 cross has a chance), VR < 1
  mean-revert (T3 cross whipsaws). This is the physics behind Scholz's autocorrelation result.
- Aggressive flow creates impact that partially REVERTS (~25-40%) after the flow stops: a big impulse candle ("BigRange", "BigBody",
  "FarT3") often marks the end of a metaorder burst -> expect partial giveback; buying the close of an impulse bar buys near the
  temporary-impact peak. Pullback entries (PullbackHold, WickReject) are microstructurally better-founded than chasing.
- Order-flow sign has long memory (futures γ≈0.8) -> short-horizon momentum exists in signed flow, but we cannot see it without
  order-flow data; OHLC proxies (close location value, body/range) are crude shadows of it.
