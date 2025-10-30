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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FuelCellComponentForm))
        Me.pcBoxMassFlowMap = New System.Windows.Forms.PictureBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.pnMinElectricPower = New System.Windows.Forms.TableLayoutPanel()
        Me.lblMinElectricPower = New System.Windows.Forms.Label()
        Me.tbMinElectricPower = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.pnMaxElectricPower = New System.Windows.Forms.TableLayoutPanel()
        Me.tbMaxElectricPower = New System.Windows.Forms.TextBox()
        Me.lblMaxElectricPower = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.pnModel = New System.Windows.Forms.TableLayoutPanel()
        Me.tbModel = New System.Windows.Forms.TextBox()
        Me.lblModel = New System.Windows.Forms.Label()
        Me.pnManufacturer = New System.Windows.Forms.TableLayoutPanel()
        Me.tbManufacturer = New System.Windows.Forms.TextBox()
        Me.lblManufacturer = New System.Windows.Forms.Label()
        Me.gpMassFlowMap = New System.Windows.Forms.GroupBox()
        Me.btnBrowseMassFlowMap = New System.Windows.Forms.Button()
        Me.tbMassFlowMap = New System.Windows.Forms.TextBox()
        Me.btnSaveFuelCellComponent = New System.Windows.Forms.Button()
        Me.btnCancel = New System.Windows.Forms.Button()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        CType(Me.pcBoxMassFlowMap,System.ComponentModel.ISupportInitialize).BeginInit
        Me.pnMinElectricPower.SuspendLayout
        Me.pnMaxElectricPower.SuspendLayout
        Me.pnModel.SuspendLayout
        Me.pnManufacturer.SuspendLayout
        Me.gpMassFlowMap.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        Me.SuspendLayout
        '
        'pcBoxMassFlowMap
        '
        Me.pcBoxMassFlowMap.BackColor = System.Drawing.Color.LightGray
        Me.pcBoxMassFlowMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.pcBoxMassFlowMap.Location = New System.Drawing.Point(403, 22)
        Me.pcBoxMassFlowMap.Name = "pcBoxMassFlowMap"
        Me.pcBoxMassFlowMap.Size = New System.Drawing.Size(382, 266)
        Me.pcBoxMassFlowMap.TabIndex = 50
        Me.pcBoxMassFlowMap.TabStop = false
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!)
        Me.lblTitle.Location = New System.Drawing.Point(12, 22)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(388, 29)
        Me.lblTitle.TabIndex = 0
        Me.lblTitle.Text = "Fuel Cell System"
        Me.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'pnMinElectricPower
        '
        Me.pnMinElectricPower.ColumnCount = 3
        Me.pnMinElectricPower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454!))
        Me.pnMinElectricPower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45.45454!))
        Me.pnMinElectricPower.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 9.090909!))
        Me.pnMinElectricPower.Controls.Add(Me.lblMinElectricPower, 0, 0)
        Me.pnMinElectricPower.Controls.Add(Me.tbMinElectricPower, 1, 0)
        Me.pnMinElectricPower.Controls.Add(Me.Label3, 2, 0)
        Me.pnMinElectricPower.Location = New System.Drawing.Point(12, 115)
        Me.pnMinElectricPower.Name = "pnMinElectricPower"
        Me.pnMinElectricPower.RowCount = 1
        Me.pnMinElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.pnMinElectricPower.Size = New System.Drawing.Size(388, 25)
        Me.pnMinElectricPower.TabIndex = 4
        '
        'lblMinElectricPower
        '
        Me.lblMinElectricPower.AutoSize = true
        Me.lblMinElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMinElectricPower.Location = New System.Drawing.Point(0, 0)
        Me.lblMinElectricPower.Margin = New System.Windows.Forms.Padding(0)
        Me.lblMinElectricPower.Name = "lblMinElectricPower"
        Me.lblMinElectricPower.Size = New System.Drawing.Size(176, 25)
        Me.lblMinElectricPower.TabIndex = 0
        Me.lblMinElectricPower.Text = "Min Electric Power"
        Me.lblMinElectricPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbMinElectricPower
        '
        Me.tbMinElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbMinElectricPower.Location = New System.Drawing.Point(179, 3)
        Me.tbMinElectricPower.Name = "tbMinElectricPower"
        Me.tbMinElectricPower.Size = New System.Drawing.Size(170, 20)
        Me.tbMinElectricPower.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(352, 0)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(36, 25)
        Me.Label3.TabIndex = 2
        Me.Label3.Text = "[kW]"
        Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
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
        Me.pnMaxElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.pnMaxElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25!))
        Me.pnMaxElectricPower.Size = New System.Drawing.Size(388, 25)
        Me.pnMaxElectricPower.TabIndex = 5
        '
        'tbMaxElectricPower
        '
        Me.tbMaxElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbMaxElectricPower.Location = New System.Drawing.Point(179, 3)
        Me.tbMaxElectricPower.Name = "tbMaxElectricPower"
        Me.tbMaxElectricPower.Size = New System.Drawing.Size(170, 20)
        Me.tbMaxElectricPower.TabIndex = 1
        '
        'lblMaxElectricPower
        '
        Me.lblMaxElectricPower.AutoSize = true
        Me.lblMaxElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMaxElectricPower.Location = New System.Drawing.Point(0, 0)
        Me.lblMaxElectricPower.Margin = New System.Windows.Forms.Padding(0)
        Me.lblMaxElectricPower.Name = "lblMaxElectricPower"
        Me.lblMaxElectricPower.Size = New System.Drawing.Size(176, 25)
        Me.lblMaxElectricPower.TabIndex = 0
        Me.lblMaxElectricPower.Text = "Max Electric Power"
        Me.lblMaxElectricPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = true
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(352, 0)
        Me.Label7.Margin = New System.Windows.Forms.Padding(0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(36, 25)
        Me.Label7.TabIndex = 2
        Me.Label7.Text = "[kW]"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnModel
        '
        Me.pnModel.ColumnCount = 2
        Me.pnModel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.pnModel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.pnModel.Controls.Add(Me.tbModel, 0, 0)
        Me.pnModel.Controls.Add(Me.lblModel, 0, 0)
        Me.pnModel.Location = New System.Drawing.Point(12, 84)
        Me.pnModel.Name = "pnModel"
        Me.pnModel.RowCount = 1
        Me.pnModel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.pnModel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25!))
        Me.pnModel.Size = New System.Drawing.Size(388, 25)
        Me.pnModel.TabIndex = 3
        '
        'tbModel
        '
        Me.tbModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbModel.Location = New System.Drawing.Point(197, 3)
        Me.tbModel.Name = "tbModel"
        Me.tbModel.Size = New System.Drawing.Size(188, 20)
        Me.tbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.AutoSize = true
        Me.lblModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblModel.Location = New System.Drawing.Point(0, 0)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(194, 25)
        Me.lblModel.TabIndex = 0
        Me.lblModel.Text = "Model"
        Me.lblModel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnManufacturer
        '
        Me.pnManufacturer.ColumnCount = 2
        Me.pnManufacturer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.pnManufacturer.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.pnManufacturer.Controls.Add(Me.tbManufacturer, 0, 0)
        Me.pnManufacturer.Controls.Add(Me.lblManufacturer, 0, 0)
        Me.pnManufacturer.Location = New System.Drawing.Point(12, 54)
        Me.pnManufacturer.Name = "pnManufacturer"
        Me.pnManufacturer.RowCount = 1
        Me.pnManufacturer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100!))
        Me.pnManufacturer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25!))
        Me.pnManufacturer.Size = New System.Drawing.Size(388, 25)
        Me.pnManufacturer.TabIndex = 2
        '
        'tbManufacturer
        '
        Me.tbManufacturer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbManufacturer.Location = New System.Drawing.Point(197, 3)
        Me.tbManufacturer.Name = "tbManufacturer"
        Me.tbManufacturer.Size = New System.Drawing.Size(188, 20)
        Me.tbManufacturer.TabIndex = 1
        '
        'lblManufacturer
        '
        Me.lblManufacturer.AutoSize = true
        Me.lblManufacturer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblManufacturer.Location = New System.Drawing.Point(0, 0)
        Me.lblManufacturer.Margin = New System.Windows.Forms.Padding(0)
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Size = New System.Drawing.Size(194, 25)
        Me.lblManufacturer.TabIndex = 0
        Me.lblManufacturer.Text = "Manufacturer"
        Me.lblManufacturer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gpMassFlowMap
        '
        Me.gpMassFlowMap.Controls.Add(Me.btnBrowseMassFlowMap)
        Me.gpMassFlowMap.Controls.Add(Me.tbMassFlowMap)
        Me.gpMassFlowMap.Location = New System.Drawing.Point(6, 188)
        Me.gpMassFlowMap.Name = "gpMassFlowMap"
        Me.gpMassFlowMap.Size = New System.Drawing.Size(394, 49)
        Me.gpMassFlowMap.TabIndex = 6
        Me.gpMassFlowMap.TabStop = false
        Me.gpMassFlowMap.Text = "MassFlowMap"
        '
        'btnBrowseMassFlowMap
        '
        Me.btnBrowseMassFlowMap.Image = CType(resources.GetObject("btnBrowseMassFlowMap.Image"),System.Drawing.Image)
        Me.btnBrowseMassFlowMap.Location = New System.Drawing.Point(364, 16)
        Me.btnBrowseMassFlowMap.Name = "btnBrowseMassFlowMap"
        Me.btnBrowseMassFlowMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseMassFlowMap.TabIndex = 1
        Me.btnBrowseMassFlowMap.UseVisualStyleBackColor = true
        '
        'tbMassFlowMap
        '
        Me.tbMassFlowMap.Location = New System.Drawing.Point(6, 19)
        Me.tbMassFlowMap.Name = "tbMassFlowMap"
        Me.tbMassFlowMap.Size = New System.Drawing.Size(352, 20)
        Me.tbMassFlowMap.TabIndex = 0
        '
        'btnSaveFuelCellComponent
        '
        Me.btnSaveFuelCellComponent.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnSaveFuelCellComponent.Location = New System.Drawing.Point(625, 299)
        Me.btnSaveFuelCellComponent.Name = "btnSaveFuelCellComponent"
        Me.btnSaveFuelCellComponent.Size = New System.Drawing.Size(75, 23)
        Me.btnSaveFuelCellComponent.TabIndex = 7
        Me.btnSaveFuelCellComponent.Text = "Save"
        Me.btnSaveFuelCellComponent.UseVisualStyleBackColor = true
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(710, 299)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = true
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 324)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(800, 22)
        Me.StatusStrip1.SizingGrip = false
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
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(800, 346)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pnManufacturer)
        Me.Controls.Add(Me.pnModel)
        Me.Controls.Add(Me.pnMinElectricPower)
        Me.Controls.Add(Me.pnMaxElectricPower)
        Me.Controls.Add(Me.gpMassFlowMap)
        Me.Controls.Add(Me.pcBoxMassFlowMap)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.btnSaveFuelCellComponent)
        Me.Controls.Add(Me.btnCancel)
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MinimizeBox = False
        Me.Name = "FuelCellComponentForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Fuel Cell System"
        CType(Me.pcBoxMassFlowMap,System.ComponentModel.ISupportInitialize).EndInit
        Me.pnMinElectricPower.ResumeLayout(false)
        Me.pnMinElectricPower.PerformLayout
        Me.pnMaxElectricPower.ResumeLayout(false)
        Me.pnMaxElectricPower.PerformLayout
        Me.pnModel.ResumeLayout(false)
        Me.pnModel.PerformLayout
        Me.pnManufacturer.ResumeLayout(false)
        Me.pnManufacturer.PerformLayout
        Me.gpMassFlowMap.ResumeLayout(false)
        Me.gpMassFlowMap.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents pcBoxMassFlowMap As PictureBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents pnMinElectricPower As TableLayoutPanel
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
    Friend WithEvents tbMassFlowMap As TextBox
    Friend WithEvents btnSaveFuelCellComponent As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents LbStatus As ToolStripStatusLabel
    Friend WithEvents btnBrowseMassFlowMap As Button
End Class
