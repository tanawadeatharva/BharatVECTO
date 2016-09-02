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
Partial Class GearboxForm
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
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(GearboxForm))
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
		Me.ButCancel = New Button()
		Me.ButOK = New Button()
		Me.TbTracInt = New TextBox()
		Me.LvGears = New ListView()
		Me.ColumnHeader1 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader2 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader3 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader5 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader6 = CType(New ColumnHeader(), ColumnHeader)
		Me.TBI_getr = New TextBox()
		Me.Label49 = New Label()
		Me.Label33 = New Label()
		Me.Label48 = New Label()
		Me.Label6 = New Label()
		Me.Label3 = New Label()
		Me.TbName = New TextBox()
		Me.PictureBox1 = New PictureBox()
		Me.BtRemGear = New Button()
		Me.GrGearShift = New GroupBox()
		Me.GroupBox1 = New GroupBox()
		Me.tbUpshiftMinAcceleration = New TextBox()
		Me.tbUpshiftAfterDownshift = New TextBox()
		Me.tbDownshiftAfterUpshift = New TextBox()
		Me.Label24 = New Label()
		Me.Label23 = New Label()
		Me.Label22 = New Label()
		Me.Label21 = New Label()
		Me.Label20 = New Label()
		Me.Label19 = New Label()
		Me.PnTorqRes = New Panel()
		Me.Label2 = New Label()
		Me.Label4 = New Label()
		Me.TbTqResv = New TextBox()
		Me.ChShiftInside = New CheckBox()
		Me.TbShiftTime = New TextBox()
		Me.Label12 = New Label()
		Me.Label13 = New Label()
		Me.ChSkipGears = New CheckBox()
		Me.GroupBox2 = New GroupBox()
		Me.TbStartAcc = New TextBox()
		Me.Label11 = New Label()
		Me.TbStartSpeed = New TextBox()
		Me.Label9 = New Label()
		Me.Label10 = New Label()
		Me.TbTqResvStart = New TextBox()
		Me.Label8 = New Label()
		Me.Label5 = New Label()
		Me.Label7 = New Label()
		Me.CmOpenFile = New ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New ToolStripMenuItem()
		Me.GroupBox3 = New GroupBox()
		Me.PnTC = New Panel()
		Me.Label17 = New Label()
		Me.Label18 = New Label()
		Me.Label15 = New Label()
		Me.TbTCinertia = New TextBox()
		Me.Label1 = New Label()
		Me.Label14 = New Label()
		Me.BtTCfileBrowse = New Button()
		Me.TbTCfile = New TextBox()
		Me.BtTCfileOpen = New Button()
		Me.TbTCrefrpm = New TextBox()
		Me.ChTCon = New CheckBox()
		Me.Label16 = New Label()
		Me.CbGStype = New ComboBox()
		Me.BtAddGear = New Button()
		Me.GroupBox4 = New GroupBox()
		Me.Label32 = New Label()
		Me.PnInertiaTI = New Panel()
		Me.PicBox = New PictureBox()
		Me.TBTCShiftPolygon = New TextBox()
		Me.LblTCShiftFile = New Label()
		Me.BtTCShiftFileBrowse = New Button()
		Me.ToolStrip1.SuspendLayout()
		Me.StatusStrip1.SuspendLayout()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		Me.GrGearShift.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me.PnTorqRes.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		Me.CmOpenFile.SuspendLayout()
		Me.GroupBox3.SuspendLayout()
		Me.PnTC.SuspendLayout()
		Me.GroupBox4.SuspendLayout()
		Me.PnInertiaTI.SuspendLayout()
		CType(Me.PicBox, ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New Size(877, 25)
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
		Me.StatusStrip1.Location = New Point(0, 684)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New Size(877, 22)
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
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButCancel.DialogResult = DialogResult.Cancel
		Me.ButCancel.Location = New Point(790, 658)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New Size(75, 23)
		Me.ButCancel.TabIndex = 7
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButOK.Location = New Point(709, 658)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New Size(75, 23)
		Me.ButOK.TabIndex = 6
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'TbTracInt
		'
		Me.TbTracInt.Location = New Point(303, 3)
		Me.TbTracInt.Name = "TbTracInt"
		Me.TbTracInt.Size = New Size(40, 20)
		Me.TbTracInt.TabIndex = 1
		'
		'LvGears
		'
		Me.LvGears.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3, Me.ColumnHeader5, Me.ColumnHeader6})
		Me.LvGears.FullRowSelect = True
		Me.LvGears.GridLines = True
		Me.LvGears.HideSelection = False
		Me.LvGears.Location = New Point(6, 18)
		Me.LvGears.MultiSelect = False
		Me.LvGears.Name = "LvGears"
		Me.LvGears.Size = New Size(429, 183)
		Me.LvGears.TabIndex = 0
		Me.LvGears.TabStop = False
		Me.LvGears.UseCompatibleStateImageBehavior = False
		Me.LvGears.View = View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Gear"
		Me.ColumnHeader1.Width = 46
		'
		'ColumnHeader2
		'
		Me.ColumnHeader2.Text = "Ratio"
		Me.ColumnHeader2.Width = 50
		'
		'ColumnHeader3
		'
		Me.ColumnHeader3.Text = "Loss Map or Efficiency"
		Me.ColumnHeader3.Width = 128
		'
		'ColumnHeader5
		'
		Me.ColumnHeader5.Text = "Shift Polygons"
		Me.ColumnHeader5.Width = 94
		'
		'ColumnHeader6
		'
		Me.ColumnHeader6.Text = "Max Torque"
		Me.ColumnHeader6.Width = 95
		'
		'TBI_getr
		'
		Me.TBI_getr.Location = New Point(53, 3)
		Me.TBI_getr.Name = "TBI_getr"
		Me.TBI_getr.Size = New Size(57, 20)
		Me.TBI_getr.TabIndex = 0
		'
		'Label49
		'
		Me.Label49.AutoSize = True
		Me.Label49.Location = New Point(349, 6)
		Me.Label49.Name = "Label49"
		Me.Label49.Size = New Size(18, 13)
		Me.Label49.TabIndex = 2
		Me.Label49.Text = "[s]"
		'
		'Label33
		'
		Me.Label33.AutoSize = True
		Me.Label33.Location = New Point(116, 6)
		Me.Label33.Name = "Label33"
		Me.Label33.Size = New Size(36, 13)
		Me.Label33.TabIndex = 2
		Me.Label33.Text = "[kgm²]"
		'
		'Label48
		'
		Me.Label48.AutoSize = True
		Me.Label48.Location = New Point(195, 6)
		Me.Label48.Name = "Label48"
		Me.Label48.Size = New Size(102, 13)
		Me.Label48.TabIndex = 2
		Me.Label48.Text = "Traction Interruption"
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New Point(11, 6)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New Size(36, 13)
		Me.Label6.TabIndex = 2
		Me.Label6.Text = "Inertia"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(26, 85)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(87, 13)
		Me.Label3.TabIndex = 42
		Me.Label3.Text = "Make and Model"
		'
		'TbName
		'
		Me.TbName.Location = New Point(119, 82)
		Me.TbName.Name = "TbName"
		Me.TbName.Size = New Size(334, 20)
		Me.TbName.TabIndex = 0
		'
		'PictureBox1
		'
		Me.PictureBox1.BackColor = Color.White
		Me.PictureBox1.Image = My.Resources.Resources.VECTO_GBX
		Me.PictureBox1.Location = New Point(12, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(441, 40)
		Me.PictureBox1.TabIndex = 43
		Me.PictureBox1.TabStop = False
		'
		'BtRemGear
		'
		Me.BtRemGear.Image = My.Resources.Resources.minus_circle_icon
		Me.BtRemGear.Location = New Point(30, 202)
		Me.BtRemGear.Name = "BtRemGear"
		Me.BtRemGear.Size = New Size(24, 24)
		Me.BtRemGear.TabIndex = 2
		Me.BtRemGear.UseVisualStyleBackColor = True
		'
		'GrGearShift
		'
		Me.GrGearShift.Controls.Add(Me.GroupBox1)
		Me.GrGearShift.Controls.Add(Me.PnTorqRes)
		Me.GrGearShift.Controls.Add(Me.ChShiftInside)
		Me.GrGearShift.Controls.Add(Me.TbShiftTime)
		Me.GrGearShift.Controls.Add(Me.Label12)
		Me.GrGearShift.Controls.Add(Me.Label13)
		Me.GrGearShift.Controls.Add(Me.ChSkipGears)
		Me.GrGearShift.Controls.Add(Me.GroupBox2)
		Me.GrGearShift.Location = New Point(12, 377)
		Me.GrGearShift.Name = "GrGearShift"
		Me.GrGearShift.Size = New Size(441, 291)
		Me.GrGearShift.TabIndex = 3
		Me.GrGearShift.TabStop = False
		Me.GrGearShift.Text = "Gear shift parameters"
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.tbUpshiftMinAcceleration)
		Me.GroupBox1.Controls.Add(Me.tbUpshiftAfterDownshift)
		Me.GroupBox1.Controls.Add(Me.tbDownshiftAfterUpshift)
		Me.GroupBox1.Controls.Add(Me.Label24)
		Me.GroupBox1.Controls.Add(Me.Label23)
		Me.GroupBox1.Controls.Add(Me.Label22)
		Me.GroupBox1.Controls.Add(Me.Label21)
		Me.GroupBox1.Controls.Add(Me.Label20)
		Me.GroupBox1.Controls.Add(Me.Label19)
		Me.GroupBox1.Location = New Point(6, 82)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(429, 100)
		Me.GroupBox1.TabIndex = 10
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Shift Strategy Parameters"
		'
		'tbUpshiftMinAcceleration
		'
		Me.tbUpshiftMinAcceleration.Location = New Point(209, 71)
		Me.tbUpshiftMinAcceleration.Name = "tbUpshiftMinAcceleration"
		Me.tbUpshiftMinAcceleration.Size = New Size(57, 20)
		Me.tbUpshiftMinAcceleration.TabIndex = 8
		'
		'tbUpshiftAfterDownshift
		'
		Me.tbUpshiftAfterDownshift.Location = New Point(209, 45)
		Me.tbUpshiftAfterDownshift.Name = "tbUpshiftAfterDownshift"
		Me.tbUpshiftAfterDownshift.Size = New Size(57, 20)
		Me.tbUpshiftAfterDownshift.TabIndex = 7
		'
		'tbDownshiftAfterUpshift
		'
		Me.tbDownshiftAfterUpshift.Location = New Point(209, 20)
		Me.tbDownshiftAfterUpshift.Name = "tbDownshiftAfterUpshift"
		Me.tbDownshiftAfterUpshift.Size = New Size(57, 20)
		Me.tbDownshiftAfterUpshift.TabIndex = 6
		'
		'Label24
		'
		Me.Label24.AutoSize = True
		Me.Label24.Location = New Point(275, 74)
		Me.Label24.Name = "Label24"
		Me.Label24.Size = New Size(34, 13)
		Me.Label24.TabIndex = 5
		Me.Label24.Text = "[m/s²]"
		'
		'Label23
		'
		Me.Label23.AutoSize = True
		Me.Label23.Location = New Point(275, 48)
		Me.Label23.Name = "Label23"
		Me.Label23.Size = New Size(18, 13)
		Me.Label23.TabIndex = 4
		Me.Label23.Text = "[s]"
		'
		'Label22
		'
		Me.Label22.AutoSize = True
		Me.Label22.Location = New Point(275, 25)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New Size(18, 13)
		Me.Label22.TabIndex = 3
		Me.Label22.Text = "[s]"
		'
		'Label21
		'
		Me.Label21.AutoSize = True
		Me.Label21.Location = New Point(20, 74)
		Me.Label21.Name = "Label21"
		Me.Label21.Size = New Size(146, 13)
		Me.Label21.TabIndex = 2
		Me.Label21.Text = "Min. acceleration after upshift"
		'
		'Label20
		'
		Me.Label20.AutoSize = True
		Me.Label20.Location = New Point(20, 48)
		Me.Label20.Name = "Label20"
		Me.Label20.Size = New Size(137, 13)
		Me.Label20.TabIndex = 1
		Me.Label20.Text = "Upshift afer downshift delay"
		'
		'Label19
		'
		Me.Label19.AutoSize = True
		Me.Label19.Location = New Point(20, 25)
		Me.Label19.Name = "Label19"
		Me.Label19.Size = New Size(137, 13)
		Me.Label19.TabIndex = 0
		Me.Label19.Text = "Downshift afer upshift delay"
		'
		'PnTorqRes
		'
		Me.PnTorqRes.Controls.Add(Me.Label2)
		Me.PnTorqRes.Controls.Add(Me.Label4)
		Me.PnTorqRes.Controls.Add(Me.TbTqResv)
		Me.PnTorqRes.Location = New Point(230, 19)
		Me.PnTorqRes.Name = "PnTorqRes"
		Me.PnTorqRes.Size = New Size(190, 32)
		Me.PnTorqRes.TabIndex = 2
		'
		'Label2
		'
		Me.Label2.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(10, 9)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(79, 13)
		Me.Label2.TabIndex = 0
		Me.Label2.Text = "Torque reserve"
		'
		'Label4
		'
		Me.Label4.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(158, 9)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(21, 13)
		Me.Label4.TabIndex = 0
		Me.Label4.Text = "[%]"
		'
		'TbTqResv
		'
		Me.TbTqResv.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.TbTqResv.Location = New Point(95, 6)
		Me.TbTqResv.Name = "TbTqResv"
		Me.TbTqResv.Size = New Size(57, 20)
		Me.TbTqResv.TabIndex = 0
		'
		'ChShiftInside
		'
		Me.ChShiftInside.AutoSize = True
		Me.ChShiftInside.Checked = True
		Me.ChShiftInside.CheckState = CheckState.Checked
		Me.ChShiftInside.Location = New Point(9, 27)
		Me.ChShiftInside.Name = "ChShiftInside"
		Me.ChShiftInside.Size = New Size(195, 17)
		Me.ChShiftInside.TabIndex = 0
		Me.ChShiftInside.Text = "Allow shift-up inside polygons (AMT)"
		Me.ChShiftInside.UseVisualStyleBackColor = True
		'
		'TbShiftTime
		'
		Me.TbShiftTime.Location = New Point(325, 51)
		Me.TbShiftTime.Name = "TbShiftTime"
		Me.TbShiftTime.Size = New Size(57, 20)
		Me.TbShiftTime.TabIndex = 3
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New Point(388, 54)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New Size(18, 13)
		Me.Label12.TabIndex = 9
		Me.Label12.Text = "[s]"
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New Point(227, 54)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New Size(92, 13)
		Me.Label13.TabIndex = 8
		Me.Label13.Text = "Minimum shift time"
		'
		'ChSkipGears
		'
		Me.ChSkipGears.AutoSize = True
		Me.ChSkipGears.Location = New Point(9, 53)
		Me.ChSkipGears.Name = "ChSkipGears"
		Me.ChSkipGears.Size = New Size(132, 17)
		Me.ChSkipGears.TabIndex = 1
		Me.ChSkipGears.Text = "Skip Gears (MT, AMT)"
		Me.ChSkipGears.UseVisualStyleBackColor = True
		'
		'GroupBox2
		'
		Me.GroupBox2.Controls.Add(Me.TbStartAcc)
		Me.GroupBox2.Controls.Add(Me.Label11)
		Me.GroupBox2.Controls.Add(Me.TbStartSpeed)
		Me.GroupBox2.Controls.Add(Me.Label9)
		Me.GroupBox2.Controls.Add(Me.Label10)
		Me.GroupBox2.Controls.Add(Me.TbTqResvStart)
		Me.GroupBox2.Controls.Add(Me.Label8)
		Me.GroupBox2.Controls.Add(Me.Label5)
		Me.GroupBox2.Controls.Add(Me.Label7)
		Me.GroupBox2.Location = New Point(6, 186)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New Size(429, 99)
		Me.GroupBox2.TabIndex = 4
		Me.GroupBox2.TabStop = False
		Me.GroupBox2.Text = "Start Gear"
		'
		'TbStartAcc
		'
		Me.TbStartAcc.Location = New Point(209, 71)
		Me.TbStartAcc.Name = "TbStartAcc"
		Me.TbStartAcc.Size = New Size(57, 20)
		Me.TbStartAcc.TabIndex = 2
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New Point(20, 74)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New Size(173, 13)
		Me.Label11.TabIndex = 0
		Me.Label11.Text = "Reference acceleration at clutch-in"
		'
		'TbStartSpeed
		'
		Me.TbStartSpeed.Location = New Point(209, 45)
		Me.TbStartSpeed.Name = "TbStartSpeed"
		Me.TbStartSpeed.Size = New Size(57, 20)
		Me.TbStartSpeed.TabIndex = 1
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New Point(20, 48)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New Size(181, 13)
		Me.Label9.TabIndex = 0
		Me.Label9.Text = "Reference vehicle speed at clutch-in"
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New Point(272, 74)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New Size(34, 13)
		Me.Label10.TabIndex = 0
		Me.Label10.Text = "[m/s²]"
		'
		'TbTqResvStart
		'
		Me.TbTqResvStart.Location = New Point(209, 19)
		Me.TbTqResvStart.Name = "TbTqResvStart"
		Me.TbTqResvStart.Size = New Size(57, 20)
		Me.TbTqResvStart.TabIndex = 0
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New Point(272, 48)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New Size(31, 13)
		Me.Label8.TabIndex = 0
		Me.Label8.Text = "[m/s]"
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(20, 22)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(79, 13)
		Me.Label5.TabIndex = 0
		Me.Label5.Text = "Torque reserve"
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New Point(272, 22)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New Size(21, 13)
		Me.Label7.TabIndex = 0
		Me.Label7.Text = "[%]"
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
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me.BtTCShiftFileBrowse)
		Me.GroupBox3.Controls.Add(Me.LblTCShiftFile)
		Me.GroupBox3.Controls.Add(Me.TBTCShiftPolygon)
		Me.GroupBox3.Controls.Add(Me.PnTC)
		Me.GroupBox3.Controls.Add(Me.ChTCon)
		Me.GroupBox3.Location = New Point(459, 440)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New Size(414, 162)
		Me.GroupBox3.TabIndex = 5
		Me.GroupBox3.TabStop = False
		Me.GroupBox3.Text = "Torque Converter"
		'
		'PnTC
		'
		Me.PnTC.Controls.Add(Me.Label17)
		Me.PnTC.Controls.Add(Me.Label18)
		Me.PnTC.Controls.Add(Me.Label15)
		Me.PnTC.Controls.Add(Me.TbTCinertia)
		Me.PnTC.Controls.Add(Me.Label1)
		Me.PnTC.Controls.Add(Me.Label14)
		Me.PnTC.Controls.Add(Me.BtTCfileBrowse)
		Me.PnTC.Controls.Add(Me.TbTCfile)
		Me.PnTC.Controls.Add(Me.BtTCfileOpen)
		Me.PnTC.Controls.Add(Me.TbTCrefrpm)
		Me.PnTC.Location = New Point(6, 39)
		Me.PnTC.Name = "PnTC"
		Me.PnTC.Size = New Size(402, 72)
		Me.PnTC.TabIndex = 36
		'
		'Label17
		'
		Me.Label17.AutoSize = True
		Me.Label17.Location = New Point(0, 5)
		Me.Label17.Name = "Label17"
		Me.Label17.Size = New Size(176, 13)
		Me.Label17.TabIndex = 0
		Me.Label17.Text = "Torque converter characteristics file"
		'
		'Label18
		'
		Me.Label18.AutoSize = True
		Me.Label18.Location = New Point(5, 50)
		Me.Label18.Name = "Label18"
		Me.Label18.Size = New Size(36, 13)
		Me.Label18.TabIndex = 2
		Me.Label18.Text = "Inertia"
		'
		'Label15
		'
		Me.Label15.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label15.AutoSize = True
		Me.Label15.Location = New Point(185, 50)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New Size(77, 13)
		Me.Label15.TabIndex = 0
		Me.Label15.Text = "Reference rpm"
		'
		'TbTCinertia
		'
		Me.TbTCinertia.Location = New Point(47, 47)
		Me.TbTCinertia.Name = "TbTCinertia"
		Me.TbTCinertia.Size = New Size(67, 20)
		Me.TbTCinertia.TabIndex = 3
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(120, 50)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(36, 13)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "[kgm²]"
		'
		'Label14
		'
		Me.Label14.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label14.AutoSize = True
		Me.Label14.Location = New Point(341, 50)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New Size(40, 13)
		Me.Label14.TabIndex = 0
		Me.Label14.Text = "[1/min]"
		'
		'BtTCfileBrowse
		'
		Me.BtTCfileBrowse.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.BtTCfileBrowse.Image = My.Resources.Resources.Open_icon
		Me.BtTCfileBrowse.Location = New Point(348, 19)
		Me.BtTCfileBrowse.Name = "BtTCfileBrowse"
		Me.BtTCfileBrowse.Size = New Size(24, 24)
		Me.BtTCfileBrowse.TabIndex = 1
		Me.BtTCfileBrowse.TabStop = False
		Me.BtTCfileBrowse.UseVisualStyleBackColor = True
		'
		'TbTCfile
		'
		Me.TbTCfile.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbTCfile.Location = New Point(4, 21)
		Me.TbTCfile.Name = "TbTCfile"
		Me.TbTCfile.Size = New Size(343, 20)
		Me.TbTCfile.TabIndex = 0
		'
		'BtTCfileOpen
		'
		Me.BtTCfileOpen.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.BtTCfileOpen.Image = My.Resources.Resources.application_export_icon_small
		Me.BtTCfileOpen.Location = New Point(372, 19)
		Me.BtTCfileOpen.Name = "BtTCfileOpen"
		Me.BtTCfileOpen.Size = New Size(24, 24)
		Me.BtTCfileOpen.TabIndex = 2
		Me.BtTCfileOpen.TabStop = False
		Me.BtTCfileOpen.UseVisualStyleBackColor = True
		'
		'TbTCrefrpm
		'
		Me.TbTCrefrpm.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.TbTCrefrpm.Location = New Point(268, 47)
		Me.TbTCrefrpm.Name = "TbTCrefrpm"
		Me.TbTCrefrpm.Size = New Size(67, 20)
		Me.TbTCrefrpm.TabIndex = 4
		'
		'ChTCon
		'
		Me.ChTCon.AutoSize = True
		Me.ChTCon.Checked = True
		Me.ChTCon.CheckState = CheckState.Checked
		Me.ChTCon.Location = New Point(9, 19)
		Me.ChTCon.Name = "ChTCon"
		Me.ChTCon.Size = New Size(88, 17)
		Me.ChTCon.TabIndex = 0
		Me.ChTCon.Text = "Installed (AT)"
		Me.ChTCon.UseVisualStyleBackColor = True
		'
		'Label16
		'
		Me.Label16.AutoSize = True
		Me.Label16.Location = New Point(18, 111)
		Me.Label16.Name = "Label16"
		Me.Label16.Size = New Size(95, 13)
		Me.Label16.TabIndex = 45
		Me.Label16.Text = "Transmission Type"
		'
		'CbGStype
		'
		Me.CbGStype.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbGStype.FormattingEnabled = True
		Me.CbGStype.Location = New Point(119, 108)
		Me.CbGStype.Name = "CbGStype"
		Me.CbGStype.Size = New Size(227, 21)
		Me.CbGStype.TabIndex = 1
		'
		'BtAddGear
		'
		Me.BtAddGear.Image = My.Resources.Resources.plus_circle_icon
		Me.BtAddGear.Location = New Point(6, 202)
		Me.BtAddGear.Name = "BtAddGear"
		Me.BtAddGear.Size = New Size(24, 24)
		Me.BtAddGear.TabIndex = 1
		Me.BtAddGear.UseVisualStyleBackColor = True
		'
		'GroupBox4
		'
		Me.GroupBox4.Controls.Add(Me.Label32)
		Me.GroupBox4.Controls.Add(Me.LvGears)
		Me.GroupBox4.Controls.Add(Me.BtRemGear)
		Me.GroupBox4.Controls.Add(Me.BtAddGear)
		Me.GroupBox4.Location = New Point(12, 135)
		Me.GroupBox4.Name = "GroupBox4"
		Me.GroupBox4.Size = New Size(441, 232)
		Me.GroupBox4.TabIndex = 2
		Me.GroupBox4.TabStop = False
		Me.GroupBox4.Text = "Gears"
		'
		'Label32
		'
		Me.Label32.AutoSize = True
		Me.Label32.Location = New Point(331, 202)
		Me.Label32.Name = "Label32"
		Me.Label32.Size = New Size(106, 13)
		Me.Label32.TabIndex = 3
		Me.Label32.Text = "(Double-Click to Edit)"
		'
		'PnInertiaTI
		'
		Me.PnInertiaTI.Controls.Add(Me.Label6)
		Me.PnInertiaTI.Controls.Add(Me.TBI_getr)
		Me.PnInertiaTI.Controls.Add(Me.Label33)
		Me.PnInertiaTI.Controls.Add(Me.Label48)
		Me.PnInertiaTI.Controls.Add(Me.Label49)
		Me.PnInertiaTI.Controls.Add(Me.TbTracInt)
		Me.PnInertiaTI.Location = New Point(469, 406)
		Me.PnInertiaTI.Name = "PnInertiaTI"
		Me.PnInertiaTI.Size = New Size(398, 30)
		Me.PnInertiaTI.TabIndex = 4
		'
		'PicBox
		'
		Me.PicBox.BackColor = Color.LightGray
		Me.PicBox.Location = New Point(459, 28)
		Me.PicBox.Name = "PicBox"
		Me.PicBox.Size = New Size(406, 372)
		Me.PicBox.TabIndex = 48
		Me.PicBox.TabStop = False
		'
		'TBTCShiftPolygon
		'
		Me.TBTCShiftPolygon.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TBTCShiftPolygon.Location = New Point(10, 136)
		Me.TBTCShiftPolygon.Name = "TBTCShiftPolygon"
		Me.TBTCShiftPolygon.Size = New Size(343, 20)
		Me.TBTCShiftPolygon.TabIndex = 37
		'
		'LblTCShiftFile
		'
		Me.LblTCShiftFile.AutoSize = True
		Me.LblTCShiftFile.Location = New Point(7, 120)
		Me.LblTCShiftFile.Name = "LblTCShiftFile"
		Me.LblTCShiftFile.Size = New Size(172, 13)
		Me.LblTCShiftFile.TabIndex = 5
		Me.LblTCShiftFile.Text = "Torque converter shift polygons file"
		'
		'BtTCShiftFileBrowse
		'
		Me.BtTCShiftFileBrowse.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.BtTCShiftFileBrowse.Image = My.Resources.Resources.Open_icon
		Me.BtTCShiftFileBrowse.Location = New Point(354, 134)
		Me.BtTCShiftFileBrowse.Name = "BtTCShiftFileBrowse"
		Me.BtTCShiftFileBrowse.Size = New Size(24, 24)
		Me.BtTCShiftFileBrowse.TabIndex = 5
		Me.BtTCShiftFileBrowse.TabStop = False
		Me.BtTCShiftFileBrowse.UseVisualStyleBackColor = True
		'
		'F_GBX
		'
		Me.AcceptButton = Me.ButOK
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New Size(877, 706)
		Me.Controls.Add(Me.PicBox)
		Me.Controls.Add(Me.PnInertiaTI)
		Me.Controls.Add(Me.GroupBox4)
		Me.Controls.Add(Me.CbGStype)
		Me.Controls.Add(Me.Label16)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.GrGearShift)
		Me.Controls.Add(Me.GroupBox3)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.TbName)
		Me.Controls.Add(Me.ButCancel)
		Me.Controls.Add(Me.ButOK)
		Me.Controls.Add(Me.StatusStrip1)
		Me.Controls.Add(Me.ToolStrip1)
		Me.FormBorderStyle = FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Me.MaximizeBox = False
		Me.Name = "GearboxForm"
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "F_GBX"
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		Me.StatusStrip1.ResumeLayout(False)
		Me.StatusStrip1.PerformLayout()
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		Me.GrGearShift.ResumeLayout(False)
		Me.GrGearShift.PerformLayout()
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.PnTorqRes.ResumeLayout(False)
		Me.PnTorqRes.PerformLayout()
		Me.GroupBox2.ResumeLayout(False)
		Me.GroupBox2.PerformLayout()
		Me.CmOpenFile.ResumeLayout(False)
		Me.GroupBox3.ResumeLayout(False)
		Me.GroupBox3.PerformLayout()
		Me.PnTC.ResumeLayout(False)
		Me.PnTC.PerformLayout()
		Me.GroupBox4.ResumeLayout(False)
		Me.GroupBox4.PerformLayout()
		Me.PnInertiaTI.ResumeLayout(False)
		Me.PnInertiaTI.PerformLayout()
		CType(Me.PicBox, ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents LbStatus As ToolStripStatusLabel
	Friend WithEvents ButCancel As Button
	Friend WithEvents ButOK As Button
	Friend WithEvents BtRemGear As Button
	Friend WithEvents TbTracInt As TextBox
	Friend WithEvents LvGears As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents ColumnHeader2 As ColumnHeader
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents TBI_getr As TextBox
	Friend WithEvents Label49 As Label
	Friend WithEvents Label33 As Label
	Friend WithEvents Label48 As Label
	Friend WithEvents Label6 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents TbName As TextBox
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents GrGearShift As GroupBox
	Friend WithEvents Label2 As Label
	Friend WithEvents TbTqResvStart As TextBox
	Friend WithEvents TbTqResv As TextBox
	Friend WithEvents Label7 As Label
	Friend WithEvents Label5 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents GroupBox2 As GroupBox
	Friend WithEvents TbStartAcc As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents TbStartSpeed As TextBox
	Friend WithEvents Label9 As Label
	Friend WithEvents Label10 As Label
	Friend WithEvents Label8 As Label
	Friend WithEvents TbShiftTime As TextBox
	Friend WithEvents Label12 As Label
	Friend WithEvents Label13 As Label
	Friend WithEvents ChSkipGears As CheckBox
	Friend WithEvents ChShiftInside As CheckBox
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents GroupBox3 As GroupBox
	Friend WithEvents TbTCfile As TextBox
	Friend WithEvents ChTCon As CheckBox
	Friend WithEvents BtTCfileBrowse As Button
	Friend WithEvents BtTCfileOpen As Button
	Friend WithEvents TbTCrefrpm As TextBox
	Friend WithEvents Label14 As Label
	Friend WithEvents Label15 As Label
	Friend WithEvents Label16 As Label
	Friend WithEvents CbGStype As ComboBox
	Friend WithEvents Label17 As Label
	Friend WithEvents PnTC As Panel
	Friend WithEvents PnTorqRes As Panel
	Friend WithEvents BtAddGear As Button
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents GroupBox4 As GroupBox
	Friend WithEvents Label32 As Label
	Friend WithEvents PnInertiaTI As Panel
	Friend WithEvents ColumnHeader5 As ColumnHeader
	Friend WithEvents PicBox As PictureBox
	Friend WithEvents TbTCinertia As TextBox
	Friend WithEvents Label1 As Label
	Friend WithEvents Label18 As Label
	Friend WithEvents ColumnHeader6 As ColumnHeader
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents tbUpshiftMinAcceleration As TextBox
	Friend WithEvents tbUpshiftAfterDownshift As TextBox
	Friend WithEvents tbDownshiftAfterUpshift As TextBox
	Friend WithEvents Label24 As Label
	Friend WithEvents Label23 As Label
	Friend WithEvents Label22 As Label
	Friend WithEvents Label21 As Label
	Friend WithEvents Label20 As Label
	Friend WithEvents Label19 As Label
	Friend WithEvents BtTCShiftFileBrowse As Button
	Friend WithEvents LblTCShiftFile As Label
	Friend WithEvents TBTCShiftPolygon As TextBox
End Class
