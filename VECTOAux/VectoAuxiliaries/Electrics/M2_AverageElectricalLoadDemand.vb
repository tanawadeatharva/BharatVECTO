' Copyright 2015 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
Imports TUGraz.VectoCommon.Utils

Namespace Electrics
	Public Class M2_AverageElectricalLoadDemand
		Implements IM2_AverageElectricalLoadDemand

		Public _powerNetVoltage As Volt = 26.3.SI(Of Volt)()
		Private _electricalConsumers As IElectricalConsumerList
		Private _module0 As IM0_NonSmart_AlternatorsSetEfficiency
		Private _alternatorPulleyEffiency As Double
		Private _signals As Signals


		'Constructor
		Public Sub New(ByVal electricalConsumers As IElectricalConsumerList, m0 As IM0_NonSmart_AlternatorsSetEfficiency,
						altPulleyEfficiency As Double, powerNetVoltage As Volt, signals As ISignals)

			If electricalConsumers Is Nothing Then Throw New ArgumentException("Electrical Consumer List must be supplied")
			If m0 Is Nothing Then Throw New ArgumentException("Must supply module 0")
			If altPulleyEfficiency = 0 OrElse altPulleyEfficiency > 1 Then _
				Throw New ArgumentException("Alternator Gear efficiency out of range.")
			If powerNetVoltage < ElectricConstants.PowenetVoltageMin OrElse powerNetVoltage > ElectricConstants.PowenetVoltageMax _
				Then
				Throw New ArgumentException("Powernet Voltage out of known range.")
			End If


			_powerNetVoltage = powerNetVoltage
			_electricalConsumers = electricalConsumers
			_module0 = m0
			_alternatorPulleyEffiency = altPulleyEfficiency
		End Sub

		'Public class outputs (Properties)
		Public Function GetAveragePowerDemandAtAlternator() As Watt _
			Implements IM2_AverageElectricalLoadDemand.GetAveragePowerDemandAtAlternator

			'Stored Energy Efficience removed from V8.0 21/4/15 by Mike Preston  //tb
			'Return ( _electricalConsumers.GetTotalAverageDemandAmps(False)/ElectricConstants.StoredEnergyEfficiency) * _powerNetVoltage
			Return _powerNetVoltage * _electricalConsumers.GetTotalAverageDemandAmps(False)
		End Function

		Public Function GetAveragePowerAtCrankFromElectrics() As Watt _
			Implements IM2_AverageElectricalLoadDemand.GetAveragePowerAtCrankFromElectrics

			Dim ElectricalPowerDemandsWatts As Watt = GetAveragePowerDemandAtAlternator()
			Dim alternatorsEfficiency As Double = _module0.AlternatorsEfficiency
			Dim ElectricalPowerDemandsWattsDividedByAlternatorEfficiency As Watt = ElectricalPowerDemandsWatts * (1 /
																												alternatorsEfficiency)

			Dim averagePowerDemandAtCrankFromElectricsWatts As Watt

			averagePowerDemandAtCrankFromElectricsWatts = ElectricalPowerDemandsWattsDividedByAlternatorEfficiency * (1 /
																													_alternatorPulleyEffiency)

			Return averagePowerDemandAtCrankFromElectricsWatts
		End Function
	End Class
End Namespace