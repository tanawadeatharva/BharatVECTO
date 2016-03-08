##Gearbox Editor



![](pics/GBX-Editor.svg)


###Description



The [Gearbox File (.vgbx)](#gearbox-file) defines alls gearbox-related input parameters like gear ratios and transmission loss maps. See [Gear Shift Model](#gear-shift-model) for details.


###Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. \
Example: "Gears\\Gear1.vtlm" points to the "Gears" subdirectory of the Gearbox File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Shift Polygons File) is in the same directory as the Gearbox File. (The Gearbox File must be saved before browsing for input files.)


###Main Gearbox Parameters

Make and Model
:   Free text defining the gearbox model, type, etc.


Transmission Type
:   Depending on the transmission type some options below are not available. The following types are available:
:   -   **MT**: Manual Transmission
-   **AMT**: Automated Manual Transmission
-   **AT**: Automatic Transmission
-   **Custom**
:	Note: The types AT and Custom are not available in [Declaration Mode](#declaration-mode).


Inertia \[kgm²\]
:   Rotational inertia of the gearbox (constant for all gears). (Engineering mode only)


Traction Interruption \[s\]
:   Interruption during gear shift event. (Engineering mode only)


###Gears

Use the ![add](pics/plus-circle-icon.png) and ![remove](pics/minus-circle-icon.png) buttons to add or remove gears from the vehicle. Doubleclick entries to edit existing gears.

-   Gear **"A"** defines the ratio of the axle transmission / differential.
-    **"TC"** (AT only) defines which gears are using the torque converter (lock-up clutch open).
-    **"Ratio"** defines the ratio between the output speed and input speed for the current gear. Must be greater than 0.
-    **"Loss Map or Efficiency"** allows to define either a constant efficiency value or a [loss map (.vtlm)](#transmission-loss-map). <span class="vecto3">Note: in Vecto 3 it is mandatory to specify a loss map for every gear!</span>
-    **"Shift polygons"** defines the [Shift Polygons InputFile (.vgbs)](#shift-polygons-input-file-.vgbs) for each gear. Not required in [Declaration Mode](#declaration-mode). See [GearShift Model](#gear-shift-model) for details.
-	 **"Full Load Curves"** defines the [Full Load Curve for (.vfld)](#full-load-and-drag-curves-.vfld) each gear. It is used for torque limiting in the current gear. Note: in Declaration mode the [generic shift polygons](#gear-shift-model) are computed from the engine's full-load curve. If the maximum torque is limited by the gearbox, the minimum of the gearbox and engine maximum torque will be used to compute the [generic shift polygons](#gear-shift-model)!


###Gear shift parameters


![](pics/checkbox.png) Allow shift-up inside polygons
:   See [Gear Shift Model](#gear-shift-model).

![](pics/checkbox.png) Skip Gears
:   See [Gear Shift Model](#gear-shift-model).


Torque Reserve \[%\]
:   This parameter is required for the **Allow shift-up inside polygons** and **Skip Gears** options.


Minimum shift time \[s\]
:   Limits the time between two gear shifts. This rule will be ignored if rpms are too high or too low. <span class="vecto2">Vecto 2.2 uses fixed time-steps of 1 second, hence only whole seconds can be specified.</span>
<span class="vecto3">Vecto 3 uses dynamic time-steps, hence any values greater than 0 seconds can be given.</span>


Start Gear
:   In order to calculate an appropriate gear for vehicle start (first gear after vehicle standstill) a fictional load case is calculated using a specified **reference vehicle speed** and **reference acceleration** together with the actual road gradient, transmission losses and auxiliary power demand. This way the start gear is independent from the target speed. VECTO uses the highest possible gear which provides the defined **torque reserve**.


###Chart Area



The Chart Area displays the [Shift Polygons Input File(.vgbs)](#shift-polygons-input-file-.vgbs) for the selected gear.


###Torque Converter

<div class="vecto2">

The [Torque Converter Model](#torque-converter-model) is still in development.

Inertia \[kgm²\]
:   Rotational inertia of the engine-side part of the torque converter.
(Gearbox-side inertia is not considered in VECTO.)


###Controls



![](pics/blue-document-icon.png) New file
:   Create a new empty .vgbx file

![open](pics/Open-icon.png)Open existing file
:   Open an existing .vgbx file


![save](pics/Actions-document-save-icon.png) ***Save current file***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save file as...***

![sendto](pics/export-icon.png) Send current file to the [VECTO Editor](#job-editor)
:   **Note:** If the current file was opened via the [VECTO Editor](#job-editor) the file will be sent automatically when saved.


![](pics/browse.png) ***Open file browser***

![](pics/OpenFile.PNG) ***Open file*** (see [File Open Command)](#settings).

![OK](pics/OK.png) Save and close file
:   If necessary the file path in the [VECTO Editor](#job-editor) will
be updated.


![Cancel](pics/Cancel.png) ***Cancel without saving***
</div>