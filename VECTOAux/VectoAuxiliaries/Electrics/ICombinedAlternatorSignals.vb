
Imports TUGraz.VectoCommon.Utils

Namespace Electrics
	'Used by CombinedAlternator
	Public Interface ICombinedAlternatorSignals
		Property NumberOfAlternators As Integer
		Property CrankRPM As Double
		Property CurrentDemandAmps As Ampere
	End Interface
End Namespace


