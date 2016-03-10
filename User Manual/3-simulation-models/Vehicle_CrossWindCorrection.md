##Cross Wind Correction


VECTO offers three different modes to consider cross wind influence on the drag coefficient. It is configured in the [Vehicle File](#vehicle-file).


###Speed dependent correction (Declaration Mode)


This is the default mode which is used in [Declaration Mode](#declaration-mode). The speed dependent c~d~ x A curve (see below) is calculated based on generic parameters for each vehicle class and the base c~d~ x A value from the [Vehicle File](#vehicle-file).



###Speed dependent correction (User-defined)
The base c~d~ x A value (see [Vehicle File](#vehicle-file)) is corrected with a user-defined speed dependent scaling function. The input file (.vcdv) format is described [here](#speed-dependent-cross-wind-correction-input-file-.vcdv).

The CdxA value given in the vehicle configuration is corrected depending on the vehicle's speed and the CD scaling factor from the input file as follows:

C~d~xA~effective~ = CdxA * Cd(v_veh)

 ![](pics/VCDV.png)


###Correction using Vair & Beta Input

The actual (measured) air speed and direction can be used to correct cross-wid influence if available. The input file (.vcdb) defines delta C~d~xA in square meters depending on the wind speed and wind angle. The input file (.vcdb) format is described [here](#vair-beta-cross-wind-correction-input-file-.vcdb). The [driving cycle](#driving-cycles) must include the air speed relative to the vehicle v~air~ (&lt;vair\_res&gt;) and the wind yaw angle (&lt;vair\_beta&gt;).

The CdxA value given in the vehicle configuration is corrected depending on the wind speed and wind angle (given in the driving cycle) using the input file as follows:

C~d~xA~effective~ = CdxA + delta-Cd(beta)

 ![](pics/VCDB.png)
