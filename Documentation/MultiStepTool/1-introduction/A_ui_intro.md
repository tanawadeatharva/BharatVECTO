
# User Interface Overview


The main UI consists of three tabs.

- [Jobs](#jobs)
- [Settings](#settings)
- [About](#about)





## Jobs
The Jobs tab shows an overview of currently loaded jobs and allows the loading, editing and simulation of jobs.
Note: Also jobs that cannot be edited with the GUI (i.e Primary Vehicle Files), can be loaded and simulated.



![](images/1-ui-overview/overview1-edited.png){width=100%}\
\


### Simulation- and Job Commands

- **Simulation**: 
    Starts the simulation of all jobs that are marked for simulation.
- **Stop**:
    Stops the current simulation
- **Up**: 
    Moves the currently selected job up
- **Down**:
    Moves the currently selected job down

### Joblist
The joblist shows the name, type and file location of all currently loaded jobs. 

#### Adding Jobs
New jobs from existing files can be added to this list with the buttons in the [File Section](#file-section), Drag & Drop or using **File -> Load File**. New Jobs can be created using **File -> New File** (explained in more detail in the [Create a new Job](#create-a-new-job) section).

#### Simulation
By ticking the checkbox in the first column, a job is marked for simulation.
If no simulation is possible, a small icon is displayed instead of the checkbox. Clicking the icon shows why the job can not be simulated.

#### Edit Jobs
A job can be edited by double clicking or using the **Edit Job** button in the [File Section](#file-section).
It is also possible to show the source file of the currently selected job in the explorer or in the default editor using **Edit -> Source File -> Show in Explorer**  or **Edit -> Source File -> Open in Editor**.

### File Section
- **Load File**: 
    Load a new job from a .xml or .vecto file.
- **Edit**:
    Edit the selected job.
- **Remove**: 
    removes the selected job from the list.


### Messages


![](images/1-ui-overview/simulation-section1-edited.png){width=100%}\
\

The message section display status messages from the simulation (Error, Info, Warning etc.) as well as the total progress of the simulation.

The status bar shows the progress of the individual simulation runs. 



