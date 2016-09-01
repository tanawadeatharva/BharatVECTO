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
Partial Class FB_Dialog
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
		Dim resources As ComponentResourceManager = New ComponentResourceManager(GetType(FB_Dialog))
		Me.SplitContainer1 = New SplitContainer()
		Me.Label1 = New Label()
		Me.ButtonNewDir = New Button()
		Me.ButtonDesktop = New Button()
		Me.ButtonHisFolder = New Button()
		Me.ButtonFolderBack = New Button()
		Me.TextBoxSearchFolder = New TextBox()
		Me.ListViewFolder = New ListView()
		Me.ColumnHeader3 = CType(New ColumnHeader(), ColumnHeader)
		Me.ImageList1 = New ImageList(Me.components)
		Me.ComboBoxDrive = New ComboBox()
		Me.Label5 = New Label()
		Me.Label2 = New Label()
		Me.ButtonAll = New Button()
		Me.ComboBoxExt = New ComboBox()
		Me.ButtonHisFile = New Button()
		Me.TextBoxSearchFile = New TextBox()
		Me.ListViewFiles = New ListView()
		Me.ColumnHeader1 = CType(New ColumnHeader(), ColumnHeader)
		Me.ContextMenuFile = New ContextMenuStrip(Me.components)
		Me.RenameFileToolStripMenuItem = New ToolStripMenuItem()
		Me.DeleteFileToolStripMenuItem = New ToolStripMenuItem()
		Me.TextBoxPath = New TextBox()
		Me.ContextMenuHisFolder = New ContextMenuStrip(Me.components)
		Me.ContextMenuHisFile = New ContextMenuStrip(Me.components)
		Me.ButtonOK = New Button()
		Me.ButtonCancel = New Button()
		Me.TextBoxCurrent = New TextBox()
		Me.Label3 = New Label()
		Me.Label4 = New Label()
		Me.ToolTip1 = New ToolTip(Me.components)
		CType(Me.SplitContainer1, ISupportInitialize).BeginInit()
		Me.SplitContainer1.Panel1.SuspendLayout()
		Me.SplitContainer1.Panel2.SuspendLayout()
		Me.SplitContainer1.SuspendLayout()
		Me.ContextMenuFile.SuspendLayout()
		Me.SuspendLayout()
		'
		'SplitContainer1
		'
		Me.SplitContainer1.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.SplitContainer1.BorderStyle = BorderStyle.Fixed3D
		Me.SplitContainer1.Location = New Point(0, 46)
		Me.SplitContainer1.Name = "SplitContainer1"
		'
		'SplitContainer1.Panel1
		'
		Me.SplitContainer1.Panel1.Controls.Add(Me.Label1)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonNewDir)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonDesktop)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonHisFolder)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ButtonFolderBack)
		Me.SplitContainer1.Panel1.Controls.Add(Me.TextBoxSearchFolder)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ListViewFolder)
		Me.SplitContainer1.Panel1.Controls.Add(Me.ComboBoxDrive)
		'
		'SplitContainer1.Panel2
		'
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label5)
		Me.SplitContainer1.Panel2.Controls.Add(Me.Label2)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonAll)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ComboBoxExt)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ButtonHisFile)
		Me.SplitContainer1.Panel2.Controls.Add(Me.TextBoxSearchFile)
		Me.SplitContainer1.Panel2.Controls.Add(Me.ListViewFiles)
		Me.SplitContainer1.Size = New Size(663, 308)
		Me.SplitContainer1.SplitterDistance = 329
		Me.SplitContainer1.TabIndex = 5
		Me.SplitContainer1.TabStop = False
		'
		'Label1
		'
		Me.Label1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(3, 284)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(32, 13)
		Me.Label1.TabIndex = 28
		Me.Label1.Text = "Filter:"
		'
		'ButtonNewDir
		'
		Me.ButtonNewDir.Image = My.Resources.Resources.new_dir
		Me.ButtonNewDir.Location = New Point(119, 1)
		Me.ButtonNewDir.Name = "ButtonNewDir"
		Me.ButtonNewDir.Size = New Size(26, 25)
		Me.ButtonNewDir.TabIndex = 21
		Me.ButtonNewDir.TabStop = False
		Me.ToolTip1.SetToolTip(Me.ButtonNewDir, "Create new Directory")
		Me.ButtonNewDir.UseVisualStyleBackColor = True
		'
		'ButtonDesktop
		'
		Me.ButtonDesktop.Image = My.Resources.Resources.desktop
		Me.ButtonDesktop.Location = New Point(90, 1)
		Me.ButtonDesktop.Name = "ButtonDesktop"
		Me.ButtonDesktop.Size = New Size(26, 25)
		Me.ButtonDesktop.TabIndex = 22
		Me.ButtonDesktop.TabStop = False
		Me.ToolTip1.SetToolTip(Me.ButtonDesktop, "Go to Desktop")
		Me.ButtonDesktop.UseVisualStyleBackColor = True
		'
		'ButtonHisFolder
		'
		Me.ButtonHisFolder.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonHisFolder.Image = My.Resources.Resources.file_history
		Me.ButtonHisFolder.Location = New Point(298, 1)
		Me.ButtonHisFolder.Name = "ButtonHisFolder"
		Me.ButtonHisFolder.Size = New Size(26, 25)
		Me.ButtonHisFolder.TabIndex = 24
		Me.ButtonHisFolder.TabStop = False
		Me.ToolTip1.SetToolTip(Me.ButtonHisFolder, "History / Favorites")
		Me.ButtonHisFolder.UseVisualStyleBackColor = True
		'
		'ButtonFolderBack
		'
		Me.ButtonFolderBack.Image = CType(resources.GetObject("ButtonFolderBack.Image"), Image)
		Me.ButtonFolderBack.Location = New Point(61, 1)
		Me.ButtonFolderBack.Name = "ButtonFolderBack"
		Me.ButtonFolderBack.Size = New Size(26, 25)
		Me.ButtonFolderBack.TabIndex = 20
		Me.ButtonFolderBack.TabStop = False
		Me.ToolTip1.SetToolTip(Me.ButtonFolderBack, "Move up one directory")
		Me.ButtonFolderBack.UseVisualStyleBackColor = True
		'
		'TextBoxSearchFolder
		'
		Me.TextBoxSearchFolder.Anchor = CType(((AnchorStyles.Bottom Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TextBoxSearchFolder.Location = New Point(37, 281)
		Me.TextBoxSearchFolder.Name = "TextBoxSearchFolder"
		Me.TextBoxSearchFolder.Size = New Size(288, 20)
		Me.TextBoxSearchFolder.TabIndex = 15
		Me.ToolTip1.SetToolTip(Me.TextBoxSearchFolder, "Filter the directories")
		'
		'ListViewFolder
		'
		Me.ListViewFolder.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.ListViewFolder.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader3})
		Me.ListViewFolder.FullRowSelect = True
		Me.ListViewFolder.GridLines = True
		Me.ListViewFolder.HideSelection = False
		Me.ListViewFolder.Location = New Point(-2, 27)
		Me.ListViewFolder.MultiSelect = False
		Me.ListViewFolder.Name = "ListViewFolder"
		Me.ListViewFolder.Size = New Size(327, 253)
		Me.ListViewFolder.SmallImageList = Me.ImageList1
		Me.ListViewFolder.TabIndex = 10
		Me.ListViewFolder.UseCompatibleStateImageBehavior = False
		Me.ListViewFolder.View = View.Details
		'
		'ColumnHeader3
		'
		Me.ColumnHeader3.Text = "Sub-Directories:"
		Me.ColumnHeader3.Width = 368
		'
		'ImageList1
		'
		Me.ImageList1.ColorDepth = ColorDepth.Depth32Bit
		Me.ImageList1.ImageSize = New Size(16, 16)
		Me.ImageList1.TransparentColor = Color.Transparent
		'
		'ComboBoxDrive
		'
		Me.ComboBoxDrive.DropDownStyle = ComboBoxStyle.DropDownList
		Me.ComboBoxDrive.FormattingEnabled = True
		Me.ComboBoxDrive.Location = New Point(3, 3)
		Me.ComboBoxDrive.Name = "ComboBoxDrive"
		Me.ComboBoxDrive.Size = New Size(54, 21)
		Me.ComboBoxDrive.TabIndex = 5
		'
		'Label5
		'
		Me.Label5.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.Label5.AutoSize = True
		Me.Label5.Location = New Point(256, 284)
		Me.Label5.Name = "Label5"
		Me.Label5.Size = New Size(10, 13)
		Me.Label5.TabIndex = 30
		Me.Label5.Text = "."
		'
		'Label2
		'
		Me.Label2.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(3, 284)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(32, 13)
		Me.Label2.TabIndex = 29
		Me.Label2.Text = "Filter:"
		'
		'ButtonAll
		'
		Me.ButtonAll.Location = New Point(2, 2)
		Me.ButtonAll.Name = "ButtonAll"
		Me.ButtonAll.Size = New Size(71, 23)
		Me.ButtonAll.TabIndex = 19
		Me.ButtonAll.Text = "Select All"
		Me.ToolTip1.SetToolTip(Me.ButtonAll, "Select all shown files")
		Me.ButtonAll.UseVisualStyleBackColor = True
		'
		'ComboBoxExt
		'
		Me.ComboBoxExt.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ComboBoxExt.FormattingEnabled = True
		Me.ComboBoxExt.Location = New Point(268, 281)
		Me.ComboBoxExt.Name = "ComboBoxExt"
		Me.ComboBoxExt.Size = New Size(57, 21)
		Me.ComboBoxExt.TabIndex = 20
		Me.ToolTip1.SetToolTip(Me.ComboBoxExt, "Filter the file type")
		'
		'ButtonHisFile
		'
		Me.ButtonHisFile.Anchor = CType((AnchorStyles.Top Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonHisFile.Image = My.Resources.Resources.file_history
		Me.ButtonHisFile.Location = New Point(299, 1)
		Me.ButtonHisFile.Name = "ButtonHisFile"
		Me.ButtonHisFile.Size = New Size(26, 25)
		Me.ButtonHisFile.TabIndex = 24
		Me.ButtonHisFile.TabStop = False
		Me.ToolTip1.SetToolTip(Me.ButtonHisFile, "History")
		Me.ButtonHisFile.UseVisualStyleBackColor = True
		'
		'TextBoxSearchFile
		'
		Me.TextBoxSearchFile.Anchor = CType(((AnchorStyles.Bottom Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TextBoxSearchFile.Location = New Point(36, 281)
		Me.TextBoxSearchFile.Name = "TextBoxSearchFile"
		Me.TextBoxSearchFile.Size = New Size(218, 20)
		Me.TextBoxSearchFile.TabIndex = 15
		Me.ToolTip1.SetToolTip(Me.TextBoxSearchFile, "Filter the files")
		'
		'ListViewFiles
		'
		Me.ListViewFiles.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
			Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.ListViewFiles.Columns.AddRange(New ColumnHeader() {Me.ColumnHeader1})
		Me.ListViewFiles.ContextMenuStrip = Me.ContextMenuFile
		Me.ListViewFiles.FullRowSelect = True
		Me.ListViewFiles.GridLines = True
		Me.ListViewFiles.HideSelection = False
		Me.ListViewFiles.Location = New Point(0, 27)
		Me.ListViewFiles.Name = "ListViewFiles"
		Me.ListViewFiles.Size = New Size(328, 253)
		Me.ListViewFiles.SmallImageList = Me.ImageList1
		Me.ListViewFiles.TabIndex = 10
		Me.ListViewFiles.UseCompatibleStateImageBehavior = False
		Me.ListViewFiles.View = View.Details
		'
		'ColumnHeader1
		'
		Me.ColumnHeader1.Text = "Files:"
		Me.ColumnHeader1.Width = 367
		'
		'ContextMenuFile
		'
		Me.ContextMenuFile.Items.AddRange(New ToolStripItem() {Me.RenameFileToolStripMenuItem, Me.DeleteFileToolStripMenuItem})
		Me.ContextMenuFile.Name = "ContextMenuFile"
		Me.ContextMenuFile.Size = New Size(148, 48)
		'
		'RenameFileToolStripMenuItem
		'
		Me.RenameFileToolStripMenuItem.Name = "RenameFileToolStripMenuItem"
		Me.RenameFileToolStripMenuItem.Size = New Size(147, 22)
		Me.RenameFileToolStripMenuItem.Text = "Rename File..."
		'
		'DeleteFileToolStripMenuItem
		'
		Me.DeleteFileToolStripMenuItem.Name = "DeleteFileToolStripMenuItem"
		Me.DeleteFileToolStripMenuItem.Size = New Size(147, 22)
		Me.DeleteFileToolStripMenuItem.Text = "Delete File..."
		'
		'TextBoxPath
		'
		Me.TextBoxPath.Anchor = CType(((AnchorStyles.Bottom Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TextBoxPath.Location = New Point(47, 370)
		Me.TextBoxPath.Name = "TextBoxPath"
		Me.TextBoxPath.Size = New Size(609, 20)
		Me.TextBoxPath.TabIndex = 15
		'
		'ContextMenuHisFolder
		'
		Me.ContextMenuHisFolder.Name = "ContextMenuFolderHis"
		Me.ContextMenuHisFolder.Size = New Size(61, 4)
		'
		'ContextMenuHisFile
		'
		Me.ContextMenuHisFile.Name = "ContextMenuFileHis"
		Me.ContextMenuHisFile.Size = New Size(61, 4)
		'
		'ButtonOK
		'
		Me.ButtonOK.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonOK.DialogResult = DialogResult.OK
		Me.ButtonOK.Location = New Point(503, 399)
		Me.ButtonOK.Name = "ButtonOK"
		Me.ButtonOK.Size = New Size(75, 23)
		Me.ButtonOK.TabIndex = 20
		Me.ButtonOK.TabStop = False
		Me.ButtonOK.Text = "OK"
		Me.ButtonOK.UseVisualStyleBackColor = True
		'
		'ButtonCancel
		'
		Me.ButtonCancel.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.ButtonCancel.DialogResult = DialogResult.Cancel
		Me.ButtonCancel.Location = New Point(581, 399)
		Me.ButtonCancel.Name = "ButtonCancel"
		Me.ButtonCancel.Size = New Size(75, 23)
		Me.ButtonCancel.TabIndex = 25
		Me.ButtonCancel.TabStop = False
		Me.ButtonCancel.Text = "Cancel"
		Me.ButtonCancel.UseVisualStyleBackColor = True
		'
		'TextBoxCurrent
		'
		Me.TextBoxCurrent.Anchor = CType(((AnchorStyles.Top Or AnchorStyles.Left) _
			Or AnchorStyles.Right), AnchorStyles)
		Me.TextBoxCurrent.Cursor = Cursors.Hand
		Me.TextBoxCurrent.Location = New Point(5, 17)
		Me.TextBoxCurrent.Name = "TextBoxCurrent"
		Me.TextBoxCurrent.ReadOnly = True
		Me.TextBoxCurrent.Size = New Size(651, 20)
		Me.TextBoxCurrent.TabIndex = 0
		Me.TextBoxCurrent.TabStop = False
		Me.ToolTip1.SetToolTip(Me.TextBoxCurrent, "Click for changing the directory")
		'
		'Label3
		'
		Me.Label3.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(5, 373)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(38, 13)
		Me.Label3.TabIndex = 29
		Me.Label3.Text = "Name:"
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(4, 3)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(89, 13)
		Me.Label4.TabIndex = 30
		Me.Label4.Text = "Current Directory:"
		'
		'FB_Dialog
		'
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.ButtonCancel
		Me.ClientSize = New Size(663, 428)
		Me.Controls.Add(Me.Label4)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.TextBoxCurrent)
		Me.Controls.Add(Me.ButtonCancel)
		Me.Controls.Add(Me.ButtonOK)
		Me.Controls.Add(Me.TextBoxPath)
		Me.Controls.Add(Me.SplitContainer1)
		Me.MinimizeBox = False
		Me.MinimumSize = New Size(341, 272)
		Me.Name = "FB_Dialog"
		Me.ShowIcon = False
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "File Browser"
		Me.SplitContainer1.Panel1.ResumeLayout(False)
		Me.SplitContainer1.Panel1.PerformLayout()
		Me.SplitContainer1.Panel2.ResumeLayout(False)
		Me.SplitContainer1.Panel2.PerformLayout()
		CType(Me.SplitContainer1, ISupportInitialize).EndInit()
		Me.SplitContainer1.ResumeLayout(False)
		Me.ContextMenuFile.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents SplitContainer1 As SplitContainer
	Friend WithEvents ComboBoxDrive As ComboBox
	Friend WithEvents ListViewFolder As ListView
	Friend WithEvents ListViewFiles As ListView
	Friend WithEvents ColumnHeader1 As ColumnHeader
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

End Class
