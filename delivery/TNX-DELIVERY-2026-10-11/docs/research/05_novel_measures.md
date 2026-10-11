# 05 — Composite measures for an "Imbalance Strength" indicator on range / tick bars

Written 2026-10-11 for TradingPlatform.BusinessLayer 1.146.7.0. Companion to 01 (API), 02 (literature), 03 (script audit).
Everything here uses only per-bar data; nothing needs the order book or raw ticks.

## 0. Status legend and what could be verified today

| Status | Meaning |
|---|---|
| VERIFIED | Read in an official Quantower source held locally: official example / script code (`Quantower/Examples`, `Quantower/Scripts`, downloaded in full from raw.githubusercontent.com, copies in `research/dl/`) or the official help markdown (`Quantower/QuantowerKB`, copies in `research/dl2/` and `research/dl/kb/`). |
| DOCS-EXTRACT | api.quantower.com page seen only through a search-engine extract (WebFetch to api.quantower.com, help.quantower.com, arxiv.org and cis.upenn.edu all failed today with `getaddrinfo ENOTFOUND`; retried from this session, same result). |
| DECOMPILED | Seen only in the decompiled mirror (`research/decompiled_mirror/`, older build). Concrete but unverified. |
| INFERRED | Deduced from the user's compiling source `TNXCVDPro.cs` and the DLL member dump `api_surface_from_dll.txt`. |
| SNIPPET / UNVERIFIED | Literature or vendor claim carried over from 02 with its status there; nothing new could be opened today. |
| SPECULATIVE | My own construction; no literature or test behind it. Marked in the text. |

### 0.1 The per-bar inputs (the only data the measures may use)

| Symbol | Source member | Status and source |
|---|---|---|
| `B_i`, `S_i`, `D_i = B_i − S_i`, `V_i` | `GetVolumeAnalysisData(off)?.Total.BuyVolume / SellVolume / Delta / Volume` | INFERRED (user's `Compute`, lines 1706–1715; DLL dump lists `get_BuyVolume, get_SellVolume, get_Delta, get_Volume, get_MaxDelta, get_MinDelta` on `VolumeAnalysisItem`, `get_Total` on `VolumeAnalysisData`). DOCS-EXTRACT: `BuyVolume`/`SellVolume` double get/set, `Delta` get-only, `MaxDelta`/`MinDelta` get/set (https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisItem.html). KB definition VERIFIED: "Delta — the difference in traded Volume between Buyers and Sellers", "DeltaPercent — the difference (%)" (`research/dl/kb/quantower-algo_access-volume-analysis-data-from-indicators.md`, source https://help.quantower.com/quantower-algo/access-volume-analysis-data-from-indicators). |
| `D⁺_i = MaxDelta`, `D⁻_i = MinDelta` (running max/min of the bar's delta) | `Total.MaxDelta`, `Total.MinDelta` | VERIFIED sentinel handling in official `IndicatorCumulativeDelta.cs` lines 1053–1059 (`MaxDelta != double.MinValue`, `MinDelta != double.MaxValue`, NaN check) — https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorCumulativeDelta.cs. Semantics "running extreme of Delta while trades accumulate": DECOMPILED. |
| `N_i` trades | `Total.Trades` (int) | VERIFIED usage `tradesCount += volumeData.Total.Trades` in official `IndicatorDeltaDots.cs` line 131 (https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorDeltaDots.cs). Can be 0 on bid/ask-type tick history (DECOMPILED). Not referenced by the user's DLL. |
| `O_i, H_i, L_i, C_i` | `GetPrice(PriceType.Open/High/Low/Close, off)` or `HistoryItemBar.Open/High/Low/Close` | INFERRED (user's source) / DECOMPILED (`HistoryItemBar.cs`). DOCS-EXTRACT confirms `HistoryItemBar : HistoryItem` with `Typical = (H+L+C)/3` (https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemBar.html). |
| `τ_i` duration in seconds | `new TimeSpan(item.TicksRight − item.TicksLeft).TotalSeconds` for non-time aggregations | VERIFIED idiom, official `IndicatorSpeedOfTape.cs` lines 66–72 (https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorSpeedOfTape.cs); the same script skips `HistoryAggregationTick`. The user's code instead uses the next bar's open minus this bar's open (INFERRED, `LastBarSpeed`), which includes the dead time between bars. Exact semantics of `TicksRight` on range/tick bars (last trade time vs something else): UNVERIFIED (the DOCS-EXTRACT sentence "TicksRight defines the bar's ticks count" is not usable). |
| Footprint `A(p)` (buy at price p), `Bd(p)` (sell at p) | `VolumeAnalysisData.PriceLevels : Dictionary<double, VolumeAnalysisItem>` | VERIFIED: official `DrawValueAreaForEachBarIndicator.cs` (`priceLevels ... OrderBy(it => it.Price)`) and `IndicatorDeltaDots.cs` lines 166–171 (https://raw.githubusercontent.com/Quantower/Examples/master/IndicatorExamples/DrawValueAreaForEachBarIndicator.cs). Needs `IsRequirePriceLevelsCalculation => true` (VERIFIED pattern in every official per-price indicator). Keys must be sorted; adjacent-key differences give the level step. |
| Chart type, range size `R` (ticks), tick count | `HistoricalData.Aggregation is HistoryAggregationRangeBars { RangeBars }` / `HistoryAggregationTickBars { TicksCount }` | INFERRED (user's `DetectGeometry`, DLL dump). |
| Range-bar construction | "Each range bar must have a high/low range that equals the specified range. Every new range bar must open outside the high/low range of the previous bar. Each range bar must close at either its high or its low." | VERIFIED, official help markdown `analytics-panels/chart/chart-types/range-bars.md` (local `research/dl2/KB_chart-types_range-bars.md`; page https://help.quantower.com/quantower/analytics-panels/chart/chart-types/range-bars). |
| Tick-bar construction | "build a new bar after a set number of trades … A tick can be a trade of just one contract, or 10,000 contracts." | VERIFIED, official help markdown `chart-types/tick-chart.md` (local `research/dl2/KB_chart-types_tick-chart.md`). |
| Official "effort × result" precedent | `IndicatorDeltaFlow.cs` plots `Total.Delta * (High − Low)` | VERIFIED (02 §1). On range bars it is `D · const`. |
| Official diagonal-imbalance guards | DeltaDots: zero side replaced by 1 unless `allowZeroComparsion`; skip a level when `Volume <= minimumVolume` or both sides `<= compareTreshold` | VERIFIED, `IndicatorDeltaDots.cs` lines 174–199. |

Consequences used throughout (derived from the VERIFIED range-bar rules): on a finished range bar `H − L = R` exactly; `C ∈ {H, L}`; the open may lie inside the bar, so the signed result `C − O` satisfies `0 < |C − O| ≤ R` and `R − |C − O|` is the part of the range first traded the other way (the "open-side wick"). The direction `d_i = sgn(C_i − O_i)` is never 0 on a finished range bar. On tick bars `H − L` and `C − O` are free.

---

## 1. Notation and the shared primitives

```
i            bar index (0 = oldest); "finished" = not the forming bar; "real" = VA present and V_i > 0
d_i          = sgn(C_i − O_i)                     bar direction (−1, 0, +1)
s_i          = sgn(D_i), or d_i when D_i = 0        flow side
agree_i      = (s_i == d_i) and d_i != 0            the flow won the bar
t            tick size (price units);  R  range size in ticks (range bars);  Δx_i = (C_i − O_i)/t  signed displacement in ticks
τ_i          bar duration in seconds, floored (see 1.3)
```

### 1.1 Typical size: `med_N(x)`
Median of `x_j` over the last `N` finished, real, same-session-type bars `j < i` (default `N = 60`; the user's `LookbackBars`). Use the median, not the mean: `|D|`, `V`, `τ` are heavy-tailed and one news bar drags a mean for `N` bars (02 §3.3). Floors: `med_N|D| ≥ max(2 contracts, min non-zero |D| in window)`, `med_N V ≥ 1`, `med_N τ ≥ 0.1 s`. Cost: O(N) per closed bar (N = 60: negligible); or O(log N) with a sorted window.

### 1.2 Rank: `rank_N(x)`
`rank_N(x_i) = (#{j : x_j < x_i} + ½ · #{j : x_j = x_i}) / N` over the same window, ∈ [0, 1]. This is the user's `FlowRank` (INFERRED, lines 6394–6417) generalised. Return "undefined" (the user's −1) while fewer than 20 bars are in the window. Step size 1/N; saturates above the window's maximum (show the multiple `x / med_N x` next to it, as the user already does: "+1.2K · 91%").

Why rank for the composite: distribution-free, bounded, instrument-free, robust to the heavy tails; the VPIN literature's convention (02 §3.4). Why a fixed scale for `ω` (one-sidedness) and footprint ratios: they already have units (fractions, ratios).

### 1.3 Guards shared by every measure
- No VA / `V_i = 0`: the bar is not real. Exclude it from every window, rank and autocorrelation. Never fill with 0.
- `τ_i`: floor at `τ_min = 0.1 s` for rates (a 2-tick ES bar can genuinely finish in 100 ms; the floor only stops ∞). Exclude a bar whose `[TicksLeft, TicksRight]` crosses a session boundary (`ISession` plumbing exists in the user's code, INFERRED) or cap `τ_i ≤ 10 · med_N τ` when session data are unavailable. Gaps between bars (no trade) are not inside `TicksRight − TicksLeft`, so prefer that form over "next open − this open".
- Unclassified volume: `u_i = 1 − (B_i + S_i)/V_i` (how Quantower classifies without an aggressor flag is UNVERIFIED, 02 §1). Use `B + S` as the denominator of one-sidedness so it stays ≤ 1; if `u_i > 0.2` down-weight the bar (multiply the participation component by `1 − u_i`).
- `MaxDelta`/`MinDelta` sentinels (`double.MinValue` / `double.MaxValue`) and NaN → "unknown" → the intrabar measures (§3.9) return neutral 0, not an extreme.
- First bar of history / after a reset: anything needing `i − 1` returns neutral (persistence 0, confirmation unknown).
- Range bar with `H − L < R` (cut by session end or still forming): treat as forming; do not rank its totals.
- Forming bar: rank only rates (`V/τ`, `D/τ`); totals (`V`, `|D|`, `N`) are partial. Extrapolating by `1/F` (the user's progress fraction) is unstable for `F < 0.25`; show "pending" below that.

---

## 2. The physics frame, stated precisely

Two separate analogies are used. They are not one consistent mechanics and must not be mixed:

**A — kinematic.** Position = price in ticks, `x`. Velocity `v = Δx/τ` [ticks/s]. "Mass" `m = V` (contracts traded: the amount of material that changed hands while the price moved). Momentum `p = m v`, kinetic energy `K = ½ m v²`.

**B — dynamic.** Force = net aggressive flow rate `F = D/τ` [contracts/s]. Impulse `J = Σ F τ = Σ D` [contracts] — the change of cumulative delta. Work = force × displacement.

The link between A and B is the empirical price-impact law: `Δx ≈ λ · D` (Kyle 1985; Cont–Kukanov–Stoikov 2014 `Δp ≈ OFI / depth`; both SNIPPET in 02 §2b–c). So "efficiency" `Δx/D = λ̂` and "friction" `D/Δx = 1/λ̂ = depth`. Where the analogy breaks: no conservation law (volume is not conserved mass; impulse does not equal momentum change); `λ` is not a constant of the system but a state variable that the measures are trying to read; `τ` is itself an outcome (range bars complete faster when volatility is high), so every "per second" quantity is partly a volatility reading (Andersen–Dobrev–Schaumburg 2008, SNIPPET in 02 §3.1: passage time of a fixed distance ∝ `R²/σ²`).

On range bars `|Δx| ≈ R` is fixed. This collapses several A/B quantities onto each other (§3 shows each collapse explicitly). The free variables on a finished range bar are exactly: `s_i`, `agree_i`, `V_i`, `D_i`, `τ_i`, `N_i`, the delta path (`D⁺, D⁻`), the open-side wick, and the footprint. Everything below is built from these.

---

## 3. The measures

Each entry: formula, units, normalisation, degenerate cases, what it adds beyond `D` and `V`, and its acceleration (the general rule is §4; only deviations are stated here).

### 3.1 Force `F` (net flow rate)
```
F_i = D_i / τ_i                      [contracts / s]        signed
F^R_i = D_i / R                      [contracts / tick]     range bars: force per unit of range = D_i / const
```
Normalise: `rank_N(|F|)` with sign `s_i`; magnitude as `|F_i| / med_N|F|`.
Degenerate: `τ → τ_min`; `D = 0` → 0; forming bar: `F` is a rate, so rank it directly (no `1/F` scaling).
Adds beyond `D`: time. On range bars `τ` is free, so `F ≠ D`: the same 400-contract delta over 2 s and over 40 s are different forces. But `F` is dominated by `1/τ` at the tails (a 20-contract delta in 0.2 s out-ranks 400 in 10 s). For the composite, keep `|D|` and `τ` as separate dials and show `F` as the headline rate; do not put `F` alone into the product.
Acceleration: physically `dF/dt ≈ (F_i − F_{i−1}) / τ_i`; for display use the per-bar rule of §4 on `s·F`.

### 3.2 Impulse `J_k`
```
J_k,i = Σ_{j = i−k+1 .. i} D_j        [contracts]      = CVD_i − CVD_{i−k}
```
Normalise: `rank_N(|J_k|)`; sign `sgn(J_k)`. Default `k = 4` (the ABSORBED rule, §6.3) and `k = VelocityPeriod` for the flow lane (this is the user's `Fast`·k).
Degenerate: fewer than `k` real bars → undefined; a non-real bar inside the window → undefined (do not sum around it).
Adds: multi-bar effort. Needed to detect absorption on range bars, where a single bar's result is fixed (02 §3.1) and only the net result of several bars can be "small".
Acceleration: `J_k` differenced is just `D_i − D_{i−k}`; use §4 on `J_k` directly (O(1)).

### 3.3 Velocity `v`, duration `τ`
```
v_i = Δx_i / τ_i                     [ticks / s]        range bars: ≈ d_i · R / τ_i
```
Normalise: `log τ_i − log med_N τ` (durations are log-symmetric; 02 §3.4); tiers at ratio 1.25 / 0.8 (the user's "faster / slower than typical", INFERRED `LastBarSpeed`).
Degenerate: `τ_min` floor; session gaps excluded (1.3).
Adds: on range bars `R²/τ` is a volatility estimate (SNIPPET, ADS 2008). It is not imbalance; it is the regime the imbalance happened in.
Acceleration: §4 on `log τ` (sign flipped so that "speeding up" is positive): `Δ(−log τ)`.

### 3.4 Momentum `p` → participation `π`
```
p_i = V_i · v_i = V_i Δx_i / τ_i     [contracts · ticks / s]     range bars: ≈ d_i · R · V_i / τ_i
π_i = V_i / τ_i                      [contracts / s]              "participation": |p| / R on range bars
```
Normalise: `π̂_i = rank_N(V_i / τ_i)`; magnitude `(V/τ) / med_N(V/τ)`.
Degenerate: as 3.1; forming bar: a rate, rank directly; `u_i > 0.2` → multiply `π̂` by `1 − u_i`.
Adds beyond `V`: `V` alone (contracts per range) is the depth taken; `V/τ` is the activity rate. They separate four regimes that `V` alone does not: high `V`, short `τ` = burst; high `V`, long `τ` = grind; low `V`, short `τ` = vacuum (thin, fast); low `V`, long `τ` = dead. The composite needs both: `π̂` (activity) and `rank_N(V)` (depth taken, §3.7).
Score note: `p` itself (signed) adds nothing beyond `π` and `d_i`; use `π`.

### 3.5 Kinetic energy `K`
```
K_i = ½ V_i v_i²                     [contracts · ticks² / s²]   range bars: ∝ V_i / τ_i²
```
Normalise: `rank_N(log K)`.
Adds: nothing that `π` and `τ` do not already carry; it just squares the speed. Heavier tails than `π`. Recommendation: drop.

### 3.6 Work `W`
```
W_i = D_i · Δx_i                     [contracts · ticks]     (official DeltaFlow uses D · (H − L), VERIFIED)
```
Sign: positive when the flow's side and the bar's direction agree, i.e. `sgn(W) = agree`. On range bars `|W| = |D| · R`, a constant times effort; so on range bars `W` carries exactly `(agree, |D|)`. On tick bars `|W|` grows with both effort and result and is the honest "effort × result".
Normalise: `rank_N(|W|)` with sign.
Degenerate: `Δx = 0` (tick-bar doji) → `W = 0` regardless of `D` — a big `|D|` with `W = 0` is the single-bar absorption case; keep `agree` as a separate flag so it is not lost.
Adds beyond `D`: the agreement sign (the TRAPPED detector). Keep `agree`; `W` itself is redundant on range bars.

### 3.7 Efficiency `η`, friction `Z` (Kyle, Amihud, Wyckoff)
```
η_i = Δx_i / D_i                     [ticks / contract]   = λ̂_i  (price moved per unit of net flow; Kyle's λ, SNIPPET)
Z_i = V_i / max(|Δx_i|, 1)           [contracts / tick]   friction: volume needed per tick (Amihud-form, per bar; Amihud's ILLIQ is |return|/dollar volume on daily data, SNIPPET — this is the per-bar analogue, not his measure)
Z^D_i = |D_i| / max(|Δx_i|, 1)       [contracts / tick]   depth taken by the aggressor = 1/λ̂  (CKS: depth, SNIPPET)
η^W_i = (|Δx_i| / med_N|Δx|) / (V_i / med_N V)            Wyckoff result-per-effort, dimensionless (the user's r/e, INFERRED BarRead)
```
Range-bar collapse: `|Δx| = R` ⇒ `Z = V/R ∝ V`, `Z^D ∝ |D|`, `η = R/D ∝ 1/D`, `η^W = 1/(V/med V)`. So on range bars "efficiency" and "friction" are the effort dials inverted; show one: `rank_N(V)` as **depth taken** (high = many resting orders were in the way and were taken: the user's "heavy buying broke through"; low = thin book).
Tick bars: `Z` is a genuine second dimension (a 10-tick move on 200 contracts vs a 2-tick move on 200 contracts). Keep `Z` there.
Unified definition for the THIN rule: `ζ_i = rank_N(Z_i)` with `Z_i = V_i / max(|Δx_i|, 1)`; on range bars this is `rank_N(V_i)`.
Normalise: ranks; magnitude `Z / med_N Z`.
Degenerate: `D = 0` → `η` undefined → report "no net flow"; tick-bar doji → `Z` large (not thin) — correct: lots of volume, no displacement is friction, not thinness.
Adds: on tick bars, the Kyle/Amihud slope per bar. On range bars nothing beyond `V` (say so in the UI: "depth taken").
Acceleration: §4 on `log Z`.

### 3.8 Pressure
```
P_i = D_i / n_i                      [contracts / level]    n_i = number of price levels in the bar (≈ R/t + 1 on range bars)
```
Range-bar collapse: `n_i` is constant ⇒ `P ∝ D`. Tick bars: `n_i ≈ (H−L)/t + 1` ⇒ `P ≈ D · t/(H − L)`, which is `Z^D` again with `H − L` instead of `|C − O|`. Recommendation: drop as a separate measure; the footprint "concentration" (share of the bar's `|delta|` in the top-k levels) is the useful pressure-like quantity and lives in §3.13.

### 3.9 Intrabar battle: give-back `g` and open-side wick `w`
```
E⁺_i = max(D⁺_i, 0), E⁻_i = max(−D⁻_i, 0)              largest excursions of the running delta (from MaxDelta / MinDelta)
g_i  = (E_side − |D_i|) / E_side,  E_side = (D_i ≥ 0 ? E⁺ : E⁻),  g ∈ [0, 1]      delta given back by the close (the user's `val`, INFERRED lines 1760–1766)
w_i  = 1 − |C_i − O_i| / (H_i − L_i),  w ∈ [0, 1)                                 share of the bar's range first traded against its final direction
```
Units: fractions. Normalise: fixed thresholds (bounded quantities): `g ≥ 0.5` or `w ≥ 0.5` = the bar reversed internally (half the push, or half the range, was undone).
Degenerate: `E_side = 0` (no excursion) → `g = 0`; sentinels/NaN → `g = 0`; `H = L` → `w = 0`; the decompiled `Combine` writes NaN into Min/MaxDelta (DECOMPILED) — never use combined items for `g`.
Adds beyond `D`: the path. Two bars with `D = +300` differ if one ran to `+900` and came back. On range bars `w` is the only within-bar price-path information left (the range is fixed, the close is at an extreme; where the open sat is free).
Acceleration: not meaningful per bar; use the rolling mean `mean_k(g)` (`k = 5`) and §4 on it if a "battle is building" dial is wanted.

### 3.10 Path efficiency over k bars `η_k`
```
η_k,i = |C_i − C_{i−k}| / Σ_{j=i−k+1..i} |C_j − O_j|      ∈ [0, 1]       net move / path length (the standard efficiency-ratio form; attribution UNVERIFIED here)
```
Range bars: each `|C_j − O_j| ≈ R`, so `η_k ∈ {0, 2/k, 4/k, …}`-ish — coarse. Use `k ≥ 4`. The ABSORBED rule uses the raw net move instead: `|C_i − C_{i−k}| ≤ m̄`, with `m̄ = med_N|C − O|` (= `R` on range bars).
Degenerate: `k` real bars needed; a reset/session boundary inside the window → undefined.
Adds: the multi-bar result that a single range bar cannot have. With `J_k` (3.2): big impulse + small net move = absorbed ("force applied, momentum unchanged: a wall").
Acceleration: not needed.

### 3.11 Inertia / persistence `ψ`
```
ψ_i  = (1 − β) ψ_{i−1} + β · s_i s_{i−1}      β = 2/(k+1), k = 8      EWMA of consecutive flow-sign agreement, ∈ [−1, 1]
c_i  = (1/k) Σ_{j=i−k+1..i} 1[s_j = s_i]     fraction of the last k bars on the current side, ∈ [0, 1]   (k = 4)
run_i = length of the current same-sign run of D
```
Normalise: `ψ̃_i = (1 + ψ_i)/2 ∈ [0, 1]`; `run` as `rank_N(run)` or fixed (≥ 3 = "persistent").
Degenerate: first bar `ψ = 0`, `c = 1/k`; non-real bar breaks the run (do not bridge). `s_i = 0` (no flow, no direction) → leave `ψ` unchanged.
Adds beyond `D`: context of the sequence. Order signs have long memory (Lillo–Farmer 2004, SNIPPET): part of persistence is order splitting, not information, so persistence raises "real" only in combination with participation (the composite does that). It also separates REAL from a one-bar spike.
Acceleration: `ψ` is already smoothed; `Δψ` per bar under §4 is the "persistence breaking" dial.

### 3.12 Resonance / self-excitation `H` (Hawkes-style) — SPECULATIVE as an estimator
```
x_i   = max(0, 2 (π̂_i − 0.5))                      burst size: 0 below the median activity, 1 at the top   (or 1[π̂ ≥ 0.8])
H⁺_i  = H⁺_{i−1} e^{−τ_i / T} + x_i · 1[s_i = +1]
H⁻_i  = H⁻_{i−1} e^{−τ_i / T} + x_i · 1[s_i = −1]          T = 10 · med_N τ  (memory ≈ ten typical bars, in real time)
ρ_i   = (H⁺_i − H⁻_i) / (H⁺_i + H⁻_i + 1)            ∈ (−1, 1)     which side's bursts dominate recent memory
```
Units: "bursts remembered" (dimensionless). Under independent bursts with probability `q` per bar of typical length, `E[H] ≈ q / (1 − e^{−0.1}) ≈ 10.5 q`; normalise by `rank_N(H⁺ + H⁻)` for "excited regime" and use `ρ` as is.
Degenerate: `H = 0` at start; a long gap (`τ` large) empties the memory, which is the intended behaviour (quiet time dissolves excitation); session boundary → reset `H` to 0.
Adds beyond persistence: it weights bursts, not every bar, and decays in real time, not in bars (two bursts 10 s apart resonate; two bursts an hour apart do not). This is a Hawkes intensity with an exponential kernel evaluated at bar closes, but the branching ratio `n = α/β` is not estimated (unequal bar windows make the count-variance estimator invalid; 02 §2f). Literature basis for self-excitation of aggressive flow: Hawkes-in-finance surveys (SNIPPET only, 02 §2f). Treat `ρ` as a context word ("buyers keep coming in bursts"), not as a component of the composite.
Acceleration: `ΔH` per bar is noisy; use §4 on `H⁺ + H⁻` with `m = 5`.

### 3.13 Depth of imbalance `φ` (footprint)
Levels sorted ascending `p_1 < … < p_n`, step `t` = smallest positive key difference (fallback `Symbol.TickSize`); missing levels inside `[L, H]` count as `A = Bd = 0`. Ratio `r` (default 3.0), floor `v_min = max(v_user, 0.5 · median level volume of the bar)`, stack length `k_s = 3` — conventions SNIPPET (02 §2h), the zero-guard VERIFIED (DeltaDots).
```
buyFlag(p_k)  = A(p_k)  ≥ r · max(Bd(p_{k−1}), 1)  and  A(p_k)  ≥ v_min          (ask lifted vs bid one tick below)
sellFlag(p_k) = Bd(p_k) ≥ r · max(A(p_{k+1}), 1)   and  Bd(p_k) ≥ v_min          (bid hit vs ask one tick above)
K⁺, K⁻        = longest run of consecutive buy / sell flags            (levels)
Φ⁺, Φ⁻        = Σ_{flagged} (A + Bd) / (B_i + S_i)                   share of bar volume inside flagged levels, ∈ [0, 1]
z⁺, z⁻        = (volume-weighted mean price of flagged levels − L) / (H − L)    position in the bar, ∈ [0, 1]  (0 = low, 1 = high)
UA_low        = A(p_1) > 0 and Bd(p_1) > 0 ;   UA_high likewise at p_n            unfinished auction flags (both sides traded at the extreme)
"with" side   = the flow side s_i:  (K_w, Φ_w, z_w) = (K⁺, Φ⁺, z⁺) if s_i = +1 else (K⁻, Φ⁻, z⁻);  "against" = the other
φ_i = clip( 0.5 + 0.25 · min(K_w, k_s)/k_s + 0.5 (Φ_w − Φ_a) − 0.25 · min(K_a, k_s)/k_s , 0, 1 )
```
Units: `φ` dimensionless, 0.5 = no footprint information. `K` in levels, `Φ` in volume share, `z` in bar fractions.
Normalise: fixed (bounded). Report `K` as a fraction of the bar's levels on range bars (an 8-tick bar has at most 9 levels; `K = 3` is a third of it — 02 §3.1).
Degenerate: `PriceLevels` empty or `n < 2` → `φ = 0.5`, flags false; `IsRequirePriceLevelsCalculation = false` → `PriceLevels` is expected empty (UNVERIFIED whether null or empty; guard both); keys off the tick grid → snap by rounding to `t` before pairing; forming bar → the dictionary mutates live: clone or try/catch `InvalidOperationException` (official pattern VERIFIED).
Position `z` and the unfinished-auction flags are NOT folded into `φ`: on range bars the closing extreme is almost always two-sided (the bar is cut mid-auction; 02 §3.1 caveat 2), so `UA` at the close side is uninformative and only `UA` at the rejected extreme (the side opposite `C`) keeps its meaning. `z_w` is reported as a word ("buy stacks at the low / mid / high of the bar") and used by the confirmation measure (3.14): a with-side stack at the closing extreme whose next bar reverses is "chased and trapped at the top". Scoring `z` monotonically is SPECULATIVE and is not done.
Adds beyond `D`: where and how the aggression traded. Two bars with `D = +300`: one with three stacked buy levels holding 60 % of the volume at the bottom of the bar, one with the delta spread thinly. `Φ_w − Φ_a` is a signed concentration ("pressure" in the useful sense); `K_a` catches against-side stacks (trapped sellers at the lows of an up bar, or responsive sellers at its top — distinguished by `z_a`).
Cost: O(n) per bar with `n ≤ R/t + 1` on range bars; per tick on the forming bar, bounded the same way. Requires the heavier platform calculation (price levels) — VERIFIED pattern.
Acceleration: not per bar; `mean_k(φ)` with §4 if wanted.

### 3.14 Confirmation `κ` (next-bar response; lag 1)
```
κ_i    = d_i · (C_{i+1} − C_i) / m̄           m̄ = med_N|C − O|  (= R on range bars);  range bars: κ ∈ {−1, +1} essentially
κ^D_i  = s_i · D_{i+1} / med_N|D|              did the next bar's flow stay on the side
close_i = 1[ L_{i+1} ≤ p_UA ≤ H_{i+1} ]         the next bar traded back through the rejected-extreme unfinished level p_UA (if any)
hit_N   = fraction of bars in the window whose state was REAL and had κ > 0        running calibration dial
```
Units: `κ` in typical moves (dimensionless); `κ^D` in typical deltas; `close` a flag; `hit_N` a fraction.
Normalise: fixed (`κ > 0` confirmed, `< 0` rejected); `hit_N` shown with its sampling error (`N = 60`, `p ≈ 0.5`: ± 0.13 at 95 %; do not read a 55 % as different from 45 %).
Degenerate: the last finished bar has no `i + 1` → "pending"; a non-real `i + 1` → unknown; after a session boundary → unknown.
Adds: the only out-of-sample element in the set. It repaints by design (bar `i` gets its `κ` when `i + 1` closes): store it on bar `i` (the user's `_dirtyFrom` mechanism already recopies bar `abs − 1`, INFERRED 03 §5b) and never let it enter bar `i`'s own state (§6.3), only the "last bar" row and `hit_N`.
Acceleration: none.

### 3.15 Fakeness `Fake`
"Imbalance without participation or without result."
```
f_trap_i  = 1[ not agree_i ]                                            the flow lost the bar
f_abs_i   = 1[ rank_N|J_4| ≥ 0.65  and  |C_i − C_{i−4}| ≤ m̄ ]           big 4-bar impulse, no net move (wall)
f_gb_i    = g_i · 1[g_i ≥ 0.5]                                           the push was given back inside the bar
f_wick_i  = w_i · 1[w_i ≥ 0.5]                                           the bar first went the other way
f_thin_i  = (1 − π̂_i) · 1[ζ_i < 0.35]                                   one-sided but nobody there (low activity and low depth taken)
Fake_i    = max(f_trap, f_abs, f_gb, f_wick, f_thin)                     ∈ [0, 1]
```
Units: dimensionless. Normalise: already bounded; thresholds in rank terms where the input is a rank.
Degenerate: `g`, `w` neutral 0 when unknown; `J_4` undefined → `f_abs = 0`; `ζ`, `π̂` undefined (< 20 bars) → `f_thin = 0`.
Adds beyond `D`: it is the negation layer that `D` cannot express — `D` says how hard, `Fake` says whether it mattered. Each component is a different failure mode and keeps its own word (TRAPPED / ABSORBED / GIVEN BACK / REVERSED INSIDE / THIN).
Acceleration: none; `Fake` feeds the composite.

### 3.16 Real score `G` and the composite — see §6.2.
```
G_i = ( π̂_i^1 · ω̂_i^1 · ê_i^1 · φ_i^0.5 · ψ̃_i^0.5 )^(1/4)       participation × one-sidedness × effort × depth × persistence (weighted geometric mean)
```
with `ω̂_i = rank_N(ω_i)`, `ω_i = |D_i| / (B_i + S_i)` (one-sidedness = `|DeltaPercent|/100`, Quantower's own definition VERIFIED in KB), `ê_i = rank_N|D_i|` (effort), each component floored at 0.05. Geometric, not additive, so that a zero in any dial kills the score ("real" needs all of them); the task's definition `participation × one-sidedness × efficiency × persistence` is this product with efficiency expressed, on range bars, as the effort-per-range dial `ê` (3.7 collapse) and with the footprint depth added at half weight.

---

## 4. Acceleration: one rule for every measure

For a measure `x` (a rank, a bounded fraction, a smoothed composite, or `log` of a rate):
```
Δ_i = x_i − x_{i−L}                                  L = 1 (the user's AccelLag)
A_i = A_{i−1} + α (Δ_i − A_{i−1})                    α = 2/(m+1), m = 3 (the user's AccelSmooth); forming bar: α_F = 1 − (1−α)^F
a_i = A_i / max( med_N |Δ_j|, floor_x )              "typical absolute change" = median of |Δ| over the window, not the mean
```
Floors: `floor_IS = 2` points (0–100 scale), `floor_rank = 0.05`, `floor_log = 0.1`. Tiers (the user's constants, INFERRED): `|a| < 1` steady; `≥ 1` speeding up / slowing; `≥ 2` THRUST / STALL.
Side convention: acceleration is computed on the side-framed value `x_i = σ_i · IS_i` with `σ_i = sgn(A-smoothed IS)`; positive = the current side is strengthening; a sign flip produces a large positive `Δ` (new side speeding up), which is the user's `Flip` marker.
Why not accelerate raw ranks: a rank of a noisy quantity jumps by ±0.3 between neighbouring bars; its difference is mostly noise. Accelerate the composite (a product of five ranks is much smoother) or the EWMA'd measures (`ψ`, `H`), and normalise by the median absolute change so the units are "typical changes".
Physical acceleration of a rate (`dF/dt`) is `Δ_i / τ_i`; it is correct but makes fast bars look explosive; use the per-bar rule for display and the per-second one only for a research export.

---

## 5. Scores (0–10)

Information added = beyond `D`, `V` and the user's existing `e`, `r`, `Fast`, `Slow`, `AccE`. Robustness = on finished range bars. Cheapness = per finished bar, incremental. Explainability = one sentence to a trader.

| # | Measure | Info added | Robust (range) | Cheap | Explainable | Keep? |
|---|---|---|---|---|---|---|
| 3.1 | Force `F = D/τ` | 6 | 7 | 10 | 9 "net contracts per second" | headline rate, not in the product |
| 3.2 | Impulse `J_k` | 5 | 9 | 10 | 8 "4-bar net flow" | yes (ABSORBED rule) |
| 3.3 | Duration / velocity `τ` | 6 | 8 | 10 | 10 "bar took 3 s, typical 12 s" | yes (regime, already in CVD PRO) |
| 3.4 | Participation `π = V/τ` | 8 | 8 | 10 | 9 "contracts per second" | yes |
| 3.5 | Kinetic energy | 1 | 4 | 10 | 3 | drop |
| 3.6 | Work `W` / `agree` | 3 (`agree` 7) | 9 | 10 | 8 "flow won / lost the bar" | keep `agree` only |
| 3.7 | Depth taken `ζ` (= `rank V` on range, `V/|Δx|` on tick) | 6 (range) / 8 (tick) | 9 | 10 | 8 "volume it took to move one range" | yes (THIN rule) |
| 3.8 | Pressure `D/n` | 1 | 3 | 9 | 5 | drop (lives in 3.13) |
| 3.9 | Give-back `g`, wick `w` | 7 | 9 | 10 | 9 "half the push was given back" | yes |
| 3.10 | Path efficiency `η_k` | 5 | 6 (coarse) | 10 | 8 "4 bars, went nowhere" | yes, as the net-move test |
| 3.11 | Persistence `ψ` | 6 | 9 | 10 | 9 "4th bar on the same side" | yes |
| 3.12 | Resonance `H`, `ρ` | 5 | 7 | 10 | 6 "bursts keep coming" | context word only; SPECULATIVE |
| 3.13 | Footprint depth `φ` | 8 | 7 (level cap; close-side UA) | 7 (needs price levels) | 8 "3 stacked buy levels, 60 % of the bar" | yes |
| 3.14 | Confirmation `κ`, `hit_N` | 7 | 8 | 10 | 10 "the next bar agreed" | yes, display + calibration only |
| 3.15 | Fakeness | 8 | 8 | 10 | 8 (five named failure modes) | yes (composite) |
| 3.16 | Real score `G` | — | 8 | 9 | 7 "all five dials" | the composite |

---

## 6. Recommendation

### 6.1 Final set (8 per-bar measures, all O(1) or O(levels) per finished bar)
1. `π̂` participation rank (`V/τ`) — with magnitude `V/τ` in contracts/s.
2. `ê` effort rank (`|D|`) with sign `s` — with magnitude `|D|/med|D|` ("1.4× typical"). On range bars this is also the depth-taken / inverse-λ dial.
3. `ω̂` one-sidedness rank, with `ω` itself as `Delta %`.
4. `ζ` depth-taken rank (`V` on range bars; `V/|Δx|` on tick bars).
5. `g`, `w` intrabar give-back and open-side wick (fractions).
6. `ψ̃` persistence (EWMA sign agreement) and `run`.
7. `φ` footprint depth, with the words `K_w` stacked levels, `Φ_w` share, `z_w` position, `UA` at the rejected extreme.
8. `J_4` with `|C_i − C_{i−4}|` (the absorption pair), and `κ` confirmation on the previous bar with `hit_N`.
Context (not in the composite): `τ` tier, `F`, `ρ` resonance.

### 6.2 The composite "Imbalance Strength" `IS ∈ [−100, +100]`
```
G_i   = ( π̂_i · ω̂_i · ê_i · φ_i^0.5 · ψ̃_i^0.5 )^(1/4)        each factor clipped to [0.05, 1];  φ = 0.5 and ψ̃ = 0.5 when unknown
G'_i  = (G_i − 0.05) / 0.95                                    → [0, 1]
IS_i  = s_i · 100 · G'_i · (1 − 0.7 · Fake_i)                  sign = who is pushing (flow side), never the outcome
```
Properties (checked): bounded; symmetric in sign; monotone non-decreasing in every dial (product of positive powers); invariant to volume units and to range size (ranks over the same chart); zero when any dial is at its floor and all others are not extreme (`G` for `(0.05, 1, 1, 1, 1) = 0.05^(1/4) = 0.47` → `IS = 44` — a floor is not a kill; if a hard kill is wanted use floor 0.01: `(0.01)^(1/4) = 0.32` → `IS = 28`. Default 0.05 chosen so a bar with no footprint data is not punished). `Fake` scales by 0.3 rather than 0 so the magnitude of the failed push is still visible ("buyers 21, ABSORBED").
Forming bar: `π̂` from the rate so far; `ê`, `ω̂`, `ζ` from partial totals → shown hollow / "pending" until `F ≥ 0.25`, exactly as the user draws the forming bar hollow today; `φ` from the live dictionary; `ψ̃` from closed bars; `Fake` without `f_abs` (needs the close).
Worked archetypes (window ranks assumed; `Fake` as stated):

| Bar | `π̂` | `ê` | `ω̂` | `φ` | `ψ̃` | `Fake` | `G` | `IS` | state (§6.3) |
|---|---|---|---|---|---|---|---|---|---|
| heavy push, 3 stacked levels, 3rd bar on the side, fast | .90 | .95 | .80 | .80 | .90 | 0 | .873 | +87 | REAL |
| median bar | .50 | .50 | .50 | .50 | .50 | 0 | .500 | +47 | REAL (weak) |
| thin & fast (`rank V` = .10) | .50 | .15 | .60 | .50 | .50 | f_thin = 1 − .5 = .50 | .387 | +23 | THIN |
| busy, balanced, stacks on both sides | .70 | .30 | .10 | .40 | .50 | 0 | .311 | +28 | BATTLE |
| big delta against the bar (trapped) | .80 | .90 | .70 | .60 | .50 | 1 | .725 | +21 (buyers) | ABSORBED |
| 3rd up bar, strength falling from 87 → 75 → 55 | .60 | .55 | .50 | .50 | .90 | 0 | .577 | +55 | FADING |

(Values recomputed by script; `G` to three decimals, `IS` rounded.) The numbers confirm the intended ordering (REAL ≫ FADING > BATTLE ≈ THIN ≈ ABSORBED) and that the five states separate bars the number alone would conflate (BATTLE 28, THIN 23 and ABSORBED 21 mean three different things). Note the floor's effect: a bar with `φ` unknown (0.5) loses little; a bar with `φ` at the floor 0.05 and every other dial at 1 still scores 67, because `φ` carries half weight — the footprint modulates, it does not veto.

### 6.3 The 5-state verdict: decision tree, in rank terms
Evaluated on finished real bars at their close (no repaint); the forming bar gets the same tree on pending values and is labelled "forming". Precedence is the order below; the first true rule wins, so every bar gets exactly one state. Thresholds are percentile ranks over the same-session window `N = 60` unless the quantity is a bounded fraction. Parameters in brackets are the suggested user settings.

```
NO DATA   : not real, or ranks undefined (< 20 bars)                                  → "—"
THIN      : ζ_i < 0.35                                      [thin rank 0.35]            little depth taken: the move was cheap; nobody was there
            (range bars: rank V < 0.35; tick bars: rank(V/|Δx|) < 0.35)
BATTLE    : ω̂_i < 0.35  or  g_i ≥ 0.5  or  w_i ≥ 0.5  or  (min(K⁺,k_s) = k_s and min(K⁻,k_s) = k_s)
                                                            [balance rank 0.35; give-back 0.5; wick 0.5; stacks both sides]
            two-sided: no one-sidedness, or the push / the range was reversed inside the bar
ABSORBED  : not agree_i                                                                  the pushing side lost the bar (TRAPPED sub-label)
            or ( rank_N|J_4| ≥ 0.65 and |C_i − C_{i−4}| ≤ m̄ )   [impulse rank 0.65; k = 4]  big flow, price went nowhere (WALL sub-label)
FADING    : a_i ≤ −1.0 and s_i = s_{i−1} and ê_i ≥ 0.35        [accel −1.0 typical changes]  same side as before but strength is falling
REAL      : everything else (agree, one-sided, depth taken, not fading); grade by IS
```
Reading the sub-labels: ABSORBED/TRAPPED means "aggressors pushed, passive side won — the other side is in control"; ABSORBED/WALL means "k bars of net flow without progress — resting orders are soaking it up"; both are the user's existing "Absorbed by: passive buyers/sellers" row.
Hysteresis: none inside the tree (a finished bar's state is final). To stop the forming bar flickering between states on every tick, require the pending state to hold for 2 consecutive evaluations or for `F ≥ 0.5` before it is printed.
Expected frequencies (so the thresholds are not surprising): by the rank definition about 35 % of bars are THIN; the `ω̂` rule alone flags up to another 35 % of all bars as BATTLE (fewer, because low-depth bars are often one-sided and are already THIN); REAL is the residual, roughly 30–40 % of bars, graded by `IS`. If the user wants REAL to be rarer, raise the THIN and BATTLE ranks to 0.4, or print REAL only when `IS ≥ 50` and "real (weak)" below.

### 6.4 What is speculative
- The weights `(1, 1, 1, ½, ½)`, the component floor 0.05, the `Fake` scale 0.7, and every rank threshold in §6.3 are design choices with no test behind them. They are chosen so the archetype table orders as intended; they are not fitted.
- `φ`'s formula (stack weight 0.25, share weight 0.5, against-stack penalty 0.25) is SPECULATIVE; the only parts with vendor backing are the diagonal rule, the ratio `r`, the floor `v_min` and the stack length `k_s` (SNIPPET conventions, 02 §2h).
- `H`/`ρ` as a Hawkes reading is SPECULATIVE (a real-time EWMA of bursts; no branching ratio).
- The multi-bar WALL rule `rank|J_4| ≥ 0.65 ∧ |ΔC| ≤ m̄` is a plausible reading of "absorption" (Jigsaw's description, SNIPPET) but untested, like the user's current absorption rule (guide: H-0006).
- Any predictive claim. The literature in 02 §2j says trade imbalance explains a large part of the contemporaneous move and forecasts only seconds-to-two-price-changes ahead. `IS` describes the bar that happened and who is in control; `hit_N` shows the user how often REAL was followed by a same-direction bar on this chart, which on range bars is near the base rate of continuation (unknown here; measure it).

---

## 7. Cost and placement

| Item | Where | Cost per finished bar | Forming bar |
|---|---|---|---|
| `π̂, ê, ω̂, ζ` ranks | compute thread, once at close (`Compute`, like `FlowRank` but stored in `Bar`) | 4 × O(N), N = 60 | rates re-ranked per tick (O(N)); totals pending |
| `med_N` scales | once at close | O(N) (or O(log N) sorted window) | reuse |
| `g, w, agree, s, d` | at close | O(1) | per tick O(1) |
| `ψ, run, H⁺, H⁻, ρ` | at close | O(1) | not updated |
| `J_4`, `|C_i − C_{i−4}|` | at close | O(1) (prefix sum `PDelta` exists, INFERRED) | not updated |
| `φ, K, Φ, z, UA` | at close | O(levels), ≤ R/t + 1 | per tick O(levels), cloned / try-catch |
| `κ, close, hit_N` on bar `i − 1` | when bar `i` closes; mark `i − 1` dirty | O(1) | — |
| `IS, Fake, state, a` | at close | O(1) | per tick O(1) on pending inputs |

Everything is computed under the compute lock and copied to the paint snapshot as whole structs (the user's `CopyForPaint`, INFERRED 03 §1.3); the paint thread reads, never computes. Price levels require `IsRequirePriceLevelsCalculation => true` and so a heavier initial calculation than CVD PRO's `Total`-only profile (VERIFIED pattern); keep the footprint optional (setting) with `φ = 0.5` when off.

Degenerate-case summary (one line each): no VA → "—", excluded from windows; `V = 0` → same; `τ = 0` → `τ_min`; session gap → bar excluded from `τ` statistics and `H` reset; first 20 bars → ranks undefined → "loading"; first bar → `ψ = 0`, `κ` unknown; `D = 0` → `s = d`, `η` undefined, `ω = 0`; sentinels in `MaxDelta/MinDelta` → `g = 0`; `PriceLevels` empty → `φ = 0.5`; `H − L < R` on a range chart → forming; unclassified share `u > 0.2` → participation scaled by `1 − u`; forming bar `F < 0.25` → pending.

---

## 8. Sources

Official, VERIFIED (local copies in `research/dl/`, `research/dl2/`, `research/dl/kb/`):
- https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorSpeedOfTape.cs — duration idiom `TicksRight − TicksLeft`, `VolumeAnalysisField` list, tick-aggregation skip.
- https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorDeltaDots.cs — `Total.Trades`, `PriceLevels` enumeration, diagonal neighbour comparison, zero/minimum-volume guards.
- https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorCumulativeDelta.cs — `MaxDelta`/`MinDelta` sentinel checks.
- https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/IndicatorDeltaFlow.cs — `Delta × (High − Low)` precedent.
- https://raw.githubusercontent.com/Quantower/Examples/master/IndicatorExamples/DrawValueAreaForEachBarIndicator.cs — `IsRequirePriceLevelsCalculation => true`, sorted `PriceLevels`.
- Quantower help markdown (github.com/Quantower/QuantowerKB): `analytics-panels/chart/chart-types/range-bars.md`, `tick-chart.md` (pages https://help.quantower.com/quantower/analytics-panels/chart/chart-types/range-bars and …/tick-chart, unreachable today); `quantower-algo/access-volume-analysis-data-from-indicators.md` (Delta / DeltaPercent definitions).

DOCS-EXTRACT (search-engine extract only; pages unreachable today):
- https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisItem.html — `BuyVolume`, `SellVolume` (get/set double), `Delta` (get), `MaxDelta`, `MinDelta` (get/set).
- https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisData.html — `PriceLevels : Dictionary<double, VolumeAnalysisItem>`, `Total`.
- https://api.quantower.com/docs/TradingPlatform.BusinessLayer.HistoryItemBar.html — `TicksRight` (long, overrides `HistoryItem`), `TimeRight`, `Typical`.

INFERRED: `/tmp/claude-0/-home-user-TNX-Levels-Pine/8e550d63-3cbb-5fdc-bfc6-5a88d0326c31/scratchpad/upload/TNX-CVD-PRO/src/TNXIndicators/TNXCVDPro.cs` (`Compute` 1695–1812, `Progress` 1864–1903, `ScaleP/ScaleC/BarRead` 1949–2018, `Verdict` 4134–4190, `LastBarSpeed` 5604–5640, `FlowRank` 6394–6417) and `research/api_surface_from_dll.txt`.

Literature (status as in 02 §4; none could be opened today): Kyle 1985; Cont–Kukanov–Stoikov 2014 (arXiv 1011.6402); Amihud 2002; Lillo–Farmer 2004 (cond-mat/0311053); Hawkes-in-finance survey (arXiv 1502.04592); Andersen–Dobrev–Schaumburg 2008; Easley–López de Prado–O'Hara 2012 (VPIN, rank/CDF convention); vendor footprint conventions (Sierra, ATAS, Jigsaw, Quantower blog) — all SNIPPET.
