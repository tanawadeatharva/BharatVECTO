
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules
	Public Interface IM14
		ReadOnly Property TotalCycleFCGrams As Gram

		ReadOnly Property TotalCycleFCLitres As Liter
	End Interface
End Namespace
