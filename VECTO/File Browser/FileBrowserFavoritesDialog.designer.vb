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
Partial Class FileBrowserFavoritesDialog
	Inherits Form

	'Das Formular überschreibt den Löschvorgang, um die Komponentenliste zu bereinigen.
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

	'Wird vom Windows Form-Designer benötigt.
	Private components As IContainer

	'Hinweis: Die folgende Prozedur ist für den Windows Form-Designer erforderlich.
	'Das Bearbeiten ist mit dem Windows Form-Designer möglich.  
	'Das Bearbeiten mit dem Code-Editor ist nicht möglich.
	<DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.TableLayoutPanel1 = New TableLayoutPanel()
		Me.OK_Button = New Button()
		Me.Cancel_Button = New Button()
		Me.ListBox1 = New ListBox()
		Me.Label1 = New Label()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'TableLayoutPanel1
		'
		Me.TableLayoutPanel1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.TableLayoutPanel1.ColumnCount = 2
		Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.ColumnStyles.Add(New ColumnStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
		Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
		Me.TableLayoutPanel1.Location = New Point(326, 173)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 1
		Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New Size(146, 29)
		Me.TableLayoutPanel1.TabIndex = 0
		'
		'OK_Button
		'
		Me.OK_Button.Anchor = AnchorStyles.None
		Me.OK_Button.Location = New Point(3, 3)
		Me.OK_Button.Name = "OK_Button"
		Me.OK_Button.Size = New Size(67, 23)
		Me.OK_Button.TabIndex = 0
		Me.OK_Button.Text = "OK"
		'
		'Cancel_Button
		'
		Me.Cancel_Button.Anchor = AnchorStyles.None
		Me.Cancel_Button.DialogResult = DialogResult.Cancel
		Me.Cancel_Button.Location = New Point(76, 3)
		Me.Cancel_Button.Name = "Cancel_Button"
		Me.Cancel_Button.Size = New Size(67, 23)
		Me.Cancel_Button.TabIndex = 1
		Me.Cancel_Button.Text = "Cancel"
		'
		'ListBox1
		'
		Me.ListBox1.Anchor = CType((((AnchorStyles.Top Or AnchorStyles.Bottom) _
									Or AnchorStyles.Left) _
									Or AnchorStyles.Right), AnchorStyles)
		Me.ListBox1.FormattingEnabled = True
		Me.ListBox1.Location = New Point(12, 12)
		Me.ListBox1.Name = "ListBox1"
		Me.ListBox1.Size = New Size(460, 134)
		Me.ListBox1.TabIndex = 1
		'
		'Label1
		'
		Me.Label1.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(369, 147)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(106, 13)
		Me.Label1.TabIndex = 2
		Me.Label1.Text = "(Double-Click to Edit)"
		'
		'FB_FavDlog
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleMode = AutoScaleMode.Inherit
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New Size(484, 208)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.ListBox1)
		Me.Controls.Add(Me.TableLayoutPanel1)
		Me.FormBorderStyle = FormBorderStyle.SizableToolWindow
		Me.MaximizeBox = False
		Me.MaximumSize = New Size(5000, 242)
		Me.MinimizeBox = False
		Me.MinimumSize = New Size(0, 242)
		Me.Name = "FileBrowserFavoritesDialog"
		Me.ShowIcon = False
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Edit Favorites"
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents ListBox1 As ListBox
	Friend WithEvents Label1 As Label

End Class
