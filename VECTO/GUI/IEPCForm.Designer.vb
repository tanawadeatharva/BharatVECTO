<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class IEPCForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IEPCForm))
        Me.Button4 = New System.Windows.Forms.Button()
        Me.TextBox4 = New System.Windows.Forms.TextBox()
        Me.Label52 = New System.Windows.Forms.Label()
        Me.Label48 = New System.Windows.Forms.Label()
        Me.TextBox3 = New System.Windows.Forms.TextBox()
        Me.Label51 = New System.Windows.Forms.Label()
        Me.tbInertia = New System.Windows.Forms.TextBox()
        Me.Label49 = New System.Windows.Forms.Label()
        Me.tbModel = New System.Windows.Forms.TextBox()
        Me.tcVoltageLevels = New System.Windows.Forms.TabControl()
        Me.tpFirstVoltageLevel = New System.Windows.Forms.TabPage()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ListView1 = New System.Windows.Forms.ListView()
        Me.ColumnHeader13 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader14 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Label47 = New System.Windows.Forms.Label()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.tbFLCurve = New System.Windows.Forms.TextBox()
        Me.Label44 = New System.Windows.Forms.Label()
        Me.tpVoltageLevel = New System.Windows.Forms.TabPage()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label37 = New System.Windows.Forms.Label()
        Me.tbVoltage = New System.Windows.Forms.TextBox()
        Me.Label39 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbContinousTorque = New System.Windows.Forms.TextBox()
        Me.tbContinousTorqueSpeed = New System.Windows.Forms.TextBox()
        Me.tbOverloadTorque = New System.Windows.Forms.TextBox()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.TextBox2 = New System.Windows.Forms.TextBox()
        Me.Label41 = New System.Windows.Forms.Label()
        Me.Label42 = New System.Windows.Forms.Label()
        Me.Label43 = New System.Windows.Forms.Label()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.cbDesignTypeWheelMotor = New System.Windows.Forms.CheckBox()
        Me.cbDifferentialIncluded = New System.Windows.Forms.CheckBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tcVoltageLevels.SuspendLayout
        Me.tpFirstVoltageLevel.SuspendLayout
        Me.GroupBox2.SuspendLayout
        Me.TableLayoutPanel1.SuspendLayout
        Me.TableLayoutPanel2.SuspendLayout
        Me.SuspendLayout
        '
        'Button4
        '
        Me.Button4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Button4.Image = CType(resources.GetObject("Button4.Image"),System.Drawing.Image)
        Me.Button4.Location = New System.Drawing.Point(495, 141)
        Me.Button4.Name = "Button4"
        Me.Button4.Size = New System.Drawing.Size(24, 24)
        Me.Button4.TabIndex = 29
        Me.Button4.UseVisualStyleBackColor = true
        '
        'TextBox4
        '
        Me.TextBox4.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TextBox4.Location = New System.Drawing.Point(217, 138)
        Me.TextBox4.Name = "TextBox4"
        Me.TextBox4.Size = New System.Drawing.Size(57, 20)
        Me.TextBox4.TabIndex = 51
        '
        'Label52
        '
        Me.Label52.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label52.AutoSize = true
        Me.Label52.Location = New System.Drawing.Point(38, 142)
        Me.Label52.Name = "Label52"
        Me.Label52.Size = New System.Drawing.Size(173, 13)
        Me.Label52.TabIndex = 50
        Me.Label52.Text = "Thermal Overload Recovery Factor"
        '
        'Label48
        '
        Me.Label48.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label48.AutoSize = true
        Me.Label48.Location = New System.Drawing.Point(4, 115)
        Me.Label48.Name = "Label48"
        Me.Label48.Size = New System.Drawing.Size(207, 13)
        Me.Label48.TabIndex = 48
        Me.Label48.Text = "Nr of Design Type Wheel Motor Measured"
        '
        'TextBox3
        '
        Me.TextBox3.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.TextBox3.Location = New System.Drawing.Point(217, 111)
        Me.TextBox3.Name = "TextBox3"
        Me.TextBox3.Size = New System.Drawing.Size(57, 20)
        Me.TextBox3.TabIndex = 49
        '
        'Label51
        '
        Me.Label51.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label51.AutoSize = true
        Me.Label51.Location = New System.Drawing.Point(175, 34)
        Me.Label51.Name = "Label51"
        Me.Label51.Size = New System.Drawing.Size(36, 13)
        Me.Label51.TabIndex = 45
        Me.Label51.Text = "Inertia"
        '
        'tbInertia
        '
        Me.tbInertia.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.tbInertia.Location = New System.Drawing.Point(217, 30)
        Me.tbInertia.Name = "tbInertia"
        Me.tbInertia.Size = New System.Drawing.Size(57, 20)
        Me.tbInertia.TabIndex = 46
        '
        'Label49
        '
        Me.Label49.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label49.AutoSize = true
        Me.Label49.Location = New System.Drawing.Point(175, 7)
        Me.Label49.Name = "Label49"
        Me.Label49.Size = New System.Drawing.Size(36, 13)
        Me.Label49.TabIndex = 43
        Me.Label49.Text = "Model"
        '
        'tbModel
        '
        Me.tbModel.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.tbModel.Location = New System.Drawing.Point(217, 3)
        Me.tbModel.Name = "tbModel"
        Me.tbModel.Size = New System.Drawing.Size(57, 20)
        Me.tbModel.TabIndex = 44
        '
        'tcVoltageLevels
        '
        Me.tcVoltageLevels.Controls.Add(Me.tpFirstVoltageLevel)
        Me.tcVoltageLevels.Controls.Add(Me.tpVoltageLevel)
        Me.tcVoltageLevels.Location = New System.Drawing.Point(12, 220)
        Me.tcVoltageLevels.Name = "tcVoltageLevels"
        Me.tcVoltageLevels.SelectedIndex = 0
        Me.tcVoltageLevels.Size = New System.Drawing.Size(532, 394)
        Me.tcVoltageLevels.TabIndex = 41
        '
        'tpFirstVoltageLevel
        '
        Me.tpFirstVoltageLevel.Controls.Add(Me.GroupBox2)
        Me.tpFirstVoltageLevel.Controls.Add(Me.Button4)
        Me.tpFirstVoltageLevel.Controls.Add(Me.tbFLCurve)
        Me.tpFirstVoltageLevel.Controls.Add(Me.Label44)
        Me.tpFirstVoltageLevel.Location = New System.Drawing.Point(4, 22)
        Me.tpFirstVoltageLevel.Name = "tpFirstVoltageLevel"
        Me.tpFirstVoltageLevel.Padding = New System.Windows.Forms.Padding(3)
        Me.tpFirstVoltageLevel.Size = New System.Drawing.Size(524, 368)
        Me.tpFirstVoltageLevel.TabIndex = 0
        Me.tpFirstVoltageLevel.Text = "First Voltage Level"
        Me.tpFirstVoltageLevel.UseVisualStyleBackColor = true
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ListView1)
        Me.GroupBox2.Controls.Add(Me.Button1)
        Me.GroupBox2.Controls.Add(Me.Label47)
        Me.GroupBox2.Controls.Add(Me.Button2)
        Me.GroupBox2.Location = New System.Drawing.Point(16, 175)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(249, 125)
        Me.GroupBox2.TabIndex = 60
        Me.GroupBox2.TabStop = false
        Me.GroupBox2.Text = "Power Map Per Gear"
        '
        'ListView1
        '
        Me.ListView1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ListView1.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader13, Me.ColumnHeader14})
        Me.ListView1.FullRowSelect = true
        Me.ListView1.GridLines = true
        Me.ListView1.HideSelection = false
        Me.ListView1.Location = New System.Drawing.Point(6, 16)
        Me.ListView1.MultiSelect = false
        Me.ListView1.Name = "ListView1"
        Me.ListView1.Size = New System.Drawing.Size(237, 72)
        Me.ListView1.TabIndex = 7
        Me.ListView1.TabStop = false
        Me.ListView1.UseCompatibleStateImageBehavior = false
        Me.ListView1.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader13
        '
        Me.ColumnHeader13.Text = "Gear #"
        Me.ColumnHeader13.Width = 59
        '
        'ColumnHeader14
        '
        Me.ColumnHeader14.Text = "Power Map File"
        Me.ColumnHeader14.Width = 172
        '
        'Button1
        '
        Me.Button1.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.Button1.Location = New System.Drawing.Point(6, 94)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(24, 24)
        Me.Button1.TabIndex = 4
        Me.Button1.UseVisualStyleBackColor = true
        '
        'Label47
        '
        Me.Label47.AutoSize = true
        Me.Label47.Location = New System.Drawing.Point(137, 91)
        Me.Label47.Name = "Label47"
        Me.Label47.Size = New System.Drawing.Size(106, 13)
        Me.Label47.TabIndex = 6
        Me.Label47.Text = "(Double-Click to Edit)"
        '
        'Button2
        '
        Me.Button2.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.Button2.Location = New System.Drawing.Point(36, 94)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(24, 24)
        Me.Button2.TabIndex = 5
        Me.Button2.UseVisualStyleBackColor = true
        '
        'tbFLCurve
        '
        Me.tbFLCurve.Location = New System.Drawing.Point(13, 144)
        Me.tbFLCurve.Name = "tbFLCurve"
        Me.tbFLCurve.Size = New System.Drawing.Size(384, 20)
        Me.tbFLCurve.TabIndex = 57
        '
        'Label44
        '
        Me.Label44.AutoSize = true
        Me.Label44.Location = New System.Drawing.Point(13, 128)
        Me.Label44.Name = "Label44"
        Me.Label44.Size = New System.Drawing.Size(115, 13)
        Me.Label44.TabIndex = 59
        Me.Label44.Text = "Fuel Consumption Map"
        '
        'tpVoltageLevel
        '
        Me.tpVoltageLevel.Location = New System.Drawing.Point(4, 22)
        Me.tpVoltageLevel.Name = "tpVoltageLevel"
        Me.tpVoltageLevel.Padding = New System.Windows.Forms.Padding(3)
        Me.tpVoltageLevel.Size = New System.Drawing.Size(524, 368)
        Me.tpVoltageLevel.TabIndex = 1
        Me.tpVoltageLevel.Text = "Secondary Voltage Level"
        Me.tpVoltageLevel.UseVisualStyleBackColor = true
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.ColumnCount = 6
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 65.83851!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 65!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 101!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 87!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20!))
        Me.TableLayoutPanel1.Controls.Add(Me.Label4, 2, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label37, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.tbVoltage, 1, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label39, 0, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label1, 0, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.tbContinousTorque, 1, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.tbContinousTorqueSpeed, 1, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.tbOverloadTorque, 4, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox1, 4, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.TextBox2, 4, 2)
        Me.TableLayoutPanel1.Controls.Add(Me.Label41, 3, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Label42, 3, 1)
        Me.TableLayoutPanel1.Controls.Add(Me.Label43, 3, 2)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(318, 58)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 3
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(439, 82)
        Me.TableLayoutPanel1.TabIndex = 61
        '
        'Label37
        '
        Me.Label37.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label37.AutoSize = true
        Me.Label37.Location = New System.Drawing.Point(94, 7)
        Me.Label37.Name = "Label37"
        Me.Label37.Size = New System.Drawing.Size(43, 13)
        Me.Label37.TabIndex = 1
        Me.Label37.Text = "Voltage"
        '
        'tbVoltage
        '
        Me.tbVoltage.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbVoltage.Location = New System.Drawing.Point(144, 3)
        Me.tbVoltage.Name = "tbVoltage"
        Me.tbVoltage.Size = New System.Drawing.Size(57, 20)
        Me.tbVoltage.TabIndex = 2
        '
        'Label39
        '
        Me.Label39.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label39.AutoSize = true
        Me.Label39.Location = New System.Drawing.Point(40, 34)
        Me.Label39.Name = "Label39"
        Me.Label39.Size = New System.Drawing.Size(97, 13)
        Me.Label39.TabIndex = 3
        Me.Label39.Text = "Continuous Torque"
        '
        'Label1
        '
        Me.Label1.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(6, 61)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(131, 13)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Continuous Torque Speed"
        '
        'tbContinousTorque
        '
        Me.tbContinousTorque.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbContinousTorque.Location = New System.Drawing.Point(144, 30)
        Me.tbContinousTorque.Name = "tbContinousTorque"
        Me.tbContinousTorque.Size = New System.Drawing.Size(57, 20)
        Me.tbContinousTorque.TabIndex = 4
        '
        'tbContinousTorqueSpeed
        '
        Me.tbContinousTorqueSpeed.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbContinousTorqueSpeed.Location = New System.Drawing.Point(144, 58)
        Me.tbContinousTorqueSpeed.Name = "tbContinousTorqueSpeed"
        Me.tbContinousTorqueSpeed.Size = New System.Drawing.Size(57, 20)
        Me.tbContinousTorqueSpeed.TabIndex = 6
        '
        'tbOverloadTorque
        '
        Me.tbOverloadTorque.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbOverloadTorque.Location = New System.Drawing.Point(347, 3)
        Me.tbOverloadTorque.Name = "tbOverloadTorque"
        Me.tbOverloadTorque.Size = New System.Drawing.Size(57, 20)
        Me.tbOverloadTorque.TabIndex = 8
        '
        'TextBox1
        '
        Me.TextBox1.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TextBox1.Location = New System.Drawing.Point(347, 30)
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.Size = New System.Drawing.Size(57, 20)
        Me.TextBox1.TabIndex = 10
        '
        'TextBox2
        '
        Me.TextBox2.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TextBox2.Location = New System.Drawing.Point(347, 58)
        Me.TextBox2.Name = "TextBox2"
        Me.TextBox2.Size = New System.Drawing.Size(57, 20)
        Me.TextBox2.TabIndex = 12
        '
        'Label41
        '
        Me.Label41.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label41.AutoSize = true
        Me.Label41.Location = New System.Drawing.Point(242, 7)
        Me.Label41.Name = "Label41"
        Me.Label41.Size = New System.Drawing.Size(87, 13)
        Me.Label41.TabIndex = 7
        Me.Label41.Text = "Overload Torque"
        '
        'Label42
        '
        Me.Label42.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label42.AutoSize = true
        Me.Label42.Location = New System.Drawing.Point(239, 27)
        Me.Label42.Name = "Label42"
        Me.Label42.Size = New System.Drawing.Size(90, 26)
        Me.Label42.TabIndex = 9
        Me.Label42.Text = "Overload Torque Speed"
        '
        'Label43
        '
        Me.Label43.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label43.AutoSize = true
        Me.Label43.Location = New System.Drawing.Point(253, 61)
        Me.Label43.Name = "Label43"
        Me.Label43.Size = New System.Drawing.Size(76, 13)
        Me.Label43.TabIndex = 11
        Me.Label43.Text = "Overload Time"
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 2
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 214!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 306!))
        Me.TableLayoutPanel2.Controls.Add(Me.Label3, 0, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.Label52, 0, 5)
        Me.TableLayoutPanel2.Controls.Add(Me.TextBox4, 1, 5)
        Me.TableLayoutPanel2.Controls.Add(Me.Label48, 0, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.Label2, 0, 2)
        Me.TableLayoutPanel2.Controls.Add(Me.cbDesignTypeWheelMotor, 1, 3)
        Me.TableLayoutPanel2.Controls.Add(Me.tbModel, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.TextBox3, 1, 4)
        Me.TableLayoutPanel2.Controls.Add(Me.Label49, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.tbInertia, 1, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.Label51, 0, 1)
        Me.TableLayoutPanel2.Controls.Add(Me.cbDifferentialIncluded, 1, 2)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(12, 12)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 6
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(280, 162)
        Me.TableLayoutPanel2.TabIndex = 52
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(30, 88)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(181, 13)
        Me.Label3.TabIndex = 54
        Me.Label3.Text = "Design Type Wheel Motor Measured"
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Right
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(110, 61)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(101, 13)
        Me.Label2.TabIndex = 53
        Me.Label2.Text = "Differential Included"
        '
        'cbDesignTypeWheelMotor
        '
        Me.cbDesignTypeWheelMotor.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.cbDesignTypeWheelMotor.AutoSize = true
        Me.cbDesignTypeWheelMotor.Location = New System.Drawing.Point(217, 87)
        Me.cbDesignTypeWheelMotor.Name = "cbDesignTypeWheelMotor"
        Me.cbDesignTypeWheelMotor.Size = New System.Drawing.Size(15, 14)
        Me.cbDesignTypeWheelMotor.TabIndex = 53
        Me.cbDesignTypeWheelMotor.UseVisualStyleBackColor = true
        '
        'cbDifferentialIncluded
        '
        Me.cbDifferentialIncluded.Anchor = System.Windows.Forms.AnchorStyles.Left
        Me.cbDifferentialIncluded.AutoSize = true
        Me.cbDifferentialIncluded.Location = New System.Drawing.Point(217, 60)
        Me.cbDifferentialIncluded.Name = "cbDifferentialIncluded"
        Me.cbDifferentialIncluded.Size = New System.Drawing.Size(15, 14)
        Me.cbDifferentialIncluded.TabIndex = 47
        Me.cbDifferentialIncluded.UseVisualStyleBackColor = true
        '
        'Label4
        '
        Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(208, 7)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(20, 13)
        Me.Label4.TabIndex = 62
        Me.Label4.Text = "[V]"
        '
        'IEPCForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 626)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.tcVoltageLevels)
        Me.Name = "IEPCForm"
        Me.Text = "IEPCForm"
        Me.tcVoltageLevels.ResumeLayout(false)
        Me.tpFirstVoltageLevel.ResumeLayout(false)
        Me.tpFirstVoltageLevel.PerformLayout
        Me.GroupBox2.ResumeLayout(false)
        Me.GroupBox2.PerformLayout
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.TableLayoutPanel1.PerformLayout
        Me.TableLayoutPanel2.ResumeLayout(false)
        Me.TableLayoutPanel2.PerformLayout
        Me.ResumeLayout(false)

End Sub
    Friend WithEvents Button4 As Button
    Friend WithEvents TextBox4 As TextBox
    Friend WithEvents Label52 As Label
    Friend WithEvents Label48 As Label
    Friend WithEvents TextBox3 As TextBox
    Friend WithEvents Label51 As Label
    Friend WithEvents tbInertia As TextBox
    Friend WithEvents Label49 As Label
    Friend WithEvents tbModel As TextBox
    Friend WithEvents tcVoltageLevels As TabControl
    Friend WithEvents tpFirstVoltageLevel As TabPage
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents ListView1 As ListView
    Friend WithEvents ColumnHeader13 As ColumnHeader
    Friend WithEvents ColumnHeader14 As ColumnHeader
    Friend WithEvents Button1 As Button
    Friend WithEvents Label47 As Label
    Friend WithEvents Button2 As Button
    Friend WithEvents tbFLCurve As TextBox
    Friend WithEvents Label44 As Label
    Friend WithEvents Label43 As Label
    Friend WithEvents TextBox2 As TextBox
    Friend WithEvents Label42 As Label
    Friend WithEvents TextBox1 As TextBox
    Friend WithEvents Label41 As Label
    Friend WithEvents tbOverloadTorque As TextBox
    Friend WithEvents tbContinousTorqueSpeed As TextBox
    Friend WithEvents Label39 As Label
    Friend WithEvents tbContinousTorque As TextBox
    Friend WithEvents Label37 As Label
    Friend WithEvents tbVoltage As TextBox
    Friend WithEvents tpVoltageLevel As TabPage
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents Label1 As Label
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents Label2 As Label
    Friend WithEvents cbDesignTypeWheelMotor As CheckBox
    Friend WithEvents cbDifferentialIncluded As CheckBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
End Class
