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
Imports System.Linq
Imports Microsoft.VisualBasic.CompilerServices
Imports TUGraz.VectoCommon.Models

<DesignerGenerated()> _
Partial Class F_VEH
	Inherits Form

	'Das Formular Ã¼berschreibt den LÃ¶schvorgang, um die Komponentenliste zu bereinigen.
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

	'Wird vom Windows Form-Designer benÃ¶tigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist fÃ¼r den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer mÃ¶glich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht mÃ¶glich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.components = New Container()
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(F_VEH))
		Me.Label1 = New Label()
		Me.TbMass = New TextBox()
		Me.Label2 = New Label()
		Me.TbLoad = New TextBox()
		Me.Label3 = New Label()
		Me.TBcdA = New TextBox()
		Me.Label13 = New Label()
		Me.TBrdyn = New TextBox()
		Me.ButOK = New Button()
		Me.ButCancel = New Button()
		Me.Label14 = New Label()
		Me.Label31 = New Label()
		Me.Label35 = New Label()
		Me.CbCdMode = New ComboBox()
		Me.TbCdFile = New TextBox()
		Me.BtCdFileBrowse = New Button()
		Me.GroupBox6 = New GroupBox()
		Me.BtCdFileOpen = New Button()
		Me.LbCdMode = New Label()
		Me.ToolStrip1 = New ToolStrip()
		Me.ToolStripBtNew = New ToolStripButton()
		Me.ToolStripBtOpen = New ToolStripButton()
		Me.ToolStripBtSave = New ToolStripButton()
		Me.ToolStripBtSaveAs = New ToolStripButton()
		Me.ToolStripSeparator3 = New ToolStripSeparator()
		Me.ToolStripBtSendTo = New ToolStripButton()
		Me.ToolStripSeparator1 = New ToolStripSeparator()
		Me.ToolStripButton1 = New ToolStripButton()
		Me.GroupBox7 = New GroupBox()
		Me.PnRt = New Panel()
		Me.Label15 = New Label()
		Me.BtRtBrowse = New Button()
		Me.TbRtPath = New TextBox()
		Me.Label45 = New Label()
		Me.LbRtRatio = New Label()
		Me.TbRtRatio = New TextBox()
		Me.CbRtType = New ComboBox()
		Me.Label46 = New Label()
		Me.Label50 = New Label()
		Me.TbMassExtra = New TextBox()
		Me.GroupBox8 = New GroupBox()
		Me.PnWheelDiam = New Panel()
		Me.Label6 = New Label()
		Me.ButAxlRem = New Button()
		Me.LvRRC = New ListView()
		Me.ColumnHeader7 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader8 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader2 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader9 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader1 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader3 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader4 = CType(New ColumnHeader(), ColumnHeader)
		Me.ButAxlAdd = New Button()
		Me.CbAxleConfig = New ComboBox()
		Me.CbCat = New ComboBox()
		Me.Label5 = New Label()
		Me.Label9 = New Label()
		Me.TbMassMass = New TextBox()
		Me.StatusStrip1 = New StatusStrip()
		Me.LbStatus = New ToolStripStatusLabel()
		Me.TbHDVclass = New TextBox()
		Me.Label11 = New Label()
		Me.TbLoadingMax = New TextBox()
		Me.Label22 = New Label()
		Me.GroupBox1 = New GroupBox()
		Me.PnLoad = New Panel()
		Me.GrAirRes = New GroupBox()
		Me.PnCdATrTr = New Panel()
		Me.Label38 = New Label()
		Me.PictureBox1 = New PictureBox()
		Me.CmOpenFile = New ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New ToolStripMenuItem()
		Me.PnAll = New Panel()
		Me.GroupBox2 = New GroupBox()
		Me.pnAngularGearFields = New Panel()
		Me.Label4 = New Label()
		Me.Label12 = New Label()
		Me.Label10 = New Label()
		Me.tbAngularGearRatio = New TextBox()
		Me.btAngularGearLossMapBrowse = New Button()
		Me.tbAngularGearLossMapPath = New TextBox()
		Me.cbAngularGearType = New ComboBox()
		Me.PicVehicle = New PictureBox()
		Me.Label8 = New Label()
		Me.ToolTip1 = New ToolTip(Me.components)
		Me.GroupBox3 = New GroupBox()
		Me.GroupBox6.SuspendLayout()
		Me.ToolStrip1.SuspendLayout()
		Me.GroupBox7.SuspendLayout()
		Me.PnRt.SuspendLayout()
		Me.GroupBox8.SuspendLayout()
		Me.PnWheelDiam.SuspendLayout()
		Me.StatusStrip1.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me.PnLoad.SuspendLayout()
		Me.GrAirRes.SuspendLayout()
		Me.PnCdATrTr.SuspendLayout()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		Me.CmOpenFile.SuspendLayout()
		Me.PnAll.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		Me.pnAngularGearFields.SuspendLayout()
		CType(Me.PicVehicle, ISupportInitialize).BeginInit()
		Me.GroupBox3.SuspendLayout()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(62, 22)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(104, 13)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Curb Weight Vehicle"
		'
		'TbMass
		'
		Me.TbMass.Location = New Point(172, 19)
		Me.TbMass.Name = "TbMass"
		Me.TbMass.Size = New Size(57, 20)
		Me.TbMass.TabIndex = 0
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(115, 31)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(45, 13)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Loading"
		'
		'TbLoad
		'
		Me.TbLoad.Location = New Point(166, 28)
		Me.TbLoad.Name = "TbLoad"
		Me.TbLoad.Size = New Size(57, 20)
		Me.TbLoad.TabIndex = 1
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(3, 6)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(37, 13)
		Me.Label3.TabIndex = 8
		Me.Label3.Text = "cd x A"
		'
		'TBcdA
		'
		Me.TBcdA.Location = New Point(46, 3)
		Me.TBcdA.Name = "TBcdA"
		Me.TBcdA.Size = New Size(57, 20)
		Me.TBcdA.TabIndex = 0
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New Point(3, 7)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New Size(40, 13)
		Me.Label13.TabIndex = 6
		Me.Label13.Text = "Radius"
		'
		'TBrdyn
		'
		Me.TBrdyn.Location = New Point(46, 4)
		Me.TBrdyn.Name = "TBrdyn"
		Me.TBrdyn.Size = New Size(57, 20)
		Me.TBrdyn.TabIndex = 0
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButOK.Location = New Point(421, 661)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New Size(75, 23)
		Me.ButOK.TabIndex = 5
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButCancel.DialogResult = DialogResult.Cancel
		Me.ButCancel.Location = New Point(502, 661)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New Size(75, 23)
		Me.ButCancel.TabIndex = 6
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New Point(231, 22)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New Size(25, 13)
		Me.Label14.TabIndex = 24
		Me.Label14.Text = "[kg]"
		'
		'Label31
		'
		Me.Label31.AutoSize = True
		Me.Label31.Location = New Point(225, 31)
		Me.Label31.Name = "Label31"
		Me.Label31.Size = New Size(25, 13)
		Me.Label31.TabIndex = 24
		Me.Label31.Text = "[kg]"
		'
		'Label35
		'
		Me.Label35.AutoSize = True
		Me.Label35.Location = New Point(105, 7)
		Me.Label35.Name = "Label35"
		Me.Label35.Size = New Size(29, 13)
		Me.Label35.TabIndex = 24
		Me.Label35.Text = "[mm]"
		'
		'CbCdMode
		'
		Me.CbCdMode.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbCdMode.FormattingEnabled = True
		Me.CbCdMode.Location = New Point(6, 19)
		Me.CbCdMode.Name = "CbCdMode"
		Me.CbCdMode.Size = New Size(207, 21)
		Me.CbCdMode.TabIndex = 0
		'
		'TbCdFile
		'
		Me.TbCdFile.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbCdFile.Enabled = False
		Me.TbCdFile.Location = New Point(6, 46)
		Me.TbCdFile.Name = "TbCdFile"
		Me.TbCdFile.Size = New Size(504, 20)
		Me.TbCdFile.TabIndex = 1
		'
		'BtCdFileBrowse
		'
		Me.BtCdFileBrowse.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.BtCdFileBrowse.Enabled = False
		Me.BtCdFileBrowse.Image = My.Resources.Resources.Open_icon
		Me.BtCdFileBrowse.Location = New Point(510, 43)
		Me.BtCdFileBrowse.Name = "BtCdFileBrowse"
		Me.BtCdFileBrowse.Size = New Size(24, 24)
		Me.BtCdFileBrowse.TabIndex = 2
		Me.BtCdFileBrowse.UseVisualStyleBackColor = True
		'
		'GroupBox6
		'
		Me.GroupBox6.Controls.Add(Me.BtCdFileOpen)
		Me.GroupBox6.Controls.Add(Me.LbCdMode)
		Me.GroupBox6.Controls.Add(Me.CbCdMode)
		Me.GroupBox6.Controls.Add(Me.BtCdFileBrowse)
		Me.GroupBox6.Controls.Add(Me.TbCdFile)
		Me.GroupBox6.Location = New Point(6, 407)
		Me.GroupBox6.Name = "GroupBox6"
		Me.GroupBox6.Size = New Size(562, 74)
		Me.GroupBox6.TabIndex = 5
		Me.GroupBox6.TabStop = False
		Me.GroupBox6.Text = "Cross Wind Correction"
		'
		'BtCdFileOpen
		'
		Me.BtCdFileOpen.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.BtCdFileOpen.Enabled = False
		Me.BtCdFileOpen.Image = My.Resources.Resources.application_export_icon_small
		Me.BtCdFileOpen.Location = New Point(533, 43)
		Me.BtCdFileOpen.Name = "BtCdFileOpen"
		Me.BtCdFileOpen.Size = New Size(24, 24)
		Me.BtCdFileOpen.TabIndex = 3
		Me.BtCdFileOpen.TabStop = False
		Me.BtCdFileOpen.UseVisualStyleBackColor = True
		'
		'LbCdMode
		'
		Me.LbCdMode.AutoSize = True
		Me.LbCdMode.Location = New Point(219, 22)
		Me.LbCdMode.Name = "LbCdMode"
		Me.LbCdMode.Size = New Size(59, 13)
		Me.LbCdMode.TabIndex = 28
		Me.LbCdMode.Text = "LbCdMode"
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New Size(589, 25)
		Me.ToolStrip1.TabIndex = 29
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
		'GroupBox7
		'
		Me.GroupBox7.Controls.Add(Me.PnRt)
		Me.GroupBox7.Controls.Add(Me.CbRtType)
		Me.GroupBox7.Location = New Point(6, 290)
		Me.GroupBox7.Name = "GroupBox7"
		Me.GroupBox7.Size = New Size(278, 111)
		Me.GroupBox7.TabIndex = 3
		Me.GroupBox7.TabStop = False
		Me.GroupBox7.Text = "Retarder Losses"
		'
		'PnRt
		'
		Me.PnRt.Controls.Add(Me.Label15)
		Me.PnRt.Controls.Add(Me.BtRtBrowse)
		Me.PnRt.Controls.Add(Me.TbRtPath)
		Me.PnRt.Controls.Add(Me.Label45)
		Me.PnRt.Controls.Add(Me.LbRtRatio)
		Me.PnRt.Controls.Add(Me.TbRtRatio)
		Me.PnRt.Location = New Point(3, 42)
		Me.PnRt.Name = "PnRt"
		Me.PnRt.Size = New Size(272, 63)
		Me.PnRt.TabIndex = 1
		'
		'Label15
		'
		Me.Label15.Location = New Point(6, 23)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New Size(201, 16)
		Me.Label15.TabIndex = 15
		Me.Label15.Text = "Retarder Loss Map"
		Me.Label15.TextAlign = ContentAlignment.BottomLeft
		'
		'BtRtBrowse
		'
		Me.BtRtBrowse.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtRtBrowse.Image = My.Resources.Resources.Open_icon
		Me.BtRtBrowse.Location = New Point(245, 39)
		Me.BtRtBrowse.Name = "BtRtBrowse"
		Me.BtRtBrowse.Size = New Size(24, 24)
		Me.BtRtBrowse.TabIndex = 14
		Me.BtRtBrowse.UseVisualStyleBackColor = True
		'
		'TbRtPath
		'
		Me.TbRtPath.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.TbRtPath.Location = New Point(6, 41)
		Me.TbRtPath.Name = "TbRtPath"
		Me.TbRtPath.Size = New Size(239, 20)
		Me.TbRtPath.TabIndex = 13
		'
		'Label45
		'
		Me.Label45.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label45.AutoSize = True
		Me.Label45.Location = New Point(251, 4)
		Me.Label45.Name = "Label45"
		Me.Label45.Size = New Size(16, 13)
		Me.Label45.TabIndex = 10
		Me.Label45.Text = "[-]"
		'
		'LbRtRatio
		'
		Me.LbRtRatio.Location = New Point(19, 4)
		Me.LbRtRatio.Name = "LbRtRatio"
		Me.LbRtRatio.Size = New Size(167, 17)
		Me.LbRtRatio.TabIndex = 5
		Me.LbRtRatio.Text = "Ratio"
		Me.LbRtRatio.TextAlign = ContentAlignment.TopRight
		'
		'TbRtRatio
		'
		Me.TbRtRatio.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.TbRtRatio.Location = New Point(193, 2)
		Me.TbRtRatio.Name = "TbRtRatio"
		Me.TbRtRatio.Size = New Size(56, 20)
		Me.TbRtRatio.TabIndex = 0
		'
		'CbRtType
		'
		Me.CbRtType.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbRtType.FormattingEnabled = True
		
		Me.CbRtType.Location = New Point(6, 19)
		Me.CbRtType.Name = "CbRtType"
		Me.CbRtType.Size = New Size(266, 21)
		Me.CbRtType.TabIndex = 0
		'
		'Label46
		'
		Me.Label46.AutoSize = True
		Me.Label46.Location = New Point(6, 5)
		Me.Label46.Name = "Label46"
		Me.Label46.Size = New Size(154, 13)
		Me.Label46.TabIndex = 31
		Me.Label46.Text = "Curb Weight Extra Trailer/Body"
		'
		'Label50
		'
		Me.Label50.AutoSize = True
		Me.Label50.Location = New Point(225, 5)
		Me.Label50.Name = "Label50"
		Me.Label50.Size = New Size(25, 13)
		Me.Label50.TabIndex = 24
		Me.Label50.Text = "[kg]"
		'
		'TbMassExtra
		'
		Me.TbMassExtra.Location = New Point(166, 2)
		Me.TbMassExtra.Name = "TbMassExtra"
		Me.TbMassExtra.Size = New Size(57, 20)
		Me.TbMassExtra.TabIndex = 0
		'
		'GroupBox8
		'
		Me.GroupBox8.Controls.Add(Me.Label6)
		Me.GroupBox8.Controls.Add(Me.ButAxlRem)
		Me.GroupBox8.Controls.Add(Me.LvRRC)
		Me.GroupBox8.Controls.Add(Me.ButAxlAdd)
		Me.GroupBox8.Location = New Point(6, 133)
		Me.GroupBox8.Name = "GroupBox8"
		Me.GroupBox8.Size = New Size(562, 151)
		Me.GroupBox8.TabIndex = 2
		Me.GroupBox8.TabStop = False
		Me.GroupBox8.Text = "Axles / Wheels"
		'
		'PnWheelDiam
		'
		Me.PnWheelDiam.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.PnWheelDiam.Controls.Add(Me.Label13)
		Me.PnWheelDiam.Controls.Add(Me.TBrdyn)
		Me.PnWheelDiam.Controls.Add(Me.Label35)
		Me.PnWheelDiam.Location = New Point(3, 16)
		Me.PnWheelDiam.Name = "PnWheelDiam"
		Me.PnWheelDiam.Size = New Size(228, 34)
		Me.PnWheelDiam.TabIndex = 5
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New Point(450, 121)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New Size(106, 13)
		Me.Label6.TabIndex = 3
		Me.Label6.Text = "(Double-Click to Edit)"
		'
		'ButAxlRem
		'
		Me.ButAxlRem.Image = My.Resources.Resources.minus_circle_icon
		Me.ButAxlRem.Location = New Point(29, 122)
		Me.ButAxlRem.Name = "ButAxlRem"
		Me.ButAxlRem.Size = New Size(24, 24)
		Me.ButAxlRem.TabIndex = 2
		Me.ButAxlRem.UseVisualStyleBackColor = True
		'
		'LvRRC
		'
		Me.LvRRC.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.LvRRC.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader2, Me.ColumnHeader9, Me.ColumnHeader1, Me.ColumnHeader3, Me.ColumnHeader4})
		Me.LvRRC.FullRowSelect = True
		Me.LvRRC.GridLines = True
		Me.LvRRC.HideSelection = False
		Me.LvRRC.Location = New Point(6, 19)
		Me.LvRRC.MultiSelect = False
		Me.LvRRC.Name = "LvRRC"
		Me.LvRRC.Size = New Size(550, 102)
		Me.LvRRC.TabIndex = 0
		Me.LvRRC.TabStop = False
		Me.LvRRC.UseCompatibleStateImageBehavior = False
		Me.LvRRC.View = View.Details
		'
		'ColumnHeader7
		'
		Me.ColumnHeader7.Text = "#"
		Me.ColumnHeader7.Width = 22
		'
		'ColumnHeader8
		'
		Me.ColumnHeader8.Text = "Rel. load"
		Me.ColumnHeader8.Width = 62
		'
		'ColumnHeader2
		'
		Me.ColumnHeader2.Text = "Twin T."
		Me.ColumnHeader2.Width = 51
		'
		'ColumnHeader9
		'
		Me.ColumnHeader9.Text = "RRC"
		Me.ColumnHeader9.Width = 59
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Fz ISO"
		Me.ColumnHeader1.Width = 55
		'
		'ColumnHeader3
		'
		Me.ColumnHeader3.Text = "Wheels"
		Me.ColumnHeader3.Width = 181
		'
		'ColumnHeader4
		'
		Me.ColumnHeader4.Text = "Inertia"
		'
		'ButAxlAdd
		'
		Me.ButAxlAdd.Image = My.Resources.Resources.plus_circle_icon
		Me.ButAxlAdd.Location = New Point(5, 122)
		Me.ButAxlAdd.Name = "ButAxlAdd"
		Me.ButAxlAdd.Size = New Size(24, 24)
		Me.ButAxlAdd.TabIndex = 1
		Me.ButAxlAdd.UseVisualStyleBackColor = True
		'
		'CbAxleConfig
		'
		Me.CbAxleConfig.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbAxleConfig.FormattingEnabled = True
		
		Me.CbAxleConfig.Location = New Point(153, 80)
		Me.CbAxleConfig.Name = "CbAxleConfig"
		Me.CbAxleConfig.Size = New Size(60, 21)
		Me.CbAxleConfig.TabIndex = 1
		'
		'CbCat
		'
		Me.CbCat.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbCat.FormattingEnabled = True
		
		Me.CbCat.Location = New Point(12, 80)
		Me.CbCat.Name = "CbCat"
		Me.CbCat.Size = New Size(135, 21)
		Me.CbCat.TabIndex = 0
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(13, 110)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(134, 13)
		Me.Label5.TabIndex = 2
		Me.Label5.Text = "Gross Vehicle Mass Rating"
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New Point(197, 110)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New Size(16, 13)
		Me.Label9.TabIndex = 3
		Me.Label9.Text = "[t]"
		Me.Label9.TextAlign = ContentAlignment.MiddleLeft
		'
		'TbMassMass
		'
		Me.TbMassMass.Location = New Point(153, 107)
		Me.TbMassMass.Name = "TbMassMass"
		Me.TbMassMass.Size = New Size(42, 20)
		Me.TbMassMass.TabIndex = 2
		'
		'StatusStrip1
		'
		Me.StatusStrip1.Items.AddRange(New ToolStripItem() {Me.LbStatus})
		Me.StatusStrip1.Location = New Point(0, 687)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New Size(589, 22)
		Me.StatusStrip1.SizingGrip = False
		Me.StatusStrip1.TabIndex = 36
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'LbStatus
		'
		Me.LbStatus.Name = "LbStatus"
		Me.LbStatus.Size = New Size(39, 17)
		Me.LbStatus.Text = "Status"
		'
		'TbHDVclass
		'
		Me.TbHDVclass.Location = New Point(153, 133)
		Me.TbHDVclass.Name = "TbHDVclass"
		Me.TbHDVclass.ReadOnly = True
		Me.TbHDVclass.Size = New Size(42, 20)
		Me.TbHDVclass.TabIndex = 3
		Me.TbHDVclass.TabStop = False
		Me.TbHDVclass.TextAlign = HorizontalAlignment.Center
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New Point(89, 57)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New Size(71, 13)
		Me.Label11.TabIndex = 31
		Me.Label11.Text = "Max. Loading"
		'
		'TbLoadingMax
		'
		Me.TbLoadingMax.Location = New Point(166, 54)
		Me.TbLoadingMax.Name = "TbLoadingMax"
		Me.TbLoadingMax.ReadOnly = True
		Me.TbLoadingMax.Size = New Size(57, 20)
		Me.TbLoadingMax.TabIndex = 2
		Me.TbLoadingMax.TabStop = False
		'
		'Label22
		'
		Me.Label22.AutoSize = True
		Me.Label22.Location = New Point(225, 57)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New Size(25, 13)
		Me.Label22.TabIndex = 24
		Me.Label22.Text = "[kg]"
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.PnLoad)
		Me.GroupBox1.Controls.Add(Me.TbMass)
		Me.GroupBox1.Controls.Add(Me.Label1)
		Me.GroupBox1.Controls.Add(Me.Label14)
		Me.GroupBox1.Location = New Point(6, 3)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(278, 124)
		Me.GroupBox1.TabIndex = 0
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Weight / Loading"
		'
		'PnLoad
		'
		Me.PnLoad.Controls.Add(Me.Label2)
		Me.PnLoad.Controls.Add(Me.Label31)
		Me.PnLoad.Controls.Add(Me.TbLoad)
		Me.PnLoad.Controls.Add(Me.TbMassExtra)
		Me.PnLoad.Controls.Add(Me.TbLoadingMax)
		Me.PnLoad.Controls.Add(Me.Label50)
		Me.PnLoad.Controls.Add(Me.Label46)
		Me.PnLoad.Controls.Add(Me.Label22)
		Me.PnLoad.Controls.Add(Me.Label11)
		Me.PnLoad.Location = New Point(6, 43)
		Me.PnLoad.Name = "PnLoad"
		Me.PnLoad.Size = New Size(256, 75)
		Me.PnLoad.TabIndex = 1
		'
		'GrAirRes
		'
		Me.GrAirRes.Controls.Add(Me.PnCdATrTr)
		Me.GrAirRes.Location = New Point(290, 3)
		Me.GrAirRes.Name = "GrAirRes"
		Me.GrAirRes.Size = New Size(278, 48)
		Me.GrAirRes.TabIndex = 1
		Me.GrAirRes.TabStop = False
		Me.GrAirRes.Text = "Air Resistance"
		'
		'PnCdATrTr
		'
		Me.PnCdATrTr.Controls.Add(Me.TBcdA)
		Me.PnCdATrTr.Controls.Add(Me.Label38)
		Me.PnCdATrTr.Controls.Add(Me.Label3)
		Me.PnCdATrTr.Dock = DockStyle.Fill
		Me.PnCdATrTr.Location = New Point(3, 16)
		Me.PnCdATrTr.Name = "PnCdATrTr"
		Me.PnCdATrTr.Size = New Size(272, 29)
		Me.PnCdATrTr.TabIndex = 0
		'
		'Label38
		'
		Me.Label38.AutoSize = True
		Me.Label38.Location = New Point(105, 6)
		Me.Label38.Name = "Label38"
		Me.Label38.Size = New Size(24, 13)
		Me.Label38.TabIndex = 24
		Me.Label38.Text = "[m²]"
		'
		'PictureBox1
		'
		Me.PictureBox1.BackColor = Color.White
		Me.PictureBox1.Image = My.Resources.Resources.VECTO_VEH
		Me.PictureBox1.Location = New Point(12, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(569, 40)
		Me.PictureBox1.TabIndex = 37
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
		'PnAll
		'
		Me.PnAll.Controls.Add(Me.GroupBox3)
		Me.PnAll.Controls.Add(Me.GroupBox2)
		Me.PnAll.Controls.Add(Me.GrAirRes)
		Me.PnAll.Controls.Add(Me.GroupBox1)
		Me.PnAll.Controls.Add(Me.GroupBox8)
		Me.PnAll.Controls.Add(Me.GroupBox7)
		Me.PnAll.Controls.Add(Me.GroupBox6)
		Me.PnAll.Location = New Point(6, 172)
		Me.PnAll.Name = "PnAll"
		Me.PnAll.Size = New Size(575, 485)
		Me.PnAll.TabIndex = 4
		'
		'GroupBox2
		'
		Me.GroupBox2.Controls.Add(Me.pnAngularGearFields)
		Me.GroupBox2.Controls.Add(Me.cbAngularGearType)
		Me.GroupBox2.Location = New Point(290, 290)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New Size(278, 111)
		Me.GroupBox2.TabIndex = 4
		Me.GroupBox2.TabStop = False
		Me.GroupBox2.Text = "Angular Gear"
		'
		'pnAngularGearFields
		'
		Me.pnAngularGearFields.Controls.Add(Me.Label4)
		Me.pnAngularGearFields.Controls.Add(Me.Label12)
		Me.pnAngularGearFields.Controls.Add(Me.Label10)
		Me.pnAngularGearFields.Controls.Add(Me.tbAngularGearRatio)
		Me.pnAngularGearFields.Controls.Add(Me.btAngularGearLossMapBrowse)
		Me.pnAngularGearFields.Controls.Add(Me.tbAngularGearLossMapPath)
		Me.pnAngularGearFields.Location = New Point(3, 42)
		Me.pnAngularGearFields.Name = "pnAngularGearFields"
		Me.pnAngularGearFields.Size = New Size(272, 63)
		Me.pnAngularGearFields.TabIndex = 6
		'
		'Label4
		'
		Me.Label4.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(251, 4)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(16, 13)
		Me.Label4.TabIndex = 16
		Me.Label4.Text = "[-]"
		'
		'Label12
		'
		Me.Label12.Location = New Point(6, 24)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New Size(263, 16)
		Me.Label12.TabIndex = 17
		Me.Label12.Text = "Transmission Loss Map or Efficiency Value [0..1]"
		Me.Label12.TextAlign = ContentAlignment.BottomLeft
		'
		'Label10
		'
		Me.Label10.Location = New Point(144, 4)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New Size(44, 18)
		Me.Label10.TabIndex = 15
		Me.Label10.Text = "Ratio"
		Me.Label10.TextAlign = ContentAlignment.TopRight
		'
		'tbAngularGearRatio
		'
		Me.tbAngularGearRatio.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.tbAngularGearRatio.Location = New Point(193, 2)
		Me.tbAngularGearRatio.Name = "tbAngularGearRatio"
		Me.tbAngularGearRatio.Size = New Size(56, 20)
		Me.tbAngularGearRatio.TabIndex = 12
		'
		'btAngularGearLossMapBrowse
		'
		Me.btAngularGearLossMapBrowse.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.btAngularGearLossMapBrowse.Image = My.Resources.Resources.Open_icon
		Me.btAngularGearLossMapBrowse.Location = New Point(244, 39)
		Me.btAngularGearLossMapBrowse.Name = "btAngularGearLossMapBrowse"
		Me.btAngularGearLossMapBrowse.Size = New Size(24, 24)
		Me.btAngularGearLossMapBrowse.TabIndex = 14
		Me.btAngularGearLossMapBrowse.UseVisualStyleBackColor = True
		'
		'tbAngularGearLossMapPath
		'
		Me.tbAngularGearLossMapPath.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.tbAngularGearLossMapPath.Location = New Point(6, 41)
		Me.tbAngularGearLossMapPath.Name = "tbAngularGearLossMapPath"
		Me.tbAngularGearLossMapPath.Size = New Size(238, 20)
		Me.tbAngularGearLossMapPath.TabIndex = 13
		'
		'cbAngularGearType
		'
		Me.cbAngularGearType.DropDownStyle = ComboBoxStyle.DropDownList
		Me.cbAngularGearType.FormattingEnabled = True
		
		Me.cbAngularGearType.Location = New Point(6, 19)
		Me.cbAngularGearType.Name = "cbAngularGearType"
		Me.cbAngularGearType.Size = New Size(266, 21)
		Me.cbAngularGearType.TabIndex = 0
		'
		'PicVehicle
		'
		Me.PicVehicle.BackColor = Color.LightGray
		Me.PicVehicle.Location = New Point(281, 74)
		Me.PicVehicle.Name = "PicVehicle"
		Me.PicVehicle.Size = New Size(300, 88)
		Me.PicVehicle.SizeMode = PictureBoxSizeMode.StretchImage
		Me.PicVehicle.TabIndex = 39
		Me.PicVehicle.TabStop = False
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New Point(89, 136)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New Size(58, 13)
		Me.Label8.TabIndex = 10
		Me.Label8.Text = "HDV Class"
		'
		'GroupBox3
		'
		Me.GroupBox3.Controls.Add(Me.PnWheelDiam)
		Me.GroupBox3.Location = New Point(290, 54)
		Me.GroupBox3.Name = "GroupBox3"
		Me.GroupBox3.Size = New Size(278, 73)
		Me.GroupBox3.TabIndex = 6
		Me.GroupBox3.TabStop = False
		Me.GroupBox3.Text = "Dynamic Tire Radius"
		'
		'F_VEH
		'
		Me.AcceptButton = Me.ButOK
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New Size(589, 709)
		Me.Controls.Add(Me.Label8)
		Me.Controls.Add(Me.TbHDVclass)
		Me.Controls.Add(Me.PicVehicle)
		Me.Controls.Add(Me.PnAll)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.Label9)
		Me.Controls.Add(Me.StatusStrip1)
		Me.Controls.Add(Me.CbAxleConfig)
		Me.Controls.Add(Me.TbMassMass)
		Me.Controls.Add(Me.CbCat)
		Me.Controls.Add(Me.Label5)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Controls.Add(Me.ButCancel)
		Me.Controls.Add(Me.ButOK)
		Me.FormBorderStyle = FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Me.MaximizeBox = False
		Me.Name = "F_VEH"
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "F05_VEH"
		Me.GroupBox6.ResumeLayout(False)
		Me.GroupBox6.PerformLayout()
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		Me.GroupBox7.ResumeLayout(False)
		Me.PnRt.ResumeLayout(False)
		Me.PnRt.PerformLayout()
		Me.GroupBox8.ResumeLayout(False)
		Me.GroupBox8.PerformLayout()
		Me.PnWheelDiam.ResumeLayout(False)
		Me.PnWheelDiam.PerformLayout()
		Me.StatusStrip1.ResumeLayout(False)
		Me.StatusStrip1.PerformLayout()
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.PnLoad.ResumeLayout(False)
		Me.PnLoad.PerformLayout()
		Me.GrAirRes.ResumeLayout(False)
		Me.PnCdATrTr.ResumeLayout(False)
		Me.PnCdATrTr.PerformLayout()
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		Me.CmOpenFile.ResumeLayout(False)
		Me.PnAll.ResumeLayout(False)
		Me.GroupBox2.ResumeLayout(False)
		Me.pnAngularGearFields.ResumeLayout(False)
		Me.pnAngularGearFields.PerformLayout()
		CType(Me.PicVehicle, ISupportInitialize).EndInit()
		Me.GroupBox3.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As Label
	Friend WithEvents TbMass As TextBox
	Friend WithEvents Label2 As Label
	Friend WithEvents TbLoad As TextBox
	Friend WithEvents Label3 As Label
	Friend WithEvents TBcdA As TextBox
	Friend WithEvents Label13 As Label
	Friend WithEvents TBrdyn As TextBox
	Friend WithEvents ButOK As Button
	Friend WithEvents ButCancel As Button
	Friend WithEvents Label14 As Label
	Friend WithEvents Label31 As Label
	Friend WithEvents Label35 As Label
	Friend WithEvents CbCdMode As ComboBox
	Friend WithEvents TbCdFile As TextBox
	Friend WithEvents BtCdFileBrowse As Button
	Friend WithEvents GroupBox6 As GroupBox
	Friend WithEvents LbCdMode As Label
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents GroupBox7 As GroupBox
	Friend WithEvents LbRtRatio As Label
	Friend WithEvents TbRtRatio As TextBox
	Friend WithEvents CbRtType As ComboBox
	Friend WithEvents Label45 As Label
	Friend WithEvents PnRt As Panel
	Friend WithEvents Label46 As Label
	Friend WithEvents Label50 As Label
	Friend WithEvents TbMassExtra As TextBox
	Friend WithEvents GroupBox8 As GroupBox
	Friend WithEvents ButAxlRem As Button
	Friend WithEvents LvRRC As ListView
	Friend WithEvents ColumnHeader7 As ColumnHeader
	Friend WithEvents ColumnHeader8 As ColumnHeader
	Friend WithEvents ButAxlAdd As Button
	Friend WithEvents CbCat As ComboBox
	Friend WithEvents Label5 As Label
	Friend WithEvents Label9 As Label
	Friend WithEvents TbMassMass As TextBox
	Friend WithEvents ColumnHeader9 As ColumnHeader
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents LbStatus As ToolStripStatusLabel
	Friend WithEvents CbAxleConfig As ComboBox
	Friend WithEvents TbHDVclass As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents TbLoadingMax As TextBox
	Friend WithEvents Label22 As Label
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents GrAirRes As GroupBox
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BtCdFileOpen As Button
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents ColumnHeader2 As ColumnHeader
	Friend WithEvents PnLoad As Panel
	Friend WithEvents PnAll As Panel
	Friend WithEvents Label6 As Label
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents PnWheelDiam As Panel
	Friend WithEvents PicVehicle As PictureBox
	Friend WithEvents Label8 As Label
	Friend WithEvents PnCdATrTr As Panel
	Friend WithEvents Label38 As Label
	Friend WithEvents GroupBox2 As GroupBox
	Friend WithEvents cbAngularGearType As ComboBox
	Friend WithEvents Label15 As Label
	Friend WithEvents BtRtBrowse As Button
	Friend WithEvents TbRtPath As TextBox
	Friend WithEvents pnAngularGearFields As Panel
	Friend WithEvents Label4 As Label
	Friend WithEvents Label12 As Label
	Friend WithEvents Label10 As Label
	Friend WithEvents tbAngularGearRatio As TextBox
	Friend WithEvents btAngularGearLossMapBrowse As Button
	Friend WithEvents tbAngularGearLossMapPath As TextBox
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents GroupBox3 As GroupBox
End Class
