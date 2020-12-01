##Electric Motor

The electric motor is modeled by basically 4 map files:

 - Maximum drive torque over motor speed
 - Maximum generation torque over motor speed
 - Drag curve (i.e., the motor is not energized) over motor speed
 - Electric power map ($P_\textrm{map,el}$)
 - Continuous power ($P_\textrm{cont}$)
 - Engine speed for continuous power ($n_\textrm{P,cont}$)
 - Maximum overload time ($t_\textrm{ovl}$)

The first two curves are read from a single .vemp file (see [Electric Motor Max Torque File (.vemp)](#electric-motor-max-torque-file-.vemp)). The drag curve is provided in a .vemd file (see [Electric Motor Drag Curve File (.vemd)](#electric-motor-drag-curve-file-.vemd)) and the electric power map in a .vemo file (see [Electric Motor Map (.vemo)](#electric-motor-map-.vemo)).

The convention for all input files is that positive torque values drive the vehicle while negative torque values apply additional drag and generate electric power.

The follwing picture shows the signals used in VECTO and provided in the .vmod file. The VECTO convention is that positive torque adds additional drag to the drivetrain. Thus, if the electric motor propells the vehicle it applies negative torque.

![](pics/electric_motor_map.png)

###Electric Motor Model

The VECTO component for the electric motor contains the electric motor itself which is connected via a transmission stage to the drivetrain. The ratio and efficiency of the transmission stage can be defined in the vehicle model.

![](pics/EM_Model_scaled.png)

The naming convention for the signals is that 'X' denotes the position of the EM in the powertrain. P_X_... denotes signals related to the drivetrain speed while P_X-em_... denotes signals to the electric motor shaft.

P_X_in = P_X_out + P_X_mech

P_X_mech = P_X-em_mech + P_X_transm_loss

P_X-em_mech = P_X-em_mech_elmap + P_X-em_inertia

P_X-em_mech_elmap = P_X-em_el + P_X-em_loss

P_X-em_mech_elmap = n_X-em * T_X-em_map

P_X-em_el = PowerMap(n_X-em, T_X-em_map)

P_X_loss = P_X_mech - P_X-em_el


###Thermal De-Rating

The electric machine can be overloaded for a certain period. In addition to the maximum drive and generation torque (which already is in overload condition) the mechanical power the electric machine can generate is required.

The basic principal of the thermal de-rating is as follows: based on the continuous power and the angular velocity for the continuous power as well as the maximum overload time a thermal energy buffer is calculated. During the simulation the difference between the current losses in the electric machine and the losses at the continuous power operating point are integrated over time. If this value reaches the capacity of the thermal energy buffer the electric machine can only deliver the specified continuous power until the thermal energy buffer goes below a certain.


$E_\textrm{th,buf} = P_\textrm{loss,cont} * t_\textrm{ovl}$

$P_\textrm{loss,cont} = P_\textrm{cont} - P_\textrm{map, el}(\frac{P_\textrm{cont}}{n_\textrm{P, cont}}, n_\textrm{P, cont})$

In every simulation step the losses of the electric machine are accumulated:

$E_{\textrm{ovl,} i + 1} = E_{\textrm{ovl,} i} + (P_\textrm{loss, i} - P_\textrm{loss,cont}) * dt$

$P_\textrm{loss, i} = T_\textrm{em, mech} * n_\textrm{em} - P_\textrm{map, el}(T_\textrm{em, mech}, n_\textrm{em})$


If $E_\textrm{ovl, i}$ reaches the overload capacity $E_\textrm{th,buf}$ the power of the electric machine is limited to the continuous power until $E_\textrm{ovl,i}$ goes below the overload capacity multiplied by a certain factor. Then the maximum torque is available again.

