///------------------- CIF -------------------
[ ] Component Order








[ ] unit as attribute,  define type of unit in OutputDefinitions, add attribute to element 


 GetValueAsUnit






///--------------------- MRF -------------------


[ ] ??? BoostingLimitations not implemented ? 

[ ] TODO: Electricmachine xsd update

[ ] TODO: IEPC MaxContinuousPower ? attribute voltage (attribute) MaxContinuousPower =  COntinousTorque * TestSpeedContinuous Torqe (auch bei electric machine) per voltage level
    [ ] Not implemented yet ? not up to date


[ ] REESS Battery Total Usable Capacity in simulation (aus run daten) (eigene Methode) Helper Methode

[ ] Completed Bus Primary Manufacturer as step 1 manufacturer ?  yes!

[ ] Off vehicle charging max power 


[X] VehicleTypeApprovalNumber completed bus mandatory ? leave optional


[ ] BusAux Electric System Max AlternatorPower, select alternator where RatedCurrent * RatedVoltage max. (aus run daten, helper methode !!) (now alternator where RatedCurrent * RatedVoltage is max.) 

[ ] unit as attribute,  define type of unit in OutputDefinitions, add attribute to element 



[x] TODO: Update REESS specifications in XSD schema according to output. (SuperCap!)
[x] BusAux Electric System ElectricStorage, (currently: sum of electric storages) convert to wh oder kWh einheit einheit als attribut in xml 

///--------------------------- DONE -----------------------

[x] Look into axlewheels ()

[X] Reess battery and supercap update in xml schema



[X] BusAux SmartCompression system not in inputdata for HEV (update xsd scheme)


[V] TODO: ?? REESS specifications per battery system - sortiert nach string id aufsteigend
[x] TODO: HEV_IEPC_S_LorryManufacturerReport
[X] REESS Battery Nominal voltage //Calculated based on table ANNEXES_LS Page 290
[X] Split electric system in Conventional/HEV and PEV, (PEV has no alternator)

[x] Vehicle Type Approval number (MRF(x)/CIF)






ComponentOrder

Engine
Transmission
Retarder
TorqueConverter
Angledrive
ElectricMachines
IEPC
REESS
AirDrag
AxleGear
AxleWheels
Auxiliaries