Imports System.IO
Imports System.Windows.Forms

Public Class JiraDialog
	Private Sub OK_Button_Click(ByVal sender As Object, ByVal e As EventArgs) Handles OK_Button.Click
		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
		If File.Exists(MyAppPath & "User Manual\JIRA Quick Start Guide.pdf") Then
			Process.Start(MyAppPath & "User Manual\JIRA Quick Start Guide.pdf")
		Else
			MsgBox("File not found!", MsgBoxStyle.Critical)
		End If
	End Sub

	Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
		Dim bodytext As String

		bodytext = "Please provide the following information:" & "%0A" &
					"- Email" & "%0A" &
					"- Name, Surname" & "%0A" &
					"- Country of workplace" & "%0A" &
					"- Position"

		Process.Start("mailto:vecto@jrc.ec.europa.eu?subject=CITnet%20account&body=" & bodytext)
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Process.Start("https://webgate.ec.europa.eu/CITnet/jira/browse/VECTO")
	End Sub
End Class
