##Driver: Overspeed


Both functions control the vehicle's behaviour on uneven road sections (slope ≠ 0) and can be configured in the [Job File](#job-file)'s Driver Assist Tab. Overspeed is designed to model an average driver's behaviour without the aid of driver assistance systems. Eco-Roll  represents an optional driver assistance feature. For this reason vehicles without Eco-Roll should always have the Overspeed function enabled.


###Overspeed


Overspeed activates as soon as the total power demand at the wheels (Pwheel) falls below zero, i.e. the vehicle accelerates on a negative slope. The clutch remains closed, engine in motoring operation, and the vehicle accelerates beyond the cycle's target speed. When the speed limit (target speed plus **Max. Overspeed**) is reached the mechanical brakes are engaged to prevent further acceleration.


![](pics/Overspeed.svg)

*Example with target (purple) and actual speed (orange) on the top left axis, slope (brown) on the top right axis. The bottom graph shows engine power (blue), motoring curve (orange) and mechanical brake power (green). In this example Overspeed is allowed until the vehicle's speed exceeds target speed by 5 \[km/h\].*


Parameters in [Job File](#job-file):
:	-   **Minimum speed \[km/h\]**. Below this speed the function is disabled.
-   **Max. Overspeed \[km/h\]** (relative to target speed)


##Advanced Driver Assistant Systems, Eco-Roll, Engine Stop/Start

###Engine Stop/Start

If engine stop/start is enabled in the Vehicle, the engine is turned off during vehicle stops to reduce the fuel consumption. During vehicle stops the energy demand for certain auxiliaires and for starting the engine is accumulated. In a post-processing step the final [fuel consumption is corrected](#engine-stopstart-fuel-consumption-correction) to consider the energy demand for the auxiliaries and engine start.

<div class="declaration">
In declaration mode, the engine is switched on after a period of 120 seconds of engine-off.
</div>

**Engine Start-Up Energy Demand**

The energy demand to ramp-up the engine depends on the engine's inertia and the engine's drag torque and is computed according to the following equation:

$E_{ICE,rampUp} = 0.5 * I_{ICE} * n_{idle}^2 + T_{drag}(n_{idle}) * n_{idle} / 2 * t_{ICE,start}$

$E_{ICE,start} = E_{ICE,rampUp} / \eta_{alternator}^2$


$E_{ICE,start}$ is the amount of energy the combustion engine needs to provide to compensate the start up is the ramp-up energy multiplied by the efficiency of the alternator.  $t_{ICE,start}$ is assumed to be 1 second and $\eta_{alternator}$ is 0.7.

**Utility Factor**

Engine Stop/Start is usually not activated at every vehicle stop. This is considered in VECTO via a utility factor (e.g. 0.8). This utility factor (f) is applied for every engine stop as follows:

   - the auxiliary demand during engine stops is multiplied by the utility factor
   - the fuel consumption FC_final during engine stop is the fuel consumption with the engine idling and all auxiliaires on multiplied by  1-f
   - the energy demand for starting the engine is multiplied by the utility factor

<div class="declaration">
In declaration mode the utility factor is set to 0.8.
</div>

<div class="declaration">
**Auxiliary energy demand**

In Declaration Mode the energy demand of all auxiliaries except the engine cooling fan and the steering pump is considered during vehicle stops.

</div>

<div class="engineering">
**Auxiliary energy demand**

In Engineering Mode the energy demand of all auxiliaries is assumed to be drawn also during engine stop periods and the fuel consumption is corrected in a post-processing step.
</div>
