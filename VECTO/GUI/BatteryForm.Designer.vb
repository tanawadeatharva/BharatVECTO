' Copyright 2017 European Union.
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

<DesignerGenerated()>
Partial Class BatteryForm
    Inherits Form

    'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
    <DebuggerNonUserCode()>
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
    <DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(BatteryForm))
        Me.tbCapacity = New System.Windows.Forms.TextBox()
        Me.lblCapacityUnit = New System.Windows.Forms.Label()
        Me.lblCapacity = New System.Windows.Forms.Label()
        Me.ButCancel = New System.Windows.Forms.Button()
        Me.ButOK = New System.Windows.Forms.Button()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtSave = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtSaveAs = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripBtSendTo = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tbMakeModel = New System.Windows.Forms.TextBox()
        Me.lblMakeModel = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnInertia = New System.Windows.Forms.Panel()
        Me.tbSoCCurve = New System.Windows.Forms.TextBox()
        Me.lblSoCCurve = New System.Windows.Forms.Label()
        Me.btnBrowseSoCCurve = New System.Windows.Forms.Button()
        Me.btnSoCCurveOpen = New System.Windows.Forms.Button()
        Me.btnRiMapOpen = New System.Windows.Forms.Button()
        Me.btnBrowseRiMap = New System.Windows.Forms.Button()
        Me.lblRiMap = New System.Windows.Forms.Label()
        Me.tbRiCurve = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.PicBox = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblMinSoc = New System.Windows.Forms.Label()
        Me.lblSoCMinUnit = New System.Windows.Forms.Label()
        Me.tbSoCMin = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblSoCMax = New System.Windows.Forms.Label()
        Me.lblSoCMaxUnit = New System.Windows.Forms.Label()
        Me.tbSoCMax = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblCFactor = New System.Windows.Forms.Label()
        Me.lblCFactorUnit = New System.Windows.Forms.Label()
        Me.tbCFactor = New System.Windows.Forms.TextBox()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.pnInertia.SuspendLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel1.SuspendLayout
        Me.Panel2.SuspendLayout
        Me.Panel3.SuspendLayout
        Me.SuspendLayout
        '
        'tbCapacity
        '
        Me.tbCapacity.Location = New System.Drawing.Point(90, 4)
        Me.tbCapacity.Name = "tbCapacity"
        Me.tbCapacity.Size = New System.Drawing.Size(57, 20)
        Me.tbCapacity.TabIndex = 3
        '
        'lblCapacityUnit
        '
        Me.lblCapacityUnit.AutoSize = true
        Me.lblCapacityUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblCapacityUnit.Name = "lblCapacityUnit"
        Me.lblCapacityUnit.Size = New System.Drawing.Size(25, 13)
        Me.lblCapacityUnit.TabIndex = 24
        Me.lblCapacityUnit.Text = "[As]"
        '
        'lblCapacity
        '
        Me.lblCapacity.AutoSize = true
        Me.lblCapacity.Location = New System.Drawing.Point(3, 7)
        Me.lblCapacity.Name = "lblCapacity"
        Me.lblCapacity.Size = New System.Drawing.Size(48, 13)
        Me.lblCapacity.TabIndex = 0
        Me.lblCapacity.Text = "Capacity"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(811, 303)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 13
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(730, 303)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 12
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(898, 25)
        Me.ToolStrip1.TabIndex = 30
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtNew.Text = "ToolStripButton1"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripBtSave.Text = "ToolStripButton1"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
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
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
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
        Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(23, 22)
        Me.ToolStripButton1.Text = "Help"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 329)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(898, 22)
        Me.StatusStrip1.SizingGrip = false
        Me.StatusStrip1.TabIndex = 37
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(39, 17)
        Me.LbStatus.Text = "Status"
        '
        'tbMakeModel
        '
        Me.tbMakeModel.Location = New System.Drawing.Point(109, 82)
        Me.tbMakeModel.Name = "tbMakeModel"
        Me.tbMakeModel.Size = New System.Drawing.Size(370, 20)
        Me.tbMakeModel.TabIndex = 0
        '
        'lblMakeModel
        '
        Me.lblMakeModel.AutoSize = true
        Me.lblMakeModel.Location = New System.Drawing.Point(16, 85)
        Me.lblMakeModel.Name = "lblMakeModel"
        Me.lblMakeModel.Size = New System.Drawing.Size(87, 13)
        Me.lblMakeModel.TabIndex = 11
        Me.lblMakeModel.Text = "Make and Model"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_ENG
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(502, 40)
        Me.PictureBox1.TabIndex = 39
        Me.PictureBox1.TabStop = false
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
        'pnInertia
        '
        Me.pnInertia.Controls.Add(Me.lblCapacity)
        Me.pnInertia.Controls.Add(Me.lblCapacityUnit)
        Me.pnInertia.Controls.Add(Me.tbCapacity)
        Me.pnInertia.Location = New System.Drawing.Point(12, 108)
        Me.pnInertia.Name = "pnInertia"
        Me.pnInertia.Size = New System.Drawing.Size(212, 30)
        Me.pnInertia.TabIndex = 3
        '
        'tbSoCCurve
        '
        Me.tbSoCCurve.Location = New System.Drawing.Point(12, 219)
        Me.tbSoCCurve.Name = "tbSoCCurve"
        Me.tbSoCCurve.Size = New System.Drawing.Size(434, 20)
        Me.tbSoCCurve.TabIndex = 5
        '
        'lblSoCCurve
        '
        Me.lblSoCCurve.AutoSize = true
        Me.lblSoCCurve.Location = New System.Drawing.Point(12, 203)
        Me.lblSoCCurve.Name = "lblSoCCurve"
        Me.lblSoCCurve.Size = New System.Drawing.Size(58, 13)
        Me.lblSoCCurve.TabIndex = 38
        Me.lblSoCCurve.Text = "SoC Curve"
        '
        'btnBrowseSoCCurve
        '
        Me.btnBrowseSoCCurve.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseSoCCurve.Location = New System.Drawing.Point(446, 217)
        Me.btnBrowseSoCCurve.Name = "btnBrowseSoCCurve"
        Me.btnBrowseSoCCurve.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseSoCCurve.TabIndex = 6
        Me.btnBrowseSoCCurve.TabStop = false
        Me.btnBrowseSoCCurve.UseVisualStyleBackColor = true
        '
        'btnSoCCurveOpen
        '
        Me.btnSoCCurveOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnSoCCurveOpen.Location = New System.Drawing.Point(469, 217)
        Me.btnSoCCurveOpen.Name = "btnSoCCurveOpen"
        Me.btnSoCCurveOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnSoCCurveOpen.TabIndex = 7
        Me.btnSoCCurveOpen.TabStop = false
        Me.btnSoCCurveOpen.UseVisualStyleBackColor = true
        '
        'btnRiMapOpen
        '
        Me.btnRiMapOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnRiMapOpen.Location = New System.Drawing.Point(469, 263)
        Me.btnRiMapOpen.Name = "btnRiMapOpen"
        Me.btnRiMapOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnRiMapOpen.TabIndex = 42
        Me.btnRiMapOpen.TabStop = false
        Me.btnRiMapOpen.UseVisualStyleBackColor = true
        '
        'btnBrowseRiMap
        '
        Me.btnBrowseRiMap.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseRiMap.Location = New System.Drawing.Point(446, 263)
        Me.btnBrowseRiMap.Name = "btnBrowseRiMap"
        Me.btnBrowseRiMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseRiMap.TabIndex = 41
        Me.btnBrowseRiMap.TabStop = false
        Me.btnBrowseRiMap.UseVisualStyleBackColor = true
        '
        'lblRiMap
        '
        Me.lblRiMap.AutoSize = true
        Me.lblRiMap.Location = New System.Drawing.Point(12, 249)
        Me.lblRiMap.Name = "lblRiMap"
        Me.lblRiMap.Size = New System.Drawing.Size(129, 13)
        Me.lblRiMap.TabIndex = 43
        Me.lblRiMap.Text = "Internal Resistance Curve"
        '
        'tbRiCurve
        '
        Me.tbRiCurve.Location = New System.Drawing.Point(12, 265)
        Me.tbRiCurve.Name = "tbRiCurve"
        Me.tbRiCurve.Size = New System.Drawing.Size(434, 20)
        Me.tbRiCurve.TabIndex = 40
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(119, 35)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(266, 29)
        Me.lblTitle.TabIndex = 48
        Me.lblTitle.Text = "Electric Energy Storage"
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(508, 28)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(382, 266)
        Me.PicBox.TabIndex = 49
        Me.PicBox.TabStop = false
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblMinSoc)
        Me.Panel1.Controls.Add(Me.lblSoCMinUnit)
        Me.Panel1.Controls.Add(Me.tbSoCMin)
        Me.Panel1.Location = New System.Drawing.Point(12, 144)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(212, 30)
        Me.Panel1.TabIndex = 25
        '
        'lblMinSoc
        '
        Me.lblMinSoc.AutoSize = true
        Me.lblMinSoc.Location = New System.Drawing.Point(3, 7)
        Me.lblMinSoc.Name = "lblMinSoc"
        Me.lblMinSoc.Size = New System.Drawing.Size(46, 13)
        Me.lblMinSoc.TabIndex = 0
        Me.lblMinSoc.Text = "SoC min"
        '
        'lblSoCMinUnit
        '
        Me.lblSoCMinUnit.AutoSize = true
        Me.lblSoCMinUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblSoCMinUnit.Name = "lblSoCMinUnit"
        Me.lblSoCMinUnit.Size = New System.Drawing.Size(21, 13)
        Me.lblSoCMinUnit.TabIndex = 24
        Me.lblSoCMinUnit.Text = "[%]"
        '
        'tbSoCMin
        '
        Me.tbSoCMin.Location = New System.Drawing.Point(90, 4)
        Me.tbSoCMin.Name = "tbSoCMin"
        Me.tbSoCMin.Size = New System.Drawing.Size(57, 20)
        Me.tbSoCMin.TabIndex = 3
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblSoCMax)
        Me.Panel2.Controls.Add(Me.lblSoCMaxUnit)
        Me.Panel2.Controls.Add(Me.tbSoCMax)
        Me.Panel2.Location = New System.Drawing.Point(234, 144)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(212, 30)
        Me.Panel2.TabIndex = 25
        '
        'lblSoCMax
        '
        Me.lblSoCMax.AutoSize = true
        Me.lblSoCMax.Location = New System.Drawing.Point(3, 7)
        Me.lblSoCMax.Name = "lblSoCMax"
        Me.lblSoCMax.Size = New System.Drawing.Size(49, 13)
        Me.lblSoCMax.TabIndex = 0
        Me.lblSoCMax.Text = "SoC max"
        '
        'lblSoCMaxUnit
        '
        Me.lblSoCMaxUnit.AutoSize = true
        Me.lblSoCMaxUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblSoCMaxUnit.Name = "lblSoCMaxUnit"
        Me.lblSoCMaxUnit.Size = New System.Drawing.Size(21, 13)
        Me.lblSoCMaxUnit.TabIndex = 24
        Me.lblSoCMaxUnit.Text = "[%]"
        '
        'tbSoCMax
        '
        Me.tbSoCMax.Location = New System.Drawing.Point(90, 4)
        Me.tbSoCMax.Name = "tbSoCMax"
        Me.tbSoCMax.Size = New System.Drawing.Size(57, 20)
        Me.tbSoCMax.TabIndex = 3
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.lblCFactor)
        Me.Panel3.Controls.Add(Me.lblCFactorUnit)
        Me.Panel3.Controls.Add(Me.tbCFactor)
        Me.Panel3.Location = New System.Drawing.Point(234, 108)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(212, 30)
        Me.Panel3.TabIndex = 25
        '
        'lblCFactor
        '
        Me.lblCFactor.AutoSize = true
        Me.lblCFactor.Location = New System.Drawing.Point(3, 7)
        Me.lblCFactor.Name = "lblCFactor"
        Me.lblCFactor.Size = New System.Drawing.Size(47, 13)
        Me.lblCFactor.TabIndex = 0
        Me.lblCFactor.Text = "C-Factor"
        '
        'lblCFactorUnit
        '
        Me.lblCFactorUnit.AutoSize = true
        Me.lblCFactorUnit.Location = New System.Drawing.Point(153, 7)
        Me.lblCFactorUnit.Name = "lblCFactorUnit"
        Me.lblCFactorUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblCFactorUnit.TabIndex = 24
        Me.lblCFactorUnit.Text = "[-]"
        '
        'tbCFactor
        '
        Me.tbCFactor.Location = New System.Drawing.Point(90, 4)
        Me.tbCFactor.Name = "tbCFactor"
        Me.tbCFactor.Size = New System.Drawing.Size(57, 20)
        Me.tbCFactor.TabIndex = 3
        '
        'BatteryForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(898, 351)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.btnRiMapOpen)
        Me.Controls.Add(Me.btnBrowseRiMap)
        Me.Controls.Add(Me.lblRiMap)
        Me.Controls.Add(Me.tbRiCurve)
        Me.Controls.Add(Me.pnInertia)
        Me.Controls.Add(Me.btnSoCCurveOpen)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnBrowseSoCCurve)
        Me.Controls.Add(Me.lblSoCCurve)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.tbSoCCurve)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.lblMakeModel)
        Me.Controls.Add(Me.tbMakeModel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "BatteryForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Electric Energy Storage"
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.pnInertia.ResumeLayout(false)
        Me.pnInertia.PerformLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).EndInit
        Me.Panel1.ResumeLayout(false)
        Me.Panel1.PerformLayout
        Me.Panel2.ResumeLayout(false)
        Me.Panel2.PerformLayout
        Me.Panel3.ResumeLayout(false)
        Me.Panel3.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents tbCapacity As TextBox
    Friend WithEvents lblCapacityUnit As Label
    Friend WithEvents lblCapacity As Label
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
    Friend WithEvents tbMakeModel As TextBox
    Friend WithEvents lblMakeModel As Label
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents CmOpenFile As ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents pnInertia As Panel
    Friend WithEvents tbSoCCurve As TextBox
    Friend WithEvents lblSoCCurve As Label
    Friend WithEvents btnBrowseSoCCurve As Button
    Friend WithEvents btnSoCCurveOpen As Button
    Friend WithEvents btnRiMapOpen As Button
    Friend WithEvents btnBrowseRiMap As Button
    Friend WithEvents lblRiMap As Label
    Friend WithEvents tbRiCurve As TextBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents PicBox As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblMinSoc As Label
    Friend WithEvents lblSoCMinUnit As Label
    Friend WithEvents tbSoCMin As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblSoCMax As Label
    Friend WithEvents lblSoCMaxUnit As Label
    Friend WithEvents tbSoCMax As TextBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents lblCFactor As Label
    Friend WithEvents lblCFactorUnit As Label
    Friend WithEvents tbCFactor As TextBox
End Class
