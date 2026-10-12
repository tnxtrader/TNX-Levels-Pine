**Xin Guo and Mihael Zervos Optimal execution with multiplicative price impact** 

# **Article (Published version) (Refereed)** 

### **Original citation:** 

Guo, Xin and Zervos, Mihail (2015) _Optimal execution with multiplicative price impact_ . SIAM Journal on Financial Mathematics, 6 (1). pp. 281-306. ISSN 1945-497X DOI: 10.1137/120894622 

© 2015 Society for Industrial and Applied Mathematics 

This version available at: http://eprints.lse.ac.uk/65409/ Available in LSE Research Online: February 2016 

LSE has developed LSE Research Online so that users may access research output of the School. Copyright © and Moral Rights for the papers on this site are retained by the individual authors and/or other copyright owners. Users may download and/or print one copy of any article(s) in LSE Research Online to facilitate their private study or for non-commercial research. You may not engage in further distribution of the material or use it for any profit-making activities or any commercial gain. You may freely distribute the URL (http://eprints.lse.ac.uk) of the LSE Research Online website. 

_⃝_ c 2015 Society for Industrial and Applied Mathematics 

SIAM J. FINANCIAL MATH. 

Vol. 6, pp. 281–306 

## **Optimal Execution with Multiplicative Price Impact**<sup>_∗_</sup> 

Xin Guo<sup>_†_</sup> and Mihail Zervos<sup>_‡_</sup> 



**Abstract.** We consider the so-called optimal execution problem in algorithmic trading, which is the problem faced by an investor who has a large number of stock shares to sell over a given time horizon and whose actions have an impact on the stock price. In particular, we develop and study a price model that presents the stochastic dynamics of a geometric Brownian motion and incorporates a log-linear effect of the investor’s transactions. We then formulate the optimal execution problem as a degenerate singular stochastic control problem. Using both analytic and probabilistic techniques, we establish simple conditions for the market to allow for no arbitrage or price manipulation and develop a detailed characterization of the value function and the optimal strategy. In particular, we derive an explicit solution to the problem if the time horizon is infinite. 

**Key words.** optimal execution problem, multiplicative price impact, singular stochastic control 

**AMS subject classifications.** 93E20, 91G80, 49L20 

**DOI.** 10.1137/120894622 

**1. Introduction.** We consider an investor who has a large number of stock shares to sell within a given time frame. Rapid selling of the stock may depress the stock price, while slicing the big order into many smaller blocks of orders to be executed sequentially over time may take too long to realize. Such an investor is therefore faced with the problem of how to slice the order, when to trade and at what price, etc. This problem, known as the “optimal execution problem” in algorithmic trading, is concerned with finding a trading strategy that maximizes an appropriate objective function. A key issue of the problem is concerned with modeling the price impact of stock transactions. 

The study of the optimal execution problem was initiated by Bertsimas and Lo [8], who analyzed a discrete random walk model and by Almgren and Chriss [5, 6] and Almgren [4], who considered continuous time Bachelier-type models with additive price impact. Since then, the area has attracted considerable interest; an incomplete list of notable contributions in the mathematics literature includes Huberman and Stanzl [21], He and Mamaysky [20], Gatheral, Schied, and Slynko [19], Obizhaeva and Wang [25], Almgren and Lorenz [7], Engle and Ferstenberg [13], Schied and Sch¨oneborn [29], Alfonsi, Fruth, and Schied [1, 2], Schied, Sch¨oneborn, and Tehranchi [30], Predoiu, Shaikhet, and Shreve [27], and Løkka [23]. 

Modeling stock prices by an arithmetic Brownian motion/random walk with additive impact of large stock sales is a common feature in the references on the optimal execution 

> _∗_ Received by the editors October 10, 2012; accepted for publication (in revised form) February 2, 2015; published electronically April 14, 2015. 

http://www.siam.org/journals/sifin/6/89462.html 

> _†_ Department of Industrial Engineering and Operations Research, UC at Berkeley, Berkeley, CA 94720 (xinguo@ 

ieor.berkeley.edu). This author’s research was partially supported by NSF grant DMS 1008759. 

> _‡_ Department of Mathematics, London School of Economics, London, WC2A 2AE, UK (mihalis.zervos@gmail. com). 

**281** 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**282** 

**XIN GUO AND MIHAIL ZERVOS** 

problem discussed above. An intriguing consequence of this modeling approach is that optimal strategies turn out to be more or less static or deterministic. Such strategies may lead to predictable trading patterns, which can give rise to market manipulation with techniques such as predatory trading (to this end, see the game formulations studied by Schied and Sch¨oneborn [28] and Moallemi, Park, and Van Roy [24]). Recent work by Schied, Sch¨oneborn, and Tehranchi [30], Gatheral and Schied [17], and Predoiu, Shaikhet, and Shreve [27] has revealed that such deterministic optimal strategies can be recovered by a simple argument involving an integration by parts calculation and an appropriate Euler–Lagrange equation, establishing an effective equivalence between minimizing costs and minimizing the price impact of trading strategies. 

Beyond the context of Bachelier-type models, Gatheral and Schied [17] studied a continuous time Black–Scholes-type model with additive price impact. Discrete time models with multiplicative price impact have been considered by Bertsimas and Lo [8] and Bertsimas, Lo, and Hummel [9]. Also, Forsyth et al. [14, 15] proposed a continuous time Black–Scholestype model with multiplicative price impact and derived its Hamilton–Jacobi–Bellman (HJB) equation using heuristic arguments, which they studied by means of numerical techniques. In these references, it is argued that such models are more natural than those with additive price impact because, e.g., they do not allow for strictly negative prices with nonzero probability. 

In this paper, we study the optimal execution problem in the context of a continuous time model with multiplicative price impact. To the best of our knowledge, this model is the very first one in the continuous time optimal execution literature involving singular control rather than absolutely continuous control: this setting does not restrict stock transactions to be realized at a rate over time; instead, it allows for block sales of stock. The objective of the paper is to exhaustively study the model’s analytical properties. The development of further realistic and applicable models can be motivated by the one we study here (see Remark 1 for such a generalization). 

In particular, we consider an investor who holds _Yt ≥_ 0 shares of stock at time _t_ , not including any transactions made at _t_ . The investor can buy or sell any amount of shares at any time, but short-selling is not allowed. We denote by _ξt_<sup>s(resp.,</sup><sup>_ξ_</sup> _t_<sup>b)thetotalamountof</sup> shares the investor has sold (resp., bought) up to time _t_ , so that 



where _y ≥_ 0 is the number of shares held by the investor at time 0. 

We assume that, in the absence of any transactions, stock prices follow a geometric Brownian motion. Also, we assume that (a) the price impact of small transactions is proportional to the stock price at which they are executed as well as proportional to their size, and (b) the price impact of a large transaction is the same as that of any number of smaller transactions of the same total size that are executed at the same time. In section 2, we show that such requirements give rise to the stock price dynamics 



where _λ >_ 0 is a constant and the operators _◦_ s, _◦_ b are defined by (2.5)–(2.6) below. Effectively, this is a model with multiplicative price impact: the impact of a transaction is additive to 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**283** 

the logarithm of the stock price (see (2.7) below). There are several possible generalizations of these dynamics that exhibit resilience, namely, allowing the effect of transactions on the stock price to fade over time (we briefly discuss one in Remark 1). The investor has a horizon _T ∈_ (0 _, ∞_ ], by which time she exits the market by clearing all her shares. The investor’s objective is to maximize the performance criterion 



over all admissible strategies ( _ξ_<sup>s</sup> _, ξ_<sup>b</sup> ). Here, the constant _δ ≥_ 0 reflects the investor’s impatience, while the constants _C_ s _, C_ b _≥_ 0 provide for a bid-ask spread or for proportional transaction costs. The choice _δ_ = 0 is the most natural one if the time horizon _T_ is very short. We allow for choices _δ >_ 0 because these might be appropriate for execution problems lasting several days (see Lebedeva, Maug, and Schneider [22] for real-world examples of such executions) and are essential for a nontrivial solution if _T_ = _∞_ . Also, strictly positive values of _C_ s, _C_ b can arise from the existence of a bid-ask spread. Indeed, if we interpret _Xt_ as the midprice of the stock price at time _t_ , then we can view _Xt − C_ s (resp., _Xt_ + _C_ b) as the bid (resp., ask) price of the stock at time _t_ . Such a modeling context has been considered in the literature, e.g., by Cont and de Larrard [10], who, based on empirical evidence, assume that the bid-ask spread is equal to one tick. 

The performance criterion we have adopted is the expected revenue one featured in the models studied by, e.g., Bertsimas and Lo [8] and Gatheral [16]. Other choices of performance criteria that have been considered in the literature include the mean-variance criterion in Almgren and Chriss [5, 6], the expected utility criterion in Schied and Sch¨oneborn [28], and the mean-quadratic variation criterion in Forsyth et al. [14]. Such alternative performance indices give rise to several variants of the model we study that could be the subject of future research. It is worth noting that Gatheral and Schied [18] have argued that a risk-neutral expected revenue or cost optimization objective is a reasonable choice, especially in contexts where market regularity conditions should be independent of investor preferences. 

Mathematically, the optimization problem above takes the form of a singular stochastic control problem. Its HJB equation is a degenerate parabolic (if _T < ∞_ ) or elliptic (if _T_ = _∞_ ) PDE with state-dependent gradient constraints. Although the literature of singular stochastic control is rich and long, we are unaware of any results that characterize the value function or the optimal strategies in a context similar to the one we consider here; models that are closest to the one we analyze have been studied by Shreve and Soner [31] Soner and Shreve [32], Davis and Norman [12], Zhu [33], Ocone and Weerasinghe [26], and Dai and Yi [11]. 

Our analysis involves probabilistic as well as analytic techniques. A brief summary of our main results is as follows. First, we show that if we allowed for asymmetric price impact of buying and selling, then the market would present arbitrage opportunities (see Definition 3.2 and Proposition 3.4(I)–(II)). On the other hand, we prove that there are no arbitrage opportunities in the model with symmetric price impact that we consider (see Proposition 3.6(I)). In the spirit of Huberman and Stanzl [21], we define a price manipulation to be a round-trip trade, namely, a 0 net buying and selling trading strategy, that results in a strictly positive expected revenue (see Definition 3.3). It is worth noting here that the definitions of arbitrage 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**284** 

**XIN GUO AND MIHAIL ZERVOS** 

and price manipulation that we have adopted involve no discounting, namely, _δ_ = 0, because the choice of a discounting rate characterizes specific investors rather than the market itself. We show that there is no price manipulation if and only if _μ_ = 0 (see Proposition 3.4(III) and Proposition 3.6(II)). This result is not surprising: given its definition and the symmetric nature of the market in terms of buying and selling, one can argue that a price manipulation cannot exist if and only if the stock price process is a martingale in the absence of transactions by a big investor. Indeed, in any model incorporating no price impact, the strategy that buys (resp., short-sells) one share of stock at time 0 and then sells it (resp., buys it back) at time 1 is a price manipulation if the stock price is a submartingale (resp., supermartingale) such as, e.g., a geometric Brownian motion with strictly positive (resp., negative) drift. Although the absence of a price manipulation is a desirable property of a model involving very short time scales (such as seconds or minutes), it could be viewed as rather restrictive for models involving long time scales (such as days or weeks) where the time-value of money and issues involving investor preferences come into play. From a mathematical perspective, our results are consistent with those of Huberman and Stanzl [21] and Gatheral [16], who showed that permanent price impact must be linear and symmetric to exclude price manipulation in the zero-drift models with additive price impact that they studied. 

In our analysis of possible arbitrage opportunities and price manipulation, we naturally consider trading strategies that involve short-selling subject to the constraint that short positions are bounded by a constant. On the other hand, we assume that short-selling is not permitted in our analysis of the optimal execution problem itself (see, however, the paragraph above the statement of Proposition 3.5). In this context, we first prove that the investor would be able to realize arbitrarily high expected payoffs by means of simple round-trip trades if her discounting rate were strictly less than the drift of the stock price (see Proposition 3.4(IV)). To avoid unrealistic trivialities, we therefore assume that _δ ≥_ max _{μ,_ 0 _}_ . In this case, we show that the optimal liquidation strategy involves no buying of shares (see Proposition 3.5(I)), namely, there is no transaction-triggered price manipulation in the sense of Alfonsi, Schied, and Slynko [3]. 

In the case when _T < ∞_ , we prove a verification theorem (Proposition 4.1) that relates an appropriate solution to the problem’s HJB equation to the problem’s value function. Such a solution to the HJB equation, which can be computed numerically offline, fully determines the optimal liquidation strategy. Indeed, its nature is such that the state space splits into two regions, the “waiting” one and the “selling” one. Beyond a possible sale of an appropriate amount of stock that positions the state process at the boundary of the two regions at time 0, the optimal strategy involves minimal action to keep the state process inside the closure of the waiting region and takes no action while the state process is in the interior of the waiting region. 

If _T_ = _∞_ , then we derive the solution to the problem in an explicit form (see Proposition 5.1). An interesting feature of this solution is that an optimal strategy may not exist even though the value function is finite (see Proposition 5.1(II)). If it exists, the optimal strategy can be described informally as follows (see also Figure 1). If the stock price is below a critical level _F◦_ , then it is optimal to take no action. If the stock price at time 0 is above _F◦_ , then it is optimal to either sell all available shares immediately or liquidate an amount that would cause the stock price to drop to _F◦_ and then keep on selling until all shares are exhausted by 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**285** 



**Figure 1.** _The regions providing the optimal strategy when T_ = _∞. If the stock price takes values in the “waiting” region W, then it is optimal to take no action. If the stock price at time_ 0 _is inside the “selling” region S_ 1 _, then it is optimal to sell all available shares immediately. If the stock price at time_ 0 _is inside the “selling” region S_ 2 _, then it is optimal to liquidate an amount that would cause the stock price to drop to F◦ and then keep on selling until all shares are exhausted by just preventing the stock price from rising above F◦._ 

just preventing the stock price from rising above _F◦_ . 

**2. The market model and the control problem.** We fix a filtered probability space (Ω _, F, Ft,_ P) satisfying the usual conditions and carrying a standard ( _Ft_ )-Brownian motion _W_ . 

We denote by _Yt_ the total number of shares held by the investor at time _t_ . Also, we denote by _ξt_<sup>s(resp.,</sup><sup>_ξ_</sup> _t_<sup>b)thetotalnumberofsharesthattheinvestorhassold(resp.,bought)upto</sup> time _t_ , so that _Yt_ = _y − ξt_<sup>s+</sup><sup>_ξ_</sup> _t_<sup>b,where</sup><sup>_y≥_0isthenumberofsharesheldbytheinvestor</sup> at time 0. We assume that _ξ_<sup>s</sup> and _ξ_<sup>b</sup> are ( _Ft_ )-adapted increasing c`agl`ad processes such that _ξ_ 0<sup>s=</sup><sup>_ξ_</sup> 0<sup>b=0.Also,weassumethattheinvestordoesnotsimultaneouslybuyandsell.In</sup> particular, we restrict our attention to strategies ( _ξ_<sup>s</sup> _, ξ_<sup>b</sup> ) such that 



where _ξ_<sup>ˇ</sup> is the total variation process of _ξ_ . 

The investor’s aim is to liquidate all share holdings by a time horizon _T ∈_ (0 _, ∞_ ]. We therefore consider trading strategies ( _ξ_<sup>s</sup> _, ξ_<sup>b</sup> ) such that 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**286** 

**XIN GUO AND MIHAIL ZERVOS** 

In the absence of any transactions from the investor, we model the stock price by the geometric Brownian motion _X_<sup>0</sup> given by 



for some constants _μ_ and _σ̸_ = 0. We assume that small transactions made by the investor affect the share price proportionally to its value. In particular, if the investor sells (resp., buys) a small amount _ε >_ 0 of shares at time _t_ , then the share price exhibits a jump of size 



for some constant _λ >_ 0, where we have assumed that _X_ is c`agl`ad. In this context, a small sale (resp., buy) of size _ε >_ 0 is associated with the expressions 



If we view the sale of Δ _ξt_<sup>ssharesas</sup><sup>_N_individualsalesof</sup><sup>_ε_= Δ</sup><sup>_ξ_</sup> _t_<sup>s</sup><sup>_/N_shareseach,then,for</sup><sup>_N_</sup> large enough, we obtain _Xt_ + = _e_<sup>_−λNε_</sup> _Xt_ = _e_<sup>_−λ_Δ</sup><sup>_ξ_</sup> _t_<sup>s</sup> _Xt._ 

Similarly, we can see that buying Δ _ξt_<sup>bsharesisassociatedwiththejump</sup><sup>_Xt_+=</sup><sup>_eλ_Δ</sup><sup>_ξ_</sup> _t_<sup>s</sup> _Xt_ . 

In view of the above considerations, we model the stock price dynamics by the stochastic equation 



where 



and 



where the process ( _ξ_<sup>s</sup> )<sup>c</sup> (resp., ( _ξ_<sup>b</sup> )<sup>c</sup> ) is the continuous part of the process _ξ_<sup>s</sup> (resp., _ξ_<sup>b</sup> ). Using Itˆo’s formula, we can verify that the solution to (2.4) is given by 



where _X_<sup>0</sup> is the solution to (2.3). 

If we consider the sale of Δ _ξt_<sup>ssharesattime</sup><sup>_t_asequivalenttothesaleof</sup><sup>_N_packetsof</sup> shares of small size _ε_ = Δ _ξt_<sup>s</sup><sup>_/N_,thenwecanseethatsuchasaleshouldresultinarevenueof</sup> 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**287** 

In view of this observation and a similar one concerning the buying of Δ _ξt_<sup>bsharesattime</sup><sup>_t_,</sup> we associate the performance criterion 



with each liquidation strategy ( _ξ_<sup>s</sup> _, ξ_<sup>b</sup> ), where _JT,x,y_ ( _ξ_<sup>s</sup> _, ξ_<sup>b</sup> ) is defined by 



for ( _T, x, y_ ) _∈_ R+ _×_ R<sup>_∗_</sup> +<sup>_×_R+.1Here,thediscountingrate</sup><sup>_δ≥_0reflectstheinvestor’s“impa-</sup> tience,” while the constants _C_ s _, C_ b _≥_ 0 may account for a constant bid-ask spread or provide for proportional transaction costs. 

The investor’s objective is to maximize _IT,x,y_<sup>(</sup><sup>_ξ_s</sup><sup>_, ξ_b) overall liquidationstrategies(</sup><sup>_ξ_s</sup><sup>_, ξ_b).</sup> Accordingly, we define the problem’s value function _v_ by 



where _AT,y_<sup>isthefamilyofalladmissiblestrategies,whichisintroducedbythefollowing</sup> definition. 

Definition 2.1. _Given a time horizon T ∈_ (0 _, ∞_ ] _and an initial holding of y ≥_ 0 _shares, the family AT,y_<sup>_ofalladmissibleliquidationstrategiesisthesetofallpairs_(</sup><sup>_ξ_s</sup><sup>_, ξ_b)</sup><sup>_composedby_</sup> ( _Ft_ ) _-adapted increasing c`agl`ad processes ξ_<sup>s</sup> _and ξ_<sup>b</sup> _such that ξ_ 0<sup>s=</sup><sup>_ξ_</sup> 0<sup>b= 0</sup><sup>_,_(2.1)</sup><sup>_and_(2.2)</sup><sup>_hold_</sup> _true, and_ 



_We denote by A_<sup>s</sup> _T,y_<sup>_thefamilyofallprocessesξ_s</sup><sup>_suchthat_(</sup><sup>_ξ_s</sup><sup>_,_0)</sup><sup>_∈A_</sup> _T,y_<sup>_._</sup> 

The integrability assumption that we make in (2.9) is quite general and ensures that the optimization problem is well-posed. In particular, it would plainly be satisfied if we imposed an upper bound on the process _ξ_<sup>b</sup> , which would rule out unbounded total buying and selling over a finite time horizon. On the other hand, the inequality _Yt ≥_ 0 for all _t ≥_ 0 reflects the idea that the possibility of short-selling is not permitted. 

In the next assumption we summarize the possible values that the various constants we have considered may take. 

Assumption 1. _μ, σ̸_ = 0 _, δ ≥_ max _{μ,_ 0 _}, and λ >_ 0 _are constants._ 

_Remark_ 1. In the model that we have developed, transactions made by the investor have a permanent impact. There are several extensions of the model that can accommodate transient impact. For instance, we can replace the dynamics given by (2.7) by _Xt_ = _Xt_<sup>0</sup><sup>_eZt_,where</sup> 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**288** 

**XIN GUO AND MIHAIL ZERVOS** 

for some kernel _G_ . In this context, if we choose _G_ ( _t − s_ ) = _e_<sup>_−γt_</sup> _e_<sup>_γs_</sup> for some constant _γ >_ 0, then 

_dZt_ = _−γZt dt − λ dξt_<sup>s+</sup><sup>_λ dξ_</sup> _t_<sup>b</sup><sup>_._</sup> 

In such extensions, the resulting optimization problem’s state space would involve four variables (namely, _t_ , _x_ , _y_ , and _z_ ) instead of three (namely, _t_ , _x_ , and _y_ ). We leave this as well as other extensions accommodating resilience of the stock price for future research. 

**3. Study of the market and preliminary results.** In this section, we establish a range of results that characterize the market we study as well as some estimates we will need. To this end, we first consider the so-called round-trip trades, which are trading strategies that involve 0 net buying or selling of shares over a given finite time horizon. It is worth noting that the inequality in (3.1) is an admissibility condition that requires the maximum number of stock 

shares that a round trip can be short to be bounded by a constant. 

Definition 3.1. _An admissible round-trip trade with time horizon T ∈_ R<sup>_∗_</sup> +<sup>_is any pair_(</sup><sup>_ζ_s</sup><sup>_, ζ_b)</sup> _of_ ( _Ft_ ) _-adapted increasing c`agl`ad processes such that ζ_ 0<sup>s=</sup><sup>_ζ_</sup> 0<sup>b= 0</sup><sup>_and_</sup> 



_for some constant_ Γ _>_ 0 _, which may depend on the trading strategy itself._ 

Our first result shows that the model we consider would be unviable if we allowed for asymmetric impact of buying and selling. In particular, we prove that if we model the stock price dynamics by 



where _◦_<sup>_λ_</sup> s<sup>(resp.,</sup><sup>_◦κ_</sup> b<sup>) is defined by (2.5) (resp., (2.6) with</sup><sup>_κ_in place of</sup><sup>_λ_),for some</sup><sup>_κ̸_=</sup><sup>_λ_, then</sup> 

the market may present arbitrage opportunities in the following sense. 

Definition 3.2. _The market allows for arbitrage opportunities if there exists a round-trip trade with resulting revenue that is positive and strictly positive with strictly positive probability, namely, if there exists a round-trip trade_ ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) _such that_ 



_and_ P � _R_ ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) _>_ 0� _>_ 0 _._ 

We also prove that price manipulation exists if and only if _μ̸_ = 0. 

Definition 3.3. _A price manipulation is a round-trip trade_ ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) _resulting in a strictly positive expected revenue, namely,_ E � _R_ ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> )� _>_ 0 _, where R is defined by_ (3.3) _. An unbounded price manipulation is a sequence of round-trip trades_ ( _ζ_<sup>s</sup><sup>_,n_</sup> _, ζ_<sup>b</sup><sup>_,n_</sup> ) _such that_ lim _n→∞_ E� _R_ ( _ζ_<sup>s</sup><sup>_,n_</sup> _, ζ_<sup>b</sup><sup>_,n_</sup> )� = _∞._ 

In these definitions, we have taken _δ_ = 0 because the choice of a discounting rate is investor specific. In this way, the existence or not of arbitrage and/or price manipulation characterizes a market of risk-neutral investors as a whole. 

The next result, which is complemented by Proposition 3.6 below, is concerned with these issues. We also show here that the investor’s optimization problem would be trivial if the 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**289** 

investor’s discounting rate _δ_ were strictly less than the stock price drift _μ_ , which we have excluded as a possibility in Assumption 1. 

Proposition 3.4. _The following statements are true:_ 

(I) _If the price process dynamics are given by_ (3.2) _for some κ > λ >_ 0 _, then the market presents arbitrage opportunities and arbitrarily high risk-free profits can be realized by simple round-trip strategies._ 

(II) _If the price process dynamics are given by_ (3.2) _for some λ > κ >_ 0 _, then the market may present arbitrage opportunities._ 

(III) _Suppose that the price process dynamics are given by_ (3.2) _for some κ_ = _λ >_ 0 _, namely, by_ (2.4) _. If μ <_ 0 _, then price manipulation may exist, while if μ >_ 0 _, then unbounded price manipulation exists._ 

(IV) _Consider the optimal execution problem formulated in section_ 2 _, in which the price process dynamics are given by_ (3.2) _for some κ_ = _λ >_ 0 _, namely, by_ (2.4) _. If_ 0 _≤ δ < μ in violation of Assumption_ 1 _, then round-trip trades involving no short-selling can realize arbitrarily high expected payoffs and_ 



_Proof_ . Suppose that the price process dynamics are given by (3.2) for some _κ > λ_ , and let ( _ϱn_ ) be any sequence of strictly positive numbers such that lim _n→∞ ϱn_ = _∞_ . Given _ε ∈_ (0 _,_ ( _κ − λ_ ) _∧ λ_ ), we define the ( _Ft_ )-stopping time 



and we note that 



The round-trip trade ( _ζ_<sup>s</sup><sup>_,n_</sup> _, ζ_<sup>b</sup><sup>_,n_</sup> ) that buys _ϱn_ shares at time 0 and then sells them at time _τ ∈_ (0 _,_ 1] results in the revenue 



The last expression tends to _∞_ as _n →∞_ , and (I) follows. 

To show (II), we assume that the price process dynamics are given by (3.2) for some _κ < λ_ , we define the ( _Ft_ )-stopping time 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**290** 

**XIN GUO AND MIHAIL ZERVOS** 

for some _ε ∈_ (0 _,_ ( _λ − κ_ ) _∧ κ_ ), and we note that 



The round-trip trade ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) that short-sells _ϱ >_ 0 shares at time 0 and then buys them back at time _τ ∈_ (0 _,_ 1] results in the revenue 



The coefficient of _x_ in the last expression is strictly positive for all _ϱ >_ 0 sufficiently large. Given any such _ϱ_ , the revenue _R_ ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) is strictly positive for all _x_ sufficiently large, and the claim that the market may present arbitrage opportunities follows. 

To see (IV), suppose that _δ < μ_ and consider the round-trip trade that buys _ϱ >_ 0 shares 

at time 0 and sells them at time _T >_ 0. This strategy has expected payoff 



which tends to _∞_ as _ϱ →∞_ . In particular, (3.4) holds true. 

To prove (III), suppose first that _μ <_ 0. The round-trip trade ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) that short-sells _ϱ >_ 0 shares at time 0 and buys them back at time _T >_ 0 results in the expected revenue 



which is strictly positive for all _ϱ >_ 0 provided _x_ is sufficiently large. On the other hand, if _μ >_ 0, then the round-trip trade ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) we considered in the proof of (IV) above results in the expected revenue 



which tends to _∞_ as _ϱ →∞_ for all _x_ and _T_ . 



We now switch our attention to the actual optimal execution problem. It is worth recalling that, contrary to our analysis thus far, short-selling is not permitted in this problem. However, a simple inspection of the proof of part (IV) reveals that its conclusions remain true if shortselling is indeed allowed. Furthermore, it is worth noting that if the conditions of parts (III) and (IV) are both satisfied, then every admissible liquidation strategy is optimal. 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**291** 

Proposition 3.5. _Consider the optimal execution problem formulated in section_ 2 _. Given a time horizon T ∈_ (0 _, ∞_ ] _and any_ ( _x, y_ ) _∈_ R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_,thefollowingstatementsaretrue:_</sup> 

(I) _The optimal liquidation strategy involves no buying of shares, namely,_ 



_In particular, the market does not allow for transaction-triggered price manipulation._ (II) _The value function satisfies_ 



_for all T ∈_ (0 _, ∞_ ] _and_ ( _x, y_ ) _∈_ R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_._</sup> (III) _If C_ s = 0 _, then it is optimal to sell all shares at time_ 0 _, and the value function is given by v_ ( _T, x, y_ ) = _λ_<sup>1</sup><sup>_x_</sup> �1 _− e_<sup>_−λy_�</sup> _for all T ∈_ (0 _, ∞_ ] _and_ ( _x, y_ ) _∈_ R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_._</sup> (IV) _Suppose that δ_ = _μ ≥_ 0 _. If T ∈_ R<sup>_∗_</sup> +<sup>_,thenitisoptimaltosellallavailablesharesat_</sup> _T . On the other hand, if T_ = _∞, then selling all available shares at time n_ = 1 _,_ 2 _, . . . provides_ 

_a sequence of ε-optimal strategies. In this case, the value function is given by_ 



_for all_ ( _x, y_ ) _∈_ R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_._</sup> _Proof_ . Given a liquidation strategy ( _ξ_<sup>s</sup> _, ξ_<sup>b</sup> ) _∈AT,y_<sup>,wedefine</sup> 



and we note that 



In view of (2.9), we can see that 



Also, (2.2) and this observation imply that if _T < ∞_ , then 



while if _T_ = _∞_ , then 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**292** 

**XIN GUO AND MIHAIL ZERVOS** 

It follows that ( _ξ_<sup>˜s</sup> _,_ 0) _∈AT,y_<sup>, namely,</sup><sup>_ξ_˜s</sup><sup>_∈A_s</sup> _T,y_<sup>.Forfuture reference,we alsonote that (3.11)</sup> implies that 



In view of the observations that 



which follow from Itˆo’s formula and (2.5)–(2.6), and the calculation 



which follows from an application of the integration by parts formula, we can see that, given any _T ∈_ [0 _, T_ ] _∩_ R+, 



In view of these identities, the assumption that _δ ≥ μ_ , the definition (3.10) of _ξ_<sup>˜s</sup> , and (3.12), we can see that 



where _X_<sup>˜</sup> is the solution to (2.4) with _ξ_<sup>˜s</sup> and 0 in place of _ξ_<sup>s</sup> and _ξ_<sup>b</sup> , respectively. 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**293** 

Using Itˆo’s isometry, H¨older’s inequality, and (2.9) in Definition 2.1, we calculate 





Therefore, the stochastic integral in (3.14) defines a square-integrable martingale and has 0 expectation. Taking expectations in (3.14), we therefore obtain 



and (3.7) follows (see also (2.8)). Furthermore, the expression for _JT,x,y_ ( _ξ_<sup>˜s</sup> _,_ 0) provided here implies the upper bound in (3.8) as well as establishes (III) because if _C_ s = 0, then it is plainly maximized by the choice _ξ_<sup>˜</sup> _t_<sup>s=</sup><sup>_y_forall</sup><sup>_t >_0thatcorrespondstosellingallsharesattime0.</sup> On the other hand, the lower bound in (3.8) is just the payoff of the strategy that sells all shares at time 0. 

Finally, suppose that _δ_ = _μ ≥_ 0. Taking expectations in (3.13), we obtain 



If _T < ∞_ , then this expression and the fact that _ξT_<sup>s</sup> +<sup>_−ξ_</sup> _T_<sup>b</sup> +<sup>=</sup><sup>_y_implythatitisoptimalto</sup> sell all available shares at time _T_ and 



On the other hand, if _T_ = _∞_ , then we can see that selling all available shares at time _n_ = 1 _,_ 2 _, . . ._ provides a sequence of _ε_ -optimal strategies once we combine the observation that these strategies have expected payoffs such that 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**294** 

**XIN GUO AND MIHAIL ZERVOS** 

with the upper bound in (3.8). 



_Remark_ 2. For future reference, we note the following estimate that we can derive using the integration by parts formula and Itˆo’s isometry in the same way as in (3.13) and (3.15): given a time horizon _T ∈_ (0 _, ∞_ ], a strategy _ξ_<sup>s</sup> _∈A_<sup>s</sup> _T,y_<sup>,andanytime</sup><sup>_T∈_[0</sup><sup>_,_</sup> _T_ ] _∩_ R+, 



Using Proposition 3.5, we now establish the following result that complements Proposition 3.4. 

Proposition 3.6. _Consider the market model developed in section_ 2 _, and recall Definitions_ 

- 3.2 _and_ 3.3 _. The following statements are true:_ 

   - (I) _The market does not allow for arbitrage opportunities._ 

   - (II) _If μ_ = 0 _, then there exists no price manipulation._ 

_Proof_ . To establish (II), we assume that _μ_ = 0 and we show that every round-trip trade has negative expected execution payoff. To this end, we consider any round-trip trade ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) with time horizon _T ∈_ R<sup>_∗_</sup> +<sup>andwedefinetheliquidationstrategy(</sup><sup>_ξ_s</sup><sup>_, ξ_b)</sup><sup>_∈A_</sup> _T,_ Γ<sup>by</sup> 



where _T_ = _T_ + _ε_ for some _ε >_ 0, and Γ _>_ 0 is any bound as in (3.1). This strategy puts us in the context of an investor who starts with Γ shares, follows the round-trip trade up to time _T_ , and then sells all available shares Γ at a later time _T_ . The expected revenue resulting from the execution of the round-trip trade is 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**295** 

and (II) follows. 

To show (I), we argue by contradiction and we assume that there exists a round-trip trade ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) with time horizon _T ∈_ R<sup>_∗_</sup> +<sup>satisfyingtherequirementsofDefinition3.2.Wethen</sup> define the probability measure Q on (Ω _, F_ ) by 



and we note that 

_dXt_ = _−λXt ◦_ s _dξt_<sup>s+</sup><sup>_λXt◦_b</sup><sup>_dξ_</sup> _t_<sup>b+</sup><sup>_σXtdW_Q</sup> _t_<sup>_,_</sup> where ( _Wt_<sup>Q</sup><sup>_,t ∈_[0</sup><sup>_, T_])is the Q-Brownianmotiondefined by</sup><sup>_W_Q</sup> _t_<sup>=</sup><sup>_μ_</sup> _σ_<sup>_t_+</sup><sup>_Wt_.The equivalence</sup> of P and Q implies that 



Therefore, EQ � _R_ ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> )� _>_ 0. It follows that ( _ζ_<sup>s</sup> _, ζ_<sup>b</sup> ) is a price manipulation in a setting with _μ_ = 0, which contradicts (II), and the proof is complete. 

**4. The finite time horizon case (** **_T < ∞_ ).** In view of Proposition 3.5(I), we expect that the value function _v_ of the stochastic control problem formulated in section 2 identifies with an appropriate solution _w_ : R+ _×_ R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_→_RtotheHJBequation</sup> 



with boundary condition 



where 



To obtain qualitative understanding of this equation, we consider the following heuristic arguments. Suppose that, at a given time, the investor’s horizon is _t >_ 0, the share price is _x >_ 0, and the investor holds an amount _y >_ 0 of shares. At that time, the investor is faced with two possible actions. The first one is to wait for a short time Δ _t_ and then continue optimally. Bellman’s principle of optimality implies that this possibility, which is not necessarily optimal, is associated with the inequality 



Applying Itˆo’s formula and dividing by Δ _t_ before letting Δ _t ↓_ 0, we obtain 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**296** 

**XIN GUO AND MIHAIL ZERVOS** 

The second possibility is to sell a small amount _ε >_ 0 of shares and then continue optimally. This action is associated with the inequality 



Rearranging terms and letting _ε ↓_ 0, we obtain 

(4.5) _−λxvx_ ( _t, x, y_ ) _− vy_ ( _t, x, y_ ) + _x − C_ s _≤_ 0 _._ 

The Markovian character of the problem implies that one of these two possibilities should be optimal and one of (4.4)–(4.5) should hold with equality at any point in the state space. It follows that the problem’s value function _v_ should identify with an appropriate solution _w_ of the HJB equation (4.1). Also, the boundary condition in (4.2) follows from the requirement that the investor must liquidate all share holdings at the end of the planning horizon. 

We now prove a verification theorem that associates a smooth solution to the HJB equation (4.1)–(4.2) with the control problem’s value function and can be used to identify an optimal liquidation strategy. To this end, we consider the sets 



and we call them the “waiting” region and the “selling” region, respectively, consistent with the heuristics that we have discussed above. Also, we note that the inequalities in (4.6) are consistent with the bounds (3.8) that the value function satisfies. 

Proposition 4.1. _Consider the optimal execution problem formulated in section_ 2 _. Given a time horizon T ∈_ (0 _, ∞_ ) _, suppose that a function w_ : [0 _, T_ ] _×_ R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_→_R</sup><sup>_isaC_1</sup><sup>_,_2</sup><sup>_,_1</sup><sup>_solution_</sup> _to the HJB equation_ (4.1) _–_ (4.2) _such that_ 







_where X_<sup>_⋆_</sup> _and Y_<sup>_⋆_</sup> _are the share price and shares held processes associated with the liquidation strategy_ ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0) _, then w identifies with the value function v of the stochastic control problem formulated in section_ 2 _. In particular,_ 



_and_ ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0) _is an optimal liquidation strategy._ 

_Proof_ . We have established the first identity in (4.9) in Proposition 3.5(I). To prove the second one, we fix any initial condition ( _x, y_ ) _∈_ R<sup>_∗_</sup> +<sup>_×_R+andanyprocess</sup><sup>_ξ_s</sup><sup>_∈A_s</sup> _T,y_<sup>.Inview</sup> 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**297** 

of the Itˆo–Tanaka–Meyer formula and the left-continuity of the processes _X_ , _Y_ , we can see that 



where 



Combining this calculation with the observation that 



and (2.5), we obtain 



Since _w_ satisfies (4.1) and admits the lower bound in (4.6), this calculation implies that 



In particular, 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**298** 

**XIN GUO AND MIHAIL ZERVOS** 

because _YT_ +<sup>=0.</sup> In view of (3.16) in Remark 2, (4.11), and the fact that _Yt ∈_ [0 _, y_ ] for all _t ∈_ [0 _, T_ ], we can see that inf 0 _≤t≤T_<sup>_Mt_isanintegrablerandomvariable.Therefore,</sup> the stochastic integral _M_ is a supermartingale. In light of this observation, we can take expectations in (4.12) to obtain 



This inequality and the first identity in (4.9) imply that 



because _ξ_<sup>s</sup> _∈A_<sup>s</sup> _T,y_<sup>hasbeenarbitrary.</sup> 

If a strategy _ξ_<sup>s</sup><sup>_⋆_</sup> _∈A_<sup>s</sup> _T,y_<sup>issuchthat(4.7)–(4.8)holdtrue,thenwecancheckthat(4.1)–</sup> (4.2), the upper bound in (4.6), (4.10), and the fact that _YT_<sup>_⋆_</sup> +<sup>= 0implythat</sup> 



and 



instead of (4.11) and (4.12), respectively. The inequality here and (3.16) in Remark 2 imply that sup0 _≤t≤T_<sup>_Mt_isanintegrablerandomvariable.Therefore,</sup><sup>_M_isasubmartingaleandwe</sup> can take expectations in the identity to obtain 



which, combined with the first identity in (4.9) and (4.13), implies the second identity in (4.9) as well as the optimality of ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0). 

**5. The infinite time horizon case (** **_T_ =** **_∞_ ).** Throughout this section, we write _v_ ( _x, y_ ) 

instead of _v_ ( _∞, x, y_ ) and we assume that 



(we have solved the cases arising in the context of Assumption 1 when the problem data does not satisfy these inequalities in Proposition 3.5(III)–(IV)). 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**299** 

In light of the heuristics we considered in the previous section that explain the structure of the HJB equation (4.1)–(4.2), we solve the stochastic control problem that arises when _T_ = _∞_ and (5.1) holds true by constructing an appropriate solution _w_ : R<sup>_∗_</sup> +<sup>_×_R+</sup><sup>_→_Rtothe</sup> HJB equation 



where _L_ is defined by (4.3), with boundary condition 



To this end, we look for a solution _w_ to (5.2)–(5.3) that is characterized by a function _F_ : R+ _→_ R+ that partitions the state space R<sup>_∗_</sup> +<sup>_×_R+intotworegions,the“waiting”region</sup><sup>_W_</sup> and the “selling” region _S_ , defined by 



Inside _W_ , _w_ should satisfy the differential equation 



The only solution to this ODE that remains bounded as _x ↓_ 0 is given by 



for some function _A_ : R+ _→_ R, where _n_ is the positive solution to the quadratic equation 



For future reference, we note that _n >_ 1 if and only if _δ > μ_ . On the other hand, _w_ should satisfy 



which implies that 



To proceed further, we look for _A_ and _F_ such that _w_ is _C_<sup>2</sup><sup>_,_1</sup> . Such a requirement, (5.6), and (5.8)–(5.9) yield the system of equations 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**300** 

**XIN GUO AND MIHAIL ZERVOS** 

which is equivalent to 



In view of the boundary condition (5.3) and (5.6), we require that _A_ (0) = 0 and we solve (5.11) to obtain 



The analysis thus far has fully characterized _w_ inside the waiting region _W_ . To determine _w_ inside the selling region _S_ , we consider the function Y defined by 

(5.13) 



and we note that 



In particular, we note that the restriction of Y in ( _F◦, ∞_ ) partitions the selling region into 



(see also Figure 1). The region _S_ 1 is the part of the state space where it is optimal to sell all available shares at time 0. On the other hand the region _S_ 2 is the part of the state space where it is optimal to sell an amount Y( _x_ ) of shares at time 0 and then sell continuously in a manner such that the optimal joint process ( _X_<sup>_⋆_</sup> _, Y_<sup>_⋆_</sup> ) is reflected in the line _x_ = _F◦_ in an appropriate oblong way until all shares are exhausted. These considerations and the structure of the performance criterion that we maximize suggest that 



We conclude this discussion with the candidate for a solution to the HJB equations (5.2)– (5.3) given by 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**301** 

We can now prove the main result of the section, which shows that this function is indeed the 

control problem’s value function and identifies an optimal liquidation strategy. 

Proposition 5.1. _Consider the optimal execution problem formulated in section_ 2 _, and suppose that T_ = _∞ and that_ (5.1) _holds true. The function w defined by_ (5.15) _, where F◦, A are given by_ (5.10) _,_ (5.12) _, is a C_<sup>2</sup><sup>_,_1</sup> _solution to the HJB equation_ (5.2) _that identifies with_ 

_the value function v of the stochastic control problem. In particular,_ 



_Furthermore, if we define_ 



_where_ 



_then the following statements are true:_ 

(I) _If μ −_<sup>1</sup> 2<sup>_σ_2</sup><sup>_≥_0</sup><sup>_,then_(</sup><sup>_ξ_s</sup><sup>_⋆,_0)</sup><sup>_isanoptimalliquidationstrategy._</sup> (II) _If μ −_<sup>1</sup> 2<sup>_σ_2</sup><sup>_<_0</sup><sup>_,then_(</sup><sup>_ξ_s</sup><sup>_⋆,_0)</sup><sup>_isnotanadmissibleliquidationstrategy.Inthiscase,if_</sup> _we define_ 



_then_ ( _ξ_<sup>s</sup><sup>_⋆j_</sup> _,_ 0) _gives rise to a sequence of ε-optimal strategies._ 

_Proof_ . In view of its construction, we will prove that _w_ is _C_<sup>2</sup><sup>_,_1</sup> if we show that _wy_ , _wx_ , and _wxx_ are continuous along the free-boundary _F_ as well as along the restriction of Y in ( _F◦, ∞_ ). To this end, we consider any ( _x, y_ ) _∈S_ 2 and we use the ODE (5.11) that _A_ satisfies as well as the definition (5.13) of Y to calculate 



and 



Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**302** 

**XIN GUO AND MIHAIL ZERVOS** 

where the last identity follows thanks to (5.10). These calculations imply the required continuity results along _F_ because lim _x↓F◦_ Y( _x_ ) = 0. Also, these calculations, the observation that 



which follows from (5.11), and the fact that _A_ (0) = 0 imply that given any point _x > F◦_ and any sequence ( _xn, yn_ ) _∈S_ 2 converging to ( _x,_ Y( _x_ )), 



These expressions are the same as the corresponding ones that we derive using the definition (5.15) of _w_ for a point _x > F◦_ and any sequence ( _xn, yn_ ) _∈S_ 1 converging to ( _x,_ Y( _x_ )), and the required continuity results along the restriction of Y in ( _F◦, ∞_ ) follow. 

By the construction and the _C_<sup>2</sup><sup>_,_1</sup> continuity of _w_ , we will show that _w_ satisfies the HJB equation (5.2) if we prove that 



In view of (5.11), we can see that (5.23) is equivalent to 



which is true thanks to the calculation 









Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**303** 

In view of (5.25) and the definition (5.13) of Y, we can see that 

_Q_ 1(0) = 0 and _Q_<sup>_′_</sup> 1<sup>(</sup><sup>_y_) =</sup><sup>_δC_s</sup><sup>_−_(</sup><sup>_δ −μ_)</sup><sup>_F◦eλy<_0</sup> for all _y ≥_ 0 

and 



It follows that the right-hand side of (5.26) (resp., (5.27)) is negative for all _y ≥_ 0 (resp., _x ≥ F◦_ ), and (5.24) has been established. 

We have established the first identity in (5.16) in Proposition 3.5(I). To derive the second one, we consider any strategy _ξ_<sup>s</sup> _∈A_<sup>s</sup> _∞,y_<sup>.Arguing inthe samewayas intheproofofProposi-</sup> tion 4.1 up to (4.11)–(4.12) and using the positivity of _w_ instead of the lower bound in (4.6), we can show that 





where 



This result and (3.16) in Remark 2 imply that the random variable inf _t∈_ [0 _,T_ ] _Mt_ is integrable for all _T >_ 0. Therefore, the stochastic integral _M_ is a supermartingale. In light of this observation, we can take expectations in (5.28) to obtain 



It follows that _v_ ( _x, y_ ) _≤ w_ ( _x, y_ ) because _ξ_<sup>s</sup> _∈A_<sup>s</sup> _∞,y_<sup>hasbeenarbitrary.</sup> 

To prove the reverse inequality and establish the optimality claims associated with ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0), where _ξ_<sup>s</sup><sup>_⋆_</sup> is given by (5.17), we first note that apart from a jump of size min _{y, λ_<sup>1(ln</sup> _F_<sup>_x_</sup> _◦_<sup>)+</sup><sup>_}_=</sup> min � _y,_ (Y( _x_ ))<sup>+�</sup> at time 0, the process (ln _x_ + _B − λξ_<sup>s</sup><sup>_⋆_</sup> _, Y_<sup>_⋆_</sup> _− ξ_<sup>s</sup><sup>_⋆_</sup> ) is reflecting in the line _x_ = ln _F◦_ in the direction defined by the vector ( _−λ, −_ 1). In particular, 



where _τ_<sup>_∗_</sup> = inf _{t ≥_ 0 _| ξt_<sup>s</sup><sup>_⋆_</sup> = _y}_ . In view of this observation and (2.7), if we denote by _X_<sup>_⋆_</sup> and _Y_<sup>_⋆_</sup> the price process and the remaining amount of shares process associated with ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0), then 



where the waiting region _W_ and the selling region _S_ are given by (5.4) and (5.5). Also, we can check that the strategy ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0) is admissible provided lim _T →∞ YT_<sup>_⋆_= 0.In view of (5.17)–(5.18),</sup> 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**304** 

**XIN GUO AND MIHAIL ZERVOS** 

we can see that this is indeed the case if and only if _μ −_<sup>1</sup> 2<sup>_σ_2</sup><sup>_≥_0becauseaBrownianmotion</sup> with strictly negative drift has a supremum over time that is an exponentially distributed random variable. 

In the same way as in the proof of Proposition 4.1, we now see that 



where the local martingale _M_<sup>_⋆_</sup> is defined as in (5.29). In view of this identity, (3.16), and the inequality 



which follows from (5.12) and the definition (5.15) of _w_ , we can see that the random variable sup _t∈_ [0 _,T_ ] _Mt_ is integrable for all _T >_ 0. Therefore, _M_<sup>_⋆_</sup> is a submartingale, and we can take expectations in (5.30) to obtain 





These identities and (5.31) imply that 



Combining this result with the inequality _v_ ( _x, y_ ) _≤ w_ ( _x, y_ ) that we have established above, we derive (5.16) as well as the optimality of ( _ξ_<sup>s</sup><sup>_⋆_</sup> _,_ 0), which is admissible if and only if _μ −_<sup>1</sup> 2<sup>_σ_2</sup><sup>_≥_0.</sup> If _μ −_<sup>1</sup> 2<sup>_σ_2</sup><sup>_<_0,thenwecanuse(5.32)tocheckthatthestrategy(</sup><sup>_ξ_s</sup><sup>_⋆j,_0)givenby(5.19)</sup> has payoff 



The inequality _v_ ( _x, y_ ) _≤ w_ ( _x, y_ ) and the fact that the right-hand side of this expression converges to _w_ ( _x, y_ ) as _j →∞_ imply (5.16) and establish that ( _ξ_<sup>s</sup><sup>_⋆j_</sup> _,_ 0) is a sequence of _ε_ -optimal strategies. 

**Acknowledgments.** We thank the three anonymous referees for their constructive comments. We are also grateful to the associate editor, who handled the paper for numerous detailed comments and suggestions that enhanced the motivation and the presentation of the model we study. 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**OPTIMAL EXECUTION WITH MULTIPLICATIVE PRICE IMPACT** 

**305** 

#### **REFERENCES** 

- [1] A. Alfonsi, A. Fruth, and A. Schied, _Constrained portfolio liquidation in a limit order book model_ , in Advances in Mathematics of Finance, Banach Center Publ. 83, Polish Academy of Sciences, Warsaw, 2008, pp. 9–25. 

- [2] A. Alfonsi, A. Fruth, and A. Schied, _Optimal execution strategies in limit order books with general shape functions_ , Quant. Finance, 10 (2010), pp. 143–157. 

- [3] A. Alfonsi, A. Schied, and A. Slynko, _Order book resilience, price manipulation, and the positive portfolio problem_ , SIAM J. Financial Math., 3 (2012), pp. 511–533. 

- [4] R. Almgren, _Optimal execution with nonlinear impact function and trading enhanced risk_ , Appl. Math. Finance, 10 (2003), pp. 1–18. 

- [5] R. Almgren and N. Chriss, _Value under liquidation_ , Risk, 12 (1999), pp. 61–63. 

- [6] R. Almgren and N. Chriss, _Optimal execution of portfolio transactions_ , J. Risk, 3 (2000), pp. 5–39. 

- [7] R. Almgren and J. Lorenz, _Adaptive arrival price_ , Institutional Investor Journals, 1 (2007), pp. 59–66. 

- [8] D. Bertsimas and A. W. Lo, _Optimal control of execution costs_ , J. Financ. Market, 1 (1998), pp. 1–50. 

- [9] D. Bertsimas, A. W. Lo, and P. Hummel, _Optimal control of execution costs for portfolios_ , Comput. Sci. Eng., 1 (1999), pp. 40–53. 

- [10] R. Cont and A. de Larrard, _Order Book Dynamics in Liquid Markets: Limit Theorem and Diffusion Approximations_ , http://ssrn.com/abstract=1757861 (February 1, 2012). 

- [11] M. Dai and F. Yi, _Finite horizon optimal investment with transaction costs: A parabolic double obstacle problem_ , J. Differential Equations, 246 (2009), pp. 1445–1469. 

- [12] M. H. A. Davis and A. R. Norman, _Portfolio selection with transaction costs_ , Math. Oper. Res., 15 (1990), pp. 676–713. 

- [13] R. Engle and R. Ferstenberg, _Execution risk_ , J. Portfolio Manage., 33 (2007), pp. 34–44. 

- [14] P. A. Forsyth, J. S. Kennedy, S. T. Tse, and H. Windcliff, _Optimal trade execution: A meanquadratic-variation approach_ , J. Econom. Dynam. Control, 36 (2011), pp. 1971–1991. 

- [15] P. A. Forsyth, J. S. Kennedy, S. T. Tse, and H. Windcliff, _Comparison between the mean-variance optimal and the mean-quadratic-variation optimal trading strategies_ , Appl. Math. Finance, 20 (2013), pp. 415–449. 

- [16] J. Gatheral, _No-dynamic-arbitrage and market impact_ , Quant. Finance, 10 (2010), pp. 749–759. 

- [17] J. Gatheral and A. Schied, _Optimal trade execution under geometric Brownian motion in the Almgren and Chriss framework_ , Int. J. Theor. Appl. Finance, 14 (2011), pp. 353–368. 

- [18] J. Gatheral and A. Schied, _Dynamical models of market impact and algorithms for order execution_ , in Handbook on Systemic Risk, Cambridge University Press, Cambridge, UK, 2013, pp. 579–599. 

- [19] J. Gatheral, A. Schied, and A. Slynko, _Transient linear price impact and Fredholm integral equations_ , Math. Finance, 22 (2012), pp. 445–474. 

- [20] H. He and H. Mamaysky, _Dynamic trading with price impact_ , J. Econom. Dynam. Control, 29 (2005), pp. 891–930. 

- [21] G. Huberman and W. Stanzl, _Price manipulation and quasi-arbitrage_ , Econometrica, 72 (2004), pp. 1247–1275. 

- [22] O. Lebedeva, E. Maug, and C. Schneider, _Trading Strategies of Corporate Insiders_ , http://ssrn.com/ abstract=2118110 (January 16, 2015). 

- [23] A. Løkka, _Optimal liquidation in a limit order book for a risk-averse investor_ , Math. Finance, 24 (2014), pp. 696–727. 

- [24] C. Moallemi, B. Park, and B. Van Roy, _The Execution Game_ , Columbia Business School Research Paper Forthcoming; available from http://ssrn.com/abstract=1916298 (April 28, 2008). 

- [25] A. Obizhaeva and J. Wang, _Optimal trading strategy and supply/demand dynamics_ , J. Financ. Market, 16 (2013), pp. 1–32. 

- [26] D. Ocone and A. Weerasinghe, _Degenerate variance control of a one-dimensional diffusion_ , SIAM J. Control Optim., 39 (2000), pp. 1–24. 

- [27] S. Predoiu, G. Shaikhet, and S. Shreve, _Optimal execution in a general one-sided limit-order book_ , SIAM J. Financial Math., 2 (2011), pp. 183–212. 

- [28] A. Schied and T. Sch¨oneborn, _Liquidation in the Face of Adversity: Stealth vs. Sunshine Trading_ , EFA 2008 Athens Meetings Paper; available from http://ssrn.com/abstract=1007014 (March 18, 2009). 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

**306** 

**XIN GUO AND MIHAIL ZERVOS** 

- [29] A. Schied and T. Sch¨oneborn, _Risk aversion and the dynamics of optimal liquidation strategies in illiquid markets_ , Finance Stoch., 13 (2009), pp. 181–204. 

- [30] A. Schied, T. Sch¨oneborn, and M. Tehranchi, _Optimal basket liquidation for CARA investors is deterministic_ , Appl. Math. Finance, 17 (2010), pp. 471–489. 

- [31] S. Shreve and H. M. Soner, _Optimal investment and consumption with transaction costs_ , Ann. Appl. Probab., 4 (1994), pp. 609–692. 

- [32] H. M. Soner and S. E. Shreve, _Regularity of the value function for a two-dimensional singular stochastic control problem_ , SIAM J. Control Optim., 27 (1989), pp. 876–907. 

- [33] H. Zhu, _Generalized solution in singular stochastic control: The nondegenerate problem_ , Appl. Math. Optim., 25 (1992), pp. 225–245. 

Copyright © by SIAM. Unauthorized reproduction of this article is prohibited. 

