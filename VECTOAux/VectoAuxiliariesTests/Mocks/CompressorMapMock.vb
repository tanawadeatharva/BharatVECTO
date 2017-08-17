Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries
Imports VectoAuxiliaries.Pneumatics

Namespace Mocks
	Public Class CompressorMapMock
		Implements ICompressorMap

		Dim failing As Boolean

		Public Sub New(ByVal isFailing As Boolean)
			failing = isFailing
		End Sub

		Public Function Initialise() As Boolean Implements ICompressorMap.Initialise
			If failing Then
				Throw New System.ArgumentException
			Else
				Return True
			End If
		End Function

		Public Function GetFlowRate(ByVal rpm As Double) As NormLiterPerSecond Implements ICompressorMap.GetFlowRate
			Return 2.0.SI(Of NormLiterPerSecond)()
		End Function

		Public Function GetPowerCompressorOn(ByVal rpm As Double) As Watt Implements ICompressorMap.GetPowerCompressorOn
			Return 8.0.SI(Of Watt)()
		End Function

		Public Function GetPowerCompressorOff(ByVal rpm As Double) As Watt Implements ICompressorMap.GetPowerCompressorOff
			Return 5.0.SI(Of Watt)()
		End Function


		Public Function GetAveragePowerDemandPerCompressorUnitFlowRate() As Double _
			Implements ICompressorMap.GetAveragePowerDemandPerCompressorUnitFlowRate

			Return 0.01
		End Function


		Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) _
			Implements IAuxiliaryEvent.AuxiliaryEvent
	End Class
End Namespace