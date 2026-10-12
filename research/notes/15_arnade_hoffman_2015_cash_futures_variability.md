# Arnade & Hoffman (2015) — Impact of price variability on cash/futures relationships: market efficiency and price discovery.
J. Agricultural & Applied Economics 47(4):539-559. 21 pp. Read in full.

## Hypothesis
Recent price variability changes how fast cash and futures prices revert to their long-run equilibrium and which market leads.

## Model / formulas
Long-run (Engle–Granger step 1): P_cash,t = c + β_fut P_fut,t + u_t      (long-run efficient if c = 0, β = 1)
ECM (step 2): ΔP_cash,t = γ_csh u_{t−1} + θ_csh (u_{t−1}·vp_{t−1}) + Σ_{k=1..3}(a_k ΔP_cash,t−k + b_k ΔP_fut,t−k) + e_t   (same for futures)
vp = lagged 12-day moving variance (exogenous; Wu–Hausman confirmed; robust for 9-16-day windows).
Adjustment half-life = ln(0.5)/ln(1 + γ) days. Ratio = (γ + θ·vp)/γ (>1 => variability speeds convergence).
Price-discovery weights (Schwarz–Szakmary 1994 / Theissen 2002): market that adjusts LESS gets the larger weight.
Structural breaks found with rolling unit-root regressions (Zivot–Andrews-like) -> 3 regimes 1992-99, 1999-2005, 2005-13.

## Results
Cash/futures cointegrated in every sub-period; R² 0.99 (beans) / 0.97 (meal); β_fut 1.02 / 1.05.
Half-lives: cash beans 41 -> 13 -> 37 days; futures beans 37 -> (wrong-way: futures lead) -> 46 days. Meal futures 230 days (leader).
Price-discovery weights: futures beans 0.48 / Nc(=100%) / 0.58, whole period 0.66; futures meal 0.835 / Nc / 0.72, whole 0.86.
Variability speeds cash adjustment in 10/14 cases; whole-sample estimates hide regime changes.
Data handling: roll S1->S2 on last day of month before delivery; never difference across a roll; delivery months excluded; open-price
models gave different results than settle/HL-average (price field choice matters).

## Relevance to T3 Lab
- CONTINUOUS-CONTRACT ROLLS: an unadjusted TradingView continuous series (e.g. ES1!) contains roll gaps that are not tradable moves;
  they hit T3, ATR and "GapUp/GapDown" filters (IDs 15, 24, 28, 30, 32, 43, 65) and can create fake signals/barrier hits. Use back-adjusted
  continuous data ("Adjust for contract changes"/ "BADJ") or filter roll dates. Validate that no trade straddles a roll.
- Regime breaks are real (3 regimes in 20 years); a single full-sample statistic hides them -> report Lab results per sub-period.
- Lagged short-window variance as an interaction term (signal × vol) is a clean, exogenous way to let volatility modulate a rule.
