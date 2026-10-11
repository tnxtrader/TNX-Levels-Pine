# TNX FLOW PRO 1.0 (2026-10-11) — TNX CVD PRO 2.2 + the 2.3 changes, under a new name

New class `TNXFlowPro`, new DLL `TNXFlowPro.dll`, listed as "TNX FLOW PRO": it installs next to your old TNX CVD PRO without touching it.

Every new behaviour has its own setting (sort index 87–93). With "2.3 Flow bars brightness = Classic", "THRUST only
when…" off, "Early fade dot" off, "Acceleration inside the flow lane" off, "Card: flow compass" off and "Signal filter:
all-history average" off, the picture is the 2.2 picture; the fixes below apply either way.

## Why

The flow lane measured one number, the average delta of the last 7 bars, and the acceleration measured how that number
changed. Delta size cannot tell a busy, one-sided market (a real push) from a quiet one (a thin move) or from a
two-sided fight (a battle). The three test situations (a real push, an absorption at the high, a thin fast move) showed
the lane calling all three "strong" and firing THRUST into the thin one.

## New per-bar dials (computed once per bar on the data thread; the picture only reads them)

- **Dur** – seconds the bar took (a finished bar: its open to the next bar's open; the forming bar: open to now).
- **Imb7 / Imb3** – one-sidedness (buy − sell) ÷ (buy + sell) over the fast window and over 3 bars (Quantower's Delta %).
- **Vps** – volume per second over the fast window. **BusyRank** – its rank in the lookback.
- **ImbRank** – rank of |Imb7|. **FRank / SRank** – the flow tiers' ranks, stored (were computed per bar per picture).
- **Q** – real push 0..1 = one-sided rank × gate(busy rank): nothing is bright under a busy rank of 20 %, from 70 % up the
  one-sidedness alone decides.
- **Absorbed** – the bar's own effort-vs-result reading (the verdict's ABSORBED / TRAPPED rule).
- **Fade** – flow rank ≥ 80 % but its 3-bar one-sidedness fell under 60 % of the 7-bar one, or changed side.
- **ThrustOk** – busy rank ≥ 50 %, one-sided the flow's way (≥ 10 %), one-sidedness not falling, not absorbed.

## On the chart

- Flow bars: brightness = Q (bright = one-sided AND busy; dark = thin or battle). Amber outline on an absorbed bar.
- THRUST: the chevron is solid only when ThrustOk; otherwise a grey hollow chevron and the words "thin thrust" (strip,
  flow pill and card agree).
- Early fade: an amber dot at half the bar.
- Acceleration inside the flow lane when the strip is off: a slowing bar is hollow, THRUST / STALL / flip marks at the
  bar's end, the strip's words in the flow pill. The CVD pane gets the room.
- Flow lane looks: Bars (2.2), Polarized area, Fast vs slow (histogram of fast − slow with both lines and a diamond at
  every cross), Ribbon (the fast / slow band), Needles.
- Card compass: needle direction = who (buyers right, sellers left, full deflection at 60 % one-sided), needle length =
  busy rank (rings at 50 / 80 %), gold arrow at the tip = acceleration (outward speeding up, inward slowing), three lines
  beside it and a pill: REAL PUSH · THIN ⚠ · BATTLE ⚠ · ABSORBED ⚠ · FADING · normal.

## Fixed

1. **"Show signals" (All / Above average / Extreme) did nothing.** The filter compares a signal's score with the average
   of the same kind in the last 60 bars; with fewer than 3 such signals (almost always, since repeats are merged) there was
   no average, so every setting showed every signal. 2.3 uses the all-history average of the kind when the lookback has
   fewer than 3. Still frozen with the signal (no repaint); the F6 live re-read snapshot carries the averages too.
2. A changed setting redraws at once (OnSettingsUpdated never asked for a redraw; a quiet market kept the old picture).
3. "Velocity: bar style = Gradient (lit top)" reached only the acceleration strip (HBarTier had no Gradient branch).
4. The dashed swing line and the dashed forming-bar markers set DashStyle on shared cached pens and set it back; an
   exception in between left the pen dashed for every later use. They have their own dashed pens now.
5. The Custom theme had no SignalColor arm: the Velocity CVD line was orange. It is the "CVD line on top" colour.
6. "Info table: Top right" was tested in code but not offered in the dialog.
7. "Swing divergence: show" was an evaluation gate only; existing swing markers stayed until a reload. It is a paint gate too.
8. A picture that could not map bars (XMap) left stale hover boxes; they are cleared.
9. RangeFull allocated an array per forming bar on the data thread; it reuses a buffer.
10. The indicator cleans up (mouse hooks, timer, fonts, pens, brushes) in Dispose, not only in OnClear.
11. Performance: the flow tiers' ranks were computed for every visible bar in every picture (O(lookback) each); they are
    computed once per bar now. The test run: 257 range bars replayed + 5 live ticks in 2 ms; a full Velocity picture in
    about 8 ms on Mono/libgdiplus.

## Not changed

Every formula of 2.2 (CVD, Fast/Slow, AccE, R/E, the signals, the swing divergences, the card's classic rows), every
existing setting's meaning, the pane mechanics (drag, zoom, AUTO/MANUAL), the markers, the hover cards.

## Known limits

- Duplicate sort indexes (13 twice, 28 three times) were left as they were so the dialog order you know does not move.
- The data thread's view of "now" (the forming bar's duration) is the platform's server-synchronised UTC clock; during a
  replay it is the replay clock only if the platform reports it as such.
