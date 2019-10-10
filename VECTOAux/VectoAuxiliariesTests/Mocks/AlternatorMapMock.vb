
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics

Namespace Mocks

	Public Class AlternatorMapMock
		Implements IAlternatorMap



		Dim failing As Boolean

		Public Sub New(ByVal isFailing As Boolean)
			failing = isFailing
		End Sub

		Public Function Initialise() As Boolean Implements IAlternatorMap.Initialise
			If failing Then
				Throw New ArgumentException
			Else
				Return True
			End If
		End Function

		Public Function GetEfficiency(rpm1 As Double, amps1 As Ampere) As AlternatorMapValues Implements IAlternatorMap.GetEfficiency
			Return New AlternatorMapValues()
		End Function


		Public Event IAuxiliaryEvent_AuxiliaryEvent As AuxiliaryEventEventHandler Implements IAuxiliaryEvent.AuxiliaryEvent
	End Class

End Namespace
