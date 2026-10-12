



Scholz, Peter 

**Working Paper** 

Size matters! How position sizing determines risk and return of technical timing strategies 

CPQF Working Paper Series, No. 31 

**Provided in Cooperation with:** Frankfurt School of Finance and Management 

_Suggested Citation:_ Scholz, Peter (2012) : Size matters! How position sizing determines risk and return of technical timing strategies, CPQF Working Paper Series, No. 31, Frankfurt School of Finance & Management, Centre for Practical Quantitative Finance (CPQF), Frankfurt a. M. 

This Version is available at: https://hdl.handle.net/10419/55526 

**Standard-Nutzungsbedingungen:** 

Die Dokumente auf EconStor dürfen zu eigenen wissenschaftlichen Zwecken und zum Privatgebrauch gespeichert und kopiert werden. 

Sie dürfen die Dokumente nicht für öffentliche oder kommerzielle Zwecke vervielfältigen, öffentlich ausstellen, öffentlich zugänglich machen, vertreiben oder anderweitig nutzen. 

Sofern die Verfasser die Dokumente unter Open-Content-Lizenzen (insbesondere CC-Lizenzen) zur Verfügung gestellt haben sollten, gelten abweichend von diesen Nutzungsbedingungen die in der dort genannten Lizenz gewährten Nutzungsrechte. 

###### **Terms of use:** 

_Documents in EconStor may be saved and copied for your personal and scholarly purposes._ 

_You are not to copy documents for public or commercial purposes, to exhibit the documents publicly, to make them publicly available on the internet, or to distribute or otherwise use the documents in public._ 

_If the documents have been made available under an Open Content Licence (especially Creative Commons Licences), you may exercise further usage rights as specified in the indicated licence._ 





**Centre for Practical Quantitative Finance** 

# CPQF Working Paper Series 

CPQF Working Paper Series No. 31 

Size Matters! 

How Position Sizing Determines Risk and Return of Technical Timing Strategies 

Peter Scholz 

January 2012 

Authors: Peter Scholz Associate Research Fellow CPQF Frankfurt School of Finance & Management Frankfurt/Main p.scholz@fs.de 

Publisher: 

Frankfurt School of Finance & Management Phone: +49 (0) 69 154 008-0  �  Fax: +49 (0) 69 154 008-728 Sonnemannstr. 9-11  � D-60314 Frankfurt/M. � Germany 

## Size Matters! 

How Position Sizing Determines Risk and Return of Technical Timing Strategies 

Peter Scholz 

Frankfurt School of Finance & Management Centre for Practical Quantitative Finance Working Paper, Version: January 15<sup>th</sup> , 2012 

##### **Abstract** 

The application of a technical trading rule, which just provides long and short signals, requires the investor to decide upon the exposure to stake in each trade. Although this position sizing (or money management) crucially affects the risk and return characteristics, recent academic literature has largely ignored this effect, leaving reported results incomparable. This work systematically analyzes the impact of position sizing on timing strategies and clarifies the relation to the Kelly criterion, which proposes to bet relative fractions from the remaining gambling budget. Both erratic as well as different relative positions, i.e. fixed proportions of the remaining portfolio value, are compared for simple moving average trading rules. The simulation of parametrized return series allows systematically varying those asset price properties, which are most influential on timing results: drift, volatility, and autocorrelation. 

The study reveals that the introduction of relative position sizing has a severe impact on trading results compared to erratic positions. In contrast to a standard Kelly framework, however, an optimal position size does not exist. Interestingly, smaller trading fractions deliver the highest risk-adjusted returns in most scenarios. 

**Keywords** : Kelly Criterion, Money Management, Parameterized Simulation, Position Sizing, Technical Analysis, Technical Trading, Timing Strategy. 

**JEL Classification** : G11 

I 

II 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

### **Contents** 

|**I**<br>**Introduction**|**1**|
|---|---|
|**II Literature Review**|**2**|
|**IIIMethodology**|**4**|
|III.1 Parametric Simulation<br>. . . . . . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>4|
|III.2 The Simple Moving Average Trading Rule . . . . . . . . . . . . . . .|. . . . .<br>5|
|III.3 The Money Management Component . . . . . . . . . . . . . . . . . .|. . . . .<br>6|
|III.4 Evaluation Criteria for Trading Systems<br>. . . . . . . . . . . . . . . .|. . . . .<br>7|
|**IV Simulation Results**|**7**|
|IV.1 Optimality of Fractions . . . . . . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>8|
|IV.2 Position Sizing Effects for Different Trend Levels<br>. . . . . . . . . . .|. . . . .<br>10|
|IV.3 Position Sizing Effects for Different Volatility Levels . . . . . . . . . .|. . . . .<br>14|
|IV.4 Position Sizing Effects for Different Autocorrelation Levels . . . . . .|. . . . .<br>17|
|**V Conclusions**|**20**|



### **List of Figures** 

|1|Terminal wealth relative dependent on fraction . . . . . . . . . . . . . .|. . . . .<br>9|
|---|---|---|
|2|Terminal wealth relative dependent on fraction for single paths . . . . .|. . . . .<br>10|
|3|Return figures dependent on drift . . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>12|
|4|Risk figures dependent on drift . . . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>13|
|5|Return figures dependent on volatility . . . . . . . . . . . . . . . . . . .|. . . . .<br>15|
|6|Risk figures dependent on volatility . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>16|
|7|Return figures dependent on autocorrelation . . . . . . . . . . . . . . . .|. . . . .<br>19|
|8|Risk figures dependent on autocorrelation . . . . . . . . . . . . . . . . .|. . . . .<br>20|
|**List **|**of Tables**||
|1|Kelly position sizes.<br>. . . . . . . . . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>8|
|2|Overview of the 35 selected leading equity indices.<br>. . . . . . . . . . . .|. . . . .<br>25|
|3|Evaluation criteria . . . . . . . . . . . . . . . . . . . . . . . . . . . . . .|. . . . .<br>26|
|4|f-25% trade results of different drift levels. . . . . . . . . . . . . . . . . .|. . . . .<br>29|
|5|f-50% trade results of different drift levels. . . . . . . . . . . . . . . . . .|. . . . .<br>30|



III 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|6|f-75% trade results of different drift levels. . . . . . . . . . . . . . . .|. . . . . . .<br>31|
|---|---|---|
|7|f-100% trade results of different drift levels. . . . . . . . . . . . . . .|. . . . . . .<br>32|
|8|f-125% trade results of different drift levels. . . . . . . . . . . . . . .|. . . . . . .<br>33|
|9|Unsized trade results of different drift levels. . . . . . . . . . . . . . .|. . . . . . .<br>34|
|10|f-25% trade results of different volatility levels.<br>. . . . . . . . . . . .|. . . . . . .<br>35|
|11|f-50% trade results of different volatility levels.<br>. . . . . . . . . . . .|. . . . . . .<br>36|
|12|f-75% trade results of different volatility levels.<br>. . . . . . . . . . . .|. . . . . . .<br>37|
|13|f-100% trade results of different volatility levels. . . . . . . . . . . . .|. . . . . . .<br>38|
|14|f-125% trade results of different volatility levels. . . . . . . . . . . . .|. . . . . . .<br>39|
|15|Unsized trade results of different volatility levels. . . . . . . . . . . .|. . . . . . .<br>40|
|16|f-25% trade results of different autocorrelation levels. . . . . . . . . .|. . . . . . .<br>41|
|17|f-50% trade results of different autocorrelation levels. . . . . . . . . .|. . . . . . .<br>42|
|18|f-75% trade results of different autocorrelation levels. . . . . . . . . .|. . . . . . .<br>43|
|19|f-100% trade results of different autocorrelation levels. . . . . . . . .|. . . . . . .<br>44|
|20|f-125% trade results of different autocorrelation levels. . . . . . . . .|. . . . . . .<br>45|
|21|Unsized trade results of different autocorrelation levels.<br>. . . . . . .|. . . . . . .<br>46|



1 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

_“[Money Management] — It is not that part of your system that dictates how much you will lose on a given trade. It is not how to exit a profitable trade. It is not diversification. It is not risk control. It is not risk avoidance. It is not that part of your system that tells you what to invest. Instead, [...] it is the part of your trading system that answers the question “How much?” throughout the course of a trade.”_ 

Van K. Tharp (2007) 

### **I Introduction** 

Technical trading rules usually generate a binary series of buy (1) and sell (0) signals, but do not provide information **_how much_** of the trading budget<sup>1</sup> should be invested in every trade. The part of a trading system, which answers this particular question, is called _position sizing_ or _money management_ . The choice of the money management policy has a severe impact on trading results from both a risk and return perspective. The most basic way to follow a trading rule is _unmanaged positions_ , i.e. always to buy or sell one unit (e.g. one share) of the underlying asset.<sup>2</sup> In this case, the position size is _erratic_ because it only depends on the share price and leveraging may become very large.<sup>3</sup> To avoid this, two basic money management techniques can be applied: _absolute positions_ , i.e. to invest fixed amounts, which is widely preferred by practitioners (Potters & Bouchaud 2005); or _relative positions_ , i.e. a fixed proportion of the remaining trading capital as inspired by Kelly (1956).<sup>4</sup> In trading systems, however, the original Kelly idea is modified such that the capital allocation is a dynamic process over time: the investor follows active trading signals and sizes the trading position relative to the remaining trading budget. 

Recent academic literature on technical trading, which primarily focuses on the highly controversial issue of benefits from trading rules, spends surprisingly little attention to describe their detailed implemented position sizing. As a consequence, different views exist on the applied position sizes: Fifield, Power & Sinclair (2005), for example, suggest that in previous studies the investor had an unlimited trading budget, whereas Zhu & Zhou (2009) assume that trading positions are restricted to 100% of the remaining trading capital; and Anderson & Faff (2004) suppose that a fixed number of contracts is traded. However, it is more than doubtful that findings from studies with different implementations are comparable. In practice, experienced system developers generally are aware of the dilemma of money management: undersized positions cannot fully exploit the trading rule’s potential, but even promising trading strategies may 

> 1The trading budget is the value of the actively managed portfolio. 

> 2To show that asset price characteristics explain timing success, we applied a trading system with unmanaged positions to analyze the pure influence of the trading rule (Scholz & Walther 2011). 

> 3For example, if the remaining trading budget is low and the share price is comparatively high. 

> 4In order to achieve an optimal betting strategy, Kelly suggested reinvestment of gainings such that the expected value of the logarithm of the gambling budget is maximized. 

2 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

generate losses if too much exposure is taken.<sup>5</sup> Traders are prone to another fallacy, though: to integrate highly complex money management rules solely on the basis of backtests (Harris 2002). This practice is methodologically weak and never provides a structured insight into the impact of position sizing on the investment’s risk/return profile. 

The present study contributes to the literature by revealing the systematic impact of money management on a trading systems’ return distribution. The focus thus lies not only on identifying optimal Kelly strategies or on evaluating the wealth levels achieved. Instead, I compare two basic money management implementations applied to the prominent simple moving average trading rule: erratic positions as well as different leveraging levels of relative position sizing. Asset returns are simulated by parametric stochastic processes, with systematic variations of the most influential asset price characteristics: the trend (or drift) _µ_ , the volatility of returns _σ_ , and the first-lag serial autocorrelation parameter _ϕ_ of an AR(1) process. In contrast to standard backtests, the simulation approach allows to evaluate the return distributions of terminal results as well as the daily return distributions. Both is done by applying a wide range of popular statistical-, return-, risk-, and performance figures. 

The study documents a severe impact of money management on the overall success or failure of the trading system and a clear dependence of this impact on the asset price characteristics of the underlying. In most scenarios, smaller relative trading positions deliver comparatively high Sharpe ratios. In fact, the introduction and especially the reduction of relative position sizes offers some kind of protective element: return is sacrificed in order to limit the risks from timing, especially large drawdowns. However, a universal optimal position size as supposed by the Kelly criterion does not exist. This finding contradicts Anderson & Faff (2004), who claim empirical optima based on backtests. 

After a brief literature review, Section III explains the methodology in more detail. Section IV presents the simulation results and summarizes the risk and performance implications from managed positions. Section V concludes. 

### **II Literature Review** 

The idea of optimal position sizing goes back to Kelly (1956), who applied information theory on gambling and proved that the information transfer rate over a channel is equal to the maximum exponential growth rate of a gambler’s capital. The original Kelly criterion, however, is not directly applicable in finance since here the outcomes are typically not Bernoulli distributed, in contrast to many gambling games. It can be shown, though, that maximizing the growth rate 

> 5For example, consecutive losing trades and drawdowns may consume the initial capital before net profits can be realized. 

3 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

is formally achieved by maximizing logarithmic utility. In 1959, Latan´e put forth the idea of maximizing logarithmic utility to get an optimal growth rate in an economic context. In many publications, the U.S. mathematician Edward Thorp adapted the Kelly formula to portfolio selection (e.g. Thorp 1969, 1971, 1980, 2006, 2010, Rotando & Thorp 1992). Other works on the subject further elaborated the concepts and worked out implementable solutions (Browne 1999, Hakansson & Ziemba 1995, MacLean, Ziemba & Li 2005, McEnally 1986, Mulvey, Pauling 

& Madey 2003, Wilcox 2003 _a_ , _b_ , 2005). 

In continuous time, the Kelly criterion has two major beneficial long run properties:<sup>6</sup> the asymptotic growth rate is maximized and the time to reach a given wealth level is minimized (Breiman 1961). Although many scientists consider maximizing the log utility as superior, some economists vigorously argue against the criterion since it neglects individual investing preferences in favor of the optimal growth rate. Paul Samuelson (cf. Samuelson 1969, 1971, 1979, Merton & Samuelson 1974) points out that the application of the Kelly criterion is comparably risky due to potentially highly levered bets. It could indeed be discouraging for investors to follow the Kelly strategy since trading success is not guaranteed if the time horizon is finite and there is a significant potential to interim setback periods (MacLean, Thorp & Ziemba 2010). MacLean, Ziemba & Blazenko (1992) considered the trade-off between _growth_ by applying the Kelly criterion and _security_ in terms of drawdowns and found that the high wagers of full Kelly bets may lead to an immense reduction of wealth. However, _fractional_ Kelly strategies, which lower the fraction of the original Kelly bet proportionally, may help to overcome these issues by lowering volatility and reducing the error-proneness in the edge<sup>7</sup> calculations (MacLean, Sanegre, Zhao & Ziemba 2004, MacLean, Zhao & Ziemba 2009). Admittedly, even those who support Kelly’s idea tend to adopt fractional strategies since one of the fundamental insights of Kelly is the fact that overbetting is more harmful than underbetting, since it lowers growth but increases risk (Ziemba 2009).<sup>8</sup> MacLean, Thorp, Zhao & Ziemba (2010) confirm this perception since they find that the full Kelly approach does not stochastically dominate the fractional strategies. 

In investment practice, there are many applications for Kelly strategies:<sup>9</sup> to allocate capital between different asset classes (e.g. Heath, Orey, Pestien & Sudderth 1987), to manage the exposure of the risky asset in a portfolio (e.g. MacLean, Thorp, Zhao & Ziemba 2011), as well 

> 6Cf. MacLean, Thorp & Ziemba (2010) for an extensive discussion about the “good and the bad properties of the Kelly criterion”. 

> 7The term _edge_ denotes the individual advantage over the general public. The determination of the optimal Kelly bet relies on an estimate of the gambler’s edge: in a portfolio context, _edge_ describes the individually expected return. In a very simplistic view, the optimal full Kelly bet is the edge divided by the odds (MacLean, Thorp & Ziemba 2010), i.e. depending on the estimates of the asset’s drift and volatility. 

> 8Kelly assumes that if more risk is taken, then the investor increases the probability of extreme outcomes. 

> 9Ziemba (2005) lists famous investors which are suspected to use Kelly strategies, including John M. Keynes and Warren Buffett. 

4 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

as to size individual positions within a trading system or stocks in a portfolio (e.g. Anderson & Faff 2004). In an empirical study, MacLean et al. (2011) analyze the performance of different Kelly strategies in realistic market scenarios and confirm that in the two asset case (U.S. equity and T-bills with annual re-balancing) there is a trade-off between terminal wealth and risk: the more aggressive the Kelly bet, the higher the moments of the terminal wealth distribution. In contrast to the portfolio selection approach, Anderson & Faff (2004) consider the world of trading strategies, which implies a dynamic allocation process over time. They size the position dependent on the remaining trading capital, following the optimal-f money management policy.<sup>10</sup> Therefore, Anderson & Faff (2004) do not maximize the utility function of the investor but try to find the optimal trading size empirically, assuming that such optimum exists. They base their findings solely on return figures extracted from a backtest of five different future markets, covering a nine-year period. With this backtest approach they find that money management has an important impact on trading rule profitability, which supports the perception of real traders. But, as will be revealed in this study, they were deceived by the incidental sample of their study. 

### **III Methodology** 

#### **III.1 Parametric Simulation** 

For the analysis, asset prices are simulated by standard time series models to obtain the entire return distribution of terminal results. The simulation approach furthermore allows to systematically test the influence of different asset price characteristics on the effectiveness of money management and to exclude certain patterns which may occur in empirical data. 

A discretized random walk is used to analyze the impact from the drift (or trend) _µ_ and the standard deviation (or volatility) _σ_ on trading results. The model is given as 



where _St_ denotes the stock price at time _t_ , ∆ _t_ = 1 _/_ 250 a time interval of one day, and _εt_ a standard-normal random variable (Glasserman 2003). 

Whereas the random walk model creates normally distributed returns, stock market returns typically exhibit some well-known stylized facts such as fat tails, time varying volatility or clustering of extreme returns (McNeil, Frey & Embrechts 2005). This is confirmed by the descriptive statistics of our data sample, where all 35 markets show such non-normality. With respect to trading results, the most influential amongst the stylized facts are autocorrelated 

> 10As a practitioner, Ralph Vince published a series of books which deal with Kelly-based money management approaches (cf. Vince 1990, 1992, 1995, 2007). He tries to identify an optimal relative position size _f_ , which is denoted as _optimal-f_ and which he assumes will maximize the geometric rate of return. 

5 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

returns. Statistical tests indicate that short time lags have the strongest impact on future returns (Cerqueira 2006). Therefore, a first-order autoregressive process AR(1) is applied as given by _rt_ = _ϕ · rt−_ 1 + _εt_ , such that the full model with drift results in 



For every parametric simulation, 10,000 paths were generated, which produced stable results.<sup>11</sup> All paths comprise 2,500 data points, which corresponds to 10 years with 250 trading days. Additionally, a forerun of prices ensures the availability of an SMA value for the first day. The initial underlying asset price is always set at 100 e. 

For the parameterization of the stochastic processes, real world data is used from 35 leading global equity indices [cf. table (2)]. The database contains daily closing prices from 1 January 2000 to 31 December 2009 taken from Thomson Reuters. The extremes and averages of drift, volatility, and autocorrelation are put into the parameterized simulations.<sup>12</sup> 

#### **III.2 The Simple Moving Average Trading Rule** 

A trading rule generally converts information from past prices into a digital series of buy and sell signals. In contrast to the portfolio selection problem, the trading rule aims to forecast market movements by indicating rising (1) or falling (0) prices. Accordingly, the exposure is shifted between the risky benchmark, for example a stock market index, and the risk free alternative, i.e. cash. The price path, which is finally generated by the trading rule is referred to as _equity curve_ or _active portfolio_ . 

Simple moving averages as a trading rule are a very popular example in the academic literature. The basic idea is to follow established trends, which are detected by comparing historical price averages with the current price. SMAs have also been applied in our earlier study as an example to verify the impact from asset price parameters on trading success. Hence, this trading rule is also used in this study to ease comparison. The _SMA_ ( _d_ ) _t_ on day _t_ is the unweighted average of the previous _d_ asset prices _pi_ ( _i_ = _t − d −_ 1 _, ..., t −_ 1) excluding the present day _t_ : 



If _pt ≥ SMA_ ( _d_ ) _t_ , then the system indicates a long position, i.e. holding the risky asset. If _pt < SMA_ ( _d_ ) _t_ , then the trading budget is entirely invested in cash. In case a buy or sell signal is triggered, the positions are opened or closed completely. 

The practical implementation of an SMA trading rule needs additional specifications (besides money management): in this study, short positions are not allowed, interest on the risk free cash 

> 11A simulation with 100,000 paths was also run to verify that the results are stable enough. 

> 12It should be noted, though, that the empirical levels of the parameters may not be stable over time. 

6 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

account as well as dividend payments are not considered, and, in case of levered portfolios, credit rates do not apply. Lending is possible as long as the net position from debt and the portfolio is positive. Nevertheless, leverage may cause losses beyond the initial investment and hence imply the ruin of the investor. If a strategy on a given price path loses the total initial investment then the strategy stops, the terminal value is set to zero and registered as _total loss_ . All transactions take place at the very moment when the price of the benchmark is compared to the derived SMA.<sup>13</sup> Sufficient liquidity and an atomistic market are therefore assumed. As SMA intervals, the 5, 10, 20, 38, 50, 100, and 200 day average are applied.<sup>14</sup> 

#### **III.3 The Money Management Component** 

In this study, a trading system with unmanaged positions is compared to relative position sizing, which allows studying the sensitivity of timing results with respect to the money management policy. If one trading unit of the underlying is bought or sold every time, e.g. one stock, then the position size is unmanaged, i.e. erratic. This implies uncontrolled leveraging since the exposure is only dependent on the ratio of stock price and remaining trading budget. The relative position sizing is inspired by Kelly’s idea of relative wagers and thus reinvestment of gains. The original Kelly bet follows from maximizing the logarithmic utility function, assuming normally distributed returns and is given as _xK_ =<sup>_<u>µr−rf</u>_</sup> with mean _µr_ and variance _σr_<sup>2ofthe</sup> _σr_<sup>2</sup> risky asset _e_ and the risk-free rate _rf_ (cf. Merton 1992, MacLean et al. 2011). Following a strict Kelly bet poses various problems, both on the market and on the investor side (shifting input variables, changing risk tolerance and/or utility preferences of investors, non-normal distribution of asset returns, Black Swan events etc.). Therefore, it may be appropriate to lower the full Kelly bet in case of extreme leveraging. If a signal is triggered, feasible portions of 25%, 50%, 75%, 100%, or 125% of the remaining portfolio value are invested in the risky asset. Those fractions are constant over time and not adjusted to new return or volatility estimates.<sup>15</sup> 

Once a trade is executed, the position remains unadjusted during the whole course of the trade. After a sell signal is triggered, the position will be closed completely. It is important to note that the timing signals derive from the technical trading rule only. No additional instruments to limit the exposure such as stop-loss levels are applied. These assumptions are meant to ensure, that the pure influence from position sizing is analyzed, not risk management, transaction costs or interest rate sensitivity. 

> 13In practice, one could assume that the price of the midday auction triggers the SMA and the system buys or sells at the very next price. 

> 14A test with all in-between levels showed a rather smooth development, hence only seven nodes are used. 

> 15In general, parameters could be adjusted dynamically. 

7 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

#### **III.4 Evaluation Criteria for Trading Systems** 

The evaluation of an investment’s success has to balance _risk_ and _return_ . While Anderson & Faff (2004) focus on the return component only, this work also considers the risk element. Due to the simulation approach, the distribution of terminal results can be analyzed, which delivers the main findings and describes the investor’s real exposure. Standard backtests, which are especially popular in practice, are based on the single historical price path and hence deliver a _pathwise distribution_ only and are thus merely an estimate for the terminal result distribution. Particularly if the distributions are skewed and “reshaped”, the pathwise distributions may be a biased estimator. Nevertheless, some key figures are path-dependent; in this case, the mean over all generated paths is reported (particularly including total profits, total losses, total net results, and maximum drawdowns.) 

For the analysis of the position sizing effect, a broad range of evaluation criteria is applied (the complete set of criteria is listed in table (3)).<sup>16</sup> It turned out that the different measures mostly delivered comparable information. Even measures which are especially designed for highly non-normal (reshaped) distributions do not rank investment alternatives differently than standard measures. While this finding seems to be surprising, it is actually in-line with literature. I therefore focus on total profits, total losses, total net results, the terminal wealth relative (TWR),<sup>17</sup> maximum drawdowns, the (higher) moments of the return distribution, the value-atrisk, the Sharpe ratio, and the expected excess return (compared to a buy-and-hold investment) for the description of timing results. 

### **IV Simulation Results** 

The results section is structured as follows: first of all, I demonstrate that there is no optimal position size in trading and that empirical optima are just a statistical artifact [subsection IV.1]. Subsequently, the impact of introducing a money management policy in a trading system is revealed. Therefore, in the subsections IV.2 to IV.4, the sensitivity of trading results to position sizing is analyzed, depending on the properties of the underlying asset price process: drift, volatility, and autocorrelation. I apply a trading system with unmanaged (or erratic) positions, which is compared to relative position sizing; trading systems with different levels of relative positions sizing are also compared. The complete set of results is given in tables (4) to (21) in the Appendix. Figures (3) to (8) moreover illustrate the results for the maximum and minimum input variables, found in the empirical dataset (e.g. the maximum and minimum drift). For reasons of clarity and readability, I abstain from graphically displaying the mean 

> 16Additional figures can be provided upon request in case of particular interest. 

> 17The _terminal wealth relative_ is defined as _final balance_ divided by _initial account size_ . 

8 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

values of the input variables. Every figure shows six pictures: on the left hand side, the results for the maximum value of the input variable is shown; and on the right hand side the results for the minimum value. Each line in a graph shows the results from one specific position sizing method. The erratic position size as the point of reference is depicted in bold black (marked with squares). It should be noted that the scaling of the graphs may be very different due to the absolute performance differences. 

Figures (3), (5), and (7) present the profitability figures (total net results, mean returns, and Sharpe ratios); figures (4), (6), and (8) illustrate the corresponding risk perspective (volatility, maximum drawdowns, and value-at-risk). All path-dependent values represent the average of the 10,000 simulated paths. 

|Model|Parameter|Max|Mean|Min|
|---|---|---|---|---|
|Brownian motion with varying drift|_µ_|**22.6%**|**5.2%**|**-11.6%**|
||_σ_|26%|26%|26%|
||Kelly size|334%|77%|-118%|
|Brownian motion with varying volatility|_µ_|5.2%|5.2%|5.2%|
||_σ_|**39%**|**26%**|**16%**|
||Kelly size|34%|77%|203%|
|AR(1) with varying autocorrelation|_µ_|5.2%|5.2%|5.2%|
||_σ_|26%|26%|26%|
||_ϕ_|**0.201**|**0.025**|**-0.103**|
||Kelly size|77%|77%|77%|



**Table 1: Kelly position sizes.** The table displays the Kelly position sizes with respect to the three different scenarios, in which either the drift, the volatility, or the autocorrelation is sensitive. The Kelly size corresponds to the relative amount of risky assets compared to the remaining trading capital. Negative numbers imply short positions. 

#### **IV.1 Optimality of Fractions** 

Considering portfolio selection problems, the Kelly criterion provides optimal position sizes, at least if returns are normally distributed and the investment horizon is infinite. It is thus possible to determine optimal position sizes by applying the Kelly criterion. The question is, whether or not the Kelly formula also allows to find optimal position sizes in trading systems, in which the capital allocation is triggered by trading signals and thus develops as a dynamic process over time. To begin with, the Kelly position sizes for the scenarios analyzed in this study 

9 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

are determined, which are dependent on trend and volatility of the underlying asset. For the Brownian motion, the normality assumption is fulfilled and the application of the Kelly formula is possible in principal. Autocorrelated returns do generally not qualify for Kelly bets, however, the corresponding level for normal distributed returns is used as a reference. The full Kelly wagers are given in table (1), assuming a risk-free rate of 0%. 



**Figure 1: Terminal wealth relative dependent on fraction.** The graph displays the dependency between the TWR and the applied fraction of the trading strategy. Every line stands for a different trading rule. Interestingly, there is no optimal terminal wealth relative but a monotone gradient, by contrast to the findings of the classical portfolio application of Kelly fractions. This finding holds for different drift, volatility, and autocorrelation levels in the underlying. The example is based on _µ_ =0.052 and _σ_ = 0 _._ 26. 

To test for optimal position sizes, I follow Anderson & Faff (2004) who use the terminal wealth relative (TWR)<sup>18</sup> as success measure and plot the growth rate against the corresponding position size. The highest TWR indicates the best fraction to be used as position size. If an optimal fraction exists then the chart should show an unique peak. Anderson & Faff (2004) indeed identified distinctive concave curves and thus derived optimal fractions. However, their findings are solely based on the one realized price path of the respective underlying. In this study, I carry out the same analysis for each of the 10,000 simulated paths and take the average. Interestingly, for this average, the optimal fraction vanishes and there is a monotone gradient (cf. figure (1)). This finding holds also for the level of the Kelly fraction, which I derived from the underlying processes. Considering the single paths from the simulation, there is indeed a 

> 18The analysis was also carried out with the logarithm of the terminal wealth as in MacLean et al. (2011). As expected, this delivers similar findings. 

10 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 



**Figure 2: Terminal wealth relative dependent on fraction for single paths.** The figure on the left shows some exemplary TWR curves. Some of them are concave, most of them are not. The right-hand figure displays the development of all TWR curves: the low fractions exhibit a lower variance in trading results compared to the levered position sizes. This finding holds for different drift, volatility, and autocorrelation levels in the underlying. The example is based on _µ_ =0.052 and _σ_ = 0 _._ 26 for an SMA(38) trading rule. 

significant number of paths, which generate concave TWR curves (cf. figure (2)).<sup>19</sup> But even in case of concave TWR curves, the optimal position size is not the same for every path. First and foremost, those paths are concave, which provide unfavorable characteristics for the trading rule: by definition, in a simulation half of the paths emerge below average with respect to the expected drift and some may even show extremely negative courses. The higher the leveraging, the more sensitive the trading system reacts to the characteristics of the underlying path, i.e. high profits are accompanied by severe losses in the trading account. In case of below average drifts, the system thus generates very poor results for high fractions, as expressed e.g. by low total net result or small hit-ratios. On average, however, these effects cancel each other out and leave a monotone gradient. Ex-ante, there is thus no optimal fraction or position size with respect to the TWR (except for corner solutions). The monotone gradient can be found in all simulations, independent of drift, volatility, and autocorrelation. Put in a nutshell: the empirical optimum of a single path is a statistical artifact. 

#### **IV.2 Position Sizing Effects for Different Trend Levels** 

To analyze the influence of position sizing on trading results, a geometric Brownian motion is used in the first place. I am using this stochastic process to model different market conditions: a bullish setup, represented by the maximum (22.6% p.a.) drift in the dataset; a moderate increasing trend, i.e. the mean (5.2% p.a.) in the sample; and a bearish market, applying the 19As an example, take the SMA38 trading rule where about 38% of paths show a concave TWR curve. 

11 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

minimum drift (-11.6% p.a.) from the sample.<sup>20</sup> All drift levels are combined with the empirical mean volatility level of 26% p.a. Tables (4) to (9) contain the detailed results. 

**Relative versus Erratic Positions** In bull markets, the total net result is always positive and generally highest if no money management component is applied. In bear markets, the outcome of erratic trading positions is negative but on comparable levels as the 100% fraction. If we now compare the erratic strategy with the flock of relative ones by considering the relative position of the black curve (with squares) against the others, we find similar effects for the mean return. But the erratic position is always inferior to the 100% relative position size, independent of the market conditions. 

With respect to risk-adjusted returns, however, trading systems with relative position sizing clearly deliver better results than erratic positions. While this clearly holds for bull markets, this is also the case in bear markets on closer examination. For negative returns, the Sharpe ratio is misleading, since higher volatilities reward a higher ratio (Scholz & Wilkens 2006). If the Sharpe ratio is decomposed then it becomes clear that unmanaged positions always raise the volatility of the equity curve, especially for short term SMAs. The reason for the increase is the high and uncontrolled leverage, which may occur if the position size is erratic. Since the short term SMAs trigger more trades, the risk of trading extremely levered positions is higher than in the less reactive long term SMAs. Furthermore, the introduction of (relative) money management improves skewness and kurtosis of the trading system’s return distribution [see table (4) to (9) in Appendix]. The maximum drawdowns, by contrast to volatilities, are on high but comparable levels in bear markets, if the 100% and 125% fractions are considered as a reference. In the bullish scenario, the relative position sizing is capable of effectively reducing the risk from drawdowns. In rising markets, the value-at-risk levels again show the riskiness of erratic positions, especially if short term SMAs are applied. The introduction of money management based on relative positions clearly improves the tail risk, except for long SMAs in bullish environments. In bear markets, only the levered f-125% delivers higher tail risks than erratic positions. If compared to the underyling asset, erratic positions lead to similar results as the highly levered (f-125%) portfolio: a small underperformance in terms of expected excess return in bullish markets; and comparatively poor expected excess returns if medium or negative drifts are applied. 

**Different Leverage Levels** Relative positions are assumed to correspond to Kelly strategies, which also recommend constant relative fractions as position sizes. In comparison to the full 

> 20To correct for the volatility drag, the drifts _µe_ measured in the descriptive analysis must be transformed into _µa_ = _µe_ + 2<sup><u>1</u></sup><sup>_· σ_2togeneratetheapplieddriftlevels.</sup> 

12 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 



**Figure 3: Return figures dependent on drift.** The graphs show the total net results, the mean returns, and the Sharpe ratio for managed and unmanaged position sizing. On the left, the results for maximum drift are displayed. On the right, the results for the minimum drift are shown. 

Kelly position size, the applied positions are undersized in bull markets (Kelly ratio of 334%) and oversized in bear markets (Kelly ratio of -118%). In markets with the average trend of 5.2% p.a., the applied range of f-25% to f-125% lies near the full Kelly size of f-77% (in terms of portfolio fractions). 

Regarding the absolute outcomes in case of positive drifts, the different fractions behave proportional to the drift component: the higher the leveraging, the higher the profits, the losses and hence total net results. If negative drifts apply, however, then all systems lose likewise, with high fractions generating more losses than low fractions. As expected, those findings are also reflected by the mean returns: in bullish markets ( _µ_ = 0 _._ 226), the mean return of the distribution of terminal results falls as the leverage decreases; if medium drifts apply, then the 

13 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 



**Figure 4: Risk figures dependent on drift.** The graphs show the volatility, the maximum drawdowns, and the value-at-risk (5%) for managed and unmanaged position sizing. On the left, the results for maximum drift are displayed. On the right, the results for the minimum drift are shown. 

mean return slightly increases for lowered leverage levels; and in case of negative trends, the mean return clearly rises with shrinking leverage. Considering the risk, reducing the fractions effectively lowers volatility, maximum drawdowns and value-at-risk from trading. Especially if the fractions are lowered below 50% then the trading system largely displays less risk than the buy-and-hold approach. Similar findings hold for the higher moments: skewness is improved as leverage is lowered and kurtosis is stable between 2.91 and 4.56. Interestingly, the lower the leverage levels, the broader the kurtosis range. As a result, low fractions generally yield superior Sharpe ratios, independent from the drift level.<sup>21</sup> Regarding the underlying asset as standard of 

> 21With the SMA(200) as the only exception in case of positive drifts. Here, the Sharpe ratio is slightly larger for higher levered systems. 

14 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

comparison, the reduction of leveraging leads to typical protection characteristics: the expected excess returns suffer in bullish markets but flourish in bearish ones. 

**Conclusions** If positive drift levels apply, then the erratic position sizing seems to be the most profitable implementation. In case of negative trends, however, the profit and loss figures lie well within the range given from relative position sizing. The application of relative position sizing in trading systems effectively lowers the risk from timing compared to erratic positions. The exact benefit is dependent on the drift level, though. In general, smaller position sizes have a protection effect, i.e. mean returns are smaller but risk is effectively reduced. Interestingly, the lowest fraction of 25% has generally better Sharpe ratios than highly levered portfolios (f-125%), i.e. leveraging does not pay a risk premium. This finding holds in every market scenario. 

#### **IV.3 Position Sizing Effects for Different Volatility Levels** 

For erratic position sizing, we found a negative impact from rising volatility on trading results (Scholz & Walther 2011). To analyze the sensitivity of trading systems with different position sizes on volatility, a Brownian motion is again applied (with mean drift level of 5.2% p.a.). I check for the maximum (39%), the mean (26%), and the minimum (16%) annual volatility levels given in the empirical dataset.<sup>22</sup> The results from volatility influence can be found in tables (10) to (15) and figures (5) to (6). 

**Relative versus Erratic Positions** If the total net result is considered then the erratic position sizing is amongst the most profitable implementations, no matter how volatile the underlying is. In terms of mean returns, the picture however changes: the erratic position sizing suffers in case of high and medium volatilities of the underlying. This is due to a significant number of total losses if erratic position sizing is applied. Log-returns weigh the impact of a total loss stronger than the P&L of a trading account. Only in case of low volatility levels in the markets, the erratic position sizing generates returns, which are akin to the f-100% and f- 125% implementation. Similar findings hold for risk-adjusted returns: trading systems without money management clearly underperform in the given volatility setups. A high volatility level is generally disadvantageous for erratic position sizing under risk aspects: the volatility of the equity curve is higher, the maximum drawdowns are larger, and the value-at-risk (5%) signals a high tail-risk. This is confirmed by skewness as well as kurtosis of the equity curve’s return distribution, which reach extreme levels. Here, introducing relative position sizing really adds 

> 22To cope with the drag effects, the mean drift of 0.052 was adjusted with _µa_ = _µe_ + <u>12</u><sup>_· σ_</sup> _a_<sup>2(with</sup><sup>_σ_</sup> _a_<sup>2beingthe</sup> applied volatility level in the simulation) since I intended to measure the volatility effect on the timing result only, not the mixed influence including effects on the drift component. 

15 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

value. In case of comparably low volatility levels of the underlying, however, the introduction of a relative money management policy has a minor impact; the risk from erratic positions still approximates the risk from the 125% levered position size and is thus in the upper range. 



**Figure 5: Return figures dependent on volatility.** The graphs show the total net results, the mean returns, and the Sharpe ratio for managed and unmanaged position sizing. On the left, the results for maximum volatility are displayed. On the right, the results for the minimum volatility are shown. 

**Different Leverage Levels** The applied levels of leverage may again be compared to the corresponding Kelly wagers. A low volatility environment requires high leveraging (203%), whereas fractions of 34% have to be chosen for high volatilities; for the medium volatility level, a fraction of 77% corresponds to the Kelly size. Except for the highest fractions, those levels are covered by the applied position sizes. 

16 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 



**Figure 6: Risk figures dependent on volatility.** The graphs show the volatility, the maximum drawdowns, and the value-at-risk (5%) for managed and unmanaged position sizing. On the left, the results for maximum volatility are displayed. On the right, the results for the minimum volatility are shown. 

As one would expect, lower fractions decrease the profit potential in the trading account: the total net result, as well as total profits and losses shrink with the position size, independent of volatility levels. This finding is also reflected by the mean returns, which slightly decrease in case of deleveraging.<sup>23</sup> Interestingly, if the risk-adjusted returns are examined then the lower fractions are more appealing: their Sharpe ratios are slightly higher compared to the levered positions. In turbulent markets, these differences are bigger than in calm markets. Since lower 

> 23High volatility of the underlying asset generally leads to higher trading profits in absolute terms, but not to higher mean returns. Surprisingly, the strategy’s success with relative position sizing is nearly independent from the leverage. This effect arises from the log-return effect. Since the erratic approach produces a high number of paths, which lead to a total loss, it delivers extremely negative return values in the log-return calculation. 

17 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

fractions yield less returns, the source must be the risk side. Relative position sizing indeed effectively reduces the volatility of the underlying. Downsizing of the position size additionally increases this effect. Moreover, the higher the input volatility, the stronger the reduction effect. This finding is also confirmed by the maximum drawdowns, which may be disastrous in case of high volatility in the underlying returns and highly levered positions sizes. A lower fraction can attenuate the drawdowns to more acceptable levels, especially in highly volatile environments. The value-at-risk levels also indicate the risk reduction by delevered fractions: tail risks slightly decrease if the position size is lowered. With respect to the underlying benchmark, the expected excess returns are rather insensitive to the tested volatility levels. 

**Conclusions** If only the total net result is considered, then the erratic positions work fine. The moments of the trading system’s return distribution, however, clearly show that relative position sizing is superior: especially if the underlying exhibits high volatility, the money management effectively decreases risks. Only in case of low volatile underlyings, the reduction effect is negligible. Low fractions further reduce the moments but only slightly improve the risk-adjusted returns. 

#### **IV.4 Position Sizing Effects for Different Autocorrelation Levels** 

In order to analyze the relevance of autocorrelation, an AR(1) process is used to generate autocorrelated return series. In all simulations, the drift is set at the empirical mean return level of 5.2% p.a. and the volatility at the empirical mean volatility level of 26% p.a.<sup>24</sup> Three different lag-one autocorrelation levels are examined: the maximum (0.2064), the mean (0.025), and the minimum (-0.1027) of the empirical estimates from the dataset. An overview of the results can be found in tables (16) to (21). Figures (7) and (8) display the findings for the highest and lowest autocorrelation levels. 

**Relative versus Erratic Positions** In highly autocorrelated markets ( _ϕ_ = 0 _._ 2064), erratic positions generate positive total net results, which are similar to those of 50% to 75% fractions. In case of negative autocorrelation, however, the success of erratic positions extremely depends on the moving average window (low total and mean returns for short windows, higher for long ones). The SMA(38) seems to be the pivotal point. Especially short term SMA trading rules are prone to whiplash signals since they respond quickly to the underlying. Erratic positions aggravate this effect by uncontrolled leveraging; hence they are particularly susceptible to repeated loss trades. The mean returns generally confirm these findings; due to the log-return calculation, however, 

> 24Once more, the applied drifts have to be corrected with _µa_ = _µe ·_ (1 _− ϕ_ ); and the volatilities _σa_ = _σe_ 2<sup>_·_(1</sup><sup>_−ϕ_2).</sup> 

18 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

erratic positions generate inferior mean returns.<sup>25</sup> If risk-adjusted returns are considered, the picture slightly changes such that the erratic positions generate inferior Sharpe ratios due to lower returns and increased volatility. Looking at the risk measures, erratic position sizing produces extreme risks in the negative autocorrelation scenario: volatility is excessive, maximum drawdowns may be disastrous, and the value-at-risk indicates a very high tail risk. This is again due to the susceptibility to whiplash signals. Consequently, in case of positive autocorrelation, trading systems without position sizing do not cause extended risks. If the trading systems’ returns are compared with those from the underlying asset, the expected excess returns imply that the erratic positions are inferior to the money-managed trading systems, no matter which leveraging is applied or which autocorrelation level is used for the simulations. 

**Different Leverage Levels** Although underlyings with autocorrelated returns do generally not qualify to implement Kelly strategies, the corresponding level for normally distributed returns of 77% is used as a reference. That way, the impact on trading results can be analyzed in case Kelly sizing is nevertheless applied by investors. 

For positive autocorrelation, the total net result is the higher, the higher the leverage. Especially the short term SMA trading rules benefit, while the long term SMAs are less sensitive due to a smaller number of trades. Higher leveraging is unfavorable for short term SMAs in this scenario, however, long term SMAs even benefit from leveraging since they are less sensitive to whiplash signals. These findings are confirmed by the mean returns of the distribution of terminal results. Independent from the autocorrelation level, a lower leverage improves the Sharpe ratio of the trading system. Compared to the underlying, however, the f-125% trading system delivers the highest expected excess returns against the benchmark if positive autocorrelation applies. The picture changes for negative autocorrelated underlyings and the lower fractions show the least poor expected excess returns. The implementation of a relative money management component effectively controls the risk of the active portfolio: excess volatility against the underlying never occurs, even if highly levered f-125% positions are traded. The skewness improves in the trading portfolio and the kurtosis rises only moderately (4.64), which implies that the number of extreme outcomes is only slighly raised. Lower fractions of the position size moreover reduce the maximum drawdowns and the value-at-risk from timing. The only exception is the value-at-risk for high serial autocorrelation levels in the underlying: here, the short term SMA trading rules benefit if they are combined with highly levered trading positions. 

**Conclusions** In terms of profitability, highly levered portfolios are preferable if markets exhibit positive autocorrelation, while unlevered portfolios are better suited if autocorrelation is 

> 25Total net results which imply high losses in the trading account deliver highly negative return figures. 

19 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 



**Figure 7: Return figures dependent on autocorrelation.** The graphs show the total net results, the mean returns, and the Sharpe ratio for managed and unmanaged position sizing. On the left, the results for maximum autocorrelation are displayed. On the right, the results for the minimum autocorrelation are shown. 

negative (this is consistent with the fact that SMA-timing is highly successful in autocorrelated markets but suffers from negative autocorrelation). The effect remains, that in positive autocorrelated markets short term SMAs are preferable (and long term SMAs if negative autocorrelation applies). Risk can be effectively controlled by relative position sizing, especially for lower fractions. Erratic positions are inferior, particularly if markets show negative autocorrelation. The cautious approach with trading lower fractions thus generates the highest risk-adjusted returns. 

20 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 



**Figure 8: Risk figures dependent on autocorrelation.** The graphs show the volatility, the maximum drawdowns, and the value-at-risk (5%) for managed and unmanaged position sizing. On the left, the results for maximum autocorrelation are displayed. On the right, the results for the minimum autocorrelation are shown. 

### **V Conclusions** 

The study shows that position sizing has a considerable impact on the performance of SMA timing strategies. Academic papers, which spend only little attention to detailed money management, miss an important influencing factor and run the risk of misinterpreting or insufficiently understanding the empirical results. A clear recommendation for optimal position sizes is given by the well known Kelly ratio, which is applicable in gambling and long-term portfolio allocation. However, the application of relative positions in trading systems, is different to the “traditional” Kelly strategies, in which the capital is always invested with a fractional or full Kelly proportion 

21 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

in the risky asset. An active trading strategy proposes investments only for specific time periods according to the signal. 

A major finding of the study is that there is no optimal position size in a trading system which is based on the SMA trading rule. By contrast to portfolio optimization, where generally a maximum of the terminal wealth relative can be found, the terminal wealth relative of the trading account is linear and shows no turning point at the Kelly ratio. Findings from other authors, which suggest that there is an optimal position size, rely on single paths of a backtest. This is, however, misleading. If the terminal distribution of total net results is considered, then the concave shape vanishes. Hence, there seems to be no optimal money management approach; the choice is rather dependent on the underlying asset price characteristics. For instance, if the underlying shows positive trends, high serial autocorrelation, and/or low volatility, then erratic or highly levered position management provide fair results. In contrast, if negative drifts, negative autocorrelation, and/or high volatilities apply, then relative position management is the better choice. In any case, the introduction of relative money management effectively reduces the risks in the actively managed portfolio. The exclusive focus on return aspects from money management hence fails to meet the real strong point of relative position sizing and the major distinguishing feature to unmanaged positions. If relative position sizing is implemented, however, then the difference between the diverse fractions is surprisingly small. Interestingly, the lowest fraction f-25% shows the highest Sharpe ratios amongst the tested portions but not the highest returns. In terms of risk-adjusted returns, this confirms Kelly’s suggestion that overbetting is more harmful than underbetting. In a nutshell, _all-in_ actually seems not to be the wisest investment strategy for investors. 

### **Acknowledgments** 

I am grateful to Leonard C. MacLean from Dalhousie University and Ursula Walther from Frankfurt School of Finance & Management for helpful discussion and suggestions. The dataset from Thomson Reuters is very much appreciated. Furthermore, I want to thank the members of the Centre for Practical Quantitative Finance and the participants of the 19th Triennial Conference of the International Federation of Operational Research Societies (Melbourne, Australia) for comments and feedback. 

### **References** 

- Anderson, J. A. & Faff, R. W. (2004), ‘Maximizing Futures Returns Using Fixed Fraction Asset Allocation’, _Applied Financial Economics_ **14** , 1067–1073. 

- Breiman, L. (1961), Optimal Gambling System for Favorable Games, _in_ ‘Proceedings of the 4th Berkely Symposium on Mathematical Statistics and Probability 1’, pp. 63–78. 

- Browne, S. (1999), ‘The Risk and Rewards of Minimizing Shortfall Probability’, _Journal of Portfolio Management_ **25** (4), 76–85. 

22 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

- Cerqueira, A. (2006), Autocorrelation in Daily Stock Prices. Working Paper, 4th PFN Conference. 

- Fifield, S. G. M., Power, D. M. & Sinclair, C. D. (2005), ‘An Analysis of Trading Strategies in Eleven European Stock Markets’, _The European Journal of Finance_ **11** (6), 531–584. 

- Glasserman, P. (2003), _Monte Carlo Methods in Financial Engineering_ , first edn, Springer, New York. 

- Hakansson, N. H. & Ziemba, W. (1995), Capital Growth Theory, _in_ ‘Handbooks in OR & MS’, Elsevier, North Holland, pp. 65–86. 

- Harris, M. (2002), ‘Facing the Facts of Risk and Money Management’, _Active Trader_ **May** , 33– 35. 

- Heath, D., Orey, S., Pestien, V. & Sudderth, W. (1987), ‘Minimizing or Maximizing the Expected Time to Reach Zero’, _SIAM Journal on Control and Optimization_ **25** (1), 195–205. 

- Kelly, J. L. (1956), ‘A New Interpretation of Information Rate’, _The Bell System Technical Journal_ **35** , 917–926. 

- Latan´e, H. A. (1959), ‘Criteria for Choice Among Risky Ventures’, _The Journal of Political Economy_ **67** (2), 144–155. 

- MacLean, L. C., Sanegre, R., Zhao, Y. & Ziemba, W. T. (2004), ‘Capital Growth with Security’, _Journal of Economic Dynamics and Control_ **28** (5), 937–954. 

- MacLean, L. C., Thorp, E. O., Zhao, Y. & Ziemba, W. T. (2010), Medium Term Simulations of the Full Kelly and Fractional Kelly Investment Strategies. Working Paper. 

- MacLean, L. C., Thorp, E. O., Zhao, Y. & Ziemba, W. T. (2011), ‘How Does the Fortune’s Formula Kelly Capital Growth Model Perform?’, _The Journal of Portfolio Management_ **37** (4), 96– 111. 

- MacLean, L. C., Thorp, E. O. & Ziemba, W. T. (2010), Good and Bad Properties of the Kelly Criterion. Working Paper. 

- MacLean, L. C., Zhao, Y. & Ziemba, W. (2009), Optimal Capital Growth with Convex Loss Penalties. Working Paper, Dalhouse University. 

- MacLean, L. C., Ziemba, W. T. & Blazenko, G. (1992), ‘Growth versus Security in Dynamic Investment Analysis’, _Management Science_ **38** (11), 1562–1585. 

23 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

- MacLean, L. C., Ziemba, W. T. & Li, Y. (2005), ‘Time to Wealth Goals in Capital Accumulation and the Optimal Trade-Off Growth versus Security’, _Quantitative Finance_ **5** (4), 343–357. 

- McEnally, R. W. (1986), ‘Latan´e’s Bequest: The Best of Portfolio Strategies’, _Journal of Portfolio Management_ **12** (2), 21–30. 

- McNeil, A. J., Frey, R. & Embrechts, P. (2005), _Quantitative Risk Management_ , Princeton Series in Finance, New Jersey. 

- Merton, R. C. (1992), _Continuous-Time Finance_ , Blackwell Publishers, Oxford, Melbourne, Berlin. 

- Merton, R. C. & Samuelson, P. A. (1974), ‘Fallacy of the Log-Normal Approximation to Optimal Portfolio Decision-Making Over Many Periods’, _Journal of Financial E_ **1** , 67–94. 

- Mulvey, J. M., Pauling, W. R. & Madey, R. E. (2003), ‘Advantages of Multiperiod Portfolio Models’, _Journal of Portfolio Management_ **29** (2), 35–45. 

- Potters, M. & Bouchaud, J.-P. (2005), ‘Trend Followers Lose More Often Than They Gain’, _Wilmott Magazine_ **26** , 58–63. 

- Rotando, L. M. & Thorp, E. O. (1992), ‘The Kelly Criterion and the Stock Market’, _The American Mathematical Monthly_ **99** (10), 922–931. 

- Samuelson, P. A. (1969), ‘Lifetime Portfolio Selection by Dynamic Stochastic Programming’, _Review of Economics and Statistics_ **51** , 239–246. 

- Samuelson, P. A. (1971), ‘The “Fallacy” of Maximizing the Geometric Mean in Long Sequences of Investing or Gambling’, _Proceedings of the National Academy of Sciences of the United States of America_ **68** (10), 2493–2496. 

- Samuelson, P. A. (1979), ‘Why We Should not Make Mean Log of Wealth Big Though Years to Act are Long’, _Journal of Banking & Finance_ **3** (4), 305–307. 

- Scholz, H. & Wilkens, M. (2006), ‘Die Marktphasenabh¨angigkeit der Sharpe Ratio — Eine empirische Untersuchung f¨ur deutsche Aktienfonds’, _Zeitschrift f¨ur Betriebswirtschaft_ **76** (12), 1275–1302. 

- Scholz, P. & Walther, U. (2011), The Trend is not Your Friend! Why Empirical Timing Success is Determined by the Underlying’s Price Characteristics and Market Efficiency is Irrelevant. CPQF Working Paper No. 29, Frankfurt School of Finance & Management. 

24 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

- Tharp, V. K. (2007), _Trade Your Way to Financial Freedom_ , second edn, McGraw-Hill, New York. 

- Thorp, E. O. (1969), ‘Optimal Gambling Systems for Favorable Games’, _Review of the International Statistical Institute_ **37** (3), 273–293. 

- Thorp, E. O. (1971), Portfolio Choice and the Kelly Criterion, _in_ ‘Business and Economics Statistics Section’, Proceedings of the American Statistical Association, pp. 215–224. 

- Thorp, E. O. (1980), ‘The Kelly Money Management System’, _Gambling Times_ pp. 91–92. 

- Thorp, E. O. (2006), The Kelly Criterion in Blackjack, Sports Betting, and the Stock Market, _in_ S. Zenios & W. Ziemba, eds, ‘Handbook of Asset and Liability Management’, Vol. 1, Elsevier, North Holland, pp. 387–428. 

- Thorp, E. O. (2010), Understanding the Kelly Criterion, _in_ L. MacLean, E. Thorp & W. Ziemba, eds, ‘The Kelly Capital Growth Investment Criterion: Theory and Practice’, World Scientific Press, Singapore. 

- Vince, R. (1990), _Portfolio Management Formulas: Mathematical Trading Methods for the Futures, Options, and Stock Markets_ , John Wiley & Sons, New York. 

- Vince, R. (1992), _The Mathematics of Money Management: Risk Analysis Techniques for Traders_ , John Wiley & Sons, New York. 

- Vince, R. (1995), _The New Money Management: A Framework for Asset Allocation_ , John Wiley & Sons, New York. 

- Vince, R. (2007), _The Handbook of Portfolio Mathematics: Formulas for Optimal Allocation & Leverage_ , John Wiley & Sons, New York. 

- Wilcox, J. W. (2003 _a_ ), ‘Harry Markowitz and the Discretionary Wealth Hypothesis’, _Journal of Portfolio Management_ **29** (3), 58–65. 

- Wilcox, J. W. (2003 _b_ ), ‘Risk Management: Survival of the Fittest’, Wilcox Investment Inc. Wilcox, J. W. (2005), ‘A Better Paradigm of Finance’, _Finance Letters_ **3** (1), 5–11. 

- Zhu, Y. & Zhou, G. (2009), ‘Technical Analysis: An Asset Allocation Perspective on the Use of Moving Averages’, _Journal of Financial Economics_ **92** , 519–544. 

- Ziemba, W. T. (2005), ‘The Symmetric Downside Risk Sharpe Ratio and the Evaluation of Great Investors and Speculators’, _The Journal of Portfolio Management_ **32** (1), 108–122. 

- Ziemba, W. T. (2009), Utility Theory for Growth versus Security. Working Paper. 

25 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

### **Appendix** 

|**Country**|**Index**|**RIC**|**Region**|**Market Status**|**Remark**|
|---|---|---|---|---|---|
|Argentina|MERVAL|.MERV|South America|Frontier Emerging||
|Australia|All Ordinaries|.AORD|Oceania|Developed||
|Austria|ATX|.ATX|Central Europe|Developed||
|Belgium|BEL-20|.BFX|Western Europe|Developed||
|Brazil|BOVESPA|.BVSP|South America|Advanced Emerging|BRIC|
|Canada|S&P TSX 60|.TSE60|North America|Developed||
|China|HSCE|.HSCE|East Asia|Secondary Emerging|BRIC|
|Europe|DJ EuroStoxx 50|.STOXX50E|Europe|Developed||
|France|CAC 40|.FCHI|Western Europe|Developed||
|Germany|DAX 30|.GDAXI|Central Europe|Developed||
|Greece|Athex 20|.ATF|Southeast Europe|Developed||
|Hong Kong|Hang Seng|.HSI|East Asia|Developed||
|Hungary|BUX|.BUX|Eastern Europe|Advanced Emerging||
|India|S&P CXN NIFTY|.NSEI|South Asia|Secondary Emerging|BRIC|
|Indonesia|JSX Composite|.JKSE|Southeast Asia|Secondary Emerging|Next 11|
|Italy|MIB 30|.FTMIB|Southern Europe|Developed||
|Japan|Nikkei 225|.N225|East Asia|Developed||
|Mexico|IPC|.MXX|Central America|Advanced Emerging|Next 11|
|The Netherlands|AEX|.AEX|Western Europe|Developed||
|Pakistan|KSE 100|.KSE|South Asia|Secondary Emerging|Next 11|
|Peru|Lima General Index|.IGRA|South America|Secondary Emerging||
|The Philippines|PSEi|.PSI|Southeast Asia|Secondary Emerging|Next 11|
|Poland|WIG 20|.WIG20|Eastern Europe|Advanced Emerging||
|Russia|RTS|.IRTS|Eastern Europe|Secondary Emerging|BRIC|
|Saudi Arabia|Tadawul FF Index|.TASI|Arabia|n/a||
|Singapore|STI|.FTSTI|Southeast Asia|Developed||
|South Africa|JSE Top 40|.FTJ20USD|Africa|Advanced Emerging||
|South Korea|KOSPI|.KS11|East Asia|Developed|Next 11|
|Spain|IBEX 30|.IBEX|Southwest Europe|Developed||
|Sweden|OMX Stockholm 30|.OMXS30|Northern Europe|Developed||
|Switzerland|SMI|.SSMI|Central Europe|Developed||
|Thailand|SET|.SETI|Southeast Asia|Secondary Emerging||
|Turkey|ISE 100|.XU100|Arabia|Secondary Emerging|Next 11|
|U.K.|FTSE 100|.FTSE|Western Europe|Developed||
|U.S.A.|S&P 500|.GSPC|North America|Developed||



**Table 2: The selection of 35 leading equity indices.** The table shows the 35 selected countries as well as their corresponding leading equity index. Moreover, their geographical location is given. All data is taken from Thomson Reuters. RIC denotes the Reuters Instrument Code. The rating of the market status is taken from the FTSE list and divided into 4 classes: developed, advanced emerging, secondary emerging and frontier markets. 

26 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

**Table 3: Overview of applied evaluation criteria.** For the analyses, a rich selection of different evaluation criteria are applied. It turned out that the different criteria pointed largely in the same direction. The results of the study are hence based on the relevant findings only. 

|**Criterion**|**Brief Description**|
|---|---|
|**Trade Statistics**||
|**Profits and Losses**||
|Sum of profits|Accumulated single profit trades of the trading system|
|Sum of losses|Accumulated single loss trades of the trading system|
|Total (net) result|Net profit or loss of the trading system|
|Terminal wealth relative|Total net result divided by initial capital|
|Biggest win|Biggest single profit trade of the trading system|
|Biggest loss|Biggest single loss trade of the trading system|
|Average win trade|Mean of the single profit trades|
|Average loss trade|Mean of the single loss trades|
|Average trade|Mean of all trades|
|Best result|Best timing result over all paths|
|Worst result|Worst timing result over all paths|
|Average result|Mean timing result over all paths|



|**Trades**||
|---|---|
|Number of transactions|Total number of all open and closing transactions|
|Number of profits|Total number of single profit trades|
|Number of losses|Total number of single loss trades|
|Hit ratio|Total number of profit trade divided by total number<br>of trades|



|**Equity Curve Analysis**||
|---|---|
|All-time-high|Price and point in time of all-time-high|
|All-time-low|Price and point in time of all-time-low|



_continued on next page_ 

27 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Criterion**|**Brief Description**|
|---|---|
|**Psychological key figures**||
|Maximal drawdown|Maximal distance between global peak and valley of<br>price path|
|Mean drawdown|Mean distance between local peaks and valleys of<br>price path|
|Longest sequence of losses|Longest series of loss trades only|
|Average duration of a profit trade|Mean time of investment in a profit trade|
|Average duration of a loss trade|Mean time of investment in a loss trade|
|**Return distribution measures**||
|Mean of returns|First moment of return distribution|
|Median of returns|Numeric value separating the probability<br>distribution in two halves.|
|Skew of returns|Third moment of return distribution|
|Kurtosis of returns|Fourth moment of return distribution|
|**Risk measures**||
|Variance|Second moment of return distribution|
|Volatility|Standard deviation of returns|
|Expected shortfall|Expresses the expected loss with respect to an<br>individual threshold|
|Semivariance|Defined as expected squared shortfall|
|Downside deviation|Square root of the semivariance, which corresponds|
||to “downside” counterpart of the standard deviation|
|Tracking error|Standard deviation of the difference between the|
||portfolio and benchmark returns|
|**Expected excess measures**|(based on terminal distribution, compared to benchmark)|
|Expected excess return|Expected over-return from timing|
|Expected excess volatility|Expected higher volatility in the equity curve|



_continued on next page_ 

28 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Criterion**|**Brief Description**|
|---|---|
|**Performance measures**||
|Sharpe ratio|Ratio of return to standard deviation|
|Sortino ratio|Ratio of return to downside deviation|
|Calmar ratio|Ratio of returns to maximum drawdown|
|Sterling ratio|Ratio of returns to (increased) adjusted<br>maximum drawdown|
|Information ratio|Ratio of residual return to residual risk|
|Omega ratio|Ratio of probability-weighted profits to probability-<br>weighted losses with respect to an individual threshold.|
|RINA index|Combines the total result, the time in the market and<br>mean drawdown to a reward-risk ratio.|
|**Sequence analysis of timing**||
|Runs analysis|Test if the timing returns are stochastic|
|Autocorrelation|Serial autocorrelation of first lag of timing returns|
|**Miscellaneous**||
|Stochastic dominance|Form of stochastic ordering that requires only limited<br>preference assumptions|



29 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|-0.1897|-0.1883|-0.1862|-0.1833|-0.1817|-0.1756|-0.1648|-0.0416|-0.0415|-0.0413|-0.0411|-0.0410|-0.0405|-0.0397|0.1053|0.1053|0.1056|0.1057|0.1059|0.1064|0.1072|g rules with f-25% position|to an initial investment of||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|0.6673|0.6671|0.6548|0.6454|0.6393|0.6080|0.5552|0.2075|0.2075|0.2044|0.2023|0.1995|0.1955|0.1895|-0.2406|-0.2439|-0.2409|-0.2455|-0.2455|-0.2460|-0.2417|A tradin|h respect||
|**VaR(5%)**|0.0082|0.0085|0.0090|0.0092|0.0100|0.0106|0.0123|-0.0148|-0.0152|-0.0152|-0.0150|-0.0157|-0.0162|-0.0169|-0.0341|-0.0339|-0.0329|-0.0319|-0.0311|-0.0287|-0.0262|of the SM|e in e wit||
|**Kurt**|3.02|3.01|3.06|3.08|3.16|3.53|4.56|3.04|3.02|3.01|3.06|3.16|3.41|4.21|3.04|3.03|3.06|3.14|3.26|3.51|4.17|results|iures ar||
|**Skew**|0.08|0.12|0.20|0.25|0.31|0.51|0.88|0.10|0.11|0.18|0.25|0.30|0.47|0.76|0.09|0.12|0.19|0.28|0.33|0.50|0.69|trade|loss fig||
|**Vola**|0.0524|0.0545|0.0587|0.0640|0.0671|0.0807|0.1077|0.0489|0.0493|0.0510|0.0526|0.0538|0.0575|0.0634|0.0456|0.0448|0.0443|0.0429|0.0421|0.0400|0.0372|hows the|ofit and||
|**Mean Rtrn**|0.0349|0.0363|0.0384|0.0413|0.0429|0.0491|0.0598|0.0101|0.0102|0.0104|0.0106|0.0107|0.0112|0.0120|-0.0110|-0.0109|-0.0107|-0.0105|-0.0103|-0.0098|-0.0090|The table s|ark. The pri||
|**Max DD**|0.1021|0.1033|0.1063|0.1097|0.1117|0.1203|0.1395|0.1464|0.1473|0.1495|0.1522|0.1537|0.1582|0.1664|0.2288|0.2270|0.2237|0.2196|0.2162|0.2064|0.1925|**t levels.**|g benchm||
|**Tot. Result**|43.79|45.98|49.39|54.34|57.12|68.95|94.05|12.00|12.12|12.46|12.80|12.96|13.80|15.16|-9.46|-9.45|-9.24|-9.15|-9.01|-8.63|-7.96|**ifferent drif**|the underlyin||
|**Tot. Loss**|-94.58|-75.21|-58.24|-44.78|-39.77|-28.83|-20.07|-88.96|-71.02|-55.26|-43.01|-38.44|-28.74|-21.79|-84.60|-67.78|-52.84|-41.23|-36.86|-27.39|-20.16|**sults of df**|fnt drifts of||
|**Tot. Prof.**|138.37|121.18|107.63|99.12|96.89|97.78|114.12|100.95|83.13|67.72|55.80|51.41|42.54|36.94|75.14|58.33|43.60|32.07|27.85|18.76|12.20|**% trade re**|t on differe||
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**4: f-25**|ependen||
|_µ_|0.259|0.259|0.259|0.259|0.259|0.259|0.259|0.086|0.086|0.086|0.086|0.086|0.086|0.086|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|**Table **|sizing d|e 100.|



30 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|sizing de  f<br>of e 100|**Table 5**|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|0.086|0.086|0.086<br>0.086<br>0.086|0.086|0.086|0.259|0.259|0.259|0.259|0.259|0.259|0.259|_µ_<br>|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|pendent on differen<br>.|**: f-50% trade re**|200<br>23.09|100<br>35.48|50<br>52.74|38<br>60.77|20<br>82.72|10<br>110.69|5<br>142.79|200<br>76.33|100<br>88.73|20<br>142.55<br>38<br>117.10<br>50<br>107.72|10<br>175.19|5<br>213.11|200<br>283.71|100<br>244.42|50<br>240.18|38<br>244.93|20<br>263.82|10<br>294.92|5<br>335.11|**SMA**<br>**Tot. Prof.**|
|ft drift levels of the unde|**sults of different drif**|-38.17<br>-15.08|-51.81<br>-16.33|-69.80<br>-17.06|-78.10<br>-17.32|-100.20<br>-17.49|-128.61<br>-17.91|-160.71<br>-17.93|-45.08<br>31.25|-59.89<br>28.83|-116.25<br>26.30<br>-90.22<br>26.88<br>-80.52<br>27.20|-149.62<br>25.56|-187.70<br>25.41|-51.09<br>232.62|-72.46<br>171.96|-98.70<br>141.47|-110.67<br>134.26|-142.62<br>121.21|-182.95<br>111.97|-228.93<br>106.18|**Tot. Loss**<br>**Tot. Result**|
|f     rlying ben|**t levels.**|0.3811|0.4122|0.4350|0.4429|0.4533|0.4618|0.4666|0.3222|0.3126|0.3018<br>0.3053<br>0.3073|0.2986|0.2976|0.2512|0.2318|0.2210|0.2182|0.2130|0.2080|0.2062|**Max DD**|
|f      chmark. Th i   i|The table s|-0.0190|-0.0209|-0.0222|-0.0226|-0.0231|-0.0237|-0.0239|0.0200|0.0192|0.0183<br>0.0185<br>0.0185|0.0180|0.0179|0.1036|0.0891|0.0799|0.0775|0.0728|0.0693|0.0670|**Mean Rtrn**|
|e profit   i|hows the|0.0716|0.0775|0.0822|0.0840|0.0873|0.0886|0.0905|0.1156|0.1086|0.1000<br>0.1020<br>0.1037|0.0972|0.0968|0.1737|0.1443|0.1264|0.1221|0.1139|0.1070|0.1035|**Vola**|
|i and loss i|trade|0.57|0.44|0.29|0.26|0.18|0.11|0.09|0.55|0.38|0.16<br>0.22<br>0.25|0.11|0.09|0.49|0.33|0.24|0.20|0.18|0.11|0.08|**Skew**|
|i   figures|results|3.77|3.38|3.21|3.11|3.04|3.02|3.03|3.54|3.21|2.99<br>3.02<br>3.09|3.01|3.04|3.39|3.14|3.05|3.02|3.03|3.00|3.02|**Kurt**|
|i   i are in e|of the SM|-0.0527|-0.0579|-0.0629|-0.0647|-0.0669|-0.0693|-0.0698|-0.0346|-0.0334|-0.0320<br>-0.0315<br>-0.0328|-0.0322|-0.0315|0.0210|0.0180|0.0172|0.0157|0.0153|0.0145|0.0139|**VaR(5%)**|
|with resp|A tradin|-0.2657|-0.2702|-0.2696|-0.2693|-0.2644|-0.2674|-0.2639|0.1733|0.1767|0.1828<br>0.1813<br>0.1789|0.1852|0.1851|0.5965|0.6174|0.6323|0.6346|0.6388|0.6481|0.6468|**Sharpe**|
|ect to an initial investment|g rules with f-50% position|0.0972|0.0953|0.0941|0.0936|0.0931|0.0925|0.0924|-0.0317|-0.0326|-0.0335<br>-0.0333<br>-0.0332|-0.0338|-0.0338|-0.1210|-0.1355|-0.1447|-0.1471|-0.1518|-0.1553|-0.1576|**Exp.Exc.Retrn**|



31 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|-0.1284|-0.1254|-0.1210|-0.1153|-0.1124|-0.1018|-0.0860|-0.0283|-0.0283|-0.0280|-0.0279|-0.0279|-0.0273|-0.0265|0.0776|0.0780|0.0791|0.0801|0.0809|0.0831|0.0863|g rules with f-75% position|ect to an initial investment||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|0.6263|0.6289|0.6220|0.6215|0.6216|0.6166|0.6135|0.1630|0.1631|0.1614|0.1604|0.1584|0.1575|0.1558|-0.2867|-0.2905|-0.2872|-0.2922|-0.2925|-0.2927|-0.2874|A tradin|with resp||
|**VaR(5%)**|0.0176|0.0180|0.0193|0.0199|0.0216|0.0227|0.0272|-0.0502|-0.0510|-0.0502|-0.0493|-0.0513|-0.0518|-0.0528|-0.1071|-0.1060|-0.1021|-0.0981|-0.0955|-0.0876|-0.0795|of the SM|i   is are in e||
|**Kurt**|3.02|3.00|3.02|2.99|3.00|3.02|3.08|3.04|3.01|2.98|2.99|3.05|3.12|3.28|3.03|3.02|3.04|3.09|3.18|3.30|3.57|results|i  s figure||
|**Skew**|0.08|0.10|0.16|0.17|0.19|0.24|0.30|0.09|0.10|0.15|0.19|0.22|0.32|0.44|0.08|0.11|0.16|0.24|0.27|0.40|0.49|trade|i and los i||
|**Vola**|0.1536|0.1578|0.1665|0.1759|0.1805|0.1992|0.2259|0.1439|0.1439|0.1472|0.1489|0.1507|0.1555|0.1619|0.1347|0.1316|0.1292|0.1238|0.1209|0.1132|0.1040|ows the|e profit   i||
|**Mean Rtrn**|0.0962|0.0993|0.1036|0.1093|0.1122|0.1228|0.1386|0.0235|0.0235|0.0238|0.0239|0.0239|0.0245|0.0252|-0.0386|-0.0382|-0.0371|-0.0362|-0.0354|-0.0331|-0.0299|The table sh|f      chmark. Th i   i||
|**Max DD**|0.3124|0.3143|0.3205|0.3262|0.3291|0.3396|0.3552|0.4538|0.4541|0.4571|0.4600|0.4618|0.4653|0.4729|0.7137|0.7046|0.6891|0.6702|0.6568|0.6184|0.5676|**t levels.**|f     rlying ben||
|**Tot. Result**|194.86|206.20|224.81|250.04|263.70|320.61|425.56|40.42|40.45|41.65|42.33|42.80|45.12|48.23|-25.51|-25.51|-24.86|-24.62|-24.27|-23.21|-21.47|**ifferent drif**|f  els of the unde||
|**Tot. Loss**|-419.14|-336.43|-263.80|-206.16|-184.39|-136.14|-95.77|-297.25|-236.51|-183.44|-141.92|-126.45|-93.54|-69.82|-229.25|-183.24|-142.70|-111.12|-99.29|-73.63|-54.30|**sults of df**|ft drift lev||
|**Tot. Prof.**|614.00|542.64|488.61|456.21|448.09|456.76|521.34|337.66|276.96|225.08|184.25|169.24|138.66|118.06|203.73|157.74|117.84|86.50|75.02|50.41|32.83|**% trade re**|t on differen||
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**6: f-75**|ependen  f|0.|
|_µ_|0.259|0.259|0.259|0.259|0.259|0.259|0.259|0.086|0.086|0.086|0.086|0.086|0.086|0.086|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|**Table **|sizing d  f|of e 10|



32 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|sizing de  f<br>of e 100|**Table 7**|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|0.086|0.086|0.086<br>0.086<br>0.086|0.086|0.086|0.259|0.259|0.259|0.259|0.259|0.259|0.259|_µ_<br>|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|pendent on differen<br>.|**: f-100% trade r**|200<br>41.56|100<br>63.79|50<br>95.01|38<br>109.61|20<br>149.41|10<br>200.02|5<br>258.69|200<br>162.03|100<br>192.46|20<br>315.94<br>38<br>257.61<br>50<br>236.28|10<br>389.30|5<br>475.92|200<br>840.30|100<br>755.29|50<br>744.52|38<br>758.01|20<br>809.47|10<br>893.95|5<br>1008.18|**SMA**<br>**Tot. Prof.**|
|ft drift levels of the unde|**esults of different drif**|-68.78<br>-27.22|-93.16<br>-29.38|-125.74<br>-30.73|-140.76<br>-31.15|-180.87<br>-31.46|-232.35<br>-32.33|-291.01<br>-32.32|-95.98<br>66.06|-129.76<br>62.70|-257.31<br>58.63<br>-198.40<br>59.21<br>-176.45<br>59.82|-332.39<br>56.91|-418.70<br>57.22|-157.04<br>683.26|-226.29<br>529.00|-306.79<br>437.73|-342.57<br>415.44|-436.41<br>373.06|-553.85<br>340.10|-687.59<br>320.59|**Tot. Loss**<br>**Tot. Result**|
|f     rlying ben|**t levels.**|0.7531|0.8257|0.8820|0.9020|0.9313|0.9555|0.9705|0.6209|0.6174|0.6158<br>0.6167<br>0.6176|0.6141|0.6152|0.4555|0.4451|0.4366|0.4342|0.4290|0.4223|0.4209|**Max DD**|
|f      chmark. Th i   i|The table s|-0.0415|-0.0463|-0.0498|-0.0510|-0.0526|-0.0544|-0.0552|0.0282|0.0276|0.0271<br>0.0271<br>0.0270|0.0268|0.0268|0.1675|0.1517|0.1406|0.1375|0.1311|0.1263|0.1228|**Mean Rtrn**|
|e profit   i|hows the|0.1350|0.1475|0.1583|0.1624|0.1701|0.1738|0.1785|0.2043|0.1994|0.1929<br>0.1939<br>0.1955|0.1895|0.1903|0.2707|0.2485|0.2309|0.2265|0.2169|0.2072|0.2027|**Vola**|
|i and loss i|trade r|0.43|0.36|0.25|0.23|0.15|0.10|0.08|0.36|0.28|0.14<br>0.17<br>0.20|0.09|0.09|0.18|0.18|0.16|0.15|0.14|0.09|0.07|**Skew**|
|i   figures|esults|3.45|3.24|3.15|3.08|3.03|3.01|3.03|3.15|3.07|2.98<br>2.97<br>3.02|3.00|3.04|2.96|2.96|2.97|2.97|3.00|3.00|3.02|**Kurt**|
|i   i are in e|of the SM|-0.1066|-0.1176|-0.1288|-0.1326|-0.1382|-0.1439|-0.1459|-0.0720|-0.0710|-0.0699<br>-0.0684<br>-0.0710|-0.0714|-0.0708|0.0314|0.0255|0.0237|0.0223|0.0205|0.0199|0.0188|**VaR(5%)**|
|with respect to an initial investment|A trading rules with f-100% position|-0.3074<br>0.0747|-0.3139<br>0.0699|-0.3145<br>0.0664|-0.3143<br>0.0652|-0.3095<br>0.0636|-0.3131<br>0.0618|-0.3093<br>0.0610|0.1379<br>-0.0236|0.1385<br>-0.0241|0.1402<br>-0.0247<br>0.1398<br>-0.0247<br>0.1382<br>-0.0247|0.1413<br>-0.0250|0.1410<br>-0.0249|0.6187<br>-0.0571|0.6105<br>-0.0729|0.6088<br>-0.0840|0.6069<br>-0.0871|0.6047<br>-0.0935|0.6095<br>-0.0983|0.6058<br>-0.1018|**Sharpe**<br>**Exp.Exc.Retrn**|



33 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|-0.0778|-0.0740|-0.0689|-0.0622|-0.0591|-0.0480|-0.0327|-0.0236|-0.0237|-0.0234|-0.0234|-0.0236|-0.0229|-0.0225|0.0427|0.0440|0.0466|0.0491|0.0509|0.0558|0.0625|rules with f-125% position|ect to an initial investment||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|0.5853|0.5899|0.5869|0.5913|0.5946|0.6012|0.6172|0.1191|0.1196|0.1193|0.1194|0.1182|0.1197|0.1201|-0.3316|-0.3354|-0.3312|-0.3357|-0.3357|-0.3340|-0.3259|A trading|with resp||
|**VaR(5%)**|0.0180|0.0195|0.0197|0.0225|0.0240|0.0259|0.0335|-0.0929|-0.0933|-0.0912|-0.0888|-0.0918|-0.0912|-0.0919|-0.1861|-0.1833|-0.1753|-0.1682|-0.1627|-0.1483|-0.1341|of the SM|i   is are in e||
|**Kurt**|3.01|2.99|3.00|2.96|2.96|2.94|2.91|3.04|3.00|2.97|2.96|3.01|3.04|3.08|3.03|3.01|3.02|3.07|3.13|3.20|3.37|results|i  s figure||
|**Skew**|0.07|0.09|0.13|0.13|0.13|0.13|0.10|0.08|0.08|0.13|0.15|0.17|0.24|0.30|0.08|0.10|0.15|0.21|0.23|0.33|0.38|trade|i and los i||
|**Vola**|0.2509|0.2553|0.2654|0.2746|0.2784|0.2938|0.3109|0.2360|0.2343|0.2374|0.2371|0.2385|0.2410|0.2438|0.2217|0.2154|0.2102|0.2000|0.1947|0.1808|0.1649|ows the|e profit   i||
|**Mean Rtrn**|0.1469|0.1506|0.1558|0.1624|0.1655|0.1766|0.1919|0.0281|0.0280|0.0283|0.0283|0.0282|0.0289|0.0293|-0.0735|-0.0723|-0.0696|-0.0671|-0.0654|-0.0604|-0.0537|The table sh|f      chmark. Th i   i||
|**Max DD**|0.5319|0.5323|0.5387|0.5425|0.5440|0.5492|0.5533|0.7823|0.7789|0.7781|0.7757|0.7753|0.7697|0.7672|1.2370|1.2149|1.1800|1.1384|1.1111|1.0346|0.9383|**t levels.**|f     rlying ben||
|**Tot. Result**|498.43|529.37|583.53|648.60|681.51|813.92|1016.61|76.03|75.08|77.39|77.59|78.36|81.59|84.67|-38.43|-38.46|-37.38|-37.00|-36.53|-34.91|-32.40|**different drif**|f  els of the unde||
|**Tot. Loss**|-1065.28|-860.16|-680.28|-534.80|-478.85|-350.62|-237.96|-553.27|-438.01|-338.37|-259.91|-230.76|-168.61|-123.48|-346.72|-276.52|-215.18|-167.40|-149.48|-110.69|-81.82|**esults of f**|ft drift lev||
|**Tot. Prof.**|1563.71|1389.53|1263.82|1183.40|1160.36|1164.54|1254.57|629.30|513.09|415.75|337.50|309.11|250.20|208.15|308.29|238.06|177.80|130.39|112.95|75.78|49.42|**% trade r**|t on differen||
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**8: f-125**|ependen  f|0.|
|_µ_|0.259|0.259|0.259|0.259|0.259|0.259|0.259|0.086|0.086|0.086|0.086|0.086|0.086|0.086|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|**Table **|sizing d  f|of e 10|



34 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|-0.082|0.086|0.086|0.086|0.086|0.086|0.086|0.086|0.259|0.259|0.259|0.259|0.259|0.259|0.259|_µ_|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|200|100|50|38|20|10|5|200|100|50|38|20|10|5|200|100|50|38|20|10|5|**SMA**|
|36.81|54.96|80.61|92.71|125.45|167.48|215.16|207.99|247.47|302.55|329.26|401.42|493.86|600.61|1227.34|1281.59|1391.14|1451.60|1628.97|1869.79|2159.46|**Tot. Prof.**|
|-61.11|-80.54|-106.68|-119.09|-152.16|-194.68|-242.28|-123.39|-167.10|-226.14|-253.44|-326.78|-421.35|-528.46|-236.33|-386.87|-575.09|-657.33|-877.74|-1156.94|-1470.79|**Tot. Loss**|
|-24.30|-25.58|-26.07|-26.38|-26.71|-27.20|-27.12|84.59|80.36|76.41|75.82|74.64|72.51|72.15|991.01|894.72|816.05|794.26|751.23|712.85|688.67|**Tot. Result**|
|0.7402|0.7848|0.8123|0.8253|0.8547|0.8689|0.8842|0.8977|0.9188|0.9242|0.9177|0.9195|0.9229|0.9290|0.6303|0.6811|0.7257|0.7339|0.7647|0.7835|0.8221|**Max DD**|
|-0.0514|-0.0536|-0.0575|-0.0558|-0.0605|-0.0613|-0.0625|0.0054|-0.0004|-0.0004|0.0012|0.0005|-0.0023|-0.0046|0.1909|0.1795|0.1693|0.1674|0.1593|0.1523|0.1442|**Mean Rtrn**|
|0.4341|0.4323|0.4771|0.4378|0.4905|0.4881|0.5013|0.5960|0.6447|0.6340|0.6138|0.6196|0.6394|0.6620|0.4009|0.4437|0.4810|0.4740|0.5168|0.5547|0.6041|**Vola**|
|-7.18|-7.21|-6.85|-7.06|-6.42|-6.37|-6.28|-4.81|-4.61|-4.72|-4.73|-4.71|-4.56|-4.47|-4.26|-4.84|-4.79|-4.77|-4.76|-4.72|-4.44|**Skew**|
|60.53|62.44|54.90|59.72|48.36|47.65|46.42|30.10|26.70|27.95|28.23|27.85|25.67|24.45|41.82|43.84|37.69|38.02|34.70|32.13|27.06|**Kurt**|
|-0.1377|-0.1421|-0.1416|-0.1443|-0.1518|-0.1557|-0.1602|-0.1200|-0.1325|-0.1337|-0.1266|-0.1240|-0.1301|-0.1308|0.0378|0.0293|0.0222|0.0206|0.0135|0.0113|-0.0006|**VaR(5%)**|
|-0.1185|-0.1240|-0.1205|-0.1275|-0.1233|-0.1257|-0.1247|0.0090|-0.0006|-0.0006|0.0019|0.0007|-0.0037|-0.0069|0.4762|0.4046|0.3519|0.3532|0.3082|0.2745|0.2388|**Sharpe**|
|0.0648|0.0626|0.0588|0.0604|0.0558|0.0549|0.0537|-0.0464|-0.0521|-0.0522|-0.0506|-0.0513|-0.0541|-0.0563|-0.0337|-0.0451|-0.0554|-0.0572|-0.0653|-0.0723|-0.0804|**Exp.Exc.Retrn**|



35 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|-0.0353|-0.0352|-0.0349|-0.0347|-0.0346|-0.0341|-0.0332|-0.0416|-0.0415|-0.0413|-0.0411|-0.0410|-0.0405|-0.0397|-0.0419|-0.0418|-0.0416|-0.0414|-0.0413|-0.0409|-0.0402|MA trading rules with f-25%|are in e with respect to an||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|0.1896|0.1886|0.1847|0.1808|0.1778|0.1705|0.1627|0.2075|0.2075|0.2044|0.2023|0.1995|0.1955|0.1895|0.2594|0.2606|0.2586|0.2567|0.2552|0.2506|0.2457|of the S|s figures||
|**VaR(5%)**|-0.0237|-0.0240|-0.0246|-0.0249|-0.0256|-0.0267|-0.0269|-0.0148|-0.0152|-0.0152|-0.0150|-0.0157|-0.0162|-0.0169|-0.0075|-0.0076|-0.0078|-0.0077|-0.0081|-0.0083|-0.0087|e results|fit and los||
|**Kurt**|3.04|3.04|3.05|3.13|3.27|3.64|4.61|3.04|3.02|3.01|3.06|3.16|3.41|4.21|3.05|3.01|2.98|3.03|3.08|3.20|3.62|the trad|The proi||
|**Skew**|0.10|0.14|0.20|0.29|0.35|0.55|0.90|0.10|0.11|0.18|0.25|0.30|0.47|0.76|0.09|0.11|0.16|0.22|0.24|0.39|0.57|shows|mark.||
|**Vola**|0.0737|0.0747|0.0775|0.0805|0.0824|0.0888|0.0987|0.0489|0.0493|0.0510|0.0526|0.0538|0.0575|0.0634|0.0301|0.0304|0.0313|0.0324|0.0331|0.0353|0.0388|e table|bench||
|**Mean Rtrn**|0.0140|0.0141|0.0143|0.0146|0.0147|0.0151|0.0161|0.0101|0.0102|0.0104|0.0106|0.0107|0.0112|0.0120|0.0078|0.0079|0.0081|0.0083|0.0084|0.0088|0.0095|**levels.** Th|e underlying||
|**Max DD**|0.2249|0.2268|0.2312|0.2364|0.2395|0.2493|0.2653|0.1464|0.1473|0.1495|0.1522|0.1537|0.1582|0.1664|0.0856|0.0859|0.0870|0.0884|0.0890|0.0914|0.0957|**volatility **|levels of th||
|**Tot. Result**|18.17|18.40|18.96|19.56|19.91|21.27|23.92|12.00|12.12|12.46|12.80|12.96|13.80|15.16|8.61|8.73|8.97|9.23|9.40|9.92|10.83|**f different **|ft volatility||
|**Tot. Loss**|-137.08|-109.38|-85.14|-66.29|-59.23|-44.38|-33.74|-88.96|-71.02|-55.26|-43.01|-38.44|-28.74|-21.79|-53.64|-42.80|-33.27|-25.86|-23.07|-17.22|-12.98|**results o**|on differen|0.|
|**Tot. Prof.**|155.25|127.79|104.10|85.85|79.14|65.65|57.66|100.95|83.13|67.72|55.80|51.41|42.54|36.94|62.25|51.53|42.24|35.09|32.47|27.14|23.81|**-25% trade**|g dependent|ment of e 10|
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**10: f**|on sizin|invest|
|_σ_|0.39|0.39|0.39|0.39|0.39|0.39|0.39|0.26|0.26|0.26|0.26|0.26|0.26|0.26|0.16|0.16|0.16|0.16|0.16|0.16|0.16|**Table**|positi|initial|



36 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|0.16|0.16|0.16|0.16|0.16|0.16|0.16|0.26|0.26|0.26|0.26|0.26|0.26|0.26|0.39|0.39|0.39|0.39|0.39|0.39|0.39|_σ_|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|200|100|50|38|20|10|5|200|100|50|38|20|10|5|200|100|50|38|20|10|5|**SMA**|
|48.91|56.08|67.31|72.79|87.76|107.13|129.52|76.33|88.73|107.72|117.10|142.55|175.19|213.11|121.06|139.65|169.66|184.44|224.60|276.24|336.42|**Tot. Prof.**|
|-26.69|-35.58|-47.81|-53.64|-69.10|-88.97|-111.58|-45.08|-59.89|-80.52|-90.22|-116.25|-149.62|-187.70|-70.97|-94.28|-126.85|-142.33|-183.49|-236.35|-296.83|**Tot. Loss**|
|22.22|20.51|19.50|19.15|18.66|18.16|17.94|31.25|28.83|27.20|26.88|26.30|25.56|25.41|50.09|45.37|42.81|42.10|41.11|39.89|39.59|**Tot. Result**|
|0.1876|0.1814|0.1779|0.1770|0.1750|0.1732|0.1728|0.3222|0.3126|0.3073|0.3053|0.3018|0.2986|0.2976|0.5046|0.4889|0.4785|0.4746|0.4685|0.4627|0.4609|**Max DD**|
|0.0173|0.0163|0.0157|0.0155|0.0152|0.0149|0.0147|0.0200|0.0192|0.0185|0.0185|0.0183|0.0180|0.0179|0.0237|0.0231|0.0229|0.0229|0.0229|0.0227|0.0227|**Mean Rtrn**|
|0.0731|0.0680|0.0647|0.0635|0.0619|0.0602|0.0599|0.1156|0.1086|0.1037|0.1020|0.1000|0.0972|0.0968|0.1728|0.1629|0.1558|0.1535|0.1502|0.1462|0.1452|**Vola**|
|0.46|0.34|0.21|0.20|0.15|0.10|0.09|0.55|0.38|0.25|0.22|0.16|0.11|0.09|0.62|0.41|0.28|0.24|0.17|0.12|0.09|**Skew**|
|3.33|3.11|3.05|3.00|2.97|3.00|3.05|3.54|3.21|3.09|3.02|2.99|3.01|3.04|3.64|3.28|3.13|3.06|3.02|3.03|3.04|**Kurt**|
|-0.0178|-0.0172|-0.0168|-0.0159|-0.0163|-0.0160|-0.0157|-0.0346|-0.0334|-0.0328|-0.0315|-0.0320|-0.0322|-0.0315|-0.0554|-0.0556|-0.0541|-0.0532|-0.0527|-0.0520|-0.0518|**VaR(5%)**|
|0.2369|0.2395|0.2428|0.2439|0.2453|0.2469|0.2456|0.1733|0.1767|0.1789|0.1813|0.1828|0.1852|0.1851|0.1375|0.1420|0.1470|0.1493|0.1523|0.1552|0.1561|**Sharpe**|
|-0.0324|-0.0334|-0.0340|-0.0342|-0.0345|-0.0348|-0.0350|-0.0317|-0.0326|-0.0332|-0.0333|-0.0335|-0.0338|-0.0338|-0.0255|-0.0261|-0.0264|-0.0264|-0.0264|-0.0266|-0.0266|**Exp.Exc.Retrn**|



37 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|-0.0229|-0.0230|-0.0229|-0.0231|-0.0232|-0.0233|-0.0231|-0.0283|-0.0283|-0.0280|-0.0279|-0.0279|-0.0273|-0.0265|-0.0290|-0.0288|-0.0284|-0.0281|-0.0278|-0.0272|-0.0260|MA trading rules with f-75%|are in e with respect to an||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|0.1229|0.1223|0.1204|0.1182|0.1166|0.1134|0.1108|0.1630|0.1631|0.1614|0.1604|0.1584|0.1575|0.1558|0.2320|0.2334|0.2322|0.2312|0.2304|0.2281|0.2270|of the S|s figures||
|**VaR(5%)**|-0.0837|-0.0837|-0.0840|-0.0843|-0.0854|-0.0866|-0.0854|-0.0502|-0.0510|-0.0502|-0.0493|-0.0513|-0.0518|-0.0528|-0.0247|-0.0250|-0.0255|-0.0247|-0.0260|-0.0264|-0.0274|e results|fit and los||
|**Kurt**|3.04|3.02|3.00|3.01|3.06|3.15|3.32|3.04|3.01|2.98|2.99|3.05|3.12|3.28|3.05|3.00|2.96|2.99|3.03|3.06|3.18|the trad|The proi||
|**Skew**|0.09|0.11|0.15|0.20|0.23|0.33|0.48|0.09|0.10|0.15|0.19|0.22|0.32|0.44|0.09|0.10|0.14|0.18|0.19|0.30|0.38|shows|mark.||
|**Vola**|0.2150|0.2151|0.2191|0.2214|0.2234|0.2292|0.2365|0.1439|0.1439|0.1472|0.1489|0.1507|0.1555|0.1619|0.0893|0.0896|0.0917|0.0936|0.0950|0.0989|0.1044|e table|bench||
|**Mean Rtrn**|0.0264|0.0263|0.0264|0.0262|0.0261|0.0260|0.0262|0.0235|0.0235|0.0238|0.0239|0.0239|0.0245|0.0252|0.0207|0.0209|0.0213|0.0216|0.0219|0.0226|0.0237|**levels.** Th|e underlying||
|**Max DD**|0.7089|0.7090|0.7135|0.7171|0.7199|0.7258|0.7353|0.4538|0.4541|0.4571|0.4600|0.4618|0.4653|0.4729|0.2617|0.2618|0.2639|0.2661|0.2669|0.2705|0.2771|**volatility **|levels of th||
|**Tot. Result**|64.87|64.92|66.86|67.89|68.96|72.39|78.35|40.42|40.45|41.65|42.33|42.80|45.12|48.23|28.06|28.33|29.11|29.80|30.33|31.77|34.16|**of different **|fnt volatility||
|**Tot. Loss**|-482.74|-383.28|-296.65|-229.04|-203.61|-149.92|-111.49|-297.25|-236.51|-183.44|-141.92|-126.45|-93.54|-69.82|-174.14|-138.75|-107.66|-83.44|-74.30|-55.10|-41.12|**results **|on differe|0.|
|**Tot. Prof.**|547.61|448.19|363.51|296.93|272.57|222.31|189.84|337.66|276.96|225.08|184.25|169.24|138.66|118.06|202.20|167.09|136.77|113.24|104.62|86.88|75.28|**75% trade**|dependent|ent of e 10|
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**12: f-**|on sizing|investm|
|_σ_|0.39|0.39|0.39|0.39|0.39|0.39|0.39|0.26|0.26|0.26|0.26|0.26|0.26|0.26|0.16|0.16|0.16|0.16|0.16|0.16|0.16|**Table**|positi|initial|



38 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|0.16|0.16|0.16|0.16|0.16|0.16|0.16|0.26|0.26|0.26|0.26|0.26|0.26|0.26|0.39|0.39|0.39|0.39|0.39|0.39|0.39|_σ_|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|200|100|50|38|20|10|5|200|100|50|38|20|10|5|200|100|50|38|20|10|5|**SMA**|
|102.92|119.58|144.55|156.58|189.48|231.69|280.71|162.03|192.46|236.28|257.61|315.94|389.30|475.92|263.60|313.85|388.83|424.42|522.88|646.72|793.47|**Tot. Prof.**|
|-56.28|-75.84|-102.62|-115.36|-149.10|-192.38|-241.68|-95.98|-129.76|-176.45|-198.40|-257.31|-332.39|-418.70|-155.07|-211.51|-290.21|-327.30|-426.22|-552.76|-698.72|**Tot. Loss**|
|46.64|43.75|41.92|41.22|40.38|39.31|39.04|66.06|62.70|59.82|59.21|58.63|56.91|57.22|108.53|102.34|98.61|97.12|96.66|93.96|94.76|**Tot. Result**|
|0.3649|0.3591|0.3563|0.3558|0.3539|0.3519|0.3523|0.6209|0.6174|0.6176|0.6167|0.6158|0.6141|0.6152|0.9625|0.9628|0.9654|0.9651|0.9671|0.9664|0.9698|**Max DD**|
|0.0289|0.0278|0.0271|0.0268|0.0265|0.0261|0.0259|0.0282|0.0276|0.0270|0.0271|0.0271|0.0268|0.0268|0.0248|0.0248|0.0249|0.0251|0.0254|0.0253|0.0255|**Mean Rtrn**|
|0.1337|0.1282|0.1243|0.1228|0.1209|0.1185|0.1184|0.2043|0.1994|0.1955|0.1939|0.1929|0.1895|0.1903|0.2942|0.2904|0.2869|0.2856|0.2852|0.2819|0.2832|**Vola**|
|0.32|0.27|0.18|0.16|0.14|0.09|0.09|0.36|0.28|0.20|0.17|0.14|0.09|0.09|0.39|0.27|0.20|0.17|0.13|0.10|0.08|**Skew**|
|3.09|3.02|3.01|2.97|2.96|3.00|3.05|3.15|3.07|3.02|2.97|2.98|3.00|3.04|3.17|3.08|3.03|2.99|2.99|3.01|3.04|**Kurt**|
|-0.0371|-0.0360|-0.0357|-0.0341|-0.0352|-0.0348|-0.0343|-0.0720|-0.0710|-0.0710|-0.0684|-0.0699|-0.0714|-0.0708|-0.1168|-0.1195|-0.1190|-0.1181|-0.1193|-0.1190|-0.1196|**VaR(5%)**|
|0.2166|0.2167|0.2181|0.2185|0.2192|0.2199|0.2185|0.1379|0.1385|0.1382|0.1398|0.1402|0.1413|0.1410|0.0842|0.0853|0.0868|0.0877|0.0890|0.0898|0.0900|**Sharpe**|
|-0.0208|-0.0219|-0.0226|-0.0229|-0.0232|-0.0237|-0.0238|-0.0236|-0.0241|-0.0247|-0.0247|-0.0247|-0.0250|-0.0249|-0.0245|-0.0245|-0.0244|-0.0242|-0.0239|-0.0239|-0.0238|**Exp.Exc.Retrn**|



39 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|-0.0291|-0.0292|-0.0290|-0.0292|-0.0292|-0.0291|-0.0290|-0.0236|-0.0237|-0.0234|-0.0234|-0.0236|-0.0229|-0.0225|-0.0195|-0.0193|-0.0189|-0.0186|-0.0183|-0.0176|-0.0165|MA trading rules with f-25%|are in e with respect to an||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|0.0575|0.0578|0.0581|0.0579|0.0577|0.0580|0.0584|0.1191|0.1196|0.1193|0.1194|0.1182|0.1197|0.1201|0.2050|0.2066|0.2062|0.2060|0.2058|0.2052|0.2059|of the S|s figures||
|**VaR(5%)**|-0.1591|-0.1579|-0.1570|-0.1544|-0.1552|-0.1542|-0.1494|-0.0929|-0.0933|-0.0912|-0.0888|-0.0918|-0.0912|-0.0919|-0.0448|-0.0451|-0.0456|-0.0440|-0.0459|-0.0462|-0.0473|de results|fit and los||
|**Kurt**|3.03|3.01|2.98|2.97|3.00|3.04|3.09|3.04|3.00|2.97|2.96|3.01|3.04|3.08|3.05|2.99|2.96|2.96|3.00|3.00|3.04|the tra|The proi||
|**Skew**|0.08|0.09|0.12|0.15|0.17|0.23|0.32|0.08|0.08|0.13|0.15|0.17|0.24|0.30|0.08|0.09|0.13|0.15|0.16|0.24|0.28|shows|mark.||
|**Vola**|0.3502|0.3470|0.3490|0.3469|0.3473|0.3479|0.3477|0.2360|0.2343|0.2374|0.2371|0.2385|0.2410|0.2438|0.1473|0.1470|0.1495|0.1511|0.1527|0.1563|0.1612|e table|bench||
|**Mean Rtrn**|0.0201|0.0200|0.0203|0.0201|0.0200|0.0202|0.0203|0.0281|0.0280|0.0283|0.0283|0.0282|0.0289|0.0293|0.0302|0.0304|0.0308|0.0311|0.0314|0.0321|0.0332|**levels.** Th|e underlying||
|**Max DD**|1.2446|1.2358|1.2302|1.2197|1.2163|1.2019|1.1887|0.7823|0.7789|0.7781|0.7757|0.7753|0.7697|0.7672|0.4447|0.4435|0.4450|0.4461|0.4460|0.4473|0.4515|**f volatility**|evels of th||
|**Tot. Result**|130.09|127.50|130.91|129.97|132.02|135.20|140.41|76.03|75.08|77.39|77.59|78.36|81.59|84.67|50.95|51.15|52.51|53.43|54.33|56.44|59.66|**of different**|fnt volatility l||
|**Tot. Loss**|-949.19|-747.53|-573.82|-437.89|-387.37|-279.20|-201.49|-553.27|-438.01|-338.37|-259.91|-230.76|-168.61|-123.48|-314.56|-250.12|-193.62|-149.52|-132.88|-97.82|-72.15|**e results **|on differe|0.|
|**Tot. Prof.**|1079.28|875.02|704.74|567.87|519.39|414.40|341.90|629.30|513.09|415.75|337.50|309.11|250.20|208.15|365.51|301.27|246.13|202.95|187.21|154.26|131.81|**125% trad**|dependent|ent of e 10|
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**14: f-**|on sizing|investm|
|_σ_|0.39|0.39|0.39|0.39|0.39|0.39|0.39|0.26|0.26|0.26|0.26|0.26|0.26|0.26|0.16|0.16|0.16|0.16|0.16|0.16|0.16|**Table**|positi|initial|



40 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|unman<br>respect|**Table **|0.16|0.16|0.16|0.16|0.16|0.16|0.16|0.26<br>0.26<br>0.26|0.26<br>0.26|0.26|0.26|0.39|0.39|0.39|0.39|0.39|0.39|0.39|_σ_<br>|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|aged position sizin<br> to an initial invest|**15:**<br>**Unsized tra**|200<br>121.08|100<br>141.60|50<br>171.08|38<br>185.30|20<br>223.89|10<br>273.68|5<br>331.04|50<br>302.55<br>100<br>247.47<br>200<br>207.99|20<br>401.42<br>38<br>329.26|10<br>493.86|5<br>600.61|200<br>387.92|100<br>463.28|50<br>567.49|38<br>618.04|20<br>754.43|10<br>928.60|5<br>1130.35|**SMA**<br>**Tot. Prof.**|
|g depende<br>ment of e|**de resul**|-66.28|-89.91|-121.52|-136.52|-176.12|-227.14|-285.03|-226.14<br>-167.10<br>-123.39|-326.78<br>-253.44|-421.35|-528.46|-228.89|-312.00|-423.97|-476.03|-614.55|-793.09|-995.49|**Tot. Loss**|
|nt on f<br> 100.|**ts of f**|||||||||||||||||||**Tot. R**|
|differen|**differe**|54.80|51.69|49.57|48.78|47.77|46.54|46.00|76.41<br>80.36<br>84.59|74.64<br>75.82|72.51|72.15|159.03|151.29|143.52|142.02|139.88|135.51|134.86|**esult**|
|ft volatility|**fnt volati**|0.4287|0.4278|0.4265|0.4249|0.4244|0.4235|0.4263|0.9242<br>0.9188<br>0.8977|0.9195<br>0.9177|0.9229|0.9290|1.7247|1.7480|1.7524|1.7596|1.7382|1.7213|1.7440|**Max DD**|
|levels of t|**lity levels.**|0.0314|0.0299|0.0293|0.0291|0.0285|0.0280|0.0276|-0.0004<br>-0.0004<br>0.0054|0.0005<br>0.0012|-0.0023|-0.0046|-0.1089|-0.1189|-0.1233|-0.1230|-0.1167|-0.1181|-0.1261|**Mean Rtrn**|
|he und|The|0.1593|0.1681|0.1581|0.1493|0.1586|0.1581|0.1606|0.6340<br>0.6447<br>0.5960|0.6196<br>0.6138|0.6394|0.6620|1.2214|1.2556|1.2628|1.2631|1.2396|1.2388|1.2656|**Vola**|
|erlying|table s|-1.29|-5.43|-3.82|-1.52|-4.74|-4.84|-6.52|-4.72<br>-4.61<br>-4.81|-4.71<br>-4.73|-4.56|-4.47|-1.84|-1.76|-1.73|-1.73|-1.79|-1.77|-1.70|**Skew**|
|benchm|hows the|34.88|150.12|103.84|39.06|118.88|112.00|184.51|27.95<br>26.70<br>30.10|27.85<br>28.23|25.67|24.45|5.14|4.75|4.58|4.61|4.82|4.76|4.45|**Kurt**|
|ark. The i|trade res|-0.0450|-0.0445|-0.0437|-0.0424|-0.0432|-0.0433|-0.0436|-0.1337<br>-0.1325<br>-0.1200|-0.1240<br>-0.1266|-0.1301|-0.1308|-1.0526|-1.0594|-1.0594|-1.0637|-1.0600|-1.0530|-1.0641|**VaR(5%)**|
|profit an|ults of t|0.1973|0.1780|0.1853|0.1952|0.1800|0.1771|0.1720|-0.0006<br>-0.0006<br>0.0090|0.0007<br>0.0019|-0.0037|-0.0069|-0.0892|-0.0947|-0.0976|-0.0974|-0.0941|-0.0954|-0.0996|**Sharpe**|
|d loss figures are in e with|he SMA trading rules with|-0.0183|-0.0198|-0.0204|-0.0206|-0.0212|-0.0217|-0.0221|-0.0522<br>-0.0521<br>-0.0464|-0.0513<br>-0.0506|-0.0541|-0.0563|-0.1582|-0.1681|-0.1726|-0.1723|-0.1659|-0.1674|-0.1753|**Exp.Exc.Retrn**|



41 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|0.0175|0.0035|-0.0077|-0.0158|-0.0185|-0.0239|-0.0272|-0.0345|-0.0362|-0.0373|-0.0381|-0.0384|-0.0386|-0.0383|-0.0696|-0.0629|-0.0573|-0.0530|-0.0515|-0.0482|-0.0454|he SMA trading rules with|i d loss figures are in ewith||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|1.2279|0.9653|0.7409|0.5831|0.5269|0.4091|0.3221|0.3471|0.3117|0.2790|0.2555|0.2453|0.2254|0.2086|-0.3864|-0.2397|-0.1148|-0.0265|0.0036|0.0640|0.1055|sults of t|profit an  i||
|**VaR(5%)**|0.0403|0.0258|0.0142|0.0058|0.0024|-0.0038|-0.0086|-0.0082|-0.0102|-0.0116|-0.0125|-0.0133|-0.0146|-0.0157|-0.0417|-0.0354|-0.0300|-0.0261|-0.0250|-0.0229|-0.0214|he trade re|f       mark. The i   i||
|**Kurt**|3.03|3.04|3.06|3.13|3.27|3.63|4.64|3.04|3.03|3.02|3.07|3.18|3.44|4.26|3.03|3.02|3.01|3.04|3.12|3.31|3.97|hows t|f      g bench||
|**Skew**|0.10|0.14|0.22|0.31|0.38|0.58|0.93|0.10|0.12|0.18|0.25|0.30|0.48|0.78|0.08|0.10|0.16|0.22|0.25|0.40|0.66|table s|f      derlyin||
|**Vola**|0.0561|0.0570|0.0590|0.0612|0.0625|0.0673|0.0753|0.0496|0.0501|0.0518|0.0535|0.0547|0.0585|0.0646|0.0464|0.0468|0.0483|0.0498|0.0510|0.0543|0.0594|**s.** The|f    f the un||
|**Mean Rtrn**|0.0689|0.0550|0.0437|0.0357|0.0329|0.0275|0.0242|0.0172|0.0156|0.0144|0.0137|0.0134|0.0132|0.0135|-0.0179|-0.0112|-0.0055|-0.0013|0.0002|0.0035|0.0063|**lation level**|f  ation levels o||
|**Max DD**|0.0688|0.0798|0.0934|0.1073|0.1135|0.1319|0.1543|0.1284|0.1334|0.1390|0.1444|0.1469|0.1539|0.1640|0.2765|0.2367|0.2094|0.1926|0.1882|0.1803|0.1791|**utocorre**|f  autocorrel||
|**Tot. Result**|102.39|76.13|57.57|45.64|41.76|34.86|31.38|20.26|18.36|17.11|16.30|16.09|16.11|16.95|-15.52|-9.63|-4.29|-0.07|1.50|5.09|8.44|**different a**|fferent lag-1|100.|
|**Tot. Loss**|-88.50|-66.52|-49.45|-37.38|-33.09|-24.35|-18.49|-88.85|-70.41|-54.50|-42.26|-37.72|-28.15|-21.34|-89.77|-73.65|-58.57|-46.22|-41.49|-31.31|-23.73|**results of**|ndent on dif|ment of e|
|**Tot. Prof.**|190.89|142.65|107.02|83.03|74.85|59.21|49.88|109.11|88.77|71.60|58.56|53.81|44.27|38.29|74.25|64.02|54.29|46.14|43.00|36.41|32.18|**25% trade **|sizing depe  f|nitial invest|
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**16: f-**|position    f|to an i|
|_ϕ_|0.2|0.2|0.2|0.2|0.2|0.2|0.2|0.025|0.025|0.025|0.025|0.025|0.025|0.025|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|**Table **|f-25%     f|respect|



42 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|0.025|0.025|0.025|0.025|0.025|0.025|0.025|0.2|0.2|0.2|0.2|0.2|0.2|0.2|_ϕ_|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|200|100|50|38|20|10|5|200|100|50|38|20|10|5|200|100|50|38|20|10|5|**SMA**|
|64.75|73.08|85.49|91.22|105.50|121.32|136.64|79.65|93.22|114.31|124.83|153.91|192.46|239.21|109.23|134.26|176.58|199.58|271.20|388.72|571.23|**Tot. Prof.**|
|-47.82|-62.82|-82.47|-91.36|-113.80|-139.58|-165.15|-44.44|-59.24|-80.08|-90.05|-117.06|-152.61|-194.68|-40.50|-55.15|-77.98|-89.77|-125.09|-181.04|-264.44|**Tot. Loss**|
|16.93|10.26|3.02|-0.14|-8.29|-18.26|-28.51|35.21|33.98|34.23|34.78|36.85|39.86|44.53|68.74|79.10|98.60|109.81|146.10|207.68|306.79|**Tot. Result**|
|0.3505|0.3587|0.3783|0.3883|0.4248|0.4821|0.5644|0.3165|0.3035|0.2933|0.2893|0.2802|0.2700|0.2603|0.2893|0.2556|0.2240|0.2128|0.1864|0.1600|0.1381|**Max DD**|
|0.0063|0.0024|-0.0032|-0.0058|-0.0141|-0.0250|-0.0384|0.0193|0.0206|0.0223|0.0231|0.0251|0.0281|0.0314|0.0380|0.0470|0.0593|0.0654|0.0823|0.1056|0.1334|**Mean Rtrn**|
|0.1100|0.1036|0.0990|0.0972|0.0951|0.0926|0.0922|0.1173|0.1102|0.1052|0.1035|0.1013|0.0987|0.0982|0.1329|0.1242|0.1183|0.1169|0.1142|0.1113|0.1104|**Vola**|
|0.49|0.33|0.21|0.19|0.14|0.09|0.07|0.57|0.39|0.26|0.22|0.17|0.11|0.09|0.67|0.46|0.31|0.26|0.19|0.12|0.09|**Skew**|
|3.45|3.17|3.06|3.01|3.00|3.01|3.03|3.55|3.23|3.10|3.02|3.00|3.02|3.04|3.68|3.30|3.14|3.06|3.03|3.03|3.02|**Kurt**|
|-0.0435|-0.0468|-0.0512|-0.0535|-0.0615|-0.0724|-0.0850|-0.0322|-0.0302|-0.0281|-0.0265|-0.0247|-0.0221|-0.0185|-0.0180|-0.0092|0.0029|0.0093|0.0261|0.0489|0.0779|**VaR(5%)**|
|0.0838|0.0407|-0.0205|-0.0509|-0.1392|-0.2642|-0.4104|0.1940|0.2078|0.2259|0.2355|0.2582|0.2902|0.3254|0.3205|0.4034|0.5185|0.5737|0.7299|0.9532|1.2147|**Sharpe**|
|-0.0425|-0.0475|-0.0537|-0.0567|-0.0649|-0.0762|-0.0896|-0.0290|-0.0289|-0.0280|-0.0274|-0.0256|-0.0231|-0.0198|-0.0089|-0.0014|0.0099|0.0156|0.0319|0.0547|0.0827|**Exp.Exc.Retrn**|



43 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|0.1445|0.1023|0.0681|0.0433|0.0347|0.0174|0.0056|-0.0074|-0.0125|-0.0164|-0.0192|-0.0203|-0.0219|-0.0227|-0.1114|-0.0914|-0.0746|-0.0623|-0.0580|-0.0490|-0.0420|he SMA trading rules with|i d loss figures are in ewith||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|1.2009|0.9402|0.7176|0.5625|0.5077|0.3943|0.3131|0.3039|0.2689|0.2376|0.2156|0.2064|0.1897|0.1775|-0.4341|-0.2882|-0.1629|-0.0745|-0.0437|0.0182|0.0625|sults of t|profit an  i||
|**VaR(5%)**|0.1128|0.0695|0.0357|0.0109|0.0016|-0.0159|-0.0284|-0.0307|-0.0358|-0.0397|-0.0419|-0.0444|-0.0469|-0.0493|-0.1299|-0.1109|-0.0942|-0.0821|-0.0787|-0.0716|-0.0665|he trade re|f       mark. The i   i||
|**Kurt**|3.02|3.02|3.01|3.01|3.08|3.17|3.35|3.04|3.01|2.99|2.99|3.06|3.13|3.28|3.03|3.01|2.99|2.99|3.03|3.10|3.24|hows t|f      g bench||
|**Skew**|0.09|0.11|0.17|0.22|0.27|0.38|0.53|0.09|0.10|0.15|0.19|0.22|0.33|0.45|0.07|0.08|0.13|0.17|0.18|0.28|0.39|table s|f      derlyin||
|**Vola**|0.1632|0.1635|0.1665|0.1685|0.1696|0.1747|0.1821|0.1460|0.1459|0.1490|0.1509|0.1526|0.1575|0.1639|0.1374|0.1376|0.1406|0.1427|0.1447|0.1495|0.1557|**s.** The|f    f the un||
|**Mean Rtrn**|0.1959|0.1537|0.1195|0.0948|0.0861|0.0689|0.0570|0.0444|0.0392|0.0354|0.0325|0.0315|0.0299|0.0291|-0.0596|-0.0397|-0.0229|-0.0106|-0.0063|0.0027|0.0097|**lation level**|f  ation levels o||
|**Max DD**|0.2079|0.2407|0.2796|0.3173|0.3330|0.3753|0.4162|0.3960|0.4098|0.4238|0.4354|0.4402|0.4510|0.4636|0.8636|0.7364|0.6463|0.5877|0.5710|0.5367|0.5183|**utocorre**|f  autocorrel||
|**Tot. Result**|712.01|433.14|281.09|199.05|175.14|134.58|112.57|73.58|64.96|59.58|55.64|54.60|53.69|54.72|-39.41|-25.98|-12.05|-0.21|4.53|15.48|25.42|**different a**|fferent lag-1|100.|
|**Tot. Loss**|-611.89|-376.67|-239.78|-162.45|-138.26|-93.65|-66.33|-320.62|-248.37|-188.71|-143.90|-127.51|-93.41|-69.30|-228.54|-198.63|-165.89|-135.46|-122.95|-94.51|-72.19|**results of**|dent on dif|ment of e|
|**Tot. Prof.**|1323.89|809.80|520.87|361.50|313.40|228.23|178.90|394.20|313.33|248.29|199.54|182.11|147.11|124.03|189.12|172.65|153.84|135.26|127.47|109.99|97.62|**75% trade **|sizing depen  f|nitial invest|
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**18: f-**|osition    f|to an i|
|_ϕ_|0.2|0.2|0.2|0.2|0.2|0.2|0.2|0.025|0.025|0.025|0.025|0.025|0.025|0.025|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|**Table **|f-75% p    f|respect|



44 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|0.025|0.025|0.025|0.025|0.025|0.025|0.025|0.2|0.2|0.2|0.2|0.2|0.2|0.2|_ϕ_|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|200|100|50|38|20|10|5|200|100|50|38|20|10|5|200|100|50|38|20|10|5|**SMA**|
|130.70|147.11|168.96|178.26|199.46|218.64|233.34|171.33|206.16|257.81|283.48|356.24|453.92|578.65|259.67|344.60|495.63|584.23|897.35|1524.58|2803.14|**Tot. Prof.**|
|-96.79|-126.37|-162.93|-178.54|-215.05|-251.56|-281.91|-95.88|-130.81|-180.43|-204.40|-270.55|-359.69|-470.29|-96.29|-141.25|-218.45|-262.31|-412.21|-708.16|-1293.24|**Tot. Loss**|
|33.91|20.74|6.03|-0.29|-15.59|-32.92|-48.57|75.45|75.34|77.38|79.09|85.69|94.23|108.36|163.38|203.35|277.17|321.92|485.14|816.42|1509.91|**Tot. Result**|
|0.6843|0.7153|0.7668|0.7912|0.8743|0.9997|1.1746|0.6076|0.5977|0.5881|0.5830|0.5702|0.5532|0.5358|0.5386|0.4925|0.4411|0.4215|0.3730|0.3219|0.2785|**Max DD**|
|0.0083|-0.0007|-0.0125|-0.0182|-0.0344|-0.0567|-0.0833|0.0331|0.0346|0.0370|0.0384|0.0424|0.0476|0.0545|0.0685|0.0847|0.1078|0.1194|0.1524|0.1982|0.2545|**Mean Rtrn**|
|0.1980|0.1929|0.1887|0.1866|0.1849|0.1818|0.1821|0.2064|0.2016|0.1977|0.1961|0.1950|0.1921|0.1929|0.2260|0.2209|0.2175|0.2170|0.2164|0.2139|0.2145|**Vola**|
|0.32|0.24|0.16|0.15|0.12|0.08|0.07|0.37|0.29|0.20|0.17|0.14|0.10|0.09|0.44|0.33|0.23|0.20|0.16|0.10|0.08|**Skew**|
|3.14|3.06|3.01|2.97|2.98|3.01|3.03|3.15|3.08|3.03|2.97|2.98|3.01|3.04|3.18|3.09|3.04|2.99|3.00|3.02|3.02|**Kurt**|
|-0.0898|-0.0977|-0.1072|-0.1120|-0.1282|-0.1509|-0.1765|-0.0670|-0.0646|-0.0618|-0.0586|-0.0564|-0.0514|-0.0450|-0.0394|-0.0235|-0.0013|0.0107|0.0432|0.0879|0.1451|**VaR(5%)**|
|0.0418|-0.0034|-0.0663|-0.0974|-0.1861|-0.3118|-0.4574|0.1605|0.1715|0.1869|0.1957|0.2172|0.2478|0.2825|0.3031|0.3832|0.4956|0.5500|0.7045|0.9265|1.1866|**Sharpe**|
|-0.0434|-0.0524|-0.0642|-0.0699|-0.0861|-0.1084|-0.1350|-0.0186|-0.0172|-0.0148|-0.0134|-0.0094|-0.0042|0.0027|0.0171|0.0332|0.0564|0.0679|0.1010|0.1467|0.2030|**Exp.Exc.Retrn**|



45 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|**Exp.Exc.Retrn**|0.2585|0.1881|0.1310|0.0897|0.0753|0.0465|0.0262|0.0107|0.0020|-0.0046|-0.0096|-0.0114|-0.0144|-0.0165|-0.1604|-0.1272|-0.0994|-0.0792|-0.0721|-0.0574|-0.0465|he SMA trading rules with|it and loss figures are in e|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|**Sharpe**|1.1719|0.9122|0.6906|0.5367|0.4825|0.3711|0.2917|0.2612|0.2267|0.1968|0.1760|0.1676|0.1534|0.1434|-0.4804|-0.3350|-0.2089|-0.1198|-0.0882|-0.0243|0.0217|esults of t|The profi|
|**VaR(5%)**|0.1746|0.1039|0.0489|0.0091|-0.0057|-0.0325|-0.0515|-0.0608|-0.0685|-0.0744|-0.0769|-0.0803|-0.0834|-0.0855|-0.2245|-0.1922|-0.1637|-0.1431|-0.1369|-0.1245|-0.1141|he trade r|nchmark.|
|**Kurt**|3.02|3.01|2.99|2.96|3.01|3.05|3.09|3.04|3.00|2.98|2.96|3.01|3.05|3.07|3.02|3.01|2.98|2.96|2.99|3.04|3.08|shows t|ying be|
|**Skew**|0.08|0.09|0.15|0.18|0.21|0.29|0.38|0.08|0.09|0.13|0.15|0.18|0.25|0.31|0.07|0.07|0.11|0.14|0.14|0.21|0.26|table|underl|
|**Vola**|0.2645|0.2626|0.2642|0.2631|0.2628|0.2639|0.2662|0.2390|0.2373|0.2398|0.2397|0.2409|0.2433|0.2460|0.2263|0.2253|0.2283|0.2291|0.2312|0.2344|0.2378|**ls.** The|s of the|
|**Mean Rtrn**|0.3099|0.2396|0.1825|0.1412|0.1268|0.0979|0.0777|0.0624|0.0538|0.0472|0.0422|0.0404|0.0373|0.0353|-0.1087|-0.0755|-0.0477|-0.0274|-0.0204|-0.0057|0.0052|**elation leve**|relation level|
|**Max DD**|0.3497|0.4039|0.4669|0.5257|0.5489|0.6081|0.6579|0.6798|0.7004|0.7196|0.7327|0.7375|0.7443|0.7499|1.4973|1.2724|1.1091|0.9992|0.9660|0.8952|0.8496|**autocorr**|1 autocor|
|**Tot. Result**|3070.22|1462.69|790.95|489.39|411.90|287.71|221.67|149.98|128.26|115.58|105.32|102.78|99.00|97.34|-56.27|-39.15|-18.93|-0.39|7.52|26.03|42.35|**f different **|different lag-|
|**Tot. Loss**|-2619.96|-1265.42|-669.22|-398.26|-324.17|-199.52|-130.68|-647.99|-488.81|-363.79|-272.12|-239.28|-171.58|-124.14|-326.90|-299.01|-261.45|-220.62|-202.41|-158.35|-121.56|**results o**|endent on|
|**Tot. Prof.**|5690.18|2728.11|1460.17|887.65|736.07|487.24|352.35|797.97|617.08|479.38|377.44|342.06|270.58|221.48|270.63|259.86|242.52|220.22|209.93|184.38|163.91|**25% trade **|n sizing dep|
|**SMA**|5|10|20|38|50|100|200|5|10|20|38|50|100|200|5|10|20|38|50|100|200|**20: f-1**|positio|
|_ϕ_|0.2|0.2|0.2|0.2|0.2|0.2|0.2|0.025|0.025|0.025|0.025|0.025|0.025|0.025|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|**Table **|f-125%|



46 

Frankfurt School of Finance & Management — CPQF Working Paper No. 31 

|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|-0.1|0.025|0.025|0.025|0.025|0.025|0.025|0.025|0.2|0.2|0.2|0.2|0.2|0.2|0.2|_ϕ_|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|200|100|50|38|20|10|5|200|100|50|38|20|10|5|200|100|50|38|20|10|5|**SMA**|
|181.18|213.72|258.74|280.31|337.77|410.06|491.74|215.77|257.22|315.27|343.44|419.70|517.94|631.79|285.46|344.50|428.41|469.31|582.47|731.83|907.96|**Tot. Prof.**|
|-134.84|-183.98|-249.83|-280.86|-364.07|-471.41|-593.99|-120.87|-163.40|-220.77|-247.42|-318.52|-410.07|-513.69|-105.41|-140.75|-188.52|-210.27|-267.65|-340.14|-419.77|**Tot. Loss**|
|46.34|29.74|8.91|-0.55|-26.30|-61.35|-102.26|94.90|93.81|94.50|96.01|101.18|107.87|118.10|180.05|203.75|239.89|259.04|314.82|391.68|488.19|**Tot. Result**|
|1.2240|1.4253|1.7521|1.9101|2.4333|3.1302|3.9456|0.8395|0.8343|0.7997|0.7752|0.7353|0.6871|0.6387|0.5756|0.4896|0.3993|0.3664|0.2961|0.2332|0.1854|**Max DD**|
|-0.0559|-0.1011|-0.1698|-0.2032|-0.3278|-0.5057|-0.7154|0.0157|0.0158|0.0227|0.0263|0.0322|0.0385|0.0465|0.0679|0.0799|0.0948|0.1015|0.1177|0.1364|0.1551|**Mean Rtrn**|
|0.8761|1.0506|1.2488|1.3222|1.5259|1.6219|1.4721|0.5477|0.5574|0.5001|0.4718|0.4393|0.4206|0.3816|0.2681|0.2501|0.2274|0.2149|0.2081|0.2023|0.1993|**Vola**|
|-3.28|-2.56|-1.88|-1.64|-0.96|-0.25|0.60|-5.17|-5.15|-5.60|-5.74|-5.92|-6.26|-6.55|-1.98|-1.79|-0.72|0.57|0.57|0.53|0.51|**Skew**|
|13.41|8.38|4.96|4.04|2.16|1.29|1.69|35.74|34.87|42.67|45.82|50.29|56.01|65.38|36.19|36.88|22.76|3.34|3.31|3.26|3.22|**Kurt**|
|-0.9713|-1.0640|-1.1216|-1.1438|-1.1906|-1.2220|-1.2249|-0.1026|-0.1043|-0.0935|-0.0848|-0.0737|-0.0641|-0.0531|-0.0385|-0.0228|-0.0044|0.0037|0.0226|0.0424|0.0610|**VaR(5%)**|
|-0.0638|-0.0963|-0.1360|-0.1536|-0.2148|-0.3118|-0.4860|0.0286|0.0284|0.0454|0.0557|0.0734|0.0915|0.1219|0.2533|0.3194|0.4169|0.4722|0.5658|0.6742|0.7785|**Sharpe**|
|-0.1076|-0.1529|-0.2215|-0.2549|-0.3795|-0.5574|-0.7672|-0.0361|-0.0359|-0.0290|-0.0255|-0.0195|-0.0133|-0.0052|0.0165|0.0284|0.0433|0.0501|0.0663|0.0849|0.1037|**Exp.Exc.Retrn**|



##### **FRANKFURT SCHOOL / HFB – WORKING PAPER SERIES** 

|**No.**<br>|**Author/Title**<br>|**Year**<br>|
|---|---|---|
|184.|Sarah Eaton and Genia Kostka<br>Does Cadre Turnover Help or Hinder China’s Green Rise? Evidence from Shanxi Province|2012|
|183.|Behley, Dustin / Leyer, Michael<br>Evaluating Concepts for Short-term Control in Financial Service Processes|2011|
|182.|Herrmann-Pillath, Carsten<br>Naturalizing Institutions: Evolutionary Principles and Application on the Case of Money|2011|
|181.|Herrmann-Pillath, Carsten<br>Making Sense of  Institutional Change in China: The Cultural Dimension of Economic Growth and Modernization|2011|
|180.|Herrmann-Pillath, Carsten<br>Hayek 2.0: Grundlinien einer naturalistischen Theorie wirtschaftlicher Ordnungen|2011|
|179.|Braun, Daniel / Allgeier, Burkhard / Cremres, Heinz<br>Ratingverfahren: Diskriminanzanalyse versus Logistische Regression|<br>2011|
|178.|Kostka, Genia / Moslener, Ulf / Andreas, Jan G.<br>Barriers to Energy Efficency Improvement: Empirical Evidence from Small- and-Medium-Sized Enterprises in China|2011|
|177.|Löchel, Horst / Xiang Li, Helena<br>Understanding the High Profitability of Chinese Banks|2011|
|176.|Herrmann-Pillath, Carsten<br>Neuroökonomik, Institutionen und verteilte Kognition: Empirische Grundlagen eines nicht-reduktionistischen natura-<br>listischen Forschungsprogramms in den Wirtschaftswissenschaften|<br>2011|
|175.|Libman, Alexander/ Mendelski, Martin<br>History Matters, but How? An Example of Ottoman and Habsburg Legacies and Judicial Performance in Romania|<br>2011|
|174.|Kostka, Genia<br>Environmental Protection Bureau Leadership at the Provincial Level in China: Examining Diverging Career Back-<br>grounds and Appointment Patterns|2011|
|173.|Durst, Susanne / Leyer, Michael<br>Bedürfnisse von Existenzgründern in der Gründungsphase|2011|
|172.|Klein, Michael<br>Enrichment with Growth|2011|
|171.|Yu, Xiaofan<br>A Spatial Interpretation of the Persistency of China’s Provincial Inequality|2011|
|170.|Leyer, Michael<br>Stand der Literatur zur operativen Steuerung von Dienstleistungsprozessen|2011|
|169.|Libman, Alexander / Schultz, André<br>Tax Return as a Political Statement|2011|
|168.|Kostka, Genia / Shin, Kyoung<br>Energy Service Companies in China: The Role of Social Networks and Trust|2011|
|167.|Andriani, Pierpaolo / Herrmann-Pillath, Carsten<br>Performing Comparative Advantage: The Case of the Global Coffee Business|2011|
|166.|Klein, Michael / Mayer, Colin<br>Mobile Banking and Financial Inclusion: The Regulatory Lessons|2011|
|165.|Cremers, Heinz / Hewicker, Harald<br>Modellierung von Zinsstrukturkurven|2011|
|164.|Roßbach, Peter / Karlow, Denis<br>The Stability of Traditional Measures of Index Tracking Quality|2011|
|163.|Libman, Alexander / Herrmann-Pillath, Carsten / Yarav, Gaudav<br>Are Human Rights and Economic Well-Being Substitutes? Evidence from Migration Patterns across the Indian States|2011|
|162.|Herrmann-Pillath, Carsten / Andriani, Pierpaolo<br>Transactional Innovation and the De-commoditization of the Brazilian Coffee Trade|2011|
|161.|Christian Büchler, Marius Buxkaemper, Christoph Schalast, Gregor Wedell<br>Incentivierung des Managements bei Unternehmenskäufen/Buy-Outs mit Private Equity Investoren<br>– eine empirische Untersuchung –|2011|
|160.|Herrmann-Pillath, Carsten<br>Revisiting the Gaia Hypothesis: Maximum Entropy, Kauffman´s “Fourth Law” and Physiosemeiosis|2011|
|159.|Herrmann-Pillath, Carsten<br>A ‘Third Culture’ in Economics? An Essay on Smith, Confucius and the Rise of China|2011|
|158.|Boeing. Philipp / Sandner, Philipp<br>The Innovative Performance of China’s National Innovation System|2011|



|157.|Herrmann-Pillath, Carsten<br>Institutions, Distributed Cognition and Agency: Rule-following as Performative Action|2011|
|---|---|---|
|156.|Wagner, Charlotte<br>From Boom to Bust: How different has microfinance been from traditional banking?|2010|
|155.|Libman Alexander / Vinokurov, Evgeny<br>Is it really different? Patterns of Regionalisation in the Post-Soviet Central Asia|2010|
|154.|Libman, Alexander<br>Subnational Resource Curse: Do Economic or Political Institutions Matter?|2010|
|153.|Herrmann-Pillath, Carsten<br>Meaning and Function in the Theory of Consumer Choice: Dual Selves in Evolving Networks|2010|
|152.|Kostka, Genia / Hobbs, William<br>Embedded Interests and the Managerial Local State: Methanol Fuel-Switching in China|2010|
|151.|Kostka, Genia / Hobbs, William<br>Energy Efficiency in China: The Local Bundling of Interests and Policies|2010|
|150.|Umber, Marc P. / Grote, Michael H. / Frey, Rainer<br>Europe Integrates Less Than You Think. Evidence from the Market for Corporate Control in Europe and the US|2010|
|149.|Vogel, Ursula / Winkler, Adalbert<br>Foreign banks and financial stability in emerging markets: evidence from the global financial crisis|2010|
|148.|Libman, Alexander<br>Words or Deeds – What Matters? Experience of Decentralization in Russian Security Agencies|2010|
|147.|Kostka, Genia / Zhou, Jianghua<br>Chinese firms entering China's low-income market: Gaining competitive advantage by partnering governments|2010|
|146.|Herrmann-Pillath, Carsten<br>Rethinking Evolution, Entropy and Economics: A triadic conceptual framework for the Maximum Entropy Principle as<br>applied to the growth of knowledge|2010|
|145.|Heidorn, Thomas / Kahlert, Dennis<br>Implied Correlations of iTraxx Tranches during the Financial Crisis|2010|
|144|Fritz-Morgenthal, Sebastian G. / Hach, Sebastian T. / Schalast, Christoph<br>M&A im Bereich Erneuerbarer Energien|2010|
|143.|Birkmeyer, Jörg / Heidorn, Thomas / Rogalski, André<br>Determinanten von Banken-Spreads während der Finanzmarktkrise|2010|
|142.|Bannier, Christina E. / Metz, Sabrina<br>Are SMEs large firms en miniature? Evidence from a growth analysis|2010|
|141.|Heidorn, Thomas / Kaiser, Dieter G. / Voinea, André<br>The Value-Added of Investable Hedge Fund Indices|2010|
|140.|Herrmann-Pillath, Carsten<br>The Evolutionary Approach to Entropy: Reconciling Georgescu-Roegen’s Natural Philosophy with the Maximum<br>Entropy Framework|2010|
|139.|Heidorn, Thomas / Löw, Christian / Winker, Michael<br>Funktionsweise und Replikationstil europäischer Exchange Traded Funds auf Aktienindices|2010|
|138.|Libman, Alexander<br>Constitutions, Regulations, and Taxes: Contradictions of Different Aspects of Decentralization|2010|
|137.|Herrmann-Pillath, Carsten / Libman, Alexander / Yu, Xiaofan<br>State and market integration in China: A spatial econometrics approach to ‘local protectionism’|2010|
|136.|Lang, Michael / Cremers, Heinz / Hentze, Rainald<br>Ratingmodell zur Quantifizierung des Ausfallrisikos von LBO-Finanzierungen|2010|
|135.|Bannier, Christina / Feess, Eberhard<br>When high-powered incentive contracts reduce performance: Choking under pressure as a screening device|2010|
|134.|Herrmann-Pillath, Carsten<br>Entropy, Function and Evolution: Naturalizing Peircian Semiosis|2010|
|133.|Bannier, Christina E. / Behr, Patrick / Güttler, Andre<br>Rating opaque borrowers: why are unsolicited ratings lower?|2009|
|132.|Herrmann-Pillath, Carsten<br>Social Capital, Chinese Style: Individualism, Relational Collectivism and the Cultural Embeddedness of the Institu-<br>tions-Performance Link|2009|
|131.|Schäffler, Christian / Schmaltz, Christian<br>Market Liquidity: An Introduction for Practitioners|2009|
|130.|Herrmann-Pillath, Carsten<br>||
||Dimensionen des Wissens: Ein kognitiv-evolutionärer Ansatz auf der Grundlage von F.A. von Hayeks Theorie der<br>„Sensory Order“|2009|



|129.|Hankir, Yassin / Rauch, Christian / Umber, Marc<br>It’s the Market Power, Stupid! – Stock Return Patterns in International Bank M&A|2009|
|---|---|---|
|128.|Herrmann-Pillath, Carsten<br>Outline of a Darwinian Theory of Money|2009|
|127.|Cremers, Heinz / Walzner, Jens<br>Modellierung des Kreditrisikos im Portfoliofall|2009|
|126.|Cremers, Heinz / Walzner, Jens<br>||
||Modellierung des Kreditrisikos im Einwertpapierfall|2009|
|125.|Heidorn, Thomas / Schmaltz, Christian||
||Interne Transferpreise für Liquidität|2009|
|124.|Bannier, Christina E. / Hirsch, Christian<br>The economic function of credit rating agencies - What does the watchlist tell us?|2009|
|123.|Herrmann-Pillath, Carsten<br>A Neurolinguistic Approach to Performativity in Economics|2009|
|122.|Winkler, Adalbert / Vogel, Ursula<br>Finanzierungsstrukturen und makroökonomische Stabilität in den Ländern Südosteuropas, der Türkei und in den GUS-<br>Staaten|2009|
|121.|Heidorn, Thomas / Rupprecht, Stephan<br>Einführung in das Kapitalstrukturmanagement bei Banken|2009|
|120.|Rossbach, Peter<br>Die Rolle des Internets als Informationsbeschaffungsmedium in Banken|2009|
|119.|Herrmann-Pillath, Carsten<br>Diversity Management und diversi-tätsbasiertes Controlling: Von der „Diversity Scorecard“ zur „Open Balanced<br>Scorecard|2009|
|118.|Hölscher, Luise / Clasen, Sven<br>Erfolgsfaktoren von Private Equity Fonds|2009|
|117|Bannier Christina E||
|.|,  .<br>Is there a hold-up benefit in heterogeneous multiple bank financing?|2009|
|116.|Roßbach, Peter / Gießamer, Dirk<br>Ein eLearning-System zur Unterstützung der Wissensvermittlung von Web-Entwicklern in Sicherheitsthemen|2009|
|115.|Herrmann-Pillath, Carsten<br>Kulturelle Hybridisierung und Wirtschaftstransformation in China|2009|
|114.|Schalast, Christoph:||
||<br>Staatsfonds – „neue“ Akteure an den Finanzmärkten?|2009|
|113.|Schalast, Christoph / Alram, Johannes||
||<br>Konstruktion einer Anleihe mit hypothekarischer Besicherung|2009|
|112.|Schalast, Christoph / Bolder, Markus / Radünz, Claus / Siepmann, Stephanie / Weber, Thorsten<br>Transaktionen und Servicing in der Finanzkrise: Berichte und Referate des Frankfurt School NPL Forums 2008|2009|
|111.|Werner, Karl / Moormann, Jürgen<br>Efficiency and Profitability of European Banks – How Important Is Operational Efficiency?|2009|
|110.|Herrmann-Pillath, Carsten<br>Moralische Gefühle als Grundlage einer wohlstandschaffenden Wettbewerbsordnung:<br>Ein neuer Ansatz zur erforschung von Sozialkapital und seine Anwendung auf China|2009|
|109.|Heidorn, Thomas / Kaiser, Dieter G. / Roder, Christoph<br>Empirische Analyse der Drawdowns von Dach-Hedgefonds|2009|
|108.|Herrmann-Pillath, Carsten<br>Neuroeconomics, Naturalism and Language|2008|
|107.|Schalast, Christoph / Benita, Barten<br>Private Equity und Familienunternehmen – eine Untersuchung unter besonderer Berücksichtigung deutscher<br>Maschinen- und Anlagenbauunternehmen|2008|
|106.|Bannier, Christina E. / Grote, Michael H.<br>Equity Gap? – Which Equity Gap? On the Financing Structure of Germany’s Mittelstand|2008|
|105.|Herrmann-Pillath, Carsten<br>The Naturalistic Turn in Economics: Implications for the Theory of Finance|2008|
|104.|Schalast, Christoph (Hrgs.) / Schanz, Kay-Michael / Scholl, Wolfgang<br>Aktionärsschutz in der AG falsch verstanden? Die Leica-Entscheidung des LG Frankfurt am Main|2008|
|103.|Bannier, Christina E./ Müsch, Stefan<br>Die Auswirkungen der Subprime-Krise auf den deutschen LBO-Markt für Small- und MidCaps|2008|
|102|Cremers Heinz / Vetter Michael||
|.|,   ,<br>Das IRB-Modell des Kreditrisikos im Vergleich zum Modell einer logarithmisch normalverteilten Verlustfunktion|2008|
|101.|Heidorn, Thomas / Pleißner, Mathias<br>||
||Determinanten Europäischer CMBS Spreads. Ein empirisches Modell zur Bestimmung der Risikoaufschläge von<br>Commercial Mortgage-Backed Securities (CMBS)|2008|



|100.|Schalast, Christoph (Hrsg.) / Schanz, Kay-Michael<br>Schaeffler KG/Continental AG im Lichte der CSX Corp-Entscheidung des US District Court for the Southern District||
|---|---|---|
||.<br>of New York|2008|
|99.|Hölscher, Luise / Haug, Michael / Schweinberger, Andreas<br>Analyse von Steueramnestiedaten|2008|
|98.|Heimer, Thomas / Arend, Sebastian<br>The Genesis of the Black-Scholes Option Pricing Formula|2008|
|97.|Heimer, Thomas / Hölscher, Luise / Werner, Matthias Ralf<br>Access to Finance and Venture Capital for Industrial SMEs|2008|
|96.|Böttger, Marc / Guthoff, Anja / Heidorn, Thomas<br>Loss Given Default Modelle zur Schätzung von Recovery Rates|2008|
|95.|Almer, Thomas / Heidorn, Thomas / Schmaltz, Christian<br>The Dynamics of Short- and Long-Term CDS-spreads of Banks|2008|
|94.|Barthel, Erich / Wollersheim, Jutta<br>Kulturunterschiede bei Mergers & Acquisitions: Entwicklung eines Konzeptes zur Durchführung einer Cultural Due<br>Diligence|2008|
|93.|Heidorn, Thomas / Kunze, Wolfgang / Schmaltz, Christian<br>Liquiditätsmodellierung von Kreditzusagen (Term Facilities and Revolver)|2008|
|92.|Burger, Andreas<br>Produktivität und Effizienz in Banken – Terminologie, Methoden und Status quo|2008|
|91.|Löchel, Horst / Pecher, Florian<br>The Strategic Value of Investments in Chinese Banks by Foreign Financial Insitutions|2008|
|90.|Schalast, Christoph / Morgenschweis, Bernd / Sprengetter, Hans Otto / Ockens, Klaas / Stachuletz, Rainer /<br>Safran, Robert<br>Der deutsche NPL Markt 2007: Aktuelle Entwicklungen, Verkauf und Bewertung – Berichte und Referate des NPL<br>Forums 2007|2008|
|89.|Schalast, Christoph / Stralkowski, Ingo<br>10 Jahre deutsche Buyouts|2008|
|88|Bannier Christina E/ Hirsch Christian||
|.|,  . ,<br>The Economics of Rating Watchlists: Evidence from Rating Changes|2007|
|87.|Demidova-Menzel, Nadeshda / Heidorn, Thomas<br>Gold in the Investment Portfolio|2007|
|86.|Hölscher, Luise / Rosenthal, Johannes<br>Leistungsmessung der Internen Revision|2007|
|85.|Bannier, Christina / Hänsel, Dennis<br>Determinants of banks' engagement in loan securitization|2007|
|84.|Bannier, Christina<br>“Smoothing“ versus “Timeliness“ - Wann sind stabile Ratings optimal und welche Anforderungen sind an optimale<br>Berichtsregeln zu stellen?|2007|
|83.|Bannier, Christina E.<br>Heterogeneous Multiple Bank Financing: Does it Reduce Inefficient Credit-Renegotiation Incidences?|2007|
|82.|Cremers, Heinz / Löhr, Andreas<br>Deskription und Bewertung strukturierter Produkte unter besonderer Berücksichtigung verschiedener Marktszenarien|2007|
|81.|Demidova-Menzel, Nadeshda / Heidorn, Thomas<br>Commodities in Asset Management|2007|
|80.|Cremers, Heinz / Walzner, Jens<br>Risikosteuerung mit Kreditderivaten unter besonderer Berücksichtigung von Credit Default Swaps|2007|
|79.|Cremers, Heinz / Traughber, Patrick<br>Handlungsalternativen einer Genossenschaftsbank im Investmentprozess unter Berücksichtigung der Risikotragfähig-<br>keit|2007|
|78.|Gerdesmeier, Dieter / Roffia, Barbara<br>Monetary Analysis: A VAR Perspective|2007|
|77.|Heidorn, Thomas / Kaiser, Dieter G. / Muschiol, Andrea<br>Portfoliooptimierung mit Hedgefonds unter Berücksichtigung höherer Momente der Verteilung|2007|
|76.|Jobe, Clemens J. / Ockens, Klaas / Safran, Robert / Schalast, Christoph<br>Work-Out und Servicing von notleidenden Krediten – Berichte und Referate des HfB-NPL Servicing Forums 2006|2006|
|75.|Abrar, Kamyar / Schalast, Christoph<br>Fusionskontrolle in dynamischen Netzsektoren am Beispiel des Breitbandkabelsektors|2006|
|74.|Schalast, Christoph / Schanz, Kay-Michael<br>Wertpapierprospekte: Markteinführungspublizität nach EU-Prospektverordnung und Wertpapierprospektgesetz 2005|2006|



|73.|Dickler, Robert A. / Schalast, Christoph<br>Distressed Debt in Germany: What´s Next? Possible Innovative Exit Strategies|2006|
|---|---|---|
|72|Blk A / Pllit Tht||
|.|ee, nsgar  oe, orsen<br>How the ECB and the US Fed set interest rates|2006|
|71.|Heidorn, Thomas / Hoppe, Christian / Kaiser, Dieter G.<br>Heterogenität von Hedgefondsindizes|2006|
|70.|Baumann, Stefan / Löchel, Horst||
||<br>The Endogeneity Approach of the Theory of Optimum Currency Areas - What does it mean for ASEAN + 3?|2006|
|69.|Heidorn, Thomas / Trautmann, Alexandra<br>Niederschlagsderivate|2005|
|68.|Heidorn, Thomas / Hoppe, Christian / Kaiser, Dieter G.<br>Möglichkeiten der Strukturierung von Hedgefondsportfolios|2005|
|67.|Belke, Ansgar / Polleit, Thorsten<br>(How) Do Stock Market Returns React to Monetary Policy ? An ARDL Cointegration Analysis for Germany|2005|
|66.|Daynes, Christian / Schalast, Christoph<br>Aktuelle Rechtsfragen des Bank- und Kapitalmarktsrechts II: Distressed Debt - Investing in Deutschland|2005|
|65.|Gerdesmeier, Dieter / Polleit, Thorsten<br>Measures of excess liquidity|2005|
|64.|Becker, Gernot M. / Harding, Perham / Hölscher, Luise||
||Financing the Embedded Value of Life Insurance Portfolios|2005|
|63.|Schalast, Christoph||
||<br>Modernisierung der Wasserwirtschaft im Spannungsfeld von Umweltschutz und Wettbewerb – Braucht Deutschland<br>eine Rechtsgrundlage für die Vergabe von Wasserversorgungskonzessionen? –|2005|
|62.|Bayer, Marcus / Cremers, Heinz / Kluß, Norbert<br>Wertsicherungsstrategien für das Asset Management|2005|
|61.|Löchel, Horst / Polleit, Thorsten||
||A case for money in the ECB monetary policy strategy|2005|
|60.|Richard Jörg / Schalast Christoph / Schanz Kay-Michael||
||,   ,   ,<br>Unternehmen im Prime Standard - „Staying Public“ oder „Going Private“? - Nutzenanalyse der Börsennotiz -|2004|
|59.|Heun, Michael / Schlink, Torsten<br>Early Warning Systems of Financial Crises - Implementation of a currency crisis model for Uganda|2004|
|58.|Heimer, Thomas / Köhler, Thomas||
||Auswirkungen des Basel II Akkords auf österreichische KMU|2004|
|57.|Heidorn, Thomas / Meyer, Bernd / Pietrowiak, Alexander||
||<br>Performanceeffekte nach Directors´Dealings in Deutschland, Italien und den Niederlanden|2004|
|56.|Gerdesmeier, Dieter / Roffia, Barbara<br>The Relevance of real-time data in estimating reaction functions for the euro area|2004|
|55.|Barthel, Erich / Gierig, Rauno / Kühn, Ilmhart-Wolfram<br>Unterschiedliche Ansätze zur Messung des Humankapitals|2004|
|54.|Anders, Dietmar / Binder, Andreas / Hesdahl, Ralf / Schalast, Christoph / Thöne, Thomas<br>Aktuelle Rechtsfragen des Bank- und Kapitalmarktrechts I :<br>Non-Performing-Loans / Faule Kredite - Handel, Work-Out, Outsourcing und Securitisation|2004|
|53.|Polleit, Thorsten<br>The Slowdown in German Bank Lending – Revisited|2004|
|52|Heidorn Thomas / Sirausano Tindaro||
|.|,   g,<br>Die Anwendbarkeit der Behavioral Finance im Devisenmarkt|2004|
|51.|Schütze, Daniel / Schalast, Christoph (Hrsg.)<br>Wider die Verschleuderung von Unternehmen durch Pfandversteigerung|2004|
|50.|Gerhold, Mirko / Heidorn, Thomas<br>Investitionen und Emissionen von Convertible Bonds (Wandelanleihen)|2004|
|49.|Chevalier, Pierre / Heidorn, Thomas / Krieger, Christian<br>Temperaturderivate zur strategischen Absicherung von Beschaffungs- und Absatzrisiken|2003|
|48|Bkr Grnt M / Sr Nrbrt||
|.|ece, eo .  eege, oe<br>Internationale Cash Flow-Rechnungen aus Eigner- und Gläubigersicht|2003|
|47.|Boenkost, Wolfram / Schmidt, Wolfgang M.||
||Notes on convexity and quanto adjustments for interest rates and related options|2003|
|46.|Hess, Dieter||
||<br>Determinants of the relative price impact of unanticipated Information in<br>U.S. macroeconomic releases|2003|
|45.|Cremers, Heinz / Kluß, Norbert / König, Markus<br>Incentive Fees. Erfolgsabhängige Vergütungsmodelle deutscher Publikumsfonds|2003|



|44.|Heidorn, Thomas / König, Lars<br>Investitionen in Collateralized Debt Obligations|2003|
|---|---|---|
|43|Khlt Hl / S Nbt||
|.|aer, oger  eeger, orer<br>Bilanzierung von Unternehmenszusammenschlüssen nach US-GAAP|2003|
|42.|Beiträge von Studierenden des Studiengangs BBA 012 unter Begleitung von Prof. Dr. Norbert Seeger<br>Rechnungslegung im Umbruch - HGB-Bilanzierung im Wettbewerb mit den internationalen<br>Standards nach IAS und US-GAAP|2003|
|41.|Overbeck, Ludger / Schmidt, Wolfgang<br>Modeling Default Dependence with Threshold Models|2003|
|40.|Balthasar, Daniel / Cremers, Heinz / Schmidt, Michael<br>Portfoliooptimierung mit Hedge Fonds unter besonderer Berücksichtigung der Risikokomponente|2002|
|39.|Heidorn, Thomas / Kantwill, Jens<br>Eine empirische Analyse der Spreadunterschiede von Festsatzanleihen zu Floatern im Euroraum<br>und deren Zusammenhang zum Preis eines Credit Default Swaps|2002|
|38.|Böttcher, Henner / Seeger, Norbert<br>Bilanzierung von Finanzderivaten nach HGB, EstG, IAS und US-GAAP|2003|
|37.|Moormann, Jürgen<br>Terminologie und Glossar der Bankinformatik|2002|
|36.|Heidorn, Thomas<br>Bewertung von Kreditprodukten und Credit Default Swaps|2001|
|35.|Heidorn, Thomas / Weier, Sven<br>Einführung in die fundamentale Aktienanalyse|2001|
|34.|Seeger, Norbert<br>International Accounting Standards (IAS)|2001|
|33.|Moormann, Jürgen / Stehling, Frank<br>Strategic Positioning of E-Commerce Business Models in the Portfolio of Corporate Banking|2001|
|32.|Sokolovsky, Zbynek / Strohhecker, Jürgen||
||<br>Fit für den Euro, Simulationsbasierte Euro-Maßnahmenplanung für Dresdner-Bank-Geschäftsstellen|2001|
|31.|Roßbach, Peter<br>Behavioral Finance - Eine Alternative zur vorherrschenden Kapitalmarkttheorie?|2001|
|30.|Heidorn, Thomas / Jaster, Oliver / Willeitner, Ulrich<br>Event Risk Covenants|2001|
|29.|Biswas, Rita / Löchel, Horst<br>Recent Trends in U.S. and German Banking: Convergence or Divergence?|2001|
|28.|Eberle, Günter Georg / Löchel, Horst<br>Die Auswirkungen des Übergangs zum Kapitaldeckungsverfahren in der Rentenversicherung auf die Kapitalmärkte|2001|
|27.|Heidorn, Thomas / Klein, Hans-Dieter / Siebrecht, Frank<br>Economic Value Added zur Prognose der Performance europäischer Aktien|2000|
|26.|Cremers, Heinz<br>Konvergenz der binomialen Optionspreismodelle gegen das Modell von Black/Scholes/Merton|2000|
|25.|Löchel, Horst<br>Die ökonomischen Dimensionen der ‚New Economy‘|2000|
|24.|Frank, Axel / Moormann, Jürgen<br>Grenzen des Outsourcing: Eine Exploration am Beispiel von Direktbanken|2000|
|23.|Heidorn, Thomas / Schmidt, Peter / Seiler, Stefan<br>Neue Möglichkeiten durch die Namensaktie|2000|
|22.|Böger, Andreas / Heidorn, Thomas / Graf Waldstein, Philipp<br>Hybrides Kernkapital für Kreditinstitute|2000|
|21.|Heidorn, Thomas<br>Entscheidungsorientierte Mindestmargenkalkulation|2000|
|20.|Wolf, Birgit<br>Die Eigenmittelkonzeption des § 10 KWG|2000|
|19.|Cremers, Heinz / Robé, Sophie / Thiele, Dirk<br>Beta als Risikomaß - Eine Untersuchung am europäischen Aktienmarkt|2000|
|18.|Cremers, Heinz||
||<br>Optionspreisbestimmung|1999|
|17.|Cremers, Heinz<br>Value at Risk-Konzepte für Marktrisiken|1999|
|16.|Chevalier, Pierre / Heidorn, Thomas / Rütze, Merle<br>Gründung einer deutschen Strombörse für Elektrizitätsderivate|1999|



|15.|Deister, Daniel / Ehrlicher, Sven / Heidorn, Thomas<br>CatBonds|1999|
|---|---|---|
|14.|Jochum, Eduard||
||Hoshin Kanri / Management by Policy (MbP)|1999|
|13.|Heidorn, Thomas||
||Kreditderivate|1999|
|12.|Heidorn, Thomas||
||Kreditrisiko (CreditMetrics)|1999|
|11.|Moormann, Jürgen||
||Terminologie und Glossar der Bankinformatik|1999|
|10.|Löchel, Horst<br>The EMU and the Theory of Optimum Currency Areas|1998|
|09.|Löchel, Horst<br>Die Geldpolitik im Währungsraum des Euro|1998|
|08.|Heidorn, Thomas / Hund, Jürgen<br>Die Umstellung auf die Stückaktie für deutsche Aktiengesellschaften|1998|
|07.|Moormann, Jürgen<br>Stand und Perspektiven der Informationsverarbeitung in Banken|1998|
|06.|Heidorn, Thomas / Schmidt, Wolfgang<br>LIBOR in Arrears|1998|
|05.|Jahresbericht 1997|1998|
|04.|Ecker, Thomas / Moormann, Jürgen<br>Die Bank als Betreiberin einer elektronischen Shopping-Mall|1997|
|03.|Jahresbericht 1996|1997|
|02.|Cremers, Heinz / Schwarz, Willi<br>Interpolation of Discount Factors|1996|
|01.|Moormann, Jürgen<br>Lean Reporting und Führungsinformationssysteme bei deutschen Finanzdienstleistern|1995|
|**FRAN**|**KFURTSCHOOL/ HFB – WORKINGPAPERSERIES**||
|**CENT**|**REFORPRACTICALQUANTITATIVEFINANCE**||



|**No.**|**Author/Title**|**Year**|
|---|---|---|
|30.|Detering, Nils / Zhou, Qixiang / Wystup, Uwe<br>Volatilität als Investment. Diversifikationseigenschaften von Volatilitätsstrategien|2012|
|29.|Scholz, Peter / Walther, Ursula<br>The Trend is not Your Friend! Why Empirical Timing Success is Determined by the Underlying’s Price Characteristics<br>and Market Efficiency is Irrelevant|2011|
|28.|Beyna, Ingo / Wystup, Uwe<br>Characteristic Functions in the Cheyette Interest Rate Model|2011|
|27.|Detering, Nils / Weber, Andreas / Wystup, Uwe<br>Return distributions of equity-linked retirement plans|2010|
|26.|Veiga, Carlos / Wystup, Uwe<br>Ratings of Structured Products and Issuers’ Commitments|2010|
|25.|Beyna, Ingo / Wystup, Uwe<br>On the Calibration of the Cheyette. Interest Rate Model|2010|
|24.|Scholz, Peter / Walther, Ursula<br>Investment Certificates under German Taxation. Benefit or Burden for Structured Products’ Performance|2010|
|23.|Esquível, Manuel L. / Veiga, Carlos / Wystup, Uwe<br>Unifying Exotic Option Closed Formulas|2010|
|22.|Packham, Natalie / Schlögl, Lutz / Schmidt, Wolfgang M.<br>Credit gap risk in a first passage time model with jumps|2009|
|21.|Packham, Natalie / Schlögl, Lutz / Schmidt, Wolfgang M.<br>Credit dynamics in a first passage time model with jumps|2009|
|20.|Reiswich, Dimitri / Wystup, Uwe<br>FX Volatility Smile Construction|2009|
|19.|Reiswich, Dimitri / Tompkins, Robert<br>Potential PCA Interpretation Problems for Volatility Smile Dynamics|2009|



|18.|Keller-Ressel, Martin / Kilin, Fiodar||
|---|---|---|
||Forward-Start Options in the Barndorff-Nielsen-Shephard Model|2008|
|17.|Griebsch, Susanne / Wystup, Uwe<br>On the Valuation of Fader and Discrete Barrier Options in Heston’s Stochastic Volatility Model|2008|
|16.|Veiga, Carlos / Wystup, Uwe<br>Closed Formula for Options with Discrete Dividends and its Derivatives|2008|
|15.|Packham, Natalie / Schmidt, Wolfgang<br>Latin hypercube sampling with dependence and applications in finance|2008|
|14.|Hakala, Jürgen / Wystup, Uwe<br>FX Basket Options|2008|
|13.|Weber, Andreas / Wystup, Uwe<br>Vergleich von Anlagestrategien bei Riesterrenten ohne Berücksichtigung von Gebühren. Eine Simulationsstudie zur<br>Verteilung der Renditen|2008|
|12.|Weber, Andreas / Wystup, Uwe<br>Riesterrente im Vergleich. Eine Simulationsstudie zur Verteilung der Renditen|2008|
|11.|Wystup, Uwe<br>Vanna-Volga Pricing|2008|
|10.|Wystup, Uwe<br>Foreign Exchange Quanto Options|2008|
|09.|Wystup, Uwe<br>Foreign Exchange Symmetries|2008|
|08.|Becker, Christoph / Wystup, Uwe<br>Was kostet eine Garantie? Ein statistischer Vergleich der Rendite von langfristigen Anlagen|2008|
|07.|Schmidt, Wolfgang<br>Default Swaps and Hedging Credit Baskets|2007|
|06.|Kilin, Fiodar<br>Accelerating the Calibration of Stochastic Volatility Models|2007|
|05.|Griebsch, Susanne/ Kühn, Christoph / Wystup, Uwe<br>Instalment Options: A Closed-Form Solution and the Limiting Case|2007|
|04.|Boenkost, Wolfram / Schmidt, Wolfgang M.<br>Interest Rate Convexity and the Volatility Smile|2006|
|03.|Becker, Christoph/ Wystup, Uwe<br>On the Cost of Delayed Currency Fixing Announcements|2005|
|02.|Boenkost, Wolfram / Schmidt, Wolfgang M.<br>Cross currency swap valuation|2004|
|01.|Wallner, Christian / Wystup, Uwe||
||Efficient Computation of Option Price Sensitivities for Options of American Style|2004|



##### **HFB – SONDERARBEITSBERICHTE DER HFB - BUSINESS SCHOOL OF FINANCE & MANAGEMENT** 

|**No.**|**Author/Title**|**Year**|
|---|---|---|
|01.|Nicole Kahmer / Jürgen Moormann||
||Studie zur Ausrichtung von Banken an Kundenprozessen am Beispiel des Internet||
||(Preis: €  120,--)|2003|



Printed edition: € 25.00 + € 2.50 shipping 

##### Download: 

Working Paper: http://www.frankfurt- 

school.de/content/de/research/publications/list_of_publication/list_of_publication CPQF: http://www.frankfurt-school.de/content/de/cpqf/research_publications.html 

##### **Order address / contact** 

Frankfurt School of Finance & Management 

Sonnemannstr. 9 – 11  �  D – 60314 Frankfurt/M.  �  Germany Phone: +49 (0) 69 154 008 – 734  �  Fax: +49 (0) 69 154 008 – 728 

eMail: e.lahdensuu@fs.de 

Further information about Frankfurt School of Finance & Management 

may be obtained at: http://www.fs.de 

