<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class FuelCellComponentDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
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
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FuelCellComponentDialog))
        Me.pnSelectFuelCellComponent = New System.Windows.Forms.Panel()
        Me.btnOpenFuelCellComponent = New System.Windows.Forms.Button()
        Me.btnBrowseFuelCellComponent = New System.Windows.Forms.Button()
        Me.tbFuelCellComponent = New System.Windows.Forms.TextBox()
        Me.lblFuelCellCount = New System.Windows.Forms.Label()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.numFuelCellCount = New System.Windows.Forms.NumericUpDown()
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.pnSelectFuelCellComponent.SuspendLayout()
        Me.Panel1.SuspendLayout()
        CType(Me.numFuelCellCount, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TableLayoutPanel1.SuspendLayout()
        Me.SuspendLayout()
        '
        'pnSelectFuelCellComponent
        '
        Me.pnSelectFuelCellComponent.Controls.Add(Me.btnOpenFuelCellComponent)
        Me.pnSelectFuelCellComponent.Controls.Add(Me.btnBrowseFuelCellComponent)
        Me.pnSelectFuelCellComponent.Controls.Add(Me.tbFuelCellComponent)
        Me.pnSelectFuelCellComponent.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnSelectFuelCellComponent.Location = New System.Drawing.Point(8, 8)
        Me.pnSelectFuelCellComponent.Margin = New System.Windows.Forms.Padding(8)
        Me.pnSelectFuelCellComponent.Name = "pnSelectFuelCellComponent"
        Me.pnSelectFuelCellComponent.Size = New System.Drawing.Size(563, 27)
        Me.pnSelectFuelCellComponent.TabIndex = 1
        '
        'btnOpenFuelCellComponent
        '
        Me.btnOpenFuelCellComponent.Location = New System.Drawing.Point(4, 2)
        Me.btnOpenFuelCellComponent.Name = "btnOpenFuelCellComponent"
        Me.btnOpenFuelCellComponent.Size = New System.Drawing.Size(116, 21)
        Me.btnOpenFuelCellComponent.TabIndex = 0
        Me.btnOpenFuelCellComponent.TabStop = False
        Me.btnOpenFuelCellComponent.Text = "Fuel Cell Component"
        Me.btnOpenFuelCellComponent.UseVisualStyleBackColor = True
        '
        'btnBrowseFuelCellComponent
        '
        Me.btnBrowseFuelCellComponent.Image = CType(resources.GetObject("btnBrowseFuelCellComponent.Image"), System.Drawing.Image)
        Me.btnBrowseFuelCellComponent.Location = New System.Drawing.Point(538, 1)
        Me.btnBrowseFuelCellComponent.Name = "btnBrowseFuelCellComponent"
        Me.btnBrowseFuelCellComponent.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseFuelCellComponent.TabIndex = 1
        Me.btnBrowseFuelCellComponent.UseVisualStyleBackColor = True
        '
        'tbFuelCellComponent
        '
        Me.tbFuelCellComponent.Location = New System.Drawing.Point(126, 4)
        Me.tbFuelCellComponent.Name = "tbFuelCellComponent"
        Me.tbFuelCellComponent.Size = New System.Drawing.Size(407, 20)
        Me.tbFuelCellComponent.TabIndex = 0
        '
        'lblFuelCellCount
        '
        Me.lblFuelCellCount.AutoSize = True
        Me.lblFuelCellCount.Location = New System.Drawing.Point(3, 6)
        Me.lblFuelCellCount.Name = "lblFuelCellCount"
        Me.lblFuelCellCount.Size = New System.Drawing.Size(104, 13)
        Me.lblFuelCellCount.TabIndex = 27
        Me.lblFuelCellCount.Text = "Number of Fuel Cells"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.numFuelCellCount)
        Me.Panel1.Controls.Add(Me.lblFuelCellCount)
        Me.Panel1.Location = New System.Drawing.Point(12, 46)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(200, 26)
        Me.Panel1.TabIndex = 28
        '
        'numFuelCellCount
        '
        Me.numFuelCellCount.Location = New System.Drawing.Point(133, 4)
        Me.numFuelCellCount.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numFuelCellCount.Name = "numFuelCellCount"
        Me.numFuelCellCount.Size = New System.Drawing.Size(64, 20)
        Me.numFuelCellCount.TabIndex = 28
        Me.numFuelCellCount.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(424, 120)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(146, 29)
        Me.TableLayoutPanel1.TabIndex = 29
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(3, 3)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(67, 23)
        Me.OK_Button.TabIndex = 3
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(76, 3)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(67, 23)
        Me.Cancel_Button.TabIndex = 4
        Me.Cancel_Button.Text = "Cancel"
        '
        'FuelCellComponentDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(579, 160)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.pnSelectFuelCellComponent)
        Me.Margin = New System.Windows.Forms.Padding(2)
        Me.Name = "FuelCellComponentDialog"
        Me.Padding = New System.Windows.Forms.Padding(8)
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "FuelCellComponentDialog"
        Me.pnSelectFuelCellComponent.ResumeLayout(False)
        Me.pnSelectFuelCellComponent.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        CType(Me.numFuelCellCount, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TableLayoutPanel1.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub

    Friend WithEvents pnSelectFuelCellComponent As Panel
    Friend WithEvents btnOpenFuelCellComponent As Button
    Friend WithEvents btnBrowseFuelCellComponent As Button
    Friend WithEvents tbFuelCellComponent As TextBox
    Friend WithEvents lblFuelCellCount As Label
    Friend WithEvents Panel1 As Panel
    Friend WithEvents numFuelCellCount As NumericUpDown
    Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
    Friend WithEvents OK_Button As Button
    Friend WithEvents Cancel_Button As Button
End Class
