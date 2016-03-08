#Calculation Modes

VECTO supports different calculation modes for declaring a vehicle, validation of test-results, or experimenting with different parameters and components. These modes are described here.

- [**Declaration Mode**](#declaration-mode)
    : In this mode a vehicle can be declared. Many simulation parameters are predefined to provide a generic way of comparing the emissions.

- [**Engineering Mode**](#engineering-mode)
    : This mode is for experimenting and validation of a vehicle. There exist several options how the driving cycle may be defined (Target speed, Measured Speed, Pwheel).

- [**Engine Only Mode**](#engine-only-mode)
    : This mode is for validation of a measured engine component. Only the engine is simulated in this mode.


In the GUI the Calculation Mode can be changed via the Options Tab of the [Main Form](#main-form).

In the Command Line the Calculation Mode is Declaration by default, but can be changed to Engineering with the "-eng" flag.

<div class="vecto2">
A so called [Batch Mode](#batch-mode) exists in VECTO v2.2, which simulates every given job file with every given cycle file. This has nothing to do with the command line, it is just a convenience function to combine job files and cycle files.
</div>

<div class="vecto3">
VECTO V3.x doesn't support Batch mode anymore. The same functionality can be achieved by referencing every needed cycle file in the job files.
</div>



