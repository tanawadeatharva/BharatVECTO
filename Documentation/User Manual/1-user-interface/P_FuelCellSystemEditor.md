
## Fuel Cell System Editor
#### Description

In the Fuel Cell System Editor a new FCS file can be created or an existing file can be updated.
For more info on the input files see [Fuel Cell System Input File](#fuel-cell-system-input-file) and 
[Fuel Cell Mass Flow Map (.vfcm)](#fuel-cell-system-mass-flow-map)
#### Main Parameters

Manufacturer and Model
:   Free text defining the model, type, etc.

Min Electric Power \[kW\]
:   The minimum electric power that can be provided by the fuel cell system

Max Electric Power \[kW\]
:   The maximum electric power that can be provided by the fuel cell system

MassFlowMap
:   Path to the MassFlowMap file (.vfcm) containing the fuel consumption in g/h depending on the power in kW



#### Massflow Map Graph

In the graph, a preview of the loaded mass flow map file (.vfcm) can be found. The graph only shows the entries of the loaded files, power limits are not considered.

![](pics/FuelCell/fuel_cell_system_form.png)


#### Relative File Paths

It is recommended to use relative filepaths. This way the Job File and all input files can be moved without having to update the paths. 



