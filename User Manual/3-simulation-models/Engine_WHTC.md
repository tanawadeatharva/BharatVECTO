##Engine: WHTC Correction

<div class="declaration">
In declaration mode the fuel consumption is corrected as follows:

To prevent inconsistencies of regulated emissions and fuel consumption between the WHTC (hot part) test and the steady state fuel map as well as considering effects of transient engine behaviour a “WHTC correction factor” is used.

Based on the target engine operation points of the particular engine in WHTC the fuel consumption is interpolated from the steady state fuel map (“backward calculation”) in each of the three parts of the WHTC separately. The measured specific fuel consumption per WHTC part in [g/kWh] is then divided by the interpolated specific fuel consumption to obtain the “WHTC correction factors” CF~Urb~, CF~Road~, CF~MW~. For the interpolation the same method as for interpolation in VECTO is applied (Delauney triangulation).

All calculations regarding the brake specific fuel consumption from the interpolation as well as from the measurement and the three correction factors CF~Urb~, CF~Road~, CF~MW~ are fully implemented in the VECTO-Engine evaluation tool.

The total correction factor CF~Tot-i~ depends on the mission profile "i" and is produced in VECTO by mission profile specific weighting factors (WF~i~) listed in the table below.

CF~Tot-i~ = CF~Urb~ * WF~Urb-i~ + CF~Rur~ * WF~Rur-i~ + CF~MW~ * WF~MW-i~

| Index | Mission profile		| WF~MW~| WF~Road~| WF~Urb~ |
|-------|-----------------------|-------|-------|---------|
| 1 	| 	Long haul			| 89%	| 0%	| 11% |
| 2 	| 	Regional delivery	| 53%	| 30%	| 17% |
| 3 	| 	Urban delivery		| 4%	| 27%	| 69% |
| 4 	| 	Municipial utility	| 2%	| 0%	| 98% |
| 5 	| 	Construction		| 6%	| 32%	| 62% |
| 6 	| 	Citybus				| 0%	| 0%	| 100% |
| 7 	| 	Interurban bus		| 19%	| 36%	| 45% |
| 8 	| 	Coach				| 78%	| 22%	| 0% |

</div>