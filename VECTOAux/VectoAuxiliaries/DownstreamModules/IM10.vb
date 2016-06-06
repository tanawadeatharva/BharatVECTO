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

Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Namespace DownstreamModules
	Public Interface IM10
		'AverageLoadsFuelConsumptionInterpolatedForPneumatics
		ReadOnly Property AverageLoadsFuelConsumptionInterpolatedForPneumatics As Kilogram

		'Interpolated FC between points 2-3-1 Representing smart Pneumatics = Fuel consumption with smart Pneumatics and average electrical  power demand
		ReadOnly Property FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand As Kilogram

		Sub CycleStep(stepTimeInSeconds As Second)

		'Added for diagnostic inspection purposes only, does not materially affect the class function.
		ReadOnly Property P1X As NormLiter
		ReadOnly Property P1Y As Kilogram
		ReadOnly Property P2X As NormLiter
		ReadOnly Property P2Y As Kilogram
		ReadOnly Property P3X As NormLiter
		ReadOnly Property P3Y As Kilogram
		ReadOnly Property XTAIN As NormLiter
		ReadOnly Property INTRP1 As Kilogram
		ReadOnly Property INTRP2 As Kilogram
	End Interface
End Namespace

