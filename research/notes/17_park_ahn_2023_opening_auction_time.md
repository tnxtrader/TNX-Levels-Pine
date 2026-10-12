# Park & Ahn (2023) — Relationship between the reduction of opening price auction-process time and price discovery (KRX).
Korean J. Financial Studies 52(2):213-237. 25 pp, body in Korean (read in the original). Read in full.

## Hypothesis
KRX cut the KOSPI opening call auction from 60 to 30 min (29-Apr-2019). Does price discovery of the opening survive (learning-process
hypothesis), and how fast does the indicative price converge to the open?

## Methods / formulas
Unbiasedness regression (Biais, Hillion & Spatt 1999):
   (V_open − P_prevclose)/P_prevclose = α + β_t (I_t − P_prevclose)/P_prevclose + ε,  I_t = indicative price at time t
   cross-section per day, time-series average of β_t; β -> 0 = pure noise, β -> 1 = full information (signal/noise ratio).
Weighted price contribution (Barclay & Warner 1993): WPC_k = Σ_i ( |ΔP_i| / Σ_j |ΔP_j| ) · ( ΔP_{i,k} / ΔP_i ),
   ΔP_i = open − prev close for stock i, ΔP_{i,k} = indicative price change in interval k.  Sum over k = 1.
Sample: 50 trading days before/after; common stocks; liquidity quintiles Q1..Q5.
KRX design: 30-s random end, static VI (±10% -> 2-min extension), info disclosure from minute 10.
## Results
β at 09:00 ≈ 0.96-0.97 both regimes; random end adds 0.03-0.04. β 1 min after disclosure: 0.136 -> 0.181.
WPC inverse-J: first 15 min contributed 46.2% before vs 79.0% after (1.7×) -> faster but OVERSHOOTING; negative WPC in last 5 minutes
(sharp late correction). Least liquid quintile: slower/less complete discovery.

## Relevance to T3 Lab (futures)
- CME futures open (Globex reopen after the daily halt, and the 9:30 ET cash open for equity index futures) is where overnight info
  is impounded fast and with OVERSHOOT then late correction -> first bars of a session are noisy/mean-reverting; signals there are
  lower quality unless tested separately. Supports a session-open exclusion window or separate statistics.
- WPC-style decomposition can be done with OHLC: share of the daily move achieved by bar k -> tells WHEN in the session price
  discovery happens for your symbol (where trend-following entries have the best information content).
