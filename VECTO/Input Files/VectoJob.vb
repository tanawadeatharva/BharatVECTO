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
'Option Infer On
'Option Explicit On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports System.Text
Imports Newtonsoft.Json.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.Simulation.Data
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(VectoJob), "ValidateJob")>
Public Class VectoJob
	Implements IEngineeringInputDataProvider, IDeclarationInputDataProvider, IEngineeringJobInputData, 
				IDeclarationJobInputData, IDriverEngineeringInputData, IDriverDeclarationInputData, IAuxiliariesEngineeringInputData, 
				IAuxiliariesDeclarationInputData

	'AA-TB
	'STORES THE Type and version of the chosen or default Auxiliary Type ( Classic/Original or other )
	Public AuxiliaryAssembly As String
	Public AuxiliaryVersion As String
	Public AdvancedAuxiliaryFilePath As String

	Private _sFilePath As String
	Private _myPath As String

	'Input parameters
	Private ReadOnly _vehicleFile As SubPath
	Private ReadOnly _engineFile As SubPath
	Private ReadOnly _gearboxFile As SubPath

	Private _startStop As Boolean
	Public StartStopDelay As Double

	Private ReadOnly _driverAccelerationFile As SubPath

	Public ReadOnly AuxPaths As Dictionary(Of String, AuxEntry)
	'Alle Nebenverbraucher die in der Veh-Datei UND im Zyklus definiert sind

	Public ReadOnly CycleFiles As List(Of SubPath)

	Public EngineOnly As Boolean

	Public VMin As Double
	Public LookAheadOn As Boolean
	Public OverSpeedOn As Boolean
	Public OverSpeed As Double
	Public UnderSpeed As Double
	Public EcoRollOn As Boolean

	Public SavedInDeclMode As Boolean
	Private _vehicleInputData As JSONComponentInputData
	Private _engineInputData As JSONComponentInputData
	Private _gearboxInputData As JSONComponentInputData
	Public LookAheadMinSpeed As Double

	Public Property StartStopMaxSpeed As Double

	Public Property StartStopTime As Double

	Public Class AuxEntry
		Public Type As String
		Public ReadOnly Path As SubPath
		Public ReadOnly TechnologyList As List(Of String)

		Public Sub New()
			Path = New SubPath
			TechnologyList = New List(Of String)()
		End Sub
	End Class

	Public Sub New()

		_myPath = ""
		_sFilePath = ""

		_vehicleFile = New SubPath
		_engineFile = New SubPath
		_gearboxFile = New SubPath

		_driverAccelerationFile = New SubPath

		AuxPaths = New Dictionary(Of String, AuxEntry)

		CycleFiles = New List(Of SubPath)
	End Sub

	Public Function SaveFile() As Boolean
		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save Vecto Job")
			Return False
		End If

		Return JSONFileWriter.Instance.SaveJob(Me, _sFilePath)
	End Function

	'This Sub reads those Input-files that do not have their own class, etc.


#Region "Properties"


	Public Property FilePath As String
		Get
			Return _sFilePath
		End Get
		Set(value As String)
			_sFilePath = value
			If _sFilePath = "" Then
				_myPath = ""
			Else
				_myPath = Path.GetDirectoryName(_sFilePath) & "\"
			End If
		End Set
	End Property


	Public Property PathVeh(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _vehicleFile.OriginalPath
			Else
				Return _vehicleFile.FullPath
			End If
		End Get
		Set(value As String)
			_vehicleFile.Init(_myPath, value)
		End Set
	End Property

	Public Property PathEng(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _engineFile.OriginalPath
			Else
				Return _engineFile.FullPath
			End If
		End Get
		Set(value As String)
			_engineFile.Init(_myPath, value)
		End Set
	End Property

	Public Property PathGbx(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _gearboxFile.OriginalPath
			Else
				Return _gearboxFile.FullPath
			End If
		End Get
		Set(value As String)
			_gearboxFile.Init(_myPath, value)
		End Set
	End Property


	Public ReadOnly Property IDriverDeclarationInputData_SavedInDeclarationMode As Boolean _
		Implements IDriverDeclarationInputData.SavedInDeclarationMode
		Get
			Return SavedInDeclMode
		End Get
	End Property

	Public Property StartStop As Boolean
		Get
			Return _startStop
		End Get
		Set(value As Boolean)
			_startStop = value
		End Set
	End Property

	Public ReadOnly Property IDriverEngineeringInputData_OverSpeedEcoRoll As IOverSpeedEcoRollEngineeringInputData _
		Implements IDriverEngineeringInputData.OverSpeedEcoRoll
		Get
			Dim mode As DriverMode = DriverMode.Off
			If EcoRollOn Then
				mode = DriverMode.EcoRoll
			ElseIf OverSpeedOn Then
				mode = DriverMode.Overspeed
			End If

			Return New OverSpeedEcoRollInputData() With {
				.Mode = mode,
				.MinSpeed = VMin.KMPHtoMeterPerSecond(),
				.OverSpeed = OverSpeed.KMPHtoMeterPerSecond(),
				.UnderSpeed = UnderSpeed.KMPHtoMeterPerSecond()
				}
		End Get
	End Property

	Public ReadOnly Property IDriverEngineeringInputData_StartStop As IStartStopEngineeringInputData _
		Implements IDriverEngineeringInputData.StartStop
		Get
			Return New StartStopInputData With {
				.Enabled = _startStop,
				.MaxSpeed = StartStopMaxSpeed.KMPHtoMeterPerSecond(),
				.MinTime = StartStopTime.SI(Of Second)(),
				.Delay = StartStopDelay.SI(Of Second)()
				}
		End Get
	End Property

	Public ReadOnly Property OverSpeedEcoRoll As IOverSpeedEcoRollDeclarationInputData _
		Implements IDriverDeclarationInputData.OverSpeedEcoRoll
		Get
			Return IDriverEngineeringInputData_OverSpeedEcoRoll
		End Get
	End Property

	Public ReadOnly Property AccelerationCurve As TableData Implements IDriverEngineeringInputData.AccelerationCurve
		Get
			If String.IsNullOrWhiteSpace(_driverAccelerationFile.FullPath) Then Return Nothing
			If Not File.Exists(_driverAccelerationFile.FullPath) Then
				Try
					Dim cycleDataRes As Stream =
							RessourceHelper.ReadStream(
								RessourceHelper.Namespace + "VACC." + _driverAccelerationFile.OriginalPath +
								VectoCore.Configuration.Constants.FileExtensions.DriverAccelerationCurve)
					Return VectoCSVFile.ReadStream(cycleDataRes)
				Catch ex As Exception
					Return Nothing
				End Try
			End If
			Return VectoCSVFile.Read(_driverAccelerationFile.FullPath)
		End Get
	End Property

	Public ReadOnly Property Lookahead As ILookaheadCoastingInputData Implements IDriverEngineeringInputData.Lookahead
		Get
			Dim lacTargetLookup As TableData =
					If(File.Exists(LacDfTargetSpeedFile), VectoCSVFile.Read(LacDfTargetSpeedFile), Nothing)
			Dim lacVdropLookup As TableData =
					If(File.Exists(LacDfVelocityDropFile), VectoCSVFile.Read(LacDfVelocityDropFile), Nothing)
			Return New LookAheadCoastingInputData With {
				.Enabled = LookAheadOn,
				.MinSpeed = LookAheadMinSpeed.KMPHtoMeterPerSecond(),
				.CoastingDecisionFactorScaling = LacDfScale,
				.CoastingDecisionFactorOffset = LacDfOffset,
				.LookaheadDistanceFactor = LacPreviewFactor,
				.CoastingDecisionFactorTargetSpeedLookup = lacTargetLookup,
				.CoastingDecisionFactorVelocityDropLookup = lacVdropLookup
				}
		End Get
	End Property


	Public Property DesMaxFile(Optional ByVal original As Boolean = False) As String
		Get
			If original Then
				Return _driverAccelerationFile.OriginalPath
			Else
				Return _driverAccelerationFile.FullPath
			End If
		End Get
		Set(value As String)
			_driverAccelerationFile.Init(_myPath, value)
		End Set
	End Property

	Public Property LacPreviewFactor As Double
	Public Property LacDfOffset As Double
	Public Property LacDfScale As Double
	Public Property LacDfTargetSpeedFile As String
	Public Property LacDfVelocityDropFile As String


#End Region

	' ReSharper disable once UnusedMember.Global -- used by Validation
	Public Shared Function ValidateJob(vectoJob As VectoJob, validationContext As ValidationContext) As ValidationResult
		Dim modeService As ExecutionModeServiceContainer = TryCast(validationContext.GetService(GetType(ExecutionMode)), 
																	ExecutionModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

		If mode = ExecutionMode.Engineering AndAlso vectoJob.EngineOnly Then
			Return ValidateEngineOnlyJob(vectoJob, mode)
		End If

		Return ValidateVehicleJob(vectoJob, mode)
	End Function

	Private Shared Function ValidateEngineOnlyJob(vectoJob As VectoJob, executionMode As ExecutionMode) As ValidationResult
		Dim result As IList(Of ValidationResult) = New List(Of ValidationResult)

		vectoJob._engineInputData = New JSONComponentInputData(vectoJob._engineFile.FullPath)

		If vectoJob._engineInputData.EngineInputData Is Nothing Then _
			result.Add(New ValidationResult("Engine File is missing or invalid"))
		If result.Any() Then
			Return _
				New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
		End If

		Return ValidationResult.Success
	End Function

	Private Shared Function ValidateVehicleJob(vectoJob As VectoJob, mode As ExecutionMode) As ValidationResult

		Dim jobData As VectoRunData

		vectoJob._vehicleInputData = New JSONComponentInputData(vectoJob._vehicleFile.FullPath)
		vectoJob._engineInputData = New JSONComponentInputData(vectoJob._engineFile.FullPath)
		vectoJob._gearboxInputData = New JSONComponentInputData(vectoJob._gearboxFile.FullPath)


		Dim result As IList(Of ValidationResult) = New List(Of ValidationResult)

		If vectoJob._vehicleInputData.VehicleInputData Is Nothing Then _
			result.Add(New ValidationResult("Vehicle File is missing or invalid"))
		If vectoJob._engineInputData.EngineInputData Is Nothing Then _
			result.Add(New ValidationResult("Engine File is missing or invalid"))
		If vectoJob._gearboxInputData.GearboxInputData Is Nothing Then _
			result.Add(New ValidationResult("Gearbox File is missing or invalid"))

		If result.Any() Then
			Return _
				New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
		End If
		Try
			If mode = ExecutionMode.Declaration Then
				If Not vectoJob._vehicleInputData.VehicleInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Vehicle File is not in Declaration Mode"))
				End If
				If Not vectoJob._engineInputData.EngineInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Engine File is not in Declaration Mode"))
				End If
				If Not vectoJob._gearboxInputData.GearboxInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Gearbox File is not in Declaration Mode"))
				End If
				If result.Any() Then
					Return _
						New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
				End If

				Dim dataFactory As DeclarationModeVectoRunDataFactory = New DeclarationModeVectoRunDataFactory(vectoJob, Nothing)

				jobData = dataFactory.NextRun().First()
			Else
				If vectoJob._vehicleInputData.VehicleInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Vehicle File is not in Engineering Mode"))
				End If
				If vectoJob._engineInputData.EngineInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Engine File is not in Engineering Mode"))
				End If
				If vectoJob._gearboxInputData.GearboxInputData.SavedInDeclarationMode Then
					result.Add(New ValidationResult("Gearbox File is not in Engineering Mode"))
				End If
				If result.Any() Then
					Return _
						New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
				End If
				Dim dataFactory As EngineeringModeVectoRunDataFactory = New EngineeringModeVectoRunDataFactory(vectoJob)
				jobData = dataFactory.NextRun().First()
			End If


			result = jobData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))
			If result.Any() Then
				Return _
					New ValidationResult("Vecto Job Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
			End If


			Return ValidationResult.Success

		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		Finally
			vectoJob._vehicleInputData = Nothing
			vectoJob._engineInputData = Nothing
			vectoJob._gearboxInputData = Nothing
		End Try
	End Function


#Region "IInputData"

	Public Function JobInputData() As IEngineeringJobInputData Implements IEngineeringInputDataProvider.JobInputData
		Return Me
	End Function

	Public ReadOnly Property IDeclarationInputDataProvider_VehicleInputData As IVehicleDeclarationInputData _
		Implements IDeclarationInputDataProvider.VehicleInputData
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public Function IDeclarationInputDataProvider_JobInputData() As IDeclarationJobInputData _
		Implements IDeclarationInputDataProvider.JobInputData
		Return Me
	End Function

	Public ReadOnly Property VehicleInputData As IVehicleEngineeringInputData _
		Implements IEngineeringInputDataProvider.VehicleInputData
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_GearboxInputData As IGearboxDeclarationInputData _
		Implements IDeclarationInputDataProvider.GearboxInputData
		Get
			Return _gearboxInputData.GearboxInputData
		End Get
	End Property

	Public ReadOnly Property GearboxInputData As IGearboxEngineeringInputData _
		Implements IEngineeringInputDataProvider.GearboxInputData
		Get
			Return _gearboxInputData.GearboxInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_TorqueConverterInputData As ITorqueConverterDeclarationInputData _
		Implements IDeclarationInputDataProvider.TorqueConverterInputData
		Get
			Return _gearboxInputData.TorqueConverterInputData
		End Get
	End Property

	Public ReadOnly Property TorqueConverterInputData As ITorqueConverterEngineeringInputData _
		Implements IEngineeringInputDataProvider.TorqueConverterInputData
		Get
			Return _gearboxInputData.TorqueConverterInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_AxleGearInputData As IAxleGearInputData _
		Implements IDeclarationInputDataProvider.AxleGearInputData
		Get
			Return _gearboxInputData.AxleGearInputData
		End Get
	End Property

	Public ReadOnly Property AxleGearInputData As IAxleGearInputData _
		Implements IEngineeringInputDataProvider.AxleGearInputData
		Get
			Return _gearboxInputData.AxleGearInputData
		End Get
	End Property

	Public ReadOnly Property DeclarationInputDataProviderAngledriveInputData As IAngledriveInputData _
		Implements IDeclarationInputDataProvider.AngledriveInputData
		Get
			Return _vehicleInputData.AngledriveInputData
		End Get
	End Property

	Public ReadOnly Property AngledriveInputData As IAngledriveInputData _
		Implements IEngineeringInputDataProvider.AngledriveInputData
		Get
			Return _vehicleInputData.AngledriveInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_EngineInputData As IEngineDeclarationInputData _
		Implements IDeclarationInputDataProvider.EngineInputData
		Get
			Return _engineInputData.EngineInputData
		End Get
	End Property

	Public ReadOnly Property EngineInputData As IEngineEngineeringInputData _
		Implements IEngineeringInputDataProvider.EngineInputData
		Get
			Return _engineInputData.EngineInputData
		End Get
	End Property

	Public Function AuxiliaryInputData() As IAuxiliariesEngineeringInputData _
		Implements IEngineeringInputDataProvider.AuxiliaryInputData

		Return _vehicleInputData.AuxiliaryInputData()
	End Function

	Public Function IDeclarationInputDataProvider_AuxiliaryInputData() As IAuxiliariesDeclarationInputData _
		Implements IDeclarationInputDataProvider.AuxiliaryInputData

		Return Me
	End Function

	Public ReadOnly Property IDeclarationInputDataProvider_RetarderInputData As IRetarderInputData _
		Implements IDeclarationInputDataProvider.RetarderInputData
		Get
			Return _vehicleInputData.RetarderInputData
		End Get
	End Property

	Public ReadOnly Property RetarderInputData As IRetarderInputData _
		Implements IEngineeringInputDataProvider.RetarderInputData
		Get
			Return _vehicleInputData.RetarderInputData
		End Get
	End Property

	Public ReadOnly Property IDeclarationInputDataProvider_DriverInputData As IDriverDeclarationInputData _
		Implements IDeclarationInputDataProvider.DriverInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property DriverInputData As IDriverEngineeringInputData _
		Implements IEngineeringInputDataProvider.DriverInputData
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property PTOTransmissionInputData As IPTOTransmissionInputData _
		Implements IEngineeringInputDataProvider.PTOTransmissionInputData
		Get
			Return _vehicleInputData.PTOTransmissionInputData
		End Get
	End Property


	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IDeclarationJobInputData.SavedInDeclarationMode
		Get
			Return SavedInDeclMode
		End Get
	End Property


	Public ReadOnly Property IDriverDeclarationInputData_StartStop As IStartStopDeclarationInputData _
		Implements IDriverDeclarationInputData.StartStop
		Get
			Return IDriverEngineeringInputData_StartStop
		End Get
	End Property

	Public ReadOnly Property IEngineeringJobInputData_Vehicle As IVehicleEngineeringInputData _
		Implements IEngineeringJobInputData.Vehicle
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public ReadOnly Property Vehicle As IVehicleDeclarationInputData Implements IDeclarationJobInputData.Vehicle
		Get
			Return _vehicleInputData.VehicleInputData
		End Get
	End Property

	Public ReadOnly Property Cycles As IList(Of ICycleData) Implements IEngineeringJobInputData.Cycles
		Get
			Dim retVal As ICycleData() = New ICycleData(CycleFiles.Count) {}
			Dim i As Integer = 0
			For Each cycleFile As SubPath In CycleFiles
				Dim cycleData As TableData
				If (File.Exists(cycleFile.FullPath)) Then
					cycleData = VectoCSVFile.Read(cycleFile.FullPath)
				Else
					Try
						Dim cycleDataRes As Stream =
								RessourceHelper.ReadStream(RessourceHelper.Namespace + "MissionCycles." + cycleFile.OriginalPath + ".vdri")
						cycleData = VectoCSVFile.ReadStream(cycleDataRes)
					Catch ex As Exception
						Throw New VectoException("Driving Cycle could not be read: " + cycleFile.OriginalPath)
					End Try
				End If
				retVal(i) = New CycleInputData With {
					.Name = Path.GetFileNameWithoutExtension(cycleFile.FullPath),
					.CycleData = cycleData
					}
				i += 1
			Next
			Return retVal
		End Get
	End Property

	Public ReadOnly Property EngineOnlyMode As Boolean Implements IEngineeringJobInputData.EngineOnlyMode
		Get
			Return EngineOnly
		End Get
	End Property

	Public ReadOnly Property JobName As String Implements IDeclarationJobInputData.JobName
		Get
			Return Path.GetFileNameWithoutExtension(FilePath)
		End Get
	End Property

	Public Property AuxPAdd As Double

	Public ReadOnly Property IAuxiliariesDeclarationInputData_SavedInDeclarationMode As Boolean _
		Implements IAuxiliariesDeclarationInputData.SavedInDeclarationMode
		Get
			Return SavedInDeclMode
		End Get
	End Property

	Public ReadOnly Property Auxiliaries As IList(Of IAuxiliaryEngineeringInputData) _
		Implements IAuxiliariesEngineeringInputData.Auxiliaries
		Get
			Return AuxData().Cast(Of IAuxiliaryEngineeringInputData).ToList()
		End Get
	End Property

	Public ReadOnly Property IAuxiliariesEngineeringInputData_AdvancedAuxiliaryFilePath As String _
		Implements IAuxiliariesEngineeringInputData.AdvancedAuxiliaryFilePath
		Get
			Return AdvancedAuxiliaryFilePath
		End Get
	End Property

	Public ReadOnly Property IAuxiliariesEngineeringInputData_AuxiliaryVersion As String _
		Implements IAuxiliariesEngineeringInputData.AuxiliaryVersion
		Get
			Return AuxiliaryVersion
		End Get
	End Property

	Public ReadOnly Property IAuxiliariesEngineeringInputData_AuxiliaryAssembly As AuxiliaryModel _
		Implements IAuxiliariesEngineeringInputData.AuxiliaryAssembly
		Get
			Return AuxiliaryModelHelper.Parse(AuxiliaryAssembly)
		End Get
	End Property

	Public ReadOnly Property IAuxiliariesDeclarationInputData_Auxiliaries As IList(Of IAuxiliaryDeclarationInputData) _
		Implements IAuxiliariesDeclarationInputData.Auxiliaries
		Get
			Return AuxData().Cast(Of IAuxiliaryDeclarationInputData).ToList()
		End Get
	End Property

	Protected Function AuxData() As IList(Of AuxiliaryDataInputData)
		Dim retVal As List(Of AuxiliaryDataInputData) = New List(Of AuxiliaryDataInputData)

		If AuxPAdd > 0 Then
			retVal.Add(New AuxiliaryDataInputData() With {
						.ID = "ConstantAux",
						.AuxiliaryType = AuxiliaryDemandType.Constant,
						.ConstantPowerDemand = AuxPAdd.SI(Of Watt)()
						})
		End If
		For Each auxEntry As KeyValuePair(Of String, AuxEntry) In AuxPaths
			Dim theAuxData As AuxiliaryDataInputData = New AuxiliaryDataInputData() With {
					.Type = AuxiliaryTypeHelper.Parse(auxEntry.Value.Type),
					.Technology = auxEntry.Value.TechnologyList,
					.ID = auxEntry.Key
					}
			retVal.Add(theAuxData)
			If Not File.Exists(auxEntry.Value.Path.FullPath) Then Continue For

			Dim stream As StreamReader = New StreamReader(auxEntry.Value.Path.FullPath)
			stream.ReadLine() ' skip header "Transmission ration to engine rpm [-]"
			theAuxData.TransmissionRatio = stream.ReadLine().IndulgentParse()
			stream.ReadLine() ' skip header "Efficiency to engine [-]"
			theAuxData.EfficiencyToEngine = stream.ReadLine().IndulgentParse()
			stream.ReadLine() ' skip header "Efficiency auxiliary to supply [-]"
			theAuxData.EfficiencyToSupply = stream.ReadLine().IndulgentParse()
			theAuxData.DemandMap = VectoCSVFile.ReadStream(New MemoryStream(Encoding.UTF8.GetBytes(stream.ReadToEnd())))
		Next

		Return retVal
	End Function

#End Region
End Class


