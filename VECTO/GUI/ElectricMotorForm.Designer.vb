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
Partial Class ElectricMotorForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ElectricMotorForm))
        Me.tbInertia = New System.Windows.Forms.TextBox()
        Me.lblinertiaUnit = New System.Windows.Forms.Label()
        Me.lblInertia = New System.Windows.Forms.Label()
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
        Me.btnEmMapOpenHi = New System.Windows.Forms.Button()
        Me.btnBrowseEmMapHi = New System.Windows.Forms.Button()
        Me.lblPowerMapHi = New System.Windows.Forms.Label()
        Me.tbMapHi = New System.Windows.Forms.TextBox()
        Me.btnMaxTorqueCurveOpenHi = New System.Windows.Forms.Button()
        Me.btnBrowseMaxTorqueHi = New System.Windows.Forms.Button()
        Me.lblMaxTorqueHi = New System.Windows.Forms.Label()
        Me.tbMaxTorqueHi = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.PicBox = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblContTq = New System.Windows.Forms.Label()
        Me.lblContTqUnit = New System.Windows.Forms.Label()
        Me.tbContTqLo = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblOvlTime = New System.Windows.Forms.Label()
        Me.lblOvltimeUnit = New System.Windows.Forms.Label()
        Me.tbOvlTimeLo = New System.Windows.Forms.TextBox()
        Me.pnThermalOverloadRecovery = New System.Windows.Forms.Panel()
        Me.lblOvlRecovery = New System.Windows.Forms.Label()
        Me.lblOvlRecoveryFactorUnit = New System.Windows.Forms.Label()
        Me.tbOverloadRecoveryFactor = New System.Windows.Forms.TextBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.lblRatedSpeed = New System.Windows.Forms.Label()
        Me.lblRatedSpeedUnit = New System.Windows.Forms.Label()
        Me.tbRatedSpeedLo = New System.Windows.Forms.TextBox()
        Me.pnOverloadTq = New System.Windows.Forms.Panel()
        Me.lblOverloadTq = New System.Windows.Forms.Label()
        Me.lblOverloadTqUnit = New System.Windows.Forms.Label()
        Me.tbOverloadTqLo = New System.Windows.Forms.TextBox()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.lblOverloadSpeed = New System.Windows.Forms.Label()
        Me.lblOverloadSpeedUnit = New System.Windows.Forms.Label()
        Me.tbOvlSpeedLo = New System.Windows.Forms.TextBox()
        Me.lblVoltageHi = New System.Windows.Forms.Label()
        Me.lblVoltageHiUnit = New System.Windows.Forms.Label()
        Me.tbVoltageHi = New System.Windows.Forms.TextBox()
        Me.lblVoltageLow = New System.Windows.Forms.Label()
        Me.lblVoltageLowUnit = New System.Windows.Forms.Label()
        Me.tbVoltageLow = New System.Windows.Forms.TextBox()
        Me.lblMaxTorqueLow = New System.Windows.Forms.Label()
        Me.tbDragTorque = New System.Windows.Forms.TextBox()
        Me.lblDragTorque = New System.Windows.Forms.Label()
        Me.btnBrowseDragCurve = New System.Windows.Forms.Button()
        Me.btnDragCurveOpen = New System.Windows.Forms.Button()
        Me.tbMapLow = New System.Windows.Forms.TextBox()
        Me.lblPowerMapLow = New System.Windows.Forms.Label()
        Me.btnBrowseEmMapLow = New System.Windows.Forms.Button()
        Me.btnEmMapOpenLow = New System.Windows.Forms.Button()
        Me.tbMaxTorqueLow = New System.Windows.Forms.TextBox()
        Me.btnMaxTorqueCurveOpenLow = New System.Windows.Forms.Button()
        Me.btnBrowseMaxTorqueLow = New System.Windows.Forms.Button()
        Me.tcVoltageLevels = New System.Windows.Forms.TabControl()
        Me.tpVoltageLow = New System.Windows.Forms.TabPage()
        Me.tpVoltageHigh = New System.Windows.Forms.TabPage()
        Me.Panel5 = New System.Windows.Forms.Panel()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.tbOvlSpeedHi = New System.Windows.Forms.TextBox()
        Me.Panel7 = New System.Windows.Forms.Panel()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbOverloadTqHi = New System.Windows.Forms.TextBox()
        Me.Panel8 = New System.Windows.Forms.Panel()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.tbRatedSpeedHi = New System.Windows.Forms.TextBox()
        Me.Panel9 = New System.Windows.Forms.Panel()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.tbOvlTimeHi = New System.Windows.Forms.TextBox()
        Me.Panel10 = New System.Windows.Forms.Panel()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.tbContTqHi = New System.Windows.Forms.TextBox()
        Me.pnDragCurve = New System.Windows.Forms.Panel()
        Me.pnRatedPower = New System.Windows.Forms.Panel()
        Me.lblRatedPower = New System.Windows.Forms.Label()
        Me.lblRatedPowerUnit = New System.Windows.Forms.Label()
        Me.tbRatedPower = New System.Windows.Forms.TextBox()
        Me.pnElectricMachineType = New System.Windows.Forms.Panel()
        Me.cbEmType = New System.Windows.Forms.ComboBox()
        Me.lblEmType = New System.Windows.Forms.Label()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.pnInertia.SuspendLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel1.SuspendLayout
        Me.Panel2.SuspendLayout
        Me.pnThermalOverloadRecovery.SuspendLayout
        Me.Panel4.SuspendLayout
        Me.pnOverloadTq.SuspendLayout
        Me.Panel6.SuspendLayout
        Me.tcVoltageLevels.SuspendLayout
        Me.tpVoltageLow.SuspendLayout
        Me.tpVoltageHigh.SuspendLayout
        Me.Panel5.SuspendLayout
        Me.Panel7.SuspendLayout
        Me.Panel8.SuspendLayout
        Me.Panel9.SuspendLayout
        Me.Panel10.SuspendLayout
        Me.pnDragCurve.SuspendLayout
        Me.pnRatedPower.SuspendLayout
        Me.pnElectricMachineType.SuspendLayout
        Me.SuspendLayout
        '
        'tbInertia
        '
        Me.tbInertia.Location = New System.Drawing.Point(168, 5)
        Me.tbInertia.Name = "tbInertia"
        Me.tbInertia.Size = New System.Drawing.Size(49, 20)
        Me.tbInertia.TabIndex = 3
        '
        'lblinertiaUnit
        '
        Me.lblinertiaUnit.AutoSize = True
        Me.lblinertiaUnit.Location = New System.Drawing.Point(223, 8)
        Me.lblinertiaUnit.Name = "lblinertiaUnit"
        Me.lblinertiaUnit.Size = New System.Drawing.Size(36, 13)
        Me.lblinertiaUnit.TabIndex = 24
        Me.lblinertiaUnit.Text = "[kgm²]"
        '
        'lblInertia
        '
        Me.lblInertia.AutoSize = True
        Me.lblInertia.Location = New System.Drawing.Point(3, 7)
        Me.lblInertia.Name = "lblInertia"
        Me.lblInertia.Size = New System.Drawing.Size(36, 13)
        Me.lblInertia.TabIndex = 0
        Me.lblInertia.Text = "Inertia"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(854, 465)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 13
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = True
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(773, 465)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 12
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = True
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(941, 31)
        Me.ToolStrip1.TabIndex = 30
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = CType(resources.GetObject("ToolStripBtNew.Image"), System.Drawing.Image)
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtNew.Text = "ToolStripButton1"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = CType(resources.GetObject("ToolStripBtOpen.Image"), System.Drawing.Image)
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtSave.Text = "ToolStripButton1"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
        Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
        Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtSaveAs.Text = "ToolStripButton1"
        Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 31)
        '
        'ToolStripBtSendTo
        '
        Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
        Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
        Me.ToolStripBtSendTo.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtSendTo.Text = "Send to Job Editor"
        Me.ToolStripBtSendTo.ToolTipText = "Send to Job Editor"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 31)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = CType(resources.GetObject("ToolStripButton1.Image"), System.Drawing.Image)
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripButton1.Text = "Help"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 490)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(941, 22)
        Me.StatusStrip1.SizingGrip = False
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
        Me.lblMakeModel.AutoSize = True
        Me.lblMakeModel.Location = New System.Drawing.Point(16, 85)
        Me.lblMakeModel.Name = "lblMakeModel"
        Me.lblMakeModel.Size = New System.Drawing.Size(87, 13)
        Me.lblMakeModel.TabIndex = 11
        Me.lblMakeModel.Text = "Make and Model"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_About
        Me.PictureBox1.Location = New System.Drawing.Point(12, 34)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(91, 48)
        Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox1.TabIndex = 39
        Me.PictureBox1.TabStop = False
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
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
        Me.pnInertia.Controls.Add(Me.lblInertia)
        Me.pnInertia.Controls.Add(Me.lblinertiaUnit)
        Me.pnInertia.Controls.Add(Me.tbInertia)
        Me.pnInertia.Location = New System.Drawing.Point(246, 107)
        Me.pnInertia.Name = "pnInertia"
        Me.pnInertia.Size = New System.Drawing.Size(261, 30)
        Me.pnInertia.TabIndex = 3
        '
        'btnEmMapOpenHi
        '
        Me.btnEmMapOpenHi.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnEmMapOpenHi.Location = New System.Drawing.Point(441, 87)
        Me.btnEmMapOpenHi.Name = "btnEmMapOpenHi"
        Me.btnEmMapOpenHi.Size = New System.Drawing.Size(24, 24)
        Me.btnEmMapOpenHi.TabIndex = 42
        Me.btnEmMapOpenHi.TabStop = False
        Me.btnEmMapOpenHi.UseVisualStyleBackColor = True
        '
        'btnBrowseEmMapHi
        '
        Me.btnBrowseEmMapHi.Image = CType(resources.GetObject("btnBrowseEmMapHi.Image"), System.Drawing.Image)
        Me.btnBrowseEmMapHi.Location = New System.Drawing.Point(419, 87)
        Me.btnBrowseEmMapHi.Name = "btnBrowseEmMapHi"
        Me.btnBrowseEmMapHi.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseEmMapHi.TabIndex = 41
        Me.btnBrowseEmMapHi.TabStop = False
        Me.btnBrowseEmMapHi.UseVisualStyleBackColor = True
        '
        'lblPowerMapHi
        '
        Me.lblPowerMapHi.AutoSize = True
        Me.lblPowerMapHi.Location = New System.Drawing.Point(5, 74)
        Me.lblPowerMapHi.Name = "lblPowerMapHi"
        Me.lblPowerMapHi.Size = New System.Drawing.Size(163, 13)
        Me.lblPowerMapHi.TabIndex = 43
        Me.lblPowerMapHi.Text = "Electric Power Consumption Map"
        '
        'tbMapHi
        '
        Me.tbMapHi.Location = New System.Drawing.Point(5, 90)
        Me.tbMapHi.Name = "tbMapHi"
        Me.tbMapHi.Size = New System.Drawing.Size(406, 20)
        Me.tbMapHi.TabIndex = 40
        '
        'btnMaxTorqueCurveOpenHi
        '
        Me.btnMaxTorqueCurveOpenHi.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnMaxTorqueCurveOpenHi.Location = New System.Drawing.Point(441, 48)
        Me.btnMaxTorqueCurveOpenHi.Name = "btnMaxTorqueCurveOpenHi"
        Me.btnMaxTorqueCurveOpenHi.Size = New System.Drawing.Size(24, 24)
        Me.btnMaxTorqueCurveOpenHi.TabIndex = 46
        Me.btnMaxTorqueCurveOpenHi.TabStop = False
        Me.btnMaxTorqueCurveOpenHi.UseVisualStyleBackColor = True
        '
        'btnBrowseMaxTorqueHi
        '
        Me.btnBrowseMaxTorqueHi.Image = CType(resources.GetObject("btnBrowseMaxTorqueHi.Image"), System.Drawing.Image)
        Me.btnBrowseMaxTorqueHi.Location = New System.Drawing.Point(419, 48)
        Me.btnBrowseMaxTorqueHi.Name = "btnBrowseMaxTorqueHi"
        Me.btnBrowseMaxTorqueHi.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseMaxTorqueHi.TabIndex = 45
        Me.btnBrowseMaxTorqueHi.TabStop = False
        Me.btnBrowseMaxTorqueHi.UseVisualStyleBackColor = True
        '
        'lblMaxTorqueHi
        '
        Me.lblMaxTorqueHi.AutoSize = True
        Me.lblMaxTorqueHi.Location = New System.Drawing.Point(5, 34)
        Me.lblMaxTorqueHi.Name = "lblMaxTorqueHi"
        Me.lblMaxTorqueHi.Size = New System.Drawing.Size(222, 13)
        Me.lblMaxTorqueHi.TabIndex = 47
        Me.lblMaxTorqueHi.Text = "Max Drive and Max Generation Torque Curve"
        '
        'tbMaxTorqueHi
        '
        Me.tbMaxTorqueHi.Location = New System.Drawing.Point(5, 50)
        Me.tbMaxTorqueHi.Name = "tbMaxTorqueHi"
        Me.tbMaxTorqueHi.Size = New System.Drawing.Size(406, 20)
        Me.tbMaxTorqueHi.TabIndex = 44
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BackColor = System.Drawing.Color.WhiteSmoke
        Me.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!)
        Me.lblTitle.Location = New System.Drawing.Point(109, 48)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(192, 31)
        Me.lblTitle.TabIndex = 48
        Me.lblTitle.Text = "Electric Machine"
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(517, 28)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(382, 266)
        Me.PicBox.TabIndex = 49
        Me.PicBox.TabStop = False
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.lblContTq)
        Me.Panel1.Controls.Add(Me.lblContTqUnit)
        Me.Panel1.Controls.Add(Me.tbContTqLo)
        Me.Panel1.Location = New System.Drawing.Point(5, 120)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(221, 30)
        Me.Panel1.TabIndex = 25
        '
        'lblContTq
        '
        Me.lblContTq.AutoSize = True
        Me.lblContTq.Location = New System.Drawing.Point(3, 7)
        Me.lblContTq.Name = "lblContTq"
        Me.lblContTq.Size = New System.Drawing.Size(100, 13)
        Me.lblContTq.TabIndex = 0
        Me.lblContTq.Text = "Continuous Torque:"
        '
        'lblContTqUnit
        '
        Me.lblContTqUnit.AutoSize = True
        Me.lblContTqUnit.Location = New System.Drawing.Point(182, 8)
        Me.lblContTqUnit.Name = "lblContTqUnit"
        Me.lblContTqUnit.Size = New System.Drawing.Size(29, 13)
        Me.lblContTqUnit.TabIndex = 24
        Me.lblContTqUnit.Text = "[Nm]"
        '
        'tbContTqLo
        '
        Me.tbContTqLo.Location = New System.Drawing.Point(127, 5)
        Me.tbContTqLo.Name = "tbContTqLo"
        Me.tbContTqLo.Size = New System.Drawing.Size(49, 20)
        Me.tbContTqLo.TabIndex = 3
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblOvlTime)
        Me.Panel2.Controls.Add(Me.lblOvltimeUnit)
        Me.Panel2.Controls.Add(Me.tbOvlTimeLo)
        Me.Panel2.Location = New System.Drawing.Point(5, 181)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(221, 30)
        Me.Panel2.TabIndex = 25
        '
        'lblOvlTime
        '
        Me.lblOvlTime.AutoSize = True
        Me.lblOvlTime.Location = New System.Drawing.Point(3, 7)
        Me.lblOvlTime.Name = "lblOvlTime"
        Me.lblOvlTime.Size = New System.Drawing.Size(96, 13)
        Me.lblOvlTime.TabIndex = 0
        Me.lblOvlTime.Text = "Overload Duration:"
        '
        'lblOvltimeUnit
        '
        Me.lblOvltimeUnit.AutoSize = True
        Me.lblOvltimeUnit.Location = New System.Drawing.Point(180, 7)
        Me.lblOvltimeUnit.Name = "lblOvltimeUnit"
        Me.lblOvltimeUnit.Size = New System.Drawing.Size(18, 13)
        Me.lblOvltimeUnit.TabIndex = 24
        Me.lblOvltimeUnit.Text = "[s]"
        '
        'tbOvlTimeLo
        '
        Me.tbOvlTimeLo.Location = New System.Drawing.Point(127, 5)
        Me.tbOvlTimeLo.Name = "tbOvlTimeLo"
        Me.tbOvlTimeLo.Size = New System.Drawing.Size(49, 20)
        Me.tbOvlTimeLo.TabIndex = 3
        '
        'pnThermalOverloadRecovery
        '
        Me.pnThermalOverloadRecovery.Controls.Add(Me.lblOvlRecovery)
        Me.pnThermalOverloadRecovery.Controls.Add(Me.lblOvlRecoveryFactorUnit)
        Me.pnThermalOverloadRecovery.Controls.Add(Me.tbOverloadRecoveryFactor)
        Me.pnThermalOverloadRecovery.Location = New System.Drawing.Point(246, 139)
        Me.pnThermalOverloadRecovery.Name = "pnThermalOverloadRecovery"
        Me.pnThermalOverloadRecovery.Size = New System.Drawing.Size(261, 30)
        Me.pnThermalOverloadRecovery.TabIndex = 26
        '
        'lblOvlRecovery
        '
        Me.lblOvlRecovery.AutoSize = True
        Me.lblOvlRecovery.Location = New System.Drawing.Point(2, 8)
        Me.lblOvlRecovery.Name = "lblOvlRecovery"
        Me.lblOvlRecovery.Size = New System.Drawing.Size(176, 13)
        Me.lblOvlRecovery.TabIndex = 0
        Me.lblOvlRecovery.Text = "Thermal Overload Recovery Factor:"
        '
        'lblOvlRecoveryFactorUnit
        '
        Me.lblOvlRecoveryFactorUnit.AutoSize = True
        Me.lblOvlRecoveryFactorUnit.Location = New System.Drawing.Point(223, 8)
        Me.lblOvlRecoveryFactorUnit.Name = "lblOvlRecoveryFactorUnit"
        Me.lblOvlRecoveryFactorUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblOvlRecoveryFactorUnit.TabIndex = 24
        Me.lblOvlRecoveryFactorUnit.Text = "[-]"
        '
        'tbOverloadRecoveryFactor
        '
        Me.tbOverloadRecoveryFactor.Location = New System.Drawing.Point(168, 4)
        Me.tbOverloadRecoveryFactor.Name = "tbOverloadRecoveryFactor"
        Me.tbOverloadRecoveryFactor.Size = New System.Drawing.Size(49, 20)
        Me.tbOverloadRecoveryFactor.TabIndex = 3
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.lblRatedSpeed)
        Me.Panel4.Controls.Add(Me.lblRatedSpeedUnit)
        Me.Panel4.Controls.Add(Me.tbRatedSpeedLo)
        Me.Panel4.Location = New System.Drawing.Point(226, 120)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(261, 30)
        Me.Panel4.TabIndex = 26
        '
        'lblRatedSpeed
        '
        Me.lblRatedSpeed.AutoSize = True
        Me.lblRatedSpeed.Location = New System.Drawing.Point(3, 7)
        Me.lblRatedSpeed.Name = "lblRatedSpeed"
        Me.lblRatedSpeed.Size = New System.Drawing.Size(158, 13)
        Me.lblRatedSpeed.TabIndex = 0
        Me.lblRatedSpeed.Text = "Test Speed Continuous Torque:"
        '
        'lblRatedSpeedUnit
        '
        Me.lblRatedSpeedUnit.AutoSize = True
        Me.lblRatedSpeedUnit.Location = New System.Drawing.Point(229, 7)
        Me.lblRatedSpeedUnit.Name = "lblRatedSpeedUnit"
        Me.lblRatedSpeedUnit.Size = New System.Drawing.Size(30, 13)
        Me.lblRatedSpeedUnit.TabIndex = 24
        Me.lblRatedSpeedUnit.Text = "[rpm]"
        '
        'tbRatedSpeedLo
        '
        Me.tbRatedSpeedLo.Location = New System.Drawing.Point(177, 5)
        Me.tbRatedSpeedLo.Name = "tbRatedSpeedLo"
        Me.tbRatedSpeedLo.Size = New System.Drawing.Size(49, 20)
        Me.tbRatedSpeedLo.TabIndex = 3
        '
        'pnOverloadTq
        '
        Me.pnOverloadTq.Controls.Add(Me.lblOverloadTq)
        Me.pnOverloadTq.Controls.Add(Me.lblOverloadTqUnit)
        Me.pnOverloadTq.Controls.Add(Me.tbOverloadTqLo)
        Me.pnOverloadTq.Location = New System.Drawing.Point(5, 150)
        Me.pnOverloadTq.Name = "pnOverloadTq"
        Me.pnOverloadTq.Size = New System.Drawing.Size(221, 30)
        Me.pnOverloadTq.TabIndex = 26
        '
        'lblOverloadTq
        '
        Me.lblOverloadTq.AutoSize = True
        Me.lblOverloadTq.Location = New System.Drawing.Point(3, 7)
        Me.lblOverloadTq.Name = "lblOverloadTq"
        Me.lblOverloadTq.Size = New System.Drawing.Size(90, 13)
        Me.lblOverloadTq.TabIndex = 0
        Me.lblOverloadTq.Text = "Overload Torque:"
        '
        'lblOverloadTqUnit
        '
        Me.lblOverloadTqUnit.AutoSize = True
        Me.lblOverloadTqUnit.Location = New System.Drawing.Point(182, 8)
        Me.lblOverloadTqUnit.Name = "lblOverloadTqUnit"
        Me.lblOverloadTqUnit.Size = New System.Drawing.Size(29, 13)
        Me.lblOverloadTqUnit.TabIndex = 24
        Me.lblOverloadTqUnit.Text = "[Nm]"
        '
        'tbOverloadTqLo
        '
        Me.tbOverloadTqLo.Location = New System.Drawing.Point(127, 5)
        Me.tbOverloadTqLo.Name = "tbOverloadTqLo"
        Me.tbOverloadTqLo.Size = New System.Drawing.Size(49, 20)
        Me.tbOverloadTqLo.TabIndex = 3
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.lblOverloadSpeed)
        Me.Panel6.Controls.Add(Me.lblOverloadSpeedUnit)
        Me.Panel6.Controls.Add(Me.tbOvlSpeedLo)
        Me.Panel6.Location = New System.Drawing.Point(226, 150)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(261, 30)
        Me.Panel6.TabIndex = 27
        '
        'lblOverloadSpeed
        '
        Me.lblOverloadSpeed.AutoSize = True
        Me.lblOverloadSpeed.Location = New System.Drawing.Point(3, 7)
        Me.lblOverloadSpeed.Name = "lblOverloadSpeed"
        Me.lblOverloadSpeed.Size = New System.Drawing.Size(148, 13)
        Me.lblOverloadSpeed.TabIndex = 0
        Me.lblOverloadSpeed.Text = "Test Speed Overload Torque:"
        '
        'lblOverloadSpeedUnit
        '
        Me.lblOverloadSpeedUnit.AutoSize = True
        Me.lblOverloadSpeedUnit.Location = New System.Drawing.Point(229, 7)
        Me.lblOverloadSpeedUnit.Name = "lblOverloadSpeedUnit"
        Me.lblOverloadSpeedUnit.Size = New System.Drawing.Size(30, 13)
        Me.lblOverloadSpeedUnit.TabIndex = 24
        Me.lblOverloadSpeedUnit.Text = "[rpm]"
        '
        'tbOvlSpeedLo
        '
        Me.tbOvlSpeedLo.Location = New System.Drawing.Point(177, 5)
        Me.tbOvlSpeedLo.Name = "tbOvlSpeedLo"
        Me.tbOvlSpeedLo.Size = New System.Drawing.Size(49, 20)
        Me.tbOvlSpeedLo.TabIndex = 3
        '
        'lblVoltageHi
        '
        Me.lblVoltageHi.AutoSize = True
        Me.lblVoltageHi.Location = New System.Drawing.Point(11, 9)
        Me.lblVoltageHi.Name = "lblVoltageHi"
        Me.lblVoltageHi.Size = New System.Drawing.Size(43, 13)
        Me.lblVoltageHi.TabIndex = 48
        Me.lblVoltageHi.Text = "Voltage"
        '
        'lblVoltageHiUnit
        '
        Me.lblVoltageHiUnit.AutoSize = True
        Me.lblVoltageHiUnit.Location = New System.Drawing.Point(188, 10)
        Me.lblVoltageHiUnit.Name = "lblVoltageHiUnit"
        Me.lblVoltageHiUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblVoltageHiUnit.TabIndex = 50
        Me.lblVoltageHiUnit.Text = "[V]"
        '
        'tbVoltageHi
        '
        Me.tbVoltageHi.Location = New System.Drawing.Point(135, 7)
        Me.tbVoltageHi.Name = "tbVoltageHi"
        Me.tbVoltageHi.Size = New System.Drawing.Size(49, 20)
        Me.tbVoltageHi.TabIndex = 49
        '
        'lblVoltageLow
        '
        Me.lblVoltageLow.AutoSize = True
        Me.lblVoltageLow.Location = New System.Drawing.Point(11, 9)
        Me.lblVoltageLow.Name = "lblVoltageLow"
        Me.lblVoltageLow.Size = New System.Drawing.Size(43, 13)
        Me.lblVoltageLow.TabIndex = 48
        Me.lblVoltageLow.Text = "Voltage"
        '
        'lblVoltageLowUnit
        '
        Me.lblVoltageLowUnit.AutoSize = True
        Me.lblVoltageLowUnit.Location = New System.Drawing.Point(188, 10)
        Me.lblVoltageLowUnit.Name = "lblVoltageLowUnit"
        Me.lblVoltageLowUnit.Size = New System.Drawing.Size(20, 13)
        Me.lblVoltageLowUnit.TabIndex = 50
        Me.lblVoltageLowUnit.Text = "[V]"
        '
        'tbVoltageLow
        '
        Me.tbVoltageLow.Location = New System.Drawing.Point(135, 7)
        Me.tbVoltageLow.Name = "tbVoltageLow"
        Me.tbVoltageLow.Size = New System.Drawing.Size(49, 20)
        Me.tbVoltageLow.TabIndex = 49
        '
        'lblMaxTorqueLow
        '
        Me.lblMaxTorqueLow.AutoSize = True
        Me.lblMaxTorqueLow.Location = New System.Drawing.Point(5, 34)
        Me.lblMaxTorqueLow.Name = "lblMaxTorqueLow"
        Me.lblMaxTorqueLow.Size = New System.Drawing.Size(222, 13)
        Me.lblMaxTorqueLow.TabIndex = 47
        Me.lblMaxTorqueLow.Text = "Max Drive and Max Generation Torque Curve"
        '
        'tbDragTorque
        '
        Me.tbDragTorque.Location = New System.Drawing.Point(3, 22)
        Me.tbDragTorque.Name = "tbDragTorque"
        Me.tbDragTorque.Size = New System.Drawing.Size(406, 20)
        Me.tbDragTorque.TabIndex = 5
        '
        'lblDragTorque
        '
        Me.lblDragTorque.AutoSize = True
        Me.lblDragTorque.Location = New System.Drawing.Point(3, 5)
        Me.lblDragTorque.Name = "lblDragTorque"
        Me.lblDragTorque.Size = New System.Drawing.Size(98, 13)
        Me.lblDragTorque.TabIndex = 38
        Me.lblDragTorque.Text = "Drag Torque Curve"
        '
        'btnBrowseDragCurve
        '
        Me.btnBrowseDragCurve.Image = CType(resources.GetObject("btnBrowseDragCurve.Image"), System.Drawing.Image)
        Me.btnBrowseDragCurve.Location = New System.Drawing.Point(417, 18)
        Me.btnBrowseDragCurve.Name = "btnBrowseDragCurve"
        Me.btnBrowseDragCurve.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseDragCurve.TabIndex = 6
        Me.btnBrowseDragCurve.TabStop = False
        Me.btnBrowseDragCurve.UseVisualStyleBackColor = True
        '
        'btnDragCurveOpen
        '
        Me.btnDragCurveOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnDragCurveOpen.Location = New System.Drawing.Point(441, 18)
        Me.btnDragCurveOpen.Name = "btnDragCurveOpen"
        Me.btnDragCurveOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnDragCurveOpen.TabIndex = 7
        Me.btnDragCurveOpen.TabStop = False
        Me.btnDragCurveOpen.UseVisualStyleBackColor = True
        '
        'tbMapLow
        '
        Me.tbMapLow.Location = New System.Drawing.Point(5, 90)
        Me.tbMapLow.Name = "tbMapLow"
        Me.tbMapLow.Size = New System.Drawing.Size(406, 20)
        Me.tbMapLow.TabIndex = 40
        '
        'lblPowerMapLow
        '
        Me.lblPowerMapLow.AutoSize = True
        Me.lblPowerMapLow.Location = New System.Drawing.Point(5, 74)
        Me.lblPowerMapLow.Name = "lblPowerMapLow"
        Me.lblPowerMapLow.Size = New System.Drawing.Size(163, 13)
        Me.lblPowerMapLow.TabIndex = 43
        Me.lblPowerMapLow.Text = "Electric Power Consumption Map"
        '
        'btnBrowseEmMapLow
        '
        Me.btnBrowseEmMapLow.Image = CType(resources.GetObject("btnBrowseEmMapLow.Image"), System.Drawing.Image)
        Me.btnBrowseEmMapLow.Location = New System.Drawing.Point(419, 87)
        Me.btnBrowseEmMapLow.Name = "btnBrowseEmMapLow"
        Me.btnBrowseEmMapLow.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseEmMapLow.TabIndex = 41
        Me.btnBrowseEmMapLow.TabStop = False
        Me.btnBrowseEmMapLow.UseVisualStyleBackColor = True
        '
        'btnEmMapOpenLow
        '
        Me.btnEmMapOpenLow.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnEmMapOpenLow.Location = New System.Drawing.Point(441, 87)
        Me.btnEmMapOpenLow.Name = "btnEmMapOpenLow"
        Me.btnEmMapOpenLow.Size = New System.Drawing.Size(24, 24)
        Me.btnEmMapOpenLow.TabIndex = 42
        Me.btnEmMapOpenLow.TabStop = False
        Me.btnEmMapOpenLow.UseVisualStyleBackColor = True
        '
        'tbMaxTorqueLow
        '
        Me.tbMaxTorqueLow.Location = New System.Drawing.Point(5, 50)
        Me.tbMaxTorqueLow.Name = "tbMaxTorqueLow"
        Me.tbMaxTorqueLow.Size = New System.Drawing.Size(406, 20)
        Me.tbMaxTorqueLow.TabIndex = 44
        '
        'btnMaxTorqueCurveOpenLow
        '
        Me.btnMaxTorqueCurveOpenLow.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnMaxTorqueCurveOpenLow.Location = New System.Drawing.Point(441, 48)
        Me.btnMaxTorqueCurveOpenLow.Name = "btnMaxTorqueCurveOpenLow"
        Me.btnMaxTorqueCurveOpenLow.Size = New System.Drawing.Size(24, 24)
        Me.btnMaxTorqueCurveOpenLow.TabIndex = 46
        Me.btnMaxTorqueCurveOpenLow.TabStop = False
        Me.btnMaxTorqueCurveOpenLow.UseVisualStyleBackColor = True
        '
        'btnBrowseMaxTorqueLow
        '
        Me.btnBrowseMaxTorqueLow.Image = CType(resources.GetObject("btnBrowseMaxTorqueLow.Image"), System.Drawing.Image)
        Me.btnBrowseMaxTorqueLow.Location = New System.Drawing.Point(419, 48)
        Me.btnBrowseMaxTorqueLow.Name = "btnBrowseMaxTorqueLow"
        Me.btnBrowseMaxTorqueLow.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseMaxTorqueLow.TabIndex = 45
        Me.btnBrowseMaxTorqueLow.TabStop = False
        Me.btnBrowseMaxTorqueLow.UseVisualStyleBackColor = True
        '
        'tcVoltageLevels
        '
        Me.tcVoltageLevels.Controls.Add(Me.tpVoltageLow)
        Me.tcVoltageLevels.Controls.Add(Me.tpVoltageHigh)
        Me.tcVoltageLevels.Location = New System.Drawing.Point(12, 223)
        Me.tcVoltageLevels.Margin = New System.Windows.Forms.Padding(2)
        Me.tcVoltageLevels.Name = "tcVoltageLevels"
        Me.tcVoltageLevels.SelectedIndex = 0
        Me.tcVoltageLevels.Size = New System.Drawing.Size(501, 246)
        Me.tcVoltageLevels.TabIndex = 52
        '
        'tpVoltageLow
        '
        Me.tpVoltageLow.Controls.Add(Me.lblVoltageLow)
        Me.tpVoltageLow.Controls.Add(Me.tbVoltageLow)
        Me.tpVoltageLow.Controls.Add(Me.lblVoltageLowUnit)
        Me.tpVoltageLow.Controls.Add(Me.btnBrowseMaxTorqueLow)
        Me.tpVoltageLow.Controls.Add(Me.Panel6)
        Me.tpVoltageLow.Controls.Add(Me.btnMaxTorqueCurveOpenLow)
        Me.tpVoltageLow.Controls.Add(Me.pnOverloadTq)
        Me.tpVoltageLow.Controls.Add(Me.lblMaxTorqueLow)
        Me.tpVoltageLow.Controls.Add(Me.Panel4)
        Me.tpVoltageLow.Controls.Add(Me.tbMaxTorqueLow)
        Me.tpVoltageLow.Controls.Add(Me.Panel2)
        Me.tpVoltageLow.Controls.Add(Me.btnEmMapOpenLow)
        Me.tpVoltageLow.Controls.Add(Me.Panel1)
        Me.tpVoltageLow.Controls.Add(Me.btnBrowseEmMapLow)
        Me.tpVoltageLow.Controls.Add(Me.lblPowerMapLow)
        Me.tpVoltageLow.Controls.Add(Me.tbMapLow)
        Me.tpVoltageLow.Location = New System.Drawing.Point(4, 22)
        Me.tpVoltageLow.Margin = New System.Windows.Forms.Padding(2)
        Me.tpVoltageLow.Name = "tpVoltageLow"
        Me.tpVoltageLow.Padding = New System.Windows.Forms.Padding(2)
        Me.tpVoltageLow.Size = New System.Drawing.Size(493, 220)
        Me.tpVoltageLow.TabIndex = 0
        Me.tpVoltageLow.Text = "Voltage Level Low"
        Me.tpVoltageLow.UseVisualStyleBackColor = True
        '
        'tpVoltageHigh
        '
        Me.tpVoltageHigh.Controls.Add(Me.Panel5)
        Me.tpVoltageHigh.Controls.Add(Me.Panel7)
        Me.tpVoltageHigh.Controls.Add(Me.Panel8)
        Me.tpVoltageHigh.Controls.Add(Me.Panel9)
        Me.tpVoltageHigh.Controls.Add(Me.Panel10)
        Me.tpVoltageHigh.Controls.Add(Me.lblVoltageHi)
        Me.tpVoltageHigh.Controls.Add(Me.tbMaxTorqueHi)
        Me.tpVoltageHigh.Controls.Add(Me.lblVoltageHiUnit)
        Me.tpVoltageHigh.Controls.Add(Me.btnBrowseMaxTorqueHi)
        Me.tpVoltageHigh.Controls.Add(Me.tbVoltageHi)
        Me.tpVoltageHigh.Controls.Add(Me.btnMaxTorqueCurveOpenHi)
        Me.tpVoltageHigh.Controls.Add(Me.lblMaxTorqueHi)
        Me.tpVoltageHigh.Controls.Add(Me.btnEmMapOpenHi)
        Me.tpVoltageHigh.Controls.Add(Me.btnBrowseEmMapHi)
        Me.tpVoltageHigh.Controls.Add(Me.lblPowerMapHi)
        Me.tpVoltageHigh.Controls.Add(Me.tbMapHi)
        Me.tpVoltageHigh.Location = New System.Drawing.Point(4, 22)
        Me.tpVoltageHigh.Margin = New System.Windows.Forms.Padding(2)
        Me.tpVoltageHigh.Name = "tpVoltageHigh"
        Me.tpVoltageHigh.Padding = New System.Windows.Forms.Padding(2)
        Me.tpVoltageHigh.Size = New System.Drawing.Size(493, 220)
        Me.tpVoltageHigh.TabIndex = 1
        Me.tpVoltageHigh.Text = "Voltage Level High"
        Me.tpVoltageHigh.UseVisualStyleBackColor = True
        '
        'Panel5
        '
        Me.Panel5.Controls.Add(Me.Label1)
        Me.Panel5.Controls.Add(Me.Label2)
        Me.Panel5.Controls.Add(Me.tbOvlSpeedHi)
        Me.Panel5.Location = New System.Drawing.Point(226, 150)
        Me.Panel5.Name = "Panel5"
        Me.Panel5.Size = New System.Drawing.Size(261, 30)
        Me.Panel5.TabIndex = 55
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(3, 7)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(148, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Test Speed Overload Torque:"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(229, 7)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(30, 13)
        Me.Label2.TabIndex = 24
        Me.Label2.Text = "[rpm]"
        '
        'tbOvlSpeedHi
        '
        Me.tbOvlSpeedHi.Location = New System.Drawing.Point(177, 5)
        Me.tbOvlSpeedHi.Name = "tbOvlSpeedHi"
        Me.tbOvlSpeedHi.Size = New System.Drawing.Size(49, 20)
        Me.tbOvlSpeedHi.TabIndex = 3
        '
        'Panel7
        '
        Me.Panel7.Controls.Add(Me.Label3)
        Me.Panel7.Controls.Add(Me.Label4)
        Me.Panel7.Controls.Add(Me.tbOverloadTqHi)
        Me.Panel7.Location = New System.Drawing.Point(5, 150)
        Me.Panel7.Name = "Panel7"
        Me.Panel7.Size = New System.Drawing.Size(221, 30)
        Me.Panel7.TabIndex = 53
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(3, 7)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(90, 13)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = "Overload Torque:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(182, 8)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(29, 13)
        Me.Label4.TabIndex = 24
        Me.Label4.Text = "[Nm]"
        '
        'tbOverloadTqHi
        '
        Me.tbOverloadTqHi.Location = New System.Drawing.Point(127, 5)
        Me.tbOverloadTqHi.Name = "tbOverloadTqHi"
        Me.tbOverloadTqHi.Size = New System.Drawing.Size(49, 20)
        Me.tbOverloadTqHi.TabIndex = 3
        '
        'Panel8
        '
        Me.Panel8.Controls.Add(Me.Label5)
        Me.Panel8.Controls.Add(Me.Label6)
        Me.Panel8.Controls.Add(Me.tbRatedSpeedHi)
        Me.Panel8.Location = New System.Drawing.Point(226, 120)
        Me.Panel8.Name = "Panel8"
        Me.Panel8.Size = New System.Drawing.Size(261, 30)
        Me.Panel8.TabIndex = 54
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(3, 7)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(158, 13)
        Me.Label5.TabIndex = 0
        Me.Label5.Text = "Test Speed Continuous Torque:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(229, 7)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(30, 13)
        Me.Label6.TabIndex = 24
        Me.Label6.Text = "[rpm]"
        '
        'tbRatedSpeedHi
        '
        Me.tbRatedSpeedHi.Location = New System.Drawing.Point(177, 5)
        Me.tbRatedSpeedHi.Name = "tbRatedSpeedHi"
        Me.tbRatedSpeedHi.Size = New System.Drawing.Size(49, 20)
        Me.tbRatedSpeedHi.TabIndex = 3
        '
        'Panel9
        '
        Me.Panel9.Controls.Add(Me.Label7)
        Me.Panel9.Controls.Add(Me.Label8)
        Me.Panel9.Controls.Add(Me.tbOvlTimeHi)
        Me.Panel9.Location = New System.Drawing.Point(5, 181)
        Me.Panel9.Name = "Panel9"
        Me.Panel9.Size = New System.Drawing.Size(221, 30)
        Me.Panel9.TabIndex = 51
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(3, 7)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(96, 13)
        Me.Label7.TabIndex = 0
        Me.Label7.Text = "Overload Duration:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(180, 7)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(18, 13)
        Me.Label8.TabIndex = 24
        Me.Label8.Text = "[s]"
        '
        'tbOvlTimeHi
        '
        Me.tbOvlTimeHi.Location = New System.Drawing.Point(127, 5)
        Me.tbOvlTimeHi.Name = "tbOvlTimeHi"
        Me.tbOvlTimeHi.Size = New System.Drawing.Size(49, 20)
        Me.tbOvlTimeHi.TabIndex = 3
        '
        'Panel10
        '
        Me.Panel10.Controls.Add(Me.Label9)
        Me.Panel10.Controls.Add(Me.Label10)
        Me.Panel10.Controls.Add(Me.tbContTqHi)
        Me.Panel10.Location = New System.Drawing.Point(5, 120)
        Me.Panel10.Name = "Panel10"
        Me.Panel10.Size = New System.Drawing.Size(221, 30)
        Me.Panel10.TabIndex = 52
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(3, 7)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(100, 13)
        Me.Label9.TabIndex = 0
        Me.Label9.Text = "Continuous Torque:"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(182, 8)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(29, 13)
        Me.Label10.TabIndex = 24
        Me.Label10.Text = "[Nm]"
        '
        'tbContTqHi
        '
        Me.tbContTqHi.Location = New System.Drawing.Point(127, 5)
        Me.tbContTqHi.Name = "tbContTqHi"
        Me.tbContTqHi.Size = New System.Drawing.Size(49, 20)
        Me.tbContTqHi.TabIndex = 3
        '
        'pnDragCurve
        '
        Me.pnDragCurve.Controls.Add(Me.tbDragTorque)
        Me.pnDragCurve.Controls.Add(Me.btnBrowseDragCurve)
        Me.pnDragCurve.Controls.Add(Me.btnDragCurveOpen)
        Me.pnDragCurve.Controls.Add(Me.lblDragTorque)
        Me.pnDragCurve.Location = New System.Drawing.Point(12, 172)
        Me.pnDragCurve.Name = "pnDragCurve"
        Me.pnDragCurve.Size = New System.Drawing.Size(495, 46)
        Me.pnDragCurve.TabIndex = 53
        '
        'pnRatedPower
        '
        Me.pnRatedPower.Controls.Add(Me.lblRatedPower)
        Me.pnRatedPower.Controls.Add(Me.lblRatedPowerUnit)
        Me.pnRatedPower.Controls.Add(Me.tbRatedPower)
        Me.pnRatedPower.Location = New System.Drawing.Point(12, 139)
        Me.pnRatedPower.Name = "pnRatedPower"
        Me.pnRatedPower.Size = New System.Drawing.Size(230, 30)
        Me.pnRatedPower.TabIndex = 25
        '
        'lblRatedPower
        '
        Me.lblRatedPower.AutoSize = True
        Me.lblRatedPower.Location = New System.Drawing.Point(3, 7)
        Me.lblRatedPower.Name = "lblRatedPower"
        Me.lblRatedPower.Size = New System.Drawing.Size(69, 13)
        Me.lblRatedPower.TabIndex = 0
        Me.lblRatedPower.Text = "Rated Power"
        '
        'lblRatedPowerUnit
        '
        Me.lblRatedPowerUnit.AutoSize = True
        Me.lblRatedPowerUnit.Location = New System.Drawing.Point(199, 8)
        Me.lblRatedPowerUnit.Name = "lblRatedPowerUnit"
        Me.lblRatedPowerUnit.Size = New System.Drawing.Size(30, 13)
        Me.lblRatedPowerUnit.TabIndex = 24
        Me.lblRatedPowerUnit.Text = "[kW]"
        '
        'tbRatedPower
        '
        Me.tbRatedPower.Location = New System.Drawing.Point(144, 5)
        Me.tbRatedPower.Name = "tbRatedPower"
        Me.tbRatedPower.Size = New System.Drawing.Size(49, 20)
        Me.tbRatedPower.TabIndex = 3
        '
        'pnElectricMachineType
        '
        Me.pnElectricMachineType.Controls.Add(Me.cbEmType)
        Me.pnElectricMachineType.Controls.Add(Me.lblEmType)
        Me.pnElectricMachineType.Location = New System.Drawing.Point(12, 107)
        Me.pnElectricMachineType.Name = "pnElectricMachineType"
        Me.pnElectricMachineType.Size = New System.Drawing.Size(230, 30)
        Me.pnElectricMachineType.TabIndex = 54
        '
        'cbEmType
        '
        Me.cbEmType.FormattingEnabled = True
        Me.cbEmType.Location = New System.Drawing.Point(97, 6)
        Me.cbEmType.Name = "cbEmType"
        Me.cbEmType.Size = New System.Drawing.Size(130, 21)
        Me.cbEmType.TabIndex = 26
        '
        'lblEmType
        '
        Me.lblEmType.AutoSize = True
        Me.lblEmType.Location = New System.Drawing.Point(4, 8)
        Me.lblEmType.Name = "lblEmType"
        Me.lblEmType.Size = New System.Drawing.Size(50, 13)
        Me.lblEmType.TabIndex = 25
        Me.lblEmType.Text = "EM Type"
        '
        'ElectricMotorForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(941, 512)
        Me.Controls.Add(Me.pnElectricMachineType)
        Me.Controls.Add(Me.pnRatedPower)
        Me.Controls.Add(Me.pnDragCurve)
        Me.Controls.Add(Me.tcVoltageLevels)
        Me.Controls.Add(Me.pnThermalOverloadRecovery)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.pnInertia)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.lblMakeModel)
        Me.Controls.Add(Me.tbMakeModel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.Name = "ElectricMotorForm"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Electric Machine"
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
        Me.pnThermalOverloadRecovery.ResumeLayout(false)
        Me.pnThermalOverloadRecovery.PerformLayout
        Me.Panel4.ResumeLayout(false)
        Me.Panel4.PerformLayout
        Me.pnOverloadTq.ResumeLayout(false)
        Me.pnOverloadTq.PerformLayout
        Me.Panel6.ResumeLayout(false)
        Me.Panel6.PerformLayout
        Me.tcVoltageLevels.ResumeLayout(false)
        Me.tpVoltageLow.ResumeLayout(false)
        Me.tpVoltageLow.PerformLayout
        Me.tpVoltageHigh.ResumeLayout(false)
        Me.tpVoltageHigh.PerformLayout
        Me.Panel5.ResumeLayout(false)
        Me.Panel5.PerformLayout
        Me.Panel7.ResumeLayout(false)
        Me.Panel7.PerformLayout
        Me.Panel8.ResumeLayout(false)
        Me.Panel8.PerformLayout
        Me.Panel9.ResumeLayout(false)
        Me.Panel9.PerformLayout
        Me.Panel10.ResumeLayout(false)
        Me.Panel10.PerformLayout
        Me.pnDragCurve.ResumeLayout(false)
        Me.pnDragCurve.PerformLayout
        Me.pnRatedPower.ResumeLayout(false)
        Me.pnRatedPower.PerformLayout
        Me.pnElectricMachineType.ResumeLayout(false)
        Me.pnElectricMachineType.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents tbInertia As TextBox
    Friend WithEvents lblinertiaUnit As Label
    Friend WithEvents lblInertia As Label
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
    Friend WithEvents btnEmMapOpenHi As Button
    Friend WithEvents btnBrowseEmMapHi As Button
    Friend WithEvents lblPowerMapHi As Label
    Friend WithEvents tbMapHi As TextBox
    Friend WithEvents btnMaxTorqueCurveOpenHi As Button
    Friend WithEvents btnBrowseMaxTorqueHi As Button
    Friend WithEvents lblMaxTorqueHi As Label
    Friend WithEvents tbMaxTorqueHi As TextBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents PicBox As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblContTq As Label
    Friend WithEvents lblContTqUnit As Label
    Friend WithEvents tbContTqLo As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblOvlTime As Label
    Friend WithEvents lblOvltimeUnit As Label
    Friend WithEvents tbOvlTimeLo As TextBox
    Friend WithEvents pnThermalOverloadRecovery As Panel
    Friend WithEvents lblOvlRecovery As Label
    Friend WithEvents lblOvlRecoveryFactorUnit As Label
    Friend WithEvents tbOverloadRecoveryFactor As TextBox
    Friend WithEvents Panel4 As Panel
    Friend WithEvents lblRatedSpeed As Label
    Friend WithEvents lblRatedSpeedUnit As Label
    Friend WithEvents tbRatedSpeedLo As TextBox
    Friend WithEvents pnOverloadTq As Panel
    Friend WithEvents lblOverloadTq As Label
    Friend WithEvents lblOverloadTqUnit As Label
    Friend WithEvents tbOverloadTqLo As TextBox
    Friend WithEvents Panel6 As Panel
    Friend WithEvents lblOverloadSpeed As Label
    Friend WithEvents lblOverloadSpeedUnit As Label
    Friend WithEvents tbOvlSpeedLo As TextBox
    Friend WithEvents lblVoltageHi As Label
    Friend WithEvents lblVoltageHiUnit As Label
    Friend WithEvents tbVoltageHi As TextBox
    Friend WithEvents lblVoltageLow As Label
    Friend WithEvents lblVoltageLowUnit As Label
    Friend WithEvents tbVoltageLow As TextBox
    Friend WithEvents lblMaxTorqueLow As Label
    Friend WithEvents tbDragTorque As TextBox
    Friend WithEvents lblDragTorque As Label
    Friend WithEvents btnBrowseDragCurve As Button
    Friend WithEvents btnDragCurveOpen As Button
    Friend WithEvents tbMapLow As TextBox
    Friend WithEvents lblPowerMapLow As Label
    Friend WithEvents btnBrowseEmMapLow As Button
    Friend WithEvents btnEmMapOpenLow As Button
    Friend WithEvents tbMaxTorqueLow As TextBox
    Friend WithEvents btnMaxTorqueCurveOpenLow As Button
    Friend WithEvents btnBrowseMaxTorqueLow As Button
    Friend WithEvents tcVoltageLevels As TabControl
    Friend WithEvents tpVoltageLow As TabPage
    Friend WithEvents tpVoltageHigh As TabPage
    Friend WithEvents Panel5 As Panel
    Friend WithEvents Label1 As Label
    Friend WithEvents Label2 As Label
    Friend WithEvents tbOvlSpeedHi As TextBox
    Friend WithEvents Panel7 As Panel
    Friend WithEvents Label3 As Label
    Friend WithEvents Label4 As Label
    Friend WithEvents tbOverloadTqHi As TextBox
    Friend WithEvents Panel8 As Panel
    Friend WithEvents Label5 As Label
    Friend WithEvents Label6 As Label
    Friend WithEvents tbRatedSpeedHi As TextBox
    Friend WithEvents Panel9 As Panel
    Friend WithEvents Label7 As Label
    Friend WithEvents Label8 As Label
    Friend WithEvents tbOvlTimeHi As TextBox
    Friend WithEvents Panel10 As Panel
    Friend WithEvents Label9 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents tbContTqHi As TextBox
    Friend WithEvents pnDragCurve As Panel
    Friend WithEvents pnRatedPower As Panel
    Friend WithEvents lblRatedPower As Label
    Friend WithEvents lblRatedPowerUnit As Label
    Friend WithEvents tbRatedPower As TextBox
    Friend WithEvents pnElectricMachineType As Panel
    Friend WithEvents cbEmType As ComboBox
    Friend WithEvents lblEmType As Label
End Class
