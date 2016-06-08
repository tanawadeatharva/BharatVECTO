Imports TUGraz.VectoCommon.Utils

Namespace Hvac
	Public Interface IHVACConstants
		''' <summary>
		''' Diesel: 44800   [J/g]
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property DieselGCVJperGram As JoulePerKilogramm

		''' <summary>
		''' 835  [g/l]
		''' </summary>
		''' <value></value>
		''' <returns></returns>
		''' <remarks></remarks>
		ReadOnly Property FuelDensity As KilogramPerCubicMeter

		ReadOnly Property FuelDensityAsGramPerLiter As Double
	End Interface
End Namespace


