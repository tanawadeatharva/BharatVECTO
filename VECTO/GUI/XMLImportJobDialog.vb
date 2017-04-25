Imports System.IO
Imports Microsoft.WindowsAPICodePack.Dialogs

Public Class XMLImportJobDialog
	Private Sub btnBrowseJob_Click(sender As Object, e As EventArgs) Handles btnBrowseJob.Click
		Dim dialog As OpenFileDialog = New OpenFileDialog()
		If (dialog.ShowDialog() = DialogResult.OK) Then
			tbJobFile.Text = Path.GetFullPath(dialog.FileName)
		End If
	End Sub

	Private Sub btnBrowseOutput_Click(sender As Object, e As EventArgs) Handles btnBrowseOutput.Click
		Dim dialog As CommonOpenFileDialog = New CommonOpenFileDialog()
		dialog.IsFolderPicker = True
		If (dialog.ShowDialog() = CommonFileDialogResult.Ok) Then
			tbDestination.Text = Path.GetFullPath(dialog.FileName)
		End If
	End Sub

	Private Sub btnClose_Click(sender As Object, e As EventArgs) Handles btnClose.Click
		Close()
	End Sub

	Private Sub btnImport_Click(sender As Object, e As EventArgs) Handles btnImport.Click
		' TODO!
	End Sub
End Class