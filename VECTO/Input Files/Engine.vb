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
Imports TUGraz.VectoCore.InputData.FileIO.JSON

''' <summary>
''' Engine input file
''' </summary>
''' <remarks></remarks>
Public Class Engine
	''' <summary>
	''' Current format version
	''' </summary>
	''' <remarks></remarks>
	Private Const FormatVersion As Short = 3

	''' <summary>
	''' Format version of input file. Defined in ReadFile.
	''' </summary>
	''' <remarks></remarks>
	Private _fileVersion As Integer

	''' <summary>
	''' Engine description (model, type, etc.). Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public ModelName As String

	''' <summary>
	''' Engine displacement [ccm]. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public Displacement As Double

	''' <summary>
	''' Idling speed [1/min]. Saved in input file.
	''' </summary>
	''' <remarks></remarks>
	Public IdleSpeed As Double

	''' <summary>
	''' Rotational inertia including flywheel [kgm²]. Saved in input file. Overwritten by generic value in Declaration mode.
	''' </summary>
	''' <remarks></remarks>
	Public EngineInertia As Double

	''' <summary>
	''' List of full load/motoring curve files (.vfld)
	''' </summary>
	''' <remarks></remarks>
	Private ReadOnly _fullLoadCurvePath As SubPath

	''' <summary>
	''' Path to fuel consumption map
	''' </summary>
	''' <remarks></remarks>
	Private ReadOnly _fuelConsumptionMapPath As SubPath

	''' <summary>
	''' Directory of engine file. Defined in FilePath property (Set)
	''' </summary>
	''' <remarks></remarks>
	Private _myPath As String

	''' <summary>
	''' Full file path. Needs to be defined via FilePath property before calling ReadFile or SaveFile.
	''' </summary>
	''' <remarks></remarks>
	Private _filePath As String


	''' <summary>
	''' WHTC Urban test results. Saved in input file. 
	''' </summary>
	''' <remarks></remarks>
	Public WHTCurban As Double

	''' <summary>
	''' WHTC Rural test results. Saved in input file. 
	''' </summary>
	''' <remarks></remarks>
	Public WHTCrural As Double

	''' <summary>
	''' WHTC Motorway test results. Saved in input file. 
	''' </summary>
	''' <remarks></remarks>
	Public WHTCmw As Double


	Public SavedInDeclMode As Boolean


	''' <summary>
	''' New instance. Initialise
	''' </summary>
	''' <remarks></remarks>
	Public Sub New()
		_myPath = ""
		_filePath = ""
		_fuelConsumptionMapPath = New SubPath
		_fullLoadCurvePath = New SubPath
		SetDefault()
	End Sub

	''' <summary>
	''' Set default values
	''' </summary>
	''' <remarks></remarks>
	Private Sub SetDefault()
		ModelName = "Undefined"
		Displacement = 0
		IdleSpeed = 0
		EngineInertia = 0


		_fuelConsumptionMapPath.Clear()
		_fullLoadCurvePath.Clear()

		WHTCurban = 0
		WHTCrural = 0
		WHTCmw = 0

		SavedInDeclMode = False
	End Sub

	''' <summary>
	''' Save file. <see cref="P:VECTO.cENG.FilePath" /> must be set before calling.
	''' </summary>
	''' <returns>True if successful.</returns>
	''' <remarks></remarks>
	Public Function SaveFile() As Boolean
		Dim JSON As New JSONParser
		Dim dic As Dictionary(Of String, Object)

		'Header
		dic = New Dictionary(Of String, Object)
		dic.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		dic.Add("Date", Now.ToUniversalTime().ToString("o"))
		dic.Add("AppVersion", VECTOvers)
		dic.Add("FileVersion", FormatVersion)
		JSON.Content.Add("Header", JToken.FromObject(dic))

		'Body
		dic = New Dictionary(Of String, Object)

		dic.Add("SavedInDeclMode", Cfg.DeclMode)
		SavedInDeclMode = Cfg.DeclMode

		dic.Add("ModelName", ModelName)

		dic.Add("Displacement", Displacement)
		dic.Add("IdlingSpeed", IdleSpeed)
		dic.Add("Inertia", EngineInertia)

		dic.Add("FullLoadCurve", _fullLoadCurvePath.PathOrDummy)

		dic.Add("FuelMap", _fuelConsumptionMapPath.PathOrDummy)

		dic.Add("WHTC-Urban", WHTCurban)
		dic.Add("WHTC-Rural", WHTCrural)
		dic.Add("WHTC-Motorway", WHTCmw)


		JSON.Content.Add("Body", JToken.FromObject(dic))


		Return JSON.WriteFile(_filePath)
	End Function

	''' <summary>
	''' Read file. <see cref="P:VECTO.cENG.FilePath" /> must be set before calling.
	''' </summary>
	''' <returns>True if successful.</returns>
	''' <remarks></remarks>
	Public Function ReadFile(Optional ByVal showMsg As Boolean = True) As Boolean
		Dim msgSrc As String
		Dim json As New JSONParser

		msgSrc = "ENG/ReadFile"

		SetDefault()


		If Not json.ReadFile(_filePath) Then Return False

		Try

			_fileVersion = json.Content.GetEx("Header").GetEx(Of Integer)("FileVersion")

			Dim body As JToken = json.Content.GetEx("Body")
			If _fileVersion > 1 Then
				SavedInDeclMode = body.GetEx(Of Boolean)("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			ModelName = body.GetEx(Of String)("ModelName")

			Displacement = body.GetEx(Of Double)("Displacement")
			IdleSpeed = body.GetEx(Of Double)("IdlingSpeed")
			EngineInertia = body.GetEx(Of Double)("Inertia")

			If _fileVersion < 3 Then
				_fullLoadCurvePath.Init(_myPath, body.GetEx("FullLoadCurves").First.GetEx(Of String)("Path"))
			Else
				_fullLoadCurvePath.Init(_myPath, body.GetEx(Of String)("FullLoadCurve"))
			End If

			_fuelConsumptionMapPath.Init(_myPath, body.GetEx(Of String)("FuelMap"))

			If _fileVersion > 2 AndAlso Not body("WHTC-Urban") Is Nothing Then
				WHTCurban = (body.GetEx(Of Double)("WHTC-Urban"))
				WHTCrural = (body.GetEx(Of Double)("WHTC-Rural"))
				WHTCmw = (body.GetEx(Of Double)("WHTC-Motorway"))
			End If

		Catch ex As Exception
			If showMsg Then WorkerMsg(MessageType.Err, "Failed to read VECTO file! " & ex.Message, msgSrc)
			Return False
		End Try

		Return True
	End Function


	''' <summary>
	''' Get or set Filepath before calling <see cref="M:VECTO.cENG.ReadFile" /> or <see cref="M:VECTO.cENG.SaveFile" />
	''' </summary>
	''' <value></value>
	''' <returns>Full filepath</returns>
	''' <remarks></remarks>
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


	Public Property PathFLD(Optional ByVal Original As Boolean = False) As String
		Get
			If Original Then
				Return _fullLoadCurvePath.OriginalPath
			Else
				Return _fullLoadCurvePath.FullPath
			End If
		End Get
		Set(ByVal value As String)
			_fullLoadCurvePath.Init(_myPath, value)
		End Set
	End Property

	''' <summary>
	''' Get or set file path (cSubPath) to FC map (.vmap)
	''' </summary>
	''' <param name="Original">True= (relative) file path as saved in file; False= full file path</param>
	''' <value></value>
	''' <returns>Relative or absolute file path to FC map</returns>
	''' <remarks></remarks>
	Public Property PathMAP(Optional ByVal Original As Boolean = False) As String
		Get
			If Original Then
				Return _fuelConsumptionMapPath.OriginalPath
			Else
				Return _fuelConsumptionMapPath.FullPath
			End If
		End Get
		Set(ByVal value As String)
			_fuelConsumptionMapPath.Init(_myPath, value)
		End Set
	End Property
End Class

