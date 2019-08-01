' Copyright 2017 European Union.
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
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Utils

''' <summary>
''' Engine input file
''' </summary>
''' <remarks></remarks>
<CustomValidation(GetType(Engine), "ValidateEngine")>
Public Class Engine
	Implements IEngineEngineeringInputData, IEngineDeclarationInputData, IEngineModeDeclarationInputData, IEngineFuelDelcarationInputData

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
	Public correctionFactorRegPerInput As Double
	Public FuelTypeInput As FuelType
	Public ratedPowerInput As Watt
	Public ratedSpeedInput As PerSecond
	Public maxTorqueInput As NewtonMeter


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
	''' </summary>
	''' <returns>True if successful.</returns>
	''' <remarks></remarks>
	Public Function SaveFile() As Boolean

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), Nothing, False)

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save engine")
			Return False
		End If

		Try
			Dim writer As JSONFileWriter = New JSONFileWriter()
			writer.SaveEngine(Me, _filePath)

		Catch ex As Exception
			MsgBox("Faled to write Engine file: " + ex.Message)
			Return False
		End Try
		Return True
	End Function


	''' <summary>
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


		Dim modeService As VectoValidationModeServiceContainer =
				TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)), 
						VectoValidationModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
		Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
		Dim gbxType As GearboxType? = If(modeService Is Nothing, GearboxType.MT, modeService.GearboxType)

		Try
			If mode = ExecutionMode.Declaration Then
				Dim doa As DeclarationDataAdapter = New DeclarationDataAdapter()
				Dim dummyGearboxData As IGearboxDeclarationInputData = New Gearbox() With {
						.Type = GearboxType.AMT,
						.MaxTorque = New List(Of String),
						.GearRatios = New List(Of Double)()
						}
				dim dummyVehicle as IVehicleDeclarationInputData = New DummyVehicle() With {
					.GearboxInputData = dummyGearboxData,
					.EngineInputData = engine
				}
				engineData = doa.CreateEngineData(dummyVehicle, engine.EngineModes.First(), New Mission() With {.MissionType = MissionType.LongHaul})
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				engineData = doa.CreateEngineData(engine, Nothing, New List(Of ITorqueLimitInputData), Nothing, TankSystem.Compressed)
			End If

			Dim result As IList(Of ValidationResult) =
					engineData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)

			If Not result.Any() Then Return ValidationResult.Success

			Return New ValidationResult("Engine Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		End Try
	End Function

#Region "IInputData"

	Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
		Get
			Dim retVal As DataSource =  New DataSource() 
			retVal.SourceType = DataSourceType.JSONFile
			retVal.SourceFile = FilePath
			Return retVal
		End Get
	End Property

	
	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
		Get
			Return Cfg.DeclMode
		End Get
	End Property

	Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
		Get
			' Just for the interface. Value is not available in GUI yet.
			Return TUGraz.VectoCore.Configuration.Constants.NOT_AVailABLE
		End Get
	End Property

	Public ReadOnly Property [Date] As String Implements IComponentInputData.[Date]
		Get
			Return Now.ToUniversalTime().ToString("o")
		End Get
	End Property

	Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
		Get
			Return CertificationMethod.NotCertified
		End Get
	End Property

	Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
		Get
			' Just for the interface. Value is not available in GUI yet.
			Return TUGraz.VectoCore.Configuration.Constants.NOT_AVailABLE
		End Get
	End Property

	Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
		Get
			Return Nothing
		End Get
	End Property

	Public ReadOnly Property Model As String Implements IComponentInputData.Model
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

	Public ReadOnly Property IEngineModeDeclarationInputData_IdleSpeed As PerSecond _
		Implements IEngineModeDeclarationInputData.IdleSpeed
		Get
			Return IdleSpeed.RPMtoRad()
		End Get
	End Property

	Public ReadOnly Property WHTCMotorway As Double Implements IEngineFuelDelcarationInputData.WHTCMotorway
		Get
			Return WHTCMotorwayInput
		End Get
	End Property

	Public ReadOnly Property WHTCRural As Double Implements IEngineFuelDelcarationInputData.WHTCRural
		Get
			Return WHTCRuralInput
		End Get
	End Property

	Public ReadOnly Property WHTCUrban As Double Implements IEngineFuelDelcarationInputData.WHTCUrban
		Get
			Return WHTCUrbanInput
		End Get
	End Property

	Public ReadOnly Property ColdHotBalancingFactor As Double Implements IEngineFuelDelcarationInputData.ColdHotBalancingFactor
		Get
			Return ColdHotBalancingFactorInput
		End Get
	End Property

	Public ReadOnly Property CorrectionFactorRegPer As Double Implements IEngineFuelDelcarationInputData.CorrectionFactorRegPer
		Get
			Return correctionFactorRegPerInput
		End Get
	End Property

	Public ReadOnly Property FuelType As FuelType Implements IEngineFuelDelcarationInputData.FuelType
		Get
			Return FuelTypeInput
		End Get
	End Property

	Public ReadOnly Property FuelConsumptionMap As TableData Implements IEngineFuelDelcarationInputData.FuelConsumptionMap
		Get
			If Not File.Exists(_fuelConsumptionMapPath.FullPath) Then _
				Throw New VectoException("FuelConsumptionMap is missing or invalid")
			Return VectoCSVFile.Read(_fuelConsumptionMapPath.FullPath)
		End Get
	End Property

	Public ReadOnly Property FullLoadCurve As TableData Implements IEngineModeDeclarationInputData.FullLoadCurve
		Get
			If Not File.Exists(_fullLoadCurvePath.FullPath) Then _
				Throw New VectoException("Full-Load Curve is missing or invalid")
			Return VectoCSVFile.Read(_fullLoadCurvePath.FullPath)
		End Get
	End Property

	Public ReadOnly Property Fuels As IList(Of IEngineFuelDelcarationInputData) Implements IEngineModeDeclarationInputData.Fuels
	Get
			Return new List(Of IEngineFuelDelcarationInputData)({me})
	End Get
	End Property

	Public ReadOnly Property RatedPowerDeclared As Watt Implements IEngineDeclarationInputData.RatedPowerDeclared
		Get
			Return ratedPowerInput
		End Get
	End Property

	Public ReadOnly Property RatedSpeedDeclared As PerSecond Implements IEngineDeclarationInputData.RatedSpeedDeclared
		Get
			Return ratedSpeedInput
		End Get
	End Property

	Public ReadOnly Property MaxTorqueDeclared As NewtonMeter Implements IEngineDeclarationInputData.MaxTorqueDeclared
		Get
			Return maxTorqueInput
		End Get
	End Property

	Public ReadOnly Property EngineModes As IList(Of IEngineModeDeclarationInputData) Implements IEngineDeclarationInputData.EngineModes
	get
			Return New List(Of IEngineModeDeclarationInputData)({me})
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

Public Class DummyVehicle
	Implements IVehicleDeclarationInputData, IVehicleComponentsDeclaration
	Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
	Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
	Public ReadOnly Property Model As String Implements IComponentInputData.Model
	Public ReadOnly Property [Date] As String Implements IComponentInputData.[Date]
	Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
	Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
	Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
	Public ReadOnly Property Identifier As String Implements IVehicleDeclarationInputData.Identifier
	Public ReadOnly Property ExemptedVehicle As Boolean Implements IVehicleDeclarationInputData.ExemptedVehicle
	Public ReadOnly Property VIN As String Implements IVehicleDeclarationInputData.VIN
	Public ReadOnly Property LegislativeClass As LegislativeClass Implements IVehicleDeclarationInputData.LegislativeClass
	Public ReadOnly Property VehicleCategory As VehicleCategory Implements IVehicleDeclarationInputData.VehicleCategory
	Public ReadOnly Property AxleConfiguration As AxleConfiguration Implements IVehicleDeclarationInputData.AxleConfiguration
	Public ReadOnly Property CurbMassChassis As Kilogram Implements IVehicleDeclarationInputData.CurbMassChassis
	Public ReadOnly Property GrossVehicleMassRating As Kilogram Implements IVehicleDeclarationInputData.GrossVehicleMassRating
	Public ReadOnly Property TorqueLimits As IList(Of ITorqueLimitInputData) Implements IVehicleDeclarationInputData.TorqueLimits
	get
			Return new List(Of ITorqueLimitInputData)()
	End Get
	End Property
	Public ReadOnly Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
	Public ReadOnly Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
	Public ReadOnly Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
	Public ReadOnly Property SleeperCab As Boolean Implements IVehicleDeclarationInputData.SleeperCab
	Public ReadOnly Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem
	Public ReadOnly Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData Implements IVehicleDeclarationInputData.ADAS
	Public ReadOnly Property ZeroEmissionVehicle As Boolean Implements IVehicleDeclarationInputData.ZeroEmissionVehicle
	Public ReadOnly Property HybridElectricHDV As Boolean Implements IVehicleDeclarationInputData.HybridElectricHDV
	Public ReadOnly Property DualFuelVehicle As Boolean Implements IVehicleDeclarationInputData.DualFuelVehicle
	Public ReadOnly Property MaxNetPower1 As Watt Implements IVehicleDeclarationInputData.MaxNetPower1
	Public ReadOnly Property MaxNetPower2 As Watt Implements IVehicleDeclarationInputData.MaxNetPower2
	Public ReadOnly Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
	get
			Return me
	End Get
	End Property

	Public ReadOnly Property AirdragInputData As IAirdragDeclarationInputData Implements IVehicleComponentsDeclaration.AirdragInputData
	Public Property GearboxInputData As IGearboxDeclarationInputData Implements IVehicleComponentsDeclaration.GearboxInputData
	Public ReadOnly Property TorqueConverterInputData As ITorqueConverterDeclarationInputData Implements IVehicleComponentsDeclaration.TorqueConverterInputData
	Public ReadOnly Property AxleGearInputData As IAxleGearInputData Implements IVehicleComponentsDeclaration.AxleGearInputData
	Public ReadOnly Property AngledriveInputData As IAngledriveInputData Implements IVehicleComponentsDeclaration.AngledriveInputData
	Public Property EngineInputData As IEngineDeclarationInputData Implements IVehicleComponentsDeclaration.EngineInputData
	Public ReadOnly Property AuxiliaryInputData As IAuxiliariesDeclarationInputData Implements IVehicleComponentsDeclaration.AuxiliaryInputData
	Public ReadOnly Property RetarderInputData As IRetarderInputData Implements IVehicleComponentsDeclaration.RetarderInputData
	Public ReadOnly Property PTOTransmissionInputData As IPTOTransmissionInputData Implements IVehicleComponentsDeclaration.PTOTransmissionInputData
	Public ReadOnly Property AxleWheels As IAxlesDeclarationInputData Implements IVehicleComponentsDeclaration.AxleWheels
End Class


