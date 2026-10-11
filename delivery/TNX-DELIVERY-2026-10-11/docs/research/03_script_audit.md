# 03 · Script audit: TNXCVDPro.cs (TNX CVD PRO 2.2)

Audited file: `upload/TNX-CVD-PRO/src/TNXIndicators/TNXCVDPro.cs` (6457 lines, read in full). Guide text:
`upload/TNX-CVD-PRO/guide/make_guide.py` (read in full). DLL reference dump: `research/api_surface_from_dll.txt`.
Helper classes `GdiCache`, `PictureArrays`, `TnxErrors` are compiled in from `..\..\TNX-COMMON\*.cs` (csproj lines 15-17);
their sources are not in the upload, so their members below come from the IL dump `research/tnx_ikdasm.txt`
(classes at IL lines 33685, 34949, 35175).

## Status key and sources

- **VERIFIED** = read in an official Quantower source: an official example file (github.com/Quantower/Examples or
  github.com/Quantower/Scripts, local copies in `research/official_examples`, `research/qt_examples`, `research/dl`) or a
  help.quantower.com knowledge-base page (local copies in `research/dl/kb`).
- **SNIPPET** = only the search-engine snippet of an api.quantower.com page was readable. `WebFetch` could not resolve
  `api.quantower.com` or `help.quantower.com` from this session (`getaddrinfo ENOTFOUND`, tried 2026-10-11); the pages
  themselves were NOT read. Treat as one step below VERIFIED.
- **INFERRED** = the member is referenced by the user's compiled DLL (it compiles against
  TradingPlatform.BusinessLayer 1.146.7.0), so it exists with that name; its semantics are deduced from the user's code.
- **UNVERIFIED** = assumed; or taken from the third-party decompiled mirror `research/decompiled_mirror` (PickReaper/quantower,
  DLL version unknown).

API facts used in this audit:

| Member | Status | Source |
|---|---|---|
| `Indicator : IVolumeAnalysisIndicator`, `bool IsRequirePriceLevelsCalculation`, `void VolumeAnalysisData_Loaded()` | VERIFIED | kb `quantower-algo_access-volume-analysis-data-from-indicators.md` (https://help.quantower.com/quantower/quantower-algo/access-volume-analysis-data-from-indicators); `IndicatorCumulativeDelta.cs` |
| `HistoricalData.VolumeAnalysisCalculationProgress.State != VolumeAnalysisCalculationState.Finished` | VERIFIED | same kb page; `IndicatorTimeHistogram.cs` lines 49-52 |
| `GetVolumeAnalysisData(int offset)` returns `VolumeAnalysisData` with `.Total` (a `VolumeAnalysisItem`) | VERIFIED | `IndicatorTimeHistogram.cs` line 80 (`this.GetVolumeAnalysisData(offset) is VolumeAnalysisData`); `.Total` on the kb page |
| `VolumeAnalysisItem.Volume/BuyVolume/SellVolume/Delta` | VERIFIED | kb page table |
| `VolumeAnalysisItem.MinDelta/MaxDelta` exist | INFERRED | DLL refs `get_MinDelta, get_MaxDelta`. Mirror (UNVERIFIED) shows the empty item holds sentinels `MinDelta = double.MaxValue, MaxDelta = double.MinValue` and `Combine` can set them to NaN; the code's `Safe()` (1686-1693) maps exactly those values to 0, which suggests the author saw them. |
| `OnPaintChart(PaintChartEventArgs args)`, `args.Graphics` | VERIFIED | kb `quantower-algo_indicator-with-custom-painting-gdi.md` |
| `PaintChartEventArgs.Rectangle`, `.WindowIndex`, `.MousePosition` | SNIPPET + VERIFIED usage | api.quantower.com `PaintChartEventArgs` snippet lists Graphics, Rectangle, MousePosition, WindowIndex, DrawBackground; `FitDrawingTestIndicator.cs` line 60 uses `this.CurrentChart.Windows[args.WindowIndex].CoordinatesConverter` |
| `IChart.Windows`, `IChartWindow.CoordinatesConverter` | VERIFIED usage | `FitDrawingTestIndicator.cs` line 60 |
| `IChartWindow.WindowNumber` | INFERRED | DLL refs `get_WindowNumber` (the user's code matches it against `args.WindowIndex`, 2606) |
| `IChartWindowCoordinatesConverter.GetBarIndex(DateTime)->double`, `GetChartX(DateTime)->double`, `GetChartY(double)->double`, `GetTime(double)`, `GetPrice(double)` | SNIPPET + VERIFIED usage | snippet of https://api.quantower.com/docs/TradingPlatform.BusinessLayer.Chart.IChartWindowCoordinatesConverter.html; `DrawOnBars.cs` 52-68, `IndicatorChartMouseEvents.cs` 70 |
| `IChart.BarsWidth` (int, bar width in px) | SNIPPET + VERIFIED usage | IChart snippet; `DrawOnBars.cs` 67 |
| `IChart.MouseWheel/MouseDown/MouseMove/MouseUp` as `EventHandler<ChartMouseNativeEventArgs>` | VERIFIED (Down/Up), SNIPPET (Wheel), INFERRED (Move) | `IndicatorChartMouseEvents.cs` 30-31; IChart snippet; DLL refs `add_MouseMove` |
| `ChartMouseNativeEventArgs.Button`, `.Handled`, `NativeMouseButtons.Left` | VERIFIED | `IndicatorChartMouseEvents.cs` 38-44 |
| `ChartMouseNativeEventArgs.Window`, `.NeedRedraw`, `.NeedMouseCapture`, `.X/.Y/.Delta/.Clicks` (base `NativeMouseEventArgs`) | INFERRED | DLL refs |
| `IChart.RedrawBuffer()` | INFERRED | DLL refs (not found in any official example or snippet) |
| `IChart.CurrentTimeZone.TimeZoneInfo`, `Core.Instance.TimeUtils.SelectedTimeZone` | INFERRED | DLL refs; IChart snippet says `CurrentTimeZone` returns a `TimeZone` |
| `Core.Instance.TimeUtils.DateTimeUtcNow` | VERIFIED | `AccessSessionsInfoExample.cs` line 20 |
| `OnTryGetMinMax(int fromOffset, int toOffset, out double min, out double max)` | VERIFIED | `FitDrawingTestIndicator.cs` line 34 |
| `OnInit` is called "after creating an indicator as well as after its input params reset or chart (symbol or timeframe) updates" | VERIFIED (quoted comment) | `IndicatorChartMouseEvents.cs` line 27 |
| `Symbol.CurrentSessionsInfo` (`SessionsContainer`), `.ActiveSessions`, `ISession.IsPrimary`, `SessionsExtensions.EnumerateSessionTimeFrames(from, to)` -> `Interval<DateTime>` with `From/To` | INFERRED | DLL refs; whether the intervals are closed or half-open: UNVERIFIED |
| `HistoryAggregationRangeBars.RangeBars`, `HistoryAggregationTickBars.TicksCount`, `HistoryAggregationRenko.BrickSize`, `HistoryAggregationTime.Period` (`BasePeriod`, `PeriodMultiplier`, `Ticks`) | INFERRED | DLL refs; the kb page on aggregations only shows constructors |
| `LineSeries.SetMarker(offset, IndicatorLineMarker)`, `IndicatorLineMarker(Color)` | VERIFIED | `IndicatorSpeedOfTape.cs` 86 |
| `InputParameterAttribute(name, sortIndex, min, max, increment, decimalPlaces, variants)`; "parameters displayed in specified [SortIndex] order" | VERIFIED | kb `quantower-algo_input-parameters.md` |
| `InputParameterAttribute.Variants`, `SelectItem.Value`, `SettingItem.Name/Value` | INFERRED | DLL refs |
| `UpdateArgs.Reason`, `UpdateReason.NewBar/HistoricalBar` | INFERRED | DLL refs |
| Which thread calls `OnPaintChart`, the mouse events, `OnUpdate`, `OnTryGetMinMax`; whether `RedrawBuffer` may be called from any thread | UNVERIFIED | no source found. The code's own comment (6172-6174) assumes mouse events arrive on the chart window's thread and that a picture may be in progress at the same time. |

---

## 1. Architecture map

### 1.1 Data model

`Bar` (struct, 144-217), one per chart bar at absolute index `abs` (0 = oldest; `Off(abs) = Count-1-abs`, 1681):

| Field | Written at | Meaning |
|---|---|---|
| `Delta` | 1739 | bar delta = `Total.Delta` (buy minus sell volume); 0 when no VA |
| `MinD`, `MaxD` | 1740-1741 (from 1714-1715) | intra-bar min/max of the running delta, clamped so that `MinD <= min(0,Delta)` and `MaxD >= max(0,Delta)`; VA sentinels/NaN mapped to 0 by `Safe` |
| `Buy`, `Sell`, `Vol` | 1742-1744 | `Total.BuyVolume/SellVolume/Volume`; without VA `Vol = GetPrice(PriceType.Volume)` (1720) |
| `O,H,L,C` | 1746-1749 | the CVD candle: `O` = previous `C` (0 after a reset, 1737), `C = O+Delta`, `H = O+MaxD`, `L = O+MinD` |
| `Px, PO, PH, PL` | 1750-1753 | price close/open/high/low |
| `Ticks` | 1727 | bar open time as `Time(off).Ticks`, treated as UTC everywhere (2538, 2782, 4564) |
| `Real` | 1745 | VA present and any volume (1716) |
| `Has` | 1754 | computed at least once |
| `PAbsDp` | 1772/1782 | prefix sum of abs(close-to-close price change) |
| `PAbsDc` | 1773/1783 | prefix sum of abs(Delta) |
| `PBuy`, `PSell` | 1774-1775 | prefix sums of Buy/Sell, Real bars only |
| `PVol` | 1776 | prefix sum of Vol |
| `PRej` | 1777 | prefix sum of the "rejection" fraction `val` (1760-1766): how much of the bar's largest delta excursion was given back by the close, 0..1 |
| `PDelta` | 1778 | prefix sum of Delta (the Continuous CVD) |
| `R`, `E` | 1790 via `Velocity` 1985-2003 | price velocity and delta velocity over `VelocityBars`, in units of the typical per-bar move (`ScaleP`) / typical per-bar abs delta (`ScaleC`), divided by sqrt(n) |
| `DivRun` | 1811 | signed length of the run of consecutive bars where R and E have opposite signs and both exceed `DivergeMin` (positive = price up / delta down) |
| `Fast`, `Slow` | 1798-1799 | mean Delta over `max(2,VelocityPeriod/3)` and `max(3,VelocityPeriod)` bars (`Win` on `PDelta`, forming-bar weighted by `F`) |
| `AccE` | 1804 | EMA (`AccelSmooth`) of `Fast - Fast[abs-AccelLag]` (1800); the forming bar's lag reference is interpolated by `F` |
| `PAbsAcc` | 1805 | prefix sum of abs(AccE) (the "typical" acceleration for `AccNorm`) |
| `Flip` | 1806 | `Fast` changed sign vs. previous bar |
| `F` | 1755 via `Progress` 1864-1903 | forming-bar progress 0..1 (1.0 for every closed bar); range: range so far / `RangeFull`; tick: ticks / `TicksCount`; time: elapsed / period |
| `Seg` | 1738 | abs index of the first bar of the current CVD segment (last reset) |
| `Anchor` | 1729 | session start ticks (only when Reset == Session or Bars-from-session) |
| `SessIdx` | 1734 | bars since the session's first bar (only Reset == Bars with `ResetBarsFromSession`) |

`Sig` (class, 219-280): `Abs` (bar it was evaluated on), `Kind` (Absorption/Exhaustion/Divergence), `Bull`, `Score`, `R`, `E`,
`Pending` (forming bar), `Note`, `P1/P2/V1/V2` (swing divergence: the two swing bars and their CVD), `Seen` (last bar the
same event fired on), `Avg0/AtAvg/AtExt` (the frozen filter answer, see 2.3). `Again()` (260-279) returns a new object so
the picture's copy is never mutated.

Scales: `ScaleP` (1949-1967) = mean abs price change over `Lb` bars, floored at half the bar height and one tick;
`ScaleC` (1975-1983) = mean abs delta over `Lb` bars, floored at 1e-9; `Lb(abs) = min(max(5,LookbackBars), abs+1)` (1815).

### 1.2 Compute pipeline (all under `_sync`)

```
OnUpdate 1442 -> UpdateCore 1469-1542
  DetectGeometry 1392 (once; chart type -> _agg/_barHeight/_aggN, _chartTag)
  count < _n -> RecalcAll 1641 (full recompute; also VolumeAnalysisData_Loaded 1340)
  Ensure 1673 (grow _b)
  _pendingVa check 1487-1498 (a bar whose VA arrived late -> RecalcAll)
  for i in first..last: Compute(i) 1695, Publish(i) 2444
     Compute: VA read 1706-1721, _dirtyFrom 1722, Ticks/Anchor/SessIdx 1727-1735,
              reset/segment 1736-1738, OHLC 1746-1749, prices 1750-1753, F=Progress 1755,
              prefix sums 1760-1789, Velocity 1790, Fast/Slow/AccE/PAbsAcc/Flip 1791-1806, DivRun 1807-1811
     Publish: LineSeries values/markers for the platform scale 2449-2469
  _recheck / RecheckClosed 1506-1514, 1547-1574 (F6: re-read the bar that just closed for 3 updates)
  Evaluate(j, pending:false) for every newly closed bar 1516-1531 (with F6 snapshot of swing state + _sigs)
  Evaluate(last, pending:true) if !ClosedOnly, else RemovePending 1533-1540
  return LiveRedrawDue 1576-1615 -> RedrawBuffer now, or arm _liveTimer (OnLiveTimer 1617 -> RedrawBuffer)
```

`Win(b, abs, len, k)` (1846-1862) = prefix-sum window of length `len` ending at `abs`, with the forming bar weighted by
`F` (the oldest bar of the window gets the complementary `1-F`; for `AbsAcc`/`Rej` the forming bar is removed instead).

`Evaluate` (2043-2225): guards (2045, 2050) -> `RemovePending` -> lookback extremes loop (2065-2088) -> flow divergence
from `DivRun` (2093-2103, `Add`) -> swing divergence (2104-2183, `AddSig`, updates `_ph*/_pl*` state) -> exhaustion
(2184-2202, `Push` 2032) -> absorption on the bar or over `VelocityBars` (2203-2224, `BarRead` 2011).

Signal store: `Add` (2281-2360) and `AddSig` (2250-2279) keep `_sigs` in bar order, cap 4000 (2275, 2321, 2356).
With `KeepFirstSignal` (default) a repeat of the same kind/side within `SignalGap` bars of the last firing is merged by
`Again` (2296-2304) and the new signal gets its filter answer frozen by `Freeze` (2403-2409) from `AvgScoreIn` (2369-2399).
Classic path (2327-2359) deletes the weaker earlier marker.

### 1.3 Paint pipeline (under `_paintSync`; `_sync` only inside `CopyForPaint`)

```
OnPaintChart 2595 -> find window by WindowNumber 2602-2617 -> HookMouse 2618-2621 -> g.Save 2623
  lock _paintSync { lock _sync { CopyForPaint 2648 } ; _arrays.BeginPaint 2632 ; PaintCore 2679 } ; g.Restore 2642
CopyForPaint: Array.Copy of _b[_dirtyFrom.._n) into _pb 2650-2670; _psigs refreshed when _sigsVersion changed 2671-2676
PaintCore 2679-2734:
  PaintClip 2752 (window minus the TitleAreaWidth x 24 px strip), Fonts 6237, XMap 2761 (x0,i0,slot from two anchors)
  vL/vR 2696-2697
  Mode: Candles 2825 / CandlesCrisp 2868 | DeltaBars 2947 | VelocityPanel 2968
  Signals 4195 (clipped above the lanes in Velocity mode 2712-2717)
  DrawTable 2737 -> PanelCard 4789 (Card) | Panel 4630 (Classic)
  DrawLaneTitles 3511 -> LaneTitle 3531 (Pro lanes; after the card so the pill can avoid it)
  HoverCard 4474
VelocityPanel 2968-3469:
  pane heights 2977-3010 (PaneHeights 3474 in Pro), separators 3011-3016, rects 3018-3019
  CVD pane scale 3031-3154 (AUTO 3080-3115 / MANUAL 3116-3152), clip 3156
  CVD candles (CandlesCrisp 3159 | smoothed 3161-3199) or line (3201-3227, _arrays slots 1,2)
  CVD axis, ticks, zero line, last-value tag, AUTO/MANUAL badge 3229-3271
  flow lane: background 3272, LaneClip 3277, scale 3277-3294, zero 3297-3307,
             per-bar fast bars 3309-3395 (HBarTier 3942; tier by FlowRank 3328 or classic 3329; crossing dots 3382-3394)
             slow line points 3396-3408 (_arrays slot 3, exact copy slot 4), Pro area 3417-3433 (slot 6), classic dotted 3437-3438
             captions/axis/pill 3441-3461 (Axis 3983; _ltFlow 3455; FlowText/FlowWords)
  AccelStrip 3625-3848: scale 3634-3656, levels 3664-3703, per-bar 3705-3817 (AccNorm 3614, HBar 3907, AccelMarkPro 3876
             or classic marks 3786-3816), axis/text 3818-3847 (_ltAcc)
Signals 4195-4289: per sig -> DrawBar 2414 (Passes 2424) -> y 4218-4220 -> swing line + SwingLink 4222-4243 ->
  GlyphBadge 4333 | GlyphClean 4414 | Glyph 4577 -> tail 4272-4278 -> score 4279 -> _hitList 4283
PanelCard 4789-5039: readings 4797-4844 -> rows (CardRow kinds) 4846-4886 -> place 4888-4926 -> CardTemplates 5061 ->
  CardGroups 5090 -> fit loop 4936-4993 -> fonts at the chosen scale 4994-5000 -> layout 5002-5037 -> DrawCard 5310-5495
```

Y mapping: platform scale via `conv.GetChartY` in Candles/Line/DeltaBars (`CY` 5700); the Velocity CVD pane has its own
linear map `CvdY/CvdV` (5690-5698) and `OnTryGetMinMax` (4020-4118) hands that view back to the platform (4088-4099) so
the platform's own scale/zero line coincide with it.

### 1.4 Threading and locks

- `_sync` (698): live data (`_b`, `_n`, `_sigs`, swing state, `_pendingVa`, geometry). Taken in OnInit 1241, OnClearCore
  1299, VolumeAnalysisData_Loaded 1344, OnSettingsUpdated 1370, OnUpdate 1447, OnLiveTimer 1622, OnPaintChart 2628,
  OnTryGetMinMax 4026.
- `_paintSync` (703): the picture copy (`_pb`, `_pn`, `_psigs`) and every GDI object/cache. Taken in OnClearCore 1297,
  OnSettingsUpdated 1368, OnPaintChart 2626; `Monitor.TryEnter` in HoverAt 6175 (never blocks the mouse).
- Lock order `_paintSync` then `_sync` (comment 700-702) is respected at every site.
- Live timer: `System.Threading.Timer _liveTimer` (819) armed with `Interlocked.Exchange(ref _liveArmed, 1)` (1603),
  one-shot `Change(due, -1)` (1607); callback 1617 clears the flag, updates `_liveAt` under `_sync`, calls `RedrawBuffer`
  from a thread-pool thread. Disposed in OnClearLocked 1335-1337.
- Mouse handlers (5860-6235) run with NO lock and read/write the view state (`_spZoom`, `_acZoom`, `_cvAuto`, `_cvMCenter`,
  `_cvMHalf`, `_cvMSet`, `_cvFollow`, `_cvOff`, `_spOff`, `_acOff`, `_spFrac/_acFrac`, `_spHpx/_acHpx`, `_drag*`, `_soft*`,
  `_hover`, and the two settings `VelocityPanelHeight`/`AccelHeight`) while the paint thread reads them and writes
  `_cvCenter/_cvHalf/_cvMid/_cvHalfPx/_cvAnchor/_cvRefHalf/_cvBandTop/_cvBandBot` (3063-3153), `_spRect/_acRect/_cvRect`
  (3018-3019, 3155), `_spRoom/_acRoom` (3298, 3631), `_axisX` (3025), `_winRect` (3010), `_cvAutoBadgeRect` (3267),
  `_tableRect` (4701, 5037), `_hitList` (4198, 4285). See 3.1.

### 1.5 Caches

| Cache | Field | Key / policy |
|---|---|---|
| Pens | `_pens` 755, `Pen()` 6320 | `PenKey` 6318 = ARGB<<32 \| float bits of width; never disposed until OnClear/OnSettingsUpdated... (NOTE: OnSettingsUpdated disposes fonts only, 1373-1378; pens/brushes survive) |
| Dotted pens | `_dotPens` 757, `DotPen()` 6332 | same key, `DashStyle.Dot` set once |
| Brushes | `_brushes` 779, `Brush()` 6344 | ARGB |
| Fonts | `_f/_fb/_fs` 749-753; card `_cf/_cfb/_cfs/_cfh` 6278-6284 at `_cfScale` 5041; reference `_rf*` 6286-6292 | built lazily in `Fonts()` 6237; disposed in OnClearLocked 1308-1312 and OnSettingsUpdated 1373-1378; card fonts rebuilt when the fit changes scale 4994-5000 |
| Paths | `_gdi` (GdiCache) 781 | `Path(id)` ids used: 1,2,3,10-17,20,21; `Clear()` 1327; IL also shows a `LinearGradientBrush` cache in GdiCache (methods at IL 33685+943 and +1026) that this indicator never calls |
| Work arrays | `_arrays` (PictureArrays) 789 | `BeginPaint()` 2632, `Get<T>(slot, n)` with slots 1,2 (CVD line 3203, 3221), 3,4 (slow line 3396, 3413), 6 (area 3420), 5 (`double[]` in LastBarSpeed 5611); `Clear()` 1328 |
| Points | `_points0/1/2` 783-787 | fixed-size reused for glyphs/marks |
| Text widths | `_textW` 5552, `TextW()` 5559 | (text, font id 0-10); cleared on DPI change 5568-5572, when > 2048 entries 5577-5580, and in `Fonts()` 6242/6257 |
| Table label width | `_labelW/_labelFont/_labelDpi` 759-763 | classic Panel 4670-4685 |
| Card templates | `_tplLabelW/_tplValueW/_tplTitleW` 5053-5057 | per reference font + DPI 5061-5087 |
| Last-bar texts | `_lastBarKey/_lastBarSpeed/_lastBarReading` 5538-5546 | invalidated by CopyForPaint 2665-2668 and OnSettingsUpdated 1380 |
| Flow text | `_flowTextKey/_flowText` 6442-6444 | classic `FlowText` only |
| Sessions | `_sessionCache` 723 | per UTC day, cleared at 400 entries 2574-2577 |
| Range median | `_rangeObsAt/_rangeObs` 743-745 | per forming bar |
| Card rows / groups | `_cardRows` 5289, `_cardGroupH/_cardGroupStart/_cardColStart/_splitFirst` 5043-5047, 5136 | reused lists/arrays |

### 1.6 Mouse handling

Hooked in OnPaintChart (2618-2621) via `HookMouse` 5754 (MouseWheel/Down/Move/Up on `IChart`), unhooked in OnClearLocked
1334. `SplitterAt` 5722 (the two separator lines, 5 px grab), `LaneAt` 5796 (the price panel at the right of a lane,
`e.X >= _axisX`), `BodyLaneAt` 5824 (inside a lane), `InPlatformUi` 5709 (title strip and the card). `OnWheel` 5860 zooms
the flow/accel lane scale or stretches the CVD pane (CvdToManual 3567, CvdStretch 3582). `OnDown` 5895: AUTO/MANUAL badge
5904-5914; splitter right/double-click resets heights 5917-5929; splitter drag start 5930-5939; CVD-body soft drag in
MANUAL 5950-5963; right/double-click on a panel resets that lane 5965-5986; panel drag start 5987-6016. `OnMove` 6024: soft
drag 6028-6046, splitter drags 6058-6102 (Pro: shares `_spFrac/_acFrac`; Classic: px `_spHpx/_acHpx`), axis zoom 6103-6125,
offset drag 6126-6141, else `HoverAt` 6153 (hit test on `_hitList` under `TryEnter(_paintSync)`). `OnUp` 6203: writes the
dragged shares back into `VelocityPanelHeight`/`AccelHeight` (6217-6224).

---

## 2. Settings audit

75 `[InputParameter]` fields (302-680). "Read at" = lines where the value is used. "Effect" = Yes when the value changes
the computation or the picture when changed through the dialog; "Recompute" = the value feeds `Compute`/`Evaluate`, so the
history only changes if the platform re-initialises the indicator after the dialog (VERIFIED comment in the official
example says OnInit runs "after its input params reset"; whether every dialog change counts as a reset: UNVERIFIED).
`OnSettingsUpdated` (1363-1390) itself only rebuilds fonts and resets pane shares; it calls neither `RecalcAll` nor
`RedrawBuffer`, so with `LiveReadout` off or a quiet market the picture changes only at the next platform repaint.

| # | Field (decl. line) | Name in dialog | Sort | Read at | Effect | Defect / note |
|---|---|---|---|---|---|---|
| 1 | `Mode` 313 | Display | 0 | 1272-1274, 2451-2469, 2618, 2699-2712, 4047-4052, 4086, 4208-4222, 4700, 4899, 4916, 5702, 5724, 5798, 5826, 5904 | Yes | — |
| 2 | `Reset` 328 | Reset | 1 | 1728-1729, 2479-2506 | Recompute | Session degrades silently to Day when the symbol has no primary session (2564-2567); Week = Monday 00:00 chart tz (2495-2503), documented in guide |
| 3 | `ResetBars` 331 | Reset every N bars | 2 | 2484-2490 | Recompute | only with Reset = Bars |
| 4 | `Skin` 380 | Theme | 3 | 989-1067 (BullColor/BearColor/SignalColor), 4828 | Yes | `SignalColor` has no `Theme.Custom` arm -> falls to `Color.Orange` (1066): with Custom theme the Velocity CVD line (3202) is orange |
| 5 | `CustomBull` 383 | Custom bull candle colour | 4 | 1002, 4828 | Yes (Custom only) | — |
| 6 | `CustomBear` 386 | Custom bear candle colour | 5 | 1029, 4828 | Yes (Custom only) | — |
| 7 | `LineWidth` 389 | Line / candle width | 6 | 1273, 2828, 2873, 3164, 3225 | Yes | — |
| 8 | `VelocityStyle` 398 | Velocity: top CVD style | **13** | 3039, 3091-3092, 3157, 3162, 4047 | Yes | duplicate sort index 13 with `VelocityCvd` (419) |
| 9 | `VelocityPeriod` 401 | Velocity: period | 7 | 1791-1792, 2977-2978 | Recompute | — |
| 10 | `VelocityPanelHeight` 404 | Velocity: panel height (%) | 8 | 2982, 3489; written by the mouse 5924, 6219 | Yes | a splitter drag overwrites the user's value (6219); right-click on a splitter forces 34 (5924), not the dialog value |
| 11 | `VelocityUseTheme` 407 | Velocity: use theme colours | 9 | 3020-3021, 3202, 3433, 3437 | Yes | default true makes #12-#15 dead until switched off |
| 12 | `VelocityUp` 410 | Velocity: rising flow | 10 | 3020 | Only if #11 off | — |
| 13 | `VelocityDown` 413 | Velocity: falling flow | 11 | 3021 | Only if #11 off | — |
| 14 | `VelocityAvg` 416 | Velocity: smoothed average line | 12 | 3433 (Pro), 3437 (Classic) | Only if #11 off | — |
| 15 | `VelocityCvd` 419 | Velocity: CVD line on top | **13** | 3202 | Only if #11 off AND #8 = Line | duplicate sort index 13; effectively dead with defaults (theme on, candles) |
| 16 | `Bars` 430 | Velocity: bar style | 64 | `HBar` 3929, 3936 (accel strip); `HBarTier` 3964 (flow lane) | Partly | **Gradient is implemented only in `HBar` (3936-3939). `HBarTier`, which draws the flow lane (3361), handles `Outline` (3964) and otherwise paints Solid + a dark 0.8 px outline (3971-3974) + highlight cap (3975-3980). So "Gradient (lit top)" changes only the acceleration strip, never the flow lane.** Sort index 64 puts it far from the other Velocity settings (7-13) |
| 17 | `LaneBack` 433 | Velocity: lane background | 65 | 3272 (flow), 3465 (accel) | Yes | not applied to the CVD pane; opaque fill, so a translucent colour shows the chart background through the lanes but the CVD pane keeps the chart's own background |
| 18 | `FlowScale` 436 | Flow speed: fixed scale | 67 | 3277-3278 | Yes | — |
| 19 | `AccelScale` 439 | Acceleration: fixed scale | 68 | 3634, 3636 | Yes | — |
| 20 | `LaneMouse` 442 | Lanes: mouse wheel/drag | 69 | 2618, 5724, 5798, 5826, 5904 | Yes | — |
| 21 | `ShowLaneText` 445 | Velocity: show lane captions / readouts | 66 | 3023-3024, 3229, 3441, 3818 | Yes | when off, `_axisW` is 0 (3024) so `_axisX = rect.Right` (3025): the lane price panels disappear and wheel/drag zoom of the lanes becomes impossible (LaneAt 5817); `_cvAutoBadgeRect` keeps its last rectangle (set only inside the text block, 3267) so a click there still toggles AUTO/MANUAL (5904) with no badge on screen |
| 22 | `ShowAccel` 448 | Acceleration: show strip | 14 | 2979-2988, 3005-3009, 3463, 3480-3493, 5747, 5849, 6064-6070 | Yes | — |
| 23 | `AccelMode` 457 | Acceleration: bar meaning | 15 | 3721 | Yes | only the bar direction; the words (AccelText/AccelWords) always use "with/against the side" |
| 24 | `AccelHeight` 460 | Acceleration: strip height (%) | 16 | 2982, 3490; written 5925, 6223 | Yes | same overwrite as #10 |
| 25 | `AccelLag` 463 | Acceleration: lag | 17 | 1793 | Recompute | — |
| 26 | `AccelSmooth` 466 | Acceleration: smoothing | 18 | 1794 | Recompute | — |
| 27 | `AccelMin` 469 | Acceleration: meaningful >= x | 60 | 3634, 3664, 3724, 4125-4127, 5524 | Yes | — |
| 28 | `AccelExtreme` 472 | Acceleration: extreme >= x | 61 | 3634, 3655, 3665, 3725, 4125, 5532, 5535 | Yes | — |
| 29 | `ShowAccelMarkers` 475 | Acceleration: show thrust/stall markers | 62 | 3633, 3774 | Yes | — |
| 30 | `ShowAccelText` 478 | Acceleration: show live reading text | 63 | 3829, 3833 | Yes | — |
| 31 | `LookbackBars` 481 | Lookback (bars) | 20 | 1817, 2384, 3616, 5610, 6396 | Recompute + paint | 6396: `FlowRank` cost is O(LookbackBars) per visible bar per paint (see 4) |
| 32 | `DominanceBars` 484 | Dominance lookback | 19 | 4639, 4797 | Yes | — |
| 33 | `VelocityBars` 487 | Velocity window for R/E | 21 | 1989, 2045, 2050, 2054 | Recompute | — |
| 34 | `DivergeMin` 490 | Divergence: both velocities >= | 22 | 1809 | Recompute | — |
| 35 | `DivergePersist` 493 | Divergence: persistence | 23 | 2093 | Recompute | — |
| 36 | `ExhaustDeltaPct` 496 | Exhaustion: push <= % | 24 | 2192 | Recompute | — |
| 37 | `ExhaustNeedVolume` 499 | Exhaustion: thinner volume | 25 | 2195 | Recompute | — |
| 38 | `AbsorbMult` 502 | Absorption: delta >= x | 26 | 2211, 2219 | Recompute | — |
| 39 | `AbsorbProgress` 505 | Absorption: progress <= x | 27 | 2211, 2219 | Recompute | — |
| 40 | `ClosedOnly` 508 | Closed bars only for SIGNALS | **28** | 1075, 1533, 1667 | Yes | duplicate sort index 28 (three fields) |
| 41 | `LiveReadout` 511 | LIVE readout | **28** | 1077, 1578, 3354, 3733 | Yes | duplicate 28 |
| 42 | `LiveFps` 514 | LIVE readout: max redraws/s | **28** | 1587 | Yes | duplicate 28; 0 disables the indicator's own redraw requests entirely (1588-1591) |
| 43 | `SignalGap` 517 | Minimum bars between same-type signals | 29 | 2255, 2296, 2330 | Recompute | — |
| 44 | `Filter` 528 | Show signals | 30 | 2421, 2427, 2437, 4249, 4261 | **Mostly none** | see 2.3 |
| 45 | `ShowAbsorption` 531 | Show absorption | 31 | 4204 | Yes | — |
| 46 | `ShowExhaustion` 534 | Show exhaustion | 32 | 4204 | Yes | — |
| 47 | `ShowDivergence` 537 | Show divergence | 33 | 4204 | Yes | hides flow AND swing divergences |
| 48 | `ShowSwingDiv` 540 | Swing divergence: show | 34 | 2104 | Recompute only | an evaluation-time gate, not a paint gate: switching it off leaves already-found swing markers on the chart until a reload |
| 49 | `SwingBars` 543 | Swing divergence: pivot bars | 35 | 2106 | Recompute | — |
| 50 | `Table` 554 | Info table | 40 | 2690, 2722, 3518, 4699, 4898-4899 | Yes | **`Corner.TopRight` (57) is not offered (variants 545-553 list only BottomRight, BottomLeft, Hidden) although 4899 tests for it.** In Velocity mode the classic Panel is always placed at the top (4700) whatever the "Bottom ..." choice; only left/right is honoured (4699) |
| 51 | `ShowScores` 557 | Show score labels | 41 | 4279 | Yes | — |
| 52 | `ShowZero` 560 | Show zero line | 42 | 1279 (`_zero.Visible`), 4075-4084 (range includes 0) | Yes | in Velocity mode with lane text the indicator draws its own zero line too (3252-3255): two zero lines |
| 53 | `TitleAreaWidth` 563 | Keep title area clear | 43 | 2682 (-> PaintClip 2757), 5711 | Yes | clip strip is 24 px high (2757), the mouse exclusion is 28 px (5711) and the card top is 28 px (4909, 4700): three constants for one idea |
| 54 | `FontSize` 566 | Font size | 44 | 6239 | Yes | — |
| 55 | `AbsorbColor` 569 | Absorption colour | 50 | 4174, 4221, 4497 | Yes | — |
| 56 | `ExhaustColor` 572 | Exhaustion colour | 51 | 4129, 4151, 4186, 4221, 4497, 4733, 5327, 5343-5344, 5452-5453, 5674 | Yes | also used for LOADING badge, TURNING chip, error chip, "fragile"/"stall" words |
| 57 | `DivergeColor` 575 | Divergence colour | 52 | 4221, 4497 | Yes | — |
| 58 | `TextColor` 578 | Text colour | 53 | 4281, 4552, 5348 | Partly | only score labels, the hover card's first line and the card title "CVD PRO"; every other text is hard-coded grey |
| 59 | `TableStyle` 589 | 2.0 Info table style | 70 | 2739 | Yes | — |
| 60 | `KeepFirstSignal` 592 | 2.0 Signals: keep the first marker | 71 | 2269, 2283, 4272, 4517 | Recompute | — |
| 61 | `SwingAtConfirm` 595 | 2.0 Swing divergence: marker on confirming bar | 72 | 4232 | Yes | — |
| 62 | `FlowTiers` 604 | 2.0 Flow speed tiers | 73 | 3328, 3453, 5504, 6422 | Yes | — |
| 63 | `ShowLastBar` 607 | 2.0 Table: last bar | 74 | 4825 | Yes | — |
| 64 | `HoverCards` 610 | 2.0 Signal card on hover | 75 | 2618, 2730, 4283, 6051 | Yes | — |
| 65 | `MarkerStyle` 621 | 2.0 Markers style | 76 | 4245, 4257 | Yes | guide table (make_guide 470) says default "Clean"; code default is Badge (621); guide 479 says Pro badges: the two guide rows disagree |
| 66 | `ResetBarsFromSession` 624 | 2.0 Reset every N bars from session | 77 | 1728, 2486 | Recompute | — |
| 67 | `CrispCandles` 627 | 2.0 Crisp candles | 78 | 2829, 3157, 3356-3360, 3378, 3749-3752, 3770 | Yes | toggles `SmoothingMode` twice per bar per lane (see 4) |
| 68 | `CardCorner` 647 | 2.1 Card place | 79 | 4888-4905, 4989 | Yes | — |
| 69 | `CardOffsetX` 650 | 2.1 Card move right | 80 | 5035 | Yes | — |
| 70 | `CardOffsetY` 653 | 2.1 Card move down | 81 | 5036 | Yes | — |
| 71 | `CardWidth` 656 | 2.1 Card width | 82 | 4925-4926 | Yes | — |
| 72 | `CardSize` 659 | 2.1 Card size (%) | 83 | 4936, 6254 | Yes | — |
| 73 | `CardOpacity` 662 | 2.1 Card background | 84 | 5316 | Yes | — |
| 74 | `CardLayout` 671 | 2.1 Card rows | 85 | 4846 | Yes | — |
| 75 | `LaneStyle` 680 | 2.1 Lanes style | 86 | 2990, 3023, 3264, 3313, 3360, 3417, 3444, 3451, 3632, 5917, 5920, 6060, 6083, 6214 | Yes | — |

### 2.1 Sort-index problems

- Index 13 twice: `VelocityStyle` (391) and `VelocityCvd` (418). Index 28 three times: `ClosedOnly` (507), `LiveReadout`
  (510), `LiveFps` (513). The kb page says parameters are "displayed in specified order"; the order among equal indexes is
  UNVERIFIED (probably declaration order).
- The Velocity look settings are split: 7-13, then 14-18 (acceleration), then 60-69 (`AccelMin`, `AccelExtreme`, markers,
  text, bar style, lane background, lane text, scales, mouse) after the signal/table block 19-53.

### 2.2 'Velocity: bar style' = Gradient

Confirmed dead for the flow lane: `HBarTier` (3942-3981) has no `Gradient` branch. The only Gradient code is `HBar`
3936-3939 (the acceleration strip). Fix: add the same lit-top strip to `HBarTier` after 3970, or route both lanes through
one function.

### 2.3 'Show signals' (Filter): why changing it changes nothing

Trace for a drawn marker: `Signals` 4204 -> `DrawBar(sig)` 2414-2422.

1. 2.0 signals (`KeepFirstSignal` = true, default) carry `Avg0 >= 0` and `DrawBar` returns
   `Filter == All ? Abs : (AboveAverage ? AtAvg : AtExt)` (2421). `AtAvg/AtExt` were set once by `Freeze` (2403-2409):
   `avg = AvgScoreIn(_sigs, Abs, Kind, swing)`; `AtAvg = (avg <= 0 || Score >= avg) ? Abs : -1`;
   `AtExt = (avg <= 0 || Score >= 1.5*avg) ? Abs : -1`.
2. `AvgScoreIn` (2369-2399) returns **0 when fewer than 3** closed signals of the same kind and the same swing-ness
   (`P1 >= 0`) lie within `LookbackBars` (60) bars before the signal (2394-2397).
3. Therefore whenever `Avg0 == 0`, `AtAvg == AtExt == Abs`, and all three filter values draw the marker on the same bar.
   With `SignalGap` merging repeats into one object (2296-2304), three same-kind, same-side-or-not, same-swing-ness
   closed signals inside 60 bars is uncommon; so for almost every signal the filter has no effect at all. The guide's own
   example text admits it ("fewer than 3 signals of its kind in the last 60 bars, so no average yet: every filter shows
   it", make_guide.py 146-147).
4. When `Avg0 > 0` the filter hides only signals with `Score < avg` (AboveAverage) or `Score < 1.5*avg` (Extreme); a signal
   that is already >= 1.5x average shows under every value.
5. Classic path (`KeepFirstSignal` off): `Passes` (2424-2442) recomputes the average on the picture copy and returns
   `true` when it is `<= 0` (2433-2436): the same <3 rule, so the same no-op.
6. Nothing forces a repaint: `OnSettingsUpdated` 1363-1390 never calls `RedrawBuffer`; the chart repaints on the next tick
   (LiveRedrawDue), mouse event, or platform repaint. With a quiet market the old picture stays.
7. Additionally the custom `Settings` setter (1083-1158) applies every enum (including `Filter`) by reflection
   (1120-1128) and removes it from the list handed to `base.Settings` (1136-1141). If the platform's "settings changed"
   handling lives in the base setter, an enum-only change is invisible to it (UNVERIFIED, flagged as a plausible
   contributor).

Net: the only visible change the user can get from the filter today is on the rare signals with `Avg0 > 0` (bigger or
smaller markers come from `_lastAvg` 2420 / 4249-4254, not from the filter). To make the filter do what its name says,
`Freeze` needs a fallback when `avg == 0` (e.g. a fixed score threshold per kind, or a global average over all kinds), and
`OnSettingsUpdated` should request a redraw.

---

## 3. Bugs and risks (with line numbers)

### 3.1 Thread safety

1. **View state written by mouse handlers without a lock and read by the painter** (see 1.4). All are 4/8-byte primitives
   or `RectangleF` (16 bytes, torn reads possible). Consequence: a one-frame glitch, a drag that jumps, a wrong hit test;
   not a crash. `_tableRect` (read 5715, written 4701/5037), `_cvAutoBadgeRect` (5904 / 3267), `_spRect/_acRect/_cvRect`
   (5845-5853 / 3018-3019, 3155), `_axisX` (5817, 5995 / 3025), `_spTop/_acTop` (5743, 5747 / 3006-3009), `_spRoom/_acRoom`
   (5959-5960, 6033, 6037 / 3298, 3631), `_cvCenter/_cvHalf` (5882, 5955-5956, 5993-5994 / 3113-3114, 3147-3148),
   `_cvHalfPx/_cvMid/_cvAnchor/_cvBandTop/_cvBandBot` (CvdNewestInView 5883, 6209; 6042 / 3063-3078). Whether mouse events
   and painting can really overlap is UNVERIFIED; the author's comment at 6172-6174 says they can.
2. **`OnTryGetMinMax` (4020-4118) holds `_sync` but reads painter state guarded by `_paintSync`** (`_cvOk`, `_winRect`,
   `_cvCenter`, `_cvHalf`, `_cvMid`, `_cvOff`, `_cvHalfPx` via `CvdV` 4090-4091, and `_reserveFrac` 4101). Also uses the
   previous picture's `_winRect` (3010): the platform scale lags one frame after a resize/mode switch.
3. **Public setting fields are written by the platform (reflection 1127 / `base.Settings` 1141) with no lock while
   `Compute`/`Evaluate`/paint read them.** A mode change mid-paint produces one inconsistent frame. Low severity.
4. **`VelocityPanelHeight`/`AccelHeight` are written from mouse handlers** (5924-5925, 6219-6223) — public settings mutated
   outside the dialog; whether the platform persists such writes with the workspace is UNVERIFIED (the guide claims it,
   make_guide.py 440).
5. `HookMouse` is called from inside the paint callback (2620): subscribing to chart events while the chart raises the
   paint event. Works in practice (the guard `_mouseChart == currentChart` 5757 keeps it to once), but the first paint
   subscribes from whatever thread paints.
6. `OnLiveTimer` may fire after `OnClearLocked` disposed the timer (1335-1337) — it only locks `_sync` and calls
   `CurrentChart?.RedrawBuffer()`, so harmless.

### 3.2 Correctness

7. **Pen `DashStyle` mutation on shared cached pens**, contradicting the code's own rule at 6331 ("the shared pens must
   not have their dash style changed"): `Signals` 4228-4231 (swing line), `GlyphBadge` 4339-4340 restored at 4359/4369,
   `GlyphClean` 4417-4418 restored at 4470, `Glyph` 4579-4580 restored at 4627. Any exception thrown between set and restore
   (OnPaintChart's catch at 2636 swallows it) leaves that colour+width pen dashed for every later use. Use `DotPen`-style
   dedicated pens (6332) for the dashed cases.
8. **`_sigs` snapshot cost**: `_sigsBefore.AddRange(_sigs)` (1524) copies up to 4000 references for every closed bar in
   live mode; `RecheckClosed` restores it (1568-1569). O(S) per bar, fine today; grows with the cap.
9. **`_pendingVa` can trigger repeated full recomputes**: when VA is "Finished" (1756 `VaReady()`) but a non-last bar still
   has no `Total`, `_pendingVa` is set; the next update that finds it (1493) runs `RecalcAll` (O(N)), which may set
   `_pendingVa` again to the next missing bar -> one RecalcAll per late bar in the worst case. Frequency UNVERIFIED.
10. **`XMap` (2761-2818) extrapolates every bar's X from one anchor and one slot width** (`X()` 2820). Any chart with
    non-uniform bar spacing, or where `GetBarIndex(time)` does not map 1:1 to the indicator's `abs` index (check at 2784
    `|barIndex - num6| < 0.5`), makes XMap fail (nothing but the table is drawn, 2690-2694) or drift. On range/tick charts
    two bars can share a timestamp; the uniqueness check 2780 skips them, so the anchor can land up to 600 bars back
    (2776). INFERRED fragility; no official example extrapolates this way (DrawOnBars.cs calls `GetChartX` per bar).
11. **`Progress()` time bars** (1890-1895): `F = (DateTimeUtcNow - bar.Ticks)/period` assumes `Time(offset)` is UTC
    (UNVERIFIED) and a live clock; during a replay/backtest or a stalled feed the forming bar's `F` is wrong (clamped to
    0..1, 1897). `F` feeds `Win` weighting (1849-1860), `AccE` (1800-1804), the card's "Bar now" row (4839-4840).
12. **`RangeFull` allocates `new double[num]` (1916)** — once per new forming bar (cached by `_rangeObsAt` 1907), not per
    tick; still an allocation on the compute thread for no reason (a fixed `double[21]` field would do; `_arrays` is
    paint-side and must not be used here).
13. **`SessionAnchor` interval test is closed on both ends** (2582): a bar whose open time equals one session's `To` and the
    next session's `From` is assigned to the earlier session, delaying the reset by one bar; depends on the interval
    semantics of `EnumerateSessionTimeFrames` (UNVERIFIED). `_hasPrimary` is cached once per OnInit (2560-2563): if the
    session info arrives after the first `Compute`, Session reset stays degraded to Day until a reload.
14. **`IsReset` Day/Week call `ChartDate` twice per `Compute`** (2494, 2497-2498), each calling `ChartTz()` (2512-2534,
    `CurrentChart?.CurrentTimeZone.TimeZoneInfo`) and `TimeZoneInfo.ConvertTimeFromUtc`. Fine per tick; a full
    `RecalcAll` over a long history does 2N conversions. A time-zone change in the chart does not recompute history.
15. **`ShowSwingDiv` is an evaluation gate only** (2104): markers already stored stay drawn after switching it off (their
    paint gate is `ShowDivergence`, 4204).
16. **Classic table in Velocity mode ignores "Bottom"** (4700 chooses top for Velocity), and `Corner.TopRight` is tested
    (4899) but not offered (545-553).
17. **`_hitList` goes stale when `XMap` fails** (not cleared at 2690): `HoverCard` 4474 then positions a card next to a box
    from an older picture. Minor.
18. **Right-click on a splitter resets to hard-coded 34/18** (5924-5925), not to the user's dialog values.
19. **`SignalColor` lacks a `Theme.Custom` arm** (1043-1067) -> orange CVD line for the Custom theme.
20. **`Verdict` uses `AbsorbColor`/`ExhaustColor` as semantic colours** (4152, 4174, 4186): changing "Exhaustion colour" also
    recolours the LOADING badge (5327), the error chip (5343), TURNING chips (5452) and "fragile" verdicts. Design choice,
    but surprising from the dialog.
21. **Guide vs code**: make_guide.py line 470 says "2.0 Markers style — Clean (default)"; code default is `Badge` (621) and
    line 479 says "Pro badges". Line 256 says "2.1", footer 577 says "2.2".
22. **Swallowed exceptions without a report** (platform errors vanish): 1110, 1143, 1609, 1899, 1937, 2530, 2544, 4114,
    5735, 5809, 5837, 6167. All others go through `TnxErrors.Report` (counted; shown on the card 5334-5347).
23. **No `Dispose()` override**: the official mouse example unsubscribes in `Dispose()`; this code relies on `OnClear`
    (1283) being called on removal (INFERRED). If the platform skips `OnClear` on removal, the chart keeps four delegates
    to a dead indicator.
24. **Int overflow**: none found. Candidates checked: `PenKey` (6318, long), `_sigsVersion++` (int, only compared with
    `!=`), `1000L/num` (1597, num >= 1 by 1587-1591), `ticks % 864000000000L` (2569, long), `CardOpacity*255/100` (5316),
    `num65*45/100` (3361), `(int)Math.Round(_spFrac*100f)` (6219).
25. **Divide by zero**: none unguarded found. Checked: `Win(...)/num14` (1798, >= 1), `ScaleP/ScaleC` (`num > 1`),
    `AccNorm` (3618 guard), `FlowRank` (n >= 20), `num64` (3321 guard), `num51` (3277 floor 1e-9, `_spZoom >= 0.05`),
    `num6` (3634, AccelMin >= 0.2), XMap slope (2809), `CvdY/CvdV` (`_cvHalf >= 1e-6`, `_cvHalfPx >= 4`), `Verdict`
    (4168, num >= 0.7), `d/d2` (2190), `v/max(v2,1e-9)` (2197), `Score/avg` (4253, avg > 0), Panel shares (4645-4646),
    `LastBarSpeed` (5623-5635), ticks (`num8 > 0` 4660), `Progress` (1878, 1885, 1891), `PaneHeights` (3485, 3500),
    `OnMove` (6042 `Max(1f,…)`, 6105 `_dragH >= 20`), `Axis` (`scale > 0`).

### 3.3 Settings mechanics

26. `OnSettingsUpdated` (1363-1390) rebuilds fonts and resets `_spFrac/_acFrac` but neither recomputes (`RecalcAll`) nor
    redraws. Every "Recompute" setting in section 2 depends on the platform re-initialising the indicator (OnInit 1237
    resets everything) — VERIFIED that OnInit runs "after its input params reset", UNVERIFIED that every dialog change
    is such a reset. If it is, the custom `Settings` setter's reflection path (1127) for enums bypasses `base.Settings`
    for those items, which may skip that reset for enum-only changes (UNVERIFIED).
27. The `Settings` setter's fallback (1143-1156) sets items one by one, each call possibly triggering platform work.

---

## 4. Performance

Per paint (every repaint; with `LiveReadout` on, up to `LiveFps` = 20 per second from the indicator alone, 1587-1613):

| Driver | Where | Cost |
|---|---|---|
| `FlowRank` | 3328 (per visible bar, `FlowTiers` = Percentile default), 3453, 5504 (x2 via PanelCard 4813-4814) | O(min(LookbackBars, abs)) each -> visible bars x 60 comparisons; 5000 lookback -> 5000 per bar. The single largest per-bar cost. |
| `AccNorm` | 3653 (vmax loop) + 3718 (bar loop) + 3827 + 4651/4815 | O(1) each thanks to `PAbsAcc` prefix sums (`Win` 1846); computed twice per visible bar |
| `ScaleC` per bar | 3320 | O(1) (prefix sums) |
| CVD scale loops | 3033-3060 (all visible), 3088-3104 (last 25) | O(visible) |
| GDI calls per visible bar | flow lane: HBarTier 1-3 fills/strokes (3970-3979) + cap (3365) + hollow (3374) + crossing dots 2 (3391-3392); accel: HBar 1-2 + cap + hollow outline + AccelMarkPro 2-5; CVD crisp candles 2-3 (2901-2919) | roughly 6-12 GDI calls per bar in Velocity mode |
| `SmoothingMode` toggles | 3358/3379, 3751/3772 | 2 state changes per bar per lane when `CrispCandles` |
| Clip resets | `PaintClip` 2683, 2716, 3228, 3468, 3517; `LaneClip` 3277, 3428, 3431, 3464 (+ 3426/3429 SetClip) | ~12-14 `SetClip`/`ExcludeClip` per paint, each a GDI+ Region |
| Polygons/lines | 3427, 3430 (`FillPolygon` n+2 points, x2), 3433/3438 `DrawLines`, 3225 | O(visible) |
| `Signals` loop | 4199 iterates ALL `_psigs` (<= 4000) | O(S) per paint even when few are visible; the list is sorted by `Abs`, so a binary search for `vL` would cut it |
| `MeasureString` | behind `TextW` 5559; misses on every new dynamic string (numbers in the card/pills change every tick); cache wiped at 2048 entries (5577) and on every `Fonts()` rebuild | with live numbers the cache churns; `CardTemplates` 5061 measures 20 strings only on font/DPI change; `HoverCard` 4522-4526 measures 4-6 strings per paint while hovering |
| String allocation | PanelCard 4813-4844, 4882, 4885 (~12 strings), `FlowWords` 5509/5513 x3, `AccelWords` 5527-5535 x2, `Axis` 3986-4005 (3 per lane), `Fmt` 3248/3260, `LaneTitle` name 3455, classic `Panel` 4655-4668 incl. `new string[9]` and closure objects for `Row`/`MeterRow` (IL `<>c__DisplayClass331_0`), `HoverCard` 4512/4521 | the guide measures 1.3 / 2.3 KB per picture (make_guide 518) |
| Card fit | 4942-4972: up to 18 sizes x 2 columns x `ColumnsHeight` -> `BestSplit` (recursive, <= 10 splits) | small but runs every paint |

Per tick (compute thread): `Compute` O(1) per bar (1-2 bars), `Evaluate` O(Lb + SwingBars + VelocityBars) ~ 70 steps,
`_sigsBefore` copy O(S) per closed bar, `Add` scans back up to 5000 bars of signals (2289-2305).

What must NOT be made heavier:
- The four per-visible-bar loops: flow lane 3309-3395, accel 3705-3817, CVD candles 2876-2921, `Signals` 4199-4288. Any new
  per-bar quantity that needs a window (rank, percentile, median, "busy" score) must be computed once in `Compute`
  (1695-1813, prefix sums or stored fields) and only READ in paint. Never add another `FlowRank`-like O(Lookback) call
  per bar in paint; if a rank is needed per bar, compute it in `Compute` for closed bars (abs+1 bars at close time) and
  store it in `Bar`.
- Text: no `MeasureString` outside `TextW`; avoid strings that change every tick (quantise numbers, or cache by key as
  `_lastBarKey` 4828-4834 and `_flowTextKey` 6429-6436 do).
- GDI objects: only `Pen()`/`Brush()`/`DotPen()`/`_gdi.Path()`/`_arrays.Get()`; no `new Pen/Brush/Font/Region/Path` in
  paint; no `Graphics.Clip` reads (comment 2716).
- Locks: nothing new under `_sync` inside the picture besides `CopyForPaint`; the mouse must keep `TryEnter`.
- Do not widen `_sigs` scans or the 4000 cap without indexing.

---

## 5. Extension points

### (a) A 'flow lane style' setting (bars / polarized area / histogram+line / ribbon / heat strip)

- Declare a new enum + `[InputParameter]` next to `Bars` (421-430); give it a free sort index (e.g. 64 is taken; use 87+ or
  renumber the Velocity block).
- Branch point for the fast bars: the per-bar loop 3309-3395 in `VelocityPanel`; today every bar goes through `HBarTier`
  3361 with (x `num62`, zero `num54`, width `num12`, height `num61`, up `flag2`, colour `col`, alpha `num65`, lane top/bottom
  `num56/num57`, `highlight`, cap colour). The tier (0-3) is at 3329, the side colour at 3322, the live-bar hollow at
  3367-3376, the fast/slow crossing dots at 3382-3394.
- Branch point for the slow line/area: 3396-3440. Points are built once into `_arrays.Get<PointF>(3, num)` (3396) and
  trimmed to `exact2` slot 4 (3413); the Pro area polygon uses slot 6 (3420-3423) and two clipped `FillPolygon` calls
  (3426-3431) above/below zero, then `DrawLines` 3433 (classic: `DotPen` 3437).
- Reusable arrays: slots 1-6 are taken (1,2 CVD line; 3,4,6 slow; 5 `double[]`). New styles should take slots 7+ (a
  polarized area for the FAST series needs its own PointF slot; a ribbon needs two). `PictureArrays.Get<T>(slot, n)`
  returns an array of at least `n` (exact-length copies are done by hand at 3221-3224 and 3413-3416, needed because
  `DrawLines/FillPolygon` take the whole array).
- A heat strip can reuse the tier and `FlowRank` already computed per bar (3328-3329) and paint one `FillRectangle` per bar
  of height `num13`.. lane bottom; the colour ramp should come from `Brush()` with quantised alpha (avoid 255 distinct
  brushes per paint: quantise alpha to e.g. 8 steps).
- Gradients: `GdiCache` already has a `LinearGradientBrush` cache (IL 33685+943, +1026, fields `_gradients`, `_unused`,
  `GradientKey{X,Y,W,H,Argb1,Argb2,Angle,ByRectangle}`), unused by this indicator; its C# source is in TNX-COMMON (not
  uploaded) — method names UNVERIFIED, read `..\..\TNX-COMMON\GdiCache.cs` before using it.
- Keep `LaneClip` (3277, 3428, 3431) around any new fill so nothing leaks into the CVD pane or the accel strip, and keep the
  `_laneFlow*` fields (3302-3306) updated because the test harness reads them (comment 954).
- `OnSettingsUpdated` needs no change for a paint-only style, but a redraw request would make the switch immediate.

### (b) New per-bar fields (imbalance %, volume/sec, busy rank, real-push quality)

- Add fields to `Bar` (144-217). `CopyForPaint` copies whole structs (`Array.Copy` 2660) and `_dirtyFrom` is already set
  for every computed bar (1722-1725), so the picture sees them with no further change.
- Compute them in `Compute` after the VA read (1739-1745) and before/after the prefix sums (1769-1789):
  - imbalance % = `Vol > 0 ? Delta / Vol * 100 : 0` (or `(Buy-Sell)/(Buy+Sell)`); matches Quantower's `DeltaPercent`
    definition (kb page: "difference (%) in traded Volume between Buyers and Sellers"; mirror shows `Delta/Volume*100`).
  - volume/sec needs the bar's duration: closed bar = `_b[abs+1].Ticks - _b[abs].Ticks` (as `LastBarSpeed` 5609 does on the
    picture) — in `Compute(abs)` the next bar is not known yet, so compute it when bar `abs+1` is computed (the loop at
    1500-1504 already recomputes `abs-1`), or store duration on the forming bar from `DateTimeUtcNow - Ticks` (1893 style).
    `Bar.Ticks` is the OPEN time (1727).
  - busy rank / any percentile over `LookbackBars`: compute once per closed bar in `Compute` (O(Lookback) per bar on the
    compute thread, like `FlowRank` 6394-6417 but on `_b`) and store the result; do not call it per bar in paint.
  - real-push quality = f(e, r) from `BarRead` (2011-2018) which today is recomputed in paint (4653, 4808, 5653); store
    `e`, `r` (or the derived quality) in `Bar` at 1790.
- Add a `Pf` member (290-300) and a prefix field if a windowed sum is needed (`Pref` 1820-1838, prefix update 1769-1789).
- Readers: card rows in `PanelCard` 4846-4886 (`CardRow` kind 0/1/2/3), classic `Panel` 4744-4752, lane pills
  (`_ltFlow` 3455, `_ltAcc` 3831/3839), hover card lines 4512-4520. `LiveRedrawDue` (1583) compares `C/Fast/Px/Vol` to decide
  whether a tick is worth a redraw — add new live fields there if they change without those four changing.
- `RecheckClosed` (1555) compares the VA fields to decide a re-read; new VA-derived fields need no change.

### (c) A compass drawn inside PanelCard/DrawCard

- `CardRow` (5264-5287) has `Kind` 0 text, 1 meter, 2 split bar, 3 chip, 4 gap, 5 live strip, 6 hero number. Add Kind 7
  (compass) with `V` = angle/score and `Full` = scale, built at 4846-4886 (the row list is rebuilt every paint, same rows
  for a chart so the layout does not move — keep that rule, comment 4845).
- Row height: kinds other than 6 use `rowH` (5027, 5109, 5487). A taller compass must be special-cased in three places:
  column height 5023-5027, `CardGroups` 5109 (template height at 100 %), and the `DrawCard` row switch 5377-5400 (advance
  `cy` by its own height and `continue`, as Kind 6 does at 5399-5400). Width: if it is wider than `_tplValueW`, add a
  template measurement in `CardTemplates` 5074-5083.
- Draw in the `switch (row.Kind)` at 5404-5486 with `vx` (value column x), `rx` (right edge), `cy`, `rowH`, scale `k`,
  colours via `Brush()`/`Pen()`; use `_gdi.Path(n)` with an unused id (18, 19, 22+) for arcs; the card clip is set at 5320.
- The compass needs `BullColor/BearColor` (989-1041) and the `dim/soft` colours passed in (4810-4811).

### (d) Gating THRUST / STALL markers in AccelStrip

- Marker decision (Pro): 3780-3782 `if (_pb[num11].Flip || (flag6 && _pb[j].Real)) AccelMarkPro(...)` with `flag6 = num12 >=
  AccelExtreme` (3725), `flag3` = with the side (3720). Classic: 3786-3816 (`flag6 && _pb[j].Real` at 3790).
- The same threshold produces the words: `AccelWords` 5531-5535 (card "Speed" row 4815 and the lane pill 3838) and
  `AccelText` 4125 (classic table 4651). A gate (e.g. flow tier >= 2 from `FlowRank`, or a stored real-push quality from (b),
  or `Real` and `Vol >= typical`) must be applied at 3780, 3790, 5529-5535 and 4125 so marks and words agree. The bar
  colour/alpha (3741-3747) can stay.
- The lane's own scale (3640-3655) and level lines (3664-3703) are independent of the gate.

### (e) Making the acceleration lane optional while keeping its info in the flow lane

- `ShowAccel` already removes the strip: heights 2979-2988 / `PaneHeights` 3480-3493, separators 3014-3016, rects 3019,
  drawing 3463-3467, mouse 5747, 5849, 6064-6070. `AccNorm` (3614-3623) only needs `_pb` and works anywhere in paint.
- To fold the information into the flow lane: in the per-bar loop 3309-3395 use `AccNorm(num60)` to (1) tint or cap the
  fast bar (the existing cap at 3362-3366 is a 2 px `FillRectangle` at the lane edge — reuse that idea at the bar's end
  for "speeding up/slowing"), or (2) draw the THRUST/STALL/flip marks (`AccelMarkPro` 3876 is self-contained: x, zero y,
  bar height, radius, side colours, lane top/bottom) just above/below the fast bar; and merge the acceleration words into
  the flow pill: `_ltFlow` (3455) and `_ltAcc` (3831, 3839) are both `LaneTitleAsk` drawn by `DrawLaneTitles` 3511; a second
  text or a second dot row in `LaneTitle` 3531 is the smallest change. The card's "Speed" row (4859) is already independent
  of `ShowAccel`.
- Keep `_laneAcc*` fields (3658-3662) consistent (test harness reads them) — when the strip is off they are stale from the
  last picture that had it; set them to zero in the `!showAccel` path.
