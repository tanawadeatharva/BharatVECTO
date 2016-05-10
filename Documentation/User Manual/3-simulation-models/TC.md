##Torque Converter Model

**!!! The Torque Converter Model is still in development and at the moment only available in Vecto 2.2 !!!**

<div class="vecto2">
The torque converter is defined as (virtual) separate gear. While TC active: Iterative calculation of engine torque and speed based on TC characteristic. Creeping: Engine speed set to idling. Brakes engaged to absorb surplus torque.

 ![](pics/GBX-TC.svg)


###Torque converter characteristics file (.vtcc)

The file is described [here](#torque-converter-characteristics-.vtcc).

This file defines the torque converter characteristics as described in VDI 2153:

-   **Speed Ratio** (ν) = Output Speed / Input Speed
-   **Torque Ratio** (μ) = Output Torque / Input Torque
-   **Input Torque** (T~ref(ν)~) is the input torque (over ν) for a specific reference engine speed (see below).

The Input Torque at  reference engine speed is needed to calculate the actual engine torque using this formula:

$T_{in} = T_{ref}(v) \cdot ( \frac{n_{in}}{n_{ref}} )^{2}$

with:

-   T~in~ = engine torque \[Nm\]
-   T~ref(ν)~ = reference torque at reference rpm (form .vtcc file) \[Nm\]
-   n~in~ = engine speed \[1/min\]
-   n~ref~ = reference rpm \[1/min\] (see below)

The torque converter characteristics must also be defined for speed ratios of more than one (ν&gt;1) in order to calculate overrun conditions (torque&lt;0).



###Setup for Conventional AT gearboxes 
Torque converter file is defined for **torque converter only**

![](pics/GBX-TC-Setup-1-1.svg)

-   Define TC gear with ratio of first (mechanical) gear
-   Set transmission losses of first gear (map or constant efficiency)

![](pics/GBX-TC-Setup-1-2.svg)


###Setup for Power-distributed AT gearboxes
Torque converter file is defined for the **whole gearbox**

![](pics/GBX-TC-Setup-2-1.svg)

-   Define TC gear with ratio = 1
-   Set transmission efficiency to 1 (= 100%) because losses are covered  by the .vtcc file.

![](pics/GBX-TC-Setup-2-2.svg)

</div>