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
        Me.tbDragTorque = New System.Windows.Forms.TextBox()
        Me.lblDragTorque = New System.Windows.Forms.Label()
        Me.btnBrowseDragCurve = New System.Windows.Forms.Button()
        Me.btnDragCurveOpen = New System.Windows.Forms.Button()
        Me.btnEmMapOpen = New System.Windows.Forms.Button()
        Me.btnBrowseEmMap = New System.Windows.Forms.Button()
        Me.lblEmMap = New System.Windows.Forms.Label()
        Me.tbMap = New System.Windows.Forms.TextBox()
        Me.btnMaxTorqueCurveOpen = New System.Windows.Forms.Button()
        Me.btnBrowseMaxTorque = New System.Windows.Forms.Button()
        Me.lblMaxTorque = New System.Windows.Forms.Label()
        Me.tbMaxTorque = New System.Windows.Forms.TextBox()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.PicBox = New System.Windows.Forms.PictureBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.lblContTq = New System.Windows.Forms.Label()
        Me.lblContTqUnit = New System.Windows.Forms.Label()
        Me.tbContTq = New System.Windows.Forms.TextBox()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.lblOvlTime = New System.Windows.Forms.Label()
        Me.lblOvltimeUnit = New System.Windows.Forms.Label()
        Me.tbOvlTime = New System.Windows.Forms.TextBox()
        Me.Panel3 = New System.Windows.Forms.Panel()
        Me.lblOvlRecovery = New System.Windows.Forms.Label()
        Me.lblOvlRecoveryFactorUnit = New System.Windows.Forms.Label()
        Me.tbOverloadRecoveryFactor = New System.Windows.Forms.TextBox()
        Me.Panel4 = New System.Windows.Forms.Panel()
        Me.lblRatedSpeed = New System.Windows.Forms.Label()
        Me.lblRatedSpeedUnit = New System.Windows.Forms.Label()
        Me.tbRatedSpeed = New System.Windows.Forms.TextBox()
        Me.pnOverloadTq = New System.Windows.Forms.Panel()
        Me.lblOverloadTq = New System.Windows.Forms.Label()
        Me.lblOverloadTqUnit = New System.Windows.Forms.Label()
        Me.tbOverloadTq = New System.Windows.Forms.TextBox()
        Me.Panel6 = New System.Windows.Forms.Panel()
        Me.lblOverloadSpeed = New System.Windows.Forms.Label()
        Me.lblOverloadSpeedUnit = New System.Windows.Forms.Label()
        Me.tbOvlSpeed = New System.Windows.Forms.TextBox()
        Me.ToolStrip1.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.pnInertia.SuspendLayout
        CType(Me.PicBox,System.ComponentModel.ISupportInitialize).BeginInit
        Me.Panel1.SuspendLayout
        Me.Panel2.SuspendLayout
        Me.Panel3.SuspendLayout
        Me.Panel4.SuspendLayout
        Me.pnOverloadTq.SuspendLayout
        Me.Panel6.SuspendLayout
        Me.SuspendLayout
        '
        'tbInertia
        '
        Me.tbInertia.Location = New System.Drawing.Point(127, 5)
        Me.tbInertia.Name = "tbInertia"
        Me.tbInertia.Size = New System.Drawing.Size(49, 20)
        Me.tbInertia.TabIndex = 3
        '
        'lblinertiaUnit
        '
        Me.lblinertiaUnit.AutoSize = true
        Me.lblinertiaUnit.Location = New System.Drawing.Point(182, 8)
        Me.lblinertiaUnit.Name = "lblinertiaUnit"
        Me.lblinertiaUnit.Size = New System.Drawing.Size(36, 13)
        Me.lblinertiaUnit.TabIndex = 24
        Me.lblinertiaUnit.Text = "[kgm²]"
        '
        'lblInertia
        '
        Me.lblInertia.AutoSize = true
        Me.lblInertia.Location = New System.Drawing.Point(3, 7)
        Me.lblInertia.Name = "lblInertia"
        Me.lblInertia.Size = New System.Drawing.Size(36, 13)
        Me.lblInertia.TabIndex = 0
        Me.lblInertia.Text = "Inertia"
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(811, 343)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 13
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(730, 343)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 12
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
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(898, 31)
        Me.ToolStrip1.TabIndex = 30
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripBtNew.Text = "ToolStripButton1"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
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
        Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(28, 28)
        Me.ToolStripButton1.Text = "Help"
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 368)
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
        Me.pnInertia.Location = New System.Drawing.Point(12, 108)
        Me.pnInertia.Name = "pnInertia"
        Me.pnInertia.Size = New System.Drawing.Size(221, 30)
        Me.pnInertia.TabIndex = 3
        '
        'tbDragTorque
        '
        Me.tbDragTorque.Location = New System.Drawing.Point(12, 296)
        Me.tbDragTorque.Name = "tbDragTorque"
        Me.tbDragTorque.Size = New System.Drawing.Size(434, 20)
        Me.tbDragTorque.TabIndex = 5
        '
        'lblDragTorque
        '
        Me.lblDragTorque.AutoSize = true
        Me.lblDragTorque.Location = New System.Drawing.Point(12, 280)
        Me.lblDragTorque.Name = "lblDragTorque"
        Me.lblDragTorque.Size = New System.Drawing.Size(98, 13)
        Me.lblDragTorque.TabIndex = 38
        Me.lblDragTorque.Text = "Drag Torque Curve"
        '
        'btnBrowseDragCurve
        '
        Me.btnBrowseDragCurve.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseDragCurve.Location = New System.Drawing.Point(446, 294)
        Me.btnBrowseDragCurve.Name = "btnBrowseDragCurve"
        Me.btnBrowseDragCurve.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseDragCurve.TabIndex = 6
        Me.btnBrowseDragCurve.TabStop = false
        Me.btnBrowseDragCurve.UseVisualStyleBackColor = true
        '
        'btnDragCurveOpen
        '
        Me.btnDragCurveOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnDragCurveOpen.Location = New System.Drawing.Point(469, 294)
        Me.btnDragCurveOpen.Name = "btnDragCurveOpen"
        Me.btnDragCurveOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnDragCurveOpen.TabIndex = 7
        Me.btnDragCurveOpen.TabStop = false
        Me.btnDragCurveOpen.UseVisualStyleBackColor = true
        '
        'btnEmMapOpen
        '
        Me.btnEmMapOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnEmMapOpen.Location = New System.Drawing.Point(469, 335)
        Me.btnEmMapOpen.Name = "btnEmMapOpen"
        Me.btnEmMapOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnEmMapOpen.TabIndex = 42
        Me.btnEmMapOpen.TabStop = false
        Me.btnEmMapOpen.UseVisualStyleBackColor = true
        '
        'btnBrowseEmMap
        '
        Me.btnBrowseEmMap.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseEmMap.Location = New System.Drawing.Point(446, 335)
        Me.btnBrowseEmMap.Name = "btnBrowseEmMap"
        Me.btnBrowseEmMap.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseEmMap.TabIndex = 41
        Me.btnBrowseEmMap.TabStop = false
        Me.btnBrowseEmMap.UseVisualStyleBackColor = true
        '
        'lblEmMap
        '
        Me.lblEmMap.AutoSize = true
        Me.lblEmMap.Location = New System.Drawing.Point(12, 321)
        Me.lblEmMap.Name = "lblEmMap"
        Me.lblEmMap.Size = New System.Drawing.Size(163, 13)
        Me.lblEmMap.TabIndex = 43
        Me.lblEmMap.Text = "Electric Power Consumption Map"
        '
        'tbMap
        '
        Me.tbMap.Location = New System.Drawing.Point(12, 337)
        Me.tbMap.Name = "tbMap"
        Me.tbMap.Size = New System.Drawing.Size(434, 20)
        Me.tbMap.TabIndex = 40
        '
        'btnMaxTorqueCurveOpen
        '
        Me.btnMaxTorqueCurveOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.btnMaxTorqueCurveOpen.Location = New System.Drawing.Point(469, 254)
        Me.btnMaxTorqueCurveOpen.Name = "btnMaxTorqueCurveOpen"
        Me.btnMaxTorqueCurveOpen.Size = New System.Drawing.Size(24, 24)
        Me.btnMaxTorqueCurveOpen.TabIndex = 46
        Me.btnMaxTorqueCurveOpen.TabStop = false
        Me.btnMaxTorqueCurveOpen.UseVisualStyleBackColor = true
        '
        'btnBrowseMaxTorque
        '
        Me.btnBrowseMaxTorque.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnBrowseMaxTorque.Location = New System.Drawing.Point(446, 254)
        Me.btnBrowseMaxTorque.Name = "btnBrowseMaxTorque"
        Me.btnBrowseMaxTorque.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseMaxTorque.TabIndex = 45
        Me.btnBrowseMaxTorque.TabStop = false
        Me.btnBrowseMaxTorque.UseVisualStyleBackColor = true
        '
        'lblMaxTorque
        '
        Me.lblMaxTorque.AutoSize = true
        Me.lblMaxTorque.Location = New System.Drawing.Point(12, 239)
        Me.lblMaxTorque.Name = "lblMaxTorque"
        Me.lblMaxTorque.Size = New System.Drawing.Size(222, 13)
        Me.lblMaxTorque.TabIndex = 47
        Me.lblMaxTorque.Text = "Max Drive and Max Generation Torque Curve"
        '
        'tbMaxTorque
        '
        Me.tbMaxTorque.Location = New System.Drawing.Point(12, 255)
        Me.tbMaxTorque.Name = "tbMaxTorque"
        Me.tbMaxTorque.Size = New System.Drawing.Size(434, 20)
        Me.tbMaxTorque.TabIndex = 44
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(84, 34)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(190, 29)
        Me.lblTitle.TabIndex = 48
        Me.lblTitle.Text = "Electric Machine"
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
        Me.Panel1.Controls.Add(Me.lblContTq)
        Me.Panel1.Controls.Add(Me.lblContTqUnit)
        Me.Panel1.Controls.Add(Me.tbContTq)
        Me.Panel1.Location = New System.Drawing.Point(12, 144)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(221, 30)
        Me.Panel1.TabIndex = 25
        '
        'lblContTq
        '
        Me.lblContTq.AutoSize = true
        Me.lblContTq.Location = New System.Drawing.Point(3, 7)
        Me.lblContTq.Name = "lblContTq"
        Me.lblContTq.Size = New System.Drawing.Size(100, 13)
        Me.lblContTq.TabIndex = 0
        Me.lblContTq.Text = "Continuous Torque:"
        '
        'lblContTqUnit
        '
        Me.lblContTqUnit.AutoSize = true
        Me.lblContTqUnit.Location = New System.Drawing.Point(182, 8)
        Me.lblContTqUnit.Name = "lblContTqUnit"
        Me.lblContTqUnit.Size = New System.Drawing.Size(29, 13)
        Me.lblContTqUnit.TabIndex = 24
        Me.lblContTqUnit.Text = "[Nm]"
        '
        'tbContTq
        '
        Me.tbContTq.Location = New System.Drawing.Point(127, 5)
        Me.tbContTq.Name = "tbContTq"
        Me.tbContTq.Size = New System.Drawing.Size(49, 20)
        Me.tbContTq.TabIndex = 3
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.lblOvlTime)
        Me.Panel2.Controls.Add(Me.lblOvltimeUnit)
        Me.Panel2.Controls.Add(Me.tbOvlTime)
        Me.Panel2.Location = New System.Drawing.Point(12, 206)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(221, 30)
        Me.Panel2.TabIndex = 25
        '
        'lblOvlTime
        '
        Me.lblOvlTime.AutoSize = true
        Me.lblOvlTime.Location = New System.Drawing.Point(3, 7)
        Me.lblOvlTime.Name = "lblOvlTime"
        Me.lblOvlTime.Size = New System.Drawing.Size(96, 13)
        Me.lblOvlTime.TabIndex = 0
        Me.lblOvlTime.Text = "Overload Duration:"
        '
        'lblOvltimeUnit
        '
        Me.lblOvltimeUnit.AutoSize = true
        Me.lblOvltimeUnit.Location = New System.Drawing.Point(180, 7)
        Me.lblOvltimeUnit.Name = "lblOvltimeUnit"
        Me.lblOvltimeUnit.Size = New System.Drawing.Size(18, 13)
        Me.lblOvltimeUnit.TabIndex = 24
        Me.lblOvltimeUnit.Text = "[s]"
        '
        'tbOvlTime
        '
        Me.tbOvlTime.Location = New System.Drawing.Point(127, 5)
        Me.tbOvlTime.Name = "tbOvlTime"
        Me.tbOvlTime.Size = New System.Drawing.Size(49, 20)
        Me.tbOvlTime.TabIndex = 3
        '
        'Panel3
        '
        Me.Panel3.Controls.Add(Me.lblOvlRecovery)
        Me.Panel3.Controls.Add(Me.lblOvlRecoveryFactorUnit)
        Me.Panel3.Controls.Add(Me.tbOverloadRecoveryFactor)
        Me.Panel3.Location = New System.Drawing.Point(233, 206)
        Me.Panel3.Name = "Panel3"
        Me.Panel3.Size = New System.Drawing.Size(261, 30)
        Me.Panel3.TabIndex = 26
        '
        'lblOvlRecovery
        '
        Me.lblOvlRecovery.AutoSize = true
        Me.lblOvlRecovery.Location = New System.Drawing.Point(-2, 8)
        Me.lblOvlRecovery.Name = "lblOvlRecovery"
        Me.lblOvlRecovery.Size = New System.Drawing.Size(176, 13)
        Me.lblOvlRecovery.TabIndex = 0
        Me.lblOvlRecovery.Text = "Thermal Overload Recovery Factor:"
        '
        'lblOvlRecoveryFactorUnit
        '
        Me.lblOvlRecoveryFactorUnit.AutoSize = true
        Me.lblOvlRecoveryFactorUnit.Location = New System.Drawing.Point(230, 7)
        Me.lblOvlRecoveryFactorUnit.Name = "lblOvlRecoveryFactorUnit"
        Me.lblOvlRecoveryFactorUnit.Size = New System.Drawing.Size(16, 13)
        Me.lblOvlRecoveryFactorUnit.TabIndex = 24
        Me.lblOvlRecoveryFactorUnit.Text = "[-]"
        '
        'tbOverloadRecoveryFactor
        '
        Me.tbOverloadRecoveryFactor.Location = New System.Drawing.Point(177, 5)
        Me.tbOverloadRecoveryFactor.Name = "tbOverloadRecoveryFactor"
        Me.tbOverloadRecoveryFactor.Size = New System.Drawing.Size(49, 20)
        Me.tbOverloadRecoveryFactor.TabIndex = 3
        '
        'Panel4
        '
        Me.Panel4.Controls.Add(Me.lblRatedSpeed)
        Me.Panel4.Controls.Add(Me.lblRatedSpeedUnit)
        Me.Panel4.Controls.Add(Me.tbRatedSpeed)
        Me.Panel4.Location = New System.Drawing.Point(233, 144)
        Me.Panel4.Name = "Panel4"
        Me.Panel4.Size = New System.Drawing.Size(261, 30)
        Me.Panel4.TabIndex = 26
        '
        'lblRatedSpeed
        '
        Me.lblRatedSpeed.AutoSize = true
        Me.lblRatedSpeed.Location = New System.Drawing.Point(3, 7)
        Me.lblRatedSpeed.Name = "lblRatedSpeed"
        Me.lblRatedSpeed.Size = New System.Drawing.Size(158, 13)
        Me.lblRatedSpeed.TabIndex = 0
        Me.lblRatedSpeed.Text = "Test Speed Continuous Torque:"
        '
        'lblRatedSpeedUnit
        '
        Me.lblRatedSpeedUnit.AutoSize = true
        Me.lblRatedSpeedUnit.Location = New System.Drawing.Point(229, 7)
        Me.lblRatedSpeedUnit.Name = "lblRatedSpeedUnit"
        Me.lblRatedSpeedUnit.Size = New System.Drawing.Size(30, 13)
        Me.lblRatedSpeedUnit.TabIndex = 24
        Me.lblRatedSpeedUnit.Text = "[rpm]"
        '
        'tbRatedSpeed
        '
        Me.tbRatedSpeed.Location = New System.Drawing.Point(177, 5)
        Me.tbRatedSpeed.Name = "tbRatedSpeed"
        Me.tbRatedSpeed.Size = New System.Drawing.Size(49, 20)
        Me.tbRatedSpeed.TabIndex = 3
        '
        'pnOverloadTq
        '
        Me.pnOverloadTq.Controls.Add(Me.lblOverloadTq)
        Me.pnOverloadTq.Controls.Add(Me.lblOverloadTqUnit)
        Me.pnOverloadTq.Controls.Add(Me.tbOverloadTq)
        Me.pnOverloadTq.Location = New System.Drawing.Point(12, 175)
        Me.pnOverloadTq.Name = "pnOverloadTq"
        Me.pnOverloadTq.Size = New System.Drawing.Size(221, 30)
        Me.pnOverloadTq.TabIndex = 26
        '
        'lblOverloadTq
        '
        Me.lblOverloadTq.AutoSize = true
        Me.lblOverloadTq.Location = New System.Drawing.Point(3, 7)
        Me.lblOverloadTq.Name = "lblOverloadTq"
        Me.lblOverloadTq.Size = New System.Drawing.Size(90, 13)
        Me.lblOverloadTq.TabIndex = 0
        Me.lblOverloadTq.Text = "Overload Torque:"
        '
        'lblOverloadTqUnit
        '
        Me.lblOverloadTqUnit.AutoSize = true
        Me.lblOverloadTqUnit.Location = New System.Drawing.Point(182, 8)
        Me.lblOverloadTqUnit.Name = "lblOverloadTqUnit"
        Me.lblOverloadTqUnit.Size = New System.Drawing.Size(29, 13)
        Me.lblOverloadTqUnit.TabIndex = 24
        Me.lblOverloadTqUnit.Text = "[Nm]"
        '
        'tbOverloadTq
        '
        Me.tbOverloadTq.Location = New System.Drawing.Point(127, 5)
        Me.tbOverloadTq.Name = "tbOverloadTq"
        Me.tbOverloadTq.Size = New System.Drawing.Size(49, 20)
        Me.tbOverloadTq.TabIndex = 3
        '
        'Panel6
        '
        Me.Panel6.Controls.Add(Me.lblOverloadSpeed)
        Me.Panel6.Controls.Add(Me.lblOverloadSpeedUnit)
        Me.Panel6.Controls.Add(Me.tbOvlSpeed)
        Me.Panel6.Location = New System.Drawing.Point(233, 175)
        Me.Panel6.Name = "Panel6"
        Me.Panel6.Size = New System.Drawing.Size(261, 30)
        Me.Panel6.TabIndex = 27
        '
        'lblOverloadSpeed
        '
        Me.lblOverloadSpeed.AutoSize = true
        Me.lblOverloadSpeed.Location = New System.Drawing.Point(3, 7)
        Me.lblOverloadSpeed.Name = "lblOverloadSpeed"
        Me.lblOverloadSpeed.Size = New System.Drawing.Size(148, 13)
        Me.lblOverloadSpeed.TabIndex = 0
        Me.lblOverloadSpeed.Text = "Test Speed Overload Torque:"
        '
        'lblOverloadSpeedUnit
        '
        Me.lblOverloadSpeedUnit.AutoSize = true
        Me.lblOverloadSpeedUnit.Location = New System.Drawing.Point(229, 7)
        Me.lblOverloadSpeedUnit.Name = "lblOverloadSpeedUnit"
        Me.lblOverloadSpeedUnit.Size = New System.Drawing.Size(30, 13)
        Me.lblOverloadSpeedUnit.TabIndex = 24
        Me.lblOverloadSpeedUnit.Text = "[rpm]"
        '
        'tbOvlSpeed
        '
        Me.tbOvlSpeed.Location = New System.Drawing.Point(177, 5)
        Me.tbOvlSpeed.Name = "tbOvlSpeed"
        Me.tbOvlSpeed.Size = New System.Drawing.Size(49, 20)
        Me.tbOvlSpeed.TabIndex = 3
        '
        'ElectricMotorForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(898, 390)
        Me.Controls.Add(Me.Panel6)
        Me.Controls.Add(Me.pnOverloadTq)
        Me.Controls.Add(Me.Panel4)
        Me.Controls.Add(Me.Panel3)
        Me.Controls.Add(Me.Panel2)
        Me.Controls.Add(Me.Panel1)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.btnMaxTorqueCurveOpen)
        Me.Controls.Add(Me.btnBrowseMaxTorque)
        Me.Controls.Add(Me.lblMaxTorque)
        Me.Controls.Add(Me.tbMaxTorque)
        Me.Controls.Add(Me.btnEmMapOpen)
        Me.Controls.Add(Me.btnBrowseEmMap)
        Me.Controls.Add(Me.lblEmMap)
        Me.Controls.Add(Me.tbMap)
        Me.Controls.Add(Me.pnInertia)
        Me.Controls.Add(Me.btnDragCurveOpen)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.btnBrowseDragCurve)
        Me.Controls.Add(Me.lblDragTorque)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.tbDragTorque)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.lblMakeModel)
        Me.Controls.Add(Me.tbMakeModel)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
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
        Me.Panel3.ResumeLayout(false)
        Me.Panel3.PerformLayout
        Me.Panel4.ResumeLayout(false)
        Me.Panel4.PerformLayout
        Me.pnOverloadTq.ResumeLayout(false)
        Me.pnOverloadTq.PerformLayout
        Me.Panel6.ResumeLayout(false)
        Me.Panel6.PerformLayout
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
    Friend WithEvents tbDragTorque As TextBox
    Friend WithEvents lblDragTorque As Label
    Friend WithEvents btnBrowseDragCurve As Button
    Friend WithEvents btnDragCurveOpen As Button
    Friend WithEvents btnEmMapOpen As Button
    Friend WithEvents btnBrowseEmMap As Button
    Friend WithEvents lblEmMap As Label
    Friend WithEvents tbMap As TextBox
    Friend WithEvents btnMaxTorqueCurveOpen As Button
    Friend WithEvents btnBrowseMaxTorque As Button
    Friend WithEvents lblMaxTorque As Label
    Friend WithEvents tbMaxTorque As TextBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents PicBox As PictureBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents lblContTq As Label
    Friend WithEvents lblContTqUnit As Label
    Friend WithEvents tbContTq As TextBox
    Friend WithEvents Panel2 As Panel
    Friend WithEvents lblOvlTime As Label
    Friend WithEvents lblOvltimeUnit As Label
    Friend WithEvents tbOvlTime As TextBox
    Friend WithEvents Panel3 As Panel
    Friend WithEvents lblOvlRecovery As Label
    Friend WithEvents lblOvlRecoveryFactorUnit As Label
    Friend WithEvents tbOverloadRecoveryFactor As TextBox
    Friend WithEvents Panel4 As Panel
    Friend WithEvents lblRatedSpeed As Label
    Friend WithEvents lblRatedSpeedUnit As Label
    Friend WithEvents tbRatedSpeed As TextBox
    Friend WithEvents pnOverloadTq As Panel
    Friend WithEvents lblOverloadTq As Label
    Friend WithEvents lblOverloadTqUnit As Label
    Friend WithEvents tbOverloadTq As TextBox
    Friend WithEvents Panel6 As Panel
    Friend WithEvents lblOverloadSpeed As Label
    Friend WithEvents lblOverloadSpeedUnit As Label
    Friend WithEvents tbOvlSpeed As TextBox
End Class
