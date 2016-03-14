##Transmission Loss Map
This file defines losses in gearbox and axle transmission and must be provided for each gear in the [Gearbox File](#gearbox-editor). The file uses the [VECTO CSV format](#csv).

- Filetype: .vtlm
- Header: **Input Speed \[rpm], Input Torque \[Nm], Torque Loss [Nm]**
- Requires at least 3 data entries

Input speed and input torque are meant at the engine-side.

**Example:**

    Input Speed [rpm],Input Torque [Nm],Torque Loss [Nm]
    0,-350,6.81
    0,-150,5.81
    0,50,5.31
    0,250,6.31
    0,450,7.31
    0,650,8.31



###Sign of torque values###

* Input Torque >0 means normal driving operation.
* Input Torque \<0 means motoring operation. **The Torque Loss Map must include negative torque values for engine motoring operation!**
* Torque Loss is always positive!

###Calculation of Output Torque###

VECTO calculates the output torque using this formula, independent from the current operation mode (driving/braking):

$T_{output} = (T_{input} - T_{loss}) * r_{gear}$

with:

* T~output~ ... Output torque
* T~input~ ... Input torque
* T~loss~ ... Torque loss
* r~gear~ ... The tranmission ratio for the gurrent gear

