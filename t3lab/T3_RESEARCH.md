# T3 Program 3 — Research Record

The evidence behind `FRAMEWORK.html` (Program 3: 10,046 candidates in 21 families, five futures markets, 30-minute bars).
The framework holds the rules. This file holds what the research found, what it did not test, and what our own
earlier programs measured. Reference numbers [R..] are those of the framework's Part D.

---

## 1. What matters on 30-minute futures

| finding | evidence | how the framework uses it |
|---|---|---|
| Costs decide almost everything at short holds | Program 1: micro costs were 0.18–0.25 ATR per trade at an 8-bar hold, more than almost every gross excess measured; most anomalies with high turnover do not survive costs [R34] | Stage I proves the edge before costs, then P5 prices it on micro AND mini (the mini costs 35–38% fewer ticks); Pool N adds DAY holds and day-session entries, where the move is large against a fixed cost |
| Most "discoveries" are luck from many trials | Program 1: member-level BH over 25,000 units found nothing, with fewer gross t ≥ 2 than chance (272 against about 569); a new factor needs t > 3 [R43]; the best of many trials looks good by luck [R2, R3, R6a, R6b] | families are tested first (P2), members second (P3, Benjamini–Bogomolov); every trial is counted in the ledger; the selection is repeated inside every randomization (N1-S) |
| The trading clock carries the strongest documented intraday effects | the open (opening range), the last half-hour (intraday momentum), the overnight-to-day handover (gap), the same half-hour on following days (slot continuation) [R19, R24–R27, R36–R39, R46, R47] | Pool N is built on session anchors: OB, IB, SB, O_day, S_prev, VWAP, σ_τ |
| Effects depend on the volatility state | opening-range returns are larger in high-volatility states [R37]; intraday momentum is stronger on volatile and large-move days [R24] | VOL_HIGH / VOL_NOT_LOW market contexts; IMOM_LARGE |
| Intraday volatility has a strong time-of-day shape | the U-shaped intraday volatility pattern must be removed before comparing bars [R28] | σ_τ and the SLOT z compare each bar with the same time slot of earlier sessions; the drift baseline is built by bar time |
| Recent independent tests fail more often than not | 14 OHLCV intraday signal families on MNQ, 2021–2025, walk-forward with costs: none passed (Mesfin 2026, arXiv 2605.04004) | STOP is a valid and likely outcome; every gate is hard |

---

## 2. Each family: what the source found, and what is new here

### Pool N (new session-anchored logic)

| family | what the source found | what the sources did not test, and is tested here |
|---|---|---|
| OPENING_RANGE ★ | Crabel's opening-range breakout [R19]; ORB profitable on crude-oil futures [R26]; 5-minute ORB on QQQ [R27]; larger returns in high-volatility states [R37] | 30-minute opening bars on NQ, YM, CL, GC, SI, with T3 and market contexts |
| INTRADAY_MOMENTUM ★ | the first half-hour return predicts the last half-hour on the S&P 500 ETF, more on volatile days [R24]; the rest-of-day return predicts the last 30 minutes in more than 60 futures, through hedging demand [R25] | the effect at each market's own settlement time (CL 13:30, GC 12:30, SI 12:25, NQ and YM 15:00 CT) |
| NOISE_BOUNDARY | moves beyond the average absolute move from the open at that time of day (14 days) carry through the day on SPY, 2007–2024 [R36] | the same noise area on futures, from max(O_day, S_prev) |
| VOLATILITY_BREAKOUT | a move from the open larger than a fraction of the previous day's range starts an expansion [R19]; state dependence [R37] | 0.25 and 0.50 of R_prev on futures |
| OVERNIGHT_GAP | overnight and day-session returns behave differently [R38, R39]; buying at the open after attention-grabbing nights is costly [R46] | whether the first day-session bar confirms or rejects the overnight gap |
| VWAP_TREND | VWAP is the execution benchmark of institutional orders [R35]; VWAP-based day-trading rules [R44] | acceptance above the session VWAP from OB, on futures |
| SAME_SLOT_CONTINUATION | half-hour stock returns continue at the same half-hour over the following days and weeks [R47] | untested on futures: a new hypothesis, measured as excess over the slot's Dev mean so the fixed seasonal part is removed |
| COMPRESSION_DAY | narrow-range days (NR4, NR7) are followed by expansion days [R19] | breaks of the previous day's high after NR4/NR7 days |
| PRIOR_DAY_LEVELS | the previous session's extremes are the most watched day-trading levels [R19] | first break of H_prev, reclaim of L_prev |
| T3_DAY_TREND ★ | T3 smoothing [R17]; Program 1's largest gross statistics came from T3 pullback continuation on YM, CL, GC (none significant after multiplicity) | the same continuation restricted to the day session |
| EXTREME_REVERSION ★ | overextension followed by a reversal bar [R11]; Program 1's mean reversion with a strong T3 slope on GC (not significant after multiplicity) | the day-session form, against VWAP and SMA20 |

### Pool L (legacy families from Programs 1 and 2)

| family | evidence |
|---|---|
| CANDLE_REVERSAL | short-horizon reversal after one-sided flow [R51]; candlestick strategies did not add value on liquid stocks [R49] — kept only as a legacy test |
| COMPRESSION_EXPANSION | range contraction precedes expansion [R19] |
| MOMENTUM_CONTINUATION | gradual diffusion of information and order flow; intraday momentum [R24]; time-series momentum at longer horizons [R40] |
| STRUCTURE_BREAKOUT | trading-range breakouts beat bootstrapped null models on a century of the Dow [R50]; channel breakouts of trend followers [R13] |
| STRUCTURE_RECLAIM | stop-loss orders cluster beyond recent extremes; when hit they accelerate the move, and the market then absorbs it [R48] |
| T3_CROSS_RECLAIM, T3_PULLBACK_CONTINUATION, T3_TURN_DISTANCE | T3 as a low-lag trend line [R17]; moving-average rules [R50] |
| VOLATILITY_MEAN_REVERSION | short-term reversals of extreme moves [R51]; randomization tests of reversion rules [R11] |
| T3_ACTIVE_SESSION | volume and information concentrate after the cash open [R28] |
| OPENING_RANGE, INTRADAY_MOMENTUM | as Pool N |

### Why every family carries a "refuted if" line

A family can pass P1–P7 for a reason other than its stated mechanism, for example by riding the time of day. Each
family therefore names the same entries with its key condition removed (FRAMEWORK.html C.2, C.7). P8 runs that
comparison once on Validation, so the selection on Dev cannot inflate it. If the key condition adds nothing out of
sample, the stated mechanism is refuted, and the strategy stops.

---

## 3. Why the statistics are the ones in A.6

| question | method | why this one |
|---|---|---|
| Is a member's excess real under serial dependence? | Newey–West t of the per-session excess [R7, R8] | 30-minute trades overlap sessions and cluster |
| Does a family as a whole have an edge? | Simes p (a few strong members) and pooled p (many weak members) [R30] | one test per family, so thousands of weak members cannot drown a real family |
| Which members carry it? | Benjamini–Bogomolov selective inference [R31]; StepM reported [R32] | controls false discoveries after the family selection |
| Is the best member luck? | selection-aware permutation (N1-S) [R12, R33]; DSR with the ledger N [R2]; PBO [R3]; CPCV [R10] | the whole search is repeated inside each randomization |
| Is the edge from the signal, not the clock? | random entry (N2) and circular shift (N3) | keeps bar time, side and hold, moves only the signal |
| How long a record is needed? | MinTRL [R1, R15] | the Validation and Test windows must be long enough to judge |

---

## 4. Earlier programs on the same Dev data

| program | what it tested | what it found | how Program 3 uses it |
|---|---|---|---|
| Program 1 | 32 triggers × 28 filters × side with a T3 setting, 8-bar hold, 5 markets | nothing survived multiplicity; costs larger than the gross edges | its trials stay in the ledger; Pool L keeps every candidate; ★ marks Dev-informed ideas |
| Program 2 | opening range and intraday momentum at one specification each | 7 of 230 beat chance against about 5 expected | widened into families with thresholds, volatility states and T3 contexts, flagged ★ |
| Both | — | Validation and Test were never opened | both remain clean exams |

---

## 5. Limits

1. Nothing here says any family makes money. The framework decides that, and STOP is a valid outcome.
2. Several sources study stocks or ETFs (R24, R27, R36, R38, R39, R46, R47). Their effects on futures are hypotheses.
3. The engine and the Pine script are verified on synthetic markets (see TOOLS_RESEARCH.md). The real-data parity is P10.
