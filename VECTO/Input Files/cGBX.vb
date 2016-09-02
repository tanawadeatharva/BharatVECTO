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
Imports System.IO
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils

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

	'Gear shift polygons
	Public gs_files As List(Of cSubPath)

	Public MaxTorque As List(Of String)

	Public gs_TorqueResv As Single
	Public gs_SkipGears As Boolean
	Public gs_ShiftTime As Integer
	Public gs_TorqueResvStart As Single
	Public gs_StartSpeed As Single
	Public gs_StartAcc As Single
	Public gs_ShiftInside As Boolean

	Public gs_Type As GearboxType

	'Torque Converter Input
	Public TCon As Boolean
	Public TCrefrpm As Single
	Private TC_file As New cSubPath
	Public TCinertia As Single


	Public SavedInDeclMode As Boolean
	Public UpshiftMinAcceleration As Single
	Public DownshiftAfterUpshift As Single
	Public UpshiftAfterDownshift As Single
	Public TCshiftFile As String


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

		gs_TorqueResv = 0
		gs_SkipGears = False
		gs_ShiftTime = 0
		gs_TorqueResvStart = 0
		gs_StartSpeed = 0
		gs_StartAcc = 0
		gs_ShiftInside = False

		gs_Type = GearboxType.MT

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

		dic.Add("GearboxType", gs_Type)

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

			gs_Type = JSON.Content("Body")("GearboxType").ToString.ParseEnum(Of GearboxType)()

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


	Public Function GearCount() As Integer
		Return Me.Igetr.Count - 1
	End Function


	Public Property FilePath() As String
		Get
			Return sFilePath
		End Get
		Set(ByVal value As String)
			sFilePath = value
			If sFilePath = "" Then
				MyPath = ""
			Else
				MyPath = Path.GetDirectoryName(sFilePath) & "\"
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
End Class
