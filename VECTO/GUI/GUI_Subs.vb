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

''' <summary>
''' Methods for GUI interaction
''' </summary>
''' <remarks></remarks>
Public Module GUI_Subs

#Region "GUI control via background worker"

	'Status Message => Msg-Listview
	Public Sub WorkerMsg(ByVal ID As tMsgID, ByVal Msg As String, ByVal Source As String,
						Optional ByVal Link As String = "")
		Dim WorkProg As New cWorkProg(tWorkMsgType.StatusListBox)
		WorkProg.ID = ID
		Select Case ID
			Case tMsgID.Err
			Case tMsgID.Warn
		End Select
		WorkProg.Msg = Msg
		WorkProg.Source = Source
		Try
			'VECTOworker.ReportProgress(0, WorkProg)
		Catch ex As Exception
			GUImsg(ID, Msg)
		End Try
	End Sub

#End Region

#Region "Direct GUI control - Cannot be called by background worker!"

	'Status message
	Public Sub GUImsg(ByVal ID As tMsgID, ByVal Msg As String)
		F_MAINForm.MSGtoForm(ID, Msg, "", "")
	End Sub

	'Statusbar
	Public Sub Status(ByVal txt As String)
		F_MAINForm.ToolStripLbStatus.Text = txt
	End Sub

	'Status form reset
	Public Sub ClearMSG()
		F_MAINForm.LvMsg.Items.Clear()
	End Sub

#End Region

	'Class used to pass Messages from BackgroundWorker to GUI
	Public Class cWorkProg
		Private MyID As tMsgID
		Private MyMsg As String
		Private MySource As String

		Public Sub New(ByVal MsgTarget As tWorkMsgType)
			MySource = ""
		End Sub


		Public Property Source As String
			Get
				Return MySource
			End Get
			Set(value As String)
				MySource = value
			End Set
		End Property

		Public Property ID() As tMsgID
			Get
				Return MyID
			End Get
			Set(ByVal value As tMsgID)
				MyID = value
			End Set
		End Property

		Public Property Msg() As String
			Get
				Return MyMsg
			End Get
			Set(ByVal value As String)
				MyMsg = value
			End Set
		End Property
	End Class

	'Progress bar control
	Public Class cProgBarCtrl
		Public ProgOverallStartInt As Integer = -1
		Public PgroOverallEndInt As Integer = -1
		Public ProgJobInt As Integer = -1
		Public ProgLock As Boolean = False
	End Class

#Region "Textbox text conversion for file open/save operations"

	'Text-to-number
	Public Function fTextboxToNumString(ByVal txt As String) As String
		If Not IsNumeric(txt) Then
			Return "0"
		Else
			Return txt
		End If
	End Function


#End Region

	'Open File with software defined in Config
	Public Function FileOpenAlt(ByVal file As String) As Boolean
		Dim PSI As New ProcessStartInfo

		If Not IO.File.Exists(file) Then Return False

		PSI.FileName = Cfg.OpenCmd
		PSI.Arguments = ChrW(34) & file & ChrW(34)
		Try
			Process.Start(PSI)
			Return True
		Catch ex As Exception
			Return False
		End Try
	End Function

	Public Function WrongMode() As Integer

		If Cfg.DeclMode Then

			Select Case _
				MsgBox(
					"This file was created in Engineering Mode! Opening in Declaration Mode will overwrite some parameters with generic values." &
					vbCrLf & vbCrLf & "Do you want to switch to Engineering Mode?" & vbCrLf & vbCrLf &
					"[Yes] Switch mode and open file" & vbCrLf & "[No] Open file without changing mode" & vbCrLf &
					"[Cancel] Abort opening file", MsgBoxStyle.YesNoCancel, "Warning")
				Case MsgBoxResult.Yes
					Return 1

				Case (MsgBoxResult.No)
					Return 0

				Case Else
					Return -1

			End Select

		Else

			Select Case _
				MsgBox(
					"This file was created in Declaration Mode! For use in Engineering Mode missing parameters must be defined." &
					vbCrLf & vbCrLf & "Do you want to switch to Declaration Mode?" & vbCrLf & vbCrLf &
					"[Yes] Switch mode and open file" & vbCrLf & "[No] Open file without changing mode" & vbCrLf &
					"[Cancel] Abort opening file", MsgBoxStyle.YesNoCancel, "Warning")
				Case MsgBoxResult.Yes
					Return 1

				Case (MsgBoxResult.No)
					Return 0

				Case Else
					Return -1

			End Select

		End If
	End Function
End Module
