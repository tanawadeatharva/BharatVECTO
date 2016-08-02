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
Option Infer On

Imports System.Windows.Forms

''' <summary>
''' Aux Config Editor (Job Editor sub-dialog)
''' </summary>
''' <remarks></remarks>
Public Class F_VEH_AuxDlog
	Public VehPath As String = ""

	Public Sub New()
		InitializeComponent()

		CbType.Items.Add("Fan")
		CbType.Items.Add("Steering pump")
		CbType.Items.Add("HVAC")
		CbType.Items.Add("Electric System")
		PnFile.Enabled = Not Cfg.DeclMode
		PnTech.Enabled = Cfg.DeclMode
	End Sub

	'Initialise form
	Private Sub F_VEH_AuxDlog_Load(sender As Object, e As EventArgs) Handles Me.Load
		Me.Text = CbType.Text
	End Sub

	'Set generic values for Declaration mode
	Private Sub DeclInit()
		CbTech.Items.Clear()
		Select Case TbID.Text
			Case sKey.AUX.Fan
				For Each txt In Declaration.AuxTechs(tAux.Fan)
					CbTech.Items.Add(txt)
				Next

			Case sKey.AUX.SteerPump
				For Each txt In Declaration.AuxTechs(tAux.SteerPump)
					CbTech.Items.Add(txt)
				Next

			Case sKey.AUX.HVAC
				For Each txt In Declaration.AuxTechs(tAux.HVAC)
					CbTech.Items.Add(txt)
				Next
				CbTech.SelectedIndex = 0

			Case sKey.AUX.ElecSys
				For Each txt In Declaration.AuxTechs(tAux.ElectricSys)
					CbTech.Items.Add(txt)
				Next
				CbTech.SelectedIndex = 0

			Case Else 'sKey.AUX.PneumSys
				For Each txt In Declaration.AuxTechs(tAux.PneumSys)
					CbTech.Items.Add(txt)
				Next
				CbTech.SelectedIndex = 0
		End Select
	End Sub

	'Close form. Check if form is complete and valid
	Private Sub F_VEH_AuxDlog_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.WindowsShutDown And Me.DialogResult <> DialogResult.Cancel Then

			If Trim(Me.TbID.Text) = "" Or Trim(Me.CbType.Text) = "" Then
				MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
				e.Cancel = True
			End If

			If Me.TbID.Text.Contains(",") Or Me.CbType.Text.Contains(",") Or Me.TbPath.Text.Contains(",") Then
				MsgBox("',' is no valid character!", MsgBoxStyle.Critical)
				e.Cancel = True
			End If

			If Cfg.DeclMode Then

				If Me.CbTech.Text = "" Then
					MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
					e.Cancel = True
				End If

			Else

				If Trim(Me.TbPath.Text) = "" Then
					MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
					e.Cancel = True
				End If

			End If

		End If
	End Sub

	'Browse for .vaux files
	Private Sub BtBrowse_Click(sender As Object, e As EventArgs) Handles BtBrowse.Click
		If fbAUX.OpenDialog(fFileRepl(Me.TbPath.Text, VehPath)) Then Me.TbPath.Text = fFileWoDir(fbAUX.Files(0), VehPath)
	End Sub

	'Update ID when Aux Type was changed
	Private Sub CbType_TextChanged(sender As Object, e As EventArgs) Handles CbType.TextChanged

		If Me.CbType.Text = "" Then
			Me.TbID.Text = ""
		Else
			If Cfg.DeclMode Then
				Select Case Me.CbType.SelectedIndex
					Case 0
						Me.TbID.Text = sKey.AUX.Fan
					Case 1
						Me.TbID.Text = sKey.AUX.SteerPump

					Case Else '2
						Me.TbID.Text = sKey.AUX.HVAC

				End Select
			Else
				Me.TbID.Text = Trim(UCase(Me.CbType.Text.Substring(0, CInt(Math.Min(Me.CbType.Text.Length, 3)))))
			End If
		End If
	End Sub

	'Update help label if ID was changed
	Private Sub TbID_TextChanged(sender As Object, e As EventArgs) Handles TbID.TextChanged

		DeclInit()

		If Trim(Me.TbID.Text) = "" Or Cfg.DeclMode Then
			Me.LbIDhelp.Text = ""
		Else
			Me.LbIDhelp.Text = "Header in Driving cycle: <AUX_" & Trim(Me.TbID.Text) & ">"
		End If
	End Sub
End Class
