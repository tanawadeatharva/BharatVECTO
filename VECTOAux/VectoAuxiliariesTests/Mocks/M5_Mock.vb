Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules

Public Class M5_Mock
	Implements IM5_SmartAlternatorSetGeneration

	Public Property _AlternatorsGenerationPowerAtCrankIdleWatts As Watt
	Public Property _AlternatorsGenerationPowerAtCrankOverrunWatts As Watt
	Public Property _AlternatorsGenerationPowerAtCrankTractionOnWatts As Watt


	Public Function AlternatorsGenerationPowerAtCrankIdleWatts() As Watt _
		Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankIdleWatts
		Return _AlternatorsGenerationPowerAtCrankIdleWatts
	End Function

	Public Function AlternatorsGenerationPowerAtCrankOverrunWatts() As Watt _
		Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankOverrunWatts
		Return _AlternatorsGenerationPowerAtCrankOverrunWatts
	End Function

	Public Function AlternatorsGenerationPowerAtCrankTractionOnWatts() As Watt _
		Implements IM5_SmartAlternatorSetGeneration.AlternatorsGenerationPowerAtCrankTractionOnWatts
		Return _AlternatorsGenerationPowerAtCrankTractionOnWatts
	End Function


	Public Sub New()
	End Sub


	Public Sub New(AlternatorsGenerationPowerAtCrankIdleWatts As Double,
					AlternatorsGenerationPowerAtCrankOverrunWatts As Double,
					AlternatorsGenerationPowerAtCrankTractionOnWatts As Double)

		_AlternatorsGenerationPowerAtCrankIdleWatts = AlternatorsGenerationPowerAtCrankIdleWatts.SI(Of Watt)()
		_AlternatorsGenerationPowerAtCrankOverrunWatts = AlternatorsGenerationPowerAtCrankOverrunWatts.SI(Of Watt)()
		_AlternatorsGenerationPowerAtCrankTractionOnWatts = AlternatorsGenerationPowerAtCrankTractionOnWatts.SI(Of Watt)()
	End Sub
End Class

