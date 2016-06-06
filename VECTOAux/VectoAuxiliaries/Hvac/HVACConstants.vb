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

Namespace Hvac
	Public Class HVACConstants
		Implements IHVACConstants

		Private _fuelDensity As KilogramPerCubicMeter

		Public Sub New()
			_fuelDensity = 835.SI(Of KilogramPerCubicMeter)()
		End Sub

		Public Sub New(fuelDensitySingle As KilogramPerCubicMeter)
			_fuelDensity = fuelDensitySingle
		End Sub

		Public ReadOnly Property DieselGCVJperGram As JoulePerKilogramm Implements IHVACConstants.DieselGCVJperGram
			Get
				Return 44800.SI().Joule.Per.Gramm.Cast(Of JoulePerKilogramm)()
			End Get
		End Property

		Public ReadOnly Property FuelDensity As KilogramPerCubicMeter Implements IHVACConstants.FuelDensity
			Get
				Return _fuelDensity
			End Get
		End Property

		Public ReadOnly Property FuelDensityAsGramPerLiter As Double Implements IHVACConstants.FuelDensityAsGramPerLiter
			Get
				Return _fuelDensity.Value() * 1000
			End Get
		End Property
	End Class
End Namespace


