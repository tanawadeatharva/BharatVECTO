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
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(Gearbox), "ValidateGearbox")>
Public Class Gearbox
	Implements IGearboxEngineeringInputData, IGearboxDeclarationInputData, IAxleGearInputData, 
				ITorqueConverterEngineeringInputData, 
				ITorqueConverterDeclarationInputData

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

		SavedInDeclMode = Cfg.DeclMode

		Dim validationResults As IList(Of ValidationResult) =
				Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))

		If validationResults.Count > 0 Then
			Dim messages As IEnumerable(Of String) =
					validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input." + Environment.NewLine + String.Join("; ", messages), MsgBoxStyle.OkOnly,
					"Failed to save gearbox")
			Return False
		End If

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

		Dim ls As New List(Of Dictionary(Of String, Object))
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


	' ReSharper disable once UnusedMember.Global -- used by Validation
	Public Shared Function ValidateGearbox(gearbox As Gearbox, validationContext As ValidationContext) As ValidationResult
		Dim modeService As ExecutionModeServiceContainer = TryCast(validationContext.GetService(GetType(ExecutionMode)), 
																	ExecutionModeServiceContainer)
		Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)

		Dim axlegearData As AxleGearData
		Dim gearboxData As GearboxData

		Try
			'Dim vectoJob As VectoJob = New VectoJob() With {.FilePath = VectoJobForm.VECTOfile}
			Dim inputData As IEngineeringInputDataProvider =
					TryCast(JSONInputDataFactory.ReadComponentData(VectoJobForm.VECTOfile), 
							IEngineeringInputDataProvider)
			'Dim vehicle As IVehicleEngineeringInputData = inputData.VehicleInputData
			Dim engine As CombustionEngineData
			Dim rdyn As Meter = 0.5.SI(Of Meter)()
			If mode = ExecutionMode.Declaration Then
				Dim doa As DeclarationDataAdapter = New DeclarationDataAdapter()

				engine = doa.CreateEngineData(inputData.EngineInputData, gearbox.Type)

				axlegearData = doa.CreateAxleGearData(gearbox, False)
				gearboxData = doa.CreateGearboxData(gearbox, engine, axlegearData.AxleGear.Ratio, rdyn, False)
			Else
				Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
				engine = doa.CreateEngineData(inputData.EngineInputData, gearbox)
				axlegearData = doa.CreateAxleGearData(gearbox, True)
				gearboxData = doa.CreateGearboxData(gearbox, engine, axlegearData.AxleGear.Ratio, rdyn, True)
			End If

			Dim result As IList(Of ValidationResult) =
					gearboxData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))
			If result.Any() Then
				Return _
					New ValidationResult("Gearbox Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
			End If

			result = axlegearData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))
			If result.Any() Then
				Return _
					New ValidationResult("Gearbox Configuration is invalid. ", result.Select(Function(r) r.ErrorMessage).ToList())
			End If

			Return ValidationResult.Success

		Catch ex As Exception
			Return New ValidationResult(ex.Message)
		End Try
	End Function


	Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
		Get
			Return Cfg.DeclMode
		End Get
	End Property

	Public ReadOnly Property Vendor As String Implements IComponentInputData.Vendor
		Get
			Return "N.A."  ' Todo MQ 20160915
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
			Return "N.A." ' todo MQ 20160915
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
			Return "N.A." ' todo MQ 20160915
		End Get
	End Property

	Public ReadOnly Property IGearboxDeclarationInputData_Type As GearboxType Implements IGearboxDeclarationInputData.Type
		Get
			Return Type
		End Get
	End Property

	Public ReadOnly Property Gears As IList(Of ITransmissionInputData) Implements IGearboxDeclarationInputData.Gears
		Get
			Dim ls As IList(Of ITransmissionInputData) = New List(Of ITransmissionInputData)
			Dim i As Integer
			For i = 1 To GearRatios.Count - 1
				Dim gearDict As New TransmissionInputData With {
						.Ratio = GearRatios(i)
						}
				If File.Exists(GearshiftFiles(i).OriginalPath) Then
					gearDict.ShiftPolygon = VectoCSVFile.Read(GearshiftFiles(i).OriginalPath)
				End If
				If Not String.IsNullOrWhiteSpace(MaxTorque(i)) AndAlso IsNumeric(MaxTorque(i)) Then
					gearDict.MaxTorque = MaxTorque(i).ToDouble().SI(Of NewtonMeter)()
				End If
				If IsNumeric(GearLossMap(i, True)) Then
					gearDict.Efficiency = GearLossMap(i, True).ToDouble()
				Else
					gearDict.LossMap = VectoCSVFile.Read(GearLossmaps(i).PathOrDummy)
				End If

				ls.Add(gearDict)
			Next
			Return ls
		End Get
	End Property

	Public ReadOnly Property ReferenceRPM As PerSecond Implements ITorqueConverterEngineeringInputData.ReferenceRPM
		Get
			Return TorqueConverterReferenceRpm.RPMtoRad()
		End Get
	End Property

	Public ReadOnly Property ITorqueConverterEngineeringInputData_Inertia As KilogramSquareMeter _
		Implements ITorqueConverterEngineeringInputData.Inertia
		Get
			Return TorqueConverterInertia.SI(Of KilogramSquareMeter)()
		End Get
	End Property

	Public ReadOnly Property Inertia As KilogramSquareMeter Implements IGearboxEngineeringInputData.Inertia
		Get
			Return GbxInertia.SI(Of KilogramSquareMeter)()
		End Get
	End Property

	Public ReadOnly Property ShiftPolygon As TableData Implements ITorqueConverterEngineeringInputData.ShiftPolygon
		Get
			Return VectoCSVFile.Read(TorqueConverterShiftPolygonFile)
		End Get
	End Property

	Public ReadOnly Property TractionInterruption As Second Implements IGearboxEngineeringInputData.TractionInterruption
		Get
			Return TracIntrSi.SI(Of Second)()
		End Get
	End Property

	Public ReadOnly Property EarlyShiftUp As Boolean Implements IGearboxEngineeringInputData.EarlyShiftUp
		Get
			Return ShiftInside
		End Get
	End Property

	Public ReadOnly Property TorqueReserve As Double Implements IGearboxEngineeringInputData.TorqueReserve
		Get
			Return TorqueResv
		End Get
	End Property

	Public ReadOnly Property StartAcceleration As MeterPerSquareSecond _
		Implements IGearboxEngineeringInputData.StartAcceleration
		Get
			Return StartAcc.SI(Of MeterPerSquareSecond)()
		End Get
	End Property

	Public ReadOnly Property StartTorqueReserve As Double Implements IGearboxEngineeringInputData.StartTorqueReserve
		Get
			Return TorqueResvStart
		End Get
	End Property

	Public ReadOnly Property TorqueConverter As ITorqueConverterEngineeringInputData _
		Implements IGearboxEngineeringInputData.TorqueConverter
		Get
			Return Me
		End Get
	End Property

	Public ReadOnly Property DownshiftAferUpshiftDelay As Second _
		Implements IGearboxEngineeringInputData.DownshiftAferUpshiftDelay
		Get
			Return DownshiftAfterUpshift.SI(Of Second)()
		End Get
	End Property

	Public ReadOnly Property UpshiftAfterDownshiftDelay As Second _
		Implements IGearboxEngineeringInputData.UpshiftAfterDownshiftDelay
		Get
			Return UpshiftAfterDownshift.SI(Of Second)()
		End Get
	End Property

	Public ReadOnly Property IGearboxEngineeringInputData_UpshiftMinAcceleration As MeterPerSquareSecond _
		Implements IGearboxEngineeringInputData.UpshiftMinAcceleration
		Get
			Return UpshiftMinAcceleration.SI(Of MeterPerSquareSecond)()
		End Get
	End Property

	Public ReadOnly Property IGearboxEngineeringInputData_SkipGears As Boolean _
		Implements IGearboxEngineeringInputData.SkipGears
		Get
			Return SkipGears
		End Get
	End Property

	Public ReadOnly Property IGearboxEngineeringInputData_StartSpeed As MeterPerSecond _
		Implements IGearboxEngineeringInputData.StartSpeed
		Get
			Return StartSpeed.SI(Of MeterPerSecond)()
		End Get
	End Property

	Public ReadOnly Property IGearboxEngineeringInputData_ShiftTime As Second _
		Implements IGearboxEngineeringInputData.ShiftTime
		Get
			Return ShiftTime.SI(Of Second)()
		End Get
	End Property

	Public ReadOnly Property TCData As TableData Implements ITorqueConverterDeclarationInputData.TCData
		Get
			Return VectoCSVFile.Read(_torqueConverterFile.OriginalPath)
		End Get
	End Property


	Public ReadOnly Property Ratio As Double Implements IAxleGearInputData.Ratio
		Get
			Return GearRatios(0)
		End Get
	End Property

	Public ReadOnly Property LossMap As TableData Implements IAxleGearInputData.LossMap
		Get
			Return VectoCSVFile.Read(GearLossmaps(0).PathOrDummy)
		End Get
	End Property

	Public ReadOnly Property Efficiency As Double Implements IAxleGearInputData.Efficiency
		Get
			Return GearLossMap(0, True).ToDouble()
		End Get
	End Property
End Class

