# Pseudo-Mathematics and Financial Charlatanism: The Effects of Backtest Overfitting on Out-of-Sample Performance 

_David H. Bailey, Jonathan M. Borwein, Marcos López de Prado, and Qiji Jim Zhu_ 

Another thing I must point out is that you cannot prove a vague theory wrong. […] Also, if the process of computing the consequences is indefinite, then with a little skill any experimental result can be made to look like the expected consequences. 

—Richard Feynman [1964] 

## Introduction 

A _backtest_ is a historical simulation of an algorithmic investment strategy. Among other things, it computes the series of profits and losses that such strategy would have generated had that algorithm been run over that time period. Popular performance statistics, such as the _Sharpe ratio_ or the _Information ratio_ , are used to quantify the backtested strategy’s return on risk. Investors typically study those backtest statistics and then allocate capital to the best performing scheme. 

Regarding the measured performance of a backtested strategy, we have to distinguish between two very different readings: _in-sample_ (IS) and _outof-sample_ (OOS). The IS performance is the one simulated over the sample used in the design of the strategy (also known as “learning period” or 

_David H. Bailey is retired from Lawrence Berkeley National Laboratory. He is a Research Fellow at the University of California, Davis, Department of Computer Science. His email address is_ `david@davidhbailey.com` _._ 

_Jonathan M. Borwein is Laureate Professor of Mathematics at the University of Newcastle, Australia, and a Fellow of the Royal Society of Canada, the Australian Academy of Science, and the AAAS. His email address is_ `jonathan.borwein@ newcastle.edu.au` _._ 

_Marcos López de Prado is Senior Managing Director at Guggenheim Partners, New York, and Research Affiliate at Lawrence Berkeley National Laboratory. His email address is_ `lopezdeprado@lbl.gov` _._ 

_Qiji Jim Zhu is Professor of Mathematics at Western Michigan University. His email address is_ `zhu@wmich.edu` _._ DOI: http://dx.doi.org/10.1090/noti1105 

“training set” in the machine-learning literature). The OOS performance is simulated over a sample not used in the design of the strategy (a.k.a. “testing set”). A backtest is _realistic_ when the IS performance is consistent with the OOS performance. 

When an investor receives a promising backtest from a researcher or portfolio manager, one of her key problems is to assess how realistic that simulation is. This is because, given any financial series, it is relatively simple to _overfit_ an investment strategy so that it performs well IS. 

Overfitting is a concept borrowed from machine learning and denotes the situation when a model targets particular observations rather than a general structure. For example, a researcher could design a trading system based on some parameters that target the removal of specific recommendations that she knows led to losses IS (a practice known as “data snooping”). After a few iterations, the researcher will come up with “optimal parameters”, which profit from features that are present in that particular sample but may well be rare in the population. 

Recent computational advances allow investment managers to methodically search through thousands or even millions of potential options for a profitable investment strategy. In many instances, that search involves a pseudo-mathematical argument which is spuriously validated through a backtest. For example, consider a time series of daily prices for a stock _X_ . For every day in the sample, we can compute one average price of that stock using the previous _m_ observations _x_ ¯ _m_ and another average price using the previous _n_ observations _x_ ¯ _n_ , where _m < n_ . A popular investment strategy called “crossing moving averages” consists of owning _X_ whenever _x_ ¯ _m > x_ ¯ _n_ . Indeed, since the sample size determines a limited number of parameter combinations that _m_ and _n_ can adopt, 

458 

Notices of the AMS 

Volume 61, Number 5 

it is relatively easy to determine the pair _(m, n)_ that maximizes the backtest’s performance. There are hundreds of such popular strategies, marketed to unsuspecting lay investors as mathematically sound and empirically tested. 

In the context of econometric models several procedures have been proposed to determine overfit in White [27], Romano et al. [23], and Harvey et al. [9]. These methods propose to adjust the _p_ -values of estimated regression coefficients to account for the multiplicity of trials. These approaches are valuable for dealing with trading rules based on an econometric specification. 

The machine-learning literature has devoted significant effort to studying the problem of overfitting. The proposed methods typically are not applicable to investment problems for multiple reasons. First, these methods often require explicit point forecasts and confidence bands over a defined event horizon in order to evaluate the explanatory power or quality of the prediction (e.g., “E-mini S&P500 is forecasted to be around 1,600 with a one-standard deviation of 5 index points at Friday’s close”). Very few investment strategies yield such explicit forecasts; instead, they provide qualitative recommendations (e.g., “buy” or “strong buy”) over an undefined period until another such forecast is generated, with random frequency. For instance, trading systems, like the crossing of moving averages explained earlier, generate buy and sell recommendations with little or no indication as to forecasted values, confidence in a particular recommendation, or expected holding period. 

Second, even if a particular investment strategy relies on such a forecasting equation, other components of the investment strategy may have been overfitted, including entry thresholds, risk sizing, profit taking, stop-loss, cost of capital, and so on. In other words, there are many ways to overfit an investment strategy other than simply tuning the forecasting equation. Third, regression overfitting methods are parametric and involve a number of assumptions regarding the underlying data which may not be easily ascertainable. Fourth, some methods do not control for the number of trials attempted. 

To illustrate this point, suppose that a researcher is given a finite sample and told that she needs to come up with a strategy with an SR (Sharpe Ratio, a popular measure of performance in the presence of risk) above 2.0, based on a forecasting equation for which the AIC statistic (Akaike Information Criterion, a standard of the regularization method) rejects the null hypothesis of overfitting with a 95 percent confidence level (i.e., a false positive rate of 5 percent). After only twenty trials, the researcher is expected to find one specification 

that passes the AIC criterion. The researcher will quickly be able to present a specification that not only (falsely) passes the AIC test but also gives an SR above 2.0. The problem is, AIC’s assessment did not take into account the hundreds of other trials that the researcher neglected to mention. For these reasons, commonly used regression overfitting methods are poorly equipped to deal with backtest overfitting. 

Although there are many academic studies that claim to have identified profitable investment strategies, their reported results are almost always based on IS statistics. Only exceptionally do we find an academic study that applies the “hold-out” method or some other procedure to evaluate performance OOS. Harvey, Liu, and Zhu [10] argue that there are hundreds of papers supposedly identifying hundreds of factors with explanatory power over future stock returns. They echo Ioannidis [13] in concluding that “most claimed research findings are likely false.” Factor models are only the tip of the iceberg.<sup>1</sup> The reader is probably familiar with many publications solely discussing IS performance. 

This situation is, quite frankly, depressing, particularly because academic researchers are expected to recognize the dangers and practice of overfitting. One common criticism, of course, is the credibility problem of “holding-out” when the researcher had access to the full sample anyway. Leinweber and Sisk [15] present a meritorious exception. They proposed an investment strategy in a conference and announced that six months later they would publish the results with the pure (yet to be observed) OOS data. They called this approach “model sequestration”, which is an extreme variation of “hold-out”. 

## Our Intentions 

In this paper we shall show that it takes a relatively small number of trials to identify an investment strategy with a spuriously high backtested performance. We also compute the _minimum backtest length_ (MinBTL) that an investor should require given the number of trials attempted. Although in our examples we always choose the Sharpe ratio to evaluate performance, our methodology can be applied to any other performance measure. 

We believe our framework to be helpful to the academic and investment communities by providing a benchmark methodology to assess the reliability of a backtested performance. We would 

> 1 _We invite the reader to read specific instances of pseudomathematical financial advice at this website:_ `http://www. m-a-f-f-i-a.org/` _. Also, Edesses (2007) provides numerous examples._ 

May 2014 

Notices of the AMS 

459 



Figure 1. Overfitting a backtest’s results as the number of trials grows. 

_Figure 1 provides a graphical representation of Proposition 1. The blue (dotted) line shows the maximum of a particular set of N independent random numbers, each following a Standard Normal distribution. The black (continuous) line is the expected value of the maximum of that set of N random numbers. The red (dashed) line is an upper bound estimate of that maximum. The implication is that it is relatively easy to wrongly select a strategy on the basis of a maximum Sharpe ratio when displayed IS._ 

feel sufficiently rewarded in our efforts if at least this paper succeeded in drawing the attention of the mathematical community regarding the widespread proliferation of journal publications, many of them claiming profitable investment strategies on the sole basis of IS performance. This is perhaps understandable in business circles, but a higher standard is and should be expected from an academic forum. 

We would also like to raise the question of whether mathematical scientists should continue to tolerate the proliferation of investment products that are misleadingly marketed as mathematically sound. In the recent words of Sir Andrew Wiles, 

One has to be aware now that mathematics can be misused and that we have to protect its good name. [29] 

We encourage the reader to search the Internet for terms such as “stochastic oscillators”, “Fibonacci ratios”, “cycles”, “Elliot wave”, “Golden ratio”, “parabolic SAR”, “pivot point”, “momentum”, and others in the context of finance. Although such terms clearly evoke precise mathematical concepts, in fact in almost all cases their usage is scientifically unsound. 

Historically, scientists have led the way in exposing those who utilize pseudoscience to extract a commercial benefit. As early as the eighteenth century, physicists exposed the nonsense of astrologers. Yet mathematicians in the twenty-first century have remained disappointingly silent with regard to those in the investment community who, knowingly or not, misuse mathematical techniques such as probability theory, statistics, and stochastic calculus. Our silence is consent, making us accomplices in these abuses. 

The rest of our study is organized as follows: The section “Backtest Overfitting” introduces the problem in a more formal way. The section “Minimum Backtest Length (MinBTL)” defines the concept of Minimum Backtest Length (MinBTL). The section “Model Complexity” argues how model complexity leads to backtest overfitting. The section “Overfitting in Absence of Compensation Effects” analyzes overfitting in the absence of compensation effects. The section “Overfitting in Presence of Compensation Effects” studies overfitting in the presence of compensation effects. The section “Is Backtest Overfitting a Fraud?” exposes how backtest overfitting can be used to commit fraud. The section “A Practical Application” presents a typical example of backtest overfitting. The section “Conclusions” lists our conclusions. The mathematical appendices supply proofs of the propositions presented throughout the paper. 

## Backtest Overfitting 

The design of an investment strategy usually begins with a _prior_ or _belief_ that a certain pattern may help forecast the future value of a financial variable. For example, if a researcher recognizes a lead-lag effect between various tenor bonds in a yield curve, she could design a strategy that bets on a reversion towards equilibrium values. This model might take the form of a cointegration equation, a vector-error correction model, or a system of stochastic differential equations, just to name a few. The number of possible model configurations (or trials) is enormous, and naturally the researcher would like to select the one that maximizes the performance of the strategy. Practitioners often rely on _historical simulations_ (also called backtests) to discover the optimal specification of an investment strategy. The researcher will evaluate, among other variables, what are the optimal sample sizes, signal update frequency, entry and profit-taking thresholds, risk sizing, stop losses, maximum holding periods, etc. 

The _Sharpe ratio_ is a statistic that evaluates an investment manager’s or strategy’s performance on the basis of a sample of past returns. It is defined as the ratio between average excess returns (in excess of the rate of return paid by a risk-free asset, such as 

460 

Notices of the AMS 

Volume 61, Number 5 

a government note) and the standard deviation of the same returns. Intuitively, this can be interpreted as a “return on risk” (or as William Sharpe put it, “return on variability”). But the standard deviation of excess returns may be a misleading measure of variability when returns follow asymmetric or fat-tailed distributions or when returns are not independent or identically distributed. Suppose that a strategy’s excess returns (or risk premiums), _rt_ , are independent and identically distributed (IID) following a Normal law: 



where _N_ represents a Normal distribution with mean _µ_ and variance _σ_<sup>2</sup> . The _annualized_ Sharpe ratio (SR) can be computed as 



where _q_ is the number of returns per year (see Lo [17] for a detailed derivation of this expression). Sharpe ratios are typically expressed in annual terms in order to allow for the comparison of strategies that trade with different frequency. The great majority of financial models are built upon the IID Normal assumption, which may explain why the Sharpe ratio has become the most popular statistic for evaluating an investment’s performance. 

Since _µ, σ_ are usually unknown, the true value SR cannot be known for certain. Instead, we can estimate the Sharpe ratio as _SR_<sup>�</sup> _= σ_<sup>_<u>µ</u>_</sup> ˆ<sup>ˆ</sup> _~~√~~_ _<u>q</u>_ <u>,</u> where _µ_ ˆ and _σ_ ˆ are the sample mean and sample standard deviation. The inevitable consequence is that SR calculations are likely to be the subject of substantial estimation errors (see Bailey and López de Prado [2] for a confidence band and an extension of the concept of Sharpe Ratio beyond the IID Normal assumption). 

From Lo [17] we know that the distribution of the estimated annualized Sharpe ratio _SR_<sup>�</sup> converges asymptotically (as _y →∞_ ) to 



where _SR_ �.<sup>2</sup> As _y_ is _y_ theincreasesnumberwithoutof yearsbound,used totheestimateprobability distribution of _SR_<sup>�</sup> approaches a Normal 1 _+_<sup>_<u>SR</u>_2</sup> distribution with mean SR and variance 2 _<u>q</u>_ . For a _y_ 

> 2 _Most performance statistics assume IID Normal returns and so are normally distributed. In the case of the Sharpe ratio, several authors have proved that its asymptotic distribution follows a Normal law even when the returns are not IID Normal. The same result applies to the Information Ratio. The only requirement is that the returns be ergodic. We refer the interested reader to Bailey and López de Prado_ [2] _._ 



Figure 2. Minimum Backtest Length needed to avoid overfitting, as a function of the number of trials. 

_Figure 2 shows the tradeoff between the number of trials (N) and the minimum backtest length (MinBTL) needed to prevent skill-less strategies to be generated with a Sharpe ratio IS of_ 1 _. For instance, if only five years of data are available, no more than forty-five independent model configurations should be tried. For that number of trials, the expected maximum SR IS is_ 1 _, whereas the expected SR OOS is_ 0 _. After trying only seven independent strategy configurations, the expected maximum SR IS is_ 1 _for a two-year long backtest, while the expected SR OOS is_ 0 _. The implication is that a backtest which does not report the number of trials N used to identify the selected configuration makes it impossible to assess the risk of overfitting._ 

sufficiently large _y_ , (3) provides an approximation of the distribution of _SR_<sup>�</sup> . 

Even for a small number _N_ of trials, it is relatively easy to find a strategy with a high Sharpe ratio IS but which also delivers a null Sharpe ratio OOS. To illustrate this point, consider _N_ strategies with _T = yq_ returns distributed according to a Normal law with mean excess returns _µ_ and with standard deviation _σ_ . Suppose that we would like to select the strategy with optimal _SR_<sup>�</sup> IS, based on one year of observations. A risk we face is choosing a strategy with a high Sharpe ratio IS but zero Sharpe ratio OOS. So we ask the question, _how high is the expected maximum Sharpe ratio IS among a set of strategy configurations where the true Sharpe ratio is zero?_ 

Bailey and López de Prado [2] derived an estimate of the _Minimum Track Record Length (MinTRL)_ needed to reject the hypothesis that an estimated Sharpe ratio is below a certain threshold 

May 2014 

Notices of the AMS 

461 



Figure 3. Performance IS vs. OOS before introducing strategy selection. 

_Figure 3 shows the relation between SR IS (xaxis) and SR OOS (y-axis) for µ =_ 0 _, σ =_ 1 _, N =_ 1000 _, T =_ 1000 _. Because the process follows a random walk, the scatter plot has a circular shape centered at the point (_ 0 _,_ 0 _). This illustrates the fact that, in absence of compensation effects, overfitting the IS performance (x-axis) has no bearing on the OOS performance (y-axis), which remains around zero._ 

(let’s say zero). MinTRL was developed to evaluate a strategy’s track record (a single realized path, _N =_ 1). The question we are asking now is different, because we are interested in the backtest length needed to avoid selecting a skill-less strategy among _N_ alternative specifications. In other words, in this article we are concerned with overfitting prevention when comparing multiple strategies, not in evaluating the statistical significance of a single Sharpe ratio estimate. Next, we will derive the analogue to MinTRL in the context of overfitting, which we will call _Minimum Backtest Length (MinBTL)_ , since it specifically addresses the problem of backtest overfitting. 

From (3), if _µ =_ 0 and _y =_ 1, then _SR_<sup>�</sup> � _→Na (_ 0 _,_ 1 _)_ . Note that because _SR =_ 0, increasing _q_ does not reduce the variance of the distribution. The proof of the following proposition is left for the appendix. 

Proposition 1. _Given a sample of IID random variables, xn ∼ Z, n =_ 1 _, . . . , N, where Z is the CDF of the Standard Normal distribution, the expected maximum of that sample, E[_ max _N ] = E[_ max _{xn}], can be approximated for a large N as_ 



_where γ ≈_ 0 _._ 5772156649 _. . . is the Euler-Mascheroni constant and N ≫_ 1 _._ 

An upper bound to (4) is ~~�~~ 2 ln _[N]_ .<sup>3</sup> Figure 1 plots, for various values of _N_ ( _x_ -axis), the expected Sharpe ratio of the optimal strategy IS. For example, if the researcher tries only _N =_ 10 alternative configurations of an investment strategy, she is expected to find a strategy with a Sharpe ratio IS of 1.57 despite the fact that all strategies are expected to deliver a Sharpe ratio of zero OOS (including the “optimal” one selected IS). 

Proposition 1 has important implications. As the researcher tries a growing number of strategy configurations, there will be a nonnull probability of selecting IS a strategy with null expected performance OOS. Because the hold-out method does not take into account the number of trials attempted before selecting a model, it cannot assess the representativeness of a backtest. 

## Minimum Backtest Length (MinBTL) 

Let us consider now the case that _µ =_ 0 but _y_ ≠ 1. Then, we can still apply Proposition 1 by rescaling the expected maximum by the standard deviation of the annualized Sharpe ratio, _y_<sup>_−_1</sup><sup>_/_2</sup> . Thus, the researcher is expected to find an “optimal” strategy with an IS annualized Sharpe ratio of 





Equation (5) says that the more independent the configurations a researcher tries _(N)_ , the more likely she is to overfit, and therefore the higher the acceptance threshold should be for the backtested result to be trusted. This situation can be partially mitigated by increasing the sample size _(y)_ . By solving (5) for _y_ , we obtain the following statement. 

Theorem 2. _The Minimum Backtest Length (MinBTL, in years) needed to avoid selecting a strategy with an IS Sharpe ratio of E[_ max _N ] among N independent strategies with an expected OOS Sharpe ratio of zero is_ (6) 

_MinBTL_ 



Equation (6) tells us that MinBTL must grow as the researcher tries more independent model configurations ( _N_ ) in order to keep constant the expected maximum Sharpe ratio at a given level 

> 3 _See Example 3.5.4 of Embrechts et al._ [5] _for a detailed treatment of the derivation of upper bounds on the maximum of a Normal distribution._ 

462 

Notices of the AMS 

Volume 61, Number 5 

_E[_ max _N ]_ . Figure 2 shows how many years of backtest length (MinBTL) are needed so that _E[_ max _N ]_ is fixed at 1. For instance, if only five years of data are available, no more than forty-five independent model configurations should be tried or we are almost guaranteed to produce strategies with an annualized Sharpe ratio IS of 1 but an expected Sharpe ratio OOS of zero. Note that Proposition 1 assumed the _N_ trials to be independent, which leads to a quite conservative estimate. If the trials performed were not independent, the number of independent trials _N_ involved could be derived using a dimension-reduction procedure, such as Principal Component Analysis. 

We will examine this tradeoff between _N_ and _T_ in greater depth later in the paper without requiring such a strong assumption, but MinBTL gives us a first glance at how easy it is to overfit by merely trying alternative model configurations. As an approximation, the reader may find it helpful to remember the upper bound to the minimum 2 ln _<u>[N]</u>_ backtest length (in years), _MinBTL <_ ~~2~~<sup>.</sup> _E[_ max _N ]_ 

Of course, a backtest may be overfit even if it is computed on a sample greater than MinBTL. From that perspective, MinBTL should be considered a necessary, nonsufficient condition to avoid overfitting. We leave to Bailey et al. [1] the derivation of a more precise measure of backtest overfitting. 



Figure 4. Performance IS vs. performance OOS for one path after introducing strategy selection. 

_Figure 4 provides a graphical representation of what happens when we select the random walk with highest SR IS. The performance of the first half was optimized IS, and the performance of the second half is what the investor receives OOS. The good news is, in the absence of memory, there is no reason to expect overfitting to induce negative performance._ 

maximum Sharpe ratio above 2.6 (see Figure 1). 

## Model Complexity 

How does the previous result relate to model complexity? Consider a one-parameter model that may adopt two possible values (like a switch that generates a random sequence of trades) on a sample of _T_ observations. Overfitting will be difficult, because _N =_ 2. Let’s say that we make the model more complex by adding four more parameters so that the total number of parameters becomes 5, i.e., _N =_ 2<sup>5</sup> _=_ 32. Having thirty-two independent sequences of random trades greatly increases the possibility of overfitting. 

While a greater _N_ makes overfitting easier, it makes perfectly fitting harder. Modern supercomputers can only perform around 2<sup>50</sup> raw computations per second, or less than 2<sup>58</sup> raw computations per year. Even if a trial could be reduced to a raw computation, searching _N =_ 2<sup>100</sup> will take us 2<sup>42</sup> supercomputer-years of computation (assuming a 1 Pflop/s system, capable of 10<sup>15</sup> floating-point operations per second). Hence, a skill-less brute force search is certainly impossible. While it is hard to perfectly fit a complex skill-less strategy, Proposition 1 shows that there is no need for that. Without perfectly fitting a strategy or making it overcomplex, a researcher can achieve high Sharpe ratios. A relatively simple strategy with just seven binomial independent parameters offers _N =_ 2<sup>7</sup> _=_ 128 trials, with an expected 

We suspect, however, that backtested strategies that significantly beat the market typically rely on some combination of valid insight, boosted by some degree of overfitting. Since believing in such an artificially enhanced high-performance strategy will often also lead to over leveraging, such overfitting is still very damaging. Most Technical Analysis strategies rely on filters, which are sets of conditions that trigger trading actions, like the random switches exemplified earlier. Accordingly, extra caution is warranted to guard against overfitting in using Technical Analysis strategies, as well as in complex nonparametric modeling tools, such as Neural Networks and Kernel Estimators. 

Here is a key concept that investors generally miss: 

_A researcher that does not report the number of trials N used to identify the selected backtest configuration makes it impossible to assess the risk of overfitting._ 

Because _N_ is almost never reported, the magnitude of overfitting in published backtests is unknown. It is not hard to overfit a backtest (indeed, the previous theorem shows that it is hard _not to_ ), so we suspect that a large proportion of backtests published in academic journals may be misleading. The 

May 2014 

Notices of the AMS 

463 



Figure 5. Performance degradation after introducing strategy selection in absence of compensation effects. 

_Figure 5 illustrates what happens once we add a “model selection” procedure. Now the SR IS ranges from_ 1.2 _to_ 2.6 _, and it is centered around_ 1.7 _. Although the backtest for the selected model generates the expectation of a_ 1.7 _SR, the expected SR OOS is unchanged and lies around_ 0 _._ 

situation is not likely to be better among practitioners. 

In our experience, overfitting is pathological within the financial industry, where proprietary and commercial software is developed to estimate the combination of parameters that best fits (or, more precisely, overfits) the data. These tools allow the user to add filters without ever reporting how such additions increase the probability of backtest overfitting. Institutional players are not immune to this pitfall. Large mutual fund groups typically discontinue and replace poorly performing funds, introducing _survivorship_ and selection bias. While the motivation of this practice may be entirely innocent, the effect is the same as that of hiding experiments and inflating expectations. 

We are not implying that those technical analysts, quantitative researchers, or fund managers are “snake oil salesmen”. Most likely they most genuinely believe that the backtested results are legitimate or that adjusted fund offerings better represent future performance. Hedge fund managers are often unaware that most backtests presented to them by researchers and analysts may be useless, and so they unknowingly package faulty investment propositions into products. One goal of this paper is to make investors, practitioners, and academics aware of the futility of considering backtest without controlling for the probability of overfitting. 

## Overfitting in Absence of Compensation Effects 

Regardless of how realistic the prior being tested is, there is always a combination of parameters that is optimal. In fact, even if the prior is false, the researcher is very likely to identify a combination of parameters that happens to deliver an outstanding performance IS. But because the prior is false, OOS performance will almost certainly underperform the backtest’s results. As we have described, this phenomenon, by which IS results tend to outperform the OOS results, is called overfitting. It occurs because a sufficiently large number of parameters are able to target specific data points— say by chance buying just before a rally and shorting a position just before a sell-off—rather than triggering trades according to the prior. 

To illustrate this point, suppose we generate _N_ Gaussian random walks by drawing from a Standard Normal distribution, each walk having a size _T_ . Each performance path _mτ_ can be obtained as a cumulative sum of Gaussian draws 

(7) ∆ _mτ = µ + σετ ,_ 

where the _random shocks ετ_ are IID distributed _ετ ∼ Z, τ =_ 1 _, . . . , T_ . Suppose that each path has been generated by a particular combination of parameters, backtested by a researcher. Without loss of generality, assume that _µ =_ 0 _, σ =_ 1, and _T =_ 1000, covering a period of one year (with about four observations per trading day). We divide these paths into two disjoint samples of equal size, 500, and call the first one IS and the second one OOS. 

At the moment of choosing a particular parameter combination as optimal, the researcher had access to the IS series, not the OOS. For each model configuration, we may compute the Sharpe ratio of the series IS and compare it with the Sharpe ratio of the series OOS. Figure 3 shows the resulting scatter plot. The _p_ -values associated with the intercept and the IS performance (SR a priori) are respectively 0.6261 and 0.7469. 

The problem of overfitting arises when the researcher uses the IS performance (backtest) to choose a particular model configuration, with the expectation that configurations that performed well in the past will continue to do so in the future. This would be a correct assumption if the parameter configurations were associated with a truthful prior, but this is clearly not the case of the simulation above, which is the result of Gaussian random walks without trend ( _µ =_ 0). 

Figure 4 shows what happens when we select the model configuration associated with the random walk with highest Sharpe ratio IS. The performance of the first half was optimized IS, and the performance of the second half is what the investor receives OOS. The good news is that under these 

464 

Notices of the AMS 

Volume 61, Number 5 

conditions, there is no reason to expect overfitting to induce negative performance. This is illustrated in Figure 5, which shows how the optimization causes the expected performance IS to range between 1.2 and 2.6, while the OOS performance will range between -1.5 and 1.5 (i.e., around _µ_ , which in this case is zero). The _p_ -values associated with the intercept and the IS performance (SR a priori) are respectively 0.2146 and 0.2131. Selecting an optimal model IS had no bearing on the performance OOS, which simply equals the zero mean of the process. A positive mean _(µ >_ 0 _)_ would lead to positive expected performance OOS, but such performance would nevertheless be inferior to the one observed IS. 

## Overfitting in Presence of Compensation Effects 

Multiple causes create compensation effects in practice, such as overcrowded investment opportunities, major corrections, economic cycles, reversal of financial flows, structural breaks, bubble bursts, etc. Optimizing a strategy’s parameters (i.e., choosing the model configuration that maximizes the strategy’s performance IS) does not necessarily lead to improved performance (compared to not optimizing) OOS, yet again leading to overfitting. 

In some instances, when the strategy’s performance series lacks memory, overfitting leads to no improvement in performance OOS. However, the presence of memory in a strategy’s performance series induces a compensation effect, which increases the chances for that strategy to be selected IS, only to underperform the rest OOS. Under those circumstances, IS backtest optimization is in fact detrimental to OOS performance.<sup>4</sup> 

## Global Constraint 

Unfortunately, overfitting rarely has the neutral implications discussed in the previous section. Our previous example was purposely chosen to exhibit a globally unconditional behavior. As a result, the OOS data had no memory of what occurred IS. Centering each path to match a mean _µ_ removes one degree of freedom: 





Figure 6. Performance degradation as a result of strategy selection under compensation effects (global constraint). 

_Figure 6 shows that adding a single global constraint causes the OOS performance to be negative even though the underlying process was trendless. Also, a strongly negative linear relation between performance IS and OOS arises, indicating that the more we optimize IS, the worse the OOS performance of the strategy._ 

We may rerun the same Monte Carlo experiment as before, this time on the recentered variables ∆ _mτ_ . Somewhat scarily, adding this single global constraint causes the OOS performance to be negative even though the underlying process was trendless. Moreover, a strongly negative linear relation between performance IS and OOS arises, indicating that the more we optimize IS, the worse the OOS performance. Figure 6 displays this disturbing pattern. The _p_ -values associated with the intercept and the IS performance (SR a priori) are respectively 0.5005 and 0, indicating that the negative linear relation between IS and OOS Sharpe ratios is statistically significant. 

The following proposition is proven in the appendix. 

Proposition 3. _Given two alternative configurations (A and B) of the same model, where σIS_<sup>_A=_</sup> _σOOS_<sup>_A=σ_</sup> _IS_<sup>_B=σ B_</sup> _OOS_<sup>_imposingaglobalconstraint_</sup> _µ_<sup>_A_</sup> _= µ_<sup>_B_</sup> _, implies that_ 

(9) _SRIS_<sup>_A> SR_</sup> _IS_<sup>_B_�</sup><sup>_SR_</sup> _OOS_<sup>_A< SR_</sup> _OOS_<sup>_B._</sup> 

> 4 _Bailey et al._ [1] _propose a method to determine the degree to which a particular backtest may have been compromised by the risk of overfitting._ 

May 2014 

Notices of the AMS 

465 



Figure 7. Performance degradation as a result of strategy selection under compensation effects (first-order serial correlation). 

_Figure 7 illustrates that a serially correlated performance introduces another form of compensation effects, just as we saw in the case of a global constraint. For example, if ϕ =_ 0 _._ 995 _, it takes about 138 observations to recover half of the deviation from the equilibrium. We have rerun the previous Monte Carlo experiment, this time on an autoregressive process with µ =_ 0 _, σ =_ 1 _, ϕ =_ 0 _._ 995 _, and have plotted the pairs of performance IS vs. OOS._ Recentering a series is one way to introduce memory into a process, because some data points will now compensate for the extreme outcomes from other data points. By optimizing a backtest, the researcher selects a model configuration that spuriously works well IS and consequently is likely to generate losses OOS. 

## Serial Dependence 

Imposing a global constraint is not the only situation in which overfitting actually is detrimental. To cite another (less restrictive) example, the same effect happens if the performance series is serially conditioned, such as a first-order autoregressive process, 

(10) ∆ _mτ = (_ 1 _− ϕ)µ + (ϕ −_ 1 _)ϕmτ−_ 1 _+ σετ_ or, analogously, 

(11) _mτ = (_ 1 _− ϕ)µ + ϕmτ−_ 1 _+ σετ ,_ 

where the random shocks are again IID distributed as _ετ ∼ Z_ . The following proposition is proven in the appendix. The number of observations that it takes for a process to reduce its divergence from the long-run equilibrium by half is known as the _half-life period_ , or simply _half-life_ (a familiar physical concept introduced by Ernest Rutherford in 1907). 

Proposition 4. _The half-life period of a first-order autoregressive process with autoregressive coefficient ϕ ∈ (_ 0 _,_ 1 _) occurs at_ 



For example, if _ϕ =_ 0 _._ 995, it takes about 138 observations to retrace half of the deviation from the equilibrium. This introduces another form of compensation effect, just as we saw in the case of a global constraint. If we rerun the previous Monte Carlo experiment, this time for the autoregressive process with _µ =_ 0 _, σ =_ 1 _, ϕ =_ 0 _._ 995, and plot the pairs of performance IS vs. OOS, we obtain Figure 7. 

The _p_ -values associated with the intercept and the IS performance (SR a priori) are respectively 0.4513 and 0, confirming that the negative linear relation between IS and OOS Sharpe ratios is again statistically significant. Such serial correlation is a well-known statistical feature, present in the performance of most hedge fund strategies. Proposition 5 is proved in the appendix. 

Proposition 5. _Given two alternative configurations (A and B) of the same model, where σIS_<sup>_A=_</sup> _σOOS_<sup>_A= σ B_</sup> _IS_<sup>_= σ B_</sup> _OOS_<sup>_and the performance series fol-_</sup> _lows the same first-order autoregressive stationary process,_ 



Proposition 5 reaches the same conclusion as Proposition 3 (a compensation effect) without requiring a global constraint. 

## Is Backtest Overfitting a Fraud? 

Consider an investment manager who emails his stock market forecast for the next month to 2<sup>_n_</sup> _x_ prospective investors, where _x_ and _n_ are positive integers. To half of them he predicts that markets will go up, and to the other half that markets will go down. After the month passes, he drops from his list the names to which he sent the incorrect forecast, and sends a new forecast to the remaining 2<sup>_n−_1</sup> _x_ names. He repeats the same procedure _n_ times, after which only _x_ names remain. These _x_ investors have witnessed _n_ consecutive infallible forecasts and may be extremely tempted to give this investment manager all of their savings. Of course, this is a fraudulent scheme based on random screening: The investment manager is hiding the fact that for every one of the _x_ successful witnesses, he has tried 2<sup>_n_</sup> unsuccessful ones (see Harris [8, p. 473] for a similar example). 

To avoid falling for this psychologically compelling fraud, a potential investor needs to consider the economic cost associated with manufacturing the successful experiments, and require the investment manager to produce a number _n_ for which 

466 

Notices of the AMS 

Volume 61, Number 5 

the scheme is uneconomic. One caveat is, even if _n_ is too large for a skill-less investment manager, it may be too low for a mediocre investment manager who uses this scheme to inflate his skills. 

Not reporting the number of trials _(N)_ involved in identifying a successful backtest is a similar kind of fraud. The investment manager only publicizes the model that works but says nothing about all the failed attempts, which as we have seen can greatly increase the probability of backtest overfitting. 

An analogous situation occurs in medical research, where drugs are tested by treating hundreds or thousands of patients; however, only the best outcomes are publicized. The reality is that the selected outcomes may have healed in spite of (rather than thanks to) the treatment or due to a placebo effect (recall Proposition 1). Such behavior is unscientific—not to mention dangerous and expensive—and has led to the launch of the _alltrials.net_ project, which demands that all results (positive and negative) for every experiment are made publicly available. A step forward in this direction is the recent announcement by Johnson & Johnson that it plans to open all of its clinical test results to the public [14]. For a related discussion of reproducibility in the context of mathematical computing, see Stodden et al. [25]. 

Hiding trials appears to be standard procedure in financial research and financial journals. As an aggravating factor, we know from the section “Overfitting in Presence of Compensation Effects” that backtest overfitting typically has a detrimental effect on future performance due to the compensation effects present in financial series. Indeed, the customary disclaimer “past performance is not an indicator of future results” is too optimistic in the context for backtest overfitting. When investment advisers do not control for backtest overfitting, good backtest performance is an indicator of negative future results. 

## A Practical Application 

Institutional asset managers follow certain investment procedures on a regular basis, such as rebalancing the duration of a fixed income portfolio (PIMCO); rolling holdings on commodities (Goldman Sachs, AIG, JP Morgan, Morgan Stanley); investing or divesting as new funds flow at the end of the month (Fidelity, BlackRock); participating in the regular U.S. Treasury Auctions (all major investment banks); delevering in anticipation of payroll, FOMC or GDP releases; tax-driven effects around the end of the year and mid-April; positioning for electoral cycles, etc. There are a large number of instances where asset managers will engage in somewhat predictable actions on a regular basis. It should come as no surprise that a very popular 



Figure 8. Backtested performance of a seasonal strategy (Example 6). 

_We have generated a time series of 1000 daily prices (about four years) following a random walk. The PSR-Stat of the optimal model configuration is_ 2.83 _, which implies a less-than 1 percent probability that the true Sharpe ratio is below_ 0 _. Consequently, we have been able to identify a plausible seasonal strategy with an SR of_ 1.27 _despite the fact that no true seasonal effect exists._ 

investment strategy among hedge funds is to profit from such seasonal effects. 

For example, a type of question often asked by hedge fund managers follows the form: “Is there a time interval every [ ] when I would have made money on a regular basis?” You may replace the blank space with a word like day, week, month, quarter, auction, nonfarm payroll (NFP) release, European Central Bank (ECB) announcement, presidential election year, _. . ._ . The variations are as abundant as they are inventive. Doyle and Chen [4] study the “weekday effect” and conclude that it appears to “wander”. 

The problem with this line of questioning is that there is always a time interval that is arbitrarily “optimal” regardless of the cause. The answer to one such question is the title of a very popular investment classic, _Do Not Sell Stocks on Monday_ , by Hirsch [12]. The same author wrote an almanac for stock traders that reached its forty-fifth edition in 2012 and he is also a proponent of the “Santa Claus Rally”, the quadrennial political/stock market cycle, and investing during the “Best Six Consecutive Months” of the year, November through April. While these findings may indeed be caused by some underlying seasonal effect, it is easy to demonstrate that any random data contains similar patterns. The discovery of a pattern IS typically has no bearing OOS, yet again is a result of overfitting. Running such experiments 

May 2014 

Notices of the AMS 

467 

without controlling for the probability of backtest overfitting will lead the researcher to spurious claims. OOS performance will disappoint, and the reason will not be that “the market has found out the seasonal effect and arbitraged away the strategy’s profits.” Rather, the effect was never there; instead, it was just a random pattern that gave rise to an overfitted trading rule. We will illustrate this point with an example. 

Example 6. Suppose that we would like to identify the optimal monthly trading rule given four customary parameters: Entry <u>day,</u> Holding <u>period,</u> Stop <u>loss,</u> and Side. _Side_ defines whether we will hold long or short positions on a monthly basis. _Entry_ _<u>day</u>_ determines the business day of the month when we enter a position. _Holding period_ gives the number of days that the position is held. _Stop_ _<u>loss</u>_ determines the size of the loss (as a multiple of the series’ volatility) that triggers an exit for that month’s position. For example, we could explore all nodes that span the set _{_ 1 _, . . . ,_ 22 _}_ for _Entry_ _<u>day</u>_ , the set _{_ 1 _, . . . ,_ 20 _}_ for _Holding_ _<u>period</u>_ , the set _{_ 0 _, . . . ,_ 10 _}_ for _Stop_ _<u>loss</u>_ , and _{−_ 1 _,_ 1 _}_ for _Side_ . The parameter combinations involved form a four-dimensional mesh of 8,800 elements. The optimal parameter combination can be discovered by computing the performance derived by each node. 

First, we generated a time series of 1,000 daily prices (about four years), following a random walk. Figure 8 plots the random series, as well as the performance associated with the optimal parameter combination: Entry <u>day</u> = 11, Holding <u>period</u> = 4, Stop <u>loss</u> = _−_ 1 and Side = 1. The annualized Sharpe ratio is 1.27. 

Given the elevated Sharpe ratio, we could conclude that this strategy’s performance is significantly greater than zero for any confidence level. Indeed, the _PSR-Stat_ is 2.83, which implies a less than 1 percent probability that the true Sharpe ratio is below 0.<sup>5</sup> Several studies in the practitioners and academic literature report similar results, which are conveniently justified with some ex-post explanation (“the posterior gives rise to a prior”). What this analysis misses is an evaluation of the probability that this backtest has been overfit to the data, which is the subject of Bailey et al. [1]. 

In this practical application we have illustrated how simple it is to produce overfit backtests when answering common investment questions, such as the presence of seasonal effects. We refer the reader to the appendix section “Reproducing the 

> 5 _The Probabilistic Sharpe Ratio (or PSR) is an extension to the SR. Nonnormality increases the error of the variance estimator, and PSR takes that into consideration when determining whether an SR estimate is statistically significant. See Bailey and López de Prado_ [2] _for details._ 

Results in Example 6” for the implementation of this experiment in the Python language. Similar experiments can be designed to demonstrate overfitting in the context of other effects, such as trend following, momentum, mean-reversion, event-driven effects, etc. Given the facility with which elevated Sharpe ratios can be manufactured IS, the reader would be well advised to remain highly suspicious of backtests and of researchers who fail to report the number of trials attempted. 

## Conclusions 

While the literature on regression overfitting is extensive, we believe that this is the first study to discuss the issue of overfitting on the subject of investment simulations (backtests) and its negative effect on OOS performance. On the subject of regression overfitting, the great Enrico Fermi once remarked (Mayer et al. [20]): 

I remember my friend Johnny von Neumann used to say, with four parameters I can fit an elephant, and with five I can make him wiggle his trunk. 

The same principle applies to backtesting, with some interesting peculiarities. We have shown that backtest overfitting is difficult indeed to avoid. Any perseverant researcher will always be able to find a backtest with a desired Sharpe ratio regardless of the sample length requested. Model complexity is only one way that backtest overfitting is facilitated. Given that most published backtests do not report the number of trials attempted, many of them may be overfitted. In that case, if an investor allocates capital to them, performance will vary: _It will be around zero if the process has no memory, but it may be significantly negative if the process has memory._ The standard warning that _“past performance is not an indicator of future results”_ understates the risks associated with investing on overfit backtests. When financial advisors do not control for overfitting, positive backtested performance will often be followed by negative investment results. 

We have derived the expected maximum Sharpe ratio as a function of the number of trials _(N)_ and sample length. This has allowed us to determine the _Minimum Backtest Length_ (MinBTL) needed to avoid selecting a strategy with a given IS Sharpe ratio among _N_ trials with an expected OOS Sharpe ratio of zero. Our conclusion is that the more trials a financial analyst executes, the greater should be the IS Sharpe ratio demanded by the potential investor. 

We strongly suspect that such backtest overfitting is a large part of the reason why so many algorithmic or systematic hedge funds do not live up to the elevated expectations generated by their managers. 

468 

Notices of the AMS 

Volume 61, Number 5 

We would feel sufficiently rewarded in our efforts if this paper succeeds in drawing the attention of the mathematical community to the widespread proliferation of journal publications, many of them claiming profitable investment strategies on the sole basis of in-sample performance. This is understandable in business circles, but a higher standard is and should be expected from an academic forum. 

A depressing parallel can be drawn between today’s financial academic research and the situation denounced by economist and Nobel Laureate Wassily Leontief writing in _Science_ (see Leontief [16]): 

A dismal performance 

_. . ._ “What economists revealed most clearly was the extent to which their profession lags intellectually.” This editorial comment by the leading economic weekly (on the 1981 annual proceedings of the American Economic Association) says, essentially, that the “king is naked.” But no one taking part in the elaborate and solemn procession of contemporary U.S. academic economics seems to know it, and those who do don’t dare speak up. 

_[. . .]_ 

[E]conometricians fit algebraic functions of all possible shapes to essentially the same sets of data without being able to advance, in any perceptible way, a systematic understanding of the structure and the operations of a real economic system. 

_[. . .]_ 

That state is likely to be maintained as long as tenured members of leading economics departments continue to exercise tight control over the training, promotion, and research activities of their younger faculty members and, by means of peer review, of the senior members as well. 

We hope that our distinguished colleagues will follow this humble attempt with ever-deeper and more convincing analysis. We did not write this paper to settle a discussion. On the contrary, our wish is to ignite a dialogue among mathematicians and a reflection among investors and regulators. We should do well also to heed Newton’s comment after he lost heavily in the South Seas bubble; see [21]: 

For those who had realized big losses or gains, the mania redistributed wealth. The largest honest fortune was made by Thomas Guy, a stationer turned philanthropist, who owned £54,000 of South Sea stock in April 1720 and sold it over the following six weeks for £234,000. Sir Isaac Newton, scientist, 

master of the mint, and a certifiably rational man, fared less well. He sold his £7,000 of stock in April for a profit of 100 percent. But something induced him to reenter the market at the top, and he lost £20,000. “I can calculate the motions of the heavenly bodies,” he said, “but not the madness of people.” 

## Appendices 

## Proof of Proposition 1 

Embrechts et al. [5, pp. 138–147] show that the maximum value (or last order statistic) in a sample of independent random variables following an exponential distribution converges asymptotically to a Gumbel distribution. As a particular case, the Gumbel distribution covers the Maximum Domain of Attraction of the Gaussian distribution, and therefore it can be used to estimate the expected value of the maximum of several independent random Gaussian variables. 

To see how, suppose there is a sample of IID random variables, _zn ∼ Z, n =_ 1 _, . . . , N_ , where _Z_ is the CDF of the Standard Normal distribution. To derive an approximation for the sample maximum, max _n =_ max _{zn}_ , we apply the Fisher-TippettGnedenko theorem to the Gaussian distribution and obtain that 



where 

- _G[x] = e_<sup>_−e−x_</sup> is the CDF for the Standard Gumbel distribution. 

- _α = Z_<sup>_−_1 �</sup> 1 _− N_<sup>1</sup> �, _β = Z_<sup>_−_1 �</sup> 1 _− N_<sup>1</sup><sup>_e−_1�</sup> _− α_ , and _Z_<sup>_−_1</sup> corresponds to the inverse of the Standard Normal’s CDF. 

The normalizing constants _(α, β)_ are derived in Resnick [22] and Embrechts et al. [5]. The limit of the expectation of the normalized maxima from a distribution in the Gumbel Maximum Domain of Attraction (see Proposition 2.1(iii) in Resnick [22]) is 



where _γ_ is the Euler-Mascheroni constant, _γ ≈_ 0 _._ 5772156649 _. . ._ . Hence, for _N_ sufficiently large, the mean of the sample maximum of standard normally distributed random variables can be approximated by (16) _E[_ max _N_<sup>_]_</sup> 



May 2014 

Notices of the AMS 

469 

## Proof of Proposition 3 

Suppose there are two random samples ( _A_ and _B_ ) of the same process _{_ ∆ _mτ }_ , where _A_ and _B_ are of equal size and have means and standard deviations _µ_<sup>_A_</sup> _, µ_<sup>_B_</sup> _, σ_<sup>_A_</sup> _, σ_<sup>_B_</sup> . A fraction _δ_ of each sample is called IS, and the remainder is called OOS, where for simplicity we have assumed that _σIS_<sup>_A=σ A_</sup> _OOS_<sup>_=σ B_</sup> _IS_<sup>_=σ B_</sup> _OOS_<sup>.Wewouldliketo</sup> understand the implications of a global constraint _µ_<sup>_A_</sup> _= µ_<sup>_B_</sup> . 

First, we note that _µ_<sup>_A_</sup> _= δµIS_<sup>_A+ (_1</sup><sup>_−δ)µ_</sup> _OOS_<sup>_A_</sup> and _µ_<sup>_B_</sup> _= δµIS_<sup>_B+ (_1</sup><sup>_−δ)µ_</sup> _OOS_<sup>_B_. Then,</sup><sup>_µ_</sup> _IS_<sup>_A> µ_</sup> _OOS_<sup>_A_�</sup> _µIS_<sup>_A>µA_�</sup><sup>_µ_</sup> _OOS_<sup>_A<µA_.Likewise,</sup><sup>_µ_</sup> _IS_<sup>_B>µ_</sup> _OOS_<sup>_B_�</sup> _µIS_<sup>_B> µB_�</sup><sup>_µ_</sup> _OOS_<sup>_B< µB_.</sup> Second, because of the global constraint _µ_<sup>_A_</sup> _= µ_<sup>_B_</sup> , _µIS_<sup>_A+_</sup><sup>_<u>(</u>_1</sup><sup>_−_</sup> _δ_<sup>_<u>δ)</u>_</sup> _µOOS_<sup>_A= µ_</sup> _IS_<sup>_B+_</sup><sup>_<u>(</u>_1</sup><sup>_−_</sup> _δ_<sup>_<u>δ)</u>_</sup> _µOOS_<sup>_B_and</sup><sup>_µ_</sup> _IS_<sup>_A−µ_</sup> _IS_<sup>_B=_</sup> _<u>(</u>_ 1 _−δδ) (µOOS_<sup>_B−µ_</sup> _OOS_<sup>_A)_.Then,</sup><sup>_µ_</sup> _IS_<sup>_A>µ_</sup> _IS_<sup>_B_�</sup><sup>_µ_</sup> _OOS_<sup>_A<_</sup> _µOOS_<sup>_B_.Wecandividethisexpressionby</sup><sup>_σ A_</sup> _IS_<sup>_>_0,</sup> with the implication that 



where we have denoted _SRIS_<sup>_A=_</sup> _σµIS_<sup>_A_</sup> _IS_<sup>_A_, etc. Note that</sup> we did not have to assume that ∆ _mτ_ is IID, thanks to our assumption of equal standard deviations. The same conclusion can be reached without assuming equality of standard deviations; however, the proof would be longer but no more revealing (the point of this proposition is the implication of global constraints). 

## Proof of Proposition 4 

This proposition computes the half-life of a firstorder autoregressive process. Suppose there is a random variable _mτ_ that takes values of a sequence of observations _τ ∈{_ 1 _, . . . , ∞}_ , where 

(18) _mτ = (_ 1 _− ϕ)µ + ϕmτ−_ 1 _+ σετ_ such that the random shocks are IID distributed as _ετ ∼ N(_ 0 _,_ 1 _)_ . Then 



if and only if _ϕ ∈ (−_ 1 _,_ 1 _)_ . In particular, from Bailey and López de Prado [3] we know that the expected value of this process at a particular observation _τ_ is 



Suppose that the process is initialized or reset at some value _m_ 0 ≠ _µ_ . We ask the question, how many observations must pass before 





which implies the additional constraint that _ϕ ∈ (_ 0 _,_ 1 _)_ . 

## Proof of Proposition 5 

Suppose that we draw two samples ( _A_ and _B_ ) of a first-order autoregressive process and generate two subsamples of each. The first subsample is called IS and is comprised of _τ =_ 1 _, . . . , δT_ , and the second subsample is called OOS and is comprised of _τ = δT +_ 1 _, . . . , T_ , with _δ ∈ (_ 0 _,_ 1 _)_ and _T_ an integer multiple of _δ_ . For simplicity, let us assume that _σIS_<sup>_A= σ A_</sup> _OOS_<sup>_= σ B_</sup> _IS_<sup>_= σ B_</sup> _OOS_<sup>. From Proposition 4,</sup> (18), we obtain 

(22) _EδT [mT ] − mδT = (_ 1 _− ϕ_<sup>_T_</sup> _)(µ − mδT )._ Because 1 _− ϕ_<sup>_T_</sup> _>_ 0, _σIS_<sup>_A=σ B_</sup> _IS_<sup>,</sup><sup>_SR_</sup> _IS_<sup>_A>SR_</sup> _IS_<sup>_B_�</sup> _mδT_<sup>_A>m_</sup> _δT_<sup>_B_.ThismeansthattheOOSof</sup><sup>_A_</sup> begins with a seed that is greater than the seed that initializes the OOS of _B_ . Therefore, _mδT_<sup>_A>_</sup> _mδT_<sup>_B_�</sup><sup>_EδT [m_</sup> _T_<sup>_A]−m_</sup> _δT_<sup>_A< EδT [m_</sup> _T_<sup>_B]−m_</sup> _δT_<sup>_B_. Because</sup> _σIS_<sup>_B= σ B_</sup> _OOS_<sup>, we conclude that</sup> 



## Reproducing the Results in Example 6 

Python code implementing the experiment described in “A Practical Application” can be found at `http://www.quantresearch.info/Software. htm` and at `http://www.financial-math.org/ software/` . 

## Acknowledgments 

We are indebted to the editor and two anonymous referees who peer-reviewed this article for the _Notices of the American Mathematical Society_ . We are also grateful to Tony Anagnostakis (Moore Capital), Marco Avellaneda (Courant Institute, NYU), Peter Carr (Morgan Stanley, NYU), Paul Embrechts (ETH Zürich), Matthew D. Foreman (University of California, Irvine), Jeffrey Lange (Guggenheim Partners), Attilio Meucci (KKR, NYU), Natalia Nolde (University of British Columbia ad ETH Zürich), and Riccardo Rebonato (PIMCO, University of Oxford). The opinions expressed in this article are the authors’ and they do not necessarily reflect the views of the Lawrence Berkeley National Laboratory, Guggenheim Partners, or any other organization. No particular investment or course of action is recommended. 

## References 

- [1] D. Bailey, J. Borwein, M. López de Prado and J. Zhu, The probability of backtest overfitting, 2013, working paper, available at `http://ssrn.com/abstract=2326253` . 

- [2] D. Bailey and M. López de Prado, The Sharpe ratio efficient frontier, _Journal of Risk_ 15(2) (2012), 3–44. Available at `http://ssrn.com/abstract=1821643` . 

470 

Notices of the AMS 

Volume 61, Number 5 

- [3] , Drawdown-based stop-outs and the triple penance rule, 2013, working paper. Available at `http://ssrn.com/abstract=2201302` . 

- [4] J. Doyle and C. Chen, The wandering weekday effect in major stock markets, _Journal of Banking and Finance_ 33 (2009), 1388–1399. 

- [5] P. Embrechts, C. Klueppelberg and T. Mikosch, _Modelling Extremal Events_ , Springer-Verlag, New York, 2003. 

- [6] R. Feynman, _The Character of Physical Law_ , The MIT Press, 1964. 

- [7] J. Hadar and W. Russell, Rules for ordering uncertain prospects, _American Economic Review_ 59 (1969), 25– 34. 

- [8] L. Harris, _Trading and Exchanges: Market Microstructure for Practitioners_ , Oxford University Press, 2003. 

      - experimental mathematics, February 2013. Available at `http://www.davidhbailey.com/dhbpapers/ icerm-report.pdf` . 

   - [26] G. Van Belle and K. Kerr, _Design and Analysis of Experiments in the Health Sciences_ , John Wiley & Sons. 

   - [27] H. White, A reality check for data snooping, _Econometrica_ 68(5), 1097–1126. 

   - [28] S. Weiss and C. Kulikowski, _Computer Systems That Learn: Classification and Prediction Methods from Statistics, Neural Nets, Machine Learning and Expert Systems_ , 1st edition, Morgan Kaufman,1990. 

   - [29] A. Wiles, Financial greed threatens the good name of maths, _The Times_ (04 Oct 2013). Available online at `http://www.thetimes.co.uk/tto/education/ article3886043.ece` . 

- [9] C. Harvey and Y. Liu, Backtesting, working paper, SSRN, 2013, `http://ssrn.com/abstract=2345489` . 

- [10] C. Harvey, Y. Liu, and H. Zhu, _. . ._ and the crosssection of expected returns, working paper, SSRN, 2013, `http://ssrn.com/abstract=2249314` . 

- [11] D. Hawkins, The problem of overfitting, _Journal of Chemical Information and Computer Science_ 44 (2004), 1–12. 

- [12] Y. Hirsch, _Don’t Sell Stocks on Monday_ , Penguin Books, 1st edition, 1987. 

- [13] J. Ioannidis, Why most published research findings are false, _PLoS Medicine_ 2(8), August 2005. 

- [14] H. Krumholz Give the data to the people, _New York Times_ , February 2, 2014. Available at `http://www.nytimes.com/2014/02/03/opinion/ give-the-data-to-the-people.html` . 

- [15] D. Leinweber and K. Sisk, Event driven trading and the “new news”, _Journal of Portfolio Management_ 38(1) (2011), 110–124. 

- [16] W. Leontief, Academic economics, _Science Magazine_ (July 9, 1982), 104–107. 

- [17] A. Lo, The statistics of Sharpe ratios, _Financial Analysts Journal_ 58 4 (Jul/Aug, 2002). Available at `http://ssrn.com/abstract=377260` . 

- [18] M. López de Prado and A. Peijan, Measuring the loss potential of hedge fund strategies, _Journal of Alternative Investments_ 7(1), (2004), 7–31. Available at `http://ssrn.com/abstract=641702` . 

- [19] M. López de Prado and M. Foreman, A mixture of Gaussians approach to mathematical portfolio oversight: The EF3M algorithm, working paper, RCC at Harvard University, 2012. Available at `http://ssrn.com/abstract=1931734` . 

- [20] J. Mayer, K. Khairy, and J. Howard, Drawing an elephant with four complex parameters, _American Journal of Physics_ 78(6) (2010). 

- [21] C. Reed, The damn’d South Sea, _Harvard Magazine_ (May–June 1999). 

- [22] S. Resnick, _Extreme Values, Regular Variation and Point Processes_ , Springer, 1987. 

- [23] J. Romano and M. Wolf, Stepwise multiple testing as formalized data snooping, _Econometrica_ 73(4) (2005), 1273–1282. 

- [24] F. Schorfheide and K. Wolpin, On the use of holdout samples for model selection, _American Economic Review_ 102(3) (2012), 477–481. 

- [25] V. Stodden, D. Bailey, J. Borwein, R. LeVeque, W. Rider, and W. Stein, Setting the default to reproducible: Reproducibility in computational and 

May 2014 

Notices of the AMS 

471 

