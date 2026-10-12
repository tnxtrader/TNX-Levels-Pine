Capital Markets Review Vol. 31, No. 1, pp. 59-71 (2023) 

# **Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants** 

Dinabandhu Bag<sup>1</sup> & Saurabh Goel<sup>2</sup> 

_1National Institute of Technology Rourkela, Odisha, India._ 

_2Delhi Technological University, New Delhi, India._ 

**<mark>Abstract: Research Question</mark>** <mark>: This paper explores the pre-market auction price behaviour. The pre-market auction is a short duration auction, where the orders are executed with too little time for revision by the makers. The literature paid attention to application of random walk hypothesis (RWH) and its variants in efficient market (EMH) tests.</mark> **<mark>Motivation:</mark>** <mark>The pre-opening auction is an extremely short duration auction where traders are interested in a limited number of large cap stocks and the orders are not transparent. The interest lies on efficiency tests of discrete prices during the pre-market auction for the benefit of investors.</mark> **<mark>Idea</mark>** <mark>: The mechanism of price discovery in call auctions is important since they could impact normal markets. We aim to test major relevant hypotheses for pre-opening equilibrium prices. The rejection of the randomness would mean that it is possible to use historical stock prices alone.</mark> **<mark>Data</mark>** <mark>: The sample comprises all 50 NSE 50 Index constituent stocks sampled during the year 2019. The NSE constituent stocks maintain the highest market capitalization and have a long history of trading.</mark> **<mark>Method/Tools</mark>** <mark>: It summarizes the source literature on objectively driven synthesis on simulation-based decision making since the early period of 1973. Multivariate lognormal distribution is a challenging method than ordinary univariate Monte Carlo. By generating a 50 X 50 covariance matrix of prices and solving for Cholesky roots, the results were compared against lognormal multivariate Monte Carlo simulation to explore the estimates of volatility.</mark> **<mark>Findings</mark>** <mark>: The results demonstrate a good case for the tests of RWH and objectively arriving at the pre-opening equilibrium prices. The co-efficient of variation (COV) remained at 3.33%. We found that the stock prices were correlated among themselves, which infers the weak form of efficiency. Previous results had mentioned that MC generated higher sample variances and unsuitable, however, we found lower variances in using multi-variate Monte Carlo.</mark> **<mark>Contributions</mark>** <mark>: The contribution lies in the attempts using multi-variate log normal distribution to deduce prices with lower estimated variance. The results have implications to making trade decisions and portfolio construction during the Covid period, where high degree of persisting decline happened to indices.</mark> 

**Keywords** : Pre-opening, efficient, multivariate log-normal, Monte Carlo. **JEL Classification** : G11, C14, C15, C19 

>  Corresponding author: Dinabandhu Bag. Tel.: +91-6612462803. Email: dinabandhu.bag@gmail.com 

> Received 18 January 2022; Final revised 27 Dec 2022; Accepted 16 Mar 2023; Available online 30 Mar 2023. To link to this article: https://www.mfa.com.my/cmr/v31_i1_a4/ 

> © Malaysian Finance Association, 2023. This work is licensed under the terms of the Creative Commons Attribution (CC BY) (http://creativecommons.org/licenses/by/4.0/). 

59 

Dinabandhu Bag & Saurabh Goel 

## **1. Introduction** 

An efficient stock market attracts genuine investors to the capital market. The weak form of efficient market hypothesis (EMH) states that a stock's current price would be reflected in the stock's historical prices (Fama, 1991). In a pre-market auction, the individual trade orders are collated and the equilibrium opening prices are derived for each stock and publicly displayed each day. During the short period auction, the makers cannot see the order position of each other, until the final allocation are displayed at the end of the auction. In an efficient market such equilibrium prices may follow Random Walk (RWH). However, due to the very nature of the pre-market, one would expect the equilibrium prices to have some degree of influence with the stock closing prices of the previous trading day (historical). Shiller (1981) identified that stock prices were volatile than their expected discounted cash flows would have made Camerer (1990) observed that prices were volatile and random. Further works on efficient market hypothesis on normal markets had provided mixed inferences. The pre-opening auction prices would follow random walk and could be consistent with the weak form of the EMH (see Malkiel, 2003; Fama, 1991). There exist alternative methods to test for the weak form of market efficiency (e.g., runs test, unit root test, etc.) on normal trading data. However, the technique of Monte Carlo simulation (MC) were not used abundantly in field applications. The MC method is more relevant today because of the uncertainty during the Covid period, where high degree of te persisting decline happened to indices. Technical analysis could not be relied on because of its strong demand on historical prices alone. 

There are few questions examined here; how to use MC simulations to stock prices, how accurate is MC method, how to improve the accuracy and reliability for the simulation of the stock prices, respectively? Although, there are many ways of using MC simulation, this study will concentrate on basic approaches with historical data as input parameters; how effectively and efficiently the prediction may result in knowledge to benefit the investors? The results could demonstrate a good case for the tests of RWH and objectively arriving at the preopening equilibrium prices. We would find that the stock prices were correlated among themselves, which infers the weak form of efficiency. The contribution would lie in the attempts using multi-variate log normal distribution to deduce prices with lower estimated variance. The results have implications for making trade decisions and portfolio construction during the Covid period. 

The rest of the paper is divided into sections; Section 2 highlights the literature review; Section 3 discusses the Hypotheses; Section 4 explains the methodology; Section 5 describes the data; Section 6 describes the results of simulation. Lastly, Section 7 gives concluding remarks. 

## **2. Literature Review** 

Early discussions of the RWH quote Tversky and Kahneman (1974) who had suggested ‘the rule of thumb’ in “decision making”. The thumb rule occurs when the current price levels were used as “anchors” to arrive at future prices. Later, a growing body of literature paid attention to application of RWH and its variants in testing forms of the EMH. Camerer (1990) had examined price bubbles to distinguish rational response from irrational herd behaviour in the market. Ariely _et al_ . (2006) argued that the judgment approach led to biased response. Few studies had devoted to comparing the results between ARIMA (time series) and MC methods in models of stock price, real estate price, interest rate yield, derivative and option prices, etc. 

Table 1 summarizes a chronological account of relevant findings in the literature since early period of 1973. It presents relevant source literature in the domain of objectively driven synthesis on simulation based decision making since early period of 1973. As shown in Table 1, many authors have inferred RWH whereas, only a few have negated RWH. Few authors, 

60 

Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants 

Chang and Ting (2000), negated RWH by variance ratio tests of the Taiwan Stock Exchange index. Lo and MacKinlay (1998) tested RWH using variance ratio and did not confirm it for indices of for USA. The use of MC for auction prices were few. Specifically, the works of Boyle (1977), Hoesli _et al._ (2005), Whiteside (2008), had been discussed in the literature. Hoesli _et al._ (2005) applied MC simulation to derive and compare the Swiss real estate market prices. Hoesli _et al._ (2005) detected the sensitivity of input parameters of MC. Whiteside (2008) had confirmed that MC could be used to simulate conditional distribution in input functions. The useful properties of MC were highlighted in applications of EMH, and RWH. Cheung and Coutts (2001) demonstrated the weak form of Hang Seng index. Abraham _et al._ (2002) tested RWH for Kuwait, Saudi Arabia and Bahrain markets. Buguk and Brorsen (2003) found some evidence of weak form of the Istanbul exchange. Asiri (2008) confirmed RWH for Bahrain stock prices. Erdos and Ormos (2010) inferred weak-form of the US market. Okpara  (2010) inferred random walk in the Nigerian market. Alexeev and Tapon (2011) demonstrated the weak-form efficiency for the Toronto Stock Exchange (TSX). 

**<u>Table 1:</u>** <u>Summary of simulations applications to stock prices</u> 

|Source|Year|Global|Purpose|Inferences & limitations|
|---|---|---|---|---|
|Malkiel|1973|TSX|Weak form efficiency for stocks.|Weak form efficiency for<br>stocks.|
|Tversky and<br>Kahneman|1974|Theory|Psychological bias and heuristic<br>judgments in numeric “decision<br>making”.|Heuristic Judgments could<br>lead to strong bias and<br>systemic errors.|
|Boyle|1977|USA|MC simulation for option pricing.|MC can be used for<br>numerical forecasting of<br>European stock call<br>options that pay<br>dividends.|
|Camerer|1990|USA|Asset price bubbles.|Rational response is<br>distinguished from<br>irrational bias observed in<br>the market.|
|Lo and<br>MacKinlay|1998|USA|RWH random walk using<br>variance ratio.|Do not follow random<br>walks, simple<br>specification test.|
|Chang and<br>Ting|2000|Taiwan|RWH on variance ratio tests of<br>the stock index.|Negated RWH on<br>variance ratio tests of the<br>stock index.|
|Cheung and<br>Coutts|2001|Cross<br>Country|Weak-form efficiency.|Confirmed weak form|
|Abeysekera|2001|India|RWH for the Calcutta Stock<br>Exchange (CSE).|Negated RWH|
|Abraham_et_<br>_al._|2002|Cross<br>Country|Variance ratio test to explore the<br>weak-form.|Confirmed weak form|
|Buguka and<br>Brorsen|2003|Turkey|RWH for stock prices.|Confirmed RWH for<br>stock prices|
|Jabbour and<br>Liu|2005|Hang Seng|MC simulation by the number of<br>simulations.|MC simulation is<br>increased by a larger<br>number of attempts in<br>simulations.|
|Hoesli_et al._|2005|Theory|Monte Carlo simulation finds its<br>application in much wider areas|MC simulation can be<br>adopted for stocks|
|Ariely_et al._|2006|USA|The judgment leads to bias and<br>errors carried over previous<br>prices.|Judgments are unreliable|



61 

Dinabandhu Bag & Saurabh Goel 

### **<u>Table 1</u>** **_<u>(continued)</u>_** 

|Source|Year|Global|Purpose|Inferences & limitations|
|---|---|---|---|---|
|Asiri|2008|Bahrain|Unit root test for RWH for BSE|Prices follow a random<br>walk, confirmed RWH for<br>stock prices|
|Whiteside|2008|Theoretical|conditional distribution in input<br>functions.|<br>MC could be used to<br>simulate conditional<br>distribution in input<br>functions.|
|Charles and<br>Darne|2009|Shanghai<br>& Shenzhen|RWH.|RWH was rejected.|
|Farid_et al._|2010|Tehran|Generated (VaR) with MC for<br>automobile stocks.|Suggested generating VaR<br>(value at risk) forecast<br>with MC|
|Erdos and<br>Ormos|2010|USA|Tested weak-form of the US<br>market.|Confirmed weak form|
|Okpara|2010|Nigerian|RWH|Confirmed random walk|
|Gupta and<br>Siddiqui|2010|Teheran|RWH and used the Kolmogorov–<br>Smirnov test (K–S) test.|did not exhibit a weak<br>form of market efficiency.|
|Alexeev and|2011|Toronto|Demonstrated the weak-form|Confirmed weak-form|
|Tapon|||efficiency.|efficiency for the TSE|
|Landauskas<br>and<br>Valakevičius|2011||Compared standard MC<br>simulation with Markov chain<br>MC simulation (MCMC).|After 300 executed<br>trajectories, the average<br>stock price after 50 trades<br>was exactly the same.|
|Khan_et al._|2011|India|Used the unit root test and the<br>GARCH model to test RWH for<br>BSE and NSE|Negated the presence of<br>RWH.|
|Khan and<br>Vieito|2012|Portuguese|Efficient market hypothesis.|Did not Infer RWH,<br>market was inefficient.|
|Pant and<br>Bishnoi|2012|India|Used unit root test,<br>autocorrelation and variance ratio<br>to NSE.|RWH was rejected.|
|Abidin and<br>Jaffar|2014|Malaysian|Implemented MC to Malaysian<br>stocks to find its acceptance.|MC is applicable to stocks|
|Kyng  and<br>Otto|2014|Europe|Multivariate normal Log- normal<br>distribution to European stock<br>options|Suggested multi-asset,<br>multi-period simulation to<br>arrive at the option price.|
|Mishra_et al._|2015|India|RWH|Rejected the presence of<br>RWH.|
|Sonono and<br>Mashele|2015|South<br>Africa|Compared MC with the VG<br>(parametric) model in stock price<br>behaviour in terms of the hit<br>ratios applied to the JSE top 40<br>index.|GBM worked better than<br>the VG (parametric)<br>model in terms of the hit<br>ratios.|
|Reddy and<br>Clinton|2016|Australia|Deployed MC for multi-period<br>price samples of large stocks.|Inferred MC as a<br>promising technique for<br>simulation of prices.|
|Zhang|2020|Asia|Adopted MC for Asian option<br>prices|<br>Derived higher accuracy<br>in a forecast of Asian<br>option prices using Monte<br>Carlo|



_Notes:_ Authors’ compilation from literature. 

62 

Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants 

However, the tests of RWH by using MC methods specific to call auction price were few. In separate studies, Timothy and Otto (2014), had implemented <mark>multivariate distribution model to European stock options to confirm their usability.</mark> Abidin and Jaffar (2014) had implemented MC to Malaysian stocks to find its acceptance. Reddy and Clinton (2016) deployed MC to Australian stocks to compare its accuracy over other methods. Boyle (1977) presented the application of the MC to option pricing, who modelled underlying stock returns as continuous and sudden processes. Jabbour and Liu (2005) found that MC simulation accuracy improved with larger number of simulations. Landauskas and Valakevičius (2011) compared standard MC simulation with Markov chain MC simulation (MCMC) on similar trades. After 300 executed trajectories, Landauskas and Valakevičius (2011) found that the average stock price of a sample of 50 trades were identical in both methods. Landauskas and Valakevičius (2011) stated that more number of intervals used in sampling led to higher forecast accuracy. Similarly, the specific tests of accepting RWH were also conducted by few other authors, namely, Charles and Darne (2009) rejected RWH for the Shanghai and Shenzhen markets. Khan and Vieito (2012) inferred that the Portuguese market was inefficient. Sonono and Mashele (2015) showed that the GBM worked better than the VG (parametric) model in stock price behaviour in terms of the hit ratios applied to JSE (South Africa). Zhang (2020) adopted MC for Asian option prices, and, derived higher accuracy in forecast of option prices using MC. Farid _et al_ . (2010) suggested generating VaR (value at risk) forecast with MC simulation for automobile stocks in Tehran. Khan _et al_ . (2011) used the unit root test and the GARCH model to test RWH for BSE and NSE in India. Gupta and Siddiqui (2010) examined the RWH for the NSE indices in India and used the Kolmogorov– Smirnov test (K–S) test. The results did not exhibit a weak form of market efficiency. Pant and Bishnoi (2012) used unit root test, autocorrelation and variance ratio tests to reject the RWH for NSE. Mishra _et al_ . (2015) conduct unit root tests on NSE indices and failed to support RWH. Siddiqui and Patil (2017) also demonstrated that the MC were suited for many Indian stock prices. 

## **3. Hypotheses** 

As mentioned above, previous findings on RWH tests using methods other than MC based were mixed in nature. Mishra et al (2015) tested RWH on NSE indices and failed to support it. Gupta and Siddiqui (2010) examined the RWH for the NSE indices in India and could not confirm the weak form of market efficiency. We therefore, proceed to test the following hypotheses to characterize the behaviour of pre-opening equilibrium prices. 

If the random walk hypothesis is rejected for pre-opening prices, it would imply that historical prices are related to current prices. 

_H1: Call auction equilibrium prices follow random walk._ 

Alternatively, if prices exhibit pure randomness, one cannot foresee prices to his/her benefit. 

_H2: The correlation between equilibrium prices today and the previous day are insignificant._ 

Alternatively, if prices exhibit pure randomness, there is no possibility of correlations among stocks either. 

_H3: The correlation between the auction prices of different stocks is insignificant._ 

63 

Dinabandhu Bag & Saurabh Goel 

If the MC method is an appropriate candidate for simulation, an appropriate distribution exists to achieve accurate forecast prices. 

_H4: Multivariate lognormal distribution could provide higher accuracy than simple univariate MC._ 

## **4. Methodology** 

The MC (Geometric Brownian Motion) with normal distribution is a case of dimension independence. It differs from numerical analysis whose accuracy could fall with more number of dimensions. The basic notion is that the future prices are conditionally independent of past prices. The common geometric Brownian motion GBM is a Markov process is given as: 



<mark>where "P" is the stock price, "μ" is the expected return, "σ" (Greek sigma) is the standard deviation of returns, "t" is Time Step, and "ε " (Greek epsilon) is the random variable.</mark> 



<mark>where, μ∆t is the drift, and σε√∆t is the shock.</mark> Price drifts up by the expected return for each period. But the drift will be shocked (added or subtracted) by a random term. The stock price follows increments where each increment is a drift plus/minus a random shock of the standard deviation (σ). The residual between the log of Prices ( <mark>ΔP</mark> t <mark>)</mark> in two consecutive periods are given as: 



The residuals “εt” represent the prediction error observed to analyze the weak form hypothesis. Subsequently, the Multivariate Monte Carlo Simulation (MVMC) is described by relaxing independence assumption, which takes into consideration the correlation between prices. Suppose that X = (X1,..., Xn) is a random vector (natural log of equilibrium prices of N stocks), then Σ, the covariance matrix of X, is the (n×n) matrix that has (i, j)<sup>th</sup> element given by: 



The Cholesky Decomposition of Covariance Matrix could reduce the Covariance Matrix to a lower triangular matrix. The Cholesky Matrix is written as: 



where, L is a lower triangular matrix and D is a diagonal matrix with positive diagonal elements. Since the variance-covariance (VACOV) matrix (Σ) is symmetric positive-definite, we can therefore write: 



where, the matrix C =√DL<sup>T</sup> satisfies: 



64 

Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants 

where C is the Cholesky Decomposition of Σ. We generate random prices by using a multivariate lognormal random vector with mean µ and variance-covariance (VACOV) matrix Σ<sup>’</sup> , which are passed as input: 



where Y:= (Y1,...,Yn) ∼ MN(µ,Σ). Since, X = exp(Y), the natural log of prices, Ys are generated. We proceed to implement the above methods to our test sample dataset. 

## **5. Data** 

The sample comprises NSE 50 Index constituent stocks, which have a long history of trading. The reason for picking NIFTY stocks is the availability of adequate historical data, greater volume of transactions, and consistency as the elements of a popular index. Due to their larger volumes, NIFTY stocks also attract greater institutional interest during the pre-market period. The period of sampling days pertained to the year 2019. The equilibrium prices were collected for consecutive 30 trading days. Each day, the data were collected only after NSE displayed the equilibrium prices after 9.15AM, which is the closing time of pre-opening call. 

The descriptive statistics of 50 NSE stocks are given in Table 2, which includes the average equilibrium prices, Std. dev. of prices, average volume of trading, average value of trading, and market capitalization, respectively. The standard deviation of prices varies from 0.82% to 3.53% in the sample. The volume of auctions during pre-market is related to the market capitalization of stocks. There is no observed relationship between standard deviation of prices and market capitalization. 

## **6. Results** 

The test results against each hypothesis are described here. We proceed with the first 500 simple MC simulation trials. Later, we conduct 500 independent trials for multi-variate log normal distribution. We calculate the t-values and compare the probability of significance for 99.99% confidence limits (95% confidence limit, tTABULATED = 1.6, and, 99.99% confidence limit tTABULATED = 3.46). 

For the correlation tests, we follow <mark>Ratner (2009) who suggests that the Pearson</mark> coefficient higher than (≥) 0.3 is numerically significant. For the first hypothesis, for example, for the ticker ACC as in Table 2, Average (εt) = -0.001; Standard Error (εt) = 0.00; tCALCULATED= **|** -0.001/0.003 **|** = 0.21. Since, tCALCULATED < tTABULATED, the null hypothesis cannot be rejected. Therefore, auction prices follow a random distribution. We continue to compute the Standard Error (εt) to infer insignificant tCALCULATED for similar other tickers named in Table 1. The results are similar to those reported by Asiri (2008) and Okpara (2010). 

We compute the correlation between each stock's residual errors (εt,εt-1) in the second hypothesis separately. As in Table 2, for the first ticker ACC, ρt = -0.31. Since ρt > 0.3, it is significant. Therefore, the null hypothesis cannot be rejected. We continue to compute the correlation tests to infer significant tCALCULATED for similar other tickers named in Table 1. The reported tests are similar to the findings of Reddy and Clinton (2016), who have also reported a negative correlation (ρ) during short periods of simulation. 

Towards the third hypotheses of correlations between any two stocks, we compute the correlations (ρi,j) between the residual errors in the sample. As in Table 2, for example, between the tickers ACC and Ambuja Cements, we find the correlation (ρi,j) equals 0.67. Since ρi,j > 0.3, it is significant. Therefore, the null hypothesis cannot be rejected. 

We continue to compute the correlation tests to infer significant tCALCULATED for similar other tickers named in Table 1. These results are in line with the findings of Schwartz and Whitcomb (1977) and Pant and Bishnoi (2012). 

65 

Dinabandhu Bag & Saurabh Goel 

**<u>Table 2:</u>** <u>Descriptive statistics</u> 

|No.|<br>Ticker|Average<br>price<br>(INR)|Std. dev.<br>of price<br>(INR)|<br>Average<br>volume<br>(No)|Average<br>value<br>(INR 0.1|Average<br>market<br>capitalization|
|---|---|---|---|---|---|---|
||||||mil.)|<br>(INR 10 mil.)|
|1|ACC CEMENT LTD|1,462.79|20.43|235.13|3.43|13,677|
|2|AMBUJA CEMENT LTD|221.18|3.10|2,020.50|4.46|16,906|
|3|ASIAN PAINTS LTD|572.21|13.51|864.63|4.91|25,796|
|4|AXIS BANK|1,887.73|37.78|1,019.13|19.14|62,290|
|5|BAJAJ AUTO LTD|2,217.08|57.53|306.00|6.93|29,615|
|6|BANK OF BARODA|855.07|16.57|1,575.13|13.43|15,980|
|7|BHARTI AIRTEL|338.84|2.77|3,413.13|11.57|40,110|
|8|BHEL ELECTRICAL LTD|245.49|5.58|9,118.63|22.28|22,089|
|9|BPCL PETROLEUM LTD|572.02|15.81|3,434.75|19.64|14,805|
|10|CAIRN ENERGY|371.29|4.12|<br>2,078.25|7.73|<br>21,771|
|11|CIPLA LTD|425.60|4.51|2,075.88|8.85|21,531|
|12|COAL INDIA LTD|391.07|6.51|5,108.88|19.90|25,518|
|13|DLF ESTATES|212.71|6.53|10,258.50|21.73|9,410|
|14|DR.REDDY’S PHARMA|2,455.15|32.36|254.63|6.20|31,135|
|15|GAIL LTD|448.32|11.11|2,279.13|10.36|20,837|
|16|GRASIM LTD|3,508.21|53.58|70.50|2.48|22,281|
|17|HCLTECH|1,426.69|18.19|672.63|9.61|38,473|
|18|HDFC LTD|979.43|13.94|1,566.13|15.52|153,225|
|19|HDFC BANK|827.91|8.73|2,206.50|18.19|153,052|
|20|HEROMOTO CORP|2,606.00|70.36|6,516.38|176.97|31,026|
|21|HINDALCO|166.69|1.85|5,565.88|9.30|21,305|
|22|HINDUSTAN UNILEVER|622.26|5.50|717.25|4.48|44,130|
|23|ICICI BANK LTD|1,425.09|19.47|2,299.75|32.75|163,445|
|24|IDFC LTD|129.02|1.41|10,658.88|13.70|16,112|
|25|INDUSIND BANK|563.00|11.42|537.63|3.04|24,407|
|26|INFOSYS LTD|3,243.67|45.83|1,604.00|52.07|<br>156,655|
|27|ITC LTD|<br>326.16|10.40|<br>20,984.13|67.04|<br>180,026|
|28|JINDAL STEEL LTD|326.20|7.16|<br>1,643.25|5.36|<br>11,703|
|29|KOTAK BANK|911.73|30.28|<br>1,059.75|9.60|<br>34,695|
|30|L & T LTD|1,675.26|21.81|2,385.13|40.20|135,366|
|31|LUPIN PHARMA|1,011.56|13.26|326.63|3.30|24,070|
|32|M &M LTD|1,177.71|25.65|1,661.38|19.60|54,110|
|33|MARUTI LTD|2,431.41|48.38|776.63|19.01|32,047|
|34|MCDOWELLS BEVERAGES|2,650.45|93.69|626.88|16.19|20,607|
|35|NMDC MINERALS LTD|178.96|4.36|2,552.25|4.52|14,091|
|36|NTPC POWER LTD|154.34|3.33|6,742.13|10.42|31,598|
|37|ONGC GAS LTD|428.65|12.16|24,910.38|106.29|76,760|
|38|PNB BANK|967.57|10.85|518.75|5.02|14,352|
|39|POWERGRID CORP OF INDIA|<br>133.28|1.78|5,026.38|6.69|29,209|
|40|RELIANCE INDUSTRIES|104936|2828|1093663|11333|171092|
|<br>41|<br>SBI BANK OF INDIA|,.<br>2,645.71|.<br>45.73|,.<br>1,502.38|.<br>39.73|,<br>81,454|
|42|SS SESA STERLITE LIMITED|295.37|2.93|6,065.00|17.93|36,431|
|43|SUNPHARMA LTD|634.98|7.83|3,859.50|24.94|47,671|
|44|TATAMOTORS LTD|440.10|4.94|<br>6,225.00|420.28|<br>78,855|
|45|TATAPOWER LTD|104.21|1.98|<br>5,527.25|5.74|<br>18,782|
|46|TATASTEEL LTD|529.34|9.19|<br>4,938.38|26.15|<br>35,010|
|47|TCS SOFTWARE LTD|2,279.74|31.85|655.00|14.94|117,556|
|48<br>|TECH MAHNIDRA LTD<br>|2,012.53<br>|18.45<br>|1,524.50<br>|30.69<br>|30,042<br>|
|49|ULTRA CEMENT CO|2,738.54|36.64|95.88|2.64|27,990|
|50|WIPRO|533.87|4.38|917.00|4.90|34,884|



_Notes:_ National Stock Exchange of India (2019) 

In continuation, using simple MC, Table 3 produces the summary statistics of simple univariate MC simulation on the sample of 50 stocks. 

66 

Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants 

**<u>Table 3:</u>** <u>Univariate log-normal MC simulation of 100 trials</u> 

|No.|Stock Name|Mean|Median|Std<br>|Co-<br>|Min|Max|
|---|---|---|---|---|---|---|---|
|||||dev|efficient<br>of<br>variation|||
|1|ACC CEMENT LTD|1469.75|1467.32|6.75|0.46%|1459.53|1482.76|
|2|AMBUJA CEMENT LTD|226.91|227.08|0.63|0.28%|225.58|227.84|
|3|ASIAN PAINTS LTD|551.61|551.32|1.40|0.25%|549.40|555.35|
|4|AXIS BANK|1879.19|1882.85|13.94|0.74%|1853.48|1898.72|
|5|BAJAJ AUTO LTD|2133.31|2141.20|17.65|0.83%|1758.70|2608.91|
|6|BANK OF BARODA|869.60|869.35|3.99|0.46%|861.92|877.28|
|7|BHARTI AIRTEL|340.83|340.94|1.23|0.36%|337.94|342.92|
|8|BHEL ELECTRICAL LTD|245.98|246.44|1.39|0.57%|242.51|247.99|
|9|BPCL PETROLEUM LTD|596.68|595.38|3.65|0.61%|591.25|606.02|
|10|CAIRN ENERGY|377.08|376.78|1.07|0.28%|375.48|379.29|
|11|CIPLA LTD|418.36|418.37|1.15|0.27%|414.99|420.24|
|12|COAL INDIA LTD|394.31|393.60|2.33|0.59%|390.28|398.69|
|13|DLF ESTATES|229.33|229.33|1.64|0.72%|225.26|233.13|
|14|DR. REDDY’S PHARMA|2431.52|2432.36|3.59|0.15%|2422.99|2436.96|
|15|GAIL LTD|438.20|439.35|6.24|1.42%|426.62|447.57|
|16|GRASIM LTD|3600.88|3604.80|7.66|0.21%|3580.02|3611.85|
|17|HCLTECH|1401.14|1401.39|3.46|0.25%|1393.34|1408.12|
|18|HDFC LTD|979.38|978.82|3.36|0.34%|973.17|986.80|
|19|HDFC BANK|842.44|842.49|0.82|0.10%|840.82|844.79|
|20|HEROMOTO CORP|2728.85|2726.42|22.19|0.81%|2680.64|2766.53|
|21|HINDALCO|167.40|167.12|0.96|0.57%|165.80|169.57|
|22|HINDUSTAN UNILEVER|629.61|629.54|1.52|0.24%|626.79|632.80|
|23|ICICI BANK LTD|1456.50|1457.12|5.96|0.41%|1445.34|1466.71|
|24|IDFC LTD|131.77|131.76|0.25|0.19%|131.25|132.37|
|25|INDUSIND BANK|553.34|553.75|1.84|0.33%|549.20|556.79|
|26|INFOSYS LTD|3193.93|3193.28|9.51|0.30%|3174.75|3216.85|
|27|ITC LTD|337.70|337.54|0.92|0.27%|335.66|340.15|
|28|JINDAL STEEL LTD|342.07|342.95|3.12|0.91%|335.59|348.04|
|29|KOTAK BANK|929.35|929.24|4.96|0.53%|918.91|938.95|
|30|L & T LTD|1722.42|1722.49|8.11|0.47%|1700.97|1737.62|
|31|LUPIN PHARMA|987.87|987.90|2.26|0.23%|983.38|992.32|
|32|M &M LTD|1230.49|1230.30|3.11|0.25%|1223.93|1238.17|
|33|MARUTI LTD|2515.63|2511.06|17.68|0.70%|2482.82|2546.98|
|34|MCDOWELLS BEVERAGES|2779.87|2780.90|29.57|1.06%|2721.64|2844.93|
|35|NMDC MINERALS LTD|188.46|188.42|1.61|0.85%|185.59|191.60|
|36|NTPC POWER LTD|161.42|161.32|0.43|0.27%|160.61|162.45|
|37|ONGC GAS LTD|439.97|441.58|4.60|1.05%|428.53|445.70|
|38|PNB BANK|979.77|979.87|1.42|0.14%|976.65|982.04|
|39|POWERGRID CORP OF INDIA|137.93|138.03|0.49|0.36%|137.00|138.85|
|40|RELIANCE INDUSTRIES|1091.82|1091.48|6.42|0.59%|1081.65|1102.26|
|41|SBI BANK OF INDIA|2696.16|2697.89|11.29|0.42%|2674.44|2713.64|
|<br>42|<br>SS SESA STERLITE LIMITED|<br>295.88|<br>295.48|<br>1.55|<br>0.52%|<br>293.25|<br>299.22|
|43|SUNPHARMA LTD|630.34|630.18|1.35|0.21%|627.65|633.27|
|44|TATAMOTORS LTD|448.33|448.28|1.14|0.25%|445.83|450.65|
|<br>45|<br>TATAPOWER LTD|<br>107.21|<br>107.04|<br>0.55|<br>0.51%|<br>106.45|<br>108.35|
|46|TATASTEEL LTD|543.11|543.21|5.41|1.00%|532.36|550.62|
|47|TCS SOFTWARE LTD|2246.14|2244.44|6.14|0.27%|2235.29|2260.83|
|48|TECH MAHNIDRA LTD|1985.22|1985.87|5.14|0.26%|1974.18|1992.67|
|49|ULTRA CEMENT CO LTD|277571|277664|728|026%|275585|278722|
|<br>50|<br>WIPROLTD|.<br>530.79|.<br>530.96|.<br>1.41|.<br>0.27%|.<br>527.64|.<br>533.33|



_Notes:_ National Stock Exchange of India (2019) 

Table 3 shows the higher deviation between historical volatility and estimates, displaying the nature of a karyolitic curve. As shown in Table 2, the standard deviation of prices varied from 0.82% to 3.53%. However, in simulated output, the standard deviation estimates vary 

67 

Dinabandhu Bag & Saurabh Goel 

from 0.1% to 1.42%, which have fallen, by about 3/4<sup>th</sup> of the actual variance. This implies univariate MC simulation cannot be accepted for field use and prediction. Therefore, simple MC generates prices which are not close to the actual. 

In continuation, towards the fourth hypothesis Table 4 shows the summary statistics of multivariate MC simulation on the sample of 50 stocks. 

The standard deviation varies between 0.68% to 3.33%, which is closer to the historical variation between 0.82% to 3.53%, in descriptive statistics in Table 2. Therefore, we cannot reject the null hypothesis which is the fourth hypothesis. This is in consonance with the findings of Milevsky and Posner (1998). In contrast Clewlow and Strickland (1998), and Hull and White (2012) had mentioned that MC generated higher sample variances and unsuitable, we found multi-variate that reduces the variance. 

Overall, the findings are consistent with the weak form of the EMH. The implications of these findings are important to portfolio managers and investors. Since the simulated prices vary within reasonable limit, they can be used as forecast prices. The prediction may result in knowledge to benefit the investors. The novelty of this research stems from the lack of an exact current method that either use semi-parametric or time series techniques to price behaviour. The MC method can potentially become an effective tool to predict and model stock prices. MC simulation has advantages over time series based ARIMA models (Abidin and Jaffar, 2012). Although it is often highlighted that when the number of variables are many in the dataset, PDE (Partial Differential Equations), numerical techniques (or finite difference methods) are less practical. Hence, the Monte-Carlo method provides an effective approach for complex situations such as multi-dimensionality. 

## **7. Conclusions** 

This paper investigated the mechanism of price discovery in pre-opening call auctions. The pre-opening auction is an extremely short duration auction where traders are interested in a limited number of large-cap stocks. This study examined weak-form efficiency for companies listed in NSE50. By generating a 50 X 50 covariance matrix of prices and solving for Cholesky roots, the results were compared with lognormal multivariate MC simulation to explore the estimates of volatility. We found that the stock prices were correlated among themselves, which infers the weak form of efficiency for pre-market auction prices. 

<mark>The difference in the results of this study lies in the unique approach of choosing multivariate log normal distribution to produce a lower estimated variance. The weak-form efficiency is observed using MC evaluation. Monte Carlo method (MC) was not used abundantly in field applications. The prediction may result in knowledge to benefit the investors. This method is more relevant today because of the uncertainty during the Covid period.</mark> 

There are few limitations in the use of Multivariate lognormal distribution. MC model rely on the assumption of normality of returns. <mark>The other limitation is it does not provide comparative analysis against other predictive methods like Martingale. Since t</mark> he long-run estimates of variances may not remain stable, MC is only suitable for short-run price forecasts. Further works must conduct add more relevant economic factors to improve the performance of MC simulations. Further research may explore innovative variants of Monte Carlo methods to be applied to stock prices, or indices. Lastly, portfolio-level tests of pre-market auction prices, and long-run historical simulation would be beneficial to develop robust trade strategies. 

68 

Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants 

**<u>Table 4:</u>** <u>Multivariate log-normal MC simulation</u> 

|No.|Stock Name|Mean|Median|Std.<br>|Coefficient<br>|Min|Max|
|---|---|---|---|---|---|---|---|
|||||Dev.|of<br>variation|||
|1|ACC CEMENT LTD|1468.76|1470.41|20.84|1.42%|1426.00|1515.35|
|2|AMBUJA CEMENT LTD|224.63|224.61|3.37|1.50%|216.22|231.92|
|3|ASIAN PAINTS LTD|553.97|553.16|13.64|2.46%|522.06|584.17|
|4|AXIS BANK|1903.28|1900.67|34.19|1.80%|1827.28|1984.57|
|5|BAJAJ AUTO LTD|2208.90|2206.94|51.96|2.35%|2059.39|2337.87|
|6|BANK OFBARODA|871.74|871.61|17.71|2.03%|832.16|914.83|
|7|BHARTI AIRTEL|342.23|341.90|3.03|0.89%|336.82|350.96|
|8|BHEL ELECTRICAL LTD|245.85|245.69|4.88|1.98%|235.72|257.47|
|9|BPCL PETROLEUM LTD|595.58|593.84|15.89|2.67%|562.97|630.90|
|10|CAIRN ENERGY|374.43|374.94|4.31|1.15%|364.30|383.93|
|11|CIPLA LTD|415.75|415.98|4.49|1.08%|401.73|423.01|
|12|COAL INDIA LTD|397.21|396.98|7.02|1.77%|381.21|414.95|
|13|DLF ESTATES|222.22|222.30|6.61|2.97%|205.61|238.33|
|14|DR.REDDY’S PHARMA|2441.69|2440.68|35.85|1.47%|2359.94|2520.39|
|15|GAIL LTD|435.56|436.33|10.34|2.37%|415.55|461.23|
|16|GRASIM LTD|3558.14|3554.09|53.48|1.50%|3430.47|3685.17|
|17|HCLTECH|1412.63|1415.42|16.68|1.18%|1368.06|1443.11|
|18|HDFC LTD|979.86|980.37|14.86|1.52%|928.37|1023.40|
|19|HDFC BANK|839.46|838.86|9.00|1.07%|819.48|861.26|
|20|HEROMOTO CORP|2707.60|2704.30|68.66|2.54%|2563.34|2874.28|
|21|HINDALCO|168.54|168.73|1.93|1.15%|163.61|173.08|
|22|HINDUSTAN UNILEVER|628.82|628.59|6.03|0.96%|614.78|644.07|
|23|ICICI BANK LTD|1452.15|1455.39|19.40|1.34%|1402.28|1498.70|
|24|IDFC LTD|130.81|130.82|1.40|1.07%|127.55|133.99|
|25|INDUSIND BANK|558.32|557.64|11.51|2.06%|534.11|593.52|
|26|INFOSYS LTD|3193.89|3196.99|41.74|1.31%|3097.05|3281.43|
|27|ITC LTD|334.08|334.43|10.67|3.19%|305.80|364.26|
|28|JINDAL STEEL LTD|334.95|334.91|6.87|2.05%|321.03|350.38|
|29|KOTAK BANK|926.97|930.32|30.90|3.33%|846.97|1019.43|
|30|L & T LTD|1719.53|1720.30|19.79|1.15%|1676.22|1766.13|
|31|LUPIN PHARMA|995.99|994.61|10.78|1.08%|973.10|1022.31|
|32|M &M LTD|1225.90|1224.77|27.78|2.27%|1149.53|1307.68|
|33|MARUTI LTD|2480.85|2483.25|46.07|1.86%|2379.03|2575.97|
|34|MCDOWELLS BEVERAGES|2732.75|2728.40|87.37|3.20%|2509.99|2953.02|
|35|NMDC MINERALS LTD|185.46|185.56|4.19|2.26%|174.86|195.43|
|36|NTPC POWER LTD|159.61|160.18|3.20|2.00%|151.78|166.50|
|37|ONGC GAS LTD|430.00|430.22|9.69|2.25%|408.06|455.38|
|38|PNB BANK|978.53|978.61|12.05|1.23%|950.15|1010.41|
|39|POWERGRID CORP OF INDIA|136.48|136.43|1.61|1.18%|132.66|140.03|
|40|RELIANCE INDUSTRIES|1088.85|1086.04|28.34|2.60%|1027.22|1178.81|
|41|SBI BANK OF INDIA|2699.76|2698.52|45.65|1.69%|2571.39|2843.33|
|42|SESA STERLITE LIMITED|293.04|292.82|2.53|0.86%|287.69|301.28|
|43|SUNPHARMA LTD|634.31|634.05|6.77|1.07%|612.28|648.44|
|44|TATAMOTORS LTD|447.83|448.35|4.64|1.04%|435.08|456.17|
|45|TATAPOWER LTD|107.53|107.59|2.14|1.99%|101.48|112.53|
|46|TATASTEEL LTD|545.38|545.88|8.95|1.64%|522.73|565.68|
|47|TCS SOFTWARE LTD|2246.90|2248.49|28.59|1.27%|2167.87|2316.08|
|48|TECH MAHNIDRA LTD|1999.87|1999.21|14.58|0.73%|1969.43|2046.53|
|49|ULTRA CEMENT CO LTD|2752.81|2756.75|36.35|1.32%|2654.09|2855.53|
|50|WIPROLTD|532.82|532.85|3.63|0.68%|524.64|544.58|



_Notes:_ National Stock Exchange of India (2019) 

69 

Dinabandhu Bag & Saurabh Goel 

## **References** 

Abeysekera, S. P. (2001). Efficient markets hypothesis and the emerging capital market in Sri Lanka: 

Evidence from the Colombo Stock Exchange–A note. _Journal of Business Finance & Accounting, 28_ (1‐2), 249–261. 

Abidin, S. N. Z., & Jaffar, M. M. (2012). A review on Geometric Brownian Motion in forecasting the share prices in Bursa Malaysia. _World Applied Sciences Journal_ , _17_ , 87–93. 

<mark>Abraham, A., Seyyed, F. J., & Alsakran, S. A. (2002). Testing the random walk behavior and efficiency of the Gulf stock markets.</mark> _<mark>Financial Review</mark>_ <mark>,</mark> _<mark>37</mark>_ <mark>(3), 469</mark> – <mark>480.</mark> 

<mark>Alexeev, V. & Tapon, F. (2011). Testing weak form efficiency on the Toronto Stock Exchange.</mark> _<mark>Journal of Empirical Finance, 18</mark>_ <mark>(4), 661-691.</mark> 

<mark>Ariely, D., Loewenstein, G., & Prelec, D. (2006). Tom Sawyer and the construction of value.</mark> _<mark>Journal of Economic Behavior and Organization</mark>_ <mark>,</mark> _<mark>60</mark>_ <mark>(1), 1-10.</mark> 

Asiri, B. (2008). Testing weak-form efficiency in the Bahrain stock market. _International Journal of Emerging Markets_ , _3_ (1), 38-53. 

Boyle, P. P. (1977). Options: A Monte Carlo approach. _Journal of Financial Economics_ , _4(3)_ , 323–338. Buguk, C. & Brorsen, B. W. (2003). Testing weak-form market efficiency: Evidence from the Istanbul 

Stock Exchange. _International Review of Financial Analysis_ , 12(5), 579–590. 

Camerer, C. (1990). Do markets correct biases in probability judgment? Evidence from market 

experiments. In L. Green & J. Kagel (Eds.), _Advances in Behavioral Economics_ (Vol. 2, pp. 125– 172). Ablex Publishing. 

<mark>Chang, K. P., & Ting, K. S. (2000). A variance ratio test of the random walk hypothesis for Taiwan's stock market.</mark> _<mark>Applied Financial Economics</mark>_ <mark>,</mark> _<mark>10</mark>_ <mark>(5), 525–532.</mark> 

Charles, A. & Darné, O. (2009). The random walk hypothesis for Chinese stock markets: Evidence from variance ratio tests. _Economic Systems, 33_ (2), 117–126. 

Cheung, K. C., & Coutts, J. A. (2001). A note on weak form market efficiency in security prices: 

Evidence from the Hong Kong stock exchange. _Applied Economics Letters, 8_ (6), 407–410. 

Clewlow, L., & Strickland, C. (1998). _Implementing derivatives models_ . John Wiley & Son Ltd. 

Erdos, P., & Ormos, M. (2010). Random walk theory and the weak-form efficiency of the US art auction 

prices. _Journal of Banking and Finance, 34_ (5), 1062–1076. 

Fama, E. F. (1991). Efficient capital markets: II. _The Journal of Finance_ _<mark>, 46</mark>_ <mark>(5), 1575–1617.</mark> 

<mark>Farid, D., Rajabipoor M. A., & Heydar M. S. (2010). Investment risk management in Tehran Stock</mark> 

- <mark>Exchange (TSE) using technique of Monte Carlo Simulation (MCS).</mark> _<mark>J</mark> ournal of Financial Crime_ , _<mark>17</mark>_ <mark>(2), 265-278.</mark> 

Gupta, P. K., & Siddiqui S. (2010, October). _Weak form of market efficiency: Evidences from selected NSE indices_ (SSRN Working Paper No. 1355103).  http://ssrn.com/abstract=1355103 

Hoesli, M., Jani, E., & Bender, A. (2005, June). _Monte Carlo simulations for real estate valuation_ (FAME Research Paper N148). https://core.ac.uk/download/pdf/7183128.pdf 

<mark>Hull, J., & White, A. (2012). CVA and wrong way risk.</mark> _<mark>Financial Analysts Journal, 68</mark>_ <mark>(5), 58–69.</mark> 

Jabbour, G. M., & Liu Y. (2005). Option pricing and Monte Carlo simulations. _Journal of Business and Economics Research_ , _3_ (9), 1–6. 

Khan, A. Q., Ikram, S., & Mehtab, M. (2011). Testing weak form market efficiency of Indian capital market: A case of National Stock Exchange (NSE) and Bombay Stock Exchange (BSE). _African Journal of Marketing Management_ , _3_ (6), 115–127. 

- Khan, W., & Vieito, J. P. (2012). Stock exchange mergers and weak form of market efficiency: The case of Euronext Lisbon. _International Review of Economics and Finance_ , _22_ (1), 173-189. 

Kyng, T. J., & Otto, K. (2014). Multivariate Monte-Carlo simulation and economic valuation of complex financial contracts: An excel based implementation. _Spreadsheets in Education, 7_ (2), 1- 38. 

- Landauskas, M., & Valakevičius, E. (2011). Modelling of stock prices by the Markov Chain Montecarlo method. _Intellectual Economics, 5_ (20), 244–256. 

- Lo, A. W., & MacKinlay, A. C. (1998). Stock market prices do not follow random walks: Evidence from a simple specification test. _The Review of Financial Studies_ , _1_ (1), 41–66. 

- Malkiel, B. G. (2003). The efficient market hypothesis and its critics. _Journal of Economic Perspectives_ , _17_ (1), 59–82. 

- Milevsky, M. A., & Posner, S. E. (1998). Asian options, the sum of lognormals, and the reciprocal gamma distribution. _Journal of Financial and Quantitative Analysis, 33_ (3), 409–422. 

70 

Weak Form of Call Auction Prices: Simulation Using Monte Carlo Variants 

Mishra, A., Mishra, V. & Smyth, R. (2015). The random-walk hypothesis on the Indian stock market. _Emerging Markets Finance and Trade_ , _51_ (5), 879–892. 

National Stock Exchange of India (2019). NSE Data Archives. Retrieved December 31, 2019 from https:www.nseindia.com 

Okpara, G. C. (2010). Analysis of weak-form efficiency on the Nigerian stock market: Further evidence 

from GARCH Model. _The International Journal of Applied Economics and Finance_ , _4_ (2), 62–66. Pant B., & Bishnoi T. R. (2012). Testing random walk hypothesis for Indian stock market indices. http://utiicm.com/Cmc/PDFs/2002/bhanu_pant.pdf 

Ratner, B. (2009). The correlation coefficient: Its values range between +1/−1, or do they? _Journal of Targeting, Measurement and Analysis for Marketing_ , _17_ , 139–142. 

Reddy, K., & Clinton, V. (2016). Simulating stock prices using geometric brownian motion: Evidence 

from Australian companies. _Australasian Accounting, Business and Finance Journal_ , _10_ (3), 23–47. Schwartz, R. A., & Whitcomb, D. K. (1977). Evidence on the presence and causes of serial correlation 

in market model residuals. _Journal of Financial and Quantitative Analysis_ , _12_ (2), 291–313. 

Shiller, R. J. (1981). Do stock prices move too much to be justified by subsequent changes in dividends? _The American Economic Review_ , _71_ (3), 421–436. 

<mark>Siddiqui, S. S., & Patil, V. A. (2017). Proposed system for estimating intrinsic value of stock using Monte Carlo simulation.</mark> _<mark>International Conference on Intelligent Computing and Control Systems</mark>_ <mark>, 723</mark> – <mark>729.</mark> 

<mark>Sonono, M. E., & Mashele H. P. (2015). Prediction of stock price movement using continuous time models.</mark> _<mark>Journal of Mathematical Finance, 5</mark>_ <mark>(2), 178–191.</mark> 

- Tversky, A., & Kahneman, D. (1974). Judgment under uncertainty: Heuristics and biases, _Science, 185_ (4157), 1124–1131. 

- <mark>Whiteside, J. D. (2008). A practical application of Monte Carlo simulation in forecasting.</mark> _<mark>AACE International Transactions</mark>_ <mark>, EST.04.</mark> 

- <mark>Zhang Y. (2020). The value of Monte Carlo model-based variance reduction technology in the pricing of financial derivatives.</mark> _<mark>PLoS ONE</mark>_ <mark>15(2): e0229737.</mark> 

71 

