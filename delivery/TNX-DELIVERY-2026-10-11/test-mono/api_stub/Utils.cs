// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;

namespace TradingPlatform.BusinessLayer.Utils
{
    /// <summary>
    /// VERIFIED(IL): TradingPlatform.BusinessLayer.Utils.Interval`1 is a VALUE type (get_From/get_To invoked with 'call' on array elements);
    /// From/To are properties of type T. Constructor ASSUMED.
    /// </summary>
    public readonly struct Interval<T>
    {
        public Interval(T from, T to) { this.From = from; this.To = to; }
        public T From { get; }   // VERIFIED(IL)
        public T To { get; }     // VERIFIED(IL)
        public override string ToString() => this.From + " - " + this.To;
    }

    /// <summary>VERIFIED(ex:FitDrawingTestIndicator.cs): Const.DOUBLE_UNDEFINED compared with double.IsNaN. Namespace ASSUMED (that file imports both BusinessLayer and BusinessLayer.Utils).</summary>
    public static class Const
    {
        public const double DOUBLE_UNDEFINED = double.NaN;   // VERIFIED(ex) name; value ASSUMED (the example treats it as NaN)
        public const int INT_UNDEFINED = int.MinValue;       // ASSUMED
    }
}
