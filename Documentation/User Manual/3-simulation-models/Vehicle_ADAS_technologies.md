##Vehicle: ADAS Technologies

<div class="declaration">
In Declaration mode VECTO applies a correction factor to take into account certain Advanced Driver Assistant System technologies. The following Technologies are currently considered:

- Engine stop start
- EcoRoll without engine stop
- EcoRoll with engine stop
- Predictive cruise control

For predictive cruise control three different options are considered:
 
- Option 1: crest coasting: Approaching a crest the vehicle velocity is reduced before the point where the vehicle starts accelerating by gravity alone compared to the set speed of the cruise control so that the braking during the following downhill phase can be reduced.
- Option 2: acceleration without engine power: During downhill driving with a low vehicle velocity and a high negative slope the vehicle acceleration is performed without any engine power usage so that the downhill braking can be reduced.
- Option 3: dip coasting: During downhill driving when the vehicle is braking at the overspeed velocity, PCC increases the overspeed for a short period of time to end the downhill event with a higher vehicle velocity. Overspeed is a higher vehicle speed than the set speed of the cruise control system.

A PCC system can be declared as input to the simulation tool if either the functionalities set out in points 1) and 2) or points 1), 2) and 3) are covered.

Out of this four technologies as listed above only 11 combinations are valid. For every valid ADAS technology combination VECTO reduces the final fuel consumtion by a certain percentage depending on the vehicle group, driving cycle, and payload.
</div>