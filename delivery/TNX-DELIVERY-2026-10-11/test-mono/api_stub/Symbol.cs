// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;
using System.Collections.Generic;
using TradingPlatform.BusinessLayer.Utils;

namespace TradingPlatform.BusinessLayer
{
    /// <summary>VERIFIED(IL): ISession.get_IsPrimary() : bool. Name/OpenTime/CloseTime/ContainsDate VERIFIED(ex:AccessSessionsInfoExample.cs).</summary>
    public interface ISession
    {
        bool IsPrimary { get; }                 // VERIFIED(IL)
        string Name { get; }                    // VERIFIED(ex:AccessSessionsInfoExample.cs)
        TimeSpan OpenTime { get; }              // VERIFIED(ex:AccessSessionsInfoExample.cs) member; type ASSUMED (only printed)
        TimeSpan CloseTime { get; }             // VERIFIED(ex:AccessSessionsInfoExample.cs) member; type ASSUMED (only printed)
        bool ContainsDate(DateTime dateTime);   // VERIFIED(ex:AccessSessionsInfoExample.cs)
    }

    /// <summary>VERIFIED(IL): the first parameter of SessionsExtensions.EnumerateSessionTimeFrames is ISessionsContainer.</summary>
    public interface ISessionsContainer
    {
        string Name { get; }                    // ASSUMED
        ISession[] ActiveSessions { get; }      // VERIFIED(ex:AccessSessionsInfoExample.cs) on FindSessionsContainer() result; array type VERIFIED(IL) on SessionsContainer
    }

    /// <summary>VERIFIED(IL): Symbol.get_CurrentSessionsInfo() : SessionsContainer; SessionsContainer.get_ActiveSessions() : ISession[] (reference type).</summary>
    public class SessionsContainer : ISessionsContainer
    {
        public string Name { get; set; }                                         // ASSUMED
        public ISession[] ActiveSessions { get; set; } = new ISession[0];        // VERIFIED(IL) ISession[]
    }

    /// <summary>VERIFIED(IL): static EnumerateSessionTimeFrames(ISessionsContainer, DateTime, DateTime) : IEnumerable&lt;Interval&lt;DateTime&gt;&gt; (used as an extension method in src).</summary>
    public static class SessionsExtensions
    {
        public static IEnumerable<Interval<DateTime>> EnumerateSessionTimeFrames(this ISessionsContainer container, DateTime from, DateTime to) // VERIFIED(IL)
        {
            yield break;
        }
    }

    /// <summary>
    /// VERIFIED(IL): get_TickSize() : double, get_CurrentSessionsInfo() : SessionsContainer.
    /// GetHistory overloads VERIFIED(ex:HistoryExamples.cs, AccessCustomVolumeAnalysisData.cs, Scripts); HistoryType, FormatPrice, FormatQuantity, FindSessionsContainer VERIFIED(ex).
    /// </summary>
    public class Symbol
    {
        public string Name { get; set; }                                      // ASSUMED (common)
        public string Id { get; set; }                                        // ASSUMED
        public double TickSize { get; set; }                                  // VERIFIED(IL) float64
        public double LotSize { get; set; }                                   // VERIFIED(Scripts) member; type ASSUMED
        public double LotStep { get; set; }                                   // VERIFIED(Scripts) member; type ASSUMED
        public SessionsContainer CurrentSessionsInfo { get; set; }            // VERIFIED(IL)
        public HistoryType HistoryType { get; set; }                          // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs)

        public string FormatPrice(double price) => price.ToString(System.Globalization.CultureInfo.InvariantCulture);       // VERIFIED(ex:IndicatorChartMouseEvents.cs)
        public string FormatQuantity(double quantity) => quantity.ToString(System.Globalization.CultureInfo.InvariantCulture); // VERIFIED(ex:AccessCustomVolumeAnalysisData.cs)

        public ISessionsContainer FindSessionsContainer() => this.CurrentSessionsInfo;   // VERIFIED(ex:AccessSessionsInfoExample.cs)

        public HistoricalData GetHistory(Period period, DateTime fromTime, DateTime toTime = default(DateTime)) => new HistoricalData { Symbol = this, Aggregation = new HistoryAggregationTime(period, this.HistoryType), FromTime = fromTime, ToTime = toTime };             // VERIFIED(ex:HistoryExamples.cs) GetHistory(Period.MIN1, DateTime)
        public HistoricalData GetHistory(Period period, HistoryType historyType, DateTime fromTime, DateTime toTime = default(DateTime)) => new HistoricalData { Symbol = this, Aggregation = new HistoryAggregationTime(period, historyType), FromTime = fromTime, ToTime = toTime }; // VERIFIED(ex:HistoryExamples.cs) 3-arg; Scripts 4-arg (from, to)
        public HistoricalData GetHistory(HistoryAggregation aggregation, DateTime fromTime, DateTime toTime = default(DateTime)) => new HistoricalData { Symbol = this, Aggregation = aggregation, FromTime = fromTime, ToTime = toTime };                                            // VERIFIED(ex:HistoryExamples.cs) GetHistory(new HistoryAggregationRenko(...), DateTime)
        public HistoricalData GetHistory(HistoryRequestParameters parameters) => new HistoricalData { Symbol = this, Aggregation = parameters?.Aggregation, FromTime = parameters?.FromTime ?? default(DateTime), ToTime = parameters?.ToTime ?? default(DateTime) };       // VERIFIED(ex:HistoryExamples.cs)
    }
}
