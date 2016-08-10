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
Imports System.Linq
Imports Newtonsoft.Json.Linq

Public Class cVECTO
	Private Const FormatVersion As Short = 3

	'AA-TB
	'STORES THE Type and version of the chosen or default Auxiliary Type ( Classic/Original or other )
	Public AuxiliaryAssembly As String
	Public AuxiliaryVersion As String
	Public AdvancedAuxiliaryFilePath As String

	Private _sFilePath As String
	Private _myPath As String

	'Input parameters
	Private ReadOnly _stPathVeh As cSubPath
	Private ReadOnly _stPathEng As cSubPath
	Private ReadOnly _stPathGbx As cSubPath

	Private _boStartStop As Boolean
	Private _siStStV As Single
	Private _siStStT As Single
	Public StStDelay As Integer

	Private ReadOnly _stDesMaxFile As cSubPath
	Private ReadOnly _laDesV As List(Of Single)
	Private ReadOnly _laDesMax As List(Of Single)
	Private ReadOnly _laDesMin As List(Of Single)
	Private _desMaxDim As Integer

	Public ReadOnly AuxPaths As Dictionary(Of String, AuxEntry)
	Public AuxRefs As Dictionary(Of String, cAux) _
	'Alle Nebenverbraucher die in der Veh-Datei UND im Zyklus definiert sind
	Public AuxDef As Boolean							   'True wenn ein oder mehrere Nebenverbraucher definiert sind

	Public ReadOnly CycleFiles As List(Of cSubPath)

	Public EngOnly As Boolean

	Public ALookahead As Single
	Public VMin As Single
	Public VMinLa As Single
	Public LookAheadOn As Boolean
	Public OverSpeedOn As Boolean
	Public OverSpeed As Single
	Public UnderSpeed As Single
	Public EcoRollOn As Boolean

	Private _myFileList As List(Of String)

	Public SavedInDeclMode As Boolean

	Public Class AuxEntry
		Public Type As String
		Public ReadOnly Path As cSubPath
		Public TechStr As String = ""

		Public Sub New()
			Path = New cSubPath
		End Sub
	End Class

	Public Function CreateFileList() As Boolean
		_myFileList = New List(Of String)

		'.vecto
		_myFileList.Add(_sFilePath)

		'Veh
		If Not EngOnly Then
			_myFileList.Add(PathVEH)

			If Not VEH.CreateFileList Then Return False

			_myFileList.AddRange(VEH.FileList)
		End If

		'Eng
		_myFileList.Add(PathENG)

		If Not ENG.CreateFileList Then Return False
		_myFileList.AddRange(ENG.FileList)

		If Not EngOnly Then
			'Gbx
			_myFileList.Add(PathGBX)

			If Not GBX.CreateFileList Then Return False
			_myFileList.AddRange(GBX.FileList)

			'Aux
			If AuxDef And Not Cfg.DeclMode Then
				_myFileList.AddRange(AuxPaths.Values.Select(Function(entry) entry.Path.FullPath))
			End If

			'.vacc
			_myFileList.Add(_stDesMaxFile.FullPath)

		End If

		'Cycles
		_myFileList.AddRange(CycleFiles.Select(Function(path) path.FullPath))

		Return True
	End Function

	Public Sub New()

		_myPath = ""
		_sFilePath = ""

		_stPathVeh = New cSubPath
		_stPathEng = New cSubPath
		_stPathGbx = New cSubPath

		_stDesMaxFile = New cSubPath

		_laDesV = New List(Of Single)
		_laDesMax = New List(Of Single)
		_laDesMin = New List(Of Single)

		AuxPaths = New Dictionary(Of String, AuxEntry)
		AuxRefs = New Dictionary(Of String, cAux)
		AuxDef = False

		CycleFiles = New List(Of cSubPath)
	End Sub

	Public Function SaveFile() As Boolean
		Dim json As New JSON

		'Header
		json.Content.Add("Header", New Dictionary(Of String, Object) From {
							{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
							{"Date", Now.ToUniversalTime().ToString("o")},
							{"AppVersion", VECTOvers},
							{"FileVersion", FormatVersion}})

		'Body
		Dim dic0 = New Dictionary(Of String, Object)

		dic0.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		'Main Files
		dic0.Add("VehicleFile", _stPathVeh.PathOrDummy)
		dic0.Add("EngineFile", _stPathEng.PathOrDummy)
		dic0.Add("GearboxFile", _stPathGbx.PathOrDummy)

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
												{"Technology", IIf(kv.Value.TechStr = "", New List(Of String), New List(Of String) From {kv.Value.TechStr})}
												}))
		End If

		dic0.Add("VACC", _stDesMaxFile.PathOrDummy)
		dic0.Add("EngineOnlyMode", EngOnly)
		dic0.Add("StartStop", New Dictionary(Of String, Object) From {
					{"Enabled", _boStartStop},
					{"MaxSpeed", _siStStV},
					{"MinTime", _siStStT},
					{"Delay", StStDelay}})
		dic0.Add("LAC", New Dictionary(Of String, Object) From {
					{"Enabled", LookAheadOn},
					{"Dec", ALookahead},
					{"MinSpeed", vMinLA},
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
		overspeedDic.Add("MinSpeed", vMin)
		overspeedDic.Add("OverSpeed", OverSpeed)
		overspeedDic.Add("UnderSpeed", UnderSpeed)
		dic0.Add("OverSpeedEcoRoll", overspeedDic)

		json.Content.Add("Body", dic0)
		Return json.WriteFile(_sFilePath)
	End Function

	Public Function ReadFile() As Boolean
		Const msgSrc = "Main/ReadInp/GEN"

		SetDefault()

		Dim json As New JSON
		If Not json.ReadFile(_sFilePath) Then Return False

		Try
			Dim fileVersion = json.Content("Header")("FileVersion")

			Dim body As JObject = json.Content("Body")

			If fileVersion > 1 Then
				SavedInDeclMode = body("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			If Not body("VehicleFile") Is Nothing Then _
				_stPathVeh.Init(_myPath, body("VehicleFile"))

			_stPathEng.Init(_myPath, body("EngineFile"))

			If Not body("GearboxFile") Is Nothing Then _
				_stPathGbx.Init(_myPath, body("GearboxFile"))

			If Not body("Cycles") Is Nothing Then
				For Each str As String In body("Cycles")
					Dim subPath = New cSubPath
					subPath.Init(_myPath, str)
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
				For Each dic In body("Aux")

					Dim auxId As String = UCase(Trim(dic("ID").ToString))

					If AuxPaths.ContainsKey(auxId) Then
						WorkerMsg(tMsgID.Err, "Multiple definitions of the same auxiliary type (" & auxId & ")!", msgSrc)
						Return False
					End If

					Dim auxEntry = New AuxEntry

					auxEntry.Type = dic("Type")
					auxEntry.Path.Init(_myPath, dic("Path"))

					If Not dic("Technology") Is Nothing Then
						If fileVersion = 2 Then
							auxEntry.TechStr = dic("Technology")
						End If
						If fileVersion = 3 Then
							auxEntry.TechStr = dic("Technology").FirstOrDefault()
						End If
					End If

					If (auxId = sKey.AUX.HVAC) Then
						If Not String.IsNullOrWhiteSpace(auxEntry.TechStr) Then
							auxEntry.TechStr = ""
							WorkerMsg(tMsgID.Normal, "Aux: Automatically Upgraded HVAC to new format.", msgSrc)
						End If
					End If

					If auxId = sKey.AUX.ElecSys Then
						If auxEntry.TechStr = "Custom Technology List" OrElse String.IsNullOrWhiteSpace(auxEntry.TechStr) Then
							Dim hasTech = False

							If Not dic("TechList") Is Nothing Then
								For Each t In dic("TechList")
									hasTech = True
								Next
							End If

							If Not hasTech Then
								auxEntry.TechStr = "Standard technology"
							Else
								auxEntry.TechStr = "Standard technology - LED headlights, all"
							End If
							WorkerMsg(tMsgID.Normal, "Aux: Automatically Upgraded Electric System to new format: '" + auxEntry.TechStr + "'",
									msgSrc)
						End If
					End If

					If auxId = sKey.AUX.SteerPump Then
						Select Case auxEntry.TechStr
							Case "Variable displacement"
								WorkerMsg(tMsgID.Warn, "Aux: Steering Pump Technology not automatically convertible. Please set new value.",
										msgSrc)
							Case "Hydraulic supported by electric"
								WorkerMsg(tMsgID.Warn, "Aux: Steering Pump Technology not automatically convertible. Please set new value.",
										msgSrc)
						End Select
					End If

					If auxId = sKey.AUX.Fan Then
						Select Case auxEntry.TechStr
							Case "Crankshaft mounted - Electronically controlled visco clutch (Default)"
								auxEntry.TechStr = "Crankshaft mounted - Electronically controlled visco clutch"
							Case "Crankshaft mounted - On/Off clutch"
								auxEntry.TechStr = "Crankshaft mounted - On/off clutch"
							Case "Belt driven or driven via transm. - On/Off clutch"
								auxEntry.TechStr = "Belt driven or driven via transm. - On/off clutch"
						End Select
					End If

					If fileVersion = 2 AndAlso auxId = sKey.AUX.PneumSys Then
						auxEntry.TechStr = ""
						WorkerMsg(tMsgID.Warn, "Aux: Pneumatic System must be updated. Please set new value.",
								msgSrc)
					End If

					AuxPaths.Add(auxId, auxEntry)
					AuxDef = True
				Next
			End If

			If Not body("VACC") Is Nothing Then
				_stDesMaxFile.Init(_myPath, body("VACC"))
			End If

			EngOnly = body("EngineOnlyMode")

			If Not body("StartStop") Is Nothing Then
				Dim dic = body("StartStop")
				_boStartStop = dic("Enabled")
				_siStStV = dic("MaxSpeed")
				_siStStT = dic("MinTime")
				StStDelay = dic("Delay")
			Else
				_boStartStop = False
			End If

			If Not body("LAC") Is Nothing Then
				Dim dic = body("LAC")
				LookAheadOn = dic("Enabled")
				ALookahead = dic("Dec")
				vMinLA = dic("MinSpeed")
				LacPreviewFactor = If(dic("PreviewDistanceFactor") Is Nothing, 10, dic("PreviewDistanceFactor"))
				LacDfOffset = If(dic("DF_offset") Is Nothing, 2.5, dic("DF_offset"))
				LacDfScale = If(dic("DF_scaling") Is Nothing, 1.5, dic("DF_scaling"))
				LacDfTargetSpeedFile = If(Not dic("DF_targetSpeedLookup") Is Nothing, dic("DF_targetSpeedLookup"), "")
				LacDfVelocityDropFile = If(Not dic("Df_velocityDropLookup") Is Nothing, dic("Df_velocityDropLookup"), "")
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
						WorkerMsg(tMsgID.Err, "Value '" & dic("Mode").ToString() & "' is not valid for OverSpeedEcoRoll/Mode!", msgSrc)
						Return False
				End Select

				vMin = dic("MinSpeed")
				OverSpeed = dic("OverSpeed")
				If Not dic("UnderSpeed") Is Nothing Then UnderSpeed = dic("UnderSpeed")

			Else
				OverSpeedOn = False
				EcoRollOn = False
			End If


		Catch ex As Exception
			WorkerMsg(tMsgID.Err, "Failed to read VECTO file! " & ex.Message, msgSrc)
			Return False
		End Try


		Return True
	End Function

	Private Sub SetDefault()

		AuxiliaryAssembly = "CLASSIC"
		AuxiliaryVersion = "CLASSIC"
		AdvancedAuxiliaryFilePath = String.Empty


		_boStartStop = False
		_siStStV = 5
		_siStStT = 5
		StStDelay = 0

		_stPathVeh.Clear()
		_stPathEng.Clear()
		CycleFiles.Clear()
		_stPathGbx.Clear()

		_stDesMaxFile.Clear()
		_laDesV.Clear()
		_laDesMax.Clear()
		_laDesMin.Clear()
		_desMaxDim = -1

		AuxPaths.Clear()
		AuxRefs.Clear()
		AuxDef = False
		EngOnly = False

		ALookahead = 0
		VMin = 0
		LookAheadOn = True
		OverSpeedOn = False
		EcoRollOn = False
		OverSpeed = 0
		UnderSpeed = 0
		VMinLa = 0

		SavedInDeclMode = False
	End Sub

	Public Function DeclInit() As Boolean
		EngOnly = False

		CycleFiles.Clear()

		Dim cl = Declaration.SegRef.GetCycles

		For Each s In cl
			Dim subPath = New cSubPath
			subPath.Init(_myPath, s)
			CycleFiles.Add(subPath)
		Next

		_stDesMaxFile.Init(_myPath, Declaration.SegRef.VACCfile)

		_siStStV = cDeclaration.SSspeed
		_siStStT = cDeclaration.SStime
		StStDelay = cDeclaration.SSdelay

		If Not EcoRollOn Then OverSpeedOn = True

		OverSpeed = cDeclaration.Overspeed
		UnderSpeed = cDeclaration.Underspeed
		VMin = cDeclaration.ECvmin

		LookAheadOn = True
		ALookahead = cDeclaration.LACa
		VMinLa = cDeclaration.LACvmin

		'No need to check Aux (AuxDef). Will be checked in cDeclaration.CalcInitLoad

		Return True
	End Function

	'This Sub reads those Input-files that do not have their own class, etc.
	Public Function Init() As Boolean
		Dim file As cFile_V3
		Dim line As String()

		Dim msgSrc = "VECTO/Init"

		If Not EngOnly Then

			file = New cFile_V3

			If Not file.OpenRead(_stDesMaxFile.FullPath) Then
				WorkerMsg(tMsgID.Err, "Can't read .vacc file (" & _stDesMaxFile.FullPath & ")", msgSrc)
				Return False
			End If

			'Skip Header
			file.ReadLine()

			_laDesV.Clear()
			_laDesMax.Clear()
			_laDesMin.Clear()
			_desMaxDim = -1
			Try

				Do While Not file.EndOfFile

					_desMaxDim += 1

					line = file.ReadLine

					_laDesV.Add(CSng(line(0)) / 3.6)																																  'km/h => m/s !!!!
					_laDesMax.Add(CSng(line(1)))
					_laDesMin.Add(CSng(line(2)))

				Loop

			Catch ex As Exception

				file.Close()
				WorkerMsg(tMsgID.Err, "Error in .vacc file. " & ex.Message & " (" & _stDesMaxFile.FullPath & ")", msgSrc,
						_stDesMaxFile.FullPath)
				Return False

			End Try

			file.Close()

		End If

		Return True
	End Function

#Region "Aux"

	Public Function AuxInit() As Boolean
		Dim msgSrc = "VEH/AuxInit"
		AuxRefs = New Dictionary(Of String, cAux)

		If Cfg.DeclMode Then
			For Each auxPathKv In AuxPaths
				AuxRefs.Add(auxPathKv.Key, Nothing)
			Next
			Return True
		End If

		If DRI.AuxDef Xor AuxDef Then
			If AuxDef Then
				WorkerMsg(tMsgID.Err, "No auxiliary input defined in driving cycle!", msgSrc)
				Return False
			Else
				WorkerMsg(tMsgID.Warn, "No auxiliary defined in vehicle file! Psupply input will be ignored!", msgSrc)
				Return True
			End If
		End If

		If Not (DRI.AuxDef Or AuxDef) Then
			Return True
		End If

		Dim drIauxcheck = DRI.AuxComponents.Keys.ToDictionary(Function(auxId) auxId, Function(auxId) False)

		For Each auxPathKv In AuxPaths
			msgSrc = "VEH/AuxInit/" & auxPathKv.Key
			If Not DRI.AuxComponents.ContainsKey(auxPathKv.Key) Then
				WorkerMsg(tMsgID.Err, "No Psupply input defined in driving cycle for auxiliary '" & auxPathKv.Key & "'!", msgSrc)
				Return False
			End If

			Dim aux0 = New cAux
			aux0.Filepath = auxPathKv.Value.Path.FullPath

			If Not aux0.Readfile Then
				Return False
			End If

			AuxRefs.Add(auxPathKv.Key, aux0)
			drIauxcheck(auxPathKv.Key) = True
		Next

		msgSrc = "VEH/AuxInit"

		For Each auxId In DRI.AuxComponents.Keys
			If Not drIauxcheck(auxId) Then
				WorkerMsg(tMsgID.Warn, "Auxiliary '" & auxId & "' not found! Psupply input will be ignored!", msgSrc)
			End If
		Next

		Return True
	End Function

	Public Function Paux(auxId As String, t As Integer, nU As Single) As Single
		If Cfg.DeclMode Then Return Declaration.AuxPower(auxId)

		If AuxDef Then
			Dim aux0 = AuxRefs(auxId)
			Dim psupply As Single = DRI.AuxComponents(auxId)(t)
			If psupply < 0 Then
				GoTo lbAuxError
			End If

			Dim px As Single = aux0.Paux(nU, psupply)
			If px < 0 Then
				GoTo lbAuxError
			End If
			Return px
		Else
			Return 0
		End If

lbAuxError:
		MODdata.ModErrors.AuxNegative = auxId
		Return 0
	End Function

	Public Function PauxSum(t As Integer, nU As Single) As Single
		If AuxDef Then
			Return AuxRefs.Keys.Sum(Function(auxId) Paux(auxId, t, nU))
		End If
		Return 0
	End Function

#End Region


#Region "Properties"

	Public ReadOnly Property FileList As List(Of String)
		Get
			Return _myFileList
		End Get
	End Property

	Public Property FilePath As String
		Get
			Return _sFilePath
		End Get
		Set(value As String)
			_sFilePath = value
			If _sFilePath = "" Then
				_myPath = ""
			Else
				_myPath = IO.Path.GetDirectoryName(_sFilePath) & "\"
			End If
		End Set
	End Property


	Public Property PathVeh(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _stPathVeh.OriginalPath
			Else
				Return _stPathVeh.FullPath
			End If
		End Get
		Set(value As String)
			_stPathVeh.Init(_myPath, value)
		End Set
	End Property

	Public Property PathEng(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _stPathEng.OriginalPath
			Else
				Return _stPathEng.FullPath
			End If
		End Get
		Set(value As String)
			_stPathEng.Init(_myPath, value)
		End Set
	End Property

	Public Property PathGbx(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _stPathGbx.OriginalPath
			Else
				Return _stPathGbx.FullPath
			End If
		End Get
		Set(value As String)
			_stPathGbx.Init(_myPath, value)
		End Set
	End Property


	Public Property StartStop As Boolean
		Get
			Return _boStartStop
		End Get
		Set(value As Boolean)
			_boStartStop = value
		End Set
	End Property

	Public Property StStV As Single
		Get
			Return _siStStV
		End Get
		Set(value As Single)
			_siStStV = value
		End Set
	End Property

	Public Property StStT As Single
		Get
			Return _siStStT
		End Get
		Set(value As Single)
			_siStStT = value
		End Set
	End Property

	Public Property DesMaxFile(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _stDesMaxFile.OriginalPath
			Else
				Return _stDesMaxFile.FullPath
			End If
		End Get
		Set(value As String)
			_stDesMaxFile.Init(_myPath, value)
		End Set
	End Property

	Public Property LacPreviewFactor As Single
	Public Property LacDfOffset As Single
	Public Property LacDfScale As Single
	Public Property LacDfTargetSpeedFile As String
	Public Property LacDfVelocityDropFile As String


#End Region

	Public Function ADesMax(v As Single) As Single
		Dim i As Int32

		'Extrapolation for x < x(1)
		If _laDesV(0) >= v Then
			If _laDesV(0) > v Then MODdata.ModErrors.DesMaxExtr = "v= " & v * 3.6 & "[km/h]"
			i = 1
			GoTo lbInt
		End If

		i = 0
		Do While _laDesV(i) < v And i < _desMaxDim
			i += 1
		Loop

		'Extrapolation for x > x(imax)
		If _laDesV(i) < v Then
			MODdata.ModErrors.DesMaxExtr = "v= " & v * 3.6 & "[km/h]"
		End If

lbInt:
		'Interpolation
		Return (v - _laDesV(i - 1)) * (_laDesMax(i) - _laDesMax(i - 1)) / (_laDesV(i) - _laDesV(i - 1)) + _laDesMax(i - 1)
	End Function

	Public Function ADesMin(v As Single) As Single
		Dim i As Int32

		'Extrapolation for x < x(1)
		If _laDesV(0) >= v Then
			If _laDesV(0) > v Then MODdata.ModErrors.DesMaxExtr = "v= " & v * 3.6 & "[km/h]"
			i = 1
			GoTo lbInt
		End If

		i = 0
		Do While _laDesV(i) < v And i < _desMaxDim
			i += 1
		Loop

		'Extrapolation for x > x(imax)
		If _laDesV(i) < v Then
			MODdata.ModErrors.DesMaxExtr = "v= " & v * 3.6 & "[km/h]"
		End If

lbInt:
		'Interpolation
		Return (v - _laDesV(i - 1)) * (_laDesMin(i) - _laDesMin(i - 1)) / (_laDesV(i) - _laDesV(i - 1)) + _laDesMin(i - 1)
	End Function
End Class


