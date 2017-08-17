## Job File

File for the definition of an job in vecto. A job contains everything what is needed to run a simulation. Can be created with the [Job Editor](#job-editor).

- File format is [JSON](#json).
- Filetype ending is ".vecto"

Refers to other files:

* [Vehicle (VVEH)](#vehicle-file-.vveh)
* [Engine (VENG)](#engine-file-.veng)
* [Gearbox (VGBX)](#gearbox-file-.vgbx)
* [Driving Cycle (VDRI)](#driving-cycles-.vdri)
* [Auxiliary Input File (VAUX)](#auxiliary-input-file-.vaux)
* [Acceleration Limiting (VACC)](#acceleration-limiting-input-file-.vacc)


**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "Michael Krisper (Graz University of Technology)",
    "Date": "2016-03-18T14:37:05+01:00",
    "AppVersion": "3.0.2",
    "FileVersion": 2
  },
  "Body": {
    "SavedInDeclMode": false,
    "VehicleFile": "Vehicle.vveh",
    "EngineFile": "Engine.veng",
    "GearboxFile": "Gearbox.vgbx",
    "Cycles": [
      "DrivingCycle_Rural.vdri",
      "DrivingCycle_Urban.vdri"
    ],
    "Aux": [
      {
        "ID": "ALT",
        "Type": "Alternator",
        "Path": "Alternator.vaux",
        "Technology": ""
      },
      {
        "ID": "PN",
        "Type": "PneumaticSystem",
        "Path": "Pneumatic System.vaux",
        "Technology": ""
      },
      {
        "ID": "HVAC",
        "Type": "HVAC",
        "Path": "AirCondition.vaux",
        "Technology": ""
      }
    ],
    "VACC": "Driver.vacc",
    "EngineOnlyMode": true,
    "StartStop": {
      "Enabled": false,
      "MaxSpeed": 5.0,
      "MinTime": 0.0,
      "Delay": 0
    },
    "LAC": {
      "Enabled": true,
      "Dec": -0.5,
      "MinSpeed": 50.0
    },
    "OverSpeedEcoRoll": {
      "Mode": "OverSpeed",
      "MinSpeed": 70.0,
      "OverSpeed": 5.0,
      "UnderSpeed": 5.0
    }
  }
}
~~~

