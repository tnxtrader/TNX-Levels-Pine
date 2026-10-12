



Luo, Jiawen; Klein, Tony; Ji, Qiang; Hou, Chenghan 

**Working Paper** Forecasting Realized Volatility of Agricultural Commodity Futures with Infinite Hidden Markov HAR Models 

QMS Research Paper, No. 2019/10 

**Provided in Cooperation with:** Queen's University Belfast, Queen's Business School 

_Suggested Citation:_ Luo, Jiawen; Klein, Tony; Ji, Qiang; Hou, Chenghan (2019) : Forecasting Realized Volatility of Agricultural Commodity Futures with Infinite Hidden Markov HAR Models, QMS Research Paper, No. 2019/10, Queen's University Belfast, Queen's Management School, Belfast, https://doi.org/10.2139/ssrn.3435054 

This Version is available at: https://hdl.handle.net/10419/271230 

**Standard-Nutzungsbedingungen:** 

Die Dokumente auf EconStor dürfen zu eigenen wissenschaftlichen Zwecken und zum Privatgebrauch gespeichert und kopiert werden. 

Sie dürfen die Dokumente nicht für öffentliche oder kommerzielle Zwecke vervielfältigen, öffentlich ausstellen, öffentlich zugänglich machen, vertreiben oder anderweitig nutzen. 

Sofern die Verfasser die Dokumente unter Open-Content-Lizenzen (insbesondere CC-Lizenzen) zur Verfügung gestellt haben sollten, gelten abweichend von diesen Nutzungsbedingungen die in der dort genannten Lizenz gewährten Nutzungsrechte. 

### **Terms of use:** 

_Documents in EconStor may be saved and copied for your personal and scholarly purposes._ 

_You are not to copy documents for public or commercial purposes, to exhibit the documents publicly, to make them publicly available on the internet, or to distribute or otherwise use the documents in public._ 

_If the documents have been made available under an Open Content Licence (especially Creative Commons Licences), you may exercise further usage rights as specified in the indicated licence._ 



















































# Forecasting Realized Volatility of Agricultural Commodity Futures with Infinite Hidden Markov HAR Models<sup>$</sup> 

Jiawen Luo<sup>a</sup> , Tony Klein<sup>b</sup> , Qiang Ji<sup>c,</sup><sup>_∗_</sup> , Chenghan Hou<sup>d</sup> 

> _aSouth China University of Technology, Guangzhou, P. R. China_ 

> _bQueen’s Management School, Queen’s University Belfast, United Kingdom_ 

> _cChinese Academy of Sciences, Beijing, P. R. China_ 

> _dHunan University, Changsha, P. R. China_ 

## **Abstract** 

We construct a set of HAR models with three types of infinite Hidden Markov regime switching structures. Particularly, jumps, leverage effects, and speculation effects are taken into account in realized volatility modeling. We forecast five agricultural commodity futures (Corn, Cotton, Indica Rice, Palm oil and Soybean) based on high frequency data from Chinese futures markets and evaluate the forecast performances with both statistical and economic evaluation measures. The statistical evaluation results suggest that HAR models with infinite Hidden Markov regime switching structures have better precision compared the benchmark HAR models based on the MZ- _R_<sup>2</sup> , MAFE, and MCS results. The economic evaluation results suggest that portfolios constructed with infinite Hidden Markov regime switching HARs achieve higher portfolio returns for risk averse investors compared to benchmark HAR model for short-term volatility forecasts. 

_Keywords:_ Agriculture commodity futures; Realized volatility forecasts; Infinite Hidden Markov switching process; HAR models; _JEL classification: C53; G17; Q14_ 

## **1. Introduction & Literature review** 

Agricultural commodities, such as Corn, Wheat, Rice, and Soybean, are farming products and have been existing for centuries. For the same time, these commodities have been subject to trade and exchange. Only recently, those commodities are being traded centralized and in globalized markets. Volatility of agricultural commodity prices and sudden, unforeseen changes play an important role as they directly affect our daily lives. From a financialized perspective, forecasting volatility of agriculture commodity futures helps to assess and hedge risks associated with these contracts as well as to provide policy makers with tools to evaluate different scenarios. 

> $We thank the editor Tao Hong and three anonymous referees for insightful comments and hints which led to a substantial improvement of the manuscript. 

> _∗_ Corresponding author, mail: jqwxnjq@casipm.ac.cn. 

_August 9, 2019_ 

_Preprint submitted to International Journal of Forecasting_ 

Nerlove (1956) presents some early stage research on realized and expected prices as well as their elasticities of Cotton, Wheat, and Corn between 1909 and 1932. It is found that the response of supply to prices and price changes is only partly based on historical prices. Price expectation plays significant and vital role in supply management. This is of particular interest as it is early evidence that expected prices of future crop yields—to some degree similar to a futures price—are directly incorporated in crop management. Research on agricultural commodities is otherwise sparse but is being addressed with an increasing degree of financialization of these markets. This is outlined by Witt et al. (1987) who address cross-hedging applications and optimal hedge ratios. Krueger et al. (1988) analyzes the impact of trade policies on different categories of agricultural commodities and their effect on price stabilization for importable and exportable commodities in the U.S. Other countries have functioning derivative markets as well. The efficiency of commodity futures markets in the U.K. are analyzed with co-integration methods in Aulton et al. (1997) for example, where the Wheat market found to be efficiently functioning while Potatoes are not. 

Regulation of agricultural commodities is a delicate topic in general as the aim for profits might clash with vital food supply. Those contrary positions become apparent in the food crisis of 2006 which is linked to the sharp rise of biofuels, in particular the first generation, which are fuels derived from crops grown on arable land. This directly impacts food security as less land and less crops are available for a growing population. With regard to this socio-economic impact, Escobar et al. (2009) call for regulation of biofuels to overcome the risks for food security. Schmidhuber (2007) finds evidence that policy changes with respect to biofuels should lead to a closer market integration between energy and agricultural commodities. Tyner (2010) finds a strong link between Ethanol and Corn prices after the financial crisis while before the crisis, Corn and crude oil linked significantly. Nazlioglu et al. (2013) find spillover effects between crude oil and major agricultural commodities (Wheat, Corn, Soybeans, and Sugar). A structural break is found in their relationship after the food crisis in 2006. Crude oil is now a major contributor to the volatility of agricultural commodities. This is also found for an earlier period in Natanelov et al. (2011) in a co-movement relationship of agricultural futures prices and crude oil. Policy developments are affecting these relationships; for low crude oil futures prices, biofuel policies buffer co-movements with Corn futures prices until the oil futures surpass a price threshold. Du et al. (2011) address spillover and linkage effects of crude oil markets to Corn and Wheat futures. After 2006, the spillover induced by dependence of these markets with ethanol production increases. 

Forecasting volatility of commodity prices and their spillovers is of essential importance in order to identify and understand what factors drive different commodity markets (Nguyen & Walther, 2019). Also, co-movements play a vital role as outlined above. In recent literature, forecasting volatility has gained interest, oftentimes from a risk- 

2 

management perspective (Klein & Walther, 2016, Ji & Fan, 2016) or in terms of quantifying spillovers between markets (Nazlioglu et al., 2013, Todorova et al., 2014, Ji et al., 2018, Klein, 2018, Ma et al., 2019). 

Having identified food crises and crude oil as a major contributor to the volatility of agricultural commodities, we expect highly volatile markets. By making use of highfrequency data at the 5-minute interval, we put particular focus on breakpoints and structural changes as well as sudden jumps in these volatilities. We model realized volatilities with the heterogeneous autoregressive (HAR) model of Corsi (2009). This HAR framework allows to model short, medium, and long-term dependencies among realized volatilities. Given its simplicity in application, the standard HAR gives rise to a plethora of model extensions covering different stylized facts. In this vein, we extend the standard HAR for jumps (Andersen et al., 2007), with jump thresholds (Corsi et al., 2010), or with signed jumps (Patton & Sheppard, 2015). Additionally, we make use of speculative behaviour and include volume and open interest as a predictor of future volatility (Bessembinder & Seguin, 1992). Accounting for recent advances, we also apply the centralized HAR of Bollerslev et al. (2018), where the long-run volatility is used to centralize volatilities. 

The application of HAR-type models on agricultural commodities to forecast volatility is much less focused on than on other commodities such as crude oil (Souˇcek & Todorova, 2013, S´evi, 2014, Wen et al., 2016, Degiannakis & Filis, 2017), precious metals (Caporin et al., 2015, Ly´ocsa & Moln´ar, 2016) or non-ferrous metals (Todorova et al., 2014, Zhu et al., 2017, Ly´ocsa et al., 2017). Forecasting realized volatilities with HAR models over longer time horizons becomes problematic if structural breaks are present in the observed data (Raggi & Bordignon, 2012, Yang & Chen, 2014). Ma et al. (2018a) and Ma et al. (2018b) apply regime switching HAR models on oil futures markets and find that regime switching increases the forecasting ability of HAR models. For oil futures, Ma et al. (2017) find that HAR models incorporating jumps yield better forecasting performance along with improvements of portfolio returns, where portfolio weights are calculated based on forecasting volatilities. Tian et al. (2016) use high-frequency data of Chinese agricultural commodity markets and find evidence for long memory and regime switching in form of a high and low volatility regime. Regime switching approaches yield better forecasting performances than their non-switching equivalent. Yang et al. (2017) employ so-called bagging and combination approaches to HAR models on Chinese agricultural commodity markets. It is again found that more sophisticated approaches surpass the forecasting quality of the base HAR models. Luo & Ji (2018) combine HAR-type models with dynamic correlation modelling (DCC, Engle, 2002) and detect a spillover from U.S. crude markets to Chinese agriculture commodities. These findings support earlier evidence of shortterm spillovers in Jiang et al. (2017). We focus on those detected structural breaks and make use of infinite Hidden Markov switching (Hou, 2017). We compare in-sample 

3 

performance as well as forecasting ability of our base and switching HAR models for five agricultural commodities; Corn, Cotton, Indica Rice, Palm Oil, and Soybeans. In this manuscript, we employ the _infinite Hidden Markov switching_ model (IHM) based on hierarchical Dirichlet processes introduced in Teh et al. (2006), the _double infinite Hidden Markov_ Model (DIHM), and the IHM variant with constant conditional mean (IHMC). These three Markov-type frameworks are defined and described thoroughly in Hou (2017) and references therein. Volatility forecasts are carried out for aggregated one, five, and 22-day ahead forecasts, corresponding to daily, weekly, and monthly volatility. We test all 24 model variants<sup>1</sup> for intra-day prices of five agricultural commodities to gain a broad understanding of what drives these futures markets. This novel—and so far unique— approach of incorporating IHM variants reveals that regime switching Markov models offer an advantage in forecasting over the non-switching variants of the HAR class. 

We contribute to the literature in the following aspects. Firstly, we focus on forecasting the realized volatilities of agricultural commodity futures in Chinese futures market for different time horizons (one, five, and 22-day ahead forecasts), which sheds some light on risk management, portfolio allocation, and pricing strategies for market participants in Chinese agricultural futures market. Secondly, we construct a set of volatility forecast models with structural breaks by combining three types of infinite Hidden Markov switching frameworks with conventional HAR models. Therefore, we can accommodate unknown breakpoints in volatilities due to uncertain factors such as policy changes, financial market crises, or investor sentiment in Chinese futures markets. We employ the precision-based algorithm of Chan & Jeliazkov (2009) which can simulate the parameters in distinct regimes efficiently and improve the computing speed greatly. The application of infinite Hidden Markov processes in modeling volatilities of agricultural commodities is our most important contribution as we are the first to thoroughly test this class of models in these markets. Finally, we employ both statistical and economic evaluation measures to evaluate the forecast precision as well as the economic significance in portfolio allocations for different volatility forecast models. Thus, we can provide a comprehensive scope for the comparisons of the volatility forecast models. 

The remainder of this article is structured as follows. Section 2 presents the methodology and modelling framework. Section 3 gives an overview of the data and presents results of preliminary analyses and tests of daily returns, realized volatilities, and the speculative components for the five commodity futures markets focused on in this article. Section 4 discusses the results of the in-sample analysis as well as the the performance of forecasting over different horizons. Section 5 provides robustness checks of the findings and Section 6 concludes this research. 

> 1For each of the six HAR-type models, we apply the base as well as three switching variants, totalling 24 models. 

4 

## **2. Methodology** 

## _2.1. Realized Volatility_ 

In order to calculate realized volatilities of intraday prices, we define returns as logdifference between two consecutive intraday prices: 



where _M_ denotes the number of 5-min prices per day. Hence, we obtain a sampling frequency of 1 _/M_ . The realized volatility of day _t_ , denoted _RVt_ , is then defined as 



Barndorff-Nielsen & Sheppard (2004) introduce a measure of realized volatility which is robust against present jumps. This realized bi-power variation measure, _BPVt_ , is defined as 



and used for identifying the jump component _Jt,α_ . We apply the definitions of Huang & Tauchen (2005) and calculate the jump component as follows 



where Φ _α_ refers to the critical value of the Gaussian distribution with 



and _µ_ 1 = E ( _Z_ ) = ~~�~~ 2 _/π_ . The tri-power quarticity _TQt_ is defined as 



with _µp_ = 2<sup>_p/_2</sup> _·_ Γ (1 _/_ 2 _·_ ( _p_ + 1)) _·_ Γ (1 _/_ 2) (Andersen et al., 2007). Alternatives to this jump identification exist. 

One commonly used approach that is robust against small and very high jumps is the _median RV_ measures presented in Andersen et al. (2012). An additional mesaure, which is applied in this paper, is outlined in Corsi et al. (2010) and labelled _threshold bipower variation (TBPV)_ . It yields less biased estimates compared to standard, multipower variations of the continuous quadratic variation in finite samples (Corsi et al., 2010). The 

5 

TBPV is calculated as: 



where _I{·}_ is the indicator function and _ϑt,j_ as a threshold variable for all _t_ = 1 _, . . . , N_ and _j_ = 1 _, . . . , M_ . For this threshold variable, it holds 



where _cϑ_ is a scale-free constant while _V_<sup>ˆ</sup> _t,j_ is the non-parameter recursive filter for calculating the local variance. Similar to Corsi et al. (2010), we set _cϑ_ = 3. Analogously to the calculation of the test statistic _Zt_ in Eq. (4), we define the _TBPV_ -specific test statistic as follows: 



with 



Hence, we calculate the threshold jump component by 



Calculating the jump components _Jt,α_ and _Jt,α_<sup>_TBPV_</sup> in Eq. (3) and Eq. (6) yields the separation of the continuous components: 



In order to exploit possible leverage effects, Barndorff-Nielsen et al. (2010) separate the realized volatility in signed realized semivariances: 





6 

## _2.2. Heterogeneous Autoregressive (HAR) Models_ 

For all realized measures such as _RVt_ , _BPVt_ , _TBPVt_ , _Ct_ , _TCt_ , _Jt,α_ , and _TJt,α_ we calculate the average over a period of _h_ days as 



where _RM_ is a placeholder for the aforementioned measures. We apply _RM_ 1 _,t−_ 1, _RM_ 5 _,t−_ 1, and _RM_ 22 _,t−_ 1 as the daily, weekly, and monthly measures. 

We follow Corsi (2009) in the definition of the standard HAR model: 



where _ut_ is an error term. 

There are several extensions to the standard HAR model obtained by incorporating estimators for jumps or leverage effects. In addition to the standard HAR model, we apply the HAR-CJ model of Andersen et al. (2007) defined as 



the HAR-TCJ model (Corsi et al., 2010) defined as 



and the HAR-∆J model (Patton & Sheppard, 2015) 



The significant connection between speculation activities and the market volatility has been addressed in literature (Bessembinder & Seguin, 1992, Chang et al., 2000, Brunetti et al., 2016). Therefore, we augment the standard HAR with the speculation variable _Xspec_ , which is defined as 



and translates to the quotient of trading volume, _V ol_ , and open interest, _OI_ . Including this variable follows the assumption that speculation activity has some predictive power on future volatility (Bessembinder & Seguin, 1992). A large value of _Xspec_ provides some evidence of increased speculation activity. The augmented HAR-spec model reads 



7 

As last model we apply the recently introduced centralized HAR model by Bollerslev et al. (2018). It is suggested that the unconditional variance level parameter (the constant _a_ 0) should be modelled variably. The intercept of the HAR model is replaced with a variable long-run volatility factor _RVt_<sup>_LR_</sup> , which equals the expanding sample mean of daily RV from the start of the sample up until day _t_ . The model is defined as 



## _2.3. The infinite Hidden Markov switching (IHMS) process_ 

The HAR models defined in the previous section can be rewritten as general, linear regression model 



where _N_ denotes the Gaussian distribution and _σ_<sup>2</sup> is the variance of the residuals. The conventional HAR models outlined in Sec. 2.2 are specified with constant coefficients and variances. However, recent literature suggests that considering structural breaks improves the forecasting accuracy with HAR models as high-frequency data based realized volatilities of financial assets are subject to unknown structures (Bollerslev et al., 2015, **?** ). Therefore, we extend the HAR models with structural breaks by modeling coefficients and variance with infinite Hidden Markov switching processes. 

As outlined before, three specifications of IHM-HAR models are implemented; the DIHM-HAR, the IHM-HAR, and the IHMC-HAR variants. Specifically, both IHM-HAR and DIHM-HAR allow the model coefficients and variances to change over time. The key difference between these two models appears in the latent infinite Markov switching processes that drive the time variation in the model parameters. The IHM-HAR implicitly restricts the changes in all model parameters to occur at the same time. However, forcing all parameters to change simultaneously might not be empirically plausible. To allow for richer dynamics, we consider a more general DIHM-HAR model in which switches in coefficients and variances are governed by two independent infinite Hidden Markov processes ( _st_ and _zt_ ). As many studies have shown that allowing time-varying variances is important in improving the out-of-sample forecasting performance, we also consider the IHMC-HAR in which only the variance is allowed to be changing over time. These three types of IHM-HAR models are defined in detail below. 

For the DIHM-HAR model, we specify two independent infinite Markov regime switching processes to drive changes of the coefficients and variance, or in other words; the structural breaks in the coefficients and variances can occur at different time points. 

Then, Eq. (16) can be rewritten with state-dependent coefficients and variances as 

8 

follows: 



where the HAR coefficients **_αst_** and variance _σz_<sup>2</sup> _t_<sup>aregovernedbytwoindependentstate</sup> variables _st_ and _zt_ , respectively. 

These regime indicators follow two infinite Hidden Markov processes in the following framework: 













In this framework, the coefficients **_α_** _st_ , and variances _σz_<sup>2</sup> _t_<sup>aresampledfromtwobase</sup> distribution. In line with recent literature, we specify a Gaussian distribution (with parameters **_α_ 0** and Σ0) for _αi_ and an Inverse Gamma distribution (with parameters _φ_ 0 and _ν_ 0) for _σi_ . The regime indicators _st_ and _zt_ depend on the infinite state parameter **p**<sup>_s_</sup> _k_ and **p**<sup>_z_</sup> _k_<sup>whicharevectorsoftransitionprobabilitieswithinfinitedimension.</sup> 

Particularly, the infinite states parameter **p**<sup>_s_</sup> _k_<sup>and</sup><sup>**p**</sup><sup>_z_</sup> _k_<sup>followaDirichletprocess(DP).</sup> The DP process is first proposed by Ferguson (1973) while we use the version of Fox et al. (2011). The DP process is determined by the positive concentration parameter _c_ and the base distribution _G_ 0 which has the form _G_ 0 = (1 _− ρ_ ) _π_ + _ρδi_ , where 0 _< ρ <_ 1 denotes the sticky parameter that enhances the state self-transition probability and _δi_ is the degenerate probability measure sampled from _G_ 0. 

The parameter _π_ in the DP process is obtained from a stick-breaking process ( **SBP** ) in Eq. (23) following Sethuraman (1994). The **SBP** can be described as dividing a unit-length stick for infinite times. Breaking the stick the _i_ -th time is determined by _Vi ∼B_ (1 _, γ_ ) where _B_ denotes the Beta distribution. Then, _πi_ = _Vi_ � _ij−_ =11<sup>(1</sup><sup>_−Vj_)andwe</sup> define _π|γ ∼_ **SBP** ( _γ_ ) as the stick-breaking process. 

For the IHM-HAR model, the variation coefficients and variances are governed by the same Markov state variable _st_ , which follows an infinite Hidden Markov switching process 

9 

with 









For the IHMC-HAR model, the coefficients are assumed to be time-invariant and only the variances are driven by the infinite Hidden Markov switching process. The IHMCHAR then reads 











This paper follows Song (2014) by imposing a second hierarchical structure on the base distribution. This setting has been shown to be helpful in improving model forecasting performance. It is worth noting that an infinite Hidden Markov switching model allows structural breaks to occur in each out-of-sample period. Without the second hierarchical prior, the parameters in the new born regimes are assumed to be generated from a pre-fix base distribution which might not be an attractive feature in practice. In the modeling setting with a hierarchical structure, the parameters specified the base distribution are unknown and updated based on the existing regime parameters. This allows the parameters from the new born regimes to learn from the existing regimes through this second hierarchical structure. The empirical importance of setting a second hierarchical structure on the base distribution has also been highlighted in many recent empirical studies (Maheu & Yang, 2016, Bauwens et al., 2017, Hou, 2017). 

We follow this approach and set the second hierarchical priors as 





where _W_ ( _S, v_ ) denotes the Wishart distribution with scale matrix _S_ and degrees of freedom _v_ , _E_ ( _λ_ ) refers to the exponential distribution with mean _λ_ , and 1 ( _ν_ 0 _> n_ ) denotes the indicator function indicating that the support of the prior for _ν_ 0 is restricted to be greater than _n_ . Finally, the hyperparameters ( _cs, cz, γs, γz, ρs, ρz_ ) are assumed to follow 

10 

independent priors 





where _G_ ( _κ_ 1 _, κ_ 2) denotes the Gamma distribution with mean _κ_ 1 _/κ_ 2 and variance _κ_ 1 _/κ_ 2<sup>2.</sup> 

## _2.4. Estimations and priors_ 

In this section we briefly summarize the Markov-chain Monte Carlo (MCMC) sampler used to estimate the IHM models. More details about the MCMC posterior sampler can be found in Hou (2017). The main difficulty for obtaining the posterior draws for the IHM models appears during the sampling of the state parameters. As the number of states for the IHM model is not bounded, the usual sampling method (Chib, 1996), which is designed to deal with the traditional Markov switching model, cannot be applied for the IHM model. To obtain the exact posterior samples for the state parameters, this paper follows the beam sampler approach proposed by Van Gael et al. (2008). The main idea of the beam sampler is to augment the parameter space by introducing a set of auxiliary variables which are sampled along with the other model parameters. These auxiliary variables do not change the marginal posterior distributions of the other parameters. Suppose that we introduce a set of auxiliary variables ( _u_ 1 _, . . . , uT_ ) which follow a conditional distribution with density 



For simplicity, we suppress the rest of the conditioning variables. It can be seen that the joint density of ( _ut, st_ ) conditional on _st−_ 1 is given by _p_ ( _ut, st|st−_ 1) = 1 (0 _< ut < pst−_ 1 _,st_ ), which implies that the distribution obtained by marginalizaing out _ut_ is _p_ ( _st|st−_ 1) = _pst−_ 1 _,st_ which is exactly the same as the transition probability of the original model. Furthermore, it can be checked that the conditional density of _st_ given ( _ut, st−_ 1) is 



The finiteness of the set _{i_ : 0 _< u_<sup>_s_</sup> _t_<sup>_< ps_</sup> _st−_ 1 _,i_<sup>_}_impliesthatintroducingtheauxiliaryvari-</sup> able _ut_ truncates the infinite number of state trajectories to a finite number. Conditional on the auxiliary variable _ut_ , the method of Chib (1996) can now be applied as usual to obtain posterior samples of the state parameters _st_ . Given these state parameters, the other parameters are sampled using the standard method. For more discussion about the beam and posterior sampler for the IHM model, we refer to Song (2014), Hou (2017), and Van Gael et al. (2008). 

11 

For a more efficient comparison, we assume the same priors for parameters across all models. An informative prior is set for hyperparameters of the base distribution. In particular, we set the hyperparameters **_β_** _c_ = **_β_** 00 = **0** , **V** _c_ = **B** 00 = **I** _kβ_ , _a_ 00 = _kβ_ + 10, **A** 00 = 2 _a_ 00 **I** _kβ_ , _b_ 00 = 5, _q_ 00 = _b_ 00<sup>_−_1,and</sup><sup>_λ_00=</sup><sup>_n_+ 2.ForthepriorsoftheHiddenMarkov</sup> parameters, we set _ws_ = _wz_ = 5, _θs_ = _θz_ = 1, and _hs_ = _hz_ = _ηs_ = _ηz_ = 1. For the sticky parameters, we set _fs_ = _fz_ = 10 and _gs_ = _gz_ = 1 which implies a relatively high self-transition probability. 

## **3. Data and sampling** 

With the rapid development of China’s economy and the gradual relaxation of regulations for foreign investors, the trading volume of its commodity futures market has increased significantly. There are three main futures exchanges for agricultural commodities in China: the Zhengzhou Commodity Exchange (ZCE), the Dalian Commodity Exchange (DCE), and the Shanghai Futures Exchange (SHFE). According to the 2018 statistics of the American Futures Industry Association (FIA) based on the number of contracts traded and/or cleared at 76 exchanges worldwide, 12 of the top 20 agricultural products with the largest trading volume in the world come from Chinese exchanges. Chinese commodity futures markets are gaining in global importance and prices have begun to affect global prices for commodities. Biofuels are gaining more and more attention as it is considered clean energy and an important renewable substitute for fossil fuels. Corn, Palm Oil, and Soybean are the main feedstock for the biofuel industry. Prices and volatilities of these commodities are shown to be correlated to fossil fuel markets. 

In this paper, we use five agricultural commodity futures: Corn, Cotton, Indica Rice, Palm Oil, and Soybean. Soybean, Corn, and Palm Oil futures are traded at DCE, while Cotton and Indica Rice futures are traded at ZCE. Corn, Soybean, and Cotton futures are ranked among the top 20 agricultural agriculture futures contracts with the largest global trading volume. Indica Rice and Palm futures are two import agriculture commodity futures with high trading volumes in Chinese futures market. 

We analyze their behavior and aim to accurately forecast the volatilities of these commodity futures to provide useful information and implications on derivative pricing and portfolio selection. A better understanding of volatility dynamics and forecastability benefits researchers, market participants, as well as policymakers. 

We acquire high-frequency data in 5-min intervals from the _Wind Financial Database_ for the time period from January 1, 2011 to December 31, 2015 for Corn, Cotton, Indica Rice, Palm Oil, and Soybean. After data pre-processing, we finally obtain 1256 daily observations for Corn, 1176 for Indica Rice, and 1214 for the other three commodities. 

Tab. 1 presents descriptive statistics and preliminary tests of daily returns (Panel A), realized volatilities (Panel B), and the speculation variable (Panel C). In Panel A, it is found that the mean of all selected agriculture commodities in this paper is negative, 

12 

indicating the overall declining price trend during the sample period. Cotton traded at ZCE has the lowest mean return with _−_ 0 _._ 075, while Palm oil has the largest standard deviation with 1 _._ 214. All return series are stationary which is verified by an ADF unit root test. Panel B of Tab. 1 shows that Palm oil has the largest mean of realized volatility with 0 _._ 918, while Indica Rice has the largest standard deviation with 2 _._ 924. All realized volatilities have a fat-tailed and leptokurtic distribution and thus, follow a non-normal distribution. This is additionally verified by significant Jarque-Bera statistics. Panel C presents findings for the speculation variable, the quotient of trading volume and open interest, to measure the speculative behavior. It is assumed that an increase (decrease) in the trading volume relative to the open interest indicates there is an increase (decrease) in the activity of the speculators. The statistics results in Panel C show that Palm oil has the largest mean of the speculation variable with 1 _._ 086. It indicates that Palm oil tends to attract more speculation activity relative to other agricultural commodities, which is consistent with its largest mean of realized volatility among the five commodities. In addition to Tab. 1, daily returns, realized volatilities, and the speculation variable for all five commodities are visualized in Fig. 1, Fig. 2, and Fig. 3, respectively. 

_Insert Tab. 1 approximately here._ 

_Insert Fig. 1-3 approximately here._ 

Fig. 1 and Fig. 2 plot the daily returns and realized volatilities of the five agricultural commodity futures. The returns and volatilities exhibit great fluctuations in 2010, 2012, and throughout 2015. These periods correspond to the first listing of Chinese index futures in 2010, the Euro debt crisis in 2011, and the China’s stock market crisis in 2015. Additionally, we plot the speculation variables for each futures series in Fig. 3. In these plots, we find evidence that high levels of these speculation variables correspond to market phases of high volatility for the respective commodity. 

## **4. Results** 

## _4.1. In-sample estimations_ 

This section presents the full-sample estimation results for the DIHM-HAR and IHMHAR model. Fig. 4 and Fig. 5 show the posterior mean and posterior distribution of the number of regimes for the DIHM-HAR and IHM-HAR model, respectively. These figures show that each agricultural commodity is specific in its characteristics on posterior mean and distribution of regime numbers, implying its different regime uncertainty in model estimations. Different numbers of regimes intuitively reflect differing uncertainty in which the existence of more regimes usually indicates better capacity to depict extreme situations in market changes. 

In Fig. 4 for the DIHM-HAR model, the posterior distributions of the number of regimes for coefficients _β_ for Corn, Indica Rice, and Soybean present symmetric char- 

13 

acteristics with similar probability peaks around 30 regimes, though the shapes of their posterior mean of regime switching is quite different. The posterior distribution of the number of regimes for coefficiients _β_ for Cotton and Palm oil is a right-skewed, fat-tail distribution. The shape of the posterior distribution of number of regimes for variances for the five agricultural commodities are similar in the sense that the probability distribution decreases with an increased regime number, except for Soybean. 

Fig. 5 shows a different situation of posterior distributions of regime numbers for the IHM-HAR model. The shapes of posterior distribution of regime numbers for the five agricultural commodities are relatively asymmetric compared with Fig. 4, except for Palm oil. Moreover, the shapes of posterior mean of regime switching for the five agricultural commodities present no obvious difference. 

## _Insert Fig. 4-5 approximately here._ 

To better investigate the data classification into different regimes, the heat maps for the posterior probabilities in the DIHM-HAR and the IHM-HAR model for the five commodities are depicted in Fig. 6 and Fig. 7. Referring to Hou (2017), the lighter (darker) the color is in a cell ( _i, j_ ) of a heat map, the higher (lower) the probability that time _i_ and time _j_ are clustered into the same regime. For the heat maps of Fig. 6 and Fig. 7, it is evident that the clustering of regimes for coefficients and the volatilities are quite different over time for each agricultural commodity. The posterior probabilities for the coefficients visualized in the left column of Fig. 6 show that the regimes are not likely to be recurrent over time, indicated by dark colors for each agricultural commodity. However, for the posterior probabilities of volatilities visualized in the right column of Fig. 6, the color distribution is quite different for each agricultural commodity. In the case of Indica Rice the color is light before 2015, indicating that regimes have no apparent switching over time. However, for Cotton and Soybean, the light regions for the posterior probabilities of volatilities are few, indicating their time-variable regime switching. The heat map in Fig. 7 based on the IHM-HAR model also shows that the regimes change frequently over time. 

## _Insert Fig. 6-7 approximately here._ 

Exploring the effects of the speculation activity on the volatility of agriculture commodity futures, we analyze the mean and standard deviation of the time-varying coefficients of the DIHM-HAR-spec and IHM-HAR model which are given in Tab. 2 and Tab. 3, respectively. In short, the results suggest that the speculation variable exhibits strong and positively directed effects on the future volatility of commodity futures. The coefficient _as_ is highly significant, particularly for Corn, Cotton, and Palm oil. Hence, we augment the conventional HAR framework with the speculation variable and quantify its usefulness for forecasting in what follows. 

14 

_Insert Tab. 2 and Tab. 3 approximately here._ 

## _4.2. Out-of-sample evaluation_ 

To evaluate the out-of-sample forecasting performance of the different HAR variants, we divide each sample series into two parts: the in-sample that includes about 2/3 of observations and the out-of-sample that includes the remainder of 1/3 of the sample observations. We employ a recursive forecast method to obtain the short-term ( _h_ = 1), mid-term ( _h_ = 5), and long-term ( _h_ = 22) out-of-sample forecasts, corresponding to one-day, one-week and one-month ahead forecasts. 

For the 1-step-ahead forecasts, we obtain realized volatility forecasts by re-estimating each volatility forecast model with in-sample observations. 

For the multi-step forecasts, there are two types of forecast methods: the direct forecasts and the iterated forecasts as suggested in Marcellino et al. (2006). Specifically, the multi-step, iterated forecast results are obtained by iterating the forecast model with the previous forecast results. However, the forecast precision would deteriorate if the misspecification problem exists as suggested by Andersen et al. (2003) and Sizova (2011). Therefore, we use the direct forecasts to obtain the multi-step ahead forecast results following Corsi (2009). The direct forecasts aim at forecasting the cumulative _h_ -day ahead realized volatility 



for _h_ = 1 _,_ 5 _,_ 22, which means that the one-step ahead realized volatility is replaced with the accumulated realized volatility over the weekly or monthly horizon in the forecast models. 

Both the forecast precision and the economic significance of various volatility forecast models are evaluated in this article. By combining three types of the infinite Hidden Markov switching methods and six different HAR base models, outlined in Section 2.2, we construct a total of 18 HAR models. The six non-switching HAR models are included as benchmark models. We evaluate the unbiasedness of the forecast model based on the Mincer-Zarnowitz- _R_<sup>2</sup> (MZ- _R_<sup>2</sup> ) and the statistical loss functions such as the Mean Absolute Forecast Error (MAFE). The forecast precision is evaluated using the Model Confident Set (MCS) method of Hansen et al. (2011). Furthermore, we assess the forecast quality for all models from a portfolio optimization perspective. Given the optimal weights of a portfolio based on a risky and a risk-free asset, we compute the excess return and the economic value in order to measure the performances of various volatility models in portfolio construction. 

## _4.3. Statistical evaluation_ 

Firstly, we calculate the MZ- _R_<sup>2</sup> and the MAFE loss function to evaluate the out-ofsample forecast precision. The MZ- _R_<sup>2</sup> is the goodness-of-fit ( _R_<sup>2</sup> ) for the Mincer-Zarnowitz 

15 

regression 

_RVt_ +1: _t_ + _h_ = _β_ 0 + _β_ 1 _RV_<sup>ˆ</sup> _t_ +1: _t_ + _h,t_ + _εt,_ 

ˆ where _RV t_ +1: _t_ + _h,t_ is the _h_ -step-ahead forecast of the realized volatility based on time _t_ and _RVt_ +1: _t_ + _h_ is the actual realized volatility. The higher the value of MZ- _R_<sup>2</sup> , in our case when _β_ 0 = 0 and _β_ 1 = 1, the better the forecast performance of the respective volatility model. 

Given the size of the in-sample period _T_ 1 and the size of the out-of-sample period _T_ 2, with _T_ 1 + _T_ 2 = _T_ , the MAFE is defined as 



In order to determine models which outperform in terms of forecasting quality, we apply the Model Confidence Set (MCS) of Hansen et al. (2011). The MCS identifies a set of models with preferable forecasting performance (the confidence set) from a set of all models 



For a given confidence level _α_ , we identify the MCS _M_<sup>_∗_</sup> 1 _−α_<sup>_⊆M_0whichincludes</sup> models that outperform all other models in the complementary set _M_<sup>_<u>∗</u>_</sup> 1 _−α_<sup>.Itholdsthat</sup> _M_<sup>_∗_</sup> 1 _−α_<sup>_∪_</sup> _M_<sup>_<u>∗</u>_</sup> 1 _−α_<sup>=</sup><sup>_M_0.Bydefiningtherelativeperformancevariable</sup><sup>_dij,t_:=</sup><sup>_Li,t−Lj,t_</sup> with _i, j_ = 1 _, . . . , M_ and _Li,t_ being the loss function of the _i_ -th Model, _M_<sup>_i_</sup> , the MCS method then removes the least-performing models based on the equal predictive accuracy (ECA) test with 



We implement the MCS using the stationary bootstrap outlined in Hansen et al. (2011). Firstly, we generate _B_ bootstrap resamples given the loss functions and choose the block-length bootstrap parameter _L_ = 16 as in Hansen et al. (2011) and calculate the relative performance variable for each bootstrap resample. Based on the range statistic _TR_ of Hansen et al. (2011), we determine _M_<sup>_∗_</sup> 1 _−α_<sup>inatwo-stepiterativeprocedure.2</sup> 

We display the statistical evaluation results for the 1-step forecasts in Tab. 4 based on the MZ- _R_<sup>2</sup> , the MAFE loss function and the MCS results. The results are consistent for selecting the best performing forecast models. For all five agriculture commodity futures, the HAR model combined with the independent Hidden Markov switching have better forecast precision compared with the benchmark HAR models. Particularly, for the sample of Cotton, Indica Rice, and Palm futures, the HAR models within the three 

> 2For more detail on the MCS and stationary block bootstrap we refer the reader to Hansen et al. (2011) and Politis & Romano (1994). 

16 

types of infinite Hidden Markov switching outperform the corresponding benchmark HAR models, suggesting that considering structural breaks in coefficients and variance improves the forecast performances of the original HAR model. For Corn and Soybean, the IHMC model with constant coefficients cannot outperform the benchmark HAR model. As shown in Tab. 1, realized volatilities of Corn and Soybean futures have low standard deviations; hence, only considering the structural breaks in variances results in insufficient performance of the IHMC-HAR models for these two samples. 

Moreover, we find that the specification of independent Markov processes (DIHM) for coefficients and variances in the HAR models achieves the best forecasting precision among each group of HAR models. It is worth noting that the HAR models with the speculation variable have better forecast accuracy than all other models, suggesting that the speculation variable plays an import role in volatility forecasts for agricultural commodity futures. Based on the MCS results, most of the HAR models with infinite Hidden Markov processes survive in the MCS at the _α_ = 25% confidence level. 

## _Insert Tab. 4 approximately here._ 

Tab. 5 and Tab. 6 display the statistical evaluation results for the mid-term (a week ahead) and long-term (a month ahead) forecast models. As shown in Tab. 5, the results of MZ- _R_<sup>2</sup> and the MAFE loss function suggest that the HAR models with the DIHM and IHM structures outperform the corresponding benchmark HAR models for all the five agriculture commodity futures. Moreover, most of the DIHM-HAR type models and IHM-HAR type models can survive in the MCS at the 25% confident level, suggesting their significant improvement in the forecast precision compared to the benchmark models. 

The long-term forecast results as shown in Tab.6 are similar to the mid-term forecast results. We also find superior forecast precision of the DIHM-HAR type models and IHMHAR type models compared to the corresponding benchmark HAR models. All HAR models with DIHM and IHM structures can survive in the MCS at the 10% confident set for the long-term forecasts. 

In addition, we find that including the speculation variable in the HAR models improve forecast accuracy of the original HAR models for both the mid-term and long-term forecasts. 

## _Insert Tab. 5 and Tab. 6 approximately here._ 

In conclusion, the IHM-HAR and DIHM-HAR models achieve higher forecast precision for short-, mid-, and long-term forecasts compared with to the benchmark HAR models. Both IHM-HAR and DIHM-HAR models allow the coefficients and variances to be changing over time. As the time-varying coefficients capture the unknown structural breaks in the volatilities of agricultural commodity futures, both of these two models perform rather well for the out-of-sample forecasts in term of statistical evaluation. Par- 

17 

ticularly, the DIHM-HAR models perform better than the IHM-HAR models for the shortand long-term forecasts, suggesting that modeling structural changes in the coefficients and variances separately produces better forecast precision. However, the IHMC-HAR models perform relative worse compared with the benchmark HAR models in terms of forecast precision. Only variances are allowed to be time-varying and coefficients are set to be time-invariant in the IHMC-HAR. Thus, the time-variation in the volatilities cannot be fully captured by the IHMC-HAR models. 

## _4.4. The economic evaluation_ 

The statistical evaluation focuses on comparing the forecast accuracy and quality of various volatility forecasting models. However, market participants care more about the economic gains from volatility forecasts. To evaluate their economic significance, we consider an investor with the following mean-variance utility: 



where _γ_ refers to the _risk aversion rate_ , E ( _Rp_ ) denotes the expected value of the portfolio return _Rp_ , and V( _Rp_ ) denotes the portfolio variance. 

The risk-averse investor allocates its budget to a portfolio containing a risky asset and a risk-free asset. According to Campbell & Thompson (2008) and Neely et al. (2014), the optimal weight which is allocated to the agricultural commodity futures, based on forecasts of time _t_ + 1, should be 



where _r_ ˆ _t_ +1 is the forecasted excess return of one of the agricultural commodity futures and _σ_ ˆ _t_<sup>2</sup> +1<sup>istheforecastedvariance.BasedonCampbell&Thompson(2008)andNeely</sup> et al. (2014), we employ the moving average of the excess returns with a rolling window of 256 trading days (1-year) to proxy the forecasted excess return. As the volatility of risk-free asset is trivial compared to the risky asset, the forecasted variance of returns is the volatility forecast obtained by the models outlined and analyzed in previous sections. 

Then, the portfolio return at time _t_ + 1 is given by 



where _rt_ +1 and _rt_<sup>_f_</sup> +1<sup>aretheexcessreturnsofagriculturefuturesandtherisk-freereturn</sup> at time _t_ + 1, respectively. In this paper, we employ the return of a 5-year Treasury bill in China as the return of the risk-free asset. 

Moreover, for the direct multi-step forecasts, the results for the multi-step forecast models are averaged over all possible weekly (e.g., Monday to Monday, Tuesday to Tuesday, etc.) horizons and monthly horizons (e.g. the 1st day of a month, the 2nd day of a 

18 

month and so on). Thus, the actual returns corresponding to the long horizon forecast of realized volatility for portfolio optimization are the moving averages of returns with a weekly or monthly window such that 



In addition, we employ the economic value to assess the economic significance of various multivariate volatility models. The economic value is the value of ∆such that 



for two different portfolios _p_ 1 and _p_ 2. The greater ∆, the more returns a risk-averse investor is willing to sacrifice to switch from model _l_ to model _k_ . Based on Fleming & Hern´andez-Hern´andez (2003), Bollerslev et al. (2016), and Callot et al. (2017), we use the quadratic utility with risk aversion to determine the economic values of various forecast models: 



In this paper, we consider two levels of risk aversion rates for the investor, the mild risk aversion rate _γ_ = 1 and the strong risk aversion rate _γ_ = 10. Tab. 7 shows the results of economic evaluation in terms of the portfolio return and the economic values of various HAR models within the infinite Hidden Markov switching framework. For the sake of brevity, we only report the return of portfolio based on a mild risk aversion level _γ_ = 1. We also display the economic values of each infinite Hidden Markov switching HAR models against the corresponding benchmark HAR model based on two types of risk aversion rates. 

The economic evaluation results suggest the HAR models combined with the three types of infinite Hidden Markov switching methods achieve higher portfolio returns compared to the original HAR models. Particularly, including the speculation variables in the HAR models increases the portfolio returns for all types of agricultural commodity futures. Moreover, almost all infinite Hidden Markov switching HAR models have positive economic values against the corresponding benchmark HAR model, suggesting the utility of the risk-averse investor can be improved by switching from the benchmark HAR models to the infinite Hidden Markov switching HAR models for making investment decisions. The improvement is more significant for the investor with a mild risk-averse rate. 

_Insert Tab. 7 approximately here._ 

Tab. 8 and Tab. 9 present the results of the economic evaluation in terms of the mid- 

19 

term ( _h_ = 5) and long-term ( _h_ = 22) forecasts. The results among these two forecasting horizons are similar. For Palm Oil futures, almost all infinite Hidden Markov switching HAR models achieve positive economic values against the corresponding benchmark HAR models, which is consistent with the results of short-term forecasts. However, for the other agriculture commodity samples, only HAR models with IHMC structure outperform the corresponding HAR models for mid-term and long-term forecasts. 

In conclusion, the HAR models combined with the three types of infinite Hidden Markov switching structure improve the portfolio return and achieve positive economic gains compared to the base HAR models for the short-term forecasts, while only the IHMC-HAR models outperform the corresponding HAR models in terms of the portfolio return and the economic value for the mid-term and long-term forecasts. 

_Insert Tab. 8 and Tab. 9 approximately here._ 

## **5. Robustness checks with alternative realized volatility estimators** 

Following Fiszeder & Perczak (2016) and **?** , we conduct a robustness check of the proposed models with alternative realized volatility estimators. Hansen & Lunde (2006) suggest the realized kernel estimator to correct the effects of market microstructure noise on the estimators of realized volatility, which reads 



Following Hansen & Lunde (2006), we set 



where _⌈x⌉_ denotes the smallest integer that is greater or equal to _x_ , _ω_ is the desired width of the lag window, and _b − a_ is the length of the sampling period. 

Then, we conduct the 1-step ahead forecasts with the realized kernel estimator. The statistical evaluation and the economic evaluation results are displayed in Tab. 10 and Tab. 11 respectively. 

From Tab. 10 the statistical evaluation results suggest that most of the HAR models with infinite Hidden Markov models outperform the benchmark HAR models. Moreover, as shown in Tab. 11, the HAR models with infinite Hidden Markov switching structures perform better than the benchmark HAR models in terms of portfolio return and economic value. These results verify that our models have outstanding forecast performances and that these forecast results are robust to the choice of realized volatility estimators. 

20 

_Insert Tab. 10 and Tab. 11 approximately here._ 

Moreover, we employ another alternative realized volatility estimator, the _medRV_ estimator proposed by Andersen et al. (2012), which is a jump-robust estimators of integrated variance. The statistical and economic evaluation results are shown in Tab. 12 and Tab. 12, respectively. According to these tables, the HAR models with infinite Hidden Markov models outperform the benchmark HAR models in terms of the statistical and economic evaluation criteria. Therefore, the forecast results are robust to the choice of realized volatility estimator. 

_Insert Tab. 12 and Tab. 13 approximately here._ 

## **6. Conclusions** 

We construct a set of HAR models with three types of infinite Hidden Markov regime switching structures. Particularly, jumps, leverage effects, and speculation effects are taken into account in realized volatility modeling. We forecast five agricultural commodity futures (Corn, Cotton, Indica Rice, Palm Oil, and Soybean) based on high frequency data obtained from Chinese futures markets. We analyze regime switching behavior with infinite Hidden Markov variants of different base HAR models. We compare the forecast precision of various models based on the MZ- _R_<sup>2</sup> , the MAFE, and the MCS tests and further evaluate the economic significance of these forecast models in terms of portfolio return and economic value. 

With respect to the statistical evaluation, the HAR models combined with double Hidden Markov processes (DIHM) and a single Hidden Markov process (IHM) have better forecast precision when compared with non-switching benchmark HAR models. These results hold for short-term, mid-term, and long-term forecasts. The HAR models with speculation variable have better forecast accuracy than all other models, suggesting the speculation variable plays an important role in volatility forecasts for agricultural commodity futures. 

With respect to economic evaluation, HAR models combined with three types of infinite Hidden Markov switching structures improve the portfolio return and achieve positive economic gains compared to the corresponding benchmark HAR models for short-term forecasts. The IHMC-HAR model outperforms the corresponding benchmark models in terms of portfolio return and economic value for the mid-term and long-term forecasts. In addition, the improvements in the portfolio performances of the HAR models with infinite Hidden Markov switching structures are higher for mildly risk-averse investors. 

In conclusion, we focus on introducing different Markov switching structures to improve the efficiency and accuracy of high-frequency volatility forecast models. We compare the forecast performance based on statistical and economic evaluation methods, which provide a comprehensive way to identify suitable forecast models for practical application. 

21 

We provide a new analytical tool for market participants to model and forecast volatility of agricultural commodity futures. This has great economic significance for investors to generate suitable portfolio strategies to quantify and control risk as well as for policy makers to determine adequate policies to ensure the stability of financial markets. 

22 

## **References** 

- Andersen, T. G., Bollerslev, T., & Diebold, F. X. (2007). Roughing It Up: Including Jump Components in the Measurement, Modeling, and Forecasting of Return Volatility. _Review of Economics and Statistics_ , _89_ , 701–720. doi: `10.1162/rest.89.4.701` . 

- Andersen, T. G., Bollerslev, T., Diebold, F. X., & Labys, P. (2003). Modeling and Forecasting Realized Volatility. _Econometrica_ , _71_ , 579–625. doi: `10.1111/1468-0262. 00418` . 

- Andersen, T. G., Dobrev, D., & Schaumburg, E. (2012). Jump-robust volatility estimation using nearest neighbor truncation. _Journal of Econometrics_ , _169_ , 75–93. doi: `10.1016/ j.jeconom.2012.01.011` . 

- Aulton, A. J., Ennew, C. T., & Rayner, A. J. (1997). Efficiency Tests of Futures Markets for UK Agricultural Commodities. _Journal of Agricultural Economics_ , _48_ , 408–424. doi: `10.1111/j.1477-9552.1997.tb01162.x` . 

- Barndorff-Nielsen, O. E., Kinnebrock, S., & Shephard, N. (2010). Measuring Downside Risk – Realized Semivariance. In _Volatility and Time Series Econometrics_ (pp. 117– 136). Oxford University Press. doi: `10.1093/acprof:oso/9780199549498.003.0007` . 

- Barndorff-Nielsen, O. E., & Sheppard, N. (2004). Power and Bipower Variation with Stochastic Volatility and Jumps. _Journal of Financial Econometrics_ , _2_ , 1–37. doi: `10. 1093/jjfinec/nbh001` . 

- Bauwens, L., Carpantier, J.-F., & Dufays, A. (2017). Autoregressive Moving Average Infinite Hidden Markov-Switching Models. _Journal of Business & Economic Statistics_ , _35_ , 162–182. doi: `10.1080/07350015.2015.1123636` . 

- Bessembinder, H., & Seguin, P. J. (1992). Futures-Trading Activity and Stock Price Volatility. _The Journal of Finance_ , _47_ , 2015–2034. doi: `10.1111/j.1540-6261.1992. tb04695.x` . 

- Bollerslev, T., Hood, B., Huss, J., & Pedersen, L. H. (2018). Risk Everywhere: Modeling and Managing Volatility. _The Review of Financial Studies_ , _31_ , 2729–2773. doi: `10. 1093/rfs/hhy041` . 

- Bollerslev, T., Patton, A. J., & Quaedvlieg, R. (2016). Exploiting the errors: A simple approach for improved volatility forecasting. _Journal of Econometrics_ , _192_ , 1–18. doi: `10.1016/j.jeconom.2015.10.007` . 

- Bollerslev, T., Xu, L., & Zhou, H. (2015). Stock return and cash flow predictability: The role of volatility risk. _Journal of Econometrics_ , _187_ , 458–471. doi: `10.1016/j.jeconom. 2015.02.031` . 

- Brunetti, C., B¨uy¨uk¸sahin, B., & Harris, J. H. (2016). Speculators, Prices, and Market Volatility. _Journal of Financial and Quantitative Analysis_ , _51_ , 1545–1574. doi: `10. 1017/S0022109016000569` . 

- Callot, L. A. F., Kock, A. B., & Medeiros, M. C. (2017). Modeling and Forecasting Large Realized Covariance Matrices and Portfolio Choice. _Journal of Applied Econometrics_ , _32_ , 140–158. doi: `10.1002/jae.2512` . 

23 

- Campbell, J. Y., & Thompson, S. B. (2008). Predicting Excess Stock Returns Out of Sample: Can Anything Beat the Historical Average? _Review of Financial Studies_ , _21_ , 1509–1531. doi: `10.1093/rfs/hhm055` . 

- Caporin, M., Ranaldo, A., & Velo, G. G. (2015). Precious metals under the microscope: a high-frequency analysis. _Quantitative Finance_ , _15_ , 743–759. doi: `10.1080/14697688. 2014.947313` . 

- Chan, J. C.-C., & Jeliazkov, I. (2009). MCMC Estimation of Restricted Covariance Matrices. _Journal of Computational and Graphical Statistics_ , _18_ , 457–480. doi: `10. 1198/jcgs.2009.08095` . 

- Chang, E., Chou, R. Y., & Nelling, E. F. (2000). Market volatility and the demand for hedging in stock index futures. _Journal of Futures Markets_ , _20_ , 105–125. doi: `10.1002/ (SICI)1096-9934(200002)20:2<105::AID-FUT1>3.0.CO;2-Q` . 

- Chib, S. (1996). Calculating posterior distributions and modal estimates in Markov mixture models. _Journal of Econometrics_ , _75_ , 79–97. doi: `10.1016/0304-4076(95) 01770-4` . 

- Corsi, F. (2009). A simple approximate long-memory model of realized volatility. _Journal of Financial Econometrics_ , _7_ , 174–196. doi: `10.1093/jjfinec/nbp001` . 

- Corsi, F., Pirino, D., & Ren`o, R. (2010). Threshold bipower variation and the impact of jumps on volatility forecasting. _Journal of Econometrics_ , _159_ , 276–288. doi: `10.1016/ j.jeconom.2010.07.008` . 

- Degiannakis, S., & Filis, G. (2017). Forecasting oil price realized volatility using information channels from other asset classes. _Journal of International Money and Finance_ , _76_ , 28–49. doi: `10.1016/j.jimonfin.2017.05.006` . 

- Du, X., Yu, C. L., & Hayes, D. J. (2011). Speculation and volatility spillover in the crude oil and agricultural commodity markets: A Bayesian analysis. _Energy Economics_ , _33_ , 497–503. doi: `10.1016/j.eneco.2010.12.015` . 

- Engle, R. (2002). Dynamic Conditional Correlation. _Journal of Business & Economic Statistics_ , _20_ , 339–350. doi: `10.1198/073500102288618487` . 

- Escobar, J. C., Lora, E. S., Venturini, O. J., Y´a˜nez, E. E., Castillo, E. F., & Almazan, O. (2009). Biofuels: Environment, technology and food security. _Renewable and Sustainable Energy Reviews_ , _13_ , 1275–1287. doi: `10.1016/j.rser.2008.08.014` . 

- Ferguson, T. S. (1973). A Bayesian Analysis of Some Nonparametric Problems. _The Annals of Statistics_ , _1_ , 209–230. doi: `10.1214/aos/1176342360` . 

- Fiszeder, P., & Perczak, G. (2016). Low and high prices can improve volatility forecasts during periods of turmoil. _International Journal of Forecasting_ , _32_ , 398–410. doi: `10. 1016/j.ijforecast.2015.07.003` . 

- Fleming, W. H., & Hern´andez-Hern´andez, D. (2003). An optimal consumption model with stochastic volatility. _Finance and Stochastics_ , _7_ , 245–262. doi: `10.1007/ s007800200083` . 

24 

- Fox, E., Sudderth, E. B., Jordan, M. I., & Willsky, A. S. (2011). Bayesian Nonparametric Inference of Switching Dynamic Linear Models. _IEEE Transactions on Signal Processing_ , _59_ , 1569–1585. doi: `10.1109/TSP.2010.2102756` . 

- Hansen, P. R., & Lunde, A. (2006). Consistent ranking of volatility models. _Journal of Econometrics_ , _131_ , 97–121. doi: `10.1016/j.jeconom.2005.01.005` . 

- Hansen, P. R., Lunde, A., & Nason, J. M. (2011). The Model Confidence Set. _Econometrica_ , _79_ , 453–497. doi: `10.3982/ECTA5771` . 

- Hou, C. (2017). Infinite hidden markov switching VARs with application to macroeconomic forecast. _International Journal of Forecasting_ , _33_ , 1025–1043. doi: `10.1016/j. ijforecast.2017.06.006` . 

- Huang, X., & Tauchen, G. (2005). The Relative Contribution of Jumps to Total Price Variance. _Journal of Financial Econometrics_ , _3_ , 456–499. doi: `10.1093/jjfinec/nbi025` . 

- Ji, Q., Bouri, E., Roubaud, D., & Shahzad, S. J. H. (2018). Risk spillover between energy and agricultural commodity markets: A dependence-switching CoVaR-copula model. _Energy Economics_ , _75_ , 14–27. doi: `10.1016/j.eneco.2018.08.015` . 

- Ji, Q., & Fan, Y. (2016). Evolution of the world crude oil market integration: A graph theory analysis. _Energy Economics_ , _53_ , 90–100. doi: `10.1016/j.eneco.2014.12.003` . 

- Jiang, H., Todorova, N., Roca, E., & Su, J.-J. (2017). Dynamics of volatility transmission between the U.S. and the Chinese agricultural futures markets. _Applied Economics_ , _49_ , 3435–3452. doi: `10.1080/00036846.2016.1262517` . 

- Klein, T. (2018). Trends and contagion in WTI and Brent crude oil spot and futures markets - The role of OPEC in the last decade. _Energy Economics_ , _75_ , 636–646. doi: `10.1016/j.eneco.2018.09.013` . 

- Klein, T., & Walther, T. (2016). Oil Price Volatility Forecast with Mixture Memory GARCH. _Energy Economics_ , _58_ , 46–58. doi: `10.1016/j.eneco.2016.06.004` . 

- Krueger, A. O., Schiff, M., & Vald´es, A. (1988). Agricultural Incentives in Developing Countries: Measuring the Effect of Sectoral and Economywide Policies. _The World Bank Economic Review_ , _2_ , 255–271. doi: `10.1093/wber/2.3.255` . 

- Luo, J., & Ji, Q. (2018). High-frequency volatility connectedness between the US crude oil market and China’s agricultural commodity markets. _Energy Economics_ , _76_ , 424–438. doi: `10.1016/j.eneco.2018.10.031` . 

- Ly´ocsa, S., & Moln´ar, P. (2016). Volatility forecasting of strategically linked commodity ETFs: gold-silver. _Quantitative Finance_ , _16_ , 1809–1822. doi: `10.1080/14697688.2016. 1211799` . 

- Ly´ocsa, S., Moln´ar, P., & Todorova, N. (2017). Volatility forecasting of non-ferrous metal futures: Covariances, covariates or combinations? _Journal of International Financial Markets, Institutions and Money_ , _51_ , 228–247. doi: `10.1016/j.intfin.2017.08.005` . 

- Ma, F., Wahab, M., Huang, D., & Xu, W. (2017). Forecasting the realized volatility of the oil futures market: A regime switching approach. _Energy Economics_ , _67_ , 136–145. doi: `10.1016/j.eneco.2017.08.004` . 

25 

- Ma, F., Wei, Y., Chen, W., & He, F. (2018a). Forecasting the volatility of crude oil futures using high-frequency data: further evidence. _Empirical Economics_ , _55_ , 653– 678. doi: `10.1007/s00181-017-1294-6` . 

- Ma, F., Wei, Y., Liu, L., & Huang, D. (2018b). Forecasting realized volatility of oil futures market: A new insight. _Journal of Forecasting_ , _37_ , 419–436. doi: `10.1002/for.2511` . 

- Ma, Y., Ji, Q., & Pan, J. (2019). Oil financialisation and volatility forecast: Evidence from multidimensional predictors. _Journal of Forecasting_ , (p. for.2577). doi: `10.1002/ for.2577` . 

- Maheu, J. M., & Yang, Q. (2016). An infinite hidden Markov model for short-term interest rates. _Journal of Empirical Finance_ , _38_ , 202–220. doi: `10.1016/j.jempfin.2016.06. 006` . 

- Marcellino, M., Stock, J. H., & Watson, M. W. (2006). A comparison of direct and iterated multistep AR methods for forecasting macroeconomic time series. _Journal of Econometrics_ , _135_ , 499–526. doi: `10.1016/j.jeconom.2005.07.020` . 

- Natanelov, V., Alam, M. J., McKenzie, A. M., & Van Huylenbroeck, G. (2011). Is there co-movement of agricultural commodities futures prices and crude oil? _Energy Policy_ , _39_ , 4971–4984. doi: `10.1016/j.enpol.2011.06.016` . 

- Nazlioglu, S., Erdem, C., & Soytas, U. (2013). Volatility spillover between oil and agricultural commodity markets. _Energy Economics_ , _36_ , 658–665. doi: `10.1016/j.eneco. 2012.11.009` . 

- Neely, C. J., Rapach, D. E., Tu, J., & Zhou, G. (2014). Forecasting the Equity Risk Premium: The Role of Technical Indicators. _Management Science_ , _60_ , 1772–1791. doi: `10.1287/mnsc.2013.1838` . 

- Nerlove, M. (1956). Estimates of the Elasticities of Supply of Selected Agricultural Commodities. _Journal of Farm Economics_ , _38_ , 496. doi: `10.2307/1234389` . 

- Nguyen, D. K., & Walther, T. (2019). Modeling and Forecasting Commodity Market Volatility with Long-term Economic and Financial Variables. _Journal of Forecasting, forth._ , . doi: `10.1002/for.2617` . 

- Patton, A. J., & Sheppard, K. (2015). Good Volatility, Bad Volatility: Signed Jumps and The Persistence of Volatility. _Review of Economics and Statistics_ , _97_ , 683–697. doi: `10.1162/REST{\_}a{\_}00503` . 

- Politis, D. N., & Romano, J. P. (1994). The Stationary Bootstrap. _Journal of the American Statistical Association_ , _89_ , 1303–1313. doi: `10.1080/01621459.1994.10476870` . 

- Raggi, D., & Bordignon, S. (2012). Long memory and nonlinearities in realized volatility: A Markov switching approach. _Computational Statistics & Data Analysis_ , _56_ , 3730– 3742. doi: `10.1016/j.csda.2010.12.008` . 

- Schmidhuber, J. (2007). Biofuels: An Emerging Threat to Europe’s Food Security? _Notre Europe Policy Paper 27_ , (pp. 1–40). 

26 

- Sethuraman, J. (1994). A Constructive Deefinition of Dirichlet Priors. _Statistica Sinica_ , _4_ , 639–650. 

- S´evi, B. (2014). Forecasting the volatility of crude oil futures using intraday data. _European Journal of Operational Research_ , _235_ , 643–659. doi: `10.1016/j.ejor.2014.01. 019` . 

- Sizova, N. (2011). Integrated variance forecasting: Model based vs. reduced form. _Journal of Econometrics_ , _162_ , 294–311. doi: `10.1016/j.jeconom.2011.02.004` . 

- Song, Y. (2014). Modelling Regime Switching and Structural Breaks with an Infinite Hidden Markov Model. _Journal of Applied Econometrics_ , _29_ , 825–842. doi: `10.1002/ jae.2337` . 

- Souˇcek, M., & Todorova, N. (2013). Realized volatility transmission between crude oil and equity futures markets: A multivariate HAR approach. _Energy Economics_ , _40_ , 586–597. doi: `10.1016/j.eneco.2013.08.011` . 

- Teh, Y. W., Jordan, M. I., Beal, M. J., & Blei, D. M. (2006). Hierarchical Dirichlet Processes. _Journal of the American Statistical Association_ , _101_ , 1566–1581. doi: `10. 1198/016214506000000302` . 

- Tian, F., Yang, K., & Chen, L. (2016). Realized Volatility Forecasting of Agricultural Commodity Futures Using Long Memory and Regime Switching. _Journal of Forecasting_ , . doi: `10.1002/for.2443` . 

- Todorova, N., Worthington, A., & Souˇcek, M. (2014). Realized volatility spillovers in the non-ferrous metal futures market. _Resources Policy_ , _39_ , 21–31. doi: `10.1016/j. resourpol.2013.10.008` . 

- Tyner, W. E. (2010). The integration of energy and agricultural markets. _Agricultural Economics_ , _41_ , 193–201. doi: `10.1111/j.1574-0862.2010.00500.x` . 

- Van Gael, J., Saatci, Y., Teh, Y. W., & Ghahramani, Z. (2008). Beam sampling for the infinite hidden Markov model. In _Proceedings of the 25th international conference on Machine learning - ICML ’08_ (pp. 1088–1095). New York, New York, USA: ACM Press. doi: `10.1145/1390156.1390293` . 

- Wen, F., Gong, X., & Cai, S. (2016). Forecasting the volatility of crude oil futures using HAR-type models with structural breaks. _Energy Economics_ , _59_ , 400–413. doi: `10. 1016/j.eneco.2016.07.014` . 

- Witt, H. J., Schroeder, T. C., & Hayenga, M. L. (1987). Comparison of analytical approaches for estimating hedge ratios for agricultural commodities. _Journal of Futures Markets_ , _7_ , 135–146. doi: `10.1002/fut.3990070204` . 

- Yang, K., & Chen, L. (2014). Realized Volatility Forecast: Structural Breaks, Long Memory, Asymmetry, and Day-of-the-Week Effect. _International Review of Finance_ , _14_ , 345–392. doi: `10.1111/irfi.12030` . 

- Yang, K., Tian, F., Chen, L., & Li, S. (2017). Realized volatility forecast of agricultural futures using the HAR models with bagging and combination approaches. _International Review of Economics & Finance_ , _49_ , 276–291. doi: `10.1016/j.iref.2017.01.030` . 

27 

- Zhu, X.-H., Zhang, H.-W., & Zhong, M.-R. (2017). Volatility forecasting in Chinese nonferrous metals futures market. _Transactions of Nonferrous Metals Society of China_ , _27_ , 1206–1214. doi: `10.1016/S1003-6326(17)60141-9` . 

## **7. Tables** 

Table 1: Descriptive Statistics of the agricultural commodities of this study; Corn, Cotton, Indica Rice, Palm, and Soybeans. 

|Agriculture<br>commodit|Mean|<sup>Standard </sup><br>Deviation|S|kewness|Kurtosis|Jarque-Bera|LB Q(5)|LB Q(10)|LB Q(20)|ADF|
|---|---|---|---|---|---|---|---|---|---|---|
|y|||||Pane|l A: Daily Retu|rns||||
|Corn|_−_0_._030|1_._182||_−_4_._89|105_._69|556889_._74<sup>_∗∗∗_</sup>|84_._06|143_._90|190_._14<sup>_∗_</sup>|_−_31_._58<sup>_∗∗∗_</sup>|
|Cotton|_−_0_._075|1_._101||_−_2_._93|44_._17|87492_._75<sup>_∗∗∗_</sup>|5_._23|11_._12|28_._90|_−_25_._83<sup>_∗∗∗_</sup>|
|Indica Rice|_−_0_._005|0_._733||1_._95|31_._51|40586_._82<sup>_∗∗∗_</sup>|6_._86|10_._45|18_._94|_−_23_._00<sup>_∗∗∗_</sup>|
|Palm Oil|_−_0_._057|1_._214||0_._31|6_._60|674_._21<sup>_∗∗∗_</sup>|2_._56|3_._80|10_._88|_−_24_._28<sup>_∗∗∗_</sup>|
|Soybean|_−_0_._018|0_._877||_−_0_._08|6_._70|692_._03<sup>_∗∗∗_</sup>|2_._57|11_._17|20_._29|_−_25_._18<sup>_∗∗∗_</sup>|
||||||Panel|B: Realized Vola|tility||||
|Corn|0_._231|0_._306||4_._85|37_._70|67943_._83<sup>_∗∗∗_</sup>|1186_._67<sup>_∗∗∗_</sup>|1932_._48<sup>_∗∗∗_</sup>|2791_._40<sup>_∗∗∗_</sup>|_−_14_._24<sup>_∗∗∗_</sup>|
|Cotton|0_._637|1_._030||4_._80|36_._76|62300_._92<sup>_∗∗∗_</sup>|1376_._24<sup>_∗∗∗_</sup>|2232_._77<sup>_∗∗∗_</sup>|3626_._86<sup>_∗∗∗_</sup>|_−_13_._06<sup>_∗∗∗_</sup>|
|Indica Rice|0_._686|2_._924||19_._59|483_._47|11386890_._41<sup>_∗∗∗_</sup>|34_._27<sup>_∗∗∗_</sup>|53_._89<sup>_∗∗∗_</sup>|77_._56<sup>_∗∗∗_</sup>|_−_21_._64<sup>_∗∗∗_</sup>|
|Palm Oil|0_._918|1_._895||16_._33|331_._85|5524222_._36<sup>_∗∗∗_</sup>|95_._35<sup>_∗∗∗_</sup>|112_._65<sup>_∗∗∗_</sup>|139_._77<sup>_∗∗∗_</sup>|_−_21_._47<sup>_∗∗∗_</sup>|
|Soybean|0_._459|0_._509||3_._62|22_._42|21719_._07<sup>_∗∗∗_</sup>|761_._81<sup>_∗∗∗_</sup>|1202_._67<sup>_∗∗∗_</sup>|1734_._76<sup>_∗∗∗_</sup>|_−_15_._40<sup>_∗∗∗_</sup>|
||||||Panel C|: Speculation V|ariable||||
|Corn|0_._309|0_._228||2_._31|12_._53|5869_._32<sup>_∗∗∗_</sup>|1338_._78<sup>_∗∗∗_</sup>|2137_._89<sup>_∗∗∗_</sup>|3027_._59<sup>_∗∗∗_</sup>|_−_13_._38<sup>_∗∗∗_</sup>|
|Cotton|0_._884|1_._195||2_._63|9_._53|3558_._62<sup>_∗∗∗_</sup>|5030_._28<sup>_∗∗∗_</sup>|9443_._85<sup>_∗∗∗_</sup>|17071_._90<sup>_∗∗∗_</sup>|_−_4_._91<sup>_∗∗∗_</sup>|
|Indica Rice|0_._291|0_._330||4_._19|28_._89|36277_._70<sup>_∗∗∗_</sup>|2157_._24<sup>_∗∗∗_</sup>|3888_._59<sup>_∗∗∗_</sup>|5768_._60<sup>_∗∗∗_</sup>|_−_11_._57<sup>_∗∗∗_</sup>|
|Palm Oil|1_._086|0_._516||0_._97|4_._33|278_._19<sup>_∗∗∗_</sup>|2556_._19<sup>_∗∗∗_</sup>|4404_._98<sup>_∗∗∗_</sup>|6830_._32<sup>_∗∗∗_</sup>|_−_10_._01<sup>_∗∗∗_</sup>|
|Soybean|0_._600|0_._433||2_._35|12_._11|5312_._79<sup>_∗∗∗_</sup>|1900_._82<sup>_∗∗∗_</sup>|2842_._42<sup>_∗∗∗_</sup>|3866_._14<sup>_∗∗∗_</sup>|_−_11_._19<sup>_∗∗∗_</sup>|



_Note:_ This table reports the statistical summary of the daily returns (Panel A), realized volatility (Panel B), and the speculation variable (Panel C) of each agricultural commodity future. The speculation variable is the ratio between the open interest and the trading volume. ***, **, and * refer to the level of significance of 1%, 5%, and 10%, respectively. 

Table 2: Time-varying coefficients for the DIHM-HAR-spec model. 

||Corn|Co|tton|Indic|a Rice|Pa|lm|Soy|bean|
|---|---|---|---|---|---|---|---|---|---|
||mean<br>std|mean|std|mean|std|mean|std|mean|std|
|_a_0<br>_ad_<br>_aw_<br>_am_<br>_as_|0_._0407 0_._0390<br>0_._0711 0_._0705<br>0_._1081 0_._0674<br>0_._1936 0_._1105<br>0_._3092 0_._0434|0_._0930 <br>0_._0687 <br>0_._0024 <br>0_._2907 <br>0_._2037|0_._0979<br> 0_._0756<br> 0_._0526<br> 0_._1328<br> 0_._0528|0_._0906 <br>0_._0772 <br>0_._0966 <br>0_._2129 <br>0_._2274|0_._0554<br> 0_._0491<br> 0_._0975<br> 0_._1554<br> 0_._1939|0_._2847 <br>0_._1338 <br>0_._0830 <br>0_._0210 <br>0_._2355|0_._1412<br> 0_._1012<br> 0_._0690<br> 0_._0397<br> 0_._0413|0_._0306<br>0_._0573<br>0_._1559<br>0_._2567<br>0_._3293|0_._0158<br>0_._0435<br>0_._1052<br>0_._0704<br>0_._1950|
|_Not_<br>var<br>var|_e:_<br>This table <br>ying coefficients<br>iables, and the|shows <br>fi for th<br>specula|the mea<br>e const<br>tion var|n and <br>ant, the<br>iable in|standar<br> daily, <br> the DI|d devia<br> weekly, <br>HM-HA|tion (st<br> and m<br>R-Spec|d) of t<br>onthly <br> model.|he time-<br>volatility|



28 

Table 3: Time-varying coefficients for the IHM-HAR-spec model. 

||Co|rn|Cot|ton|Indica|Rice|Pa|lm|Soy|bean|
|---|---|---|---|---|---|---|---|---|---|---|
||mean|std|mean|std|mean|std|mean|std|mean|std|
|_a_0|0_._1109|0_._0857|0_._2424|0_._1693|0_._2509|0_._1913|0_._4312|0_._1791|0_._2406|0_._1581|
|_ad_|0_._0285|0_._0116|0_._0830|0_._0765|0_._0639|0_._0444|0_._0748|0_._0514|0_._0521|0_._0202|
|_aw_|0_._0422|0_._0278|0_._0404|0_._0327|0_._0219|0_._0241|0_._0325|0_._0116|0_._0158|0_._0468|
|_am_|0_._0538|0_._0631|0_._1098|0_._0688|0_._0964|0_._0762|0_._0483|0_._0240|0_._0966|0_._0436|
|_as_|0_._2784|0_._0382|0_._1244|0_._0250|0_._1322|0_._0654|0_._1898|0_._0532|0_._2008|0_._1061|



_Note:_ This table shows the mean and standard deviation (std) of the timevarying coefficients for the constant, the daily, weekly, and monthly volatility variables, and the speculation variable in the IHM-HAR-Spec model. 

Table 4: Forecasting precision results for 1-step forecasts. 

||C|orn|Co|tton|Indic|a Rice|Pal|m Oil|So|ybean|
|---|---|---|---|---|---|---|---|---|---|---|
||MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|
|DIHM-HAR|0_._345|0_._174<sup>_∗∗_</sup>|0_._219|0_._398<sup>_∗∗_</sup>|0_._006|1_._231<sup>_∗∗_</sup>|0_._032|0_._819<sup>_∗∗_</sup>|0_._177|0_._320<sup>_∗∗_</sup>|
|IHM-HAR|0_._361|0_._176<sup>_∗∗_</sup>|0_._181|0_._406<sup>_∗∗_</sup>|0_._011|1_._194<sup>_∗∗_</sup>|0_._025|0_._793<sup>_∗∗_</sup>|0_._182|0_._319<sup>_∗∗_</sup>|
|IHMC-HAR<br>HAR|0_._372<br>0_._357|0_._180<sup>_∗∗_</sup><br>0_._175<sup>_∗∗_</sup>|0_._175<br>0_._159|0_._408<sup>_∗∗_</sup><br>0_._445|0_._005<br>0_._002|1_._250<sup>_∗∗_</sup><br>1_._271<sup>_∗∗_</sup>|0_._026<br>0_._024|0_._822<sup>_∗∗_</sup><br>0_._889|0_._155<br>0_._170|0_._333<br>0_._325|
|DIHM-HARCJ|0_._358|0_._174<sup>_∗∗_</sup>|0_._188|0_._399<sup>_∗∗_</sup>|0_._004|1_._215<sup>_∗∗_</sup>|0_._046|0_._794<sup>_∗∗_</sup>|0_._184|0_._316<sup>_∗∗_</sup>|
|IHM-HARCJ|0_._359|0_._175<sup>_∗∗_</sup>|0_._185|0_._404<sup>_∗∗_</sup>|0_._021|1_._173<sup>_∗∗_</sup>|0_._035|0_._782<sup>_∗∗_</sup>|0_._191|0_._316<sup>_∗∗_</sup>|
|IHMC-HARCJ|0_._368|0_._180<sup>_∗∗_</sup>|0_._182|0_._405<sup>_∗∗_</sup>|0_._003|1_._253<sup>_∗_</sup>|0_._049|0_._779<sup>_∗∗_</sup>|0_._175|0_._329|
|HARCJ|0_._351|0_._176<sup>_∗∗_</sup>|0_._146|0_._453|0_._002|1_._393<sup>_∗_</sup>|0_._028|0_._865|0_._194|0_._322<sup>_∗∗_</sup>|
|DIHM-HARTCJ|0_._347|0_._172<sup>_∗∗_</sup>|0_._226|0_._397<sup>_∗∗_</sup>|0_._013|1_._203<sup>_∗∗_</sup>|0_._024|0_._825|0_._206|0_._314<sup>_∗∗_</sup>|
|IHM-HARTCJ|0_._342|0_._176<sup>_∗∗_</sup>|0_._188|0_._403<sup>_∗∗_</sup>|0_._022|1_._178<sup>_∗∗_</sup>|0_._040|0_._783<sup>_∗∗_</sup>|0_._197|0_._314<sup>_∗∗_</sup>|
|IHMC-HARTCJ|0_._338|0_._179<sup>_∗∗_</sup>|0_._179|0_._403<sup>_∗∗_</sup>|0_._003|1_._260|0_._045|0_._78<sup>_∗∗_</sup>|0_._174|0_._330|
|HARTCJ|0_._354|0_._177<sup>_∗∗_</sup>|0_._150|0_._451<sup>_∗∗_</sup>|0_._001|1_._307<sup>_∗∗_</sup>|0_._030|0_._864|0_._200|0_._323<sup>_∗∗_</sup>|
|DIHM-HAR∆J|0_._377|0_._172<sup>_∗∗_</sup>|0_._221|0_._391<sup>_∗∗_</sup>|0_._007|1_._218<sup>_∗∗_</sup>|0_._012|0_._838|0_._204|0_._313<sup>_∗∗_</sup>|
|IHM-HAR∆J|0_._364|0_._175<sup>_∗∗_</sup>|0_._209|0_._395<sup>_∗∗_</sup>|0_._009|1_._195<sup>_∗∗_</sup>|0_._024|0_._793<sup>_∗∗_</sup>|0_._208|0_._313<sup>_∗∗_</sup>|
|IHMC-HAR∆J|0_._377|0_._178<sup>_∗∗_</sup>|0_._203|0_._398<sup>_∗∗_</sup>|0_._004|1_._246<sup>_∗_</sup>|0_._029|0_._814|0_._160|0_._331|
|HAR∆J|0_._363|0_._173<sup>_∗∗_</sup>|0_._170|0_._440|0_._004|1_._271<sup>_∗_</sup>|0_._031|0_._875|0_._207|0_._317<sup>_∗∗_</sup>|
|DIHM-HARspec|0_._356|0_._173<sup>_∗∗_</sup>|0_._213|0_._395<sup>_∗∗_</sup>|0_._005|1_._230<sup>_∗∗_</sup>|0_._015|0_._812<sup>_∗_</sup>|0_._163|0_._324<sup>_∗∗_</sup>|
|IHM-HARspec|0_._359|0_._175<sup>_∗∗_</sup>|0_._179|0_._398<sup>_∗∗_</sup>|0_._016|1_._193<sup>_∗∗_</sup>|0_._028|0_._791<sup>_∗∗_</sup>|0_._176|0_._319<sup>_∗∗_</sup>|
|IHMC-HARspec|0_._361|0_._185|0_._173|0_._410<sup>_∗_</sup>|0_._006|1_._237<sup>_∗∗_</sup>|0_._027|0_._814|0_._153|0_._334|
|HARspec|0_._352|0_._175<sup>_∗∗_</sup>|0_._160|0_._424<sup>_∗_</sup>|0_._002|1_._276<sup>_∗∗_</sup>|0_._024|0_._885|0_._168|0_._324|
|DIHM-HARc|0_._345|0_._173<sup>_∗∗_</sup>|0_._206|0_._394<sup>_∗∗_</sup>|0_._005|1_._255<sup>_∗_</sup>|0_._017|0_._802<sup>_∗∗_</sup>|0_._172|0_._320<sup>_∗∗_</sup>|
|IHM-HARc|0_._361|0_._173<sup>_∗∗_</sup>|0_._173|0_._409<sup>_∗_</sup>|0_._012|1_._198<sup>_∗∗_</sup>|0_._025|0_._788<sup>_∗∗_</sup>|0_._176|0_._318<sup>_∗∗_</sup>|
|IHMC-HARc|0_._377|0_._178<sup>_∗∗_</sup>|0_._171|0_._425<sup>_∗_</sup>|0_._007|1_._249<sup>_∗∗_</sup>|0_._028|0_._823<sup>_∗_</sup>|0_._162|0_._332|
|HARc|0_._356|0_._174<sup>_∗∗_</sup>|0_._160|0_._440|0_._002|1_._277<sup>_∗_</sup>|0_._025|0_._886|0_._172|0_._325|



_Note:_ This table reports the evaluation results of the forecast precision based on MZ- _R_<sup><u>2</u></sup> , MAFE, and MCS results for the 1-step forecasts. The MAFE results marked with * and ** refer to corresponding MCS 

_p_ -values of _>_ 10% and _>_ 25%, respectively. 

Table 5: Forecasting precision results for 5-step forecasts. 

||C<br>MZ-_R_<sup>2</sup>|orn<br>MAFE|Co<br>MZ-_R_<sup>2</sup>|tton<br>MAFE|Indic<br>MZ-_R_<sup>2</sup>|a Rice<br>MAFE|Palm<br>MZ-_R_<sup>2</sup>|Oil<br>MAFE|Soy<br>MZ-_R_<sup>2</sup>|bean<br>MAFE|
|---|---|---|---|---|---|---|---|---|---|---|
|DIHM-HAR<br>IHM-HAR<br>IHMC-HAR<br>HAR|0_._627<br>0_._797<br>0_._487<br>0_._496|0_._087<sup>_∗_</sup><br>0_._071<sup>_∗∗_</sup><br>0_._148<br>0_._131|0_._410<br>0_._800<br>0_._210<br>0_._233|0_._210<sup>_∗∗_</sup><br>0_._145<sup>_∗∗_</sup><br>0_._357<br>0_._375|0_._052<br>0_._058<br>0_._017<br>0_._008|0_._877<sup>_∗∗_</sup><br>0_._832<sup>_∗∗_</sup><br>1_._047<br>1_._003|0_._057<br>0_._210<br>0_._034<br>0_._038|0_._555<br>0_._483<sup>_∗∗_</sup><br>0_._775<br>0_._768|0_._728<br>0_._783<br>0_._178<br>0_._177|0_._119<sup>_∗_</sup><br>0_._109<sup>_∗∗_</sup><br>0_._277<br>0_._254|
|DIHM-HARCJ|0_._659|0_._084<sup>_∗∗_</sup>|0_._413|0_._224<sup>_∗_</sup>|0_._070|0_._840<sup>_∗∗_</sup>|0_._024|0_._566|0_._620|0_._128|
|IHM-HARCJ<br>IHMC-HARCJ<br>HARCJ|0_._763<br>0_._500<br>0_._500|0_._073<sup>_∗∗_</sup><br>0_._149<br>0_._132|0_._766<br>0_._142<br>0_._325|0_._152<sup>_∗∗_</sup><br>0_._385<br>0_._376|0_._051<br>0_._023<br>0_._005|0_._842<sup>_∗∗_</sup><br>1_._094<br>1_._172<sup>_∗∗_</sup>|0_._167<br>0_._068<br>0_._068|0_._493<sup>_∗∗_</sup><br>0_._684<br>0_._735|0_._734<br>0_._215<br>0_._203|0_._111<sup>_∗∗_</sup><br>0_._270<br>0_._249|
|DIHM-HARTCJ<br>IHM-HARTCJ<br>IHMC-HARTCJ|0_._602<br>0_._760<br>0_._458|0_._093<sup>_∗_</sup><br>0_._072<sup>_∗∗_</sup><br>0_._154|0_._416<br>0_._790<br>0_._172|0_._212<sup>_∗_</sup><br>0_._147<sup>_∗∗_</sup><br>0_._367|0_._086<br>0_._067<br>0_._009|0_._837<sup>_∗∗_</sup><br>0_._800<sup>_∗∗_</sup><br>1_._096|0_._047<br>0_._219<br>0_._053|0_._549<br>0_._476<sup>_∗∗_</sup><br>0_._707|0_._589<br>0_._755<br>0_._203|0_._131<br>0_._112<sup>_∗∗_</sup><br>0_._271|
|HARTCJ|0_._495|0_._133|0_._219|0_._398|0_._003|1_._066<sup>_∗_</sup>|0_._083|0_._729|0_._208|0_._248|
|DIHM-HAR∆J|0_._607|0_._095<sup>_∗_</sup>|0_._440|0_._210<sup>_∗_</sup>|0_._095|0_._847<sup>_∗∗_</sup>|0_._066|0_._546|0_._697|0_._125<sup>_∗_</sup>|
|IHM-HAR∆J<br>IHMC-HAR∆J<br>HAR∆J|0_._789<br>0_._492<br>0_._506|0_._070<br>0_._148<br>0_._131|0_._809<br>0_._196<br>0_._217|0_._147<sup>_∗∗_</sup><br>0_._358<br>0_._379|0_._070<br>0_._018<br>0_._019|0_._824<sup>_∗∗_</sup><br>1_._053<br>0_._981|0_._275<br>0_._039<br>0_._053|0_._467<sup>_∗∗_</sup><br>0_._760<br>0_._746|0_._734<br>0_._188<br>0_._193|0_._114<sup>_∗_</sup><br>0_._274<br>0_._249|
|DIHM-HARspec<br>IHM-HARspec<br>IHMC-HARspec<br>HARspec|0_._652<br>0_._794<br>0_._488<br>0_._499|0_._087<sup>_∗_</sup><br>0_._071<sup>_∗∗_</sup><br>0_._150<br>0_._131|0_._420<br>0_._744<br>0_._195<br>0_._248|0_._208<sup>_∗∗_</sup><br>0_._162<sup>_∗∗_</sup><br>0_._362<br>0_._346|0_._051<br>0_._039<br>0_._020<br>0_._006|0_._886<sup>_∗∗_</sup><br>0_._858<sup>_∗∗_</sup><br>1_._051<br>1_._014|0_._066<br>0_._258<br>0_._035<br>0_._042|0_._537<br>0_._467<sup>_∗∗_</sup><br>0_._774<br>0_._764|0_._637<br>0_._754<br>0_._183<br>0_._180|0_._127<br>0_._108<sup>_∗∗_</sup><br>0_._277<br>0_._254|
|DIHM-HARc<br>IHM-HARc<br>IHMC-HARc<br>HARc|0_._569<br>0_._691<br>0_._505<br>0_._498|0_._096_∗_<br>0_._080<sup>_∗∗_</sup><br>0_._151<br>0_._132|0_._651<br>0_._660<br>0_._223<br>0_._240|0_._186<sup>_∗_</sup><br>0_._181<sup>_∗_</sup><br>0_._369<br>0_._373|0_._051<br>0_._031<br>0_._031<br>0_._008|0_._887<sup>_∗∗_</sup><br>0_._897<sup>_∗∗_</sup><br>1_._013<br>1_._007|0_._043<br>0_._122<br>0_._036<br>0_._042|0_._555<br>0_._503<sup>_∗_</sup><br>0_._778<br>0_._760|0_._726<br>0_._807<br>0_._186<br>0_._181|0_._123<br>0_._105<sup>_∗∗_</sup><br>0_._272<br>0_._254|



_Note:_ This table reports the evaluation results of the forecast precision based on MZ- _R_<sup><u>2</u></sup> , MAFE, and MCS results for the 5-step forecasts. The MAFE results marked with * and ** refer to corresponding MCS _p_ -values of _>_ 10% and _>_ 25%, respectively. 

29 

Table 6: Forecasting precision results for 22-step forecasts. 

||C<br>MZ-_R_<sup>2</sup>|orn<br>MAFE|Co<br>MZ-_R_<sup>2</sup>|tton<br>MAFE|Indic<br>MZ-_R_<sup>2</sup>|a Rice<br>MAFE|Pal<br>MZ-_R_<sup>2</sup>|m Oil<br>MAFE|Soy<br>MZ-_R_<sup>2</sup>|bean<br>MAFE|
|---|---|---|---|---|---|---|---|---|---|---|
|DIHM-HAR<br>IHM-HAR<br>IHMC-HAR<br>HAR|0_._962<br>0_._946<br>0_._583<br>0_._572|0_._028<sup>_∗∗_</sup><br>0_._031<sup>_∗∗_</sup><br>0_._133<br>0_._106|0_._905<br>0_._922<br>0_._034<br>0_._089|0_._067<sup>_∗∗_</sup><br>0_._060<sup>_∗∗_</sup><br>0_._373<br>0_._348|0_._397<br>0_._552<br>0_._023<br>0_._046|0_._469<sup>_∗∗_</sup><br>0_._329<sup>_∗∗_</sup><br>0_._911<br>0_._818|0_._753<br>0_._752<br>0_._176<br>0_._205|0_._224<sup>_∗∗_</sup><br>0_._187<sup>_∗∗_</sup><br>0_._594<br>0_._600|0_._870<br>0_._940<br>0_._326<br>0_._329|0_._052<sup>_∗∗_</sup><br>0_._042<sup>_∗∗_</sup><br>0_._297<br>0_._203|
|DIHM-HARCJ<br>IHM-HARCJ<br>IHMC-HARCJ<br>HARCJ|0_._952<br>0_._953<br>0_._529<br>0_._570|0_._030<sup>_∗∗_</sup><br>0_._031<sup>_∗∗_</sup><br>0_._141<br>0_._108|0_._879<br>0_._887<br>0_._039<br>0_._132|0_._074<sup>_∗∗_</sup><br>0_._068<sup>_∗∗_</sup><br>0_._394<br>0_._349|0_._427<br>0_._554<br>0_._030<br>0_._036|0_._451<sup>_∗∗_</sup><br>0_._334<sup>_∗∗_</sup><br>0_._978<br>0_._810|0_._746<br>0_._732<br>0_._194<br>0_._215|0_._233<sup>_∗_</sup><br>0_._192<sup>_∗∗_</sup><br>0_._589<br>0_._591|0_._818<br>0_._939<br>0_._365<br>0_._343|0_._054<sup>_∗∗_</sup><br>0_._041<sup>_∗∗_</sup><br>0_._276<br>0_._198|
|DIHM-HARTCJ|0_._953|0_._031_∗∗_|0_._843|0_._082<sup>_∗∗_</sup>|0_._333|0_._505<sup>_∗∗_</sup>|0_._779|0_._225<sup>_∗_</sup>|0_._835|0_._055<sup>_∗∗_</sup>|
|IHM-HARTCJ<br>IHMC-HARTCJ|0_._956<br>0_._540|0_._029<sup>_∗∗_</sup><br>0_._138|0_._912<br>0_._046|0_._065<sup>_∗∗_</sup><br>0_._383|0_._683<br>0_._061|0_._289<sup>_∗∗_</sup><br>0_._922|0_._819<br>0_._167|0_._184<sup>_∗∗_</sup><br>0_._591|0_._933<br>0_._351|0_._042<sup>_∗∗_</sup><br>0_._282|
|HARTCJ|0_._572|0_._107|0_._085|0_._379|0_._014|0_._858|0_._219|0_._583|0_._340|0_._204|
|DIHM-HAR∆J|0_._953|0_._030<sup>_∗∗_</sup>|0_._885|0_._071<sup>_∗∗_</sup>|0_._381|0_._472<sup>_∗∗_</sup>|0_._799|0_._194<sup>_∗∗_</sup>|0_._868|0_._051<sup>_∗∗_</sup>|
|IHM-HAR∆J<br>IHMC-HAR∆J<br>HAR∆J|0_._950<br>0_._588<br>0_._570|0_._031<sup>_∗∗_</sup><br>0_._133<br>0_._107|0_._931<br>0_._032<br>0_._084|0_._062<sup>_∗∗_</sup><br>0_._375<br>0_._346|0_._602<br>0_._025<br>0_._069|0_._306<sup>_∗∗_</sup><br>0_._911<br>0_._810|0_._768<br>0_._170<br>0_._209|0_._201<sup>_∗∗_</sup><br>0_._595<br>0_._597|0_._938<br>0_._330<br>0_._321|0_._043<sup>_∗∗_</sup><br>0_._296<br>0_._204|
|DIHM-HARspec<br>IHM-HARspec<br>IHMC-HARspec|0_._967<br>0_._956<br>0_._583|0_._027<sup>_∗∗_</sup><br>0_._030<sup>_∗∗_</sup><br>0_._131|0_._893<br>0_._936<br>0_._034|0_._069<sup>_∗∗_</sup><br>0_._058<sup>_∗∗_</sup><br>0_._373|0_._347<br>0_._609<br>0_._035|0_._489<sup>_∗∗_</sup><br>0_._311<sup>_∗∗_</sup><br>0_._885|0_._812<br>0_._796<br>0_._176|0_._197<sup>_∗∗_</sup><br>0_._191<sup>_∗∗_</sup><br>0_._593|0_._886<br>0_._930<br>0_._323|0_._049<sup>_∗∗_</sup><br>0_._043<sup>_∗∗_</sup><br>0_._302|
|HARspec|0_._574|0_._107|0_._114|0_._325|0_._036|0_._827|0_._208|0_._598|0_._345|0_._204|
|DIHM-HARc<br>IHM-HARc<br>IHMC-HARc|0_._940<br>0_._906<br>0_._603|0_._035<sup>_∗∗_</sup><br>0_._039<sup>_∗_</sup><br>0_._121|0_._942<br>0_._880<br>0_._074|0_._066<sup>_∗∗_</sup><br>0_._080<sup>_∗∗_</sup><br>0_._378|0_._419<br>0_._590<br>0_._027|0_._451<sup>_∗∗_</sup><br>0_._316<sup>_∗∗_</sup><br>0_._901|0_._801<br>0_._801<br>0_._200|0_._196<sup>_∗∗_</sup><br>0_._176<sup>_∗∗_</sup><br>0_._571|0_._892<br>0_._944<br>0_._405|0_._048<sup>_∗∗_</sup><br>0_._041<sup>_∗∗_</sup><br>0_._259|
|HARc|0_._592|0_._111|0_._101|0_._366|0_._039|0_._827|0_._214|0_._595|0_._343|0_._203|



_Note:_ This table reports the evaluation results of the forecast precision based on MZ- _R_<sup><u>2</u></sup> , MAFE, and MCS results for the 22-step forecasts. The MAFE results marked with * and ** refer to corresponding MCS _p_ -values of _>_ 10% and _>_ 25%, respectively. 

Table 7: Results of the economic evaluation for 1-step forecasts. 

|||Corn|||Cotton||In|dica Ric|e||Palm Oi|l||Soybea|n|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|||E|V||E|V||E|V||E|V||E|V|
||_r_<br>|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|
|DIHM-HAR|20_._381|1_._176|0_._127|14_._388|2_._404|0_._260|2_._804|_−_0_._822|_−_0_._089|5_._707|0_._656|0_._071|5_._862|0_._151|0_._016|
|IHM-HAR<br>|20_._677|1_._469|0_._159|12_._984|0_._965|0_._104|3_._758|0_._199|0_._021|5_._532|0_._479|0_._052|5_._729|0_._016|0_._002|
|IHMC-HAR|25_._739|6_._659|0_._722|18_._113|6_._299|0_._681|4_._332|3_._679|0_._397|5_._461|0_._406|0_._044|6_._559|0_._858|0_._093|
|HAR|19_._223|0_._000|0_._000|12_._041|0_._000|0_._000|3_._655|0_._000|0_._000|5_._058|0_._000|0_._000|5_._712|0_._000|0_._000|
|DIHM-HARCJ|21_._562|1_._798|0_._195|14_._410|2_._257|0_._244|4_._242|1_._104|0_._119|5_._516|0_._356|0_._038|5_._890|0_._206|0_._022|
|IHM-HARCJ|20_._642|0_._892|0_._097|12_._496|0_._287|0_._031|4_._650|0_._459|0_._050|5_._725|0_._568|0_._061|5_._740|0_._055|0_._006|
|IHMC-HARCJ|25_._131|5_._498|0_._596|17_._162|5_._123|0_._554|10_._243|8_._481|2_._706|5_._876|0_._720|0_._078|6_._550|0_._876|0_._095|
|HARCJ|19_._772|0_._000|0_._000|12_._205|0_._000|0_._000|3_._877|0_._000|0_._000|5_._163|0_._000|0_._000|5_._686|0_._000|0_._000|
|DIHM-HARTCJ|21_._405|1_._170|0_._127|15_._050|2_._601|0_._281|4_._462|0_._331|0_._036|5_._566|0_._347|0_._037|5_._988|0_._115|0_._012|
|IHM-HARTCJ|21_._274|1_._024|0_._111|13_._133|0_._626|0_._068|3_._852|_−_0_._576|_−_0_._062|5_._655|0_._436|0_._047|5_._764|_−_0_._111|_−_0_._012|
|IHMC-HARTCJ|26_._017|5_._943|0_._645|18_._075|5_._761|0_._623|_−_3_._655|3_._205|_−_0_._755|5_._773|0_._555|0_._060|6_._668|0_._805|0_._087|
|HARTCJ|20_._287|0_._000|0_._000|12_._516|0_._000|0_._000|3_._804|0_._000|0_._000|5_._222|0_._000|0_._000|5_._874|0_._000|0_._000|
|DIHM-HARDJ|20_._402|2_._344|0_._254|14_._576|2_._722|0_._294|3_._992|0_._858|0_._093|5_._393|_−_0_._084|_−_0_._009|5_._979|0_._387|0_._042|
|IHM-HARDJ|20_._151|2_._104|0_._228|11_._970|0_._028|0_._003|3_._991|0_._382|0_._041|5_._465|_−_0_._011|_−_0_._001|5_._638|0_._041|0_._004|
|IHMC-HARDJ|24_._330|6_._399|0_._694|6_._543|_−_6_._170|_−_0_._666|4_._589|3_._283|0_._354|5_._436|_−_0_._042|_−_0_._004|6_._702|1_._121|0_._121|
|HARDJ|18_._081|0_._000|0_._000|11_._927|0_._000|0_._000|4_._096|0_._000|0_._000|5_._474|0_._000|0_._000|5_._597|0_._000|0_._000|
|DIHM-HARspec|21_._217|1_._364|0_._148|14_._526|1_._439|0_._155|4_._534|1_._323|0_._143|5_._347|0_._384|0_._041|5_._944|0_._168|0_._018|
|IHM-HARspec|20_._971|1_._109|0_._120|13_._236|0_._132|0_._014|3_._851|0_._330|0_._036|5_._529|0_._570|0_._061|5_._767|_−_0_._012|_−_0_._001|
|IHMC-HARspec|27_._350|7_._655|0_._831|19_._004|6_._129|0_._663|2_._953|2_._847|0_._307|5_._465|0_._503|0_._054|6_._641|0_._874|0_._094|
|HARspec|19_._863|0_._000|0_._000|13_._105|0_._000|0_._000|3_._667|0_._000|0_._000|4_._965|0_._000|0_._000|5_._778|0_._000|0_._000|
|DIHM-HARc|21_._286|1_._685|0_._183|16_._518|3_._429|0_._371|3_._822|0_._394|0_._042|5_._330|0_._347|0_._037|5_._850|0_._218|0_._023|
|IHM-HARc|21_._154|1_._564|0_._170|15_._436|2_._321|0_._251|3_._930|0_._175|0_._019|5_._585|0_._607|0_._065|5_._739|0_._106|0_._011|
|IHMC-HARc|25_._277|5_._808|0_._630|15_._247|2_._122|0_._229|4_._076|2_._411|0_._260|5_._412|0_._430|0_._046|6_._429|0_._805|0_._087|
|HARc|19_._624|0_._000|0_._000|13_._190|0_._000|0_._000|3_._630|0_._000|0_._000|4_._985|0_._000|0_._000|5_._634|0_._000|0_._000|



_Note:_ This table shows the results of economic evaluation in terms of the portfolio return and the economic values of various HAR models for the 1-step forecasts, where _r_ refers to the portfolio return given a mild risk aversion rate ( _γ_ = 1) and EV denotes the _economic values_ of different HAR models with infinite Hidden Markov switching structures against the corresponding benchmark HAR model. 

30 

Table 8: Results of the economic evaluation for 5-step forecasts. 

|||Corn|||Cotton||I|ndica Ri|ce||Palm O|il||Soybea|n|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
||_r_|E<br>_γ_ = 1|V<br>_γ_ = 10|_r_|E<br>_γ_ = 1|V<br>_γ_ = 10|_r_|E<br>_γ_ = 1|V<br>_γ_ = 10|_r_|E<br>_γ_ = 1|V<br>_γ_ = 10|_r_|E<br>_γ_ = 1|V<br>_γ_ = 10|
|DIHM-HAR<br>IHM-HAR<br>IHMC-HAR<br>HAR|9_._238<br>8_._335<br>17_._104<br>13_._328|_−_4_._151 <br>_−_5_._053 <br>3_._891<br>0_._000|_−_0_._448<br> _−_0_._546<br>0_._421<br>0_._000|10_._140 <br>10_._766 <br>14_._834<br>10_._336|_−_3_._650 <br> _−_3_._975 <br>7_._896<br>0_._000|_−_0_._394<br> _−_0_._429<br>0_._853<br>0_._000|3_._454 <br>6_._992 <br>3_._038<br>3_._151|_−_3_._203 <br> _−_0_._689 <br>3_._106<br>0_._000|_−_0_._345<br> _−_0_._074<br>0_._335<br>0_._000|4_._589<br>4_._921<br>4_._447<br>4_._274|0_._317<br>0_._655<br>0_._174<br>0_._000|0_._034<br>0_._071<br>0_._019<br>0_._000|4_._529 <br>4_._490 <br>4_._806<br>4_._561|_−_0_._032 <br> _−_0_._072 <br>0_._247<br>0_._000|_−_0_._003<br> _−_0_._008<br>0_._027<br>0_._000|
|DIHM-HARCJ<br>IHM-HARCJ<br>IHMC-HARCJ<br>HARCJ|9_._171<br>8_._545<br>16_._846<br>13_._302|_−_4_._192 <br>_−_4_._818 <br>3_._660<br>0_._000|_−_0_._453<br> _−_0_._520<br>0_._396<br>0_._000|9_._993<br>10_._842 <br>14_._755<br>10_._362|_−_3_._847 <br> _−_3_._672 <br>7_._176<br>0_._000|_−_0_._415<br> _−_0_._397<br>0_._776<br>0_._000|3_._521 <br>3_._208 <br>8_._732<br>3_._132|_−_3_._133 <br> _−_4_._144 <br>8_._317<br>0_._000|_−_0_._338<br> _−_0_._447<br>0_._898<br>0_._000|2_._556 <br>4_._609<br>4_._508<br>4_._215|_−_1_._720 <br>0_._400<br>0_._296<br>0_._000|_−_0_._186<br>0_._043<br>0_._032<br>0_._000|4_._471 <br>4_._505 <br>4_._815<br>4_._569|_−_0_._099 <br> _−_0_._064 <br>0_._249<br>0_._000|_−_0_._011<br> _−_0_._007<br>0_._027<br>0_._000|
|DIHM-HARTCJ<br>IHM-HARTCJ<br>IHMC-HARTCJ<br>HARTCJ|8_._280<br>8_._077<br>16_._960<br>13_._302|_−_5_._100 <br>_−_5_._289 <br>3_._777<br>0_._000|_−_0_._551<br> _−_0_._571<br>0_._409<br>0_._000|10_._445 <br>10_._897 <br>14_._626<br>10_._744|_−_3_._706 <br> _−_4_._245 <br>6_._712<br>0_._000|_−_0_._400<br> _−_0_._458<br>0_._725<br>0_._000|3_._209 <br>3_._715 <br>3_._300<br>3_._185|_−_3_._428 <br> _−_3_._933 <br>2_._799<br>0_._000|_−_0_._369<br> _−_0_._424<br>0_._302<br>0_._000|5_._213<br>4_._495<br>4_._437<br>4_._222|1_._002<br>0_._276<br>0_._216<br>0_._000|0_._108<br>0_._030<br>0_._023<br>0_._000|4_._514 <br>4_._510 <br>4_._788<br>4_._709|_−_0_._197 <br> _−_0_._201 <br>0_._080<br>0_._000|_−_0_._021<br> _−_0_._022<br>0_._009<br>0_._000|
|DIHM-HAR∆J<br>IHM-HAR∆J|9_._723<br>8_._307|_−_3_._371 <br>_−_4_._793|_−_0_._364<br> _−_0_._518|10_._139 <br>10_._838|_−_3_._229 <br> _−_3_._755|_−_0_._349<br> _−_0_._406|2_._941 <br>3_._768|_−_3_._171 <br> _−_3_._599|_−_0_._342<br> _−_0_._388|4_._466<br>4_._778|0_._169<br>0_._486|0_._018<br>0_._052|4_._522 <br>4_._504|_−_0_._034 <br> _−_0_._052|_−_0_._004<br> _−_0_._006|
|IHMC-HAR∆J<br>HAR∆J|16_._667<br>13_._040|3_._738<br>0_._000|0_._405<br>0_._000|14_._680<br>10_._496|7_._363<br>0_._000|0_._796<br>0_._000|3_._265<br>3_._202|3_._083<br>0_._000|0_._332<br>0_._000|4_._411<br>4_._298|0_._113<br>0_._000|0_._012<br>0_._000|4_._784<br>4_._556|0_._231<br>0_._000|0_._025<br>0_._000|
|DIHM-HARspec<br>IHM-HARspec<br>IHMC-HARspec|9_._382<br>8_._271<br>17_._032|_−_3_._875 <br>_−_4_._989 <br>3_._954|_−_0_._419<br> _−_0_._539<br>0_._428|10_._247 <br>10_._870 <br>14_._910|_−_4_._614 <br> _−_5_._104 <br>6_._466|_−_0_._498<br> _−_0_._551<br>0_._699|4_._042 <br>2_._193 <br>3_._214|_−_2_._307 <br> _−_5_._289 <br>3_._169|_−_0_._249<br> _−_0_._570<br>0_._342|4_._628<br>4_._733<br>4_._440|0_._383<br>0_._490<br>0_._193|0_._041<br>0_._053<br>0_._021|4_._545 <br>4_._440 <br>4_._799|_−_0_._030 <br> _−_0_._136 <br>0_._227|_−_0_._003<br> _−_0_._015<br>0_._024|
|HARspec|13_._199|0_._000|0_._000|11_._634|0_._000|0_._000|3_._108|0_._000|0_._000|4_._249|0_._000|0_._000|4_._575|0_._000|0_._000|
|DIHM-HARc<br>IHM-HARc<br>IHMC-HARc|9_._914<br>8_._681<br>16_._734|_−_3_._447 <br>_−_4_._687 <br>3_._540|_−_0_._373<br> _−_0_._506<br>0_._383|10_._838 <br>11_._377 <br>13_._916|_−_3_._824 <br> _−_4_._453 <br>4_._515|_−_0_._413<br> _−_0_._481<br>0_._488|0_._265 <br>3_._944 <br>3_._059|_−_5_._619 <br> _−_3_._125 <br>2_._386|_−_0_._605<br> _−_0_._337<br>0_._257|5_._397<br>4_._562<br>4_._433|1_._174<br>0_._329<br>0_._198|0_._127<br>0_._035<br>0_._021|4_._539<br>4_._476 <br>4_._670|0_._038<br> _−_0_._026 <br>0_._171|0_._004<br> _−_0_._003<br>0_._018|
|HARc|13_._305|0_._000|0_._000|11_._843|0_._000|0_._000|3_._046|0_._000|0_._000|4_._237|0_._000|0_._000|4_._501|0_._000|0_._000|
|_Note:_ This table<br>models for the 5<br>_economic values_ o<br>model.|shows t<br>-step for<br>f differe|he resul<br>ecasts, <br>fnt HAR|ts of ec<br>where _r_ <br> models|onomic e<br> refers t<br>with infi|valuatio<br>o the p<br>inite Hid|n in ter<br>ortfolio <br>den Ma|ms of <br>return <br>rkov sw|the port<br> given a <br>itching|folio ret<br>mild ris<br>structur|urn an<br>k avers<br>es agai|d the ec<br>ion rate <br>nst the c|onomic <br> (_γ_ = 1<br>orrespo|values <br>) and <br>nding b|of variou<br>EV deno<br>enchmar|s HAR<br>tes the<br>k HAR|



Table 9: Results of the economic evaluation for 22-step forecasts. 

|||Corn|||Cotton||I|ndica Ri|ce||Palm O|il||Soybea|n|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
|||E|V||E|V||E|V||E|V||E|V|
||_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|_r_|_γ_ = 1|_γ_ = 10|
|DIHM-HAR|6_._517|_−_1_._734|_−_0_._187|9_._383|_−_1_._539|_−_0_._166|2_._932|_−_0_._841|_−_0_._091|4_._065|0_._197|0_._021|4_._153|_−_0_._135|_−_0_._015|
|IHM-HAR|6_._418|_−_1_._834|_−_0_._198|9_._338|_−_1_._702|_−_0_._184|2_._829|_−_1_._062|_−_0_._115|4_._016|0_._147|0_._016|4_._125|_−_0_._163|_−_0_._018|
|IHMC-HAR|10_._021|1_._845|0_._199|13_._452|4_._421|0_._478|2_._881|0_._964|0_._104|4_._041|0_._173|0_._019|4_._580|0_._297|0_._032|
|HAR|8_._223|0_._000|0_._000|9_._966|0_._000|0_._000|2_._799|0_._000|0_._000|3_._870|0_._000|0_._000|4_._286|0_._000|0_._000|
|DIHM-HARCJ|6_._562|_−_1_._549|_−_0_._167|9_._378|_−_1_._305|_−_0_._141|2_._867|_−_0_._784|_−_0_._085|4_._047|0_._148|0_._016|4_._141|_−_0_._175|_−_0_._019|
|IHM-HARCJ|6_._409|_−_1_._702|_−_0_._184|9_._312|_−_1_._577|_−_0_._170|3_._020|_−_0_._837|_−_0_._090|4_._055|0_._157|0_._017|4_._119|_−_0_._198|_−_0_._021|
|IHMC-HARCJ|10_._334|2_._310|0_._250|12_._009|3_._055|0_._330|1_._069|_−_1_._035|_−_0_._112|4_._055|0_._156|0_._017|4_._585|0_._274|0_._030|
|HARCJ|8_._085|0_._000|0_._000|9_._887|0_._000|0_._000|2_._822|0_._000|0_._000|3_._900|0_._000|0_._000|4_._315|0_._000|0_._000|
|DIHM-HARTCJ|6_._505|_−_1_._613|_−_0_._174|9_._339|_−_1_._853|_−_0_._200|2_._968|_−_0_._824|_−_0_._089|4_._079|0_._211|0_._023|4_._145|_−_0_._224|_−_0_._024|
|IHM-HARTCJ|6_._379|_−_1_._741|_−_0_._188|9_._333|_−_1_._924|_−_0_._208|3_._046|_−_0_._809|_−_0_._087|4_._085|0_._217|0_._023|4_._126|_−_0_._244|_−_0_._026|
|IHMC-HARTCJ|10_._783|2_._767|0_._299|13_._597|4_._492|0_._486|3_._340|1_._604|0_._173|4_._027|0_._158|0_._017|4_._774|0_._412|0_._044|
|HARTCJ|8_._092|0_._000|0_._000|10_._248|0_._000|0_._000|2_._840|0_._000|0_._000|3_._870|0_._000|0_._000|4_._367|0_._000|0_._000|
|DIHM-HARDJ|6_._433|_−_1_._819|_−_0_._196|9_._366|_−_1_._705|_−_0_._184|2_._990|_−_0_._961|_−_0_._104|4_._069|0_._192|0_._021|4_._147|_−_0_._138|_−_0_._015|
|IHM-HARDJ|6_._415|_−_1_._837|_−_0_._198|9_._331|_−_1_._779|_−_0_._192|3_._000|_−_0_._987|_−_0_._106|4_._062|0_._184|0_._020|4_._125|_−_0_._160|_−_0_._017|
|IHMC-HARDJ|10_._001|1_._824|0_._197|13_._344|4_._281|0_._463|2_._931|0_._962|0_._104|4_._044|0_._166|0_._018|4_._583|0_._303|0_._033|
|HARDJ|8_._223|0_._000|0_._000|9_._970|0_._000|0_._000|2_._829|0_._000|0_._000|3_._880|0_._000|0_._000|4_._283|0_._000|0_._000|
|DIHM-HARspec|6_._385|_−_1_._764|_−_0_._190|9_._361|_−_2_._725|_−_0_._294|2_._902|_−_0_._951|_−_0_._103|4_._078|0_._223|0_._024|4_._134|_−_0_._170|_−_0_._018|
|IHM-HARspec|6_._380|_−_1_._769|_−_0_._191|9_._322|_−_2_._782|_−_0_._300|3_._015|_−_0_._854|_−_0_._092|4_._078|0_._223|0_._024|4_._133|_−_0_._171|_−_0_._018|
|IHMC-HARspec|9_._952|1_._878|0_._203|13_._827|3_._720|0_._402|2_._774|0_._858|0_._093|4_._021|0_._164|0_._018|4_._634|0_._335|0_._036|
|HARspec|8_._122|0_._000|0_._000|10_._981|0_._000|0_._000|2_._739|0_._000|0_._000|3_._858|0_._000|0_._000|4_._303|0_._000|0_._000|
|DIHM-HARc|6_._517|_−_1_._574|_−_0_._170|9_._809|_−_4_._347|_−_0_._469|2_._750|_−_0_._830|_−_0_._090|4_._060|0_._192|0_._021|4_._144|_−_0_._084|_−_0_._009|
|IHM-HARc|6_._462|_−_1_._629|_−_0_._176|9_._726|_−_4_._457|_−_0_._481|2_._929|_−_0_._682|_−_0_._074|4_._085|0_._217|0_._023|4_._124|_−_0_._105|_−_0_._011|
|IHMC-HARc|9_._282|1_._247|0_._135|12_._489|_−_0_._193|_−_0_._021|2_._708|0_._590|0_._064|4_._000|0_._131|0_._014|4_._307|0_._080|0_._009|
|HARc|8_._065|0_._000|0_._000|13_._134|0_._000|0_._000|2_._601|0_._000|0_._000|3_._870|0_._000|0_._000|4_._227|0_._000|0_._000|



_Note:_ This table shows the results of economic evaluation in terms of the portfolio return and the economic values of various HAR models for the 22-step forecasts, where _r_ refers to the portfolio return given a mild risk aversion rate ( _γ_ = 1) and EV denotes the _economic values_ of different HAR models with infinite Hidden Markov switching structures against the corresponding benchmark HAR model. 

31 

Table 10: Robustness check for the 1-step forecasting precision results. 

||C|orn|Co|tton|Indic|a Rice|Pal|m Oil|Soy|bean|
|---|---|---|---|---|---|---|---|---|---|---|
||MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|
|DIHM-HAR|0_._201|0_._187<sup>_∗∗_</sup>|0_._109|0_._403<sup>_∗∗_</sup>|0_._024|0_._605<sup>_∗∗_</sup>|0_._019|0_._813<sup>_∗∗_</sup>|0_._083|0_._371<sup>_∗∗_</sup>|
|IHM-HAR|0_._195|0_._195|0_._093|0_._428|0_._038|0_._604<sup>_∗∗_</sup>|0_._034|0_._800<sup>_∗∗_</sup>|0_._088|0_._369<sup>_∗∗_</sup>|
|IHMC-HAR<br>HAR|0_._216<br>0_._203|0_._196<br>0_._194<sup>_∗_</sup>|0_._089<br>0_._083|0_._403<sup>_∗∗_</sup><br>0_._471|0_._028<br>0_._034|0_._610<sup>_∗∗_</sup><br>0_._621<sup>_∗∗_</sup>|0_._032<br>0_._023|0_._808<sup>_∗∗_</sup><br>0_._866_∗_|0_._076<br>0_._073|0_._390<br>0_._385|
|DIHM-HARCJ|0_._223|0_._187<sup>_∗∗_</sup>|0_._112|0_._401<sup>_∗∗_</sup>|0_._007|0_._635<sup>_∗∗_</sup>|0_._045|0_._793<sup>_∗∗_</sup>|0_._075|0_._373<sup>_∗∗_</sup>|
|IHM-HARCJ|0_._201|0_._193|0_._095|0_._419<sup>_∗_</sup>|0_._011|0_._632<sup>_∗∗_</sup>|0_._036|0_._803<sup>_∗∗_</sup>|0_._095|0_._371<sup>_∗∗_</sup>|
|IHMC-HARCJ|0_._225|0_._195<sup>_∗_</sup>|0_._082|0_._405<sup>_∗_</sup>|0_._000|0_._651|0_._044|0_._802<sup>_∗∗_</sup>|0_._095|0_._387|
|HARCJ|0_._212|0_._194<sup>_∗∗_</sup>|0_._068|0_._504|0_._009|0_._692|0_._028|0_._848<sup>_∗∗_</sup>|0_._109|0_._386|
|DIHM-HARTCJ|0_._214|0_._186<sup>_∗∗_</sup>|0_._090|0_._401<sup>_∗∗_</sup>|0_._005|0_._629<sup>_∗∗_</sup>|0_._028|0_._810<sup>_∗∗_</sup>|0_._105|0_._370<sup>_∗∗_</sup>|
|IHM-HARTCJ|0_._187|0_._192<sup>_∗_</sup>|0_._102|0_._418<sup>_∗_</sup>|0_._020|0_._615<sup>_∗∗_</sup>|0_._037|0_._798<sup>_∗∗_</sup>|0_._093|0_._370<sup>_∗∗_</sup>|
|IHMC-HARTCJ|0_._205|0_._193<sup>_∗∗_</sup>|0_._108|0_._398<sup>_∗∗_</sup>|0_._000|0_._647|0_._042|0_._804<sup>_∗∗_</sup>|0_._092|0_._387|
|HARTCJ|0_._211|0_._195<sup>_∗∗_</sup>|0_._095|0_._489|0_._003|0_._646<sup>_∗∗_</sup>|0_._031|0_._847<sup>_∗_</sup>|0_._098|0_._385|
|DIHM-HAR∆J|0_._216|0_._187<sup>_∗∗_</sup>|0_._216|0_._398<sup>_∗∗_</sup>|0_._007|0_._643<sup>_∗∗_</sup>|0_._016|0_._828<sup>_∗_</sup>|0_._081|0_._372<sup>_∗∗_</sup>|
|IHM-HAR∆J|0_._198|0_._193|0_._099|0_._423<sup>_∗_</sup>|0_._009|0_._642<sup>_∗∗_</sup>|0_._024|0_._807<sup>_∗∗_</sup>|0_._091|0_._370<sup>_∗∗_</sup>|
|IHMC-HAR∆J|0_._217|0_._196|0_._097|0_._396<sup>_∗∗_</sup>|0_._001|0_._640<sup>_∗∗_</sup>|0_._028|0_._812<sup>_∗∗_</sup>|0_._098|0_._387|
|HAR∆J|0_._200|0_._196|0_._092|0_._488|0_._002|0_._660|0_._036|0_._837<sup>_∗∗_</sup>|0_._098|0_._382<sup>_∗∗_</sup>|
|DIHM-HARspec|0_._223|0_._188<sup>_∗∗_</sup>|0_._109|0_._404<sup>_∗_</sup>|0_._013|0_._628<sup>_∗∗_</sup>|0_._020|0_._818<sup>_∗∗_</sup>|0_._059|0_._375<sup>_∗∗_</sup>|
|IHM-HARspec|0_._203|0_._193|0_._106|0_._407<sup>_∗_</sup>|0_._012|0_._634<sup>_∗∗_</sup>|0_._038|0_._805<sup>_∗∗_</sup>|0_._081|0_._371<sup>_∗∗_</sup>|
|IHMC-HARspec|0_._207|0_._199|0_._103|0_._404<sup>_∗_</sup>|0_._014|0_._623<sup>_∗∗_</sup>|0_._038|0_._807<sup>_∗∗_</sup>|0_._068|0_._393|
|HARspec|0_._209|0_._191<sup>_∗∗_</sup>|0_._095|0_._458|0_._008|0_._662<sup>_∗_</sup>|0_._027|0_._851<sup>_∗∗_</sup>|0_._070|0_._384|
|DIHM-HARc|0_._202|0_._190<sup>_∗∗_</sup>|0_._093|0_._399<sup>_∗∗_</sup>|0_._010|0_._609<sup>_∗∗_</sup>|0_._016|0_._815<sup>_∗∗_</sup>|0_._082|0_._373<sup>_∗∗_</sup>|
|IHM-HARc|0_._187|0_._192<sup>_∗∗_</sup>|0_._082|0_._415<sup>_∗_</sup>|0_._031|0_._606<sup>_∗∗_</sup>|0_._029|0_._795<sup>_∗∗_</sup>|0_._071|0_._368<sup>_∗∗_</sup>|
|IHMC-HARc|0_._220|0_._191<sup>_∗∗_</sup>|0_._090|0_._455|0_._034|0_._608<sup>_∗∗_</sup>|0_._035|0_._803<sup>_∗∗_</sup>|0_._077|0_._384<sup>_∗_</sup>|
|HARc|0_._203|0_._191<sup>_∗∗_</sup>|0_._080|0_._456|0_._037|0_._619<sup>_∗∗_</sup>|0_._024|0_._870<sup>_∗_</sup>|0_._074|0_._387|



_Note:_ Robustness check of the statistical evaluation for 1-step forecast based on the realized kernel estimator. The MAFE results marked with * and ** refer to corresponding MCS _p_ -values of _>_ 10% and _>_ 25%, respectively. 

Table 11: Robustness check for the economic evaluation for 1-step forecasts. 

|||Corn|||Cotton||I|ndica R|ice||Palm O|il||Soybean||
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
||_r_|E|V|_r_|E|V|_r_|E|V|_r_|E|V|_r_|E|V|
|||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10|
|DIHM-HAR<br>IHM-HAR<br>IHMC-HAR<br>HAR|18_._688<br>16_._818 <br>25_._589<br>15_._004|0_._582<br>_−_0_._194 <br>4_._121<br>0_._000|0_._063<br> _−_0_._021<br>0_._444<br>0_._000|13_._872<br>12_._656 <br>20_._301<br>10_._508|0_._582<br>_−_0_._194 <br>4_._121<br>0_._000|0_._063<br> _−_0_._021<br>0_._444<br>0_._000|2_._868<br>2_._919 <br>2_._903<br>2_._942|0_._582<br>_−_0_._194 <br>4_._121<br>0_._000|0_._063<br> _−_0_._021<br>0_._444<br>0_._000|6_._249<br>5_._995<br>6_._648<br>5_._681|5_._383<br>4_._908<br>6_._098<br>4_._602|0_._582<br>_−_0_._194<br>4_._121<br>0_._000|0_._063<br>_−_0_._021 <br>0_._444<br>0_._000|18_._688<br> 16_._818 <br>25_._589<br>15_._004|0_._582<br> _−_0_._194<br>4_._121<br>0_._000|
|DIHM-HARCJ|17_._245|0_._618|0_._067|13_._824|0_._618|0_._067|2_._979|0_._618|0_._067|6_._910|5_._125|0_._618|0_._067|17_._245|0_._618|
|IHM-HARCJ|16_._497|0_._473|0_._051|12_._437|0_._473|0_._051|2_._818|0_._473|0_._051|6_._788|4_._947|0_._473|0_._051|16_._497|0_._473|
|IHMC-HARCJ|23_._779|2_._757|0_._297|19_._144|2_._757|0_._297|2_._582|2_._757|0_._297|7_._557|5_._713|2_._757|0_._297|23_._779|2_._757|
|HARCJ|14_._540|0_._000|0_._000|10_._918|0_._000|0_._000|2_._830|0_._000|0_._000|6_._180|_−_6_._712|0_._000|0_._000|14_._540|0_._000|
|DIHM-HARTCJ|18_._423|0_._373|0_._040|14_._805|0_._373|0_._040|3_._232|0_._373|0_._040|6_._719|5_._228|0_._373|0_._040|18_._423|0_._373|
|IHM-HARTCJ|17_._075|_−_0_._233|_−_0_._025|12_._408|_−_0_._233|_−_0_._025|2_._878|_−_0_._233|_−_0_._025|6_._702|5_._001|_−_0_._233|_−_0_._025|17_._075|_−_0_._233|
|IHMC-HARTCJ|25_._764|3_._184|0_._343|19_._826|3_._184|0_._343|3_._552|3_._184|0_._343|7_._600|5_._925|3_._184|0_._343|25_._764|3_._184|
|HARTCJ|15_._140|0_._000|0_._000|11_._312|0_._000|0_._000|2_._947|0_._000|0_._000|6_._067|4_._687|0_._000|0_._000|15_._140|0_._000|
|DIHM-HAR∆J|17_._312|1_._763|0_._190|13_._824|2_._975|0_._322|2_._885|1_._763|0_._190|6_._188|5_._175|1_._763|0_._190|17_._312|1_._763|
|IHM-HAR∆J|16_._136|1_._396|0_._150|12_._417|1_._396|0_._150|2_._939|1_._396|0_._150|6_._289|4_._915|1_._396|0_._150|16_._136|1_._396|
|IHMC-HAR∆J|23_._101|4_._069|0_._438|29_._219|4_._069|0_._438|2_._834|4_._069|0_._438|6_._698|5_._728|4_._069|0_._438|23_._101|4_._069|
|HAR∆J|13_._029|0_._000|0_._000|10_._742|0_._000|0_._000|2_._703|0_._000|0_._000|6_._562|4_._381|0_._000|0_._000|13_._029|0_._000|
|DIHM-HARspec<br>|17_._988<br>|0_._988<br>|0_._106<br>|15_._125<br>|0_._988<br>|0_._106<br>|2_._928<br>|0_._988<br>|0_._106<br>|6_._154<br>|5_._176<br>|0_._988<br>|0_._106<br>|17_._988<br>|0_._988<br>|
|IHM-HARspec|17_._150|0_._728|0_._078|12_._649|0_._728|0_._078|2_._915|0_._728|0_._078|6_._214|5_._013|0_._728|0_._078|17_._150|0_._728|
|IHMC-HARspec|26_._172|5_._224|0_._563|20_._401|5_._224|0_._563|2_._830|5_._224|0_._563|6_._857|5_._880|5_._224|0_._563|26_._172|5_._224|
|HARspec|14_._104|0_._000|0_._000|11_._998|0_._000|0_._000|2_._917|0_._000|0_._000|5_._654|4_._708|0_._000|0_._000|14_._104|0_._000|
|DIHM-HARc|20_._951|0_._410|0_._044|17_._853|0_._410|0_._044|2_._953|0_._410|0_._044|6_._230|5_._279|0_._410|0_._044|20_._951|0_._410|
|IHM-HARc|17_._489|_−_0_._407|_−_0_._044|14_._720|_−_0_._407|_−_0_._044|3_._078|_−_0_._407|_−_0_._044|6_._319|4_._803|_−_0_._407|_−_0_._044|17_._489|_−_0_._407|
|IHMC-HARc<br>|23_._599|2_._049|0_._221|16_._858|2_._049|0_._221|2_._979|2_._049|0_._221|6_._552|5_._840|2_._049|0_._221|23_._599|2_._049|
|HARc|15_._903|0_._000|0_._000|12_._781|0_._000|0_._000|2_._946|0_._000|0_._000|5_._654|4_._534|0_._000|0_._000|15_._903|0_._000|



_Note:_ Robustness check of the economic evaluation for 1-step forecast based on the realized kernel estimators, where _r_ refers to the portfolio return given a mild risk aversion rate ( _γ_ = 1) and EV denotes the _economic values_ of different HAR models with infinite Hidden Markov switching structures against the corresponding benchmark HAR model. 

32 

Table 12: Robustness check for the 1-step forecasting precision results based on the MedRV estimator. 

||C|orn|Co|tton|Indic|a Rice|Pal|m Oil|So|ybean|
|---|---|---|---|---|---|---|---|---|---|---|
||MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|MZ-_R_<sup>2</sup>|MAFE|
|DIHM-HAR|0_._377|0_._142<sup>_∗∗_</sup>|0_._159|0_._290<sup>_∗∗_</sup>|0_._000|0_._675<sup>_∗∗_</sup>|0_._189|0_._371<sup>_∗_</sup>|0_._218|0_._240<sup>_∗∗_</sup>|
|IHM-HAR|0_._360|0_._144<sup>_∗_</sup>|0_._186|0_._290<sup>_∗∗_</sup>|0_._001|0_._654<sup>_∗∗_</sup>|0_._191|0_._367<sup>_∗_</sup>|0_._220|0_._235<sup>_∗∗_</sup>|
|IHMC-HAR<br>HAR|0_._387<br>0_._373|0_._145<sup>_∗_</sup><br>0_._144<sup>_∗_</sup>|0_._175<br>0_._167|0_._294<sup>_∗∗_</sup><br>0_._311<sup>_∗_</sup>|0_._000<br>0_._000|0_._708<sup>_∗∗_</sup><br>0_._749<sup>_∗∗_</sup>|0_._244<br>0_._233|0_._364<sup>_∗_</sup><br>0_._370<sup>_∗_</sup>|0_._219<br>0_._218|0_._251<sup>_∗∗_</sup><br>0_._238<sup>_∗∗_</sup>|
|DIHM-HARCJ|0_._417|0_._139<sup>_∗∗_</sup>|0_._209|0_._297<sup>_∗_</sup>|0_._000|0_._666<sup>_∗∗_</sup>|0_._266|0_._364<sup>_∗_</sup>|0_._196|0_._243<sup>_∗_</sup>|
|IHM-HARCJ|0_._387|0_._143<sup>_∗_</sup>|0_._259|0_._290<sup>_∗∗_</sup>|0_._003|0_._642<sup>_∗∗_</sup>|0_._179|0_._374<sup>_∗_</sup>|0_._227|0_._234<sup>_∗∗_</sup>|
|IHMC-HARCJ|0_._410|0_._146<sup>_∗_</sup>|0_._215|0_._306|0_._000|0_._650<sup>_∗∗_</sup>|0_._253|0_._372<sup>_∗_</sup>|0_._217|0_._254<sup>_∗∗_</sup>|
|HARCJ|0_._396|0_._142<sup>_∗∗_</sup>|0_._182|0_._332|0_._000|0_._823<sup>_∗_</sup>|0_._219|0_._380<sup>_∗_</sup>|0_._216|0_._240<sup>_∗∗_</sup>|
|DIHM-HARTCJ|0_._395|0_._139<sup>_∗∗_</sup>|0_._208|0_._286<sup>_∗∗_</sup>|0_._001|0_._643<sup>_∗∗_</sup>|0_._248|0_._368<sup>_∗_</sup>|0_._238|0_._237<sup>_∗∗_</sup>|
|IHM-HARTCJ|0_._097|0_._239|0_._227|0_._293<sup>_∗∗_</sup>|0_._002|0_._65<sup>_∗∗_</sup>|0_._218|0_._366<sup>_∗_</sup>|0_._229|0_._234<sup>_∗∗_</sup>|
|IHMC-HARTCJ|0_._383|0_._142<sup>_∗∗_</sup>|0_._238|0_._292<sup>_∗∗_</sup>|0_._000|0_._645<sup>_∗∗_</sup>|0_._237|0_._377<sup>_∗_</sup>|0_._236|0_._253|
|HARTCJ|0_._404|0_._141<sup>_∗∗_</sup>|0_._198|0_._323|0_._000|0_._781<sup>_∗∗_</sup>|0_._212|0_._384<sup>_∗_</sup>|0_._242|0_._240<sup>_∗∗_</sup>|
|DIHM-HAR∆J|0_._419|0_._137<sup>_∗∗_</sup>|0_._162|0_._291<sup>_∗∗_</sup>|0_._001|0_._635<sup>_∗∗_</sup>|0_._233|0_._372<sup>_∗_</sup>|0_._216|0_._234<sup>_∗∗_</sup>|
|IHM-HAR∆J|0_._385|0_._141<sup>_∗∗_</sup>|0_._197|0_._292<sup>_∗∗_</sup>|0_._001|0_._643<sup>_∗∗_</sup>|0_._210|0_._368<sup>_∗_</sup>|0_._222|0_._235<sup>_∗∗_</sup>|
|IHMC-HAR∆J|0_._417|0_._144<sup>_∗∗_</sup>|0_._172|0_._298<sup>_∗_</sup>|0_._000|0_._666<sup>_∗∗_</sup>|0_._254|0_._366<sup>_∗_</sup>|0_._235|0_._241<sup>_∗∗_</sup>|
|HAR∆J|0_._403|0_._140<sup>_∗∗_</sup>|0_._166|0_._313|0_._000|0_._674<sup>_∗∗_</sup>|0_._265|0_._369<sup>_∗_</sup>|0_._238|0_._237<sup>_∗∗_</sup>|
|DIHM-HARspec|0_._428|0_._139<sup>_∗∗_</sup>|0_._157|0_._289<sup>_∗∗_</sup>|0_._000|0_._679<sup>_∗∗_</sup>|0_._221|0_._366<sup>_∗_</sup>|0_._238|0_._234<sup>_∗∗_</sup>|
|IHM-HARspec|0_._392|0_._141<sup>_∗∗_</sup>|0_._155|0_._286<sup>_∗∗_</sup>|0_._000|0_._649<sup>_∗∗_</sup>|0_._186|0_._366<sup>_∗_</sup>|0_._203|0_._235<sup>_∗∗_</sup>|
|IHMC-HARspec|0_._399|0_._150|0_._168|0_._296<sup>_∗∗_</sup>|0_._000|0_._694<sup>_∗∗_</sup>|0_._231|0_._366<sup>_∗_</sup>|0_._234|0_._242<sup>_∗∗_</sup>|
|HARspec|0_._401|0_._140<sup>_∗∗_</sup>|0_._168|0_._290<sup>_∗∗_</sup>|0_._000|0_._758<sup>_∗∗_</sup>|0_._234|0_._370<sup>_∗_</sup>|0_._225|0_._237<sup>_∗∗_</sup>|
|DIHM-HARc|0_._363|0_._139<sup>_∗∗_</sup>|0_._161|0_._288<sup>_∗∗_</sup>|0_._001|0_._654<sup>_∗∗_</sup>|0_._189|0_._367<sup>_∗_</sup>|0_._234|0_._235<sup>_∗∗_</sup>|
|IHM-HARc|0_._372|0_._142<sup>_∗∗_</sup>|0_._157|0_._289<sup>_∗∗_</sup>|0_._003|0_._628<sup>_∗∗_</sup>|0_._177|0_._366<sup>_∗_</sup>|0_._221|0_._235<sup>_∗∗_</sup>|
|IHMC-HARc|0_._393|0_._145<sup>_∗_</sup>|0_._170|0_._304<sup>_∗_</sup>|0_._000|0_._715<sup>_∗∗_</sup>|0_._245|0_._362<sup>_∗∗_</sup>|0_._236|0_._240<sup>_∗∗_</sup>|
|HARc|0_._373|0_._143<sup>_∗∗_</sup>|0_._167|0_._306<sup>_∗_</sup>|0_._000|0_._758<sup>_∗∗_</sup>|0_._236|0_._370<sup>_∗_</sup>|0_._234|0_._238<sup>_∗∗_</sup>|



_Note:_ Robustness check of the statistical evaluation for 1-step forecast based on the realized kernel estimator. The MAFE results marked with * and ** refer to corresponding MCS _p_ -values of _>_ 10% and _>_ 25%, respectively. 

Table 13: Robustness check for the economic evaluation for 1-step forecasts based on the MedRV estimator. 

|||Corn|||Cotton||In|dica Ri|ce||Palm Oil|||Soybea|n|
|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|---|
||_r_|E|V|_r_|E|V|_r_|E|V|_r_|E|V|_r_|E|V|
|||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10||_γ_ = 1|_γ_ = 10|
|DIHM-HAR|24_._040|1_._586|0_._172|16_._287|2_._327|0_._252|4_._477|0_._342|0_._037|6_._226|0_._339|0_._037|7_._875|0_._376|0_._041|
|IHM-HAR|22_._775|0_._280|0_._030|14_._503|0_._495|0_._053|3_._978|_−_0_._469|_−_0_._051|6_._079|0_._191|0_._021|7_._223|_−_0_._286|_−_0_._031|
|IHMC-HAR|28_._032|5_._666|0_._615|24_._654|11_._108|1_._203|4_._606|3_._501|0_._377|6_._294|0_._409|0_._044|8_._849|1_._370|0_._148|
|HAR|22_._486|0_._000|0_._000|14_._012|0_._000|0_._000|4_._091|0_._000|0_._000|5_._888|0_._000|0_._000|7_._505|0_._000|0_._000|
|DIHM-HARCJ|24_._036|1_._778|0_._193|16_._150|1_._561|0_._169|6_._693|2_._473|0_._267|6_._053|0_._090|0_._010|8_._013|0_._444|0_._048|
|IHM-HARCJ|23_._045|0_._762|0_._083|14_._212|_−_0_._442|_−_0_._048|5_._343|0_._307|0_._033|6_._067|0_._106|0_._012|7_._217|_−_0_._364|_−_0_._039|
|IHMC-HARCJ|27_._649|5_._436|0_._590|20_._951|6_._582|0_._712|4_._986|3_._632|0_._391|6_._414|0_._456|0_._049|9_._041|1_._495|0_._161|
|HARCJ|22_._318|0_._000|0_._000|14_._608|0_._000|0_._000|4_._343|0_._000|0_._000|5_._962|0_._000|0_._000|7_._576|0_._000|0_._000|
|DIHM-HARTCJ|24_._404|1_._351|0_._147|16_._953|1_._822|0_._197|6_._009|1_._000|0_._108|6_._097|0_._107|0_._012|7_._597|0_._444|0_._048|
|IHM-HARTCJ|69_._746|47_._962|5_._269|15_._298|0_._125|0_._013|4_._107|2_._546|_−_1_._545|6_._125|0_._137|0_._015|6_._986|_−_0_._178|_−_0_._019|
|IHMC-HARTCJ|28_._119|5_._141|0_._558|21_._626|6_._747|0_._730|4_._972|2_._364|0_._255|6_._417|0_._431|0_._047|8_._412|1_._275|0_._138|
|HARTCJ|23_._150|0_._000|0_._000|15_._147|0_._000|0_._000|4_._182|0_._000|0_._000|5_._988|0_._000|0_._000|7_._161|0_._000|0_._000|
|DIHM-HAR∆J|23_._297|2_._158|0_._234|15_._288|0_._702|0_._076|4_._621|0_._506|0_._054|_−_0_._973|_−_6_._713|_−_0_._723|7_._365|0_._477|0_._051|
|IHM-HAR∆J|21_._864|0_._718|0_._078|14_._173|_−_0_._437|_−_0_._047|4_._569|_−_0_._882|_−_0_._095|5_._487|_−_0_._002|0_._000|6_._995|0_._099|0_._011|
|IHMC-HAR∆J|26_._453|5_._392|0_._585|13_._996|_−_1_._011|_−_0_._109|3_._755|1_._632|0_._176|6_._046|0_._565|0_._061|8_._194|1_._321|0_._143|
|HAR∆J|21_._163|0_._000|0_._000|14_._571|0_._000|0_._000|5_._033|0_._000|0_._000|5_._486|0_._000|0_._000|6_._896|0_._000|0_._000|
|DIHM-HARspec<br>|24_._723<br>|1_._884<br>|0_._204<br>|17_._139<br>|1_._845<br>|0_._199<br>|4_._998<br>|1_._252<br>|0_._135<br>|5_._948<br>|0_._088<br>|0_._009<br>|7_._185<br>|0_._206<br>|0_._022<br>|
|IHM-HARspec|23_._410|0_._545|0_._059|14_._687|_−_0_._673|_−_0_._073|0_._559|_−_3_._804|_−_0_._410|6_._075|0_._217|0_._023|7_._054|0_._074|0_._008|
|IHMC-HARspec|29_._830|7_._069|0_._768|20_._999|5_._870|0_._635|4_._694|4_._780|0_._515|6_._305|0_._449|0_._048|8_._103|1_._141|0_._123|
|HARspec|22_._868|0_._000|0_._000|15_._338|0_._000|0_._000|4_._223|0_._000|0_._000|5_._858|0_._000|0_._000|6_._982|0_._000|0_._000|
|DIHM-HARc|24_._284|1_._338|0_._145|19_._198|4_._419|0_._478|13_._815|8_._931|0_._964|5_._957|0_._113|0_._012|7_._054|0_._218|0_._023|
|IHM-HARc|22_._910|_−_0_._099|_−_0_._011|18_._203|3_._376|0_._365|5_._090|0_._690|0_._074|6_._003|0_._160|0_._017|6_._917|0_._079|0_._009|
|IHMC-HARc|28_._160|5_._301|0_._575|16_._268|1_._338|0_._145|4_._887|2_._956|0_._318|6_._239|0_._398|0_._043|7_._971|1_._152|0_._124|
|HARc|22_._994|0_._000|0_._000|14_._957|0_._000|0_._000|4_._097|0_._000|0_._000|5_._843|0_._000|0_._000|6_._840|0_._000|0_._000|



_Note:_ Robustness check of the economic evaluation for 1-step forecast based on the realized kernel estimators, where _r_ refers to the portfolio return given a mild risk aversion rate ( _γ_ = 1) and EV denotes the _economic values_ of different HAR models with infinite Hidden Markov switching structures against the corresponding benchmark HAR model. 

33 

## **8. Figures** 



Figure 1: Daily returns of the agricultural commodity futures from January 1, 2011 to December 31, 2015. 

34 



Figure 2: Daily realized volatilities of the agricultural commodity futures from January 1, 2011 to December 31, 2015. 



Figure 3: Speculation variable of the agricultural commodity futures from January 1, 2011 to December 31, 2015. 

35 







<!-- Start of picture text -->
(a) Corn<br><!-- End of picture text -->







<!-- Start of picture text -->
(b) Cotton<br><!-- End of picture text -->







<!-- Start of picture text -->
(c) Indica Rice<br><!-- End of picture text -->

36 







<!-- Start of picture text -->
(d) Palm<br><!-- End of picture text -->







<!-- Start of picture text -->
(e) Soybeans<br><!-- End of picture text -->

Figure 4: The posterior mean and distribution for the DIHM-HAR model. 

37 







<!-- Start of picture text -->
(a) Corn<br><!-- End of picture text -->







<!-- Start of picture text -->
(b) Cotton<br><!-- End of picture text -->





(c) Indica Rice 

38 







<!-- Start of picture text -->
(d) Palm<br><!-- End of picture text -->







<!-- Start of picture text -->
(e) Soybean<br><!-- End of picture text -->

Figure 5: The posterior mean and distribution for the IHM-HAR model. 

39 







<!-- Start of picture text -->
(a) Corn (b) Cotton<br>(c) Indica Rice (d) Palm Oil<br>(e) Soybean<br><!-- End of picture text -->

Figure 6: Heat maps for coefficients _β_ (left plot of each subplot (a) - (e)) and variances _σ_<sup>2</sup> (right plot of each subplot (a) - (e)) in the DIHM-HAR model for (a) Corn, (b) Cotton, (c) Indica Rice, (d) Palm Oil, and (e) Soybean. 

40 







<!-- Start of picture text -->
(a) Corn (b) Cotton<br>(c) Indica Rice (d) Palm Oil<br>(e) Soybeans<br><!-- End of picture text -->

Figure 7: Heat map in the IHM-HAR model for different agricultural commodity futures: (a) Corn, (b) Cotton, (c) Indica Rice, (d) Palm Oil, and (e) Soybeans. 

41 

