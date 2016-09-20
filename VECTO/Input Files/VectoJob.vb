' Copyright 2014 European Union.
' Licensed under the EUPL (the 'Licence');
'
' * You may not use this work except in compliance with the Licence.
' * You may obtain a copy of the Licence at: http://ec.europa.eu/idabc/eupl
' * Unless required by applicable law or agreed to in writing,
'   software distributed under the Licence is distributed on an "AS IS" basis,
'   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
'
' See the LICENSE.txt for the specific language governing permissions and limitations.
'Option Infer On
'Option Explicit On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.Impl
Imports TUGraz.VectoCore.Models.Simulation.Data
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(VectoJob), "ValidateJob")>
Public Class VectoJob
	Implements IEngineeringInputDataProvider, IDeclarationInputDataProvider, IEngineeringJobInputData, 
				IDeclarationJobInputData, IDriverEngineeringInputData, IDriverDeclarationInputData

	Private Const FormatVersion As Short = 3

	'AA-TB
	'STORES THE Type and version of the chosen or default Auxiliary Type ( Classic/Original or other )
	Public AuxiliaryAssembly As String
	Public AuxiliaryVersion As String
	Public AdvancedAuxiliaryFilePath As String

	Private _sFilePath As String
	Private _myPath As String

	'Input parameters
	Private ReadOnly _vehicleFile As SubPath
	Private ReadOnly _engineFile As SubPath
	Private ReadOnly _gearboxFile As SubPath

	Private _startStop As Boolean
	Public StartStopDelay As Double

	Private ReadOnly _driverAccelerationFile As SubPath

	Public ReadOnly AuxPaths As Dictionary(Of String, AuxEntry)
	'Alle Nebenverbraucher die in der Veh-Datei UND im Zyklus definiert sind

	Public ReadOnly CycleFiles As List(Of SubPath)

	Public EngineOnly As Boolean

	Public VMin As Double
	Public LookAheadOn As Boolean
	Public OverSpeedOn As Boolean
	Public OverSpeed As Double
	Public UnderSpeed As Double
	Public EcoRollOn As Boolean

	Public SavedInDeclMode As Boolean
	Private _vehicleInputData As JSONComponentInputData
	Private _engineInputData As JSONComponentInputData
	Private _gearboxInputData As JSONComponentInputData

	Public Property StartStopMaxSpeed As Double

	Public Property StartStopTime As Double

	Public Class AuxEntry
		Public Type As String
		Public ReadOnly Path As SubPath
		Public ReadOnly TechnologyList As List(Of String)

		Public Sub New()
			Path = New SubPath
			TechnologyList = New List(Of String)()
		End Sub
	End Class

	Public Sub New()

		_myPath = ""
		_sFilePath = ""

		_vehicleFile = New SubPath
		_engineFile = New SubPath
		_gearboxFile = New SubPath

		_driverAccelerationFile = New SubPath

		AuxPaths = New Dictionary(Of String, AuxEntry)

		CycleFiles = New List(Of SubPath)
	End Sub

	Public Function SaveFile() As Boolean
		Dim json As New JSONWriter

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join("; ", messages), MsgBoxStyle.OkOnly,
					"Failed to save Vecto Job")
			Return False
		End If

		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
				{"Date", Now.ToUniversalTime().ToString("o")},
				{"AppVersion", VECTOvers},
				{"FileVersion", FormatVersion}}

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		'Main Files
		body.Add("VehicleFile", _vehicleFile.PathOrDummy)
		body.Add("EngineFile", _engineFile.PathOrDummy)
		body.Add("GearboxFile", _gearboxFile.PathOrDummy)

		'Cycles
		If CycleFiles.Count > 0 Then
			body.Add("Cycles", CycleFiles.Select(Function(sb) sb.PathOrDummy))
		End If

		'AA-TB
		'ADVANCED AUXILIARIES 
		body.Add("AuxiliaryAssembly", AuxiliaryAssembly)
		body.Add("AuxiliaryVersion", AuxiliaryVersion)
		body.Add("AdvancedAuxiliaryFilePath", AdvancedAuxiliaryFilePath)

		If AuxPaths.Any() Then
			body.Add("Aux", AuxPaths.Select(Function(kv) New Dictionary(Of String, Object) From {
												{"ID", Trim(UCase(kv.Key))},
												{"Type", kv.Value.Type},
												{"Path", kv.Value.Path.PathOrDummy},
												{"Technology", kv.Value.TechnologyList}
												}))
		End If

		body.Add("VACC", _driverAccelerationFile.PathOrDummy)
		body.Add("EngineOnlyMode", EngineOnly)
		body.Add("StartStop", New Dictionary(Of String, Object) From {
					{"Enabled", _startStop},
					{"MaxSpeed", StartStopMaxSpeed},
					{"MinTime", StartStopTime},
					{"Delay", StartStopDelay}})
		body.Add("LAC", New Dictionary(Of String, Object) From {
					{"Enabled", LookAheadOn},
					{"PreviewDistanceFactor", LacPreviewFactor},
					{"DF_offset", LacDfOffset},
					{"DF_scaling", LacDfScale},
					{"DF_targetSpeedLookup", LacDfTargetSpeedFile},
					{"Df_velocityDropLookup", LacDfVelocityDropFile}})

		'Overspeed / EcoRoll
		Dim overspeedDic As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		If EcoRollOn Then
			overspeedDic.Add("Mode", "EcoRoll")
		ElseIf OverSpeedOn Then
			overspeedDic.Add("Mode", "OverSpeed")
		Else
			overspeedDic.Add("Mode", "Off")
		End If
		overspeedDic.Add("MinSpeed", VMin)
		overspeedDic.Add("OverSpeed", OverSpeed)
		overspeedDic.Add("UnderSpeed", UnderSpeed)
		body.Add("OverSpeedEcoRoll", overspeedDic)

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})
		Return json.WriteFile(_sFilePath)
	End Function

	'Public Function ReadFile() As Boolean
	'	Const msgSrc = "Main/ReadInp/GEN"

	'	SetDefault()

	'	Dim json As New JSONParser
	'	If Not json.ReadFile(_sFilePath) Then Return False

	'	Try

	'		Dim fileVersion As Integer = json.Content.GetEx("Header").GetEx(Of Integer)("FileVersion")

	'		Dim body As JToken = json.Content.GetEx("Body")

	'		If fileVersion > 1 Then
	'			SavedInDeclMode = body.GetEx(Of Boolean)("SavedInDeclMode")
	'		Else
	'			SavedInDeclMode = Cfg.DeclMode
	'		End If

	'		If Not body("VehicleFile") Is Nothing Then _
	'			_vehicleFile.Init(_myPath, body.GetEx(Of String)("VehicleFile"))

	'		_engineFile.Init(_myPath, body.GetEx(Of String)("EngineFile"))

	'		If Not body("GearboxFile") Is Nothing Then _
	'			_gearboxFile.Init(_myPath, body.GetEx(Of String)("GearboxFile"))

	'		If Not body("Cycles") Is Nothing Then
	'			For Each entry As JToken In body.GetEx("Cycles")
	'				Dim subPath = New SubPath
	'				subPath.Init(_myPath, entry.Value(Of String))
	'				CycleFiles.Add(subPath)
	'			Next
	'		End If

	'		'AA-TB
	'		'ADVANCED AUXILIARIES 
	'		If Not body("AuxiliaryAssembly") Is Nothing AndAlso
	'			Not body("AuxiliaryVersion") Is Nothing Then

	'			AuxiliaryAssembly = body("AuxiliaryAssembly").ToString()
	'			AuxiliaryVersion = body("AuxiliaryVersion").ToString()

	'		End If
	'		If Not body("AdvancedAuxiliaryFilePath") Is Nothing Then
	'			AdvancedAuxiliaryFilePath = body("AdvancedAuxiliaryFilePath").ToString()
	'		End If


	'		If Not body("Aux") Is Nothing Then
	'			For Each dic As JToken In body.GetEx("Aux")

	'				Dim auxId As String = UCase(Trim(dic.GetEx(Of String)("ID")))

	'				If AuxPaths.ContainsKey(auxId) Then
	'					WorkerMsg(MessageType.Err, "Multiple definitions of the same auxiliary type (" & auxId & ")!", msgSrc)
	'					Return False
	'				End If

	'				Dim auxEntry = New AuxEntry

	'				auxEntry.Type = dic.GetEx(Of String)("Type")
	'				auxEntry.Path.Init(_myPath, dic.GetEx(Of String)("Path"))

	'				If Not dic("Technology") Is Nothing Then
	'					If fileVersion = 2 Then
	'						auxEntry.TechnologyList.Add(dic.GetEx(Of String)("Technology"))
	'					End If
	'					If fileVersion = 3 Then
	'						auxEntry.TechnologyList = dic.GetEx("Technology").ToObject(Of List(Of String))() '.FirstOrDefault()
	'					End If
	'				End If

	'				If (auxId = Constants.AuxiliaryKey.HVAC) Then
	'					If auxEntry.TechnologyList.Count > 0 Then ' Not String.IsNullOrWhiteSpace(auxEntry.TechStr) Then
	'						auxEntry.TechnologyList.Clear()
	'						WorkerMsg(MessageType.Normal, "Aux: Automatically Upgraded HVAC to new format.", msgSrc)
	'					End If
	'				End If

	'				If auxId = Constants.AuxiliaryKey.ElecSys Then
	'					If auxEntry.TechnologyList.Contains("Custom Technology List") OrElse auxEntry.TechnologyList.Count > 0 Then
	'						Dim hasTech = False

	'						If Not dic("TechList") Is Nothing Then
	'							For Each t In dic("TechList")
	'								hasTech = True
	'							Next
	'						End If

	'						auxEntry.TechnologyList.Clear()
	'						If Not hasTech Then
	'							auxEntry.TechnologyList.Add("Standard technology")
	'						Else
	'							auxEntry.TechnologyList.Add("Standard technology - LED headlights, all")
	'						End If
	'						WorkerMsg(MessageType.Normal,
	'								"Aux: Automatically Upgraded Electric System to new format: '" + auxEntry.TechnologyList.FirstOrDefault() + "'",
	'								msgSrc)
	'					End If
	'				End If

	'				If auxId = Constants.AuxiliaryKey.SteerPump Then
	'					If _
	'						auxEntry.TechnologyList.Contains("Variable displacement") OrElse
	'						auxEntry.TechnologyList.Contains("Hydraulic supported by electric") Then
	'						auxEntry.TechnologyList.Clear()
	'						WorkerMsg(MessageType.Warn, "Aux: Steering Pump Technology not automatically convertible. Please set new value.",
	'								msgSrc)
	'					End If
	'				End If

	'				If auxId = Constants.AuxiliaryKey.Fan Then
	'					If auxEntry.TechnologyList.Contains("Crankshaft mounted - Electronically controlled visco clutch (Default)") Then
	'						auxEntry.TechnologyList.Clear()
	'						auxEntry.TechnologyList.Add("Crankshaft mounted - Electronically controlled visco clutch")
	'					End If
	'					If auxEntry.TechnologyList.Contains("Crankshaft mounted - On/Off clutch") Then
	'						auxEntry.TechnologyList.Clear()
	'						auxEntry.TechnologyList.Add("Crankshaft mounted - On/off clutch")
	'					End If
	'					If auxEntry.TechnologyList.Contains("Belt driven or driven via transm. - On/Off clutch") Then
	'						auxEntry.TechnologyList.Clear()
	'						auxEntry.TechnologyList.Add("Belt driven or driven via transm. - On/off clutch")
	'					End If
	'				End If

	'				If fileVersion = 2 AndAlso auxId = Constants.AuxiliaryKey.PneumSys Then
	'					auxEntry.TechnologyList.Clear()
	'					WorkerMsg(MessageType.Warn, "Aux: Pneumatic System must be updated. Please set new value.",
	'							msgSrc)
	'				End If

	'				AuxPaths.Add(auxId, auxEntry)

	'			Next
	'		End If

	'		If Not body("VACC") Is Nothing Then
	'			_driverAccelerationFile.Init(_myPath, body.GetEx(Of String)("VACC"))
	'		End If

	'		EngineOnly = body.GetEx(Of Boolean)("EngineOnlyMode")

	'		If Not body("StartStop") Is Nothing Then
	'			Dim startStop As JToken = body.GetEx("StartStop")
	'			_startStop = startStop.GetEx(Of Boolean)("Enabled")
	'			_startStopMaxSpeed = startStop.GetEx(Of Double)("MaxSpeed")
	'			_startStopMinTime = startStop.GetEx(Of Double)("MinTime")
	'			StartStopDelay = startStop.GetEx(Of Double)("Delay")
	'		Else
	'			_startStop = False
	'		End If

	'		If Not body("LAC") Is Nothing Then
	'			Dim lac = body.GetEx("LAC")
	'			LookAheadOn = lac.GetEx(Of Boolean)("Enabled")
	'			LacPreviewFactor = If(lac("PreviewDistanceFactor") Is Nothing, 10, lac.GetEx(Of Double)("PreviewDistanceFactor"))
	'			LacDfOffset = If(lac("DF_offset") Is Nothing, 2.5, lac.GetEx(Of Double)("DF_offset"))
	'			LacDfScale = If(lac("DF_scaling") Is Nothing, 1.5, lac.GetEx(Of Double)("DF_scaling"))
	'			LacDfTargetSpeedFile =
	'				If(Not lac("DF_targetSpeedLookup") Is Nothing, lac.GetEx(Of String)("DF_targetSpeedLookup"), "")
	'			LacDfVelocityDropFile =
	'				If(Not lac("Df_velocityDropLookup") Is Nothing, lac.GetEx(Of String)("Df_velocityDropLookup"), "")
	'		Else
	'			LookAheadOn = False
	'		End If

	'		If Not body("OverSpeedEcoRoll") Is Nothing Then
	'			Dim dic = body("OverSpeedEcoRoll")

	'			Select Case UCase(dic("Mode").ToString).Trim
	'				Case "ECOROLL"
	'					OverSpeedOn = False
	'					EcoRollOn = True

	'				Case "OVERSPEED"
	'					OverSpeedOn = True
	'					EcoRollOn = False

	'				Case "OFF"
	'					OverSpeedOn = False
	'					EcoRollOn = False

	'				Case Else
	'					WorkerMsg(MessageType.Err, "Value '" & dic("Mode").ToString() & "' is not valid for OverSpeedEcoRoll/Mode!",
	'							msgSrc)
	'					Return False
	'			End Select

	'			VMin = dic.GetEx(Of Double)("MinSpeed")
	'			OverSpeed = dic.GetEx(Of Double)("OverSpeed")
	'			If Not dic("UnderSpeed") Is Nothing Then UnderSpeed = dic.GetEx(Of Double)("UnderSpeed")

	'		Else
	'			OverSpeedOn = False
	'			EcoRollOn = False
	'		End If


	'	Catch ex As Exception
	'		WorkerMsg(MessageType.Err, "Failed to read VECTO file! " & ex.Message, msgSrc)
	'		Return False
	'	End Try


	'	Return True
	'End Function

	Private Sub SetDefault()

		AuxiliaryAssembly = "CLASSIC"
		AuxiliaryVersion = "CLASSIC"
		AdvancedAuxiliaryFilePath = String.Empty


		_startStop = False
		StartStopMaxSpeed = 5
		StartStopTime = 5
		StartStopDelay = 0

		_vehicleFile.Clear()
		_engineFile.Clear()
		CycleFiles.Clear()
		_gearboxFile.Clear()

		_driverAccelerationFile.Clear()

		AuxPaths.Clear()
		EngineOnly = False

		VMin = 0
		LookAheadOn = True
		OverSpeedOn = False
		EcoRollOn = False
		OverSpeed = 0
		UnderSpeed = 0

		SavedInDeclMode = False
	End Sub

	'This Sub reads those Input-files that do not have their own class, etc.


#Region "Properties"


	Public Property FilePath As String
		Get
			Return _sFilePath
		End Get
		Set(value As String)
			_sFilePath = value
			If _sFilePath = "" Then
				_myPath = ""
			Else
				_myPath = Path.GetDirectoryName(_sFilePath) & "\"
			End If
		End Set
	End Property


	Public Property PathVeh(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _vehicleFile.OriginalPath
			Else
				Return _vehicleFile.FullPath
			End If
		End Get
		Set(value As String)
			_vehicleFile.Init(_myPath, value)
		End Set
	End Property

	Public Property PathEng(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _engineFile.OriginalPath
			Else
				Return _engineFile.FullPath
			End If
		End Get
		Set(value As String)
			_engineFile.Init(_myPath, value)
		End Set
	End Property

	Public Property PathGbx(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _gearboxFile.OriginalPath
			Else
				Return _gearboxFile.FullPath
			End If
		End Get
		Set(value As String)
			_gearboxFile.Init(_myPath, value)
		End Set
	End Property


	Public ReadOnly Property IDriverDeclarationInputData_SavedInDeclarationMode As Boolean _
		Implements IDriverDeclarationInputData.SavedInDeclarationMode
		Get
			Return SavedInDeclMode
		End Get
	End Property

	Public Property StartStop As Boolean
		Get
			Return _startStop
		End Get
		Set(value As Boolean)
			_startStop = value
		End Set
	End Property

	Public ReadOnly Property IDriverEngineeringInputData_OverSpeedEcoRoll As IOverSpeedEcoRollEngineeringInputData _
		Implements IDriverEngineeringInputData.OverSpeedEcoRoll
		Get
			Dim mode As DriverMode = DriverMode.Off
			If EcoRollOn Then
				mode = DriverMode.EcoRoll
			ElseIf OverSpeedOn Then
				mode = DriverMode.Overspeed
			End If

			Return New OverSpeedEcoRollInputData() With {
				.Mode = mode,
				.MinSpeed = VMin.KMPHtoMeterPerSecond(),
				.OverSpeed = OverSpeed.KMPHtoMeterPerSecond(),
				.UnderSpeed = UnderSpeed.KMPHtoMeterPerSecond()
				}
		End Get
	End Property

	Public ReadOnly Property IDriverEngineeringInputData_StartStop As IStartStopEngineeringInputData _
		Implements IDriverEngineeringInputData.StartStop
		Get
			Return New StartStopInputData With {
				.Enabled = _startStop,
				.MaxSpeed = StartStopMaxSpeed.KMPHtoMeterPerSecond(),
				.MinTime = StartStopTime.SI(Of Second)(),
				.Delay = StartStopDelay.SI(Of Second)()
				}
		End Get
	End Property

	Public ReadOnly Property OverSpeedEcoRoll As IOverSpeedEcoRollDeclarationInputData _
		Implements IDriverDeclarationInputData.OverSpeedEcoRoll
		Get
			Return IDriverEngineeringInputData_OverSpeedEcoRoll
		End Get
	End Property

	Public ReadOnly Property AccelerationCurve As TableData Implements IDriverEngineeringInputData.AccelerationCurve
		Get
			If String.IsNullOrWhiteSpace(_driverAccelerationFile.FullPath) Then Return Nothing
			If Not File.Exists(_driverAccelerationFile.FullPath) Then
				Try
					Dim cycleDataRes As Stream =
							RessourceHelper.ReadStream(RessourceHelper.Namespace + "VACC." + _driverAccelerationFile.OriginalPath + VectoCore.Configuration.Constants.FileExtensions.DriverAccelerationCurve)
					Return VectoCSVFile.ReadStream(cycleDataRes)
				Catch ex As Exception
					Return Nothing
				End Try
			End If
			Return VectoCSVFile.Read(_driverAccelerationFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property Lookahead As ILookaheadCoastingInputData Implements IDriverEngineeringInputData.Lookahead
		Get
			Dim lacTargetLookup As TableData =
					If(File.Exists(LacDfTargetSpeedFile), VectoCSVFile.Read(LacDfTargetSpeedFile), Nothing)
			Dim lacVdropLookup As TableData =
					If(File.Exists(LacDfVelocityDropFile), VectoCSVFile.Read(LacDfVelocityDropFile), Nothing)
			Return New LookAheadCoastingInputData With {
				.CoastingDecisionFactorScaling = LacDfScale,
				.CoastingDecisionFactorOffset = LacDfOffset,
				.Enabled = LookAheadOn,
				.LookaheadDistanceFactor = LacPreviewFactor,
				.CoastingDecisionFactorTargetSpeedLookup = lacTargetLookup,
				.CoastingDecisionFactorVelocityDropLookup = lacVdropLookup
				}
		End Get
	End Property


	Public Property DesMaxFile(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _driverAccelerationFile.OriginalPath
			Else
				Return _driverAccelerationFile.FullPath
			End If
		End Get
		Set(value As String)
			_driverAccelerationFile.Init(_myPath, value)
		End Set
	End Property

	Public Property LacPreviewFactor As Double
	Public Property LacDfOffset As Double
	Public Property LacDfScale As Double
	Public Property LacDfTargetSpeedFile As String
	Public Property LacDfVelocityDropFile As String


#End Region

	' ReSharper disable once UnusedMember.Global -- used by Validation
	Public Shared Function ValidateJob(vectoJob As VectoJob, validationContext As ValidationContext) As ValidationResult
		Dim modeService As ExecutionModeServiceContainer = TryCast(validationContext.GetService(GetType(ExecutionMode)), 
																	ExecutionModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

		Dim jobData As IEnumerable(Of VectoRunData)

		vectoJob._vehicleInputData = New JSONComponentInputData(vectoJob._vehicleFile.FullPath)
		vectoJob._engineInputData = New JSONComponentInputData(vectoJob._engineFile.FullPath)
		vectoJob._gearboxInputData = New JSONComponentInputData(vectoJob._gearboxFile.FullPath)

		Dim result As IList(Of ValidationResult) = New List(Of ValidationResult)
		Try
			If mode = ExecutionMode.Declaration Then
				If Not vectoJob._vehicleInputData.VehicleInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Vehicle File is not in Declaration Mode"))
				End If
				If Not vectoJob._engineInputData.EngineInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Engine File is not in Declaration Mode"))
				End If
				If Not vectoJob._gearboxInputData.GearboxInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Gearbox File is not in Declaration Mode"))
				End If
				If result.Any() Then
					Return _
						New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
				End If

				Dim dataFactory As DeclarationModeVectoRunDataFactory = New DeclarationModeVectoRunDataFactory(vectoJob, Nothing)

				jobData = dataFactory.NextRun()
			Else
				If vectoJob._vehicleInputData.VehicleInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Vehicle File is not in Engineering Mode"))
				End If
				If vectoJob._engineInputData.EngineInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Engine File is not in Engineering Mode"))
				End If
				If vectoJob._gearboxInputData.GearboxInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Gearbox File is not in Engineering Mode"))
				End If
				If result.Any() Then
					Return _
						New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
				End If
				Dim dataFactory As EngineeringModeVectoRunDataFactory = New EngineeringModeVectoRunDataFactory(vectoJob)
				jobData = dataFactory.NextRun()
			End If


			jobData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))
			If result.Any() Then
				Return _
					New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
			End If


			Return ValidationResult.Success

		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		Finally
			vectoJob._vehicleInputData = Nothing
			vectoJob._engineInputData = Nothing
			vectoJob._gearboxInputData = Nothing
		End Try
	End Function


#Region "IInputData"

	Public Function JobInputData() As IEngineeringJobInputData Implements IEngineeringInputDataProvider.JobInputData
		Return Me
	End Function

	Public ReadOnly Property IDeclarationInputDataProvider_VehicleInputData As IVehicleDeclarationInputData _
		Implements IDeclarationInputDataProvider.VehicleInputData
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public Function IDeclarationInputDataProvider_JobInputData() As IDeclarationJobInputData _
		Implements IDeclarationInputDataProvider.JobInputData
		Throw New NotImplementedException
	End Function

	Public ReadOnly Property VehicleInputData As IVehicleEngineeringInputData _
		Implements IEngineeringInputDataProvider.VehicleInputData
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_GearboxInputData As IGearboxDeclarationInputData _
		Implements IDeclarationInputDataProvider.GearboxInputData
		Get
			Return _gearboxInputData.GearboxInputData
		End Get
	End Property

	Public ReadOnly Property GearboxInputData As IGearboxEngineeringInputData _
		Implements IEngineeringInputDataProvider.GearboxInputData
		Get
			Return _gearboxInputData.GearboxInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_TorqueConverterInputData As ITorqueConverterDeclarationInputData _
		Implements IDeclarationInputDataProvider.TorqueConverterInputData
		Get
			Return _gearboxInputData.TorqueConverterInputData
		End Get
	End Property

	Public ReadOnly Property TorqueConverterInputData As ITorqueConverterEngineeringInputData _
		Implements IEngineeringInputDataProvider.TorqueConverterInputData
		Get
			Return _gearboxInputData.TorqueConverterInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_AxleGearInputData As IAxleGearInputData _
		Implements IDeclarationInputDataProvider.AxleGearInputData
		Get
			Return _gearboxInputData.AxleGearInputData
		End Get
	End Property

	Public ReadOnly Property AxleGearInputData As IAxleGearInputData _
		Implements IEngineeringInputDataProvider.AxleGearInputData
		Get
			Return _gearboxInputData.AxleGearInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_AngularGearInputData As IAngularGearInputData _
		Implements IDeclarationInputDataProvider.AngularGearInputData
		Get
			Return _vehicleInputData.AngularGearInputData
		End Get
	End Property

	Public ReadOnly Property AngularGearInputData As IAngularGearInputData _
		Implements IEngineeringInputDataProvider.AngularGearInputData
		Get
			Return _vehicleInputData.AngularGearInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_EngineInputData As IEngineDeclarationInputData _
		Implements IDeclarationInputDataProvider.EngineInputData
		Get
			Return _engineInputData.EngineInputData
		End Get
	End Property

	Public ReadOnly Property EngineInputData As IEngineEngineeringInputData _
		Implements IEngineeringInputDataProvider.EngineInputData
		Get
			Return _engineInputData.EngineInputData
		End Get
	End Property

	Public Function AuxiliaryInputData() As IAuxiliariesEngineeringInputData _
		Implements IEngineeringInputDataProvider.AuxiliaryInputData

		Throw New NotImplementedException
	End Function

	Public ReadOnly Property IDeclarationInputDataProvider_RetarderInputData As IRetarderInputData _
		Implements IDeclarationInputDataProvider.RetarderInputData
		Get
			Return _vehicleInputData.RetarderInputData
		End Get
	End Property

	Public Function IDeclarationInputDataProvider_AuxiliaryInputData() As IAuxiliariesDeclarationInputData _
		Implements IDeclarationInputDataProvider.AuxiliaryInputData
		Throw New NotImplementedException
	End Function

	Public ReadOnly Property RetarderInputData As IRetarderInputData _
		Implements IEngineeringInputDataProvider.RetarderInputData
		Get
			Return _vehicleInputData.RetarderInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_DriverInputData As IDriverDeclarationInputData _
		Implements IDeclarationInputDataProvider.DriverInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property DriverInputData As IDriverEngineeringInputData _
		Implements IEngineeringInputDataProvider.DriverInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property PTOTransmissionInputData As IPTOTransmissionInputData _
		Implements IEngineeringInputDataProvider.PTOTransmissionInputData
		Get
			Return _vehicleInputData.PTOTransmissionInputData
		End Get
	End Property


	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IDeclarationJobInputData.SavedInDeclarationMode
		Get
			Return SavedInDeclMode
		End Get
	End Property

	Public ReadOnly Property IDriverDeclarationInputData_StartStop As IStartStopDeclarationInputData _
		Implements IDriverDeclarationInputData.StartStop
		Get
			Return IDriverEngineeringInputData_StartStop
		End Get
	End Property

	Public ReadOnly Property IEngineeringJobInputData_Vehicle As IVehicleEngineeringInputData _
		Implements IEngineeringJobInputData.Vehicle
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public ReadOnly Property Vehicle As IVehicleDeclarationInputData Implements IDeclarationJobInputData.Vehicle
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public ReadOnly Property Cycles As IList(Of ICycleData) Implements IEngineeringJobInputData.Cycles
		Get
			Dim retVal As ICycleData() = New ICycleData(CycleFiles.Count) {}
			Dim i As Integer = 0
			For Each cycleFile As SubPath In CycleFiles
				Dim cycleData As TableData
				If (File.Exists(cycleFile.FullPath)) Then
					cycleData = VectoCSVFile.Read(cycleFile.FullPath)
				Else
					Try
						Dim cycleDataRes As Stream =
								RessourceHelper.ReadStream(RessourceHelper.Namespace + "MissionCycles." + cycleFile.OriginalPath + ".vdri")
						cycleData = VectoCSVFile.ReadStream(cycleDataRes)
					Catch ex As Exception
						Throw New VectoException("Driving Cycle could not be read: " + cycleFile.OriginalPath)
					End Try
				End If
				retVal(i) = New CycleInputData With {
					.Name = Path.GetFileNameWithoutExtension(cycleFile.FullPath),
					.CycleData = cycleData
					}
				i += 1
			Next
			Return retVal
		End Get
	End Property

	Public ReadOnly Property EngineOnlyMode As Boolean Implements IEngineeringJobInputData.EngineOnlyMode
		Get
			Return EngineOnly
		End Get
	End Property

	Public ReadOnly Property JobName As String Implements IDeclarationJobInputData.JobName
		Get
			Return Path.GetFileNameWithoutExtension(FilePath)
		End Get
	End Property

#End Region
End Class


