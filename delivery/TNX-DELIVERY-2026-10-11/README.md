# TNX delivery · 2026-10-11

Two Quantower indicators, built the same way (.NET 10 Windows, TradingPlatform.BusinessLayer 1.146.7.0, System.Drawing.Common 9.0.0,
`dotnet build -c Release` with `QuantowerApi_1_146_7` = the path of Quantower's TradingPlatform.BusinessLayer.dll).

| Folder | What |
|---|---|
| `TNX-CVD-PRO-2.3/` | your CVD PRO with the 2.3 changes: `src/TNXIndicators/TNXCVDPro.cs` (full file), the unchanged csproj (still links `..\..\TNX-COMMON\*.cs`), `guide/make_guide.py` with a 2.3 section, `CHANGELOG-2.3.md`, and `CHANGES-2.2-to-2.3.diff` (every line that changed) |
| `TNX-IMBALANCE-STRENGTH-1.0/` | the new indicator: `src/TNXIndicators/TNXImbalanceStrength.cs`, its csproj (self-contained, no TNX-COMMON), `README.md` (how to read it, every rule and formula, every setting) |
| `docs/VERIFICATION.md` | what was checked and how (compile against the real API surface, a Mono run on simulated range bars, the numbers) |
| `docs/pictures/` | the test renders: `IS_*.png` (the five looks of the new indicator), `CVD_*.png` (the flow lane looks, with and without the strip, the exact 2.2 look) |
| `docs/research/` | the research reports the design came from (Quantower API, imbalance literature, the audit of 2.2, range bars and platform conventions, the measures) |
| `test-mono/` | the test harness and the compile-only API stub, if you want to re-run the test (Mono + libgdiplus, see VERIFICATION.md) |

## Install

1. Build each project. Copy `TNXCVDPro.dll` over your current one (same name, same settings keys: your saved workspaces keep their values; the
   new 2.3 settings start at their defaults). Copy `TNXImbalanceStrength.dll` next to it.
2. CVD PRO 2.3 defaults: brightness = real push, THRUST gated, early fade on, acceleration inside the flow lane when the strip is off,
   compass in the card, signal filter with the all-history fallback. "2.3 Flow lane look" = Bars. Set every 2.3 setting to its "Classic"/off
   value to get the 2.2 picture back.
3. TNX IMBALANCE STRENGTH opens in its own sub-window with the acceleration lane under the strength lane and the card top-left. The first
   load asks Quantower for the footprint levels (LOADING on the card until done).

## Not done on this machine

No Quantower here: the DLLs were compiled against a stub generated from your real DLL's references and Quantower's official examples
(0 errors, 0 warnings) and run on a fake chart. Please build on your machine and load both on a range chart; if the build complains about
one member, send the error line.
