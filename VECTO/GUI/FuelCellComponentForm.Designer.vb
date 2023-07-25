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
        Me.TableLayoutPanel2 = New System.Windows.Forms.TableLayoutPanel()
        Me.tbMinElectricPower = New System.Windows.Forms.TextBox()
        Me.lblMinElectricPower = New System.Windows.Forms.Label()
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
        Me.pcBoxMassFlowMap.Location = New System.Drawing.Point(537, 27)
        Me.pcBoxMassFlowMap.Margin = New System.Windows.Forms.Padding(4)
        Me.pcBoxMassFlowMap.Name = "pcBoxMassFlowMap"
        Me.pcBoxMassFlowMap.Size = New System.Drawing.Size(509, 327)
        Me.pcBoxMassFlowMap.TabIndex = 50
        Me.pcBoxMassFlowMap.TabStop = False
        '
        'lblTitle
        '
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!)
        Me.lblTitle.Location = New System.Drawing.Point(16, 27)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(517, 36)
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
        Me.TableLayoutPanel2.Location = New System.Drawing.Point(16, 142)
        Me.TableLayoutPanel2.Margin = New System.Windows.Forms.Padding(4)
        Me.TableLayoutPanel2.Name = "TableLayoutPanel2"
        Me.TableLayoutPanel2.RowCount = 1
        Me.TableLayoutPanel2.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.TableLayoutPanel2.Size = New System.Drawing.Size(517, 31)
        Me.TableLayoutPanel2.TabIndex = 56
        '
        'tbMinElectricPower
        '
        Me.tbMinElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbMinElectricPower.Location = New System.Drawing.Point(239, 4)
        Me.tbMinElectricPower.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMinElectricPower.Name = "tbMinElectricPower"
        Me.tbMinElectricPower.Size = New System.Drawing.Size(227, 22)
        Me.tbMinElectricPower.TabIndex = 2
        '
        'lblMinElectricPower
        '
        Me.lblMinElectricPower.AutoSize = True
        Me.lblMinElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMinElectricPower.Location = New System.Drawing.Point(0, 0)
        Me.lblMinElectricPower.Margin = New System.Windows.Forms.Padding(0)
        Me.lblMinElectricPower.Name = "lblMinElectricPower"
        Me.lblMinElectricPower.Size = New System.Drawing.Size(235, 31)
        Me.lblMinElectricPower.TabIndex = 5
        Me.lblMinElectricPower.Text = "Min Electric Power"
        Me.lblMinElectricPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label3.Location = New System.Drawing.Point(470, 0)
        Me.Label3.Margin = New System.Windows.Forms.Padding(0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(47, 31)
        Me.Label3.TabIndex = 6
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
        Me.pnMaxElectricPower.Location = New System.Drawing.Point(16, 180)
        Me.pnMaxElectricPower.Margin = New System.Windows.Forms.Padding(4)
        Me.pnMaxElectricPower.Name = "pnMaxElectricPower"
        Me.pnMaxElectricPower.RowCount = 1
        Me.pnMaxElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnMaxElectricPower.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31.0!))
        Me.pnMaxElectricPower.Size = New System.Drawing.Size(517, 31)
        Me.pnMaxElectricPower.TabIndex = 58
        '
        'tbMaxElectricPower
        '
        Me.tbMaxElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbMaxElectricPower.Location = New System.Drawing.Point(239, 4)
        Me.tbMaxElectricPower.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMaxElectricPower.Name = "tbMaxElectricPower"
        Me.tbMaxElectricPower.Size = New System.Drawing.Size(227, 22)
        Me.tbMaxElectricPower.TabIndex = 3
        '
        'lblMaxElectricPower
        '
        Me.lblMaxElectricPower.AutoSize = True
        Me.lblMaxElectricPower.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblMaxElectricPower.Location = New System.Drawing.Point(0, 0)
        Me.lblMaxElectricPower.Margin = New System.Windows.Forms.Padding(0)
        Me.lblMaxElectricPower.Name = "lblMaxElectricPower"
        Me.lblMaxElectricPower.Size = New System.Drawing.Size(235, 31)
        Me.lblMaxElectricPower.TabIndex = 3
        Me.lblMaxElectricPower.Text = "Max Electric Power"
        Me.lblMaxElectricPower.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Label7.Location = New System.Drawing.Point(470, 0)
        Me.Label7.Margin = New System.Windows.Forms.Padding(0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(47, 31)
        Me.Label7.TabIndex = 6
        Me.Label7.Text = "[kW]"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'pnModel
        '
        Me.pnModel.ColumnCount = 2
        Me.pnModel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.pnModel.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.pnModel.Controls.Add(Me.tbModel, 0, 0)
        Me.pnModel.Controls.Add(Me.lblModel, 0, 0)
        Me.pnModel.Location = New System.Drawing.Point(16, 103)
        Me.pnModel.Margin = New System.Windows.Forms.Padding(4)
        Me.pnModel.Name = "pnModel"
        Me.pnModel.RowCount = 1
        Me.pnModel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnModel.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31.0!))
        Me.pnModel.Size = New System.Drawing.Size(517, 31)
        Me.pnModel.TabIndex = 59
        '
        'tbModel
        '
        Me.tbModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbModel.Location = New System.Drawing.Point(262, 4)
        Me.tbModel.Margin = New System.Windows.Forms.Padding(4)
        Me.tbModel.Name = "tbModel"
        Me.tbModel.Size = New System.Drawing.Size(251, 22)
        Me.tbModel.TabIndex = 1
        '
        'lblModel
        '
        Me.lblModel.AutoSize = True
        Me.lblModel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblModel.Location = New System.Drawing.Point(0, 0)
        Me.lblModel.Margin = New System.Windows.Forms.Padding(0)
        Me.lblModel.Name = "lblModel"
        Me.lblModel.Size = New System.Drawing.Size(258, 31)
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
        Me.pnManufacturer.Location = New System.Drawing.Point(16, 66)
        Me.pnManufacturer.Margin = New System.Windows.Forms.Padding(4)
        Me.pnManufacturer.Name = "pnManufacturer"
        Me.pnManufacturer.RowCount = 1
        Me.pnManufacturer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
        Me.pnManufacturer.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 31.0!))
        Me.pnManufacturer.Size = New System.Drawing.Size(517, 31)
        Me.pnManufacturer.TabIndex = 60
        '
        'tbManufacturer
        '
        Me.tbManufacturer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.tbManufacturer.Location = New System.Drawing.Point(262, 4)
        Me.tbManufacturer.Margin = New System.Windows.Forms.Padding(4)
        Me.tbManufacturer.Name = "tbManufacturer"
        Me.tbManufacturer.Size = New System.Drawing.Size(251, 22)
        Me.tbManufacturer.TabIndex = 0
        '
        'lblManufacturer
        '
        Me.lblManufacturer.AutoSize = True
        Me.lblManufacturer.Dock = System.Windows.Forms.DockStyle.Fill
        Me.lblManufacturer.Location = New System.Drawing.Point(0, 0)
        Me.lblManufacturer.Margin = New System.Windows.Forms.Padding(0)
        Me.lblManufacturer.Name = "lblManufacturer"
        Me.lblManufacturer.Size = New System.Drawing.Size(258, 31)
        Me.lblManufacturer.TabIndex = 0
        Me.lblManufacturer.Text = "Manufacturer"
        Me.lblManufacturer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'gpMassFlowMap
        '
        Me.gpMassFlowMap.Controls.Add(Me.btnBrowseMassFlowMap)
        Me.gpMassFlowMap.Controls.Add(Me.tbMassFlowMap)
        Me.gpMassFlowMap.Location = New System.Drawing.Point(8, 231)
        Me.gpMassFlowMap.Margin = New System.Windows.Forms.Padding(4)
        Me.gpMassFlowMap.Name = "gpMassFlowMap"
        Me.gpMassFlowMap.Padding = New System.Windows.Forms.Padding(4)
        Me.gpMassFlowMap.Size = New System.Drawing.Size(525, 60)
        Me.gpMassFlowMap.TabIndex = 65
        Me.gpMassFlowMap.TabStop = False
        Me.gpMassFlowMap.Text = "MassFlowMap"
        '
        'btnBrowseMassFlowMap
        '
        Me.btnBrowseMassFlowMap.Image = CType(resources.GetObject("btnBrowseMassFlowMap.Image"), System.Drawing.Image)
        Me.btnBrowseMassFlowMap.Location = New System.Drawing.Point(485, 20)
        Me.btnBrowseMassFlowMap.Margin = New System.Windows.Forms.Padding(4)
        Me.btnBrowseMassFlowMap.Name = "btnBrowseMassFlowMap"
        Me.btnBrowseMassFlowMap.Size = New System.Drawing.Size(32, 30)
        Me.btnBrowseMassFlowMap.TabIndex = 5
        Me.btnBrowseMassFlowMap.UseVisualStyleBackColor = True
        '
        'tbMassFlowMap
        '
        Me.tbMassFlowMap.Location = New System.Drawing.Point(8, 23)
        Me.tbMassFlowMap.Margin = New System.Windows.Forms.Padding(4)
        Me.tbMassFlowMap.Name = "tbMassFlowMap"
        Me.tbMassFlowMap.Size = New System.Drawing.Size(468, 22)
        Me.tbMassFlowMap.TabIndex = 4
        '
        'btnSaveFuelCellComponent
        '
        Me.btnSaveFuelCellComponent.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSaveFuelCellComponent.Location = New System.Drawing.Point(827, 373)
        Me.btnSaveFuelCellComponent.Margin = New System.Windows.Forms.Padding(4)
        Me.btnSaveFuelCellComponent.Name = "btnSaveFuelCellComponent"
        Me.btnSaveFuelCellComponent.Size = New System.Drawing.Size(100, 28)
        Me.btnSaveFuelCellComponent.TabIndex = 7
        Me.btnSaveFuelCellComponent.Text = "Save"
        Me.btnSaveFuelCellComponent.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(935, 373)
        Me.btnCancel.Margin = New System.Windows.Forms.Padding(4)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(100, 28)
        Me.btnCancel.TabIndex = 8
        Me.btnCancel.Text = "Cancel"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 528)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 19, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(1067, 26)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 66
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(49, 20)
        Me.LbStatus.Text = "Status"
        '
        'FuelCellComponentForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1067, 554)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.gpMassFlowMap)
        Me.Controls.Add(Me.btnCancel)
        Me.Controls.Add(Me.pnManufacturer)
        Me.Controls.Add(Me.btnSaveFuelCellComponent)
        Me.Controls.Add(Me.pnModel)
        Me.Controls.Add(Me.pnMaxElectricPower)
        Me.Controls.Add(Me.TableLayoutPanel2)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pcBoxMassFlowMap)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4)
        Me.MinimizeBox = False
        Me.Name = "FuelCellComponentForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "FuelCellComponent"
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
    Friend WithEvents tbMassFlowMap As TextBox
    Friend WithEvents btnSaveFuelCellComponent As Button
    Friend WithEvents btnCancel As Button
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents LbStatus As ToolStripStatusLabel
    Friend WithEvents btnBrowseMassFlowMap As Button
End Class
