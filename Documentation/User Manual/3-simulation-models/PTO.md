##PTO

Here the simulation of PTO related fuel consumption in VECTO is described.

![](pics/pto.png)

The PTO related influence on the fuel consumption is considered in VECTO as follows:

### Losses in the PTO "Transmission" part (blue)

Considered by constant power consumption as a function of the PTO type. Power consumption added in all vehicle operation conditions as VECTO does not differentiate between clutch open/closed and gear engaged/disengaged. Configurable in the [Vehicle Editor](#vehicle-editor). The values are displayed in the following table:


|                                        Technology                                        | Power Loss \[W] |
|------------------------------------------------------------------------------------------|-----------------|
| None                                                                                     |               0 |
| only the drive shaft of the PTO - shift claw, synchronizer, sliding gearwheel            |              50 |
| only the drive shaft of the PTO - multi-disc clutch                                      |            1000 |
| only the drive shaft of the PTO - multi-disc clutch, oil pump                            |            2000 |
| drive shaft and/or up to 2 gear wheels - shift claw, synchronizer, sliding gearwheel     |             300 |
| drive shaft and/or up to 2 gear wheels - multi-disc clutch                               |            1500 |
| drive shaft and/or up to 2 gear wheels - multi-disc clutch, oil pump                     |            3000 |
| drive shaft and/or more than 2 gear wheels - shift claw, synchronizer, sliding gearwheel |             600 |
| drive shaft and/or more than 2 gear wheels - multi-disc clutch                           |            2000 |
| drive shaft and/or more than 2 gear wheels - multi-disc clutch, oil pump                 |            4000 |

### Idling losses of the PTO "Consumer" (red)

Torque loss as a function of speed as determined by the DIN 30752-1 procedure. If PTO transmission includes shifting element (i.e. declutching of consumer part possible) the torque losses of the consumer in VECTO input shall be defined with zero. Not relevant during PTO cycle. Configurable in the [Vehicle Editor](#vehicle-editor) and following the file format described in [PTO-Consumer](#pto-consumer).


### Power consumption of the PTO "Consumer" (red) during the PTO cycle

A specific PTO cycle (time-based, engine speed and torque from PTO consumer as determined by the DIN 30752-1 procedure) is simulated during vehicle stops additionally labelled as “with PTO activation”. Power consumption in the PTO transmission part added to power demand from the PTO cycle. The cycle is configurable in the [Vehicle Editor](#vehicle-editor) and follows the file format described in [PTO-Cycle](#pto-cycle).


