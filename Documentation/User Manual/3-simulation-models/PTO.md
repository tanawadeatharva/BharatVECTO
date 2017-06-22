##PTO

VECTO supports the simulation of PTO related components and losses in the powertrain. Structurally this consists of 2 components (PTO transmission, PTO consumer) and 3 different kind of losses (transmission, idling, cycle).

### Structural Overview of PTO Components
![](pics/pto.png)

#### Losses in the PTO "Transmission" part (blue)

This is considered by constant power consumption as a function of the PTO type. The power consumption is added in all vehicle operation conditions, due to VECTO not differentiating between clutch open/closed and gear engaged/disengaged. The PTO type is configurable in the [Vehicle Editor](#vehicle-editor). The exact values are shown in the following table:


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

#### Idling losses of the PTO "Consumer" (red)

The idling losses are a function of speed as determined by the DIN 30752-1 procedure. If the PTO transmission includes a shifting element (i.e. declutching of consumer part possible) the torque losses of the consumer in VECTO input shall be defined with zero. This is only used outside of PTO cycles, since the PTO cycles already include these losses. The idling losses are defined as a lossmap dependend on speed which is configurable in the [Vehicle Editor](#vehicle-editor). The file format is described in [PTO-Consumer](#pto-consumer).


#### Cycle losses during the PTO cycle of the PTO "Consumer" (red)

A specific PTO cycle (time-based, engine speed and torque from PTO consumer as determined by the DIN 30752-1 procedure) is simulated during vehicle stops labelled as "with PTO activation". The execution of the driving cycle stops during this time and the pto cycle is executed. Afterwards the normal driving cycle continues.

Power consumption in the PTO transmission part added to power demand from the PTO cycle. The cycle is configurable in the [Vehicle Editor](#vehicle-editor) and follows the file format described in [PTO-Cycle](#pto-cycle). The timings in the PTO cycle get shifted to start at 0.


### Behavior During PTO Driving Cycles

A PTO cycle can only be activated during a stop phase in the driving cycle. When the PTO cycle is activated VECTO exhibits the following behavior: Half of the stop time is added before the pto cycle, and the other half is added afterwards. If the halved stop times are still longer than 3 seconds, they get divided even further to 3 intervals in order to achieve a more appealing visualization in the output (falling down, low baseline, rising again).

The following image shows the behavior of running PTO cycles during a normal driving cycle:


![](pics/pto-behavior.png)

(#) Normal driving behavior.
(#) The first half of the stop phase begins, the vehicle stops and the engine speed goes down to idle speed (if there is enough time).
(#) The PTO cycle continues from the last engine speed in stop phase and sets it to the engine speed of the first entry in the PTO cycle.
(#) After the PTO cycle ends, the second half of the stop phase begins and the engine speed again goes to idle speed (if enough time passes).
(#) After the stop phase the normal driving behavior starts again - the vehicle drives off.
