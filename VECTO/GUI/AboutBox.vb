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


''' <summary>
''' About Dialog. Shows Licence and contact/support information
''' </summary>
''' <remarks></remarks>
Public Class AboutBox
	'Initialize
	Private Sub F10_AboutBox_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
		Text = "VECTO " & VECTOvers & " / VectoCore " & COREvers
		LabelLic.Text = Lic.LicString
		LabelLicDate.Text = "Expiring date (y/m/d):   " & Lic.ExpTime
	End Sub

	'e-mail links----------------------------------------------------------------
	Private Sub LinkLabel1_LinkClicked_1(sender As Object, e As LinkLabelLinkClickedEventArgs) _
		Handles LinkLabel1.LinkClicked
		Process.Start("mailto:vecto@jrc.ec.europa.eu")
	End Sub

	'----------------------------------------------------------------------------

	'Picture Links------------------------------------------------------------------
	Private Sub PictureBoxJRC_Click(sender As Object, e As EventArgs) Handles PictureBoxJRC.Click
		Process.Start("http://ec.europa.eu/dgs/jrc/index.cfm")
	End Sub


	Private Sub LinkLabel2_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) _
		Handles LinkLabel2.LinkClicked
		Process.Start("https://joinup.ec.europa.eu/community/eupl/og_page/eupl")
	End Sub
End Class
