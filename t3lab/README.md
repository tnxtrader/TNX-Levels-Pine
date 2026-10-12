# T3 Entry Lab: research framework

This framework tests all 5000 registry candidates on your 30-min CL / GC / NQ / SI / YM data. Each candidate is tested on each instrument separately (one **unit** = candidate × instrument, so 25 000 units).

There are 7 phases. Every phase runs its steps, applies gates and writes `out/<P>/report.md`. Each report has the same five sections: what the phase did, what it tried, what it changed, what it found, and the gates. Each phase also writes `gates.json`, PNG plots and CSV tables.

A phase will not start unless the phase before it ended with **PASS**. If a phase ends with **STOP**, the pipeline stops there. That is a valid result: no edge survived the gates.

## Files

| file | what it is |
|---|---|
| `../T3_Entry_Lab_v2.pine` | Fixed and optimised Pine script. It uses the same trigger, filter and exit rules as the Python code. |
| `config.yaml` | Every setting and every gate threshold. Edit it **before** running P2, never after seeing results. |
| `run.py` | Runner. `python run.py P0` … `P6`, `all` (every phase in order) or `demo` (synthetic data). |
| `t3lab/core.py` | Data loading, CME sessions, T3 / ATR, the 32 triggers + 28 filters (executable definitions) and the registry fix. |
| `t3lab/engine.py` | Numba bar engine. Entry at the next open; SL / TP / time / session-end exits; costs; MFE / MAE. |
| `t3lab/stats.py` | Newey-West t, stationary bootstrap, Benjamini-Hochberg, K_eff, PSR / DSR, PBO (CSCV), Hansen SPA. |
| `t3lab/phases.py` | P0 … P6. |
| `t3lab/selftest.py`, `tests/` | 14 engine and signal tests on synthetic data (`python -m pytest tests`). |
| `registry/` | Your raw registry and `candidate_registry_fixed.csv`. |

Libraries: numpy, pandas, numba, scipy, arch (bootstrap, SPA), optuna (optional search), matplotlib.

## Run it (for you or Antigravity)

```bash
cd t3lab
pip install -r requirements.txt
python -m pytest tests          # must show 14 passed
python run.py demo              # optional: whole pipeline on synthetic random data (no edge exists, P6 must STOP)
# copy the 5 CSVs into t3lab/data/ (names as in config.yaml; *SYM_30min*.csv also works)
python run.py P0                # then read out/P0/report.md
python run.py P1                # ... read each report before running the next phase
python run.py P2
python run.py P3
python run.py P4
python run.py P5
python run.py P6                # opens the sealed test ONCE
```

Instructions for an agent:
- Run the phases in order.
- Do not edit `config.yaml` or the code after P2 has run.
- Never use `--force`.
- Never copy test-period numbers back into any setting.

Runtime has not been measured on your data. A rough estimate for 212k bars per instrument is minutes per phase; P2, the 25 000 units, is the longest.

## The phases

The split is chronological by session, done separately for each instrument:
- **Dev**: the first 60%, cut into 6 blocks.
- **Val**: the next 20%.
- **Test**: the last 20%.
- There is a 1-session embargo between segments.

Trades never cross a session end or a contract roll, so no further purging is needed.

| phase | data used | steps | hard gates | plots |
|---|---|---|---|---|
| **P0 Data** | all (QA only) | load, de-duplicate, repair OHLC, build sessions (17:00 CT, gap rule), flag rolls, split, hash the test segment | OHLC repairs ≤ 0.1%; timestamps on the 30-min grid ≥ 99%; prices on the tick grid ≥ 99%; ≥ 250 sessions per segment | price with the split shaded; session lengths; bar-hour histogram (timezone check) |
| **P1 Registry + engine** | Dev bars, no returns | 14 self-tests; rebuild the registry with executable definitions, spec hash and flags; trigger overlap | all tests pass; 0 unknown rules; unique specs | registry composition; trigger Jaccard overlap |
| **P2 Screen** | Dev | all 25 000 units with ONE fixed exit (time exit at 8 bars, full costs); per instrument K_eff, Sharpe variance, Hansen SPA, PBO | per unit: ≥ 100 trades; mean > 0; Newey-West t ≥ 2; BH q ≤ 0.10 over all 25 000 units; ≥ 4 of 6 positive blocks; median MFE > median MAE. Then drop overlapping entries (Jaccard > 0.30) and keep the best 50 | t histogram vs N(0,1); volcano; family heatmap; MFE / MAE; horizon profile |
| **P3 Refine** | Dev, walk-forward | 5 expanding folds. Stage A tunes T3 length × vf (T3 units only). Stage B tunes SL (ATR) × TP (R) × max bars = 252. Objective = 25th percentile of the block means. Choices are plateau-smoothed. The final parameters are chosen on all of Dev | ≥ 50 WF-OOS trades; WF-OOS mean > 0 | exit surfaces (SL × TP); OOS result of each fold |
| **P4 Evidence** | Dev (WF-OOS) | stationary bootstrap CI; DSR with N = K_eff + configs tried; PBO; SPA; circular-shift null; robustness; stability | CI low > 0; DSR ≥ 0.95; null p ≤ 0.05; PBO ≤ 0.40; universe SPA p ≤ 0.10; ≥ 70% positive folds; no fold > 50% of profit; still > 0 at cost × 1.5, +1 tick per side and +1 bar delay; neighbour median > 0; > 0 without the top 5% of trades and without the best year; ≥ 60% positive years; no year / vol-regime / session bucket (≥ 30 trades) significantly negative | WF-OOS equity + CI forest; gate matrix; null histograms |
| **P5 Validation** | Val, once | frozen parameters | ≥ 30 trades; mean > 0; BH q ≤ 0.05 across survivors; null p ≤ 0.05; > 0 at cost × 1.5 | Dev → Val equity; CI forest |
| **P6 Sealed test** | Test, once (lock file) | frozen parameters; deployment cards (Pine inputs) | ≥ 30 trades; 95% CI low > 0; DSR ≥ 0.95 (N = units tested); > 0 at cost × 1.5 | full-history equity; test CI forest |

After P6, run each deployment card in `T3_Entry_Lab_v2.pine`.
- Paper or sim trade it for **at least 30 trades** (incubation).
- Stop it if live mean R falls below the test CI lower bound, or if drawdown exceeds the worst historical drawdown.

## What is trained where

| decision | made on | never on |
|---|---|---|
| which candidates are worth refining | Dev, with one fixed exit (P2) | Val, Test |
| T3 length / vf, SL, TP, max bars | Dev train blocks only, one fold at a time (P3) | the fold being scored, Val, Test |
| keep or reject | Dev WF-OOS (P4), then Val (P5), then Test (P6), each opened once | — |

**Your baseline** is not another strategy. It is three things:
- **zero**: no trade, scored per session;
- **the same signals at random times**: the circular-shift null;
- **the best of the whole universe by luck**: SPA, DSR with K_eff, BH.

## Anti-overfitting rules (built in)

1. **One test per question.** Val is opened once and Test is opened once. P6 writes a `LOCK` file and refuses to run on a different unit set.
2. **Count every trial.** All 25 000 units and every P3 config count:
   - Multiple testing is handled by BH, DSR with N = K_eff + configs, the universe SPA and PBO.
   - A t of 2 among 25 000 units means nothing. About 570 units would reach it by luck.
3. **Screen entries with a fixed exit; tune exits only on the shortlist.** Tuning 252 exits for 25 000 units would be 6.3 M trials.
4. **Plateaus, not peaks.** The objective is the 25th percentile of the blocks, not the mean. The grid is smoothed over neighbours, and P4 requires the neighbouring parameters to work too.
5. **Costs always on.** 3 ticks round trip plus 1 tick on stops (edit them per broker). P4 also tests cost × 1.5, +1 tick per side and +1 bar delay.
6. **Conservative fills.**
   - Entry at the next open.
   - Stop wins when SL and TP are both inside one bar.
   - A gap through a level fills at the open.
   - No trade across a session end or a roll.
7. **Units are ATR and ticks, never %.** Back-adjusted CL goes negative, so percentage returns are wrong.
8. **Per-session statistics with zeros on no-trade sessions.** This avoids the overlap and irregular spacing of per-trade Sharpe.
9. **Robust across time.** Folds, years, volatility regimes and session buckets are all checked. One great year cannot carry a unit.

**What to avoid:**
- Re-running P2–P6 after changing a setting because of what you saw (that is p-hacking).
- Picking the best Test result.
- Adding filters to "fix" a losing period.
- Tuning on Val.
- Optimising PF or win rate instead of net R.
- Using the in-sample (all-Dev) equity as evidence.
- Treating SL / TP grid peaks as real.
- Ignoring the "pure luck would give ~N" line in P2.

**ML / Optuna:** grid search is the default.
- `p3_refine.search: optuna` uses Optuna's TPE sampler on the joint space (200 trials, the PDF's budget). Its trials are counted like grid trials.
- A meta-label ML filter is **not** included: with OHLC-only features and a few hundred trades per unit it adds more freedom than evidence.
- Only consider ML for units that pass P6, using the same walk-forward and counting it as new trials.

## Registry fix (`registry/candidate_registry_fixed.csv`)

Problems found in the 5000 rows:
- **Not 5000 independent ideas.**
  - They are 1516 distinct trigger × filter × side cells.
  - 770 cells appear several times, up to 22 times, only with different random T3 length / vf draws (2–30 / 0.5–1.0).
  - 146 T3 rows have no other T3 setting in their cell.
  - 600 rows do not use T3 at all.
- **Definitions were English only.**
  - `rule_hash` cannot be reproduced.
  - Terms such as "compresses", "directional progress" and "excursion" had no numbers.
- **Overlaps and contradictions.**
  - DONCHIAN_3 ≈ PREVBAR_BREAK (173 rows).
  - GAP trigger + GAP filter (redundant).
  - RANGE_EXPANSION_CLOSE + RANGE_EXPAND or CLV filters (redundant).
  - RANGE_EXPANSION + RANGE_COMPRESS (contradictory, 2 rows).
  - The nested thresholds (NEAR 025 ⊂ 050 ⊂ 100, CLV, BODY, SLOPE) give near-duplicate signals.

Fixes:
- `trigger_def` / `filter_def` hold the exact formulas, the same ones the Pine script uses.
- `spec_hash` is a sha1 of the executable spec.
- `cell_id` is the identity of the idea. T3 length / vf became a **tuned axis in P3** instead of a frozen random draw.
- `registry_flags` marks the overlaps.
- P2 removes duplicate entries by signal overlap, not by name.
- All 5000 rows are still tested, as you asked.

Operational choices made where the registry text was vague (all with `atr1` = ATR(14) of the previous bar):
- **COMPRESSION_BREAK_N**: prior N-bar range ≤ 0.75·atr1·√N.
- **MOMENTUM_N**: c − c[N] ≥ 0.5·atr1·√N.
- **GAP_CONTINUATION**: gap ≥ 0.05·atr1.
- **MEAN_REVERSION_k**: low reaches SMA20[1] − k·atr1, with a bullish close in the upper half.
- **SWEEP_RECLAIM_N**: level = lowest low of bars 2…N+1.

## Pine parity

Set the same trigger, filter, side, T3, ATR, SL, TP and max bars, and the date window from `out/P0/splits.csv`. The script then reproduces the Python trades, with these known differences:
1. **Intrabar order.** TradingView decides with its O-H-L-C path, while Python defaults to pessimistic. Set `intrabar: tv` to match exactly, or enable the bar magnifier.
2. **Rolls.** A continuous chart has no roll information, so Python's "no trade across a roll" cannot apply.
3. **EMA / ATR seed.** Values differ only during the first few hundred bars.
4. **Rounding.** Pine rounds .5 away from zero; numpy rounds to even.
5. **Costs.** Set the commission and slippage in the strategy Properties to the same ticks as `config.yaml`.

## Claims in the PDF / readme that were checked

| claim | verdict |
|---|---|
| 60/20/20 chronological split, 5 WF folds in Dev, 8-bar primary endpoint, 100-event floor, 25th-percentile objective, 252 exit grid, BH 0.10 / 0.05, PBO bands, DSR > 0.95, 2 000 / 10 000 bootstrap reps, (1+#)/(B+1) p-values, cost × 1.5 / × 2, +1 / +2 ticks, +1 bar delay, CI low > 0 | **kept** (standard practice: Bailey & López de Prado, Hansen, Politis-Romano) |
| T3 length 3 as a fixed baseline | **changed**: T3 length / vf is tuned in P3 and checked with neighbours in P4 |
| per-trade Sharpe / t | **changed** to per-session series with zeros (overlap-safe), with Newey-West t |
| DSR with N = number of rows | **changed** to N = K_eff (clustered effective trials) + configs tried. Raw N overstates independence; ignoring N understates it |
| random-entry null | **changed** to a circular session shift of the real signal. It keeps clustering and time of day and breaks only the timing link to future prices |
| missing in the PDF | **added**: Hansen SPA over the universe, session and roll handling, pessimistic fills, sealed-test lock, plateau smoothing, regime / session stability |
