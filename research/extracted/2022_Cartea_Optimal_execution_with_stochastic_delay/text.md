Finance and Stochastics (2022) 27:1–47 https://doi.org/10.1007/s00780-022-00491-w 



# **Optimal execution with stochastic delay** 

### **Álvaro Cartea**<sup>**1,2**</sup> **· Leandro Sánchez-Betancourt**<sup>**3**</sup> 

Received: 1 June 2022 / Accepted: 6 July 2022 / Published online: 1 December 2022 © The Author(s) 2022 

### **Abstract** 

We show how traders use marketable limit orders (MLOs) to liquidate a position over a trading window when there is latency in the marketplace. MLOs are liquidity-taking orders that specify a price limit and are for immediate execution only; however, if the price limit of the MLO precludes it from being filled, the exchange cancels the order. We frame our model as an impulse control problem with stochastic latency where the trader controls the times and the price limits of the MLOs sent to the exchange. We show that impatient liquidity takers submit MLOs that may walk the book (capped by the limit price) to increase the probability of filling the trades. On the other hand, patient liquidity takers use speculative MLOs that are only filled if there has been an advantageous move in prices over the latency period. Patient traders who are fast do not use their speed to hit the quotes they observe, or to finish the execution programme early; they use speed to complete the execution programme with as many speculative MLOs as possible. We use foreign exchange data to implement the random-latencyoptimal strategy and to compare it with four benchmarks. For patient traders, the random-latency-optimal strategy outperforms the benchmarks by an amount that is greater than the transaction costs paid by liquidity takers in foreign exchange markets. Around news announcements, the value of the outperformance is between two and ten times the value of the transaction costs. The superiority of the strategy is due to both the speculative MLOs that are filled and the price protection of the MLOs. 

**Keywords** Algorithmic trading · High-frequency trading · Stochastic delay · Latency 

**Mathematics Subject Classification (2020)** 93E20 · 91G80 · 49L20 · 49L25 

**JEL Classification** C02 · C61 

> � Á. Cartea alvaro.cartea@maths.ox.ac.uk 

> L. Sánchez-Betancourt leandro.sanchez-betancourt@kcl.ac.uk 

> 1 Mathematical Institute, University of Oxford, Oxford, OX2 6GG, UK 

- 2 Oxford-Man Institute of Quantitative Finance, Oxford, OX2 6ED, UK 

- 3 Department of Mathematics, King’s College London, London, WC2R 2LS, UK 

Á. Cartea, L. Sánchez-Betancourt 

2 

## **1 Introduction** 

In electronic trading, agents are exposed to the risks that stem from latency in the marketplace. Latency is the aggregate of the time lags associated with the various stages of a trade. These stages, which occur in succession and are separated in time by random delays, include: the exchange sends quotes to an agent, the agent receives the quotes, the agent processes information and sends an instruction to the exchange, the exchange receives and handles the instruction, and finally, the outcome is notified to the agent. During the latency period, it is likely that the exchange will process other instructions that modify the limit order book (LOB) and possibly affect the outcome of the agent’s instructions. In particular, liquidity takers face the risk that the prices they target are not available by the time the exchange processes their order because the best quotes they observed were stale and therefore updated during the latency period. The risks faced by liquidity providers are different; e.g. their limit orders may be adversely selected by a taker. If the market moves against the trader’s interest, the order will not be filled or will be filled at worse prices – the outcome depends on the type of liquidity-taking order sent by the trader. 

At high frequencies (milliseconds, microseconds), the best quotes in the LOB tend to flicker. Flickers are unpredictable short-lived deviations of a few ticks from the best quotes, and are the result of very frequent occurrences of rapid sequences of post-and-cancel activity in the LOB and the less frequent arrival of aggressive orders that consume liquidity that is immediately replenished. Thus over the latency period, the main source of risk faced by liquidity takers is from flickers in the book, and, to a lesser extent, the risk that stems from unexpected changes in the fundamental best quotes by the time their orders are processed. If latency were zero, traders would not face these risks because they would always take liquidity at the prices and quantities they observe in the LOB; however, all market participants face latency and latency is random. 

In this paper, we show how a trader can execute a large position in a financial instrument when the trader faces random latency in the marketplace. The dynamics of the best bid price in the LOB consist of the ‘fundamental’ best quote of the instrument and the flickers (similarly for the best ask price). Innovations in the fundamental best quote are driven by a stochastic process, and the size and arrival times of flickers in the quotes are represented by a marked point process. The trader employs marketable limit orders (MLOs) to execute the position over a time window; MLOs are liquidity-taking orders that specify a price limit and are for immediate execution only. Filled MLOs have permanent price impact: a filled buy (sell) MLO exerts upward (downward) pressure on the fundamental value of the instrument. However, if the price limit of the MLO precludes it from being filled, the exchange cancels the order and there is no price impact because other market participants cannot observe missed trades. 

In our model, the trader controls the price limit of the MLO and controls when to submit it to the exchange, both of which largely depend on the trader’s degree of urgency to execute the position. An impatient liquidity taker will send sell (buy) MLOs with price limits that are below (above) the best bid (ask) price they observed in the LOB. This strategy increases the probability that the MLO is filled when processed 

Optimal execution with stochastic delay 

3 

by the exchange and caps how far the MLO is allowed to walk the LOB when there are adverse price changes over the latency period. On the other hand, we show that patient liquidity takers use MLOs predominantly to send speculative trades. Speculative MLOs seek a price improvement relative to the fundamental best quote observed by the trader when sending the order to the exchange. Patient liquidity takers do not use their speed to finalise their execution programme ahead of time, or to hit the best quotes they observe in the LOB, they use speed mainly to send as many speculative MLOs as possible during the trading horizon – fast traders have many opportunities to retry missed orders before reaching the end of the trading window. 

We use proprietary foreign exchange data from the LMAX Exchange (henceforth LMAX) between September 2019 and February 2020 for ten currency pairs to study the order types that liquidity takers use in the foreign exchange market and to analyse the effect of latency on the efficacy (i.e., hit and miss) of liquidity-taking orders. In all pairs, MLOs trade more volume than any other type of liquidity-taking order; in particular, more than market orders (MOs), which are for immediate execution and walk the LOB until filled in full. Though MLOs offer protection against adverse price moves, traders concede that the order may be cancelled by the exchange because clearing prices would breach the price limit instructed in the MLO. For example, in the EUR/USD pair, 41.0% of the MLOs were filled in full, 1.9% were partially filled, and 57.1% were missed. The MLOs that were partially or fully filled represent 56.9% of the volume traded in the pair, while MOs represent 13.5% of the total volume. The limit rates of missed MLOs were, on average, three ticks away from the best quote in the LOB when they were processed by the exchange. 

We use the LMAX data to implement and to benchmark the performance of the trader’s random-latency-optimal execution strategy against four strategies: (i) send MOs over a trading window and assume latency is zero, (ii) send MLOs over a trading window and assume that latency is deterministic, (iii) employ a discrete timeweighted average price (TWAP) strategy that sends MOs at equally spaced time intervals over the execution window, (iv) send one MO to execute the entire order at the beginning of the trading window, where, unrealistically, we assume that there is enough liquidity at the best bid in the LOB to fill the entire order. 

Estimates of model parameters for the EUR/USD currency pair are obtained with data between 1 August 2019 and 31 August 2019, and trading strategies are implemented with data between 1 September 2019 and 29 February 2020. We show that the performances of the random-latency-optimal strategy and of the deterministiclatency-optimal strategy are statistically the same. When the trader is patient, both strategies outperform the other benchmarks by a cash amount that is greater than the transaction costs paid by liquidity takers in foreign exchange markets, and around news events, the value of the outperformance increases to between two and ten times the value of the transaction costs. The source of the better performance of the latencyoptimal strategies stems from the speculative MLOs that are filled during the trading interval, and from the price protection of the MLOs against adverse price moves. In the EUR/USD pair, the number and the value of filled speculative MLOs increase when market activity increases because the probability and the size of positive flickers in the best bid rate increase during heightened market activity – this explains the considerable outperformance of the latency-optimal strategies around news events. 

Á. Cartea, L. Sánchez-Betancourt 

4 

Finally, we show that the value of the outperformance decreases as the degree of impatience of the trader increases because the strategy sends fewer speculative MLOs – very impatient traders do not send speculative MLOs, all their MLOs are for price protection. 

As far as we are aware, this is the first work to appear in the literature that shows how to optimally execute a position in an asset when the trader faces random latency. Closest to our work are those by Øksendal and Sulem [25] and Bruder and Pham [9], where the authors investigate general impulse control problems with deterministic delay, while ours assumes stochastic delay; [25] studies an infinite-horizon control problem with deterministic delay and an arbitrary number of pending orders, while [9] investigates a finite-horizon stochastic control problem with any finite number of pending orders. The key mathematical novelty that distinguishes our work is that delays are stochastic. From a modelling perspective, this requires an explicit definition of the _σ_ -algebra of the trader and an account of the new terms that arise as a consequence of stochastic delays. 

Although our model focuses on an optimal execution problem with stochastic latency, our framework is general and can be applied to control problems in which the outcomes of the controlled actions are observed with stochastic delay. In financial applications, our model may be implemented in all asset classes that trade in electronic LOBs with liquidity-taking orders that consider price limits and immediate time-in-force of the liquidity-taking orders (equity and foreign exchanges that offer MLOs include LMAX, LSE, Chi-X, CBOE, NASDAQ, CME, and NYSE). In contrast, all models of optimal execution in the extant literature assume that the trader operates in the marketplace with zero latency and employ only MOs to take liquidity; see e.g. Almgren and Chriss [4], Almgren [2], Bayraktar and Ludkovski [6], Alfonsi et al. [1], Guéant et al. [21], Guilbaud and Pham [22], Cartea et al. [13], Cartea and Jaimungal [10], Guéant [19], Cartea and Jaimungal [11], Barger and Lorig [5] and the monographs by Cartea et al. [12, Sect. 6] and Guéant [20, Part II]. 

In the literature, there is work that discusses how latency affects liquidity-taking strategies, market making and passive trading. Cartea and Sánchez-Betancourt [15] show how traders can adjust the price limit of MLOs to target a fill ratio in a given currency pair when the trader sends a sequence of orders over a trading interval (finite or infinite). In market making, Gao and Wang [18] show how an agent provides liquidity to the LOB of large-tick stocks when latency is deterministic and fixed during the trading interval. In passive trading (i.e., trading with limit orders), the work by Moallemi and Saˆglam [24] quantifies the cost of latency in equity (NASDAQ). 

In the next section, we employ high-frequency foreign exchange data to show that MLOs protect orders from adverse price moves and also receive price improvements when prices move in the trader’s interest over the latency period. Section 3 introduces the trader’s optimal execution problem with random latency and characterises the random-latency-optimal strategy as the solution of a Hamilton–Jacobi–Bellman quasi-variational inequality (HJBQVI), and develops two new benchmarks where the investor faces deterministic latency or faces no latency. Section 4 compares the performance of the trader’s random-latency-optimal strategy with that of the benchmarks. Section 5 concludes, and we collect some proofs in the appendices. 

Optimal execution with stochastic delay 

5 

## **2 Order types and data** 

### **2.1 Orders and revealed preferences** 

In order-driven electronic markets, the basic building blocks to trade are orders that provide liquidity and orders that take liquidity. When a trader sends an order to the exchange, the order contains two specific instructions: quote type and time-in-force. The quote type specifies the main feature of the order: price limit (i.e., LO), no price limit (i.e., MO), a trigger (i.e., stop orders), among others. The time-in-force refers to how long the order is active in the market. The shortest time-in-force is immediate execution and the longest time-in-force is typically for the rest of the trading day. Two prominent liquidity-taking order types that have an immediate execution timein-force are immediate-or-cancel (IoC) and fill-or-kill (FoK). IoC is an order to buy or to sell assets that must be executed immediately, in full or in part, while obeying the order’s price limit, and any portion of the volume of the order that cannot be filled at the desired price limit is cancelled. FoK is an order to buy or to sell assets that must be executed immediately in full or it is cancelled. 

Combinations of the various features constitute the type of order that market participants send to the exchange. Here, we focus on the types of liquidity-taking order that are designed to protect traders against the frictions that stem from latency. We define MLOs as liquidity-taking orders that have a price limit and the time-in-force is IoC or FoK. An MO is an MLO without price limit; the MLOs we consider have a finite price limit and are for immediate execution. MLOs protect liquidity takers from adverse price movements that occur between the time the trader makes a decision to trade (on a possibly stale quote) and the time the exchange matches the order with a limit order resting in the book (when possible). 

Traders reveal their preferences when they choose a type of liquidity-taking order for a trade or a sequence of trades whose outcome is contingent on latency. Their choices demonstrate that traders balance the cost of completing a trade and the costs of price protection. A trader who must complete a trade without delay will choose an MO to guarantee execution in full – and expose the trade to price movements over the latency period. In contrast, a trader who can afford to miss the trade if the price is ‘not right’ will choose an MLO. Thus in exchange for price protection against adverse price movements, the trader concedes that the trade might not get filled. On the other hand, if prices move in a favourable direction, the trader receives a price improvement, i.e., the MLO is executed at a better price than the trader’s decision price. Indeed, MLOs are also used by traders to complete a trade only if the price improves over the latency period. In the next section, we provide summary statistics of the use of MLOs in foreign exchange markets. 

### **2.2 Data** 

We present descriptive statistics for ten currency pairs in the foreign exchange spot market at LMAX. The data are stamped at a microsecond frequency and the range is from 1 September 2019 to 29 February 2020 – in Sect. 4, we use data from August 2019 to estimate model parameters. For each currency pair, we have: the aggregate 

Á. Cartea, L. Sánchez-Betancourt 

6 

|**Table 1** Percentage of<br>liquidity-taking by order type.||Number|of trades||Volume|traded||
|---|---|---|---|---|---|---|---|
|Period: 1 September 2019 to 29<br>February 2020. For each||MLOs|MOs|Other|MLOs|MOs|Other|
|currency pair, we use bold to<br>highlight the highest percentage|EUR/USD|**36.7%**|32.1%|31.2%|**56.9%**|13.5%|29.6%|
|<br>by number of trades and by|USD/JPY|41.4%|6.9%|**51.7%**|**55.8%**|3.1%|41.1%|
|volume traded|EUR/GBP|38.3%|8.0%|**53.7%**|**73.0%**|5.2%|21.8%|
||GBP/USD|**53.6%**|21.9%|24.5%|**72.8%**|7.1%|20.2%|
||USD/MXN|**44.7%**|24.6%|30.7%|**52.0%**|26.3%|21.7%|
||USD/CAD|**62.2%**|10.9%|26.9%|**71.2%**|2.6%|26.1%|
||GBP/JPY|**49.1%**|19.4%|31.5%|**61.2%**|8.5%|30.3%|
||EUR/JPY|33.0%|7.3%|**59.8%**|**65.0%**|4.3%|30.8%|
||AUD/USD|**66.7%**|10.6%|22.7%|**71.0%**|4.4%|24.6%|
||AUD/JPY|**52.8%**|11.7%|35.5%|**59.9%**|6.1%|34.0%|



volume of the limit sell orders posted at the best ask rate; the aggregate volume of the limit buy orders posted at the best bid rate; liquidity-taking orders sent to the LOB, including the type of order and trader identification; rate limits of MLOs; full fills, partial fills, and missed liquidity-taking trades. Finally, our data set does not contain the liquidity posted at the LOB beyond the best bid and best ask rates, but does contain the average rate paid or received for all liquidity-taking orders, including those that walked the LOB beyond the best quotes. 

Table 1 shows the percentage of the total number of trades that are MLOs, MOs and others, and also shows the percentages by traded volume. Recall that the MLOs we consider are IoC and FoK with an immediate time-in-force. In practice, the exchange processes the order as soon as it arrives and matches it (if possible). This processing time usually takes under 80 microseconds and is followed by a message from the exchange to the trader to notify the outcome. 

The category ‘others’ includes the remaining order types that consume liquidity, for example, marketable orders: (i) limit orders that are good-until-cancel and goodfor-day, (ii) stop orders good-for-day and good-until-cancel, and (iii) dark limit orders good-for-day and good-until-cancel. 

### **2.3 Flickering of best rates in the LOB** 

Figure 1 shows snippets of the evolution of best quotes and liquidity-taking activity for the currency pair EUR/USD on 3 September 2019 at around 2.00pm British summer time – the data are timestamped to the microsecond. Solid circles denote filled sell MLOs, squares denote filled sell MOs, and empty circles denote sell MLOs that were not filled because the limit sell rate in the order was higher than the best bid rate in the LOB when the exchange processed the order. 

The top and middle panels of Fig. 1 show the best bid rate and sell liquidity-taking orders between 2.00.00pm and 2.01.00pm (i.e., one minute) and between 2.00.08pm and 2.00.14pm (i.e., six seconds), respectively. It is clearly visible that the best bid rate goes through unpredictable flickers, which are extremely short-lived deviations of a significant number of ticks from the ‘fundamental’ or ‘true’ value of the best bid 

Optimal execution with stochastic delay 

7 

**Fig. 1** Best bid rate (blue line), best ask rate (red line), missed MLOs (empty circles), filled MLOs (full circles), filled MOs (squares). Top panel: 2.00pm to 2.01pm (60 seconds). Middle panel: 2.00.08pm to 2.00.14pm (6 seconds). Bottom panel: 2.00.12pm to 2.00.13pm (1 second). Microsecond data from EUR/USD pair, 3 September 2019 



rate. These flickers are the result of cancellations and arrivals of limit orders at the best bid rates, and of liquidity that is consumed by aggressive orders and immediately replenished by the arrival of limit orders. 

The bottom panel shows a one-second snippet, between 2.00.12pm and 2.00.13pm, of the best bid rate and the best ask rate and the liquidity-taking orders, all of which were buy MOs. In most cases, flickers on either side of the LOB cause a short-lived widening of the quoted spread of the LOB. 

Á. Cartea, L. Sánchez-Betancourt 

8 

From visual inspection, flickers in the best bid (best ask) rate are more often negative (positive) than positive (negative). This is a prevalent feature in the dynamics of the best quotes in the LOB. We return to this in Sect. 4 when we estimate model parameters and discuss the distribution of the flickers in the performance of the execution strategies. 

### **2.4 Distribution of flickers: hit and miss** 

We proceed to discuss the limit rates employed by traders who send MLOs to buy and to sell the EUR/USD currency pair. We employ the same data as that in Table 1 – the results for the remaining currency pairs are similar. To analyse both the hit and miss performance of the MLOs and the distribution of the flickers in the best quotes, we compute the slippage-price-improvement (SPI) measure 



Here, _Li_ denotes the exchange rate limit of the order, i.e., the maximum (minimum) rate that the trader is willing to pay (receive) per unit that she wishes to buy (sell); _Mi_ is the exchange rate that the order would pay (receive) per unit bought (sold) if the order is filled in full. The indicator _Ii_ determines the direction of the order: _Ii_ takes the value +1 when the trader sends a buy MLO and the value −1 when the trader sends a sell MLO, for _i_ = 1 _,...,n_ , where _n_ is the total number of MLOs. Thus when SPI _i_ is positive, the order was filled (relative to the limit rate _Li_ ) with a slack of SPI _i_ ticks, which we refer to as price improvement. Similarly, when SPI _i_ is negative, the order missed (fully or in part) by SPI _i_ ticks (relative to the limit rate _Li_ ), which we refer to as slippage. 

For each MLO, the quantity in (2.1) could understate or overstate slippage and price improvement relative to the trader’s decision rate; note that we compute SPI relative to the limit rate instructed in the order. Our data set does not contain the timestamps of when the trader submitted the MLO to the exchange, nor does it contain the rate observed by the trader when she decided to trade (i.e., the decision rate). We have the time-stamp of when the order is processed by the exchange. Thus we do not know the best bid and best ask rates and quantities posted in the LOB when the trader decided to send the order. 

The trader may find it optimal to send a buy MLO with a limit rate that is below the observed best ask rate or to send a sell MLO with a limit rate that is above the observed best bid rate. In other words, traders may send speculative MLOs that are filled only if there is a price improvement relative to the quotes they observe when deciding to trade – in Sect. 3.6 below we return to this point and show that it is optimal to send MLOs that target a price improvement. 

Figure 2 shows histograms in log-scale of SPI for the MLOs in the EUR/USD currency pair, where the _x_ -axes in both panels are truncated to lie between −25 and 25 ticks. To gain insights into the limit rates of the MLOs and into the distribution of the flickers, the top panel shows the histogram of SPI with all the MLOs sent to the exchange, and the bottom panel shows a histogram of SPI with the MLOs that aimed at the best quote, i.e., the trader’s decision rate and the limit rate she instructed in the MLO are the same. We consider that the limit rate of an MLO is equal to the decision rate of the trader if at any point within the previous 150 ms of the processing time of 

Optimal execution with stochastic delay 

9 

**Fig. 2** Histograms of SPI between −25 ticks and 25 ticks in log-scale for the EUR/USD, 1 September 2019 to 29 February 2020. One tick is 10<sup>−5</sup> USD. Top: all MLOs. Bottom: MLOs with limit rate equal to trader’s decision rate 



the MLO, there was a best quote in the LOB equal to the limit rate of the MLO (we obtain similar results when we assume that the time period of the look-back window is 100 ms). The histograms are for both IoCs and FoKs. 

In both panels, the histograms are skewed to the left. The skewness may be due to informed traders in the market and due to the distribution of the flickers. Informed traders send buy (sell) MLOs in anticipation of an increase (decrease) in the exchange rate. Thus all else being equal, their trading strategies skew the distribution of SPI to the left because it is more likely that a buy (sell) MLO misses the trade when the exchange rate drifts up (down). Informed MLOs are included in the top panel. In addition, the skew in the histogram also results from asymmetry in the distribution of the size of the flickers – this is better represented in the bottom panel where the limit rate of the MLO is equal to the trader’s decision rate; see also the bottom panel of Fig. 1, where most flickers on the best bid (ask) rate are negative (positive). Below we show that flickers in the best bid rate are negatively skewed and flickers in the best ask rate are positively skewed. 

### **2.5 Liquidity: make and take** 

In this subsection, we report various statistics of the volume posted at the best quotes in the LOB and the volume of the filled liquidity-taking orders of the ten currency 

Á. Cartea, L. Sánchez-Betancourt 

10 

**Table 2** TWS: time-weighted spread. TWQ: time-weighted quantity at best quotes. Period: 1 September 2019 to 29 February 2020 between 9.00am and 4.00pm. Each unit of quantity is a lot of 10’000 units of the base currency 

||Liquidit|y-taking (v|olume)|Liquidity provi|sion|||
|---|---|---|---|---|---|---|---|
|Pair|Mean|Std|Median|TWS<br>ask/bid|TWQ<br>ask/bid|Median Q<br>ask/bid|Mode Q|
|EUR/USD|20.94|26.64|4_._00|3_._94×10<sup>−5</sup>|160/159|100/100|50/50|
|USD/JPY|26.81|23.78|27_._00|4_._20×10<sup>−3</sup>|110/109|100/100|50/50|
|EUR/GBP|12.93|21.50|0_._60|9_._30×10<sup>−5</sup>|94/94|80/82|50/50|
|GBP/USD|20.65|21.97|14_._40|9_._83×10<sup>−5</sup>|91/91|75/66|50/50|
|USD/MXN|37.09|23.55|50_._00|4_._67×10<sup>−3</sup>|80/80|50/50|50/50|
|USD/CAD|31.30|27.42|30_._00|8_._34×10<sup>−5</sup>|101/104|100/100|50/50|
|GBP/JPY|20.03|16.35|30_._00|1_._55×10<sup>−2</sup>|71/71|50/50|50/50|
|EUR/JPY|14.44|22.33|0_._60|8_._77×10<sup>−3</sup>|100/100|100/100|50/50|
|AUD/USD|32.77|28.23|32_._10|5_._65×10<sup>−5</sup>|113/114|100/100|50/50|
|AUD/JPY|31.08|23.27|40_._00|8_._62×10<sup>−3</sup>|88/90|94/99|50/50|



**Table 3** TWQ: time-weighted quantity available at best quotes. TQ: traded quantity. Q ask/bid: quantity posted at best ask rate and best bid rate. Period: 1 September 2019 to 29 February 2020 between 9.00am and 4.00pm. Each unit of quantity is for 10’000 units of the base currency 

|Currency pair|TWQ<br>ask/bid|TQ_>_TWQ|Median Q<br>ask/bid|TQ_>_MQ|Mode Q<br>ask/bid|TQ_>_MoQ|
|---|---|---|---|---|---|---|
|EUR/USD|160/159|0.1%|100/100|0.2%|50/50|4.6%|
|USD/JPY|110/109|0.1%|100/100|0.1%|50/50|2.6%|
|EUR/GBP|94/94|1.2%|80/82|1.4%|50/50|1.9%|
|GBP/USD|91/91|1.2%|75/66|1.5%|50/50|2.3%|
|USD/MXN|80/80|4.6%|50/50|5.9%|50/50|5.9%|
|USD/CAD|101/104|0.2%|100/100|0.2%|50/50|7.6%|
|GBP/JPY|71/71|0.7%|50/50|0.9%|50/50|0.9%|
|EUR/JPY|100/100|1.8%|100/100|0.2%|50/50|2.8%|
|AUD/USD|113/114|0.1%|100/100|0.1%|50/50|8.8%|
|AUD/JPY|88/90|1.5%|94/99|1.4%|50/50|0.1%|



pairs we study. The key message is that liquidity-taking orders hardly ever consume all the liquidity available at the best quotes in the LOB. 

Table 2 reports statistics of the liquidity-taking and the liquidity-provision activity during the period 1 September 2019 to 29 February 2020 between 9.00am and 4.00pm British summer time. Columns 2–4 describe the liquidity-taking activity. The mean, median and standard deviation are in 10’000 units of the base currency. For example, the mean value of the traded quantity in the pair EUR/USD is 20.94, which, in the base currency EUR, is a mean of 209’400 EUR. The last four columns of the table summarise statistics of the spread and of the best quotes in the LOB: the time-weighted spread for each currency pair, the time-weighted quantity, the median quantity available at the best quotes, and the mode quantity at the best quotes. 

Optimal execution with stochastic delay 

11 

Table 3 shows the percentage of occurrences when the traded quantity is greater than: the time-weighted quantity at the best quotes; the median quantity at the best quotes; and the mode of the quantity at the best quotes. Note that the information in columns 1, 3, 5 is the same as that in the last three columns of Table 2. 

We observe that in all currency pairs, the proportion of trades that cannot be filled with the liquidity available at the best quotes is between 0.1% and 8.8%. In the model that follows, we assume that the MLOs sent by the trader to the exchange will not walk the LOB. 

## **3 Optimal execution with random latency** 

We present a model of optimal execution with random latency. We focus on the execution of a large order that is divided in child orders that are sent to the market over a finite trading horizon. However, we remark that our model is useful to decide the optimal strategy to execute as little as one order over a trading horizon. Our discussion is framed within foreign exchange markets; however, our model is general and applicable in all asset classes in which instruments are traded in a visible electronic LOB. Also, our framework may be applied in general stochastic control problems in which the outcomes of the agent’s actions are known at a random future date. 

### **3.1 Exchange rate dynamics and execution of MLOs** 

As discussed above, flickers are short-lived deviations; so here we assume that only the flicker that occurs at the processing time _t_ ˜0 is relevant when the exchange processes the trader’s sell MLO. Therefore, the trader models the best bid rate _S_ ˆ = _(S_ ˆ _t )t_ ≥0 as 



where _S_ = _(St )t_ ≥0 denotes the fundamental best bid rate, and _F_ = _(Ft )t_ ≥0 denotes the flickers that affect the bid rate only at processing times of MLOs. The model (3.1) does not include flickers that occur between processing times of the MLOs sent by the trader – it models paths that are relevant for the trader. In the sequel, we refer to _S_ ˆ _t_ as the observed best bid rate, and in Sect. 4, we show how the trader employs the data of the LOB to obtain the fundamental best bid rate _St_ . 

Formally, the notification time of the order and the size of the flicker are modelled by the background marked point process (MPP) _N_ = _(Tn,Zn)n_ ≥1 with random measure p _(_ d _t,_ d _z)_ . The background MPP dictates the stopping time when the outcome of each order is notified to the trader and dictates the flicker that affects the fundamental bid rate when each order is processed. That is, the trader sends a sell MLO to the exchange at time _t_ and the exchange notifies the outcome of the order at time _t_ ˜, where _t_ ˜ is the next stopping time (after _t_ ) when a mark (i.e., flicker) of the MPP arrives. More precisely, let _n_ ≥ 1 be such that _Tn_ −1 ≤ _t < Tn_ ; then the notification time _t_ ˜ is _Tn_ and the flicker _Ft_ ˜ is _Zn_ . Note that the value _Ft_ ˜ of the flicker at the notification time is independent of the trader’s information at time _t_ . 

Á. Cartea, L. Sánchez-Betancourt 

12 

We assume that the background process _N_ is non-explosive and the interarrival times _(Ti_ − _Ti_ −1 _)i_ ≥1 are independent and identically distributed exponential random variables with parameter _λ >_ 0 and _T_ 0 = 0. The flickers _(Zn)n_ ≥1 that affect the fundamental best bid rate at processing times take values in R and are i.i.d. with law 



where _δ_ {0} _(_ d _z)_ is the Dirac measure at zero, _η_ + _,η_ − ∈ _(_ 0 _,_ ∞ _)_ and _p_ 0 _,p_ + _,p_ − ∈ R+ with _p_ 0 + _p_ + + _p_ − = 1. 

Let _T_ ∈ _(_ 0 _,_ ∞ _)_ . It is straightforward to see that 



The law in (3.2) is a modelling choice; the framework we develop here is valid for any law _ν(_ d _z)_ that satisfies the two conditions in (3.3). 

### **3.2 Outcome of trade attempts: miss or fill** 

The information flow that the trader observes is encoded in the filtration F = _(Ft )t_ ≥0 defined by 



where _(Ws)s_ ≥0 is a standard Brownian motion. Let _τ_ be an F-stopping time, and set _Nt_ = p _((_ 0 _,t_ ] _,_ R _)_ for _t_ ∈ _(_ 0 _,T_ ] and _N_ 0 = 0, where _Nt_ denotes the total number of marks of the background process _N_ up to time _t_ . We define the notification time _τ_ ˜ of a trade attempt by 



where _�Nt_ = _Nt_ − _Nt_ −. Thus _τ_ ˜ denotes the time when the exchange notifies the outcome of the trade attempt sent at time _τ < τ_ ˜. **Henceforth** , we put a tilde ˜ on stopping times to denote that they are notification times. 

The following lemma shows that: (i) the notification times are stopping times, and (ii) the times between trade attempts and notification times are i.i.d. with exponential distribution. 

**Lemma 3.1** _Let (τi)i_ ∈N _be a sequence of_ F _-stopping times_ , _where the index i denotes the ith trade attempt_ , _and let (τ_ ˜ _i)i_ ∈N _be the sequence of notification times of the outcome of each trade attempt_ . _Let (τi)i_ ∈N _and (τ_ ˜ _i)i_ ∈N _satisfy τ_ ˜ _i_ ≤ _τi_ +1 _for all i_ ∈ N. _Then_ : 

(i) _(τ_ ˜ _i)i_ ∈N _are_ F _-stopping times_ . 

(ii) _(τ_ ˜ _i_ − _τi)i_ ∈N _constitute a collection of independent and identically distributed random variables that are exponentially distributed with parameter λ >_ 0. 

Optimal execution with stochastic delay 

13 

**_Proof_** _(_ i _)_ Let _j_ ∈ N and _t_ ≥ 0. Then 



because { _τj_ ≤ _t_ } ∈ _Ft_ and { _Nt > Nt_ ∧ _τj_ } ∈ _Ft_ . _(_ ii _)_ For _i_ ∈ N and _t_ ≥ 0, we have 



because _(Nτi_ + _s_ − _Nτi )s_ ≥0 is a Poisson process with parameter _λ_ . Finally, the collection _(τ_ ˜ _i_ − _τi)i_ ∈N has the independence property because [ _τi,_ ˜ _τi)i_ ∈N are over nonoverlapping intervals. □ 

Next, we define the auxiliary process F = _(_ F _t )t_ ≥0 to determine the outcome of the trade attempts. The value of F _t_ is the most recent mark up to time _t_ . Thus F satisfies 



where we recall that p _(_ d _t,_ d _z)_ is the random measure of the background process _N_ . When the trader sends a trade at time _τ_ and the notification time is _τ_ ˜, the value _Fτ_ ˜ of the flicker is F _τ_ ˜ . 

At the notification time _τ_ ˜, if the sell MLO is an FoK with limit rate l, it will be filled in full if l ≤ _Sτ_ ˜− + F _τ_ ˜ ; otherwise the MLO misses the trade and is cancelled by the exchange. To formally define a miss and a fill of a trade attempt, we proceed as follows. Let _H(x)_<sup>˜</sup> = **1** { _x_ ≤0} be a step function. The outcome of the sell MLO is determined by the step function _H(_<sup>˜</sup> l − _Sτ_ ˜− − F _τ_ ˜ _)_ , which takes the value 1 when the trader receives the notification of a fill or the value 0 when the notification is of a miss. When the value of the flicker is positive and the MLO is filled, the liquidity providers that were offering liquidity at the best quote are likely to be adversely selected. Next, approximate _H_<sup>˜</sup> with a _C_<sup>∞</sup> function _f_<sup>_ε_</sup> , where the parameter _ε_ controls the convergence of _f_<sup>_ε_</sup> to _H_<sup>˜</sup> as _ε_ ↘ 0. For example, to determine the fill or miss outcome of a trade attempt, we use instead of _H(x)_<sup>˜</sup> the sigmoid function evaluated at − _x/ε_ . We recall that the sigmoid function is given by _S(x)_ = 1 _/(_ 1 + exp _(_ − _x))_ . We work with an approximation to the step function _H_<sup>˜</sup> to preserve the continuity of the impulse operator. **To simplify notation** , we drop the superscript _ε_ and refer to _f_<sup>_ε_</sup> as _f_ . 

### **3.3 Admissible strategies** 

The trader controls when to send MLOs and controls the limit rate of the MLOs; this is summarised in the execution strategy _α_ = _(τi,_ l _i)i_ ≥1. The limit rates _(_ l _i)i_ ≥1 of the MLOs are _Fτi_ -measurable and take values in a non-empty compact set _L_ ⊆ R. We assume that the trader does not attempt a trade until the outcome of the previous MLO 

Á. Cartea, L. Sánchez-Betancourt 

14 

is known. Thus we require that the F-stopping times in the sequence _(τi)i_ ≥1 satisfy _τi_ +1 ≥˜ _τi > τi_ for _i_ ∈ N. Specifically, the trader’s set of admissible strategies is 



If at the terminal time _T_ , there is an outstanding MLO in the exchange, the order is cancelled. The trader keeps track of pending orders in the exchange with the process _kt (α)_ (for notational convenience, we use _k(t,α)_ ), which returns the value 1 if the trader is waiting to be notified of the outcome of a trade attempt, or the value 0 if there is no pending order waiting to be processed by the exchange. 

Hence, _kt (α)_ = card{ _i_ ∈ N : _τi_ ≤ _t,_ ˜ _τi > t_ } ∈{0 _,_ 1}, which is adapted to F because for a fixed _t_ ∈[0 _,T_ ] and _i_ ∈ N, we have that _τi_ and _τ_ ˜ _i_ are F-stopping times. Thus { _τi_ ≤ _t_ } ∩{˜ _τi > t_ } ∈ _Ft_ . 

### **3.4 System dynamics** 

In our framework, MLOs have permanent price impact when the order is filled. Note that if the MLO is not filled, the trader is the only market participant who knows that the trade attempt was not successful; thus, missed MLOs do not have price impact. If market participants had access to the information on missed trades, they would learn about buy and sell pressure in the market and would adjust their liquidity-provision and liquidity-taking strategies. 

All trade attempts are of size one unit, which we refer to as child orders. The size of the parent order is M _>_ 0 child orders, which is the initial inventory the trader seeks to liquidate. If the limit rate of the sell MLO to execute a child order is less than or equal to the best bid rate, we assume that there is enough liquidity to fill the MLO without walking the book, in which case the trader is indifferent between sending an IoC or an FoK to the exchange. Thus we assume that trades do not have temporary price impact; it is straightforward to include this price impact in the price dynamics of our model. In Table 3, we see that the majority of liquidity-taking orders do not walk the LOB. Traders specify quantities that can be filled with the volumes displayed at the best bid and best ask rates in the market. 

For an execution strategy _α_ = _(τi,_ l _i)i_ ≥1 ∈ _A_ , the trader monitors the system 



where _(St_<sup>_α)t_≥0is the fundamental best bid rate process,</sup><sup>_(Qα_</sup> _t_<sup>_)t_≥0is the inventory of</sup> the agent, and _(Ct_<sup>_α)t_≥0 is the cash process. The dynamics of the fundamental best bid</sup> rate process are 



Optimal execution with stochastic delay 

15 

and the cash and the inventory process satisfy, respectively, 



In the fundamental bid rate in (3.4), the functions _b,σ_ : R → R are Lipschitzcontinuous. The function _b_ is the drift of the fundamental bid price and _σ_ is the volatility of the innovations. The last term on the right-hand side represents the permanent price impact that filled MLOs have on the fundamental best bid rate, where _κ_ ≥ 0 is the permanent impact parameter of the bid rate. Recall that _f_ is the _C_<sup>∞</sup> approximation of the step function that flags when a trade is filled or missed, and that all MLOs sent by the trader are of size one. We assume that the affected fundamental best bid rate _(St_<sup>_α)t_≥0is bounded by the unaffected fundamen-</sup> tal best bid rate _(St )t_ ≥0 as follows: for any execution strategy _α_ ∈ _A_ , we have that sup0≤ _t_ ≤ _T_ | _St_<sup>_α_| ≤sup</sup> 0≤ _t_ ≤ _T_<sup>|</sup><sup>_St_| +</sup><sup>_κNT_.Thisassumptionisnon-restrictive.Iftheaf-</sup> fected fundamental best bid rate is assumed to be always positive, then _St_<sup>_α_≤</sup><sup>_St_for</sup> _t_ ∈[0 _,T_ ] implies that sup0≤ _t_ ≤ _T_ | _St_<sup>_α_| ≤sup</sup> 0≤ _t_ ≤ _T_<sup>|</sup><sup>_St_| ≤sup</sup> 0≤ _t_ ≤ _T_<sup>|</sup><sup>_St_| +</sup><sup>_κNT_. Alter-</sup> natively, if _b,σ_ are constant functions, the assumption is also satisfied. The trader intervenes in the system at time _τi_ (i.e., sends a sell MLO), and due to latency, the outcome is known at the notification time _τ_ ˜ _i > τi_ when the system evolves from _Xτ_<sup>_α_</sup> ˜ _i_ −<sup>to</sup><sup>_X_</sup> _τ_<sup>_α_</sup> ˜ _i_<sup>=</sup><sup>_�(Xτ_˜</sup><sup>_i_−</sup><sup>_,_F</sup><sup>_τ_˜</sup><sup>_i,_l</sup><sup>_i)_. Here, the function</sup><sup>_�_: R3 × R ×</sup><sup>_L_→R3is the</sup> impulse operator 



which describes how the system (i.e., best bid rate, inventory, cash) changes at notification times. If the MLO sell order is filled: the first argument on the right-hand side of (3.5) shows that the fundamental bid rate decreases by one tick; the second argument shows that the inventory decreases by one unit; and the last argument shows that the amount of cash increases by the fundamental bid rate plus the flicker. If the sell MLO is not filled, the fundamental bid rate is not affected, and the cash and inventory positions do not change. 

We denote by | · | the Euclidean norm, and the operator _�_ is a continuous function that satisfies 



The initial state of the system is _X_ 0 = _(S_ 0 _,_ M _,_ 0 _)_ . Here, _S_ 0 is the initial value of the fundamental bid rate, we recall that M is the number of child orders (lots of equal size) that the trader wishes to liquidate, and the initial value of the cash account is 

Á. Cartea, L. Sánchez-Betancourt 

16 

zero. The controlled system _X_<sup>_α_</sup> is the solution to the SDE 



where **_b_** : R<sup>3</sup> → R<sup>3</sup> and **_σ_** : R<sup>3</sup> → R<sup>3</sup> are given by **_b_** _(x_ 1 _,x_ 2 _,x_ 3 _)_ = _(b(x_ 1 _),_ 0 _,_ 0 _)_ and **_σ_** _(x_ 1 _,x_ 2 _,x_ 3 _)_ = _(σ(x_ 1 _),_ 0 _,_ 0 _)_ , respectively, and both are Lipschitz-continuous. We now fix a finite horizon _T <_ ∞ and an execution strategy _α_ ∈ _A_ . It follows that 



a result we employ below to show that the value function of the trader is well defined. 

### **3.5 Liquidity taking with stochastic delay** 

For all _t_ ∈[0 _,T )_ , the agent has either one or no pending order in the exchange. Thus we define two sets of admissible strategies: (i) admissible strategies with one pending order, and (ii) admissible strategies with no pending order. If at time _t_ ∈[0 _,T )_ , there is one pending order with price limit _ℓ_ ∈ _L_ , then the set of admissible strategies is 



and if there is no pending order, the set of admissible strategies is 



For any _(t,x,_ f _)_ ∈[0 _,T_ ] × _(_ R × _(_ −∞ _,_ M] × R _)_ × R with _x_ = _(s,q,c)_ and for a pending order with limit price _ℓ_ ∈ _L_ and _α_ ∈ _At,ℓ_ , we denote by _X_<sup>_t,x,_f</sup><sup>_,ℓ,α_</sup> the solution to (3.7) for _t_ ≤ _s_ ≤ _T_ with initial data _Xt_ = _x_ and F _t_ = f. One can drop the dependence on f in _X_<sup>_t,x,ℓ,α_</sup> because for any f1 _,_ f2 ∈ R, we have F<sup>_t,_</sup> _t_ ˜<sup>f1</sup> = F<sup>_t,_</sup> _t_ ˜<sup>f2</sup> . That is, we write 



Similarly, when there is no pending order at time _t_ , we denote by _X_<sup>_t,x,α_</sup> the solution to (3.7) for _t_ ≤ _s_ ≤ _T_ with _Xt_ = _x_ , for any _(t,x)_ ∈[0 _,T_ ] × _(_ R × _(_ −∞ _,_ M] × R _)_ . 

Fix _(t,x)_ ∈[0 _,T_ ] × R × _(_ −∞ _,_ M] × R, _ℓ_ ∈ _L_ , _α_ 1 ∈ _At,ℓ_ and _α_ 0 ∈ _At_ . It follows from (3.6), Gronwall’s lemma, the Burkholder–Davis–Gundy inequality and (3.3) 

Optimal execution with stochastic delay 

17 

that there exists a constant _C >_ 0 such that 



The trader evaluates the performance of the execution strategy _α_ with the function 



where _g_ : R<sup>3</sup> → R, _h_ : R<sup>3</sup> → R, C : R<sup>4</sup> → R are 

_g(s,q,c)_ = _c_ + _q_ � _s_ + _ζ_ − _a(q_ − 1 _)_ � _(_ 1 − _ρ), h(s,q,c)_ = − _φq_<sup>2</sup> _,_ C _(s,q,c,_ f _,ℓ)_ = − _ρ(s_ + f _)f (ℓ_ − _s_ − f _)._ (3.9) 

Here, _a_ ≥ 0 is the terminal inventory penalty parameter, and _φ_ ≥ 0 is the running inventory penalty parameter. 

The function _g_ consists of three terms: (i) _c_ , the cash accumulated by the strategy, (ii) _q(s_ + _ζ)_ , the mark-to-market value of the inventory (net of fees), which includes the expected value _ζ_ = �R<sup>_zν(_d</sup><sup>_z)_oftheflicker,and(iii)</sup><sup>_aq(q_−1</sup><sup>_)_,thecostsof</sup> walking the LOB (net of fees) when the terminal inventory is greater than one child order. 

The function _h_ represents the urgency of the agent to complete the execution programme. Everything else being equal, the higher the value of _φ_ ≥ 0, the quicker will the trader liquidate inventory at the beginning of the trading interval. Each trader decides the urgency of their trade or set of trades. For example, the value of the urgency parameter is arbitrarily high for a trader who requires immediate execution. On the other hand, a patient liquidity taker who has time to retry missed trades will set the value of _φ_ close to or at zero. 

The function C represents transaction costs per unit of inventory traded. These costs are based on the notional traded, which is _(s_ + f _)f (_ l − _s_ − f _)_ per one unit of inventory, and _ρ_ ∈ _(_ 0 _,_ 1 _)_ is the transaction cost parameter. 

The controlled processes _(Ct_<sup>_α)_0≤</sup><sup>_t_≤</sup><sup>_T_,</sup><sup>_(S_</sup> _t_<sup>_α)_0≤</sup><sup>_t_≤</sup><sup>_T_and</sup><sup>_(Qα_</sup> _t_<sup>_)_0≤</sup><sup>_t_≤</sup><sup>_T_obey the bounds,</sup> which do not depend on the strategy _α_ or _t_ ∈[0 _,T_ ], 



Á. Cartea, L. Sánchez-Betancourt 

18 

Furthermore, we see that 



which ensures that the value functions we present below are well defined, i.e., they are finite as a consequence of the inequality above and (3.8) and (3.10). 

When there is one pending order in the exchange, the trader’s performance criterion and value function are 



and 



respectively, for _(t,x)_ ∈[0 _,T_ ] × R × _(_ −∞ _,_ M] × R, _ℓ_ ∈ _L_ , _α_ ∈ _At,ℓ_ . 

Similarly, when the trader does not have an order pending in the exchange, we have 



for _(t,x)_ ∈[0 _,T_ ] × R × _(_ −∞ _,_ M] × R, _α_ ∈ _At_ , with the corresponding value function 



In Appendix A, we show in Theorem A.1 that the trader’s value functions satisfy the dynamic programming principle (DPP). The part of the proof that deals with measurable selection arguments is omitted (see Bouchard and Touzi [8] for the derivation of the dynamic programming equation in the sense of viscosity solutions). In Sect. A.1, we derive the HJBQVI satisfied by the value functions, and in Sect. A.2, we study the viscosity properties of the value functions. 

### **3.6 Benchmarks** 

In Appendix B, we develop two new strategies to benchmark the performance of the random-latency-optimal trading strategy. Throughout, most of the notation is the same as in the previous sections. We derive optimal execution strategies when latency is zero in the marketplace, and when latency is greater than zero and deterministic, respectively. In both cases, the trader solves an impulse control problem. In the first model, the strategy determines the optimal times to send MOs to the exchange, and in the second, the strategy determines the timing and the limit rate of each MLO. We remark that most approaches in the execution literature assume that agents send MOs at a continuous rate. Although agents cannot continuously trade in the market, 

Optimal execution with stochastic delay 

19 

this assumption is convenient because in some cases one finds strategies in closed form; see Cartea et al. [12, Sect. 6] and Guéant [20, Part II]. One notable exception is the paper by Cartea and Jaimungal [10], where the authors solve an impulse control problem in which the trader employs both MOs and limit orders to execute a large position in a financial instrument when there is zero latency in the marketplace. 

## **4 Performance of execution strategy** 

We employ ultra-high-frequency market data for the currency pair EUR/USD to compare the performance of the random-latency-optimal strategy (RLOS) developed in Sect. 3 with four benchmarks (of which the first two are characterised in Appendix B): (i) deterministic-latency-optimal strategy (DLOS), (ii) zero-latencyoptimal strategy (ZLOS), (iii) time-weighted-average-price (TWAP), and (iv) execution now (ENOW). (RLOS, DLOS and ZLOS are computed using finite differences. Convergence results for these algorithms remain to be studied.) TWAP sends MOs of equal size and at equally spaced time intervals over the trading window. ENOW is a hypothetical benchmark that assumes there is enough liquidity at the best bid rate in the LOB to execute all the inventory with one MO, with zero latency, at the beginning of the trading window. 

Recall that in our formulation, the fundamental bid rate _St_ is an input to RLOS, DLOS, ZLOS. However, market participants observe the best bid rate posted on the LOB of the exchange, a rate that consists of the fundamental bid rate _St_ and shortlived deviations. Here, we employ LOB observations of the best bid rates and of the quoted spreads in the LOB of the exchange to estimate the fundamental bid rate _St_ . In Sect. 2.3, the bottom panel of Fig. 1 depicted the evolution of the best rates in the LOB of the EUR/USD currency pair, where we saw that most flickers cause shortlived widening of the quoted spread: most flickers in the best bid rate are positive, and most flickers in the best ask rate are negative. Thus we assume here that at time _t_ , the fundamental best bid rate _St_ is the bid rate in the LOB the last time the spread in the LOB was less than _υ_ ticks. **Throughout** , we assume that _υ_ = 11 ticks – our results are robust to various choices of the value of _υ_ . 

The trader’s execution horizon is _T_ = 6 seconds, and the inventory to liquidate is M = 10 lots, where each lot is €500’000; so the objective is to exchange €5’000’000 into USD. The execution value of ENOW in a frictionless market is the value of M lots exchanged at the best bid rate in the LOB at the start of the trading horizon. For TWAP and ZLOS, the MO for each lot is processed with the liquidity posted at the best bid rate in the LOB of the exchange, and if necessary, the order will walk down until filled in full. Similarly, if the limit rate of the MLO allows it, RLOS and DLOS will also walk down the LOB if the liquidity in the best quotes is not enough – we do so even though the strategies characterised in this paper assume that orders of size one do not walk the LOB. 

To compute RLOS, DLOS, ZLOS, we assume that the fundamental best bid rate satisfies 



Á. Cartea, L. Sánchez-Betancourt 

20 

where _σ_ is a volatility parameter. However, we remark that when we implement the liquidation strategies, we do not simulate the fundamental best bid rate and do not simulate the flickers that affect the MLO when it is processed by the exchange. Instead, we use the best bid rate in the LOB of LMAX. The only aspect of the model that we simulate is the latency of the trader that employs the strategy – in Sect. 4.5, we perform robustness checks with respect to latency. Thus the results we report are robust to model and parameter misspecification. We assume that our trades do not impact the dynamics of the LOB. Ideally, one would like to test the model when other market participants react to the activity generated by the latency-optimal strategy; however, it is not possible to endogenise the reaction of other market participants to our trades. 

We employ data from 1 August 2019 to 31 August 2019 to estimate the parameters of the model, and data between 1 September 2019 and 29 February 2020 to implement and compute the performance of RLOS and the benchmark strategies. Each day, we implement the liquidation strategy over three ten-minute windows. We use the data in the ten minutes starting at 9.00am, at 2.00pm and at 7.00pm for each trading day between 1 September 2019 and 29 February 2020. Times reported in this paper are in London time. At 9.00am and 2.00pm, the foreign exchange activity tends to be very high because by 9.00am, many financial markets in London are open and very active, and many financial markets in the US open at around 2.00pm. Later in the afternoon, foreign exchange activity decreases, and by 7.00pm, many markets worldwide are outside the hours of their main trading activity. 

### **4.1 Model parameters** 

We employ data from August 2019 to estimate the parameters of the model as follows: 

(i) _Volatility of fundamental best bid rate dynamics_ : We compute the quadratic variation of the fundamental best bid rate to estimate the volatility parameter _σ_ in (4.1). The estimates over the three ten-minute windows are: (9.00am to 9.10am) _σ_ ˆ9am = 2 _._ 1 × 10<sup>−4</sup> ; (2.00pm to 2.10pm) _σ_ ˆ2pm = 2 _._ 9 × 10<sup>−4</sup> ; (7.00pm to 7.10pm) _σ_ ˆ7pm = 1 _._ 7 × 10<sup>−4</sup> . The estimate of the volatility from 9.00am to 8.00pm is given by _σ_ ˆ = 1 _._ 9 × 10<sup>−4</sup> ; we use this parameter estimate in Sect. 4.4 when trading around news events. 

(ii) _Distribution of the size of the flicker that affects the MLOs at processing times_ : Recall that the law of the flickers is 



Agents use their trade activity to estimate their expected latency in the marketplace; here, we assume that their estimate is one of 10, 30, 60, 90 ms. Next, for a given expected latency and for a value of _υ_ to obtain the fundamental best bid rate, agents estimate the parameters of the distribution of flickers as follows: employ LOB data to study the hit and miss of MLOs with limit rates equal to the fundamental best bid rate when latency is exponentially distributed; use ten minutes of LOB data (beginning at 

Optimal execution with stochastic delay 

21 

**Table 4** Trading window 9.00am to 9.10am. Fundamental best bid rate is obtained with spread filter of _υ_ = 11 ticks. EL stands for expected latency 

|ˆ_σ_9am<br>2_._1×10<sup>−4</sup>|ˆ_p_+<br>0.05|ˆ_p_−<br>0.08|ˆ_η_+, ˆ_η_−<br>{1_._87_,_2_._66} ×10<sup>−5</sup>|ˆ_a_<br>1_._80×10<sup>−5</sup>|EL<br>30 ms|_φ_<br>_κ_<br>0<br>0|c<br>3×10<sup>−6</sup>|
|---|---|---|---|---|---|---|---|



9.00am, 2.00pm and 7.00pm) of each trading day in August to simulate over one million MLOs; compute the SPIs of the MLOs; and finally, obtain maximum likelihood estimates of the parameters _p_ 0 _,p_ + _,p_ − _,η_ + _,η_ − for each ten-minute trading interval and for each expected latency. 

(iii) _Terminal penalty_ : We employ the liquidity-taking trades that walk the LOB to compute the value of the terminal penalty parameter _a_ . For each order, we look at the excess traded quantity above the liquidity posted at the best quotes and perform a linear regression where the explanatory variable is the excess traded quantity, bought or sold, and the dependent variable is the excess rate paid or received in the quote currency for walking the book. The slope of the regression for data in August 2019 is _a_ ˆ = 1 _._ 80 × 10<sup>−5</sup> , which the agent uses to compute the optimal strategies. On the other hand, to test the performance of the strategy from September 2019 to February 2020, we use _a_ ˆ = 1 _._ 61 × 10<sup>−5</sup> , which is the in-sample estimate, and we remark that the trader does not know this value. 

We assume that the value of the permanent price impact parameter _κ_ is zero because we use LOB data to run the simulations; so we cannot include the permanent impact that the trader’s orders would have had on the market. Recall that in the implementations, the MLOs walk the book if necessary (while obeying the price limit), and the strategies incorporate the fees paid to the exchange. Thus we account for all trading frictions except the friction arising from the permanent price impact. The central scenarios we study assume that the liquidity trader is patient; so we set the value of the urgency parameter _φ_ to zero. Finally, the transaction cost parameter is c = 3 × 10<sup>−6</sup> , i.e., 3 USD per million USD traded, which are the fees paid by active traders in the foreign exchange market. Table 4 summarises the model parameters for RLOS, DLOS, ZLOS to trade between 9.00am and 9.10am when expected latency is 30 ms. In the interest of space, we do not report the parameters for the other scenarios we study. 

### **4.2 Simulations and performance measure** 

There are 128 days of market data in the period from 1 September 2019 to 29 February 2020. For each day, we split the ten-minute window into 100 intervals of six seconds. For each interval of six seconds, we combine the one market run of the best bid rate posted on the LOB with 100 simulated runs of the random latency that the trader would face in each trade attempt. Thus the total number of runs for each ten-minute window (9.00am to 9.10am, 2.00pm to 2.10pm, 7.00pm to 7.10pm) is 128 × 100 × 100 = 1’280’000. In each run, the strategy exchanges 5’000’000 EUR into USD, which is 10 times the volume that one normally observes posted at the best bid rate in the exchange, i.e., M = 10 lots of 500’000 EUR each. 

Á. Cartea, L. Sánchez-Betancourt 

22 

For RLOS, DLOS, ZLOS, we recall that the cost of liquidating terminal inventory with one MO is as follows: if _QT_ − = 1 lot, the order does not walk the LOB. However, if _QT_ ≥ 2, the first lot does not walk the book, and the remaining _QT_ − 1 lots walk the LOB, so that the USD received from exchanging the final position in EUR is _QT_ − _(ST_ + _ζ_ − _a(QT_ − − 1 _))(_ 1 − _ρ)_ ; see (3.9). 

The measure of performance is 



where TC _T_ is the terminal cash in USD obtained by the strategy, _V_ 0 is the value of the position to exchange at the beginning of the trading window, and B represents one of the four benchmark strategies. The numerator of (4.2) is in USD and the denominator is in EUR; so the units of the performance measure are $/€M, i.e., USD per million of EUR exchanged. Finally, when the benchmark is ENOW, the performance measure is ‘implementation shortfall’, which is a widely used benchmark to assess implicit costs and the liquidity of markets. ENOW is seldom achieved because large orders need much more than the liquidity available at the best quotes of the LOB; see Almgren [3] and Cartea et al. [12, Sect. 4]. Generally, one computes implementation shortfall with the midprice. Here, we use the best bid rate for a liquidation problem. 

### **4.3 Results: performance of RLOS and benchmarks** 

In this subsection, we present the results of the execution strategy for the three tenminute trading windows that start at 9.00am, 2.00pm and 7.00pm for various levels of the trader’s expected latency in the marketplace. Later, in Sect. 4.4, we examine the performance of the strategies around news announcements. The main finding we report is the superior performance of the latency-optimal strategies RLOS and DLOS. Both RLOS and DLOS perform similarly; we do not find evidence at 95% confidence to reject the hypothesis that their mean performance is the same. The outperformance of RLOS over DLOS is positive in most scenarios, but is always less than $0.5/€M in absolute value. RLOS and DLOS produce similar performances because the optimal limit rates coincide for most of the simulations when rounded to the nearest tick size. The cash value of the outperformance is approximately the same as the fees paid by traders in foreign exchange markets; the value of the outperformance considerably increases during news events. The outperformance stems from the speculative MLOs sent by RLOS and DLOS, which are more valuable when the market is more active. 

There are two key insights. First, the performance of MLOs depends on the distribution of the flickers. When activity in the markets increases, the value of MLOs increases because both the probability of receiving a price improvement and the size of the improvements increase – the value of MLOs is highest during news events. On the other hand, an increase in either the size or the probability of slippage during times of heightened activity has little effect on the performance of latency-optimal strategies because the rate limits in the sell MLOs prevent orders from walking down the LOB. This is in contrast with strategies that send MOs (e.g. TWAP and ZLOS) because these orders are exposed to adverse changes in the rates. Second, traders use speed to complete the execution programme with as many MLOs as possible because 

Optimal execution with stochastic delay 

23 

**Table 5** Values of performance measure (4.2) from 9.00am to 9.10am, _T_ = 6 seconds, _φ_ = 0. The revenues from RLOS and from DLOS (not shown) are statistically the same 

||**Performance 9.00am–9.1**<br>Expected latency in ms|**0am**||
|---|---|---|---|
||10<br>30|60|90|
|**ZLOS**|4.56<br>3.40|2.74|2.34|
|**TWAP**|5.14<br>3.80|3.09|2.65|
|**ENOW**|3.91<br>2.57|1.86|1.42|



they can retry missed speculative MLOs before reaching the end of the trading interval. Thus the lower the latency of the trader, the more opportunities the trader will have to fill speculative MLOs, so the better is the performance of latency-optimal strategies that use MLOs over strategies that employ MOs. 

Cartea and Sánchez-Betancourt [15] estimate the range of the stochastic latency of an active trader in the foreign exchange market to be between 10 and 50 ms. Table 5 reports the results of the performance measure (4.2) over the window 9.00am to 9.10am for a patient trader with 10, 30, 60, 90 ms of expected latency. RLOS receives approximately between 5.14 and 2.65 USD more than TWAP for every million EUR exchanged into USD, and RLOS receives between 4.56 and 2.34 USD more than ZLOS per million EUR exchanged, and RLOS outperforms ENOW by between 1.42 and 3.91 USD per million EUR exchanged. Finally, the average USD amount that RLOS receives is statistically the same as that received by DLOS, so the table does not report the results for DLOS. Here, two means are statistically the same if the Student _t_ -test does not reject the hypothesis that the two means are the same with 95% confidence. All outperformances we report are statistically different from zero. For example, in Table 5, the outperformance of RLOS over TWAP when the expected latency is 30 ms is $3.80/€M. Here, we reject the null hypothesis that the two means are the same with a _p_ -value of 2 _._ 15 × 10<sup>−5</sup> . 

The results in Table 5 show that the value of the outperformance of RLOS (and DLOS) decreases as the value of the expected latency increases – we provide two stylised facts of the strategies to explain the results in the table. First, as expected latency increases, it is more likely that at the end of the trading interval, all strategies will fall short of liquidating the target inventory. In the extreme case where expected latency is arbitrarily large, it is very likely that by the terminal time _T_ , most of the target inventory will remain in euros, i.e., is not exchanged into USD. Thus at time _T_ , the remaining lots of currency pairs will be liquidated with one MO, where the trader pays the costs of walking the book; the costs are the same for all strategies (except for ENOW). 

Second, for a fixed trading window and for a fixed liquidation target, faster traders (i.e., with lower expected latency) have on average more opportunities than slow traders to send MLOs to the exchange (recall that in our model, the trader cannot send another trade if her previous order is pending in the exchange). Therefore, compared with slow traders, faster traders send more (and fill more) MLOs that seek a rate improvement, which explains the results we report in the table – we return to this point below. In an extreme case, a patient trader with nearly zero latency will use her superior speed advantage to send a large amount of MLOs that seek price 

Á. Cartea, L. Sánchez-Betancourt 

24 

**Table 6** Expected latency is 30 ms, _T_ = 6 seconds, so expected number of attempts is 200, and _φ_ = 0. LR: limit rate of MLO. _St_ : fundamental best bid rate when the trader decides to send the MLO 

||**Expected latency**|**is 30 ms, start ti**|**me 9.00am**||||
|---|---|---|---|---|---|---|
||MLO Attempts|MLO Fills|_QT_−≥2|LR_< St_|LR=_St_|LR_> St_|
|**RLOS**|195_._07|9_._95|0.00|1_._00|5.71|188.35|
|**DLOS**|197_._44|9_._64|0.07|1_._26|5.01|191.17|
|**ZLOS**|10_._00|10_._00|0.00|10_._00|–|–|
|**TWAP**|10_._00|10_._00|0.00|10_._00|–|–|
|**ENOW**|1_._00|1_._00|0.00|–|1.00|–|



improvements, and due to the flickers in the best bid rate in the LOB, the trader will exchange a large proportion of the initial inventory in EUR into USD with these speculative MLOs. A very fast and very patient trader will not employ her speed in the marketplace to liquidate the target inventory quickly and ahead of time; the trader will use her low latency to fill as many speculative trades as possible to complete the execution target. 

Our results show that the slower the trader, the less she benefits from latencyoptimal strategies. However, on the other hand, an implication of our findings is that slow traders may profit from investing in co-location, software and other services to increase their speed in the marketplace if the improvement in the performance of the execution strategies outweighs the costs to reduce their latency, which largely depends on the volumes traded. 

To gain further insights, we report summary statistics of the behaviour of the strategies from 9.00am to 9.10am when expected latency is 30 ms. Table 6 reports: the average number of trade attempts (there are 200 periods of 30 ms for an execution horizon of six seconds); the average number of filled orders; the proportion of runs for which the strategy sends an MO at time _T_ to liquidate the terminal inventory and pays extra costs from walking the LOB (i.e., average number of runs for which _QT_ − ≥ 2); the proportion of the number of attempts for which the limit rate (LR) of the MLO is lower than the fundamental best bid rate _St_ (the trader is willing to walk the LOB to complete the trade); the average number of attempts for which LR = _St_ , and the average number of attempts where LR _> St_ , which are speculative trades that target a rate improvement (relative to the fundamental exchange rate). 

On average, RLOS and DLOS employ a similar number of MLO attempts, of which RLOS fills 9.95 and DLOS fills 9.65 (these figures exclude the MOs sent, if necessary, at time _T_ ). Approximately 97% of the attempted MLOs by both strategies are speculative, of which 2.4% (2.3%) sent by RLOS (DLOS) were filled. As discussed above, filled speculative MLOs are the main source of the superior performance of latency-optimal strategies. Within the trading window, the limit rates of the speculative MLOs become less ambitious if the inventory is not on target and the terminal date approaches. However, the latency-optimal strategies cannot guarantee that at time _T_ , the inventory in EUR is drawn to _QT_ − _<_ 2. The table shows that in approximately 0.3% (6.9%) of the runs, RLOS (DLOS) arrives at the end of the trading horizon with two or more lots that are liquidated with one MO. Finally, the 

Optimal execution with stochastic delay 

25 

**Table 7** Expected latency is 30 ms, _T_ = 6 seconds, so expected number of attempts is 200, and _φ_ = 10<sup>−3</sup> . LR: limit rate of MLO. _St_ : fundamental best bid rate when the trader decides to send the MLO 

||**Expected latency**|**is 30 ms, start ti**|**me 9.00am,**_φ_|=10<sup>−3</sup>|||
|---|---|---|---|---|---|---|
||MLO Attempts|MLO Fills|_QT_−≥2|LR_< St_|LR=_St_|LR_> St_|
|**RLOS**|10_._20|10_._00|0.00|10.20|–|–|
|**DLOS**|10_._21|10_._00|0.00|10.21|–|–|
|**ZLOS**|10_._00|10_._00|0.00|10.00|–|–|
|**TWAP**|10_._00|10_._00|0.00|10.00|–|–|
|**ENOW**|1_._00|1_._00|0.00|–|1.00|–|



percentage of runs where RLOS outperforms is 57% for DLOS, 61% for ZLOS, 67% for TWAP and 55% for ENOW. 

As Table 6 reports, when the value of the urgency parameter _φ_ is zero, RLOS and DLOS miss approximately 95% of attempts with MLOs – the majority of the MLOs seek a price improvement that does not materialise. To examine the role of the urgency parameter, we implement the execution strategies described above for a range of values of _φ_ and report a summary of the findings for _φ_ = 5 × 10<sup>−5</sup> and _φ_ = 1 × 10<sup>−3</sup> . These two choices represent impatient liquidity takers who have more urgency to liquidate the position than the patient trader with _φ_ = 0. 

When _φ_ = 1 × 10<sup>−4</sup> , RLOS misses 85% of the MLOs and outperforms TWAP by 1.68 USD per million euro exchanged. Similarly, when _φ_ = 1 × 10<sup>−3</sup> , RLOS misses 2% of the MLOs and outperforms TWAP by 0.79 USD per million euro exchanged. The performance of latency-optimal strategies is similar to that of TWAP because the urgency with which RLOS must liquidate inventory precludes the strategy from sending speculative trades, and because the limit rates of the MLOs are set low to increase the probability of a fill at the expense of price protection. In Table 7, we observe that RLOS sends nearly all MLOs with a limit rate below the fundamental best bid rate to ensure a prompt liquidation and does not send speculative MLOs – due to the trader’s impatience, in 99.97% of the simulations, the strategy completes the liquidation early. These results are in stark contrast with those reported in Table 6, where the trader was patient and used the entire window to complete the execution programme with as many speculative MLOs as possible, and where the majority of orders sent are retries of missed speculative MLOs. 

When we amalgamate the MLOs of all traders in our data set between 1 September 2019 and 29 February 2020 in the EUR/USD currency pair, we find that 57.1% of MLOs sent by all traders missed the trade. On the other hand, when we look at the MLOs sent by each trader, we find that approximately 35% of the traders miss up to 25% of their MLOs, and approximately 6% of traders miss between 75% and 100% of their MLOs – these traders also send other types of orders to the exchange (e.g. MOs), see Table 1. Finally, one cannot tell from the data which trades are part of an execution programme (i.e., child orders) or which trades are a single execution that is not part of a larger order. However, as discussed above, our model is designed to execute one stand-alone order or to execute a large parent order that is split into child orders. 

Á. Cartea, L. Sánchez-Betancourt 

26 

**Table 8** Values of performance measure (4.2) from 9.00am to 9.10am (first column), from 2.00pm to 2.10pm (second column), and from 7.00pm to 7.10pm (third column), _T_ = 6 seconds, and expected latency is 30 ms 

||**Performance mea**|**sure**(4.2)||
|---|---|---|---|
||9.00am to 9.10am|2.00pm to 2.10pm|7.00pm to 7.10pm|
|**ZLOS**|3.40|3.36|1.93|
|**TWAP**|3.80|3.51|2.25|
|**ENOW**|2.57|2.84|1.17|



**Table 9** Values of performance measure (4.2) starting at 9.00am, _T_ = 6 seconds, and _φ_ = 0 

||**Trader assumes expected latency of 30 ms**<br>**True expected latency in marketplace is:**|
|---|---|
||20 ms<br>30 ms<br>40 ms|
|**ZLOS**|3.73<br>3.39<br>3.14|
|**TWAP**|4.20<br>3.79<br>3.50|
|**ENOW**|2.97<br>2.55<br>2.27|



In the remainder of this subsection, we focus on a trader with 30 ms of expected latency. Table 8 reports the performance measure when the ten-minute execution windows start at 9.00am, 2.00pm and 7.00pm. We find that the outperformance of RLOS over ZLOS, TWAP, ENOW is worse when the start time is 7.00pm, which is the least active period we study and one for which the probability of positive flickers in the best bid rate is lowest, so MLOs are less valuable. 

Next, we study the performance of RLOS when the trader estimates 30 ms of expected latency in the market, but her latency is one of 20 ms (underestimated speed in the marketplace), 30 ms (correct speed estimate), 40 ms (overestimated speed in the marketplace). The results, reported in Table 9, show that the value of the outperformance of the RLOS and DLOS over the other strategies is slightly higher (lower) when the agent underestimates (overestimates) expected latency. Recall that over the trading window, for a lower (higher) value of the expected latency, the trader has more (fewer) attempts on average to execute her inventory. 

### **4.4 Trading around news events** 

Throughout the calendar year, many pieces of news are released according to a schedule. Market participants know the timing of the release, but do not know the content. In general, trade activity and volatility of the exchange rates tend to increase around the time of the news release. Here, we employ the EUR/USD news event information provided by FXStreet between 1 September 2019 and 29 February 2020 (see https:// www.fxstreet.com/economic-calendar). During this period, there are 117 news events marked as high impact for EUR and for USD, and the time of the release of each event is timestamped with precision up to one minute. 

We use the estimate _σ_ ˆ = 1 _._ 9 × 10<sup>−4</sup> , which is the volatility of the fundamental best bid between 9.00am to 8.00pm (see Sect. 4.1), to compute the trading strategies around the time of the arrival of the scheduled news; results are robust to employing volatility estimates computed with the data around news events in August 2019. The execution horizon is _T_ = 6 seconds and the agent trades during 1, 3, 5, 7 and 

Optimal execution with stochastic delay 

27 

**Table 10** Performance measure around news events, _T_ = 6 seconds, _φ_ = 0, and expected latency is 30 ms 

||**News ev**<br>Minutes|**ents: Perfo**<br>around new|**rmance mea**<br>s announcem|**sure**(4.2)<br>ents:||
|---|---|---|---|---|---|
||1 min|3 mins|5 mins|7 mins|10 mins|
|**ZLOS**|36.31|15.16|10_._62|8.66|7.21|
|**TWAP**|29.45|13.35|9_._62|8.12|6.92|
|**ENOW**|23.86|10.23|7_._28|6.06|5.15|



**Table 11** Period is from 1 September 2019 to 29 February 2020. Expected latency is 30ms 

||**Parameter     l**<br>ˆ_p_−<br>slippage|**s of the dist  l**<br>ˆ_p_0 on<br>target|**ribution of flickers (ex-p**<br>ˆ_p_+<br>price improvement|**l ost)**<br>1_/_ˆ_η_−<br>slippage|1_/_ˆ_η_+<br>price improvement|
|---|---|---|---|---|---|
|**9am**|0.057|0.901|0.042|2_._68×10<sup>−5</sup>|1_._92×10<sup>−5</sup>|
|**2pm**|0.064|0.889|0.047|2_._66×10<sup>−5</sup>|2_._04×10<sup>−5</sup>|
|**7pm**|0.054|0.923|0.023|2_._81×10<sup>−5</sup>|2_._00×10<sup>−5</sup>|
|**News**1 min|0.229|0.715|0.056|1_._62×10<sup>−4</sup>|2_._52×10<sup>−5</sup>|
|**News**3 min|0.158|0.792|0.050|1_._08×10<sup>−4</sup>|2_._25×10<sup>−5</sup>|
|**News**5 min|0.131|0.821|0.047|8_._92×10<sup>−5</sup>|2_._18×10<sup>−5</sup>|
|**News**7 min|0.118|0.835|0.047|7_._94×10<sup>−5</sup>|2_._14×10<sup>−5</sup>|
|**News**10 min|0.107|0.844|0.049|6_._89×10<sup>−5</sup>|2_._12×10<sup>−5</sup>|



10 minutes around the news event. For example, the ISM manufacturing index was scheduled to be released at 2.00pm on 3 September 2019; so for the one-minute window, we employ market data from 1.59.30pm to 1.59.36pm for the first simulation, data from 1.59.36pm to 1.59.42pm for the second simulation, and so on, until the tenth simulation from 2.00.24pm to 2.00.30pm. For each six-second execution interval, we perform 100 simulations of the random latency faced by the trader. Thus for the one-minute window, the study consists of 117 × 10 × 100 = 117’000 simulations. Table 10 reports the results of the performance measure in (4.2). 

The value of the outperformance of RLOS over ZLOS and TWAP is approximately between two and ten times the value reported in Table 5. This significant improvement in the performance of RLOS follows from an increase in the number of speculative MLOs that are filled. Table 11 shows the ex-post parameters of the distribution of flickers faced by the trader with 30 ms of expected latency. It is clear that during news announcements, the value of speculative MLOs is higher because the probability of a price improvement (respectively, slippage) and the size of the positive (respectively, negative) flickers are greater than those of the distribution of flickers for the tenminute trading windows that start at 9.00am, 2.00pm and 7.00pm. 

### **4.5 Robustness checks** 

In the results we showed above, the fundamental best bid rate was the current or most recent observed best bid rate when the spread was less than _υ_ = 11 ticks; recall that one tick is 10<sup>−5</sup> USD. Table 12 shows that the outperformance of RLOS over 

Á. Cartea, L. Sánchez-Betancourt 

28 

|**Table 12** Performance measure<br>from 9.00am to 9.10am,_T_ =6||**Filter w**|**ith**_ν_ **ticks t**|**o obtain fun**|**damental p**|**rice**|
|---|---|---|---|---|---|---|
|seconds, and expected latency<br>30 ms||7|9|11|13|∞|
||**ZLOS**|3.03|3.45|3.61|3.55|3.34|
||**TWAP**|3.29|3.71|3.87|3.81|3.60|
||**ENOW**|1.89|2.31|2.47|2.41|2.20|



the benchmarks is robust for _υ_ ∈{7 _,_ 9 _,_ 11 _,_ 13 _,_ ∞} ticks, where expected latency is 30 ms. Note that when _υ_ = ∞ ticks, the fundamental best bid rate is the observed bid rate, i.e., _S_<sup>ˆ</sup> _t_ = _St_ . 

The value of the outperformance of RLOS and DLOS over other benchmarks peaks for _υ_ = 11 ticks. In addition, we note that for the outperformance of RLOS over TWAP, any two consecutive results are statistically the same. For example, the outperformance of RLOS over TWAP for _υ_ = 9 and _υ_ = 11 are statistically the same. Finally, the performances of RLOS and DLOS are robust to misspecification of the volatility parameter of the fundamental best rate. We ran simulations with volatility estimates _σ_ ∈{1 _,_ 2 _,_ 3}× 10<sup>−4</sup> and expected latency 30 ms. The performance measure are as those reported above plus minus $0.4/€M. 

## **5 Conclusions** 

We have solved a general stochastic impulse control problem in which there is a stochastic delay between the action and its outcome. As an application, an investor liquidates a large position in a financial instrument when there is latency in the marketplace. We have derived the optimal strategies for stochastic and for deterministic latencies, and compared their performance with that of three benchmarks, including TWAP. During normal trading hours, we have found that the latency-optimal strategies outperform the benchmarks by an amount similar to that of the fees paid by traders in the foreign exchange market. Around news events, the value of the outperformance increases to between two and ten times the value of the fees. The superior performance of the latency-optimal strategies stems from both the speculative MLOs sent by the trader and the rate protection provided by the MLOs. We have shown that fast and patient traders use their superior speed to execute the target inventory with as many speculative MLOs as possible; they do not use their speed advantage to finish the execution programme early, or to hit the best quotes they observe in the LOB. 

In financial applications, interesting research problems include a study of the effect that latency has on the financial performance of electronic trading strategies, e.g. market making, pairs trading and other statistical arbitrage strategies. In particular, one could try to extend the recent works of Kalsi et al. [23] and Cartea et al. [14] to account for latency. In these works, the authors use techniques from rough path theory to solve relevant algorithmic trading problems, where it is assumed that there is no delay between trade attempts and executions. Finally, from a mathematical point of view, a challenging problem is to extend the current framework to include other distributions of the delay in the marketplace, and to extend the model so that there can be more than one pending order in the system when latency is stochastic. 

Optimal execution with stochastic delay 

29 

## **Appendix A: Random latency** 

To simplify notation, we let _L_ 0 = ∅ and _L_ 1 = _L_ , and we drop the argument of a mathematical object if it is the empty set. For example, we write _At_ instead of _At,_ ∅, or _X_<sup>_t,x,α_</sup> instead of _X_<sup>_t,x,_∅</sup><sup>_,α_</sup> , etc. Finally, when the index of a function is the empty set, the function returns the empty set. For _k_ ∈{0 _,_ 1}, the domains of the value functions _vk_ are 

_Dk_ = { _(t,x,ℓ)_ : _(t,x)_ ∈[0 _,T )_ × R × _(_ −∞ _,_ M] × R and _ℓ_ ∈ _Lk_ } _._ 

Here, we prove the DPP satisfied by _vk(t,x,ℓ)_ , _k_ ∈{0 _,_ 1}, _t_ ∈[0 _,T_ ] and _ℓ_ ∈ _Lk_ . We introduce the function _ι(t,α)_ which returns the index of the pending order (if any) at time _t_ and strategy _α_ . That is, 



**Theorem A.1** _The value functions in_ (3.11) _and_ (3.12) _satisfy the dynamic programming principle_ ( _DPP_ ). _That is_ , _for k_ ∈{0 _,_ 1}, _(t,x,ℓ)_ ∈ _Dk and θ_ ∈ _Tt,T_ , _the set of stopping times valued in_ [ _t,T_ ], _we have_ 



_where_ l∅ = ∅. _In other words_ , _for k_ ∈{0 _,_ 1}, _(t,x,ℓ)_ ∈ _Dk_ , _we have_ : 

(a) **_DPP_ 1.** _For all α_ ∈ _At,ℓ_ , _we have for all stopping times θ valued in_ [ _t,T_ ] _that_ 



(b) **_DPP_ 2.** _For all ϵ >_ 0, _there exists α_ ∈ _At,ℓ such that for all stopping times θ valued in_ [ _t,T_ ], _we have_ 



Before proceeding to the proof, we remark that from the dynamics of the controlled system _X_<sup>_α_</sup> in (3.7), the independence of the marks and the memoryless property of the exponential distribution of latency, we have the following properties: 

Á. Cartea, L. Sánchez-Betancourt 

30 

(1) _Markov property of (X_<sup>_α_</sup> _,k(_ · _,α),_ l _ι(_ · _,α))_ : For any _α_ ∈ _A_ , we have 



for any bounded measurable function _ψ_ and stopping times _θ_ 1 ≤ _θ_ 2 a.s. 

(2) _Causality of the control_ : For any _α_ = _(τi,_ l _i)i_ ≥1 ∈ _A_ and stopping time _θ_ , 



where _α_<sup>_θ_</sup> = _(τi_ + _ι(θ,α),_ l _i_ + _ι(θ,α))i_ ≥1. 

(3) _Pathwise uniqueness of the state process_ : 



on [0 _,T_ ], for any _(t,x,ℓ)_ ∈ _Dk,k_ = 0 _,_ 1 _,α_ ∈ _At,ℓ_ and _θ_ ∈ _Tt,T_ . 

**_Proof of Theorem A.1_** (a) Fix _(t,x,ℓ)_ ∈ _Dk_ , _k_ ∈{0 _,_ 1} and take an arbitrary control _α_ ∈ _At,ℓ_ . By the law of iterated conditional expectations, we have 

_Jk(t,x,ℓ,α)_ 



and because of the joint Markov property of _(X_<sup>_α_</sup> _,k(_ · _,α),_ l _ι(_ · _,α))_ , the causality of the control and the pathwise uniqueness of _X_<sup>_t,x,ℓ,α_</sup> (see (A.1)–(A.3)), we have 

_Jk(t,x,ℓ,α)_ 



Now take the supremum over _α_ ∈ _At,ℓ_ on both sides to obtain the desired result. 

(b) Fix _(t,x,ℓ)_ ∈ _Dk_ , _k_ = 0 _,_ 1 and take an arbitrary control _α_ ∈ _At,ℓ_ . By the definition of _vk_ , for any _ϵ >_ 0 and _ω_ ∈ _�_ , there exists _αϵ,ω_ ∈ _Aθ(ω),_ l _ι(θ(ω),α(ω))_ which is an _ϵ_ -optimal control for _vk(θ(ω),α(ω))_ at the point 



Optimal execution with stochastic delay 

31 

Next, by a measurable selection argument, see Bertsekas and Shreve [7, Chap. 7], one can show that there is _α_ ¯ _ϵ_ ∈ _Aθ,_ l _ι(θ,α)_ such that _α_ ¯ _ϵ(ω)_ = _αϵ,ω(ω)_ for almost all _w_ ∈ _�_ , so that 



Next, concatenate the controls _α_ and _α_ ¯ _ϵ_ ; that is, take _α_ ¯ to be _α_ for the impulses that are before (or include) time _θ_ , and take _α_ ¯ _ϵ_ for the impulses that are strictly after time _θ_ . By construction, _α_ ¯ ∈ _At,ℓ_ so that _Xu_<sup>_t,x,ℓ,α_</sup> = _Xu_<sup>_t,x,ℓ,α_¯</sup> for all _u_ ∈[ _t,θ_ ], _k(θ,α)_ = _k(θ,_ ¯ _α)_ , l _ι(θ,α)_ = l _ι(θ,α)_ ¯ and _α_ ¯<sup>_θ_</sup> = _α_ ¯ _ϵ_ . Therefore, following the same steps as in the proof of (a), we have 



which together with (A.4) yields 



where _α_ ¯ can be replaced by _α_ because _α_ ¯ is less than or equal to _θ_ . Now take the supremum over _α_ ∈ _At,ℓ_ on both sides to conclude that 



which completes the proof because _ϵ_ is arbitrary. 



We characterise the value functions with the following two corollaries which are stated without proof (they follow from Theorem A.1). In what follows, _Z_ is a random variable with law _ν(_ d _x)_ as in (3.2), independent of _W_ and of the exponentially distributed delays between trade attempts and notifications. 

Á. Cartea, L. Sánchez-Betancourt 

32 

**Corollary A.2** _For k_ = 1, _(t,x,ℓ)_ ∈ _D_ 1, _p_ ∈ R _and θ_ ∈ _Tt,T_ , _we have_ 



_In particular_ , _when θ_ = _T_ , _it is possible to characterise v_ 1 _in terms of v_ 0 _via_ 



_and v_ 1 _(T,x,ℓ)_ = _v_ 0 _(T,x)_ = _c_ + _q(s_ + _ζ_ − _a(q_ − 1 _))(_ 1 − _ρ)_ , _for all ℓ_ ∈ _L_ . 

**Corollary A.3** _For (t,x)_ ∈ _D_ 0 _and θ_ ∈ _Tt,T_ , _we have_ 



_where It is the set of all pairs (τ,_ l _) where τ is a stopping time with t_ ≤ _τ < T or τ_ = ∞ _a_ . _s_ ., _and_ l _is an Fτ -measurable random variable valued in L_ . 

We use the following corollary to prove the subsolution property of the value function (see Theorem A.9). 

**Corollary A.4** _For (t,x)_ ∈ _D_ 0 _and θ_ ∈ _Tt,T_ , _we have_ 



### **A.1 PDE system viscosity characterisation** 

In Corollaries A.2 and A.3, we let _θ_ → _t_ to characterise the value functions _v_ 0 and _v_ 1. 

Optimal execution with stochastic delay 

33 

**Theorem A.5** _The value functions satisfy the dynamic programming equation_ 



_and_ 



_where λ >_ 0 _is the parameter of the exponential distribution that describes the random latency_ , 



_the random variable Z has law ν(_ d _x) as in_ (3.2) _and the terminal conditions are_ 



The intuition for the result in Theorem A.5 is as follows. With no pending orders, the trader will submit an order with limit rate l<sup>∗</sup> when it is better to have one order pending in the exchange than none; this decision is determined by the QVI in (A.7). The crucial tradeoff is that once the agent sends an order, she must wait until it is processed by the exchange before sending another order; thus, a pending order precludes the trader from taking advantage of any opportunities that arise while a pending order is being processed during the period of latency. Finally, the trader’s value function satisfies the dynamic programming equation in (A.8) when there is an order pending in the exchange. 

A priori, in (A.7), it is not clear that the candidate optimal control is well defined. If _v_ 1 _(t,x,ℓ)_ is continuous in _ℓ_ , then given that _L_ is compact, we can define the candidate optimal control as _ℓ_<sup>∗</sup> = argmax _ℓv_ 1 _(t,x,ℓ)_ whenever _v_ 0 _(t,x)_ = _v_ 1 _(t,x,ℓ_<sup>∗</sup> _)_ . On the other hand, if _v_ 1 _(t,x,ℓ)_ is not continuous in _ℓ_ , we can take the set _L_ to be a finite collection of points in R. Recall that _L_ is the set of possible limit rates, and limit rates in EUR/USD take values with precision of (at most) five decimals. Thus we can define _L_ to be all limit rates with five decimals of precision between zero and a fixed _ℓ_<sup>¯</sup> – in practice, any number that is twice the current level of the exchange rate suffices. 

We use (A.5) to make a key simplification to the system in (A.7) and (A.8). We plug (A.5) into (A.7) to obtain an HJBQVI that characterises the value function _v_ 0. We remark that once we know _v_ 0, we compute _v_ 1 by means of (A.5). Equation (A.7) 

Á. Cartea, L. Sánchez-Betancourt 

34 

becomes 



with terminal condition _v_ 0 _(T,x)_ = _c_ + _q(s_ + _ζ_ − _a(q_ − 1 _))(_ 1 − _ρ)_ . We propose the ansatz _v_ 0 _(t,s,q,c)_ = _c_ + _h_ 0 _(t,s,q)_ to reduce the system (A.9) to 



with terminal condition _h_ 0 _(T,s,q)_ = _(qs_ + _qζ_ − _aq(q_ − 1 _))(_ 1 − _ρ)_ , and 



### **A.2 Viscosity properties of the value function** **_v_ 0** 

The value functions _v_ 0 and _v_ 1 do not need to be smooth a priori. We focus on the viscosity properties of the HJBQVI satisfied by _v_ 0 because we fully characterised _v_ 1 in terms of _v_ 0 (see Corollary A.2). Bruder and Pham [9] use backward and forward iterations on the domains and value functions because they have more than two coupled value functions. In our case, a more ‘standard’ approach suffices because of the reduction in (A.9). For classical references on the subject of viscosity solutions we refer the reader to Crandall et al. [16, Sects. 1–3] and Fleming and Soner [17, Sect. 5]. 

For a locally bounded function _w_ on _D_ 0, we denote by _<u>w</u>_ (respectively, _<u>w</u>_ ) the lower-semicontinuous (respectively, upper-semicontinuous) envelope of _w_ , given by 



Optimal execution with stochastic delay 

35 

For any locally bounded function _u_ on _D_ 0, define the locally bounded function _Hu_ on _D_ 0 by 



**Lemma A.6** _Let w be a locally bounded function on D_ 0. _Then Hw is upper-semicontinuous_ , _and Hw_ ≤ _Hw_ . 

**_Proof_** First, we prove that _Hw_ is upper-semicontinuous. Fix _(t,x)_ ∈ _D_ 0 and let _(tn,xn)n_ ≥1 be a sequence in _D_ 0 converging to _(t,x)_ as _n_ →∞. There exists a sequence _(_ l _n)n_ ∈N valued in _L_ such that _Hw(tn,xn)_ is equal to 



for _n_ ≥ 1 because _<u>w</u>_ is upper-semicontinuous and _L_ is compact. By the Bolzano– Weierstrass theorem, the sequence _(_ l _n)n_ ∈N converges (up to a subsequence) to an element<sup>ˇ</sup> l ∈ _L_ . Thus we get 





where the first inequality follows because<sup>ˇ</sup> l ∈ _L_ . The second inequality follows because _w_ ¯ is upper-semicontinuous, _�_ is continuous and by Fatou’s lemma. Finally, the last equality follows by definition. Hence _Hw_ is upper-semicontinuous, as required. 

Next, to prove that _Hw_ ≤ _Hw_ , fix _(t,x)_ ∈ _D_ 0 and let _(tn,xn)n_ <u>≥1</u> be a sequence in _D_ 0 converging to _(t,x)_ as _n_ →∞ such that _Hw(tn,xn)_ → _Hw(t,x)_ . Then by the 

Á. Cartea, L. Sánchez-Betancourt 

36 

properties of the sequence and the upper-semicontinuity of _<u>w</u>_ , we have that 



where the last inequality follows from the upper-semicontinuity of _Hw_ . Thus we obtain _Hw_ ≤ _Hw_ . □ 

**Definition A.7** A locally bounded function _w_ on _D_ 0 is a _viscosity supersolution_ of (A.9) on _D_ 0 if for all _(t_ 0 _,x_ 0 _,ψ)_ ∈ _D_ 0 × _C_<sup>2</sup> _(D_ 0 _)_ such that _<u>w</u>_ − _ψ_ attains a minimum at _(t_ 0 _,x_ 0 _)_ , we have 



It is a _viscosity subsolution_ of (A.9) on _D_ 0 if for all _(t_ 0 _,x_ 0 _,ψ)_ ∈ _D_ 0 × _C_<sup>2</sup> _(D_ 0 _)_ such that _<u>w</u>_ − _ψ_ attains a maximum at _(t_ 0 _,x_ 0 _)_ , we have 



Finally, we say that _w_ is a _viscosity solution_ of (A.9) on _D_ 0 if it is a viscosity supersolution and a viscosity subsolution of (A.9) on _D_ 0. 

Before we state the main theorem of this section, we present a comparison result that we need in the proof of uniqueness of the viscosity solution. The following growth condition for the value function is assumed **for the remainder of the paper** : 



This assumption is not too restrictive. If _b(s)_ ≡ _b_ ∈ R and _σ(s)_ ≡ _σ_ ∈ R+ in the dynamics of the fundamental best bid rate process, then a calculation shows that the bound is satisfied. Note that in Sect. 4, we use _b(s)_ ≡ 0 and _σ(s)_ ≡ _σ >_ 0. The bound in (A.11) is required for the comparison result which guarantees uniqueness. However, if we relax this assumption, it is not clear we could ensure uniqueness. 

Optimal execution with stochastic delay 

37 

**Proposition A.8** _Let u_ 0 ( _respectively_ , _w_ 0) _be a viscosity subsolution_ ( _respectively_ , _supersolution_ ) _of_ (A.9) _satisfying the growth condition_ (A.11). _Suppose that w_ 0 _satisfies w_ 0 ≥ _Hw_ 0 _on D_ 0, _and that_ _<u>u</u>_ 0 _(T,x)_ ≤ _<u>w</u>_ ~~0~~<sup>_(T,x) for all x_∈R×</sup><sup>_(_−∞</sup><sup>_,_M]×R.</sup><sup>_Then_</sup> _<u>u</u>_ 0 ≤ _<u>w</u>_ ~~0~~<sup>_on D_0.</sup> 

We state the steps one needs to follow (together with the assumptions needed) to complete the proof. First, we show that for any _η >_ 0, there is a viscosity _η_ -strict supersolution<sup>1</sup> _w_ 0<sup>_η_of (A.9) satisfying</sup><sup>_w_</sup> 0<sup>_η_≥</sup><sup>_Hw_</sup> 0<sup>_η_on</sup><sup>_D_0and such that for</sup><sup>_(t,x)_∈</sup><sup>_D_0,</sup> we have _w_ 0 + _ηC_ 1 | _x_ |<sup>2</sup> ≤ _w_ 0<sup>_η_≤</sup><sup>_w_0+</sup><sup>_ηC_2</sup><sup>_(_1+|</sup><sup>_x_|2</sup><sup>_)_for some positive constants</sup><sup>_C_1</sup><sup>_,C_2</sup> independent of _η_ . Such a construction is in Bruder and Pham [9, Lemma 7.4] where we take _m_ = 1. It then follows by the linear growth condition that _ηC_ 1 | _x_ |<sup>2</sup> − _C_ 2 ≤ _w_ 0<sup>_η_</sup> on _D_ 0 for some positive constants _C_ 1 _,C_ 2. Similarly to [9], the main step in the proof of Proposition A.8 is to compare a viscosity subsolution to a viscosity _η_ -strict supersolution. It is then standard to write the sub viscosity inequalities<sup>2</sup> and _η_ -strict supersolution inequality in terms of semi-jets, and then conclude with Ishii’s lemma. For the proof, we require (i) continuity of _f_ , (ii) the Lipschitz property of _b,σ_ , (iii) the growth condition (A.11), and (iv) the two inequalities above. Finally, comparison of _η_ -strict supersolutions implies comparison for supersolutions by letting _η_ → 0. 

**Theorem A.9** _The value function v_ 0 _is the unique viscosity solution of the HJBQVI in_ (A.9) _on D_ 0, _satisfying_ (A.11), _with v_ 0 _(T,x)_ = _c_ + _q(s_ + _ζ_ − _a(q_ − 1 _))(_ 1 − _ρ) and satisfying v_ 0 ≥ _Hv_ 0 _on D_ 0. 

**_Proof_** ( **Local boundedness** ) Let _(t,x)_ ∈ _D_ 0. From (A.11), there exists _r_ 1 _>_ 0 that does not depend on _(t,x)_ such that | _v_ 0 _(t,x)_ | ≤ _r_ 1 _(_ 1 + | _x_ | _)_ . Thus the value function _v_ 0 is locally bounded. 

( **Supersolution property** ) Let _(t_ 0 _,x_ 0 _,ψ)_ ∈ _D_ 0 × _C_<sup>2</sup> _(D_ 0 _)_ be such that _v_ <u>0</u> − _ψ_ attains a minimum at _(t_ 0 _,x_ 0 _)_ with _v_ <u>0</u> _<u>(t</u>_ 0 _,x_ 0 _)_ = _ψ(t_ 0 _,x_ 0 _)_ . By the definition of _v_ <u>0,</u> there is a sequence _(tn,xn)n_ ≥1 ∈ _D_ 0 such that _v_ 0 _(tn,xn)_ → _v_ <u>0</u> _<u>(t</u>_ 0 _,x_ 0 _)_ with _(tn,xn)_ → _(t_ 0 _,x_ 0 _)_ . Let _ℓ_ 1 ∈ _L_ and _(t,x)_ ∈ _D_ 0. Use (A.6) and Corollary A.2 with an immediate impulse and price limit _ℓ_ 1, i.e., _τ_ = _t_ , _θ_ = _T_ and l = _ℓ_ 1, to write 

_v_ 0 _(t,x)_ ≥ _v_ 1 _(t,x,ℓ_ 1 _)_ 



> 1See Bruder and Pham [9] for a definition of a viscosity _η_ -strict supersolution. 

2These inequalities would be the analogues of [9, Eqs. (7.22) and (7.23)]. 

Á. Cartea, L. Sánchez-Betancourt 

38 

Thus 

liminf _n_ →∞<sup>_v_0</sup><sup>_(tn,xn)_</sup> 



Next, use Fatou’s lemma, the continuity of _�_ and the definition of _<u>v</u>_ ~~0~~<sup>to obtain</sup> 



Therefore, 



because the price limit _ℓ_ 1 is arbitrary. Finally, to complete the viscosity supersolution property, use Corollary A.3 with _τ_ = ∞, Itô’s formula for _ψ_ ∈ _C_<sup>2</sup> _(D_ 0 _)_ , the property that _<u>v</u>_ ~~0~~<sup>−</sup><sup>_ψ_attains a minimum at</sup><sup>_(t_0</sup><sup>_,x_0</sup><sup>_)_with</sup><sup>_<u>v</u>_</sup> ~~0~~<sup>_(t_0</sup><sup>_,x_0</sup><sup>_)_=</sup><sup>_ψ(t_0</sup><sup>_,x_0</sup><sup>_)_, and let</sup><sup>_θ_→</sup><sup>_t_0</sup> to obtain 



We base the next part on Bruder and Pham [9]. 

( **Subsolution property** ) Let _(t_ 0 _,x_ 0 _,ψ)_ ∈ _D_ 0 × _C_<sup>2</sup> _(D_ 0 _)_ be such that _<u>v</u>_ 0 − _ψ_ attains a maximum at _(t_ 0 _,x_ 0 _)_ , _ψ(t_ 0 _,x_ 0 _)_ = _<u>v</u>_ 0 _<u>(t</u>_ 0 _,x_ 0 _)_ and _ψ_ ≥ _<u>v</u>_ 0 on _D_ 0. If we have _<u>v</u>_ 0 _<u>(t</u>_ 0 _,x_ 0 _)_ ≤ _Hv_ 0 _<u>(t</u>_ 0 _,x_ 0 _)_ , the desired inequality follows and the proof is complete. On the other hand, if _<u>v</u>_ 0 _<u>(t</u>_ 0 _,x_ 0 _) > Hv_ 0 _<u>(t</u>_ 0 _,x_ 0 _)_ , we proceed by contradiction, i.e., we assume there is _η >_ 0 such that 



Optimal execution with stochastic delay 

39 

By the definition of _<u>v</u>_ 0 and the continuity of _ψ_ , there exist _ϵ >_ ˜ 0 and a sequence 



such that 





Here _B(x_ 0 _,_ ˜ _ϵ)_ is the open ball with centre in _x_ 0 and radius _ϵ_ ˜ under the Euclidean metric. By continuity of _ψ_ and (A.12), we also have 



By Corollary A.4, for each _n_ ≥ 1, there is a control _(τn,_ l _n)_ ∈ _Itn_ such that 



Choose _θn_ = _(tn_ + _ϵn)_ ∧ _ϑn_ ∧˜ _tn_ with 



and _(ϵn)n_ ∈N a strictly positive sequence that satisfies _ϵn_ → 0 and _γn/ϵn_ → 0 as _n_ →∞. After simple inspection of (A.15) and because _t_ ˜ _n_ is the first time there is a jump in _N_ after _tn_ , write 



Use Lemma A.6 to write _Hv_ 0 _(t_ 0 _,x_ 0 _)_ ≤ _Hv_ 0 _(t_ 0 _<u>,</u> x_ 0 _<u>) < v</u>_ 0 _<u>(t</u>_ 0 _,x_ 0 _)_ = _ψ(t_ 0 _,x_ 0 _)_ . As _Hv_ 0 is upper semicontinuous, _ψ_ is continuous and _Hv_ 0 _(t_ 0 _,x_ 0 _) < ψ(t_ 0 _,x_ 0 _)_ , the inequality _Hv_ 0 ≤ _ψ_ holds in a neighbourhood of _(t_ 0 _,x_ 0 _)_ . Thus for _n_ sufficiently large and because _v_ 1 _(tn,xn,_ l _n)_ ≤ _Hv_ 0 _(tn,xn)_ , we obtain 



Á. Cartea, L. Sánchez-Betancourt 

40 

Equation (A.16) together with (A.14) and (A.17) yields 



Use Itô’s formula between _tn_ and _θn_ , divide by _ϵn_ and use the bound in (A.13) to obtain 



Here, the lack of regularity of the value function precludes us from proving a verification-type result, as in the case of classical solutions to HJB equations, to prove that the strategy we find is indeed optimal. However, as shown by our results, the latency-optimal strategy outperforms the four benchmarks we consider (among which there is the robust and well-known TWAP), which lends strong support to our assumption that the random-latency strategy is the optimal strategy. 

### **A.3 A note about numerical approximations** 

To compute a numerical approximation to the value function _v_ 0, we proceed as follows. First, use the ansatz _v_ 0 _(t,x)_ = _c_ + _h_ 0 _(t,s,q)_ and compute an approximation to _h_ 0 satisfying (A.10) with terminal condition _h_ 0 _(T,s,q)_ = _(qs_ + _qζ_ − _aq(q_ −1 _))(_ 1− _ρ)_ . Next, consider the grid discretisation [0 _,T_ ] × [ _S_ 0 − 100t _,S_ 0 + 100t] × [0 _,Q_ 0] with 1000 points in the time axis, 201 points in the price axis and 11 in the inventory axis – here t = 10<sup>−5</sup> which is the tick-size in EUR/USD in LMAX. At the terminal time _T_ , the value of the function is given by the terminal condition. Then for any point _(t,s,q)_ in the grid, we approximate 



together with the control _ℓ_<sup>∗</sup> ∈ _L_ that attains the supremum in (A.18), which we obtain as follows. Split the set _L_ into intervals of size 10<sup>−5</sup> considering 100 ticks above and 100 ticks below _St_ = _s_ – considering more ticks is not necessary because the probability that {| _St_ ˜ − _s_ | _>_ 100 × 10<sup>−5</sup> } is negligible. We perform (i) 10<sup>4</sup> simulations for _t_ ˜, where we use as _t_ ˆ ∈ _(t,T_ ] the closest value to _t_ ˜ in the time grid, (ii) 10<sup>4</sup> simulations for _St_<sup>_t,s_</sup> ˆ rounded to five decimals, and (iii) 10<sup>4</sup> simulations for _Z_ . If for a given simulation, the fundamental best bid rate _St_<sup>_t,s_</sup> ˆ is not in the grid, we use a cubic spline approximation with the functions _x_ �→ _h(t,x,q)_ ˆ and _x_ �→ _h(t,x,q_ ˆ − 1 _)_ . We store the optimal _ℓ_<sup>∗</sup> ∈ _L_ as a function of _(t,s,q)_ together with a variable that flags when an impulse happens. 

Optimal execution with stochastic delay 

41 

The methodology described above is effective, but computationally expensive. However, once the value function is approximated, the implementation of the optimal strategy is fast enough so that it can be used to trade in the market. Further analysis of numerical methods for this type of problems is left for future research. 

## **Appendix B: Benchmarks** 

### **B.1 Optimal execution with zero latency** 

Assume there is no latency in the marketplace. The trader controls the times at which she sends sell MOs to the exchange. The size of all MOs is one and the filled trades have permanent price impact as in the model of Sect. 3. 

Let _τ_ = _(τi)i_ ≥1 denote an increasing sequence of stopping times when the trader sends sell MOs to the exchange. Observe that if latency is zero, a sell MO of size one at time _τi_ is the same as a sell MLO for a unit size with a limit price _Sτ_<sup>_τ_</sup> _i_ −<sup>because the</sup> trader observes and acts with no delay in the marketplace and the exchange processes the order with no delay. The dynamics of the best bid price process, denoted by _S_<sup>_τ_</sup> = _(St_<sup>_τ)t_≥0, follow</sup> 



where the functions _b,σ_ : R → R are Lipschitz-continuous and _κ_ is the permanent bid rate impact parameter. The cash process _C_<sup>_τ_</sup> = _(Ct_<sup>_τ)t_≥0and the inventory process</sup> _Q_<sup>_τ_</sup> = _(Q_<sup>_τ_</sup> _t_<sup>_)t_≥0are given by</sup> 



For each execution strategy _τ_ over the trading window [0 _,T_ ], the trader computes 



where _g,h_ : R<sup>3</sup> → R are as in (3.9) and the function C _(s,q,c)_ = − _ρs_ represents a transaction cost every time a trade of unit size is executed. The trader solves the control problem sup _τ_ ∈ _A_ E[ _�(τ)_ ], where 

_A_ = { _τ_ = _(τi)i_ ≥1 : for each _i_ ≥ 1 _, τi_ is an F-stopping time, and _τi_ +1 _> τi_ } _._ 

For _(t,x)_ ∈[0 _,T_ ]× R × _(_ −∞ _,_ M]× R and _τ_ ∈ _At_ := { _τ_ = _(τi)i_ ≥1 ∈ _A_ : _τ_ 1 ≥ _t_ }, the system _X_<sup>_t,x,τ_</sup> is given by 



Á. Cartea, L. Sánchez-Betancourt 

42 

where _�(s,q,c)_ = _(s_ − _κ,q_ − 1 _,c_ + _s)_ . The trader’s performance criterion and value function are 





Finally, for zero latency, the characterisations of the value function are standard results in the theory of impulse control, and we do not mention them here for brevity. 

### **B.2 Optimal execution with deterministic latency** 

The trader employs MLOs and operates in the market with a fixed and known latency. The execution strategy has at most one pending order at any one time – we can easily extend this framework so the agent can have up to _m_ ∈ N pending orders. Our approach is based on the work of Bruder and Pham [9], who develop a general framework for impulse control with deterministic delay. 

Let _�>_ 0 be the fixed and known delay of the trader in the marketplace and _(Zn)n_ ∈N a collection of i.i.d. random variables with law as in (3.2) such that for _i_ ∈ N, _Zi_ is _Ft_ -measurable for _t_ ≥ _i�_ . As in Sect. 3, we use the sequence of marks _(Zn)n_ ∈N to model the flickers that affect the trader’s MLO when the exchange processes the order. 

The agent’s execution strategy is _β_ = _(τi,_ l _i)i_ ≥1, where _(τi)_ is an increasing sequence of stopping times at which the agent sends MLOs with limit price l _i_ . We require that _τi_ +1 − _τi_ ≥ _�_ a.s. for all _i_ , because the trader can have at most one pending order in the exchange. The set of admissible strategies is 

_A_ = { _β_ = _(τi,_ l _i)i_ ≥1 : for all _i_ ≥ 1 _,τi_ +1 − _τi_ ≥ _�_ a.s. and l _i_ is _Fτi_ -measurable} _._ 

Interventions in the system _Xt_<sup>_β_=</sup><sup>_(S_</sup> _t_<sup>_β,Qβ_</sup> _t_<sup>_,C_</sup> _t_<sup>_β)_are at time</sup><sup>_τi_, but processed by the</sup> exchange and notified to the trader at _τi_ + _�_ ; so the system evolves from _X(τ_<sup>_β_</sup> _i_ + _�)_ − to _Xτ_<sup>_β_</sup> _i_ + _�_<sup>=</sup><sup>_�(X_</sup> _(τ_<sup>_β_</sup> _i_ + _�)_ −<sup>_,Z_⌊</sup><sup>_(τi_+</sup><sup>_�)/�_⌋</sup><sup>_,_l</sup><sup>_i)_,where</sup><sup>_�_isasin(3.5)and⌊·⌋isthefloor</sup> function. The value of _j_ = ⌊ _(t_ + _�)/�_ ⌋ is the index of the random variable _Zj_ which is measurable at time _j�_ ∈ _(t,t_ + _�_ ]. 

Let _β_ = _(τi,_ l _i)i_ ≥1 ∈ _A_ . The fundamental best bid price process is 



where _f_ is as in the random latency case. The cash process satisfies 



Optimal execution with stochastic delay 

43 

and the inventory is 



For _X_ 0 = _(S_ 0 _,_ M _,_ 0 _)_ , the controlled system _Xt_<sup>_β_is the solution to the SDE</sup> 



where the functions **_b_** _,_ **_σ_** : R<sup>3</sup> → R<sup>3</sup> are given by **_b_** _(x_ 1 _,x_ 2 _,x_ 3 _)_ = _(b(x_ 1 _),_ 0 _,_ 0 _)_ and **_σ_** _(x_ 1 _,x_ 2 _,x_ 3 _)_ = _(σ(x_ 1 _),_ 0 _,_ 0 _)_ , and _b_ , _σ_ are Lipschitz-continuous. Finally, the function _�_ : R<sup>4</sup> × _L_ → R<sup>3</sup> is as in (3.5). 

Now fix a finite horizon _T <_ ∞ and assume that _T_ − _�_ ≥ 0 to avoid trivialities. As above, the agent computes 



where _g,h_ : R<sup>3</sup> → R and C : R<sup>4</sup> × _L_ → R are as in (3.9). Thus the agent solves the control problem sup _β_ ∈ _A_ E[ _�(β)_ ], which is well posed if 



The system in (B.1) is non-Markovian. Given an impulse, the future of _X_<sup>_β_</sup> is influenced by a possible pending order attempted between _t_ − _�_ and _t_ . For any _t_ ∈[0 _,T_ ], _k_ = 0 _,_ 1, we let _Pt (_ 0 _)_ = ∅ and 



denote whether there is a pending order in the exchange. For any _p_ = _(t_ 1 _,ℓ_ 1 _)_ ∈ _Pt (_ 1 _)_ , _t_ ∈[0 _,T_ ], the set of admissible strategies at time _t_ with pending order _p_ is 



Similarly, if there is no pending order in the exchange, 



For any _(t,x)_ ∈[0 _,T_ ] × R<sup>3</sup> , _p_ ∈ _Pt (k)_ for _k_ = 0 _,_ 1 and _β_ ∈ _At,p_ , we denote by _X_<sup>_t,x,p,β_</sup> the solution to 



Á. Cartea, L. Sánchez-Betancourt 

44 

Next, consider the performance criterion 



for _(t,x)_ ∈[0 _,T_ ] × R<sup>3</sup> , _p_ ∈ _Pt (k)_ , _k_ = 0 _,_ 1, _β_ ∈ _At,p_ , and the corresponding value functions 



where _Dk_ = { _(t,x,p)_ : _(t,x)_ ∈[0 _,T )_ × R<sup>3</sup> _,p_ ∈ _Pt (k)_ }. Recall that for _k_ = 0, _Pt (_ 0 _)_ = ∅; therefore we write _v_ 0 _(t,x)_ = _v_ 0 _(t,x,_ ∅ _)_ , _D_ 0 = [0 _,T )_ × R<sup>3</sup> . 

We state the DPP that holds for deterministic latency. Let _t_ ∈[0 _,T_ ], _β_ ∈ _A_ , with _β_ = _(τi,_ l _i)i_ ≥1. Denote the number of executed orders by _ι(t,β)_ , the number of pending orders at time _t_ by _kt (β)_ (which is zero or one), and the pending order by _p(t,β)_ (which is the empty set if there is no pending order); so 



For the proofs of the theorems that follow, see Bruder and Pham [9, Sects. 3 and 4]. 

**Theorem B.1** _Let θ be any stopping time valued in_ [ _t,T_ ], _possibly depending on β_ . _For k_ ∈{0 _,_ 1}, _(t,x,p)_ ∈ _Dk_ , 



The following corollary is a consequence of the DPP. For all _t_ ∈[0 _,T_ ], denote by _It_ the set of pairs _(τ,_ l _)_ where _τ_ is a stopping time with _t_ ≤ _τ_ ≤ _T_ − _�_ or _τ_ = ∞ a.s., and l is an _Fτ_ -measurable random variable in _L_ . 

**Corollary B.2** _Let (t,x)_ ∈[0 _,T )_ × R<sup>_d_</sup> . 

(1) _For any stopping time θ valued in_ [ _t,t_ 1 + _�) and for p_ = _(t_ 1 _,ℓ_ 1 _)_ ∈ _Pt (_ 1 _)_ , 



Optimal execution with stochastic delay 

45 

(2) _For any stopping time θ valued in_ [ _t,t_ + _�)_ , 



The proof is a straightforward modification of the proofs of Theorem 3.1 and Bruder and Pham [9, Corollary 3.2]. Define _Dk_<sup>1</sup><sup>_,_2</sup> for _k_ = 0 _,_ 1 as _D_ 0<sup>1=</sup><sup>_(T_−</sup><sup>_�,T )_×R3,</sup> _D_ 0<sup>2= [0</sup><sup>_,T_−</sup><sup>_�_] × R3,</sup><sup>_D_</sup> 1<sup>1=</sup><sup>_D_1,</sup><sup>_D_</sup> 1<sup>2= ∅.</sup> 

**Theorem B.3** _For k_ ∈{0 _,_ 1}, _(t,x,p)_ ∈ _Dk_<sup>1</sup><sup>_,_2,</sup><sup>_the value functions satisfy the dynamic_</sup> _programming equation_ 



_and_ 



From the system of equations (B.2) and (B.3), we have the Feynman–Kac representation 



for all _(t_ 1 _,ℓ_ 1 _)_ ∈[0 _,T_ − _�_ ] × _L_ , _t_ ∈[ _t_ 1 _,t_ 1 + _�)_ × R<sup>3</sup> , and where _Z_ is an independent random variable with law as in (3.2). Substitute (B.4) in (B.2) to obtain the variational inequality satisfied by _v_ 0, and transform the original problem into a non-delay impulse control problem. Finally, we use the ansatz _v_ 0 _(t,x)_ = _v_ 0 _(t,s,q,c)_ = _c_ + _h_ 0 _(t,s,q)_ to reduce the dimension of the value function by one. 

**Acknowledgements** We are grateful to I. Cialenco, S. Cohen, R. Cont, R. Donnelly, L. Hughston, S. Jaimungal, J. Muhle-Karbe, J. Penalva, H. Pham, J. Ricci and A. Stewart for comments and discussions. We are grateful to seminar participants at SIAM SIAG/FME virtual seminars series, U. of Leeds and U. of Oxford. 

### **Declarations** 

**Competing Interests** The authors declare no competing interests. 

Á. Cartea, L. Sánchez-Betancourt 

46 

**Open Access** This article is licensed under a Creative Commons Attribution 4.0 International License, which permits use, sharing, adaptation, distribution and reproduction in any medium or format, as long as you give appropriate credit to the original author(s) and the source, provide a link to the Creative Commons licence, and indicate if changes were made. The images or other third party material in this article are included in the article’s Creative Commons licence, unless indicated otherwise in a credit line to the material. If material is not included in the article’s Creative Commons licence and your intended use is not permitted by statutory regulation or exceeds the permitted use, you will need to obtain permission directly from the copyright holder. To view a copy of this licence, visit http://creativecommons.org/licenses/by/ 4.0/. 

## **References** 

1. Alfonsi, A., Fruth, A., Schied, A.: Optimal execution strategies in limit order books with general shape functions. Quant. Finance **10** , 143–157 (2010) 

2. Almgren, R.: Optimal execution with nonlinear impact functions and trading-enhanced risk. Appl. Math. Finance **10** , 1–18 (2003) 

3. Almgren, R.: Execution costs. In: Cont, R. (ed.) Encyclopedia of Quantitative Finance, pp. 612–616. Wiley, New York (2010) 

4. Almgren, R., Chriss, N.: Optimal execution of portfolio transactions. J. Risk **3** , 5–39 (2000) 5. Barger, W., Lorig, M.: Optimal liquidation under stochastic price impact. Int. J. Theor. Appl. Finance **22** , 1850059 (2019) 

6. Bayraktar, E., Ludkovski, M.: Optimal trade execution in illiquid markets. Math. Finance **21** , 681–701 (2010) 

7. Bertsekas, D.P., Shreve, S.: Stochastic Optimal Control: The Discrete-Time Case. Academic Press, San Diego (1978) 

8. Bouchard, B., Touzi, N.: Weak dynamic programming principle for viscosity solutions. SIAM J. Control Optim. **49** , 948–962 (2011) 

9. Bruder, B., Pham, H.: Impulse control problem on finite horizon with execution delay. Stoch. Process. Appl. **119** , 1436–1469 (2009) 

10. Cartea, Á., Jaimungal, S.: Optimal execution with limit and market orders. Quant. Finance **15** , 1279–1291 (2015) 

11. Cartea, Á., Jaimungal, S.: Incorporating order-flow into optimal execution. Math. Financ. Econ. **10** , 339–364 (2016) 

12. Cartea, Á., Jaimungal, S., Penalva, J.: Algorithmic and High-Frequency Trading. Cambridge University Press, Cambridge (2015) 

13. Cartea, Á., Jaimungal, S., Ricci, J.: Buy low, sell high: a high frequency trading perspective. SIAM J. Financ. Math. **5** , 415–444 (2014) 

14. Cartea, Á., Arribas, I.P., Sánchez-Betancourt, L.: Double-execution strategies using path signatures. SIAM J. Financ. Math. **13** (4), 1379–1417 (2022). https://doi.org/10.1137/21M1456467 

15. Cartea, Á., Sánchez-Betancourt, L.: The shadow price of latency: improving intraday fill ratios in foreign exchange markets. SIAM J. Financ. Math. **12** , 254–294 (2021) 

16. Crandall, M.G., Ishii, H., Lions, P.L.: User’s guide to viscosity solutions of second order partial differential equations. Bull. Am. Math. Soc. **27** , 1–67 (1992) 

17. Fleming, W.H., Soner, H.M.: Controlled Markov Processes and Viscosity Solutions. Springer, Berlin (2006) 

18. Gao, X., Wang, Y.: Optimal market making in the presence of latency. Quant. Finance **20** , 1495–1512 (2020) 

19. Guéant, O.: Optimal execution and block trade pricing: a general framework. Appl. Math. Finance **22** , 336–365 (2015) 

20. Guéant, O.: The Financial Mathematics of Market Liquidity: From Optimal Execution to Market Making. CRC Press, Boca Raton (2016) 

21. Guéant, O., Lehalle, C.A., Fernandez-Tapia, J.: Optimal portfolio liquidation with limit orders. SIAM J. Financ. Math. **3** , 740–764 (2012) 

22. Guilbaud, F., Pham, H.: Optimal high-frequency trading with limit and market orders. Quant. Finance **13** , 79–94 (2013) 

23. Kalsi, J., Lyons, T., Perez Arribas, I.: Optimal execution with rough path signatures. SIAM J. Financ. Math. **11** , 470–493 (2020) 

Optimal execution with stochastic delay 

47 

24. Moallemi, C.C., Saˆglam, M.: The cost of latency in high-frequency trading. Oper. Res. **61** , 1070–1086 (2013) 

25. Øksendal, B., Sulem, A.: Optimal stochastic impulse control with delayed reaction. Appl. Math. Optim. **58** , 243–255 (2008) 

**Publisher’s Note** Springer Nature remains neutral with regard to jurisdictional claims in published maps and institutional affiliations. 

