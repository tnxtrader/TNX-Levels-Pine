# Quantower API compile-only stub - notes

`toolchain/stubs/api/*.cs` is a COMPILE-ONLY stand-in for `TradingPlatform.BusinessLayer` 1.146.7.0 (net10.0-windows).
It never runs. Its single job is to let `TNXCVDPro.cs` (and sources written against the same API) compile on this Linux box
with the exact member names and signatures the real API has. `toolchain/stubs/common/TnxCommon.cs` stands in for the three
shared helper classes (`GdiCache`, `PictureArrays`, `TnxErrors`) from the TNX-COMMON folder that the csproj links in but which
were not uploaded.

## Result

| Step | Errors | Warnings |
|---|---|---|
| `build_api.sh` (stub itself), first build | 0 | 3 (CS1701 unification notice, 2x CS0414 unused events) |
| `build_api.sh`, final | 0 | 0 |
| `build_indicator.sh upload/TNX-CVD-PRO/src/TNXIndicators/TNXCVDPro.cs`, first attempt | **0** | 1 (CS1701 unification notice only) |
| same, final (scripts now pass the SDK's default `-nowarn:1701,1702`) | **0** | **0** |

The user's source was not modified. There was no error-fixing iteration on the stub: the first compile of the original
source was already clean, because every signature was taken from the IL of the user's own real build (see "How members were
verified").

### Fidelity check (strong)
`ikdasm` was run on the real `upload/.../obj/Release/net10.0-windows/TNXCVDPro.dll` and on the stub-built
`toolchain/out/TNXCVDPro.dll`; every `call/callvirt/newobj/ldfld/...` into `[TradingPlatform.BusinessLayer]` was extracted
with its full signature (`research/extract_refs.py`, outputs `research/refs_real.txt`, `research/refs_stub.txt`).
**All 137 member references are identical** - same parent type, member name, parameter types and return types - including
the arguments the compiler fills in for optional parameters (e.g. `LineSeries.SetValue(float64, int32, SeekOriginHistory)`,
`AddLineSeries(string, Color, int32, LineStyle)`, `IndicatorLineMarker..ctor(Color, IndicatorLineMarkerIconType, IndicatorLineMarkerIconType)`)
and the `InputParameterAttribute` blob (`.ctor(string, int32, float64, float64, float64, int32, object[])`).
The stub-built class header is also identical: `TNXIndicators.TNXCVDPro extends Indicator implements IVolumeAnalysisIndicator`.

### Official examples compiled against the stub (bonus check, `research/qt_examples/*.cs`)
8 of 9 compile with 0 errors: AccessCustomVolumeAnalysisData, AccessSessionsInfoExample, DrawOnBars,
DrawValueAreaForEachBarIndicator, FitDrawingTestIndicator, HistoryExamples, IndicatorChartMouseEvents, SettingsRelationsExample.
`AsynchronousIndicator.cs` fails (2 errors) only on `Core.Loggers.Log(ex)` - a STATIC access. The IL of the user's build proves
`Core.Instance` is typed `Core` and `Core.TimeUtils`/`Core.Instance.Loggers` are INSTANCE members; C# cannot expose a static and an
instance member of the same name on one class, so only the instance form (`Core.Instance.Loggers`, used by
AccessSessionsInfoExample.cs) is stubbed. (The same applies to `Core.TimeUtils` in TestIndicatorWithOneMoreHistoricalData.cs.)
Negative control: a source calling `double d = this.Time(0)` fails as it must (Time returns DateTime).

## Build commands (exact)

```bash
# 1. the stub -> toolchain/out/TradingPlatform.BusinessLayer.dll
toolchain/build_api.sh

# 2. an indicator -> toolchain/out/<name>.dll   (adds stubs/common/TnxCommon.cs unless -nocommon)
toolchain/build_indicator.sh upload/TNX-CVD-PRO/src/TNXIndicators/TNXCVDPro.cs
toolchain/build_indicator.sh [-o NAME] [-nocommon] <source.cs> [more.cs ...]
```
Both scripts print only error lines plus `warnings: N` / `errors: N`; the exit code is csc's (0 = no errors).

What the scripts run (expanded):
```
mono toolchain/roslyn/tasks/net472/csc.exe -nologo -noconfig -nostdlib -target:library -langversion:latest \
  -nullable:disable -warn:4 -nowarn:1701,1702 -optimize+ \
  -define:WINDOWS -define:NET -define:NETCOREAPP -define:NET10_0 -define:NET10_0_OR_GREATER -define:RELEASE \
  -deterministic -debug:portable -out:toolchain/out/TNXCVDPro.dll \
  -r:toolchain/ref/ref/net10.0/<every>.dll \
  -r:toolchain/sdc/lib/net8.0/System.Drawing.Common.dll -r:toolchain/sdc/lib/net8.0/System.Private.Windows.Core.dll \
  -r:toolchain/out/TradingPlatform.BusinessLayer.dll \
  upload/TNX-CVD-PRO/src/TNXIndicators/TNXCVDPro.cs toolchain/stubs/common/TnxCommon.cs
```

### C# features / flags that needed attention
* `-langversion:latest` - the source uses file-scoped namespaces, switch expressions, tuples, `stackalloc` into `Span<int>`,
  static local functions, target-typed `new`, pattern matching; all accepted by this Roslyn.
* `-nullable:disable` - matches the csproj (no `<Nullable>`), avoids CS8xxx noise.
* No `ImplicitUsings` in the csproj, so none are defined; the source has explicit usings.
* `-nowarn:1701,1702` - the only warning left otherwise is CS1701 (System.Drawing.Common 9.0 references System.Runtime 8.0.0.0;
  the net10 ref pack is 10.0.0.0). The .NET SDK suppresses exactly these two by default.
* `-nostdlib -noconfig` + every dll of the net10.0 ref pack: that is what the SDK does for `FrameworkReference Microsoft.NETCore.App`.
* `-define:WINDOWS` etc. are defined to mirror `net10.0-windows`; the source contains no `#if`, so they are inert.
* No `unsafe`, no `AllowUnsafeBlocks` needed.

## How members were verified

Legend (also used as trailing comments in the stub files):
* **VERIFIED(IL)** - read from the IL of the user's real build (`research/tnx_ikdasm.txt`, made with `ikdasm` from
  `upload/.../obj/Release/net10.0-windows/TNXCVDPro.dll`). This is the strongest evidence: the exact memberref signature.
* **VERIFIED(src)** - implied by the user's source `TNXCVDPro.cs` (it compiles against the real API).
* **VERIFIED(ex:File)** - used in an official Quantower example (`research/qt_examples/`) or official Scripts source (`research/dl/`,
  marked "Scripts").
* **ASSUMED** - not seen anywhere; chosen to be harmless.

Notable facts proven by the IL (they contradict a naive reading of the sources):
* `Core.Instance` is `Core`; `Core.TimeUtils` is an instance property -> `Core.TimeUtils`/`Core.Loggers` static forms are impossible.
* `Indicator.Time(int)`, `Ticks(int)`, `GetVolumeAnalysisData(int)` take exactly ONE parameter; `GetPrice(PriceType, int)` two.
  (`SeekOriginHistory` reaches the user's DLL only through the default third argument of `LineSeries.SetValue`.)
* `Ticks(int)` returns `double`; `Time(int)` returns `DateTime`; `Period.Ticks` is `long`; `HistoryAggregationRenko.BrickSize`,
  `RangeBars`, `TicksCount` are `int`.
* `SelectItem.Value` is `IComparable` (not `object`); `SettingItem.Value` is `object`; `InputParameterAttribute.Variants` is `IComparable[]`.
* `SessionsContainer.ActiveSessions` is `ISession[]`; `IChart.Windows` is `IChartWindow[]`.
* `Period`, `Interval<T>` and `TimeZone` are value types (their getters are invoked with `call` on unchecked receivers).
* `Line` (base of `LineSeries`/`LineLevel`) owns `Color`, `Width`, `Visible`; `LineSeries` owns `ShowLineMarker`, `SetValue`, `SetMarker`.
* `Settings` is declared/overridden on `Indicator` (the `base.Settings` calls target `Indicator::get_Settings`).

## Every stubbed member

### namespace TradingPlatform.BusinessLayer - Core.cs
| Member | Status |
|---|---|
| `class Core`; `static Core Instance` | VERIFIED(IL) |
| `Core.TimeUtils : TimeUtils` (instance) | VERIFIED(IL) |
| `Core.Loggers : Loggers` (instance) | VERIFIED(ex:AccessSessionsInfoExample.cs); static form NOT stubbed (see above) |
| `Core.VolumeAnalysis : VolumeAnalysisManager` | member VERIFIED(ex:AccessCustomVolumeAnalysisData.cs); type name ASSUMED |
| `Core.Indicators : IndicatorsManager` | member VERIFIED(ex:AccessIndicatorByName.cs); type name ASSUMED |
| `Core.Symbols : IList<Symbol>` | VERIFIED(ex:HistoryExamples.cs) |
| `class TimeUtils`: `DateTimeUtcNow : DateTime`, `SelectedTimeZone : TimeZone` | VERIFIED(IL) |
| `TimeUtils.ConvertFromUTCToTimeZone`, `ConvertFromTimeZoneToUTC` | names VERIFIED(Scripts); signatures ASSUMED |
| `struct TimeZone`: `TimeZoneInfo : TimeZoneInfo` | VERIFIED(IL); `Id` ASSUMED |
| `class Loggers`: `Log(string, LoggingLevel = Info)`, `Log(Exception, string = null, LoggingLevel = Error)` | VERIFIED(ex + Scripts, all call forms) |
| `enum LoggingLevel { Info, Warning, Error, Trading, System }` | Error, Trading VERIFIED(Scripts); rest ASSUMED |
| `class VolumeAnalysisManager.CalculateProfile(HistoricalData) : IVolumeAnalysisCalculationProgress` | VERIFIED(ex:AccessCustomVolumeAnalysisData.cs) |
| `class IndicatorsManager`: `All : IEnumerable<IndicatorInfo>`, `CreateIndicator(IndicatorInfo)`, `BuiltIn : BuiltInIndicators` | VERIFIED(ex:AccessIndicatorByName.cs, DoubleSMAIndicator.cs) |
| `class IndicatorInfo.Name` | VERIFIED(ex:AccessIndicatorByName.cs) |
| `class BuiltInIndicators.SMA(int, PriceType)` | VERIFIED(ex:DoubleSMAIndicator.cs); `EMA` ASSUMED |

### Indicator.cs
| Member | Status |
|---|---|
| `abstract class ExecutionEntity`: `Name`, `Description` (string, get/set) | VERIFIED(IL) |
| `interface IVolumeAnalysisIndicator`: `bool IsRequirePriceLevelsCalculation {get;}`, `void VolumeAnalysisData_Loaded()` | VERIFIED(src, ex:DrawValueAreaForEachBarIndicator.cs) |
| `enum IndicatorUpdateType { OnTick, OnBarClose }` | OnBarClose VERIFIED(ex:AsynchronousIndicator.cs); OnTick ASSUMED |
| `enum UpdateReason { HistoricalBar, NewBar, NewTick }` | VERIFIED(src; Scripts) |
| `class UpdateArgs.Reason : UpdateReason` | VERIFIED(IL) |
| `class PaintChartEventArgs`: `Graphics`, `Rectangle`, `WindowIndex : int` | VERIFIED(IL); `MousePosition : Point` VERIFIED(ex:IndicatorChartMouseEvents.cs) |
| `abstract class Indicator : ExecutionEntity, IDisposable`; public ctor | VERIFIED(IL) |
| `ShortName` | VERIFIED(ex:SimpleMovingAverage.cs) |
| `SeparateWindow : bool` | VERIFIED(IL) |
| `UpdateType : IndicatorUpdateType` | VERIFIED(ex:AsynchronousIndicator.cs) |
| `CurrentChart : IChart`, `Symbol : Symbol`, `HistoricalData : HistoricalData`, `Count : int` | VERIFIED(IL) |
| `LinesSeries : IList<LineSeries>` | VERIFIED(Scripts) |
| `virtual IList<SettingItem> Settings {get;set;}` | VERIFIED(IL) |
| `AddLineSeries(string = "Line", Color = default, int = 1, LineStyle = Solid) : LineSeries` | 4-arg VERIFIED(IL); optionality VERIFIED(src 2/3 args, ex 0 args) |
| `AddLineLevel(double, string = "Level", Color = default, int = 1, LineStyle = Solid) : LineLevel` | 5-arg VERIFIED(IL) |
| `SetValue(double, int lineIndex = 0, int offset = 0)` | VERIFIED(ex:AsynchronousIndicator.cs `offset:`; DoubleSMAIndicator.cs; Scripts) |
| `GetValue(int offset = 0, int lineIndex = 0) : double` | VERIFIED(ex:AccessIndicatorByName.cs, DoubleSMAIndicator.cs) |
| `GetPrice(PriceType, int = 0) : double` | VERIFIED(IL) |
| `Time(int = 0) : DateTime` | VERIFIED(IL) |
| `Ticks(int = 0) : double` | VERIFIED(IL) |
| `Open/High/Low/Close/Volume/Median(int = 0) : double` | VERIFIED(ex/Scripts) |
| `Typical/Weighted(int = 0)` | ASSUMED |
| `GetVolumeAnalysisData(int = 0) : VolumeAnalysisData` | VERIFIED(IL) |
| `AddIndicator`, `RemoveIndicator`, `Refresh` | VERIFIED(ex:AccessIndicatorByName.cs, UsingInnerIndicatorSettings.cs) |
| `protected virtual OnInit/OnUpdate(UpdateArgs)/OnClear/OnSettingsUpdated` | VERIFIED(IL) |
| `public virtual OnPaintChart(PaintChartEventArgs)` | VERIFIED(IL) |
| `protected virtual bool OnTryGetMinMax(int, int, out double, out double)` | VERIFIED(IL, ex:FitDrawingTestIndicator.cs) |
| `public virtual Dispose()` | VERIFIED(ex:AccessCustomVolumeAnalysisData.cs) |
| `abstract class Strategy : ExecutionEntity` with `virtual Settings` | Settings VERIFIED(ex:SettingsRelationsExample.cs); rest ASSUMED |

### Lines.cs
| Member | Status |
|---|---|
| `enum LineStyle { Solid, Dash, Dot, DashDot, Histogramm, Points, Columns, StepLine }` | VERIFIED(src/ex/Scripts) |
| `enum IndicatorLineMarkerIconType { None, UpArrow, DownArrow, FillCircle, Flag }` | VERIFIED(Scripts) |
| `IndicatorLineMarker(Color, IndicatorLineMarkerIconType upperIcon = None, IndicatorLineMarkerIconType bottomIcon = None)` | VERIFIED(IL + Scripts named args); property names ASSUMED |
| `abstract class Line`: `Color`, `Width : int`, `Visible` | VERIFIED(IL); `Style` VERIFIED(Scripts); `Name` ASSUMED |
| `LineSeries : Line`: `ShowLineMarker`, `SetValue(double, int = 0, SeekOriginHistory = End)`, `SetMarker(int, IndicatorLineMarker)` | VERIFIED(IL) |
| `LineSeries.GetValue(int = 0, SeekOriginHistory = End)`, `SetMarker(int, Color)`, `RemoveMarker(int)` | VERIFIED(Scripts) |
| `LineLevel : Line`; `Level` | type VERIFIED(IL); `Level` ASSUMED |

### Settings.cs
| Member | Status |
|---|---|
| `InputParameterAttribute(string name = null, int sortIndex = 0, double minimum = int.MinValue, double maximum = int.MaxValue, double increment = 0.01, int decimalPlaces = 2, object[] variants = null)` | 7-arg shape VERIFIED(IL); names/optionality VERIFIED(ex: `[InputParameter]`, `("x",10)`, `("x",0,1,1000)`, `minimum:`/`maximum:`/`increment:`/`decimalPlaces:`/`variants:`); defaults VERIFIED(src decompiled values) |
| `.Name : string`, `.Variants : IComparable[]` | VERIFIED(IL) |
| `.SortIndex/.Minimum/.Maximum/.Increment/.DecimalPlaces` | ASSUMED names |
| `enum VisibilityMode { Visible, Hidden }` | Visible VERIFIED(ex:UsingInnerIndicatorSettings.cs) |
| `SelectItem(string, IComparable)`; `Value : IComparable` | ctor VERIFIED(ex); Value type VERIFIED(IL); `Text` ASSUMED |
| `SettingItemRelation`, `SettingItemRelationVisibility(string, params SelectItem[])`, `SettingItemRelationEnability(...)` | VERIFIED(ex:SettingsRelationsExample.cs) |
| `SettingItemSeparatorGroup(string)`, `.Text` | VERIFIED(ex:UsingInnerIndicatorSettings.cs) |
| `SettingItem`: `Name : string`, `Value : object` | VERIFIED(IL) |
| `SettingItem.Text`, `SortIndex`, `Relation`, `SeparatorGroup`, `VisibilityMode` | VERIFIED(ex) |
| `SettingItemInteger(string,int)`, `SettingItemDouble(string,double)`, `SettingItemString`, `SettingItemDateTime` | VERIFIED(ex) |
| `SettingItemBoolean`, `SettingItemColor` | ASSUMED |
| `SettingItemSelectorLocalized(string, object, IList<SelectItem>)` | VERIFIED(ex:SettingsRelationsExample.cs; Scripts) |
| `SettingItemGroup(string, IList<SettingItem>)` | VERIFIED(ex:UsingInnerIndicatorSettings.cs) |
| `static TryGetValue<T>(this IList<SettingItem>, string, out T)`, `GetItemByName(this IList<SettingItem>, string)` | VERIFIED(ex); static class name `SettingItemExtensions` ASSUMED |

### History.cs
| Member | Status |
|---|---|
| `enum SeekOriginHistory { Begin, End }` | VERIFIED(ex; IL default arg) |
| `enum BasePeriod { Tick, Second, Minute, Hour, Day, Week, Month, Year }` | VERIFIED(src/Scripts) |
| `enum HistoryType { Last, BidAsk, Bid, Ask, Mark }` | Last, BidAsk VERIFIED; others ASSUMED |
| `enum PriceType { Open, High, Low, Close, Volume, Median, Typical, Weighted, Ticks, OpenInterest, QuoteAssetVolume, Bid, BidSize, Ask, AskSize, Last }` | VERIFIED(src/ex/Scripts) |
| `enum RenkoStyle { Classic, AdvancedClassic }`, `enum PointsAndFiguresStyle { Close, HighLow }` | AdvancedClassic, HighLow VERIFIED(ex:HistoryExamples.cs); others ASSUMED |
| `readonly struct Period`: `BasePeriod`, `PeriodMultiplier : int`, `Ticks : long` | VERIFIED(IL) |
| `Period.Duration`, static `TICK1, SECOND30, MIN1, MIN5, HOUR1, DAY1, YEAR1` | VERIFIED(ex/Scripts); `SECOND1, MIN15, MIN30, HOUR4, WEEK1, MONTH1`, ctor ASSUMED |
| `abstract class HistoryAggregation`; `.GetHistoryType` | type VERIFIED(IL); `GetHistoryType` VERIFIED(Scripts) |
| `HistoryAggregationTime.Period` | VERIFIED(IL); ctor `(Period, HistoryType)` VERIFIED(Scripts) |
| `HistoryAggregationTick(HistoryType)` | VERIFIED(Scripts) |
| `HistoryAggregationTickBars.TicksCount : int`, `HistoryAggregationRangeBars.RangeBars : int`, `HistoryAggregationRenko.BrickSize : int` | VERIFIED(IL) |
| `HistoryAggregationRenko(Period, HistoryType, int, RenkoStyle, int, int, bool, bool)` | VERIFIED(ex:HistoryExamples.cs); property names ASSUMED |
| `HistoryAggregationPointsAndFigures(Period, HistoryType, int, int, PointsAndFiguresStyle)` | VERIFIED(ex:HistoryExamples.cs); property names ASSUMED |
| `HistoryRequestParameters { Aggregation, FromTime, ToTime, ForceReload }` | VERIFIED(ex:HistoryExamples.cs); `SessionsContainer` VERIFIED(Scripts); `Symbol`, `HistoryType` ASSUMED |
| `interface IHistoryItem : ICloneable`: `TimeLeft`, `this[PriceType]`, `VolumeAnalysisData` | VERIFIED(ex/Scripts); `TimeRight` ASSUMED |
| `HistoryItemBar`: `Open/High/Low/Close/Median/Typical/Weighted/Ticks/Volume : double`, `TimeLeft`, `VolumeAnalysisData` | VERIFIED(ex:HistoryExamples.cs, DrawOnBars.cs, DrawValueAreaForEachBarIndicator.cs); `TimeRight`, `OpenInterest` ASSUMED |
| `HistoryItemTick { Bid, BidSize, Ask, AskSize }`, `HistoryItemLast { Price, Volume }` | VERIFIED(ex:HistoryExamples.cs) |
| `HistoryEventArgs.HistoryItem` | VERIFIED(ex:HistoryExamples.cs) |
| `HistoricalData`: `Aggregation`, `VolumeAnalysisCalculationProgress` | VERIFIED(IL) |
| `HistoricalData.Count`, `this[int]`, `this[int, SeekOriginHistory]`, events `HistoryItemUpdated`/`NewHistoryItem`, `Dispose()` | VERIFIED(ex) |
| `HistoricalData.GetIndexByTime(long, SeekOriginHistory = End) : double` | member VERIFIED(ex/Scripts, result is cast to int); return type ASSUMED |
| `HistoricalData.FromTime` | VERIFIED(Scripts); `ToTime`, `Symbol`, `RemoveIndicator`, `OnHistoryItemUpdated/OnNewHistoryItem` raisers ASSUMED |
| `HistoricalData.AddIndicator(Indicator)` | VERIFIED(ex:DoubleSMAIndicator.cs) |
| `HistoricalDataCustom(Indicator)`, `this[PriceType] {set;}` | VERIFIED(ex:DoubleSMAIndicator.cs) |

### Symbol.cs
| Member | Status |
|---|---|
| `interface ISession.IsPrimary : bool` | VERIFIED(IL) |
| `ISession.Name`, `OpenTime`, `CloseTime`, `ContainsDate(DateTime)` | names VERIFIED(ex:AccessSessionsInfoExample.cs); OpenTime/CloseTime types ASSUMED (TimeSpan) |
| `interface ISessionsContainer.ActiveSessions : ISession[]` | VERIFIED(IL: extension parameter type; ex:AccessSessionsInfoExample.cs) |
| `class SessionsContainer : ISessionsContainer`; `ActiveSessions : ISession[]` | VERIFIED(IL) |
| `static SessionsExtensions.EnumerateSessionTimeFrames(this ISessionsContainer, DateTime, DateTime) : IEnumerable<Interval<DateTime>>` | VERIFIED(IL) |
| `Symbol.TickSize : double`, `CurrentSessionsInfo : SessionsContainer` | VERIFIED(IL) |
| `Symbol.HistoryType`, `FormatQuantity(double)`, `FormatPrice(double)`, `FindSessionsContainer()` | VERIFIED(ex) |
| `Symbol.GetHistory(Period, DateTime[, DateTime])`, `(Period, HistoryType, DateTime[, DateTime])`, `(HistoryAggregation, DateTime[, DateTime])`, `(HistoryRequestParameters)` | VERIFIED(ex:HistoryExamples.cs, AccessCustomVolumeAnalysisData.cs; 4-arg forms Scripts) |
| `Symbol.Name`, `Id`, `LotSize`, `LotStep` | ASSUMED (LotSize/LotStep names seen in Scripts) |

### VolumeAnalysis.cs
| Member | Status |
|---|---|
| `enum VolumeAnalysisCalculationState { NotStarted, Processing, Finished, Aborted }` | Finished VERIFIED(src/ex), Processing VERIFIED(Scripts); others ASSUMED |
| `interface IVolumeAnalysisCalculationProgress.State` | VERIFIED(IL) |
| `.ProgressPercent : int`, `.AbortLoading()` | VERIFIED(ex:AccessCustomVolumeAnalysisData.cs); int ASSUMED |
| `enum VolumeAnalysisField` (17 members) | VERIFIED(Scripts + ex) |
| `VolumeAnalysisItem.Volume/BuyVolume/SellVolume/Delta/MaxDelta/MinDelta : double` | VERIFIED(IL) |
| `.Trades/BuyTrades/SellTrades/AverageSize/AverageBuySize/AverageSellSize/MaxOneTradeVolume/OpenInterest` | names VERIFIED(Scripts); double ASSUMED; `CumulativeDelta` ASSUMED |
| `.GetValue(VolumeAnalysisField) : double` | VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs) |
| `VolumeAnalysisData.Total : VolumeAnalysisItem` | VERIFIED(IL) |
| `VolumeAnalysisData.PriceLevels : Dictionary<double, VolumeAnalysisItem>` | VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs) |

### Chart.cs (namespaces .Chart and .Native)
| Member | Status |
|---|---|
| `enum NativeMouseButtons { None, Left, Right, Middle, XButton1, XButton2 }` | Left, Right VERIFIED(src/ex); others ASSUMED |
| `NativeMouseEventArgs : EventArgs`: `Button`, `Clicks/Delta/X/Y : int`, `Handled` | VERIFIED(IL) |
| `NativeMouseEventArgs.Location : Point` | VERIFIED(ex:IndicatorChartMouseEvents.cs) |
| `ChartMouseNativeEventArgs : NativeMouseEventArgs`: `Window : IChartWindow`, `NeedMouseCapture`, `NeedRedraw` | VERIFIED(IL) |
| `IChartWindowCoordinatesConverter.GetBarIndex(DateTime) : double`, `GetChartX(DateTime) : double`, `GetChartY(double) : double` | VERIFIED(IL) |
| `.GetTime(double) : DateTime`, `.GetPrice(double) : double` | VERIFIED(ex:DrawOnBars.cs, IndicatorChartMouseEvents.cs) |
| `IChartWindow.CoordinatesConverter`, `WindowNumber : int` | VERIFIED(IL) |
| `IChartWindow.ClientRectangle : Rectangle`, `YScaleFactor : double` | VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs) |
| `IChart.RedrawBuffer()`, `BarsWidth : int`, `CurrentTimeZone : TimeZone`, `Windows : IChartWindow[]`, events `MouseDown/MouseMove/MouseUp/MouseWheel : EventHandler<ChartMouseNativeEventArgs>` | VERIFIED(IL) |
| `IChart.MainWindow : IChartWindow` | VERIFIED(ex:DrawOnBars.cs) |
| `IChart.TickSize`, `RightOffset` | names VERIFIED(Scripts); types ASSUMED |

### Utils.cs (namespace TradingPlatform.BusinessLayer.Utils)
| Member | Status |
|---|---|
| `readonly struct Interval<T>`: `From : T`, `To : T` | VERIFIED(IL); ctor ASSUMED |
| `static class Const.DOUBLE_UNDEFINED` | VERIFIED(ex:FitDrawingTestIndicator.cs); namespace and value (NaN) ASSUMED; `INT_UNDEFINED` ASSUMED |

### stubs/common/TnxCommon.cs (namespace TNXIndicators, compiled into the indicator assembly)
Shapes from the IL of the real build (these are TypeDefs inside TNXCVDPro.dll):
| Member | Status |
|---|---|
| `internal sealed class GdiCache`: `BeginPaint()`, `Brush(Color) : SolidBrush`, `Pen(Color, float) : Pen`, `Path(int site) : GraphicsPath` (Reset before return), `Clear()`; consts `MaxEach = 4096`, `GradientSlack = 512` | names/consts VERIFIED(IL); `Pen` parameter list ASSUMED; the two `LinearGradientBrush` helpers of the real class are NOT stubbed (unused by the source) |
| `internal sealed class PictureArrays`: `BeginPaint()`, `Get<T>(int slot, int n) : T[]` (exactly n elements, cached per (slot, n, T)), `Clear()`; const `Slack = 512` | VERIFIED(IL names, slot key `(int,int,Type)`, const); semantics per the source's comment "the same as Array.Resize" |
| `internal static class TnxErrors`: `internal static long Total` (field), `public static void Report(string indicator, string site, Exception error)`, `internal static List<string> Summary()`, `private static bool IsCancellation(Exception)` | VERIFIED(IL: field kind/visibility, method names, first parameter name); Report's 2nd/3rd parameter names ASSUMED |

## Known limitations
* `Core.Loggers` / `Core.TimeUtils` STATIC access (two stale files in the Examples repo, and some Scripts files) cannot be stubbed
  alongside the instance members the user's build demonstrably uses.
* Level 2 / DOM types used by `Level2WithMBOIndicator.cs` (`DepthOfMarketAggregatedCollections`, `GetDepthOfMarketParameters`,
  `Symbol.NewLevel2`, ...) and trading types (`Core.Instance.Positions`, `PlaceOrder`, ...) are out of scope and not stubbed.
* All method bodies are placeholders (return NaN/null/default). The stub DLL must never be deployed to Quantower.
