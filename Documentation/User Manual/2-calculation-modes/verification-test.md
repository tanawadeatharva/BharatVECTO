##Verification Test Mode

The purpose of the verification test is to simulate a vehicle defined in declaration mode on a measured real-driving cycle. This simulation mode uses its own [cyle format](#verification-test-cycle), requiring mainly vehicle speed, wheel speed, wheel torque, engine-fan speed, and engine speed. VECTO then calculates the appropriate gear and simulates the cycle. Auxiliary power is according to the technologies defined in the vehicle. However, the engine fan auxiliary is ignored and the power demand for the engine fan is calcuated based on the engine-fan speed. The power demand for the other auxiliaries depends on the vehicle's actual speed. The fuel consumption is calculated using the engine speed from the driving cycle and the torque demand as given in the cycle, adding the losses of all powertrain components.

###Requirements

-   One or more checked job files in the Job List
-   Each job must include a vehicle in declaration mode (XML)
-   Each job file must include at least one driving cycle

###Results

-   Modal results (.vmod). One file for each vehicle/cycle combination. Modal results are only written if the modal output is enabled in the 'Options' tab on the [Main Window](#main-form)
-   Sum results (.vsum). One file for each invocation of VECTO.
