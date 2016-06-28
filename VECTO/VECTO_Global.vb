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
Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports vectolic

Module VECTO_Global
	Public Const VECTOvers As String = "2.2"
	Public COREvers As String = "NOT FOUND"

	Public Const LicSigAppCode As String = "VECTO-Release-0093C61E0A2E4BFA9A7ED7E729C56AE4"
	Public MyAppPath As String
	Public MyConfPath As String
	Public MyDeclPath As String

	Public LogFile As cLogFile

	'to ensure correct format for backgroundworker thread
	Public SetCulture As Boolean

	Public VECTOworker As BackgroundWorker
	Public VECTOworkerV3 As BackgroundWorker

	Public MSGerror As Integer
	Public MSGwarn As Integer

	Public Cfg As Configuration

	Public sKey As csKey

	Public FileFormat As Encoding = Encoding.UTF8

	Public VEC As cVECTO
	Public VEH As cVEH
	Public ENG As cENG
	Public GBX As cGBX
	Public MAP As cMAP
	Public DRI As cDRI
	Public MODdata As cMOD
	Public Lic As cLicense
	Public VSUM As cVSUM
	Public DEV As cDEV

	Public Declaration As cDeclaration

	Public ProgBarCtrl As cProgBarCtrl

	''' <summary>
	''' Converts engine speed and torque to power.
	''' </summary>
	''' <param name="nU">engine speed</param>
	''' <param name="M">Torque</param>
	''' <returns>Power</returns>
	''' <remarks></remarks>
	Public Function nMtoPe(nU As Double, M As Double) As Double
		Return (nU * 2 * Math.PI / 60) * M / 1000
	End Function

	''' <summary>
	''' Convert engine speed and power to torque.
	''' </summary>
	''' <param name="nU">engine speed</param>
	''' <param name="Pe">Power</param>
	''' <returns>Torque</returns>
	''' <remarks></remarks>
	Public Function nPeToM(nU As Single, Pe As Double) As Single
		Return Pe * 1000 / (nU * 2 * Math.PI / 60)
	End Function


#Region "sKey > Typ Umwandlung"


	Public Function GearboxConv(Gearbox As tGearbox) As String
		Select Case Gearbox
			Case tGearbox.Manual
				Return "MT"
			Case tGearbox.Automatic
				Return "AT"
			Case tGearbox.SemiAutomatic
				Return "AMT"
			Case Else 'tGearbox.Custom
				Return "Custom"
		End Select
	End Function

	Public Function GearboxConv(Gearbox As String) As tGearbox
		Select Case UCase(Trim(Gearbox))
			Case "MT"
				Return tGearbox.Manual
			Case "AT"
				Return tGearbox.Automatic
			Case "AMT"
				Return tGearbox.SemiAutomatic
			Case Else '"Custom"
				Return tGearbox.Custom
		End Select
	End Function

	Public Function fDriComp(sK As String) As tDriComp
		sK = Trim(UCase(sK))
		Select Case sK
			Case sKey.DRI.t
				Return tDriComp.t
			Case sKey.DRI.V
				Return tDriComp.V
			Case sKey.DRI.Grad
				Return tDriComp.Grad
			Case sKey.DRI.nU
				Return tDriComp.nU
			Case sKey.DRI.Gears
				Return tDriComp.Gears
			Case sKey.DRI.Padd
				Return tDriComp.Padd
			Case sKey.DRI.Pe
				Return tDriComp.Pe
			Case sKey.DRI.VairVres
				Return tDriComp.VairVres
			Case sKey.DRI.VairBeta
				Return tDriComp.VairBeta
			Case sKey.DRI.s
				Return tDriComp.s
			Case sKey.DRI.StopTime
				Return tDriComp.StopTime
			Case sKey.DRI.Torque
				Return tDriComp.Torque
			Case sKey.DRI.Alt
				Return tDriComp.Alt
			Case sKey.DRI.Pwheel
				Return tDriComp.Pwheel
			Case Else
				Return tDriComp.Undefined

		End Select
	End Function

	Public Function fAuxComp(sK As String) As tAuxComp
		Dim x As Integer
		sK = Trim(UCase(sK))

		x = sK.IndexOf("_")

		If x = -1 Then Return tAuxComp.Undefined

		sK = Left(sK, x + 1)

		Select Case sK
			Case sKey.PauxSply
				Return tAuxComp.Psupply
			Case Else
				Return tAuxComp.Undefined
		End Select
	End Function


	Public Function fCompSubStr(sK As String) As String
		Dim x As Integer

		sK = Trim(UCase(sK))

		x = sK.IndexOf("_")

		If x = -1 Then Return ""

		sK = Right(sK, Len(sK) - x - 1)

		x = CShort(sK.IndexOf(">"))

		If x = -1 Then Return ""

		sK = Left(sK, x)

		Return sK
	End Function


#End Region

#Region "Typ > Name Conversion"

	Public Function ConvLoading(load As tLoading) As String
		Select Case load
			Case tLoading.FullLoaded
				Return "Full Loading"

			Case tLoading.RefLoaded
				Return "Reference Loading"

			Case tLoading.EmptyLoaded
				Return "Empty Loading"

			Case Else ' tLoading.UserDefLoaded
				Return "User-defined Loading"

		End Select
	End Function


	Public Function ConvVehCat(VehCat As tVehCat, NiceName As Boolean) As String
		Select Case VehCat
			Case tVehCat.Citybus
				Return "Citybus"
			Case tVehCat.Coach
				Return "Coach"
			Case tVehCat.InterurbanBus
				If NiceName Then
					Return "Interurban Bus"
				Else
					Return "InterurbanBus"
				End If
			Case tVehCat.RigidTruck
				If NiceName Then
					Return "Rigid Truck"
				Else
					Return "RigidTruck"
				End If
			Case tVehCat.Tractor
				If NiceName Then
					Return "Semitrailer Truck"
				Else
					Return "Tractor"
				End If
			Case Else ' tVehCat.Undef
				Return "not defined"
		End Select
	End Function

	Public Function ConvVehCat(VehCat As String) As tVehCat
		Select Case UCase(Trim(VehCat))
			Case "CITYBUS"
				Return tVehCat.Citybus
			Case "COACH"
				Return tVehCat.Coach
			Case "INTERURBANBUS"
				Return tVehCat.InterurbanBus
			Case "RIGIDTRUCK"
				Return tVehCat.RigidTruck
			Case "TRACTOR"
				Return tVehCat.Tractor
			Case Else
				Return tVehCat.Undef
		End Select
	End Function

	Public Function ConvAxleConf(AxleConf As tAxleConf) As String
		Select Case AxleConf
			Case tAxleConf.a4x2
				Return "4x2"
			Case tAxleConf.a4x4
				Return "4x4"
			Case tAxleConf.a6x2
				Return "6x2"
			Case tAxleConf.a6x4
				Return "6x4"
			Case tAxleConf.a6x6
				Return "6x6"
			Case tAxleConf.a8x2
				Return "8x2"
			Case tAxleConf.a8x4
				Return "8x4"
			Case tAxleConf.a8x6
				Return "8x6"
			Case Else 'tAxleConf.a8x8
				Return "8x8"
		End Select
	End Function

	Public Function ConvAxleConf(AxleConf As String) As tAxleConf
		Select Case UCase(Trim(AxleConf))
			Case "4X2"
				Return tAxleConf.a4x2
			Case "4X4"
				Return tAxleConf.a4x4
			Case "6X2"
				Return tAxleConf.a6x2
			Case "6X4"
				Return tAxleConf.a6x4
			Case "6X6"
				Return tAxleConf.a6x6
			Case "8X2"
				Return tAxleConf.a8x2
			Case "8X4"
				Return tAxleConf.a8x4
			Case "8X6"
				Return tAxleConf.a8x6
			Case Else '"8X8"
				Return tAxleConf.a8x8
		End Select
	End Function

	Public Function ConvMission(Mission As tMission) As String
		Select Case Mission
			Case tMission.LongHaul
				Return "LongHaul"
			Case tMission.RegionalDelivery
				Return "RegionalDelivery"
			Case tMission.UrbanDelivery
				Return "UrbanDelivery"
			Case tMission.MunicipalUtility
				Return "MunicipalUtility"
			Case tMission.Construction
				Return "Construction"
			Case tMission.HeavyUrban
				Return "HeavyUrban"
			Case tMission.Urban
				Return "Urban"
			Case tMission.Suburban
				Return "Suburban"
			Case tMission.Interurban
				Return "Interurban"
			Case tMission.Coach
				Return "Coach"
			Case Else
				Return "not defined"
		End Select
	End Function

	Public Function ConvMission(Mission As String) As tMission
		Select Case Mission
			Case "LongHaul"
				Return tMission.LongHaul
			Case "RegionalDelivery"
				Return tMission.RegionalDelivery
			Case "UrbanDelivery"
				Return tMission.UrbanDelivery
			Case "MunicipalUtility"
				Return tMission.MunicipalUtility
			Case "Construction"
				Return tMission.Construction
			Case "HeavyUrban"
				Return tMission.HeavyUrban
			Case "Urban"
				Return tMission.Urban
			Case "Suburban"
				Return tMission.Suburban
			Case "Interurban"
				Return tMission.Interurban
			Case "Coach"
				Return tMission.Coach
			Case Else
				Return tMission.Undef
		End Select
	End Function


	Public Function CdModeConv(CdMode As tCdMode) As String
		Select Case CdMode
			Case tCdMode.CdOfBeta
				Return "CdOfBeta"
			Case tCdMode.CdOfVeng
				Return "CdOfVeng"
			Case tCdMode.CdOfVdecl
				Return "CdOfVdecl"
			Case Else 'tCdMode.ConstCd0
				Return "Off"
		End Select
	End Function

	Public Function CdModeConv(CdMode As String) As tCdMode
		Select Case UCase(Trim(CdMode))
			Case "CDOFBETA"
				Return tCdMode.CdOfBeta
			Case "CDOFV", "CDOFVENG"
				Return tCdMode.CdOfVeng
			Case "CDOFVDECL"
				Return tCdMode.CdOfVdecl
			Case Else '"OFF"
				Return tCdMode.ConstCd0
		End Select
	End Function


	Public Function RtTypeConv(RtType As tRtType) As String
		Select Case RtType
			Case tRtType.Primary
				Return "Primary"
			Case tRtType.Secondary
				Return "Secondary"
			Case Else 'tRtType.None
				Return "None"
		End Select
	End Function

	Public Function RtTypeConv(RtType As String) As tRtType
		Select Case UCase(Trim(RtType))
			Case "PRIMARY"
				Return tRtType.Primary
			Case "SECONDARY"
				Return tRtType.Secondary
			Case Else '"NONE"
				Return tRtType.None
		End Select
	End Function

#End Region

	Public Class cLogFile
		Private LOGstream As StreamWriter

		Public Function StartLog() As Boolean
			Try
				LOGstream = My.Computer.FileSystem.OpenTextFileWriter(MyAppPath & "LOG.txt", True, FileFormat)
				LOGstream.AutoFlush = True
				WriteToLog(tMsgID.Normal, "Starting Session " & Now)
				WriteToLog(tMsgID.Normal, "VECTO " & VECTOvers)
			Catch ex As Exception
				Return False
			End Try

			Return True
		End Function

		Public Function SizeCheck() As Boolean
			Dim logfDetail As FileInfo
			Dim BackUpError As Boolean

			'Start new log if file size limit reached
			If File.Exists(MyAppPath & "LOG.txt") Then

				'File size check
				logfDetail = My.Computer.FileSystem.GetFileInfo(MyAppPath & "LOG.txt")

				'If Log too large: Delete
				If logfDetail.Length / (2 ^ 20) > Cfg.LogSize Then

					WriteToLog(tMsgID.Normal, "Starting new logfile")
					LOGstream.Close()

					BackUpError = False

					Try
						If File.Exists(MyAppPath & "LOG_backup.txt") Then File.Delete(MyAppPath & "LOG_backup.txt")
						File.Move(MyAppPath & "LOG.txt", MyAppPath & "LOG_backup.txt")
					Catch ex As Exception
						BackUpError = True
					End Try

					If Not StartLog() Then Return False

					If BackUpError Then
						WriteToLog(tMsgID.Err, "Failed to backup logfile! (" & MyAppPath & "LOG_backup.txt)")
					Else
						WriteToLog(tMsgID.Normal, "Logfile restarted. Old log saved to LOG_backup.txt")
					End If

				End If

			End If

			Return True
		End Function

		Public Function CloseLog() As Boolean
			Try
				WriteToLog(tMsgID.Normal, "Closing Session " & Now)
				LOGstream.Close()
			Catch ex As Exception
				Return False
			End Try

			Return True
		End Function


		Public Function WriteToLog(MsgType As tMsgID, Msg As String) As Boolean
			Dim MsgTypeStr As String

			Select Case MsgType
				Case tMsgID.Err
					MsgTypeStr = "Error"
				Case tMsgID.Warn
					MsgTypeStr = "Warning"
				Case Else
					MsgTypeStr = "-"
			End Select

			Try
				LOGstream.WriteLine(Now.ToString("yyyy/MM/dd-HH:mm:ss") & vbTab & MsgTypeStr & vbTab & Msg)
				Return True
			Catch ex As Exception
				Return False
			End Try
		End Function
	End Class

#Region "File path functions"

	'When no path is specified, then insert either HomeDir or MainDir   Special-folders
	Public Function fFileRepl(file As String, Optional ByVal MainDir As String = "") As String

		Dim ReplPath As String

		'Trim Path
		file = Trim(file)

		'If empty file => Abort
		If file = "" Then Return ""

		'Replace sKeys
		file = Replace(file, sKey.DefVehPath & "\", MyAppPath & "Default Vehicles\", 1, -1,
						CompareMethod.Text)
		file = Replace(file, sKey.HomePath & "\", MyAppPath, 1, -1, CompareMethod.Text)

		'Replace - Determine folder
		If MainDir = "" Then
			ReplPath = MyAppPath
		Else
			ReplPath = MainDir
		End If

		' "..\" => One folder-level up
		Do While ReplPath.Length > 0 AndAlso Left(file, 3) = "..\"
			ReplPath = fPathUp(ReplPath)
			file = file.Substring(3)
		Loop


		'Supplement Path, if not available
		If fPATH(file) = "" Then

			Return ReplPath & file

		Else
			Return file
		End If
	End Function

	'Path one-level-up      "C:\temp\ordner1\"  >>  "C:\temp\"
	Private Function fPathUp(Pfad As String) As String
		Dim x As Int16

		Pfad = Pfad.Substring(0, Pfad.Length - 1)

		x = Pfad.LastIndexOf("\")

		If x = -1 Then Return ""

		Return Pfad.Substring(0, x + 1)
	End Function

	'File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
	Public Function fFILE(Pfad As String, MitEndung As Boolean) As String
		Dim x As Int16
		x = Pfad.LastIndexOf("\") + 1
		Pfad = Right(Pfad, Len(Pfad) - x)
		If Not MitEndung Then
			x = Pfad.LastIndexOf(".")
			If x > 0 Then Pfad = Left(Pfad, x)
		End If
		Return Pfad
	End Function

	'Filename without extension   "C:\temp\TEST.txt" >> "C:\temp\TEST"
	Public Function fFileWoExt(Path As String) As String
		Return fPATH(Path) & fFILE(Path, False)
	End Function

	'Filename without path if Path = WorkDir or MainDir
	Public Function fFileWoDir(file As String, Optional ByVal MainDir As String = "") As String
		Dim path As String

		If MainDir = "" Then
			path = MyAppPath
		Else
			path = MainDir
		End If

		If UCase(fPATH(file)) = UCase(path) Then file = fFILE(file, True)

		Return file
	End Function

	'Path alone        "C:\temp\TEST.txt"  >>  "C:\temp\"
	'                   "TEST.txt"          >>  ""
	Public Function fPATH(Pfad As String) As String
		Dim x As Int16
		If Pfad Is Nothing OrElse Pfad.Length < 3 OrElse Pfad.Substring(1, 2) <> ":\" Then Return ""
		x = Pfad.LastIndexOf("\")
		Return Left(Pfad, x + 1)
	End Function

	'Extension alone      "C:\temp\TEST.txt" >> ".txt"
	Public Function fEXT(Pfad As String) As String
		Dim x As Int16
		x = Pfad.LastIndexOf(".")
		If x = -1 Then
			Return ""
		Else
			Return Right(Pfad, Len(Pfad) - x)
		End If
	End Function


#End Region
End Module


Public Class csKey
	Public DRI As csKeyDRI
	Public AUX As csKeyAux

	Public HomePath As String = "<HOME>"
	Public JobPath As String = "<JOBPATH>"
	Public DefVehPath As String = "<VEHDIR>"
	Public NoFile As String = "<NOFILE>"
	Public EmptyString As String = "<EMPTYSTRING>"
	Public Break As String = "<//>"

	Public Normed As String = "NORM"

	Public PauxSply As String = "<AUX_"

	Public EngDrag As String = "<DRAG>"

	Public Sub New()
		DRI = New csKeyDRI
		AUX = New csKeyAux
	End Sub

	Public Class csKeyDRI
		Public t As String = "<T>"
		Public V As String = "<V>"
		Public Grad As String = "<GRAD>"
		Public Alt As String = "<ALT>"
		Public Gears As String = "<GEAR>"
		Public nU As String = "<N>"
		Public Pe As String = "<PE>"
		Public Padd As String = "<PADD>"
		Public VairVres As String = "<VAIR_RES>"
		Public VairBeta As String = "<VAIR_BETA>"
		Public s As String = "<S>"
		Public StopTime As String = "<STOP>"
		Public Torque As String = "<ME>"
		Public Pwheel As String = "<PWHEEL>"
	End Class

	Public Class csKeyAux
		Public Fan As String = "FAN"
		Public SteerPump As String = "STP"
		Public HVAC As String = "AC"
		Public ElecSys As String = "ES"
		Public PneumSys As String = "PS"
	End Class
End Class


