##Fuel Consumption Map (.vmap)
The FC map is used to interpolate the base fuel consumption before corrections are applied. For details see [Fuel Consumption Calculation](#fuel-consumption-calculation). The file uses the [VECTO CSV format](#csv).


- Filetype: .vmap
- Header: **engine speed [rpm], engine torque [Nm], Fuel Consumption [g/h]**
- Requires at least 3 data entries
- The map must cover the full engine range between full load and motoring curve.

<div class="vecto2">
Extrapolation of fuel consumption map is not possible.
</div>

<div class="vecto3">
Extrapolation of fuel consumption map is possible, but only allowed in Engineering Mode (with warnings). In Declaration Mode the simulation aborts.
</div>

**Example:**

    Retarder Speed [1/min],Loss Torque [Nm]
    0,10
    100,10.02
    200,10.08
    300,10.18
