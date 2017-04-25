Imports System.IO
Imports System.Xml.Linq
Imports Microsoft.WindowsAPICodePack.Dialogs
Imports TUGraz.IVT.VectoXML.Writer
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCore.Models.Declaration

Public Class XMLExportJobDialog
	Private _mode As ExecutionMode
	Private _data As IInputDataProvider

	Public Sub Initialize(data As IInputDataProvider)
		Dim source As String
		Dim allowSingleFile As Boolean
		Dim eng As IEngineeringInputDataProvider = CType(data, IEngineeringInputDataProvider)
		If (Not eng Is Nothing AndAlso Not eng.JobInputData().SavedInDeclarationMode) Then
			source = eng.JobInputData().JobName
			_mode = ExecutionMode.Engineering
			allowSingleFile = True
		Else
			Dim decl As IDeclarationInputDataProvider = CType(data, IDeclarationInputDataProvider)
			If (Not decl Is Nothing AndAlso decl.JobInputData().SavedInDeclarationMode) Then
				source = decl.JobInputData().JobName
				_mode = ExecutionMode.Declaration
				allowSingleFile = False
			Else
				Throw New VectoException("Input data is neither declaration nor engineering mode!")
			End If
		End If
		_data = data
		tbJobfile.Text = source
		tbMode.Text = If(_mode = ExecutionMode.Engineering, "Engineering Mode", "Declaration Mode")
		cbSingleFile.Checked = True
		cbSingleFile.Enabled = allowSingleFile
	End Sub

	Private Sub btnBrowseOutputDir_Click(sender As Object, e As EventArgs) Handles btnBrowseOutputDir.Click
		Dim dialog As CommonOpenFileDialog = New CommonOpenFileDialog()
		dialog.IsFolderPicker = True
		If (dialog.ShowDialog() = CommonFileDialogResult.Ok) Then
			tbDestination.Text = Path.GetFullPath(dialog.FileName)
		End If
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		Close()
	End Sub

	Private Sub btnExport_Click(sender As Object, e As EventArgs) Handles btnExport.Click
		If (String.IsNullOrWhiteSpace(tbDestination.Text) OrElse Not Directory.Exists(tbDestination.Text)) Then
			MessageBox.Show("Output directory is invalid", "Invalid Output", MessageBoxButtons.OK)
			Exit Sub
		End If

		If (_mode = ExecutionMode.Engineering) Then
			Dim engineeringData As IEngineeringInputDataProvider = CType(_data, IEngineeringInputDataProvider)
			If (engineeringData Is Nothing OrElse engineeringData.JobInputData().SavedInDeclarationMode) Then
				Throw New Exception("Input data is not in engineering mode!")
			End If
			Dim document As XDocument =
					New XMLEngineeringWriter(tbDestination.Text, cbSingleFile.Checked, tbVendor.Text).GenerateVectoJob(engineeringData)
			document.Save(Path.Combine(tbDestination.Text, engineeringData.JobInputData().JobName + ".xml"))
			MessageBox.Show("Successfully exported")
			Close()
		Else
			Dim declarationData As IDeclarationInputDataProvider = CType(_data, IDeclarationInputDataProvider)
			If (declarationData Is Nothing OrElse Not declarationData.JobInputData().SavedInDeclarationMode) Then
				Throw New Exception("Input data is not in declaration mode")
			End If
			Dim document As XDocument =
					New XMLDeclarationWriter(tbVendor.Text).GenerateVectoJob(declarationData)
			document.Save(Path.Combine(tbDestination.Text, declarationData.JobInputData().JobName + ".xml"))
			MessageBox.Show("Successfully exported")
			Close()
		End If
	End Sub
End Class