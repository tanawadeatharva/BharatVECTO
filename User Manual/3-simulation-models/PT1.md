##Transient Full Load

<div class="vecto2">
VECTO uses a PT1 function to model transient torque build up using this formula:


$P_{fld\ dyn_{i}} = \frac{1}{T(n_{i})+1} \cdot [P_{fld\ stat}(n_{i})+T(n_{i}) \cdot P_{act_{i-1}}]$

with:

* n~i~ ... current engine speed 
* T(n~i~) ... PT1 time constant at engine speed n~i~ (col. 4 in [.vfld file](#full-load-and-drag-curves-.vfld))
* P~fld\ stat~(n~i~) ... Static full load at engine speed n~i~ (col. 2 in [.vfld file](#full-load-and-drag-curves-.vfld))
* P~act\ i-1~ ... Engine power in previous time step
</div>

<div class="vecto3">
Vecto 3 uses basically the same PT1 behavior to model transient torque build up. However, due to the dynamic time steps the formula is implemented as follows:

$P_{fld\ dyn_{i}} = P_{fld\ stat}(n_i) \cdot (1 - exp(-\frac{t_i^*}{PT1}))$

where $t^*$ is computed from the dynamic full-load power in the previous simulation interval:

$t_{i-1}^* = PT1 \cdot ln(\frac{1.0}{1 - \frac{P_{eng_{i - 1}}}{P_{fld\ stat}(n_i)}})$

$t_i^* = t_{i-1}^* + dt$

</div>