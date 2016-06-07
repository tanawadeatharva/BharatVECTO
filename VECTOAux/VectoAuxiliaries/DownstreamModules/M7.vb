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
	Public Class M7
		Implements IM7

		Private _m5 As IM5_SmartAlternatorSetGeneration
		Private _m6 As IM6
		Private _signals As ISignals

		'Boolean  Conditions
		Private ReadOnly Property C1 As Boolean
			Get
				Return _m6.OverrunFlag AndAlso _signals.ClutchEngaged AndAlso _signals.InNeutral = False
			End Get
		End Property

		'Internal Switched Outputs 
		Private ReadOnly Property SW1 As Watt
			Get

				Dim idle As Boolean = _signals.EngineSpeed <= _signals.EngineIdleSpeed AndAlso
									(Not _signals.ClutchEngaged OrElse _signals.InNeutral)

				Return _
					If(idle, _m5.AlternatorsGenerationPowerAtCrankIdleWatts, _m5.AlternatorsGenerationPowerAtCrankTractionOnWatts)
			End Get
		End Property

		Private ReadOnly Property SW2 As Watt
			Get
				Return If(C1, _m6.SmartElecAndPneumaticAltPowerGenAtCrank, SW1)
			End Get
		End Property

		Private ReadOnly Property SW3 As Watt
			Get
				Return If(C1, _m6.SmartElecAndPneumaticAirCompPowerGenAtCrank, _m6.AveragePowerDemandAtCrankFromPneumatics)
			End Get
		End Property

		Private ReadOnly Property SW4 As Watt
			Get
				Return If(C1, _m6.SmartElecOnlyAltPowerGenAtCrank, SW1)
			End Get
		End Property

		Private ReadOnly Property SW5 As Watt
			Get
				Return If(C1, _m6.SmartPneumaticOnlyAirCompPowerGenAtCrank, _m6.AveragePowerDemandAtCrankFromPneumatics)
			End Get
		End Property

		'Public readonly properties  ( Outputs )
		Public ReadOnly Property SmartElectricalAndPneumaticAuxAltPowerGenAtCrank As Watt _
			Implements IM7.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank
			Get
				Return SW2
			End Get
		End Property

		Public ReadOnly Property SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank As Watt _
			Implements IM7.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank
			Get
				Return SW3
			End Get
		End Property

		Public ReadOnly Property SmartElectricalOnlyAuxAltPowerGenAtCrank As Watt _
			Implements IM7.SmartElectricalOnlyAuxAltPowerGenAtCrank
			Get
				Return SW4
			End Get
		End Property

		Public ReadOnly Property SmartPneumaticOnlyAuxAirCompPowerGenAtCrank As Watt _
			Implements IM7.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank
			Get
				Return SW5
			End Get
		End Property

		'Constructor
		Public Sub New(m5 As IM5_SmartAlternatorSetGeneration,
						m6 As IM6,
						signals As ISignals)

			_m5 = m5
			_m6 = m6
			_signals = signals
		End Sub
	End Class
End Namespace


