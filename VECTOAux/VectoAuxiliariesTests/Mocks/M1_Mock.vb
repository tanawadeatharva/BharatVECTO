
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules


Public Class M1_Mock
	Implements IM1_AverageHVACLoadDemand

	Public _AveragePowerDemandAtAlternatorFromHVACElectricsWatts As Watt
	Public _AveragePowerDemandAtCrankFromHVACElectricsWatts As Watt
	Public _AveragePowerDemandAtCrankFromHVACMechanicalsWatts As Watt
	Public _HVACFuelingLitresPerHour As LiterPerSecond

	Public Function AveragePowerDemandAtAlternatorFromHVACElectricsWatts() As Watt _
		Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectricsWatts
		Return _AveragePowerDemandAtAlternatorFromHVACElectricsWatts
	End Function

	Public Function AveragePowerDemandAtCrankFromHVACElectricsWatts() As Watt _
		Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectricsWatts
		Return _AveragePowerDemandAtCrankFromHVACElectricsWatts
	End Function

	Public Function AveragePowerDemandAtCrankFromHVACMechanicalsWatts() As Watt _
		Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
		Return _AveragePowerDemandAtCrankFromHVACMechanicalsWatts
	End Function

	Public Function HVACFuelingLitresPerHour() As LiterPerSecond _
		Implements IM1_AverageHVACLoadDemand.HVACFuelingLitresPerHour
		Return _HVACFuelingLitresPerHour
	End Function


	Public Sub New()
	End Sub

	Public Sub New(AveragePowerDemandAtAlternatorFromHVACElectricsWatts As Double,
					AveragePowerDemandAtCrankFromHVACElectricsWatts As Double,
					AveragePowerDemandAtCrankFromHVACMechanicalsWatts As Double,
					HVACFuelingLitresPerHour As Double)

		'Assign Values
		_AveragePowerDemandAtAlternatorFromHVACElectricsWatts =
			AveragePowerDemandAtAlternatorFromHVACElectricsWatts.SI(Of Watt)()
		_AveragePowerDemandAtCrankFromHVACElectricsWatts = AveragePowerDemandAtCrankFromHVACElectricsWatts.SI(Of Watt)()
		_AveragePowerDemandAtCrankFromHVACMechanicalsWatts = AveragePowerDemandAtCrankFromHVACMechanicalsWatts.SI(Of Watt)()
        _HVACFuelingLitresPerHour = HVACFuelingLitresPerHour.SI(Unit.SI.Liter.Per.Hour).Cast(Of LiterPerSecond)() _
        '(Of LiterPerHour)()
    End Sub
End Class

