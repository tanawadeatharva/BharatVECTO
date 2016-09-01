Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class F_VECTO
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
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(F_VECTO))
		Me.TabPgGen = New TabPage()
		Me.GrCycles = New GroupBox()
		Me.Label2 = New Label()
		Me.LvCycles = New ListView()
		Me.ColumnHeader1 = CType(New ColumnHeader(), ColumnHeader)
		Me.BtDRIrem = New Button()
		Me.BtDRIadd = New Button()
		Me.GrAux = New GroupBox()
		Me.btnAAUXOpen = New Button()
		Me.Label1 = New Label()
		Me.btnBrowseAAUXFile = New Button()
		Me.txtAdvancedAuxiliaryFile = New TextBox()
		Me.picAuxInfo = New PictureBox()
		Me.cboAdvancedAuxiliaries = New ComboBox()
		Me.lbAdvancedAuxiliaries = New Label()
		Me.Label32 = New Label()
		Me.LvAux = New ListView()
		Me.ColumnHeader4 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader5 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader6 = CType(New ColumnHeader(), ColumnHeader)
		Me.ButAuxRem = New Button()
		Me.ButAuxAdd = New Button()
		Me.TbGBX = New TextBox()
		Me.TbENG = New TextBox()
		Me.TbVEH = New TextBox()
		Me.ButOpenGBX = New Button()
		Me.ButOpenENG = New Button()
		Me.ButOpenVEH = New Button()
		Me.ButtonVEH = New Button()
		Me.ButtonGBX = New Button()
		Me.ButtonMAP = New Button()
		Me.TabControl1 = New TabControl()
		Me.TabPgDriver = New TabPage()
		Me.GrVACC = New GroupBox()
		Me.TbDesMaxFile = New TextBox()
		Me.BtDesMaxBr = New Button()
		Me.BtAccOpen = New Button()
		Me.GrLAC = New GroupBox()
		Me.Label12 = New Label()
		Me.tbDfCoastingScale = New TextBox()
		Me.CbLookAhead = New CheckBox()
		Me.Label11 = New Label()
		Me.Label3 = New Label()
		Me.tbDfCoastingOffset = New TextBox()
		Me.tbLacDfTargetSpeedFile = New TextBox()
		Me.Label10 = New Label()
		Me.Label4 = New Label()
		Me.Label5 = New Label()
		Me.btnDfTargetSpeed = New Button()
		Me.tbLacPreviewFactor = New TextBox()
		Me.tbLacDfVelocityDropFile = New TextBox()
		Me.GroupBox1 = New GroupBox()
		Me.PnEcoRoll = New Panel()
		Me.Label21 = New Label()
		Me.Label20 = New Label()
		Me.Label14 = New Label()
		Me.TbVmin = New TextBox()
		Me.TbUnderSpeed = New TextBox()
		Me.TbOverspeed = New TextBox()
		Me.Label23 = New Label()
		Me.Label22 = New Label()
		Me.Label13 = New Label()
		Me.RdEcoRoll = New RadioButton()
		Me.RdOverspeed = New RadioButton()
		Me.RdOff = New RadioButton()
		Me.GrStartStop = New GroupBox()
		Me.PnStartStop = New Panel()
		Me.Label31 = New Label()
		Me.Label27 = New Label()
		Me.TbSSspeed = New TextBox()
		Me.LabelSSspeed = New Label()
		Me.Label26 = New Label()
		Me.Label30 = New Label()
		Me.LabelSStime = New Label()
		Me.TbSSdelay = New TextBox()
		Me.TbSStime = New TextBox()
		Me.ChBStartStop = New CheckBox()
		Me.StatusStrip1 = New StatusStrip()
		Me.ToolStripStatusLabelGEN = New ToolStripStatusLabel()
		Me.ButOK = New Button()
		Me.ButCancel = New Button()
		Me.ToolStrip1 = New ToolStrip()
		Me.ToolStripBtNew = New ToolStripButton()
		Me.ToolStripBtOpen = New ToolStripButton()
		Me.ToolStripBtSave = New ToolStripButton()
		Me.ToolStripBtSaveAs = New ToolStripButton()
		Me.ToolStripSeparator1 = New ToolStripSeparator()
		Me.ToolStripBtSendTo = New ToolStripButton()
		Me.ToolStripSeparator2 = New ToolStripSeparator()
		Me.ToolStripButton1 = New ToolStripButton()
		Me.PictureBox1 = New PictureBox()
		Me.CbEngOnly = New CheckBox()
		Me.CmOpenFile = New ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New ToolStripMenuItem()
		Me.PicVehicle = New PictureBox()
		Me.PicBox = New PictureBox()
		Me.TbEngTxt = New TextBox()
		Me.TbVehCat = New TextBox()
		Me.TbAxleConf = New TextBox()
		Me.TbHVCclass = New TextBox()
		Me.TbGbxTxt = New TextBox()
		Me.TbMass = New TextBox()
		Me.ToolTip1 = New ToolTip(Me.components)
		Me.TabPgGen.SuspendLayout()
		Me.GrCycles.SuspendLayout()
		Me.GrAux.SuspendLayout()
		CType(Me.picAuxInfo, ISupportInitialize).BeginInit()
		Me.TabControl1.SuspendLayout()
		Me.TabPgDriver.SuspendLayout()
		Me.GrVACC.SuspendLayout()
		Me.GrLAC.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me.PnEcoRoll.SuspendLayout()
		Me.GrStartStop.SuspendLayout()
		Me.PnStartStop.SuspendLayout()
		Me.StatusStrip1.SuspendLayout()
		Me.ToolStrip1.SuspendLayout()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		Me.CmOpenFile.SuspendLayout()
		CType(Me.PicVehicle, ISupportInitialize).BeginInit()
		CType(Me.PicBox, ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'TabPgGen
		'
		Me.TabPgGen.Controls.Add(Me.GrCycles)
		Me.TabPgGen.Controls.Add(Me.GrAux)
		Me.TabPgGen.Controls.Add(Me.TbGBX)
		Me.TabPgGen.Controls.Add(Me.TbENG)
		Me.TabPgGen.Controls.Add(Me.TbVEH)
		Me.TabPgGen.Controls.Add(Me.ButOpenGBX)
		Me.TabPgGen.Controls.Add(Me.ButOpenENG)
		Me.TabPgGen.Controls.Add(Me.ButOpenVEH)
		Me.TabPgGen.Controls.Add(Me.ButtonVEH)
		Me.TabPgGen.Controls.Add(Me.ButtonGBX)
		Me.TabPgGen.Controls.Add(Me.ButtonMAP)
		Me.TabPgGen.Location = New Point(4, 22)
		Me.TabPgGen.Name = "TabPgGen"
		Me.TabPgGen.Padding = New Padding(3)
		Me.TabPgGen.Size = New Size(527, 534)
		Me.TabPgGen.TabIndex = 0
		Me.TabPgGen.Text = "General"
		Me.TabPgGen.UseVisualStyleBackColor = True
		'
		'GrCycles
		'
		Me.GrCycles.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.GrCycles.Controls.Add(Me.Label2)
		Me.GrCycles.Controls.Add(Me.LvCycles)
		Me.GrCycles.Controls.Add(Me.BtDRIrem)
		Me.GrCycles.Controls.Add(Me.BtDRIadd)
		Me.GrCycles.Location = New Point(9, 314)
		Me.GrCycles.Name = "GrCycles"
		Me.GrCycles.Size = New Size(515, 184)
		Me.GrCycles.TabIndex = 10
		Me.GrCycles.TabStop = False
		Me.GrCycles.Text = "Cycles"
		'
		'Label2
		'
		Me.Label2.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(379, 148)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(133, 13)
		Me.Label2.TabIndex = 3
		Me.Label2.Text = "(Double-Click to Open File)"
		'
		'LvCycles
		'
		Me.LvCycles.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.LvCycles.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader1})
		Me.LvCycles.FullRowSelect = True
		Me.LvCycles.GridLines = True
		Me.LvCycles.HeaderStyle = ColumnHeaderStyle.None
		Me.LvCycles.HideSelection = False
		Me.LvCycles.LabelEdit = True
		Me.LvCycles.Location = New Point(6, 25)
		Me.LvCycles.MultiSelect = False
		Me.LvCycles.Name = "LvCycles"
		Me.LvCycles.Size = New Size(503, 123)
		Me.LvCycles.TabIndex = 0
		Me.LvCycles.TabStop = False
		Me.LvCycles.UseCompatibleStateImageBehavior = False
		Me.LvCycles.View = View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Cycle path"
		Me.ColumnHeader1.Width = 470
		'
		'BtDRIrem
		'
		Me.BtDRIrem.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtDRIrem.Image = My.Resources.Resources.minus_circle_icon
		Me.BtDRIrem.Location = New Point(29, 149)
		Me.BtDRIrem.Name = "BtDRIrem"
		Me.BtDRIrem.Size = New Size(24, 24)
		Me.BtDRIrem.TabIndex = 2
		Me.BtDRIrem.UseVisualStyleBackColor = True
		'
		'BtDRIadd
		'
		Me.BtDRIadd.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtDRIadd.Image = My.Resources.Resources.plus_circle_icon
		Me.BtDRIadd.Location = New Point(5, 149)
		Me.BtDRIadd.Name = "BtDRIadd"
		Me.BtDRIadd.Size = New Size(24, 24)
		Me.BtDRIadd.TabIndex = 1
		Me.BtDRIadd.UseVisualStyleBackColor = True
		'
		'GrAux
		'
		Me.GrAux.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.GrAux.Controls.Add(Me.btnAAUXOpen)
		Me.GrAux.Controls.Add(Me.Label1)
		Me.GrAux.Controls.Add(Me.btnBrowseAAUXFile)
		Me.GrAux.Controls.Add(Me.txtAdvancedAuxiliaryFile)
		Me.GrAux.Controls.Add(Me.picAuxInfo)
		Me.GrAux.Controls.Add(Me.cboAdvancedAuxiliaries)
		Me.GrAux.Controls.Add(Me.lbAdvancedAuxiliaries)
		Me.GrAux.Controls.Add(Me.Label32)
		Me.GrAux.Controls.Add(Me.LvAux)
		Me.GrAux.Controls.Add(Me.ButAuxRem)
		Me.GrAux.Controls.Add(Me.ButAuxAdd)
		Me.GrAux.Location = New Point(6, 87)
		Me.GrAux.Name = "GrAux"
		Me.GrAux.Size = New Size(515, 221)
		Me.GrAux.TabIndex = 9
		Me.GrAux.TabStop = False
		Me.GrAux.Text = "Auxiliaries"
		'
		'btnAAUXOpen
		'
		Me.btnAAUXOpen.Image = My.Resources.Resources.application_export_icon_small
		Me.btnAAUXOpen.Location = New Point(465, 45)
		Me.btnAAUXOpen.Name = "btnAAUXOpen"
		Me.btnAAUXOpen.Size = New Size(24, 24)
		Me.btnAAUXOpen.TabIndex = 41
		Me.btnAAUXOpen.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(7, 52)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(96, 13)
		Me.Label1.TabIndex = 40
		Me.Label1.Text = "Advanced Aux File"
		'
		'btnBrowseAAUXFile
		'
		Me.btnBrowseAAUXFile.Image = My.Resources.Resources.Open_icon
		Me.btnBrowseAAUXFile.Location = New Point(441, 45)
		Me.btnBrowseAAUXFile.Name = "btnBrowseAAUXFile"
		Me.btnBrowseAAUXFile.Size = New Size(24, 24)
		Me.btnBrowseAAUXFile.TabIndex = 39
		Me.ToolTip1.SetToolTip(Me.btnBrowseAAUXFile, "Configure/Browser  Advanced Auxiliary Files")
		Me.btnBrowseAAUXFile.UseVisualStyleBackColor = True
		'
		'txtAdvancedAuxiliaryFile
		'
		Me.txtAdvancedAuxiliaryFile.Location = New Point(119, 47)
		Me.txtAdvancedAuxiliaryFile.Name = "txtAdvancedAuxiliaryFile"
		Me.txtAdvancedAuxiliaryFile.Size = New Size(321, 20)
		Me.txtAdvancedAuxiliaryFile.TabIndex = 38
		'
		'picAuxInfo
		'
		Me.picAuxInfo.Image = My.Resources.Resources.Information_icon
		Me.picAuxInfo.InitialImage = My.Resources.Resources.Information_icon
		Me.picAuxInfo.Location = New Point(451, 19)
		Me.picAuxInfo.Name = "picAuxInfo"
		Me.picAuxInfo.Size = New Size(16, 16)
		Me.picAuxInfo.SizeMode = PictureBoxSizeMode.AutoSize
		Me.picAuxInfo.TabIndex = 37
		Me.picAuxInfo.TabStop = False
		'
		'cboAdvancedAuxiliaries
		'
		Me.cboAdvancedAuxiliaries.FormattingEnabled = True
		Me.cboAdvancedAuxiliaries.Location = New Point(119, 18)
		Me.cboAdvancedAuxiliaries.Name = "cboAdvancedAuxiliaries"
		Me.cboAdvancedAuxiliaries.Size = New Size(321, 21)
		Me.cboAdvancedAuxiliaries.TabIndex = 36
		'
		'lbAdvancedAuxiliaries
		'
		Me.lbAdvancedAuxiliaries.AutoSize = True
		Me.lbAdvancedAuxiliaries.Location = New Point(7, 21)
		Me.lbAdvancedAuxiliaries.Name = "lbAdvancedAuxiliaries"
		Me.lbAdvancedAuxiliaries.Size = New Size(72, 13)
		Me.lbAdvancedAuxiliaries.TabIndex = 35
		Me.lbAdvancedAuxiliaries.Text = "Auxiliary Type"
		'
		'Label32
		'
		Me.Label32.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.Label32.AutoSize = True
		Me.Label32.Location = New Point(406, 189)
		Me.Label32.Name = "Label32"
		Me.Label32.Size = New Size(106, 13)
		Me.Label32.TabIndex = 3
		Me.Label32.Text = "(Double-Click to Edit)"
		'
		'LvAux
		'
		Me.LvAux.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.LvAux.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
		Me.LvAux.FullRowSelect = True
		Me.LvAux.GridLines = True
		Me.LvAux.HideSelection = False
		Me.LvAux.Location = New Point(6, 72)
		Me.LvAux.MultiSelect = False
		Me.LvAux.Name = "LvAux"
		Me.LvAux.Size = New Size(503, 117)
		Me.LvAux.TabIndex = 0
		Me.LvAux.TabStop = False
		Me.LvAux.UseCompatibleStateImageBehavior = False
		Me.LvAux.View = View.Details
		'
		'ColumnHeader4
		'
		Me.ColumnHeader4.Text = "ID"
		Me.ColumnHeader4.Width = 45
		'
		'ColumnHeader5
		'
		Me.ColumnHeader5.Text = "Type"
		Me.ColumnHeader5.Width = 108
		'
		'ColumnHeader6
		'
		Me.ColumnHeader6.Text = "Input File"
		Me.ColumnHeader6.Width = 331
		'
		'ButAuxRem
		'
		Me.ButAuxRem.Image = My.Resources.Resources.minus_circle_icon
		Me.ButAuxRem.Location = New Point(29, 190)
		Me.ButAuxRem.Name = "ButAuxRem"
		Me.ButAuxRem.Size = New Size(24, 24)
		Me.ButAuxRem.TabIndex = 2
		Me.ButAuxRem.UseVisualStyleBackColor = True
		'
		'ButAuxAdd
		'
		Me.ButAuxAdd.Image = My.Resources.Resources.plus_circle_icon
		Me.ButAuxAdd.Location = New Point(5, 190)
		Me.ButAuxAdd.Name = "ButAuxAdd"
		Me.ButAuxAdd.Size = New Size(24, 24)
		Me.ButAuxAdd.TabIndex = 1
		Me.ButAuxAdd.UseVisualStyleBackColor = True
		'
		'TbGBX
		'
		Me.TbGBX.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbGBX.Location = New Point(84, 60)
		Me.TbGBX.Name = "TbGBX"
		Me.TbGBX.Size = New Size(411, 20)
		Me.TbGBX.TabIndex = 7
		'
		'TbENG
		'
		Me.TbENG.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbENG.Location = New Point(84, 33)
		Me.TbENG.Name = "TbENG"
		Me.TbENG.Size = New Size(411, 20)
		Me.TbENG.TabIndex = 4
		'
		'TbVEH
		'
		Me.TbVEH.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbVEH.Location = New Point(84, 7)
		Me.TbVEH.Name = "TbVEH"
		Me.TbVEH.Size = New Size(411, 20)
		Me.TbVEH.TabIndex = 1
		'
		'ButOpenGBX
		'
		Me.ButOpenGBX.Location = New Point(6, 60)
		Me.ButOpenGBX.Name = "ButOpenGBX"
		Me.ButOpenGBX.Size = New Size(72, 21)
		Me.ButOpenGBX.TabIndex = 6
		Me.ButOpenGBX.TabStop = False
		Me.ButOpenGBX.Text = "Gearbox"
		Me.ButOpenGBX.UseVisualStyleBackColor = True
		'
		'ButOpenENG
		'
		Me.ButOpenENG.Location = New Point(6, 33)
		Me.ButOpenENG.Name = "ButOpenENG"
		Me.ButOpenENG.Size = New Size(72, 21)
		Me.ButOpenENG.TabIndex = 3
		Me.ButOpenENG.TabStop = False
		Me.ButOpenENG.Text = "Engine"
		Me.ButOpenENG.UseVisualStyleBackColor = True
		'
		'ButOpenVEH
		'
		Me.ButOpenVEH.Location = New Point(6, 6)
		Me.ButOpenVEH.Name = "ButOpenVEH"
		Me.ButOpenVEH.Size = New Size(72, 21)
		Me.ButOpenVEH.TabIndex = 0
		Me.ButOpenVEH.TabStop = False
		Me.ButOpenVEH.Text = "Vehicle"
		Me.ButOpenVEH.UseVisualStyleBackColor = True
		'
		'ButtonVEH
		'
		Me.ButtonVEH.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonVEH.Image = CType(resources.GetObject("ButtonVEH.Image"), Image)
		Me.ButtonVEH.Location = New Point(496, 5)
		Me.ButtonVEH.Name = "ButtonVEH"
		Me.ButtonVEH.Size = New Size(24, 24)
		Me.ButtonVEH.TabIndex = 2
		Me.ButtonVEH.TabStop = False
		Me.ButtonVEH.UseVisualStyleBackColor = True
		'
		'ButtonGBX
		'
		Me.ButtonGBX.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonGBX.Image = CType(resources.GetObject("ButtonGBX.Image"), Image)
		Me.ButtonGBX.Location = New Point(496, 58)
		Me.ButtonGBX.Name = "ButtonGBX"
		Me.ButtonGBX.Size = New Size(24, 24)
		Me.ButtonGBX.TabIndex = 8
		Me.ButtonGBX.TabStop = False
		Me.ButtonGBX.UseVisualStyleBackColor = True
		'
		'ButtonMAP
		'
		Me.ButtonMAP.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonMAP.Image = CType(resources.GetObject("ButtonMAP.Image"), Image)
		Me.ButtonMAP.Location = New Point(496, 31)
		Me.ButtonMAP.Name = "ButtonMAP"
		Me.ButtonMAP.Size = New Size(24, 24)
		Me.ButtonMAP.TabIndex = 5
		Me.ButtonMAP.TabStop = False
		Me.ButtonMAP.UseVisualStyleBackColor = True
		'
		'TabControl1
		'
		Me.TabControl1.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TabControl1.Controls.Add(Me.TabPgGen)
		Me.TabControl1.Controls.Add(Me.TabPgDriver)
		Me.TabControl1.Location = New Point(1, 107)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New Size(535, 560)
		Me.TabControl1.SizeMode = TabSizeMode.Fixed
		Me.TabControl1.TabIndex = 0
		'
		'TabPgDriver
		'
		Me.TabPgDriver.Controls.Add(Me.GrVACC)
		Me.TabPgDriver.Controls.Add(Me.GrLAC)
		Me.TabPgDriver.Controls.Add(Me.GroupBox1)
		Me.TabPgDriver.Controls.Add(Me.GrStartStop)
		Me.TabPgDriver.Location = New Point(4, 22)
		Me.TabPgDriver.Name = "TabPgDriver"
		Me.TabPgDriver.Padding = New Padding(3)
		Me.TabPgDriver.Size = New Size(527, 534)
		Me.TabPgDriver.TabIndex = 7
		Me.TabPgDriver.Text = "Driver Assist"
		Me.TabPgDriver.UseVisualStyleBackColor = True
		'
		'GrVACC
		'
		Me.GrVACC.Controls.Add(Me.TbDesMaxFile)
		Me.GrVACC.Controls.Add(Me.BtDesMaxBr)
		Me.GrVACC.Controls.Add(Me.BtAccOpen)
		Me.GrVACC.Location = New Point(6, 459)
		Me.GrVACC.Name = "GrVACC"
		Me.GrVACC.Size = New Size(515, 65)
		Me.GrVACC.TabIndex = 3
		Me.GrVACC.TabStop = False
		Me.GrVACC.Text = "Max. acceleration and brake curves"
		'
		'TbDesMaxFile
		'
		Me.TbDesMaxFile.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbDesMaxFile.Location = New Point(6, 29)
		Me.TbDesMaxFile.Name = "TbDesMaxFile"
		Me.TbDesMaxFile.Size = New Size(440, 20)
		Me.TbDesMaxFile.TabIndex = 0
		'
		'BtDesMaxBr
		'
		Me.BtDesMaxBr.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.BtDesMaxBr.Image = My.Resources.Resources.Open_icon
		Me.BtDesMaxBr.Location = New Point(446, 27)
		Me.BtDesMaxBr.Name = "BtDesMaxBr"
		Me.BtDesMaxBr.Size = New Size(24, 24)
		Me.BtDesMaxBr.TabIndex = 1
		Me.BtDesMaxBr.UseVisualStyleBackColor = True
		'
		'BtAccOpen
		'
		Me.BtAccOpen.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.BtAccOpen.Image = My.Resources.Resources.application_export_icon_small
		Me.BtAccOpen.Location = New Point(469, 27)
		Me.BtAccOpen.Name = "BtAccOpen"
		Me.BtAccOpen.Size = New Size(24, 24)
		Me.BtAccOpen.TabIndex = 2
		Me.BtAccOpen.TabStop = False
		Me.BtAccOpen.UseVisualStyleBackColor = True
		'
		'GrLAC
		'
		Me.GrLAC.Controls.Add(Me.Label12)
		Me.GrLAC.Controls.Add(Me.tbDfCoastingScale)
		Me.GrLAC.Controls.Add(Me.CbLookAhead)
		Me.GrLAC.Controls.Add(Me.Label11)
		Me.GrLAC.Controls.Add(Me.Label3)
		Me.GrLAC.Controls.Add(Me.tbDfCoastingOffset)
		Me.GrLAC.Controls.Add(Me.tbLacDfTargetSpeedFile)
		Me.GrLAC.Controls.Add(Me.Label10)
		Me.GrLAC.Controls.Add(Me.Label4)
		Me.GrLAC.Controls.Add(Me.Label5)
		Me.GrLAC.Controls.Add(Me.btnDfTargetSpeed)
		Me.GrLAC.Controls.Add(Me.tbLacPreviewFactor)
		Me.GrLAC.Controls.Add(Me.tbLacDfVelocityDropFile)
		Me.GrLAC.Location = New Point(7, 290)
		Me.GrLAC.Name = "GrLAC"
		Me.GrLAC.Size = New Size(514, 163)
		Me.GrLAC.TabIndex = 2
		Me.GrLAC.TabStop = False
		Me.GrLAC.Text = "Look-Ahead Coasting"
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New Point(279, 133)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New Size(130, 13)
		Me.Label12.TabIndex = 17
		Me.Label12.Text = "* DF_vTarget * DF_vDrop"
		'
		'tbDfCoastingScale
		'
		Me.tbDfCoastingScale.Location = New Point(236, 130)
		Me.tbDfCoastingScale.Name = "tbDfCoastingScale"
		Me.tbDfCoastingScale.Size = New Size(37, 20)
		Me.tbDfCoastingScale.TabIndex = 16
		'
		'CbLookAhead
		'
		Me.CbLookAhead.AutoSize = True
		Me.CbLookAhead.Checked = True
		Me.CbLookAhead.CheckState = CheckState.Checked
		Me.CbLookAhead.Location = New Point(16, 21)
		Me.CbLookAhead.Name = "CbLookAhead"
		Me.CbLookAhead.Size = New Size(65, 17)
		Me.CbLookAhead.TabIndex = 0
		Me.CbLookAhead.Text = "Enabled"
		Me.CbLookAhead.UseVisualStyleBackColor = True
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New Point(219, 132)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New Size(13, 13)
		Me.Label11.TabIndex = 15
		Me.Label11.Text = "- "
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(50, 54)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(118, 13)
		Me.Label3.TabIndex = 4
		Me.Label3.Text = "Preview distance factor"
		Me.Label3.TextAlign = ContentAlignment.MiddleRight
		'
		'tbDfCoastingOffset
		'
		Me.tbDfCoastingOffset.Location = New Point(175, 130)
		Me.tbDfCoastingOffset.Name = "tbDfCoastingOffset"
		Me.tbDfCoastingOffset.Size = New Size(37, 20)
		Me.tbDfCoastingOffset.TabIndex = 14
		'
		'tbLacDfTargetSpeedFile
		'
		Me.tbLacDfTargetSpeedFile.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.tbLacDfTargetSpeedFile.Location = New Point(174, 77)
		Me.tbLacDfTargetSpeedFile.Name = "tbLacDfTargetSpeedFile"
		Me.tbLacDfTargetSpeedFile.Size = New Size(264, 20)
		Me.tbLacDfTargetSpeedFile.TabIndex = 6
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New Point(89, 133)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New Size(79, 13)
		Me.Label10.TabIndex = 12
		Me.Label10.Text = "DF_coasting = "
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(13, 80)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(155, 13)
		Me.Label4.TabIndex = 8
		Me.Label4.Text = "Decision Factor - Target Speed"
		Me.Label4.TextAlign = ContentAlignment.MiddleRight
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(15, 107)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(153, 13)
		Me.Label5.TabIndex = 11
		Me.Label5.Text = "Decision Factor - Velocity Drop"
		Me.Label5.TextAlign = ContentAlignment.MiddleRight
		'
		'btnDfTargetSpeed
		'
		Me.btnDfTargetSpeed.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.btnDfTargetSpeed.Image = CType(resources.GetObject("btnDfTargetSpeed.Image"), Image)
		Me.btnDfTargetSpeed.Location = New Point(438, 76)
		Me.btnDfTargetSpeed.Name = "btnDfTargetSpeed"
		Me.btnDfTargetSpeed.Size = New Size(24, 24)
		Me.btnDfTargetSpeed.TabIndex = 7
		Me.btnDfTargetSpeed.TabStop = False
		Me.btnDfTargetSpeed.UseVisualStyleBackColor = True
		'
		'tbLacPreviewFactor
		'
		Me.tbLacPreviewFactor.Location = New Point(174, 51)
		Me.tbLacPreviewFactor.Name = "tbLacPreviewFactor"
		Me.tbLacPreviewFactor.Size = New Size(64, 20)
		Me.tbLacPreviewFactor.TabIndex = 5
		'
		'tbLacDfVelocityDropFile
		'
		Me.tbLacDfVelocityDropFile.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.tbLacDfVelocityDropFile.Location = New Point(174, 104)
		Me.tbLacDfVelocityDropFile.Name = "tbLacDfVelocityDropFile"
		Me.tbLacDfVelocityDropFile.Size = New Size(264, 20)
		Me.tbLacDfVelocityDropFile.TabIndex = 9
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.PnEcoRoll)
		Me.GroupBox1.Controls.Add(Me.RdEcoRoll)
		Me.GroupBox1.Controls.Add(Me.RdOverspeed)
		Me.GroupBox1.Controls.Add(Me.RdOff)
		Me.GroupBox1.Location = New Point(6, 149)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(515, 135)
		Me.GroupBox1.TabIndex = 1
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Overspeed / Eco-Roll"
		'
		'PnEcoRoll
		'
		Me.PnEcoRoll.Controls.Add(Me.Label21)
		Me.PnEcoRoll.Controls.Add(Me.Label20)
		Me.PnEcoRoll.Controls.Add(Me.Label14)
		Me.PnEcoRoll.Controls.Add(Me.TbVmin)
		Me.PnEcoRoll.Controls.Add(Me.TbUnderSpeed)
		Me.PnEcoRoll.Controls.Add(Me.TbOverspeed)
		Me.PnEcoRoll.Controls.Add(Me.Label23)
		Me.PnEcoRoll.Controls.Add(Me.Label22)
		Me.PnEcoRoll.Controls.Add(Me.Label13)
		Me.PnEcoRoll.Location = New Point(137, 16)
		Me.PnEcoRoll.Name = "PnEcoRoll"
		Me.PnEcoRoll.Size = New Size(232, 102)
		Me.PnEcoRoll.TabIndex = 3
		'
		'Label21
		'
		Me.Label21.AutoSize = True
		Me.Label21.Location = New Point(178, 61)
		Me.Label21.Name = "Label21"
		Me.Label21.Size = New Size(38, 13)
		Me.Label21.TabIndex = 3
		Me.Label21.Text = "[km/h]"
		'
		'Label20
		'
		Me.Label20.AutoSize = True
		Me.Label20.Location = New Point(178, 35)
		Me.Label20.Name = "Label20"
		Me.Label20.Size = New Size(38, 13)
		Me.Label20.TabIndex = 3
		Me.Label20.Text = "[km/h]"
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New Point(178, 9)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New Size(38, 13)
		Me.Label14.TabIndex = 3
		Me.Label14.Text = "[km/h]"
		'
		'TbVmin
		'
		Me.TbVmin.Location = New Point(108, 58)
		Me.TbVmin.Name = "TbVmin"
		Me.TbVmin.Size = New Size(64, 20)
		Me.TbVmin.TabIndex = 2
		'
		'TbUnderSpeed
		'
		Me.TbUnderSpeed.Location = New Point(108, 32)
		Me.TbUnderSpeed.Name = "TbUnderSpeed"
		Me.TbUnderSpeed.Size = New Size(64, 20)
		Me.TbUnderSpeed.TabIndex = 1
		'
		'TbOverspeed
		'
		Me.TbOverspeed.Location = New Point(108, 6)
		Me.TbOverspeed.Name = "TbOverspeed"
		Me.TbOverspeed.Size = New Size(64, 20)
		Me.TbOverspeed.TabIndex = 0
		'
		'Label23
		'
		Me.Label23.AutoSize = True
		Me.Label23.Location = New Point(22, 61)
		Me.Label23.Name = "Label23"
		Me.Label23.Size = New Size(80, 13)
		Me.Label23.TabIndex = 1
		Me.Label23.Text = "Minimum speed"
		'
		'Label22
		'
		Me.Label22.AutoSize = True
		Me.Label22.Location = New Point(11, 35)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New Size(91, 13)
		Me.Label22.TabIndex = 1
		Me.Label22.Text = "Max. Underspeed"
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New Point(17, 9)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New Size(85, 13)
		Me.Label13.TabIndex = 1
		Me.Label13.Text = "Max. Overspeed"
		'
		'RdEcoRoll
		'
		Me.RdEcoRoll.AutoSize = True
		Me.RdEcoRoll.Checked = True
		Me.RdEcoRoll.Location = New Point(13, 68)
		Me.RdEcoRoll.Name = "RdEcoRoll"
		Me.RdEcoRoll.Size = New Size(65, 17)
		Me.RdEcoRoll.TabIndex = 2
		Me.RdEcoRoll.TabStop = True
		Me.RdEcoRoll.Text = "Eco-Roll"
		Me.RdEcoRoll.UseVisualStyleBackColor = True
		'
		'RdOverspeed
		'
		Me.RdOverspeed.AutoSize = True
		Me.RdOverspeed.Location = New Point(13, 45)
		Me.RdOverspeed.Name = "RdOverspeed"
		Me.RdOverspeed.Size = New Size(77, 17)
		Me.RdOverspeed.TabIndex = 1
		Me.RdOverspeed.Text = "Overspeed"
		Me.RdOverspeed.UseVisualStyleBackColor = True
		'
		'RdOff
		'
		Me.RdOff.AutoSize = True
		Me.RdOff.Location = New Point(13, 22)
		Me.RdOff.Name = "RdOff"
		Me.RdOff.Size = New Size(39, 17)
		Me.RdOff.TabIndex = 0
		Me.RdOff.Text = "Off"
		Me.RdOff.UseVisualStyleBackColor = True
		'
		'GrStartStop
		'
		Me.GrStartStop.Controls.Add(Me.PnStartStop)
		Me.GrStartStop.Controls.Add(Me.ChBStartStop)
		Me.GrStartStop.Location = New Point(6, 6)
		Me.GrStartStop.Name = "GrStartStop"
		Me.GrStartStop.Size = New Size(515, 137)
		Me.GrStartStop.TabIndex = 0
		Me.GrStartStop.TabStop = False
		Me.GrStartStop.Text = "Engine Start Stop"
		'
		'PnStartStop
		'
		Me.PnStartStop.Controls.Add(Me.Label31)
		Me.PnStartStop.Controls.Add(Me.Label27)
		Me.PnStartStop.Controls.Add(Me.TbSSspeed)
		Me.PnStartStop.Controls.Add(Me.LabelSSspeed)
		Me.PnStartStop.Controls.Add(Me.Label26)
		Me.PnStartStop.Controls.Add(Me.Label30)
		Me.PnStartStop.Controls.Add(Me.LabelSStime)
		Me.PnStartStop.Controls.Add(Me.TbSSdelay)
		Me.PnStartStop.Controls.Add(Me.TbSStime)
		Me.PnStartStop.Location = New Point(87, 21)
		Me.PnStartStop.Name = "PnStartStop"
		Me.PnStartStop.Size = New Size(422, 95)
		Me.PnStartStop.TabIndex = 1
		'
		'Label31
		'
		Me.Label31.AutoSize = True
		Me.Label31.Location = New Point(228, 58)
		Me.Label31.Name = "Label31"
		Me.Label31.Size = New Size(18, 13)
		Me.Label31.TabIndex = 38
		Me.Label31.Text = "[s]"
		'
		'Label27
		'
		Me.Label27.AutoSize = True
		Me.Label27.Location = New Point(228, 32)
		Me.Label27.Name = "Label27"
		Me.Label27.Size = New Size(18, 13)
		Me.Label27.TabIndex = 38
		Me.Label27.Text = "[s]"
		'
		'TbSSspeed
		'
		Me.TbSSspeed.Location = New Point(158, 3)
		Me.TbSSspeed.Name = "TbSSspeed"
		Me.TbSSspeed.Size = New Size(64, 20)
		Me.TbSSspeed.TabIndex = 0
		'
		'LabelSSspeed
		'
		Me.LabelSSspeed.AutoSize = True
		Me.LabelSSspeed.Location = New Point(91, 6)
		Me.LabelSSspeed.Name = "LabelSSspeed"
		Me.LabelSSspeed.Size = New Size(61, 13)
		Me.LabelSSspeed.TabIndex = 37
		Me.LabelSSspeed.Text = "Max Speed"
		'
		'Label26
		'
		Me.Label26.AutoSize = True
		Me.Label26.Location = New Point(228, 6)
		Me.Label26.Name = "Label26"
		Me.Label26.Size = New Size(38, 13)
		Me.Label26.TabIndex = 38
		Me.Label26.Text = "[km/h]"
		'
		'Label30
		'
		Me.Label30.AutoSize = True
		Me.Label30.Location = New Point(68, 58)
		Me.Label30.Name = "Label30"
		Me.Label30.Size = New Size(84, 13)
		Me.Label30.TabIndex = 35
		Me.Label30.Text = "Activation Delay"
		'
		'LabelSStime
		'
		Me.LabelSStime.AutoSize = True
		Me.LabelSStime.Location = New Point(65, 32)
		Me.LabelSStime.Name = "LabelSStime"
		Me.LabelSStime.Size = New Size(87, 13)
		Me.LabelSStime.TabIndex = 35
		Me.LabelSStime.Text = "Min ICE-On Time"
		'
		'TbSSdelay
		'
		Me.TbSSdelay.Location = New Point(158, 55)
		Me.TbSSdelay.Name = "TbSSdelay"
		Me.TbSSdelay.Size = New Size(64, 20)
		Me.TbSSdelay.TabIndex = 2
		'
		'TbSStime
		'
		Me.TbSStime.Location = New Point(158, 29)
		Me.TbSStime.Name = "TbSStime"
		Me.TbSStime.Size = New Size(64, 20)
		Me.TbSStime.TabIndex = 1
		'
		'ChBStartStop
		'
		Me.ChBStartStop.AutoSize = True
		Me.ChBStartStop.Checked = True
		Me.ChBStartStop.CheckState = CheckState.Checked
		Me.ChBStartStop.Location = New Point(16, 21)
		Me.ChBStartStop.Name = "ChBStartStop"
		Me.ChBStartStop.Size = New Size(65, 17)
		Me.ChBStartStop.TabIndex = 0
		Me.ChBStartStop.Text = "Enabled"
		Me.ChBStartStop.UseVisualStyleBackColor = True
		'
		'StatusStrip1
		'
		Me.StatusStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripStatusLabelGEN})
		Me.StatusStrip1.Location = New Point(0, 672)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New Size(944, 22)
		Me.StatusStrip1.SizingGrip = False
		Me.StatusStrip1.TabIndex = 6
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'ToolStripStatusLabelGEN
		'
		Me.ToolStripStatusLabelGEN.Name = "ToolStripStatusLabelGEN"
		Me.ToolStripStatusLabelGEN.Size = New Size(121, 17)
		Me.ToolStripStatusLabelGEN.Text = "ToolStripStatusLabel1"
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButOK.Location = New Point(779, 646)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New Size(75, 23)
		Me.ButOK.TabIndex = 0
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButCancel.DialogResult = DialogResult.Cancel
		Me.ButCancel.Location = New Point(860, 646)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New Size(75, 23)
		Me.ButCancel.TabIndex = 1
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator1, Me.ToolStripBtSendTo, Me.ToolStripSeparator2, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New Size(944, 25)
		Me.ToolStrip1.TabIndex = 20
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtNew
		'
		Me.ToolStripBtNew.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtNew.Image = My.Resources.Resources.blue_document_icon
		Me.ToolStripBtNew.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtNew.Name = "ToolStripBtNew"
		Me.ToolStripBtNew.Size = New Size(23, 22)
		Me.ToolStripBtNew.Text = "New"
		Me.ToolStripBtNew.ToolTipText = "New"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New Size(23, 22)
		Me.ToolStripBtOpen.Text = "Open"
		Me.ToolStripBtOpen.ToolTipText = "Open..."
		'
		'ToolStripBtSave
		'
		Me.ToolStripBtSave.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSave.Image = My.Resources.Resources.Actions_document_save_icon
		Me.ToolStripBtSave.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtSave.Name = "ToolStripBtSave"
		Me.ToolStripBtSave.Size = New Size(23, 22)
		Me.ToolStripBtSave.Text = "Save"
		Me.ToolStripBtSave.ToolTipText = "Save"
		'
		'ToolStripBtSaveAs
		'
		Me.ToolStripBtSaveAs.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSaveAs.Image = My.Resources.Resources.Actions_document_save_as_icon
		Me.ToolStripBtSaveAs.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
		Me.ToolStripBtSaveAs.Size = New Size(23, 22)
		Me.ToolStripBtSaveAs.Text = "Save As"
		Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New Size(6, 25)
		'
		'ToolStripBtSendTo
		'
		Me.ToolStripBtSendTo.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSendTo.Image = My.Resources.Resources.export_icon
		Me.ToolStripBtSendTo.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
		Me.ToolStripBtSendTo.Size = New Size(23, 22)
		Me.ToolStripBtSendTo.Text = "Send to Job List"
		Me.ToolStripBtSendTo.ToolTipText = "Send to Job List"
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New Size(6, 25)
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
		'PictureBox1
		'
		Me.PictureBox1.BackColor = Color.White
		Me.PictureBox1.Image = My.Resources.Resources.VECTO_VECTO
		Me.PictureBox1.Location = New Point(12, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(920, 40)
		Me.PictureBox1.TabIndex = 21
		Me.PictureBox1.TabStop = False
		'
		'CbEngOnly
		'
		Me.CbEngOnly.AutoSize = True
		Me.CbEngOnly.Location = New Point(17, 84)
		Me.CbEngOnly.Name = "CbEngOnly"
		Me.CbEngOnly.Size = New Size(113, 17)
		Me.CbEngOnly.TabIndex = 0
		Me.CbEngOnly.Text = "Engine Only Mode"
		Me.CbEngOnly.UseVisualStyleBackColor = True
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
		'PicVehicle
		'
		Me.PicVehicle.BackColor = Color.LightGray
		Me.PicVehicle.Location = New Point(542, 122)
		Me.PicVehicle.Name = "PicVehicle"
		Me.PicVehicle.Size = New Size(300, 88)
		Me.PicVehicle.SizeMode = PictureBoxSizeMode.StretchImage
		Me.PicVehicle.TabIndex = 36
		Me.PicVehicle.TabStop = False
		'
		'PicBox
		'
		Me.PicBox.BackColor = Color.LightGray
		Me.PicBox.Location = New Point(542, 268)
		Me.PicBox.Name = "PicBox"
		Me.PicBox.Size = New Size(390, 327)
		Me.PicBox.TabIndex = 36
		Me.PicBox.TabStop = False
		'
		'TbEngTxt
		'
		Me.TbEngTxt.Location = New Point(542, 216)
		Me.TbEngTxt.Name = "TbEngTxt"
		Me.TbEngTxt.ReadOnly = True
		Me.TbEngTxt.Size = New Size(390, 20)
		Me.TbEngTxt.TabIndex = 6
		'
		'TbVehCat
		'
		Me.TbVehCat.Location = New Point(848, 126)
		Me.TbVehCat.Name = "TbVehCat"
		Me.TbVehCat.ReadOnly = True
		Me.TbVehCat.Size = New Size(87, 20)
		Me.TbVehCat.TabIndex = 2
		'
		'TbAxleConf
		'
		Me.TbAxleConf.Location = New Point(904, 155)
		Me.TbAxleConf.Name = "TbAxleConf"
		Me.TbAxleConf.ReadOnly = True
		Me.TbAxleConf.Size = New Size(31, 20)
		Me.TbAxleConf.TabIndex = 4
		'
		'TbHVCclass
		'
		Me.TbHVCclass.Location = New Point(848, 184)
		Me.TbHVCclass.Name = "TbHVCclass"
		Me.TbHVCclass.ReadOnly = True
		Me.TbHVCclass.Size = New Size(87, 20)
		Me.TbHVCclass.TabIndex = 5
		'
		'TbGbxTxt
		'
		Me.TbGbxTxt.Location = New Point(542, 242)
		Me.TbGbxTxt.Name = "TbGbxTxt"
		Me.TbGbxTxt.ReadOnly = True
		Me.TbGbxTxt.Size = New Size(390, 20)
		Me.TbGbxTxt.TabIndex = 7
		'
		'TbMass
		'
		Me.TbMass.Location = New Point(848, 155)
		Me.TbMass.Name = "TbMass"
		Me.TbMass.ReadOnly = True
		Me.TbMass.Size = New Size(50, 20)
		Me.TbMass.TabIndex = 3
		'
		'F_VECTO
		'
		Me.AcceptButton = Me.ButOK
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New Size(944, 694)
		Me.Controls.Add(Me.TbHVCclass)
		Me.Controls.Add(Me.TbMass)
		Me.Controls.Add(Me.TbAxleConf)
		Me.Controls.Add(Me.TbVehCat)
		Me.Controls.Add(Me.TbGbxTxt)
		Me.Controls.Add(Me.TbEngTxt)
		Me.Controls.Add(Me.PicBox)
		Me.Controls.Add(Me.PicVehicle)
		Me.Controls.Add(Me.CbEngOnly)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Controls.Add(Me.ButCancel)
		Me.Controls.Add(Me.TabControl1)
		Me.Controls.Add(Me.ButOK)
		Me.Controls.Add(Me.StatusStrip1)
		Me.FormBorderStyle = FormBorderStyle.FixedSingle
		Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Me.MaximizeBox = False
		Me.Name = "F_VECTO"
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Job Editor"
		Me.TabPgGen.ResumeLayout(False)
		Me.TabPgGen.PerformLayout()
		Me.GrCycles.ResumeLayout(False)
		Me.GrCycles.PerformLayout()
		Me.GrAux.ResumeLayout(False)
		Me.GrAux.PerformLayout()
		CType(Me.picAuxInfo, ISupportInitialize).EndInit()
		Me.TabControl1.ResumeLayout(False)
		Me.TabPgDriver.ResumeLayout(False)
		Me.GrVACC.ResumeLayout(False)
		Me.GrVACC.PerformLayout()
		Me.GrLAC.ResumeLayout(False)
		Me.GrLAC.PerformLayout()
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.PnEcoRoll.ResumeLayout(False)
		Me.PnEcoRoll.PerformLayout()
		Me.GrStartStop.ResumeLayout(False)
		Me.GrStartStop.PerformLayout()
		Me.PnStartStop.ResumeLayout(False)
		Me.PnStartStop.PerformLayout()
		Me.StatusStrip1.ResumeLayout(False)
		Me.StatusStrip1.PerformLayout()
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		Me.CmOpenFile.ResumeLayout(False)
		CType(Me.PicVehicle, ISupportInitialize).EndInit()
		CType(Me.PicBox, ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents TabPgGen As TabPage
	Friend WithEvents TabControl1 As TabControl
	Friend WithEvents StatusStrip1 As StatusStrip
	Friend WithEvents ButtonVEH As Button
	Friend WithEvents ButtonMAP As Button
	Friend WithEvents ButtonGBX As Button
	Friend WithEvents ButOpenVEH As Button
	Friend WithEvents ButOpenGBX As Button
	Friend WithEvents ButOpenENG As Button
	Friend WithEvents ToolStripStatusLabelGEN As ToolStripStatusLabel
	Friend WithEvents ButOK As Button
	Friend WithEvents TbGBX As TextBox
	Friend WithEvents TbENG As TextBox
	Friend WithEvents TbVEH As TextBox
	Friend WithEvents ButCancel As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripBtSave As ToolStripButton
	Friend WithEvents ToolStripBtSaveAs As ToolStripButton
	Friend WithEvents ToolStripBtSendTo As ToolStripButton
	Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
	Friend WithEvents GrAux As GroupBox
	Friend WithEvents LvAux As ListView
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents ColumnHeader5 As ColumnHeader
	Friend WithEvents ColumnHeader6 As ColumnHeader
	Friend WithEvents ButAuxRem As Button
	Friend WithEvents ButAuxAdd As Button
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents TabPgDriver As TabPage
	Friend WithEvents BtDesMaxBr As Button
	Friend WithEvents TbDesMaxFile As TextBox
	Friend WithEvents GrCycles As GroupBox
	Friend WithEvents LvCycles As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents BtDRIrem As Button
	Friend WithEvents BtDRIadd As Button
	Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
	Friend WithEvents ToolStripButton1 As ToolStripButton
	Friend WithEvents CbEngOnly As CheckBox
	Friend WithEvents BtAccOpen As Button
	Friend WithEvents Label2 As Label
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ChBStartStop As CheckBox
	Friend WithEvents GrStartStop As GroupBox
	Friend WithEvents TbSSspeed As TextBox
	Friend WithEvents TbSStime As TextBox
	Friend WithEvents LabelSStime As Label
	Friend WithEvents LabelSSspeed As Label
	Friend WithEvents GrLAC As GroupBox
	Friend WithEvents CbLookAhead As CheckBox
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents Label21 As Label
	Friend WithEvents Label20 As Label
	Friend WithEvents Label14 As Label
	Friend WithEvents TbVmin As TextBox
	Friend WithEvents TbUnderSpeed As TextBox
	Friend WithEvents TbOverspeed As TextBox
	Friend WithEvents Label23 As Label
	Friend WithEvents Label22 As Label
	Friend WithEvents Label13 As Label
	Friend WithEvents RdEcoRoll As RadioButton
	Friend WithEvents RdOverspeed As RadioButton
	Friend WithEvents RdOff As RadioButton
	Friend WithEvents PnStartStop As Panel
	Friend WithEvents Label27 As Label
	Friend WithEvents Label26 As Label
	Friend WithEvents Label31 As Label
	Friend WithEvents Label30 As Label
	Friend WithEvents TbSSdelay As TextBox
	Friend WithEvents Label32 As Label
	Friend WithEvents PnEcoRoll As Panel
	Friend WithEvents PicVehicle As PictureBox
	Friend WithEvents PicBox As PictureBox
	Friend WithEvents TbEngTxt As TextBox
	Friend WithEvents TbVehCat As TextBox
	Friend WithEvents TbAxleConf As TextBox
	Friend WithEvents TbHVCclass As TextBox
	Friend WithEvents TbGbxTxt As TextBox
	Friend WithEvents TbMass As TextBox
	Friend WithEvents GrVACC As GroupBox
	Friend WithEvents cboAdvancedAuxiliaries As ComboBox
	Friend WithEvents picAuxInfo As PictureBox
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents Label1 As Label
	Friend WithEvents btnBrowseAAUXFile As Button
	Friend WithEvents txtAdvancedAuxiliaryFile As TextBox
	Friend WithEvents lbAdvancedAuxiliaries As Label
	Friend WithEvents btnAAUXOpen As Button
	Friend WithEvents Label12 As Label
	Friend WithEvents tbDfCoastingScale As TextBox
	Friend WithEvents Label11 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents tbDfCoastingOffset As TextBox
	Friend WithEvents tbLacDfTargetSpeedFile As TextBox
	Friend WithEvents Label10 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents Label5 As Label
	Friend WithEvents btnDfTargetSpeed As Button
	Friend WithEvents tbLacPreviewFactor As TextBox
	Friend WithEvents tbLacDfVelocityDropFile As TextBox
End Class
