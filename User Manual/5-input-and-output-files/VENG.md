## Engine File

File for the definition of an engine in Vecto. Can be created with the [Engine Editor](#engine-editor).

- File format is [JSON](#json).
- Filetype ending is ".veng"

Refers to other files:

* [Full Load And Drag Curve (VFLD)](#full-load-and-drag-curves-.vfld)
* [Fuel Consumption (VMAP)](#fuel-consumption-map-.vmap)


**Example:**

~~~json
{
  "Header": {
    "CreatedBy": "Michael Krisper (Graz University of Technology",
    "Date": "2016-03-18T14:48:38+01:00",
    "AppVersion": "3.0.2",
    "FileVersion": 3
  },
  "Body": {
    "SavedInDeclMode": false,
    "ModelName": "Engine",
    "Displacement": 7700.0,
    "IdlingSpeed": 600.0,
    "Inertia": 3.789,
    "FullLoadCurve": "EngineFullLoadCurve.vfld",
    "FuelMap": "FuelConsumptionMap.vmap",
    "WHTC-Urban": 0.97,
    "WHTC-Rural": 0.99,
    "WHTC-Motorway": 1.05
  }
}
~~~

