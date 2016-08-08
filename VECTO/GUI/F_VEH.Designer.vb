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
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class F_VEH
    Inherits System.Windows.Forms.Form

    'Das Formular Ã¼berschreibt den LÃ¶schvorgang, um die Komponentenliste zu bereinigen.
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

    'Wird vom Windows Form-Designer benÃ¶tigt.
    Private components As System.ComponentModel.IContainer

    'Hinweis: Die folgende Prozedur ist fÃ¼r den Windows Form-Designer erforderlich.
    'Das Bearbeiten ist mit dem Windows Form-Designer mÃ¶glich.  
    'Das Bearbeiten mit dem Code-Editor ist nicht mÃ¶glich.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(F_VEH))
		Me.Label1 = New System.Windows.Forms.Label()
		Me.TbMass = New System.Windows.Forms.TextBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.TbLoad = New System.Windows.Forms.TextBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.TBcdA = New System.Windows.Forms.TextBox()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.TBrdyn = New System.Windows.Forms.TextBox()
		Me.ButOK = New System.Windows.Forms.Button()
		Me.ButCancel = New System.Windows.Forms.Button()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.Label31 = New System.Windows.Forms.Label()
		Me.Label35 = New System.Windows.Forms.Label()
		Me.CbCdMode = New System.Windows.Forms.ComboBox()
		Me.TbCdFile = New System.Windows.Forms.TextBox()
		Me.BtCdFileBrowse = New System.Windows.Forms.Button()
		Me.GroupBox6 = New System.Windows.Forms.GroupBox()
		Me.BtCdFileOpen = New System.Windows.Forms.Button()
		Me.LbCdMode = New System.Windows.Forms.Label()
		Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
		Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtSave = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtSaveAs = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripBtSendTo = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
		Me.GroupBox7 = New System.Windows.Forms.GroupBox()
		Me.PnRt = New System.Windows.Forms.Panel()
		Me.Label15 = New System.Windows.Forms.Label()
		Me.BtRtBrowse = New System.Windows.Forms.Button()
		Me.TbRtPath = New System.Windows.Forms.TextBox()
		Me.Label45 = New System.Windows.Forms.Label()
		Me.LbRtRatio = New System.Windows.Forms.Label()
		Me.TbRtRatio = New System.Windows.Forms.TextBox()
		Me.CbRtType = New System.Windows.Forms.ComboBox()
		Me.Label46 = New System.Windows.Forms.Label()
		Me.Label50 = New System.Windows.Forms.Label()
		Me.TbMassExtra = New System.Windows.Forms.TextBox()
		Me.GroupBox8 = New System.Windows.Forms.GroupBox()
		Me.PnWheelDiam = New System.Windows.Forms.Panel()
		Me.CbRim = New System.Windows.Forms.ComboBox()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.ButAxlRem = New System.Windows.Forms.Button()
		Me.LvRRC = New System.Windows.Forms.ListView()
		Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ButAxlAdd = New System.Windows.Forms.Button()
		Me.CbAxleConfig = New System.Windows.Forms.ComboBox()
		Me.CbCat = New System.Windows.Forms.ComboBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.TbMassMass = New System.Windows.Forms.TextBox()
		Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
		Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
		Me.TbHDVclass = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.TbLoadingMax = New System.Windows.Forms.TextBox()
		Me.Label22 = New System.Windows.Forms.Label()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.PnLoad = New System.Windows.Forms.Panel()
		Me.GrAirRes = New System.Windows.Forms.GroupBox()
		Me.PnCdATrTr = New System.Windows.Forms.Panel()
		Me.Label38 = New System.Windows.Forms.Label()
		Me.PictureBox1 = New System.Windows.Forms.PictureBox()
		Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.PnAll = New System.Windows.Forms.Panel()
		Me.GroupBox2 = New System.Windows.Forms.GroupBox()
		Me.pnAngularGearFields = New System.Windows.Forms.Panel()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.tbAngularGearRatio = New System.Windows.Forms.TextBox()
		Me.btAngularGearLossMapBrowse = New System.Windows.Forms.Button()
		Me.tbAngularGearLossMapPath = New System.Windows.Forms.TextBox()
		Me.cbAngularGearType = New System.Windows.Forms.ComboBox()
		Me.PicVehicle = New System.Windows.Forms.PictureBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
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
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.CmOpenFile.SuspendLayout()
		Me.PnAll.SuspendLayout()
		Me.GroupBox2.SuspendLayout()
		Me.pnAngularGearFields.SuspendLayout()
		CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(62, 22)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(104, 13)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Curb Weight Vehicle"
		'
		'TbMass
		'
		Me.TbMass.Location = New System.Drawing.Point(172, 19)
		Me.TbMass.Name = "TbMass"
		Me.TbMass.Size = New System.Drawing.Size(57, 20)
		Me.TbMass.TabIndex = 0
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(115, 31)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(45, 13)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Loading"
		'
		'TbLoad
		'
		Me.TbLoad.Location = New System.Drawing.Point(166, 28)
		Me.TbLoad.Name = "TbLoad"
		Me.TbLoad.Size = New System.Drawing.Size(57, 20)
		Me.TbLoad.TabIndex = 1
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(3, 6)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(37, 13)
		Me.Label3.TabIndex = 8
		Me.Label3.Text = "cd x A"
		'
		'TBcdA
		'
		Me.TBcdA.Location = New System.Drawing.Point(46, 3)
		Me.TBcdA.Name = "TBcdA"
		Me.TBcdA.Size = New System.Drawing.Size(57, 20)
		Me.TBcdA.TabIndex = 0
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New System.Drawing.Point(7, 7)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(96, 13)
		Me.Label13.TabIndex = 6
		Me.Label13.Text = "Dynamic tire radius"
		'
		'TBrdyn
		'
		Me.TBrdyn.Location = New System.Drawing.Point(109, 4)
		Me.TBrdyn.Name = "TBrdyn"
		Me.TBrdyn.Size = New System.Drawing.Size(57, 20)
		Me.TBrdyn.TabIndex = 0
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButOK.Location = New System.Drawing.Point(421, 697)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New System.Drawing.Size(75, 23)
		Me.ButOK.TabIndex = 5
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.ButCancel.Location = New System.Drawing.Point(502, 697)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New System.Drawing.Size(75, 23)
		Me.ButCancel.TabIndex = 6
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New System.Drawing.Point(231, 22)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(25, 13)
		Me.Label14.TabIndex = 24
		Me.Label14.Text = "[kg]"
		'
		'Label31
		'
		Me.Label31.AutoSize = True
		Me.Label31.Location = New System.Drawing.Point(225, 31)
		Me.Label31.Name = "Label31"
		Me.Label31.Size = New System.Drawing.Size(25, 13)
		Me.Label31.TabIndex = 24
		Me.Label31.Text = "[kg]"
		'
		'Label35
		'
		Me.Label35.AutoSize = True
		Me.Label35.Location = New System.Drawing.Point(168, 7)
		Me.Label35.Name = "Label35"
		Me.Label35.Size = New System.Drawing.Size(29, 13)
		Me.Label35.TabIndex = 24
		Me.Label35.Text = "[mm]"
		'
		'CbCdMode
		'
		Me.CbCdMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.CbCdMode.FormattingEnabled = True
		Me.CbCdMode.Items.AddRange(New Object() {"No Correction", "Speed dependent (User-defined)", "Speed dependent (Declaration Mode)", "Vair & Beta Input"})
		Me.CbCdMode.Location = New System.Drawing.Point(6, 19)
		Me.CbCdMode.Name = "CbCdMode"
		Me.CbCdMode.Size = New System.Drawing.Size(207, 21)
		Me.CbCdMode.TabIndex = 0
		'
		'TbCdFile
		'
		Me.TbCdFile.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbCdFile.Enabled = False
		Me.TbCdFile.Location = New System.Drawing.Point(6, 46)
		Me.TbCdFile.Name = "TbCdFile"
		Me.TbCdFile.Size = New System.Drawing.Size(504, 20)
		Me.TbCdFile.TabIndex = 1
		'
		'BtCdFileBrowse
		'
		Me.BtCdFileBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtCdFileBrowse.Enabled = False
		Me.BtCdFileBrowse.Image = Global.VECTO.My.Resources.Resources.Open_icon
		Me.BtCdFileBrowse.Location = New System.Drawing.Point(510, 43)
		Me.BtCdFileBrowse.Name = "BtCdFileBrowse"
		Me.BtCdFileBrowse.Size = New System.Drawing.Size(24, 24)
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
		Me.GroupBox6.Location = New System.Drawing.Point(6, 444)
		Me.GroupBox6.Name = "GroupBox6"
		Me.GroupBox6.Size = New System.Drawing.Size(562, 74)
		Me.GroupBox6.TabIndex = 5
		Me.GroupBox6.TabStop = False
		Me.GroupBox6.Text = "Cross Wind Correction"
		'
		'BtCdFileOpen
		'
		Me.BtCdFileOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtCdFileOpen.Enabled = False
		Me.BtCdFileOpen.Image = Global.VECTO.My.Resources.Resources.application_export_icon_small
		Me.BtCdFileOpen.Location = New System.Drawing.Point(533, 43)
		Me.BtCdFileOpen.Name = "BtCdFileOpen"
		Me.BtCdFileOpen.Size = New System.Drawing.Size(24, 24)
		Me.BtCdFileOpen.TabIndex = 3
		Me.BtCdFileOpen.TabStop = False
		Me.BtCdFileOpen.UseVisualStyleBackColor = True
		'
		'LbCdMode
		'
		Me.LbCdMode.AutoSize = True
		Me.LbCdMode.Location = New System.Drawing.Point(219, 22)
		Me.LbCdMode.Name = "LbCdMode"
		Me.LbCdMode.Size = New System.Drawing.Size(59, 13)
		Me.LbCdMode.TabIndex = 28
		Me.LbCdMode.Text = "LbCdMode"
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New System.Drawing.Size(589, 25)
		Me.ToolStrip1.TabIndex = 29
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtNew
		'
		Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtNew.Image = Global.VECTO.My.Resources.Resources.blue_document_icon
		Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtNew.Name = "ToolStripBtNew"
		Me.ToolStripBtNew.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtNew.Text = "ToolStripButton1"
		Me.ToolStripBtNew.ToolTipText = "New"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = Global.VECTO.My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtOpen.Text = "ToolStripButton1"
		Me.ToolStripBtOpen.ToolTipText = "Open..."
		'
		'ToolStripBtSave
		'
		Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSave.Image = Global.VECTO.My.Resources.Resources.Actions_document_save_icon
		Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSave.Name = "ToolStripBtSave"
		Me.ToolStripBtSave.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtSave.Text = "ToolStripButton1"
		Me.ToolStripBtSave.ToolTipText = "Save"
		'
		'ToolStripBtSaveAs
		'
		Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSaveAs.Image = Global.VECTO.My.Resources.Resources.Actions_document_save_as_icon
		Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
		Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtSaveAs.Text = "ToolStripButton1"
		Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
		'
		'ToolStripSeparator3
		'
		Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
		Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 25)
		'
		'ToolStripBtSendTo
		'
		Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSendTo.Image = Global.VECTO.My.Resources.Resources.export_icon
		Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
		Me.ToolStripBtSendTo.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtSendTo.Text = "Send to Job Editor"
		Me.ToolStripBtSendTo.ToolTipText = "Send to Job Editor"
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
		'
		'ToolStripButton1
		'
		Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripButton1.Image = Global.VECTO.My.Resources.Resources.Help_icon
		Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripButton1.Name = "ToolStripButton1"
		Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripButton1.Text = "Help"
		'
		'GroupBox7
		'
		Me.GroupBox7.Controls.Add(Me.PnRt)
		Me.GroupBox7.Controls.Add(Me.CbRtType)
		Me.GroupBox7.Location = New System.Drawing.Point(6, 327)
		Me.GroupBox7.Name = "GroupBox7"
		Me.GroupBox7.Size = New System.Drawing.Size(278, 111)
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
		Me.PnRt.Location = New System.Drawing.Point(3, 42)
		Me.PnRt.Name = "PnRt"
		Me.PnRt.Size = New System.Drawing.Size(272, 63)
		Me.PnRt.TabIndex = 1
		'
		'Label15
		'
		Me.Label15.Location = New System.Drawing.Point(6, 23)
		Me.Label15.Name = "Label15"
		Me.Label15.Size = New System.Drawing.Size(201, 16)
		Me.Label15.TabIndex = 15
		Me.Label15.Text = "Retarder Loss Map"
		Me.Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft
		'
		'BtRtBrowse
		'
		Me.BtRtBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.BtRtBrowse.Image = Global.VECTO.My.Resources.Resources.Open_icon
		Me.BtRtBrowse.Location = New System.Drawing.Point(245, 39)
		Me.BtRtBrowse.Name = "BtRtBrowse"
		Me.BtRtBrowse.Size = New System.Drawing.Size(24, 24)
		Me.BtRtBrowse.TabIndex = 14
		Me.BtRtBrowse.UseVisualStyleBackColor = True
		'
		'TbRtPath
		'
		Me.TbRtPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.TbRtPath.Location = New System.Drawing.Point(6, 41)
		Me.TbRtPath.Name = "TbRtPath"
		Me.TbRtPath.Size = New System.Drawing.Size(239, 20)
		Me.TbRtPath.TabIndex = 13
		'
		'Label45
		'
		Me.Label45.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label45.AutoSize = True
		Me.Label45.Location = New System.Drawing.Point(251, 4)
		Me.Label45.Name = "Label45"
		Me.Label45.Size = New System.Drawing.Size(16, 13)
		Me.Label45.TabIndex = 10
		Me.Label45.Text = "[-]"
		'
		'LbRtRatio
		'
		Me.LbRtRatio.Location = New System.Drawing.Point(19, 4)
		Me.LbRtRatio.Name = "LbRtRatio"
		Me.LbRtRatio.Size = New System.Drawing.Size(167, 17)
		Me.LbRtRatio.TabIndex = 5
		Me.LbRtRatio.Text = "Ratio"
		Me.LbRtRatio.TextAlign = System.Drawing.ContentAlignment.TopRight
		'
		'TbRtRatio
		'
		Me.TbRtRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbRtRatio.Location = New System.Drawing.Point(193, 2)
		Me.TbRtRatio.Name = "TbRtRatio"
		Me.TbRtRatio.Size = New System.Drawing.Size(56, 20)
		Me.TbRtRatio.TabIndex = 0
		'
		'CbRtType
		'
		Me.CbRtType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.CbRtType.FormattingEnabled = True
		Me.CbRtType.Items.AddRange(New Object() {"Included in Transmission Loss Maps", "Primary Retarder", "Secondary Retarder"})
		Me.CbRtType.Location = New System.Drawing.Point(6, 19)
		Me.CbRtType.Name = "CbRtType"
		Me.CbRtType.Size = New System.Drawing.Size(266, 21)
		Me.CbRtType.TabIndex = 0
		'
		'Label46
		'
		Me.Label46.AutoSize = True
		Me.Label46.Location = New System.Drawing.Point(6, 5)
		Me.Label46.Name = "Label46"
		Me.Label46.Size = New System.Drawing.Size(154, 13)
		Me.Label46.TabIndex = 31
		Me.Label46.Text = "Curb Weight Extra Trailer/Body"
		'
		'Label50
		'
		Me.Label50.AutoSize = True
		Me.Label50.Location = New System.Drawing.Point(225, 5)
		Me.Label50.Name = "Label50"
		Me.Label50.Size = New System.Drawing.Size(25, 13)
		Me.Label50.TabIndex = 24
		Me.Label50.Text = "[kg]"
		'
		'TbMassExtra
		'
		Me.TbMassExtra.Location = New System.Drawing.Point(166, 2)
		Me.TbMassExtra.Name = "TbMassExtra"
		Me.TbMassExtra.Size = New System.Drawing.Size(57, 20)
		Me.TbMassExtra.TabIndex = 0
		'
		'GroupBox8
		'
		Me.GroupBox8.Controls.Add(Me.PnWheelDiam)
		Me.GroupBox8.Controls.Add(Me.CbRim)
		Me.GroupBox8.Controls.Add(Me.Label7)
		Me.GroupBox8.Controls.Add(Me.Label6)
		Me.GroupBox8.Controls.Add(Me.ButAxlRem)
		Me.GroupBox8.Controls.Add(Me.LvRRC)
		Me.GroupBox8.Controls.Add(Me.ButAxlAdd)
		Me.GroupBox8.Location = New System.Drawing.Point(6, 133)
		Me.GroupBox8.Name = "GroupBox8"
		Me.GroupBox8.Size = New System.Drawing.Size(562, 190)
		Me.GroupBox8.TabIndex = 2
		Me.GroupBox8.TabStop = False
		Me.GroupBox8.Text = "Axles / Wheels"
		'
		'PnWheelDiam
		'
		Me.PnWheelDiam.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.PnWheelDiam.Controls.Add(Me.Label13)
		Me.PnWheelDiam.Controls.Add(Me.TBrdyn)
		Me.PnWheelDiam.Controls.Add(Me.Label35)
		Me.PnWheelDiam.Location = New System.Drawing.Point(321, 151)
		Me.PnWheelDiam.Name = "PnWheelDiam"
		Me.PnWheelDiam.Size = New System.Drawing.Size(228, 34)
		Me.PnWheelDiam.TabIndex = 5
		'
		'CbRim
		'
		Me.CbRim.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.CbRim.FormattingEnabled = True
		Me.CbRim.Location = New System.Drawing.Point(131, 155)
		Me.CbRim.Name = "CbRim"
		Me.CbRim.Size = New System.Drawing.Size(184, 21)
		Me.CbRim.TabIndex = 4
		Me.ToolTip1.SetToolTip(Me.CbRim, "(Vecto 2 only)" & Global.Microsoft.VisualBasic.ChrW(13) & Global.Microsoft.VisualBasic.ChrW(10) & "The Rims parameter will be removed in Vecto 3")
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(6, 157)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(119, 13)
		Me.Label7.TabIndex = 46
		Me.Label7.Text = "Powered axle tyres/rims"
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(450, 121)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(106, 13)
		Me.Label6.TabIndex = 3
		Me.Label6.Text = "(Double-Click to Edit)"
		'
		'ButAxlRem
		'
		Me.ButAxlRem.Image = Global.VECTO.My.Resources.Resources.minus_circle_icon
		Me.ButAxlRem.Location = New System.Drawing.Point(29, 122)
		Me.ButAxlRem.Name = "ButAxlRem"
		Me.ButAxlRem.Size = New System.Drawing.Size(24, 24)
		Me.ButAxlRem.TabIndex = 2
		Me.ButAxlRem.UseVisualStyleBackColor = True
		'
		'LvRRC
		'
		Me.LvRRC.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.LvRRC.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader2, Me.ColumnHeader9, Me.ColumnHeader1, Me.ColumnHeader3, Me.ColumnHeader4})
		Me.LvRRC.FullRowSelect = True
		Me.LvRRC.GridLines = True
		Me.LvRRC.HideSelection = False
		Me.LvRRC.Location = New System.Drawing.Point(6, 19)
		Me.LvRRC.MultiSelect = False
		Me.LvRRC.Name = "LvRRC"
		Me.LvRRC.Size = New System.Drawing.Size(550, 102)
		Me.LvRRC.TabIndex = 0
		Me.LvRRC.TabStop = False
		Me.LvRRC.UseCompatibleStateImageBehavior = False
		Me.LvRRC.View = System.Windows.Forms.View.Details
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
		Me.ButAxlAdd.Image = Global.VECTO.My.Resources.Resources.plus_circle_icon
		Me.ButAxlAdd.Location = New System.Drawing.Point(5, 122)
		Me.ButAxlAdd.Name = "ButAxlAdd"
		Me.ButAxlAdd.Size = New System.Drawing.Size(24, 24)
		Me.ButAxlAdd.TabIndex = 1
		Me.ButAxlAdd.UseVisualStyleBackColor = True
		'
		'CbAxleConfig
		'
		Me.CbAxleConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.CbAxleConfig.FormattingEnabled = True
		Me.CbAxleConfig.Items.AddRange(New Object() {"-", "4x2", "4x4", "6x2", "6x4", "6x6", "8x2", "8x4", "8x6", "8x8"})
		Me.CbAxleConfig.Location = New System.Drawing.Point(153, 80)
		Me.CbAxleConfig.Name = "CbAxleConfig"
		Me.CbAxleConfig.Size = New System.Drawing.Size(60, 21)
		Me.CbAxleConfig.TabIndex = 1
		'
		'CbCat
		'
		Me.CbCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.CbCat.FormattingEnabled = True
		Me.CbCat.Items.AddRange(New Object() {"-", "Rigid Truck", "Tractor", "City Bus", "Interurban Bus", "Coach"})
		Me.CbCat.Location = New System.Drawing.Point(12, 80)
		Me.CbCat.Name = "CbCat"
		Me.CbCat.Size = New System.Drawing.Size(135, 21)
		Me.CbCat.TabIndex = 0
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(13, 110)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(134, 13)
		Me.Label5.TabIndex = 2
		Me.Label5.Text = "Gross Vehicle Mass Rating"
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(197, 110)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(16, 13)
		Me.Label9.TabIndex = 3
		Me.Label9.Text = "[t]"
		Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
		'
		'TbMassMass
		'
		Me.TbMassMass.Location = New System.Drawing.Point(153, 107)
		Me.TbMassMass.Name = "TbMassMass"
		Me.TbMassMass.Size = New System.Drawing.Size(42, 20)
		Me.TbMassMass.TabIndex = 2
		'
		'StatusStrip1
		'
		Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
		Me.StatusStrip1.Location = New System.Drawing.Point(0, 723)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New System.Drawing.Size(589, 22)
		Me.StatusStrip1.SizingGrip = False
		Me.StatusStrip1.TabIndex = 36
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'LbStatus
		'
		Me.LbStatus.Name = "LbStatus"
		Me.LbStatus.Size = New System.Drawing.Size(39, 17)
		Me.LbStatus.Text = "Status"
		'
		'TbHDVclass
		'
		Me.TbHDVclass.Location = New System.Drawing.Point(153, 133)
		Me.TbHDVclass.Name = "TbHDVclass"
		Me.TbHDVclass.ReadOnly = True
		Me.TbHDVclass.Size = New System.Drawing.Size(42, 20)
		Me.TbHDVclass.TabIndex = 3
		Me.TbHDVclass.TabStop = False
		Me.TbHDVclass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(89, 57)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(71, 13)
		Me.Label11.TabIndex = 31
		Me.Label11.Text = "Max. Loading"
		'
		'TbLoadingMax
		'
		Me.TbLoadingMax.Location = New System.Drawing.Point(166, 54)
		Me.TbLoadingMax.Name = "TbLoadingMax"
		Me.TbLoadingMax.ReadOnly = True
		Me.TbLoadingMax.Size = New System.Drawing.Size(57, 20)
		Me.TbLoadingMax.TabIndex = 2
		Me.TbLoadingMax.TabStop = False
		'
		'Label22
		'
		Me.Label22.AutoSize = True
		Me.Label22.Location = New System.Drawing.Point(225, 57)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New System.Drawing.Size(25, 13)
		Me.Label22.TabIndex = 24
		Me.Label22.Text = "[kg]"
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.PnLoad)
		Me.GroupBox1.Controls.Add(Me.TbMass)
		Me.GroupBox1.Controls.Add(Me.Label1)
		Me.GroupBox1.Controls.Add(Me.Label14)
		Me.GroupBox1.Location = New System.Drawing.Point(6, 3)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(278, 124)
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
		Me.PnLoad.Location = New System.Drawing.Point(6, 43)
		Me.PnLoad.Name = "PnLoad"
		Me.PnLoad.Size = New System.Drawing.Size(256, 75)
		Me.PnLoad.TabIndex = 1
		'
		'GrAirRes
		'
		Me.GrAirRes.Controls.Add(Me.PnCdATrTr)
		Me.GrAirRes.Location = New System.Drawing.Point(290, 3)
		Me.GrAirRes.Name = "GrAirRes"
		Me.GrAirRes.Size = New System.Drawing.Size(278, 48)
		Me.GrAirRes.TabIndex = 1
		Me.GrAirRes.TabStop = False
		Me.GrAirRes.Text = "Air Resistance"
		'
		'PnCdATrTr
		'
		Me.PnCdATrTr.Controls.Add(Me.TBcdA)
		Me.PnCdATrTr.Controls.Add(Me.Label38)
		Me.PnCdATrTr.Controls.Add(Me.Label3)
		Me.PnCdATrTr.Dock = System.Windows.Forms.DockStyle.Fill
		Me.PnCdATrTr.Location = New System.Drawing.Point(3, 16)
		Me.PnCdATrTr.Name = "PnCdATrTr"
		Me.PnCdATrTr.Size = New System.Drawing.Size(272, 29)
		Me.PnCdATrTr.TabIndex = 0
		'
		'Label38
		'
		Me.Label38.AutoSize = True
		Me.Label38.Location = New System.Drawing.Point(105, 6)
		Me.Label38.Name = "Label38"
		Me.Label38.Size = New System.Drawing.Size(24, 13)
		Me.Label38.TabIndex = 24
		Me.Label38.Text = "[m²]"
		'
		'PictureBox1
		'
		Me.PictureBox1.BackColor = System.Drawing.Color.White
		Me.PictureBox1.Image = Global.VECTO.My.Resources.Resources.VECTO_VEH
		Me.PictureBox1.Location = New System.Drawing.Point(12, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New System.Drawing.Size(569, 40)
		Me.PictureBox1.TabIndex = 37
		Me.PictureBox1.TabStop = False
		'
		'CmOpenFile
		'
		Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
		Me.CmOpenFile.Name = "CmOpenFile"
		Me.CmOpenFile.Size = New System.Drawing.Size(153, 48)
		'
		'OpenWithToolStripMenuItem
		'
		Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
		Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
		Me.OpenWithToolStripMenuItem.Text = "Open with ..."
		'
		'ShowInFolderToolStripMenuItem
		'
		Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
		Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(152, 22)
		Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
		'
		'PnAll
		'
		Me.PnAll.Controls.Add(Me.GroupBox2)
		Me.PnAll.Controls.Add(Me.GrAirRes)
		Me.PnAll.Controls.Add(Me.GroupBox1)
		Me.PnAll.Controls.Add(Me.GroupBox8)
		Me.PnAll.Controls.Add(Me.GroupBox7)
		Me.PnAll.Controls.Add(Me.GroupBox6)
		Me.PnAll.Location = New System.Drawing.Point(6, 172)
		Me.PnAll.Name = "PnAll"
		Me.PnAll.Size = New System.Drawing.Size(575, 524)
		Me.PnAll.TabIndex = 4
		'
		'GroupBox2
		'
		Me.GroupBox2.Controls.Add(Me.pnAngularGearFields)
		Me.GroupBox2.Controls.Add(Me.cbAngularGearType)
		Me.GroupBox2.Location = New System.Drawing.Point(290, 327)
		Me.GroupBox2.Name = "GroupBox2"
		Me.GroupBox2.Size = New System.Drawing.Size(278, 111)
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
		Me.pnAngularGearFields.Location = New System.Drawing.Point(3, 42)
		Me.pnAngularGearFields.Name = "pnAngularGearFields"
		Me.pnAngularGearFields.Size = New System.Drawing.Size(272, 63)
		Me.pnAngularGearFields.TabIndex = 6
		'
		'Label4
		'
		Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(251, 4)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(16, 13)
		Me.Label4.TabIndex = 16
		Me.Label4.Text = "[-]"
		'
		'Label12
		'
		Me.Label12.Location = New System.Drawing.Point(6, 24)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(263, 16)
		Me.Label12.TabIndex = 17
		Me.Label12.Text = "Transmission Loss Map or Efficiency Value [0..1]"
		Me.Label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft
		'
		'Label10
		'
		Me.Label10.Location = New System.Drawing.Point(144, 4)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(44, 18)
		Me.Label10.TabIndex = 15
		Me.Label10.Text = "Ratio"
		Me.Label10.TextAlign = System.Drawing.ContentAlignment.TopRight
		'
		'tbAngularGearRatio
		'
		Me.tbAngularGearRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbAngularGearRatio.Location = New System.Drawing.Point(193, 2)
		Me.tbAngularGearRatio.Name = "tbAngularGearRatio"
		Me.tbAngularGearRatio.Size = New System.Drawing.Size(56, 20)
		Me.tbAngularGearRatio.TabIndex = 12
		'
		'btAngularGearLossMapBrowse
		'
		Me.btAngularGearLossMapBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.btAngularGearLossMapBrowse.Image = Global.VECTO.My.Resources.Resources.Open_icon
		Me.btAngularGearLossMapBrowse.Location = New System.Drawing.Point(244, 39)
		Me.btAngularGearLossMapBrowse.Name = "btAngularGearLossMapBrowse"
		Me.btAngularGearLossMapBrowse.Size = New System.Drawing.Size(24, 24)
		Me.btAngularGearLossMapBrowse.TabIndex = 14
		Me.btAngularGearLossMapBrowse.UseVisualStyleBackColor = True
		'
		'tbAngularGearLossMapPath
		'
		Me.tbAngularGearLossMapPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.tbAngularGearLossMapPath.Location = New System.Drawing.Point(6, 41)
		Me.tbAngularGearLossMapPath.Name = "tbAngularGearLossMapPath"
		Me.tbAngularGearLossMapPath.Size = New System.Drawing.Size(238, 20)
		Me.tbAngularGearLossMapPath.TabIndex = 13
		'
		'cbAngularGearType
		'
		Me.cbAngularGearType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.cbAngularGearType.FormattingEnabled = True
		Me.cbAngularGearType.Items.AddRange(New Object() {"No Angular Gear", "Separate Angular Gear", "Included in Transmission Loss Maps"})
		Me.cbAngularGearType.Location = New System.Drawing.Point(6, 19)
		Me.cbAngularGearType.Name = "cbAngularGearType"
		Me.cbAngularGearType.Size = New System.Drawing.Size(266, 21)
		Me.cbAngularGearType.TabIndex = 0
		'
		'PicVehicle
		'
		Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
		Me.PicVehicle.Location = New System.Drawing.Point(281, 74)
		Me.PicVehicle.Name = "PicVehicle"
		Me.PicVehicle.Size = New System.Drawing.Size(300, 88)
		Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.PicVehicle.TabIndex = 39
		Me.PicVehicle.TabStop = False
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(89, 136)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(58, 13)
		Me.Label8.TabIndex = 10
		Me.Label8.Text = "HDV Class"
		'
		'F_VEH
		'
		Me.AcceptButton = Me.ButOK
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New System.Drawing.Size(589, 745)
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
		Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.Name = "F_VEH"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
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
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.CmOpenFile.ResumeLayout(False)
		Me.PnAll.ResumeLayout(False)
		Me.GroupBox2.ResumeLayout(False)
		Me.pnAngularGearFields.ResumeLayout(False)
		Me.pnAngularGearFields.PerformLayout()
		CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents TbMass As System.Windows.Forms.TextBox
	Friend WithEvents Label2 As System.Windows.Forms.Label
	Friend WithEvents TbLoad As System.Windows.Forms.TextBox
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents TBcdA As System.Windows.Forms.TextBox
	Friend WithEvents Label13 As System.Windows.Forms.Label
	Friend WithEvents TBrdyn As System.Windows.Forms.TextBox
	Friend WithEvents ButOK As System.Windows.Forms.Button
	Friend WithEvents ButCancel As System.Windows.Forms.Button
	Friend WithEvents Label14 As System.Windows.Forms.Label
	Friend WithEvents Label31 As System.Windows.Forms.Label
	Friend WithEvents Label35 As System.Windows.Forms.Label
	Friend WithEvents CbCdMode As System.Windows.Forms.ComboBox
	Friend WithEvents TbCdFile As System.Windows.Forms.TextBox
	Friend WithEvents BtCdFileBrowse As System.Windows.Forms.Button
	Friend WithEvents GroupBox6 As System.Windows.Forms.GroupBox
	Friend WithEvents LbCdMode As System.Windows.Forms.Label
	Friend WithEvents ToolStrip1 As System.Windows.Forms.ToolStrip
	Friend WithEvents ToolStripBtNew As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripBtOpen As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripBtSave As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As System.Windows.Forms.ToolStripButton
	Friend WithEvents ToolStripSeparator3 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripBtSendTo As System.Windows.Forms.ToolStripButton
	Friend WithEvents GroupBox7 As System.Windows.Forms.GroupBox
	Friend WithEvents LbRtRatio As System.Windows.Forms.Label
	Friend WithEvents TbRtRatio As System.Windows.Forms.TextBox
	Friend WithEvents CbRtType As System.Windows.Forms.ComboBox
	Friend WithEvents Label45 As System.Windows.Forms.Label
	Friend WithEvents PnRt As System.Windows.Forms.Panel
	Friend WithEvents Label46 As System.Windows.Forms.Label
	Friend WithEvents Label50 As System.Windows.Forms.Label
	Friend WithEvents TbMassExtra As System.Windows.Forms.TextBox
	Friend WithEvents GroupBox8 As System.Windows.Forms.GroupBox
	Friend WithEvents ButAxlRem As System.Windows.Forms.Button
	Friend WithEvents LvRRC As System.Windows.Forms.ListView
	Friend WithEvents ColumnHeader7 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader8 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ButAxlAdd As System.Windows.Forms.Button
	Friend WithEvents CbCat As System.Windows.Forms.ComboBox
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents TbMassMass As System.Windows.Forms.TextBox
	Friend WithEvents ColumnHeader9 As System.Windows.Forms.ColumnHeader
	Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
	Friend WithEvents LbStatus As System.Windows.Forms.ToolStripStatusLabel
	Friend WithEvents CbAxleConfig As System.Windows.Forms.ComboBox
	Friend WithEvents TbHDVclass As System.Windows.Forms.TextBox
	Friend WithEvents Label11 As System.Windows.Forms.Label
	Friend WithEvents TbLoadingMax As System.Windows.Forms.TextBox
	Friend WithEvents Label22 As System.Windows.Forms.Label
	Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
	Friend WithEvents GrAirRes As System.Windows.Forms.GroupBox
	Friend WithEvents PictureBox1 As System.Windows.Forms.PictureBox
	Friend WithEvents ToolStripSeparator1 As System.Windows.Forms.ToolStripSeparator
	Friend WithEvents ToolStripButton1 As System.Windows.Forms.ToolStripButton
	Friend WithEvents CmOpenFile As System.Windows.Forms.ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents BtCdFileOpen As System.Windows.Forms.Button
	Friend WithEvents ColumnHeader1 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader2 As System.Windows.Forms.ColumnHeader
	Friend WithEvents PnLoad As System.Windows.Forms.Panel
	Friend WithEvents PnAll As System.Windows.Forms.Panel
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents CbRim As System.Windows.Forms.ComboBox
	Friend WithEvents PnWheelDiam As System.Windows.Forms.Panel
	Friend WithEvents PicVehicle As System.Windows.Forms.PictureBox
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents PnCdATrTr As System.Windows.Forms.Panel
	Friend WithEvents Label38 As System.Windows.Forms.Label
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents cbAngularGearType As System.Windows.Forms.ComboBox
	Friend WithEvents Label15 As System.Windows.Forms.Label
	Friend WithEvents BtRtBrowse As System.Windows.Forms.Button
	Friend WithEvents TbRtPath As System.Windows.Forms.TextBox
	Friend WithEvents pnAngularGearFields As System.Windows.Forms.Panel
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents tbAngularGearRatio As System.Windows.Forms.TextBox
	Friend WithEvents btAngularGearLossMapBrowse As System.Windows.Forms.Button
	Friend WithEvents tbAngularGearLossMapPath As System.Windows.Forms.TextBox
	Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
End Class
