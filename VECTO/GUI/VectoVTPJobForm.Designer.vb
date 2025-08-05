Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices

<DesignerGenerated()> _
Partial Class VectoVTPJobForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(VectoVTPJobForm))
        Me.GrCycles = New System.Windows.Forms.GroupBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.LvCycles = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.BtDRIrem = New System.Windows.Forms.Button()
        Me.BtDRIadd = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.GrAux = New System.Windows.Forms.GroupBox()
        Me.LvAux = New System.Windows.Forms.ListView()
        Me.ColumnHeader4 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader5 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader6 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TbVEH = New System.Windows.Forms.TextBox()
        Me.ButtonVEH = New System.Windows.Forms.Button()
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
        Me.lblEngineCharacteristics = New System.Windows.Forms.Label()
        Me.PictureBox2 = New System.Windows.Forms.PictureBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.tbFanDiameter = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.pnFanParameters = New System.Windows.Forms.Panel()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.tbC4 = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbC1 = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.tbC3 = New System.Windows.Forms.TextBox()
        Me.tbC2 = New System.Windows.Forms.TextBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.pnManufacturerRecord = New System.Windows.Forms.Panel()
        Me.primaryVIFBtn = New System.Windows.Forms.Button()
        Me.primaryVIFTb = New System.Windows.Forms.TextBox()
        Me.primaryVIFLbl = New System.Windows.Forms.Label()
        Me.cifBtn = New System.Windows.Forms.Button()
        Me.cifTb = New System.Windows.Forms.TextBox()
        Me.cifLbl = New System.Windows.Forms.Label()
        Me.completedVIFButton = New System.Windows.Forms.Button()
        Me.completedVIFTxtbox = New System.Windows.Forms.TextBox()
        Me.completedVIFLlb = New System.Windows.Forms.Label()
        Me.lblMileageUnit = New System.Windows.Forms.Label()
        Me.tbMileage = New System.Windows.Forms.TextBox()
        Me.lblMileage = New System.Windows.Forms.Label()
        Me.mrfLbl = New System.Windows.Forms.Label()
        Me.tbManufacturerRecord = New System.Windows.Forms.TextBox()
        Me.ButtonManR = New System.Windows.Forms.Button()
        Me._ncvGrpBox = New System.Windows.Forms.GroupBox()
        Me._ncvFuel2UnitLbl = New System.Windows.Forms.Label()
        Me._ncvFuel1UnitLbl = New System.Windows.Forms.Label()
        Me._ncvFuel2Txtbox = New System.Windows.Forms.TextBox()
        Me._ncvFuel1Txtbox = New System.Windows.Forms.TextBox()
        Me._ncvFuel2Lbl = New System.Windows.Forms.Label()
        Me._ncvFuel1Lbl = New System.Windows.Forms.Label()
        Me._wheelTorqueDriftGrpbox = New System.Windows.Forms.GroupBox()
        Me._tqdriftRightUnitsLbl = New System.Windows.Forms.Label()
        Me._tqDriftLeftUnitsLbl = New System.Windows.Forms.Label()
        Me._tqDriftRightTextbox = New System.Windows.Forms.TextBox()
        Me._tqDriftLeftTextbox = New System.Windows.Forms.TextBox()
        Me._tqDriftRightLbl = New System.Windows.Forms.Label()
        Me._tqDriftLeftLbl = New System.Windows.Forms.Label()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.tbLifetimeFCVolumeEnd = New System.Windows.Forms.TextBox()
        Me.tbLifetimeFCVolumeStart = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.groupBoxLftmFcMass = New System.Windows.Forms.GroupBox()
        Me.Label18 = New System.Windows.Forms.Label()
        Me.Label19 = New System.Windows.Forms.Label()
        Me.tbLifetimeFCMassEnd = New System.Windows.Forms.TextBox()
        Me.tbLifetimeFCMassStart = New System.Windows.Forms.TextBox()
        Me.Label20 = New System.Windows.Forms.Label()
        Me.Label21 = New System.Windows.Forms.Label()
        Me.Label22 = New System.Windows.Forms.Label()
        Me.tbOdometerReading = New System.Windows.Forms.TextBox()
        Me.Label23 = New System.Windows.Forms.Label()
        Me.gbOBFCM = New System.Windows.Forms.GroupBox()
        Me.GrCycles.SuspendLayout()
        Me.GrAux.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.CmOpenFile.SuspendLayout()
        CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PicBox, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.GroupBox1.SuspendLayout()
        Me.pnFanParameters.SuspendLayout()
        Me.pnManufacturerRecord.SuspendLayout()
        Me._ncvGrpBox.SuspendLayout()
        Me._wheelTorqueDriftGrpbox.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.groupBoxLftmFcMass.SuspendLayout()
        Me.gbOBFCM.SuspendLayout()
        Me.SuspendLayout()
        '
        'GrCycles
        '
        Me.GrCycles.Controls.Add(Me.Label2)
        Me.GrCycles.Controls.Add(Me.LvCycles)
        Me.GrCycles.Controls.Add(Me.BtDRIrem)
        Me.GrCycles.Controls.Add(Me.BtDRIadd)
        Me.GrCycles.Location = New System.Drawing.Point(17, 651)
        Me.GrCycles.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrCycles.Name = "GrCycles"
        Me.GrCycles.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrCycles.Size = New System.Drawing.Size(773, 199)
        Me.GrCycles.TabIndex = 10
        Me.GrCycles.TabStop = False
        Me.GrCycles.Text = "Cycles"
        '
        'Label2
        '
        Me.Label2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(586, 155)
        Me.Label2.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(177, 20)
        Me.Label2.TabIndex = 3
        Me.Label2.Text = "(Right-Click for Options)"
        '
        'LvCycles
        '
        Me.LvCycles.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LvCycles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1})
        Me.LvCycles.FullRowSelect = True
        Me.LvCycles.GridLines = True
        Me.LvCycles.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None
        Me.LvCycles.HideSelection = False
        Me.LvCycles.LabelEdit = True
        Me.LvCycles.Location = New System.Drawing.Point(9, 29)
        Me.LvCycles.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.LvCycles.MultiSelect = False
        Me.LvCycles.Name = "LvCycles"
        Me.LvCycles.Size = New System.Drawing.Size(752, 122)
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
        Me.BtDRIrem.Location = New System.Drawing.Point(44, 155)
        Me.BtDRIrem.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtDRIrem.Name = "BtDRIrem"
        Me.BtDRIrem.Size = New System.Drawing.Size(36, 38)
        Me.BtDRIrem.TabIndex = 2
        Me.BtDRIrem.UseVisualStyleBackColor = True
        '
        'BtDRIadd
        '
        Me.BtDRIadd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtDRIadd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.BtDRIadd.Location = New System.Drawing.Point(8, 155)
        Me.BtDRIadd.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.BtDRIadd.Name = "BtDRIadd"
        Me.BtDRIadd.Size = New System.Drawing.Size(36, 38)
        Me.BtDRIadd.TabIndex = 1
        Me.BtDRIadd.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(6, 20)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(426, 13)
        Me.Label12.TabIndex = 4
        Me.Label12.Text = "The fuel cocnsumption in the cycle has to be corrected for standard NCV!"
        '
        'GrAux
        '
        Me.GrAux.Controls.Add(Me.LvAux)
        Me.GrAux.Location = New System.Drawing.Point(25, 426)
        Me.GrAux.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrAux.Name = "GrAux"
        Me.GrAux.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GrAux.Size = New System.Drawing.Size(773, 215)
        Me.GrAux.TabIndex = 9
        Me.GrAux.TabStop = False
        Me.GrAux.Text = "Auxiliaries"
        '
        'LvAux
        '
        Me.LvAux.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.LvAux.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader5, Me.ColumnHeader6})
        Me.LvAux.FullRowSelect = True
        Me.LvAux.GridLines = True
        Me.LvAux.HideSelection = False
        Me.LvAux.Location = New System.Drawing.Point(6, 29)
        Me.LvAux.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.LvAux.MultiSelect = False
        Me.LvAux.Name = "LvAux"
        Me.LvAux.Size = New System.Drawing.Size(752, 175)
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
        'TbVEH
        '
        Me.TbVEH.Location = New System.Drawing.Point(192, 132)
        Me.TbVEH.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbVEH.Name = "TbVEH"
        Me.TbVEH.Size = New System.Drawing.Size(558, 26)
        Me.TbVEH.TabIndex = 1
        '
        'ButtonVEH
        '
        Me.ButtonVEH.Image = CType(resources.GetObject("ButtonVEH.Image"), System.Drawing.Image)
        Me.ButtonVEH.Location = New System.Drawing.Point(753, 129)
        Me.ButtonVEH.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonVEH.Name = "ButtonVEH"
        Me.ButtonVEH.Size = New System.Drawing.Size(36, 38)
        Me.ButtonVEH.TabIndex = 2
        Me.ButtonVEH.TabStop = False
        Me.ButtonVEH.UseVisualStyleBackColor = True
        '
        'StatusStrip1
        '
        Me.StatusStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripStatusLabelGEN})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 1297)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Padding = New System.Windows.Forms.Padding(1, 0, 21, 0)
        Me.StatusStrip1.Size = New System.Drawing.Size(1438, 32)
        Me.StatusStrip1.SizingGrip = False
        Me.StatusStrip1.TabIndex = 6
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'ToolStripStatusLabelGEN
        '
        Me.ToolStripStatusLabelGEN.Name = "ToolStripStatusLabelGEN"
        Me.ToolStripStatusLabelGEN.Size = New System.Drawing.Size(180, 25)
        Me.ToolStripStatusLabelGEN.Text = "ToolStripStatusLabel1"
        '
        'ButOK
        '
        Me.ButOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButOK.Location = New System.Drawing.Point(1172, 1257)
        Me.ButOK.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButOK.Name = "ButOK"
        Me.ButOK.Size = New System.Drawing.Size(112, 35)
        Me.ButOK.TabIndex = 0
        Me.ButOK.Text = "Save"
        Me.ButOK.UseVisualStyleBackColor = True
        '
        'ButCancel
        '
        Me.ButCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.ButCancel.Location = New System.Drawing.Point(1289, 1257)
        Me.ButCancel.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButCancel.Name = "ButCancel"
        Me.ButCancel.Size = New System.Drawing.Size(112, 35)
        Me.ButCancel.TabIndex = 1
        Me.ButCancel.Text = "Cancel"
        Me.ButCancel.UseVisualStyleBackColor = True
        '
        'ToolStrip1
        '
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(20, 20)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripBtSave, Me.ToolStripBtSaveAs, Me.ToolStripSeparator1, Me.ToolStripBtSendTo, Me.ToolStripSeparator2, Me.ToolStripButton1})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1438, 29)
        Me.ToolStrip1.TabIndex = 20
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(34, 24)
        Me.ToolStripBtNew.Text = "New"
        Me.ToolStripBtNew.ToolTipText = "New"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(34, 24)
        Me.ToolStripBtOpen.Text = "Open"
        Me.ToolStripBtOpen.ToolTipText = "Open..."
        '
        'ToolStripBtSave
        '
        Me.ToolStripBtSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSave.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_icon
        Me.ToolStripBtSave.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSave.Name = "ToolStripBtSave"
        Me.ToolStripBtSave.Size = New System.Drawing.Size(34, 24)
        Me.ToolStripBtSave.Text = "Save"
        Me.ToolStripBtSave.ToolTipText = "Save"
        '
        'ToolStripBtSaveAs
        '
        Me.ToolStripBtSaveAs.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSaveAs.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_document_save_as_icon
        Me.ToolStripBtSaveAs.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSaveAs.Name = "ToolStripBtSaveAs"
        Me.ToolStripBtSaveAs.Size = New System.Drawing.Size(34, 24)
        Me.ToolStripBtSaveAs.Text = "Save As"
        Me.ToolStripBtSaveAs.ToolTipText = "Save As..."
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 29)
        '
        'ToolStripBtSendTo
        '
        Me.ToolStripBtSendTo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtSendTo.Image = Global.TUGraz.VECTO.My.Resources.Resources.export_icon
        Me.ToolStripBtSendTo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtSendTo.Name = "ToolStripBtSendTo"
        Me.ToolStripBtSendTo.Size = New System.Drawing.Size(34, 24)
        Me.ToolStripBtSendTo.Text = "Send to Job List"
        Me.ToolStripBtSendTo.ToolTipText = "Send to Job List"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 29)
        '
        'ToolStripButton1
        '
        Me.ToolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripButton1.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripButton1.Name = "ToolStripButton1"
        Me.ToolStripButton1.Size = New System.Drawing.Size(34, 24)
        Me.ToolStripButton1.Text = "Help"
        '
        'PictureBox1
        '
        Me.PictureBox1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PictureBox1.BackColor = System.Drawing.Color.White
        Me.PictureBox1.Image = Global.TUGraz.VECTO.My.Resources.Resources.VECTO_VECTO
        Me.PictureBox1.Location = New System.Drawing.Point(0, 42)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(1438, 61)
        Me.PictureBox1.TabIndex = 21
        Me.PictureBox1.TabStop = False
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(20, 20)
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
        'PicVehicle
        '
        Me.PicVehicle.BackColor = System.Drawing.Color.LightGray
        Me.PicVehicle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicVehicle.Location = New System.Drawing.Point(813, 122)
        Me.PicVehicle.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PicVehicle.Name = "PicVehicle"
        Me.PicVehicle.Size = New System.Drawing.Size(449, 134)
        Me.PicVehicle.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.PicVehicle.TabIndex = 36
        Me.PicVehicle.TabStop = False
        '
        'PicBox
        '
        Me.PicBox.BackColor = System.Drawing.Color.LightGray
        Me.PicBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.PicBox.Location = New System.Drawing.Point(813, 338)
        Me.PicBox.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PicBox.Name = "PicBox"
        Me.PicBox.Size = New System.Drawing.Size(584, 454)
        Me.PicBox.TabIndex = 36
        Me.PicBox.TabStop = False
        '
        'TbEngTxt
        '
        Me.TbEngTxt.Location = New System.Drawing.Point(813, 262)
        Me.TbEngTxt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbEngTxt.Name = "TbEngTxt"
        Me.TbEngTxt.ReadOnly = True
        Me.TbEngTxt.Size = New System.Drawing.Size(583, 26)
        Me.TbEngTxt.TabIndex = 6
        '
        'TbVehCat
        '
        Me.TbVehCat.Location = New System.Drawing.Point(1272, 129)
        Me.TbVehCat.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbVehCat.Name = "TbVehCat"
        Me.TbVehCat.ReadOnly = True
        Me.TbVehCat.Size = New System.Drawing.Size(129, 26)
        Me.TbVehCat.TabIndex = 2
        '
        'TbAxleConf
        '
        Me.TbAxleConf.Location = New System.Drawing.Point(1356, 174)
        Me.TbAxleConf.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbAxleConf.Name = "TbAxleConf"
        Me.TbAxleConf.ReadOnly = True
        Me.TbAxleConf.Size = New System.Drawing.Size(44, 26)
        Me.TbAxleConf.TabIndex = 4
        '
        'TbHVCclass
        '
        Me.TbHVCclass.Location = New System.Drawing.Point(1272, 219)
        Me.TbHVCclass.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbHVCclass.Name = "TbHVCclass"
        Me.TbHVCclass.ReadOnly = True
        Me.TbHVCclass.Size = New System.Drawing.Size(129, 26)
        Me.TbHVCclass.TabIndex = 5
        '
        'TbGbxTxt
        '
        Me.TbGbxTxt.Location = New System.Drawing.Point(813, 299)
        Me.TbGbxTxt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbGbxTxt.Name = "TbGbxTxt"
        Me.TbGbxTxt.ReadOnly = True
        Me.TbGbxTxt.Size = New System.Drawing.Size(583, 26)
        Me.TbGbxTxt.TabIndex = 7
        '
        'TbMass
        '
        Me.TbMass.Location = New System.Drawing.Point(1272, 174)
        Me.TbMass.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TbMass.Name = "TbMass"
        Me.TbMass.ReadOnly = True
        Me.TbMass.Size = New System.Drawing.Size(73, 26)
        Me.TbMass.TabIndex = 3
        '
        'lblEngineCharacteristics
        '
        Me.lblEngineCharacteristics.AutoSize = True
        Me.lblEngineCharacteristics.Location = New System.Drawing.Point(813, 798)
        Me.lblEngineCharacteristics.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblEngineCharacteristics.Name = "lblEngineCharacteristics"
        Me.lblEngineCharacteristics.Size = New System.Drawing.Size(0, 20)
        Me.lblEngineCharacteristics.TabIndex = 37
        '
        'PictureBox2
        '
        Me.PictureBox2.Image = Global.TUGraz.VECTO.My.Resources.Resources.P_fan_eqn
        Me.PictureBox2.InitialImage = CType(resources.GetObject("PictureBox2.InitialImage"), System.Drawing.Image)
        Me.PictureBox2.Location = New System.Drawing.Point(10, 29)
        Me.PictureBox2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.PictureBox2.Name = "PictureBox2"
        Me.PictureBox2.Size = New System.Drawing.Size(415, 186)
        Me.PictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
        Me.PictureBox2.TabIndex = 38
        Me.PictureBox2.TabStop = False
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.tbFanDiameter)
        Me.GroupBox1.Controls.Add(Me.Label7)
        Me.GroupBox1.Controls.Add(Me.pnFanParameters)
        Me.GroupBox1.Controls.Add(Me.PictureBox2)
        Me.GroupBox1.Controls.Add(Me.Label6)
        Me.GroupBox1.Location = New System.Drawing.Point(15, 858)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.GroupBox1.Size = New System.Drawing.Size(775, 215)
        Me.GroupBox1.TabIndex = 39
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Fan Power"
        '
        'tbFanDiameter
        '
        Me.tbFanDiameter.Location = New System.Drawing.Point(557, 20)
        Me.tbFanDiameter.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbFanDiameter.Name = "tbFanDiameter"
        Me.tbFanDiameter.Size = New System.Drawing.Size(127, 26)
        Me.tbFanDiameter.TabIndex = 0
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(694, 25)
        Me.Label7.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(43, 20)
        Me.Label7.TabIndex = 47
        Me.Label7.Text = "[mm]"
        '
        'pnFanParameters
        '
        Me.pnFanParameters.Controls.Add(Me.Label13)
        Me.pnFanParameters.Controls.Add(Me.Label14)
        Me.pnFanParameters.Controls.Add(Me.tbC4)
        Me.pnFanParameters.Controls.Add(Me.Label11)
        Me.pnFanParameters.Controls.Add(Me.Label10)
        Me.pnFanParameters.Controls.Add(Me.Label9)
        Me.pnFanParameters.Controls.Add(Me.Label1)
        Me.pnFanParameters.Controls.Add(Me.tbC1)
        Me.pnFanParameters.Controls.Add(Me.Label3)
        Me.pnFanParameters.Controls.Add(Me.tbC3)
        Me.pnFanParameters.Controls.Add(Me.tbC2)
        Me.pnFanParameters.Controls.Add(Me.Label4)
        Me.pnFanParameters.Location = New System.Drawing.Point(435, 48)
        Me.pnFanParameters.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnFanParameters.Name = "pnFanParameters"
        Me.pnFanParameters.Size = New System.Drawing.Size(327, 168)
        Me.pnFanParameters.TabIndex = 48
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Location = New System.Drawing.Point(260, 138)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(22, 20)
        Me.Label13.TabIndex = 56
        Me.Label13.Text = "[-]"
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Location = New System.Drawing.Point(78, 136)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(33, 20)
        Me.Label14.TabIndex = 55
        Me.Label14.Text = "C4:"
        '
        'tbC4
        '
        Me.tbC4.Location = New System.Drawing.Point(122, 132)
        Me.tbC4.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbC4.Name = "tbC4"
        Me.tbC4.Size = New System.Drawing.Size(127, 26)
        Me.tbC4.TabIndex = 54
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Location = New System.Drawing.Point(260, 98)
        Me.Label11.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(43, 20)
        Me.Label11.TabIndex = 50
        Me.Label11.Text = "[mm]"
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Location = New System.Drawing.Point(260, 58)
        Me.Label10.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(44, 20)
        Me.Label10.TabIndex = 49
        Me.Label10.Text = "[rpm]"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Location = New System.Drawing.Point(260, 18)
        Me.Label9.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(40, 20)
        Me.Label9.TabIndex = 48
        Me.Label9.Text = "[kW]"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(78, 18)
        Me.Label1.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(33, 20)
        Me.Label1.TabIndex = 39
        Me.Label1.Text = "C1:"
        '
        'tbC1
        '
        Me.tbC1.Location = New System.Drawing.Point(122, 12)
        Me.tbC1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbC1.Name = "tbC1"
        Me.tbC1.Size = New System.Drawing.Size(127, 26)
        Me.tbC1.TabIndex = 40
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(78, 58)
        Me.Label3.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(33, 20)
        Me.Label3.TabIndex = 41
        Me.Label3.Text = "C2:"
        '
        'tbC3
        '
        Me.tbC3.Location = New System.Drawing.Point(122, 92)
        Me.tbC3.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbC3.Name = "tbC3"
        Me.tbC3.Size = New System.Drawing.Size(127, 26)
        Me.tbC3.TabIndex = 44
        '
        'tbC2
        '
        Me.tbC2.Location = New System.Drawing.Point(122, 52)
        Me.tbC2.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbC2.Name = "tbC2"
        Me.tbC2.Size = New System.Drawing.Size(127, 26)
        Me.tbC2.TabIndex = 42
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(78, 98)
        Me.Label4.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(33, 20)
        Me.Label4.TabIndex = 43
        Me.Label4.Text = "C3:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(441, 25)
        Me.Label6.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(107, 20)
        Me.Label6.TabIndex = 45
        Me.Label6.Text = "Fan diameter:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(22, 138)
        Me.Label5.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(102, 20)
        Me.Label5.TabIndex = 40
        Me.Label5.Text = "Vehicle XML:"
        '
        'pnManufacturerRecord
        '
        Me.pnManufacturerRecord.Controls.Add(Me.primaryVIFBtn)
        Me.pnManufacturerRecord.Controls.Add(Me.primaryVIFTb)
        Me.pnManufacturerRecord.Controls.Add(Me.primaryVIFLbl)
        Me.pnManufacturerRecord.Controls.Add(Me.cifBtn)
        Me.pnManufacturerRecord.Controls.Add(Me.cifTb)
        Me.pnManufacturerRecord.Controls.Add(Me.cifLbl)
        Me.pnManufacturerRecord.Controls.Add(Me.completedVIFButton)
        Me.pnManufacturerRecord.Controls.Add(Me.completedVIFTxtbox)
        Me.pnManufacturerRecord.Controls.Add(Me.completedVIFLlb)
        Me.pnManufacturerRecord.Controls.Add(Me.lblMileageUnit)
        Me.pnManufacturerRecord.Controls.Add(Me.tbMileage)
        Me.pnManufacturerRecord.Controls.Add(Me.lblMileage)
        Me.pnManufacturerRecord.Controls.Add(Me.mrfLbl)
        Me.pnManufacturerRecord.Controls.Add(Me.tbManufacturerRecord)
        Me.pnManufacturerRecord.Controls.Add(Me.ButtonManR)
        Me.pnManufacturerRecord.Location = New System.Drawing.Point(17, 172)
        Me.pnManufacturerRecord.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnManufacturerRecord.Name = "pnManufacturerRecord"
        Me.pnManufacturerRecord.Size = New System.Drawing.Size(788, 251)
        Me.pnManufacturerRecord.TabIndex = 41
        '
        'primaryVIFBtn
        '
        Me.primaryVIFBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.primaryVIFBtn.Image = CType(resources.GetObject("primaryVIFBtn.Image"), System.Drawing.Image)
        Me.primaryVIFBtn.Location = New System.Drawing.Point(738, 154)
        Me.primaryVIFBtn.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.primaryVIFBtn.Name = "primaryVIFBtn"
        Me.primaryVIFBtn.Size = New System.Drawing.Size(36, 38)
        Me.primaryVIFBtn.TabIndex = 58
        Me.primaryVIFBtn.TabStop = False
        Me.primaryVIFBtn.UseVisualStyleBackColor = True
        '
        'primaryVIFTb
        '
        Me.primaryVIFTb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.primaryVIFTb.Location = New System.Drawing.Point(176, 160)
        Me.primaryVIFTb.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.primaryVIFTb.Name = "primaryVIFTb"
        Me.primaryVIFTb.Size = New System.Drawing.Size(559, 26)
        Me.primaryVIFTb.TabIndex = 57
        '
        'primaryVIFLbl
        '
        Me.primaryVIFLbl.AutoSize = True
        Me.primaryVIFLbl.Location = New System.Drawing.Point(5, 167)
        Me.primaryVIFLbl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.primaryVIFLbl.Name = "primaryVIFLbl"
        Me.primaryVIFLbl.Size = New System.Drawing.Size(95, 20)
        Me.primaryVIFLbl.TabIndex = 56
        Me.primaryVIFLbl.Text = "Primary VIF:"
        '
        'cifBtn
        '
        Me.cifBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cifBtn.Image = CType(resources.GetObject("cifBtn.Image"), System.Drawing.Image)
        Me.cifBtn.Location = New System.Drawing.Point(738, 54)
        Me.cifBtn.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cifBtn.Name = "cifBtn"
        Me.cifBtn.Size = New System.Drawing.Size(36, 38)
        Me.cifBtn.TabIndex = 55
        Me.cifBtn.TabStop = False
        Me.cifBtn.UseVisualStyleBackColor = True
        '
        'cifTb
        '
        Me.cifTb.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cifTb.Location = New System.Drawing.Point(175, 60)
        Me.cifTb.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.cifTb.Name = "cifTb"
        Me.cifTb.Size = New System.Drawing.Size(559, 26)
        Me.cifTb.TabIndex = 54
        '
        'cifLbl
        '
        Me.cifLbl.AutoSize = True
        Me.cifLbl.Location = New System.Drawing.Point(5, 63)
        Me.cifLbl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.cifLbl.Name = "cifLbl"
        Me.cifLbl.Size = New System.Drawing.Size(167, 20)
        Me.cifLbl.TabIndex = 53
        Me.cifLbl.Text = "Customer Information:"
        '
        'completedVIFButton
        '
        Me.completedVIFButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.completedVIFButton.Image = CType(resources.GetObject("completedVIFButton.Image"), System.Drawing.Image)
        Me.completedVIFButton.Location = New System.Drawing.Point(738, 105)
        Me.completedVIFButton.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.completedVIFButton.Name = "completedVIFButton"
        Me.completedVIFButton.Size = New System.Drawing.Size(36, 38)
        Me.completedVIFButton.TabIndex = 52
        Me.completedVIFButton.TabStop = False
        Me.completedVIFButton.UseVisualStyleBackColor = True
        '
        'completedVIFTxtbox
        '
        Me.completedVIFTxtbox.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.completedVIFTxtbox.Location = New System.Drawing.Point(176, 110)
        Me.completedVIFTxtbox.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.completedVIFTxtbox.Name = "completedVIFTxtbox"
        Me.completedVIFTxtbox.Size = New System.Drawing.Size(559, 26)
        Me.completedVIFTxtbox.TabIndex = 51
        '
        'completedVIFLlb
        '
        Me.completedVIFLlb.AutoSize = True
        Me.completedVIFLlb.Location = New System.Drawing.Point(4, 117)
        Me.completedVIFLlb.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.completedVIFLlb.Name = "completedVIFLlb"
        Me.completedVIFLlb.Size = New System.Drawing.Size(120, 20)
        Me.completedVIFLlb.TabIndex = 50
        Me.completedVIFLlb.Text = "Completed VIF:"
        '
        'lblMileageUnit
        '
        Me.lblMileageUnit.AutoSize = True
        Me.lblMileageUnit.Location = New System.Drawing.Point(297, 210)
        Me.lblMileageUnit.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMileageUnit.Name = "lblMileageUnit"
        Me.lblMileageUnit.Size = New System.Drawing.Size(38, 20)
        Me.lblMileageUnit.TabIndex = 49
        Me.lblMileageUnit.Text = "[km]"
        '
        'tbMileage
        '
        Me.tbMileage.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbMileage.Location = New System.Drawing.Point(174, 205)
        Me.tbMileage.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbMileage.Name = "tbMileage"
        Me.tbMileage.Size = New System.Drawing.Size(112, 26)
        Me.tbMileage.TabIndex = 48
        '
        'lblMileage
        '
        Me.lblMileage.AutoSize = True
        Me.lblMileage.Location = New System.Drawing.Point(4, 210)
        Me.lblMileage.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblMileage.Name = "lblMileage"
        Me.lblMileage.Size = New System.Drawing.Size(68, 20)
        Me.lblMileage.TabIndex = 47
        Me.lblMileage.Text = "Mileage:"
        '
        'mrfLbl
        '
        Me.mrfLbl.AutoSize = True
        Me.mrfLbl.Location = New System.Drawing.Point(4, 18)
        Me.mrfLbl.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.mrfLbl.Name = "mrfLbl"
        Me.mrfLbl.Size = New System.Drawing.Size(48, 20)
        Me.mrfLbl.TabIndex = 43
        Me.mrfLbl.Text = "MRF:"
        '
        'tbManufacturerRecord
        '
        Me.tbManufacturerRecord.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbManufacturerRecord.Location = New System.Drawing.Point(176, 12)
        Me.tbManufacturerRecord.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbManufacturerRecord.Name = "tbManufacturerRecord"
        Me.tbManufacturerRecord.Size = New System.Drawing.Size(559, 26)
        Me.tbManufacturerRecord.TabIndex = 41
        '
        'ButtonManR
        '
        Me.ButtonManR.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ButtonManR.Image = CType(resources.GetObject("ButtonManR.Image"), System.Drawing.Image)
        Me.ButtonManR.Location = New System.Drawing.Point(738, 9)
        Me.ButtonManR.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.ButtonManR.Name = "ButtonManR"
        Me.ButtonManR.Size = New System.Drawing.Size(36, 38)
        Me.ButtonManR.TabIndex = 42
        Me.ButtonManR.TabStop = False
        Me.ButtonManR.UseVisualStyleBackColor = True
        '
        '_ncvGrpBox
        '
        Me._ncvGrpBox.Controls.Add(Me._ncvFuel2UnitLbl)
        Me._ncvGrpBox.Controls.Add(Me._ncvFuel1UnitLbl)
        Me._ncvGrpBox.Controls.Add(Me._ncvFuel2Txtbox)
        Me._ncvGrpBox.Controls.Add(Me._ncvFuel1Txtbox)
        Me._ncvGrpBox.Controls.Add(Me._ncvFuel2Lbl)
        Me._ncvGrpBox.Controls.Add(Me._ncvFuel1Lbl)
        Me._ncvGrpBox.Location = New System.Drawing.Point(813, 830)
        Me._ncvGrpBox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._ncvGrpBox.Name = "_ncvGrpBox"
        Me._ncvGrpBox.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._ncvGrpBox.Size = New System.Drawing.Size(304, 112)
        Me._ncvGrpBox.TabIndex = 42
        Me._ncvGrpBox.TabStop = False
        Me._ncvGrpBox.Text = "Measured Net Calorific Values"
        '
        '_ncvFuel2UnitLbl
        '
        Me._ncvFuel2UnitLbl.AutoSize = True
        Me._ncvFuel2UnitLbl.Location = New System.Drawing.Point(228, 75)
        Me._ncvFuel2UnitLbl.Name = "_ncvFuel2UnitLbl"
        Me._ncvFuel2UnitLbl.Size = New System.Drawing.Size(59, 20)
        Me._ncvFuel2UnitLbl.TabIndex = 5
        Me._ncvFuel2UnitLbl.Text = "[MJ/kg]"
        '
        '_ncvFuel1UnitLbl
        '
        Me._ncvFuel1UnitLbl.AutoSize = True
        Me._ncvFuel1UnitLbl.Location = New System.Drawing.Point(228, 35)
        Me._ncvFuel1UnitLbl.Name = "_ncvFuel1UnitLbl"
        Me._ncvFuel1UnitLbl.Size = New System.Drawing.Size(59, 20)
        Me._ncvFuel1UnitLbl.TabIndex = 4
        Me._ncvFuel1UnitLbl.Text = "[MJ/kg]"
        '
        '_ncvFuel2Txtbox
        '
        Me._ncvFuel2Txtbox.Location = New System.Drawing.Point(105, 71)
        Me._ncvFuel2Txtbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._ncvFuel2Txtbox.Name = "_ncvFuel2Txtbox"
        Me._ncvFuel2Txtbox.Size = New System.Drawing.Size(112, 26)
        Me._ncvFuel2Txtbox.TabIndex = 3
        '
        '_ncvFuel1Txtbox
        '
        Me._ncvFuel1Txtbox.Location = New System.Drawing.Point(105, 31)
        Me._ncvFuel1Txtbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._ncvFuel1Txtbox.Name = "_ncvFuel1Txtbox"
        Me._ncvFuel1Txtbox.Size = New System.Drawing.Size(112, 26)
        Me._ncvFuel1Txtbox.TabIndex = 2
        '
        '_ncvFuel2Lbl
        '
        Me._ncvFuel2Lbl.AutoSize = True
        Me._ncvFuel2Lbl.Location = New System.Drawing.Point(20, 75)
        Me._ncvFuel2Lbl.Name = "_ncvFuel2Lbl"
        Me._ncvFuel2Lbl.Size = New System.Drawing.Size(87, 20)
        Me._ncvFuel2Lbl.TabIndex = 1
        Me._ncvFuel2Lbl.Text = "<fuel type>"
        '
        '_ncvFuel1Lbl
        '
        Me._ncvFuel1Lbl.AutoSize = True
        Me._ncvFuel1Lbl.Location = New System.Drawing.Point(20, 35)
        Me._ncvFuel1Lbl.Name = "_ncvFuel1Lbl"
        Me._ncvFuel1Lbl.Size = New System.Drawing.Size(87, 20)
        Me._ncvFuel1Lbl.TabIndex = 0
        Me._ncvFuel1Lbl.Text = "<fuel type>"
        '
        '_wheelTorqueDriftGrpbox
        '
        Me._wheelTorqueDriftGrpbox.Controls.Add(Me._tqdriftRightUnitsLbl)
        Me._wheelTorqueDriftGrpbox.Controls.Add(Me._tqDriftLeftUnitsLbl)
        Me._wheelTorqueDriftGrpbox.Controls.Add(Me._tqDriftRightTextbox)
        Me._wheelTorqueDriftGrpbox.Controls.Add(Me._tqDriftLeftTextbox)
        Me._wheelTorqueDriftGrpbox.Controls.Add(Me._tqDriftRightLbl)
        Me._wheelTorqueDriftGrpbox.Controls.Add(Me._tqDriftLeftLbl)
        Me._wheelTorqueDriftGrpbox.Location = New System.Drawing.Point(1124, 830)
        Me._wheelTorqueDriftGrpbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._wheelTorqueDriftGrpbox.Name = "_wheelTorqueDriftGrpbox"
        Me._wheelTorqueDriftGrpbox.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._wheelTorqueDriftGrpbox.Size = New System.Drawing.Size(259, 112)
        Me._wheelTorqueDriftGrpbox.TabIndex = 43
        Me._wheelTorqueDriftGrpbox.TabStop = False
        Me._wheelTorqueDriftGrpbox.Text = "Wheel Torque Drift"
        '
        '_tqdriftRightUnitsLbl
        '
        Me._tqdriftRightUnitsLbl.AutoSize = True
        Me._tqdriftRightUnitsLbl.Location = New System.Drawing.Point(202, 75)
        Me._tqdriftRightUnitsLbl.Name = "_tqdriftRightUnitsLbl"
        Me._tqdriftRightUnitsLbl.Size = New System.Drawing.Size(41, 20)
        Me._tqdriftRightUnitsLbl.TabIndex = 5
        Me._tqdriftRightUnitsLbl.Text = "[Nm]"
        '
        '_tqDriftLeftUnitsLbl
        '
        Me._tqDriftLeftUnitsLbl.AutoSize = True
        Me._tqDriftLeftUnitsLbl.Location = New System.Drawing.Point(202, 35)
        Me._tqDriftLeftUnitsLbl.Name = "_tqDriftLeftUnitsLbl"
        Me._tqDriftLeftUnitsLbl.Size = New System.Drawing.Size(41, 20)
        Me._tqDriftLeftUnitsLbl.TabIndex = 4
        Me._tqDriftLeftUnitsLbl.Text = "[Nm]"
        '
        '_tqDriftRightTextbox
        '
        Me._tqDriftRightTextbox.Location = New System.Drawing.Point(79, 71)
        Me._tqDriftRightTextbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._tqDriftRightTextbox.Name = "_tqDriftRightTextbox"
        Me._tqDriftRightTextbox.Size = New System.Drawing.Size(112, 26)
        Me._tqDriftRightTextbox.TabIndex = 3
        '
        '_tqDriftLeftTextbox
        '
        Me._tqDriftLeftTextbox.Location = New System.Drawing.Point(79, 31)
        Me._tqDriftLeftTextbox.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me._tqDriftLeftTextbox.Name = "_tqDriftLeftTextbox"
        Me._tqDriftLeftTextbox.Size = New System.Drawing.Size(112, 26)
        Me._tqDriftLeftTextbox.TabIndex = 2
        '
        '_tqDriftRightLbl
        '
        Me._tqDriftRightLbl.AutoSize = True
        Me._tqDriftRightLbl.Location = New System.Drawing.Point(20, 75)
        Me._tqDriftRightLbl.Name = "_tqDriftRightLbl"
        Me._tqDriftRightLbl.Size = New System.Drawing.Size(47, 20)
        Me._tqDriftRightLbl.TabIndex = 1
        Me._tqDriftRightLbl.Text = "Right"
        '
        '_tqDriftLeftLbl
        '
        Me._tqDriftLeftLbl.AutoSize = True
        Me._tqDriftLeftLbl.Location = New System.Drawing.Point(20, 35)
        Me._tqDriftLeftLbl.Name = "_tqDriftLeftLbl"
        Me._tqDriftLeftLbl.Size = New System.Drawing.Size(37, 20)
        Me._tqDriftLeftLbl.TabIndex = 0
        Me._tqDriftLeftLbl.Text = "Left"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.Label8)
        Me.GroupBox2.Controls.Add(Me.Label15)
        Me.GroupBox2.Controls.Add(Me.tbLifetimeFCVolumeEnd)
        Me.GroupBox2.Controls.Add(Me.tbLifetimeFCVolumeStart)
        Me.GroupBox2.Controls.Add(Me.Label16)
        Me.GroupBox2.Controls.Add(Me.Label17)
        Me.GroupBox2.Location = New System.Drawing.Point(328, 58)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.GroupBox2.Size = New System.Drawing.Size(259, 112)
        Me.GroupBox2.TabIndex = 45
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Lifetime Consumption by Volume"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(202, 75)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(26, 20)
        Me.Label8.TabIndex = 5
        Me.Label8.Text = "[L]"
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Location = New System.Drawing.Point(202, 35)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(26, 20)
        Me.Label15.TabIndex = 4
        Me.Label15.Text = "[L]"
        '
        'tbLifetimeFCVolumeEnd
        '
        Me.tbLifetimeFCVolumeEnd.Location = New System.Drawing.Point(79, 71)
        Me.tbLifetimeFCVolumeEnd.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbLifetimeFCVolumeEnd.Name = "tbLifetimeFCVolumeEnd"
        Me.tbLifetimeFCVolumeEnd.Size = New System.Drawing.Size(112, 26)
        Me.tbLifetimeFCVolumeEnd.TabIndex = 3
        '
        'tbLifetimeFCVolumeStart
        '
        Me.tbLifetimeFCVolumeStart.Location = New System.Drawing.Point(79, 31)
        Me.tbLifetimeFCVolumeStart.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbLifetimeFCVolumeStart.Name = "tbLifetimeFCVolumeStart"
        Me.tbLifetimeFCVolumeStart.Size = New System.Drawing.Size(112, 26)
        Me.tbLifetimeFCVolumeStart.TabIndex = 2
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Location = New System.Drawing.Point(20, 75)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(38, 20)
        Me.Label16.TabIndex = 1
        Me.Label16.Text = "End"
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Location = New System.Drawing.Point(20, 35)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(44, 20)
        Me.Label17.TabIndex = 0
        Me.Label17.Text = "Start"
        '
        'groupBoxLftmFcMass
        '
        Me.groupBoxLftmFcMass.Controls.Add(Me.Label18)
        Me.groupBoxLftmFcMass.Controls.Add(Me.Label19)
        Me.groupBoxLftmFcMass.Controls.Add(Me.tbLifetimeFCMassEnd)
        Me.groupBoxLftmFcMass.Controls.Add(Me.tbLifetimeFCMassStart)
        Me.groupBoxLftmFcMass.Controls.Add(Me.Label20)
        Me.groupBoxLftmFcMass.Controls.Add(Me.Label21)
        Me.groupBoxLftmFcMass.Location = New System.Drawing.Point(17, 58)
        Me.groupBoxLftmFcMass.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.groupBoxLftmFcMass.Name = "groupBoxLftmFcMass"
        Me.groupBoxLftmFcMass.Padding = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.groupBoxLftmFcMass.Size = New System.Drawing.Size(304, 112)
        Me.groupBoxLftmFcMass.TabIndex = 44
        Me.groupBoxLftmFcMass.TabStop = False
        Me.groupBoxLftmFcMass.Text = "Lifetime Consumption by Mass"
        '
        'Label18
        '
        Me.Label18.AutoSize = True
        Me.Label18.Location = New System.Drawing.Point(228, 75)
        Me.Label18.Name = "Label18"
        Me.Label18.Size = New System.Drawing.Size(34, 20)
        Me.Label18.TabIndex = 5
        Me.Label18.Text = "[kg]"
        '
        'Label19
        '
        Me.Label19.AutoSize = True
        Me.Label19.Location = New System.Drawing.Point(228, 35)
        Me.Label19.Name = "Label19"
        Me.Label19.Size = New System.Drawing.Size(34, 20)
        Me.Label19.TabIndex = 4
        Me.Label19.Text = "[kg]"
        '
        'tbLifetimeFCMassEnd
        '
        Me.tbLifetimeFCMassEnd.Location = New System.Drawing.Point(105, 71)
        Me.tbLifetimeFCMassEnd.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbLifetimeFCMassEnd.Name = "tbLifetimeFCMassEnd"
        Me.tbLifetimeFCMassEnd.Size = New System.Drawing.Size(112, 26)
        Me.tbLifetimeFCMassEnd.TabIndex = 3
        '
        'tbLifetimeFCMassStart
        '
        Me.tbLifetimeFCMassStart.Location = New System.Drawing.Point(105, 31)
        Me.tbLifetimeFCMassStart.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbLifetimeFCMassStart.Name = "tbLifetimeFCMassStart"
        Me.tbLifetimeFCMassStart.Size = New System.Drawing.Size(112, 26)
        Me.tbLifetimeFCMassStart.TabIndex = 2
        '
        'Label20
        '
        Me.Label20.AutoSize = True
        Me.Label20.Location = New System.Drawing.Point(20, 75)
        Me.Label20.Name = "Label20"
        Me.Label20.Size = New System.Drawing.Size(38, 20)
        Me.Label20.TabIndex = 1
        Me.Label20.Text = "End"
        '
        'Label21
        '
        Me.Label21.AutoSize = True
        Me.Label21.Location = New System.Drawing.Point(20, 35)
        Me.Label21.Name = "Label21"
        Me.Label21.Size = New System.Drawing.Size(44, 20)
        Me.Label21.TabIndex = 0
        Me.Label21.Text = "Start"
        '
        'Label22
        '
        Me.Label22.AutoSize = True
        Me.Label22.Location = New System.Drawing.Point(318, 30)
        Me.Label22.Name = "Label22"
        Me.Label22.Size = New System.Drawing.Size(38, 20)
        Me.Label22.TabIndex = 48
        Me.Label22.Text = "[km]"
        '
        'tbOdometerReading
        '
        Me.tbOdometerReading.Location = New System.Drawing.Point(195, 26)
        Me.tbOdometerReading.Margin = New System.Windows.Forms.Padding(3, 4, 3, 4)
        Me.tbOdometerReading.Name = "tbOdometerReading"
        Me.tbOdometerReading.Size = New System.Drawing.Size(112, 26)
        Me.tbOdometerReading.TabIndex = 47
        '
        'Label23
        '
        Me.Label23.AutoSize = True
        Me.Label23.Location = New System.Drawing.Point(21, 29)
        Me.Label23.Name = "Label23"
        Me.Label23.Size = New System.Drawing.Size(168, 20)
        Me.Label23.TabIndex = 46
        Me.Label23.Text = "Odometer end reading"
        '
        'gbOBFCM
        '
        Me.gbOBFCM.Controls.Add(Me.Label22)
        Me.gbOBFCM.Controls.Add(Me.tbOdometerReading)
        Me.gbOBFCM.Controls.Add(Me.Label23)
        Me.gbOBFCM.Controls.Add(Me.GroupBox2)
        Me.gbOBFCM.Controls.Add(Me.groupBoxLftmFcMass)
        Me.gbOBFCM.Location = New System.Drawing.Point(14, 1094)
        Me.gbOBFCM.Name = "gbOBFCM"
        Me.gbOBFCM.Size = New System.Drawing.Size(593, 182)
        Me.gbOBFCM.TabIndex = 49
        Me.gbOBFCM.TabStop = False
        Me.gbOBFCM.Text = "OBFCM"
        '
        'VectoVTPJobForm
        '
        Me.AcceptButton = Me.ButOK
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9.0!, 20.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.CancelButton = Me.ButCancel
        Me.ClientSize = New System.Drawing.Size(1438, 1329)
        Me.Controls.Add(Me.gbOBFCM)
        Me.Controls.Add(Me._wheelTorqueDriftGrpbox)
        Me.Controls.Add(Me._ncvGrpBox)
        Me.Controls.Add(Me.pnManufacturerRecord)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GrAux)
        Me.Controls.Add(Me.GrCycles)
        Me.Controls.Add(Me.lblEngineCharacteristics)
        Me.Controls.Add(Me.TbHVCclass)
        Me.Controls.Add(Me.TbMass)
        Me.Controls.Add(Me.TbVEH)
        Me.Controls.Add(Me.TbAxleConf)
        Me.Controls.Add(Me.TbVehCat)
        Me.Controls.Add(Me.TbGbxTxt)
        Me.Controls.Add(Me.TbEngTxt)
        Me.Controls.Add(Me.ButtonVEH)
        Me.Controls.Add(Me.PicBox)
        Me.Controls.Add(Me.PicVehicle)
        Me.Controls.Add(Me.PictureBox1)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.ButCancel)
        Me.Controls.Add(Me.ButOK)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MinimumSize = New System.Drawing.Size(1460, 1385)
        Me.Name = "VectoVTPJobForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Job Editor"
        Me.GrCycles.ResumeLayout(False)
        Me.GrCycles.PerformLayout()
        Me.GrAux.ResumeLayout(False)
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.CmOpenFile.ResumeLayout(False)
        CType(Me.PicVehicle, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PicBox, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.PictureBox2, System.ComponentModel.ISupportInitialize).EndInit()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.pnFanParameters.ResumeLayout(False)
        Me.pnFanParameters.PerformLayout()
        Me.pnManufacturerRecord.ResumeLayout(False)
        Me.pnManufacturerRecord.PerformLayout()
        Me._ncvGrpBox.ResumeLayout(False)
        Me._ncvGrpBox.PerformLayout()
        Me._wheelTorqueDriftGrpbox.ResumeLayout(False)
        Me._wheelTorqueDriftGrpbox.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.groupBoxLftmFcMass.ResumeLayout(False)
        Me.groupBoxLftmFcMass.PerformLayout()
        Me.gbOBFCM.ResumeLayout(False)
        Me.gbOBFCM.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents ButtonVEH As Button
    Friend WithEvents ToolStripStatusLabelGEN As ToolStripStatusLabel
    Friend WithEvents ButOK As Button
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
    Friend WithEvents PictureBox1 As PictureBox
    Friend WithEvents GrCycles As GroupBox
    Friend WithEvents LvCycles As ListView
    Friend WithEvents ColumnHeader1 As ColumnHeader
    Friend WithEvents BtDRIrem As Button
    Friend WithEvents BtDRIadd As Button
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripButton1 As ToolStripButton
    Friend WithEvents Label2 As Label
    Friend WithEvents CmOpenFile As ContextMenuStrip
    Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents PicVehicle As PictureBox
    Friend WithEvents PicBox As PictureBox
    Friend WithEvents TbEngTxt As TextBox
    Friend WithEvents TbVehCat As TextBox
    Friend WithEvents TbAxleConf As TextBox
    Friend WithEvents TbHVCclass As TextBox
    Friend WithEvents TbGbxTxt As TextBox
    Friend WithEvents TbMass As TextBox
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents lblEngineCharacteristics As System.Windows.Forms.Label
    Friend WithEvents PictureBox2 As System.Windows.Forms.PictureBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents tbC3 As System.Windows.Forms.TextBox
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents tbC2 As System.Windows.Forms.TextBox
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents tbC1 As System.Windows.Forms.TextBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents tbFanDiameter As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents Label7 As Label
    Friend WithEvents pnManufacturerRecord As Panel
    Friend WithEvents mrfLbl As Label
    Friend WithEvents tbManufacturerRecord As TextBox
    Friend WithEvents ButtonManR As Button
    Friend WithEvents lblMileageUnit As Label
    Friend WithEvents tbMileage As TextBox
    Friend WithEvents lblMileage As Label
    Friend WithEvents pnFanParameters As Panel
    Friend WithEvents Label11 As Label
    Friend WithEvents Label10 As Label
    Friend WithEvents Label9 As Label
    Friend WithEvents Label12 As Label
    Friend WithEvents _ncvGrpBox As GroupBox
    Friend WithEvents _ncvFuel1Lbl As Label
    Friend WithEvents _ncvFuel2Lbl As Label
    Friend WithEvents _ncvFuel1UnitLbl As Label
    Friend WithEvents _ncvFuel2Txtbox As TextBox
    Friend WithEvents _ncvFuel1Txtbox As TextBox
    Friend WithEvents _ncvFuel2UnitLbl As Label
    Friend WithEvents _wheelTorqueDriftGrpbox As GroupBox
    Friend WithEvents _tqDriftLeftLbl As Label
    Friend WithEvents _tqDriftLeftUnitsLbl As Label
    Friend WithEvents _tqDriftRightTextbox As TextBox
    Friend WithEvents _tqDriftLeftTextbox As TextBox
    Friend WithEvents _tqDriftRightLbl As Label
    Friend WithEvents _tqdriftRightUnitsLbl As Label
    Friend WithEvents Label13 As Label
    Friend WithEvents Label14 As Label
    Friend WithEvents tbC4 As TextBox
    Friend WithEvents completedVIFLlb As Label
    Friend WithEvents completedVIFTxtbox As TextBox
    Friend WithEvents completedVIFButton As Button
    Friend WithEvents cifBtn As Button
    Friend WithEvents cifTb As TextBox
    Friend WithEvents cifLbl As Label
    Friend WithEvents primaryVIFBtn As Button
    Friend WithEvents primaryVIFTb As TextBox
    Friend WithEvents primaryVIFLbl As Label
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents Label8 As Label
    Friend WithEvents Label15 As Label
    Friend WithEvents tbLifetimeFCVolumeEnd As TextBox
    Friend WithEvents tbLifetimeFCVolumeStart As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents Label17 As Label
    Friend WithEvents groupBoxLftmFcMass As GroupBox
    Friend WithEvents Label18 As Label
    Friend WithEvents Label19 As Label
    Friend WithEvents tbLifetimeFCMassEnd As TextBox
    Friend WithEvents tbLifetimeFCMassStart As TextBox
    Friend WithEvents Label20 As Label
    Friend WithEvents Label21 As Label
    Friend WithEvents Label22 As Label
    Friend WithEvents tbOdometerReading As TextBox
    Friend WithEvents Label23 As Label
    Friend WithEvents gbOBFCM As GroupBox
End Class
