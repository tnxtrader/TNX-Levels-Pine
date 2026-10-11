# TNX IMBALANCE STRENGTH 1.0

A Quantower indicator (sub-window) that gives every bar one number, the **Imbalance Strength** from −100 to +100, a
**verdict** in one word, and the **acceleration** of that strength. Built for range bars and tick bars; works on time bars.

## Build

Same as TNX CVD PRO: `src/TNXImbalanceStrength.csproj`, .NET 10 Windows, `QuantowerApi_1_146_7` = the path of Quantower's
`TradingPlatform.BusinessLayer.dll`. `dotnet build -c Release`. No TNX-COMMON files needed. Copy the DLL where your other
TNX indicators live. It asks Quantower for the footprint levels (`IsRequirePriceLevelsCalculation = true`), so the first
load takes the volume-analysis calculation (the card says LOADING until it is done).

## What you see

**Top lane: strength.** One bar per chart bar. Up = buyers pushing, down = sellers. Height = how strong (0 to 100).
Brightness = the verdict: bright = REAL, dim = THIN, half = BATTLE, gold outline = ABSORBED. Hollow = the forming bar.
Dotted lines at ±50 = the REAL grade. Small dashes beyond the tip = stacked imbalance levels with the flow (one per level).
Looks (setting "Look"): Bars, Polarized area, Needles + dots, Lollipops (the ball = the acceleration: filled = speeding up,
empty = slowing), Capsules (fill = strength).

**Bottom lane: acceleration.** Is the strength growing (bar up, solid) or falling (bar down, hollow)? In multiples of its
typical change: ±1× = meaningful, ±2× = THRUST (▲ chevron) / STALL (○ amber ring). A grey chevron = a thrust into a bar
that is not REAL (thin or absorbed): don't chase it. A gold dot on the zero line = the side flipped with some strength.

**The card** (top-left by default): the number and the verdict, then every dial behind it:

| Row | Meaning |
|---|---|
| Who | buyers or sellers, how one-sided (delta ÷ traded volume) and its rank in the last 60 bars |
| Busy | volume per second and its rank (how many traders are there) |
| Effort | the bar's delta and its rank (how hard they pushed) |
| Depth taken | volume per range bar (per tick moved on other charts): little = a cheap move, a lot = many resting orders were taken |
| Inside the bar | given back = how much of the delta's largest excursion was undone by the close; wick = how much of the range first traded the other way |
| Persistence | the same-side run and the EWMA of side agreement |
| Stacks | stacked diagonal imbalances with the flow / against it, their share of the bar's volume, "unfinished" when both sides traded at the rejected extreme |
| 4 bars | net flow of the last 4 bars against the ticks they moved (big flow, no move = a wall) |
| Speed | the acceleration in words |
| Last bar | the previous bar's verdict and whether the next bar confirmed it (✓) or rejected it (✗) |
| REAL hit rate | how often a REAL bar was followed by a same-direction bar, over the last 60 REAL bars. On range bars this sits near the base rate of continuation; read it over time, not bar by bar |

## The verdict (one word per bar, in this order; the first true rule wins)

| Verdict | Rule (ranks are against the last 60 real finished bars) | Read it as |
|---|---|---|
| no data | no volume analysis, or fewer than 20 bars | wait |
| THIN | participation rank < 0.35, or (depth-taken rank < 0.35 and participation < 0.6) | nobody there: a cheap move, easy to reverse |
| BATTLE | one-sidedness rank < 0.35, or given back ≥ 50 %, or wick ≥ 50 %, or stacks on both sides | two-sided, no winner yet |
| ABSORBED | the pushing side lost the bar (trapped), or 4-bar flow rank ≥ 0.65 with no net move (wall) | the other side is in control |
| FADING | same side as the bar before, acceleration ≤ −1×, effort still there | the push is dying |
| REAL | everything else; graded by the number | a push that mattered |

## The number

Five dials, each a rank 0..1 (floored at 0.05): participation π, one-sidedness ω, effort e, footprint depth φ (½ weight,
0.5 when there are no levels), persistence ψ (½ weight).

    G  = (π · ω · e · √φ · √ψ) ^ (1/4)                 geometric: a zero in any dial kills the score
    IS = sign · 100 · (G − 0.05)/0.95 · (1 − 0.7 · Fake)

Fake = the largest of five failure modes: trapped (flow lost the bar), wall (4-bar flow without a move), given back
(≥ 50 %), reversed inside (wick ≥ 50 %), thin (1 − participation, when THIN). The sign is who pushes, never the outcome.

Acceleration: the change of IS framed by the side (positive = the current side strengthening), smoothed by an EMA
(setting), divided by the median |change| of the lookback (never under 5 points), capped at ±10×.

## Footprint rule (Quantower's convention)

Levels sorted by price, snapped to the tick. Buy imbalance at p: ask volume at p ≥ ratio × bid volume one tick below
(a missing level counts as 1). Sell imbalance at p: bid volume at p ≥ ratio × ask volume one tick above. Ratio 3 (300 %)
by default, minimum volume on the dominant side = half the bar's average level (or your number), a stack = 3 consecutive
flagged levels. A range bar has at most range + 1 levels, so a 3-stack on an 8-tick bar is a third of the bar.

## Settings worth knowing

- **Lookback** (60): every rank and typical size. Keep it inside one session type; a lookback across the open mixes regimes.
- **Footprint**: ratio, stack length, minimum volume; turn the levels off to skip the heavier calculation (φ = 0.5 then).
- **THIN / BATTLE / wall / REAL** thresholds: ranks and fractions; the defaults are what the test pictures used.
- **Acceleration**: lag, smoothing, meaningful ×, THRUST ×, lane height (0 = off), marks.
- **Card**: corner, rows (full / compact), size, black.
- **Readings follow the forming bar** (on) or the last finished bar (off). LIVE max redraws per second.

## What it is not

It describes the bar that happened and who is in control. The published evidence on trade imbalance says it explains
the current move and forecasts seconds ahead, not minutes. The hit rate row is there so you can measure, on your chart,
how often REAL was followed through. Every threshold is a setting; none of them is fitted to data.
