## Powertrain and Components Structure
The powertrain in Vecto V3 consists of the following components which are generally connected in this order:

![](pics/powertrain.svg)

The engine tries to supply the requested power demand (including all power losses happening in the powertrain and auxiliaries).
If the engine can't supply the given power demand, the driver reduces the  accelerating.


### Powertrain Values

The powertrain can be configured to represent different situations depending on the used retarder and gearbox configuration. 
The output values in the [Modfile](#modal-results-.vmod) depict different points in the powertrain depending on the current configuration. 
Here are some schematic overviews which show the values and the position in the powertrain they represent:

* [AMT Transmission Input Retarder](#amt-transmission-input-retarder)
* [AMT Transmission Output Retarder](#amt-transmission-output-retarder)
* [AT Transmission Input Retarder](#at-transmission-input-retarder)
* [AT Transmission Output Retarder](#at-transmission-output-retarder)
* [Axle](#axle)

### AMT Transmission Input Retarder

![](pics/AMT-TransmissionInputRetarder.svg)

### AMT Transmission Output Retarder

![](pics/AMT-TransmissionOutputRetarder.svg)

### AT Transmission Input Retarder

![](pics/AT-TransmissionInputRetarder.svg)

### AT Transmission Output Retarder

![](pics/AT-TransmissionOutputRetarder.svg)

### Axle

![](pics/Axle.svg)

