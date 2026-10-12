# Zhang (2022) — A Model Combining LightGBM and Neural Network for High-frequency RV Forecasting. ICFIED 2022, AEBMR vol 211. 7 pp. Read in full.

## Hypothesis
A LightGBM + NN additive ensemble on order-book features forecasts 10-min realized volatility better than LightGBM alone.

## Method / formulas
Data: Optiver Kaggle "Realized Volatility Prediction" (1-second order book + trades, ~150k targets, 10-min buckets).
Features: BidSpread = Bid1 − Bid2; AskSpread = Ask1 − Ask2; |BidSpread − AskSpread|; TotalVolume = ΣAsk/Bid sizes (L1+L2);
VolumeImbalance = (Ask1+Ask2 sizes) − (Bid1+Bid2 sizes); WAP_n = (Bid_n·AskSize_n + Ask_n·BidSize_n)/(BidSize_n + AskSize_n);
WAP balance = WAP1 − WAP2; log return of WAP; RV = sqrt(Σ r²) per bucket; k-means cluster id as a feature.
LightGBM params: max_depth 5, num_leaves 10, max_bin 100, min_data_in_leaf 500, feature/bagging fraction 0.8, lr 0.1; NN 128-64-32.
Metrics: RMSE, RMSPE = sqrt(mean(((y−ŷ)/y)²)); and (oddly) accuracy/precision/recall/F1.
Results: RMSE 0.0021 -> 0.0012; RMSPE 0.2575 -> 0.2115.

## Critical assessment
- 5-fold CV (not time-ordered) on time series -> look-ahead leakage risk; classification metrics on a regression target are undefined
  in the text -> results not trustworthy as reported. No baseline vs HAR.
- Requires L2 order-book data -> NOT available to an OHLC-only user.

## Relevance to T3 Lab
- Practically none for features. Methodological lesson (negative example): never use shuffled k-fold on market time series; use
  purged/embargoed walk-forward or CPCV (López de Prado). WAP/imbalance ideas need order-book data the user does not have.
