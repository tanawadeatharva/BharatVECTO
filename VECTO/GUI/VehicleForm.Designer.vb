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
        Me.gbRetarderLosses = New System.Windows.Forms.GroupBox()
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
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.gbAngledrive = New System.Windows.Forms.GroupBox()
        Me.pnAngledriveFields = New System.Windows.Forms.Panel()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.tbAngledriveRatio = New System.Windows.Forms.TextBox()
        Me.btAngledriveLossMapBrowse = New System.Windows.Forms.Button()
        Me.tbAngledriveLossMapPath = New System.Windows.Forms.TextBox()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.cbAngledriveType = New System.Windows.Forms.ComboBox()
        Me.PicVehicle = New System.Windows.Forms.PictureBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.cbPTOType = New System.Windows.Forms.ComboBox()
        Me.tbPTOCycle = New System.Windows.Forms.TextBox()
        Me.tbPTOLossMap = New System.Windows.Forms.TextBox()
        Me.tbPTODrive = New System.Windows.Forms.TextBox()
        Me.tcVehicleComponents = New System.Windows.Forms.TabControl()
        Me.tpGeneral = New System.Windows.Forms.TabPage()
        Me.tpPowertrain = New System.Windows.Forms.TabPage()
        Me.gbVehicleIdlingSpeed = New System.Windows.Forms.GroupBox()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.tbVehIdlingSpeed = New System.Windows.Forms.TextBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.gbTankSystem = New System.Windows.Forms.GroupBox()
        Me.cbTankSystem = New System.Windows.Forms.ComboBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.tpElectricMachine = New System.Windows.Forms.TabPage()
        Me.gpElectricMotor = New System.Windows.Forms.GroupBox()
        Me.Label33 = New System.Windows.Forms.Label()
        Me.Label32 = New System.Windows.Forms.Label()
        Me.gbRatiosPerGear = New System.Windows.Forms.GroupBox()
        Me.lvRatioPerGear = New System.Windows.Forms.ListView()
        Me.ColumnHeader11 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader12 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.btnAddEMRatio = New System.Windows.Forms.Button()
        Me.Label29 = New System.Windows.Forms.Label()
        Me.btnRemoveEMRatio = New System.Windows.Forms.Button()
        Me.btnEmADCLossMap = New System.Windows.Forms.Button()
        Me.tbEmADCLossMap = New System.Windows.Forms.TextBox()
        Me.lblEmADCLossmap = New System.Windows.Forms.Label()
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
        Me.tpIEPC = New System.Windows.Forms.TabPage()
        Me.btIEPCFilePath = New System.Windows.Forms.Button()
        Me.tbIEPCFilePath = New System.Windows.Forms.TextBox()
        Me.btnIEPC = New System.Windows.Forms.Button()
        Me.tbIHPC = New System.Windows.Forms.TabPage()
        Me.btIHPC = New System.Windows.Forms.Button()
        Me.btIHPCFile = New System.Windows.Forms.Button()
        Me.tbIHPCFilePath = New System.Windows.Forms.TextBox()
        Me.tpReess = New System.Windows.Forms.TabPage()
        Me.gbBattery = New System.Windows.Forms.GroupBox()
        Me.lvREESSPacks = New System.Windows.Forms.ListView()
        Me.chReessPackPack = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.chReessPackCount = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.chReessPackStringId = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.btnAddReessPack = New System.Windows.Forms.Button()
        Me.lblEditReessPack = New System.Windows.Forms.Label()
        Me.btnRemoveReessPack = New System.Windows.Forms.Button()
        Me.lblInitialSoCUnit = New System.Windows.Forms.Label()
        Me.tbInitialSoC = New System.Windows.Forms.TextBox()
        Me.lblInitialSoC = New System.Windows.Forms.Label()
        Me.tpGensetComponents = New System.Windows.Forms.TabPage()
        Me.Label30 = New System.Windows.Forms.Label()
        Me.gbGenSet = New System.Windows.Forms.GroupBox()
        Me.Label36 = New System.Windows.Forms.Label()
        Me.Label34 = New System.Windows.Forms.Label()
        Me.btnGenSetLossMap = New System.Windows.Forms.Button()
        Me.tbGenSetADC = New System.Windows.Forms.TextBox()
        Me.lblGenSetADC = New System.Windows.Forms.Label()
        Me.tbGenSetRatio = New System.Windows.Forms.TextBox()
        Me.lblGenSetRatio = New System.Windows.Forms.Label()
        Me.tbGenSetCount = New System.Windows.Forms.TextBox()
        Me.lblGenSetCount = New System.Windows.Forms.Label()
        Me.pnGenSetEM = New System.Windows.Forms.Panel()
        Me.btnOpenGenSetEM = New System.Windows.Forms.Button()
        Me.btnBrowseGenSetEM = New System.Windows.Forms.Button()
        Me.tbGenSetEM = New System.Windows.Forms.TextBox()
        Me.tpTorqueLimits = New System.Windows.Forms.TabPage()
        Me.gbPropulsionTorque = New System.Windows.Forms.GroupBox()
        Me.btnPropulsionTorqueLimit = New System.Windows.Forms.Button()
        Me.tbPropulsionTorqueLimit = New System.Windows.Forms.TextBox()
        Me.gbEMTorqueLimits = New System.Windows.Forms.GroupBox()
        Me.btnEmTorqueLimits = New System.Windows.Forms.Button()
        Me.tbEmTorqueLimits = New System.Windows.Forms.TextBox()
        Me.bgVehicleTorqueLimits = New System.Windows.Forms.GroupBox()
        Me.lvTorqueLimits = New System.Windows.Forms.ListView()
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
        Me.btAddMaxTorqueEntry = New System.Windows.Forms.Button()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.btDelMaxTorqueEntry = New System.Windows.Forms.Button()
        Me.tpADAS = New System.Windows.Forms.TabPage()
        Me.gbADAS = New System.Windows.Forms.GroupBox()
        Me.pnEcoRoll = New System.Windows.Forms.Panel()
        Me.cbEcoRoll = New System.Windows.Forms.ComboBox()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.cbAtEcoRollReleaseLockupClutch = New System.Windows.Forms.CheckBox()
        Me.cbPcc = New System.Windows.Forms.ComboBox()
        Me.cbEngineStopStart = New System.Windows.Forms.CheckBox()
        Me.lblPCC = New System.Windows.Forms.Label()
        Me.tpRoadSweeper = New System.Windows.Forms.TabPage()
        Me.pnPTO = New System.Windows.Forms.Panel()
        Me.gbPTODrive = New System.Windows.Forms.GroupBox()
        Me.Label27 = New System.Windows.Forms.Label()
        Me.tbPtoGear = New System.Windows.Forms.TextBox()
        Me.Label26 = New System.Windows.Forms.Label()
        Me.tbPtoEngineSpeed = New System.Windows.Forms.TextBox()
        Me.Label25 = New System.Windows.Forms.Label()
        Me.Label24 = New System.Windows.Forms.Label()
        Me.btPTOCycleDrive = New System.Windows.Forms.Button()
        Me.Label28 = New System.Windows.Forms.Label()
        Me.btPTOCycle = New System.Windows.Forms.Button()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.btPTOLossMapBrowse = New System.Windows.Forms.Button()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.gbPTO = New System.Windows.Forms.GroupBox()
        Me.cbLegislativeClass = New System.Windows.Forms.ComboBox()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.lblTitle = New System.Windows.Forms.Label()
        Me.FlowLayoutPanel1 = New System.Windows.Forms.FlowLayoutPanel()
        Me.FlowLayoutPanel2 = New System.Windows.Forms.FlowLayoutPanel()
        Me.GroupBox6.SuspendLayout
        Me.ToolStrip1.SuspendLayout
        Me.gbRetarderLosses.SuspendLayout
        Me.PnRt.SuspendLayout
        Me.GroupBox8.SuspendLayout
        Me.PnWheelDiam.SuspendLayout
        Me.StatusStrip1.SuspendLayout
        Me.GroupBox1.SuspendLayout
        Me.PnLoad.SuspendLayout
        Me.GrAirRes.SuspendLayout
        Me.PnCdATrTr.SuspendLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).BeginInit
        Me.CmOpenFile.SuspendLayout
        Me.GroupBox3.SuspendLayout
        Me.gbAngledrive.SuspendLayout
        Me.pnAngledriveFields.SuspendLayout
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).BeginInit
        Me.tcVehicleComponents.SuspendLayout
        Me.tpGeneral.SuspendLayout
        Me.tpPowertrain.SuspendLayout
        Me.gbVehicleIdlingSpeed.SuspendLayout
        Me.Panel1.SuspendLayout
        Me.gbTankSystem.SuspendLayout
        Me.tpElectricMachine.SuspendLayout
        Me.gpElectricMotor.SuspendLayout
        Me.gbRatiosPerGear.SuspendLayout
        Me.pnElectricMotor.SuspendLayout
        Me.tpIEPC.SuspendLayout
        Me.tbIHPC.SuspendLayout
        Me.tpReess.SuspendLayout
        Me.gbBattery.SuspendLayout
        Me.tpGensetComponents.SuspendLayout
        Me.gbGenSet.SuspendLayout
        Me.pnGenSetEM.SuspendLayout
        Me.tpTorqueLimits.SuspendLayout
        Me.gbPropulsionTorque.SuspendLayout
        Me.gbEMTorqueLimits.SuspendLayout
        Me.bgVehicleTorqueLimits.SuspendLayout
        Me.tpADAS.SuspendLayout
        Me.gbADAS.SuspendLayout
        Me.pnEcoRoll.SuspendLayout
        Me.tpRoadSweeper.SuspendLayout
        Me.pnPTO.SuspendLayout
        Me.gbPTODrive.SuspendLayout
        Me.gbPTO.SuspendLayout
        Me.FlowLayoutPanel1.SuspendLayout
        Me.FlowLayoutPanel2.SuspendLayout
        Me.SuspendLayout
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(6, 22)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(177, 13)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Corrected Actual Curb Mass Vehicle"
        '
        'TbMass
        '
        Me.TbMass.Location = New System.Drawing.Point(188, 19)
        Me.TbMass.Name = "TbMass"
        Me.TbMass.Size = New System.Drawing.Size(57, 20)
        Me.TbMass.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = true
        Me.Label2.Location = New System.Drawing.Point(128, 31)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(45, 13)
        Me.Label2.TabIndex = 2
        Me.Label2.Text = "Loading"
        '
        'TbLoad
        '
        Me.TbLoad.Location = New System.Drawing.Point(182, 28)
        Me.TbLoad.Name = "TbLoad"
        Me.TbLoad.Size = New System.Drawing.Size(57, 20)
        Me.TbLoad.TabIndex = 1
        '
        'Label3
        '
        Me.Label3.AutoSize = true
        Me.Label3.Location = New System.Drawing.Point(10, 6)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(38, 13)
        Me.Label3.TabIndex = 8
        Me.Label3.Text = "Cd x A"
        '
        'TBcdA
        '
        Me.TBcdA.Location = New System.Drawing.Point(54, 3)
        Me.TBcdA.Name = "TBcdA"
        Me.TBcdA.Size = New System.Drawing.Size(57, 20)
        Me.TBcdA.TabIndex = 0
        '
        'Label13
        '
        Me.Label13.AutoSize = true
        Me.Label13.Location = New System.Drawing.Point(136, 6)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(40, 13)
        Me.Label13.TabIndex = 6
        Me.Label13.Text = "Radius"
        '
        'TBrdyn
        '
        Me.TBrdyn.Location = New System.Drawing.Point(185, 3)
        Me.TBrdyn.Name = "TBrdyn"
        Me.TBrdyn.Size = New System.Drawing.Size(57, 20)
        Me.TBrdyn.TabIndex = 0
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(498, 577)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(75, 23)
        Me.ButOK.TabIndex = 6
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = true
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(579, 577)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(75, 23)
        Me.ButCancel.TabIndex = 7
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = true
        '
        'Label14
        '
        Me.Label14.AutoSize = true
        Me.Label14.Location = New System.Drawing.Point(247, 22)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(25, 13)
        Me.Label14.TabIndex = 24
        Me.Label14.Text = "[kg]"
        '
        'Label31
        '
        Me.Label31.AutoSize = true
        Me.Label31.Location = New System.Drawing.Point(241, 31)
        Me.Label31.Name = "Label31"
        Me.Label31.Size = New System.Drawing.Size(25, 13)
        Me.Label31.TabIndex = 24
        Me.Label31.Text = "[kg]"
        '
        'Label35
        '
        Me.Label35.AutoSize = true
        Me.Label35.Location = New System.Drawing.Point(244, 6)
        Me.Label35.Name = "Label35"
        Me.Label35.Size = New System.Drawing.Size(29, 13)
        Me.Label35.TabIndex = 24
        Me.Label35.Text = "[mm]"
        '
        'CbCdMode
        '
        Me.CbCdMode.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.CbCdMode.DisplayMember = "Value"
        Me.CbCdMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCdMode.FormattingEnabled = true
        Me.CbCdMode.Items.AddRange(New Object() {"No Correction", "Speed dependent (User-defined)", "Speed dependent (Declaration Mode)", "Vair & Beta Input"})
        Me.CbCdMode.Location = New System.Drawing.Point(6, 19)
        Me.CbCdMode.Name = "CbCdMode"
        Me.CbCdMode.Size = New System.Drawing.Size(333, 21)
        Me.CbCdMode.TabIndex = 0
        Me.CbCdMode.ValueMember = "Key"
        '
        'TbCdFile
        '
        Me.TbCdFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbCdFile.Enabled = false
        Me.TbCdFile.Location = New System.Drawing.Point(9, 65)
        Me.TbCdFile.Name = "TbCdFile"
        Me.TbCdFile.Size = New System.Drawing.Size(276, 20)
        Me.TbCdFile.TabIndex = 1
        '
        'BtCdFileBrowse
        '
        Me.BtCdFileBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtCdFileBrowse.Enabled = false
        Me.BtCdFileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtCdFileBrowse.Location = New System.Drawing.Point(291, 62)
        Me.BtCdFileBrowse.Name = "BtCdFileBrowse"
        Me.BtCdFileBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtCdFileBrowse.TabIndex = 2
        Me.BtCdFileBrowse.UseVisualStyleBackColor = true
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.BtCdFileOpen)
        Me.GroupBox6.Controls.Add(Me.LbCdMode)
        Me.GroupBox6.Controls.Add(Me.CbCdMode)
        Me.GroupBox6.Controls.Add(Me.BtCdFileBrowse)
        Me.GroupBox6.Controls.Add(Me.TbCdFile)
        Me.GroupBox6.Location = New System.Drawing.Point(293, 84)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(346, 96)
        Me.GroupBox6.TabIndex = 3
        Me.GroupBox6.TabStop = false
        Me.GroupBox6.Text = "Cross Wind Correction"
        '
        'BtCdFileOpen
        '
        Me.BtCdFileOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtCdFileOpen.Enabled = false
        Me.BtCdFileOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
        Me.BtCdFileOpen.Location = New System.Drawing.Point(315, 62)
        Me.BtCdFileOpen.Name = "BtCdFileOpen"
        Me.BtCdFileOpen.Size = New System.Drawing.Size(24, 24)
        Me.BtCdFileOpen.TabIndex = 3
        Me.BtCdFileOpen.UseVisualStyleBackColor = true
        '
        'LbCdMode
        '
        Me.LbCdMode.AutoSize = true
        Me.LbCdMode.Location = New System.Drawing.Point(6, 47)
        Me.LbCdMode.Name = "LbCdMode"
        Me.LbCdMode.Size = New System.Drawing.Size(59, 13)
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
        Me.ToolStrip1.Padding = New System.Windows.Forms.Padding(0, 0, 2, 0)
        Me.ToolStrip1.Size = New System.Drawing.Size(666, 31)
        Me.ToolStrip1.TabIndex = 29
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
        'gbRetarderLosses
        '
        Me.gbRetarderLosses.Controls.Add(Me.PnRt)
        Me.gbRetarderLosses.Controls.Add(Me.CbRtType)
        Me.gbRetarderLosses.Location = New System.Drawing.Point(6, 76)
        Me.gbRetarderLosses.Name = "gbRetarderLosses"
        Me.gbRetarderLosses.Size = New System.Drawing.Size(310, 111)
        Me.gbRetarderLosses.TabIndex = 1
        Me.gbRetarderLosses.TabStop = false
        Me.gbRetarderLosses.Text = "Retarder Losses"
        '
        'PnRt
        '
        Me.PnRt.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.PnRt.Controls.Add(Me.Label15)
        Me.PnRt.Controls.Add(Me.BtRtBrowse)
        Me.PnRt.Controls.Add(Me.TbRtPath)
        Me.PnRt.Controls.Add(Me.Label45)
        Me.PnRt.Controls.Add(Me.LbRtRatio)
        Me.PnRt.Controls.Add(Me.TbRtRatio)
        Me.PnRt.Location = New System.Drawing.Point(3, 42)
        Me.PnRt.Name = "PnRt"
        Me.PnRt.Size = New System.Drawing.Size(299, 63)
        Me.PnRt.TabIndex = 1
        '
        'Label15
        '
        Me.Label15.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label15.Location = New System.Drawing.Point(2, 23)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(232, 16)
        Me.Label15.TabIndex = 15
        Me.Label15.Text = "Retarder Loss Map"
        Me.Label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'BtRtBrowse
        '
        Me.BtRtBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.BtRtBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtRtBrowse.Location = New System.Drawing.Point(272, 37)
        Me.BtRtBrowse.Name = "BtRtBrowse"
        Me.BtRtBrowse.Size = New System.Drawing.Size(24, 24)
        Me.BtRtBrowse.TabIndex = 2
        Me.BtRtBrowse.UseVisualStyleBackColor = true
        '
        'TbRtPath
        '
        Me.TbRtPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbRtPath.Location = New System.Drawing.Point(3, 39)
        Me.TbRtPath.Name = "TbRtPath"
        Me.TbRtPath.Size = New System.Drawing.Size(269, 20)
        Me.TbRtPath.TabIndex = 1
        '
        'Label45
        '
        Me.Label45.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label45.AutoSize = true
        Me.Label45.Location = New System.Drawing.Point(263, 5)
        Me.Label45.Name = "Label45"
        Me.Label45.Size = New System.Drawing.Size(16, 13)
        Me.Label45.TabIndex = 10
        Me.Label45.Text = "[-]"
        '
        'LbRtRatio
        '
        Me.LbRtRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LbRtRatio.Location = New System.Drawing.Point(35, 5)
        Me.LbRtRatio.Name = "LbRtRatio"
        Me.LbRtRatio.Size = New System.Drawing.Size(167, 17)
        Me.LbRtRatio.TabIndex = 1
        Me.LbRtRatio.Text = "Ratio"
        Me.LbRtRatio.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'TbRtRatio
        '
        Me.TbRtRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TbRtRatio.Location = New System.Drawing.Point(205, 3)
        Me.TbRtRatio.Name = "TbRtRatio"
        Me.TbRtRatio.Size = New System.Drawing.Size(56, 20)
        Me.TbRtRatio.TabIndex = 0
        '
        'CbRtType
        '
        Me.CbRtType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.CbRtType.DisplayMember = "Value"
        Me.CbRtType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbRtType.Items.AddRange(New Object() {"Included in Transmission Loss Maps", "Primary Retarder", "Secondary Retarder"})
        Me.CbRtType.Location = New System.Drawing.Point(6, 19)
        Me.CbRtType.Name = "CbRtType"
        Me.CbRtType.Size = New System.Drawing.Size(297, 21)
        Me.CbRtType.TabIndex = 0
        Me.CbRtType.ValueMember = "Key"
        '
        'Label46
        '
        Me.Label46.AutoSize = true
        Me.Label46.Location = New System.Drawing.Point(32, 5)
        Me.Label46.Name = "Label46"
        Me.Label46.Size = New System.Drawing.Size(145, 13)
        Me.Label46.TabIndex = 31
        Me.Label46.Text = "Curb Mass Extra Trailer/Body"
        '
        'Label50
        '
        Me.Label50.AutoSize = true
        Me.Label50.Location = New System.Drawing.Point(241, 5)
        Me.Label50.Name = "Label50"
        Me.Label50.Size = New System.Drawing.Size(25, 13)
        Me.Label50.TabIndex = 24
        Me.Label50.Text = "[kg]"
        '
        'TbMassExtra
        '
        Me.TbMassExtra.Location = New System.Drawing.Point(182, 2)
        Me.TbMassExtra.Name = "TbMassExtra"
        Me.TbMassExtra.Size = New System.Drawing.Size(57, 20)
        Me.TbMassExtra.TabIndex = 0
        '
        'GroupBox8
        '
        Me.GroupBox8.Controls.Add(Me.Label6)
        Me.GroupBox8.Controls.Add(Me.ButAxlRem)
        Me.GroupBox8.Controls.Add(Me.LvRRC)
        Me.GroupBox8.Controls.Add(Me.ButAxlAdd)
        Me.GroupBox8.Location = New System.Drawing.Point(6, 186)
        Me.GroupBox8.Name = "GroupBox8"
        Me.GroupBox8.Size = New System.Drawing.Size(633, 182)
        Me.GroupBox8.TabIndex = 4
        Me.GroupBox8.TabStop = false
        Me.GroupBox8.Text = "Axles / Wheels"
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = true
        Me.Label6.Location = New System.Drawing.Point(525, 152)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(106, 13)
        Me.Label6.TabIndex = 3
        Me.Label6.Text = "(Double-Click to Edit)"
        '
        'ButAxlRem
        '
        Me.ButAxlRem.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.ButAxlRem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.ButAxlRem.Location = New System.Drawing.Point(29, 151)
        Me.ButAxlRem.Name = "ButAxlRem"
        Me.ButAxlRem.Size = New System.Drawing.Size(24, 24)
        Me.ButAxlRem.TabIndex = 2
        Me.ButAxlRem.UseVisualStyleBackColor = true
        '
        'LvRRC
        '
        Me.LvRRC.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.LvRRC.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader7, Me.ColumnHeader8, Me.ColumnHeader2, Me.ColumnHeader9, Me.ColumnHeader1, Me.ColumnHeader3, Me.ColumnHeader4, Me.ColumnHeader10})
        Me.LvRRC.FullRowSelect = true
        Me.LvRRC.GridLines = true
        Me.LvRRC.HideSelection = false
        Me.LvRRC.Location = New System.Drawing.Point(6, 19)
        Me.LvRRC.MultiSelect = false
        Me.LvRRC.Name = "LvRRC"
        Me.LvRRC.Size = New System.Drawing.Size(621, 131)
        Me.LvRRC.TabIndex = 0
        Me.LvRRC.TabStop = false
        Me.LvRRC.UseCompatibleStateImageBehavior = false
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
        Me.ButAxlAdd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.ButAxlAdd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.ButAxlAdd.Location = New System.Drawing.Point(5, 151)
        Me.ButAxlAdd.Name = "ButAxlAdd"
        Me.ButAxlAdd.Size = New System.Drawing.Size(24, 24)
        Me.ButAxlAdd.TabIndex = 1
        Me.ButAxlAdd.UseVisualStyleBackColor = true
        '
        'PnWheelDiam
        '
        Me.PnWheelDiam.Controls.Add(Me.Label13)
        Me.PnWheelDiam.Controls.Add(Me.TBrdyn)
        Me.PnWheelDiam.Controls.Add(Me.Label35)
        Me.PnWheelDiam.Dock = System.Windows.Forms.DockStyle.Fill
        Me.PnWheelDiam.Location = New System.Drawing.Point(3, 16)
        Me.PnWheelDiam.Name = "PnWheelDiam"
        Me.PnWheelDiam.Size = New System.Drawing.Size(272, 31)
        Me.PnWheelDiam.TabIndex = 0
        '
        'CbAxleConfig
        '
        Me.CbAxleConfig.DisplayMember = "Value"
        Me.CbAxleConfig.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbAxleConfig.FormattingEnabled = true
        Me.CbAxleConfig.Items.AddRange(New Object() {"-", "4x2", "4x4", "6x2", "6x4", "6x6", "8x2", "8x4", "8x6", "8x8"})
        Me.CbAxleConfig.Location = New System.Drawing.Point(153, 80)
        Me.CbAxleConfig.Name = "CbAxleConfig"
        Me.CbAxleConfig.Size = New System.Drawing.Size(60, 21)
        Me.CbAxleConfig.TabIndex = 1
        Me.CbAxleConfig.ValueMember = "Key"
        '
        'CbCat
        '
        Me.CbCat.DisplayMember = "Value"
        Me.CbCat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.CbCat.FormattingEnabled = true
        Me.CbCat.Items.AddRange(New Object() {"-", "Rigid Truck", "Tractor", "City Bus", "Interurban Bus", "Coach"})
        Me.CbCat.Location = New System.Drawing.Point(12, 80)
        Me.CbCat.Name = "CbCat"
        Me.CbCat.Size = New System.Drawing.Size(135, 21)
        Me.CbCat.TabIndex = 0
        Me.CbCat.ValueMember = "Key"
        '
        'Label5
        '
        Me.Label5.AutoSize = true
        Me.Label5.Location = New System.Drawing.Point(31, 108)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(116, 13)
        Me.Label5.TabIndex = 2
        Me.Label5.Text = "Technically Permissible"
        '
        'Label9
        '
        Me.Label9.AutoSize = true
        Me.Label9.Location = New System.Drawing.Point(197, 114)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(16, 13)
        Me.Label9.TabIndex = 3
        Me.Label9.Text = "[t]"
        Me.Label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'TbMassMass
        '
        Me.TbMassMass.Location = New System.Drawing.Point(153, 111)
        Me.TbMassMass.Name = "TbMassMass"
        Me.TbMassMass.Size = New System.Drawing.Size(42, 20)
        Me.TbMassMass.TabIndex = 2
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LbStatus})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 603)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(666, 22)
        Me.StatusStrip1.SizingGrip = false
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
        Me.TbHDVclass.Location = New System.Drawing.Point(153, 141)
        Me.TbHDVclass.Name = "TbHDVclass"
        Me.TbHDVclass.ReadOnly = true
        Me.TbHDVclass.Size = New System.Drawing.Size(42, 20)
        Me.TbHDVclass.TabIndex = 3
        Me.TbHDVclass.TabStop = false
        Me.TbHDVclass.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.PnLoad)
        Me.GroupBox1.Controls.Add(Me.TbMass)
        Me.GroupBox1.Controls.Add(Me.Label1)
        Me.GroupBox1.Controls.Add(Me.Label14)
        Me.GroupBox1.Location = New System.Drawing.Point(6, 6)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(278, 118)
        Me.GroupBox1.TabIndex = 0
        Me.GroupBox1.TabStop = false
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
        Me.PnLoad.Location = New System.Drawing.Point(6, 43)
        Me.PnLoad.Name = "PnLoad"
        Me.PnLoad.Size = New System.Drawing.Size(269, 58)
        Me.PnLoad.TabIndex = 1
        '
        'GrAirRes
        '
        Me.GrAirRes.Controls.Add(Me.PnCdATrTr)
        Me.GrAirRes.Location = New System.Drawing.Point(290, 6)
        Me.GrAirRes.Name = "GrAirRes"
        Me.GrAirRes.Size = New System.Drawing.Size(349, 72)
        Me.GrAirRes.TabIndex = 2
        Me.GrAirRes.TabStop = false
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
        Me.PnCdATrTr.Location = New System.Drawing.Point(3, 16)
        Me.PnCdATrTr.Name = "PnCdATrTr"
        Me.PnCdATrTr.Size = New System.Drawing.Size(343, 53)
        Me.PnCdATrTr.TabIndex = 0
        '
        'tbVehicleHeight
        '
        Me.tbVehicleHeight.Location = New System.Drawing.Point(54, 29)
        Me.tbVehicleHeight.Name = "tbVehicleHeight"
        Me.tbVehicleHeight.Size = New System.Drawing.Size(57, 20)
        Me.tbVehicleHeight.TabIndex = 1
        '
        'Label11
        '
        Me.Label11.AutoSize = true
        Me.Label11.Location = New System.Drawing.Point(117, 32)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(21, 13)
        Me.Label11.TabIndex = 27
        Me.Label11.Text = "[m]"
        '
        'Label20
        '
        Me.Label20.AutoSize = true
        Me.Label20.Location = New System.Drawing.Point(10, 32)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(38, 13)
        Me.Label20.TabIndex = 26
        Me.Label20.Text = "Height"
        '
        'Label38
        '
        Me.Label38.AutoSize = true
        Me.Label38.Location = New System.Drawing.Point(117, 6)
        Me.Label38.Name = "Label38"
        Me.Label38.Size = New System.Drawing.Size(24, 13)
        Me.Label38.TabIndex = 24
        Me.Label38.Text = "[m²]"
        '
        'PictureBox1
        '
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_Mainform
        Me.PictureBox1.Location = New System.Drawing.Point(0, 28)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(666, 40)
        Me.PictureBox1.TabIndex = 37
        Me.PictureBox1.TabStop = false
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.ShowImageMargin = false
        Me.CmOpenFile.Size = New System.Drawing.Size(128, 48)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(127, 22)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.PnWheelDiam)
        Me.GroupBox3.Location = New System.Drawing.Point(6, 130)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(278, 50)
        Me.GroupBox3.TabIndex = 1
        Me.GroupBox3.TabStop = false
        Me.GroupBox3.Text = "Dynamic Tyre Radius"
        '
        'gbAngledrive
        '
        Me.gbAngledrive.Controls.Add(Me.pnAngledriveFields)
        Me.gbAngledrive.Controls.Add(Me.cbAngledriveType)
        Me.gbAngledrive.Location = New System.Drawing.Point(329, 76)
        Me.gbAngledrive.Name = "gbAngledrive"
        Me.gbAngledrive.Size = New System.Drawing.Size(313, 111)
        Me.gbAngledrive.TabIndex = 3
        Me.gbAngledrive.TabStop = false
        Me.gbAngledrive.Text = "Angledrive"
        '
        'pnAngledriveFields
        '
        Me.pnAngledriveFields.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.pnAngledriveFields.Controls.Add(Me.Label4)
        Me.pnAngledriveFields.Controls.Add(Me.Label10)
        Me.pnAngledriveFields.Controls.Add(Me.tbAngledriveRatio)
        Me.pnAngledriveFields.Controls.Add(Me.btAngledriveLossMapBrowse)
        Me.pnAngledriveFields.Controls.Add(Me.tbAngledriveLossMapPath)
        Me.pnAngledriveFields.Controls.Add(Me.Label12)
        Me.pnAngledriveFields.Location = New System.Drawing.Point(3, 42)
        Me.pnAngledriveFields.Name = "pnAngledriveFields"
        Me.pnAngledriveFields.Size = New System.Drawing.Size(303, 63)
        Me.pnAngledriveFields.TabIndex = 1
        '
        'Label4
        '
        Me.Label4.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label4.AutoSize = true
        Me.Label4.Location = New System.Drawing.Point(277, 6)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(16, 13)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "[-]"
        '
        'Label10
        '
        Me.Label10.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label10.Location = New System.Drawing.Point(170, 6)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(44, 18)
        Me.Label10.TabIndex = 15
        Me.Label10.Text = "Ratio"
        Me.Label10.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'tbAngledriveRatio
        '
        Me.tbAngledriveRatio.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbAngledriveRatio.Location = New System.Drawing.Point(219, 4)
        Me.tbAngledriveRatio.Name = "tbAngledriveRatio"
        Me.tbAngledriveRatio.Size = New System.Drawing.Size(56, 20)
        Me.tbAngledriveRatio.TabIndex = 0
        '
        'btAngledriveLossMapBrowse
        '
        Me.btAngledriveLossMapBrowse.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btAngledriveLossMapBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btAngledriveLossMapBrowse.Location = New System.Drawing.Point(276, 39)
        Me.btAngledriveLossMapBrowse.Name = "btAngledriveLossMapBrowse"
        Me.btAngledriveLossMapBrowse.Size = New System.Drawing.Size(24, 24)
        Me.btAngledriveLossMapBrowse.TabIndex = 2
        Me.btAngledriveLossMapBrowse.UseVisualStyleBackColor = true
        '
        'tbAngledriveLossMapPath
        '
        Me.tbAngledriveLossMapPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbAngledriveLossMapPath.Location = New System.Drawing.Point(3, 41)
        Me.tbAngledriveLossMapPath.Name = "tbAngledriveLossMapPath"
        Me.tbAngledriveLossMapPath.Size = New System.Drawing.Size(273, 20)
        Me.tbAngledriveLossMapPath.TabIndex = 1
        '
        'Label12
        '
        Me.Label12.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label12.Location = New System.Drawing.Point(0, 24)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(294, 16)
        Me.Label12.TabIndex = 17
        Me.Label12.Text = "Transmission Loss Map or Efficiency Value [0..1]"
        Me.Label12.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'cbAngledriveType
        '
        Me.cbAngledriveType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.cbAngledriveType.DisplayMember = "Value"
        Me.cbAngledriveType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbAngledriveType.Location = New System.Drawing.Point(6, 19)
        Me.cbAngledriveType.Name = "cbAngledriveType"
        Me.cbAngledriveType.Size = New System.Drawing.Size(297, 21)
        Me.cbAngledriveType.TabIndex = 0
        Me.cbAngledriveType.ValueMember = "Key"
        '
        'PicVehicle
        '
        Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
        Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVehicle.Location = New System.Drawing.Point(281, 70)
        Me.PicVehicle.Name = "PicVehicle"
        Me.PicVehicle.Size = New System.Drawing.Size(300, 88)
        Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicVehicle.TabIndex = 39
        Me.PicVehicle.TabStop = false
        '
        'Label8
        '
        Me.Label8.AutoSize = true
        Me.Label8.Location = New System.Drawing.Point(85, 144)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(62, 13)
        Me.Label8.TabIndex = 10
        Me.Label8.Text = "HDV Group"
        '
        'cbPTOType
        '
        Me.cbPTOType.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.cbPTOType.DisplayMember = "Value"
        Me.cbPTOType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPTOType.Location = New System.Drawing.Point(6, 24)
        Me.cbPTOType.Name = "cbPTOType"
        Me.cbPTOType.Size = New System.Drawing.Size(619, 21)
        Me.cbPTOType.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.cbPTOType, "Transmission type to the PTO consumer")
        Me.cbPTOType.ValueMember = "Key"
        '
        'tbPTOCycle
        '
        Me.tbPTOCycle.Location = New System.Drawing.Point(6, 71)
        Me.tbPTOCycle.Name = "tbPTOCycle"
        Me.tbPTOCycle.Size = New System.Drawing.Size(430, 20)
        Me.tbPTOCycle.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.tbPTOCycle, "PTO Consumer Loss Map")
        '
        'tbPTOLossMap
        '
        Me.tbPTOLossMap.Location = New System.Drawing.Point(6, 24)
        Me.tbPTOLossMap.Name = "tbPTOLossMap"
        Me.tbPTOLossMap.Size = New System.Drawing.Size(430, 20)
        Me.tbPTOLossMap.TabIndex = 0
        Me.ToolTip1.SetToolTip(Me.tbPTOLossMap, "PTO Consumer Loss Map")
        '
        'tbPTODrive
        '
        Me.tbPTODrive.Location = New System.Drawing.Point(6, 183)
        Me.tbPTODrive.Name = "tbPTODrive"
        Me.tbPTODrive.Size = New System.Drawing.Size(430, 20)
        Me.tbPTODrive.TabIndex = 5
        Me.ToolTip1.SetToolTip(Me.tbPTODrive, "PTO Consumer Loss Map")
        '
        'tcVehicleComponents
        '
        Me.tcVehicleComponents.Controls.Add(Me.tpGeneral)
        Me.tcVehicleComponents.Controls.Add(Me.tpPowertrain)
        Me.tcVehicleComponents.Controls.Add(Me.tpElectricMachine)
        Me.tcVehicleComponents.Controls.Add(Me.tpIEPC)
        Me.tcVehicleComponents.Controls.Add(Me.tbIHPC)
        Me.tcVehicleComponents.Controls.Add(Me.tpReess)
        Me.tcVehicleComponents.Controls.Add(Me.tpGensetComponents)
        Me.tcVehicleComponents.Controls.Add(Me.tpTorqueLimits)
        Me.tcVehicleComponents.Controls.Add(Me.tpADAS)
        Me.tcVehicleComponents.Controls.Add(Me.tpRoadSweeper)
        Me.tcVehicleComponents.Location = New System.Drawing.Point(5, 173)
        Me.tcVehicleComponents.Name = "tcVehicleComponents"
        Me.tcVehicleComponents.SelectedIndex = 0
        Me.tcVehicleComponents.Size = New System.Drawing.Size(656, 400)
        Me.tcVehicleComponents.TabIndex = 5
        '
        'tpGeneral
        '
        Me.tpGeneral.Controls.Add(Me.GroupBox1)
        Me.tpGeneral.Controls.Add(Me.GroupBox3)
        Me.tpGeneral.Controls.Add(Me.GroupBox6)
        Me.tpGeneral.Controls.Add(Me.GroupBox8)
        Me.tpGeneral.Controls.Add(Me.GrAirRes)
        Me.tpGeneral.Location = New System.Drawing.Point(4, 22)
        Me.tpGeneral.Name = "tpGeneral"
        Me.tpGeneral.Padding = New System.Windows.Forms.Padding(3)
        Me.tpGeneral.Size = New System.Drawing.Size(648, 374)
        Me.tpGeneral.TabIndex = 0
        Me.tpGeneral.Text = "General"
        Me.tpGeneral.UseVisualStyleBackColor = true
        '
        'tpPowertrain
        '
        Me.tpPowertrain.Controls.Add(Me.gbVehicleIdlingSpeed)
        Me.tpPowertrain.Controls.Add(Me.gbTankSystem)
        Me.tpPowertrain.Controls.Add(Me.gbRetarderLosses)
        Me.tpPowertrain.Controls.Add(Me.gbAngledrive)
        Me.tpPowertrain.Location = New System.Drawing.Point(4, 22)
        Me.tpPowertrain.Name = "tpPowertrain"
        Me.tpPowertrain.Padding = New System.Windows.Forms.Padding(3)
        Me.tpPowertrain.Size = New System.Drawing.Size(648, 374)
        Me.tpPowertrain.TabIndex = 1
        Me.tpPowertrain.Text = "Powertrain"
        Me.tpPowertrain.UseVisualStyleBackColor = true
        '
        'gbVehicleIdlingSpeed
        '
        Me.gbVehicleIdlingSpeed.Controls.Add(Me.Panel1)
        Me.gbVehicleIdlingSpeed.Location = New System.Drawing.Point(6, 6)
        Me.gbVehicleIdlingSpeed.Name = "gbVehicleIdlingSpeed"
        Me.gbVehicleIdlingSpeed.Size = New System.Drawing.Size(310, 63)
        Me.gbVehicleIdlingSpeed.TabIndex = 0
        Me.gbVehicleIdlingSpeed.TabStop = false
        Me.gbVehicleIdlingSpeed.Text = "Vehicle Idling Speed"
        '
        'Panel1
        '
        Me.Panel1.Controls.Add(Me.tbVehIdlingSpeed)
        Me.Panel1.Controls.Add(Me.Label18)
        Me.Panel1.Controls.Add(Me.Label19)
        Me.Panel1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.Panel1.Location = New System.Drawing.Point(3, 16)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(304, 44)
        Me.Panel1.TabIndex = 0
        '
        'tbVehIdlingSpeed
        '
        Me.tbVehIdlingSpeed.Location = New System.Drawing.Point(205, 4)
        Me.tbVehIdlingSpeed.Name = "tbVehIdlingSpeed"
        Me.tbVehIdlingSpeed.Size = New System.Drawing.Size(56, 20)
        Me.tbVehIdlingSpeed.TabIndex = 0
        '
        'Label18
        '
        Me.Label18.AutoSize = true
        Me.Label18.Location = New System.Drawing.Point(264, 7)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(30, 13)
        Me.Label18.TabIndex = 24
        Me.Label18.Text = "[rpm]"
        '
        'Label19
        '
        Me.Label19.AutoSize = true
        Me.Label19.Location = New System.Drawing.Point(104, 7)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(94, 13)
        Me.Label19.TabIndex = 8
        Me.Label19.Text = "Engine Idle Speed"
        '
        'gbTankSystem
        '
        Me.gbTankSystem.Controls.Add(Me.cbTankSystem)
        Me.gbTankSystem.Controls.Add(Me.Label23)
        Me.gbTankSystem.Location = New System.Drawing.Point(329, 6)
        Me.gbTankSystem.Name = "gbTankSystem"
        Me.gbTankSystem.Size = New System.Drawing.Size(310, 63)
        Me.gbTankSystem.TabIndex = 2
        Me.gbTankSystem.TabStop = false
        Me.gbTankSystem.Text = "Tank System"
        '
        'cbTankSystem
        '
        Me.cbTankSystem.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.cbTankSystem.DisplayMember = "Value"
        Me.cbTankSystem.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbTankSystem.Location = New System.Drawing.Point(6, 19)
        Me.cbTankSystem.Name = "cbTankSystem"
        Me.cbTankSystem.Size = New System.Drawing.Size(297, 21)
        Me.cbTankSystem.TabIndex = 1
        Me.cbTankSystem.ValueMember = "Key"
        '
        'Label23
        '
        Me.Label23.AutoSize = true
        Me.Label23.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label23.Location = New System.Drawing.Point(3, 43)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(187, 13)
        Me.Label23.TabIndex = 0
        Me.Label23.Text = "Only applicable for NG engines!"
        '
        'tpElectricMachine
        '
        Me.tpElectricMachine.Controls.Add(Me.gpElectricMotor)
        Me.tpElectricMachine.Location = New System.Drawing.Point(4, 22)
        Me.tpElectricMachine.Name = "tpElectricMachine"
        Me.tpElectricMachine.Padding = New System.Windows.Forms.Padding(3)
        Me.tpElectricMachine.Size = New System.Drawing.Size(648, 374)
        Me.tpElectricMachine.TabIndex = 6
        Me.tpElectricMachine.Text = "Electric Machine"
        Me.tpElectricMachine.UseVisualStyleBackColor = true
        '
        'gpElectricMotor
        '
        Me.gpElectricMotor.Controls.Add(Me.Label33)
        Me.gpElectricMotor.Controls.Add(Me.Label32)
        Me.gpElectricMotor.Controls.Add(Me.gbRatiosPerGear)
        Me.gpElectricMotor.Controls.Add(Me.btnEmADCLossMap)
        Me.gpElectricMotor.Controls.Add(Me.tbEmADCLossMap)
        Me.gpElectricMotor.Controls.Add(Me.lblEmADCLossmap)
        Me.gpElectricMotor.Controls.Add(Me.tbRatioEm)
        Me.gpElectricMotor.Controls.Add(Me.lblRatioEm)
        Me.gpElectricMotor.Controls.Add(Me.tbEmCount)
        Me.gpElectricMotor.Controls.Add(Me.cbEmPos)
        Me.gpElectricMotor.Controls.Add(Me.lblEmCount)
        Me.gpElectricMotor.Controls.Add(Me.lblEmPosition)
        Me.gpElectricMotor.Controls.Add(Me.pnElectricMotor)
        Me.gpElectricMotor.Location = New System.Drawing.Point(6, 6)
        Me.gpElectricMotor.Name = "gpElectricMotor"
        Me.gpElectricMotor.Size = New System.Drawing.Size(633, 163)
        Me.gpElectricMotor.TabIndex = 1
        Me.gpElectricMotor.TabStop = false
        Me.gpElectricMotor.Text = "Electric Machine"
        '
        'Label33
        '
        Me.Label33.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label33.AutoSize = true
        Me.Label33.Location = New System.Drawing.Point(203, 108)
        Me.Label33.Name = "Label33"
        Me.Label33.Size = New System.Drawing.Size(16, 13)
        Me.Label33.TabIndex = 27
        Me.Label33.Text = "[-]"
        '
        'Label32
        '
        Me.Label32.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label32.AutoSize = true
        Me.Label32.Location = New System.Drawing.Point(203, 82)
        Me.Label32.Name = "Label32"
        Me.Label32.Size = New System.Drawing.Size(16, 13)
        Me.Label32.TabIndex = 26
        Me.Label32.Text = "[-]"
        '
        'gbRatiosPerGear
        '
        Me.gbRatiosPerGear.Controls.Add(Me.lvRatioPerGear)
        Me.gbRatiosPerGear.Controls.Add(Me.btnAddEMRatio)
        Me.gbRatiosPerGear.Controls.Add(Me.Label29)
        Me.gbRatiosPerGear.Controls.Add(Me.btnRemoveEMRatio)
        Me.gbRatiosPerGear.Location = New System.Drawing.Point(444, 12)
        Me.gbRatiosPerGear.Name = "gbRatiosPerGear"
        Me.gbRatiosPerGear.Size = New System.Drawing.Size(181, 145)
        Me.gbRatiosPerGear.TabIndex = 1
        Me.gbRatiosPerGear.TabStop = false
        Me.gbRatiosPerGear.Text = "Transmission Ratio per Gear"
        '
        'lvRatioPerGear
        '
        Me.lvRatioPerGear.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lvRatioPerGear.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader11, Me.ColumnHeader12})
        Me.lvRatioPerGear.FullRowSelect = true
        Me.lvRatioPerGear.GridLines = true
        Me.lvRatioPerGear.HideSelection = false
        Me.lvRatioPerGear.Location = New System.Drawing.Point(6, 16)
        Me.lvRatioPerGear.MultiSelect = false
        Me.lvRatioPerGear.Name = "lvRatioPerGear"
        Me.lvRatioPerGear.Size = New System.Drawing.Size(169, 94)
        Me.lvRatioPerGear.TabIndex = 7
        Me.lvRatioPerGear.TabStop = false
        Me.lvRatioPerGear.UseCompatibleStateImageBehavior = false
        Me.lvRatioPerGear.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader11
        '
        Me.ColumnHeader11.Text = "Gear #"
        Me.ColumnHeader11.Width = 59
        '
        'ColumnHeader12
        '
        Me.ColumnHeader12.Text = "Ratio"
        Me.ColumnHeader12.Width = 172
        '
        'btnAddEMRatio
        '
        Me.btnAddEMRatio.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.btnAddEMRatio.Location = New System.Drawing.Point(6, 116)
        Me.btnAddEMRatio.Name = "btnAddEMRatio"
        Me.btnAddEMRatio.Size = New System.Drawing.Size(24, 24)
        Me.btnAddEMRatio.TabIndex = 4
        Me.btnAddEMRatio.UseVisualStyleBackColor = true
        '
        'Label29
        '
        Me.Label29.AutoSize = true
        Me.Label29.Location = New System.Drawing.Point(71, 111)
        Me.Label29.Name = "Label29"
        Me.Label29.Size = New System.Drawing.Size(106, 13)
        Me.Label29.TabIndex = 6
        Me.Label29.Text = "(Double-Click to Edit)"
        '
        'btnRemoveEMRatio
        '
        Me.btnRemoveEMRatio.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.btnRemoveEMRatio.Location = New System.Drawing.Point(33, 116)
        Me.btnRemoveEMRatio.Name = "btnRemoveEMRatio"
        Me.btnRemoveEMRatio.Size = New System.Drawing.Size(24, 24)
        Me.btnRemoveEMRatio.TabIndex = 5
        Me.btnRemoveEMRatio.UseVisualStyleBackColor = true
        '
        'btnEmADCLossMap
        '
        Me.btnEmADCLossMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnEmADCLossMap.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnEmADCLossMap.Location = New System.Drawing.Point(346, 129)
        Me.btnEmADCLossMap.Name = "btnEmADCLossMap"
        Me.btnEmADCLossMap.Size = New System.Drawing.Size(24, 24)
        Me.btnEmADCLossMap.TabIndex = 5
        Me.btnEmADCLossMap.UseVisualStyleBackColor = true
        '
        'tbEmADCLossMap
        '
        Me.tbEmADCLossMap.Location = New System.Drawing.Point(110, 131)
        Me.tbEmADCLossMap.Name = "tbEmADCLossMap"
        Me.tbEmADCLossMap.Size = New System.Drawing.Size(234, 20)
        Me.tbEmADCLossMap.TabIndex = 4
        '
        'lblEmADCLossmap
        '
        Me.lblEmADCLossmap.AutoSize = true
        Me.lblEmADCLossmap.Location = New System.Drawing.Point(7, 134)
        Me.lblEmADCLossmap.Name = "lblEmADCLossmap"
        Me.lblEmADCLossmap.Size = New System.Drawing.Size(100, 13)
        Me.lblEmADCLossmap.TabIndex = 25
        Me.lblEmADCLossmap.Text = "Loss Map EM ADC:"
        '
        'tbRatioEm
        '
        Me.tbRatioEm.Location = New System.Drawing.Point(110, 105)
        Me.tbRatioEm.Name = "tbRatioEm"
        Me.tbRatioEm.Size = New System.Drawing.Size(91, 20)
        Me.tbRatioEm.TabIndex = 3
        '
        'lblRatioEm
        '
        Me.lblRatioEm.AutoSize = true
        Me.lblRatioEm.Location = New System.Drawing.Point(7, 108)
        Me.lblRatioEm.Name = "lblRatioEm"
        Me.lblRatioEm.Size = New System.Drawing.Size(79, 13)
        Me.lblRatioEm.TabIndex = 23
        Me.lblRatioEm.Text = "Ratio EM ADC:"
        '
        'tbEmCount
        '
        Me.tbEmCount.Location = New System.Drawing.Point(110, 79)
        Me.tbEmCount.Name = "tbEmCount"
        Me.tbEmCount.Size = New System.Drawing.Size(91, 20)
        Me.tbEmCount.TabIndex = 2
        '
        'cbEmPos
        '
        Me.cbEmPos.DisplayMember = "Value"
        Me.cbEmPos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbEmPos.FormattingEnabled = true
        Me.cbEmPos.Location = New System.Drawing.Point(110, 52)
        Me.cbEmPos.Name = "cbEmPos"
        Me.cbEmPos.Size = New System.Drawing.Size(153, 21)
        Me.cbEmPos.TabIndex = 1
        Me.cbEmPos.ValueMember = "Key"
        '
        'lblEmCount
        '
        Me.lblEmCount.AutoSize = true
        Me.lblEmCount.Location = New System.Drawing.Point(7, 82)
        Me.lblEmCount.Name = "lblEmCount"
        Me.lblEmCount.Size = New System.Drawing.Size(83, 13)
        Me.lblEmCount.TabIndex = 20
        Me.lblEmCount.Text = "Number of EMs:"
        '
        'lblEmPosition
        '
        Me.lblEmPosition.AutoSize = true
        Me.lblEmPosition.Location = New System.Drawing.Point(7, 56)
        Me.lblEmPosition.Name = "lblEmPosition"
        Me.lblEmPosition.Size = New System.Drawing.Size(47, 13)
        Me.lblEmPosition.TabIndex = 19
        Me.lblEmPosition.Text = "Position:"
        '
        'pnElectricMotor
        '
        Me.pnElectricMotor.Controls.Add(Me.btnOpenElectricMotor)
        Me.pnElectricMotor.Controls.Add(Me.btnBrowseElectricMotor)
        Me.pnElectricMotor.Controls.Add(Me.tbElectricMotor)
        Me.pnElectricMotor.Location = New System.Drawing.Point(6, 19)
        Me.pnElectricMotor.Name = "pnElectricMotor"
        Me.pnElectricMotor.Size = New System.Drawing.Size(432, 27)
        Me.pnElectricMotor.TabIndex = 0
        '
        'btnOpenElectricMotor
        '
        Me.btnOpenElectricMotor.Location = New System.Drawing.Point(4, 3)
        Me.btnOpenElectricMotor.Name = "btnOpenElectricMotor"
        Me.btnOpenElectricMotor.Size = New System.Drawing.Size(94, 21)
        Me.btnOpenElectricMotor.TabIndex = 0
        Me.btnOpenElectricMotor.Text = "Electric Machine"
        Me.btnOpenElectricMotor.UseVisualStyleBackColor = true
        '
        'btnBrowseElectricMotor
        '
        Me.btnBrowseElectricMotor.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnBrowseElectricMotor.Image = CType(resources.GetObject("btnBrowseElectricMotor.Image"),System.Drawing.Image)
        Me.btnBrowseElectricMotor.Location = New System.Drawing.Point(406, 2)
        Me.btnBrowseElectricMotor.Name = "btnBrowseElectricMotor"
        Me.btnBrowseElectricMotor.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseElectricMotor.TabIndex = 2
        Me.btnBrowseElectricMotor.UseVisualStyleBackColor = true
        '
        'tbElectricMotor
        '
        Me.tbElectricMotor.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbElectricMotor.Location = New System.Drawing.Point(104, 4)
        Me.tbElectricMotor.Name = "tbElectricMotor"
        Me.tbElectricMotor.Size = New System.Drawing.Size(300, 20)
        Me.tbElectricMotor.TabIndex = 1
        '
        'tpIEPC
        '
        Me.tpIEPC.Controls.Add(Me.FlowLayoutPanel1)
        Me.tpIEPC.Location = New System.Drawing.Point(4, 22)
        Me.tpIEPC.Name = "tpIEPC"
        Me.tpIEPC.Size = New System.Drawing.Size(648, 374)
        Me.tpIEPC.TabIndex = 8
        Me.tpIEPC.Text = "IEPC"
        Me.tpIEPC.UseVisualStyleBackColor = true
        '
        'btIEPCFilePath
        '
        Me.btIEPCFilePath.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btIEPCFilePath.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btIEPCFilePath.Location = New System.Drawing.Point(399, 1)
        Me.btIEPCFilePath.Margin = New System.Windows.Forms.Padding(1)
        Me.btIEPCFilePath.Name = "btIEPCFilePath"
        Me.btIEPCFilePath.Size = New System.Drawing.Size(24, 24)
        Me.btIEPCFilePath.TabIndex = 58
        Me.btIEPCFilePath.UseVisualStyleBackColor = true
        '
        'tbIEPCFilePath
        '
        Me.tbIEPCFilePath.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbIEPCFilePath.Location = New System.Drawing.Point(97, 3)
        Me.tbIEPCFilePath.Margin = New System.Windows.Forms.Padding(1)
        Me.tbIEPCFilePath.Name = "tbIEPCFilePath"
        Me.tbIEPCFilePath.Size = New System.Drawing.Size(300, 20)
        Me.tbIEPCFilePath.TabIndex = 28
        '
        'btnIEPC
        '
        Me.btnIEPC.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btnIEPC.Location = New System.Drawing.Point(1, 2)
        Me.btnIEPC.Margin = New System.Windows.Forms.Padding(1)
        Me.btnIEPC.Name = "btnIEPC"
        Me.btnIEPC.Size = New System.Drawing.Size(94, 21)
        Me.btnIEPC.TabIndex = 27
        Me.btnIEPC.Text = "IEPC"
        Me.btnIEPC.UseVisualStyleBackColor = true
        '
        'tbIHPC
        '
        Me.tbIHPC.Controls.Add(Me.FlowLayoutPanel2)
        Me.tbIHPC.Location = New System.Drawing.Point(4, 22)
        Me.tbIHPC.Name = "tbIHPC"
        Me.tbIHPC.Padding = New System.Windows.Forms.Padding(3)
        Me.tbIHPC.Size = New System.Drawing.Size(648, 374)
        Me.tbIHPC.TabIndex = 9
        Me.tbIHPC.Text = "IHPC"
        Me.tbIHPC.UseVisualStyleBackColor = true
        '
        'btIHPC
        '
        Me.btIHPC.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btIHPC.Location = New System.Drawing.Point(1, 2)
        Me.btIHPC.Margin = New System.Windows.Forms.Padding(1)
        Me.btIHPC.Name = "btIHPC"
        Me.btIHPC.Size = New System.Drawing.Size(94, 21)
        Me.btIHPC.TabIndex = 0
        Me.btIHPC.Text = "IHPC"
        Me.btIHPC.UseVisualStyleBackColor = true
        '
        'btIHPCFile
        '
        Me.btIHPCFile.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.btIHPCFile.Image = CType(resources.GetObject("btIHPCFile.Image"),System.Drawing.Image)
        Me.btIHPCFile.Location = New System.Drawing.Point(399, 1)
        Me.btIHPCFile.Margin = New System.Windows.Forms.Padding(1)
        Me.btIHPCFile.Name = "btIHPCFile"
        Me.btIHPCFile.Size = New System.Drawing.Size(24, 24)
        Me.btIHPCFile.TabIndex = 2
        Me.btIHPCFile.UseVisualStyleBackColor = true
        '
        'tbIHPCFilePath
        '
        Me.tbIHPCFilePath.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.tbIHPCFilePath.Location = New System.Drawing.Point(97, 3)
        Me.tbIHPCFilePath.Margin = New System.Windows.Forms.Padding(1)
        Me.tbIHPCFilePath.Name = "tbIHPCFilePath"
        Me.tbIHPCFilePath.Size = New System.Drawing.Size(300, 20)
        Me.tbIHPCFilePath.TabIndex = 1
        '
        'tpReess
        '
        Me.tpReess.Controls.Add(Me.gbBattery)
        Me.tpReess.Location = New System.Drawing.Point(4, 22)
        Me.tpReess.Name = "tpReess"
        Me.tpReess.Padding = New System.Windows.Forms.Padding(3)
        Me.tpReess.Size = New System.Drawing.Size(648, 374)
        Me.tpReess.TabIndex = 7
        Me.tpReess.Text = "REESS"
        Me.tpReess.UseVisualStyleBackColor = true
        '
        'gbBattery
        '
        Me.gbBattery.Controls.Add(Me.lvREESSPacks)
        Me.gbBattery.Controls.Add(Me.btnAddReessPack)
        Me.gbBattery.Controls.Add(Me.lblEditReessPack)
        Me.gbBattery.Controls.Add(Me.btnRemoveReessPack)
        Me.gbBattery.Controls.Add(Me.lblInitialSoCUnit)
        Me.gbBattery.Controls.Add(Me.tbInitialSoC)
        Me.gbBattery.Controls.Add(Me.lblInitialSoC)
        Me.gbBattery.Location = New System.Drawing.Point(6, 6)
        Me.gbBattery.Name = "gbBattery"
        Me.gbBattery.Size = New System.Drawing.Size(633, 187)
        Me.gbBattery.TabIndex = 3
        Me.gbBattery.TabStop = false
        Me.gbBattery.Text = "Electric Energy Storage system"
        '
        'lvREESSPacks
        '
        Me.lvREESSPacks.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lvREESSPacks.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.chReessPackPack, Me.chReessPackCount, Me.chReessPackStringId})
        Me.lvREESSPacks.FullRowSelect = true
        Me.lvREESSPacks.GridLines = true
        Me.lvREESSPacks.HideSelection = false
        Me.lvREESSPacks.Location = New System.Drawing.Point(6, 45)
        Me.lvREESSPacks.MultiSelect = false
        Me.lvREESSPacks.Name = "lvREESSPacks"
        Me.lvREESSPacks.Size = New System.Drawing.Size(553, 102)
        Me.lvREESSPacks.TabIndex = 31
        Me.lvREESSPacks.TabStop = false
        Me.lvREESSPacks.UseCompatibleStateImageBehavior = false
        Me.lvREESSPacks.View = System.Windows.Forms.View.Details
        '
        'chReessPackPack
        '
        Me.chReessPackPack.Text = "REESS Pack"
        Me.chReessPackPack.Width = 350
        '
        'chReessPackCount
        '
        Me.chReessPackCount.Text = "Count"
        Me.chReessPackCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'chReessPackStringId
        '
        Me.chReessPackStringId.Text = "Stream #"
        Me.chReessPackStringId.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'btnAddReessPack
        '
        Me.btnAddReessPack.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.btnAddReessPack.Location = New System.Drawing.Point(6, 153)
        Me.btnAddReessPack.Name = "btnAddReessPack"
        Me.btnAddReessPack.Size = New System.Drawing.Size(24, 24)
        Me.btnAddReessPack.TabIndex = 1
        Me.btnAddReessPack.UseVisualStyleBackColor = true
        '
        'lblEditReessPack
        '
        Me.lblEditReessPack.AutoSize = true
        Me.lblEditReessPack.Location = New System.Drawing.Point(452, 148)
        Me.lblEditReessPack.Name = "lblEditReessPack"
        Me.lblEditReessPack.Size = New System.Drawing.Size(106, 13)
        Me.lblEditReessPack.TabIndex = 30
        Me.lblEditReessPack.Text = "(Double-Click to Edit)"
        '
        'btnRemoveReessPack
        '
        Me.btnRemoveReessPack.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.btnRemoveReessPack.Location = New System.Drawing.Point(33, 153)
        Me.btnRemoveReessPack.Name = "btnRemoveReessPack"
        Me.btnRemoveReessPack.Size = New System.Drawing.Size(24, 24)
        Me.btnRemoveReessPack.TabIndex = 2
        Me.btnRemoveReessPack.UseVisualStyleBackColor = true
        '
        'lblInitialSoCUnit
        '
        Me.lblInitialSoCUnit.AutoSize = true
        Me.lblInitialSoCUnit.Location = New System.Drawing.Point(265, 21)
        Me.lblInitialSoCUnit.Name = "lblInitialSoCUnit"
        Me.lblInitialSoCUnit.Size = New System.Drawing.Size(21, 13)
        Me.lblInitialSoCUnit.TabIndex = 27
        Me.lblInitialSoCUnit.Text = "[%]"
        '
        'tbInitialSoC
        '
        Me.tbInitialSoC.Location = New System.Drawing.Point(204, 19)
        Me.tbInitialSoC.Name = "tbInitialSoC"
        Me.tbInitialSoC.Size = New System.Drawing.Size(59, 20)
        Me.tbInitialSoC.TabIndex = 0
        '
        'lblInitialSoC
        '
        Me.lblInitialSoC.AutoSize = true
        Me.lblInitialSoC.Location = New System.Drawing.Point(7, 22)
        Me.lblInitialSoC.Name = "lblInitialSoC"
        Me.lblInitialSoC.RightToLeft = System.Windows.Forms.RightToLeft.Yes
        Me.lblInitialSoC.Size = New System.Drawing.Size(54, 13)
        Me.lblInitialSoC.TabIndex = 25
        Me.lblInitialSoC.Text = "Initial SoC"
        '
        'tpGensetComponents
        '
        Me.tpGensetComponents.Controls.Add(Me.Label30)
        Me.tpGensetComponents.Controls.Add(Me.gbGenSet)
        Me.tpGensetComponents.Location = New System.Drawing.Point(4, 22)
        Me.tpGensetComponents.Margin = New System.Windows.Forms.Padding(2)
        Me.tpGensetComponents.Name = "tpGensetComponents"
        Me.tpGensetComponents.Size = New System.Drawing.Size(648, 374)
        Me.tpGensetComponents.TabIndex = 5
        Me.tpGensetComponents.Text = "GenSet Components"
        Me.tpGensetComponents.UseVisualStyleBackColor = true
        '
        'Label30
        '
        Me.Label30.AutoSize = true
        Me.Label30.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Label30.Location = New System.Drawing.Point(13, 153)
        Me.Label30.Name = "Label30"
        Me.Label30.Size = New System.Drawing.Size(429, 13)
        Me.Label30.TabIndex = 21
        Me.Label30.Text = "Note: The internal combustion engine of the GenSet is configured in the Job-Edito"& _ 
    "r dialog."
        '
        'gbGenSet
        '
        Me.gbGenSet.Controls.Add(Me.Label36)
        Me.gbGenSet.Controls.Add(Me.Label34)
        Me.gbGenSet.Controls.Add(Me.btnGenSetLossMap)
        Me.gbGenSet.Controls.Add(Me.tbGenSetADC)
        Me.gbGenSet.Controls.Add(Me.lblGenSetADC)
        Me.gbGenSet.Controls.Add(Me.tbGenSetRatio)
        Me.gbGenSet.Controls.Add(Me.lblGenSetRatio)
        Me.gbGenSet.Controls.Add(Me.tbGenSetCount)
        Me.gbGenSet.Controls.Add(Me.lblGenSetCount)
        Me.gbGenSet.Controls.Add(Me.pnGenSetEM)
        Me.gbGenSet.Location = New System.Drawing.Point(6, 6)
        Me.gbGenSet.Name = "gbGenSet"
        Me.gbGenSet.Size = New System.Drawing.Size(633, 135)
        Me.gbGenSet.TabIndex = 1
        Me.gbGenSet.TabStop = false
        Me.gbGenSet.Text = "Electric Machine"
        '
        'Label36
        '
        Me.Label36.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label36.AutoSize = true
        Me.Label36.Location = New System.Drawing.Point(203, 83)
        Me.Label36.Name = "Label36"
        Me.Label36.Size = New System.Drawing.Size(16, 13)
        Me.Label36.TabIndex = 28
        Me.Label36.Text = "[-]"
        '
        'Label34
        '
        Me.Label34.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label34.AutoSize = true
        Me.Label34.Location = New System.Drawing.Point(203, 57)
        Me.Label34.Name = "Label34"
        Me.Label34.Size = New System.Drawing.Size(16, 13)
        Me.Label34.TabIndex = 27
        Me.Label34.Text = "[-]"
        '
        'btnGenSetLossMap
        '
        Me.btnGenSetLossMap.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btnGenSetLossMap.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnGenSetLossMap.Location = New System.Drawing.Point(412, 105)
        Me.btnGenSetLossMap.Name = "btnGenSetLossMap"
        Me.btnGenSetLossMap.Size = New System.Drawing.Size(24, 24)
        Me.btnGenSetLossMap.TabIndex = 5
        Me.btnGenSetLossMap.UseVisualStyleBackColor = true
        '
        'tbGenSetADC
        '
        Me.tbGenSetADC.Location = New System.Drawing.Point(110, 107)
        Me.tbGenSetADC.Name = "tbGenSetADC"
        Me.tbGenSetADC.Size = New System.Drawing.Size(301, 20)
        Me.tbGenSetADC.TabIndex = 4
        '
        'lblGenSetADC
        '
        Me.lblGenSetADC.AutoSize = true
        Me.lblGenSetADC.Location = New System.Drawing.Point(7, 110)
        Me.lblGenSetADC.Name = "lblGenSetADC"
        Me.lblGenSetADC.Size = New System.Drawing.Size(100, 13)
        Me.lblGenSetADC.TabIndex = 25
        Me.lblGenSetADC.Text = "Loss Map EM ADC:"
        '
        'tbGenSetRatio
        '
        Me.tbGenSetRatio.Location = New System.Drawing.Point(110, 81)
        Me.tbGenSetRatio.Name = "tbGenSetRatio"
        Me.tbGenSetRatio.Size = New System.Drawing.Size(91, 20)
        Me.tbGenSetRatio.TabIndex = 3
        '
        'lblGenSetRatio
        '
        Me.lblGenSetRatio.AutoSize = true
        Me.lblGenSetRatio.Location = New System.Drawing.Point(7, 84)
        Me.lblGenSetRatio.Name = "lblGenSetRatio"
        Me.lblGenSetRatio.Size = New System.Drawing.Size(79, 13)
        Me.lblGenSetRatio.TabIndex = 23
        Me.lblGenSetRatio.Text = "Ratio EM ADC:"
        '
        'tbGenSetCount
        '
        Me.tbGenSetCount.Location = New System.Drawing.Point(110, 55)
        Me.tbGenSetCount.Name = "tbGenSetCount"
        Me.tbGenSetCount.Size = New System.Drawing.Size(91, 20)
        Me.tbGenSetCount.TabIndex = 2
        '
        'lblGenSetCount
        '
        Me.lblGenSetCount.AutoSize = true
        Me.lblGenSetCount.Location = New System.Drawing.Point(7, 58)
        Me.lblGenSetCount.Name = "lblGenSetCount"
        Me.lblGenSetCount.Size = New System.Drawing.Size(83, 13)
        Me.lblGenSetCount.TabIndex = 20
        Me.lblGenSetCount.Text = "Number of EMs:"
        '
        'pnGenSetEM
        '
        Me.pnGenSetEM.Controls.Add(Me.btnOpenGenSetEM)
        Me.pnGenSetEM.Controls.Add(Me.btnBrowseGenSetEM)
        Me.pnGenSetEM.Controls.Add(Me.tbGenSetEM)
        Me.pnGenSetEM.Location = New System.Drawing.Point(6, 19)
        Me.pnGenSetEM.Name = "pnGenSetEM"
        Me.pnGenSetEM.Size = New System.Drawing.Size(432, 27)
        Me.pnGenSetEM.TabIndex = 0
        '
        'btnOpenGenSetEM
        '
        Me.btnOpenGenSetEM.Location = New System.Drawing.Point(4, 3)
        Me.btnOpenGenSetEM.Name = "btnOpenGenSetEM"
        Me.btnOpenGenSetEM.Size = New System.Drawing.Size(94, 21)
        Me.btnOpenGenSetEM.TabIndex = 0
        Me.btnOpenGenSetEM.Text = "Electric Machine"
        Me.btnOpenGenSetEM.UseVisualStyleBackColor = true
        '
        'btnBrowseGenSetEM
        '
        Me.btnBrowseGenSetEM.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnBrowseGenSetEM.Image = CType(resources.GetObject("btnBrowseGenSetEM.Image"),System.Drawing.Image)
        Me.btnBrowseGenSetEM.Location = New System.Drawing.Point(406, 2)
        Me.btnBrowseGenSetEM.Name = "btnBrowseGenSetEM"
        Me.btnBrowseGenSetEM.Size = New System.Drawing.Size(24, 24)
        Me.btnBrowseGenSetEM.TabIndex = 2
        Me.btnBrowseGenSetEM.UseVisualStyleBackColor = true
        '
        'tbGenSetEM
        '
        Me.tbGenSetEM.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbGenSetEM.Location = New System.Drawing.Point(104, 4)
        Me.tbGenSetEM.Name = "tbGenSetEM"
        Me.tbGenSetEM.Size = New System.Drawing.Size(301, 20)
        Me.tbGenSetEM.TabIndex = 1
        '
        'tpTorqueLimits
        '
        Me.tpTorqueLimits.Controls.Add(Me.gbPropulsionTorque)
        Me.tpTorqueLimits.Controls.Add(Me.gbEMTorqueLimits)
        Me.tpTorqueLimits.Controls.Add(Me.bgVehicleTorqueLimits)
        Me.tpTorqueLimits.Location = New System.Drawing.Point(4, 22)
        Me.tpTorqueLimits.Name = "tpTorqueLimits"
        Me.tpTorqueLimits.Size = New System.Drawing.Size(648, 374)
        Me.tpTorqueLimits.TabIndex = 2
        Me.tpTorqueLimits.Text = "Torque Limits"
        Me.tpTorqueLimits.UseVisualStyleBackColor = true
        '
        'gbPropulsionTorque
        '
        Me.gbPropulsionTorque.Controls.Add(Me.btnPropulsionTorqueLimit)
        Me.gbPropulsionTorque.Controls.Add(Me.tbPropulsionTorqueLimit)
        Me.gbPropulsionTorque.Location = New System.Drawing.Point(233, 64)
        Me.gbPropulsionTorque.Name = "gbPropulsionTorque"
        Me.gbPropulsionTorque.Size = New System.Drawing.Size(403, 52)
        Me.gbPropulsionTorque.TabIndex = 2
        Me.gbPropulsionTorque.TabStop = false
        Me.gbPropulsionTorque.Text = "Propulsion Torque Limit"
        '
        'btnPropulsionTorqueLimit
        '
        Me.btnPropulsionTorqueLimit.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnPropulsionTorqueLimit.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnPropulsionTorqueLimit.Location = New System.Drawing.Point(373, 18)
        Me.btnPropulsionTorqueLimit.Name = "btnPropulsionTorqueLimit"
        Me.btnPropulsionTorqueLimit.Size = New System.Drawing.Size(24, 24)
        Me.btnPropulsionTorqueLimit.TabIndex = 1
        Me.btnPropulsionTorqueLimit.UseVisualStyleBackColor = true
        '
        'tbPropulsionTorqueLimit
        '
        Me.tbPropulsionTorqueLimit.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbPropulsionTorqueLimit.Location = New System.Drawing.Point(6, 20)
        Me.tbPropulsionTorqueLimit.Name = "tbPropulsionTorqueLimit"
        Me.tbPropulsionTorqueLimit.Size = New System.Drawing.Size(365, 20)
        Me.tbPropulsionTorqueLimit.TabIndex = 0
        '
        'gbEMTorqueLimits
        '
        Me.gbEMTorqueLimits.Controls.Add(Me.btnEmTorqueLimits)
        Me.gbEMTorqueLimits.Controls.Add(Me.tbEmTorqueLimits)
        Me.gbEMTorqueLimits.Location = New System.Drawing.Point(233, 6)
        Me.gbEMTorqueLimits.Name = "gbEMTorqueLimits"
        Me.gbEMTorqueLimits.Size = New System.Drawing.Size(403, 52)
        Me.gbEMTorqueLimits.TabIndex = 1
        Me.gbEMTorqueLimits.TabStop = false
        Me.gbEMTorqueLimits.Text = "Electric Machine Torque Limits"
        '
        'btnEmTorqueLimits
        '
        Me.btnEmTorqueLimits.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnEmTorqueLimits.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btnEmTorqueLimits.Location = New System.Drawing.Point(373, 17)
        Me.btnEmTorqueLimits.Name = "btnEmTorqueLimits"
        Me.btnEmTorqueLimits.Size = New System.Drawing.Size(24, 24)
        Me.btnEmTorqueLimits.TabIndex = 1
        Me.btnEmTorqueLimits.UseVisualStyleBackColor = true
        '
        'tbEmTorqueLimits
        '
        Me.tbEmTorqueLimits.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbEmTorqueLimits.Location = New System.Drawing.Point(6, 19)
        Me.tbEmTorqueLimits.Name = "tbEmTorqueLimits"
        Me.tbEmTorqueLimits.Size = New System.Drawing.Size(365, 20)
        Me.tbEmTorqueLimits.TabIndex = 0
        '
        'bgVehicleTorqueLimits
        '
        Me.bgVehicleTorqueLimits.Controls.Add(Me.lvTorqueLimits)
        Me.bgVehicleTorqueLimits.Controls.Add(Me.btAddMaxTorqueEntry)
        Me.bgVehicleTorqueLimits.Controls.Add(Me.Label17)
        Me.bgVehicleTorqueLimits.Controls.Add(Me.btDelMaxTorqueEntry)
        Me.bgVehicleTorqueLimits.Location = New System.Drawing.Point(6, 6)
        Me.bgVehicleTorqueLimits.Name = "bgVehicleTorqueLimits"
        Me.bgVehicleTorqueLimits.Size = New System.Drawing.Size(221, 227)
        Me.bgVehicleTorqueLimits.TabIndex = 0
        Me.bgVehicleTorqueLimits.TabStop = false
        Me.bgVehicleTorqueLimits.Text = "ICE Torque Limits"
        '
        'lvTorqueLimits
        '
        Me.lvTorqueLimits.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.lvTorqueLimits.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader5, Me.ColumnHeader6})
        Me.lvTorqueLimits.FullRowSelect = true
        Me.lvTorqueLimits.GridLines = true
        Me.lvTorqueLimits.HideSelection = false
        Me.lvTorqueLimits.Location = New System.Drawing.Point(6, 19)
        Me.lvTorqueLimits.MultiSelect = false
        Me.lvTorqueLimits.Name = "lvTorqueLimits"
        Me.lvTorqueLimits.Size = New System.Drawing.Size(209, 169)
        Me.lvTorqueLimits.TabIndex = 7
        Me.lvTorqueLimits.TabStop = false
        Me.lvTorqueLimits.UseCompatibleStateImageBehavior = false
        Me.lvTorqueLimits.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader5
        '
        Me.ColumnHeader5.Text = "Gear #"
        Me.ColumnHeader5.Width = 71
        '
        'ColumnHeader6
        '
        Me.ColumnHeader6.Text = "Max. Torque"
        Me.ColumnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right
        Me.ColumnHeader6.Width = 133
        '
        'btAddMaxTorqueEntry
        '
        Me.btAddMaxTorqueEntry.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btAddMaxTorqueEntry.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.btAddMaxTorqueEntry.Location = New System.Drawing.Point(6, 193)
        Me.btAddMaxTorqueEntry.Name = "btAddMaxTorqueEntry"
        Me.btAddMaxTorqueEntry.Size = New System.Drawing.Size(24, 24)
        Me.btAddMaxTorqueEntry.TabIndex = 4
        Me.btAddMaxTorqueEntry.UseVisualStyleBackColor = true
        '
        'Label17
        '
        Me.Label17.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.Label17.AutoSize = true
        Me.Label17.Location = New System.Drawing.Point(112, 190)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(106, 13)
        Me.Label17.TabIndex = 6
        Me.Label17.Text = "(Double-Click to Edit)"
        Me.Label17.TextAlign = System.Drawing.ContentAlignment.TopRight
        '
        'btDelMaxTorqueEntry
        '
        Me.btDelMaxTorqueEntry.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
        Me.btDelMaxTorqueEntry.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.btDelMaxTorqueEntry.Location = New System.Drawing.Point(30, 193)
        Me.btDelMaxTorqueEntry.Name = "btDelMaxTorqueEntry"
        Me.btDelMaxTorqueEntry.Size = New System.Drawing.Size(24, 24)
        Me.btDelMaxTorqueEntry.TabIndex = 5
        Me.btDelMaxTorqueEntry.UseVisualStyleBackColor = true
        '
        'tpADAS
        '
        Me.tpADAS.Controls.Add(Me.gbADAS)
        Me.tpADAS.Location = New System.Drawing.Point(4, 22)
        Me.tpADAS.Name = "tpADAS"
        Me.tpADAS.Padding = New System.Windows.Forms.Padding(3)
        Me.tpADAS.Size = New System.Drawing.Size(648, 374)
        Me.tpADAS.TabIndex = 3
        Me.tpADAS.Text = "ADAS"
        Me.tpADAS.UseVisualStyleBackColor = true
        '
        'gbADAS
        '
        Me.gbADAS.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.gbADAS.Controls.Add(Me.pnEcoRoll)
        Me.gbADAS.Controls.Add(Me.cbAtEcoRollReleaseLockupClutch)
        Me.gbADAS.Controls.Add(Me.cbPcc)
        Me.gbADAS.Controls.Add(Me.cbEngineStopStart)
        Me.gbADAS.Controls.Add(Me.lblPCC)
        Me.gbADAS.Location = New System.Drawing.Point(6, 6)
        Me.gbADAS.Name = "gbADAS"
        Me.gbADAS.Size = New System.Drawing.Size(636, 161)
        Me.gbADAS.TabIndex = 0
        Me.gbADAS.TabStop = false
        Me.gbADAS.Text = "ADAS Options"
        '
        'pnEcoRoll
        '
        Me.pnEcoRoll.Controls.Add(Me.cbEcoRoll)
        Me.pnEcoRoll.Controls.Add(Me.Label22)
        Me.pnEcoRoll.Location = New System.Drawing.Point(0, 44)
        Me.pnEcoRoll.Name = "pnEcoRoll"
        Me.pnEcoRoll.Size = New System.Drawing.Size(300, 46)
        Me.pnEcoRoll.TabIndex = 7
        '
        'cbEcoRoll
        '
        Me.cbEcoRoll.DisplayMember = "Value"
        Me.cbEcoRoll.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbEcoRoll.FormattingEnabled = true
        Me.cbEcoRoll.Location = New System.Drawing.Point(10, 20)
        Me.cbEcoRoll.Name = "cbEcoRoll"
        Me.cbEcoRoll.Size = New System.Drawing.Size(266, 21)
        Me.cbEcoRoll.TabIndex = 1
        Me.cbEcoRoll.ValueMember = "Key"
        '
        'Label22
        '
        Me.Label22.AutoSize = true
        Me.Label22.Location = New System.Drawing.Point(7, 5)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(50, 13)
        Me.Label22.TabIndex = 6
        Me.Label22.Text = "Eco-Roll:"
        '
        'cbAtEcoRollReleaseLockupClutch
        '
        Me.cbAtEcoRollReleaseLockupClutch.AutoSize = true
        Me.cbAtEcoRollReleaseLockupClutch.Location = New System.Drawing.Point(238, 21)
        Me.cbAtEcoRollReleaseLockupClutch.Name = "cbAtEcoRollReleaseLockupClutch"
        Me.cbAtEcoRollReleaseLockupClutch.Size = New System.Drawing.Size(243, 17)
        Me.cbAtEcoRollReleaseLockupClutch.TabIndex = 3
        Me.cbAtEcoRollReleaseLockupClutch.Text = "AT Gearbox: Eco-Roll Release Lockup Clutch"
        Me.cbAtEcoRollReleaseLockupClutch.UseVisualStyleBackColor = true
        '
        'cbPcc
        '
        Me.cbPcc.DisplayMember = "Value"
        Me.cbPcc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPcc.FormattingEnabled = true
        Me.cbPcc.Location = New System.Drawing.Point(10, 124)
        Me.cbPcc.Name = "cbPcc"
        Me.cbPcc.Size = New System.Drawing.Size(266, 21)
        Me.cbPcc.TabIndex = 2
        Me.cbPcc.ValueMember = "Key"
        '
        'cbEngineStopStart
        '
        Me.cbEngineStopStart.AutoSize = true
        Me.cbEngineStopStart.Location = New System.Drawing.Point(10, 21)
        Me.cbEngineStopStart.Name = "cbEngineStopStart"
        Me.cbEngineStopStart.Size = New System.Drawing.Size(203, 17)
        Me.cbEngineStopStart.TabIndex = 0
        Me.cbEngineStopStart.Text = "Engine Stop/Start during vehicle stop"
        Me.cbEngineStopStart.UseVisualStyleBackColor = true
        '
        'lblPCC
        '
        Me.lblPCC.AutoSize = true
        Me.lblPCC.Location = New System.Drawing.Point(7, 108)
        Me.lblPCC.Name = "lblPCC"
        Me.lblPCC.Size = New System.Drawing.Size(125, 13)
        Me.lblPCC.TabIndex = 3
        Me.lblPCC.Text = "Predictive Cruise Control:"
        '
        'tpRoadSweeper
        '
        Me.tpRoadSweeper.Controls.Add(Me.pnPTO)
        Me.tpRoadSweeper.Controls.Add(Me.gbPTO)
        Me.tpRoadSweeper.Location = New System.Drawing.Point(4, 22)
        Me.tpRoadSweeper.Name = "tpRoadSweeper"
        Me.tpRoadSweeper.Size = New System.Drawing.Size(648, 374)
        Me.tpRoadSweeper.TabIndex = 4
        Me.tpRoadSweeper.Text = "PTO"
        Me.tpRoadSweeper.UseVisualStyleBackColor = true
        '
        'pnPTO
        '
        Me.pnPTO.Controls.Add(Me.gbPTODrive)
        Me.pnPTO.Controls.Add(Me.btPTOCycleDrive)
        Me.pnPTO.Controls.Add(Me.Label28)
        Me.pnPTO.Controls.Add(Me.tbPTODrive)
        Me.pnPTO.Controls.Add(Me.btPTOCycle)
        Me.pnPTO.Controls.Add(Me.Label16)
        Me.pnPTO.Controls.Add(Me.tbPTOCycle)
        Me.pnPTO.Controls.Add(Me.btPTOLossMapBrowse)
        Me.pnPTO.Controls.Add(Me.Label7)
        Me.pnPTO.Controls.Add(Me.tbPTOLossMap)
        Me.pnPTO.Location = New System.Drawing.Point(6, 73)
        Me.pnPTO.Name = "pnPTO"
        Me.pnPTO.Size = New System.Drawing.Size(633, 211)
        Me.pnPTO.TabIndex = 1
        '
        'gbPTODrive
        '
        Me.gbPTODrive.Controls.Add(Me.Label27)
        Me.gbPTODrive.Controls.Add(Me.tbPtoGear)
        Me.gbPTODrive.Controls.Add(Me.Label26)
        Me.gbPTODrive.Controls.Add(Me.tbPtoEngineSpeed)
        Me.gbPTODrive.Controls.Add(Me.Label25)
        Me.gbPTODrive.Controls.Add(Me.Label24)
        Me.gbPTODrive.Location = New System.Drawing.Point(0, 99)
        Me.gbPTODrive.Name = "gbPTODrive"
        Me.gbPTODrive.Size = New System.Drawing.Size(633, 55)
        Me.gbPTODrive.TabIndex = 4
        Me.gbPTODrive.TabStop = false
        Me.gbPTODrive.Text = "Working operation settings (PTO mode 2)"
        '
        'Label27
        '
        Me.Label27.AutoSize = true
        Me.Label27.Location = New System.Drawing.Point(446, 24)
        Me.Label27.Name = "Label27"
        Me.Label27.Size = New System.Drawing.Size(20, 13)
        Me.Label27.TabIndex = 6
        Me.Label27.Text = "[#]"
        Me.Label27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbPtoGear
        '
        Me.tbPtoGear.Location = New System.Drawing.Point(371, 21)
        Me.tbPtoGear.Name = "tbPtoGear"
        Me.tbPtoGear.Size = New System.Drawing.Size(70, 20)
        Me.tbPtoGear.TabIndex = 1
        '
        'Label26
        '
        Me.Label26.AutoSize = true
        Me.Label26.Location = New System.Drawing.Point(189, 24)
        Me.Label26.Name = "Label26"
        Me.Label26.Size = New System.Drawing.Size(30, 13)
        Me.Label26.TabIndex = 4
        Me.Label26.Text = "[rpm]"
        Me.Label26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'tbPtoEngineSpeed
        '
        Me.tbPtoEngineSpeed.Location = New System.Drawing.Point(113, 21)
        Me.tbPtoEngineSpeed.Name = "tbPtoEngineSpeed"
        Me.tbPtoEngineSpeed.Size = New System.Drawing.Size(70, 20)
        Me.tbPtoEngineSpeed.TabIndex = 0
        '
        'Label25
        '
        Me.Label25.AutoSize = true
        Me.Label25.Location = New System.Drawing.Point(297, 24)
        Me.Label25.Name = "Label25"
        Me.Label25.Size = New System.Drawing.Size(68, 13)
        Me.Label25.TabIndex = 1
        Me.Label25.Text = "Gear number"
        '
        'Label24
        '
        Me.Label24.AutoSize = true
        Me.Label24.Location = New System.Drawing.Point(7, 24)
        Me.Label24.Name = "Label24"
        Me.Label24.Size = New System.Drawing.Size(97, 13)
        Me.Label24.TabIndex = 0
        Me.Label24.Text = "Min. Engine Speed"
        '
        'btPTOCycleDrive
        '
        Me.btPTOCycleDrive.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOCycleDrive.Location = New System.Drawing.Point(438, 181)
        Me.btPTOCycleDrive.Name = "btPTOCycleDrive"
        Me.btPTOCycleDrive.Size = New System.Drawing.Size(24, 24)
        Me.btPTOCycleDrive.TabIndex = 6
        Me.btPTOCycleDrive.UseVisualStyleBackColor = true
        '
        'Label28
        '
        Me.Label28.Location = New System.Drawing.Point(3, 164)
        Me.Label28.Name = "Label28"
        Me.Label28.Size = New System.Drawing.Size(266, 16)
        Me.Label28.TabIndex = 21
        Me.Label28.Text = "PTO Cycle during driving (PTO mode 3) (.vptor)"
        Me.Label28.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'btPTOCycle
        '
        Me.btPTOCycle.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOCycle.Location = New System.Drawing.Point(438, 69)
        Me.btPTOCycle.Name = "btPTOCycle"
        Me.btPTOCycle.Size = New System.Drawing.Size(24, 24)
        Me.btPTOCycle.TabIndex = 3
        Me.btPTOCycle.UseVisualStyleBackColor = true
        '
        'Label16
        '
        Me.Label16.Location = New System.Drawing.Point(3, 52)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(266, 16)
        Me.Label16.TabIndex = 18
        Me.Label16.Text = "PTO Cycle during standstill (PTO mode 1) (.vptoc)"
        Me.Label16.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'btPTOLossMapBrowse
        '
        Me.btPTOLossMapBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.btPTOLossMapBrowse.Location = New System.Drawing.Point(438, 22)
        Me.btPTOLossMapBrowse.Name = "btPTOLossMapBrowse"
        Me.btPTOLossMapBrowse.Size = New System.Drawing.Size(24, 24)
        Me.btPTOLossMapBrowse.TabIndex = 1
        Me.btPTOLossMapBrowse.UseVisualStyleBackColor = true
        '
        'Label7
        '
        Me.Label7.Location = New System.Drawing.Point(3, 5)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(201, 16)
        Me.Label7.TabIndex = 15
        Me.Label7.Text = "PTO Consumer Loss Map (.vptoi)"
        Me.Label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft
        '
        'gbPTO
        '
        Me.gbPTO.Controls.Add(Me.cbPTOType)
        Me.gbPTO.Location = New System.Drawing.Point(6, 6)
        Me.gbPTO.Name = "gbPTO"
        Me.gbPTO.Size = New System.Drawing.Size(633, 61)
        Me.gbPTO.TabIndex = 0
        Me.gbPTO.TabStop = false
        Me.gbPTO.Text = "PTO Design Variant"
        '
        'cbLegislativeClass
        '
        Me.cbLegislativeClass.DisplayMember = "Value"
        Me.cbLegislativeClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbLegislativeClass.FormattingEnabled = true
        Me.cbLegislativeClass.Location = New System.Drawing.Point(220, 140)
        Me.cbLegislativeClass.Name = "cbLegislativeClass"
        Me.cbLegislativeClass.Size = New System.Drawing.Size(52, 21)
        Me.cbLegislativeClass.TabIndex = 4
        Me.cbLegislativeClass.ValueMember = "Key"
        '
        'Label21
        '
        Me.Label21.AutoSize = true
        Me.Label21.Location = New System.Drawing.Point(32, 123)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(112, 13)
        Me.Label21.TabIndex = 42
        Me.Label21.Text = "Maximum Laden Mass"
        '
        'lblTitle
        '
        Me.lblTitle.AutoSize = true
        Me.lblTitle.BackColor = System.Drawing.Color.White
        Me.lblTitle.Font = New System.Drawing.Font("Microsoft Sans Serif", 18!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblTitle.Location = New System.Drawing.Point(117, 34)
        Me.lblTitle.Name = "lblTitle"
        Me.lblTitle.Size = New System.Drawing.Size(166, 29)
        Me.lblTitle.TabIndex = 43
        Me.lblTitle.Text = "Vehicle TITLE"
        '
        'FlowLayoutPanel1
        '
        Me.FlowLayoutPanel1.Controls.Add(Me.btnIEPC)
        Me.FlowLayoutPanel1.Controls.Add(Me.tbIEPCFilePath)
        Me.FlowLayoutPanel1.Controls.Add(Me.btIEPCFilePath)
        Me.FlowLayoutPanel1.Location = New System.Drawing.Point(25, 19)
        Me.FlowLayoutPanel1.Name = "FlowLayoutPanel1"
        Me.FlowLayoutPanel1.Size = New System.Drawing.Size(430, 27)
        Me.FlowLayoutPanel1.TabIndex = 61
        '
        'FlowLayoutPanel2
        '
        Me.FlowLayoutPanel2.Controls.Add(Me.btIHPC)
        Me.FlowLayoutPanel2.Controls.Add(Me.tbIHPCFilePath)
        Me.FlowLayoutPanel2.Controls.Add(Me.btIHPCFile)
        Me.FlowLayoutPanel2.Location = New System.Drawing.Point(25, 19)
        Me.FlowLayoutPanel2.Name = "FlowLayoutPanel2"
        Me.FlowLayoutPanel2.Size = New System.Drawing.Size(427, 27)
        Me.FlowLayoutPanel2.TabIndex = 3
        '
        'VehicleForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(666, 625)
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
        Me.Icon = CType(resources.GetObject("$this.Icon"),System.Drawing.Icon)
        Me.MaximizeBox = false
        Me.Name = "VehicleForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "F05_VEH"
        Me.GroupBox6.ResumeLayout(false)
        Me.GroupBox6.PerformLayout
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.gbRetarderLosses.ResumeLayout(false)
        Me.PnRt.ResumeLayout(false)
        Me.PnRt.PerformLayout
        Me.GroupBox8.ResumeLayout(false)
        Me.GroupBox8.PerformLayout
        Me.PnWheelDiam.ResumeLayout(false)
        Me.PnWheelDiam.PerformLayout
        Me.StatusStrip1.ResumeLayout(false)
        Me.StatusStrip1.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.PnLoad.ResumeLayout(false)
        Me.PnLoad.PerformLayout
        Me.GrAirRes.ResumeLayout(false)
        Me.PnCdATrTr.ResumeLayout(false)
        Me.PnCdATrTr.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.CmOpenFile.ResumeLayout(false)
        Me.GroupBox3.ResumeLayout(false)
        Me.gbAngledrive.ResumeLayout(false)
        Me.pnAngledriveFields.ResumeLayout(false)
        Me.pnAngledriveFields.PerformLayout
        CType(Me.PicVehicle,System.ComponentModel.ISupportInitialize).EndInit
        Me.tcVehicleComponents.ResumeLayout(false)
        Me.tpGeneral.ResumeLayout(false)
        Me.tpPowertrain.ResumeLayout(false)
        Me.gbVehicleIdlingSpeed.ResumeLayout(false)
        Me.Panel1.ResumeLayout(false)
        Me.Panel1.PerformLayout
        Me.gbTankSystem.ResumeLayout(false)
        Me.gbTankSystem.PerformLayout
        Me.tpElectricMachine.ResumeLayout(false)
        Me.gpElectricMotor.ResumeLayout(false)
        Me.gpElectricMotor.PerformLayout
        Me.gbRatiosPerGear.ResumeLayout(false)
        Me.gbRatiosPerGear.PerformLayout
        Me.pnElectricMotor.ResumeLayout(false)
        Me.pnElectricMotor.PerformLayout
        Me.tpIEPC.ResumeLayout(false)
        Me.tbIHPC.ResumeLayout(false)
        Me.tpReess.ResumeLayout(false)
        Me.gbBattery.ResumeLayout(false)
        Me.gbBattery.PerformLayout
        Me.tpGensetComponents.ResumeLayout(false)
        Me.tpGensetComponents.PerformLayout
        Me.gbGenSet.ResumeLayout(false)
        Me.gbGenSet.PerformLayout
        Me.pnGenSetEM.ResumeLayout(false)
        Me.pnGenSetEM.PerformLayout
        Me.tpTorqueLimits.ResumeLayout(false)
        Me.gbPropulsionTorque.ResumeLayout(false)
        Me.gbPropulsionTorque.PerformLayout
        Me.gbEMTorqueLimits.ResumeLayout(false)
        Me.gbEMTorqueLimits.PerformLayout
        Me.bgVehicleTorqueLimits.ResumeLayout(false)
        Me.bgVehicleTorqueLimits.PerformLayout
        Me.tpADAS.ResumeLayout(false)
        Me.gbADAS.ResumeLayout(false)
        Me.gbADAS.PerformLayout
        Me.pnEcoRoll.ResumeLayout(false)
        Me.pnEcoRoll.PerformLayout
        Me.tpRoadSweeper.ResumeLayout(false)
        Me.pnPTO.ResumeLayout(false)
        Me.pnPTO.PerformLayout
        Me.gbPTODrive.ResumeLayout(false)
        Me.gbPTODrive.PerformLayout
        Me.gbPTO.ResumeLayout(false)
        Me.FlowLayoutPanel1.ResumeLayout(false)
        Me.FlowLayoutPanel1.PerformLayout
        Me.FlowLayoutPanel2.ResumeLayout(false)
        Me.FlowLayoutPanel2.PerformLayout
        Me.ResumeLayout(false)
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
	Friend WithEvents gbRetarderLosses As System.Windows.Forms.GroupBox
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
	Friend WithEvents gbAngledrive As System.Windows.Forms.GroupBox
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
'<<<<<<< HEAD
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
'=======
'	Friend WithEvents TabControl1 As System.Windows.Forms.TabControl
'	Friend WithEvents TabPage1 As System.Windows.Forms.TabPage
'	Friend WithEvents TabPage2 As System.Windows.Forms.TabPage
'	Friend WithEvents TabPage3 As System.Windows.Forms.TabPage
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
    Friend WithEvents gbADAS As GroupBox
    Friend WithEvents cbEngineStopStart As CheckBox
    Friend WithEvents lblPCC As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents cbPcc As ComboBox
    Friend WithEvents cbEcoRoll As ComboBox
    Friend WithEvents gbTankSystem As GroupBox
    Friend WithEvents cbTankSystem As ComboBox
    Friend WithEvents Label23 As Label
    Friend WithEvents cbAtEcoRollReleaseLockupClutch As CheckBox
    Friend WithEvents lblTitle As Label
    Friend WithEvents gbVehicleIdlingSpeed As GroupBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents tbVehIdlingSpeed As TextBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents gbEMTorqueLimits As GroupBox
    Friend WithEvents btnEmTorqueLimits As Button
    Friend WithEvents tbEmTorqueLimits As TextBox
    Friend WithEvents bgVehicleTorqueLimits As GroupBox
    Friend WithEvents gbPropulsionTorque As GroupBox
    Friend WithEvents btnPropulsionTorqueLimit As Button
    Friend WithEvents tbPropulsionTorqueLimit As TextBox
'=======
    Friend WithEvents tpRoadSweeper As TabPage
    Friend WithEvents gbPTODrive As GroupBox
    Friend WithEvents tbPtoGear As TextBox
    Friend WithEvents Label26 As Label
    Friend WithEvents tbPtoEngineSpeed As TextBox
    Friend WithEvents Label25 As Label
    Friend WithEvents Label24 As Label
    'Friend WithEvents gbPTO As GroupBox
    'Friend WithEvents cbPTOType As ComboBox
    'Friend WithEvents pnPTO As Panel
    'Friend WithEvents btPTOCycle As Button
    'Friend WithEvents Label16 As Label
    'Friend WithEvents tbPTOCycle As TextBox
    'Friend WithEvents btPTOLossMapBrowse As Button
    'Friend WithEvents Label7 As Label
    'Friend WithEvents tbPTOLossMap As TextBox
    Friend WithEvents Label27 As Label
    Friend WithEvents btPTOCycleDrive As Button
    Friend WithEvents Label28 As Label
    Friend WithEvents tbPTODrive As TextBox
    Friend WithEvents tpGensetComponents As TabPage
    Friend WithEvents gbGenSet As GroupBox
    Friend WithEvents btnGenSetLossMap As Button
    Friend WithEvents tbGenSetADC As TextBox
    Friend WithEvents lblGenSetADC As Label
    Friend WithEvents tbGenSetRatio As TextBox
    Friend WithEvents lblGenSetRatio As Label
    Friend WithEvents tbGenSetCount As TextBox
    Friend WithEvents lblGenSetCount As Label
    Friend WithEvents pnGenSetEM As Panel
    Friend WithEvents btnOpenGenSetEM As Button
    Friend WithEvents btnBrowseGenSetEM As Button
    Friend WithEvents tbGenSetEM As TextBox
    Friend WithEvents Label30 As Label
	Friend WithEvents Label36 As Label
	Friend WithEvents Label34 As Label
	Friend WithEvents pnEcoRoll As Panel
    Friend WithEvents tpElectricMachine As TabPage
    Friend WithEvents tpReess As TabPage
    Friend WithEvents gpElectricMotor As GroupBox
    Friend WithEvents Label33 As Label
    Friend WithEvents Label32 As Label
    Friend WithEvents gbRatiosPerGear As GroupBox
    Friend WithEvents lvRatioPerGear As ListView
    Friend WithEvents ColumnHeader11 As ColumnHeader
    Friend WithEvents ColumnHeader12 As ColumnHeader
    Friend WithEvents btnAddEMRatio As Button
    Friend WithEvents Label29 As Label
    Friend WithEvents btnRemoveEMRatio As Button
    Friend WithEvents btnEmADCLossMap As Button
    Friend WithEvents tbEmADCLossMap As TextBox
    Friend WithEvents lblEmADCLossmap As Label
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
    Friend WithEvents gbBattery As GroupBox
    Friend WithEvents lvREESSPacks As ListView
    Friend WithEvents chReessPackPack As ColumnHeader
    Friend WithEvents chReessPackCount As ColumnHeader
    Friend WithEvents chReessPackStringId As ColumnHeader
    Friend WithEvents btnAddReessPack As Button
    Friend WithEvents lblEditReessPack As Label
    Friend WithEvents btnRemoveReessPack As Button
    Friend WithEvents lblInitialSoCUnit As Label
    Friend WithEvents tbInitialSoC As TextBox
    Friend WithEvents lblInitialSoC As Label
    Friend WithEvents tpIEPC As TabPage
    Friend WithEvents btIEPCFilePath As Button
    Friend WithEvents btnIEPC As Button
    Friend WithEvents tbIEPCFilePath As TextBox
    Friend WithEvents tbIHPC As TabPage
    Friend WithEvents btIHPC As Button
    Friend WithEvents btIHPCFile As Button
    Friend WithEvents tbIHPCFilePath As TextBox
    Friend WithEvents FlowLayoutPanel1 As FlowLayoutPanel
    Friend WithEvents FlowLayoutPanel2 As FlowLayoutPanel
    '>>>>>>> VECTO_CERT/master
End Class
