## In-Motion Charging

Vehicles with in-motion charging are simulated in VECTO in a twofold approach. The effect of the additional air-drag when in-motion charging is active (e.g. pantograph, etc) is considered in the in-the-loop simulation. The effect of additional electric driving range and electric energy consumption from the grid is considered in a post-processing step.

<div class="engineering">
In engineering mode only the effect of the additional air-drag when in-motion charging is active can be considered. The following model parameter are necessary for IMC in engineering mode:

  - Share of in-motion charging infrastructure available on total mission distance
  - Delta CdxA with active in-motion charging feature
  - In-motion charging feature only available on motorway sections

**Note:** In case "In-motion charging feature only applicable on motorway sections" is selected, the value for "Share of in-motion charging infrastructure available on total mission distance" must be aligned with the distance shares declared as "highway" in the driving cycle.

If "In-motion charging feature only available on motorway sections" is selected, then the additional CdxA is only applied to when driving on highway sections. The additional CdxA is calculated from the "Share of in-motion charging infrastructure available on total mission distance" and "Delta CdxA with active in-motion charging feature".

In engineering mode, no post-processing for the electric range and electric energy consumption is done as the mission parameters and usage parameters are not known.
</div>


<div class="declaration">
In declaration mode the technology for in-motion charging can be selected. Depending on the technology, delta CdxA is looked up from the generic parameters. Also the share of in-motion charging infrastructure availability is generic per mission.

The generic values can be found in the *Declaration* folder and in the Excel file for [in-motion post-processing](OVC_IMC.xlsx).

The post-processing for the utility factor (OVC-HEV) and electric energy consumption and electric range (OVC-HEV, PEV) is described in a separate [Excel file](OVC_IMC.xlsx)
</div>