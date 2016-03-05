##Acceleration Limiting Input File (.vacc)

The file is used for [Acceleration Limiting](#acceleration-limiting). It defines the acceleration and deceleration limits as function of
vehicle speed. The filepath has to be defined in the [Job File](#job-file). The file uses the [VECTO CSV format](#csv).

- Filetype: .vacc
- Header: **v \[km/h], acc \[m/s^2], dec \[m/s^2]**
    + **v [km/h]**: the vehicle speed. Must be >= 0 km/h.
    + **acc [m/s^2]**: the maximum acceleration. Must be > 0 km/h.
    + **dec [m/s^2]**: the maximum deceleration. Must be < 0 km/h.
- Requires at least 2 data entries
- Data should cover the whole possible range of vehicle speeds

**Example Data:**

    v [km/h], acc [m/s^2]      , dec [m/s^2]
    0       , 1.01570922360353, -0.231742702878269
    5       , 1.38546581120225, -0.45346198022574
    10      , 1.34993329755465, -0.565404125020508
    15      , 1.29026714002479, -0.703434814668512
    ...


**Example Graph:**

![](pics/AccLimit.png)


