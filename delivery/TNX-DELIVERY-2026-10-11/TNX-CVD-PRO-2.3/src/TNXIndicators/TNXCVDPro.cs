using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Chart;
using TradingPlatform.BusinessLayer.Native;
using TradingPlatform.BusinessLayer.Utils;

namespace TNXIndicators;

public class TNXCVDPro : Indicator, IVolumeAnalysisIndicator
{
	public enum Display
	{
		Candles,
		Line,
		DeltaBars,
		Velocity
	}

	public enum VelocityCvdStyle
	{
		Candles,
		Line
	}

	public enum AccelStyle
	{
		Strength,
		Raw
	}

	public enum ResetMode
	{
		Continuous,
		Day,
		Week,
		Session,
		Bars
	}

	public enum SignalFilter
	{
		All,
		AboveAverage,
		Extreme
	}

	public enum Corner
	{
		TopRight,
		BottomRight,
		BottomLeft,
		Hidden
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

	public enum BarStyle
	{
		Solid,
		Gradient,
		Outline
	}

	public enum TableLook
	{
		Card,
		Classic
	}

	public enum FlowTierMode
	{
		Percentile,
		Classic
	}

	public enum MarkerLook
	{
		Clean,
		Classic,
		Badge
	}

	public enum CardPlace
	{
		InfoTable,
		TopRight,
		TopLeft,
		BottomRight,
		BottomLeft,
		Auto
	}

	public enum CardRows
	{
		Full,
		Compact
	}

	public enum LaneLook
	{
		Pro,
		Classic
	}

	public enum FlowLook
	{
		Bars,
		Area,
		Histogram,
		Ribbon,
		Needles
	}

	public enum FlowShade
	{
		RealPush,
		Classic
	}

	internal enum SigKind
	{
		Absorption,
		Exhaustion,
		Divergence
	}

	private struct Bar
	{
		public double Delta;

		public double MinD;

		public double MaxD;

		public double Buy;

		public double Sell;

		public double Vol;

		public double O;

		public double H;

		public double L;

		public double C;

		public double Px;

		public double PO;

		public double PH;

		public double PL;

		public long Ticks;

		public bool Real;

		public bool Has;

		public double PAbsDp;

		public double PAbsDc;

		public double PBuy;

		public double PSell;

		public double PVol;

		public double PRej;

		public double PDelta;

		public double R;

		public double E;

		public int DivRun;

		public double Fast;

		public double Slow;

		public double AccE;

		public double PAbsAcc;

		public bool Flip;

		public double F;

		public int Seg;

		public long Anchor;

		public int SessIdx;   // 2.0 (F5): bars since the session's first bar (only kept in "every N bars" mode)

		// ---- 2.3: the real-push dials, computed once per bar in Compute and only read by the picture ----
		public double Dur;        // seconds the bar took (finished: next open - this open; forming: now - open), floored at 0.1

		public double PDur;       // prefix sum of Dur

		public double Imb7;       // one-sidedness of the fast window: (buy - sell) / (buy + sell), -1 .. 1

		public double Imb3;       // the same over the last 3 bars

		public double Vps;        // volume per second over the fast window

		public double FRank;      // rank of |Fast| among the lookback's bars (0 .. 1); -1 while fewer than 20 bars

		public double SRank;      // the same for |Slow|

		public double BusyRank;   // rank of Vps (how busy the market is); -1 while fewer than 20 bars

		public double ImbRank;    // rank of |Imb7| (how one-sided); -1 while fewer than 20 bars

		public double Q;          // real push 0 .. 1 = sqrt(busy rank x one-sided rank); 0.5 while the ranks are unknown

		public bool Absorbed;     // the bar's own reading is ABSORBED or TRAPPED (Verdict's rule: effort >= 1x, result < 0.35 of it)

		public bool Fade;         // flow still strong, but its one-sidedness over 3 bars fell under 60 % of the 7-bar one: the push is dying

		public bool ThrustOk;     // a THRUST here is backed by a busy (rank >= 0.5), one-sided, not fading, not absorbed market
	}

	private sealed class Sig
	{
		public int Abs;

		public SigKind Kind;

		public bool Bull;

		public double Score;

		public double R;

		public double E;

		public bool Pending;

		public string Note;

		public int P1 = -1;

		public int P2 = -1;

		public double V1;

		public double V2;

		// 2.0 (F2): the last bar the same event fired on (= Abs until it fires again within SignalGap bars)
		public int Seen = -1;

		// 2.0 (F2): the signal filter's answer, given when the signal is known and never changed afterwards. Avg0 = the
		// average score of its kind in the LookbackBars before it, as known then (-1: classic, the filter is asked again at
		// every picture with the scores as they are now); AtAvg / AtExt = the bar on which its strongest reading first
		// reached that average / 1.5 x that average (-1: not yet)
		public double Avg0 = -1.0;

		public int AtAvg = -1;

		public int AtExt = -1;

		// 2.0 (F2): the same event fired again: a new object (the picture's copy shares the old one), same place, the
		// strongest score, the last bar it fired on, and the bar on which it first reached its filter's level
		public Sig Again(double score, int seen)
		{
			Sig sig = new Sig
			{
				Abs = Abs, Kind = Kind, Bull = Bull, Score = Math.Max(Score, score), R = R, E = E, Pending = Pending, Note = Note,
				P1 = P1, P2 = P2, V1 = V1, V2 = V2, Seen = Math.Max(Seen, seen), Avg0 = Avg0, AtAvg = AtAvg, AtExt = AtExt
			};
			if (Avg0 >= 0.0)
			{
				if (sig.AtAvg < 0 && sig.Score >= Avg0)
				{
					sig.AtAvg = seen;
				}
				if (sig.AtExt < 0 && sig.Score >= 1.5 * Avg0)
				{
					sig.AtExt = seen;
				}
			}
			return sig;
		}
	}

	private enum Agg
	{
		Other,
		Range,
		Tick,
		Time
	}

	private enum Pf
	{
		AbsDp,
		AbsDc,
		Buy,
		Sell,
		Vol,
		Rej,
		Delta,
		AbsAcc,
		Dur
	}

	[InputParameter("Display", 0, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Delta candles (CVD OHLC)",
		Display.Candles,
		"Line",
		Display.Line,
		"Per-bar delta bars",
		Display.DeltaBars,
		"Velocity (flow speed panel + CVD)",
		Display.Velocity
	})]
	public Display Mode;

	[InputParameter("Reset", 1, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Continuous",
		ResetMode.Continuous,
		"Every day (chart time zone)",
		ResetMode.Day,
		"Every week (chart time zone)",
		ResetMode.Week,
		"Every trading session",
		ResetMode.Session,
		"Every N bars",
		ResetMode.Bars
	})]
	public ResetMode Reset = ResetMode.Session;

	[InputParameter("Reset every N bars", 2, 2.0, 100000.0, 1.0, 0, null)]
	public int ResetBars = 500;

	[InputParameter("Theme", 3, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Pro dark",
		Theme.ProDark,
		"Cyberpunk (neon gold/pink)",
		Theme.Cyberpunk,
		"Dracula (emerald/coral/purple)",
		Theme.Dracula,
		"Nordic (arctic teal/crimson)",
		Theme.Nordic,
		"Midnight Gold",
		Theme.MidnightGold,
		"Solarized",
		Theme.Solarized,
		"Matrix (digital green)",
		Theme.Matrix,
		"ATAS X",
		Theme.AtasX,
		"DeepCharts",
		Theme.DeepCharts,
		"Bookmap",
		Theme.Bookmap,
		"TradingView",
		Theme.TradingView,
		"Neon",
		Theme.Neon,
		"Classic",
		Theme.Classic,
		"Ocean",
		Theme.Ocean,
		"Monochrome",
		Theme.Monochrome,
		"Ember",
		Theme.Ember,
		"Ice",
		Theme.Ice,
		"Aurora",
		Theme.Aurora,
		"Terminal",
		Theme.Terminal,
		"Sunset",
		Theme.Sunset,
		"High contrast (colour-blind safe)",
		Theme.HighContrast,
		"Custom (colours below)",
		Theme.Custom
	})]
	public Theme Skin;

	[InputParameter("Custom bull candle colour", 4, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color CustomBull = Color.FromArgb(38, 198, 145);

	[InputParameter("Custom bear candle colour", 5, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color CustomBear = Color.FromArgb(235, 64, 92);

	[InputParameter("Line / candle width", 6, 1.0, 6.0, 1.0, 0, null)]
	public int LineWidth = 2;

	[InputParameter("Velocity: top CVD style", 13, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Candles",
		VelocityCvdStyle.Candles,
		"Line",
		VelocityCvdStyle.Line
	})]
	public VelocityCvdStyle VelocityStyle;

	[InputParameter("Velocity: period (fast = period/3, slow = period)", 7, 3.0, 500.0, 1.0, 0, null)]
	public int VelocityPeriod = 21;

	[InputParameter("Velocity: panel height (% of window)", 8, 10.0, 80.0, 1.0, 0, null)]
	public int VelocityPanelHeight = 34;

	[InputParameter("Velocity: use theme colours", 9, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool VelocityUseTheme = true;

	[InputParameter("Velocity: rising flow", 10, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color VelocityUp = Color.FromArgb(38, 198, 145);

	[InputParameter("Velocity: falling flow", 11, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color VelocityDown = Color.FromArgb(235, 64, 92);

	[InputParameter("Velocity: smoothed average line", 12, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color VelocityAvg = Color.FromArgb(255, 255, 255);

	[InputParameter("Velocity: CVD line on top", 13, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color VelocityCvd = Color.FromArgb(235, 240, 250);

	[InputParameter("Velocity: bar style", 64, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Solid",
		BarStyle.Solid,
		"Gradient (lit top)",
		BarStyle.Gradient,
		"Outline",
		BarStyle.Outline
	})]
	public BarStyle Bars;

	[InputParameter("Velocity: lane background", 65, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color LaneBack = Color.Black;

	[InputParameter("Flow speed: fixed scale (0 = auto fit visible bars)", 67, 0.0, 100000000.0, 1.0, 0, null)]
	public double FlowScale;

	[InputParameter("Acceleration: fixed scale in x typical (0 = auto from thresholds)", 68, 0.0, 50.0, 0.5, 1, null)]
	public double AccelScale;

	[InputParameter("Lanes: mouse wheel = zoom, drag = move, double-click = reset", 69, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool LaneMouse = true;

	[InputParameter("Velocity: show lane captions / readouts", 66, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowLaneText = true;

	[InputParameter("Acceleration: show strip under velocity", 14, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowAccel = true;

	[InputParameter("Acceleration: bar meaning", 15, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Up = side getting stronger, down = side fading",
		AccelStyle.Strength,
		"Up = buy-side acceleration, down = sell-side",
		AccelStyle.Raw
	})]
	public AccelStyle AccelMode;

	[InputParameter("Acceleration: strip height (% of window)", 16, 8.0, 50.0, 1.0, 0, null)]
	public int AccelHeight = 18;

	[InputParameter("Acceleration: lag (bars) - speed now vs speed N bars ago", 17, 1.0, 10.0, 1.0, 0, null)]
	public int AccelLag = 1;

	[InputParameter("Acceleration: smoothing (EMA bars, 1 = off)", 18, 1.0, 20.0, 1.0, 0, null)]
	public int AccelSmooth = 3;

	[InputParameter("Acceleration: meaningful >= x typical (solid bars)", 60, 0.2, 5.0, 0.1, 1, null)]
	public double AccelMin = 1.0;

	[InputParameter("Acceleration: extreme >= x typical (thrust / stall markers)", 61, 0.5, 10.0, 0.1, 1, null)]
	public double AccelExtreme = 2.0;

	[InputParameter("Acceleration: show thrust / stall / side-flip markers", 62, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowAccelMarkers = true;

	[InputParameter("Acceleration: show live reading text", 63, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowAccelText = true;

	[InputParameter("Lookback (bars) – scales, extremes, signal average", 20, 20.0, 5000.0, 5.0, 0, null)]
	public int LookbackBars = 60;

	[InputParameter("Dominance lookback (bars) – table 'Dominant'", 19, 20.0, 5000.0, 10.0, 0, null)]
	public int DominanceBars = 40;

	[InputParameter("Velocity window (bars) for R / E", 21, 1.0, 50.0, 1.0, 0, null)]
	public int VelocityBars = 3;

	[InputParameter("Divergence: both velocities ≥", 22, 0.3, 10.0, 0.1, 1, null)]
	public double DivergeMin = 1.0;

	[InputParameter("Divergence: persistence (consecutive bars)", 23, 1.0, 10.0, 1.0, 0, null)]
	public int DivergePersist = 2;

	[InputParameter("Exhaustion: push delta ≤ % of previous push", 24, 10.0, 100.0, 5.0, 0, null)]
	public int ExhaustDeltaPct = 70;

	[InputParameter("Exhaustion: also require thinner volume", 25, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ExhaustNeedVolume;

	[InputParameter("Absorption: |delta| ≥ × typical", 26, 1.0, 10.0, 0.1, 1, null)]
	public double AbsorbMult = 2.5;

	[InputParameter("Absorption: price progress ≤ × typical (in delta direction)", 27, -1.0, 1.0, 0.05, 2, null)]
	public double AbsorbProgress = 0.25;

	[InputParameter("Closed bars only for SIGNALS – recommended on range bars", 28, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ClosedOnly = true;

	[InputParameter("LIVE readout: table / lanes follow the forming bar tick by tick", 28, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool LiveReadout = true;

	[InputParameter("LIVE readout: max redraws per second (0 = let the chart decide)", 28, 0.0, 60.0, 1.0, 0, null)]
	public int LiveFps = 20;

	[InputParameter("Minimum bars between same-type signals", 29, 1.0, 100.0, 1.0, 0, null)]
	public int SignalGap = 3;

	[InputParameter("Show signals", 30, 0.0, 0.0, 0.0, 0, new object[]
	{
		"All",
		SignalFilter.All,
		"Above lookback average",
		SignalFilter.AboveAverage,
		"Extreme (≥ 1.5× average)",
		SignalFilter.Extreme
	})]
	public SignalFilter Filter = SignalFilter.AboveAverage;

	[InputParameter("Show absorption", 31, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowAbsorption = true;

	[InputParameter("Show exhaustion", 32, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowExhaustion = true;

	[InputParameter("Show divergence", 33, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowDivergence = true;

	[InputParameter("Swing divergence: show (price HH / CVD LH and mirror)", 34, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowSwingDiv = true;

	[InputParameter("Swing divergence: pivot bars each side", 35, 1.0, 20.0, 1.0, 0, null)]
	public int SwingBars = 3;

	[InputParameter("Info table", 40, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Bottom right",
		Corner.BottomRight,
		"Top right",
		Corner.TopRight,
		"Bottom left",
		Corner.BottomLeft,
		"Hidden",
		Corner.Hidden
	})]
	public Corner Table = Corner.BottomRight;

	[InputParameter("Show score labels on signals", 41, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowScores;

	[InputParameter("Show zero line", 42, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowZero = true;

	[InputParameter("Keep indicator title / settings gear area clear (px wide)", 43, 0.0, 800.0, 10.0, 0, null)]
	public int TitleAreaWidth = 280;

	[InputParameter("Font size", 44, 6.0, 14.0, 0.5, 1, null)]
	public double FontSize = 7.0;

	[InputParameter("Absorption colour", 50, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color AbsorbColor = Color.FromArgb(255, 196, 61);

	[InputParameter("Exhaustion colour", 51, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color ExhaustColor = Color.FromArgb(255, 122, 66);

	[InputParameter("Divergence colour", 52, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color DivergeColor = Color.FromArgb(150, 130, 255);

	[InputParameter("Text colour", 53, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public Color TextColor = Color.FromArgb(232, 236, 244);

	// ---- 2.0 (tnx_cvd_optimization_plan.md): every new behaviour has its own setting; at "classic" CVD PRO is exactly 1.x ----

	[InputParameter("2.0 Info table style", 70, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Card (2.0: title, groups, last bar)",
		TableLook.Card,
		"Classic",
		TableLook.Classic
	})]
	public TableLook TableStyle;

	[InputParameter("2.0 Signals: keep the first marker (no repaint)", 71, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool KeepFirstSignal = true;

	[InputParameter("2.0 Swing divergence: marker on the bar that confirmed it", 72, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool SwingAtConfirm = true;

	[InputParameter("2.0 Flow speed tiers", 73, 0.0, 0.0, 0.0, 0, new object[]
	{
		"By rank in the lookback (research)",
		FlowTierMode.Percentile,
		"Fixed multiples (classic)",
		FlowTierMode.Classic
	})]
	public FlowTierMode FlowTiers;

	[InputParameter("2.0 Table: last bar speed and reading (range / tick bars)", 74, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowLastBar = true;

	[InputParameter("2.0 Signal card when the mouse points at a marker", 75, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool HoverCards = true;

	[InputParameter("2.0 Markers style", 76, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Pro badges (2.1)",
		MarkerLook.Badge,
		"Clean (2.0)",
		MarkerLook.Clean,
		"Classic",
		MarkerLook.Classic
	})]
	public MarkerLook MarkerStyle = MarkerLook.Badge;

	[InputParameter("2.0 Reset every N bars: count from each session start", 77, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ResetBarsFromSession = true;

	[InputParameter("2.0 Crisp candles (on whole pixels: sharper and faster)", 78, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool CrispCandles = true;

	// ---- 2.1: the look. The card can be placed, moved, sized and darkened; the lanes and markers have a Pro look. Nothing
	// here changes a number CVD PRO computes; the classic look stays one setting away ----

	[InputParameter("2.1 Card place", 79, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Auto: top left, over the oldest bars",
		CardPlace.Auto,
		"As the 'Info table' corner",
		CardPlace.InfoTable,
		"Top right",
		CardPlace.TopRight,
		"Top left",
		CardPlace.TopLeft,
		"Bottom right",
		CardPlace.BottomRight,
		"Bottom left",
		CardPlace.BottomLeft
	})]
	public CardPlace CardCorner = CardPlace.Auto;

	[InputParameter("2.1 Card move right (px, minus = left)", 80, -3000.0, 3000.0, 1.0, 0, null)]
	public int CardOffsetX;

	[InputParameter("2.1 Card move down (px, minus = up)", 81, -3000.0, 3000.0, 1.0, 0, null)]
	public int CardOffsetY;

	[InputParameter("2.1 Card width (px, 0 = fits its text)", 82, 0.0, 1200.0, 10.0, 0, null)]
	public int CardWidth;

	[InputParameter("2.1 Card size (%)", 83, 70.0, 160.0, 5.0, 0, null)]
	public int CardSize = 90;

	[InputParameter("2.1 Card background: black (%)", 84, 0.0, 100.0, 5.0, 0, null)]
	public int CardOpacity = 94;

	[InputParameter("2.1 Card rows", 85, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Full",
		CardRows.Full,
		"Compact (the key rows)",
		CardRows.Compact
	})]
	public CardRows CardLayout;

	[InputParameter("2.1 Flow speed and acceleration lanes style", 86, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Pro (2.1)",
		LaneLook.Pro,
		"Classic",
		LaneLook.Classic
	})]
	public LaneLook LaneStyle;

	// ---- 2.3: the flow lane reads the market, not only the delta. Every new behaviour has its own setting; the 2.2 picture
	// is one setting away (FlowShade = Classic, GateThrust off, ShowFade off, AccelInFlow off, CardCompass off) ----

	[InputParameter("2.3 Flow lane look", 87, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Bars (2.2)",
		FlowLook.Bars,
		"Polarized area (smooth)",
		FlowLook.Area,
		"Fast vs slow (histogram + lines)",
		FlowLook.Histogram,
		"Ribbon (fast / slow band)",
		FlowLook.Ribbon,
		"Needles (thin lines + dots)",
		FlowLook.Needles
	})]
	public FlowLook FlowStyle;

	[InputParameter("2.3 Flow bars brightness", 88, 0.0, 0.0, 0.0, 0, new object[]
	{
		"Real push: bright = one-sided AND busy, dark = thin or battle",
		FlowShade.RealPush,
		"Classic: by flow tier (2.2)",
		FlowShade.Classic
	})]
	public FlowShade Shade;

	[InputParameter("2.3 THRUST only when busy, one-sided and not absorbed (else a hollow 'thin' mark)", 89, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool GateThrust = true;

	[InputParameter("2.3 Early fade dot: flow still strong, one-sidedness dropping", 90, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool ShowFade = true;

	[InputParameter("2.3 Acceleration inside the flow lane (hollow = slowing, marks) when the strip is off", 91, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool AccelInFlow = true;

	[InputParameter("2.3 Card: flow compass (who, how busy, acceleration arrow, warning) instead of the flow text rows", 92, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool CardCompass = true;

	[InputParameter("2.3 Signal filter: all-history average when the lookback has fewer than 3 signals", 93, -2147483648.0, 2147483647.0, 0.01, 2, null)]
	public bool SignalAvgFallback = true;

	private Bar[] _b = new Bar[0];

	private int _n;

	private int _evalUpTo = -1;

	private readonly List<Sig> _sigs = new List<Sig>();

	private LineSeries _line;

	private LineSeries _hi;

	private LineSeries _lo;

	private LineLevel _zero;

	private readonly object _sync = new object();

	// P4 F18: the picture draws from its own copy of the bars and signals, so a tick never waits for a whole picture.
	// _sync guards the live data; the picture holds it only to copy what changed. _paintSync guards the copy and the
	// picture's fonts, pens and brushes. When both are taken: _paintSync first, then _sync.
	private readonly object _paintSync = new object();

	private Bar[] _pb = new Bar[0];

	private int _pn;

	private readonly List<Sig> _psigs = new List<Sig>();

	private double _pBarHeight;

	private int _dirtyFrom;   // lowest bar Compute wrote since the last copy; int.MaxValue = none

	private int _sigsVersion;   // changes with every change of _sigs

	private int _psigsVersion = -1;

	private int _clipTitleW;   // P4: the title area PaintCore cut out of the clip (PaintClip)

	private double _barHeight;

	private readonly Dictionary<long, Interval<DateTime>[]> _sessionCache = new Dictionary<long, Interval<DateTime>[]>();

	private bool? _hasPrimary;

	private int _phAbs = -1;

	private int _plAbs = -1;

	private double _phPx;

	private double _phCvd;

	private double _plPx;

	private double _plCvd;

	private Agg _agg;

	private double _aggN;

	private int _rangeObsAt = -1;

	private readonly double[] _rangeBuf = new double[21];   // 2.3: RangeFull's sort buffer

	private double _rangeObs;

	private int _pendingVa = -1;

	private Font _f;

	private Font _fb;

	private Font _fs;

	private readonly Dictionary<long, Pen> _pens = new Dictionary<long, Pen>();   // 2.0 (F4): keyed by colour and exact width

	private readonly Dictionary<long, Pen> _dotPens = new Dictionary<long, Pen>();   // 2.0: dotted pens, never changed after made

	private float _labelW = -1f;   // 2.0 (S2): width of the table's fixed labels for _labelFont

	private Font _labelFont;

	private float _labelDpi;   // 2.1: the screen scale _labelW was measured at (a monitor with another scale measures again)

	private readonly List<(RectangleF Box, Sig Sig)> _hitList = new List<(RectangleF Box, Sig Sig)>();   // 2.0 (A4): markers drawn in the last picture

	private double _lastAvg;   // 2.0: the average Passes used last (the marker's size)

	private Sig _hover;   // 2.0 (A4): the marker the mouse points at

	private int _recheck;   // 2.0 (F6): updates left in which the bar that just closed is read again

	private int _swingSavedAt = -1;   // 2.0 (F6): the bar whose swing state is kept in _swingSaved

	private (int, double, double, int, double, double) _swingSaved;

	private readonly List<Sig> _sigsBefore = new List<Sig>();   // 2.0 (F6): the signals before _swingSavedAt was evaluated

	private readonly Dictionary<int, SolidBrush> _brushes = new Dictionary<int, SolidBrush>();

	private readonly GdiCache _gdi = new GdiCache();   // P4: reused paths

	private readonly PointF[] _points0 = new PointF[4];

	private readonly PointF[] _points1 = new PointF[3];

	private readonly PointF[] _points2 = new PointF[3];

	private readonly PictureArrays _arrays = new PictureArrays();   // P4: work arrays reused between pictures

	private static readonly StringFormat Center = new StringFormat
	{
		Alignment = StringAlignment.Center,
		LineAlignment = StringAlignment.Center
	};

	private static readonly StringFormat Right = new StringFormat
	{
		Alignment = StringAlignment.Far,
		LineAlignment = StringAlignment.Near
	};

	private static readonly StringFormat NoWrap = new StringFormat
	{
		Trimming = StringTrimming.None,
		FormatFlags = (StringFormatFlags.NoWrap | StringFormatFlags.NoClip)
	};

	private double _liveC = double.NaN;

	private double _liveFast = double.NaN;

	private double _livePx = double.NaN;

	private double _liveVol = double.NaN;

	private long _liveAt;

	private Timer _liveTimer;

	private int _liveArmed;

	private const double ActiveX = 0.7;

	private float _laneTop;

	private double _reserveFrac = 0.5;

	private float _capH = 12f;

	private float _axisW = 48f;

	private bool _cvOk;

	private bool _cvAuto = true;

	private RectangleF _cvAutoBadgeRect;

	private double _cvCenter;

	private double _cvHalf = 1.0;

	private double _cvZoom = 1.0;

	private float _cvMid;

	private float _cvHalfPx = 1f;

	private float _cvOff;

	private double _cvMCenter;   // 2.2: MANUAL view of the CVD pane (centre, half range); set by the mouse, followed by the picture

	private double _cvMHalf = 1.0;

	private bool _cvMSet;   // 2.2: the MANUAL view starts from the view on the screen (false = take it at the next picture)

	private double _cvAnchor;   // 2.2: the newest visible candle's close (a stretch grows around it; it is kept in view)

	private double _cvRefHalf = 1.0;   // 2.2: half the visible range (the stretch stays within 1/50 .. 50 x of it)

	private double _dragCvC0;   // 2.2: the CVD view when a stretch / a drag started

	private bool _cvFollow = true;   // 2.2: MANUAL keeps the newest candle in view only when the user left it in view

	private float _cvBandTop;   // 2.2: the CVD candles' band in the last picture (where "in view" is)

	private float _cvBandBot;

	private double _dragCvH0 = 1.0;

	private RectangleF _cvRect;

	private RectangleF _spRect;

	private RectangleF _acRect;

	private int _winNum = -1;

	private IChart _mouseChart;

	private double _spZoom = 1.0;

	private double _acZoom = 1.0;

	private float _spOff;

	private float _acOff;

	private float _spRoom;   // 2.1: how far the flow lane's zero line may move from its middle (half the lane), last picture

	private float _acRoom;   // 2.1: the same for the acceleration lane

	private bool _drag;

	private bool _dragAxis;

	private int _dragLane;

	private int _dragY;

	private float _dragOff0;

	private float _dragH;

	private double _dragZoom0;

	private float _axisX = float.MaxValue;

	private float _spTop;

	private float _spBot;

	private float _acTop;

	private float _acBot;

	private float _spHpx;

	private float _acHpx;

	private float _spFrac;   // 2.2 (Pro): the flow pane's share of the window set by a drag on a line (0 = the setting)

	private float _acFrac;   // 2.2 (Pro): the same for the acceleration pane

	private float _pMinC = 40f;   // 2.2 (Pro): the panes' minimum heights in the last picture (CVD, flow, acceleration)

	private float _pMinF = 30f;

	private float _pMinA = 28f;

	// 2.2 (Pro): a lane's title pill, asked for by the lane and drawn after the card in the same picture (so it knows where
	// the card is now; a card position from the last picture made the first picture after a change differ: a flicker)
	private struct LaneTitleAsk
	{
		public bool On;

		public float Y;

		public string Name;

		public string Text;

		public Color Col;

		public int Tier;

		public Color Side;
	}

	private LaneTitleAsk _ltFlow;

	private LaneTitleAsk _ltAcc;

	// 2.2: each lane's scale in the last picture (read by the test machine: every bar must end inside its lane)
	private double _laneFlowScale;

	private float _laneFlowHalf;

	private float _laneFlowZero;

	private float _laneFlowTop;

	private float _laneFlowBot;

	private double _laneAccScale;

	private float _laneAccHalf;

	private float _laneAccZero;

	private float _laneAccTop;

	private float _laneAccBot;

	private float _dragH0;

	private float _dragH1;

	private Rectangle _winRect;

	private const float SplitGrab = 5f;

	private RectangleF _tableRect;

	private bool _soft;

	private int _softLane;

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

	private Color SignalColor => Skin switch
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
		Theme.Custom => VelocityCvd, 
		_ => Color.Orange, 
	};

	public bool IsRequirePriceLevelsCalculation => false;

	private bool LaneClosed
	{
		get
		{
			if (ClosedOnly)
			{
				return !LiveReadout;
			}
			return false;
		}
	}

	public override IList<SettingItem> Settings
	{
		get
		{
			return base.Settings;
		}
		set
		{
			if (value == null)
			{
				return;
			}
			List<SettingItem> list = new List<SettingItem>(value.Count);
			Dictionary<string, FieldInfo> dictionary = null;
			try
			{
				dictionary = new Dictionary<string, FieldInfo>();
				FieldInfo[] fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				foreach (FieldInfo fieldInfo in fields)
				{
					InputParameterAttribute customAttribute = fieldInfo.GetCustomAttribute<InputParameterAttribute>(inherit: true);
					if (customAttribute != null)
					{
						dictionary[string.IsNullOrEmpty(customAttribute.Name) ? fieldInfo.Name : customAttribute.Name] = fieldInfo;
					}
				}
			}
			catch
			{
				dictionary = null;
			}
			foreach (SettingItem item in value)
			{
				try
				{
					if (item != null)
					{
						if (dictionary == null || !dictionary.TryGetValue(item.Name, out var value2) || !value2.FieldType.IsEnum)
						{
							goto IL_00f9;
						}
						object obj2 = CoerceEnum((item.Value is SelectItem selectItem) ? selectItem.Value : item.Value, value2.FieldType, value2);
						if (obj2 != null)
						{
							value2.SetValue(this, obj2);
						}
					}
				}
				catch (Exception tnxError)
				{
					TnxErrors.Report("TNX CVD PRO", "class#1", tnxError);   // P4 F5: was empty
				}
				continue;
				IL_00f9:
				list.Add(item);
			}
			try
			{
				base.Settings = list;
			}
			catch
			{
				foreach (SettingItem item2 in list)
				{
					try
					{
						base.Settings = new List<SettingItem> { item2 };
					}
					catch (Exception tnxError)
					{
						TnxErrors.Report("TNX CVD PRO", "class#2", tnxError);   // P4 F5: was empty
					}
				}
			}
		}
	}

	public TNXCVDPro()
	{
		base.Name = "TNX CVD PRO";
		base.Description = "Cumulative delta with velocity panel, themes, and measured detectors: velocity divergence, exhaustion (declining push at new extremes), absorption (high delta, no result).";
		base.SeparateWindow = true;
		_line = AddLineSeries("CVD", Color.Transparent, 2);
		_hi = AddLineSeries("scale hi", Color.Transparent);
		_hi.ShowLineMarker = false;
		_lo = AddLineSeries("scale lo", Color.Transparent);
		_lo.ShowLineMarker = false;
		_zero = AddLineLevel(0.0, "zero", Color.FromArgb(110, 128, 128, 128), 1, LineStyle.Dash);
	}

	private static object CoerceEnum(object raw, Type enumType, FieldInfo f)
	{
		if (raw == null)
		{
			return null;
		}
		if (raw.GetType() == enumType)
		{
			return raw;
		}
		try
		{
			if (raw is string text)
			{
				if (Enum.TryParse(enumType, text, ignoreCase: true, out object result))
				{
					return result;
				}
				if (enumType == typeof(ResetMode) && string.Equals(text, "Every day (exchange time)", StringComparison.OrdinalIgnoreCase))
				{
					return ResetMode.Day;
				}
				if (enumType == typeof(ResetMode) && string.Equals(text, "Every week", StringComparison.OrdinalIgnoreCase))
				{
					return ResetMode.Week;
				}
				IComparable[] array = f.GetCustomAttribute<InputParameterAttribute>(inherit: true)?.Variants;
				if (array != null)
				{
					for (int i = 0; i + 1 < array.Length; i += 2)
					{
						if (array[i] is string a && string.Equals(a, text, StringComparison.OrdinalIgnoreCase) && array[i + 1] != null)
						{
							return CoerceEnum(array[i + 1], enumType, f);
						}
					}
				}
				if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result2) && Enum.IsDefined(enumType, result2))
				{
					return Enum.ToObject(enumType, result2);
				}
				return null;
			}
			if (raw is Enum)
			{
				if (Enum.TryParse(enumType, raw.ToString(), ignoreCase: true, out object result3))
				{
					return result3;
				}
				return Enum.ToObject(enumType, Convert.ToInt32(raw, CultureInfo.InvariantCulture));
			}
			if (raw is IConvertible)
			{
				int num = Convert.ToInt32(raw, CultureInfo.InvariantCulture);
				return Enum.IsDefined(enumType, num) ? Enum.ToObject(enumType, num) : null;
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "CoerceEnum#1", tnxError);   // P4 F5: was empty
		}
		return null;
	}

	protected override void OnInit()
	{
		try
		{
			lock (_sync)
			{
				StyleLines();
				_b = new Bar[0];
				_n = 0;
				_dirtyFrom = 0;   // P4 F18
				_recheck = 0;   // 2.0 (F6)
				_swingSavedAt = -1;
				_sigsBefore.Clear();
				_evalUpTo = -1;
				_sigs.Clear();
				_sigsVersion++;   // P4 F18
				Array.Clear(_kindSum, 0, 6);   // 2.3
				Array.Clear(_kindCnt, 0, 6);
				_sessionCache.Clear();
				_phAbs = (_plAbs = -1);
				_pendingVa = -1;
				_rangeObsAt = -1;
				_hasPrimary = null;
				DetectGeometry();
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnInit#1", tnxError);   // P4 F5: was empty
		}
	}

	private void StyleLines()
	{
		// P5 (F14): an if/else here only computed values it threw away (no effect); removed
		if (_line != null)
		{
			_line.Color = ((Mode == Display.Line) ? BullColor : Color.Transparent);
			_line.Width = Math.Max(1, LineWidth);
			_line.ShowLineMarker = Mode == Display.Line;
			_line.Visible = true;
		}
		if (_zero != null)
		{
			_zero.Visible = ShowZero;
		}
	}

	protected override void OnClear()
	{
		try
		{
			OnClearCore();
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnClear#1", tnxError);   // P4 F5: was empty
		}
	}

	private void OnClearCore()
	{
		lock (_paintSync)   // P4 F18: no picture is using the fonts, pens and brushes disposed here
		{
			lock (_sync)
			{
				OnClearLocked();
			}
		}
	}

	private void OnClearLocked()
	{
		_f?.Dispose();
		_fb?.Dispose();
		_fs?.Dispose();
		_f = (_fb = (_fs = null));
		DisposeCardFonts();   // 2.1
		DisposeRefFonts();
		foreach (Pen value in _pens.Values)
		{
			value.Dispose();
		}
		_pens.Clear();
		foreach (Pen value3 in _dotPens.Values)
		{
			value3.Dispose();
		}
		_dotPens.Clear();
		foreach (Pen value4 in _dashPens.Values)   // 2.3
		{
			value4.Dispose();
		}
		_dashPens.Clear();
		_labelFont = null;
		_hitList.Clear();
		_hover = null;
		_gdi.Clear();
		_arrays.Clear();
		foreach (SolidBrush value2 in _brushes.Values)
		{
			value2.Dispose();
		}
		_brushes.Clear();
		UnhookMouse();
		_liveTimer?.Dispose();
		_liveTimer = null;
		_liveArmed = 0;
	}

	public void VolumeAnalysisData_Loaded()
	{
		try
		{
			lock (_sync)
			{
				RecalcAll();
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "VolumeAnalysisData_Loaded#1", tnxError);   // P4 F5: was empty
		}
		try
		{
			base.CurrentChart?.RedrawBuffer();
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "VolumeAnalysisData_Loaded#2", tnxError);   // P4 F5: was empty
		}
	}

	protected override void OnSettingsUpdated()
	{
		try
		{
			base.OnSettingsUpdated();
			lock (_paintSync)   // P4 F18: no picture is using the fonts disposed here
			{
				lock (_sync)
				{
					StyleLines();
					_f?.Dispose();
					_fb?.Dispose();
					_fs?.Dispose();
					_f = (_fb = (_fs = null));
					DisposeCardFonts();   // 2.1
					DisposeRefFonts();
					_cfScale = -1f;
					_lastBarSpeed = null;   // 2.0: the card's texts are made again with the new settings
					_flowText = null;
					_spFrac = (_acFrac = 0f);   // 2.2: the dialog's pane shares apply
				}
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnSettingsUpdated#1", tnxError);   // P4 F5: was empty
		}
		try
		{
			base.CurrentChart?.RedrawBuffer();   // 2.3: a changed setting shows at once, not at the next tick (a quiet market kept the old picture)
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnSettingsUpdated#2", tnxError);
		}
	}

	// 2.3: the platform disposes a removed indicator; the mouse hooks, timer, fonts, pens and brushes go with it even when
	// OnClear is not called first (the official mouse example unsubscribes in Dispose)
	public override void Dispose()
	{
		try
		{
			OnClearCore();
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "Dispose#1", tnxError);
		}
		base.Dispose();
	}

	private void DetectGeometry()
	{
		_barHeight = 0.0;
		_agg = Agg.Other;
		_aggN = 0.0;
		_rangeObsAt = -1;
		try
		{
			double num = base.Symbol?.TickSize ?? 0.0;
			HistoryAggregation historyAggregation = base.HistoricalData?.Aggregation;
			if (historyAggregation is HistoryAggregationRangeBars historyAggregationRangeBars)
			{
				_agg = Agg.Range;
				if (num > 0.0)
				{
					_barHeight = (double)historyAggregationRangeBars.RangeBars * num;
				}
			}
			else if (historyAggregation is HistoryAggregationTickBars historyAggregationTickBars)
			{
				if (historyAggregationTickBars.TicksCount > 0)
				{
					_agg = Agg.Tick;
					_aggN = historyAggregationTickBars.TicksCount;
				}
			}
			else if (historyAggregation is HistoryAggregationRenko historyAggregationRenko)
			{
				if (num > 0.0)
				{
					_barHeight = (double)historyAggregationRenko.BrickSize * num;
				}
			}
			else if (historyAggregation != null && historyAggregation.GetType() == typeof(HistoryAggregationTime))
			{
				Period period = ((HistoryAggregationTime)historyAggregation).Period;
				if (period.PeriodMultiplier > 0 && (period.BasePeriod == BasePeriod.Second || period.BasePeriod == BasePeriod.Minute || period.BasePeriod == BasePeriod.Hour))
				{
					_agg = Agg.Time;
					_aggN = period.Ticks;
				}
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "DetectGeometry#1", tnxError);   // P4 F5: was empty
		}
		_chartTag = ChartTag();   // 2.0: the card's title; the picture gets its copy with the bars (CopyForPaint)
	}

	protected override void OnUpdate(UpdateArgs args)
	{
		bool flag = false;
		try
		{
			lock (_sync)
			{
				flag = UpdateCore(args);
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnUpdate#1", tnxError);   // P4 F5: was empty
		}
		if (flag)
		{
			try
			{
				base.CurrentChart?.RedrawBuffer();
			}
			catch (Exception tnxError)
			{
				TnxErrors.Report("TNX CVD PRO", "OnUpdate#2", tnxError);   // P4 F5: was empty
			}
		}
	}

	private bool UpdateCore(UpdateArgs args)
	{
		int count = base.Count;
		int num = count - 1;
		if (num < 0)
		{
			return false;
		}
		if (_n == 0 && _agg == Agg.Other && _barHeight == 0.0 && base.HistoricalData?.Aggregation != null)
		{
			DetectGeometry();
		}
		if (count < _n)
		{
			RecalcAll();
			return LiveRedrawDue(base.Count - 1);
		}
		Ensure(count);
		if (_pendingVa >= 0)
		{
			if (_pendingVa >= num)
			{
				_pendingVa = -1;
			}
			else if (GetVolumeAnalysisData(Off(_pendingVa))?.Total != null)
			{
				RecalcAll();
				return LiveRedrawDue(base.Count - 1);
			}
		}
		int first = ((_n < count || (args != null && args.Reason == UpdateReason.NewBar)) ? Math.Max(0, Math.Min(_n, num) - 1) : num);
		for (int i = first; i <= num; i++)
		{
			Compute(i);
			Publish(i);
		}
		_n = count;
		if (first < num)
		{
			_recheck = 3;
		}
		else if (_recheck > 0 && num >= 1 && num - 1 <= _evalUpTo)
		{
			_recheck--;
			RecheckClosed(num - 1);
		}
		bool live = args == null || args.Reason != UpdateReason.HistoricalBar;
		for (int j = _evalUpTo + 1; j <= num - 1; j++)
		{
			if (live)
			{
				// 2.0 (F6): the swing state and the signals before this bar is read, so it can be read again exactly
				_swingSavedAt = j;
				_swingSaved = (_phAbs, _phPx, _phCvd, _plAbs, _plPx, _plCvd);
				_sigsBefore.Clear();
				_sigsBefore.AddRange(_sigs);
				Array.Copy(_kindSum, _kindSumBefore, 6);   // 2.3: the filter's running averages belong to the same snapshot
				Array.Copy(_kindCnt, _kindCntBefore, 6);
			}
			else
			{
				_swingSavedAt = -1;
			}
			Evaluate(j, pending: false);
		}
		_evalUpTo = Math.Max(_evalUpTo, num - 1);
		if (!ClosedOnly)
		{
			Evaluate(num, pending: true);
		}
		else
		{
			RemovePending(num);
		}
		return LiveRedrawDue(num);
	}

	// 2.0 (plan F6): the bar that just closed was computed once, when the next bar began. If Quantower's volume analysis
	// completes it a moment later, compute it (and the forming bar after it) again and evaluate its signals again, so the
	// live chart equals a reload. Nothing happens when the data did not change.
	private void RecheckClosed(int abs)
	{
		VolumeAnalysisItem va = GetVolumeAnalysisData(Off(abs))?.Total;
		if (va == null)
		{
			return;
		}
		ref Bar b = ref _b[abs];
		if (va.Delta == b.Delta && va.Volume == b.Vol && va.BuyVolume == b.Buy && va.SellVolume == b.Sell)
		{
			return;
		}
		for (int i = abs; i < _n; i++)
		{
			Compute(i);
			Publish(i);
		}
		if (_swingSavedAt == abs)
		{
			// the signals as they were before this bar was first read (what that reading added, removed or made stronger is
			// undone), so the bar is read again exactly as a reload reads it
			_sigs.Clear();
			_sigs.AddRange(_sigsBefore);
			Array.Copy(_kindSumBefore, _kindSum, 6);   // 2.3
			Array.Copy(_kindCntBefore, _kindCnt, 6);
			_sigsVersion++;
			(_phAbs, _phPx, _phCvd, _plAbs, _plPx, _plCvd) = _swingSaved;
			Evaluate(abs, pending: false);
		}
	}

	private bool LiveRedrawDue(int last)
	{
		if (!LiveReadout || last < 0 || last >= _b.Length)
		{
			return false;
		}
		ref Bar reference = ref _b[last];
		if (reference.C == _liveC && reference.Fast == _liveFast && reference.Px == _livePx && reference.Vol == _liveVol)
		{
			return false;
		}
		int num = Math.Max(0, Math.Min(60, LiveFps));
		if (num == 0)
		{
			return false;
		}
		_liveC = reference.C;
		_liveFast = reference.Fast;
		_livePx = reference.Px;
		_liveVol = reference.Vol;
		long tickCount = Environment.TickCount64;
		long num2 = 1000L / (long)num - (tickCount - _liveAt);
		if (num2 <= 0)
		{
			_liveAt = tickCount;
			return true;
		}
		if (Interlocked.Exchange(ref _liveArmed, 1) == 0)
		{
			try
			{
				(_liveTimer ?? (_liveTimer = new Timer(OnLiveTimer))).Change(num2, -1L);
			}
			catch
			{
				_liveArmed = 0;
			}
		}
		return false;
	}

	private void OnLiveTimer(object state)
	{
		Interlocked.Exchange(ref _liveArmed, 0);
		try
		{
			lock (_sync)
			{
				_liveAt = Environment.TickCount64;
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnLiveTimer#1", tnxError);   // P4 F5: was empty
		}
		try
		{
			base.CurrentChart?.RedrawBuffer();
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnLiveTimer#2", tnxError);   // P4 F5: was empty
		}
	}

	private void RecalcAll()
	{
		if (base.Count <= 0)
		{
			return;
		}
		_sigs.Clear();
		_sigsVersion++;   // P4 F18
		Array.Clear(_kindSum, 0, 6);   // 2.3
		Array.Clear(_kindCnt, 0, 6);
		_phAbs = (_plAbs = -1);
		_pendingVa = -1;
		_rangeObsAt = -1;
		_recheck = 0;   // 2.0 (F6)
		_swingSavedAt = -1;
		Ensure(base.Count);
		for (int i = 0; i < base.Count; i++)
		{
			Compute(i);
			Publish(i);
			if (i < base.Count - 1)
			{
				Evaluate(i, pending: false);
			}
		}
		_n = base.Count;
		_evalUpTo = base.Count - 2;
		_sigsBefore.Clear();   // 2.0 (F6): only kept for the bar a live update reads
		if (!ClosedOnly)
		{
			Evaluate(base.Count - 1, pending: true);
		}
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

	private static double Safe(double v, double fb)
	{
		if (!double.IsNaN(v) && !double.IsInfinity(v) && v != double.MaxValue && v != double.MinValue)
		{
			return v;
		}
		return fb;
	}

	private void Compute(int abs)
	{
		int num = Off(abs);
		if (num >= 0)
		{
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			double num5 = 0.0;
			double num6 = 0.0;
			bool flag = false;
			VolumeAnalysisItem volumeAnalysisItem = GetVolumeAnalysisData(num)?.Total;
			double num7;
			if (volumeAnalysisItem != null)
			{
				num2 = volumeAnalysisItem.Delta;
				num5 = volumeAnalysisItem.BuyVolume;
				num6 = volumeAnalysisItem.SellVolume;
				num7 = volumeAnalysisItem.Volume;
				num4 = Math.Max(Safe(volumeAnalysisItem.MaxDelta, 0.0), Math.Max(0.0, num2));
				num3 = Math.Min(Safe(volumeAnalysisItem.MinDelta, 0.0), Math.Min(0.0, num2));
				flag = num7 > 0.0 || num5 > 0.0 || num6 > 0.0;
			}
			else
			{
				num7 = Safe(GetPrice(PriceType.Volume, num), 0.0);
			}
			if (abs < _dirtyFrom)
			{
				_dirtyFrom = abs;   // P4 F18: the picture copies this bar again
			}
			ref Bar reference = ref _b[abs];
			reference.Ticks = Time(num).Ticks;
			// 2.3: how long the bar took, in seconds: a finished bar from its open to the next bar's open (as the card's "Last bar"
			// row reads it), the forming bar from its open until now. Floored at 0.1 s (a small bar can finish inside one timestamp).
			reference.Dur = Math.Max(0.1, (double)(((num >= 1) ? Time(num - 1).Ticks : NowTicks()) - reference.Ticks) / 1E+07);
			bool barsFromSession = Reset == ResetMode.Bars && ResetBarsFromSession;
			reference.Anchor = ((Reset == ResetMode.Session || barsFromSession) ? SessionAnchor(abs) : 0);
			if (barsFromSession)
			{
				// 2.0 (plan F5): N bars counted from each session's first bar, so the reset points do not move when the chart
				// loads a different number of bars (classic: counted from the first bar the chart loaded)
				reference.SessIdx = (abs == 0 || reference.Anchor != _b[abs - 1].Anchor) ? 0 : (_b[abs - 1].SessIdx + 1);
			}
			bool flag2 = abs == 0 || IsReset(abs);
			double num8 = (flag2 ? 0.0 : _b[abs - 1].C);
			reference.Seg = (flag2 ? abs : _b[abs - 1].Seg);
			reference.Delta = num2;
			reference.MinD = num3;
			reference.MaxD = num4;
			reference.Buy = num5;
			reference.Sell = num6;
			reference.Vol = num7;
			reference.Real = flag;
			reference.O = num8;
			reference.C = num8 + num2;
			reference.H = num8 + num4;
			reference.L = num8 + num3;
			reference.Px = GetPrice(PriceType.Close, num);
			reference.PO = GetPrice(PriceType.Open, num);
			reference.PH = GetPrice(PriceType.High, num);
			reference.PL = GetPrice(PriceType.Low, num);
			reference.Has = true;
			reference.F = Progress(abs);
			if (volumeAnalysisItem == null && abs < base.Count - 1 && (_pendingVa < 0 || abs < _pendingVa) && VaReady())
			{
				_pendingVa = abs;
			}
			double num9 = Math.Max(num4, 0.0 - num3);
			double val = 0.0;
			if (num9 > 0.0)
			{
				val = ((num2 >= 0.0) ? ((num4 - num2) / num9) : ((num2 - num3) / num9));
			}
			val = Math.Max(0.0, Math.Min(1.0, val));
			double num10 = ((abs > 0) ? Math.Abs(reference.Px - _b[abs - 1].Px) : 0.0);
			double num11 = Math.Abs(num2);
			if (abs > 0)
			{
				ref Bar reference2 = ref _b[abs - 1];
				reference.PAbsDp = reference2.PAbsDp + num10;
				reference.PAbsDc = reference2.PAbsDc + num11;
				reference.PBuy = reference2.PBuy + (flag ? num5 : 0.0);
				reference.PSell = reference2.PSell + (flag ? num6 : 0.0);
				reference.PVol = reference2.PVol + num7;
				reference.PRej = reference2.PRej + val;
				reference.PDelta = reference2.PDelta + num2;
				reference.PDur = reference2.PDur + reference.Dur;   // 2.3
			}
			else
			{
				reference.PAbsDp = num10;
				reference.PAbsDc = num11;
				reference.PBuy = (flag ? num5 : 0.0);
				reference.PSell = (flag ? num6 : 0.0);
				reference.PVol = num7;
				reference.PRej = val;
				reference.PDelta = num2;
				reference.PDur = reference.Dur;   // 2.3
			}
			Velocity(abs, out reference.R, out reference.E);
			int val2 = Math.Max(2, VelocityPeriod / 3);
			int val3 = Math.Max(3, VelocityPeriod);
			int num12 = Math.Max(1, AccelLag);
			int num13 = Math.Max(1, AccelSmooth);
			int num14 = Math.Min(val2, abs + 1);
			int num15 = Math.Min(val3, abs + 1);
			double f = reference.F;
			reference.Fast = Win(abs, num14, Pf.Delta) / (double)num14;
			reference.Slow = Win(abs, num15, Pf.Delta) / (double)num15;
			double num16 = ((abs - num12 < 0) ? 0.0 : (reference.Fast - ((f < 1.0 && abs - num12 - 1 >= 0) ? (_b[abs - num12 - 1].Fast + f * (_b[abs - num12].Fast - _b[abs - num12 - 1].Fast)) : _b[abs - num12].Fast)));
			double num17 = ((abs > 0) ? _b[abs - 1].AccE : num16);
			double num18 = 2.0 / (double)(num13 + 1);
			double num19 = ((f >= 1.0) ? num18 : (1.0 - Math.Pow(1.0 - num18, f)));
			reference.AccE = ((num13 <= 1 || abs == 0) ? num16 : (num17 + num19 * (num16 - num17)));
			reference.PAbsAcc = ((abs > 0) ? _b[abs - 1].PAbsAcc : 0.0) + Math.Abs(reference.AccE);
			reference.Flip = abs > 0 && reference.Fast >= 0.0 != _b[abs - 1].Fast >= 0.0;
			int num20 = ((reference.R >= 0.0) ? 1 : (-1));
			int num21 = ((reference.E >= 0.0) ? 1 : (-1));
			bool flag3 = Math.Abs(reference.R) >= DivergeMin && Math.Abs(reference.E) >= DivergeMin && num20 != num21;
			int num22 = ((abs > 0) ? _b[abs - 1].DivRun : 0);
			reference.DivRun = (flag3 ? ((num20 <= 0) ? ((num22 < 0) ? (num22 - 1) : (-1)) : ((num22 <= 0) ? 1 : (num22 + 1))) : 0);
			// ---- 2.3: the real-push dials. The flow lane used delta size alone; delta size cannot tell a busy, one-sided market
			// (a real push) from a quiet one (a thin move) or from a two-sided fight (a battle). Three dials over the fast window:
			// one-sidedness (delta / traded volume, Quantower's own Delta %), volume per second (how busy), and the bar's own
			// effort-vs-result reading (absorbed). Ranks over the lookback, like the flow tiers. All O(lookback) once per bar. ----
			double bs7 = Win(abs, num14, Pf.Buy) + Win(abs, num14, Pf.Sell);
			reference.Imb7 = ((bs7 > 0.0) ? (Win(abs, num14, Pf.Delta) / bs7) : 0.0);
			int n3 = Math.Min(3, abs + 1);
			double bs3 = Win(abs, n3, Pf.Buy) + Win(abs, n3, Pf.Sell);
			reference.Imb3 = ((bs3 > 0.0) ? (Win(abs, n3, Pf.Delta) / bs3) : 0.0);
			reference.Vps = Win(abs, num14, Pf.Vol) / Math.Max(0.1, Win(abs, num14, Pf.Dur));
			reference.FRank = FlowRank(_b, abs, slow: false);
			reference.SRank = FlowRank(_b, abs, slow: true);
			reference.BusyRank = RankOf(abs, 0);
			reference.ImbRank = RankOf(abs, 1);
			// real push = how one-sided, gated by how busy: under a busy rank of 20 % nothing is bright, from 70 % up the
			// one-sidedness alone decides (a quiet one-sided move is a thin move, not a push)
			reference.Q = ((reference.BusyRank < 0.0 || reference.ImbRank < 0.0) ? 0.5 : (reference.ImbRank * Math.Max(0.0, Math.Min(1.0, (reference.BusyRank - 0.2) / 0.5))));
			BarRead(abs, out double e1, out double r1);
			double ae1 = Math.Abs(e1);
			double res1 = r1 * ((e1 >= 0.0) ? 1.0 : (-1.0));
			reference.Absorbed = reference.Real && ae1 >= 0.7 && (res1 <= -0.7 || (ae1 >= 1.0 && res1 / ae1 < 0.35));
			bool sameSide = (reference.Imb3 >= 0.0) == (reference.Fast >= 0.0);
			reference.Fade = reference.FRank >= 0.8 && (!sameSide || Math.Abs(reference.Imb3) < 0.6 * Math.Abs(reference.Imb7));
			reference.ThrustOk = reference.BusyRank >= 0.5 && ((reference.Imb7 >= 0.0) == (reference.Fast >= 0.0)) && Math.Abs(reference.Imb7) >= 0.1 && sameSide && Math.Abs(reference.Imb3) >= 0.9 * Math.Abs(reference.Imb7) && !reference.Absorbed;
		}
	}

	private int Lb(int abs)
	{
		return Math.Min(Math.Max(5, LookbackBars), abs + 1);
	}

	private static double Pref(Bar[] b, int i, Pf k)
	{
		if (i < 0)
		{
			return 0.0;
		}
		ref Bar reference = ref b[i];
		return k switch
		{
			Pf.AbsDp => reference.PAbsDp, 
			Pf.AbsDc => reference.PAbsDc, 
			Pf.Buy => reference.PBuy, 
			Pf.Sell => reference.PSell, 
			Pf.Vol => reference.PVol, 
			Pf.Rej => reference.PRej, 
			Pf.Delta => reference.PDelta, 
			Pf.Dur => reference.PDur, 
			_ => reference.PAbsAcc, 
		};
	}

	private double Win(int abs, int len, Pf k)
	{
		return Win(_b, abs, len, k);
	}

	// P4 F18: the same sum over any copy of the bars (the live ones, or the picture's copy)
	private static double Win(Bar[] b, int abs, int len, Pf k)
	{
		double num = Pref(b, abs, k) - Pref(b, abs - len, k);
		double f = b[abs].F;
		if (f < 1.0)
		{
			if (abs - len >= 0)
			{
				num += (1.0 - f) * (Pref(b, abs - len, k) - Pref(b, abs - len - 1, k));
			}
			if (k == Pf.AbsAcc || k == Pf.Rej)
			{
				num -= (1.0 - f) * (Pref(b, abs, k) - Pref(b, abs - 1, k));
			}
		}
		return num;
	}

	private double Progress(int abs)
	{
		if (abs < 1 || abs != base.Count - 1)
		{
			return 1.0;
		}
		try
		{
			double num = 1.0;
			switch (_agg)
			{
			case Agg.Range:
			{
				double num2 = RangeFull(abs);
				if (num2 > 0.0)
				{
					num = (_b[abs].PH - _b[abs].PL) / num2;
				}
				break;
			}
			case Agg.Tick:
				if (_aggN > 0.0)
				{
					num = Ticks(Off(abs)) / _aggN;
				}
				break;
			case Agg.Time:
				if (_aggN > 0.0)
				{
					num = (double)(Core.Instance.TimeUtils.DateTimeUtcNow.Ticks - _b[abs].Ticks) / _aggN;
				}
				break;
			}
			return (double.IsNaN(num) || double.IsInfinity(num)) ? 1.0 : Math.Max(0.0, Math.Min(1.0, num));
		}
		catch
		{
			return 1.0;
		}
	}

	private double RangeFull(int forming)
	{
		if (_rangeObsAt == forming)
		{
			return _rangeObs;
		}
		_rangeObsAt = forming;
		_rangeObs = _barHeight;
		int num = Math.Min(21, forming);
		if (num >= 3)
		{
			double[] array = _rangeBuf;   // 2.3: reused (was a new array per forming bar on the compute thread)
			for (int i = 0; i < num; i++)
			{
				array[i] = _b[forming - 1 - i].PH - _b[forming - 1 - i].PL;
			}
			Array.Sort(array, 0, num);
			if (array[num / 2] > 0.0)
			{
				_rangeObs = array[num / 2];
			}
		}
		return _rangeObs;
	}

	private bool VaReady()
	{
		try
		{
			HistoricalData historicalData = base.HistoricalData;
			return historicalData != null && historicalData.VolumeAnalysisCalculationProgress?.State == VolumeAnalysisCalculationState.Finished;
		}
		catch
		{
			return false;
		}
	}

	private double ScaleP(int abs)
	{
		return ScaleP(_b, _barHeight, abs);
	}

	// P4 F18: over any copy of the bars, with the bar height that goes with it
	private double ScaleP(Bar[] b, double barHeight, int abs)
	{
		int num = Lb(abs);
		if (num <= 1)
		{
			return 0.0;
		}
		double num2 = Win(b, abs, num, Pf.AbsDp) / (double)num;
		if (barHeight > 0.0)
		{
			num2 = Math.Max(num2, barHeight * 0.5);
		}
		double num3 = base.Symbol?.TickSize ?? 0.0;
		if (num3 > 0.0)
		{
			num2 = Math.Max(num2, num3);
		}
		return num2;
	}

	private double ScaleC(int abs)
	{
		return ScaleC(_b, abs);
	}

	// P4 F18: over any copy of the bars
	private double ScaleC(Bar[] b, int abs)
	{
		int num = Lb(abs);
		if (num > 1)
		{
			return Math.Max(Win(b, abs, num, Pf.AbsDc) / (double)num, 1E-09);
		}
		return 0.0;
	}

	private void Velocity(int abs, out double R, out double E)
	{
		R = 0.0;
		E = 0.0;
		int num = Math.Max(1, VelocityBars);
		if (abs >= num)
		{
			double num2 = ScaleP(abs);
			double num3 = ScaleC(abs);
			if (!(num2 <= 0.0) && !(num3 <= 0.0))
			{
				double num4 = Math.Sqrt(num);
				double f = _b[abs].F;
				double num5 = ((f < 1.0 && abs - num - 1 >= 0) ? (_b[abs - num - 1].Px + f * (_b[abs - num].Px - _b[abs - num - 1].Px)) : _b[abs - num].Px);
				R = (_b[abs].Px - num5) / (num2 * num4);
				E = Win(abs, num, Pf.Delta) / (num3 * num4);
			}
		}
	}

	private void BarRead(int abs, out double e, out double r)
	{
		BarRead(_b, _barHeight, abs, out e, out r);
	}

	// P4 F18: over any copy of the bars, with the bar height that goes with it
	private void BarRead(Bar[] b, double barHeight, int abs, out double e, out double r)
	{
		ref Bar reference = ref b[abs];
		double num = ScaleC(b, abs);
		double num2 = ScaleP(b, barHeight, abs);
		e = ((num > 1E-09) ? (reference.Delta / num) : 0.0);
		r = ((num2 > 0.0) ? ((reference.Px - reference.PO) / num2) : 0.0);
	}

	private bool RealWindow(int abs, int W)
	{
		for (int i = Math.Max(0, abs - W); i <= abs; i++)
		{
			if (!_b[i].Real)
			{
				return false;
			}
		}
		return true;
	}

	private void Push(int end, int W, bool up, out double d, out double v)
	{
		d = 0.0;
		v = 0.0;
		for (int i = Math.Max(0, end - W + 1); i <= end; i++)
		{
			d += (up ? Math.Max(0.0, _b[i].Delta) : Math.Max(0.0, 0.0 - _b[i].Delta));
			v += _b[i].Vol;
		}
	}

	private void Evaluate(int abs, bool pending)
	{
		if (abs < Math.Max(VelocityBars, 10) || abs >= _b.Length || !_b[abs].Has)
		{
			return;
		}
		RemovePending(abs);
		if (!RealWindow(abs, Math.Max(VelocityBars, 2)))
		{
			return;
		}
		int num = Math.Max(1, VelocityBars);
		int num2 = Lb(abs) - 1;
		ref Bar reference = ref _b[abs];
		double num3 = ScaleC(abs);
		double num4 = ScaleP(abs);
		double num5 = double.MinValue;
		double num6 = double.MaxValue;
		double num7 = double.MinValue;
		double num8 = double.MaxValue;
		int num9 = -1;
		int num10 = -1;
		for (int i = Math.Max(0, abs - num2); i < abs; i++)
		{
			if (_b[i].PH > num5)
			{
				num5 = _b[i].PH;
				num9 = i;
			}
			if (_b[i].PL < num6)
			{
				num6 = _b[i].PL;
				num10 = i;
			}
			if (i >= reference.Seg)
			{
				if (_b[i].C > num7)
				{
					num7 = _b[i].C;
				}
				if (_b[i].C < num8)
				{
					num8 = _b[i].C;
				}
			}
		}
		bool flag = num9 >= 0 && reference.PH > num5;
		bool flag2 = num10 >= 0 && reference.PL < num6;
		bool flag3 = reference.C >= num7;
		bool flag4 = reference.C <= num8;
		if (Math.Abs(reference.DivRun) >= Math.Max(1, DivergePersist))
		{
			int num11 = Math.Abs(reference.DivRun);
			double num12 = 0.0;
			for (int j = abs - num11 + 1; j <= abs; j++)
			{
				num12 += Math.Min(Math.Abs(_b[j].R), Math.Abs(_b[j].E));
			}
			bool flag5 = reference.DivRun > 0;
			Add(abs, SigKind.Divergence, flag5, num12, reference.R, reference.E, pending, flag5 ? "flow: price rises on net selling – sellers trapped" : "flow: price falls on net buying – buyers trapped");
		}
		if (!pending && ShowSwingDiv)
		{
			int num13 = Math.Max(1, SwingBars);
			int num14 = abs - num13;
			if (num14 - num13 >= 0 && !IsResetBetween(num14 - num13, abs))
			{
				bool flag6 = true;
				bool flag7 = true;
				for (int k = num14 - num13; k <= abs; k++)
				{
					if (!(flag6 || flag7))
					{
						break;
					}
					if (k != num14)
					{
						if ((k < num14) ? (_b[k].PH >= _b[num14].PH) : (_b[k].PH > _b[num14].PH))
						{
							flag6 = false;
						}
						if ((k < num14) ? (_b[k].PL <= _b[num14].PL) : (_b[k].PL < _b[num14].PL))
						{
							flag7 = false;
						}
					}
				}
				if (flag6)
				{
					if (_phAbs >= 0 && num14 - _phAbs >= num13 && !IsResetBetween(_phAbs, num14) && _b[num14].PH > _phPx && _b[num14].C < _phCvd && num4 > 0.0 && num3 > 0.0)
					{
						double val = (_b[num14].PH - _phPx) / num4 + (_phCvd - _b[num14].C) / num3;
						Sig sg = new Sig
						{
							Abs = abs,
							Kind = SigKind.Divergence,
							Bull = false,
							Score = Math.Max(0.1, val),
							R = reference.R,
							E = reference.E,
							Pending = false,
							Note = "swing: price higher high, CVD lower high",
							P1 = _phAbs,
							P2 = num14,
							V1 = _phCvd,
							V2 = _b[num14].C
						};
						AddSig(sg);
					}
					_phAbs = num14;
					_phPx = _b[num14].PH;
					_phCvd = _b[num14].C;
				}
				if (flag7)
				{
					if (_plAbs >= 0 && num14 - _plAbs >= num13 && !IsResetBetween(_plAbs, num14) && _b[num14].PL < _plPx && _b[num14].C > _plCvd && num4 > 0.0 && num3 > 0.0)
					{
						double val2 = (_plPx - _b[num14].PL) / num4 + (_b[num14].C - _plCvd) / num3;
						Sig sg2 = new Sig
						{
							Abs = abs,
							Kind = SigKind.Divergence,
							Bull = true,
							Score = Math.Max(0.1, val2),
							R = reference.R,
							E = reference.E,
							Pending = false,
							Note = "swing: price lower low, CVD higher low",
							P1 = _plAbs,
							P2 = num14,
							V1 = _plCvd,
							V2 = _b[num14].C
						};
						AddSig(sg2);
					}
					_plAbs = num14;
					_plPx = _b[num14].PL;
					_plCvd = _b[num14].C;
				}
			}
		}
		if (flag || flag2)
		{
			bool flag8 = flag;
			int num15 = (flag8 ? num9 : num10);
			Push(abs, num, flag8, out var d, out var v);
			Push(num15, num, flag8, out var d2, out var v2);
			if (abs - num15 >= num && d2 > 0.0 && RealWindow(num15, num))
			{
				bool num16 = d <= d2 * (double)ExhaustDeltaPct / 100.0;
				bool flag9 = v <= v2;
				bool flag10 = (flag8 ? (!flag3) : (!flag4));
				if (num16 && (flag10 || d <= d2 * 0.5) && (!ExhaustNeedVolume || flag9))
				{
					double val3 = (1.0 - d / d2) * 2.0 + (flag9 ? (1.0 - v / Math.Max(v2, 1E-09)) : 0.0) + (flag10 ? 0.5 : 0.0);
					string note = $"push {d / d2 * 100.0:0}% of previous{(flag9 ? ", thinner volume" : "")}{(flag10 ? ", CVD no new extreme" : "")}";
					Add(abs, SigKind.Exhaustion, !flag8, Math.Max(0.1, val3), reference.R, reference.E, pending, note);
				}
			}
		}
		if (!(num3 > 0.0) || !(num4 > 0.0))
		{
			return;
		}
		BarRead(abs, out var e, out var r);
		int num17 = ((e >= 0.0) ? 1 : (-1));
		double num18 = r * (double)num17;
		double num19 = Win(abs, num, Pf.Rej) / (double)num;
		if (Math.Abs(e) >= AbsorbMult && num18 <= AbsorbProgress)
		{
			double score = Math.Abs(e) * (1.0 + num19) * ((num18 <= 0.0) ? 1.25 : 1.0);
			string note2 = ((num17 > 0) ? ("buyers absorbed by passive sellers (bar delta " + FmtS(reference.Delta) + ")") : ("sellers absorbed by passive buyers (bar delta " + FmtS(reference.Delta) + ")"));
			Add(abs, SigKind.Absorption, num17 < 0, score, reference.R, reference.E, pending, note2);
			return;
		}
		int num20 = ((reference.E >= 0.0) ? 1 : (-1));
		if (Math.Abs(reference.E) >= AbsorbMult && reference.R * (double)num20 <= AbsorbProgress)
		{
			double score2 = Math.Abs(reference.E) * (1.0 + num19);
			string note3 = ((num20 > 0) ? "buyers absorbed over " : "sellers absorbed over ") + num + " bars (net delta " + FmtS(Win(abs, num, Pf.Delta)) + ")";
			Add(abs, SigKind.Absorption, num20 < 0, score2, reference.R, reference.E, pending, note3);
		}
	}

	private void RemovePending(int abs)
	{
		int num = _sigs.Count - 1;
		while (num >= 0 && (_sigs[num].Pending || _sigs[num].Abs >= abs))
		{
			if (_sigs[num].Pending || _sigs[num].Abs == abs)
			{
				_sigs.RemoveAt(num);
				_sigsVersion++;   // P4 F18
			}
			num--;
		}
	}

	private bool IsResetBetween(int from, int to)
	{
		if (to >= 0 && to < _b.Length)
		{
			return _b[to].Seg > from;
		}
		return false;
	}

	private void AddSig(Sig sg)
	{
		for (int num = _sigs.Count - 1; num >= 0; num--)
		{
			Sig sig = _sigs[num];
			if (sg.Abs - sig.Abs > SignalGap)
			{
				break;
			}
			if (sig.Kind == sg.Kind && sig.Bull == sg.Bull && !sig.Pending && sig.P2 == sg.P2)
			{
				if (sig.Score >= sg.Score)
				{
					return;
				}
				_sigs.RemoveAt(num);
				_sigsVersion++;   // P4 F18
			}
		}
		if (KeepFirstSignal)
		{
			Freeze(sg);   // 2.0 (F2)
		}
		_sigs.Add(sg);
		_sigsVersion++;   // P4 F18
		if (_sigs.Count > 4000)
		{
			_sigs.RemoveRange(0, _sigs.Count - 4000);
		}
	}

	private void Add(int abs, SigKind k, bool bull, double score, double R, double E, bool pending, string note)
	{
		if (KeepFirstSignal)
		{
			// 2.0 (plan F2): a marker on a closed bar never moves or disappears. While the same kind and side fires again
			// within SignalGap bars of the last time it fired, it is the same event: the marker stays on its first bar, its
			// score becomes the strongest reading and it remembers the last bar. (Classic: the earlier marker was deleted,
			// so a flow divergence's marker walked forward every bar and history never showed what was seen live.)
			for (int i = _sigs.Count - 1; i >= 0; i--)
			{
				Sig s = _sigs[i];
				if (abs - s.Abs > 5000)
				{
					break;
				}
				if (s.Kind == k && s.Bull == bull && !s.Pending && s.P1 < 0 && abs - Math.Max(s.Seen, s.Abs) <= SignalGap)
				{
					if (!pending && (score > s.Score || abs > s.Seen))
					{
						_sigs[i] = s.Again(score, abs);
						_sigsVersion++;
					}
					return;
				}
			}
			Sig sig0 = new Sig
			{
				Abs = abs,
				Kind = k,
				Bull = bull,
				Score = score,
				R = R,
				E = E,
				Pending = pending,
				Note = note,
				Seen = abs
			};
			Freeze(sig0);
			_sigsVersion++;
			_sigs.Add(sig0);
			if (_sigs.Count > 4000)
			{
				_sigs.RemoveRange(0, _sigs.Count - 4000);
			}
			return;
		}
		for (int num = _sigs.Count - 1; num >= 0; num--)
		{
			Sig sig = _sigs[num];
			if (abs - sig.Abs > SignalGap)
			{
				break;
			}
			if (sig.Kind == k && sig.Bull == bull && !sig.Pending && sig.P1 < 0)
			{
				if (sig.Score >= score)
				{
					return;
				}
				_sigs.RemoveAt(num);
				_sigsVersion++;   // P4 F18
			}
		}
		_sigsVersion++;   // P4 F18
		_sigs.Add(new Sig
		{
			Abs = abs,
			Kind = k,
			Bull = bull,
			Score = score,
			R = R,
			E = E,
			Pending = pending,
			Note = note
		});
		if (_sigs.Count > 4000)
		{
			_sigs.RemoveRange(0, _sigs.Count - 4000);
		}
	}

	private double AvgScore(Sig g)
	{
		double avg = AvgScoreIn(_psigs, g.Abs, g.Kind, g.P1 >= 0);   // P4 F18: only the picture uses this, on its copy
		if (avg <= 0.0 && SignalAvgFallback)
		{
			avg = _pKindAvg[KindSlot(g)];   // 2.3: the all-history average of the kind (0 while fewer than 3)
		}
		return avg;
	}

	// the average score of the closed signals of one kind (swing divergences apart) in the LookbackBars before bar abs; 0
	// when there are fewer than 3
	private double AvgScoreIn(List<Sig> list, int abs, SigKind kind, bool swing)
	{
		double num = 0.0;
		int num2 = 0;
		// 2.0 (S1): the list is in bar order (every signal is stored on the bar it was evaluated on), so start at the last
		// signal before abs instead of walking every newer one (the same signals in the same order: the same number)
		int lo = 0, hi = list.Count;
		while (lo < hi)
		{
			int mid = (lo + hi) >> 1;
			if (list[mid].Abs < abs) lo = mid + 1; else hi = mid;
		}
		for (int num3 = lo - 1; num3 >= 0; num3--)
		{
			Sig sig = list[num3];
			if (abs - sig.Abs > LookbackBars)
			{
				break;
			}
			if (!sig.Pending && sig.Kind == kind && sig.P1 >= 0 == swing && sig.Abs < abs)
			{
				num += sig.Score;
				num2++;
			}
		}
		if (num2 < 3)
		{
			return 0.0;
		}
		return num / (double)num2;
	}

	// 2.0 (plan F2): the filter's answer for a new signal, from the signals known before it (the rule of Passes), kept with
	// the signal: what the chart showed live is what it shows after a reload
	private void Freeze(Sig s)
	{
		double avg = AvgScoreIn(_sigs, s.Abs, s.Kind, s.P1 >= 0);
		int slot = KindSlot(s);
		if (avg <= 0.0 && SignalAvgFallback && _kindCnt[slot] >= 3)
		{
			// 2.3: fewer than 3 signals of this kind in the lookback (the usual case: with the gap merging repeats, three in 60
			// bars is rare), so the filter had nothing to compare with and every setting showed every signal. The average of
			// all the signals of this kind seen so far on the chart is the reference then. Still frozen with the signal.
			avg = _kindSum[slot] / (double)_kindCnt[slot];
		}
		s.Avg0 = avg;
		s.AtAvg = (avg <= 0.0 || s.Score >= avg) ? s.Abs : (-1);
		s.AtExt = (avg <= 0.0 || s.Score >= 1.5 * avg) ? s.Abs : (-1);
		if (!s.Pending)
		{
			_kindSum[slot] += s.Score;
			_kindCnt[slot]++;
		}
	}

	// 2.3: the signal kind's slot in the running averages (swing divergences apart from flow divergences)
	private static int KindSlot(Sig s)
	{
		return (int)s.Kind * 2 + ((s.P1 >= 0) ? 1 : 0);
	}

	private readonly double[] _kindSum = new double[6];   // 2.3: sum and count of the closed signals' scores per kind, all history

	private readonly int[] _kindCnt = new int[6];

	private readonly double[] _kindSumBefore = new double[6];   // 2.3: the F6 snapshot of the two above

	private readonly int[] _kindCntBefore = new int[6];

	private readonly double[] _pKindAvg = new double[6];   // 2.3: the picture's copy (classic path), made in CopyForPaint

	// 2.0 (plan F2): the bar a marker is drawn on, or -1 when the signal filter hides it. Classic signals ask the filter at
	// every picture with the scores as they are now, so a marker could appear late on an older bar or vanish. A 2.0 signal
	// keeps the answer it got when it was known: drawn from the bar on which it reached its filter's level, never removed
	private int DrawBar(Sig g)
	{
		if (g.Avg0 < 0.0)
		{
			return Passes(g) ? g.Abs : (-1);
		}
		_lastAvg = g.Avg0;
		return (Filter == SignalFilter.All) ? g.Abs : ((Filter == SignalFilter.AboveAverage) ? g.AtAvg : g.AtExt);
	}

	private bool Passes(Sig g)
	{
		_lastAvg = 0.0;
		if (Filter == SignalFilter.All)
		{
			return true;
		}
		double num = AvgScore(g);
		_lastAvg = num;
		if (num <= 0.0)
		{
			return true;
		}
		if (Filter != SignalFilter.AboveAverage)
		{
			return g.Score >= 1.5 * num;
		}
		return g.Score >= num;
	}

	private void Publish(int abs)
	{
		int num = Off(abs);
		if (num >= 0)
		{
			ref Bar reference = ref _b[abs];
			_line.SetValue(reference.C, num);
			if (Mode == Display.Line)
			{
				_line.SetMarker(num, new IndicatorLineMarker((reference.C >= reference.O) ? BullColor : BearColor));
			}
			if (Mode == Display.Candles)
			{
				_hi.SetValue(reference.H, num);
				_lo.SetValue(reference.L, num);
			}
			else if (Mode == Display.DeltaBars)
			{
				_hi.SetValue(Math.Max(0.0, reference.Delta), num);
				_lo.SetValue(Math.Min(0.0, reference.Delta), num);
			}
			else
			{
				_hi.SetValue(reference.C, num);
				_lo.SetValue(reference.C, num);
			}
		}
	}

	private bool IsReset(int abs)
	{
		if (abs <= 0)
		{
			return true;
		}
		switch (Reset)
		{
		case ResetMode.Continuous:
			return false;
		case ResetMode.Bars:
			if (ResetBars > 0)
			{
				if (ResetBarsFromSession)
				{
					return _b[abs].SessIdx % ResetBars == 0;   // 2.0 (F5)
				}
				return abs % ResetBars == 0;
			}
			return false;
		case ResetMode.Day:
			return ChartDate(abs) != ChartDate(abs - 1);
		case ResetMode.Week:
		{
			DateTime date = ChartDate(abs);
			DateTime date2 = ChartDate(abs - 1);
			if (ISOWeek.GetWeekOfYear(date) == ISOWeek.GetWeekOfYear(date2))
			{
				return ISOWeek.GetYear(date) != ISOWeek.GetYear(date2);
			}
			return true;
		}
		case ResetMode.Session:
			return _b[abs].Anchor != _b[abs - 1].Anchor;
		default:
			return false;
		}
	}

	private TimeZoneInfo ChartTz()
	{
		try
		{
			TimeZoneInfo timeZoneInfo = base.CurrentChart?.CurrentTimeZone.TimeZoneInfo;
			if (timeZoneInfo != null)
			{
				return timeZoneInfo;
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "ChartTz#1", tnxError);   // P4 F5: was empty
		}
		try
		{
			return Core.Instance.TimeUtils.SelectedTimeZone.TimeZoneInfo;
		}
		catch
		{
			return null;
		}
	}

	private DateTime ChartDate(int abs)
	{
		DateTime dateTime = new DateTime(_b[abs].Ticks, DateTimeKind.Utc);
		try
		{
			TimeZoneInfo timeZoneInfo = ChartTz();
			return ((timeZoneInfo == null) ? dateTime : TimeZoneInfo.ConvertTimeFromUtc(dateTime, timeZoneInfo)).Date;
		}
		catch
		{
			return dateTime.Date;
		}
	}

	private long SessionAnchor(int abs)
	{
		long result = ((abs > 0) ? _b[abs - 1].Anchor : 0);
		try
		{
			SessionsContainer sessionsContainer = base.Symbol?.CurrentSessionsInfo;
			if (sessionsContainer == null)
			{
				return ChartDate(abs).Ticks;
			}
			if (!_hasPrimary.HasValue)
			{
				_hasPrimary = sessionsContainer.ActiveSessions?.Any((ISession s) => s.IsPrimary) ?? false;
			}
			if (_hasPrimary != true)
			{
				return ChartDate(abs).Ticks;
			}
			long ticks = _b[abs].Ticks;
			long num = ticks - ticks % 864000000000L;
			if (!_sessionCache.TryGetValue(num, out var value))
			{
				DateTime dateTime = new DateTime(num, DateTimeKind.Utc);
				value = sessionsContainer.EnumerateSessionTimeFrames(dateTime.AddDays(-1.0), dateTime.AddDays(1.0)).ToArray();
				if (_sessionCache.Count > 400)
				{
					_sessionCache.Clear();
				}
				_sessionCache[num] = value;
			}
			for (int num2 = 0; num2 < value.Length; num2++)
			{
				if (value[num2].From.Ticks <= ticks && ticks <= value[num2].To.Ticks)
				{
					return value[num2].From.Ticks;
				}
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "SessionAnchor#1", tnxError);   // P4 F5: was empty
		}
		return result;
	}

	public override void OnPaintChart(PaintChartEventArgs args)
	{
		base.OnPaintChart(args);
		if (base.CurrentChart == null || args?.Graphics == null || _n == 0)
		{
			return;
		}
		IChartWindow chartWindow = null;
		IChartWindow[] windows = base.CurrentChart.Windows;
		foreach (IChartWindow chartWindow2 in windows)
		{
			if (chartWindow2.WindowNumber == args.WindowIndex)
			{
				chartWindow = chartWindow2;
				break;
			}
		}
		IChartWindowCoordinatesConverter chartWindowCoordinatesConverter = chartWindow?.CoordinatesConverter;
		if (chartWindowCoordinatesConverter == null)
		{
			return;
		}
		_winNum = chartWindow.WindowNumber;
		if ((Mode == Display.Velocity && LaneMouse) || HoverCards)   // 2.0: the signal cards need the mouse in every mode
		{
			HookMouse();
		}
		Graphics graphics = args.Graphics;
		GraphicsState gstate = graphics.Save();
		try
		{
			lock (_paintSync)
			{
				lock (_sync)
				{
					CopyForPaint();   // P4 F18: the live data is locked only for this copy, not for the whole picture
				}
				_arrays.BeginPaint();
				PaintCore(graphics, args, chartWindowCoordinatesConverter);
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnPaintChart#1", tnxError);   // P4 F5: was empty
		}
		finally
		{
			graphics.Restore(gstate);
		}
	}

	// P4 F18 (under _sync): bring the picture's copy up to date. Only the bars Compute wrote since the last copy are
	// copied (normally the last one or two) and the signal list only when it changed, so this takes microseconds.
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
			_pBarHeight = _barHeight;
			_pChartTag = _chartTag;   // 2.0
			if (from <= _lastBarKey.Item1)
			{
				_lastBarSpeed = null;   // 2.0: the card's last-bar texts are made again when that bar or one before it changed
			}
			_dirtyFrom = int.MaxValue;
		}
		if (_psigsVersion != _sigsVersion)
		{
			_psigs.Clear();
			_psigs.AddRange(_sigs);
			_psigsVersion = _sigsVersion;
			// 2.3: the all-history average score per kind for the classic filter path (only when the list changed)
			Span<double> sum = stackalloc double[6];
			Span<int> cnt = stackalloc int[6];
			for (int i = 0; i < _psigs.Count; i++)
			{
				Sig sg = _psigs[i];
				if (!sg.Pending)
				{
					int k = KindSlot(sg);
					sum[k] += sg.Score;
					cnt[k]++;
				}
			}
			for (int k = 0; k < 6; k++)
			{
				_pKindAvg[k] = ((cnt[k] >= 3) ? (sum[k] / (double)cnt[k]) : 0.0);
			}
		}
	}

	private void PaintCore(Graphics g, PaintChartEventArgs args, IChartWindowCoordinatesConverter conv)
	{
		Rectangle rectangle = args.Rectangle;
		_clipTitleW = TitleAreaWidth;
		PaintClip(g, rectangle);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
		Fonts();
		int num = Math.Min(base.Count, _pn);
		if (!XMap(conv, base.CurrentChart.BarsWidth, num, out var x, out var i, out var slot))
		{
			_hitList.Clear();   // 2.3: no markers in this picture, so no hover boxes from an older one
			if (Table != Corner.Hidden)
			{
				DrawTable(g, rectangle, (LaneClosed && num >= 2) ? (num - 2) : (num - 1));
			}
			return;
		}
		int vL = Math.Max(0, (int)Math.Floor(((double)rectangle.Left - x) / slot) + i - 1);
		int vR = Math.Min(num - 1, (int)Math.Ceiling(((double)rectangle.Right - x) / slot) + i + 1);
		bool laneTitles = false;
		switch (Mode)
		{
		case Display.Candles:
			Candles(g, conv, vL, vR, x, i, slot);
			break;
		case Display.DeltaBars:
			DeltaBars(g, conv, vL, vR, x, i, slot);
			break;
		case Display.Velocity:
			VelocityPanel(g, conv, rectangle, vL, vR, x, i, slot);
			laneTitles = true;
			break;
		}
		if (Mode == Display.Velocity && _spTop > (float)(rectangle.Top + 10))
		{
			g.SetClip(new RectangleF(rectangle.Left, rectangle.Top, rectangle.Width, Math.Max(1f, _spTop - 6f - (float)rectangle.Top)), CombineMode.Intersect);
			Signals(g, conv, rectangle, vL, vR, x, i, slot);
			PaintClip(g, rectangle);   // P4: was a copy of the clip (Graphics.Clip, a new Region every picture), put back
		}
		else
		{
			Signals(g, conv, rectangle, vL, vR, x, i, slot);
		}
		if (Table != Corner.Hidden)
		{
			DrawTable(g, rectangle, (LaneClosed && num >= 2) ? (num - 2) : (num - 1));
		}
		if (laneTitles)
		{
			DrawLaneTitles(g, rectangle);   // 2.2: after the card, so they know where it is in this picture
		}
		if (HoverCards)
		{
			HoverCard(g, rectangle);
		}
	}

	// 2.0: the info table: the new card, or the classic table exactly as 1.x
	private void DrawTable(Graphics g, Rectangle rect, int last)
	{
		if (TableStyle == TableLook.Card)
		{
			PanelCard(g, rect, last);
		}
		else
		{
			Panel(g, rect, last);
		}
	}

	// P4 (§5.6: no GDI+ object made per picture): the clip PaintCore starts with, the window minus the title area.
	// Velocity mode narrows it twice and used to put it back from a copy (Graphics.Clip makes a new Region on every
	// read); setting it again the same way gives the same region, so the same pixels are drawn.
	private void PaintClip(Graphics g, Rectangle rect)
	{
		g.SetClip(rect);
		if (_clipTitleW > 0)
		{
			g.ExcludeClip(new Rectangle(rect.Left, rect.Top, Math.Min(rect.Width, _clipTitleW), 24));
		}
	}

	private bool XMap(IChartWindowCoordinatesConverter conv, int barsWidth, int n, out double x0, out int i0, out double slot)
	{
		x0 = 0.0;
		i0 = -1;
		slot = Math.Max(1, barsWidth);
		if (n <= 0)
		{
			return false;
		}
		int num = 0;
		double num2 = 0.0;
		double num3 = 0.0;
		int num4 = -1;
		int num5 = -1;
		int num6 = n - 1;
		int num7 = Math.Max(0, n - 600);
		while (num6 >= num7 && num < 2)
		{
			long ticks = _pb[num6].Ticks;
			if ((num6 <= 0 || _pb[num6 - 1].Ticks != ticks) && (num6 >= n - 1 || _pb[num6 + 1].Ticks != ticks))
			{
				DateTime dt = new DateTime(ticks, DateTimeKind.Utc);
				double barIndex = conv.GetBarIndex(dt);
				if (!double.IsNaN(barIndex) && Math.Abs(barIndex - Math.Round(barIndex)) < 0.02 && Math.Abs(barIndex - (double)num6) < 0.5)
				{
					double chartX = conv.GetChartX(dt);
					if (num == 0)
					{
						num2 = chartX;
						num4 = num6;
						num = 1;
						num6 -= 25;
						continue;
					}
					num3 = chartX;
					num5 = num6;
					num = 2;
					break;
				}
			}
			num6--;
		}
		if (num == 0)
		{
			return false;
		}
		x0 = num2;
		i0 = num4;
		if (num == 2 && num4 > num5)
		{
			double num8 = (num2 - num3) / (double)(num4 - num5);
			if (num8 > 0.1 && Math.Abs(num8 - slot) <= slot * 0.35 + 0.5)
			{
				slot = num8;
			}
		}
		return true;
	}

	private float X(int i, double x0, int i0, double slot)
	{
		return (float)(x0 + (double)(i - i0) * slot + slot * 0.5);
	}

	private void Candles(Graphics g, IChartWindowCoordinatesConverter conv, int vL, int vR, double x0, int i0, double slot)
	{
		float num = (float)Math.Max(1.0, Math.Min(slot - 2.0, slot * 0.72));
		float w = Math.Max(1f, (float)LineWidth * 0.6f);
		if (CrispCandles)
		{
			CandlesCrisp(g, conv, vL, vR, x0, i0, slot, num, cvdScale: false, xMax: float.MaxValue);
			return;
		}
		for (int j = vL; j <= vR; j++)
		{
			ref Bar reference = ref _pb[j];
			if (reference.Has)
			{
				float num2 = X(j, x0, i0, slot);
				float num3 = (float)conv.GetChartY(reference.O);
				float num4 = (float)conv.GetChartY(reference.C);
				float y = (float)conv.GetChartY(reference.H);
				float y2 = (float)conv.GetChartY(reference.L);
				bool flag = reference.C >= reference.O;
				Color color = (flag ? BullColor : BearColor);
				if (!reference.Real)
				{
					color = Color.FromArgb(120, color);
				}
				g.DrawLine(Pen(color, w), num2, y, num2, y2);
				float num5 = Math.Min(num3, num4);
				float num6 = Math.Max(1f, Math.Abs(num4 - num3));
				if (num >= 3f)
				{
					g.FillRectangle(Brush(Color.FromArgb(flag ? 150 : 200, color)), num2 - num / 2f, num5, num, num6);
					g.DrawRectangle(Pen(color, w), num2 - num / 2f, num5, num, num6);
				}
				else
				{
					g.DrawLine(Pen(color, num), num2, num5, num2, num5 + num6);
				}
			}
		}
	}

	// 2.0: the CVD candles on whole pixels, without anti-aliasing: sharp edges, an odd body width centred on its wick, the
	// same colours and alphas as before; GDI+ draws such rectangles several times faster than smoothed ones
	private void CandlesCrisp(Graphics g, IChartWindowCoordinatesConverter conv, int vL, int vR, double x0, int i0, double slot, float num, bool cvdScale, float xMax)
	{
		SmoothingMode sm = g.SmoothingMode;
		g.SmoothingMode = SmoothingMode.None;
		int body = CrispBody(num, slot);
		int wick = (LineWidth >= 3) ? 2 : 1;
		float lo = -20000f;
		float hi = 20000f;
		for (int j = vL; j <= vR; j++)
		{
			ref Bar reference = ref _pb[j];
			if (!reference.Has)
			{
				continue;
			}
			float xf = X(j, x0, i0, slot);
			if (xf > xMax)
			{
				break;
			}
			int xc = (int)Math.Floor(xf);
			float fo = Math.Max(lo, Math.Min(hi, cvdScale ? CvdY(reference.O) : (float)conv.GetChartY(reference.O)));
			float fc = Math.Max(lo, Math.Min(hi, cvdScale ? CvdY(reference.C) : (float)conv.GetChartY(reference.C)));
			float fh = Math.Max(lo, Math.Min(hi, cvdScale ? CvdY(reference.H) : (float)conv.GetChartY(reference.H)));
			float fl = Math.Max(lo, Math.Min(hi, cvdScale ? CvdY(reference.L) : (float)conv.GetChartY(reference.L)));
			bool up = reference.C >= reference.O;
			Color color = (up ? BullColor : BearColor);
			if (!reference.Real)
			{
				color = Color.FromArgb(120, color);
			}
			int yTop = (int)Math.Round(Math.Min(fh, fl));
			int yBot = (int)Math.Round(Math.Max(fh, fl));
			g.FillRectangle(Brush(color), xc - wick / 2, yTop, wick, Math.Max(1, yBot - yTop + 1));
			int bTop = (int)Math.Round(Math.Min(fo, fc));
			int bH = Math.Max(1, (int)Math.Round(Math.Max(fo, fc)) - bTop + 1);
			int bx = xc - body / 2;
			if (body >= 3)
			{
				if (bH >= 3)
				{
					g.FillRectangle(Brush(Color.FromArgb(up ? 150 : 200, color)), bx + 1, bTop + 1, body - 2, bH - 2);
					g.DrawRectangle(Pen(color, 1f), bx, bTop, body - 1, bH - 1);
				}
				else
				{
					g.FillRectangle(Brush(color), bx, bTop, body, bH);
				}
			}
			else
			{
				g.FillRectangle(Brush(color), bx, bTop, body, bH);
			}
		}
		g.SmoothingMode = sm;
	}

	// 2.1: the crisp candle's body: an odd number of pixels (centred on its wick) as close to num as possible (the larger
	// one on a tie), at least 3 px when the bar slot is 4 px or more, always a pixel between two candles. (2.0 rounded down:
	// at a 4 px slot the body was 1 px, a line like the wick)
	private static int CrispBody(float num, double slot)
	{
		int lo = Math.Max(1, ((int)Math.Floor(num) - 1) / 2 * 2 + 1);
		int hi = lo + 2;
		int body = (((float)hi - num <= num - (float)lo) ? hi : lo);
		int room = (int)Math.Floor(slot - 1.0);
		if (room % 2 == 0)
		{
			room--;
		}
		room = Math.Max(1, room);
		body = Math.Min(body, room);
		if (slot >= 4.0)
		{
			body = Math.Max(body, Math.Min(3, room));
		}
		return body;
	}

	private void DeltaBars(Graphics g, IChartWindowCoordinatesConverter conv, int vL, int vR, double x0, int i0, double slot)
	{
		float num = (float)Math.Max(1.0, Math.Min(slot - 2.0, slot * 0.72));
		float num2 = (float)conv.GetChartY(0.0);
		for (int j = vL; j <= vR; j++)
		{
			ref Bar reference = ref _pb[j];
			if (reference.Has)
			{
				float num3 = X(j, x0, i0, slot);
				float num4 = (float)conv.GetChartY(reference.Delta);
				Color baseColor = ((reference.Delta >= 0.0) ? BullColor : BearColor);
				if (!reference.Real)
				{
					baseColor = Color.FromArgb(120, baseColor);
				}
				g.FillRectangle(Brush(Color.FromArgb(180, baseColor)), num3 - num / 2f, Math.Min(num4, num2), num, Math.Max(1f, Math.Abs(num4 - num2)));
			}
		}
	}

	private void VelocityPanel(Graphics g, IChartWindowCoordinatesConverter conv, Rectangle rect, int vL, int vR, double x0, int i0, double slot)
	{
		_ltFlow.On = false;   // 2.2: no title pill unless a lane asks for one in this picture
		_ltAcc.On = false;
		int num = vR - vL + 1;
		if (num <= 0)
		{
			return;
		}
		int num2 = Math.Max(2, VelocityPeriod / 3);
		int num3 = Math.Max(3, VelocityPeriod);
		bool showAccel = ShowAccel;
		float num4 = Math.Max(60f, rect.Height);
		float val = ((_spHpx > 0f) ? _spHpx : Math.Max(34f, num4 * (float)Math.Max(10, Math.Min(80, VelocityPanelHeight)) / 100f));
		float num5 = ((!showAccel) ? 0f : ((_acHpx > 0f) ? _acHpx : Math.Max(26f, num4 * (float)Math.Max(8, Math.Min(50, AccelHeight)) / 100f)));
		val = Math.Max(24f, val);
		if (showAccel)
		{
			num5 = Math.Max(18f, num5);
		}
		float num6 = val + num5 + 6f * (float)((!showAccel) ? 1 : 2) + 3f;
		if (LaneStyle == LaneLook.Pro)
		{
			// 2.2: the three panes sized like sub-windows: shares of the window with a minimum each; a big share takes only
			// the room above the other panes' minimums (1.x: a big acceleration strip squeezed the CVD area to 37 px)
			PaneHeights(rect.Height, showAccel, out val, out num5);
			num6 = val + num5 + 6f * (float)((!showAccel) ? 1 : 2) + 3f;
		}
		else if (num6 > num4 * 0.85f)
		{
			float num7 = num4 * 0.85f / num6;
			val *= num7;
			num5 *= num7;
			num6 = val + num5 + 6f * (float)((!showAccel) ? 1 : 2) + 3f;
		}
		float num8 = (float)rect.Bottom - 3f;
		float num9 = num8 - num5;
		float num10 = (showAccel ? (num9 - 6f) : num8);
		float num11 = (_spTop = (_laneTop = num10 - val));
		_spBot = num10;
		_acTop = num9;
		_acBot = num8;
		_winRect = rect;
		Pen pen = Pen(Color.FromArgb(255, 44, 48, 56), 1f);
		g.DrawLine(pen, rect.Left, num11 - 3f, rect.Right, num11 - 3f);
		if (showAccel)
		{
			g.DrawLine(pen, rect.Left, num9 - 3f, rect.Right, num9 - 3f);
		}
		_reserveFrac = Math.Min(0.8, (num6 + 6f) / num4);
		_spRect = new RectangleF(rect.Left, num11, rect.Width, num10 - num11);
		_acRect = (showAccel ? new RectangleF(rect.Left, num9, rect.Width, num8 - num9) : RectangleF.Empty);
		if (!showAccel)
		{
			_laneAccScale = 0.0;   // 2.3: no strip in this picture (the test machine reads these)
			_laneAccHalf = (_laneAccZero = (_laneAccTop = (_laneAccBot = 0f)));
		}
		Color color = (VelocityUseTheme ? BullColor : VelocityUp);
		Color color2 = (VelocityUseTheme ? BearColor : VelocityDown);
		float num12 = (float)Math.Max(1.5, Math.Min(slot - 1.5, slot * 0.66));
		float num13 = ((ShowLaneText && LaneStyle != LaneLook.Pro) ? _capH : 2f);   // 2.2: Pro has no caption rows
		float num14 = (ShowLaneText ? _axisW : 0f);
		float num15 = (_axisX = (float)rect.Right - num14);
		SolidBrush brush = Brush(Color.FromArgb(200, 150, 158, 175));
		int num16 = ((LaneClosed && vR == Math.Min(base.Count, _pn) - 1) ? Math.Max(0, vR - 1) : vR);
		float num17 = num11 - 6f;
		float num18 = (float)rect.Top + _capH + 6f;
		float num19 = num17 - 4f;
		double num20 = double.MaxValue;
		double num21 = double.MinValue;
		for (int j = vL; j <= vR; j++)
		{
			if (!_pb[j].Has)
			{
				continue;
			}
			if (VelocityStyle == VelocityCvdStyle.Candles)
			{
				if (_pb[j].L < num20)
				{
					num20 = _pb[j].L;
				}
				if (_pb[j].H > num21)
				{
					num21 = _pb[j].H;
				}
				continue;
			}
			double c = _pb[j].C;
			if (c < num20)
			{
				num20 = c;
			}
			if (c > num21)
			{
				num21 = c;
			}
		}
		if (num21 >= num20)
		{
			_cvMid = (num18 + num19) / 2f;
			_cvHalfPx = Math.Max(4f, (num19 - num18) / 2f);
			// 2.2: the newest visible candle: a stretch grows around it, MANUAL keeps it in view
			int newest = vR;
			while (newest > vL && !_pb[newest].Has)
			{
				newest--;
			}
			bool hasNewest = _pb[newest].Has;
			if (hasNewest)
			{
				_cvAnchor = _pb[newest].C;
			}
			_cvRefHalf = Math.Max(1E-09, (num21 - num20) / 2.0 * 1.08);
			_cvBandTop = num18;
			_cvBandBot = num19;
			if (_cvAuto)
			{
				_cvOff = 0f;
				int num22 = Math.Max(vL, vR - 25);
				double num23 = double.MaxValue;
				double num24 = double.MinValue;
				double num25 = 0.0;
				int num26 = 0;
				for (int k = num22; k <= vR; k++)
				{
					if (_pb[k].Has)
					{
						double num27 = ((VelocityStyle == VelocityCvdStyle.Candles) ? _pb[k].L : _pb[k].C);
						double num28 = ((VelocityStyle == VelocityCvdStyle.Candles) ? _pb[k].H : _pb[k].C);
						if (num27 < num23)
						{
							num23 = num27;
						}
						if (num28 > num24)
						{
							num24 = num28;
						}
						num25 += Math.Max(1E-06, num28 - num27);
						num26++;
					}
				}
				if (num24 < num23)
				{
					num23 = num20;
					num24 = num21;
				}
				double num29 = ((num26 > 0) ? (num25 / (double)num26) : 10.0);
				double num30 = Math.Max(Math.Max(num24 - num23, num29 * 3.5) * 1.25, (num21 - num20) * 0.72);
				_cvCenter = (num23 + num24) / 2.0;
				_cvHalf = Math.Max(1E-06, num30 / 2.0);
			}
			else
			{
				// 2.2: MANUAL = the view the user stretched, and the newest candle never leaves it (Quantower's "Keep in
				// view", Sierra Chart's constant range). 1.x fitted the whole visible range times the zoom around its
				// middle: a stretch pushed the newest candle out of the pane (30 px of stretch: 9 px below it, 120 px:
				// 640 px), and the switch from AUTO jumped to another view. Now MANUAL starts from the view on the screen.
				if (!_cvMSet)
				{
					_cvMCenter = _cvCenter;   // the last picture's view: no jump
					_cvMHalf = Math.Max(1E-09, _cvHalf);
					_cvMSet = true;
				}
				_cvOff = 0f;
				double mc = _cvMCenter;
				double mh = Math.Max(1E-09, _cvMHalf);
				// 2.2: never while the user moves the view (it follows the mouse exactly, up and down, as far as he wants), and
				// only when he left the newest candle in view: a view he moved away from it stays where he put it (Quantower's
				// Manual). A first 2.2 kept the newest candle in view always: a drag inside stopped after about 30 px.
				if (hasNewest && _cvFollow && !_drag && !_soft)
				{
					float m = Math.Min(10f, (num19 - num18) * 0.12f);
					float ya = _cvMid - (float)((_cvAnchor - mc) / mh * (double)_cvHalfPx);
					if (ya < num18 + m)
					{
						mc = _cvAnchor - (double)((_cvMid - (num18 + m)) / _cvHalfPx) * mh;
					}
					else if (ya > num19 - m)
					{
						mc = _cvAnchor - (double)((_cvMid - (num19 - m)) / _cvHalfPx) * mh;
					}
				}
				_cvCenter = mc;
				_cvHalf = mh;
				if (!_drag && !_soft)
				{
					_cvMCenter = mc;   // the view stays where it followed the newest candle to
				}
			}
			_cvOk = true;
		}
		_cvRect = new RectangleF(rect.Left, rect.Top, rect.Width, Math.Max(1f, num17 - (float)rect.Top));
		g.SetClip(new RectangleF(rect.Left, rect.Top, Math.Max(1f, num15 - (float)rect.Left), Math.Max(1f, num17 - (float)rect.Top)), CombineMode.Intersect);
		if (VelocityStyle == VelocityCvdStyle.Candles && CrispCandles)
		{
			CandlesCrisp(g, conv, vL, vR, x0, i0, slot, (float)Math.Max(1.5, Math.Min(slot - 2.0, slot * 0.72)), cvdScale: true, xMax: num15);   // 2.0
		}
		else if (VelocityStyle == VelocityCvdStyle.Candles)
		{
			float num31 = (float)Math.Max(1.5, Math.Min(slot - 2.0, slot * 0.72));
			float w = Math.Max(1f, (float)LineWidth * 0.6f);
			for (int l = vL; l <= vR; l++)
			{
				ref Bar reference = ref _pb[l];
				if (reference.Has)
				{
					float num32 = X(l, x0, i0, slot);
					if (num32 > num15)
					{
						break;
					}
					float num33 = CvdY(reference.O);
					float num34 = CvdY(reference.C);
					float y = CvdY(reference.H);
					float y2 = CvdY(reference.L);
					bool flag = reference.C >= reference.O;
					Color color3 = (flag ? BullColor : BearColor);
					if (!reference.Real)
					{
						color3 = Color.FromArgb(120, color3);
					}
					g.DrawLine(Pen(color3, w), num32, y, num32, y2);
					float num35 = Math.Min(num33, num34);
					float num36 = Math.Max(1f, Math.Abs(num34 - num33));
					if (num31 >= 3f)
					{
						g.FillRectangle(Brush(Color.FromArgb(flag ? 150 : 200, color3)), num32 - num31 / 2f, num35, num31, num36);
						g.DrawRectangle(Pen(color3, w), num32 - num31 / 2f, num35, num31, num36);
					}
					else
					{
						g.DrawLine(Pen(color3, num31), num32, num35, num32, num35 + num36);
					}
				}
			}
		}
		else
		{
			Color baseColor = (VelocityUseTheme ? SignalColor : VelocityCvd);
			PointF[] array = _arrays.Get<PointF>(1, num);
			int num37 = 0;
			for (int m = vL; m <= vR; m++)
			{
				if (_pb[m].Has)
				{
					float num38 = X(m, x0, i0, slot);
					if (num38 > num15)
					{
						break;
					}
					array[num37++] = new PointF(num38, CvdY(_pb[m].C));
				}
			}
			if (num37 >= 2)
			{
				if (num37 < num)
				{
					PointF[] exact = _arrays.Get<PointF>(2, num37);   // P4: the same as Array.Resize (its first points)
					Array.Copy(array, exact, num37);
					array = exact;
				}
				g.DrawLines(Pen(Color.FromArgb(245, baseColor), Math.Max(1.2f, (float)LineWidth * 0.9f)), array);
			}
		}
		PaintClip(g, rect);   // P4: was a copy of the clip (Graphics.Clip, a new Region every picture), put back
		if (ShowLaneText && _cvOk)
		{
			SolidBrush brush2 = Brush(Color.FromArgb(200, 150, 158, 175));
			Pen pen2 = Pen(Color.FromArgb(110, 150, 158, 175), 1f);
			g.DrawLine(Pen(Color.FromArgb(50, 150, 158, 175), 1f), num15, (float)rect.Top + 2f, num15, num17 - 2f);
			double num39 = CvdV(num18);
			double num40 = CvdV(num19);
			double num41 = Math.Abs(num39 - num40);
			int num42 = Math.Max(2, (int)((num19 - num18) / (_capH * 2.6f)));
			double num43 = num41 / (double)num42;
			double num44 = Math.Pow(10.0, Math.Floor(Math.Log10(Math.Max(1E-09, num43))));
			double num45 = ((num43 / num44 < 1.5) ? num44 : ((num43 / num44 < 3.5) ? (2.0 * num44) : ((num43 / num44 < 7.5) ? (5.0 * num44) : (10.0 * num44))));
			for (double num46 = Math.Ceiling(Math.Min(num39, num40) / num45) * num45; num46 <= Math.Max(num39, num40) + 1E-09; num46 += num45)
			{
				float num47 = CvdY(num46);
				if (!(num47 < num18 - 1f) && !(num47 > num19 + 1f))
				{
					g.DrawLine(pen2, num15, num47, num15 + 4f, num47);
					g.DrawString((Math.Abs(num46) < num45 / 2.0) ? "0" : Fmt(num46), _fs, brush2, num15 + 6f, num47 - _capH / 2f, NoWrap);
				}
			}
			g.DrawString("CVD", _fb, Brush(Color.FromArgb(200, 150, 158, 175)), rect.Left + 8, (float)rect.Top + 2f);
			float num48 = CvdY(0.0);
			if (num48 > (float)(rect.Top + 1) && num48 < num17 - 1f)
			{
				g.DrawLine(Pen(Color.FromArgb(60, 150, 158, 175), 1f), rect.Left, num48, num15, num48);
			}
			if (vR >= 0 && _pb[vR].Has)
			{
				float num49 = Math.Max((float)rect.Top + _capH / 2f, Math.Min(num17 - _capH / 2f, CvdY(_pb[vR].C)));
				g.FillRectangle(Brush(Color.FromArgb(235, (_pb[vR].C >= 0.0) ? color : color2)), num15 + 1f, num49 - _capH / 2f, _axisW - 2f, _capH);
				g.DrawString(Fmt(_pb[vR].C), _fs, Brush(Color.FromArgb(255, 10, 12, 16)), num15 + 3f, num49 - _capH / 2f, NoWrap);
			}
			string s = (_cvAuto ? "AUTO" : "MANUAL");
			Color color4 = (_cvAuto ? color : Color.FromArgb(230, 210, 155, 45));
			float num50 = num17 - _capH - ((LaneStyle == LaneLook.Pro) ? 6f : 3f);   // 2.2: clear of the line below it
			float width = _axisW - 2f;
			float height = _capH + 2f;
			_cvAutoBadgeRect = new RectangleF(num15 + 1f, num50, width, height);
			g.FillRectangle(Brush(Color.FromArgb(_cvAuto ? 55 : 35, color4)), num15 + 1f, num50, width, height);
			g.DrawRectangle(Pen(Color.FromArgb(180, color4), 1f), num15 + 1f, num50, width, height);
			g.DrawString(s, _fs, Brush(color4), num15 + 3f, num50 + 1f, NoWrap);
		}
		g.FillRectangle(Brush(LaneBack), rect.Left, num11, rect.Width, num10 - num11);
		// 2.1: the flow lane paints only between its separator lines (the CVD candles above, the acceleration lane below)
		float flowTop = num11 - 2f;
		float flowBot = (showAccel ? num9 : ((float)rect.Bottom));
		LaneClip(g, rect, flowTop, flowBot);
		double num51 = ((FlowScale > 0.0) ? FlowScale : 1E-09);
		if (FlowScale <= 0.0)
		{
			for (int n = vL; n <= vR; n++)
			{
				double num52 = Math.Abs(_pb[n].Fast);
				double num53 = Math.Abs(_pb[n].Slow);
				if (num52 > num51)
				{
					num51 = num52;
				}
				if (num53 > num51)
				{
					num51 = num53;
				}
			}
		}
		num51 *= _spZoom;
		// 2.1: the zero line stays inside its lane. "drag = move" moved it without a limit: out of the lane, its line, area and
		// marks were drawn over the acceleration lane and the CVD candles (1.x did the same)
		_spRoom = Math.Max(0f, (num10 - num11 - num13) / 2f);
		float num54 = num11 + num13 + (num10 - num11 - num13) / 2f + Lim(_spOff, _spRoom);
		float num55 = (num10 - num11 - num13) / 2f - 3f;
		float num56 = num11 + num13;
		float num57 = num10;
		_laneFlowScale = num51;
		_laneFlowHalf = num55;
		_laneFlowZero = num54;
		_laneFlowTop = num56;
		_laneFlowBot = num57;
		g.DrawLine(Pen(Color.FromArgb(70, 150, 158, 175), 1f), rect.Left, num54, num15, num54);
		int num58 = Math.Min(base.Count, _pn);
		if (FlowStyle != FlowLook.Bars && LaneStyle == LaneLook.Pro)
		{
			// 2.3: another look of the same numbers (the Bars look is the per-bar loop and the slow line below)
			FlowStyled(g, rect, vL, vR, x0, i0, slot, num12, num51, num54, num55, num56, num57, num15, color, color2, flowTop, flowBot);
		}
		else
		{
			for (int num59 = vL; num59 <= vR; num59++)
			{
				int num60 = ((LaneClosed && num59 == num58 - 1) ? Math.Max(0, num59 - 1) : num59);
				double fast = _pb[num60].Fast;
				float num61 = ((LaneStyle == LaneLook.Pro) ? ((float)((double)num55 * Math.Abs(fast) / num51)) : ((float)Math.Min((double)num55 * 1.5, (double)num55 * Math.Abs(fast) / num51)));
				float num62 = X(num59, x0, i0, slot);
				if (num62 > num15)
				{
					break;
				}
				bool flag2 = fast >= 0.0;
				double num63 = ScaleC(_pb, num60);
				double num64 = ((num63 > 1E-09) ? (Math.Abs(fast) / num63) : ((num55 > 0f) ? ((double)num61 / (double)num55 * 2.0) : 0.0));
				Color color5 = (flag2 ? color : color2);
				bool highlight = false;
				Color col;
				int num65;
				// classic tiers: 0.4 / 1 / 2 x the typical single-bar |delta|; 2.0 (plan A2): the rank of |fast| among the
				// lookback's bars (Easley et al. 2012 read flow imbalance by its own distribution): < 50 % / 80 % / 95 %
				double rank = (FlowTiers == FlowTierMode.Percentile) ? _pb[num60].FRank : -1.0;   // 2.3: ranked once per bar in Compute (was O(lookback) per bar per picture)
				int tier = (rank >= 0.0) ? ((rank < 0.5) ? 0 : ((rank < 0.8) ? 1 : ((rank < 0.95) ? 2 : 3))) : ((num64 < 0.4) ? 0 : ((num64 < 1.0) ? 1 : ((num64 < 2.0) ? 2 : 3)));
				if (tier == 0)
				{
					col = Dark(color5, 0.4);
					num65 = 130;
				}
				else if (tier == 1)
				{
					col = color5;
					num65 = 195;
				}
				else if (tier == 2)
				{
					col = color5;
					num65 = 245;
					highlight = true;
				}
				else
				{
					col = Bright(color5, 0.22);
					num65 = 255;
					highlight = true;
				}
				if (Shade == FlowShade.RealPush)
				{
					// 2.3: brightness = real push (one-sided AND busy), not the tier: a thin move or a battle stays dark however big
					// its delta; the tier still picks the colour step and the cap
					num65 = (int)(255.0 * (0.26 + 0.74 * _pb[num60].Q));
				}
				if (num59 == num58 - 1)
				{
					num65 = num65 * (LiveReadout ? 9 : 6) / 10;
				}
				// 2.3: the acceleration inside the flow lane (its strip off): a slowing bar is hollow, the marks sit at the bar's end
				bool accelHere = AccelInFlow && !showAccel && LaneStyle == LaneLook.Pro;
				double accN = (accelHere ? AccNorm(num60) : 0.0);
				bool accWith = (_pb[num60].AccE >= 0.0) == flag2;
				bool slowing = accelHere && Math.Abs(accN) >= AccelMin && !accWith;
				if (CrispCandles)
				{
					g.SmoothingMode = SmoothingMode.None;   // 2.0: lane bars on whole pixels (sharper, faster)
				}
				bool proLive = LaneStyle == LaneLook.Pro && num59 == num58 - 1;
				int alphaBar = (proLive ? (num65 * 45 / 100) : (slowing ? Math.Max(40, num65 / 3) : num65));
				HBarTier(g, num62, num54, num12, num61, flag2, col, alphaBar, _pb[num59].Real, num56, num57, highlight, ((rank >= 0.0) ? (tier == 3) : (num64 >= 2.0)) ? Bright(color5, 0.7) : Bright(color5, 0.45));
				if (LaneStyle == LaneLook.Pro && num61 > (flag2 ? (num54 - num56) : (num57 - num54)) + 0.5f)
				{
					// 2.2: the lane is stretched and its edge cuts this bar: a bright cap says so
					g.FillRectangle(Brush(Color.FromArgb(255, Bright(color5, 0.6))), num62 - num12 / 2f, flag2 ? num56 : (num57 - 2f), num12, 2f);
				}
				bool absorbedBar = Shade == FlowShade.RealPush && _pb[num60].Absorbed && !proLive;
				if ((proLive || slowing || absorbedBar) && num61 >= 1f)
				{
					// 2.1: the bar that is forming is drawn hollow: it is still moving. 2.3: a slowing bar too (hollow = losing speed, as
					// in the acceleration strip), and a bar whose own reading is ABSORBED / TRAPPED gets the absorption colour's outline
					float yTop = Math.Max(num56, flag2 ? (num54 - num61) : num54);
					float yBot = Math.Min(num57, flag2 ? num54 : (num54 + num61));
					if (yBot > yTop)
					{
						Color oc = (absorbedBar ? Color.FromArgb(235, AbsorbColor) : Color.FromArgb(230, Bright(color5, 0.35)));
						g.DrawRectangle(Pen(oc, 1f), num62 - num12 / 2f, yTop, num12, yBot - yTop);
					}
				}
				if (CrispCandles)
				{
					g.SmoothingMode = SmoothingMode.AntiAlias;
				}
				if (num59 != num58 - 1 && _pb[num59].Real)
				{
					if (ShowFade && _pb[num60].Fade)
					{
						// 2.3: the push is dying: the flow is still strong but its one-sidedness is dropping. A dot at half the bar.
						float fy = (flag2 ? (num54 - num61 * 0.5f) : (num54 + num61 * 0.5f));
						g.FillEllipse(Brush(Color.FromArgb(255, 10, 12, 16)), num62 - 3.5f, fy - 3.5f, 7f, 7f);
						g.FillEllipse(Brush(Color.FromArgb(245, ExhaustColor)), num62 - 2.5f, fy - 2.5f, 5f, 5f);
					}
					if (accelHere && ShowAccelMarkers && (_pb[num60].Flip || Math.Abs(accN) >= AccelExtreme))
					{
						AccelMarkPro(g, num62, num54, num61, Math.Max(2.5f, Math.Min(5f, (float)slot * 0.35f)), flag2, accWith, Math.Abs(accN) >= AccelExtreme, _pb[num60].Flip, color5, flag2, num56, num57, GateThrust && accWith && !_pb[num60].ThrustOk);
					}
				}
				if (num60 > 0 && num59 < num58 - 1 && _pb[num60 - 1].Has)
				{
					bool num66 = _pb[num60 - 1].Fast >= _pb[num60 - 1].Slow;
					bool flag3 = _pb[num60].Fast >= _pb[num60].Slow;
					if (num66 != flag3)
					{
						Color color6 = (flag3 ? color : color2);
						float val2 = num54 - (float)((double)num55 * _pb[num60].Slow / num51);
						val2 = Math.Max(num56 + 4f, Math.Min(num57 - 4f, val2));
						float num67 = Math.Max(3f, Math.Min(5.5f, num12 * 0.55f));
						g.DrawEllipse(Pen(Color.FromArgb(190, Bright(color6, 0.55)), 1.2f), num62 - num67 - 1f, val2 - num67 - 1f, (num67 + 1f) * 2f, (num67 + 1f) * 2f);
						g.FillEllipse(Brush(Color.FromArgb(245, color6)), num62 - num67, val2 - num67, num67 * 2f, num67 * 2f);
					}
				}
			}
			PointF[] array2 = _arrays.Get<PointF>(3, num);
			int num68 = 0;
			for (int num69 = vL; num69 <= vR; num69++)
			{
				int num70 = ((LaneClosed && num69 == num58 - 1) ? Math.Max(0, num69 - 1) : num69);
				float num71 = X(num69, x0, i0, slot);
				if (num71 > num15)
				{
					break;
				}
				float val3 = num54 - (float)((double)num55 * _pb[num70].Slow / num51);
				array2[num68++] = new PointF(num71, Math.Max(num56, Math.Min(num57, val3)));
			}
			if (num68 >= 2)
			{
				if (num68 < num)
				{
					PointF[] exact2 = _arrays.Get<PointF>(4, num68);   // P4: the same as Array.Resize (its first points)
					Array.Copy(array2, exact2, num68);
					array2 = exact2;
				}
				if (LaneStyle == LaneLook.Pro)
				{
					// 2.1: the slow flow as a soft area (green above zero, red below) under a solid line
					PointF[] area = _arrays.Get<PointF>(6, num68 + 2);
					Array.Copy(array2, area, num68);
					area[num68] = new PointF(array2[num68 - 1].X, num54);
					area[num68 + 1] = new PointF(array2[0].X, num54);
					// the area without smoothing (it is see-through: its edge is drawn by the line), the line smoothed
					g.SmoothingMode = SmoothingMode.None;
					g.SetClip(new RectangleF(rect.Left, num56, num15 - rect.Left, Math.Max(0f, num54 - num56)), CombineMode.Intersect);
					g.FillPolygon(Brush(Color.FromArgb(34, color)), area);
					LaneClip(g, rect, flowTop, flowBot);
					g.SetClip(new RectangleF(rect.Left, num54, num15 - rect.Left, Math.Max(0f, num57 - num54)), CombineMode.Intersect);
					g.FillPolygon(Brush(Color.FromArgb(34, color2)), area);
					LaneClip(g, rect, flowTop, flowBot);
					g.SmoothingMode = SmoothingMode.AntiAlias;
					g.DrawLines(Pen(Color.FromArgb(225, VelocityUseTheme ? Color.FromArgb(235, 238, 245) : VelocityAvg), 1.4f), array2);
				}
				else
				{
					Pen pen3 = DotPen(Color.FromArgb(210, VelocityUseTheme ? Color.White : VelocityAvg), 1.5f);   // 2.0 (S3): kept, was made every picture
					g.DrawLines(pen3, array2);
				}
			}
		}
		if (ShowLaneText)
		{
			ref Bar reference2 = ref _pb[num16];
			if (LaneStyle != LaneLook.Pro)
			{
				g.DrawString("FLOW SPEED  " + num2 + " / " + num3, _fb, brush, rect.Left + 8, num11 + 1f);
			}
			Axis(g, num15, num56, num57, num54, num55, num51, reference2.Fast, (reference2.Fast >= 0.0) ? color : color2, typical: false);
			double sc = ScaleC(_pb, num16);
			if (LaneStyle == LaneLook.Pro)
			{
				string ft = FlowWords(_pb, num16, sc, slow: false, out Color fc);
				if (AccelInFlow && !showAccel && ShowAccelText)
				{
					ft = ft + "  ·  " + AccelWords(reference2.Fast >= 0.0, reference2.AccE, AccNorm(num16), reference2.ThrustOk, out Color _);   // 2.3: the strip is off: its words live here
				}
				double rk = (FlowTiers == FlowTierMode.Percentile) ? _pb[num16].FRank : -1.0;   // 2.3: stored
				int tr = (rk < 0.0) ? -1 : ((rk < 0.5) ? 0 : ((rk < 0.8) ? 1 : ((rk < 0.95) ? 2 : 3)));
				_ltFlow = new LaneTitleAsk { On = true, Y = num11 + 2f, Name = "FLOW " + num2 + " / " + num3, Text = ft, Col = fc, Tier = tr, Side = (reference2.Fast >= 0.0) ? color : color2 };   // 2.2: drawn after the card
			}
			else
			{
				g.DrawString(FlowText(_pb, num16, sc), _fb, Brush((reference2.Fast >= 0.0) ? color : color2), num15 - 4f, num11 + 1f, Right);
			}
		}
		if (showAccel)
		{
			LaneClip(g, rect, num9 - 2f, rect.Bottom);
			g.FillRectangle(Brush(LaneBack), rect.Left, num9, rect.Width, num8 - num9);
			AccelStrip(g, rect, vL, vR, x0, i0, slot, num12, color, color2, num9, num8, num13, num15);
		}
		PaintClip(g, rect);
	}

	// 2.3: the flow lane's other looks: the same numbers (fast, slow, tier, real push, fade, acceleration) drawn as a polarized
	// area, a fast-vs-slow histogram, a ribbon or needles. Points go into reused arrays (slots 7-12); the lane clip stays.
	private void FlowStyled(Graphics g, Rectangle rect, int vL, int vR, double x0, int i0, double slot, float barW, double scale, float zero, float half, float top, float bot, float xR, Color up, Color down, float flowTop, float flowBot)
	{
		int last = Math.Min(base.Count, _pn) - 1;
		int n = vR - vL + 1;
		PointF[] pf = _arrays.Get<PointF>(7, n);
		PointF[] ps = _arrays.Get<PointF>(8, n);
		int[] bi = _arrays.Get<int>(12, n);
		int m = 0;
		for (int i = vL; i <= vR; i++)
		{
			float x = X(i, x0, i0, slot);
			if (x > xR)
			{
				break;
			}
			if (!_pb[i].Has)
			{
				continue;
			}
			int idx = ((LaneClosed && i == last) ? Math.Max(0, i - 1) : i);
			pf[m] = new PointF(x, LaneY(_pb[idx].Fast, zero, half, scale, top, bot));
			ps[m] = new PointF(x, LaneY(_pb[idx].Slow, zero, half, scale, top, bot));
			bi[m] = idx;
			m++;
		}
		if (m < 2)
		{
			return;
		}
		bool accelHere = AccelInFlow && !ShowAccel && LaneStyle == LaneLook.Pro;
		float r = Math.Max(2.5f, Math.Min(5f, (float)slot * 0.35f));
		Pen slowPen = Pen(Color.FromArgb(220, VelocityUseTheme ? Color.FromArgb(235, 238, 245) : VelocityAvg), 1.3f);
		g.SmoothingMode = SmoothingMode.AntiAlias;
		switch (FlowStyle)
		{
		case FlowLook.Area:
		{
			// the fast flow as a soft area (green above zero, red below) under a line in the side's colour; EXTREME bars as dots
			PointF[] area = _arrays.Get<PointF>(9, m + 2);
			Array.Copy(pf, area, m);
			area[m] = new PointF(pf[m - 1].X, zero);
			area[m + 1] = new PointF(pf[0].X, zero);
			PointF[] exact = ExactPoints(10, area, m + 2);
			g.SmoothingMode = SmoothingMode.None;
			g.SetClip(new RectangleF(rect.Left, top, Math.Max(1f, xR - (float)rect.Left), Math.Max(0f, zero - top)), CombineMode.Intersect);
			g.FillPolygon(Brush(Color.FromArgb(72, up)), exact);
			LaneClip(g, rect, flowTop, flowBot);
			g.SetClip(new RectangleF(rect.Left, zero, Math.Max(1f, xR - (float)rect.Left), Math.Max(0f, bot - zero)), CombineMode.Intersect);
			g.FillPolygon(Brush(Color.FromArgb(72, down)), exact);
			LaneClip(g, rect, flowTop, flowBot);
			g.SmoothingMode = SmoothingMode.AntiAlias;
			for (int k = 1; k < m; k++)
			{
				Color c = ((_pb[bi[k]].Fast >= 0.0) ? up : down);
				int a = ((Shade == FlowShade.RealPush) ? (int)(255.0 * (0.35 + 0.65 * _pb[bi[k]].Q)) : 235);
				g.DrawLine(Pen(Color.FromArgb(a, Bright(c, 0.2)), 1.7f), pf[k - 1], pf[k]);
			}
			g.DrawLines(slowPen, ExactPoints(11, ps, m));
			for (int k = 0; k < m; k++)
			{
				int idx = bi[k];
				bool forming = idx == last;
				if (FlowTier(idx) == 3 || forming)
				{
					Color c = ((_pb[idx].Fast >= 0.0) ? up : down);
					g.FillEllipse(Brush(Color.FromArgb(255, 10, 12, 16)), pf[k].X - 4f, pf[k].Y - 4f, 8f, 8f);
					if (forming)
					{
						g.DrawEllipse(Pen(Color.FromArgb(230, Bright(c, 0.35)), 1.2f), pf[k].X - 3f, pf[k].Y - 3f, 6f, 6f);
					}
					else
					{
						g.FillEllipse(Brush(Color.FromArgb(255, Bright(c, 0.3))), pf[k].X - 3f, pf[k].Y - 3f, 6f, 6f);
					}
				}
			}
			break;
		}
		case FlowLook.Histogram:
		{
			// bars = fast minus slow (bright while the gap grows), both lines on top, a diamond where they cross
			if (CrispCandles)
			{
				g.SmoothingMode = SmoothingMode.None;
			}
			double prevH = 0.0;
			for (int k = 0; k < m; k++)
			{
				int idx = bi[k];
				double h = _pb[idx].Fast - _pb[idx].Slow;
				float y = LaneY(h, zero, half, scale, top, bot);
				Color c = ((h >= 0.0) ? up : down);
				bool grow = k > 0 && Math.Abs(h) >= Math.Abs(prevH);
				prevH = h;
				int a = (grow ? 230 : 120);
				if (Shade == FlowShade.RealPush)
				{
					a = (int)((double)a * (0.4 + 0.6 * _pb[idx].Q));
				}
				float y0 = Math.Min(y, zero);
				float hh = Math.Max(1f, Math.Abs(y - zero));
				if (idx == last)
				{
					g.DrawRectangle(Pen(Color.FromArgb(220, Bright(c, 0.35)), 1f), pf[k].X - barW / 2f, y0, barW, hh);   // forming: hollow
				}
				else
				{
					g.FillRectangle(Brush(Color.FromArgb(a, grow ? c : Dark(c, 0.35))), pf[k].X - barW / 2f, y0, barW, hh);
				}
			}
			g.SmoothingMode = SmoothingMode.AntiAlias;
			for (int k = 1; k < m; k++)
			{
				Color c = ((_pb[bi[k]].Fast >= 0.0) ? up : down);
				g.DrawLine(Pen(Color.FromArgb(235, Bright(c, 0.3)), 1.4f), pf[k - 1], pf[k]);
			}
			g.DrawLines(Pen(Color.FromArgb(230, SignalColor), 1.3f), ExactPoints(11, ps, m));
			for (int k = 1; k < m; k++)
			{
				bool nowUp = _pb[bi[k]].Fast >= _pb[bi[k]].Slow;
				if (nowUp != (_pb[bi[k - 1]].Fast >= _pb[bi[k - 1]].Slow))
				{
					PointF[] dia = _points0;
					float x = pf[k].X;
					float y = pf[k].Y;
					dia[0] = new PointF(x, y - 4.5f);
					dia[1] = new PointF(x + 4.5f, y);
					dia[2] = new PointF(x, y + 4.5f);
					dia[3] = new PointF(x - 4.5f, y);
					g.FillPolygon(Brush(Color.FromArgb(255, nowUp ? up : down)), dia);
					g.DrawPolygon(Pen(Color.FromArgb(255, 245, 247, 250), 1f), dia);
				}
			}
			break;
		}
		case FlowLook.Ribbon:
		{
			// the band between fast and slow: green while fast leads above, red below; its width is the momentum
			PointF[] quad = _points0;
			for (int k = 1; k < m; k++)
			{
				int idx = bi[k - 1];
				bool lead = _pb[idx].Fast >= _pb[idx].Slow;
				int a = ((Shade == FlowShade.RealPush) ? (int)(150.0 * (0.45 + 0.55 * _pb[idx].Q)) : 130);
				quad[0] = pf[k - 1];
				quad[1] = pf[k];
				quad[2] = ps[k];
				quad[3] = ps[k - 1];
				g.FillPolygon(Brush(Color.FromArgb(a, lead ? up : down)), quad);
			}
			for (int k = 1; k < m; k++)
			{
				Color c = ((_pb[bi[k]].Fast >= 0.0) ? up : down);
				g.DrawLine(Pen(Color.FromArgb(230, Bright(c, 0.35)), 1.2f), pf[k - 1], pf[k]);
			}
			g.DrawLines(Pen(Color.FromArgb(235, VelocityUseTheme ? Color.FromArgb(235, 238, 245) : VelocityAvg), 1.5f), ExactPoints(11, ps, m));
			for (int k = 1; k < m; k++)
			{
				if ((_pb[bi[k]].Fast >= _pb[bi[k]].Slow) != (_pb[bi[k - 1]].Fast >= _pb[bi[k - 1]].Slow))
				{
					g.FillEllipse(Brush(Color.FromArgb(255, 10, 12, 16)), ps[k].X - 4f, ps[k].Y - 4f, 8f, 8f);
					g.FillEllipse(Brush(Color.FromArgb(255, SignalColor)), ps[k].X - 3f, ps[k].Y - 3f, 6f, 6f);
				}
			}
			break;
		}
		default:
		{
			// needles: a thin line per bar from zero to the fast flow, a dot on the strong and EXTREME bars, the slow line
			float w = Math.Max(1f, Math.Min(2.5f, barW * 0.35f));
			for (int k = 0; k < m; k++)
			{
				int idx = bi[k];
				Color c = ((_pb[idx].Fast >= 0.0) ? up : down);
				int tier = FlowTier(idx);
				int a = ((Shade == FlowShade.RealPush) ? (int)(255.0 * (0.3 + 0.7 * _pb[idx].Q)) : ((tier == 0) ? 110 : ((tier == 1) ? 170 : 235)));
				if (idx == last)
				{
					a = a * 45 / 100;
				}
				g.DrawLine(Pen(Color.FromArgb(a, (tier == 3) ? Bright(c, 0.25) : c), w), pf[k].X, zero, pf[k].X, pf[k].Y);
				if (tier >= 2 && idx != last)
				{
					float dr = ((tier == 3) ? 3.5f : 2.5f);
					g.FillEllipse(Brush(Color.FromArgb(255, 10, 12, 16)), pf[k].X - dr - 1f, pf[k].Y - dr - 1f, 2f * dr + 2f, 2f * dr + 2f);
					g.FillEllipse(Brush(Color.FromArgb(255, Bright(c, (tier == 3) ? 0.35 : 0.1))), pf[k].X - dr, pf[k].Y - dr, 2f * dr, 2f * dr);
				}
			}
			g.DrawLines(slowPen, ExactPoints(11, ps, m));
			break;
		}
		}
		// the extras every look shares: the early-fade dot and, with the strip off, the acceleration marks at the bar's end
		for (int k = 0; k < m; k++)
		{
			int idx = bi[k];
			if (idx == last || !_pb[idx].Real)
			{
				continue;
			}
			bool upBar = _pb[idx].Fast >= 0.0;
			Color c = (upBar ? up : down);
			float h = Math.Abs(pf[k].Y - zero);
			if (ShowFade && _pb[idx].Fade)
			{
				float fy = (upBar ? (zero - h * 0.5f) : (zero + h * 0.5f));
				g.FillEllipse(Brush(Color.FromArgb(255, 10, 12, 16)), pf[k].X - 3.5f, fy - 3.5f, 7f, 7f);
				g.FillEllipse(Brush(Color.FromArgb(245, ExhaustColor)), pf[k].X - 2.5f, fy - 2.5f, 5f, 5f);
			}
			if (accelHere && ShowAccelMarkers)
			{
				double an = AccNorm(idx);
				bool with = (_pb[idx].AccE >= 0.0) == upBar;
				if (_pb[idx].Flip || Math.Abs(an) >= AccelExtreme)
				{
					AccelMarkPro(g, pf[k].X, zero, h, r, upBar, with, Math.Abs(an) >= AccelExtreme, _pb[idx].Flip, c, upBar, top, bot, GateThrust && with && !_pb[idx].ThrustOk);
				}
			}
		}
	}

	// 2.3: a lane value's y, inside the lane
	private static float LaneY(double v, float zero, float half, double scale, float top, float bot)
	{
		float y = zero - (float)((double)half * v / scale);
		return Math.Max(top, Math.Min(bot, y));
	}

	// 2.3: the first count points of src as an array of exactly count points (DrawLines / FillPolygon take the whole array)
	private PointF[] ExactPoints(int slotId, PointF[] src, int count)
	{
		if (src.Length == count)
		{
			return src;
		}
		PointF[] exact = _arrays.Get<PointF>(slotId, count);
		if (exact.Length != count)
		{
			exact = new PointF[count];
		}
		Array.Copy(src, exact, count);
		return exact;
	}

	// 2.3: the bar's flow tier 0-3 (quiet / building / strong / EXTREME) by rank (stored) or by the classic multiples
	private int FlowTier(int idx)
	{
		double rank = ((FlowTiers == FlowTierMode.Percentile) ? _pb[idx].FRank : (-1.0));
		if (rank >= 0.0)
		{
			return (rank < 0.5) ? 0 : ((rank < 0.8) ? 1 : ((rank < 0.95) ? 2 : 3));
		}
		double sc = ScaleC(_pb, idx);
		double mult = ((sc > 1E-09) ? (Math.Abs(_pb[idx].Fast) / sc) : 0.0);
		return (mult < 0.4) ? 0 : ((mult < 1.0) ? 1 : ((mult < 2.0) ? 2 : 3));
	}

	// 2.2 (Pro): the flow and acceleration panes' heights in a window h px high. Each pane keeps a minimum (CVD 28 % / 40 px,
	// flow 18 % / 30 px, acceleration 14 % / 28 px); the shares come from the two settings or from a drag on a line; when the
	// shares ask for more than the room, the panes give back what is above their minimums, in proportion.
	private void PaneHeights(float h, bool showAccel, out float flow, out float acc)
	{
		float gaps = 6f * (float)(showAccel ? 2 : 1) + 3f;
		float avail = Math.Max(30f, h - gaps);
		float mC = Math.Max(40f, h * 0.28f);
		float mF = Math.Max(30f, h * 0.18f);
		float mA = (showAccel ? Math.Max(28f, h * 0.14f) : 0f);
		float mSum = mC + mF + mA;
		if (mSum > avail)
		{
			float k = avail / mSum;
			mC *= k;
			mF *= k;
			mA *= k;
		}
		float fF = ((_spFrac > 0f) ? _spFrac : ((float)Math.Max(10, Math.Min(80, VelocityPanelHeight)) / 100f));
		float fA = ((!showAccel) ? 0f : ((_acFrac > 0f) ? _acFrac : ((float)Math.Max(8, Math.Min(50, AccelHeight)) / 100f)));
		flow = Math.Max(mF, h * fF);
		acc = (showAccel ? Math.Max(mA, h * fA) : 0f);
		float over = flow + acc - (avail - mC);
		if (over > 0f)
		{
			float rF = flow - mF;
			float rA = acc - mA;
			if (rF + rA > 0.01f)
			{
				float k2 = Math.Min(1f, over / (rF + rA));
				flow -= rF * k2;
				acc -= rA * k2;
			}
		}
		_pMinC = mC;
		_pMinF = mF;
		_pMinA = mA;
	}

	// 2.2 (Pro): the lanes' title pills, after the card of this picture
	private void DrawLaneTitles(Graphics g, Rectangle rect)
	{
		if (!_ltFlow.On && !_ltAcc.On)
		{
			return;
		}
		PaintClip(g, rect);
		RectangleF card = ((Table != Corner.Hidden) ? _tableRect : RectangleF.Empty);
		if (_ltFlow.On)
		{
			LaneTitle(g, rect, card, _ltFlow.Y, _ltFlow.Name, _ltFlow.Text, _ltFlow.Col, _ltFlow.Tier, _ltFlow.Side);
		}
		if (_ltAcc.On)
		{
			LaneTitle(g, rect, card, _ltAcc.Y, _ltAcc.Name, _ltAcc.Text, _ltAcc.Col, _ltAcc.Tier, _ltAcc.Side);
		}
	}

	// 2.2 (Pro): a lane's name and reading as one pill at its top left (after the card when the card covers that corner):
	// the lane's whole height is for the bars, and the newest bars on the right stay clear
	private void LaneTitle(Graphics g, Rectangle rect, RectangleF card, float y, string name, string text, Color col, int tier, Color side)
	{
		float x = (float)rect.Left + 6f;
		float h = _capH + 2f;
		if (card.Width > 0f && card.Left < (float)rect.Left + 40f && card.Top < y + h && card.Bottom > y - 1f)
		{
			x = card.Right + 8f;
		}
		float nameW = TextW(g, name, _fs);
		float tw = (string.IsNullOrEmpty(text) ? 0f : TextW(g, text, _fb));
		float dots = ((tier >= 0) ? 32f : 0f);
		float w = 8f + nameW + ((tw > 0f || dots > 0f) ? (8f + dots + tw) : 0f) + 8f;
		SmoothingMode sm = g.SmoothingMode;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		GraphicsPath path = Rounded(_gdi.Path(21), x, y - 1f, w, h, h / 2f);
		g.FillPath(Brush(Color.FromArgb(225, 8, 10, 14)), path);
		g.FillPath(Brush(Color.FromArgb(36, col)), path);
		g.DrawString(name, _fs, Brush(Color.FromArgb(205, 150, 158, 175)), x + 8f, y + 1f, NoWrap);
		float dx = x + 8f + nameW + 8f;
		if (tier >= 0)
		{
			for (int i = 0; i < 4; i++)
			{
				g.FillEllipse(Brush((i <= tier) ? Color.FromArgb(240, side) : Color.FromArgb(255, 46, 52, 64)), dx + (float)i * 7f, y + h / 2f - 3.5f, 5f, 5f);
			}
			dx += dots;
		}
		if (tw > 0f)
		{
			g.DrawString(text, _fb, Brush(col), dx, y, NoWrap);
		}
		g.SmoothingMode = sm;
	}

	// 2.2: the CVD pane goes MANUAL from the view on the screen (no jump)
	private void CvdToManual()
	{
		if (_cvAuto || !_cvMSet)
		{
			_cvMCenter = _cvCenter;
			_cvMHalf = Math.Max(1E-09, _cvHalf);
			_cvMSet = true;
			_cvFollow = true;   // AUTO had the newest candle in view
		}
		_cvAuto = false;
		_cvOff = 0f;
	}

	// 2.2: stretch (k < 1) or squeeze (k > 1) the CVD pane from the view (c0, h0) around the newest candle: it stays where
	// it is on the screen; at most 50 x either way of the visible range
	private void CvdStretch(double k, double c0, double h0)
	{
		h0 = Math.Max(1E-09, h0);
		double h = Math.Max(_cvRefHalf / 50.0, Math.Min(_cvRefHalf * 50.0, h0 * k));
		_cvMHalf = Math.Max(1E-09, h);
		// around the newest candle while the view follows it (it keeps its place), else around the pane's middle (what the
		// user is looking at keeps its place)
		double a = (_cvFollow ? _cvAnchor : c0);
		_cvMCenter = a - (a - c0) * (_cvMHalf / h0);
		_cvMSet = true;
	}

	// 2.2: the newest candle inside the CVD candles' band (with the follow margin), in the view the mouse set
	private bool CvdNewestInView()
	{
		float m = Math.Min(10f, (_cvBandBot - _cvBandTop) * 0.12f);
		float y = _cvMid - (float)((_cvAnchor - _cvMCenter) / Math.Max(1E-09, _cvMHalf) * (double)_cvHalfPx);
		return y >= _cvBandTop + m - 0.5f && y <= _cvBandBot - m + 0.5f;
	}

	// 2.1: a lane paints only inside its band (the window's clip, then the band)
	private void LaneClip(Graphics g, Rectangle rect, float top, float bottom)
	{
		PaintClip(g, rect);
		g.SetClip(new RectangleF(rect.Left, top, rect.Width, Math.Max(0f, bottom - top)), CombineMode.Intersect);
	}

	// 2.1: a lane's offset within its room: its zero line never leaves the lane
	private static float Lim(float off, float room)
	{
		return Math.Max(0f - room, Math.Min(room, off));
	}

	private double AccNorm(int abs)
	{
		int num = Math.Min(Math.Max(5, LookbackBars), abs + 1);
		double num2 = Win(_pb, abs, num, Pf.AbsAcc) / (double)num;
		if (!(num2 > 1E-12))
		{
			return 0.0;
		}
		return _pb[abs].AccE / num2;
	}

	private void AccelStrip(Graphics g, Rectangle rect, int vL, int vR, double x0, int i0, double slot, float w, Color up, Color down, float top, float bot, float capH, float xR)
	{
		float num = Math.Max(2.5f, Math.Min(5f, (float)slot * 0.35f));
		float num2 = top + capH;
		float num3 = bot - num2;
		_acRoom = Math.Max(0f, num3 / 2f);   // 2.1: the zero line stays inside its lane (see the flow lane)
		float num4 = num2 + num3 / 2f + Lim(_acOff, _acRoom);
		bool pro = LaneStyle == LaneLook.Pro;
		float num5 = (pro ? Math.Max(3f, num3 / 2f - 2f - (ShowAccelMarkers ? (num * 2f) : 0f)) : Math.Max(3f, num3 / 2f - 3f - (ShowAccelMarkers ? (num * 2.2f) : 0f)));
		double num6 = ((AccelScale > 0.0) ? AccelScale : (Math.Max(AccelExtreme, AccelMin) * 1.5)) * _acZoom;
		if (pro && AccelScale <= 0.0)
		{
			// 2.2: the scale fits the visible bars, so every bar ends inside the lane (the biggest one + 4 %), never under
			// 1.25 x the extreme level (a quiet stretch is not blown up). 1.x: a fixed 3x; bars over 4.5x were drawn smaller
			// and over the lane's edge cut. "Acceleration: fixed scale" still wins.
			double vmax = 0.0;
			int lastBar = Math.Min(base.Count, _pn) - 1;
			for (int j = vL; j <= vR; j++)
			{
				if (!_pb[j].Has)
				{
					continue;
				}
				if (X(j, x0, i0, slot) > xR)
				{
					break;
				}
				int jj = ((LaneClosed && j == lastBar) ? Math.Max(0, j - 1) : j);
				vmax = Math.Max(vmax, Math.Abs(AccNorm(jj)));
			}
			num6 = Math.Max(Math.Max(AccelExtreme, AccelMin) * 1.25, vmax * 1.04) * _acZoom;
		}
		g.DrawLine(Pen(Color.FromArgb(70, 150, 158, 175), 1f), rect.Left, num4, xR, num4);
		_laneAccScale = num6;
		_laneAccHalf = num5;
		_laneAccZero = num4;
		_laneAccTop = num2;
		_laneAccBot = bot;
		float width = Math.Max(0f, xR - (float)rect.Left);
		float num7 = ((num6 > 0.0) ? ((float)Math.Min(num5, (double)num5 * AccelMin / num6)) : 0f);
		float num8 = ((num6 > 0.0) ? ((float)Math.Min(num5, (double)num5 * AccelExtreme / num6)) : num5);
		if (pro)
		{
			// 2.1: the two levels as thin lines: faint = meaningful (1x typical), brighter = extreme (2x)
			Pen dash = Pen(Color.FromArgb(38, 150, 158, 175), 1f);
			Pen solid = Pen(Color.FromArgb(70, 150, 158, 175), 1f);
			if (num4 - num7 > num2)
			{
				g.DrawLine(dash, rect.Left, num4 - num7, xR, num4 - num7);
			}
			if (num4 + num7 < bot)
			{
				g.DrawLine(dash, rect.Left, num4 + num7, xR, num4 + num7);
			}
			if (num4 - num8 > num2)
			{
				g.DrawLine(solid, rect.Left, num4 - num8, xR, num4 - num8);
			}
			if (num4 + num8 < bot)
			{
				g.DrawLine(solid, rect.Left, num4 + num8, xR, num4 + num8);
			}
		}
		else
		{
			if (num4 - num8 > num2)
			{
				g.FillRectangle(Brush(Color.FromArgb(22, up)), rect.Left, num2, width, num4 - num8 - num2);
			}
			if (num4 + num8 < bot)
			{
				g.FillRectangle(Brush(Color.FromArgb(22, down)), rect.Left, num4 + num8, width, bot - (num4 + num8));
			}
			if (num8 > num7)
			{
				g.FillRectangle(Brush(Color.FromArgb(13, up)), rect.Left, Math.Max(num2, num4 - num8), width, Math.Max(0f, num8 - num7));
				g.FillRectangle(Brush(Color.FromArgb(13, down)), rect.Left, num4 + num7, width, Math.Max(0f, Math.Min(bot, num4 + num8) - (num4 + num7)));
			}
		}
		int num9 = Math.Min(base.Count, _pn);
		for (int j = vL; j <= vR; j++)
		{
			if (!_pb[j].Has)
			{
				continue;
			}
			float num10 = X(j, x0, i0, slot);
			if (num10 > xR)
			{
				break;
			}
			bool flag = j == num9 - 1;
			int num11 = ((LaneClosed && flag) ? Math.Max(0, j - 1) : j);
			double num12 = Math.Abs(AccNorm(num11));
			bool flag2 = _pb[num11].Fast >= 0.0;
			bool flag3 = _pb[num11].AccE >= 0.0 == flag2;
			bool flag4 = ((AccelMode == AccelStyle.Strength) ? flag3 : (_pb[num11].AccE >= 0.0));
			Color color = (flag2 ? up : down);
			float num13 = (pro ? ((float)((double)num5 * num12 / num6)) : ((float)Math.Min((double)num5 * 1.5, (double)num5 * num12 / num6)));
			bool flag5 = num12 >= AccelMin;
			bool flag6 = num12 >= AccelExtreme;
			int num14 = ((!flag5) ? 80 : (flag6 ? 255 : 185));
			if (!flag3 && flag5)
			{
				num14 -= 35;
			}
			if (flag)
			{
				num14 = num14 * (LiveReadout ? 9 : 6) / 10;
			}
			Color barCol = color;
			bool hollow = false;
			if (pro)
			{
				// 2.1: the colour is always the side (green = buyers, red = sellers); up = speeding up, down = slowing (as
				// always); speeding up = a solid bar (brighter from 2x), slowing = a hollow bar, under 1x = a faint bar
				barCol = ((flag3 && flag6) ? Bright(color, 0.2) : color);
				num14 = ((!flag5) ? 90 : (flag3 ? (flag6 ? 255 : 210) : 55));
				hollow = flag5 && !flag3;
				if (flag)
				{
					num14 = num14 * 45 / 100;
				}
			}
			if (CrispCandles)
			{
				g.SmoothingMode = SmoothingMode.None;   // 2.0: acceleration bars on whole pixels
			}
			HBar(g, num10, num4, w, num13, flag4, barCol, num14, _pb[j].Real, num2, bot);
			if (pro && num13 > (flag4 ? (num4 - num2) : (bot - num4)) + 0.5f)
			{
				// 2.2: the lane is stretched and its edge cuts this bar: a bright cap says so
				g.FillRectangle(Brush(Color.FromArgb(255, Bright(color, 0.6))), num10 - w / 2f, flag4 ? num2 : (bot - 2f), w, 2f);
			}
			if (hollow && w >= 3f)
			{
				// the slowing bar's outline in the side's colour
				float hh = Math.Max(1f, num13);
				float yTop = Math.Max(num2, flag4 ? (num4 - hh) : num4);
				float yBot = Math.Min(bot, flag4 ? num4 : (num4 + hh));
				if (yBot - yTop >= 1f)
				{
					g.DrawRectangle(Pen(Color.FromArgb(flag ? 140 : 235, color), 1f), num10 - w / 2f, yTop, w - 1f, yBot - yTop);
				}
			}
			if (CrispCandles)
			{
				g.SmoothingMode = SmoothingMode.AntiAlias;
			}
			if (!ShowAccelMarkers || flag)
			{
				continue;
			}
			if (pro)
			{
				if (_pb[num11].Flip || (flag6 && _pb[j].Real))
				{
					AccelMarkPro(g, num10, num4, num13, num, flag4, flag3, flag6 && _pb[j].Real, _pb[num11].Flip, color, flag2, num2, bot, GateThrust && flag3 && !_pb[num11].ThrustOk);   // 2.3: gated
				}
				continue;
			}
			if (_pb[num11].Flip)
			{
				g.DrawLine(Pen(Color.FromArgb(230, color), 1.5f), num10, num4 - num * 1.2f, num10, num4 + num * 1.2f);
			}
			if (flag6 && _pb[j].Real)
			{
				float num15 = (flag4 ? (num4 - num13) : num4);
				float val = (flag4 ? (num15 - num - 1.5f) : (num15 + num13 + num + 1.5f));
				val = Math.Max(num2 + num, Math.Min(bot - num, val));
				if (flag3)
				{
					PointF[] points = _points1;   // P4: reused (the same points)
					if (!flag2)
					{
						points[0] = new PointF(num10, val + num);
						points[1] = new PointF(num10 - num, val - num * 0.6f);
						points[2] = new PointF(num10 + num, val - num * 0.6f);
					}
					else
					{
						points[0] = new PointF(num10, val - num);
						points[1] = new PointF(num10 - num, val + num * 0.6f);
						points[2] = new PointF(num10 + num, val + num * 0.6f);
					}
					if (GateThrust && !_pb[num11].ThrustOk)
					{
						g.DrawPolygon(Pen(Color.FromArgb(175, 128, 136, 152), 1.2f), points);   // 2.3: a thin thrust: outline only
					}
					else
					{
						g.FillPolygon(Brush(Color.FromArgb(240, color)), points);
					}
				}
				else
				{
					g.DrawEllipse(Pen(Color.FromArgb(230, color), 1.4f), num10 - num * 0.8f, val - num * 0.8f, num * 1.6f, num * 1.6f);
				}
			}
		}
		if (ShowLaneText)
		{
			SolidBrush brush = Brush(Color.FromArgb(200, 150, 158, 175));
			if (!pro)
			{
				g.DrawString("ACCELERATION", _fb, brush, rect.Left + 8, top + 1f);
			}
			int num16 = ((LaneClosed && vR == num9 - 1) ? Math.Max(0, vR - 1) : vR);
			ref Bar reference = ref _pb[num16];
			double num17 = AccNorm(num16);
			Axis(g, xR, num2, bot, num4, num5, num6, num17, (reference.Fast >= 0.0) ? up : down, typical: true);
			if (pro && (!ShowAccelText || !reference.Has))
			{
				_ltAcc = new LaneTitleAsk { On = true, Y = top + 2f, Name = "ACCELERATION", Text = null, Col = Color.FromArgb(150, 158, 174), Tier = -1, Side = up };   // 2.2
			}
			if (ShowAccelText && reference.Has)
			{
				Color col;
				if (pro)
				{
					string s2 = AccelWords(reference.Fast >= 0.0, reference.AccE, num17, reference.ThrustOk, out col);
					_ltAcc = new LaneTitleAsk { On = true, Y = top + 2f, Name = "ACCELERATION", Text = s2, Col = col, Tier = -1, Side = col };   // 2.2: a pill at the top left, drawn after the card
				}
				else
				{
					string s = AccelText(reference.Fast >= 0.0, reference.AccE, num17, reference.ThrustOk, out col);
					g.DrawString(s, _fb, Brush(col), xR - 4f, top + 1f, Right);
				}
			}
		}
	}

	// 2.1: a lane's reading as a pill at the right of its caption row; four dots show the flow's strength step
	private void LanePill(Graphics g, float xRight, float y, string text, Color col, int tier, Color side)
	{
		float h = _capH + 2f;
		float tw = TextW(g, text, _fb);
		float dots = ((tier >= 0) ? 32f : 0f);
		float w = tw + 14f + dots;
		float x = xRight - w;
		SmoothingMode sm = g.SmoothingMode;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		g.FillPath(Brush(Color.FromArgb(36, col)), Rounded(_gdi.Path(20), x, y - 1f, w, h, h / 2f));
		float dx = x + 8f;
		if (tier >= 0)
		{
			for (int i = 0; i < 4; i++)
			{
				g.FillEllipse(Brush((i <= tier) ? Color.FromArgb(240, side) : Color.FromArgb(255, 46, 52, 64)), dx + (float)i * 7f, y + h / 2f - 3.5f, 5f, 5f);
			}
			dx += dots;
		}
		g.DrawString(text, _fb, Brush(col), dx, y, NoWrap);
		g.SmoothingMode = sm;
	}

	// 2.1: the acceleration strip's marks: a thrust (2x or more, with the flow) is a chevron on a dark halo, a stall (2x or
	// more, against the flow) an amber ring with a dot, a change of side a dot on the zero line
	private void AccelMarkPro(Graphics g, float x, float yMid, float h, float r, bool upBar, bool with, bool extreme, bool flip, Color side, bool buyers, float top, float bot, bool thin = false)
	{
		SmoothingMode sm = g.SmoothingMode;
		g.SmoothingMode = SmoothingMode.AntiAlias;
		if (flip)
		{
			g.FillEllipse(Brush(Color.FromArgb(255, 10, 12, 16)), x - 3f, yMid - 3f, 6f, 6f);
			g.FillEllipse(Brush(Color.FromArgb(240, side)), x - 2f, yMid - 2f, 4f, 4f);
		}
		if (extreme)
		{
			float end = (upBar ? (yMid - h) : (yMid + h));
			float y = (upBar ? (end - r - 2f) : (end + r + 2f));
			y = Math.Max(top + r, Math.Min(bot - r, y));
			g.FillEllipse(Brush(Color.FromArgb(220, 8, 10, 14)), x - r - 1.5f, y - r - 1.5f, 2f * r + 3f, 2f * r + 3f);
			if (with)
			{
				float d = (buyers ? -1f : 1f);
				Pen p = (thin ? Pen(Color.FromArgb(175, 128, 136, 152), 1.4f) : Pen(Color.FromArgb(250, Bright(side, 0.2)), 1.8f));   // 2.3: a thin thrust (no busy, one-sided market behind it) is a grey chevron
				g.DrawLine(p, x - r * 0.75f, y - d * r * 0.3f, x, y + d * r * 0.45f);
				g.DrawLine(p, x, y + d * r * 0.45f, x + r * 0.75f, y - d * r * 0.3f);
			}
			else
			{
				g.DrawEllipse(Pen(Color.FromArgb(240, side), 1.5f), x - r * 0.8f, y - r * 0.8f, r * 1.6f, r * 1.6f);
				g.FillEllipse(Brush(side), x - 1.5f, y - 1.5f, 3f, 3f);
			}
		}
		g.SmoothingMode = sm;
	}

	private void HBar(Graphics g, float x, float yMid, float w, float h, bool upBar, Color col, int alpha, bool real, float laneTop, float laneBot)
	{
		if (!real)
		{
			alpha = Math.Min(alpha, 60);
		}
		h = Math.Max(1f, h);
		float num = (upBar ? (yMid - h) : yMid);
		float num2 = num + h;
		if (num < laneTop)
		{
			num = laneTop;
		}
		if (num2 > laneBot)
		{
			num2 = laneBot;
		}
		if (num2 <= num)
		{
			return;
		}
		float x2 = x - w / 2f;
		if (Bars == BarStyle.Outline)
		{
			g.FillRectangle(Brush(Color.FromArgb(alpha / 3, col)), x2, num, w, num2 - num);
			g.DrawRectangle(Pen(Color.FromArgb(alpha, col), 1f), x2, num, w, num2 - num);
			return;
		}
		g.FillRectangle(Brush(Color.FromArgb(alpha, col)), x2, num, w, num2 - num);
		if (Bars == BarStyle.Gradient && num2 - num >= 3f)
		{
			g.FillRectangle(Brush(Color.FromArgb(Math.Min(255, alpha + 50), Bright(col, 0.45))), x2, upBar ? num : (num2 - 1.5f), w, 1.5f);
		}
	}

	private void HBarTier(Graphics g, float x, float yMid, float w, float h, bool upBar, Color col, int alpha, bool real, float laneTop, float laneBot, bool highlight, Color hiCol)
	{
		if (!real)
		{
			alpha = Math.Min(alpha, 60);
		}
		h = Math.Max(1.5f, h);
		float num = (upBar ? (yMid - h) : yMid);
		float num2 = num + h;
		if (num < laneTop)
		{
			num = laneTop;
		}
		if (num2 > laneBot)
		{
			num2 = laneBot;
		}
		if (num2 <= num)
		{
			return;
		}
		float x2 = x - w / 2f;
		if (Bars == BarStyle.Outline)
		{
			g.FillRectangle(Brush(Color.FromArgb(alpha / 3, col)), x2, num, w, num2 - num);
			g.DrawRectangle(Pen(Color.FromArgb(alpha, col), 1f), x2, num, w, num2 - num);
			return;
		}
		g.FillRectangle(Brush(Color.FromArgb(alpha, col)), x2, num, w, num2 - num);
		if (Bars == BarStyle.Gradient && num2 - num >= 3f)
		{
			g.FillRectangle(Brush(Color.FromArgb(Math.Min(255, alpha + 50), Bright(col, 0.45))), x2, upBar ? num : (num2 - 1.5f), w, 1.5f);   // 2.3: "Gradient (lit top)" reached only the acceleration strip
		}
		if (w >= 3.5f)
		{
			g.DrawRectangle(Pen(Color.FromArgb(Math.Min(255, alpha + 35), Dark(col, 0.25)), 0.8f), x2, num, w, num2 - num);
		}
		if (highlight && num2 - num >= 3f)
		{
			float num3 = Math.Min(2.5f, (num2 - num) * 0.35f);
			float y = (upBar ? num : (num2 - num3));
			g.FillRectangle(Brush(Color.FromArgb(255, hiCol)), x2, y, w, num3);
		}
	}

	private void Axis(Graphics g, float xR, float laneTop, float laneBot, float yMid, float half, double scale, double value, Color col, bool typical)
	{
		SolidBrush brush = Brush(Color.FromArgb(170, 150, 158, 175));
		string s = (typical ? ("+" + scale.ToString("0.0", CultureInfo.InvariantCulture) + "x") : FmtS(scale));
		string s2 = (typical ? ("-" + scale.ToString("0.0", CultureInfo.InvariantCulture) + "x") : FmtS(0.0 - scale));
		g.DrawLine(Pen(Color.FromArgb(50, 150, 158, 175), 1f), xR, laneTop, xR, laneBot);
		float num = yMid - half;
		float num2 = yMid + half;
		if (num >= laneTop - 1f && num <= laneBot)
		{
			g.DrawString(s, _fs, brush, xR + 3f, num - _capH / 2f);
		}
		if (yMid >= laneTop && yMid <= laneBot)
		{
			g.DrawString("0", _fs, brush, xR + 3f, yMid - _capH / 2f);
		}
		if (num2 >= laneTop && num2 <= laneBot + 1f)
		{
			g.DrawString(s2, _fs, brush, xR + 3f, num2 - _capH / 2f);
		}
		float val = yMid - (float)((double)half * value / scale);
		val = Math.Max(laneTop + _capH / 2f, Math.Min(laneBot - _capH / 2f, val));
		string s3 = (typical ? (Math.Abs(value).ToString("0.0", CultureInfo.InvariantCulture) + "x") : FmtS(value));
		g.FillRectangle(Brush(Color.FromArgb(230, col)), xR + 1f, val - _capH / 2f, _axisW - 2f, _capH);
		g.DrawString(s3, _fs, Brush(Color.FromArgb(255, 10, 12, 16)), xR + 3f, val - _capH / 2f);
	}

	private static Color Bright(Color c, double k)
	{
		return Color.FromArgb(c.A, (int)Math.Min(255.0, (double)(int)c.R + (double)(255 - c.R) * k), (int)Math.Min(255.0, (double)(int)c.G + (double)(255 - c.G) * k), (int)Math.Min(255.0, (double)(int)c.B + (double)(255 - c.B) * k));
	}

	private static Color Dark(Color c, double k)
	{
		return Color.FromArgb(c.A, (int)Math.Max(0.0, (double)(int)c.R * (1.0 - k)), (int)Math.Max(0.0, (double)(int)c.G * (1.0 - k)), (int)Math.Max(0.0, (double)(int)c.B * (1.0 - k)));
	}

	protected override bool OnTryGetMinMax(int fromOffset, int toOffset, out double min, out double max)
	{
		min = double.MaxValue;
		max = double.MinValue;
		try
		{
			lock (_sync)
			{
				int num = Math.Min(base.Count, _n);
				if (num <= 0)
				{
					return false;
				}
				int num2 = Math.Max(0, Math.Min(fromOffset, toOffset));
				int num3 = Math.Max(fromOffset, toOffset);
				for (int i = num2; i <= num3; i++)
				{
					int num4 = num - 1 - i;
					if (num4 < 0)
					{
						break;
					}
					if (num4 < _b.Length && _b[num4].Has)
					{
						ref Bar reference = ref _b[num4];
						double num5;
						double num6;
						if (Mode == Display.Candles || (Mode == Display.Velocity && VelocityStyle == VelocityCvdStyle.Candles))
						{
							num5 = reference.L;
							num6 = reference.H;
						}
						else if (Mode == Display.DeltaBars)
						{
							num5 = Math.Min(0.0, reference.Delta);
							num6 = Math.Max(0.0, reference.Delta);
						}
						else
						{
							num5 = (num6 = reference.C);
						}
						if (num5 < min)
						{
							min = num5;
						}
						if (num6 > max)
						{
							max = num6;
						}
					}
				}
				if (min > max)
				{
					return false;
				}
				if (ShowZero)
				{
					if (min > 0.0)
					{
						min = 0.0;
					}
					if (max < 0.0)
					{
						max = 0.0;
					}
				}
				if (Mode == Display.Velocity)
				{
					if (_cvOk && _winRect.Height > 10)
					{
						max = CvdV(_winRect.Top);
						min = CvdV(_winRect.Bottom);
						if (max < min)
						{
							double num7 = max;
							max = min;
							min = num7;
						}
						return max > min;
					}
					double num8 = ((max > min) ? (max - min) : Math.Max(1.0, Math.Abs(max) * 0.02));
					double num9 = ((_reserveFrac > 0.0) ? _reserveFrac : 0.5);
					max += num8 * 0.04;
					min -= num8 * num9 / (1.0 - num9);
				}
				else if (max > min)
				{
					double num10 = (max - min) * 0.04;
					min -= num10;
					max += num10;
				}
				return true;
			}
		}
		catch
		{
			return false;
		}
	}

	private string AccelText(bool buyers, double acc, double norm, bool thrustOk, out Color col)
	{
		bool flag = acc >= 0.0 == buyers;
		double num = Math.Abs(norm);
		string text = (buyers ? "buyers" : "sellers");
		string text2 = ((num < AccelMin) ? "steady" : ((!flag) ? ((num >= AccelExtreme) ? "STALL - losing speed" : "fading") : ((num >= AccelExtreme) ? ((GateThrust && !thrustOk) ? "thin THRUST - no volume behind it" : "THRUST - accelerating") : "accelerating")));   // 2.3: gated
		col = ((num < AccelMin) ? Color.FromArgb(150, 158, 174) : (buyers ? BullColor : BearColor));
		if (!flag && num >= AccelMin)
		{
			col = ExhaustColor;
		}
		return text + " " + text2 + "  " + num.ToString("0.0", CultureInfo.InvariantCulture) + "x typical";
	}

	private void Verdict(bool real, double e, double r, out string text, out Color col, out int passive)
	{
		passive = 0;
		Color color = Color.FromArgb(150, 158, 174);
		if (!real)
		{
			text = "no order-flow data for this bar";
			col = color;
			return;
		}
		double num = Math.Abs(e);
		int num2 = ((e >= 0.0) ? 1 : (-1));
		if (num < 0.7)
		{
			if (Math.Abs(r) >= 0.7)
			{
				text = ((r >= 0.0) ? "rises with no push – thin, fragile" : "falls with no push – thin, fragile");
				col = ExhaustColor;
			}
			else
			{
				text = "quiet – wait";
				col = color;
			}
			return;
		}
		double num3 = r * (double)num2;
		if (num3 <= -0.7)
		{
			text = ((num2 > 0) ? "buyers push, price falls – BUYERS TRAPPED" : "sellers push, price rises – SELLERS TRAPPED");
			col = ((num2 > 0) ? BearColor : BullColor);
			passive = -num2;
			return;
		}
		double num4 = num3 / num;
		if (num4 < 0.35)
		{
			if (num >= 1.0)
			{
				text = ((num2 > 0) ? "BUYERS being ABSORBED" : "SELLERS being ABSORBED");
				col = AbsorbColor;
				passive = -num2;
			}
			else
			{
				text = ((num2 > 0) ? "buying stalls" : "selling stalls");
				col = Color.FromArgb(200, 208, 224);
			}
		}
		else if (num4 > 2.5)
		{
			text = ((num2 > 0) ? "rises on thin buying – fragile" : "falls on thin selling – fragile");
			col = ExhaustColor;
		}
		else
		{
			text = ((num2 > 0) ? "healthy buying" : "healthy selling");
			col = ((num2 > 0) ? BullColor : BearColor);
		}
	}

	private void Signals(Graphics g, IChartWindowCoordinatesConverter conv, Rectangle rect, int vL, int vR, double x0, int i0, double slot)
	{
		float num = (float)Math.Max(4.0, Math.Min(9.0, slot * 0.45));
		_hitList.Clear();
		for (int j = 0; j < _psigs.Count; j++)
		{
			Sig sig = _psigs[j];
			// 2.0 (F2): db = the bar the marker is drawn on (classic: its own bar, when the filter passes it now)
			int db;
			if (Math.Max(sig.Abs, sig.Seen) >= vL && sig.Abs <= vR && (sig.Kind != SigKind.Absorption || ShowAbsorption) && (sig.Kind != SigKind.Exhaustion || ShowExhaustion) && (sig.Kind != SigKind.Divergence || ShowDivergence) && (ShowSwingDiv || sig.P1 < 0) && (db = DrawBar(sig)) >= vL && db <= vR)
			{
				ref Bar reference = ref _pb[db];
				float x1 = X(db, x0, i0, slot);
				double v = ((Mode != Display.DeltaBars) ? ((Mode == Display.Velocity) ? reference.C : (sig.Bull ? reference.L : reference.H)) : (sig.Bull ? Math.Min(0.0, reference.Delta) : Math.Max(0.0, reference.Delta)));
				if (Mode == Display.Velocity && !_cvAuto && _cvOk)
				{
					// 2.2: a stretched CVD pane: a candle out of view has no marker (it was pushed to the pane's edge)
					float yv = CvdY(v);
					if (yv < (float)rect.Top - 1f || yv > _laneTop - 7f)
					{
						continue;
					}
				}
				float val = CY(conv, v) + (sig.Bull ? (num + 6f) : (0f - (num + 6f)));
				float val2 = ((Mode == Display.Velocity && _laneTop > (float)(rect.Top + 20)) ? (_laneTop - num - 3f) : ((float)rect.Bottom - num - 2f));
				val = Math.Max((float)rect.Top + num + 2f, Math.Min(val2, val));
				Color color = ((sig.Kind == SigKind.Absorption) ? AbsorbColor : ((sig.Kind == SigKind.Exhaustion) ? ExhaustColor : DivergeColor));
				if (sig.P1 >= 0 && sig.P2 >= 0 && Mode != Display.DeltaBars)
				{
					float x2 = X(sig.P1, x0, i0, slot);
					float num2 = X(sig.P2, x0, i0, slot);
					float y = CY(conv, sig.V1);
					float num3 = CY(conv, sig.V2);
					g.DrawLine(DashPen(Color.FromArgb(200, color), 1.5f), x2, y, num2, num3);   // 2.3: its own dashed pen
					if (SwingAtConfirm)
					{
						// 2.0 (plan F3): the second swing is only known SwingBars later; the marker sits on the bar that
						// confirmed it, linked to the swing by a thin line (classic: drawn on the swing itself)
						SwingLink(g, rect, num2, num3, x1, val, Color.FromArgb(120, color));
					}
					else
					{
						x1 = num2;
						val = Math.Max((float)rect.Top + num + 2f, Math.Min(val2, num3 + (sig.Bull ? (num + 6f) : (0f - (num + 6f)))));
					}
				}
				float r = num;
				if (MarkerStyle == MarkerLook.Badge)
				{
					// 2.1: the Pro badges - a round tag with a pointer to its candle and a symbol that says the kind; a
					// stronger signal than its kind's recent average is a little larger
					double avg = (Filter == SignalFilter.All && sig.Avg0 < 0.0) ? AvgScore(sig) : _lastAvg;
					r = num * 1.15f;
					if (avg > 0.0)
					{
						r = num * (float)Math.Max(1.05, Math.Min(1.45, 1.05 + 0.13 * sig.Score / avg));
					}
					GlyphBadge(g, sig.Kind, sig.P1 >= 0, x1, val, r, color, sig.Bull ? BullColor : BearColor, sig.Bull, sig.Pending);
				}
				else if (MarkerStyle == MarkerLook.Clean)
				{
					// 2.0: one marker family; a stronger signal than its kind's recent average is drawn a little larger (a 2.0
					// signal: the average known when it was made, set by DrawBar)
					double avg = (Filter == SignalFilter.All && sig.Avg0 < 0.0) ? AvgScore(sig) : _lastAvg;
					if (avg > 0.0)
					{
						r = num * (float)Math.Max(0.85, Math.Min(1.3, 0.85 + 0.15 * sig.Score / avg));
					}
					GlyphClean(g, sig.Kind, x1, val, r, color, sig.Bull ? BullColor : BearColor, sig.Bull, sig.Pending);
				}
				else
				{
					Glyph(g, sig.Kind, x1, val, num, color, sig.Bull ? BullColor : BearColor, sig.Bull, sig.Pending);
				}
				if (KeepFirstSignal && sig.Seen > db && !sig.Pending && sig.P1 < 0)
				{
					// 2.0 (F2): the same event fired again later: its marker stays here and a thin tail runs to the last bar it
					// fired on (the hover card says it in words)
					float tail = (float)((double)(sig.Seen - db) * slot);
					g.FillRectangle(Brush(Color.FromArgb(170, color)), x1 + r + 1f, val - 0.75f, Math.Max(1f, tail - r - 1f), 1.5f);
				}
				if (ShowScores)
				{
					g.DrawString(sig.Score.ToString("0.0", CultureInfo.InvariantCulture), _fs, Brush(Color.FromArgb(210, TextColor)), x1, sig.Bull ? (val + num + 6f) : (val - num - 6f), Center);
				}
				if (HoverCards)
				{
					_hitList.Add((new RectangleF(x1 - r - 3f, val - r - 3f, 2f * r + 6f, 2f * r + 6f), sig));
				}
			}
		}
	}

	// 2.0 (F3): the thin link from the swing to the marker on the confirming bar, drawn only where it is inside the window
	// (GDI+ walks a line from its start: one that starts far off the window would cost a lot of time)
	private void SwingLink(Graphics g, Rectangle rect, float xa, float ya, float xb, float yb, Color c)
	{
		float t0 = 0f;
		float t1 = 1f;
		float dx = xb - xa;
		float dy = yb - ya;
		// Liang-Barsky: the part of the segment inside the window
		if (!Edge(-dx, xa - rect.Left, ref t0, ref t1) || !Edge(dx, rect.Right - xa, ref t0, ref t1) ||
			!Edge(-dy, ya - rect.Top, ref t0, ref t1) || !Edge(dy, rect.Bottom - ya, ref t0, ref t1) || t0 > t1)
		{
			return;
		}
		g.DrawLine(Pen(c, 1f), xa + t0 * dx, ya + t0 * dy, xa + t1 * dx, ya + t1 * dy);
		static bool Edge(float pe, float qe, ref float a0, ref float a1)
		{
			if (pe == 0f)
			{
				return qe >= 0f;
			}
			float t = qe / pe;
			if (float.IsNaN(t))
			{
				return false;
			}
			if (pe < 0f)
			{
				a0 = Math.Max(a0, t);
			}
			else
			{
				a1 = Math.Min(a1, t);
			}
			return true;
		}
	}

	// 2.1: the Pro badge. Absorption: an arrow stopped by a wall (the aggressive side ran into resting orders). Exhaustion:
	// a double chevron that fades (the push runs out, the turn is the other way). Flow divergence: two opposite arrows
	// (price one way, delta the other). Swing divergence: a diamond with the side's dot (it is joined to its swings). A
	// pointer shows the candle; a bar still forming draws it faint and dashed.
	private void GlyphBadge(Graphics g, SigKind k, bool swing, float x, float y, float r, Color col, Color side, bool bull, bool pending)
	{
		int a = (pending ? 120 : 245);
		Color ink = Color.FromArgb(pending ? 150 : 245, 10, 12, 16);
		float d = (bull ? -1f : 1f);   // towards the candle: up for a long badge (it sits under), down for a short one
		Pen halo = Pen(Color.FromArgb(pending ? 80 : 210, 8, 10, 14), 3f);
		Pen edge = (pending ? DashPen(Color.FromArgb(a, Bright(col, 0.3)), 1.2f) : Pen(Color.FromArgb(a, Bright(col, 0.3)), 1.2f));   // 2.3
		PointF[] tri = _points2;
		tri[0] = new PointF(x, y + d * (r + r * 0.55f));
		tri[1] = new PointF(x - r * 0.42f, y + d * r * 0.8f);
		tri[2] = new PointF(x + r * 0.42f, y + d * r * 0.8f);
		g.FillPolygon(Brush(Color.FromArgb(a, col)), tri);
		if (swing)
		{
			PointF[] dia = _points0;
			dia[0] = new PointF(x, y - r * 1.12f);
			dia[1] = new PointF(x + r * 1.12f, y);
			dia[2] = new PointF(x, y + r * 1.12f);
			dia[3] = new PointF(x - r * 1.12f, y);
			g.DrawPolygon(halo, dia);
			g.FillPolygon(Brush(Color.FromArgb(a, col)), dia);
			g.DrawPolygon(edge, dia);
			float c = r * 0.42f;
			g.FillEllipse(Brush(ink), x - c - 1f, y - c - 1f, 2f * c + 2f, 2f * c + 2f);
			g.FillEllipse(Brush(Color.FromArgb(a, side)), x - c, y - c, 2f * c, 2f * c);
			return;
		}
		// a dark disc behind (the halo: readable on any candle) and the badge: two fills, cheaper than wide strokes
		g.FillEllipse(Brush(Color.FromArgb(pending ? 80 : 215, 8, 10, 14)), x - r - 1.5f, y - r - 1.5f, 2f * r + 3f, 2f * r + 3f);
		g.FillEllipse(Brush(Color.FromArgb(a, col)), x - r, y - r, 2f * r, 2f * r);
		if (pending)
		{
			g.DrawEllipse(edge, x - r, y - r, 2f * r, 2f * r);
		}
		Pen p = Pen(ink, Math.Max(1.4f, r * 0.2f));
		float m = r * 0.55f;
		switch (k)
		{
		case SigKind.Absorption:
		{
			// the aggressive side comes from the candle's side and stops at the wall
			float wall = y - d * m * 0.35f;
			g.DrawLine(p, x - m, wall, x + m, wall);
			float from = y + d * m;
			g.DrawLine(p, x, from, x, wall + d * 2f);
			g.DrawLine(p, x - m * 0.4f, wall + d * m * 0.45f, x, wall + d * 2f);
			g.DrawLine(p, x + m * 0.4f, wall + d * m * 0.45f, x, wall + d * 2f);
			break;
		}
		case SigKind.Exhaustion:
		{
			// two chevrons pointing the way of the turn; the second one fades
			float u = -d;
			g.DrawLine(p, x - m * 0.8f, y - u * m * 0.05f, x, y - u * m * 0.6f);
			g.DrawLine(p, x, y - u * m * 0.6f, x + m * 0.8f, y - u * m * 0.05f);
			Pen faint = Pen(Color.FromArgb(pending ? 70 : 130, 10, 12, 16), Math.Max(1.4f, r * 0.2f));
			g.DrawLine(faint, x - m * 0.8f, y + u * m * 0.55f, x, y);
			g.DrawLine(faint, x, y, x + m * 0.8f, y + u * m * 0.55f);
			break;
		}
		default:
		{
			// price one way, delta the other
			float s1 = x - m * 0.45f;
			float s2 = x + m * 0.45f;
			g.DrawLine(p, s1, y + m * 0.75f, s1, y - m * 0.75f);
			g.DrawLine(p, s1 - m * 0.35f, y - m * 0.35f, s1, y - m * 0.75f);
			g.DrawLine(p, s1 + m * 0.35f, y - m * 0.35f, s1, y - m * 0.75f);
			g.DrawLine(p, s2, y - m * 0.75f, s2, y + m * 0.75f);
			g.DrawLine(p, s2 - m * 0.35f, y + m * 0.35f, s2, y + m * 0.75f);
			g.DrawLine(p, s2 + m * 0.35f, y + m * 0.35f, s2, y + m * 0.75f);
			break;
		}
		}
	}

	// 2.0: the clean marker family - the classic shapes on a dark halo (readable on any candle), one stroke, the side's
	// colour in the centre of divergences; pending (forming bar) markers stay faint and dashed as before
	private void GlyphClean(Graphics g, SigKind k, float x, float y, float r, Color col, Color side, bool bull, bool pending)
	{
		Pen halo = Pen(Color.FromArgb(pending ? 90 : 200, 10, 12, 16), 3.2f);
		Pen pen = (pending ? DashPen(Color.FromArgb(160, col), 1.4f) : Pen(Color.FromArgb(250, col), 1.4f));   // 2.3
		switch (k)
		{
		case SigKind.Absorption:
		{
			RectangleF box = new RectangleF(x - r, y - r, 2f * r, 2f * r);
			g.DrawRectangle(halo, box.X, box.Y, box.Width, box.Height);
			g.FillRectangle(Brush(Color.FromArgb(pending ? 70 : 225, col)), box);
			g.DrawRectangle(pen, box.X, box.Y, box.Width, box.Height);
			Pen mark = Pen(Color.FromArgb(240, 12, 14, 18), 2f);
			float m = r * 0.55f;
			float d = bull ? -1f : 1f;
			g.DrawLine(mark, x - m, y - d * m * 0.5f, x, y + d * m * 0.5f);
			g.DrawLine(mark, x, y + d * m * 0.5f, x + m, y - d * m * 0.5f);
			break;
		}
		case SigKind.Exhaustion:
		{
			PointF[] tri = _points2;
			if (!bull)
			{
				tri[0] = new PointF(x, y + r);
				tri[1] = new PointF(x + r, y - r * 0.8f);
				tri[2] = new PointF(x - r, y - r * 0.8f);
			}
			else
			{
				tri[0] = new PointF(x, y - r);
				tri[1] = new PointF(x + r, y + r * 0.8f);
				tri[2] = new PointF(x - r, y + r * 0.8f);
			}
			g.DrawPolygon(halo, tri);
			g.FillPolygon(Brush(Color.FromArgb(pending ? 50 : 200, col)), tri);
			g.DrawPolygon(pen, tri);
			break;
		}
		default:
		{
			PointF[] dia = _points0;
			dia[0] = new PointF(x, y - r);
			dia[1] = new PointF(x + r, y);
			dia[2] = new PointF(x, y + r);
			dia[3] = new PointF(x - r, y);
			g.DrawPolygon(halo, dia);
			g.FillPolygon(Brush(Color.FromArgb(pending ? 70 : 225, col)), dia);
			g.DrawPolygon(pen, dia);
			float c = r * 0.36f;
			g.FillEllipse(Brush(Color.FromArgb(245, 12, 14, 18)), x - c - 1f, y - c - 1f, 2f * c + 2f, 2f * c + 2f);
			g.FillEllipse(Brush(side), x - c, y - c, 2f * c, 2f * c);
			break;
		}
		}
	}

	// 2.0 (A4): the card of the marker the mouse points at: what it is, what CVD PRO saw, how strong, when it was known
	private void HoverCard(Graphics g, Rectangle rect)
	{
		Sig h = _hover;
		if (h == null)
		{
			return;
		}
		Sig sig = null;
		RectangleF box = RectangleF.Empty;
		for (int i = 0; i < _hitList.Count; i++)
		{
			Sig s = _hitList[i].Sig;
			if (s.Abs == h.Abs && s.Kind == h.Kind && s.Bull == h.Bull && s.P1 == h.P1 && s.P2 == h.P2)
			{
				sig = s;
				box = _hitList[i].Box;
				break;
			}
		}
		if (sig == null || sig.Abs >= _pn)
		{
			return;
		}
		Color col = ((sig.Kind == SigKind.Absorption) ? AbsorbColor : ((sig.Kind == SigKind.Exhaustion) ? ExhaustColor : DivergeColor));
		string title = sig.Kind switch
		{
			SigKind.Absorption => sig.Bull ? "ABSORPTION · sellers absorbed (bullish)" : "ABSORPTION · buyers absorbed (bearish)",
			SigKind.Exhaustion => sig.Bull ? "EXHAUSTION · selling fading at a new low (bullish)" : "EXHAUSTION · buying fading at a new high (bearish)",
			_ => (sig.P1 >= 0 ? "SWING DIVERGENCE · " : "FLOW DIVERGENCE · ") + (sig.Bull ? "bullish" : "bearish"),
		};
		double avg = (sig.Avg0 >= 0.0) ? sig.Avg0 : AvgScore(sig);
		int db = DrawBar(sig);
		if (db < sig.Abs)
		{
			db = sig.Abs;
		}
		string strength = "strength " + sig.Score.ToString("0.0", CultureInfo.InvariantCulture) + ((avg > 0.0) ? ("  ·  " + (sig.Score / avg).ToString("0.0", CultureInfo.InvariantCulture) + "× the recent average") : "");
		string when = sig.Pending ? "on the forming bar: it can still change or go" : ("known at " + BarClock(db) + ", when the next bar began");
		List<string> list = new List<string>(5) { sig.Note ?? "", strength, when };
		if (db > sig.Abs)
		{
			list.Add("began " + (db - sig.Abs).ToString(CultureInfo.InvariantCulture) + " bar(s) earlier; strong enough to show from this bar");
		}
		if (KeepFirstSignal && sig.Seen > db)
		{
			list.Add("fired again for " + (sig.Seen - db).ToString(CultureInfo.InvariantCulture) + " more bar(s); the strongest reading is shown");
		}
		string[] lines = list.ToArray();
		float w = TextW(g, title, _fb);
		foreach (string l in lines)
		{
			w = Math.Max(w, TextW(g, l, _f));
		}
		float rowH = _capH + 3f;
		float cw = w + 20f;
		float ch = 8f + rowH * (lines.Length + 1) + 4f;
		// beside the marker (right, else left), never over the info table
		float cy = Math.Max(rect.Top + 4, Math.Min(rect.Bottom - ch - 4f, box.Top + box.Height / 2f - ch / 2f));
		float cx = box.Right + 8f;
		RectangleF table = _tableRect;
		bool hitsTable(float x) => table.Width > 0f && new RectangleF(x, cy, cw, ch).IntersectsWith(table);
		if (cx + cw > rect.Right - 4 || hitsTable(cx))
		{
			cx = box.Left - 8f - cw;
		}
		if (cx < rect.Left + 4 || hitsTable(cx))
		{
			cx = Math.Max(rect.Left + 4, Math.Min(box.Left - 8f - cw, (table.Width > 0f) ? (table.Left - cw - 8f) : (box.Left - 8f - cw)));
		}
		GraphicsPath path = Rounded(_gdi.Path(3), cx, cy, cw, ch, 5f);
		g.FillPath(Brush(Color.FromArgb(242, 8, 10, 14)), path);
		g.DrawPath(Pen(Color.FromArgb(220, col), 1.2f), path);
		g.FillRectangle(Brush(Color.FromArgb(230, col)), cx, cy + 6f, 2f, ch - 12f);
		float ty = cy + 6f;
		g.DrawString(title, _fb, Brush(col), cx + 10f, ty, NoWrap);
		ty += rowH;
		for (int i = 0; i < lines.Length; i++)
		{
			g.DrawString(lines[i], _f, Brush((i == 0) ? TextColor : Color.FromArgb(255, 150, 158, 174)), cx + 10f, ty, NoWrap);
			ty += rowH;
		}
	}

	// 2.0: when CVD PRO knew bar abs was finished: the next bar's open time in the chart's time zone (a bar is read when the
	// next one begins; on range bars that is the trade that broke its range)
	private string BarClock(int abs)
	{
		try
		{
			long ticks = (abs + 1 < _pn) ? _pb[abs + 1].Ticks : _pb[abs].Ticks;
			DateTime t = new DateTime(ticks, DateTimeKind.Utc);
			TimeZoneInfo tz = ChartTz();
			return ((tz == null) ? t : TimeZoneInfo.ConvertTimeFromUtc(t, tz)).ToString("HH:mm:ss", CultureInfo.InvariantCulture);
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "BarClock#1", tnxError);
			return "?";
		}
	}

	// 2.0 (S4): PickLastSig removed - it was never called

	private void Glyph(Graphics g, SigKind k, float x, float y, float r, Color col, Color side, bool bull, bool pending)
	{
		Pen pen = (pending ? DashPen(Color.FromArgb(240, col), 1.4f) : Pen(Color.FromArgb(240, col), 1.4f));   // 2.3
		switch (k)
		{
		case SigKind.Absorption:
		{
			RectangleF rect = new RectangleF(x - r, y - r, 2f * r, 2f * r);
			g.FillRectangle(Brush(Color.FromArgb(pending ? 70 : 200, col)), rect);
			g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
			Pen pen2 = Pen(Color.FromArgb(235, 15, 17, 22), 2f);
			float num = r * 0.55f;
			float num2 = ((!bull) ? 1 : (-1));
			g.DrawLine(pen2, x - num, y - num2 * num * 0.5f, x, y + num2 * num * 0.5f);
			g.DrawLine(pen2, x, y + num2 * num * 0.5f, x + num, y - num2 * num * 0.5f);
			break;
		}
		case SigKind.Exhaustion:
		{
			PointF[] points2 = _points2;   // P4: reused (the same points)
			if (!bull)
			{
				points2[0] = new PointF(x, y + r);
				points2[1] = new PointF(x + r, y - r * 0.8f);
				points2[2] = new PointF(x - r, y - r * 0.8f);
			}
			else
			{
				points2[0] = new PointF(x, y - r);
				points2[1] = new PointF(x + r, y + r * 0.8f);
				points2[2] = new PointF(x - r, y + r * 0.8f);
			}
			g.FillPolygon(Brush(Color.FromArgb(pending ? 40 : 90, col)), points2);
			g.DrawPolygon(pen, points2);
			break;
		}
		default:
		{
			PointF[] points = _points0;   // P4: reused (the same points)
			points[0] = new PointF(x, y - r);
			points[1] = new PointF(x + r, y);
			points[2] = new PointF(x, y + r);
			points[3] = new PointF(x - r, y);
			g.FillPolygon(Brush(Color.FromArgb(pending ? 70 : 210, col)), points);
			g.DrawPolygon(pen, points);
			g.FillEllipse(Brush(side), x - r * 0.3f, y - r * 0.3f, r * 0.6f, r * 0.6f);
			break;
		}
		}
	}

	private void Panel(Graphics g, Rectangle rect, int last)
	{
		Graphics g2 = g;
		TNXCVDPro tNXCVDPro = this;
		if (last < 0 || !_pb[last].Has)
		{
			return;
		}
		ref Bar reference = ref _pb[last];
		int num = Math.Min(Math.Max(20, DominanceBars), last + 1);
		int len = Math.Min(8, num);
		double num2 = Win(_pb, last, num, Pf.Buy);
		double num3 = Win(_pb, last, num, Pf.Sell);
		double num4 = Win(_pb, last, len, Pf.Buy);
		double num5 = Win(_pb, last, len, Pf.Sell);
		double num6 = ((num2 + num3 > 0.0) ? (num2 / (num2 + num3) * 100.0) : 50.0);
		double num7 = ((num4 + num5 > 0.0) ? (num4 / (num4 + num5) * 100.0) : 50.0);
		bool flag = num6 >= 50.0;
		bool flag2 = num7 >= 50.0;
		bool flag3 = flag2 != flag && Math.Abs(num7 - 50.0) >= 8.0 && Math.Abs(num6 - 50.0) >= 5.0;
		Color col;
		string text = AccelText(reference.Fast >= 0.0, reference.AccE, AccNorm(last), reference.ThrustOk, out col);
		Color color = Color.FromArgb(150, 158, 174);
		BarRead(_pb, _pBarHeight, last, out var e, out var r);
		Verdict(reference.Real, e, r, out var text2, out var col2, out var passive);
		string text3 = Fmt(reference.C) + "   bar " + FmtS(reference.Delta);
		double sc = ScaleC(_pb, last);
		string text4 = FlowText(_pb, last, sc);
		double num8 = base.Symbol?.TickSize ?? 0.0;
		string text5 = FmtS(e, "0.0") + "x  " + ((Math.Abs(e) < 0.7) ? "nobody" : ((e >= 0.0) ? "buyers" : "sellers")) + "   bar Δ " + FmtS(reference.Delta);
		string text6 = FmtS(r, "0.0") + "x  " + ((Math.Abs(r) < 0.7) ? "flat" : ((r >= 0.0) ? "up" : "down")) + ((num8 > 0.0) ? ("   " + FmtS(Math.Round((reference.Px - reference.PO) / num8), "0") + " ticks") : "");
		string text7 = ((passive > 0) ? "Passive buyers (limit orders)" : ((passive < 0) ? "Passive sellers (limit orders)" : "–"));
		Color col3 = ((passive > 0) ? BullColor : ((passive < 0) ? BearColor : color));
		string text8 = (flag ? "Buyers " : "Sellers ") + (flag ? num6 : (100.0 - num6)).ToString("0", CultureInfo.InvariantCulture) + "%   " + num + " bars";
		string text9 = (flag2 ? "Buyers " : "Sellers ") + (flag2 ? num7 : (100.0 - num7)).ToString("0", CultureInfo.InvariantCulture) + "%   " + len + " bars" + (flag3 ? "   ← TURNING" : "");
		double v = (num6 - 50.0) / 50.0 * 4.0;
		double v2 = (num7 - 50.0) / 50.0 * 4.0;
		float rowH = _capH + 3f;
		string[] array = new string[9] { "CVD", "Flow speed", "Acceleration", "Pushing (effort)", "Price move (result)", "Market verdict", "Absorbed by", "In control", "Right now" };
		float num9 = 0f;
		if (_labelFont != _f || _labelDpi != g2.DpiX)
		{
			// 2.0 (S2): the fixed labels are measured once per font and screen scale, not on every picture (the same widths)
			string[] array2 = array;
			foreach (string text10 in array2)
			{
				num9 = Math.Max(num9, TextW(g2, text10, _f));
			}
			_labelW = num9;
			_labelFont = _f;
			_labelDpi = g2.DpiX;
		}
		else
		{
			num9 = _labelW;
		}
		float val = 0f;
		val = Math.Max(val, TextW(g2, text3, _fb));
		val = Math.Max(val, TextW(g2, text4, _fb));
		val = Math.Max(val, TextW(g2, text, _fb));
		val = Math.Max(val, 52f + TextW(g2, text5, _fb));
		val = Math.Max(val, 52f + TextW(g2, text6, _fb));
		val = Math.Max(val, TextW(g2, text2, _fb));
		val = Math.Max(val, TextW(g2, text7, _fb));
		val = Math.Max(val, 52f + TextW(g2, text8, _fb));
		val = Math.Max(val, 52f + TextW(g2, text9, _fb));
		float val2 = 6f + num9 + 10f + val + 6f + 6f;
		val2 = Math.Min((float)rect.Width - 12f, val2);
		float num10 = 6f + rowH * 9f + 6f - 2f;
		float num11 = ((Table == Corner.BottomLeft) ? ((float)(rect.Left + 6)) : ((float)rect.Right - val2 - (_axisW + 14f)));
		float num12 = ((Mode == Display.Velocity) ? ((float)(rect.Top + 28)) : ((float)rect.Bottom - num10 - 6f));
		_tableRect = new RectangleF(num11, num12, val2, num10);
		{
			GraphicsPath path = Rounded(_gdi.Path(1), num11, num12, val2, num10, 5f);
			g2.FillPath(Brush(Color.Black), path);
			g2.DrawPath(Pen(Color.FromArgb(255, 44, 48, 56), 1f), path);
		}
		g2.FillRectangle(Brush(Color.FromArgb(220, flag ? BullColor : BearColor)), num11, num12 + 8f, 2f, num10 - 16f);
		float lx = num11 + 6f + 4f;
		float cx = lx + num9 + 10f;
		float rx = num11 + val2 - 6f;
		float cy = num12 + 6f;
		SolidBrush dim = Brush(Color.FromArgb(255, 128, 136, 152));
		Pen sep = Pen(Color.FromArgb(255, 24, 27, 34), 1f);
		int num13;
		object obj;
		if (!reference.Real)
		{
			num13 = (VaReady() ? 1 : 0);
			if (num13 == 0)
			{
				obj = "LOADING";
				goto IL_090b;
			}
		}
		else
		{
			num13 = 1;
		}
		obj = "LIVE";
		goto IL_090b;
		IL_090b:
		string text11 = (string)obj;
		Color color2 = ((num13 != 0) ? BullColor : ExhaustColor);
		float num14 = TextW(g2, text11, _fs) + 16f;
		float capH = _capH;
		float num15 = rx - num14;
		float num16 = cy;
		{
			GraphicsPath path2 = Rounded(_gdi.Path(2), num15, num16, num14, capH, 3f);
			g2.FillPath(Brush(Color.FromArgb(50, color2)), path2);
		}
		g2.FillEllipse(Brush(color2), num15 + 5f, num16 + capH / 2f - 2.5f, 5f, 5f);
		g2.DrawString(text11, _fs, Brush(color2), num15 + 12f, num16, NoWrap);
		Row(array[0], text3, (reference.C >= 0.0) ? BullColor : BearColor);
		Row(array[1], text4, (reference.Fast >= 0.0) ? BullColor : BearColor);
		Row(array[2], text, col);
		MeterRow(array[3], e, 4.0, text5, (Math.Abs(e) < 0.7) ? color : ((e >= 0.0) ? BullColor : BearColor));
		MeterRow(array[4], r, 2.0, text6, (Math.Abs(r) < 0.7) ? color : ((r >= 0.0) ? BullColor : BearColor));
		Row(array[5], text2, col2);
		Row(array[6], text7, col3);
		MeterRow(array[7], v, 4.0, text8, flag ? BullColor : BearColor);
		MeterRow(array[8], v2, 4.0, text9, flag2 ? BullColor : BearColor);
		void MeterRow(string label, double num21, double full, string s, Color c)
		{
			g2.DrawString(label, _f, dim, lx, cy, NoWrap);
			float num17 = 4f;
			float num18 = cy + rowH / 2f - 3f;
			float num19 = 22f;
			float num20 = (float)Math.Min(1.0, Math.Abs(num21) / full);
			g2.FillRectangle(Brush(Color.FromArgb(255, 38, 42, 52)), cx, num18, 44f, num17);
			if (num21 >= 0.0)
			{
				g2.FillRectangle(Brush(c), cx + num19, num18, num19 * num20, num17);
			}
			else
			{
				g2.FillRectangle(Brush(c), cx + num19 - num19 * num20, num18, num19 * num20, num17);
			}
			g2.DrawLine(Pen(Color.FromArgb(140, 200, 200, 200), 1f), cx + num19, num18 - 1f, cx + num19, num18 + num17 + 1f);
			g2.DrawString(s, _fb, Brush(c), cx + 44f + 8f, cy, NoWrap);
			cy += rowH;
			g2.DrawLine(sep, lx, cy - 2f, rx, cy - 2f);
		}
		void Row(string label, string s, Color c, bool bold = true)
		{
			g2.DrawString(label, _f, dim, lx, cy, NoWrap);
			g2.DrawString(s, bold ? _fb : _f, Brush(c), cx, cy, NoWrap);
			cy += rowH;
			g2.DrawLine(sep, lx, cy - 2f, rx, cy - 2f);
		}
	}

	// 2.1: the info card. The readings are the classic table's (the same formulas) in short plain words, plus the bar that
	// is forming and, on range and tick charts, the last finished bar (2.0, F1 + A1). Its layout depends only on the window
	// and the settings, never on the numbers it shows: its text size is "2.1 Card size", its width "2.1 Card width" (or the
	// widest text it can show, measured once), and when the window is too short its rows go into 2 to 4 columns, then the
	// text shrinks (not under 80 %). Every text stays inside its cell (a longer one ends in "..."). Nothing here computes a
	// new number.
	private void PanelCard(Graphics g, Rectangle rect, int last)
	{
		if (last < 0 || !_pb[last].Has || _cf == null || _rf == null)
		{
			return;
		}
		ref Bar b = ref _pb[last];
		// ---- the readings (the classic table's numbers) ----
		int nCtl = Math.Min(Math.Max(20, DominanceBars), last + 1);
		int nNow = Math.Min(8, nCtl);
		double buyC = Win(_pb, last, nCtl, Pf.Buy);
		double sellC = Win(_pb, last, nCtl, Pf.Sell);
		double buyN = Win(_pb, last, nNow, Pf.Buy);
		double sellN = Win(_pb, last, nNow, Pf.Sell);
		double pctC = ((buyC + sellC > 0.0) ? (buyC / (buyC + sellC) * 100.0) : 50.0);
		double pctN = ((buyN + sellN > 0.0) ? (buyN / (buyN + sellN) * 100.0) : 50.0);
		bool buyersC = pctC >= 50.0;
		bool buyersN = pctN >= 50.0;
		bool turning = buyersN != buyersC && Math.Abs(pctN - 50.0) >= 8.0 && Math.Abs(pctC - 50.0) >= 5.0;
		BarRead(_pb, _pBarHeight, last, out var e, out var r);
		Verdict(b.Real, e, r, out var tVerdict, out var colVerdict, out var passive);
		Color dim = Color.FromArgb(255, 128, 136, 152);
		Color soft = Color.FromArgb(255, 196, 202, 214);
		double sc = ScaleC(_pb, last);
		string tFast = FlowWords(_pb, last, sc, slow: false, out var colFast);
		string tSlow = FlowWords(_pb, last, sc, slow: true, out var colSlow);
		string tAcc = AccelWords(b.Fast >= 0.0, b.AccE, AccNorm(last), b.ThrustOk, out var colAcc);
		double tick = base.Symbol?.TickSize ?? 0.0;
		string tPush = ((Math.Abs(e) < 0.7) ? "nobody" : (((e >= 0.0) ? "Buyers " : "Sellers ") + Math.Abs(e).ToString("0.0", CultureInfo.InvariantCulture) + "×"));
		Color colPush = ((Math.Abs(e) < 0.7) ? dim : ((e >= 0.0) ? BullColor : BearColor));
		string tMove = ((Math.Abs(r) < 0.7) ? "flat" : (((r >= 0.0) ? "Up " : "Down ") + Math.Abs(r).ToString("0.0", CultureInfo.InvariantCulture) + "×")) + ((tick > 0.0) ? (" · " + FmtS(Math.Round((b.Px - b.PO) / tick), "0") + " ticks") : "");
		Color colMove = ((Math.Abs(r) < 0.7) ? dim : ((r >= 0.0) ? BullColor : BearColor));
		string tPassive = ((passive > 0) ? "passive buyers" : ((passive < 0) ? "passive sellers" : "–"));
		Color colPassive = ((passive > 0) ? BullColor : ((passive < 0) ? BearColor : dim));
		// the last finished bar (range and tick charts): the table shows the forming bar unless "closed bars only" is on
		int lb = LaneClosed ? last : (last - 1);
		bool lastBar = ShowLastBar && (_agg == Agg.Range || _agg == Agg.Tick) && lb >= 1 && lb + 1 < _pn && _pb[lb].Has;
		if (lastBar)
		{
			var key = (lb, _pb[lb].Ticks, _pb[lb + 1].Ticks, _pb[lb].Delta, _pb[lb].Px, Skin, CustomBull.ToArgb(), CustomBear.ToArgb());
			if (_lastBarKey != key || _lastBarSpeed == null)
			{
				_lastBarKey = key;
				_lastBarSpeed = ShortSpeed(LastBarSpeed(lb, out _lastBarSpeedCol));
				_lastBarReading = ShortReading(LastBarReading(lb, out _lastBarReadingCol));
			}
		}
		// the bar that is forming: how far it is (range: its range so far against the full one; tick: trades so far). The
		// row is always there on these charts (a row that comes and goes would move the card)
		bool liveRow = !LaneClosed && (_agg == Agg.Range || _agg == Agg.Tick || _agg == Agg.Time);
		double f = Math.Max(0.0, Math.Min(1.0, b.F));
		string tLive = ((_agg == Agg.Tick && _aggN > 0.0) ? (Math.Round(f * _aggN).ToString("0", CultureInfo.InvariantCulture) + " of " + _aggN.ToString("0", CultureInfo.InvariantCulture) + " trades") : ((f * 100.0).ToString("0", CultureInfo.InvariantCulture) + "% done"));
		string badge = ((!b.Real && !VaReady()) ? "LOADING" : "LIVE");
		string tag = _pChartTag ?? "";
		string cvd = FmtS(b.C);
		string barChip = "bar " + FmtS(b.Delta);
		// ---- the rows (always the same rows for a chart: the layout never moves with the numbers) ----
		bool compact = CardLayout == CardRows.Compact;
		List<CardRow> rows = _cardRows;
		rows.Clear();
		if (liveRow)
		{
			rows.Add(new CardRow(5, "Bar now", tLive, (b.Delta >= 0.0) ? BullColor : BearColor, f));
		}
		rows.Add(new CardRow(6, null, cvd, (b.C >= 0.0) ? BullColor : BearColor, b.Delta));
		rows.Add(new CardRow(4));
		if (CardCompass)
		{
			// 2.3: the compass row: who pushes (needle direction), how busy (its length), the acceleration (gold arrow), and the
			// warning (REAL PUSH / THIN / BATTLE / ABSORBED / FADING). It replaces the three flow text rows; its numbers come from
			// the same bar. Nothing here is computed in the picture: Compute made every dial.
			bool ranks = b.BusyRank >= 0.0 && b.ImbRank >= 0.0;
			Color sideCol = ((b.Fast >= 0.0) ? BullColor : BearColor);
			string vTxt;
			Color vCol;
			if (!b.Real || !ranks)
			{
				vTxt = "no data yet";
				vCol = dim;
			}
			else if (b.Absorbed)
			{
				vTxt = "ABSORBED ⚠ effort, no result";
				vCol = AbsorbColor;
			}
			else if (b.BusyRank < 0.4 && Math.Abs(b.Imb7) >= 0.2)
			{
				vTxt = "THIN ⚠ no real volume";
				vCol = ExhaustColor;
			}
			else if (b.BusyRank >= 0.6 && b.ImbRank < 0.35)
			{
				vTxt = "BATTLE ⚠ busy, two-sided";
				vCol = ExhaustColor;
			}
			else if (b.Fade)
			{
				vTxt = "FADING · push losing one-sidedness";
				vCol = ExhaustColor;
			}
			else if (b.Q >= 0.5)
			{
				vTxt = "REAL PUSH";
				vCol = sideCol;
			}
			else
			{
				vTxt = "normal";
				vCol = dim;
			}
			double accN = AccNorm(last);
			_cmp = new CompassData
			{
				Ready = ranks, Buyers = b.Fast >= 0.0, Imb = b.Imb7, Busy = (ranks ? b.BusyRank : 0.5), Tier = ((b.FRank < 0.0) ? -1 : ((b.FRank < 0.5) ? 0 : ((b.FRank < 0.8) ? 1 : ((b.FRank < 0.95) ? 2 : 3)))),
				AccN = Math.Abs(accN), AccWith = (b.AccE >= 0.0) == (b.Fast >= 0.0), AccWords = tAcc, AccCol = colAcc, Verdict = vTxt, VerdictCol = vCol
			};
			rows.Add(new CardRow(7, "Flow", tFast, colFast));
		}
		else
		{
			rows.Add(new CardRow(0, "Flow now", tFast, colFast));
			if (!compact)
			{
				rows.Add(new CardRow(0, "Flow trend", tSlow, colSlow));
				rows.Add(new CardRow(0, "Speed", tAcc, colAcc));
			}
		}
		rows.Add(new CardRow(4));
		if (!compact)
		{
			rows.Add(new CardRow(1, "Push", tPush, colPush, e, 4.0));
			rows.Add(new CardRow(1, "Move", tMove, colMove, r, 2.0));
		}
		rows.Add(new CardRow(3, "Reading", CardWords(tVerdict), colVerdict));
		if (!compact)
		{
			rows.Add(new CardRow(0, "Absorbed by", tPassive, colPassive));
		}
		if (lastBar)
		{
			rows.Add(new CardRow(4));
			if (!compact)
			{
				rows.Add(new CardRow(0, "Last bar", _lastBarSpeed, _lastBarSpeedCol));
			}
			rows.Add(new CardRow(3, compact ? "Last bar" : "Last reading", _lastBarReading, _lastBarReadingCol));
		}
		rows.Add(new CardRow(4));
		rows.Add(new CardRow(2, "Control " + nCtl.ToString(CultureInfo.InvariantCulture), (buyersC ? "Buyers " : "Sellers ") + (buyersC ? pctC : (100.0 - pctC)).ToString("0", CultureInfo.InvariantCulture) + "%", buyersC ? BullColor : BearColor, pctC));
		if (!compact)
		{
			rows.Add(new CardRow(2, "Now " + nNow.ToString(CultureInfo.InvariantCulture), (buyersN ? "Buyers " : "Sellers ") + (buyersN ? pctN : (100.0 - pctN)).ToString("0", CultureInfo.InvariantCulture) + "%" + (turning ? "  TURNING" : ""), buyersN ? BullColor : BearColor, pctN));
		}
		// ---- where it may go ----
		CardPlace place = CardCorner;
		bool top;
		bool left;
		if (place == CardPlace.Auto)
		{
			left = true;   // over the oldest bars: the newest (right) stay clear
			top = true;
		}
		else if (place == CardPlace.InfoTable)
		{
			left = Table == Corner.BottomLeft;
			top = Mode == Display.Velocity || Table == Corner.TopRight;
		}
		else
		{
			left = place == CardPlace.TopLeft || place == CardPlace.BottomLeft;
			top = place == CardPlace.TopLeft || place == CardPlace.TopRight;
		}
		// the space it may take. On the right (the newest bars): in Velocity mode the CVD area above the lanes, otherwise at
		// most 55 % of the height. On the left (the oldest bars): down to 85 % of the window. Width: at most 45 % of the
		// window, or exactly the width asked for.
		float topY = (float)rect.Top + 28f;
		float availH = (top ? ((float)rect.Bottom - 6f - topY) : ((float)rect.Height - 12f));
		float maxH;
		if (left)
		{
			maxH = Math.Min(availH, (float)rect.Height * 0.85f);
		}
		else if (top && Mode == Display.Velocity && _spTop > topY + 20f)
		{
			maxH = _spTop - 6f - topY;
		}
		else
		{
			maxH = Math.Min(availH, (float)rect.Height * 0.55f);
		}
		maxH = Math.Max(40f, maxH);
		bool fixedW = CardWidth > 0;
		float maxW = Math.Max(100f, Math.Min((float)rect.Width - 12f, fixedW ? (float)CardWidth : ((float)rect.Width * ((rect.Height < 140) ? 0.55f : 0.45f))));
		// ---- the sizes at 100 %, from texts that do not change (measured once per font) ----
		CardTemplates(g);
		float pad1 = 8f;
		float row1 = _rfLineH + 3f;
		float sep1 = 5f;
		float hero1 = _rfhLineH + 3f;
		float colW1 = _tplLabelW + 12f + _tplValueW;
		int nGroups = CardGroups(rows, row1, hero1);
		// ---- the fit: the largest size (up to "2.1 Card size", at least 80 %) and the fewest columns that fit ----
		float sMax = Math.Max(0.7f, Math.Min(1.6f, CardSize / 100f));
		float sMin = Math.Min(sMax, 0.75f);
		int cMax = Math.Min(2, nGroups);   // a compact block, never a long strip
		float bestS = sMin;
		int bestC = 1;
		bool found = false;
		for (float s = sMax; s >= sMin - 0.001f && !found; s -= 0.05f)
		{
			for (int c = 1; c <= cMax; c++)
			{
				float w = (fixedW ? maxW : (s * (2f * pad1 + (float)c * colW1 + (float)(c - 1) * 16f)));
				float colInside = (fixedW ? ((maxW - s * (2f * pad1 + (float)(c - 1) * 16f)) / (float)c) : (s * colW1));
				float h = s * (pad1 + row1 + 3f + ColumnsHeight(_cardGroupH, nGroups, c, sep1) + pad1 - 2f);
				if (w <= maxW && h <= maxH && colInside >= s * 110f)
				{
					bestS = s;
					bestC = c;
					found = true;
					break;
				}
			}
		}
		if (!found)
		{
			// a very short window: 3 or 4 columns at the smallest size, if they fit (still at most 45 % of the width)
			for (int c = 3; c <= Math.Min(4, nGroups) && !found; c++)
			{
				float w = (fixedW ? maxW : (sMin * (2f * pad1 + (float)c * colW1 + (float)(c - 1) * 16f)));
				float colInside = (fixedW ? ((maxW - sMin * (2f * pad1 + (float)(c - 1) * 16f)) / (float)c) : (sMin * colW1));
				float h = sMin * (pad1 + row1 + 3f + ColumnsHeight(_cardGroupH, nGroups, c, sep1) + pad1 - 2f);
				if (w <= maxW && h <= maxH && colInside >= sMin * 110f)
				{
					bestS = sMin;
					bestC = c;
					found = true;
				}
			}
		}
		if (!found)
		{
			// nothing fits: 75 %, as many columns (1 or 2) as the width allows, every row kept; the place follows the
			// setting, the 'Info table' place moves left over the oldest bars
			bestS = sMin;
			for (int c = cMax; c >= 1; c--)
			{
				float w = (fixedW ? maxW : (bestS * (2f * pad1 + (float)c * colW1 + (float)(c - 1) * 16f)));
				float colInside = (fixedW ? ((maxW - bestS * (2f * pad1 + (float)(c - 1) * 16f)) / (float)c) : (bestS * colW1));
				if (w <= maxW && colInside >= bestS * 110f)
				{
					bestC = c;
					break;
				}
			}
			if (place == CardPlace.InfoTable)
			{
				left = true;
			}
		}
		if (Math.Abs(bestS - _cfScale) > 0.01f)
		{
			// a new size (a resized window or new settings): the card's fonts are made again, once
			_cfScale = bestS;
			DisposeCardFonts();
			Fonts();
		}
		// ---- the layout ----
		float k = _cfScale;
		float pad = pad1 * k;
		float rowH = _cfLineH + 3f * k;
		float sepH = sep1 * k;
		float meterW = 54f * k;
		int cols = bestC;
		float gapC = 16f * k;
		float width = (fixedW ? maxW : Math.Min(maxW, 2f * pad + (float)cols * colW1 * k + (float)(cols - 1) * gapC));
		width = Math.Max(width, Math.Min(maxW, k * (2f * pad1 + _tplTitleW)));
		float colW = (width - 2f * pad - (float)(cols - 1) * gapC) / (float)cols;
		float labelW = Math.Min(_tplLabelW * k, colW * 0.42f);
		int[] cs = _cardColStart;
		AssignColumns(_cardGroupH, _cardGroupStart, nGroups, cols, cs);
		float colH = 0f;
		for (int c = 0; c < cols; c++)
		{
			int from = cs[c];
			int to = ((c + 1 < cols) ? cs[c + 1] : rows.Count);
			float h = 0f;
			for (int i = from; i < to; i++)
			{
				if (rows[i].Kind == 4 && (i == from || i == to - 1))
				{
					continue;
				}
				h += ((rows[i].Kind == 4) ? sepH : ((rows[i].Kind == 6) ? (_cfhLineH + 3f * k) : ((rows[i].Kind == 7) ? (rowH * CompassRows) : rowH)));   // 2.3: the compass is 3.2 rows tall
			}
			colH = Math.Max(colH, h);
		}
		float height = pad + rowH + 3f * k + colH + pad - 2f * k;
		// ---- place ----
		float x = (left ? ((float)rect.Left + 6f) : ((float)rect.Right - width - (_axisW + 14f)));
		float y = (top ? topY : ((float)rect.Bottom - height - 6f));
		x = Math.Max(rect.Left, Math.Min((float)rect.Right - width, x + (float)CardOffsetX));
		y = Math.Max(rect.Top, Math.Min((float)rect.Bottom - height, y + (float)CardOffsetY));
		_tableRect = new RectangleF(x, y, width, height);
		DrawCard(g, x, y, width, height, k, pad, rowH, sepH, meterW, labelW, colW, cols, cs, rows, badge, tag, cvd, barChip, b.Delta, buyersC, dim, soft);
	}

	private float _cfScale = -1f;   // 2.1: the size the card's fonts are made at (the fit chooses it)

	private readonly float[] _cardGroupH = new float[16];

	private readonly int[] _cardGroupStart = new int[16];

	private readonly int[] _cardColStart = new int[4];

	private Font _tplFont;   // 2.1: the reference font the template widths below were measured with

	private float _tplDpi;   // 2.1: and the screen scale

	private float _tplLabelW;

	private float _tplValueW;

	private float _tplTitleW;

	// 2.1: the widest label and the widest value the card can show, from fixed texts (measured once per font): the card's
	// width never follows the live numbers
	private void CardTemplates(Graphics g)
	{
		if (ReferenceEquals(_tplFont, _rf) && _tplDpi == g.DpiX)
		{
			return;
		}
		float lab = 0f;
		foreach (string l in new string[12] { "Bar now", "Flow now", "Flow trend", "Speed", "Push", "Move", "Reading", "Absorbed by", "Last bar", "Last reading", "Control 9999", "Now 99" })
		{
			lab = Math.Max(lab, g.MeasureString(l, _rf, PointF.Empty, NoWrap).Width);
		}
		float meter = 54f;
		float val = 0f;
		val = Math.Max(val, g.MeasureString("Selling · EXTREME · 100%", _rfb, PointF.Empty, NoWrap).Width);
		val = Math.Max(val, g.MeasureString("Sellers speeding up  9.9×", _rfb, PointF.Empty, NoWrap).Width);
		val = Math.Max(val, meter + 8f + g.MeasureString("Down 9.9× · -99 ticks", _rfb, PointF.Empty, NoWrap).Width);
		val = Math.Max(val, g.MeasureString("SELLERS ABSORBED", _rfb, PointF.Empty, NoWrap).Width + 14f);
		val = Math.Max(val, g.MeasureString("▼ 99.9 min · 9.9× slower", _rfb, PointF.Empty, NoWrap).Width);
		val = Math.Max(val, meter + 8f + g.MeasureString("Sellers 100%", _rfb, PointF.Empty, NoWrap).Width + 8f + g.MeasureString("TURNING", _rfs, PointF.Empty, NoWrap).Width + 12f);
		val = Math.Max(val, meter * 1.4f + 8f + g.MeasureString("999 of 999 trades", _rfs, PointF.Empty, NoWrap).Width);
		// 2.3: the compass row: the gauge (3.2 rows wide at most) and its widest line, measured as the value column
		float gauge = (_rfLineH + 3f) * CompassRows * 0.95f + 10f;
		val = Math.Max(val, gauge + g.MeasureString("busy 100% · EXTREME", _rf, PointF.Empty, NoWrap).Width - lab - 12f);
		val = Math.Max(val, gauge + g.MeasureString("SELLERS 100% one-sided", _rfb, PointF.Empty, NoWrap).Width + 8f + g.MeasureString("ABSORBED ⚠ effort, no result", _rfb, PointF.Empty, NoWrap).Width + 14f - lab - 12f);
		float hero = g.MeasureString("-888.8K", _rfh, PointF.Empty, NoWrap).Width + g.MeasureString("bar -88.8K", _rfb, PointF.Empty, NoWrap).Width + 28f;
		_tplLabelW = lab;
		_tplValueW = Math.Max(val, hero - lab - 12f);
		_tplTitleW = g.MeasureString("CVD PRO", _rfb, PointF.Empty, NoWrap).Width + g.MeasureString("range 99t", _rfs, PointF.Empty, NoWrap).Width + g.MeasureString("LOADING", _rfs, PointF.Empty, NoWrap).Width + 54f;
		_tplFont = _rf;
		_tplDpi = g.DpiX;
	}

	// 2.1: the groups of rows (between the separator rows), their first rows and their heights at 100 %
	private int CardGroups(List<CardRow> rows, float row1, float hero1)
	{
		float[] gh = _cardGroupH;
		int[] gs = _cardGroupStart;
		int n = 0;
		gs[0] = 0;
		gh[0] = 0f;
		for (int i = 0; i < rows.Count; i++)
		{
			if (rows[i].Kind == 4)
			{
				if (gh[n] > 0f && n < gh.Length - 1)
				{
					n++;
					gs[n] = i + 1;
					gh[n] = 0f;
				}
				continue;
			}
			gh[n] += ((rows[i].Kind == 6) ? hero1 : ((rows[i].Kind == 7) ? (row1 * CompassRows) : row1));   // 2.3
		}
		if (gh[n] > 0f)
		{
			n++;
		}
		return Math.Max(1, n);
	}

	// the height of the tallest column with the best split of the groups (in order) over c columns
	private static float ColumnsHeight(float[] gh, int n, int c, float sep)
	{
		return BestSplit(gh, n, c, sep, null);
	}

	// the first row of each column, with the same best split
	private void AssignColumns(float[] gh, int[] gs, int n, int c, int[] cs)
	{
		int[] first = _splitFirst;
		BestSplit(gh, n, c, sep: 0f, first);
		int cc = Math.Max(1, Math.Min(c, n));
		for (int j = 0; j < c; j++)
		{
			cs[j] = ((j < cc) ? gs[first[j]] : gs[Math.Max(0, n - 1)]);
		}
	}

	private readonly int[] _splitFirst = new int[4];

	// 2.1: every way to cut the groups (in order) into c columns (at most 10 ways for the card's 5 or 6 groups); the one
	// with the lowest tallest column. firstGroup gets the first group of each column.
	private static float BestSplit(float[] gh, int n, int c, float sep, int[] firstGroup)
	{
		c = Math.Max(1, Math.Min(c, Math.Max(1, n)));
		Span<int> cur = stackalloc int[4];
		cur[0] = 0;
		float best = float.MaxValue;
		Split(gh, n, c, sep, 1, 1, cur, ref best, firstGroup);
		return best;
	}

	private static void Split(float[] gh, int n, int c, float sep, int col, int from, Span<int> cur, ref float best, int[] firstGroup)
	{
		if (col == c)
		{
			float worst = 0f;
			for (int j = 0; j < c; j++)
			{
				int a = cur[j];
				int b = ((j + 1 < c) ? cur[j + 1] : n);
				float h = 0f;
				for (int i = a; i < b; i++)
				{
					h += gh[i] + ((i > a) ? sep : 0f);
				}
				worst = Math.Max(worst, h);
			}
			if (worst < best - 0.001f)
			{
				best = worst;
				if (firstGroup != null)
				{
					for (int j = 0; j < c; j++)
					{
						firstGroup[j] = cur[j];
					}
				}
			}
			return;
		}
		for (int s = from; s <= n - (c - col); s++)
		{
			cur[col] = s;
			Split(gh, n, c, sep, col + 1, s + 1, cur, ref best, firstGroup);
		}
	}

	// 2.3: the compass row's numbers (made in PanelCard, drawn by DrawCompass in the same picture)
	private struct CompassData
	{
		public bool Ready;

		public bool Buyers;

		public double Imb;

		public double Busy;

		public int Tier;

		public double AccN;

		public bool AccWith;

		public string AccWords;

		public Color AccCol;

		public string Verdict;

		public Color VerdictCol;
	}

	private CompassData _cmp;

	private const float CompassRows = 3.2f;

	// 2.3: a half-dial. Needle direction = who dominates (buyers right, sellers left; delta / traded volume over the fast window,
	// full deflection at 60 %). Needle length = how busy (volume per second, rank in the lookback; rings at 50 / 80 %). A gold
	// arrow at the tip = the acceleration (outward: speeding up, inward: slowing; length by its size). Three lines beside it.
	private void DrawCompass(Graphics g, float lx, float rx, float cy, float hgt, float rowH, float k, Color dim, Color soft)
	{
		CompassData d = _cmp;
		float R = Math.Min(hgt * 0.95f, (rx - lx) * 0.36f) / 2f;
		if (R < 6f)
		{
			return;
		}
		float cx = lx + R + 1f * k;
		float cyc = cy + 2f * k + R;
		Color bull = BullColor;
		Color bear = BearColor;
		g.FillPie(Brush(Color.FromArgb(30, bull)), cx - R, cyc - R, 2f * R, 2f * R, 270f, 90f);
		g.FillPie(Brush(Color.FromArgb(30, bear)), cx - R, cyc - R, 2f * R, 2f * R, 180f, 90f);
		Pen ring = Pen(Color.FromArgb(70, 150, 158, 175), 1f);
		g.DrawArc(ring, cx - R * 0.5f, cyc - R * 0.5f, R, R, 180f, 180f);
		g.DrawArc(ring, cx - R * 0.8f, cyc - R * 0.8f, R * 1.6f, R * 1.6f, 180f, 180f);
		g.DrawArc(Pen(Color.FromArgb(110, 150, 158, 175), 1f), cx - R, cyc - R, 2f * R, 2f * R, 180f, 180f);
		g.DrawLine(ring, cx - R, cyc, cx + R, cyc);
		double imb = Math.Max(-1.0, Math.Min(1.0, d.Imb / 0.6));
		double ang = Math.PI + (imb + 1.0) * Math.PI / 2.0;
		float len = R * (float)Math.Max(0.15, d.Busy);
		float tx = cx + (float)(len * Math.Cos(ang));
		float ty = cyc + (float)(len * Math.Sin(ang));
		Color side = (d.Buyers ? bull : bear);
		Color nc = (d.Ready ? TextColor : dim);
		g.DrawLine(Pen(Color.FromArgb(235, nc), Math.Max(1.5f, 2f * k)), cx, cyc, tx, ty);
		float hub = 2.5f * k;
		g.FillEllipse(Brush(Color.FromArgb(255, nc)), cx - hub, cyc - hub, 2f * hub, 2f * hub);
		float dot = 3.5f * k;
		g.FillEllipse(Brush(Color.FromArgb(255, 245, 247, 250)), tx - dot - 1f, ty - dot - 1f, 2f * dot + 2f, 2f * dot + 2f);
		g.FillEllipse(Brush(Color.FromArgb(255, d.Ready ? side : dim)), tx - dot, ty - dot, 2f * dot, 2f * dot);
		if (d.Ready && d.AccN >= AccelMin)
		{
			// the acceleration arrow along the needle: outward = speeding up, inward = slowing
			float dir = (d.AccWith ? 1f : (-1f));
			float al = R * 0.38f * (float)Math.Min(1.0, d.AccN / Math.Max(AccelExtreme, 0.5));
			float ux = (float)Math.Cos(ang);
			float uy = (float)Math.Sin(ang);
			float ax0 = tx + ux * dir * (dot + 2f * k);
			float ay0 = ty + uy * dir * (dot + 2f * k);
			float ax1 = ax0 + ux * dir * al;
			float ay1 = ay0 + uy * dir * al;
			Pen ap = Pen(Color.FromArgb(250, SignalColor), Math.Max(1.4f, 1.8f * k));
			g.DrawLine(ap, ax0, ay0, ax1, ay1);
			float hw = 3.2f * k;
			float px = -uy;
			float py = ux;
			g.DrawLine(ap, ax1, ay1, ax1 - ux * dir * hw + px * hw, ay1 - uy * dir * hw + py * hw);
			g.DrawLine(ap, ax1, ay1, ax1 - ux * dir * hw - px * hw, ay1 - uy * dir * hw - py * hw);
		}
		// the words
		float x0 = cx + R + 8f * k;
		float ty0 = cy + 1f * k;
		string who = (d.Ready ? ((d.Imb >= 0.0 ? "BUYERS " : "SELLERS ") + (Math.Abs(d.Imb) * 100.0).ToString("0", CultureInfo.InvariantCulture) + "% one-sided") : "waiting for 20 bars");
		CellText(g, who, _cfb, d.Ready ? ((d.Imb >= 0.0) ? bull : bear) : dim, x0, ty0, rx, _cfLineH);
		string tierW = ((d.Tier < 0) ? "" : ((d.Tier == 0) ? " · quiet" : ((d.Tier == 1) ? " · building" : ((d.Tier == 2) ? " · strong" : " · EXTREME"))));
		string acc = d.AccWords ?? "";
		if (acc.StartsWith("Buyers ", StringComparison.Ordinal) || acc.StartsWith("Sellers ", StringComparison.Ordinal))
		{
			acc = acc.Substring(acc.IndexOf(' ') + 1);   // the first line already says who
		}
		string busy = "busy " + (d.Busy * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%" + tierW;
		CellText(g, busy, _cf, soft, x0, ty0 + rowH, rx, _cfLineH);
		CellText(g, ((d.AccN >= AccelMin) ? (d.AccWith ? "▲ " : "▼ ") : "") + acc, _cf, d.AccCol, x0, ty0 + 2f * rowH, rx, _cfLineH);
		// the pill at the right of the first line (the gauge takes the height; the words take three lines)
		float ph = rowH - 3f * k;
		float pw = Math.Min(TextW(g, d.Verdict, _cfb) + 14f * k, Math.Max(0f, rx - x0));
		float whoW = TextW(g, who, _cfb) + 8f * k;
		if (pw >= 16f * k && x0 + whoW + pw <= rx + 2f * k)
		{
			float py0 = ty0 - 1f * k;
			x0 = x0 + whoW;
			g.FillPath(Brush(Color.FromArgb(46, d.VerdictCol)), Rounded(_gdi.Path(18), x0 - 4f * k, py0, pw, ph, ph / 2f));
			CellText(g, d.Verdict, _cfb, d.VerdictCol, x0 + 3f * k, py0 + (ph - _cfLineH) / 2f, x0 + pw - 6f * k, _cfLineH);
		}
	}

	// 2.1: the classic verdict in the card's short words (the classic table and the signal card keep the long ones)
	private static string CardWords(string verdict)
	{
		switch (verdict)
		{
		case "no order-flow data for this bar":
			return "no data yet";
		case "rises with no push – thin, fragile":
			return "rises, no push · fragile";
		case "falls with no push – thin, fragile":
			return "falls, no push · fragile";
		case "quiet – wait":
			return "quiet · wait";
		case "buyers push, price falls – BUYERS TRAPPED":
			return "BUYERS TRAPPED";
		case "sellers push, price rises – SELLERS TRAPPED":
			return "SELLERS TRAPPED";
		case "BUYERS being ABSORBED":
			return "BUYERS ABSORBED";
		case "SELLERS being ABSORBED":
			return "SELLERS ABSORBED";
		case "rises on thin buying – fragile":
			return "thin buying · fragile";
		case "falls on thin selling – fragile":
			return "thin selling · fragile";
		default:
			return verdict;
		}
	}

	// 2.1: the last bar's reading in the card's short words
	private static string ShortReading(string s)
	{
		if (s == null)
		{
			return "";
		}
		if (s.StartsWith("rose a full range", StringComparison.Ordinal))
		{
			return "▲ thin book (little push)";
		}
		if (s.StartsWith("fell a full range", StringComparison.Ordinal))
		{
			return "▼ thin book (little push)";
		}
		if (s.EndsWith("BUYERS TRAPPED", StringComparison.Ordinal))
		{
			return "BUYERS TRAPPED";
		}
		if (s.EndsWith("SELLERS TRAPPED", StringComparison.Ordinal))
		{
			return "SELLERS TRAPPED";
		}
		if (s.StartsWith("heavy ", StringComparison.Ordinal))
		{
			int p = s.IndexOf(" (", StringComparison.Ordinal);
			int x = s.IndexOf('×');
			return (p > 0 && x > p) ? ("heavy " + (s.StartsWith("heavy buying", StringComparison.Ordinal) ? "buying " : "selling ") + s.Substring(p + 2, x - p - 1)) : s;
		}
		return CardWords(s);
	}

	// 2.1: the last bar's speed in the card's short words (without "than typical (...)")
	private static string ShortSpeed(string s)
	{
		if (s == null)
		{
			return "";
		}
		int p = s.IndexOf(" than typical", StringComparison.Ordinal);
		if (p > 0)
		{
			return s.Substring(0, p);
		}
		p = s.IndexOf(" (", StringComparison.Ordinal);
		return (p > 0) ? s.Substring(0, p) : s;
	}

	private readonly struct CardRow
	{
		public readonly byte Kind;   // 0 text, 1 meter, 2 split bar, 3 chip, 4 gap, 5 live strip, 6 the CVD number

		public readonly string Label;

		public readonly string Text;

		public readonly Color Col;

		public readonly double V;

		public readonly double Full;

		public CardRow(byte kind, string label = null, string text = null, Color col = default(Color), double v = 0.0, double full = 1.0)
		{
			Kind = kind;
			Label = label;
			Text = text;
			Col = col;
			V = v;
			Full = full;
		}
	}

	private readonly List<CardRow> _cardRows = new List<CardRow>(24);

	private float _cfLineH = 12f;

	private float _cfhLineH = 18f;

	// 2.1: a text inside its cell: never past the cell's right edge, a longer text ends in "..."
	private static readonly StringFormat Cell = new StringFormat(StringFormatFlags.NoWrap)
	{
		Trimming = StringTrimming.EllipsisCharacter
	};

	private void CellText(Graphics g, string s, Font font, Color col, float x, float y, float right, float h)
	{
		if (string.IsNullOrEmpty(s) || right - x < 4f)
		{
			return;
		}
		g.DrawString(s, font, Brush(col), new RectangleF(x, y, right - x, h + 2f), Cell);
	}

	private void DrawCard(Graphics g, float x, float y, float width, float height, float k, float pad, float rowH, float sepH, float meterW, float labelW, float colW, int cols, int[] cs, List<CardRow> rows, string badge, string tag, string cvd, string barChip, double barDelta, bool buyersC, Color dim, Color soft)
	{
		GraphicsState state = g.Save();
		try
		{
			g.SmoothingMode = SmoothingMode.AntiAlias;
			int alpha = Math.Max(0, Math.Min(255, CardOpacity * 255 / 100));
			GraphicsPath bg = Rounded(_gdi.Path(1), x, y, width, height, 8f * k);
			g.FillPath(Brush(Color.FromArgb(alpha, 4, 5, 8)), bg);
			g.DrawPath(Pen(Color.FromArgb(255, 36, 41, 50), 1f), bg);
			g.SetClip(new RectangleF(x, y, width, height), CombineMode.Intersect);
			g.FillRectangle(Brush(Color.FromArgb(230, buyersC ? BullColor : BearColor)), x + 1.5f, y + 9f * k, 2.5f * k, Math.Max(1f, height - 18f * k));
			float th = _cfLineH;
			float hx = x + pad;
			float hr = x + width - pad;
			float cy = y + pad - 2f * k;
			// title: CVD PRO · chart type · LIVE
			Color bc = ((badge == "LIVE") ? BullColor : ExhaustColor);
			float bw = TextW(g, badge, _cfs) + 18f * k;
			float bx = hr - bw;
			g.FillPath(Brush(Color.FromArgb(46, bc)), Rounded(_gdi.Path(11), bx, cy + 1f * k, bw, th - 2f * k, (th - 2f * k) / 2f));
			g.FillEllipse(Brush(bc), bx + 6f * k, cy + th / 2f - 2.5f * k, 5f * k, 5f * k);
			g.DrawString(badge, _cfs, Brush(bc), bx + 13f * k, cy + (th - _cfsLineH) / 2f, NoWrap);
			float limit = bx - 6f * k;
			long errs = Interlocked.Read(ref TnxErrors.Total);
			if (errs > 0)
			{
				// 2.1: errors CVD PRO caught inside (counted in memory, never written anywhere): shown, never hidden
				string et = "⚠ " + errs.ToString(CultureInfo.InvariantCulture) + " errors";
				float ew = TextW(g, et, _cfs) + 12f * k;
				float ex = limit - ew;
				if (ex > hx + 40f * k)
				{
					g.FillPath(Brush(Color.FromArgb(60, ExhaustColor)), Rounded(_gdi.Path(17), ex, cy + 1f * k, ew, th - 2f * k, 4f * k));
					g.DrawString(et, _cfs, Brush(ExhaustColor), ex + 6f * k, cy + (th - _cfsLineH) / 2f, NoWrap);
					limit = ex - 6f * k;
				}
			}
			CellText(g, "CVD PRO", _cfb, TextColor, hx, cy, limit, th);
			float tagX = hx + TextW(g, "CVD PRO", _cfb) + 10f * k;
			float tw = TextW(g, tag, _cfs) + 12f * k;
			if (tag.Length > 0 && tagX + tw < limit)
			{
				g.FillPath(Brush(Color.FromArgb(255, 20, 24, 32)), Rounded(_gdi.Path(10), tagX, cy + 1f * k, tw, th - 2f * k, 4f * k));
				g.DrawString(tag, _cfs, Brush(dim), tagX + 6f * k, cy + (th - _cfsLineH) / 2f, NoWrap);
			}
			cy += rowH + 3f * k;
			Pen line = Pen(Color.FromArgb(255, 30, 34, 42), 1f);
			g.DrawLine(line, hx, cy - 3f * k, hr, cy - 3f * k);
			float headBottom = cy;
			for (int c = 0; c < cols; c++)
			{
				// the columns side by side under the title (a short window gets 2 to 4 of them)
				int from = cs[c];
				int to = ((c + 1 < cols) ? cs[c + 1] : rows.Count);
				float lx = x + pad + (float)c * (colW + 16f * k);
				float rx = Math.Min(x + width - pad, lx + colW);
				float vx = lx + labelW + 10f * k;
				cy = headBottom;
				if (c > 0)
				{
					g.DrawLine(line, lx - 8f * k, headBottom, lx - 8f * k, y + height - pad);
				}
				for (int ri = from; ri < to; ri++)
				{
					CardRow row = rows[ri];
					float ty = cy + (rowH - _cfLineH) / 2f;
					switch (row.Kind)
					{
					case 4:
						if (ri != from && ri != to - 1)
						{
							g.DrawLine(line, lx, cy + sepH / 2f, rx, cy + sepH / 2f);
							cy += sepH;
						}
						continue;
					case 6:
					{
						// the CVD number (its colour: above or below zero) and this bar's delta (its colour: bought or sold)
						Color cc = ((barDelta >= 0.0) ? BullColor : BearColor);
						float cw = Math.Min(TextW(g, barChip, _cfb) + 14f * k, Math.Max(0f, (rx - lx) * 0.45f));
						float chH = _cfLineH + 2f * k;
						float chY = cy + (_cfhLineH - chH) / 2f + 1.5f * k;
						if (cw >= 20f * k)
						{
							g.FillPath(Brush(Color.FromArgb(40, cc)), Rounded(_gdi.Path(12), rx - cw, chY, cw, chH, chH / 2f));
							CellText(g, barChip, _cfb, cc, rx - cw + 7f * k, chY + (chH - _cfLineH) / 2f, rx - 4f * k, _cfLineH);
						}
						CellText(g, cvd, _cfh, row.Col, lx - 1f * k, cy, rx - cw - 6f * k, _cfhLineH);
						cy += _cfhLineH + 3f * k;
						continue;
					}
					case 7:
					{
						DrawCompass(g, lx, rx, cy, rowH * CompassRows, rowH, k, dim, soft);   // 2.3
						cy += rowH * CompassRows;
						continue;
					}
					}
					CellText(g, row.Label, _cf, dim, lx, ty, vx - 6f * k, _cfLineH);
					switch (row.Kind)
					{
					case 0:
						CellText(g, row.Text, _cfb, row.Col, vx, ty, rx, _cfLineH);
						break;
					case 1:
					{
						// a two-sided meter around zero: the bar shows the reading, the thin marks the normal size (1x)
						float mw = Math.Min(meterW, Math.Max(0f, rx - vx) * 0.45f);
						float my = cy + rowH / 2f - 2.5f * k;
						float half = mw / 2f;
						float frac = (float)Math.Min(1.0, Math.Abs(row.V) / row.Full);
						g.FillRectangle(Brush(Color.FromArgb(255, 26, 30, 38)), vx, my, mw, 5f * k);
						float fw = half * frac;
						if (fw > 0f)
						{
							g.FillRectangle(Brush(row.Col), (row.V >= 0.0) ? (vx + half) : (vx + half - fw), my, fw, 5f * k);
						}
						g.DrawLine(Pen(Color.FromArgb(150, 210, 214, 224), 1f), vx + half, my - 2f * k, vx + half, my + 7f * k);
						float one = (float)(half / row.Full);
						Pen mark = Pen(Color.FromArgb(90, 210, 214, 224), 1f);
						g.DrawLine(mark, vx + half - one, my, vx + half - one, my + 5f * k);
						g.DrawLine(mark, vx + half + one, my, vx + half + one, my + 5f * k);
						CellText(g, row.Text, _cfb, row.Col, vx + mw + 8f * k, ty, rx, _cfLineH);
						break;
					}
					case 2:
					{
						// buyers against sellers: the green part is the buyers' share of the volume
						float mw = Math.Min(meterW, Math.Max(0f, rx - vx) * 0.45f);
						float my = cy + rowH / 2f - 3f * k;
						float bwid = mw * (float)Math.Max(0.0, Math.Min(1.0, row.V / 100.0));
						g.FillRectangle(Brush(Color.FromArgb(215, BearColor)), vx, my, mw, 6f * k);
						if (bwid > 0f)
						{
							g.FillRectangle(Brush(Color.FromArgb(215, BullColor)), vx, my, bwid, 6f * k);
						}
						g.DrawLine(Pen(Color.FromArgb(220, 8, 10, 14), 1.5f), vx + mw / 2f, my - 1f, vx + mw / 2f, my + 6f * k + 1f);
						int t = row.Text.IndexOf("  TURNING", StringComparison.Ordinal);
						string main = ((t >= 0) ? row.Text.Substring(0, t) : row.Text);
						float tx0 = vx + mw + 8f * k;
						CellText(g, main, _cfb, row.Col, tx0, ty, rx, _cfLineH);
						if (t >= 0)
						{
							float tx = tx0 + TextW(g, main, _cfb) + 6f * k;
							float tw2 = TextW(g, "TURNING", _cfs) + 12f * k;
							if (tx + tw2 <= rx)
							{
								g.FillPath(Brush(Color.FromArgb(60, ExhaustColor)), Rounded(_gdi.Path(13), tx, cy + 2f * k, tw2, rowH - 4f * k, 4f * k));
								g.DrawString("TURNING", _cfs, Brush(ExhaustColor), tx + 6f * k, cy + (rowH - _cfsLineH) / 2f, NoWrap);
							}
						}
						break;
					}
					case 3:
					{
						// the reading as a coloured tag
						float cw = Math.Min(TextW(g, row.Text, _cfb) + 14f * k, rx - vx + 6f * k);
						if (cw >= 12f)
						{
							g.FillPath(Brush(Color.FromArgb(38, row.Col)), Rounded(_gdi.Path(14), vx - 6f * k, cy + 1.5f * k, cw, rowH - 3f * k, 5f * k));
						}
						CellText(g, row.Text, _cfb, row.Col, vx + 1f * k, ty, rx - 2f * k, _cfLineH);
						break;
					}
					case 5:
					{
						// the bar that is forming: how far it is
						float w = Math.Min(meterW * 1.4f, Math.Max(0f, rx - vx) * 0.5f);
						float my = cy + rowH / 2f - 2.5f * k;
						if (w >= 8f)
						{
							g.FillPath(Brush(Color.FromArgb(255, 26, 30, 38)), Rounded(_gdi.Path(15), vx, my, w, 5f * k, 2.5f * k));
							float fw = (float)((double)w * Math.Max(0.0, Math.Min(1.0, row.V)));
							if (fw >= 5f * k)
							{
								g.FillPath(Brush(row.Col), Rounded(_gdi.Path(16), vx, my, fw, 5f * k, 2.5f * k));
							}
						}
						CellText(g, row.Text, _cfs, soft, vx + w + 8f * k, cy + (rowH - _cfsLineH) / 2f, rx, _cfsLineH);
						break;
					}
					}
					cy += rowH;
				}
			}
		}
		finally
		{
			g.Restore(state);
		}
	}

	private float _cfsLineH = 11f;

	// 2.1: the flow in plain words: who, how strong against the last 60 bars (rank), and the rank
	private string FlowWords(Bar[] b, int abs, double sc, bool slow, out Color col)
	{
		double v = (slow ? b[abs].Slow : b[abs].Fast);
		string who = ((v >= 0.0) ? "Buying" : "Selling");
		double rank = ((FlowTiers == FlowTierMode.Percentile) ? (slow ? b[abs].SRank : b[abs].FRank) : (-1.0));   // 2.3: stored
		Color side = ((v >= 0.0) ? BullColor : BearColor);
		if (rank < 0.0)
		{
			col = side;
			return who + " · " + FlowLabel(v, sc);
		}
		int tier = ((rank < 0.5) ? 0 : ((rank < 0.8) ? 1 : ((rank < 0.95) ? 2 : 3)));
		col = ((tier == 0) ? Color.FromArgb(255, 128, 136, 152) : ((tier == 3) ? Bright(side, 0.25) : side));
		return who + " · " + ((tier == 0) ? "quiet" : ((tier == 1) ? "building" : ((tier == 2) ? "strong" : "EXTREME"))) + " · " + (rank * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%";
	}

	// 2.1: the acceleration in plain words (the same levels as AccelText), always in the side's colour
	private string AccelWords(bool buyers, double acc, double norm, bool thrustOk, out Color col)
	{
		bool with = acc >= 0.0 == buyers;
		double n = Math.Abs(norm);
		string who = (buyers ? "Buyers " : "Sellers ");
		string x = "  " + n.ToString("0.0", CultureInfo.InvariantCulture) + "×";
		Color side = (buyers ? BullColor : BearColor);
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
		if (n >= AccelExtreme && GateThrust && !thrustOk)
		{
			col = Color.FromArgb(200, side);
			return who + "thin thrust" + x;   // 2.3: speeding up with no busy, one-sided market behind it
		}
		col = side;
		return who + ((n >= AccelExtreme) ? "THRUST" : "speeding up") + x;
	}

	private (int, long, long, double, double, Theme, int, int) _lastBarKey;   // 2.0: the last bar's texts, made once per bar

	private string _lastBarSpeed;

	private string _lastBarReading;

	private Color _lastBarSpeedCol;

	private Color _lastBarReadingCol;

	private string _chartTag;   // 2.0: made when the chart type is read (DetectGeometry)

	private string _pChartTag;   // 2.0: the picture's copy

	private readonly Dictionary<(string, int), float> _textW = new Dictionary<(string, int), float>();   // 2.0: widths of the table's texts

	private float _textWDpi;

	// 2.0: a text's width, measured once per text, font and screen resolution (Graphics.MeasureString is the table's
	// biggest cost and most texts repeat from one picture to the next); the same number MeasureString gives. The cache is
	// emptied whenever the fonts are made again (Fonts) or the resolution changes.
	private float TextW(Graphics g, string text, Font font)
	{
		int fi = ReferenceEquals(font, _f) ? 0 : (ReferenceEquals(font, _fb) ? 1 : (ReferenceEquals(font, _fs) ? 2 :
			(ReferenceEquals(font, _cf) ? 3 : (ReferenceEquals(font, _cfb) ? 4 : (ReferenceEquals(font, _cfs) ? 5 : (ReferenceEquals(font, _cfh) ? 6 :
			(ReferenceEquals(font, _rf) ? 7 : (ReferenceEquals(font, _rfb) ? 8 : (ReferenceEquals(font, _rfs) ? 9 : (ReferenceEquals(font, _rfh) ? 10 : -1))))))))));
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

	// the chart type for the card's title: "range 8t", "500 tick", "1 min"
	private string ChartTag()
	{
		double tick = base.Symbol?.TickSize ?? 0.0;
		switch (_agg)
		{
		case Agg.Range:
			return (tick > 0.0 && _barHeight > 0.0) ? ("range " + Math.Round(_barHeight / tick).ToString("0", CultureInfo.InvariantCulture) + "t") : "range bars";
		case Agg.Tick:
			return _aggN.ToString("0", CultureInfo.InvariantCulture) + " tick";
		case Agg.Time:
			return (_aggN >= 6E+08) ? ((_aggN / 6E+08).ToString("0.##", CultureInfo.InvariantCulture) + " min") : ((_aggN / 1E+07).ToString("0.##", CultureInfo.InvariantCulture) + " s");
		default:
			return "";
		}
	}

	// 2.0 (plan A1): how long a finished bar took against the typical bar of the lookback (median), with its direction
	private string LastBarSpeed(int i, out Color col)
	{
		ref Bar b = ref _pb[i];
		col = (b.Px > b.PO) ? BullColor : ((b.Px < b.PO) ? BearColor : Color.FromArgb(150, 158, 174));
		string dir = (b.Px > b.PO) ? "▲ " : ((b.Px < b.PO) ? "▼ " : "• ");
		double dur = (double)(_pb[i + 1].Ticks - b.Ticks) / 1E+07;
		int n = Math.Min(Math.Max(5, LookbackBars), i + 1);
		double[] d = _arrays.Get<double>(5, n);
		int m = 0;
		for (int k = i - n + 1; k <= i; k++)
		{
			if (k >= 0 && k + 1 < _pn)
			{
				d[m++] = (double)(_pb[k + 1].Ticks - _pb[k].Ticks) / 1E+07;
			}
		}
		Array.Sort(d, 0, m);
		double typical = (m == 0) ? 0.0 : ((m % 2 == 1) ? d[m / 2] : (0.5 * (d[m / 2 - 1] + d[m / 2])));
		string took = dir + Secs(dur);
		if (dur <= 0.0 || typical <= 0.0 || m < 5)
		{
			return took;
		}
		double ratio = typical / dur;
		if (ratio >= 1.25)
		{
			return took + " · " + ratio.ToString("0.0", CultureInfo.InvariantCulture) + "× faster than typical (" + Secs(typical) + ")";
		}
		if (ratio <= 0.8)
		{
			return took + " · " + (1.0 / ratio).ToString("0.0", CultureInfo.InvariantCulture) + "× slower than typical (" + Secs(typical) + ")";
		}
		return took + " · normal pace (" + Secs(typical) + ")";
	}

	private static string Secs(double s)
	{
		if (s >= 120.0)
		{
			return (s / 60.0).ToString("0.0", CultureInfo.InvariantCulture) + " min";
		}
		return s.ToString((s < 10.0) ? "0.0" : "0", CultureInfo.InvariantCulture) + " s";
	}

	// 2.0 (plan F1): the reading of a finished bar. Range bars: every finished bar moved one range, so the result is fixed
	// and the reading is the effort it took (with the bar, against it, or very little). Tick bars: the classic verdict.
	private string LastBarReading(int i, out Color col)
	{
		ref Bar b = ref _pb[i];
		BarRead(_pb, _pBarHeight, i, out var e, out var r);
		if (_agg != Agg.Range)
		{
			Verdict(b.Real, e, r, out var text, out col, out var _);
			return text;
		}
		col = Color.FromArgb(150, 158, 174);
		if (!b.Real)
		{
			return "no order-flow data for this bar";
		}
		int dir = (b.Px > b.PO) ? 1 : ((b.Px < b.PO) ? (-1) : 0);
		double ae = Math.Abs(e);
		int se = ((e >= 0.0) ? 1 : (-1));
		if (dir == 0)
		{
			Verdict(b.Real, e, r, out var text2, out col, out var _);
			return text2;
		}
		if (ae < 0.7)
		{
			col = ExhaustColor;
			return (dir > 0) ? "rose a full range on little push – thin book" : "fell a full range on little push – thin book";
		}
		if (se != dir)
		{
			col = ((se > 0) ? BearColor : BullColor);
			return (se > 0) ? "buyers pushed, the bar fell – BUYERS TRAPPED" : "sellers pushed, the bar rose – SELLERS TRAPPED";
		}
		col = ((se > 0) ? BullColor : BearColor);
		if (ae >= 2.0)
		{
			return ((se > 0) ? "heavy buying broke through (" : "heavy selling broke through (") + ae.ToString("0.0", CultureInfo.InvariantCulture) + "× normal effort)";
		}
		return (se > 0) ? "healthy buying" : "healthy selling";
	}

	private float CvdY(double v)
	{
		return _cvMid + _cvOff - (float)((v - _cvCenter) / _cvHalf * (double)_cvHalfPx);
	}

	private double CvdV(float y)
	{
		return _cvCenter + (double)((_cvMid + _cvOff - y) / _cvHalfPx) * _cvHalf;
	}

	private float CY(IChartWindowCoordinatesConverter conv, double v)
	{
		if (Mode != Display.Velocity || !_cvOk)
		{
			return (float)conv.GetChartY(v);
		}
		return CvdY(v);
	}

	private bool InPlatformUi(ChartMouseNativeEventArgs e)
	{
		if (e.Y <= _winRect.Top + 28 && e.X <= _winRect.Left + Math.Max(120, TitleAreaWidth))
		{
			return true;
		}
		if (_tableRect.Width > 0f && _tableRect.Contains(e.X, e.Y))
		{
			return true;
		}
		return false;
	}

	private int SplitterAt(ChartMouseNativeEventArgs e)
	{
		if (!LaneMouse || Mode != Display.Velocity)
		{
			return 0;
		}
		try
		{
			if (e.Window == null || e.Window.WindowNumber != _winNum)
			{
				return 0;
			}
		}
		catch
		{
			return 0;
		}
		if (!_winRect.Contains(e.X, e.Y) || InPlatformUi(e))
		{
			return 0;
		}
		if (Math.Abs((float)e.Y - (_spTop - 3f)) <= 5f)
		{
			return 1;
		}
		if (ShowAccel && Math.Abs((float)e.Y - (_acTop - 3f)) <= 5f)
		{
			return 2;
		}
		return 0;
	}

	private void HookMouse()
	{
		IChart currentChart = base.CurrentChart;
		if (currentChart == null || currentChart == _mouseChart)
		{
			return;
		}
		UnhookMouse();
		try
		{
			currentChart.MouseWheel += OnWheel;
			currentChart.MouseDown += OnDown;
			currentChart.MouseMove += OnMove;
			currentChart.MouseUp += OnUp;
			_mouseChart = currentChart;
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "HookMouse#1", tnxError);   // P4 F5: was empty
		}
	}

	private void UnhookMouse()
	{
		IChart mouseChart = _mouseChart;
		if (mouseChart != null)
		{
			try
			{
				mouseChart.MouseWheel -= OnWheel;
				mouseChart.MouseDown -= OnDown;
				mouseChart.MouseMove -= OnMove;
				mouseChart.MouseUp -= OnUp;
			}
			catch (Exception tnxError)
			{
				TnxErrors.Report("TNX CVD PRO", "UnhookMouse#1", tnxError);   // P4 F5: was empty
			}
			_mouseChart = null;
		}
	}

	private int LaneAt(ChartMouseNativeEventArgs e)
	{
		if (!LaneMouse || Mode != Display.Velocity)
		{
			return 0;
		}
		try
		{
			if (e.Window == null || e.Window.WindowNumber != _winNum)
			{
				return 0;
			}
		}
		catch
		{
			return 0;
		}
		if (InPlatformUi(e))
		{
			return 0;
		}
		if ((float)e.X < _axisX)
		{
			return 0;
		}
		return BodyLaneAt(e);
	}

	private int BodyLaneAt(ChartMouseNativeEventArgs e)
	{
		if (!LaneMouse || Mode != Display.Velocity)
		{
			return 0;
		}
		try
		{
			if (e.Window == null || e.Window.WindowNumber != _winNum)
			{
				return 0;
			}
		}
		catch
		{
			return 0;
		}
		if (InPlatformUi(e))
		{
			return 0;
		}
		if (_spRect.Contains(e.X, e.Y))
		{
			return 1;
		}
		if (ShowAccel && _acRect.Contains(e.X, e.Y))
		{
			return 2;
		}
		if (_cvOk && _cvRect.Contains(e.X, e.Y))
		{
			return 3;
		}
		return 0;
	}

	private void OnWheel(object s, ChartMouseNativeEventArgs e)
	{
		try
		{
			int num = LaneAt(e);
			if (num == 0)
			{
				return;
			}
			double num2 = ((e.Delta > 0) ? 0.8695652173913044 : 1.15);
			switch (num)
			{
			case 1:
				_spZoom = Math.Max(0.05, Math.Min(50.0, _spZoom * num2));
				break;
			case 2:
				_acZoom = Math.Max(0.05, Math.Min(50.0, _acZoom * num2));
				break;
			default:
				// 2.2: the CVD pane's panel: from AUTO too (it goes MANUAL from the view on the screen), around the
				// newest candle (1.x: only in MANUAL, around the middle of the visible range)
				CvdToManual();
				CvdStretch(num2, _cvCenter, _cvHalf);
				_cvFollow = CvdNewestInView();
				break;
			}
			e.Handled = true;
			e.NeedRedraw = true;
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnWheel#1", tnxError);   // P4 F5: was empty
		}
	}

	private void OnDown(object s, ChartMouseNativeEventArgs e)
	{
		try
		{
			// 2.2: a line between panes first: the badge sat on the line above the flow lane and was tested first, so a grab
			// there switched AUTO -> MANUAL and the candles jumped
			int num = SplitterAt(e);
			// 2.0: the AUTO badge exists only in Velocity mode with the lane mouse on (the mouse is now also hooked for the signal
			// cards in every mode; the badge's last box must not catch a click in another mode)
			if (num == 0 && e.Button == NativeMouseButtons.Left && Mode == Display.Velocity && LaneMouse && _cvAutoBadgeRect.Width > 0f && _cvAutoBadgeRect.Contains(e.X, e.Y))
			{
				_cvAuto = !_cvAuto;
				_cvOff = 0f;
				_cvZoom = 1.0;
				_cvMSet = false;   // 2.2: MANUAL starts from the view on the screen (no jump)
				_cvFollow = true;
				e.Handled = true;
				e.NeedRedraw = true;
				return;
			}
			if (num != 0)
			{
				if (e.Button == NativeMouseButtons.Right || (e.Clicks >= 2 && LaneStyle == LaneLook.Pro))
				{
					_spHpx = (_acHpx = 0f);
					if (LaneStyle == LaneLook.Pro)
					{
						// 2.2: right-click / double-click on a line: the default shares
						_spFrac = (_acFrac = 0f);
						VelocityPanelHeight = 34;
						AccelHeight = 18;
					}
					e.Handled = true;
					e.NeedRedraw = true;
				}
				else if (e.Button == NativeMouseButtons.Left)
				{
					_drag = true;
					_dragLane = num + 10;
					_dragY = e.Y;
					_dragH0 = _spBot - _spTop;
					_dragH1 = _acBot - _acTop;
					e.Handled = true;
					e.NeedMouseCapture = true;
				}
				return;
			}
			int num2 = LaneAt(e);
			if (num2 == 0)
			{
				int num3 = BodyLaneAt(e);
				// 2.1: a drag inside the flow or the acceleration lane no longer moves its bars (the owner: controlling a lane
				// must not move them). In 1.x it moved the lane's zero line, also during every chart scroll over a lane, and a
				// zero line at the lane's edge hides one side's bars. Their scale (the right side) still zooms them; the CVD
				// area in MANUAL still moves with a drag, as before; the chart itself scrolls as always.
				if (num3 == 3 && e.Button == NativeMouseButtons.Left && !_cvAuto)
				{
					_soft = true;
					_softLane = num3;
					_dragY = e.Y;
					_dragCvC0 = _cvCenter;   // 2.2
					_dragCvH0 = _cvHalf;
					_dragOff0 = num3 switch
					{
						2 => Lim(_acOff, _acRoom), 
						1 => Lim(_spOff, _spRoom), 
						_ => _cvOff, 
					};
				}
			}
			else if (e.Button == NativeMouseButtons.Right || e.Clicks >= 2)
			{
				switch (num2)
				{
				case 1:
					_spZoom = 1.0;
					_spOff = 0f;
					break;
				case 2:
					_acZoom = 1.0;
					_acOff = 0f;
					break;
				default:
					_cvZoom = 1.0;
					_cvOff = 0f;
					_cvAuto = true;
					_cvMSet = false;
					break;
				}
				e.Handled = true;
				e.NeedRedraw = true;
			}
			else if (e.Button == NativeMouseButtons.Left)
			{
				// 2.2: the CVD panel is taken in AUTO too (a stretch there starts MANUAL from the view on the screen)
				_drag = true;
				_dragLane = num2;
				_dragY = e.Y;
				_dragCvC0 = _cvCenter;
				_dragCvH0 = _cvHalf;
				_dragAxis = (float)e.X >= _axisX;
				_dragOff0 = num2 switch
				{
					2 => Lim(_acOff, _acRoom), 
					1 => Lim(_spOff, _spRoom), 
					_ => _cvOff, 
				};
				_dragZoom0 = num2 switch
				{
					2 => _acZoom, 
					1 => _spZoom, 
					_ => _cvZoom, 
				};
				_dragH = Math.Max(20f, num2 switch
				{
					2 => _acRect.Height, 
					1 => _spRect.Height, 
					_ => _cvRect.Height, 
				});
				e.Handled = true;
				e.NeedMouseCapture = true;
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnDown#1", tnxError);   // P4 F5: was empty
		}
	}

	private void OnMove(object s, ChartMouseNativeEventArgs e)
	{
		try
		{
			if (_soft)
			{
				float num = _dragOff0 + (float)(e.Y - _dragY);
				if (_softLane == 1)
				{
					_spOff = Lim(num, _spRoom);
				}
				else if (_softLane == 2)
				{
					_acOff = Lim(num, _acRoom);
				}
				else if (!_cvAuto)
				{
					// 2.2: the view moves with the mouse; the picture keeps the newest candle in view
					_cvMCenter = _dragCvC0 + (double)((float)(e.Y - _dragY) / Math.Max(1f, _cvHalfPx)) * _dragCvH0;
					_cvMSet = true;
				}
				e.NeedRedraw = true;
			}
			else
			{
				if (!_drag)
				{
					if (HoverCards)
					{
						HoverAt(e);
					}
					return;
				}
				float num2 = e.Y - _dragY;
				if (_dragLane == 11)
				{
					if (LaneStyle == LaneLook.Pro)
					{
						// 2.2: the line above the flow pane: only the CVD and flow panes change, within their minimums
						float hh = Math.Max(1f, (float)_winRect.Height);
						float gaps = 6f * (float)(ShowAccel ? 2 : 1) + 3f;
						float acc = (ShowAccel ? _dragH1 : 0f);
						float maxF = Math.Max(_pMinF, hh - gaps - _pMinC - acc);
						_spFrac = Math.Max(_pMinF, Math.Min(maxF, _dragH0 - num2)) / hh;
						if (ShowAccel)
						{
							_acFrac = acc / hh;   // the acceleration pane keeps its height
						}
					}
					else
					{
						_spHpx = Math.Max(24f, Math.Min((float)_winRect.Height * 0.7f, _dragH0 - num2));
					}
					e.Handled = true;
					e.NeedRedraw = true;
					return;
				}
				if (_dragLane == 12)
				{
					if (LaneStyle == LaneLook.Pro)
					{
						// 2.2: the line above the acceleration pane: only the flow and acceleration panes change
						float hh = Math.Max(1f, (float)_winRect.Height);
						float tot = _dragH0 + _dragH1;
						float a = Math.Max(_pMinA, Math.Min(tot - _pMinF, _dragH1 - num2));
						_acFrac = a / hh;
						_spFrac = (tot - a) / hh;
					}
					else
					{
						float num3 = Math.Max(18f, Math.Min((float)_winRect.Height * 0.6f, _dragH1 - num2));
						float num4 = num3 - _dragH1;
						_acHpx = num3;
						_spHpx = Math.Max(24f, _dragH0 - num4);
					}
					e.Handled = true;
					e.NeedRedraw = true;
					return;
				}
				if (_dragAxis)
				{
					double val = _dragZoom0 * Math.Exp((double)(num2 / _dragH) * 1.6);
					val = Math.Max(0.05, Math.Min(50.0, val));
					if (_dragLane == 1)
					{
						_spZoom = val;
					}
					else if (_dragLane == 2)
					{
						_acZoom = val;
					}
					else if (_cvAuto && Math.Abs(num2) < 2f)
					{
						// 2.2: a click on the CVD panel without a move changes nothing
					}
					else
					{
						// 2.2: stretch (up) / squeeze (down) around the newest candle, from AUTO too
						CvdToManual();
						CvdStretch(Math.Exp((double)(num2 / _dragH) * 1.6), _dragCvC0, _dragCvH0);
					}
				}
				else
				{
					float num5 = _dragOff0 + num2;
					if (_dragLane == 1)
					{
						_spOff = Lim(num5, _spRoom);
					}
					else if (_dragLane == 2)
					{
						_acOff = Lim(num5, _acRoom);
					}
					else if (!_cvAuto)
					{
						_cvOff = num5;
					}
				}
				e.Handled = true;
				e.NeedRedraw = true;
			}
		}
		catch (Exception tnxError)
		{
			TnxErrors.Report("TNX CVD PRO", "OnMove#1", tnxError);   // P4 F5: was empty
		}
	}

	// 2.0 (A4): which marker the mouse points at (the boxes are those of the last picture)
	private void HoverAt(ChartMouseNativeEventArgs e)
	{
		try
		{
			if (e.Window == null || e.Window.WindowNumber != _winNum)
			{
				if (_hover != null)
				{
					_hover = null;
					e.NeedRedraw = true;
				}
				return;
			}
		}
		catch
		{
			return;
		}
		Sig hit = null;
		// 2.1: never wait for the picture. The mouse comes on the chart window's thread; waiting there for a picture makes
		// the whole chart wait (lag, a frozen chart). While a picture is being painted this move is skipped; the next move,
		// a few ms later, does it.
		if (!Monitor.TryEnter(_paintSync))
		{
			return;
		}
		try
		{
			for (int i = 0; i < _hitList.Count; i++)
			{
				if (_hitList[i].Box.Contains(e.X, e.Y))
				{
					hit = _hitList[i].Sig;
					break;
				}
			}
		}
		finally
		{
			Monitor.Exit(_paintSync);
		}
		Sig old = _hover;
		bool same = (hit == null && old == null) || (hit != null && old != null && hit.Abs == old.Abs && hit.Kind == old.Kind && hit.Bull == old.Bull && hit.P1 == old.P1 && hit.P2 == old.P2);
		if (!same)
		{
			_hover = hit;
			e.NeedRedraw = true;
		}
	}

	private void OnUp(object s, ChartMouseNativeEventArgs e)
	{
		bool cvMoved = (_soft && _softLane == 3) || (_drag && _dragLane == 3);
		_soft = false;
		if (cvMoved && !_cvAuto)
		{
			_cvFollow = CvdNewestInView();   // 2.2: left in view -> it is kept in view while trading; moved away -> it stays
		}
		if (_drag)
		{
			_drag = false;
			if ((_dragLane == 11 || _dragLane == 12) && LaneStyle == LaneLook.Pro)
			{
				// 2.2: a dragged line is remembered: the shares go into the two settings (saved with the workspace)
				if (_spFrac > 0f)
				{
					VelocityPanelHeight = Math.Max(10, Math.Min(80, (int)Math.Round(_spFrac * 100f)));
				}
				if (_acFrac > 0f)
				{
					AccelHeight = Math.Max(8, Math.Min(50, (int)Math.Round(_acFrac * 100f)));
				}
			}
			try
			{
				e.Handled = true;
			}
			catch (Exception tnxError)
			{
				TnxErrors.Report("TNX CVD PRO", "OnUp#1", tnxError);   // P4 F5: was empty
			}
		}
	}

	private void Fonts()
	{
		float num = (float)Math.Max(6.0, FontSize);
		if (_f == null)
		{
			_textW.Clear();   // 2.0: the text widths belong to the old fonts
			_f = new Font("Segoe UI", num);
			_fb = new Font("Segoe UI", num, FontStyle.Bold);
			_fs = new Font("Segoe UI", Math.Max(6f, num - 1.5f));
			_capH = _fs.GetHeight() + 1f;
			_axisW = _fs.Size * 5.6f + 8f;
		}
		if (_cf == null)
		{
			// 2.1: the card's own fonts, at the size the fit chose (first: "2.1 Card size")
			if (_cfScale <= 0f)
			{
				_cfScale = Math.Max(0.55f, Math.Min(1.6f, CardSize / 100f));
			}
			float k = _cfScale;
			_textW.Clear();
			_cf = new Font("Segoe UI", num * k);
			_cfb = new Font("Segoe UI Semibold", num * k);
			_cfs = new Font("Segoe UI", Math.Max(6f, (num - 1.5f) * k));
			_cfh = new Font("Segoe UI", (num + 3.5f) * k, FontStyle.Bold);
			_cfLineH = _cf.GetHeight();
			_cfsLineH = _cfs.GetHeight();
			_cfhLineH = _cfh.GetHeight();
		}
		if (_rf == null)
		{
			// 2.1: the fit measures with these (100 %), so the size it chooses depends only on the window and the data
			_rf = new Font("Segoe UI", num);
			_rfb = new Font("Segoe UI Semibold", num);
			_rfs = new Font("Segoe UI", Math.Max(6f, num - 1.5f));
			_rfh = new Font("Segoe UI", num + 3.5f, FontStyle.Bold);
			_rfLineH = _rf.GetHeight();
			_rfhLineH = _rfh.GetHeight();
		}
	}

	private Font _cf;   // 2.1: card text

	private Font _cfb;   // 2.1: card values

	private Font _cfs;   // 2.1: card small text

	private Font _cfh;   // 2.1: the card's big CVD number

	private Font _rf;

	private Font _rfb;

	private Font _rfs;

	private Font _rfh;

	private float _rfLineH = 12f;

	private float _rfhLineH = 18f;

	private void DisposeRefFonts()
	{
		_rf?.Dispose();
		_rfb?.Dispose();
		_rfs?.Dispose();
		_rfh?.Dispose();
		_rf = (_rfb = (_rfs = (_rfh = null)));
	}

	private void DisposeCardFonts()
	{
		_cf?.Dispose();
		_cfb?.Dispose();
		_cfs?.Dispose();
		_cfh?.Dispose();
		_cf = (_cfb = (_cfs = (_cfh = null)));
	}

	// 2.0 (F4): the key was colour ^ ((int)(width * 10) << 1), which two different pens could share (one colour at widths
	// 1.05 and 1.09; a colour and the same colour with blue ^ 4 at widths 1.0 and 0.8): the second got the first's pen
	private static long PenKey(Color c, float w) => ((long)c.ToArgb() << 32) | (uint)BitConverter.SingleToInt32Bits(w);

	private Pen Pen(Color c, float w)
	{
		long key = PenKey(c, w);
		if (!_pens.TryGetValue(key, out var value))
		{
			value = new Pen(c, w);
			_pens[key] = value;
		}
		return value;
	}

	// 2.3: a dashed pen of its own. The markers and the swing line used to set DashStyle on a shared pen and set it back;
	// an exception in between (caught by the picture) left that pen dashed for every later use.
	private Pen DashPen(Color c, float w)
	{
		long key = PenKey(c, w);
		if (!_dashPens.TryGetValue(key, out var value))
		{
			value = new Pen(c, w);
			value.DashStyle = DashStyle.Dash;
			_dashPens[key] = value;
		}
		return value;
	}

	private readonly Dictionary<long, Pen> _dashPens = new Dictionary<long, Pen>();

	// 2.0: a dotted pen of its own (the shared pens must not have their dash style changed)
	private Pen DotPen(Color c, float w)
	{
		long key = PenKey(c, w);
		if (!_dotPens.TryGetValue(key, out var value))
		{
			value = new Pen(c, w);
			value.DashStyle = DashStyle.Dot;
			_dotPens[key] = value;
		}
		return value;
	}

	private SolidBrush Brush(Color c)
	{
		int key = c.ToArgb();
		if (!_brushes.TryGetValue(key, out var value))
		{
			value = new SolidBrush(c);
			_brushes[key] = value;
		}
		return value;
	}

	private static GraphicsPath Rounded(GraphicsPath graphicsPath, float x, float y, float w, float h, float r)
	{
		w = Math.Max(1f, w);   // 2.1: never a zero or negative size (GDI+ throws on those and the picture stops)
		h = Math.Max(1f, h);
		r = Math.Max(0.5f, Math.Min(r, Math.Min(w, h) / 2f));
		graphicsPath.AddArc(x, y, 2f * r, 2f * r, 180f, 90f);
		graphicsPath.AddArc(x + w - 2f * r, y, 2f * r, 2f * r, 270f, 90f);
		graphicsPath.AddArc(x + w - 2f * r, y + h - 2f * r, 2f * r, 2f * r, 0f, 90f);
		graphicsPath.AddArc(x, y + h - 2f * r, 2f * r, 2f * r, 90f, 90f);
		graphicsPath.CloseFigure();
		return graphicsPath;
	}

	private static string Fmt(double v)
	{
		double num = Math.Abs(v);
		if (num >= 1000000.0)
		{
			return (v / 1000000.0).ToString("0.##", CultureInfo.InvariantCulture) + "M";
		}
		if (num >= 10000.0)
		{
			return (v / 1000.0).ToString("0.#", CultureInfo.InvariantCulture) + "K";
		}
		return v.ToString("0", CultureInfo.InvariantCulture);
	}

	private static string FmtS(double v)
	{
		return ((v >= 0.0) ? "+" : "-") + Fmt(Math.Abs(v));
	}

	private static string FmtS(double v, string f)
	{
		return ((v >= 0.0) ? "+" : "-") + Math.Abs(v).ToString(f, CultureInfo.InvariantCulture);
	}

	// 2.0 (plan A2): where |fast| (or |slow|) of bar abs ranks among the LookbackBars bars before it, 0..1 (ties count
	// half); -1 while fewer than 20 bars are there
	private double FlowRank(Bar[] b, int abs, bool slow)
	{
		int n = Math.Min(Math.Max(5, LookbackBars), abs);
		if (n < 20)
		{
			return -1.0;
		}
		double v = Math.Abs(slow ? b[abs].Slow : b[abs].Fast);
		int below = 0;
		int equal = 0;
		for (int i = abs - n; i < abs; i++)
		{
			double w = Math.Abs(slow ? b[i].Slow : b[i].Fast);
			if (w < v)
			{
				below++;
			}
			else if (w == v)
			{
				equal++;
			}
		}
		return ((double)below + 0.5 * (double)equal) / (double)n;
	}

	// 2.3: where a bar's volume per second (what = 0) or one-sidedness |Imb7| (what = 1) ranks among the LookbackBars bars
	// before it, 0..1 (ties count half); -1 while fewer than 20 bars are there. The same rule as FlowRank.
	private double RankOf(int abs, int what)
	{
		int n = Math.Min(Math.Max(5, LookbackBars), abs);
		if (n < 20)
		{
			return -1.0;
		}
		double v = ((what == 0) ? _b[abs].Vps : Math.Abs(_b[abs].Imb7));
		int below = 0;
		int equal = 0;
		for (int i = abs - n; i < abs; i++)
		{
			double w = ((what == 0) ? _b[i].Vps : Math.Abs(_b[i].Imb7));
			if (w < v)
			{
				below++;
			}
			else if (w == v)
			{
				equal++;
			}
		}
		return ((double)below + 0.5 * (double)equal) / (double)n;
	}

	// 2.3: the platform's UTC clock (server-synchronised), the plain clock if the platform has none yet
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

	// the flow-speed words: classic (FlowLabel), or by rank with the rank shown ("fast strong +1.2K · 91%")
	private string FlowText(Bar[] b, int abs, double sc)
	{
		if (FlowTiers == FlowTierMode.Percentile)
		{
			double rf = b[abs].FRank;   // 2.3: stored
			double rs = b[abs].SRank;
			if (rf >= 0.0 && rs >= 0.0)
			{
				// 2.0: the text of the last call is kept; a repaint with nothing changed makes no new string
				var key = (b[abs].Fast, b[abs].Slow, rf, rs);
				if (_flowTextKey == key && _flowText != null)
				{
					return _flowText;
				}
				_flowTextKey = key;
				_flowText = "fast " + RankWord(rf, b[abs].Fast) + "   slow " + RankWord(rs, b[abs].Slow);
				return _flowText;
			}
		}
		return "fast " + FlowLabel(b[abs].Fast, sc) + "   slow " + FlowLabel(b[abs].Slow, sc);
	}

	private (double, double, double, double) _flowTextKey;

	private string _flowText;

	private static string RankWord(double rank, double v)
	{
		string w = (rank < 0.5) ? "quiet" : ((rank < 0.8) ? "building" : ((rank < 0.95) ? "strong" : "EXTREME"));
		return w + " " + FmtS(v) + " · " + (rank * 100.0).ToString("0", CultureInfo.InvariantCulture) + "%";
	}

	private static string FlowLabel(double v, double sc)
	{
		double num = ((sc > 1E-09) ? (Math.Abs(v) / sc) : 0.0);
		return ((num >= 2.0) ? "STRONG" : ((num >= 1.0) ? "mod" : ((num >= 0.4) ? "weak" : "flat"))) + " " + FmtS(v);
	}
}
