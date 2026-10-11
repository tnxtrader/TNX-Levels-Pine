// COMPILE-ONLY STUB of TradingPlatform.BusinessLayer (Quantower API) - NOT the real API. See AssemblyInfo.cs for the legend.
using System;
using System.Drawing;
using TradingPlatform.BusinessLayer.Native;

namespace TradingPlatform.BusinessLayer.Native
{
    public enum NativeMouseButtons
    {
        None,      // ASSUMED
        Left,      // VERIFIED(src, ex:IndicatorChartMouseEvents.cs)
        Right,     // VERIFIED(src)
        Middle,    // ASSUMED
        XButton1,  // ASSUMED
        XButton2,  // ASSUMED
    }

    /// <summary>VERIFIED(IL): get_Button() : NativeMouseButtons, get_Clicks/get_Delta/get_X/get_Y : int32, set_Handled(bool). Location VERIFIED(ex:IndicatorChartMouseEvents.cs).</summary>
    public class NativeMouseEventArgs : EventArgs
    {
        public NativeMouseButtons Button { get; set; }   // VERIFIED(IL)
        public int Clicks { get; set; }                  // VERIFIED(IL)
        public int Delta { get; set; }                   // VERIFIED(IL)
        public int X { get; set; }                       // VERIFIED(IL)
        public int Y { get; set; }                       // VERIFIED(IL)
        public bool Handled { get; set; }                // VERIFIED(IL)
        public Point Location => new Point(this.X, this.Y);   // VERIFIED(ex:IndicatorChartMouseEvents.cs) e.Location (Point)
    }
}

namespace TradingPlatform.BusinessLayer.Chart
{
    /// <summary>VERIFIED(IL): derives from NativeMouseEventArgs (Button/X/Y resolved on the base); get_Window() : IChartWindow; set_NeedMouseCapture(bool); set_NeedRedraw(bool).</summary>
    public class ChartMouseNativeEventArgs : NativeMouseEventArgs
    {
        public IChartWindow Window { get; set; }         // VERIFIED(IL)
        public bool NeedMouseCapture { get; set; }       // VERIFIED(IL)
        public bool NeedRedraw { get; set; }             // VERIFIED(IL)
    }

    /// <summary>VERIFIED(IL): GetBarIndex(DateTime):double, GetChartX(DateTime):double, GetChartY(double):double. GetTime/GetPrice VERIFIED(ex:DrawOnBars.cs, IndicatorChartMouseEvents.cs).</summary>
    public interface IChartWindowCoordinatesConverter
    {
        double GetBarIndex(DateTime time);   // VERIFIED(IL)
        double GetChartX(DateTime time);     // VERIFIED(IL)
        double GetChartY(double price);      // VERIFIED(IL)
        DateTime GetTime(double x);          // VERIFIED(ex:DrawOnBars.cs) called with an int (ClientRectangle.Left)
        double GetPrice(double y);           // VERIFIED(ex:IndicatorChartMouseEvents.cs) called with an int (Point.Y)
    }

    /// <summary>VERIFIED(IL): get_CoordinatesConverter() : IChartWindowCoordinatesConverter, get_WindowNumber() : int32. ClientRectangle/YScaleFactor VERIFIED(ex:DrawValueAreaForEachBarIndicator.cs).</summary>
    public interface IChartWindow
    {
        IChartWindowCoordinatesConverter CoordinatesConverter { get; }   // VERIFIED(IL)
        int WindowNumber { get; }                                        // VERIFIED(IL)
        Rectangle ClientRectangle { get; }                               // VERIFIED(ex) used with gr.SetClip(Rectangle) and .Left/.Right/.Contains(Point)
        double YScaleFactor { get; }                                     // VERIFIED(ex) used in a double expression
    }

    /// <summary>
    /// VERIFIED(IL): RedrawBuffer(), get_BarsWidth() : int32, get_CurrentTimeZone() : TimeZone, get_Windows() : IChartWindow[],
    /// events MouseDown/MouseMove/MouseUp/MouseWheel : EventHandler&lt;ChartMouseNativeEventArgs&gt;. MainWindow VERIFIED(ex:DrawOnBars.cs).
    /// </summary>
    public interface IChart
    {
        void RedrawBuffer();                       // VERIFIED(IL)
        int BarsWidth { get; }                     // VERIFIED(IL)
        TimeZone CurrentTimeZone { get; }          // VERIFIED(IL)
        IChartWindow[] Windows { get; }            // VERIFIED(IL)
        IChartWindow MainWindow { get; }           // VERIFIED(ex:DrawOnBars.cs)
        double TickSize { get; }                   // VERIFIED(Scripts: CurrentChart.TickSize) member; type ASSUMED
        int RightOffset { get; }                   // VERIFIED(Scripts: CurrentChart.RightOffset) member; type ASSUMED

        event EventHandler<ChartMouseNativeEventArgs> MouseDown;    // VERIFIED(IL)
        event EventHandler<ChartMouseNativeEventArgs> MouseMove;    // VERIFIED(IL)
        event EventHandler<ChartMouseNativeEventArgs> MouseUp;      // VERIFIED(IL)
        event EventHandler<ChartMouseNativeEventArgs> MouseWheel;   // VERIFIED(IL)
    }
}
