using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Threading;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Chart;

namespace TNXIndicators;

// TNX IMBALANCE STRENGTH 1.0 (2026-10-11)
//
// What it measures. "Imbalance" on a footprint is the diagonal bid/ask rule (ask at a price against the bid one tick below,
// 300 % by default, three stacked levels = a stack). That rule says WHERE the aggression traded, not whether it mattered.
// This indicator reads every finished bar with eight dials that only need the bar's own data (buy / sell volume, delta,
// its running extremes, OHLC, the time it took, and the footprint levels), ranks each against the last N bars, and folds
// them into one signed number, the Imbalance Strength IS in -100 .. +100 (sign = who pushes), with a five-state verdict:
//   REAL      one-sided, busy, effort, not absorbed: a push that mattered
//   THIN      little depth taken (cheap move, nobody there)
//   BATTLE    two-sided: no one-sidedness, or the push / the range was reversed inside the bar, or stacks on both sides
//   ABSORBED  the pushing side lost the bar (TRAPPED) or k bars of net flow went nowhere (WALL)
//   FADING    same side as before, strength falling
// and its acceleration (EMA of the change of IS, in multiples of its typical change): THRUST from 2x, STALL from -2x.
//
// The dials (per finished bar; the forming bar gets the same on its partial numbers and is drawn hollow):
//   participation  pi    = rank of volume per second            (how busy)
//   effort         e     = rank of |delta|                      (how hard)
//   one-sidedness  omega = rank of |delta| / (buy + sell)       (Quantower's Delta %)
//   depth taken    zeta  = rank of volume (range bars) or volume per tick moved (other charts)
//   give-back      g     = (largest delta excursion - |delta|) / excursion;  wick w = range first traded the other way
//   persistence    psi   = EWMA of consecutive flow-sign agreement
//   footprint      phi   = stacked diagonal imbalances with the flow (+), against it (-), their share of the bar's volume
//   impulse        J4    = 4-bar net delta against the 4-bar net move (absorption: big flow, no move)
//   confirmation   kappa = the next bar's direction (known one bar later; shown, never used in the bar's own number)
//   IS = sign x 100 x G x (1 - 0.7 Fake),  G = (pi . omega . e . sqrt(phi) . sqrt(psi))^(1/4), floors 0.05, Fake = max of the
//        five failure modes (trapped, wall, given back, reversed inside, thin)
//
// Everything is computed once per bar on the data thread (O(lookback) per bar) and only READ by the picture. The footprint
// is read through IsRequirePriceLevelsCalculation = true (the official way), sorted by price, snapped to the tick grid.
public class TNXImbalanceStrength : Indicator, IVolumeAnalysisIndicator
{
	public enum Look
	{
		Bars,
		Area,
		Needles,
		Lollipop,
		Capsules
	}

	public enum Theme
	{
		ProDark,
		Neon,
		Classic,
		Ocean,
		Monochrome,
		Ember,
		Ice,
		Aurora,
		Terminal,
		Sunset,
		HighContrast,
		Custom,
		AtasX,
		DeepCharts,
		Bookmap,
		TradingView,
		Cyberpunk,
		Dracula,
		Nordic,
		MidnightGold,
		Solarized,
		Matrix
	}

	public enum Corner
	{
		TopLeft,
		TopRight,
		BottomLeft,
		BottomRight,
		Hidden
	}

	public enum CardRowsMode
	{
		Full,
		Compact
	}

	public enum State
	{
		NoData,
		Thin,
		Battle,
		Absorbed,
		Fading,
		Real
	}

	private enum Agg
	{
		Other,
		Range,
		Tick,
		Time
	}

	private struct Bar
	{
		public bool Has;        // computed at least once
		public bool Real;       // volume analysis present and some volume
		public bool Closed;     // a finished bar (its dials are final)
		public double Delta;
		public double Buy;
		public double Sell;
		public double Vol;
		public double MaxD;     // running extremes of the bar's delta (sentinels mapped to 0)
		public double MinD;
		public double O;
		public double H;
		public double L;
		public double C;
		public long Ticks;      // open time (UTC ticks)
		public double Dur;      // seconds the bar took, floored at 0.1
		public double F;        // forming-bar progress 0..1
		public int Side;        // +1 buyers, -1 sellers (the flow side; the bar's direction when the delta is 0)
		public int Dir;         // bar direction +1 / -1 / 0
		public bool Agree;      // flow side == bar direction
		public double Vps;      // volume per second
		public double Omega;    // |delta| / (buy + sell), 0..1
		public double Zeta;     // depth taken, raw: volume (range) or volume per tick moved (other)
		public double G;        // give-back 0..1
		public double W;        // open-side wick 0..1
		public double Psi;      // persistence EWMA -1..1
		public int Run;         // same-sign run length
		public double J4;       // 4-bar net delta
		public double Move4;    // |C - C[-4]| in ticks
		// ranks 0..1 (-1 while fewer than MinRankBars real bars)
		public double RPi;
		public double REff;
		public double ROmega;
		public double RZeta;
		public double RJ4;
		// footprint
		public bool FpOk;       // levels were read
		public int KWith;       // stacked imbalance levels with the flow
		public int KAgainst;    // against the flow
		public double ShWith;   // share of the bar's volume inside the with-side imbalanced levels
		public double ShAgainst;
		public double Phi;      // 0..1 (0.5 = no information)
		public bool UaRejected; // unfinished auction at the rejected extreme (both sides traded there)
		public int Levels;      // number of price levels in the bar
		public double MaxLot;   // largest single trade in the bar (0 when unknown)
		// result
		public double Fake;     // 0..1
		public double Gscore;   // 0..1
		public double IS;       // -100..100
		public State St;
		public bool Trapped;    // ABSORBED sub-label
		public bool Wall;
		public double AccE;     // EMA of the side-framed change of IS
		public double AccN;     // in multiples of its typical change
		public bool Flip;       // the side changed from the previous bar
		public int Kappa;       // next bar's response: +1 confirmed, -1 rejected, 0 unknown
	}

	// ---------------- settings ----------------

	[InputParameter("Look", 0, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Bars (height = strength, brightness = state)",
		Look.Bars,
		"Polarized area (smooth)",
		Look.Area,
		"Needles + dots",
		Look.Needles,
		"Lollipops (ball = acceleration)",
		Look.Lollipop,
		"Capsules (fill = strength, marker = state)",
		Look.Capsules
	})]
	public Look Style;

	[InputParameter("Theme", 1, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Pro dark", Theme.ProDark, "Cyberpunk (neon gold/pink)", Theme.Cyberpunk, "Dracula (emerald/coral/purple)", Theme.Dracula,
		"Nordic (arctic teal/crimson)", Theme.Nordic, "Midnight Gold", Theme.MidnightGold, "Solarized", Theme.Solarized,
		"Matrix (digital green)", Theme.Matrix, "ATAS X", Theme.AtasX, "DeepCharts", Theme.DeepCharts, "Bookmap", Theme.Bookmap,
		"TradingView", Theme.TradingView, "Neon", Theme.Neon, "Classic", Theme.Classic, "Ocean", Theme.Ocean, "Monochrome", Theme.Monochrome,
		"Ember", Theme.Ember, "Ice", Theme.Ice, "Aurora", Theme.Aurora, "Terminal", Theme.Terminal, "Sunset", Theme.Sunset,
		"High contrast (colour-blind safe)", Theme.HighContrast, "Custom (colours below)", Theme.Custom
	})]
	public Theme Skin;

	[InputParameter("Custom buyers colour", 2, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color CustomBull = Color.FromArgb(38, 198, 145);

	[InputParameter("Custom sellers colour", 3, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color CustomBear = Color.FromArgb(235, 64, 92);

	[InputParameter("Lookback (bars) for ranks, typical sizes, hit rate", 4, 20.0, 2000.0, 5.0, 0, null)]
	public int Lookback = 60;

	[InputParameter("Footprint: read the price levels (stacked imbalances)", 10, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool UseFootprint = true;

	[InputParameter("Footprint: imbalance ratio (ask vs bid one tick below)", 11, 1.5, 10.0, 0.5, 1, null)]
	public double ImbRatio = 3.0;

	[InputParameter("Footprint: stacked = this many consecutive levels", 12, 2.0, 6.0, 1.0, 0, null)]
	public int StackLevels = 3;

	[InputParameter("Footprint: minimum volume on the dominant side (0 = half the bar's average level)", 13, 0.0, 100000.0, 1.0, 0, null)]
	public double MinLevelVolume;

	[InputParameter("THIN: depth-taken rank below", 20, 0.1, 0.6, 0.05, 2, null)]
	public double ThinRank = 0.35;

	[InputParameter("BATTLE: one-sidedness rank below", 21, 0.1, 0.6, 0.05, 2, null)]
	public double BalanceRank = 0.35;

	[InputParameter("BATTLE: give-back or wick at least (fraction of the bar)", 22, 0.3, 0.9, 0.05, 2, null)]
	public double ReverseFrac = 0.5;

	[InputParameter("ABSORBED (wall): 4-bar impulse rank at least, with no net move", 23, 0.4, 0.95, 0.05, 2, null)]
	public double WallRank = 0.65;

	[InputParameter("REAL from strength", 24, 20.0, 80.0, 5.0, 0, null)]
	public int RealFrom = 50;

	[InputParameter("Acceleration: lag (bars)", 30, 1.0, 10.0, 1.0, 0, null)]
	public int AccelLag = 1;

	[InputParameter("Acceleration: smoothing (EMA bars, 1 = off)", 31, 1.0, 20.0, 1.0, 0, null)]
	public int AccelSmooth = 3;

	[InputParameter("Acceleration: meaningful >= x typical change", 32, 0.3, 5.0, 0.1, 1, null)]
	public double AccelMin = 1.0;

	[InputParameter("Acceleration: THRUST / STALL >= x typical change", 33, 0.5, 10.0, 0.1, 1, null)]
	public double AccelExtreme = 2.0;

	[InputParameter("Acceleration: lane height (% of window, 0 = off)", 34, 0.0, 60.0, 1.0, 0, null)]
	public int AccelHeight = 28;

	[InputParameter("Acceleration: show THRUST / STALL / flip marks", 35, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowMarks = true;

	[InputParameter("Card", 40, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Top left", Corner.TopLeft, "Top right", Corner.TopRight, "Bottom left", Corner.BottomLeft, "Bottom right", Corner.BottomRight, "Hidden", Corner.Hidden
	})]
	public Corner CardPlace = Corner.TopLeft;

	[InputParameter("Card rows", 41, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Full", CardRowsMode.Full, "Compact", CardRowsMode.Compact
	})]
	public CardRowsMode CardRows;

	[InputParameter("Card size (%)", 42, 70.0, 160.0, 5.0, 0, null)]
	public int CardSize = 100;

	[InputParameter("Card background: black (%)", 43, 0.0, 100.0, 5.0, 0, null)]
	public int CardOpacity = 92;

	[InputParameter("Lane background", 50, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color LaneBack = Color.Black;

	[InputParameter("Show the REAL threshold lines (+/- RealFrom)", 51, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowLevels = true;

	[InputParameter("Show lane captions and right-side readouts", 52, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowLaneText = true;

	[InputParameter("Font size", 53, 6.0, 14.0, 0.5, 1, null)]
	public double FontSize = 7.5;

	[InputParameter("Keep indicator title / settings gear area clear (px wide)", 54, 0.0, 800.0, 10.0, 0, null)]
	public int TitleAreaWidth = 300;

	[InputParameter("Readings follow the forming bar (off = last finished bar)", 60, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool LiveReadout = true;

	[InputParameter("LIVE: max redraws per second", 61, 1.0, 60.0, 1.0, 0, null)]
	public int LiveFps = 15;

	// ---------------- state ----------------

	private const int MinRankBars = 20;

	private Bar[] _b = new Bar[0];
	private int _n;
	private readonly object _sync = new object();
	private readonly object _paintSync = new object();   // the picture copy and the GDI objects; take it before _sync
	private Bar[] _pb = new Bar[0];
	private int _pn;
	private int _dirtyFrom;
	private double _pHit = -1.0;   // the picture's hit rate
	private double _hit = -1.0;    // REAL bars followed by a same-direction bar, share of the last Lookback REAL bars
	private int _hitN;
	private LineSeries _line;
	private Agg _agg;
	private double _aggN;
	private double _barHeight;
	private double _tick;
	private bool _vaLoaded;
	private volatile bool _recalcAsked;
	private long _liveAt;
	private int _winNum = -1;
	private Rectangle _winRect;
	private float _laneTop;
	private float _laneBot;
	private float _accTop;
	private float _accBot;
	private double _reserveFrac;
	private string _chartTag = "";

	// footprint work arrays (compute thread only)
	private double[] _fpPrice = new double[64];
	private double[] _fpBuy = new double[64];
	private double[] _fpSell = new double[64];
	private int[] _fpOrder = new int[64];
	private bool[] _fpBuyFlag = new bool[64];
	private bool[] _fpSellFlag = new bool[64];
	private readonly double[] _rankBuf = new double[2048];

	// GDI caches (paint thread, under _paintSync)
	private readonly Dictionary<long, Pen> _pens = new Dictionary<long, Pen>();
	private readonly Dictionary<long, Pen> _dashPens = new Dictionary<long, Pen>();
	private readonly Dictionary<int, SolidBrush> _brushes = new Dictionary<int, SolidBrush>();
	private readonly Dictionary<(string, int), float> _textW = new Dictionary<(string, int), float>();
	private float _textWDpi;
	private Font _f;
	private Font _fb;
	private Font _fs;
	private Font _fh;
	private float _lineH = 12f;
	private float _lineHs = 10f;
	private float _lineHh = 18f;
	private readonly GraphicsPath _path = new GraphicsPath();
	private readonly PointF[] _pts4 = new PointF[4];
	private readonly PointF[] _pts3 = new PointF[3];
	private PointF[] _pfA = new PointF[0];
	private PointF[] _pfB = new PointF[0];
	private int[] _pIdx = new int[0];
	private static readonly StringFormat NoWrap = new StringFormat
	{
		Trimming = StringTrimming.None,
		FormatFlags = (StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
	};
	private static readonly StringFormat Cell = new StringFormat(StringFormatFlags.NoWrap)
	{
		Trimming = StringTrimming.EllipsisCharacter
	};

	// ---------------- colours ----------------

	private Color BullColor => Skin switch
	{
		Theme.ProDark => Color.FromArgb(38, 198, 145),
		Theme.Neon => Color.FromArgb(57, 255, 20),
		Theme.Classic => Color.FromArgb(0, 150, 0),
		Theme.Ocean => Color.FromArgb(0, 191, 255),
		Theme.Monochrome => Color.FromArgb(225, 225, 225),
		Theme.Ember => Color.FromArgb(255, 190, 60),
		Theme.Ice => Color.FromArgb(120, 220, 255),
		Theme.Aurora => Color.FromArgb(90, 240, 190),
		Theme.Terminal => Color.FromArgb(0, 230, 118),
		Theme.Sunset => Color.FromArgb(255, 205, 100),
		Theme.HighContrast => Color.FromArgb(61, 139, 253),
		Theme.Custom => CustomBull,
		Theme.AtasX => Color.FromArgb(0, 204, 106),
		Theme.DeepCharts => Color.FromArgb(0, 224, 255),
		Theme.Bookmap => Color.FromArgb(52, 152, 255),
		Theme.TradingView => Color.FromArgb(8, 153, 129),
		Theme.Cyberpunk => Color.FromArgb(0, 255, 170),
		Theme.Dracula => Color.FromArgb(80, 250, 123),
		Theme.Nordic => Color.FromArgb(136, 192, 208),
		Theme.MidnightGold => Color.FromArgb(245, 200, 90),
		Theme.Solarized => Color.FromArgb(42, 161, 152),
		Theme.Matrix => Color.FromArgb(0, 255, 65),
		_ => Color.LimeGreen,
	};

	private Color BearColor => Skin switch
	{
		Theme.ProDark => Color.FromArgb(235, 64, 92),
		Theme.Neon => Color.FromArgb(255, 20, 147),
		Theme.Classic => Color.FromArgb(190, 0, 0),
		Theme.Ocean => Color.FromArgb(255, 140, 0),
		Theme.Monochrome => Color.FromArgb(95, 95, 95),
		Theme.Ember => Color.FromArgb(220, 60, 40),
		Theme.Ice => Color.FromArgb(200, 90, 170),
		Theme.Aurora => Color.FromArgb(235, 80, 140),
		Theme.Terminal => Color.FromArgb(255, 70, 70),
		Theme.Sunset => Color.FromArgb(200, 60, 120),
		Theme.HighContrast => Color.FromArgb(255, 157, 46),
		Theme.Custom => CustomBear,
		Theme.AtasX => Color.FromArgb(255, 77, 77),
		Theme.DeepCharts => Color.FromArgb(255, 64, 150),
		Theme.Bookmap => Color.FromArgb(255, 72, 72),
		Theme.TradingView => Color.FromArgb(242, 54, 69),
		Theme.Cyberpunk => Color.FromArgb(255, 0, 115),
		Theme.Dracula => Color.FromArgb(255, 85, 85),
		Theme.Nordic => Color.FromArgb(191, 97, 106),
		Theme.MidnightGold => Color.FromArgb(225, 75, 95),
		Theme.Solarized => Color.FromArgb(220, 50, 47),
		Theme.Matrix => Color.FromArgb(255, 51, 51),
		_ => Color.Red,
	};

	private Color GoldColor => Skin switch
	{
		Theme.ProDark => Color.FromArgb(255, 200, 0),
		Theme.Neon => Color.FromArgb(0, 240, 255),
		Theme.Classic => Color.FromArgb(80, 80, 200),
		Theme.Ocean => Color.FromArgb(200, 230, 255),
		Theme.Monochrome => Color.FromArgb(155, 155, 155),
		Theme.Ember => Color.FromArgb(255, 240, 200),
		Theme.Ice => Color.FromArgb(235, 245, 255),
		Theme.Aurora => Color.FromArgb(200, 170, 255),
		Theme.Terminal => Color.FromArgb(190, 255, 190),
		Theme.Sunset => Color.FromArgb(255, 235, 190),
		Theme.HighContrast => Color.FromArgb(235, 235, 235),
		Theme.AtasX => Color.FromArgb(255, 214, 79),
		Theme.DeepCharts => Color.FromArgb(240, 240, 255),
		Theme.Bookmap => Color.FromArgb(255, 210, 60),
		Theme.TradingView => Color.FromArgb(41, 98, 255),
		Theme.Cyberpunk => Color.FromArgb(245, 230, 0),
		Theme.Dracula => Color.FromArgb(189, 147, 249),
		Theme.Nordic => Color.FromArgb(235, 203, 139),
		Theme.MidnightGold => Color.FromArgb(130, 210, 255),
		Theme.Solarized => Color.FromArgb(181, 137, 0),
		Theme.Matrix => Color.FromArgb(170, 255, 210),
		_ => Color.FromArgb(255, 200, 0),
	};

	private static readonly Color Amber = Color.FromArgb(255, 138, 61);
	private static readonly Color Grey = Color.FromArgb(150, 158, 175);
	private static readonly Color Dim = Color.FromArgb(128, 136, 152);
	private static readonly Color TextCol = Color.FromArgb(232, 236, 244);
	private static readonly Color Ink = Color.FromArgb(10, 12, 16);

	public bool IsRequirePriceLevelsCalculation => true;

	public TNXImbalanceStrength()
	{
		base.Name = "TNX IMBALANCE STRENGTH";
		base.Description = "Imbalance Strength -100..+100 per bar from eight ranked order-flow dials (participation, effort, one-sidedness, depth taken, give-back, persistence, stacked footprint imbalances, impulse vs move) with a REAL / THIN / BATTLE / ABSORBED / FADING verdict and its acceleration.";
		base.SeparateWindow = true;
		_line = AddLineSeries("IS", Color.Transparent, 1);
		_line.ShowLineMarker = false;
	}

	// ---------------- lifecycle ----------------

	protected override void OnInit()
	{
		try
		{
			lock (_sync)
			{
				_b = new Bar[0];
				_n = 0;
				_dirtyFrom = 0;
				_hit = -1.0;
				_hitN = 0;
				_vaLoaded = false;
				DetectGeometry();
			}
		}
		catch (Exception ex)
		{
			Report("OnInit", ex);
		}
	}

	private void DetectGeometry()
	{
		_agg = Agg.Other;
		_aggN = 0.0;
		_barHeight = 0.0;
		try
		{
			_tick = base.Symbol?.TickSize ?? 0.0;
			HistoryAggregation ha = base.HistoricalData?.Aggregation;
			if (ha is HistoryAggregationRangeBars rb)
			{
				_agg = Agg.Range;
				if (_tick > 0.0)
				{
					_barHeight = (double)rb.RangeBars * _tick;
				}
			}
			else if (ha is HistoryAggregationTickBars tb)
			{
				if (tb.TicksCount > 0)
				{
					_agg = Agg.Tick;
					_aggN = tb.TicksCount;
				}
			}
			else if (ha is HistoryAggregationRenko rk)
			{
				if (_tick > 0.0)
				{
					_barHeight = (double)rk.BrickSize * _tick;
				}
			}
			else if (ha != null && ha.GetType() == typeof(HistoryAggregationTime))
			{
				Period p = ((HistoryAggregationTime)ha).Period;
				if (p.PeriodMultiplier > 0 && (p.BasePeriod == BasePeriod.Second || p.BasePeriod == BasePeriod.Minute || p.BasePeriod == BasePeriod.Hour))
				{
					_agg = Agg.Time;
					_aggN = p.Ticks;
				}
			}
		}
		catch (Exception ex)
		{
			Report("DetectGeometry", ex);
		}
		_chartTag = _agg switch
		{
			Agg.Range => ((_tick > 0.0 && _barHeight > 0.0) ? ("range " + Math.Round(_barHeight / _tick).ToString("0", CultureInfo.InvariantCulture) + "t") : "range bars"),
			Agg.Tick => _aggN.ToString("0", CultureInfo.InvariantCulture) + " tick",
			Agg.Time => ((_aggN >= 6E+08) ? ((_aggN / 6E+08).ToString("0.##", CultureInfo.InvariantCulture) + " min") : ((_aggN / 1E+07).ToString("0.##", CultureInfo.InvariantCulture) + " s")),
			_ => "",
		};
	}

	public void VolumeAnalysisData_Loaded()
	{
		// the platform finished the volume analysis (possibly on another thread, possibly before OnUpdate replayed all bars):
		// mark it and ask for a full recompute at the next update; also recompute now under the lock
		_vaLoaded = true;
		_recalcAsked = true;
		try
		{
			lock (_sync)
			{
				RecalcAll();
			}
		}
		catch (Exception ex)
		{
			Report("VolumeAnalysisData_Loaded", ex);
		}
		Redraw();
	}

	protected override void OnUpdate(UpdateArgs args)
	{
		bool draw = false;
		try
		{
			lock (_sync)
			{
				draw = UpdateCore(args);
			}
		}
		catch (Exception ex)
		{
			Report("OnUpdate", ex);
		}
		if (draw)
		{
			Redraw();
		}
	}

	private bool UpdateCore(UpdateArgs args)
	{
		int count = base.Count;
		int last = count - 1;
		if (last < 0)
		{
			return false;
		}
		if (_n == 0 && _agg == Agg.Other && _barHeight == 0.0 && base.HistoricalData?.Aggregation != null)
		{
			DetectGeometry();
		}
		if (count < _n || _recalcAsked)
		{
			_recalcAsked = false;
			RecalcAll();
			return true;
		}
		Ensure(count);
		int first = ((_n < count || (args != null && args.Reason == UpdateReason.NewBar)) ? Math.Max(0, Math.Min(_n, last) - 1) : last);
		for (int i = first; i <= last; i++)
		{
			Compute(i, i < last);
		}
		_n = count;
		if (args != null && args.Reason == UpdateReason.HistoricalBar)
		{
			return false;
		}
		if (!LiveReadout && first == last)
		{
			return false;
		}
		int fps = Math.Max(1, Math.Min(60, LiveFps));
		long now = Environment.TickCount64;
		if (now - _liveAt < 1000L / fps)
		{
			return false;
		}
		_liveAt = now;
		return true;
	}

	private void RecalcAll()
	{
		int count = base.Count;
		if (count <= 0)
		{
			return;
		}
		Ensure(count);
		_dirtyFrom = 0;
		_hit = -1.0;
		_hitN = 0;
		for (int i = 0; i < count; i++)
		{
			Compute(i, i < count - 1);
		}
		_n = count;
	}

	private void Ensure(int n)
	{
		if (_b.Length < n)
		{
			Array.Resize(ref _b, Math.Max(n, Math.Max(1024, _b.Length * 2)));
		}
	}

	private int Off(int abs)
	{
		return base.Count - 1 - abs;
	}

	private static double Safe(double v)
	{
		if (double.IsNaN(v) || double.IsInfinity(v) || v == double.MaxValue || v == double.MinValue)
		{
			return 0.0;
		}
		return v;
	}

	private static long NowTicks()
	{
		try
		{
			return Core.Instance.TimeUtils.DateTimeUtcNow.Ticks;
		}
		catch
		{
			return DateTime.UtcNow.Ticks;
		}
	}

	private void Redraw()
	{
		try
		{
			base.CurrentChart?.RedrawBuffer();
		}
		catch (Exception ex)
		{
			Report("Redraw", ex);
		}
	}

	private static long _errors;

	private static string _lastError = "";

	// errors are counted (the card shows the count and the last place), never thrown into the platform
	private static void Report(string where, Exception ex)
	{
		Interlocked.Increment(ref _errors);
		_lastError = where + ": " + (ex?.Message ?? "");
	}

	// ---------------- compute ----------------

	private void Compute(int abs, bool closed)
	{
		int off = Off(abs);
		if (off < 0)
		{
			return;
		}
		if (abs < _dirtyFrom)
		{
			_dirtyFrom = abs;
		}
		ref Bar b = ref _b[abs];
		VolumeAnalysisData va = null;
		try
		{
			va = GetVolumeAnalysisData(off);
		}
		catch
		{
			va = null;
		}
		VolumeAnalysisItem t = va?.Total;
		double delta = 0.0, buy = 0.0, sell = 0.0, vol, maxD = 0.0, minD = 0.0;
		bool real = false;
		if (t != null)
		{
			delta = Safe(t.Delta);
			buy = Safe(t.BuyVolume);
			sell = Safe(t.SellVolume);
			vol = Safe(t.Volume);
			maxD = Math.Max(Safe(t.MaxDelta), Math.Max(0.0, delta));
			minD = Math.Min(Safe(t.MinDelta), Math.Min(0.0, delta));
			real = vol > 0.0 || buy > 0.0 || sell > 0.0;
		}
		else
		{
			vol = Safe(GetPrice(PriceType.Volume, off));
		}
		b.Has = true;
		b.Closed = closed;
		b.Real = real;
		b.Delta = delta;
		b.Buy = buy;
		b.Sell = sell;
		b.Vol = vol;
		b.MaxD = maxD;
		b.MinD = minD;
		b.O = GetPrice(PriceType.Open, off);
		b.H = GetPrice(PriceType.High, off);
		b.L = GetPrice(PriceType.Low, off);
		b.C = GetPrice(PriceType.Close, off);
		b.Ticks = Time(off).Ticks;
		b.Dur = Math.Max(0.1, (double)(((off >= 1) ? Time(off - 1).Ticks : NowTicks()) - b.Ticks) / 1E+07);
		b.F = (closed ? 1.0 : Progress(abs, off));
		b.Dir = ((b.C > b.O) ? 1 : ((b.C < b.O) ? (-1) : 0));
		b.Side = ((delta > 0.0) ? 1 : ((delta < 0.0) ? (-1) : ((b.Dir != 0) ? b.Dir : ((abs > 0) ? _b[abs - 1].Side : 1))));
		b.Agree = b.Dir != 0 && b.Side == b.Dir;
		b.Vps = vol / b.Dur;
		double bs = buy + sell;
		b.Omega = ((bs > 0.0) ? (Math.Abs(delta) / bs) : 0.0);
		double tick = ((_tick > 0.0) ? _tick : 1.0);
		double dx = Math.Abs(b.C - b.O) / tick;
		b.Zeta = ((_agg == Agg.Range) ? vol : (vol / Math.Max(1.0, dx)));
		double eSide = ((delta >= 0.0) ? Math.Max(maxD, 0.0) : Math.Max(0.0 - minD, 0.0));
		b.G = ((eSide > 0.0) ? Math.Max(0.0, Math.Min(1.0, (eSide - Math.Abs(delta)) / eSide)) : 0.0);
		double range = b.H - b.L;
		b.W = ((range > 0.0) ? Math.Max(0.0, Math.Min(1.0, 1.0 - Math.Abs(b.C - b.O) / range)) : 0.0);
		// persistence (closed bars drive it; the forming bar reads the previous value)
		if (abs > 0)
		{
			ref Bar p = ref _b[abs - 1];
			double beta = 2.0 / 9.0;
			double agree = ((p.Real && real) ? (double)(b.Side * p.Side) : 0.0);
			b.Psi = (closed ? (p.Psi + beta * (agree - p.Psi)) : p.Psi);
			b.Run = ((p.Real && real && p.Side == b.Side) ? (p.Run + 1) : 1);
			b.Flip = p.Real && real && p.Side != b.Side;
		}
		else
		{
			b.Psi = 0.0;
			b.Run = 1;
			b.Flip = false;
		}
		// impulse over 4 bars and the net move
		if (abs >= 4)
		{
			double j = 0.0;
			bool ok = true;
			for (int k = abs - 3; k <= abs; k++)
			{
				if (!_b[k].Real)
				{
					ok = false;
				}
				j += _b[k].Delta;
			}
			b.J4 = (ok ? j : 0.0);
			b.Move4 = Math.Abs(b.C - _b[abs - 4].C) / tick;
		}
		else
		{
			b.J4 = 0.0;
			b.Move4 = 0.0;
		}
		// footprint
		b.FpOk = false;
		b.KWith = (b.KAgainst = 0);
		b.ShWith = (b.ShAgainst = 0.0);
		b.Phi = 0.5;
		b.UaRejected = false;
		b.Levels = 0;
		b.MaxLot = 0.0;
		if (UseFootprint && va != null && real)
		{
			Footprint(ref b, va);
		}
		// ranks against the previous real finished bars
		int n = Math.Max(MinRankBars, Lookback);
		b.RPi = Rank(abs, n, 0, b.Vps);
		b.REff = Rank(abs, n, 1, Math.Abs(delta));
		b.ROmega = Rank(abs, n, 2, b.Omega);
		b.RZeta = Rank(abs, n, 3, b.Zeta);
		b.RJ4 = Rank(abs, n, 4, Math.Abs(b.J4));
		double typMove = TypicalMove(abs, n);
		// fake, strength, state
		if (!real || b.RPi < 0.0)
		{
			b.Fake = 0.0;
			b.Gscore = 0.0;
			b.IS = 0.0;
			b.St = State.NoData;
			b.Trapped = (b.Wall = false);
		}
		else
		{
			double fTrap = ((!b.Agree && b.Dir != 0) ? 1.0 : 0.0);
			bool wall = b.RJ4 >= WallRank && abs >= 4 && b.Move4 <= typMove;
			double fAbs = (wall ? 1.0 : 0.0);
			double fGb = ((b.G >= ReverseFrac) ? b.G : 0.0);
			double fWick = ((b.W >= ReverseFrac) ? b.W : 0.0);
			// THIN = nobody there: a quiet market (participation rank under the threshold), or a cheap move (little depth
			// taken) in a market that is not busy. A fast one-sided push takes little depth per range bar by construction
			// (it lifts the book quickly) and is NOT thin while the market is busy: participation decides, depth confirms.
			bool thin = b.RPi < ThinRank || (b.RZeta < ThinRank && b.RPi < 0.6);
			double fThin = (thin ? (1.0 - b.RPi) : 0.0);
			b.Fake = Math.Max(Math.Max(fTrap, fAbs), Math.Max(Math.Max(fGb, fWick), fThin));
			double pi = Math.Max(0.05, b.RPi);
			double om = Math.Max(0.05, b.ROmega);
			double ef = Math.Max(0.05, b.REff);
			double ph = Math.Max(0.05, b.Phi);
			double ps = Math.Max(0.05, (1.0 + b.Psi) / 2.0);
			double g = Math.Pow(pi * om * ef * Math.Sqrt(ph) * Math.Sqrt(ps), 0.25);
			b.Gscore = g;
			double g1 = Math.Max(0.0, (g - 0.05) / 0.95);
			b.IS = (double)b.Side * 100.0 * g1 * (1.0 - 0.7 * b.Fake);
			b.Trapped = !b.Agree && b.Dir != 0;
			b.Wall = wall;
			bool bothStacks = b.FpOk && b.KWith >= StackLevels && b.KAgainst >= StackLevels;
			if (thin)
			{
				b.St = State.Thin;
			}
			else if (b.ROmega < BalanceRank || b.G >= ReverseFrac || b.W >= ReverseFrac || bothStacks)
			{
				b.St = State.Battle;
			}
			else if (b.Trapped || wall)
			{
				b.St = State.Absorbed;
			}
			else
			{
				b.St = State.Real;   // FADING is decided below, after the acceleration
			}
		}
		// acceleration: the side-framed change of IS (positive = the current side strengthening), EMA, in typical changes
		int lag = Math.Max(1, AccelLag);
		double prevIS = ((abs - lag >= 0) ? _b[abs - lag].IS : b.IS);
		double d = (double)b.Side * (b.IS - prevIS);
		int m = Math.Max(1, AccelSmooth);
		double alpha = 2.0 / (double)(m + 1);
		if (!closed)
		{
			alpha = 1.0 - Math.Pow(1.0 - alpha, Math.Max(0.05, b.F));
		}
		double accPrev = ((abs > 0) ? _b[abs - 1].AccE : d);
		b.AccE = ((m <= 1 || abs == 0) ? d : (accPrev + alpha * (d - accPrev)));
		// the unit is the median |change of IS| of the lookback, never under 5 points (a flat stretch would make the first
		// real move read as 30x), and the reading is capped at 10x (the lane shows a cap on a cut bar)
		double typ = TypicalAccChange(abs, n);
		b.AccN = Math.Max(-10.0, Math.Min(10.0, b.AccE / Math.Max(typ, 5.0)));
		if (b.St == State.Real && abs > 0 && _b[abs - 1].Real && _b[abs - 1].Side == b.Side && b.AccN <= -1.0 && b.REff >= 0.35)
		{
			b.St = State.Fading;
		}
		// the previous bar's confirmation (known now) and the hit rate
		b.Kappa = 0;
		if (closed && abs > 0)
		{
			ref Bar p = ref _b[abs - 1];
			if (p.Real && p.Kappa == 0 && p.Dir != 0)
			{
				double mv = b.C - p.C;
				p.Kappa = ((mv == 0.0) ? 0 : (((double)p.Dir * mv > 0.0) ? 1 : (-1)));
				if (p.Kappa != 0 && p.St == State.Real)
				{
					_hitN++;
				}
			}
			if (abs - 1 < _dirtyFrom)
			{
				_dirtyFrom = abs - 1;
			}
		}
		if (closed)
		{
			_hit = HitRate(abs, n);
		}
	}

	private double Progress(int abs, int off)
	{
		try
		{
			double f = 1.0;
			switch (_agg)
			{
			case Agg.Range:
				if (_barHeight > 0.0)
				{
					f = (_b[abs].H - _b[abs].L) / _barHeight;
				}
				break;
			case Agg.Tick:
				if (_aggN > 0.0)
				{
					f = Ticks(off) / _aggN;
				}
				break;
			case Agg.Time:
				if (_aggN > 0.0)
				{
					f = (double)(NowTicks() - _b[abs].Ticks) / _aggN;
				}
				break;
			}
			return (double.IsNaN(f) || double.IsInfinity(f)) ? 1.0 : Math.Max(0.0, Math.Min(1.0, f));
		}
		catch
		{
			return 1.0;
		}
	}

	// the rank of v among the last n real finished bars before abs (ties count half), -1 while fewer than MinRankBars
	private double Rank(int abs, int n, int what, double v)
	{
		int seen = 0;
		int below = 0;
		int equal = 0;
		for (int i = abs - 1; i >= 0 && seen < n; i--)
		{
			ref Bar x = ref _b[i];
			if (!x.Real || !x.Closed)
			{
				continue;
			}
			seen++;
			double w = what switch
			{
				0 => x.Vps,
				1 => Math.Abs(x.Delta),
				2 => x.Omega,
				3 => x.Zeta,
				_ => Math.Abs(x.J4),
			};
			if (w < v)
			{
				below++;
			}
			else if (w == v)
			{
				equal++;
			}
		}
		if (seen < MinRankBars)
		{
			return -1.0;
		}
		return ((double)below + 0.5 * (double)equal) / (double)seen;
	}

	// the median |close - open| in ticks of the last n real finished bars (= the range size on range bars)
	private double TypicalMove(int abs, int n)
	{
		double tick = ((_tick > 0.0) ? _tick : 1.0);
		int m = 0;
		for (int i = abs - 1; i >= 0 && m < n && m < _rankBuf.Length; i--)
		{
			ref Bar x = ref _b[i];
			if (x.Real && x.Closed)
			{
				_rankBuf[m++] = Math.Abs(x.C - x.O) / tick;
			}
		}
		if (m < 5)
		{
			return ((_barHeight > 0.0) ? (_barHeight / tick) : 1.0);
		}
		Array.Sort(_rankBuf, 0, m);
		return Math.Max(1.0, (m % 2 == 1) ? _rankBuf[m / 2] : (0.5 * (_rankBuf[m / 2 - 1] + _rankBuf[m / 2])));
	}

	// the median |change of IS| over the last n real finished bars (the acceleration's unit)
	private double TypicalAccChange(int abs, int n)
	{
		int lag = Math.Max(1, AccelLag);
		int m = 0;
		for (int i = abs - 1; i - lag >= 0 && m < n && m < _rankBuf.Length; i--)
		{
			ref Bar x = ref _b[i];
			if (x.Real && x.Closed)
			{
				_rankBuf[m++] = Math.Abs(x.IS - _b[i - lag].IS);
			}
		}
		if (m < 5)
		{
			return 0.0;
		}
		Array.Sort(_rankBuf, 0, m);
		return (m % 2 == 1) ? _rankBuf[m / 2] : (0.5 * (_rankBuf[m / 2 - 1] + _rankBuf[m / 2]));
	}

	// the share of the last n REAL bars (with a known response) that the next bar confirmed
	private double HitRate(int abs, int n)
	{
		int real = 0;
		int hit = 0;
		for (int i = abs - 1; i >= 0 && real < n; i--)
		{
			ref Bar x = ref _b[i];
			if (x.Real && x.Closed && x.St == State.Real && x.Kappa != 0)
			{
				real++;
				if (x.Kappa > 0)
				{
					hit++;
				}
			}
		}
		return (real >= 10) ? ((double)hit / (double)real) : (-1.0);
	}

	// the footprint: levels sorted by price, snapped to the tick grid; the diagonal rule (ask at p against the bid one tick
	// below; bid at p against the ask one tick above), ratio ImbRatio, a volume floor; stacks = consecutive flags; the share
	// of the bar's volume inside the flagged levels; an unfinished auction at the extreme opposite to the close
	private void Footprint(ref Bar b, VolumeAnalysisData va)
	{
		Dictionary<double, VolumeAnalysisItem> levels = va.PriceLevels;
		if (levels == null)
		{
			return;
		}
		int n = 0;
		try
		{
			int cap = levels.Count;
			if (cap < 2)
			{
				return;
			}
			if (_fpPrice.Length < cap)
			{
				int sz = Math.Max(cap, _fpPrice.Length * 2);
				_fpPrice = new double[sz];
				_fpBuy = new double[sz];
				_fpSell = new double[sz];
				_fpOrder = new int[sz];
				_fpBuyFlag = new bool[sz];
				_fpSellFlag = new bool[sz];
			}
			foreach (KeyValuePair<double, VolumeAnalysisItem> kv in levels)
			{
				if (n >= _fpPrice.Length)
				{
					break;
				}
				_fpPrice[n] = kv.Key;
				_fpBuy[n] = Safe(kv.Value.BuyVolume);
				_fpSell[n] = Safe(kv.Value.SellVolume);
				n++;
			}
		}
		catch (InvalidOperationException)
		{
			return;   // the forming bar's levels changed under us: keep the previous reading, the next tick reads again
		}
		if (n < 2)
		{
			return;
		}
		// sort by price (insertion sort: a range bar has R + 1 levels)
		for (int i = 0; i < n; i++)
		{
			_fpOrder[i] = i;
		}
		for (int i = 1; i < n; i++)
		{
			int k = _fpOrder[i];
			double pk = _fpPrice[k];
			int j = i - 1;
			while (j >= 0 && _fpPrice[_fpOrder[j]] > pk)
			{
				_fpOrder[j + 1] = _fpOrder[j];
				j--;
			}
			_fpOrder[j + 1] = k;
		}
		double step = _tick;
		if (step <= 0.0)
		{
			step = double.MaxValue;
			for (int i = 1; i < n; i++)
			{
				double d = _fpPrice[_fpOrder[i]] - _fpPrice[_fpOrder[i - 1]];
				if (d > 0.0 && d < step)
				{
					step = d;
				}
			}
			if (step == double.MaxValue)
			{
				return;
			}
		}
		double tol = step * 0.01;
		double total = 0.0;
		double maxLvl = 0.0;
		for (int i = 0; i < n; i++)
		{
			double v = _fpBuy[i] + _fpSell[i];
			total += v;
			if (v > maxLvl)
			{
				maxLvl = v;
			}
		}
		if (total <= 0.0)
		{
			return;
		}
		double vmin = ((MinLevelVolume > 0.0) ? MinLevelVolume : (0.5 * total / (double)n));
		double ratio = Math.Max(1.5, ImbRatio);
		int ks = Math.Max(2, StackLevels);
		for (int i = 0; i < n; i++)
		{
			int k = _fpOrder[i];
			double p = _fpPrice[k];
			// the bid one tick below (0 when that level is missing) and the ask one tick above
			double bidBelow = 0.0;
			if (i > 0)
			{
				int kb = _fpOrder[i - 1];
				if (Math.Abs((p - _fpPrice[kb]) - step) <= tol)
				{
					bidBelow = _fpSell[kb];
				}
			}
			double askAbove = 0.0;
			if (i + 1 < n)
			{
				int ka = _fpOrder[i + 1];
				if (Math.Abs((_fpPrice[ka] - p) - step) <= tol)
				{
					askAbove = _fpBuy[ka];
				}
			}
			_fpBuyFlag[i] = i > 0 && _fpBuy[k] >= vmin && _fpBuy[k] >= ratio * Math.Max(bidBelow, 1.0);
			_fpSellFlag[i] = i + 1 < n && _fpSell[k] >= vmin && _fpSell[k] >= ratio * Math.Max(askAbove, 1.0);
		}
		int kBuy = 0, kSell = 0, runB = 0, runS = 0;
		double shBuy = 0.0, shSell = 0.0;
		for (int i = 0; i < n; i++)
		{
			int k = _fpOrder[i];
			double v = _fpBuy[k] + _fpSell[k];
			if (_fpBuyFlag[i])
			{
				runB++;
				shBuy += v;
			}
			else
			{
				runB = 0;
			}
			if (_fpSellFlag[i])
			{
				runS++;
				shSell += v;
			}
			else
			{
				runS = 0;
			}
			kBuy = Math.Max(kBuy, runB);
			kSell = Math.Max(kSell, runS);
		}
		shBuy /= total;
		shSell /= total;
		bool buyers = b.Side >= 0;
		b.FpOk = true;
		b.Levels = n;
		b.MaxLot = 0.0;
		b.KWith = (buyers ? kBuy : kSell);
		b.KAgainst = (buyers ? kSell : kBuy);
		b.ShWith = (buyers ? shBuy : shSell);
		b.ShAgainst = (buyers ? shSell : shBuy);
		double phi = 0.5 + 0.25 * Math.Min(b.KWith, ks) / (double)ks + 0.5 * (b.ShWith - b.ShAgainst) - 0.25 * Math.Min(b.KAgainst, ks) / (double)ks;
		b.Phi = Math.Max(0.0, Math.Min(1.0, phi));
		// unfinished auction at the rejected extreme: the extreme opposite to the close (on range bars the close is an extreme
		// by construction, so the closing side is two-sided far too often to mean anything)
		int lo = _fpOrder[0];
		int hi = _fpOrder[n - 1];
		bool closeAtHigh = b.H - b.C <= b.C - b.L;
		int rej = (closeAtHigh ? lo : hi);
		b.UaRejected = _fpBuy[rej] > 0.0 && _fpSell[rej] > 0.0;
	}

	// ---------------- paint ----------------

	public override void OnPaintChart(PaintChartEventArgs args)
	{
		base.OnPaintChart(args);
		if (base.CurrentChart == null || args?.Graphics == null || _n == 0)
		{
			return;
		}
		IChartWindow win = null;
		IChartWindow[] windows = base.CurrentChart.Windows;
		if (windows != null)
		{
			foreach (IChartWindow w in windows)
			{
				if (w.WindowNumber == args.WindowIndex)
				{
					win = w;
					break;
				}
			}
		}
		IChartWindowCoordinatesConverter conv = win?.CoordinatesConverter;
		if (conv == null)
		{
			return;
		}
		_winNum = win.WindowNumber;
		Graphics g = args.Graphics;
		GraphicsState st = g.Save();
		try
		{
			lock (_paintSync)
			{
				lock (_sync)
				{
					CopyForPaint();
				}
				PaintCore(g, args.Rectangle, conv);
			}
		}
		catch (Exception ex)
		{
			Report("OnPaintChart", ex);
		}
		finally
		{
			g.Restore(st);
		}
	}

	private void CopyForPaint()
	{
		if (_dirtyFrom != int.MaxValue)
		{
			int n = _n;
			if (_pb.Length < n)
			{
				Array.Resize(ref _pb, Math.Max(n, Math.Max(1024, _pb.Length * 2)));
			}
			int from = Math.Min(_dirtyFrom, _pn);
			if (n > from)
			{
				Array.Copy(_b, from, _pb, from, n - from);
			}
			_pn = n;
			_dirtyFrom = int.MaxValue;
		}
		_pHit = _hit;
	}

	private void PaintCore(Graphics g, Rectangle rect, IChartWindowCoordinatesConverter conv)
	{
		_winRect = rect;
		g.SetClip(rect);
		if (TitleAreaWidth > 0)
		{
			g.ExcludeClip(new Rectangle(rect.Left, rect.Top, Math.Min(rect.Width, TitleAreaWidth), 24));
		}
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		Fonts();
		int count = Math.Min(base.Count, _pn);
		if (count <= 0)
		{
			return;
		}
		// the visible bars, the official way: the bar index at the window's left and right edges
		int vL, vR;
		try
		{
			vL = (int)Math.Floor(conv.GetBarIndex(conv.GetTime(rect.Left)));
			vR = (int)Math.Ceiling(conv.GetBarIndex(conv.GetTime(rect.Right)));
		}
		catch
		{
			vL = Math.Max(0, count - 300);
			vR = count - 1;
		}
		vL = Math.Max(0, Math.Min(count - 1, vL - 1));
		vR = Math.Max(vL, Math.Min(count - 1, vR + 1));
		int bw = Math.Max(1, base.CurrentChart.BarsWidth);
		float barW = (float)Math.Max(1.0, Math.Min(bw - 2.0, bw * 0.68));
		// panes
		bool acc = AccelHeight > 0;
		float h = Math.Max(60f, rect.Height);
		float accH = (acc ? Math.Max(26f, h * Math.Min(60, AccelHeight) / 100f) : 0f);
		float axisW = (ShowLaneText ? (_fs.Size * 5.6f + 8f) : 0f);
		float xR = (float)rect.Right - axisW;
		float capH = _lineHs + 1f;
		float top = (float)rect.Top + 4f;
		float bot = (float)rect.Bottom - 3f;
		_accBot = bot;
		_accTop = (acc ? (bot - accH) : bot);
		_laneBot = (acc ? (_accTop - 6f) : bot);
		_laneTop = top;
		_reserveFrac = (acc ? Math.Min(0.7, (accH + 6f) / h) : 0.0);
		// the strength lane
		g.FillRectangle(Brush(LaneBack), rect.Left, _laneTop, rect.Width, _laneBot - _laneTop);
		float mid = (_laneTop + capH + _laneBot) / 2f;
		float half = Math.Max(4f, (_laneBot - _laneTop - capH) / 2f - 3f);
		Pen zeroPen = Pen(Color.FromArgb(90, Grey), 1f);
		g.DrawLine(zeroPen, rect.Left, mid, xR, mid);
		if (ShowLevels)
		{
			float ly = half * (float)Math.Min(100, Math.Max(20, RealFrom)) / 100f;
			Pen lp = DashPen(Color.FromArgb(55, Grey), 1f);
			g.DrawLine(lp, rect.Left, mid - ly, xR, mid - ly);
			g.DrawLine(lp, rect.Left, mid + ly, xR, mid + ly);
		}
		g.SetClip(new RectangleF(rect.Left, _laneTop, Math.Max(1f, xR - (float)rect.Left), Math.Max(1f, _laneBot - _laneTop)), CombineMode.Intersect);
		DrawStrength(g, conv, rect, vL, vR, count, bw, barW, mid, half, xR);
		g.SetClip(rect);
		if (TitleAreaWidth > 0)
		{
			g.ExcludeClip(new Rectangle(rect.Left, rect.Top, Math.Min(rect.Width, TitleAreaWidth), 24));
		}
		int last = count - 1;
		int read = ((LiveReadout || count < 2) ? last : (last - 1));
		if (ShowLaneText)
		{
			Axis(g, xR, _laneTop + capH, _laneBot, mid, half, 100.0, _pb[read].IS, (_pb[read].IS >= 0.0) ? BullColor : BearColor, false);
			g.DrawString("IMBALANCE STRENGTH  " + _chartTag, _fb, Brush(Color.FromArgb(200, Grey)), rect.Left + 8, _laneTop + 1f);
		}
		// the acceleration lane
		if (acc)
		{
			g.DrawLine(Pen(Color.FromArgb(255, 44, 48, 56), 1f), rect.Left, _accTop - 3f, rect.Right, _accTop - 3f);
			g.FillRectangle(Brush(LaneBack), rect.Left, _accTop, rect.Width, _accBot - _accTop);
			g.SetClip(new RectangleF(rect.Left, _accTop - 2f, rect.Width, Math.Max(1f, _accBot - _accTop + 2f)), CombineMode.Intersect);
			DrawAccel(g, conv, rect, vL, vR, count, bw, barW, xR, capH);
			g.SetClip(rect);
			if (TitleAreaWidth > 0)
			{
				g.ExcludeClip(new Rectangle(rect.Left, rect.Top, Math.Min(rect.Width, TitleAreaWidth), 24));
			}
		}
		if (CardPlace != Corner.Hidden)
		{
			DrawCard(g, rect, read);
		}
	}

	private float Xc(IChartWindowCoordinatesConverter conv, int abs, int bw)
	{
		return (float)conv.GetChartX(new DateTime(_pb[abs].Ticks, DateTimeKind.Utc)) + bw / 2f;
	}

	private Color SideCol(ref Bar b)
	{
		return (b.IS >= 0.0 || (b.IS == 0.0 && b.Side >= 0)) ? BullColor : BearColor;
	}

	// brightness by state: REAL bright, FADING bright, BATTLE half, THIN dark, ABSORBED half (its outline says it)
	private static int StateAlpha(State s)
	{
		return s switch
		{
			State.Real => 235,
			State.Fading => 215,
			State.Battle => 130,
			State.Absorbed => 140,
			State.Thin => 75,
			_ => 60,
		};
	}

	private void DrawStrength(Graphics g, IChartWindowCoordinatesConverter conv, Rectangle rect, int vL, int vR, int count, int bw, float barW, float mid, float half, float xR)
	{
		int last = count - 1;
		int n = vR - vL + 1;
		if (_pfA.Length < n)
		{
			_pfA = new PointF[Math.Max(n, _pfA.Length * 2)];
			_pfB = new PointF[_pfA.Length];
			_pIdx = new int[_pfA.Length];
		}
		int m = 0;
		for (int i = vL; i <= vR; i++)
		{
			if (!_pb[i].Has)
			{
				continue;
			}
			float x = Xc(conv, i, bw);
			if (x > xR + bw)
			{
				break;
			}
			if (x < rect.Left - bw)
			{
				continue;
			}
			_pfA[m] = new PointF(x, mid - half * (float)(_pb[i].IS / 100.0));
			_pIdx[m] = i;
			m++;
		}
		if (m == 0)
		{
			return;
		}
		float r = Math.Max(2.5f, Math.Min(5f, bw * 0.35f));
		switch (Style)
		{
		case Look.Area:
		{
			// a polarized area under a line; dots where the verdict is not REAL
			if (m >= 2)
			{
				PointF[] poly = new PointF[m + 2];
				Array.Copy(_pfA, poly, m);
				poly[m] = new PointF(_pfA[m - 1].X, mid);
				poly[m + 1] = new PointF(_pfA[0].X, mid);
				GraphicsState st = g.Save();
				g.SetClip(new RectangleF(rect.Left, _laneTop, xR - rect.Left, Math.Max(0f, mid - _laneTop)), CombineMode.Intersect);
				g.FillPolygon(Brush(Color.FromArgb(75, BullColor)), poly);
				g.Restore(st);
				st = g.Save();
				g.SetClip(new RectangleF(rect.Left, mid, xR - rect.Left, Math.Max(0f, _laneBot - mid)), CombineMode.Intersect);
				g.FillPolygon(Brush(Color.FromArgb(75, BearColor)), poly);
				g.Restore(st);
				for (int k = 1; k < m; k++)
				{
					ref Bar b = ref _pb[_pIdx[k]];
					g.DrawLine(Pen(Color.FromArgb(StateAlpha(b.St), Bright(SideCol(ref b), 0.2)), 1.7f), _pfA[k - 1], _pfA[k]);
				}
			}
			for (int k = 0; k < m; k++)
			{
				int i = _pIdx[k];
				ref Bar b = ref _pb[i];
				if (i == last)
				{
					g.DrawEllipse(Pen(Color.FromArgb(230, Bright(SideCol(ref b), 0.35)), 1.2f), _pfA[k].X - 3f, _pfA[k].Y - 3f, 6f, 6f);
				}
				StateMark(g, ref b, _pfA[k].X, _pfA[k].Y, mid, r, i == last);
			}
			break;
		}
		case Look.Needles:
		{
			float w = Math.Max(1f, Math.Min(2.5f, barW * 0.35f));
			for (int k = 0; k < m; k++)
			{
				int i = _pIdx[k];
				ref Bar b = ref _pb[i];
				int a = StateAlpha(b.St);
				if (i == last)
				{
					a = a * 45 / 100;
				}
				Color c = SideCol(ref b);
				g.DrawLine(Pen(Color.FromArgb(a, c), w), _pfA[k].X, mid, _pfA[k].X, _pfA[k].Y);
				if (b.St == State.Real && Math.Abs(b.IS) >= RealFrom && i != last)
				{
					float dr = ((Math.Abs(b.IS) >= 75.0) ? 3.5f : 2.5f);
					g.FillEllipse(Brush(Ink), _pfA[k].X - dr - 1f, _pfA[k].Y - dr - 1f, 2f * dr + 2f, 2f * dr + 2f);
					g.FillEllipse(Brush(Color.FromArgb(255, Bright(c, 0.3))), _pfA[k].X - dr, _pfA[k].Y - dr, 2f * dr, 2f * dr);
				}
				StateMark(g, ref b, _pfA[k].X, _pfA[k].Y, mid, r, i == last);
			}
			break;
		}
		case Look.Lollipop:
		{
			for (int k = 0; k < m; k++)
			{
				int i = _pIdx[k];
				ref Bar b = ref _pb[i];
				Color c = SideCol(ref b);
				int a = StateAlpha(b.St);
				g.DrawLine(Pen(Color.FromArgb(a * 60 / 100, c), 2f), _pfA[k].X, mid, _pfA[k].X, _pfA[k].Y);
				float br = 2.5f + 3.5f * (float)Math.Min(3.0, Math.Abs(b.AccN)) / 3f * Math.Max(1f, barW / 5f);
				br = Math.Min(br, Math.Max(3f, barW * 0.9f));
				bool with = b.AccN >= 0.0;
				if (i == last)
				{
					g.DrawEllipse(Pen(Color.FromArgb(200, c), 1.2f), _pfA[k].X - br, _pfA[k].Y - br, 2f * br, 2f * br);
				}
				else if (with)
				{
					g.FillEllipse(Brush(Color.FromArgb(a, c)), _pfA[k].X - br, _pfA[k].Y - br, 2f * br, 2f * br);
					g.DrawEllipse(Pen(Color.FromArgb(a, Bright(c, 0.5)), 1f), _pfA[k].X - br, _pfA[k].Y - br, 2f * br, 2f * br);
				}
				else
				{
					g.FillEllipse(Brush(LaneBack), _pfA[k].X - br, _pfA[k].Y - br, 2f * br, 2f * br);
					g.DrawEllipse(Pen(Color.FromArgb(a, c), 1.6f), _pfA[k].X - br, _pfA[k].Y - br, 2f * br, 2f * br);
				}
				StateMark(g, ref b, _pfA[k].X, _pfA[k].Y, mid, r, i == last);
			}
			break;
		}
		case Look.Capsules:
		{
			float cw = Math.Max(2f, barW * 0.8f);
			for (int k = 0; k < m; k++)
			{
				int i = _pIdx[k];
				ref Bar b = ref _pb[i];
				Color c = SideCol(ref b);
				bool up = b.IS >= 0.0;
				float y0 = (up ? (mid - half) : mid);
				g.DrawPath(Pen(Color.FromArgb(255, 44, 52, 66), 1f), Rounded(_path, _pfA[k].X - cw / 2f, y0, cw, half, cw / 2f));
				float fill = half * (float)Math.Min(1.0, Math.Abs(b.IS) / 100.0);
				if (fill >= cw * 0.6f)
				{
					float fy = (up ? (mid - fill) : mid);
					float cw2 = Math.Max(1f, cw - 2f);
					int a = StateAlpha(b.St);
					if (i == last)
					{
						g.DrawPath(Pen(Color.FromArgb(a, c), 1f), Rounded(_path, _pfA[k].X - cw2 / 2f, fy, cw2, fill, cw2 / 2f));
					}
					else
					{
						g.FillPath(Brush(Color.FromArgb(a, c)), Rounded(_path, _pfA[k].X - cw2 / 2f, fy, cw2, fill, cw2 / 2f));
					}
				}
				StateMark(g, ref b, _pfA[k].X, _pfA[k].Y, mid, r, i == last);
			}
			break;
		}
		default:
		{
			g.SmoothingMode = SmoothingMode.None;
			for (int k = 0; k < m; k++)
			{
				int i = _pIdx[k];
				ref Bar b = ref _pb[i];
				Color c = SideCol(ref b);
				int a = StateAlpha(b.St);
				float y = _pfA[k].Y;
				float y0 = Math.Min(y, mid);
				float hh = Math.Max(1f, Math.Abs(y - mid));
				float x0 = _pfA[k].X - barW / 2f;
				if (i == last)
				{
					g.DrawRectangle(Pen(Color.FromArgb(220, Bright(c, 0.35)), 1f), x0, y0, barW, hh);
				}
				else
				{
					g.FillRectangle(Brush(Color.FromArgb(a, c)), x0, y0, barW, hh);
					if (b.St == State.Absorbed)
					{
						g.DrawRectangle(Pen(Color.FromArgb(235, GoldColor), 1f), x0, y0, barW, hh);
					}
					else if (b.St == State.Real && Math.Abs(b.IS) >= RealFrom && hh >= 4f)
					{
						g.FillRectangle(Brush(Color.FromArgb(255, Bright(c, 0.6))), x0, (b.IS >= 0.0) ? y0 : (y0 + hh - 2f), barW, 2f);
					}
				}
			}
			g.SmoothingMode = SmoothingMode.AntiAlias;
			for (int k = 0; k < m; k++)
			{
				int i = _pIdx[k];
				StateMark(g, ref _pb[i], _pfA[k].X, _pfA[k].Y, mid, r, i == last);
			}
			break;
		}
		}
	}

	// the marks every look shares: the side flip (dot on the zero line), FADING (amber dot at half the bar), ABSORBED
	// (amber ring at the tip: trapped) and the stacked imbalance ticks (one small dash per stacked level, with the flow)
	private void StateMark(Graphics g, ref Bar b, float x, float y, float mid, float r, bool forming)
	{
		if (forming || !b.Real)
		{
			return;
		}
		if (b.Flip && Math.Abs(b.IS) >= RealFrom / 2.0)
		{
			// the side changed and the new side came in with some strength (a flip between two weak bars is noise)
			g.FillEllipse(Brush(Ink), x - 3f, mid - 3f, 6f, 6f);
			g.FillEllipse(Brush(Color.FromArgb(240, GoldColor)), x - 2f, mid - 2f, 4f, 4f);
		}
		if (b.St == State.Fading)
		{
			float fy = (y + mid) / 2f;
			g.FillEllipse(Brush(Ink), x - 3.5f, fy - 3.5f, 7f, 7f);
			g.FillEllipse(Brush(Color.FromArgb(245, Amber)), x - 2.5f, fy - 2.5f, 5f, 5f);
		}
		else if (b.St == State.Absorbed && Style != Look.Bars)
		{
			float ay = ((b.IS >= 0.0) ? (y - r - 2f) : (y + r + 2f));
			g.DrawEllipse(Pen(Color.FromArgb(240, GoldColor), 1.5f), x - r * 0.8f, ay - r * 0.8f, r * 1.6f, r * 1.6f);
		}
		if (ShowMarks && b.FpOk && b.KWith >= StackLevels && b.St == State.Real)
		{
			// a stacked imbalance with the flow: small dashes beyond the tip, one per stacked level (at most 4)
			int k = Math.Min(4, b.KWith);
			float d = ((b.IS >= 0.0) ? (-1f) : 1f);
			for (int j = 0; j < k; j++)
			{
				float yy = y + d * (3f + j * 3f);
				g.DrawLine(Pen(Color.FromArgb(230, Bright(SideCol(ref b), 0.5)), 1.4f), x - r * 0.6f, yy, x + r * 0.6f, yy);
			}
		}
	}

	private void DrawAccel(Graphics g, IChartWindowCoordinatesConverter conv, Rectangle rect, int vL, int vR, int count, int bw, float barW, float xR, float capH)
	{
		int last = count - 1;
		float top = _accTop + capH;
		float hgt = _accBot - top;
		float mid = top + hgt / 2f;
		float r = Math.Max(2.5f, Math.Min(5f, bw * 0.35f));
		float half = Math.Max(3f, hgt / 2f - 2f - (ShowMarks ? (r * 2f) : 0f));
		double vmax = 0.0;
		for (int i = vL; i <= vR; i++)
		{
			if (_pb[i].Has)
			{
				vmax = Math.Max(vmax, Math.Abs(_pb[i].AccN));
			}
		}
		// the scale fits the visible bars up to 6x; a bigger bar is cut at the lane's edge and gets a bright cap
		double scale = Math.Max(Math.Max(AccelExtreme, AccelMin) * 1.25, Math.Min(6.0, vmax * 1.04));
		g.DrawLine(Pen(Color.FromArgb(90, Grey), 1f), rect.Left, mid, xR, mid);
		float y1 = (float)Math.Min(half, half * AccelMin / scale);
		float y2 = (float)Math.Min(half, half * AccelExtreme / scale);
		Pen faint = Pen(Color.FromArgb(38, Grey), 1f);
		Pen solid = Pen(Color.FromArgb(70, Grey), 1f);
		g.DrawLine(faint, rect.Left, mid - y1, xR, mid - y1);
		g.DrawLine(faint, rect.Left, mid + y1, xR, mid + y1);
		g.DrawLine(solid, rect.Left, mid - y2, xR, mid - y2);
		g.DrawLine(solid, rect.Left, mid + y2, xR, mid + y2);
		for (int i = vL; i <= vR; i++)
		{
			ref Bar b = ref _pb[i];
			if (!b.Has)
			{
				continue;
			}
			float x = Xc(conv, i, bw);
			if (x > xR + bw)
			{
				break;
			}
			if (x < rect.Left - bw)
			{
				continue;
			}
			double v = Math.Abs(b.AccN);
			bool with = b.AccN >= 0.0;   // positive = the current side strengthening
			float h = (float)(half * v / scale);
			Color c = SideCol(ref b);
			bool big = v >= AccelExtreme;
			bool meaningful = v >= AccelMin;
			int a = (!meaningful ? 90 : (with ? (big ? 255 : 210) : 55));
			bool forming = i == last;
			if (forming)
			{
				a = a * 45 / 100;
			}
			bool cut = h > half + 0.5f;
			h = Math.Min(h, half);
			float yTop = (with ? (mid - h) : mid);
			float hh = Math.Max(1f, h);
			g.SmoothingMode = SmoothingMode.None;
			g.FillRectangle(Brush(Color.FromArgb(a, (with && big) ? Bright(c, 0.2) : c)), x - barW / 2f, yTop, barW, hh);
			if (cut)
			{
				g.FillRectangle(Brush(Color.FromArgb(255, Bright(c, 0.6))), x - barW / 2f, with ? (mid - half) : (mid + half - 2f), barW, 2f);
			}
			if (meaningful && !with && barW >= 3f)
			{
				g.DrawRectangle(Pen(Color.FromArgb(forming ? 140 : 235, c), 1f), x - barW / 2f, yTop, barW - 1f, hh);
			}
			g.SmoothingMode = SmoothingMode.AntiAlias;
			if (!ShowMarks || forming || !b.Real)
			{
				continue;
			}
			if (b.Flip && Math.Abs(b.IS) >= RealFrom / 2.0)
			{
				g.FillEllipse(Brush(Ink), x - 3f, mid - 3f, 6f, 6f);
				g.FillEllipse(Brush(Color.FromArgb(240, c)), x - 2f, mid - 2f, 4f, 4f);
			}
			if (big)
			{
				float end = (with ? (mid - h) : (mid + h));
				float y = (with ? (end - r - 2f) : (end + r + 2f));
				y = Math.Max(top + r, Math.Min(_accBot - r, y));
				g.FillEllipse(Brush(Color.FromArgb(220, 8, 10, 14)), x - r - 1.5f, y - r - 1.5f, 2f * r + 3f, 2f * r + 3f);
				if (with)
				{
					// THRUST: a chevron the way of the side; grey when the bar's verdict is not REAL (a thrust into a thin or absorbed bar)
					float d = ((b.IS >= 0.0) ? (-1f) : 1f);
					Pen p = ((b.St == State.Real || b.St == State.Fading) ? Pen(Color.FromArgb(250, Bright(c, 0.2)), 1.8f) : Pen(Color.FromArgb(175, Dim), 1.4f));
					g.DrawLine(p, x - r * 0.75f, y - d * r * 0.3f, x, y + d * r * 0.45f);
					g.DrawLine(p, x, y + d * r * 0.45f, x + r * 0.75f, y - d * r * 0.3f);
				}
				else
				{
					g.DrawEllipse(Pen(Color.FromArgb(240, Amber), 1.5f), x - r * 0.8f, y - r * 0.8f, r * 1.6f, r * 1.6f);
					g.FillEllipse(Brush(Amber), x - 1.5f, y - 1.5f, 3f, 3f);
				}
			}
		}
		if (ShowLaneText)
		{
			int read = ((LiveReadout || count < 2) ? last : (last - 1));
			ref Bar rb = ref _pb[read];
			Axis(g, xR, top, _accBot, mid, half, scale, rb.AccN, SideCol(ref rb), true);
			g.DrawString("ACCELERATION   ▲ THRUST · ○ STALL · ● flip", _fb, Brush(Color.FromArgb(200, Grey)), rect.Left + 8, _accTop + 1f);
			Color wc;
			string words = AccelWords(ref rb, out wc);
			g.DrawString(words, _fb, Brush(wc), xR - 4f, _accTop + 1f, RightFmt);
		}
	}

	private static readonly StringFormat RightFmt = new StringFormat
	{
		Alignment = StringAlignment.Far,
		LineAlignment = StringAlignment.Near
	};

	private void Axis(Graphics g, float xR, float laneTop, float laneBot, float yMid, float half, double scale, double value, Color col, bool typical)
	{
		SolidBrush brush = Brush(Color.FromArgb(170, Grey));
		string s1 = (typical ? ("+" + scale.ToString("0.0", CultureInfo.InvariantCulture) + "x") : "+100");
		string s2 = (typical ? ("-" + scale.ToString("0.0", CultureInfo.InvariantCulture) + "x") : "-100");
		g.DrawLine(Pen(Color.FromArgb(50, Grey), 1f), xR, laneTop, xR, laneBot);
		float capH = _lineHs + 1f;
		if (yMid - half >= laneTop - 1f)
		{
			g.DrawString(s1, _fs, brush, xR + 3f, yMid - half - capH / 2f);
		}
		g.DrawString("0", _fs, brush, xR + 3f, yMid - capH / 2f);
		if (yMid + half <= laneBot + 1f)
		{
			g.DrawString(s2, _fs, brush, xR + 3f, yMid + half - capH / 2f);
		}
		float val = yMid - (float)(half * value / scale);
		val = Math.Max(laneTop + capH / 2f, Math.Min(laneBot - capH / 2f, val));
		string s3 = (typical ? (Math.Abs(value).ToString("0.0", CultureInfo.InvariantCulture) + "x") : ((value >= 0.0 ? "+" : "") + value.ToString("0", CultureInfo.InvariantCulture)));
		float aw = _fs.Size * 5.6f + 8f;
		g.FillRectangle(Brush(Color.FromArgb(230, col)), xR + 1f, val - capH / 2f, aw - 2f, capH);
		g.DrawString(s3, _fs, Brush(Ink), xR + 3f, val - capH / 2f);
	}

	// ---------------- words ----------------

	private static string StateWord(State s, bool trapped, bool wall)
	{
		return s switch
		{
			State.Real => "REAL",
			State.Thin => "THIN",
			State.Battle => "BATTLE",
			State.Absorbed => (trapped ? "ABSORBED · trapped" : (wall ? "ABSORBED · wall" : "ABSORBED")),
			State.Fading => "FADING",
			_ => "no data",
		};
	}

	private Color StateColor(State s, Color side)
	{
		return s switch
		{
			State.Real => side,
			State.Fading => Amber,
			State.Battle => Amber,
			State.Absorbed => GoldColor,
			State.Thin => Amber,
			_ => Dim,
		};
	}

	private string AccelWords(ref Bar b, out Color col)
	{
		double n = Math.Abs(b.AccN);
		bool with = b.AccN >= 0.0;
		Color side = SideCol(ref b);
		string who = ((b.IS >= 0.0) ? "Buyers " : "Sellers ");
		string x = "  " + n.ToString("0.0", CultureInfo.InvariantCulture) + "×";
		if (!b.Real)
		{
			col = Dim;
			return "no data";
		}
		if (n < AccelMin)
		{
			col = Color.FromArgb(170, side);
			return who + "steady" + x;
		}
		if (!with)
		{
			col = side;
			return who + ((n >= AccelExtreme) ? "STALL" : "slowing") + x;
		}
		if (n >= AccelExtreme && b.St != State.Real && b.St != State.Fading)
		{
			col = Color.FromArgb(200, side);
			return who + "thin thrust" + x;
		}
		col = side;
		return who + ((n >= AccelExtreme) ? "THRUST" : "speeding up") + x;
	}

	private static string Pct(double r)
	{
		return (r < 0.0) ? "–" : ((r * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%");
	}

	private static string Fmt(double v)
	{
		double a = Math.Abs(v);
		if (a >= 1000000.0)
		{
			return (v / 1000000.0).ToString("0.##", CultureInfo.InvariantCulture) + "M";
		}
		if (a >= 10000.0)
		{
			return (v / 1000.0).ToString("0.#", CultureInfo.InvariantCulture) + "K";
		}
		return v.ToString("0", CultureInfo.InvariantCulture);
	}

	// ---------------- card ----------------

	private readonly List<(string Label, string Text, Color Col, int Kind, double V)> _rows = new List<(string, string, Color, int, double)>(16);

	private void DrawCard(Graphics g, Rectangle rect, int read)
	{
		if (read < 0 || read >= _pn || !_pb[read].Has)
		{
			return;
		}
		ref Bar b = ref _pb[read];
		Color side = SideCol(ref b);
		Color dim = Dim;
		bool compact = CardRows == CardRowsMode.Compact;
		var rows = _rows;
		rows.Clear();
		// 0 text, 1 meter (-1..1 of V), 2 pill, 3 gap, 4 hero
		string st = StateWord(b.St, b.Trapped, b.Wall);
		Color stc = StateColor(b.St, side);
		rows.Add(("", (b.IS >= 0.0 ? "+" : "") + b.IS.ToString("0", CultureInfo.InvariantCulture), side, 4, b.IS));
		rows.Add(("Verdict", st + (b.Closed ? "" : " · forming"), stc, 2, 0.0));
		rows.Add(("", "", dim, 3, 0.0));
		rows.Add(("Who", ((b.Side >= 0) ? "Buyers " : "Sellers ") + (b.Omega * 100.0).ToString("0", CultureInfo.InvariantCulture) + "% one-sided · rank " + Pct(b.ROmega), side, 1, (double)b.Side * b.Omega));
		rows.Add(("Busy", Fmt(b.Vps) + "/s · rank " + Pct(b.RPi), (b.RPi >= 0.5) ? TextCol : dim, 0, 0.0));
		rows.Add(("Effort", "Δ " + Fmt(b.Delta) + " · rank " + Pct(b.REff), side, 0, 0.0));
		if (!compact)
		{
			rows.Add(("Depth taken", ((_agg == Agg.Range) ? "volume " : "volume / tick ") + "rank " + Pct(b.RZeta) + ((b.RZeta >= 0.0 && b.RZeta < ThinRank) ? "  THIN" : ""), (b.RZeta >= 0.0 && b.RZeta < ThinRank) ? Amber : TextCol, 0, 0.0));
			rows.Add(("Inside the bar", "given back " + (b.G * 100.0).ToString("0", CultureInfo.InvariantCulture) + "% · wick " + (b.W * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%" + ((b.G >= ReverseFrac || b.W >= ReverseFrac) ? "  REVERSED" : ""), (b.G >= ReverseFrac || b.W >= ReverseFrac) ? Amber : TextCol, 0, 0.0));
			rows.Add(("Persistence", "run " + b.Run.ToString(CultureInfo.InvariantCulture) + " · " + (((1.0 + b.Psi) / 2.0) * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%", TextCol, 0, 0.0));
		}
		string fp;
		Color fpc = TextCol;
		if (!UseFootprint)
		{
			fp = "off";
			fpc = dim;
		}
		else if (!b.FpOk)
		{
			fp = "no levels";
			fpc = dim;
		}
		else
		{
			fp = b.KWith.ToString(CultureInfo.InvariantCulture) + " with · " + b.KAgainst.ToString(CultureInfo.InvariantCulture) + " against · " + (b.ShWith * 100.0).ToString("0", CultureInfo.InvariantCulture) + "% of volume" + (b.UaRejected ? " · unfinished" : "");
			if (b.KWith >= StackLevels)
			{
				fpc = side;
			}
			else if (b.KAgainst >= StackLevels)
			{
				fpc = Amber;
			}
		}
		rows.Add(("Stacks", fp, fpc, 0, 0.0));
		if (!compact)
		{
			rows.Add(("4 bars", "flow " + Fmt(b.J4) + " · moved " + b.Move4.ToString("0", CultureInfo.InvariantCulture) + " ticks" + (b.Wall ? "  WALL" : ""), b.Wall ? GoldColor : TextCol, 0, 0.0));
		}
		Color ac;
		string aw = AccelWords(ref b, out ac);
		rows.Add(("Speed", aw, ac, 0, 0.0));
		rows.Add(("", "", dim, 3, 0.0));
		// the last finished bar's response and the hit rate
		int prev = read - 1;
		if (prev >= 0 && _pb[prev].Has)
		{
			ref Bar p = ref _pb[prev];
			string resp = ((p.Kappa > 0) ? "confirmed ✓" : ((p.Kappa < 0) ? "rejected ✗" : "pending"));
			rows.Add(("Last bar", StateWord(p.St, p.Trapped, p.Wall) + " " + (p.IS >= 0.0 ? "+" : "") + p.IS.ToString("0", CultureInfo.InvariantCulture) + " · " + resp, (p.Kappa > 0) ? BullColor : ((p.Kappa < 0) ? BearColor : dim), 0, 0.0));
		}
		rows.Add(("REAL hit rate", (_pHit < 0.0) ? "needs 10 REAL bars" : (Pct(_pHit) + " followed through (last " + Lookback.ToString(CultureInfo.InvariantCulture) + ")"), (_pHit < 0.0) ? dim : TextCol, 0, 0.0));
		// layout
		float k = Math.Max(0.6f, Math.Min(1.6f, CardSize / 100f));
		float pad = 8f * k;
		float rowH = _lineH + 3f * k;
		float labelW = 0f;
		float valW = 0f;
		for (int i = 0; i < rows.Count; i++)
		{
			if (rows[i].Kind == 3)
			{
				continue;
			}
			labelW = Math.Max(labelW, TextW(g, rows[i].Label, _f));
			if (rows[i].Kind == 4)
			{
				valW = Math.Max(valW, TextW(g, rows[i].Text, _fh) + 70f * k + TextW(g, st, _fb));
			}
			else
			{
				valW = Math.Max(valW, TextW(g, rows[i].Text, _fb) + ((rows[i].Kind == 1) ? (56f * k) : 0f) + ((rows[i].Kind == 2) ? (14f * k) : 0f));
			}
		}
		float width = Math.Min((float)rect.Width - 12f, pad + labelW + 10f * k + valW + pad);
		float height = pad + rowH + 3f * k;   // title
		for (int i = 0; i < rows.Count; i++)
		{
			height += ((rows[i].Kind == 3) ? (5f * k) : ((rows[i].Kind == 4) ? (_lineHh + 3f * k) : rowH));
		}
		height += pad - 2f * k;
		bool left = CardPlace == Corner.TopLeft || CardPlace == Corner.BottomLeft;
		bool topC = CardPlace == Corner.TopLeft || CardPlace == Corner.TopRight;
		float axisW = (ShowLaneText ? (_fs.Size * 5.6f + 8f) : 0f);
		float x = (left ? ((float)rect.Left + 6f) : ((float)rect.Right - width - axisW - 10f));
		float y = (topC ? ((float)rect.Top + 28f) : ((float)rect.Bottom - height - 6f));
		y = Math.Max(rect.Top + 2, Math.Min((float)rect.Bottom - height, y));
		x = Math.Max(rect.Left, Math.Min((float)rect.Right - width, x));
		int alpha = Math.Max(0, Math.Min(255, CardOpacity * 255 / 100));
		GraphicsPath bg = Rounded(_path, x, y, width, height, 8f * k);
		g.FillPath(Brush(Color.FromArgb(alpha, 4, 5, 8)), bg);
		g.DrawPath(Pen(Color.FromArgb(255, 36, 41, 50), 1f), bg);
		GraphicsState gs = g.Save();
		g.SetClip(new RectangleF(x, y, width, height), CombineMode.Intersect);
		g.FillRectangle(Brush(Color.FromArgb(230, side)), x + 1.5f, y + 9f * k, 2.5f * k, Math.Max(1f, height - 18f * k));
		float hx = x + pad;
		float hr = x + width - pad;
		float cy = y + pad - 2f * k;
		// title and badge
		string badge = ((!b.Real && !VaReady()) ? "LOADING" : (b.Closed ? "CLOSED" : "LIVE"));
		Color bc = ((badge == "LOADING") ? Amber : BullColor);
		float bw2 = TextW(g, badge, _fs) + 18f * k;
		float bx = hr - bw2;
		g.FillPath(Brush(Color.FromArgb(46, bc)), Rounded(_path, bx, cy + 1f * k, bw2, _lineH - 2f * k, (_lineH - 2f * k) / 2f));
		g.FillEllipse(Brush(bc), bx + 6f * k, cy + _lineH / 2f - 2.5f * k, 5f * k, 5f * k);
		g.DrawString(badge, _fs, Brush(bc), bx + 13f * k, cy + (_lineH - _lineHs) / 2f, NoWrap);
		long errs = Interlocked.Read(ref _errors);
		float limit = bx - 6f * k;
		if (errs > 0)
		{
			string et = "⚠ " + errs.ToString(CultureInfo.InvariantCulture) + " errors";
			float ew = TextW(g, et, _fs) + 12f * k;
			float ex = limit - ew;
			if (ex > hx + 40f * k)
			{
				g.FillPath(Brush(Color.FromArgb(60, Amber)), Rounded(_path, ex, cy + 1f * k, ew, _lineH - 2f * k, 4f * k));
				g.DrawString(et, _fs, Brush(Amber), ex + 6f * k, cy + (_lineH - _lineHs) / 2f, NoWrap);
				limit = ex - 6f * k;
			}
		}
		CellText(g, "IMBALANCE STRENGTH", _fb, TextCol, hx, cy, limit, _lineH);
		float tagX = hx + TextW(g, "IMBALANCE STRENGTH", _fb) + 10f * k;
		float tw = TextW(g, _chartTag, _fs) + 12f * k;
		if (_chartTag.Length > 0 && tagX + tw < limit)
		{
			g.FillPath(Brush(Color.FromArgb(255, 20, 24, 32)), Rounded(_path, tagX, cy + 1f * k, tw, _lineH - 2f * k, 4f * k));
			g.DrawString(_chartTag, _fs, Brush(dim), tagX + 6f * k, cy + (_lineH - _lineHs) / 2f, NoWrap);
		}
		cy += rowH + 3f * k;
		Pen line = Pen(Color.FromArgb(255, 30, 34, 42), 1f);
		g.DrawLine(line, hx, cy - 3f * k, hr, cy - 3f * k);
		float vx = hx + labelW + 10f * k;
		for (int i = 0; i < rows.Count; i++)
		{
			var row = rows[i];
			switch (row.Kind)
			{
			case 3:
				g.DrawLine(line, hx, cy + 2.5f * k, hr, cy + 2.5f * k);
				cy += 5f * k;
				continue;
			case 4:
			{
				// the strength number, the state pill beside it
				CellText(g, row.Text, _fh, row.Col, hx - 1f * k, cy, hr, _lineHh);
				float nw = TextW(g, row.Text, _fh) + 10f * k;
				float pw = TextW(g, st, _fb) + 14f * k;
				float ph = _lineH + 2f * k;
				float py = cy + (_lineHh - ph) / 2f + 1.5f * k;
				if (hx + nw + pw <= hr)
				{
					g.FillPath(Brush(Color.FromArgb(40, stc)), Rounded(_path, hx + nw, py, pw, ph, ph / 2f));
					CellText(g, st, _fb, stc, hx + nw + 7f * k, py + (ph - _lineH) / 2f, hr, _lineH);
				}
				cy += _lineHh + 3f * k;
				continue;
			}
			}
			float ty = cy + (rowH - _lineH) / 2f;
			CellText(g, row.Label, _f, dim, hx, ty, vx - 6f * k, _lineH);
			if (row.Kind == 1)
			{
				float mw = Math.Min(48f * k, Math.Max(0f, hr - vx) * 0.4f);
				float my = cy + rowH / 2f - 2.5f * k;
				float hf = mw / 2f;
				float frac = (float)Math.Min(1.0, Math.Abs(row.V));
				g.FillRectangle(Brush(Color.FromArgb(255, 26, 30, 38)), vx, my, mw, 5f * k);
				float fw = hf * frac;
				if (fw > 0f)
				{
					g.FillRectangle(Brush(row.Col), (row.V >= 0.0) ? (vx + hf) : (vx + hf - fw), my, fw, 5f * k);
				}
				g.DrawLine(Pen(Color.FromArgb(150, 210, 214, 224), 1f), vx + hf, my - 2f * k, vx + hf, my + 7f * k);
				CellText(g, row.Text, _fb, row.Col, vx + mw + 8f * k, ty, hr, _lineH);
			}
			else if (row.Kind == 2)
			{
				float cw = Math.Min(TextW(g, row.Text, _fb) + 14f * k, hr - vx + 6f * k);
				if (cw >= 12f)
				{
					g.FillPath(Brush(Color.FromArgb(38, row.Col)), Rounded(_path, vx - 6f * k, cy + 1.5f * k, cw, rowH - 3f * k, 5f * k));
				}
				CellText(g, row.Text, _fb, row.Col, vx + 1f * k, ty, hr - 2f * k, _lineH);
			}
			else
			{
				CellText(g, row.Text, _fb, row.Col, vx, ty, hr, _lineH);
			}
			cy += rowH;
		}
		g.Restore(gs);
	}

	private bool VaReady()
	{
		try
		{
			return _vaLoaded || base.HistoricalData?.VolumeAnalysisCalculationProgress?.State == VolumeAnalysisCalculationState.Finished;
		}
		catch
		{
			return false;
		}
	}

	// ---------------- scale ----------------

	protected override bool OnTryGetMinMax(int fromOffset, int toOffset, out double min, out double max)
	{
		// the window shows -100..+100 for the strength lane; the acceleration lane takes its share below
		max = 100.0;
		min = -100.0;
		double f = _reserveFrac;
		if (f > 0.0 && f < 0.9)
		{
			min -= 200.0 * f / (1.0 - f);
		}
		max += 8.0;
		return true;
	}

	// ---------------- GDI helpers ----------------

	private void Fonts()
	{
		if (_f != null)
		{
			return;
		}
		float sz = (float)Math.Max(6.0, FontSize);
		_textW.Clear();
		_f = new Font("Segoe UI", sz);
		_fb = new Font("Segoe UI Semibold", sz);
		_fs = new Font("Segoe UI", Math.Max(6f, sz - 1.5f));
		_fh = new Font("Segoe UI", sz + 4f, FontStyle.Bold);
		_lineH = _f.GetHeight();
		_lineHs = _fs.GetHeight();
		_lineHh = _fh.GetHeight();
	}

	private void DisposeGdi()
	{
		_f?.Dispose();
		_fb?.Dispose();
		_fs?.Dispose();
		_fh?.Dispose();
		_f = (_fb = (_fs = (_fh = null)));
		foreach (Pen p in _pens.Values)
		{
			p.Dispose();
		}
		_pens.Clear();
		foreach (Pen p in _dashPens.Values)
		{
			p.Dispose();
		}
		_dashPens.Clear();
		foreach (SolidBrush br in _brushes.Values)
		{
			br.Dispose();
		}
		_brushes.Clear();
		_textW.Clear();
	}

	protected override void OnClear()
	{
		try
		{
			lock (_paintSync)
			{
				DisposeGdi();
			}
		}
		catch (Exception ex)
		{
			Report("OnClear", ex);
		}
	}

	public override void Dispose()
	{
		try
		{
			lock (_paintSync)
			{
				DisposeGdi();
			}
		}
		catch (Exception ex)
		{
			Report("Dispose", ex);
		}
		base.Dispose();
	}

	protected override void OnSettingsUpdated()
	{
		try
		{
			base.OnSettingsUpdated();
			lock (_paintSync)
			{
				lock (_sync)
				{
					DisposeGdi();
					_recalcAsked = true;   // thresholds and windows changed: the history is read again at the next update
				}
			}
		}
		catch (Exception ex)
		{
			Report("OnSettingsUpdated", ex);
		}
		Redraw();
	}

	private static long PenKey(Color c, float w)
	{
		return ((long)c.ToArgb() << 32) | (uint)BitConverter.SingleToInt32Bits(w);
	}

	private Pen Pen(Color c, float w)
	{
		long key = PenKey(c, w);
		if (!_pens.TryGetValue(key, out Pen p))
		{
			p = new Pen(c, w);
			_pens[key] = p;
		}
		return p;
	}

	private Pen DashPen(Color c, float w)
	{
		long key = PenKey(c, w);
		if (!_dashPens.TryGetValue(key, out Pen p))
		{
			p = new Pen(c, w);
			p.DashStyle = DashStyle.Dash;
			_dashPens[key] = p;
		}
		return p;
	}

	private SolidBrush Brush(Color c)
	{
		int key = c.ToArgb();
		if (!_brushes.TryGetValue(key, out SolidBrush b))
		{
			b = new SolidBrush(c);
			_brushes[key] = b;
		}
		return b;
	}

	private float TextW(Graphics g, string text, Font font)
	{
		if (string.IsNullOrEmpty(text))
		{
			return 0f;
		}
		int fi = ReferenceEquals(font, _f) ? 0 : (ReferenceEquals(font, _fb) ? 1 : (ReferenceEquals(font, _fs) ? 2 : (ReferenceEquals(font, _fh) ? 3 : -1)));
		if (fi < 0)
		{
			return g.MeasureString(text, font, PointF.Empty, NoWrap).Width;
		}
		if (g.DpiX != _textWDpi)
		{
			_textW.Clear();
			_textWDpi = g.DpiX;
		}
		if (_textW.TryGetValue((text, fi), out float w))
		{
			return w;
		}
		if (_textW.Count > 2048)
		{
			_textW.Clear();
		}
		w = g.MeasureString(text, font, PointF.Empty, NoWrap).Width;
		_textW[(text, fi)] = w;
		return w;
	}

	private void CellText(Graphics g, string s, Font font, Color col, float x, float y, float right, float h)
	{
		if (string.IsNullOrEmpty(s) || right - x < 4f)
		{
			return;
		}
		g.DrawString(s, font, Brush(col), new RectangleF(x, y, right - x, h + 2f), Cell);
	}

	private static GraphicsPath Rounded(GraphicsPath path, float x, float y, float w, float h, float r)
	{
		path.Reset();
		w = Math.Max(1f, w);
		h = Math.Max(1f, h);
		r = Math.Max(0.5f, Math.Min(r, Math.Min(w, h) / 2f));
		path.AddArc(x, y, 2f * r, 2f * r, 180f, 90f);
		path.AddArc(x + w - 2f * r, y, 2f * r, 2f * r, 270f, 90f);
		path.AddArc(x + w - 2f * r, y + h - 2f * r, 2f * r, 2f * r, 0f, 90f);
		path.AddArc(x, y + h - 2f * r, 2f * r, 2f * r, 90f, 90f);
		path.CloseFigure();
		return path;
	}

	private static Color Bright(Color c, double k)
	{
		return Color.FromArgb(c.A, (int)Math.Min(255.0, (double)(int)c.R + (double)(255 - c.R) * k), (int)Math.Min(255.0, (double)(int)c.G + (double)(255 - c.G) * k), (int)Math.Min(255.0, (double)(int)c.B + (double)(255 - c.B) * k));
	}
}
