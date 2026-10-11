// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System.Drawing;

namespace TradingPlatform.BusinessLayer
{
    public enum LineStyle
    {
        Solid,      // VERIFIED(ex: all examples)
        Dash,       // VERIFIED(src)
        Dot,        // VERIFIED(Scripts)
        DashDot,    // VERIFIED(Scripts)
        Histogramm, // VERIFIED(Scripts) (sic - the API spells it with two m)
        Points,     // VERIFIED(Scripts)
        Columns,    // VERIFIED(Scripts)
        StepLine,   // VERIFIED(Scripts)
    }

    public enum IndicatorLineMarkerIconType
    {
        None,        // VERIFIED(Scripts)
        UpArrow,     // VERIFIED(Scripts)
        DownArrow,   // VERIFIED(Scripts)
        FillCircle,  // VERIFIED(Scripts)
        Flag,        // VERIFIED(Scripts)
    }

    /// <summary>VERIFIED(IL): .ctor(Color, IndicatorLineMarkerIconType, IndicatorLineMarkerIconType); src calls it with Color only, so the icons are optional.</summary>
    public class IndicatorLineMarker
    {
        public IndicatorLineMarker(Color color, IndicatorLineMarkerIconType upperIcon = IndicatorLineMarkerIconType.None, IndicatorLineMarkerIconType bottomIcon = IndicatorLineMarkerIconType.None) // VERIFIED(IL) + Scripts 'upperIcon:'/'bottomIcon:' named args
        {
            this.Color = color; this.UpperIcon = upperIcon; this.BottomIcon = bottomIcon;
        }
        public Color Color { get; set; }                               // ASSUMED (property names)
        public IndicatorLineMarkerIconType UpperIcon { get; set; }     // ASSUMED
        public IndicatorLineMarkerIconType BottomIcon { get; set; }    // ASSUMED
    }

    /// <summary>Common base of LineSeries and LineLevel. VERIFIED(IL): Line.set_Color(Color), set_Width(int), set_Visible(bool) live on Line.</summary>
    public abstract class Line
    {
        public string Name { get; set; }      // ASSUMED
        public Color Color { get; set; }      // VERIFIED(IL)
        public int Width { get; set; }        // VERIFIED(IL) int32
        public bool Visible { get; set; }     // VERIFIED(IL)
        public LineStyle Style { get; set; }  // VERIFIED(Scripts: LinesSeries[1].Style = LineStyle.Solid)
    }

    /// <summary>VERIFIED(IL): LineSeries.SetValue(double, int, SeekOriginHistory), SetMarker(int, IndicatorLineMarker), set_ShowLineMarker(bool).</summary>
    public class LineSeries : Line
    {
        public bool ShowLineMarker { get; set; }                                                                  // VERIFIED(IL)
        public void SetValue(double value, int offset = 0, SeekOriginHistory origin = SeekOriginHistory.End) { } // VERIFIED(IL) (src passes 2 args -> origin optional)
        public double GetValue(int offset = 0, SeekOriginHistory origin = SeekOriginHistory.End) => double.NaN;  // VERIFIED(Scripts: LinesSeries[1].GetValue(offset))
        public void SetMarker(int offset, IndicatorLineMarker marker) { }                                         // VERIFIED(IL)
        public void SetMarker(int offset, Color color) { }                                                        // VERIFIED(Scripts: LinesSeries[0].SetMarker(offset, Color.Red))
        public void RemoveMarker(int offset) { }                                                                  // VERIFIED(Scripts)
    }

    /// <summary>Returned by Indicator.AddLineLevel. VERIFIED(IL) type; members beyond Line ASSUMED.</summary>
    public class LineLevel : Line
    {
        public double Level { get; set; }     // ASSUMED
    }
}
