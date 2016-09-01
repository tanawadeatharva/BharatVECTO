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
Option Strict On
Option Explicit On

Imports TUGraz.VectoCore.Models.Declaration


''' <summary>
''' Axle Config Editor (Vehicle Editor sub-dialog)
''' </summary>
Public Class F_VEH_Axle
	Public Sub New()
		InitializeComponent()

		CbWheels.Items.Add("-")
		CbWheels.Items.AddRange(DeclarationData.Wheels.GetWheelsDimensions())
	End Sub

	Public Sub Clear()
		CbTwinT.Checked = False
		TbAxleShare.Text = ""
		TbI_wheels.Text = ""
		TbRRC.Text = ""
		TbFzISO.Text = ""
		CbWheels.SelectedIndex = 0
	End Sub

	'Initialise
	Private Sub F_VEH_Axle_Load(sender As Object, e As EventArgs) Handles Me.Load
		PnAxle.Enabled = Not Cfg.DeclMode
	End Sub

	'Save and close
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click

		If Not Cfg.DeclMode Then
			If Not IsNumeric(TbAxleShare.Text) OrElse Trim(TbAxleShare.Text) = "" Then
				MsgBox("Weight input is not valid!")
				Exit Sub
			End If
		End If

		If Not IsNumeric(TbRRC.Text) OrElse Trim(TbRRC.Text) = "" Then
			MsgBox("RRC input is not valid!")
			Exit Sub
		End If

		If Not IsNumeric(TbFzISO.Text) OrElse Trim(TbFzISO.Text) = "" Then
			MsgBox("Fz ISO input is not valid!")
			Exit Sub
		End If

		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub CbWheels_SelectedIndexChanged(sender As Object, e As EventArgs) Handles CbWheels.SelectedIndexChanged
		'Dim inertia As Double
		If Cfg.DeclMode Then
			'inertia = DeclarationData.Wheels.Lookup(CbWheels.Text).Inertia.Value()
			If CbWheels.Text = "-" Then
				TbI_wheels.Text = "-"
			Else
				TbI_wheels.Text = DeclarationData.Wheels.Lookup(CbWheels.Text).Inertia.Value().ToString()
			End If
		End If
	End Sub

	'Cancel
	Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
		DialogResult = DialogResult.Cancel
		Close()
	End Sub
End Class
