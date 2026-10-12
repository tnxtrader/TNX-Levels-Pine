### Page 5 table 1

|Parameter|Description|Value|
|---|---|---|
|max_depth|The maximum depth of a tree|5|
|max_bin|The maximum number of features to be stored in a bin|100|
|min_data_in_leaf|The minimum number of records a leaf may have|500|
|num_leaves|The maximum number of leaves on a tree|10|
|feature_fraction|The proportion of features used in each iteration|0.8|
|bagging_fraction|The proportion of data used in each iteration|0.8|
|learning_rate|The rate of learning|0.1|
|output_dim|The vector dimension|24|
|units|The spatial dimension|128, 64, 32|
|activation|The activation function|‘linear’|



### Page 5 table 2

|Feature|Description|
|---|---|
|Stock ID|ID code of the stock|
|Time ID|ID code of the time bucket|
|Bucket seconds|Number of seconds from the start of the bucket|
|Bid price 1|Normalized prices of the most competitive buy level|
|Bid price 2|Normalized prices of the second most competitive buy level|
|Ask price 1|Normalized prices of the most competitive sell level|
|Ask price 2|Normalized prices of the second most competitive sell level|
|Price|The average price of executed transactions happening in one second|
|Bid size 1|The number of shares on the most competitive buy level|
|Bid size 2|The number of shares on the second most competitive buy level|
|Ask size 1|The number of shares on the most competitive sell level|
|Ask size 2|The number of shares on the second most competitive sell level|
|Size|The sum number of shares traded|
|Order Count|The number of unique trades taking place|



### Page 6 table 1

|Col1|LightGBM|LightGBM -NN|
|---|---|---|
|RMSE|0.0021|0.0012|
|RMSPE|0.2575|0.2115|
|Accuracy|0.978|0.986|
|Precision|0.971|0.982|
|Recall|0.967|0.970|
|F1 score|0.969|0.977|


