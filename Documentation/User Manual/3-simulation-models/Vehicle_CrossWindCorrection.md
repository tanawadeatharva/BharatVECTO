##Vehicle: Cross Wind Correction


VECTO offers three different modes to consider cross wind influence on the drag coefficient. It is configured in the [Vehicle File](#vehicle-file).


###Speed dependent correction (Declaration Mode)


This is the mode which is used in [Declaration Mode](#declaration-mode). The speed dependent C~d~A curve (see below) is calculated based on generic parameters for each vehicle class and the C~d~A value from the [Vehicle File](#vehicle-file).


###Speed dependent correction (User-defined)
The base C~d~A value (see [Vehicle File](#vehicle-file)) is corrected with a user-defined speed dependent scaling function. A [vcdv-File](#speed-dependent-cross-wind-correction-input-file-.vcdv) is needed for this calculation.

The C~d~A value given in the vehicle configuration is corrected depending on the vehicle's speed and the C~d~ scaling factor from the input file as follows:

$C_dA_{effective} = C_dA * C_d(v_{veh})$

 ![](pics/VCDV.png)


###Correction using Vair & Beta Input

The actual (measured) air speed and direction can be used to correct cross-wid influence if available. A [vcdb-File](#vair-beta-cross-wind-correction-input-file-.vcdb) is needed for this calculation. This file defines a ΔC~d~A value in \[m²\] depending on the wind angle. The [driving cycle](#driving-cycles) must include the air speed relative to the vehicle v~air~ (\<vair\_res\>) and the wind yaw angle (\<vair\_beta\>).

The C~d~A value given in the vehicle configuration is corrected depending on the wind speed and wind angle (given in the driving cycle) using the input file as follows:

$C_dA_{effective} = C_dA + {\Delta}C_d(\beta)$

 ![](pics/VCDB.png)
