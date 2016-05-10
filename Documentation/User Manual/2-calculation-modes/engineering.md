##Engineering Mode

The Engineering Mode lets the user define every aspect in the components of the vehicle and the driving cycle. This is for experimenting and validation purposes.

In this mode the given list of job files is simulated with the respective driving cycles. Each job file defines a separate vehicle.

<div class="vecto2">
This is the default calculation mode in VECTO V2.
</div>
<div class="vecto3">
In VectoCMD V3.x the default mode is Declaration Mode.
</div>

###Requirements

-   One or more checked job files in the Job List
-   Each job file must include at least one driving cycle

###Results

-   Modal results (.vmod). One file for each vehicle/cycle combination.
-   Sum results (.vsum). One file for each invocation of VECTO.


###Options
The Driving Cycle determines the simulation method in engineering mode. The option depends directly on the driving cycle input and cannot be set explicitely. For more information about the formats see [Driving Cycles](#driving-cycles).

* [Target speed, distance-based](#engineering-mode-target-speed-distance-based-cycle)
:   This option is the a target vehicle speed distance based cycle (like in Declaration Mode). With this option experiments can be made by the manufacturer.
* [Measured speed, time-based](#engineering-mode-measured-speed-time-based-cycle)
:   Driving Mode where the actual speed from measurements is simulated.
* [Measured speed with gear, time-based](#engineering-mode-measured-speed-with-gear-time-based-cycle)
:   Driving Mode where the actual speed from measurements is simulated. Also defines the chosen gear.
* [Pwheel (SiCo) Mode, time-based](#engineering-mode-pwheel-sico-time-based)
:   In Pwheel mode the measured power at the wheels is given, and the simulation takes that as input.





