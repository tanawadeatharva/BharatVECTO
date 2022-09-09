## Transmission Losses

Every transmission component (gearbox, angledrive, axle gear, ...) uses the following formula for calculating the torques at input and output side of the component:

$T_{output} = (T_{input} - T_{loss}) * r_{gear}$

with:

* T~output~ ... Output torque
* T~input~ ... Input torque
* T~loss~ ... Torque loss (from e.g. a loss map or efficiency for that component)
* r~gear~ ... The transmission ratio for the current gear (if the component has ratios)


The following components are accounted as transmission components (see [Powertrain and Components Structure](#powertrain-and-components-structure) for a complete overview over all components in the powertrain):

* [Gearbox](#gearbox-editor)
* Axle Gear (see [Gearbox](#gearbox-editor))
* Angledrive (see [Vehicle](#vehicle-editor-powertrain-tab))

