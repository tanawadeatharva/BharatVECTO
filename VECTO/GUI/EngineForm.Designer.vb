' Copyright 2014 European Union.
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
Partial Class EngineForm
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
		Me.components = New Container()
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(EngineForm))
		Me.TbNleerl = New TextBox()
		Me.Label11 = New Label()
		Me.TbInertia = New TextBox()
		Me.Label41 = New Label()
		Me.Label40 = New Label()
		Me.Label5 = New Label()
		Me.ButCancel = New Button()
		Me.ButOK = New Button()
		Me.ToolStrip1 = New ToolStrip()
		Me.ToolStripBtNew = New ToolStripButton()
		Me.ToolStripBtOpen = New ToolStripButton()
		Me.ToolStripBtSave = New ToolStripButton()
		Me.ToolStripBtSaveAs = New ToolStripButton()
		Me.ToolStripSeparator3 = New ToolStripSeparator()
		Me.ToolStripBtSendTo = New ToolStripButton()
		Me.ToolStripSeparator1 = New ToolStripSeparator()
		Me.ToolStripButton1 = New ToolStripButton()
		Me.StatusStrip1 = New StatusStrip()
		Me.LbStatus = New ToolStripStatusLabel()
		Me.Label1 = New Label()
		Me.Label2 = New Label()
		Me.TbDispl = New TextBox()
		Me.TbName = New TextBox()
		Me.Label3 = New Label()
		Me.TbMAP = New TextBox()
		Me.Label6 = New Label()
		Me.BtMAP = New Button()
		Me.PictureBox1 = New PictureBox()
		Me.CmOpenFile = New ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New ToolStripMenuItem()
		Me.BtMAPopen = New Button()
		Me.PnInertia = New Panel()
		Me.GrWHTC = New GroupBox()
		Me.BtWHTCimport = New Button()
		Me.Label13 = New Label()
		Me.TbWHTCmw = New TextBox()
		Me.TbWHTCrural = New TextBox()
		Me.TbWHTCurban = New TextBox()
		Me.Label8 = New Label()
		Me.Label7 = New Label()
		Me.Label4 = New Label()
		Me.PicBox = New PictureBox()
		Me.TbFLD = New TextBox()
		Me.Label14 = New Label()
		Me.BtFLD = New Button()
		Me.BtFLDopen = New Button()
		Me.ToolStrip1.SuspendLayout()
		Me.StatusStrip1.SuspendLayout()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		Me.CmOpenFile.SuspendLayout()
		Me.PnInertia.SuspendLayout()
		Me.GrWHTC.SuspendLayout()
		CType(Me.PicBox, ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'TbNleerl
		'
		Me.TbNleerl.Location = New Point(123, 108)
		Me.TbNleerl.Name = "TbNleerl"
		Me.TbNleerl.Size = New Size(57, 20)
		Me.TbNleerl.TabIndex = 1
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New Point(15, 111)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New Size(102, 13)
		Me.Label11.TabIndex = 15
		Me.Label11.Text = "Idling Engine Speed"
		'
		'TbInertia
		'
		Me.TbInertia.Location = New Point(120, 2)
		Me.TbInertia.Name = "TbInertia"
		Me.TbInertia.Size = New Size(57, 20)
		Me.TbInertia.TabIndex = 3
		'
		'Label41
		'
		Me.Label41.AutoSize = True
		Me.Label41.Location = New Point(183, 5)
		Me.Label41.Name = "Label41"
		Me.Label41.Size = New Size(36, 13)
		Me.Label41.TabIndex = 24
		Me.Label41.Text = "[kgm²]"
		'
		'Label40
		'
		Me.Label40.AutoSize = True
		Me.Label40.Location = New Point(186, 111)
		Me.Label40.Name = "Label40"
		Me.Label40.Size = New Size(30, 13)
		Me.Label40.TabIndex = 24
		Me.Label40.Text = "[rpm]"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(12, 5)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(102, 13)
		Me.Label5.TabIndex = 0
		Me.Label5.Text = "Inertia incl. Flywheel"
		'
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButCancel.DialogResult = DialogResult.Cancel
		Me.ButCancel.Location = New Point(898, 469)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New Size(75, 23)
		Me.ButCancel.TabIndex = 13
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButOK.Location = New Point(817, 469)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New Size(75, 23)
		Me.ButOK.TabIndex = 12
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New Size(985, 25)
		Me.ToolStrip1.TabIndex = 30
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtNew
		'
		Me.ToolStripBtNew.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtNew.Image = My.Resources.Resources.blue_document_icon
		Me.ToolStripBtNew.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtNew.Name = "ToolStripBtNew"
		Me.ToolStripBtNew.Size = New Size(23, 22)
		Me.ToolStripBtNew.Text = "ToolStripButton1"
		Me.ToolStripBtNew.ToolTipText = "New"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New Size(23, 22)
		Me.ToolStripBtOpen.Text = "ToolStripButton1"
		Me.ToolStripBtOpen.ToolTipText = "Open..."
		'
		'ToolStripBtSave
		'
		Me.ToolStripBtSave.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSave.Image = My.Resources.Resources.Actions_document_save_icon
		Me.ToolStripBtSave.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtSave.Name = "ToolStripBtSave"
		Me.ToolStripBtSave.Size = New Size(23, 22)
		Me.ToolStripBtSave.Text = "ToolStripButton1"
		Me.ToolStripBtSave.ToolTipText = "Save"
		'
		'ToolStripBtSaveAs
		'
		Me.ToolStripBtSaveAs.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSaveAs.Image = My.Resources.Resources.Actions_document_save_as_icon
		Me.ToolStripBtSaveAs.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
		Me.ToolStripBtSaveAs.Size = New Size(23, 22)
		Me.ToolStripBtSaveAs.Text = "ToolStripButton1"
		Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
		'
		'ToolStripSeparator3
		'
		Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
		Me.ToolStripSeparator3.Size = New Size(6, 25)
		'
		'ToolStripBtSendTo
		'
		Me.ToolStripBtSendTo.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSendTo.Image = My.Resources.Resources.export_icon
		Me.ToolStripBtSendTo.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
		Me.ToolStripBtSendTo.Size = New Size(23, 22)
		Me.ToolStripBtSendTo.Text = "Send to Job Editor"
		Me.ToolStripBtSendTo.ToolTipText = "Send to Job Editor"
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
		'StatusStrip1
		'
		Me.StatusStrip1.Items.AddRange(New ToolStripItem() {Me.LbStatus})
		Me.StatusStrip1.Location = New Point(0, 495)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New Size(985, 22)
		Me.StatusStrip1.SizingGrip = False
		Me.StatusStrip1.TabIndex = 37
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'LbStatus
		'
		Me.LbStatus.Name = "LbStatus"
		Me.LbStatus.Size = New Size(39, 17)
		Me.LbStatus.Text = "Status"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(186, 137)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(33, 13)
		Me.Label1.TabIndex = 24
		Me.Label1.Text = "[ccm]"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(46, 137)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(71, 13)
		Me.Label2.TabIndex = 13
		Me.Label2.Text = "Displacement"
		'
		'TbDispl
		'
		Me.TbDispl.Location = New Point(123, 134)
		Me.TbDispl.Name = "TbDispl"
		Me.TbDispl.Size = New Size(57, 20)
		Me.TbDispl.TabIndex = 2
		'
		'TbName
		'
		Me.TbName.Location = New Point(123, 82)
		Me.TbName.Name = "TbName"
		Me.TbName.Size = New Size(370, 20)
		Me.TbName.TabIndex = 0
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(30, 85)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(87, 13)
		Me.Label3.TabIndex = 11
		Me.Label3.Text = "Make and Model"
		'
		'TbMAP
		'
		Me.TbMAP.Location = New Point(12, 259)
		Me.TbMAP.Name = "TbMAP"
		Me.TbMAP.Size = New Size(434, 20)
		Me.TbMAP.TabIndex = 5
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New Point(12, 243)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New Size(115, 13)
		Me.Label6.TabIndex = 38
		Me.Label6.Text = "Fuel Consumption Map"
		'
		'BtMAP
		'
		Me.BtMAP.Image = My.Resources.Resources.Open_icon
		Me.BtMAP.Location = New Point(446, 257)
		Me.BtMAP.Name = "BtMAP"
		Me.BtMAP.Size = New Size(24, 24)
		Me.BtMAP.TabIndex = 6
		Me.BtMAP.TabStop = False
		Me.BtMAP.UseVisualStyleBackColor = True
		'
		'PictureBox1
		'
		Me.PictureBox1.BackColor = Color.White
		Me.PictureBox1.Image = My.Resources.Resources.VECTO_ENG
		Me.PictureBox1.Location = New Point(12, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(481, 40)
		Me.PictureBox1.TabIndex = 39
		Me.PictureBox1.TabStop = False
		'
		'CmOpenFile
		'
		Me.CmOpenFile.Items.AddRange(New ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
		Me.CmOpenFile.Name = "CmOpenFile"
		Me.CmOpenFile.Size = New Size(153, 48)
		'
		'OpenWithToolStripMenuItem
		'
		Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
		Me.OpenWithToolStripMenuItem.Size = New Size(152, 22)
		Me.OpenWithToolStripMenuItem.Text = "Open with ..."
		'
		'ShowInFolderToolStripMenuItem
		'
		Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
		Me.ShowInFolderToolStripMenuItem.Size = New Size(152, 22)
		Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
		'
		'BtMAPopen
		'
		Me.BtMAPopen.Image = My.Resources.Resources.application_export_icon_small
		Me.BtMAPopen.Location = New Point(469, 257)
		Me.BtMAPopen.Name = "BtMAPopen"
		Me.BtMAPopen.Size = New Size(24, 24)
		Me.BtMAPopen.TabIndex = 7
		Me.BtMAPopen.TabStop = False
		Me.BtMAPopen.UseVisualStyleBackColor = True
		'
		'PnInertia
		'
		Me.PnInertia.Controls.Add(Me.Label5)
		Me.PnInertia.Controls.Add(Me.Label41)
		Me.PnInertia.Controls.Add(Me.TbInertia)
		Me.PnInertia.Location = New Point(264, 106)
		Me.PnInertia.Name = "PnInertia"
		Me.PnInertia.Size = New Size(229, 32)
		Me.PnInertia.TabIndex = 3
		'
		'GrWHTC
		'
		Me.GrWHTC.Controls.Add(Me.BtWHTCimport)
		Me.GrWHTC.Controls.Add(Me.Label13)
		Me.GrWHTC.Controls.Add(Me.TbWHTCmw)
		Me.GrWHTC.Controls.Add(Me.TbWHTCrural)
		Me.GrWHTC.Controls.Add(Me.TbWHTCurban)
		Me.GrWHTC.Controls.Add(Me.Label8)
		Me.GrWHTC.Controls.Add(Me.Label7)
		Me.GrWHTC.Controls.Add(Me.Label4)
		Me.GrWHTC.Enabled = False
		Me.GrWHTC.Location = New Point(12, 300)
		Me.GrWHTC.Name = "GrWHTC"
		Me.GrWHTC.Size = New Size(481, 91)
		Me.GrWHTC.TabIndex = 9
		Me.GrWHTC.TabStop = False
		Me.GrWHTC.Text = "WHTC Correction"
		'
		'BtWHTCimport
		'
		Me.BtWHTCimport.Location = New Point(305, 19)
		Me.BtWHTCimport.Name = "BtWHTCimport"
		Me.BtWHTCimport.Size = New Size(170, 28)
		Me.BtWHTCimport.TabIndex = 4
		Me.BtWHTCimport.Text = "Import from VECTO-Engine"
		Me.BtWHTCimport.UseVisualStyleBackColor = True
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New Point(6, 27)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New Size(242, 13)
		Me.Label13.TabIndex = 3
		Me.Label13.Text = "Correction Factors calculated with VECTO-Engine"
		'
		'TbWHTCmw
		'
		Me.TbWHTCmw.Location = New Point(348, 56)
		Me.TbWHTCmw.Name = "TbWHTCmw"
		Me.TbWHTCmw.Size = New Size(57, 20)
		Me.TbWHTCmw.TabIndex = 2
		'
		'TbWHTCrural
		'
		Me.TbWHTCrural.Location = New Point(192, 56)
		Me.TbWHTCrural.Name = "TbWHTCrural"
		Me.TbWHTCrural.Size = New Size(57, 20)
		Me.TbWHTCrural.TabIndex = 1
		'
		'TbWHTCurban
		'
		Me.TbWHTCurban.Location = New Point(48, 56)
		Me.TbWHTCurban.Name = "TbWHTCurban"
		Me.TbWHTCurban.Size = New Size(57, 20)
		Me.TbWHTCurban.TabIndex = 0
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New Point(289, 59)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New Size(53, 13)
		Me.Label8.TabIndex = 0
		Me.Label8.Text = "Motorway"
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New Point(154, 59)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New Size(32, 13)
		Me.Label7.TabIndex = 0
		Me.Label7.Text = "Rural"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(6, 59)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(36, 13)
		Me.Label4.TabIndex = 0
		Me.Label4.Text = "Urban"
		'
		'PicBox
		'
		Me.PicBox.BackColor = Color.LightGray
		Me.PicBox.Location = New Point(499, 28)
		Me.PicBox.Name = "PicBox"
		Me.PicBox.Size = New Size(474, 425)
		Me.PicBox.TabIndex = 40
		Me.PicBox.TabStop = False
		'
		'TbFLD
		'
		Me.TbFLD.Location = New Point(12, 202)
		Me.TbFLD.Name = "TbFLD"
		Me.TbFLD.Size = New Size(434, 20)
		Me.TbFLD.TabIndex = 5
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New Point(12, 186)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New Size(128, 13)
		Me.Label14.TabIndex = 38
		Me.Label14.Text = "Full Load and Drag Curve"
		'
		'BtFLD
		'
		Me.BtFLD.Image = My.Resources.Resources.Open_icon
		Me.BtFLD.Location = New Point(446, 200)
		Me.BtFLD.Name = "BtFLD"
		Me.BtFLD.Size = New Size(24, 24)
		Me.BtFLD.TabIndex = 6
		Me.BtFLD.TabStop = False
		Me.BtFLD.UseVisualStyleBackColor = True
		'
		'BtFLDopen
		'
		Me.BtFLDopen.Image = My.Resources.Resources.application_export_icon_small
		Me.BtFLDopen.Location = New Point(469, 200)
		Me.BtFLDopen.Name = "BtFLDopen"
		Me.BtFLDopen.Size = New Size(24, 24)
		Me.BtFLDopen.TabIndex = 7
		Me.BtFLDopen.TabStop = False
		Me.BtFLDopen.UseVisualStyleBackColor = True
		'
		'F_ENG
		'
		Me.AcceptButton = Me.ButOK
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New Size(985, 517)
		Me.Controls.Add(Me.PicBox)
		Me.Controls.Add(Me.GrWHTC)
		Me.Controls.Add(Me.PnInertia)
		Me.Controls.Add(Me.BtFLDopen)
		Me.Controls.Add(Me.BtMAPopen)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.BtFLD)
		Me.Controls.Add(Me.BtMAP)
		Me.Controls.Add(Me.Label14)
		Me.Controls.Add(Me.Label6)
		Me.Controls.Add(Me.TbNleerl)
		Me.Controls.Add(Me.StatusStrip1)
		Me.Controls.Add(Me.Label11)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Controls.Add(Me.TbDispl)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.ButCancel)
		Me.Controls.Add(Me.TbFLD)
		Me.Controls.Add(Me.ButOK)
		Me.Controls.Add(Me.TbMAP)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.Label40)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.TbName)
		Me.FormBorderStyle = FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Me.MaximizeBox = False
		Me.Name = "EngineForm"
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "F_ENG"
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		Me.StatusStrip1.ResumeLayout(False)
		Me.StatusStrip1.PerformLayout()
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		Me.CmOpenFile.ResumeLayout(False)
		Me.PnInertia.ResumeLayout(False)
		Me.PnInertia.PerformLayout()
		Me.GrWHTC.ResumeLayout(False)
		Me.GrWHTC.PerformLayout()
		CType(Me.PicBox, ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents TbNleerl As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents TbInertia As TextBox
	Friend WithEvents Label41 As Label
	Friend WithEvents Label40 As Label
	Friend WithEvents Label5 As Label
	Friend WithEvents ButCancel As Button
	Friend WithEvents ButOK As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents LbStatus As ToolStripStatusLabel
	Friend WithEvents Label1 As Label
	Friend WithEvents Label2 As Label
	Friend WithEvents TbDispl As TextBox
	Friend WithEvents TbName As TextBox
	Friend WithEvents Label3 As Label
	Friend WithEvents TbMAP As TextBox
	Friend WithEvents Label6 As Label
	Friend WithEvents BtMAP As Button
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BtMAPopen As Button
	Friend WithEvents PnInertia As Panel
	Friend WithEvents GrWHTC As GroupBox
	Friend WithEvents TbWHTCmw As TextBox
	Friend WithEvents TbWHTCrural As TextBox
	Friend WithEvents TbWHTCurban As TextBox
	Friend WithEvents Label8 As Label
	Friend WithEvents Label7 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents PicBox As PictureBox
	Friend WithEvents Label13 As Label
	Friend WithEvents TbFLD As TextBox
	Friend WithEvents Label14 As Label
	Friend WithEvents BtFLD As Button
	Friend WithEvents BtFLDopen As Button
	Friend WithEvents BtWHTCimport As Button
End Class
