##Modal Results (.vmod)

Modal results are only created if enabled in the [Options](#main-form) tab. One file is created for each calculation and stored in the same directory as the .vecto file.

<div class="vecto2">
***Quantities:***

| **Name**        | **Unit** | **Description** |
| --------------- | ---------------------- | -------------------------- |
| time            | [s]      | Time step. |
| dist            | [km]     | Travelled distance. |
| v_act           | [km/h]   | Actual vehicle speed. |
| v_targ          | [km/h]   | Target vehicle speed. |
| acc             | [m/s²]   | Vehicle acceleration. |
| grad            | [%]      | Road gradient. |
| n               | [1/min]  | Engine speed. |
| Tq_eng          | [Nm]     | Engine torque. |
| Tq_clutch       | [Nm]     | Torque at clutch (before clutch, engine-side) |
| Tq_full         | [Nm]     | Full load torque |
| Tq_drag         | [Nm]     | Motoring torque |
| Pe_eng          | [kW]     | Engine power. |
| Pe_full         | [kW]     | Engine full load power. |
| Pe_drag         | [kW]     | Engine drag power. |
| Pe_clutch       | [kW]     | Engine power at clutch (equals **Pe** minus loss due to rotational inertia **Pa Eng**). |
| Gear            | [-]      | Gear. "0" = clutch opened / neutral.|
| Ploss GB        | [kW]     | Gearbox losses. |
| Ploss Diff      | [kW]     | Losses in differential / axle transmission. |
| Ploss Retrader  | [kW]     | Retarder losses. |
| Pa Eng          | [kW]     | Rotational acceleration power: Engine. |
| Pa GB           | [kW]     | Rotational acceleration power: Gearbox. |
| Pa Veh          | [kW]     | Vehicle acceleration power. |
| Proll           | [kW]     | Rolling resistance power demand. |
| Pair            | [kW]     | Air resistance power demand. |
| Pgrad           | [kW]     | Power demand due to road gradient. |
| Paux            | [kW]     | Total auxiliary power demand. |
| Pwheel          | [kW]     | Total power demand at wheel = sum of rolling, air, acceleration and road gradient resistance. |
| Pbrake          | [kW]     | Brake power. Drag power is included in **Pe**. |
| Paux_xxx        | [kW]     | Power demand of Auxiliary with ID xxx. See also [Aux Dialog](#auxiliary-dialog) and [Driving Cycle](#driving-cycles). |
| FC-Map          | [g/h]    | Fuel consumption interpolated from FC map. |
| FC-AUXc         | [g/h]    | Fuel consumption after [Auxiliary-Start/Stop Correction](#fuel-consumption-calculation). (Based on FC.) |
| FC-WHTCc        | [g/h]    | Fuel consumption after [WHTC Correction](#fuel-consumption-calculation). (Based on FC-AUXc.) |
| TCv             | [-]      | Torque converter speed ratio |
| TCµ             | [-]      | Torque converter torque ratio |
| TC_M_Out        | [Nm]     | Torque converter output torque |
| TC_n_Out        | [1/min]  | Torque converter output speed |
</div>

<div class="vecto3">
In Vecto 3.0.2 the structure of the modal data output has been revised and re-structured. Basicaly for every powertrain component the .vmod file contains the power at the input shaft and the individual power losses for every component. For the engine the power, torque and engine speed at the output shaft is given along with the internal power and torque used for computing the fuel consumption. The following table lists the columns in the .vmod file:
	
***Quantities:***

| **Name**          | **Unit** | **Description** |
| ----------------- | ---------------------- | -------------------------- |
| time				|	[s]		|	Absolute time. Timestamp at the middle of the current simulation interval [time - dt/2, time + dt/2] |
| dt				|	[s]		|	Length of the current simulation interval |
| dist				|	[m]		|	Distance the vehicle traveled at the end of the current simulation interval |
| v_act				|	[km/h]	|	Average vehicle speed in the current simulation interval |
| v_targ			|	[km/h]	|	Target speed |
| acc				|	[m/s^2]	|	Vehicle's acceleration, constant during the current simulation interval |
| grad				|	[%]		|	Road gradient |
| Gear				|	[-]		|	Gear. "0" = clutch opened / neutral |
| n_eng_avg			|	[1/min]	|	Average engine speed in the current simulation interval. Used for interpolation of the engine's fuel consumption |
| T_eng_fcmap		|	[Nm]	|	Engine torque used for interpolation of the engine's fuel consumption. T_eng_fcmap is the sum of torque demand on the output shaft, torque demand of the auxiliaries, and engine's inertia torque |
| Tq_full			|	[Nm]	|	Engine's transient maximum torque (see [transient full load](l#transient-full-load)) |
| Tq_drag			|	[Nm]	|	Engine's drag torque, interpolated from the full-load curve |
| P_eng_fcmap		|	[kW]	|	Total power the engine has to provide, computed from n_eng_avg and T_eng_fcmap |
| P_eng_full		|	[kW]	|	Engine's transient maximum power  (see [transient full load](l#transient-full-load)) |
| P_eng_drag		|	[kW]	|	Engine's drag power |
| P_eng_inertia		|	[kW]	|	Power loss/gain due to the engine's inertia |
| P_eng_out			|	[kW]	|	Power provided at the engine's output shaft |
| P_clutch_loss		|	[kW]	|	Power loss in the clutch due to slipping when driving off |
| P_clutch_out		|	[kW]	|	Power at the clutch's out shaft. P_clutch_out = P_eng_out - P_clutch_loss |
| P_aux				|	[kW]	|	Total power demand by the auxiliaries |
| P_gbx_in			|	[kW]	|	Power at the gearbox' input shaft |
| P_gbx_loss		|	[kW]	|	Power loss at the gerbox, interpolated from the loss-map |
| P_gbx_inertia		|	[kW]	|	Power loss due to the gearbox' inertia |
| P_ret_in			|	[kW]	|	Power at the retarder's input shaft. P_ret_in = P_gbx_in - P_gbx_loss - P_gbx_inertia |
| P_ret_loss		|	[kW]	|	Power loss at the retarder, interpolated from the loss-map |
| P_axle_in			|	[kW]	|	Power at the axle-gear input shaft. P_axle_in = P_ret_in - P_ret_loss |
| P_axle_loss		|	[kW]	|	Power loss at the axle gear, interpolated from the loss-map |
| P_brake_in		|	[kW]	|	Power at the brake input shaft (definition: serially mounted into the drive train between wheels and axle). P_brake_in = P_axle_in - P_axle_loss |
| P_brake_loss		|	[kW]	|	Power loss due to braking |
| P_wheel_in		|	[kW]	|	Power at the driven wheels. P_wheel_in = P_brake_in - P_brake_loss |
| P_wheel_inertia	|	[kW]	|	Power loss due to the wheels' inertia |
| P_trac			|	[kW]	|	Vehicle's traction power. P_trac = P_wheel_in - P_wheel_inertia |
| P_slope			|	[kW]	|	Power loss/gain due to the road's slope |
| P_air				|	[kW]	|	Power loss due to air drag |
| P_roll			|	[kW]	|	Rolling resistance power loss |
| P_veh_inertia		|	[kW]	|	Power loss due to the vehicle's inertia |
| FC-Map			|	[g/h]	|	Fuel consumption interpolated from FC map. |
| FC-AUXc			|	[g/h]	|	Fuel consumption after [Auxiliary-Start/Stop Correction](#fuel-consumption-calculation) (based on FC) |
| FC-WHTCc			|	[g/h]	|	Fuel consumption after [WHTC Correction](#fuel-consumption-calculation) (based on FC-AUXc) |
</div>
