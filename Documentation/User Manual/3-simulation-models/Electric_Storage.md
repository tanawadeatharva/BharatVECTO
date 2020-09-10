##RESS

The rechargable electrictric energy storage system uses the following model parameters:

- Capacity of the battery pack
- C-Factor (limits the max. current for charging/discharging)
- Minimum state of charge
- Maximum state of charge
- Voltage of the battery pack over state of charge
- Internal resistance of the battery pack over state of charge

The C-Factor determins the maximum current as follows: $I_\textrm{max} = \textrm{C-Factor} * \textrm{Capacity [Ah]}$.

The voltage curve over state of charge is described in [Battery Internal Voltage File (.vbatv)](#battery-internal-voltage-file-.vbatv) and the internal resistance curve over state of charge is described in [Battery Internal Resistance File (.vbatr)](#battery-internal-resistance-file-.vbatr).

During the simulation the battery's state of charge must always be between the minimum and maximum SoC threshold.

![](pics/BatteryVoltage.png)

The maximum discharge current is further limited by the battery's internal resistance:

$I_\textrm{disch,max} = \frac{U(\textrm{SoC})}{4 * R_i(\textrm{SoC})}$

