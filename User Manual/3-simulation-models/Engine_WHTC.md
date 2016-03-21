##Engine: WHTC Correction

<div class="declaration">
In declaration mode the fuel consumption is corrected as follows:

To prevent inconsistencies of regulated emissions and fuel consumption between the WHTC (hot part) test and the steady state fuel map as well as considering effects of transient engine behaviour a "WHTC correction factor" is used.

Based on the target engine operation points of the particular engine in WHTC the fuel consumption is interpolated from the steady state fuel map (“backward calculation”) in each of the three parts of the WHTC separately. The measured specific fuel consumption per WHTC part in [g/kWh] is then divided by the interpolated specific fuel consumption to obtain the "WHTC correction factors" CF~urb~ (Urban), CF~rur~ (Rural), CF~mot~ (Motorway). For the interpolation the same method as for interpolation in VECTO is applied (Delauney triangulation).

All calculations regarding the brake specific fuel consumption from the interpolation as well as from the measurement and the three correction factors CF~urb~, CF~rur~, CF~mot~ are fully implemented in the VECTO-Engine evaluation tool.

The total correction factor CF~total~ depends on the mission profile and is produced in VECTO by mission profile specific weighting factors listed in the table below.

$CF_{total} = CF_{urb} \cdot WF_{urb} + CF_{rur} \cdot WF_{rur} + CF_{mot} \cdot WF_{mot}$

with the correction factor CF~urb~, CF~rur~, CF~mot~ coming from the [Engine](#engine-file), and weighting factors WF~urb~, WF~rur~, WF~mot~ predefined in the declaration data:

|  Mission profile   | WF~urb~ | WF~rur~ | WF~mot~ |
|--------------------|---------|---------|---------|
| Long haul          | 11%     | 0%      | 89%     |
| Regional delivery  | 17%     | 30%     | 53%     |
| Urban delivery     | 69%     | 27%     | 4%      |
| Municipial utility | 98%     | 0%      | 2%      |
| Construction       | 62%     | 32%     | 6%      |
| Citybus            | 100%    | 0%      | 0%      |
| Interurban bus     | 45%     | 36%     | 19%     |
| Coach              | 0%      | 22%     | 78%     |

The whtc fuel consumption is then calculated with: $FC_{whtc} = FC \cdot CF_{total}$
</div>

<div class="engineering">
In engineering mode no WHTC correction is applied by Vecto. For an arbitrary cycle the weighting factors are not known, hence the total correction factor CF~total~ can not be computed.
WHTC correction can be applied manually as a post-processing step.
</div>