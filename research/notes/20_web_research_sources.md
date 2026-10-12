# Web research (2026-10-12) — sources verified via web search; formulas cross-checked with code where possible
Container network: only PyPI + raw.githubusercontent.com reachable; web search tool used for literature lookup.

## T3 (Tillson, "Better Moving Averages", TASC Nov-1998)
GD(n,v) = EMA·(1+v) − EMA(EMA)·v  (v=0 -> EMA, v=1 -> DEMA); T3 = GD(GD(GD)) = six-pole filter; Tillson liked v = 0.7
("cures the multiple DEMA overshoot problem at the cost of additional phase delay"). T3 overshoots on sharp reversals because the lag
correction extrapolates slope. Sources: tradingview.com/scripts/tilsont3, luxalgo.com/library/concept/t3
## Deflated Sharpe Ratio / PSR (Bailey & López de Prado 2012, 2014 JPM 40(5):94-107, SSRN 2460551)
PSR(SR*) = Φ[(ŜR − SR*)·sqrt(T−1) / sqrt(1 − γ̂3·ŜR + (γ̂4−1)/4·ŜR²)]   (γ4 raw kurtosis; verified in quantstats/stats.py source)
DSR = PSR(SR0) with SR0 = sqrt(Var[{ŜR_n}])·((1−γ)Φ⁻¹[1−1/N] + γΦ⁻¹[1−1/(N·e)])
MinTRL = 1 + (1 − γ3 SR + (γ4−1)/4 SR²)·(Z_α/(ŜR − SR*))²
## PBO via CSCV (Bailey, Borwein, López de Prado, Zhu, J. Computational Finance 20(4), SSRN 2326253; R package 'pbo')
Matrix M (T×N P&L by config) -> split rows into S even blocks -> all C(S,S/2) train/test combos -> IS best config n* ->
OOS relative rank ω̄ ∈ (0,1) -> λ = ln(ω̄/(1−ω̄)) -> PBO = P(λ ≤ 0). Also performance degradation (regress OOS on IS SR), P(OOS loss).
## Stop-losses: Kaminski & Lo (2014) J. Financial Markets 18:234-254 — under a random walk, 0/1 stop-loss rules ALWAYS reduce expected
return; with momentum (positive serial correlation) stops can add value. (dspace.mit.edu/handle/1721.1/114876)
## Intraday futures/ETF evidence
- Gao, Han, Li, Zhou (2018) JFE "Market intraday momentum": first half-hour return (incl. overnight) predicts last half-hour, SPY 1993-2013;
  stronger on volatile, high-volume, recession and macro-news days.
- Zarattini, Aziz, Barbon (2024/25, SFI WP 24-97) "Beat the Market": SPY intraday momentum with a 14-day "Noise Area" (avg intraday move
  from open) and dynamic trailing stops; 2007-2024 net Sharpe 1.33 (author-reported).
- Heston, Korajczyk, Sadka (2010) J. Finance 65(4): short-run (<1h) reversal from bid-ask bounce/liquidity imbalance; continuation at
  exact daily lags (same half-hour on subsequent days) for 40 days.
- Index futures sub-20-minute returns mostly NEGATIVELY autocorrelated (bid-ask bounce) — FIB30 study; futures autocorr near zero vs
  positive for cash indices (Ahn, Boudoukh, Richardson, Whitelaw 1999 NBER).
- Marshall, Cahan & Cahan (2008): 7,846 intraday rules (filter, MA, S/R, channel, OBV) on 5-min SPY 2002-2003; bootstrap; after
  data-snooping correction intraday TA showed no value (raw: 42 rules sig. at 1%, 163 at 5%).
## Multiple testing
- White (2000) Reality Check; Hansen (2005) SPA (more power, robust to many poor rules); Sullivan-Timmermann-White (1999): best of 7,846 rules
  insignificant out-of-sample on DJIA 1987-1996.
- Harvey, Liu & Zhu (2016 RFS 29(1):5-68): new factors need t > 3.0; haircut Sharpe via t-ratio adjustment (Bonferroni/Holm/BHY).
## Trend following in futures
- Moskowitz, Ooi, Pedersen (2012) JFE: 12-month TSMOM positive for all 58 futures; vol-scaled positions (divide by lagged σ).
- Brock, Lakonishok, LeBaron (1992) J. Finance: MA and range-break rules, DJIA 1897-1986, bootstrap vs RW/AR1/GARCH-M/EGARCH nulls.
## Volatility from OHLC (Parkinson 1980; Garman-Klass 1980; Rogers-Satchell 1991; Yang-Zhang 2000 J. Business 73(3):477-491)
Efficiency vs close-to-close ≈ Parkinson 5.2, GK 7.4, RS 8.0, YZ 14 (textbook table). RS is drift-independent; YZ handles overnight gaps.
## Regime tests
- Lo & MacKinlay (1988) variance ratio VR(q) = Var(r^q)/(q·Var(r)); Z(q) = (VR−1)/sqrt(2(2q−1)(q−1)/(3qn)); robust Z* with θ̂(q).
- Kaufman efficiency ratio ER = |C_t − C_{t−n}| / Σ|C_k − C_{k−1}| (0..1).
## Order-flow proxy from bars: Bulk Volume Classification (Easley, López de Prado, O'Hara 2012; JFE 2016 "Discerning information from trade
data"): V_buy = V·t_ν(ΔP/σ_ΔP), V_sell = V − V_buy; accuracy ~92% on 5-min bars, ~51% on 1-sec bars (Massot et al. 2018).
## Filters: Ehlers SuperSmoother (2-pole Butterworth) attenuates noise more than EMA at similar delay (mesasoftware.com UltimateSmoother.pdf).
## TradingView mechanics (official docs)
- Broker emulator infers intrabar path: open nearer high -> O→H→L→C, else O→L→H→C; assumes no intrabar gaps; Bar Magnifier uses lower-TF
  intrabars (Premium). Price crossing an order level in the gap between bars fills at the next bar's open.
- Continuous futures back-adjustment OFF by default; enable with B-ADJ button / "Adjust for contract changes"
  (tradingview.com/support/solutions/43000685266). Example roll gap ESZ2023->ESH2024 ≈ 51 points.
