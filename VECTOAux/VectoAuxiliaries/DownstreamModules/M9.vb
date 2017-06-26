' Copyright 2017 European Union.
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
	Public Class M9
		Implements IM9
		Private Const RPM_TO_RADS_PER_SECOND As Single = 60 / (2 * Math.PI)	 '9.55F

#Region "Aggregates"

		'AG1
		Private _LitresOfAirCompressorOnContinuallyAggregate As NormLiter
		'AG2
		Private _LitresOfAirCompressorOnOnlyInOverrunAggregate As NormLiter
		'AG3
		Private _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate As Kilogram
		'AG4
		Private _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate As Kilogram

#End Region

#Region "Constructor Requirements"

		Private M1 As IM1_AverageHVACLoadDemand
		Private M4 As IM4_AirCompressor
		Private M6 As IM6
		Private M8 As IM8
		Private FMAP As IFuelConsumptionMap
		Private PSAC As IPneumaticsAuxilliariesConfig
		Private Signals As ISignals

#End Region


#Region "Class Outputs"
		'OUT 1
		Public ReadOnly Property LitresOfAirCompressorOnContinually As NormLiter _
			Implements IM9.LitresOfAirCompressorOnContinually
			Get
				Return _LitresOfAirCompressorOnContinuallyAggregate
			End Get
		End Property
		'OUT 2
		Public ReadOnly Property LitresOfAirCompressorOnOnlyInOverrun As NormLiter _
			Implements IM9.LitresOfAirCompressorOnOnlyInOverrun
			Get
				Return _LitresOfAirCompressorOnOnlyInOverrunAggregate
			End Get
		End Property
		'OUT 3
		Public ReadOnly Property TotalCycleFuelConsumptionCompressorOffContinuously As Kilogram _
			Implements IM9.TotalCycleFuelConsumptionCompressorOffContinuously
			Get
				Return _TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate
			End Get
		End Property
		'OUT 4
		Public ReadOnly Property TotalCycleFuelConsumptionCompressorOnContinuously As Kilogram _
			Implements IM9.TotalCycleFuelConsumptionCompressorOnContinuously
			Get
				Return _TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate
			End Get
		End Property

#End Region

		'Staging Calculations
		Private Function S0(ByVal rpm As PerSecond) As PerSecond

			If rpm < 1 Then rpm = 1.RPMtoRad()

			Return rpm	' / RPM_TO_RADS_PER_SECOND
		End Function

		Private ReadOnly Property S1 As Watt
			Get
				Return M6.AvgPowerDemandAtCrankFromElectricsIncHVAC + M1.AveragePowerDemandAtCrankFromHVACMechanicalsWatts
			End Get
		End Property

		Private ReadOnly Property S2 As NewtonMeter
			Get
				If S0(Signals.EngineSpeed).IsEqual(0) Then _
					Throw New DivideByZeroException("Engine speed is zero and cannot be used as a divisor.")
				Return M4.GetPowerCompressorOn / S0(Signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property S3 As NewtonMeter
			Get
				If S0(Signals.EngineSpeed).IsEqual(0) Then _
					Throw New DivideByZeroException("Engine speed is zero and cannot be used as a divisor.")
				Return M4.GetPowerCompressorOff / S0(Signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property S4 As NewtonMeter
			Get
				If S0(Signals.EngineSpeed).IsEqual(0) Then _
					Throw New DivideByZeroException("Engine speed is zero and cannot be used as a divisor.")
				Return S1 / S0(Signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property S5 As NewtonMeter
			Get
				Return S2 + S14
			End Get
		End Property

		Private ReadOnly Property S6 As NewtonMeter
			Get
				Return S14 + S3
			End Get
		End Property

		Private ReadOnly Property S7 As NewtonMeter
			Get
				Return S4 + S5
			End Get
		End Property

		Private ReadOnly Property S8 As NewtonMeter
			Get
				Return S4 + S6
			End Get
		End Property

		Private ReadOnly Property S9 As NormLiterPerSecond
			Get
				Return If(M6.OverrunFlag AndAlso M8.CompressorFlag, M4.GetFlowRate, SIBase(Of NormLiterPerSecond).Create(0))
			End Get
		End Property

		Private ReadOnly Property S10 As NormLiterPerSecond
			Get
				Return S13 * PSAC.OverrunUtilisationForCompressionFraction
			End Get
		End Property

		Private ReadOnly Property S11 As KilogramPerSecond
			Get
				'SCHM 3_02
				Dim int1 As KilogramPerSecond = FMAP.GetFuelConsumption(S7, Signals.EngineSpeed)
				int1 = If(int1 > 0 AndAlso Not Double.IsNaN(int1.Value()), int1, 0.SI(Of KilogramPerSecond))

				Return int1
			End Get
		End Property

		Private ReadOnly Property S12 As KilogramPerSecond
			Get

				'SCHM 3_02
				Dim int2 As KilogramPerSecond = FMAP.GetFuelConsumption(S8, Signals.EngineSpeed)
				int2 = If(int2 > 0 AndAlso Not Double.IsNaN(int2.Value()), int2, 0.SI(Of KilogramPerSecond))

				Return int2
			End Get
		End Property

		Private ReadOnly Property S13 As NormLiterPerSecond
			Get
				Return If(Signals.ClutchEngaged AndAlso Not (Signals.InNeutral), S9, SIBase(Of NormLiterPerSecond).Create(0))
			End Get
		End Property

		Private ReadOnly Property S14 As NewtonMeter
			Get
				Return _
					Signals.EngineDrivelineTorque +
					Signals.PreExistingAuxPower / S0(Signals.EngineSpeed)
			End Get
		End Property

		Private ReadOnly Property SW1 As Boolean
			Get
				Return Not Signals.EngineStopped
			End Get
		End Property

		'Utility Methods
		Public Sub ClearAggregates() Implements IM9.ClearAggregates

			_LitresOfAirCompressorOnContinuallyAggregate = SIBase(Of NormLiter).Create(0)
			_LitresOfAirCompressorOnOnlyInOverrunAggregate = SIBase(Of NormLiter).Create(0)
			_TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate = SIBase(Of Kilogram).Create(0)
			_TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate = SIBase(Of Kilogram).Create(0)
		End Sub

		Public Sub CycleStep(stepTimeInSeconds As Second) Implements IM9.CycleStep

			If Signals.EngineStopped Then Return

			If (SW1) Then
				_LitresOfAirCompressorOnContinuallyAggregate += M4.GetFlowRate * stepTimeInSeconds
				_LitresOfAirCompressorOnOnlyInOverrunAggregate += S10 * stepTimeInSeconds
				_TotalCycleFuelConsumptionCompressorOnContinuouslyAggregate += S11 * stepTimeInSeconds
				_TotalCycleFuelConsumptionCompressorOffContinuouslyAggregate += S12 * stepTimeInSeconds
			End If
		End Sub

		'Constructor
		Public Sub New(m1 As IM1_AverageHVACLoadDemand, m4 As IM4_AirCompressor, m6 As IM6, m8 As IM8,
						fmap As IFuelConsumptionMap, psac As IPneumaticsAuxilliariesConfig, signals As ISignals)
			Me.M1 = m1
			Me.M4 = m4
			Me.M6 = m6
			Me.M8 = m8
			Me.FMAP = fmap
			Me.PSAC = psac
			Me.Signals = signals

			ClearAggregates()
		End Sub

		'Auxiliary Event
		Public Event AuxiliaryEvent(ByRef sender As Object, message As String, messageType As AdvancedAuxiliaryMessageType) _
			Implements IAuxiliaryEvent.AuxiliaryEvent
	End Class
End Namespace


