"""Builds TNX-CVD-PRO-GUIDE.html (QUANTOWER/TNX-CVD-PRO/NEW-LATEST) from the pictures the test machine draws with CVD PRO's own code.

    cd QUANTOWER/TNX-TEST/Harness && dotnet run -c Release -- cvd-guide      (pictures + guide.json)
    python QUANTOWER/TNX-CVD-PRO/guide/make_guide.py                           (this file: the guide)

The pictures come from the test machine's made-up markets (prices and times are made up). The numbers in the examples are
read from guide.json (what CVD PRO wrote on that bar); the test results are those of the 2.0 build (2026-10-10).
"""
import base64
import hashlib
import html
import json
from pathlib import Path

HERE = Path(__file__).resolve().parent
ROOT = HERE.parent                                   # QUANTOWER/TNX-CVD-PRO
PICS = ROOT.parent / "TNX-TEST" / "Harness" / "obj" / "pictures" / "cvd-guide"
DLL = ROOT / "NEW-LATEST" / "TNXCVDPro.dll"
OUT = ROOT / "NEW-LATEST" / "TNX-CVD-PRO-GUIDE.html"
TICK = 0.25

META = {m["file"]: m for m in json.loads((PICS / "guide.json").read_text(encoding="utf-8"))}


def e(s):
    return html.escape(str(s), quote=True)


def img(file, alt, caption=None, cls="shot"):
    data = base64.b64encode((PICS / file).read_bytes()).decode("ascii")
    cap = f"<figcaption>{caption}</figcaption>" if caption else ""
    return f'<figure class="{cls}"><img src="data:image/png;base64,{data}" alt="{e(alt)}">{cap}</figure>'


def price(p):
    return f"{p:.2f}"


def plan(m):
    """The example's numbers by the guide's rules: entry on a break of the signal bar, stop 2 ticks beyond the defended price,
    target twice the risk."""
    if m["long"]:
        entry = m["high"] + TICK
        stop = m["defendLow"] - 2 * TICK
        risk = entry - stop
        target = entry + 2 * risk
    else:
        entry = m["low"] - TICK
        stop = m["defendHigh"] + 2 * TICK
        risk = stop - entry
        target = entry - 2 * risk
    return entry, stop, round(risk / TICK), target


# ------------------------------------------------------------------------------------------------ the examples' words

EX = {
    "ex-absorption-long.png": dict(
        means="Sellers hit the bid hard (a big negative delta) but price did not go down. Someone bought all of it with "
              "limit orders: <b>passive buyers absorbed the selling</b>.",
        use="Best at a level: the session low, a former swing low, a round number. Wait for the next bar to hold above the "
            "absorbed bar's low. Better still when the card then shows <b>SELLERS TRAPPED</b> or <b>Right now</b> turns to "
            "buyers.",
        cancel="a bar closes below the absorbed bar's low, or a short absorption appears above you.",
        research="Hidden liquidity is real: iceberg orders refill at a price, and market orders that hit them move price less "
                 "(Frey &amp; Sandås 2017; Pardo &amp; Pascual 2011). But a big delta with little progress also happens by "
                 "chance: trade imbalance explains only about a third of 10-second price changes and 1–13 % over 2–10 trades "
                 "(Cont, Kukanov &amp; Stoikov 2011). <b>The rule itself is not tested</b> (H-0006)."),
    "ex-absorption-short.png": dict(
        means="Buyers lifted the offer hard (a big positive delta) but price did not go up. <b>Passive sellers absorbed the "
              "buying</b> with limit orders.",
        use="Best at a level: the session high, a former swing high, a round number. Wait for the next bar to stay under the "
            "absorbed bar's high; better when the card shows <b>BUYERS TRAPPED</b> or <b>Right now</b> turns to sellers.",
        cancel="a bar closes above the absorbed bar's high, or a long absorption appears under you.",
        research="As for the long: the idea fits what is known about hidden liquidity; the rule is not tested (H-0006)."),
    "ex-exhaustion-long.png": dict(
        means="Price made a new low (lowest of the last 60 bars), but the selling push that made it was at most 70 % of the "
              "push at the previous low, and CVD made no new low (or the push was under half). <b>The selling is running "
              "out.</b> The card's \"push N % of previous\" is that ratio: 0 % = no selling push at all at the new low.",
        use="A warning at the low, not an entry by itself. Wait for a higher low or a bar that breaks the signal bar's high.",
        cancel="price makes a new low with a bigger selling push.",
        research="Order imbalance builds into an extreme and peaks there; part of the move then comes back (Mu et al. 2010: "
                 "about 1 % overreaction reversed within 10 minutes). Moves on liquidity-driven volume reverse, moves on "
                 "informed volume continue (Campbell, Grossman &amp; Wang 1993; Llorente et al. 2002) — a fading push alone "
                 "cannot tell which. <b>Not tested</b> (H-0008)."),
    "ex-exhaustion-short.png": dict(
        means="Price made a new high, but the buying push that made it was at most 70 % of the push at the previous high, and "
              "CVD made no new high. <b>The buying is running out.</b>",
        use="A warning at the high. Wait for a lower high or a bar that breaks the signal bar's low.",
        cancel="price makes a new high with a bigger buying push.",
        research="As for the long (Mu et al. 2010). <b>Not tested</b> (H-0008)."),
    "ex-flowdiv-long.png": dict(
        means="For 2 bars or more, price went <b>up</b> while the delta was <b>negative</b> (both at least 1× their typical size "
              "over 3 bars). The sellers pushed and lost: <b>sellers are trapped</b> under the market.",
        use="With the price, as a continuation sign: the trapped sellers' stops are above. Better when <b>In control</b> or "
            "<b>Right now</b> turns to buyers.",
        cancel="price falls back below the bar where the divergence began.",
        research="No study tests this rule (H-0007). What fits it: informed traders often use limit orders (Kaniel &amp; Liu "
                 "2006; Bloomfield, O'Hara &amp; Saar 2005), so price can follow the passive side against the aggressive "
                 "one."),
    "ex-flowdiv-short.png": dict(
        means="For 2 bars or more, price went <b>down</b> while the delta was <b>positive</b>. The buyers pushed and lost: "
              "<b>buyers are trapped</b> above the market.",
        use="With the price, as a continuation sign: the trapped buyers' stops are below.",
        cancel="price rises back above the bar where the divergence began.",
        research="As for the long. Not tested (H-0007)."),
    "ex-swingdiv-long.png": dict(
        means="Price made a <b>lower swing low</b> (3 bars each side), but CVD at that low was <b>higher</b> than at the "
              "previous low: less selling made the new low.",
        use="A reversal warning at a low. A swing is only known 3 bars after it, so 2.0 puts the marker there (the thin "
            "line goes back to the swing). Enter on a break of the confirming bar's high.",
        cancel="price makes a new low under the swing low.",
        research="No study tests the rule (C-0012). Lagged imbalance points to a partial reversal once the same-period "
                 "imbalance is known (CL-0040)."),
    "ex-swingdiv-short.png": dict(
        means="Price made a <b>higher swing high</b>, but CVD at that high was <b>lower</b> than at the previous high: less "
              "buying made the new high.",
        use="A reversal warning at a high; the marker sits on the bar that confirmed the swing. Enter on a break of the "
            "confirming bar's low.",
        cancel="price makes a new high above the swing high.",
        research="As for the long. Not tested (C-0012)."),
}

READ = {
    "read-thin.png": ("Thin book", "Price moved one full range with <b>little aggressive volume</b> (effort under 0.7×): few "
                      "resting orders were in the way. Fast and fragile: such moves come back just as easily."),
    "read-heavy.png": ("Heavy push broke through", "It took <b>2× or more the normal volume</b> to move one range: many resting "
                       "orders were there, and they were taken. 1.x called this bar \"BUYERS being ABSORBED\" — the "
                       "opposite of what happened."),
    "read-healthy.png": ("Healthy", "The push and the move agree: normal effort (0.7–2×) in the bar's direction."),
    "read-trapped.png": ("Trapped", "The delta was <b>against the bar's direction</b>: the aggressive side pushed and the bar "
                         "went the other way. The other side's limit orders won."),
}


# ------------------------------------------------------------------------------------------------ the page

def example_block(file):
    m, t = META[file], EX[file]
    side = "long" if m["long"] else "short"
    entry, stop, risk, target = plan(m)
    began = (f" It began {m['bar'] - m['began']} bar(s) earlier." if m["began"] < m["bar"] else "")
    again = (f" It fired again for {m['seen'] - m['bar']} more bar(s); the marker stayed and grew a tail."
             if m.get("seen", -1) > m["bar"] else "")
    strength = (f"strength {m['score']:.1f} ({m['times']:.1f}× its kind's recent average)" if m["average"] > 0
                else f"strength {m['score']:.1f} (fewer than 3 signals of its kind in the last 60 bars, so no average yet: "
                     f"every filter shows it)")
    return f"""
<div class="ex">
  <div class="head"><h4>{e(m['title'])}</h4><div><span class="pill p-{side}">{side.upper()}</span><span class="pill p-info">known at {e(m['known'])}</span></div></div>
  {img(file, m['title'])}
  <div class="body2">
    <div><h5>What CVD PRO saw</h5><p>"{e(m['note'])}" — {e(strength)}.{e(began)}{e(again)}</p>
         <h5>What it means</h5><p>{t['means']}</p></div>
    <div><h5>How to use it</h5><p>{t['use']}</p>
         <p class="planline"><span>Entry {price(entry)}</span><span>Stop {price(stop)}</span><span>Risk {risk} ticks</span><span>Target {price(target)} (2×)</span></p>
         <p class="small">The example's numbers by the rules above (made-up market). <b>Cancel the idea</b> when {t['cancel']}</p></div>
  </div>
  <div class="why"><b>The research:</b> {t['research']}</div>
</div>"""


def reading_block(file):
    m = META[file]
    title, words = READ[file]
    return f"""
<div class="ex">
  <div class="head"><h4>{e(title)}</h4><div><span class="pill p-info">{e(m['reading'])}</span></div></div>
  {img(file, title)}
  <div class="why">{words} <b>Last bar:</b> {e(m['speed'])}.</div>
</div>"""


def build():
    dll_sha = hashlib.sha256(DLL.read_bytes()).hexdigest() if DLL.exists() else "(not built)"
    ov = META["overview-range8.png"]
    rs = META.get("repaint-sequence.png", {})
    examples = "".join(example_block(f) for f in EX if f in META)
    readings = "".join(reading_block(f) for f in READ if f in META)
    page = f"""<!doctype html>
<html lang="en">
<head>
<meta charset="utf-8">
<meta name="viewport" content="width=device-width, initial-scale=1">
<title>TNX CVD PRO Guide</title>
<style>
  :root{{
    --bg:#0a0d14; --panel:#111624; --panel2:#161c2e; --line:#232b42; --text:#e8ecf4; --dim:#97a1b8;
    --up:#26c691; --down:#eb405c; --blue:#3da9fc; --gold:#ffd60a; --violet:#9682ff; --orange:#ff7a42; --absorb:#ffc43d;
  }}
  *{{box-sizing:border-box}}
  html{{scroll-behavior:smooth}}
  body{{margin:0;background:var(--bg);color:var(--text);font:16px/1.6 "Segoe UI",system-ui,Arial,sans-serif}}
  a{{color:var(--blue)}}
  .wrap{{max-width:1180px;margin:0 auto;padding:0 20px}}
  header.hero{{padding:56px 0 36px;background:radial-gradient(1200px 400px at 20% -10%,#0f3a2c55,transparent),radial-gradient(900px 380px at 90% 0%,#3a145a44,transparent);border-bottom:1px solid var(--line)}}
  .hero h1{{font-size:44px;line-height:1.1;margin:0 0 8px;letter-spacing:.5px}}
  .hero h1 span{{color:var(--up)}}
  .hero p{{color:var(--dim);margin:0;max-width:820px;font-size:18px}}
  .tag{{display:inline-block;border:1px solid var(--line);border-radius:999px;padding:3px 12px;font-size:13px;color:var(--dim);margin:14px 8px 0 0}}
  nav{{position:sticky;top:0;z-index:5;background:#0a0d14ee;backdrop-filter:blur(6px);border-bottom:1px solid var(--line)}}
  nav .wrap{{display:flex;gap:4px;overflow-x:auto;padding-top:8px;padding-bottom:8px}}
  nav a{{white-space:nowrap;text-decoration:none;color:var(--dim);font-size:14px;padding:6px 11px;border-radius:8px}}
  nav a:hover{{background:var(--panel2);color:var(--text)}}
  section{{padding:44px 0 8px}}
  h2{{font-size:28px;margin:0 0 6px;border-left:4px solid var(--up);padding-left:12px}}
  h3{{font-size:20px;margin:26px 0 8px}}
  h5{{margin:10px 0 2px;font-size:14px;color:var(--gold);text-transform:uppercase;letter-spacing:.6px}}
  .lead{{color:var(--dim);margin:0 0 18px;max-width:900px}}
  .card{{background:var(--panel);border:1px solid var(--line);border-radius:14px;padding:18px 20px;margin:14px 0}}
  .step{{display:flex;gap:14px;align-items:flex-start}}
  .num{{flex:0 0 34px;height:34px;border-radius:50%;background:var(--up);color:#04120d;font-weight:700;display:flex;align-items:center;justify-content:center}}
  .num.y{{background:var(--gold)}}
  .step h4{{margin:3px 0 2px;font-size:17px}}
  .step p{{margin:0;color:var(--dim)}}
  .warn{{border-left:4px solid var(--orange);background:#2a1a0f}}
  .good{{border-left:4px solid var(--up);background:#0e2620}}
  .info{{border-left:4px solid var(--blue);background:#0f1f33}}
  table{{width:100%;border-collapse:collapse;font-size:15px}}
  th,td{{padding:9px 10px;border-bottom:1px solid var(--line);text-align:left;vertical-align:top}}
  th{{color:var(--dim);font-weight:600;font-size:13px;text-transform:uppercase;letter-spacing:.6px}}
  td.n{{font-variant-numeric:tabular-nums;font-weight:600;white-space:nowrap}}
  .scroll{{overflow-x:auto}}
  .path{{color:var(--gold);word-break:break-all}}
  code{{background:var(--panel2);border:1px solid var(--line);border-radius:6px;padding:0 6px;font:13px Consolas,monospace}}
  figure.shot{{margin:14px 0}} figure.shot img{{display:block;max-width:100%;height:auto;border:1px solid var(--line);border-radius:10px;background:#0e1117}}
  figure.shot figcaption{{font-size:13px;color:var(--dim);margin-top:6px}}
  .legend{{display:grid;grid-template-columns:repeat(auto-fit,minmax(250px,1fr));gap:10px 18px;margin-top:10px}}
  .legend div{{display:flex;gap:10px;align-items:flex-start}}
  .legend .num{{flex:0 0 26px;height:26px;font-size:13px;background:var(--gold);color:#181200}}
  .legend p{{margin:0;color:var(--dim)}} .legend b{{color:var(--text)}}
  .pill{{display:inline-block;border-radius:6px;padding:1px 8px;font-size:12px;font-weight:700;margin-left:6px}}
  .p-long{{background:#0e3a2c;color:var(--up)}} .p-short{{background:#3a0f1f;color:var(--down)}} .p-info{{background:#112a44;color:var(--blue)}}
  .ex{{background:var(--panel);border:1px solid var(--line);border-radius:14px;margin:22px 0;overflow:hidden}}
  .ex .head{{display:flex;justify-content:space-between;align-items:center;gap:10px;padding:14px 18px;border-bottom:1px solid var(--line);flex-wrap:wrap}}
  .ex .head h4{{margin:0;font-size:19px}}
  .ex figure.shot{{margin:0}} .ex figure.shot img{{border:0;border-radius:0}}
  .ex .body2{{display:grid;grid-template-columns:1fr 1fr;gap:18px;padding:6px 18px 4px}}
  .ex .body2 p{{margin:2px 0 8px;color:var(--dim)}} .ex .body2 b{{color:var(--text)}}
  .planline span{{display:inline-block;margin:0 8px 4px 0;background:var(--panel2);border:1px solid var(--line);border-radius:6px;padding:1px 8px;color:var(--text);font-size:14px}}
  .small{{font-size:13.5px}}
  .why{{padding:4px 18px 16px;font-size:14px;color:var(--dim)}} .why b{{color:var(--text)}}
  .mk{{display:grid;grid-template-columns:150px 1fr;gap:14px;align-items:start;padding:12px 0;border-bottom:1px solid var(--line)}}
  .mk:last-child{{border-bottom:0}}
  .mk .sym{{font-size:15px;font-weight:700}}
  .mk p{{margin:0;color:var(--dim)}} .mk p b{{color:var(--text)}}
  ol.steps li{{margin:0 0 8px;color:var(--dim)}} ol.steps b{{color:var(--text)}}
  @media (max-width:860px){{.ex .body2{{grid-template-columns:1fr}}.mk{{grid-template-columns:1fr}}.hero h1{{font-size:34px}}}}
  footer{{color:var(--dim);font-size:14px;padding:30px 0 60px;border-top:1px solid var(--line);margin-top:40px}}
</style>
</head>
<body>

<header class="hero">
  <div class="wrap">
    <h1>TNX <span>CVD PRO</span> 2.1</h1>
    <p>What every part of CVD PRO means on your range and tick charts, the settings to use, and how to read its signals long and short. In simple words.</p>
    <span class="tag">Quantower (AMP)</span><span class="tag">Range bars</span><span class="tag">Tick charts</span><span class="tag">Cumulative delta · order flow</span>
  </div>
</header>

<nav><div class="wrap">
  <a href="#start">1 · Start</a>
  <a href="#chart">2 · Your chart</a>
  <a href="#card">3 · The card</a>
  <a href="#markers">4 · Markers</a>
  <a href="#signals">5 · Signals long &amp; short</a>
  <a href="#lastbar">6 · The last bar</a>
  <a href="#velocity">7 · Velocity mode</a>
  <a href="#settings">8 · Settings</a>
  <a href="#new">9 · New in 2.0</a>
  <a href="#routine">10 · Routine</a>
  <a href="#limits">11 · Limits</a>
  <a href="#proof">Check it yourself</a>
</div></nav>

<div class="wrap">

<section id="start">
  <h2>1 · Start here</h2>
  <p class="lead">Do these steps once. Quantower keeps your CVD PRO settings with each chart. CVD PRO writes no files anywhere.</p>
  <div class="card step"><div class="num">1</div><div>
    <h4>Close Quantower completely</h4>
    <p>Quantower reads the indicator file when it starts. Never replace the file while Quantower is open.</p>
  </div></div>
  <div class="card step"><div class="num">2</div><div>
    <h4>Put the new file in place</h4>
    <p>The new file: <span class="path">C:\\Users\\AMINE\\Desktop\\TNX\\TNXSCRIPTS\\QUANTOWER\\TNX-CVD-PRO\\NEW-LATEST\\TNXCVDPro.dll</span><br>
    It replaces: <span class="path">C:\\AMP Quantower\\Settings\\Scripts\\Indicators\\TNXCVDPro.dll</span><br>
    Your original file stays in <span class="path">QUANTOWER\\TNX-CVD-PRO\\OLD-ORIGINAL\\TNXCVDPro.dll</span>. Claude can do this step for you and check the copy byte by byte (only with your OK).</p>
  </div></div>
  <div class="card step"><div class="num">3</div><div>
    <h4>Start Quantower</h4>
    <p>Your charts open with CVD PRO and your settings as before. At the end of its settings there are new ones whose names start with <b>2.0</b> and <b>2.1</b>. Their defaults are CVD PRO 2.1.</p>
  </div></div>
  <div class="card step"><div class="num y">↩</div><div>
    <h4>Going back is always possible</h4>
    <p>Set every "2.0" setting to <b>Classic</b> / off and CVD PRO is exactly 1.x again (tested: every value and every pixel the same as the original). Or put the old file back.</p>
  </div></div>
  <div class="card info">The pictures in this guide come from the test machine: made-up markets (range bars of 8 ticks, tick bars of 40 trades), drawn by CVD PRO's own code. The price bars above CVD PRO's window are drawn on the same columns. Prices and times are made up; on your chart the signals are the real ones.</div>
</section>

<section id="chart">
  <h2>2 · What you see on your chart</h2>
  <p class="lead">CVD PRO draws in its own window under the price. In <b>Candles</b> mode you see the CVD candles, the signal markers and the info card.</p>
  {img("overview-range8.png", "CVD PRO 2.0 on a range chart")}
  <div class="legend">
    <div><div class="num">1</div><p><b>Price</b> — your range bars (Quantower draws them).</p></div>
    <div><div class="num">2</div><p><b>CVD candles</b> — each candle is one bar's delta (buy volume − sell volume) added to the running total. The wicks are the highest and lowest the running total reached inside the bar.</p></div>
    <div><div class="num">3</div><p><b>A signal marker</b> — here a swing divergence (section 4).</p></div>
    <div><div class="num">4</div><p><b>The info card</b> — the reading of the bar now and of the last finished bar (section 3).</p></div>
  </div>
  <h3>On a tick chart</h3>
  {img("overview-tick40.png", "CVD PRO 2.0 on a 40-tick chart", "The card's title shows the chart type CVD PRO found: here <b>40 tick</b>.")}
  <h3>In Velocity mode</h3>
  {img("overview-velocity.png", "CVD PRO 2.0 in Velocity mode")}
  <div class="legend">
    <div><div class="num">1</div><p><b>Price</b>.</p></div>
    <div><div class="num">2</div><p><b>CVD area</b> — the CVD as candles (or a line). The badge on the right says <b>AUTO</b> (scale follows the bars on screen) or <b>MANUAL</b>.</p></div>
    <div><div class="num">3</div><p><b>Flow speed lanes</b> — the fast flow as bars, the slow flow as the white line over a soft green / red area; the pill on the right says who and how strong (section 7).</p></div>
    <div><div class="num">4</div><p><b>Acceleration strip</b> — up = the flow speeds up, down = it slows; the colour is always the side (green buyers, red sellers): solid = speeding up, <b>hollow = slowing</b>, faint = steady.</p></div>
    <div><div class="num">5</div><p><b>The info card</b>.</p></div>
  </div>
</section>

<section id="card">
  <h2>3 · The info card, row by row</h2>
  <p class="lead">The top part reads the bar that is forming now (tick by tick). On range and tick charts, the <b>Last bar</b> rows read the bar that just finished. Green = buyers, red = sellers, grey = nothing special. Short words, so the card stays small.</p>
  <div class="card scroll"><table>
  <tr><th>Row</th><th>Example</th><th>What it means</th></tr>
  <tr><td><b>Title</b></td><td class="n">CVD PRO · range 8t · ● LIVE</td><td>The chart type CVD PRO found (range 8t, 40 tick, 1 min). <b>LIVE</b>: order-flow data is in. <b>LOADING</b>: Quantower's volume analysis is still loading — wait. <b>⚠ N errors</b> (only if it happens): CVD PRO caught N errors inside since Quantower started (counted in memory, never written to a file) — tell Claude the number.</td></tr>
  <tr><td><b>Bar now</b> <span class="pill p-info">2.1</span></td><td class="n">▬▬▬▬▭ 75% done · 25 of 40 trades</td><td>How far the bar that is forming has got: on range bars its range so far against a full one, on tick bars the trades so far. Its colour: bought (green) or sold (red) so far.</td></tr>
  <tr><td><b>The big number</b></td><td class="n">−1687 &nbsp; [bar +14]</td><td>The CVD: the running total of delta since the last reset (default: each session); its colour says above or below zero. The tag: the delta of the bar now.</td></tr>
  <tr><td><b>Flow now</b></td><td class="n">Selling · quiet · 30%</td><td>The fast flow (the average delta of the last 7 bars): who, and how strong against the last 60 bars. The % = how many of them had a smaller flow. <b>quiet</b> under 50 %, <b>building</b> 50–80 %, <b>strong</b> 80–95 %, <b>EXTREME</b> 95 % and up.</td></tr>
  <tr><td><b>Flow trend</b></td><td class="n">Buying · EXTREME · 95%</td><td>The same for the slow flow (the last 21 bars).</td></tr>
  <tr><td><b>Speed</b></td><td class="n">Sellers slowing 1.4×</td><td>Is the flow getting stronger (<b>speeding up</b>, <b>THRUST</b> from 2×) or weaker (<b>slowing</b>, <b>STALL</b> from 2×), in times its typical change; <b>steady</b> under 1×. Always in the side's colour.</td></tr>
  <tr><td><b>Push</b></td><td class="n">▭▭▬| Sellers 1.4×</td><td>The effort: this bar's delta ÷ the typical bar delta of the last 60 bars. The small bar grows from the middle mark; the thin marks are 1× (normal). Under 0.7×: <b>nobody</b>.</td></tr>
  <tr><td><b>Move</b></td><td class="n">|▬▭ Up 1.3× · +7 ticks</td><td>The result: this bar's price move ÷ the typical move.</td></tr>
  <tr><td><b>Reading</b></td><td class="n">[SELLERS TRAPPED]</td><td>Effort against result for the bar now (table below), in short words: SELLERS / BUYERS TRAPPED, SELLERS / BUYERS ABSORBED, thin buying · fragile, healthy buying, quiet · wait …</td></tr>
  <tr><td><b>Absorbed by</b></td><td class="n">passive buyers</td><td>Who took the other side (with limit orders) when the aggressive side is absorbed or trapped; "–" otherwise.</td></tr>
  <tr><td><b>Last bar</b> <span class="pill p-info">2.0</span></td><td class="n">▲ 10.2 min · 3.6× slower</td><td>Range and tick charts only. The last finished bar: its direction, how long it took, against the middle time of the last 60 bars (0.8× to 1.25×: <b>normal pace</b>).</td></tr>
  <tr><td><b>Last reading</b> <span class="pill p-info">2.0</span></td><td class="n">[heavy buying 3.0×]</td><td>Range bars: the effort it took to move one range (section 6): ▲ / ▼ thin book (little push), healthy buying / selling, heavy buying / selling N×, BUYERS / SELLERS TRAPPED. Tick bars: the verdict of the finished bar.</td></tr>
  <tr><td><b>Control 40</b></td><td class="n">▬▬▬▬▬ Buyers 53%</td><td>The buyers' share (green part) of the volume over the last 40 bars; the dark mark is 50 %.</td></tr>
  <tr><td><b>Now 8</b></td><td class="n">▬▬▬▬▬ Sellers 54% [TURNING]</td><td>The same over the last 8 bars. <b>TURNING</b> when now is clearly on the other side of a clear control.</td></tr>
  </table></div>
  <h3>The card fits itself</h3>
  <p>The card is a small block. By default (<b>2.1 Card place = Auto</b>) it sits <b>top left, over the oldest bars</b>: the newest bars on the right are never covered. It takes at most 45 % of the window's width (55 % in a window under 140 px), or exactly the width you set. Its size is your "2.1 Card size" (default 90 %); it never grows with the window. When the window is short its rows go into 2 columns (3 or 4 only in a very short window), and only then the text shrinks, never under 75 %. Every row stays; a text too long for its place ends in "…". Its layout never moves with the numbers: it changes only when you resize the window or change a setting.</p>
  {img("fit-candles-300.png", "The card in a 300 px window", "Candles mode, a 300 px window: the card top left, one column.")}
  {img("fit-velocity-150.png", "The card in a 150 px Velocity window", "Velocity mode, a 150 px window: every row in three columns, top left over the oldest bars; the newest bars and the flow pill stay clear.")}
  <h3>Your card: place, size, width, black</h3>
  {img("card-custom.png", "The card top left, 125 %, fully black, compact", "The same chart with the card top left (2.1 Card place), 125 % (2.1 Card size), 100 % black (2.1 Card background) and Compact rows.")}
  <div class="card scroll"><table>
  <tr><th>Setting</th><th>What it does</th></tr>
  <tr><td>2.1 Card place</td><td><b>Auto: top left, over the oldest bars</b> (default) · as the 'Info table' corner · top right · top left · bottom right · bottom left.</td></tr>
  <tr><td>2.1 Card move right / down</td><td>Pixels from that place (minus = left / up). The card always stays inside the window.</td></tr>
  <tr><td>2.1 Card width</td><td>0 = as wide as its text (default); or a fixed width in pixels.</td></tr>
  <tr><td>2.1 Card size</td><td>70 % to 160 % (all its text and bars); default 90 %. The largest it will be: it shrinks only when the window cannot hold it, never under 75 %.</td></tr>
  <tr><td>2.1 Card background: black</td><td>0 % (see-through) to 100 % (solid black); default 94 %.</td></tr>
  <tr><td>2.1 Card rows</td><td><b>Full</b> or <b>Compact</b> (bar now, the CVD, flow now, reading, last bar, control).</td></tr>
  </table></div>
  <h3>The market verdict</h3>
  <div class="card scroll"><table>
  <tr><th>Effort (delta)</th><th>Result (price move)</th><th>Verdict</th></tr>
  <tr><td>under 0.7×</td><td>under 0.7×</td><td>quiet – wait</td></tr>
  <tr><td>under 0.7×</td><td>0.7× or more</td><td>rises / falls with no push – thin, fragile</td></tr>
  <tr><td>0.7× or more</td><td>0.7× or more <b>against</b> the push</td><td>BUYERS / SELLERS TRAPPED</td></tr>
  <tr><td>1× or more</td><td>less than 0.35 × the effort, with the push</td><td>BUYERS / SELLERS being ABSORBED</td></tr>
  <tr><td>0.7× to 1×</td><td>less than 0.35 × the effort</td><td>buying / selling stalls</td></tr>
  <tr><td>0.7× or more</td><td>more than 2.5 × the effort</td><td>rises / falls on thin buying / selling – fragile</td></tr>
  <tr><td colspan="2">anything else</td><td>healthy buying / selling</td></tr>
  </table></div>
  <div class="card warn"><b>On range bars</b> every finished bar moves one full range, so the result is almost always about 1×. Read that way, a range bar that went up on 3× the normal buying showed "BUYERS being ABSORBED" in 1.x — the opposite of what happened. 2.0 reads finished range bars by their effort instead (section 6). The forming bar keeps this verdict: its move is not finished yet.</div>
</section>

<section id="markers">
  <h2>4 · The markers</h2>
  <p class="lead">Long signals sit <b>under</b> the CVD candles, short signals <b>over</b> them, on the bar where CVD PRO knew them. By default signals are read on <b>closed bars only</b> (recommended on range bars).</p>
  <div class="card">
    <div class="mk"><div class="sym" style="color:var(--absorb)">■ Absorption</div><p><b>A big delta (2.5× typical or more) and price did not go its way</b> (0.25× typical or less), on the bar or over the last 3 bars. Long: sellers absorbed. Short: buyers absorbed.</p></div>
    <div class="mk"><div class="sym" style="color:var(--orange)">▲▼ Exhaustion</div><p><b>A new high (low) of the last 60 bars made with a smaller push</b>: the 3-bar push is at most 70 % of the push at the previous high (low), and CVD makes no new extreme or the push is under half. Short at a high, long at a low.</p></div>
    <div class="mk"><div class="sym" style="color:var(--violet)">◆ Flow divergence</div><p><b>Price and delta move the opposite way</b>, both at least 1× typical over 3 bars, for 2 bars in a row. Price up on net selling = long (sellers trapped).</p></div>
    <div class="mk"><div class="sym" style="color:var(--violet)">◆ Swing divergence</div><p><b>A higher swing high with a lower CVD</b> (short), or a lower swing low with a higher CVD (long); swings have 3 bars on each side, never across a reset. A dashed line joins the two swings.</p></div>
  </div>
  <h3>The Pro badges (2.1)</h3>
  <div class="card scroll"><table>
  <tr><th>Badge</th><th>Symbol inside</th><th>Why that symbol</th></tr>
  <tr><td style="color:var(--absorb)"><b>● Absorption</b></td><td>an arrow stopped by a wall</td><td>the aggressive side ran into resting orders and did not get through</td></tr>
  <tr><td style="color:var(--orange)"><b>● Exhaustion</b></td><td>a double chevron, the second one fading</td><td>the push runs out; the chevrons point the way of the turn</td></tr>
  <tr><td style="color:var(--violet)"><b>● Flow divergence</b></td><td>two opposite arrows</td><td>price one way, delta the other</td></tr>
  <tr><td style="color:var(--violet)"><b>◆ Swing divergence</b></td><td>a diamond with the side's dot</td><td>it is joined to its two swings by the dashed line</td></tr>
  </table></div>
  <p class="small">Every badge has a small pointer to its candle (long badges under the CVD candles, short ones over them) and a dark ring, so it reads on any candle. A stronger signal than its kind's recent average is a little bigger. The setting "2.0 Markers style" keeps the 2.0 shapes (Clean) and the 1.x shapes (Classic).</p>
  <h3>What 2.0 adds to every marker</h3>
  <div class="card scroll"><table>
  <tr><th>You see</th><th>It means</th></tr>
  <tr><td>A marker never moves or disappears</td><td>Once a marker is on a closed bar it stays there. In 1.x a stronger reading within 3 bars deleted it, so a flow divergence's marker walked forward every bar, and after a reload the history did not show what you saw live.</td></tr>
  <tr><td>A thin tail to the right</td><td>The same signal fired again on the next bars. The marker stays on its first bar; its strength is the strongest reading.</td></tr>
  <tr><td>A bigger marker</td><td>Stronger than the recent average of its kind (0.85× to 1.3× the normal size).</td></tr>
  <tr><td>The swing diamond 3 bars after the swing</td><td>A swing is only known 3 bars later; 2.0 puts the diamond there, with a thin line back to the swing. 1.x drew it on the swing itself.</td></tr>
  <tr><td>A card when you point at a marker</td><td>What it is, what CVD PRO saw, how strong, and when it was known.</td></tr>
  </table></div>
  <div class="card info"><b>The signal filter</b> (setting "Show signals"): <b>All</b>, <b>Above lookback average</b> (default) or <b>Extreme</b> (1.5× the average of its kind over the last 60 bars). 2.0 answers the filter once, when the signal is known, and keeps that answer: a marker you saw never vanishes. A signal that becomes strong enough later shows on the bar where it did, and its card says it began earlier.</div>
</section>

<section id="signals">
  <h2>5 · Every signal, long and short</h2>
  <p class="lead">One signal at a time: the picture with its card as CVD PRO draws it, what it saw, what it means, how to use it, and what the research says. The gold column marks the bar where the signal was known; the gold ring marks the marker.</p>
  <div class="card info"><b>The rules every example follows.</b> Entry on a break of the signal bar (above its high for a long, below its low for a short). Stop 2 ticks beyond the price the signal defended (the low or high of the bar, the swing, or the bar where it began). Target: the next level of the other side, at least twice the risk; a level in the way = skip. <b>One marker alone is information</b>: a trade needs a second thing that agrees (section 10). These rules show how to read a signal; they are not a tested edge.</div>
  {examples}
</section>

<section id="lastbar">
  <h2>6 · The last bar (range and tick charts)</h2>
  <p class="lead">Range and tick bars hide time, and every finished range bar moves one range. So 2.0 adds two rows for the bar that just finished: how fast it was, and what it took to make it.</p>
  <h3>How fast: the Last bar row</h3>
  <p>The time to cross a fixed price distance measures how fast the market moves (Andersen, Dobrev &amp; Schaumburg 2008). A range bar <b>3× faster than typical</b> = the market is moving (news, a run); <b>3× slower</b> = quiet. CVD PRO compares each bar with the middle time of the last 60 bars.</p>
  <h3>What it took: the Last bar reading</h3>
  <div class="card scroll"><table>
  <tr><th>Reading</th><th>When</th><th>What it means</th></tr>
  <tr><td><b>rose / fell a full range on little push – thin book</b></td><td>effort under 0.7×</td><td>Few resting orders in the way. Fast and fragile.</td></tr>
  <tr><td><b>healthy buying / selling</b></td><td>effort 0.7× to 2× with the bar</td><td>The push and the move agree.</td></tr>
  <tr><td><b>heavy buying / selling broke through (N× normal effort)</b></td><td>effort 2× or more with the bar</td><td>Many resting orders were there and were taken.</td></tr>
  <tr><td><b>buyers / sellers pushed, the bar fell / rose – TRAPPED</b></td><td>delta against the bar</td><td>The aggressive side lost.</td></tr>
  </table></div>
  <p class="small">Why effort: the price change is about the order-flow imbalance divided by the depth of the book, so the flow needed to move a fixed distance grows with the depth (Cont, Kukanov &amp; Stoikov 2011). On tick bars the range changes from bar to bar, so the classic verdict still works there and is kept.</p>
  {readings}
</section>

<section id="velocity">
  <h2>7 · Velocity mode: flow speed and acceleration</h2>
  <p class="lead">The lanes show how strong the flow is, bar by bar. In the Pro look (2.1, setting "2.1 Flow speed and acceleration lanes style"): the fast flow as bars, the slow flow as a white line over a soft area (green above zero, red below), the bar that is forming drawn hollow (it is still moving), and a pill with four dots: how many strength steps the flow has reached (quiet · building · strong · EXTREME).</p>
  <h3>2.2 · Three panes that work like Quantower sub-windows</h3>
  <p>The CVD, the flow speed and the acceleration are three panes, each with its own price panel on its right. In the Pro look:</p>
  <div class="card scroll"><table>
  <tr><th>You do</th><th>Where</th><th>What happens</th></tr>
  <tr><td>drag up / down, or the wheel</td><td>a pane's price panel (its right side)</td><td>that pane's bars or candles get <b>bigger / smaller</b>; nothing else moves</td></tr>
  <tr><td>the same on the <b>CVD</b> panel</td><td>CVD price panel</td><td>works from AUTO too; the candles grow <b>around the newest candle</b> (it stays where it is; if you moved away from it, around the middle of what you see) and the newest candle is <b>kept in view</b> while trading (Quantower's "Keep in view"); the badge shows MANUAL</td></tr>
  <tr><td>drag up / down</td><td>inside the CVD pane in MANUAL</td><td>the view moves <b>exactly with the mouse</b>, up and down, as far as you want; if you leave the newest candle in view it stays in view while trading; if you move away to look at older candles, the view stays where you put it (Quantower's Manual)</td></tr>
  <tr><td>double-click (or right-click)</td><td>a pane's price panel</td><td>back to automatic (CVD: AUTO)</td></tr>
  <tr><td>drag</td><td>the line between two panes</td><td>only those two panes change size; no pane goes under its minimum (CVD 28 %, flow 18 %, acceleration 14 % of the window); the new sizes are kept in the two height settings (saved with the workspace) and follow the window when it is resized</td></tr>
  <tr><td>double-click (or right-click)</td><td>the line between two panes</td><td>the default sizes (flow 34 %, acceleration 18 %)</td></tr>
  <tr><td>drag / wheel</td><td>inside the flow or acceleration pane</td><td>the chart scrolls / zooms as always (Quantower); the lane's bars do not move</td></tr>
  <tr><td>click</td><td>the AUTO / MANUAL badge</td><td>AUTO ↔ MANUAL, without a jump</td></tr>
  </table></div>
  <p><b>Every bar ends inside its lane</b>: the acceleration scale fits the visible bars (the biggest one + 4 %, never under 2.5×, so a quiet stretch is not blown up), the flow scale too; the lanes have no caption rows any more (the name and the reading are a small pill at the top left, after the card), so the bars get the whole height. If you stretch a lane so much that a bar passes its edge, a <b>bright cap</b> marks the cut bar. A stretched CVD pane hides the markers of candles that are out of view.</p>
  <div class="card scroll"><table>
  <tr><th>Word</th><th>2.0: rank among the last 60 bars</th><th>1.x: fixed multiples</th></tr>
  <tr><td>quiet / flat</td><td>under 50 %</td><td>under 0.4× the typical one-bar delta</td></tr>
  <tr><td>building / weak</td><td>50–80 %</td><td>0.4× to 1×</td></tr>
  <tr><td>strong / mod</td><td>80–95 %</td><td>1× to 2×</td></tr>
  <tr><td>EXTREME / STRONG</td><td>95 % and up</td><td>2× and up</td></tr>
  </table></div>
  <p>1.x compared a 7-bar average with the size of a single bar, so the words understated the flow: on the range test market 56 % of bars read "flat" and <b>STRONG never showed</b> (0 %). 2.0 ranks the flow in its own recent history, the way Easley, López de Prado &amp; O'Hara (2012) read order-flow imbalance (rank 0.9 = alarm): quiet, building, strong and EXTREME now show about 50 / 30 / 15 / 5 % of the time (on the test market 50 / 28 / 14 / 8 %). The setting "2.0 Flow speed tiers" brings back the 1.x words.</p>
  <p><b>Acceleration</b>: the change of the fast flow (lag 1 bar, smoothed over 3) ÷ its typical size. <b>Up = the flow speeds up, down = it slows</b>. The colour is always the side: <b>green = buyers, red = sellers</b>. A <b>solid</b> bar = speeding up (brighter from 2×), a <b>hollow</b> bar = slowing, a <b>faint</b> bar = steady (under 1×). The thin lines are 1× (faint) and 2× (brighter). From 2×: a <b>chevron</b> = THRUST, a <b>ring with a dot</b> = STALL (both in the side's colour); a <b>dot on the zero line</b> = the flow changed side. On random flow 1× or more happens on about 42 % of bars and 2× on about 11 %: one bar means little; several in a row mean more.</p>
  <div class="card info"><b>A short window</b>: the card stays top left over the oldest bars, in 2 to 4 columns, so the newest lane bars and the pills stay clear (section 3). You can also put it anywhere (2.1 Card place, move, size).</div>
  <div class="card info"><b>Measured before 2.2</b> (window 250 px): a stretch on the CVD panel in MANUAL put the newest candle 9 / 90 / 640 px <b>below</b> the pane (30 / 60 / 120 px of stretch); AUTO → MANUAL jumped; the badge sat on the line above the flow lane (a grab there switched AUTO / MANUAL); a 50 % acceleration strip squeezed the CVD to 37 px. Now: the newest candle stayed in view in every stretch, squeeze and wheel and in 1,000 live trades; the CVD keeps 70 px; 0 of 2,316 bars are cut at rest. A lane paints only inside its own band and its zero line stays in the middle.</div>
</section>

<section id="settings">
  <h2>7b · 2.3: the flow lane reads the market, not only the delta</h2>
  <p class="lead">The flow lane measured one number: the average delta of the last 7 bars. Delta size cannot tell a busy, one-sided market (a real push) from a quiet one (a thin move) or from a two-sided fight (a battle). 2.3 adds three dials to every bar, computed once on the data thread and only read by the picture, and wires them into the bars, the THRUST marks and the card.</p>
  <table>
  <tr><th>Dial</th><th>What it is</th><th>Where it shows</th></tr>
  <tr><td><b>One-sided</b></td><td>(buy − sell) ÷ (buy + sell) over the fast window: Quantower's own Delta %. Ranked against the last 60 bars.</td><td>compass needle direction; the bars' brightness</td></tr>
  <tr><td><b>Busy</b></td><td>volume per second over the fast window (a range bar's time is read from its open to the next bar's open). Ranked against the last 60 bars.</td><td>compass needle length; the bars' brightness; the THRUST gate</td></tr>
  <tr><td><b>Absorbed</b></td><td>the bar's own effort-vs-result reading (the verdict's ABSORBED / TRAPPED rule), kept on the bar</td><td>an amber outline on the bar; the THRUST gate; the compass warning</td></tr>
  </table>
  <h3>What changed on the chart</h3>
  <ul>
    <li><b>Bar brightness = real push</b> (√(one-sided rank × busy rank)): bright = one-sided AND busy; dark = thin or battle. The tier still picks the colour step and the cap. Setting "2.3 Flow bars brightness" (Classic = the 2.2 tiers).</li>
    <li><b>THRUST only when it is real</b>: busy (rank ≥ 50 %), one-sided the same way as the flow, one-sidedness not falling, not absorbed. Otherwise a grey hollow chevron and the words "thin thrust". Setting "2.3 THRUST only when…". Same gate in the strip, the lane pill and the card.</li>
    <li><b>Early fade</b> (amber dot at half the bar): flow still strong (rank ≥ 80 %) but its one-sidedness over 3 bars fell under 60 % of the 7-bar one, or changed side. The push is dying before STALL fires. Setting "2.3 Early fade dot".</li>
    <li><b>Acceleration inside the flow lane</b> when the strip is off: a slowing bar is hollow, THRUST / STALL / flip marks sit at the bar's end, the strip's words move into the flow pill. Setting "2.3 Acceleration inside the flow lane". With the strip off the CVD pane gets its room.</li>
    <li><b>Flow lane looks</b>: Bars (2.2), Polarized area, Fast vs slow (histogram of fast − slow, bright while the gap grows, a diamond at every cross), Ribbon (the fast / slow band), Needles. Same numbers, same marks. Setting "2.3 Flow lane look".</li>
    <li><b>The compass in the card</b> replaces the three flow text rows: needle direction = who (buyers right, sellers left, full deflection at 60 % one-sided), needle length = how busy (rings at 50 / 80 %), a gold arrow at the tip = the acceleration (outward speeding up, inward slowing), three lines beside it, and a pill: REAL PUSH · THIN ⚠ · BATTLE ⚠ · ABSORBED ⚠ · FADING. Setting "2.3 Card: flow compass".</li>
  </ul>
  <h3>Fixed in 2.3</h3>
  <ul>
    <li><b>"Show signals" did nothing</b>: a signal's filter compares its score with the average of the same kind in the last 60 bars, and with fewer than 3 such signals (the usual case) there was no average, so every setting showed every signal. Now the average of all the signals of that kind seen on the chart is the reference when the lookback has fewer than 3. Still frozen with the signal (no repaint). Setting "2.3 Signal filter: all-history average" (off = exactly 2.2).</li>
    <li>A changed setting redraws the chart at once (a quiet market kept the old picture until the next tick).</li>
    <li>"Velocity: bar style = Gradient" reached only the acceleration strip; now the flow lane too.</li>
    <li>The dashed swing line and the dashed forming-bar markers used a shared pen and set it back afterwards; an exception in between left the pen dashed. They have their own pens now.</li>
    <li>The Custom theme's CVD line was orange; it is the "Velocity: CVD line on top" colour.</li>
    <li>"Info table: Top right" is offered. "Swing divergence: show" hides existing swing markers at once. A picture with no bars left stale hover boxes.</li>
    <li>The flow tiers are ranked once per bar when it is computed, not for every visible bar in every picture (the single largest per-bar cost of a picture is gone).</li>
    <li>The indicator cleans up (mouse hooks, timer, fonts, pens) when the platform disposes it, not only on OnClear.</li>
  </ul>
  <h2>8 · The settings</h2>
  <h3>The nine 2.0 settings</h3>
  <div class="card scroll"><table>
  <tr><th>Setting</th><th>2.0 (default)</th><th>Classic</th><th>What it changes</th></tr>
  <tr><td>2.0 Info table style</td><td>Card</td><td>Classic</td><td>The new card (title, groups, last-bar rows) or the 1.x table.</td></tr>
  <tr><td>2.0 Signals: keep the first marker (no repaint)</td><td>on</td><td>off</td><td>Markers never move or vanish; a repeat grows a tail.</td></tr>
  <tr><td>2.0 Swing divergence: marker on the bar that confirmed it</td><td>on</td><td>off</td><td>The swing diamond where it was known, with a line to the swing.</td></tr>
  <tr><td>2.0 Flow speed tiers</td><td>By rank</td><td>Fixed multiples</td><td>The flow-speed words and lane colours.</td></tr>
  <tr><td>2.0 Table: last bar speed and reading</td><td>on</td><td>off</td><td>The two last-bar rows on range and tick charts.</td></tr>
  <tr><td>2.0 Signal card when the mouse points at a marker</td><td>on</td><td>off</td><td>The signal card.</td></tr>
  <tr><td>2.0 Markers style</td><td>Clean</td><td>Classic</td><td>The marker shapes on a dark halo, size by strength.</td></tr>
  <tr><td>2.0 Reset every N bars: count from each session start</td><td>on</td><td>off</td><td>Only with Reset = every N bars: the N bars count from each session's first bar, so the resets do not move when the chart loads more or fewer bars.</td></tr>
  <tr><td>2.0 Crisp candles</td><td>on</td><td>off</td><td>Candles and lane bars on whole pixels: sharper and faster.</td></tr>
  </table></div>
  <h3>The 2.1 settings (the look only)</h3>
  <div class="card scroll"><table>
  <tr><th>Setting</th><th>Default</th><th>What it changes</th></tr>
  <tr><td>2.1 Card place · move right · move down · width · size · background · rows</td><td>Auto (top left) · 0 · 0 · fits its text · 90 % · 94 % · Full</td><td>where the card sits, how big, how black, how many rows (section 3)</td></tr>
  <tr><td>2.1 Flow speed and acceleration lanes style</td><td>Pro</td><td>the Pro lanes, or the 1.x look (Classic)</td></tr>
  <tr><td>2.0 Markers style</td><td>Pro badges</td><td>badges (2.1), Clean (2.0) or Classic (1.x)</td></tr>
  </table></div>
  <h3>The main settings (the defaults are what was tested)</h3>
  <div class="card scroll"><table>
  <tr><th>Setting</th><th>Default</th><th>Note</th></tr>
  <tr><td>Reset</td><td>Session</td><td>CVD back to 0 at each session. "Every week" starts Monday 00:00 chart time (CME's week starts Sunday evening).</td></tr>
  <tr><td>Closed bars only for SIGNALS</td><td>on</td><td>Recommended on range bars: a signal is read when its bar is finished.</td></tr>
  <tr><td>Show signals</td><td>Above lookback average</td><td>All / Above lookback average / Extreme.</td></tr>
  <tr><td>Lookback (bars)</td><td>60</td><td>Typical sizes, extremes, the signal average, the flow rank, the typical bar time.</td></tr>
  <tr><td>Velocity window</td><td>3 bars</td><td>For the price and delta speed (R, E). E reads about 12 % high at 3 bars because order flow has memory.</td></tr>
  <tr><td>Absorption</td><td>2.5× / 0.25×</td><td>Delta at least 2.5× typical, progress at most 0.25× typical.</td></tr>
  <tr><td>Exhaustion</td><td>70 %</td><td>Push at most 70 % of the previous push.</td></tr>
  <tr><td>Divergence</td><td>1.0× · 2 bars</td><td>Both speeds at least 1×, 2 bars in a row.</td></tr>
  <tr><td>Swing bars</td><td>3</td><td>Bars on each side of a swing.</td></tr>
  <tr><td>Minimum bars between same-type signals</td><td>3</td><td>Within 3 bars the same kind and side is the same event.</td></tr>
  </table></div>
</section>

<section id="new">
  <h2>9 · What is new in 2.0, and why</h2>
  <div class="card scroll"><table>
  <tr><th>What was wrong in 1.x</th><th>What 2.0 does</th><th>Shown by</th></tr>
  <tr><td>On range bars a full-range bar on heavy buying read "BUYERS being ABSORBED"</td><td>Finished range bars read by effort per range (section 6)</td><td>781 finished range bars read again by the test's own code</td></tr>
  <tr><td>Markers disappeared or moved after you saw them (a stronger reading within 3 bars deleted them)</td><td>The first marker stays; a repeat grows a tail; the filter is answered once</td><td>On range and tick markets: 1.x 2 markers vanished, 2.0 none; history after a reload = what was drawn live</td></tr>
  <tr><td>The swing diamond was drawn 3 bars before it was known</td><td>On the confirming bar, linked to the swing</td><td>48 swing divergences checked on screen</td></tr>
  <tr><td>"Every N bars" reset moved when the chart loaded a different number of bars</td><td>Counted from each session's first bar</td><td>The same market loaded 10 hours later: 1.x every bar different, 2.0 every bar equal</td></tr>
  <tr><td>A bar whose volume data arrived late kept a wrong delta until a reload</td><td>The bar that just closed is read again for 3 updates and, if its data changed, read again exactly as a reload would</td><td>1,403 bars first read with half their trades missing: every CVD, every signal and every marker equal to a reload</td></tr>
  <tr><td>Two different pens could share a cache key (wrong colour or width)</td><td>Key = colour and exact width</td><td>Unit check</td></tr>
  <tr><td>Flow words understated (STRONG never showed)</td><td>Words by rank, the % shown</td><td>Section 7</td></tr>
  <tr><td>—</td><td>Bar speed and last-bar reading on range and tick charts; the signal card; the new card and markers; crisp candles</td><td>Pictures in this guide</td></tr>
  <tr><td>The table could not be placed or sized; lanes and markers hard to read at a glance</td><td><b>2.1</b>: the card placed, moved, sized, widened, darkened, in plain words, with the bar that is forming; the Pro lanes and badges</td><td>Sections 3, 4, 7</td></tr>
  </table></div>
  {img("repaint-sequence.png", "1.x moves the marker, 2.0 keeps it", f"The same flow divergence (it lasted {rs.get('lasted', 2)} bars) after each bar closed. <b>1.x</b>: the marker moves to the newest, stronger bar (the dashed line is where it began). <b>2.0</b>: the marker stays where it began and grows a tail.") if "repaint-sequence.png" in META else ""}
  {img("swing-pair.png", "Swing divergence 1.x and 2.0", "A swing divergence. Left, 1.x: the diamond sits on the swing, 3 bars before anyone could know it. Right, 2.0: on the bar that confirmed it, with a thin line back to the swing.")}
  <h3>Faster, lighter</h3>
  <div class="card scroll"><table>
  <tr><th>Measured on the test machine (original and 2.1 painted alternately)</th><th>1.x</th><th>2.1</th></tr>
  <tr><td>One picture, Candles mode</td><td class="n">1.00×</td><td class="n">0.94–0.99×</td></tr>
  <tr><td>One picture, Velocity mode (the new lanes, card and badges included)</td><td class="n">1.00×</td><td class="n">0.65–0.67× (a third faster)</td></tr>
  <tr><td>Memory made per picture (Candles / Velocity)</td><td class="n">1.6 / 2.5 KB</td><td class="n">1.3 / 2.3 KB</td></tr>
  <tr><td>Added and removed 50 times</td><td colspan="2">no memory, thread or drawing object left behind</td></tr>
  </table></div>
</section>

<section id="routine">
  <h2>10 · What matters most, and the routine</h2>
  <h3>What matters most (in the research's order)</h3>
  <div class="card scroll"><table>
  <tr><th>#</th><th>What</th><th>Why</th><th>In CVD PRO</th></tr>
  <tr><td>1</td><td><b>The push and the move together</b></td><td>Order-flow imbalance explains a large part of short price moves; trade imbalance alone about a third at 10 seconds (Cont, Kukanov &amp; Stoikov 2011).</td><td>Pushing vs Price move, the verdict, the last-bar reading</td></tr>
  <tr><td>2</td><td><b>Flow is persistent</b></td><td>Order signs have long memory (Lillo &amp; Farmer 2004; Tóth et al. 2011): CVD trends even without news, so a rising CVD alone is not a forecast — it says who is in control.</td><td>CVD, In control, Right now, flow speed</td></tr>
  <tr><td>3</td><td><b>A push that dries up at an extreme</b></td><td>Imbalance peaks at the extreme, part of the move comes back (Mu et al. 2010).</td><td>Exhaustion</td></tr>
  <tr><td>4</td><td><b>Hidden liquidity</b></td><td>Iceberg orders are real and market orders hitting them move price less (Frey &amp; Sandås 2017; Pardo &amp; Pascual 2011).</td><td>Absorption, "Absorbed by", TRAPPED</td></tr>
  <tr><td>5</td><td><b>Speed</b></td><td>The time to cross a fixed distance measures volatility (Andersen, Dobrev &amp; Schaumburg 2008).</td><td>Last bar speed</td></tr>
  </table></div>
  <h3>The routine, every time</h3>
  <ol class="steps">
    <li><b>Look at the card's title.</b> The right chart type and <b>LIVE</b>. LOADING = wait.</li>
    <li><b>Who is in control, and is it turning?</b> In control vs Right now; <b>← TURNING</b>.</li>
    <li><b>Wait for a marker at a level</b>: a session high or low, a former swing, a round number.</li>
    <li><b>Ask for a second thing that agrees</b>: the last-bar reading (SELLERS TRAPPED after a long absorption; heavy buying broke through = continuation, not a fade), Right now turning, or a second marker of the same side.</li>
    <li><b>Enter</b> on a break of the signal bar; <b>stop</b> 2 ticks beyond the defended price; <b>target</b> the next level of the other side, at least twice the risk.</li>
    <li><b>Skip</b> when: LOADING; several "thin book" bars in a row (fragile); EXTREME flow against you; range bars 3× faster than typical right after news.</li>
  </ol>
</section>

<section id="limits">
  <h2>11 · Limits — read this once</h2>
  <div class="card warn"><b>The signal rules are not tested on real data.</b> Absorption, exhaustion and both divergences are written down as hypotheses (H-0006 … H-0009). The research above supports the ideas in part; it does not prove the rules. A measurement on ES range and tick bars is planned (an optional step that needs your word on the data). Until then: information, not a system.</div>
  <div class="card scroll"><table>
  <tr><th>Limit</th><th>What to know</th></tr>
  <tr><td>E (delta speed)</td><td>Reads about 12 % high at 3 bars, because order flow has memory.</td></tr>
  <tr><td>"Every week" reset</td><td>Starts Monday 00:00 chart time; CME's week starts Sunday evening.</td></tr>
  <tr><td>Velocity card in a short window</td><td>Covers the right end of the lanes (section 7).</td></tr>
  <tr><td>Late volume data</td><td>The bar that just closed is read again for 3 updates. Data that arrives even later is fixed by a reload, as in 1.x.</td></tr>
  <tr><td>Two quick looks only you can do</td><td>(1) On one bar, the CVD candle's wicks should match the footprint's max / min delta. (2) Note a closed bar's CVD, reload the chart, and check it is the same.</td></tr>
  </table></div>
</section>

<section id="proof">
  <h2>Check it yourself</h2>
  <p class="lead">Everything below runs on the test machine in <span class="path">QUANTOWER\\TNX-TEST\\Harness</span> (<code>dotnet run -c Release -- &lt;command&gt;</code>).</p>
  <div class="card scroll"><table>
  <tr><th>Test</th><th>Command</th><th>Result</th></tr>
  <tr><td>2.0 with every 2.0 setting at classic = the original, on 1-minute, range and tick markets</td><td><code>run CVD</code></td><td>PASS · 1,133,748 values and 1,119,360,000 pixels identical</td></tr>
  <tr><td>The same with every choice of every setting</td><td><code>run-full CVD</code></td><td>PASS · 4,723,950 values and 4,664,000,000 pixels identical</td></tr>
  <tr><td>2.0 against itself on every market and setting (no error, stable)</td><td><code>run-self CVD full --new</code></td><td>PASS · no error caught</td></tr>
  <tr><td>The formulas recomputed by the test's own code; each 2.0 fix shown failing on the old code</td><td><code>cvd-checks</code></td><td>78,218 pass, 0 fail</td></tr>
  <tr><td>The picture reads only its own copy of the data</td><td><code>paint-isolation [--new]</code></td><td>PASS · 468 / 468 (classic and 2.0)</td></tr>
  <tr><td>Speed and memory</td><td><code>ab CVD --new</code>, <code>profile CVD --new</code>, <code>cvd-timing</code></td><td>faster, less memory (section 9)</td></tr>
  <tr><td>The program, instruction by instruction: only the 2.0 methods changed</td><td><code>python il_compare.py</code></td><td>the installed DLL vs 2.0</td></tr>
  <tr><td>Same assembly name and version; settings only added, none renamed</td><td><code>dll-compare</code></td><td>saved charts keep working</td></tr>
  <tr><td>These pictures</td><td><code>cvd-guide</code> then <code>make_guide.py</code></td><td>drawn by CVD PRO's own code</td></tr>
  </table></div>
</section>

</div>

<footer><div class="wrap">TNX CVD PRO 2.3 · built 2026-10-11 · file <span class="path">QUANTOWER\\TNX-CVD-PRO\\NEW-LATEST\\TNXCVDPro.dll</span> · SHA-256 <code>{dll_sha}</code> · not installed until you say so.</div></footer>
</body>
</html>
"""
    OUT.write_text(page, encoding="utf-8", newline="\n")
    print(f"{OUT} ({OUT.stat().st_size // 1024} KB)")


build()
