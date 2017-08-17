Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M2_Mock
	Implements IM2_AverageElectricalLoadDemand

	Public _GetAveragePowerAtCrankFromElectrics As Watt
	Public _GetAveragePowerDemandAtAlternator As Watt


	Public Function GetAveragePowerAtCrankFromElectrics() As Watt _
		Implements IM2_AverageElectricalLoadDemand.GetAveragePowerAtCrankFromElectrics
		Return _GetAveragePowerAtCrankFromElectrics
	End Function

	Public Function GetAveragePowerDemandAtAlternator() As Watt _
		Implements IM2_AverageElectricalLoadDemand.GetAveragePowerDemandAtAlternator
		Return _GetAveragePowerDemandAtAlternator
	End Function


	Public Sub New()
	End Sub


	Public Sub New(GetAveragePowerAtCrankFromElectrics As Double, GetAveragePowerDemandAtAlternator As Double)


		_GetAveragePowerAtCrankFromElectrics = GetAveragePowerAtCrankFromElectrics.SI(Of Watt)()
		_GetAveragePowerDemandAtAlternator = GetAveragePowerDemandAtAlternator.SI(Of Watt)()
	End Sub
End Class

