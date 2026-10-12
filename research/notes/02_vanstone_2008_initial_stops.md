# Vanstone (2008) — Do initial stop-losses stop losses?  JASSA (4):5-8. 6 pp. Read in full.

## Hypothesis
Initial stops (fixed % or k×ATR) that look protective per trade do NOT improve portfolio-level risk or return
for a positive-expectancy trend system; they degrade it.

## Test-bed
- Entry: close crosses above 50-day EMA (long only), exit: close below 50-EMA. Orders = day+1 market (same as T3 Lab: next-bar open).
- ASX200 constituents incl. delisted (no survivorship), Apr-2000..Dec-2007, $20 each way costs.
- Trade-level: $10k per trade, all trades; metric = Daily Mean Trade Return = mean $ return / days open.
- Portfolio-level: $1m, 2% equity per trade; ANOVA on monthly returns vs no-stop benchmark; Sharpe.

## Results (tables extracted)
Benchmark (no stop): 15,073 trades, daily mean $1.76, avg 15.85 days, Sharpe 0.50.
% stops 1..10%: daily mean 0.34,1.02,1.35,1.42,1.48,1.72,1.70,1.69,1.75,1.73 ; Sharpe 0.25,0.28,0.34,0.34,0.37,0.52,0.48,0.53,0.50,0.50.
   ANOVA F(10,1012)=0.097 -> no significant difference (stops never help, tight stops hurt).
ATR stops 1..5xATR: avg days 2.74,5.40,7.44,9.15,10.53 ; daily mean -18.28,-6.53,-3.16,-1.05,-0.54 ;
   Sharpe -3.27,-0.77,-0.32,0.06,0.29.  1xATR is statistically WORSE: F(5,552)=8.476.
MAE histogram: 1,397 trades hit -4%, only 67 rebounded to profit (Fig 2) — MAE histograms are how stops are usually chosen.

## Conclusion
"If a trading strategy has a positive expectation, then the use of initial stops will only serve to degrade performance."
Recommends studying trailing stops next. Cites Kaminski & Lo (2008) "When do stop-loss rules stop losses?" (theory: stops help
only when returns have momentum/serial correlation — see web research).

## Relevance to T3 Lab
- Lab default slMult = 1.0 ATR(14) and tpMult = 1.0 ATR: Vanstone's single worst cell (1xATR) for an MA-cross trend entry.
  A 1xATR stop on an MA-cross entry cuts trades to ~2.7 bars, i.e. it turns a trend system into a noise-coin-flip minus costs.
- Symmetric 1:1 barrier + noise -> expected hit-rate ~50% before costs; the test then measures cost drag, not entry edge.
- Action: Phase-0 should test stops as a *grid incl. "no stop / wide stop"* and judge at portfolio level (equity/month, ANOVA/
  bootstrap), not per trade; MAE/MFE distributions per entry ID should be logged (needed to choose stops non-arbitrarily).
