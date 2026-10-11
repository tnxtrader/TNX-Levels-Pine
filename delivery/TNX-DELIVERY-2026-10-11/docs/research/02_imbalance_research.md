# 02 — Order-flow imbalance for a trade-based bar indicator (Quantower)

Written 2026-10-11 for the TNX CVD PRO 2.3 / TNX Imbalance Strength work. Scope: what can be measured from
per-bar aggressor volume (plus the footprint) on range, tick and time bars; what each measure needs, what it tells,
where it breaks, and how to normalise it.

## 0. Status legend and what was actually readable

Every factual claim carries one of:

- **VERIFIED** — I read the text in this session: an official Quantower example (`Quantower/Examples`,
  `Quantower/Scripts` on GitHub) or the user's own files.
- **SNIPPET** — an official or primary page was found by the search engine and I quote its snippet, but the page
  itself could not be opened. Treat the wording as reliable, the surrounding context as unchecked.
- **INFERRED** — deduced from the user's source, which compiles against TradingPlatform.BusinessLayer 1.146.7.0
  (`/tmp/claude-0/-home-user-TNX-Levels-Pine/8e550d63-3cbb-5fdc-bfc6-5a88d0326c31/scratchpad/upload/TNX-CVD-PRO/src/TNXIndicators/TNXCVDPro.cs`),
  or from `research/api_surface_from_dll.txt` (member references in the compiled DLL).
- **UNVERIFIED** — stated from memory of a paper, or taken from a third-party page; not confirmed here.

Reachability in this session (tested, do not re-test): `api.quantower.com`, `help.quantower.com`,
`www.quantower.com`, `sierrachart.com`, `help.atas.net`, `learn.atas.net`, `jigsawtrading.com`, `luxalgo.com`,
`arxiv.org`, `ar5iv`, `ssrn.com`, `ideas.repec.org`, `semanticscholar.org`, `stern.nyu.edu` all failed DNS
(`ENOTFOUND`) in the WebFetch tool and returned no HTTP status from the container. `github.com` worked in WebFetch;
`raw.githubusercontent.com` worked from the container; the GitHub MCP code search worked across public repos.
So: **no api.quantower.com page was read**; every API statement below rests on official example code, the user's
compiling source, the DLL reference dump, or a third-party decompiled mirror (marked as such).

Files read in full or in the relevant part (all VERIFIED as sources):

- `Quantower/Examples/IndicatorExamples/DrawValueAreaForEachBarIndicator.cs`
  (https://raw.githubusercontent.com/Quantower/Examples/master/IndicatorExamples/DrawValueAreaForEachBarIndicator.cs)
- `Quantower/Scripts/Indicators/IndicatorCumulativeDelta.cs`, `IndicatorDeltaFlow.cs`, `IndicatorDeltaDots.cs`,
  `IndicatorSpeedOfTape.cs`, `IndicatorTimeHistogram.cs`
  (https://raw.githubusercontent.com/Quantower/Scripts/090f9a85763d5bafce272e2d5803e0247b389183/Indicators/<name>.cs;
  copies in `research/official_examples/`).
- Third-party decompiled mirror of the DLL: `PickReaper/quantower` (`VolumeAnalysisItem.cs`, `VolumeAnalysisData.cs`,
  `HistoryItemBar.cs`, `HistoryItem.cs`, `IVolumeAnalysisIndicator.cs`, `VolumeAnalysisField.cs`; copies in
  `research/decompiled_mirror/`). DLL version of that mirror is unknown; it is UNVERIFIED against 1.146.7.0.

## 1. The inputs the API gives per bar

| Input | Member | Status and source |
|---|---|---|
| Buy (aggressor) volume, sell volume, delta, total volume | `VolumeAnalysisItem.BuyVolume`, `.SellVolume`, `.Delta`, `.Volume` on `GetVolumeAnalysisData(offset)?.Total` | INFERRED: the user's `Compute()` reads exactly these (lines ~1706–1715); the DLL dump lists `get_BuyVolume, get_Delta, get_MaxDelta, get_MinDelta, get_SellVolume, get_Volume` on `VolumeAnalysisItem` and `get_Total` on `VolumeAnalysisData`. VERIFIED usage in official `IndicatorCumulativeDelta.cs` (`total.Delta`, `total.MaxDelta`, `total.MinDelta`). |
| Max / min cumulative delta inside the bar | `.MaxDelta`, `.MinDelta` | VERIFIED sentinel handling in official `IndicatorCumulativeDelta.cs` lines 1053–1059: `!double.IsNaN(total.MaxDelta) && total.MaxDelta != double.MinValue` and `MinDelta != double.MaxValue`, i.e. unset values are `double.MinValue` / `double.MaxValue`, not 0. The user's `Safe(...)` wrapper (INFERRED) handles NaN; check it also handles these sentinels. |
| Trades count (and buy/sell trade counts) | `.Trades` (int); enum `VolumeAnalysisField.Trades/BuyTrades/SellTrades` through `Total.GetValue(field)` | VERIFIED: official `IndicatorDeltaDots.cs` line 131 `tradesCount += volumeData.Total.Trades`; official `IndicatorSpeedOfTape.cs` and `IndicatorTimeHistogram.cs` list `VolumeAnalysisField.Trades, BuyTrades, SellTrades, Volume, BuyVolume, SellVolume, BuyVolumePercent, SellVolumePercent, Delta, DeltaPercent, CumulativeDelta, OpenInterest, AverageSize, AverageBuySize, AverageSellSize, MaxOneTradeVolume, MaxOneTradeVolumePercent`. Whether `Trades` is populated for every feed/history type: UNVERIFIED. The user's current DLL does not reference it. |
| Per-price-level buy/sell volume (footprint) | `VolumeAnalysisData.PriceLevels : Dictionary<double, VolumeAnalysisItem>` (key = price) | VERIFIED: official `DrawValueAreaForEachBarIndicator.cs` (`CalculateValueArea(bar.VolumeAnalysisData.PriceLevels, ...)`, sorted with `OrderBy(it => it.Price)`) and `IndicatorDeltaDots.cs` (`foreach (var item in currentData.VolumeAnalysisData.PriceLevels)`). It is a `Dictionary`, so **sort the keys before any diagonal comparison**. Requires `IsRequirePriceLevelsCalculation => true` (VERIFIED: DrawValueArea returns true and uses PriceLevels; CumulativeDelta returns false and uses only `Total`). The user's indicator returns `false` (line 1069, INFERRED), so a footprint-based indicator must return `true` and pay the heavier calculation. |
| Bar OHLC | `HistoryItemBar.Open/High/Low/Close`; in an `Indicator`, `GetPrice(PriceType.Open|High|Low|Close, offset)` | INFERRED (user's source uses `GetPrice(PriceType.Close, num)` etc.); `HistoryItemBar` members listed in the decompiled mirror (UNVERIFIED). |
| Bar open time, bar duration | `HistoryItem.TimeLeft`, `TicksLeft`, `TicksRight`; `Indicator.Time(offset)` | VERIFIED: official `IndicatorSpeedOfTape.cs` lines 66–72 computes seconds as `aggregationTime.Period.Duration.TotalSeconds` for time charts and `new TimeSpan(item.TicksRight - item.TicksLeft).TotalSeconds` otherwise, and skips when `Aggregation is HistoryAggregationTick`. The user's code uses `Time(num).Ticks` and the next bar's open as the end (INFERRED). |
| Chart type | `HistoricalData.Aggregation is HistoryAggregationRangeBars (RangeBars)`, `HistoryAggregationTickBars (TicksCount)`, `HistoryAggregationTime (Period)`, `HistoryAggregationRenko (BrickSize)` | INFERRED: user's source lines 1395–1425 and the DLL dump. `HistoryAggregationTick` (tick chart, one tick per item) also exists: VERIFIED in `IndicatorSpeedOfTape.cs`. |
| Data readiness | `HistoricalData.VolumeAnalysisCalculationProgress.State == VolumeAnalysisCalculationState.Finished`; `ProgressChanged` event with `VolumeAnalysisTaskEventArgs.CalculationState`; `IVolumeAnalysisIndicator.VolumeAnalysisData_Loaded()` | VERIFIED in official `IndicatorCumulativeDelta.cs` (636–638), `IndicatorDeltaFlow.cs` (55–60, 130–135), `IndicatorTimeHistogram.cs` (49–52). Interface shape `bool IsRequirePriceLevelsCalculation { get; } void VolumeAnalysisData_Loaded();` VERIFIED by official usage and the mirror. |
| Who is a buyer | The mirror shows `VolumeAnalysisData.Calculate(double price, double size, AggressorFlag aggressorFlag)` | UNVERIFIED (decompiled). It implies classification by the feed's aggressor flag where present. How Quantower classifies when the feed gives no flag (tick rule, bid/ask comparison): UNVERIFIED. Quantower help defines Delta as the difference in traded volume between buyers and sellers and `Delta % = Delta / Volume * 100` (SNIPPET, https://help.quantower.com/quantower/analytics-panels/chart/volume-analysis-tools/cluster-chart). |
| Other per-bar fields in the DLL | `FilteredTotalVolume/FilteredBuyVolume/FilteredSellVolume` (+percent), `MaxOneTradeVolume`, `AverageSize/AverageBuySize/AverageSellSize`, `DeltaFinish => Delta - (|MinDelta|>|MaxDelta| ? MinDelta : MaxDelta)`, `CumulativeDelta` | UNVERIFIED (decompiled mirror only; the `VolumeAnalysisField` enum in the official `IndicatorTimeHistogram.cs` confirms the names `AverageSize`, `AverageBuySize`, `AverageSellSize`, `MaxOneTradeVolume`, `OpenInterest`). Cross-check against api.quantower.com before relying on `DeltaFinish` or the `Filtered*` fields. |

Range-bar construction in Quantower (SNIPPET, https://help.quantower.com/quantower/analytics-panels/chart/chart-types/range-bars):
"Each range bar must have a high/low range that equals the specified range. Every new range bar must open outside
the high/low range of the previous bar. Each range bar must close at either its high or its low." Range size is set in
ticks (price increments). Consequence used below: on a finished range bar `High − Low = R` exactly and the close is
an extreme; the only free quantities are the sign, the time taken, the trades, the volume, the delta path and the
footprint.

One official convention worth knowing: `IndicatorDeltaFlow.cs` (VERIFIED, line 111) plots
`Total.Delta * (High − Low)` as its "delta flow" value, i.e. effort × result, with no normalisation. On range bars
that is delta times a constant.

## 2. The measures

Notation per bar i: `B_i` buy volume, `S_i` sell volume, `V_i = B_i + S_i`, `D_i = B_i − S_i` (delta),
`D⁺_i = MaxDelta`, `D⁻_i = MinDelta` (intrabar cumulative-delta extremes, `D⁻ ≤ 0 ≤ D⁺` once clamped as the user does),
`N_i` trades, `τ_i` duration in seconds, `O_i,H_i,L_i,C_i` prices, `Δp_i = C_i − C_{i−1}` (or `C_i − O_i`),
`tick` the price increment, `R` the range-bar height. Footprint: `A(p)` = volume traded at the ask (buy) at price p,
`Bd(p)` = volume at the bid (sell) at price p.

### a) Trade imbalance and VPIN

**Formulas.**
- Signed bar trade imbalance: `TI_i = D_i / V_i ∈ [−1, 1]` (this is Quantower's `DeltaPercent/100`, SNIPPET above).
- VPIN (Easley, López de Prado, O'Hara 2012): trades are grouped into buckets of equal volume `V`; for bucket τ,
  `VPIN = (1/n) · Σ_{τ=1..n} |V^S_τ − V^B_τ| / V`, i.e. the mean absolute bucket imbalance over the last n buckets,
  bounded in [0, 1]. Buy/sell split without an aggressor flag by bulk volume classification (BVC):
  `V^B_τ = V_τ · Z((P_τ − P_{τ−1}) / σ_ΔP)`, `V^S_τ = V_τ − V^B_τ`, `Z` the standard normal CDF.
  The paper's stated choices (V = 1/50 of average daily volume, n = 50): UNVERIFIED here (not readable).
  The formula shape is VERIFIED only at the level of the published abstract ("estimate flow toxicity based on
  volume imbalance and trade intensity", "updated in volume time", "requires trades classified as buys or sells",
  "new bulk volume classification procedure") — SNIPPET from https://papers.ssrn.com/abstract=1695596 and
  https://ideas.repec.org/a/oup/rfinst/v25y2012i5p1457-1493.html.

**Needs.** Signed volume per bar. Nothing else. For VPIN proper, equal-volume buckets; on a bar chart use the
volume-weighted version `VPIN_bars(n) = Σ_{k=i−n+1..i} |D_k| / Σ V_k`, which is exact VPIN when bars are volume bars
and a volume-weighted mean of |TI| otherwise.

**Tells.** `TI` is direction and one-sidedness of the aggressive flow in the bar; VPIN is unsigned "toxicity": how
one-sided the flow has been recently, independent of direction. Easley et al. read VPIN through its own empirical
CDF (percentile); a secondary source states VPIN "exceeded 90% threshold value near 11:55 AM" on 6 May 2010
(SNIPPET via https://www.cxoadvisory.com/volatility-effects/measuring-and-interpreting-market-information-pulse,
citing Easley, López de Prado, O'Hara, "The Microstructure of the 'Flash Crash'", J. Portfolio Management 37(2),
118–128, 2011, https://papers.ssrn.com/abstract=1695041). The exact "CDF ≥ 0.9" wording in the papers: UNVERIFIED.
The user's guide and the `FlowRank` percentile tiers rest on this reading; the reading is right in spirit
(VPIN is used as a percentile), the specific number should not be attributed to the paper without checking.

**Critique to carry.** Andersen & Bondarenko, "VPIN and the Flash Crash", J. Financial Markets, 1–46, 2014
(volume 17 from memory, UNVERIFIED)
(SNIPPET, https://www.kellogg.northwestern.edu/academics-research/research/detail/2014/vpin-and-the-flash-crash/):
on E-mini S&P 500, BVC classifies worse than a plain tick rule against a quote-based benchmark, and VPIN's
predictive content for short-run volatility "is due primarily to a mechanical relation with the underlying trading
intensity". Reply: Easley et al., "VPIN and the Flash Crash: A Comment", https://papers.ssrn.com/abstract=2062450.
Easley, López de Prado, O'Hara, "Discerning information from trade data", J. Financial Economics 120(2), 269–285,
2016 (SNIPPET, https://ideas.repec.org/a/eee/jfinec/v120y2016i2p269-285.html): tick rules and BVC are both
"relatively good classifiers of the aggressor side", BVC "better linked to proxies of information-based trading".
Implication for us: we have the aggressor flag from the feed, so BVC is not needed; and any "toxicity" number must
be shown next to trade intensity, because part of it is intensity.

### b) Order Flow Imbalance (OFI) and its trade-based proxy

**Paper.** Cont, Kukanov, Stoikov, "The price impact of order book events", J. Financial Econometrics 12(1), 47–88,
2014 (arXiv 1011.6402). Bibliographic data and the following claims VERIFIED at abstract level (SNIPPET,
https://ideas.repec.org/a/oup/jfinec/v12y2014i1p47-88..html and https://hal.sorbonne-universite.fr/hal-00937055):
"order flow imbalance ... the imbalance between supply and demand at the best bid and ask prices"; a linear relation
between OFI and price change "with a slope inversely proportional to the market depth"; robust to intraday
seasonality, stable across time scales and stocks; the linear model plus a scaling argument "implies the empirically
observed 'square-root' relation between the magnitude of price moves and trading volume, but this relation is found
to be noisy and less robust than the one based on OFI".

**Formula (UNVERIFIED text — reproduced from memory of the paper, the page was unreachable).** For the n-th
best-quote update with bid price/size `(P^b_n, q^b_n)` and ask `(P^a_n, q^a_n)`:

```
e_n = 1{P^b_n ≥ P^b_{n−1}} q^b_n − 1{P^b_n ≤ P^b_{n−1}} q^b_{n−1}
    − 1{P^a_n ≤ P^a_{n−1}} q^a_n + 1{P^a_n ≥ P^a_{n−1}} q^a_{n−1}
OFI_k = Σ_{n in interval k} e_n
ΔP_k  = β · OFI_k + ε_k,   β ≈ c / AD   (AD = average depth at the best quotes)
```

The commonly quoted figures (average R² about 65% for OFI versus about 32% for trade imbalance over 10-second
intervals) are UNVERIFIED here; the user's guide already uses "about a third" for trade imbalance.

**Why it is book-based.** `e_n` counts every event that changes the queue at the best quotes: limit-order arrivals
(+), cancellations (−) and market orders (−, through the size drop). Trades are only one of the three. That is why
OFI explains more of the contemporaneous price change than trade imbalance: the price moves when the queue empties,
whether by trades or cancellations.

**Trade-based proxy.** `TI_k = Σ signed trade volume = D_k`. This is what the API gives. Evidence that the proxy is
not hopeless: Silantyev, "Order flow analysis of cryptocurrency markets", Digital Finance 1(1), 191–218, 2019
(SNIPPET, https://ideas.repec.org/a/spr/digfin/v1y2019i1d10.1007_s42521-019-00007-w.html): on BitMEX XBTUSD "trade
flow imbalance is better at explaining contemporaneous price changes than the aggregate order flow imbalance",
attributed to "lack of depth and low update arrival rates" — i.e. the ranking depends on the market; in a deep book
(ES) expect the opposite. Deeper-book extensions: Xu, Gould, Howison, "Multi-level order-flow imbalance in a limit
order book", Market Microstructure and Liquidity, DOI 10.1142/S2382626619500114, 2019 (arXiv 1907.06230; SNIPPET):
out-of-sample fit improves as more book levels are added. Cont, Cucuringu, Zhang, "Cross-impact of order flow
imbalance in equity markets", 2023 (arXiv 2112.13213; SNIPPET): an integrated multi-level OFI explains
contemporaneous impact better than best-level OFI; cross-asset OFI forecasts future returns "mainly at short
horizons and decays quickly".

**Bar form.** `β̂ = cov(Δp, D)/var(D)` over N bars (price impact per contract, in ticks); `1/β̂` is a depth proxy.
Needs variance in `Δp` (see section 3: on range bars this degenerates).

### c) Kyle's λ, price impact, Amihud illiquidity (thinness)

- Kyle, "Continuous Auctions and Insider Trading", Econometrica 53(6), 1315–1335, 1985. Pricing rule
  `p = p_0 + λ·y`, `y` = net order flow; `λ` = price impact per unit of signed flow; in the single-period model
  `λ = ½ · sqrt(Σ_0) / σ_u` (`Σ_0` prior variance of the asset value, `σ_u²` noise-trade variance). The linear rule
  and "high λ = illiquid" are SNIPPET-confirmed (https://www.quantt.co.uk/resources/kyle-model-and-kyles-lambda,
  secondary); the closed form is UNVERIFIED here (standard textbook result).
- Empirical λ ("Hasbrouck's lambda" in Goyenko, Holden, Trzcinka, "Do liquidity measures measure liquidity?",
  J. Financial Economics 92, 153–181, 2009; SNIPPET https://ideas.repec.org/a/eee/jfinec/v92y2009i2p153-181.html):
  regress the 5-minute return on the signed square root of order flow in the same 5 minutes, average over intervals.
  The square root is the concave-impact correction (Hasbrouck 1991; Lillo, Farmer, Mantegna 2003 below).
- Amihud, "Illiquidity and stock returns: cross-section and time-series effects", J. Financial Markets 5(1), 31–56,
  2002: `ILLIQ = (1/D) Σ_t |R_t| / DVOL_t`, "the daily ratio of absolute stock return to dollar volume"
  (SNIPPET, https://www.cis.upenn.edu/~mkearns/finread/amihud.pdf). Needs only price and volume.

**Bar forms.**
- Thinness per bar: `A_i = |Δp_i| / V_i` (ticks per contract). High = thin book (price moved a lot per contract).
- Impact slope over N bars: `λ̂ = Σ Δp_k·sgn(D_k)·sqrt|D_k| / Σ |D_k|` (square-root form) or the plain regression of
  `Δp` on `D`.
- The user's "effort" `E_i = D_i / mean_N|D|` with the result fixed is the inverse of these: when `Δp` is held at
  ±R (range bar), the flow needed to produce it is proportional to depth, i.e. `E_i ∝ 1/λ`. This is exactly the
  CKS/Kyle logic the user's guide already cites (make_guide.py line 425).

**Tells.** λ/Amihud: how much price a unit of aggression buys right now (thin vs thick). Low Amihud on a bar that
moved = heavy resting liquidity was consumed (the user's "heavy buying broke through"); high Amihud = thin book
("rose on little push").

### d) Hasbrouck: information content of trades

Hasbrouck, "Measuring the information content of stock trades", J. Finance 46(1), 179–207, 1991
(SNIPPET, https://ideas.repec.org/a/bla/jfinan/v46y1991i1p179-207.html): trades and quote revisions modelled as a
VAR; "a trade's information effect is measured by the ultimate price impact of the trade innovation"; the impact
"is a positive and concave function of the trade size"; the full impact arrives with a lag.

**Formula (standard form, UNVERIFIED text).**
```
r_t = Σ_{i≥1} a_i r_{t−i} + Σ_{i≥0} b_i x_{t−i} + v1_t      (quote/price revision)
x_t = Σ_{i≥1} c_i r_{t−i} + Σ_{i≥1} d_i x_{t−i} + v2_t      (signed trade)
information content = cumulative impulse response of r to a unit shock v2 (permanent part).
```

**Bar form that is cheap enough for a chart indicator.** Lagged cross-correlations `ρ_k = corr(D_{i−k}, Δp_i)` for
k = 0..K over N bars; the cumulative response `Σ_k β_k` from a short distributed-lag regression
`Δp_i = Σ_{k=0..K} β_k D_{i−k}`. `β_0` is the contemporaneous impact (Kyle); `β_{k≥1}` is whether past flow is
still being priced (permanent information) or reversed (transient, liquidity-driven). A full VAR with
order-selection is not worth it on 60 bars.

**Tells.** Whether the delta you see is "informed" (price keeps going) or "liquidity" (reverts). The concavity in
size says: use `sqrt|D|` or a log scale when comparing bars, not raw delta.

### e) Order-sign persistence (long memory)

- Lillo & Farmer, "The long memory of the efficient market", Studies in Nonlinear Dynamics & Econometrics 8(3), 2004
  (arXiv cond-mat/0311053; SNIPPET https://ideas.repec.org/a/bpj/sndecm/v8y2004i3n1.html): on the LSE, the
  autocorrelation of market-order signs decays as a power law with exponent about 0.6 (Hurst H ≈ 0.7); this does
  not create arbitrage because it is "compensated for by anti-correlated fluctuations in transaction size and
  liquidity".
- Tóth, Palit, Lillo, Farmer, "Why is equity order flow so persistent?", J. Economic Dynamics and Control 51,
  218–239, 2015 (arXiv 1108.1632, first posted 2011; SNIPPET https://ideas.repec.org/a/eee/dyncon/v51y2015icp218-239.html):
  with broker identifiers, "on timescales of less than a few hours the persistence of order flow is overwhelmingly
  due to splitting rather than herding"; sign autocorrelation extends "out to time lags of tens of thousands of
  orders".
- Theory: Lillo, Mike, Farmer, "Theory for long memory in supply and demand", Phys. Rev. E 71, 066122, 2005 (SNIPPET):
  power-law-distributed hidden order sizes, executed in pieces, give power-law sign autocorrelation.
- Bouchaud, Gefen, Potters, Wyart, "Fluctuations and response in financial markets: the subtle nature of 'random'
  price changes", Quantitative Finance 4(2), 176–190, 2004 (SNIPPET): diffusive prices come from "long-range
  correlated market orders" plus "mean-reverting limit orders"; the market sits at a critical point where the
  response is almost constant.
- Related "Tóth et al. 2011" (the other one): Tóth, Lempérière, Deremble, de Lataillade, Kockelkoren, Bouchaud,
  "Anomalous price impact and the critical nature of liquidity in financial markets", Phys. Rev. X 1, 021006, 2011
  (arXiv 1105.1694; SNIPPET): the latent supply/demand profile is V-shaped and vanishes near the current price,
  which produces the square-root impact of metaorders.
- Concave single-trade impact: Lillo, Farmer, Mantegna, "Master curve for price-impact function", Nature 421,
  129–130, 2003 (SNIPPET): impact rises "as a power that is of order 1/2 for small volumes, then increases more
  slowly for large volumes".

**Bar forms.** Lag-1 autocorrelation of `sgn(D_i)` (or of `TI_i`) over N bars; current run length of same-sign
deltas `L_i`; "expected run" from the lookback. López de Prado's tick-imbalance bars (Advances in Financial
Machine Learning, Wiley 2018, ch. 2; secondary SNIPPET via
https://mlfinpy.readthedocs.io/en/latest/FinancialDataStructure.html) sample a bar when
`|Σ_t b_t| ≥ E[T]·|2P[b=1] − 1|` with `E[T]` and `P[b=1]` tracked by EWMA — the same idea: measure the imbalance
against its own expected level, not against zero. A practitioner note (Exocharts help, SNIPPET) warns the EWMA
threshold can run away; use bounded estimates.

**Tells.** A run of same-sign deltas is the normal state of the tape (order splitting), not information by itself.
Information is in the deviation: a run longer than expected, or — the important one — a run that stops producing
price (absorption, section h/i), or a sign flip after a long run.

### f) Hawkes self-excitation of aggressive flow

- Bacry, Mastromatteo, Muzy, "Hawkes processes in finance", Market Microstructure and Liquidity 1(1), 1550005, 2015
  (arXiv 1502.04592; DOI 10.1142/S2382626615500057 SNIPPET-confirmed). Review.
- Hardiman, Bercot, Bouchaud, "Critical reflexivity in financial markets: a Hawkes process analysis", 2013
  (arXiv 1302.1405; findings SNIPPET; journal venue Eur. Phys. J. B 86:442 from memory, UNVERIFIED): E-mini S&P
  mid-price changes 1998–2011; the branching ratio
  "fluctuates about the critical value n_c = 1"; the kernel is power-law; its short-lag cut-off "has decreased
  exponentially over time" with HFT.
- Filimonov & Sornette, "Quantifying reflexivity in financial markets: towards a prediction of flash crashes",
  2012 (arXiv 1201.3572; findings SNIPPET; journal venue Phys. Rev. E 85:056108 from memory, UNVERIFIED): branching
  ratio by MLE in 10–30-minute windows; the exponential-kernel estimate is biased when the true kernel is power-law
  (Hardiman et al.'s critique).
- Hardiman & Bouchaud, "Branching ratio approximation for the self-exciting Hawkes process", 2014
  (arXiv 1403.5227; findings SNIPPET; journal venue Phys. Rev. E 90:062807 from memory, UNVERIFIED): a model-free
  estimator needing "only the mean and variance of the event count in a sufficiently large time window".

**Formulas.** Intensity `λ(t) = μ + Σ_{t_j < t} φ(t − t_j)`; branching ratio `n = ∫_0^∞ φ(s) ds` (fraction of events
caused by other events); stationary iff `n < 1`; mean intensity `μ/(1 − n)`. Count statistics in long windows:
`Var[N_T] / E[N_T] → 1/(1 − n)²`, so `n̂ = 1 − sqrt(E[N_T] / Var[N_T])` (derived from the standard relation; the
search snippet's wording was ambiguous about which ratio is under the root, so check the paper before quoting).

**Bar form.** Needs `N_i` (trades per bar) and equal-length windows. On time bars: `n̂` from the mean and variance
of `N_i` over the lookback (needs windows much longer than the kernel scale; with 60 one-minute bars this is a crude
estimate). On range and tick bars the windows are not equal in time, so use `N_i/τ_i` (trades per second) and the
variance-to-mean of trade counts in fixed time slices if you have them; otherwise the honest bar-level proxy is
simply intensity (section g) and its clustering `max_N(N_k/τ_k) / median_N(N_k/τ_k)`.

**Tells.** How reflexive the tape is right now: a burst of aggression that breeds more aggression (`n` near 1) versus
exogenous, isolated hits. Practically: trade-intensity spikes are self-feeding; do not read a single fast bar as news.

### g) Dufour & Engle: time between trades and price impact

Dufour & Engle, "Time and the price impact of a trade", J. Finance 55(6), 2467–2498, 2000
(SNIPPET, https://ideas.repec.org/a/bla/jfinan/v55y2000i6p2467-2498.html): extends Hasbrouck's VAR so the trade
coefficients depend on the duration since the previous trade; "shorter gaps between trades go with a larger price
impact per trade, faster price adjustment to trade-related information, and stronger positive autocorrelation in
signed trades"; busy periods read as informed trading / reduced liquidity. Counterpoint: Grammig, Theissen,
Wuensche 2007 (SNIPPET only, not read).

**Bar forms.**
- Trade intensity `I_i = N_i / τ_i` (trades/s) and volume intensity `V_i / τ_i` (contracts/s) — official Quantower
  `IndicatorSpeedOfTape.cs` computes exactly `|field| / seconds` with the bar's `TicksRight − TicksLeft` (VERIFIED).
- Impact conditional on intensity: compare `A_i = |Δp_i|/V_i` (or the user's `E_i`) between fast and slow bars.
- On range bars `τ_i` is itself the primary observable (section 3).

**Tells.** Fast tape = each contract moves price more and flow is more persistent; slow tape = aggression is cheap
to absorb. The same delta means more on a fast bar.

### h) Footprint: diagonal imbalance, stacks, unfinished auctions, absorption, delta at extremes

Conventional definitions, as the vendors state them (all SNIPPET; pages could not be opened):

- **Sierra Chart, Numbers Bars** (https://www.sierrachart.com/index.php?page=doc%2FNumbersBars.php and support
  threads 57628, 90118, 44912, 46363, 84458): the diagonal rule is "a bid is measured against the ask one price
  level up, and an ask against the bid one level down". "Percent Compare Thresholds" are three comma-separated
  ratios where 1.0 = 100%, defaults .25/.50/.75; for the diagonal dominant-side method the docs recommend
  1.25/1.50/2.0, and support suggests 2, 3, 4 for 200/300/400%. The minimum-volume rule skips the comparison "only
  when both the bid and ask fall below the threshold". Unfinished auction: input "Highlight Nonzero Bid&Ask at
  High/Low Threshold" requires both bid and ask at the bar's high (or low) to exceed a minimum. Numbers Bars itself
  does not display stacked imbalances; support points to the "Volume at Price Threshold Alert V2" study.
- **ATAS** (https://help.atas.net/en/support/solutions/articles/72000602235, .../72000606631-cluster-settings,
  https://learn.atas.net/volume-basics/volume-analysis/imbalances, .../footprint-patterns): diagonal comparison
  "matching how the order book works ... the best ask and best bid are separated by a one-tick spread"; the help page
  default is 150%; ATAS Learn treats 200% as imbalance and 400% as "strong", with "a working starting point around
  300%"; settings include a "Volume Filter — minimum volume required for highlighting" and a "Minimum Difference" in
  contracts; stacked = "three or more consecutive imbalances"; unfinished auction = "a candle edge with non-zero bid
  and ask values ... price may return to this area"; a zero at the edge = finished auction.
- **Quantower** (https://www.quantower.com/blog/imbalance-footprint-chart-and-rithmic-plugin and
  https://help.quantower.com/quantower/analytics-panels/chart/volume-analysis-tools/cluster-chart): cluster chart
  type "Imbalance" shows sell volume (bid) on the left and buy volume (ask) on the right; "Buy volume at the ask is
  measured against sell volume at the bid one level below, and sell volume at the bid against buy volume at the ask
  one level above"; "Set it to 3 and Quantower highlights every level where one side traded more than 300% past the
  other. You can set several ratio levels at once and color each one"; "most order-flow traders settle somewhere
  around 200–300%" (truncated snippet). Quantower's stacked-level count and volume filter: UNVERIFIED.
- **Jigsaw** (https://daytradr.jigsawtrading.com/jt-footprint.html, https://blog.jigsawtrading.com/footprint-charts,
  https://www.jigsawtrading.com/wp-content/uploads/2014/03/PracticalApplicationOfOrderFlow-Pt2.pdf): "Tilt Mode"
  draws the diagonal pairing; settings "Minimum Imbalance volume" (volume needed before an imbalance is calculated)
  and "Imbalance Percentage" (the other side must exceed it); absorption described as offers holding firm against
  continued buying, "so delta will continue to increase but price will not", often "across many prices".
- **LuxAlgo** (third party, https://www.luxalgo.com/library/concept/bid-ask-imbalance.md): 300% default ratio,
  stacked = 3+ consecutive levels, "repeated buy imbalances that fail to lift price imply passive sellers are
  soaking up the aggression".

**Official Quantower code idiom** (VERIFIED, `IndicatorDeltaDots.cs` lines 158–200): walk `PriceLevels` into a
sorted price list; for a "diagonal comparison" option it takes, for each level, the maximum `SellVolume` among the
neighbouring levels j ∈ {−1, 0, +1}; replaces a zero side with 1 unless `allowZeroComparsion`; skips a level when
`item.Value.Volume <= minimumVolume` or both sides are `<= compareTreshold`. That is Quantower's own guard set for
the zero-division and small-number problems.

**Formulas (conventional).** Levels on a `tick` grid, `A(p)` ask/buy volume, `Bd(p)` bid/sell volume, ratio
threshold `r` (3.0 = 300%), volume floor `v_min`, stack length `k` (3):
```
buy imbalance at p :  A(p) ≥ r · max(Bd(p − tick), 1)   and  A(p) ≥ v_min
sell imbalance at p:  Bd(p) ≥ r · max(A(p + tick), 1)   and  Bd(p) ≥ v_min
stacked buy (sell) imbalance: ≥ k consecutive price levels flagged buy (sell)
unfinished auction at the high:  A(H) > 0 and Bd(H) > 0   (both ≥ v_ua)      — same at the low
diagonal delta (bar):  DD = Σ_p [A(p) − Bd(p − tick)]      (an alternative signed imbalance)
delta at extremes:     D_top(k) = Σ over the top k levels of (A − Bd);  D_bot(k) likewise
absorption at a level: volume at p ≥ m · median level volume  and  price does not trade beyond p
                       (+ Jigsaw's description: delta keeps growing, price does not)
```
Vendor defaults differ (150% ATAS help, 200/300/400% elsewhere, Sierra's own are in ratio-of-dominant-side form).
Treat `r`, `v_min`, `k` as user settings with 3.0 / instrument-scaled / 3 as defaults, which matches the modal
practitioner convention.

**Per-bar aggregates that fit a sub-window indicator.** Count of buy-stacked and sell-stacked levels, longest stack
and its side, share of bar volume inside imbalanced levels, `DD`, `D_top/D_bot`, unfinished-auction flags at high
and low, the price of the largest single-level volume (bar POC) relative to the close.

**Tells.** Diagonal imbalance = aggression that out-traded the opposite passive side one tick away; stacks = the
aggression persisted across prices (initiative); unfinished auction = the extreme was not rejected cleanly (both
sides still trading there); absorption = aggression hit a wall; delta at the extremes = who was active where the
bar turned.

### i) Wyckoff effort vs result, formalised

Wyckoff's third law (Pruden, "The Three Skills of Top Trading", Wiley 2007; StockCharts "The Laws of Wyckoff",
Dec 2015, SNIPPET https://stockcharts.com/articles/wyckoff/2015/12/the-laws-of-wyckoff.html): volume is the
effort, price change the result; "large volume producing only a marginal price advance near the end of a trend" is
read as a tiring trend. Karpoff, "The relation between price changes and trading volume: a survey", J. Financial and
Quantitative Analysis 22(1), 109–126, 1987 (SNIPPET): volume rises with |Δp| and, in equities, with its sign.

**Formal versions (equivalent up to normalisation).**
```
effort  e_i = D_i / s_D   (signed; s_D = typical |D| over N)    or  V_i / s_V  (unsigned)
result  r_i = Δp_i / s_p  (s_p = typical |Δp| over N)
ratio   ρ_i = r_i / e_i   → Amihud-like (price per unit of flow);  1/ρ_i → depth
mismatch: sgn(e_i) ≠ sgn(r_i) with |e_i| ≥ 1           → "trapped"
absorb:   |e_i| ≥ a  and  r_i·sgn(e_i) ≤ b              → "absorbed"  (user: a = 2.5, b = 0.25 of typical)
```
The user's `BarRead` computes `e = D / mean_N|D|` and `r = (C − O) / mean_N|ΔC|` (INFERRED, lines 2011–2018;
`ScaleC` = `Win(AbsDc)/N`, `ScaleP` = `Win(AbsDp)/N` floored at half the bar height and one tick) and `Verdict` is
the 2×2 effort/result classifier (lines 4134–4190). In Kyle/CKS terms `ρ` is λ and `e/r` is depth; on range bars
`r ≈ ±1` always, so the classifier must collapse to effort and sign (the user's 2.0 `LastBarReading` already does
this; the forming bar still uses the full verdict because its result is not final).

### j) 2015–2025 evidence on trade/order imbalance predictability at short horizons

- Gould & Bonart, "Queue imbalance as a one-tick-ahead price predictor in a limit order book", Market Microstructure
  and Liquidity 2(2), 1650006, 2016 (arXiv 1512.03492; SNIPPET): best-queue imbalance predicts the direction of the
  next mid-price move; strong for large-tick stocks, moderate for small-tick. Book-based.
- Kolm, Turiel, Westray, "Deep order flow imbalance: Extracting alpha at multiple horizons from the limit order
  book", Mathematical Finance 33(4), 1044–1081, 2023 (SSRN 3900141; SNIPPET): 115 Nasdaq stocks; models trained on
  order flow beat raw-book inputs; "the effective forecast horizon is about two average price changes".
- Xu, Gould, Howison 2019 and Cont, Cucuringu, Zhang 2023 (section b): contemporaneous fit improves with depth;
  cross-asset OFI forecasts returns only at short horizons.
- Lucchese, Pakkanen, Veraart, "The short-term predictability of returns in order book markets: a deep learning
  perspective", 2022/2024 (arXiv 2211.13777; SNIPPET): past order flow predicts future price changes in LOB markets.
- Lu, Reinert, Cucuringu, conditional order imbalance (COI), 2022 (SNIPPET only): association with future returns
  is positive for isolated trades, negative otherwise — sign depends on context.
- Takahashi, "Returns and Order Flow Imbalances: Intraday Dynamics and Macroeconomic News Effects", arXiv 2508.06788
  (v4 Oct 2025; SNIPPET): E-mini S&P 500, structural VAR at one-second frequency inside 15-minute windows; macro
  announcements raise price impact and lower flow impact; "shocks fade almost entirely within a second"; parameters
  vary across the day with liquidity, intensity and spreads.
- Chordia, Roll, Subrahmanyam, "Order imbalance, liquidity, and market returns", J. Financial Economics 65(1),
  111–130, 2002 (SNIPPET): daily NYSE; returns respond to contemporaneous and lagged imbalances; returns reverse after
  high-negative-imbalance days. Same authors, "Evidence on the speed of convergence to market efficiency", JFE 76,
  271–292, 2005 (SNIPPET): imbalances are persistent, returns are not; intraday return dependence is removed within
  "more than 5 minutes but less than 60 minutes".
- Mu, Zhou, Chen, Kertész, "Order flow dynamics around extreme price changes on an emerging stock market", 2010
  (arXiv 1003.0168; journal UNVERIFIED): volume imbalance rises into the extreme and decays as a power law; price
  reverses partially after both extreme drops and rises. (Cited in the user's guide for exhaustion.)

**What this adds up to.** The contemporaneous relation (imbalance ↔ price change in the same interval) is strong and
roughly linear with a depth-dependent slope. The predictive relation exists but is short: seconds to about two
price changes in liquid futures/equities, decaying fast; it is strongest for book-based imbalance; part of
trade-imbalance "predictability" is the mechanical persistence of order splitting plus the slow completion of
impact (Hasbrouck's lag). For a bar indicator this means: present imbalance as a description of the bar that just
happened and of who is in control, with the effort/result and intensity context; do not present it as a forecast.

## 3. Critical section: range bars, redundancy, pitfalls, normalisation

### 3.1 What a range bar removes and what it leaves

On a finished Quantower range bar `H − L = R` and the close is an extreme (section 1), so `|Δp|` carries almost no
information bar to bar (`|C − O| ≤ R`, usually close to `R`; the sign is the information). Everything the
displacement-based measures were built on becomes a constant. What varies: `τ_i`, `N_i`, `V_i`, `D_i`, the delta
path (`D⁺, D⁻`, give-back), the footprint, and the sign relative to the previous bars.

| Measure | Time bars | Tick bars (`N` fixed) | Range bars (`|Δp|` fixed) |
|---|---|---|---|
| `TI = D/V` | fine | fine | **robust** (independent of displacement); read with `V` |
| VPIN-style `Σ|D|/ΣV` | fine | fine | **robust**; buckets are "range buckets", so volume-weight it |
| Amihud `|Δp|/V` | fine | fine | **redundant**: collapses to `R/V`, i.e. inverse volume |
| Kyle/OFI regression `Δp ~ D` | fine | fine | **degenerate**: no variance in `|Δp|`; the only valid form is sign-based, `P(up | D)` (logistic), which is just "did the flow win" |
| Effort per range `D / typical|D|`, `V / typical V` | partial (result varies too) | partial | **the primary measure**: flow needed to move one range = depth proxy (Kyle: `ΔP` fixed ⇒ flow ∝ 1/λ) |
| Duration `τ`, intensity `N/τ`, `V/τ` | `τ` fixed; `N`, `V` vary | `N` fixed; `τ`, `V` vary | **primary**: passage time of a fixed price distance is a volatility estimator (ADS below) |
| Average trade size `V/N` | fine | **primary** (only size varies) | fine |
| Give-back `(D⁺ − D)/max(D⁺, −D⁻)` and sign mismatch `sgn(D) ≠ sgn(C − O)` | fine | fine | **robust**, and the cleanest "trapped" signal because the result is known to be one full range |
| Footprint diagonals, stacks, unfinished auctions | fine | fine | robust, with two caveats below |
| Order-sign autocorrelation, runs | fine | fine | robust (sign series is the one thing a range bar keeps intact) |
| Hawkes `n̂` from count variance | needs equal windows: OK | not equal windows | not equal windows: use intensity only |
| Wyckoff verdict (effort × result) | fine | fine | collapse to effort and sign; keep the full verdict only for the forming bar |

Andersen, Dobrev, Schaumburg, "Duration-based volatility estimation", 2008 working paper (SNIPPET
https://gcoe.ier.hit-u.ac.jp/information/schedule/pdf/ADS_DRVDraft_0807.pdf): integrated variance is estimated
from "the relationship between speed and passage time of Brownian motion", robust to jumps and microstructure noise;
one summary notes price thresholds should exceed 2–3 bid-ask spreads. For a fixed threshold `h`,
`E[τ_h] = c · h² / σ²` (the constant `c` depends on the passage definition — first exit versus range reaching `h` —
UNVERIFIED), so `σ̂² ∝ R² / τ_i`: **a range bar's duration is a local volatility reading**, and the user's "3× faster
than typical" row is a volatility tier. Combined with delta: `D_i/τ_i` is signed flow intensity, and `D_i` per range
at a given `τ_i` is depth at that volatility.

Two range-bar caveats for the footprint (INFERRED, not in any source):
1. A range bar has at most `R/tick + 1` price levels, so stack length is capped by the bar height (8-tick bar ⇒ at
   most 9 levels; a 3-stack is a third of the bar). Stack counts are not comparable across range sizes; express as a
   fraction of the bar's levels or keep `k = 3` and say so.
2. Because the bar closes at its extreme by construction and the next bar opens beyond it, the closing level is
   often still two-sided when the bar is cut. "Unfinished auction at the close extreme" will fire far more often than
   on time bars. Only the extreme opposite to the close (the rejected side) carries the conventional meaning; test
   the closing-side flag before showing it.

### 3.2 Redundancy inside one feature set

- `TI`, `BuyVolumePercent`, `DeltaPercent`, `D/V` are the same number.
- `D/typical|D|` (the user's `E`) and `V/typical V` are two views of one bar: `E = TI · V/typical|D|`. Show one
  signed (`E`) and one unsigned (`V`), not three.
- Amihud per bar on range bars is `1/V` (drop it); on tick bars it is `|Δp| · N / V·N`, i.e. displacement per
  contract — keep.
- `DeltaFinish` (mirror, UNVERIFIED) = `D − (the larger-magnitude extreme)` is the signed give-back; the user's
  `val` is the same quantity normalised. Keep one.
- The diagonal delta `DD` and the plain delta `D` are highly collinear on most bars; `DD` adds information only
  through the level-by-level pairing, so show stacks/counts, not `DD` as a second delta line.
- Fast/slow mean delta (user's `Fast`, `Slow`) and CVD slope are the same thing at two windows; acceleration is the
  difference of `Fast` — fine, but three lanes of the same series is the limit.

### 3.3 Pitfalls (each with the guard)

- **Zero volume / missing data.** `VolumeAnalysisData` or `.Total` can be null for a bar (the user's `_pendingVa`
  machinery exists for this; official scripts `continue` on null). `V_i = 0` makes `TI`, Amihud and all ratios
  undefined. Guard: mark the bar "not real" (the user's `Real` flag) and exclude it from every rolling statistic and
  from autocorrelation — do not fill with 0, which drags means and ranks toward zero and creates fake sign flips.
- **Division by small numbers.** Diagonal ratios with `Bd = 0` (the official DeltaDots replaces 0 by 1; Sierra skips
  when both sides are under the minimum). Use `max(denominator, 1)` plus a numerator floor `v_min`, never a bare
  ratio. For `E = D/s_D` the scale `s_D` must be floored (the user floors at 1e-9, which is too small to matter: use
  a floor of a few contracts, or the lookback's minimum non-zero |D|). For intensity `N/τ`, floor `τ` at the feed's
  timestamp resolution (a 0-second bar is possible when all trades share a timestamp).
- **Heavy tails.** Volume and |delta| per bar are heavy-tailed (power-law tails for trade volume are documented, e.g.
  Gopikrishnan, Plerou, Gabaix, Stanley, Phys. Rev. E 62, R4493, 2000 — UNVERIFIED here). A mean-based `s_D`
  (the user's `ScaleC` is a mean of |D|) is pulled up by one news bar for the whole lookback, so everything after it
  reads "quiet". Use the median, or a trimmed mean.
- **First bars.** Rolling statistics need N real bars; the user returns rank −1 below 20 bars. Also the
  `MaxDelta/MinDelta` sentinels (`double.MinValue/MaxValue`, VERIFIED in the official script) must be treated as
  "unknown", not as extremes. Before `VolumeAnalysisCalculationState.Finished` the data are partial: compute
  nothing that is later shown as history (or recompute on `VolumeAnalysisData_Loaded`, as the user does).
- **Session resets and session mixing.** Typical |D|, typical V and typical τ differ by an order of magnitude
  between RTH and ETH. A 60-bar lookback that straddles the open mixes regimes: the first RTH bars all rank
  "EXTREME" and the first ETH bars all rank "quiet". Options: lookback restricted to the same session type
  (the user's `SessionAnchor`/`ISession.IsPrimary` plumbing is there), or per-session-type scale tables, or at least
  a flag "lookback crosses a session boundary". Durations that span a close or halt (`TicksRight − TicksLeft`
  includes the dead time) must be capped or excluded; the official SpeedOfTape does not guard this.
- **The forming bar.** Its `V`, `N`, `D`, `τ` are partial, so its rank against finished bars is biased low and its
  footprint is live. Either scale by the progress fraction `f` (the user's `F`, range so far / R or trades so far /
  N) and label as pending, or rank only finished bars and show the forming bar hollow. On range bars the "result"
  of the forming bar is not final, which is why its verdict still uses the full effort/result table.
- **Repainting.** Pending signals on the forming bar are removed when the bar changes (`RemovePending`); anything
  derived from a rank must be recomputed when the bar closes because the lookback window shifts by one.
- **Rank granularity and saturation.** With N = 60 a percentile moves in steps of 1/60; "95%" means "top 3 of 60";
  a bar 10× the typical and a bar 3× the typical can both be rank 1.0. Show the magnitude next to the rank
  (the user already prints "+1.2K · 91%").
- **Classification source.** Delta depends on the feed's aggressor flag (CQG/AMP, Rithmic) and on Quantower's fallback
  when the flag is missing (UNVERIFIED). The same bar on two feeds can differ. Do not promise exact numbers across
  connections; the user's guide tags "Quantower (AMP)" for this reason.
- **Duplicate counting in footprints.** `PriceLevels` is keyed by exact price; with fractional ticks or a feed that
  prints off-grid prices, two keys can map to one visual level. Snap keys to the tick grid before pairing diagonals.

### 3.4 Normalisation: rank vs z-score vs fixed multiples

- **Percentile rank over N bars** (user's `FlowRank`, ties count half): distribution-free, bounded, robust to heavy
  tails, directly interpretable ("top 5% of the last 60"), and the convention of the VPIN literature (CDF of the
  measure). Costs: O(N) per bar unless a sorted window is kept; step size 1/N; saturates; adapts slowly after a
  regime change (the old regime stays in the window for N bars); needs the same-session care above. Use it for tiers,
  colours and alarms.
- **Z-score with mean/SD**: keeps magnitude beyond the window's extremes, but mean and SD are "particularly
  sensitive to outliers" (Leys, Ley, Klein, Bernard, Licata, "Detecting outliers: Do not use standard deviation around
  the mean, use absolute deviation around the median", J. Experimental Social Psychology 49(4), 764–766, 2013;
  SNIPPET). For heavy-tailed |D| and V the SD inflates after one big bar and everything else reads normal. If a
  z-score is wanted, use the robust form `z = (x − median) / (1.4826 · MAD)` with the 2.5 threshold Leys et al.
  recommend (SNIPPET), or z-score `log V`, `log τ`, `log(1 + |D|)`, which are close to symmetric.
- **Fixed multiples of a typical size** (user's 1.x tiers 0.4/1/2× typical |D|): stable meaning across time and
  easy to explain, but only as good as the "typical" (use a median) and not comparable across instruments or
  sessions unless the typical is re-estimated per session type. For bounded quantities (`TI ∈ [−1, 1]`, give-back
  fraction ∈ [0, 1]) fixed thresholds are the right tool: they already have units. For footprint ratios the
  practitioner convention is a fixed ratio (300%) plus a volume floor; keep that and let the user set it.
- **Recommendation.** Two layers per measure: the number as a robust multiple (`x / median_N|x|`, or robust z) and the
  tier as a same-session percentile rank; fixed thresholds only for bounded ratios and footprint ratios; durations in
  log space; all rolling statistics over real, finished, same-session-type bars; forming bar scaled by progress and
  marked pending.

### 3.5 What is worth carrying into the new indicator (my inference)

Per finished bar, in priority order for range bars: (1) signed effort `E = D / median_N|D|` with the bar's
direction (with / against / trapped); (2) duration tier `τ / median_N τ` (log scale) and intensity `N/τ` or `V/τ`;
(3) `TI` and the give-back fraction; (4) footprint: stack counts by side (fraction of the bar's levels), unfinished
auction at the rejected extreme, delta at the top and bottom `k` levels; (5) run length of same-sign deltas and its
rank; (6) a volume-weighted `Σ|D|/ΣV` over N bars as the unsigned "one-sidedness" (VPIN-like), shown with intensity.
On tick bars add average trade size `V/N` and keep Amihud `|Δp|/V`. On time bars everything applies as written.

## 4. Sources (one line each; status as marked in the text)

- Easley, López de Prado, O'Hara 2012, RFS 25(5) 1457–1493 — https://papers.ssrn.com/abstract=1695596 (SNIPPET)
- Easley, López de Prado, O'Hara 2011, JPM 37(2) 118–128 — https://papers.ssrn.com/abstract=1695041 (SNIPPET)
- Easley, López de Prado, O'Hara 2016, JFE 120(2) 269–285 — https://ideas.repec.org/a/eee/jfinec/v120y2016i2p269-285.html (SNIPPET)
- Andersen & Bondarenko 2014, JFM 17 1–46 — https://www.kellogg.northwestern.edu/academics-research/research/detail/2014/vpin-and-the-flash-crash/ (SNIPPET)
- Cont, Kukanov, Stoikov 2014, JFEC 12(1) 47–88 — https://arxiv.org/abs/1011.6402 ; https://hal.sorbonne-universite.fr/hal-00937055 (SNIPPET)
- Xu, Gould, Howison 2019, MML — https://arxiv.org/abs/1907.06230 (SNIPPET)
- Cont, Cucuringu, Zhang 2023 — https://arxiv.org/abs/2112.13213 (SNIPPET)
- Silantyev 2019, Digital Finance 1(1) 191–218 — https://ideas.repec.org/a/spr/digfin/v1y2019i1d10.1007_s42521-019-00007-w.html (SNIPPET)
- Kyle 1985, Econometrica 53(6) 1315–1335 — https://edegan.com/pdfs/Kyle%20%281985%29%20-%20Continuous%20Auctions%20and%20Insider%20Trader.pdf (not read; formula UNVERIFIED)
- Amihud 2002, JFM 5(1) 31–56 — https://www.cis.upenn.edu/~mkearns/finread/amihud.pdf (SNIPPET)
- Goyenko, Holden, Trzcinka 2009, JFE 92 153–181 — https://ideas.repec.org/a/eee/jfinec/v92y2009i2p153-181.html (SNIPPET)
- Hasbrouck 1991, JF 46(1) 179–207 — https://ideas.repec.org/a/bla/jfinan/v46y1991i1p179-207.html (SNIPPET)
- Dufour & Engle 2000, JF 55(6) 2467–2498 — https://ideas.repec.org/a/bla/jfinan/v55y2000i6p2467-2498.html (SNIPPET)
- Lillo & Farmer 2004, SNDE 8(3) — https://ideas.repec.org/a/bpj/sndecm/v8y2004i3n1.html ; https://arxiv.org/abs/cond-mat/0311053 (SNIPPET)
- Lillo, Mike, Farmer 2005, PRE 71 066122 — https://www.doi.org/10.1103/PHYSREVE.71.066122 (SNIPPET)
- Tóth, Palit, Lillo, Farmer 2015, JEDC 51 218–239 — https://ideas.repec.org/a/eee/dyncon/v51y2015icp218-239.html ; https://arxiv.org/abs/1108.1632 (SNIPPET)
- Tóth et al. 2011, PRX 1 021006 — https://arxiv.org/abs/1105.1694 (SNIPPET)
- Lillo, Farmer, Mantegna 2003, Nature 421 129–130 — https://www.doi.org/10.1038/421129A (SNIPPET)
- Bouchaud, Gefen, Potters, Wyart 2004, QF 4(2) 176–190 — https://ideas.repec.org/a/taf/quantf/v4y2004i2p176-190.html (SNIPPET)
- Bacry, Mastromatteo, Muzy 2015, MML 1(1) 1550005 — https://arxiv.org/abs/1502.04592 (SNIPPET)
- Hardiman, Bercot, Bouchaud 2013, EPJB 86 442 — https://arxiv.org/abs/1302.1405 (SNIPPET)
- Filimonov & Sornette 2012, PRE 85 056108 — https://arxiv.org/abs/1201.3572 (SNIPPET)
- Hardiman & Bouchaud 2014, PRE 90 062807 — https://arxiv.org/abs/1403.5227 (SNIPPET)
- Andersen, Dobrev, Schaumburg 2008 — https://gcoe.ier.hit-u.ac.jp/information/schedule/pdf/ADS_DRVDraft_0807.pdf (SNIPPET)
- López de Prado 2018, Advances in Financial ML, ch. 2 — via https://mlfinpy.readthedocs.io/en/latest/FinancialDataStructure.html (secondary, SNIPPET)
- Gould & Bonart 2016, MML 2(2) 1650006 — https://arxiv.org/abs/1512.03492 (SNIPPET)
- Kolm, Turiel, Westray 2023, Math. Finance 33(4) 1044–1081 — https://papers.ssrn.com/abstract=3900141 ; https://ideas.repec.org/a/bla/mathfi/v33y2023i4p1044-1081.html (SNIPPET)
- Lucchese, Pakkanen, Veraart 2022 — https://arxiv.org/abs/2211.13777 (SNIPPET)
- Takahashi 2025 — https://arxiv.org/abs/2508.06788 (SNIPPET)
- Chordia, Roll, Subrahmanyam 2002, JFE 65(1) 111–130 — https://ideas.repec.org/a/eee/jfinec/v65y2002i1p111-130.html (SNIPPET)
- Chordia, Roll, Subrahmanyam 2005, JFE 76 271–292 — https://rpc.cfainstitute.org/en/research/cfa-digest/2005/11/evidence-on-the-speed-of-convergence-to-market-efficiency-digest-summary (SNIPPET)
- Mu, Zhou, Chen, Kertész 2010 — https://arxiv.org/abs/1003.0168 (SNIPPET)
- Karpoff 1987, JFQA 22(1) 109–126 — https://ideas.repec.org/a/cup/jfinqa/v22y1987i01p109-126_01.html (SNIPPET)
- Pruden 2007 / StockCharts "The Laws of Wyckoff" — https://stockcharts.com/articles/wyckoff/2015/12/the-laws-of-wyckoff.html (SNIPPET)
- Leys et al. 2013, JESP 49(4) 764–766 — https://orbilu.uni.lu/handle/10993/59204 (SNIPPET)
- Sierra Chart Numbers Bars — https://www.sierrachart.com/index.php?page=doc%2FNumbersBars.php (SNIPPET)
- ATAS Bid/Ask Imbalance — https://help.atas.net/en/support/solutions/articles/72000602235 ; https://learn.atas.net/volume-basics/volume-analysis/imbalances (SNIPPET)
- Quantower imbalance blog — https://www.quantower.com/blog/imbalance-footprint-chart-and-rithmic-plugin (SNIPPET)
- Quantower cluster chart help — https://help.quantower.com/quantower/analytics-panels/chart/volume-analysis-tools/cluster-chart (SNIPPET)
- Quantower range bars help — https://help.quantower.com/quantower/analytics-panels/chart/chart-types/range-bars (SNIPPET)
- Jigsaw footprint — https://daytradr.jigsawtrading.com/jt-footprint.html ; https://blog.jigsawtrading.com/footprint-charts (SNIPPET)
- Quantower official examples (VERIFIED, read): https://github.com/Quantower/Examples (IndicatorExamples/DrawValueAreaForEachBarIndicator.cs) and https://github.com/Quantower/Scripts (Indicators/IndicatorCumulativeDelta.cs, IndicatorDeltaFlow.cs, IndicatorDeltaDots.cs, IndicatorSpeedOfTape.cs, IndicatorTimeHistogram.cs)
- Decompiled mirror (UNVERIFIED): https://github.com/PickReaper/quantower
- Official API docs (NOT reachable this session): https://api.quantower.com/docs/TradingPlatform.BusinessLayer.VolumeAnalysisItem.html, ...VolumeAnalysisData.html, ...IVolumeAnalysisIndicator.html, ...HistoryItemBar.html
