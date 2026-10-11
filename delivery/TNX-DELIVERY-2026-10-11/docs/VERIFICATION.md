# Verification (2026-10-11)

No Quantower is available on the build machine, so three independent checks were used instead.

## 1. Compile against the real API surface

- The exact Quantower members the original `TNXCVDPro.dll` references were read from its IL (`ikdasm`) and a
  compile-only stub of `TradingPlatform.BusinessLayer` 1.146.7.0 was generated from them (137 member references,
  byte-identical parent type, name, parameter and return types) plus the members used by Quantower's official examples
  (`PriceLevels`, `IsRequirePriceLevelsCalculation`, `GetTime`, `Dispose`, …).
- Both indicators compile with Roslyn 4.12 against the .NET 10.0.12 reference assemblies, System.Drawing.Common 9.0.0
  and that stub: **0 errors, 0 warnings** (`-warn:4`), C# latest, nullable off, same flags as the SDK build.
- The new indicator uses only members that are in the original DLL's reference list or in an official Quantower example
  (listed in the source header). One unverifiable call (`Core.Instance.Loggers`) was removed.

## 2. Run both indicators on simulated range bars (Mono + libgdiplus)

A test harness (`test/Harness.cs`) builds 8-tick range bars from simulated trades (per-price buy/sell levels, running
delta extremes, timestamps), with a balanced base and three situations in a row: a real buying push, an absorption at
the high followed by a drop, a thin fast move followed by a reversal. The real indicator code is driven through
OnInit → OnUpdate (history replay, NewBar, NewTick) → VolumeAnalysisData_Loaded → OnPaintChart with a GDI+ bitmap.

Results (257 bars):

| Check | Result |
|---|---|
| exceptions inside the indicators | none (the cards show an error chip when any are caught: none shown) |
| NaN / infinity in any stored number | 0 of 257 × 29 fields (IS), 0 of 257 × 19 fields (CVD PRO) |
| ranks | all in −1 (not yet 20 bars) or 0..1 |
| Imbalance Strength | −77 .. +97, states: REAL 67, THIN 100, BATTLE 49, ABSORBED 21, no data 20 |
| real push bars (30) | REAL 29, IS 79–88, participation rank 0.9–0.97, stacked levels drawn |
| absorption bars (30) | BATTLE / ABSORBED, IS 6–29 |
| thin move bars (30) | THIN, participation rank 0.31–0.33 |
| final sell-off (17) | REAL sellers, IS −50 .. −61 |
| CVD PRO real-push brightness Q | push 0.77–0.94, absorption 0.42–0.56, thin move 0.14–0.29 |
| acceleration | capped ±10× (IS), lane scale ≤ 6× with cap marks |
| speed | IS: replay 257 bars + 5 ticks 12–19 ms, picture 6–52 ms (first) / 6–11 ms. CVD PRO: replay 2–19 ms, picture ≈ 8 ms |

Pictures: `test/out/IS_*.png` (the five looks) and `test/out/CVD_*.png` (Bars / Area / Histogram / Ribbon / Needles
without the strip / Bars without the strip / the exact 2.2 look / candles).

## 3. Design review against the audit

The audit of the 2.2 source (`docs/research/03_script_audit.md`) listed every setting, where it is read and whether it
works, plus 27 risks. The ones fixed in 2.3 are in the changelog. The ones left alone (with the reason): duplicate sort
indexes (dialog order), the mouse handlers' unlocked view state (one-frame glitches at worst), XMap's extrapolation
(unchanged behaviour), Progress() on time bars using the platform clock (unchanged behaviour).

## What was NOT verified

- Behaviour inside Quantower itself (threads, repaint timing, the dialog's handling of enum settings, the exact
  semantics of `TicksRight` for the forming bar). The code keeps the 2.2 patterns for all of these.
- The thresholds: they order the three test situations correctly; they are not fitted to real data. They are settings.
