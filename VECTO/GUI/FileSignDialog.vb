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
Imports System.IO
Imports System.Windows.Forms
Imports vectolic

''' <summary>
''' Create/Verify signature files (.vsig).
''' </summary>
''' <remarks></remarks>
Public Class FileSignDialog
	'Create signature file
	Private Sub BtSign_Click(sender As Object, e As EventArgs) Handles BtSign.Click
		Dim listViewItem As ListViewItem
		Dim mainDirectory As String

		If lvFiles.Items.Count = 0 Then
			MsgBox("No files selected!", MsgBoxStyle.Critical)
			Exit Sub
		End If

		If Trim(TbSigFile.Text) = "" Then
			MsgBox("No signature file path defined!", MsgBoxStyle.Critical)
			Exit Sub
		End If

		If File.Exists(TbSigFile.Text) Then
			If MsgBox("Overwrite existing signature file?", MsgBoxStyle.YesNo) = MsgBoxResult.No Then Exit Sub
		End If

		ClearForm(False)


		mainDirectory = GetPath(TbSigFile.Text)


		Lic.FileSigning.NewFile()
		Lic.FileSigning.Mode = cFileSigning.tMode.Manual


		For Each listViewItem In lvFiles.Items
			Lic.FileSigning.AddFile(fFileRepl(listViewItem.SubItems(0).Text, mainDirectory))
			listViewItem.SubItems(1).Text = ""
			listViewItem.ForeColor = Color.Black
		Next

		If Lic.FileSigning.WriteSigFile(TbSigFile.Text, LicSigAppCode) Then
			LbStatus.Text = "Signature file created successfully"
			LbStatus.ForeColor = Color.DarkGreen
		Else
			LbStatus.Text = "Fail to create signature file! " & Lic.FileSigning.ErrorMsg
			LbStatus.ForeColor = Color.Red
		End If

		TbLicStr.Text = Lic.FileSigning.CreatorLicStr
		TbPubKey.Text = Lic.FileSigning.PubKey
		LbMode.Text = Lic.FileSigning.ModeConv(Lic.FileSigning.Mode)
		LbDateStr.Text = Lic.FileSigning.DateStr

		If Lic.FileSigning.FilesOK.Count > 0 Then
			For Each listViewItem In lvFiles.Items
				listViewItem.SubItems(1).Text = Lic.FileSigning.FilesMsg(listViewItem.Index)
				If Lic.FileSigning.FilesOK(listViewItem.Index) Then
					listViewItem.ForeColor = Color.DarkGreen
				Else
					listViewItem.ForeColor = Color.Red
					Exit For
				End If
			Next
		End If
	End Sub

	'Verify existing signature file
	Public Sub VerifySigFile()
		Dim lv0 As ListViewItem
		Dim i As Integer

		If Not File.Exists(TbSigFile.Text) Then
			MsgBox("Signature file not found!", MsgBoxStyle.Critical)
			Exit Sub
		End If

		ClearForm(True)

		If Lic.FileSigning.ReadSigFile(TbSigFile.Text, LicSigAppCode) Then
			LbStatus.Text = "File signature verified"
			LbStatus.ForeColor = Color.DarkGreen
		Else
			LbStatus.Text = "ERROR! " & Lic.FileSigning.ErrorMsg
			LbStatus.ForeColor = Color.Red
		End If

		TbLicStr.Text = Lic.FileSigning.CreatorLicStr
		TbPubKey.Text = Lic.FileSigning.PubKey
		LbMode.Text = Lic.FileSigning.ModeConv(Lic.FileSigning.Mode)
		If Lic.FileSigning.Mode = cFileSigning.tMode.Auto Then
			LbMode.ForeColor = Color.DarkGreen
		Else
			LbMode.ForeColor = Color.Red
		End If
		LbDateStr.Text = Lic.FileSigning.DateStr

		For i = 0 To Lic.FileSigning.FilesOK.Count - 1
			lv0 = New ListViewItem(Lic.FileSigning.Files(i))
			lv0.SubItems.Add(Lic.FileSigning.FilesMsg(i))
			If Lic.FileSigning.FilesOK(i) Then
				lv0.ForeColor = Color.DarkGreen
			Else
				lv0.ForeColor = Color.Red
			End If
			lvFiles.Items.Add(lv0)
		Next
	End Sub

	'Clear form
	Private Sub ClearForm(ClearFileList As Boolean)
		If ClearFileList Then lvFiles.Items.Clear()
		TbLicStr.Text = ""
		TbPubKey.Text = ""
		LbMode.Text = ""
		LbDateStr.Text = ""
		LbStatus.Text = ""
		LbMode.ForeColor = DefaultForeColor
		LbMode.BackColor = DefaultBackColor
		LbStatus.ForeColor = DefaultForeColor
		LbStatus.BackColor = DefaultBackColor
	End Sub


#Region "GUI Controls"

	Private Sub BtBrowse_Click(sender As Object, e As EventArgs) Handles BtBrowse.Click
		Dim fb As New FileBrowser("sig", False, True)
		fb.Extensions = New String() {"vsig"}

		If fb.CustomDialog(TbSigFile.Text, False, False, FileBrowserFileExtensionMode.ForceExt, False, "vsig") Then
			TbSigFile.Text = fb.Files(0)
		End If

		If File.Exists(TbSigFile.Text) Then
			VerifySigFile()
		End If
	End Sub

	Private Sub BtAddFLD_Click(sender As Object, e As EventArgs) Handles BtAddFLD.Click
		AddFile()
	End Sub

	Private Sub BtRemFLD_Click(sender As Object, e As EventArgs) Handles BtRemFLD.Click
		RemoveFile()
	End Sub

	Private Sub BtClose_Click(sender As Object, e As EventArgs) Handles BtClose.Click
		Close()
	End Sub

	Private Sub BtClearList_Click(sender As Object, e As EventArgs) Handles BtClearList.Click
		lvFiles.Items.Clear()
	End Sub

	Private Sub BtReload_Click(sender As Object, e As EventArgs) Handles BtReload.Click
		VerifySigFile()
	End Sub

	Private Sub lvFiles_KeyDown(sender As Object, e As KeyEventArgs) Handles lvFiles.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				RemoveFile()
		End Select
	End Sub

#End Region

	'Add File
	Private Sub AddFile()
		Dim lvi As ListViewItem
		Dim fb As New FileBrowser("xxx", False, True)
		Dim str As String

		If fb.OpenDialog("", True) Then

			For Each str In fb.Files

				lvi = New ListViewItem(str)
				lvi.SubItems.Add("")
				lvi.ForeColor = Color.Black

				lvFiles.Items.Add(lvi)
				lvi.EnsureVisible()

				lvFiles.Focus()

			Next

		End If
	End Sub

	'Remove File
	Private Sub RemoveFile()
		Dim i0 As Integer

		If lvFiles.Items.Count = 0 Then Exit Sub

		If lvFiles.SelectedItems.Count = 0 Then lvFiles.Items(lvFiles.Items.Count - 1).Selected = True

		i0 = lvFiles.SelectedItems(0).Index

		lvFiles.SelectedItems(0).Remove()

		If i0 < lvFiles.Items.Count Then
			lvFiles.Items(i0).Selected = True
			lvFiles.Items(i0).EnsureVisible()
		End If

		lvFiles.Focus()
	End Sub
End Class
