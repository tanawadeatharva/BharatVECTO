<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class IEPCInputDialog
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(IEPCInputDialog))
		Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
		Me.btnOk = New System.Windows.Forms.Button()
		Me.btnCancel = New System.Windows.Forms.Button()
		Me.TableLayoutPanel3 = New System.Windows.Forms.TableLayoutPanel()
		Me.TableLayoutPanel5 = New System.Windows.Forms.TableLayoutPanel()
		Me.btAddFilePath = New System.Windows.Forms.Button()
		Me.tbInputFile = New System.Windows.Forms.TextBox()
		Me.TableLayoutPanel4 = New System.Windows.Forms.TableLayoutPanel()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.tbGear = New System.Windows.Forms.TextBox()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.TableLayoutPanel3.SuspendLayout()
		Me.TableLayoutPanel5.SuspendLayout()
		Me.TableLayoutPanel4.SuspendLayout()
		Me.SuspendLayout()
		'
		'TableLayoutPanel1
		'
		Me.TableLayoutPanel1.ColumnCount = 2
		Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Controls.Add(Me.btnOk, 0, 0)
		Me.TableLayoutPanel1.Controls.Add(Me.btnCancel, 1, 0)
		Me.TableLayoutPanel1.Location = New System.Drawing.Point(356, 57)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 1
		Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New System.Drawing.Size(148, 29)
		Me.TableLayoutPanel1.TabIndex = 4
		'
		'btnOk
		'
		Me.btnOk.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.btnOk.Location = New System.Drawing.Point(3, 3)
		Me.btnOk.Name = "btnOk"
		Me.btnOk.Size = New System.Drawing.Size(67, 23)
		Me.btnOk.TabIndex = 0
		Me.btnOk.Text = "OK"
		'
		'btnCancel
		'
		Me.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.btnCancel.Location = New System.Drawing.Point(77, 3)
		Me.btnCancel.Name = "btnCancel"
		Me.btnCancel.Size = New System.Drawing.Size(67, 23)
		Me.btnCancel.TabIndex = 1
		Me.btnCancel.Text = "Cancel"
		'
		'TableLayoutPanel3
		'
		Me.TableLayoutPanel3.ColumnCount = 2
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 28.3906!))
		Me.TableLayoutPanel3.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 71.60941!))
		Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel5, 1, 0)
		Me.TableLayoutPanel3.Controls.Add(Me.TableLayoutPanel4, 0, 0)
		Me.TableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Top
		Me.TableLayoutPanel3.Location = New System.Drawing.Point(0, 0)
		Me.TableLayoutPanel3.Name = "TableLayoutPanel3"
		Me.TableLayoutPanel3.RowCount = 1
		Me.TableLayoutPanel3.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel3.Size = New System.Drawing.Size(509, 49)
		Me.TableLayoutPanel3.TabIndex = 7
		'
		'TableLayoutPanel5
		'
		Me.TableLayoutPanel5.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.TableLayoutPanel5.ColumnCount = 2
		Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 316.0!))
		Me.TableLayoutPanel5.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40.0!))
		Me.TableLayoutPanel5.Controls.Add(Me.btAddFilePath, 1, 0)
		Me.TableLayoutPanel5.Controls.Add(Me.tbInputFile, 0, 0)
		Me.TableLayoutPanel5.Location = New System.Drawing.Point(148, 5)
		Me.TableLayoutPanel5.Name = "TableLayoutPanel5"
		Me.TableLayoutPanel5.RowCount = 1
		Me.TableLayoutPanel5.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100.0!))
		Me.TableLayoutPanel5.Size = New System.Drawing.Size(356, 39)
		Me.TableLayoutPanel5.TabIndex = 8
		'
		'btAddFilePath
		'
		Me.btAddFilePath.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.btAddFilePath.Image = CType(resources.GetObject("btAddFilePath.Image"), System.Drawing.Image)
		Me.btAddFilePath.Location = New System.Drawing.Point(324, 6)
		Me.btAddFilePath.Name = "btAddFilePath"
		Me.btAddFilePath.Size = New System.Drawing.Size(24, 26)
		Me.btAddFilePath.TabIndex = 30
		Me.btAddFilePath.UseVisualStyleBackColor = True
		'
		'tbInputFile
		'
		Me.tbInputFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Left Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbInputFile.Location = New System.Drawing.Point(3, 9)
		Me.tbInputFile.Name = "tbInputFile"
		Me.tbInputFile.Size = New System.Drawing.Size(310, 20)
		Me.tbInputFile.TabIndex = 0
		'
		'TableLayoutPanel4
		'
		Me.TableLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.TableLayoutPanel4.ColumnCount = 2
		Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 32.8!))
		Me.TableLayoutPanel4.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 67.2!))
		Me.TableLayoutPanel4.Controls.Add(Me.Label3, 0, 0)
		Me.TableLayoutPanel4.Controls.Add(Me.tbGear, 1, 0)
		Me.TableLayoutPanel4.Location = New System.Drawing.Point(9, 10)
		Me.TableLayoutPanel4.Name = "TableLayoutPanel4"
		Me.TableLayoutPanel4.RowCount = 1
		Me.TableLayoutPanel4.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50.0!))
		Me.TableLayoutPanel4.Size = New System.Drawing.Size(125, 28)
		Me.TableLayoutPanel4.TabIndex = 10
		'
		'Label3
		'
		Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(5, 7)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(30, 13)
		Me.Label3.TabIndex = 9
		Me.Label3.Text = "Gear"
		'
		'tbGear
		'
		Me.tbGear.Anchor = System.Windows.Forms.AnchorStyles.None
		Me.tbGear.Location = New System.Drawing.Point(44, 4)
		Me.tbGear.Name = "tbGear"
		Me.tbGear.Size = New System.Drawing.Size(77, 20)
		Me.tbGear.TabIndex = 8
		Me.tbGear.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
		'
		'IEPCInputDialog
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(509, 98)
		Me.Controls.Add(Me.TableLayoutPanel1)
		Me.Controls.Add(Me.TableLayoutPanel3)
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "IEPCInputDialog"
		Me.ShowInTaskbar = False
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
		Me.Text = "IEPCInputDialog"
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.TableLayoutPanel3.ResumeLayout(False)
		Me.TableLayoutPanel5.ResumeLayout(False)
		Me.TableLayoutPanel5.PerformLayout()
		Me.TableLayoutPanel4.ResumeLayout(False)
		Me.TableLayoutPanel4.PerformLayout()
		Me.ResumeLayout(False)

	End Sub

	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents btnOk As Button
	Friend WithEvents btnCancel As Button
	Friend WithEvents TableLayoutPanel3 As TableLayoutPanel
	Friend WithEvents TableLayoutPanel4 As TableLayoutPanel
	Friend WithEvents Label3 As Label
	Friend WithEvents tbGear As TextBox
	Friend WithEvents TableLayoutPanel5 As TableLayoutPanel
	Friend WithEvents tbInputFile As TextBox
	Friend WithEvents btAddFilePath As Button
End Class
