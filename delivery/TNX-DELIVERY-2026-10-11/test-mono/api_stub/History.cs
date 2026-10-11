// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;
using System.Collections.Generic;

namespace TradingPlatform.BusinessLayer
{
    public enum SeekOriginHistory
    {
        Begin,   // VERIFIED(ex:DrawOnBars.cs, HistoryExamples.cs)
        End,     // VERIFIED(Scripts: IndicatorCumulativeDelta.cs); the default origin of LineSeries.SetValue (VERIFIED(IL): 3rd arg filled in by the compiler)
    }

    public enum BasePeriod
    {
        Tick,    // VERIFIED(Scripts)
        Second,  // VERIFIED(src)
        Minute,  // VERIFIED(src)
        Hour,    // VERIFIED(src)
        Day,     // VERIFIED(Scripts)
        Week,    // VERIFIED(Scripts)
        Month,   // VERIFIED(Scripts)
        Year,    // VERIFIED(Scripts)
    }

    public enum HistoryType
    {
        Last,    // VERIFIED(Scripts)
        BidAsk,  // VERIFIED(ex:HistoryExamples.cs)
        Bid,     // ASSUMED
        Ask,     // ASSUMED
        Mark,    // ASSUMED
    }

    public enum PriceType
    {
        Open,             // VERIFIED(src)
        High,             // VERIFIED(src)
        Low,              // VERIFIED(src)
        Close,            // VERIFIED(src)
        Volume,           // VERIFIED(src)
        Median,           // VERIFIED(ex:HistoryExamples.cs)
        Typical,          // VERIFIED(ex:HistoryExamples.cs)
        Weighted,         // VERIFIED(ex:HistoryExamples.cs)
        Ticks,            // VERIFIED(ex:HistoryExamples.cs)
        OpenInterest,     // VERIFIED(Scripts)
        QuoteAssetVolume, // VERIFIED(Scripts)
        Bid,              // VERIFIED(ex:HistoryExamples.cs)
        BidSize,          // VERIFIED(ex:HistoryExamples.cs)
        Ask,              // VERIFIED(ex:HistoryExamples.cs)
        AskSize,          // VERIFIED(ex:HistoryExamples.cs)
        Last,             // VERIFIED(ex:HistoryExamples.cs)
    }

    public enum RenkoStyle
    {
        Classic,          // ASSUMED
        AdvancedClassic,  // VERIFIED(ex:HistoryExamples.cs)
    }

    public enum PointsAndFiguresStyle
    {
        Close,    // ASSUMED
        HighLow,  // VERIFIED(ex:HistoryExamples.cs)
    }

    /// <summary>
    /// VERIFIED(IL): Period is a VALUE type (get_BasePeriod/get_PeriodMultiplier/get_Ticks invoked with 'call' on a local);
    /// BasePeriod : BasePeriod, PeriodMultiplier : int, Ticks : long. Static presets VERIFIED(ex:HistoryExamples.cs & Scripts): MIN1, HOUR1, TICK1, DAY1, YEAR1, MIN5, SECOND30.
    /// </summary>
    public readonly struct Period
    {
        public Period(BasePeriod basePeriod, int periodMultiplier)   // ASSUMED ctor (Scripts: new HistoryAggregationTime(period, ...))
        {
            this.BasePeriod = basePeriod;
            this.PeriodMultiplier = periodMultiplier;
        }

        public BasePeriod BasePeriod { get; }       // VERIFIED(IL)
        public int PeriodMultiplier { get; }        // VERIFIED(IL) int32
        public long Ticks                           // VERIFIED(IL) int64
        {
            get
            {
                long unit;
                switch (this.BasePeriod)
                {
                    case BasePeriod.Second: unit = TimeSpan.TicksPerSecond; break;
                    case BasePeriod.Minute: unit = TimeSpan.TicksPerMinute; break;
                    case BasePeriod.Hour: unit = TimeSpan.TicksPerHour; break;
                    case BasePeriod.Day: unit = TimeSpan.TicksPerDay; break;
                    case BasePeriod.Week: unit = TimeSpan.TicksPerDay * 7; break;
                    case BasePeriod.Month: unit = TimeSpan.TicksPerDay * 30; break;
                    case BasePeriod.Year: unit = TimeSpan.TicksPerDay * 365; break;
                    default: unit = 0; break;
                }
                return unit * this.PeriodMultiplier;
            }
        }
        public TimeSpan Duration => new TimeSpan(this.Ticks);   // VERIFIED(Scripts: Period.Duration)

        public static readonly Period TICK1 = new Period(BasePeriod.Tick, 1);       // VERIFIED(ex:HistoryExamples.cs)
        public static readonly Period SECOND1 = new Period(BasePeriod.Second, 1);   // ASSUMED
        public static readonly Period SECOND30 = new Period(BasePeriod.Second, 30); // VERIFIED(Scripts)
        public static readonly Period MIN1 = new Period(BasePeriod.Minute, 1);      // VERIFIED(ex:HistoryExamples.cs)
        public static readonly Period MIN5 = new Period(BasePeriod.Minute, 5);      // VERIFIED(Scripts)
        public static readonly Period MIN15 = new Period(BasePeriod.Minute, 15);    // ASSUMED
        public static readonly Period MIN30 = new Period(BasePeriod.Minute, 30);    // ASSUMED
        public static readonly Period HOUR1 = new Period(BasePeriod.Hour, 1);       // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs)
        public static readonly Period HOUR4 = new Period(BasePeriod.Hour, 4);       // ASSUMED
        public static readonly Period DAY1 = new Period(BasePeriod.Day, 1);         // VERIFIED(Scripts)
        public static readonly Period WEEK1 = new Period(BasePeriod.Week, 1);       // ASSUMED
        public static readonly Period MONTH1 = new Period(BasePeriod.Month, 1);     // ASSUMED
        public static readonly Period YEAR1 = new Period(BasePeriod.Year, 1);       // VERIFIED(Scripts)

        public override string ToString() => this.PeriodMultiplier + " " + this.BasePeriod;
    }

    /// <summary>Base aggregation. VERIFIED(IL): HistoricalData.get_Aggregation() : HistoryAggregation; the user compares GetType() == typeof(HistoryAggregationTime).</summary>
    public abstract class HistoryAggregation
    {
        public HistoryType GetHistoryType { get; set; }   // VERIFIED(Scripts: this.HistoricalData.Aggregation.GetHistoryType) - name as used there
        public virtual string Name => this.GetType().Name; // ASSUMED
    }

    public class HistoryAggregationTime : HistoryAggregation   // VERIFIED(IL) get_Period() : Period
    {
        public HistoryAggregationTime() { }
        public HistoryAggregationTime(Period period, HistoryType historyType = HistoryType.Last) { this.Period = period; this.GetHistoryType = historyType; } // VERIFIED(Scripts) 2-arg ctor
        public Period Period { get; set; }   // VERIFIED(IL)
    }

    public class HistoryAggregationTick : HistoryAggregation   // VERIFIED(Scripts: new HistoryAggregationTick(HistoryType.Last); 'is HistoryAggregationTick')
    {
        public HistoryAggregationTick() { }
        public HistoryAggregationTick(HistoryType historyType) { this.GetHistoryType = historyType; }
    }

    public class HistoryAggregationTickBars : HistoryAggregation   // VERIFIED(IL) get_TicksCount() : int32
    {
        public int TicksCount { get; set; }   // VERIFIED(IL)
    }

    public class HistoryAggregationRangeBars : HistoryAggregation  // VERIFIED(IL) get_RangeBars() : int32
    {
        public int RangeBars { get; set; }    // VERIFIED(IL)
    }

    public class HistoryAggregationRenko : HistoryAggregation      // VERIFIED(IL) get_BrickSize() : int32
    {
        public HistoryAggregationRenko() { }
        // VERIFIED(ex:HistoryExamples.cs): new HistoryAggregationRenko(Period.MIN1, symbol.HistoryType, 10, RenkoStyle.AdvancedClassic, 100, 100, true, true)
        public HistoryAggregationRenko(Period period, HistoryType historyType, int brickSize, RenkoStyle style, int extension, int inversion, bool showWicks, bool buildCurrentBar)
        {
            this.Period = period; this.GetHistoryType = historyType; this.BrickSize = brickSize; this.Style = style;
            this.Extension = extension; this.Inversion = inversion; this.ShowWicks = showWicks; this.BuildCurrentBar = buildCurrentBar;
        }
        public Period Period { get; set; }        // ASSUMED
        public int BrickSize { get; set; }        // VERIFIED(IL)
        public RenkoStyle Style { get; set; }     // ASSUMED name
        public int Extension { get; set; }        // ASSUMED name
        public int Inversion { get; set; }        // ASSUMED name
        public bool ShowWicks { get; set; }       // ASSUMED name
        public bool BuildCurrentBar { get; set; } // ASSUMED name
    }

    public class HistoryAggregationPointsAndFigures : HistoryAggregation
    {
        public HistoryAggregationPointsAndFigures() { }
        // VERIFIED(ex:HistoryExamples.cs): new HistoryAggregationPointsAndFigures(Period.TICK1, symbol.HistoryType, 100, 50, PointsAndFiguresStyle.HighLow)
        public HistoryAggregationPointsAndFigures(Period period, HistoryType historyType, int boxSize, int reversal, PointsAndFiguresStyle style)
        {
            this.Period = period; this.GetHistoryType = historyType; this.BoxSize = boxSize; this.Reversal = reversal; this.Style = style;
        }
        public Period Period { get; set; }                  // ASSUMED
        public int BoxSize { get; set; }                    // ASSUMED name
        public int Reversal { get; set; }                   // ASSUMED name
        public PointsAndFiguresStyle Style { get; set; }    // ASSUMED name
    }

    /// <summary>VERIFIED(ex:HistoryExamples.cs): object initializer with Aggregation, FromTime, ToTime, ForceReload. SessionsContainer VERIFIED(Scripts).</summary>
    public class HistoryRequestParameters
    {
        public Symbol Symbol { get; set; }                          // ASSUMED
        public HistoryAggregation Aggregation { get; set; }         // VERIFIED(ex:HistoryExamples.cs)
        public HistoryType HistoryType { get; set; }                // ASSUMED
        public DateTime FromTime { get; set; }                      // VERIFIED(ex:HistoryExamples.cs)
        public DateTime ToTime { get; set; }                        // VERIFIED(ex:HistoryExamples.cs)
        public bool ForceReload { get; set; }                       // VERIFIED(ex:HistoryExamples.cs)
        public ISessionsContainer SessionsContainer { get; set; }   // VERIFIED(Scripts: IndicatorCumulativeDelta.cs)
    }

    /// <summary>VERIFIED(ex:HistoryExamples.cs): TimeLeft, this[PriceType]. VolumeAnalysisData VERIFIED(Scripts: HistoricalData[i, Begin].VolumeAnalysisData).</summary>
    public interface IHistoryItem : ICloneable
    {
        DateTime TimeLeft { get; }                          // VERIFIED(ex:HistoryExamples.cs)
        DateTime TimeRight { get; }                         // ASSUMED (not seen in the examples; commonly used)
        double this[PriceType priceType] { get; }           // VERIFIED(ex:HistoryExamples.cs)
        VolumeAnalysisData VolumeAnalysisData { get; }      // VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs bar.VolumeAnalysisData; Scripts on IHistoryItem)
    }

    /// <summary>VERIFIED(ex:HistoryExamples.cs, DrawOnBars.cs): Open/High/Low/Close/Median/Typical/Weighted/Ticks/Volume : double, TimeLeft.</summary>
    public class HistoryItemBar : IHistoryItem
    {
        public DateTime TimeLeft { get; set; }                // VERIFIED(ex)
        public DateTime TimeRight { get; set; }               // ASSUMED
        public double Open { get; set; }                      // VERIFIED(ex)
        public double High { get; set; }                      // VERIFIED(ex)
        public double Low { get; set; }                       // VERIFIED(ex)
        public double Close { get; set; }                     // VERIFIED(ex)
        public double Median => (this.High + this.Low) / 2.0;                       // VERIFIED(ex:HistoryExamples.cs)
        public double Typical => (this.High + this.Low + this.Close) / 3.0;         // VERIFIED(ex:HistoryExamples.cs)
        public double Weighted => (this.High + this.Low + 2.0 * this.Close) / 4.0;  // VERIFIED(ex:HistoryExamples.cs)
        public double Ticks { get; set; }                     // VERIFIED(ex:HistoryExamples.cs) assigned to a double
        public double Volume { get; set; }                    // VERIFIED(ex)
        public double OpenInterest { get; set; }              // ASSUMED
        public VolumeAnalysisData VolumeAnalysisData { get; set; }   // VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs)

        public double this[PriceType priceType]               // VERIFIED(ex:HistoryExamples.cs)
        {
            get
            {
                switch (priceType)
                {
                    case PriceType.Open: return this.Open;
                    case PriceType.High: return this.High;
                    case PriceType.Low: return this.Low;
                    case PriceType.Close: return this.Close;
                    case PriceType.Median: return this.Median;
                    case PriceType.Typical: return this.Typical;
                    case PriceType.Weighted: return this.Weighted;
                    case PriceType.Ticks: return this.Ticks;
                    case PriceType.Volume: return this.Volume;
                    case PriceType.OpenInterest: return this.OpenInterest;
                    default: return double.NaN;
                }
            }
        }
        public object Clone() => this.MemberwiseClone();
    }

    /// <summary>VERIFIED(ex:HistoryExamples.cs): Bid, BidSize, Ask, AskSize.</summary>
    public class HistoryItemTick : IHistoryItem
    {
        public DateTime TimeLeft { get; set; }
        public DateTime TimeRight { get; set; }   // ASSUMED
        public double Bid { get; set; }
        public double BidSize { get; set; }
        public double Ask { get; set; }
        public double AskSize { get; set; }
        public VolumeAnalysisData VolumeAnalysisData => null;
        public double this[PriceType priceType]
        {
            get
            {
                switch (priceType)
                {
                    case PriceType.Bid: return this.Bid;
                    case PriceType.BidSize: return this.BidSize;
                    case PriceType.Ask: return this.Ask;
                    case PriceType.AskSize: return this.AskSize;
                    default: return double.NaN;
                }
            }
        }
        public object Clone() => this.MemberwiseClone();
    }

    /// <summary>VERIFIED(ex:HistoryExamples.cs): Price, Volume.</summary>
    public class HistoryItemLast : IHistoryItem
    {
        public DateTime TimeLeft { get; set; }
        public DateTime TimeRight { get; set; }   // ASSUMED
        public double Price { get; set; }
        public double Volume { get; set; }
        public VolumeAnalysisData VolumeAnalysisData => null;
        public double this[PriceType priceType]
        {
            get
            {
                switch (priceType)
                {
                    case PriceType.Last: return this.Price;
                    case PriceType.Volume: return this.Volume;
                    default: return double.NaN;
                }
            }
        }
        public object Clone() => this.MemberwiseClone();
    }

    /// <summary>VERIFIED(ex:HistoryExamples.cs): e.HistoryItem in HistoryItemUpdated / NewHistoryItem handlers.</summary>
    public class HistoryEventArgs : EventArgs
    {
        public IHistoryItem HistoryItem { get; set; }
    }

    /// <summary>
    /// VERIFIED(IL): get_Aggregation() : HistoryAggregation, get_VolumeAnalysisCalculationProgress() : IVolumeAnalysisCalculationProgress (reference type).
    /// Count, this[int], this[int, SeekOriginHistory], events, Dispose VERIFIED(ex:HistoryExamples.cs, DrawOnBars.cs, AccessCustomVolumeAnalysisData.cs).
    /// </summary>
    public class HistoricalData : IDisposable
    {
        protected readonly List<IHistoryItem> items = new List<IHistoryItem>();

        public HistoryAggregation Aggregation { get; set; }                                               // VERIFIED(IL)
        public IVolumeAnalysisCalculationProgress VolumeAnalysisCalculationProgress { get; set; }         // VERIFIED(IL)
        public int Count => this.items.Count;                                                             // VERIFIED(ex)
        public Symbol Symbol { get; set; }                                                                // ASSUMED
        public DateTime FromTime { get; set; }                                                            // VERIFIED(Scripts: HistoricalData.FromTime)
        public DateTime ToTime { get; set; }                                                              // ASSUMED

        public IHistoryItem this[int offset] => this[offset, SeekOriginHistory.End];                      // VERIFIED(ex:HistoryExamples.cs) historicalData[0]
        public IHistoryItem this[int index, SeekOriginHistory origin]                                     // VERIFIED(ex:DrawOnBars.cs) HistoricalData[i, SeekOriginHistory.Begin]
        {
            get
            {
                int i = origin == SeekOriginHistory.Begin ? index : this.items.Count - 1 - index;
                return i >= 0 && i < this.items.Count ? this.items[i] : null;
            }
        }

        public double GetIndexByTime(long ticks, SeekOriginHistory origin = SeekOriginHistory.End) => -1;  // VERIFIED(ex:TestIndicatorWithOneMoreHistoricalData.cs; Scripts) callers cast the result to int -> non-int return; double ASSUMED

        public event EventHandler<HistoryEventArgs> HistoryItemUpdated;   // VERIFIED(ex:HistoryExamples.cs)
        public event EventHandler<HistoryEventArgs> NewHistoryItem;       // VERIFIED(ex:HistoryExamples.cs)
        protected virtual void OnHistoryItemUpdated(HistoryEventArgs e) => this.HistoryItemUpdated?.Invoke(this, e);   // ASSUMED (raiser, keeps the stub warning-free)
        protected virtual void OnNewHistoryItem(HistoryEventArgs e) => this.NewHistoryItem?.Invoke(this, e);           // ASSUMED (raiser, keeps the stub warning-free)

        public void AddIndicator(Indicator indicator) { }                 // VERIFIED(ex:DoubleSMAIndicator.cs) smaSourceHD.AddIndicator(...)
        public void RemoveIndicator(Indicator indicator) { }              // ASSUMED

        public virtual void Dispose()                                      // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs) hoursHistory?.Dispose()
        {
            this.HistoryItemUpdated = null;
            this.NewHistoryItem = null;
        }
    }

    /// <summary>VERIFIED(ex:DoubleSMAIndicator.cs): new HistoricalDataCustom(this); hd[PriceType.Close] = value; hd.AddIndicator(ind); hd.Dispose().</summary>
    public class HistoricalDataCustom : HistoricalData
    {
        public HistoricalDataCustom(Indicator indicator) { }
        public double this[PriceType priceType] { get => double.NaN; set { } }
    }
}
