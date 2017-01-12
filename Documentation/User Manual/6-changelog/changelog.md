#Changelog

**VECTO 3.1.1**

***Build 742 (2017-01-12)***

- Improvements:
    + [VECTO-390, VECTO-400] Adapt engine speed to estimated engine speed after gear shift during traction interruption (double clutching)
    + [VECTO-396, VECTO-388] Add shift losses for AT power shifts
    + [VECTO-389] new gear shift rules for AT gearboxes
    + [VECTO-387] added max input speed for torque converter
    + [VECTO-385] Automatically add generic torque converter data for drag
    + [VECTO-399] Add missions and loadings for vehicle categories 11, 12, and 16 (declaration mode)
    + [VECTO-384] cleanup memory after simulation run
    + [VECTO-394] new option for vectocmd to disable all output
    + [VECTO-392] make the GUI scale depending on the Windows font size
    + [VECTO-391] Gearbox output speed and output torque added to .vmod files
    + [VECTO-386] Gearbox window: disable input fields not applicable for the selected gearbox type
- Bugfixes:
    + [VECTO-401] Computation of n_95h etc. fails if engine’s max torque is constant 0 
Lookup of Airdrag parameters in declaration mode
    + [VECTO-378] Improved file-handling in AAUX module


**VECTO 3.1.0**

***Build 683 (2016-11-14)***

- Bugfixes:
    + [VECTO-375] Fixed bug when braking during slope change from negative to positive values.
    + [VECTO-372] Added check for unusual acceleration/deceleration data which could lead to error when halting.
    + [VECTO-371] Added additional behavior to overcome such situations
    + [VECTO-370] Added additional behavior to overcome such situations
    + [VECTO-369] CrosswindCorrection is now saved and read again from JSON files
    + [VECTO-373] WHTC-Engineering correction factor now correctly read/write in JSON files
    + [VECTO-368] Fixed validation for specific cases when values are intentionally invalid.
    + [VECTO-357] Updated GUI to not show ECO-Roll option to avoid confusion
    + Fixed numerous bugs in AT-ShiftStrategy regarding the Torque Converter
    + Fixed numerous bugs in MeasuredSpeed Mode (and MeasuredSpeed with Gear) in connection with AT-Gearbox and TorqueConverter
    + Fixed a bug when PTO-Cycle was missing
    + Corrected axle loss maps for Generic Vehicles in Declaration Mode to match technical annex
    + Corrected SumFile Cruise Time Share. Added checks that timeshares must add up to 100%

- Improvements:
    + [VECTO-355] Updated documentation, added powertrain schematics in chapter "Simulation Models"
    + [VECTO-374] Check range for Torque Converter speed ratio input data to be at least between 0 and 2.2
    + Updated many error messages to be more explicit about the reason of error
    + Added "Mission Profiles" Directory with driving cycles publicly available in the application root directory.
    + Added "Declaration" directory with the declaration data files in the application root directory.
	+ Added warning when engine inertia is 0
    + Added check that engine speed must not fall below idle speed (even in measured speed mode)
    + Shift curve validation for AT gearboxes: shift curves may now overlap due to different shift logic in AutomaticTransmissions.
    + Updated Crosswind Coefficients for Tractor+Semitrailer


***Build 662 (2016-10-24)***

- Bugfixes:
    + [VECTO-360] Fixed error during startup of VECTO (loading of DLLs).
    + [VECTO-358] Fixed errors during simulation where vehicle unintentionally was driving backwards. Added stricter sanity checks and invariants to avoid such errors. Fixed 1Hz-Filter for ModFiles (distance was wrong under certain circumstances, vehicle seemingly jumping back before halt).
    + [VECTO-361] Fixed classification of vehicles with GVM of exactly 7500kg (Class 1).
    + [VECTO-364] Fixed an error in measured speed mode (run aborts).
    + [VECTO-363] Compute shift polygons in declaration mode now uses correct boundary for full load margin.
    + [VECTO-365] Fixed editing gears in declaration mode

- Improvements:
    + [VECTO-355] User Manual updated (Screenshots, Descriptions, File Formats, Vecto V2 Comments removed).
    + [VECTO-317] Declaration data for Wheel sizes updated
    + [VECTO-359] Simplified code regarding PT1 behavior.
    + [VECTO-323] PTO-Cycle may now be left empty when not used in driving cycle.

***Build 652 (2016-10-14)***

- Main Updates
    + Removed VECTO Core 2.2
    + Refactoring of the User-Interface Backend: loading, saving files and validating user input uses Vecto 3 models
    + AT-Gearbox Model: differentiate between AT gearbox with serial torque converter and AT gearbox using powersplit
    + Numbering of gears with AT gearbox corresponds to mechanical gears, new column TC_locked in .vmod file to indicate if torque converter is active
    + Torque converter gear no longer allowed in input (added by Vecto depending on the AT model)
    + New implementation of torque converter model (analytic solutions)
    + Added PTO option for municipal utility vehicles: PTO idle losses, separate PTO cycle during standstill
    + Added Angledrive Component
    + Option for constant Auxiliary Power Demand in Job-File
    + Normalize x/y values before triangulating Delaunay map (transmission loss-maps, fuel consumption loss map)
    + Additional fuel consumption correction factor in declaration mode: cold/hot balancing factor
    + Added fuel consumption correction factor (WHTC, Cold/Hot balancing, …) in engineering mode
    + Update auxiliaries power demand according to latest whitebook
    + Allow multiple steered axles
    + Adapted engine idle controller (during declutch) – engine speed decreases faster
    + SUM-File: split E_axl_gbx into two columns, E_axl and E_gbx
    + New columns in mod-file: PTO, torque converter
    + Removed full-load curve per gear, only single value MaxTorque
    + Removed rims (dynamic wheel radius depends on wheel type)
    + Fixes in AAUX module: open correct file-browser, save selected files

-------------------------------------------------------------------------------

**VECTO 3.0.4**

***Build 565 (2016-07-19)***

- Bugfixes
    + AAUX HVAC Dialog does not store path to ActuationsMap and SSMSource
    + GUI: check for axle loads in declaration mode renders editing dialog useless
    + Vecto 2.2: Simulation aborts (Vecto terminates) when simulating EngineOnly cycles
    + Vecto 3: Building SimulationRun EngineOnly simulation failed

***Build 544 (2016-06-28)***

- Main Updates
    + New gear shift strategy according to White Book 2016
    + New coasting strategy according to White Book 2016
    + New input parameters (enineering mode) for coasting and gear shift behavior
    + Use SI units in Advanced Auxiliaries Module and compile with strict compiler settings (no implicit casts, etc.)
    + Allow efficiency for transmission losses (in engineering mode)

- Bugfixes
    + Auxiliary TechList not read from JSON input data
    + Improvements in driver strategy
    + Bugfixes in MeasuredSpeed mode

-------------------------------------------------------------------------------

**VECTO 3.0.3**

***Build 537 (2016-06-21)***

- Main Updates
    + Support for Advanced Auxiliaries (Ricardo) in Vecto 3.0.3 and Vecto 2.2
    + Performance improvements
    + Gearshift polygons according to WB 2016
    + Revision of SUM-data file, changed order of columns, changed column headers
- Bugfixes
    + Delaunay Maps: additional check for duplicate input points
    + Creation of PDF Report when running multiple jobs at once
    + Sanity checks for gear shift lines
    + Improvements DriverStrategy: handling special cases

***Build 495 (2016-05-10)***

- Main Updates
    + Support for Advanced Auxiliaries (Ricardo) in Vecto 3.0.3 and Vecto 2.2
    + Performance improvements
    + Gearshift polygons according to WB 2016
    + Revision of SUM-data file, changed order of columns, changed column headers
- Bugfixes
    + Delaunay Maps: additional check for duplicate input points
    + Creation of PDF Report when running multiple jobs at once
    + Sanity checks for gear shift lines
    + Improvements DriverStrategy: handling special cases

-------------------------------------------------------------------------------

**VECTO 3.0.2**

***Build 466 (2016-04-11)***

- Bugfix: calculation of CO2 consumption based on FC-Final (instead of FC-map)
- Bugfix: acceleration in .vmod was 0 in certain cases (error in output)
- Bugfix: syncronized access to cycle cache (declaration)

***Build 448 (2016-03-24)***

- Bugfix: set WHTC factors to a valid default value in engineering mode
- Bugfix: first page of declaration report was missing
- fixed inconsistencies in user manual
- Bugfix: better error message roll resistance calculation could not be calculated
- Bugfix: measured speed now calculates distance correctly
- Bugfix: measured speed fills missing moddata columns (acc, dist, grad)
- Bugfix: better error message when driving cycle is missing.
- Bugfix: vectocmd errormsg when writing progress

***Build 434 (2016-03-10)***

- New simulation modes:
    + Measured Speed
    + Measured Speed with Gear
    + Pwheel (SiCo)
- Adaptations of powertrain components architecture
    + Move wheels inertia from vehicle to wheels
    + Auxiliaries no longer connected via clutch to the engine but via a separate port
    + Engine checks overload of gearbox and engine overload
- Fixed some driving behavior related issues in VectoCore:
    + When the vehicle comes to a halt during gear shift, instead of aborting the cycle, it tries to drive away again with an appropriate gear.
- [ModData Format](#modal-results-.vmod) changed for better information and clarity
- Entries in the sum-file are sorted in the same way as in Vecto 2.2
- In engineering mode the execution mode (distance-based, time-based measured speed, time-based measured speed with gear, engine only) are detected based on the cycle
- Added validation of input values
- Gravity constant set to 9.80665 (NIST standard acceleration for gravity)
- Improved input data handling: sort input values of full-load curves (engine, gbx, retarder)
- Better Integration of VectoCore into GUI (Notifications and Messages)
- Speed dependent cross-wind correction (vcdv) and v_air/beta cross-wind correction (vcdb) impemented
- For all calculations the averaged values of the current simulation step are used for interpolations in loss-maps.
- Allow extrapolation of loss maps in engineering mode (warnings)
- Refactoring of input data handling: separate InputDataProvider interfaces for model data
- Refactoring of result handling: separate result container and output writer
- New Long-Haul driving cycle included
- User Manual updated for VECTO V3.x
- Fix: sparse representation of declaration cycles had some missing entries
- Bugfix: error in computation of engine's preferred speed
- Bugfix: wrong vehicle class lookup
- Bugfix: duplicate entries in intersected full-load curves
- Bugfix: retarder takes the retarder ratio into account for lossmap lookup
- Bugfix: use unique identifier for jobs in job list
- Bugfix: error in triagulation of fuel consumption map






