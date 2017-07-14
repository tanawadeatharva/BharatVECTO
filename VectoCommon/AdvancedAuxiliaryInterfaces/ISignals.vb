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

Public Interface ISignals
	''' <summary>
	''' Pre Existing Aux Power (KW)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property PreExistingAuxPower As Watt

	''' <summary>
	''' Engine Motoring Power (KW)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property EngineMotoringPower As Watt

	''' <summary>
	''' Engine Driveline Power (KW)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property EngineDrivelinePower As Watt

	''' <summary>
	''' Smart Electrics
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Should be true if fitted to the vehicle - AAUX Input</remarks>
	Property SmartElectrics As Boolean

	''' <summary>
	''' Clucth Engaged
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property ClutchEngaged As Boolean

	''' <summary>
	''' Engine Speed 1/NU
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' 
	Property EngineSpeed As PerSecond

	''' <summary>
	''' Smart Pneumatics
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>should be true if fitted to the vehicle- AAux Config Input</remarks>
	Property SmartPneumatics As Boolean

	''' <summary>
	''' Total Cycle Time In Seconds
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property TotalCycleTimeSeconds As Integer

	''' <summary>
	''' Current Cycle Time In Seconds 
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>( Will Increment during Cycle )</remarks>
	Property CurrentCycleTimeInSeconds As Double

	''' <summary>
	''' Engine Driveline Torque
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property EngineDrivelineTorque As NewtonMeter

	''' <summary>
	''' Engine Idle
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property Idle As Boolean

	''' <summary>
	''' In Neutral
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input</remarks>
	Property InNeutral As Boolean

	''' <summary>
	''' Auxiliary Event Reporting Level
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Can be used by Vecto to choose the level of reporting</remarks>
	Property AuxiliaryEventReportingLevel As AdvancedAuxiliaryMessageType

	''' <summary>
	''' Engine Stopped 
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>'Vecto Input - Used to Cut Fueling in AAux model</remarks>
	Property EngineStopped As Boolean

	''' <summary>
	''' WHTC ( Correction factor to be applied )
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>'Vecto Input</remarks>
	Property WHTC As Double

	''' <summary>
	''' Declaration Mode
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks>Vecto Input - Used to decide if to apply WHTC/Possiblye other things in future</remarks>
	Property DeclarationMode As Boolean

	''' <summary>
	''' Engine Idle Speed ( Associated with the vehicle bein tested )
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property EngineIdleSpeed As PerSecond

	''' <summary>
	''' Pneumatic Overrun Utilisation
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property PneumaticOverrunUtilisation As Double

	''' <summary>
	''' Stored Energy Efficiency
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property StoredEnergyEfficiency As Double

	''' <summary>
	''' Running Calc
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property RunningCalc As Boolean

	''' <summary>
	''' Running Calc
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property InternalEnginePower As Watt
End Interface
