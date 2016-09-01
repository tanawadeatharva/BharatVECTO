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
Partial Class F_Welcome
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
		Me.Cancel_Button = New Button()
		Me.Label1 = New Label()
		Me.Button1 = New Button()
		Me.Button2 = New Button()
		Me.SuspendLayout()
		'
		'Cancel_Button
		'
		Me.Cancel_Button.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.Cancel_Button.DialogResult = DialogResult.Cancel
		Me.Cancel_Button.Location = New Point(239, 152)
		Me.Cancel_Button.Name = "Cancel_Button"
		Me.Cancel_Button.Size = New Size(67, 23)
		Me.Cancel_Button.TabIndex = 1
		Me.Cancel_Button.Text = "Close"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New Font("Microsoft Sans Serif", 9.75!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New Point(12, 18)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(77, 16)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "Welcome!"
		'
		'Button1
		'
		Me.Button1.Location = New Point(12, 47)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New Size(294, 37)
		Me.Button1.TabIndex = 2
		Me.Button1.Text = "Open Release Notes (pdf)"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'Button2
		'
		Me.Button2.Location = New Point(12, 90)
		Me.Button2.Name = "Button2"
		Me.Button2.Size = New Size(294, 37)
		Me.Button2.TabIndex = 2
		Me.Button2.Text = "Open User Manual (html)"
		Me.Button2.UseVisualStyleBackColor = True
		'
		'F_Welcome
		'
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.CancelButton = Me.Cancel_Button
		Me.ClientSize = New Size(318, 187)
		Me.Controls.Add(Me.Button2)
		Me.Controls.Add(Me.Cancel_Button)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.Label1)
		Me.FormBorderStyle = FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "F_Welcome"
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "F_Welcome"
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Cancel_Button As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents Button1 As Button
	Friend WithEvents Button2 As Button

End Class
