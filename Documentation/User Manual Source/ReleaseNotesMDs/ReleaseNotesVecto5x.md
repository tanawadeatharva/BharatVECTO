# VECTO 5\.x Release Notes

![](img%5CRelease%20Notes%20Vecto4x0.png)

<!-- Cover Slide -->


# VECTO v5.0.6-RC (22-09-2025)


## Features

- Update jobs in Generic Vehicles to version v2.7 (vecto/vecto!453)
- Disable v2.4 jobs (vecto/vecto!458)
- CodeEU #1140: Update multistep GUI to work with new XSDs (vecto/vecto!469)
- Multiple axles partial implementation (vecto/vecto!471)


## Bug Fixes

- Fix FCHV unit tests (vecto/vecto!448)
- XMLConversionTool bug fixes, more test cases, refactoring (vecto/vecto!452)
- Bug fixes for FCHV bus (vecto/vecto!454)
- CodeEU #1147: Made FuelCell Minpower, Maxpower optional (vecto/vecto!456)
- Engine-only simulation (vecto/vecto!457)
- EM data in PHEV rundata creation (vecto/vecto!459)
- CodeEU #1164: Lifetime ranges in reports for PEV, HEV-OVC. (vecto/vecto!461)
- Work-around in ranges to make tests succeed (vecto/vecto!462)
- CodeEU #1163: Respect job's battery SoC limits (vecto/vecto!463)
- CodeEU #870, #871, #924: Extend Accelerate condition after xEV Overload (vecto/vecto!466)
- VTP generic vehicles (vecto/vecto!465)
- Extend Accelerate condition after xEV Overload
- Changed v2.6 XSD to allow DeltaCdxA_declared and DeltaTransferredCdxA value: zero (vecto/vecto!472)
- Avoid cyclic refs from !473 (vecto/vecto!475)
- CodeEU #1167: Added WheelEnd info to MRF (vecto/vecto!476)
- Angledrive mod data, and PWheel axlegear efficiency (vecto/vecto!477)
- Disable engineering mode for multiple powertrains (vecto/vecto!478)


## Refactor

- FCHV iterative run strategy  (vecto/vecto!449)
- Update VECTO to NET 8 (vecto/vecto!467)
- Old .NET references (vecto/vecto!468)
- MultistepTool deprecated views (vecto/vecto!470)
- Remove unnecessary usings and nugets (vecto/vecto!473)


# VECTO v5.0.4-DEV (25-08-2025)


## Features

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


## Bug Fixes

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

## Refactor

- Merge refactoring branch (vecto/vecto!419)
- Merging refactorings from SW3 project to (vecto/vecto!396)



# VECTO v5.0.3 Official Release (08-07-2025)


## Bug Fixes

- Avoid em conditioning calculation for non-FCHV (vecto/vecto!416)
- Safe conditioning data lookup (vecto/vecto!417)


# VECTO v5.0.1 Official Release (03-07-2025)


## Bug Fixes

- FCHV engineering run (vecto/vecto!391)

- H2 check in bus job (vecto/vecto!392)

- Auxiliaries REESS connection (vecto/vecto!397)

- Read PowerOutputConsumptionMap as kW (vecto/vecto!401)

- Select pruned missions for FCHV primary bus (vecto/vecto!403)

- Use angledrive in lorries' gearshift data creation (vecto/vecto!404)

- Remove wrong bus angledrive restrictions (vecto/vecto!405)

- Decl GUI error message when FCHV in eng mode (vecto/vecto!407)

- Set NgTankSystem default for primary buses (vecto/vecto!408)

- FCHV F-IEPC simulation runs! (vecto/vecto!409)

- Operational range for group 10 vehicle weights (vecto/vecto!402)

- Ovc fc weighting to correspond to CO2 computation (vecto/vecto!413)

- 3s Buffer compute max EM PLoss for FL OPs (vecto/vecto!412)

- Conditioning power demand for FCHVs (vecto/vecto!414)




# VECTO v5.0.0-RC (05-06-2025)


## Features

- New (v2.7) XSD for vehicles (vecto/vecto!334)

- Partial implementation for new vehicle battery (vecto/vecto!337)

- Readers for v2.7 vehicle XSD, and support for fuel cell vehicles. (vecto/vecto!341)

- Read monitoring data from job (vecto/vecto!345)

- 3rd amendment mrf cif xml schemas (vecto/vecto!340)

- MRF v1.0 vehicle (lorries and FCHV primary buses) writers (vecto/vecto!354)

- CIF v1.0 vehicle part (v2.4 vehicles and v2.7 lorries) (vecto/vecto!355)

- Use monitoring data from job to write report (vecto/vecto!360)

- In motion charging postprocessing (vecto/vecto!344)

- Disable (for RC & official) v27 vehicles except H2-ICE & FCHV lorries (vecto/vecto!375)

- EM-IEPC Thermal Derating - Tq_max and Buffer Mods - Post VECTO-4.3.4 Feed-Back (vecto/vecto!378)

- Forbid AT upshift for reduced dt before brake (vecto/vecto!381)

- Readers for v2.7 buses, improved reader tests. (vecto/vecto!382)

- HEV - Get Best dSOC in vsum (vecto/vecto!383)


## Bug Fixes

- Non-https link in manual (vecto/vecto!339)

- V2.7 reader & XSD (vecto/vecto!342)

- Updated Monitoring Report XSD (FCHV, Multiple powertrains) (vecto/vecto!335)

- Updated XSLT file and hashing code for new vehicles and components. (vecto/vecto!338)

- Lock StoredResults list before accessing it to avoid race condition (vecto/vecto!343)

- V27 vehicle issues (vecto/vecto!346)

- 882 merge artifacts (vecto/vecto!347)

- FCHV angledrive input (vecto/vecto!348)

- Modify schema so that results can be written compatible with results for 2nd amendment: (vecto/vecto!349)

- Restore deleted code in monitoring report (vecto/vecto!350)

- Typo in MRF Inject module (vecto/vecto!351)

- Proper namespace for VIF IEPC sub-element (vecto/vecto!352)

- Mockup tests run successfully (vecto/vecto!353)

- Correcting errors in XML schema (and sample files): no engine output in... (vecto/vecto!356)

- Bugfixes/updates for the Monitoring report and testing via the MockupTests. (vecto/vecto!357)

- Replace U+2013 by regular dashes (vecto/vecto!359)

- Added missing IMC testdata (vecto/vecto!361)

- Retarder compulsory in all MRF vehicle components. (vecto/vecto!362)

- Add further condition to decide which results to write in case the input data is a Multistep bus (vecto/vecto!363)

- Check Articulated in json vehicle (vecto/vecto!365)

- Segment in Bus AirDrag data creation (vecto/vecto!364)

- FCHV pre-run execution (vecto/vecto!366)

- Write ZeroCO2EmissionsRange and HydrogenRange to H2-ICE reports. (vecto/vecto!367)

- Remove wrong angledrive restrictions (vecto/vecto!369)

- Set vectorundata in completed bus results, (vecto/vecto!368)

- Simulate OVC for FCHVs (vecto/vecto!370)

- FCHV H2 range in reports (vecto/vecto!371)

- Architecture in some MRF v1.0 tests (vecto/vecto!372)

- H2 properties check in exempted vehicle input (vecto/vecto!373)

- FCHV input classes inheritance (vecto/vecto!374)

- Standard values enum entry for v2.6 (vecto/vecto!376)

- Add Driving Actions for IEPC gearshift (vecto/vecto!377)

- ReEngage1C tolerance in AT (vecto/vecto!379)

- Take battery limit into account for EM overload - REESS Empty  (vecto/vecto!380)

- Of v2.7, allow only IMC, H2-ICE conventional, FCHV Lorries (vecto/vecto!384)

- Disable reading data from external csv (vecto/vecto!385)

- OVC results (vecto/vecto!387)

- Ignore FCHV pre-run in best deltaSoC calculation (vecto/vecto!388)


## Drop

- MaxChargingPower requirement for OVC in v2.7 (vecto/vecto!358)




