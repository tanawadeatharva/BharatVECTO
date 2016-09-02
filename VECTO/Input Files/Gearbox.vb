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

Public Class Gearbox
	Private Const FormatVersion As Short = 6
	Private _fileVersion As Short

	Private _myPath As String
	Private _filePath As String

	Public ModelName As String
	Public GbxInertia As Single
	Public TracIntrSi As Single

	Public Igetr As List(Of Single)
	Public GetrMaps As List(Of SubPath)
	'Public IsTCgear As List(Of Boolean)

	'Gear shift polygons
	Public gs_files As List(Of SubPath)

	Public MaxTorque As List(Of String)

	Public TorqueResv As Single
	Public SkipGears As Boolean
	Public ShiftTime As Integer
	Public TorqueResvStart As Single
	Public StartSpeed As Single
	Public StartAcc As Single
	Public ShiftInside As Boolean

	Public Type As GearboxType

	'Torque Converter Input
	Public TorqueConverterEnabled As Boolean
	Public TorqueConverterReferenceRpm As Single
	Private ReadOnly TorqueConverterFile As New SubPath
	Public TorqueConverterInertia As Single


	Public SavedInDeclMode As Boolean
	Public UpshiftMinAcceleration As Single
	Public DownshiftAfterUpshift As Single
	Public UpshiftAfterDownshift As Single
	Public TCshiftFile As String


	Public Sub New()
		_myPath = ""
		_filePath = ""
		SetDefault()
	End Sub

	Private Sub SetDefault()

		ModelName = ""
		GbxInertia = 0
		TracIntrSi = 0

		Igetr = New List(Of Single)
		GetrMaps = New List(Of SubPath)
		gs_files = New List(Of SubPath)
		MaxTorque = New List(Of String)

		TorqueResv = 0
		SkipGears = False
		ShiftTime = 0
		TorqueResvStart = 0
		StartSpeed = 0
		StartAcc = 0
		ShiftInside = False

		Type = GearboxType.MT

		TorqueConverterEnabled = False
		TorqueConverterReferenceRpm = 0
		TorqueConverterFile.Clear()

		TorqueConverterInertia = 0

		SavedInDeclMode = False
	End Sub

	Public Function SaveFile() As Boolean
		Dim i As Integer
		Dim JSON As New JSONParser
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
			If IsNumeric(GetrMap(i, True)) Then
				dic0.Add("Efficiency", GetrMaps(i).PathOrDummy)
			Else
				dic0.Add("LossMap", GetrMaps(i).PathOrDummy)
			End If
			If i > 0 Then
				dic0.Add("ShiftPolygon", gs_files(i).PathOrDummy)
				dic0.Add("MaxTorque", MaxTorque(i))
			End If

			ls.Add(dic0)
		Next
		dic.Add("Gears", ls)

		dic.Add("TqReserve", TorqueResv)
		dic.Add("SkipGears", SkipGears)
		dic.Add("ShiftTime", ShiftTime)
		dic.Add("EaryShiftUp", ShiftInside)

		dic.Add("StartTqReserve", TorqueResvStart)
		dic.Add("StartSpeed", StartSpeed)
		dic.Add("StartAcc", StartAcc)

		dic.Add("GearboxType", Type)

		dic0 = New Dictionary(Of String, Object)
		dic0.Add("Enabled", TorqueConverterEnabled)
		dic0.Add("File", TorqueConverterFile.PathOrDummy)
		dic0.Add("RefRPM", TorqueConverterReferenceRpm)
		dic0.Add("Inertia", TorqueConverterInertia)
		dic0.Add("ShiftPolygon", TCshiftFile)
		dic.Add("TorqueConverter", dic0)


		dic.Add("DownshiftAferUpshiftDelay", DownshiftAfterUpshift)
		dic.Add("UpshiftAfterDownshiftDelay", UpshiftAfterDownshift)
		dic.Add("UpshiftMinAcceleration", UpshiftMinAcceleration)

		JSON.Content.Add("Body", dic)

		Return JSON.WriteFile(_filePath)
	End Function

	Public Function ReadFile(Optional ByVal ShowMsg As Boolean = True) As Boolean
		Dim i As Integer
		Dim MsgSrc As String
		Dim JSON As New JSONParser
		Dim dic As Object

		MsgSrc = "GBX/ReadFile"

		SetDefault()

		If Not JSON.ReadFile(_filePath) Then Return False

		Try

			_fileVersion = JSON.Content("Header")("FileVersion")

			If _fileVersion > 3 Then
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
				GetrMaps.Add(New SubPath)

				If dic("Efficiency") Is Nothing Then
					GetrMaps(i).Init(_myPath, dic("LossMap"))
				Else
					GetrMaps(i).Init(_myPath, dic("Efficiency"))
				End If

				MaxTorque.Add(dic("MaxTorque"))
				gs_files.Add(New SubPath)

				If i = 0 Then
					gs_files(i).Init(_myPath, sKey.NoFile)
				Else
					If _fileVersion < 2 Then
						gs_files(i).Init(_myPath, JSON.Content("Body")("ShiftPolygons"))
					Else
						gs_files(i).Init(_myPath, dic("ShiftPolygon"))
					End If
				End If

			Next

			TorqueResv = JSON.Content("Body")("TqReserve")
			SkipGears = JSON.Content("Body")("SkipGears")
			ShiftTime = JSON.Content("Body")("ShiftTime")
			TorqueResvStart = JSON.Content("Body")("StartTqReserve")
			StartSpeed = JSON.Content("Body")("StartSpeed")
			StartAcc = JSON.Content("Body")("StartAcc")
			ShiftInside = JSON.Content("Body")("EaryShiftUp")

			Type = JSON.Content("Body")("GearboxType").ToString.ParseEnum(Of GearboxType)()

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
				TorqueConverterEnabled = False
			Else
				TorqueConverterEnabled = JSON.Content("Body")("TorqueConverter")("Enabled")
				TorqueConverterFile.Init(_myPath, JSON.Content("Body")("TorqueConverter")("File"))
				TorqueConverterReferenceRpm = JSON.Content("Body")("TorqueConverter")("RefRPM")
				If _fileVersion > 2 Then
					TorqueConverterInertia = JSON.Content("Body")("TorqueConverter")("Inertia")
				End If
				If _fileVersion > 5 Then
					TCshiftFile = JSON.Content("Body")("TorqueConverter")("ShiftPolygon")
				End If
			End If
		Catch ex As Exception
			If ShowMsg Then WorkerMsg(MessageType.Err, "Failed to read VECTO file! " & ex.Message, MsgSrc)
			Return False
		End Try

		Return True
	End Function


	Public Function GearCount() As Integer
		Return Igetr.Count - 1
	End Function


	Public Property FilePath() As String
		Get
			Return _filePath
		End Get
		Set(ByVal value As String)
			_filePath = value
			If _filePath = "" Then
				_myPath = ""
			Else
				_myPath = Path.GetDirectoryName(_filePath) & "\"
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
			GetrMaps(x).Init(_myPath, value)
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
			gs_files(x).Init(_myPath, value)
		End Set
	End Property

	Public Property TCfile(Optional ByVal Original As Boolean = False) As String
		Get
			If Original Then
				Return TorqueConverterFile.OriginalPath
			Else
				Return TorqueConverterFile.FullPath
			End If
		End Get
		Set(value As String)
			TorqueConverterFile.Init(_myPath, value)
		End Set
	End Property
End Class
