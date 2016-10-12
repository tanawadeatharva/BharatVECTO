##Gear shift rules for AT Gearbox

The gear shift rules for automatic gearboxes differ from AMT and MT.

Gears are shifted sequentially:

- 1C -> 1L -> 2L -> ...  (torque converter only in 1st gear)
- 1C -> 2C -> 2L -> ...  (torque converter in 1st and 2nd gear)

###Upshift rules

- If engine speed in the next gear (see shift sequence) is above the upshift line AND
- The engine can provide at least the same power as currently required (i.e., can keep the current acceleration)

###Downshift

- If the engien speed falls below engine's idle speed

- Drivetrain in "Neutral" when either
  - velocity < 5 km/h
  - During deceleration phase when the torque converter is active and the engine speed would fall below idle speed

###Shift parameters

- Min. time between two consecutive gearshifts.

