# Create a new Job

New jobs and files can be created via the file menu.

![](images/2-new-job/new_job_1_edited.png){width=70%}\
\


## New Interim/Completed Job - General Case

Used to create the VIF for a complete or a interim job. 
![](images/2-new-job/general-case.png){width=70%}\
\


![](images/2-new-job/new_vif_edited.png){width=100%}\
\


### 1 Load VIF from previous stage

### 2 (optional) Load input for current step
Load exisitng XML for interim/completed step.

### 3 The input data is split into three components.
- **Vehicle**

- **Airdrag** []

- **Auxiliaries**


### 4 Mandatory input fields for every simulation step.
Red borders indicate missing or wrong parameters.

### 5 Consolidated Data
The left column is readonly and shows the consolidated data from the previous steps.

### 6 Enable editing
To edit a parameter in the current stage, the editing has to be enabled by checking the corresponding checkbox.

### 7 Current stage input Data
In this column the input data of the current step is displayed, and can be edited.

### 8 Save and Close
- **Save Input As ...**:\
    Saves the input of the current step to a new file.
- **Save Input**: \
    Saves the input of the current step to the loaded input file.
- **Save as new VIF**: \
    creates a new VIF and adds it to the job list. If 'Vehicle Declaration Type' is set to 'Final' and all required parameters are present, the file can directly be simulated.
- **Close**:\
    Closes the window. Note: As long as VECTO is open and the job is not removed from the job list, unsaved changes remain in the job for further editing.


## New Primary Job with Interim Input - Special Case I
![](images/2-new-job/special-case-1.png){width=70%}\
\

Allows creating a VIF from a primary bus input and a step input in one step. The job is automatically added to the job list for further editing, but simulation is only possible when the job is saved.

![](images/2-new-job/special-case-1-edited.png){width=70%}\
\

### 1 Load primary input file
In the first simulation step the primary input is simulated and an intermediate VIF and a MRF are created. The step input is then appended and the resulting VIF is written to disk.

### 2 Select step input file
Note: If an exempted primary input is loaded, the step input has also be for an exempted vehicle.

### 3 Save and Close
After saving the file the job can be simulated.

## New Complete Job - Special Case II
![](images/2-new-job/special-case-2.png){width=35%}

The steps to create a complete job are the same as creating a [primary job with interim input](#new-primary-job-with-interim-input---special-case-i).

Note: If the VIF resulting from the primary input and the step input, cannot be simulated, add the resulting VIF to the job list and click the info icon to get additional information.


# Create a step input file
As mentioned before it is possible to load step input files when creating new jobs. They can be created externally, while creating or editing a new interim/completed job or using **File -> New File -> Create Interim/Completed Job**.


![](images/2-new-job/new_step_input_1_edited.png){width=70%}

When creating a new step input, the step input is automatically added to the job list for further editing.

![](images/2-new-job/new_step_input_2_edited.png){width=70%}

### 1 Mandatory fields
Mandatory fields don't have a checkbox and are marked red while empty.

### 2 Enable editing
To edit an optional parameter, this checkbox has to be ticked.

### 3 Parameters for step input

### 4 Save and Close
Save the input data to .xml, the data will still be editable via the job list as long as Vecto was not closed and the file was not removed from the joblist.

