Imports VectoAuxiliaries.Electrics
Imports System.ComponentModel
Imports TUGraz.VectoCommon.Utils

Namespace Mocks
	Public Class ElectricalConsumerMock
		Implements IElectricalConsumer

		Public Property AvgConsumptionAmps As Double Implements IElectricalConsumer.AvgConsumptionAmps

		Public Property BaseVehicle As Boolean Implements IElectricalConsumer.BaseVehicle

		Public Property Category As String Implements IElectricalConsumer.Category

		Public Property ConsumerName As String Implements IElectricalConsumer.ConsumerName

		Public Property NominalConsumptionAmps As Double Implements IElectricalConsumer.NominalConsumptionAmps

		Public Property NumberInActualVehicle As Integer Implements IElectricalConsumer.NumberInActualVehicle

		Public Property PhaseIdle_TractionOn As Double Implements IElectricalConsumer.PhaseIdle_TractionOn

		Public Property PowerNetVoltage As Double Implements IElectricalConsumer.PowerNetVoltage

		Public Function TotalAvgConumptionAmps(Optional PhaseIdle_TractionOnBasedOnCycle As Double = Nothing) As Ampere _
			Implements IElectricalConsumer.TotalAvgConumptionAmps
			Return 9.SI(Of Ampere)()
		End Function


		Public Function TotalAvgConsumptionInWatts(Optional PhaseIdle_TractionOnBasedOnCycle As Double = 0.0) As Watt _
			Implements IElectricalConsumer.TotalAvgConsumptionInWatts

			Return (9 * 26.3).SI(Of Watt)()
		End Function


		Public Property Info As String Implements IElectricalConsumer.Info

		Public Event PropertyChanged As PropertyChangedEventHandler _
			Implements INotifyPropertyChanged.PropertyChanged

		Private Sub NotifyPropertyChanged(p As String)
			RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(p))
		End Sub
	End Class
End Namespace
