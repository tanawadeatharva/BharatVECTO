Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class F_Graph
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
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(F_Graph))
		Me.PictureBox1 = New PictureBox()
		Me.GroupBox1 = New GroupBox()
		Me.BtRemCh = New Button()
		Me.BtAddCh = New Button()
		Me.ListView1 = New ListView()
		Me.ColumnHeader1 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader3 = CType(New ColumnHeader(), ColumnHeader)
		Me.CbXaxis = New ComboBox()
		Me.ToolStrip1 = New ToolStrip()
		Me.ToolStripBtOpen = New ToolStripButton()
		Me.ToolStripButton3 = New ToolStripButton()
		Me.ToolStripButton2 = New ToolStripButton()
		Me.ToolStripSeparator1 = New ToolStripSeparator()
		Me.ToolStripButton1 = New ToolStripButton()
		Me.Label1 = New Label()
		Me.TbXmin = New TextBox()
		Me.TbXmax = New TextBox()
		Me.BtReset = New Button()
		Me.Label2 = New Label()
		Me.Label3 = New Label()
		Me.BtZoomIn = New Button()
		Me.BtZoomOut = New Button()
		Me.BtMoveL = New Button()
		Me.BtMoveR = New Button()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		Me.GroupBox1.SuspendLayout()
		Me.ToolStrip1.SuspendLayout()
		Me.SuspendLayout()
		'
		'PictureBox1
		'
		Me.PictureBox1.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.PictureBox1.BackColor = Color.LightGray
		Me.PictureBox1.Location = New Point(262, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(984, 332)
		Me.PictureBox1.TabIndex = 0
		Me.PictureBox1.TabStop = False
		'
		'GroupBox1
		'
		Me.GroupBox1.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left), AnchorStyles)
		Me.GroupBox1.Controls.Add(Me.BtRemCh)
		Me.GroupBox1.Controls.Add(Me.BtAddCh)
		Me.GroupBox1.Controls.Add(Me.ListView1)
		Me.GroupBox1.Location = New Point(12, 28)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(244, 362)
		Me.GroupBox1.TabIndex = 0
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Channels"
		'
		'BtRemCh
		'
		Me.BtRemCh.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtRemCh.Image = My.Resources.Resources.minus_circle_icon
		Me.BtRemCh.Location = New Point(29, 327)
		Me.BtRemCh.Name = "BtRemCh"
		Me.BtRemCh.Size = New Size(24, 24)
		Me.BtRemCh.TabIndex = 2
		Me.BtRemCh.UseVisualStyleBackColor = True
		'
		'BtAddCh
		'
		Me.BtAddCh.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtAddCh.Image = My.Resources.Resources.plus_circle_icon
		Me.BtAddCh.Location = New Point(5, 327)
		Me.BtAddCh.Name = "BtAddCh"
		Me.BtAddCh.Size = New Size(24, 24)
		Me.BtAddCh.TabIndex = 1
		Me.BtAddCh.UseVisualStyleBackColor = True
		'
		'ListView1
		'
		Me.ListView1.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.ListView1.BackColor = Color.GhostWhite
		Me.ListView1.CheckBoxes = True
		Me.ListView1.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader3})
		Me.ListView1.FullRowSelect = True
		Me.ListView1.GridLines = True
		Me.ListView1.Location = New Point(6, 19)
		Me.ListView1.Name = "ListView1"
		Me.ListView1.Size = New Size(232, 308)
		Me.ListView1.TabIndex = 0
		Me.ListView1.UseCompatibleStateImageBehavior = False
		Me.ListView1.View = View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Channel"
		Me.ColumnHeader1.Width = 184
		'
		'ColumnHeader3
		'
		Me.ColumnHeader3.Text = "Y Axis"
		Me.ColumnHeader3.Width = 42
		'
		'CbXaxis
		'
		Me.CbXaxis.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.CbXaxis.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbXaxis.FormattingEnabled = True
		Me.CbXaxis.Items.AddRange(New Object() {"Distance", "Time"})
		Me.CbXaxis.Location = New Point(325, 369)
		Me.CbXaxis.Name = "CbXaxis"
		Me.CbXaxis.Size = New Size(90, 21)
		Me.CbXaxis.TabIndex = 1
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripBtOpen, Me.ToolStripButton3, Me.ToolStripButton2, Me.ToolStripSeparator1, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New Size(1258, 25)
		Me.ToolStrip1.TabIndex = 31
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New Size(23, 22)
		Me.ToolStripBtOpen.Text = "ToolStripButton1"
		Me.ToolStripBtOpen.ToolTipText = "Open .vmod file"
		'
		'ToolStripButton3
		'
		Me.ToolStripButton3.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripButton3.Image = My.Resources.Resources.application_add_icon
		Me.ToolStripButton3.ImageTransparentColor = Color.Magenta
		Me.ToolStripButton3.Name = "ToolStripButton3"
		Me.ToolStripButton3.Size = New Size(23, 22)
		Me.ToolStripButton3.Text = "ToolStripButton3"
		Me.ToolStripButton3.ToolTipText = "New window"
		'
		'ToolStripButton2
		'
		Me.ToolStripButton2.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripButton2.Image = My.Resources.Resources.Refresh_icon
		Me.ToolStripButton2.ImageTransparentColor = Color.Magenta
		Me.ToolStripButton2.Name = "ToolStripButton2"
		Me.ToolStripButton2.Size = New Size(23, 22)
		Me.ToolStripButton2.Text = "ToolStripButton2"
		Me.ToolStripButton2.ToolTipText = "Reload file"
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New Size(6, 25)
		'
		'ToolStripButton1
		'
		Me.ToolStripButton1.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripButton1.Image = My.Resources.Resources.Help_icon
		Me.ToolStripButton1.ImageTransparentColor = Color.Magenta
		Me.ToolStripButton1.Name = "ToolStripButton1"
		Me.ToolStripButton1.Size = New Size(23, 22)
		Me.ToolStripButton1.Text = "Help"
		'
		'Label1
		'
		Me.Label1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(283, 372)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(36, 13)
		Me.Label1.TabIndex = 32
		Me.Label1.Text = "X Axis"
		'
		'TbXmin
		'
		Me.TbXmin.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.TbXmin.Location = New Point(497, 369)
		Me.TbXmin.Name = "TbXmin"
		Me.TbXmin.Size = New Size(71, 20)
		Me.TbXmin.TabIndex = 2
		'
		'TbXmax
		'
		Me.TbXmax.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.TbXmax.Location = New Point(609, 369)
		Me.TbXmax.Name = "TbXmax"
		Me.TbXmax.Size = New Size(71, 20)
		Me.TbXmax.TabIndex = 3
		'
		'BtReset
		'
		Me.BtReset.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtReset.Location = New Point(691, 367)
		Me.BtReset.Name = "BtReset"
		Me.BtReset.Size = New Size(75, 23)
		Me.BtReset.TabIndex = 4
		Me.BtReset.Text = "Reset"
		Me.BtReset.UseVisualStyleBackColor = True
		'
		'Label2
		'
		Me.Label2.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(467, 372)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(24, 13)
		Me.Label2.TabIndex = 35
		Me.Label2.Text = "Min"
		'
		'Label3
		'
		Me.Label3.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(576, 372)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(27, 13)
		Me.Label3.TabIndex = 35
		Me.Label3.Text = "Max"
		'
		'BtZoomIn
		'
		Me.BtZoomIn.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.BtZoomIn.Location = New Point(1103, 366)
		Me.BtZoomIn.Name = "BtZoomIn"
		Me.BtZoomIn.Size = New Size(24, 24)
		Me.BtZoomIn.TabIndex = 36
		Me.BtZoomIn.Text = "+"
		Me.BtZoomIn.UseVisualStyleBackColor = True
		'
		'BtZoomOut
		'
		Me.BtZoomOut.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.BtZoomOut.Location = New Point(1126, 366)
		Me.BtZoomOut.Name = "BtZoomOut"
		Me.BtZoomOut.Size = New Size(24, 24)
		Me.BtZoomOut.TabIndex = 36
		Me.BtZoomOut.Text = "-"
		Me.BtZoomOut.UseVisualStyleBackColor = True
		'
		'BtMoveL
		'
		Me.BtMoveL.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.BtMoveL.Location = New Point(1199, 366)
		Me.BtMoveL.Name = "BtMoveL"
		Me.BtMoveL.Size = New Size(24, 24)
		Me.BtMoveL.TabIndex = 36
		Me.BtMoveL.Text = "<"
		Me.BtMoveL.UseVisualStyleBackColor = True
		'
		'BtMoveR
		'
		Me.BtMoveR.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.BtMoveR.Location = New Point(1222, 366)
		Me.BtMoveR.Name = "BtMoveR"
		Me.BtMoveR.Size = New Size(24, 24)
		Me.BtMoveR.TabIndex = 36
		Me.BtMoveR.Text = ">"
		Me.BtMoveR.UseVisualStyleBackColor = True
		'
		'F_Graph
		'
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.ClientSize = New Size(1258, 400)
		Me.Controls.Add(Me.BtZoomOut)
		Me.Controls.Add(Me.BtMoveR)
		Me.Controls.Add(Me.BtMoveL)
		Me.Controls.Add(Me.BtZoomIn)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.BtReset)
		Me.Controls.Add(Me.TbXmax)
		Me.Controls.Add(Me.TbXmin)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.CbXaxis)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.PictureBox1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Me.MinimumSize = New Size(1000, 300)
		Me.Name = "F_Graph"
		Me.SizeGripStyle = SizeGripStyle.Show
		Me.Text = "Graph"
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		Me.GroupBox1.ResumeLayout(False)
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents ListView1 As ListView
	Friend WithEvents BtRemCh As Button
	Friend WithEvents BtAddCh As Button
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents ToolStripButton2 As ToolStripButton
	Friend WithEvents CbXaxis As ComboBox
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents Label1 As Label
	Friend WithEvents TbXmin As TextBox
	Friend WithEvents TbXmax As TextBox
	Friend WithEvents BtReset As Button
	Friend WithEvents Label2 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents ToolStripButton3 As ToolStripButton
	Friend WithEvents BtZoomIn As Button
	Friend WithEvents BtZoomOut As Button
	Friend WithEvents BtMoveL As Button
	Friend WithEvents BtMoveR As Button
End Class
