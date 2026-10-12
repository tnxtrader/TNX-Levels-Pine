### Page 25 table 1

|Col1|S&P 500|Col3|Col4|
|---|---|---|---|
|Measures|Mean<br>Variance|Min|Max|
|_r_2|1.3136<br>11.8238|0.0000|67.8305|
|_RV_|1.6332<br>3.5078|0.0766|33.2162|
|_RV_ 1|1.4429<br>3.2324|0.0714|30.7842|
|_RV_ 2|1.3536<br>2.8899|0.0635|25.2236|
|_RV_ 3|1.3076<br>2.8392|0.0572|26.3249|
||JPY-USD|||
||Mean<br>Variance|Min|Max|
|_r_2|0.5253<br>2.0089|0.0000|52.9709|
|_RV_|0.6160<br>0.5963|0.0316|32.5053|
|_RV_ 1|0.5801<br>0.7541|0.0244|39.7883|
|_RV_ 2|0.5622<br>0.8262|0.0212|42.9220|
|_RV_ 3|0.5491<br>0.8473|0.0184|43.6814|
||DEM-USD|||
||Mean<br>Variance|Min|Max|
|_r_2|0.4744<br>0.8366|0.0000|12.4406|
|_RV_|0.5645<br>0.3157|0.0347|15.3277|
|_RV_ 1|0.5298<br>0.3234|0.0341|14.9701|
|_RV_ 2|0.5122<br>0.3270|0.0297|14.9052|
|_RV_ 3|0.4987<br>0.3244|0.0280|14.8933|



### Page 26 table 1

|JPY USD<br>-|Col2|Col3|Regressors|Col5|Col6|
|---|---|---|---|---|---|
||_RV_|_RPV_ (1)|_RBP_|Range|Squared Return|
|log(_PL_)|-845.32|-845.12|-856.87|-863.47|-997.31|
|RMSE|0.4918|0.4864|0.4949|0.5025|0.5620|
|MAE|0.3659|0.3601|0.3684|0.3851|0.4285|
|_R_2|0.5419|0.5594|0.5498|0.5193|0.4141|
|DEM-USD|||Regressors|Regressors||
||_RV_|_RPV_ (1_._5)|_RBP_|Range|Squared Return|
|log(_PL_)|-754.71|-752.04|-748.06|-765.03|-872.84|
|RMSE|0.4467|0.4450|0.4437|0.4521|0.4937|
|MAE|0.3346|0.3374|0.3311|0.3889|0.3729|
|_R_2|0.3954|0.3988|0.3997|0.3996|0.3085|



### Page 28 table 1

|Col1|Col2|log(PL)|A : S&P 500|Col5|RMSE|Col7|
|---|---|---|---|---|---|---|
|Model|_h_ = 1|_h_ = 5|_h_ = 10|_h_ = 1|_h_ = 5|_h_ = 10|
|AR(5)|-535.9|-332.0|-341.2|0.4865|0.3652|0.3757|
|AR(10)|-529.1|-316.0|-320.2|0.4819|0.3564|0.3635|
|AR(15)|-527.0|-310.1|-316.8|0.4803|0.3523|0.3610|
|HAR-log|-534.4|-320.2|-331.7|0.4855|0.3595|0.3715|
|SMA|-537.0|-319.0|-317.5|0.4862|0.3545|0.3597|
|BMA|-527.7|-304.4|-300.7|0.4808|0.3518|0.3545|
|||log(_PL_)|B: JPY-USD|B: JPY-USD|RMSE||
|Model|_h_ = 1|_h_ = 5|_h_ = 10|_h_ = 1|_h_ = 5|_h_ = 10|
|AR(5)|-847.6|-675.4|-693.2|0.4955|0.4293|0.4429|
|AR(10)|-844.6|-666.4|-675.2|0.4943|0.4257|0.4355|
|AR(15)|-845.6|-661.0|-664.3|0.4947|0.4232|0.4311|
|HAR-log|-846.6|-659.4|-661.5|0.4950|0.4224|0.4303|
|SMA|-838.1|-663.8|-671.5|0.4904|0.4220|0.4324|
|BMA|-836.6|-652.5|-648.9|0.4899|0.4198|0.4258|
|||log(_PL_)|C: DEM-USD|C: DEM-USD|RMSE||
|Model|_h_ = 1|_h_ = 5|_h_ = 10|_h_ = 1|_h_ = 5|_h_ = 10|
|AR(5)|-749.7|-493.3|-422.0|0.4456|0.3565|0.3345|
|AR(10)|-745.6|-476.3|-400.0|0.4447|0.3514|0.3275|
|AR(15)|-743.4|-472.1|-394.5|0.4436|0.3497|0.3253|
|HAR-log|-748.9|-483.6|-410.2|0.4455|0.3535|0.3307|
|SMA|-750.6|-494.5|-422.5|0.4415|0.3514|0.3287|
|BMA|-742.1|-475.0|-397.7|0.4423|0.3514|0.3279|



### Page 29 table 1

|Col1|h 1<br>=|Col3|h 5<br>=|Col5|h 10<br>=|Col7|
|---|---|---|---|---|---|---|
|Obs|log(_PL_)|RMSE|log(_PL_)|RMSE|log(_PL_)|RMSE|
|0|-743.1|0.4426|-476.9|0.3514|-407.5|0.3308|
|200|-741.5|0.4419|-475.3|0.3515|-397.7|0.3279|
|500|-742.1|0.4423|-475.0|0.3514|-397.7|0.3279|
|1000|-742.4|0.4424|-474.3|0.3511|-397.4|0.3282|



### Page 29 table 2

|Col1|A : S&P 500|Col3|
|---|---|---|
|Regressors|_h_ = 1<br>_h_ = 5<br>log(_PL_)<br>Rank<br>log(_PL_)<br>Rank|_h_ = 10<br>log(_PL_)<br>Rank|
|RV only|-527.0<br>2<br>-310.1<br>13|-316.8<br>38|
|With RPV|-526.0<br>1<br>-302.2<br>1|-299.1<br>1|
|With RBP|-527.9<br>4<br>-310.2<br>14|-310.5<br>18|
||B: JPY-USD||
|Regressors|_h_ = 1<br>_h_ = 5<br>log(_PL_)<br>Rank<br>log(_PL_)<br>Rank|_h_ = 10<br>log(_PL_)<br>Rank|
|RV only|-844.6<br>40<br>-659.4<br>22|-661.5<br>22|
|With RPV|-835.6<br>1<br>-648.0<br>1|-647.9<br>1|
|With RBP|-840.1<br>17<br>-653.0<br>11|-654.4<br>13|



### Page 29 table 3

|Col1|C DEM -USD<br>:|Col3|Col4|
|---|---|---|---|
|Regressors|_h_ = 1<br>log(_PL_)<br>Rank|_h_ = 5<br>log(_PL_)<br>Rank|_h_ = 10<br>log(_PL_)<br>Rank|
|RV only|-743.4<br>18|-472.1<br>1|-394.5<br>1|
|With RPV|-704.3<br>1|-475.3<br>2|-397.7<br>2|
|With RBP|-741.0<br>4|-476.1<br>4|-400.2<br>5|



### Page 30 table 1

|A : S&P 500|= = = h 1 h h 5 10|Model cation Model cation Model cation S S S pecifi pecifi pecifi log(PL) log(PL) log(PL)|: =1 : =2 .2 : .1 AR RV =10 -526 .0 HAR RV =3 -302 AR =15 -299 , , PV( .5) PV( .5) PV( .5) : : : AR RV =15 -527 .0 HAR RV =3 =3 -303 .6 AR =10 -299 .6 , PV( .5) PV( .5) : =1 : : AR RV =10 -527 .0 AR RV =10 =5 -304 .5 HAR RV =3 =3 -300 .6 , , , PV(1) PV( .5) PV( .5) : =1 : =2 : =1 AR RV =10 RBP -527 .9 HAR RV =3 -305 .9 HAR RV =3 -300 .8 , , , PV( .5) PV( .5) : =1 : : AR RV =10 528 .5 AR RV =10 =5 -306 .8 AR RV =10 =5 -300 .9 , , , PV(1 .5) PV(1) PV( .5)|- B : JPY USD|= = = h 1 h h 5 10|Model cation Model cation Model cation S S S pecifi pecifi pecifi log(PL) log(PL) log(PL)|: : =2 : =2 AR RV =10 =5 −835 .6 HAR RV =3 −648 .0 HAR RV =3 −647 .9 , , , PV(1) PV(1) PV(1) : =1 : .4 : AR RV =10 −836 .0 HAR RV =3 =3 −649 HAR RV =3 =3 −648 .6 , , , PV(1) PV(1) PV(1) : =2 : =2 : =1 HAR RV =3 −836 .5 HAR RV =3 −649 .6 HAR RV =3 −649 .8 , , , PV(1) PV(1) PV(1 .5) : =2 : =2 .1 : =2 HAR RV =3 −836 .5 HAR RV =3 −650 HAR RV =3 −649 .8 , , , PV(1) PV(1 .5) PV(1 .5) : : =2 : =2 .1 HAR RV =3 =3 −836 .6 HAR RV =3 −650 .3 HAR RV =3 −650 , , , PV(1) PV(1 .5) PV(1 .5)|: DEM C -USD|= = = h 1 h h 5 10|Model cation Model cation Model cation S S S pecifi pecifi pecifi log(PL) log(PL) log(PL)|: =2 : .2 : HAR RV =3 −740 .3 AR RV =15 −472 AR RV =15 −394 .5 , PV(1) : =2 : : HAR RV =3 −740 .3 AR RV =10 =5 −475 .3 AR RV =10 =5 −397 .7 , , , PV(1 .5) PV( .5) PV( .5) : =2 : =1 : =1 HAR RV =3 −741 .0 AR RV =10 −476 .0 AR RV =10 −398 .7 , , , PV(1) PV( .5) PV( .5) : =2 : =1 .1 : HAR RV RBP =3 −741 .0 HAR RV RBP =3 −476 AR RV =10 −400 .0 , , : =2 .1 : : =1 .2 HAR RV =3 −741 AR RV =10 −476 .3 AR RV =10 RBP −400 , , PV(1 .5)|. This table lists the models for different horizons and across markets The odd columns are the best models to 5 top predictive according . likelihood and even columns the likelihood present predictive log (log(PL)) . models are denoted with and models are denoted with a model then a HAR “HAR” AR “AR” Given HAR prefix prefix type type type -log 1 2 . If indicates a term a and term and a and term for that measure model 3 AR , , , daily daily weekly daily weekly monthly volatility type . the number of of each is listed For means the of this model include “HAR: RV =3 =3” HAR , , example type lags regressor regressors PV( .5) . all the and of and of order means of and of RV PV 0 .5 “AR: RV =10 =5” 10 5 , components daily weekly monthly lags lags ,PV(0 .5) log(RV t) . All . models include a and all models include an term S&P 500 component jump asymmetric log(RPV t(0 .5))|
|---|---|---|---|---|---|---|---|---|---|---|---|---|



### Page 31 table 1

|Model|S&P 500|JPY USD<br>-|DEM USD<br>-|
|---|---|---|---|
|AR(5)|-530.0|-815.7|-717.3|
|AR(10)|-523.3|-814.9|-715.7|
|AR(15)|-521.1|-815.4|-712.9|
|HAR-log|-527.1|-813.1|-714.1|
|BMA|-522.8|-808.9|-706.4|


