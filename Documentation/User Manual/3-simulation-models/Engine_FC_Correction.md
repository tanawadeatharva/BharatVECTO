##Engine Stop/Start Fuel Consumption Correction

The energy demand of the auxiliaries during engine-off periods as well as for starting the engine is accumulated (see [Engine Stop/Start](#advanced-driver-assistant-systems-eco-roll-engine-stopstart)). The total fuel consumption is corrected in a post-processing step according to the *vehline* approach. Therefore, for every engine operating point with a positive fuel consumption the fuel consumption is plotted over the engine power. The slope (k) of the linear regression of the fuel consumption is used to compute the additional fuel that is needed for the energy demand during engine-off periods and engine starts.

![](pics/FC_Correction.PNG)

$\Delta FC = k * (E_{aux,ICE,off} + E_{ICE,start})$

$FC_{final} = FC_{final,mod} + \Delta FC$
