@echo off
rem Builds both indicators and puts the DLLs into the DLL folder next to this file.
rem Needs the .NET 10 SDK (dotnet) and Quantower installed (the csproj finds TradingPlatform.BusinessLayer.dll by itself;
rem if it cannot, run:  set QuantowerApi_1_146_7=C:\full\path\TradingPlatform.BusinessLayer.dll  and run this again).
setlocal
cd /d "%~dp0"
if not exist DLL mkdir DLL
echo === TNX FLOW PRO ===
dotnet build "TNX-FLOW-PRO\src\TNXFlowPro.csproj" -c Release || goto :fail
copy /Y "TNX-FLOW-PRO\src\bin\Release\net10.0-windows\TNXFlowPro.dll" DLL\ >nul
echo === TNX IMBALANCE STRENGTH ===
dotnet build "TNX-IMBALANCE-STRENGTH-1.0\src\TNXImbalanceStrength.csproj" -c Release || goto :fail
copy /Y "TNX-IMBALANCE-STRENGTH-1.0\src\bin\Release\net10.0-windows\TNXImbalanceStrength.dll" DLL\ >nul
echo.
echo DONE. The DLLs are in: %cd%\DLL
echo Copy each one into its own folder under Quantower\Settings\Scripts\Indicators\  (TNXFlowPro\TNXFlowPro.dll, TNXImbalanceStrength\TNXImbalanceStrength.dll)
pause
exit /b 0
:fail
echo.
echo BUILD FAILED - send the error lines above.
pause
exit /b 1
