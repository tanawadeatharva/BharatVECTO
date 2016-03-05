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


###Options###
The option depends on the driving cycle and cannot be chosen explicitely. For more information see [Driving Cycles](#driving-cycles).

- Target Speed
- Measured Speed
- Measured Speed with Gear
- Pwheel (SiCo)


