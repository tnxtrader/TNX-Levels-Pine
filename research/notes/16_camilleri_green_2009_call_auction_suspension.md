# Camilleri & Green (2009) — The impact of the suspension of opening and closing call auctions: NSE India.
Int. J. Banking, Accounting & Finance 1(3):257-284. 47 pp (25 text + tables/figs). Read in full; formulas checked on page renders.

## Hypothesis
If call auctions aid price discovery, removing them (NSE, 9-Jun-1999, no other rule change = clean natural experiment) should worsen
volatility, efficiency and liquidity (VEL). Alternative: auctions cost immediacy, so VEL improves.

## Formulas (all OHLC/volume computable)
(1) Scaled intraday price difference  SIDP: D_{i,t} = (P_high − P_low) / P_open
(2) Overnight return reversal: r_{i,t} = a_i + π_i r^O_{i,t} + e  (π < 0 = overnight move reversed intraday = excess volatility)
(3) Relative return dispersion: RRD_t = Σ_i ε²_{i,t} / n  (ε from market model; lower = more efficient)
    Lag-1 autocorrelation of returns (daily, and 1-min returns in first trading hour) as inefficiency measure
    Volume-per-unit-return = Volume / |r|  (resiliency); volume trend regression with shift/trend dummies
EGARCH on index for news asymmetry ((α+γ) good news, (α−γ) bad news)
Event study: r'_{i,t} = α_i + β_i r'_{m,t} + ε (calendar-adjusted), AR = r − α̂ − β̂ r_m, CAR = ΣAR, window T−2..T+15, est. T−62..T−3;
non-normal residuals -> bootstrap (5,000 draws without replacement) on rescaled SSR.

## Results
- After suspension: SIDP intraday volatility DOWN significantly, but |π| overnight reversal UP (closing auction had helped overnight).
- RRD down (more efficient); first-hour 1-min autocorrelation dropped (auction did NOT help opening discovery);
  daily autocorrelation flipped from mostly negative to mostly positive.
- Volume and volume/|r| up. CARs significant but mixed (dip to T+9 then rise); illiquid stocks gained most (thin auctions useless).
- The last closing auction (8-Jun) was anomalous (software issue): Nifty +16 pts in the auction fully reversed next day.

## Relevance to T3 Lab (futures, OHLC)
- Session open is a distinct microstructure state: CME equity/energy futures open via a pre-open + opening match; opening minutes
  carry overnight information aggregation, wider spreads and higher volatility -> T3 crosses in the first bars of RTH are different
  animals. Condition / test entries by time-of-day; the Lab currently has NO session filter.
- Overnight-gap reversal coefficient π is directly estimable from OHLC: regress RTH open->close return on prior close->open gap.
  If π < 0 on your symbol, "GapUp/GapDown" confirmations (IDs 15,24,28,30,32,43,65) are buying gaps that statistically fade.
- Sign of lag-1 autocorrelation is a regime property that can flip with market structure -> measure it, don't assume it.
- Thin/illiquid states (overnight Globex, holidays) behave differently -> separate RTH vs ETH statistics.
