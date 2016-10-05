Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class VectoJobForm
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
		Me.components = New System.ComponentModel.Container()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VectoJobForm))
		Me.TabPgGen = New System.Windows.Forms.TabPage()
		Me.GrCycles = New System.Windows.Forms.GroupBox()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.LvCycles = New System.Windows.Forms.ListView()
		Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.BtDRIrem = New System.Windows.Forms.Button()
		Me.BtDRIadd = New System.Windows.Forms.Button()
		Me.GrAux = New System.Windows.Forms.GroupBox()
		Me.Label9 = New System.Windows.Forms.Label()
		Me.TbAuxPAdd = New System.Windows.Forms.TextBox()
		Me.Label8 = New System.Windows.Forms.Label()
		Me.btnAAUXOpen = New System.Windows.Forms.Button()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.btnBrowseAAUXFile = New System.Windows.Forms.Button()
		Me.txtAdvancedAuxiliaryFile = New System.Windows.Forms.TextBox()
		Me.picAuxInfo = New System.Windows.Forms.PictureBox()
		Me.cboAdvancedAuxiliaries = New System.Windows.Forms.ComboBox()
		Me.lbAdvancedAuxiliaries = New System.Windows.Forms.Label()
		Me.Label32 = New System.Windows.Forms.Label()
		Me.LvAux = New System.Windows.Forms.ListView()
		Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
		Me.ButAuxRem = New System.Windows.Forms.Button()
		Me.ButAuxAdd = New System.Windows.Forms.Button()
		Me.TbGBX = New System.Windows.Forms.TextBox()
		Me.TbENG = New System.Windows.Forms.TextBox()
		Me.TbVEH = New System.Windows.Forms.TextBox()
		Me.ButOpenGBX = New System.Windows.Forms.Button()
		Me.ButOpenENG = New System.Windows.Forms.Button()
		Me.ButOpenVEH = New System.Windows.Forms.Button()
		Me.ButtonVEH = New System.Windows.Forms.Button()
		Me.ButtonGBX = New System.Windows.Forms.Button()
		Me.ButtonMAP = New System.Windows.Forms.Button()
		Me.TabControl1 = New System.Windows.Forms.TabControl()
		Me.TabPgDriver = New System.Windows.Forms.TabPage()
		Me.GrVACC = New System.Windows.Forms.GroupBox()
		Me.TbDesMaxFile = New System.Windows.Forms.TextBox()
		Me.BtDesMaxBr = New System.Windows.Forms.Button()
		Me.BtAccOpen = New System.Windows.Forms.Button()
		Me.GrLAC = New System.Windows.Forms.GroupBox()
		Me.pnLookAheadCoasting = New System.Windows.Forms.Panel()
		Me.Label7 = New System.Windows.Forms.Label()
		Me.Label6 = New System.Windows.Forms.Label()
		Me.tbLacMinSpeed = New System.Windows.Forms.TextBox()
		Me.btnDfVelocityDrop = New System.Windows.Forms.Button()
		Me.Label12 = New System.Windows.Forms.Label()
		Me.tbDfCoastingScale = New System.Windows.Forms.TextBox()
		Me.Label11 = New System.Windows.Forms.Label()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.tbDfCoastingOffset = New System.Windows.Forms.TextBox()
		Me.tbLacDfTargetSpeedFile = New System.Windows.Forms.TextBox()
		Me.Label10 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.btnDfTargetSpeed = New System.Windows.Forms.Button()
		Me.tbLacPreviewFactor = New System.Windows.Forms.TextBox()
		Me.tbLacDfVelocityDropFile = New System.Windows.Forms.TextBox()
		Me.CbLookAhead = New System.Windows.Forms.CheckBox()
		Me.GroupBox1 = New System.Windows.Forms.GroupBox()
		Me.PnEcoRoll = New System.Windows.Forms.Panel()
		Me.Label21 = New System.Windows.Forms.Label()
		Me.Label20 = New System.Windows.Forms.Label()
		Me.Label14 = New System.Windows.Forms.Label()
		Me.TbVmin = New System.Windows.Forms.TextBox()
		Me.TbUnderSpeed = New System.Windows.Forms.TextBox()
		Me.TbOverspeed = New System.Windows.Forms.TextBox()
		Me.Label23 = New System.Windows.Forms.Label()
		Me.Label22 = New System.Windows.Forms.Label()
		Me.Label13 = New System.Windows.Forms.Label()
		Me.RdEcoRoll = New System.Windows.Forms.RadioButton()
		Me.RdOverspeed = New System.Windows.Forms.RadioButton()
		Me.RdOff = New System.Windows.Forms.RadioButton()
		Me.GrStartStop = New System.Windows.Forms.GroupBox()
		Me.PnStartStop = New System.Windows.Forms.Panel()
		Me.Label31 = New System.Windows.Forms.Label()
		Me.Label27 = New System.Windows.Forms.Label()
		Me.TbSSspeed = New System.Windows.Forms.TextBox()
		Me.LabelSSspeed = New System.Windows.Forms.Label()
		Me.Label26 = New System.Windows.Forms.Label()
		Me.Label30 = New System.Windows.Forms.Label()
		Me.LabelSStime = New System.Windows.Forms.Label()
		Me.TbSSdelay = New System.Windows.Forms.TextBox()
		Me.TbSStime = New System.Windows.Forms.TextBox()
		Me.ChBStartStop = New System.Windows.Forms.CheckBox()
		Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
		Me.ToolStripStatusLabelGEN = New System.Windows.Forms.ToolStripStatusLabel()
		Me.ButOK = New System.Windows.Forms.Button()
		Me.ButCancel = New System.Windows.Forms.Button()
		Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
		Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtSave = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripBtSaveAs = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripBtSendTo = New System.Windows.Forms.ToolStripButton()
		Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
		Me.ToolStripButton1 = New System.Windows.Forms.ToolStripButton()
		Me.PictureBox1 = New System.Windows.Forms.PictureBox()
		Me.CbEngOnly = New System.Windows.Forms.CheckBox()
		Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.PicVehicle = New System.Windows.Forms.PictureBox()
		Me.PicBox = New System.Windows.Forms.PictureBox()
		Me.TbEngTxt = New System.Windows.Forms.TextBox()
		Me.TbVehCat = New System.Windows.Forms.TextBox()
		Me.TbAxleConf = New System.Windows.Forms.TextBox()
		Me.TbHVCclass = New System.Windows.Forms.TextBox()
		Me.TbGbxTxt = New System.Windows.Forms.TextBox()
		Me.TbMass = New System.Windows.Forms.TextBox()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
		Me.TabPgGen.SuspendLayout()
		Me.GrCycles.SuspendLayout()
		Me.GrAux.SuspendLayout()
		CType(Me.picAuxInfo, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.TabControl1.SuspendLayout()
		Me.TabPgDriver.SuspendLayout()
		Me.GrVACC.SuspendLayout()
		Me.GrLAC.SuspendLayout()
		Me.pnLookAheadCoasting.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me.PnEcoRoll.SuspendLayout()
		Me.GrStartStop.SuspendLayout()
		Me.PnStartStop.SuspendLayout()
		Me.StatusStrip1.SuspendLayout()
		Me.ToolStrip1.SuspendLayout()
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.CmOpenFile.SuspendLayout()
		CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).BeginInit()
		CType(Me.PicBox, System.ComponentModel.ISupportInitialize).BeginInit()
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
		Me.TabPgGen.Location = New System.Drawing.Point(4, 22)
		Me.TabPgGen.Name = "TabPgGen"
		Me.TabPgGen.Padding = New System.Windows.Forms.Padding(3)
		Me.TabPgGen.Size = New System.Drawing.Size(527, 534)
		Me.TabPgGen.TabIndex = 0
		Me.TabPgGen.Text = "General"
		Me.TabPgGen.UseVisualStyleBackColor = True
		'
		'GrCycles
		'
		Me.GrCycles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.GrCycles.Controls.Add(Me.Label2)
		Me.GrCycles.Controls.Add(Me.LvCycles)
		Me.GrCycles.Controls.Add(Me.BtDRIrem)
		Me.GrCycles.Controls.Add(Me.BtDRIadd)
		Me.GrCycles.Location = New System.Drawing.Point(9, 344)
		Me.GrCycles.Name = "GrCycles"
		Me.GrCycles.Size = New System.Drawing.Size(515, 184)
		Me.GrCycles.TabIndex = 10
		Me.GrCycles.TabStop = False
		Me.GrCycles.Text = "Cycles"
		'
		'Label2
		'
		Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New System.Drawing.Point(379, 148)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(133, 13)
		Me.Label2.TabIndex = 3
		Me.Label2.Text = "(Double-Click to Open File)"
		'
		'LvCycles
		'
		Me.LvCycles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.LvCycles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
		Me.LvCycles.FullRowSelect = True
		Me.LvCycles.GridLines = True
		Me.LvCycles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
		Me.LvCycles.HideSelection = False
		Me.LvCycles.LabelEdit = True
		Me.LvCycles.Location = New System.Drawing.Point(6, 25)
		Me.LvCycles.MultiSelect = False
		Me.LvCycles.Name = "LvCycles"
		Me.LvCycles.Size = New System.Drawing.Size(503, 123)
		Me.LvCycles.TabIndex = 0
		Me.LvCycles.TabStop = False
		Me.LvCycles.UseCompatibleStateImageBehavior = False
		Me.LvCycles.View = System.Windows.Forms.View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Cycle path"
		Me.ColumnHeader1.Width = 470
		'
		'BtDRIrem
		'
		Me.BtDRIrem.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.BtDRIrem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
		Me.BtDRIrem.Location = New System.Drawing.Point(29, 149)
		Me.BtDRIrem.Name = "BtDRIrem"
		Me.BtDRIrem.Size = New System.Drawing.Size(24, 24)
		Me.BtDRIrem.TabIndex = 2
		Me.BtDRIrem.UseVisualStyleBackColor = True
		'
		'BtDRIadd
		'
		Me.BtDRIadd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
		Me.BtDRIadd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
		Me.BtDRIadd.Location = New System.Drawing.Point(5, 149)
		Me.BtDRIadd.Name = "BtDRIadd"
		Me.BtDRIadd.Size = New System.Drawing.Size(24, 24)
		Me.BtDRIadd.TabIndex = 1
		Me.BtDRIadd.UseVisualStyleBackColor = True
		'
		'GrAux
		'
		Me.GrAux.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
			Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.GrAux.Controls.Add(Me.Label9)
		Me.GrAux.Controls.Add(Me.TbAuxPAdd)
		Me.GrAux.Controls.Add(Me.Label8)
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
		Me.GrAux.Location = New System.Drawing.Point(6, 87)
		Me.GrAux.Name = "GrAux"
		Me.GrAux.Size = New System.Drawing.Size(515, 251)
		Me.GrAux.TabIndex = 9
		Me.GrAux.TabStop = False
		Me.GrAux.Text = "Auxiliaries"
		'
		'Label9
		'
		Me.Label9.AutoSize = True
		Me.Label9.Location = New System.Drawing.Point(191, 76)
		Me.Label9.Name = "Label9"
		Me.Label9.Size = New System.Drawing.Size(24, 13)
		Me.Label9.TabIndex = 44
		Me.Label9.Text = "[W]"
		'
		'TbAuxPAdd
		'
		Me.TbAuxPAdd.Location = New System.Drawing.Point(119, 73)
		Me.TbAuxPAdd.Name = "TbAuxPAdd"
		Me.TbAuxPAdd.Size = New System.Drawing.Size(66, 20)
		Me.TbAuxPAdd.TabIndex = 43
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Location = New System.Drawing.Point(7, 76)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New System.Drawing.Size(97, 13)
		Me.Label8.TabIndex = 42
		Me.Label8.Text = "Constant Aux Load"
		'
		'btnAAUXOpen
		'
		Me.btnAAUXOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
		Me.btnAAUXOpen.Location = New System.Drawing.Point(465, 45)
		Me.btnAAUXOpen.Name = "btnAAUXOpen"
		Me.btnAAUXOpen.Size = New System.Drawing.Size(24, 24)
		Me.btnAAUXOpen.TabIndex = 41
		Me.btnAAUXOpen.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New System.Drawing.Point(7, 52)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(96, 13)
		Me.Label1.TabIndex = 40
		Me.Label1.Text = "Advanced Aux File"
		'
		'btnBrowseAAUXFile
		'
		Me.btnBrowseAAUXFile.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
		Me.btnBrowseAAUXFile.Location = New System.Drawing.Point(441, 45)
		Me.btnBrowseAAUXFile.Name = "btnBrowseAAUXFile"
		Me.btnBrowseAAUXFile.Size = New System.Drawing.Size(24, 24)
		Me.btnBrowseAAUXFile.TabIndex = 39
		Me.ToolTip1.SetToolTip(Me.btnBrowseAAUXFile, "Configure/Browser  Advanced Auxiliary Files")
		Me.btnBrowseAAUXFile.UseVisualStyleBackColor = True
		'
		'txtAdvancedAuxiliaryFile
		'
		Me.txtAdvancedAuxiliaryFile.Location = New System.Drawing.Point(119, 47)
		Me.txtAdvancedAuxiliaryFile.Name = "txtAdvancedAuxiliaryFile"
		Me.txtAdvancedAuxiliaryFile.Size = New System.Drawing.Size(321, 20)
		Me.txtAdvancedAuxiliaryFile.TabIndex = 38
		'
		'picAuxInfo
		'
		Me.picAuxInfo.Image = Global.TUGraz.VECTO.My.Resources.Resources.Information_icon
		Me.picAuxInfo.InitialImage = Global.TUGraz.VECTO.My.Resources.Resources.Information_icon
		Me.picAuxInfo.Location = New System.Drawing.Point(451, 19)
		Me.picAuxInfo.Name = "picAuxInfo"
		Me.picAuxInfo.Size = New System.Drawing.Size(16, 16)
		Me.picAuxInfo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
		Me.picAuxInfo.TabIndex = 37
		Me.picAuxInfo.TabStop = False
		'
		'cboAdvancedAuxiliaries
		'
		Me.cboAdvancedAuxiliaries.FormattingEnabled = True
		Me.cboAdvancedAuxiliaries.Location = New System.Drawing.Point(119, 18)
		Me.cboAdvancedAuxiliaries.Name = "cboAdvancedAuxiliaries"
		Me.cboAdvancedAuxiliaries.Size = New System.Drawing.Size(321, 21)
		Me.cboAdvancedAuxiliaries.TabIndex = 36
		'
		'lbAdvancedAuxiliaries
		'
		Me.lbAdvancedAuxiliaries.AutoSize = True
		Me.lbAdvancedAuxiliaries.Location = New System.Drawing.Point(7, 21)
		Me.lbAdvancedAuxiliaries.Name = "lbAdvancedAuxiliaries"
		Me.lbAdvancedAuxiliaries.Size = New System.Drawing.Size(72, 13)
		Me.lbAdvancedAuxiliaries.TabIndex = 35
		Me.lbAdvancedAuxiliaries.Text = "Auxiliary Type"
		'
		'Label32
		'
		Me.Label32.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Label32.AutoSize = True
		Me.Label32.Location = New System.Drawing.Point(406, 220)
		Me.Label32.Name = "Label32"
		Me.Label32.Size = New System.Drawing.Size(106, 13)
		Me.Label32.TabIndex = 3
		Me.Label32.Text = "(Double-Click to Edit)"
		'
		'LvAux
		'
		Me.LvAux.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.LvAux.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
		Me.LvAux.FullRowSelect = True
		Me.LvAux.GridLines = True
		Me.LvAux.HideSelection = False
		Me.LvAux.Location = New System.Drawing.Point(6, 103)
		Me.LvAux.MultiSelect = False
		Me.LvAux.Name = "LvAux"
		Me.LvAux.Size = New System.Drawing.Size(503, 117)
		Me.LvAux.TabIndex = 0
		Me.LvAux.TabStop = False
		Me.LvAux.UseCompatibleStateImageBehavior = False
		Me.LvAux.View = System.Windows.Forms.View.Details
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
		Me.ButAuxRem.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
		Me.ButAuxRem.Location = New System.Drawing.Point(29, 221)
		Me.ButAuxRem.Name = "ButAuxRem"
		Me.ButAuxRem.Size = New System.Drawing.Size(24, 24)
		Me.ButAuxRem.TabIndex = 2
		Me.ButAuxRem.UseVisualStyleBackColor = True
		'
		'ButAuxAdd
		'
		Me.ButAuxAdd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
		Me.ButAuxAdd.Location = New System.Drawing.Point(5, 221)
		Me.ButAuxAdd.Name = "ButAuxAdd"
		Me.ButAuxAdd.Size = New System.Drawing.Size(24, 24)
		Me.ButAuxAdd.TabIndex = 1
		Me.ButAuxAdd.UseVisualStyleBackColor = True
		'
		'TbGBX
		'
		Me.TbGBX.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbGBX.Location = New System.Drawing.Point(84, 60)
		Me.TbGBX.Name = "TbGBX"
		Me.TbGBX.Size = New System.Drawing.Size(411, 20)
		Me.TbGBX.TabIndex = 7
		'
		'TbENG
		'
		Me.TbENG.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbENG.Location = New System.Drawing.Point(84, 33)
		Me.TbENG.Name = "TbENG"
		Me.TbENG.Size = New System.Drawing.Size(411, 20)
		Me.TbENG.TabIndex = 4
		'
		'TbVEH
		'
		Me.TbVEH.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbVEH.Location = New System.Drawing.Point(84, 7)
		Me.TbVEH.Name = "TbVEH"
		Me.TbVEH.Size = New System.Drawing.Size(411, 20)
		Me.TbVEH.TabIndex = 1
		'
		'ButOpenGBX
		'
		Me.ButOpenGBX.Location = New System.Drawing.Point(6, 60)
		Me.ButOpenGBX.Name = "ButOpenGBX"
		Me.ButOpenGBX.Size = New System.Drawing.Size(72, 21)
		Me.ButOpenGBX.TabIndex = 6
		Me.ButOpenGBX.TabStop = False
		Me.ButOpenGBX.Text = "Gearbox"
		Me.ButOpenGBX.UseVisualStyleBackColor = True
		'
		'ButOpenENG
		'
		Me.ButOpenENG.Location = New System.Drawing.Point(6, 33)
		Me.ButOpenENG.Name = "ButOpenENG"
		Me.ButOpenENG.Size = New System.Drawing.Size(72, 21)
		Me.ButOpenENG.TabIndex = 3
		Me.ButOpenENG.TabStop = False
		Me.ButOpenENG.Text = "Engine"
		Me.ButOpenENG.UseVisualStyleBackColor = True
		'
		'ButOpenVEH
		'
		Me.ButOpenVEH.Location = New System.Drawing.Point(6, 6)
		Me.ButOpenVEH.Name = "ButOpenVEH"
		Me.ButOpenVEH.Size = New System.Drawing.Size(72, 21)
		Me.ButOpenVEH.TabIndex = 0
		Me.ButOpenVEH.TabStop = False
		Me.ButOpenVEH.Text = "Vehicle"
		Me.ButOpenVEH.UseVisualStyleBackColor = True
		'
		'ButtonVEH
		'
		Me.ButtonVEH.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonVEH.Image = CType(resources.GetObject("ButtonVEH.Image"), System.Drawing.Image)
		Me.ButtonVEH.Location = New System.Drawing.Point(496, 5)
		Me.ButtonVEH.Name = "ButtonVEH"
		Me.ButtonVEH.Size = New System.Drawing.Size(24, 24)
		Me.ButtonVEH.TabIndex = 2
		Me.ButtonVEH.TabStop = False
		Me.ButtonVEH.UseVisualStyleBackColor = True
		'
		'ButtonGBX
		'
		Me.ButtonGBX.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonGBX.Image = CType(resources.GetObject("ButtonGBX.Image"), System.Drawing.Image)
		Me.ButtonGBX.Location = New System.Drawing.Point(496, 58)
		Me.ButtonGBX.Name = "ButtonGBX"
		Me.ButtonGBX.Size = New System.Drawing.Size(24, 24)
		Me.ButtonGBX.TabIndex = 8
		Me.ButtonGBX.TabStop = False
		Me.ButtonGBX.UseVisualStyleBackColor = True
		'
		'ButtonMAP
		'
		Me.ButtonMAP.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButtonMAP.Image = CType(resources.GetObject("ButtonMAP.Image"), System.Drawing.Image)
		Me.ButtonMAP.Location = New System.Drawing.Point(496, 31)
		Me.ButtonMAP.Name = "ButtonMAP"
		Me.ButtonMAP.Size = New System.Drawing.Size(24, 24)
		Me.ButtonMAP.TabIndex = 5
		Me.ButtonMAP.TabStop = False
		Me.ButtonMAP.UseVisualStyleBackColor = True
		'
		'TabControl1
		'
		Me.TabControl1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TabControl1.Controls.Add(Me.TabPgGen)
		Me.TabControl1.Controls.Add(Me.TabPgDriver)
		Me.TabControl1.Location = New System.Drawing.Point(1, 107)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New System.Drawing.Size(535, 560)
		Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
		Me.TabControl1.TabIndex = 0
		'
		'TabPgDriver
		'
		Me.TabPgDriver.Controls.Add(Me.GrVACC)
		Me.TabPgDriver.Controls.Add(Me.GrLAC)
		Me.TabPgDriver.Controls.Add(Me.GroupBox1)
		Me.TabPgDriver.Controls.Add(Me.GrStartStop)
		Me.TabPgDriver.Location = New System.Drawing.Point(4, 22)
		Me.TabPgDriver.Name = "TabPgDriver"
		Me.TabPgDriver.Padding = New System.Windows.Forms.Padding(3)
		Me.TabPgDriver.Size = New System.Drawing.Size(527, 534)
		Me.TabPgDriver.TabIndex = 7
		Me.TabPgDriver.Text = "Driver Assist"
		Me.TabPgDriver.UseVisualStyleBackColor = True
		'
		'GrVACC
		'
		Me.GrVACC.Controls.Add(Me.TbDesMaxFile)
		Me.GrVACC.Controls.Add(Me.BtDesMaxBr)
		Me.GrVACC.Controls.Add(Me.BtAccOpen)
		Me.GrVACC.Location = New System.Drawing.Point(6, 459)
		Me.GrVACC.Name = "GrVACC"
		Me.GrVACC.Size = New System.Drawing.Size(515, 65)
		Me.GrVACC.TabIndex = 3
		Me.GrVACC.TabStop = False
		Me.GrVACC.Text = "Max. acceleration and brake curves"
		'
		'TbDesMaxFile
		'
		Me.TbDesMaxFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.TbDesMaxFile.Location = New System.Drawing.Point(6, 29)
		Me.TbDesMaxFile.Name = "TbDesMaxFile"
		Me.TbDesMaxFile.Size = New System.Drawing.Size(433, 20)
		Me.TbDesMaxFile.TabIndex = 0
		'
		'BtDesMaxBr
		'
		Me.BtDesMaxBr.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtDesMaxBr.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
		Me.BtDesMaxBr.Location = New System.Drawing.Point(446, 27)
		Me.BtDesMaxBr.Name = "BtDesMaxBr"
		Me.BtDesMaxBr.Size = New System.Drawing.Size(24, 24)
		Me.BtDesMaxBr.TabIndex = 1
		Me.BtDesMaxBr.UseVisualStyleBackColor = True
		'
		'BtAccOpen
		'
		Me.BtAccOpen.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.BtAccOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.application_export_icon_small
		Me.BtAccOpen.Location = New System.Drawing.Point(469, 27)
		Me.BtAccOpen.Name = "BtAccOpen"
		Me.BtAccOpen.Size = New System.Drawing.Size(24, 24)
		Me.BtAccOpen.TabIndex = 2
		Me.BtAccOpen.TabStop = False
		Me.BtAccOpen.UseVisualStyleBackColor = True
		'
		'GrLAC
		'
		Me.GrLAC.Controls.Add(Me.pnLookAheadCoasting)
		Me.GrLAC.Controls.Add(Me.CbLookAhead)
		Me.GrLAC.Location = New System.Drawing.Point(7, 280)
		Me.GrLAC.Name = "GrLAC"
		Me.GrLAC.Size = New System.Drawing.Size(514, 173)
		Me.GrLAC.TabIndex = 2
		Me.GrLAC.TabStop = False
		Me.GrLAC.Text = "Look-Ahead Coasting"
		'
		'pnLookAheadCoasting
		'
		Me.pnLookAheadCoasting.Controls.Add(Me.Label7)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label6)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacMinSpeed)
		Me.pnLookAheadCoasting.Controls.Add(Me.btnDfVelocityDrop)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label12)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbDfCoastingScale)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label11)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label3)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbDfCoastingOffset)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacDfTargetSpeedFile)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label10)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label4)
		Me.pnLookAheadCoasting.Controls.Add(Me.Label5)
		Me.pnLookAheadCoasting.Controls.Add(Me.btnDfTargetSpeed)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacPreviewFactor)
		Me.pnLookAheadCoasting.Controls.Add(Me.tbLacDfVelocityDropFile)
		Me.pnLookAheadCoasting.Location = New System.Drawing.Point(16, 37)
		Me.pnLookAheadCoasting.Name = "pnLookAheadCoasting"
		Me.pnLookAheadCoasting.Size = New System.Drawing.Size(467, 129)
		Me.pnLookAheadCoasting.TabIndex = 20
		'
		'Label7
		'
		Me.Label7.AutoSize = True
		Me.Label7.Location = New System.Drawing.Point(234, 6)
		Me.Label7.Name = "Label7"
		Me.Label7.Size = New System.Drawing.Size(38, 13)
		Me.Label7.TabIndex = 4
		Me.Label7.Text = "[km/h]"
		'
		'Label6
		'
		Me.Label6.AutoSize = True
		Me.Label6.Location = New System.Drawing.Point(91, 6)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New System.Drawing.Size(67, 13)
		Me.Label6.TabIndex = 33
		Me.Label6.Text = "Min. Velocity"
		Me.Label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'tbLacMinSpeed
		'
		Me.tbLacMinSpeed.Location = New System.Drawing.Point(164, 3)
		Me.tbLacMinSpeed.Name = "tbLacMinSpeed"
		Me.tbLacMinSpeed.Size = New System.Drawing.Size(64, 20)
		Me.tbLacMinSpeed.TabIndex = 34
		'
		'btnDfVelocityDrop
		'
		Me.btnDfVelocityDrop.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.btnDfVelocityDrop.Image = CType(resources.GetObject("btnDfVelocityDrop.Image"), System.Drawing.Image)
		Me.btnDfVelocityDrop.Location = New System.Drawing.Point(435, 78)
		Me.btnDfVelocityDrop.Name = "btnDfVelocityDrop"
		Me.btnDfVelocityDrop.Size = New System.Drawing.Size(24, 24)
		Me.btnDfVelocityDrop.TabIndex = 32
		Me.btnDfVelocityDrop.TabStop = False
		Me.btnDfVelocityDrop.UseVisualStyleBackColor = True
		'
		'Label12
		'
		Me.Label12.AutoSize = True
		Me.Label12.Location = New System.Drawing.Point(269, 110)
		Me.Label12.Name = "Label12"
		Me.Label12.Size = New System.Drawing.Size(130, 13)
		Me.Label12.TabIndex = 31
		Me.Label12.Text = "* DF_vTarget * DF_vDrop"
		'
		'tbDfCoastingScale
		'
		Me.tbDfCoastingScale.Location = New System.Drawing.Point(226, 107)
		Me.tbDfCoastingScale.Name = "tbDfCoastingScale"
		Me.tbDfCoastingScale.Size = New System.Drawing.Size(37, 20)
		Me.tbDfCoastingScale.TabIndex = 30
		'
		'Label11
		'
		Me.Label11.AutoSize = True
		Me.Label11.Location = New System.Drawing.Point(209, 109)
		Me.Label11.Name = "Label11"
		Me.Label11.Size = New System.Drawing.Size(13, 13)
		Me.Label11.TabIndex = 29
		Me.Label11.Text = "- "
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New System.Drawing.Point(40, 31)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(118, 13)
		Me.Label3.TabIndex = 20
		Me.Label3.Text = "Preview distance factor"
		Me.Label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'tbDfCoastingOffset
		'
		Me.tbDfCoastingOffset.Location = New System.Drawing.Point(165, 107)
		Me.tbDfCoastingOffset.Name = "tbDfCoastingOffset"
		Me.tbDfCoastingOffset.Size = New System.Drawing.Size(37, 20)
		Me.tbDfCoastingOffset.TabIndex = 28
		'
		'tbLacDfTargetSpeedFile
		'
		Me.tbLacDfTargetSpeedFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbLacDfTargetSpeedFile.Location = New System.Drawing.Point(164, 54)
		Me.tbLacDfTargetSpeedFile.Name = "tbLacDfTargetSpeedFile"
		Me.tbLacDfTargetSpeedFile.Size = New System.Drawing.Size(264, 20)
		Me.tbLacDfTargetSpeedFile.TabIndex = 22
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Location = New System.Drawing.Point(79, 110)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New System.Drawing.Size(79, 13)
		Me.Label10.TabIndex = 27
		Me.Label10.Text = "DF_coasting = "
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New System.Drawing.Point(3, 57)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(155, 13)
		Me.Label4.TabIndex = 24
		Me.Label4.Text = "Decision Factor - Target Speed"
		Me.Label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'Label5
		'
		Me.Label5.AutoSize = True
		Me.Label5.Location = New System.Drawing.Point(5, 84)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(153, 13)
		Me.Label5.TabIndex = 26
		Me.Label5.Text = "Decision Factor - Velocity Drop"
		Me.Label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight
		'
		'btnDfTargetSpeed
		'
		Me.btnDfTargetSpeed.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.btnDfTargetSpeed.Image = CType(resources.GetObject("btnDfTargetSpeed.Image"), System.Drawing.Image)
		Me.btnDfTargetSpeed.Location = New System.Drawing.Point(434, 54)
		Me.btnDfTargetSpeed.Name = "btnDfTargetSpeed"
		Me.btnDfTargetSpeed.Size = New System.Drawing.Size(24, 24)
		Me.btnDfTargetSpeed.TabIndex = 23
		Me.btnDfTargetSpeed.TabStop = False
		Me.btnDfTargetSpeed.UseVisualStyleBackColor = True
		'
		'tbLacPreviewFactor
		'
		Me.tbLacPreviewFactor.Location = New System.Drawing.Point(164, 28)
		Me.tbLacPreviewFactor.Name = "tbLacPreviewFactor"
		Me.tbLacPreviewFactor.Size = New System.Drawing.Size(64, 20)
		Me.tbLacPreviewFactor.TabIndex = 21
		'
		'tbLacDfVelocityDropFile
		'
		Me.tbLacDfVelocityDropFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
			Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.tbLacDfVelocityDropFile.Location = New System.Drawing.Point(164, 81)
		Me.tbLacDfVelocityDropFile.Name = "tbLacDfVelocityDropFile"
		Me.tbLacDfVelocityDropFile.Size = New System.Drawing.Size(264, 20)
		Me.tbLacDfVelocityDropFile.TabIndex = 25
		'
		'CbLookAhead
		'
		Me.CbLookAhead.AutoSize = True
		Me.CbLookAhead.Checked = True
		Me.CbLookAhead.CheckState = System.Windows.Forms.CheckState.Checked
		Me.CbLookAhead.Location = New System.Drawing.Point(16, 21)
		Me.CbLookAhead.Name = "CbLookAhead"
		Me.CbLookAhead.Size = New System.Drawing.Size(65, 17)
		Me.CbLookAhead.TabIndex = 0
		Me.CbLookAhead.Text = "Enabled"
		Me.CbLookAhead.UseVisualStyleBackColor = True
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.PnEcoRoll)
		Me.GroupBox1.Controls.Add(Me.RdEcoRoll)
		Me.GroupBox1.Controls.Add(Me.RdOverspeed)
		Me.GroupBox1.Controls.Add(Me.RdOff)
		Me.GroupBox1.Location = New System.Drawing.Point(6, 149)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New System.Drawing.Size(515, 125)
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
		Me.PnEcoRoll.Location = New System.Drawing.Point(137, 16)
		Me.PnEcoRoll.Name = "PnEcoRoll"
		Me.PnEcoRoll.Size = New System.Drawing.Size(232, 102)
		Me.PnEcoRoll.TabIndex = 3
		'
		'Label21
		'
		Me.Label21.AutoSize = True
		Me.Label21.Location = New System.Drawing.Point(178, 61)
		Me.Label21.Name = "Label21"
		Me.Label21.Size = New System.Drawing.Size(38, 13)
		Me.Label21.TabIndex = 3
		Me.Label21.Text = "[km/h]"
		'
		'Label20
		'
		Me.Label20.AutoSize = True
		Me.Label20.Location = New System.Drawing.Point(178, 35)
		Me.Label20.Name = "Label20"
		Me.Label20.Size = New System.Drawing.Size(38, 13)
		Me.Label20.TabIndex = 3
		Me.Label20.Text = "[km/h]"
		'
		'Label14
		'
		Me.Label14.AutoSize = True
		Me.Label14.Location = New System.Drawing.Point(178, 9)
		Me.Label14.Name = "Label14"
		Me.Label14.Size = New System.Drawing.Size(38, 13)
		Me.Label14.TabIndex = 3
		Me.Label14.Text = "[km/h]"
		'
		'TbVmin
		'
		Me.TbVmin.Location = New System.Drawing.Point(108, 58)
		Me.TbVmin.Name = "TbVmin"
		Me.TbVmin.Size = New System.Drawing.Size(64, 20)
		Me.TbVmin.TabIndex = 2
		'
		'TbUnderSpeed
		'
		Me.TbUnderSpeed.Location = New System.Drawing.Point(108, 32)
		Me.TbUnderSpeed.Name = "TbUnderSpeed"
		Me.TbUnderSpeed.Size = New System.Drawing.Size(64, 20)
		Me.TbUnderSpeed.TabIndex = 1
		'
		'TbOverspeed
		'
		Me.TbOverspeed.Location = New System.Drawing.Point(108, 6)
		Me.TbOverspeed.Name = "TbOverspeed"
		Me.TbOverspeed.Size = New System.Drawing.Size(64, 20)
		Me.TbOverspeed.TabIndex = 0
		'
		'Label23
		'
		Me.Label23.AutoSize = True
		Me.Label23.Location = New System.Drawing.Point(22, 61)
		Me.Label23.Name = "Label23"
		Me.Label23.Size = New System.Drawing.Size(80, 13)
		Me.Label23.TabIndex = 1
		Me.Label23.Text = "Minimum speed"
		'
		'Label22
		'
		Me.Label22.AutoSize = True
		Me.Label22.Location = New System.Drawing.Point(11, 35)
		Me.Label22.Name = "Label22"
		Me.Label22.Size = New System.Drawing.Size(91, 13)
		Me.Label22.TabIndex = 1
		Me.Label22.Text = "Max. Underspeed"
		'
		'Label13
		'
		Me.Label13.AutoSize = True
		Me.Label13.Location = New System.Drawing.Point(17, 9)
		Me.Label13.Name = "Label13"
		Me.Label13.Size = New System.Drawing.Size(85, 13)
		Me.Label13.TabIndex = 1
		Me.Label13.Text = "Max. Overspeed"
		'
		'RdEcoRoll
		'
		Me.RdEcoRoll.AutoSize = True
		Me.RdEcoRoll.Checked = True
		Me.RdEcoRoll.Location = New System.Drawing.Point(13, 68)
		Me.RdEcoRoll.Name = "RdEcoRoll"
		Me.RdEcoRoll.Size = New System.Drawing.Size(65, 17)
		Me.RdEcoRoll.TabIndex = 2
		Me.RdEcoRoll.TabStop = True
		Me.RdEcoRoll.Text = "Eco-Roll"
		Me.RdEcoRoll.UseVisualStyleBackColor = True
		'
		'RdOverspeed
		'
		Me.RdOverspeed.AutoSize = True
		Me.RdOverspeed.Location = New System.Drawing.Point(13, 45)
		Me.RdOverspeed.Name = "RdOverspeed"
		Me.RdOverspeed.Size = New System.Drawing.Size(77, 17)
		Me.RdOverspeed.TabIndex = 1
		Me.RdOverspeed.Text = "Overspeed"
		Me.RdOverspeed.UseVisualStyleBackColor = True
		'
		'RdOff
		'
		Me.RdOff.AutoSize = True
		Me.RdOff.Location = New System.Drawing.Point(13, 22)
		Me.RdOff.Name = "RdOff"
		Me.RdOff.Size = New System.Drawing.Size(39, 17)
		Me.RdOff.TabIndex = 0
		Me.RdOff.Text = "Off"
		Me.RdOff.UseVisualStyleBackColor = True
		'
		'GrStartStop
		'
		Me.GrStartStop.Controls.Add(Me.PnStartStop)
		Me.GrStartStop.Controls.Add(Me.ChBStartStop)
		Me.GrStartStop.Location = New System.Drawing.Point(6, 6)
		Me.GrStartStop.Name = "GrStartStop"
		Me.GrStartStop.Size = New System.Drawing.Size(515, 137)
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
		Me.PnStartStop.Location = New System.Drawing.Point(87, 21)
		Me.PnStartStop.Name = "PnStartStop"
		Me.PnStartStop.Size = New System.Drawing.Size(422, 95)
		Me.PnStartStop.TabIndex = 1
		'
		'Label31
		'
		Me.Label31.AutoSize = True
		Me.Label31.Location = New System.Drawing.Point(228, 58)
		Me.Label31.Name = "Label31"
		Me.Label31.Size = New System.Drawing.Size(18, 13)
		Me.Label31.TabIndex = 38
		Me.Label31.Text = "[s]"
		'
		'Label27
		'
		Me.Label27.AutoSize = True
		Me.Label27.Location = New System.Drawing.Point(228, 32)
		Me.Label27.Name = "Label27"
		Me.Label27.Size = New System.Drawing.Size(18, 13)
		Me.Label27.TabIndex = 38
		Me.Label27.Text = "[s]"
		'
		'TbSSspeed
		'
		Me.TbSSspeed.Location = New System.Drawing.Point(158, 3)
		Me.TbSSspeed.Name = "TbSSspeed"
		Me.TbSSspeed.Size = New System.Drawing.Size(64, 20)
		Me.TbSSspeed.TabIndex = 0
		'
		'LabelSSspeed
		'
		Me.LabelSSspeed.AutoSize = True
		Me.LabelSSspeed.Location = New System.Drawing.Point(91, 6)
		Me.LabelSSspeed.Name = "LabelSSspeed"
		Me.LabelSSspeed.Size = New System.Drawing.Size(61, 13)
		Me.LabelSSspeed.TabIndex = 37
		Me.LabelSSspeed.Text = "Max Speed"
		'
		'Label26
		'
		Me.Label26.AutoSize = True
		Me.Label26.Location = New System.Drawing.Point(228, 6)
		Me.Label26.Name = "Label26"
		Me.Label26.Size = New System.Drawing.Size(38, 13)
		Me.Label26.TabIndex = 38
		Me.Label26.Text = "[km/h]"
		'
		'Label30
		'
		Me.Label30.AutoSize = True
		Me.Label30.Location = New System.Drawing.Point(68, 58)
		Me.Label30.Name = "Label30"
		Me.Label30.Size = New System.Drawing.Size(84, 13)
		Me.Label30.TabIndex = 35
		Me.Label30.Text = "Activation Delay"
		'
		'LabelSStime
		'
		Me.LabelSStime.AutoSize = True
		Me.LabelSStime.Location = New System.Drawing.Point(65, 32)
		Me.LabelSStime.Name = "LabelSStime"
		Me.LabelSStime.Size = New System.Drawing.Size(87, 13)
		Me.LabelSStime.TabIndex = 35
		Me.LabelSStime.Text = "Min ICE-On Time"
		'
		'TbSSdelay
		'
		Me.TbSSdelay.Location = New System.Drawing.Point(158, 55)
		Me.TbSSdelay.Name = "TbSSdelay"
		Me.TbSSdelay.Size = New System.Drawing.Size(64, 20)
		Me.TbSSdelay.TabIndex = 2
		'
		'TbSStime
		'
		Me.TbSStime.Location = New System.Drawing.Point(158, 29)
		Me.TbSStime.Name = "TbSStime"
		Me.TbSStime.Size = New System.Drawing.Size(64, 20)
		Me.TbSStime.TabIndex = 1
		'
		'ChBStartStop
		'
		Me.ChBStartStop.AutoSize = True
		Me.ChBStartStop.Checked = True
		Me.ChBStartStop.CheckState = System.Windows.Forms.CheckState.Checked
		Me.ChBStartStop.Location = New System.Drawing.Point(16, 21)
		Me.ChBStartStop.Name = "ChBStartStop"
		Me.ChBStartStop.Size = New System.Drawing.Size(65, 17)
		Me.ChBStartStop.TabIndex = 0
		Me.ChBStartStop.Text = "Enabled"
		Me.ChBStartStop.UseVisualStyleBackColor = True
		'
		'StatusStrip1
		'
		Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelGEN})
		Me.StatusStrip1.Location = New System.Drawing.Point(0, 672)
		Me.StatusStrip1.Name = "StatusStrip1"
		Me.StatusStrip1.Size = New System.Drawing.Size(944, 22)
		Me.StatusStrip1.SizingGrip = False
		Me.StatusStrip1.TabIndex = 6
		Me.StatusStrip1.Text = "StatusStrip1"
		'
		'ToolStripStatusLabelGEN
		'
		Me.ToolStripStatusLabelGEN.Name = "ToolStripStatusLabelGEN"
		Me.ToolStripStatusLabelGEN.Size = New System.Drawing.Size(121, 17)
		Me.ToolStripStatusLabelGEN.Text = "ToolStripStatusLabel1"
		'
		'ButOK
		'
		Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButOK.Location = New System.Drawing.Point(779, 646)
		Me.ButOK.Name = "ButOK"
		Me.ButOK.Size = New System.Drawing.Size(75, 23)
		Me.ButOK.TabIndex = 0
		Me.ButOK.Text = "Save"
		Me.ButOK.UseVisualStyleBackColor = True
		'
		'ButCancel
		'
		Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.ButCancel.Location = New System.Drawing.Point(860, 646)
		Me.ButCancel.Name = "ButCancel"
		Me.ButCancel.Size = New System.Drawing.Size(75, 23)
		Me.ButCancel.TabIndex = 1
		Me.ButCancel.Text = "Cancel"
		Me.ButCancel.UseVisualStyleBackColor = True
		'
		'ToolStrip1
		'
		Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator1, Me.ToolStripBtSendTo, Me.ToolStripSeparator2, Me.ToolStripButton1})
		Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New System.Drawing.Size(944, 25)
		Me.ToolStrip1.TabIndex = 20
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtNew
		'
		Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
		Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtNew.Name = "ToolStripBtNew"
		Me.ToolStripBtNew.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtNew.Text = "New"
		Me.ToolStripBtNew.ToolTipText = "New"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtOpen.Text = "Open"
		Me.ToolStripBtOpen.ToolTipText = "Open..."
		'
		'ToolStripBtSave
		'
		Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
		Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSave.Name = "ToolStripBtSave"
		Me.ToolStripBtSave.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtSave.Text = "Save"
		Me.ToolStripBtSave.ToolTipText = "Save"
		'
		'ToolStripBtSaveAs
		'
		Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
		Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
		Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtSaveAs.Text = "Save As"
		Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
		'
		'ToolStripSeparator1
		'
		Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
		Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 25)
		'
		'ToolStripBtSendTo
		'
		Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
		Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
		Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
		Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
		Me.ToolStripBtSendTo.Size = New System.Drawing.Size(23, 22)
		Me.ToolStripBtSendTo.Text = "Send to Job List"
		Me.ToolStripBtSendTo.ToolTipText = "Send to Job List"
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 25)
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
		'PictureBox1
		'
		Me.PictureBox1.BackColor = System.Drawing.Color.White
		Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_VECTO
		Me.PictureBox1.Location = New System.Drawing.Point(12, 28)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New System.Drawing.Size(920, 40)
		Me.PictureBox1.TabIndex = 21
		Me.PictureBox1.TabStop = False
		'
		'CbEngOnly
		'
		Me.CbEngOnly.AutoSize = True
		Me.CbEngOnly.Location = New System.Drawing.Point(17, 84)
		Me.CbEngOnly.Name = "CbEngOnly"
		Me.CbEngOnly.Size = New System.Drawing.Size(113, 17)
		Me.CbEngOnly.TabIndex = 0
		Me.CbEngOnly.Text = "Engine Only Mode"
		Me.CbEngOnly.UseVisualStyleBackColor = True
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
		'PicVehicle
		'
		Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
		Me.PicVehicle.Location = New System.Drawing.Point(542, 122)
		Me.PicVehicle.Name = "PicVehicle"
		Me.PicVehicle.Size = New System.Drawing.Size(300, 88)
		Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
		Me.PicVehicle.TabIndex = 36
		Me.PicVehicle.TabStop = False
		'
		'PicBox
		'
		Me.PicBox.BackColor = System.Drawing.Color.LightGray
		Me.PicBox.Location = New System.Drawing.Point(542, 268)
		Me.PicBox.Name = "PicBox"
		Me.PicBox.Size = New System.Drawing.Size(390, 327)
		Me.PicBox.TabIndex = 36
		Me.PicBox.TabStop = False
		'
		'TbEngTxt
		'
		Me.TbEngTxt.Location = New System.Drawing.Point(542, 216)
		Me.TbEngTxt.Name = "TbEngTxt"
		Me.TbEngTxt.ReadOnly = True
		Me.TbEngTxt.Size = New System.Drawing.Size(390, 20)
		Me.TbEngTxt.TabIndex = 6
		'
		'TbVehCat
		'
		Me.TbVehCat.Location = New System.Drawing.Point(848, 126)
		Me.TbVehCat.Name = "TbVehCat"
		Me.TbVehCat.ReadOnly = True
		Me.TbVehCat.Size = New System.Drawing.Size(87, 20)
		Me.TbVehCat.TabIndex = 2
		'
		'TbAxleConf
		'
		Me.TbAxleConf.Location = New System.Drawing.Point(904, 155)
		Me.TbAxleConf.Name = "TbAxleConf"
		Me.TbAxleConf.ReadOnly = True
		Me.TbAxleConf.Size = New System.Drawing.Size(31, 20)
		Me.TbAxleConf.TabIndex = 4
		'
		'TbHVCclass
		'
		Me.TbHVCclass.Location = New System.Drawing.Point(848, 184)
		Me.TbHVCclass.Name = "TbHVCclass"
		Me.TbHVCclass.ReadOnly = True
		Me.TbHVCclass.Size = New System.Drawing.Size(87, 20)
		Me.TbHVCclass.TabIndex = 5
		'
		'TbGbxTxt
		'
		Me.TbGbxTxt.Location = New System.Drawing.Point(542, 242)
		Me.TbGbxTxt.Name = "TbGbxTxt"
		Me.TbGbxTxt.ReadOnly = True
		Me.TbGbxTxt.Size = New System.Drawing.Size(390, 20)
		Me.TbGbxTxt.TabIndex = 7
		'
		'TbMass
		'
		Me.TbMass.Location = New System.Drawing.Point(848, 155)
		Me.TbMass.Name = "TbMass"
		Me.TbMass.ReadOnly = True
		Me.TbMass.Size = New System.Drawing.Size(50, 20)
		Me.TbMass.TabIndex = 3
		'
		'VectoJobForm
		'
		Me.AcceptButton = Me.ButOK
		Me.CancelButton = Me.ButCancel
		Me.ClientSize = New System.Drawing.Size(944, 694)
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
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MaximizeBox = False
		Me.Name = "VectoJobForm"
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "Job Editor"
		Me.TabPgGen.ResumeLayout(False)
		Me.TabPgGen.PerformLayout()
		Me.GrCycles.ResumeLayout(False)
		Me.GrCycles.PerformLayout()
		Me.GrAux.ResumeLayout(False)
		Me.GrAux.PerformLayout()
		CType(Me.picAuxInfo, System.ComponentModel.ISupportInitialize).EndInit()
		Me.TabControl1.ResumeLayout(False)
		Me.TabPgDriver.ResumeLayout(False)
		Me.GrVACC.ResumeLayout(False)
		Me.GrVACC.PerformLayout()
		Me.GrLAC.ResumeLayout(False)
		Me.GrLAC.PerformLayout()
		Me.pnLookAheadCoasting.ResumeLayout(False)
		Me.pnLookAheadCoasting.PerformLayout()
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
		CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
		Me.CmOpenFile.ResumeLayout(False)
		CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).EndInit()
		CType(Me.PicBox, System.ComponentModel.ISupportInitialize).EndInit()
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
	Friend WithEvents pnLookAheadCoasting As System.Windows.Forms.Panel
	Friend WithEvents btnDfVelocityDrop As System.Windows.Forms.Button
	Friend WithEvents Label12 As System.Windows.Forms.Label
	Friend WithEvents tbDfCoastingScale As System.Windows.Forms.TextBox
	Friend WithEvents Label11 As System.Windows.Forms.Label
	Friend WithEvents Label3 As System.Windows.Forms.Label
	Friend WithEvents tbDfCoastingOffset As System.Windows.Forms.TextBox
	Friend WithEvents tbLacDfTargetSpeedFile As System.Windows.Forms.TextBox
	Friend WithEvents Label10 As System.Windows.Forms.Label
	Friend WithEvents Label4 As System.Windows.Forms.Label
	Friend WithEvents Label5 As System.Windows.Forms.Label
	Friend WithEvents btnDfTargetSpeed As System.Windows.Forms.Button
	Friend WithEvents tbLacPreviewFactor As System.Windows.Forms.TextBox
	Friend WithEvents tbLacDfVelocityDropFile As System.Windows.Forms.TextBox
	Friend WithEvents Label6 As System.Windows.Forms.Label
	Friend WithEvents tbLacMinSpeed As System.Windows.Forms.TextBox
	Friend WithEvents Label7 As System.Windows.Forms.Label
	Friend WithEvents Label9 As System.Windows.Forms.Label
	Friend WithEvents TbAuxPAdd As System.Windows.Forms.TextBox
	Friend WithEvents Label8 As System.Windows.Forms.Label
End Class
