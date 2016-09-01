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
Partial Class F_AboutBox
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
		Me.Label10 = New Label()
		Me.LabelLic = New Label()
		Me.LabelLicDate = New Label()
		Me.Label2 = New Label()
		Me.Label8 = New Label()
		Me.Panel1 = New Panel()
		Me.PictureBox1 = New PictureBox()
		Me.Label1 = New Label()
		Me.LinkLabel1 = New LinkLabel()
		Me.LinkLabel2 = New LinkLabel()
		Me.Label3 = New Label()
		Me.PictureBoxJRC = New PictureBox()
		Me.Panel1.SuspendLayout()
		CType(Me.PictureBox1, ISupportInitialize).BeginInit()
		CType(Me.PictureBoxJRC, ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Label10
		'
		Me.Label10.AutoSize = True
		Me.Label10.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.Label10.Location = New Point(9, 250)
		Me.Label10.Name = "Label10"
		Me.Label10.Size = New Size(75, 13)
		Me.Label10.TabIndex = 11
		Me.Label10.Text = "License file:"
		'
		'LabelLic
		'
		Me.LabelLic.AutoSize = True
		Me.LabelLic.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.LabelLic.Location = New Point(3, 0)
		Me.LabelLic.Name = "LabelLic"
		Me.LabelLic.Size = New Size(22, 13)
		Me.LabelLic.TabIndex = 12
		Me.LabelLic.Text = "Lic"
		'
		'LabelLicDate
		'
		Me.LabelLicDate.Anchor = CType((AnchorStyles.Bottom Or AnchorStyles.Left), AnchorStyles)
		Me.LabelLicDate.AutoSize = True
		Me.LabelLicDate.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.LabelLicDate.Location = New Point(3, 26)
		Me.LabelLicDate.Name = "LabelLicDate"
		Me.LabelLicDate.Size = New Size(12, 13)
		Me.LabelLicDate.TabIndex = 13
		Me.LabelLicDate.Text = "-"
		'
		'Label2
		'
		Me.Label2.AutoSize = True
		Me.Label2.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.Label2.Location = New Point(9, 346)
		Me.Label2.Name = "Label2"
		Me.Label2.Size = New Size(162, 13)
		Me.Label2.TabIndex = 7
		Me.Label2.Text = "Developed on behalf of the"
		'
		'Label8
		'
		Me.Label8.AutoSize = True
		Me.Label8.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.Label8.Location = New Point(24, 368)
		Me.Label8.Name = "Label8"
		Me.Label8.Size = New Size(158, 39)
		Me.Label8.TabIndex = 3
		Me.Label8.Text = "Joint Research Centre" & ChrW(13) & ChrW(10) & "Sustainable Transport Unit" & ChrW(13) & ChrW(10) & "European Commission"
		'
		'Panel1
		'
		Me.Panel1.BorderStyle = BorderStyle.Fixed3D
		Me.Panel1.Controls.Add(Me.LabelLic)
		Me.Panel1.Controls.Add(Me.LabelLicDate)
		Me.Panel1.Location = New Point(12, 266)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New Size(464, 52)
		Me.Panel1.TabIndex = 16
		'
		'PictureBox1
		'
		Me.PictureBox1.Image = VECTO_About
		Me.PictureBox1.Location = New Point(23, 12)
		Me.PictureBox1.Name = "PictureBox1"
		Me.PictureBox1.Size = New Size(447, 182)
		Me.PictureBox1.TabIndex = 14
		Me.PictureBox1.TabStop = False
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New Font("Verdana", 8.25!, FontStyle.Regular, GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New Point(9, 436)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New Size(105, 13)
		Me.Label1.TabIndex = 7
		Me.Label1.Text = "Support Contact:"
		'
		'LinkLabel1
		'
		Me.LinkLabel1.AutoSize = True
		Me.LinkLabel1.Location = New Point(120, 436)
		Me.LinkLabel1.Name = "LinkLabel1"
		Me.LinkLabel1.Size = New Size(122, 13)
		Me.LinkLabel1.TabIndex = 17
		Me.LinkLabel1.TabStop = True
		Me.LinkLabel1.Text = "vecto@jrc.ec.europa.eu"
		'
		'LinkLabel2
		'
		Me.LinkLabel2.AutoSize = True
		Me.LinkLabel2.Font = New Font("Verdana", 8.25!)
		Me.LinkLabel2.Location = New Point(20, 197)
		Me.LinkLabel2.Name = "LinkLabel2"
		Me.LinkLabel2.Size = New Size(210, 13)
		Me.LinkLabel2.TabIndex = 18
		Me.LinkLabel2.TabStop = True
		Me.LinkLabel2.Text = "VECTO is licensed under EUPL 1.1+"
		'
		'Label3
		'
		Me.Label3.AutoSize = True
		Me.Label3.Font = New Font("Verdana", 8.25!)
		Me.Label3.Location = New Point(20, 213)
		Me.Label3.Name = "Label3"
		Me.Label3.Size = New Size(237, 13)
		Me.Label3.TabIndex = 19
		Me.Label3.Text = "Copyright © 2012-2016 European Union"
		'
		'PictureBoxJRC
		'
		Me.PictureBoxJRC.Cursor = Cursors.Hand
		Me.PictureBoxJRC.Image = JRC_About
		Me.PictureBoxJRC.Location = New Point(216, 353)
		Me.PictureBoxJRC.Name = "PictureBoxJRC"
		Me.PictureBoxJRC.Size = New Size(260, 54)
		Me.PictureBoxJRC.TabIndex = 15
		Me.PictureBoxJRC.TabStop = False
		'
		'F_AboutBox
		'
		Me.AutoScaleDimensions = New SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = AutoScaleMode.Font
		Me.BackColor = Color.White
		Me.ClientSize = New Size(491, 474)
		Me.Controls.Add(Me.Label3)
		Me.Controls.Add(Me.LinkLabel2)
		Me.Controls.Add(Me.LinkLabel1)
		Me.Controls.Add(Me.Panel1)
		Me.Controls.Add(Me.PictureBoxJRC)
		Me.Controls.Add(Me.PictureBox1)
		Me.Controls.Add(Me.Label10)
		Me.Controls.Add(Me.Label1)
		Me.Controls.Add(Me.Label2)
		Me.Controls.Add(Me.Label8)
		Me.FormBorderStyle = FormBorderStyle.FixedToolWindow
		Me.Name = "F_AboutBox"
		Me.StartPosition = FormStartPosition.CenterParent
		Me.Text = "About VECTO"
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		CType(Me.PictureBox1, ISupportInitialize).EndInit()
		CType(Me.PictureBoxJRC, ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label10 As Label
	Friend WithEvents LabelLic As Label
	Friend WithEvents LabelLicDate As Label
	Friend WithEvents PictureBox1 As PictureBox
	Friend WithEvents Label2 As Label
	Friend WithEvents Label8 As Label
	Friend WithEvents Panel1 As Panel
	Friend WithEvents Label1 As Label
	Friend WithEvents LinkLabel1 As LinkLabel
	Friend WithEvents LinkLabel2 As LinkLabel
	Friend WithEvents Label3 As Label
	Friend WithEvents PictureBoxJRC As PictureBox
End Class
