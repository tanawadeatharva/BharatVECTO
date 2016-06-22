# SegmentTable

This document describes the structure of the file SegmentTable.csv. This
document resembles the Vehicle Classes table from the ACEA Whitebook and
defines standard vehicle classes, values and driving cycles.
(ACEA Whitebook April 2016, Part 1, Page 51).

If a value is not defined there is a "-".

| column                            | values                                         | description                                                                                        |
|-----------------------------------|------------------------------------------------|----------------------------------------------------------------------------------------------------|
| Valid                             | 0,1                                            | 1 means the entry is enabled, 0 means the entry is disabled and will not be used in vecto.         |
| Vehicle Category                  | RigidTruck,Tractor,CityBus,InterurbanBus,Coach | The basic vehicle category from whitebook.                                                         |
| Axle Conf.                        | 4x2,4x4,6x2,6x4,6x6,8x2,8x4,8x6,8x8            |                                                                                                    |
| GVW_Min                           |                                                |                                                                                                    |
| GVW_Max                           |                                                |                                                                                                    |
| HDV class                         |                                                |                                                                                                    |
| Body                              | B1,B2,B3,B4,B5,B6                              | Standard bodies (see standardbodies.csv)                                                           |
| Trailer                           | T1, T2                                         | Standard trailers (see standardbodies.csv)                                                         |
| Semitrailer                       | ST1, ST1-v2                                    | Standard semitrailer (see standardbodies.csv)                                                      |
| .vacc file                        | <filename>.vacc                                |                                                                                                    |
| Cross Wind Correction - Long haul | RigidSolo, RigidTrailer, TractorSemitrailer    |                                                                                                    |
| Cross Wind Correction - Other     | RigidSolo, TractorSemitrailer, CoachBus        |                                                                                                    |
| Truck Axles - Long haul           | percent(/percent)\* (e.g. 40/60)               | percent share for each axle on the truck. sum must be < 100. sum + trailer must be 100.            |
| Truck Axles - Other               | percent(/percent)\*                            | percent share for each axle on the truck. sum must be < 100. sum + trailer must be 100.            |
| Trailer Axles - Long haul         | percent/count                                  | percent share divides amongst the axles of the trailer. e.g. for 3 axles: 60/3 (means 20%/20%/20%) |
| Trailer Axles - Other             | percent/count                                  | percent share divides amongst the axles of the trailer. e.g. for 3 axles: 60/3 (means 20%/20%/20%) |
| Long haul                         | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Regional delivery                 | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Urban delivery                    | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Municipal utility                 | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Construction                      | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Heavy Urban                       | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Urban                             | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Suburban                          | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Interurban                        | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Coach                             | 0,1                                            | if 1 then this driving cycle is enabled for declaration, 0 means it is disabled.                   |
| Mass Extra - Long haul            |                                                |                                                                                                    |
| Mass Extra - Regional delivery    |                                                |                                                                                                    |
| Mass Extra - Urban delivery       |                                                |                                                                                                    |
| Mass Extra - Municipal utility    |                                                |                                                                                                    |
| Mass Extra - Construction         |                                                |                                                                                                    |
| Mass Extra - Heavy Urban          |                                                |                                                                                                    |
| Mass Extra - Urban                |                                                |                                                                                                    |
| Mass Extra - Suburban             |                                                |                                                                                                    |
| Mass Extra - Interurban           |                                                |                                                                                                    |
| Mass Extra - Coach                |                                                |                                                                                                    |
| Payload - Long haul               |                                                |                                                                                                    |
| Payload - Regional delivery       |                                                |                                                                                                    |
| Payload - Urban delivery          |                                                |                                                                                                    |
| Payload - Municipal utility       |                                                |                                                                                                    |
| Payload - Construction            |                                                |                                                                                                    |
| Payload - Heavy Urban             |                                                |                                                                                                    |
| Payload - Urban                   |                                                |                                                                                                    |
| Payload - Suburban                |                                                |                                                                                                    |
| Payload - Interurban              |                                                |                                                                                                    |
| Payload - Coach                   |                                                |                                                                                                    |
