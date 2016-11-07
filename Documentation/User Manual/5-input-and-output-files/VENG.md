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
    "Date": "2016-10-03T15:25:00+01:00",
    "AppVersion": "3.1.0",
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
    "WHTC-Engineering": 1.03
  }
}
~~~

