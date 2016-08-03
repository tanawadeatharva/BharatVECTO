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

Imports System.Collections.Generic
Imports System.Linq

Public Class cVECTO
	Private Const FormatVersion As Short = 3
	Private FileVersion As Short

	'AA-TB
	'STORES THE Type and version of the chosen or default Auxiliary Type ( Classic/Original or other )
	Public AuxiliaryAssembly As String
	Public AuxiliaryVersion As String
	Public AdvancedAuxiliaryFilePath As String


	Private sFilePath As String

	Private MyPath As String

	'Input parameters
	Private stPathVEH As cSubPath
	Private stPathENG As cSubPath
	Private stPathGBX As cSubPath

	Private boStartStop As Boolean
	Private siStStV As Single
	Private siStStT As Single
	Public StStDelay As Integer

	Private stDesMaxFile As cSubPath
	Private laDesV As List(Of Single)
	Private laDesMax As List(Of Single)
	Private laDesMin As List(Of Single)
	Private DesMaxDim As Integer

	Public AuxPaths As Dictionary(Of String, cAuxEntry)
	Public AuxRefs As Dictionary(Of String, cAux) _
	'Alle Nebenverbraucher die in der Veh-Datei UND im Zyklus definiert sind
	Public AuxDef As Boolean							   'True wenn ein oder mehrere Nebenverbraucher definiert sind

	Public CycleFiles As List(Of cSubPath)

	Public EngOnly As Boolean

	Public a_lookahead As Single
	Public vMin As Single
	Public vMinLA As Single
	Public LookAheadOn As Boolean
	Public OverSpeedOn As Boolean
	Public OverSpeed As Single
	Public UnderSpeed As Single
	Public EcoRollOn As Boolean

	Private MyFileList As List(Of String)

	Public SavedInDeclMode As Boolean


	Public Class cAuxEntry
		Public Type As String
		Public ReadOnly Path As cSubPath
		Public TechStr As String = ""

		Public Sub New()
			Path = New cSubPath
		End Sub
	End Class

	Public Function CreateFileList() As Boolean
		Dim Aux0 As cAuxEntry
		Dim sb As cSubPath
		Dim str As String

		MyFileList = New List(Of String)

		'.vecto
		MyFileList.Add(sFilePath)

		'Veh
		If Not EngOnly Then
			MyFileList.Add(PathVEH)

			If Not VEH.CreateFileList Then Return False
			For Each str In VEH.FileList
				MyFileList.Add(str)
			Next
		End If

		'Eng
		MyFileList.Add(PathENG)

		If Not ENG.CreateFileList Then Return False
		For Each str In ENG.FileList
			MyFileList.Add(str)
		Next

		If Not EngOnly Then

			'Gbx
			MyFileList.Add(PathGBX)

			If Not GBX.CreateFileList Then Return False
			For Each str In GBX.FileList
				MyFileList.Add(str)
			Next

			'Aux
			If AuxDef And Not Cfg.DeclMode Then
				For Each Aux0 In AuxPaths.Values
					MyFileList.Add(Aux0.Path.FullPath)
				Next
			End If

			'.vacc
			MyFileList.Add(stDesMaxFile.FullPath)

		End If

		'Cycles
		For Each sb In CycleFiles
			MyFileList.Add(sb.FullPath)
		Next


		Return True
	End Function

	Public Sub New()

		MyPath = ""
		sFilePath = ""

		stPathVEH = New cSubPath
		stPathENG = New cSubPath
		stPathGBX = New cSubPath

		stDesMaxFile = New cSubPath

		laDesV = New List(Of Single)
		laDesMax = New List(Of Single)
		laDesMin = New List(Of Single)

		AuxPaths = New Dictionary(Of String, cAuxEntry)
		AuxRefs = New Dictionary(Of String, cAux)
		AuxDef = False

		CycleFiles = New List(Of cSubPath)
	End Sub

	Public Function SaveFile() As Boolean
		'Header
		Dim dic = New Dictionary(Of String, Object)
		dic.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		dic.Add("Date", Now.ToString)
		dic.Add("AppVersion", VECTOvers)
		dic.Add("FileVersion", FormatVersion)

		Dim JSON As New JSON
		JSON.Content.Add("Header", dic)

		'Body
		Dim dic0 = New Dictionary(Of String, Object)

		dic0.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		'Main Files
		dic0.Add("VehicleFile", stPathVEH.PathOrDummy)
		dic0.Add("EngineFile", stPathENG.PathOrDummy)
		dic0.Add("GearboxFile", stPathGBX.PathOrDummy)

		'Cycles
		If CycleFiles.Count > 0 Then
			Dim ls = New List(Of Object)
			For Each sb In CycleFiles
				ls.Add(sb.PathOrDummy)
			Next
			dic0.Add("Cycles", ls)
		End If

		'AA-TB
		'ADVANCED AUXILIARIES 
		dic0.Add("AuxiliaryAssembly", AuxiliaryAssembly)
		dic0.Add("AuxiliaryVersion", AuxiliaryVersion)
		dic0.Add("AdvancedAuxiliaryFilePath", AdvancedAuxiliaryFilePath)

		'Aux
		If AuxPaths.Count > 0 Then
			Dim ls = New List(Of Object)
			For Each AuxEntryKV In AuxPaths
				dic = New Dictionary(Of String, Object)
				dic.Add("ID", Trim(UCase(AuxEntryKV.Key)))
				dic.Add("Type", AuxEntryKV.Value.Type)
				dic.Add("Path", AuxEntryKV.Value.Path.PathOrDummy)
				dic.Add("Technology", AuxEntryKV.Value.TechStr)
				ls.Add(dic)
			Next
			dic0.Add("Aux", ls)
		End If

		'VACC
		dic0.Add("VACC", stDesMaxFile.PathOrDummy)

		'EngineOnlyMode
		dic0.Add("EngineOnlyMode", EngOnly)

		'Start Stop
		dic = New Dictionary(Of String, Object)
		dic.Add("Enabled", boStartStop)
		dic.Add("MaxSpeed", siStStV)
		dic.Add("MinTime", siStStT)
		dic.Add("Delay", StStDelay)
		dic0.Add("StartStop", dic)

		'LAC
		dic = New Dictionary(Of String, Object)
		dic.Add("Enabled", LookAheadOn)
		dic.Add("Dec", a_lookahead)
		dic.Add("MinSpeed", vMinLA)
		dic.Add("PreviewDistanceFactor", LacPreviewFactor)
		dic.Add("DF_offset", LacDfOffset)
		dic.Add("DF_scaling", LacDfScale)
		dic.Add("DF_targetSpeedLookup", LacDfTargetSpeedFile)
		dic.Add("Df_velocityDropLookup", LacDfVelocityDropFile)

		dic0.Add("LAC", dic)

		'Overspeed / EcoRoll
		dic = New Dictionary(Of String, Object)
		If EcoRollOn Then
			dic.Add("Mode", "EcoRoll")
		ElseIf OverSpeedOn Then
			dic.Add("Mode", "OverSpeed")
		Else
			dic.Add("Mode", "Off")
		End If
		dic.Add("MinSpeed", vMin)
		dic.Add("OverSpeed", OverSpeed)
		dic.Add("UnderSpeed", UnderSpeed)
		dic0.Add("OverSpeedEcoRoll", dic)

		JSON.Content.Add("Body", dic0)
		Return JSON.WriteFile(sFilePath)
	End Function

	Public Function ReadFile() As Boolean
		Dim msgSrc = "Main/ReadInp/GEN"

		SetDefault()

		Dim JSON As New JSON
		If Not JSON.ReadFile(sFilePath) Then Return False

		Try

			FileVersion = JSON.Content("Header")("FileVersion")

			If FileVersion > 1 Then
				SavedInDeclMode = JSON.Content("Body")("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			If Not JSON.Content("Body")("VehicleFile") Is Nothing Then _
				stPathVEH.Init(MyPath, JSON.Content("Body")("VehicleFile"))

			stPathENG.Init(MyPath, JSON.Content("Body")("EngineFile"))

			If Not JSON.Content("Body")("GearboxFile") Is Nothing Then _
				stPathGBX.Init(MyPath, JSON.Content("Body")("GearboxFile"))

			If Not JSON.Content("Body")("Cycles") Is Nothing Then
				For Each str As String In JSON.Content("Body")("Cycles")
					Dim subPath = New cSubPath
					subPath.Init(MyPath, str)
					CycleFiles.Add(subPath)
				Next
			End If

			'AA-TB
			'ADVANCED AUXILIARIES 
			If Not JSON.Content("Body")("AuxiliaryAssembly") Is Nothing AndAlso
				Not JSON.Content("Body")("AuxiliaryVersion") Is Nothing Then

				AuxiliaryAssembly = JSON.Content("Body")("AuxiliaryAssembly").ToString()
				AuxiliaryVersion = JSON.Content("Body")("AuxiliaryVersion").ToString()

			End If
			If Not JSON.Content("Body")("AdvancedAuxiliaryFilePath") Is Nothing Then
				AdvancedAuxiliaryFilePath = JSON.Content("Body")("AdvancedAuxiliaryFilePath").ToString()
			End If


			If Not JSON.Content("Body")("Aux") Is Nothing Then
				For Each dic In JSON.Content("Body")("Aux")

					Dim auxId As String = UCase(Trim(dic("ID").ToString))

					If AuxPaths.ContainsKey(auxId) Then
						WorkerMsg(tMsgID.Err, "Multiple definitions of the same auxiliary type (" & auxId & ")!", msgSrc)
						Return False
					End If

					Dim auxEntry = New cAuxEntry

					auxEntry.Type = dic("Type")
					auxEntry.Path.Init(MyPath, dic("Path"))

					If Not dic("Technology") Is Nothing Then auxEntry.TechStr = dic("Technology")

					AuxPaths.Add(auxId, auxEntry)

					AuxDef = True

					If auxId = sKey.AUX.ElecSys Then
						If auxEntry.TechStr = "Custom Technology List" Then
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
							WorkerMsg(tMsgID.Warn, "Aux: Upgraded Electric System to new format: '" + auxEntry.TechStr + "'", msgSrc)
						End If
					End If

					If auxId = sKey.AUX.SteerPump Then
						Select Case auxEntry.TechStr
							Case "Variable displacement"
								auxEntry.TechStr = "Variable displacement elec. controlled"
								WorkerMsg(tMsgID.Warn,
										"Aux: Upgraded Steering Pump Technology from 'Variable displacement' to '" + auxEntry.TechStr + "'", msgSrc)
							Case "Hydraulic supported by electric"
								auxEntry.TechStr = "Dual displacement"
								WorkerMsg(tMsgID.Warn,
										"Aux: Upgraded Steering Pump Technology from 'Hydraulic supported by electric' to '" + auxEntry.TechStr + "'",
										msgSrc)
						End Select
					End If
				Next
			End If

			If Not JSON.Content("Body")("VACC") Is Nothing Then
				stDesMaxFile.Init(MyPath, JSON.Content("Body")("VACC"))
			End If

			EngOnly = JSON.Content("Body")("EngineOnlyMode")

			If Not JSON.Content("Body")("StartStop") Is Nothing Then
				Dim dic = JSON.Content("Body")("StartStop")
				boStartStop = dic("Enabled")
				siStStV = dic("MaxSpeed")
				siStStT = dic("MinTime")
				StStDelay = dic("Delay")
			Else
				boStartStop = False
			End If

			If Not JSON.Content("Body")("LAC") Is Nothing Then
				Dim dic = JSON.Content("Body")("LAC")
				LookAheadOn = dic("Enabled")
				a_lookahead = dic("Dec")
				vMinLA = dic("MinSpeed")
				LacPreviewFactor = If(dic("PreviewDistanceFactor") Is Nothing, 10, dic("PreviewDistanceFactor"))
				LacDfOffset = If(dic("DF_offset") Is Nothing, 2.5, dic("DF_offset"))
				LacDfScale = If(dic("DF_scaling") Is Nothing, 1.5, dic("DF_scaling"))
				LacDfTargetSpeedFile = If(Not dic("DF_targetSpeedLookup") Is Nothing, dic("DF_targetSpeedLookup"), "")
				LacDfVelocityDropFile = If(Not dic("Df_velocityDropLookup") Is Nothing, dic("Df_velocityDropLookup"), "")
			Else
				LookAheadOn = False
			End If

			If Not JSON.Content("Body")("OverSpeedEcoRoll") Is Nothing Then
				Dim dic = JSON.Content("Body")("OverSpeedEcoRoll")

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
						WorkerMsg(tMsgID.Err, "Value '" & dic("Mode") & "' is not valid for OverSpeedEcoRoll/Mode!", msgSrc)
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


		boStartStop = False
		siStStV = 5
		siStStT = 5
		StStDelay = 0
		FileVersion = 0

		stPathVEH.Clear()
		stPathENG.Clear()
		CycleFiles.Clear()
		stPathGBX.Clear()

		stDesMaxFile.Clear()
		laDesV.Clear()
		laDesMax.Clear()
		laDesMin.Clear()
		DesMaxDim = - 1

		AuxPaths.Clear()
		AuxRefs.Clear()
		AuxDef = False
		EngOnly = False

		a_lookahead = 0
		vMin = 0
		LookAheadOn = True
		OverSpeedOn = False
		EcoRollOn = False
		OverSpeed = 0
		UnderSpeed = 0
		vMinLA = 0

		SavedInDeclMode = False
	End Sub

	Public Function DeclInit() As Boolean
		EngOnly = False

		CycleFiles.Clear()

		Dim cl = Declaration.SegRef.GetCycles

		For Each s In cl
			Dim subPath = New cSubPath
			subPath.Init(MyPath, s)
			CycleFiles.Add(subPath)
		Next

		stDesMaxFile.Init(MyPath, Declaration.SegRef.VACCfile)

		siStStV = cDeclaration.SSspeed
		siStStT = cDeclaration.SStime
		StStDelay = cDeclaration.SSdelay

		If Not EcoRollOn Then OverSpeedOn = True

		OverSpeed = cDeclaration.Overspeed
		UnderSpeed = cDeclaration.Underspeed
		vMin = cDeclaration.ECvmin

		LookAheadOn = True
		a_lookahead = cDeclaration.LACa
		vMinLA = cDeclaration.LACvmin

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

			If Not file.OpenRead(stDesMaxFile.FullPath) Then
				WorkerMsg(tMsgID.Err, "Can't read .vacc file (" & stDesMaxFile.FullPath & ")", msgSrc)
				Return False
			End If

			'Skip Header
			file.ReadLine()

			laDesV.Clear()
			laDesMax.Clear()
			laDesMin.Clear()
			DesMaxDim = - 1
			Try

				Do While Not file.EndOfFile

					DesMaxDim += 1

					line = file.ReadLine

					laDesV.Add(CSng(line(0))/3.6)																															'km/h => m/s !!!!
					laDesMax.Add(CSng(line(1)))
					laDesMin.Add(CSng(line(2)))

				Loop

			Catch ex As Exception

				file.Close()
				WorkerMsg(tMsgID.Err, "Error in .vacc file. " & ex.Message & " (" & stDesMaxFile.FullPath & ")", msgSrc,
						stDesMaxFile.FullPath)
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
			Return MyFileList
		End Get
	End Property

	Public Property FilePath As String
		Get
			Return sFilePath
		End Get
		Set(value As String)
			sFilePath = value
			If sFilePath = "" Then
				MyPath = ""
			Else
				MyPath = IO.Path.GetDirectoryName(sFilePath) & "\"
			End If
		End Set
	End Property


	Public Property PathVEH(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return stPathVEH.OriginalPath
			Else
				Return stPathVEH.FullPath
			End If
		End Get
		Set(value As String)
			stPathVEH.Init(MyPath, value)
		End Set
	End Property

	Public Property PathENG(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return stPathENG.OriginalPath
			Else
				Return stPathENG.FullPath
			End If
		End Get
		Set(value As String)
			stPathENG.Init(MyPath, value)
		End Set
	End Property

	Public Property PathGBX(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return stPathGBX.OriginalPath
			Else
				Return stPathGBX.FullPath
			End If
		End Get
		Set(value As String)
			stPathGBX.Init(MyPath, value)
		End Set
	End Property


	Public Property StartStop As Boolean
		Get
			Return boStartStop
		End Get
		Set(value As Boolean)
			boStartStop = value
		End Set
	End Property

	Public Property StStV As Single
		Get
			Return siStStV
		End Get
		Set(value As Single)
			siStStV = value
		End Set
	End Property

	Public Property StStT As Single
		Get
			Return siStStT
		End Get
		Set(value As Single)
			siStStT = value
		End Set
	End Property

	Public Property DesMaxFile(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return stDesMaxFile.OriginalPath
			Else
				Return stDesMaxFile.FullPath
			End If
		End Get
		Set(value As String)
			stDesMaxFile.Init(MyPath, value)
		End Set
	End Property

	Public Property LacPreviewFactor As Single
	Public Property LacDfOffset As Single
	Public Property LacDfScale As Single
	Public Property LacDfTargetSpeedFile As String
	Public Property LacDfVelocityDropFile As String


#End Region

	Public Function aDesMax(v As Single) As Single
		Dim i As Int32

		'Extrapolation for x < x(1)
		If laDesV(0) >= v Then
			If laDesV(0) > v Then MODdata.ModErrors.DesMaxExtr = "v= " & v*3.6 & "[km/h]"
			i = 1
			GoTo lbInt
		End If

		i = 0
		Do While laDesV(i) < v And i < DesMaxDim
			i += 1
		Loop

		'Extrapolation for x > x(imax)
		If laDesV(i) < v Then
			MODdata.ModErrors.DesMaxExtr = "v= " & v*3.6 & "[km/h]"
		End If

		lbInt:
		'Interpolation
		Return (v - laDesV(i - 1))*(laDesMax(i) - laDesMax(i - 1))/(laDesV(i) - laDesV(i - 1)) + laDesMax(i - 1)
	End Function

	Public Function aDesMin(v As Single) As Single
		Dim i As Int32

		'Extrapolation for x < x(1)
		If laDesV(0) >= v Then
			If laDesV(0) > v Then MODdata.ModErrors.DesMaxExtr = "v= " & v*3.6 & "[km/h]"
			i = 1
			GoTo lbInt
		End If

		i = 0
		Do While laDesV(i) < v And i < DesMaxDim
			i += 1
		Loop

		'Extrapolation for x > x(imax)
		If laDesV(i) < v Then
			MODdata.ModErrors.DesMaxExtr = "v= " & v*3.6 & "[km/h]"
		End If

		lbInt:
		'Interpolation
		Return (v - laDesV(i - 1))*(laDesMin(i) - laDesMin(i - 1))/(laDesV(i) - laDesV(i - 1)) + laDesMin(i - 1)
	End Function
End Class


