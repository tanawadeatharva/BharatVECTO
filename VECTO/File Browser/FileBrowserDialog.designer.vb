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
Partial Class FileBrowserDialog
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FileBrowserDialog))
		Me.SplitContainer1 = New System.Windows.Forms.SplitContainer()
		Me.ListViewFolder = New System.Windows.Forms.ListView()
		Me.ColumnHeader3 = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
		Me.ImageList1 = New System.Windows.Forms.ImageList(Me.components)
		Me.Label1 = New System.Windows.Forms.Label()
		Me.ButtonNewDir = New System.Windows.Forms.Button()
		Me.ButtonDesktop = New System.Windows.Forms.Button()
		Me.ButtonHisFolder = New System.Windows.Forms.Button()
		Me.ButtonFolderBack = New System.Windows.Forms.Button()
		Me.TextBoxSearchFolder = New System.Windows.Forms.TextBox()
		Me.ComboBoxDrive = New System.Windows.Forms.ComboBox()
		Me.ListViewFiles = New System.Windows.Forms.ListView()
		Me.ColumnFileName = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
		Me.ColumnType = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
		Me.ColumnSize = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
		Me.ColumnChangedOn = CType(New System.Windows.Forms.ColumnHeader(),System.Windows.Forms.ColumnHeader)
		Me.ContextMenuFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.RenameFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.DeleteFileToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.Label2 = New System.Windows.Forms.Label()
		Me.ButtonAll = New System.Windows.Forms.Button()
		Me.ComboBoxExt = New System.Windows.Forms.ComboBox()
		Me.ButtonHisFile = New System.Windows.Forms.Button()
		Me.TextBoxSearchFile = New System.Windows.Forms.TextBox()
		Me.Label5 = New System.Windows.Forms.Label()
		Me.TextBoxPath = New System.Windows.Forms.TextBox()
		Me.ContextMenuHisFolder = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.ContextMenuHisFile = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.ButtonOK = New System.Windows.Forms.Button()
		Me.ButtonCancel = New System.Windows.Forms.Button()
		Me.TextBoxCurrent = New System.Windows.Forms.TextBox()
		Me.Label3 = New System.Windows.Forms.Label()
		Me.Label4 = New System.Windows.Forms.Label()
		Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
		CType(Me.SplitContainer1,System.ComponentModel.ISupportInitialize).BeginInit
		Me.SplitContainer1.Panel1.SuspendLayout
		Me.SplitContainer1.Panel2.SuspendLayout
		Me.SplitContainer1.SuspendLayout
		Me.ContextMenuFile.SuspendLayout
		Me.SuspendLayout
		'
		'SplitContainer1
		'
		Me.SplitContainer1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.SplitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
		Me.SplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
		Me.SplitContainer1.Location = New System.Drawing.Point(-1, 46)
		Me.SplitContainer1.Margin = New System.Windows.Forms.Padding(0)
		Me.SplitContainer1.Name = "SplitContainer1"
		'
		'SplitContainer1.Panel1
		'
		Me.SplitContainer1.Panel1.Controls.Add(Me.ListViewFolder)
		Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonNewDir)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonDesktop)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonHisFolder)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonFolderBack)
		Me.SplitContainer1.Panel1.Controls.Add(Me.TextBoxSearchFolder)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxDrive)
		Me.SplitContainer1.Panel1MinSize = 179
		'
		'SplitContainer1.Panel2
		'
		Me.SplitContainer1.Panel2.Controls.Add(Me.ListViewFiles)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonAll)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ComboBoxExt)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonHisFile)
		Me.SplitContainer1.Panel2.Controls.Add(Me.TextBoxSearchFile)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label5)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
		Me.SplitContainer1.Size = New System.Drawing.Size(713, 273)
		Me.SplitContainer1.SplitterDistance = 179
		Me.SplitContainer1.TabIndex = 5
		Me.SplitContainer1.TabStop = false
		'
		'ListViewFolder
		'
		Me.ListViewFolder.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ListViewFolder.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnHeader3})
		Me.ListViewFolder.FullRowSelect = true
		Me.ListViewFolder.GridLines = true
		Me.ListViewFolder.HideSelection = false
		Me.ListViewFolder.Location = New System.Drawing.Point(-1, 22)
		Me.ListViewFolder.MultiSelect = false
		Me.ListViewFolder.Name = "ListViewFolder"
		Me.ListViewFolder.Size = New System.Drawing.Size(179, 232)
		Me.ListViewFolder.SmallImageList = Me.ImageList1
		Me.ListViewFolder.TabIndex = 10
		Me.ListViewFolder.UseCompatibleStateImageBehavior = false
		Me.ListViewFolder.View = System.Windows.Forms.View.Details
		'
		'ColumnHeader3
		'
		Me.ColumnHeader3.Text = "Sub-Directories:"
		Me.ColumnHeader3.Width = 368
		'
		'ImageList1
		'
		Me.ImageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit
		Me.ImageList1.ImageSize = New System.Drawing.Size(16, 16)
		Me.ImageList1.TransparentColor = System.Drawing.Color.Transparent
		'
		'Label1
		'
		Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.Label1.AutoSize = true
		Me.Label1.Location = New System.Drawing.Point(2, 257)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(32, 13)
		Me.Label1.TabIndex = 28
		Me.Label1.Text = "Filter:"
		'
		'ButtonNewDir
		'
		Me.ButtonNewDir.Image = Global.TUGraz.VECTO.My.Resources.Resources.new_dir
		Me.ButtonNewDir.Location = New System.Drawing.Point(103, -2)
		Me.ButtonNewDir.Name = "ButtonNewDir"
		Me.ButtonNewDir.Size = New System.Drawing.Size(26, 25)
		Me.ButtonNewDir.TabIndex = 21
		Me.ButtonNewDir.TabStop = false
		Me.ToolTip1.SetToolTip(Me.ButtonNewDir, "Create new Directory")
		Me.ButtonNewDir.UseVisualStyleBackColor = true
		'
		'ButtonDesktop
		'
		Me.ButtonDesktop.Image = Global.TUGraz.VECTO.My.Resources.Resources.desktop
		Me.ButtonDesktop.Location = New System.Drawing.Point(78, -2)
		Me.ButtonDesktop.Name = "ButtonDesktop"
		Me.ButtonDesktop.Size = New System.Drawing.Size(26, 25)
		Me.ButtonDesktop.TabIndex = 22
		Me.ButtonDesktop.TabStop = false
		Me.ToolTip1.SetToolTip(Me.ButtonDesktop, "Go to Desktop")
		Me.ButtonDesktop.UseVisualStyleBackColor = true
		'
		'ButtonHisFolder
		'
		Me.ButtonHisFolder.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ButtonHisFolder.Image = Global.TUGraz.VECTO.My.Resources.Resources.file_history
		Me.ButtonHisFolder.Location = New System.Drawing.Point(153, -2)
		Me.ButtonHisFolder.Name = "ButtonHisFolder"
		Me.ButtonHisFolder.Size = New System.Drawing.Size(26, 25)
		Me.ButtonHisFolder.TabIndex = 24
		Me.ButtonHisFolder.TabStop = false
		Me.ToolTip1.SetToolTip(Me.ButtonHisFolder, "History / Favorites")
		Me.ButtonHisFolder.UseVisualStyleBackColor = true
		'
		'ButtonFolderBack
		'
		Me.ButtonFolderBack.Image = CType(resources.GetObject("ButtonFolderBack.Image"),System.Drawing.Image)
		Me.ButtonFolderBack.Location = New System.Drawing.Point(53, -2)
		Me.ButtonFolderBack.Name = "ButtonFolderBack"
		Me.ButtonFolderBack.Size = New System.Drawing.Size(26, 25)
		Me.ButtonFolderBack.TabIndex = 20
		Me.ButtonFolderBack.TabStop = false
		Me.ToolTip1.SetToolTip(Me.ButtonFolderBack, "Move up one directory")
		Me.ButtonFolderBack.UseVisualStyleBackColor = true
		'
		'TextBoxSearchFolder
		'
		Me.TextBoxSearchFolder.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.TextBoxSearchFolder.Location = New System.Drawing.Point(35, 252)
		Me.TextBoxSearchFolder.Name = "TextBoxSearchFolder"
		Me.TextBoxSearchFolder.Size = New System.Drawing.Size(143, 20)
		Me.TextBoxSearchFolder.TabIndex = 15
		Me.ToolTip1.SetToolTip(Me.TextBoxSearchFolder, "Filter the directories")
		'
		'ComboBoxDrive
		'
		Me.ComboBoxDrive.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
		Me.ComboBoxDrive.Font = New System.Drawing.Font("Segoe UI", 11!)
		Me.ComboBoxDrive.FormattingEnabled = true
		Me.ComboBoxDrive.Location = New System.Drawing.Point(0, -1)
		Me.ComboBoxDrive.Name = "ComboBoxDrive"
		Me.ComboBoxDrive.Size = New System.Drawing.Size(54, 28)
		Me.ComboBoxDrive.TabIndex = 5
		'
		'ListViewFiles
		'
		Me.ListViewFiles.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom)  _
            Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ListViewFiles.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.ColumnFileName, Me.ColumnType, Me.ColumnSize, Me.ColumnChangedOn})
		Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFile
		Me.ListViewFiles.FullRowSelect = true
		Me.ListViewFiles.GridLines = true
		Me.ListViewFiles.HideSelection = false
		Me.ListViewFiles.Location = New System.Drawing.Point(-1, 22)
		Me.ListViewFiles.Name = "ListViewFiles"
		Me.ListViewFiles.Size = New System.Drawing.Size(530, 232)
		Me.ListViewFiles.SmallImageList = Me.ImageList1
		Me.ListViewFiles.TabIndex = 10
		Me.ListViewFiles.UseCompatibleStateImageBehavior = false
		Me.ListViewFiles.View = System.Windows.Forms.View.Details
		'
		'ColumnFileName
		'
		Me.ColumnFileName.Text = "Files:"
		Me.ColumnFileName.Width = 191
		'
		'ColumnType
		'
		Me.ColumnType.Text = "Type"
		'
		'ColumnSize
		'
		Me.ColumnSize.Text = "Size"
		Me.ColumnSize.Width = 48
		'
		'ColumnChangedOn
		'
		Me.ColumnChangedOn.Text = "Changed on"
		Me.ColumnChangedOn.Width = 42
		'
		'ContextMenuFile
		'
		Me.ContextMenuFile.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.RenameFileToolStripMenuItem, Me.DeleteFileToolStripMenuItem})
		Me.ContextMenuFile.Name = "ContextMenuFile"
		Me.ContextMenuFile.Size = New System.Drawing.Size(148, 48)
		'
		'RenameFileToolStripMenuItem
		'
		Me.RenameFileToolStripMenuItem.Name = "RenameFileToolStripMenuItem"
		Me.RenameFileToolStripMenuItem.Size = New System.Drawing.Size(147, 22)
		Me.RenameFileToolStripMenuItem.Text = "Rename File..."
		'
		'DeleteFileToolStripMenuItem
		'
		Me.DeleteFileToolStripMenuItem.Name = "DeleteFileToolStripMenuItem"
		Me.DeleteFileToolStripMenuItem.Size = New System.Drawing.Size(147, 22)
		Me.DeleteFileToolStripMenuItem.Text = "Delete File..."
		'
		'Label2
		'
		Me.Label2.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.Label2.AutoSize = true
		Me.Label2.Location = New System.Drawing.Point(2, 257)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New System.Drawing.Size(32, 13)
		Me.Label2.TabIndex = 29
		Me.Label2.Text = "Filter:"
		'
		'ButtonAll
		'
		Me.ButtonAll.Location = New System.Drawing.Point(-2, -2)
		Me.ButtonAll.Name = "ButtonAll"
		Me.ButtonAll.Size = New System.Drawing.Size(71, 25)
		Me.ButtonAll.TabIndex = 19
		Me.ButtonAll.Text = "Select All"
		Me.ToolTip1.SetToolTip(Me.ButtonAll, "Select all shown files")
		Me.ButtonAll.UseVisualStyleBackColor = true
		'
		'ComboBoxExt
		'
		Me.ComboBoxExt.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ComboBoxExt.FormattingEnabled = true
		Me.ComboBoxExt.Location = New System.Drawing.Point(433, 252)
		Me.ComboBoxExt.Name = "ComboBoxExt"
		Me.ComboBoxExt.Size = New System.Drawing.Size(96, 21)
		Me.ComboBoxExt.TabIndex = 20
		Me.ToolTip1.SetToolTip(Me.ComboBoxExt, "Filter the file type")
		'
		'ButtonHisFile
		'
		Me.ButtonHisFile.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ButtonHisFile.Image = Global.TUGraz.VECTO.My.Resources.Resources.file_history
		Me.ButtonHisFile.Location = New System.Drawing.Point(504, -2)
		Me.ButtonHisFile.Name = "ButtonHisFile"
		Me.ButtonHisFile.Size = New System.Drawing.Size(26, 25)
		Me.ButtonHisFile.TabIndex = 24
		Me.ButtonHisFile.TabStop = false
		Me.ToolTip1.SetToolTip(Me.ButtonHisFile, "History")
		Me.ButtonHisFile.UseVisualStyleBackColor = true
		'
		'TextBoxSearchFile
		'
		Me.TextBoxSearchFile.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.TextBoxSearchFile.Location = New System.Drawing.Point(33, 252)
		Me.TextBoxSearchFile.Name = "TextBoxSearchFile"
		Me.TextBoxSearchFile.Size = New System.Drawing.Size(389, 20)
		Me.TextBoxSearchFile.TabIndex = 15
		Me.ToolTip1.SetToolTip(Me.TextBoxSearchFile, "Filter the files")
		'
		'Label5
		'
		Me.Label5.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.Label5.AutoSize = true
		Me.Label5.Location = New System.Drawing.Point(423, 257)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New System.Drawing.Size(10, 13)
		Me.Label5.TabIndex = 30
		Me.Label5.Text = "."
		'
		'TextBoxPath
		'
		Me.TextBoxPath.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.TextBoxPath.Font = New System.Drawing.Font("Segoe UI", 11!)
		Me.TextBoxPath.Location = New System.Drawing.Point(46, 329)
		Me.TextBoxPath.Name = "TextBoxPath"
		Me.TextBoxPath.Size = New System.Drawing.Size(666, 27)
		Me.TextBoxPath.TabIndex = 15
		'
		'ContextMenuHisFolder
		'
		Me.ContextMenuHisFolder.Name = "ContextMenuFolderHis"
		Me.ContextMenuHisFolder.Size = New System.Drawing.Size(61, 4)
		'
		'ContextMenuHisFile
		'
		Me.ContextMenuHisFile.Name = "ContextMenuFileHis"
		Me.ContextMenuHisFile.Size = New System.Drawing.Size(61, 4)
		'
		'ButtonOK
		'
		Me.ButtonOK.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ButtonOK.DialogResult = System.Windows.Forms.DialogResult.OK
		Me.ButtonOK.Location = New System.Drawing.Point(550, 364)
		Me.ButtonOK.Name = "ButtonOK"
		Me.ButtonOK.Size = New System.Drawing.Size(75, 23)
		Me.ButtonOK.TabIndex = 20
		Me.ButtonOK.TabStop = false
		Me.ButtonOK.Text = "OK"
		Me.ButtonOK.UseVisualStyleBackColor = true
		'
		'ButtonCancel
		'
		Me.ButtonCancel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.ButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
		Me.ButtonCancel.Location = New System.Drawing.Point(628, 364)
		Me.ButtonCancel.Name = "ButtonCancel"
		Me.ButtonCancel.Size = New System.Drawing.Size(75, 23)
		Me.ButtonCancel.TabIndex = 25
		Me.ButtonCancel.TabStop = false
		Me.ButtonCancel.Text = "Cancel"
		Me.ButtonCancel.UseVisualStyleBackColor = true
		'
		'TextBoxCurrent
		'
		Me.TextBoxCurrent.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
		Me.TextBoxCurrent.Cursor = System.Windows.Forms.Cursors.Hand
		Me.TextBoxCurrent.Location = New System.Drawing.Point(5, 17)
		Me.TextBoxCurrent.Name = "TextBoxCurrent"
		Me.TextBoxCurrent.ReadOnly = true
		Me.TextBoxCurrent.Size = New System.Drawing.Size(701, 20)
		Me.TextBoxCurrent.TabIndex = 0
		Me.TextBoxCurrent.TabStop = false
		Me.ToolTip1.SetToolTip(Me.TextBoxCurrent, "Click for changing the directory")
		'
		'Label3
		'
		Me.Label3.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left),System.Windows.Forms.AnchorStyles)
		Me.Label3.AutoSize = true
		Me.Label3.Location = New System.Drawing.Point(5, 335)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New System.Drawing.Size(38, 13)
		Me.Label3.TabIndex = 29
		Me.Label3.Text = "Name:"
		'
		'Label4
		'
		Me.Label4.AutoSize = true
		Me.Label4.Location = New System.Drawing.Point(4, 3)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New System.Drawing.Size(89, 13)
		Me.Label4.TabIndex = 30
		Me.Label4.Text = "Current Directory:"
		'
		'FileBrowserDialog
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.CancelButton = Me.ButtonCancel
		Me.ClientSize = New System.Drawing.Size(711, 393)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.TextBoxCurrent)
		Me.Controls.Add(Me.ButtonCancel)
		Me.Controls.Add(Me.ButtonOK)
		Me.Controls.Add(Me.TextBoxPath)
		Me.Controls.Add(Me.SplitContainer1)
		Me.MinimizeBox = false
		Me.MinimumSize = New System.Drawing.Size(341, 272)
		Me.Name = "FileBrowserDialog"
		Me.ShowIcon = false
		Me.ShowInTaskbar = false
		Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
		Me.Text = "File Browser"
		Me.SplitContainer1.Panel1.ResumeLayout(false)
		Me.SplitContainer1.Panel1.PerformLayout
		Me.SplitContainer1.Panel2.ResumeLayout(false)
		Me.SplitContainer1.Panel2.PerformLayout
		CType(Me.SplitContainer1,System.ComponentModel.ISupportInitialize).EndInit
		Me.SplitContainer1.ResumeLayout(false)
		Me.ContextMenuFile.ResumeLayout(false)
		Me.ResumeLayout(false)
		Me.PerformLayout

End Sub
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents ComboBoxDrive As ComboBox
	Friend WithEvents ListViewFolder As ListView
	Friend WithEvents ListViewFiles As ListView
	Friend WithEvents ColumnFileName As ColumnHeader
	Friend WithEvents TextBoxSearchFolder As TextBox
	Friend WithEvents TextBoxPath As TextBox
	Friend WithEvents ButtonFolderBack As Button
	Friend WithEvents ContextMenuHisFolder As ContextMenuStrip
	Friend WithEvents ContextMenuHisFile As ContextMenuStrip
	Friend WithEvents ButtonHisFolder As Button
	Friend WithEvents ButtonHisFile As Button
	Friend WithEvents ButtonOK As Button
	Friend WithEvents ButtonCancel As Button
	Friend WithEvents TextBoxCurrent As TextBox
	Friend WithEvents ColumnHeader3 As ColumnHeader
	Friend WithEvents ButtonDesktop As Button
	Friend WithEvents ContextMenuFile As ContextMenuStrip
	Friend WithEvents RenameFileToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents DeleteFileToolStripMenuItem As ToolStripMenuItem
	Friend WithEvents ButtonNewDir As Button
	Friend WithEvents ButtonAll As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents ImageList1 As ImageList
	Friend WithEvents ToolTip1 As ToolTip
	Friend WithEvents Label5 As Label
	Friend WithEvents Label2 As Label
	Friend WithEvents ComboBoxExt As ComboBox
	Friend WithEvents TextBoxSearchFile As TextBox
	Friend WithEvents ColumnType As ColumnHeader
	Friend WithEvents ColumnSize As ColumnHeader
	Friend WithEvents ColumnChangedOn As ColumnHeader
End Class
