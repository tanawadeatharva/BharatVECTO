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
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.Declaration


Public Class Vehicle
	'V2 MassMax is now saved in [t] instead of [kg]
	Private Const FormatVersion As Short = 7
	Private _fileVersion As Short

	Private _filePath As String
	Private _path As String

	Public Mass As Single
	Public Loading As Single

	Public CdA0 As Single

	Public CrossWindCorrectionMode As CrossWindCorrectionMode
	Public ReadOnly CrossWindCorrectionFile As SubPath

	Public RetarderType As RetarderType
	Public RetarderRatio As Single = 0
	Public ReadOnly RetarderLossMapFile As SubPath

	Public DynamicTyreRadius As Single
	Public ReadOnly Axles As List(Of Axle)


	Public VehicleCategory As VehicleCategory
	Public MassExtra As Single
	Public MassMax As Single
	Public AxleConfiguration As AxleConfiguration

	Public SavedInDeclMode As Boolean
	Public AngularGearType As AngularGearType
	Public AngularGearRatio As Single
	Public ReadOnly AngularGearLossMapFile As SubPath

	Public PTOType As String
	Public PTOLossMap As SubPath
	Public PTOCycle As SubPath

	Public Class Axle
		Public RRC As Single
		Public Share As Single
		Public TwinTire As Boolean
		Public FzISO As Single
		Public Wheels As String
		Public Inertia As Single
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
			Dim header = json.Content("Header")
			Dim body = json.Content("Body")

			_fileVersion = header("FileVersion")
			If _fileVersion > 4 Then
				SavedInDeclMode = body("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			Mass = body("CurbWeight")
			MassExtra = body("CurbWeightExtra")
			Loading = body("Loading")
			VehicleCategory = body("VehCat").ToString.ParseEnum(Of VehicleCategory)() 'ConvVehCat(body("VehCat").ToString)
			AxleConfiguration = AxleConfigurationHelper.Parse(body("AxleConfig")("Type").ToString)
			If _fileVersion < 2 Then
				'convert kg to ton
				MassMax /= 1000
			Else
				MassMax = body("MassMax")
			End If

			If _fileVersion < 7 Then
				'calc CdA from Cd and area value
				CdA0 = CSng(body("Cd")) * CSng(body("CrossSecArea"))
			Else
				CdA0 = body("CdA")
			End If

			'CdA02 = CdA0

			CrossWindCorrectionMode = CrossWindCorrectionModeHelper.Parse(body("CdCorrMode").ToString)
			If Not body("CdCorrFile") Is Nothing Then
				CrossWindCorrectionFile.Init(_path, body("CdCorrFile"))
			End If

			If body("Retarder") Is Nothing Then
				RetarderType = RetarderType.None
			Else
				RetarderType = RetarderTypeHelper.Parse(body("Retarder")("Type").ToString)
				If Not body("Retarder")("Ratio") Is Nothing Then
					RetarderRatio = body("Retarder")("Ratio")
				End If
				If Not body("Retarder")("File") Is Nothing Then
					RetarderLossMapFile.Init(_path, body("Retarder")("File"))
				End If
			End If

			If body("AngularGear") Is Nothing Then
				AngularGearType = AngularGearType.None
			Else
				AngularGearType = body("AngularGear")("Type").ToString.ParseEnum(Of AngularGearType)()
				If Not body("AngularGear")("Ratio") Is Nothing Then
					AngularGearRatio = body("AngularGear")("Ratio")
				End If
				If Not body("AngularGear")("LossMap") Is Nothing Then
					AngularGearLossMapFile.Init(_path, body("AngularGear")("LossMap"))
				End If
			End If

			Dim inertiaTemp As Single
			If _fileVersion < 3 Then
				inertiaTemp = body("WheelsInertia")
				DynamicTyreRadius = 1000 * body("WheelsDiaEff") / 2
			Else
				DynamicTyreRadius = body("rdyn")
			End If

			Dim axleCount = body("AxleConfig")("Axles").Count()
			For Each axleEntry In body("AxleConfig")("Axles")
				Dim axle = New Axle With {
						.Share = CSng(axleEntry("AxleWeightShare")),
						.TwinTire = CBool(axleEntry("TwinTyres")),
						.RRC = CSng(axleEntry("RRCISO")),
						.FzISO = CSng(axleEntry("FzISO"))}

				If _fileVersion < 3 Then
					axle.Wheels = "-"
					axle.Inertia = inertiaTemp / (IIf(axle.TwinTire, 4, 2) * axleCount)
				Else
					axle.Wheels = CStr(axleEntry("Wheels")).Replace("R ", "R")
					axle.Inertia = CSng(axleEntry("Inertia"))
				End If
				Axles.Add(axle)
			Next

			PTOType = PTOTransmission.NoPTO
			If Not body("PTO") Is Nothing Then
				Dim ptoStr = body("PTO")("Type")

				If String.IsNullOrWhiteSpace(ptoStr) Then
					PTOType = PTOTransmission.NoPTO
					WorkerMsg(MessageType.Normal, "PTO automatically updated to '" + PTOType + "'", msgSrc)
				Else
					Try
						DeclarationData.PTOTransmission.Lookup(ptoStr)
						PTOType = ptoStr
					Catch ex As Exception
						PTOType = PTOTransmission.NoPTO
						WorkerMsg(MessageType.Normal, "PTO '" + ptoStr + "' not found, automatically updated to '" + PTOType + "'", msgSrc)
					End Try
				End If

			End If

			If Not PTOType.Equals(PTOTransmission.NoPTO) Then
				PTOLossMap.Init(_path, body("PTO")("LossMap"))
				PTOCycle.Init(_path, body("PTO")("Cycle"))
			End If

		Catch ex As Exception
			If showMsg Then WorkerMsg(MessageType.Err, "Failed to read Vehicle file! " & ex.Message, msgSrc)
			Return False
		End Try

		Return True
	End Function

	Public Function SaveFile() As Boolean
		SavedInDeclMode = Cfg.DeclMode

		Dim json As New JSONParser
		'Header
		json.Content.Add("Header", New Dictionary(Of String, Object) From {
							{"CreatedBy", Lic.LicString & " (" & Lic.GUID & ")"},
							{"Date", Now.ToUniversalTime().ToString("o")},
							{"AppVersion", VECTOvers},
							{"FileVersion", FormatVersion}})

		'Body
		Dim dic As Dictionary(Of String, Object)
		dic = New Dictionary(Of String, Object) From {
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

		json.Content.Add("Body", dic)
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