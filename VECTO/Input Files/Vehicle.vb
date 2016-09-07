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
Option Infer On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports Newtonsoft.Json.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.Models.Declaration

<CustomValidation(GetType(Vehicle), "ValidateVehicle")>
Public Class Vehicle
	'V2 MassMax is now saved in [t] instead of [kg]
	Private Const FormatVersion As Short = 7
	Private _fileVersion As Integer

	Private _filePath As String
	Private _path As String

	Public Mass As Double
	Public Loading As Double

	Public CdA0 As Double

	Public CrossWindCorrectionMode As CrossWindCorrectionMode
	Public ReadOnly CrossWindCorrectionFile As SubPath

	<ValidateObject()> Public RetarderType As RetarderType
	Public RetarderRatio As Double = 0
	Public ReadOnly RetarderLossMapFile As SubPath

	Public DynamicTyreRadius As Double
	Public ReadOnly Axles As List(Of Axle)


	Public VehicleCategory As VehicleCategory
	Public MassExtra As Double
	Public MassMax As Double
	Public AxleConfiguration As AxleConfiguration

	Public SavedInDeclMode As Boolean
	Public AngularGearType As AngularGearType
	Public AngularGearRatio As Double
	Public ReadOnly AngularGearLossMapFile As SubPath

	Public PTOType As String
	Public ReadOnly PTOLossMap As SubPath
	Public ReadOnly PTOCycle As SubPath

	Public Class Axle
		Public RRC As Double
		Public Share As Double
		Public TwinTire As Boolean
		Public FzISO As Double
		Public Wheels As String
		Public Inertia As Double
	End Class


	Public Sub New()
		_path = ""
		_filePath = ""
		CrossWindCorrectionFile = New SubPath

		RetarderLossMapFile = New SubPath
		AngularGearLossMapFile = New SubPath()

		Axles = New List(Of Axle)
		PTOLossMap = New SubPath()
		PTOCycle = New SubPath()
		SetDefault()
	End Sub


	Public Shared Function ValidateVehicle(vehicle As Vehicle, validationContext As ValidationContext) As ValidationResult
		Return New ValidationResult("bla")
	End Function

	Private Sub SetDefault()
		Mass = 0
		MassExtra = 0
		Loading = 0
		CdA0 = 0
		'		CdA0Act = CdA0
		'		CdA02 = 0
		CrossWindCorrectionFile.Clear()
		CrossWindCorrectionMode = CrossWindCorrectionMode.NoCorrection

		DynamicTyreRadius = 0

		RetarderType = RetarderType.None
		RetarderRatio = 1
		RetarderLossMapFile.Clear()
		AngularGearLossMapFile.Clear()

		AngularGearType = AngularGearType.None
		AngularGearLossMapFile.Clear()
		AngularGearRatio = 1

		PTOType = PTOTransmission.NoPTO
		PTOLossMap.Clear()
		PTOCycle.Clear()

		Axles.Clear()
		VehicleCategory = VehicleCategory.RigidTruck	'tVehCat.Undef
		MassMax = 0
		AxleConfiguration = AxleConfiguration.AxleConfig_4x2	 'tAxleConf.Undef

		SavedInDeclMode = False
	End Sub

	Public Function ReadFile(Optional showMsg As Boolean = True) As Boolean
		Const msgSrc = "VEH/ReadFile"
		SetDefault()

		Dim json As New JSONParser
		If Not json.ReadFile(_filePath) Then Return False

		Try
			Dim header As JToken = json.Content.GetEx("Header")
			Dim body As JToken = json.Content.GetEx("Body")

			_fileVersion = header.GetEx(Of Integer)("FileVersion")
			If _fileVersion > 4 Then
				SavedInDeclMode = body.GetEx(Of Boolean)("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			Mass = body.GetEx(Of Double)("CurbWeight")
			MassExtra = body.GetEx(Of Double)("CurbWeightExtra")
			Loading = body.GetEx(Of Double)("Loading")
			VehicleCategory = body("VehCat").ToString.ParseEnum(Of VehicleCategory)() 'ConvVehCat(body("VehCat").ToString)
			AxleConfiguration = AxleConfigurationHelper.Parse(body("AxleConfig")("Type").ToString)
			If _fileVersion < 2 Then
				'convert kg to ton
				MassMax /= 1000
			Else
				MassMax = body.GetEx(Of Double)("MassMax")
			End If

			If _fileVersion < 7 Then
				'calc CdA from Cd and area value
				CdA0 = (body.GetEx(Of Double)("Cd")) * (body.GetEx(Of Double)("CrossSecArea"))
			Else
				CdA0 = body.GetEx(Of Double)("CdA")
			End If

			'CdA02 = CdA0

			CrossWindCorrectionMode = CrossWindCorrectionModeHelper.Parse(body("CdCorrMode").ToString)
			If Not body("CdCorrFile") Is Nothing Then
				CrossWindCorrectionFile.Init(_path, body.GetEx(Of String)("CdCorrFile"))
			End If

			If body("Retarder") Is Nothing Then
				RetarderType = RetarderType.None
			Else
				RetarderType = RetarderTypeHelper.Parse(body("Retarder")("Type").ToString)
				Dim retarder As JToken = body.GetEx("Retarder")
				If Not retarder("Ratio") Is Nothing Then
					RetarderRatio = retarder.GetEx(Of Double)("Ratio")
				End If
				If Not retarder("File") Is Nothing Then
					RetarderLossMapFile.Init(_path, retarder.GetEx(Of String)("File"))
				End If
			End If

			If body("AngularGear") Is Nothing Then
				AngularGearType = AngularGearType.None
			Else
				AngularGearType = body("AngularGear")("Type").ToString.ParseEnum(Of AngularGearType)()
				Dim angleDrive As JToken = body("AngularGear")
				If Not angleDrive("Ratio") Is Nothing Then
					AngularGearRatio = angleDrive.GetEx(Of Double)("Ratio")
				End If
				If Not body("AngularGear")("LossMap") Is Nothing Then
					AngularGearLossMapFile.Init(_path, angleDrive.GetEx(Of String)("LossMap"))
				End If
			End If

			Dim inertiaTemp As Double
			If _fileVersion < 3 Then
				inertiaTemp = body.GetEx(Of Double)("WheelsInertia")
				DynamicTyreRadius = 1000 * body.GetEx(Of Double)("WheelsDiaEff") / 2
			Else
				DynamicTyreRadius = body.GetEx(Of Double)("rdyn")
			End If

			Dim axleCount As Integer = body("AxleConfig")("Axles").Count()
			For Each axleEntry In body.GetEx("AxleConfig").GetEx("Axles")
				Dim axle = New Axle With {
						.Share = (axleEntry.GetEx(Of Double)("AxleWeightShare")),
						.TwinTire = (axleEntry.GetEx(Of Boolean)("TwinTyres")),
						.RRC = (axleEntry.GetEx(Of Double)("RRCISO")),
						.FzISO = (axleEntry.GetEx(Of Double)("FzISO"))}

				If _fileVersion < 3 Then
					axle.Wheels = "-"
					Dim numWheels As Integer = 2
					If axle.TwinTire Then numWheels = 4
					axle.Inertia = inertiaTemp / (numWheels * axleCount)
				Else
					axle.Wheels = (axleEntry.GetEx(Of String)("Wheels")).Replace("R ", "R")
					axle.Inertia = (axleEntry.GetEx(Of Double)("Inertia"))
				End If
				Axles.Add(axle)
			Next

			PTOType = PTOTransmission.NoPTO
			If Not body("PTO") Is Nothing Then
				Dim ptoTypeToken = body.GetEx("PTO")("Type")

				If String.IsNullOrWhiteSpace(ptoTypeToken.Value(Of String)) Then
					PTOType = PTOTransmission.NoPTO
					WorkerMsg(MessageType.Normal, "PTO automatically updated to '" + ptoTypeToken.Value(Of String)() + "'", msgSrc)
				Else
					Try
						DeclarationData.PTOTransmission.Lookup(ptoTypeToken.Value(Of String))
						PTOType = ptoTypeToken.Value(Of String)()
					Catch ex As Exception
						WorkerMsg(MessageType.Normal,
								"PTO '" + ptoTypeToken.Value(Of String)() + "' not found, automatically updated to '" + PTOTransmission.NoPTO +
								"'", msgSrc)
						PTOType = PTOTransmission.NoPTO
					End Try
				End If

			End If

			If Not PTOType.Equals(PTOTransmission.NoPTO) Then
				PTOLossMap.Init(_path, body.GetEx("PTO").GetEx(Of String)("LossMap"))
				PTOCycle.Init(_path, body.GetEx("PTO").GetEx(Of String)("Cycle"))
			End If

		Catch ex As Exception
			If showMsg Then WorkerMsg(MessageType.Err, "Failed to read Vehicle file! " & ex.Message, msgSrc)
			Return False
		End Try

		Return True
	End Function

	Public Function SaveFile() As Boolean
		SavedInDeclMode = Cfg.DeclMode

		Dim validationResults = Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering))

		If validationResults.Count > 0 Then
			MsgBox(String.Format("Invalid input: \n{0}", String.Join("; ", validationResults)), MsgBoxStyle.OkOnly, "Failed to save vehicle")
			Return False
		End If

		Dim json As New JSONParser
		'Header
		Dim header As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
				{"Date", Now.ToUniversalTime().ToString("o")},
				{"AppVersion", VECTOvers},
				{"FileVersion", FormatVersion}}

		'Body
		Dim body As Dictionary(Of String, Object) = New Dictionary(Of String, Object) From {
				{"SavedInDeclMode", Cfg.DeclMode},
				{"VehCat", VehicleCategory.ToString()},
				{"CurbWeight", Mass},
				{"CurbWeightExtra", MassExtra},
				{"Loading", Loading},
				{"MassMax", MassMax},
				{"CdA", CdA0},
				{"rdyn", DynamicTyreRadius},
				{"CdCorrMode", CrossWindCorrectionMode.GetName()},
				{"CdCorrFile", CrossWindCorrectionFile.PathOrDummy},
				{"Retarder", New Dictionary(Of String, Object) From {
				{"Type", RetarderType.GetName()},
				{"Ratio", RetarderRatio},
				{"File", RetarderLossMapFile.PathOrDummy}}},
				{"AngularGear", New Dictionary(Of String, Object) From {
				{"Type", AngularGearType.ToString()},
				{"Ratio", AngularGearRatio},
				{"LossMap", AngularGearLossMapFile.PathOrDummy}}},
				{"PTO", New Dictionary(Of String, Object) From {
				{"Type", PTOType},
				{"LossMap", PTOLossMap.PathOrDummy},
				{"Cycle", PTOCycle.PathOrDummy}}},
				{"AxleConfig", New Dictionary(Of String, Object) From {
				{"Type", AxleConfiguration.GetName()},
				{"Axles", (From axle In Axles Select New Dictionary(Of String, Object) From {
				{"Inertia", axle.Inertia},
				{"Wheels", axle.Wheels},
				{"AxleWeightShare", axle.Share},
				{"TwinTyres", axle.TwinTire},
				{"RRCISO", axle.RRC},
				{"FzISO", axle.FzISO}})}}}
				}

		json.Content = JToken.FromObject(New Dictionary(Of String, Object) From {{"Header", header}, {"Body", body}})
		Return json.WriteFile(_filePath)
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
End Class