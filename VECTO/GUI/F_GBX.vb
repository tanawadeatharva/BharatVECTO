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
Imports System.Collections.Generic
Imports System.Globalization
Imports System.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Configuration
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox

''' <summary>
''' Gearbox Editor
''' </summary>
''' <remarks></remarks>
Public Class F_GBX
	Private Enum GearboxTbl
		GearNr = 0
		TorqueConverter = 1
		Ratio = 2
		LossMapEfficiency = 3
		ShiftPolygons = 4
		FullLoadCurve = 5
	End Enum

	Private GbxFile As String = ""
	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""
	Private GearDia As GearboxGearDialog

	Private Init As Boolean = False

	Private Changed As Boolean = False

	'Before closing Editor: Check if file was changed and ask to save.
	Private Sub F_GBX_FormClosing(sender As Object, e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Initialise.
	Private Sub F_GBX_Load(sender As Object, e As System.EventArgs) Handles Me.Load

		Init = False
		GearDia = New GearboxGearDialog

		Me.PnInertiaTI.Enabled = Not Cfg.DeclMode
		Me.GrGearShift.Enabled = Not Cfg.DeclMode
		Me.ChTCon.Enabled = Not Cfg.DeclMode

		Me.CbGStype.Items.Clear()
		Me.CbGStype.Items.Add("Manual Transmission (MT)")
		Me.CbGStype.Items.Add("Automated Manual Transmission (AMT)")
		If Not Cfg.DeclMode Then
			Me.CbGStype.Items.Add("Automatic Transmission - Serial (AT-S)")
			Me.CbGStype.Items.Add("Automatic Transmission - PowerSplit (AT-P)")
			Me.CbGStype.Items.Add("Custom")
		End If

		Init = True

		DeclInit()

		Changed = False
		newGBX()
	End Sub

	'Set generic values for Declaration mode.
	Private Sub DeclInit()
		Dim GStype As tGearbox
		Dim lv0 As ListViewItem

		If Not Cfg.DeclMode Then Exit Sub

		Me.TBI_getr.Text = cDeclaration.GbInertia

		GStype = CType(Me.CbGStype.SelectedIndex, tGearbox)

		Me.TbTracInt.Text = cDeclaration.TracInt(GStype)
		Me.TbShiftTime.Text = cDeclaration.ShiftTime(GStype)

		Me.TbTqResv.Text = cDeclaration.TqResv
		Me.TbTqResvStart.Text = cDeclaration.TqResvStart
		Me.TbStartSpeed.Text = cDeclaration.StartSpeed
		Me.TbStartAcc.Text = cDeclaration.StartAcc

		tbUpshiftMinAcceleration.Text = cDeclaration.UpshiftMinAcceleration
		tbDownshiftAfterUpshift.Text = cDeclaration.DownshiftAfterUpshiftDelay
		tbUpshiftAfterDownshift.Text = cDeclaration.UpshiftAfterDownshiftDelay

		For Each lv0 In Me.LvGears.Items
			lv0.SubItems(4).Text = "-"
		Next
	End Sub

#Region "Toolbar"

	Private Sub ToolStripBtNew_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripBtNew.Click
		newGBX()
	End Sub

	Private Sub ToolStripBtOpen_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripBtOpen.Click
		If fbGBX.OpenDialog(GbxFile) Then openGBX(fbGBX.Files(0))
	End Sub

	Private Sub ToolStripBtSave_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripBtSave.Click
		SaveOrSaveAs(False)
	End Sub

	Private Sub ToolStripBtSaveAs_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripBtSaveAs.Click
		SaveOrSaveAs(True)
	End Sub

	Private Sub ToolStripBtSendTo_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripBtSendTo.Click

		If ChangeCheckCancel() Then Exit Sub

		If GbxFile = "" Then
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

		F_VECTO.TbGBX.Text = fFileWoDir(GbxFile, JobDir)
	End Sub

	'Help
	Private Sub ToolStripButton1_Click(sender As System.Object, e As System.EventArgs) Handles ToolStripButton1.Click
		If IO.File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim BrowserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim DefaultBrowserPath As String =
					System.Text.RegularExpressions.Regex.Match(BrowserRegistryString, "(\"".*?\"")").Captures(0).ToString
			System.Diagnostics.Process.Start(DefaultBrowserPath,
											String.Format("""{0}{1}""", MyAppPath, "User Manual\help.html#gearbox-editor"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

#End Region

	'New file
	Private Sub newGBX()
		'Dim lvi As ListViewItem

		If ChangeCheckCancel() Then Exit Sub

		Me.CbGStype.SelectedIndex = 0

		Me.TbName.Text = ""
		Me.TbTracInt.Text = ""
		Me.TBI_getr.Text = ""

		Me.LvGears.Items.Clear()

		Me.LvGears.Items.Add(CreateListviewItem("Axle", "-", 0, 0, "", ""))

		'Me.ChSkipGears.Checked = False         'set by CbGStype.SelectedIndexChanged
		'Me.ChShiftInside.Checked = False       'set by CbGStype.SelectedIndexChanged
		Me.TbTqResv.Text = ""
		Me.TbShiftTime.Text = ""
		Me.TbTqResvStart.Text = ""
		Me.TbStartSpeed.Text = ""
		Me.TbStartAcc.Text = ""

		'Me.ChTCon.Checked = False              'set by CbGStype.SelectedIndexChanged
		Me.TbTCfile.Text = ""
		Me.TbTCrefrpm.Text = ""
		Me.TbTCinertia.Text = ""

		DeclInit()

		GbxFile = ""
		Me.Text = "GBX Editor"
		Me.LbStatus.Text = ""

		Changed = False
		UpdatePic()
	End Sub

	'Open file
	Public Sub openGBX(ByVal file As String)
		Dim GBX0 As cGBX
		Dim i As Integer
		'Dim lv0 As ListViewItem

		If ChangeCheckCancel() Then Exit Sub

		GBX0 = New cGBX

		GBX0.FilePath = file

		If Not GBX0.ReadFile Then
			MsgBox("Cannot read " & file & "!")
			Exit Sub
		End If

		If Cfg.DeclMode <> GBX0.SavedInDeclMode Then
			Select Case WrongMode()
				Case 1
					Me.Close()
					F_MAINForm.RbDecl.Checked = Not F_MAINForm.RbDecl.Checked
					F_MAINForm.OpenVectoFile(file)
				Case -1
					Exit Sub
				Case Else '0
					'Continue...
			End Select
		End If

		Me.TbName.Text = GBX0.ModelName
		Me.TbTracInt.Text = GBX0.TracIntrSi.ToString
		Me.TBI_getr.Text = GBX0.GbxInertia.ToString

		Me.ChTCon.Checked = GBX0.TCon

		Me.LvGears.Items.Clear()

		For i = 0 To GBX0.Igetr.Count - 1

			If i = 0 Then
				'lv0 = New ListViewItem("Axle")
				Me.LvGears.Items.Add(CreateListviewItem("Axle", "-", GBX.Igetr(i), GBX.GetrMap(i, True), GBX.gsFile(i, True),
														GBX0.FldFile(i, True)))
			Else
				'lv0 = New ListViewItem(i.ToString("00"))
				Me.LvGears.Items.Add(CreateListviewItem("Axle", "-", GBX.Igetr(i), GBX.GetrMap(i, True), GBX.gsFile(i, True),
														GBX0.FldFile(i, True)))
			End If

			''If Me.ChTCon.Checked And i > 0 Then
			''	If False Then ' GBX0.IsTCgear(i) Then
			''		lv0.SubItems.Add("on")
			''	Else
			''		lv0.SubItems.Add("off")
			''	End If
			''Else
			'lv0.SubItems.Add("-")
			''End If
			'lv0.SubItems.Add(GBX0.Igetr(i))
			'lv0.SubItems.Add(GBX0.GetrMap(i, True))
			'lv0.SubItems.Add(GBX0.gsFile(i, True))
			'lv0.SubItems.Add(GBX0.FldFile(i, True))


		Next

		Me.ChSkipGears.Checked = GBX0.gs_SkipGears
		Me.TbTqResv.Text = GBX0.gs_TorqueResv.ToString
		Me.TbShiftTime.Text = GBX0.gs_ShiftTime.ToString
		Me.TbTqResvStart.Text = GBX0.gs_TorqueResvStart.ToString
		Me.TbStartSpeed.Text = GBX0.gs_StartSpeed.ToString
		Me.TbStartAcc.Text = GBX0.gs_StartAcc.ToString
		Me.ChShiftInside.Checked = GBX0.gs_ShiftInside

		Me.TbTCfile.Text = GBX0.TCfile(True)
		Me.TbTCrefrpm.Text = GBX0.TCrefrpm
		Me.TbTCinertia.Text = GBX0.TCinertia

		tbUpshiftMinAcceleration.Text = GBX0.UpshiftMinAcceleration
		tbDownshiftAfterUpshift.Text = GBX0.DownshiftAfterUpshift
		tbUpshiftAfterDownshift.Text = GBX0.UpshiftAfterDownshift

		If CType(GBX0.gs_Type, Integer) <= Me.CbGStype.Items.Count - 1 Then
			Me.CbGStype.SelectedIndex = CType(GBX0.gs_Type, Integer)
		Else
			Me.CbGStype.SelectedIndex = 0
		End If

		DeclInit()


		fbGBX.UpdateHistory(file)
		Me.Text = fFILE(file, True)
		Me.LbStatus.Text = ""
		GbxFile = file
		Me.Activate()

		Changed = False
		UpdatePic()
	End Sub

	Private Function CreateListviewItem(gear As String, tc As String, ratio As Single, getrMap As String,
										shiftPolygon As String, fldFile As String) As ListViewItem
		Dim retVal As ListViewItem = New ListViewItem(gear)
		retVal.SubItems.Add(tc)
		retVal.SubItems.Add(ratio)
		retVal.SubItems.Add(getrMap)
		retVal.SubItems.Add(shiftPolygon)
		retVal.SubItems.Add(fldFile)
		Return retVal
	End Function

	'Save or Save As function = true if file is saved
	Private Function SaveOrSaveAs(ByVal SaveAs As Boolean) As Boolean
		If GbxFile = "" Or SaveAs Then
			If fbGBX.SaveDialog(GbxFile) Then
				GbxFile = fbGBX.Files(0)
			Else
				Return False
			End If
		End If
		Return saveGBX(GbxFile)
	End Function

	'Save file
	Private Function saveGBX(ByVal file As String) As Boolean
		Dim GBX0 As cGBX
		Dim i As Int16

		GBX0 = New cGBX
		GBX0.FilePath = file

		GBX0.ModelName = Me.TbName.Text
		If Trim(GBX0.ModelName) = "" Then GBX0.ModelName = "Undefined"

		GBX0.TracIntrSi = fTextboxToNumString(Me.TbTracInt.Text)
		GBX0.GbxInertia = fTextboxToNumString(Me.TBI_getr.Text)

		For i = 0 To Me.LvGears.Items.Count - 1
			'GBX0.IsTCgear.Add(Me.LvGears.Items(i).SubItems(GearboxTbl.TorqueConverter).Text = "on" And i > 0)
			GBX0.Igetr.Add(CSng(Me.LvGears.Items(i).SubItems(GearboxTbl.Ratio).Text))
			GBX0.GetrMaps.Add(New cSubPath)
			GBX0.GetrMap(i) = Me.LvGears.Items(i).SubItems(GearboxTbl.LossMapEfficiency).Text
			GBX0.gs_files.Add(New cSubPath)
			GBX0.gsFile(i) = Me.LvGears.Items(i).SubItems(GearboxTbl.ShiftPolygons).Text
			GBX0.FldFiles.Add(New cSubPath)
			GBX0.FldFile(i) = Me.LvGears.Items(i).SubItems(GearboxTbl.FullLoadCurve).Text
		Next

		GBX0.gs_TorqueResv = fTextboxToNumString(Me.TbTqResv.Text)
		GBX0.gs_SkipGears = Me.ChSkipGears.Checked
		GBX0.gs_ShiftTime = fTextboxToNumString(Me.TbShiftTime.Text)
		GBX0.gs_TorqueResvStart = fTextboxToNumString(Me.TbTqResvStart.Text)
		GBX0.gs_StartSpeed = fTextboxToNumString(Me.TbStartSpeed.Text)
		GBX0.gs_StartAcc = fTextboxToNumString(Me.TbStartAcc.Text)
		GBX0.gs_ShiftInside = Me.ChShiftInside.Checked

		GBX0.gs_Type = CType(Me.CbGStype.SelectedIndex, tGearbox)

		GBX0.TCon = Me.ChTCon.Checked
		GBX0.TCfile = Me.TbTCfile.Text
		GBX0.TCrefrpm = fTextboxToNumString(Me.TbTCrefrpm.Text)
		GBX0.TCinertia = fTextboxToNumString(Me.TbTCinertia.Text)

		GBX0.DownshiftAfterUpshift = fTextboxToNumString(tbDownshiftAfterUpshift.Text)
		GBX0.UpshiftAfterDownshift = fTextboxToNumString(tbUpshiftAfterDownshift.Text)
		GBX0.UpshiftMinAcceleration = fTextboxToNumString(tbUpshiftMinAcceleration.Text)

		If Not GBX0.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		If AutoSendTo Then
			If F_VECTO.Visible Then
				If UCase(fFileRepl(F_VECTO.TbGBX.Text, JobDir)) <> UCase(file) Then F_VECTO.TbGBX.Text = fFileWoDir(file, JobDir)
				F_VECTO.UpdatePic()
			End If
		End If

		fbGBX.UpdateHistory(file)
		Me.Text = fFILE(file, True)
		Me.LbStatus.Text = ""

		Changed = False

		Return True
	End Function

#Region "Change Events"

	'Change Status ändern |@@| Change Status change
	Private Sub Change()
		If Not Changed Then
			Me.LbStatus.Text = "Unsaved changes in current file"
			Changed = True
		End If
	End Sub

	' "Save changes ?" ...liefert True wenn User Vorgang abbricht |@@| Save changes? "... Returns True if user aborts
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

	Private Sub TbName_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbName.TextChanged
		Change()
	End Sub

	Private Sub TBI_getr_TextChanged(sender As System.Object, e As System.EventArgs) Handles TBI_getr.TextChanged
		Change()
	End Sub

	Private Sub TbTracInt_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbTracInt.TextChanged
		Change()
	End Sub

	Private Sub ChSkipGears_CheckedChanged(sender As System.Object, e As System.EventArgs) _
		Handles ChSkipGears.CheckedChanged
		CheckEnableTorqRes()
		Change()
	End Sub

	Private Sub ChShiftInside_CheckedChanged(sender As System.Object, e As System.EventArgs) _
		Handles ChShiftInside.CheckedChanged
		CheckEnableTorqRes()
		Change()
	End Sub

	Private Sub TbTqResv_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbTqResv.TextChanged
		Change()
	End Sub

	Private Sub TbShiftTime_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbShiftTime.TextChanged
		Change()
	End Sub

	Private Sub TbTqResvStart_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbTqResvStart.TextChanged
		Change()
	End Sub

	Private Sub TbStartSpeed_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbStartSpeed.TextChanged
		Change()
	End Sub

	Private Sub TbStartAcc_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbStartAcc.TextChanged
		Change()
	End Sub

	Private Sub TbTCfile_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbTCfile.TextChanged
		Change()
	End Sub

	Private Sub TbTCrefrpm_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbTCrefrpm.TextChanged
		Change()
	End Sub

	Private Sub TbTCinertia_TextChanged(sender As System.Object, e As System.EventArgs) Handles TbTCinertia.TextChanged
		Change()
	End Sub


	Private Sub CheckEnableTorqRes()
		If Me.ChShiftInside.Checked Or Me.ChSkipGears.Checked Then
			Me.PnTorqRes.Enabled = True
		Else
			Me.PnTorqRes.Enabled = False
		End If
	End Sub


#End Region

	'Save and close
	Private Sub ButOK_Click(sender As System.Object, e As System.EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Me.Close()
	End Sub

	'Cancel
	Private Sub ButCancel_Click(sender As System.Object, e As System.EventArgs) Handles ButCancel.Click
		Me.Close()
	End Sub

	'Enable/Disable settings for specific transmission types
	Private Sub CbGStype_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
		Handles CbGStype.SelectedIndexChanged
		Dim GStype As tGearbox

		Change()

		GStype = CType(Me.CbGStype.SelectedIndex, tGearbox)

		Me.ChShiftInside.Enabled = (GStype = tGearbox.Custom)
		Me.ChSkipGears.Enabled = (GStype = tGearbox.Custom)
		Me.ChTCon.Enabled = (GStype = tGearbox.Custom)

		If GStype <> tGearbox.Custom Then
			Me.ChShiftInside.Checked = cDeclaration.ShiftInside(GStype)
			Me.ChSkipGears.Checked = cDeclaration.SkipGears(GStype)
			Me.ChTCon.Checked = (GStype = tGearbox.AutomaticSerial OrElse GStype = tGearbox.AutomaticPowerSplit)
		End If
	End Sub


	Private Sub LvGears_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) _
		Handles LvGears.SelectedIndexChanged
		UpdatePic()
	End Sub

#Region "Gears"

	'Gear-DoubleClick
	Private Sub LvGears_MouseDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) _
		Handles LvGears.MouseDoubleClick
		EditGear()
	End Sub

	'Gear-KeyDown
	Private Sub LvGears_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles LvGears.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				RemoveGear(False)
			Case Keys.Enter
				EditGear()
		End Select
	End Sub

	'Remove Gear Button
	Private Sub BtClearGear_Click(sender As System.Object, e As System.EventArgs) Handles BtRemGear.Click
		RemoveGear(False)
	End Sub

	'Add Gear button
	Private Sub BtAddGear_Click(sender As System.Object, e As System.EventArgs) Handles BtAddGear.Click
		AddGear()
		Me.LvGears.Items(Me.LvGears.Items.Count - 1).Selected = True
		EditGear()
	End Sub

	'Edit Gear
	Private Sub EditGear()

		Do

			'GearDia.ChIsTCgear.Enabled = (Me.ChTCon.Checked And Me.LvGears.SelectedIndices(0) > 0)
			GearDia.PnShiftPoly.Enabled = (Not Cfg.DeclMode And Me.LvGears.SelectedIndices(0) > 0)
			GearDia.PnFld.Enabled = (Me.LvGears.SelectedIndices(0) > 0)
			GearDia.GbxPath = fPATH(GbxFile)
			GearDia.TbGear.Text = Me.LvGears.SelectedItems(0).SubItems(0).Text
			GearDia.TbRatio.Text = Me.LvGears.SelectedItems(0).SubItems(2).Text
			GearDia.TbMapPath.Text = Me.LvGears.SelectedItems(0).SubItems(3).Text
			If Me.LvGears.SelectedIndices(0) > 0 Then
				GearDia.TbShiftPolyFile.Text = Me.LvGears.SelectedItems(0).SubItems(4).Text
				GearDia.TbFld.Text = Me.LvGears.SelectedItems(0).SubItems(5).Text
			Else
				GearDia.TbShiftPolyFile.Text = ""
				GearDia.TbFld.Text = ""
			End If

			If LvGears.SelectedItems(0).Index = 0 Then
				GearDia.BtPrevious.Enabled = False
			Else
				GearDia.BtPrevious.Enabled = True
			End If

			If GearDia.ShowDialog = Windows.Forms.DialogResult.OK Then

				'If GearDia.ChIsTCgear.Checked Then
				'	Me.LvGears.SelectedItems(0).SubItems(1).Text = "on"
				'Else
				'	If Me.ChTCon.Checked Then
				'		Me.LvGears.SelectedItems(0).SubItems(1).Text = "off"
				'	Else
				Me.LvGears.SelectedItems(0).SubItems(1).Text = "-"
				'	End If
				'End If

				Me.LvGears.SelectedItems(0).SubItems(2).Text = GearDia.TbRatio.Text
				Me.LvGears.SelectedItems(0).SubItems(3).Text = GearDia.TbMapPath.Text
				Me.LvGears.SelectedItems(0).SubItems(4).Text = GearDia.TbShiftPolyFile.Text
				Me.LvGears.SelectedItems(0).SubItems(5).Text = GearDia.TbFld.Text

				UpdatePic()
				Change()

			Else

				If LvGears.SelectedItems(0).SubItems(2).Text = "" Then RemoveGear(True)

			End If

			If GearDia.NextGear Then
				If LvGears.Items.Count - 1 = LvGears.SelectedIndices(0) Then AddGear()

				LvGears.Items(LvGears.SelectedIndices(0) + 1).Selected = True
			End If

			If GearDia.PreviousGear AndAlso LvGears.SelectedIndices(0) > 0 Then
				LvGears.Items(LvGears.SelectedIndices(0) - 1).Selected = True
			End If

		Loop Until Not (GearDia.NextGear OrElse GearDia.PreviousGear)
	End Sub

	'Add Gear
	Private Sub AddGear()
		Dim lvi As ListViewItem

		lvi = New ListViewItem(Me.LvGears.Items.Count.ToString("00"))
		'If Me.ChTCon.Checked Then
		'	lvi.SubItems.Add("off")
		'Else
		lvi.SubItems.Add("-")
		'End If
		lvi.SubItems.Add("")
		lvi.SubItems.Add("")
		lvi.SubItems.Add("")
		lvi.SubItems.Add("")
		Me.LvGears.Items.Add(lvi)

		lvi.EnsureVisible()

		Me.LvGears.Focus()

		'Change() => NO! Change() is already handled by EditGear
	End Sub

	'Remove Gear
	Private Sub RemoveGear(ByVal NoChange As Boolean)
		Dim i0 As Int16
		Dim i As Int16
		Dim lv0 As ListViewItem

		If Me.LvGears.Items.Count < 2 Then Exit Sub

		If Me.LvGears.SelectedItems.Count = 0 Then Me.LvGears.Items(Me.LvGears.Items.Count - 1).Selected = True

		i0 = Me.LvGears.SelectedItems(0).Index

		If i0 = 0 Then Exit Sub 'Must not remove axle

		Me.LvGears.SelectedItems(0).Remove()

		i = 0
		For Each lv0 In Me.LvGears.Items
			If lv0.SubItems(0).Text = "Axle" Then Continue For
			i += 1
			lv0.SubItems(0).Text = i.ToString("00")
		Next

		If i0 < Me.LvGears.Items.Count Then
			Me.LvGears.Items(i0).Selected = True
			Me.LvGears.Items(i0).EnsureVisible()
		End If

		Me.LvGears.Focus()
		UpdatePic()

		If Not NoChange Then Change()
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

	Private Sub OpenWithToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) _
		Handles OpenWithToolStripMenuItem.Click
		If Not FileOpenAlt(CmFiles(0)) Then MsgBox("Failed to open file!")
	End Sub

	Private Sub ShowInFolderToolStripMenuItem_Click(sender As System.Object, e As System.EventArgs) _
		Handles ShowInFolderToolStripMenuItem.Click
		If IO.File.Exists(CmFiles(0)) Then
			Try
				System.Diagnostics.Process.Start("explorer", "/select,""" & CmFiles(0) & "")
			Catch ex As Exception
				MsgBox("Failed to open file!")
			End Try
		Else
			MsgBox("File not found!")
		End If
	End Sub

#End Region


	Private Sub UpdatePic()

		Dim f As cFile_V3 = Nothing
		Dim path As String
		Dim lM As List(Of Single) = Nothing
		Dim lup As List(Of Single) = Nothing
		Dim ldown As List(Of Single) = Nothing
		Dim line As String() = Nothing
		Dim MyChart As System.Windows.Forms.DataVisualization.Charting.Chart
		Dim s As System.Windows.Forms.DataVisualization.Charting.Series
		Dim a As System.Windows.Forms.DataVisualization.Charting.ChartArea
		Dim img As Image
		Dim Gear As Integer
		Dim fldOK As Boolean = False
		Dim fldpath As String
		Dim FLD0 As cFLD = Nothing
		Dim ShiftOK As Boolean = False
		Dim Shiftpoly As cGBX.cShiftPolygon

		Me.PicBox.Image = Nothing

		Try

			'Check Files
			If Me.LvGears.Items.Count > 1 Then

				If Me.LvGears.SelectedItems.Count > 0 AndAlso Me.LvGears.SelectedIndices(0) > 0 Then
					path = fFileRepl(Me.LvGears.SelectedItems(0).SubItems(4).Text, fPATH(GbxFile))
					fldpath = fFileRepl(Me.LvGears.SelectedItems(0).SubItems(5).Text, fPATH(GbxFile))
					Gear = Me.LvGears.SelectedIndices(0)
				Else
					path = fFileRepl(Me.LvGears.Items(1).SubItems(4).Text, fPATH(GbxFile))
					fldpath = fFileRepl(Me.LvGears.Items(1).SubItems(5).Text, fPATH(GbxFile))
					Gear = 1
				End If

				f = New cFile_V3
				ShiftOK = f.OpenRead(path)

				If fldpath.Trim = "" Then
					If F_VECTO.Visible AndAlso F_VECTO.FLDfile <> "" Then fldpath = F_VECTO.FLDfile
				End If

				fldOK = fldpath.Trim <> ""

				If fldOK Then
					FLD0 = New cFLD
					FLD0.FilePath = fldpath
					fldOK = FLD0.ReadFile(True, False)
				End If

			Else

				Exit Sub

			End If

		Catch ex As Exception
			Exit Sub

		End Try

		'Read ShiftPolygon
		If ShiftOK Then

			'Header
			f.ReadLine()

			Try
				lM = New List(Of Single)
				lup = New List(Of Single)
				ldown = New List(Of Single)

				Do While Not f.EndOfFile
					line = f.ReadLine
					lM.Add(CSng(line(0)))
					lup.Add(CSng(line(1)))
					ldown.Add(CSng(line(2)))
				Loop

				f.Close()

			Catch ex As Exception
				f.Close()
				Exit Sub
			End Try

			If lM.Count < 2 Then ShiftOK = False

		End If


		'Create plot
		If Not ShiftOK And Not fldOK Then Exit Sub

		MyChart = New System.Windows.Forms.DataVisualization.Charting.Chart
		MyChart.Width = Me.PicBox.Width
		MyChart.Height = Me.PicBox.Height

		a = New System.Windows.Forms.DataVisualization.Charting.ChartArea

		'Shiftpolygons from file
		If ShiftOK Then
			s = New System.Windows.Forms.DataVisualization.Charting.Series
			s.Points.DataBindXY(lup, lM)
			s.ChartType = DataVisualization.Charting.SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkRed
			s.Name = "Upshift curve"
			MyChart.Series.Add(s)

			s = New System.Windows.Forms.DataVisualization.Charting.Series
			s.Points.DataBindXY(ldown, lM)
			s.ChartType = DataVisualization.Charting.SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkRed
			s.Name = "Downshift curve"
			MyChart.Series.Add(s)
		End If

		Dim vectoJob As cVECTO = New cVECTO() With {.FilePath = F_VECTO.VECTOfile}
		Dim vectoOk As Boolean = vectoJob.ReadFile()
		Dim vehicle As cVEH = New cVEH() With {.FilePath = vectoJob.PathVeh(False)}
		Dim vehicleOk As Boolean = vehicle.ReadFile(False)

		'Fld
		If fldOK AndAlso vectoOk AndAlso vehicleOk Then

			s = New System.Windows.Forms.DataVisualization.Charting.Series
			s.Points.DataBindXY(FLD0.LnU, FLD0.LTq)
			s.ChartType = DataVisualization.Charting.SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkBlue
			s.Name = "Full load"
			MyChart.Series.Add(s)

			If F_VECTO.Visible AndAlso F_VECTO.n_idle > 0 Then
				If FLD0.Init(F_VECTO.n_idle) Then

					Shiftpoly = New cGBX.cShiftPolygon("", 0)
					Shiftpoly.SetGenericShiftPoly(FLD0, F_VECTO.n_idle)
					'Dim fullLoadCurve As FullLoadCurve = ConvertToFullLoadCurve(FLD0.LnU, FLD0.LTq)
					Dim gears As IList(Of ITransmissionInputData) = ConvertToGears(LvGears.Items)
					If (gears.Count > 1) Then
						Dim engine As CombustionEngineData = ConvertToEngineData(FLD0, F_VECTO.n_idle)
						Dim shiftLines As ShiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(Gear - 1, engine.FullLoadCurve, gears,
																									engine,
																									Double.Parse(LvGears.Items(0).SubItems(2).Text, CultureInfo.InvariantCulture),
																									(vehicle.rdyn / 1000.0).SI(Of Meter))

						s = New System.Windows.Forms.DataVisualization.Charting.Series

						's.Points.DataBindXY(Shiftpoly.gs_nUup, Shiftpoly.gs_TqUp)
						s.Points.DataBindXY(
							shiftLines.Upshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
							shiftLines.Upshift.Select(Function(pt) pt.Torque.Value()).ToList())
						s.ChartType = DataVisualization.Charting.SeriesChartType.FastLine
						s.BorderWidth = 2
						s.Color = Color.DarkRed
						s.BorderDashStyle = DataVisualization.Charting.ChartDashStyle.Dash
						s.Name = "Upshift curve (generic)"
						MyChart.Series.Add(s)

						s = New System.Windows.Forms.DataVisualization.Charting.Series
						's.Points.DataBindXY(Shiftpoly.gs_nUdown, Shiftpoly.gs_TqDown)
						s.Points.DataBindXY(
							shiftLines.Downshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
							shiftLines.Downshift.Select(Function(pt) pt.Torque.Value()).ToList())
						s.ChartType = DataVisualization.Charting.SeriesChartType.FastLine
						s.BorderWidth = 2
						s.Color = Color.DarkRed
						s.BorderDashStyle = DataVisualization.Charting.ChartDashStyle.Dash
						s.Name = "Downshift curve (generic)"
						MyChart.Series.Add(s)
					End If
				End If
			End If

		End If


		a.Name = "main"

		a.AxisX.Title = "engine speed [1/min]"
		a.AxisX.TitleFont = New Font("Helvetica", 10)
		a.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
		a.AxisX.LabelAutoFitStyle = DataVisualization.Charting.LabelAutoFitStyles.None
		a.AxisX.MajorGrid.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dot

		a.AxisY.Title = "engine torque [Nm]"
		a.AxisY.TitleFont = New Font("Helvetica", 10)
		a.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
		a.AxisY.LabelAutoFitStyle = DataVisualization.Charting.LabelAutoFitStyles.None
		a.AxisY.MajorGrid.LineDashStyle = DataVisualization.Charting.ChartDashStyle.Dot

		a.AxisX.Minimum = 300
		a.BorderDashStyle = DataVisualization.Charting.ChartDashStyle.Solid
		a.BorderWidth = 1

		a.BackColor = Color.GhostWhite

		MyChart.ChartAreas.Add(a)

		MyChart.Titles.Add("Gear " & Gear & " shift polygons")
		MyChart.Titles(0).Font = New Font("Helvetica", 12)

		MyChart.Update()

		img = New Bitmap(MyChart.Width, MyChart.Height, Imaging.PixelFormat.Format32bppArgb)
		MyChart.DrawToBitmap(img, New Rectangle(0, 0, Me.PicBox.Width, Me.PicBox.Height))

		Me.PicBox.Image = img
	End Sub

	Private Function ConvertToEngineData(fld As cFLD, nIdle As Single) As CombustionEngineData

		Dim retVal As CombustionEngineData = New CombustionEngineData()
		retVal.FullLoadCurve = New EngineFullLoadCurve()
		retVal.FullLoadCurve.FullLoadEntries = New List(Of FullLoadCurve.FullLoadCurveEntry)
		For i As Integer = 0 To fld.LnU.Count - 1
			retVal.FullLoadCurve.FullLoadEntries.Add(
				New FullLoadCurve.FullLoadCurveEntry() _
														With {.EngineSpeed = CType(fld.LnU(i), Double).RPMtoRad(),
														.TorqueFullLoad = CType(fld.LTq(i), Double).SI(Of NewtonMeter)(),
														.TorqueDrag = CType(fld.LTqDrag(i), Double).SI(Of NewtonMeter)()})
		Next

		retVal.IdleSpeed = CType(nIdle, Double).RPMtoRad()
		Return retVal
	End Function

	Private Function ConvertToGears(gbx As ListView.ListViewItemCollection) As IList(Of ITransmissionInputData)
		Dim retVal As List(Of ITransmissionInputData) = New List(Of ITransmissionInputData)
		Dim value As Double

		For i As Integer = 1 To gbx.Count - 1
			If gbx(i).SubItems(2).Text <> "" AndAlso Double.TryParse(gbx(i).SubItems(2).Text, value) Then
				retVal.Add(
					New TransmissionInputData() _
							With {.Ratio = Double.Parse(value, CultureInfo.InvariantCulture)})

			End If
		Next
		Return retVal
	End Function


#Region "Torque Converter"

	'TC on/off
	Private Sub ChTCon_CheckedChanged(sender As System.Object, e As System.EventArgs) Handles ChTCon.CheckedChanged
		Change()
		CheckGearTC()
		PnTC.Enabled = ChTCon.Checked
	End Sub

	'Browse TC file
	Private Sub BtTCfileBrowse_Click(sender As System.Object, e As System.EventArgs) Handles BtTCfileBrowse.Click
		If fbTCC.OpenDialog(fFileRepl(Me.TbTCfile.Text, fPATH(GbxFile))) Then
			Me.TbTCfile.Text = fFileWoDir(fbTCC.Files(0), fPATH(GbxFile))
		End If
	End Sub

	'Open TC file
	Private Sub BtTCfileOpen_Click(sender As System.Object, e As System.EventArgs) Handles BtTCfileOpen.Click
		OpenFiles(fFileRepl(Me.TbTCfile.Text, fPATH(GbxFile)))
	End Sub

	Private Sub CheckGearTC()
		Dim lv0 As ListViewItem

		If Not Init Then Exit Sub

		For Each lv0 In Me.LvGears.Items

			If lv0.SubItems(0).Text = "Axle" Then Continue For

			'If Me.ChTCon.Checked Then
			'	If lv0.Index = 1 Then
			'		lv0.SubItems(1).Text = "on"
			'	Else
			'		lv0.SubItems(1).Text = "off"
			'	End If
			'Else
			lv0.SubItems(1).Text = "-"
			'End If
		Next
	End Sub


#End Region

	Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter
	End Sub

	Public Sub New()

		' Dieser Aufruf ist für den Designer erforderlich.
		InitializeComponent()

		' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
	End Sub
End Class
