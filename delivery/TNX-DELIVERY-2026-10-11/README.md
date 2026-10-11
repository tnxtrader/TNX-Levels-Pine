# TNX delivery · 2026-10-11

Two Quantower indicators. Each folder is self-contained (no TNX-COMMON, no shared files). .NET 10 Windows, TradingPlatform.BusinessLayer 1.146.7.0.

| Folder | What |
|---|---|
| `TNX-FLOW-PRO/` | your CVD PRO with every 2.3 change, under a NEW name: class `TNXFlowPro`, `TNXFlowPro.dll`, listed as "TNX FLOW PRO". Lives next to the old TNX CVD PRO without touching it. `src/TNXIndicators/TNXFlowPro.cs` + `TnxCommon.cs`, `CHANGELOG.md`, `CHANGES-from-CVD-PRO-2.2.diff`, `guide/make_guide.py` |
| `TNX-IMBALANCE-STRENGTH-1.0/` | the new indicator: `src/TNXIndicators/TNXImbalanceStrength.cs`, `README.md` (how to read it, every rule, every setting) |
| `BUILD.cmd` | double-click: builds both and puts the two DLLs into `DLL\` |
| `docs/VERIFICATION.md`, `docs/pictures/`, `docs/research/` | what was checked and how, the test renders, the research reports |
| `test-mono/` | the test harness (optional) |

## Build (one step)

Double-click `BUILD.cmd` (or in CMD: `cd` into this folder, then `BUILD.cmd`). It needs the .NET 10 SDK and Quantower on the PC.
The csproj finds `TradingPlatform.BusinessLayer.dll` in the usual Quantower folders by itself. If it says it cannot find it:

    set QuantowerApi_1_146_7=C:\full\path\TradingPlatform.BusinessLayer.dll
    BUILD.cmd

(the file is in your Quantower folder under `TradingPlatform\v1.146.x\bin`).

## Install

Copy from `DLL\` into Quantower's indicator scripts folder (`Quantower\Settings\Scripts\Indicators\`), one folder per indicator:
`TNXFlowPro\TNXFlowPro.dll` and `TNXImbalanceStrength\TNXImbalanceStrength.dll`. Restart Quantower or reload scripts. Both appear in
the indicator list as TNX FLOW PRO and TNX IMBALANCE STRENGTH. Your old TNX CVD PRO stays as it is.

## Not done on this machine

No Quantower here: both were compiled against a stub generated from your real DLL's API references and Quantower's official examples
(0 errors, 0 warnings) and run on a fake chart (see docs/VERIFICATION.md). If BUILD.cmd fails, send the error lines.
