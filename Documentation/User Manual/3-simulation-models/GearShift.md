##Gear Shift Model

This VECTO version contains a new shift strategy called EffShift.

The shift strategy is on a first level based on gearshift lines for upshift and downshift (similar to the classic VECTO gearshift strategy). Additionally “Efficiency shifts” can be triggered between the shift lines, if the fuel efficiency (g/kWh cardan) in a candidate gear is better than in the current gear. In order to cover a large range of the engine map with the “Efficiency shifts”, the area between the downshift and upshift line has to be of sufficient size. Hence, the shift lines are defined as shown in the figure below, with the downshift line (green) to the left and the up-shift line (red) to the right. Due to the superposition of the gear-shift lines with the EffShift algorithm as described below the upshift line is not relevant for upshifts in most cases. 


![](pics/EffShift.jpg)

The points P1 to P4 are calculated according as follows:

  * P1 (upshift line)	$n_1 = n_{idle} * 1.1 / T_1 = 0$
  * P2 (upshift line)	$n_2 = n_{idle} * 1.1  / T_2=T_{98 @ n2}$   
  * P3 (upshift line)	$n_3 = n_{T99,low}  / T_3=T_{99 low}$
  * n4 (downshift line) $n_4 = n_{P98,high}$ / (vertical line)
  * n5 (left boundary for engine speed range with reduced target acceleration demand in next gear) $n_4 = n_{T98,high}$ / 	(vertical line)


The definition of the upshift line depends on the transmission type: for AMT, the pre-shift engine speed is considered for the upshift line and for AT the post-shift engine speed is used.
Additionally, the demanded acceleration to be available after a gearshift is reduced compared to the current acceleration: This is done for engine speeds between $n_{T98h}$ and $n_{P98h}$. This shall reduce reving up the engine during full-load accelerations. The demanded acceleration is calculated as follows:

$a_{demand} = a_{act} * a_{red}$   for $(n_{act} > n_{T98h})$

$a_{red} = 1+ (\textrm{AccelerationFactorNP98h} - 1) / (n_{P98h} - n_{T98h}) * (n - n_{T98h})$  for $(n_{act} > n_{T98h})$






