# Rosenberg & Traub (2009) — Price discovery in the foreign currency futures and spot market. J. Derivatives 17(2):7-25
# (FRBNY Staff Report 262). 46 pp. Read in full (main text + tables; appendix robustness skimmed).

## Hypothesis
A smaller but more transparent / anonymous venue (CME FX futures) can dominate price discovery if informed traders migrate there.

## Methods / formulas
- Futures order flow without trade signs: QUOTE RULE using forward mid-quote as proxy: +1 if trade > mid, −1 if < mid (Hasbrouck 1988/91);
  instrument = TICK RULE (+1 if price > previous trade, −1 if lower) for 2SLS errors-in-variables correction.
  Daily order flow = Σ signs over RTH (7:20-14:00 CT).
- Evans–Lyons regression: Δs_t = α + β_spot·OF_spot + β_fut·OF_fut + ε  (DM: 1000 net buys -> +2.2%; OF explains ~63% of daily DM return)
- Short-run: corr(r_spot,t, r_fut,t±k) on 3-minute returns (lead–lag).
- Long-run: VECM with 3 lags, daily re-estimated; Hasbrouck (1995) information share bounds via Cholesky orderings;
  Gonzalo–Granger common-factor weight for futures = 1/(1 + a2/a1), a_i = error-correction coefficients.
## Results
1996: futures lead spot by up to 12 minutes (JPY corr ≈ 0.12 at −3 min); futures info share 80-96% (DM spot only 4-9%).
2006: markets tightly linked (contemporaneous corr ≈ 1.0); spot dominates info share (GG spot weights 89/89/83%); Globex futures still
lead spot by ~3 min with low correlation. Price discovery location CHANGES with market structure/transparency.
Futures order flow carries incremental information beyond spot order flow (significant in all 4 currencies).

## Relevance to T3 Lab (OHLC only)
- Order flow explains a large share of returns, but the user has none. OHLC(+volume) proxies exist: tick rule (needs ticks),
  Bulk Volume Classification (Easley, López de Prado, O'Hara 2012: V_buy = V·Φ(ΔP/σ_ΔP)) and close-location value — see web research.
- Lead–lag across related contracts is OHLC-usable: a leading market's T3 slope/returns (e.g. ES vs NQ, ZN vs ES, DX vs 6E, CL vs energy
  equities) can be pulled with request.security as a confirmation; must be re-tested — lead–lag relationships decay as markets integrate.
- Information concentrates in regular trading hours / around 8:30 ET releases -> session/time-of-day conditioning of entries is justified.
