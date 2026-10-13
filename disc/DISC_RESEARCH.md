# DISC Engine — Research Record

Everything checked on the original DISC engine and on futures research, and every decision taken for DISC v3. No real market data was used. Every measurement below comes from algebra, from a random-walk simulation, or from a synthetic CME-style 30-minute market built for testing. The synthetic market has sessions from 17:00 to 16:00 CT, an intraday volatility and volume U-shape, a bump at the European open, slowly varying daily volatility and a tick grid.

---

## 1. The original engine, element by element

### 1.1 The DISC score and tanh

**What it does.** `DISC = MID + 0.5·R·tanh(z)`, where `z` is a frozen 11-term linear score of candle-shape features. tanh is the inverse Fisher transform (Ehlers 2004). It maps any score into (−1, +1), so DISC always lies inside the bar. That idea is sound.

**What the algebra shows.** The 11 features are linearly dependent. Two identities hold on every bar:
- `upw + dnw + body = 1`
- `skew = pos − ½`

So the score has only three degrees of freedom: the close location `CLV = 2·pos − 1`, the body `b` and the direction `d`. Reduced exactly, with symbolic algebra:

| direction | z as a function of CLV and body |
|---|---|
| close > open | `1.0834·CLV + b(0.6184·CLV − 0.1837) − 0.0547` |
| close < open | `1.0180·CLV + b(0.6184·CLV + 0.1749) + 0.0249` |
| doji | `1.0507·CLV − 0.0149` |

**Decomposed against the mirror operation** (CLV → −CLV, d → −d):

- **Symmetric part (kept):** `1.0507·CLV + 0.6184·b·CLV − 0.1793·d·b − 0.0398·d`.
- **Asymmetric remainder (removed):** `−0.0149 + 0.0327·d·CLV − 0.0044·b`.

**Measured on 2,000,000 random bars.**
- The original DISC is **not** a mirror image between long and short. The error reaches 2.31% of the bar range.
- So a short candidate was never the exact mirror of the long one, which breaks the framework's own law that both sides are mirrors.

**The provenance of the coefficients is not documented** anywhere in the framework (only "the frozen 11-term score"). Coefficients written to six decimals on collinear features look like the output of a regression fit. If that fit used the campaign's own instruments, the engine carries information from the data it is later tested on.

**Decision (v3).** `zs = 1.05·CLV + 0.62·body·CLV − 0.18·dir·body − 0.04·dir`. These are the symmetric part of the original, rounded to two decimals.
- DISC v3 differs from the original by at most **1.17% of the bar range** (mean 0.44%), so the engine keeps its character.
- The mirror error is exactly 0.
- `|zs| ≤ 1.89`, so the ±20 clamp is unnecessary.
- A flat bar gives exactly MID with no special case.
- No new fitting was done.

### 1.2 The rolling statistics and z_in: the Samuelson ceiling

**What it does.** The original standardizes DISC against the mean and standard deviation of a window that **includes the current bar**.

**Defect.** By the Laguerre–Samuelson inequality, no point of a sample can lie more than √(n−1) population standard deviations from the sample mean. That is where the original's `ceiling = √(Q−1)` comes from. The bands are therefore squeezed against a mathematical wall at small Q.

Measured on the synthetic market (level 90, the 95th/5th percentile of z):

| Q | ceiling √(Q−1) | K_U (status) | K_L (status) |
|---|---|---|---|
| 3 | 1.41 | 1.40 CEILING | 1.40 CEILING |
| 4 | 1.73 | 1.64 SATURATED | 1.62 SATURATED |
| 5 | 2.00 | 1.75 SATURATED | 1.70 SATURATED |
| 6 | 2.24 | 1.84 SATURATED | 1.81 SATURATED |
| 8 | 2.65 | 1.92 OK | 1.89 OK |

A Q range starting at 3 is impossible with the original definition.

**Decision (v3): prior-window z.** `μ_t, σ_t` come from the Q bars **before** t, and `z_t = (DISC_t − μ_t)/σ_t`.
- **No ceiling.** The new bar is judged against a window that does not contain it. This is the prediction-interval statistic: for normal data, `(x − x̄)/s` of a new point follows a scaled t-distribution with n − 1 degrees of freedom.
- **The band is fixed before the bar prints.** Upper and Lower at bar t use bars up to t−1 only.
- **ddof doesn't matter.** Population or sample σ differ by a constant factor, so the empirical K absorbs it and the bands are identical.
- **Measured K at level 90:** Q=3 → 5.99/5.17, Q=5 → 3.57/3.39, Q=10 → 2.85/2.62, Q=50 → 2.49/2.17. These are heavy tails at small Q, as expected, and the K census absorbs them.
- **Calibration test.** The share of bars beyond each band divided by the level's tail is 0.94–1.07 at every Q from 3 to 100 and every level. The bands mean the same thing at every Q.

### 1.3 The K census

**What it does.** K is an empirical quantile of z over a trailing window that ends at t−1. This is adaptive and robust to fat tails, so it is kept.

**Defects.**
1. TradingView does not document the interpolation convention of `ta.percentile_linear_interpolation`. A TA-Lib page lists it in the Hyndman–Fan linear family, but the exact rank formula is not published, so Python–Pine parity rested on an assumption.
2. The window is a history-buffer read, so `max_bars_back = 5000` caps it. The original's own notes say levels 98–99 are unreachable on a chart.
3. K_WINDOW is fixed in bars whatever Q is. At large Q, z moves slowly and the tail counts come from a handful of independent excursions.

**Decision (v3).**
- K is the **exact type-7 quantile** (numpy "linear") of a **sorted rolling array** of the last N_K valid z values. It is maintained by binary-search insert and remove in Pine.
- There is no history-buffer limit.
- `N_K = max(⌈100/tail⌉, 20·Q)`: at least 100 values in each tail, and at least 20 Q-windows of history.
- Test: K equals `numpy.quantile` exactly (error 0.0).

### 1.4 The status machine (OK / THIN / SATURATED / CEILING)

**Decision:** removed. CEILING and SATURATED cannot occur with the prior-window z, and THIN is prevented by N_K. A band is ready when the census is full and K_U, K_L > 0.

### 1.5 The direction rule (mean_dir, upper_dir, v_long, accel and others)

**What it does.** A line is FLAT only when its smoothed change is below the 20th percentile of its own recent changes, so about 80% of bars are UP or DOWN by construction. The label never compares the market with a no-trend benchmark.

**Decision.** Replaced with one trend measure that has a null:

`TS_q = ER_q · √q`, where `ER_q = |C_t − C_{t−q}| / Σ|ΔC|` is Kaufman's efficiency ratio.

Under a random walk, ER shrinks like 1/√q, so TS is scale-free. Simulated with 20,000 random walks per q:

| q | p40 | p50 | p80 | p90 |
|---|---|---|---|---|
| 5 | 0.74 | 0.94 | 1.68 | 2.06 |
| 10 | 0.69 | 0.88 | 1.61 | 2.03 |
| 25 | 0.68 | 0.87 | 1.61 | 2.04 |
| 100 | 0.65 | 0.84 | 1.59 | 2.04 |
| 500 | 0.65 | 0.84 | 1.59 | 2.04 |

The large-q limit is `1.2533·|Z|` (median 0.85, p90 2.06).

**Thresholds, set from the null and never from data:**
- **Choppy:** TS ≤ 0.85, the random-walk median.
- **Trending:** TS ≥ 2.0, about the random-walk 90th percentile.

TS ≤ √q always, so a TS-trend condition needs q ≥ 5. TREND_DIP reads 4Q for that reason.

### 1.6 burst_state (CLIMAX): contaminated by the time of day

**What it does.** `burst = R / SMA(R, 8)` compares a bar with the previous 4 hours. At the cash open the previous 8 bars are overnight bars, so almost every opening bar looks like a climax.

Measured share labelled CLIMAX (top third; it should be about 33% at every time):

| slot | original burst_state | v3 slot-relative range |
|---|---|---|
| 08:30 cash open | **98.6%** | 31.8% |
| 09:00 | **93.4%** | 30.0% |
| 02:00 European open | 66.2% | 33.6% |
| 14:30 | 71.2% | 34.0% |
| 20:00 night | 34.8% | 31.2% |

The families `climax_exhaustion` and `climax_early` therefore measured the clock, not exhaustion.

**Decision.** Use slot-relative range and volume (intraday periodicity, Andersen & Bollerslev 1997):
- `rr = R / median(R at the same slot, previous 20 sessions)`
- `rv = V / median(V, same slot, previous 20 sessions)`

The original called rvol "research-side, not computable in Pine". It is computable with a map of slot arrays, and v3 does it on the chart.

### 1.7 CUSUM (cusum_event)

**What it does.** A CUSUM on `dz = z_in − z_in[1]` with threshold h = 1 standard deviation of dz.

**Measured:** **39.4 events per 100 bars**. That isn't a change detector. The López de Prado CUSUM filter (AFML 2018, Snippet 2.4) runs on price changes with a volatility threshold, and it is a *sampling* method, not an edge.

**Decision.** Dropped as a family.

### 1.8 Other elements

| element | verdict | reason |
|---|---|---|
| state / COIL, std_col, vol_pressure, vol_ratio | removed | read by no surviving family |
| v_short, accel, disc_v | removed | direction-rule elements (1.5) |
| ctx4, ctx16 | removed | read by no family; ctx16 unreachable in Pine above Q 156 (the original's own note) |
| AD line, rolling VWAP, flow_press | removed | AD starts at the chart's first bar (offset); continuous-contract volume; no family left that needs them |
| session_move, prev_session_move, rvol_open | removed | read by no family (the original's RULING 17) |
| Wilder ATR(14), hand-rolled | **kept** | exact definition; the risk unit |
| recovery machine, W(Q) = max(5, ⌈√Q⌉) | **kept** | the anti-falling-knife law; the expiry scales with Q |
| session clock | **kept, simplified** | cash open, settlement, European open, flat-by; no hard-coded bucket durations |

---

## 2. What the market research says (what still works, what does not)

| effect | evidence | recency | verdict |
|---|---|---|---|
| Bollinger-type band rules | Leung & Chong 2003: BB no better than envelopes. Lento et al. 2007: no excess return after costs; contrarian versions better | old | band **reversal** only with a mechanism; plain band touches are no edge |
| Liquidity-provision reversal | Campbell, Grossman & Wang 1993: high-volume price changes reverse. Llorente et al. 2002: reversal where trading is hedging (low information asymmetry, e.g. index futures). Nagel 2012: reversal returns rise sharply with volatility | classic, replicated | **use**: recovery after an excursion, strongest with a liquidity shock |
| Short-horizon autocorrelation in index futures | Bianco & Renò 2006 (FIB30): negative below 20 min (bid–ask bounce), **positive in high unexpected-volatility regimes**. SPX-futures follow-up (2009): driven by unexpected volatility only | 2006–2009 | **use**: shock continuation at a short hold; reversion only after the shock has turned |
| Half-hour reversal and daily continuation | Heston, Korajczyk & Sadka 2010 (stocks): negative at short lags, positive at daily multiples | 2010 | supports confirmation (enter on the return, not the first break) |
| Trend (TSMOM) | Moskowitz, Ooi & Pedersen 2012; Hurst, Ooi & Pedersen 2017. SG CTA +20.1% in 2022, about −3% in 2023, small gains in 2024–25. Kwon et al. 2020: weekly commodity momentum | to 2025 | **use** with a regime test; fragile |
| Intraday momentum (last half-hour) | Gao et al. 2018; Baltussen et al. 2021 (60+ futures, to 2020). Rosa 2022: out-of-sample predictability fades and **thresholds help**. Weaker in APAC (2023) | mixed after 2020 | **use** with a threshold only |
| End-of-day reversal | Baltussen, Da & Soebhag 2024/25: individual stocks reverse in the last 30 min | 2025 | **not** for index futures (the index shows momentum) |
| Overnight drift at the European open | Boyarchenko, Larsen & Whelan, RFS 2023: US equity returns are large and positive at the European open, strongly after US sell-offs (dealer inventory), weak after rallies | 2023 | **use** (new): engine-native timing |
| Stop-loss cascades | Osler 2005 (JIMF): trends accelerate when price crosses levels where stops cluster; take-profit levels reverse more often | 2005 | **use** (new): break of a failed excursion's extreme |
| Opening range / noise area | Holmberg et al. 2013; Zarattini & Aziz 2023 (a replication found gross reproduced, net ≈ 0); Zarattini, Aziz & Barbon 2024: noise-area breakout on SPY, 2007–2024 | to 2024 | **use**: diffusion-scaled break from the open |
| Volatility squeeze breakouts | no academic test found; practitioner claims only | — | **not used** |
| Candlestick / wick rejection | Marshall et al. 2008: none of 7,846 rules profitable after data-snooping control. Park & Irwin 2010: best rules significant in 2 of 17 futures | — | **not used** (`shape_rejection` dropped) |
| Systematic falsification warning | Mesfin 2026 (arXiv 2605.04004): 14 OHLCV intraday signal families on MNQ, 2021–25, walk-forward with costs: **none passed** | 2026 | the framework must expect STOP as a likely outcome |

---

## 3. The fifteen original families, judged

| # | original family | verdict | why |
|---|---|---|---|
| 3 | band_recovery | **kept, fixed** → BAND_RECOVERY | prior-window band; exits defined; the yardstick |
| 1 | ad_divergence_recovery | dropped | AD offset and volume source issues; replaced by the slot-relative shock |
| 2 | climax_exhaustion | **rebuilt** → SHOCK_RECOVERY | the CLIMAX label measured the clock (1.6); now slot-relative range AND volume (CGW 1993) |
| 4 | band_ride | **rebuilt** → BAND_RIDE | the direction rules are replaced by the TS trend test |
| 5 | failed_reclaim | **rebuilt** → STOP_CASCADE | now the break of the failed excursion's extreme, where stops sit (Osler 2005) |
| 6 | mean_pullback | **rebuilt** → TREND_DIP | a dip beyond the band against a 4Q trend, entered on the recovery |
| 7 | cusum_event | dropped | fires on 39% of bars (1.7) |
| 8 | vol_burst_break | **rebuilt** → SHOCK_IMPULSE | unexpected volatility → short continuation (Bianco & Renò) |
| 9 | surprise_break | dropped | z_fwd > z_in has no meaning once z is prior-window |
| 10 | range_fade | merged | QUIET_RECOVERY is the regime-tested form of fading |
| 11 | vwap_reversion | dropped | rolling Q-VWAP is a second band on the same idea; continuous-contract volume |
| 12 | shape_rejection | dropped | candlestick evidence negative |
| 13–15 | early-entry forms | dropped | fading the first close beyond the band enters before the reversal is confirmed; the bounce literature supports confirmation |

---

## 4. The ten v3 families (exact rules in DISC_FAMILIES.html)

| group | family | bet |
|---|---|---|
| Reversion | BAND_RECOVERY | an excursion beyond the band that returns inside within W(Q) reverts to the mean |
| Reversion | QUIET_RECOVERY | the same, only when the market is choppier than a random walk (TS ≤ 0.85) |
| Reversion | SHOCK_RECOVERY | the same, only when the excursion held a liquidity shock (range and volume ≥ 2× their slot median) |
| Trend | TREND_DIP | a recovery from a dip against a strong 4Q trend continues with the trend |
| Trend | BAND_RIDE | two closes beyond the band inside a strong Q trend continue |
| Trend | STOP_CASCADE | a failed recovery that breaks the excursion's extreme triggers a stop cascade |
| Trend | SHOCK_IMPULSE | a shock bar closing beyond the band continues for one hour |
| Session | OPEN_BREAK | the first day-session move from the open beyond its normal size for that time of day continues to the flat-by |
| Session | CLOSE_DRIFT | a move since the previous settlement beyond its normal size continues in the last half-hour |
| Session | EURO_OPEN_REVERSAL | after the previous session settled below its lower band, buy the European open |

The search axes are Q on a geometric grid from 3 to 500 (23 values) × levels 80, 90, 95 × both sides. Each family has one fixed exit; no family has a tuned constant.

Each family also names a refutation: the same entries with its key condition removed (DISC_FAMILIES.html, "Refuted if"). It runs once on Validation (P8), where the selection on Dev cannot inflate it. A family whose key condition adds nothing out of sample has not shown its mechanism, and it stops.

---

## 5. Verification (synthetic data only)

| test | result |
|---|---|
| Pine replica (bar-by-bar, TradingView order processing) == reference engine | **0 mismatches** in two runs on two synthetic markets: 12,173 + 12,015 signals and 10,893 + 10,753 trades (10 families × 4 (Q, level) × 2 sides) |
| no look-ahead: change every bar after a cut | no element and no signal at or before the cut changes |
| mirror: short on the market == long on the mirrored market | exact, all families |
| K census == numpy linear quantile | error 0.0 |
| band calibration, Q 3–100, levels 80/90/95 | beyond-band share / tail = 0.94–1.07 |
| pure noise, BAND_RECOVERY, 10 markets | pooled z = 1.5 (no false edge) |
| planted mean reversion | BAND_RECOVERY t = 3.2; BAND_RIDE does not profit from it |
| stop-fill optimism | with 4-step intrabar paths, stops filled at the stop price gave a false +0.1 ATR on noise; with 60-step paths it vanished. Stop fills at the stop price are optimistic when price jumps through the stop, so the framework stresses stop slippage |
| second engine (Backtrader, written independently) == reference engine | **0 mismatches**: 3,462 trades in 40 runs (10 families × 2 (Q, level) × 2 sides) |
| holiday market (early closes, missing OB and SB bars, a 3-hour halt, missing sessions): Pine replica == reference | **0 mismatches**: 10,386 signals and 9,362 trades. The first run found that the research engine closed trades at the last bar *present* before 15:08, which reads the next bar; the rule is now the Pine's: the last tradable bar is known from the clock and the exchange calendar |
| Hypothesis property tests | 3,000 random bar sets: DISC inside [low, high], exact mirror, flat bar = MID; 2,000 random samples: type-7 quantile == numpy |
| Pine v6 audit against the official reference | every name and every named argument exists in v6; no lookahead_on; history offsets ≤ 2,000 < max_bars_back 5,000. The prior-window mean and sd now use ta.sma and ta.stdev (population, as the manual defines them) and the slot RMS keeps a running sum, so no loop over Q runs on a bar (1,500 loop steps per bar at Q = 500 before) |

---

## 6. Limits

1. The Pine script could not be compiled in this environment. Its logic is verified through the bar-by-bar replica. Compile it in TradingView and run the P10 parity replay.
2. All measurements are synthetic. Nothing here says a family makes money; the framework decides that.
3. The provenance of the original 11 coefficients is unknown. v3 keeps their symmetric shape (within 1.2% of the bar range).
4. Recent independent tests of intraday OHLCV signals on index futures mostly fail after costs. A STOP is a likely and valid outcome.

---

## Sources

- Samuelson's inequality — https://en.wikipedia.org/wiki/Samuelson%27s_inequality
- TA-Lib, percentile methods (lists ta.percentile_linear_interpolation in the linear family) — https://ta-lib.org/functions/percentile.html
- Leung & Chong (2003), Bollinger Bands vs envelopes — https://www.aof.org.hk/uploads/conference_detail/952/con_paper_0_72_3_chong_bollinger1.pdf
- Lento, Gradojevic & Wright (2007) — https://ideas.repec.org/a/taf/raflxx/v3y2007i4p263-267.html
- Campbell, Grossman & Wang (1993), QJE — https://campbell.scholars.harvard.edu/publications/trading-volume-and-serial-correlation-stock-returns
- Llorente, Michaely, Saar & Wang (2002), RFS — https://www.johnhcochrane.com/s/llorente_michaely_saar_wang_volume_rfs.pdf
- Nagel (2012), Evaporating Liquidity, RFS — https://www.nber.org/papers/w17653
- Bianco & Renò (2006), JFM — https://ideas.repec.org/a/wly/jfutmk/v26y2006i1p61-84.html ; unexpected volatility follow-up — https://arxiv.org/abs/physics/0610023
- Heston, Korajczyk & Sadka (2010), JF — https://www.bauer.uh.edu/departments/finance/documents/Heston_Korajczyk_Sadka_paper_UH.pdf
- Gao, Han, Li & Zhou (2018); Baltussen, Da, Lammers & Martens (2021) — https://www3.nd.edu/~zda/intramom.pdf
- Rosa (2022), JFM, intraday momentum out of sample — https://ideas.repec.org/a/wly/jfutmk/v42y2022i12p2218-2234.html
- Intraday momentum in APAC (2023) — https://ideas.repec.org/a/eee/pacfin/v80y2023ics0927538x2300152x.html
- Baltussen, Da & Soebhag, End-of-Day Reversal — https://www3.nd.edu/~zda/EOD.pdf
- Boyarchenko, Larsen & Whelan (2023), The Overnight Drift, RFS — https://ideas.repec.org/a/oup/rfinst/v36y2023i9p3502-3547..html
- Osler (2005), Stop-loss orders and price cascades, JIMF — https://www.newyorkfed.org/medialibrary/media/research/staff_reports/sr150.pdf
- Moskowitz, Ooi & Pedersen (2012); CTA performance 2022–2025 — https://www.hedgeweek.com/ctas-struggling-to-repeat-2022s-success/
- Kwon, Kang & Yun (2020), weekly momentum in commodity futures — https://strathprints.strath.ac.uk/70084
- Andersen & Bollerslev (1997), intraday periodicity — https://scholars.duke.edu/publication/761297
- Kaufman efficiency ratio — https://ta-lib.org/functions/er.html
- López de Prado (2018) CUSUM filter (implementation reference) — https://mlfinpy.readthedocs.io/en/latest/Filtering.html
- Ehlers inverse Fisher transform (tanh) — https://www.luxalgo.com/library/concept/inverse-fisher-transform/
- Lo & MacKinlay variance ratio — https://metricgate.com/docs/lo-mackinlay-variance-ratio
- Zarattini, Aziz & Barbon (2024), noise area — (Swiss Finance Institute RP 24-97)
- Mesfin (2026), falsification of OHLCV intraday signals on MNQ — https://arxiv.org/pdf/2605.04004
- Park & Irwin (2007, 2010); Marshall, Cahan & Cahan (2008) — summarized at https://www.luxalgo.com/library/concept/pattern-failure-statistics.md
