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
Imports VectoAuxiliaries.Hvac

Namespace Electrics
	Public Class M0_NonSmart_AlternatorsSetEfficiency
		Implements IM0_NonSmart_AlternatorsSetEfficiency

		Private _electricalConsumersList As IElectricalConsumerList
		Private _alternatorEfficiencyMap As IAlternatorMap
		Private _powernetVoltage As Volt
		Private _signals As ISignals
		Private _steadyStateModelHVAC As ISSMTOOL


		Private _ElectricalPowerW As Watt
		Private _MechanicalPowerW As Watt
		Private _FuelingLPerH As LiterPerHour

		'Constructor
		Public Sub New(electricalConsumers As IElectricalConsumerList, alternatorEfficiencyMap As IAlternatorMap,
						powernetVoltage As Volt, signals As ISignals, ssmHvac As ISSMTOOL)

			If electricalConsumers Is Nothing Then Throw New ArgumentException("No ElectricalConsumersList Supplied")
			If alternatorEfficiencyMap Is Nothing Then Throw New ArgumentException("No Alternator Efficiency Map Supplied")
			If (powernetVoltage < ElectricConstants.PowenetVoltageMin Or powernetVoltage > ElectricConstants.PowenetVoltageMax) _
				Then Throw New ArgumentException("Powernet Voltage out of range")
			If signals Is Nothing Then Throw New ArgumentException("No Signals reference was supplied.")

			Me._electricalConsumersList = electricalConsumers
			Me._alternatorEfficiencyMap = alternatorEfficiencyMap
			Me._powernetVoltage = powernetVoltage
			Me._signals = signals
			Me._steadyStateModelHVAC = ssmHvac

			_ElectricalPowerW = ssmHvac.ElectricalWAdjusted.SI(Of Watt)()
			_MechanicalPowerW = ssmHvac.MechanicalWBaseAdjusted.SI(Of Watt)()
			_FuelingLPerH = ssmHvac.FuelPerHBaseAdjusted.SI(Of LiterPerHour)()
		End Sub

		'Public class outputs (Properties)
		Public ReadOnly Property AlternatorsEfficiency As Double _
			Implements IM0_NonSmart_AlternatorsSetEfficiency.AlternatorsEfficiency

			Get
				'Stored Energy Efficience removed from V8.0 21/4/15 by Mike Preston  //tb
				Dim baseCurrentDemandAmps As Ampere = _electricalConsumersList.GetTotalAverageDemandAmps(False) _
				' ElectricConstants.StoredEnergyEfficiency
				Dim totalDemandAmps As Ampere = baseCurrentDemandAmps + GetHVACElectricalPowerDemandAmps
				Return _alternatorEfficiencyMap.GetEfficiency(_signals.EngineSpeed, totalDemandAmps).Efficiency
			End Get
		End Property

		Public ReadOnly Property GetHVACElectricalPowerDemandAmps As Ampere _
			Implements IM0_NonSmart_AlternatorsSetEfficiency.GetHVACElectricalPowerDemandAmps
			Get
				Return _ElectricalPowerW / _powernetVoltage
			End Get
		End Property
	End Class
End Namespace


