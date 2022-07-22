## IHPC Editor

![](pics/IHPC_Form.png)


### Description

Integrated hybrid electric vehicle powertrain component (IHPC) means a combined system of multiple electric machine systems together with the functionality of a multi-speed gearbox.


### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. 

VECTO automatically uses relative paths if the input file (e.g. electric power map) is in the same directory as the Electric Motor File. (The Electric Motor File must be saved before browsing for input files.)


### Main Parameters

Make and Model
:   Free text defining the gearbox model, type, etc.

Inertia \[kgm²\]
:   Rotational inertia of the electric machine defined at the output shaft of the EM. (Engineering mode only)

Continuous Torque \[Nm\]
:    The nominal torque the electric machine can provide continuously

Continuous Torque Speed \[rpm\]
:    Angular speed at which the continuous torque can be provided

Overload Torque \[Nm\]
:    Maximum torque above the continuous torque the electric motor can provide for a certain time

Overload Torque Speed \[rpm\]
:    Angular speed at which the overload torque was measured

Overload Duration \[s\]
:    The time interval the electric machine can operate at its peak performance

Thermal Overload Recovery Factor  \[-\]
:    The accumulated overload energy has to be below the max. overload capacity multiplied by this factor so that the peak power is available again.

Full Load Curve
:   TODO 

Power Map Per Gear
:   Defines the electric power that is required to provide a certain mechanical power (torque and angular speed) at the motor's shaft. This map is used to calculate the electric power demand. The electric power consumption map shall cover a torque range exceeding the max. drive and max. generation torque and shall cover the speed range from 0 up to the maximum speed. (see [Electric Motor Map (.viepco)](#electric-motor-power-map-.vemo)). The power map has to be provided for two different voltage levels and all gears.

Voltage Level Low/High
:    Applicable voltage level for the electric power consumption map and max drive/generation torque curve





### Controls


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

