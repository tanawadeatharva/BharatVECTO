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
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Utils

''' <summary>
''' Engine input file
''' </summary>
''' <remarks></remarks>
<CustomValidation(GetType(Engine), "ValidateEngine")>
Public Class Engine
	Implements IEngineEngineeringInputData, IEngineDeclarationInputData

	''' <summary>
	''' Current format version
	''' </summary>
	''' <remarks></remarks>
	Private Const FormatVersion As Short = 3

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
	Public WHTCUrbanInput As Double

	''' <summary>
	''' WHTC Rural test results. Saved in input file. 
	''' </summary>
	''' <remarks></remarks>
	Public WHTCRuralInput As Double

	''' <summary>
	''' WHTC Motorway test results. Saved in input file. 
	''' </summary>
	''' <remarks></remarks>
	Public WHTCMotorwayInput As Double

	Public WHTCEngineeringInput As Double


	Public ColdHotBalancingFactorInput As Double


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

		WHTCUrbanInput = 0
		WHTCRuralInput = 0
		WHTCMotorwayInput = 0
		WHTCEngineeringInput = 1
	End Sub

	''' <summary>
	''' Save file. <see cref="P:VECTO.cENG.FilePath" /> must be set before calling.
	''' </summary>
	''' <returns>True if successful.</returns>
	''' <remarks></remarks>
	Public Function SaveFile() As Boolean

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save gearbox")
			Return False
		End If

		Dim json As New JSONWriter

		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object)
		header.Add("CreatedBy", Lic.LicString & " (" & Lic.GUID & ")")
		header.Add("Date", Now.ToUniversalTime().ToString("o"))
		header.Add("AppVersion", VECTOvers)
		header.Add("FileVersion", FormatVersion)

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object)

		body.Add("SavedInDeclMode", Cfg.DeclMode)

		body.Add("ModelName", ModelName)

		body.Add("Displacement", Displacement)
		body.Add("IdlingSpeed", IdleSpeed)
		body.Add("Inertia", EngineInertia)

		body.Add("FullLoadCurve", _fullLoadCurvePath.PathOrDummy)

		body.Add("FuelMap", _fuelConsumptionMapPath.PathOrDummy)

		body.Add("WHTC-Urban", WHTCUrbanInput)
		body.Add("WHTC-Rural", WHTCRuralInput)
		body.Add("WHTC-Motorway", WHTCMotorwayInput)
		body.Add("ColdHotBalancingFactor", ColdHotBalancingFactorInput)

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})

		Return json.WriteFile(_filePath)
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


	Public Property PathFld(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
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
	''' <param name="original">True= (relative) file path as saved in file; False= full file path</param>
	''' <value></value>
	''' <returns>Relative or absolute file path to FC map</returns>
	''' <remarks></remarks>
	Public Property PathMap(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _fuelConsumptionMapPath.OriginalPath
			Else
				Return _fuelConsumptionMapPath.FullPath
			End If
		End Get
		Set(ByVal value As String)
			_fuelConsumptionMapPath.Init(_myPath, value)
		End Set
	End Property


	' ReSharper disable once UnusedMember.Global  -- used for Validation
	Public Shared Function ValidateEngine(engine As Engine, validationContext As ValidationContext) As ValidationResult
		Dim engineData As CombustionEngineData
		Dim modeService As ExecutionModeServiceContainer = TryCast(validationContext.GetService(GetType(ExecutionMode)), 
																	ExecutionModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

		Try
			If mode = ExecutionMode.Declaration Then
				Dim doa As DeclarationDataAdapter = New DeclarationDataAdapter()

				engineData = doa.CreateEngineData(engine, GearboxType.AMT)
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				engineData = doa.CreateEngineData(engine, Nothing)
			End If

			Dim result As IList(Of ValidationResult) =
					engineData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))

			If Not result.Any() Then Return ValidationResult.Success

			Return New ValidationResult("Engine Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		End Try
	End Function

#Region "IInputData"

	Public ReadOnly Property SourceType As DataSourceType Implements IComponentInputData.SourceType
		Get
			Return DataSourceType.JSONFile
		End Get
	End Property

	Public ReadOnly Property Source As String Implements IComponentInputData.Source
		Get
			Return FilePath
		End Get
	End Property

	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
		Get
			Return Cfg.DeclMode
		End Get
	End Property

	Public ReadOnly Property Vendor As String Implements IComponentInputData.Vendor
		Get
			Return "N.A." ' TODO: MQ 20160919
		End Get
	End Property

	Public ReadOnly Property Creator As String Implements IComponentInputData.Creator
		Get
			Return Lic.LicString
		End Get
	End Property

	Public ReadOnly Property [Date] As String Implements IComponentInputData.[Date]
		Get
			Return Now.ToUniversalTime().ToString("o")
		End Get
	End Property

	Public ReadOnly Property TypeId As String Implements IComponentInputData.TypeId
		Get
			Return "N.A." ' Todo: MQ 20160919
		End Get
	End Property

	Public ReadOnly Property DigestValue As String Implements IComponentInputData.DigestValue
		Get
			Return ""
		End Get
	End Property

	Public ReadOnly Property IntegrityStatus As IntegrityStatus Implements IComponentInputData.IntegrityStatus
		Get
			Return IntegrityStatus.NotChecked
		End Get
	End Property

	Public ReadOnly Property IComponentInputData_ModelName As String Implements IComponentInputData.ModelName
		Get
			Return ModelName
		End Get
	End Property

	Public ReadOnly Property IEngineDeclarationInputData_Displacement As CubicMeter _
		Implements IEngineDeclarationInputData.Displacement
		Get
			Return (Displacement / 1000.0 / 1000.0).SI(Of CubicMeter)()
		End Get
	End Property

	Public ReadOnly Property IEngineDeclarationInputData_IdleSpeed As PerSecond _
		Implements IEngineDeclarationInputData.IdleSpeed
		Get
			Return IdleSpeed.RPMtoRad()
		End Get
	End Property

	Public ReadOnly Property WHTCMotorway As Double Implements IEngineDeclarationInputData.WHTCMotorway
		Get
			Return WHTCMotorwayInput
		End Get
	End Property

	Public ReadOnly Property WHTCRural As Double Implements IEngineDeclarationInputData.WHTCRural
		Get
			Return WHTCRuralInput
		End Get
	End Property

	Public ReadOnly Property WHTCUrban As Double Implements IEngineDeclarationInputData.WHTCUrban
		Get
			Return WHTCUrbanInput
		End Get
	End Property

	Public ReadOnly Property ColdHotBalancingFactor As Double Implements IEngineDeclarationInputData.ColdHotBalancingFactor
		Get
			Return ColdHotBalancingFactorInput
		End Get
	End Property

	Public ReadOnly Property FuelConsumptionMap As TableData Implements IEngineDeclarationInputData.FuelConsumptionMap
		Get
			If Not File.Exists(_fuelConsumptionMapPath.FullPath) Then _
				Throw New VectoException("FuelConsumptionMap is missing or invalid")
			Return VectoCSVFile.Read(_fuelConsumptionMapPath.FullPath)
		End Get
	End Property

	Public ReadOnly Property FullLoadCurve As TableData Implements IEngineDeclarationInputData.FullLoadCurve
		Get
			If Not File.Exists(_fullLoadCurvePath.FullPath) Then _
				Throw New VectoException("Full-Load Curve is missing or invalid")
			Return VectoCSVFile.Read(_fullLoadCurvePath.FullPath)
		End Get
	End Property

	Public ReadOnly Property Inertia As KilogramSquareMeter Implements IEngineEngineeringInputData.Inertia
		Get
			Return EngineInertia.SI(Of KilogramSquareMeter)()
		End Get
	End Property

	Public ReadOnly Property WHTCEngineering As Double Implements IEngineEngineeringInputData.WHTCEngineering
		Get
			Return WHTCEngineeringInput
		End Get
	End Property

#End Region
End Class


