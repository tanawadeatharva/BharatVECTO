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
Partial Class REESSPackDialog
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(REESSPackDialog))
        Me.TableLayoutPanel1 = New System.Windows.Forms.TableLayoutPanel()
        Me.OK_Button = New System.Windows.Forms.Button()
        Me.Cancel_Button = New System.Windows.Forms.Button()
        Me.pnREESSPack = New System.Windows.Forms.Panel()
        Me.btnOpenBattery = New System.Windows.Forms.Button()
        Me.btnBrowseBattery = New System.Windows.Forms.Button()
        Me.tbBattery = New System.Windows.Forms.TextBox()
        Me.tbBatteryPackCnt = New System.Windows.Forms.TextBox()
        Me.lblBatteryPackCnt = New System.Windows.Forms.Label()
        Me.tbStreamId = New System.Windows.Forms.TextBox()
        Me.lblStreamId = New System.Windows.Forms.Label()
        Me.TableLayoutPanel1.SuspendLayout
        Me.pnREESSPack.SuspendLayout
        Me.SuspendLayout
        '
        'TableLayoutPanel1
        '
        Me.TableLayoutPanel1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.TableLayoutPanel1.ColumnCount = 2
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Controls.Add(Me.OK_Button, 0, 0)
        Me.TableLayoutPanel1.Controls.Add(Me.Cancel_Button, 1, 0)
        Me.TableLayoutPanel1.Location = New System.Drawing.Point(632, 166)
        Me.TableLayoutPanel1.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.TableLayoutPanel1.Name = "TableLayoutPanel1"
        Me.TableLayoutPanel1.RowCount = 1
        Me.TableLayoutPanel1.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50!))
        Me.TableLayoutPanel1.Size = New System.Drawing.Size(219, 45)
        Me.TableLayoutPanel1.TabIndex = 4
        '
        'OK_Button
        '
        Me.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.OK_Button.Location = New System.Drawing.Point(4, 5)
        Me.OK_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.OK_Button.Name = "OK_Button"
        Me.OK_Button.Size = New System.Drawing.Size(100, 35)
        Me.OK_Button.TabIndex = 3
        Me.OK_Button.Text = "OK"
        '
        'Cancel_Button
        '
        Me.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None
        Me.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Cancel_Button.Location = New System.Drawing.Point(114, 5)
        Me.Cancel_Button.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.Cancel_Button.Name = "Cancel_Button"
        Me.Cancel_Button.Size = New System.Drawing.Size(100, 35)
        Me.Cancel_Button.TabIndex = 4
        Me.Cancel_Button.Text = "Cancel"
        '
        'pnREESSPack
        '
        Me.pnREESSPack.Controls.Add(Me.btnOpenBattery)
        Me.pnREESSPack.Controls.Add(Me.btnBrowseBattery)
        Me.pnREESSPack.Controls.Add(Me.tbBattery)
        Me.pnREESSPack.Location = New System.Drawing.Point(18, 18)
        Me.pnREESSPack.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.pnREESSPack.Name = "pnREESSPack"
        Me.pnREESSPack.Size = New System.Drawing.Size(830, 42)
        Me.pnREESSPack.TabIndex = 0
        '
        'btnOpenBattery
        '
        Me.btnOpenBattery.Location = New System.Drawing.Point(6, 5)
        Me.btnOpenBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnOpenBattery.Name = "btnOpenBattery"
        Me.btnOpenBattery.Size = New System.Drawing.Size(141, 32)
        Me.btnOpenBattery.TabIndex = 0
        Me.btnOpenBattery.TabStop = false
        Me.btnOpenBattery.Text = "REESS Pack"
        Me.btnOpenBattery.UseVisualStyleBackColor = true
        '
        'btnBrowseBattery
        '
        Me.btnBrowseBattery.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.btnBrowseBattery.Image = CType(resources.GetObject("btnBrowseBattery.Image"),System.Drawing.Image)
        Me.btnBrowseBattery.Location = New System.Drawing.Point(790, 3)
        Me.btnBrowseBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.btnBrowseBattery.Name = "btnBrowseBattery"
        Me.btnBrowseBattery.Size = New System.Drawing.Size(36, 37)
        Me.btnBrowseBattery.TabIndex = 1
        Me.btnBrowseBattery.UseVisualStyleBackColor = true
        '
        'tbBattery
        '
        Me.tbBattery.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left)  _
            Or System.Windows.Forms.AnchorStyles.Right),System.Windows.Forms.AnchorStyles)
        Me.tbBattery.Location = New System.Drawing.Point(156, 6)
        Me.tbBattery.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbBattery.Name = "tbBattery"
        Me.tbBattery.Size = New System.Drawing.Size(624, 26)
        Me.tbBattery.TabIndex = 0
        '
        'tbBatteryPackCnt
        '
        Me.tbBatteryPackCnt.Location = New System.Drawing.Point(321, 69)
        Me.tbBatteryPackCnt.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbBatteryPackCnt.Name = "tbBatteryPackCnt"
        Me.tbBatteryPackCnt.Size = New System.Drawing.Size(86, 26)
        Me.tbBatteryPackCnt.TabIndex = 1
        '
        'lblBatteryPackCnt
        '
        Me.lblBatteryPackCnt.AutoSize = true
        Me.lblBatteryPackCnt.Location = New System.Drawing.Point(26, 74)
        Me.lblBatteryPackCnt.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblBatteryPackCnt.Name = "lblBatteryPackCnt"
        Me.lblBatteryPackCnt.Size = New System.Drawing.Size(183, 20)
        Me.lblBatteryPackCnt.TabIndex = 25
        Me.lblBatteryPackCnt.Text = "Number of RESS Packs:"
        '
        'tbStreamId
        '
        Me.tbStreamId.Location = New System.Drawing.Point(321, 109)
        Me.tbStreamId.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.tbStreamId.Name = "tbStreamId"
        Me.tbStreamId.Size = New System.Drawing.Size(86, 26)
        Me.tbStreamId.TabIndex = 2
        '
        'lblStreamId
        '
        Me.lblStreamId.AutoSize = true
        Me.lblStreamId.Location = New System.Drawing.Point(26, 114)
        Me.lblStreamId.Margin = New System.Windows.Forms.Padding(4, 0, 4, 0)
        Me.lblStreamId.Name = "lblStreamId"
        Me.lblStreamId.Size = New System.Drawing.Size(74, 20)
        Me.lblStreamId.TabIndex = 27
        Me.lblStreamId.Text = "Stream #"
        '
        'REESSPackDialog
        '
        Me.AcceptButton = Me.OK_Button
        Me.AutoScaleDimensions = New System.Drawing.SizeF(9!, 20!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.CancelButton = Me.Cancel_Button
        Me.ClientSize = New System.Drawing.Size(868, 229)
        Me.Controls.Add(Me.tbStreamId)
        Me.Controls.Add(Me.lblStreamId)
        Me.Controls.Add(Me.tbBatteryPackCnt)
        Me.Controls.Add(Me.lblBatteryPackCnt)
        Me.Controls.Add(Me.pnREESSPack)
        Me.Controls.Add(Me.TableLayoutPanel1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow
        Me.Margin = New System.Windows.Forms.Padding(4, 5, 4, 5)
        Me.MaximizeBox = false
        Me.MinimizeBox = false
        Me.Name = "REESSPackDialog"
        Me.ShowInTaskbar = false
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Edit REESS Pack"
        Me.TableLayoutPanel1.ResumeLayout(false)
        Me.pnREESSPack.ResumeLayout(false)
        Me.pnREESSPack.PerformLayout
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub
	Friend WithEvents TableLayoutPanel1 As TableLayoutPanel
	Friend WithEvents OK_Button As Button
	Friend WithEvents Cancel_Button As Button
    Friend WithEvents pnREESSPack As Panel
    Friend WithEvents btnOpenBattery As Button
    Friend WithEvents btnBrowseBattery As Button
    Friend WithEvents tbBattery As TextBox
    Friend WithEvents tbBatteryPackCnt As TextBox
    Friend WithEvents lblBatteryPackCnt As Label
    Friend WithEvents tbStreamId As TextBox
    Friend WithEvents lblStreamId As Label
End Class
