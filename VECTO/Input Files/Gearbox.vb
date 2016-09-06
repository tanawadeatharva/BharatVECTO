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

	Public GearRatios As List(Of Single)
	Public GearLossmaps As List(Of SubPath)

	'Gear shift polygons
	Public GearshiftFiles As List(Of SubPath)

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
	Private ReadOnly _torqueConverterFile As New SubPath
	Public TorqueConverterInertia As Single
	Public TorqueConverterShiftPolygonFile As String


	Public SavedInDeclMode As Boolean
	Public UpshiftMinAcceleration As Single
	Public DownshiftAfterUpshift As Single
	Public UpshiftAfterDownshift As Single


	Public Sub New()
		_myPath = ""
		_filePath = ""
		SetDefault()
	End Sub

	Private Sub SetDefault()

		ModelName = ""
		GbxInertia = 0
		TracIntrSi = 0

		GearRatios = New List(Of Single)
		GearLossmaps = New List(Of SubPath)
		GearshiftFiles = New List(Of SubPath)
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
		_torqueConverterFile.Clear()

		TorqueConverterInertia = 0

		SavedInDeclMode = False
	End Sub

	Public Function SaveFile() As Boolean
		Dim i As Integer
		Dim writer As New JSONParser
		Dim content As Dictionary(Of String, Object)

		'Header
		content = New Dictionary(Of String, Object)
		content.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		content.Add("Date", Now.ToUniversalTime().ToString("o"))
		content.Add("AppVersion", VECTOvers)
		content.Add("FileVersion", FormatVersion)
		writer.Content.Add("Header", content)

		'Body
		content = New Dictionary(Of String, Object)

		content.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		content.Add("ModelName", ModelName)

		content.Add("Inertia", GbxInertia)
		content.Add("TracInt", TracIntrSi)

		Dim ls As New List(Of Object)
		For i = 0 To GearRatios.Count - 1
			Dim gearDict As New Dictionary(Of String, Object)
			gearDict.Add("Ratio", GearRatios(i))
			If IsNumeric(GearLossMap(i, True)) Then
				gearDict.Add("Efficiency", GearLossmaps(i).PathOrDummy)
			Else
				gearDict.Add("LossMap", GearLossmaps(i).PathOrDummy)
			End If
			If i > 0 Then
				gearDict.Add("ShiftPolygon", GearshiftFiles(i).PathOrDummy)
				gearDict.Add("MaxTorque", MaxTorque(i))
			End If

			ls.Add(gearDict)
		Next
		content.Add("Gears", ls)

		content.Add("TqReserve", TorqueResv)
		content.Add("SkipGears", SkipGears)
		content.Add("ShiftTime", ShiftTime)
		content.Add("EaryShiftUp", ShiftInside)

		content.Add("StartTqReserve", TorqueResvStart)
		content.Add("StartSpeed", StartSpeed)
		content.Add("StartAcc", StartAcc)

		content.Add("GearboxType", Type)

		Dim torqueConverterDict As New Dictionary(Of String, Object)
		torqueConverterDict.Add("Enabled", TorqueConverterEnabled)
		torqueConverterDict.Add("File", _torqueConverterFile.PathOrDummy)
		torqueConverterDict.Add("RefRPM", TorqueConverterReferenceRpm)
		torqueConverterDict.Add("Inertia", TorqueConverterInertia)
		torqueConverterDict.Add("ShiftPolygon", TorqueConverterShiftPolygonFile)
		content.Add("TorqueConverter", torqueConverterDict)


		content.Add("DownshiftAferUpshiftDelay", DownshiftAfterUpshift)
		content.Add("UpshiftAfterDownshiftDelay", UpshiftAfterDownshift)
		content.Add("UpshiftMinAcceleration", UpshiftMinAcceleration)

		writer.Content.Add("Body", content)

		Return writer.WriteFile(_filePath)
	End Function

	Public Function ReadFile(Optional ByVal showMsg As Boolean = True) As Boolean
		Dim i As Integer
		Dim parser As New JSONParser
		Dim dic As Object

		Const msgSrc As String = "GBX/ReadFile"

		SetDefault()

		If Not parser.ReadFile(_filePath) Then Return False

		Try

			_fileVersion = parser.Content("Header")("FileVersion")

			If _fileVersion > 3 Then
				SavedInDeclMode = parser.Content("Body")("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			ModelName = parser.Content("Body")("ModelName")
			GbxInertia = parser.Content("Body")("Inertia")
			TracIntrSi = parser.Content("Body")("TracInt")

			i = -1
			For Each dic In parser.Content("Body")("Gears")
				i += 1

				GearRatios.Add(dic("Ratio"))
				GearLossmaps.Add(New SubPath)

				If dic("Efficiency") Is Nothing Then
					GearLossmaps(i).Init(_myPath, dic("LossMap"))
				Else
					GearLossmaps(i).Init(_myPath, dic("Efficiency"))
				End If

				MaxTorque.Add(dic("MaxTorque"))
				GearshiftFiles.Add(New SubPath)

				If i = 0 Then
					GearshiftFiles(i).Init(_myPath, sKey.NoFile)
				Else
					If _fileVersion < 2 Then
						GearshiftFiles(i).Init(_myPath, parser.Content("Body")("ShiftPolygons"))
					Else
						GearshiftFiles(i).Init(_myPath, dic("ShiftPolygon"))
					End If
				End If

			Next

			TorqueResv = parser.Content("Body")("TqReserve")
			SkipGears = parser.Content("Body")("SkipGears")
			ShiftTime = parser.Content("Body")("ShiftTime")
			TorqueResvStart = parser.Content("Body")("StartTqReserve")
			StartSpeed = parser.Content("Body")("StartSpeed")
			StartAcc = parser.Content("Body")("StartAcc")
			ShiftInside = parser.Content("Body")("EaryShiftUp")

			Type = parser.Content("Body")("GearboxType").ToString.ParseEnum(Of GearboxType)()

			If parser.Content("Body")("UpshiftMinAcceleration") Is Nothing Then
				UpshiftMinAcceleration = 0.1
			Else
				UpshiftMinAcceleration = parser.Content("Body")("UpshiftMinAcceleration")
			End If
			If parser.Content("Body")("DownshiftAferUpshiftDelay") Is Nothing Then
				DownshiftAfterUpshift = 10
			Else
				DownshiftAfterUpshift = parser.Content("Body")("DownshiftAferUpshiftDelay")
			End If

			If parser.Content("Body")("UpshiftAfterDownshiftDelay") Is Nothing Then
				UpshiftAfterDownshift = 10
			Else
				UpshiftAfterDownshift = parser.Content("Body")("UpshiftAfterDownshiftDelay")
			End If


			If parser.Content("Body")("TorqueConverter") Is Nothing Then
				TorqueConverterEnabled = False
			Else
				TorqueConverterEnabled = parser.Content("Body")("TorqueConverter")("Enabled")
				_torqueConverterFile.Init(_myPath, parser.Content("Body")("TorqueConverter")("File"))
				TorqueConverterReferenceRpm = parser.Content("Body")("TorqueConverter")("RefRPM")
				If _fileVersion > 2 Then
					TorqueConverterInertia = parser.Content("Body")("TorqueConverter")("Inertia")
				End If
				If _fileVersion > 5 Then
					TorqueConverterShiftPolygonFile = parser.Content("Body")("TorqueConverter")("ShiftPolygon")
				End If
			End If
		Catch ex As Exception
			If showMsg Then WorkerMsg(MessageType.Err, "Failed to read VECTO file! " & ex.Message, msgSrc)
			Return False
		End Try

		Return True
	End Function


	Public Function GearCount() As Integer
		Return GearRatios.Count - 1
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

	Public Property GearLossMap(ByVal gearNr As Short, Optional ByVal original As Boolean = False) As String
		Get
			If Original Then
				Return GearLossmaps(gearNr).OriginalPath
			Else
				Return GearLossmaps(gearNr).FullPath
			End If
		End Get
		Set(ByVal value As String)
			GearLossmaps(gearNr).Init(_myPath, value)
		End Set
	End Property

	Public Property ShiftPolygonFile(ByVal gearNr As Short, Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return GearshiftFiles(gearNr).OriginalPath
			Else
				Return GearshiftFiles(gearNr).FullPath
			End If
		End Get
		Set(value As String)
			GearshiftFiles(gearNr).Init(_myPath, value)
		End Set
	End Property

	Public Property TorqueConverterFile(Optional ByVal original As Boolean = False) As String
		Get
			If Original Then
				Return _torqueConverterFile.OriginalPath
			Else
				Return _torqueConverterFile.FullPath
			End If
		End Get
		Set(value As String)
			_torqueConverterFile.Init(_myPath, value)
		End Set
	End Property
End Class

