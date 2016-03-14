## Job File

File for the definition of an job in vecto. A job contains everything what is needed to run a simulation. Can be created with the [Job Editor](#job-editor).

- File format is [JSON](#json).
- Filetype ending is ".vecto"

**Example:**

    {
      "Header": {
        "CreatedBy": " ()",
        "Date": "3/4/2015 2:09:13 PM",
        "AppVersion": "2.0.4-beta3",
        "FileVersion": 2
      },
      "Body": {
        "SavedInDeclMode": false,
        "VehicleFile": "24t Coach.vveh",
        "EngineFile": "24t Coach.veng",
        "GearboxFile": "24t Coach.vgbx",
        "Cycles": [
          "LOT2_rural Engine Only.vdri"
        ],
        "Aux": [
          {
            "ID": "ALT1",
            "Type": "Alternator",
            "Path": "24t_Coach_ALT.vaux",
            "Technology": ""
          },
          {
            "ID": "ALT2",
            "Type": "Alternator",
            "Path": "24t_Coach_ALT.vaux",
            "Technology": ""
          },
          {
            "ID": "ALT3",
            "Type": "Alternator",
            "Path": "24t_Coach_ALT.vaux",
            "Technology": ""
          }
        ],
        "VACC": "Coach.vacc",
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
