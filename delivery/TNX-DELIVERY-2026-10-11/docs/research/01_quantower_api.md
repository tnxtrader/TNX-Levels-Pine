# Quantower indicator API — research notes

Target: TradingPlatform.BusinessLayer 1.146.7.0 (the version the user's DLL compiles against). Written 2026-10-11.

## 0. Sources and status legend

| Status | Meaning |
|---|---|
| VERIFIED | Read in an official Quantower source: the API reference (api.quantower.com, reached only through search-engine snippets of the actual pages; WebFetch cannot resolve the host), the official help text (github.com/Quantower/QuantowerKB = markdown source of help.quantower.com), or official example code (github.com/Quantower/Examples, github.com/Quantower/Scripts; files carry "Copyright QUANTOWER LLC" and the built-in indicators' `SourceCodeLink` points at Quantower/Scripts). |
| INFERRED | Deduced from the user's compiling source (`TNXCVDPro.cs`) and the member dump of the compiled DLL (`api_surface_from_dll.txt`). A member listed there exists with that name in 1.146.7; its type is inferred from how the code uses it. |
| DECOMPILED | Seen only in the third-party repository github.com/PickReaper/quantower ("detail CS files that are used by the quantower API DLL's"; obfuscated decompile of an OLDER build: it lacks `SessionsExtensions.EnumerateSessionTimeFrames` and `PaintChartEventArgs.LeftVisibleBarIndex`, both of which exist in 1.146.7). Treat as UNVERIFIED but concrete. |
| UNVERIFIED | Not confirmed anywhere. |

Environment note: api.quantower.com, help.quantower.com, help.ampfutures.com, web.archive.org, api.optimusflow.app and context7.com could not be fetched (DNS/proxy). Everything from the API reference below comes from search-engine extracts of the named page; everything else was downloaded in full from raw.githubusercontent.com. Local copies: `scratchpad/research/dl/` (official code), `scratchpad/research/dl/kb/` (help markdown), `scratchpad/research/dl/decomp/` (decompile).

URL abbreviations used below:
- DOCS = https://api.quantower.com/docs/
- KB = https://raw.githubusercontent.com/Quantower/QuantowerKB/master/quantower-algo/
- EX = https://raw.githubusercontent.com/Quantower/Examples/master/IndicatorExamples/
- SCR = https://raw.githubusercontent.com/Quantower/Scripts/master/Indicators/
- DEC = https://raw.githubusercontent.com/PickReaper/quantower/master/

---

## 1. Indicator lifecycle and chart access

### 1.1 Class and constructor
- `public abstract class Indicator : ExecutionEntity` in `TradingPlatform.BusinessLayer`. VERIFIED — DOCS TradingPlatform.BusinessLayer.Indicator.html. (DECOMPILED adds `, IIndicatorsCollection`.)
- `Name`, `Description`, `ShortName` are set in the constructor; `ShortName` is a `public virtual string` meant to be overridden as a getter (official code: `public override string ShortName => $"Time histogram ({this.FieldType})";`). VERIFIED — KB simple-indicator.md, SCR IndicatorTimeHistogram.cs. `set_Name`/`set_Description` live on `ExecutionEntity` (DLL dump). INFERRED.
- `HelpLink` and `SourceCodeLink` are `public virtual string` getters (official scripts override them). VERIFIED — SCR IndicatorDeltaFlow.cs.
- Lines are declared in the constructor with `AddLineSeries(...)`; `SeparateWindow = true/false` chooses sub-window vs main window. VERIFIED — KB simple-indicator.md.

### 1.2 OnInit / OnUpdate / OnClear / OnSettingsUpdated / Refresh / Dispose
- `protected virtual void OnInit()` — "This function will be called after creating an indicator as well as after its input params reset or chart (symbol or timeframe) updates." VERIFIED — DOCS Indicator.html (declared protected virtual, no parameters); comment text from KB simple-moving-average-example.md and EX IndicatorChartMouseEvents.cs. DECOMPILED: `Init()` wraps `OnInit()` in try/catch and logs exceptions via `Core.Instance.Loggers`.
- `protected virtual void OnUpdate(UpdateArgs args)` — "Calculation entry point. This function is called when a price data updates. Will be runing under the HistoricalBar mode during history loading. Under NewTick during realtime. Under NewBar if start of the new bar is required." VERIFIED — KB simple-moving-average-example.md (verbatim comment), DOCS Indicator.html.
  - `UpdateArgs.Reason` (`UpdateReason`) VERIFIED — used as `args.Reason == UpdateReason.HistoricalBar / NewTick / NewBar` in SCR IndicatorCumulativeDelta.cs, IndicatorHighLow.cs, IndicatorDeltaRotation.cs, IndicatorPriceActionZones.cs, IndicatorTradeSurge.cs. `UpdateArgs.MessageQuote` (`MessageQuote`, the quote that caused the update) DECOMPILED — DEC UpdateArgs.cs.
  - `enum UpdateReason { Unknown, HistoricalBar, NewTick, NewBar }` with doc comments "processing of new historical bar" / "coming of new real-time tick" / "start of new real-time bar". DECOMPILED — DEC UpdateReason.cs; the three named values VERIFIED by official usage above.
  - DECOMPILED detail (DEC Indicator.cs `Update`): the wrapper increments the indicator's `Count` when the reason implies a new bar, then calls `OnUpdate`; an exception thrown by `OnUpdate` is caught and logged (the chart survives, the update is lost).
  - DECOMPILED delivery order on a new live bar (DEC HistoricalData.cs `AddNewItem`): indicators with `UpdateType == OnBarClose` are updated (reason `NewBar`) BEFORE the new item is appended; the item is appended; indicators with `UpdateType == OnTick` are updated (reason `NewBar`); then `NewHistoryItem` fires. Every later tick of the forming bar replaces item [0] and sends `NewTick` to `OnTick` indicators.
  - Official "is this a new bar" idiom, because a pure tick chart has one bar per tick: `bool isNewBar = this.HistoricalData.Aggregation.GetPeriod == Period.TICK1 ? args.Reason == UpdateReason.NewTick : args.Reason == UpdateReason.NewBar;` VERIFIED — SCR IndicatorHighLow.cs, IndicatorDeltaRotation.cs. (`GetPeriod` is marked Obsolete in DEC HistoryAggregation.cs; prefer `Aggregation is HistoryAggregationTick`.)
- `protected virtual void OnClear()` — VERIFIED — DOCS Indicator.html. Used to remove child indicators, unsubscribe events, abort tasks (KB using-clouds-in-indicator.md, SCR IndicatorDeltaFlow.cs, IndicatorCumulativeDelta.cs). DECOMPILED: `Clear()` empties every line buffer, sets `Count = 0`, then calls `OnClear()`; it runs on `Refresh()` and on chart/symbol/timeframe changes.
- `protected override void OnSettingsUpdated()` — exists on `Indicator` (DLL dump: `Indicator -> OnSettingsUpdated`; the user overrides it and calls `base.OnSettingsUpdated()`). INFERRED. DECOMPILED body: `Refresh();`. Official scripts call `this.OnSettingsUpdated()` themselves at the end of their `Settings` setter (SCR IndicatorAutoTrendLine.cs, IndicatorBarCounter.cs, IndicatorGaps.cs, IndicatorMaxVolume.cs, IndicatorPinBar.cs, IndicatorSpeedOfTape.cs). VERIFIED that it is callable; not found on the DOCS page extract (UNVERIFIED in docs — it is probably declared on `ExecutionEntity`).
- `public void Refresh()` — "Recalculate indicator" (DEC doc comment). Official code calls it after a setting changed and from `VolumeAnalysisData_Loaded`. VERIFIED existence — SCR IndicatorCumulativeDelta.cs, IndicatorDeltaFlow.cs, IndicatorVolume.cs. DECOMPILED: it asks the owning `HistoricalData` to re-run the indicator (clear + replay of all bars with `HistoricalBar`), i.e. expect `OnClear` then N × `OnUpdate(HistoricalBar)`.
- `public override void Dispose()` — override to unsubscribe chart mouse events and dispose GDI objects, then `base.Dispose()`. VERIFIED — EX IndicatorChartMouseEvents.cs, SCR IndicatorLevel2.cs. DECOMPILED: base sets `HistoricalData = null`.
- `UpdateType` (`IndicatorUpdateType.OnTick | OnBarClose`) settable in the constructor; `IsUpdateTypesSupported` (protected bool) hides the selector. VERIFIED — EX AsynchronousIndicator.cs (`this.UpdateType = IndicatorUpdateType.OnBarClose;`), SCR IndicatorDeltaFlow.cs (`this.IsUpdateTypesSupported = false;`).
- Other flags (DECOMPILED, DEC Indicator.cs): `OnBackGround` (draw behind bars), `AllowFitAuto` (take part in price auto-scale), `Digits` (−1 = symbol precision), `TFConfig`.

### 1.3 Settings, SettingItem, SelectItem
- `public override IList<SettingItem> Settings { get; set; }` — override pattern: getter = `var settings = base.Settings; settings.Add(new SettingItemColor("BuyColor", this.buyColor) { Text = "Buy Color" }); return settings;`; setter = `base.Settings = value;` then read back by name with `value.GetItemByName("HistogramColors") is SettingItemPairColor si` or `value.TryGetValue("BuyColor", out Color c)`. VERIFIED — SCR IndicatorDeltaDivergenceReversal.cs, IndicatorDeltaFlow.cs, IndicatorSpeedOfTape.cs. The DLL dump shows `Indicator.get_Settings/set_Settings` and `SettingItem.get_Name/get_Value`. INFERRED.
- SettingItem members (DECOMPILED, DEC SettingItem.cs): `string Name`, `virtual object Value`, `virtual string Text`, `int SortIndex`, `VisibilityMode VisibilityMode`, `string Description`, `SettingItemGroup Group`, `SettingItemSeparatorGroup SeparatorGroup`, `bool Enabled`, `bool Visible`, `SettingItemRelation Relation`, event `Updated`. Concrete types used by official code: `SettingItemColor`, `SettingItemBoolean`, `SettingItemInteger`, `SettingItemDouble`, `SettingItemSelectorLocalized(name, value, List<SelectItem>)`, `SettingItemPairColor`, `SettingItemSeparatorGroup(text, sortIndex)`, `SettingItemRelationVisibility`. VERIFIED — SCR IndicatorSpeedOfTape.cs, IndicatorDeltaDots.cs.
- `SelectItem`: `Text`, `Value` (`IComparable`), constructors `(string text, IComparable value = null)`, `(string text, int value)`, `(string text, string value)`; equality by `Value`. DECOMPILED — DEC SelectItem.cs. `new SelectItem("Volume", VolumeAnalysisField.Volume)` VERIFIED — SCR IndicatorSpeedOfTape.cs. A selector setting's `Value` arrives as a `SelectItem`; unwrap with `((SelectItem)item.Value).Value` (DECOMPILED, DEC HistoryAggregationRangeBars.cs setter; the user's setter does the same).
- Base `Indicator.Settings` getter (DECOMPILED): per LineSeries a group "Line N" with Visible, line options (color/width/style), TimeShift, ShowLineMarker; per LineLevel the same plus Level; a time-frame config item; an "update type" selector when `IsUpdateTypesSupported`. The base setter writes those back; `[InputParameter]` fields/properties are handled by `ExecutionEntity`.

### 1.4 InputParameter attribute
- Declaration: `[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)] public class InputParameterAttribute : Attribute`. Single constructor: `InputParameterAttribute(string name = "", int sortIndex = 0, double minimum = -2147483648.0, double maximum = 2147483647.0, double increment = 0.01, int decimalPlaces = 2, object[] variants = null)`. Read-only properties `Name, SortIndex, Minimum, Maximum, Increment, DecimalPlaces, Variants (IComparable[])`. VERIFIED — DOCS TradingPlatform.BusinessLayer.InputParameterAttribute.html (one constructor: name, sort index, minimum, maximum, increment, decimal places, variants; defaults "very wide min/max, increment 0.01, 2 decimal places, no variants"; properties read-only). Exact signature text DECOMPILED — DEC InputParameterAttribute.cs. DLL dump: `get_Name`, `get_Variants`. INFERRED.
- Constructor forms seen in official code (all VERIFIED): `[InputParameter]` (EX FitDrawingTestIndicator.cs); `[InputParameter("Text")]`, `[InputParameter("First text", 1)]` (KB input-parameters.md); `[InputParameter("Double", 0, 0.00001, 10, 0.00001, 5)]` (KB input-parameters.md); `[InputParameter("Period of Simple Moving Average", 0, 1, 999, 1, 1)]` (KB simple-moving-average-example.md); `[InputParameter("Value area", minimum: 0, maximum: 100)]` (EX DrawValueAreaForEachBarIndicator.cs); `[InputParameter("Sources prices for MA", 1, variants: new object[]{ "Close", PriceType.Close, "Open", PriceType.Open, ... })]` (KB).
- Variants list = flat `object[]` of alternating display text and value; it is what turns an enum (or any value set) into a drop-down. VERIFIED — KB input-parameters.md ("List with options").
- Supported member types: string, bool, int, double, long, decimal, Symbol, Account, DateTime, Color, lists via variants (KB input-parameters.md), `Period` (KB access-to-advanced-aggregations.md `[InputParameter("Renko period", 20)] public Period RenkoPeriod`). VERIFIED. Works on properties (`public int ValueAreaPercent { get; set; } = 70;`) and fields. VERIFIED.

### 1.5 Line series, line levels, markers
- `public LineSeries AddLineSeries(string lineName = "", Color lineColor = default, int lineWidth = 1, LineStyle lineStyle = LineStyle.Solid)` and `public void AddLineSeries(LineSeries)`. Signature DECOMPILED — DEC Indicator.cs; both `AddLineSeries()` and the 4-argument form VERIFIED — EX AsynchronousIndicator.cs, KB simple-indicator.md. Returns the `LineSeries` (the user keeps the return value; INFERRED). Lines are also reachable as `LinesSeries[i]`. VERIFIED — KB using-markers-with-indicators.md.
- `public LineLevel AddLineLevel(double level, string lineName = "", Color lineColor = default, int lineWidth = 1, LineStyle lineStyle = LineStyle.Solid)`. DECOMPILED — DEC Indicator.cs; `AddLineLevel(0.0, "zero", color, 1, LineStyle.Dash)` compiles in the user's code and `Indicator.AddLineLevel` is in the DLL dump. INFERRED. `LineLevel.Level` (double) DECOMPILED.
- `Line` (base of `LineSeries`/`LineLevel`): `Name`, `Color`, `int Width`, `LineStyle Style`, `bool Visible`. DECOMPILED — DEC Line.cs; `set_Color`, `set_Width`, `set_Visible` in the DLL dump. INFERRED.
- `LineSeries`: `SetValue(double value, int offset = 0, SeekOriginHistory origin = End)`, `GetValue(int offset = 0, SeekOriginHistory origin = End)`, indexer `this[int offset = 0, SeekOriginHistory origin = End]`, `SetMarker(int offset, Color color)`, `SetMarker(int offset, IndicatorLineMarker)`, `RemoveMarker(int offset)`, `GetMarker(int offset)`, `ClearMarkers()`, `SetLineBreak/RemoveLineBreak/GetLineBreak(int offset)`, `bool ShowLineMarker` (default true; the price-scale label), `int TimeShift`, `int DrawBegin`. DECOMPILED — DEC LineSeries.cs. VERIFIED subset: `SetMarker` both overloads (KB using-markers-with-indicators.md), `RemoveMarker` (SCR IndicatorDeltaDivergenceReversal.cs, IndicatorSpeedOfTape.cs), `SetValue/SetMarker/set_ShowLineMarker` in DLL dump (INFERRED).
  - `SetMarker`/`RemoveMarker` throw `ArgumentOutOfRangeException` when `offset >= buffer length` (DECOMPILED). Official guard: `if (this.Count <= markerOffset) return;` (SCR IndicatorTimeHistogram.cs).
  - Indicator-level helpers: `SetValue(double value, int lineIndex = 0, int offset = 0)` (note the argument order differs from `LineSeries.SetValue`), `GetValue(int offset = 0, int lineIndex = 0, SeekOriginHistory origin = End)`. VERIFIED — KB simple-indicator.md. Offset 0 = the newest bar the indicator has processed; `SetValue(v, 1, 5)` = line 1, five bars back.
- `IndicatorLineMarker(Color color, IndicatorLineMarkerIconType upperIcon = None, IndicatorLineMarkerIconType bottomIcon = None)`; properties `Color`, `UpperIcon`, `BottomIcon`. VERIFIED — KB using-markers-with-indicators.md (`new IndicatorLineMarker(Color.Yellow, IndicatorLineMarkerIconType.Flag)`, `new IndicatorLineMarker(Color.Green, bottomIcon: IndicatorLineMarkerIconType.UpArrow)`); DEC IndicatorLineMarker.cs. `enum IndicatorLineMarkerIconType { None, UpArrow, DownArrow, Flag, FillCircle }` DECOMPILED (UpArrow/DownArrow/Flag VERIFIED by KB).
- `enum LineStyle { Solid, Dash, Dot, DashDot, Histogramm, Points, Columns, StepLine }` DECOMPILED — DEC LineStyle.cs; `Solid`, `Histogramm`, `Points` VERIFIED in official code; `Dash` INFERRED (user).
- Clouds: `protected void BeginCloud(int line1Index, int line2Index, Color color, int offset = 0)` / `EndCloud(...)`. VERIFIED — KB using-clouds-in-indicator.md.
- Bar colouring: `SetBarColor(Color? color = null, int offset = 0)`, `SetBarAppearance(IndicatorBarAppearance, int offset = 0)`. DECOMPILED; `SetBarColor` VERIFIED — SCR IndicatorSpeedOfTape.cs.

### 1.6 Buffers, prices, time
- `public int Count` — "Amount of items in internal buffers". VERIFIED — KB simple-indicator.md (`if (Count <= Period) return;`). During history replay `Count` lags `HistoricalData.Count` (official code waits `while (this.Count != this.HistoricalData.Count) Thread.Sleep(20);` — SCR IndicatorTimeHistogram.cs). VERIFIED.
- `protected double GetPrice(PriceType priceType, int offset = 0)` — VERIFIED — DOCS Indicator.html, KB simple-indicator.md (`GetPrice(PriceType.Volume, 5)` = fifth bar before current). DECOMPILED body: `HistoricalData[Count - 1 - offset, SeekOriginHistory.Begin][priceType]` — offsets are relative to the indicator's own `Count`, so during replay offset 0 is the bar being processed, not the last loaded bar.
- Shorthands `protected double Open/High/Low/Close/Median/Typical/Weighted/Volume/QuoteAssetVolume/Ticks/OpenInterest/FundingRate/Bid/Ask/Last(int offset = 0)` and `protected DateTime Time(int offset = 0)`. DECOMPILED — DEC Indicator.cs. VERIFIED subset: `Open()/Close()/High()/Low()/Open(5)` (KB), `Median()` (EX AsynchronousIndicator.cs), `Volume()`, `QuoteAssetVolume()`, `Ticks()` (SCR IndicatorVolume.cs), `Time()` (EX FitDrawingTestIndicator.cs, SCR IndicatorHighLow.cs).
- `Ticks(offset)` = `GetPrice(PriceType.Ticks, offset)` = `HistoryItemBar.Ticks` ("Defines ticks amount") = number of ticks inside the bar. DECOMPILED; `Indicator.Ticks` in DLL dump (INFERRED); official use as volume fallback VERIFIED (SCR IndicatorVolume.cs).
- `Time(offset)` = `new DateTime(item.TicksLeft, DateTimeKind.Utc)` = the bar's LEFT border (open time) in UTC. DECOMPILED. Official code passes `Time()` to `GetChartX`, which takes the bar's left time (VERIFIED EX DrawOnBars.cs uses `bar.TimeLeft`). Consistent.
- `enum PriceType { Open, High, Low, Close, Median, Typical, Weighted, Bid, BidSize, Ask, AskSize, Last, Volume, Ticks, AggressorFlag, TickDirection, BidTickDirection, AskTickDirection, OpenInterest, Mark, FundingRate, QuoteAssetVolume }`. DECOMPILED — DEC PriceType.cs; Close/Open/High/Low/Typical/Median/Weighted/Volume VERIFIED (KB).
- `enum SeekOriginHistory { Begin, End }`. DECOMPILED; both VERIFIED in official usage.

### 1.7 HistoricalData
- `public HistoricalData HistoricalData` — "Represent access to current used historical data." VERIFIED — DOCS Indicator.html (search extract), all official examples.
- `int Count`; indexer `IHistoryItem this[int offset, SeekOriginHistory origin = SeekOriginHistory.End]` → `HistoricalData[0]` is the NEWEST bar, `HistoricalData[i, SeekOriginHistory.Begin]` is absolute index from the oldest bar. VERIFIED — KB access-volume-analysis-data-from-indicators.md (`HistoricalData[0].VolumeAnalysisData.Total` for the current bar), EX DrawOnBars.cs (`this.HistoricalData[i, SeekOriginHistory.Begin] is HistoryItemBar bar`), SCR IndicatorDeltaDivergenceReversal.cs (`[offset]` current, `[offset + 1]` previous). DECOMPILED: `Count` locks `itemsLocker`; the indexer getter does NOT lock → bounds-check against `Count` from the paint thread.
- `HistoryAggregation Aggregation` — the chart's aggregation. VERIFIED — SCR IndicatorSpeedOfTape.cs (`this.HistoricalData.Aggregation is HistoryAggregationTime aggregationTime`), IndicatorCumulativeDelta.cs; DLL dump `get_Aggregation` (INFERRED).
- `IVolumeAnalysisCalculationProgress VolumeAnalysisCalculationProgress` — null until a calculation was started. VERIFIED — KB access-volume-analysis-data-from-indicators.md; official code null-checks it (SCR IndicatorTimeHistogram.cs).
- Events: `NewHistoryItem` ("fires when a new historical item is created"), `HistoryItemUpdated` ("fires when the current historical item changes or updates"), `HistoryItemVolumeAnalysisUpdated` ("fires when a bar's volume analysis changes"). VERIFIED — DOCS TradingPlatform.BusinessLayer.HistoricalData.html (search extract); `NewHistoryItem += (object sender, HistoryEventArgs e)` VERIFIED — KB access-to-advanced-aggregations.md. DECOMPILED types: `HistoryEventHandler` for the first two (`HistoryEventArgs { IHistoryItem HistoryItem; bool ResetVolumeAnalysisRequired }`), `Action` for the third.
- `IVolumeAnalysisCalculationProgress CalculateVolumeProfile(VolumeAnalysisCalculationParameters)` VERIFIED — DOCS HistoricalData.html, SCR IndicatorCumulativeDelta.cs. `AddIndicator(Indicator)`, `RemoveIndicator(Indicator)`, `Dispose()` VERIFIED — KB using-clouds-in-indicator.md, access-to-advanced-aggregations.md. `GetPrice(PriceType, int offset)` on HistoricalData VERIFIED — SCR IndicatorDeltaDots.cs. `Symbol`, `FromTime`, `ToTime`, `GetIndexByTime(long time, SeekOriginHistory)` DECOMPILED.
- `IHistoryItem`: `DateTime TimeLeft`, `long TicksLeft`, `long TicksRight`, `double this[PriceType]`, `VolumeAnalysisData VolumeAnalysisData`, `SetData/TryGetData`. DECOMPILED — DEC IHistoryItem.cs; `TimeLeft`, `TicksLeft`, `TicksRight`, `VolumeAnalysisData` VERIFIED by official usage (SCR IndicatorSpeedOfTape.cs, IndicatorSlippage.cs, EX DrawValueAreaForEachBarIndicator.cs).

### 1.8 Symbol
- `public Symbol Symbol` = `HistoricalData?.Symbol`. VERIFIED usage everywhere; DECOMPILED definition.
- `double TickSize` — "Gets cached tick size if it available, else tries to obtain GetTickSize(price) with Last, Bid, Ask, first element of VariableTick list otherwise DOUBLE_UNDEFINED" (DEC doc comment). VERIFIED usage — EX DrawValueAreaForEachBarIndicator.cs (`Symbol.TickSize * mainWindow.YScaleFactor / 2.0`). Can be NaN (variable-tick symbols) — guard. `GetTickSize(double price)`, `RoundPriceToTickSize(price)` DECOMPILED. Note `CurrentChart.TickSize` also exists (see 1.9) and official painting code prefers it.
- `SessionsContainer CurrentSessionsInfo` — DLL dump `Symbol.get_CurrentSessionsInfo`; DECOMPILED: returns the symbol's container or `Exchange?.CurrentSessionsInfo`; may be null. INFERRED.
- `FormatPrice(double)`, `FormatQuantity(double, bool inLots = true, bool abbreviate = false)` VERIFIED — EX IndicatorChartMouseEvents.cs, AccessCustomVolumeAnalysisData.cs. `GetHistory(Period, HistoryType, DateTime from, DateTime to = default)`, `GetHistory(HistoryRequestParameters)` VERIFIED — KB downloading-history.md, access-to-advanced-aggregations.md. `event LastHandler NewLast` VERIFIED — SCR IndicatorSlippage.cs. `MinVolumeAnalysisTickSize` DECOMPILED.

### 1.9 CurrentChart (IChart), IChartWindow, coordinates converter
- `public IChart CurrentChart` — "Represent access to the chart, that created indicator"; namespace `TradingPlatform.BusinessLayer.Chart`. VERIFIED — DOCS Indicator.html, DOCS TradingPlatform.BusinessLayer.Chart.IChart.html. May be null (watchlist/strategy host) — official code checks `if (this.CurrentChart == null) return;` (EX DrawOnBars.cs). VERIFIED.
- IChart members, VERIFIED — DOCS IChart.html (search extracts): `IChartWindow[] Windows`; `IChartWindow MainWindow` ("Main window of the chart"); `int BarsWidth` ("Current X scale value - width of the bar in pixels"); `void RedrawBuffer()` ("forces the chart to redraw"); `void Refresh()`; `TimeZone CurrentTimeZone` ("time zone of the current chart", read-only); `ISessionsContainer CurrentSessionContainer` ("Provides custom sessions of current chart"); `int RightOffset` ("Current right offset value"); `double TickSize` ("Current tick size of the chart"); `Drawings`; `ID`; events `MouseClick, MouseDown, MouseUp, MouseMove, MouseEnter, MouseLeave, MouseWheel`, all `EventHandler<ChartMouseNativeEventArgs>`. Official usage: `CurrentChart.MainWindow`, `.BarsWidth`, `.Windows[args.WindowIndex]`, `.TickSize`, `.RightOffset`, `.CurrentSessionContainer`, `.CurrentTimeZone`, `.MouseDown += / -=`, `.MouseUp` (SCR and EX files listed in section 8). DLL dump: `RedrawBuffer, add/remove_MouseDown/MouseMove/MouseUp/MouseWheel, get_BarsWidth, get_CurrentTimeZone, get_Windows` (INFERRED).
- Subscribe to mouse events in `OnInit`, unsubscribe in `Dispose` (guard `CurrentChart != null`). VERIFIED — EX IndicatorChartMouseEvents.cs.
- IChartWindow, VERIFIED — DOCS TradingPlatform.BusinessLayer.Chart.IChartWindow.html: `int WindowNumber`; `Rectangle ClientRectangle`; `IChartWindowCoordinatesConverter CoordinatesConverter`; `double YScaleFactor` (undocumented text; by official usage it is pixels per price unit: `MainWindow.YScaleFactor * CurrentChart.TickSize` = one tick in pixels, SCR IndicatorLevel2.cs; `Symbol.TickSize * YScaleFactor / 2.0`, EX DrawValueAreaForEachBarIndicator.cs); `bool IsMainWindow`. DLL dump `get_WindowNumber`, `get_CoordinatesConverter` (INFERRED).
- IChartWindowCoordinatesConverter, VERIFIED — DOCS TradingPlatform.BusinessLayer.Chart.IChartWindowCoordinatesConverter.html: `double GetBarIndex(DateTime)`, `double GetChartX(DateTime)`, `double GetChartY(double price)`, `double GetPrice(double y)`, `DateTime GetTime(double x)`. Semantics by official usage (VERIFIED): `GetChartX(bar.TimeLeft)` = left pixel edge of that bar, bar centre = `+ CurrentChart.BarsWidth / 2` (EX DrawOnBars.cs); `GetBarIndex(time)` = fractional absolute index (origin Begin), visible range = `(int)GetBarIndex(GetTime(ClientRectangle.Left))` … `(int)Math.Ceiling(GetBarIndex(GetTime(ClientRectangle.Right)))`, each then checked `i > 0 && i < HistoricalData.Count` (EX DrawOnBars.cs, DrawValueAreaForEachBarIndicator.cs); `GetPrice(mouseY)` (EX IndicatorChartMouseEvents.cs).
- Which window to use: official code uses `this.CurrentChart.Windows[args.WindowIndex].CoordinatesConverter` (EX FitDrawingTestIndicator.cs, SCR IndicatorDeltaDots.cs, IndicatorTradeSurge.cs, IndicatorDeltaRotation.cs) or `CurrentChart.MainWindow` for main-window overlays. VERIFIED. The user matches `Windows[k].WindowNumber == args.WindowIndex`, which is equivalent if `Windows` is ordered by `WindowNumber` (UNVERIFIED but both forms compile and run in official/user code).

### 1.10 PaintChartEventArgs and OnPaintChart
- `public virtual void OnPaintChart(PaintChartEventArgs args)` — override, call `base.OnPaintChart(args)` first (official examples do). VERIFIED — KB indicator-with-custom-painting-gdi.md, EX files. DECOMPILED: base body is empty; the chart calls `PaintChart(ev)` which wraps `OnPaintChart` in try/catch and logs — an exception aborts only that paint.
- `PaintChartEventArgs` (namespace `TradingPlatform.BusinessLayer`): `Graphics Graphics`, `Rectangle Rectangle`, `Point MousePosition`, `int WindowIndex`, `bool DrawBackground`, ctor `(Graphics graphics, Rectangle rectangle, Point mousePosition, int windowIndex)`. VERIFIED — DOCS TradingPlatform.BusinessLayer.PaintChartEventArgs.html (search extract), DEC PaintChartEventArgs.cs. Also `int LeftVisibleBarIndex`, `int RightVisibleBarIndex` (absolute indices, origin Begin, used as `for (int i = args.LeftVisibleBarIndex; i <= args.RightVisibleBarIndex; i++) this.HistoricalData[i, SeekOriginHistory.Begin]`). VERIFIED — SCR IndicatorDeltaDots.cs (official, 2024); not in the older decompile nor in the docs extract.
- `args.Rectangle` is the paintable area of the window being painted; official code clips to it: `var prevClip = gr.ClipBounds; gr.SetClip(args.Rectangle); try { ... } finally { gr.SetClip(prevClip); }` (SCR IndicatorDeltaDots.cs, IndicatorSpeedOfTape.cs) or `gr.SetClip(mainWindow.ClientRectangle)` (EX DrawValueAreaForEachBarIndicator.cs). VERIFIED.
- The old help text `Graphics.FromHdc(args.Hdc)` (KB downloading-history.md) is obsolete; current `PaintChartEventArgs` has `Graphics` and no `Hdc` in the docs extract. VERIFIED (docs) / note.

### 1.11 ChartMouseNativeEventArgs
- Type: `TradingPlatform.BusinessLayer.Chart.ChartMouseNativeEventArgs : TradingPlatform.BusinessLayer.Native.NativeMouseEventArgs` (DLL dump lists both with members split between them). INFERRED. Docs page could not be read (UNVERIFIED in docs).
- `NativeMouseEventArgs` members in the DLL dump: `get_X`, `get_Y`, `get_Button`, `get_Clicks`, `get_Delta`, `set_Handled`. INFERRED types from usage: `X`, `Y` int (used in `Rectangle.Contains(e.X, e.Y)`), `Button` = `TradingPlatform.BusinessLayer.Native.NativeMouseButtons` (VERIFIED `e.Button == NativeMouseButtons.Left`, EX IndicatorChartMouseEvents.cs), `Clicks` int, `Delta` int (wheel), `bool Handled` (VERIFIED `e.Handled = true;`), `Point Location` (VERIFIED `e.Location`, EX IndicatorChartMouseEvents.cs).
- `ChartMouseNativeEventArgs` members in the DLL dump: `get_Window` (`IChartWindow`; user: `e.Window.WindowNumber`), `set_NeedRedraw` (bool), `set_NeedMouseCapture` (bool). INFERRED. Semantics: `Handled = true` consumes the event so the chart does not process it (official example sets it when it takes a left click inside `MainWindow.ClientRectangle`) — VERIFIED usage, exact semantics UNVERIFIED; `NeedRedraw` = ask the chart to repaint after the handler, `NeedMouseCapture` = keep receiving moves while dragging — UNVERIFIED (names only; the user's compiled code relies on them).
- Handler signature: `void Handler(object sender, ChartMouseNativeEventArgs e)`. VERIFIED — EX IndicatorChartMouseEvents.cs.

### 1.12 OnTryGetMinMax
- `protected virtual bool OnTryGetMinMax(int fromOffset, int toOffset, out double min, out double max)`. VERIFIED — DOCS Indicator.html (bool, int, out double) and EX FitDrawingTestIndicator.cs. Semantics from the official example: offsets are counted from the newest bar (`index = this.Count - 1 - i`), the range is inclusive, skip NaN, return `false` when nothing is valid; it feeds the chart's auto-scale for values that are drawn with GDI rather than line series. Default (DECOMPILED) returns NaN/false; the wrapper `TryGetMinMax` catches and logs exceptions.

---

## 2. Volume analysis (critical for the footprint-imbalance indicator)

### 2.1 Opt-in interface
- `public interface IVolumeAnalysisIndicator { bool IsRequirePriceLevelsCalculation { get; } void VolumeAnalysisData_Loaded(); }`. VERIFIED — both members implemented in every official VA indicator (EX DrawValueAreaForEachBarIndicator.cs, SCR IndicatorDeltaFlow.cs, IndicatorTimeHistogram.cs, IndicatorDeltaDots.cs, IndicatorCumulativeDelta.cs which implements it explicitly: `bool IVolumeAnalysisIndicator.IsRequirePriceLevelsCalculation => false;`); exact text DEC IVolumeAnalysisIndicator.cs. KB text: "By default charts does not loading such data, as it requires time to get full trades history. You need to notify chart, that your indicator need it for calculations and you can do this by implementing a special Interface IVolumeAnalysisIndicator … It contains only one method VolumeAnalysisData_Loaded and it will be called, when all required data finish loading." VERIFIED — KB access-volume-analysis-data-from-indicators.md (the KB text predates the property).
- `IsRequirePriceLevelsCalculation`: every official indicator that reads `PriceLevels` returns `true` (EX DrawValueAreaForEachBarIndicator.cs, SCR IndicatorDeltaDots.cs); every indicator that only reads `Total` returns `false`. VERIFIED pattern. The parameter it maps to is `VolumeAnalysisCalculationParameters.CalculatePriceLevels` (bool) — official CumulativeDelta sets `CalculatePriceLevels = false` for its own Total-only profile (SCR IndicatorCumulativeDelta.cs, VERIFIED); the chart-side mapping from the interface property to that flag is in the chart assembly, not visible (UNVERIFIED). Whether `PriceLevels` is null, empty or still filled when the flag is false is UNVERIFIED; `VolumeAnalysisData`'s constructor always allocates an empty dictionary (DECOMPILED), so expect "empty", never rely on it. Rule for the new indicator: return `true`.

### 2.2 Getting the data
- `protected VolumeAnalysisData GetVolumeAnalysisData(int offset = 0)` on `Indicator` — DLL dump; DECOMPILED body `return HistoricalData[Count - 1 - offset, SeekOriginHistory.Begin].VolumeAnalysisData;` (offset relative to the indicator's `Count`, like `GetPrice`). May return null. VERIFIED usage — SCR IndicatorTimeHistogram.cs: `if (this.GetVolumeAnalysisData(offset) is VolumeAnalysisData analysisData) { double value = analysisData.Total.GetValue(this.FieldType); ... }`.
- Equivalent direct access: `this.HistoricalData[i, SeekOriginHistory.Begin].VolumeAnalysisData` / `this.HistoricalData[offset].VolumeAnalysisData` (null until that bar is calculated). VERIFIED — KB, EX, SCR.

### 2.3 VolumeAnalysisData
- `VolumeAnalysisItem Total` — "Summary calculated Volume info"; `Dictionary<double, VolumeAnalysisItem> PriceLevels` — "Volume info for each price", key = price; `DateTime TimeLeft`; methods `Calculate(double price, double size, AggressorFlag)`, `Combine(VolumeAnalysisData)`, `VolumeAnalysisData CreateAggregatedSnapshot(double aggregationStep)`; event `ItemUpdated` (`EventHandler<VolumeAnalysisDataEventArgs>`, args `double Price`, `VolumeAnalysisItem Item`, "Fire in case of price level was added or existing was updated"). VERIFIED — DOCS TradingPlatform.BusinessLayer.VolumeAnalysisData.html (search extract: PriceLevels dictionary keyed by price with VolumeAnalysisItem values, Total, TimeLeft, Calculate, Combine, CreateAggregatedSnapshot), KB (`HistoricalData[0].VolumeAnalysisData.PriceLevels[1.2564].Volume`), EX DrawValueAreaForEachBarIndicator.cs (`Dictionary<double, VolumeAnalysisItem> priceLevels`). Event DECOMPILED — DEC VolumeAnalysisData.cs.
- Key rounding (DECOMPILED, DEC VolumeAnalysisData.cs): each trade's price is rounded with `CoreMath.RoundToIncrement(price, step)` where `step = Symbol.GetTickSize(price)`, replaced by `CustomTickSize` when set, multiplied by `CustomStep` when > 1, and raised to `Symbol.MinVolumeAnalysisTickSize` when smaller. So keys are multiples of the tick size as doubles: do not index with a computed price (`PriceLevels[price + tick]`) — binary rounding will miss; iterate and sort by key, or round your own price with the symbol's helper and compare with a tolerance (`Math.Abs(a-b) < tick*0.01`). `CreateAggregatedSnapshot(step)` merges levels into coarser buckets (keeps the same `Total` object).
- The dictionary is mutated live for the forming bar (per trade: `Total` updated, level created-or-updated, `ItemUpdated` raised). DECOMPILED. The official per-price painter therefore clones the bar before enumerating: `var currentData = (IHistoryItem)(this.HistoricalData[i, SeekOriginHistory.Begin].Clone()); if (currentData.VolumeAnalysisData == null) continue; foreach (var item in currentData.VolumeAnalysisData.PriceLevels) { ... item.Key (price), item.Value.BuyVolume, item.Value.SellVolume ... }` — VERIFIED SCR IndicatorDeltaDots.cs. DECOMPILED: `HistoryItem`'s copy constructor deep-copies `VolumeAnalysisData` (new dictionary, new items), which is why the clone is safe but costs one allocation per level per paint.

### 2.4 VolumeAnalysisItem (per level and for Total)
Members, DECOMPILED — DEC VolumeAnalysisItem.cs, cross-checked with DOCS VolumeAnalysisItem.html (search extract lists AverageBuySize, AverageSellSize, BuyVolume, BuyVolumePercent, CumulativeDelta, `Delta { get; }`, DeltaFinish, DeltaPercent, `MaxDelta { get; set; }`, MinDelta, SellVolume, `Trades` as int) and the KB definitions table (VERIFIED):

| Member | Type | Definition |
|---|---|---|
| Trades | int, settable | "The number of contracts (trades) that executed at each price level." (KB) |
| BuyTrades / SellTrades | int | number of buy / sell trades (KB) |
| Volume | double | total size executed at the level (KB) |
| BuyVolume / SellVolume | double | buy / sell size (KB) |
| BuyVolumePercent / SellVolumePercent | double, computed | `BuyVolume / Volume * 100`, 0 when Volume ≤ 0 |
| Delta | double, get-only | `BuyVolume - SellVolume` (KB: "difference in traded Volume between Buyers and Sellers") |
| DeltaPercent | double | `Delta / Volume * 100`, 0 when Volume == 0 |
| CumulativeDelta | double | internal running base + Delta (reset per `CumulativeDeltaReset` parameter) |
| MinDelta / MaxDelta | double, settable | running minimum / maximum of this item's `Delta` while trades accumulate (decompiled per-trade update: after adding the trade, `MinDelta = Min(MinDelta, Delta); MaxDelta = Max(MaxDelta, Delta)`). Initialised to `double.MaxValue` / `double.MinValue` — sentinels remain if no trade was ever added. `GetValue(VolumeAnalysisField.MinDelta/MaxDelta)` maps the sentinel to 0; the properties do not. |
| DeltaFinish | double | `Delta - (|MinDelta| > |MaxDelta| ? MinDelta : MaxDelta)` |
| AverageSize / AverageBuySize / AverageSellSize | double | `Volume/Trades`, `BuyVolume/BuyTrades`, `SellVolume/SellTrades`, 0 when the count is 0 (KB definitions agree) |
| MaxOneTradeVolume, MaxOneTradeVolumePercent | double | largest single trade at the level (KB) |
| FilteredTotalVolume / FilteredBuyVolume / FilteredSellVolume (+ …Percent) | double | volume of trades with size ≥ `FilteredVolume` parameter |
| `double GetValue(VolumeAnalysisField field)` | | VERIFIED — EX DrawValueAreaForEachBarIndicator.cs, SCR IndicatorTimeHistogram.cs |
| `void Combine(VolumeAnalysisItem)` | | sums counts/volumes; note it writes NaN into Min/MaxDelta once they are initialised (DECOMPILED oddity) — do not use Combine when you need Min/MaxDelta |

- Trades vs ticks (DECOMPILED): ticks flagged as `VolumeTickDataType.Ticks` (bid/ask quote ticks rather than prints) add `Volume` but do not increment `Trades`, so on BidAsk-type history `Trades`/`AverageSize` can be 0. Aggressor side comes from `AggressorFlag` or from tick direction depending on `DeltaCalculationType`.
- `enum VolumeAnalysisField { Trades, BuyTrades, SellTrades, Volume, BuyVolume, SellVolume, BuySellVolume, FilteredVolume, FilteredBuyVolume, FilteredSellVolume, MaxOneTradeVolume, BuyVolumePercent, SellVolumePercent, Delta, DeltaPercent, CumulativeDelta, FilteredTotalVolumePercent, FilteredBuyVolumePercent, FilteredSellVolumePercent, MaxOneTradeVolumePercent, AverageSize, AverageBuySize, AverageSellSize, OpenInterest, MinDelta, MaxDelta, DeltaFinish }`. DECOMPILED — DEC VolumeAnalysisField.cs; most names VERIFIED by the variants list in SCR IndicatorTimeHistogram.cs and by DOCS VolumeAnalysisField.html (search extract).

### 2.5 Calculation state and progress
- `enum VolumeAnalysisCalculationState { None = 0, Processing = 4, Finished = 0x10 }`. DECOMPILED — DEC VolumeAnalysisCalculationState.cs. `Finished` VERIFIED (KB), `Processing` VERIFIED (SCR IndicatorDeltaFlow.cs compares `e.CalculationState == VolumeAnalysisCalculationState.Processing`). DLL dump references the enum (INFERRED).
- `IVolumeAnalysisCalculationProgress : IDisposable { VolumeAnalysisCalculationState State; int ProgressPercent; int ProgressBarIndex; bool IsAborted; VolumeAnalysisCalculationParameters CalculationParameters; event EventHandler<VolumeAnalysisTaskEventArgs> StateChanged, ProgressChanged; void AbortLoading(); void Wait(CancellationToken token = default); }`. DECOMPILED — DEC IVolumeAnalysisCalculationProgress.cs. VERIFIED members: `State`, `ProgressPercent` (KB), `AbortLoading()` (EX AccessCustomVolumeAnalysisData.cs), `ProgressChanged` with `VolumeAnalysisTaskEventArgs { CalculationState, ProgressPercent }` (SCR IndicatorDeltaFlow.cs), `Wait(token)` (SCR IndicatorCumulativeDelta.cs). DLL dump `get_State` (INFERRED).
- Ready check used by official code: `this.HistoricalData.VolumeAnalysisCalculationProgress == null || this.HistoricalData.VolumeAnalysisCalculationProgress.State != VolumeAnalysisCalculationState.Finished` → not ready. VERIFIED — KB, SCR IndicatorTimeHistogram.cs.

### 2.6 When data arrives
- Historical bars: when an `IVolumeAnalysisIndicator` is on the chart, the chart starts a profile calculation over the chart's `HistoricalData` (progress object appears on `HistoricalData.VolumeAnalysisCalculationProgress`); bars receive their `VolumeAnalysisData` progressively while `State == Processing` (official DeltaFlow recalculates every bar on each `ProgressChanged` event in `Processing` state), and `VolumeAnalysisData_Loaded()` is called once everything is loaded (`State == Finished`). VERIFIED — KB, SCR IndicatorDeltaFlow.cs, IndicatorTimeHistogram.cs. Until then individual bars can have `VolumeAnalysisData == null`: always null-check per bar. VERIFIED (all official VA code).
- `VolumeAnalysisData_Loaded` may run before the indicator has replayed the whole history (`Count < HistoricalData.Count`) and on a different thread than `OnUpdate`: the official TimeHistogram starts a `Task` that waits `while (this.Count != this.HistoricalData.Count) Thread.Sleep(20);` and then writes all values; CumulativeDelta/DeltaDivergenceReversal simply call `Refresh()`/recalculate; DrawValueArea/DeltaDots/SpeedOfTape only set a flag. VERIFIED. Recommended for the new indicator: set a flag and call `Refresh()` (replay with `HistoricalBar` where per-bar values are computed once).
- Forming (live) bar: its `VolumeAnalysisData` object is updated per trade in place (`Total` and `PriceLevels[price]` incremented, `ItemUpdated` raised; DECOMPILED DEC VolumeAnalysisData.cs). When the history processor replaces the last item object on a tick (`HistoryItemUpdated`), the existing `VolumeAnalysisData` reference is carried over to the new object unless `HistoryEventArgs.ResetVolumeAnalysisRequired` (DECOMPILED DEC HistoricalData.cs). A new bar gets its own object (who allocates it is not visible: UNVERIFIED; DeltaDots guards `== null`). The platform raises `HistoricalData.HistoryItemVolumeAnalysisUpdated` when a bar's VA changes (VERIFIED docs extract) — usable to trigger `RedrawBuffer()` instead of polling.
- Ordering between a trade's VA update and the `OnUpdate(NewTick)` for the same trade is UNVERIFIED. Official SpeedOfTape reads `HistoricalData[Count-1, Begin].VolumeAnalysisData.Total` on every `OnUpdate` and tolerates being one trade behind. Re-read the forming bar on each tick; treat historical bars as final after `Finished`.
- Historical VA data for the chart can be absent entirely when the connection has no tick history/licence (the manager returns a progress already `Finished` without data; DECOMPILED DEC VolumeAnalysisManager.cs) — then `Total` stays null on every bar; the user's "pending VA" polling already handles this.

### 2.7 Manual calculations (Core.Instance.VolumeAnalysis)
- `VolumeAnalysisManager` ("Volume Analysis calculations"): `IVolumeAnalysisCalculationProgress CalculateProfile(HistoricalData)`; `CalculateProfile(HistoricalData, VolumeAnalysisCalculationParameters)`; `IVolumeAnalysisCalculationTask CalculateProfile(Symbol, DateTime from, DateTime to)`; `CalculateProfile(VolumeAnalysisCalculationRequest)`; task = `{ VolumeAnalysisData Result; IVolumeAnalysisCalculationProgress Progress }`. VERIFIED — DOCS TradingPlatform.BusinessLayer.VolumeAnalysisManager.html (search extract), EX AccessCustomVolumeAnalysisData.cs (`Core.Instance.VolumeAnalysis.CalculateProfile(this.hoursHistory)`, abort in Dispose), SCR IndicatorCumulativeDelta.cs (`loadedHistory.CalculateVolumeProfile(new VolumeAnalysisCalculationParameters { CalculatePriceLevels = false, SessionsContainer = ..., TimeZone = ... })` then `volumeProgress?.Wait(token)`).
- `VolumeAnalysisCalculationParameters`: `FilteredVolume, ForceReload, DeltaCalculationType, CumulativeDeltaReset, CustomTickSize, CustomStep, SessionsContainer (ISessionsContainer), TimeZone, CalculatePriceLevels, ForceUsingTickData`. DECOMPILED — DEC VolumeAnalysisCalculationParameters.cs; `CalculatePriceLevels`, `SessionsContainer`, `TimeZone` VERIFIED (SCR IndicatorCumulativeDelta.cs).

### 2.8 Exact recipe to iterate a bar's price levels (quoted from official code)
From EX DrawValueAreaForEachBarIndicator.cs (VERIFIED, verbatim):
```csharp
public class DrawValueAreaForEachBarIndicator : Indicator, IVolumeAnalysisIndicator
{
    private bool volumeAnalysisLoaded;
    public bool IsRequirePriceLevelsCalculation => true;
    public void VolumeAnalysisData_Loaded() => this.volumeAnalysisLoaded = true;

    public override void OnPaintChart(PaintChartEventArgs args)
    {
        base.OnPaintChart(args);
        if (!this.volumeAnalysisLoaded) return;
        var mainWindow = this.CurrentChart.MainWindow;
        Graphics gr = args.Graphics;
        var prevClip = gr.ClipBounds;
        gr.SetClip(mainWindow.ClientRectangle);
        int halfTickSizeInPx = (int)(Symbol.TickSize * mainWindow.YScaleFactor / 2.0);
        try
        {
            DateTime leftTime = mainWindow.CoordinatesConverter.GetTime(mainWindow.ClientRectangle.Left);
            DateTime rightTime = mainWindow.CoordinatesConverter.GetTime(mainWindow.ClientRectangle.Right);
            int leftIndex = (int)mainWindow.CoordinatesConverter.GetBarIndex(leftTime);
            int rightIndex = (int)Math.Ceiling(mainWindow.CoordinatesConverter.GetBarIndex(rightTime));
            for (int i = leftIndex; i <= rightIndex; i++)
            {
                if (i > 0 && i < this.HistoricalData.Count && this.HistoricalData[i, SeekOriginHistory.Begin] is HistoryItemBar bar
                    && bar.VolumeAnalysisData != null)
                {
                    double val, vah;
                    this.CalculateValueArea(bar.VolumeAnalysisData.PriceLevels, out val, out vah);
                    int barLeftX = (int)Math.Round(mainWindow.CoordinatesConverter.GetChartX(bar.TimeLeft));
                    int vahValueY = (int)mainWindow.CoordinatesConverter.GetChartY(vah) - halfTickSizeInPx;
                    gr.DrawLine(Pens.Green, barLeftX, vahValueY, barLeftX + this.CurrentChart.BarsWidth, vahValueY);
                    ...
                }
            }
        }
        finally { gr.SetClip(prevClip); }
    }

    private void CalculateValueArea(Dictionary<double, VolumeAnalysisItem> priceLevels, out double val, out double vah)
    {
        List<VolumeInfo> sortedPricesLevels = priceLevels.Select(item => new VolumeInfo() { Item = item.Value, Price = item.Key }).OrderBy(it => it.Price).ToList();
        ...
        double value = sortedPricesLevels[i].Item.GetValue(VolumeAnalysisField.Volume);
```
From SCR IndicatorDeltaDots.cs (VERIFIED, verbatim excerpt; the clone-before-enumerate pattern and the newer visible-range properties):
```csharp
int leftIndex = args.LeftVisibleBarIndex;
int rightIndex = args.RightVisibleBarIndex;
for (int i = leftIndex; i <= rightIndex; i++)
{
    var currentData = (IHistoryItem)(this.HistoricalData[i, SeekOriginHistory.Begin].Clone());
    if (currentData.VolumeAnalysisData == null)
        continue;
    ...
    dotRect.X = (float)(currWindow.CoordinatesConverter.GetChartX(this.HistoricalData[i, SeekOriginHistory.Begin].TimeLeft) + this.CurrentChart.BarsWidth / 2);
    foreach (var item in currentData.VolumeAnalysisData.PriceLevels)
    {
        volumeData.Add(item.Key, item.Value);   // item.Key = price, item.Value = VolumeAnalysisItem
        prices.Add(item.Key);
    }
    ...
    double buyValue = item.Value.BuyVolume;
    double sellValue = item.Value.SellVolume;
    dotRect.Y = (float)currWindow.CoordinatesConverter.GetChartY(item.Key);
```
Footprint diagonal imbalance therefore means: sort the keys ascending; for level k compare `levels[k].BuyVolume` (ask side, aggressive buys at price p) against `levels[k-1].SellVolume` (bid side one tick lower) — the DeltaDots "diagonalComparsion" option compares each level's SellVolume with its neighbours at `prices[priceIndex ± 1]` (VERIFIED the neighbour-by-sorted-index technique). Price step between adjacent keys is the VA tick (`CustomTickSize`/`CustomStep` may make it larger than `Symbol.TickSize`): derive the step from the sorted keys, not from the symbol, and insert empty levels for gaps if the imbalance rule needs them.

Cost guidance (from the above): computing imbalances per bar belongs in `OnUpdate` (`HistoricalBar`/`NewBar`: compute once per closed bar; `NewTick`: recompute only the forming bar) into your own arrays; `OnPaintChart` should only read those arrays for `LeftVisibleBarIndex..RightVisibleBarIndex`. Only clone/enumerate `PriceLevels` of the forming bar on the paint thread if you must, inside try/catch for `InvalidOperationException` (collection modified) — the official code avoids the issue by cloning.

---

## 3. Chart types / aggregations and bar times

- Base `HistoryAggregation` (abstract): `string Name`, `Settings`, `Period DefaultRange`, obsolete `Period GetPeriod` / `HistoryType GetHistoryType` ("Use casting to a specific type of aggregation and refer to the properties of the aggregation directly"), `TimeScaleType` (Linear for time, NonLinear otherwise), `BarCreationBehavior` (Realtime / Deferred — Renko is Deferred). DECOMPILED — DEC HistoryAggregation.cs. `Name` constants: "Tick", "Tick bars", "Time", "Renko", "Heikin Ashi", "Range bars", "Points & Figures", "Kagi", "Line Break", "Volume", "Reversal", "Power Trades", "VWAP", "Level2", "DOM by ticks count", "DOM by time", "Aggregated DOM", "Delta bars", "Aggregated ticks (Last)", "Price changes count bars", "Time statistics", "Volume profile". DECOMPILED.
- KB lists nine public aggregation classes with constructors: `HistoryAggregationTick(int ticksCount)` [older signature], `HistoryAggregationTime(Period)`, `HistoryAggregationHeikenAshi(HeikenAshiSource, int)`, `HistoryAggregationRangeBars(int rangeBars)` ("rangeBars - the height (in ticks) of each bar"), `HistoryAggregationRenko(Period period, int brickSize, RenkoStyle renkoStyle, int extension = 100, int inversion = 100, bool showWicks = false, bool buildCurrentBar = true)`, `HistoryAggregationLineBreak(Period, int)`, `HistoryAggregationKagi(Period, int)`, `HistoryAggregationPointsAndFigures(Period, int boxSize, int reversal, PointsAndFiguresStyle)`, `HistoryAggregationVolume(int volumeValue)`. VERIFIED — KB access-to-advanced-aggregations.md. The API reference shows the Renko constructor with an extra `HistoryType historyType` after `Period` (VERIFIED — DOCS TradingPlatform.BusinessLayer.HistoryAggregationRenko.html search extract: `public sealed class HistoryAggregationRenko : HistoryAggregationTime, IHistoryAggregationHistoryTypeSupport`), and official 2024 code constructs `new HistoryAggregationTick(HistoryType.Last)` (SCR IndicatorSlippage.cs) — the newer builds add a `HistoryType` parameter to these constructors (DECOMPILED agrees: `HistoryAggregationRangeBars(int rangeBars, HistoryType historyType)`, `HistoryAggregationTickBars(int ticksCount, HistoryType historyType)`, `HistoryAggregationTime(Period period, HistoryType historyType)`).
- `HistoryAggregationRangeBars` ("Range bars"): `int RangeBars` (bar height in ticks), `HistoryType HistoryType`; `GetPeriod => Period.TICK1`. DECOMPILED — DEC HistoryAggregationRangeBars.cs; `get_RangeBars` in DLL dump (INFERRED); definition of the number VERIFIED (KB). Bar height in price = `RangeBars * tickSize` (the user's `DetectGeometry`). Range bars are built from ticks (`GetAggregationToDirectDownload` → `HistoryAggregationTick`), so a range bar's `TicksLeft` = time of its first tick and `TicksRight` = time of its last tick (DECOMPILED structure; exact fill of `TicksRight` for the forming bar UNVERIFIED).
- `HistoryAggregationTickBars` ("Tick bars"): `int TicksCount`, `HistoryType`; `GetPeriod => new Period(BasePeriod.Tick, TicksCount)`; `IsWaitingFirstQuoteRequired => true`. DECOMPILED — DEC HistoryAggregationTickBars.cs; `get_TicksCount` in DLL dump (INFERRED). Distinct from `HistoryAggregationTick` ("Tick": one bar per tick, `GetPeriod == Period.TICK1`, ctor `(HistoryType)`), which official code uses to detect pure tick charts: `if (this.HistoricalData.Aggregation is HistoryAggregationTick)`. VERIFIED — SCR IndicatorSpeedOfTape.cs, IndicatorDeltaRotation.cs. The user's `DetectGeometry` only handles TickBars; a plain Tick chart falls into `Agg.Other` (no API problem, just a note).
- `HistoryAggregationRenko : HistoryAggregationTime`: `int BrickSize` (ticks), `RenkoStyle RenkoStyle` (Classic, HighLow, AdvancedClassic, AdvancedHighLow — KB), `int Extension`, `int Inversion`, `bool ShowWicks`, `bool BuildCurrentBar`, inherited `Period Period` (base period of the ticks used), `BarCreationBehavior.Deferred`. DECOMPILED — DEC HistoryAggregationRenko.cs; `get_BrickSize` in DLL dump (INFERRED); inheritance VERIFIED (docs extract). Consequence: `aggregation is HistoryAggregationTime` is TRUE for Renko; the user's exact-type test `GetType() == typeof(HistoryAggregationTime)` is the right way to mean "plain time bars" (official SpeedOfTape uses `is HistoryAggregationTime` and would treat Renko as time bars).
- `HistoryAggregationTime` ("Time"): `Period Period`, `HistoryType HistoryType`, `bool UsePrevCloseAsOpenPriceBar`; `TimeScaleType.Linear`. DECOMPILED — DEC HistoryAggregationTime.cs; `aggregationTime.Period.Duration.TotalSeconds` VERIFIED — SCR IndicatorSpeedOfTape.cs; `get_Period` in DLL dump (INFERRED).
- `struct Period`: `BasePeriod BasePeriod`, `int PeriodMultiplier`, `long Ticks` (= `TicksInBasePeriod(BasePeriod) * PeriodMultiplier`, .NET 100 ns ticks), `TimeSpan Duration`, ctor `Period(BasePeriod, int)`, statics `TICK1, SECOND1/5/10/15/30, MIN1/2/3/4/5/10/15/30, HOUR1/2/3/4/6/8/12, DAY1, WEEK1, MONTH1, YEAR1`, comparison operators, `Format()`. DECOMPILED — DEC Period.cs; `Period.MIN15`, `Period.TICK1`, `Period.HOUR1` VERIFIED (KB, EX); `get_BasePeriod/get_PeriodMultiplier/get_Ticks` in DLL dump (INFERRED). `enum BasePeriod { Tick, Second, Minute, Hour, Day, Week, Month, Year }` DECOMPILED — DEC BasePeriod.cs; `BasePeriod.Second/Minute/Hour` INFERRED (user).
- `HistoryItemBar : HistoryItem` ("Represents historical data bar item"): from `HistoryItem`: `long TicksLeft` (UTC ticks of the left border), `DateTime TimeLeft` (= `new DateTime(TicksLeft, Utc)`, "left border of the bar" — VERIFIED KB downloading-history.md), `virtual long TicksRight` (defaults to `TicksLeft` on the base class; the bar overrides it with its own field, doc "Gets bar's right time border"), `VolumeAnalysisData`. On the bar: `DateTime TimeRight` (= `new DateTime(TicksRight, Utc)`), `double Open, High, Low, Close` (VERIFIED — DOCS HistoryItemBar.html), `Median = (H+L)/2`, `Typical = (H+L+C)/3`, `Weighted = (H+L+C+C)/4`, `long Ticks` ("Defines ticks amount" = tick count in the bar), `double Volume`, `double OpenInterest`, `double FundingRate` (VERIFIED docs), `double QuoteAssetVolume`, indexer `this[PriceType]`. DECOMPILED — DEC HistoryItemBar.cs, HistoryItem.cs. VERIFIED usages: `TicksRight - TicksLeft` as bar duration for non-time aggregations (SCR IndicatorSpeedOfTape.cs), `new DateTime(currBar.TicksRight)` (SCR IndicatorTradeSurge.cs), `bar.TimeLeft`, `bar.High/Low/Open/Close` (EX). There is no `TicksLeft?`-style nullable; both are `long`.
- What `Time(offset)` means for a range/tick/renko bar: the bar's open time (first tick's time). DECOMPILED (`Time` → `TicksLeft`) + VERIFIED definition of `TimeLeft`. The chart places the bar at `GetChartX(TimeLeft)` (VERIFIED). Bar close time: `TicksRight`/`TimeRight` (DECOMPILED; official code uses it for duration — VERIFIED usage); for time bars official code uses `Period.Duration` instead (VERIFIED SCR IndicatorSpeedOfTape.cs), which implies `TicksRight` is not guaranteed to be the scheduled end on time charts — UNVERIFIED which of "scheduled end" or "last tick time" it holds for the forming bar.
- `enum HistoryType { Bid, Ask, Midpoint, Last, BidAsk, Mark }` (DECOMPILED — DEC HistoryType.cs); `Symbol.HistoryType` is the symbol's default and `HistoryType.Last`/`Bid`/`BidAsk` are VERIFIED (KB downloading-history.md, access-to-advanced-aggregations.md).

---

## 4. Painting and performance

### 4.1 When OnPaintChart runs
- The chart calls `OnPaintChart` whenever it repaints the window the indicator lives in (scroll, zoom, mouse move with crosshair, new tick, `RedrawBuffer()`), passing the window's `Graphics`, its `Rectangle`, the mouse position and `WindowIndex`. There is no official statement of the frequency; UNVERIFIED beyond "the chart calls it on repaint". `IChart.RedrawBuffer()` "forces the chart to redraw" (VERIFIED — DOCS IChart.html search extract); `IChart.Refresh()` also exists (VERIFIED docs extract; semantics UNVERIFIED, probably a full rebuild).
- Official VA indicators never call `RedrawBuffer()`; they rely on the chart repainting on ticks and on line-series updates. The user's code calls `CurrentChart?.RedrawBuffer()` from `OnUpdate` when something visible changed; that is legal (it is in the DLL dump) but redundant on every tick — throttle it (e.g. only when a signal/value actually changed, never more than ~10–20×/s). UNVERIFIED cost, INFERRED usage.

### 4.2 Thread model (important)
- `OnUpdate` runs synchronously inside the history object's quote processing: `Symbol.NewLast/NewQuote → HistoricalData.ProcessLast/ProcessQuote → history processor → HistoryItemUpdated/NewHistoryItem handlers → Indicator.Update(args) → OnUpdate`. During the initial load it runs from the loading routine with `HistoricalBar`. DECOMPILED — DEC HistoricalData.cs, Indicator.cs. So `OnUpdate` is on the data/connection thread, NOT on the UI thread.
- `OnPaintChart` runs from the chart's painting (`Indicator.PaintChart(ev)`), i.e. the UI/render thread. The decompiled `Indicator.Update` contains a debug loop that blocks the update thread with `Thread.Sleep(100)` until `PaintChart` has set a flag — this only works if the two run on different threads. DECOMPILED (strong evidence, still not an official statement).
- Official code confirms that cross-thread access is expected: `AsynchronousIndicator` (EX) moves heavy work from `OnUpdate` to a long-running worker and calls `SetValue(value, offset: this.Count - currentIndex)` from that worker; `IndicatorTimeHistogram` (SCR) calls `SetValue`/`SetMarker` from a `Task` after `VolumeAnalysisData_Loaded`. VERIFIED. `HistoricalData.Count` and list mutations are guarded by `itemsLocker`, but the indexer getter is unguarded and `CutItems`/`Reload` can shrink the list (DECOMPILED) → every `HistoricalData[i]` read in `OnPaintChart` must be bounds-checked against a freshly read `Count`, and `IndexOutOfRangeException`/`ArgumentOutOfRangeException` should be tolerated.
- Conclusion for both TNX indicators: anything shared between `OnUpdate` (writer) and `OnPaintChart`/`OnTryGetMinMax` (readers) needs a lock or an immutable snapshot (the existing `_sync` + `_paintSync` copy-on-paint design is the right shape); GDI objects created under settings changes must not be disposed while a paint uses them (the user's `OnSettingsUpdated` takes both locks — correct). Official scripts mostly ignore this (they tolerate occasional glitches); `IndicatorPriceActionZones` "locks" a fresh `new object()` which protects nothing.

### 4.3 Practices visible in official code (all VERIFIED as patterns)
- Paint only the visible bars: `args.LeftVisibleBarIndex .. args.RightVisibleBarIndex` (SCR IndicatorDeltaDots.cs) or `GetBarIndex(GetTime(ClientRectangle.Left/Right))` (EX DrawOnBars.cs, DrawValueAreaForEachBarIndicator.cs); bounds-check `i > 0 && i < HistoricalData.Count`.
- Clip to `args.Rectangle` (or `MainWindow.ClientRectangle`) and restore the previous clip in `finally` (SCR IndicatorDeltaDots.cs, IndicatorSpeedOfTape.cs, EX DrawValueAreaForEachBarIndicator.cs).
- Cache GDI+ objects as fields: brushes/pens/fonts/`StringFormat` created in the constructor or when the colour setting changes (property setter creates a new `SolidBrush`), disposed in `Dispose()` (SCR IndicatorLevel2.cs, IndicatorDeltaDots.cs, IndicatorSpeedOfTape.cs). The KB sample that does `new Font("Arial", 20)` per paint is a toy. GDI+ objects are not thread-safe and must not be used by two threads at once (System.Drawing documentation; general knowledge, UNVERIFIED for Quantower specifically).
- Keep per-bar computations out of the paint path: official painters either recompute from `Total` only (cheap) or clone a bar once per paint; the heavy VA scans (value area over all levels) are per visible bar per paint in the example, which is acceptable only for small level counts.
- Provide `OnTryGetMinMax` when you draw values with GDI in a separate window so auto-scale works (EX FitDrawingTestIndicator.cs).
- Null-check `CurrentChart`, `HistoricalData`, each bar's `VolumeAnalysisData`, `VolumeAnalysisCalculationProgress`.
- Keep `OnUpdate` short; if work is heavy, queue it to one worker (EX AsynchronousIndicator.cs) and cancel it in `OnClear` (`CancellationTokenSource.Cancel()`), dispose extra `HistoricalData` and abort VA loading in `Dispose` (EX AccessCustomVolumeAnalysisData.cs).
- Unsubscribe every event you subscribed (`CurrentChart.MouseDown -=` in Dispose; `VolumeAnalysisCalculationProgress.ProgressChanged -=` in OnClear; `renkoHistoricalData.NewHistoryItem -=`) — EX IndicatorChartMouseEvents.cs, SCR IndicatorDeltaFlow.cs, KB access-to-advanced-aggregations.md.
- Exceptions thrown from `OnUpdate`, `OnPaintChart`, `OnInit`, `OnClear`, `OnTryGetMinMax` are caught and logged by the wrappers (DECOMPILED) — they do not crash the platform but silently drop that call; the user's own try/catch + `TnxErrors.Report` is still the better diagnostic.

---

## 5. Core.Instance.TimeUtils, sessions, Interval<T>

- `Core.Instance` (singleton; "main entry point") and `Core.Instance.TimeUtils` (`TimeUtils`). VERIFIED — DOCS TradingPlatform.BusinessLayer.Core.html (`public TimeUtils TimeUtils { get; }`), KB access-to-advanced-aggregations.md (`Core.Instance.TimeUtils.DateTimeUtcNow.AddHours(-6)`). Static shortcuts `Core.TimeUtils`, `Core.Indicators`, `Core.Loggers` also exist (VERIFIED — SCR IndicatorSlippage.cs, IndicatorCumulativeDelta.cs). DLL dump `Core.get_Instance/get_TimeUtils` (INFERRED).
- `TimeUtils`: `DateTime DateTimeUtcNow` (= `DateTime.UtcNow` + server time offset in ms, i.e. server-synchronised UTC), `int ServerTimeOffset`, `TimeZone SelectedTimeZone` (the platform's selected time zone; default Local), `DateTime StartTerminalDateTimeUtc`, `ConvertFromUTCToSelectedTimeZone(DateTime)`, `ConvertFromSelectedTimeZoneToUTC(DateTime)`, `ConvertFromUTCToTimeZone(DateTime, TimeZone)`, `ConvertFromTimeZoneToUTC(DateTime, TimeZone)`, `ConvertFromUTCToTimeZone(TimeSpan, TimeZone)`. DECOMPILED — DEC TimeUtils.cs. VERIFIED: `DateTimeUtcNow` (KB, SCR), `SelectedTimeZone` and both conversions (SCR IndicatorCumulativeDelta.cs). DLL dump `get_DateTimeUtcNow/get_SelectedTimeZone` (INFERRED).
- `struct TimeZone` (TradingPlatform.BusinessLayer, clashes with `System.TimeZone` — official code writes `TradingPlatform.BusinessLayer.TimeZone`): `TimeZoneInfo TimeZoneInfo`, `TimeZoneType Type`, `bool IsEmpty`, ctor `(TimeZoneType type, TimeZoneInfo tzi = null)`. DECOMPILED — DEC TimeZone.cs; `get_TimeZoneInfo` in DLL dump (INFERRED). Chart time zone idiom: `this.CurrentChart?.CurrentTimeZone ?? Core.Instance.TimeUtils.SelectedTimeZone`. VERIFIED — SCR IndicatorCumulativeDelta.cs.
- `SessionsContainer : BusinessObject, ISessionsContainer`: `Id`, `Name`, `Description`, `HolidayInfo[] Holidays`, `SessionsSet[] SessionsSets`, `ISession[] ActiveSessions` (all sessions of all sets), `TimeZoneInfo TimeZone` (returns null in the decompiled build), `ISession[] GetSessionsForDate(DateTime)` (empty on holidays; picks the set by certain dates then by weekday). DECOMPILED — DEC SessionsContainer.cs; `get_ActiveSessions` in DLL dump (INFERRED). `ISessionsContainer { ISession[] ActiveSessions; TimeZoneInfo TimeZone; ISession[] GetSessionsForDate(DateTime); }` DECOMPILED. `IChart.CurrentSessionContainer` returns `ISessionsContainer` (VERIFIED docs + SCR IndicatorCumulativeDelta.cs); `Symbol.CurrentSessionsInfo` returns `SessionsContainer` (INFERRED).
- `ISession { string Name; SessionType Type; TimeSpan OpenTime; TimeSpan CloseTime; bool IsPrimary; }`. DECOMPILED — DEC ISession.cs; `get_IsPrimary` in DLL dump (INFERRED). Open/Close are times of day; a session with `CloseTime < OpenTime` crosses midnight.
- `static class SessionsExtensions` (DECOMPILED — DEC SessionsExtensions.cs): `ContainsDate(this ISession, DateTime)`, `ContainsDate(this ISession, long ticks)`, `ContainsTime(this ISession, TimeSpan|long)` (half-open [Open, Close), wraps midnight), `ContainsDate(this ISession, DateTime, TimeZoneInfo)` (DST-adjusted), `GetSessionForDate(this ISessionsContainer, DateTime)`, `ContainsDate(this ISessionsContainer, DateTime|long)`, `GetSessionOffset(this ISessionsContainer, TimeZone, bool isWeek, bool applyNegativeSignForInDaySession = true)`, `GetSessionOpenTime(this ISessionsContainer, TimeZone)` (uses the primary session, else one containing midnight, else the first by open time). 
  - `EnumerateSessionTimeFrames` is NOT in the decompiled build but IS in 1.146.7 (DLL dump `SessionsExtensions.EnumerateSessionTimeFrames`): from the user's code it is `IEnumerable<Interval<DateTime>> EnumerateSessionTimeFrames(this SessionsContainer|ISessionsContainer container, DateTime fromUtc, DateTime toUtc)` returning concrete session intervals with `From`/`To` in UTC `DateTime` (the user compares `.From.Ticks <= barTicks <= .To.Ticks`). INFERRED. Exact overload set, inclusivity, holiday handling and time-zone basis: UNVERIFIED (no doc page found).
- `struct Interval<T>` (`TradingPlatform.BusinessLayer.Utils`): ctor `(T from, T to)`, `T From`, `T To`, `Split(step)` → sequence of sub-intervals (`new Interval<DateTime>(start, end).Split(this.Symbol.GetHistoryDownloadingStep(aggregation)).ToArray()`). VERIFIED — SCR IndicatorSlippage.cs, IndicatorCumulativeDelta.cs (`new Interval<DateTime>(time, DateTime.MaxValue)`, `session.From`, `session.To`); DLL dump `Utils.Interval`1` (INFERRED). Whether it is a struct or class, and other members (`Contains`, `Length`): UNVERIFIED.

---

## 6. Cross-check of the DLL member dump against the sources

Every `TradingPlatform.*` member the compiled TNXCVDPro references, with the strongest confirmation found:

| Member (from api_surface_from_dll.txt) | Confirmation |
|---|---|
| Chart.ChartMouseNativeEventArgs: get_Window, set_NeedMouseCapture, set_NeedRedraw | NOT CONFIRMED by any official doc/example (only the user's code and the DLL). Type name confirmed on DOCS IChart.html. |
| Chart.IChart: RedrawBuffer, add/remove_MouseDown/MouseMove/MouseUp/MouseWheel, get_BarsWidth, get_CurrentTimeZone, get_Windows | VERIFIED (DOCS IChart.html extracts; MouseDown/MouseUp usage EX) |
| Chart.IChartWindow: get_CoordinatesConverter, get_WindowNumber | VERIFIED (DOCS IChartWindow.html) |
| Chart.IChartWindowCoordinatesConverter: GetBarIndex, GetChartX, GetChartY | VERIFIED (DOCS + EX) |
| Core: get_Instance, get_TimeUtils | VERIFIED (DOCS Core.html, KB) |
| ExecutionEntity: set_Description, set_Name | VERIFIED by usage (`Name = …; Description = …;` in every official example); declaring type INFERRED |
| HistoricalData: get_Aggregation, get_VolumeAnalysisCalculationProgress | VERIFIED (SCR, KB) |
| HistoryAggregationRangeBars.get_RangeBars | DECOMPILED only (number's meaning VERIFIED via KB ctor doc) |
| HistoryAggregationRenko.get_BrickSize | DECOMPILED only (ctor parameter `brickSize` VERIFIED KB) |
| HistoryAggregationTickBars.get_TicksCount | DECOMPILED only |
| HistoryAggregationTime.get_Period | VERIFIED (SCR IndicatorSpeedOfTape.cs) |
| ISession.get_IsPrimary | DECOMPILED only |
| IVolumeAnalysisCalculationProgress.get_State | VERIFIED (KB) |
| Indicator: .ctor, AddLineSeries, GetPrice, OnPaintChart, Time, get_Count, get_CurrentChart, get_HistoricalData, get_Symbol, set_SeparateWindow, get/set_Settings | VERIFIED (DOCS/KB/EX) |
| Indicator.AddLineLevel | DECOMPILED only |
| Indicator.GetVolumeAnalysisData | VERIFIED (SCR IndicatorTimeHistogram.cs) |
| Indicator.OnSettingsUpdated | VERIFIED callable (SCR scripts call it); overridability INFERRED (user compiles `protected override`) |
| Indicator.Ticks | VERIFIED (SCR IndicatorVolume.cs `this.Ticks()`) |
| IndicatorLineMarker..ctor | VERIFIED (KB) |
| InputParameterAttribute: .ctor, get_Name, get_Variants | VERIFIED (DOCS) |
| Line: set_Color, set_Visible, set_Width | DECOMPILED only |
| LineSeries: SetMarker, SetValue | VERIFIED (KB) ; set_ShowLineMarker DECOMPILED only |
| Native.NativeMouseEventArgs: get_Button, get_Clicks, get_Delta, get_X, get_Y, set_Handled | Button/Handled VERIFIED (EX); Clicks/Delta/X/Y NOT CONFIRMED by official sources |
| PaintChartEventArgs: get_Graphics, get_Rectangle, get_WindowIndex | VERIFIED (DOCS) |
| Period: get_BasePeriod, get_PeriodMultiplier, get_Ticks | DECOMPILED only |
| SelectItem.get_Value | DECOMPILED; `.Value` on SelectItem used by official aggregation settings code (DEC) — not in official examples |
| SessionsContainer.get_ActiveSessions | DECOMPILED only |
| SessionsExtensions.EnumerateSessionTimeFrames | NOT CONFIRMED anywhere except the DLL (absent from decompile and docs search) |
| SettingItem: get_Name, get_Value | VERIFIED (official Settings setters use `item.Name`/`.Value` via helpers; DECOMPILED declarations) |
| Symbol.get_CurrentSessionsInfo | DECOMPILED only |
| Symbol.get_TickSize | VERIFIED (EX) |
| TimeUtils.get_DateTimeUtcNow | VERIFIED (KB) ; get_SelectedTimeZone VERIFIED (SCR) |
| TimeZone.get_TimeZoneInfo | DECOMPILED only |
| UpdateArgs.get_Reason | VERIFIED (SCR) |
| VolumeAnalysisData.get_Total | VERIFIED (KB, DOCS) |
| VolumeAnalysisItem: get_BuyVolume, get_Delta, get_MaxDelta, get_MinDelta, get_SellVolume, get_Volume | VERIFIED (DOCS extract + KB table) |
| Utils.Interval`1 | VERIFIED (SCR) |
| Enums BasePeriod, LineStyle, PriceType, SeekOriginHistory, UpdateReason, VolumeAnalysisCalculationState, IndicatorLineMarkerIconType | values listed in sections above; all DECOMPILED, most individual values VERIFIED by official usage |

Members I could not confirm in any official doc or example (exist in the DLL, used by the user's compiling code): `ChartMouseNativeEventArgs.Window/NeedRedraw/NeedMouseCapture`, `NativeMouseEventArgs.X/Y/Clicks/Delta`, `SessionsExtensions.EnumerateSessionTimeFrames`, `Symbol.CurrentSessionsInfo`, `SessionsContainer.ActiveSessions`, `ISession.IsPrimary`, `TimeZone.TimeZoneInfo`, `Period.BasePeriod/PeriodMultiplier/Ticks`, `Line.Color/Width/Visible` setters, `LineSeries.ShowLineMarker`, `Indicator.AddLineLevel`, `HistoryAggregationRangeBars.RangeBars`, `HistoryAggregationTickBars.TicksCount`, `HistoryAggregationRenko.BrickSize`. All of them appear with matching names in the decompiled source, so the risk is limited to semantics, not existence.

---

## 7. Open questions (could not be settled)

1. Is `OnPaintChart` invoked once per chart window with different `WindowIndex` values, or only for the window hosting the indicator? Official code indexes `Windows[args.WindowIndex]` without checking; the user filters by `WindowNumber == WindowIndex`. UNVERIFIED.
2. Exact semantics of `ChartMouseNativeEventArgs.NeedRedraw` / `NeedMouseCapture` and of `Handled` (does `Handled` stop chart scrolling/crosshair?). Names only.
3. `EnumerateSessionTimeFrames`: overloads, whether `From`/`To` are UTC, inclusivity, holiday and weekend handling.
4. For the forming bar, does `HistoryItemBar.TicksRight` hold the last tick time or (time bars) the scheduled close? Official code avoids the question by using `Period.Duration` on time charts.
5. Thread/ordering guarantee between a trade's `VolumeAnalysisData` update and the `OnUpdate(NewTick)` it causes; whether `HistoryItemVolumeAnalysisUpdated` fires per trade or throttled.
6. Whether `PriceLevels` is left empty (vs. not allocated / still filled) when `IsRequirePriceLevelsCalculation` is false, and whether the chart recomputes with levels when a second indicator requires them.
7. Whether `SetValue`/`SetMarker` on a line series trigger a repaint by themselves or only the next tick/`RedrawBuffer()` does.
8. The `IChart.Windows` ordering relative to `WindowNumber` (equivalence of `Windows[args.WindowIndex]` and the user's search).

---

## 8. Local evidence files

- Official examples: `scratchpad/research/dl/Quantower_Examples_master_IndicatorExamples_{AccessCustomVolumeAnalysisData,DrawValueAreaForEachBarIndicator,DrawOnBars,FitDrawingTestIndicator,IndicatorChartMouseEvents,AsynchronousIndicator,DoubleSMAIndicator,SimpleMovingAverage,UsingInnerIndicatorSettings,TestIndicatorWithOneMoreHistoricalData,Level2WithMBOIndicator,AccessIndicatorByName}.cs`
- Official built-in indicator sources: `scratchpad/research/dl/Quantower_Scripts_master_Indicators_Indicator{DeltaFlow,CumulativeDelta,TimeHistogram,DeltaDots,SpeedOfTape,DeltaRotation,DeltaDivergenceReversal,Volume,MaxVolume,PowerBars,BarCounter,RoundNumbers,Level2,TradeSurge,AbnormalVolume,RelativeVolume,HistoricalMinMaxChange,PriceActionZones,Gaps,HighLow,DepthOfBidAsk,OpenInterest,AutoTrendLine,PinBar,Slippage}.cs`
- Official help markdown: `scratchpad/research/dl/kb/quantower-algo_*.md` (access-volume-analysis-data-from-indicators, indicator-with-custom-painting-gdi, access-to-advanced-aggregations, input-parameters, downloading-history, simple-indicator, simple-moving-average-example, using-markers-with-indicators, using-clouds-in-indicator, custom-indicators-access, built-in-indicators, level2-data, …) plus `SUMMARY.md`.
- Decompiled (third-party) sources: `scratchpad/research/dl/decomp/*.cs` (Indicator, HistoricalData, Symbol, Core, VolumeAnalysisData, VolumeAnalysisItem, VolumeAnalysisManager, VolumeAnalysisCalculationParameters/Request, IVolumeAnalysisIndicator, IVolumeAnalysisCalculationProgress, VolumeAnalysisCalculationState, VolumeAnalysisField, HistoryItem, HistoryItemBar, IHistoryItem, HistoryEventArgs, LineSeries, Line, LineLevel, IndicatorLineMarker, IndicatorLineMarkerIconType, InputParameterAttribute, SettingItem, SelectItem, HistoryAggregation*, Period, BasePeriod, TimeUtils, TimeZone, SessionsContainer, ISession, ISessionsContainer, SessionsExtensions, UpdateArgs, UpdateReason, PaintChartEventArgs, PriceType, SeekOriginHistory, LineStyle, HistoryType, IndicatorUpdateType).
- DLL member dump: `scratchpad/research/api_surface_from_dll.txt`.
