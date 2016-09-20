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
Imports System.Windows.Forms
Imports TUGraz.VectoCommon.Utils


''' <summary>
''' Gear Editor (Vehicle Editor sub-dialog)
''' </summary>
''' <remarks></remarks>
Public Class GearboxGearDialog
	Public NextGear As Boolean
	Public PreviousGear As Boolean
	Public GbxPath As String

	'Save and Close
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click

		If Not IsNumeric(TbRatio.Text) Then
			MsgBox("Gear ratio is invalid!")
			TbRatio.Focus()
			TbRatio.SelectAll()
			Exit Sub
		End If

		If IsNumeric(TbMapPath.Text) AndAlso (TbMapPath.Text.ToDouble() < 0 OrElse TbMapPath.Text.ToDouble() > 1) Then
			MsgBox("Efficiency is invalid! Must be between 0 and 1.")
			TbMapPath.Focus()
			TbMapPath.SelectAll()
			Exit Sub
		End If

		NextGear = False
		PreviousGear = False
		DialogResult = DialogResult.OK
		Close()
	End Sub

	'Cancel
	Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
		NextGear = False
		PreviousGear = False
		DialogResult = DialogResult.Cancel
		Close()
	End Sub

	'Next Gear button - Close dialog and open for next gear
	Private Sub BtNext_Click(sender As Object, e As EventArgs) Handles BtNext.Click

		If Not IsNumeric(TbRatio.Text) Then
			MsgBox("Gear ratio is invalid!")
			TbRatio.Focus()
			TbRatio.SelectAll()
			Exit Sub
		End If

		If IsNumeric(TbMapPath.Text) AndAlso (TbMapPath.Text.ToDouble() < 0 OrElse TbMapPath.Text.ToDouble() > 1) Then
			MsgBox("Efficiency is invalid! Must be between 0 and 1.")
			TbMapPath.Focus()
			TbMapPath.SelectAll()
			Exit Sub
		End If

		NextGear = True
		PreviousGear = False
		DialogResult = DialogResult.OK
		Close()
	End Sub

	'Browse for transmission loss map
	Private Sub BtBrowse_Click(sender As Object, e As EventArgs) Handles BtBrowse.Click
		If TransmissionLossMapFileBrowser.OpenDialog(FileRepl(TbMapPath.Text, GbxPath)) Then
			TbMapPath.Text = GetFilenameWithoutDirectory(TransmissionLossMapFileBrowser.Files(0), GbxPath)
		End If
	End Sub

	'Browse for shift polygons file
	Private Sub BtShiftPolyBrowse_Click(sender As Object, e As EventArgs) Handles BtShiftPolyBrowse.Click
		If GearboxShiftPolygonFileBrowser.OpenDialog(FileRepl(TbShiftPolyFile.Text, GbxPath)) Then
			TbShiftPolyFile.Text = GetFilenameWithoutDirectory(GearboxShiftPolygonFileBrowser.Files(0), GbxPath)
		End If
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles BtPrevious.Click
		If Not IsNumeric(TbRatio.Text) Then
			MsgBox("Gear ratio is invalid!")
			TbRatio.Focus()
			TbRatio.SelectAll()
			Exit Sub
		End If

		If IsNumeric(TbMapPath.Text) AndAlso (TbMapPath.Text.ToDouble() < 0 OrElse TbMapPath.Text.ToDouble() > 1) Then
			MsgBox("Efficiency is invalid! Must be between 0 and 1.")
			TbMapPath.Focus()
			TbMapPath.SelectAll()
			Exit Sub
		End If

		PreviousGear = True
		NextGear = False
		DialogResult = DialogResult.OK
		Close()
	End Sub
End Class
