##Vehicle: ADAS Technologies

<div class="declaration">
Advanced Driver Assistant Systems are considered in Declaratio Mode via a technology dependent and vehicle group specific bonus on the fuel cosumption as described in the followin.
</div>		

In Declaration mode VECTO applies a correction factor to take into account certain Advanced Driver Assistant System technologies. The following Technologies are currently considered:

- Engine stop start
- EcoRoll without engine stop
- EcoRoll with engine stop
- Predictive cruise control

For predictive cruise control three different options are considered:
 
- Option 1: crest coasting: Approaching a crest the vehicle velocity is reduced before the point where the vehicle starts accelerating by gravity alone compared to the set speed of the cruise control so that the braking during the following downhill phase can be reduced.
- Option 2: acceleration without engine power: During downhill driving with a low vehicle velocity and a high negative slope the vehicle acceleration is performed without any engine power usage so that the downhill braking can be reduced.
- Option 3: dip coasting: During downhill driving when the vehicle is braking at the overspeed velocity, PCC increases the overspeed for a short period of time to end the downhill event with a higher vehicle velocity. Overspeed is a higher vehicle speed than the set speed of the cruise control system.

A PCC system can be declared as input to the simulation tool if either the functionalities set out in points 1) and 2) or points 1), 2) and 3) are covered.

Out of this four technologies as listed above only 11 combinations are valid. For every valid ADAS technology combination VECTO reduces the final fuel consumtion by a certain percentage depending on the vehicle group, driving cycle, and payload.

The following table maps the valid combinations of ADAS systems to a so-called "ADAS Combination".

| Engine Stop Start   | EcoRoll without Engine Stop   | EcoRoll with Engine Stop   | Predictive Cruise Control   | ADAS Combination  |
| ------------------- | ----------------------------- | -------------------------- | --------------------------- | ----------------- |
| false               | false                         | false                      | none                        | 0                 |
| true                | false                         | false                      | none                        | 1                 |
| false               | true                          | false                      | none                        | 2                 |
| false               | false                         | true                       | none                        | 3                 |
| false               | false                         | false                      | Option 1 & 2                | 4/1               |
| false               | false                         | false                      | Option 1 & 2 & 3            | 4/2               |
| true                | true                          | false                      | none                        | 5                 |
| true                | false                         | true                       | none                        | 6                 |
| true                | false                         | false                      | Option 1 & 2                | 7/1               |
| true                | false                         | false                      | Option 1 & 2 & 3            | 7/2               |
| false               | true                          | false                      | Option 1 & 2                | 8/1               |
| false               | true                          | false                      | Option 1 & 2 & 3            | 8/2               |
| false               | false                         | true                       | Option 1 & 2                | 9/1               |
| false               | false                         | true                       | Option 1 & 2 & 3            | 9/2               |
| true                | true                          | false                      | Option 1 & 2                | 10/1              |
| true                | true                          | false                      | Option 1 & 2 & 3            | 10/2              |
| true                | false                         | true                       | Option 1 & 2                | 11/1              |
| true                | false                         | true                       | Option 1 & 2 & 3            | 11/2              |


For the vehicle groups 4, 5, 9, and 10 the following reduction in fuel consumption is applied for the different cycle and payload combinations and ADAS Combination. The first value is applied for low-loading and the second value is applied for reference load.

### Vehicle Group 4

| ADAS Combination   | LongHaul      | LongHaul EMS   | Regional Delivery   | Regional Delivery EMS   | Urban Delivery   |
| ------------------ | ------------- | -------------- | ------------------- | ----------------------- | ---------------- |
| 1                  | -0.1% / 0.0%  |                | -0.5% / -0.5%       |                         | -1.5% / -1.2%    |
| 2                  | 0.0% / 0.0%   |                | -0.1% / -0.1%       |                         | 0.0% / 0.0%      |
| 3                  | 0.0% / -0.1%  |                | -0.1% / -0.2%       |                         | 0.0% / 0.0%      |
| 4/1                | -0.1% / -0.4% |                | 0.0% / -0.1%        |                         | 0.0% / 0.0%      |
| 4/2                | -0.1% / -0.5% |                | -0.1% / -0.2%       |                         | 0.0% / 0.0%      |
| 5                  | -0.1% / -0.1% |                | -0.6% / -0.6%       |                         | -1.5% / -1.2%    |
| 6                  | -0.1% / -0.1% |                | -0.6% / -0.7%       |                         | -1.5% / -1.2%    |
| 7/1                | -0.1% / -0.4% |                | -0.5% / -0.6%       |                         | -1.5% / -1.2%    |
| 7/2                | -0.1% / -0.6% |                | -0.6% / -0.7%       |                         | -1.5% / -1.2%    |
| 8/1                | -0.1% / -0.4% |                | -0.1% / -0.2%       |                         | 0.0% / 0.0%      |
| 8/2                | -0.1% / -0.5% |                | -0.1% / -0.3%       |                         | 0.0% / 0.0%      |
| 9/1                | -0.1% / -0.4% |                | -0.2% / -0.3%       |                         | 0.0% / 0.0%      |
| 9/2                | -0.1% / -0.5% |                | -0.2% / -0.4%       |                         | 0.0% / 0.0%      |
| 10/1               | -0.2% / -0.4% |                | -0.6% / -0.7%       |                         | -1.5% / -1.2%    |
| 10/2               | -0.2% / -0.6% |                | -0.6% / -0.7%       |                         | -1.5% / -1.2%    |
| 11/1               | -0.2% / -0.4% |                | -0.7% / -0.8%       |                         | -1.5% / -1.2%    |
| 11/2               | -0.2% / -0.6% |                | -0.7% / -0.8%       |                         | -1.5% / -1.2%    |

### Vehicle Group 5

| ADAS Combination   | LongHaul      | LongHaul EMS   | Regional Delivery   | Regional Delivery EMS   | Urban Delivery   |
| ------------------ | ------------- | -------------- | ------------------- | ----------------------- | ---------------- |
| 1                  | -0.1% / 0.0%  | 0.0% / 0.0%    | -0.4% / -0.3%       | -0.3% / -0.2%           | -1.8% / -1.3%    |
| 2                  | 0.0% / -0.1%  | 0.0% / -0.1%   | -0.1% / -0.1%       | -0.2% / 0.0%            | 0.0% / 0.0%      |
| 3                  | -0.1% / -0.2% | 0.0% / -0.1%   | -0.2% / -0.2%       | -0.3% / -0.1%           | 0.0% / 0.0%      |
| 4/1                | -0.2% / -0.5% | -0.2% / -0.1%  | -0.2% / -0.6%       | -0.3% / -0.5%           | 0.0% / 0.0%      |
| 4/2                | -0.2% / -0.7% | -0.3% / -0.3%  | -0.4% / -0.9%       | -0.5% / -0.8%           | 0.0% / 0.0%      |
| 5                  | -0.1% / -0.1% | 0.0% / -0.1%   | -0.5% / -0.4%       | -0.5% / -0.3%           | -1.8% / -1.3%    |
| 6                  | -0.1% / -0.2% | -0.1% / -0.2%  | -0.6% / -0.5%       | -0.6% / -0.4%           | -1.8% / -1.3%    |
| 7/1                | -0.2% / -0.5% | -0.3% / -0.1%  | -0.6% / -0.9%       | -0.7% / -0.8%           | -1.8% / -1.3%    |
| 7/2                | -0.3% / -0.7% | -0.3% / -0.4%  | -0.8% / -1.3%       | -0.8% / -1.1%           | -1.8% / -1.3%    |
| 8/1                | -0.2% / -0.6% | -0.2% / -0.1%  | -0.3% / -0.7%       | -0.5% / -0.5%           | 0.0% / 0.0%      |
| 8/2                | -0.2% / -0.7% | -0.3% / -0.4%  | -0.4% / -1.0%       | -0.7% / -0.8%           | 0.0% / 0.0%      |
| 9/1                | -0.2% / -0.6% | -0.3% / -0.2%  | -0.4% / -0.8%       | -0.6% / -0.6%           | 0.0% / 0.0%      |
| 9/2                | -0.2% / -0.8% | -0.3% / -0.4%  | -0.5% / -1.1%       | -0.7% / -0.9%           | 0.0% / 0.0%      |
| 10/1               | -0.2% / -0.6% | -0.3% / -0.2%  | -0.7% / -1.0%       | -0.8% / -0.8%           | -1.8% / -1.3%    |
| 10/2               | -0.3% / -0.8% | -0.3% / -0.4%  | -0.8% / -1.3%       | -1.0% / -1.1%           | -1.8% / -1.3%    |
| 11/1               | -0.3% / -0.7% | -0.3% / -0.2%  | -0.8% / -1.1%       | -0.9% / -0.9%           | -1.8% / -1.3%    |
| 11/2               | -0.3% / -0.8% | -0.4% / -0.5%  | -0.9% / -1.4%       | -1.0% / -1.2%           | -1.8% / -1.3%    |

### Vehicle Group 9

| ADAS Combination   | LongHaul      | LongHaul EMS   | Regional Delivery   | Regional Delivery EMS   | Urban Delivery   |
| ------------------ | ------------- | -------------- | ------------------- | ----------------------- | ---------------- |
| 1                  | -0.1% / 0.0%  | 0.0% / 0.0%    | -0.5% / -0.4%       | -0.3% / -0.2%           |                  |
| 2                  | 0.0% / -0.1%  | 0.0% / 0.0%    | -0.1% / -0.1%       | 0.0% / 0.0%             |                  |
| 3                  | 0.0% / -0.2%  | 0.0% / -0.1%   | -0.2% / -0.2%       | -0.2% / -0.2%           |                  |
| 4/1                | -0.1% / -0.4% | -0.2% / -0.1%  | -0.1% / -0.3%       | -0.3% / -0.6%           |                  |
| 4/2                | -0.1% / -0.6% | -0.3% / -0.3%  | -0.2% / -0.5%       | -0.5% / -0.9%           |                  |
| 5                  | -0.1% / -0.1% | -0.1% / -0.1%  | -0.6% / -0.5%       | -0.3% / -0.3%           |                  |
| 6                  | -0.1% / -0.2% | -0.1% / -0.2%  | -0.7% / -0.6%       | -0.5% / -0.4%           |                  |
| 7/1                | -0.2% / -0.5% | -0.3% / -0.1%  | -0.6% / -0.7%       | -0.6% / -0.8%           |                  |
| 7/2                | -0.2% / -0.6% | -0.3% / -0.4%  | -0.7% / -0.9%       | -0.8% / -1.1%           |                  |
| 8/1                | -0.1% / -0.5% | -0.2% / -0.1%  | -0.2% / -0.4%       | -0.3% / -0.6%           |                  |
| 8/2                | -0.1% / -0.7% | -0.3% / -0.4%  | -0.2% / -0.5%       | -0.5% / -0.9%           |                  |
| 9/1                | -0.1% / -0.6% | -0.3% / -0.2%  | -0.3% / -0.5%       | -0.4% / -0.7%           |                  |
| 9/2                | -0.2% / -0.7% | -0.3% / -0.4%  | -0.3% / -0.6%       | -0.6% / -1.0%           |                  |
| 10/1               | -0.2% / -0.5% | -0.3% / -0.2%  | -0.6% / -0.8%       | -0.6% / -0.8%           |                  |
| 10/2               | -0.2% / -0.7% | -0.3% / -0.4%  | -0.7% / -0.9%       | -0.8% / -1.1%           |                  |
| 11/1               | -0.2% / -0.6% | -0.3% / -0.2%  | -0.7% / -0.9%       | -0.7% / -0.9%           |                  |
| 11/2               | -0.2% / -0.8% | -0.3% / -0.4%  | -0.8% / -1.0%       | -0.9% / -1.2%           |                  |

### Vehicle Group 10

| ADAS Combination   | LongHaul      | LongHaul EMS   | Regional Delivery   | Regional Delivery EMS   | Urban Delivery   |
| ------------------ | ------------- | -------------- | ------------------- | ----------------------- | ---------------- |
| 1                  | -0.1% / 0.0%  | 0.0% / 0.0%    | -0.4% / -0.3%       | -0.3% / -0.2%           |                  |
| 2                  | 0.0% / -0.1%  | 0.0% / 0.0%    | -0.1% / -0.1%       | 0.0% / 0.0%             |                  |
| 3                  | -0.1% / -0.2% | 0.0% / -0.1%   | -0.2% / -0.2%       | -0.2% / -0.1%           |                  |
| 4/1                | -0.2% / -0.5% | -0.2% / -0.1%  | -0.3% / -0.6%       | -0.4% / -0.6%           |                  |
| 4/2                | -0.2% / -0.7% | -0.3% / -0.4%  | -0.4% / -0.9%       | -0.6% / -0.9%           |                  |
| 5                  | -0.1% / -0.1% | 0.0% / -0.1%   | -0.5% / -0.4%       | -0.4% / -0.2%           |                  |
| 6                  | -0.1% / -0.2% | -0.1% / -0.2%  | -0.6% / -0.5%       | -0.5% / -0.3%           |                  |
| 7/1                | -0.2% / -0.5% | -0.3% / -0.1%  | -0.6% / -1.0%       | -0.7% / -0.8%           |                  |
| 7/2                | -0.3% / -0.7% | -0.4% / -0.4%  | -0.8% / -1.3%       | -0.9% / -1.1%           |                  |
| 8/1                | -0.2% / -0.5% | -0.2% / -0.1%  | -0.3% / -0.7%       | -0.4% / -0.6%           |                  |
| 8/2                | -0.2% / -0.7% | -0.3% / -0.4%  | -0.4% / -1.0%       | -0.6% / -0.9%           |                  |
| 9/1                | -0.2% / -0.6% | -0.3% / -0.2%  | -0.4% / -0.8%       | -0.5% / -0.7%           |                  |
| 9/2                | -0.3% / -0.8% | -0.3% / -0.4%  | -0.5% / -1.1%       | -0.7% / -1.0%           |                  |
| 10/1               | -0.3% / -0.6% | -0.3% / -0.2%  | -0.7% / -1.0%       | -0.7% / -0.8%           |                  |
| 10/2               | -0.3% / -0.8% | -0.4% / -0.4%  | -0.8% / -1.3%       | -0.9% / -1.1%           |                  |
| 11/1               | -0.3% / -0.6% | -0.3% / -0.2%  | -0.8% / -1.1%       | -0.8% / -0.9%           |                  |
| 11/2               | -0.3% / -0.8% | -0.4% / -0.5%  | -0.9% / -1.4%       | -1.0% / -1.2%           |                  |