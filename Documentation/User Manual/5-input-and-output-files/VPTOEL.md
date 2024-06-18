## PTO Cycle (.vptoel)

The PTO cycle defines the power demand during standing still and doing a pto operation using electric energy. This can only be used in [Engineering Mode](#engineering-mode) when a pto transmission is defined. It can be set in the [Vehicle-Editor](#vehicle-editor-pto-tab). The basic file format is [VECTO-CSV](#csv) and the file type ending is ".vptoel". A PTO cycle is time-based and may have variable time steps, but it is recommended to use a resolution between 1[Hz] and 2[Hz]. Regardless of starting time, VECTO shifts it to always begin at 0[s].

Header: **\<t>,  \<P_PTO_el>**

**Bold columns** are mandatory. Only the listed columns are allowed (no other columns!).<br />
The order is not important when the headers are annotated with \<angle-brackets\> (less-than-sign "<" and greater-than-sign ">").<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

|    Identifier    |  Unit |                                                                Description                                                                 |
|------------------|-------|--------------------------------------------------------------------------------------------------------------------------------------------|
| **t**            | [s]   | The time during the pto cycle. Must always be increasing. Gets shifted to begin with 0 by VECTO (if thats not already the case).                                                                                                                                                        |
| **P_PTO_el**   | [Nm]  | The torque at the PTO consumer (including prop-shaft losses if applicable) as measured by the DIN test converted to torque at engine speed |

**Example:**

~~~
<t> [s], <P_PTO_el> [Nm]
0      , 8.42
1      , 8.42
2      , 8.42
3      , 8.42
4      , 8.42
5      , 33.69
~~~