Imports System.ComponentModel
Imports Microsoft.VisualBasic.CompilerServices


<DesignerGenerated()> _
Partial Class JiraDialog
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
		Me.OK_Button = New Button()
		Me.Label1 = New Label()
		Me.LinkLabel1 = New LinkLabel()
		Me.GroupBox1 = New GroupBox()
		Me.LinkLabel3 = New LinkLabel()
		Me.Label2 = New Label()
		Me.Button1 = New Button()
		Me.GroupBox1.SuspendLayout()
		Me.SuspendLayout()
		'
		'OK_Button
		'
		Me.OK_Button.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Right), AnchorStyles)
		Me.OK_Button.Location = New Point(356, 205)
		Me.OK_Button.Name = "OK_Button"
		Me.OK_Button.Size = New Size(67, 23)
		Me.OK_Button.TabIndex = 0
		Me.OK_Button.Text = "Close"
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Location = New Point(6, 16)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(269, 13)
		Me.Label1.TabIndex = 1
		Me.Label1.Text = "You need a CITnet user account to create a new issue."
		'
		'LinkLabel1
		'
		Me.LinkLabel1.AutoSize = True
		Me.LinkLabel1.Location = New Point(6, 65)
		Me.LinkLabel1.Name = "LinkLabel1"
		Me.LinkLabel1.Size = New Size(141, 13)
		Me.LinkLabel1.TabIndex = 2
		Me.LinkLabel1.TabStop = True
		Me.LinkLabel1.Text = "JIRA Quick Start Guide (pdf)"
		'
		'GroupBox1
		'
		Me.GroupBox1.Controls.Add(Me.LinkLabel3)
		Me.GroupBox1.Controls.Add(Me.Label1)
		Me.GroupBox1.Controls.Add(Me.Label2)
		Me.GroupBox1.Controls.Add(Me.LinkLabel1)
		Me.GroupBox1.Location = New Point(12, 86)
		Me.GroupBox1.Name = "GroupBox1"
		Me.GroupBox1.Size = New Size(411, 100)
		Me.GroupBox1.TabIndex = 3
		Me.GroupBox1.TabStop = False
		Me.GroupBox1.Text = "Help"
		'
		'LinkLabel3
		'
		Me.LinkLabel3.AutoSize = True
		Me.LinkLabel3.Location = New Point(246, 33)
		Me.LinkLabel3.Name = "LinkLabel3"
		Me.LinkLabel3.Size = New Size(122, 13)
		Me.LinkLabel3.TabIndex = 18
		Me.LinkLabel3.TabStop = True
		Me.LinkLabel3.Text = "vecto@jrc.ec.europa.eu"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Location = New Point(6, 33)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(241, 13)
		Me.Label2.TabIndex = 3
		Me.Label2.Text = "If you don't have one yet please contact support: "
		'
		'Button1
		'
		Me.Button1.Font = New Font("Microsoft Sans Serif", 15.75!, FontStyle.Bold, GraphicsUnit.Point, CType(0, Byte))
		Me.Button1.Location = New Point(12, 12)
		Me.Button1.Name = "Button1"
		Me.Button1.Size = New Size(411, 54)
		Me.Button1.TabIndex = 4
		Me.Button1.Text = "Create JIRA Issue"
		Me.Button1.UseVisualStyleBackColor = True
		'
		'F_JIRA
		'
		Me.AcceptButton = Me.OK_Button
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.ClientSize = New Size(435, 240)
		Me.Controls.Add(Me.Button1)
		Me.Controls.Add(Me.GroupBox1)
		Me.Controls.Add(Me.OK_Button)
		Me.FormBorderStyle = FormBorderStyle.FixedDialog
		Me.MaximizeBox = False
		Me.MinimizeBox = False
		Me.Name = "JiraDialog"
		Me.ShowInTaskbar = False
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "Report Issue via CITnet"
		Me.GroupBox1.ResumeLayout(False)
		Me.GroupBox1.PerformLayout()
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents OK_Button As Button
	Friend WithEvents Label1 As Label
	Friend WithEvents LinkLabel1 As LinkLabel
	Friend WithEvents GroupBox1 As GroupBox
	Friend WithEvents Label2 As Label
	Friend WithEvents LinkLabel3 As LinkLabel
	Friend WithEvents Button1 As Button

End Class
