## VECTO v5.0.6-RC (22-09-2025)


### Features

- Update jobs in Generic Vehicles to version v2.7 (vecto/vecto!453)
- Disable v2.4 jobs (vecto/vecto!458)
- CodeEU #1140: Update multistep GUI to work with new XSDs (vecto/vecto!469)
- Multiple axles partial implementation (vecto/vecto!471)


### Bug Fixes

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


### Refactor

- FCHV iterative run strategy  (vecto/vecto!449)
- Update VECTO to NET 8 (vecto/vecto!467)
- Old .NET references (vecto/vecto!468)
- MultistepTool deprecated views (vecto/vecto!470)
- Remove unnecessary usings and nugets (vecto/vecto!473)