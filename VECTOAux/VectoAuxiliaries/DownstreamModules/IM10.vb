' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.

Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules
	Public Interface IM10
		'AverageLoadsFuelConsumptionInterpolatedForPneumatics
		ReadOnly Property AverageLoadsFuelConsumptionInterpolatedForPneumatics As Single

		'Interpolated FC between points 2-3-1 Representing smart Pneumatics = Fuel consumption with smart Pneumatics and average electrical  power demand
		ReadOnly Property FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand As Single

		Sub CycleStep(Optional stepTimeInSeconds As Single = 0.0)

		'Added for diagnostic inspection purposes only, does not materially affect the class function.
		ReadOnly Property P1X As Single
		ReadOnly Property P1Y As Single
		ReadOnly Property P2X As Single
		ReadOnly Property P2Y As Single
		ReadOnly Property P3X As Single
		ReadOnly Property P3Y As Single
		ReadOnly Property XTAIN As Single
		ReadOnly Property INTRP1 As Single
		ReadOnly Property INTRP2 As Single
	End Interface
End Namespace

