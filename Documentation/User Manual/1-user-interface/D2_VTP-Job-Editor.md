##VTP-Job Editor


![](pics/VTP-Job.png)


###Description

The [VTP job file (.vecto)](#vtp-job-file) includes all informations to run a VECTO calculation. It defines the vehicle and the driving cycle(s) to be used for calculation. In summary it defines:

-   Filepath to the Vehicle File (.xml)](#vehicle-editor which defines all relevant parameters, including all components
-   Driving Cycles


###Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. Example: "Vehicles\\Vehicle1.xml" points to the "Vehicles" subdirectory of the Job File's directoy.

VECTO automatically uses relative paths if the input file (e.g. Vehicle File) is in the same directory as the Job File. (*Note:* The Job File must be saved before browsing for input files.)



Cycles
:	List of cycles used for calculation. The .vdri format is described [here](#driving-cycles-.vdri).
**Double-click** an entry to open the file (see [File Open Command](#settings)).
**Click** selected items to edit file paths.

: ![addcycle](pics/plus-circle-icon.png) Add cycle (.vdri)
: ![remcycle](pics/minus-circle-icon.png) Remove the selected cycle from the list



###Chart Area

If a valid Vehicle File is loaded into the Editor the main vehicle parameters like HDV class and axle configuration are shown here. The plot shows the full load curve(s) and sampling points of the fuel consumption map. 

###Controls

![new](pics/blue-document-icon.png) New Job File
:	Create a new empty .vecto file

![open](pics/Open-icon.png) Open existing Job File
:	Open an existing .vecto file

![save](pics/Actions-document-save-icon.png) ***Save current Job File***

![SaveAs](pics/Actions-document-save-as-icon.png) ***Save Job File as...***

![sendto](pics/export-icon.png) Send current file to Job List in [Main Form](#main-form)
:	**Note:** The file will be sent to the Job List automatically when saved.

![](pics/browse.png) ***Browse for vehicle file***

![OK](pics/OK.png) Save and close file
:	File will be added to Job List in the [Main Form](#main-form).

![Cancel](pics/Cancel.png) ***Cancel without saving***
