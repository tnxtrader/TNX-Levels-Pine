# LeBaron (1996) — Technical Trading Rule Profitability and Foreign Exchange Intervention. NBER WP 5505.
Scanned PDF (no text layer) -> OCR'd with Tesseract 5.3.4 @300dpi. 18 pp. Read in full; some table cells garbled by OCR,
numbers below are those legible in OCR and confirmed by the text.

## Hypothesis
Simple MA-rule predictability in FX is concentrated on days when the central bank (Fed) intervenes ("leaning against the wind"
creates trends). Remove intervention days -> predictability largely disappears.

## Method / formulas
DM/$ and JY/$ daily (London close) Jan-1979..Dec-1992 (3,544 obs) and weekly (Wednesday close, 694 obs).
(1) ma_t = (1/M) Σ_{i=0}^{M-1} P_{t-i},  M=150 daily, M=30 weekly (chosen as common trader values, NOT optimised, to limit snooping)
(2) s_t = +1 if P_t > ma_t, −1 if P_t < ma_t        (price vs MA = same family as "close > T3")
(3) r_t = s_t (p_{t+1} − p_t − (log(1+r_t) − log(1+r_t*)))   interest-adjusted;  (4) r_t = s_t (p_{t+1} − p_t)
(5) annual Sharpe ≈ sqrt(N) µ_short / σ_short   (N short periods per year; valid if returns independent)
Bootstrap: scramble log price differences with replacement, rebuild 500 random-walk price paths, re-run the rule;
p-value = fraction of simulated paths giving return ≥ actual  (Brock-Lakonishok-LeBaron style null).
(6) GARCH(1,1): h_t = a0 + β h_{t-1} + a1 ε²_{t-1}  -> remove top-10% expected-vol days as control.
Two-state Markov chain for intervention: P(I≠0|I_{t-1}=0)=0.065 (DM) / 0.029 (JY); P(I≠0|I_{t-1}≠0)=0.584 / 0.532.

## Results
- Full sample: daily Sharpe ~0.6-1.0 (DM interest-adj t=2.62, JY t=3.50), bootstrap p ≤0.014; trade fraction only 1.7-2.7% of days.
- Costs (Table 4): DM daily Sharpe 0.689 @0% -> 0.626 @0.1% -> 0.443 @0.2% -> 0.155 @0.5%; JY 1.083 -> 0.981 -> 0.864 -> 0.670.
- Remove Fed-intervention days: DM t-stats insignificant, Sharpe ≈ 0.1; JY bootstrap p 0.146 / 0.198.
- Randomly removing same-persistence Markov days does NOT kill the edge (p≈0.002-0.005) -> it is the intervention days.
- Signal vs next-day intervention sign agreement ≈ 0.81 (DM), 0.87 (JY): rule is trading against the central bank.
- Removing top-10% GARCH volatility days does NOT kill the edge (DM Sharpe 1.004 after removal).
- 1981-84 non-intervention policy period: DM Sharpe −0.17 / 0.01, i.e. no edge.
- Rolling 1-yr Sharpe swings strongly over time (Fig 2) -> regime-dependent profitability.

## Relevance to T3 Lab
- "Price vs MA" edge, when it exists, is REGIME-specific and driven by a structural participant creating persistence; in futures
  equivalents: macro release days, central-bank days (FOMC/ECB), roll periods, session opens. Edge must be tested conditional on regime.
- Use unoptimised, conventional parameters first (LeBaron chose M=150/30 *a priori*): avoids snooping.
- Bootstrap random-walk null (scrambled returns) is a better significance test than t-tests for fat-tailed returns — apply per entry ID.
- Low turnover is why MA rules survived costs; the Lab's T3(3) is extremely fast -> high turnover -> costs dominate.
