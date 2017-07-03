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
'Option Infer On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(Vehicle), "ValidateVehicle")>
Public Class Vehicle
	Implements IVehicleEngineeringInputData, IVehicleDeclarationInputData, IRetarderInputData, IPTOTransmissionInputData, 
				IAngledriveInputData, IAirdragEngineeringInputData

	Private _filePath As String
	Private _path As String

	Public Mass As Double
	Public Loading As Double

	Public CdA0 As Double

	Public CrossWindCorrectionMode As CrossWindCorrectionMode
	Public ReadOnly CrossWindCorrectionFile As SubPath

	<ValidateObject> Public RetarderType As RetarderType
	Public RetarderRatio As Double = 0
	Public ReadOnly RetarderLossMapFile As SubPath

	Public DynamicTyreRadius As Double
	Public ReadOnly Axles As List(Of AxleInputData)


	Public VehicleCategory As VehicleCategory
	Public MassExtra As Double
	Public MassMax As Double
	Public AxleConfiguration As AxleConfiguration
	Public SavedInDeclMode As Boolean

	Public AngledriveType As AngledriveType
	Public AngledriveRatio As Double
	Public ReadOnly AngledriveLossMapFile As SubPath

	Public PtoType As String
	Public ReadOnly PtoLossMap As SubPath
	Public ReadOnly PtoCycle As SubPath
	Public torqueLimitsList As List(Of ITorqueLimitInputData)
	Public VehicleidlingSpeed As PerSecond
	Public legClass As LegislativeClass


	Public Sub New()
		_path = ""
		_filePath = ""
		CrossWindCorrectionFile = New SubPath

		RetarderLossMapFile = New SubPath
		AngledriveLossMapFile = New SubPath()

		Axles = New List(Of AxleInputData)
		torqueLimitsList = New List(Of ITorqueLimitInputData)
		PtoLossMap = New SubPath()
		PtoCycle = New SubPath()
		SetDefault()
	End Sub


	' ReSharper disable once UnusedMember.Global  -- used for Validation
	Public Shared Function ValidateVehicle(vehicle As Vehicle, validationContext As ValidationContext) As ValidationResult

		Dim vehicleData As VehicleData
		Dim airdragData As AirdragData
		Dim retarderData As RetarderData
		Dim ptoData As PTOData = Nothing
		Dim angledriveData As AngledriveData

		Dim modeService As VectoValidationModeServiceContainer =
				TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)), 
						VectoValidationModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
		Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
		Dim gbxType As GearboxType? = If(modeService Is Nothing, Nothing, modeService.GearboxType)

		Try
			If mode = ExecutionMode.Declaration Then
				Dim doa As DeclarationDataAdapter = New DeclarationDataAdapter()
				Dim segment As Segment = DeclarationData.Segments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration,
																		vehicle.GrossVehicleMassRating, vehicle.CurbMassChassis)
				vehicleData = doa.CreateVehicleData(vehicle, segment.Missions.First(),
													segment.Missions.First().Loadings.First().Value, segment.MunicipalBodyWeight)
				airdragData = doa.CreateAirdragData(vehicle, segment.Missions.First(), segment)
				retarderData = doa.CreateRetarderData(vehicle)
				angledriveData = doa.CreateAngledriveData(vehicle, False)
				ptoData = doa.CreatePTOTransmissionData(vehicle)
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				vehicleData = doa.CreateVehicleData(vehicle)
				airdragData = doa.CreateAirdragData(vehicle, vehicle)
				retarderData = doa.CreateRetarderData(vehicle)
				angledriveData = doa.CreateAngledriveData(vehicle, True)
				ptoData = doa.CreatePTOTransmissionData(vehicle)
			End If

			Dim result As IList(Of ValidationResult) =
					vehicleData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Vehicle Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			result = airdragData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType,
										emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Airdrag Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			result = retarderData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType,
											emsCycle)
			If result.Any() Then
				Return _
					New ValidationResult("Retarder Configuration is invalid. ",
										result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
			End If

			If vehicle.AngledriveType = AngledriveType.SeparateAngledrive Then
				result = angledriveData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType,
												emsCycle)
				If result.Any() Then
					Return _
						New ValidationResult("AngleDrive Configuration is invalid. ",
											result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
				End If
			End If

			If Not vehicle.PTOTransmissionType = "None" Then
				result = ptoData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)
				If result.Any() Then
					Return _
						New ValidationResult("PTO Configuration is invalid. ",
											result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
				End If
			End If

			Return ValidationResult.Success

		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		End Try
	End Function

	Private Sub SetDefault()
		Mass = 0
		MassExtra = 0
		Loading = 0
		CdA0 = 0
		CrossWindCorrectionFile.Clear()
		CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection

		DynamicTyreRadius = 0

		RetarderType = RetarderType.None
		RetarderRatio = 1
		RetarderLossMapFile.Clear()
		AngledriveLossMapFile.Clear()

		AngledriveType = AngledriveType.None
		AngledriveLossMapFile.Clear()
		AngledriveRatio = 1

		PtoType = PTOTransmission.NoPTO
		PtoLossMap.Clear()
		PtoCycle.Clear()

		Axles.Clear()
		VehicleCategory = VehicleCategory.RigidTruck
		MassMax = 0
		AxleConfiguration = AxleConfiguration.AxleConfig_4x2

		SavedInDeclMode = False
	End Sub


	Public Function SaveFile() As Boolean
		SavedInDeclMode = Cfg.DeclMode

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), Nothing, False)

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save vehicle")
			Return False
		End If

		Try
			Dim writer As JSONFileWriter = JSONFileWriter.Instance
			writer.SaveVehicle(Me, Me, Me, Me, Me, _filePath)
		Catch ex As Exception
			MsgBox("Failed to save Vehicle file: " + ex.Message)
			Return False
		End Try
		Return True
	End Function


#Region "Properties"


	Public Property FilePath() As String
		Get
			Return _filePath
		End Get
		Set(value As String)
			_filePath = value
			If _filePath = "" Then
				_path = ""
			Else
				_path = Path.GetDirectoryName(_filePath) & "\"
			End If
		End Set
	End Property

#End Region

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

	Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
		Get
			' Just for the interface. Value is not available in GUI yet.
			Return "N.A."
		End Get
	End Property

	Public ReadOnly Property Model As String Implements IComponentInputData.Model
		Get
			' Just for the interface. Value is not available in GUI yet.
			Return "N.A."
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
			Return "N.A."
		End Get
	End Property

	Public ReadOnly Property DigestValue As String Implements IComponentInputData.DigestValue
		Get
			Return ""
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_VehicleCategory As VehicleCategory _
		Implements IVehicleDeclarationInputData.VehicleCategory
		Get
			Return VehicleCategory
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_AxleConfiguration As AxleConfiguration _
		Implements IVehicleDeclarationInputData.AxleConfiguration
		Get
			Return AxleConfiguration
		End Get
	End Property

	Public ReadOnly Property VIN As String Implements IVehicleDeclarationInputData.VIN
		Get
			Return "N.A."
		End Get
	End Property

	Public ReadOnly Property LegislativeClass As LegislativeClass Implements IVehicleEngineeringInputData.LegislativeClass
		Get
			Return legClass
		End Get
	End Property

	Public ReadOnly Property CurbMassChassis As Kilogram Implements IVehicleDeclarationInputData.CurbMassChassis
		Get
			Return Mass.SI(Of Kilogram)()
		End Get
	End Property

	Public ReadOnly Property GrossVehicleMassRating As Kilogram _
		Implements IVehicleDeclarationInputData.GrossVehicleMassRating
		Get
			Return MassMax.SI().Ton.Cast(Of Kilogram)()
		End Get
	End Property

	Public ReadOnly Property TorqueLimits As IList(Of ITorqueLimitInputData) _
		Implements IVehicleDeclarationInputData.TorqueLimits
		Get
			Return torqueLimitsList
		End Get
	End Property

	Public ReadOnly Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
		Get
			Return "N.A."
		End Get
	End Property

	Public ReadOnly Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
		Get
			Return VehicleidlingSpeed
		End Get
	End Property

	Public ReadOnly Property AirDragArea As SquareMeter Implements IAirdragEngineeringInputData.AirDragArea
		Get
			Return If(Double.IsNaN(CdA0), Nothing, CdA0.SI(Of SquareMeter)())
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_Axles As IList(Of IAxleEngineeringInputData) _
		Implements IVehicleEngineeringInputData.Axles
		Get
			Return Axles.Cast(Of IAxleEngineeringInputData)().ToList()
		End Get
	End Property

	Public ReadOnly Property IVehicleDeclarationInputData_Axles As IList(Of IAxleDeclarationInputData) _
		Implements IVehicleDeclarationInputData.Axles
		Get
			Return Axles.Cast(Of IAxleDeclarationInputData)().ToList()
		End Get
	End Property


	Public ReadOnly Property CurbMassExtra As Kilogram Implements IVehicleEngineeringInputData.CurbMassExtra
		Get
			Return MassExtra.SI(Of Kilogram)()
		End Get
	End Property

	Public ReadOnly Property CrosswindCorrectionMap As TableData _
		Implements IAirdragEngineeringInputData.CrosswindCorrectionMap
		Get
			Return VectoCSVFile.Read(CrossWindCorrectionFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_CrossWindCorrectionMode As CrossWindCorrectionMode _
		Implements IAirdragEngineeringInputData.CrossWindCorrectionMode
		Get
			Return CrossWindCorrectionMode
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_DynamicTyreRadius As Meter _
		Implements IVehicleEngineeringInputData.DynamicTyreRadius
		Get
			Return DynamicTyreRadius.SI().Milli.Meter.Cast(Of Meter)()
		End Get
	End Property

	Public ReadOnly Property IVehicleEngineeringInputData_Loading As Kilogram _
		Implements IVehicleEngineeringInputData.Loading
		Get
			Return Loading.SI(Of Kilogram)()
		End Get
	End Property


	Public ReadOnly Property Type As RetarderType Implements IRetarderInputData.Type
		Get
			Return RetarderType
		End Get
	End Property

	Public ReadOnly Property IRetarderInputData_Ratio As Double Implements IRetarderInputData.Ratio
		Get
			Return RetarderRatio
		End Get
	End Property

	Public ReadOnly Property Ratio As Double Implements IAngledriveInputData.Ratio
		Get
			Return AngledriveRatio
		End Get
	End Property

	Public ReadOnly Property IRetarderInputData_LossMap As TableData Implements IRetarderInputData.LossMap
		Get
			Return VectoCSVFile.Read(RetarderLossMapFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property AngledriveInputDataType As AngledriveType Implements IAngledriveInputData.Type
		Get
			Return AngledriveType
		End Get
	End Property


	Public ReadOnly Property LossMap As TableData Implements IAngledriveInputData.LossMap
		Get
			Return VectoCSVFile.Read(AngledriveLossMapFile.FullPath)
		End Get
	End Property


	Public ReadOnly Property Efficiency As Double Implements IAngledriveInputData.Efficiency
		Get
			Return If(IsNumeric(AngledriveLossMapFile.OriginalPath), AngledriveLossMapFile.OriginalPath.ToDouble(), -1.0)
		End Get
	End Property

#End Region

	Public ReadOnly Property PTOTransmissionType As String Implements IPTOTransmissionInputData.PTOTransmissionType
		Get
			Return PtoType
		End Get
	End Property

	Public ReadOnly Property IPTOTransmissionInputData_PTOCycle As TableData Implements IPTOTransmissionInputData.PTOCycle
		Get
			If String.IsNullOrWhiteSpace(PtoCycle.FullPath) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(PtoCycle.FullPath)
		End Get
	End Property

	Public ReadOnly Property IPTOTransmissionInputData_PTOLossMap As TableData _
		Implements IPTOTransmissionInputData.PTOLossMap
		Get
			If String.IsNullOrWhiteSpace(PtoCycle.FullPath) Then
				Return Nothing
			End If
			Return VectoCSVFile.Read(PtoLossMap.FullPath)
		End Get
	End Property
End Class