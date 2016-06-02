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


Public Class Signals
	Implements ISignals

	'Backing variables
	Private _WHTCCorretion As Single = 1
	Private _smartElectrics As Boolean

	Public Property ClutchEngaged As Boolean Implements ISignals.ClutchEngaged
	Public Property EngineDrivelinePower As Double Implements ISignals.EngineDrivelinePower
	Public Property EngineDrivelineTorque As Double Implements ISignals.EngineDrivelineTorque
	Public Property EngineMotoringPower As Double Implements ISignals.EngineMotoringPower
	Public Property EngineSpeed As Double Implements ISignals.EngineSpeed

	Public Property SmartElectrics As Boolean Implements ISignals.SmartElectrics
		Get
			Return _smartElectrics
		End Get
		Set(value As Boolean)
			_smartElectrics = value
		End Set
	End Property

	Public Property SmartPneumatics As Boolean Implements ISignals.SmartPneumatics
	Public Property TotalCycleTimeSeconds As Integer Implements ISignals.TotalCycleTimeSeconds
	Public Property CurrentCycleTimeInSeconds As Double Implements ISignals.CurrentCycleTimeInSeconds
	Public Property PreExistingAuxPower As Double Implements ISignals.PreExistingAuxPower
	Public Property Idle As Boolean Implements ISignals.Idle
	Public Property InNeutral As Boolean Implements ISignals.InNeutral

	Public Property AuxiliaryEventReportingLevel As AdvancedAuxiliaryMessageType _
		Implements ISignals.AuxiliaryEventReportingLevel

	Public Property EngineStopped As Boolean Implements ISignals.EngineStopped
	Public Property DeclarationMode As Boolean Implements ISignals.DeclarationMode

	Public Property WHTC As Double Implements ISignals.WHTC
		Set(value As Double)
			_WHTCCorretion = value
		End Set
		Get
			Return _WHTCCorretion
		End Get
	End Property

	Public Property EngineIdleSpeed As Double Implements ISignals.EngineIdleSpeed
	Public Property PneumaticOverrunUtilisation As Double Implements ISignals.PneumaticOverrunUtilisation
	Public Property StoredEnergyEfficiency As Double Implements ISignals.StoredEnergyEfficiency
	Public Property RunningCalc As Boolean Implements ISignals.RunningCalc
	Public Property Internal_Engine_Power As Single Implements ISignals.Internal_Engine_Power
End Class


