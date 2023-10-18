## Integrated Hybrid Electric Powertrain Component (IHPC)

Integrated hybrid electric powertrain component (IHPC) means a combined system of multiple electric machine systems together with the functionality of a multi-speed gearbox. An electric machine of type IHPC has to be used together with a gearbox of type IHPC.

The IHPC is modeled by the following parameters and map files:

- Maximum drive torque over rotational speed (related to the output shaft)
- Maximum generation torque over rotational speed (Related to the output shaft)
- Drag curve (shall be 0 as the drag losses are covered by the gearbox model)
- Electric power map for all mechanical gears
- Continuous torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Overload torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Maximum overload time

The first two curves are read from a .viepcp file (see [IEPC Max Torque File (.viepcp)](#iepc-max-torque-file-.viepcp)). The drag curve(s) are provided in .vemd file(s) (see [Electric Motor Drag Curve File (.vemd)](#electric-motor-drag-curve-file-.vemd)) and the electric power maps in .viepco file(s) (see [IEPC Power Map (.viepco)](#iepc-power-map-.viepco)). For IHPCs all maps and max. drive/generation torque files refer to the output shaft just like for IEPCs. The values of the drag torque must be set to zero because the drag losses are already included in the IHPC gearbox loss maps due to the virtual split of electric machine and gearbox in the simulation.

The following paragraph should provide a clear understanding on the modeling aproach and the implementations/adaptions made in VECTO in order to properly depict the functionality of IHPCs.

An IHPC is a component in a parallel HEV powertrain which combines the functionality of the EM with a shiftable transmission in one single unit. The exact characteristics of this system are defined in point 2(38) of Annex Xb of Commission Regulation (EU) 2017/2400. The figure below shows the principles of the arrangement of an IHPC in a vehicle powertrain.

![](pics/IHPC arrangement in vehicle powertrain.png)

The IHPC is treated for the component test as a black box with three interfaces to other components of the powertrain over which power can be transferred. Two of those are of mechanical type and connected to the ICE and the axle of the vehicle. The third one is of electrical type and connected to the REESS.

![](pics/IHPC to other powertrain components.png)

As the IHPC system operates predominantly as a parallel HEV in the relevant driving situations represented in the VECTO simulation, it is treated as such in order to fit into the established modular powertrain design of VECTO. Therefore, the individual functionalities of the black box IHPC have to be split into two virtual separate systems, represented by the parallel powertrain architecture ID "P2" - the one that most closely resembles the IHPC architecture. This means that instead of a single IHPC component, there are two separate standalone components - i.e. EM and transmission - in the vehicle powertrain as simulated by VECTO.
In order to be able to parameterize those two standalone components, two separate testruns are required which are based on already existing tests for the components substituting the IHPC:

1.	The total system performance and losses (sum of EM and gearbox losses) are determined following the Annex Xb procedure for EM – but measured separately for each gear at the output shaft of the system.
The boundary conditions of this testrun are that the system shaft for connecting the ICE is allowed to rotate freely (no torque fed into the system) and the system is only driven by electric power supplied.

![](pics/IHPC testrun 1.png)

2.	The gearbox specific losses are determined in accordance with Annex VI for transmissions of Commission Regulation (EU) 2017/2400.
The boundary conditions of this testrun are that the system shaft for connecting the ICE is driven by a dyno (specific propulsion torque fed into the system to maintain a defined operation point at the output) whereas the electric power supply is disconnected. Meaning the difference in power between the two dynos at the input and output side represents the drag losses of the EM as well as the gearbox specific losses.

![](pics/IHPC testrun 2.png)

Based on the results of these two testruns, the specific component characteristics for EM and transmission used as substitute for the IHPC are determined by the following post-processing steps:

* All EM data measured at the IHPC output shaft needs to be converted to the EM shaft and the gearbox losses need to be deducted:

    * $\textrm{T}_\textrm{EM} = \textrm{T}_{out-meas} / \textrm{i}_\textrm{GBX} - \textrm{T}_{loss-GBX} (\textrm{n}_{EM},\textrm{T}_{EM})$
    * $\textrm{n}_\textrm{EM} = \textrm{n}_{out-meas} * \textrm{i}_\textrm{GBX}$

* For the transmission component, the losses determined in testrun 2 can be applied directly following the provisions in Annex VI of Commission Regulation (EU) 2017/2400 for transformation of the losses referring to the conditions at the gearbox input side.
* The drag torque curve of the EM component is set to zero since these losses are already included in the transmission losses determined in testrun 2 (as described above).

The specific rules for parameterization of an IHPC on vehicle level using the two separate components substituting the IHPC are described in point 10 of Annex III as well as point 4.4.3 of Annex Xb of Commission Regulation (EU) 2017/2400.


