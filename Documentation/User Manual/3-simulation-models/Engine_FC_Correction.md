##Engine Fuel Consumption Correction

The final fuel consumption is corrected in a post-processing to reflect systems not directly modeled in VECTO (e.g. electric waste heat recovery sysmtes) or to account for systems not active all the time for different reasons (e.g., engine stop-start).

###Engine Stop/Start Correction

As the energy demand of auxiliaries is modeled as an average power demand over the whole simulated cycle, the energy demand of certain auxiliaries during engine-off periods needs to be compensated during engine-on periods. This is done using the [Engine-Line approach](##engine-line-approach). The energy demand of the auxiliaries that shall be active also during engine-off periods as well as the energy demand for starting the engine is accumulated (see [Engine Stop/Start](#advanced-driver-assistant-systems-engine-stopstart), [Eco-Roll with Engine Stop/Start](#advanced-driver-assistant-systems-eco-roll), and [PCC with Eco-Roll and Engine Stop/Start](#advanced-driver-assistant-systems-predictive-cruise-control)) during the simulation. The final fuel consumption is corrected for this "missing" energy

###Bus Auxiliaries Correction

**Electric System**

In case smart electrics are used the electric energy generated may be different from the electric energy consumed as the smart alternator may generate more electric power in drag situations. Moreover, the state of charge of a battery may be different at the end of the simulated cycle than at the beginning and thus the fuel consumption needs to be corrected.

$\Delta E_\textrm{ES} = E_\textrm{ES,consumed} - E_\textrm{ES,generated} + (\textrm{SOC}_\textrm{start} - \textrm{SOC}_\textrm{end}) * \textrm{Capacity}_{\textrm{RESS}}$

$E_\textrm{ES,mech,corr} = \Delta E_\textrm{ES} / \eta_\textrm{alternator} / \eta_\textrm{pulley}$

**Pneumatic System**

In case of smart pneumatics the total air generated may be different from the total air consumed by all pneumatic consumers as the smart compressor generates more compressed air in drag situations. The fuel energy difference is corrected according to the following equations:

$\Delta \textrm{Air} = \textrm{Air}_\textrm{consumed,total} - \textrm{Air}_\textrm{generated,total}$

$k_\textrm{Air} = (E_\textrm{PS,generated} - E_\textrm{PS,off}) / (\textrm{Air}_\textrm{generated,total} - 0)$

$E_\textrm{PS,corr} = k_\textrm{Air} * \Delta \textrm{Air}$


**Aux Heater**

The power demand for an additional fuel-fired heater is calculated in the post-processing. The HVAC steaty state model calculates the heating demand (weighted sum of different climatic conditions) and based on the engine's average waste heat over the cycle the power demand for the aux heater is calculated.

$E_\textrm{ice,waste heat} = \sum_\textrm{fuels} FC_\textrm{final,sum}(fuel) * NCV_\textrm{fuel}$

$\overline{P}_\textrm{ice,waste heat} = E_\textrm{ice, waste heat} / t_\textrm{cycle}$

$E_{auxHeater} = P_\textrm{HVACSSM,auxHtr}(\overline{P}_\textrm{ice,waste heat}) * t_\textrm{cycle}$

###Engine-Line Approach

The total fuel consumption is corrected in a post-processing step according to the *engine-line* approach. Therefore, for every engine operating point where the engine is switched on and has a positive fuel consumption the fuel consumption is plotted over the engine power. The slope (k) of the linear regression of the fuel consumption is used to compute the additional fuel that is needed for the energy demand during engine-off periods and engine starts.

![](pics/FC_Correction.PNG)

$\Delta FC = k * (E_\textrm{aux,ICE,off} + E_\textrm{ICE,start} - E_\textrm{WHR,corr}/\eta_\textrm{alternator}) + k * (E_\textrm{ES,mech,corr} + E_\textrm{PS,corr})$

The fuel consumption for the aux heater is only added for the main fuel:

$\Delta FC_\textrm{auxHeater} = E_\textrm{auxHeater} * NCV_\textrm{main fuel}$ 

$FC_\textrm{final} = FC_\textrm{final,mod} + \Delta FC + \Delta FC_\textrm{auxHeater}$
