Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M3_Mock
	Implements IM3_AveragePneumaticLoadDemand

	Public _GetAveragePowerDemandAtCrankFromPneumatics As Watt
	Public _TotalAirConsumedPerCycle As NormLiterPerSecond


	Public Function GetAveragePowerDemandAtCrankFromPneumatics() As Watt _
		Implements IM3_AveragePneumaticLoadDemand.GetAveragePowerDemandAtCrankFromPneumatics
		Return _GetAveragePowerDemandAtCrankFromPneumatics
	End Function

	Public Function TotalAirConsumedPerCycle() As NormLiterPerSecond _
		Implements IM3_AveragePneumaticLoadDemand.AverageAirConsumedPerSecondLitre
		Return _TotalAirConsumedPerCycle
	End Function


	Public Sub New()
	End Sub

	Public Sub New(GetAveragePowerDemandAtCrankFromPneumatics As Double, TotalAirConsumedPerCycle As Double)

		_GetAveragePowerDemandAtCrankFromPneumatics = GetAveragePowerDemandAtCrankFromPneumatics.SI(Of Watt)()
		_TotalAirConsumedPerCycle = TotalAirConsumedPerCycle.SI(Of NormLiterPerSecond)()
	End Sub
End Class

