##Engine Fuel Consumption Correction

The final fuel consumption is corrected in a post-processing to reflect systems not directly modeled in VECTO (e.g. electric waste heat recovery sysmtes) or to account for systems not active all the time for different reasons (e.g., engine stop-start).

###Engine Stop/Start Correction

As the energy demand of auxiliaries is modeled as an average power demand over the whole simulated cycle, the energy demand certain auxiliaries during engine-off periods needs to be compensated during engine-on periods. This is done using the [Vehicle-Line approach](##vehicle-line-approach). The energy demand of the auxiliaries that shall be active also during engine-off periods as well as the energy demand for starting the engine is accumulated (see [Engine Stop/Start](#advanced-driver-assistant-systems-engine-stopstart), [Eco-Roll with Engine Stop/Start](#advanced-driver-assistant-systems-eco-roll), and [PCC with Eco-Roll and Engine Stop/Start](#advanced-driver-assistant-systems-predictive-cruise-control)) during the simulation. The final fuel consumption is corrected for this "missing" energy


###Vehicle-Line Approach

The total fuel consumption is corrected in a post-processing step according to the *vehline* approach. Therefore, for every engine operating point where the engine is switched on and has a positive fuel consumption the fuel consumption is plotted over the engine power. The slope (k) of the linear regression of the fuel consumption is used to compute the additional fuel that is needed for the energy demand during engine-off periods and engine starts.

![](pics/FC_Correction.PNG)

$\Delta FC = k * (E_{aux,ICE,off} + E_{ICE,start} - E_{WHR,corr}/\eta_{alternator})$

$FC_{final} = FC_{final,mod} + \Delta FC$
