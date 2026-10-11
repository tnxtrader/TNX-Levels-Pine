// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;
using System.Collections.Generic;

namespace TradingPlatform.BusinessLayer
{
    /// <summary>Core singleton. VERIFIED(IL): Core.get_Instance() returns Core; TimeUtils is an INSTANCE property (callvirt instance).</summary>
    public class Core
    {
        public static Core Instance { get; } = new Core();                        // VERIFIED(IL)

        public TimeUtils TimeUtils { get; } = new TimeUtils();                     // VERIFIED(IL) instance
        public Loggers Loggers { get; } = new Loggers();                           // VERIFIED(ex:AccessSessionsInfoExample.cs) Core.Instance.Loggers.Log(...)
                                                                                   // NOTE: some official sources also write the STATIC form Core.Loggers.Log(ex)
                                                                                   // (AsynchronousIndicator.cs). A C# class cannot expose both a static and an
                                                                                   // instance member of the same name, and the IL proves Instance is typed Core,
                                                                                   // so only the instance form is stubbed.
        public VolumeAnalysisManager VolumeAnalysis { get; } = new VolumeAnalysisManager(); // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs) member; type name ASSUMED
        public IndicatorsManager Indicators { get; } = new IndicatorsManager();   // VERIFIED(ex:AccessIndicatorByName.cs) member; type name ASSUMED
        public IList<Symbol> Symbols { get; } = new List<Symbol>();               // VERIFIED(ex:HistoryExamples.cs) Core.Instance.Symbols.FirstOrDefault()
    }

    /// <summary>VERIFIED(IL): TimeUtils is a reference type; DateTimeUtcNow : DateTime; SelectedTimeZone : TimeZone.</summary>
    public class TimeUtils
    {
        public DateTime DateTimeUtcNow => DateTime.UtcNow;                         // VERIFIED(IL)
        public TimeZone SelectedTimeZone { get; set; } = new TimeZone(TimeZoneInfo.Utc); // VERIFIED(IL)
        public DateTime ConvertFromUTCToTimeZone(DateTime utc, TimeZone timeZone) => utc;   // ASSUMED (member name seen in research/dl Scripts)
        public DateTime ConvertFromTimeZoneToUTC(DateTime local, TimeZone timeZone) => local; // ASSUMED (member name seen in research/dl Scripts)
    }

    /// <summary>VERIFIED(IL): TimeZone.get_TimeZoneInfo() is invoked with 'call' on an unchecked receiver, i.e. a value type.</summary>
    public readonly struct TimeZone
    {
        public TimeZone(TimeZoneInfo info) { this.TimeZoneInfo = info; }
        public TimeZoneInfo TimeZoneInfo { get; }                                  // VERIFIED(IL)
        public string Id => this.TimeZoneInfo?.Id;                                 // ASSUMED
    }

    /// <summary>Logger. Overload shapes VERIFIED by usage in official sources: Log(ex), Log(ex, "msg"), Log(ex, "msg", level), Log("msg", level).</summary>
    public class Loggers
    {
        public void Log(string message, LoggingLevel level = LoggingLevel.Info) { }               // VERIFIED(ex:AccessSessionsInfoExample.cs, Scripts)
        public void Log(Exception ex, string message = null, LoggingLevel level = LoggingLevel.Error) { } // VERIFIED(Scripts: IndicatorCumulativeDelta.cs etc.)
    }

    public enum LoggingLevel
    {
        Info,       // ASSUMED
        Warning,    // ASSUMED
        Error,      // VERIFIED(Scripts)
        Trading,    // VERIFIED(Scripts)
        System,     // ASSUMED
    }

    /// <summary>Core.Instance.VolumeAnalysis. Type name ASSUMED; CalculateProfile VERIFIED(ex:AccessCustomVolumeAnalysisData.cs).</summary>
    public class VolumeAnalysisManager
    {
        public IVolumeAnalysisCalculationProgress CalculateProfile(HistoricalData historicalData) => null; // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs)
    }

    /// <summary>Core.Instance.Indicators. Type name ASSUMED; members VERIFIED(ex:AccessIndicatorByName.cs, DoubleSMAIndicator.cs).</summary>
    public class IndicatorsManager
    {
        public IEnumerable<IndicatorInfo> All { get; } = new List<IndicatorInfo>();          // VERIFIED(ex:AccessIndicatorByName.cs)
        public Indicator CreateIndicator(IndicatorInfo info) => null;                         // VERIFIED(ex:AccessIndicatorByName.cs)
        public BuiltInIndicators BuiltIn { get; } = new BuiltInIndicators();                  // VERIFIED(ex:DoubleSMAIndicator.cs)
    }

    /// <summary>VERIFIED(ex:AccessIndicatorByName.cs): info.Name.</summary>
    public class IndicatorInfo
    {
        public string Name { get; set; }
    }

    /// <summary>VERIFIED(ex:DoubleSMAIndicator.cs): Core.Instance.Indicators.BuiltIn.SMA(period, priceType).</summary>
    public class BuiltInIndicators
    {
        public Indicator SMA(int period, PriceType priceType = PriceType.Close) => null;      // VERIFIED(ex:DoubleSMAIndicator.cs)
        public Indicator EMA(int period, PriceType priceType = PriceType.Close) => null;      // ASSUMED
    }
}
