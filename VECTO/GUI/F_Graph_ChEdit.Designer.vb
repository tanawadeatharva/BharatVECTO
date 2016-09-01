Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices


<DesignerGenerated()> _
Partial Class F_Graph_ChEdit
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
		Me.ComboBox1 = New ComboBox()
		Me.GroupBox1 = New GroupBox()
		Me.RbRight = New RadioButton()
		Me.RbLeft = New RadioButton()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
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
		Me.TableLayoutPanel1.Location = New Point(58, 110)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 1
		Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New Size(146, 29)
		Me.TableLayoutPanel1.TabIndex = 0
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
		'ComboBox1
		'
		Me.ComboBox1.DropDownStyle = ComboBoxStyle.DropDownList
		Me.ComboBox1.FormattingEnabled = True
		Me.ComboBox1.Location = New Point(12, 12)
		Me.ComboBox1.Name = "ComboBox1"
		Me.ComboBox1.Size = New Size(188, 21)
		Me.ComboBox1.TabIndex = 1
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.RbRight)
		Me.GroupBox1.Controls.Add(Me.RbLeft)
		Me.GroupBox1.Location = New Point(12, 39)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(188, 55)
		Me.GroupBox1.TabIndex = 2
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Y Axis"
		'
		'RbRight
		'
		Me.RbRight.AutoSize = True
		Me.RbRight.Location = New Point(80, 19)
		Me.RbRight.Name = "RbRight"
		Me.RbRight.Size = New Size(50, 17)
		Me.RbRight.TabIndex = 3
		Me.RbRight.TabStop = True
		Me.RbRight.Text = "Right"
		Me.RbRight.UseVisualStyleBackColor = True
		'
		'RbLeft
		'
		Me.RbLeft.AutoSize = True
		Me.RbLeft.Location = New Point(16, 19)
		Me.RbLeft.Name = "RbLeft"
		Me.RbLeft.Size = New Size(43, 17)
		Me.RbLeft.TabIndex = 3
		Me.RbLeft.TabStop = True
		Me.RbLeft.Text = "Left"
		Me.RbLeft.UseVisualStyleBackColor = True
		'
		'F_Graph_ChEdit
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New Size(216, 151)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.ComboBox1)
		Me.Controls.Add(Me.TableLayoutPanel1)
		Me.FormBorderStyle = FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "F_Graph_ChEdit"
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Edit Channel"
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents ComboBox1 As ComboBox
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents RbRight As RadioButton
	Friend WithEvents RbLeft As RadioButton

End Class
