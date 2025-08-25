
## VECTO v5.0.4-DEV (25-08-2025)


### Features

- 3rd amendment reports for buses (vecto/vecto!421)
- V1.0 reports for multiple powertrain lorries (vecto/vecto!395)
- Add Diesel B100 CI fuel (vecto/vecto!399)
- MRF and Monitoring report for multiple-powertrain primary buses (vecto/vecto!410)
- battery only mode for P2 (vecto/vecto!425)
- Run simulation for H2-ICE bus (primary + completed) (vecto/vecto!432)
- FCHV bus simulation, primary & completed (vecto/vecto!433)
- Single-bus mode for FCHV (vecto/vecto!435)
- Enable all v2.7 vehicles (vecto/vecto!436)
- VTP input and formulas for buses and trucks (vecto/vecto!424)

### Bug Fixes

- Exception when getting MaxWindowsSize (vecto/vecto!422)
- For FCHV, APT-S/P gearboxes simulated as APT-N. (vecto/vecto!423)
- Ovc s-hev cs cd (vecto/vecto!420)
- Convert property Type to Architecture in axle powertrains (vecto/vecto!393)
- 3 job types for multiple powertrains (vecto/vecto!394)
- Removed NgTankSystem from Multiple_SHEV primary bus (vecto/vecto!398)
- Remove Retarder component from X4 architectures (vecto/vecto!400)
- Use multiple factory methods for Retarder and Angledrive data providers (vecto/vecto!406)
- Added FCHV missing gearbox bindings (vecto/vecto!427)
- Airdrag element in VIF report. (vecto/vecto!430)
- Do not require SoC limits for HV non-OVC (vecto/vecto!431)
- B100 density to 890 kg/m3 (vecto/vecto!411)
- Run old VIFs with v2.7 Completed vehicles (vecto/vecto!434)
- 1065 vehicle co2 group (vecto/vecto!428)
- DoCoast - add drive condition for overload (vecto/vecto!426)
- Generic retarder and failing tests (vecto/vecto!437)
- IEPC data adaptation (vecto/vecto!438)
- IHPC VECTO run data (vecto/vecto!439)
- FCHV IEPC rundata gearbox creation (vecto/vecto!440)
- Generic vehicles that failed to run (vecto/vecto!441)
- WHRCharger creation (vecto/vecto!442)
- Update wheelEnd sample (vecto/vecto!443)
- FCHV files in engineering mode (vecto/vecto!444)
- Initialize MaxChargingPower from static data if not available in input (vecto/vecto!445)
- FCHV battery and CD and CS runs (vecto/vecto!446)

### Refactor

- Merge refactoring branch (vecto/vecto!419)
- Merging refactorings from SW3 project to (vecto/vecto!396)
