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

Public Module VECTO_Global
	Public Const VECTOvers As String = "3"
	Public COREvers As String = "NOT FOUND"

	Public Const LicSigAppCode As String = "VECTO-Release-0093C61E0A2E4BFA9A7ED7E729C56AE4"
	Public MyAppPath As String
	Public MyConfPath As String


	Public LogFile As cLogFile

	'to ensure correct format for backgroundworker thread

	Public VectoWorkerV3 As BackgroundWorker

	Public Cfg As Configuration

	'Public sKey As csKey

	Public ReadOnly FileFormat As Encoding = Encoding.UTF8

	Public Lic As cLicense
	'Public VSUM As cVSUM
	'Public DEV As cDEV


	Public ProgBarCtrl As ProgressbarControl

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


	Public Class cLogFile
		Private LOGstream As StreamWriter

		Public Function StartLog() As Boolean
			Try
				LOGstream = My.Computer.FileSystem.OpenTextFileWriter(MyAppPath & "LOG.txt", True, FileFormat)
				LOGstream.AutoFlush = True
				WriteToLog(MessageType.Normal, "Starting Session " & Now)
				WriteToLog(MessageType.Normal, "VECTO " & VECTOvers)
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

					WriteToLog(MessageType.Normal, "Starting new logfile")
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
						WriteToLog(MessageType.Err, "Failed to backup logfile! (" & MyAppPath & "LOG_backup.txt)")
					Else
						WriteToLog(MessageType.Normal, "Logfile restarted. Old log saved to LOG_backup.txt")
					End If

				End If

			End If

			Return True
		End Function

		Public Function CloseLog() As Boolean
			Try
				WriteToLog(MessageType.Normal, "Closing Session " & Now)
				LOGstream.Close()
			Catch ex As Exception
				Return False
			End Try

			Return True
		End Function


		Public Function WriteToLog(MsgType As MessageType, Msg As String) As Boolean
			Dim MsgTypeStr As String

			Select Case MsgType
				Case MessageType.Err
					MsgTypeStr = "Error"
				Case MessageType.Warn
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
		file = Replace(file, Constants.DefVehPath & "\", MyAppPath & "Default Vehicles\", 1, -1,
						CompareMethod.Text)
		file = Replace(file, Constants.HomePath & "\", MyAppPath, 1, -1, CompareMethod.Text)

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
		If GetPath(file) = "" Then

			Return ReplPath & file

		Else
			Return file
		End If
	End Function

	'Path one-level-up      "C:\temp\ordner1\"  >>  "C:\temp\"
	Private Function fPathUp(Pfad As String) As String
		Dim x As Integer

		Pfad = Pfad.Substring(0, Pfad.Length - 1)

		x = Pfad.LastIndexOf("\", StringComparison.Ordinal)

		If x = -1 Then Return ""

		Return Pfad.Substring(0, x + 1)
	End Function

	'File name without the path    "C:\temp\TEST.txt"  >>  "TEST.txt" oder "TEST"
	Public Function GetFilenameWithoutPath(file As String, includeFileExtension As Boolean) As String _
'GetFilenameWithoutPath
		Dim x As Integer
		x = file.LastIndexOf("\", StringComparison.Ordinal) + 1
		file = Right(file, Len(file) - x)
		If Not includeFileExtension Then
			x = file.LastIndexOf(".", StringComparison.Ordinal)
			If x > 0 Then file = Left(file, x)
		End If
		Return file
	End Function

	'Filename without extension   "C:\temp\TEST.txt" >> "C:\temp\TEST"

	'Filename without path if Path = WorkDir or MainDir
	Public Function GetFilenameWithoutDirectory(file As String, Optional ByVal mainDir As String = "") As String _
'GetFilenameWithoutDirectory
		Dim path As String

		If mainDir = "" Then
			path = MyAppPath
		Else
			path = mainDir
		End If

		If UCase(GetPath(file)) = UCase(path) Then file = GetFilenameWithoutPath(file, True)

		Return file
	End Function

	'Path alone        "C:\temp\TEST.txt"  >>  "C:\temp\"
	'                   "TEST.txt"          >>  ""
	Public Function GetPath(file As String) As String 'GetPath
		Dim x As Integer
		If file Is Nothing OrElse file.Length < 3 OrElse file.Substring(1, 2) <> ":\" Then Return ""
		x = file.LastIndexOf("\", StringComparison.Ordinal)
		Return Left(file, x + 1)
	End Function

	'Extension alone      "C:\temp\TEST.txt" >> ".txt"
	Public Function GetExtension(file As String) As String 'GetExtension
		Dim x As Integer
		x = file.LastIndexOf(".", StringComparison.Ordinal)
		If x = -1 Then
			Return ""
		Else
			Return Right(file, Len(file) - x)
		End If
	End Function


#End Region
End Module


Module Constants
	'Public ReadOnly AUX As AuxiliaryKey

	Public Const HomePath As String = "<HOME>"
	Public Const DefVehPath As String = "<VEHDIR>"
	Public Const NoFile As String = "<NOFILE>"

	'Public Sub New()

	'	AUX = New AuxiliaryKey
	'End Sub


	' ReSharper disable once ClassNeverInstantiated.Global
	Public Class AuxiliaryKey
		Public Const Fan As String = "FAN"
		Public Const SteerPump As String = "STP"
		Public Const HVAC As String = "AC"
		Public Const ElecSys As String = "ES"
		Public Const PneumSys As String = "PS"
	End Class
End Module


