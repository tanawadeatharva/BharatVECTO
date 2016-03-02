##Driving Cycle (.vdri)

The Driving Cycle defines vehicle speed, road gradient and other parameters for a test route. It is either time-based or distance-based and defines a simulated route the vehicle should be tested with.
The format is [Vecto-CSV](#csv-format) and the columns depend on the cycle type:

- **Declaration Mode**: [Target speed, time-based, predefined!](#declaration-mode-cycle)
- **Engineering Mode**:
	- [Target speed, distance-based](#distance-based-cycle)
	- [Measured speed, time-based](#measured-speed-cycle-without-gear)
	- [Measured speed with gear, time-based](#measured-speed-cycle-with-gear-and-engine-speed)
	- [Pwheel (SiCo) Mode, time-based](#pwheel-sico-mode-cycle)
- **Engine Only Mode**: [Engine Only Mode, time-based](#engine-only-mode-cycle)

**Important Notes:**

- Distance-based cycles must have at least a resolution of 1[m].
- Time-based cycles can be defined in any given time resolution, including variable time steps. If the time column ("\<t\>") is missing, the data will be interpreted to have a 1[s] resolution.

###Declaration Mode Cycle
Cycles are always distance-based and are predefined default cycles depending on vehicle category. They represent a typical usage cycle of the corresponding vehicle category. Depending on the vehicle category, up two 3 different cycles could be chosen for a simulation.

- Long Haul - 100km
- Regional Delivery - 26km
- Urban Delivery - 28km
- Coach - 275km
- Urban - 40km
- Sub Urban - 23km
- Inter Urban - 123km
- Heavy Urban - 30km
- Municipal Utility - 10km
- Construction - 21km


###Engineering Mode Driving Cycles
Cycles can be defined in [Job](#job-editor). A job is time based if the column \<t\> is defined. A job is distance based if the column \<s\> is defined. If both are defined, the time column is prefered (=> time-based). If none are defined, the entries will be interpreted with 1Hz (=> time-based).

####Distance-Based Cycle

Header: **\<s\>**,**\<v\>**,*[\<grad\>]*,**\<stop\>**,**\<Padd\>** *[,\<vair\_res\>,\<vair\_beta\>\]\[,\<Aux\_xxx\>\]*

| Identifier         | Quantity                      | Unit     | Description
| ----------         | --------                      | ------   | ------------------------------------------------
| **\<s\>**          | Distance                      | [m]      | Travelled distance.
| **\<v\>**          | Vehicle Speed                 | [km/h]   | The target vehicle velocity.
| *\<grad\>*         | Road Gradient                 | [%]      | Optional.
| **\<stop\>**       | Stopping Time                 | [s]      | Required for distance-based cycles. Not used in time based cycles. \<stop\> defines the time the vehicle spends in stop phases.
| **\<Padd\>**       | Additional Aux Power Demand   | [kW]     | This power input will be directly added to the engine power in addition to possible other auxiliaries..
| *\<vair_res\>*     | Air speed relative to vehicle | [km/h]   | Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.
| *\<vair_beta\>*    | Wind Yaw Angle                | [°]      | Only required if [**Cross Wind Correction**](#cross-wind-correction) is set to **Vair & Beta Input**.
| *\<Aux_xxx\>*      | Auxiliary Supply Power        | [kW]     | Supply Power input for each auxiliary defined in the [.vecto file](#job-editor) where xxx matches the ID of the corresponding [Auxiliary](#auxiliary-dialog). ID's are not case sensitive and must not contain space or special characters.

**\<bold\>** identifiers are required, *\<italic\>* are optional.

**Example:**

|\<s\>|\<v\>|\<grad\>|\<stop\>|\<Aux_Alt\>|\<Aux_Demo\>
| - | - | ---- | ---- | ------- | ---------
| 0 | 60 | 2.95016969 | 0 | 0.5 | 4.752
| 1 | 60 | 2.95016969 | 0 | 0.5 | 4.752
| 2 | 60 | 2.95016969 | 0 | 0.5 | 4.752
| 3 | 60 | 2.95016969 | 0 | 0.5 | 4.752



###Engineering Mode: Measured Speed

####Measured Speed Cycle Without Gear
This cycle type defines the measured vehicle velocity and the road gradient. It uses the built-in shift strategy of vecto to determine the current gear and the resulting engine power. If the cross wind correction for the [Vehicle](#vehicle-editor) is set to "Vair & Beta" the columns <vair_res> and <vair_beta> are required.

Header: **\<t\>,\<v\>,\<grad\>,\<Padd\>** *\[,\<vair\_res\>,\<vair\_beta\>\]\[,\<Aux\_xxx\>\]*

| Identifier      | Quantity                      | Unit     | Description
| ----------      | --------                      | ------   | ------------------------------------------------
| **\<t\>**       | Time                          | [s]      | The absolute time.
| **\<v\>**       | Vehicle Speed                 | [km/h]   | The actual velocity of the vehicle.
| **\<grad\>**    | Road Gradient                 | [%]      | The road gradient.
| **\<Padd\>**    | Additional Aux Power Demand   | [kW]     | This power input will be directly added to the engine power.
| *\<vair_res\>*  | Air speed relative to vehicle | [km/h]   | Only required if [Cross Wind Correction](#cross-wind-correction) in [Vehicle](#vehicle-editor) is set to **Vair & Beta Input**.
| *\<vair_beta\>* | Wind Yaw Angle                | [°]      | Only required if [Cross Wind Correction](#cross-wind-correction) in [Vehicle](#vehicle-editor) is set to **Vair & Beta Input**.
| *\<Aux\_xxx\>*  | Auxiliary Supply Power        | [kW]     | Power input for each auxiliary defined in the [.vecto file](#job-editor) where "xxx" matches the ID of the corresponding [Auxiliary](#auxiliary-dialog). ID's are not case sensitive and may consist of word-characters only (A-Z, a-z, 0-9).

**\<bold\>** identifiers are required, *\<italic\>* are optional.


####Measured Speed Cycle With Gear and Engine Speed
This cycle type defines the measured vehicle velocity, the road gradient and the gear and engine speed. It overrides the built-in shift strategy. From the vehicle speed and gear the needed torque can be calculated, and together with the engine speed the resulting engine power which is used for fuel consumption can be calculated. If the cross wind correction for the [Vehicle](#vehicle-editor) is set to "Vair & Beta" the columns \<vair_res\> and \<vair_beta\> are required.

Header: **\<t\>,\<v\>,\<grad\>,\<Padd\>,\<n\>,\<gear\>** *\[,\<vair\_res\>,\<vair\_beta\>\]\[,\<Aux\_xxx\>\]*

| Identifier      | Quantity                      | Unit     | Description
| ----------      | --------                      | ------   | ------------------------------------------------
| **\<t\>**       | Time                          | [s]      | The absolute time.
| **\<v\>**       | Vehicle Speed                 | [km/h]   | The actual velocity of the vehicle.
| **\<grad\>**    | Road Gradient                 | [%]      | The road gradient.
| **\<Padd\>**    | Additional Aux Power Demand   | [kW]     | This power input will be directly added to the engine power.
| **\<n\>**       | Engine Speed                  | [rpm]    | The engine speed (will always be maintained - even when \<v\> cannot be reached in the simulation)
| **\<gear\>**    | Gear                          | [-]      | The current gear
| *\<vair_res\>*  | Air speed relative to vehicle | [km/h]   | Only required if [Cross Wind Correction](#cross-wind-correction) in [Vehicle](#vehicle-editor) is set to **Vair & Beta Input**.
| *\<vair_beta\>* | Wind Yaw Angle                | [°]      | Only required if [Cross Wind Correction](#cross-wind-correction) in [Vehicle](#vehicle-editor) is set to **Vair & Beta Input**.
| *\<Aux\_xxx\>*  | Auxiliary Supply Power        | [kW]     | Power input for each auxiliary defined in the [.vecto file](#job-editor) where "xxx" matches the ID of the corresponding [Auxiliary](#auxiliary-dialog). ID's are not case sensitive and may consist of word-characters only (A-Z, a-z, 0-9).

**\<bold\>** identifiers are required, *\<italic\>* are optional.


###Pwheel (SiCo) Mode Cycle

Power measured at the 



Header: **\<t\>**,**\<Pwheel\>**,**\<gear\>**,**\<n\>**,**\<Padd\>**

| Identifier      | Quantity                      | Unit     | Description
| ----------      | --------                      | ------   | ------------------------------------------------
| **\<t\>**       | Time                          | [s]      | The absolute time.
| **\<Pwheel\>**  | Wheel Power                   | [kW]     | Power at the wheels.
| **\<gear\>**    | Gear                          | [-]      | The current gear
| **\<n\>**       | Engine Speed                  | [rpm]    | The engine speed (will always be maintained - even when \<v\> cannot be reached in the simulation)
| **\<Padd\>**    | Additional Aux Power Demand   | [kW]     | This power input will be directly added to the engine power.


###Engine Only Mode Cycle
Either power or torque must be defined.

Header: **\<t\>**,**\<n\>**,**[\<Pe\>|\<Me\>]**,**\<Padd\>**

| Identifier      | Quantity                      | Unit     | Description
| ----------      | --------                      | ------   | ------------------------------------------------
| **\<t\>**       | Time                          | [s]      | The absolute time.
| **\<n\>**       | Engine Speed                  | [km/h]   | The engine speed.
| **\<Pe\>**      | Engine Power                  | [kW]     | The power consumption of the engine. (either power or torque must be defined)
| **\<Me\>**      | Engine Torque                 | [Nm]     | The torque of the engine. (either power or torque must be defined)
| **\<Padd\>**    | Additional Aux Power Demand   | [kW]     | This power input will be directly added to the engine power.
