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
Public Class F_VEH_AuxDlog
	Public VehPath As String = ""

	Public Sub New()
		InitializeComponent()

		CbType.Items.Add("Fan")
		CbType.Items.Add("Steering pump")
		CbType.Items.Add("HVAC")
		CbType.Items.Add("Electric System")
		PnTech.Visible = Cfg.DeclMode
		PnFile.Visible = Not Cfg.DeclMode
	End Sub

	'Initialise form
	Private Sub F_VEH_AuxDlog_Load(sender As Object, e As EventArgs) Handles Me.Load
		Text = CbType.Text
	End Sub

	'Set generic values for Declaration mode
	Private Sub DeclInit()
		CbTech.Items.Clear()
		Select Case TbID.Text
			Case sKey.AUX.Fan
				CbTech.Items.AddRange(Declaration.AuxTechs(tAux.Fan).ToArray())
			Case sKey.AUX.SteerPump
				CbTech.Items.AddRange(Declaration.AuxTechs(tAux.SteerPump).ToArray())
			Case sKey.AUX.HVAC
				CbTech.Items.AddRange(Declaration.AuxTechs(tAux.HVAC).ToArray())
			Case sKey.AUX.ElecSys
				CbTech.Items.AddRange(Declaration.AuxTechs(tAux.ElectricSys).ToArray())
			Case Else 'sKey.AUX.PneumSys
				CbTech.Items.AddRange(Declaration.AuxTechs(tAux.PneumSys).ToArray())
		End Select
		If CbTech.Items.Count > 0 Then
			CbTech.SelectedIndex = 0
			PnTech.Enabled = True
		Else
			PnTech.Enabled = False
		End If
	End Sub

	'Close form. Check if form is complete and valid
	Private Sub F_VEH_AuxDlog_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.WindowsShutDown And DialogResult <> DialogResult.Cancel Then

			If Trim(TbID.Text) = "" Or Trim(CbType.Text) = "" Then
				MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
				e.Cancel = True
			End If

			If TbID.Text.Contains(",") Or CbType.Text.Contains(",") Or TbPath.Text.Contains(",") Then
				MsgBox("',' is no valid character!", MsgBoxStyle.Critical)
				e.Cancel = True
			End If

			If Cfg.DeclMode Then

				If CbTech.Items.Count > 0 AndAlso CbTech.Text = "" Then
					MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
					e.Cancel = True
				End If

			Else

				If Trim(TbPath.Text) = "" Then
					MsgBox("Form is incomplete!", MsgBoxStyle.Critical)
					e.Cancel = True
				End If

			End If

		End If
	End Sub

	'Browse for .vaux files
	Private Sub BtBrowse_Click(sender As Object, e As EventArgs) Handles BtBrowse.Click
		If fbAUX.OpenDialog(fFileRepl(TbPath.Text, VehPath)) Then TbPath.Text = fFileWoDir(fbAUX.Files(0), VehPath)
	End Sub

	'Update ID when Aux Type was changed
	Private Sub CbType_TextChanged(sender As Object, e As EventArgs) Handles CbType.TextChanged

		If CbType.Text = "" Then
			TbID.Text = ""
		Else
			If Cfg.DeclMode Then
				Select Case CbType.SelectedIndex
					Case 0
						TbID.Text = sKey.AUX.Fan
					Case 1
						TbID.Text = sKey.AUX.SteerPump

					Case Else '2
						TbID.Text = sKey.AUX.HVAC

				End Select
			Else
				TbID.Text = Trim(UCase(CbType.Text.Substring(0, CInt(Math.Min(CbType.Text.Length, 3)))))
			End If
		End If
	End Sub

	'Update help label if ID was changed
	Private Sub TbID_TextChanged(sender As Object, e As EventArgs) Handles TbID.TextChanged

		DeclInit()

		If Trim(TbID.Text) = "" Or Cfg.DeclMode Then
			LbIDhelp.Text = ""
		Else
			LbIDhelp.Text = "Header in Driving cycle: <AUX_" & Trim(TbID.Text) & ">"
		End If
	End Sub
End Class
