## VTP-Job File

File for the definition of a verification test job in vecto. A job contains everything what is needed to run a simulation. Can be created with the [Verifcation Test Job Editor](#vtp-job-editor).

- File format is [JSON](#json).
- Filetype ending is ".vecto"

Refers to other files:

* [Vehicle (XML)](#vehicle-file-.vveh)
* [Gearbox (VGBX)](#gearbox-file-.vgbx)
* [Driving Cycle (VDRI)](#driving-cycles-.vdri)



**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "VECTO 3.2",
    "Date": "2017-11-14T13:16:31.7337506Z",
    "AppVersion": "3",
    "FileVersion": 4
  },
  "Body": {
    "SavedInDeclMode": false,
    "DeclarationVehicle": "SampleVehicle.xml",
    "FanPowerCoefficients": [
      0.00000055,
      14.62,
      108.5
    ],
    "FanDiameter": 0.225,
    "Cycles": [
      "VTP-cycle.vdri"
    ]
  }
}
~~~

