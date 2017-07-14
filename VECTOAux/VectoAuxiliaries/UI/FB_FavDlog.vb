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
Option Infer On

Imports System.Windows.Forms

''' <summary>
''' Sub-dialog for File Browser. Entirely controlled by cFilebrowser class (via FB_Dialog).
''' </summary>
''' <remarks></remarks>
Public Class FB_FavDlog
	Private Const NoFavString As String = "<empty favorite slot>"
	Private Const EmptyText As String = " "

	Private Sub FB_FavDlog_Load(sender As Object, e As EventArgs) Handles Me.Load
		For x = 10 To 19
			Dim txt = FB_FolderHistory(x)
			If txt = EmptyText Then
				ListBox1.Items.Add(NoFavString)
			Else
				ListBox1.Items.Add(txt)
			End If
		Next
	End Sub

	Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click
		DialogResult = DialogResult.OK
		Close()
	End Sub

	Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
		DialogResult = DialogResult.Cancel
		Close()
	End Sub

	Private Sub ListBox1_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles ListBox1.MouseDoubleClick
		Dim i = ListBox1.SelectedIndex
		Dim txt = ListBox1.Items(i).ToString

		If txt = NoFavString Then txt = ""

		Dim fb = New cFileBrowser("DirBr", True, True)

		If fb.OpenDialog(txt) Then
			txt = fb.Files(0)
			ListBox1.Items.Insert(i, txt)
			ListBox1.Items.RemoveAt(i + 1)
		End If
	End Sub

	Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListBox1.SelectedIndexChanged

	End Sub
End Class
