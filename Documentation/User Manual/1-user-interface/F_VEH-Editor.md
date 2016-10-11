##Vehicle Editor

![](pics/VEH-Editor.png)

###Description

The [Vehicle File (.vveh)](#vehicle-file) defines the main vehicle/chassis parameters like axles including [RRC](#rolling-resistance-coefficient)s, air resistance and weight.

###Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths.
Example: "Demo\\RT1.vrlm" points to the "Demo" subdirectory of the Vehicle File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Retarder Losses File) is in the same directory as the Vehicle File. (*Note:* The Vehicle File must be saved before browsing for input files.)

###General vehicle parameters

Vehicle Category
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Class.

Axle Configuration
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Class.

Gross Vehicle Mass Rating [t]
: Needed for [Declaration Mode](#declaration-mode) to identify the HDV Class.

HDV Class
: Displays the automatically selected HDV Class depending on the settings above.

###Weight/Loading

These fields define the weight and loading of the vehicle. **Max. Loading** displays the maximum possible loading for the selected vehicle depending on curb weight and GVW values.

***Note:*** *VECTO uses the sum of* ***Curb Weight Vehicle, Curb Weight Extra Trailer/Body*** *and* ***Loading*** *for calculation!*

###Air Resistance

The product of Drag Coefficient [-] and Cross Sectional Area [m²] (**c~d~ x A**) and **Air Density** [kg/m³] (see [Settings](#settings)) together with the vehicle speed defines the Air Resistance. Note that the Air Drag depends on the chosen [**Cross Wind Correction**](#cross-wind-correction).


###Dynamic Tyre Radius

In [Engineering Mode](#engineering-mode) this defines the effective (dynamic) wheel radius (in [mm]) used to calculate engine speed. In [Declaration Mode](#declaration-mode) the radius calculated automatically using tyres/rims of the powered axle.


###Cross Wind Correction Options

Four different options are available:
: -  No Correction: The specified CdxA value is used to compute the air drag, no cross-wind correction is applied
-  Speed dependent (User-defined): The specified CdxA value is corrected depending on the vehicle's speed.
-  Speed dependent (Declaration Mode): A uniformly distributed cross-wind is assumed and used for correcting the air-drag depending on the vehicle's speed
-  Vair & Beta Input: Correction mode if the actual wind speed and wind angle relative to the vehicle have been measured.

In delcaration mode the 'Speed dependent (Declaration Mode)' cross-wind correction is used.

Depending on the chosen mode either a [Speed Dependent Cross Wind Correction Input File (.vcdv)](#speed-dependent-cross-wind-correction-input-file-.vcdv) or a [Vair & Beta Cross Wind Correction Input File (.vcdb)](#speed-dependent-cross-wind-correction-input-file-.vcdv) must be defined. For details see [Cross Wind Correction](#cross-wind-correction).


###Axles/Wheels

For each axle the parameters **Relative axle load, RRC~ISO~** and **F~zISO~** have to be given in order to calculate the total [Rolling Resistance Coefficient](#rolling-resistance-coefficient).
Furthermore the **Wheels Inertia [kgm²]** has to be set per wheel for each axle. In [Declaration Mode](#declaration-mode) the inertia is computed based on the selected tyres and rims. The number of axles specified have to match the vehicle type (e.g., 2 axles for a 4x2 truck).

<div class="declaration">
In [Declaration mode](#declaration-mode) only the axles of the truck have to be given. For the trailer predefined wheels and weight-shares are used.
</div>

<div class="engineering">
In [Engineering Mode](#engineering-mode) all axles, for both  truck and trailer, have to be given.
</div>

Use the ![](pics/plus-circle-icon.png) and ![](pics/minus-circle-icon.png) buttons to add or remove axles form the vehicle. Doubleclick entries to edit existing axle configurations.

###Retarder Losses

If a separate retarder is used in the vehicle a **Retarder Torque Loss Map** can be defined here to consider idling losses caused by the retarder.

Four options are available:
: -   No retarder
-	Included in Transmission Loss Maps: Use this if the [Transmission Loss Maps](#transmission-loss-map) already include retarder losses.
-   Primary Retarder (before gearbox): The rpm ratio is relative to the engine speed
-   Secondary Retarder (after gearbox): The rpm ratio is relative to the cardan shaft speed

Both, primary and secondary retarders, require an [Retarder Torque Loss Input File (.vrlm)](#retarder-loss-torque-input-file-.vrlm).


###Angular Gear

If an angular gear is used in the vehicle, it can be defined here.
Three options are available:

- No angular gear (**default**)
- Separate angular gear: Use this if the angular gear is measured separately. In this case the ratio must be set and the [Transmission Loss Map](#transmission-loss-map) (or an Efficiency value) must also be given.
- Included in transmission: Use this if the gearbox already includes the transmission losses for the angular gear in the respective transmission loss maps.


###PTO Transmission

If the vehicle has an PTO consumer, a pto transmission and consumer can be defined here.

Three settings can be set:

- PTO Transmission: Here a transmission type can be chosen (adds constant load at all times).
- PTO Consumer Loss Map (.vptol): Here the [PTO Idle Loss Map](#pto-idle-consumption-map-.vptoi) of the pto consumer can be defined (adds power demand when the pto cycle is not active).
- PTO Cycle (.vptoc): Defines the [PTO Cycle](#pto-cycle-.vptoc) which is used when the pto-cycle is activated (when the PTO-Flag in the driving cycle is set).

###Controls


![](pics/blue-document-icon.png) New file
: Create a new empty .vveh file

![](pics/Open-icon.png) Open existing file
: Open an existing .vveh file

![](pics/Actions-document-save-icon.png) ***Save current file***

![](pics/Actions-document-save-as-icon.png) ***Save file as...***

![](pics/export-icon.png) Send current file to the [VECTO Editor](#job-editor)
: **Note:** If the current file was opened via the [VECTO Editor](#job-editor) the file will be sent automatically when saved.

![](pics/OK.png) Save and close file
: If necessary the file path in the [VECTO Editor](#job-editor) will be updated.

![](pics/Cancel.png) ***Cancel without saving***
