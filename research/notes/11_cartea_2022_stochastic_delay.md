# Cartea & Sánchez-Betancourt (2022) — Optimal execution with stochastic delay. Finance & Stochastics 27:1-47. 47 pp. Read in full
# (main text + results; appendix proofs = HJB-QVI viscosity theory, skimmed).

## Hypothesis
With random latency, marketable limit orders (MLO = IoC/FoK with a price cap) beat market orders: they cap slippage and harvest
favourable "flickers" in the quote during the latency window. Patient traders use speed for speculative retries, not to finish early.

## Model / formulas
Fundamental bid S_t = S_0 + σ W_t (arithmetic BM) + flicker ζ at processing time: P(ζ=0)=p0, P(ζ>0)=p+ ~Exp(η+), P(ζ<0)=p− ~Exp(η−).
Latency ~ Exp (expected 10-90 ms). Impulse-control problem -> HJB quasi-variational inequality; trader chooses send times + limit.
Slippage-price-improvement: SPI_i = I_i (L_i − M_i)/tick (+ = filled with slack, − = missed by that many ticks).
Terminal penalty for walking the book: cash = Q(S_T + ζ − a(Q−1))(1−ρ); fees c = 3e-6.
Performance = (TC_T − TC_T^B)/V0 in $ per €1M.

## Data / results (LMAX EUR/USD etc, Sep-2019..Feb-2020; params from Aug-2019 = proper OOS)
- MLOs = 37-67% of trades and 52-73% of taker volume; 57.1% of EUR/USD MLOs MISS; missed limits avg 3 ticks away.
- Taker orders exceed top-of-book size in only 0.1-8.8% of cases.
- RLOS beats TWAP by $2.65-5.14/€M (≈ the fee level) in normal hours; around news +$7-36/€M (2-10× fees).
- Flicker params: normal 9am p− = 0.057, p+ = 0.042; NEWS ±1 min p− = 0.229 (4×), mean adverse size 1/η− 6× larger.
- Outperformance shrinks with latency (10 ms -> 90 ms) and at quiet hours (7pm).

## Relevance to T3 Lab
- The Lab sends MARKET entries at next-bar open and market time-exits. Real fills deviate from the bar's open print; cost is state-
  dependent: around scheduled news and at session opens, adverse slippage probability rises ~4× and size ~6×. Backtest slippage
  must be conditional (session/news), not a constant.
- Execution choice matters for an edge measured in fractions of an ATR: a capped limit (marketable limit at open ± k ticks) prevents
  disaster fills but creates MISSED trades -> a backtest with guaranteed fills is optimistic. Report fill-assumption sensitivity.
- News-minute filter (don't enter within ±N minutes of high-impact releases) is an evidence-based cost filter, not a signal filter.
