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


![](pics/electric_motor_map.png)


###Thermal De-Rating

The electric machine can be overloaded for a certain period. In addition to the maximum drive and generation torque (which already is in overload condition) the mechanical power the electric machine can generate is required.

The basic principal of the thermal de-rating is as follows: based on the continuous power and the angular velocity for the continuous power as well as the maximum overload time a thermal energy buffer is calculated. During the simulation the difference between the current losses in the electric machine and the losses at the continuous power operating point are integrated over time. If this value reaches the capacity of the thermal energy buffer the electric machine can only deliver the specified continuous power until the thermal energy buffer goes below a certain.


$E_\textrm{th,buf} = P_\textrm{loss,cont} * t_\textrm{ovl}$

$P_\textrm{loss,cont} = P_\textrm{map, el}(\frac{P_\textrm{cont}}{n_\textrm{P, cont}}, n_\textrm{P, cont}) - P_\textrm{cont}$

In every simulation step the losses of the electric machine are accumulated:

$E_{\textrm{ovl,} i + 1} = E_{\textrm{ovl,} i} + P_\textrm{loss, i} * dt$

$P_\textrm{loss, i} = T_\textrm{em, mech} * n_\textrm{em} - P_\textrm{map, el}(T_\textrm{em, mech}, n_\textrm{em})$

If $E_\textrm{ovl, i}$ reaches the overload capacity $E_\textrm{th,buf}$ the power of the electric machine is limited to the continuous power until $E_\textrm{ovl,i}$ goes below the overload capacity multiplied by a certain factor. Then the maximum torque is available again.

