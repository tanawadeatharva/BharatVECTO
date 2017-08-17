' Copyright 2017 European Union.
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

Public Interface IVectoInputs
	''' <summary>
	''' Vehicle Mass (KG)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property VehicleWeightKG As Kilogram

	''' <summary>
	''' Cycle ( Urban, Interurban etc )
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property Cycle As String

	''' <summary>
	''' PowerNet Voltage (V) Volts available on the bus by Batteries
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property PowerNetVoltage As Volt

	''' <summary>
	''' Fuel Map Used in Vecto.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property FuelMap As IFuelConsumptionMap

	Property FuelMapFile As String

	''' <summary>
	''' Fuel density used in Vecto.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property FuelDensity As KilogramPerCubicMeter
End Interface
