##Bus Auxiliaries

The general approach for bus auxiliaries is that depending on the simulated driving cycle, number of passengers and selected auxiliary technologies the average power demand is calculated and applied during simulation. 
In case of smart auxiliaries (smart air compressor or smart alternator) the smart systems are only active during braking events if there is enough exessive power to provide the increased power demand for the smart systems. This reduces the amount of mechanical braking power required. Thus, during braking events the smart air compressor may produce more compressed air than required on average and the smart alternator may generate more electric power than required on average. The final fuel consumption is corrected for the excessive compressed air volume and electric energy in a [post processing step](#engine-fuel-consumption-correction).

###Engine Cooling Fan

###Steering Pump

###Pneumatic System

###Electric System

###HVAC

####Model Parameters:

  - Bus body
  	 + Length $l_\textrm{Bus}$
     + Width $b_\textrm{Bus}$
  	 + Height $h_\textrm{Bus}$
  	 + Double decker
  	 + Floor type (low floor, raised floor)
  - Auxiliary heater power
  - HVAC system configuration
  - Number of passengers
  - Fuel saving technologies
  - Environmental contidions map

The environmental conditions map contains a list of environmental conditions (environmental temperature, solar factor) and a weighting factor. The power demand for the HVAC system (separated into mechanical and electrical power demand) is calculated for every environmental contition in the map and summed up with the according weighting factor.

####Calculation of HVAC Power Demand

$P_\textrm{HVAC,mech,sum} = \sum_\textrm{env} w_\textrm{env} *  min(P_\textrm{HVAC,mech}(T_\textrm{env}, S_\textrm{env}) * (1 - \textrm{TechBenefitsMech}), P_\textrm{HVAC,max}) / \textrm{COP}$

$P_\textrm{HVAC,el,sum} = \sum_\textrm{env} w_\textrm{env} * min(P_\textrm{HVAC,el}(T_\textrm{env}, S_\textrm{env}) * (1 - \textrm{TechBenefitsEl}), P_\textrm{HVAC,max}) / \textrm{COP} + P_\textrm{ventilation,heating}(T_\textrm{env}, S_\textrm{env}) * (1 - \textrm{TechBenefitsElHeatingVent}) + P_\textrm{ventilation,cooling}(T_\textrm{env}, S_\textrm{env}) * (1 - \textrm{TechBenefitsElCoolingVent})$

$P_\textrm{HVAC,mech}(T, S) = \left\{
  \begin{array}{ll}
    0 & : T < 17^{\circ}C\\
    0 & : \textrm{electric compressor}\\
    min( P_\textrm{HVAC}(T, S, T_\textrm{low}) , P_\textrm{HVAC}(T, S, T_\textrm{high}(T))) & : P_\textrm{HVAC}(T, S, T_\textrm{low}) > 0 \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{high}(T)) > 0 \\
    0 & : \textrm{otherwise}
  \end{array}
\right.$

$P_\textrm{HVAC,el}(T, S) = \left\{
  \begin{array}{ll}
    0 & : T < 17^{\circ}C\\
    0 & : \textrm{mechanical compressor}\\
    min( P_\textrm{HVAC}(T, S, T_\textrm{low}) , P_\textrm{HVAC}(T, S, T_\textrm{high}(T))) & : P_\textrm{HVAC}(T, S, T_\textrm{low}) > 0 \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{high}(T)) > 0 \\
    0 & : \textrm{otherwise}
  \end{array}
\right.$

$P_\textrm{ventilation,heating}(T_\textrm{env}, S_\textrm{env}) = \left\{
  \begin{array}{ll}
    V_\textrm{Bus} * r(\textrm{true}) * 0.56 Wh/m^3 & : P_\textrm{HVAC}(T, S, T_\textrm{low}) < 0 \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{high}(T)) < 0 \\
    0 & : \textrm{otherwise}
  \end{array}
\right.$

$P_\textrm{ventilation,cooling}(T_\textrm{env}, S_\textrm{env}) = \left\{
  \begin{array}{ll}
    V_\textrm{Bus} * r(\textrm{false}) * 0.56 Wh/m^3 & : T_\textrm{env} \geq 17^{\circ}C \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{low}) > 0 \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{high}(T)) > 0 \\
    0 & : \textrm{otherwise}
  \end{array}
\right.$

$r(\textrm{heating}) = \left\{\begin{array}{ll}
    7 \;1/h & : \textrm{HVAC Configuration 1 \& 2}\\
    10 \;1/h & : \textrm{heating} \;\textrm{and}\; \textrm{HVAC Configuration 3 -- 9} \\
    20 \;1/h & : \textrm{HVAC Configuration 3 -- 9} \\
    \end{array}
\right.$

$T_\textrm{low} = 18^{\circ}C$

$T_\textrm{high}(T_\textrm{env}) = \left\{\begin{array}{ll}
    max(23^{\circ}C, T_\textrm{env} - 3^{\circ}C) & : \textrm{Low floor bus}\\
    23^{\circ}C & : \textrm{otherwise}
    \end{array}
\right.$

$P_\textrm{HVAC}(T_\textrm{env}, S, T_\textrm{calc}) = Q_\textrm{Wall}(T_\textrm{env}, T_\textrm{calc}) + P_\textrm{Passenger}(T_\textrm{env}) + P_\textrm{Solar}(T_\textrm{env}, S)$

$Q_\textrm{Wall}(T_\textrm{env}, T_\textrm{calc}) = (T_\textrm{env} - T_\textrm{calc}) * A_\textrm{BusSurface} * U$

$P_\textrm{Passenger}(T_\textrm{env}) = \#_\textrm{Passenger} * \left\{\begin{array}{ll}
    50\; W & : T_\textrm{env} < 17^{\circ}C\\
    80\; W & : \textrm{otherwise}
    \end{array}
\right.$

$P_\textrm{Solar}(T_\textrm{env}, S) = S * A_\textrm{Windows} * G * S_\textrm{clouding}(T_\textrm{env}) * 0.25$

$S_\textrm{clouding}(T_\textrm{env}) = \left\{\begin{array}{ll}
    0.65 & : T_\textrm{env} < 17^{\circ}C\\
    0.8 & : \textrm{otherwise}
    \end{array}
\right.$

$U = \left\{\begin{array}{ll}
    4\; W/Km^2 & : \textrm{Low floor bus}\\
    3\; W/Km^2 & : \textrm{Raised floor bus}
    \end{array}
\right.$

$G = 0.95$

$V_\textrm{Bus} = l_\textrm{HVAC} * b_\textrm{Bus} * h_\textrm{Bus}$

$A_\textrm{BusSurface} = 2 * (l_\textrm{HVAC} * b_\textrm{Bus} + l_\textrm{HVAC} * h_\textrm{Bus} + b_\textrm{Bus} * h_\textrm{Bus})$

$A_\textrm{Window} = l_\textrm{HVAC} * h_\textrm{Windows} + A_\textrm{Front\&Rear}$

$l_\textrm{HVAC} =  \left\{\begin{array}{ll}
    2 * 1.2 \;m & : \textrm{HVAC Configuration 2}\\
    l_\textrm{Bus} & : \textrm{otherwise} \\
    \end{array}
\right.$

$h_\textrm{Windows} = \left\{\begin{array}{ll}
    2.5 \;m & : \textrm{Double Decker}\\
    1.5 \;m & : \textrm{Single Decker} 
    \end{array}
\right.$

$A_\textrm{Front\&Rear} = \left\{\begin{array}{ll}
    8 \;m^2 & : \textrm{Double Decker}\\
    5 \;m^2 & : \textrm{Single Decker}
    \end{array}
\right.$

$P_\textrm{HVAC,max} = P_\textrm{HVAC,max,passenger} + P_\textrm{HVAC,max,driver}$

$P_\textrm{HVAC,max,passenger} = \textrm{Lookup}_\textrm{passenger}(\textrm{HVAC Configuration}, \textrm{driving cycle}) * V_\textrm{passenger}$

$V_\textrm{passenger} = l_\textrm{internal} * h_\textrm{internal} * b_\textrm{Bus}$

$l_\textrm{internal} =  \left\{\begin{array}{ll}
    2 * l_\textrm{Bus} & : \textrm{low floor} \;\textrm{and}\; \textrm{double decker}\\
    l_\textrm{Bus} & : \textrm{low floor} \;\textrm{and}\; \textrm{single decker}\\
    1.5 * l_\textrm{Bus} & : \textrm{raised floor} \;\textrm{and}\; \textrm{double decker} \;\textrm{and}\; \#_\textrm{passengers lower deck} > 6\\
 l_\textrm{Bus} + 2.4 \;m & : \textrm{raised floor} \;\textrm{and}\; \textrm{double decker} \;\textrm{and}\; \#_\textrm{passengers lower deck} \leq 6\\
    l_\textrm{Bus} & : \textrm{raised floor} \;\textrm{and}\; \textrm{single decker}\\
    \end{array}
\right.$

$h_\textrm{internal} = \left\{\begin{array}{ll}
    1.8 \;m & : \textrm{double decker} \\
    h_\textrm{Bus} - 0.5 \;m & : \textrm{single decker} \;\textrm{and}\; \textrm{raised floor} \\
    h_\textrm{Bus} & : \textrm{single decker} \;\textrm{and}\;\textrm{low floor}
    \end{array}
\right.$

$P_\textrm{HVAC,max,driver} = \textrm{Lookup}_\textrm{driver}(\textrm{HVAC Configuration}, \textrm{driving cycle})$


**Aux Heater Power**

$P_\textrm{HVACSSM,auxHtr}(\overline{P}_\textrm{ice,waste heat}) = \sum_\textrm{env} w_\textrm{env} *  P_\textrm{auxHtr}(T_\textrm{env}, S_\textrm{env}, \overline{P}_\textrm{ice,waste heat}) * 0.2 * 0.75)$


$P_\textrm{auxHtr}(T, S, P_\textrm{wasteHeat}) = \left\{
  \begin{array}{ll}
    |max( P_\textrm{additionalHeating}(T, S, T_\textrm{low}, P_\textrm{wasteHeat}) , P_\textrm{additionalHeating}(T, S, T_\textrm{high}(T), P_\textrm{wasteHeat}))| & : P_\textrm{HVAC}(T, S, T_\textrm{low}) < 0 \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{high}(T)) < 0 \\
    0 & : \textrm{otherwise}
  \end{array}
\right.$

$P_\textrm{additionalHeating}(T, S, T_\textrm{calc}, P_\textrm{wasteHeat}) = \left\{
    \begin{array}{ll}
        P_\textrm{HVAC}(T, S, T_\textrm{calc}) * (1 - \textrm{TechBenefitsFuelHeater})) + P_\textrm{wasteHeat} & : P_\textrm{HVAC}(T, S, T_\textrm{calc}) * (1 - \textrm{TechBenefitsFuelHeater})) < 0 \;\textrm{and}\; P_\textrm{HVAC}(T, S, T_\textrm{calc}) * (1 - \textrm{TechBenefitsFuelHeater})) < -P_\textrm{wasteHeat} \\
        0 & : \textrm{otherwise}
    \end{array}
\right.$


###Auxiliary Power Demand