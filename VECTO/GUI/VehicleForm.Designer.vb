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
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class VehicleForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VehicleForm))
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
        Me.Label6 = New System.Windows.Forms.Label()
        Me.ButAxlRem = New System.Windows.Forms.Button()
        Me.LvRRC = New System.Windows.Forms.ListView()
        Me.ColumnHeader7 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader8 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader9 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader10 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ButAxlAdd = New System.Windows.Forms.Button()
        Me.PnWheelDiam = New System.Windows.Forms.Panel()
        Me.CbAxleConfig = New System.Windows.Forms.ComboBox()
        Me.CbCat = New System.Windows.Forms.ComboBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.TbMassMass = New System.Windows.Forms.TextBox()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.LbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.TbHDVclass = New System.Windows.Forms.TextBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.PnLoad = New System.Windows.Forms.Panel()
        Me.GrAirRes = New System.Windows.Forms.GroupBox()
        Me.PnCdATrTr = New System.Windows.Forms.Panel()
        Me.tbVehicleHeight = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label38 = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.gbPTO = New System.Windows.Forms.GroupBox()
        Me.pnPTO = New System.Windows.Forms.Panel()
        Me.btPTOCycle = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.tbPTOCycle = New System.Windows.Forms.TextBox()
        Me.btPTOLossMapBrowse = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.tbPTOLossMap = New System.Windows.Forms.TextBox()
        Me.cbPTOType = New System.Windows.Forms.ComboBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.pnAngledriveFields = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.tbAngledriveRatio = New System.Windows.Forms.TextBox()
        Me.btAngledriveLossMapBrowse = New System.Windows.Forms.Button()
        Me.tbAngledriveLossMapPath = New System.Windows.Forms.TextBox()
        Me.cbAngledriveType = New System.Windows.Forms.ComboBox()
        Me.PicVehicle = New System.Windows.Forms.PictureBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.tcVehicleComponents = New System.Windows.Forms.TabControl()
        Me.tpGeneral = New System.Windows.Forms.TabPage()
        Me.tpPowertrain = New System.Windows.Forms.TabPage()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.tbVehIdlingSpeed = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.GroupBox9 = New System.Windows.Forms.GroupBox()
        Me.cbTankSystem = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.tpElectricComponents = New System.Windows.Forms.TabPage()
        Me.gbBattery = New System.Windows.Forms.GroupBox()
        Me.lblInitialSoCUnit = New System.Windows.Forms.Label()
        Me.tbInitialSoC = New System.Windows.Forms.TextBox()
        Me.lblInitialSoC = New System.Windows.Forms.Label()
        Me.tbBatteryPackCnt = New System.Windows.Forms.TextBox()
        Me.lblBatteryPackCnt = New System.Windows.Forms.Label()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.btnOpenBattery = New System.Windows.Forms.Button()
        Me.btnBrowseBattery = New System.Windows.Forms.Button()
        Me.tbBattery = New System.Windows.Forms.TextBox()
        Me.gpElectricMotor = New System.Windows.Forms.GroupBox()
        Me.tbEmEfficiency = New System.Windows.Forms.TextBox()
        Me.lblEmEfficiency = New System.Windows.Forms.Label()
        Me.tbRatioEm = New System.Windows.Forms.TextBox()
        Me.lblRatioEm = New System.Windows.Forms.Label()
        Me.tbEmCount = New System.Windows.Forms.TextBox()
        Me.cbEmPos = New System.Windows.Forms.ComboBox()
        Me.lblEmCount = New System.Windows.Forms.Label()
        Me.lblEmPosition = New System.Windows.Forms.Label()
        Me.pnElectricMotor = New System.Windows.Forms.Panel()
        Me.btnOpenElectricMotor = New System.Windows.Forms.Button()
        Me.btnBrowseElectricMotor = New System.Windows.Forms.Button()
        Me.tbElectricMotor = New System.Windows.Forms.TextBox()
        Me.tpTorqueLimits = New System.Windows.Forms.TabPage()
        Me.lvTorqueLimits = New System.Windows.Forms.ListView()
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Label17 = New System.Windows.Forms.Label()
        Me.btDelMaxTorqueEntry = New System.Windows.Forms.Button()
        Me.btAddMaxTorqueEntry = New System.Windows.Forms.Button()
        Me.tpADAS = New System.Windows.Forms.TabPage()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.cbAtEcoRollReleaseLockupClutch = New System.Windows.Forms.CheckBox()
        Me.cbPcc = New System.Windows.Forms.ComboBox()
        Me.cbEcoRoll = New System.Windows.Forms.ComboBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.cbEngineStopStart = New System.Windows.Forms.CheckBox()
        Me.lblPCC = New System.Windows.Forms.Label()
        Me.cbLegislativeClass = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.lblMaxDrivetrainPwrUnit = New System.Windows.Forms.Label()
        Me.tbMaxDrivetrainPwr = New System.Windows.Forms.TextBox()
        Me.lblMaxDrivetrainPwr = New System.Windows.Forms.Label()
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
        Me.gbPTO.SuspendLayout()
        Me.pnPTO.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.pnAngledriveFields.SuspendLayout()
        CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tcVehicleComponents.SuspendLayout()
        Me.tpGeneral.SuspendLayout()
        Me.tpPowertrain.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.GroupBox9.SuspendLayout()
        Me.tpElectricComponents.SuspendLayout()
        Me.gbBattery.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.gpElectricMotor.SuspendLayout()
        Me.pnElectricMotor.SuspendLayout()
        Me.tpTorqueLimits.SuspendLayout()
        Me.tpADAS.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(9, 34)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(264, 20)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Corrected Actual Curb Mass Vehicle"
        '
        'TbMass
        '
        Me.TbMass.Location = New System.Drawing.Point(282, 29)
        Me.TbMass.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbMass.Name = "TbMass"
        Me.TbMass.Size = New System.Drawing.Size(84, 26)
        Me.TbMass.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(192, 48)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(66, 20)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Loading"
        '
        'TbLoad
        '
        Me.TbLoad.Location = New System.Drawing.Point(273, 43)
        Me.TbLoad.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbLoad.Name = "TbLoad"
        Me.TbLoad.Size = New System.Drawing.Size(84, 26)
        Me.TbLoad.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(112, 9)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(55, 20)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Cd x A"
        '
        'TBcdA
        '
        Me.TBcdA.Location = New System.Drawing.Point(178, 5)
        Me.TBcdA.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBcdA.Name = "TBcdA"
        Me.TBcdA.Size = New System.Drawing.Size(84, 26)
        Me.TBcdA.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(204, 9)
        Me.Label13.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(59, 20)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "Radius"
        '
        'TBrdyn
        '
        Me.TBrdyn.Location = New System.Drawing.Point(278, 5)
        Me.TBrdyn.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TBrdyn.Name = "TBrdyn"
        Me.TBrdyn.Size = New System.Drawing.Size(84, 26)
        Me.TBrdyn.TabIndex = 0
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(646, 862)
        Me.ButOK.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(112, 35)
        Me.ButOK.TabIndex = 5
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = True
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(768, 862)
        Me.ButCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(112, 35)
        Me.ButCancel.TabIndex = 6
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = True
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(370, 34)
        Me.Label14.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(34, 20)
        Me.Label14.TabIndex = 24
        Me.Label14.Text = "[kg]"
        '
        'Label31
        '
        Me.Label31.AutoSize = True
        Me.Label31.Location = New System.Drawing.Point(362, 48)
        Me.Label31.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(34, 20)
        Me.Label31.TabIndex = 24
        Me.Label31.Text = "[kg]"
        '
        'Label35
        '
        Me.Label35.AutoSize = True
        Me.Label35.Location = New System.Drawing.Point(366, 9)
        Me.Label35.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(43, 20)
        Me.Label35.TabIndex = 24
        Me.Label35.Text = "[mm]"
        '
        'CbCdMode
        '
        Me.CbCdMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCdMode.FormattingEnabled = True
        Me.CbCdMode.Items.AddRange(New Object() {"No Correction", "Speed dependent (User-defined)", "Speed dependent (Declaration Mode)", "Vair & Beta Input"})
        Me.CbCdMode.Location = New System.Drawing.Point(9, 29)
        Me.CbCdMode.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CbCdMode.Name = "CbCdMode"
        Me.CbCdMode.Size = New System.Drawing.Size(398, 28)
        Me.CbCdMode.TabIndex = 0
        '
        'TbCdFile
        '
        Me.TbCdFile.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.TbCdFile.Enabled = False
        Me.TbCdFile.Location = New System.Drawing.Point(14, 100)
        Me.TbCdFile.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbCdFile.Name = "TbCdFile"
        Me.TbCdFile.Size = New System.Drawing.Size(313, 26)
        Me.TbCdFile.TabIndex = 1
        '
        'BtCdFileBrowse
        '
        Me.BtCdFileBrowse.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.BtCdFileBrowse.Enabled = False
        Me.BtCdFileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtCdFileBrowse.Location = New System.Drawing.Point(338, 95)
        Me.BtCdFileBrowse.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtCdFileBrowse.Name = "BtCdFileBrowse"
        Me.BtCdFileBrowse.Size = New System.Drawing.Size(36, 37)
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
        Me.GroupBox6.Location = New System.Drawing.Point(435, 129)
        Me.GroupBox6.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox6.Size = New System.Drawing.Size(422, 148)
        Me.GroupBox6.TabIndex = 5
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "Cross Wind Correction"
        '
        'BtCdFileOpen
        '
        Me.BtCdFileOpen.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.BtCdFileOpen.Enabled = False
        Me.BtCdFileOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtCdFileOpen.Location = New System.Drawing.Point(374, 95)
        Me.BtCdFileOpen.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtCdFileOpen.Name = "BtCdFileOpen"
        Me.BtCdFileOpen.Size = New System.Drawing.Size(36, 37)
        Me.BtCdFileOpen.TabIndex = 3
        Me.BtCdFileOpen.TabStop = False
        Me.BtCdFileOpen.UseVisualStyleBackColor = True
        '
        'LbCdMode
        '
        Me.LbCdMode.AutoSize = True
        Me.LbCdMode.Location = New System.Drawing.Point(9, 72)
        Me.LbCdMode.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LbCdMode.Name = "LbCdMode"
        Me.LbCdMode.Size = New System.Drawing.Size(87, 20)
        Me.LbCdMode.TabIndex = 28
        Me.LbCdMode.Text = "LbCdMode"
        Me.LbCdMode.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator3, Me.ToolStripBtSendTo, Me.ToolStripSeparator1, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 3, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(898, 33)
        Me.ToolStrip1.TabIndex = 29
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
        'GroupBox7
        '
        Me.GroupBox7.Controls.Add(Me.PnRt)
        Me.GroupBox7.Controls.Add(Me.CbRtType)
        Me.GroupBox7.Location = New System.Drawing.Point(9, 117)
        Me.GroupBox7.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox7.Name = "GroupBox7"
        Me.GroupBox7.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox7.Size = New System.Drawing.Size(418, 171)
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
        Me.PnRt.Location = New System.Drawing.Point(4, 65)
        Me.PnRt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PnRt.Name = "PnRt"
        Me.PnRt.Size = New System.Drawing.Size(402, 97)
        Me.PnRt.TabIndex = 1
        '
        'Label15
        '
        Me.Label15.Location = New System.Drawing.Point(9, 35)
        Me.Label15.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(302, 25)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Retarder Loss Map"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'BtRtBrowse
        '
        Me.BtRtBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtRtBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtRtBrowse.Location = New System.Drawing.Point(354, 60)
        Me.BtRtBrowse.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtRtBrowse.Name = "BtRtBrowse"
        Me.BtRtBrowse.Size = New System.Drawing.Size(36, 37)
        Me.BtRtBrowse.TabIndex = 14
        Me.BtRtBrowse.UseVisualStyleBackColor = True
        '
        'TbRtPath
        '
        Me.TbRtPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TbRtPath.Location = New System.Drawing.Point(9, 63)
        Me.TbRtPath.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbRtPath.Name = "TbRtPath"
        Me.TbRtPath.Size = New System.Drawing.Size(334, 26)
        Me.TbRtPath.TabIndex = 13
        '
        'Label45
        '
        Me.Label45.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label45.AutoSize = True
        Me.Label45.Location = New System.Drawing.Point(348, 8)
        Me.Label45.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(22, 20)
        Me.Label45.TabIndex = 10
        Me.Label45.Text = "[-]"
        '
        'LbRtRatio
        '
        Me.LbRtRatio.Location = New System.Drawing.Point(6, 8)
        Me.LbRtRatio.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.LbRtRatio.Name = "LbRtRatio"
        Me.LbRtRatio.Size = New System.Drawing.Size(250, 26)
        Me.LbRtRatio.TabIndex = 5
        Me.LbRtRatio.Text = "Ratio"
        Me.LbRtRatio.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TbRtRatio
        '
        Me.TbRtRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TbRtRatio.Location = New System.Drawing.Point(261, 5)
        Me.TbRtRatio.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbRtRatio.Name = "TbRtRatio"
        Me.TbRtRatio.Size = New System.Drawing.Size(82, 26)
        Me.TbRtRatio.TabIndex = 0
        '
        'CbRtType
        '
        Me.CbRtType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbRtType.FormattingEnabled = True
        Me.CbRtType.Items.AddRange(New Object() {"Included in Transmission Loss Maps", "Primary Retarder", "Secondary Retarder"})
        Me.CbRtType.Location = New System.Drawing.Point(9, 29)
        Me.CbRtType.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CbRtType.Name = "CbRtType"
        Me.CbRtType.Size = New System.Drawing.Size(397, 28)
        Me.CbRtType.TabIndex = 0
        '
        'Label46
        '
        Me.Label46.AutoSize = True
        Me.Label46.Location = New System.Drawing.Point(48, 8)
        Me.Label46.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(213, 20)
        Me.Label46.TabIndex = 31
        Me.Label46.Text = "Curb Mass Extra Trailer/Body"
        '
        'Label50
        '
        Me.Label50.AutoSize = True
        Me.Label50.Location = New System.Drawing.Point(362, 8)
        Me.Label50.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(34, 20)
        Me.Label50.TabIndex = 24
        Me.Label50.Text = "[kg]"
        '
        'TbMassExtra
        '
        Me.TbMassExtra.Location = New System.Drawing.Point(273, 3)
        Me.TbMassExtra.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbMassExtra.Name = "TbMassExtra"
        Me.TbMassExtra.Size = New System.Drawing.Size(84, 26)
        Me.TbMassExtra.TabIndex = 0
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label6)
        Me.GroupBox8.Controls.Add(Me.ButAxlRem)
        Me.GroupBox8.Controls.Add(Me.LvRRC)
        Me.GroupBox8.Controls.Add(Me.ButAxlAdd)
        Me.GroupBox8.Location = New System.Drawing.Point(9, 286)
        Me.GroupBox8.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox8.Size = New System.Drawing.Size(846, 232)
        Me.GroupBox8.TabIndex = 2
        Me.GroupBox8.TabStop = False
        Me.GroupBox8.Text = "Axles / Wheels"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(675, 186)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(158, 20)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "(Double-Click to Edit)"
        '
        'ButAxlRem
        '
        Me.ButAxlRem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.ButAxlRem.Location = New System.Drawing.Point(44, 188)
        Me.ButAxlRem.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButAxlRem.Name = "ButAxlRem"
        Me.ButAxlRem.Size = New System.Drawing.Size(36, 37)
        Me.ButAxlRem.TabIndex = 2
        Me.ButAxlRem.UseVisualStyleBackColor = True
        '
        'LvRRC
        '
        Me.LvRRC.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LvRRC.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader2, Me.ColumnHeader9, Me.ColumnHeader1, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader10})
        Me.LvRRC.FullRowSelect = True
        Me.LvRRC.GridLines = True
        Me.LvRRC.HideSelection = False
        Me.LvRRC.Location = New System.Drawing.Point(9, 29)
        Me.LvRRC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.LvRRC.MultiSelect = False
        Me.LvRRC.Name = "LvRRC"
        Me.LvRRC.Size = New System.Drawing.Size(826, 155)
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
        Me.ColumnHeader3.Width = 100
        '
        'ColumnHeader4
        '
        Me.ColumnHeader4.Text = "Inertia"
        '
        'ColumnHeader10
        '
        Me.ColumnHeader10.Text = "Axle Type"
        Me.ColumnHeader10.Width = 130
        '
        'ButAxlAdd
        '
        Me.ButAxlAdd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.ButAxlAdd.Location = New System.Drawing.Point(8, 188)
        Me.ButAxlAdd.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButAxlAdd.Name = "ButAxlAdd"
        Me.ButAxlAdd.Size = New System.Drawing.Size(36, 37)
        Me.ButAxlAdd.TabIndex = 1
        Me.ButAxlAdd.UseVisualStyleBackColor = True
        '
        'PnWheelDiam
        '
        Me.PnWheelDiam.Controls.Add(Me.Label13)
        Me.PnWheelDiam.Controls.Add(Me.TBrdyn)
        Me.PnWheelDiam.Controls.Add(Me.Label35)
        Me.PnWheelDiam.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PnWheelDiam.Location = New System.Drawing.Point(4, 24)
        Me.PnWheelDiam.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PnWheelDiam.Name = "PnWheelDiam"
        Me.PnWheelDiam.Size = New System.Drawing.Size(409, 48)
        Me.PnWheelDiam.TabIndex = 5
        '
        'CbAxleConfig
        '
        Me.CbAxleConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbAxleConfig.FormattingEnabled = True
        Me.CbAxleConfig.Items.AddRange(New Object() {"-", "4x2", "4x4", "6x2", "6x4", "6x6", "8x2", "8x4", "8x6", "8x8"})
        Me.CbAxleConfig.Location = New System.Drawing.Point(230, 123)
        Me.CbAxleConfig.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CbAxleConfig.Name = "CbAxleConfig"
        Me.CbAxleConfig.Size = New System.Drawing.Size(88, 28)
        Me.CbAxleConfig.TabIndex = 1
        '
        'CbCat
        '
        Me.CbCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCat.FormattingEnabled = True
        Me.CbCat.Items.AddRange(New Object() {"-", "Rigid Truck", "Tractor", "City Bus", "Interurban Bus", "Coach"})
        Me.CbCat.Location = New System.Drawing.Point(18, 123)
        Me.CbCat.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.CbCat.Name = "CbCat"
        Me.CbCat.Size = New System.Drawing.Size(200, 28)
        Me.CbCat.TabIndex = 0
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(46, 166)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(170, 20)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "Technically Permissible"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(296, 175)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(22, 20)
        Me.Label9.TabIndex = 3
        Me.Label9.Text = "[t]"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TbMassMass
        '
        Me.TbMassMass.Location = New System.Drawing.Point(230, 171)
        Me.TbMassMass.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbMassMass.Name = "TbMassMass"
        Me.TbMassMass.Size = New System.Drawing.Size(61, 26)
        Me.TbMassMass.TabIndex = 2
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 903)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(2, 0, 21, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(898, 32)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 36
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'LbStatus
        '
        Me.LbStatus.Name = "LbStatus"
        Me.LbStatus.Size = New System.Drawing.Size(60, 25)
        Me.LbStatus.Text = "Status"
        '
        'TbHDVclass
        '
        Me.TbHDVclass.Location = New System.Drawing.Point(230, 217)
        Me.TbHDVclass.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbHDVclass.Name = "TbHDVclass"
        Me.TbHDVclass.ReadOnly = True
        Me.TbHDVclass.Size = New System.Drawing.Size(61, 26)
        Me.TbHDVclass.TabIndex = 3
        Me.TbHDVclass.TabStop = False
        Me.TbHDVclass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.PnLoad)
        Me.GroupBox1.Controls.Add(Me.TbMass)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Location = New System.Drawing.Point(9, 9)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(417, 182)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Masses / Loading"
        '
        'PnLoad
        '
        Me.PnLoad.Controls.Add(Me.Label2)
        Me.PnLoad.Controls.Add(Me.Label31)
        Me.PnLoad.Controls.Add(Me.TbLoad)
        Me.PnLoad.Controls.Add(Me.TbMassExtra)
        Me.PnLoad.Controls.Add(Me.Label50)
        Me.PnLoad.Controls.Add(Me.Label46)
        Me.PnLoad.Location = New System.Drawing.Point(9, 66)
        Me.PnLoad.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PnLoad.Name = "PnLoad"
        Me.PnLoad.Size = New System.Drawing.Size(404, 89)
        Me.PnLoad.TabIndex = 1
        '
        'GrAirRes
        '
        Me.GrAirRes.Controls.Add(Me.PnCdATrTr)
        Me.GrAirRes.Location = New System.Drawing.Point(435, 9)
        Me.GrAirRes.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrAirRes.Name = "GrAirRes"
        Me.GrAirRes.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrAirRes.Size = New System.Drawing.Size(420, 111)
        Me.GrAirRes.TabIndex = 1
        Me.GrAirRes.TabStop = False
        Me.GrAirRes.Text = "Air Resistance"
        '
        'PnCdATrTr
        '
        Me.PnCdATrTr.Controls.Add(Me.tbVehicleHeight)
        Me.PnCdATrTr.Controls.Add(Me.Label11)
        Me.PnCdATrTr.Controls.Add(Me.Label20)
        Me.PnCdATrTr.Controls.Add(Me.TBcdA)
        Me.PnCdATrTr.Controls.Add(Me.Label38)
        Me.PnCdATrTr.Controls.Add(Me.Label3)
        Me.PnCdATrTr.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PnCdATrTr.Location = New System.Drawing.Point(4, 24)
        Me.PnCdATrTr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PnCdATrTr.Name = "PnCdATrTr"
        Me.PnCdATrTr.Size = New System.Drawing.Size(412, 82)
        Me.PnCdATrTr.TabIndex = 0
        '
        'tbVehicleHeight
        '
        Me.tbVehicleHeight.Location = New System.Drawing.Point(178, 45)
        Me.tbVehicleHeight.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbVehicleHeight.Name = "tbVehicleHeight"
        Me.tbVehicleHeight.Size = New System.Drawing.Size(84, 26)
        Me.tbVehicleHeight.TabIndex = 25
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(273, 49)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(30, 20)
        Me.Label11.TabIndex = 27
        Me.Label11.Text = "[m]"
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(112, 49)
        Me.Label20.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(56, 20)
        Me.Label20.TabIndex = 26
        Me.Label20.Text = "Height"
        '
        'Label38
        '
        Me.Label38.AutoSize = True
        Me.Label38.Location = New System.Drawing.Point(273, 9)
        Me.Label38.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(35, 20)
        Me.Label38.TabIndex = 24
        Me.Label38.Text = "[m²]"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_VEH
        Me.PictureBox1.Location = New System.Drawing.Point(0, 43)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(906, 62)
        Me.PictureBox1.TabIndex = 37
        Me.PictureBox1.TabStop = False
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.ShowImageMargin = False
        Me.CmOpenFile.Size = New System.Drawing.Size(178, 68)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(177, 32)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(177, 32)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'gbPTO
        '
        Me.gbPTO.Controls.Add(Me.pnPTO)
        Me.gbPTO.Controls.Add(Me.cbPTOType)
        Me.gbPTO.Location = New System.Drawing.Point(9, 297)
        Me.gbPTO.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.gbPTO.Name = "gbPTO"
        Me.gbPTO.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.gbPTO.Size = New System.Drawing.Size(846, 132)
        Me.gbPTO.TabIndex = 4
        Me.gbPTO.TabStop = False
        Me.gbPTO.Text = "PTO Transmission"
        '
        'pnPTO
        '
        Me.pnPTO.Controls.Add(Me.btPTOCycle)
        Me.pnPTO.Controls.Add(Me.Label16)
        Me.pnPTO.Controls.Add(Me.tbPTOCycle)
        Me.pnPTO.Controls.Add(Me.btPTOLossMapBrowse)
        Me.pnPTO.Controls.Add(Me.Label7)
        Me.pnPTO.Controls.Add(Me.tbPTOLossMap)
        Me.pnPTO.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.pnPTO.Location = New System.Drawing.Point(4, 64)
        Me.pnPTO.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnPTO.Name = "pnPTO"
        Me.pnPTO.Size = New System.Drawing.Size(838, 63)
        Me.pnPTO.TabIndex = 4
        '
        'btPTOCycle
        '
        Me.btPTOCycle.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btPTOCycle.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOCycle.Location = New System.Drawing.Point(794, 25)
        Me.btPTOCycle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btPTOCycle.Name = "btPTOCycle"
        Me.btPTOCycle.Size = New System.Drawing.Size(36, 37)
        Me.btPTOCycle.TabIndex = 17
        Me.btPTOCycle.UseVisualStyleBackColor = True
        '
        'Label16
        '
        Me.Label16.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label16.Location = New System.Drawing.Point(430, -2)
        Me.Label16.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(302, 25)
        Me.Label16.TabIndex = 18
        Me.Label16.Text = "PTO Cycle (.vptoc)"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'tbPTOCycle
        '
        Me.tbPTOCycle.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tbPTOCycle.Location = New System.Drawing.Point(435, 28)
        Me.tbPTOCycle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbPTOCycle.Name = "tbPTOCycle"
        Me.tbPTOCycle.Size = New System.Drawing.Size(356, 26)
        Me.tbPTOCycle.TabIndex = 16
        Me.ToolTip1.SetToolTip(Me.tbPTOCycle, "PTO Consumer Loss Map")
        '
        'btPTOLossMapBrowse
        '
        Me.btPTOLossMapBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btPTOLossMapBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOLossMapBrowse.Location = New System.Drawing.Point(368, 25)
        Me.btPTOLossMapBrowse.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btPTOLossMapBrowse.Name = "btPTOLossMapBrowse"
        Me.btPTOLossMapBrowse.Size = New System.Drawing.Size(36, 37)
        Me.btPTOLossMapBrowse.TabIndex = 14
        Me.btPTOLossMapBrowse.UseVisualStyleBackColor = True
        '
        'Label7
        '
        Me.Label7.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label7.Location = New System.Drawing.Point(4, -2)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(302, 25)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "PTO Consumer Loss Map (.vptol)"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'tbPTOLossMap
        '
        Me.tbPTOLossMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tbPTOLossMap.Location = New System.Drawing.Point(9, 28)
        Me.tbPTOLossMap.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbPTOLossMap.Name = "tbPTOLossMap"
        Me.tbPTOLossMap.Size = New System.Drawing.Size(356, 26)
        Me.tbPTOLossMap.TabIndex = 13
        Me.ToolTip1.SetToolTip(Me.tbPTOLossMap, "PTO Consumer Loss Map")
        '
        'cbPTOType
        '
        Me.cbPTOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPTOType.Location = New System.Drawing.Point(9, 26)
        Me.cbPTOType.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbPTOType.Name = "cbPTOType"
        Me.cbPTOType.Size = New System.Drawing.Size(823, 28)
        Me.cbPTOType.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.cbPTOType, "Transmission type to the PTO consumer")
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.PnWheelDiam)
        Me.GroupBox3.Location = New System.Drawing.Point(9, 200)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox3.Size = New System.Drawing.Size(417, 77)
        Me.GroupBox3.TabIndex = 6
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Dynamic Tyre Radius"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.pnAngledriveFields)
        Me.GroupBox2.Controls.Add(Me.cbAngledriveType)
        Me.GroupBox2.Location = New System.Drawing.Point(436, 117)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox2.Size = New System.Drawing.Size(423, 171)
        Me.GroupBox2.TabIndex = 4
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Angledrive"
        '
        'pnAngledriveFields
        '
        Me.pnAngledriveFields.Controls.Add(Me.Label4)
        Me.pnAngledriveFields.Controls.Add(Me.Label12)
        Me.pnAngledriveFields.Controls.Add(Me.Label10)
        Me.pnAngledriveFields.Controls.Add(Me.tbAngledriveRatio)
        Me.pnAngledriveFields.Controls.Add(Me.btAngledriveLossMapBrowse)
        Me.pnAngledriveFields.Controls.Add(Me.tbAngledriveLossMapPath)
        Me.pnAngledriveFields.Location = New System.Drawing.Point(4, 65)
        Me.pnAngledriveFields.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnAngledriveFields.Name = "pnAngledriveFields"
        Me.pnAngledriveFields.Size = New System.Drawing.Size(408, 97)
        Me.pnAngledriveFields.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(369, 9)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(22, 20)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "[-]"
        '
        'Label12
        '
        Me.Label12.Location = New System.Drawing.Point(9, 35)
        Me.Label12.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(394, 25)
        Me.Label12.TabIndex = 17
        Me.Label12.Text = "Transmission Loss Map or Efficiency Value [0..1]"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'Label10
        '
        Me.Label10.Location = New System.Drawing.Point(208, 9)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(66, 28)
        Me.Label10.TabIndex = 15
        Me.Label10.Text = "Ratio"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbAngledriveRatio
        '
        Me.tbAngledriveRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbAngledriveRatio.Location = New System.Drawing.Point(282, 6)
        Me.tbAngledriveRatio.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbAngledriveRatio.Name = "tbAngledriveRatio"
        Me.tbAngledriveRatio.Size = New System.Drawing.Size(82, 26)
        Me.tbAngledriveRatio.TabIndex = 12
        '
        'btAngledriveLossMapBrowse
        '
        Me.btAngledriveLossMapBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btAngledriveLossMapBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btAngledriveLossMapBrowse.Location = New System.Drawing.Point(368, 60)
        Me.btAngledriveLossMapBrowse.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btAngledriveLossMapBrowse.Name = "btAngledriveLossMapBrowse"
        Me.btAngledriveLossMapBrowse.Size = New System.Drawing.Size(36, 37)
        Me.btAngledriveLossMapBrowse.TabIndex = 14
        Me.btAngledriveLossMapBrowse.UseVisualStyleBackColor = True
        '
        'tbAngledriveLossMapPath
        '
        Me.tbAngledriveLossMapPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.tbAngledriveLossMapPath.Location = New System.Drawing.Point(9, 63)
        Me.tbAngledriveLossMapPath.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbAngledriveLossMapPath.Name = "tbAngledriveLossMapPath"
        Me.tbAngledriveLossMapPath.Size = New System.Drawing.Size(356, 26)
        Me.tbAngledriveLossMapPath.TabIndex = 13
        '
        'cbAngledriveType
        '
        Me.cbAngledriveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAngledriveType.FormattingEnabled = True
        Me.cbAngledriveType.Location = New System.Drawing.Point(9, 29)
        Me.cbAngledriveType.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbAngledriveType.Name = "cbAngledriveType"
        Me.cbAngledriveType.Size = New System.Drawing.Size(397, 28)
        Me.cbAngledriveType.TabIndex = 0
        '
        'PicVehicle
        '
        Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
        Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVehicle.Location = New System.Drawing.Point(422, 108)
        Me.PicVehicle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PicVehicle.Name = "PicVehicle"
        Me.PicVehicle.Size = New System.Drawing.Size(449, 134)
        Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicVehicle.TabIndex = 39
        Me.PicVehicle.TabStop = False
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(128, 222)
        Me.Label8.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(93, 20)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "HDV Group"
        '
        'tcVehicleComponents
        '
        Me.tcVehicleComponents.Controls.Add(Me.tpGeneral)
        Me.tcVehicleComponents.Controls.Add(Me.tpPowertrain)
        Me.tcVehicleComponents.Controls.Add(Me.tpElectricComponents)
        Me.tcVehicleComponents.Controls.Add(Me.tpTorqueLimits)
        Me.tcVehicleComponents.Controls.Add(Me.tpADAS)
        Me.tcVehicleComponents.Location = New System.Drawing.Point(9, 266)
        Me.tcVehicleComponents.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tcVehicleComponents.Name = "tcVehicleComponents"
        Me.tcVehicleComponents.SelectedIndex = 0
        Me.tcVehicleComponents.Size = New System.Drawing.Size(880, 586)
        Me.tcVehicleComponents.TabIndex = 40
        '
        'tpGeneral
        '
        Me.tpGeneral.Controls.Add(Me.GroupBox1)
        Me.tpGeneral.Controls.Add(Me.GroupBox3)
        Me.tpGeneral.Controls.Add(Me.GroupBox6)
        Me.tpGeneral.Controls.Add(Me.GroupBox8)
        Me.tpGeneral.Controls.Add(Me.GrAirRes)
        Me.tpGeneral.Location = New System.Drawing.Point(4, 29)
        Me.tpGeneral.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpGeneral.Name = "tpGeneral"
        Me.tpGeneral.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpGeneral.Size = New System.Drawing.Size(872, 553)
        Me.tpGeneral.TabIndex = 0
        Me.tpGeneral.Text = "General"
        Me.tpGeneral.UseVisualStyleBackColor = True
        '
        'tpPowertrain
        '
        Me.tpPowertrain.Controls.Add(Me.GroupBox4)
        Me.tpPowertrain.Controls.Add(Me.GroupBox9)
        Me.tpPowertrain.Controls.Add(Me.gbPTO)
        Me.tpPowertrain.Controls.Add(Me.GroupBox7)
        Me.tpPowertrain.Controls.Add(Me.GroupBox2)
        Me.tpPowertrain.Location = New System.Drawing.Point(4, 29)
        Me.tpPowertrain.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpPowertrain.Name = "tpPowertrain"
        Me.tpPowertrain.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpPowertrain.Size = New System.Drawing.Size(872, 553)
        Me.tpPowertrain.TabIndex = 1
        Me.tpPowertrain.Text = "Powertrain"
        Me.tpPowertrain.UseVisualStyleBackColor = True
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.Panel1)
        Me.GroupBox4.Location = New System.Drawing.Point(9, 11)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox4.Size = New System.Drawing.Size(418, 97)
        Me.GroupBox4.TabIndex = 6
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Vehicle Idling Speed"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.tbVehIdlingSpeed)
        Me.Panel1.Controls.Add(Me.Label18)
        Me.Panel1.Controls.Add(Me.Label19)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(4, 24)
        Me.Panel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(410, 68)
        Me.Panel1.TabIndex = 0
        '
        'tbVehIdlingSpeed
        '
        Me.tbVehIdlingSpeed.Location = New System.Drawing.Point(260, 5)
        Me.tbVehIdlingSpeed.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbVehIdlingSpeed.Name = "tbVehIdlingSpeed"
        Me.tbVehIdlingSpeed.Size = New System.Drawing.Size(84, 26)
        Me.tbVehIdlingSpeed.TabIndex = 0
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(350, 9)
        Me.Label18.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(44, 20)
        Me.Label18.TabIndex = 24
        Me.Label18.Text = "[rpm]"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(110, 9)
        Me.Label19.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(140, 20)
        Me.Label19.TabIndex = 8
        Me.Label19.Text = "Engine Idle Speed"
        '
        'GroupBox9
        '
        Me.GroupBox9.Controls.Add(Me.cbTankSystem)
        Me.GroupBox9.Controls.Add(Me.Label23)
        Me.GroupBox9.Location = New System.Drawing.Point(436, 11)
        Me.GroupBox9.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox9.Name = "GroupBox9"
        Me.GroupBox9.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox9.Size = New System.Drawing.Size(418, 97)
        Me.GroupBox9.TabIndex = 5
        Me.GroupBox9.TabStop = False
        Me.GroupBox9.Text = "Tank System"
        '
        'cbTankSystem
        '
        Me.cbTankSystem.FormattingEnabled = True
        Me.cbTankSystem.Location = New System.Drawing.Point(14, 51)
        Me.cbTankSystem.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbTankSystem.Name = "cbTankSystem"
        Me.cbTankSystem.Size = New System.Drawing.Size(368, 28)
        Me.cbTankSystem.TabIndex = 1
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label23.Location = New System.Drawing.Point(9, 25)
        Me.Label23.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(277, 20)
        Me.Label23.TabIndex = 0
        Me.Label23.Text = "Only applicable for NG engines!"
        '
        'tpElectricComponents
        '
        Me.tpElectricComponents.Controls.Add(Me.lblMaxDrivetrainPwrUnit)
        Me.tpElectricComponents.Controls.Add(Me.tbMaxDrivetrainPwr)
        Me.tpElectricComponents.Controls.Add(Me.lblMaxDrivetrainPwr)
        Me.tpElectricComponents.Controls.Add(Me.gbBattery)
        Me.tpElectricComponents.Controls.Add(Me.gpElectricMotor)
        Me.tpElectricComponents.Location = New System.Drawing.Point(4, 29)
        Me.tpElectricComponents.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpElectricComponents.Name = "tpElectricComponents"
        Me.tpElectricComponents.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpElectricComponents.Size = New System.Drawing.Size(872, 553)
        Me.tpElectricComponents.TabIndex = 4
        Me.tpElectricComponents.Text = "Electric Components"
        Me.tpElectricComponents.UseVisualStyleBackColor = True
        '
        'gbBattery
        '
        Me.gbBattery.Controls.Add(Me.lblInitialSoCUnit)
        Me.gbBattery.Controls.Add(Me.tbInitialSoC)
        Me.gbBattery.Controls.Add(Me.lblInitialSoC)
        Me.gbBattery.Controls.Add(Me.tbBatteryPackCnt)
        Me.gbBattery.Controls.Add(Me.lblBatteryPackCnt)
        Me.gbBattery.Controls.Add(Me.Panel2)
        Me.gbBattery.Location = New System.Drawing.Point(9, 269)
        Me.gbBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.gbBattery.Name = "gbBattery"
        Me.gbBattery.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.gbBattery.Size = New System.Drawing.Size(848, 182)
        Me.gbBattery.TabIndex = 27
        Me.gbBattery.TabStop = False
        Me.gbBattery.Text = "Electric Energy Storage system"
        '
        'lblInitialSoCUnit
        '
        Me.lblInitialSoCUnit.AutoSize = True
        Me.lblInitialSoCUnit.Location = New System.Drawing.Point(404, 125)
        Me.lblInitialSoCUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblInitialSoCUnit.Name = "lblInitialSoCUnit"
        Me.lblInitialSoCUnit.Size = New System.Drawing.Size(31, 20)
        Me.lblInitialSoCUnit.TabIndex = 27
        Me.lblInitialSoCUnit.Text = "[%]"
        '
        'tbInitialSoC
        '
        Me.tbInitialSoC.Location = New System.Drawing.Point(306, 120)
        Me.tbInitialSoC.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbInitialSoC.Name = "tbInitialSoC"
        Me.tbInitialSoC.Size = New System.Drawing.Size(86, 26)
        Me.tbInitialSoC.TabIndex = 26
        '
        'lblInitialSoC
        '
        Me.lblInitialSoC.AutoSize = True
        Me.lblInitialSoC.Location = New System.Drawing.Point(10, 125)
        Me.lblInitialSoC.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblInitialSoC.Name = "lblInitialSoC"
        Me.lblInitialSoC.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblInitialSoC.Size = New System.Drawing.Size(81, 20)
        Me.lblInitialSoC.TabIndex = 25
        Me.lblInitialSoC.Text = "Initial SoC"
        '
        'tbBatteryPackCnt
        '
        Me.tbBatteryPackCnt.Location = New System.Drawing.Point(306, 80)
        Me.tbBatteryPackCnt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbBatteryPackCnt.Name = "tbBatteryPackCnt"
        Me.tbBatteryPackCnt.Size = New System.Drawing.Size(86, 26)
        Me.tbBatteryPackCnt.TabIndex = 24
        '
        'lblBatteryPackCnt
        '
        Me.lblBatteryPackCnt.AutoSize = True
        Me.lblBatteryPackCnt.Location = New System.Drawing.Point(10, 85)
        Me.lblBatteryPackCnt.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblBatteryPackCnt.Name = "lblBatteryPackCnt"
        Me.lblBatteryPackCnt.Size = New System.Drawing.Size(183, 20)
        Me.lblBatteryPackCnt.TabIndex = 23
        Me.lblBatteryPackCnt.Text = "Number of RESS Packs:"
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.btnOpenBattery)
        Me.Panel2.Controls.Add(Me.btnBrowseBattery)
        Me.Panel2.Controls.Add(Me.tbBattery)
        Me.Panel2.Location = New System.Drawing.Point(9, 29)
        Me.Panel2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(830, 42)
        Me.Panel2.TabIndex = 19
        '
        'btnOpenBattery
        '
        Me.btnOpenBattery.Location = New System.Drawing.Point(6, 5)
        Me.btnOpenBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnOpenBattery.Name = "btnOpenBattery"
        Me.btnOpenBattery.Size = New System.Drawing.Size(141, 32)
        Me.btnOpenBattery.TabIndex = 0
        Me.btnOpenBattery.TabStop = False
        Me.btnOpenBattery.Text = "REESS Pack"
        Me.btnOpenBattery.UseVisualStyleBackColor = True
        '
        'btnBrowseBattery
        '
        Me.btnBrowseBattery.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowseBattery.Image = CType(resources.GetObject("btnBrowseBattery.Image"), System.Drawing.Image)
        Me.btnBrowseBattery.Location = New System.Drawing.Point(790, 3)
        Me.btnBrowseBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnBrowseBattery.Name = "btnBrowseBattery"
        Me.btnBrowseBattery.Size = New System.Drawing.Size(36, 37)
        Me.btnBrowseBattery.TabIndex = 2
        Me.btnBrowseBattery.TabStop = False
        Me.btnBrowseBattery.UseVisualStyleBackColor = True
        '
        'tbBattery
        '
        Me.tbBattery.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbBattery.Location = New System.Drawing.Point(156, 6)
        Me.tbBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbBattery.Name = "tbBattery"
        Me.tbBattery.Size = New System.Drawing.Size(624, 26)
        Me.tbBattery.TabIndex = 1
        '
        'gpElectricMotor
        '
        Me.gpElectricMotor.Controls.Add(Me.tbEmEfficiency)
        Me.gpElectricMotor.Controls.Add(Me.lblEmEfficiency)
        Me.gpElectricMotor.Controls.Add(Me.tbRatioEm)
        Me.gpElectricMotor.Controls.Add(Me.lblRatioEm)
        Me.gpElectricMotor.Controls.Add(Me.tbEmCount)
        Me.gpElectricMotor.Controls.Add(Me.cbEmPos)
        Me.gpElectricMotor.Controls.Add(Me.lblEmCount)
        Me.gpElectricMotor.Controls.Add(Me.lblEmPosition)
        Me.gpElectricMotor.Controls.Add(Me.pnElectricMotor)
        Me.gpElectricMotor.Location = New System.Drawing.Point(9, 9)
        Me.gpElectricMotor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.gpElectricMotor.Name = "gpElectricMotor"
        Me.gpElectricMotor.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.gpElectricMotor.Size = New System.Drawing.Size(848, 251)
        Me.gpElectricMotor.TabIndex = 0
        Me.gpElectricMotor.TabStop = False
        Me.gpElectricMotor.Text = "Electric Motor"
        '
        'tbEmEfficiency
        '
        Me.tbEmEfficiency.Location = New System.Drawing.Point(306, 202)
        Me.tbEmEfficiency.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbEmEfficiency.Name = "tbEmEfficiency"
        Me.tbEmEfficiency.Size = New System.Drawing.Size(86, 26)
        Me.tbEmEfficiency.TabIndex = 26
        '
        'lblEmEfficiency
        '
        Me.lblEmEfficiency.AutoSize = True
        Me.lblEmEfficiency.Location = New System.Drawing.Point(10, 206)
        Me.lblEmEfficiency.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEmEfficiency.Name = "lblEmEfficiency"
        Me.lblEmEfficiency.Size = New System.Drawing.Size(198, 20)
        Me.lblEmEfficiency.TabIndex = 25
        Me.lblEmEfficiency.Text = "Efficiency EM to Drivetrain:"
        '
        'tbRatioEm
        '
        Me.tbRatioEm.Location = New System.Drawing.Point(306, 162)
        Me.tbRatioEm.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbRatioEm.Name = "tbRatioEm"
        Me.tbRatioEm.Size = New System.Drawing.Size(86, 26)
        Me.tbRatioEm.TabIndex = 24
        '
        'lblRatioEm
        '
        Me.lblRatioEm.AutoSize = True
        Me.lblRatioEm.Location = New System.Drawing.Point(10, 166)
        Me.lblRatioEm.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblRatioEm.Name = "lblRatioEm"
        Me.lblRatioEm.Size = New System.Drawing.Size(168, 20)
        Me.lblRatioEm.TabIndex = 23
        Me.lblRatioEm.Text = "Ratio EM to Drivetrain:"
        '
        'tbEmCount
        '
        Me.tbEmCount.Location = New System.Drawing.Point(306, 122)
        Me.tbEmCount.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbEmCount.Name = "tbEmCount"
        Me.tbEmCount.Size = New System.Drawing.Size(86, 26)
        Me.tbEmCount.TabIndex = 22
        '
        'cbEmPos
        '
        Me.cbEmPos.FormattingEnabled = True
        Me.cbEmPos.Location = New System.Drawing.Point(213, 80)
        Me.cbEmPos.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbEmPos.Name = "cbEmPos"
        Me.cbEmPos.Size = New System.Drawing.Size(180, 28)
        Me.cbEmPos.TabIndex = 21
        '
        'lblEmCount
        '
        Me.lblEmCount.AutoSize = True
        Me.lblEmCount.Location = New System.Drawing.Point(10, 126)
        Me.lblEmCount.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEmCount.Name = "lblEmCount"
        Me.lblEmCount.Size = New System.Drawing.Size(123, 20)
        Me.lblEmCount.TabIndex = 20
        Me.lblEmCount.Text = "Number of EMs:"
        '
        'lblEmPosition
        '
        Me.lblEmPosition.AutoSize = True
        Me.lblEmPosition.Location = New System.Drawing.Point(10, 86)
        Me.lblEmPosition.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEmPosition.Name = "lblEmPosition"
        Me.lblEmPosition.Size = New System.Drawing.Size(69, 20)
        Me.lblEmPosition.TabIndex = 19
        Me.lblEmPosition.Text = "Position:"
        '
        'pnElectricMotor
        '
        Me.pnElectricMotor.Controls.Add(Me.btnOpenElectricMotor)
        Me.pnElectricMotor.Controls.Add(Me.btnBrowseElectricMotor)
        Me.pnElectricMotor.Controls.Add(Me.tbElectricMotor)
        Me.pnElectricMotor.Location = New System.Drawing.Point(9, 29)
        Me.pnElectricMotor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnElectricMotor.Name = "pnElectricMotor"
        Me.pnElectricMotor.Size = New System.Drawing.Size(830, 42)
        Me.pnElectricMotor.TabIndex = 18
        '
        'btnOpenElectricMotor
        '
        Me.btnOpenElectricMotor.Location = New System.Drawing.Point(6, 5)
        Me.btnOpenElectricMotor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnOpenElectricMotor.Name = "btnOpenElectricMotor"
        Me.btnOpenElectricMotor.Size = New System.Drawing.Size(141, 32)
        Me.btnOpenElectricMotor.TabIndex = 0
        Me.btnOpenElectricMotor.TabStop = False
        Me.btnOpenElectricMotor.Text = "Electric Motor"
        Me.btnOpenElectricMotor.UseVisualStyleBackColor = True
        '
        'btnBrowseElectricMotor
        '
        Me.btnBrowseElectricMotor.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnBrowseElectricMotor.Image = CType(resources.GetObject("btnBrowseElectricMotor.Image"), System.Drawing.Image)
        Me.btnBrowseElectricMotor.Location = New System.Drawing.Point(790, 3)
        Me.btnBrowseElectricMotor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnBrowseElectricMotor.Name = "btnBrowseElectricMotor"
        Me.btnBrowseElectricMotor.Size = New System.Drawing.Size(36, 37)
        Me.btnBrowseElectricMotor.TabIndex = 2
        Me.btnBrowseElectricMotor.TabStop = False
        Me.btnBrowseElectricMotor.UseVisualStyleBackColor = True
        '
        'tbElectricMotor
        '
        Me.tbElectricMotor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbElectricMotor.Location = New System.Drawing.Point(156, 6)
        Me.tbElectricMotor.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbElectricMotor.Name = "tbElectricMotor"
        Me.tbElectricMotor.Size = New System.Drawing.Size(624, 26)
        Me.tbElectricMotor.TabIndex = 1
        '
        'tpTorqueLimits
        '
        Me.tpTorqueLimits.Controls.Add(Me.lvTorqueLimits)
        Me.tpTorqueLimits.Controls.Add(Me.Label17)
        Me.tpTorqueLimits.Controls.Add(Me.btDelMaxTorqueEntry)
        Me.tpTorqueLimits.Controls.Add(Me.btAddMaxTorqueEntry)
        Me.tpTorqueLimits.Location = New System.Drawing.Point(4, 29)
        Me.tpTorqueLimits.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpTorqueLimits.Name = "tpTorqueLimits"
        Me.tpTorqueLimits.Size = New System.Drawing.Size(872, 553)
        Me.tpTorqueLimits.TabIndex = 2
        Me.tpTorqueLimits.Text = "Torque Limits"
        Me.tpTorqueLimits.UseVisualStyleBackColor = True
        '
        'lvTorqueLimits
        '
        Me.lvTorqueLimits.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.lvTorqueLimits.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader5, Me.ColumnHeader6})
        Me.lvTorqueLimits.FullRowSelect = True
        Me.lvTorqueLimits.GridLines = True
        Me.lvTorqueLimits.HideSelection = False
        Me.lvTorqueLimits.Location = New System.Drawing.Point(10, 12)
        Me.lvTorqueLimits.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.lvTorqueLimits.MultiSelect = False
        Me.lvTorqueLimits.Name = "lvTorqueLimits"
        Me.lvTorqueLimits.Size = New System.Drawing.Size(421, 155)
        Me.lvTorqueLimits.TabIndex = 7
        Me.lvTorqueLimits.TabStop = False
        Me.lvTorqueLimits.UseCompatibleStateImageBehavior = False
        Me.lvTorqueLimits.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Gear #"
        Me.ColumnHeader5.Width = 67
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Max. Torque"
        Me.ColumnHeader6.Width = 146
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(274, 174)
        Me.Label17.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(158, 20)
        Me.Label17.TabIndex = 6
        Me.Label17.Text = "(Double-Click to Edit)"
        '
        'btDelMaxTorqueEntry
        '
        Me.btDelMaxTorqueEntry.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.btDelMaxTorqueEntry.Location = New System.Drawing.Point(46, 178)
        Me.btDelMaxTorqueEntry.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btDelMaxTorqueEntry.Name = "btDelMaxTorqueEntry"
        Me.btDelMaxTorqueEntry.Size = New System.Drawing.Size(36, 37)
        Me.btDelMaxTorqueEntry.TabIndex = 5
        Me.btDelMaxTorqueEntry.UseVisualStyleBackColor = True
        '
        'btAddMaxTorqueEntry
        '
        Me.btAddMaxTorqueEntry.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.btAddMaxTorqueEntry.Location = New System.Drawing.Point(10, 178)
        Me.btAddMaxTorqueEntry.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btAddMaxTorqueEntry.Name = "btAddMaxTorqueEntry"
        Me.btAddMaxTorqueEntry.Size = New System.Drawing.Size(36, 37)
        Me.btAddMaxTorqueEntry.TabIndex = 4
        Me.btAddMaxTorqueEntry.UseVisualStyleBackColor = True
        '
        'tpADAS
        '
        Me.tpADAS.Controls.Add(Me.GroupBox5)
        Me.tpADAS.Location = New System.Drawing.Point(4, 29)
        Me.tpADAS.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpADAS.Name = "tpADAS"
        Me.tpADAS.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tpADAS.Size = New System.Drawing.Size(872, 553)
        Me.tpADAS.TabIndex = 3
        Me.tpADAS.Text = "ADAS"
        Me.tpADAS.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.cbAtEcoRollReleaseLockupClutch)
        Me.GroupBox5.Controls.Add(Me.cbPcc)
        Me.GroupBox5.Controls.Add(Me.cbEcoRoll)
        Me.GroupBox5.Controls.Add(Me.Label22)
        Me.GroupBox5.Controls.Add(Me.cbEngineStopStart)
        Me.GroupBox5.Controls.Add(Me.lblPCC)
        Me.GroupBox5.Location = New System.Drawing.Point(9, 9)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox5.Size = New System.Drawing.Size(848, 209)
        Me.GroupBox5.TabIndex = 0
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "ADAS Options"
        '
        'cbAtEcoRollReleaseLockupClutch
        '
        Me.cbAtEcoRollReleaseLockupClutch.AutoSize = True
        Me.cbAtEcoRollReleaseLockupClutch.Location = New System.Drawing.Point(398, 29)
        Me.cbAtEcoRollReleaseLockupClutch.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbAtEcoRollReleaseLockupClutch.Name = "cbAtEcoRollReleaseLockupClutch"
        Me.cbAtEcoRollReleaseLockupClutch.Size = New System.Drawing.Size(356, 24)
        Me.cbAtEcoRollReleaseLockupClutch.TabIndex = 9
        Me.cbAtEcoRollReleaseLockupClutch.Text = "AT Gearbox: Eco-Roll Release Lockup Clutch"
        Me.cbAtEcoRollReleaseLockupClutch.UseVisualStyleBackColor = True
        '
        'cbPcc
        '
        Me.cbPcc.FormattingEnabled = True
        Me.cbPcc.Location = New System.Drawing.Point(27, 160)
        Me.cbPcc.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbPcc.Name = "cbPcc"
        Me.cbPcc.Size = New System.Drawing.Size(266, 28)
        Me.cbPcc.TabIndex = 8
        '
        'cbEcoRoll
        '
        Me.cbEcoRoll.FormattingEnabled = True
        Me.cbEcoRoll.Location = New System.Drawing.Point(27, 89)
        Me.cbEcoRoll.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbEcoRoll.Name = "cbEcoRoll"
        Me.cbEcoRoll.Size = New System.Drawing.Size(266, 28)
        Me.cbEcoRoll.TabIndex = 7
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(9, 65)
        Me.Label22.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(69, 20)
        Me.Label22.TabIndex = 6
        Me.Label22.Text = "Eco-Roll"
        '
        'cbEngineStopStart
        '
        Me.cbEngineStopStart.AutoSize = True
        Me.cbEngineStopStart.Location = New System.Drawing.Point(9, 29)
        Me.cbEngineStopStart.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbEngineStopStart.Name = "cbEngineStopStart"
        Me.cbEngineStopStart.Size = New System.Drawing.Size(297, 24)
        Me.cbEngineStopStart.TabIndex = 4
        Me.cbEngineStopStart.Text = "Engine Stop/Start during vehicle stop"
        Me.cbEngineStopStart.UseVisualStyleBackColor = True
        '
        'lblPCC
        '
        Me.lblPCC.AutoSize = True
        Me.lblPCC.Location = New System.Drawing.Point(9, 135)
        Me.lblPCC.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblPCC.Name = "lblPCC"
        Me.lblPCC.Size = New System.Drawing.Size(181, 20)
        Me.lblPCC.TabIndex = 3
        Me.lblPCC.Text = "Predictive Cruise Control"
        '
        'cbLegislativeClass
        '
        Me.cbLegislativeClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLegislativeClass.FormattingEnabled = True
        Me.cbLegislativeClass.Location = New System.Drawing.Point(330, 215)
        Me.cbLegislativeClass.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cbLegislativeClass.Name = "cbLegislativeClass"
        Me.cbLegislativeClass.Size = New System.Drawing.Size(76, 28)
        Me.cbLegislativeClass.TabIndex = 41
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(48, 189)
        Me.Label21.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(167, 20)
        Me.Label21.TabIndex = 42
        Me.Label21.Text = "Maximum Laden Mass"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = True
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTitle.Location = New System.Drawing.Point(182, 52)
        Me.lblTitle.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(242, 40)
        Me.lblTitle.TabIndex = 43
        Me.lblTitle.Text = "Vehicle TITLE"
        '
        'lblMaxDrivetrainPwrUnit
        '
        Me.lblMaxDrivetrainPwrUnit.AutoSize = True
        Me.lblMaxDrivetrainPwrUnit.Location = New System.Drawing.Point(414, 467)
        Me.lblMaxDrivetrainPwrUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMaxDrivetrainPwrUnit.Name = "lblMaxDrivetrainPwrUnit"
        Me.lblMaxDrivetrainPwrUnit.Size = New System.Drawing.Size(31, 20)
        Me.lblMaxDrivetrainPwrUnit.TabIndex = 30
        Me.lblMaxDrivetrainPwrUnit.Text = "[%]"
        '
        'tbMaxDrivetrainPwr
        '
        Me.tbMaxDrivetrainPwr.Location = New System.Drawing.Point(316, 462)
        Me.tbMaxDrivetrainPwr.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbMaxDrivetrainPwr.Name = "tbMaxDrivetrainPwr"
        Me.tbMaxDrivetrainPwr.Size = New System.Drawing.Size(86, 26)
        Me.tbMaxDrivetrainPwr.TabIndex = 29
        '
        'lblMaxDrivetrainPwr
        '
        Me.lblMaxDrivetrainPwr.AutoSize = True
        Me.lblMaxDrivetrainPwr.Location = New System.Drawing.Point(20, 467)
        Me.lblMaxDrivetrainPwr.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMaxDrivetrainPwr.Name = "lblMaxDrivetrainPwr"
        Me.lblMaxDrivetrainPwr.Size = New System.Drawing.Size(242, 30)
        Me.lblMaxDrivetrainPwr.TabIndex = 28
        Me.lblMaxDrivetrainPwr.Text = "Max. Drivetrain Power"
        '
        'VehicleForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(898, 935)
        Me.Controls.Add(Me.lblTitle)
        Me.Controls.Add(Me.Label21)
        Me.Controls.Add(Me.cbLegislativeClass)
        Me.Controls.Add(Me.tcVehicleComponents)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.TbHDVclass)
        Me.Controls.Add(Me.PicVehicle)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.Label9)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Controls.Add(Me.CbAxleConfig)
        Me.Controls.Add(Me.TbMassMass)
        Me.Controls.Add(Me.CbCat)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.ToolStrip1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = False
        Me.Name = "VehicleForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
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
        Me.gbPTO.ResumeLayout(False)
        Me.pnPTO.ResumeLayout(False)
        Me.pnPTO.PerformLayout()
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.pnAngledriveFields.ResumeLayout(False)
        Me.pnAngledriveFields.PerformLayout()
        CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tcVehicleComponents.ResumeLayout(False)
        Me.tpGeneral.ResumeLayout(False)
        Me.tpPowertrain.ResumeLayout(False)
        Me.GroupBox4.ResumeLayout(False)
        Me.Panel1.ResumeLayout(False)
        Me.Panel1.PerformLayout()
        Me.GroupBox9.ResumeLayout(False)
        Me.GroupBox9.PerformLayout()
        Me.tpElectricComponents.ResumeLayout(False)
        Me.tpElectricComponents.PerformLayout()
        Me.gbBattery.ResumeLayout(False)
        Me.gbBattery.PerformLayout()
        Me.Panel2.ResumeLayout(False)
        Me.Panel2.PerformLayout()
        Me.gpElectricMotor.ResumeLayout(False)
        Me.gpElectricMotor.PerformLayout()
        Me.pnElectricMotor.ResumeLayout(False)
        Me.pnElectricMotor.PerformLayout()
        Me.tpTorqueLimits.ResumeLayout(False)
        Me.tpTorqueLimits.PerformLayout()
        Me.tpADAS.ResumeLayout(False)
        Me.GroupBox5.ResumeLayout(False)
        Me.GroupBox5.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout

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
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents ColumnHeader3 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader4 As System.Windows.Forms.ColumnHeader
	Friend WithEvents PnWheelDiam As System.Windows.Forms.Panel
	Friend WithEvents PicVehicle As System.Windows.Forms.PictureBox
	Friend WithEvents Label8 As System.Windows.Forms.Label
	Friend WithEvents PnCdATrTr As System.Windows.Forms.Panel
	Friend WithEvents Label38 As System.Windows.Forms.Label
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents cbAngledriveType As System.Windows.Forms.ComboBox
	Friend WithEvents Label15 As System.Windows.Forms.Label
	Friend WithEvents BtRtBrowse As System.Windows.Forms.Button
	Friend WithEvents TbRtPath As System.Windows.Forms.TextBox
	Friend WithEvents pnAngledriveFields As System.Windows.Forms.Panel
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents tbAngledriveRatio As System.Windows.Forms.TextBox
	Friend WithEvents btAngledriveLossMapBrowse As System.Windows.Forms.Button
	Friend WithEvents tbAngledriveLossMapPath As System.Windows.Forms.TextBox
	Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents tbPTOLossMap As System.Windows.Forms.TextBox
	Friend WithEvents gbPTO As System.Windows.Forms.GroupBox
	Friend WithEvents btPTOLossMapBrowse As System.Windows.Forms.Button
	Friend WithEvents cbPTOType As System.Windows.Forms.ComboBox
	Friend WithEvents pnPTO As System.Windows.Forms.Panel
	Friend WithEvents btPTOCycle As System.Windows.Forms.Button
	Friend WithEvents Label16 As System.Windows.Forms.Label
	Friend WithEvents tbPTOCycle As System.Windows.Forms.TextBox
	Friend WithEvents tcVehicleComponents As System.Windows.Forms.TabControl
	Friend WithEvents tpGeneral As System.Windows.Forms.TabPage
	Friend WithEvents tpPowertrain As System.Windows.Forms.TabPage
	Friend WithEvents tpTorqueLimits As System.Windows.Forms.TabPage
	Friend WithEvents Label17 As System.Windows.Forms.Label
	Friend WithEvents btDelMaxTorqueEntry As System.Windows.Forms.Button
	Friend WithEvents btAddMaxTorqueEntry As System.Windows.Forms.Button
	Friend WithEvents lvTorqueLimits As System.Windows.Forms.ListView
	Friend WithEvents ColumnHeader5 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader6 As System.Windows.Forms.ColumnHeader
	Friend WithEvents ColumnHeader10 As System.Windows.Forms.ColumnHeader
    Friend WithEvents cbLegislativeClass As System.Windows.Forms.ComboBox
    Friend WithEvents tbVehicleHeight As System.Windows.Forms.TextBox
    Friend WithEvents Label11 As System.Windows.Forms.Label
    Friend WithEvents Label20 As System.Windows.Forms.Label
    Friend WithEvents Label21 As Label
    Friend WithEvents tpADAS As TabPage
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents cbEngineStopStart As CheckBox
    Friend WithEvents lblPCC As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents cbPcc As ComboBox
    Friend WithEvents cbEcoRoll As ComboBox
    Friend WithEvents GroupBox9 As GroupBox
    Friend WithEvents cbTankSystem As ComboBox
    Friend WithEvents Label23 As Label
    Friend WithEvents cbAtEcoRollReleaseLockupClutch As CheckBox
    Friend WithEvents tpElectricComponents As TabPage
    Friend WithEvents gpElectricMotor As GroupBox
    Friend WithEvents gbBattery As GroupBox
    Friend WithEvents tbEmEfficiency As TextBox
    Friend WithEvents lblEmEfficiency As Label
    Friend WithEvents tbRatioEm As TextBox
    Friend WithEvents lblRatioEm As Label
    Friend WithEvents tbEmCount As TextBox
    Friend WithEvents cbEmPos As ComboBox
    Friend WithEvents lblEmCount As Label
    Friend WithEvents lblEmPosition As Label
    Friend WithEvents pnElectricMotor As Panel
    Friend WithEvents btnOpenElectricMotor As Button
    Friend WithEvents btnBrowseElectricMotor As Button
    Friend WithEvents tbElectricMotor As TextBox
    Friend WithEvents tbBatteryPackCnt As TextBox
    Friend WithEvents lblBatteryPackCnt As Label
    Friend WithEvents Panel2 As Panel
    Friend WithEvents btnOpenBattery As Button
    Friend WithEvents btnBrowseBattery As Button
    Friend WithEvents tbBattery As TextBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents tbVehIdlingSpeed As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents lblInitialSoCUnit As Label
    Friend WithEvents tbInitialSoC As TextBox
    Friend WithEvents lblInitialSoC As Label
    Friend WithEvents lblMaxDrivetrainPwrUnit As Label
    Friend WithEvents tbMaxDrivetrainPwr As TextBox
    Friend WithEvents lblMaxDrivetrainPwr As Label
End Class
