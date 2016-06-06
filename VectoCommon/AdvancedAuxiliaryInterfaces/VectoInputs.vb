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
Imports Newtonsoft.Json
Imports TUGraz.VectoCommon.Utils

Public Class VectoInputs
	Implements IVectoInputs

	''' <summary>
	''' Name of the Cycle ( Urban, Interurban etc )
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Public Property Cycle As String Implements IVectoInputs.Cycle

	''' <summary>
	''' Vehicle Mass (KG)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<JsonIgnore>
	Public Property VehicleWeightKG As Kilogram Implements IVectoInputs.VehicleWeightKG
		Get
			Return _vehicleWeight.SI(Of Kilogram)()
		End Get
		Set(value As Kilogram)
			_vehicleWeight = value.Value()
		End Set
	End Property

	<JsonProperty("VehicleWeightKG")> Dim _vehicleWeight As Double

	''' <summary>
	''' Powernet Voltage (V)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>This is the power voltage available in the bus - usually 26.3 Volts</remarks>
	<JsonIgnore>
	Public Property PowerNetVoltage As Volt Implements IVectoInputs.PowerNetVoltage
		Get
			Return _powerNetVoltage.SI(Of Volt)()
		End Get
		Set(value As Volt)
			_powerNetVoltage = value.Value()
		End Set
	End Property

	<JsonProperty("PowerNetVoltage")> Dim _powerNetVoltage As Double

	''' <summary>
	''' Fuel Map Same One as used in Vecto.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<JsonIgnore>
	Public Property FuelMap As IFuelConsumptionMap Implements IVectoInputs.FuelMap

	<JsonProperty("FuelMap")>
	Public Property FuelMapFile As String Implements IVectoInputs.FuelMapFile

	''' <summary>
	''' Fuel Density as used in Vecto.
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	<JsonIgnore>
	Public Property FuelDensity As KilogramPerCubicMeter Implements IVectoInputs.FuelDensity
		Get
			Return _fuelDensity.SI(Of KilogramPerCubicMeter)()
		End Get
		Set(value As KilogramPerCubicMeter)
			_fuelDensity = value.Value()
		End Set
	End Property

	<JsonProperty("FuelDensity")> Dim _fuelDensity As Double
End Class

