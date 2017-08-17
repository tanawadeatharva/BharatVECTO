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
Imports System.IO
Imports TUGraz.VectoCommon.Utils

''' <summary>
''' Welcome screen. Shows only on the first time application start
''' </summary>
''' <remarks></remarks>
Public Class WelcomeDialog
	'Close
	Private Sub Cancel_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Cancel_Button.Click
		DialogResult = DialogResult.Cancel
		Close()
	End Sub

	'Init
	Private Sub F_Welcome_Load(sender As Object, e As EventArgs) Handles Me.Load
		Text = "VECTO " & VECTOvers & " / VectoCore " & COREvers
	End Sub

	'Open Release Notes
	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		If File.Exists(MyAppPath & "User Manual\Release Notes.pdf") Then
			Process.Start(MyAppPath & "User Manual\Release Notes.pdf")
		Else
			MsgBox("Release Notes not found!", MsgBoxStyle.Critical)
		End If
	End Sub

	'Open Quick Start Guide
	Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
			Process.Start(defaultBrowserPath, String.Format("""file://{0}{1}""", MyAppPath, "User Manual\help.html"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub
End Class
