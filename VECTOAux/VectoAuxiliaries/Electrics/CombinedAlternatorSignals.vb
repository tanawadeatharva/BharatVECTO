
Imports TUGraz.VectoCommon.Utils

Namespace Electrics
	'Used by the CombinedAlternator class and any other related classes.
	Public Class CombinedAlternatorSignals
		Implements ICombinedAlternatorSignals

		Public Property CrankRPM As Double Implements ICombinedAlternatorSignals.CrankRPM

		Public Property CurrentDemandAmps As Ampere Implements ICombinedAlternatorSignals.CurrentDemandAmps

		'Number of alternators in the Combined Alternator
		Public Property NumberOfAlternators As Integer Implements ICombinedAlternatorSignals.NumberOfAlternators
	End Class
End Namespace


