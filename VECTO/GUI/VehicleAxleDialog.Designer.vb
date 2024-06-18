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
Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class VehicleAxleDialog
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
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TbAxleShare = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TbRRC = New System.Windows.Forms.TextBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.TbFzISO = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.CbTwinT = New System.Windows.Forms.CheckBox()
        Me.PnAxle = New System.Windows.Forms.Panel()
        Me.TbI_wheels = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CbWheels = New System.Windows.Forms.ComboBox()
        Me.cbAxleType = New System.Windows.Forms.ComboBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.TbFriction = New System.Windows.Forms.TextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.PnAxle.SuspendLayout()
        Me.SuspendLayout()
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(412, 215)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(195, 36)
        Me.TableLayoutPanel1.TabIndex = 6
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(4, 4)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(89, 28)
        Me.OK_Button.TabIndex = 6
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(101, 4)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(4)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(89, 28)
        Me.Cancel_Button.TabIndex = 7
        Me.Cancel_Button.Text = "Cancel"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(4, 4)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(120, 16)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Relative Axle Load"
        '
        'TbAxleShare
        '
        Me.TbAxleShare.Location = New System.Drawing.Point(140, 0)
        Me.TbAxleShare.Margin = New System.Windows.Forms.Padding(4)
        Me.TbAxleShare.Name = "TbAxleShare"
        Me.TbAxleShare.Size = New System.Drawing.Size(111, 22)
        Me.TbAxleShare.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(260, 4)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(47, 16)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "[-][0..1]"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(73, 116)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(61, 16)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "RRC ISO"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(269, 81)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(19, 16)
        Me.Label4.TabIndex = 1
        Me.Label4.Text = "[-]"
        '
        'TbRRC
        '
        Me.TbRRC.Location = New System.Drawing.Point(149, 112)
        Me.TbRRC.Margin = New System.Windows.Forms.Padding(4)
        Me.TbRRC.Name = "TbRRC"
        Me.TbRRC.Size = New System.Drawing.Size(111, 22)
        Me.TbRRC.TabIndex = 4
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(89, 148)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(46, 16)
        Me.Label5.TabIndex = 1
        Me.Label5.Text = "Fz ISO"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(269, 148)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(25, 16)
        Me.Label6.TabIndex = 1
        Me.Label6.Text = "[N]"
        '
        'TbFzISO
        '
        Me.TbFzISO.Location = New System.Drawing.Point(149, 144)
        Me.TbFzISO.Margin = New System.Windows.Forms.Padding(4)
        Me.TbFzISO.Name = "TbFzISO"
        Me.TbFzISO.Size = New System.Drawing.Size(111, 22)
        Me.TbFzISO.TabIndex = 5
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(331, 116)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(176, 16)
        Me.Label9.TabIndex = 1
        Me.Label9.Text = "RRC according to ISO 28580"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(331, 144)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(204, 32)
        Me.Label10.TabIndex = 1
        Me.Label10.Text = "Test load according to ISO 28580" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "(85% of max. tyre load capacity)"
        '
        'CbTwinT
        '
        Me.CbTwinT.AutoSize = True
        Me.CbTwinT.Location = New System.Drawing.Point(380, 48)
        Me.CbTwinT.Margin = New System.Windows.Forms.Padding(4)
        Me.CbTwinT.Name = "CbTwinT"
        Me.CbTwinT.Size = New System.Drawing.Size(95, 20)
        Me.CbTwinT.TabIndex = 2
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
        Me.PnAxle.Location = New System.Drawing.Point(9, 80)
        Me.PnAxle.Margin = New System.Windows.Forms.Padding(4)
        Me.PnAxle.Name = "PnAxle"
        Me.PnAxle.Size = New System.Drawing.Size(600, 28)
        Me.PnAxle.TabIndex = 3
        '
        'TbI_wheels
        '
        Me.TbI_wheels.Location = New System.Drawing.Point(429, 0)
        Me.TbI_wheels.Margin = New System.Windows.Forms.Padding(4)
        Me.TbI_wheels.Name = "TbI_wheels"
        Me.TbI_wheels.Size = New System.Drawing.Size(111, 22)
        Me.TbI_wheels.TabIndex = 1
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(321, 4)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(92, 16)
        Me.Label7.TabIndex = 25
        Me.Label7.Text = "Wheels Inertia"
        '
        'Label32
        '
        Me.Label32.AutoSize = True
        Me.Label32.Location = New System.Drawing.Point(549, 4)
        Me.Label32.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(45, 16)
        Me.Label32.TabIndex = 27
        Me.Label32.Text = "[kgm²]"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(5, 16)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(102, 16)
        Me.Label8.TabIndex = 6
        Me.Label8.Text = "Tyre Dimension"
        '
        'CbWheels
        '
        Me.CbWheels.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbWheels.FormattingEnabled = True
        Me.CbWheels.Location = New System.Drawing.Point(120, 12)
        Me.CbWheels.Margin = New System.Windows.Forms.Padding(4)
        Me.CbWheels.Name = "CbWheels"
        Me.CbWheels.Size = New System.Drawing.Size(233, 24)
        Me.CbWheels.TabIndex = 0
        '
        'cbAxleType
        '
        Me.cbAxleType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAxleType.FormattingEnabled = True
        Me.cbAxleType.Location = New System.Drawing.Point(120, 46)
        Me.cbAxleType.Margin = New System.Windows.Forms.Padding(4)
        Me.cbAxleType.Name = "cbAxleType"
        Me.cbAxleType.Size = New System.Drawing.Size(233, 24)
        Me.cbAxleType.TabIndex = 1
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(5, 49)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(85, 16)
        Me.Label11.TabIndex = 9
        Me.Label11.Text = "Configuration"
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Location = New System.Drawing.Point(269, 116)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(19, 16)
        Me.Label12.TabIndex = 10
        Me.Label12.Text = "[-]"
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(15, 180)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(119, 16)
        Me.Label13.TabIndex = 11
        Me.Label13.Text = "Wheel End Friction"
        '
        'TbFriction
        '
        Me.TbFriction.Location = New System.Drawing.Point(149, 177)
        Me.TbFriction.Margin = New System.Windows.Forms.Padding(4)
        Me.TbFriction.Name = "TbFriction"
        Me.TbFriction.Size = New System.Drawing.Size(111, 22)
        Me.TbFriction.TabIndex = 12
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(269, 180)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(36, 16)
        Me.Label14.TabIndex = 13
        Me.Label14.Text = "[Nm]"
        '
        'VehicleAxleDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(623, 266)
        Me.Controls.Add(Me.Label14)
        Me.Controls.Add(Me.TbFriction)
        Me.Controls.Add(Me.Label13)
        Me.Controls.Add(Me.Label12)
        Me.Controls.Add(Me.Label11)
        Me.Controls.Add(Me.cbAxleType)
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
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MaximizeBox = False
        Me.MinimizeBox = false
        Me.Name = "VehicleAxleDialog"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Axle configuration"
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.PnAxle.ResumeLayout(false)
        Me.PnAxle.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

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
	Friend WithEvents cbAxleType As System.Windows.Forms.ComboBox
	Friend WithEvents Label11 As System.Windows.Forms.Label
	Friend WithEvents Label12 As System.Windows.Forms.Label
    Friend WithEvents Label13 As Label
    Friend WithEvents TbFriction As TextBox
    Friend WithEvents Label14 As Label
End Class
