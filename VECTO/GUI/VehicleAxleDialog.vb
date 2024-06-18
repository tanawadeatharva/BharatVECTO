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
Option Infer On
Option Strict On
Option Explicit On

Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.Declaration


''' <summary>
''' Axle Config Editor (Vehicle Editor sub-dialog)
''' </summary>
Public Class VehicleAxleDialog

	Private _storedFriction As String
	Public _stdFriction As Double = INVALID_FRICTION

	Public Const INVALID_FRICTION As Integer = -1

	Public Sub New()
		InitializeComponent()

		CbWheels.Items.Add("-")
		CbWheels.Items.AddRange(DeclarationData.Wheels.GetWheelsDimensions().OrderBy(Function(s) s).ToArray())

		cbAxleType.Items.Clear()
		cbAxleType.ValueMember = "Value"
		cbAxleType.DisplayMember = "Label"

		cbAxleType.DataSource = [Enum].GetValues(GetType(AxleType)) _
			.Cast(Of AxleType)() _
			.Where(Function(type) Not Cfg.DeclMode OrElse type <> AxleType.Trailer) _
			.Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()

	End Sub

	Public Sub Clear()
		CbTwinT.Checked = False
		TbAxleShare.Text = ""
		TbI_wheels.Text = ""
		TbRRC.Text = ""
		TbFzISO.Text = ""
		TbFriction.Text = ""
		CbWheels.SelectedIndex = 0
		_storedFriction = ""
	End Sub

	'Initialise
	Private Sub F_VEH_Axle_Load(sender As Object, e As EventArgs) Handles Me.Load
		PnAxle.Enabled = Not Cfg.DeclMode
	End Sub

	'Save and close
	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click

		Dim frVal = TbFriction.Text.Trim()
		If (Not ValidateWheelEndFriction(frVal)) Then
			MsgBox("Invalid input:" + Environment.NewLine +
				   "Wheel End Friction must either be empty or within [0, " + _stdFriction.ToString() + "].",
				   MsgBoxStyle.OkOnly, "Failed to save axle gear")
			Exit Sub
		End If

		Dim axleData As Axle = New Axle With {
				.AxleWeightShare = TbAxleShare.Text.ToDouble(0),
				.RollResistanceCoefficient = TbRRC.Text.ToDouble(0),
				.TyreTestLoad = TbFzISO.Text.ToDouble(0).SI(Of Newton)(),
				.TwinTyres = CbTwinT.Checked,
				.WheelsDimension = If(IsNothing(CbWheels.SelectedItem), "", CbWheels.SelectedItem.ToString()),
				.Inertia = TbI_wheels.Text.ToDouble(0).SI(Of KilogramSquareMeter)(),
				.AxleType = CType(If(IsNothing(cbAxleType.SelectedValue), AxleType.VehicleNonDriven, cbAxleType.SelectedValue), AxleType)
				}

		Dim results As IList(Of ValidationResult) =
				axleData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), VectoSimulationJobType.ConventionalVehicle, Nothing, Nothing, False)

		If results.Any() Then
			Dim messages As IEnumerable(Of String) =
					results.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
			MsgBox("Invalid input:" + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
					"Failed to save axle gear")
			Exit Sub
		End If

		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Function ValidateWheelEndFriction(frVal As String) As Boolean
		If (frVal Is "") Then
			Return True
		End If

		Dim result As Double = -1
		Return If(Double.TryParse(frVal, result), (result >= 0) And (result <= _stdFriction), False)
	End Function

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

	Private Sub cbAxleType_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cbAxleType.SelectedIndexChanged
		Dim v1 = CType(cbAxleType.SelectedValue, AxleType) = AxleType.VehicleDriven
		Dim v2 = CType(cbAxleType.SelectedValue, AxleType) = AxleType.Trailer

		If (v1 Or v2 Or (_stdFriction = INVALID_FRICTION)) Then
			_storedFriction = TbFriction.Text
			TbFriction.Text = ""
			TbFriction.Enabled = False
		End If

		If ((CType(cbAxleType.SelectedValue, AxleType) = AxleType.VehicleNonDriven) And (_stdFriction <> INVALID_FRICTION)) Then
			TbFriction.Enabled = True
			TbFriction.Text = _storedFriction
		End If
	End Sub
End Class
