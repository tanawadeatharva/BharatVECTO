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
Option Infer On
Option Explicit On

Imports System.Collections.Generic
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class VectoJob
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
	Private _startStopMaxSpeed As Double
	Private _startStopMinTime As Double
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

	Public Class AuxEntry
		Public Type As String
		Public ReadOnly Path As SubPath
		Public TechnologyList As List(Of String)

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
		Dim json As New JSONParser

		'Header
		json.Content.Add("Header", JToken.FromObject(New Dictionary(Of String, Object) From {
														{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
														{"Date", Now.ToUniversalTime().ToString("o")},
														{"AppVersion", VECTOvers},
														{"FileVersion", FormatVersion}}))

		'Body
		Dim dic0 = New Dictionary(Of String, Object)

		dic0.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		'Main Files
		dic0.Add("VehicleFile", _vehicleFile.PathOrDummy)
		dic0.Add("EngineFile", _engineFile.PathOrDummy)
		dic0.Add("GearboxFile", _gearboxFile.PathOrDummy)

		'Cycles
		If CycleFiles.Count > 0 Then
			dic0.Add("Cycles", CycleFiles.Select(Function(sb) sb.PathOrDummy))
		End If

		'AA-TB
		'ADVANCED AUXILIARIES 
		dic0.Add("AuxiliaryAssembly", AuxiliaryAssembly)
		dic0.Add("AuxiliaryVersion", AuxiliaryVersion)
		dic0.Add("AdvancedAuxiliaryFilePath", AdvancedAuxiliaryFilePath)

		If AuxPaths.Any() Then
			dic0.Add("Aux", AuxPaths.Select(Function(kv) New Dictionary(Of String, Object) From {
												{"ID", Trim(UCase(kv.Key))},
												{"Type", kv.Value.Type},
												{"Path", kv.Value.Path.PathOrDummy},
												{"Technology", kv.Value.TechnologyList}
												}))
		End If

		dic0.Add("VACC", _driverAccelerationFile.PathOrDummy)
		dic0.Add("EngineOnlyMode", EngineOnly)
		dic0.Add("StartStop", New Dictionary(Of String, Object) From {
					{"Enabled", _startStop},
					{"MaxSpeed", _startStopMaxSpeed},
					{"MinTime", _startStopMinTime},
					{"Delay", StartStopDelay}})
		dic0.Add("LAC", New Dictionary(Of String, Object) From {
					{"Enabled", LookAheadOn},
					{"PreviewDistanceFactor", LacPreviewFactor},
					{"DF_offset", LacDfOffset},
					{"DF_scaling", LacDfScale},
					{"DF_targetSpeedLookup", LacDfTargetSpeedFile},
					{"Df_velocityDropLookup", LacDfVelocityDropFile}})

		'Overspeed / EcoRoll
		Dim overspeedDic = New Dictionary(Of String, Object)
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
		dic0.Add("OverSpeedEcoRoll", overspeedDic)

		json.Content.Add("Body", JToken.FromObject(dic0))
		Return json.WriteFile(_sFilePath)
	End Function

	Public Function ReadFile() As Boolean
		Const msgSrc = "Main/ReadInp/GEN"

		SetDefault()

		Dim json As New JSONParser
		If Not json.ReadFile(_sFilePath) Then Return False

		Try

			Dim fileVersion As Integer = json.Content.GetEx("Header").GetEx(Of Integer)("FileVersion")

			Dim body As JToken = json.Content.GetEx("Body")

			If fileVersion > 1 Then
				SavedInDeclMode = body.GetEx(Of Boolean)("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			If Not body("VehicleFile") Is Nothing Then _
				_vehicleFile.Init(_myPath, body.GetEx(Of String)("VehicleFile"))

			_engineFile.Init(_myPath, body.GetEx(Of String)("EngineFile"))

			If Not body("GearboxFile") Is Nothing Then _
				_gearboxFile.Init(_myPath, body.GetEx(Of String)("GearboxFile"))

			If Not body("Cycles") Is Nothing Then
				For Each entry As JToken In body.GetEx("Cycles")
					Dim subPath = New SubPath
					subPath.Init(_myPath, entry.Value(Of String))
					CycleFiles.Add(subPath)
				Next
			End If

			'AA-TB
			'ADVANCED AUXILIARIES 
			If Not body("AuxiliaryAssembly") Is Nothing AndAlso
				Not body("AuxiliaryVersion") Is Nothing Then

				AuxiliaryAssembly = body("AuxiliaryAssembly").ToString()
				AuxiliaryVersion = body("AuxiliaryVersion").ToString()

			End If
			If Not body("AdvancedAuxiliaryFilePath") Is Nothing Then
				AdvancedAuxiliaryFilePath = body("AdvancedAuxiliaryFilePath").ToString()
			End If


			If Not body("Aux") Is Nothing Then
				For Each dic As JToken In body.GetEx("Aux")

					Dim auxId As String = UCase(Trim(dic.GetEx(Of String)("ID")))

					If AuxPaths.ContainsKey(auxId) Then
						WorkerMsg(MessageType.Err, "Multiple definitions of the same auxiliary type (" & auxId & ")!", msgSrc)
						Return False
					End If

					Dim auxEntry = New AuxEntry

					auxEntry.Type = dic.GetEx(Of String)("Type")
					auxEntry.Path.Init(_myPath, dic.GetEx(Of String)("Path"))

					If Not dic("Technology") Is Nothing Then
						If fileVersion = 2 Then
							auxEntry.TechnologyList.Add(dic.GetEx(Of String)("Technology"))
						End If
						If fileVersion = 3 Then
							auxEntry.TechnologyList = dic.GetEx("Technology").ToObject(Of List(Of String))() '.FirstOrDefault()
						End If
					End If

					If (auxId = Constants.AuxiliaryKey.HVAC) Then
						If auxEntry.TechnologyList.Count > 0 Then ' Not String.IsNullOrWhiteSpace(auxEntry.TechStr) Then
							auxEntry.TechnologyList.Clear()
							WorkerMsg(MessageType.Normal, "Aux: Automatically Upgraded HVAC to new format.", msgSrc)
						End If
					End If

					If auxId = Constants.AuxiliaryKey.ElecSys Then
						If auxEntry.TechnologyList.Contains("Custom Technology List") OrElse auxEntry.TechnologyList.Count > 0 Then
							Dim hasTech = False

							If Not dic("TechList") Is Nothing Then
								For Each t In dic("TechList")
									hasTech = True
								Next
							End If

							auxEntry.TechnologyList.Clear()
							If Not hasTech Then
								auxEntry.TechnologyList.Add("Standard technology")
							Else
								auxEntry.TechnologyList.Add("Standard technology - LED headlights, all")
							End If
							WorkerMsg(MessageType.Normal,
									"Aux: Automatically Upgraded Electric System to new format: '" + auxEntry.TechnologyList.FirstOrDefault() + "'",
									msgSrc)
						End If
					End If

					If auxId = Constants.AuxiliaryKey.SteerPump Then
						If _
							auxEntry.TechnologyList.Contains("Variable displacement") OrElse
							auxEntry.TechnologyList.Contains("Hydraulic supported by electric") Then
							auxEntry.TechnologyList.Clear()
							WorkerMsg(MessageType.Warn, "Aux: Steering Pump Technology not automatically convertible. Please set new value.",
									msgSrc)
						End If
					End If

					If auxId = Constants.AuxiliaryKey.Fan Then
						If auxEntry.TechnologyList.Contains("Crankshaft mounted - Electronically controlled visco clutch (Default)") Then
							auxEntry.TechnologyList.Clear()
							auxEntry.TechnologyList.Add("Crankshaft mounted - Electronically controlled visco clutch")
						End If
						If auxEntry.TechnologyList.Contains("Crankshaft mounted - On/Off clutch") Then
							auxEntry.TechnologyList.Clear()
							auxEntry.TechnologyList.Add("Crankshaft mounted - On/off clutch")
						End If
						If auxEntry.TechnologyList.Contains("Belt driven or driven via transm. - On/Off clutch") Then
							auxEntry.TechnologyList.Clear()
							auxEntry.TechnologyList.Add("Belt driven or driven via transm. - On/off clutch")
						End If
					End If

					If fileVersion = 2 AndAlso auxId = Constants.AuxiliaryKey.PneumSys Then
						auxEntry.TechnologyList.Clear()
						WorkerMsg(MessageType.Warn, "Aux: Pneumatic System must be updated. Please set new value.",
								msgSrc)
					End If

					AuxPaths.Add(auxId, auxEntry)

				Next
			End If

			If Not body("VACC") Is Nothing Then
				_driverAccelerationFile.Init(_myPath, body.GetEx(Of String)("VACC"))
			End If

			EngineOnly = body.GetEx(Of Boolean)("EngineOnlyMode")

			If Not body("StartStop") Is Nothing Then
				Dim startStop As JToken = body.GetEx("StartStop")
				_startStop = startStop.GetEx(Of Boolean)("Enabled")
				_startStopMaxSpeed = startStop.GetEx(Of Double)("MaxSpeed")
				_startStopMinTime = startStop.GetEx(Of Double)("MinTime")
				StartStopDelay = startStop.GetEx(Of Double)("Delay")
			Else
				_startStop = False
			End If

			If Not body("LAC") Is Nothing Then
				Dim lac = body.GetEx("LAC")
				LookAheadOn = lac.GetEx(Of Boolean)("Enabled")
				LacPreviewFactor = If(lac("PreviewDistanceFactor") Is Nothing, 10, lac.GetEx(Of Double)("PreviewDistanceFactor"))
				LacDfOffset = If(lac("DF_offset") Is Nothing, 2.5, lac.GetEx(Of Double)("DF_offset"))
				LacDfScale = If(lac("DF_scaling") Is Nothing, 1.5, lac.GetEx(Of Double)("DF_scaling"))
				LacDfTargetSpeedFile =
					If(Not lac("DF_targetSpeedLookup") Is Nothing, lac.GetEx(Of String)("DF_targetSpeedLookup"), "")
				LacDfVelocityDropFile =
					If(Not lac("Df_velocityDropLookup") Is Nothing, lac.GetEx(Of String)("Df_velocityDropLookup"), "")
			Else
				LookAheadOn = False
			End If

			If Not body("OverSpeedEcoRoll") Is Nothing Then
				Dim dic = body("OverSpeedEcoRoll")

				Select Case UCase(dic("Mode").ToString).Trim
					Case "ECOROLL"
						OverSpeedOn = False
						EcoRollOn = True

					Case "OVERSPEED"
						OverSpeedOn = True
						EcoRollOn = False

					Case "OFF"
						OverSpeedOn = False
						EcoRollOn = False

					Case Else
						WorkerMsg(MessageType.Err, "Value '" & dic("Mode").ToString() & "' is not valid for OverSpeedEcoRoll/Mode!",
								msgSrc)
						Return False
				End Select

				VMin = dic.GetEx(Of Double)("MinSpeed")
				OverSpeed = dic.GetEx(Of Double)("OverSpeed")
				If Not dic("UnderSpeed") Is Nothing Then UnderSpeed = dic.GetEx(Of Double)("UnderSpeed")

			Else
				OverSpeedOn = False
				EcoRollOn = False
			End If


		Catch ex As Exception
			WorkerMsg(MessageType.Err, "Failed to read VECTO file! " & ex.Message, msgSrc)
			Return False
		End Try


		Return True
	End Function

	Private Sub SetDefault()

		AuxiliaryAssembly = "CLASSIC"
		AuxiliaryVersion = "CLASSIC"
		AdvancedAuxiliaryFilePath = String.Empty


		_startStop = False
		_startStopMaxSpeed = 5
		_startStopMinTime = 5
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


	Public Property StartStop As Boolean
		Get
			Return _startStop
		End Get
		Set(value As Boolean)
			_startStop = value
		End Set
	End Property

	Public Property StStV As Double
		Get
			Return _startStopMaxSpeed
		End Get
		Set(value As Double)
			_startStopMaxSpeed = value
		End Set
	End Property

	Public Property StStT As Double
		Get
			Return _startStopMinTime
		End Get
		Set(value As Double)
			_startStopMinTime = value
		End Set
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
End Class


