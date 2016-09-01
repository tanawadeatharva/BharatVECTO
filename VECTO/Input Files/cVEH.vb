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


Public Class cVEH
	'V2 MassMax is now saved in [t] instead of [kg]
	Private Const FormatVersion As Short = 7
	Private FileVersion As Short

	Private sFilePath As String
	Private MyPath As String

	Public Mass As Single
	Public Loading As Single
	Private siFr0 As Single

	Public CdA0 As Single
	'Public CdA02 As Single
	'Private CdA0Act As Single

	Public CdMode As CrossWindCorrectionMode
	Public CdFile As cSubPath
	Private CdX As List(Of Single)
	Private CdY As List(Of Single)


	Public RtType As RetarderType 'tRtType '0=None, 1=Primary, 2=Secondary
	Public RtRatio As Single = 0
	Public RtFile As cSubPath
	Private RtnU As List(Of Single)
	Private RtM As List(Of Single)

	Public rdyn As Single
	Public Axles As List(Of cAxle)


	Public VehCat As VehicleCategory
	Public MassExtra As Single
	Public MassMax As Single
	Public AxleConf As AxleConfiguration

	Private _myFileList As List(Of String)

	Public SavedInDeclMode As Boolean
	Public AngularGearType As AngularGearType '0=None, 1=Separate, 2=Included
	Public AngularGearRatio As Single
	Public AngularGearLossMapFile As cSubPath


	Public Class cAxle
		Public RRC As Single
		Public Share As Single
		Public TwinTire As Boolean
		Public FzISO As Single
		Public Wheels As String
		Public Inertia As Single
	End Class


	Public Sub New()
		MyPath = ""
		sFilePath = ""
		CdFile = New cSubPath
		CdX = New List(Of Single)
		CdY = New List(Of Single)
		RtFile = New cSubPath
		AngularGearLossMapFile = New cSubPath()
		RtnU = New List(Of Single)
		RtM = New List(Of Single)
		Axles = New List(Of cAxle)
		SetDefault()
	End Sub

	Private Sub SetDefault()
		Mass = 0
		MassExtra = 0
		Loading = 0
		CdA0 = 0
		'		CdA0Act = CdA0
		'		CdA02 = 0
		CdFile.Clear()
		CdMode = CrossWindCorrectionMode.NoCorrection
		CdX.Clear()
		CdY.Clear()

		siFr0 = 0
		rdyn = 0

		RtType = RetarderType.None
		RtRatio = 1
		RtnU.Clear()
		RtM.Clear()
		RtFile.Clear()
		AngularGearLossMapFile.Clear()

		AngularGearType = AngularGearType.None
		AngularGearLossMapFile.Clear()
		AngularGearRatio = 1

		Axles.Clear()
		VehCat = VehicleCategory.RigidTruck	'tVehCat.Undef
		MassMax = 0
		AxleConf = AxleConfiguration.AxleConfig_4x2	 'tAxleConf.Undef

		SavedInDeclMode = False
	End Sub

	Public Function ReadFile(Optional showMsg As Boolean = True) As Boolean
		Const msgSrc = "VEH/ReadFile"
		SetDefault()

		Dim json As New JSON
		If Not json.ReadFile(sFilePath) Then Return False

		Try
			Dim header = json.Content("Header")
			Dim body = json.Content("Body")

			FileVersion = header("FileVersion")
			If FileVersion > 4 Then
				SavedInDeclMode = body("SavedInDeclMode")
			Else
				SavedInDeclMode = Cfg.DeclMode
			End If

			Mass = body("CurbWeight")
			MassExtra = body("CurbWeightExtra")
			Loading = body("Loading")
			VehCat = body("VehCat").ToString.ParseEnum(Of VehicleCategory)() 'ConvVehCat(body("VehCat").ToString)
			AxleConf = AxleConfigurationHelper.Parse(body("AxleConfig")("Type").ToString) _
			'ConvAxleConf(body("AxleConfig")("Type").ToString)

			If FileVersion < 2 Then
				'convert kg to ton
				MassMax /= 1000
			Else
				MassMax = body("MassMax")
			End If

			If FileVersion < 7 Then
				'calc CdA from Cd and area value
				CdA0 = CSng(body("Cd")) * CSng(body("CrossSecArea"))
			Else
				CdA0 = body("CdA")
			End If

			'CdA02 = CdA0

			'If FileVersion < 4 Then
			'	If Not body("CdRigid") Is Nothing AndAlso Not body("CrossSecAreaRigid") Is Nothing Then
			'		CdA02 = CSng(body("CdRigid")) * CSng(body("CrossSecAreaRigid"))
			'	End If
			'ElseIf FileVersion < 7 Then
			'	If Not body("Cd2") Is Nothing AndAlso Not body("CrossSecArea2") Is Nothing Then
			'		CdA02 = CSng(body("Cd2")) * CSng(body("CrossSecArea2"))
			'	End If
			'Else
			'	If Not body("CdA2") Is Nothing Then
			'		CdA02 = body("CdA2")
			'	End If
			'End If

			'CdA0Act = CdA0

			CdMode = CrossWindCorrectionModeHelper.Parse(body("CdCorrMode").ToString) 'CdModeConv(body("CdCorrMode").ToString)
			If Not body("CdCorrFile") Is Nothing Then
				CdFile.Init(MyPath, body("CdCorrFile"))
			End If

			If body("Retarder") Is Nothing Then
				RtType = RetarderType.None
			Else
				RtType = RetarderTypeHelper.Parse(body("Retarder")("Type").ToString) 'RtTypeConv(body("Retarder")("Type").ToString)
				If Not body("Retarder")("Ratio") Is Nothing Then
					RtRatio = body("Retarder")("Ratio")
				End If
				If Not body("Retarder")("File") Is Nothing Then
					RtFile.Init(MyPath, body("Retarder")("File"))
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
					AngularGearLossMapFile.Init(MyPath, body("AngularGear")("LossMap"))
				End If
			End If

			Dim inertiaTemp As Single
			If FileVersion < 3 Then
				inertiaTemp = body("WheelsInertia")
				rdyn = 1000 * body("WheelsDiaEff") / 2
			Else
				rdyn = body("rdyn")
			End If

			Dim axleCount = body("AxleConfig")("Axles").Count()
			For Each axleEntry In body("AxleConfig")("Axles")
				Dim axle = New cAxle With {
						.Share = CSng(axleEntry("AxleWeightShare")),
						.TwinTire = CBool(axleEntry("TwinTyres")),
						.RRC = CSng(axleEntry("RRCISO")),
						.FzISO = CSng(axleEntry("FzISO"))}

				If FileVersion < 3 Then
					axle.Wheels = "-"
					axle.Inertia = inertiaTemp / (IIf(axle.TwinTire, 4, 2) * axleCount)
				Else
					axle.Wheels = CStr(axleEntry("Wheels")).Replace("R ", "R")
					axle.Inertia = CSng(axleEntry("Inertia"))
				End If
				Axles.Add(axle)
			Next

		Catch ex As Exception
			If showMsg Then WorkerMsg(tMsgID.Err, "Failed to read Vehicle file! " & ex.Message, msgSrc)
			Return False
		End Try

		Return True
	End Function

	Public Function SaveFile() As Boolean
		SavedInDeclMode = Cfg.DeclMode

		Dim json As New JSON
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
			{"VehCat", VehCat.ToString()},
			{"CurbWeight", Mass},
			{"CurbWeightExtra", MassExtra},
			{"Loading", Loading},
			{"MassMax", MassMax},
			{"CdA", CdA0},
			{"rdyn", rdyn},
			{"CdCorrMode", CdMode.GetName()},
			{"CdCorrFile", CdFile.PathOrDummy},
			{"Retarder", New Dictionary(Of String, Object) From {
				{"Type", RtType.GetName()},
				{"Ratio", RtRatio},
				{"File", RtFile.PathOrDummy}}},
			{"AngularGear", New Dictionary(Of String, Object) From {
				{"Type", AngularGearType.ToString()},
				{"Ratio", AngularGearRatio},
				{"LossMap", AngularGearLossMapFile.PathOrDummy}}},
			{"AxleConfig", New Dictionary(Of String, Object) From {
				{"Type", AxleConf.GetName()},
				{"Axles", (From axle In Axles Select New Dictionary(Of String, Object) From {
					{"Inertia", axle.Inertia},
					{"Wheels", axle.Wheels},
					{"AxleWeightShare", axle.Share},
					{"TwinTyres", axle.TwinTire},
					{"RRCISO", axle.RRC},
					{"FzISO", axle.FzISO}})}}}
			}

		json.Content.Add("Body", dic)
		Return json.WriteFile(sFilePath)
	End Function


#Region "Properties"


	Public Property FilePath() As String
		Get
			Return sFilePath
		End Get
		Set(value As String)
			sFilePath = value
			If sFilePath = "" Then
				MyPath = ""
			Else
				MyPath = Path.GetDirectoryName(sFilePath) & "\"
			End If
		End Set
	End Property

#End Region
End Class