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
Imports Newtonsoft.Json.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class Gearbox
	Private Const FormatVersion As Short = 6
	Private _fileVersion As Integer

	Private _myPath As String
	Private _filePath As String

	Public ModelName As String
	Public GbxInertia As Double
	Public TracIntrSi As Double

	Public GearRatios As List(Of Double)
	Public GearLossmaps As List(Of SubPath)

	'Gear shift polygons
	Public GearshiftFiles As List(Of SubPath)

	Public MaxTorque As List(Of String)

	Public TorqueResv As Double
	Public SkipGears As Boolean
	Public ShiftTime As Double
	Public TorqueResvStart As Double
	Public StartSpeed As Double
	Public StartAcc As Double
	Public ShiftInside As Boolean

	Public Type As GearboxType

	'Torque Converter Input
	Public TorqueConverterEnabled As Boolean
	Public TorqueConverterReferenceRpm As Double
	Private ReadOnly _torqueConverterFile As New SubPath
	Public TorqueConverterInertia As Double
	Public TorqueConverterShiftPolygonFile As String


	Public SavedInDeclMode As Boolean
	Public UpshiftMinAcceleration As Double
	Public DownshiftAfterUpshift As Double
	Public UpshiftAfterDownshift As Double


	Public Sub New()
		_myPath = ""
		_filePath = ""
		SetDefault()
	End Sub

	Private Sub SetDefault()

		ModelName = ""
		GbxInertia = 0
		TracIntrSi = 0

		GearRatios = New List(Of Double)
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
		Dim json As New JSONParser

		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		header.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		header.Add("Date", Now.ToUniversalTime().ToString("o"))
		header.Add("AppVersion", VECTOvers)
		header.Add("FileVersion", FormatVersion)


		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		body.Add("ModelName", ModelName)

		body.Add("Inertia", GbxInertia)
		body.Add("TracInt", TracIntrSi)

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
		body.Add("Gears", ls)

		body.Add("TqReserve", TorqueResv)
		body.Add("SkipGears", SkipGears)
		body.Add("ShiftTime", ShiftTime)
		body.Add("EaryShiftUp", ShiftInside)

		body.Add("StartTqReserve", TorqueResvStart)
		body.Add("StartSpeed", StartSpeed)
		body.Add("StartAcc", StartAcc)

		body.Add("GearboxType", Type)

		Dim torqueConverterDict As New Dictionary(Of String, Object)
		torqueConverterDict.Add("Enabled", TorqueConverterEnabled)
		torqueConverterDict.Add("File", _torqueConverterFile.PathOrDummy)
		torqueConverterDict.Add("RefRPM", TorqueConverterReferenceRpm)
		torqueConverterDict.Add("Inertia", TorqueConverterInertia)
		torqueConverterDict.Add("ShiftPolygon", TorqueConverterShiftPolygonFile)
		body.Add("TorqueConverter", torqueConverterDict)


		body.Add("DownshiftAferUpshiftDelay", DownshiftAfterUpshift)
		body.Add("UpshiftAfterDownshiftDelay", UpshiftAfterDownshift)
		body.Add("UpshiftMinAcceleration", UpshiftMinAcceleration)

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})

		Return json.WriteFile(_filePath)
	End Function

	Public Function ReadFile(Optional ByVal showMsg As Boolean = True) As Boolean
		Dim i As Integer
		Dim json As New JSONParser
		Dim dic As JToken

		Const msgSrc As String = "GBX/ReadFile"

		SetDefault()

		If Not json.ReadFile(_filePath) Then Return False

		Try

			_fileVersion = json.Content.GetEx("Header").GetEx(Of Integer)("FileVersion")

			Dim body As JToken = json.Content.GetEx("Body")
			If _fileVersion > 3 Then
				SavedInDeclMode = body.GetEx(Of Boolean)("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			ModelName = body.GetEx(Of String)("ModelName")
			GbxInertia = body.GetEx(Of Double)("Inertia")
			TracIntrSi = body.GetEx(Of Double)("TracInt")

			i = -1
			For Each dic In body.GetEx("Gears")
				i += 1

				GearRatios.Add(dic.GetEx(Of Double)("Ratio"))
				GearLossmaps.Add(New SubPath)

				If dic("Efficiency") Is Nothing Then
					GearLossmaps(i).Init(_myPath, dic.GetEx(Of String)("LossMap"))
				Else
					GearLossmaps(i).Init(_myPath, dic.GetEx(Of Double)("Efficiency").ToString())
				End If

				MaxTorque.Add(dic.GetEx(Of String)("MaxTorque"))
				GearshiftFiles.Add(New SubPath)

				If i = 0 Then
					GearshiftFiles(i).Init(_myPath, Constants.NoFile)
				Else
					If _fileVersion < 2 Then
						GearshiftFiles(i).Init(_myPath, body.GetEx(Of String)("ShiftPolygons"))
					Else
						GearshiftFiles(i).Init(_myPath, dic.GetEx(Of String)("ShiftPolygon"))
					End If
				End If

			Next

			TorqueResv = body.GetEx(Of Double)("TqReserve")
			SkipGears = body.GetEx(Of Boolean)("SkipGears")
			ShiftTime = body.GetEx(Of Double)("ShiftTime")
			TorqueResvStart = body.GetEx(Of Double)("StartTqReserve")
			StartSpeed = body.GetEx(Of Double)("StartSpeed")
			StartAcc = body.GetEx(Of Double)("StartAcc")
			ShiftInside = body.GetEx(Of Boolean)("EaryShiftUp")

			Type = json.Content("Body")("GearboxType").ToString.ParseEnum(Of GearboxType)()

			If body("UpshiftMinAcceleration") Is Nothing Then
				UpshiftMinAcceleration = 0.1
			Else
				UpshiftMinAcceleration = body.GetEx(Of Double)("UpshiftMinAcceleration")
			End If
			If body("DownshiftAferUpshiftDelay") Is Nothing Then
				DownshiftAfterUpshift = 10
			Else
				DownshiftAfterUpshift = body.GetEx(Of Double)("DownshiftAferUpshiftDelay")
			End If

			If body("UpshiftAfterDownshiftDelay") Is Nothing Then
				UpshiftAfterDownshift = 10
			Else
				UpshiftAfterDownshift = body.GetEx(Of Double)("UpshiftAfterDownshiftDelay")
			End If


			If json.Content("Body")("TorqueConverter") Is Nothing Then
				TorqueConverterEnabled = False
			Else
				Dim torqueConverter As JToken = body.GetEx("TorqueConverter")
				TorqueConverterEnabled = torqueConverter.GetEx(Of Boolean)("Enabled")
				_torqueConverterFile.Init(_myPath, torqueConverter.GetEx(Of String)("File"))
				TorqueConverterReferenceRpm = torqueConverter.GetEx(Of Double)("RefRPM")
				If _fileVersion > 2 Then
					TorqueConverterInertia = torqueConverter.GetEx(Of Double)("Inertia")
				End If
				If _fileVersion > 5 Then
					TorqueConverterShiftPolygonFile = torqueConverter.GetEx(Of String)("ShiftPolygon")
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

	Public Property GearLossMap(ByVal gearNr As Integer, Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return GearLossmaps(gearNr).OriginalPath
			Else
				Return GearLossmaps(gearNr).FullPath
			End If
		End Get
		Set(ByVal value As String)
			GearLossmaps(gearNr).Init(_myPath, value)
		End Set
	End Property

	Public Property ShiftPolygonFile(ByVal gearNr As Integer, Optional ByVal original As Boolean = False) As String
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
			If original Then
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

