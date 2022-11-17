## Auxiliary Dialog


<div class="declaration">
![Auxiliary Dialog (Declaration Mode)](pics/VECTO-Editor_Aux_DECL.jpg)

### Description


The Auxiliary Dialog is used to configure auxiliaries. In [Declaration Mode](#declaration-mode) the set of auxiliaries and their power demand is predefined. For every auxiliary the user has to select the technology from a given list. 

### Settings

Technology
:   List of available technology for the auxiliary type
For the  steering pump multiple technologies can be defined, one for each steered axle.

### Controls

![ok](pics/OK.png) ***Save and close***

![cancel](pics/Cancel.png) ***Close without saving***

</div>

<div class="engineering">
In Engineering Mode the auxiliary power demand can either be specified in the driving cycle over distance (or time), specified as constant load, or via the bus auxiliaries. For more details see [the Auxiliaries tab in the Job editor](#job-editor).
</div>


## BusAuxiliary Dialog

<div class="engineering">

![](pics/BusAux_Engineering.png)

In Engineering Mode the electrical and mechanical power demand for the electric system, the pneumatic system and the HVAC can be provided.

#### Electric System

Current Demand Engine On \[A\]
:   Demand of the electric system when the ICE is on. The current is multiplied with the nominal voltage of 28.3V.

Current Demand Engine Off Driving \[A\]
:   Demand of the electric system when the ICE is off and the vehicle is driving. The current is multiplied with the nominal voltage of 28.3V.

Current Demand Engine Off Standstill \[A\]
:   Demand of the electric system when the ICE is off and the vehicle is at standstill. The current is multiplied with the nominal voltage of 28.3V.

Alternator Efficiency  \[-\]
:   The electric power demand is divided by the alternator efficiency to get the mechanical power demand at the crank shaft

Alternator Technology
:   The "conventional alternator" generated exactly the electric power as demanded by the auxiliaries. The "smart alternator" may generate more electric power than needed during braking phases. The excessive electric power is stored in a battery. In case "no alternator" is selected (only available for xEV vehicles) the electric system is supplied from the high voltage REESS via a DC/DC converter.

Max Recuperation Power \[W\]
:   In case of a smart alternator, defines the maximum electric power the alternator can generate during braking phases.

Useable Electric Storage Capacity \[Wh\]
:   In case of a smart alternator, defines the storage capacity of the battery. In case the battery is not empty, the electric auxiliaries are supplied from the battery. Excessive electric energy from the smart alternator during braking phases is stored in the battery.

Electric Storage Efficiency \[-\]
:   This efficiency is applied when storing electric energy from the alternator in the battery.

ESS supply from HEV REESS
:   If selected, the low-voltage electric auxiliaries can be supplied from the high voltage REESS via the DC/DC converter. Needs to be selected in case "no alternator" is chosen as alternator technology. In case of a smart alternator, the low-voltage battery is used first and if empty the energy is drawn from the high voltage system.

DC/DC Converter Efficiency  \[-]\
:	TODO

#### Pneumatic System

Compressor Map
:   [Compressor map file](#advanced-compressor-map-.acmp) defining the mechanical power demand and the air flow depending on the compressor speed.

Average Air Demand  \[NI/s\]
:    Defines the average demand of compressed air throughout the cycle.

Compressor Ratio \[-\]
:    Defines the ratio between the air compressor and combustion engine

Smart Air Compressor
:    If enabled, the air compressor may generate excessive air during braking events. The air consumed and generated are [corrected in post processing](#engine-fuel-consumption-correction).

#### HVAC System

Mechanical Power Demand  \[W\]
:   Power demand of the HVAC system directly applied at the crank shaft

Electric Power Demand  \[W\]
:   Electric power demand of the HVAC system. This is added to the current demand of the electric system

Aux Heater Power  \[W\]
:   Maximum power of the auxiliary heater

Average Heating Demand  \[MJ\]
:   Heating demand for the passenger compartment. This demand is primary satisfied from the combustion engines waste heat. In case the heating demand is higher, the auxiliary heater may provide additional heating power. The fuel consumption of the aux heater is [corrected in post processing](#engine-fuel-consumption-correction).

</div>	
