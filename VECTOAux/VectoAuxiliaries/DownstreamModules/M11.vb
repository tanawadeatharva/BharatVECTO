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
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac

Namespace DownstreamModules
	Public Class M11
		Implements IM11

		Private Const RPM_to_RadiansPerSecond As Single = 9.55

#Region "Private Aggregates"
		'Private Aggregations
		Private AG1 As Joule
		Private AG2 As Joule
		Private AG3 As Joule
		Private AG4 As Gram
		Private AG5 As Gram
		Private AG6 As Joule
		Private AG7 As Gram

#End Region

#Region "Private Fields Assigned by Constructor."

		Private M1 As IM1_AverageHVACLoadDemand
		Private M3 As IM3_AveragePneumaticLoadDemand
		Private M6 As IM6
		Private M8 As IM8
		Private fmap As IFuelConsumptionMap
		Private signals As ISignals

#End Region


		'Staging Calculations
		Private Function Sum0(ByVal rpm As Double) As PerSecond

			If rpm < 1 Then rpm = 1

			Return rpm.RPMtoRad()	' / RPM_to_RadiansPerSecond
		End Function

		Private ReadOnly Property Sum1 As Watt
			Get
				Return If(M6.OverrunFlag, M8.SmartElectricalAlternatorPowerGenAtCrank, SIBase(Of Watt).Create(0.0))
				'Return M6.OverrunFlag*M8.SmartElectricalAlternatorPowerGenAtCrank
			End Get
		End Property

		Private ReadOnly Property Sum2 As Watt
			Get
				Return M3.GetAveragePowerDemandAtCrankFromPneumatics + M1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
			End Get
		End Property

		Private ReadOnly Property Sum3 As NewtonMeter
			Get
				Return M8.SmartElectricalAlternatorPowerGenAtCrank / Sum0(signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property Sum4 As NewtonMeter
			Get
				Return Sum2 / Sum0(signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property Sum5 As NewtonMeter
			Get
				Return Sum4 + Sum9
			End Get
		End Property

		Private ReadOnly Property Sum6 As NewtonMeter
			Get
				Return Sum3 + Sum5
			End Get
		End Property

		Private ReadOnly Property Sum7 As GramPerSecond
			Get

				'SCM 3_02
				Dim intrp1 As GramPerSecond = fmap.GetFuelConsumption(Sum6, signals.EngineSpeed)
				intrp1 = If(Not Double.IsNaN(intrp1.Value()) AndAlso intrp1 > 0, intrp1, 0.SI(Of GramPerSecond))
				Return intrp1
			End Get
		End Property

		Private ReadOnly Property Sum8 As GramPerSecond
			Get

				'SCHM 3_2
				Dim intrp2 As GramPerSecond = fmap.GetFuelConsumption(Sum5, signals.EngineSpeed)
				intrp2 = If(Not Double.IsNaN(intrp2.Value()) AndAlso intrp2 > 0, intrp2, 0.SI(Of GramPerSecond))
				Return intrp2
			End Get
		End Property

		Private ReadOnly Property Sum9 As NewtonMeter
			Get

				Return _
					signals.EngineDrivelineTorque.SI(Of NewtonMeter)() +
					((signals.PreExistingAuxPower * 1000).SI(Of Watt)() / Sum0(signals.EngineSpeed))
			End Get
		End Property

		Private ReadOnly Property Sum10 As NewtonMeter
			Get

				Return M6.AvgPowerDemandAtCrankFromElectricsIncHVAC / Sum0(signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property Sum11 As NewtonMeter
			Get

				Return Sum5 + Sum10
			End Get
		End Property

		Private ReadOnly Property Sum12 As GramPerSecond
			Get

				'SCHM 3_2
				Dim intrp3 As GramPerSecond = fmap.GetFuelConsumption(Sum11, signals.EngineSpeed)
				intrp3 = If(Not Double.IsNaN(intrp3.Value()) AndAlso intrp3 > 0, intrp3, 0.SI(Of GramPerSecond))
				Return intrp3
			End Get
		End Property

		'OUT1
		Public ReadOnly Property SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly As Joule _
			Implements IM11.SmartElectricalTotalCycleElectricalEnergyGeneratedDuringOverrunOnly
			Get
				Return AG1
			End Get
		End Property
		'OUT2
		Public ReadOnly Property SmartElectricalTotalCycleEletricalEnergyGenerated As Joule _
			Implements IM11.SmartElectricalTotalCycleEletricalEnergyGenerated
			Get
				Return AG2
			End Get
		End Property
		'OUT3
		Public ReadOnly Property TotalCycleElectricalDemand As Joule Implements IM11.TotalCycleElectricalDemand
			Get
				Return AG3
			End Get
		End Property
		'OUT4
		Public ReadOnly Property TotalCycleFuelConsumptionSmartElectricalLoad As Gram _
			Implements IM11.TotalCycleFuelConsumptionSmartElectricalLoad
			Get
				Return AG4
			End Get
		End Property
		'OUT5
		Public ReadOnly Property TotalCycleFuelConsumptionZeroElectricalLoad As Gram _
			Implements IM11.TotalCycleFuelConsumptionZeroElectricalLoad
			Get
				Return AG5
			End Get
		End Property
		'OUT6
		Public ReadOnly Property StopStartSensitiveTotalCycleElectricalDemand As Joule _
			Implements IM11.StopStartSensitiveTotalCycleElectricalDemand
			Get
				Return AG6
			End Get
		End Property
		'OUT7
		Public ReadOnly Property TotalCycleFuelConsuptionAverageLoads As Gram _
			Implements IM11.TotalCycleFuelConsuptionAverageLoads
			Get
				Return AG7
			End Get
		End Property

		Private ReadOnly Property SW1 As Boolean
			Get
				Return Not signals.EngineStopped
			End Get
		End Property

		'Clear at the beginning of cycle
		Sub ClearAggregates() Implements IM11.ClearAggregates

			AG1 = 0.SI(Of Joule)()
			AG2 = 0.SI(Of Joule)()
			AG3 = 0.SI(Of Joule)()
			AG4 = 0.SI(Of Gram)()
			AG5 = 0.SI(Of Gram)()
			AG6 = 0.SI(Of Joule)()
			AG7 = 0.SI(Of Gram)()
		End Sub

		'Add to Aggregates dependent on cycle step time.
		Sub CycleStep(stepTimeInSeconds As Second) Implements IM11.CycleStep

			'S/S Insensitive
			AG3 += (M6.AvgPowerDemandAtCrankFromElectricsIncHVAC * stepTimeInSeconds)


			If signals.EngineStopped Then Return

			'S/S Sensitive
			If (SW1) Then
				AG1 += (Sum1 * stepTimeInSeconds)
				AG2 += (M8.SmartElectricalAlternatorPowerGenAtCrank * stepTimeInSeconds)

				AG6 += (M6.AvgPowerDemandAtCrankFromElectricsIncHVAC * stepTimeInSeconds)

				'MQ: No longer needed - already per Second 'These need to be divided by 3600 as the Fuel Map output is in Grams/Second.
				AG4 += (Sum7 * stepTimeInSeconds)  '/ 3600
				AG5 += (Sum8 * stepTimeInSeconds) ' / 3600
				AG7 += (Sum12 * stepTimeInSeconds)			 '/ 3600
			End If
		End Sub

		'Constructor
		Public Sub New(m1 As IM1_AverageHVACLoadDemand, m3 As IM3_AveragePneumaticLoadDemand, m6 As IM6, m8 As IM8,
						fmap As IFuelConsumptionMap, signals As ISignals)

			Me.M1 = m1
			Me.M3 = m3
			Me.M6 = m6
			Me.M8 = m8
			Me.fmap = fmap
			Me.signals = signals
			ClearAggregates()
		End Sub
	End Class
End Namespace


