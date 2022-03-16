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
Partial Class HybridStrategyParamsForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(HybridStrategyParamsForm))
        Me.tbEquivalenceFactorDischarge = New System.Windows.Forms.TextBox()
        Me.lblEquivFactorUnit = New System.Windows.Forms.Label()
        Me.lblEvquivFactorDischg = New System.Windows.Forms.Label()
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
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.pnEquivFactor = New System.Windows.Forms.Panel()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.pnMinSoC = New System.Windows.Forms.Panel()
        Me.lblMinSoC = New System.Windows.Forms.Label()
        Me.lblMinSoCUnit = New System.Windows.Forms.Label()
        Me.tbMinSoC = New System.Windows.Forms.TextBox()
        Me.pnMaxSoC = New System.Windows.Forms.Panel()
        Me.lblMaxSoC = New System.Windows.Forms.Label()
        Me.lblMaxSoCUnit = New System.Windows.Forms.Label()
        Me.tbMaxSoC = New System.Windows.Forms.TextBox()
        Me.pnTargetSoC = New System.Windows.Forms.Panel()
        Me.lblTargetSoC = New System.Windows.Forms.Label()
        Me.lblTargetSoCUnit = New System.Windows.Forms.Label()
        Me.tbTargetSoC = New System.Windows.Forms.TextBox()
        Me.pnAuxBufferTime = New System.Windows.Forms.Panel()
        Me.lblAuxBufferTime = New System.Windows.Forms.Label()
        Me.lblAuxBufferTimeUnit = New System.Windows.Forms.Label()
        Me.tbauxBufferTime = New System.Windows.Forms.TextBox()
        Me.pnAuxBufferChgTime = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.lblAuxBufferChgTimeUnit = New System.Windows.Forms.Label()
        Me.tbAuxBufferChargeTime = New System.Windows.Forms.TextBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblMinIceOnTime = New System.Windows.Forms.Label()
        Me.lblMinIceOnTimeUnit = New System.Windows.Forms.Label()
        Me.tbMinICEOnTime = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.EquivalenceFactorChg = New System.Windows.Forms.Label()
        Me.lblEquivFactorChargeUnit = New System.Windows.Forms.Label()
        Me.tbEquivalenceFactorCharge = New System.Windows.Forms.TextBox()
        Me.pnICEStartPenaltyFactor = New System.Windows.Forms.Panel()
        Me.lblICEStartPenaltyFactor = New System.Windows.Forms.Label()
        Me.lblICEStartPenaltyFactorUnit = New System.Windows.Forms.Label()
        Me.tbICEStartPenaltyFactor = New System.Windows.Forms.TextBox()
        Me.pnCostFactorSoCExponent = New System.Windows.Forms.Panel()
        Me.lblCostFactorSoCExponent = New System.Windows.Forms.Label()
        Me.lblCostFactorSoCExponentUnit = New System.Windows.Forms.Label()
        Me.tbCostFactorSoCExponent = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblGensetMinPwrFactor = New System.Windows.Forms.Label()
        Me.lblGensetMinPowerFactorUnit = New System.Windows.Forms.Label()
        Me.tbGensetMinOptPowerFactor = New System.Windows.Forms.TextBox()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.pnEquivFactor.SuspendLayout
        Me.pnMinSoC.SuspendLayout
        Me.pnMaxSoC.SuspendLayout
        Me.pnTargetSoC.SuspendLayout
        Me.pnAuxBufferTime.SuspendLayout
        Me.pnAuxBufferChgTime.SuspendLayout
        Me.Panel1.SuspendLayout
        Me.Panel2.SuspendLayout
        Me.pnICEStartPenaltyFactor.SuspendLayout
        Me.pnCostFactorSoCExponent.SuspendLayout
        Me.Panel3.SuspendLayout
        Me.SuspendLayout
        '
        'tbEquivalenceFactorDischarge
        '
        Me.tbEquivalenceFactorDischarge.Location = New System.Drawing.Point(272, 6)
        Me.tbEquivalenceFactorDischarge.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbEquivalenceFactorDischarge.Name = "tbEquivalenceFactorDischarge"
        Me.tbEquivalenceFactorDischarge.Size = New System.Drawing.Size(84, 26)
        Me.tbEquivalenceFactorDischarge.TabIndex = 3
        '
        'lblEquivFactorUnit
        '
        Me.lblEquivFactorUnit.AutoSize = true
        Me.lblEquivFactorUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblEquivFactorUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEquivFactorUnit.Name = "lblEquivFactorUnit"
        Me.lblEquivFactorUnit.Size = New System.Drawing.Size(22, 20)
        Me.lblEquivFactorUnit.TabIndex = 24
        Me.lblEquivFactorUnit.Text = "[-]"
        '
        'lblEvquivFactorDischg
        '
        Me.lblEvquivFactorDischg.AutoSize = true
        Me.lblEvquivFactorDischg.Location = New System.Drawing.Point(4, 11)
        Me.lblEvquivFactorDischg.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEvquivFactorDischg.Name = "lblEvquivFactorDischg"
        Me.lblEvquivFactorDischg.Size = New System.Drawing.Size(221, 20)
        Me.lblEvquivFactorDischg.TabIndex = 0
        Me.lblEvquivFactorDischg.Text = "Equivalence Factor Discharge"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(543, 660)
        Me.ButCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(112, 35)
        Me.ButCancel.TabIndex = 11
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(422, 660)
        Me.ButOK.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(112, 35)
        Me.ButOK.TabIndex = 10
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(674, 33)
        Me.ToolStrip1.TabIndex = 30
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtNew.Text = "ToolStripButton1"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtSave.Text = "ToolStripButton1"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
        Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
        Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtSaveAs.Text = "ToolStripButton1"
        Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 33)
        '
        'ToolStripBtSendTo
        '
        Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
        Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
        Me.ToolStripBtSendTo.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripBtSendTo.Text = "Send to Job Editor"
        Me.ToolStripBtSendTo.ToolTipText = "Send to Job Editor"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 33)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(34, 28)
        Me.ToolStripButton1.Text = "Help"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 702)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(2, 0, 21, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(674, 32)
        Me.StatusStrip1.SizingGrip = false
        Me.StatusStrip1.TabIndex = 37
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(60, 25)
        Me.LbStatus.Text = "Status"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_ENG
        Me.PictureBox1.Location = New System.Drawing.Point(0, 43)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(753, 62)
        Me.PictureBox1.TabIndex = 39
        Me.PictureBox1.TabStop = false
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.Size = New System.Drawing.Size(203, 68)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(202, 32)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(202, 32)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'pnEquivFactor
        '
        Me.pnEquivFactor.Controls.Add(Me.lblEvquivFactorDischg)
        Me.pnEquivFactor.Controls.Add(Me.lblEquivFactorUnit)
        Me.pnEquivFactor.Controls.Add(Me.tbEquivalenceFactorDischarge)
        Me.pnEquivFactor.Location = New System.Drawing.Point(18, 114)
        Me.pnEquivFactor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnEquivFactor.Name = "pnEquivFactor"
        Me.pnEquivFactor.Size = New System.Drawing.Size(432, 43)
        Me.pnEquivFactor.TabIndex = 0
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(178, 54)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(461, 40)
        Me.lblTitle.TabIndex = 48
        Me.lblTitle.Text = "Hybrid Strategy Parameters"
        '
        'pnMinSoC
        '
        Me.pnMinSoC.Controls.Add(Me.lblMinSoC)
        Me.pnMinSoC.Controls.Add(Me.lblMinSoCUnit)
        Me.pnMinSoC.Controls.Add(Me.tbMinSoC)
        Me.pnMinSoC.Location = New System.Drawing.Point(18, 206)
        Me.pnMinSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnMinSoC.Name = "pnMinSoC"
        Me.pnMinSoC.Size = New System.Drawing.Size(432, 43)
        Me.pnMinSoC.TabIndex = 2
        '
        'lblMinSoC
        '
        Me.lblMinSoC.AutoSize = true
        Me.lblMinSoC.Location = New System.Drawing.Point(4, 11)
        Me.lblMinSoC.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMinSoC.Name = "lblMinSoC"
        Me.lblMinSoC.Size = New System.Drawing.Size(69, 20)
        Me.lblMinSoC.TabIndex = 0
        Me.lblMinSoC.Text = "Min SoC"
        '
        'lblMinSoCUnit
        '
        Me.lblMinSoCUnit.AutoSize = true
        Me.lblMinSoCUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblMinSoCUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMinSoCUnit.Name = "lblMinSoCUnit"
        Me.lblMinSoCUnit.Size = New System.Drawing.Size(31, 20)
        Me.lblMinSoCUnit.TabIndex = 24
        Me.lblMinSoCUnit.Text = "[%]"
        '
        'tbMinSoC
        '
        Me.tbMinSoC.Location = New System.Drawing.Point(272, 6)
        Me.tbMinSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbMinSoC.Name = "tbMinSoC"
        Me.tbMinSoC.Size = New System.Drawing.Size(84, 26)
        Me.tbMinSoC.TabIndex = 3
        '
        'pnMaxSoC
        '
        Me.pnMaxSoC.Controls.Add(Me.lblMaxSoC)
        Me.pnMaxSoC.Controls.Add(Me.lblMaxSoCUnit)
        Me.pnMaxSoC.Controls.Add(Me.tbMaxSoC)
        Me.pnMaxSoC.Location = New System.Drawing.Point(18, 252)
        Me.pnMaxSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnMaxSoC.Name = "pnMaxSoC"
        Me.pnMaxSoC.Size = New System.Drawing.Size(432, 43)
        Me.pnMaxSoC.TabIndex = 3
        '
        'lblMaxSoC
        '
        Me.lblMaxSoC.AutoSize = true
        Me.lblMaxSoC.Location = New System.Drawing.Point(4, 11)
        Me.lblMaxSoC.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMaxSoC.Name = "lblMaxSoC"
        Me.lblMaxSoC.Size = New System.Drawing.Size(73, 20)
        Me.lblMaxSoC.TabIndex = 0
        Me.lblMaxSoC.Text = "Max SoC"
        '
        'lblMaxSoCUnit
        '
        Me.lblMaxSoCUnit.AutoSize = true
        Me.lblMaxSoCUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblMaxSoCUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMaxSoCUnit.Name = "lblMaxSoCUnit"
        Me.lblMaxSoCUnit.Size = New System.Drawing.Size(31, 20)
        Me.lblMaxSoCUnit.TabIndex = 24
        Me.lblMaxSoCUnit.Text = "[%]"
        '
        'tbMaxSoC
        '
        Me.tbMaxSoC.Location = New System.Drawing.Point(272, 6)
        Me.tbMaxSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbMaxSoC.Name = "tbMaxSoC"
        Me.tbMaxSoC.Size = New System.Drawing.Size(84, 26)
        Me.tbMaxSoC.TabIndex = 3
        '
        'pnTargetSoC
        '
        Me.pnTargetSoC.Controls.Add(Me.lblTargetSoC)
        Me.pnTargetSoC.Controls.Add(Me.lblTargetSoCUnit)
        Me.pnTargetSoC.Controls.Add(Me.tbTargetSoC)
        Me.pnTargetSoC.Location = New System.Drawing.Point(18, 298)
        Me.pnTargetSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnTargetSoC.Name = "pnTargetSoC"
        Me.pnTargetSoC.Size = New System.Drawing.Size(432, 43)
        Me.pnTargetSoC.TabIndex = 4
        '
        'lblTargetSoC
        '
        Me.lblTargetSoC.AutoSize = true
        Me.lblTargetSoC.Location = New System.Drawing.Point(4, 11)
        Me.lblTargetSoC.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTargetSoC.Name = "lblTargetSoC"
        Me.lblTargetSoC.Size = New System.Drawing.Size(90, 20)
        Me.lblTargetSoC.TabIndex = 0
        Me.lblTargetSoC.Text = "Target SoC"
        '
        'lblTargetSoCUnit
        '
        Me.lblTargetSoCUnit.AutoSize = true
        Me.lblTargetSoCUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblTargetSoCUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTargetSoCUnit.Name = "lblTargetSoCUnit"
        Me.lblTargetSoCUnit.Size = New System.Drawing.Size(31, 20)
        Me.lblTargetSoCUnit.TabIndex = 24
        Me.lblTargetSoCUnit.Text = "[%]"
        '
        'tbTargetSoC
        '
        Me.tbTargetSoC.Location = New System.Drawing.Point(272, 6)
        Me.tbTargetSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbTargetSoC.Name = "tbTargetSoC"
        Me.tbTargetSoC.Size = New System.Drawing.Size(84, 26)
        Me.tbTargetSoC.TabIndex = 3
        '
        'pnAuxBufferTime
        '
        Me.pnAuxBufferTime.Controls.Add(Me.lblAuxBufferTime)
        Me.pnAuxBufferTime.Controls.Add(Me.lblAuxBufferTimeUnit)
        Me.pnAuxBufferTime.Controls.Add(Me.tbauxBufferTime)
        Me.pnAuxBufferTime.Location = New System.Drawing.Point(18, 391)
        Me.pnAuxBufferTime.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnAuxBufferTime.Name = "pnAuxBufferTime"
        Me.pnAuxBufferTime.Size = New System.Drawing.Size(432, 43)
        Me.pnAuxBufferTime.TabIndex = 6
        '
        'lblAuxBufferTime
        '
        Me.lblAuxBufferTime.AutoSize = true
        Me.lblAuxBufferTime.Location = New System.Drawing.Point(4, 11)
        Me.lblAuxBufferTime.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblAuxBufferTime.Name = "lblAuxBufferTime"
        Me.lblAuxBufferTime.Size = New System.Drawing.Size(122, 20)
        Me.lblAuxBufferTime.TabIndex = 0
        Me.lblAuxBufferTime.Text = "Aux Buffer Time"
        '
        'lblAuxBufferTimeUnit
        '
        Me.lblAuxBufferTimeUnit.AutoSize = true
        Me.lblAuxBufferTimeUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblAuxBufferTimeUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblAuxBufferTimeUnit.Name = "lblAuxBufferTimeUnit"
        Me.lblAuxBufferTimeUnit.Size = New System.Drawing.Size(25, 20)
        Me.lblAuxBufferTimeUnit.TabIndex = 24
        Me.lblAuxBufferTimeUnit.Text = "[s]"
        '
        'tbauxBufferTime
        '
        Me.tbauxBufferTime.Location = New System.Drawing.Point(272, 6)
        Me.tbauxBufferTime.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbauxBufferTime.Name = "tbauxBufferTime"
        Me.tbauxBufferTime.Size = New System.Drawing.Size(84, 26)
        Me.tbauxBufferTime.TabIndex = 3
        '
        'pnAuxBufferChgTime
        '
        Me.pnAuxBufferChgTime.Controls.Add(Me.Label1)
        Me.pnAuxBufferChgTime.Controls.Add(Me.lblAuxBufferChgTimeUnit)
        Me.pnAuxBufferChgTime.Controls.Add(Me.tbAuxBufferChargeTime)
        Me.pnAuxBufferChgTime.Location = New System.Drawing.Point(18, 437)
        Me.pnAuxBufferChgTime.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnAuxBufferChgTime.Name = "pnAuxBufferChgTime"
        Me.pnAuxBufferChgTime.Size = New System.Drawing.Size(432, 43)
        Me.pnAuxBufferChgTime.TabIndex = 7
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(4, 11)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(178, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Aux Buffer Charge Time"
        '
        'lblAuxBufferChgTimeUnit
        '
        Me.lblAuxBufferChgTimeUnit.AutoSize = true
        Me.lblAuxBufferChgTimeUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblAuxBufferChgTimeUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblAuxBufferChgTimeUnit.Name = "lblAuxBufferChgTimeUnit"
        Me.lblAuxBufferChgTimeUnit.Size = New System.Drawing.Size(25, 20)
        Me.lblAuxBufferChgTimeUnit.TabIndex = 24
        Me.lblAuxBufferChgTimeUnit.Text = "[s]"
        '
        'tbAuxBufferChargeTime
        '
        Me.tbAuxBufferChargeTime.Location = New System.Drawing.Point(272, 6)
        Me.tbAuxBufferChargeTime.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbAuxBufferChargeTime.Name = "tbAuxBufferChargeTime"
        Me.tbAuxBufferChargeTime.Size = New System.Drawing.Size(84, 26)
        Me.tbAuxBufferChargeTime.TabIndex = 3
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblMinIceOnTime)
        Me.Panel1.Controls.Add(Me.lblMinIceOnTimeUnit)
        Me.Panel1.Controls.Add(Me.tbMinICEOnTime)
        Me.Panel1.Location = New System.Drawing.Point(18, 345)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(432, 43)
        Me.Panel1.TabIndex = 5
        '
        'lblMinIceOnTime
        '
        Me.lblMinIceOnTime.AutoSize = true
        Me.lblMinIceOnTime.Location = New System.Drawing.Point(4, 11)
        Me.lblMinIceOnTime.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMinIceOnTime.Name = "lblMinIceOnTime"
        Me.lblMinIceOnTime.Size = New System.Drawing.Size(128, 20)
        Me.lblMinIceOnTime.TabIndex = 0
        Me.lblMinIceOnTime.Text = "Min ICE On Time"
        '
        'lblMinIceOnTimeUnit
        '
        Me.lblMinIceOnTimeUnit.AutoSize = true
        Me.lblMinIceOnTimeUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblMinIceOnTimeUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMinIceOnTimeUnit.Name = "lblMinIceOnTimeUnit"
        Me.lblMinIceOnTimeUnit.Size = New System.Drawing.Size(25, 20)
        Me.lblMinIceOnTimeUnit.TabIndex = 24
        Me.lblMinIceOnTimeUnit.Text = "[s]"
        '
        'tbMinICEOnTime
        '
        Me.tbMinICEOnTime.Location = New System.Drawing.Point(272, 6)
        Me.tbMinICEOnTime.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbMinICEOnTime.Name = "tbMinICEOnTime"
        Me.tbMinICEOnTime.Size = New System.Drawing.Size(84, 26)
        Me.tbMinICEOnTime.TabIndex = 3
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.EquivalenceFactorChg)
        Me.Panel2.Controls.Add(Me.lblEquivFactorChargeUnit)
        Me.Panel2.Controls.Add(Me.tbEquivalenceFactorCharge)
        Me.Panel2.Location = New System.Drawing.Point(18, 160)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(432, 43)
        Me.Panel2.TabIndex = 1
        '
        'EquivalenceFactorChg
        '
        Me.EquivalenceFactorChg.AutoSize = true
        Me.EquivalenceFactorChg.Location = New System.Drawing.Point(4, 11)
        Me.EquivalenceFactorChg.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.EquivalenceFactorChg.Name = "EquivalenceFactorChg"
        Me.EquivalenceFactorChg.Size = New System.Drawing.Size(201, 20)
        Me.EquivalenceFactorChg.TabIndex = 0
        Me.EquivalenceFactorChg.Text = "Equivalence Factor Charge"
        '
        'lblEquivFactorChargeUnit
        '
        Me.lblEquivFactorChargeUnit.AutoSize = true
        Me.lblEquivFactorChargeUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblEquivFactorChargeUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEquivFactorChargeUnit.Name = "lblEquivFactorChargeUnit"
        Me.lblEquivFactorChargeUnit.Size = New System.Drawing.Size(22, 20)
        Me.lblEquivFactorChargeUnit.TabIndex = 24
        Me.lblEquivFactorChargeUnit.Text = "[-]"
        '
        'tbEquivalenceFactorCharge
        '
        Me.tbEquivalenceFactorCharge.Location = New System.Drawing.Point(272, 6)
        Me.tbEquivalenceFactorCharge.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbEquivalenceFactorCharge.Name = "tbEquivalenceFactorCharge"
        Me.tbEquivalenceFactorCharge.Size = New System.Drawing.Size(84, 26)
        Me.tbEquivalenceFactorCharge.TabIndex = 3
        '
        'pnICEStartPenaltyFactor
        '
        Me.pnICEStartPenaltyFactor.Controls.Add(Me.lblICEStartPenaltyFactor)
        Me.pnICEStartPenaltyFactor.Controls.Add(Me.lblICEStartPenaltyFactorUnit)
        Me.pnICEStartPenaltyFactor.Controls.Add(Me.tbICEStartPenaltyFactor)
        Me.pnICEStartPenaltyFactor.Location = New System.Drawing.Point(18, 483)
        Me.pnICEStartPenaltyFactor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnICEStartPenaltyFactor.Name = "pnICEStartPenaltyFactor"
        Me.pnICEStartPenaltyFactor.Size = New System.Drawing.Size(432, 43)
        Me.pnICEStartPenaltyFactor.TabIndex = 8
        '
        'lblICEStartPenaltyFactor
        '
        Me.lblICEStartPenaltyFactor.AutoSize = true
        Me.lblICEStartPenaltyFactor.Location = New System.Drawing.Point(4, 11)
        Me.lblICEStartPenaltyFactor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblICEStartPenaltyFactor.Name = "lblICEStartPenaltyFactor"
        Me.lblICEStartPenaltyFactor.Size = New System.Drawing.Size(172, 20)
        Me.lblICEStartPenaltyFactor.TabIndex = 0
        Me.lblICEStartPenaltyFactor.Text = "ICE start penalty factor"
        '
        'lblICEStartPenaltyFactorUnit
        '
        Me.lblICEStartPenaltyFactorUnit.AutoSize = true
        Me.lblICEStartPenaltyFactorUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblICEStartPenaltyFactorUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblICEStartPenaltyFactorUnit.Name = "lblICEStartPenaltyFactorUnit"
        Me.lblICEStartPenaltyFactorUnit.Size = New System.Drawing.Size(22, 20)
        Me.lblICEStartPenaltyFactorUnit.TabIndex = 24
        Me.lblICEStartPenaltyFactorUnit.Text = "[-]"
        '
        'tbICEStartPenaltyFactor
        '
        Me.tbICEStartPenaltyFactor.Location = New System.Drawing.Point(272, 6)
        Me.tbICEStartPenaltyFactor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbICEStartPenaltyFactor.Name = "tbICEStartPenaltyFactor"
        Me.tbICEStartPenaltyFactor.Size = New System.Drawing.Size(84, 26)
        Me.tbICEStartPenaltyFactor.TabIndex = 3
        '
        'pnCostFactorSoCExponent
        '
        Me.pnCostFactorSoCExponent.Controls.Add(Me.lblCostFactorSoCExponent)
        Me.pnCostFactorSoCExponent.Controls.Add(Me.lblCostFactorSoCExponentUnit)
        Me.pnCostFactorSoCExponent.Controls.Add(Me.tbCostFactorSoCExponent)
        Me.pnCostFactorSoCExponent.Location = New System.Drawing.Point(18, 529)
        Me.pnCostFactorSoCExponent.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnCostFactorSoCExponent.Name = "pnCostFactorSoCExponent"
        Me.pnCostFactorSoCExponent.Size = New System.Drawing.Size(432, 43)
        Me.pnCostFactorSoCExponent.TabIndex = 9
        '
        'lblCostFactorSoCExponent
        '
        Me.lblCostFactorSoCExponent.AutoSize = true
        Me.lblCostFactorSoCExponent.Location = New System.Drawing.Point(4, 11)
        Me.lblCostFactorSoCExponent.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblCostFactorSoCExponent.Name = "lblCostFactorSoCExponent"
        Me.lblCostFactorSoCExponent.Size = New System.Drawing.Size(199, 20)
        Me.lblCostFactorSoCExponent.TabIndex = 0
        Me.lblCostFactorSoCExponent.Text = "Cost Factor SoC Exponent"
        '
        'lblCostFactorSoCExponentUnit
        '
        Me.lblCostFactorSoCExponentUnit.AutoSize = true
        Me.lblCostFactorSoCExponentUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblCostFactorSoCExponentUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblCostFactorSoCExponentUnit.Name = "lblCostFactorSoCExponentUnit"
        Me.lblCostFactorSoCExponentUnit.Size = New System.Drawing.Size(22, 20)
        Me.lblCostFactorSoCExponentUnit.TabIndex = 24
        Me.lblCostFactorSoCExponentUnit.Text = "[-]"
        '
        'tbCostFactorSoCExponent
        '
        Me.tbCostFactorSoCExponent.Location = New System.Drawing.Point(272, 6)
        Me.tbCostFactorSoCExponent.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbCostFactorSoCExponent.Name = "tbCostFactorSoCExponent"
        Me.tbCostFactorSoCExponent.Size = New System.Drawing.Size(84, 26)
        Me.tbCostFactorSoCExponent.TabIndex = 3
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.lblGensetMinPwrFactor)
        Me.Panel3.Controls.Add(Me.lblGensetMinPowerFactorUnit)
        Me.Panel3.Controls.Add(Me.tbGensetMinOptPowerFactor)
        Me.Panel3.Location = New System.Drawing.Point(18, 582)
        Me.Panel3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(432, 43)
        Me.Panel3.TabIndex = 25
        '
        'lblGensetMinPwrFactor
        '
        Me.lblGensetMinPwrFactor.AutoSize = true
        Me.lblGensetMinPwrFactor.Location = New System.Drawing.Point(4, 11)
        Me.lblGensetMinPwrFactor.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblGensetMinPwrFactor.Name = "lblGensetMinPwrFactor"
        Me.lblGensetMinPwrFactor.Size = New System.Drawing.Size(244, 20)
        Me.lblGensetMinPwrFactor.TabIndex = 0
        Me.lblGensetMinPwrFactor.Text = "Genset min optimal Power Factor"
        '
        'lblGensetMinPowerFactorUnit
        '
        Me.lblGensetMinPowerFactorUnit.AutoSize = true
        Me.lblGensetMinPowerFactorUnit.Location = New System.Drawing.Point(366, 11)
        Me.lblGensetMinPowerFactorUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblGensetMinPowerFactorUnit.Name = "lblGensetMinPowerFactorUnit"
        Me.lblGensetMinPowerFactorUnit.Size = New System.Drawing.Size(22, 20)
        Me.lblGensetMinPowerFactorUnit.TabIndex = 24
        Me.lblGensetMinPowerFactorUnit.Text = "[-]"
        '
        'tbGensetMinOptPowerFactor
        '
        Me.tbGensetMinOptPowerFactor.Location = New System.Drawing.Point(272, 6)
        Me.tbGensetMinOptPowerFactor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbGensetMinOptPowerFactor.Name = "tbGensetMinOptPowerFactor"
        Me.tbGensetMinOptPowerFactor.Size = New System.Drawing.Size(84, 26)
        Me.tbGensetMinOptPowerFactor.TabIndex = 3
        '
        'HybridStrategyParamsForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9!, 20!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(674, 734)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.pnCostFactorSoCExponent)
        Me.Controls.Add(Me.pnICEStartPenaltyFactor)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.pnAuxBufferChgTime)
        Me.Controls.Add(Me.pnAuxBufferTime)
        Me.Controls.Add(Me.pnTargetSoC)
        Me.Controls.Add(Me.pnMaxSoC)
        Me.Controls.Add(Me.pnMinSoC)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pnEquivFactor)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = false
        Me.Name = "HybridStrategyParamsForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Hybrid Strategy Parameters"
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.pnEquivFactor.ResumeLayout(false)
        Me.pnEquivFactor.PerformLayout
        Me.pnMinSoC.ResumeLayout(false)
        Me.pnMinSoC.PerformLayout
        Me.pnMaxSoC.ResumeLayout(false)
        Me.pnMaxSoC.PerformLayout
        Me.pnTargetSoC.ResumeLayout(false)
        Me.pnTargetSoC.PerformLayout
        Me.pnAuxBufferTime.ResumeLayout(false)
        Me.pnAuxBufferTime.PerformLayout
        Me.pnAuxBufferChgTime.ResumeLayout(false)
        Me.pnAuxBufferChgTime.PerformLayout
        Me.Panel1.ResumeLayout(false)
        Me.Panel1.PerformLayout
        Me.Panel2.ResumeLayout(false)
        Me.Panel2.PerformLayout
        Me.pnICEStartPenaltyFactor.ResumeLayout(false)
        Me.pnICEStartPenaltyFactor.PerformLayout
        Me.pnCostFactorSoCExponent.ResumeLayout(false)
        Me.pnCostFactorSoCExponent.PerformLayout
        Me.Panel3.ResumeLayout(false)
        Me.Panel3.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents tbEquivalenceFactorDischarge As TextBox
    Friend WithEvents lblEquivFactorUnit As Label
    Friend WithEvents lblEvquivFactorDischg As Label
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
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents CmOpenFile As ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents pnEquivFactor As Panel
    Friend WithEvents lblTitle As Label
    Friend WithEvents pnMinSoC As Panel
    Friend WithEvents lblMinSoC As Label
    Friend WithEvents lblMinSoCUnit As Label
    Friend WithEvents tbMinSoC As TextBox
    Friend WithEvents pnMaxSoC As Panel
    Friend WithEvents lblMaxSoC As Label
    Friend WithEvents lblMaxSoCUnit As Label
    Friend WithEvents tbMaxSoC As TextBox
    Friend WithEvents pnTargetSoC As Panel
    Friend WithEvents lblTargetSoC As Label
    Friend WithEvents lblTargetSoCUnit As Label
    Friend WithEvents tbTargetSoC As TextBox
    Friend WithEvents pnAuxBufferTime As Panel
    Friend WithEvents lblAuxBufferTime As Label
    Friend WithEvents lblAuxBufferTimeUnit As Label
    Friend WithEvents tbauxBufferTime As TextBox
    Friend WithEvents pnAuxBufferChgTime As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents lblAuxBufferChgTimeUnit As Label
    Friend WithEvents tbAuxBufferChargeTime As TextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblMinIceOnTime As Label
    Friend WithEvents lblMinIceOnTimeUnit As Label
    Friend WithEvents tbMinICEOnTime As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents EquivalenceFactorChg As Label
    Friend WithEvents lblEquivFactorChargeUnit As Label
    Friend WithEvents tbEquivalenceFactorCharge As TextBox
    Friend WithEvents pnICEStartPenaltyFactor As Panel
    Friend WithEvents lblICEStartPenaltyFactor As Label
    Friend WithEvents lblICEStartPenaltyFactorUnit As Label
    Friend WithEvents tbICEStartPenaltyFactor As TextBox
    Friend WithEvents pnCostFactorSoCExponent As Panel
    Friend WithEvents lblCostFactorSoCExponent As Label
    Friend WithEvents lblCostFactorSoCExponentUnit As Label
    Friend WithEvents tbCostFactorSoCExponent As TextBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents lblGensetMinPwrFactor As Label
    Friend WithEvents lblGensetMinPowerFactorUnit As Label
    Friend WithEvents tbGensetMinOptPowerFactor As TextBox
End Class
