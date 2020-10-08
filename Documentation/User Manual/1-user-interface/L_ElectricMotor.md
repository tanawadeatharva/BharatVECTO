##Electric Motor Editor

![](pics/VECTO_ElectricMotor.png)


###Description

The electric motor file defines all parameters relevant for the electric machine. These are the motor's maximum drive and recuperation torque, the drag torque as well as the electric power map.

###Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. 

VECTO automatically uses relative paths if the input file (e.g. elctric power map) is in the same directory as the Electric Motor File. (The Electric Motor File must be saved before browsing for input files.)


###Main Parameters

Make and Model
:   Free text defining the gearbox model, type, etc.

Inertia \[kgm²\]
:   Rotational inertia of the gearbox (constant for all gears). (Engineering mode only)

Max. Drive and Max. Generation Torque Curve
:   Torque over engine speed the electric motor can apply on its output shaft. (see [Electric Motor Max Torque File (.vemp)](#electric-motor-max-torque-file-.vemp))

Drag Torque Curve
:   The motor's drag torque over engine speed when the motor is not energized. The torque values in the drag curve have to be negative. (see [Electric Motor Drag Curve File (.vemd)](#electric-motor-drag-curve-file-.vemd))

Electric Power Consumption Map
:   Defines the electric power that is required to provide a certain mechanical power (torque and angular speed) at the motor's shaft. This map is used to calculate the electric power demand. The electric power consumption map shall cover a torque range exceeding the max. drive and max. generation torque and shall cover the speed range from 0 up to the maximum speed. (see [Electric Motor Map (.vemo)](#electric-motor-map-.vemo))



###Chart Area

The Chart Area displays the electric machine's max. drive curve and max. generation curve (blue), the drag curve (green) and the entries provided in the electric power consumption map (red).


###Controls



![](pics/blue-document-icon.png) New file
:   Create a new empty .vem file

![open](pics/Open-icon.png)Open existing file
:   Open an existing .vem file


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

