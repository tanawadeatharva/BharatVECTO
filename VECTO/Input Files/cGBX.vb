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
Imports System.Collections.Generic

Public Class cGBX
	Private Const FormatVersion As Short = 6
	Private FileVersion As Short

	Private MyPath As String
	Private sFilePath As String

	Public ModelName As String
	Public GbxInertia As Single
	Public TracIntrSi As Single

	Public Igetr As List(Of Single)
	Public GetrMaps As List(Of cSubPath)
	'Public IsTCgear As List(Of Boolean)

	Private MyGBmaps As List(Of cDelaunayMap)
	Private GetrEffDef As List(Of Boolean)
	Private GetrEff As List(Of Single)

	'Gear shift polygons
	Public gs_files As List(Of cSubPath)
	Public Shiftpolygons As List(Of cShiftPolygon)
	Public MaxTorque As List(Of String)

	Public gs_TorqueResv As Single
	Public gs_SkipGears As Boolean
	Public gs_ShiftTime As Integer
	Public gs_TorqueResvStart As Single
	Public gs_StartSpeed As Single
	Public gs_StartAcc As Single
	Public gs_ShiftInside As Boolean

	Public gs_Type As tGearbox

	'Torque Converter Input
	Public TCon As Boolean
	Public TCrefrpm As Single
	Private TC_file As New cSubPath
	Public TCinertia As Single


	Private TCnu As New List(Of Single)
	Private TCmu As New List(Of Single)
	Private TCtorque As New List(Of Single)
	Private TCdim As Integer


	'Torque Converter Iteration Results
	Public TCMin As Single
	Public TCnUin As Single
	Public TC_PeBrake As Single
	Public TCMout As Single
	Public TCnUout As Single
	Public TCReduce As Boolean
	Public TCNeutral As Boolean
	Public TC_mu As Single
	Public TC_nu As Single
	Private TCnuMax As Single


	Private MyFileList As List(Of String)
	Public SavedInDeclMode As Boolean
	Public UpshiftMinAcceleration As Single
	Public DownshiftAfterUpshift As Single
	Public UpshiftAfterDownshift As Single
	Public TCshiftFile As String


	Public Function CreateFileList() As Boolean
		Dim i As Integer

		MyFileList = New List(Of String)

		'Transm. Loss Maps
		For i = 0 To GearCount() - 1
			If Not IsNumeric(Me.GetrMap(i, True)) Then
				If Not MyFileList.Contains(Me.GetrMap(i)) Then MyFileList.Add(Me.GetrMap(i))
			End If

			'.vgbs
			If Not Cfg.DeclMode Then
				If i > 0 AndAlso Not MyFileList.Contains(Me.gs_files(i).FullPath) Then MyFileList.Add(Me.gs_files(i).FullPath)
			End If

		Next

		'Torque Converter
		If Me.TCon Then MyFileList.Add(TCfile)

		Return True
	End Function


	Public Sub New()
		MyPath = ""
		sFilePath = ""
		SetDefault()
	End Sub

	Private Sub SetDefault()

		ModelName = ""
		GbxInertia = 0
		TracIntrSi = 0

		Igetr = New List(Of Single)
		'IsTCgear = New List(Of Boolean)
		GetrMaps = New List(Of cSubPath)
		gs_files = New List(Of cSubPath)
		MaxTorque = New List(Of String)

		GetrEffDef = New List(Of Boolean)
		GetrEff = New List(Of Single)

		MyGBmaps = Nothing

		gs_TorqueResv = 0
		gs_SkipGears = False
		gs_ShiftTime = 0
		gs_TorqueResvStart = 0
		gs_StartSpeed = 0
		gs_StartAcc = 0
		gs_ShiftInside = False

		gs_Type = tGearbox.Manual

		TCon = False
		TCrefrpm = 0
		TC_file.Clear()

		TCinertia = 0

		SavedInDeclMode = False
	End Sub

	Public Function SaveFile() As Boolean
		Dim i As Integer
		Dim JSON As New JSON
		Dim dic As Dictionary(Of String, Object)
		Dim dic0 As Dictionary(Of String, Object)
		Dim ls As List(Of Object)

		'Header
		dic = New Dictionary(Of String, Object)
		dic.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		dic.Add("Date", Now.ToUniversalTime().ToString("o"))
		dic.Add("AppVersion", VECTOvers)
		dic.Add("FileVersion", FormatVersion)
		JSON.Content.Add("Header", dic)

		'Body
		dic = New Dictionary(Of String, Object)

		dic.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		dic.Add("ModelName", ModelName)

		dic.Add("Inertia", GbxInertia)
		dic.Add("TracInt", TracIntrSi)

		ls = New List(Of Object)
		For i = 0 To Igetr.Count - 1
			dic0 = New Dictionary(Of String, Object)
			dic0.Add("Ratio", Igetr(i))
			If IsNumeric(Me.GetrMap(i, True)) Then
				dic0.Add("Efficiency", GetrMaps(i).PathOrDummy)
			Else
				dic0.Add("LossMap", GetrMaps(i).PathOrDummy)
			End If
			If i > 0 Then
				'dic0.Add("TCactive", IsTCgear(i))
				dic0.Add("ShiftPolygon", gs_files(i).PathOrDummy)
				'dic0.Add("FullLoadCurve", FldFiles(i).PathOrDummy)
				dic0.Add("MaxTorque", MaxTorque(i))
			End If

			ls.Add(dic0)
		Next
		dic.Add("Gears", ls)

		dic.Add("TqReserve", gs_TorqueResv)
		dic.Add("SkipGears", gs_SkipGears)
		dic.Add("ShiftTime", gs_ShiftTime)
		dic.Add("EaryShiftUp", gs_ShiftInside)

		dic.Add("StartTqReserve", gs_TorqueResvStart)
		dic.Add("StartSpeed", gs_StartSpeed)
		dic.Add("StartAcc", gs_StartAcc)

		dic.Add("GearboxType", GearboxConv(gs_Type))

		dic0 = New Dictionary(Of String, Object)
		dic0.Add("Enabled", TCon)
		dic0.Add("File", TC_file.PathOrDummy)
		dic0.Add("RefRPM", TCrefrpm)
		dic0.Add("Inertia", TCinertia)
		dic0.Add("ShiftPolygon", TCshiftFile)
		dic.Add("TorqueConverter", dic0)


		dic.Add("DownshiftAferUpshiftDelay", DownshiftAfterUpshift)
		dic.Add("UpshiftAfterDownshiftDelay", UpshiftAfterDownshift)
		dic.Add("UpshiftMinAcceleration", UpshiftMinAcceleration)

		JSON.Content.Add("Body", dic)

		Return JSON.WriteFile(sFilePath)
	End Function

	Public Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean
		Dim i As Integer
		Dim MsgSrc As String
		Dim JSON As New JSON
		Dim dic As Object

		MsgSrc = "GBX/ReadFile"

		SetDefault()

		If Not JSON.ReadFile(sFilePath) Then Return False

		Try

			FileVersion = JSON.Content("Header")("FileVersion")

			If FileVersion > 3 Then
				SavedInDeclMode = JSON.Content("Body")("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			ModelName = JSON.Content("Body")("ModelName")
			GbxInertia = JSON.Content("Body")("Inertia")
			TracIntrSi = JSON.Content("Body")("TracInt")

			i = -1
			For Each dic In JSON.Content("Body")("Gears")
				i += 1

				Igetr.Add(dic("Ratio"))
				GetrMaps.Add(New cSubPath)

				If dic("Efficiency") Is Nothing Then
					GetrMaps(i).Init(MyPath, dic("LossMap"))
				Else
					GetrMaps(i).Init(MyPath, dic("Efficiency"))
				End If

				MaxTorque.Add(dic("MaxTorque"))
				gs_files.Add(New cSubPath)
				'FldFiles.Add(New cSubPath)

				If i = 0 Then
					'IsTCgear.Add(False)
					gs_files(i).Init(MyPath, sKey.NoFile)
					'FldFiles(i).Init(MyPath, sKey.NoFile)
				Else
					'IsTCgear.Add(dic("TCactive"))
					If FileVersion < 2 Then
						gs_files(i).Init(MyPath, JSON.Content("Body")("ShiftPolygons"))
					Else
						gs_files(i).Init(MyPath, dic("ShiftPolygon"))
					End If
					'If FileVersion < 5 Then
					'	FldFiles(i).Init(MyPath, sKey.NoFile)
					'Else
					'	FldFiles(i).Init(MyPath, dic("FullLoadCurve"))
					'End If
				End If

			Next

			gs_TorqueResv = JSON.Content("Body")("TqReserve")
			gs_SkipGears = JSON.Content("Body")("SkipGears")
			gs_ShiftTime = JSON.Content("Body")("ShiftTime")
			gs_TorqueResvStart = JSON.Content("Body")("StartTqReserve")
			gs_StartSpeed = JSON.Content("Body")("StartSpeed")
			gs_StartAcc = JSON.Content("Body")("StartAcc")
			gs_ShiftInside = JSON.Content("Body")("EaryShiftUp")

			gs_Type = GearboxConv(JSON.Content("Body")("GearboxType").ToString)

			If JSON.Content("Body")("UpshiftMinAcceleration") Is Nothing Then
				UpshiftMinAcceleration = 0.1
			Else
				UpshiftMinAcceleration = JSON.Content("Body")("UpshiftMinAcceleration")
			End If
			If JSON.Content("Body")("DownshiftAferUpshiftDelay") Is Nothing Then
				DownshiftAfterUpshift = 10
			Else
				DownshiftAfterUpshift = JSON.Content("Body")("DownshiftAferUpshiftDelay")
			End If

			If JSON.Content("Body")("UpshiftAfterDownshiftDelay") Is Nothing Then
				UpshiftAfterDownshift = 10
			Else
				UpshiftAfterDownshift = JSON.Content("Body")("UpshiftAfterDownshiftDelay")
			End If


			If JSON.Content("Body")("TorqueConverter") Is Nothing Then
				TCon = False
			Else
				TCon = JSON.Content("Body")("TorqueConverter")("Enabled")
				TC_file.Init(MyPath, JSON.Content("Body")("TorqueConverter")("File"))
				TCrefrpm = JSON.Content("Body")("TorqueConverter")("RefRPM")
				If FileVersion > 2 Then
					TCinertia = JSON.Content("Body")("TorqueConverter")("Inertia")
				End If
				If FileVersion > 5 Then
					TCshiftFile = JSON.Content("Body")("TorqueConverter")("ShiftPolygon")
				End If
			End If
		Catch ex As Exception
			If ShowMsg Then WorkerMsg(tMsgID.Err, "Failed to read VECTO file! " & ex.Message, MsgSrc)
			Return False
		End Try

		Return True
	End Function

	Public Function DeclInit() As Boolean
		Dim MsgSrc As String
		Dim i As Int16

		MsgSrc = "GBX/DeclInit"

		If gs_Type = tGearbox.Custom Or tGearboxExtension.AutomaticTransmission(gs_Type) Then
			WorkerMsg(tMsgID.Err, "Invalid gearbox type for Declaration Mode!", MsgSrc)
			Return False
		End If

		GbxInertia = cDeclaration.GbInertia
		TracIntrSi = cDeclaration.TracInt(gs_Type)
		gs_SkipGears = cDeclaration.SkipGears(gs_Type)
		gs_ShiftTime = cDeclaration.ShiftTime(gs_Type)
		gs_ShiftInside = cDeclaration.ShiftInside(gs_Type)
		gs_TorqueResv = cDeclaration.TqResv
		gs_TorqueResvStart = cDeclaration.TqResvStart
		gs_StartSpeed = cDeclaration.StartSpeed
		gs_StartAcc = cDeclaration.StartAcc

		UpshiftAfterDownshift = 10
		DownshiftAfterUpshift = 10
		UpshiftMinAcceleration = 0.1

		TCon = (AutomaticTransmission(gs_Type))

		For i = 1 To GearCount()
			Shiftpolygons(i).SetGenericShiftPoly(ENG.FLD, ENG.Nidle)
		Next


		Return True
	End Function

	Public Function TCinit() As Boolean
		Dim file As New cFile_V3
		Dim MsgSrc As String
		Dim line() As String

		MsgSrc = "GBX/TCinit"

		If Not file.OpenRead(TC_file.FullPath) Then
			WorkerMsg(tMsgID.Err, "Torque Converter file not found! (" & TC_file.FullPath & ")", MsgSrc)
			Return False
		End If

		'Skip Header
		file.ReadLine()

		If TCrefrpm <= 0 Then
			WorkerMsg(tMsgID.Err, "Torque converter reference torque invalid! (" & TCrefrpm & ")", MsgSrc)
			Return False
		End If

		TCnu.Clear()
		TCmu.Clear()
		TCtorque.Clear()
		TCdim = -1

		Try
			Do While Not file.EndOfFile
				line = file.ReadLine
				TCnuMax = CSng(line(0))
				'If CSng(line(0)) < 1 Then '@@@quam: read the complete file!
				TCnu.Add(TCnuMax)
				TCmu.Add(CSng(line(1)))
				TCtorque.Add(CSng(line(2)))
				TCdim += 1
				'End If
			Loop
		Catch ex As Exception
			WorkerMsg(tMsgID.Err, "Error while reading Torque Converter file! (" & ex.Message & ")", MsgSrc)
			Return False
		End Try

		file.Close()

		'Check if more then one point
		If TCdim < 1 Then
			WorkerMsg(tMsgID.Err, "More points in Torque Converter file needed!", MsgSrc)
			Return False
		End If

		If TCnuMax > 1 Then TCnuMax = 1

		If False Then ' @@@quam: don'r read default tcc file
			'Add default values for nu>1
			If Not file.OpenRead(MyDeclPath & "DefaultTC.vtcc") Then
				WorkerMsg(tMsgID.Err, "Default Torque Converter file not found!", MsgSrc)
				Return False
			End If

			'Skip Header
			file.ReadLine()

			Try
				Do While Not file.EndOfFile
					line = file.ReadLine
					TCnu.Add(CSng(line(0)))
					TCmu.Add(CSng(line(1)))
					TCtorque.Add(CSng(line(2)))
					TCdim += 1
				Loop
			Catch ex As Exception
				WorkerMsg(tMsgID.Err, "Error while reading Default Torque Converter file! (" & ex.Message & ")", MsgSrc)
				Return False
			End Try

			file.Close()
		End If

		Return True
	End Function


	Public Function GearCount() As Integer
		Return Me.Igetr.Count - 1
	End Function


	Public ReadOnly Property FileList As List(Of String)
		Get
			Return MyFileList
		End Get
	End Property

	Public Property FilePath() As String
		Get
			Return sFilePath
		End Get
		Set(ByVal value As String)
			sFilePath = value
			If sFilePath = "" Then
				MyPath = ""
			Else
				MyPath = IO.Path.GetDirectoryName(sFilePath) & "\"
			End If
		End Set
	End Property

	Public Property GetrMap(ByVal x As Short, Optional ByVal Original As Boolean = False) As String
		Get
			If Original Then
				Return GetrMaps(x).OriginalPath
			Else
				Return GetrMaps(x).FullPath
			End If
		End Get
		Set(ByVal value As String)
			GetrMaps(x).Init(MyPath, value)
		End Set
	End Property

	Public Property gsFile(ByVal x As Short, Optional ByVal Original As Boolean = False) As String
		Get
			If Original Then
				Return gs_files(x).OriginalPath
			Else
				Return gs_files(x).FullPath
			End If
		End Get
		Set(value As String)
			gs_files(x).Init(MyPath, value)
		End Set
	End Property

	'Public Property FldFile(ByVal x As Short, Optional ByVal Original As Boolean = False) As String
	'	Get
	'		If Original Then
	'			Return FldFiles(x).OriginalPath
	'		Else
	'			Return FldFiles(x).FullPath
	'		End If
	'	End Get
	'	Set(value As String)
	'		FldFiles(x).Init(MyPath, value)
	'	End Set
	'End Property


	Public Property TCfile(Optional ByVal Original As Boolean = False) As String
		Get
			If Original Then
				Return TC_file.OriginalPath
			Else
				Return TC_file.FullPath
			End If
		End Get
		Set(value As String)
			TC_file.Init(MyPath, value)
		End Set
	End Property


	Public Class cShiftPolygon
		Private Filepath As String

		Public gs_TqUp As New List(Of Single)
		Public gs_TqDown As New List(Of Single)
		Public gs_nUup As New List(Of Single)
		Public gs_nUdown As New List(Of Single)
		Private gs_Dup As Integer = -1
		Private gs_Ddown As Integer = -1

		Public Sub New(ByVal Path As String, ByVal Gear As Integer)
			Filepath = Path
		End Sub

		Public Function ReadFile() As Boolean
			Dim file As cFile_V3
			Dim line As String()

			Dim MsgSrc As String

			MsgSrc = "GBX/GSinit/ShiftPolygon.Init"

			'Check if file exists
			If Not IO.File.Exists(Filepath) Then
				WorkerMsg(tMsgID.Err, "Gear Shift Polygon File not found! '" & Filepath & "'", MsgSrc)
				Return False
			End If

			'Init file instance
			file = New cFile_V3

			'Open file
			If Not file.OpenRead(Filepath) Then
				WorkerMsg(tMsgID.Err, "Failed to load Gear Shift Polygon File! '" & Filepath & "'", MsgSrc)
				Return False
			End If

			'Skip Header
			file.ReadLine()

			'Clear lists
			gs_TqUp.Clear()
			gs_TqDown.Clear()
			gs_nUdown.Clear()
			gs_nUup.Clear()
			gs_Dup = -1

			'Read file
			Try
				Do While Not file.EndOfFile
					line = file.ReadLine
					gs_Dup += 1
					gs_TqUp.Add(CSng(line(0)))
					gs_TqDown.Add(CSng(line(0)))
					gs_nUdown.Add(CSng(line(1)))
					gs_nUup.Add(CSng(line(2)))
				Loop
			Catch ex As Exception
				WorkerMsg(tMsgID.Err, "Error while reading Gear Shift Polygon File! (" & ex.Message & ")", MsgSrc)
				Return False
			End Try

			'Check if more then one point
			If gs_Dup < 1 Then
				WorkerMsg(tMsgID.Err, "More points in Gear Shift Polygon File needed!", MsgSrc)
				Return False
			End If

			gs_Ddown = gs_Dup

			Return True
		End Function


		Public Sub SetGenericShiftPoly(ByVal fld0 As cFLD, ByVal nidle As Single)
			Dim Tmax As Single

			'Clear lists
			gs_TqUp.Clear()
			gs_TqDown.Clear()
			gs_nUdown.Clear()
			gs_nUup.Clear()

			Tmax = fld0.Tmax

			gs_nUdown.Add(nidle)
			gs_nUdown.Add(nidle)
			gs_nUdown.Add((fld0.Npref + fld0.Nlo) / 2)

			gs_TqDown.Add(0)
			gs_TqDown.Add(Tmax * nidle / (fld0.Npref + fld0.Nlo - nidle))
			gs_TqDown.Add(Tmax)

			gs_nUup.Add(fld0.Npref)
			gs_nUup.Add(fld0.Npref)
			gs_nUup.Add(fld0.N95h)

			gs_TqUp.Add(0)
			gs_TqUp.Add(Tmax * (fld0.Npref - nidle) / (fld0.N95h - nidle))
			gs_TqUp.Add(Tmax)

			gs_Ddown = 2
			gs_Dup = 2
		End Sub

		Public Function fGSnUdown(ByVal Tq As Single) As Single
			Dim i As Int32

			'Extrapolation for x < x(1)
			If gs_TqDown(0) >= Tq Then
				i = 1
				GoTo lbInt
			End If

			i = 0
			Do While gs_TqDown(i) < Tq And i < gs_Ddown
				i += 1
			Loop


lbInt:
			'Interpolation
			Return (Tq - gs_TqDown(i - 1)) * (gs_nUdown(i) - gs_nUdown(i - 1)) / (gs_TqDown(i) - gs_TqDown(i - 1)) + gs_nUdown(i - 1)
		End Function

		Public Function fGSnUup(ByVal Tq As Single) As Single
			Dim i As Int32

			'Extrapolation for x < x(1)
			If gs_TqUp(0) >= Tq Then
				i = 1
				GoTo lbInt
			End If

			i = 0
			Do While gs_TqUp(i) < Tq And i < gs_Dup
				i += 1
			Loop


lbInt:
			'Interpolation
			Return (Tq - gs_TqUp(i - 1)) * (gs_nUup(i) - gs_nUup(i - 1)) / (gs_TqUp(i) - gs_TqUp(i - 1)) + gs_nUup(i - 1)
		End Function
	End Class
End Class
