##PTO Cycle (.vptoc)

The PTO cycle defines the power demands during standing still and doing a pto operation. This can only be used in [Engineering Mode](#engineering-mode) when a pto transmission is defined. It can be set in the [Vehicle-Editor](#vehicle-editor). The basic file format is [Vecto-CSV](#csv) and the file type ending is ".vptoc". A PTO cycle is time-based and may have variable time steps.

Header: **\<t>, \<Engine speed>, \<PTO Torque>**

**Bold columns** are mandatory. Only the listed columns are allowed (no other columns!).<br />
The order is not important when the headers are annotated with \<angle-brackets\> (less-than-sign "<" and greater-than-sign ">").<br />
Units are optional and are enclosed in [square-brackets] after the header-column. Comments may be written with a preceding hash-sign "#".

|    Identifier    |  Unit |                        Description                        |
|------------------|-------|-----------------------------------------------------------|
| **t**            | [s]   | The time during the pto cycle. Must always be increasing. |
| **Engine speed** | [rpm] | The engine speed.                                         |
| **PTO Torque**   | [Nm]  | The torque demand.                                        |

**Example:**

~~~
<t> [s], <Engine speed> [rpm], <PTO Torque> [Nm]
0      , 600                 , 0
1      , 600                 , 0
2      , 900                 , 0
3      , 1200                , 50
4      , 1200                , 70
5      , 1200                , 100
~~~

