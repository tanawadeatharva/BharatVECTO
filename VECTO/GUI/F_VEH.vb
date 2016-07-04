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
Option Infer On


Imports System.IO
Imports System.Text.RegularExpressions

''' <summary>
''' Vehicle Editor.
''' </summary>
''' <remarks></remarks>
Public Class F_VEH
	Dim AxlDlog As F_VEH_Axle
	Private HDVclass As String
	Dim VehFile As String
	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""

	Private Changed As Boolean = False

	'Close - Check for unsaved changes
	Private Sub F_VEH_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Initialise form
	Private Sub F05_VEH_Load(sender As Object, e As EventArgs) Handles MyBase.Load
		Dim txt As String

		TbLoadingMax.Text = "-"
		PnLoad.Enabled = Not Cfg.DeclMode
		ButAxlAdd.Enabled = Not Cfg.DeclMode
		ButAxlRem.Enabled = Not Cfg.DeclMode
		CbCdMode.Enabled = Not Cfg.DeclMode
		PnWheelDiam.Enabled = Not Cfg.DeclMode

		AxlDlog = New F_VEH_Axle

		CbRim.Items.Add("-")
		For Each txt In Declaration.RimsList
			CbRim.Items.Add(txt)
		Next

		Changed = False

		newVEH()
	End Sub

	'Set HDVclasss
	Private Sub SetHDVclass()
		Dim VehC = CType(CbCat.SelectedIndex, tVehCat)
		Dim AxlC = CType(CbAxleConfig.SelectedIndex, tAxleConf)
		Dim MaxMass = CSng(fTextboxToNumString(TbMassMass.Text))

		Dim s0 As cSegmentTableEntry = Declaration.SegmentTable.SetRef(VehC, AxlC, MaxMass)
		HDVclass = "-"
		If Not s0 Is Nothing Then
			HDVclass = s0.HDVclass
		End If

		TbHDVclass.Text = HDVclass
		PicVehicle.Image = Image.FromFile(Declaration.ConvPicPath(HDVclass, False))
	End Sub


	'Set generic values for Declaration mode
	Private Sub DeclInit()
		If Not Cfg.DeclMode Then Exit Sub

		Dim vehC = CType(CbCat.SelectedIndex, tVehCat)
		Dim axlC = CType(CbAxleConfig.SelectedIndex, tAxleConf)
		Dim maxMass = CSng(fTextboxToNumString(TbMassMass.Text))
		Dim s0 = Declaration.SegmentTable.SetRef(vehC, axlC, maxMass)

		If Not s0 Is Nothing Then
			HDVclass = s0.HDVclass
			Dim axleCount As Short = s0.AxleShares(s0.Missions(0)).Count
			Dim i0 = LvRRC.Items.Count

			If axleCount > i0 Then
				For i = 1 To axleCount - LvRRC.Items.Count
					Dim lvi = New ListViewItem
					lvi.SubItems(0).Text = (i + i0).ToString
					lvi.SubItems.Add("-")
					lvi.SubItems.Add("no")
					lvi.SubItems.Add("")
					lvi.SubItems.Add("")
					lvi.SubItems.Add("-")
					lvi.SubItems.Add("-")
					LvRRC.Items.Add(lvi)
				Next

			ElseIf AxleCount < LvRRC.Items.Count Then
				For i = AxleCount To LvRRC.Items.Count - 1
					LvRRC.Items.RemoveAt(LvRRC.Items.Count - 1)
					'LvRRC.Items(i).ForeColor = Color.Red
				Next
			End If

			PnAll.Enabled = True

		Else
			PnAll.Enabled = False
			HDVclass = "-"
		End If

		TbMassExtra.Text = "-"
		TbLoad.Text = "-"
		CbCdMode.SelectedIndex = CType(tCdMode.CdOfVdecl, Integer)
		TbCdFile.Text = ""

		Dim rdyn As Single
		If LvRRC.Items.Count > 0 Then
			rdyn = Declaration.rdyn(LvRRC.Items(1).SubItems(5).Text, CbRim.Text)
		Else
			rdyn = -1
		End If

		If rdyn < 0 Then
			TBrdyn.Text = "-"
		Else
			TBrdyn.Text = rdyn
		End If
	End Sub


#Region "Toolbar"

	'New
	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		newVEH()
	End Sub

	'Open
	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If fbVEH.OpenDialog(VehFile) Then openVEH(fbVEH.Files(0))
	End Sub

	'Save
	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		SaveOrSaveAs(False)
	End Sub

	'Save As
	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		SaveOrSaveAs(True)
	End Sub

	'Send to VECTO Editor
	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click

		If ChangeCheckCancel() Then Exit Sub

		If VehFile = "" Then
			If MsgBox("Save file now?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
				If Not SaveOrSaveAs(True) Then Exit Sub
			Else
				Exit Sub
			End If
		End If


		If Not F_VECTO.Visible Then
			JobDir = ""
			F_VECTO.Show()
			F_VECTO.VECTOnew()
		Else
			F_VECTO.WindowState = FormWindowState.Normal
		End If

		F_VECTO.TbVEH.Text = fFileWoDir(VehFile, JobDir)
	End Sub

	'Help
	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim BrowserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim DefaultBrowserPath As String =
					Regex.Match(BrowserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(DefaultBrowserPath,
						String.Format("""{0}{1}""", MyAppPath, "User Manual\help.html#vehicle-editor"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

#End Region

	'Save and Close
	Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Close()
	End Sub

	'Cancel
	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub

	'Save or Save As function = true if file is saved
	Private Function SaveOrSaveAs(SaveAs As Boolean) As Boolean
		If VehFile = "" Or SaveAs Then
			If fbVEH.SaveDialog(VehFile) Then
				VehFile = fbVEH.Files(0)
			Else
				Return False
			End If
		End If
		Return saveVEH(VehFile)
	End Function

	'New VEH
	Private Sub newVEH()

		If ChangeCheckCancel() Then Exit Sub

		TbMass.Text = ""
		TbLoad.Text = ""
		TBrdyn.Text = ""
		TBcdA.Text = ""

		CbCdMode.SelectedIndex = 0
		TbCdFile.Text = ""

		CbRtType.SelectedIndex = 0
		TbRtRatio.Text = "1"
		TbRtPath.Text = ""

		CbCat.SelectedIndex = 0

		LvRRC.Items.Clear()

		TbMassMass.Text = ""
		TbMassExtra.Text = ""
		CbAxleConfig.SelectedIndex = 0

		CbRim.SelectedIndex = 0


		DeclInit()


		VehFile = ""
		Text = "VEH Editor"
		LbStatus.Text = ""

		Changed = False
	End Sub

	'Open VEH
	Sub openVEH(file As String)
		Dim i As Int16
		Dim VEH0 As cVEH
		Dim inertia As Single

		Dim a0 As cVEH.cAxle
		Dim lvi As ListViewItem

		If ChangeCheckCancel() Then Exit Sub

		VEH0 = New cVEH

		VEH0.FilePath = file

		If Not VEH0.ReadFile Then
			MsgBox("Cannot read " & file & "!")
			Exit Sub
		End If

		If Cfg.DeclMode <> VEH0.SavedInDeclMode Then
			Select Case WrongMode()
				Case 1
					Close()
					F_MAINForm.RbDecl.Checked = Not F_MAINForm.RbDecl.Checked
					F_MAINForm.OpenVectoFile(file)
				Case -1
					Exit Sub
				Case Else '0
					'Continue...
			End Select
		End If

		TbMass.Text = VEH0.Mass
		TbMassExtra.Text = VEH0.MassExtra
		TbLoad.Text = VEH0.Loading
		TBrdyn.Text = VEH0.rdyn
		CbRim.Text = VEH0.Rim


		CbCdMode.SelectedIndex = CType(VEH0.CdMode, Integer)
		TbCdFile.Text = VEH0.CdFile.OriginalPath

		CbRtType.SelectedIndex = CType(VEH0.RtType, Integer)
		TbRtRatio.Text = CStr(VEH0.RtRatio)
		TbRtPath.Text = CStr(VEH0.RtFile.OriginalPath)


		CbCat.SelectedIndex = CType(VEH0.VehCat, Integer)


		LvRRC.Items.Clear()
		i = 0
		For Each a0 In VEH0.Axles
			i += 1
			lvi = New ListViewItem
			lvi.SubItems(0).Text = i.ToString

			If Cfg.DeclMode Then
				lvi.SubItems.Add("-")
			Else
				lvi.SubItems.Add(a0.Share)
			End If

			If a0.TwinTire Then
				lvi.SubItems.Add("yes")
			Else
				lvi.SubItems.Add("no")
			End If
			lvi.SubItems.Add(a0.RRC)
			lvi.SubItems.Add(a0.FzISO)
			lvi.SubItems.Add(a0.Wheels)

			If Cfg.DeclMode Then
				inertia = Declaration.WheelsInertia(a0.Wheels)
				If inertia < 0 Then
					lvi.SubItems.Add("-")
				Else
					lvi.SubItems.Add(inertia)
				End If
			Else
				lvi.SubItems.Add(a0.Inertia)
			End If

			LvRRC.Items.Add(lvi)
		Next

		TbMassMass.Text = VEH0.MassMax
		TbMassExtra.Text = VEH0.MassExtra

		CbAxleConfig.SelectedIndex = CType(VEH0.AxleConf, Integer)

		TBcdA.Text = VEH0.CdA0

		DeclInit()

		fbVEH.UpdateHistory(file)
		Text = fFILE(file, True)
		LbStatus.Text = ""
		VehFile = file
		Activate()

		Changed = False
	End Sub

	'Save VEH
	Private Function saveVEH(file As String) As Boolean

		Dim veh0 = New cVEH
		veh0.FilePath = file

		veh0.Mass = CSng(fTextboxToNumString(TbMass.Text))
		veh0.MassExtra = CSng(fTextboxToNumString(TbMassExtra.Text))
		veh0.Loading = CSng(fTextboxToNumString(TbLoad.Text))

		veh0.CdA0 = CSng(fTextboxToNumString(TBcdA.Text))
		veh0.CdA02 = veh0.CdA0

		Dim vehC = CType(CbCat.SelectedIndex, tVehCat)
		Dim axlC = CType(CbAxleConfig.SelectedIndex, tAxleConf)
		Dim maxMass = CSng(fTextboxToNumString(TbMassMass.Text))
		Dim s0 As cSegmentTableEntry = Declaration.SegmentTable.SetRef(vehC, axlC, maxMass)
		If Not s0 Is Nothing Then
			If s0.HDVclass = "2" Then
				' CdA Addition for T1 Trailer
				veh0.CdA02 += 1.1
			End If
			If s0.HDVclass = "4" OrElse s0.HDVclass = "9" Then
				' CdA Addition for T2 Trailer
				veh0.CdA02 += 0.6
			End If
		End If

		veh0.Rim = CbRim.Text
		veh0.rdyn = CSng(fTextboxToNumString(TBrdyn.Text))
		veh0.CdMode = CType(CbCdMode.SelectedIndex, tCdMode)
		veh0.CdFile.Init(fPATH(file), TbCdFile.Text)
		veh0.RtType = CType(CbRtType.SelectedIndex, tRtType)
		veh0.RtRatio = CSng(fTextboxToNumString(TbRtRatio.Text))
		veh0.RtFile.Init(fPATH(file), TbRtPath.Text)
		veh0.VehCat = CType(CbCat.SelectedIndex, tVehCat)

		Dim axleShareCheck As Double
		For Each LV0 In LvRRC.Items
			Dim a0 = New cVEH.cAxle
			a0.Share = fTextboxToNumString(LV0.SubItems(1).Text)
			axleShareCheck += a0.Share
			a0.TwinTire = (LV0.SubItems(2).Text = "yes")
			a0.RRC = fTextboxToNumString(LV0.SubItems(3).Text)
			a0.FzISO = fTextboxToNumString(LV0.SubItems(4).Text)
			a0.Wheels = LV0.SubItems(5).Text
			a0.Inertia = fTextboxToNumString(LV0.SubItems(6).Text)
			veh0.Axles.Add(a0)
		Next

		If Not Cfg.DeclMode AndAlso Math.Abs(axleShareCheck - 1) > 0.000001 Then
			MsgBox("Relative axle loads must sum up to 1.0. Current value: " & axleShareCheck, MsgBoxStyle.Critical)
			Return False
		End If

		veh0.MassMax = CSng(fTextboxToNumString(TbMassMass.Text))
		veh0.MassExtra = CSng(fTextboxToNumString(TbMassExtra.Text))
		veh0.AxleConf = CType(CbAxleConfig.SelectedIndex, tAxleConf)

		'---------------------------------------------------------------------------------
		If Not veh0.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		If AutoSendTo Then
			If F_VECTO.Visible Then
				If UCase(fFileRepl(F_VECTO.TbVEH.Text, JobDir)) <> UCase(file) Then F_VECTO.TbVEH.Text = fFileWoDir(file, JobDir)
				F_VECTO.UpdatePic()
			End If
		End If

		fbVEH.UpdateHistory(file)
		Text = fFILE(file, True)
		LbStatus.Text = ""

		Changed = False

		Return True
	End Function

#Region "Cd"

	'Cd Mode Change
	Private Sub CbCdMode_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbCdMode.SelectedIndexChanged
		Dim bEnabled As Boolean

		Select Case CType(CbCdMode.SelectedIndex, tCdMode)

			Case tCdMode.CdOfBeta
				bEnabled = True
				LbCdMode.Text = "Input file: Yaw Angle [°], Cd Scaling Factor [-]"

			Case tCdMode.CdOfVeng
				bEnabled = True
				LbCdMode.Text = "Input file: Vehicle Speed [km/h], Cd Scaling Factor [-]"

			Case Else ' tCdMode.ConstCd0, tCdMode.CdOfVdecl
				bEnabled = False
				LbCdMode.Text = ""

		End Select

		If Not Cfg.DeclMode Then
			TbCdFile.Enabled = bEnabled
			BtCdFileBrowse.Enabled = bEnabled
			BtCdFileOpen.Enabled = bEnabled
		End If

		Change()
	End Sub

	'Cd File Browse
	Private Sub BtCdFileBrowse_Click(sender As Object, e As EventArgs) Handles BtCdFileBrowse.Click
		Dim ex As String

		If CbCdMode.SelectedIndex = 1 Then
			ex = "vcdv"
		Else
			ex = "vcdb"
		End If

		If fbCDx.OpenDialog(fFileRepl(TbCdFile.Text, fPATH(VehFile)), False, ex) Then _
			TbCdFile.Text = fFileWoDir(fbCDx.Files(0), fPATH(VehFile))
	End Sub

	'Open Cd File
	Private Sub BtCdFileOpen_Click(sender As Object, e As EventArgs) Handles BtCdFileOpen.Click
		OpenFiles(fFileRepl(TbCdFile.Text, fPATH(VehFile)))
	End Sub

#End Region

#Region "Retarder"

	'Rt Type Change
	Private Sub CbRtType_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbRtType.SelectedIndexChanged
		Select Case CbRtType.SelectedIndex
			Case 1 'Primary
				LbRtRatio.Text = "Ratio to engine speed"
				TbRtPath.Enabled = True
				BtRtBrowse.Enabled = True
				PnRt.Enabled = True
			Case 2 'Secondary
				LbRtRatio.Text = "Ratio to cardan shaft speed"
				TbRtPath.Enabled = True
				BtRtBrowse.Enabled = True
				PnRt.Enabled = True
			Case Else '0 None
				LbRtRatio.Text = "Ratio"
				TbRtPath.Enabled = False
				BtRtBrowse.Enabled = False
				PnRt.Enabled = False
		End Select

		Change()
	End Sub

	'Rt File Browse
	Private Sub BtRtBrowse_Click(sender As Object, e As EventArgs) Handles BtRtBrowse.Click

		If fbRLM.OpenDialog(fFileRepl(TbRtPath.Text, fPATH(VehFile))) Then _
			TbRtPath.Text = fFileWoDir(fbRLM.Files(0), fPATH(VehFile))
	End Sub

#End Region

#Region "Track changes"

	Private Sub Change()
		If Not Changed Then
			LbStatus.Text = "Unsaved changes in current file"
			Changed = True
		End If
	End Sub

	' "Save changes? "... Returns True if user aborts
	Private Function ChangeCheckCancel() As Boolean

		If Changed Then
			Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
				Case MsgBoxResult.Yes
					Return Not SaveOrSaveAs(False)
				Case MsgBoxResult.Cancel
					Return True
				Case Else 'MsgBoxResult.No
					Changed = False
					Return False
			End Select

		Else

			Return False

		End If
	End Function

	Private Sub TBmass_TextChanged(sender As Object, e As EventArgs) Handles TbMass.TextChanged
		SetMaxLoad()
		Change()
	End Sub

	Private Sub CbRim_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbRim.SelectedIndexChanged
		Change()
		DeclInit()
	End Sub

	Private Sub TBcw_TextChanged(sender As Object, e As EventArgs) _
		Handles TbLoad.TextChanged, TBrdyn.TextChanged, TBcdA.TextChanged, TbCdFile.TextChanged, TbRtPath.TextChanged,
				TbRtRatio.TextChanged
		Change()
	End Sub

	Private Sub CbCat_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbCat.SelectedIndexChanged
		Change()
		SetHDVclass()
		DeclInit()
	End Sub

	Private Sub TbMassTrailer_TextChanged(sender As Object, e As EventArgs) Handles TbMassExtra.TextChanged
		SetMaxLoad()
		Change()
	End Sub

	Private Sub TbMassMax_TextChanged(sender As Object, e As EventArgs) Handles TbMassMass.TextChanged
		SetMaxLoad()
		Change()
		SetHDVclass()
		DeclInit()
	End Sub

	Private Sub CbAxleConfig_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbAxleConfig.SelectedIndexChanged
		Change()
		SetHDVclass()
		DeclInit()
	End Sub

#End Region

	'Update maximum load when truck/trailer mass was changed
	Private Sub SetMaxLoad()
		If Not Cfg.DeclMode Then
			If IsNumeric(TbMass.Text) And IsNumeric(TbMassExtra.Text) And IsNumeric(TbMassMass.Text) Then
				TbLoadingMax.Text = CStr(CSng(TbMassMass.Text) * 1000 - CSng(TbMass.Text) - CSng(TbMassExtra.Text))
			Else
				TbLoadingMax.Text = ""
			End If
		End If
	End Sub

#Region "Axle Configuration"

	Private Sub ButAxlAdd_Click(sender As Object, e As EventArgs) Handles ButAxlAdd.Click
		Dim lv0 As ListViewItem

		AxlDlog.Clear()

		If AxlDlog.ShowDialog = DialogResult.OK Then
			lv0 = New ListViewItem

			lv0.SubItems(0).Text = LvRRC.Items.Count + 1
			lv0.SubItems.Add(Trim(AxlDlog.TbAxleShare.Text))
			If AxlDlog.CbTwinT.Checked Then
				lv0.SubItems.Add("yes")
			Else
				lv0.SubItems.Add("no")
			End If
			lv0.SubItems.Add(Trim(AxlDlog.TbRRC.Text))
			lv0.SubItems.Add(Trim(AxlDlog.TbFzISO.Text))
			lv0.SubItems.Add(Trim(AxlDlog.CbWheels.Text))
			lv0.SubItems.Add(Trim(AxlDlog.TbI_wheels.Text))

			LvRRC.Items.Add(lv0)

			Change()
			DeclInit()

		End If
	End Sub

	Private Sub ButAxlRem_Click(sender As Object, e As EventArgs) Handles ButAxlRem.Click
		RemoveAxleItem()
	End Sub

	Private Sub LvAxle_DoubleClick(sender As Object, e As EventArgs) Handles LvRRC.DoubleClick
		EditAxleItem()
	End Sub

	Private Sub LvAxle_KeyDown(sender As Object, e As KeyEventArgs) Handles LvRRC.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				If Not Cfg.DeclMode Then RemoveAxleItem()
			Case Keys.Enter
				EditAxleItem()
		End Select
	End Sub

	Private Sub RemoveAxleItem()
		Dim lv0 As ListViewItem
		Dim i As Integer

		If LvRRC.SelectedItems.Count = 0 Then
			If LvRRC.Items.Count = 0 Then
				Exit Sub
			Else
				LvRRC.Items(LvRRC.Items.Count - 1).Selected = True
			End If
		End If

		LvRRC.SelectedItems(0).Remove()

		If LvRRC.Items.Count > 0 Then

			i = 0
			For Each lv0 In LvRRC.Items
				i += 1
				lv0.SubItems(0).Text = i.ToString
			Next

			LvRRC.Items(LvRRC.Items.Count - 1).Selected = True
			LvRRC.Focus()
		End If

		Change()
	End Sub

	Private Sub EditAxleItem()
		If LvRRC.SelectedItems.Count = 0 Then Exit Sub

		Dim lv0 = LvRRC.SelectedItems(0)

		AxlDlog.TbAxleShare.Text = lv0.SubItems(1).Text
		AxlDlog.CbTwinT.Checked = (lv0.SubItems(2).Text = "yes")
		AxlDlog.TbRRC.Text = lv0.SubItems(3).Text
		AxlDlog.TbFzISO.Text = lv0.SubItems(4).Text
		AxlDlog.TbI_wheels.Text = lv0.SubItems(6).Text
		AxlDlog.CbWheels.Text = lv0.SubItems(5).Text

		If AxlDlog.ShowDialog = DialogResult.OK Then
			lv0.SubItems(1).Text = AxlDlog.TbAxleShare.Text
			If AxlDlog.CbTwinT.Checked Then
				lv0.SubItems(2).Text = "yes"
			Else
				lv0.SubItems(2).Text = "no"
			End If
			lv0.SubItems(3).Text = AxlDlog.TbRRC.Text
			lv0.SubItems(4).Text = AxlDlog.TbFzISO.Text
			lv0.SubItems(5).Text = AxlDlog.CbWheels.Text
			lv0.SubItems(6).Text = AxlDlog.TbI_wheels.Text

			Change()
			DeclInit()
		End If
	End Sub

#End Region

#Region "Open File Context Menu"

	Private CmFiles As String()

	Private Sub OpenFiles(ParamArray files() As String)
		If files.Length = 0 Then Exit Sub

		CmFiles = files
		OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName
		CmOpenFile.Show(Cursor.Position)
	End Sub

	Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles OpenWithToolStripMenuItem.Click
		If Not FileOpenAlt(CmFiles(0)) Then MsgBox("Failed to open file!")
	End Sub

	Private Sub ShowInFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles ShowInFolderToolStripMenuItem.Click
		If File.Exists(CmFiles(0)) Then
			Try
				Process.Start("explorer", "/select,""" & CmFiles(0) & "")
			Catch ex As Exception
				MsgBox("Failed to open file!")
			End Try
		Else
			MsgBox("File not found!")
		End If
	End Sub

#End Region
End Class

