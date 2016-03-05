##Acceleration Limiting Input File (.vacc)

The file is used for [Acceleration Limiting](#acceleration-limiting). It defines the acceleration and deceleration limits as function of
vehicle speed. The filepath has to be defined in the [Job File](#job-editor). The file uses the [VECTO CSV format](#csv).

![](pics/AccLimit.png)

- Filetype: .vacc
- Header: **vehicle speed \[km/h\], Max. acceleration \[m/s^2^\], Max. deceleration \[m/s^2^\]**
- Requires at least 2 data entries

**Example:**

    v [km/h],acc [m/s²],dec [m/s²]
    0,1.01570922360353,-0.231742702878269
    5,1.38546581120225,-0.45346198022574
    10,1.34993329755465,-0.565404125020508
    15,1.29026714002479,-0.703434814668512
    ...
