
## VECTO v4.3.2-RC (06-02-2025)

### Features

- New battery and supercap readers (vecto/vecto!309)

### Bug Fixes

- ATShiftStrategyOptimized - No UpshiftFomL if not locked (vecto/vecto!301)

- Forbid downshift to locked gear in APT-S if it generates direct upshift condition (vecto/vecto!307)

- Add condition to write BusAuxiliaries output data in vsum (vecto/vecto!306)

- Allow old XMLs for battery and supercap in development only (vecto/vecto!311)

- Correct binding for supercap input data class; use correct xml data type in xml component reader for supercap (vecto/vecto!310)