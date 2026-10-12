



Horst, Ulrich; Kivman, Evgueni 

**Article  —  Published Version** Optimal trade execution under small market impact and portfolio liquidation with semimartingale strategies 

Finance and Stochastics 

**Provided in Cooperation with:** Springer Nature 

_Suggested Citation:_ Horst, Ulrich; Kivman, Evgueni (2024) : Optimal trade execution under small market impact and portfolio liquidation with semimartingale strategies, Finance and Stochastics, ISSN 1432-1122, Springer, Berlin, Heidelberg, Vol. 28, Iss. 3, pp. 759-812, https://doi.org/10.1007/s00780-024-00536-2 

This Version is available at: https://hdl.handle.net/10419/315063 

**Standard-Nutzungsbedingungen:** 

Die Dokumente auf EconStor dürfen zu eigenen wissenschaftlichen Zwecken und zum Privatgebrauch gespeichert und kopiert werden. 

Sie dürfen die Dokumente nicht für öffentliche oder kommerzielle Zwecke vervielfältigen, öffentlich ausstellen, öffentlich zugänglich machen, vertreiben oder anderweitig nutzen. 

Sofern die Verfasser die Dokumente unter Open-Content-Lizenzen (insbesondere CC-Lizenzen) zur Verfügung gestellt haben sollten, gelten abweichend von diesen Nutzungsbedingungen die in der dort genannten Lizenz gewährten Nutzungsrechte. 

#### **Terms of use:** 

_Documents in EconStor may be saved and copied for your personal and scholarly purposes._ 

_You are not to copy documents for public or commercial purposes, to exhibit the documents publicly, to make them publicly available on the internet, or to distribute or otherwise use the documents in public._ 

_If the documents have been made available under an Open Content Licence (especially Creative Commons Licences), you may exercise further usage rights as specified in the indicated licence._ 

http://creativecommons.org/licenses/by/4.0/ 



Finance and Stochastics (2024) 28:759–812 https://doi.org/10.1007/s00780-024-00536-2 



# **Optimal trade execution under small market impact and portfolio liquidation with semimartingale strategies** 

**Ulrich Horst**<sup>**1**</sup> **· Evgueni Kivman**<sup>**1**</sup> 

Received: 6 June 2022 / Accepted: 5 July 2023 / Published online: 14 June 2024 © The Author(s) 2024 

### **Abstract** 

We consider an optimal liquidation problem with instantaneous price impact and stochastic resilience for small instantaneous impact factors. Within our modelling framework, the optimal portfolio process converges to the solution of an optimal liquidation problem with general semimartingale controls when the instantaneous impact factor converges to zero. Our results provide a unified framework within which to embed the two most commonly used modelling frameworks in the liquidation literature and provide a foundation for the use of semimartingale liquidation strategies and the use of portfolio processes of unbounded variation. Our convergence results are based on novel convergence results for BSDEs with singular terminal conditions and novel representation results of BSDEs in terms of uniformly continuous functions of forward processes. 

**Keywords** Portfolio liquidation · Singular BSDE · Stochastic liquidity · Singular control 

**Mathematics Subject Classification** 93E20 · 91B70 · 60H30 

**JEL Classification** G11 · G12 · G19 

## **1 Introduction** 

The impact of limited liquidity on optimal trade execution has been extensively analysed in the mathematical finance and stochastic control literature in recent years. The majority of the optimal portfolio liquidation literature allows one of two possible price impacts. The first approach, pioneered by Bertsimas and Lo [6] and Almgren 

> � U. Horst horst@math.hu-berlin.de 

> E. Kivman kivmanev@hu-berlin.de 

> 1 Department of Mathematics, Humboldt-Universität zu Berlin, Unter den Linden 6, 10099 Berlin, Germany 

U. Horst, E. Kivman 

760 

and Chriss [3], divides the price impact into a purely temporary effect, which depends only on the present trading rate and does not influence future prices, and a permanent effect, which influences the price depending only on the total volume that has been traded in the past. The temporary impact is typically assumed to be linear in the trading rate, leading to a quadratic term in the cost functional. The original modelling framework has been extended in various directions, including general stochastic settings with and without model uncertainty and multi-player and mean-field-type models by many authors, including Ankirchner et al. [4], Cartea et al. [9], Fu et al. [14], Gatheral and Schied [17], Graewe et al. [19], Horst et al. [23], Kruse and Popier [24] and Neuman and Voß [29]. 

A second approach, initiated by Obizhaeva and Wang [30], assumes that price impact is not permanent but transient, with the impact of past trades on current prices decaying over time. When impact is transient, one often allows both absolutely continuous and singular trading strategies. When singular controls are admissible, optimal liquidation strategies usually comprise large block trades at the initial and terminal time. The work of Obizhaeva and Wang has been extended by Alfonsi et al. [2], Chen et al. [11], Fruth et al. [13], Gatheral [16], Guéant [20], Horst and Naujokat [21], Lokka and Xu [27] and Predoiu et al. [31], among many others. 

Single- and multi-asset liquidation problems with instantaneous and transient market impact and stochastic resilience where trading is confined to absolutely continuous strategies have been analysed in Graewe and Horst [18] and Horst and Xia [22], respectively. This is consistent with the empirical work of Large [25] and Lo and Hall [26], which suggests that this resilience does indeed vary stochastically. Although only absolutely continuous trading strategies were admissible in [18, 22], numerical simulations reported in [18] suggest that if all model parameters are deterministic constants, then the optimal portfolio process converges to the solution in [30] with two block trades and a constant trading rate as the instantaneous impact parameter converges to zero. Cartea and Jaimungal [10] provide empirical evidence that the instantaneous price impact is indeed (much) smaller than permanent (or transient) price impact. The numerical simulations in [18] suggest that the model in [18] provides a common framework within which to embed the two most commonly used liquidation models [3, 30] as limiting cases. 

This paper provides a rigorous convergence analysis within a Markovian factor model. It turns out that the stochastic setting is quite different from the deterministic one. Most importantly, we show that in the stochastic setting, the optimal portfolio processes obtained in [18] converge in the Skorohod _M_ 2-topology to a process of _infinite variation_ with jumps as the instantaneous market impact parameter converges to zero. Our second main result is to prove that the limiting portfolio process is optimal in a liquidation model with semimartingale execution strategies and to explicitly compute the optimal trading cost in the semimartingale execution framework. 

Showing that the limiting model solves a liquidation model with semimartingale execution strategies is more than a mere byproduct. Control problems with semimartingale strategies are usually difficult to solve because there are no canonical candidates for the value function and/or optimal strategies. We show that the solution in the limiting model is fully determined by the unique bounded solution to a onedimensional quadratic BSDE. Our limit result provides a novel approach to solving 

Optimal trade execution under small market impact 

761 

control problems with semimartingale strategies that complements the approaches in Ackermann et al. [1] and Lorenz and Schied [15]. They solved related models by passing to a continuous-time limit from a sequence of discrete-time models. 

Within a portfolio liquidation framework, inventory processes with infinite variation were first considered by Lorenz and Schied [28] to the best of our knowledge. Later, Becherer et al. [5] considered a trading framework with generalised price impact and proved that the cost functional depends continuously on the trading strategy, considered in several topologies. Bouchard et al. [7] considered infinite-variation inventory processes in the context of hedging. 

The paper closest to ours is the recent work by Ackermann et al. [1]. They considered a liquidation model with general RCLL semimartingale trading strategies. Their framework is more general than ours as they allow more general filtrations and stochastic order book depth. At the same time, their analysis is confined to riskneutral traders. In our setting, when the model parameters are deterministic and the instantaneous price impact goes to zero, the case of risk-neutral traders – which is then a special case of the model studied in [1] – is explicitly solvable. Allowing for risk aversion renders the impact model significantly more complicated as it adds a quadratic dependence of the integrated trading rate into the HJB equation; cf. [18] for details. 

Our work also complements the work of Gârleanu and Pedersen [15]. They consider an array of market impact models, including a model with purely transient costs. They write [15, beginning of Sect. 1.3] that “with purely persistent price-impact costs, the optimal portfolio policy can have jumps and infinite quadratic variation.” As in [1], they justify portfolio holdings with infinite quadratic variation by taking a limit of a sequence of discrete-time models with increasing trading frequency. They also prove that the optimal portfolio processes in the discrete-time models converge to the optimal portfolio process in the corresponding continuous-time model if either the instantaneous price impact converges to a positive constant or the instantaneous price impact factor multiplied by the (increasing) trading frequency converges to zero. However, they do not consider the general case of an instantaneous price impact factor converging to zero. Most importantly, they consider a portfolio choice problem on an infinite time horizon, which avoids the liquidation constraint at the terminal time. 

Last but not least, our work complements the work of Carmona and Webster [8], who provide strong evidence that inventories of large traders often do have indeed a nontrivial quadratic-variation component. For instance, for the Blackberry stock, they analyse the inventories of “the three most active Blackberry traders” on a particular day, namely CIBC World Markets Inc., Royal Bank of Canada Capital Markets and TD Securities Inc. From their data, they “suspect that RBC (resp. TD Securities) were trading to acquire a long (resp. short) position in Blackberry” and found that the corresponding inventory processes were of infinite variation. More generally, they find that systematic tests “on different days and other actively traded stocks give systematic rejections of this null hypothesis [quadratic variation of inventory being zero], with a _p_ -value never greater than 10<sup>−5</sup> ”. Our results suggest that inventories with nontrivial quadratic variation arise naturally when market depth is high and resilience and/or market risk fluctuates stochastically. This is very intuitive; in deep markets, it is comparably cheap to frequently adjust portfolios to stochastically varying market environments. 

U. Horst, E. Kivman 

762 

The main technical challenge in establishing our convergence results is that the solution to the limiting model cannot be obtained by taking the limit of the threedimensional quadratic BSDE system that characterises the solution in the model with positive instantaneous impact. Instead, we prove that the limit is fully characterised by the solution to a one-dimensional quadratic BSDE. Remarkably, this BSDE is independent of the liquidation requirement. As a result, full liquidation takes place if the instantaneous impact parameter converges to zero even if full liquidation is not strictly required. The reason is a loss in book value of the remaining shares that outweighs the liquidation cost for small instantaneous impact. Our convergence result is based on a novel representation result for solutions to BSDEs driven by Itô processes in terms of _uniformly_ continuous functions of the forward process and on a series of novel convergence results for sequences of singular stochastic integral equations and random ODEs, which we choose to report in an abstract setting in Appendix A and B, respectively. 

The limiting portfolio process is optimal in a liquidation model with general semimartingale execution strategies. Within our modelling framework where the cost coefficients are driven by continuous factor processes, block trades optimally occur only at the beginning and the end of the trading period. This is very intuitive as one would expect large block trades to require some form of trigger such as an external shock leading to a discontinuous change of cost coefficients. The proof of optimality proceeds in three steps. We first prove that the jump part of the limiting portfolio process can be approximated by absolutely continuous processes. This allows us to approximate the trading costs in the semimartingale model by trading costs in the pre-limit models from which we finally deduce the optimality of the limiting process in the semimartingale model by using the optimality of the approximating inventory processes in the pre-limit models. As a byproduct of our approximation, we also obtain that the optimal costs are given in terms of the aforementioned one-dimensional quadratic BSDE. 

The rest of the paper is organised as follows. In Sect. 2, we recall the modelling setup from [18, 22] and summarise our main results. The proofs are given in Sects. 3 and 4. A series of fairly abstract convergence results for various stochastic equations with singularities upon which our convergence results are based is postponed to two appendices. 

**Notation.** Throughout, randomness is described by an R<sup>_m_</sup> -valued Brownian motion _(Wt )t_ ∈[0 _,T_ ] defined on _(�, F, (Ft )t_ ∈[0 _,T_ ] _,_ P _)_ , a complete probability space, where _(Ft )t_ ∈[0 _,T_ ] denotes the filtration generated by _W_ , augmented by the P-nullsets. Unless otherwise specified, all equations and inequalities hold in the P-a.s. sense. For a subset _A_ ⊆ R<sup>_d_</sup> , we denote by _L_<sup>2</sup> Prog<sup>_(�_× [0</sup><sup>_, T_];</sup><sup>_A)_thesetofallprogressively</sup> measurable _A_ -valued stochastic processes _(Xt )t_ ∈[0 _,T_ ] such that E[�0 _T_<sup>|</sup><sup>_Xt_|2 d</sup><sup>_t_]</sup><sup>_<_∞,</sup> while _L_<sup>∞</sup> Prog<sup>_(�_× [0</sup><sup>_, T_];</sup><sup>_A)_denotes the subset of essentially bounded processes. By</sup> _L_<sup>2</sup> _P_<sup>_(�_× [0</sup><sup>_, T_];</sup><sup>_A)_and</sup><sup>_L_∞</sup> _P_<sup>_(�_× [0</sup><sup>_, T_];</sup><sup>_A)_, we denote the respective subsets of pre-</sup> dictable processes. Whenever _T_ − appears, we mean that there exists an _ε >_ 0 such that a statement holds for all _T_<sup>′</sup> ∈ _(T_ − _ε, T )_ . 

Optimal trade execution under small market impact 

763 

## **2 Problem formulation and main results** 

In this section, we introduce two portfolio liquidation models with stochastic market impact. In the first model, analysed in Sect. 2.1, the investor is confined to absolutely continuous trading strategies. For small instantaneous market impact, we prove that the optimal liquidation strategy converges to a semimartingale with jumps. In Sect. 2.2, we therefore analyse a liquidation model with semimartingale trading strategies. We prove that the limiting process obtained in Sect. 2.1 is optimal in a model where semimartingale strategies that satisfy a suitable regularity condition are admissible. 

### **2.1 Portfolio liquidation with absolutely continuous strategies** 

We take the liquidation model analysed in Graewe and Horst [18] and Horst and Xia [22] as our starting point and consider an investor that needs to close, within a given time interval [0 _, T_ ], a (single-asset) portfolio of _x_ 0 _>_ 0 shares using a trading strategy _ξ_ = _(ξt )t_ ∈[0 _,T_ ]. If _ξt <_ 0, the investor is selling the asset at a rate _ξt_ at time _t_ ∈[0 _, T_ ]; otherwise she is buying it. For a given strategy _ξ_ , the corresponding portfolio process _X_<sup>_ξ_</sup> = _(Xt_<sup>_ξ)t_∈[0</sup><sup>_,T_]satisfies the ODE</sup> 



The set of _admissible strategies_ is given by 



For a general inventory process _X_ ∈ _L_<sup>2</sup> _P_<sup>_(�_× [0</sup><sup>_, T_]; R</sup><sup>_)_, the corresponding</sup><sup>_transient_</sup> _price impact_ is described by the unique stochastic process _Y_<sup>_X_</sup> = _(Yt_<sup>_X)t_∈[0</sup><sup>_,T_]that</sup> satisfies the ODE 



for some constant _γ >_ 0 and some essentially bounded, progressively measurable, _(_ 0 _,_ ∞ _)_ -valued process _ρ_ = _(ρt )t_ ∈[0 _,T_ ]. The process _Y_<sup>_X_</sup> may be viewed as describing an additional shift in the unaffected benchmark price process generated by the large investor’s trading activity. For _ξ_ ∈ _A_ , we write _Y_<sup>_ξ_</sup> := _Y_<sup>_Xξ_</sup> . 

For any _instantaneous impact factor η >_ 0 and any _penalisation factor_ 



where 



U. Horst, E. Kivman 

764 

the _cost functional_ is given by 



The first term in _J_<sup>_η,N_</sup> _(ξ)_ captures the _instantaneous trading costs_ , the second the costs from a _transient price impact_ , and the third captures _market risk_ , where the adapted and nonnegative process _λ_ = _(λt )t_ ∈[0 _,T_ ] specifies the degree of risk. If _N_ = ∞, the fourth term should formally be read as +∞1{ _XTξ_ ̸<sup>=0}withtheconven-</sup> tion 0 · ∞= 0. This captures the case where full liquidation is required; this case is analysed in [18]. The case _γ_ + 1 ≤ _N <_ ∞ is analysed in [22]. The fifth term captures an additional loss in the book value of the remaining shares. It drops out of the cost function if _N_ = ∞; see [18, 22] for further details on the impact costs and cost coefficients. 

It has been shown in [18, 22] that the optimisation problem 



has a solution _ξ_<sup>ˆ</sup><sup>_η,N_</sup> for any _N_ ∈ _N_ and any _η >_ 0. The solution is given in terms of a backward SDE system with possibly singular terminal condition. We index the optimal trading strategies and state processes by _η_ and _N_ as we are interested in their behaviour for small instantaneous impact factors for both finite and infinite _N_ . 

The next theorem follows directly from [18, Theorems 2.5 and 2.6] and [22, Theorem 2.1]. 

**Theorem 2.1** _With the above assumptions_ , _for all η_ ∈ _(_ 0 _,_ ∞ _) and N_ ∈ _N_ , _the following hold_ : 

i) _The BSDE system_ 



_with terminal condition_ 



Optimal trade execution under small market impact 

765 

_together with A_<sup>_η,N_</sup> _T_ = _N for N <_ ∞ _and_ lim _t_ ↗ _T At_<sup>_η,_∞</sup> = ∞ _in L_<sup>∞</sup> _if N_ = ∞, _has a solution_ 



_that belongs to the space L_<sup>∞</sup> _P_<sup>_(�_× [0</sup><sup>_, T_]; R3</sup><sup>_)_×</sup><sup>_L_2</sup> Prog<sup>_(�_× [0</sup><sup>_, T_]; R3×</sup><sup>_m) if N<_∞</sup> _and to the space L_<sup>∞</sup> _P_<sup>_(�_× [0</sup><sup>_, T_−]; R3</sup><sup>_)_×</sup><sup>_L_2</sup> Prog<sup>_(�_× [0</sup><sup>_, T_−]; R3×</sup><sup>_m) if N_= ∞.</sup> ii) _The liquidation problem_ (2.2) _has a solution ξ_<sup>ˆ</sup><sup>_η,N_</sup> . _The corresponding state process_ 



_is given by the_ ( _unique_ ) _solution to the ODE system_ 



_with initial conditions X_<sup>ˆ</sup> 0<sup>_η,N_</sup> = _x_ 0 _and Y_<sup>ˆ</sup> 0<sup>_η,N_</sup> = 0, _where_ 



Let us now define the process 



The benefit of defining this process is that in the ODE (2.3), the terms that are multiplied by � _η_<sup>−1</sup> drop out so that we expect that the process _Z_<sup>ˆ</sup><sup>_η,N_</sup> remains stable for small values of _η_ . Next, we state a result on the optimal state process and the previously introduced process _Z_<sup>ˆ</sup><sup>_η,N_</sup> that will be important for our subsequent analysis. In particular, we show that the optimal portfolio process _X_<sup>ˆ</sup><sup>_η,N_</sup> never changes its sign. The proof is given in Sect. 3.1.1. 

**Theorem 2.2** _For all η_ ∈ _(_ 0 _,_ ∞ _)_ , _N_ ∈ _N_ , _the process Z_<sup>ˆ</sup><sup>_η,N_</sup> _is nonincreasing on_ [0 _, T_ ]. _Moreover_ , 



We are interested in the dynamics of the optimal portfolio processes for small instantaneous price impact. We address this problem within a factor model where the cost coefficients _λ_ and _ρ_ are driven by an Itô diffusion, which is given by the unique strong solution to the SDE 



U. Horst, E. Kivman 

766 

on [0 _, T_ ] with _<u>χ</u>_ 0 ∈ R<sup>_n_</sup> . We assume **throughout** that the function 



is bounded, measurable and uniformly Lipschitz-continuous in the space variable, i.e., 



**Assumption 2.3** The processes _ρ_ and _λ_ are of the form 



for some bounded _C_<sup>1</sup><sup>_,_2</sup> function _f_ = _(f_<sup>_ρ_</sup> _, f_<sup>_λ_</sup> _)_ : [0 _, T_ ] × R<sup>_n_</sup> →[0 _,_ ∞ _)_<sup>2</sup> with bounded derivatives. Moreover, the function _f_<sup>_ρ_</sup> is bounded away from zero. 

For convenience, we define the stochastic process _ϕ_ = _(ϕt )t_ ∈[0 _,T_ ] by 



and choose constants _<u>ρ, ρ, ϕ, ϕ,</u> λ_ ∈ _(_ 0 _,_ ∞ _)_ such that 



In what follows, we heuristically argue that the processes _X_<sup>ˆ</sup><sup>_η,N_</sup> converge to a limit process _X_<sup>ˆ0</sup> (independent of _N_ ) as _η_ → 0 and identify the limit _X_<sup>ˆ0</sup> . Since the ODE system (2.3) is not defined for _η_ = 0, we cannot define the limiting process as the solution to this system. Instead, we first identify the limits of the coefficients of the ODE system and then derive candidate limits for the state processes in terms of the limiting coefficients. 

### **2.1.1 Convergence of the coefficient processes** 

In this section, we state the convergence results for the coefficient processes _D_<sup>_η,N_</sup> and _E_<sup>_η,N_</sup> of the ODE system (2.3) as _η_ → 0. In particular, we prove that their limits _D_<sup>0</sup> and _E_<sup>0</sup> exist and are driven by a common factor, which is given by the solution of a quadratic BSDE. 

Before proceeding to the limit result, we provide some heuristics for the convergence. Assuming for simplicity that all coefficients are deterministic, the dynamics of the coefficient processes satisfy 



Letting _η_ → 0, we expect that 



Optimal trade execution under small market impact 

767 

that is, we expect that 



Moreover, by the choice of the coefficients _D_<sup>_η,N_</sup> and _E_<sup>_η,N_</sup> and by the BSDE in Theorem 2.1 i), we expect that in the limit, 



The three equalities combined yield 



Plugging (2.5) and (2.6) back into (2.4) yields 



Hence we expect _D_<sup>0</sup> and _E_<sup>0</sup> to be driven by _B_<sup>0</sup> and _B_<sup>0</sup> to satisfy the ODE 



with terminal condition _BT_<sup>0= 1 (because</sup><sup>_B_</sup> _T_<sup>_η,N_</sup> = 1). Our heuristic also suggests that the limit processes are independent of the liquidation requirement. 

**Example 2.4** If _λ_ = _Cρ_ for some constant _C_ ≥ 0, then the process _B_<sup>0</sup> can be computed explicitly. If _C_ = 0, then _Bt_<sup>0= 2</sup><sup>_/(_2 +</sup> � _tT_<sup>_ρs_d</sup><sup>_s)_. If</sup><sup>_C>_0, then</sup> 



The preceding heuristic suggests that the limiting coefficient processes are driven by a solution to the BSDE corresponding to the above ODE for _B_<sup>0</sup> . The following lemma is proved in Sect. 3.1.2. 

**Lemma 2.5** _There exists a unique solution (B_<sup>0</sup> _, Z_<sup>0</sup><sup>_,B_</sup> _) in the space_ 



_to the BSDE_ 



U. Horst, E. Kivman 

768 

_The process B_<sup>0</sup> _is bounded from above by_ 1 _and from below by_ 



_Moreover_ , _there exists a_ uniformly _continuous function_ ( _a so-called “decoupling field”_ ) _h_ : [0 _, T_ ] × R<sup>_n_</sup> → R _such that B_<sup>0</sup> _and (h(t, χt ))t_ ∈[0 _,T_ ] _are indistinguishable_ . 

We prove below that the process _B_<sup>_η,N_</sup> converges to _B_<sup>0</sup> as _η_ → 0 and that _D_<sup>_η,N_</sup> and _E_<sup>_η,N_</sup> converge to the processes 



respectively. In view of Lemma 2.5, these processes are well defined, and so the dynamics (2.7) of the process _B_<sup>0</sup> can be rewritten as 



Likewise, the BSDE for the process _B_<sup>_η,N_</sup> in Theorem 2.1 can be rewritten as 



This suggests that the process _B_<sup>_η,N_</sup> converges to _B_<sup>0</sup> on the entire interval [0 _, T_ ], and this is proved below; note that all the processes _B_ have the same terminal condition at time _T_ . By contrast, convergence of the processes _D_<sup>_η,N_</sup> and _E_<sup>_η,N_</sup> can only be expected to hold on compact subintervals [0 _, T_ − _ε_ ] of [0 _, T )_ because the terminal conditions of the limiting and approximating processes are different. It turns out that we can prove a slightly stronger result for the supremum of the processes _D_ and _E_ , and this explains the different choices of intervals for the supremum and the infimum in the following lemma. Specifically, we have the following result. Its proof is given in Sect. 3.2.1. 

### **Proposition 2.6** _Let_ 



_Then for all ε >_ 0, _there exists an η_ 0 _>_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 0] _and for all N_ ∈ _N_ , 



### **2.1.2 Convergence of the state process** 

Having derived the limits of the coefficient processes, we can now heuristically derive the limits of the processes _X_<sup>ˆ</sup><sup>_η,N_</sup> , _Y_<sup>ˆ</sup><sup>_η,N_</sup> and _Z_<sup>ˆ</sup><sup>_η,N_</sup> , which we denote by _X_<sup>ˆ0</sup> , _Y_<sup>ˆ0</sup> and _Z_ ˆ<sup>0</sup> , respectively. 

Optimal trade execution under small market impact 

769 

Since _X_<sup>ˆ</sup> _T_<sup>_η,_∞</sup> = 0 for all _η >_ 0, we expect that _X_<sup>ˆ</sup> _T_<sup>0=0. We prove in Lemma 3.11</sup> that this convergence also holds if _N_ is finite. The proof heavily relies on the optimality of _X_<sup>ˆ0</sup> in the semimartingale portfolio liquidation model. 

Assuming that the optimal trading strategy remains stable if _η_ → 0, the ODE (2.3) suggests that the term _D_<sup>_η,N_</sup> _X_<sup>ˆ</sup><sup>_η,N_</sup> + _E_<sup>_η,N_</sup> _Y_<sup>ˆ</sup><sup>_η,N_</sup> is small for small _η_ and hence that 



We do not conjecture the above relation at the terminal time because the convergence of _D_<sup>_η,N_</sup> and _E_<sup>_η,N_</sup> only holds on [0 _, T )_ . Assuming that 



equation (2.6) implies 



On the other hand, by definition, _∂t Z_<sup>ˆ</sup><sup>_η,N_</sup> = _ρY_<sup>ˆ</sup><sup>_η,N_</sup> . As _η_ → 0, this suggests that _∂t Z_<sup>ˆ0</sup> = _ρY_<sup>ˆ0</sup> and hence that 



This motivates us to _define the process_ 



Since we expect that _X_<sup>ˆ</sup> _T_<sup>0=0andthat</sup><sup>_Z_ˆ0=</sup><sup>_γX_ˆ0−ˆ</sup><sup>_Y_0,wenowintroducethe</sup> _candidate limiting state processes_ 



Since _(X_<sup>ˆ</sup> 0<sup>0</sup> −<sup>_,Y_ˆ</sup> 0<sup>0</sup> −<sup>_)_=</sup><sup>_(x_0</sup><sup>_,_0</sup><sup>_)_,weexpectthatthelimitingstateprocessjumpsatthe</sup> initial and the terminal time. In particular, we cannot expect uniform convergence on [0 _, T_ ]. 

We also expect the limiting state processes to be of unbounded variation, as suggested by Fig. 1. The figure also suggests that the portfolio process is more or less monotone for large _η_ , while this property is lost for small _η_ . When _η_ → 0, adjustments to small changes in market environments are cheap. This is very different from round-trip strategies where an investor uses the price impact caused by him/herself to drive market prices into a favourable direction. 

Figure 1 also suggests that the limiting portfolio process jumps only at the times 0 and _T_ . This is consistent with the definition of the candidate processes (2.11) as 

U. Horst, E. Kivman 

770 



**Fig. 1** Optimal trading strategies _X_<sup>ˆ</sup><sup>_η,_∞</sup> for the liquidation model for different instantaneous impact factors, and their limit _X_<sup>ˆ0</sup> for _m_ = _n_ = _T_ = _x_ 0 = 1, _γ_ = 3, _ρt_ = 1 + 0 _._ 9 sin _(_ 2 _._ 5 _Wt )_ , _λ_ ≡ 1 

well as the observation in Horst and Naujokat [21], according to which jumps in the optimal strategy can only be triggered by exogenous shocks like jumps in the cost coefficients, which are absent in the present model. 

It remains to clarify in which sense the state processes converge. In contrast to the convergence result in Proposition 2.6, we can only expect convergence in probability because the state process follows a forward ODE while the coefficient processes follow backward SDEs; see also Appendix B.2. The following result establishes uniform convergence in probability on compact subintervals of _(_ 0 _, T )_ along with some “upper/lower convergence” at the initial and terminal time. The proof is given in Sect. 3.2.2. 

**Theorem 2.7** _For all ε >_ 0 _and δ >_ 0, _there exists an η_ 0 _>_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 0] _and all N_ ∈ _N_ , 



The preceding theorem does not provide a convergence result on the whole time interval, due to the jumps of the limit processes at the initial and terminal time. However, along with our results from Sect. 2.2, it allows us to prove the convergence of the graphs of the state processes on the entire time interval. The completed graph of an RCLL function _X_ : {0−} ∪[0 _, T_ ] → R with finitely many jumps is defined by 



Optimal trade execution under small market impact 

771 

The Skorohod _M_ 2-distance between _X_ and _Y_ is defined as the Hausdorff distance between their completed graphs, i.e., 



where 



If strict liquidation is required, then Theorem 2.7 is sufficient to prove convergence of the state processes in the Skorohod _M_ 2 sense. Even if liquidation is not required, it turns out that the terminal position converges to zero as _η_ → 0. Heuristically, this can be seen as follows. 

Let _t_ 0 ∈ _(_ 0 _, T )_ . Disregarding market risk costs, which we expect to be of order _O(T_ − _t_ 0 _)_ and hence negligible if _t_ 0 → _T_ , and disregarding instantaneous impact costs for the moment, the cost functional for any given admissible strategy _ξ_ is given by 



Hence we expect the _controllable_ costs to satisfy 

plus instantaneous impact costs. Since _N > γ_ , this suggests to make _XT_<sup>_ξ_small, which</sup> is cheap if _η_ is small. More precisely, we have the following result; its proof is given in Sect. 3.2.3. 

**Proposition 2.8** _For all ε >_ 0 _and δ >_ 0, _there exists an η_ 0 _>_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 0] _and all N_ ∈ _N_ , 



U. Horst, E. Kivman 

772 

### **2.2 Optimal liquidation with semimartingale strategies** 

In this section, we prove that the limit process _X_<sup>ˆ0</sup> is the optimal portfolio process in a trade execution model with semimartingale trading strategies. 

In our semimartingale model, a trading strategy is given by a triple 



where _j_<sup>+</sup> and _j_<sup>−</sup> are real-valued, nondecreasing pure jump processes and _V_ is with respect to the Brownian filtration a real-valued continuous semimartingale starting in zero. The jump processes _j_<sup>+</sup> and _j_<sup>−</sup> describe the cumulative effects of buying, respectively selling, large blocks of shares, while the continuous semimartingale _V_ describes the effect of continuously trading small amounts of the stock. The portfolio process _X_<sup>_θ_</sup> = _(Xt_<sup>_θ)t_∈[0</sup><sup>_,T_] associated with a strategy</sup><sup>_θ_is then given by</sup> 



The associated price impact process, again given by (2.1), is denoted by _Y_<sup>_θ_</sup> . We note that _X_<sup>_θ_</sup> and _Y_<sup>_θ_</sup> are semimartingales. 

We assume that strict liquidation is required and that the cost associated with a trading strategy _θ_ is given by 



The first term captures the transient price impact cost; the second captures market risk. The third term emerges as an additional cost term when passing from discrete to continuous time, as shown in Ackermann et al. [1]. Moreover, in the absence of this term, arbitrarily low costs can be achieved; see [1] for details. 

The cost function can be conveniently rewritten as 



This representation supports our intuition that the price impact before and the price impact after the jump equally influence the total cost. The first term in this expression captures the cost of the initial block trade at time _t_ = 0. 

The cost functional is well defined under the following admissibility condition. 

**Definition 2.9** A trading strategy _θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ is called _admissible_ if the liquidation constraint _XT_<sup>_θ_=0holds,if</sup><sup>_j_±areRCLL,predictable,real-valued,non-</sup> decreasing and square-integrable pure jump processes, and if _V_ is a continuous semimartingale starting in zero with 



The set of all admissible trading strategies is denoted by _A_<sup>0</sup> . 

Optimal trade execution under small market impact 

773 

Our goal is now to solve the optimisation problem 



To this end, we verify directly that the limit process _X_<sup>ˆ0</sup> obtained in Sect. 2.1 is optimal. The results there show that the process has the representation 



where the jump process _j_<sup>ˆ−</sup> and the continuous part are given by, respectively, 



In view of Assumption 2.3 and because _B_<sup>0</sup> is a continuous semimartingale and _Z_<sup>ˆ0</sup> is differentiable, the process _V_<sup>ˆ</sup> is a continuous semimartingale starting in zero. Hence the following holds. 

**Lemma 2.10** _The strategy θ_<sup>ˆ</sup> = _(_ 0 _, j_<sup>ˆ−</sup> _, V )_<sup>ˆ</sup> _is admissible_ . 

In order to prove that _θ_<sup>ˆ</sup> is optimal, we approximate the cost and the portfolio process associated with any strategy _θ_ ∈ _A_<sup>0</sup> by the cost and portfolio processes corresponding to absolutely continuous trading strategies. To this end, we first approximate the continuous semimartingale part _V_ by differentiable processes. The proof of the following result is given in Sect. 4.1. 

**Lemma 2.11** _For all θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> _and all β, δ >_ 0, _there exist a constant ν >_ 0 _and an adapted and continuous V_<sup>˙</sup><sup>_β,ν_</sup> : _�_ × [0 _, T_ ] →[−<sup>_<u>β</u>_</sup> _ν_<sup>_,_</sup><sup>_<u>β</u>_</sup> _ν_<sup>]</sup><sup>_such that_</sup> 



_and_ 



Next, we approximate the portfolio process _X_<sup>_θ_</sup> by a portfolio process associated with an absolutely continuous strategy. To this end, for all _θ_ ∈ _A_<sup>0</sup> and _β, ν, ε >_ 0, we define the integrable process 



In view of the square-integrability of _j_<sup>±</sup> , we see that _ξ_<sup>_θ,β,ν,ε_</sup> belongs to _A_ . The corresponding portfolio process is denoted by _X_<sup>_θ,β,ν,ε_</sup> . The proof of the following result is given in Sect. 4.2. 

U. Horst, E. Kivman 

774 

**Lemma 2.12** _For all θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> _and for all δ >_ 0, _there exist β >_ 0, _ν >_ 0 _and ε >_ 0 _such that_ 



For all _ξ_ ∈ _A_ with �0 _T_<sup>_ξs_d</sup><sup>_s_=−</sup><sup>_x_0, we can define</sup><sup>_V_</sup> _t_<sup>_ξ_:=</sup> �0 _t_<sup>_ξs_d</sup><sup>_s_to obtain that</sup> _(_ 0 _,_ 0 _, V_<sup>_ξ_</sup> _)_ ∈ _A_<sup>0</sup> and for all _η >_ 0, we have 



The preceding lemma allows us to establish a cost estimate. The proof is given in Sect. 4.3. 

**Lemma 2.13** _For all C >_ 0, _there exists a constant D(C) >_ 0 _such that for all θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> _with_ E[�0 _T_<sup>_(X_</sup> _t_<sup>_θ)_2 d</sup><sup>_t_] ≤</sup><sup>_C and all ξ_∈</sup><sup>_A with_</sup> �0 _T_<sup>_ξs_d</sup><sup>_s_= −</sup><sup>_x_0,</sup> _we have_ 



As a consequence of the previous results, the optimal instantaneous price impact term converges to zero as _η_ → 0. The proof is given in Sect. 4.4. 

**Lemma 2.14** _We have_ 



The cost estimate in Lemma 2.13 allows us to establish the optimality of the trading strategy _θ_<sup>ˆ</sup> by using the optimality of _ξ_<sup>ˆ</sup><sup>_η,_∞</sup> in the strict liquidation model with absolutely continuous strategies. It turns out that the minimal trading costs are fully determined by the initial value _B_ 0<sup>0of the process</sup><sup>_B_0along with the impact factor</sup><sup>_γ_</sup> and the initial portfolio. 

**Theorem 2.15** _It holds that_ 



**_Proof_** Let us assume to the contrary that _θ_<sup>ˆ</sup> does not minimise the cost functional _J_<sup>0</sup> over the set _A_<sup>0</sup> . Then there exist a strategy _θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> and a constant 

Optimal trade execution under small market impact 

775 

_δ_ ∈ _(_ 0 _,_ ∞ _)_ such that _J_<sup>0</sup> _(θ)_ + _δ_ ≤ _J_<sup>0</sup> _(θ)_<sup>ˆ</sup> . Moreover, due to (2.15) and Lemma 2.13, for all _η, β, ν, ε_ ∈ _(_ 0 _,_ ∞ _)_ , we have (for convenience, let _p(x)_ := _x_ +<sup>√</sup> _<u>x</u>_ <u>)</u> 



According to Lemma 2.12, we can first choose _β, ν, ε_ ∈ _(_ 0 _,_ ∞ _)_ and then, due to | _X_<sup>ˆ</sup> _t_<sup>_η,_∞</sup> | ≤ _x_ 0 (cf. Theorem 2.2), choose _η_ ∈ _(_ 0 _,_ ∞ _)_ small enough according to Theorem 2.7 such that the right-hand side of (2.16) is negative. This contradicts the optimality of _ξ_<sup>ˆ</sup><sup>_η,_∞</sup> for _J_<sup>_η,_∞</sup> and proves the optimality of _θ_<sup>ˆ</sup> . 

It remains to compute _J_<sup>0</sup> _(θ)_<sup>ˆ</sup> . In view of (2.15), for all _η_ ∈ _(_ 0 _,_ ∞ _)_ , we have 



The difference of the first two terms converges to zero as _η_ → 0, which is verified by using Lemma 2.13 and Theorem 2.7 as in the above calculation. The third term converges to zero by Lemma 2.14. Hence, using the representation of the value function given in [18], 



**_Remark 2.16_** If _λ_ ≡ 0, our model is a special case of the model analysed in Ackermann et al. [1], which also contains cases when there is no optimal trading strategy. However, since _γ_ is constant in our model, the processes “ _μ_ ” and “ _σ_ ” introduced in [1] are equal to zero. This implies that the equation “ _β_<sup>˜</sup> = _Y_ ” holds in their notation. As shown in [1, Sect. 5], the process “ _M_<sup>⊥</sup> ” introduced therein is also equal to zero, which implies that “ _Y_ ” and hence “ _β_<sup>˜</sup> ” is a semimartingale. Thus [1, Theorem 2.3 (ii)] confirms that under this property, an optimal trading strategy does indeed exist. 

U. Horst, E. Kivman 

776 

## **3 Proofs for Sect. 2.1** 

This section proves the results stated in Sect. 2.1. We start with a priori estimates and regularity properties for the coefficient processes that specify the optimal state processes. 

### **3.1 A priori estimates and regularity properties** 

### **3.1.1 The case** **_η >_ 0** 

The following estimates have been established in Graewe and Horst [18] and Horst and Xia [22], except for the upper bound on _E_<sup>_η,N_</sup> for finite _N_ which is stronger than the corresponding one in [22]. It can be established using the same arguments as in [18, proof of Proposition 3.2] by noting that _DT_<sup>_η,N_</sup> _<_ ∞ if _N_ is finite. 

### **Lemma 3.1** _Let_ 



_For all η_ ∈ _(_ 0 _,_ ∞ _)_ , _N_ ∈ _N and s, t_ ∈[0 _, T ) with s_ ≤ _t_ , _we have that_ 



The preceding estimates allow us to prove that neither the optimal portfolio process nor the corresponding spread process change sign. 

**_Proof_** _of Theorem 2.2_ Put _V (t)_ := _X_ ˆ _t_<sup>_η,N_</sup> _Y_ ˆ _t_<sup>_η,N_</sup> . Then _V (_ 0 _)_ = 0 and _V_ satisfies the ODE 



As a result, 



Optimal trade execution under small market impact 

777 

In view of Lemma 3.1, this shows that _V (t) <_ 0 on _(_ 0 _, T )_ . Hence strict positivity of _X_ ˆ 0<sup>_η,N_</sup> yields 



Thus the definition of _Z_<sup>ˆ</sup><sup>_η,N_</sup> along with (2.3) yields _∂t Z_<sup>ˆ</sup> _t_<sup>_η,N_</sup> = _ρt Y_<sup>ˆ</sup> _t_<sup>_η,N_</sup> _<_ 0 on _(_ 0 _, T )_ . Moreover, 



Next, we prove that the process _B_<sup>_η,N_</sup> satisfies an _L_<sup>1</sup> uniform continuity property. We refer to Appendix A for a discussion of general regularity properties of stochastic processes. 

**Lemma 3.2** _The process B_<sup>_η,N_</sup> _satisfies Condition A_ .1 _stated in Appendix A on_ [0 _, T_ ] _uniformly in N_ ∈ _N and η_ ∈ _(_ 0 _, H_ ], _for all H_ ∈ _(_ 0 _,_ ∞ _)_ . 

**_Proof_** Let _ε, ε_ 1 _>_ 0, _s > T_ − _ε_ 1 and let _V_ and _τ_ be arbitrary according to the definition of Condition A.1. By Lemma 3.1, if _ε_ 1 is small enough, 



If _s_ ≤ _T_ − _ε_ 1, the assertion follows from the integral representation (2.10) along with the estimates established in Lemma 3.1 by using that the stochastic integral in (2.10) is a martingale on [0 _, T_ − _ε_ 1]. We emphasise that _Z_<sup>_η,N,B_</sup> is possibly defined only on [0 _, T_ −], and so the stochastic integral may be a martingale only away from the terminal time. □ 

### **3.1.2 The case** **_η_ = 0** 

We are now going to establish a priori estimates on the candidate limiting coefficient processes. First, we show that Assumption 2.3 directly implies the following regularity result for the parameter processes. 

**Lemma 3.3** _The processes χ_ , _ρ_ , _λ_ , _ϕ and ϕ_<sup>−1</sup> _satisfy Condition A_ .2 _introduced in Appendix A_ . 

**_Proof_** The process _χ_ satisfies Condition A.2 due to Lemmas A.11, A.4 and A.7. The rest immediately follows by Lemma A.10. □ 

We are now ready to prove that the process _B_<sup>0</sup> is well defined. 

**_Proof_** _of Lemma 2.5_ The existence result follows from a standard argument. In fact, it is well known that for any _b_ ∈[1 _,_ ∞ _)_ , the BSDE 



U. Horst, E. Kivman 

778 

with Lipschitz-continuous driver (we recall _f_<sup>_λ_</sup> , _f_<sup>_ρ_</sup> defined in Assumption 2.3) 



has a unique solution _(Bb, Zb)_ ∈ _L_ 2 _P_<sup>_(�_× [0</sup><sup>_, T_]; R × R</sup><sup>_m)_(cf. Theorem A.13). Let</sup> us then define the functions _<u>φ,</u> φ_ : _�_ × [0 _, T_ ] × R × R<sup>_m_</sup> → R by 



By definition, _<u>(B</u>_ 0 _,_ 0 _)_ is the unique solution to the BSDE with driver _<u>φ</u>_ and terminal condition 1, where the lower bound _<u>B</u>_ 0 on the process _B_ 0 was defined in (2.8). Likewise, _(_ 1 _,_ 0 _)_ is the unique solution of the BSDE with driver _φ_ and the same terminal condition. Since 



the standard comparison principle for BSDEs with Lipschitz-continuous drivers yields 



This proves that _(B_<sup>1</sup> _, Z_<sup>1</sup> _)_ is the desired unique bounded solution to the BSDE (2.7). The second assertion follows Theorem A.13 applied to the BSDE (3.1) for _b_ = 1. □ 

Having established the existence of the process _B_<sup>0</sup> , the processes _D_<sup>0</sup> and _E_<sup>0</sup> are well defined. The following lemma establishes estimates and regularity properties for _D_<sup>0</sup> and _E_<sup>0</sup> . 

**Lemma 3.4** _We have the a priori estimates_ 



_Moreover_ , _the processes D_<sup>0</sup> _and E_<sup>0</sup> _satisfy Condition A_ .2. 

**_Proof_** The a priori estimates can be obtained by plugging the bounds on _B_<sup>0</sup> from Lemma 2.5 into the definitions of _D_<sup>0</sup> and _E_<sup>0</sup> given in (2.9). Moreover, if we denote by _h_ the function derived from Lemma 2.5, then for all _t_ ∈[0 _, T_ ], 



Optimal trade execution under small market impact 

779 

In view of Assumption 2.3, the processes _D_<sup>0</sup> and _E_<sup>0</sup> can be represented as uniformly continuous functions of the factor process _χ_ , and hence the assertion follows from Lemmas 3.3 and A.10. □ 

The next lemma can be viewed as the analogue to Theorem 2.2 in the case _η_ = 0. 

**Lemma 3.5** _It holds_ 



**_Proof_** Due to Lemma 3.4, _D_<sup>0</sup> is positive and hence, for _t_ ∈ _(_ 0 _, T )_ , 



Since _D_<sup>0</sup> + _γ E_<sup>0</sup> = _ϕ_ on [0 _, T )_ , it thus follows from Lemma 3.4 that for all _t_ ∈ _(_ 0 _, T )_ , 



### **3.2 Proof of the convergence results** 

In this section, we prove our main convergence results. We start with the convergence of the coefficients of the ODE system (2.3). Subsequently, we prove that the convergence of the coefficients yields convergence of the state process. 

### **3.2.1 Proof of Proposition 2.6** 

The proof of Proposition 2.6 is split into a series of lemmas. In the first step, we establish the convergence, as _η_ → 0, of the auxiliary processes 



and 



On [0 _, T )_ , the processes _F_<sup>_η,N_</sup> and _G_<sup>_η,N_</sup> satisfy the dynamics 



U. Horst, E. Kivman 

780 

A general convergence result for integral equations of the above form is established in Appendix B.1.2. It allows us to prove the following two results. 

**Lemma 3.6** _Let f_<sup>_η,N_</sup> := _F_<sup>_η,N_</sup> − _ϕ_ . _For all ε >_ 0, _there exists an η_ 0 _>_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 0] _and all N_ ∈ _N_ , 



**_Proof_** For every _N_ ∈ _N_ , we apply Lemma B.4 to _P_<sup>_η_</sup> := _F_<sup>_η,N_</sup> with 



noticing that _ε_ �→ _η_ 0 = _η_ 0 _(ε)_ is independent of _N_ . Assumption B.3 i) is satisfied with _P_ := _<u>ϕ</u>_ + 3 _<u>κ</u>_ due to Lemma 3.1. If _N_ = ∞, then Assumption B.3 ii) a) is satisfied due to Theorem 2.1 and the a priori estimates derived in Lemmas 3.1 and 3.4. If _N <_ ∞ and _η_ ≤ _<u>ϕ</u>_<sup>~~−~~2</sup> , then _ET_<sup>_η,N_</sup> = 0 and 



which shows Assumption B.3 ii) b). Assumption B.3 iii) is trivially satisfied because _q_<sup>_η_</sup> = 0. Assumption B.3 iv) follows by direct computation using _<u>ε</u>_ := _<u>ϕ/</u>_ 2 and _β_ := _<u>ϕ/</u>_ 2. Assumption B.3 v) follows from Lemmas 3.3 and 3.1. □ 

It is not difficult to show that similar arguments as those used to prove the convergence of _f_<sup>_η,N_</sup> can be applied to − _g_<sup>_η,N_</sup> := − _G_<sup>_η,N_</sup> + 2 _ρ_ . As a result, the intervals [0 _, T )_ and [0 _, T_ − _ε_ ] in the statement of the convergence result for _g_<sup>_η,N_</sup> need to be swapped. We omit the proof of the corresponding next result. 

**Lemma 3.7** _For all ε >_ 0, _there exists an η_ 0 _>_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 0] _and all N_ ∈ _N_ , 



We are now going to prove the almost sure convergence to zero of the process _b_<sup>_η,N_</sup> := _B_<sup>_η,N_</sup> − _B_<sup>0</sup> . To this end, we first observe that 



yields 



Optimal trade execution under small market impact 

781 

Plugging this into (2.10) shows that 



on [0 _, T )_ . Performing an analogous computation for _B_<sup>0</sup> and subtracting the two equations yields 



on [0 _, T )_ . Using that 



shows that 



on [0 _, T )_ . This BSDE is different from (3.2) and (3.3). We apply Lemma B.2 to prove the following result. 

**Lemma 3.8** _For all ε >_ 0, _there exists an η_ 0 _>_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 0] _and all N_ ∈ _N_ , 



**_Proof_** For every _N_ ∈ _N_ , we apply Lemma B.2 with 



Assumption B.1 i) follows from the a priori estimates on _B_<sup>_η,N_</sup> (Lemma 3.1), _B_<sup>0</sup> (Lemma 2.5) and _D_<sup>0</sup> (Lemma 3.4). Assumption B.1 ii) follows from the a priori 

U. Horst, E. Kivman 

782 

estimates on _B_<sup>_η,N_</sup> and _B_<sup>0</sup> , where the mapping _ε_ �→ _δ_ is independent of _N_ . Assumption B.1 iii) follows from the same estimates and Lemmas 3.6 and 3.7, where the choice of _η_ 1 is independent of _N_ . Assumption B.1 iv) is satisfied because we can choose _ε_ 1 _>_ 0 small enough such that 



### **3.2.2 Proof of Theorem 2.7** 

First we need to prove an auxiliary result: 

**Lemma 3.9** _Let I_ ⊆ R _be a compact interval and Y_ = _(Yt )t_ ∈ _I a continuous_ , _adapted_ , R<sup>_d_</sup> _-valued stochastic process_ . _Then the modulus of continuity_ 



_converges to_ 0 _in probability as ν_ → 0. 

**_Proof_** For all _ω_ ∈ _�_ , _Y(ω)_ is uniformly continuous; hence the modulus of continuity converges to 0 as _ν_ → 0 P-a.s. This implies convergence in probability. □ 

**_Proof_** _of Theorem 2.7_ Convergence on compact subintervals of [0 _, T )_ follows from Theorem B.8 applied to the ODEs 

and 



on [0 _, T )_ . In fact, Condition (B.10) follows from Proposition 2.6 and Condition (B.11) from Theorem 2.2. Finally, _D_<sup>0</sup> _>_ 0 (see Lemma 3.4), from which we de- <u>0</u> duce that _x_ 0 _>_<sup>_E_</sup> _ϕ_ 0<sup>0</sup><sup>_Z_ˆ</sup> 0<sup>0.ThusbyTheoremB.8,appliedonceforevery</sup><sup>_N_∈</sup><sup>_N_and</sup> every _ν >_ 0, there exists for all _ν, δ_ ∈ _(_ 0 _,_ ∞ _)_ some _η_ 1 = _η_ 1 _(ν, δ)_ ∈ _(_ 0 _,_ ∞ _)_ such that for all _η_ ∈ _(_ 0 _, η_ 1] and all _N_ ∈ _N_ , the set 



satisfies 



Optimal trade execution under small market impact 

783 

Near the terminal time, we cannot use this theorem since the convergence<sup>_<u>E</u>_</sup> _F_<sup>_η,Nη,N_→</sup><sup>_<u>E</u>_</sup> _ϕ_<sup>0</sup> holds only on [0 _, T_ −]. Instead, we apply increment bounds on intervals of the form [ _T_ − _ν, T_ ] to prove that for all _η >_ 0, _N_ ∈ _N_ and for all sufficiently small 



the set 



contains the set _M_ 0<sup>_ν,η,N_</sup> ∩ _M_ 2<sup>_β,ν_</sup> ∩ _M_ 3<sup>_ν,η,N_</sup> , where 

The probability of the last two events can be made large by Lemma 3.9 and Proposition 2.6: for all _β, δ, ν_ ∈ _(_ 0 _,_ ∞ _)_ , there exist _ν_ 0 _(β, δ), η_ 2 _(ν)_ ∈ _(_ 0 _,_ ∞ _)_ such that 



In order to see that _M_ 1<sup>_ν,ε,η,N_</sup> ⊇ _M_ 0<sup>_ν,η,N_</sup> ∩ _M_ 2<sup>_β,ν_</sup> ∩ _M_ 3<sup>_ν,η,N_</sup> , we assume to the contrary that there exists 



To obtain the desired contradiction, we show that for any such _ω_ , there exists a time _s(ω)_ ∈[ _T_ − _ν, T )_ for which we can deduce both nonnegativity and strict negativity of _∂t X_<sup>ˆ</sup><sup>_η,N_</sup> _s(ω)_<sup>_(ω)_simultaneously.</sup> 

We start with the choice of _s(ω)_ . Because we have _ω_ ∈ _/ M_ 1<sup>_ν,ε,η,N_</sup> , there exists some _t_ ∈[ _T_ − _ν, T )_ with _X_<sup>ˆ</sup> _t_<sup>_η,N_</sup> _(ω)_ − _X_<sup>ˆ</sup> _t_<sup>0</sup><sup>_(ω) > ε_. Since</sup><sup>_ω_∈</sup><sup>_M_</sup> 2<sup>_β,ν_</sup> and due to (3.6), this yields that 



Since _ω_ belongs to _M_ 0<sup>_ν,η,N_</sup> and due to (3.6), this implies that 



Now we can choose _s(ω)_ ∈ _(T_ − _ν, t)_ minimal with the property that 



Due to the minimality of _s(ω)_ , we have _∂t X_<sup>ˆ</sup> _s(ω)_<sup>_η,N(ω)_≥0.</sup> 

U. Horst, E. Kivman 

784 

We now show that this derivative must also be strictly negative. In fact, due to (3.4), 



Since _Z_<sup>ˆ</sup><sup>_η,N_</sup> is nonincreasing (Theorem 2.2), using (3.7) again, the right-hand side of the above equation can be bounded from above by 



Since _ω_ ∈ _M_ 3<sup>_ν,η,N_</sup> , we have _Fs(ω)_<sup>_η,N(ω)_=</sup><sup>_ϕs(ω)(ω)_+</sup><sup>_f η,N_</sup> _s(ω)_<sup>_(ω)_≥</sup><sup>_<u>ϕ</u>_</sup> − _ν_ , which is strictly positive if _ν <_ _<u>ϕ</u>_ . Moreover, we have − _ϕX_<sup>ˆ0</sup> + _E_<sup>0 ˆ</sup> _Z_<sup>0</sup> = 0 on [0 _, T )_ . Because also _ω_ ∈ _M_ 0<sup>_ν,η,N_</sup> ∩ _M_ 2<sup>_β,ν_</sup> ∩ _M_ 3<sup>_ν,η,N_</sup> and _X_<sup>ˆ0</sup> , _E_<sup>0</sup> and _Z_<sup>ˆ</sup><sup>_η,N_</sup> are bounded, this shows that _∂t X_<sup>ˆ</sup> _s(ω)_<sup>_η,N(ω) <_0 if</sup><sup>_ν_and</sup><sup>_β_are chosen small enough. The convergence of</sup><sup>_Y_ˆ</sup><sup>_η,N_−ˆ</sup><sup>_Y_0</sup> on [0 _, T_ −] follows from 



and (3.5). The convergence on [ _T_ − _, T_ ] follows from the fact that 



can be made arbitrarily small by choosing _ν_ small since _∂t Z_<sup>ˆ</sup><sup>_η,N_</sup> = _ρY_<sup>ˆ</sup><sup>_η,N_</sup> , _∂t Z_<sup>ˆ0</sup> = _ρY_<sup>ˆ0</sup> and both _Y_<sup>ˆ</sup><sup>_η,N_</sup> and _Y_<sup>ˆ0</sup> are bounded. □ 

### **3.2.3 Proof of Proposition 2.8** 

The proof of the convergence of the optimal portfolio processes in the Skorohod _M_ 2-sense follows from Theorem 2.7 if strict liquidation is required. If strict liquidation is not required, the results of Sect. 2.2 are required to establish the assertion. This is not a circular argument since the proofs of Sect. 2.2 only use the results of Sect. 2.1 concerning the liquidating case _N_ = ∞. 

Since _θ_<sup>ˆ</sup> = _(_ 0 _, j_<sup>ˆ−</sup> _, V )_<sup>ˆ</sup> is optimal in the model introduced in Sect. 2.2 (cf. Theorem 2.15), it is easy to show that the strategy _θ_<sup>ˆ</sup><sup>_q_</sup> := _(j_<sup>ˆ+</sup><sup>_,q_</sup> _, j_<sup>ˆ−</sup><sup>_,q_</sup> _, V_<sup>ˆ</sup><sup>_q_</sup> _)_ is admissible for every _q_ ∈ R, where 



The following lemma shows that we can express the cost term corresponding to the transient price impact without Itô integrals. The proof is an immediate consequence 

Optimal trade execution under small market impact 

785 

of Itô’s formula for semimartingales (see Protter [32, Theorem II.32]) and the fact that _γ_<sup>2</sup> [ _V_ ] is equal to the continuous part of [ _Y_<sup>_θ_</sup> ]. We omit it. 

**Lemma 3.10** _For all θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> _and all t_ ∈[0 _, T_ ], _it holds that_ 



Using Lemma 3.10, it is not difficult to check that the mapping _q_ �→ _J_<sup>0</sup> _(θ_<sup>ˆ</sup><sup>_q_</sup> _)_ is differentiable and that 



This allows us to prove that full liquidation is optimal if _η_ → 0 even if it is not formally required. 

**Lemma 3.11** _We have that_ lim _η_ →0 sup _N_ ∈ _N_ E[ _X_<sup>ˆ</sup> _T_<sup>_η,N_</sup> ] = 0. 

**_Proof_** We assume to the contrary that lim sup _η_ →0 sup _N_ ∈ _N_ E[ _X_<sup>ˆ</sup> _T_<sup>_η,N_</sup> ] _>_ 0 and prove that this contradicts the optimality of _J_<sup>_η,N_</sup> _(ξ_<sup>ˆ</sup><sup>_η,N_</sup> _)_ . To this end, we consider the admissible trading strategies 



compute the derivative of the function _q_ �→ _J_<sup>_η,N_</sup> _(ξ_<sup>ˆ</sup><sup>_η,N,q_</sup> _)_ and show that the derivative at _q_ = 0 does not vanish for small _η_ if 



Obviously, _ξ_<sup>ˆ</sup><sup>_η,N,q_</sup> ∈ _A_ and 



U. Horst, E. Kivman 

786 

Straightforward computations lead to 



Subtracting (3.8) yields 

In view of Theorems 2.2 and 2.7, Lemma 3.5 and using 



the sum of the three last expected values is small uniformly in _N_ if _η_ is small. Hence, if lim sup _η_ →0 sup _N_ ∈ _N_ E[ _X_<sup>ˆ</sup> _T_<sup>_η,N_</sup> ] _>_ 0, then the sum on the right-hand side of the above inequality is strictly positive when first choosing _η_ 0 _>_ 0 small enough and then choosing _(η,_ ˜ _N)_<sup>˜</sup> with _η_ ˜ ≤ _η_ 0 such that 



We are now ready to prove the convergence of _X_<sup>ˆ</sup><sup>_η,N_</sup> to _X_<sup>ˆ0</sup> in the Skorohod _M_ 2-sense. To this end, we have to bound the distance of each point of any of the graphs to the other graph. In the inner interval [ _ε, T_ − _ε_ ], it is enough to consider _X_ ˆ<sup>_η,N_</sup> −ˆ _X_<sup>0</sup> , which we have bounded by Theorem 2.7. 

**_Proof_** _of Proposition 2.8_ To prove that the probability of 



Optimal trade execution under small market impact 

787 

is large for small _η >_ 0, we need to prove that the distance of any point _(t, x)_ from either _GX_ ˆ _η,N (ω)_ or _GX_ ˆ 0 _(ω)_ to the respective other graph is small on a set of large probability. To this end, we fix a small enough _ν_ ∈ _(_ 0 _, ε)_ . If _t_ ∈[ _ν, T_ − _ν_ ], the claim follows directly from Theorem 2.7. For _t_ ∈[0 _, ν)_ ∪ _(T_ − _ν, T_ ], we use Theorem 2.7 along with the facts that (i) the completed graph of a discontinuous function contains the line segments joining the values of the function at the points of discontinuity, (ii) the increments of _X_<sup>ˆ0</sup> are small in the sense of Lemma 3.9, and (iii) lim _η_ →0 sup _N_ ∈ _N_ E[ _X_<sup>ˆ</sup> _T_<sup>_η,N_</sup> ] = 0 by Lemma 3.11. For instance, consider an _ω_ ∈ _�_ with 



and assume that _(t, x)_ ∈ _GX_ ˆ _η,N (ω)_ with _t < ν_ and _x < X_ ˆ 0<sup>0</sup><sup>_(ω)_−</sup><sup>_ν_.Since</sup> _x_ = _X_<sup>ˆ</sup> _t_<sup>_η,N_</sup> _(ω)_ , the mean value theorem yields an _s_ ∈[0 _, t_ ] such that _X_<sup>ˆ</sup> _s_<sup>0</sup><sup>_(ω)_=</sup><sup>_x_+</sup><sup>_ν_,</sup> which proves that _(s, x_ + _ν)_ ∈ _GX_ ˆ 0 _(ω)_ and 



## **4 Proofs for Sect. 2.2** 

### **4.1 Proof of Lemma 2.11** 

In order to prove Lemma 2.11, we first define, for all _β, ν_ ∈ _(_ 0 _,_ ∞ _)_ and _x_ ∈ R, 



For any admissible strategy _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> , there exists a unique pathwise differentiable, adapted stochastic process _(V_<sup>˜</sup> _t_<sup>_β,ν_</sup> _)t_ ∈[0 _,T_ ] such that _V_<sup>˜</sup> 0<sup>_β,ν_</sup> = 0 and its time derivative satisfies 



Now (2.13) can easily be verified by the comparison principle: For all _ε >_ 0 and all _t_ ∈[0 _, T_ ], we have 



and hence, for all _ε >_ 0 and _t_ ∈[0 _, T_ ], 

Analogously, we can prove that _V_<sup>˜</sup> _t_<sup>_β,ν_</sup> ≥− max _s_ ∈[0 _,T_ ] | _Vs_ | − _ε_ . 

U. Horst, E. Kivman 

788 

Now we are going to prove (2.14). For _β, ν_ ∈ _(_ 0 _,_ ∞ _)_ , let 



In view of Lemma 3.9, it is enough to prove that 



for all _ω_ ∈ _M_ and _t_ ∈[0 _, T_ ]. In order to see this, let us assume to the contrary that the statement is wrong. Then by continuity, since _V_ 0 − _V_<sup>˜</sup> 0<sup>_β,ν_</sup> = 0, there exist some _ω_ ∈ _M_ and _t_ 1 ∈[0 _, T_ ] such that 



We choose the smallest such _t_ 1 ∈[0 _, T_ ]. Then for each _t_ ∈[ _(t_ 1 − _ν)_<sup>+</sup> _, t_ 1], 



Since _V_ 0 − _V_<sup>˜</sup> 0<sup>_β,ν_</sup> = 0, we have _t_ 1 _> ν_ . Since _V_ · _(ω)_ − _V_<sup>˜</sup> ·<sup>_β,ν_</sup> _(ω)_ is continuous and has no roots in [ _t_ 1 − _ν, t_ 1], it does not change sign on this interval. We may hence without loss of generality assume that _Vt_ 1 _(ω)_ − _V_<sup>˜</sup> _t_<sup>_β,ν_</sup> 1 _(ω)_ = 3 _β_ . This implies that 



for all _t_ ∈[ _t_ 1 − _ν, t_ 1]. By definition, for all those _t_ , we have _V_<sup>˙</sup> _t_<sup>_β,ν_</sup> _(ω)_ = _β/ν_ . This, however, contradicts the minimality of _t_ 1 as _V_<sup>˙</sup> _s_<sup>_β,ν_</sup> = _β/ν_ and 



This finishes the proof of Lemma 2.11. 



### **4.2 Proof of Lemma 2.12** 

The proof of Lemma 2.12 requires the following result on the jump processes. 

**Lemma 4.1** _Let (jt )t_ ∈[0 _,T_ ] _be a_ [0 _,_ ∞ _)-valued_ , _nondecreasing and progressively measurable stochastic process with_ E[ _(jT )_<sup>2</sup> ] _<_ ∞. _Then_ ( _putting jt_ = 0 _for t <_ 0) 



Optimal trade execution under small market impact 

789 

**_Proof_** First we want to bound �0 _T_<sup>_(jt_−</sup><sup>_jt_−</sup><sup>_ε)_2 d</sup><sup>_t_throughatermthatdependson</sup> �0 _T_<sup>_(jt_−</sup><sup>_jt_−2</sup><sup>_ε)_2 d</sup><sup>_t_and apply this bound inductively. For every</sup><sup>_ε>_0, we have</sup> 



Moreover, _jt_ ≤ _jT_ and _jt_ − _ε_ ≥ _jT_ − _ε_ − _ε_ for _t_ ∈[ _T_ − _ε, T_ ] give 



As a result, 



Hence, inductively, for all _ε_ ∈ _(_ 0 _, T )_ and _k_ ∈{1 _,_ 2 _, . . ._ }, we have 





Hence for all _k_ ∈ N, 



This shows the desired result. 

We are now ready to prove the approximation of arbitrary portfolio processes by absolutely continuous ones. 

U. Horst, E. Kivman 

790 

**_Proof_** _of Lemma 2.12_ By the triangle inequality, for all _β, ν, ε >_ 0, 



We analyse the four terms separately. The first can be bounded by 



Regarding the second term, for all _ε < T /_ 2, 



Optimal trade execution under small market impact 

791 

Using the monotonicity of the jump processes, it follows from Lemma 4.1 that 



For the third term, we conclude from the Itô isometry and the definition of _ξ_<sup>_θ,β,ν,ε_</sup> that 



Now we can bound 



U. Horst, E. Kivman 

792 

Moreover, in view of (4.2), 



It remains to consider the fourth term in (4.1). To this end, let 



Then using (2.13) in the last step, 



By (2.12), the single random variable max _t_ ∈[0 _,T_ ] | _Vt_ |<sup>2</sup> is in _L_<sup>1</sup> and hence uniformly integrable. So we can first choose _β >_ 0 small enough, then, according to Lemma 2.11, choose _ν >_ 0 small enough such that P[ _�_ \ _M_<sup>_β,ν_</sup> ] is sufficiently small, and finally choose _ε >_ 0 small enough in order to obtain the desired result. □ 

### **4.3 Proof of Lemma 2.13** 

We start with a technical lemma. 

Optimal trade execution under small market impact 

793 

**Lemma 4.2** _Let (M, A, μ) be a measure space and u, v_ ∈ _L_<sup>2</sup> _(M)_ . _Then_ 



**_Proof_** Due to the Hölder and triangle inequalities, 



The following technical lemma provides useful estimates for the impact process. 

**Lemma 4.3** _Let X_ ∈ _L_<sup>2</sup> _P_<sup>_(�_× [0</sup><sup>_, T_]; R</sup><sup>_)_.</sup><sup>_Then the transient price impact process Y X_</sup> _given by_ (2.1) _satisfies Y_<sup>_X_</sup> ∈ _L_<sup>2</sup> _P_<sup>_(�_× [0</sup><sup>_, T_]; R</sup><sup>_) and_</sup> 



_If X_ 0 = 0, _then we additionally have for all s, t_ ∈[0 _, T_ ] _with s < t that_ 



**_Proof_** Inequality (4.3) follows from the explicit formula 



U. Horst, E. Kivman 

794 

and the triangle inequality. Moreover, 



Substituting the inequality 



into (4.7) yields (4.4). To prove (4.5), let _X_ 0 = 0. Then due to (4.6), 



for all _u_ ∈[0 _, T_ ] and so 



Using the subadditivity of the square root, we now obtain (4.5) from 



Optimal trade execution under small market impact 

795 

We are now ready to prove our approximation result for the cost functional. 

**_Proof_** _of Lemma 2.13_ For _θ_ = _(j_<sup>+</sup> _, j_<sup>−</sup> _, V )_ ∈ _A_<sup>0</sup> and _ξ_ ∈ _A_ , 



Due to Lemma 4.2, the last term can be estimated as 



Moreover, using first Lemma 3.10 and then Lemmas 4.3 and 4.2, we obtain 

U. Horst, E. Kivman 

796 

### **4.4 Proof of Lemma 2.14** 

We assume the contrary, i.e., that there exists a constant _c >_ 0 such that for all _H >_ 0, there exists some _η_ ∈ _(_ 0 _, H)_ with 



The optimality of _ξ_<sup>ˆ</sup><sup>_η,_∞</sup> and (2.15) imply that for all _η, ν, β, ε >_ 0, 



We now prove that (4.8) contradicts (4.9). By Theorem 2.13 and since | _X_<sup>ˆ</sup> _t_<sup>0| ≤</sup><sup>_x_0, we</sup> obtain (for convenience, let _p(x)_ := _x_ +<sup>~~√~~</sup> _<u>x</u>_ ) 



and 



Plugging the results into (4.9) yields that for all _η >_ 0 that satisfy (4.8), we have 



Due to Lemma 2.12, we can first choose _β, ν, ε >_ 0 sufficiently small to get 



Since | _X_<sup>ˆ</sup> _t_<sup>_η,_∞</sup> | ≤ _x_ 0 (cf. Theorem 2.2) and in view of Theorem 2.7, we can then choose _η >_ 0 sufficiently small and satisfying (4.8) such that the right-hand side of (4.10) is larger than zero, which is a contradiction. This finishes the proof of Lemma 2.14. 

Optimal trade execution under small market impact 

797 

## **Appendix A: Regularity properties of Itô processes and BSDEs** 

In this appendix, we introduce some regularity properties of stochastic processes, which we use to prove various convergence results for stochastic processes. 

We consider a continuous, adapted, R<sup>_d_</sup> -valued stochastic process _Y_ = _(Yt )t_ ∈ _I_ on some interval _I_ (i.e., a convex subset of R) and introduce the following continuity conditions. 

**Condition A.1** For all _ε_ ∈ _(_ 0 _,_ ∞ _)_ , there exists _δ_ ∈ _(_ 0 _,_ ∞ _)_ such that for all _s_ ∈ _I_ , _Fs_ -measurable and integrable _V_ : _�_ → R and stopping times _τ_ : _�_ →[ _s, s_ + _δ_ ] ∩ _I_ , 



**Condition A.2** For all _ε_ ∈ _(_ 0 _,_ ∞ _)_ , there exists _δ_ ∈ _(_ 0 _,_ ∞ _)_ such that for all _s_ ∈ _I_ , 



**Definition A.3** A family of stochastic processes is said to _uniformly_ satisfy Condition A.1 or A.2 on _I_ if all processes satisfy the respective property and _δ_ can be chosen uniformly for all processes. 

In what follows, we list some auxiliary results. 

**Lemma A.4** _An_ R<sup>_d_</sup> _-valued stochastic process Y_ = _((Yt_<sup>1</sup><sup>_, . . . , Y_</sup> _t_<sup>_d))t_∈</sup><sup>_Isatisfies Con-_</sup> _dition A_ .1 ( _resp_ . _Condition A_ .2) _if and only if all components Y_<sup>_i_</sup> _satisfy Condition A_ .1 ( _resp_ . _Condition A_ .2). 

**Lemma A.5** _Condition A_ .2 _implies Condition A_ .1. 

**_Proof_** Due to Lemma A.4, it is enough to consider a real-valued _Y_ . To show the assertion, we then calculate 



**_Remark A.6_** If we want to bound a term of the form |E[1 _N (Yτ_ − _Ys)_ ]| for some _N_ ⊆ _M_ ∈ _Fs_ with _N_ ∈ _/ Fs_ , then Condition A.1 is not enough; in this case, we need the stronger Condition A.2. 

**Lemma A.7** _Let X_ = _(Xt )t_ ∈ _I and Y_ = _(Yt )t_ ∈ _I be continuous_ , _adapted_ , _real-valued processes which satisfy Condition A_ .1 ( _resp_ . _Condition A_ .2). _Then X_ + _Y satisfies Condition A_ .1 ( _resp_ . _Condition A_ .2). 

**Theorem A.8** _Let X_ = _(Xt )t_ ∈ _I and Y_ = _(Yt )t_ ∈ _I be continuous_ , _adapted_ , _essentially bounded_ , _real-valued processes_ . _Then_ : 

- _If X satisfies Condition A_ .2 _and Y satisfies Condition A_ .1, _then XY satisfies_ 

- _Condition A_ .1. 

   - _If X and Y both satisfy Condition A_ .2, _then XY also satisfies Condition A_ .2. 

U. Horst, E. Kivman 

798 

**_Proof_** The second part of the statement can be proved directly via 



The first part of the statement follows from 



Next, we prove some properties of the concave envelope of the modulus of continuity of a uniformly continuous function. We use these results to show that a uniformly continuous function of an Itô process with bounded coefficients satisfies Condition A.2. 

**Lemma A.9** _Let (X,_ ∥·∥ _X) be a normed space with a nonempty and convex subset D_ , _let (Y, dY ) be a metric space_ , _let f_ : _D_ → _Y and for every t_ ∈[0 _,_ ∞ _)_ , _let_ 





_Then ω_ ˜ _f_ : [0 _,_ ∞ _)_ →[0 _,_ ∞] _is nondecreasing and satisfies ω_ ˜ _f (_ 0 _)_ = 0, _and for all s, t_ ∈[0 _,_ ∞ _) with s >_ 0, _we have_ 



_If ω_ ˜ _f is finite-valued_ , _then ωf takes values in_ [0 _,_ ∞ _)_ , _is concave_ , _nondecreasing and for all x_ 1 _, x_ 2 ∈ _D_ , 



_If f is uniformly continuous_ , _then ω_ ˜ _f is finite-valued_ , _ω_ ˜ _f and ωf are continuous in_ 0 _and ωf (_ 0 _)_ = 0. 

**_Proof_** By definition, _ω_ ˜ _f_ is nondecreasing and satisfies _ω_ ˜ _f (_ 0 _)_ = 0. To prove (A.1), let _s, t_ ∈[0 _,_ ∞ _)_ with _s >_ 0 and _x_ 1 _, x_ 2 ∈ _D_ with ∥ _x_ 1 − _x_ 2∥ _X_ ≤ _t_ . Let _N_ := ⌈ _t/s_ ⌉ and 



Then _x_<sup>_k_</sup> ∈ _D_ because _D_ is convex and ∥ _x_<sup>_k_</sup> − _x_<sup>_k_+1</sup> ∥ _X_ = _N_<sup><u>1</u>∥</sup><sup>_x_1 −</sup><sup>_x_2∥</sup><sup>_X_≤</sup><sup>_s_. Hence</sup> 



Optimal trade execution under small market impact 

799 

Having established (A.1), we know that if _ω_ ˜ _f_ is finite-valued, then it is bounded above by an affine (hence concave) function. In particular, the concave envelope is well defined and satisfies (A.2). Concavity and monotonicity of _ωf_ are clear. Finally, uniform continuity of _f_ implies that _ω_ ˜ _f_ is finite-valued and continuous in 0. In order to see _ωf (_ 0 _)_ = 0 and continuity of _ωf_ in 0, let _ε >_ 0 be arbitrary and let _δ >_ 0 be such that _ω_ ˜ _f (δ)_ ≤ _ε/_ 2. Due to (A.1), _ω_ ˜ _f_ and hence _ωf_ is bounded above by the affine and hence concave function _t_ �→ _((t_ + _δ)/δ)(ε/_ 2 _)_ . In particular, for all _t_ ≤ _δ_ , we obtain _ωf (t)_ ≤ _ε_ . □ 

**Lemma A.10** _Let X_ = _(Xt )t_ ∈ _I be a continuous_ , _adapted_ , R<sup>_d_</sup> _-valued process which satisfies Condition A_ .2 _and let f_ : [0 _, T_ ] × R<sup>_d_</sup> → R _be uniformly continuous_ . _Then (f (t, Xt ))t_ ∈[0 _,T_ ] _also satisfies Condition A_ .2. 

**_Proof_** There exists a function _ωf_ with the properties listed by Lemma A.9 (with the max-norm | · |∞ on [0 _, T_ ] × R<sup>_d_</sup> ). Now let _δ >_ 0 be arbitrary. Using (A.2) and the concavity and monotonicity of _ωf_ , we obtain that 



Since _ωf_ is continuous in 0 and _X_ satisfies Condition A.2, we can choose _δ >_ 0 small enough such that this term is not greater than _ε_ . □ 

**Lemma A.11** _Let (σ_ ˜ _t )t_ ∈[0 _,T_ ] ∈ _L_<sup>∞</sup> _P_<sup>_(�_×[0</sup><sup>_, T_]; R</sup><sup>_m)_.</sup><sup>_Then (_</sup> �0 _t_<sup>_σ_˜</sup><sup>_u_d</sup><sup>_Wu)t_∈[0</sup><sup>_,T_]</sup><sup>_satisfies_</sup> _Condition A_ .2. 

**_Proof_** Let _<u>σ</u> >_ 0 be a componentwise bound on _σ_ ˜ . By Jensen’s inequality, 



In order to prove that the conditional expectation on the right-hand side is bounded, we apply the classical Doob maximal inequality for the conditional measures with respect to all sets _A_ ∈ _Fs_ with positive probability and obtain 



U. Horst, E. Kivman 

800 

Then by combining these inequalities and using the Itô isometry for conditional expectations, we finally obtain 



**_Remark A.12_** Obviously, we have the same result for the drift part of an Itô process, i.e., if _μ_ ˜ ∈ _L_<sup>∞</sup> Prog<sup>_(�_× [0</sup><sup>_, T_]; R</sup><sup>_m)_,then</sup><sup>_(_</sup> �0 _t_<sup>_μ_˜</sup><sup>_u_d</sup><sup>_u)t_∈[0</sup><sup>_,T_]satisfiesConditionA.2.</sup> However, we cannot expect the weaker Condition A.1 under weaker integrability assumptions, as the following example shows. Take _T_ = 2, _μ_ ˜ _u_ := 1[1 _,_ 2] _(u)_ | _W_ 1|, _ε_ := 1, _δ_ ∈ _(_ 0 _,_ 1 _)_ arbitrary and _s_ := 1, _τ_ := 1 + _δ_ , _V_ := 1{ _δ_ | _W_ 1| _>_ 1}. Then 



Finally, we prove that the strong Condition A.2 holds for a certain class of BSDEs driven by forward SDEs. Specifically, we prove that the solution to the BSDE can be expressed as a _uniformly continuous_ function of the forward process and then we apply Lemma A.10. The representation of the solution in terms of a _continuous_ function has already been proved by El Karoui [12]. 

For all _(t, x)_ ∈[0 _, T_ ] × R<sup>_n_</sup> , we consider on [ _t, T_ ] the SDE 



where _μ,_ ˜ _σ_ ˜ : [0 _, T_ ] × R<sup>_n_</sup> → R<sup>_n_</sup> × R<sup>_n_×</sup><sup>_m_</sup> are measurable and bounded and satisfy the Lipschitz condition 



By the previous results, we obtain that _X_<sup>_t,x_</sup> satisfies Condition A.2. Standard computations show that there exists a constant _C_ ∈ _(_ 0 _,_ ∞ _)_ such that for all _t_ ∈[0 _, T_ ] and _x_ 1 _, x_ 2 ∈ R<sup>_n_</sup> , 



**Theorem A.13** _Let �_ : R<sup>_n_</sup> → R _be bounded_ , _ψ_ : [0 _, T_ ] × R<sup>_n_</sup> × R → R _continuous and bounded and let both functions satisfy the Lipschitz condition_ 



_For (t, x)_ ∈[0 _, T_ ] × R<sup>_n_</sup> , _let (Y_<sup>_t,x_</sup> _, Z_<sup>_t,x_</sup> _)_ ∈ _L_<sup>2</sup> _P_<sup>_(�_× [0</sup><sup>_, T_]; R × R</sup><sup>_m) be the unique_</sup> _solution to the BSDE on_ [ _t, T_ ] _given by_ 



Optimal trade execution under small market impact 

801 

_driven by the forward process X_<sup>_t,x_</sup> ( _Y_<sup>_t,x_</sup> _exists due to_ [12, _Theorem_ 2.1]). _Then there exists a uniformly continuous function h_ : [0 _, T_ ] × R<sup>_n_</sup> → R, _which does not depend on (t, x)_ , _such that_ 



_In particular_ , _the process Y_<sup>_t,x_</sup> _satisfies Condition A_ .2 _on_ [ _t, T_ ]. 

**_Proof_** By [12, Theorem 3.4] and the remark following it, there exists a decoupling field _h_ : [0 _, T_ ] × R<sup>_n_</sup> → R that is 1 _/_ 2-Hölder continuous in _t_ and locally Lipschitzcontinuous in _x_ such that (A.4) holds. It therefore remains to prove the uniform continuity of _h_ . Since 



we see that _h_ is bounded, say | _h_ | ≤ _h_ . Then 



is finite-valued, and Lemma A.9 therefore allows us to define its concave envelope _ωh(s,_ · _)_ : [0 _,_ ∞ _)_ →[0 _,_ ∞ _)_ . 

Using the Lipschitz-continuity of _ψ_ , we obtain for all _x_ 1 _, x_ 2 ∈ R<sup>_n_</sup> and _t_ ∈[0 _, T_ ] that 



Using (A.3) along with Lemma A.9, if | _x_ 1 − _x_ 2| ≤ _δ_ , then 



and 



for some constant _C_ ∈ _(_ 0 _,_ ∞ _)_ that depends only on _μ_ ˜ , _σ_ ˜ and _L_ . In view of (A.1), 



This shows that 





U. Horst, E. Kivman 

802 

Hence Gronwall’s inequality implies that _h_ is uniformly Lipschitz-continuous in the second variable. Since _h_ is uniformly 1 _/_ 2-Hölder-continuous in the first variable, this proves the assertion. □ 

## **Appendix B: Convergence results for SDEs and random ODEs** 

In this appendix, we establish three convergence results for stochastic integral equations that are useful to establish the convergence of our coefficient and state processes. In Sect. B.1, we consider sequences of SDEs parametrised by _η_ → 0 with “positive feedback”, i.e., sequences of equations that are driven away from their limits. In Sect. B.2, we consider sequences of random ODE systems parametrised by _η_ → 0 with “negative feedback”, which are driven towards their limit. 

For equations with positive feedback, we require a priori information about the terminal value and the increments of the coefficient processes to be bounded independently of their past (see Conditions A.1 and A.2). These bounds prevent the stochastic process from reaching its terminal value if a large difference between the limit and the pre-limit occurs at some time _s_ ∈ _(_ 0 _, T )_ . This will imply a.s. uniform convergence. An almost sure statement under negative feedback cannot be expected. Instead, we follow a pathwise approach and establish uniform convergence in probability. 

### **B.1 SDEs with positive feedback** 

### **B.1.1 An equation without scaling** 

We first consider a family of real-valued, adapted, continuous stochastic processes _b_<sup>_η_</sup> = _(bt_<sup>_η)t_∈</sup><sup>_(_0</sup><sup>_,T )_that satisfy the integral equation</sup> 



where _a_ : _�_ × _(_ 0 _, T )_ × R → R is adapted and continuous and _(qt_<sup>_η)t_∈</sup><sup>_(_0</sup><sup>_,T )_isan</sup> adapted, real-valued, continuous stochastic process and 



Our goal is to prove that the processes _(bt_<sup>_η)t_∈</sup><sup>_(_0</sup><sup>_,T )_with terminal conditions</sup><sup>_b_</sup> _T_<sup>_η_=0</sup> converge to 0 as _η_ → 0 uniformly on _(_ 0 _, T )_ if the process _q_<sup>_η_</sup> does and if the mapping _a_ is such that _b_<sup>_η_</sup> is driven away from 0. Intuitively, the last condition makes it impossible for _b_<sup>_η_</sup> to return to 0 at the terminal time once it has left a neighbourhood of 0. Specifically, we assume that the following assumption is satisfied. 

**Assumption B.1** i) There exists a real constant _<u>a</u> >_ 0 such that 



ii) For all _ε >_ 0, there exists a _δ >_ 0 such that for all _η >_ 0, 



Optimal trade execution under small market impact 

803 

iii) There exists a function _η_ 1 : _(_ 0 _,_ ∞ _)_ → _(_ 0 _,_ ∞ _)_ such that for all _ε >_ 0 and _η_ ≤ _η_ 1 _(ε)_ , 



iv) There exists an _ε_ 1 _>_ 0 such that 



**Lemma B.2** _Under Assumption B_ .1, _there exists a function η_ 0 : _(_ 0 _,_ ∞ _)_ → _(_ 0 _,_ ∞ _)_ , _which depends only on η_ 1 _and the mapping ε_ �→ _δ introduced in Assumption B_ .1 ii), _such that for all ε >_ 0 _and all η_ ∈ _(_ 0 _, η_ 0 _(ε)_ ], 



**_Proof_** Let _ε_ ∈ _(_ 0 _, ε_ 1]. In the first step, we show that there exists an _η_ 0 _(ε) >_ 0 such that 

_σ_<sup>_ε,η_</sup> := inf{ _t_ ∈ _(_ 0 _, T )_ : P[ _bs_<sup>_η<_−</sup><sup>_ε_] = 0 for all</sup><sup>_s_∈[</sup><sup>_t, T )_}</sup> 

satisfies _σ_<sup>_ε,η_</sup> = 0 for all _η_ ∈ _(_ 0 _, η_ 0 _(ε)_ ]. Since _b_<sup>_η_</sup> is continuous, this will imply that for all _η_ ∈ _(_ 0 _, η_ 0 _(ε)_ ], 



The other part of the statement can be proved analogously. 

We now assume to the contrary that 0 _< σ_<sup>_ε,η_</sup> and prove that this leads to a contradiction if _η_ is small enough. For all _s_ ∈ _(_ 0 _, T )_ and _η_ ∈ _(_ 0 _,_ ∞ _)_ , we can define the stopping time 



By Assumption B.1 ii), there exists a small enough _δ >_ 0 such that for all _s_ ∈ _(_ 0 _, T )_ and _η_ ∈ _(_ 0 _,_ ∞ _)_ , 



Since _τ_<sup>_s_</sup> is a stopping time, the square-integrability of _Z_<sup>_~~η~~_</sup> , the optional sampling theorem and Assumption B.1 i), iv), (B.1) and (B.2) yield that for all _s_ ∈ _(_ 0 _, σ_<sup>_ε,η_</sup> _)_ and _η_ ∈ _(_ 0 _,_ ∞ _)_ , 



U. Horst, E. Kivman 

804 

Inside the second integral, if _σ_<sup>_ε,η_</sup> _< τ_<sup>_s_</sup> , we have _bt_<sup>_η_∈[−</sup><sup>_ε,_−</sup><sup>_ε/_2]. Hence</sup> 



The assumption 0 _< σ_<sup>_ε,η_</sup> implies that for any _α >_ 0, we can find 



with P[ _bs_<sup>_η<_−</sup><sup>_ε_]</sup><sup>_>_0. However, due to Assumption B.1 ii) and iii), we can choose</sup> _α_ ≤ _ε/(_ 5 _<u>a)</u>_ , _δ_ ≤ _ε/(_ 5 _T )_ and _η_ 0 _(ε) < η_ 1 _(δ)_ such that (B.3) leads to a contradiction because P[ _bs_<sup>_η<_−</sup><sup>_ε_]</sup><sup>_>_0. This implies 0 =</sup><sup>_σ ε,η_.</sup> □ 

### **B.1.2 An equation with scaling** 

In this section, we establish an abstract convergence result for stochastic processes _(Pt_<sup>_η)t_∈</sup><sup>_(_0</sup><sup>_,T )_indexed by some parameter</sup><sup>_η >_0 that satisfy the integral equation</sup> 



where _a_ : R<sup>2</sup> → R is a measurable mapping, _(ψt )t_ ∈ _(_ 0 _,T )_ , _(Pt_<sup>_η)t_∈</sup><sup>_(_0</sup><sup>_,T )_,</sup><sup>_(P_</sup> _t_<sup>0</sup><sup>_)t_∈</sup><sup>_(_0</sup><sup>_,T )_,</sup> _(qt_<sup>_η)t_∈</sup><sup>_(_0</sup><sup>_,T )_and</sup><sup>_(Lη_</sup> _t_<sup>_)t_∈</sup><sup>_(_0</sup><sup>_,T )_are adapted, real-valued, continuous stochastic processes</sup> and 



Our goal is to prove that the processes _P_<sup>_η_</sup> converge to _P_<sup>0</sup> as _η_ → 0 uniformly on compact subintervals of _(_ 0 _, T )_ if the mapping _a_ is such that _P_<sup>_η_</sup> is driven away from _P_<sup>0</sup> and if the boundary condition lim _t_ → _T (Pt_<sup>_η_−</sup><sup>_P_</sup> _t_<sup>0</sup><sup>_)_≥0 holds. If the integral</sup> equation (B.4) holds on the whole interval _(_ 0 _, T_ ], then it is enough to assume that _PT_<sup>_η_≥</sup><sup>_P_</sup> _T_<sup>0.WhenapplyingtheabstractconvergenceresulttotheBSDEs(3.2)and</sup> (3.3), the former condition holds if _N_ = ∞, while the latter holds if _N_ is finite. 

**Assumption B.3** i) There exist positive constants _<u>ψ</u>_ <u>,</u> _ψ_ , _P_ such that _<u>ψ</u>_ ≤ _ψ_ ≤ _ψ_ , | _P_<sup>0</sup> | ≤ _P_ and _P_<sup>_η_</sup> ≥− _P_ . Moreover, there exists a function _η_ 1 : _(_ 0 _,_ ∞ _)_ → _(_ 0 _,_ ∞ _)_ such that 



ii) One of the following two “boundary conditions” holds: 

a) For all _η >_ 0, there exists a _Tη < T_ such that 



b) The integral equation (B.4) holds on the whole interval _(_ 0 _, T_ ] and _PT_<sup>_η_≥</sup><sup>_P_</sup> _T_<sup>0.</sup> Thus in particular, the processes _ψ_ , _P_<sup>_η_</sup> , _P_<sup>0</sup> , _q_<sup>_η_</sup> and _L_<sup>_η_</sup> are continuous on _(_ 0 _, T_ ] and _Z_<sup>_~~η~~_</sup> ∈ _L_<sup>2</sup> Prog<sup>_(�_×</sup><sup>_(_0</sup><sup>_, T_]; R</sup><sup>_m)_.</sup> 

Optimal trade execution under small market impact 

805 

iii) There exists a function _η_ 2 : _(_ 0 _,_ ∞ _)_ → _(_ 0 _,_ ∞ _)_ such that for all _ε >_ 0 and all _η_ ∈ _(_ 0 _, η_ 2 _(ε)_ ], 



iv) The function _a_ : R<sup>2</sup> → R is nondecreasing and differentiable in the first variable and _a(Pt_<sup>0</sup><sup>_(ω), P_</sup> _t_<sup>0</sup><sup>_(ω))_≡0. Moreover, there exist</sup> _<u>ε, β</u> >_ 0 such that 



v) The processes _ψ_ and _P_<sup>0</sup> satisfy Condition A.2, and the processes _L_<sup>_η_</sup> _(η >_ 0 _)_ satisfy Condition A.1 uniformly in _η >_ 0. 

**Lemma B.4** _Under Assumption B_ .3, _there exists a mapping η_ 0 : _(_ 0 _,_ ∞ _)_ → _(_ 0 _,_ ∞ _) that depends only on ψ_ , _P_ , _η_ 1, _η_ 2, _P_<sup>0</sup> , _<u>ε</u>_ <u>,</u> _β and the mapping ε_ �→ _δ corresponding to the uniform satisfaction of Condition A_ .1 _by (L_<sup>_η_</sup> _)η>_ 0 _such that for all ε >_ 0 _and η_ ∈ _(_ 0 _, η_ 0 _(ε)_ ], 



**_Proof_** Let _p_<sup>_η_</sup> := _P_<sup>_η_</sup> − _P_<sup>0</sup> and fix _ε_ ∈ _(_ 0 _,_ _<u>ε</u>_ ]. We first show that there exists _η_ 0 _(ε) >_ 0 such that 



Since _p_<sup>_η_</sup> is continuous, this proves that, for all _η_ ≤ _η_ 0 _(ε)_ , 



To show (B.5), we fix _δ_ ≤ _ε/_ 2 and _s_ ∈ _(_ 0 _, T_ − _ε_ ]. For all _η >_ 0, we define the stopping time 



The _L_<sup>2</sup> -integrability of _Z_<sup>_~~η~~_</sup> on [ _s, T_ − _ε/_ 2] and the optional sampling theorem give 



We now analyse the various terms in (B.6) separately to deduce that P[ _ps_<sup>_η>ε_]=0</sup> if 



U. Horst, E. Kivman 

806 

We first consider the term 



On { _ps_<sup>_η> ε_}, we have</sup><sup>_p_</sup> _u_<sup>_η_≥</sup><sup>_ε/_2 for all</sup><sup>_u_∈[</sup><sup>_s, τ η_] and hence</sup> 



Together with our assumption on the process _q_<sup>_η_</sup> and our choice of _η_ , this implies by using (B.7) and _qu_<sup>_η_≥−</sup><sup>_βε/_4 that</sup> 



By Assumption B.3 v), Lemma A.5 and Theorem A.8, we can choose for any _ε_ 0 _>_ 0 some _δ >_ 0, which does not depend on _η_ , such that 



and 



Using (B.8), the fact that _P_<sup>_η_</sup> | _�_ × _(_ 0 _,T_ − _ε_ ] ≤ _P_ by Assumption B.3 i) and _pτ_<sup>_ηη_≤2</sup> _P_ yields 



Altogether this yields 



Now if we choose first _ε_ 0 and then _η_ 0 _(ε)_ small enough, the coefficients that multiply the probabilities in (B.9) become positive. Hence both probabilities must be equal to 

Optimal trade execution under small market impact 

807 

zero, and so we have P[ _ps_<sup>_η>ε_]=0 if</sup><sup>_η_≤</sup><sup>_η_</sup> 0<sup>_(ε)_. Analogously, we can prove that</sup> P[ _ps_<sup>_η<_−</sup><sup>_ε_] = 0 for all</sup><sup>_s_∈</sup><sup>_(_0</sup><sup>_, T )_. The main difference is that</sup><sup>_τ η< T_−</sup><sup>_δ_does not</sup> hold on the set { _ps_<sup>_η<_−</sup><sup>_ε_}. Instead, we only obtain</sup><sup>_τ η< T_(if Assumption B.3 ii) b)</sup> holds) or _τ_<sup>_η_</sup> _< Tη_ (if Assumption B.3 ii) a) holds). □ 

### **B.2 ODE systems with negative feedback** 

In this section, we establish a convergence result for ODEs with random coefficients and a “nearly singular” driver. Specifically, we consider pairs of continuously differentiable stochastic processes _(X_<sup>_η_</sup> _, Z_<sup>_η_</sup> _)_ which satisfy the ODE system 



on some time interval [0 _, S_ ] for all _η >_ 0, where _A_<sup>_η_</sup> , _B_<sup>_η_</sup> , _C_ and _D_ are continuous, adapted, real-valued stochastic processes. We assume that the process _A_<sup>_η_</sup> converges in probability to infinity as _η_ → 0, that the process _B_<sup>_η_</sup> converges to a process _B_<sup>0</sup> in probability and that the processes _(X_<sup>_η_</sup> _, Z_<sup>_η_</sup> _)_ are uniformly bounded in probability. 

**Assumption B.5** There exists a continuous adapted process _B_<sup>0</sup> such that for all _ε, δ_ ∈ _(_ 0 _,_ ∞ _)_ , there exists some _η_ 0 = _η_ 0 _(ε, δ) >_ 0 such that for all _η_ ∈ _(_ 0 _, η_ 0], 



Moreover, for all _δ_ ∈ _(_ 0 _,_ 1 _)_ , there exists an _L >_ 0 such that for all _η >_ 0, 



In terms of the process _B_<sup>0</sup> , we define 



Our goal is to prove that _(X_<sup>_η_</sup> _, Z_<sup>_η_</sup> _)_ → _(X_<sup>0</sup> _, Z_<sup>0</sup> _)_ in a suitable sense as _η_ → 0. Since the initial condition of the processes _X_<sup>_η_</sup> does not vary with _η_ and since _A_<sup>_η_</sup> 0<sup>↑∞, we</sup> cannot expect convergence in _t_ = 0. Instead, we first prove that<sup>_<u>X</u>_</sup> _A_<sup>˙</sup><sup>_<u>ηη</u>_converges to 0 on</sup> any compact subinterval of _(_ 0 _, S_ ], with lower, respectively upper convergence near the origin. More precisely, we have the following result. 

**Lemma B.6** _If Assumption B_ .5 _holds and if x_ 0 + _B_ 0<sup>0</sup><sup>_z_0</sup><sup>_>_0,</sup><sup>_then for all ε, δ_∈</sup><sup>_(_0</sup><sup>_,_∞</sup><sup>_)_,</sup> _there exists η_ 1 = _η_ 1 _(ε, δ) >_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 1], 



U. Horst, E. Kivman 

808 

_If x_ 0 + _B_ 0<sup>0</sup><sup>_z_0</sup><sup>_<_0,</sup><sup>_thentheintervals_[0</sup><sup>_, S_]</sup><sup>_and_[</sup><sup>_ε, S_]</sup><sup>_intheabovestatementareto_</sup> _be swapped_ . _The mapping η_ 1 _depends only on B_<sup>0</sup> , _C_ , _D_ , _x_ 0, _z_ 0 _and the mappings (ε, δ)_ �→ _η_ 0 _(ε, δ) and δ_ �→ _L introduced in Assumption B_ .5. 

**_Proof_** Let _y_ 0 := _x_ 0 + _B_ 0<sup>0</sup><sup>_z_0. We assume without loss of generality that</sup><sup>_y_0</sup><sup>_>_0. Then</sup> there exists _α_ 0 _>_ 0 such that 



Let us now put 



Due to Assumption B.5 and Lemma 3.9, for all _δ >_ 0, _β >_ 0 and _α_ ≤ _α_ 0, there exist constants _L_ 0 _(δ)_ , _η_ 0 _(α, δ)_ and _ν_ 0 _(β, δ)_ such that 



Moreover, for all _α_ ≤ _α_ 0 and P-a.a. _ω_ ∈ _M_<sup>_α,η_</sup> , 



We are now ready to prove that for all _ε, δ >_ 0, there exists _η_ 1 _>_ 0 such that for all _η_ ≤ _η_ 1, 



To this end, we take an _ω_ ∈ _M_<sup>_L,η_</sup> ∩ _M_<sup>_α,η_</sup> ∩ _M_<sup>_β,ν_</sup> where (B.12) holds and assume _X_ ˙ _<u>t</u>_<sup>_η(ω)_</sup> that there exists some _t_ ∈[0 _, S_ ] such that _A_<sup>_<u>η</u>_</sup> _t_<sup>_(ω)_≥</sup><sup>_ε_. It is enough to show that such</sup> _X_ ˙<sup>_η_</sup> _<u>(ω)</u>_ a _t_ cannot exist for _L_ large and _α_ , _β_ , _ν_ , _η_ small enough. Since _A_<sup>_<u>η</u>_</sup> _(ω)_<sup>iscontinuous</sup> _X_ ˙ <u>0</u><sup>_η(ω)_</sup> and _A_<sup>_<u>η</u>_</sup> 0<sup>_(ω)<_0 if</sup><sup>_α_≤</sup><sup>_α_0, we can choose a minimal</sup><sup>_t_∈[0</sup><sup>_, S_] with the property that</sup> _X_ ˙ _<u>t</u>_<sup>_η(ω)_</sup> _A_<sup>_<u>η</u>_</sup> _t_<sup>_(ω)_=</sup><sup>_ε_. Let</sup> 



Optimal trade execution under small market impact 

809 

Then _XA_ ˙<sup>_<u>η</u>_</sup> _ss_<sup>_η(ω)(ω)_=</sup> 2<sup>_<u>ε</u>_and</sup> _XA_ ˙<sup>_<u>η</u>_</sup> _uu_<sup>_η(ω)(ω)>_</sup> 2<sup>_<u>ε</u>_for all</sup><sup>_u_∈</sup><sup>_(s, t_]. We now distinguish two cases. If</sup> _t_ − _s_ ≥ _ν_ , then the fact that _ω_ ∈ _M_<sup>_L,η_</sup> ∩ _M_<sup>_α,η_</sup> yields 



If _t_ − _s < ν_ , we have 



Using that _ω_ ∈ _M_<sup>_α,η_</sup> yields − _Xt_<sup>_η(ω)_+</sup><sup>_X_</sup> _s_<sup>_η(ω)_=−</sup> � _st XA_ ˙<sup>_<u>η</u>_</sup> _uu_<sup>_η(ω)(ω)A_</sup> _u_<sup>_η(ω)_d</sup><sup>_u<_0. Using</sup> that _ω_ ∈ _M_<sup>_L,η_</sup> ∩ _M_<sup>_β,ν_</sup> yields 



Now we first choose _L_ ≥ _L_ 0 _(δ)_ , then _β >_ 0 such that 3 _Lβ <_ 4<sup>_<u>ε</u>_, then</sup><sup>_ν_≤</sup><sup>_ν_0</sup><sup>_(β, δ)_</sup> such that 2 _L_<sup>2</sup> _ν(L_ + _α_ 0 _) <_ 4<sup>_<u>ε</u>_, then</sup><sup>_α_≤</sup><sup>_α_0 such that</sup> 2<sup>_<u>νε</u>_</sup> _α_<sup>_>_2</sup><sup>_L_and</sup><sup>_α_≤</sup><sup>_β_, and finally</sup> _η_ 1 _(ε, δ)_ ≤ _η_ 0 _(α, δ)_ . Then both (B.14) and (B.15) are violated. As a result, 



and hence (B.13) holds. 

It remains to prove that for suitably chosen parameters, 



To this end, we fix _ω_ ∈ _M_<sup>_L,η_</sup> ∩ _M_<sup>_α,η_</sup> ∩ _M_<sup>_β,ν_</sup> and assume that there exists a minimal _X_ ˙ _<u>t</u>_<sup>_η(ω)_</sup> _t_ ∈[ _ε, S_ ] such that _A_<sup>_<u>η</u>_</sup> _t_<sup>_(ω)_≤−</sup><sup>_ε_.Usingthat</sup><sup>_ω_∈</sup><sup>_ML,η_∩</sup><sup>_Mα,η_,itisstraightfor-</sup> ward to show that for all sufficiently small _α_ , there exists an _r_ ∈[0 _, ε_ ] such that _X_ ˙ _<u>r</u>_<sup>_η(ω)_</sup> _A_<sup>_<u>η</u>_</sup> _r_<sup>_(ω)_≥−</sup><sup>_ε/_2. Hence we can define</sup> 



U. Horst, E. Kivman 

810 

Then 0 ≤ _s < t_ and _X_ ˙ _<u>u</u>_<sup>_η(ω)_</sup> _A_<sup>_<u>η</u>_</sup> _u_<sup>_(ω)_∈[−</sup><sup>_ε,_−</sup><sup>_ε/_2] for all</sup><sup>_u_∈[</sup><sup>_s, t_]. We can now use the same</sup> arguments as in the first step to conclude that 



Combining (B.13) and (B.16) yields the desired result. 



**Lemma B.7** _Let Assumption B_ .5 _be satisfied and let x_ 0 + _B_ 0<sup>0</sup><sup>_z_0̸=0.</sup><sup>_Thenforall_</sup> _ε, δ_ ∈ _(_ 0 _,_ ∞ _)_ , _there exists η_ 2 = _η_ 2 _(ε, δ) >_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 2], 



_The mapping η_ 2 _depends only on B_<sup>0</sup> , _C_ , _D_ , _x_ 0, _z_ 0 _and the mappings (ε, δ)_ �→ _η_ 0 _(ε, δ) and δ_ �→ _L introduced in Assumption B_ .5. 

**_Proof_** Since 



the assertion follows from Assumption B.5 and Lemma B.6. 

The next theorem follows by Assumption B.5 and Lemmas B.6 and B.7. 

**Theorem B.8** _If Assumption B_ .5 _holds and x_ 0 + _B_ 0<sup>0</sup><sup>_z_0</sup><sup>_>_0,</sup><sup>_then for all ε, δ_∈</sup><sup>_(_0</sup><sup>_,_∞</sup><sup>_)_,</sup> _there exists η_ 3 = _η_ 3 _(ε, δ) >_ 0 _such that for all η_ ∈ _(_ 0 _, η_ 3], 



_If x_ 0 + _B_ 0<sup>0</sup><sup>_z_0</sup><sup>_<_0,</sup><sup>_thentheintervals_[0</sup><sup>_, S_]</sup><sup>_and_[</sup><sup>_ε, S_]</sup><sup>_inthestatementaretobe_</sup> _swapped_ . _The mapping η_ 3 _depends only on B_<sup>0</sup> , _C_ , _D_ , _x_ 0, _z_ 0 _and the mappings (ε, δ)_ �→ _η_ 0 _(ε, δ) and δ_ �→ _L introduced in Assumption B_ .5. 

**Acknowledgements** Financial support through the Stiftung der Deutschen Wirtschaft (sdw) is gratefully acknowledged. We thank Peter Bank, Mikhail Urusov and seminar participants at various institutions for valuable comments and remarks. We thank two anonymous referees for valuable comments and suggestions that greatly helped to improve the presentation of the results. 

**Funding** Open Access funding enabled and organized by Projekt DEAL. 

### **Declarations** 

**Competing Interests** The authors declare no competing interests. 

Optimal trade execution under small market impact 

811 

**Open Access** This article is licensed under a Creative Commons Attribution 4.0 International License, which permits use, sharing, adaptation, distribution and reproduction in any medium or format, as long as you give appropriate credit to the original author(s) and the source, provide a link to the Creative Commons licence, and indicate if changes were made. The images or other third party material in this article are included in the article’s Creative Commons licence, unless indicated otherwise in a credit line to the material. If material is not included in the article’s Creative Commons licence and your intended use is not permitted by statutory regulation or exceeds the permitted use, you will need to obtain permission directly from the copyright holder. To view a copy of this licence, visit http://creativecommons.org/licenses/by/ 4.0/. 

## **References** 

1. Ackermann, J., Kruse, T., Urusov, M.: Càdlàg semimartingale strategies for optimal trade execution in stochastic order book models. Finance Stoch. **25** , 757–810 (2021) 

2. Alfonsi, A., Fruth, A., Schied, A.: Optimal execution strategies in limit order books with general shape functions. Quant. Finance **10** , 143–157 (2010) 

3. Almgren, R., Chriss, N.: Optimal execution of portfolio transactions. J. Risk **3** , 5–40 (2001) 

4. Ankirchner, S., Jeanblanc, M., Kruse, T.: BSDEs with singular terminal condition and a control problem with constraints. SIAM J. Control Optim. **52** , 893–913 (2014) 

5. Becherer, D., Bilarev, T., Frentrup, P.: Stability for gains from large investors’ strategies in _M_ 1 _/J_ 1 topologies. Bernoulli **25** , 1105–1140 (2019) 

6. Bertsimas, D., Lo, A.W.: Optimal control of execution costs. J. Financ. Mark. **1** , 1–50 (1998) 

7. Bouchard, B., Loeper, G., Zou, Y.: Almost-sure hedging with permanent price impact. Finance Stoch. **20** , 741–771 (2016) 

8. Carmona, R., Webster, K.: The self-financing equation in limit order book markets. Finance Stoch. **23** , 729–759 (2019) 

9. Cartea, Á., Donnelly, R., Jaimungal, S.: Portfolio liquidation and ambiguity aversion. In: Dempster, M.A.H., et al. (eds.) High-Performance Computing in Finance, pp. 77–114. Chapman & Hall, London (2018) 

10. Cartea, Á., Jaimungal, S.: Incorporating order-flow into optimal execution. Math. Financ. Econ. **10** , 339–364 (2016) 

11. Chen, N., Kou, S., Wang, C.: A partitioning algorithm for Markov decision processes with applications to market microstructure. Manag. Sci. **64** , 784–803 (2018) 

12. El Karoui, N.: Backward stochastic differential equations. A general introduction. In: El Karoui, N., Mazliak, L. (eds.) Pitman Research Notes in Mathematics Series, pp. 7–26 (1997) 

13. Fruth, A., Schöneborn, T., Urusov, M.: Optimal trade execution in order books with stochastic liquidity. Math. Finance **29** , 507–541 (2019) 

14. Fu, G., Graewe, P., Horst, U., Popier, A.: A mean field game of optimal portfolio liquidation. Math. Oper. Res. **46** , 1250–1281 (2021) 

15. Gârleanu, N., Pedersen, L.H.: Dynamic portfolio choice with frictions. J. Econ. Theory **165** , 487–516 (2016) 

16. Gatheral, J.: No-dynamic-arbitrage and market impact. Quant. Finance **10** , 749–759 (2010) 

17. Gatheral, J., Schied, A.: Optimal trade execution under geometric Brownian motion in the Almgren and Chriss framework. Int. J. Theor. Appl. Finance **14** , 353–368 (2011) 

18. Graewe, P., Horst, U.: Optimal trade execution with instantaneous price impact and stochastic resilience. SIAM J. Control Optim. **55** , 3707–3725 (2017) 

19. Graewe, P., Horst, U., Qiu, J.: A non-Markovian liquidation problem and backward SPDEs with singular terminal conditions. SIAM J. Control Optim. **53** , 690–711 (2015) 

20. Guéant, O.: Optimal execution and block trade pricing: a general framework. Appl. Math. Finance **22** , 336–365 (2015) 

21. Horst, U., Naujokat, F.: When to cross the spread? Trading in two-sided limit order books. SIAM J. Financ. Math. **5** , 278–315 (2014) 

22. Horst, U., Xia, X.: Multi-dimensional optimal trade execution under stochastic resilience. Finance Stoch. **23** , 889–923 (2019) 

23. Horst, U., Xia, X., Zhou, C.: Optimal portfolio liquidation under factor uncertainty. Ann. Appl. Probab. **32** , 80–123 (2022) 

U. Horst, E. Kivman 

812 

24. Kruse, T., Popier, A.: Minimal supersolutions for BSDEs with singular terminal condition and application to optimal position targeting. Stoch. Process. Appl. **126** , 2554–2592 (2016) 

25. Large, J.: Measuring the resiliency of an electronic limit order book. J. Financ. Mark. **10** , 1–25 (2007) 

26. Lo, D.K., Hall, A.D.: Resiliency of the limit order book. J. Econ. Dyn. Control **61** , 222–244 (2015) 

27. Lokka, A., Xu, J.: Optimal liquidation for a risk averse investor in a one-sided limit order book driven by a Lévy process (2020). Preprint, Available online at https://arxiv.org/abs/2002.03379 

28. Lorenz, C., Schied, A.: Drift dependence of optimal trade execution strategies under transient price impact. Finance Stoch. **17** , 743–770 (2013) 

29. Neuman, E., Voß, M.: Optimal signal-adaptive trading with temporary and transient price impact. SIAM J. Financ. Math. **13** , 551–575 (2022) 

30. Obizhaeva, A.A., Wang, J.: Optimal trading strategy and supply/demand dynamics. J. Financ. Mark. **16** , 1–32 (2013) 

31. Predoiu, S., Shaikhet, G., Shreve, S.: Optimal execution in a general one-sided limit-order book. SIAM J. Financ. Math. **2** , 183–212 (2011) 

32. Protter, P.E.: Stochastic Integration and Differential Equations, 2nd edn. Springer, Berlin (2004) 

**Publisher’s Note** Springer Nature remains neutral with regard to jurisdictional claims in published maps and institutional affiliations. 

