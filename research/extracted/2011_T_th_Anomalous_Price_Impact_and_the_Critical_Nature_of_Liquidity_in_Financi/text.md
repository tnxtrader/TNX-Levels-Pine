PHYSICAL REVIEW X 1, 021006 (2011) 

# Anomalous Price Impact and the Critical Nature of Liquidity in Financial Markets 

B. To´th, Y. Lempe´rie`re, C. Deremble, J. de Lataillade, J. Kockelkoren, and J.-P. Bouchaud Capital Fund Management, 6, blvd Haussmann 75009 Paris, France (Received 9 May 2011; published 31 October 2011) 

We propose a dynamical theory of market liquidity that predicts that the average supply/demand profile is V shaped and vanishes around the current price. This result is generic, and only relies on mild assumptions about the order flow and on the fact that prices are, to a first approximation, diffusive. This naturally accounts for two striking stylized facts: First, large metaorders have to be fragmented in order to be digested by the liquidity funnel, which leads to a long memory in the sign of the order flow. Second, the anomalously small local liquidity induces a breakdown of the linear response and a diverging impact of small orders, explaining the ‘‘square-root’’ impact law, for which we provide additional empirical support. Finally, we test our arguments quantitatively using a numerical model of order flow based on the same minimal ingredients. 

DOI: 10.1103/PhysRevX.1.021006 Subject Areas: Complex Systems, Interdisciplinary Physics, Statistical Physics 

## I. INTRODUCTION 

Price impact refers to the correlation between an incoming order (to buy or to sell) and the subsequent price change [1–3]. That a buy (sell) trade should push the price up (down) is intuitively obvious and is easily demonstrated empirically (see [3] for a recent review). Such a mechanism must, in fact, be present for private information to be incorporated into market prices. But it is also a sore reality for large trading firms for which price impact induces extra costs. Indeed, large volumes must typically be fragmented and executed incrementally. However, since each ‘‘child order’’ pushes the price up or down, the total cost of the ‘‘metaorder’’<sup>1</sup> is quickly dominated, as sizes become large by the average price impact. Monitoring and controlling impact has therefore become one of the most active domains of research in quantitative finance since the mid-nineties. A huge amount of empirical results has accumulated over the years, in particular, concerning the relation between the total size Q of the metaorder and the resulting average price change. These empirical results come from either proprietary trading strategies (and are often not published), or from brokerage firms, who execute on behalf of clients [4–8], or else from the exchanges, who give exceptional access to identification codes that allow one to reconstruct the metaorders from some market participants [9,10]. Remarkably, although these data sets are extremely heterogeneous in terms of markets (equities, futures, FX, etc.), epochs (from the mid-nineties, when liquidity was provided by market makers, to the present 

Published by the American Physical Society under the terms of the Creative Commons Attribution 3.0 License. Further distribution of this work must maintain attribution to the author(s) and the published article’s title, journal citation, and DOI. 

> 1We call the metaorder (or parent order) the bundle of orders corresponding to a single trading decision. A metaorder is typically traded incrementally through several child orders. 

day’s electronic markets), market participants and underlying trading strategies (fundamental, technical, etc.), and style of execution (using limit or market orders, with high or low participation ratio, etc.), a very similar concave impact law is reported in most studies. More precisely, the average relative price change � between the first and the last trade of a metaorder of size Q is well described by the so-called ‘‘square-root’’ law: 



where � is the daily volatility of the asset and V is the daily traded volume, and both quantities are measured contemporaneously to the trade. The numerical constant Y is of order unity. Published and unpublished data suggest slightly different versions of this law; in particular, the pfQ **f** i f **f** i dependence is more generally described as a powerlaw relation �ðQÞ / Q<sup>�</sup> , with � in the range 0.4 to 0.7 [4–9]. For example, using a large data sample of 700 000 U.S. stock trade orders executed by Citigroup Equity Trading, Almgren et al. [4] extract � � 0:6. Moro et al. [9] report � � 0:5 for trades on the Madrid stock exchange and � � 0:7 for the London stock exchange. We show in Fig. 1 our own proprietary data corresponding to nearly 500 000 trades on a variety of futures contracts, which yield � � 0:5 for small tick contracts and � � 0:6 for large tick contracts, for Q=V ranging from a few 10<sup>�4</sup> to a few percent. Our data on stocks is also compatible with � � 0:5, although noisier. We note that all these studies differ quite significantly in the details of (a) how the price impact � is defined and measured, (b) how different assets and periods are collated together in the analysis, and (c) how the fit is performed: over what range of Q=V, adding an intercept or not, etc. But in spite of all these differences and those mentioned above—in particular concerning the strategies motivating the trades—it is quite remarkable that the 

2160-3308=11=1(2)=021006(11) 

021006-1 

Published by the American Physical Society 

B. TO<sup>´</sup> TH et al. 

PHYS. REV. X 1, 021006 (2011) 



<!-- Start of picture text -->
Impact<br>Jun. 2007 - Dec. 2010<br>-1<br>10<br>Small ticks<br>Large ticks<br>δ=1/2<br>δ=1<br>-2<br>10<br>-3<br>10<br>-5 -4 -3 -2<br>10 10 10 10<br>Q / V<br>∆/σ<br><!-- End of picture text -->

FIG. 1. The impact of metaorders for Capital Fund Management proprietary trades on futures markets, in the period from June 2007 to December 2010. Impact is measured here as the average execution shortfall of a metaorder of size Q. The data base contains nearly 500 000 trades. We show �=� vs Q=V on a log-log scale, where � and V are the daily volatility and daily volume measured the day the metaorder is executed. The blue curve is for large tick sizes, and the red curve is for small tick sizes. For large ticks, the curve can be well fit with � ¼ 0:6, while for small ticks we find � ¼ 0:5. For comparison, we also show the lines of slope 0.5 (corresponding to a square-root impact) and 1 (corresponding to linear impact). We have removed a small positive intercept �=� ¼ 0:0015 for Q ¼ 0, which is probably due to a conditioning effect. 

square-root impact law appears to hold approximately in all cases. 

The aim of the present paper is to provide a theoretical underpinning for such a universal impact law. We first give a general dynamical theory of market liquidity that predicts that the average supply/demand profile is V shaped around the current price. The anomalously small local liquidity induces a breakdown of the linear response and explains the square-root impact law. We then study numerically a stylized model of order flow based on minimal ingredients. The numerical results fully support our analytical arguments and allow us to get quantitative insights into various aspects of the problem. 

## II. AN INTRIGUING IMPACT LAW 

One should first carefully distinguish the total impact of a given metaorder of size Q from other measures of impact that have been reported in the literature. One is the immediate impact of an individual market order of size q, which has been studied by various authors and is also strongly concave as a function of q, i.e., q<sup>�</sup> with � � 0:2, or even lnq [3]. Another easily accessible measure of impact is to relate the average price change �T in a given time interval T to the total market order imbalance QT in the same time 

period, i.e., the sum of the signed volumes of all market orders. This quantity is estimated using all the trades in the market (i.e., those coming from different market participants) and is clearly different from the impact of a given metaorder (see below). However, there seems to be quite a bit of confusion in the literature and many authors unduly identify the two quantities. If T is very short, such that there are only one or a few trades, one essentially observes the concave impact of individual orders that we just mentioned. But as T increases, and as such, the number of trades becomes large, the relation between �T and QT becomes more and more linear for small imbalances (see, e.g., [3], Fig. 2.5), and on time scales comparable to those needed to complete a metaorder, the concavity has almost disappeared, except in rare cases when QT=V is large—in any case, much larger than the region where Eq. (1) holds. 

A square-root singularity for small traded volumes is highly nontrivial, and certainly not accounted for in Kyle’s classical model of impact [11], which predicts a linear impact � / Q. A concave impact function is often thought of as a saturation of impact for large volumes. We believe that the emphasis should rather be placed on the anomalous high impact of small trades. Numerically, Eq. (1) means that trading 100th of the daily volume moves the price by a tenth of its daily volatility, which is indeed a huge amplification. Mathematically, Eq. (1) implies that the marginal impact diverges for small volumes as Q<sup>�1=2</sup> , which means that the susceptibility of the market to trades of vanishing size is formally infinite. In most systems, the response to a small perturbation is linear, i.e., small disturbances lead to small effects. The breakdown of the linear response often implies that the system is at, or close to, a critical point, where very special properties emerge, such as long-range memory or scale-invariant avalanches, that accompany this diverging susceptibility. Of course, the mathematical divergence is cut off in practice—for one thing, the volume Q of a metaorder cannot be smaller than a single lot. Empirical data will never be in the asymptotic limit Q=V ! 0, but this is irrelevant to our discussion. This is, in fact, also the case for most physical systems for which critical behavior is observed. The important point here is that the proximity of a critical point can lead to strongly nonlinear effects and extreme fragility. As we will argue in detail below, and substantiate within a precise numerical model, we believe that markets operate in a critical regime where liquidity vanishes. This offers a framework to understand many of the anomalies in the behavior of markets, including the long-term memory in order flow and the presence of frequent unexplained jumps in prices, that are—or so we believe—a consequence of the chronic lack of liquidity that leads to a micro crisis. The anomalous high impact of small trades implied by the concave impact law, Eq. (1), is, in our view, another side of the same coin. 

Numerous interpretations have been put forth to explain a concave impact law, and can be broadly classified into 

021006-2 

PHYS. REV. X 1, 021006 (2011) 

ANOMALOUS PRICE IMPACT AND THE CRITICAL . . . 

three types of mechanisms (which are not necessarily exclusive): (a) risk-reward optimization of the liquidity providers and market makers, [12–16], (b) surprise in the order flow and decay of impact [3,17], and (c) locally linear supply profiles [18]. Many of the above stories require that liquidity providers know the fundamental value, carefully monitor the order flow, identify metaorders, and adjust their quotes such that they eke out some profit or at least break even. There is no doubt that a fraction of market participants strive to achieve such goals and develop astute algorithms with this aim. However, liquidity providing is not (anymore) the monopoly of these market participants, who compete with anyone placing limit orders as part of an execution strategy. Further, the concave impact law, in fact, holds even when a substantial fraction of the metaorder is executed using limit orders. That individual metaorders can be detected using statistical methods, as advocated in [17], may well be true for large metaorders of unsophisticated traders, but it would be surprising (although not impossible) that our own trades, which are used to obtain Fig. 1, can be systematically detected. The universality of the concave impact law suggests that a robust self-organizing mechanism is at play. Our thesis, that we will substantiate below using both analytical arguments and numerical simulations, is that one can indeed build a theory of impact that relies on minimal assumptions, with no reference whatsoever to notions such as fundamental prices, market maker profit, or adverse selection. 

## III. A DYNAMICAL THEORY FOR LINEAR SUPPLY/DEMAND PROFILES 

An interesting idea is that the supply/demand curve is a growing function of the difference between the fundamental value p0 and the price. More precisely, suppose the available volume for sells (respectively, buys) at price p or above (below) V �ðpÞ is a linear function, �b�ðp � p0Þ. The execution of a volume Q of buy orders must, therefore, take place by moving the price by a quantity � such that 



and similarly for sells. Therefore, if the supply/demand profile is linear, the impact is a square root: �� � f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i ffi p2Q=b�. It is indeed tempting to wave hands and argue that the available volume is proportional to the mispricing jp � p0j, but one soon hits a major impediment: What exactly is p0? If the above argument is to make any sense, p0 is the fundamental value when the metaorder starts trading, and the assumption is that p0 should not vary too much (compared to �) during the execution of the metaorder. But this is absurd: There is no equilibrium price 

around which the market pauses; prices move all the time, in a diffusive manner and in such a way that the difference between p0 and the final price pT is in fact much larger than the impact � we try to account for. If the linear profile follows the price instantaneously, the above argument completely falls into pieces since only the locally available volume would play a role. If the linear profile for some reason remains centered for some time around a specific p0, why should this price coincide with the price at the beginning of the metaorder? 

Still, the basic intuition, that the available volume grows as price excursions get larger, must somehow make sense. The aim of the present section is to propose a dynamic theory of liquidity largely inspired from [19], based on minimal and plausible assumptions that indeed predicts that the average supply (or demand) is a V-shaped curve that vanishes around the current price pt. The square-root impact then follows from an argument similar to Eq. (2). These arguments are then tested quantitatively using a numerical model of order flow based on the same minimal ingredients. 

Our basic idea is that of a ‘‘latent order book,’’ which, at any instant of time t, aggregates the total intended volume for sells at price p or above V þðp; tÞ, and the total intended volume for buys at price p or below V �ðp; tÞ. We want to emphasize that this is, in general, not the volume revealed in the real (observable) order book, in particular, for p, which is remote from the current price pt. It is rather the volume that would reveal itself in the order book, or as market orders, if the price came instantaneously closer to p. But since there is little incentive to reveal one’s intentions too early, most of the volume is latent and not revealed. This is obvious from the basic order of magnitude estimates: Whereas the total instantaneous volume in the real order book of a typical liquid stock is of the order of 10<sup>�5</sup> of the market capitalization, the total transaction volume per day is 10<sup>�3</sup> , which shows that liquidity is a dynamical process. The empirical analysis of Weber and Rosenow [20] shows very clearly how the volume appearing in the order book is indeed stimulated by the trades themselves. 

So, our latent volumes V �ðp; tÞ reflect intentions that do not necessarily materialize. How do these volumes evolve with time? Between t and t þ dt, new buyers and sellers may become interested at levels pt � u, at a rate �ðuÞ and with unit volume q ¼ 1; while existing buyers and sellers at pt � u might change their price to pt � u<sup>0</sup> at rate �ðu; u<sup>0</sup> Þ, or even disappear temporarily (corresponding to u<sup>0</sup> ¼ 1).<sup>2</sup> Clearly, V þðp < pt; tÞ ¼ 0 and V �ðp > pt; tÞ ¼ 0, 

> 2The following equations would not change if we allow q to fluctuate, provided the average of q is finite and set to unity without loss of generality. We furthermore assume complete symmetry between p > pt and p < pt, i.e., �þðuÞ ¼ ��ðuÞ, etc. 

021006-3 

B. TO<sup>´</sup> TH et al. 

PHYS. REV. X 1, 021006 (2011) 

which means that there cannot be an unsatisfied seller (buyer) below (above) the current price pt. 

We now assume that the price process pt is a Brownian walk,<sup>3</sup> which is only an approximation since in practice (a) at short times, microstructure effects play a role and (b) large jumps are present and, in fact, quite frequent (the distribution of price changes is well known to be a power law for large arguments [22]). This approximation, however, allows us to make precise analytical calculations that illustrate our main point. Since we are interested in phenomena that take place on time scales of a few minutes to a few days, drift effects are completely negligible and we discard them. For the same reason, the difference between an additive and a geometric Brownian motion is irrelevant. A simple equation for the latent volume, averaged over price paths V �ðp; tÞ, can be obtained by working in the reference frame moving with the price pt, provided an extra assumption is made on the rates �ðu; u<sup>0</sup> Þ. We assume that either u<sup>0</sup> ¼ 1, with rate �1ðuÞ, or that the change of price is small, and occurs equally, often up or down. We define DðuÞ ¼<sup>R</sup> du<sup>0</sup> ðu � u<sup>0</sup> Þ<sup>2</sup> �ðu; u<sup>0</sup> Þ, where the integral over u<sup>0</sup> is rapidly convergent (small jumps); it can be interpreted as the (squared) volatility of intentions. With DðuÞ ¼ DðuÞ þ �<sup>2</sup> , where � is the <u>price</u> volatility, the final equation for ��ðu; tÞ ¼ V �ðpt � u; tÞ reads [23]<sup>4</sup> 



Note that all rates are symmetric for buy orders and sell orders, the long-time, stationary solution �stðuÞ is the same for �þ and ��. It describes the most probable shape of the latent order book, and is such that the right-hand side of the above equation is zero. For arbitrary functions DðuÞ, �ðuÞ, and �1ðuÞ, the explicit form of �stðuÞ is not known, but provided these functions are regular when u ! 0, one can show that the stationary profile is linear and close enough to the current price, i.e., �stðuÞ � bu when u ! 0, where b is a certain finite constant. In fact, 



is the transaction rate per unit time, i.e., the volume of buy or sell market orders per unit time. If we choose the unit of 

3That the price is a diffusive process is a standard assumption in quantitative finance. It is also very well corroborated by data down to very short time scales (see, e.g., [21]), at least in liquid markets. This ‘‘statistical efficiency’’ precludes the existence of simple arbitrage strategies. 

> 4Here we assume that FðuÞ ¼<sup>R</sup> du0ðu � u0Þ�ðu; u0Þ ¼ 0 8u, but adding a nonzero drift term in the reconfiguration of orders would not change any of the following conclusions. Only the value of u<sup>�</sup> would change. 

time to be 1 day, J is precisely what we called V above. Provided that DðuÞ is regular at u ¼ 0, the condition �stðuÞ ¼ 0, and J is positive and finite, is enough to impose that the profile is locally linear with b ¼ 2J=Dð0Þ. Therefore, the hypothesis of a diffusive price with a finite transaction rate immediately leads on average to a locally linear order book. 

As a simple illustration, consider the case where new orders appear uniformly, i.e., �ðuÞ ¼ �, and DðuÞ ¼ D independent of u. The exact solution is then 



**f f f f f f f f** with �1 ¼ �=�1 and u<sup>�</sup> ¼ pD=2�1, which leads to b ¼ �1=u<sup>�</sup> . One sees that even when new orders appear with a finite density around the current price, they also have a large probability to be executed and disappear. This eventually leads to a liquidity trough at u ¼ 0 and a linear profile around u ¼ 0; u<sup>�</sup> can be interpreted as the width of the linear region. It is reasonable to think that D and �<sup>2</sup> are comparable, and that 1=�1, which measures the (volume weighted) average lifetime of an intended order, is dominated by slow players and is on the scale of a few days.<sup>5</sup> Therefore, u<sup>�</sup> is of the order of the daily volatility, which shows that the trough in the latent order book extends over a very significant region around the current price. Note that �life ¼ 1=�1 is also the persistence time of the fluctuations of the order book, and the time to reach the stationary state �stðuÞ. This time plays a crucial role in the following. 

Note that if �ðuÞ is not constant but decays over a price range u�, the width of the linear region is still given by u<sup>�</sup> provided u<sup>�</sup> � u�. In the other limit, on the other hand, f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i f **f** one finds u<sup>�</sup> � u� [24]. We believe that u� and pD=2�1 are in fact of the same order of magnitude (a few percent); this means that the players contributing to the true liquidity of the market are not sensitive to price changes much smaller than the daily volatility (see also footnote<sup>5</sup> ). 

Equation (5) contains the central result of the present paper. It predicts that the available volume in the immediate proximity of the current price goes to zero, which is the reason why we say markets are critical, i.e., they operate in a regime of vanishing liquidity. This scenario does not arise by fiat but is rather a natural consequence of the diffusivity of prices: We believe this is a genuine example of selforganized criticality [25]. 

In more concrete terms, the volume at the best quotes (bid or ask), given by qbest � bw<sup>2</sup> =2 (where w is the tick 

5Note here that this is where the distinction between latent and revealed orders is crucial: The average lifetime of orders in the order book is much shorter than this, but this is a result of high frequency strategies which are sensitive to minute price changes, but does not relate to changes of intentions from slow players. Correspondingly, the average shape of the true order book is nonmonotonous [23], and thus very different from the linear prediction, Eq. (5), for the latent liquidity profile. 

021006-4 

PHYS. REV. X 1, 021006 (2011) 

ANOMALOUS PRICE IMPACT AND THE CRITICAL . . . 

size) is �ðw=u<sup>�</sup> Þ smaller than the volume �1w one would expect in the absence of the above sweeping mechanism. This is typically small since w � 0:05% and u<sup>�</sup> � 2% (for stocks or futures). One can also compare qbest to the total volume traded in a time T, which is V ¼ JT � �1u<sup>�</sup> if one chooses T ¼ �life� 1 day. The ratio is now qbest=V � ðw=u<sup>�</sup> Þ<sup>2</sup> , which is very small, as is indeed the case empirically: The immediately accessible volume is typically a factor of 1000 smaller than the daily turnover. This small liquidity compels market participants to fragment their trades, which leads to the universally observed long-range correlation in the sign of market orders [21,26]. It also leads to the square-root impact law, if one trusts the argument after Eq. (2), where b ¼ �1=u<sup>�</sup> . This gives f **f** i f **f** f **f** iffi � � q2bQ / �qQJ , where we have used b � J=�<sup>2</sup> , where J is the trading rate. This is exactly the square-root impact law, Eq. (1). 

However, this last argument is quite naı¨ve since the average impact � is much smaller (for small Q) than the typical excursion of the price within the same period. Furthermore, the diffusive behavior of the price is taken for granted in the above calculation, whereas, in fact, it results from a subtle compensation [21] between a confinement effect created by the linear supply/demand curve (a price movement in one direction hits larger opposing volumes and is more likely to revert) and a correlation effect created by the fragmentation of the trades [27,28]—itself imposed by the liquidity trough mentioned above. 

Since we are unable, at this stage, to treat these effects consistently within an analytical framework, we now turn to a minimal numerical model that captures all the above effects. We will indeed find a linear demand profile and a concave impact function, and gain considerable insights into the mechanisms leading to such behavior. 

## IV. A NUMERICAL MODEL FOR AN EFFICIENT MARKET WITH LONG-RANGED ORDER FLOW 

The numerical implementation of the above simple Poisson model for intended order flow is quite simple, and we follow the framework proposed in [19]—see also the Appendix below. All orders have a unit volume. Limit orders are launched at a uniform rate � in a finite (but large) interval around the current price. Existing limit orders are individually cancelled at rate �1. If market orders are themselves completely random, with buys and sells drawn independently with probability <u>12</u><sup>and at rate �, the resulting</sup> price motion is known to be strongly subdiffusive, in the sense that the lag dependent diffusion constant �<sup>2</sup> ð‘Þ � hðpt � ptþ‘Þ<sup>2</sup> i=‘ decays when ‘ increases (here time is counted in number of transactions). This was noted in [19], and is a result of the confining effect of the supply/ demand curve. The price only becomes diffusive on time scales larger than the memory time �life. Nothing of that 

sort is seen in real price dynamics. However, we know from empirical data [3,21,26] that the signs of transactions �t are in fact long-ranged correlated, i.e., that Cð‘Þ ¼ h�t�tþ‘i is decaying as a power law, Cð‘Þ � ‘<sup>��</sup> , where � � 0:5 for single stocks and � � 0:8 for futures markets [29]. This persistent direction of trading can counterbalance the confinement effect and restore diffusion. In fact, if � is too small, one expects superdiffusion, i.e., �<sup>2</sup> ð‘Þ growing with ‘. 

We therefore want to upgrade the ‘‘zero-intelligence’’ model of [19] to an ‘‘"-intelligence’’ numerical model, which explicitly includes the long-ranged correlated nature of the trades, reflecting the fact that large metaorders are fragmented and traded incrementally; see Ref. [27,28]. We have chosen to work with the Lillo-Mike-Farmer specification of the sign process [27], i.e., sequences of þ and � signs (buy and sell market orders) are generated, with lengths L drawn from a power-law distribution: PðLÞ � L<sup>�ð�þ1Þ</sup> . It can be shown that the sign process has an autocorrelation function that decays asymptotically as Cð‘Þ ¼ ‘<sup>��</sup> with � ¼ � � 1. We do not expect that the following results depend much on the precise specification of the model of signs [30]. 

At this stage, the model contains four parameters: the rate of limit orders �, the rate of cancellation �1, the rate of market orders �, and the autocorrelation exponent �. In fact, only three of them are relevant up to a change of time unit: the ratios �=�1 and �=�1, and �. As noted above, the ratio �1 ¼ �=�1 determines the supply depth far away from the best quotes, whereas the ratio r ¼ �=�1 tells us whether we are in a ‘‘slow’’ market, where the renewal time �life of the supply/demand is much longer than the time between individual trades (r � 1), or if the market is ‘‘fast’’ in the opposite limit r � 1. It is clear that real markets are in the former limit: If trades take place on a scale of seconds while the renewal time is of the order of hours or days, the ratio r is on the order of 10<sup>4</sup> . As will be discussed later, r � 1 is a crucial condition for a concave impact law to hold. On the other hand, � and � � w are of similar orders of magnitude, which means that, as expected, markets are also ‘‘deep’’: �1 � w � 1. 

The problem is that in the limit of deep and slow markets, the above model is always in the subdiffusive phase unless � is very close to zero (see Fig. 2 below). We therefore need an extra ingredient to make markets statistically efficient (meaning that prices are diffusive), while keeping � in the empirical range � � 0:5–0:8. When compared with real markets, the above model is obviously much too simple. For example, the size of both limit and market orders is known to be broadly distributed, whereas we assume, as in [19,31], that all volumes are of unit size. The direction and the size of market orders are furthermore strongly conditioned by the volumes at the best quotes qbest: The volume of market orders is larger when the offered volume is larger, and the sign of the next market 

021006-5 

B. TO<sup>´</sup> TH et al. 

PHYS. REV. X 1, 021006 (2011) 



<!-- Start of picture text -->
subdiffusive<br>superdiffusive<br>0.0 0.2 0.4 0.6 0.8 1.0<br>γ<br>3.0<br>2.5<br>2.0<br>ζ<br>1.5<br>1.0<br>0.5<br>0.0<br><!-- End of picture text -->

FIG. 2. Map of diffusivity in the plane �; � for � ¼ 0:5, � ¼ 0:1, �1 ¼ 10<sup>�4</sup> . The large �, large � region corresponds to subdiffusion, while the small �, small � region corresponds to superdiffusion. The efficient market (diffusive) line determined numerically is such that �ð0:8Þ � 0:65, �ð0:5Þ � 0:95, and �ð0:3Þ � 2:5 (crosses). Note that the unit volume limit � ! 1 corresponds to � ! 0. Without volume fluctuations, deep and slow markets are therefore always found to be in the subdiffusive phase. 

order is anticorrelated with the volume imbalance (i.e., when qbid � qask, the next trade is most likely to be a buy and vice-versa). 

We have chosen to add one extra parameter both to make a step toward reality and to solve the efficiency problem in the following way: The volume of market orders is chosen to be a random fraction f of the volume at the opposite best quote, where the distribution of f is given by P� ðfÞ ¼ �ð1 � fÞ<sup>��1</sup> and where � is a parameter (�> 0) that determines the typical relative volume of market orders. For � ! 0, the distribution P� ðfÞ peaks around f ¼ 1 and most orders ‘‘eat’’ all the available liquidity; � ¼ 1 corresponds to a flat distribution for the fraction of eaten volume. Finally, the limit � ! 1 corresponds to very small (unit) volumes and recovers the previous model. The correlation between the volume at the best quote and the volume of the impinging market order has been reported in many papers (see [32]). This makes perfect sense: Since large metaorders must be fragmented because of the small available liquidity, one expects that the executed volume opportunistically follows the offered liquidity. 

With the help of the extra parameter � we can now tune the model to guarantee diffusive prices for any value of �, even in the limit of deep and slow markets. We define a measure of pure diffusivity as �ð‘1Þ=�ð‘2Þ for two time scales ‘1 and ‘2 > ‘1. Subdiffusion corresponds to ratios less than 1, superdiffusion corresponds to ratios greater than 1, and for pure diffusion this ratio must be equal to unity for all ‘1; ‘2. In our simulations, we chose � ¼ 0:5, � ¼ 0:1, �1 ¼ 10<sup>�4</sup> (corresponding to �1 ¼ 5000 � 1 and r ¼ 1000 � 1). We determine the diffusion line in the 



<!-- Start of picture text -->
empirical results<br>theoretical prediction<br>0.0 0.5 1.0 1.5 2.0 2.5 3.0<br>u<br>1.0<br>0.8<br>0.6<br>ρρ∞<br>0.4<br>0.2<br><!-- End of picture text -->

FIG. 3. Comparison of the stationary density of the book with the expected shape calculated from Eq. (5). The symbols show the empirical shape of the book from the simulations, while the solid line shows the predicted exponential form, with u<sup>�</sup> being computed from the actual price volatility. In the simulations we used the parameters, � ¼ 0:8, � ¼ 0:65, �1 ¼ 10<sup>�4</sup> , � ¼ 0:5. f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i **f** The analytical computations lead to u<sup>�</sup> ¼ pD=2�1 ¼ 0:49, while a direct fit to Eq. (5) leads to u<sup>�</sup> � 0:48. 

plane �; � that separates the subdiffusion regime from the superdiffusion regime, by setting ‘1 ¼ 10, ‘2 ¼ 1000.<sup>6</sup> The result is shown in Fig. 2. As expected, smaller �’s favor superdiffusion, whereas larger values of � (corresponding to less aggressive market orders) favor subdiffusion. On the boundary between the two regimes we find the purely diffusive, efficient markets we are looking for. In the following, we fix the value of �ð�Þ so that it is exactly diffusive.<sup>7</sup> Since the value of � is fixed by the condition of price diffusivity, our model still has three parameters: depth (�1), inverse speed (r), and trade persistence (�). 

To illustrate the tight connection between the dynamical theory discussed in the previous section and the above described numerical model, in Fig. 3 we compare the stationary density of the book to its expected shape calculated from Eq. (5). The symbols show the stationary density �st measured in the simulations, while the solid line is Eq. (5) f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i f **f** i **f** with u<sup>�</sup> ¼ pD=2�1, with D being the actual measured price volatility. The latent order book is found to be linear in the immediate vicinity of the price, as predicted. The exponential curve with the analytical expression for u<sup>�</sup> leads to a very good fit in the whole range of u. 

We now have a model such that (i) the order flow has a long-range memory but (ii) the price is diffusive. We are thus in a position to test quantitatively our above ideas 

> 6The results are not sensitive to the precise choice of ‘1 and ‘2, provided ‘1, ‘2 � �life. We have explored a wide range of values of �; �; �1. Provided that one remains in the limit of deep and slow markets, the broad picture is unaffected, although the precise location of the diffusion line in Fig. 2 is changed. 

> 7It would be interesting to obtain an analytical form for the diffusion line, but we have not attempted to make a theory for this at the present stage. 

021006-6 

PHYS. REV. X 1, 021006 (2011) 

ANOMALOUS PRICE IMPACT AND THE CRITICAL . . . 

about the linear supply function and the impact of metaorders. In order to do this, we add to the above order flow an extra agent who becomes active at a certain (arbitrary) time t, chooses a random sign �, and a random size Q for his metaorder, which he executes incrementally using market orders until all the volume is transacted. We have considered two execution styles: (a) ‘‘� execution,’’ where the extra agent trades exactly as the rest of the market, by sending a market order of volume f � qbest, where f is chosen according to P� ðfÞ above; (b) ‘‘unit execution,’’ where the market orders are all of unit volume whenever he trades. (We also studied limit order execution with similar results; see [30].) In both cases, he participates to a fraction � of all market orders. We measure, as the real data shown in Fig. 1, the impact � as the price paid by the extra agent compared to the price pt at the beginning of the metaorder, averaged over many different metaorders of size Q. The results are shown in Fig. 4, where we show in a log-log scale �=� on the y axis versus Q=V on the x axis, for different values of �, r, and �, and for both � execution and unit execution. We also show two straight lines of slope � ¼ 1=2, corresponding to a square-root impact, and � ¼ 1, corresponding to a linear impact. It is clear that for all cases where the execution time T is much smaller than the renewal time �life, the impact is strongly concave and, in a first approximation, independent of � and of the participation rate �, which is in agreement with empirical observations. The exponent � is found to be 



<!-- Start of picture text -->
δ = 0.5<br>δ = 1<br>0.001 0.005 0.020 0.050<br>Q V<br>0.50<br>0.20<br>∆σ<br>0.05<br>0.01<br><!-- End of picture text -->

FIG. 4. Impact of metaorders on log-log scale. We show �=� vs Q=V, where � and V are measured over the time scale �life. The three curves with symbols show the impact for the � execution with high participation rate, � ¼ 0:3, and three values of the sign autocorrelation exponent: � ¼ 0:3 (green crosses), � ¼ 0:5 (black circles), � ¼ 0:8 (red triangles). The two blue lines show the impact for low participation rate � ¼ 0:05, for the � execution (full line) and unit execution (dashed line), both for � ¼ 0:5. The purple dots show a case when the time to complete metaorders is longer than the lifetime of the book T > �life. In this case we get back a linear impact. The two solid black lines are of slope � ¼ 0:5 and � ¼ 1, for comparison, with the choice Y ¼ 1 for � ¼ 0:5. 

close to 1=2 for unit execution, and close to 2=3 for � execution. The Y factor, defined in Eq. (1), can be measured from the data shown in Fig. 4, and is of order unity, as found empirically. More details about these results will be given in [30]. 

Some parameters of the model, however, do influence the value of the effective exponent �, which might explain why empirical data show some scatter around the value � ¼ <u>12</u><sup>.</sup> In particular, when the execution time T increases and becomes comparable to the renewal time �life, the effective exponent � increases and the impact becomes linear in the limit T � �life; see Fig. 4. This is indeed expected since impact is necessarily additive when all memory of the latent order book has been erased. It is also a direct proof that the persistence of the supply/demand is central to explain the functional form of the impact. The naive prediction for the price impact, Eq. (2), based on the average slope of the supply curve, is shown in Fig. 5 (left), together with the numerical determination of �ðQÞ already plotted in Fig. 4. We see that the naive argument indeed leads to the correct order of magnitude for the impact, but fails to be quantitatively accurate: It underestimates the real impact by a factor of approximately 2. On the same graph, we also show the global measure of the impact mentioned above, where the average price change is plotted against the total volume imbalance Q. We see that the latter is very different from the impact of a given metaorder. The global impact is linear in Q (as observed on empirical data for large enough T) and, for small volume imbalances, much smaller than the impact of an additional metaorder that perturbs the equilibrium flow. 

Another interesting aspect of the problem is the temporal structure of the impact of metaorders. In Fig. 5 (right) we show the average difference �� between the price pt of the first trade of a metaorder and the price ptþ� at time � later, in the direction of the trade. The x axis is in units of the time needed to complete the metaorder. The exact shape of the buildup in time depends on the execution mechanism, which we will discuss in detail in [30]. Here we focus on the price dynamics after the completion of the metaorder. Once the metaorder is completed (i.e., for �> T), the impact decays down to smaller values, and appears to reach a plateau �1, such that �1=�T depends on the value of �, and is approximately 0.75 for � ¼ 0:5. The cause of the reversion in our model is that, during the trading of a metaorder, the order book becomes, on average, unbalanced: Orders on the opposite side become statistically more numerous, as a consequence of the V-shaped liquidity. This results in a partial reversion of the price, once the pressure from the metaorder is over. 

Interestingly, a similar behavior has also been observed in [9], and is predicted by the theory of impact recently put forth in [17], which elaborates on the idea of a ‘‘fair execution price.’’ As emphasized in [9,17], if a metaorder of size Q has an impact that grows as ðqexec=QÞ<sup>�</sup> (where 

021006-7 

B. TO<sup>´</sup> TH et al. 

PHYS. REV. X 1, 021006 (2011) 



<!-- Start of picture text -->
γ = 0.8<br>γ = 0.5<br>γ = 0.3<br>0.00 0.02 0.04 0.06 0.08 0 2 4 6 8 10<br>τ T<br>Q V<br>0.25 1.0<br>0.20 0.8<br>0.15 0.6<br>∆σ<br>∆∆Tτ<br>0.10 0.4<br>0.05 0.2<br>0.00 0.0<br><!-- End of picture text -->

FIG. 5. Left: The true impact of �-executed metaorders for � ¼ 0:5, � ¼ 0:3 (black circles), the naive estimate of the impact from integrating the volume in the average latent order book (blue solid line), and the average price change against the global volume imbalance in a given period (red triangles). We also show a power-law fit for the metaorder impact, as YðQ=VÞ<sup>�</sup> , with � ¼ 0:7 and Y ¼ 1:59 (solid black line) and a linear fit for the global measure of impact (solid red line). Right: The decay of impact after the completion of the metaorder for the unit execution for � ¼ 0:3, 0.5, and 0.8 and � ¼ 0:5. The x axis is �=T, the clock time in units of the time needed to complete the metaorder (with T � �life), the y axis is the rescaled price change during the metaorder. After the metaorder, the price appears to relax to a constant level, which is a � dependent fraction of the temporary impact. The dotted horizontal lines correspond to the actual average execution price of the metaorder, which is found to be � 0:6�T for all three values of �. 

qexec is the volume already executed, i.e., qexec=Q is ‘‘volume time’’),<sup>8</sup> then the average execution price of the metaorder is �T=ð1þ�Þ. In this case, �1=�T � 1=ð1 þ �Þ ensures that the average execution price of the metaorder is equal to the price long after the execution is completed, in other words, that the execution price is fair. Used together with the condition that the price should be a martingale and that the size distribution of metaorders is a power law, Q<sup>�1��</sup> , Farmer et al. [17] obtain a concave impact ��Q<sup>�</sup> with � ¼ � � 1 ¼ �.<sup>9</sup> Using ��3=2 from empirical data on stocks, the square-root impact law is recovered [17], with �1=�T ¼ 2=3. We see, however, in Fig. 5 (right) that the ‘‘fair price’’ condition does not hold in general within our model since the plateau value significantly changes with �, whereas the average price paid is close to 0:6�T in all cases. Nevertheless, for � ¼ 0:5 the plateau is not far from the value 2=3 predicted by Farmer et al. [17]. 

## V. CONCLUSION 

Although some elements of our model are common with the framework of [17] (broad distribution of metaorders 

8Since the impact is �ðQÞ ¼ Y�ðQ=VÞ�, we expect the impact to grow as �qexec � �ðQÞðqexec=QÞ<sup>�</sup> meaning that a metaorder of size Q stopped halfway through execution impacts the price the same way as a metaorder of size Q=2, which a priori makes sense since there is no way to anticipate when a metaorder is going to stop. Indeed, we find the buildup of the impact in volume time to be concave. 

> 9Note that in the theory of Farmer et al. [17], the exponents � and � are equal, whereas in our model � is, to a large extent, unaffected by the precise value of �. 

leading to long-range correlation of the order flow and the efficient price condition), others are very different. The theory advocated in [17] requires that metaorders can be identified by market makers, in particular, that the very first trades can be detected. As the authors openly admit, the need for market participants to be able to detect the starting and stopping of a given metaorder is potentially a problem, and in fact our model does not rely at all on such a strong assumption. On the contrary, we have shown that the universally observed concave impact law is a consequence of some robust, generic assumptions about market dynamics. In particular, we have provided a dynamical theory of liquidity which leads to a locally linear supply/demand curve, provided high frequency strategies guarantee price diffusivity on all time scales. Our story is purely statistical and does not rely on additional (and sometimes woolly) notions such as fundamental prices, adverse selection, or fair pricing. There are no explicit market makers, strategic players, or optimizing agents in our picture, but rather an ecological equilibrium of indistinguishable traders that interact in a way to make the price statistically efficient. 

Although our "-intelligence numerical model makes an important step toward reality (in particular by including long-range correlations in the order flow and ensuring a diffusive price dynamics), there are still many assumptions that are ad hoc. One knows, for example, that the deposition and cancellation rates of limit orders strongly depend on the distance from the current price [33], that the assumption of a Poisson process is an oversimplification [34] (these observations apply to the real order book, but probably will also hold for the latent order book), that the volume of limit orders is not at all constant, etc. Building a 

021006-8 

PHYS. REV. X 1, 021006 (2011) 

ANOMALOUS PRICE IMPACT AND THE CRITICAL . . . 

detailed, realistic model of order flow is of course needed to get fully quantitative predictions for the impact, but was not the scope of the present work. We instead wanted to have a simplified model that would allow us to test our central hypothesis: that the latent order book is locally linear and that this is the crucial ingredient to explain the square-root impact law. We believe that this objective has been reached, and we leave the more ambitious project of building a full-scale model for future work. We also note that our central assumption of a latent order book has other, empirically testable assumptions, which we are currently investigating. 

As we emphasized in the Introduction, understanding impact has immediate practical implications in terms of trading costs and capacity of strategies. In our view, the most important message of the theory presented above concerns the critical, inherently fragile nature of liquidity. By necessity, a diffusive price leads to a vanishing liquidity in the vicinity of the current price. This naturally accounts for two striking stylized facts: First, large metaorders have to be fragmented in order to be digested by the liquidity funnel, which leads to a long memory in the sign of the order flow. Second, the anomalously small local liquidity induces a breakdown of the linear response and a diverging impact of small orders. Furthermore, liquidity fluctuations are bound to play a crucial role when the average liquidity is small, and we expect these fluctuations to be at the heart of the turbulent dynamics of financial markets as postulated in, e.g., [21,35,36]. 

## ACKNOWLEDGMENTS 

We want to thank Paul Besson, Re´my Chicheportiche, Zolta´n Eisler, Pe´ter Horvai, Fabrizio Lillo, Matteo Marsili, Marc Potters, Emmanuel Se´rie´, Damien Trouve´, and especially Doyne Farmer and Jim Gatheral for very helpful discussions and suggestions. 

## APPENDIX 

This Appendix summarizes the main steps of the numerical model discussed in the paper. In the model we consider that the price axis is very large and is discretized by the tick size (i.e., the minimum increment of price). The minimum and maximum price are K ticks away, with K � 1. Time is measured in discrete steps, and in each time step, three types of events can happen: new limit orders are placed, market orders are placed, or extant limit orders are canceled. Many of these events can happen simultaneously during a single time step. 

- (i) Limit orders are orders to buy or sell that do not trigger an immediate trade. According to this, limit orders are placed on a support of size K that is in practice infinite, i.e., much larger than all other price scales in the system. Limit orders placed below the 

   - current midpoint price are considered as limit orders to buy, while those placed above the midpoint price are considered orders to sell. All limit orders have a unit volume in the present version of the model (but this is in no way an essential ingredient). Limit orders arrive with a uniform rate � per unit time per unit price. In practice this means that in each time step, there is a probability �<sup>n</sup> e<sup>��</sup> =n! that n new limit orders fall in each of the K bins of the price axis. 

- (ii) Market orders are orders that trigger an immediate transaction (with existing limit orders on the opposite best price level). They arrive with a rate � per unit time. The sign (direction) of market orders is generated using the algorithm proposed in [27] with one active agent at any instant of time. The volume of market orders is chosen to be a random fraction f of the volume at the opposite best quote where the distribution of f is given by P� ðfÞ ¼ �ð1 � fÞ<sup>��1</sup> , where � is a parameter (�> 0) that determines the typical relative volume of market orders. When � ! 0, market orders take all the prevailing volume on the opposite best. 

- (iii) In each time step, each limit order in the book has the probability �1 of being cancelled. This Poisson process of cancellation defines the typical lifetime of a limit order (as �<sup>�</sup> 1<sup>1).</sup> 

The order of the processes in each time step is the placement of limit orders, followed by the possible execution of market orders, and finally, the cancellation of some limit orders. As shown in Fig. 2, with the help of the parameters � and � we can tune the system to guarantee diffusive prices for any value of the parameters �, �, and �1. This diffusive market allows us to test the predictions of our analytical model about the locally linear profile around the current midpoint price. To do this, we add an extra ‘‘agent’’ to the model, who wishes to transact a metaorder that is typically larger than the available volume on the first level of the book and thus has to be split up and traded incrementally. It is the price change from the beginning to the end of such a metaorder that we would like to study in order to compare the predictions of the model to empirical results. The only remaining point is to define how the extra agent trades. In the present paper we allowed two trading strategies for the execution of the metaorder: (a) � execution, where the extra agent trades exactly as the rest of the market, by sending a market order of volume f � qbest, where f is chosen according to P� ðfÞ above and (b) unit execution, where the market orders are all of unit volume, whenever he trades. 

In Fig. 6 we show a segment of the price process for a simulation run, chosen only for illustration purposes. We plot the price as a function of time (simulation steps). The two simulations shown are governed by the same random seed, however, in one case an extra agent trading a 

021006-9 

B. TO<sup>´</sup> TH et al. 

PHYS. REV. X 1, 021006 (2011) 



<!-- Start of picture text -->
sell metaorder<br>without metaorder<br>with metaorder<br>0 500 1000 1500 2000 2500 3000<br>time<br>254.0<br>253.8<br>price 253.6<br>253.4<br>253.2<br><!-- End of picture text -->

FIG. 6. The price process with and without the metaorder trading, with the same random seed used for the two simulations. The black curve shows the price dynamics without the metaorder, while the green curve shows it with the metaorder. Trades of the metaorder are denoted by blue crosses. In the simulations we used the parameters, � ¼ 0:8, � ¼ 0:65, �1 ¼ 10<sup>�4</sup> , � ¼ 0:5. The participation rate of the metaorder is � ¼ 0:05. 

metaorder to sell is added to the market. The black curve shows the price dynamics without the metaorder, while the green curve shows it with the metaorder trades (the metaorder starts at time 0 and finishes at time 3000). Blue crosses indicate the moments when a trade by the extra agent was made. The agent follows the �-execution strategy in this particular run. For a while the two curves are identical, then after the first trade of the metaorder the two curves start to deviate from each other. The coarse-grained dynamics of the two curves are similar, however, the final price in case of the green curve is pushed down as a result of the extra sell metaorder. 



- [1] J. Hasbrouck, Measuring the Information Content of Stock Trades, Journal of Finance 46, 179 (1991). 

- [2] J.-P. Bouchaud, Price Impact, in Encyclopedia of Quantitative Finance, edited by R. Cont (John Wiley & Sons Ltd, New York, 2010). 

- [3] J.-P. Bouchaud, J. D. Farmer, and F. Lillo, How Markets Slowly Digest Changes in Supply and Demand, in Handbook of Financial Markets: Dynamics and Evolution, edited by T. Hens and K. Schenk-Hoppe (Elsevier: Academic Press, New York, 2008), pp 57–156. 

- [4] R. Almgren, C. Thum, E. Hauptmann, and H. Li, Equity Market Impact, Risk 18, 5762 (2005) [http://corp.bankofamerica.com/publicpdf/equities/ Equity_Mkt_impact.pdf]. 

- [5] R. Engle, R. Ferstenberg, and J. Russell, Measuring and Modeling Execution Cost and Risk, http://papers .ssrn.com/sol3/papers.cfm?abstract_id=1293666. 

- [6] X. Abdobal, Transaction Cost Estimation, Lehman Brothers (2006). 

- [7] A. Ferraris, Market Impact Models, Deutsche Bank (2007). 

- [8] R. Kissel and R. Malamut, Algorithmic Decision Making Framework, J. P. Morgan (2005). 

- [9] E. Moro, J. Vicente, L. G. Moyano, A. Gerig, J. D. Farmer, G. Vaglica, F. Lillo, and R. N. Mantegna, Market Impact and Trading Profile of Hidden Orders in Stock Markets, Phys. Rev. E 80, 066102 (2009). 

- [10] A. J. Menkveld, High Frequency Trading and The NewMarket Makers, http://papers.ssrn.com/sol3/papers.cfm? abstract_id=1722924. 

- [11] A. S. Kyle, Continuous Auctions and Insider Trading, Econometrica 53, 1315 (1985). 

- [12] N. Torre and M. Ferrari, The Market Impact Model, http:// www.mscibarra.com/research/articles/barra/Market_ Impact_Model.pdf. 

- [13] R. C. Grinold and R. N. Kahn, Active Portfolio Management (The McGraw-Hill Companies, Inc., New York, 1999). 

- [14] X. Gabaix, P. Gopikrishnan, V. Plerou, and H. E. Stanley, Institutional Investors and Stock Market Volatility, Quarterly Journal of Economics 121, 461 (2006). 

- [15] G. C. Chacko, J. W. Jurek, and E. Stafford, The Price of Immediacy, The Journal of Finance 63, 1253 (2008). 

- [16] Y. C. Zhang, Toward a Theory of Marginally Efficient Markets, Physica (Amsterdam) 269A, 30 (1999). 

- [17] J. D. Farmer, A. Gerig, F. Lillo, and H. Waelbroeck, How Efficiency Shapes Market Impact, http://arxiv.org/abs/ 1102.5457. 

- [18] This is a very old idea. L. Harris, for example, presented a back-of-envelope theory of the square-root impact based on this idea at the conference, Microstructure, Confronting Many Viewpoints, Paris, December 2010 (unpublished); P. Besson also elaborated on this (private communication). 

- [19] E. Smith, J. D. Farmer, L. Gillemot, and S. Krishnamurthy, Statistical Theory of the Continuous Double Auction, Quantitative Finance 3, 481 (2003). 

- [20] P. Weber, B. Rosenow, Order Book Approach to Price Impact, Quantitative Finance 5, 357 (2005). 

- [21] J.-P. Bouchaud, Y. Gefen, M. Potters, and M. Wyart, Fluctuations and Response in Financial Markets: The Subtle Nature of Random Price Changes, Quantitative Finance 4, 176 (2004); J.-P. Bouchaud, J. Kockelkoren, and M. Potters, Random Walks, Liquidity Molasses and Critical Response in Financial Markets, Quantitative Finance 6, 115 (2006). 

- [22] V. Plerou, P. Gopikrishnan, L. A. Amaral, M. Meyer, and H. E. Stanley, Scaling of the Distribution of Price Fluctuations of Individual Companies, Phys. Rev. E 60, 6519 (1999); P. Gopikrishnan, V. Plerou, L. A. Amaral, M. Meyer, and H. E. Stanley, Scaling of the Distribution of Fluctuations of Financial Market Indices, Phys. Rev. E 60, 5305 (1999). 

- [23] See J.-P. Bouchaud, M. Me´zard, and M. Potters, Statistical Properties of the Stock Order Books: Empirical Results and Models, Quantitative Finance 2, 251 (2002) for an alternative presentation. 

021006-10 

PHYS. REV. X 1, 021006 (2011) 

ANOMALOUS PRICE IMPACT AND THE CRITICAL . . . 

- [24] J. Gatheral (private communication). 

- [25] See, e.g., P. Bak, How Nature Works: The Science of SelfOrganized Criticality, Copernicus (Springer, New York, 1996). 

- [26] F. Lillo and J. D. Farmer, The Long Memory of the Efficient Market, Studies in Nonlinear Dynamics and Econometrics 8, 1 (2004). 

- [27] F. Lillo, S. Mike, and J. D. Farmer, Theory for Long Memory in Supply and Demand, Phys. Rev. E 71, 066122 (2005). 

- [28] B. To´th, F. Lillo, and J. D. Farmer, Segmentation Algorithm for Non-Stationary Compound Poisson Processes, Eur. Phys. J. B 78, 235 (2010). 

- [29] Y. Lempe´rie`re, M. Vettorazzo, J. Kockelkoren, and J.-P. Bouchaud, CFM Internal Report (2006). 

- [30] B. To´th, Y. Lempe´rie`re, C. Deremble, J. de Lataillade, J. Kockelkoren, and J.-P. Bouchaud (to be published). 

- [31] See, e.g., R. Cont and A. Larrard, Price Dynamics in a Markovian Limit Order Market, http://arxiv.org/abs/ 1104.4596. 

- [32] J. D. Farmer, L. Gillemot, F. Lillo, Sz. Mike, and A. Sen, What Really Causes Large Price Changes?, Quantitative Finance 4, 383 (2004) [http://www.tandfonline.com/doi/ abs/10.1080/14697680400008627]. 

- [33] M. Potters and J.-P. Bouchaud, More Statistical Properties of Order Books and Price Impact, Physica (Amsterdam) 324A, 133 (2003). 

- [34] Sz. Mike and J. D. Farmer, An Empirical Behavioral Model of Liquidity and Volatility, J Economic Dynamics and Control 32, 200 (2008). 

- [35] F. Lillo and J. D. Farmer, The Key Role of Liquidity Fluctuations in Determining Large Price Fluctuations, Fluctuations and Noise Letters 5, L209 (2005). 

- [36] A. Joulin, A. Lefe`vre, D. Grunberg, and J.-P. Bouchaud, Stock Price Jumps: News and Volume Play a Minor Role, Wilmott Magazine, September-October (2008); J.-P. Bouchaud, The Endogenous Dynamics of Markets: Price Impact, Feedback Loops and Instabilities, in Lessons from the 2008 Crisis, edited by A. Berd (Risk Books, Incisive Media, London, 2011). 

021006-11 

