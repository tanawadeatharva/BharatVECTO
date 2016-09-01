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
Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class F_VEH_Axle
	Inherits Form

	'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
	<DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Wird vom Windows Form-Designer benötigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.TableLayoutPanel1 = New TableLayoutPanel()
		Me.OK_Button = New Button()
		Me.Cancel_Button = New Button()
		Me.Label1 = New Label()
		Me.TbAxleShare = New TextBox()
		Me.Label2 = New Label()
		Me.Label3 = New Label()
		Me.Label4 = New Label()
		Me.TbRRC = New TextBox()
		Me.Label5 = New Label()
		Me.Label6 = New Label()
		Me.TbFzISO = New TextBox()
		Me.Label9 = New Label()
		Me.Label10 = New Label()
		Me.CbTwinT = New CheckBox()
		Me.PnAxle = New Panel()
		Me.TbI_wheels = New TextBox()
		Me.Label7 = New Label()
		Me.Label32 = New Label()
		Me.Label8 = New Label()
		Me.CbWheels = New ComboBox()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.PnAxle.SuspendLayout()
		Me.SuspendLayout()
		'
		'TableLayoutPanel1
		'
		Me.TableLayoutPanel1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.TableLayoutPanel1.ColumnCount = 2
		Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
		Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
		Me.TableLayoutPanel1.Location = New Point(250, 217)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 1
		Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New Size(146, 29)
		Me.TableLayoutPanel1.TabIndex = 4
		'
		'OK_Button
		'
		Me.OK_Button.Anchor = AnchorStyles.None
		Me.OK_Button.Location = New Point(3, 3)
		Me.OK_Button.Name = "OK_Button"
		Me.OK_Button.Size = New Size(67, 23)
		Me.OK_Button.TabIndex = 0
		Me.OK_Button.Text = "OK"
		'
		'Cancel_Button
		'
		Me.Cancel_Button.Anchor = AnchorStyles.None
		Me.Cancel_Button.DialogResult = DialogResult.Cancel
		Me.Cancel_Button.Location = New Point(76, 3)
		Me.Cancel_Button.Name = "Cancel_Button"
		Me.Cancel_Button.Size = New Size(67, 23)
		Me.Cancel_Button.TabIndex = 1
		Me.Cancel_Button.Text = "Cancel"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(7, 10)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(96, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "Relative Axle Load"
		'
		'TbAxleShare
		'
		Me.TbAxleShare.Location = New Point(109, 8)
		Me.TbAxleShare.Name = "TbAxleShare"
		Me.TbAxleShare.Size = New Size(53, 20)
		Me.TbAxleShare.TabIndex = 0
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(168, 11)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(40, 13)
		Me.Label2.TabIndex = 1
		Me.Label2.Text = "[-][0..1]"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(41, 108)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(51, 13)
		Me.Label3.TabIndex = 1
		Me.Label3.Text = "RRC ISO"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(188, 108)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(16, 13)
		Me.Label4.TabIndex = 1
		Me.Label4.Text = "[-]"
		'
		'TbRRC
		'
		Me.TbRRC.Location = New Point(98, 105)
		Me.TbRRC.Name = "TbRRC"
		Me.TbRRC.Size = New Size(84, 20)
		Me.TbRRC.TabIndex = 2
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(53, 154)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(39, 13)
		Me.Label5.TabIndex = 1
		Me.Label5.Text = "Fz ISO"
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New Point(188, 154)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New Size(21, 13)
		Me.Label6.TabIndex = 1
		Me.Label6.Text = "[N]"
		'
		'TbFzISO
		'
		Me.TbFzISO.Location = New Point(98, 151)
		Me.TbFzISO.Name = "TbFzISO"
		Me.TbFzISO.Size = New Size(84, 20)
		Me.TbFzISO.TabIndex = 3
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New Point(223, 108)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New Size(146, 13)
		Me.Label9.TabIndex = 1
		Me.Label9.Text = "RRC according to ISO 28580"
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New Point(223, 154)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New Size(167, 26)
		Me.Label10.TabIndex = 1
		Me.Label10.Text = "Test load according to ISO 28580" & ChrW(13) & ChrW(10) & "(85% of max. tyre load capacity)"
		'
		'CbTwinT
		'
		Me.CbTwinT.AutoSize = True
		Me.CbTwinT.Location = New Point(255, 19)
		Me.CbTwinT.Name = "CbTwinT"
		Me.CbTwinT.Size = New Size(78, 17)
		Me.CbTwinT.TabIndex = 1
		Me.CbTwinT.Text = "Twin Tyres"
		Me.CbTwinT.UseVisualStyleBackColor = True
		'
		'PnAxle
		'
		Me.PnAxle.Controls.Add(Me.TbI_wheels)
		Me.PnAxle.Controls.Add(Me.Label1)
		Me.PnAxle.Controls.Add(Me.Label7)
		Me.PnAxle.Controls.Add(Me.Label2)
		Me.PnAxle.Controls.Add(Me.Label32)
		Me.PnAxle.Controls.Add(Me.TbAxleShare)
		Me.PnAxle.Location = New Point(9, 49)
		Me.PnAxle.Name = "PnAxle"
		Me.PnAxle.Size = New Size(394, 39)
		Me.PnAxle.TabIndex = 5
		'
		'TbI_wheels
		'
		Me.TbI_wheels.Location = New Point(292, 8)
		Me.TbI_wheels.Name = "TbI_wheels"
		Me.TbI_wheels.Size = New Size(57, 20)
		Me.TbI_wheels.TabIndex = 26
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New Point(211, 11)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New Size(75, 13)
		Me.Label7.TabIndex = 25
		Me.Label7.Text = "Wheels Inertia"
		'
		'Label32
		'
		Me.Label32.AutoSize = True
		Me.Label32.Location = New Point(355, 11)
		Me.Label32.Name = "Label32"
		Me.Label32.Size = New Size(36, 13)
		Me.Label32.TabIndex = 27
		Me.Label32.Text = "[kgm²]"
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New Point(6, 20)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New Size(43, 13)
		Me.Label8.TabIndex = 6
		Me.Label8.Text = "Wheels"
		'
		'CbWheels
		'
		Me.CbWheels.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbWheels.FormattingEnabled = True
		Me.CbWheels.Location = New Point(55, 17)
		Me.CbWheels.Name = "CbWheels"
		Me.CbWheels.Size = New Size(176, 21)
		Me.CbWheels.TabIndex = 7
		'
		'F_VEH_Axle
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New Size(408, 258)
		Me.Controls.Add(Me.CbWheels)
		Me.Controls.Add(Me.Label8)
		Me.Controls.Add(Me.PnAxle)
		Me.Controls.Add(Me.CbTwinT)
		Me.Controls.Add(Me.TbFzISO)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.TbRRC)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.Label10)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.TableLayoutPanel1)
		Me.FormBorderStyle = FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "F_VEH_Axle"
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Axle configuration"
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.PnAxle.ResumeLayout(False)
		Me.PnAxle.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents TbAxleShare As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents TbRRC As TextBox
	Friend WithEvents Label5 As Label
	Friend WithEvents Label6 As Label
	Friend WithEvents TbFzISO As TextBox
	Friend WithEvents Label9 As Label
	Friend WithEvents Label10 As Label
	Friend WithEvents CbTwinT As CheckBox
	Friend WithEvents PnAxle As Panel
	Friend WithEvents TbI_wheels As TextBox
	Friend WithEvents Label7 As Label
	Friend WithEvents Label32 As Label
	Friend WithEvents Label8 As Label
	Friend WithEvents CbWheels As ComboBox

End Class
