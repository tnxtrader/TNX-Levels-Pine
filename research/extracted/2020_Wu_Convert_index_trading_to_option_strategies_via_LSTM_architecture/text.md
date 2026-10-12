Neural Computing and Applications (2025) 37:23047–23064 https://doi.org/10.1007/s00521-020-05377-6 (0123456789().,-volV)(0123456789().,volV) 

##### S.I. : DATA FUSION IN THE ERA OF DATA SCIENCE 



# Convert index trading to option strategies via LSTM architecture 

Jimmy Ming-Tai Wu<sup>1•</sup> Mu-En Wu<sup>2•</sup> Pang-Jen Hung<sup>2•</sup> Mohammad Mehedi Hassan<sup>3•</sup> Giancarlo Fortino<sup>4</sup> 

Received: 3 July 2020 / Accepted: 18 September 2020 / Published online: 3 October 2020 � The Author(s) 2020, corrected publication 2020 

#### Abstract 

In the past, most strategies were mainly designed to focus on stocks or futures as the trading target. However, due to the enormous number of companies in the market, it is not easy to select a set of stocks or futures for investment. By investigating each company’s financial situation and the trend of the overall financial market, people can invest precisely in the market and choose to go long or short. Moreover, how to determine the position size of the transaction is also a problematic issue. In the past, many money management theories were based on the Kelly criterion. And they put a certain percentage of their total funds into the market for trading. Nonetheless, three massive problems cannot be overcome. First, futures are leveraged transactions, and extra funds must be deposited as margin. It causes that the position size is hard to be estimated by the Kelly criterion. The second point is that the trading strategy is difficult to determine the winning rate in the financial market and cannot be brought into the Kelly criterion to calculate the optimal fraction. Last, the financial data are always massive. A big data technique should be applied to resolve this issue and enhance the performance of the framework to reveal knowledge in the financial data. Therefore, in this paper, a concept of converting the original futures trading strategy into options trading is proposed. An LSTM (long short-term memory)-based framework is proposed to predict the profit probability of the original futures strategy and convert the corresponding daily take-profit and stop-loss points according to the delta value of the options. Finally, the proposed framework brings the results into the Kelly criterion to get the optimal fraction of options trading. The final research results show that options trading is closer to the optimal fraction calculated by the Kelly criterion than futures trading. If the original futures trading strategy can profit, the benefits after converting to options trading can be further superior. 

Keywords Long short-term memory � Big data � Options � Futures � Kelly criterion � Trading strategy 

|& Giancarlo Fortino<br>g.fortino@unical.it|
|---|
|Jimmy Ming-Tai Wu<br>wmt@wmt35.idv.tw|
|Mu-En Wu<br>mnwu@ntut.edu.tw|
|Pang-Jen Hung<br>t107ab8007@ntut.edu.tw|
|Mohammad Mehedi Hassan<br>mmhassan@ksu.edu.sa|



> 1 College of Computer Science and Engineering, Shandong University of Science and Technology, Qingdao, China 

> 2 Department of Information and Finance Management, National Taipei University of Technology, Taipei, Taiwan 

> 3 Information Systems Department, College of Computer and Information Sciences, King Saud University, Riyadh, Kingdom of Saudi Arabia 

> 4 Department of Informatics, Modeling, Electronics, and Systems, University of Calabria, Rende, Italy 

## 1 Introduction 

For many experts, scholars, and even traders, it is fascinating to research and develop profitable trading strategies or optimize already existing trading strategies to maximize the overall profitable growth. In many trading strategies, most of the trading targets are stocks and futures, and stocks represent the ownership of a company. Therefore, when choosing which stock to invest in, a lot of homework will be done in advance such as the company’s operating status, debt status, evaluation. Also, many people directly use stock index futures as the investment target, because as long as the overall stock market trend is judged, enter the market to go long or short. Many trading strategies are based on common strategies or ideas and further extended, such as moving average trading strategies based on the average price over a while [10, 13, 18]; or because of trading at the opening the trading volume and price 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23048 

fluctuations are larger than other periods, there are also many trading strategies based on this, such as the open range breakthrough (ORB) trading strategy [15, 35], first formulate a period as the observation period, if only when there is a breakthrough up or down before entering the market. After deciding the target and strategy of the transaction, one of the most important factors in the financial market is money management [36–38]. Many traders are more concerned about choosing the investment target and the time to enter and exit, and ignore the control of the position size, making the final result loss or performance not as expected. The foundation of money management was derived from the Kelly criterion [17] proposed by John Larry Kelly at Bell Labs in 1956, but it was originally a formula proposed to study the probability of noise occurring in communications. It was until Edward O. Thorp put the Kelly criterion in other fields, such as blackjack for playing cards, sports lottery tickets, and the stock market [33]. The Kelly criterion can be used in situations such as gambling that can be played unlimited times and has a fixed probability of winning and odds [41], in the process of repeated betting to maximize the growth of his assets [20, 30, 34, 42]. However, there is a gap between the financial market and the gambling [16], so many scholars have begun to explore how to use the Kelly formula in a more general situation [4, 6, 9, 11, 43]. 

To use the Kelly criterion in the financial market, we need to know the odds and winning probability of the trading strategy in advance. Although the odds of the trading strategy can be controlled by formulating the takeprofit and stop-loss point, due to the futures are leveraged transactions, we have to deposit some money as a margin to operate. So if we want to calculate the position size according to the Kelly criterion, we cannot reach the size of the position calculated by the Kelly criterion when using futures trading. Therefore, we propose an architecture to convert the original futures trading strategy to using the options as the target of the transaction. At the same time, because the options premium unit is small, the position size can be controlled more accurately to achieve the optimal betting fraction by the Kelly criterion. After we use options trading to replace the original futures trading strategy, another problem is that the trading strategy we have developed is difficult to estimate the winning rate, which cannot be brought into the Kelly criterion for calculation. Many studies have combined financial transactions with machine learning to estimate stock prices or the direction of ups and downs [7, 19, 23, 31, 32]. To further improve the accuracy of prediction, artificial neural networks and deep learning have also been used in financial markets [12, 25, 45]. The study also pointed out that because of the time series characteristics in the financial market, each time the price is affected by the long-term or short-term [3]. 

Thus, the long short-term memory (LSTM) algorithm [14] has many different applications in the financial market and also more accurate than traditional machine-learning methods in the past [5, 27]. Therefore, we decided to use LSTM to estimate our trading strategy’s winning rate and, at the same time, using fixed take-profit and stop-loss points to fix the odds and bring it into the Kelly criterion to calculate the position size. 

Due to the difference between the final profit and loss results of using futures and options under the same trading strategy, the original futures trading strategy cannot be converted into options trading directly. The options have a different strike price, so the first in the money strike price is usually set as target and using the delta value as the basis for conversion from the original futures strategy’s takeprofit and stop-loss points to options operations. In the futures, according to the characteristics of the option, different options retest strategies can be used in combination with the Kelly formula to achieve better performance [39, 40]. However, if we can convert the futures strategy with good performance into options as the trading target and control the position size, it can save a lot of time in developing strategies. In this paper, a LSTM-based framework is proposed to convert a futures strategy to a options strategy.The major contributions of our work are listed as follows: 

1. We first proposed a conversion process from a futures trading strategy to a new options trading strategy. It causes a futures trading strategy can be applied in options transaction directly. 

2. A win rate prediction system-based LSTM network is proposed to apply the proposed method into Kelly formula. 

3. Due to the proposed LSTM-based prediction framework and the conversion process, the new generated options trading strategy can effectively enhance the total profit. 

## 2 Preliminaries and related work 

### 2.1 Futures trading strategy at market opening 

Futures trading price fluctuations and volume at opening periods are much bigger than other periods. For instance, Fig. 1 shows the candlestick chart minutely with the target of TAIEX futures on December 09, 2019. We discover that the first half time of differences of high and low is more than 60 points, but the second half time of differences is less than 30 points, namely the first half time is twice as much as the second half time. Not only the price 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23049 

fluctuations but also the trading volume is much bigger than the second half time except for the last closing period. 

Another example on December 13, 2019 is shown in Fig. 2. We discover that the first half time of differences of high and low is even more than 80 points, but the second half time of differences is less than 40 points. The price fluctuations and the trading volume are lager in the first half time than the second half no matter in bull market or bear market. So, there are many trading strategies based on the opening period. For example, the opening range breakout strategy is a famous and classic case that many people know. One of the basic operations is shown in Fig. 3. Take the first 15 candlesticks as standard, if the price breaks through the highest price of the first 15 candlesticks, we long the target, otherwise, we short the target. In Fig. 3, the price is lower than the lowest price of the first 15 min, so we short the target. 

### 2.2 Kelly criterion 

When we formulate trading strategies and control profit margins by setting stop-loss and take-profit points, the biggest problem we are currently facing is that we do not know how many positions we should put in the market, so many financial theories have emerged. Kelly criterion is one of the most famous theories. First, we consider a binary outcomes game with win rate p, and if the player wins, he will get b1 times of bet and get all bet back, but if the player loses, he will be deducted b2 times of bet for punishment. Without loss of generality, if the player bid 1 dollar, the player will earn 1 þ b1 dollars if win and will lose 1 � b2 dollar if lose. Namely, in the case of bidding 1 dollar, the net profit is b1 dollars. Assume that the initial capital is A0, and the capital in tth step is At, the player will 





Fig. 2 The candlestick chart of every minute of TX between 08:45 A.M. and 13:45 P.M. on December 09, 2019 



Fig. 3 An example of an opening range breakout strategy. The candlestick chart of every minute of TX between 08:45 A.M. and 09:30 A.M. on December 16, 2019 

always bid the fraction f of his total capital in every step, where 0% � f � 100%. Therefore, if win in tth step, the capital will be At ¼ At�1ð1 þ b1f Þ, on the other hand, if lose in tth step, the capital will be At ¼ At�1ð1 � b2f Þ. After playing T rounds, with winning W times and losing L times, that is, T ¼ W þ L, the function is given as 





Then, take the equation log and divide by T, we get 

Fig. 1 The candlestick chart of every minute of TX between 08:45 A.M. and 13:45 P.M. on December 09, 2019 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23050 



After many times, let T approach to infinity, and the win rate is p, we get 





To get the maxima of the capital growth rate, we take the derivative of the above equation and get the optimal fraction as 



However, we recognize that there is a gap between reality and theory. For example, consider a game with 50% win rate, it may not exactly win 50 times by playing 100 times. Its probability of winning k times is Ck<sup>100 �ð50%Þk �</sup> ð50%Þ<sup>1�k</sup> by binomial theorem. But let us not consider the issue now. Let us give an example of a practical application of the Kelly criterion. We play a fair coin-tossing game that the probability of getting head or tail is 50%, as shown in Fig. 4. If the outcome is head, we can earn double bet; otherwise, we will lose all the money we bet this round. 

Considering playing the above game after 10 rounds, we can calculate an expected return with optimal fraction by the Kelly criterion, 



As a result of the above equation and considering 1 dollar as the initial capital, the final result is shown in Fig. 5. Via the above estimation with the Kelly criterion, a game with win rate 50% and odds 2 will obtain optimal fraction as 25%, and final capital will be about 1.8 times than that of initial capital. 

### 2.3 Long short-term memory and related work 

Before we discuss long short-term memory, let us talk about the original neural network, artificial neural network (ANN). An ANN works like a human’s nervous system, based on a collection of connected artificial neurons. Each connection can transmit the signal or data to other neurons. Besides, ANN can learn from us by giving him variables, weight, bias, activation function, and so on. A neural network can be divided into three layers, input layer, hidden layer, and output layer as Fig. 6. After that, many scholars begin to increase the number of hidden layers to improve the accuracy of prediction, and the multiple hidden layer neural network is also called deep learning as shown in Fig. 7. 



Fig. 4 Coin-tossing game with win rate 50% and odds 2 



Fig. 5 The returns after 10 rounds of coin-tossing game with bidding fractions from 0 to 100% 

The output of the traditional neural network after each layer of calculation is only passed to the next layer in one direction, which means that the input and output are independent of each other. Therefore, recurrent neural network (RNN) algorithm was born. In Fig. 8, we can see that the calculation results are not only output but also become one of the inputs to the next calculation. 

Until 1997, the long short-term memory (LSTM) was proposed by Hochreiter and Schmidhuber. The LSTM algorithm solves the problem of the disappearance of gradients generated in mathematics, causing long-term memories to be hidden by short-term memories. The LSTM adds four important components, including input gate, output gate, forget gate, and memory cell as shown in Fig. 9. The input gate will control whether the input features should be input or not. The output gate will control whether to output the result of this calculation. The memory cell will store the calculation results for the next 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23051 



Fig. 6 An architecture diagram of deep learning neural network 



Fig. 7 An architecture diagram of simple artificial neural network 



Fig. 8 An architecture diagram of deep recurrent neural networks 



Fig. 9 An architecture diagram of long short-term memory 

stage to use, and the forget gate will control whether to clear the memory cell. 

After the proposal of neural network, many scholars use it to predict stock prices or weighted stock index [21, 44]. Not only the original neural network, many studies using recurrent neural network or long short-term memory have also been applied in many journals [22, 24]. Furthermore, in order to improve the prediction accuracy, many studies have added many hidden layers or mixed multiple algorithms [1, 12, 28, 29]. In addition to the stock price information, other information such as textual information, financial news, emotional data, and so on are also be added [2, 8, 26, 46]. 

## 3 The proposed method 

In this chapter, we will define the original futures day trading strategy and use long short-term memory algorithms to train the strategy’s win rate today. Next, use the delta value to convert the take-profit and stop-loss points of the original futures strategy into trading with options. The Kelly formula will be used to calculate the optimal betting ratio to estimate the size of the entry position and compare it to the futures and what is the difference in the outcome of an options trading. 

### 3.1 Define the original futures strategy 

We first consider a simple futures day trading strategy, for example, by setting a take-profit point and a stop-loss point and, at the same time, formulating a daily observation and trading period as shown in Fig. 10. Suppose our trading strategy is long at 80 points take-profit and 20 points for stop-loss. The observation period is from 08:45 A.M. to 09:15 A.M. During this period time, we do not trade but judge whether to enter the market today to execute the strategy and estimate the size of our entry position. If we decide to enter the market today to implement the strategy, 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23052 

we will enter at 09:15 A.M., and during the period from 09:15 A.M. to 10:00 A.M. If the price touches the takeprofit point or stop-loss point, it will enforce to close out. Still, if no take-profit or stop-loss points are touched before 10:00 A.M., we will also enforce close the position at the end of the trading period (10:00 A.M.) and compute profit and loss. 

For example, as shown in Fig. 11, we entered the market at 09:15 A.M., the entry point is 10,000, so our stop-loss point is 10,000 ? 80 = 10,080. The stop-loss point is 10,000 – 40 = 9960, if the take-profit or stop-loss point is touched during the trading period, we will enforce to close out the position, and we find that at 09:15 A.M, the points rise above the take-profit point so that we will gain from closing positions and in this example, we have gained 80 points. 

And Fig. 12 is another example. We enter the market at 09:15 A.M., and the entry point is 10,000, so our take-profit point is 10,000 ? 80 = 10,080, and the stop-loss point is 10,000 – 40 = 9960, if the take-profit or stop-loss point is touched during the trading period, we will enforce to close out the position, and when the time reaches 09:37 A.M., the price is lower than the stop-loss of 9960 points, so we close out the position, losing 40 points in this example. 



Fig. 10 The timeline of the futures day trading strategy architecture in this paper and divided into an observation period and a trading period 



Fig. 11 The simulates run chart of the take-profit example appearance of the futures day trading strategy 



Fig. 12 The simulates run chart of the stop-loss example appearance of the futures day trading strategy 



Fig. 13 The simulates run chart of the no trigger take-profit and stoploss example appearance of the futures day trading strategy 

To give another example, as shown in Fig. 13, we also entered the market at 09:15 A.M., the entry point is 10,000, so our take-profit point is 10,000 ? 80 = 10,080. The stoploss point is 10,000 – 40 = 9960, but the price did not touch the take-profit or stop-loss point during the trading period, so we enforce to close out the position at the end of the trading period of 10:00 A.M. to settle the profit and loss, and finally, we lost 23 points. 

After we have set the trading strategy, we can bring the historical data into the long short-term algorithm to train the model and estimate the daily winning rate according to the strategy we have formulated. That is, we can calculate how much funds to invest at 09:15 A.M., and even further convert to options for the target transaction. 

### 3.2 Predict the trading strategy win rate by long short-term memory 

After we formulated the original futures day trading strategy in the previous section, if we want to use the Kelly formula to calculate the optimal betting ratio to determine 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23053 

the size of the position, we need to have a fixed profit-toloss ratio and the winning rate of this strategy. The ways of take-profit and stop-loss control the profit-to-loss ratio. Still, the strategy’s win rate is difficult to estimate in the financial market, so we decided to use long short-term memory algorithms to estimate the win rate. As shown in Fig. 14, the parameters are required before we train the model. We organize the transaction data per minute during the observation period into the opening price (the first transaction price per minute), the highest price, the lowest price, and the closing price (the last transaction price per minute), and the total transaction volume are used as the predicted features. For example, we have a trading period from 08:45 A.M. to 09:15 A.M., which means that we have five kings of features: the opening price, the highest price, the lowest price, the closing price, and the transaction volume, multiplied by 30 min. The total of 5 � 30 ¼ 150 transaction data. Among them, our four features of the opening price, highest price, lowest price, and closing price will be converted into fluctuations based on yesterday’s closing price. For example, the opening price of the first minute is 10,087 points, and yesterday’s closing price is 10,000 points. So, we will mark as 10;08710�;00010;000 ¼ 0:0087. And for the volume of the transaction, we take the five days of the week as the benchmark, and in the past five days, the average per minute during the observation period. We use the average transaction volume as a benchmark to highlight whether the trading volume explodes or decreases. For example, the transaction volume in the first minute of today is 1032, and the average transaction volume per minute in the observation period of the past five days is 1412, so we mark it as <u>10321412�1412</u> ¼ �0:269. We also need to input the parameters we want to predict. In this paper, we will divide the results into two parts to discuss as shown in Fig. 15. If 



Fig. 14 Schematic diagram of input feature data for training LSTM model 

Fig. 15 Input prediction value for training LSTM model 

the final result is assumed to be 60 points of profit, and our take-profit point is 80 points, so today’s strategy’s profit and loss value is <u>6080</u><sup>¼ 0:75. If the result is a take-profit and</sup> close position, then the strategy’s profit and loss value will be the maximal value <u>8080</u><sup>¼ 1. On the other hand, if the final</sup> result of the day is a loss, suppose we lose 10 points today, and we formulate the stop-loss point is 40 points, so today’s strategy’s profit and loss value is �4010<sup>¼ �0:25.If</sup> the final result is a stop-loss appearance, the strategy’s profit and loss value is the minimum value �4040<sup>¼ �1.</sup> 

After we have trained the prediction model, the next step is to bring the testing data into the model for prediction, as shown in Fig. 16. We also organize the observation period data into the model as input data and then output the daily input data as a result of �1 to 1. Still, this result is not the final estimation strategy we want to use. We linearly scale the output to a value between 0 and 1 as the winning rate of strategy. We regard this result as the win rate of the trade entry at 9:15 A.M. on that day, and we use this win rate to calculate the expected value, if the expected value is positive, we will enter the market today. For example, assuming that the predicted result of the LSTM model is �0:072, we get a win rate of 0.464 based on linear conversion. It means that if enter the market and execution strategy trade have a profit probability of 0.464, we finally calculate the expected value of 0:464 � 80 �ð1 � 0:464Þ � 40 ¼ 15:68, so in this example, we will enter the market today to execute the trading strategy. 

If we use LSTM to accurately predict the winning rate of the daily execution of the trading strategy, then we can further bring into the Kelly formula based on this result to calculate the optimal betting ratio and convert it to use the options as the target can be more accurate control the position size to maximize growth. 

### 3.3 Convert futures strategy to use options as the target and calculate the size of the position 

In the previous section, we used LSTM to calculate the winning rate of the daily execution of the proposed trading strategy and calculated the expected value of the day’s execution of the strategy. Although we know whether to enter the trade on the day, we still do not know how much money should be invested in the market. So here, we use the Kelly formula to calculate the size of the position. For example, our futures day trading strategy sets a stop profit of 80 points and a stop-loss of 40 points. Using LSTM to calculate the strategy win rate of today’s trading is 0.464, and assume the initial capital is 1,000,000 dollars. We can 0:464ð1þ8040<sup>~~<u>Þ</u>~~�1</sup> bring in the Kelly formula f ¼ <u>80</u> ¼ 0:196, which 40 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23054 



Fig. 16 Schematic diagram of input testing data for prediction by LSTM model 

means that we will put 0.196 times of all the current funds into the market for trading, which is 1; 000; 000 � 0:196 ¼ 196; 000 dollars. And according to the TAIEX futures, each point is worth 200 dollars, the number of the position is investedcapital ¼ 196;000 � maximumloss <u>�</u> �40�200� ¼ 24 lots. But according to the actual situation, if the futures need to bet a few, it needs a few times the original margin, assuming that the current futures original margin is 148,000 dollars, which means that to bet 24 lots futures, we need to have at least 24 � 148; 000 ¼ 3; 552; 000 dollars, but, if our funds are only 1,000,000 dollars, at most only j1148;000;000;000k ¼ 6 lots, and because there is a gap between 6 lots and 24 lots, we hope to convert the original futures trading strategy to the operation with the options as the target. If we want to convert the futures trading strategy with fixed points to options trading strategy, we need to use the delta value in the Greek letter of the options to calculate. Taking Fig. 17 as an example, assuming today’s delta value is 0.8, it means that assuming our original futures day trading strategy has a take-profit of 80 points and a stop-loss of 40 points, today’s options trading strategy take-profit after conversion is 80 � 0:8 ¼ 64 points, and the stop-loss is 40 � 0:8 ¼ 32 points. 

However, the biggest difference between options and futures is that options have many different strike prices. Assuming that the futures are now 11,108 points, we will use first in the price option, that is, the options with a strike price of 11,100. If the original futures strategy is long, we will buy call options after converting to options; otherwise, if the original futures strategy is short, then we will long put options. Assuming that our initial capital is 1,000,000 dollars, and the winning rate predicted according to the futures strategy is brought into the Kelly formula to obtain 0.196 as the optimal betting ratio, and each point of options is 50 dollars, and our position size is theoretically 





Fig. 17 Schematic diagram of input feature data for training LSTM model 

> investedcapital ¼ 196;000 ¼ 122 lots. But the same we � maximumloss <u>�</u> � 32�50 � have to pay the premium when trading options, assuming the options is now 260 points, in fact, we can only purchase �1260;000�;00050 � ¼ 76 lots. Although there are still some gaps between the 76 lots and the 122 lots with the best betting ratio, compared with the 6 lots and 24 lots with futures, the gap has been reduced a lot. 

### 3.4 The pseudo-codes of converting the original futures strategy to the options trading strategy 

In this section, the formal pseudo-codes of the proposed framework are proposed here. First of all, Algorithm 1 reads the original futures exchange tick data for this study and converts it into the required data (opening price per minute, highest price, lowest price, closing price, and trading volume). First, when the data are read in the proposed framework, it is divided into the data needed for the observation period, and we store it in total observe data. The data in the trading period are calculated through backtesting, and the tick transaction price during the trading period is temporarily stored for future strategy backtesting when used. After the observation period data are entered, standardization is required. The four data of opening, high, low, and closing are converted into percentages of change based on the opening of yesterday. The trading volume is based on the average trading per minute of the observation period in the past days (this study is set as the fifth trading day of the week) as a percentage to determine whether the trading volume has increased or decreased. 

Then we can bring in the planned strategy, such as entering the market at 09:15 A.M., stopping profit at 80, and stopping loss at 40. The data in the total price list during the trading period can be calculated. The first is the entry price. If the price touches the take-profit or stop-loss point, it will exit the market directly and calculate the profit and loss mark as the current day’s strategy profit and loss value between �1 and 1. 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23055 

- **Algorithm 1** Read futures data and standardized **Input:** _data_ , the futures data in a specific period. **Output:** _totalObserveData_ <u>,</u> daily observation <u>period</u> data. 

- 1: **for** each futures data _d_ (per day) in data **do** 2: _observeData_ = Filter out the observation period time and transaction target from _d_ and convert them to _O_ , _H_ , _L_ , _C_ , _V_ . 

- 3: _totalObserveData ≪ observeData ▷_ Save this day’s data into the daily observation period data 

- 4: _priceList_ = Transaction price trend during the trading period 5: _totalPriceList ≪ totalPriceList ▷_ Save today’s data into daily trading period data 6: **end for** 7: _averageDays_ = 5 _▷_ Standardized days 8: **for** _i_ from averageDays to the length of the data period **do** 9: _totalObserveData_ [ _i_ ]<sup>_O_</sup> = � _totalObserveData_ [ _i_ ]<sup>_O_</sup> _− priceList_ [ _i_ ]<sup>_C_�</sup> _÷ priceList_ [ _i_ ]<sup>_C_</sup> 

- 10: _totalObserveData_ [ _i_ ]<sup>_H_</sup> = � _totalObserveData_ [ _i_ ]<sup>_H_</sup> _− priceList_ [ _i_ ]<sup>_C_�</sup> _÷ priceList_ [ _i_ ]<sup>_C_</sup> 11: _totalObserveData_ [ _i_ ]<sup>_L_</sup> = � _totalObserveData_ [ _i_ ]<sup>_L_</sup> _− priceList_ [ _i_ ]<sup>_C_�</sup> _÷ priceList_ [ _i_ ]<sup>_C_</sup> 12: _totalObserveData_ [ _i_ ]<sup>_C_</sup> = � _totalObserveData_ [ _i_ ]<sup>_C_</sup> _− priceList_ [ _i_ ]<sup>_C_�</sup> _÷ priceList_ [ _i_ ]<sup>_C_</sup> 13: _meanVolume_ = average transaction volume in the preivous _averageDays_ days 14: _totalObserveData_ [ _i_ ]<sup>_V_</sup> = � _totalObserveData_ [ _i_ ]<sup>_V_</sup> _− meanV olume_ � _÷ meanV olume_ 

- 15: **end for** 

- 16: **return** _totalObserveData_ 

O, H, L, C, and V indicate the opening price, highest price, lowest price, closing price, and trading volume separately. The output data in Algorithm 1 are the trend of O, H, L, C depending on the previous closing price and the average volume for each averageDays days. 

After processing the training set data, bring it into the LSTM model to calculate the winning rate and expected value of the daily execution strategy, and then 

backtest and calculate the daily part size and profit and loss results. However, due to the margin system of futures trading, the calculated Kelly ratio may not be executed accurately. If the original margin is insufficient, the total assets will be used to calculate the profit and loss results. The detailed pseudo-codes are provided in Algorithm 2. 

**Algorithm 2** Backtesting the historical <u>performance</u> of futures 

**Input:** _data_ , the testing futures data in a specific period, _s_ , the strategy experimentation function (a trained LSTM network), _k_ , the daily Kelly fractioin function, _sp_ , the pre-defined stop-loss points, _fw_ , the pre-defined futures worth, _om_ , the pre-defined futures original margin, _p_ , the daily profit function and _a_ , the current assets. **Output:** _assetArray_ <u>,</u> daily assets record and _<u>positionArray</u>_ <u>,</u> daily <u>positions</u> record. 1: set _assetArray_ = _{}_ 2: set _positionArray_ = _{}_ 3: **for** each futures data _d_ (per day) in _data_ **do** 4: _postion_ = 0 5: **if** _s_ ( _d_ ) _>_ 0 **then** 6: _postion_ = _⌊a × k_ ( _today_ ) _÷_ ( _−sp × fw_ ) _⌋_ 7: **if** ( _position × om_ ) _> a_ **then** 8: _position_ = _⌊a ÷ om⌋_ 9: **end if** 10: _a_ = _a_ + ( _p_ ( _today_ ) _× position × fw_ ) 11: **end if** 12: _assetArray ≪ a_ 13: _positionArray ≪ postion_ 14: **end for** 15: **return** _assetArray_ and _positionArray_ 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23056 

|**Al**|**gorithm 3** Backtesting the historical performance of options|
|---|---|
|**In**|**put:**<br>_data_, the testing options data in a specific period,<br>_s_, the strategy experimentation function (a trained LSTM network),<br>_k_, the daily Kelly fractioin function,<br>_sp_, the pre-defined stop-loss points,<br>_ow_, the pre-defined options worth,<br>_a_, the current assets.|
|**O**|**utput:**<br>_assetArray_, daily assets record and _positionArray_, daily positions record.|
|1:|_bpArray_ =_{}_<br>_▷_the record of buying price of options|
|2:|_spArray_ =_{}_<br>_▷_the record of selling price of options|
|3:|**for** each options data _d_ (per day) in _data_ **do**<br>_▷_initial the two price record arrays|
|4:|select the options _o_ whose strike price is most close the current price and less than<br>the current price|
|5:|set _bp_ is the buying price of _o_<br>_▷_by the pre-defined strategy|
|6:|set _sp_ is the selling price of _o_<br>_▷_by the pre-defined strategy|
|7:|_bpArray ≪bp_<br>|
|8:|_spArray ≪sp_|
|9:|**end for**|
|10:|set _assetArray_ =_{}_|
|11:|set _positionArray_ =_{}_|
|12:|**for** each options data _d_ (per day) in _data_ **do**|
|13:|_postion_= 0|
|14:|**if** _s_(_d_)_>_0 **then**|
|15:|_postion_=_⌊a × k_(_today_)_÷_(_−sp × ow_)_⌋_|
|16:|**if** (_position × bpArray_[_today_])_> a_ **then**|
|17:|_position_=_⌊a ÷_(_bpArray_[_today_]_× ow_)_⌋_|
|18:|**end if**|
|19:|_a_=_a_+ ((_spArray_[_today_]_−bpArray_[_today_])_× position × ow_)|
|20:|**end if**|
|21:|_assetArray ≪a_|
|22:|_positionArray ≪postion_|
|23:|**end for**|
|24:|**return** _assetArray_ and _positionArray_|



Moreover, the pseudo-codes of the converted options trading strategy are shown in Algorithm 3. However, because of the nature of the option, in addition to being divided into call and put, there are also different performance prices to choose from. In this thesis, we only consider the options whose strike price is most close the current price as the target. There may still be a gap between the same ideal position size and the actual tradable position size, so the actual position size of the bet due to insufficient funds on hand should also be considered. 

## 4 Experimental results 

In this section, different experiments are compared to show the effectiveness of the proposed method. The experimentation will establish different frameworks with long or short strategies, different take-profits, stop-loss points, and various training set lengths. The futures and options transaction data used in the experiments were downloaded from the Taiwan Futures Exchange official website, and the period starts from January 02, 2018 to June 28, 2020. The target products are Taiwan Stock Index Futures (TX) and 

Taiwan Stock Index Options (TXO). All of the experiments are implemented by Python and performed on a Mid-2014 MacBook Pro with macOS Catalina OS. It has 2.2 GHz quad-core Intel Core i7 CPU, 16GB 1600 MHz DDR3 Ram and Intel Iris Pro 1536MB GPU. As the original futures strategy is established, the proposed LSTM framework is used to estimate the winning rate of the strategy and calculate the optimal betting position by Kelly criterion. Next, the result will be converted to an options trading strategy, which is used to compare with the original futures trading strategy. Assume the limitation of the initial budget is 1,000,000 New Taiwan Dollar (NTD). Here, the discussion focuses on the different profits of a strategy applying in the futures market and options market, therefore it ignores the transaction fee and the tax to simplify the experimental environment. The difference between the actual trading position and the ideal position size by the long and short futures day trading strategy will be shown in the following subsection. The influence of the length of the training period will be provided in the last of this section. 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23057 

### 4.1 Convert futures trading strategies into options strategies 

In this subsection, the description of the transferring process and the performance of the proposed frameworks are shown. It will be divided into two parts. The strategy transferred from the long futures day trading strategy is shown in the first part, and the strategy moved from the short futures day trading strategy is shown in the second part separately. Due to the limitation of the budget, the following experiments will show the ideal positions and actual positions in futures and options markets. 

#### 4.1.1 Convert the long futures day trading strategy 

First of all, an example is given to describe a futures strategy. If it is a long trading strategy and sets 80 points as take-profit and 40 points as stop-loss, the observation period is 08:45 A.M. to 09:15 A.M., and 09:15 A.M. to 10:00 A.M. is the trading period. And the trading time is 09:15 A.M if the strategy decides to make a transaction. During the trading period, if the price achieves the takeprofit/stop-loss points or the trading period is up, the strategy will close the position and calculate the profit and loss. In the following LSTM training process, the period of the training and validation set is set from January 01, 2018 to February 29, 2020, and the period of the testing data is set from March 01, 2020 to June 28, 2020, to backtest the transaction performance. Figure 18 shows the state of the loss values at each epoch in the process of training LSTM. The batch size is set as 8 and patience is set as 30. If training results no longer improve in 30 epoch, the training will be interrupted, and the max value of the total epoch is 633 rounds. 

In Fig. 18, the proposed process labels the winning rate for each transaction in the testing data and puts it into the 



Fig. 18 Loss value of training LSTM model of longing futures at 80 points take-profit and 40 points stop-loss 



Fig. 19 The ideal and actual positions of longing the futures at 80 points take-profit and 40 points stop-loss 

LSTM model as the input data. The optimal position, therefore, can be calculated by the output results from the LSTM model. Nonetheless, a sufficient extra margin for futures leverage trading is required. It, thus, is hard to reach the optimal position size in the real situation. The budget limits it; the actual transaction position could not reach the ideal position. As shown in Fig. 19, ideal position is the ideal position size, and actual transaction position is the position size that can be traded. The ideal position is average 7.69 times by the actual position. Among the 59 transactions, there are only three transactions that meet the ideal position size. Obviously, it is hard to achieve the ideal positions and cannot obtain a satisfied profit. 

Then the original futures longing trading strategy is converted into longing call options trading strategy according to the method proposed in Sect. 3. And the takeprofit and stop-loss points corresponding to the options strategy are calculated through the delta value. Bring the converted options strategy and the winning rate predicted by LSTM into the Kelly criterion to calculate the optimal position size as shown in Fig. 20. The results show that the actual tradable position size is 4.82 times smaller than the ideal position size and is closer to the ideal position size than the futures strategy. The results show that among the 59 trades of options trading after the actual conversion, although there are only three actual positions in line with the ideal position size. The actual position size is average 4.82 times worse than the perfect position size. However, comparing with the futures strategy, it is still closer to the ideal part size. It, therefore, has more potential to obtain a higher profit. 

Figure 21 shows the different quotients between the original futures strategy and the options strategy. It can be seen that the quotient of the options strategy is smaller than the original futures strategy, representing that the options strategy is closer to the ideal position size. Take the ideal positions calculated by the previous steps for backtesting 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23058 



Fig. 20 The ideal and actual positions of longing the call options based on original futures strategy longing at 80 points take-profit and 40 points stop-loss 

with the initial budget. Figure 22 shows the different asset changes between the futures strategy and the options strategy. It can be seen that during the backtesting of the original futures trading strategy, the minimum asset was 938,400 NTD, the highest asset came to 1,944,800 NTD, and the final asset was 1,738,600 NTD. On the other hand, the minimum options trading strategy was 780,400 NTD, the highest came to 5,919,400 NTD, and the final asset achieved 3,473,950 NTD. It means that the operable position of the options trading is closer to the ideal value obtained by the Kelly criterion. Due to the more significant positions can be invested, the fluctuation of asset is also more considerable. If the final profit and loss have a benefit, it will also be higher than the futures transaction. Table 1 reveals the different performances by taking the different take-profit and stop-loss points in the original futures strategy and the options strategy. Obviously, no matter what the values of take-profit and stop-loss points were set, the position gap between the ideal positions and actual positions in the options strategy is always smaller 



Fig. 21 Comparing the actual and ideal position size of the futures and options is shown in Fig. 21 



Fig. 22 Backtesting long futures strategy with 80 points take-profit and 40 points stop-loss and corresponding converted options strategy 

than in the futures strategy. That is to say, the options strategy can always put more positions than the futures strategy. It, thus, suffers more risk and causes the minimal value of the assets is smaller than the futures strategy. However, during the backtesting process, Kelly criterion showed excellent performance in the options strategy. The largest asset of the options strategy was much better than the futures strategy. Therefore, the Kelly criterion can maximize long-term profit growth. 

#### 4.1.2 Convert the short futures day trading strategy 

The previous section described how to convert the original long futures trading strategy to the call options strategy. In the market, it is in addition to going long and also need to consider the going short situation. As long as the required margin is deposited in futures trading, traders can choose to go long or short, but the options are divided into two types of call and put options. So if the original futures strategy is going short, it should be converted into longing put options and will get the same result. 

An example is also provided here; a short futures strategy sets at 80 points take-profit and 40 points stop-loss. And the observation period starts from 08:45 A.M. to 09:15 A.M. The trading period is 09:15 A.M. to 10:00 A.M. If the strategy decides to make a transaction today, then it will be at 09:15 A.M. During the trading period, if take-profit or stop-loss points are achieved or the trading period is times-up, the strategy will be forced to close out and calculate the profit and loss. Set the same parameters of the previous subsection and perform the LSTM model to reveal the ideal position. The experimental results are provided and discussed in Fig. 23. 

The results are similar to the previous subsection. Due to the limitation of the budget, the gaps between the actual position and the ideal position are also huge in the futures strategy. Figure 24 shows the ideal position size and actual 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23059 

Table 1 Comparison of longing futures trading strategy and converted call options strategy 

||Take-profit|Stop-loss|Epoch|Backtesting||||
|---|---|---|---|---|---|---|---|
|||||Max|Min|Final|Quotient|
|Futures|80|40|633|1,944,800|938,400|1,738,600|7.69|
|Options||||5,919,400|780,400|3,473,950|4.82|
|Futures|60|40|381|2,056,600|936,000|1,863,000|7.04|
|Options||||3,769,350|771,550|2,693,400|3.91|
|Futures|40|40|418|1,881,000|904,000|1,821,000|8.89|
|Futures||||2,896,250|842,450|2,762,550|6.29|
|Futures|60|20|418|1,461,600|952,000|1,409,400|22.57|
|Options||||1,314,900|533,150|1,144,700|12.68|
|Futures|40|20|393|1,448,000|976,000|1,412,000|22.45|
|Options||||2,654,050|847,250|2,654,050|14.76|
|Futures|20|20|289|1,024,000|952,000|952,000|31.49|
|Options||||1,177,450|755,350|910,450|19.25|





Fig. 23 Loss value of training LSTM model of shorting futures at 80 points take-profit and 40 points stop-loss 



Fig. 24 The ideal and actual positions of shorting the futures at 80 points take-profit and 40 points stop-loss 

position size. The average difference of the actual and ideal position sizes is 6.37 times. In this case, there are only 2 of the 54 tradings achieving the ideal position size. The 

trading strategy almost just decides the opportunity for doing a transaction. 

After predicting the winning rate through the LSTM model, the original short futures trading strategy can also be converted into a long put options trading strategy according to the method proposed in Sect. 3. The optimal options positions and the actual positions by the converted options trading strategy are shown in Fig. 25. It can be seen that although the actual operable positions still have a gap with the ideal positions, it is reduced to 2.90 times and closer to the size of the position required by the Kelly criterion than the original futures trading strategy. There are 12 out of all 55 tradings reaching the ideal position size. This number is obviously more than the same strategy applying in futures trading. That is to say, Kelly criterion can present more ability in the case of using the same strategy in options trading. 

Figure 26 shows the quotient of the ideal positions and the actual operating positions of the original futures trading strategy and the converted options trading strategy. It is the same as the long trading strategy, the options strategy has a better ability to provide similar positions with the positions calculated by Kelly formula. 

Then, the original short futures trading strategy and the converted options strategy also do backtesting for comparison, the results are shown in Fig. 27. The data shows that the futures strategy asset reaches a maximum of 1,108,400 NTD, the minimum value is 530,400 NTD, and the final asset is 600,000 NTD. However, for the options strategy, the highest asset reaches 1,481,200 NTD, the lowest is 556,250 NTD, and the final asset comes to 1,321,175 NTD. Apparently, the options strategy has much better performance than the original futures strategy. 

Next, we also tried to adjust the take-profit and stop-loss points of different original short futures strategy, compare 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23060 



Fig. 25 The ideal and actual positions of longing the put options based on original futures strategy shorting at 80 points take-profit and 40 points stop-loss 



Fig. 26 Comparison of the quotient of ideal and actual position size of futures and options of original short futures at 80 points take-profit and 40 points stop-loss and converted options strategy 



Fig. 27 Backtesting short futures strategy with 80 points take-profit and 40 points stop-loss and corresponding converted options strategy 

the difference between the actual operating and the ideal position size, and the performance during the backtesting is shown in Table 2. The results show that the gap between 

the actual operating and ideal position of the options is closer to the original futures strategy. Therefore, the method that converts the original futures strategy into the options strategy in this paper can do more ability to reliable money management. However, it can also be seen that in addition to the 80 points take-profit, 40 points stop-loss, and the 60 points take-profit, 20 points stop-loss, the converted options strategy has a benefit in backtesting performance and performs better than the original futures strategy. Instead of backtesting the performance of other group strategies, the options are losing. The representative said that if the LSTM model predicts a large winning rate at the beginning, the position size calculated according to the Kelly criterion will also be more extensive. If it gets a more considerable profit value when trading in the market, it will be relatively higher, for example, the original strategy shorts the futures at 80 points take-profit and 40 points stop-loss. Taking the original short futures at 60 points take-profit and 40 points stop-loss as examples, and the result is shown in Fig. 28. It can be seen that the futures and the converted options strategy are similar in trend, but because the position control of the options is closer to the ideal position size than the futures. When a loss is encountered, more money will be lost in the converted options strategy. In addition to whether the original strategy will benefit, the results of machine-learning predictions will also affect the final return. 

### 4.2 Comparison of different LSTM forget gate settings and training set length 

To use the Kelly criterion to calculate the best betting ratio and position size, in addition to the odds and winning rate. In the first two sections, we compared the difference between the final profit and loss results of different takeprofit points and stop-loss points. This chapter discusses adjusting the parameters of the forgetting gate and different training under the same take-profit points and stop-loss points. Set length, what is the difference between the results predicted by the LSTM training model. We divide the training set into three types: 1. long time—January 01, 2018 to February 29, 2020; 2. medium time—January 01, 2019 to February 29, 2020; 3. short time—July 01, 2019 to February 29, 2020. The LSTM model settings are the same as those in the previous two sections. The batch size is 8, the patience is 30, and the training is stopped when 30 consecutive strokes do not improve. The dropout parameters are changed to 0.4 and 0.6 in addition to the previous 0.2. The original futures strategy The same is to do the 80-point profit stop and 40-point stop loss. The initial capital is 1,000,000 dollars. The difference in the final backtest result is shown in Table 3. It can be seen that the 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23061 

Table 2 Comparison of shorting futures trading strategy and converted put options strategy 

||Take-profit|Stop-loss|Epoch|Backtesting||||
|---|---|---|---|---|---|---|---|
|||||Max|Min|Final|Quotient|
|Futures|80|40|530|1,108,400|530,400|600,000|6.37|
|Options||||1,481,200|556,250|1,321,175|2.90|
|Futures|60|40|491|1,060,000|600,800|763,400|6.76|
|Options||||1,235,500|215,225|262,350|3.07|
|Futures|40|40|508|1,280,000|794,000|834,000|8.58|
|Futures||||1,327,350|147,490|165,090|3.75|
|Futures|60|20|401|1,205,200|877,200|980,000|20.97|
|Options||||1,991,275|782,900|1,872,475|8.64|
|Futures|40|20|345|1,300,000|976,000|1,116,000|19.23|
|Options||||1,312,250|696,450|737,700|9.08|
|Futures|20|20|328|1,112,000|944,000|1,024,000|28.20|
|Options||||1,133,700|275,250|275,250|12.80|





Fig. 28 Backtesting short futures strategy with 60 points take-profit and 40 points stop-loss and corresponding converted options strategy 

position quotient that can be operated when converted to the options strategy is smaller than that of the original futures, which means that the street is close to the Kelly formula. It is easier to remember historical data when the dropout value is set to 0.2. Therefore, when the training set time is longer, the performance of the backtest of the long training set will also have greater benefits than the medium and short training period. As a result, the final asset of the long-term training set operation futures came to 1,738,600 dollars, the final asset of the operation options came to 3,473,950 dollars, while the short-term training set operation futures final asset was only 1,246,000 dollars, and the final asset of the operation options was only 1,706,950 dollars. When the dropout value is set to 0.6, it is easier to forget historical data, so we can see that the final performance results in a short time and a long time are similar. The final asset of the operation futures grows to 1.2 to 1.7 times, and the final asset of the operation options grows to 2.6 to 3.3 times. 

After comparing the long futures trading strategies, compare the situation when the original futures trading strategy is short. Table 4 shows the case of a short trading strategy with a profit stop at 80 points and a stop-loss at 40 points. The result of the experiment is roughly the same as that of long. When the dropout value is set smaller, the longer the training set time, the better the backtest performance. When the overall profit performance is low, and the predicted result is not good, it may also lead to more losses in the final profit and loss result. The results of these changes to the experimental parameters are limited to the conversion of Taiwan stock index futures to options trading strategies. If the transition to a different trading target has a more diversified original trading strategy, the most suitable parameters and training set length may be set not the same, need to do more experiments to find the best parameters. 

## 5 Conclusion and future research 

We propose a method of converting the original futures strategy into options trading strategy. In other words, even if the capital is limited, the same strategic logic as the original futures trading can be achieved by executing options trading. At the same time, in order to carry out better money management, the optimal fraction calculated by the Kelly criterion has traditionally been used. However, due to the problem of the futures trading system, it is difficult to apply the Kelly criterion on the margin system. If the original futures trading strategy is converted into longing call and put options through the framework proposed in this paper, it can be closer to the position size of the Kelly criterion and maximize the overall benefit growth. Although it is impossible to truly calculate the 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23062 

Table 3 Long original futures trading strategy 80 points take-profit and 40 points stop-loss and the converted long call strategy trading with different dropout value and training set length results 

||Dropout|Training set time|Testing set time|Epoch|Backtesting||||
|---|---|---|---|---|---|---|---|---|
||||||Max|Min|Final|Quotient|
|Futures|0.2|2018/01/01�2020/02/29|2020/03/01�2020/06/28|633|1,944,800|938,400|1,738,600|7.69|
|Options|||||5,919,400|780,400|3,473,950|4.82|
|Futures||2019/01/01�2020/02/29|2020/03/01�2020/06/28|497|1,461,000|847,000|1,315,400|6.29|
|Options|||||3,202,400|786,650|2,518,600|4.82|
|Futures||2019/07/01�2020/02/29|2020/03/01�2020/06/28|394|1,454,400|852,000|1,246,000|5.91|
|Options|||||3,214,850|697,800|1,706,950|3.71|
|Futures|0.4|2018/01/01�2020/02/29|2020/03/01�2020/06/28|496|2,005,000|959,200|1,800,200|6.69|
|Options|||||1,997,450|831,900|1,630,250|4.24|
|Futures||2019/01/01�2020/02/29|2020/03/01�2020/06/28|385|1,846,400|839,200|1,678,200|4.99|
|Options|||||5,377,500|718,450|4,327,750|3.06|
|Futures||2019/07/01�2020/02/29|2020/03/01�2020/06/28|500|1,684,400|842,000|1,463,200|8.73|
|Options|||||3,147,400|644,700|1,789,800|5.67|
|Futures|0.6|2018/01/01�2020/02/29|2020/03/01�2020/06/28|420|1,278,400|721,600|1,200,000|6.69|
|Options|||||3,183,850|707,750|2,784,700|4.33|
|Futures||2019/01/01�2020/02/29|2020/03/01�2020/06/28|418|1,832,200|862,000|1,643,800|6.49|
|Options|||||3,230,350|696,350|2,626,400|4.60|
|Futures||2019/07/01�2020/02/29|2020/03/01�2020/06/28|570|1,856,200|889,200|1,709,600|7.37|
|Options|||||3,959,450|722,600|3,333,800|4.64|



Table 4 Short original futures trading strategy 80 points take-profit and 40 points stop-loss and the converted long put strategy trading with different dropout value and training set length results 

||Dropout|Training set time|Testing set time|Epoch|Backtesting||||
|---|---|---|---|---|---|---|---|---|
||||||Max|Min|Final|Quotient|
|Futures|0.2|2018/01/01�2020/02/29|2020/03/01�2020/06/28|530|1,108,400|530,400|600,000|6.37|
|Options|||||1,481,200|556,250|1,321,175|2.90|
|Futures||2019/01/01�2020/02/29|2020/03/01�2020/06/28|448|1,082,800|689,400|753,600|6.12|
|Options|||||1,310,300|288,270|376,170|2.72|
|Futures||2019/07/01�2020/02/29|2020/03/01�2020/06/28|299|1,102,000|579,600|635,600|6.65|
|Options|||||1,359,700|76,680|94,530|2.96|
|Futures|0.4|2018/01/01�2020/02/29|2020/03/01�2020/06/28|406|1,051,600|513,600|519,600|5.83|
|Options|||||1,393,400|488,450|579,150|3.07|
|Futures||2019/01/01�2020/02/29|2020/03/01�2020/06/28|386|1,131,200|513,400|606,600|6.41|
|Options|||||1,401,300|192,440|252,865|2.85|
|Futures||2019/07/01�2020/02/29|2020/03/01�2020/06/28|427|1,214,000|651,600|669,400|7.13|
|Options|||||1,752,300|271,575|271,575|3.17|
|Futures|0.6|2018/01/01�2020/02/29|2020/03/01�2020/06/28|388|1,201,200|395,200|420,000|5.04|
|Options|||||1,482,150|128,010|151,760|2.82|
|Futures||2019/01/01�2020/02/29|2020/03/01�2020/06/28|276|1,243,600|458,400|525,600|3.38|
|Options|||||1,891,100|353,330|451,180|1.63|
|Futures||2019/07/01�2020/02/29|2020/03/01�2020/06/28|589|1,037,200|493,600|552,400|7.04|
|Options|||||1,127,700|149,925|210,000|3.00|



123 

Neural Computing and Applications (2025) 37:23047–23064 

23063 

winning rate of a trading strategy in the financial market, most of the past used the performance of backtesting as the winning rate. Still, the past does not represent the future, so we use the LSTM model to predict the winning percentage of the strategy. The experimental results also show that different training set lengths will also give different prediction results and cause different position sizes; the final profit and loss results are also different. If we can further optimize the prediction results in the winning rate, money management can be implemented better, and the final performance can be further superior. Future research not only can improve the prediction accuracy of machine learning but also other futures trading strategies that are currently known to benefit can be converted to options trading strategies based on the framework proposed in this paper. At the same time, if the position is properly controlled in combination with the theory of money management, the growth in profit may be better than the past. 

Acknowledgements The authors are grateful to King Saud University, Riyadh, Saudi Arabia for funding this work through Researchers Supporting Project Number RSP-2020/18. This research is also partially supported by Ministry of Education, Taiwan and Ministry of Science and Technology, Taiwan under Grant no. MOST 107-2221E-027-104-MY2 and MOST 109-2622-E-027-008-CC3. 

Funding Open access funding provided by Universita` della Calabria within the CRUI-CARE Agreement. 

### Compliance with ethical standards 

Conflict of interest The authors declare that they have no conflict of interest. 

Open Access This article is licensed under a Creative Commons Attribution 4.0 International License, which permits use, sharing, adaptation, distribution and reproduction in any medium or format, as long as you give appropriate credit to the original author(s) and the source, provide a link to the Creative Commons licence, and indicate if changes were made. The images or other third party material in this article are included in the article’s Creative Commons licence, unless indicated otherwise in a credit line to the material. If material is not included in the article’s Creative Commons licence and your intended use is not permitted by statutory regulation or exceeds the permitted use, you will need to obtain permission directly from the copyright holder. To view a copy of this licence, visit http://creativecommons. org/licenses/by/4.0/. 

## References 

1. Abe M, Nakayama H (2018) Deep learning for forecasting stock returns in the cross-section. In: Pacific-Asia conference on knowledge discovery and data mining, pp 273–284. Springer 

2. Akita R, Yoshihara A, Matsubara T, Uehara K (2016) Deep learning for stock prediction using numerical and textual information. In: 2016 IEEE/ACIS 15th international conference on computer and information science (ICIS), pp 1–6. IEEE 

3. Bengio Y, Simard P, Frasconi P (1994) Learning long-term dependencies with gradient descent is difficult. IEEE Trans Neural Netw 5(2):157–166 

4. Casadei R, Fortino G, Pianini D, Russo W, Savaglio C, Viroli M (2019) Modelling and simulation of opportunistic IoT services with aggregate computing. Future Gener Comput Syst 91:252–262 

5. Chen K, Zhou Y, Dai F (2015) A LSTM-based method for stock returns prediction: a case study of China stock market. In: 2015 IEEE international conference on big data (big data), pp 2823–2824. IEEE 

6. Conforti R, La Rosa M, Fortino G, Ter Hofstede AH, Recker J, Adams M (2013) Real-time risk monitoring in business processes: a sensor-based approach. J Syst Softw 86(11):2939–2965 

7. Dash R, Dash PK (2016) A hybrid stock trading framework integrating technical analysis with machine learning techniques. J Finance Data Sci 2(1):42–57 

8. Ding X, Zhang Y, Liu T, Duan J (2015) Deep learning for eventdriven stock prediction. In: Twenty-fourth international joint conference on artificial intelligence 

9. Ferdous J, Mollah MP, Razzaque MA, Hassan MM, Alamri A, Fortino G, Zhou M (2017) Optimal dynamic pricing for tradingoff user utility and operator profit in smart grid. IEEE Trans Syst Man Cybern Syst 50:455–467 

10. Fong WM, Yong LH (2005) Chasing trends: recursive moving average trading rules and internet stocks. J Empir Finance 12(1):43–76 

11. Fortino G, Russo W, Savaglio C, Shen W, Zhou M (2017) Agentoriented cooperative smart objects: from IoT system design to implementation. IEEE Trans Syst Man Cybern Syst 48(11):1939–1956 

12. Go¨c¸ken M, O<sup>¨</sup> zc¸alıcı M, Boru A, Dosdog˘ru AT (2016) Integrating metaheuristics and artificial neural networks for improved stock price prediction. Expert Syst Appl 44:320–331 

13. Gunasekarage A, Power DM (2001) The profitability of moving average trading rules in South Asian stock markets. Emerg Mark Rev 2(1):17–33 

14. Hochreiter S, Schmidhuber J (1997) Long short-term memory. Neural Comput 9(8):1735–1780 

15. Holmberg U, Lo¨nnbark C, Lundstro¨m C (2013) Assessing the profitability of intraday opening range breakout strategies. Finance Res Lett 10(1):27–33 

16. Hsieh CH, Barmish BR, Gubner JA (2016) Kelly betting can be too conservative. In: 2016 IEEE 55th conference on decision and control (CDC), pp 3695–3701. IEEE 

17. Kelly JL Jr (2011) A new interpretation of information rate. In: The Kelly capital growth investment criterion: theory and practice, pp 25–34. World Scientific 

18. Kilgallen T (2012) Testing the simple moving average across commodities, global stock indices, and currencies. J Wealth Manag 15(1):82–100 

19. Lee TK, Cho JH, Kwon DS, Sohn SY (2019) Global stock market investment strategies based on financial network indicators using machine learning techniques. Expert Syst Appl 117:228–242 

20. MacLean LC, Thorp EO, Ziemba WT (2010) Good and bad properties of the Kelly criterion. Risk 20(2):1 

21. Moghaddam AH, Moghaddam MH, Esfandyari M (2016) Stock market index prediction using artificial neural network. J Econ Finance Admin Sci 21(41):89–93 

22. Naik N, Mohan BR (2019) Study of stock return predictions using recurrent neural networks with LSTM. In: International conference on engineering applications of neural networks, pp 453–459. Springer 

23. Ndikum P (2020) Machine learning algorithms for financial asset price forecasting. arXiv:2004.01504 

123 

Neural Computing and Applications (2025) 37:23047–23064 

23064 

24. Nelson DM, Pereira AC, de Oliveira RA (2017) Stock market’s price movement prediction with LSTM neural networks. In: 2017 international joint conference on neural networks (IJCNN), pp 1419–1426. IEEE 

25. Patel J, Shah S, Thakkar P, Kotecha K (2015) Predicting stock and stock price index movement using trend deterministic data preparation and machine learning techniques. Expert Syst Appl 42(1):259–268 

26. Peng Y, Jiang H (2015) Leverage financial news to predict stock price movements using word embeddings and deep neural networks. arXiv:1506.07220 

27. Roondiwala M, Patel H, Varma S (2017) Predicting stock prices using LSTM. Int J Sci Res (IJSR) 6(4):1754–1756 

28. Selvin S, Vinayakumar R, Gopalakrishnan E, Menon VK, Soman K (2017) Stock price prediction using LSTM, RNN and CNNsliding window model. In: 2017 international conference on advances in computing, communications and informatics (ICACCI), pp 1643–1647. IEEE 

29. Singh R, Srivastava S (2017) Stock prediction using deep learning. Multimed Tools Appl 76(18):18569–18584 

30. Stutzer M (2011) On growth-optimality vs. security against underperformance. In: The Kelly capital growth investment criterion: theory and practice, pp 641–653. World Scientific 

31. Sun Z, Zhao S (2019) A review of the application of machine learning in stock investment. In: 4th international conference on humanities science, management and education technology (HSMET 2019). Atlantis Press 

32. The´ate T, Ernst D (2020) An application of deep reinforcement learning to algorithmic trading. arXiv:2004.06627 

33. Thorp EO (2011) The Kelly criterion in blackjack sports betting, and the stock market. In: The Kelly capital growth investment criterion: theory and practice, pp 789–832. World Scientific 

34. Thorp EO (2011) Understanding the Kelly criterion. In: The Kelly capital growth investment criterion: theory and practice, pp 509–523. World Scientific 

35. Tsai YC, Wu ME, Syu JH, Lei CL, Wu CS, Ho JM, Wang CJ (2019) Assessing the profitability of timely opening range breakout on index futures markets. IEEE Access 7:32061–32071 

36. Vince R (1992) The mathematics of money management: risk analysis techniques for traders, vol 18. Wiley, Hoboken 

37. Vince R (1995) The new money management: a framework for asset allocation, vol 47. Wiley, Hoboken 

38. Vince R (2009) The leverage space trading model: reconciling portfolio management strategies and economic theory, vol 425. Wiley, Hoboken 

39. Wu ME, Chung WH (2018) A novel approach of option portfolio construction using the Kelly criterion. IEEE Access 6:53044–53052 

40. Wu ME, Hung PJ (2018) A framework of option buy-side strategy with simple index futures trading based on Kelly criterion. In: 2018 5th international conference on behavioral, economic, and socio-cultural computing (BESC), pp 210–212. IEEE 

41. Wu ME, Tsai HH, Tso R, Weng CY (2015) An adaptive Kelly betting strategy for finite repeated games. In: International conference on genetic and evolutionary computing, pp 39–46. Springer 

42. Wu ME, Wang CH, Chung WH (2017) Using trading mechanisms to investigate large futures data and their implications to market trends. Soft Comput 21(11):2821–2834 

43. Wu ME, Wang CH, Chung WH, Tso R, Yang IH (2015) An empirical comparison between Kelly criterion and Vince’s optimal f. In: 2015 IEEE international conference on smart city/ SocialCom/SustainCom (SmartCity), pp 806–810. IEEE 

44. Zahedi J, Rounaghi MM (2015) Application of artificial neural network models and principal component analysis method in predicting stock prices on Tehran stock exchange. Physica A 438:178–187 

45. Zhong X, Enke D (2019) Predicting the daily return direction of the stock market using hybrid machine learning algorithms. Financ Innov 5(1):4 

46. Zhuge Q, Xu L, Zhang G (2017) LSTM neural network with emotional analysis for prediction of stock price. Eng Lett 25(2):1–9 

Publisher’s Note Springer Nature remains neutral with regard to jurisdictional claims in published maps and institutional affiliations. 

123 

