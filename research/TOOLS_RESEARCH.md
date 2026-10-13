# Tools Research — how the tools were used on T3 and DISC

The AI and quant tools were used to **audit and fix** both projects: the frameworks, the families, the Pine scripts, the
logic and the formulas. They are not part of the frameworks. An agent builds each project from its framework alone.

---

## 1. In plain words

| tool | what it did here | result |
|---|---|---|
| **LangGraph** 1.2.15 | ran the audit as a graph: a research library, then five audits in parallel (Pine, family specs, constants, safeguards, engine tests), a human gate (interrupt and resume) and a report; every run is checkpointed in SQLite | two rounds: round 1 found the gaps below, round 2 checked the fixes |
| **LlamaIndex** 0.14.25 (BM25 retriever) | a searchable library of the official Pine v6 manual and reference (2,705 passages), both frameworks, the family pages and the research records; every finding cites the passage it rests on | the Pine limits, ta.stdev's definition, math.round's rounding and the built-in advice were all read from the manual itself |
| **Pydantic** (the model layer of PydanticAI 2.55) | every family checked against a 12-field specification (mechanism, entry, exit, parameters, timeframe, data, risk, refutation, experiment plan, sources); every finding is a typed record | round 1: no family of either project had a refutation rule; T3's 10 legacy families had no mechanism or sources |
| **Pine v6 reference** (pinescript-mcp 0.7.5) | every function, variable, constant, method and named argument of both scripts checked against the v6 lists | all valid; no lookahead_on; history offsets inside max_bars_back |
| **Backtrader** 1.9.78.123 | a second engine, written independently, for both projects | T3: 12,710 trades, 0 mismatches. DISC: 3,462 trades, and 4,895 on the holiday market, 0 mismatches |
| **vectorbt** 1.1.2 | the T3 Stage I screen rebuilt in vectorbt | 8,470 trades, 0 mismatches |
| **Hypothesis** 6.169 | property tests on random markets | T3 engine: 4,000 random markets, all properties hold. DISC: 3,000 random bar sets and 2,000 quantile samples |
| **statsmodels** 0.15, **arch** 8.0, **scikit-learn** 1.9.1, **QuantStats** 0.0.86, **Optuna** 5.0 | the statistics checked against independent references | HAC t, BH, SPA, StepM, K_eff all match; QuantStats and Optuna conventions pinned (section 4) |
| **MLflow** 3.17 | each audit round logged (findings by severity, the report) | MLflow 3 refuses the old file store: use `sqlite:///mlflow.db` |
| **pytest** 9.1 | runs the self-tests | T3 self-test: 46 of 46 |
| **DSPy** 3.4, **PydanticAI agents**, **CrewAI** | need a language-model API key; none was available here | not run (section 6) |
| **DVC** 3.67 | versions the data files | not run here: no data in this environment |

---

## 2. What the audit found and what was fixed

| # | project | found by | finding | fix | check after the fix |
|---|---|---|---|---|---|
| 1 | T3 | Backtrader work + the manual's math.round | the engine rounded halves to even (2.5 → 2), TradingView rounds halves up (2.5 → 3): with TP 0.5 or 1.5 and an odd stop size, the target was one tick apart | engine and slow reference round halves up; B.4 states it | new hand case; Backtrader 0 mismatches |
| 2 | both | holiday-market test | the research engines ended the session at the last bar *present* before 15:08, which reads the next bar (a small look-ahead on days with a missing bar) | the last tradable bar is now known at its close from the clock and the exchange calendar, exactly as the Pine does | DISC replica 0 mismatches on the holiday market; T3 46/46 |
| 3 | both Pine | constants audit | the Pine scripts started sessions on `time_tradingday`; the research engines on 17:00 CT or a 120-minute gap | both scripts use the research rule | holiday tests |
| 4 | DISC Pine | Pine audit + the manual's limits (20 s basic, 40 s others) | 1,500 loop steps on every bar at Q = 500 (two loops over Q bars and an RMS loop over Q sessions) | `ta.sma` and `ta.stdev` (population, as defined in the manual) for the prior-window mean and sd; a running sum of squares for the slot RMS; no loop over Q remains | parity 0 mismatches |
| 5 | both | coverage audit | no rule for holidays and early closes | B.2 data law; F0 matches every long gap to the CME holiday calendar | — |
| 6 | both | coverage audit | no repaint check | P10: weekly bar-replay comparison of live signals and fills | — |
| 7 | both | coverage audit | no check of the Pine execution-time limit | P10: Pine Profiler on the full chart | — |
| 8 | T3 | coverage audit | P10 parity allowed the trade count to differ by ±5% | exact parity against the engine's TradingView path, defined in B.4 | — |
| 9 | T3 | coverage audit | no second engine | Backtrader and vectorbt (B.21-6) | 0 mismatches |
| 10 | T3 | coverage audit | no property tests | Hypothesis (B.21-7) | 4,000 markets |
| 11 | both | coverage audit | randomization without fixed seeds | every draw uses a seed fixed in F2 and kept in the ledger | — |
| 12 | T3 | coverage audit | no stop-fill stress (DISC had it) | B.16 and P7: stop fills 2 ticks worse | — |
| 13 | both | spec audit | no family could be refuted by its own mechanism | each family names the same entries without its key condition; P8 runs it once on Validation | 31 refutation lines |
| 14 | T3 | spec audit | 10 legacy families had no mechanism or sources | reasons with sources R48–R51 in C.2 | spec audit passes |
| 15 | T3 | Pine audit | MARKET_AGREE reads a second market: on a live bar its bar must be complete | covered by the repaint check | — |

Everything else checked clean in round 1: all 16 shared constants match between frameworks, Pine and engines; Pine
commissions equal half the TopstepX round trip for every contract; no look-ahead; exact mirror; every engine test passed.

---

## 3. The audit rounds

| | round 1 (before) | rounds 2 and 3 (after) |
|---|---|---|
| blocking findings | 18 | 0 |
| safeguards covered (of 30), T3 / DISC | 24 / 24 | 30 / 30 |
| families with a full specification, T3 / DISC | 0 of 21 / 0 of 10 | 21 of 21 / 10 of 10 |
| T3 self-test (pytest) | 44 of 44 | 46 of 46 |
| DISC loop steps per bar at Q = 500 | 1,500 | 0 |
| DISC holiday market: Pine replica / Backtrader / causality | not tested | 9,362 trades, 0 mismatches / 4,895 trades, 0 mismatches / pass |

The safeguard check searches each framework for the wording of each safeguard; every hit was also read by hand.

---

## 4. Tool rules that silently change results (all verified here)

| tool | trap | rule |
|---|---|---|
| vectorbt | `from_signals` fills at the signal bar's close | shift the signals one bar and pass `price = open` |
| Backtrader | orders are checked in the order they were sent, each against the whole bar | handle an open beyond the target first; cancelling one order of an OCO pair cancels both; a cheat-on-close fill is reported on the next bar; `Order.Close` does not fill at an intraday close (use `set_coc(True)` with `coc=False` on every order except the session-end exit) |
| numpy / numba | `round` and `np.round` round halves to even | Pine's `math.round` rounds halves up: use `floor(x + 0.5)` |
| Pine | `ta.stdev` is the population sd by default (`biased = true`); `request.security` with `gaps_on` returns na where the other market has no bar | the engines use the same definitions |
| QuantStats | `sharpe` is annualised with `periods` and ddof 1 | never read it as a per-session Sharpe |
| Optuna | `TPESampler(seed)` repeats exactly only with `n_jobs = 1` | — |
| scikit-learn | K_eff by `AgglomerativeClustering(metric="precomputed", linkage="average", distance_threshold=0.70)` | equals scipy's average linkage |
| statsmodels | HAC with `use_correction=False` | equals the textbook Newey–West t |
| MLflow 3 | the file store is refused | `sqlite:///mlflow.db` |
| Python | CrewAI and vectorbt do not support Python 3.14 | use Python 3.13 for the whole stack (243 packages resolve together) |

---

## 5. Versions checked on PyPI (2026-10-12)

langgraph 1.2.15 · llama-index-core 0.14.25 · dspy 3.4.1 · pydantic-ai 2.55.0 · crewai 1.15.27 · vectorbt 1.1.2 ·
backtrader 1.9.78.123 (last release 2023, unmaintained but stable) · optuna 5.0.0 · scikit-learn 1.9.1 ·
statsmodels 0.15.0 · arch 8.0.0 · pytest 9.1.1 · hypothesis 6.169.0 · quantstats 0.0.86 · mlflow 3.17.0 ·
dvc 3.67.1 · numba 0.68.0 · numpy 2.5.4 · pandas 3.0.6 · scipy 1.18.1

---

## 6. What could not run here, and how to use it

| item | why not here | how to use it on your PC |
|---|---|---|
| DSPy, PydanticAI agents, CrewAI | they call a language model and need an API key | give the agent your key; use DSPy to tune the review prompts on the findings of section 2 as examples, and a PydanticAI agent as a reviewer whose answers must fit the `Finding` and `FamilySpec` records |
| Compile and profile the Pine scripts | TradingView is not reachable from this environment | P10 step 1: compile, run the Profiler, then the parity replay |
| Real-data results | your data stays with you | the framework runs on your data, phase by phase |
| DVC | no data here | `dvc init`, then `dvc add` each market file before F0, so the data hash in F0 is the DVC hash |
