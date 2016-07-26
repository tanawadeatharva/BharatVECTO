##Driving Cycles

A Driving Cycle defines the parameters of a simulated route in Vecto. It is either time-based or distance-based and has different fields depending on the driving cycle type.
The basic file format is [Vecto-CSV](#csv) and the file type ending is ".vdri". A Job must have at least one driving cycle (except in Declaration mode, where the driving cycles are predefined).

###Driving Cycle Types
- **Declaration Mode**: [Target speed, distance-based](#declaration-mode-cycles)
- **Engineering Mode**:
	- [Target speed, distance-based](#engineering-mode-target-speed-distance-based-cycle)
	- [Measured speed, time-based](#engineering-mode-measured-speed-time-based-cycle)
	- [Measured speed with gear, time-based](#engineering-mode-measured-speed-with-gear-time-based-cycle)
	- [Pwheel (SiCo) Mode, time-based](#engineering-mode-pwheel-sico-time-based)
- **Engine Only Mode**: [Engine Only Mode, time-based](#engine-only-mode-engine-only-driving-cycle)

<div class="vecto2">
- Distance-based cycles must have at least a resolution of 1[m].
- Time-based cycles must have exactly a resolution of 1[s].
</div>

<div class="vecto3">
- Distance-based cycles can be defined in any distance resolution, including variable distance steps.
- Time-based cycles can be defined in any time resolution, including variable time steps.
</div>

###Declaration Mode Cycles
In Declaration Mode driving cycles are automatically chosen depending on vehicle category and cannot be changed by the user. These predefined cycles are of type target-speed, distance-based.

- Coach: 275km
- Construction: 21km
- Heavy Urban: 30km
- Inter Urban: 123km
- Long Haul: 100km
- Municipal Utility: 10km
- Regional Delivery: 26km
- Sub Urban: 23km
- Urban: 40km
- Urban Delivery: 28km

###Engineering Mode: Target-Speed, Distance-Based Cycle
This driving cycle defines the target speed over distance. Vecto tries to achieve and maintain this target speed.

Header: **\<s>, \<v>, \<stop>***\[, \<Padd>]\[, \<grad>]\[, \<vair\_res>, \<vair\_beta>]\[, \<Aux\_ID>]*

|  Identifier |  Unit  |                                                                                                                                      Description                                                                                                                                      |
| ----------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **s**       | [m]    | Traveled distance. Must always be increasing.                                                                                                                                                                                                                                         |
| **v**       | [km/h] | The target vehicle velocity.  Must be >= 0 km/h.                                                                                                                                                                                                                                      |
| **stop**    | [s]    | Stopping Time. Defines the time span the vehicle is standing still (time the vehicle spending in a stop phase). After this time, the vehicle tries to accelerate to \<v>.                                                                                                             |
| *Padd*      | [kW]   | Additional auxiliary power demand. This power demand will be directly added to the engine power in addition to possible other auxiliaries. Must be >= 0 kW.                                                                                                                                            |
| *grad*      | [%]    | The road gradient.                                                                                                                                                                                                                                                                    |
| *vair_res*  | [km/h] | Air speed relative to vehicle for cross wind correction. Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                        |
| *vair_beta* | [°]    | Wind Yaw Angle for cross wind correction. Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                       |
| *Aux_ID*    | [kW]   | Auxiliary Supply Power. Can be defined multiple times with different Identifiers. The supply power input for each auxiliary defined in the [.vecto file](#job-file) with the corresponding ID. ID's are not case sensitive and must only contain letters and numbers [a-z,A-Z,0-9]. Must be >= 0 kW. |

**Example:**

| \<s> [m] | \<v> [km/h] | \<stop> [s] | \<grad> [%] | \<Padd> [kW] |
| -------- | ----------- | ----------- | ----------- | ------------ |
|        0 |          10 |          10 |        2.95 |          1.5 |
|        1 |          20 |           0 |        2.97 |          1.3 |
|        2 |          35 |           0 |        3.03 |          1.3 |
|        3 |          50 |           0 |        2.99 |          1.3 |

###Engineering Mode: Measured-Speed, Time-Based Cycle
This driving cycle defines the actual measured speed over time. Vecto tries to simulate the vehicle model using this speed as the actual vehicle speed.
Due to differences in the real and simulated shift strategies a short difference in speed could occur, but Vecto immediately tries to catch up after the gear is engaged again.

Header: **\<t>, \<v>***\[, \<grad>]\[, \<Padd>]\[, \<vair\_res>, \<vair\_beta>\]\[, \<Aux\_ID>]*

|  Identifier |  Unit  |                                                                                                                                              Description                                                                                                                                               |
| ----------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **t**       | [s]    | The absolute time. Must always be increasing.                                                                                                                                                                                                                                                          |
| **v**       | [km/h] | The actual velocity of the vehicle. Must be >= 0 km/h.                                                                                                                                                                                                                                                 |
| *Padd*      | [kW]   | Additional auxiliary power demand. This power demand will be directly added to the engine power in addition to possible other auxiliaries. Must be >= 0 kW.                                                                                                                                            |
| *grad*      | [%]    | The road gradient.                                                                                                                                                                                                                                                                                     |
| *vair_res*  | [km/h] | Air speed relative to vehicle for cross wind correction. Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                         |
| *vair_beta* | [°]    | Wind Yaw Angle for cross wind correction. Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                                        |
| *Aux_ID*    | [kW]   | Auxiliary Supply Power. Can be defined multiple times with different Identifiers. The supply power input for each auxiliary defined in the [.vecto file](#job-editor) with the corresponding ID. ID's are not case sensitive and must only contain letters and numbers [a-z,A-Z,0-9]. Must be >= 0 kW. |

**Example:**

| \<t> [s] | \<v> [km/h] | \<grad> [%] | \<Padd> [kW] |
| -------- | ----------- | ----------- | ------------ |
|        0 |           0 |        2.95 |          1.5 |
|        1 |         0.6 |        2.97 |          1.3 |
|        2 |         1.2 |        3.03 |          1.3 |
|        3 |         2.4 |        2.99 |          1.3 |


###Engineering Mode: Measured-Speed With Gear, Time-Based Cycle
This driving cycle defines the actual measured speed of the vehicle, the gear, and the engine speed over time.
It overrides the shift strategy of Vecto and also directly sets the engine speed.

<div class="vecto2">
It is necessary to set the option 'Use gears/rpm\'s from driving cycle in the **Options** tab.

![](pics/MeasuredSpeedSettings.png)
</div>

Header: **\<t>, \<v>, \<n>, \<gear>***\[, \<grad>]\[, \<Padd>]\[, \<vair\_res>, \<vair\_beta>]\[, \<Aux\_ID>\]*

|  Identifier |  Unit  |                                                                                                                                              Description                                                                                                                                               |
| ----------- | ------ | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| **t**       | [s]    | The absolute time. Must always be increasing.                                                                                                                                                                                                                                                          |
| **v**       | [km/h] | The actual velocity of the vehicle. Must be >= 0 km/h.                                                                                                                                                                                                                                                 |
| **n**       | [rpm]  | The actual engine speed. Must always be >= 0 rpm.                                                                                                                                                                                                                                                      |
| **gear**    | [-]    | The current gear. Must be >= 0 (0 is neutral).                                                                                                                                                                                                                                                         |
| *Padd*      | [kW]   | Additional auxiliary power demand. This power demand will be directly added to the engine power in addition to possible other auxiliaries. Must be >= 0 kW.                                                                                                                                            |
| *grad*      | [%]    | The road gradient.                                                                                                                                                                                                                                                                                     |
| *vair_res*  | [km/h] | Air speed relative to vehicle for cross wind correction. Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                         |
| *vair_beta* | [°]    | Wind Yaw Angle for cross wind correction. Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.                                                                                                                                                        |
| *Aux_ID*    | [kW]   | Auxiliary Supply Power. Can be defined multiple times with different Identifiers. The supply power input for each auxiliary defined in the [.vecto file](#job-editor) with the corresponding ID. ID's are not case sensitive and must only contain letters and numbers [a-z,A-Z,0-9]. Must be >= 0 kW. |

**Example:**

| \<t> [s] | \<v> [km/h] | \<n> [rpm] | \<gear> [-] | \<grad> [%] | \<Padd> [kW] |
| -------- | ----------- | ---------- | ----------- | ----------- | ------------ |
|        0 |           0 |        600 |           0 |        2.95 |          1.5 |
|        1 |         0.6 |        950 |           3 |        2.97 |          1.3 |
|        2 |         1.2 |       1200 |           3 |        3.03 |          1.3 |
|        3 |         2.4 |       1400 |           3 |        2.99 |          1.3 |

###Engineering Mode: Pwheel (SiCo), Time-Based
This driving cycle defines the power measured at the wheels over time. Vecto tries to simulate the vehicle with this power requirement.

Header: **\<t>, \<Pwheel>, \<gear>, \<n>***\[, \<Padd>]*

| Identifier |  Unit |                      Quantity                                                    Description                      |
| ---------- | ----- | ----------------------------------------------------------------------------------------------------------------- |
| **t**      | [s]   | The absolute time. Must always be increasing.                                                                     |
| **Pwheel** | [kW]  | Power at the wheels.                                                                                              |
| **gear**   | [-]   | The current gear. Must be >= 0 (0 is neutral).                                                                    |
| **n**      | [rpm] | The actual engine speed. Must be >= 0 rpm.                                                                        |
| *Padd*     | [kW]  | Additional auxiliary power demand. This power demand will be directly added to the engine power. Must be >= 0 kW. |

**Example:**

| \<t> [s] | \<Pwheel> [kW] | \<gear> [-] | \<n> [rpm] | \<Padd> [kW] |
| -------- | -------------- | ----------- | ---------- | ------------ |
|        0 |              0 |           0 |        600 |          1.5 |
|        1 |          4.003 |           3 |        950 |          1.3 |
|        2 |         15.333 |           3 |       1200 |          1.3 |
|        3 |          50.56 |           3 |       1400 |          1.3 |


###Engine Only Mode: Engine Only Driving Cycle
This driving cycle directly defines the power or torque at the output shaft of the engine over time. Vecto add the engine's inertia to the given power demand and simulates the engine.

Header: **\<t>, \<n>, (\<Pe>|\<Me>)***\[, \<Padd>]*

| Identifier |  Unit |                                                    Description                                                    |
| ---------- | ----- | ----------------------------------------------------------------------------------------------------------------- |
| **t**      | [s]   | The absolute time. Must always be increasing.                                                                     |
| **n**      | [rpm] | The actual engine speed. Must be >= 0 rpm.                                                                        |
| **Pe**     | [kW]  | The power at the output shaft of the engine. Either \<Pe> or \<Me> must be defined.                               |
| **Me**     | [Nm]  | The torque at the output shaft of the engine. Either \<Pe> or \<Me> must be defined.                              |
| *Padd*     | [kW]  | Additional auxiliary power demand. This power demand will be directly added to the engine power. Must be >= 0 kW. |

**Example:**

| \<t> [s] | \<n> [rpm] | \<Pe> [kW] | \<Padd> [kW] |
| -------- | ---------- | ---------- | ------------ |
|        0 |        600 |          0 |          1.5 |
|        1 |        950 |       25.3 |          1.3 |
|        2 |       1200 |     65.344 |          1.3 |
|        3 |       1400 |      110.1 |          1.3 |

<div class="vecto2"
>To explicitly define *motoring operation* use the **\<DRAG>** keyword as power demand (column \<Pe> or \<Me>). VECTO v2 replaces the keyword with the corresponding motoring torque/power from the drag curve during calculation (see [Full Load and Drag Curve File](#full-load-and-drag-curves-.vfld)).
</div>
