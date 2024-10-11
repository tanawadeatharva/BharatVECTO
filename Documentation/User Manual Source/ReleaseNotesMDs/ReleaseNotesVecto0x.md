# VECTO 0.x

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV1.png)

<!-- Cover Slide -->



## VECTO 0.11.2.3456 Development Version 18.06.2024
### Release Notes

* Features
  * CodeEU\-196: Implement wheel bearings
  * Input for Engineering mode is available via the GUI & JSON file\. In the vehicle file \(\.vveh\) in an entry of the "Axles" array\, you can add the field "WheelEndFriction" and assign it a numerical value. Input for Declaration mode is available via a temporary XSD \(VectoDeclarationDefinitions\.DEV\.2\.6\.xsd\)\. Sample file in folder: Generic Vehicles\\Declaration Mode\\Wheel\_Bearings
  * CodeEU\-643: added support for fuel cell IEPC2
  * Added new simulation job type for IEPC vehicles with fuel cells\. This feature is only supported in engineering mode\. The GUI has been amended to allow the creation/modification of this new job type\. A sample job has been added to the folder 'Generic Vehicles\\Engineering Mode\\GenericIEPC \- FCHV'\.

* Bugfixes
  * CodeEU\-236: Fuel Cell Simulation Aborts when Charging Power of Battery is too low
  * CodeEU\-209: VECTO\-0\.11\.1\.3228\-DEV crash in declaration mode
  * CodeEU\-207: Writing XML Reports fails for vehicles without battery

## VECTO 0.11.1.3382 Development Version 03.11.2023

### Release Notes

* Changes
  * Enabled the usage of multiple Fuel Cell Systems \(FCS\)
    * Up to two FCS Strings
    * Up to three FCS in each FCS String
  * New naming convention:
    * FCS: Single Fuel Cell System\, described with MassFlowMap \(\.vfcm\) and power limits container in the Fuel Cell System File \(\.vfcc\)\.
    * FCS String: Defined in the \.vveh file\, can contain up to three identical FCS
    * CFCS: Composite Fuel Cell System\, a virtual fuel cell system containing up to two FCS String\.
  * Changes in input files due to new naming convention
  * Optimized power distribution among FCS Strings and between individual FCS inside a string\.\(Explained in more detail in the user manual\)
  * Allow operating a FCS below its minimum power via time splitting
  * Removed On/Off - Hysteresis
  * Removed Gradient Power Change

# First VECTO beta release for the 3rd Amendment H2 Internal Combustion Engines
## VECTO 0\.11\.0\.3193 Development Version 29\.09\.2023
### Release Notes

# H2 ICE

New fuel types implemented  "H2 PI" and "H2 CI" \(i\.e\. H2\-Diesel dual\-fuel\).

Well\-known ICE input data structure unchanged\, no new elements

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV4.png)

Fuel characteristics used

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV8.png)

# First VECTO beta release for the 3rd Amendment Fuel Cell Hybrid Vehicles (FCHV)

# Fuel cell hybrid vehicles (FCHV) - Op. strategy (1/5)

* FCHV generic operation strategy \(Charge Sustaining Mode\) implemented as presented in TF FCS \#15 with following basic principle:
  * Maximum phlegmatisation of FCS by operation strategy
  * FCS needs to provide average electric power demand over longer horizon
  * Longest possible averaging horizon depends on battery size  and specific vehicle\-cycle combination  __  _ The smaller the battery or the greater the electric energy demand of the vehicle\,_  _the smaller the averaging horizon will get leading to a more dynamic operation of the FCS\.

* Basic approach for implementation of FCS operation in "CS" Mode in 4 steps
  * Pre\-simulation run as PEV to determine electric power demand for propulsion and auxiliaries
    * Power limit of "special" battery defined as sum of respective maximum powers \(BAT \+ FCS\)
  * Resulting total electric power from step 1 will be averaged over a certain   _fixed_   window size  which is distance based \(details next slide\)
    * Buffering of energy is limited by battery size\, SOC limits min/max are not allowed to be exceeded
    * Using largest possible window size without violating SOC limits in any window
    * Adaption of start SOC to specific mission in order to maximize usage of battery as buffer
  * Actual simulation run as FCHV in\-the\-loop where FCS is operated following a fixed power trace  to cover average electric power demand determined from pre\-processing in step 2  \(battery covers instantaneous deviations from average electric power demand\)
  * Δ  SOC correction in post\-processing to account for small deviations from neutral SOC behaviour over cycle \(e\.g\. variations in auxiliary power\, small deviations in battery losses etc\.\)

* Averaging and window size explained \(1/2\)
  * The averaging window is placed distance\-symmetrically over each actual time step from the pre\-simulation
  * Window size is determined by total distance within window
![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV7.png)

* Averaging and window size explained \(2/2\)
  * Maximum window size is given by complete cycle\, thus cycle is concatenated 3 times in a row
  * For time steps near the beginning or end of the cycle\, the windows expand over the actual cycle and continue at the adjacent parts at the end or beginning\.

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV6.png)

* Actual simulation run as FCHV and applicable boundary conditions
  * In case average power demand over total cycle is higher than FCS maximum power: defined error message and simulation is aborted (vehicle configuration not reasonable for this mission\, should not occur in practice)
  * In case average power in a certain timestep is lower than FCS minimum power: FCS is switched off missing energy from FCS for those timesteps will be accounted for in post\-processing via ΔSOC correction

# Fuel cell hybrid vehicles (FCHV) - VECTO features (1/5)

New powertrain type of FCHV added to Job Editor

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV9.png)

FCS to be parameterized via specific "Fuel Cell System" tab in "Vehicle" editor

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV10.png)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV11.png)

Power gradient limits change in FCS power over time

On/off\-Hysteresis is minimum time that FCS stays off or on after state change occurs  <span style="color:#FF0000"> __ </span>  <span style="color:#FF0000"> _ not required anymore after insights gained during work on implementation of multiple FCS_ </span>

FCS to be parameterized   via specific "Fuel Cell System" tab in "Vehicle" editor

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV12.png)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV13.png)

Max / min power limits operation range of FCS component

FCS mass flow map needs to cover the indicated range

<span style="color:#FF0000"> Double\-click or "\+"

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV14.png)

<span style="color:#FF0000"> Limited to 1

<span style="color:#FF0000"> for this release

FCS to be parameterized   via specific "Fuel Cell System" tab in "Vehicle" editor

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV15.png)

* Input data structure for FCS H2 mass flow map
* CSV file format
  * 1  st   column: Electric power output of FCS in \[kW\]
  * 2  nd   column: H2 mass flow in \[g/h\]
* _FCS component efficiency should also include DC/DC converter \(as this is part of component test procedure\)\!_

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV16.png)

* Parameterization of auxiliaries for FCHV
  * Electric power demand of auxiliaries for both\, lorries and buses\, need to be parameterized via the "Bus Auxiliaries" GUI window

_INFO: Electric aux power demand is calculated by multiplying the specified current values by a voltage of 28\.3V_

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV17.png)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV18.png)

Demand during vehicle driving

Demand during vehicle standstill

# FCHV - Examples for generic operation strategy (1/5)

* Exemplary results for VECTO Group 2 12\-ton rigid lorry  \(9\.5 ton actual mass\) on following slides
* Simulation in VECTO Urban Delivery Cycle
  * Average electric power demand around 22 kW
* FCHV powertrain specifications:
  * 100 kW FCS max power
  * following variants of battery:
    * 5 kWh
    * 10 kWh
    * 20 kWh
* Resulting speed profile was exactly the same for all different configurations

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV19.png)

* The lower the buffer size\, the more dynamic the FCS operation gets
* _Exactly same speed profile for all variants_  _ _
* Averaging window sizes determined:
  * 5 kWh: 6 km
  * 10 kWh: 37\.5 km
  * 20 kWh: 100 km

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV20.png)

_5 kWh_

<span style="color:#FFFF00">10 kWh</span>

_20 kWh_

Fuel cell power for different battery sizes

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV21.png)

Battery covers instantaneous deviations from average electric power demand

Average electric power demand \(i\.e\. FCS power\) constant for the setting shown with 20 kWh REESS size  _ _

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV22.png)

<span style="color:#00B0F0"> Battery power

_Fuel cell power_

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV23.png)

Using largest possible window size without violating SOC limits in any window     maximizing of battery buffer usage

_Exactly same speed profile for all variants_  _ _

_SOC limits are Engineering settings\, no connex to generic limits used in Declaration Mode_

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV24.png)

SOC for different battery sizes

_5 kWh_

_10 kWh_

<span style="color:#00B0F0">20 kWh</span>

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV25.png)

Adaption of start SOC to specific mission in order to maximize usage of battery as buffer  \(detail from previous graph\)

_Exactly same speed profile for all variants_

Adapted start SOC

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV26.png)

SOC for different battery sizes:
* 5 kWh
* 10 kWh
* 20 kWh

# Fuel cell hybrid vehicles (FCHV)  - Comments

* Release to be distributed via VECTO\-DEV mailing list end of September
  * Basic description of FCHV features will also be added to VECTO release notes
  * Some example vehicles will be provided
* Current release restricted to one single FCS component
* **Updated release end of October will be able to handle:**
  * multiple FCS configurations as presented in TF FCS \#17
    * Maximum 2x3 configuration  i\.e\. 2 different FCS with a maximum number of 3 identical systems for each of the 2 different ones
  * Special provisions for handling power demands below minimum FCS power
  * IMC feature combined with FCHV

# First VECTO beta release for the 3rd Amendment Vehicles with In-Motion Charging Features

# Vehicles with In-Motion Charging (IMC) features (1/11)

* One of the tasks of the pre\-development project for the 3rd Amendment was to analyze different systems for in\-use charging of electric vehicles
  * State of the art
  * Relevance and possible paths of implementation to Regulation \(EU\) 2017/2400
  * Functional prototype
* The analyses and proposals for \(1\) and \(2\) were presented and discussed with stakeholders in five stakeholder meetings \(Oct 2021\, Dec 2021\, Jan 2022\, Jun 2022 and Sept 2022\)\.
* The following slides summarize the most important principles and present the functional prototype

Reference point for results regarding electric energy consumption

* Principle applied already for the 2nd amendment
* Consequences for the 3rd amendment
  * Stationary charging: minor improvements to the Annex \(max\. charging power\)
  * In\-motion charging: Requires model extension and separate provisions in the Annex

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV27.png)

* Modelling in VECTO \- Overview
  * _Driving with "direct feed"_ incorporated to simulations of "charge depleting" mode by postprocessing \(no additional sim time\, no additional definitions etc\.\) \(DECL) higher utility factor / share of electric driving for OVC\-HEV \(slightly\) lower battery internal losses.
  * _Charging of batteries by IMC_ handled by postprocessing (DECL) higher utility factor / share of electric driving for OVC\-HEV.
  * _Impact on air drag_ incorporated to in\-the loop simulation via a mission average share "active (which applies either to motorway sections or to the total cycle) (ENG\, DECL)
  * _No direct influence on the "electric ranges"_, i\.e\. these are also only calculated on the basis of the usable battery capacity (DECL)

<span style="color:#7030A0">DECL / ENG: Effect visible in Declaration Mode / Engineering Mode</span>

_Report with detailed documentation to be published soon_

Modelling in VECTO - Calculations of electric energy consumption at battery terminals

<span style="color:#7030A0">Lila terms: new elements introduced for IMC modelling</span>

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV28.png)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV29.png)

_Calculation examples see Task3\_Masterexcel\_OVC\_IMC\.xlsx as distributed with ppt_  _Report with detailed documentation to be published soon_

Modelling in VECTO - Current proposals for Declaration Mode

| In-motion charging "technology" | Relevance |  |  | Proposal for definition in Regulation (EU) 2017/2400 |
| :-: | :-: | :-: | :-: | :-: |
|  | Heavy lorries | Heavy buses | Medium lorries |  |
| Overhead pantograph | Yes | Yes | No | Vehicle equipped with overhead pantograph for connection with overhead catenary infrastructure as regulated by standard tbd<br />and which is not overhead trolley. |
| Overhead trolley | No | Yes | No | Vehicle equipped with poles for connection with overhead catenary infrastructure as regulated in Annex 12 to UN Regulation 107 revision 8.  |
| Ground-rail | Yes1 | Yes1 | Yes1 | Charging technology that conductively transfers the electrical energy to the vehicle through rails embedded in or on top of the road surface. |
| Wireless | Yes1 | Yes1 | Yes1 | Charging technology that inductively transfers the electrical energy to the vehicle through devices embedded in or on top of the road surface providing magnetic fields as they are specified IEC 61980. |

1. Only Potential niche applications are conceivable\.

<span style="color:#7030A0"> _Lila: to be worked out for the 3_ </span>  <span style="color:#7030A0"> _rd_ </span>  <span style="color:#7030A0"> _ amendment_ </span>

Modelling in VECTO - Proposal for IMC infrastructure availability

Overhead \- Trolley

Overhead \- Pantograph

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV30.png)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV31.png)

Modelling in VECTO - Proposal for IMC infrastructure availability

Ground rail \+ Wireless

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV32.png)

* Modelling in VECTO - Further proposed assumptions
  * Battery charging during mission
    * 50% of usable battery capacity via "SOC lift" by IMC
    * No further stationary charging
  * Air drag
    * Considered only for "Overhead - pantograph"
    * ΔCdxA = 0\.6 m² for the shares in the mission where IMC is available

Inputs in Graphical User Interface \(ENG\)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV33.png)

New tab in "VEH Editor" for HEV and PEV vehicles

**Attention!**

Working with ENG mode simulations for IMC features requires detailed knowledge of the proposed approach \(mainly for developers\)

For checking of regulatory impact DECL mode testing recommended\!

Inputs in Graphical User Interface \(DECL\)

![](amdm3img%5CRelease%20Notes%20Vecto0x-DEV34.png)

* Dropdown Menu with pre\-defined technologies
  * None
  * Overhead Pantograph
  * Overhead Trolley
  * Ground Rail
  * Wireless

* Example vehicle models will be distributed with the release \(located in "\\Generic vehicles"\)
* Base specs example models
  * Group 5 / HEV P2 / OVC: P_cont_EM: 200kW | E_Reess: 120kWh nominal
  * Group 5 / PEV: P  cont\_EM  : 325kW | E  Reess  : 170kWh nominal
* Example results: Long haul with rep payload  Comparison results "w/o" and "w/" pantograph with similar other vehicle specs like mass
  * Group 5 / P2 / OVC w/o pantograph: UF = 0\.149; CO2  weighted = 692\.2 g/km
  * Group 5 / P2 / OVC _w/_ pantograph: UF = 0\.660; CO2_weighted = 347\.1 g/km -> \-50% CO2
  * Group 5 / PEV w/o pantograph: EC_el = 1\.262 kWh/km
  * Group 5 / PEV   w/ pantograph: EC_el = 1\.297 kWh/km -> +2.8% EC_el

