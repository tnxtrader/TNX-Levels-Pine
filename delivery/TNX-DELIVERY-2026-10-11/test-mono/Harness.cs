// TEST ONLY. Runs the two TNX indicators on simulated range-bar trades (with footprint levels) under Mono + libgdiplus,
// renders them to PNG and dumps their per-bar numbers. Not part of the deliverable.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TradingPlatform.BusinessLayer;
using TradingPlatform.BusinessLayer.Chart;
using TNXIndicators;

public static class TnxShim
{
	public static long TickCount64 => Environment.TickCount;
}

namespace TnxTest
{
	class BarB
	{
		public int O, H, L, C;
		public double T0, T1;
		public double Buy, Sell, MaxD, MinD, D;
		public int Trades;
		public Dictionary<int, double[]> Lv = new Dictionary<int, double[]>();   // price ticks -> {buy, sell}
	}

	struct Phase
	{
		public double Sec, Rate, PBuy, PUp, PDn;
		public int? Ceil;
		public Phase(double sec, double rate, double pBuy, double pUp, double pDn, int? ceil) { Sec = sec; Rate = rate; PBuy = pBuy; PUp = pUp; PDn = pDn; Ceil = ceil; }
	}

	static class Sim
	{
		public static List<BarB> Run(int seed, List<Phase> phases, int R)
		{
			var rng = new Random(seed);
			int p = 0;
			double t = 0.0;
			var bars = new List<BarB>();
			BarB cur = new BarB { O = 0, H = 0, L = 0, C = 0, T0 = 0, T1 = 0 };
			foreach (var ph in phases)
			{
				int start = p;
				int? cap = ph.Ceil.HasValue ? start + ph.Ceil.Value : (int?)null;
				int n = (int)Math.Round(ph.Rate * ph.Sec * (0.9 + 0.2 * rng.NextDouble()));
				var times = new double[n];
				for (int i = 0; i < n; i++) times[i] = t + rng.NextDouble() * ph.Sec;
				Array.Sort(times);
				for (int i = 0; i < n; i++)
				{
					bool bu = rng.NextDouble() < ph.PBuy;
					int sz = 1; while (rng.NextDouble() > 0.45 && sz < 40) sz++;
					if (!cur.Lv.TryGetValue(p, out var lv)) { lv = new double[2]; cur.Lv[p] = lv; }
					if (bu)
					{
						cur.Buy += sz; lv[0] += sz; cur.D += sz;
						if (rng.NextDouble() < ph.PUp && (!cap.HasValue || p < cap.Value)) p++;
					}
					else
					{
						cur.Sell += sz; lv[1] += sz; cur.D -= sz;
						if (rng.NextDouble() < ph.PDn) p--;
					}
					cur.MaxD = Math.Max(cur.MaxD, cur.D); cur.MinD = Math.Min(cur.MinD, cur.D);
					cur.H = Math.Max(cur.H, p); cur.L = Math.Min(cur.L, p); cur.C = p; cur.T1 = times[i]; cur.Trades++;
					if (cur.H - cur.L >= R)
					{
						bars.Add(cur);
						cur = new BarB { O = p, H = p, L = p, C = p, T0 = times[i], T1 = times[i] };
					}
				}
				t += ph.Sec;
			}
			bars.Add(cur);
			return bars;
		}
	}

	class FakeConv : IChartWindowCoordinatesConverter
	{
		public int First, Bw, Left;
		public Func<double, double> YMap = v => 0.0;
		public Dictionary<long, int> ByTicks = new Dictionary<long, int>();
		public List<DateTime> Times;
		public double GetBarIndex(DateTime time)
		{
			if (ByTicks.TryGetValue(time.Ticks, out int i)) return i;
			int lo = 0, hi = Times.Count - 1;
			while (lo < hi) { int m = (lo + hi) / 2; if (Times[m] < time) lo = m + 1; else hi = m; }
			return lo;
		}
		public double GetChartX(DateTime time) => Left + (GetBarIndex(time) - First) * Bw;
		public double GetChartY(double price) => YMap(price);
		public DateTime GetTime(double x)
		{
			int i = First + (int)Math.Floor((x - Left) / Bw);
			i = Math.Max(0, Math.Min(Times.Count - 1, i));
			return Times[i];
		}
		public double GetPrice(double y) => 0.0;
	}

	class FakeWindow : IChartWindow
	{
		public IChartWindowCoordinatesConverter CoordinatesConverter { get; set; }
		public int WindowNumber { get; set; }
		public Rectangle ClientRectangle { get; set; }
		public double YScaleFactor => 1.0;
	}

	class FakeChart : IChart
	{
		public void RedrawBuffer() { }
		public int BarsWidth { get; set; } = 6;
		public TradingPlatform.BusinessLayer.TimeZone CurrentTimeZone => new TradingPlatform.BusinessLayer.TimeZone(TimeZoneInfo.Utc);
		public IChartWindow[] Windows { get; set; }
		public IChartWindow MainWindow => Windows[0];
		public double TickSize => 0.25;
		public int RightOffset => 0;
		public event EventHandler<ChartMouseNativeEventArgs> MouseDown { add { } remove { } }
		public event EventHandler<ChartMouseNativeEventArgs> MouseMove { add { } remove { } }
		public event EventHandler<ChartMouseNativeEventArgs> MouseUp { add { } remove { } }
		public event EventHandler<ChartMouseNativeEventArgs> MouseWheel { add { } remove { } }
	}

	class FakeProgress : IVolumeAnalysisCalculationProgress
	{
		public VolumeAnalysisCalculationState State => VolumeAnalysisCalculationState.Finished;
		public int ProgressPercent => 100;
		public void AbortLoading() { }
	}

	class TestCvd : TNXCVDPro
	{
		public void Setup(IChart c, Symbol s, HistoricalData h) { CurrentChart = c; Symbol = s; HistoricalData = h; }
		public void Init() => OnInit();
		public void Update(UpdateReason r) => OnUpdate(new UpdateArgs { Reason = r });
		public bool MinMax(out double mn, out double mx) => OnTryGetMinMax(0, Math.Max(0, Count - 1), out mn, out mx);
	}

	class TestIS : TNXImbalanceStrength
	{
		public void Setup(IChart c, Symbol s, HistoricalData h) { CurrentChart = c; Symbol = s; HistoricalData = h; }
		public void Init() => OnInit();
		public void Update(UpdateReason r) => OnUpdate(new UpdateArgs { Reason = r });
		public bool MinMax(out double mn, out double mx) => OnTryGetMinMax(0, Math.Max(0, Count - 1), out mn, out mx);
	}

	static class Program
	{
		const double Tick = 0.25;
		const double Base = 5800.0;

		static void Fill(List<BarB> bars, DateTime t0)
		{
			TnxFeed.O.Clear(); TnxFeed.H.Clear(); TnxFeed.L.Clear(); TnxFeed.C.Clear(); TnxFeed.V.Clear(); TnxFeed.Tk.Clear(); TnxFeed.T.Clear(); TnxFeed.VA.Clear();
			foreach (var b in bars)
			{
				TnxFeed.O.Add(Base + b.O * Tick); TnxFeed.H.Add(Base + b.H * Tick); TnxFeed.L.Add(Base + b.L * Tick); TnxFeed.C.Add(Base + b.C * Tick);
				TnxFeed.V.Add(b.Buy + b.Sell); TnxFeed.Tk.Add(b.Trades); TnxFeed.T.Add(t0.AddSeconds(b.T0));
				var va = new VolumeAnalysisData();
				va.Total = new VolumeAnalysisItem { Volume = b.Buy + b.Sell, BuyVolume = b.Buy, SellVolume = b.Sell, Delta = b.Buy - b.Sell, MaxDelta = b.MaxD, MinDelta = b.MinD };
				foreach (var kv in b.Lv)
					va.PriceLevels[Base + kv.Key * Tick] = new VolumeAnalysisItem { Volume = kv.Value[0] + kv.Value[1], BuyVolume = kv.Value[0], SellVolume = kv.Value[1], Delta = kv.Value[0] - kv.Value[1] };
				TnxFeed.VA.Add(va);
			}
		}

		static (FakeChart, FakeConv) Chart(int width, int bw, int first)
		{
			var conv = new FakeConv { First = first, Bw = bw, Left = 0, Times = TnxFeed.T };
			for (int i = 0; i < TnxFeed.T.Count; i++) conv.ByTicks[TnxFeed.T[i].Ticks] = i;
			var win = new FakeWindow { CoordinatesConverter = conv, WindowNumber = 0, ClientRectangle = new Rectangle(0, 0, width, 100) };
			var chart = new FakeChart { BarsWidth = bw, Windows = new IChartWindow[] { win } };
			return (chart, conv);
		}

		static string Dump(object ind, string field, string[] names, int n)
		{
			var fi = ind.GetType().BaseType.GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
			var arr = (Array)fi.GetValue(ind);
			var et = arr.GetType().GetElementType();
			var fs = names.Select(nm => et.GetField(nm, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)).ToArray();
			var sb = new StringBuilder();
			sb.AppendLine("bar," + string.Join(",", names));
			for (int i = 0; i < n; i++)
			{
				object b = arr.GetValue(i);
				sb.Append(i);
				foreach (var f in fs) { object v = f.GetValue(b); sb.Append(',').Append(v is double d ? d.ToString("0.###", CultureInfo.InvariantCulture) : Convert.ToString(v, CultureInfo.InvariantCulture)); }
				sb.AppendLine();
			}
			return sb.ToString();
		}

		static void Main(string[] args)
		{
			string outDir = args.Length > 0 ? args[0] : "test/out";
			Directory.CreateDirectory(outDir);
			var phases = new List<Phase>
			{
				new Phase(9000, 4, .5, .12, .12, null),          // base: balanced, 60+ bars
				new Phase(110, 6, .6, .15, .10, null), new Phase(110, 10, .68, .16, .10, null), new Phase(110, 14, .74, .17, .10, null),   // A real push
				new Phase(600, 4, .5, .12, .12, null),           // rest
				new Phase(600, 15, .62, .15, .20, 3), new Phase(300, 10, .30, .10, .17, null),   // B absorption then drop
				new Phase(600, 4, .5, .12, .12, null),           // rest
				new Phase(140, 3, .85, .55, .2, null), new Phase(320, 5, .30, .10, .17, null),   // C thin then reverse
			};
			var bars = Sim.Run(1, phases, 8);
			double total = bars[bars.Count - 1].T1;
			DateTime t0 = DateTime.UtcNow.AddSeconds(-total - 5);
			Fill(bars, t0);
			int N = bars.Count;
			Console.WriteLine($"bars={N}");
			var symbol = new Symbol { TickSize = Tick, Name = "ES" };
			var hd = new HistoricalData { Aggregation = new HistoryAggregationRangeBars { RangeBars = 8 }, VolumeAnalysisCalculationProgress = new FakeProgress() };

			// ---------- TNX IMBALANCE STRENGTH ----------
			int width = 1700, bw = 6;
			int first = Math.Max(0, N - width / bw + 2);
			{
				var (chart, conv) = Chart(width, bw, first);
				var looks = new[] { TNXImbalanceStrength.Look.Bars, TNXImbalanceStrength.Look.Area, TNXImbalanceStrength.Look.Needles, TNXImbalanceStrength.Look.Lollipop, TNXImbalanceStrength.Look.Capsules };
				foreach (var look in looks)
				{
					var ind = new TestIS();
					ind.Style = look;
					ind.Setup(chart, symbol, hd);
					ind.Init();
					var sw = System.Diagnostics.Stopwatch.StartNew();
					for (int i = 1; i <= N; i++) { TnxFeed.Count = i; ind.Update(i == 1 ? UpdateReason.HistoricalBar : UpdateReason.NewBar); }
					ind.VolumeAnalysisData_Loaded();
					for (int k = 0; k < 5; k++) ind.Update(UpdateReason.NewTick);
					sw.Stop();
					Console.WriteLine($"IS {look}: replay {N} bars + 5 ticks in {sw.ElapsedMilliseconds} ms");
					ind.MinMax(out double mn, out double mx);
					int h = 480;
					conv.YMap = v => (mx - v) / (mx - mn) * h;
					using (var bmp = new Bitmap(width, h))
					using (var g = Graphics.FromImage(bmp))
					{
						g.Clear(Color.FromArgb(11, 13, 18));
						var sw2 = System.Diagnostics.Stopwatch.StartNew();
						ind.OnPaintChart(new PaintChartEventArgs { Graphics = g, Rectangle = new Rectangle(0, 0, width, h), WindowIndex = 0 });
						sw2.Stop();
						Console.WriteLine($"IS {look}: paint {sw2.ElapsedMilliseconds} ms");
						bmp.Save(Path.Combine(outDir, $"IS_{look}.png"), ImageFormat.Png);
					}
					if (look == TNXImbalanceStrength.Look.Bars)
						File.WriteAllText(Path.Combine(outDir, "IS_bars.csv"), Dump(ind, "_b", new[] { "Real", "Closed", "Side", "Dir", "Agree", "Dur", "Vps", "Omega", "RPi", "REff", "ROmega", "RZeta", "RJ4", "G", "W", "Psi", "Run", "FpOk", "KWith", "KAgainst", "ShWith", "Phi", "UaRejected", "Fake", "IS", "St", "AccN", "Kappa", "Levels" }, N));
					ind.Dispose();
				}
			}

			// ---------- TNX CVD PRO 2.3 ----------
			{
				var (chart, conv) = Chart(width, bw, first);
				var variants = new List<(string name, Action<TestCvd> cfg)>
				{
					("velocity_bars", c => { c.Mode = TNXCVDPro.Display.Velocity; }),
					("velocity_area", c => { c.Mode = TNXCVDPro.Display.Velocity; c.FlowStyle = TNXCVDPro.FlowLook.Area; }),
					("velocity_histogram", c => { c.Mode = TNXCVDPro.Display.Velocity; c.FlowStyle = TNXCVDPro.FlowLook.Histogram; }),
					("velocity_ribbon", c => { c.Mode = TNXCVDPro.Display.Velocity; c.FlowStyle = TNXCVDPro.FlowLook.Ribbon; }),
					("velocity_needles_noaccel", c => { c.Mode = TNXCVDPro.Display.Velocity; c.FlowStyle = TNXCVDPro.FlowLook.Needles; c.ShowAccel = false; }),
					("velocity_bars_noaccel", c => { c.Mode = TNXCVDPro.Display.Velocity; c.ShowAccel = false; }),
					("velocity_classic22", c => { c.Mode = TNXCVDPro.Display.Velocity; c.Shade = TNXCVDPro.FlowShade.Classic; c.GateThrust = false; c.ShowFade = false; c.AccelInFlow = false; c.CardCompass = false; c.SignalAvgFallback = false; }),
					("candles", c => { c.Mode = TNXCVDPro.Display.Candles; }),
				};
				foreach (var v in variants)
				{
					var ind = new TestCvd();
					v.cfg(ind);
					ind.Setup(chart, symbol, hd);
					ind.Init();
					var sw = System.Diagnostics.Stopwatch.StartNew();
					for (int i = 1; i <= N; i++) { TnxFeed.Count = i; ind.Update(i == 1 ? UpdateReason.HistoricalBar : UpdateReason.NewBar); }
					ind.VolumeAnalysisData_Loaded();
					for (int k = 0; k < 5; k++) ind.Update(UpdateReason.NewTick);
					sw.Stop();
					Console.WriteLine($"CVD {v.name}: replay {N} bars + 5 ticks in {sw.ElapsedMilliseconds} ms");
					int h = 560;
					ind.MinMax(out double mn, out double mx);
					conv.YMap = val => (mx - val) / (mx - mn) * h;
					using (var bmp = new Bitmap(width, h))
					using (var g = Graphics.FromImage(bmp))
					{
						g.Clear(Color.FromArgb(11, 13, 18));
						var sw2 = System.Diagnostics.Stopwatch.StartNew();
						for (int rep = 0; rep < 2; rep++)
							ind.OnPaintChart(new PaintChartEventArgs { Graphics = g, Rectangle = new Rectangle(0, 0, width, h), WindowIndex = 0 });
						sw2.Stop();
						Console.WriteLine($"CVD {v.name}: 2 paints {sw2.ElapsedMilliseconds} ms");
						bmp.Save(Path.Combine(outDir, $"CVD_{v.name}.png"), ImageFormat.Png);
					}
					if (v.name == "velocity_bars")
						File.WriteAllText(Path.Combine(outDir, "CVD_bars.csv"), Dump(ind, "_b", new[] { "Real", "Delta", "Vol", "Dur", "Fast", "Slow", "AccE", "Imb7", "Imb3", "Vps", "FRank", "SRank", "BusyRank", "ImbRank", "Q", "Absorbed", "Fade", "ThrustOk", "Flip" }, N));
					ind.Dispose();
				}
			}
			Console.WriteLine("done");
		}
	}
}
