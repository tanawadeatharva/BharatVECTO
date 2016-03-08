##Shift Polygons Input File (.vgbs)

Defines up- and down-shift curves. See [Gear Shift Model](#gear-shift-model) for details. The file uses the [VECTO CSV format](#csv).

- Filetype: .vgbs
- Header: **Engine Torque [Nm], Downshift rpm [1/min], Upshift rpm [1/min]**
- Requires at least 2 data entries

![](pics/Shift.svg)

**Example:**

    Engine Torque [Nm], Downshift rpm [1/min], Upshift rpm [1/min]
    -400,560,1289
    759,560,1289
    1252,742,1289
    2372,1155,1942

