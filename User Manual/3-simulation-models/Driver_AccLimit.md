##Acceleration Limiting

To model a realistic driver behavior, VECTO limits the vehicle acceleration and deceleration according to speed-dependent limits. These limits are defined in the [Acceleration Limiting Input File (.vacc)](#acceleration-limiting-input-file-.vacc), which is defined in the [Job File](#job-file).

* If the engine can't provide the required power, the vehicle might accelerate slower than the defined driver limit.
* The minimum deceleration can always be maintained via the brakes.
* In [Measured Speed Mode](#engineering-mode-measured-speed) this limits are not used, due to the nature of this mode (speeds and accelerations are already real measured values, therefore VECTO uses these directly).

![](pics/AccLimit.png)

The Image shows the acceleration and deceleration limits depending on the current vehicle speed.
