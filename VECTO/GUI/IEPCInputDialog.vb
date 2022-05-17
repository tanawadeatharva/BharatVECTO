Imports System.Globalization

Public Enum IEPCDialogType
	DragCurveDialog
	PowerMapDialog
End Enum

Public Class IEPCInputDialog

	Private _dialogType As IEPCDialogType

	Public Sub New(dialogType As IEPCDialogType)

		' This call is required by the designer.
		InitializeComponent()

		' Add any initialization after the InitializeComponent() call.
		SetDialogTitle(dialogType)
		_dialogType = dialogType

	End Sub

	Public Sub Clear()
		_tbGear.Text = ""
		_tbInputFile.Text = ""
		_tbGear.Focus()
	End Sub


#Region "Button Handling"

	Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
		Dim gear As Integer
		If Not Integer.TryParse(tbGear.Text, gear) Then
			MsgBox("Invalid input for Gear")
			tbGear.Focus()
			Return
		End If

		If gear < 0 Then
			MsgBox("Invalid input for Gear")
			tbGear.Focus()
			Return
		End If

		If tbInputFile.Text.Length = 0 Then
			MsgBox("Invalid input no file path given")
			tbGear.Focus()
			Return
		End If

		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
		DialogResult = DialogResult.Cancel
		Close()
		Clear()
	End Sub

	Private Sub btAddFilePath_Click(sender As Object, e As EventArgs) Handles btAddFilePath.Click

	End Sub

#End Region

	Private Sub SetDialogTitle(dialogType As IEPCDialogType)
		Select Case dialogType
			Case IEPCDialogType.DragCurveDialog
				Text = "Drag Curve"
			Case IEPCDialogType.PowerMapDialog
				Text = "Power Map"
		End Select
	End Sub

	Private Sub IEPCInputDialog_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		_tbGear.Focus()
	End Sub
End Class