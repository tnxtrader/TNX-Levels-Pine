# Wu, Wu, Hung, Hassan, Fortino (2020) — Convert index trading to option strategies via LSTM architecture.
Neural Computing & Applications 37:23047-23064 (online 2020). 18 pp. Read in full.

## Hypothesis
Kelly sizing needs win-rate p and odds b; futures margins prevent hitting Kelly size; LSTM can predict a day's p; converting the futures
TP/SL strategy to ATM options (delta-scaled TP/SL) allows finer sizing closer to Kelly.

## Method / formulas
Day strategy on TAIEX futures (TX): observe 08:45-09:15 (30 one-minute OHLCV bars), enter 09:15, TP / SL in points, TIME EXIT 10:00
  = exactly a triple barrier (same structure as the T3 Lab's tp/sl/maxBars).
Features: O,H,L,C of each minute as % of previous close; volume vs 5-day mean minute volume -> 5×30 = 150 inputs.
LABEL (useful idea): PnL scaled to [−1, 1]: +1 at TP, −1 at SL, time-exit PnL/TP if >0 or PnL/SL if <0 (e.g. +60/80 = 0.75, −10/40 = −0.25).
p̂ = (LSTM output + 1)/2; trade only if EV = p̂·TP − (1−p̂)·SL > 0.
Kelly (binary): capital A_t = A_{t−1}(1 + b1 f) win / (1 − b2 f) loss;  G(f) = p ln(1+b1 f) + (1−p) ln(1−b2 f);
  f* = p/b2 − (1−p)/b1  (with b2=1: f* = (p(1+b) − 1)/b, b = TP/SL).  Example p=0.464, b=2 -> f* = 0.196.
  Fair coin, odds 2 -> f* = 25%.  Contracts = floor(A·f* / (SL·point value)), capped by margin.
Options conversion: delta-scaled TP/SL on first ITM strike; buy calls for long, puts for short.
## Results (TX/TXO; train 2018-01..2020-02; TEST ONLY 2020-03..2020-06 = COVID crash/rebound; NO fees/tax)
Long 80/40: futures final 1.74M (from 1.0M), options 3.47M; ideal/actual size ratio 7.69 (futures) vs 4.82 (options).
Short 80/40: futures 0.60M (−40%), options 1.32M. Many settings LOSE (e.g. short 60/40 options 0.26M; 40/40 options 0.17M).
Results swing massively with dropout (0.2/0.4/0.6) and training length -> strong evidence of instability/overfitting.
## Critical assessment
Single 4-month test window in an extreme regime, no costs, ~55-59 trades, many configurations shown (multiple testing, not corrected).
Do not trust profitability claims. Keep the methodology pieces only.
## Relevance to T3 Lab
- Opening-range / first-30-min information is used to decide whether to trade the day: a regime/day filter based on early-session OHLC.
- The continuous [−1,1] barrier label is better than the Lab's binary win/loss for measuring entry quality (captures time-exit P&L).
- Kelly needs a STABLE p and b; Lab's p must come from out-of-sample, then use fractional Kelly (Scholz: ≤½). Kelly on in-sample p
  = guaranteed over-betting.
- Fixed-point TP/SL (not volatility-scaled) in Wu vs ATR-scaled in the Lab: ATR scaling is preferable across regimes.
