<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FuelCellComponentForm
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
        Me.pcBoxMassFlowMap = New System.Windows.Forms.PictureBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.tbMinElectricPower = New System.Windows.Forms.TextBox()
        Me.pnMaxElectricPower = New System.Windows.Forms.TableLayoutPanel()
        Me.tbMaxElectricPower = New System.Windows.Forms.TextBox()
        Me.lblMaxElectricPower = New System.Windows.Forms.Label()
        Me.pnModel = New System.Windows.Forms.TableLayoutPanel()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.pnManufacturer = New System.Windows.Forms.TableLayoutPanel()
        Me.lblManufacturer = New System.Windows.Forms.Label()
        Me.lblMinElectricPower = New System.Windows.Forms.Label()
        Me.tbModel = New System.Windows.Forms.TextBox()
        Me.tbManufacturer = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.gpMassFlowMap = New System.Windows.Forms.GroupBox()
        Me.tbRiCurve = New System.Windows.Forms.TextBox()
        Me.btnRiMapOpen = New System.Windows.Forms.Button()
        Me.btnBrowseRiMap = New System.Windows.Forms.Button()
        Me.Button1 = New System.Windows.Forms.Button()
        Me.Button2 = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.pcBoxMassFlowMap, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel2.SuspendLayout()
        Me.pnMaxElectricPower.SuspendLayout()
        Me.pnModel.SuspendLayout()
        Me.pnManufacturer.SuspendLayout()
        Me.gpMassFlowMap.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pcBoxMassFlowMap
        '
        Me.pcBoxMassFlowMap.BackColor = System.Drawing.Color.LightGray
        Me.pcBoxMassFlowMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pcBoxMassFlowMap.Location = New System.Drawing.Point(403, 22)
        Me.pcBoxMassFlowMap.Name = "pcBoxMassFlowMap"
        Me.pcBoxMassFlowMap.Size = New System.Drawing.Size(382, 266)
        Me.pcBoxMassFlowMap.TabIndex = 50
        Me.pcBoxMassFlowMap.TabStop = False
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!)
        Me.lblTitle.Location = New System.Drawing.Point(12, 22)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(388, 29)
        Me.lblTitle.TabIndex = 54
        Me.lblTitle.Text = "Fuel Cell Component"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'TableLayoutPanel2
        '
        Me.TableLayoutPanel2.ColumnCount = 3
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454!))
        Me.TableLayoutPanel2.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090909!))
        Me.TableLayoutPanel2.Controls.Add(Me.tbMinElectricPower, 1, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.lblMinElectricPower, 0, 0)
        Me.TableLayoutPanel2.Controls.Add(Me.Label3, 2, 0)
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(12, 115)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(388, 25)
        Me.TableLayoutPanel2.TabIndex = 56
        '
        'tbMinElectricPower
        '
        Me.tbMinElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbMinElectricPower.Location = New System.Drawing.Point(179, 3)
        Me.tbMinElectricPower.Name = "tbMinElectricPower"
        Me.tbMinElectricPower.Size = New System.Drawing.Size(170, 20)
        Me.tbMinElectricPower.TabIndex = 4
        '
        'pnMaxElectricPower
        '
        Me.pnMaxElectricPower.ColumnCount = 3
        Me.pnMaxElectricPower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454!))
        Me.pnMaxElectricPower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454!))
        Me.pnMaxElectricPower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090909!))
        Me.pnMaxElectricPower.Controls.Add(Me.tbMaxElectricPower, 0, 0)
        Me.pnMaxElectricPower.Controls.Add(Me.lblMaxElectricPower, 0, 0)
        Me.pnMaxElectricPower.Controls.Add(Me.Label7, 2, 0)
        Me.pnMaxElectricPower.Location = New System.Drawing.Point(12, 146)
        Me.pnMaxElectricPower.Name = "pnMaxElectricPower"
        Me.pnMaxElectricPower.RowCount = 1
        Me.pnMaxElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnMaxElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20.0!))
        Me.pnMaxElectricPower.Size = New System.Drawing.Size(388, 25)
        Me.pnMaxElectricPower.TabIndex = 58
        '
        'tbMaxElectricPower
        '
        Me.tbMaxElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbMaxElectricPower.Location = New System.Drawing.Point(179, 3)
        Me.tbMaxElectricPower.Name = "tbMaxElectricPower"
        Me.tbMaxElectricPower.Size = New System.Drawing.Size(170, 20)
        Me.tbMaxElectricPower.TabIndex = 5
        '
        'lblMaxElectricPower
        '
        Me.lblMaxElectricPower.AutoSize = True
        Me.lblMaxElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMaxElectricPower.Location = New System.Drawing.Point(0, 0)
        Me.lblMaxElectricPower.Margin = New System.Windows.Forms.Padding(0)
        Me.lblMaxElectricPower.Name = "lblMaxElectricPower"
        Me.lblMaxElectricPower.Size = New System.Drawing.Size(176, 25)
        Me.lblMaxElectricPower.TabIndex = 3
        Me.lblMaxElectricPower.Text = "Max Electric Power"
        Me.lblMaxElectricPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnModel
        '
        Me.pnModel.ColumnCount = 2
        Me.pnModel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.pnModel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.pnModel.Controls.Add(Me.tbModel, 0, 0)
        Me.pnModel.Controls.Add(Me.lblModel, 0, 0)
        Me.pnModel.Location = New System.Drawing.Point(12, 84)
        Me.pnModel.Name = "pnModel"
        Me.pnModel.RowCount = 1
        Me.pnModel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnModel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.pnModel.Size = New System.Drawing.Size(388, 25)
        Me.pnModel.TabIndex = 59
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblModel.Location = New System.Drawing.Point(0, 0)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(194, 25)
        Me.lblModel.TabIndex = 1
        Me.lblModel.Text = "Model"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnManufacturer
        '
        Me.pnManufacturer.ColumnCount = 2
        Me.pnManufacturer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.pnManufacturer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.pnManufacturer.Controls.Add(Me.tbManufacturer, 0, 0)
        Me.pnManufacturer.Controls.Add(Me.lblManufacturer, 0, 0)
        Me.pnManufacturer.Location = New System.Drawing.Point(12, 54)
        Me.pnManufacturer.Name = "pnManufacturer"
        Me.pnManufacturer.RowCount = 1
        Me.pnManufacturer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnManufacturer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25.0!))
        Me.pnManufacturer.Size = New System.Drawing.Size(388, 25)
        Me.pnManufacturer.TabIndex = 60
        '
        'lblManufacturer
        '
        Me.lblManufacturer.AutoSize = True
        Me.lblManufacturer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblManufacturer.Location = New System.Drawing.Point(0, 0)
        Me.lblManufacturer.Margin = New System.Windows.Forms.Padding(0)
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Size = New System.Drawing.Size(194, 25)
        Me.lblManufacturer.TabIndex = 0
        Me.lblManufacturer.Text = "Manufacturer"
        Me.lblManufacturer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'lblMinElectricPower
        '
        Me.lblMinElectricPower.AutoSize = True
        Me.lblMinElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMinElectricPower.Location = New System.Drawing.Point(0, 0)
        Me.lblMinElectricPower.Margin = New System.Windows.Forms.Padding(0)
        Me.lblMinElectricPower.Name = "lblMinElectricPower"
        Me.lblMinElectricPower.Size = New System.Drawing.Size(176, 25)
        Me.lblMinElectricPower.TabIndex = 5
        Me.lblMinElectricPower.Text = "Min Electric Power"
        Me.lblMinElectricPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbModel
        '
        Me.tbModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbModel.Location = New System.Drawing.Point(197, 3)
        Me.tbModel.Name = "tbModel"
        Me.tbModel.Size = New System.Drawing.Size(188, 20)
        Me.tbModel.TabIndex = 5
        '
        'tbManufacturer
        '
        Me.tbManufacturer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbManufacturer.Location = New System.Drawing.Point(197, 3)
        Me.tbManufacturer.Name = "tbManufacturer"
        Me.tbManufacturer.Size = New System.Drawing.Size(188, 20)
        Me.tbManufacturer.TabIndex = 5
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(352, 0)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 25)
        Me.Label3.TabIndex = 6
        Me.Label3.Text = "[kW]"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(352, 0)
        Me.Label7.Margin = New System.Windows.Forms.Padding(0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(36, 25)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "[kW]"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gpMassFlowMap
        '
        Me.gpMassFlowMap.Controls.Add(Me.tbRiCurve)
        Me.gpMassFlowMap.Controls.Add(Me.btnRiMapOpen)
        Me.gpMassFlowMap.Controls.Add(Me.btnBrowseRiMap)
        Me.gpMassFlowMap.Location = New System.Drawing.Point(6, 188)
        Me.gpMassFlowMap.Name = "gpMassFlowMap"
        Me.gpMassFlowMap.Size = New System.Drawing.Size(394, 49)
        Me.gpMassFlowMap.TabIndex = 65
        Me.gpMassFlowMap.TabStop = False
        Me.gpMassFlowMap.Text = "MassFlowMap"
        '
        'tbRiCurve
        '
        Me.tbRiCurve.Location = New System.Drawing.Point(6, 19)
        Me.tbRiCurve.Name = "tbRiCurve"
        Me.tbRiCurve.Size = New System.Drawing.Size(333, 20)
        Me.tbRiCurve.TabIndex = 64
        '
        'btnRiMapOpen
        '
        Me.btnRiMapOpen.Location = New System.Drawing.Point(370, 17)
        Me.btnRiMapOpen.Name = "btnRiMapOpen"
        Me.btnRiMapOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnRiMapOpen.TabIndex = 66
        Me.btnRiMapOpen.UseVisualStyleBackColor = True
        '
        'btnBrowseRiMap
        '
        Me.btnBrowseRiMap.Location = New System.Drawing.Point(345, 17)
        Me.btnBrowseRiMap.Name = "btnBrowseRiMap"
        Me.btnBrowseRiMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseRiMap.TabIndex = 65
        Me.btnBrowseRiMap.UseVisualStyleBackColor = True
        '
        'Button1
        '
        Me.Button1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button1.Location = New System.Drawing.Point(620, 303)
        Me.Button1.Name = "Button1"
        Me.Button1.Size = New System.Drawing.Size(75, 23)
        Me.Button1.TabIndex = 53
        Me.Button1.Text = "Save"
        Me.Button1.UseVisualStyleBackColor = True
        '
        'Button2
        '
        Me.Button2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Button2.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Button2.Location = New System.Drawing.Point(701, 303)
        Me.Button2.Name = "Button2"
        Me.Button2.Size = New System.Drawing.Size(75, 23)
        Me.Button2.TabIndex = 54
        Me.Button2.Text = "Cancel"
        Me.Button2.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 428)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(800, 22)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 66
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(39, 17)
        Me.LbStatus.Text = "Status"
        '
        'FuelCellComponentForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 450)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.gpMassFlowMap)
        Me.Controls.Add(Me.Button2)
        Me.Controls.Add(Me.pnManufacturer)
        Me.Controls.Add(Me.Button1)
        Me.Controls.Add(Me.pnModel)
        Me.Controls.Add(Me.pnMaxElectricPower)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pcBoxMassFlowMap)
        Me.Name = "FuelCellComponentForm"
        Me.Text = "pnMinElectricPower"
        CType(Me.pcBoxMassFlowMap, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel2.ResumeLayout(False)
        Me.TableLayoutPanel2.PerformLayout()
        Me.pnMaxElectricPower.ResumeLayout(False)
        Me.pnMaxElectricPower.PerformLayout()
        Me.pnModel.ResumeLayout(False)
        Me.pnModel.PerformLayout()
        Me.pnManufacturer.ResumeLayout(False)
        Me.pnManufacturer.PerformLayout()
        Me.gpMassFlowMap.ResumeLayout(False)
        Me.gpMassFlowMap.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents pcBoxMassFlowMap As PictureBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents TableLayoutPanel2 As TableLayoutPanel
    Friend WithEvents pnMaxElectricPower As TableLayoutPanel
    Friend WithEvents pnModel As TableLayoutPanel
    Friend WithEvents pnManufacturer As TableLayoutPanel
    Friend WithEvents lblModel As Label
    Friend WithEvents lblManufacturer As Label
    Friend WithEvents lblMaxElectricPower As Label
    Friend WithEvents tbMinElectricPower As TextBox
    Friend WithEvents tbMaxElectricPower As TextBox
    Friend WithEvents lblMinElectricPower As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents tbModel As TextBox
    Friend WithEvents tbManufacturer As TextBox
    Friend WithEvents gpMassFlowMap As GroupBox
    Friend WithEvents tbRiCurve As TextBox
    Friend WithEvents btnRiMapOpen As Button
    Friend WithEvents btnBrowseRiMap As Button
    Friend WithEvents Button1 As Button
    Friend WithEvents Button2 As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents LbStatus As ToolStripStatusLabel
End Class
