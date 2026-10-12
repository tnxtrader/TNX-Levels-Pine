# Bailey, Borwein, López de Prado, Zhu (2014) — Pseudo-Mathematics and Financial Charlatanism
Notices of the AMS 61(5):458-471. doi:10.1090/noti1105. 14 pp. Read in full (text + page renders for equations).

## Hypothesis
With enough configurations tried (N), a backtest with a high in-sample (IS) Sharpe is almost guaranteed even when every
configuration has zero true skill. With memory in the series (global constraint / serial correlation), IS optimisation is
*harmful* OOS (IS and OOS Sharpe become negatively related).

## Formulas (verified from page renders)
(1) r_t ~ N(mu, sigma^2)
(2) SR = (mu/sigma) * sqrt(q)                       q = returns per year
(3) SR_hat ->a N[ SR, (1 + SR^2/(2q)) / y ]         y = years of data (Lo 2002)
(4) E[max_N] ≈ (1-γ) Z^-1[1 - 1/N] + γ Z^-1[1 - 1/(N e)]      γ = 0.5772156649 (Euler–Mascheroni), Z^-1 = inverse std-normal CDF
    Upper bound: sqrt(2 ln N)
(5) E[max_N] (annualised, y years) ≈ y^(-1/2) * ((1-γ) Z^-1[1-1/N] + γ Z^-1[1-1/(N e)])
(6) MinBTL (years) ≈ ( ((1-γ)Z^-1[1-1/N] + γ Z^-1[1-1/(N e)]) / E[max_N] )^2  <  2 ln N / E[max_N]^2
(7) Δm_τ = µ + σ ε_τ  (random-walk performance path)
(10/11) AR(1): m_τ = (1-φ)µ + φ m_{τ-1} + σ ε_τ
Prop 4 half-life: τ = -ln 2 / ln φ     (φ = 0.995 -> ~138 obs)
Prop 3/5: with a global constraint or AR(1) memory, SR_IS^A > SR_IS^B  =>  SR_OOS^A < SR_OOS^B

## Key numbers
- N = 10 skill-less configs -> expected max IS Sharpe = 1.57 (OOS = 0).
- 5 years of data -> try no more than ~45 independent configs to keep E[max] at SR 1.
- 2 years of data -> only ~7 configs.
- 7 binary parameters -> N=128 -> expected max IS SR > 2.6.
- Example 6: random walk, 8,800-node grid (entry day x hold x stop x side) -> "optimal" SR 1.27, PSR 2.83 (looks <1% chance of SR<=0) yet it is pure noise.

## Figures (images/ + pages/)
Fig 1 max of N normals vs N; Fig 2 MinBTL vs N; Fig 3 IS vs OOS no selection; Fig 4/5 selection without memory (OOS ~0);
Fig 6 global constraint -> negative IS/OOS slope; Fig 7 AR(1) phi=0.995 -> negative slope; Fig 8 seasonal-rule overfit.

## Direct relevance to T3 Entry Lab
- Lab = 103 entry IDs x 2 directions = 206 trials on the same data, *before* any T3 length / VF / TP / SL / maxBars grid.
  Each extra grid value multiplies N. With e.g. 5 T3 lengths x 4 VF x 4 TP x 4 SL x 4 maxBars -> 206*1280 = 263,680 trials.
- The 103 rules are NOT independent (most are Base+Filter combos of 9 base rules x ~15 filters) -> effective N is smaller;
  estimate via correlation/PCA of the trade-return series (paper suggests PCA). Must report N.
- Must deflate the winner (DSR) and estimate PBO via CSCV (Bailey et al. 2016, cited [1]); hold-out alone is insufficient.
- Filters ("sets of conditions that trigger trading actions") are explicitly called out as an overfitting engine — exactly the Lab's design.
