// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System.Collections.Generic;

namespace TradingPlatform.BusinessLayer
{
    public enum VolumeAnalysisCalculationState
    {
        NotStarted,  // ASSUMED
        Processing,  // VERIFIED(Scripts)
        Finished,    // VERIFIED(src, ex:AccessCustomVolumeAnalysisData.cs)
        Aborted,     // ASSUMED
    }

    /// <summary>VERIFIED(IL): get_State() : VolumeAnalysisCalculationState (interface). ProgressPercent / AbortLoading VERIFIED(ex:AccessCustomVolumeAnalysisData.cs).</summary>
    public interface IVolumeAnalysisCalculationProgress
    {
        VolumeAnalysisCalculationState State { get; }   // VERIFIED(IL)
        int ProgressPercent { get; }                    // VERIFIED(ex) member; int ASSUMED (only used in string interpolation)
        void AbortLoading();                            // VERIFIED(ex)
    }

    /// <summary>Field selector for VolumeAnalysisItem.GetValue. All names VERIFIED(Scripts + ex:DrawValueAreaForEachBarIndicator.cs).</summary>
    public enum VolumeAnalysisField
    {
        Volume,                   // VERIFIED(ex)
        Trades,                   // VERIFIED(Scripts)
        BuyVolume,                // VERIFIED(Scripts)
        SellVolume,               // VERIFIED(Scripts)
        BuyTrades,                // VERIFIED(Scripts)
        SellTrades,               // VERIFIED(Scripts)
        Delta,                    // VERIFIED(Scripts)
        DeltaPercent,             // VERIFIED(Scripts)
        CumulativeDelta,          // VERIFIED(Scripts)
        AverageSize,              // VERIFIED(Scripts)
        AverageBuySize,           // VERIFIED(Scripts)
        AverageSellSize,          // VERIFIED(Scripts)
        BuyVolumePercent,         // VERIFIED(Scripts)
        SellVolumePercent,        // VERIFIED(Scripts)
        MaxOneTradeVolume,        // VERIFIED(Scripts)
        MaxOneTradeVolumePercent, // VERIFIED(Scripts)
        OpenInterest,             // VERIFIED(Scripts)
    }

    /// <summary>VERIFIED(IL): Volume, BuyVolume, SellVolume, Delta, MaxDelta, MinDelta : double (reference type, callvirt). Other members VERIFIED(Scripts) by name; double ASSUMED for the trade counts.</summary>
    public class VolumeAnalysisItem
    {
        public double Volume { get; set; }            // VERIFIED(IL)
        public double BuyVolume { get; set; }         // VERIFIED(IL)
        public double SellVolume { get; set; }        // VERIFIED(IL)
        public double Delta { get; set; }             // VERIFIED(IL)
        public double MaxDelta { get; set; }          // VERIFIED(IL)
        public double MinDelta { get; set; }          // VERIFIED(IL)
        public double Trades { get; set; }            // name VERIFIED(Scripts); type ASSUMED
        public double BuyTrades { get; set; }         // name VERIFIED(Scripts); type ASSUMED
        public double SellTrades { get; set; }        // name VERIFIED(Scripts); type ASSUMED
        public double AverageSize { get; set; }       // name VERIFIED(Scripts); type ASSUMED
        public double AverageBuySize { get; set; }    // name VERIFIED(Scripts); type ASSUMED
        public double AverageSellSize { get; set; }   // name VERIFIED(Scripts); type ASSUMED
        public double MaxOneTradeVolume { get; set; } // name VERIFIED(Scripts); type ASSUMED
        public double OpenInterest { get; set; }      // name VERIFIED(Scripts); type ASSUMED
        public double CumulativeDelta { get; set; }   // ASSUMED

        public double GetValue(VolumeAnalysisField field)   // VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs)
        {
            switch (field)
            {
                case VolumeAnalysisField.Volume: return this.Volume;
                case VolumeAnalysisField.Trades: return this.Trades;
                case VolumeAnalysisField.BuyVolume: return this.BuyVolume;
                case VolumeAnalysisField.SellVolume: return this.SellVolume;
                case VolumeAnalysisField.BuyTrades: return this.BuyTrades;
                case VolumeAnalysisField.SellTrades: return this.SellTrades;
                case VolumeAnalysisField.Delta: return this.Delta;
                case VolumeAnalysisField.DeltaPercent: return this.Volume != 0 ? this.Delta / this.Volume * 100.0 : 0.0;
                case VolumeAnalysisField.CumulativeDelta: return this.CumulativeDelta;
                case VolumeAnalysisField.AverageSize: return this.AverageSize;
                case VolumeAnalysisField.AverageBuySize: return this.AverageBuySize;
                case VolumeAnalysisField.AverageSellSize: return this.AverageSellSize;
                case VolumeAnalysisField.BuyVolumePercent: return this.Volume != 0 ? this.BuyVolume / this.Volume * 100.0 : 0.0;
                case VolumeAnalysisField.SellVolumePercent: return this.Volume != 0 ? this.SellVolume / this.Volume * 100.0 : 0.0;
                case VolumeAnalysisField.MaxOneTradeVolume: return this.MaxOneTradeVolume;
                case VolumeAnalysisField.MaxOneTradeVolumePercent: return this.Volume != 0 ? this.MaxOneTradeVolume / this.Volume * 100.0 : 0.0;
                case VolumeAnalysisField.OpenInterest: return this.OpenInterest;
                default: return double.NaN;
            }
        }
    }

    /// <summary>VERIFIED(IL): get_Total() : VolumeAnalysisItem. PriceLevels : Dictionary&lt;double, VolumeAnalysisItem&gt; VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs).</summary>
    public class VolumeAnalysisData
    {
        public VolumeAnalysisItem Total { get; set; }                                                        // VERIFIED(IL)
        public Dictionary<double, VolumeAnalysisItem> PriceLevels { get; set; } = new Dictionary<double, VolumeAnalysisItem>(); // VERIFIED(ex)
    }
}
