

Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules


Public Class M1_Mock
	Implements IM1_AverageHVACLoadDemand

	Public _AveragePowerDemandAtAlternatorFromHVACElectricsWatts As Watt
	Public _AveragePowerDemandAtCrankFromHVACElectricsWatts As Watt
	Public _AveragePowerDemandAtCrankFromHVACMechanicalsWatts As Watt
	'Public _HVACFuelingLitresPerHour As KilogramPerSecond

	Public ReadOnly Property AveragePowerDemandAtAlternatorFromHVACElectrics As Watt _
		Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtAlternatorFromHVACElectrics
    Get         
		Return _AveragePowerDemandAtAlternatorFromHVACElectricsWatts
    End Get

	End Property

	Public Function AveragePowerDemandAtCrankFromHVACElectrics() As Watt _
		Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACElectrics
		Return _AveragePowerDemandAtCrankFromHVACElectricsWatts
	End Function

	Public Function AveragePowerDemandAtCrankFromHVACMechanicals() As Watt _
		Implements IM1_AverageHVACLoadDemand.AveragePowerDemandAtCrankFromHVACMechanicals
		Return _AveragePowerDemandAtCrankFromHVACMechanicalsWatts
	End Function

	'Public Function HVACFueling() As KilogramPerSecond _
	'    Implements IM1_AverageHVACLoadDemand.HVACFueling
	'	Return _HVACFuelingLitresPerHour
	'End Function


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
       		'_HVACFuelingLitresPerHour = HVACFuelingLitresPerHour.SI(Of KilogramPerSecond)() _
		'(Of LiterPerHour)()
	End Sub

    Public Sub ResetCalculations() Implements IAbstractModule.ResetCalculations
        Throw New NotImplementedException
    End Sub
End Class

