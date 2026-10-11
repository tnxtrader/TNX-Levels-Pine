// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;
using System.Collections.Generic;
using System.Drawing;
using TradingPlatform.BusinessLayer.Chart;

namespace TradingPlatform.BusinessLayer
{
    /// <summary>Base of Indicator / Strategy. VERIFIED(IL): ExecutionEntity.set_Name(string), set_Description(string).</summary>
    public abstract class ExecutionEntity
    {
        public string Name { get; set; }          // VERIFIED(IL)
        public string Description { get; set; }   // VERIFIED(IL)
    }

    /// <summary>Implemented by TNXCVDPro. VERIFIED(src) + VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs).</summary>
    public interface IVolumeAnalysisIndicator
    {
        bool IsRequirePriceLevelsCalculation { get; }   // VERIFIED(src, ex)
        void VolumeAnalysisData_Loaded();               // VERIFIED(src, ex)
    }

    public enum IndicatorUpdateType
    {
        OnTick,      // ASSUMED
        OnBarClose,  // VERIFIED(ex:AsynchronousIndicator.cs)
    }

    public enum UpdateReason
    {
        HistoricalBar, // VERIFIED(src)
        NewBar,        // VERIFIED(src)
        NewTick,       // VERIFIED(Scripts: IndicatorDeltaRotation.cs)
    }

    /// <summary>VERIFIED(IL): UpdateArgs.get_Reason() : UpdateReason (reference type, callvirt).</summary>
    public class UpdateArgs : EventArgs
    {
        public UpdateReason Reason { get; set; }   // VERIFIED(IL)
    }

    /// <summary>VERIFIED(IL): Graphics (System.Drawing.Common), Rectangle (System.Drawing.Primitives), WindowIndex : int.</summary>
    public class PaintChartEventArgs : EventArgs
    {
        public Graphics Graphics { get; set; }       // VERIFIED(IL)
        public Rectangle Rectangle { get; set; }     // VERIFIED(IL)
        public int WindowIndex { get; set; }         // VERIFIED(IL)
        public Point MousePosition { get; set; }     // VERIFIED(ex:IndicatorChartMouseEvents.cs)
    }

    /// <summary>
    /// Indicator base class. Member shapes VERIFIED(IL) unless marked otherwise:
    ///   .ctor(), get_Count:int, get_CurrentChart:IChart, get_HistoricalData:HistoricalData, get_Symbol:Symbol,
    ///   get/set_Settings:IList&lt;SettingItem&gt;, set_SeparateWindow(bool),
    ///   AddLineSeries(string, Color, int, LineStyle):LineSeries, AddLineLevel(double, string, Color, int, LineStyle):LineLevel,
    ///   GetPrice(PriceType, int):double, GetVolumeAnalysisData(int):VolumeAnalysisData, Ticks(int):double, Time(int):DateTime,
    ///   OnPaintChart(PaintChartEventArgs) public virtual, OnSettingsUpdated() protected virtual,
    ///   OnInit/OnClear/OnUpdate(UpdateArgs) protected virtual, OnTryGetMinMax(int,int,out double,out double) protected virtual bool.
    /// </summary>
    public abstract class Indicator : ExecutionEntity, IDisposable
    {
        private readonly List<LineSeries> linesSeries = new List<LineSeries>();
        private IList<SettingItem> settings = new List<SettingItem>();

        public Indicator() { }                                                     // VERIFIED(IL) Indicator::.ctor()

        public string ShortName { get; set; }                                      // VERIFIED(ex:SimpleMovingAverage.cs)
        public bool SeparateWindow { get; set; }                                   // VERIFIED(IL)
        public IndicatorUpdateType UpdateType { get; set; }                        // VERIFIED(ex:AsynchronousIndicator.cs)

        public IChart CurrentChart { get; protected set; }                         // VERIFIED(IL)
        public Symbol Symbol { get; protected set; }                               // VERIFIED(IL)
        public HistoricalData HistoricalData { get; protected set; }               // VERIFIED(IL)
        public int Count => TnxFeed.Count;                       // VERIFIED(IL) get_Count : int

        public IList<LineSeries> LinesSeries => this.linesSeries;                  // VERIFIED(Scripts: LinesSeries[0].SetMarker(...))

        public virtual IList<SettingItem> Settings                                 // VERIFIED(IL) virtual get/set on Indicator
        {
            get => this.settings;
            set => this.settings = value;
        }

        // ---- lines ----
        public LineSeries AddLineSeries(string lineName = "Line", Color lineColor = default(Color), int lineWidth = 1, LineStyle lineStyle = LineStyle.Solid) // VERIFIED(IL) 4-arg form; defaults VERIFIED(src: 2/3-arg calls; ex:AsynchronousIndicator.cs: no-arg call)
        {
            var line = new LineSeries { Name = lineName, Color = lineColor, Width = lineWidth, Style = lineStyle };
            this.linesSeries.Add(line);
            return line;
        }

        public LineLevel AddLineLevel(double value, string lineName = "Level", Color color = default(Color), int width = 1, LineStyle style = LineStyle.Solid) // VERIFIED(IL) 5-arg form
            => new LineLevel { Level = value, Name = lineName, Color = color, Width = width, Style = style };

        public void SetValue(double value, int lineIndex = 0, int offset = 0) { }  // VERIFIED(ex:AsynchronousIndicator.cs 'offset:' named; DoubleSMAIndicator.cs (v, lineIndex); Scripts (v, line, offset))
        public double GetValue(int offset = 0, int lineIndex = 0) => double.NaN;   // VERIFIED(ex:AccessIndicatorByName.cs GetValue(0,0); DoubleSMAIndicator.cs GetValue())

        // ---- price access (VERIFIED(IL): GetPrice(PriceType,int), Ticks(int), Time(int) - exactly these arities) ----
        public double GetPrice(PriceType priceType, int offset = 0) => TnxFeed.Price(priceType, offset); // VERIFIED(IL)
        public DateTime Time(int offset = 0) => TnxFeed.Time(offset);                 // VERIFIED(IL) Time(int32) : DateTime
        public double Ticks(int offset = 0) => TnxFeed.Ticks(offset);                         // VERIFIED(IL) Ticks(int32) : float64
        public double Open(int offset = 0) => double.NaN;                          // VERIFIED(Scripts/ex: this.Open(offset))
        public double High(int offset = 0) => double.NaN;                          // VERIFIED(Scripts)
        public double Low(int offset = 0) => double.NaN;                           // VERIFIED(Scripts)
        public double Close(int offset = 0) => double.NaN;                         // VERIFIED(ex:FitDrawingTestIndicator.cs)
        public double Volume(int offset = 0) => double.NaN;                        // VERIFIED(Scripts: IndicatorAbnormalVolume.cs)
        public double Median(int offset = 0) => double.NaN;                        // VERIFIED(ex:AsynchronousIndicator.cs)
        public double Typical(int offset = 0) => double.NaN;                       // ASSUMED (by analogy)
        public double Weighted(int offset = 0) => double.NaN;                      // ASSUMED (by analogy)
        public VolumeAnalysisData GetVolumeAnalysisData(int offset = 0) => TnxFeed.Va(offset);   // VERIFIED(IL) GetVolumeAnalysisData(int32)

        // ---- inner indicators ----
        public void AddIndicator(Indicator indicator) { }                          // VERIFIED(ex:AccessIndicatorByName.cs)
        public void RemoveIndicator(Indicator indicator) { }                       // VERIFIED(ex:UsingInnerIndicatorSettings.cs)
        public void Refresh() { }                                                  // VERIFIED(ex:UsingInnerIndicatorSettings.cs)

        // ---- lifecycle (VERIFIED(IL): family/public virtual as declared here) ----
        protected virtual void OnInit() { }                                        // VERIFIED(IL)
        protected virtual void OnUpdate(UpdateArgs args) { }                       // VERIFIED(IL)
        protected virtual void OnClear() { }                                       // VERIFIED(IL)
        protected virtual void OnSettingsUpdated() { }                             // VERIFIED(IL)
        public virtual void OnPaintChart(PaintChartEventArgs args) { }             // VERIFIED(IL) public virtual
        protected virtual bool OnTryGetMinMax(int fromOffset, int toOffset, out double min, out double max) // VERIFIED(IL) + ex:FitDrawingTestIndicator.cs
        {
            min = double.NaN; max = double.NaN; return false;
        }
        public virtual void Dispose() { }                                          // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs, IndicatorChartMouseEvents.cs) public override void Dispose()
    }

    /// <summary>Minimal Strategy base so that SettingsRelationsExample.cs compiles. ASSUMED shape except Settings (VERIFIED(ex:SettingsRelationsExample.cs)).</summary>
    public abstract class Strategy : ExecutionEntity
    {
        private IList<SettingItem> settings = new List<SettingItem>();
        protected Strategy() { }
        public virtual IList<SettingItem> Settings { get => this.settings; set => this.settings = value; } // VERIFIED(ex:SettingsRelationsExample.cs)
        protected virtual void OnCreated() { }   // ASSUMED
        protected virtual void OnRun() { }       // ASSUMED
        protected virtual void OnStop() { }      // ASSUMED
        protected virtual void OnRemove() { }    // ASSUMED
    }
}

namespace TradingPlatform.BusinessLayer
{
    // TEST ONLY: the data the test indicator reads (bar index 0 = oldest; offset 0 = newest of the Count bars visible)
    public static class TnxFeed
    {
        public static int Count;
        public static System.Collections.Generic.List<double> O = new(), H = new(), L = new(), C = new(), V = new(), Tk = new();
        public static System.Collections.Generic.List<DateTime> T = new();
        public static System.Collections.Generic.List<VolumeAnalysisData> VA = new();
        public static bool VaOn = true;
        static int Idx(int offset) => Count - 1 - offset;
        public static double Price(PriceType p, int offset)
        {
            int i = Idx(offset); if (i < 0 || i >= O.Count) return double.NaN;
            switch (p) { case PriceType.Open: return O[i]; case PriceType.High: return H[i]; case PriceType.Low: return L[i]; case PriceType.Close: return C[i]; case PriceType.Volume: return V[i]; case PriceType.Ticks: return Tk[i]; default: return C[i]; }
        }
        public static DateTime Time(int offset) { int i = Idx(offset); return (i < 0 || i >= T.Count) ? default(DateTime) : T[i]; }
        public static double Ticks(int offset) { int i = Idx(offset); return (i < 0 || i >= Tk.Count) ? double.NaN : Tk[i]; }
        public static VolumeAnalysisData Va(int offset) { int i = Idx(offset); return (!VaOn || i < 0 || i >= VA.Count) ? null : VA[i]; }
    }
}
