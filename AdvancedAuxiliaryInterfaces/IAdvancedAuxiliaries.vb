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

Public Interface IAdvancedAuxiliaries
	' Inherits IAuxiliaryEvent

	Event AuxiliaryEvent(ByRef sender As Object, ByVal message As String, ByVal messageType As AdvancedAuxiliaryMessageType)


	'Information
	ReadOnly Property Running As Boolean

	ReadOnly Property AuxiliaryName As String
	ReadOnly Property AuxiliaryVersion As String


	'Diagnostic Only - Remove when beta over.
	ReadOnly Property AA_D_M12_P1X As Single
	ReadOnly Property AA_D_M12_P1Y As Single
	ReadOnly Property AA_D_M12_P2X As Single
	ReadOnly Property AA_D_M12_P2Y As Single
	ReadOnly Property AA_D_M12_P3X As Single
	ReadOnly Property AA_D_M12_P3Y As Single
	ReadOnly Property AA_D_M12_XTAIN As Single
	ReadOnly Property AA_D_M12_INTERP1 As Single
	ReadOnly Property AA_D_M12_INTERP2 As Single

	'Additional Permenent Monitoring Signals - Required by engineering
	ReadOnly Property AA_NonSmartAlternatorsEfficiency As Single?
	ReadOnly Property AA_SmartIdleCurrent_Amps As Single?
	ReadOnly Property AA_SmartIdleAlternatorsEfficiency As Single?
	ReadOnly Property AA_SmartTractionCurrent_Amps As Single?
	ReadOnly Property AA_SmartTractionAlternatorEfficiency As Single?
	ReadOnly Property AA_SmartOverrunCurrent_Amps As Single?
	ReadOnly Property AA_SmartOverrunAlternatorEfficiency As Single?
	ReadOnly Property AA_CompressorFlowRate_LitrePerSec As Single?
	ReadOnly Property AA_OverrunFlag As Integer?
	ReadOnly Property AA_EngineIdleFlag As Integer?
	ReadOnly Property AA_CompressorFlag As Integer?
	ReadOnly Property AA_TotalCycleFC_Grams As Single?
	ReadOnly Property AA_TotalCycleFC_Litres As Single?
	ReadOnly Property AA_AveragePowerDemandCrankHVACMechanicals As Single?
	ReadOnly Property AA_AveragePowerDemandCrankHVACElectricals As Single?
	ReadOnly Property AA_AveragePowerDemandCrankElectrics As Single?
	ReadOnly Property AA_AveragePowerDemandCrankPneumatics As Single?
	ReadOnly Property AA_TotalCycleFuelConsumptionCompressorOff As Single?
	ReadOnly Property AA_TotalCycleFuelConsumptionCompressorOn As Single?

	''' <summary>
	''' Total Cycle Fuel In Grams
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	ReadOnly Property TotalFuelGRAMS As Single

	''' <summary>
	''' Total Cycle Fuel in Litres
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	ReadOnly Property TotalFuelLITRES As Single

	''' <summary>
	''' Total Power Demans At Crank From Auxuliaries (W)
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	ReadOnly Property AuxiliaryPowerAtCrankWatts As Single


	''' <summary>
	''' Vecto Inputs
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property VectoInputs As IVectoInputs

	''' <summary>
	''' Signals From Vecto 
	''' </summary>
	''' <value></value>
	''' <returns></returns>
	''' <remarks></remarks>
	Property Signals As ISignals

	''' <summary>
	''' Configure Auxuliaries ( Launches Config Form )
	''' </summary>
	''' <param name="filePath"></param>
	''' <param name="vectoFilePath"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function Configure(filePath As String, vectoFilePath As String) As Boolean

	''' <summary>
	''' Validate AAUX file path supplied.
	''' </summary>
	''' <param name="filePath"></param>
	''' <param name="message"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function ValidateAAUXFile(ByVal filePath As String, ByRef message As String) As Boolean

	'Command
	''' <summary>
	''' Cycle Step - Used to calculate fuelling
	''' </summary>
	''' <param name="seconds"></param>
	''' <param name="message"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function CycleStep(seconds As Integer, ByRef message As String) As Boolean

	''' <summary>
	''' Initialises AAUX Environment ( Begin Processs )
	''' </summary>
	''' <param name="auxFilePath"></param>
	''' <param name="vectoFilePath"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function RunStart(ByVal auxFilePath As String, ByVal vectoFilePath As String) As Boolean

	''' <summary>
	''' Any Termination Which Needs to be done ( Model depenent )
	''' </summary>
	''' <param name="message"></param>
	''' <returns></returns>
	''' <remarks></remarks>
	Function RunStop(ByRef message As String) As Boolean
End Interface


