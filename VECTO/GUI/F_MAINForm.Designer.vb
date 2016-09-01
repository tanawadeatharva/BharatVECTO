' Copyright 2014 European Union.
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
Partial Class F_MAINForm
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
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(F_MAINForm))
		Me.StatusBAR = New StatusStrip()
		Me.ToolStripLbStatus = New ToolStripStatusLabel()
		Me.ToolStripProgBarJob = New ToolStripProgressBar()
		Me.ToolStripProgBarOverall = New ToolStripProgressBar()
		Me.TabControl1 = New TabControl()
		Me.TabPageGEN = New TabPage()
		Me.Label6 = New Label()
		Me.btStartV3 = New Button()
		Me.LbDecl = New Label()
		Me.PictureBox1 = New PictureBox()
		Me.BtGENdown = New Button()
		Me.BtGENup = New Button()
		Me.LbAutoShDown = New Label()
		Me.ChBoxAllGEN = New CheckBox()
		Me.LvGEN = New ListView()
		Me.ColGENpath = CType(New ColumnHeader(), ColumnHeader)
		Me.ColGENstatus = CType(New ColumnHeader(), ColumnHeader)
		Me.ButtonGENremove = New Button()
		Me.ButtonGENadd = New Button()
		Me.TabPageDRI = New TabPage()
		Me.Label3 = New Label()
		Me.BtDRIdown = New Button()
		Me.BtDRIup = New Button()
		Me.ChBoxAllDRI = New CheckBox()
		Me.LvDRI = New ListView()
		Me.ColDRIpath = CType(New ColumnHeader(), ColumnHeader)
		Me.ColDRIstatus = CType(New ColumnHeader(), ColumnHeader)
		Me.ButtonDRIremove = New Button()
		Me.ButtonDRIadd = New Button()
		Me.TabPgOptions = New TabPage()
		Me.GrBoxBATCH = New GroupBox()
		Me.ChBoxBatchSubD = New CheckBox()
		Me.Label2 = New Label()
		Me.ButBObrowse = New Button()
		Me.CbBOmode = New ComboBox()
		Me.TbBOpath = New TextBox()
		Me.GrBoxSTD = New GroupBox()
		Me.ChBoxAutoSD = New CheckBox()
		Me.PanelOptAllg = New Panel()
		Me.ChBoxMod1Hz = New CheckBox()
		Me.ChBoxModOut = New CheckBox()
		Me.GroupBox1 = New GroupBox()
		Me.RbDev = New RadioButton()
		Me.RbDecl = New RadioButton()
		Me.PnDeclOpt = New Panel()
		Me.CbBatch = New CheckBox()
		Me.ChBoxCyclDistCor = New CheckBox()
		Me.ChBoxUseGears = New CheckBox()
		Me.TabPageDEV = New TabPage()
		Me.Label1 = New Label()
		Me.LvDEVoptions = New ListView()
		Me.ColumnHeader4 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader7 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader5 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader6 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader8 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader9 = CType(New ColumnHeader(), ColumnHeader)
		Me.ConMenFilelist = New ContextMenuStrip(Me.components)
		Me.SaveListToolStripMenuItem = New ToolStripMenuItem()
		Me.LoadListToolStripMenuItem = New ToolStripMenuItem()
		Me.LoadDefaultListToolStripMenuItem = New ToolStripMenuItem()
		Me.ClearListToolStripMenuItem = New ToolStripMenuItem()
		Me.BackgroundWorker1 = New BackgroundWorker()
		Me.LvMsg = New ListView()
		Me.ColumnHeader1 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader2 = CType(New ColumnHeader(), ColumnHeader)
		Me.ColumnHeader3 = CType(New ColumnHeader(), ColumnHeader)
		Me.SplitContainer1 = New SplitContainer()
		Me.ToolStrip1 = New ToolStrip()
		Me.ToolStripBtNew = New ToolStripButton()
		Me.ToolStripBtOpen = New ToolStripButton()
		Me.ToolStripSeparator2 = New ToolStripSeparator()
		Me.ToolStripDrDnBtTools = New ToolStripDropDownButton()
		Me.GENEditorToolStripMenuItem1 = New ToolStripMenuItem()
		Me.VEHEditorToolStripMenuItem = New ToolStripMenuItem()
		Me.EngineEditorToolStripMenuItem = New ToolStripMenuItem()
		Me.GearboxEditorToolStripMenuItem = New ToolStripMenuItem()
		Me.GraphToolStripMenuItem = New ToolStripMenuItem()
		Me.ToolStripSeparator6 = New ToolStripSeparator()
		Me.SignOrVerifyFilesToolStripMenuItem = New ToolStripMenuItem()
		Me.ToolStripSeparator4 = New ToolStripSeparator()
		Me.OpenLogToolStripMenuItem = New ToolStripMenuItem()
		Me.SettingsToolStripMenuItem = New ToolStripMenuItem()
		Me.ToolStripDrDnBtInfo = New ToolStripDropDownButton()
		Me.UserManualToolStripMenuItem = New ToolStripMenuItem()
		Me.UpdateNotesToolStripMenuItem = New ToolStripMenuItem()
		Me.ReportBugViaCITnetToolStripMenuItem = New ToolStripMenuItem()
		Me.ToolStripSeparator3 = New ToolStripSeparator()
		Me.CreateActivationFileToolStripMenuItem = New ToolStripMenuItem()
		Me.AboutVECTOToolStripMenuItem1 = New ToolStripMenuItem()
		Me.CmDEV = New ContextMenuStrip(Me.components)
		Me.TmProgSec = New Timer(Me.components)
		Me.CmOpenFile = New ContextMenuStrip(Me.components)
		Me.OpenWithToolStripMenuItem = New ToolStripMenuItem()
		Me.OpenInGraphWindowToolStripMenuItem = New ToolStripMenuItem()
		Me.ShowInFolderToolStripMenuItem = New ToolStripMenuItem()
		Me.ToolTip1 = New ToolTip(Me.components)
		Me.StatusBAR.SuspendLayout()
		Me.TabControl1.SuspendLayout()
		Me.TabPageGEN.SuspendLayout()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		Me.TabPageDRI.SuspendLayout()
		Me.TabPgOptions.SuspendLayout()
		Me.GrBoxBATCH.SuspendLayout()
		Me.PanelOptAllg.SuspendLayout()
		Me.GroupBox1.SuspendLayout()
		Me.PnDeclOpt.SuspendLayout()
		Me.TabPageDEV.SuspendLayout()
		Me.ConMenFilelist.SuspendLayout()
		CType(Me.SplitContainer1, ISupportInitialize).BeginInit()
		Me.SplitContainer1.Panel1.SuspendLayout()
		Me.SplitContainer1.Panel2.SuspendLayout()
		Me.SplitContainer1.SuspendLayout()
		Me.ToolStrip1.SuspendLayout()
		Me.CmOpenFile.SuspendLayout()
		Me.SuspendLayout()
		'
		'StatusBAR
		'
		Me.StatusBAR.Items.AddRange(New ToolStripItem() {Me.ToolStripLbStatus, Me.ToolStripProgBarJob, Me.ToolStripProgBarOverall})
		Me.StatusBAR.Location = New Point(0, 648)
		Me.StatusBAR.Name = "StatusBAR"
		Me.StatusBAR.Size = New Size(1045, 22)
		Me.StatusBAR.TabIndex = 7
		Me.StatusBAR.Text = "StatusBAR"
		'
		'ToolStripLbStatus
		'
		Me.ToolStripLbStatus.Name = "ToolStripLbStatus"
		Me.ToolStripLbStatus.Size = New Size(1030, 17)
		Me.ToolStripLbStatus.Spring = True
		Me.ToolStripLbStatus.Text = "Status Text"
		Me.ToolStripLbStatus.TextAlign = ContentAlignment.MiddleLeft
		'
		'ToolStripProgBarJob
		'
		Me.ToolStripProgBarJob.Alignment = ToolStripItemAlignment.Right
		Me.ToolStripProgBarJob.AutoSize = False
		Me.ToolStripProgBarJob.Name = "ToolStripProgBarJob"
		Me.ToolStripProgBarJob.Size = New Size(100, 16)
		Me.ToolStripProgBarJob.Style = ProgressBarStyle.Continuous
		Me.ToolStripProgBarJob.ToolTipText = "overall progress"
		Me.ToolStripProgBarJob.Visible = False
		'
		'ToolStripProgBarOverall
		'
		Me.ToolStripProgBarOverall.Alignment = ToolStripItemAlignment.Right
		Me.ToolStripProgBarOverall.AutoSize = False
		Me.ToolStripProgBarOverall.Name = "ToolStripProgBarOverall"
		Me.ToolStripProgBarOverall.Size = New Size(100, 16)
		Me.ToolStripProgBarOverall.Style = ProgressBarStyle.Continuous
		Me.ToolStripProgBarOverall.ToolTipText = "job progress"
		Me.ToolStripProgBarOverall.Visible = False
		'
		'TabControl1
		'
		Me.TabControl1.Controls.Add(Me.TabPageGEN)
		Me.TabControl1.Controls.Add(Me.TabPageDRI)
		Me.TabControl1.Controls.Add(Me.TabPgOptions)
		Me.TabControl1.Controls.Add(Me.TabPageDEV)
		Me.TabControl1.Dock = DockStyle.Fill
		Me.TabControl1.Location = New Point(3, 3)
		Me.TabControl1.Margin = New Padding(0)
		Me.TabControl1.Name = "TabControl1"
		Me.TabControl1.Padding = New Point(0, 0)
		Me.TabControl1.SelectedIndex = 0
		Me.TabControl1.Size = New Size(1042, 328)
		Me.TabControl1.TabIndex = 10
		'
		'TabPageGEN
		'
		Me.TabPageGEN.Controls.Add(Me.Label6)
		Me.TabPageGEN.Controls.Add(Me.btStartV3)
		Me.TabPageGEN.Controls.Add(Me.LbDecl)
		Me.TabPageGEN.Controls.Add(Me.PictureBox1)
		Me.TabPageGEN.Controls.Add(Me.BtGENdown)
		Me.TabPageGEN.Controls.Add(Me.BtGENup)
		Me.TabPageGEN.Controls.Add(Me.LbAutoShDown)
		Me.TabPageGEN.Controls.Add(Me.ChBoxAllGEN)
		Me.TabPageGEN.Controls.Add(Me.LvGEN)
		Me.TabPageGEN.Controls.Add(Me.ButtonGENremove)
		Me.TabPageGEN.Controls.Add(Me.ButtonGENadd)
		Me.TabPageGEN.Location = New Point(4, 22)
		Me.TabPageGEN.Margin = New Padding(0)
		Me.TabPageGEN.Name = "TabPageGEN"
		Me.TabPageGEN.Size = New Size(1034, 302)
		Me.TabPageGEN.TabIndex = 0
		Me.TabPageGEN.Text = "Job Files"
		Me.TabPageGEN.UseVisualStyleBackColor = True
		'
		'Label6
		'
		Me.Label6.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.Label6.AutoSize = True
		Me.Label6.Location = New Point(814, 268)
		Me.Label6.Name = "Label6"
		Me.Label6.Size = New Size(217, 13)
		Me.Label6.TabIndex = 21
		Me.Label6.Text = "(Double-Click to Edit, Right-Click for Options)"
		'
		'btStartV3
		'
		Me.btStartV3.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.btStartV3.Image = My.Resources.Resources.Play_icon
		Me.btStartV3.ImageAlign = ContentAlignment.MiddleLeft
		Me.btStartV3.Location = New Point(3, 56)
		Me.btStartV3.Name = "btStartV3"
		Me.btStartV3.Size = New Size(108, 50)
		Me.btStartV3.TabIndex = 20
		Me.btStartV3.Text = "START"
		Me.btStartV3.TextImageRelation = TextImageRelation.ImageBeforeText
		Me.ToolTip1.SetToolTip(Me.btStartV3, "Start Simulation")
		Me.btStartV3.UseVisualStyleBackColor = True
		'
		'LbDecl
		'
		Me.LbDecl.AutoSize = True
		Me.LbDecl.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.LbDecl.Location = New Point(5, 109)
		Me.LbDecl.Name = "LbDecl"
		Me.LbDecl.Size = New Size(107, 13)
		Me.LbDecl.TabIndex = 19
		Me.LbDecl.Text = "Declaration Mode"
		Me.LbDecl.Visible = False
		'
		'PictureBox1
		'
		Me.PictureBox1.Image = CType(resources.GetObject("PictureBox1.Image"), Image)
		Me.PictureBox1.Location = New Point(3, 3)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(108, 47)
		Me.PictureBox1.TabIndex = 18
		Me.PictureBox1.TabStop = False
		'
		'BtGENdown
		'
		Me.BtGENdown.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtGENdown.Image = My.Resources.Resources.Actions_arrow_down_icon
		Me.BtGENdown.Location = New Point(307, 267)
		Me.BtGENdown.Name = "BtGENdown"
		Me.BtGENdown.Size = New Size(30, 30)
		Me.BtGENdown.TabIndex = 6
		Me.ToolTip1.SetToolTip(Me.BtGENdown, "Move job down one row")
		Me.BtGENdown.UseVisualStyleBackColor = True
		'
		'BtGENup
		'
		Me.BtGENup.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtGENup.Image = My.Resources.Resources.Actions_arrow_up_icon
		Me.BtGENup.Location = New Point(276, 267)
		Me.BtGENup.Name = "BtGENup"
		Me.BtGENup.Size = New Size(30, 30)
		Me.BtGENup.TabIndex = 4
		Me.ToolTip1.SetToolTip(Me.BtGENup, "Move job up one row")
		Me.BtGENup.UseVisualStyleBackColor = True
		'
		'LbAutoShDown
		'
		Me.LbAutoShDown.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.LbAutoShDown.AutoSize = True
		Me.LbAutoShDown.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.LbAutoShDown.ForeColor = Color.Red
		Me.LbAutoShDown.Location = New Point(408, 275)
		Me.LbAutoShDown.Name = "LbAutoShDown"
		Me.LbAutoShDown.Size = New Size(225, 13)
		Me.LbAutoShDown.TabIndex = 17
		Me.LbAutoShDown.Text = "!!! Automatic Shutdown is activated !!!"
		Me.LbAutoShDown.Visible = False
		'
		'ChBoxAllGEN
		'
		Me.ChBoxAllGEN.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.ChBoxAllGEN.AutoSize = True
		Me.ChBoxAllGEN.Location = New Point(195, 274)
		Me.ChBoxAllGEN.Name = "ChBoxAllGEN"
		Me.ChBoxAllGEN.Size = New Size(70, 17)
		Me.ChBoxAllGEN.TabIndex = 16
		Me.ChBoxAllGEN.Text = "Select All"
		Me.ToolTip1.SetToolTip(Me.ChBoxAllGEN, "Select All / None")
		Me.ChBoxAllGEN.UseVisualStyleBackColor = True
		'
		'LvGEN
		'
		Me.LvGEN.AllowDrop = True
		Me.LvGEN.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.LvGEN.CheckBoxes = True
		Me.LvGEN.Columns.AddRange(New ColumnHeader() {Me.ColGENpath, Me.ColGENstatus})
		Me.LvGEN.FullRowSelect = True
		Me.LvGEN.GridLines = True
		Me.LvGEN.HeaderStyle = ColumnHeaderStyle.Nonclickable
		Me.LvGEN.HideSelection = False
		Me.LvGEN.LabelEdit = True
		Me.LvGEN.Location = New Point(114, 3)
		Me.LvGEN.Name = "LvGEN"
		Me.LvGEN.Size = New Size(917, 263)
		Me.LvGEN.TabIndex = 14
		Me.LvGEN.UseCompatibleStateImageBehavior = False
		Me.LvGEN.View = View.Details
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
		Me.ButtonGENremove.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.ButtonGENremove.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.ButtonGENremove.Image = My.Resources.Resources.minus_circle_icon
		Me.ButtonGENremove.Location = New Point(147, 267)
		Me.ButtonGENremove.Name = "ButtonGENremove"
		Me.ButtonGENremove.Size = New Size(33, 30)
		Me.ButtonGENremove.TabIndex = 2
		Me.ToolTip1.SetToolTip(Me.ButtonGENremove, "Remove selected entries")
		Me.ButtonGENremove.UseVisualStyleBackColor = True
		'
		'ButtonGENadd
		'
		Me.ButtonGENadd.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.ButtonGENadd.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.ButtonGENadd.Image = My.Resources.Resources.plus_circle_icon
		Me.ButtonGENadd.Location = New Point(113, 267)
		Me.ButtonGENadd.Name = "ButtonGENadd"
		Me.ButtonGENadd.Size = New Size(33, 30)
		Me.ButtonGENadd.TabIndex = 1
		Me.ToolTip1.SetToolTip(Me.ButtonGENadd, "Add Job File")
		Me.ButtonGENadd.UseVisualStyleBackColor = True
		'
		'TabPageDRI
		'
		Me.TabPageDRI.Controls.Add(Me.Label3)
		Me.TabPageDRI.Controls.Add(Me.BtDRIdown)
		Me.TabPageDRI.Controls.Add(Me.BtDRIup)
		Me.TabPageDRI.Controls.Add(Me.ChBoxAllDRI)
		Me.TabPageDRI.Controls.Add(Me.LvDRI)
		Me.TabPageDRI.Controls.Add(Me.ButtonDRIremove)
		Me.TabPageDRI.Controls.Add(Me.ButtonDRIadd)
		Me.TabPageDRI.Location = New Point(4, 22)
		Me.TabPageDRI.Name = "TabPageDRI"
		Me.TabPageDRI.Padding = New Padding(3)
		Me.TabPageDRI.Size = New Size(1034, 302)
		Me.TabPageDRI.TabIndex = 1
		Me.TabPageDRI.Text = "Driving Cycles"
		Me.TabPageDRI.UseVisualStyleBackColor = True
		'
		'Label3
		'
		Me.Label3.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(814, 268)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(217, 13)
		Me.Label3.TabIndex = 22
		Me.Label3.Text = "(Double-Click to Edit, Right-Click for Options)"
		'
		'BtDRIdown
		'
		Me.BtDRIdown.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtDRIdown.Image = My.Resources.Resources.Actions_arrow_down_icon
		Me.BtDRIdown.Location = New Point(196, 268)
		Me.BtDRIdown.Name = "BtDRIdown"
		Me.BtDRIdown.Size = New Size(33, 30)
		Me.BtDRIdown.TabIndex = 3
		Me.BtDRIdown.UseVisualStyleBackColor = True
		'
		'BtDRIup
		'
		Me.BtDRIup.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.BtDRIup.Image = My.Resources.Resources.Actions_arrow_up_icon
		Me.BtDRIup.Location = New Point(163, 268)
		Me.BtDRIup.Name = "BtDRIup"
		Me.BtDRIup.Size = New Size(33, 30)
		Me.BtDRIup.TabIndex = 2
		Me.BtDRIup.UseVisualStyleBackColor = True
		'
		'ChBoxAllDRI
		'
		Me.ChBoxAllDRI.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.ChBoxAllDRI.AutoSize = True
		Me.ChBoxAllDRI.Location = New Point(85, 275)
		Me.ChBoxAllDRI.Name = "ChBoxAllDRI"
		Me.ChBoxAllDRI.Size = New Size(70, 17)
		Me.ChBoxAllDRI.TabIndex = 7
		Me.ChBoxAllDRI.Text = "Select All"
		Me.ChBoxAllDRI.UseVisualStyleBackColor = True
		'
		'LvDRI
		'
		Me.LvDRI.AllowDrop = True
		Me.LvDRI.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.LvDRI.CheckBoxes = True
		Me.LvDRI.Columns.AddRange(New ColumnHeader() {Me.ColDRIpath, Me.ColDRIstatus})
		Me.LvDRI.FullRowSelect = True
		Me.LvDRI.GridLines = True
		Me.LvDRI.HeaderStyle = ColumnHeaderStyle.Nonclickable
		Me.LvDRI.HideSelection = False
		Me.LvDRI.LabelEdit = True
		Me.LvDRI.Location = New Point(6, 6)
		Me.LvDRI.Name = "LvDRI"
		Me.LvDRI.Size = New Size(1022, 261)
		Me.LvDRI.TabIndex = 6
		Me.LvDRI.UseCompatibleStateImageBehavior = False
		Me.LvDRI.View = View.Details
		'
		'ColDRIpath
		'
		Me.ColDRIpath.Text = "Filepath"
		Me.ColDRIpath.Width = 915
		'
		'ColDRIstatus
		'
		Me.ColDRIstatus.Text = ""
		Me.ColDRIstatus.Width = 150
		'
		'ButtonDRIremove
		'
		Me.ButtonDRIremove.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.ButtonDRIremove.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.ButtonDRIremove.Image = My.Resources.Resources.minus_circle_icon
		Me.ButtonDRIremove.Location = New Point(38, 268)
		Me.ButtonDRIremove.Name = "ButtonDRIremove"
		Me.ButtonDRIremove.Size = New Size(33, 30)
		Me.ButtonDRIremove.TabIndex = 1
		Me.ButtonDRIremove.UseVisualStyleBackColor = True
		'
		'ButtonDRIadd
		'
		Me.ButtonDRIadd.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.ButtonDRIadd.Font = New Font("Microsoft Sans Serif", 8.25!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.ButtonDRIadd.Image = My.Resources.Resources.plus_circle_icon
		Me.ButtonDRIadd.Location = New Point(5, 268)
		Me.ButtonDRIadd.Name = "ButtonDRIadd"
		Me.ButtonDRIadd.Size = New Size(33, 30)
		Me.ButtonDRIadd.TabIndex = 0
		Me.ButtonDRIadd.UseVisualStyleBackColor = True
		'
		'TabPgOptions
		'
		Me.TabPgOptions.Controls.Add(Me.GrBoxBATCH)
		Me.TabPgOptions.Controls.Add(Me.GrBoxSTD)
		Me.TabPgOptions.Controls.Add(Me.ChBoxAutoSD)
		Me.TabPgOptions.Controls.Add(Me.PanelOptAllg)
		Me.TabPgOptions.Location = New Point(4, 22)
		Me.TabPgOptions.Name = "TabPgOptions"
		Me.TabPgOptions.Padding = New Padding(3)
		Me.TabPgOptions.Size = New Size(1034, 302)
		Me.TabPgOptions.TabIndex = 2
		Me.TabPgOptions.Text = "Options"
		Me.TabPgOptions.UseVisualStyleBackColor = True
		'
		'GrBoxBATCH
		'
		Me.GrBoxBATCH.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.GrBoxBATCH.Controls.Add(Me.ChBoxBatchSubD)
		Me.GrBoxBATCH.Controls.Add(Me.Label2)
		Me.GrBoxBATCH.Controls.Add(Me.ButBObrowse)
		Me.GrBoxBATCH.Controls.Add(Me.CbBOmode)
		Me.GrBoxBATCH.Controls.Add(Me.TbBOpath)
		Me.GrBoxBATCH.Location = New Point(6, 224)
		Me.GrBoxBATCH.Name = "GrBoxBATCH"
		Me.GrBoxBATCH.Size = New Size(1022, 72)
		Me.GrBoxBATCH.TabIndex = 5
		Me.GrBoxBATCH.TabStop = False
		Me.GrBoxBATCH.Text = "Batch Options"
		'
		'ChBoxBatchSubD
		'
		Me.ChBoxBatchSubD.AutoSize = True
		Me.ChBoxBatchSubD.Location = New Point(14, 46)
		Me.ChBoxBatchSubD.Name = "ChBoxBatchSubD"
		Me.ChBoxBatchSubD.Size = New Size(206, 17)
		Me.ChBoxBatchSubD.TabIndex = 4
		Me.ChBoxBatchSubD.Text = "Create Subdirectories for modal results"
		Me.ChBoxBatchSubD.UseVisualStyleBackColor = True
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(11, 22)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(64, 13)
		Me.Label2.TabIndex = 2
		Me.Label2.Text = "Output Path"
		'
		'ButBObrowse
		'
		Me.ButBObrowse.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ButBObrowse.Image = My.Resources.Resources.Open_icon
		Me.ButBObrowse.Location = New Point(961, 16)
		Me.ButBObrowse.Name = "ButBObrowse"
		Me.ButBObrowse.Size = New Size(24, 24)
		Me.ButBObrowse.TabIndex = 3
		Me.ButBObrowse.UseVisualStyleBackColor = True
		'
		'CbBOmode
		'
		Me.CbBOmode.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbBOmode.FormattingEnabled = True
		Me.CbBOmode.Items.AddRange(New Object() {"Directory of .vecto File", "Custom Directory"})
		Me.CbBOmode.Location = New Point(81, 19)
		Me.CbBOmode.Name = "CbBOmode"
		Me.CbBOmode.Size = New Size(140, 21)
		Me.CbBOmode.TabIndex = 0
		'
		'TbBOpath
		'
		Me.TbBOpath.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TbBOpath.Location = New Point(227, 18)
		Me.TbBOpath.Name = "TbBOpath"
		Me.TbBOpath.Size = New Size(734, 20)
		Me.TbBOpath.TabIndex = 1
		'
		'GrBoxSTD
		'
		Me.GrBoxSTD.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.GrBoxSTD.Location = New Point(6, 224)
		Me.GrBoxSTD.Name = "GrBoxSTD"
		Me.GrBoxSTD.Size = New Size(1022, 72)
		Me.GrBoxSTD.TabIndex = 14
		Me.GrBoxSTD.TabStop = False
		Me.GrBoxSTD.Text = "Standard Options"
		'
		'ChBoxAutoSD
		'
		Me.ChBoxAutoSD.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ChBoxAutoSD.AutoSize = True
		Me.ChBoxAutoSD.Location = New Point(859, 6)
		Me.ChBoxAutoSD.Name = "ChBoxAutoSD"
		Me.ChBoxAutoSD.Size = New Size(169, 17)
		Me.ChBoxAutoSD.TabIndex = 13
		Me.ChBoxAutoSD.Text = "Shutdown system after last job"
		Me.ChBoxAutoSD.UseVisualStyleBackColor = True
		'
		'PanelOptAllg
		'
		Me.PanelOptAllg.Controls.Add(Me.ChBoxMod1Hz)
		Me.PanelOptAllg.Controls.Add(Me.ChBoxModOut)
		Me.PanelOptAllg.Controls.Add(Me.GroupBox1)
		Me.PanelOptAllg.Controls.Add(Me.PnDeclOpt)
		Me.PanelOptAllg.Location = New Point(6, 6)
		Me.PanelOptAllg.Name = "PanelOptAllg"
		Me.PanelOptAllg.Size = New Size(519, 212)
		Me.PanelOptAllg.TabIndex = 0
		'
		'ChBoxMod1Hz
		'
		Me.ChBoxMod1Hz.AutoSize = True
		Me.ChBoxMod1Hz.Location = New Point(9, 182)
		Me.ChBoxMod1Hz.Name = "ChBoxMod1Hz"
		Me.ChBoxMod1Hz.Size = New Size(121, 17)
		Me.ChBoxMod1Hz.TabIndex = 16
		Me.ChBoxMod1Hz.Text = "Modal results in 1Hz"
		Me.ChBoxMod1Hz.UseVisualStyleBackColor = True
		'
		'ChBoxModOut
		'
		Me.ChBoxModOut.AutoSize = True
		Me.ChBoxModOut.Checked = True
		Me.ChBoxModOut.CheckState = CheckState.Checked
		Me.ChBoxModOut.Location = New Point(9, 159)
		Me.ChBoxModOut.Name = "ChBoxModOut"
		Me.ChBoxModOut.Size = New Size(115, 17)
		Me.ChBoxModOut.TabIndex = 0
		Me.ChBoxModOut.Text = "Write modal results"
		Me.ChBoxModOut.UseVisualStyleBackColor = True
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.RbDev)
		Me.GroupBox1.Controls.Add(Me.RbDecl)
		Me.GroupBox1.Location = New Point(3, 3)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(121, 72)
		Me.GroupBox1.TabIndex = 15
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Mode"
		'
		'RbDev
		'
		Me.RbDev.AutoSize = True
		Me.RbDev.Checked = True
		Me.RbDev.Location = New Point(6, 42)
		Me.RbDev.Name = "RbDev"
		Me.RbDev.Size = New Size(111, 17)
		Me.RbDev.TabIndex = 1
		Me.RbDev.TabStop = True
		Me.RbDev.Text = "Engineering Mode"
		Me.RbDev.UseVisualStyleBackColor = True
		'
		'RbDecl
		'
		Me.RbDecl.AutoSize = True
		Me.RbDecl.Location = New Point(6, 19)
		Me.RbDecl.Name = "RbDecl"
		Me.RbDecl.Size = New Size(109, 17)
		Me.RbDecl.TabIndex = 0
		Me.RbDecl.TabStop = True
		Me.RbDecl.Text = "Declaration Mode"
		Me.RbDecl.UseVisualStyleBackColor = True
		'
		'PnDeclOpt
		'
		Me.PnDeclOpt.Controls.Add(Me.CbBatch)
		Me.PnDeclOpt.Controls.Add(Me.ChBoxCyclDistCor)
		Me.PnDeclOpt.Controls.Add(Me.ChBoxUseGears)
		Me.PnDeclOpt.Location = New Point(3, 81)
		Me.PnDeclOpt.Name = "PnDeclOpt"
		Me.PnDeclOpt.Size = New Size(202, 72)
		Me.PnDeclOpt.TabIndex = 13
		'
		'CbBatch
		'
		Me.CbBatch.AutoSize = True
		Me.CbBatch.Location = New Point(6, 3)
		Me.CbBatch.Name = "CbBatch"
		Me.CbBatch.Size = New Size(84, 17)
		Me.CbBatch.TabIndex = 15
		Me.CbBatch.Text = "Batch Mode"
		Me.CbBatch.UseVisualStyleBackColor = True
		'
		'ChBoxCyclDistCor
		'
		Me.ChBoxCyclDistCor.AutoSize = True
		Me.ChBoxCyclDistCor.Location = New Point(6, 26)
		Me.ChBoxCyclDistCor.Name = "ChBoxCyclDistCor"
		Me.ChBoxCyclDistCor.Size = New Size(148, 17)
		Me.ChBoxCyclDistCor.TabIndex = 0
		Me.ChBoxCyclDistCor.Text = "Cycle Distance Correction"
		Me.ChBoxCyclDistCor.UseVisualStyleBackColor = True
		'
		'ChBoxUseGears
		'
		Me.ChBoxUseGears.AutoSize = True
		Me.ChBoxUseGears.Location = New Point(6, 49)
		Me.ChBoxUseGears.Name = "ChBoxUseGears"
		Me.ChBoxUseGears.Size = New Size(188, 17)
		Me.ChBoxUseGears.TabIndex = 0
		Me.ChBoxUseGears.Text = "Use gears/rpm's form driving cycle"
		Me.ChBoxUseGears.UseVisualStyleBackColor = True
		'
		'TabPageDEV
		'
		Me.TabPageDEV.Controls.Add(Me.Label1)
		Me.TabPageDEV.Controls.Add(Me.LvDEVoptions)
		Me.TabPageDEV.Location = New Point(4, 22)
		Me.TabPageDEV.Name = "TabPageDEV"
		Me.TabPageDEV.Padding = New Padding(3)
		Me.TabPageDEV.Size = New Size(1034, 302)
		Me.TabPageDEV.TabIndex = 3
		Me.TabPageDEV.Text = "Test"
		Me.TabPageDEV.UseVisualStyleBackColor = True
		'
		'Label1
		'
		Me.Label1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(1012, 283)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(106, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "(Double-Click to Edit)"
		'
		'LvDEVoptions
		'
		Me.LvDEVoptions.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.LvDEVoptions.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader4, Me.ColumnHeader7, Me.ColumnHeader5, Me.ColumnHeader6, Me.ColumnHeader8, Me.ColumnHeader9})
		Me.LvDEVoptions.FullRowSelect = True
		Me.LvDEVoptions.GridLines = True
		Me.LvDEVoptions.Location = New Point(6, 6)
		Me.LvDEVoptions.MultiSelect = False
		Me.LvDEVoptions.Name = "LvDEVoptions"
		Me.LvDEVoptions.Size = New Size(1022, 277)
		Me.LvDEVoptions.TabIndex = 0
		Me.LvDEVoptions.UseCompatibleStateImageBehavior = False
		Me.LvDEVoptions.View = View.Details
		'
		'ColumnHeader4
		'
		Me.ColumnHeader4.Text = "Property"
		Me.ColumnHeader4.Width = 89
		'
		'ColumnHeader7
		'
		Me.ColumnHeader7.Text = "Description"
		Me.ColumnHeader7.Width = 527
		'
		'ColumnHeader5
		'
		Me.ColumnHeader5.Text = "Type"
		Me.ColumnHeader5.Width = 82
		'
		'ColumnHeader6
		'
		Me.ColumnHeader6.Text = "Value"
		Me.ColumnHeader6.Width = 134
		'
		'ColumnHeader8
		'
		Me.ColumnHeader8.Text = "Default"
		Me.ColumnHeader8.Width = 120
		'
		'ColumnHeader9
		'
		Me.ColumnHeader9.Text = "Saved In DEVconfig.txt"
		Me.ColumnHeader9.Width = 129
		'
		'ConMenFilelist
		'
		Me.ConMenFilelist.Items.AddRange(New ToolStripItem() {Me.SaveListToolStripMenuItem, Me.LoadListToolStripMenuItem, Me.LoadDefaultListToolStripMenuItem, Me.ClearListToolStripMenuItem})
		Me.ConMenFilelist.Name = "ConMenFilelist"
		Me.ConMenFilelist.Size = New Size(176, 92)
		'
		'SaveListToolStripMenuItem
		'
		Me.SaveListToolStripMenuItem.Name = "SaveListToolStripMenuItem"
		Me.SaveListToolStripMenuItem.Size = New Size(175, 22)
		Me.SaveListToolStripMenuItem.Text = "Save List..."
		'
		'LoadListToolStripMenuItem
		'
		Me.LoadListToolStripMenuItem.Name = "LoadListToolStripMenuItem"
		Me.LoadListToolStripMenuItem.Size = New Size(175, 22)
		Me.LoadListToolStripMenuItem.Text = "Load List..."
		'
		'LoadDefaultListToolStripMenuItem
		'
		Me.LoadDefaultListToolStripMenuItem.Name = "LoadDefaultListToolStripMenuItem"
		Me.LoadDefaultListToolStripMenuItem.Size = New Size(175, 22)
		Me.LoadDefaultListToolStripMenuItem.Text = "Load Autosave-List"
		'
		'ClearListToolStripMenuItem
		'
		Me.ClearListToolStripMenuItem.Name = "ClearListToolStripMenuItem"
		Me.ClearListToolStripMenuItem.Size = New Size(175, 22)
		Me.ClearListToolStripMenuItem.Text = "Clear List"
		'
		'BackgroundWorker1
		'
		'
		'LvMsg
		'
		Me.LvMsg.AllowColumnReorder = True
		Me.LvMsg.BorderStyle = BorderStyle.FixedSingle
		Me.LvMsg.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader1, Me.ColumnHeader2, Me.ColumnHeader3})
		Me.LvMsg.Dock = DockStyle.Fill
		Me.LvMsg.Font = New Font("Courier New", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.LvMsg.FullRowSelect = True
		Me.LvMsg.GridLines = True
		Me.LvMsg.HeaderStyle = ColumnHeaderStyle.Nonclickable
		Me.LvMsg.LabelWrap = False
		Me.LvMsg.Location = New Point(0, 0)
		Me.LvMsg.Margin = New Padding(0)
		Me.LvMsg.Name = "LvMsg"
		Me.LvMsg.Size = New Size(1045, 281)
		Me.LvMsg.TabIndex = 0
		Me.LvMsg.UseCompatibleStateImageBehavior = False
		Me.LvMsg.View = View.Details
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
		Me.SplitContainer1.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.SplitContainer1.Location = New Point(0, 27)
		Me.SplitContainer1.Margin = New Padding(0)
		Me.SplitContainer1.Name = "SplitContainer1"
		Me.SplitContainer1.Orientation = Orientation.Horizontal
		'
		'SplitContainer1.Panel1
		'
		Me.SplitContainer1.Panel1.Controls.Add(Me.TabControl1)
		Me.SplitContainer1.Panel1.Padding = New Padding(3, 3, 0, 2)
		'
		'SplitContainer1.Panel2
		'
		Me.SplitContainer1.Panel2.Controls.Add(Me.LvMsg)
		Me.SplitContainer1.Size = New Size(1045, 618)
		Me.SplitContainer1.SplitterDistance = 333
		Me.SplitContainer1.TabIndex = 12
		'
		'ToolStrip1
		'
		Me.ToolStrip1.BackgroundImageLayout = ImageLayout.None
		Me.ToolStrip1.GripStyle = ToolStripGripStyle.Hidden
		Me.ToolStrip1.Items.AddRange(New ToolStripItem() {Me.ToolStripBtNew, Me.ToolStripBtOpen, Me.ToolStripSeparator2, Me.ToolStripDrDnBtTools, Me.ToolStripDrDnBtInfo})
		Me.ToolStrip1.Location = New Point(0, 0)
		Me.ToolStrip1.Name = "ToolStrip1"
		Me.ToolStrip1.Size = New Size(1045, 25)
		Me.ToolStrip1.TabIndex = 11
		Me.ToolStrip1.Text = "ToolStrip1"
		'
		'ToolStripBtNew
		'
		Me.ToolStripBtNew.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtNew.Image = My.Resources.Resources.blue_document_icon
		Me.ToolStripBtNew.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtNew.Name = "ToolStripBtNew"
		Me.ToolStripBtNew.Size = New Size(23, 22)
		Me.ToolStripBtNew.Text = "ToolStripBtNew"
		Me.ToolStripBtNew.ToolTipText = "New Job File"
		'
		'ToolStripBtOpen
		'
		Me.ToolStripBtOpen.DisplayStyle = ToolStripItemDisplayStyle.Image
		Me.ToolStripBtOpen.Image = My.Resources.Resources.Open_icon
		Me.ToolStripBtOpen.ImageTransparentColor = Color.Magenta
		Me.ToolStripBtOpen.Name = "ToolStripBtOpen"
		Me.ToolStripBtOpen.Size = New Size(23, 22)
		Me.ToolStripBtOpen.Text = "ToolStripButton1"
		Me.ToolStripBtOpen.ToolTipText = "Open File..."
		'
		'ToolStripSeparator2
		'
		Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
		Me.ToolStripSeparator2.Size = New Size(6, 25)
		'
		'ToolStripDrDnBtTools
		'
		Me.ToolStripDrDnBtTools.DropDownItems.AddRange(New ToolStripItem() {Me.GENEditorToolStripMenuItem1, Me.VEHEditorToolStripMenuItem, Me.EngineEditorToolStripMenuItem, Me.GearboxEditorToolStripMenuItem, Me.GraphToolStripMenuItem, Me.ToolStripSeparator6, Me.SignOrVerifyFilesToolStripMenuItem, Me.ToolStripSeparator4, Me.OpenLogToolStripMenuItem, Me.SettingsToolStripMenuItem})
		Me.ToolStripDrDnBtTools.Image = My.Resources.Resources.Misc_Tools_icon
		Me.ToolStripDrDnBtTools.ImageTransparentColor = Color.Magenta
		Me.ToolStripDrDnBtTools.Name = "ToolStripDrDnBtTools"
		Me.ToolStripDrDnBtTools.Size = New Size(65, 22)
		Me.ToolStripDrDnBtTools.Text = "Tools"
		'
		'GENEditorToolStripMenuItem1
		'
		Me.GENEditorToolStripMenuItem1.Image = My.Resources.Resources.F_VECTO
		Me.GENEditorToolStripMenuItem1.Name = "GENEditorToolStripMenuItem1"
		Me.GENEditorToolStripMenuItem1.Size = New Size(170, 22)
		Me.GENEditorToolStripMenuItem1.Text = "Job Editor"
		'
		'VEHEditorToolStripMenuItem
		'
		Me.VEHEditorToolStripMenuItem.Image = My.Resources.Resources.F_VEH
		Me.VEHEditorToolStripMenuItem.Name = "VEHEditorToolStripMenuItem"
		Me.VEHEditorToolStripMenuItem.Size = New Size(170, 22)
		Me.VEHEditorToolStripMenuItem.Text = "Vehicle Editor"
		'
		'EngineEditorToolStripMenuItem
		'
		Me.EngineEditorToolStripMenuItem.Image = My.Resources.Resources.F_ENG
		Me.EngineEditorToolStripMenuItem.Name = "EngineEditorToolStripMenuItem"
		Me.EngineEditorToolStripMenuItem.Size = New Size(170, 22)
		Me.EngineEditorToolStripMenuItem.Text = "Engine Editor"
		'
		'GearboxEditorToolStripMenuItem
		'
		Me.GearboxEditorToolStripMenuItem.Image = My.Resources.Resources.F_GBX
		Me.GearboxEditorToolStripMenuItem.Name = "GearboxEditorToolStripMenuItem"
		Me.GearboxEditorToolStripMenuItem.Size = New Size(170, 22)
		Me.GearboxEditorToolStripMenuItem.Text = "Gearbox Editor"
		'
		'GraphToolStripMenuItem
		'
		Me.GraphToolStripMenuItem.Image = My.Resources.Resources.F_Graph
		Me.GraphToolStripMenuItem.Name = "GraphToolStripMenuItem"
		Me.GraphToolStripMenuItem.Size = New Size(170, 22)
		Me.GraphToolStripMenuItem.Text = "Graph"
		'
		'ToolStripSeparator6
		'
		Me.ToolStripSeparator6.Name = "ToolStripSeparator6"
		Me.ToolStripSeparator6.Size = New Size(167, 6)
		'
		'SignOrVerifyFilesToolStripMenuItem
		'
		Me.SignOrVerifyFilesToolStripMenuItem.Image = My.Resources.Resources.Status_dialog_password_icon
		Me.SignOrVerifyFilesToolStripMenuItem.Name = "SignOrVerifyFilesToolStripMenuItem"
		Me.SignOrVerifyFilesToolStripMenuItem.Size = New Size(170, 22)
		Me.SignOrVerifyFilesToolStripMenuItem.Text = "Sign or Verify Files"
		'
		'ToolStripSeparator4
		'
		Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
		Me.ToolStripSeparator4.Size = New Size(167, 6)
		Me.ToolStripSeparator4.Visible = False
		'
		'OpenLogToolStripMenuItem
		'
		Me.OpenLogToolStripMenuItem.Name = "OpenLogToolStripMenuItem"
		Me.OpenLogToolStripMenuItem.Size = New Size(170, 22)
		Me.OpenLogToolStripMenuItem.Text = "Open Log"
		'
		'SettingsToolStripMenuItem
		'
		Me.SettingsToolStripMenuItem.Name = "SettingsToolStripMenuItem"
		Me.SettingsToolStripMenuItem.Size = New Size(170, 22)
		Me.SettingsToolStripMenuItem.Text = "Settings"
		'
		'ToolStripDrDnBtInfo
		'
		Me.ToolStripDrDnBtInfo.DropDownItems.AddRange(New ToolStripItem() {Me.UserManualToolStripMenuItem, Me.UpdateNotesToolStripMenuItem, Me.ReportBugViaCITnetToolStripMenuItem, Me.ToolStripSeparator3, Me.CreateActivationFileToolStripMenuItem, Me.AboutVECTOToolStripMenuItem1})
		Me.ToolStripDrDnBtInfo.Image = My.Resources.Resources.Help_icon
		Me.ToolStripDrDnBtInfo.ImageTransparentColor = Color.Magenta
		Me.ToolStripDrDnBtInfo.Name = "ToolStripDrDnBtInfo"
		Me.ToolStripDrDnBtInfo.Size = New Size(61, 22)
		Me.ToolStripDrDnBtInfo.Text = "Help"
		'
		'UserManualToolStripMenuItem
		'
		Me.UserManualToolStripMenuItem.Name = "UserManualToolStripMenuItem"
		Me.UserManualToolStripMenuItem.Size = New Size(222, 22)
		Me.UserManualToolStripMenuItem.Text = "User Manual"
		'
		'UpdateNotesToolStripMenuItem
		'
		Me.UpdateNotesToolStripMenuItem.Name = "UpdateNotesToolStripMenuItem"
		Me.UpdateNotesToolStripMenuItem.Size = New Size(222, 22)
		Me.UpdateNotesToolStripMenuItem.Text = "Release Notes"
		'
		'ReportBugViaCITnetToolStripMenuItem
		'
		Me.ReportBugViaCITnetToolStripMenuItem.Name = "ReportBugViaCITnetToolStripMenuItem"
		Me.ReportBugViaCITnetToolStripMenuItem.Size = New Size(222, 22)
		Me.ReportBugViaCITnetToolStripMenuItem.Text = "Report Bug via CITnet / JIRA"
		'
		'ToolStripSeparator3
		'
		Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
		Me.ToolStripSeparator3.Size = New Size(219, 6)
		'
		'CreateActivationFileToolStripMenuItem
		'
		Me.CreateActivationFileToolStripMenuItem.Name = "CreateActivationFileToolStripMenuItem"
		Me.CreateActivationFileToolStripMenuItem.Size = New Size(222, 22)
		Me.CreateActivationFileToolStripMenuItem.Text = "Create Activation File"
		'
		'AboutVECTOToolStripMenuItem1
		'
		Me.AboutVECTOToolStripMenuItem1.Name = "AboutVECTOToolStripMenuItem1"
		Me.AboutVECTOToolStripMenuItem1.Size = New Size(222, 22)
		Me.AboutVECTOToolStripMenuItem1.Text = "About VECTO"
		'
		'CmDEV
		'
		Me.CmDEV.Name = "CmDEV"
		Me.CmDEV.Size = New Size(61, 4)
		'
		'TmProgSec
		'
		Me.TmProgSec.Interval = 1000
		'
		'CmOpenFile
		'
		Me.CmOpenFile.Items.AddRange(New ToolStripItem() {Me.OpenWithToolStripMenuItem, Me.OpenInGraphWindowToolStripMenuItem, Me.ShowInFolderToolStripMenuItem})
		Me.CmOpenFile.Name = "CmOpenFile"
		Me.CmOpenFile.Size = New Size(199, 70)
		'
		'OpenWithToolStripMenuItem
		'
		Me.OpenWithToolStripMenuItem.Name = "OpenWithToolStripMenuItem"
		Me.OpenWithToolStripMenuItem.Size = New Size(198, 22)
		Me.OpenWithToolStripMenuItem.Text = "Open with ..."
		'
		'OpenInGraphWindowToolStripMenuItem
		'
		Me.OpenInGraphWindowToolStripMenuItem.Name = "OpenInGraphWindowToolStripMenuItem"
		Me.OpenInGraphWindowToolStripMenuItem.Size = New Size(198, 22)
		Me.OpenInGraphWindowToolStripMenuItem.Text = "Open in Graph Window"
		'
		'ShowInFolderToolStripMenuItem
		'
		Me.ShowInFolderToolStripMenuItem.Name = "ShowInFolderToolStripMenuItem"
		Me.ShowInFolderToolStripMenuItem.Size = New Size(198, 22)
		Me.ShowInFolderToolStripMenuItem.Text = "Show in Folder"
		'
		'F_MAINForm
		'
		Me.AcceptButton = Me.btStartV3
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.ClientSize = New Size(1045, 670)
		Me.Controls.Add(Me.ToolStrip1)
		Me.Controls.Add(Me.SplitContainer1)
		Me.Controls.Add(Me.StatusBAR)
		Me.Icon = CType(resources.GetObject("$this.Icon"), Icon)
		Me.MinimumSize = New Size(785, 485)
		Me.Name = "F_MAINForm"
		Me.Text = "VECTO"
		Me.StatusBAR.ResumeLayout(False)
		Me.StatusBAR.PerformLayout()
		Me.TabControl1.ResumeLayout(False)
		Me.TabPageGEN.ResumeLayout(False)
		Me.TabPageGEN.PerformLayout()
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		Me.TabPageDRI.ResumeLayout(False)
		Me.TabPageDRI.PerformLayout()
		Me.TabPgOptions.ResumeLayout(False)
		Me.TabPgOptions.PerformLayout()
		Me.GrBoxBATCH.ResumeLayout(False)
		Me.GrBoxBATCH.PerformLayout()
		Me.PanelOptAllg.ResumeLayout(False)
		Me.PanelOptAllg.PerformLayout()
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.PnDeclOpt.ResumeLayout(False)
		Me.PnDeclOpt.PerformLayout()
		Me.TabPageDEV.ResumeLayout(False)
		Me.TabPageDEV.PerformLayout()
		Me.ConMenFilelist.ResumeLayout(False)
		Me.SplitContainer1.Panel1.ResumeLayout(False)
		Me.SplitContainer1.Panel2.ResumeLayout(False)
		CType(Me.SplitContainer1, ISupportInitialize).EndInit()
		Me.SplitContainer1.ResumeLayout(False)
		Me.ToolStrip1.ResumeLayout(False)
		Me.ToolStrip1.PerformLayout()
		Me.CmOpenFile.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents StatusBAR As StatusStrip
	Friend WithEvents ToolStripLbStatus As ToolStripStatusLabel
	Friend WithEvents TabControl1 As TabControl
	Friend WithEvents TabPageGEN As TabPage
	Friend WithEvents TabPageDRI As TabPage
	Friend WithEvents ButtonGENadd As Button
	Friend WithEvents ButtonGENremove As Button
	Friend WithEvents ButtonDRIremove As Button
	Friend WithEvents ButtonDRIadd As Button
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
	Friend WithEvents LvDRI As ListView
	Friend WithEvents ColDRIpath As ColumnHeader
	Friend WithEvents ColDRIstatus As ColumnHeader
	Friend WithEvents ChBoxAllGEN As CheckBox
	Friend WithEvents ChBoxAllDRI As CheckBox
	Friend WithEvents TabPgOptions As TabPage
	Friend WithEvents ChBoxModOut As CheckBox
	Friend WithEvents ChBoxUseGears As CheckBox
	Friend WithEvents ChBoxCyclDistCor As CheckBox
	Friend WithEvents PanelOptAllg As Panel
	Friend WithEvents LbAutoShDown As Label
	Friend WithEvents ChBoxAutoSD As CheckBox
	Friend WithEvents TbBOpath As TextBox
	Friend WithEvents CbBOmode As ComboBox
	Friend WithEvents Label2 As Label
	Friend WithEvents ButBObrowse As Button
	Friend WithEvents ChBoxBatchSubD As CheckBox
	Friend WithEvents LvMsg As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents ColumnHeader2 As ColumnHeader
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents GrBoxBATCH As GroupBox
	Friend WithEvents TabPageDEV As TabPage
	Friend WithEvents LvDEVoptions As ListView
	Friend WithEvents ColumnHeader4 As ColumnHeader
	Friend WithEvents ColumnHeader5 As ColumnHeader
	Friend WithEvents ColumnHeader6 As ColumnHeader
	Friend WithEvents CmDEV As ContextMenuStrip
	Friend WithEvents ColumnHeader7 As ColumnHeader
	Friend WithEvents BtGENup As Button
	Friend WithEvents BtGENdown As Button
	Friend WithEvents BtDRIdown As Button
	Friend WithEvents BtDRIup As Button
	Friend WithEvents ToolStrip1 As ToolStrip
	Friend WithEvents ToolStripBtNew As ToolStripButton
	Friend WithEvents ToolStripBtOpen As ToolStripButton
	Friend WithEvents ToolStripDrDnBtTools As ToolStripDropDownButton
	Friend WithEvents GENEditorToolStripMenuItem1 As ToolStripMenuItem
	Friend WithEvents VEHEditorToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
	Friend WithEvents OpenLogToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents SettingsToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripDrDnBtInfo As ToolStripDropDownButton
	Friend WithEvents CreateActivationFileToolStripMenuItem As ToolStripMenuItem
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
	Friend WithEvents ColumnHeader8 As ColumnHeader
	Friend WithEvents ColumnHeader9 As ColumnHeader
	Friend WithEvents UpdateNotesToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents SignOrVerifyFilesToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ToolStripSeparator6 As ToolStripSeparator
	Friend WithEvents GrBoxSTD As GroupBox
	Friend WithEvents PnDeclOpt As Panel
	Friend WithEvents LbDecl As Label
	Friend WithEvents GraphToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents OpenInGraphWindowToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents CbBatch As CheckBox
	Friend WithEvents RbDev As RadioButton
	Friend WithEvents RbDecl As RadioButton
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents Label1 As Label
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
	Friend WithEvents Label3 As Label

End Class
