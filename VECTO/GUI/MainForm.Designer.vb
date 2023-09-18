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

<DesignerGenerated()> _
Partial Class MainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.StatusBAR = New System.Windows.Forms.StatusStrip()
        Me.ToolStripLbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.ToolStripProgBarJob = New System.Windows.Forms.ToolStripProgressBar()
        Me.ToolStripProgBarOverall = New System.Windows.Forms.ToolStripProgressBar()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.TabPageGEN = New System.Windows.Forms.TabPage()
        Me.btnImportXML = New System.Windows.Forms.Button()
        Me.btnExportXML = New System.Windows.Forms.Button()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.btStartV3 = New System.Windows.Forms.Button()
        Me.LbDecl = New System.Windows.Forms.Label()
        Me.PictureBox1 = New System.Windows.Forms.PictureBox()
        Me.BtGENdown = New System.Windows.Forms.Button()
        Me.BtGENup = New System.Windows.Forms.Button()
        Me.ChBoxAllGEN = New System.Windows.Forms.CheckBox()
        Me.LvGEN = New System.Windows.Forms.ListView()
        Me.ColGENpath = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColGENstatus = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ButtonGENremove = New System.Windows.Forms.Button()
        Me.ButtonGENadd = New System.Windows.Forms.Button()
        Me.TabPgOptions = New System.Windows.Forms.TabPage()
        Me.PanelOptAllg = New System.Windows.Forms.Panel()
        Me.GroupBox6 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.tbInitSOCinPercent = New System.Windows.Forms.TextBox()
        Me.cbInitialSOC = New System.Windows.Forms.CheckBox()
        Me.cbCSIteratingModeDeactivated = New System.Windows.Forms.CheckBox()
        Me.GroupBox5 = New System.Windows.Forms.GroupBox()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.tbMinSpeedLAC = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.GroupBox4 = New System.Windows.Forms.GroupBox()
        Me.BtTCfileBrowse = New System.Windows.Forms.Button()
        Me.tbOutputFolder = New System.Windows.Forms.TextBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.cbSaveVectoRunData = New System.Windows.Forms.CheckBox()
        Me.cbActVmod = New System.Windows.Forms.CheckBox()
        Me.cbValidateRunData = New System.Windows.Forms.CheckBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.ChBoxModOut = New System.Windows.Forms.CheckBox()
        Me.ChBoxMod1Hz = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.RbDev = New System.Windows.Forms.RadioButton()
        Me.RbDecl = New System.Windows.Forms.RadioButton()
        Me.ConMenFilelist = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ShowInFolderMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SaveListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadDefaultListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ClearListToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.BackgroundWorker1 = New System.ComponentModel.BackgroundWorker()
        Me.LvMsg = New System.Windows.Forms.ListView()
        Me.ColumnHeader1 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader2 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
        Me.ToolStrip1 = New System.Windows.Forms.ToolStrip()
        Me.ToolStripBtNew = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripBtOpen = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripDrDnBtTools = New System.Windows.Forms.ToolStripDropDownButton()
        Me.GENEditorToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorSerialHybridVehicleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorParallelHybridVehicleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorBatteryElectricVehicleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorIEPC_E_VehicleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorIEPC_S_VehicleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorIHPCVehicleToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.JobEditorEngineOnlyModeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EPTPJobEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.VEHEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EngineEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GearboxEditorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GraphToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator6 = New System.Windows.Forms.ToolStripSeparator()
        Me.OpenLogToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.SettingsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripDrDnBtInfo = New System.Windows.Forms.ToolStripDropDownButton()
        Me.UserManualToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UpdateNotesToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ReportBugViaCITnetToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.AboutVECTOToolStripMenuItem1 = New System.Windows.Forms.ToolStripMenuItem()
        Me.CmDEV = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TmProgSec = New System.Windows.Forms.Timer(Me.components)
        Me.CmOpenFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.OpenWithToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.OpenInGraphWindowToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ShowInFolderToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.StatusBAR.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.TabPageGEN.SuspendLayout()
        CType(Me.PictureBox1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TabPgOptions.SuspendLayout()
        Me.PanelOptAllg.SuspendLayout()
        Me.GroupBox6.SuspendLayout()
        Me.GroupBox5.SuspendLayout()
        Me.GroupBox4.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.ConMenFilelist.SuspendLayout()
        CType(Me.SplitContainer1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SplitContainer1.Panel1.SuspendLayout()
        Me.SplitContainer1.Panel2.SuspendLayout()
        Me.SplitContainer1.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.CmOpenFile.SuspendLayout()
        Me.SuspendLayout()
        '
        'StatusBAR
        '
        Me.StatusBAR.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.StatusBAR.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripLbStatus, Me.ToolStripProgBarJob, Me.ToolStripProgBarOverall})
        Me.StatusBAR.Location = New System.Drawing.Point(0, 799)
        Me.StatusBAR.Name = "StatusBAR"
        Me.StatusBAR.Padding = New System.Windows.Forms.Padding(1, 0, 18, 0)
        Me.StatusBAR.Size = New System.Drawing.Size(1393, 26)
        Me.StatusBAR.TabIndex = 7
        Me.StatusBAR.Text = "StatusBAR"
        '
        'ToolStripLbStatus
        '
        Me.ToolStripLbStatus.Name = "ToolStripLbStatus"
        Me.ToolStripLbStatus.Size = New System.Drawing.Size(1374, 20)
        Me.ToolStripLbStatus.Spring = True
        Me.ToolStripLbStatus.Text = "Status Text"
        Me.ToolStripLbStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'ToolStripProgBarJob
        '
        Me.ToolStripProgBarJob.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripProgBarJob.AutoSize = False
        Me.ToolStripProgBarJob.Name = "ToolStripProgBarJob"
        Me.ToolStripProgBarJob.Size = New System.Drawing.Size(134, 18)
        Me.ToolStripProgBarJob.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ToolStripProgBarJob.ToolTipText = "overall progress"
        Me.ToolStripProgBarJob.Visible = False
        '
        'ToolStripProgBarOverall
        '
        Me.ToolStripProgBarOverall.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right
        Me.ToolStripProgBarOverall.AutoSize = False
        Me.ToolStripProgBarOverall.Name = "ToolStripProgBarOverall"
        Me.ToolStripProgBarOverall.Size = New System.Drawing.Size(134, 18)
        Me.ToolStripProgBarOverall.Style = System.Windows.Forms.ProgressBarStyle.Continuous
        Me.ToolStripProgBarOverall.ToolTipText = "job progress"
        Me.ToolStripProgBarOverall.Visible = False
        '
        'TabControl1
        '
        Me.TabControl1.Controls.Add(Me.TabPageGEN)
        Me.TabControl1.Controls.Add(Me.TabPgOptions)
        Me.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.TabControl1.Location = New System.Drawing.Point(5, 3)
        Me.TabControl1.Margin = New System.Windows.Forms.Padding(0)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.Padding = New System.Drawing.Point(0, 0)
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1388, 401)
        Me.TabControl1.TabIndex = 10
        '
        'TabPageGEN
        '
        Me.TabPageGEN.Controls.Add(Me.btnImportXML)
        Me.TabPageGEN.Controls.Add(Me.btnExportXML)
        Me.TabPageGEN.Controls.Add(Me.Label6)
        Me.TabPageGEN.Controls.Add(Me.btStartV3)
        Me.TabPageGEN.Controls.Add(Me.LbDecl)
        Me.TabPageGEN.Controls.Add(Me.PictureBox1)
        Me.TabPageGEN.Controls.Add(Me.BtGENdown)
        Me.TabPageGEN.Controls.Add(Me.BtGENup)
        Me.TabPageGEN.Controls.Add(Me.ChBoxAllGEN)
        Me.TabPageGEN.Controls.Add(Me.LvGEN)
        Me.TabPageGEN.Controls.Add(Me.ButtonGENremove)
        Me.TabPageGEN.Controls.Add(Me.ButtonGENadd)
        Me.TabPageGEN.Location = New System.Drawing.Point(4, 25)
        Me.TabPageGEN.Margin = New System.Windows.Forms.Padding(0)
        Me.TabPageGEN.Name = "TabPageGEN"
        Me.TabPageGEN.Size = New System.Drawing.Size(1380, 372)
        Me.TabPageGEN.TabIndex = 0
        Me.TabPageGEN.Text = "Job Files"
        Me.TabPageGEN.UseVisualStyleBackColor = True
        '
        'btnImportXML
        '
        Me.btnImportXML.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnImportXML.Location = New System.Drawing.Point(614, 317)
        Me.btnImportXML.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.btnImportXML.Name = "btnImportXML"
        Me.btnImportXML.Size = New System.Drawing.Size(153, 37)
        Me.btnImportXML.TabIndex = 23
        Me.btnImportXML.Text = "Import from XML"
        Me.btnImportXML.UseVisualStyleBackColor = True
        Me.btnImportXML.Visible = False
        '
        'btnExportXML
        '
        Me.btnExportXML.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnExportXML.Enabled = False
        Me.btnExportXML.Location = New System.Drawing.Point(458, 317)
        Me.btnExportXML.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.btnExportXML.Name = "btnExportXML"
        Me.btnExportXML.Size = New System.Drawing.Size(153, 37)
        Me.btnExportXML.TabIndex = 22
        Me.btnExportXML.Text = "Export as XML"
        Me.btnExportXML.UseVisualStyleBackColor = True
        Me.btnExportXML.Visible = False
        '
        'Label6
        '
        Me.Label6.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(1084, 318)
        Me.Label6.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(269, 16)
        Me.Label6.TabIndex = 21
        Me.Label6.Text = "(Double-Click to Edit, Right-Click for Options)"
        '
        'btStartV3
        '
        Me.btStartV3.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!)
        Me.btStartV3.Image = Global.TUGraz.VECTO.My.Resources.Resources.Play_icon
        Me.btStartV3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.btStartV3.Location = New System.Drawing.Point(5, 69)
        Me.btStartV3.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.btStartV3.Name = "btStartV3"
        Me.btStartV3.Size = New System.Drawing.Size(144, 62)
        Me.btStartV3.TabIndex = 20
        Me.btStartV3.Text = "START"
        Me.btStartV3.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText
        Me.ToolTip1.SetToolTip(Me.btStartV3, "Start Simulation")
        Me.btStartV3.UseVisualStyleBackColor = True
        '
        'LbDecl
        '
        Me.LbDecl.AutoSize = True
        Me.LbDecl.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.LbDecl.Location = New System.Drawing.Point(7, 134)
        Me.LbDecl.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.LbDecl.Name = "LbDecl"
        Me.LbDecl.Size = New System.Drawing.Size(135, 17)
        Me.LbDecl.TabIndex = 19
        Me.LbDecl.Text = "Declaration Mode"
        Me.LbDecl.Visible = False
        '
        'PictureBox1
        '
        Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), System.Drawing.Image)
        Me.PictureBox1.Location = New System.Drawing.Point(5, 3)
        Me.PictureBox1.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.PictureBox1.Name = "PictureBox1"
        Me.PictureBox1.Size = New System.Drawing.Size(144, 58)
        Me.PictureBox1.TabIndex = 18
        Me.PictureBox1.TabStop = False
        '
        'BtGENdown
        '
        Me.BtGENdown.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtGENdown.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_arrow_down_icon
        Me.BtGENdown.Location = New System.Drawing.Point(409, 317)
        Me.BtGENdown.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.BtGENdown.Name = "BtGENdown"
        Me.BtGENdown.Size = New System.Drawing.Size(40, 37)
        Me.BtGENdown.TabIndex = 6
        Me.ToolTip1.SetToolTip(Me.BtGENdown, "Move job down one row")
        Me.BtGENdown.UseVisualStyleBackColor = True
        '
        'BtGENup
        '
        Me.BtGENup.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.BtGENup.Image = Global.TUGraz.VECTO.My.Resources.Resources.Actions_arrow_up_icon
        Me.BtGENup.Location = New System.Drawing.Point(368, 317)
        Me.BtGENup.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.BtGENup.Name = "BtGENup"
        Me.BtGENup.Size = New System.Drawing.Size(40, 37)
        Me.BtGENup.TabIndex = 4
        Me.ToolTip1.SetToolTip(Me.BtGENup, "Move job up one row")
        Me.BtGENup.UseVisualStyleBackColor = True
        '
        'ChBoxAllGEN
        '
        Me.ChBoxAllGEN.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ChBoxAllGEN.AutoSize = True
        Me.ChBoxAllGEN.Location = New System.Drawing.Point(259, 325)
        Me.ChBoxAllGEN.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.ChBoxAllGEN.Name = "ChBoxAllGEN"
        Me.ChBoxAllGEN.Size = New System.Drawing.Size(85, 20)
        Me.ChBoxAllGEN.TabIndex = 16
        Me.ChBoxAllGEN.Text = "Select All"
        Me.ToolTip1.SetToolTip(Me.ChBoxAllGEN, "Select All / None")
        Me.ChBoxAllGEN.UseVisualStyleBackColor = True
        '
        'LvGEN
        '
        Me.LvGEN.AllowDrop = True
        Me.LvGEN.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LvGEN.CheckBoxes = True
        Me.LvGEN.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColGENpath, Me.ColGENstatus})
        Me.LvGEN.FullRowSelect = True
        Me.LvGEN.GridLines = True
        Me.LvGEN.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.LvGEN.HideSelection = False
        Me.LvGEN.LabelEdit = True
        Me.LvGEN.Location = New System.Drawing.Point(152, 3)
        Me.LvGEN.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.LvGEN.Name = "LvGEN"
        Me.LvGEN.Size = New System.Drawing.Size(1219, 310)
        Me.LvGEN.TabIndex = 14
        Me.LvGEN.UseCompatibleStateImageBehavior = False
        Me.LvGEN.View = System.Windows.Forms.View.Details
        '
        'ColGENpath
        '
        Me.ColGENpath.Text = "Filepath"
        Me.ColGENpath.Width = 797
        '
        'ColGENstatus
        '
        Me.ColGENstatus.Text = ""
        Me.ColGENstatus.Width = 175
        '
        'ButtonGENremove
        '
        Me.ButtonGENremove.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonGENremove.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.ButtonGENremove.Image = Global.TUGraz.VECTO.My.Resources.Resources.minus_circle_icon
        Me.ButtonGENremove.Location = New System.Drawing.Point(197, 317)
        Me.ButtonGENremove.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.ButtonGENremove.Name = "ButtonGENremove"
        Me.ButtonGENremove.Size = New System.Drawing.Size(43, 37)
        Me.ButtonGENremove.TabIndex = 2
        Me.ToolTip1.SetToolTip(Me.ButtonGENremove, "Remove selected entries")
        Me.ButtonGENremove.UseVisualStyleBackColor = True
        '
        'ButtonGENadd
        '
        Me.ButtonGENadd.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ButtonGENadd.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold)
        Me.ButtonGENadd.Image = Global.TUGraz.VECTO.My.Resources.Resources.plus_circle_icon
        Me.ButtonGENadd.Location = New System.Drawing.Point(151, 317)
        Me.ButtonGENadd.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.ButtonGENadd.Name = "ButtonGENadd"
        Me.ButtonGENadd.Size = New System.Drawing.Size(43, 37)
        Me.ButtonGENadd.TabIndex = 1
        Me.ToolTip1.SetToolTip(Me.ButtonGENadd, "Add Job File")
        Me.ButtonGENadd.UseVisualStyleBackColor = True
        '
        'TabPgOptions
        '
        Me.TabPgOptions.Controls.Add(Me.PanelOptAllg)
        Me.TabPgOptions.Location = New System.Drawing.Point(4, 25)
        Me.TabPgOptions.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.TabPgOptions.Name = "TabPgOptions"
        Me.TabPgOptions.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.TabPgOptions.Size = New System.Drawing.Size(1381, 372)
        Me.TabPgOptions.TabIndex = 2
        Me.TabPgOptions.Text = "Options"
        Me.TabPgOptions.UseVisualStyleBackColor = True
        '
        'PanelOptAllg
        '
        Me.PanelOptAllg.Controls.Add(Me.GroupBox6)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox5)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox4)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox3)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox2)
        Me.PanelOptAllg.Controls.Add(Me.GroupBox1)
        Me.PanelOptAllg.Location = New System.Drawing.Point(8, 7)
        Me.PanelOptAllg.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.PanelOptAllg.Name = "PanelOptAllg"
        Me.PanelOptAllg.Size = New System.Drawing.Size(1362, 357)
        Me.PanelOptAllg.TabIndex = 0
        '
        'GroupBox6
        '
        Me.GroupBox6.Controls.Add(Me.Label1)
        Me.GroupBox6.Controls.Add(Me.tbInitSOCinPercent)
        Me.GroupBox6.Controls.Add(Me.cbInitialSOC)
        Me.GroupBox6.Controls.Add(Me.cbCSIteratingModeDeactivated)
        Me.GroupBox6.Location = New System.Drawing.Point(248, 199)
        Me.GroupBox6.Name = "GroupBox6"
        Me.GroupBox6.Size = New System.Drawing.Size(383, 154)
        Me.GroupBox6.TabIndex = 21
        Me.GroupBox6.TabStop = False
        Me.GroupBox6.Text = "2nd Amendment Test Settings"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(347, 124)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(27, 16)
        Me.Label1.TabIndex = 3
        Me.Label1.Text = "[%]"
        '
        'tbInitSOCinPercent
        '
        Me.tbInitSOCinPercent.Location = New System.Drawing.Point(290, 119)
        Me.tbInitSOCinPercent.Name = "tbInitSOCinPercent"
        Me.tbInitSOCinPercent.Size = New System.Drawing.Size(50, 22)
        Me.tbInitSOCinPercent.TabIndex = 2
        '
        'cbInitialSOC
        '
        Me.cbInitialSOC.AutoSize = True
        Me.cbInitialSOC.Location = New System.Drawing.Point(11, 60)
        Me.cbInitialSOC.Name = "cbInitialSOC"
        Me.cbInitialSOC.Size = New System.Drawing.Size(303, 20)
        Me.cbInitialSOC.TabIndex = 1
        Me.cbInitialSOC.Text = "Charge Depleting Mode Central SOC Override"
        Me.cbInitialSOC.UseVisualStyleBackColor = True
        '
        'cbCSIteratingModeDeactivated
        '
        Me.cbCSIteratingModeDeactivated.AutoSize = True
        Me.cbCSIteratingModeDeactivated.Checked = True
        Me.cbCSIteratingModeDeactivated.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbCSIteratingModeDeactivated.Location = New System.Drawing.Point(11, 26)
        Me.cbCSIteratingModeDeactivated.Name = "cbCSIteratingModeDeactivated"
        Me.cbCSIteratingModeDeactivated.Size = New System.Drawing.Size(339, 20)
        Me.cbCSIteratingModeDeactivated.TabIndex = 0
        Me.cbCSIteratingModeDeactivated.Text = "P-HEV Single Simulation in Charge Sustaining Mode"
        Me.cbCSIteratingModeDeactivated.UseVisualStyleBackColor = True
        '
        'GroupBox5
        '
        Me.GroupBox5.Controls.Add(Me.Label5)
        Me.GroupBox5.Controls.Add(Me.Label4)
        Me.GroupBox5.Controls.Add(Me.tbMinSpeedLAC)
        Me.GroupBox5.Controls.Add(Me.Label3)
        Me.GroupBox5.Controls.Add(Me.Label2)
        Me.GroupBox5.Location = New System.Drawing.Point(245, 70)
        Me.GroupBox5.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox5.Name = "GroupBox5"
        Me.GroupBox5.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox5.Size = New System.Drawing.Size(386, 123)
        Me.GroupBox5.TabIndex = 20
        Me.GroupBox5.TabStop = False
        Me.GroupBox5.Text = "Look-Ahead Coasting Override"
        Me.GroupBox5.Visible = False
        '
        'Label5
        '
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Italic)
        Me.Label5.Location = New System.Drawing.Point(9, 59)
        Me.Label5.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(329, 45)
        Me.Label5.TabIndex = 4
        Me.Label5.Text = "Overrides Look-Ahead Coasting in declaration mode. Leave empty to use default beh" &
    "aviour."
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(14, 54)
        Me.Label4.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(0, 16)
        Me.Label4.TabIndex = 3
        '
        'tbMinSpeedLAC
        '
        Me.tbMinSpeedLAC.Location = New System.Drawing.Point(128, 21)
        Me.tbMinSpeedLAC.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.tbMinSpeedLAC.Name = "tbMinSpeedLAC"
        Me.tbMinSpeedLAC.Size = New System.Drawing.Size(74, 22)
        Me.tbMinSpeedLAC.TabIndex = 2
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(210, 25)
        Me.Label3.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(44, 16)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "[km/h]"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(9, 23)
        Me.Label2.Margin = New System.Windows.Forms.Padding(5, 0, 5, 0)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(75, 16)
        Me.Label2.TabIndex = 1
        Me.Label2.Text = "Min Speed:"
        '
        'GroupBox4
        '
        Me.GroupBox4.Controls.Add(Me.BtTCfileBrowse)
        Me.GroupBox4.Controls.Add(Me.tbOutputFolder)
        Me.GroupBox4.Location = New System.Drawing.Point(245, 5)
        Me.GroupBox4.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox4.Name = "GroupBox4"
        Me.GroupBox4.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox4.Size = New System.Drawing.Size(386, 57)
        Me.GroupBox4.TabIndex = 19
        Me.GroupBox4.TabStop = False
        Me.GroupBox4.Text = "Output Directory"
        '
        'BtTCfileBrowse
        '
        Me.BtTCfileBrowse.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.BtTCfileBrowse.Location = New System.Drawing.Point(303, 17)
        Me.BtTCfileBrowse.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.BtTCfileBrowse.Name = "BtTCfileBrowse"
        Me.BtTCfileBrowse.Size = New System.Drawing.Size(32, 30)
        Me.BtTCfileBrowse.TabIndex = 27
        Me.BtTCfileBrowse.TabStop = False
        Me.BtTCfileBrowse.UseVisualStyleBackColor = True
        '
        'tbOutputFolder
        '
        Me.tbOutputFolder.Location = New System.Drawing.Point(8, 21)
        Me.tbOutputFolder.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.tbOutputFolder.Name = "tbOutputFolder"
        Me.tbOutputFolder.Size = New System.Drawing.Size(285, 22)
        Me.tbOutputFolder.TabIndex = 0
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.cbSaveVectoRunData)
        Me.GroupBox3.Controls.Add(Me.cbActVmod)
        Me.GroupBox3.Controls.Add(Me.cbValidateRunData)
        Me.GroupBox3.Location = New System.Drawing.Point(5, 218)
        Me.GroupBox3.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox3.Size = New System.Drawing.Size(231, 135)
        Me.GroupBox3.TabIndex = 18
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Misc"
        '
        'cbSaveVectoRunData
        '
        Me.cbSaveVectoRunData.AutoSize = True
        Me.cbSaveVectoRunData.Location = New System.Drawing.Point(9, 106)
        Me.cbSaveVectoRunData.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.cbSaveVectoRunData.Name = "cbSaveVectoRunData"
        Me.cbSaveVectoRunData.Size = New System.Drawing.Size(205, 20)
        Me.cbSaveVectoRunData.TabIndex = 19
        Me.cbSaveVectoRunData.Text = "Export ModelData (EXPERT!)"
        Me.cbSaveVectoRunData.UseVisualStyleBackColor = True
        '
        'cbActVmod
        '
        Me.cbActVmod.Location = New System.Drawing.Point(8, 42)
        Me.cbActVmod.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.cbActVmod.Name = "cbActVmod"
        Me.cbActVmod.Size = New System.Drawing.Size(223, 64)
        Me.cbActVmod.TabIndex = 18
        Me.cbActVmod.Text = "Output values in vmod at beginning and end of simulation interval (EXPERT!)"
        Me.cbActVmod.UseVisualStyleBackColor = True
        '
        'cbValidateRunData
        '
        Me.cbValidateRunData.AutoSize = True
        Me.cbValidateRunData.Checked = True
        Me.cbValidateRunData.CheckState = System.Windows.Forms.CheckState.Checked
        Me.cbValidateRunData.Location = New System.Drawing.Point(8, 23)
        Me.cbValidateRunData.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.cbValidateRunData.Name = "cbValidateRunData"
        Me.cbValidateRunData.Size = New System.Drawing.Size(111, 20)
        Me.cbValidateRunData.TabIndex = 17
        Me.cbValidateRunData.Text = "Validate Data"
        Me.cbValidateRunData.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.ChBoxModOut)
        Me.GroupBox2.Controls.Add(Me.ChBoxMod1Hz)
        Me.GroupBox2.Location = New System.Drawing.Point(5, 101)
        Me.GroupBox2.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox2.Size = New System.Drawing.Size(231, 110)
        Me.GroupBox2.TabIndex = 16
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Output"
        '
        'ChBoxModOut
        '
        Me.ChBoxModOut.AutoSize = True
        Me.ChBoxModOut.Checked = True
        Me.ChBoxModOut.CheckState = System.Windows.Forms.CheckState.Checked
        Me.ChBoxModOut.Location = New System.Drawing.Point(8, 23)
        Me.ChBoxModOut.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.ChBoxModOut.Name = "ChBoxModOut"
        Me.ChBoxModOut.Size = New System.Drawing.Size(143, 20)
        Me.ChBoxModOut.TabIndex = 0
        Me.ChBoxModOut.Text = "Write modal results"
        Me.ChBoxModOut.UseVisualStyleBackColor = True
        '
        'ChBoxMod1Hz
        '
        Me.ChBoxMod1Hz.AutoSize = True
        Me.ChBoxMod1Hz.Location = New System.Drawing.Point(8, 51)
        Me.ChBoxMod1Hz.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.ChBoxMod1Hz.Name = "ChBoxMod1Hz"
        Me.ChBoxMod1Hz.Size = New System.Drawing.Size(148, 20)
        Me.ChBoxMod1Hz.TabIndex = 16
        Me.ChBoxMod1Hz.Text = "Modal results in 1Hz"
        Me.ChBoxMod1Hz.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.RbDev)
        Me.GroupBox1.Controls.Add(Me.RbDecl)
        Me.GroupBox1.Location = New System.Drawing.Point(5, 3)
        Me.GroupBox1.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Padding = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.GroupBox1.Size = New System.Drawing.Size(231, 89)
        Me.GroupBox1.TabIndex = 15
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Mode"
        '
        'RbDev
        '
        Me.RbDev.AutoSize = True
        Me.RbDev.Checked = True
        Me.RbDev.Location = New System.Drawing.Point(8, 51)
        Me.RbDev.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.RbDev.Name = "RbDev"
        Me.RbDev.Size = New System.Drawing.Size(138, 20)
        Me.RbDev.TabIndex = 1
        Me.RbDev.TabStop = True
        Me.RbDev.Text = "Engineering Mode"
        Me.RbDev.UseVisualStyleBackColor = True
        '
        'RbDecl
        '
        Me.RbDecl.AutoSize = True
        Me.RbDecl.Location = New System.Drawing.Point(8, 23)
        Me.RbDecl.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.RbDecl.Name = "RbDecl"
        Me.RbDecl.Size = New System.Drawing.Size(135, 20)
        Me.RbDecl.TabIndex = 0
        Me.RbDecl.TabStop = True
        Me.RbDecl.Text = "Declaration Mode"
        Me.RbDecl.UseVisualStyleBackColor = True
        '
        'ConMenFilelist
        '
        Me.ConMenFilelist.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ConMenFilelist.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ShowInFolderMenuItem, Me.SaveListToolStripMenuItem, Me.LoadListToolStripMenuItem, Me.LoadDefaultListToolStripMenuItem, Me.ClearListToolStripMenuItem})
        Me.ConMenFilelist.Name = "ConMenFilelist"
        Me.ConMenFilelist.ShowImageMargin = False
        Me.ConMenFilelist.Size = New System.Drawing.Size(180, 124)
        '
        'ShowInFolderMenuItem
        '
        Me.ShowInFolderMenuItem.Name = "ShowInFolderMenuItem"
        Me.ShowInFolderMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.ShowInFolderMenuItem.Text = "Show in Folder"
        '
        'SaveListToolStripMenuItem
        '
        Me.SaveListToolStripMenuItem.Name = "SaveListToolStripMenuItem"
        Me.SaveListToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.SaveListToolStripMenuItem.Text = "Save List..."
        '
        'LoadListToolStripMenuItem
        '
        Me.LoadListToolStripMenuItem.Name = "LoadListToolStripMenuItem"
        Me.LoadListToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.LoadListToolStripMenuItem.Text = "Load List..."
        '
        'LoadDefaultListToolStripMenuItem
        '
        Me.LoadDefaultListToolStripMenuItem.Name = "LoadDefaultListToolStripMenuItem"
        Me.LoadDefaultListToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.LoadDefaultListToolStripMenuItem.Text = "Load Autosave-List"
        '
        'ClearListToolStripMenuItem
        '
        Me.ClearListToolStripMenuItem.Name = "ClearListToolStripMenuItem"
        Me.ClearListToolStripMenuItem.Size = New System.Drawing.Size(179, 24)
        Me.ClearListToolStripMenuItem.Text = "Clear List"
        '
        'LvMsg
        '
        Me.LvMsg.AllowColumnReorder = True
        Me.LvMsg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.LvMsg.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
        Me.LvMsg.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LvMsg.Font = New System.Drawing.Font("Courier New", 8.25!)
        Me.LvMsg.FullRowSelect = True
        Me.LvMsg.GridLines = True
        Me.LvMsg.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable
        Me.LvMsg.HideSelection = False
        Me.LvMsg.LabelWrap = False
        Me.LvMsg.Location = New System.Drawing.Point(0, 0)
        Me.LvMsg.Margin = New System.Windows.Forms.Padding(0)
        Me.LvMsg.Name = "LvMsg"
        Me.LvMsg.Size = New System.Drawing.Size(1393, 350)
        Me.LvMsg.TabIndex = 0
        Me.LvMsg.UseCompatibleStateImageBehavior = False
        Me.LvMsg.View = System.Windows.Forms.View.Details
        '
        'ColumnHeader1
        '
        Me.ColumnHeader1.Text = "Message"
        Me.ColumnHeader1.Width = 779
        '
        'ColumnHeader2
        '
        Me.ColumnHeader2.Text = "Time"
        Me.ColumnHeader2.Width = 151
        '
        'ColumnHeader3
        '
        Me.ColumnHeader3.Text = "Source"
        Me.ColumnHeader3.Width = 138
        '
        'SplitContainer1
        '
        Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.SplitContainer1.Location = New System.Drawing.Point(0, 33)
        Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
        Me.SplitContainer1.Name = "SplitContainer1"
        Me.SplitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'SplitContainer1.Panel1
        '
        Me.SplitContainer1.Panel1.Controls.Add(Me.TabControl1)
        Me.SplitContainer1.Panel1.Padding = New System.Windows.Forms.Padding(5, 3, 0, 2)
        '
        'SplitContainer1.Panel2
        '
        Me.SplitContainer1.Panel2.Controls.Add(Me.LvMsg)
        Me.SplitContainer1.Size = New System.Drawing.Size(1393, 761)
        Me.SplitContainer1.SplitterDistance = 406
        Me.SplitContainer1.SplitterWidth = 5
        Me.SplitContainer1.TabIndex = 12
        '
        'ToolStrip1
        '
        Me.ToolStrip1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ToolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripSeparator2, Me.ToolStripDrDnBtTools, Me.ToolStripDrDnBtInfo})
        Me.ToolStrip1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.Size = New System.Drawing.Size(1393, 31)
        Me.ToolStrip1.TabIndex = 11
        Me.ToolStrip1.Text = "ToolStrip1"
        '
        'ToolStripBtNew
        '
        Me.ToolStripBtNew.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtNew.Image = Global.TUGraz.VECTO.My.Resources.Resources.blue_document_icon
        Me.ToolStripBtNew.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtNew.Name = "ToolStripBtNew"
        Me.ToolStripBtNew.Size = New System.Drawing.Size(29, 28)
        Me.ToolStripBtNew.Text = "ToolStripBtNew"
        Me.ToolStripBtNew.ToolTipText = "New Job File"
        '
        'ToolStripBtOpen
        '
        Me.ToolStripBtOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.ToolStripBtOpen.Image = Global.TUGraz.VECTO.My.Resources.Resources.Open_icon
        Me.ToolStripBtOpen.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
        Me.ToolStripBtOpen.Size = New System.Drawing.Size(29, 28)
        Me.ToolStripBtOpen.Text = "ToolStripButton1"
        Me.ToolStripBtOpen.ToolTipText = "Open File..."
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 31)
        '
        'ToolStripDrDnBtTools
        '
        Me.ToolStripDrDnBtTools.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.GENEditorToolStripMenuItem1, Me.JobEditorSerialHybridVehicleToolStripMenuItem, Me.JobEditorParallelHybridVehicleToolStripMenuItem, Me.JobEditorBatteryElectricVehicleToolStripMenuItem, Me.JobEditorIEPC_E_VehicleToolStripMenuItem, Me.JobEditorIEPC_S_VehicleToolStripMenuItem, Me.JobEditorIHPCVehicleToolStripMenuItem, Me.JobEditorEngineOnlyModeToolStripMenuItem, Me.EPTPJobEditorToolStripMenuItem, Me.VEHEditorToolStripMenuItem, Me.EngineEditorToolStripMenuItem, Me.GearboxEditorToolStripMenuItem, Me.GraphToolStripMenuItem, Me.ToolStripSeparator6, Me.OpenLogToolStripMenuItem, Me.SettingsToolStripMenuItem})
        Me.ToolStripDrDnBtTools.Image = Global.TUGraz.VECTO.My.Resources.Resources.Misc_Tools_icon
        Me.ToolStripDrDnBtTools.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDrDnBtTools.Name = "ToolStripDrDnBtTools"
        Me.ToolStripDrDnBtTools.Size = New System.Drawing.Size(82, 28)
        Me.ToolStripDrDnBtTools.Text = "Tools"
        '
        'GENEditorToolStripMenuItem1
        '
        Me.GENEditorToolStripMenuItem1.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.GENEditorToolStripMenuItem1.Name = "GENEditorToolStripMenuItem1"
        Me.GENEditorToolStripMenuItem1.Size = New System.Drawing.Size(323, 26)
        Me.GENEditorToolStripMenuItem1.Text = "Job Editor - Conventional Vehicle"
        '
        'JobEditorSerialHybridVehicleToolStripMenuItem
        '
        Me.JobEditorSerialHybridVehicleToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorSerialHybridVehicleToolStripMenuItem.Name = "JobEditorSerialHybridVehicleToolStripMenuItem"
        Me.JobEditorSerialHybridVehicleToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorSerialHybridVehicleToolStripMenuItem.Text = "Job Editor - Serial Hybrid Vehicle"
        '
        'JobEditorParallelHybridVehicleToolStripMenuItem
        '
        Me.JobEditorParallelHybridVehicleToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorParallelHybridVehicleToolStripMenuItem.Name = "JobEditorParallelHybridVehicleToolStripMenuItem"
        Me.JobEditorParallelHybridVehicleToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorParallelHybridVehicleToolStripMenuItem.Text = "Job Editor - Parallel Hybrid Vehicle"
        '
        'JobEditorBatteryElectricVehicleToolStripMenuItem
        '
        Me.JobEditorBatteryElectricVehicleToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorBatteryElectricVehicleToolStripMenuItem.Name = "JobEditorBatteryElectricVehicleToolStripMenuItem"
        Me.JobEditorBatteryElectricVehicleToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorBatteryElectricVehicleToolStripMenuItem.Text = "Job Editor - Battery Electric Vehicle"
        '
        'JobEditorIEPC_E_VehicleToolStripMenuItem
        '
        Me.JobEditorIEPC_E_VehicleToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorIEPC_E_VehicleToolStripMenuItem.Name = "JobEditorIEPC_E_VehicleToolStripMenuItem"
        Me.JobEditorIEPC_E_VehicleToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorIEPC_E_VehicleToolStripMenuItem.Text = "Job Editor - IEPC-E Vehicle"
        '
        'JobEditorIEPC_S_VehicleToolStripMenuItem
        '
        Me.JobEditorIEPC_S_VehicleToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorIEPC_S_VehicleToolStripMenuItem.Name = "JobEditorIEPC_S_VehicleToolStripMenuItem"
        Me.JobEditorIEPC_S_VehicleToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorIEPC_S_VehicleToolStripMenuItem.Text = "Job Editor - IEPC-S Vehicle"
        '
        'JobEditorIHPCVehicleToolStripMenuItem
        '
        Me.JobEditorIHPCVehicleToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorIHPCVehicleToolStripMenuItem.Name = "JobEditorIHPCVehicleToolStripMenuItem"
        Me.JobEditorIHPCVehicleToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorIHPCVehicleToolStripMenuItem.Text = "Job Editor - IHPC Vehicle"
        '
        'JobEditorEngineOnlyModeToolStripMenuItem
        '
        Me.JobEditorEngineOnlyModeToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.JobEditorEngineOnlyModeToolStripMenuItem.Name = "JobEditorEngineOnlyModeToolStripMenuItem"
        Me.JobEditorEngineOnlyModeToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.JobEditorEngineOnlyModeToolStripMenuItem.Text = "Job Editor - Engine Only Mode"
        '
        'EPTPJobEditorToolStripMenuItem
        '
        Me.EPTPJobEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VECTO
        Me.EPTPJobEditorToolStripMenuItem.Name = "EPTPJobEditorToolStripMenuItem"
        Me.EPTPJobEditorToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.EPTPJobEditorToolStripMenuItem.Text = "VTP Job Editor"
        '
        'VEHEditorToolStripMenuItem
        '
        Me.VEHEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_VEH
        Me.VEHEditorToolStripMenuItem.Name = "VEHEditorToolStripMenuItem"
        Me.VEHEditorToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.VEHEditorToolStripMenuItem.Text = "Vehicle Editor"
        '
        'EngineEditorToolStripMenuItem
        '
        Me.EngineEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_ENG
        Me.EngineEditorToolStripMenuItem.Name = "EngineEditorToolStripMenuItem"
        Me.EngineEditorToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.EngineEditorToolStripMenuItem.Text = "Engine Editor"
        '
        'GearboxEditorToolStripMenuItem
        '
        Me.GearboxEditorToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_GBX
        Me.GearboxEditorToolStripMenuItem.Name = "GearboxEditorToolStripMenuItem"
        Me.GearboxEditorToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.GearboxEditorToolStripMenuItem.Text = "Gearbox Editor"
        '
        'GraphToolStripMenuItem
        '
        Me.GraphToolStripMenuItem.Image = Global.TUGraz.VECTO.My.Resources.Resources.F_Graph
        Me.GraphToolStripMenuItem.Name = "GraphToolStripMenuItem"
        Me.GraphToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.GraphToolStripMenuItem.Text = "Graph"
        '
        'ToolStripSeparator6
        '
        Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
        Me.ToolStripSeparator6.Size = New System.Drawing.Size(320, 6)
        '
        'OpenLogToolStripMenuItem
        '
        Me.OpenLogToolStripMenuItem.Name = "OpenLogToolStripMenuItem"
        Me.OpenLogToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.OpenLogToolStripMenuItem.Text = "Open Log"
        '
        'SettingsToolStripMenuItem
        '
        Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
        Me.SettingsToolStripMenuItem.Size = New System.Drawing.Size(323, 26)
        Me.SettingsToolStripMenuItem.Text = "Settings"
        '
        'ToolStripDrDnBtInfo
        '
        Me.ToolStripDrDnBtInfo.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.UserManualToolStripMenuItem, Me.UpdateNotesToolStripMenuItem, Me.ReportBugViaCITnetToolStripMenuItem, Me.ToolStripSeparator3, Me.AboutVECTOToolStripMenuItem1})
        Me.ToolStripDrDnBtInfo.Image = Global.TUGraz.VECTO.My.Resources.Resources.Help_icon
        Me.ToolStripDrDnBtInfo.ImageTransparentColor = System.Drawing.Color.Magenta
        Me.ToolStripDrDnBtInfo.Name = "ToolStripDrDnBtInfo"
        Me.ToolStripDrDnBtInfo.Size = New System.Drawing.Size(79, 28)
        Me.ToolStripDrDnBtInfo.Text = "Help"
        '
        'UserManualToolStripMenuItem
        '
        Me.UserManualToolStripMenuItem.Name = "UserManualToolStripMenuItem"
        Me.UserManualToolStripMenuItem.Size = New System.Drawing.Size(296, 26)
        Me.UserManualToolStripMenuItem.Text = "User Manual"
        '
        'UpdateNotesToolStripMenuItem
        '
        Me.UpdateNotesToolStripMenuItem.Name = "UpdateNotesToolStripMenuItem"
        Me.UpdateNotesToolStripMenuItem.Size = New System.Drawing.Size(296, 26)
        Me.UpdateNotesToolStripMenuItem.Text = "Release Notes"
        '
        'ReportBugViaCITnetToolStripMenuItem
        '
        Me.ReportBugViaCITnetToolStripMenuItem.Name = "ReportBugViaCITnetToolStripMenuItem"
        Me.ReportBugViaCITnetToolStripMenuItem.Size = New System.Drawing.Size(302, 26)
        Me.ReportBugViaCITnetToolStripMenuItem.Text = "Report Issue via code.europa.eu"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(293, 6)
        '
        'AboutVECTOToolStripMenuItem1
        '
        Me.AboutVECTOToolStripMenuItem1.Name = "AboutVECTOToolStripMenuItem1"
        Me.AboutVECTOToolStripMenuItem1.Size = New System.Drawing.Size(296, 26)
        Me.AboutVECTOToolStripMenuItem1.Text = "About VECTO"
        '
        'CmDEV
        '
        Me.CmDEV.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmDEV.Name = "CmDEV"
        Me.CmDEV.ShowImageMargin = False
        Me.CmDEV.Size = New System.Drawing.Size(36, 4)
        '
        'TmProgSec
        '
        Me.TmProgSec.Interval = 1000
        '
        'CmOpenFile
        '
        Me.CmOpenFile.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.CmOpenFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.OpenInGraphWindowToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
        Me.CmOpenFile.Name = "CmOpenFile"
        Me.CmOpenFile.ShowImageMargin = False
        Me.CmOpenFile.Size = New System.Drawing.Size(209, 76)
        '
        'OpenWithToolStripMenuItem
        '
        Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
        Me.OpenWithToolStripMenuItem.Size = New System.Drawing.Size(208, 24)
        Me.OpenWithToolStripMenuItem.Text = "Open with ..."
        '
        'OpenInGraphWindowToolStripMenuItem
        '
        Me.OpenInGraphWindowToolStripMenuItem.Name = "OpenInGraphWindowToolStripMenuItem"
        Me.OpenInGraphWindowToolStripMenuItem.Size = New System.Drawing.Size(208, 24)
        Me.OpenInGraphWindowToolStripMenuItem.Text = "Open in Graph Window"
        '
        'ShowInFolderToolStripMenuItem
        '
        Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
        Me.ShowInFolderToolStripMenuItem.Size = New System.Drawing.Size(208, 24)
        Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
        '
        'MainForm
        '
        Me.AcceptButton = Me.btStartV3
        Me.AutoScaleDimensions = New System.Drawing.SizeF(8.0!, 16.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1393, 825)
        Me.Controls.Add(Me.ToolStrip1)
        Me.Controls.Add(Me.SplitContainer1)
        Me.Controls.Add(Me.StatusBAR)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(5, 3, 5, 3)
        Me.MinimumSize = New System.Drawing.Size(1041, 588)
        Me.Name = "MainForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "VECTO"
        Me.StatusBAR.ResumeLayout(false)
        Me.StatusBAR.PerformLayout
        Me.TabControl1.ResumeLayout(false)
        Me.TabPageGEN.ResumeLayout(false)
        Me.TabPageGEN.PerformLayout
        CType(Me.PictureBox1,System.ComponentModel.ISupportInitialize).EndInit
        Me.TabPgOptions.ResumeLayout(false)
        Me.PanelOptAllg.ResumeLayout(false)
        Me.GroupBox6.ResumeLayout(false)
        Me.GroupBox6.PerformLayout
        Me.GroupBox5.ResumeLayout(false)
        Me.GroupBox5.PerformLayout
        Me.GroupBox4.ResumeLayout(false)
        Me.GroupBox4.PerformLayout
        Me.GroupBox3.ResumeLayout(false)
        Me.GroupBox3.PerformLayout
        Me.GroupBox2.ResumeLayout(false)
        Me.GroupBox2.PerformLayout
        Me.GroupBox1.ResumeLayout(false)
        Me.GroupBox1.PerformLayout
        Me.ConMenFilelist.ResumeLayout(false)
        Me.SplitContainer1.Panel1.ResumeLayout(false)
        Me.SplitContainer1.Panel2.ResumeLayout(false)
        CType(Me.SplitContainer1,System.ComponentModel.ISupportInitialize).EndInit
        Me.SplitContainer1.ResumeLayout(false)
        Me.ToolStrip1.ResumeLayout(false)
        Me.ToolStrip1.PerformLayout
        Me.CmOpenFile.ResumeLayout(false)
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
    Friend WithEvents StatusBAR As StatusStrip
	Friend WithEvents ToolStripLbStatus As ToolStripStatusLabel
	Friend WithEvents TabControl1 As TabControl
	Friend WithEvents TabPageGEN As TabPage
	Friend WithEvents ButtonGENadd As Button
	Friend WithEvents ButtonGENremove As Button
	Friend WithEvents ConMenFilelist As ContextMenuStrip
	Friend WithEvents SaveListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LoadListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents LoadDefaultListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ClearListToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents BackgroundWorker1 As BackgroundWorker
	Friend WithEvents ToolStripProgBarOverall As ToolStripProgressBar
	Friend WithEvents LvGEN As ListView
	Friend WithEvents ColGENpath As ColumnHeader
	Friend WithEvents ColGENstatus As ColumnHeader
	Friend WithEvents ChBoxAllGEN As CheckBox
	Friend WithEvents TabPgOptions As TabPage
	Friend WithEvents ChBoxModOut As CheckBox
	Friend WithEvents PanelOptAllg As Panel
	Friend WithEvents LvMsg As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents ColumnHeader2 As ColumnHeader
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents CmDEV As ContextMenuStrip
	Friend WithEvents BtGENup As Button
	Friend WithEvents BtGENdown As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripDrDnBtTools As ToolStripDropDownButton
	Friend WithEvents GENEditorToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents VEHEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents OpenLogToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripDrDnBtInfo As ToolStripDropDownButton
	Friend WithEvents AboutVECTOToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
	Friend WithEvents ToolStripProgBarJob As ToolStripProgressBar
	Friend WithEvents TmProgSec As Timer
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents EngineEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents GearboxEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UserManualToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
	Friend WithEvents CmOpenFile As ContextMenuStrip
	Friend WithEvents OpenWithToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ShowInFolderToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents UpdateNotesToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
	Friend WithEvents LbDecl As Label
	Friend WithEvents GraphToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents OpenInGraphWindowToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents RbDev As RadioButton
	Friend WithEvents RbDecl As RadioButton
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents ReportBugViaCITnetToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents btStartV3 As Button
	Friend WithEvents ChBoxMod1Hz As CheckBox
	Friend WithEvents Label6 As Label
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents NewToolStripButton As ToolStripButton
	Friend WithEvents OpenToolStripButton As ToolStripButton
	Friend WithEvents SaveToolStripButton As ToolStripButton
	Friend WithEvents PrintToolStripButton As ToolStripButton
	Friend WithEvents toolStripSeparator As ToolStripSeparator
	Friend WithEvents CutToolStripButton As ToolStripButton
	Friend WithEvents CopyToolStripButton As ToolStripButton
	Friend WithEvents PasteToolStripButton As ToolStripButton
	Friend WithEvents toolStripSeparator1 As ToolStripSeparator
	Friend WithEvents HelpToolStripButton As ToolStripButton
	Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
	Friend WithEvents btnExportXML As System.Windows.Forms.Button
	Friend WithEvents btnImportXML As System.Windows.Forms.Button
	Friend WithEvents ShowInFolderMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents GroupBox3 As System.Windows.Forms.GroupBox
	Friend WithEvents cbValidateRunData As System.Windows.Forms.CheckBox
	Friend WithEvents cbActVmod As System.Windows.Forms.CheckBox
	Friend WithEvents EPTPJobEditorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GroupBox4 As GroupBox
    Friend WithEvents tbOutputFolder As TextBox
    Friend WithEvents BtTCfileBrowse As Button
    Friend WithEvents cbSaveVectoRunData As CheckBox
    Friend WithEvents Label2 As Label
    Friend WithEvents GroupBox5 As GroupBox
    Friend WithEvents Label4 As Label
    Friend WithEvents tbMinSpeedLAC As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents Label5 As Label
    Friend WithEvents JobEditorParallelHybridVehicleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JobEditorBatteryElectricVehicleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JobEditorEngineOnlyModeToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JobEditorSerialHybridVehicleToolStripMenuItem As ToolStripMenuItem

    Friend WithEvents JobEditorIEPC_E_VehicleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JobEditorIEPC_S_VehicleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents JobEditorIHPCVehicleToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GroupBox6 As GroupBox
    Friend WithEvents cbCSIteratingModeDeactivated As CheckBox
    Friend WithEvents Label1 As Label
    Friend WithEvents tbInitSOCinPercent As TextBox
    Friend WithEvents cbInitialSOC As CheckBox
End Class
