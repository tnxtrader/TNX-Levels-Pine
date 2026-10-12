# **The Feller Diffusion, Filter Rules and Abnormal Stock Returns** 

Paul Docherty<sup>**a**</sup> , Yizhe Dong<sup>**b**</sup> , Xiaojong Song<sup>**c**</sup> and Mark Tippett<sup>**a,d,***</sup> 

> **a** Business School, University of Newcastle, Callaghan, NSW, 2308, Australia 

> **b** Business School, University of Aberdeen, Dunbar Street, Aberdeen, AB24 3QY, UK ; 

> **c** Business School, University of East Anglia, Chancellor’s Drive, Norwich, NR4 7TJ, UK; 

> **d** Business School, University of Sydney, Codrington Street, Sydney, NSW, 2008, Australia 

We determine the conditional expected logarithmic (that is, continuously compounded) return on a stock whose price evolves in terms of the Feller diffusion and then use it to demonstrate how one must know the exact probability density that describes a stock’s return before one can determine the correct way to calculate the abnormal returns that accrue on the stock.  We show in particular that misspecification of the stochastic process which generates a stock’s price will lead to systematic biases in the abnormal returns calculated on the stock.  We examine the implications this has for the proper conduct of empirical work and for the evaluation of stock and portfolio performance. 

_Keywords_ : Feller diffusion; Fokker-Planck equation; Geometric Brownian Motion; Logarithmic return 

_JEL Classification_ : C52, G14, G17 

> ***** Corresponding author. Email: M.Tippett@lboro.ac.uk.  The authors acknowledge the helpful comments and suggestions of the Editor and referee.  The usual disclaimer applies. 

2 

## **1.  Introduction** 

The Feller (1951a) diffusion (or continuous time branching process) has often been advanced as a model of stock price behaviour (Cox and Ross 1976; Cox 1996; Ashton et al. 2003, Gan and Waxman 2015).  In this paper we determine the conditional expected logarithmic (that is, continuously compounded) return on a stock whose price evolves in terms of the Feller diffusion.  We then use it to show that one must know the exact probability density that generates a stock’s return before one can determine the correct way to calculate the abnormal returns that accrue on the stock.  We show, for example, that if one mistakenly applies the abnormal returns formula for the Geometric Brownian Motion to a stock whose returns actually evolve in terms of the Feller diffusion, then there will be systematic biases in the computed abnormal returns which will give the appearance of abnormal returns when, in fact, there are no abnormal returns at all. Our analysis is purely theoretical in the sense that it contrasts key properties of two widely applied stochastic processes in the asset pricing literature and shows how they lead to contradictory specifications of the expected logarithmic returns that accrue on risky assets.  We then develop the implications of this outcome for the way that abnormal stock returns ought to be calculated in the empirical work of the discipline.<sup>1</sup> 

We commence our analysis in Section 2 by using the hyperbolic (sinh-arcsinh) functions (Jones and Pewsey 2009) to model the evolution of stock prices in terms of a discrete time binomial filtration (Nelson and Ramaswamy 1990).  Binomial models are particularly simple because they assume a stock’s price will either increase with given probability or decline with complementary probability at each of the discrete points in time on which the modelling procedures are based.  Moreover, variations in the stock’s 

> 1  We emphasise in particular that we are not suggesting that the expected logarithmic returns formula for determining abnormal returns under the Feller diffusion is in any way superior to that of the equivalent formula for the Geometric Brownian Motion. 

3 

price and the probabilities associated with them can often be specified in such a way as to ensure that desired modelling requirements are satisfied - as, for example, when the stock’s price cannot be negative or when increments in the stock’s price must possess a mean and variance with given attributes.<sup>2</sup> We exploit this feature of the binomial filtration by specifying the discrete time mean and variance of increments in a stock’s price in such a way as to ensure that as the time between stock price movements tends toward zero, the stock price itself will converge in distribution to the Feller diffusion. We then use the stochastic differential equation defined by these procedures to resolve an apparent inconsistency in the distributional properties of the Feller diffusion arising from the application of the Laplace transform to the Fokker-Planck (that is, ChapmanKolmogorov) equation in Feller (1951b). 

In Section 3 we broaden our use of the Fokker-Planck equation by determining the expected logarithmic return on a stock whose price evolves in terms of the Feller diffusion.  We show, in particular, that the expected logarithmic return for the Feller diffusion varies according to the conditional probability of ultimate extinction (that is, the probability that the stock’s price will eventually fall away to zero or equivalently, the firm will enter bankruptcy).  This contrasts with the more commonly employed Geometric Brownian Motion for which the conditional expected logarithmic return is the same irrespective of the prevailing stock price.  We demonstrate in particular, how the differing distributional properties of the Feller diffusion and the Geometric Brownian Motion will mean that it is crucially important for empirical researchers to test the compatibility of the observed returns on a given stock with the distributional assumptions on which the empirical analysis is based, before deciding on an appropriate quadrature 

> 2 This also makes binomial models particularly attractive because of the straightforward way in which they can be empirically implemented. 

4 

formula through which to determine the abnormal returns that accrue on the stock. Section 4 concludes the paper. 

## **2.  Feller Diffusion** 

Let _x_ ( _t_ )  0 be a stock’s price at time _t_ .  Now, suppose the increment in the stock’s price over the period from time _t_ until time ( _t_   _t_ ) will be either positive, in the amount: 

3  _x_ ( _t_ )  _x_ ( _t_   _t_ )  _x_ ( _t_ )  sinh[  _x_ ( _t_ )  _t_ ]   _x_ ( _t_ )  _t_  _O_ [(  _t_ ) 2 ] (1a) 

or negative, in the amount: 



where  is a parameter whose meaning will become clear from context.  Moreover, the probability of a positive increment in the stock’s price is: 



where again,  is a parameter whose meaning will become clear from context.  This in 

turn will mean that the probability of a negative increment in the stock’s price will be: 



Bringing these results together shows that the expected increment in the stock’s price 

over the period from time _t_ until time _(t +_  _t)_ will be: 



5 



or, upon evaluation: 



One can then take limits across the above expression in which case it follows that the 

instantaneous expected increment (per unit time) in the stock’s price will be: 



Note, in particular, how this shows that  is the expected proportionate change in the stock’s price or equivalently, the expected instantaneous buy and hold return on the stock 

on a per unit time basis.  Similar procedures show that the variance of the increment in the stock’s price will be: 



or upon evaluation: 



One can again take limits across the above expression in which case it follows that the instantaneous variance of the increment in the stock’s price will be: 

6 



This in turn will mean that the variance of instantaneous increments in the stock’s price 

will be proportional to the current stock price, with <sup>_2_</sup> serving as the constant of proportionality.  One can then use this and previous results to define the standardised variable: 



It then follows that the instantaneous increment in the stock’s price will be governed by 

the stochastic differential equation for the Feller diffusion; namely: 



where _dq_ ( _t_ )  _q_ ( _t_ ) _dt_ is a white noise process with unit variance parameter (Hoel, Port and Stone 1987, 142). 

Now, one can use the differential equation (8) for the Feller diffusion in conjunction with the Fokker-Planck equation (Cox and Miller 1965, 213-215) to show that the conditional probability density, _p(x(0),x(t),t)_ , for the stock’s price, _x(t)_ , at time _t_ will satisfy the partial differential equation (Feller 1951a, 235): 



Moreover at _t = 0_ the stock’s conditional probability density will take the form of a Dirac delta function which is completely concentrated at _x(0)_ (Sneddon 1961, 51-53; Cox and 

7 

Miller 1965, 209).  One can obtain the solution of this initial value problem by first making the substitution (Nariboli 1977, 165): 





transformations the Fokker-Planck equation (9) is reduced to the ordinary differential equation: 



(11) 

Hence, if one then makes the further substitution: 



the differential equation (11) becomes: 

Direct substitution into equation (13) will then show: 



8 

where _I_ 1(<sup></sup> ) is the modified Bessel function of the first kind of order one.<sup>3</sup> It then follows 

that the conditional probability density of the stock’s price is given by: 



(15) 



or upon substituting for  :<sup>4</sup> 



(16) 



Moreover, if one then makes the substitutions: 



3   In order to obtain this result, one must use the recurrence relationships _dI_ 1( _z_ <u>)</u>  _I_ 0 ( _z_ )  1 _I_ 1( _z_ ) and _dz z_ 

_dIdz_ <u>0 (</u> _z_ <u>)</u>  _I_ 1 ( _z_ ) , where _I0(z)_ is the modified Bessel function of the first kind of order zero (Abramowitz and Stegun 1964, 376). 

> 4  Feller (1951a, 235) states this probability density without proof, whilst Feller (1951b, 180) uses the Fokker-Planck equation (9) to determine the Laplace transform of the probability density which he then inverts to determine the probability density itself.  Unfortunately, the probability density determined in Feller (1951a, 235) differs from that stated in Feller (1951b, 180) by a multiple of _2b_ - where _b =_  in our analysis is the expected buy and hold return on the stock on a per unit time basis.  It thus follows that the probability density determined in Feller (1951b, 180) is incorrect - something that is of considerable importance given that the probability density summarised in Feller (1951b, 180) has been widely used in empirical analysis for parameter estimation (Gibbons and Ramaswamy, 1993). 

9 

it follows that the probability density for the Feller diffusion possesses the following canonical representation:<sup>5</sup> 



One can then use this latter result in conjunction with a simple application of the Algebra of Limits to show: 

_Limit p_ ( _x_ (0), _x_ ( _t_ ), _t_ )  0 and _Limit p_ ( _x_ (0), _x_ ( _t_ ), _t_ )  0 _x_ ( _t_ )  0 _x_ ( _t_ )  

which are necessary conditions for the existence of convergent moments (Gan and Waxman 2015).  We also demonstrate in the Appendix how one can use these results to show that the conditional probability of the stock’s price falling away to zero at some unspecified time in the future (that is, the conditional probability of eventual bankruptcy) 



## **3.  Expected Logarithmic Return** 

The conditional expected logarithmic return over the period from time zero until time _t_ 

on a stock whose price evolves in terms of the Feller diffusion is defined as follows: 

5   In the Appendix we use this result to show (Feller 1951a, 236): 



Note how this result shows that at _t = 0_ the conditional probability density for the Feller diffusion as given by equation (16) will take the form of a Dirac delta function which is completely concentrated at _x(0)_ (Sneddon 1961, 51-53; Cox and Miller 1965, 209). 

> 6  Feller (1951a, 236) again states this result without proof.  Cox and Miller (1965, 236-237) provide an alternative proof based on a series expansion of the moment generating function for the probability density (16). 

10 



Differentiating through this expression and then substituting the Fokker-Planck equation for the Feller diffusion will then show (Cox and Miller 1965, 213-215): 



Moreover, one can apply integration by parts to the second term on the right hand side of the above expression in which case it follows: 



One can also apply repeated integration by parts to the first term in equation (19) in which case we have: 



Bringing these results together by substituting equation (20) and equation (21) into equation (19) shows that the conditional expected logarithmic return on the stock satisfies the following differential equation: 

11 



Moreover, integrating across the above equation and applying the initial condition 

_x_ <u>(0)</u> _M_ (0)  _E_ {log[ ]}  0 will then show: _x_ (0) 





diffusion are strictly non-negative. 

Note how this result shows that the conditional expected logarithmic return over the period from time zero until time _t_ will be equal to the expected buy and hold return on the stock over this period,  _t_ , less a factor which hinges (at least partially) on the volatility parameter,  .  Moreover, this result possesses a superficial similarity to the expected logarithmic return on a stock whose price evolves in terms of the Geometric Brownian Motion; namely (Karlin and Taylor 1981, 363-364): 



where  is the expected instantaneous buy and hold return (and  _t_ is the buy and hold 

return over the period from time zero until time _t_ ), whilst <sup>_2_</sup> is the instantaneous variance of the logarithmic return on the stock (and <sup>_2_</sup> _t_ is the variance of the logarithmic return 

over the period from time zero until time _t_ ).  Note how for both the Geometric Brownian Motion and the Feller diffusion the expected logarithmic return is equal to the expected buy and hold return less an adjustment which is related to the volatility of the return. 

12 

However, for the Geometric Brownian Motion the volatility adjustment, 

1 2  2 _t_ , is a 

constant which does not depend the opening stock price, _x(0)_ .  This is in contrast to the volatility adjustment for the Feller diffusion which varies with the magnitude of the opening stock price and is also related to the probability of the stock’s price eventually falling away to zero.  One can demonstrate this latter point in more detail by considering the conditional expectation of the following exponential integral:<sup>7</sup> 



(24) 

where, as previously, _p(x(0),x(t),t)_ is the conditional probability density for the Feller 

 2 <u></u> _x_ <u>(</u> _t_ <u>)</u> diffusion as given by equation (16) and exp ~~{~~ 2 ~~}~~ is the conditional probability of  

ultimate extinction - that is, the probability that the stock’s price will eventually fall away to zero or equivalently, that the firm will enter bankruptcy (Feller 1951a, 236). Differentiating through this expression and substituting the Fokker-Planck equation for the Feller diffusion will then show: 



> 7  The exponential integral given here arises in a number of areas relating to the Feller diffusion.  In the Appendix, for example, we demonstrate how the average time it will take for a stock’s price to reach one or other of the upper and lower selling price triggers associated with a filter rule trading strategy (Fama and Blume 1966; Hong and Stein 1999; Sweeney 1988; Chan _et al._ 1996; D’Aspremont 2011) is stated in terms of the exponential integral given here. 

13 

Applying repeated integration by parts to the first term on the right hand side of the above expression implies: 



A similar application of integration by parts to the second term on the right hand side of equation (25) will also show: 





Substituting these latter two results into equation (25) will then imply: 





14 

Taking equation (26) in conjunction with equation (22) will then mean that the conditional expected logarithmic return over the period from time zero until time _t_ on a stock whose price evolves in terms of the Feller diffusion may be expressed as:<sup>8</sup> 



One can demonstrate the implementation of this result by considering a stock with an expected instantaneous buy and hold return of  _= 0.15_ (that is, _15%_ per annum) and a volatility parameter of  _= 1_ .  Substituting these parameter values into equation (27) will then show that the conditional expected annual logarithmic return on the stock will be: 



Table 1 summarises the conditional expected annual logarithmic return on a stock with 

the above parameter values (that is,  _= 0.15_ and  _= 1_ ) for various values of the opening stock price, _x(0)_ .<sup>9</sup> Thus, when the opening stock price amounts to _x(0)=£5_ then the conditional expected logarithmic return is _4.65%_ per annum reflecting the fact that the 

8  Here it is important to note that a simple application of L’Hôpital’s rule shows: 



as x(t)   - a result which is consistent with the numerical example summarised in Table 1 below. 

> 9  All integrals associated with the Feller diffusion are evaluated numerically using a combination of _50_ point Gauss-Legendre quadrature and _50_ point Gauss-Laguerre quadrature.  These quadrature rules integrate polynomial expressions of order _99_ or less exactly (Carnahan, Luther and Wilkes 1969, 101-105). 

15 

probability of ultimate extinction (that is, the probability the firm will eventually enter bankruptcy) amounts to  (Feller 1951a, 236): 



or, just over _22%_ .  However, as the opening stock price increases in magnitude and the probability of extinction declines, then the conditional expected logarithmic return on the 

## TABLE ONE ABOUT HERE 

stock increases significantly.  Thus, when the opening stock price is _x_ (0)  £90, there is only a trivial probability of ultimate extinction and the conditional expected logarithmic return on the stock has grown to _14.48%_ per annum.<sup>10</sup> Hence, the results summarised in this table show how the conditional expected logarithmic return on a stock whose price evolves in terms of the Feller diffusion is not an intertemporal constant but varies according to the conditional probability of ultimate extinction.  This is in direct contrast to the Geometric Brownian Motion (which underscores much of the empirical work conducted in the discipline), for which the conditional expected annual logarithmic return 

10  One can reinforce this point by using equation (8) in conjunction with Itô’s formula to determine the distributional properties of instantaneous increments in the logarithmic return, _r_ ( _t_ )  log[ _x_ <u>(</u> _t_ <u>)</u> ].  It then _x_ (0) 

follows: 



This result shows that the instantaneous increment in the logarithmic return (per unit time) will have a mean of _E_ <u>[</u> _dr_ <u>(</u> _t_ <u>)]</u>  (  1  2 and variance _Var_ <u>[</u> _dr_ <u>(</u> _t_ <u>)]</u>   2 Note that when _x_ ( _t_ )   2 or equivalently, _dt_ 2 _x_ ( _t_ )) _dt x_ ( _t_ .) 2  the probability of eventual extinction exceeds _e_  1  .0367879, then the expected instantaneous change in the stock’s logarithmic return will be negative.  This in turn will mean that in expectations there will be a downward spiral in the logarithmic return that will culminate ultimately, in the firm entering bankruptcy (that is, ultimate extinction). 

16 

will be the same irrespective of the opening stock price _, x(0)_ .  This in turn will mean that if a researcher mistakenly assumes a stock’s price evolves in terms of a Geometric Brownian Motion when, in fact, the actual process describing the evolution of the stock’s price is the Feller diffusion, there will be systematic biases in the abnormal returns calculated by the researcher.<sup>11</sup> 

This result has important implications for empirical analysis.  There is, for example, an extensive body of empirical literature that documents evidence of a long-run positive (negative) price drift after positive (negative) news announcements.  This drift in abnormal returns has been identified following earnings announcements (Ball and Brown, 1968; Bernard and Thomas, 1989), stock splits (Grinblatt et al., 1984), share repurchases (Lakonishok and Vermaelen, 1990), dividend initiations and omissions (Michaely et al., 1995) and revisions to analysts’ earnings forecasts (Elton et al., 1984). As this post-announcement drift in abnormal returns is difficult to reconcile with models 

11  Further insight may be obtained into the biases which arise from mistakenly assuming a stock’s price, _x(t)_ , evolves in terms of a Geometric Brownian Motion by applying a Taylor series expansion to _log[x(t)]_ about the point _x(0)e_<sup></sup><sup>_t_</sup> .  If one then takes expectations across the series expansion and substitutes the first four central moments for the Feller diffusion (Davidson and Tippett 2012, 218 and 232) one will end up with the following series expansion for the expected logarithmic return for the Feller diffusion: 



Substituting  _= 0.15_ ,  _= 1_ and _t = 1_ shows that the above expansion exactly replicates the results summarised in Table 1 except when _x(0) = $5_ and _x(0) = $3_<sup>_1_</sup> _/3_ where the expansion gives expected logarithmic returns of _0.0461_ and _-0.0168_ , respectively.  In these latter two cases the series expansion will need to encompass additional terms involving the higher order (fifth, sixth, etc.) moments if it is to give a more satisfactory approximation to the expected logarithmic return.  This contrasts with the Geometric Brownian Motion where equation (23) shows that only the first two moments are necessary to give an exact representation of the expected logarithmic return.  Moreover, the above series expansion shows that the opening stock price, _x(0)_ , affects the expected logarithmic return for the Feller diffusion in a way that is entirely absent from the expected logarithmic return for the Geometric Brownian Motion. 

17 

of rational expectations, the prevailing view in the literature is that these empirical observations can be attributed to investor under-reaction (Fama, 1998).<sup>12</sup> 

It should be noted that a drift pattern in abnormal returns can also occur if a stock’s benchmark return is based on the Geometric Brownian Motion when in fact the actual process describing the evolution of the stock’s price is the Feller diffusion.  Here it will be recalled that when a stock is associated with positive (negative) news, there will in general be an instantaneous positive (negative) increase in the market price of the given stock.  Moreover, under the assumption that the stock’s price evolves in terms of the Geometric Brownian Motion the instantaneous price shock would have no impact on the benchmark return used to isolate the abnormal returns on the stock.  However, as shown in Table 1, if the stock’s price evolves in terms of the Feller diffusion, the positive price shock would also result in an increased expected return on the stock.  Under these circumstances, mistakenly assuming the stock’s price evolves in terms of a Geometric Brownian Motion will give the appearance of a post-announcement drift in the abnormal returns that accrue on the stock.<sup>13</sup> 

12  A little reflection will convince the reader that our analysis has important implications for a much wider class of issues – including filter trading rules and momentum in stock returns, the prediction of corporate failure and the pricing of derivative securities (Cox 1996). 

13  Here it will be recalled that the expected logarithmic return on the Geometric Brownian Motion is a constant which is independent of the opening stock price, _x(0)_ .  Whether the stock price is “low” or “high”, the expected logarithmic return is the same.  This in turn means that the expected logarithmic returns in Table 1 under the false assumption of the Geometric Brownian Motion will be the same irrespective of the opening stock price.  One can demonstrate the nature of the biases which arise under the false assumption of the Geometric Brownian Motion by supposing a researcher undertakes parameter estimation when the stock’s price is concentrated around _x(0) = £45_ per share.  Consistent with the results summarised in Table 1 the researcher will find that the average annual logarithmic return over the estimation period will be around _13.96%_ (per annum).  Given this, the researcher will set this return as the “benchmark” against which the stock’s subsequent performance is to be assessed.  However, if the researcher applies this benchmark to returns based on opening stock prices that are not in the neighbourhood of _x(0) = £45_ , there will be evidence of (fictitious) abnormal returns.  Thus, for example, Table 1 shows that if the stock’s price is in the neighbourhood of _x(0) = £30_ then applying a benchmark return of _13.96%_ will lead, on average, to a negative abnormal return of around _(0.1343 - 0.1396 =) -0.53%_ (per annum).  Similarly, if the stock’s price is in the vicinity of _x(0) = £60_ there will, on average, be a positive abnormal return of around _(0.1422 - 0.1396 =) 0.26%_ (per annum). 

18 

Whilst we make no contention about the actual source of post-announcement drift anomalies the results summarised in Table 1 are informative as they identify an alternative set of conditions under which an empirical researcher may identify a postevent drift in stock prices.  We would not contend either that the Geometric Brownian Motion or the Feller diffusion is capable of providing a completely satisfactory description of the way stock prices evolve in practice.  It nonetheless warrants emphasising that if two commonly implemented stochastic processes can lead to such conflicting prescriptions for the calculation of abnormal returns, then it is important for researchers (and others) to assess the compatibility of the empirically observed returns on a given stock (or portfolio) with the assumed distributional assumptions on which the calculation of abnormal returns are based.  We note that such goodness of fit testing procedures have rarely been conducted in the empirical literature of the discipline and where they have, it is seldom the case that the empirically calculated returns are compatible with the Gaussian probability density on which most of the commonly employed abnormal returns quadrature rules are based. 

## **4.  Conclusions** 

We determine the conditional expected logarithmic return on a stock whose price evolves in terms of the Feller (1951a) diffusion and then use it to demonstrate how one must know the exact stochastic process that generates a stock’s return before one can determine the correct way to calculate the abnormal returns which accrue on the stock. We demonstrate this by considering a stock whose price actually evolves in terms of the Feller diffusion but where a researcher mistakenly assumes the stock’s price develops in terms of the more commonly employed Geometric Brownian Motion.  We show that when abnormal returns are calculated under the mistaken belief that the stock’s price 

19 

evolves in terms of the Geometric Brownian Motion, there will be the appearance of abnormal returns when, in fact, there are no abnormal returns at all.  This occurs because the expected logarithmic return on a stock whose price evolves in terms of the Feller diffusion varies according to the conditional probability of ultimate extinction (that is, the probability that the stock’s price will eventually fall away zero or equivalently, the firm will enter bankruptcy).  This is in contrast with the Geometric Brownian Motion for which the conditional expected logarithmic return is the same irrespective of the current level of the stock’s price.  Our analysis thus suggests that it is crucially important for researchers to test the compatibility of the empirically observed returns on a given stock with the assumed distributional assumptions on which the calculation of abnormal returns are based before deciding on an appropriate quadrature formula through which to isolate the abnormal returns that accrue on the stock. 

20 

## **Appendix: Probability Density of the Feller Diffusion** 

One can integrate through the probability density given by equation (17) of the text, over the semi-infinite real line and thereby show: 



Here one can let _z_   1 _y_ 2 from which it follows 1  _dz_  _ydy_ . Substitution will then 2 show: 



It then follows that the probability of the stock price falls away to nothing before time t is (Feller 1951a, 236): 



Moreover, this result also shows that the probability of the stock’s price falling away to zero at some unspecified future time (that is, the probability of ultimate extinction) amounts to (Feller 1951a, 236): 



## **Appendix: Filter Rule Trading Strategy** 

Consider a simple filter rule trading strategy under which a stock is purchased at a price of _x_ at time zero (Fama and Blume 1966; Hong and Stein 1999; Sweeney 1988; Chan _et al._ 1996; D’Aspremont 2011).  If the stock’s price subsequently rises to b  _x_ then the 

21 

stock is sold and the profits realised.  Alternatively, if the stock’s price falls to _a_  _x_ before it rises to b  _x_ then the investor invokes a “stop loss” strategy of selling the stock and realising the losses.  It then follows that the probability, _u(x)_ , of the stock’s price rising from its current level, _x_ , to the upper price selling trigger, b  _x_ , before it falls back to the lower price selling trigger, _a_  _x_ , can be determined as the solution of the following differential equation (Cox and Miller 1965, 231): 



with the boundary conditions being _u_ ( _a_ )  0 and _u_ ( _b_ )  .1  Determining the solution to 

the above boundary value problem shows: 



This in turn will mean that the probability of the stock’s price falling to _a_  _x_ before it rises to b  _x_ will be: 



Moreover, the average time,  _(x)_ , it will take the stock’s price to reach one or other of the two (that is, upper and lower) selling price triggers can be determined as the solution of the following differential equation (Cox and Miller 1965, 232): 



22 

with the boundary conditions being  _(a) = 0 =_  _(b)_ .  Solving the given boundary value 

problem shows that the average time it will take for the stock’s price to rise to _b_ or alternatively, to fall back to _a_ amounts to: 



where: 



and: 



Now, from equation (27) of the text it follows that the conditional abnormal logarithmic 

return over the period from time zero until time _t_ on a stock whose price evolves in terms of the Feller diffusion will amount to: 



Hence, comparing equation (27) of the text with equation (B5) above shows that the expected abnormal logarithmic return over the average period,  _(x)_ , it will take for the stock’s price to either rise to the upper selling price trigger, _b_ , or alternatively, fall back 

23 

to the lower selling price trigger, _a_ , turns out to be zero - something one would expect given that the stock’s price evolves in terms of a purely random process.<sup>14</sup> 

> 14  Here it is important to emphasise that the expected abnormal buy and hold return under the filter rule trading strategy considered in this section does _not_ turn out to be zero. This result has important implications for empirical work involving abnormal returns calculated in terms of the buy and hold returns which accrue on stocks (Jegadeesh, and Titman 1993; Jegadeesh, and Titman 2001). 

24 

Table 1. Conditional expected annual logarithmic return on a stock whose price evolves in terms of the Feller diffusion with an instantaneous expected buy and hold return of  _= 0.15_ and volatility parameter of  _= 1_ . 

|||<br><br><br>)<br>1<br>(<br>}<br>)]<br>)1(<br>(<br>3.0<br>exp[<br>{<br>_x_<br>_dy_<br><br>_y_<br>_x_<br>_E_|]}<br><br><br><br>)1(<br>{log[ <br>_x_<br>_E_|Probability of|
|---|---|---|---|---|
|x(0)||)<br>0<br>(<br>_x_<br>_y_|)<br>0<br>(<br> _x_|Bankruptcy|
|<sup>2</sup><br><sup>10</sup>|||||
|2<br> = <br>3|0.15|-0.1605|-0.0105|0.3679|
|5|0.15|-0.1035|0.0465|0.2231|
|10|0.15|-0.0488|0.1012|0.0498|
|15|0.15|-0.0320|0.1180|0.0111|
|20|0.15|-0.0238|0.1262|0.0025|
|25|0.15|-0.0189|0.1311|0.0006|
|30|0.15|-0.0157|0.1343|0.0001|
|35|0.15|-0.0134|0.1366|0.0000|
|40|0.15|-0.0117|0.1383|0.0000|
|45|0.15|-0.0104|0.1396|0.0000|
|50|0.15|-0.0094|0.1406|0.0000|
|55|0.15|-0.0085|0.1415|0.0000|
|60|0.15|-0.0078|0.1422|0.0000|
|65|0.15|-0.0072|0.1428|0.0000|
|70|0.15|-0.0067|0.1433|0.0000|
|75|0.15|-0.0062|0.1438|0.0000|
|80|0.15|-0.0058|0.1442|0.0000|
|85|0.15|-0.0055|0.1445|0.0000|
|90|0.15|-0.0052|0.1448|0.0000|



Notes: This table determines the expected annual logarithmic return as defined by equation (27) of the text for a stock with an opening price of _x(0)_ and whose terminal price, _x(1)_ , evolves in terms of the Feller diffusion with parameter values of  _= 0.15_ and  _= 1_ , respectively.  The integral expression summarised in column 3 is evaluated using 50 point Gauss-Legendre quadrature whilst the expectation of the integral expression is evaluated using 50 point Gauss-Laguerre quadrature (Carnahan, Luther and Wilkes 1969, 101-105).  The probability of eventual extinction (that is, the probability of bankruptcy) appearing in the final column of the table is computed using equation (A4) in the Appendix. 

25 

## **References** 

Abramowitz, M. and Stegun, I., 1964. _Handbook of mathematical functions with formulas, graphs, and mathematical tables_ . Washington, DC: NBS Applied Mathematics Series 55, National Bureau of Standards. 

Ashton, D., Cooke, T. and Tippett, M., 2003. An aggregation theorem for the valuation of equity under linear information dynamics. _Journal of Business Finance and Accounting_ , 30 (3&4), 413-440. 

Ball, R. and Brown. P., 1968. An empirical evaluation of accounting income numbers. _Journal of Accounting Research_ , 6(2), 159-178. 

Bernard, V. and Thomas, J., 1989. Post-earnings announcement drift: Delayed price response or risk premium. _Journal of Accounting Research_ . 27(Supplement), 1-36. 

Carnahan, B. Luther, H and Wilkes, J., 1969. J., _Applied Numerical Methods_ . (New York: Wiley). 

Chan L., Jegadeesh, N. and Lakonishok, J., 1996. Momentum strategies. _Journal of Finance_ , 1996, 51(5), 1681-1713. 

Cox, D. and Miller, H., 1965. _Theory of stochastic processes_ . (London: Chapman and Hall). 

Cox, J., 1996. The constant elasticity of variance option pricing model. _Journal of Portfolio Management_ . Special Issue, 15-17. 

Cox, J. and Ross, S., 1976. The valuation of options for alternative stochastic processes. _Journal of Financial Economics_ , 3(1&2), 145-166. 

D’Aspremont A., 2011. Identifying small mean reverting portfolios, _Quantitative Finance_ , 11(3), 351-364. 

Davidson, I. and Tippett, M., 2012. _Principles of equity valuation_ . (Abingdon, Oxon: Routledge). 

Elton, E., Gruber, M. and Gultekin, M., 1984. Professional expectations: accurary and diagnosis of errors. _Journal of Financial and Quantitative Analysis_ . 19(4), 351-363. 

Fama, E., 1988. Market efficiency, long-term returns, and behavioral finance. _Journal of Financial Economics_ . 49(3), 283-306. 

Feller, W., 1951a. Diffusion processes in genetics. _Proceedings of the Second Berkley Symposium on Mathematical Statistics and Probability_ . 54(1), 227-246. 

Feller, W., 1951b. Two singular diffusion processes.” _Annals of Mathematics_ . 173182. 

26 

Gan, X. and Waxman, D., 2015. Singular solution of the Feller diffusion equation via a spectral decomposition. _Physical Review E_ . 91(1), 012123-1-10. 

Gibbons, M. and Ramaswamy, K., 1993. A test of the Cox, Ingersoll and Ross model of the term structure. _Review of Financial Studies_ . 6(3), 619-658. 

Grinblatt, M., Masulis, R. and Titman, S., 1984. The valuation effects of stock splits and stock dividends. _Journal of Financial Economics_ . 13(4), 461-490. 

Hoel, P., S. Port, and C. Stone. 1987. _Introduction to Stochastic Processes_ . Long Grove, IL:Waveland Press. 

Hong H. and Stein, J., 1999. A unified theory of underreaction, momentum trading, and overreaction in asset markets. _Journal of Finance_ . 54(6), 2143-2184. 

Jegadeesh, N. and Titman, S., 1993. Returns to buying winners and selling losers: implications for stock market efficiency. _Journal of Finance_ . 48(1), 65-91. 

Jegadeesh, N. and Titman, S., 2001. Profitability of momentum strategies: an evaluation of alternative explanations. _Journal of Finance_ . 56(2), 699 -720. 

Jones, M. and Pewsey, A. 2009. Sinh-arcsinh distributions. _Biometrika_ . 96(4), 761780. 

Karlin, S. and Taylor, H., 1981. _A Second Course in Stochastic Processes_ . (London: Academic Press). 

Lakonishok, J. and Vermaelen, T., 1990. Anomalous price behavior around repurchase tender offers. _Journal of Finance_ . 45(2), 455-477. 

Michaely, R., Thaler, R. and Womack, K., 1995. Price reaction to dividend initiations and omissions: overreaction or drift. _Journal of Finance_ . 50(2), 573-608. 

Nariboli, G., 1977. Group invariant solutions of the Fokker-Planck equation. _Stochastic Processes and their Applications_ . 5(2), 157-171. 

Nelson, D. and Ramaswamy, K., 1990. Simple binomial processes as diffusion approximations in financial models. _Review of Financial Studies_ , 3(3), 393-430. 

Sneddon, I. 1961. _Fourier Series_ . London: Routledge & Kegan Paul Ltd. 

Sweeney, R., 1988. Some new filter rule tests: methods and results. _Journal of Financial and Quantitative Analysis_ . 23(3), 285-300. 

