##Battery Internal Voltage File (.vbatv)

This file contains the battery's internal voltage as function of the state of charge (SoC). The file must cover the SOC range from 0 to 100%! The file uses the [VECTO CSV format](#csv).

- Filetype: .vbatv
- Header: **SOC, V**
- Requires at least 2 data entries


**Example:**

~~~
SOC , V
0   , 590
10  , 614
20  , 626
30  , 634
40  , 638
50  , 640
60  , 640
70  , 642
80  , 646
90  , 650
100 , 658
~~~


##Battery Internal Resistance File (.vbatr)

This file contains the battery's internal resistance as function of the state of charge (SoC). The file must cover the SOC range from 0 to 100%! The file uses the [VECTO CSV format](#csv).

- Filetype: .vbatr
- Header: **SOC, R**
- Requires at least 2 data entries


**Example:**

~~~
SoC , Ri
0   , 0.04
100 , 0.04
~~~