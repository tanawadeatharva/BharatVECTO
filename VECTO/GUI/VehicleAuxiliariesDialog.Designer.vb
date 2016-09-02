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
Imports TUGraz.VECTO.My.Resources

<DesignerGenerated()> _
Partial Class VehicleAuxiliariesDialog
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
		Me.Label1 = New Label()
		Me.Label2 = New Label()
		Me.Label3 = New Label()
		Me.TbID = New TextBox()
		Me.TbPath = New TextBox()
		Me.BtBrowse = New Button()
		Me.CbType = New ComboBox()
		Me.LbIDhelp = New Label()
		Me.Label4 = New Label()
		Me.CbTech = New ComboBox()
		Me.PnTech = New Panel()
		Me.PnFile = New Panel()
		Me.TableLayoutPanel1.SuspendLayout()
		Me.PnTech.SuspendLayout()
		Me.PnFile.SuspendLayout()
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
		Me.TableLayoutPanel1.Location = New Point(301, 100)
		Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
		Me.TableLayoutPanel1.RowCount = 1
		Me.TableLayoutPanel1.RowStyles.Add(New RowStyle(SizeType.Percent, 50.0!))
		Me.TableLayoutPanel1.Size = New Size(146, 29)
		Me.TableLayoutPanel1.TabIndex = 25
		'
		'OK_Button
		'
		Me.OK_Button.Anchor = AnchorStyles.None
		Me.OK_Button.DialogResult = DialogResult.OK
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
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(171, 10)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(18, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "ID"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(9, 10)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(31, 13)
		Me.Label2.TabIndex = 1
		Me.Label2.Text = "Type"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Location = New Point(7, 39)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(50, 13)
		Me.Label3.TabIndex = 1
		Me.Label3.Text = "Input File"
		'
		'TbID
		'
		Me.TbID.Location = New Point(195, 7)
		Me.TbID.Name = "TbID"
		Me.TbID.Size = New Size(39, 20)
		Me.TbID.TabIndex = 5
		'
		'TbPath
		'
		Me.TbPath.Location = New Point(7, 55)
		Me.TbPath.Name = "TbPath"
		Me.TbPath.Size = New Size(401, 20)
		Me.TbPath.TabIndex = 10
		'
		'BtBrowse
		'
		Me.BtBrowse.Image = Open_icon
		Me.BtBrowse.Location = New Point(408, 53)
		Me.BtBrowse.Name = "BtBrowse"
		Me.BtBrowse.Size = New Size(24, 24)
		Me.BtBrowse.TabIndex = 15
		Me.BtBrowse.UseVisualStyleBackColor = True
		'
		'CbType
		'
		Me.CbType.FormattingEnabled = True
		Me.CbType.Location = New Point(46, 7)
		Me.CbType.Name = "CbType"
		Me.CbType.Size = New Size(109, 21)
		Me.CbType.TabIndex = 0
		'
		'LbIDhelp
		'
		Me.LbIDhelp.AutoSize = True
		Me.LbIDhelp.Location = New Point(240, 10)
		Me.LbIDhelp.Name = "LbIDhelp"
		Me.LbIDhelp.Size = New Size(0, 13)
		Me.LbIDhelp.TabIndex = 26
		'
		'Label4
		'
		Me.Label4.AutoSize = True
		Me.Label4.Location = New Point(6, 9)
		Me.Label4.Name = "Label4"
		Me.Label4.Size = New Size(63, 13)
		Me.Label4.TabIndex = 1
		Me.Label4.Text = "Technology"
		'
		'CbTech
		'
		Me.CbTech.DropDownStyle = ComboBoxStyle.DropDownList
		Me.CbTech.FormattingEnabled = True
		Me.CbTech.Location = New Point(75, 6)
		Me.CbTech.Name = "CbTech"
		Me.CbTech.Size = New Size(352, 21)
		Me.CbTech.TabIndex = 27
		'
		'PnTech
		'
		Me.PnTech.Controls.Add(Me.CbTech)
		Me.PnTech.Controls.Add(Me.Label4)
		Me.PnTech.Location = New Point(12, 12)
		Me.PnTech.Name = "PnTech"
		Me.PnTech.Size = New Size(435, 34)
		Me.PnTech.TabIndex = 28
		'
		'PnFile
		'
		Me.PnFile.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.PnFile.Controls.Add(Me.LbIDhelp)
		Me.PnFile.Controls.Add(Me.BtBrowse)
		Me.PnFile.Controls.Add(Me.CbType)
		Me.PnFile.Controls.Add(Me.TbID)
		Me.PnFile.Controls.Add(Me.TbPath)
		Me.PnFile.Controls.Add(Me.Label2)
		Me.PnFile.Controls.Add(Me.Label3)
		Me.PnFile.Controls.Add(Me.Label1)
		Me.PnFile.Location = New Point(12, 8)
		Me.PnFile.Name = "PnFile"
		Me.PnFile.Size = New Size(435, 89)
		Me.PnFile.TabIndex = 29
		'
		'F_VEH_AuxDlog
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New Size(459, 141)
		Me.Controls.Add(Me.PnFile)
		Me.Controls.Add(Me.PnTech)
		Me.Controls.Add(Me.TableLayoutPanel1)
		Me.FormBorderStyle = FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "VehicleAuxiliariesDialog"
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Auxiliary"
		Me.TableLayoutPanel1.ResumeLayout(False)
		Me.PnTech.ResumeLayout(False)
		Me.PnTech.PerformLayout()
		Me.PnFile.ResumeLayout(False)
		Me.PnFile.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents Label2 As Label
	Friend WithEvents Label3 As Label
	Friend WithEvents TbID As TextBox
	Friend WithEvents TbPath As TextBox
	Friend WithEvents BtBrowse As Button
	Friend WithEvents CbType As ComboBox
	Friend WithEvents LbIDhelp As Label
	Friend WithEvents Label4 As Label
	Friend WithEvents CbTech As ComboBox
	Friend WithEvents PnTech As Panel
	Friend WithEvents PnFile As Panel

End Class
