Imports System.IO

Public Class JiraDialog
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click
		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
		Process.Start(New ProcessStartInfo("mailto:JRC-VECTO@ec.europa.eu") With {.UseShellExecute = True})
	End Sub

	Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
		Process.Start(New ProcessStartInfo("https://code.europa.eu/groups/vecto/-/wikis/home") With {.UseShellExecute = True})
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
		Process.Start(New ProcessStartInfo("https://code.europa.eu/vecto/vecto/-/issues") With {.UseShellExecute = True})
	End Sub
End Class
