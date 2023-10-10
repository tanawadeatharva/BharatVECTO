## Integrated Electric Powertrain Component (IEPC)

Integrated electric powertrain component (IEPC) means a combined system of an electric machine system together with the functionality of either a single- or multi-speed gearbox or a differential or both. 

An IEPC can be of design-type wheel motor which means that the output shaft (or two output shafts) are directly connected to the wheel hub(s).

The IEPC is modeled by the following parameters and map files:

- Maximum drive torque over rotational speed (related to the output shaft)
- Maximum generation torque over rotational speed (Related to the output shaft)
- Gear ratios of all gears (electric motor to out shaft)
- Drag curve. either for a single gear (measured with the gear with the ratio closest to 1) or all gears
- Electric power map for all mechanical gears
- Continuous torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Overload torque and rotational speed (output shaft), measured with the gear with the ratio closest to 1
- Maximum overload time

The first two curves are read from a .viepcp file (see [IEPC Max Torque File (.viepcp)](#iepc-max-torque-file-.viepcp)). The drag curve(s) are provided in .viepcd file(s) (see [IEPC Drag Curve File (.viepcd)](#iepc-drag-curve-file-.viepcd)) and the electric power maps in .viepco file(s) (see [IEPC Power Map (.viepco)](#iepc-power-map-.viepco)). It is important to note that for the IEPC all maps are related to the output shaft speed (including all integrated components of the IEPC).

The following paragraph should provide a clear understanding on the modeling aproach and the implementations/adaptions made in VECTO in order to properly depict the functionality of IEPCs.

In the VECTO simulation, the IEPC component is virtually split up into the electric machine (with gear-dependent electric power maps), an APT-N gearbox in case of a multi-speed gearbox and optionally an axle gear using already available models and the same shift strategy as for E2/S2 architectures. All speed and torque values from the component certification are automatically converted from the measurement reference point at the output shaft of the IEPC via the respective gear ratios for each forward gear to the input shaft of the “virtual” transmission component used in VECTO (which is also reflecting the output shaft of the “virtual” EM inside the IEPC).
In case of a single-speed IEPC, a simplified transmission component was implemented which requires only one single transmission ratio as compared to the shiftable transmission.
Since the measured component data of the IEPC includes also the losses of the gearbox specific parts, the existing VECTO transmission component model is automatically parameterized with a generic dataset depicting a loss-free transmission component, which is only responsible for handling the different transmission ratios for the gear shifting process. Thus, the simulation of an IEPC is similar to E2 vehicles in case of a multi-speed gearbox or an E3 vehicle in case of a single-speed gearbox.
For IEPCs with no differential included, in addition to the functions covered by the IEPC (EM and gearbox), an axle component must be configured for the vehicle. However, in the existing architecture of VECTO, input to gearbox and axle are linked. Therefore, to enable the input of an axle component without the need to define at additional gear ratios, as those are already implicitly defined by the IEPC inputs, a dummy gearbox is provided by VECTO.

![](pics/Structure_IEPC.png)

All signals with the suffix "_int" refer to the electric motor, while signals without this suffix refer to the whole component.