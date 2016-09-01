Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices



<DesignerGenerated()> _
Partial Class GearboxGearDialog
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
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(GearboxGearDialog))
		Me.TableLayoutPanel1 = New TableLayoutPanel()
		Me.OK_Button = New Button()
		Me.Cancel_Button = New Button()
		Me.TbGear = New TextBox()
		Me.Label1 = New Label()
		Me.TbRatio = New TextBox()
		Me.Label2 = New Label()
		Me.TbMapPath = New TextBox()
		Me.Label3 = New Label()
		Me.BtBrowse = New Button()
		Me.BtNext = New Button()
		Me.TbShiftPolyFile = New TextBox()
		Me.BtShiftPolyBrowse = New Button()
		Me.Label4 = New Label()
		Me.PnShiftPoly = New Panel()
		Me.Label5 = New Label()
		Me.TbMaxTorque = New TextBox()
		Me.PnFld = New Panel()
		Me.BtPrevious = New Button()
		Me.Label6 = New Label()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.PnShiftPoly.SuspendLayout()
		Me.PnFld.SuspendLayout()
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
		Me.TableLayoutPanel1.Location = New Point(370, 184)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 1
		Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New Size(146, 29)
		Me.TableLayoutPanel1.TabIndex = 12
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
		'TbGear
		'
		Me.TbGear.Location = New Point(48, 6)
		Me.TbGear.Name = "TbGear"
		Me.TbGear.ReadOnly = True
		Me.TbGear.Size = New Size(37, 20)
		Me.TbGear.TabIndex = 1
		Me.TbGear.TabStop = False
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(12, 9)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(30, 13)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Gear"
		'
		'TbRatio
		'
		Me.TbRatio.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.TbRatio.Location = New Point(139, 6)
		Me.TbRatio.Name = "TbRatio"
		Me.TbRatio.Size = New Size(66, 20)
		Me.TbRatio.TabIndex = 3
		'
		'Label2
		'
		Me.Label2.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(101, 9)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(32, 13)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Ratio"
		'
		'TbMapPath
		'
		Me.TbMapPath.Location = New Point(12, 51)
		Me.TbMapPath.Name = "TbMapPath"
		Me.TbMapPath.Size = New Size(476, 20)
		Me.TbMapPath.TabIndex = 6
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(11, 37)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(187, 13)
		Me.Label3.TabIndex = 5
		Me.Label3.Text = "Loss Map file or Efficiency Value [0..1]"
		'
		'BtBrowse
		'
		Me.BtBrowse.Image = CType(resources.GetObject("BtBrowse.Image"), Image)
		Me.BtBrowse.Location = New Point(488, 49)
		Me.BtBrowse.Name = "BtBrowse"
		Me.BtBrowse.Size = New Size(24, 24)
		Me.BtBrowse.TabIndex = 7
		Me.BtBrowse.UseVisualStyleBackColor = True
		'
		'BtNext
		'
		Me.BtNext.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtNext.Location = New Point(196, 187)
		Me.BtNext.Name = "BtNext"
		Me.BtNext.Size = New Size(67, 23)
		Me.BtNext.TabIndex = 11
		Me.BtNext.Text = "&Next >"
		Me.BtNext.UseVisualStyleBackColor = True
		'
		'TbShiftPolyFile
		'
		Me.TbShiftPolyFile.Location = New Point(2, 17)
		Me.TbShiftPolyFile.Name = "TbShiftPolyFile"
		Me.TbShiftPolyFile.Size = New Size(476, 20)
		Me.TbShiftPolyFile.TabIndex = 1
		'
		'BtShiftPolyBrowse
		'
		Me.BtShiftPolyBrowse.Image = CType(resources.GetObject("BtShiftPolyBrowse.Image"), Image)
		Me.BtShiftPolyBrowse.Location = New Point(478, 15)
		Me.BtShiftPolyBrowse.Name = "BtShiftPolyBrowse"
		Me.BtShiftPolyBrowse.Size = New Size(24, 24)
		Me.BtShiftPolyBrowse.TabIndex = 2
		Me.BtShiftPolyBrowse.TabStop = False
		Me.BtShiftPolyBrowse.UseVisualStyleBackColor = True
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(1, 3)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(89, 13)
		Me.Label4.TabIndex = 0
		Me.Label4.Text = "Shift polygons file"
		'
		'PnShiftPoly
		'
		Me.PnShiftPoly.Controls.Add(Me.TbShiftPolyFile)
		Me.PnShiftPoly.Controls.Add(Me.BtShiftPolyBrowse)
		Me.PnShiftPoly.Controls.Add(Me.Label4)
		Me.PnShiftPoly.Location = New Point(10, 79)
		Me.PnShiftPoly.Name = "PnShiftPoly"
		Me.PnShiftPoly.Size = New Size(513, 41)
		Me.PnShiftPoly.TabIndex = 8
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(1, 5)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(123, 13)
		Me.Label5.TabIndex = 0
		Me.Label5.Text = "Maximum allowed torque"
		'
		'TbMaxTorque
		'
		Me.TbMaxTorque.Location = New Point(2, 19)
		Me.TbMaxTorque.Name = "TbMaxTorque"
		Me.TbMaxTorque.Size = New Size(88, 20)
		Me.TbMaxTorque.TabIndex = 1
		'
		'PnFld
		'
		Me.PnFld.Controls.Add(Me.Label6)
		Me.PnFld.Controls.Add(Me.TbMaxTorque)
		Me.PnFld.Controls.Add(Me.Label5)
		Me.PnFld.Location = New Point(10, 126)
		Me.PnFld.Name = "PnFld"
		Me.PnFld.Size = New Size(513, 46)
		Me.PnFld.TabIndex = 9
		'
		'BtPrevious
		'
		Me.BtPrevious.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtPrevious.Location = New Point(129, 187)
		Me.BtPrevious.Name = "BtPrevious"
		Me.BtPrevious.Size = New Size(67, 23)
		Me.BtPrevious.TabIndex = 10
		Me.BtPrevious.Text = "< &Previous"
		Me.BtPrevious.UseVisualStyleBackColor = True
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New Point(96, 22)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New Size(29, 13)
		Me.Label6.TabIndex = 2
		Me.Label6.Text = "[Nm]"
		'
		'GearboxGearDialog
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New Size(528, 225)
		Me.Controls.Add(Me.BtPrevious)
		Me.Controls.Add(Me.PnFld)
		Me.Controls.Add(Me.PnShiftPoly)
		Me.Controls.Add(Me.BtNext)
		Me.Controls.Add(Me.BtBrowse)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.TbMapPath)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.TbRatio)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.TbGear)
		Me.Controls.Add(Me.TableLayoutPanel1)
		Me.FormBorderStyle = FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "GearboxGearDialog"
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Edit Gears"
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.PnShiftPoly.ResumeLayout(False)
		Me.PnShiftPoly.PerformLayout()
		Me.PnFld.ResumeLayout(False)
		Me.PnFld.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents TbGear As TextBox
	Friend WithEvents Label1 As Label
	Friend WithEvents TbRatio As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents TbMapPath As TextBox
	Friend WithEvents Label3 As Label
	Friend WithEvents BtBrowse As Button
	Friend WithEvents BtNext As Button
	Friend WithEvents TbShiftPolyFile As TextBox
	Friend WithEvents BtShiftPolyBrowse As Button
	Friend WithEvents Label4 As Label
	Friend WithEvents PnShiftPoly As Panel
	Friend WithEvents Label5 As Label
	Friend WithEvents TbMaxTorque As TextBox
	Friend WithEvents PnFld As Panel
	Friend WithEvents BtPrevious As Button
	Friend WithEvents Label6 As Label

End Class
