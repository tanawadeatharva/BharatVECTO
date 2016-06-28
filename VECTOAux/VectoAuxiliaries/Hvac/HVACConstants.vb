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

Namespace Hvac
	Public Class HVACConstants
		Implements IHVACConstants

		<JsonProperty("FuelDensity")> ReadOnly _fuelDensity As Double
		<JsonProperty("DieselGCVJperGram")> ReadOnly _dieselGcvJperGram As Double = 44800

		Public Sub New()
			_fuelDensity = 835 '.SI(Of KilogramPerCubicMeter)()
		End Sub

		Public Sub New(fuelDensitySingle As KilogramPerCubicMeter)
			_fuelDensity = fuelDensitySingle.Value()
		End Sub


		<JsonIgnore>
		Public ReadOnly Property DieselGCVJperGram As JoulePerKilogramm Implements IHVACConstants.DieselGCVJperGram
			Get
				Return _dieselGcvJperGram.SI().Joule.Per.Gramm.Cast(Of JoulePerKilogramm)()
			End Get
		End Property

		<JsonIgnore()>
		Public ReadOnly Property FuelDensity As KilogramPerCubicMeter Implements IHVACConstants.FuelDensity
			Get
				Return _fuelDensity.SI(Of KilogramPerCubicMeter)()
			End Get
		End Property

		<JsonIgnore()>
		Public ReadOnly Property FuelDensityAsGramPerLiter As Double Implements IHVACConstants.FuelDensityAsGramPerLiter
			Get
				Return _fuelDensity * 1000
			End Get
		End Property
	End Class
End Namespace


