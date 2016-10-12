##PTO

Simulation of PTO related fuel consumption in VECTO.

![](pics/pto.png)

The PTO related influence on the fuel consumption shall be considered in VECTO as follows:

* **Losses in the PTO "Transmission" part (blue)**: Considered by a constant power consumption as a function of the PTO type. When set, the transmission losses are always present. Configurable in the [Vehicle Editor](#vehicle-editor).
* **Idling losses of the PTO "Consumer" (red)**: Torque loss as a function of speed as determined by the DIN 30752-1 procedure.  Always present except during the PTO cycle. Configurable in the [Vehicle Editor](#vehicle-editor) and following the file format described in [PTO-Consumer](#pto-consumer).
* **Power consumption of the PTO "Consumer" (red) during the PTO cycle**: A specific PTO cycle (time-based, engine speed and torque from PTO consumer as determined by the DIN 30752-1 procedure) is simulated during vehicle stops which are additionally labelled as "with PTO activation". The cycle is configurable in the [Vehicle Editor](#vehicle-editor) and follows the file format described in [PTO-Cycle](#pto-cycle).


