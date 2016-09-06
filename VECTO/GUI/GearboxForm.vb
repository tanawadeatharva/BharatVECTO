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
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Configuration
Imports TUGraz.VectoCore.InputData.Impl
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox

''' <summary>
''' Gearbox Editor
''' </summary>
''' <remarks></remarks>
Public Class GearboxForm
	Private Enum GearboxTbl
		GearNr = 0
		'TorqueConverter = 1
		Ratio = 1
		LossMapEfficiency = 2
		ShiftPolygons = 3
		MaxTorque = 4
	End Enum

	Private GbxFile As String = ""
	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""
	Private GearDia As GearboxGearDialog

	Private Init As Boolean = False

	Private Changed As Boolean = False

	'Before closing Editor: Check if file was changed and ask to save.
	Private Sub F_GBX_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Initialise.
	Private Sub F_GBX_Load(sender As Object, e As EventArgs) Handles Me.Load

		Init = False
		GearDia = New GearboxGearDialog

		PnInertiaTI.Enabled = Not Cfg.DeclMode
		GrGearShift.Enabled = Not Cfg.DeclMode
		'ChTCon.Enabled = Not Cfg.DeclMode

		CbGStype.Items.Clear()
		CbGStype.ValueMember = "Value"
		CbGStype.DisplayMember = "Label"

		If Cfg.DeclMode Then
			CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
				.Cast(Of GearboxType)() _
				.Where(Function(type) type.ManualTransmission()) _
				.Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()
		Else
			CbGStype.DataSource = [Enum].GetValues(GetType(GearboxType)) _
				.Cast(Of GearboxType) _
				.Where(Function(type) type.AutomaticTransmission() OrElse type.ManualTransmission()) _
				.Select(Function(type) New With {Key .Value = type, .Label = type.GetLabel()}).ToList()
		End If

		Init = True

		DeclInit()

		Changed = False
		newGBX()
	End Sub

	'Set generic values for Declaration mode.
	Private Sub DeclInit()
		Dim GStype As GearboxType
		Dim lv0 As ListViewItem

		If Not Cfg.DeclMode Then Exit Sub

		TBI_getr.Text = DeclarationData.Gearbox.Inertia.Value()	'cDeclaration.GbInertia

		GStype = CbGStype.SelectedValue	'CType(Me.CbGStype.SelectedIndex, tGearbox)

		TbTracInt.Text = GStype.TractionInterruption().Value()
		TbShiftTime.Text = DeclarationData.Gearbox.MinTimeBetweenGearshifts.Value()	'cDeclaration.ShiftTime(GStype)

		TbTqResv.Text = DeclarationData.Gearbox.TorqueReserve ' cDeclaration.TqResv
		TbTqResvStart.Text = DeclarationData.Gearbox.TorqueReserveStart	'cDeclaration.TqResvStart
		TbStartSpeed.Text = DeclarationData.Gearbox.StartSpeed.Value() 'cDeclaration.StartSpeed
		TbStartAcc.Text = DeclarationData.Gearbox.StartAcceleration.Value()	' cDeclaration.StartAcc

		tbUpshiftMinAcceleration.Text = DeclarationData.Gearbox.UpshiftMinAcceleration.Value()
		tbDownshiftAfterUpshift.Text = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay.Value()
		tbUpshiftAfterDownshift.Text = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay.Value()

		'ChTCon.Checked = GStype.AutomaticTransmission()
		For Each lv0 In LvGears.Items
			lv0.SubItems(GearboxTbl.ShiftPolygons).Text = "-"
		Next
	End Sub

#Region "Toolbar"

	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		newGBX()
	End Sub

	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If GearboxFileBrowser.OpenDialog(GbxFile) Then openGBX(GearboxFileBrowser.Files(0))
	End Sub

	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		SaveOrSaveAs(False)
	End Sub

	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		SaveOrSaveAs(True)
	End Sub

	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click

		If ChangeCheckCancel() Then Exit Sub

		If GbxFile = "" Then
			If MsgBox("Save file now?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
				If Not SaveOrSaveAs(True) Then Exit Sub
			Else
				Exit Sub
			End If
		End If

		If Not VectoJobForm.Visible Then
			JobDir = ""
			VectoJobForm.Show()
			VectoJobForm.VECTOnew()
		Else
			VectoJobForm.WindowState = FormWindowState.Normal
		End If

		VectoJobForm.TbGBX.Text = GetFilenameWithoutDirectory(GbxFile, JobDir)
	End Sub

	'Help
	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim BrowserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim DefaultBrowserPath As String =
					Regex.Match(BrowserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(DefaultBrowserPath,
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

		'CbGStype.SelectedIndex = 0

		TbName.Text = ""
		TbTracInt.Text = ""
		TBI_getr.Text = ""

		LvGears.Items.Clear()

		LvGears.Items.Add(CreateListviewItem("Axle", "-", 0, 0, "", ""))

		'Me.ChSkipGears.Checked = False         'set by CbGStype.SelectedIndexChanged
		'Me.ChShiftInside.Checked = False       'set by CbGStype.SelectedIndexChanged
		TbTqResv.Text = ""
		TbShiftTime.Text = ""
		TbTqResvStart.Text = ""
		TbStartSpeed.Text = ""
		TbStartAcc.Text = ""

		'ChTCon.Checked = False				'set by CbGStype.SelectedIndexChanged
		TbTCfile.Text = ""
		TbTCrefrpm.Text = ""
		TbTCinertia.Text = ""

		DeclInit()

		GbxFile = ""
		Text = "GBX Editor"
		LbStatus.Text = ""

		Changed = False
		UpdatePic()
	End Sub

	'Open file
	Public Sub openGBX(ByVal file As String)
		Dim GBX0 As Gearbox
		Dim i As Integer
		'Dim lv0 As ListViewItem

		If ChangeCheckCancel() Then Exit Sub

		GBX0 = New Gearbox

		GBX0.FilePath = file

		If Not GBX0.ReadFile Then
			MsgBox("Cannot read " & file & "!")
			Exit Sub
		End If

		If Cfg.DeclMode <> GBX0.SavedInDeclMode Then
			Select Case WrongMode()
				Case 1
					Close()
					MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
					MainForm.OpenVectoFile(file)
				Case -1
					Exit Sub
				Case Else '0
					'Continue...
			End Select
		End If

		TbName.Text = GBX0.ModelName
		TbTracInt.Text = GBX0.TracIntrSi.ToString
		TBI_getr.Text = GBX0.GbxInertia.ToString

		'ChTCon.Checked = GBX0.TorqueConverterEnabled

		LvGears.Items.Clear()

		For i = 0 To GBX0.GearRatios.Count - 1

			If i = 0 Then
				'lv0 = New ListViewItem("Axle")
				LvGears.Items.Add(CreateListviewItem("Axle", "-", GBX0.GearRatios(i), GBX0.GearLossMap(i, True),
													GBX0.ShiftPolygonFile(i, True),
													GBX0.MaxTorque(i)))
			Else
				'lv0 = New ListViewItem(i.ToString("00"))
				LvGears.Items.Add(CreateListviewItem(i.ToString("00"), "-", GBX0.GearRatios(i), GBX0.GearLossMap(i, True),
													GBX0.ShiftPolygonFile(i, True), GBX0.MaxTorque(i)))
			End If

		Next

		ChSkipGears.Checked = GBX0.SkipGears
		TbTqResv.Text = GBX0.TorqueResv.ToString
		TbShiftTime.Text = GBX0.ShiftTime.ToString
		TbTqResvStart.Text = GBX0.TorqueResvStart.ToString
		TbStartSpeed.Text = GBX0.StartSpeed.ToString
		TbStartAcc.Text = GBX0.StartAcc.ToString
		ChShiftInside.Checked = GBX0.ShiftInside

		TbTCfile.Text = GBX0.TorqueConverterFile(True)
		TbTCrefrpm.Text = GBX0.TorqueConverterReferenceRpm
		TbTCinertia.Text = GBX0.TorqueConverterInertia
		TBTCShiftPolygon.Text = GBX0.TorqueConverterShiftPolygonFile

		tbUpshiftMinAcceleration.Text = GBX0.UpshiftMinAcceleration
		tbDownshiftAfterUpshift.Text = GBX0.DownshiftAfterUpshift
		tbUpshiftAfterDownshift.Text = GBX0.UpshiftAfterDownshift

		CbGStype.SelectedValue = GBX0.Type
		'If CType(GBX0.gs_Type, Integer) <= Me.CbGStype.Items.Count - 1 Then
		'	Me.CbGStype.SelectedIndex = CType(GBX0.gs_Type, Integer)
		'Else
		'	Me.CbGStype.SelectedIndex = 0
		'End If

		DeclInit()


		GearboxFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""
		GbxFile = file
		Activate()

		Changed = False
		UpdatePic()
	End Sub

	Private Function CreateListviewItem(gear As String, tc As String, ratio As Single, getrMap As String,
										shiftPolygon As String, fldFile As String) As ListViewItem
		Dim retVal As ListViewItem = New ListViewItem(gear)
		'retVal.SubItems.Add(tc)
		retVal.SubItems.Add(ratio)
		retVal.SubItems.Add(getrMap)
		retVal.SubItems.Add(shiftPolygon)
		retVal.SubItems.Add(fldFile)
		Return retVal
	End Function

	'Save or Save As function = true if file is saved
	Private Function SaveOrSaveAs(ByVal SaveAs As Boolean) As Boolean
		If GbxFile = "" Or SaveAs Then
			If GearboxFileBrowser.SaveDialog(GbxFile) Then
				GbxFile = GearboxFileBrowser.Files(0)
			Else
				Return False
			End If
		End If
		Return saveGBX(GbxFile)
	End Function

	'Save file
	Private Function saveGBX(ByVal file As String) As Boolean
		Dim GBX0 As Gearbox
		Dim i As Int16

		GBX0 = New Gearbox
		GBX0.FilePath = file

		GBX0.ModelName = TbName.Text
		If Trim(GBX0.ModelName) = "" Then GBX0.ModelName = "Undefined"

		GBX0.TracIntrSi = fTextboxToNumString(TbTracInt.Text)
		GBX0.GbxInertia = fTextboxToNumString(TBI_getr.Text)

		For i = 0 To LvGears.Items.Count - 1
			'GBX0.IsTCgear.Add(Me.LvGears.Items(i).SubItems(GearboxTbl.TorqueConverter).Text = "on" And i > 0)
			GBX0.GearRatios.Add(CSng(LvGears.Items(i).SubItems(GearboxTbl.Ratio).Text))
			GBX0.GearLossmaps.Add(New SubPath)
			GBX0.GearLossMap(i) = LvGears.Items(i).SubItems(GearboxTbl.LossMapEfficiency).Text
			GBX0.GearshiftFiles.Add(New SubPath)
			GBX0.ShiftPolygonFile(i) = LvGears.Items(i).SubItems(GearboxTbl.ShiftPolygons).Text
			'GBX0.FldFiles.Add(New cSubPath)
			'GBX0.FldFile(i) = Me.LvGears.Items(i).SubItems(GearboxTbl.MaxTorque).Text
			GBX0.MaxTorque.Add(LvGears.Items(i).SubItems(GearboxTbl.MaxTorque).Text)
		Next

		GBX0.TorqueResv = fTextboxToNumString(TbTqResv.Text)
		GBX0.SkipGears = ChSkipGears.Checked
		GBX0.ShiftTime = fTextboxToNumString(TbShiftTime.Text)
		GBX0.TorqueResvStart = fTextboxToNumString(TbTqResvStart.Text)
		GBX0.StartSpeed = fTextboxToNumString(TbStartSpeed.Text)
		GBX0.StartAcc = fTextboxToNumString(TbStartAcc.Text)
		GBX0.ShiftInside = ChShiftInside.Checked

		GBX0.Type = CbGStype.SelectedValue

		GBX0.TorqueConverterEnabled = GBX0.Type.AutomaticTransmission()
		GBX0.TorqueConverterFile = TbTCfile.Text
		GBX0.TorqueConverterReferenceRpm = fTextboxToNumString(TbTCrefrpm.Text)
		GBX0.TorqueConverterInertia = fTextboxToNumString(TbTCinertia.Text)
		GBX0.TorqueConverterShiftPolygonFile = TBTCShiftPolygon.Text

		GBX0.DownshiftAfterUpshift = fTextboxToNumString(tbDownshiftAfterUpshift.Text)
		GBX0.UpshiftAfterDownshift = fTextboxToNumString(tbUpshiftAfterDownshift.Text)
		GBX0.UpshiftMinAcceleration = fTextboxToNumString(tbUpshiftMinAcceleration.Text)

		If Not GBX0.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		If AutoSendTo Then
			If VectoJobForm.Visible Then
				If UCase(fFileRepl(VectoJobForm.TbGBX.Text, JobDir)) <> UCase(file) Then _
					VectoJobForm.TbGBX.Text = GetFilenameWithoutDirectory(file, JobDir)
				VectoJobForm.UpdatePic()
			End If
		End If

		GearboxFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""

		Changed = False

		Return True
	End Function

#Region "Change Events"

	'Change Status ändern |@@| Change Status change
	Private Sub Change()
		If Not Changed Then
			LbStatus.Text = "Unsaved changes in current file"
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

	Private Sub TbName_TextChanged(sender As Object, e As EventArgs) Handles TbName.TextChanged
		Change()
	End Sub

	Private Sub TBI_getr_TextChanged(sender As Object, e As EventArgs) Handles TBI_getr.TextChanged
		Change()
	End Sub

	Private Sub TbTracInt_TextChanged(sender As Object, e As EventArgs) Handles TbTracInt.TextChanged
		Change()
	End Sub

	Private Sub ChSkipGears_CheckedChanged(sender As Object, e As EventArgs) _
		Handles ChSkipGears.CheckedChanged
		CheckEnableTorqRes()
		Change()
	End Sub

	Private Sub ChShiftInside_CheckedChanged(sender As Object, e As EventArgs) _
		Handles ChShiftInside.CheckedChanged
		CheckEnableTorqRes()
		Change()
	End Sub

	Private Sub TbTqResv_TextChanged(sender As Object, e As EventArgs) Handles TbTqResv.TextChanged
		Change()
	End Sub

	Private Sub TbShiftTime_TextChanged(sender As Object, e As EventArgs) Handles TbShiftTime.TextChanged
		Change()
	End Sub

	Private Sub TbTqResvStart_TextChanged(sender As Object, e As EventArgs) Handles TbTqResvStart.TextChanged
		Change()
	End Sub

	Private Sub TbStartSpeed_TextChanged(sender As Object, e As EventArgs) Handles TbStartSpeed.TextChanged
		Change()
	End Sub

	Private Sub TbStartAcc_TextChanged(sender As Object, e As EventArgs) Handles TbStartAcc.TextChanged
		Change()
	End Sub

	Private Sub TbTCfile_TextChanged(sender As Object, e As EventArgs) Handles TbTCfile.TextChanged
		Change()
	End Sub

	Private Sub TbTCrefrpm_TextChanged(sender As Object, e As EventArgs) Handles TbTCrefrpm.TextChanged
		Change()
	End Sub

	Private Sub TbTCinertia_TextChanged(sender As Object, e As EventArgs) Handles TbTCinertia.TextChanged
		Change()
	End Sub


	Private Sub CheckEnableTorqRes()
		If ChShiftInside.Checked Or ChSkipGears.Checked Then
			PnTorqRes.Enabled = True
		Else
			PnTorqRes.Enabled = False
		End If
	End Sub


#End Region

	'Save and close
	Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Close()
	End Sub

	'Cancel
	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub

	'Enable/Disable settings for specific transmission types
	Private Sub CbGStype_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles CbGStype.SelectedIndexChanged
		Dim GStype As GearboxType = CbGStype.SelectedItem.Value

		Change()

		ChShiftInside.Enabled = (GStype.EarlyShiftGears())
		ChSkipGears.Enabled = (GStype.SkipGears())
		'ChTCon.Enabled = (GStype.AutomaticTransmission())
		PnTC.Enabled = GStype.AutomaticTransmission()
	End Sub


	Private Sub LvGears_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles LvGears.SelectedIndexChanged
		UpdatePic()
	End Sub

#Region "Gears"

	'Gear-DoubleClick
	Private Sub LvGears_MouseDoubleClick(ByVal sender As Object, ByVal e As MouseEventArgs) _
		Handles LvGears.MouseDoubleClick
		EditGear()
	End Sub

	'Gear-KeyDown
	Private Sub LvGears_KeyDown(sender As Object, e As KeyEventArgs) Handles LvGears.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				RemoveGear(False)
			Case Keys.Enter
				EditGear()
		End Select
	End Sub

	'Remove Gear Button
	Private Sub BtClearGear_Click(sender As Object, e As EventArgs) Handles BtRemGear.Click
		RemoveGear(False)
	End Sub

	'Add Gear button
	Private Sub BtAddGear_Click(sender As Object, e As EventArgs) Handles BtAddGear.Click
		AddGear()
		LvGears.Items(LvGears.Items.Count - 1).Selected = True
		EditGear()
	End Sub

	'Edit Gear
	Private Sub EditGear()

		Do

			'GearDia.ChIsTCgear.Enabled = (Me.ChTCon.Checked And Me.LvGears.SelectedIndices(0) > 0)
			GearDia.PnShiftPoly.Enabled = (Not Cfg.DeclMode And LvGears.SelectedIndices(0) > 0)
			GearDia.PnFld.Enabled = (LvGears.SelectedIndices(0) > 0)
			GearDia.GbxPath = GetPath(GbxFile)
			GearDia.TbGear.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.GearNr).Text
			GearDia.TbRatio.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text
			GearDia.TbMapPath.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.LossMapEfficiency).Text
			If LvGears.SelectedIndices(0) > 0 Then
				GearDia.TbShiftPolyFile.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text
				GearDia.TbMaxTorque.Text = LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxTorque).Text
			Else
				GearDia.TbShiftPolyFile.Text = ""
				GearDia.TbMaxTorque.Text = ""
			End If

			If LvGears.SelectedItems(0).Index = 0 Then
				GearDia.BtPrevious.Enabled = False
			Else
				GearDia.BtPrevious.Enabled = True
			End If

			If GearDia.ShowDialog = DialogResult.OK Then

				'Me.LvGears.SelectedItems(0).SubItems(GearboxTbl.TorqueConverter).Text = "-"


				LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text = GearDia.TbRatio.Text
				LvGears.SelectedItems(0).SubItems(GearboxTbl.LossMapEfficiency).Text = GearDia.TbMapPath.Text
				LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text = GearDia.TbShiftPolyFile.Text
				LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxTorque).Text = GearDia.TbMaxTorque.Text

				UpdatePic()
				Change()

			Else

				If LvGears.SelectedItems(0).SubItems(GearboxTbl.Ratio).Text = "" Then RemoveGear(True)

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

		lvi = CreateListviewItem(LvGears.Items.Count.ToString("00"), "-", 1, "", "", "")

		LvGears.Items.Add(lvi)

		lvi.EnsureVisible()

		LvGears.Focus()

		'Change() => NO! Change() is already handled by EditGear
	End Sub

	'Remove Gear
	Private Sub RemoveGear(ByVal NoChange As Boolean)
		Dim i0 As Int16
		Dim i As Int16
		Dim lv0 As ListViewItem

		If LvGears.Items.Count < 2 Then Exit Sub

		If LvGears.SelectedItems.Count = 0 Then LvGears.Items(LvGears.Items.Count - 1).Selected = True

		i0 = LvGears.SelectedItems(0).Index

		If i0 = 0 Then Exit Sub 'Must not remove axle

		LvGears.SelectedItems(0).Remove()

		i = 0
		For Each lv0 In LvGears.Items
			If lv0.SubItems(GearboxTbl.GearNr).Text = "Axle" Then Continue For
			i += 1
			lv0.SubItems(GearboxTbl.GearNr).Text = i.ToString("00")
		Next

		If i0 < LvGears.Items.Count Then
			LvGears.Items(i0).Selected = True
			LvGears.Items(i0).EnsureVisible()
		End If

		LvGears.Focus()
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

		CmOpenFile.Show(Windows.Forms.Cursor.Position)
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


	Private Sub UpdatePic()

		Dim f As CsvFile
		Dim path As String
		Dim lM As List(Of Single) = Nothing
		Dim lup As List(Of Single) = Nothing
		Dim ldown As List(Of Single) = Nothing
		Dim line As String()
		Dim MyChart As Chart
		Dim s As Series
		Dim a As ChartArea
		Dim img As Image
		Dim Gear As Integer
		Dim fldOK As Boolean
		Dim fldpath As String
		Dim FLD0 As EngineFullLoadCurve = Nothing
		Dim ShiftOK As Boolean


		PicBox.Image = Nothing

		Try

			'Check Files
			If LvGears.Items.Count > 1 Then

				If LvGears.SelectedItems.Count > 0 AndAlso LvGears.SelectedIndices(0) > 0 Then
					path = fFileRepl(LvGears.SelectedItems(0).SubItems(GearboxTbl.ShiftPolygons).Text, GetPath(GbxFile))
					'fldpath = fFileRepl(LvGears.SelectedItems(0).SubItems(GearboxTbl.MaxTorque).Text, fPATH(GbxFile))
					Gear = LvGears.SelectedIndices(0)
				Else
					path = fFileRepl(LvGears.Items(1).SubItems(GearboxTbl.ShiftPolygons).Text, GetPath(GbxFile))
					'fldpath = fFileRepl(Me.LvGears.Items(1).SubItems(GearboxTbl.MaxTorque).Text, fPATH(GbxFile))
					Gear = 1
				End If

				f = New CsvFile
				ShiftOK = f.OpenRead(path)

				fldpath = VectoJobForm.FLDfile

				fldOK = Not IsNothing(fldpath) AndAlso fldpath.Trim <> ""

				If fldOK Then
					FLD0 = New EngineFullLoadCurve
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

		MyChart = New Chart
		MyChart.Width = PicBox.Width
		MyChart.Height = PicBox.Height

		a = New ChartArea

		'Shiftpolygons from file
		If ShiftOK Then
			s = New Series
			s.Points.DataBindXY(lup, lM)
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkRed
			s.Name = "Upshift curve"
			MyChart.Series.Add(s)

			s = New Series
			s.Points.DataBindXY(ldown, lM)
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkRed
			s.Name = "Downshift curve"
			MyChart.Series.Add(s)
		End If

		Dim vectoJob As VectoJob = New VectoJob() With {.FilePath = VectoJobForm.VECTOfile}
		Dim vectoOk As Boolean = vectoJob.ReadFile()
		Dim vehicle As Vehicle = New Vehicle() With {.FilePath = vectoJob.PathVeh(False)}
		Dim vehicleOk As Boolean = vehicle.ReadFile(False)

		'Fld
		If fldOK AndAlso vectoOk AndAlso vehicleOk Then

			s = New Series
			s.Points.DataBindXY(FLD0.EngineSpeedList, FLD0.MaxTorqueList)
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkBlue
			s.Name = "Full load"
			MyChart.Series.Add(s)

			If VectoJobForm.Visible AndAlso VectoJobForm.n_idle > 0 Then
				'If FLD0.Init(VectoJobForm.n_idle) Then

				'Dim fullLoadCurve As FullLoadCurve = ConvertToFullLoadCurve(FLD0.LnU, FLD0.LTq)
				Dim gears As IList(Of ITransmissionInputData) = ConvertToGears(LvGears.Items)
				Dim shiftLines As ShiftPolygon = GetShiftLines(FLD0, vehicle, gears, Gear)
				If (CType(CbGStype.SelectedValue, GearboxType).ManualTransmission() AndAlso Not IsNothing(shiftLines)) Then


					s = New Series

					's.Points.DataBindXY(Shiftpoly.gs_nUup, Shiftpoly.gs_TqUp)
					s.Points.DataBindXY(
						shiftLines.Upshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
						shiftLines.Upshift.Select(Function(pt) pt.Torque.Value()).ToList())
					s.ChartType = SeriesChartType.FastLine
					s.BorderWidth = 2
					s.Color = Color.DarkRed
					s.BorderDashStyle = ChartDashStyle.Dash
					s.Name = "Upshift curve (generic)"
					MyChart.Series.Add(s)

					s = New Series
					's.Points.DataBindXY(Shiftpoly.gs_nUdown, Shiftpoly.gs_TqDown)
					s.Points.DataBindXY(
						shiftLines.Downshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
						shiftLines.Downshift.Select(Function(pt) pt.Torque.Value()).ToList())
					s.ChartType = SeriesChartType.FastLine
					s.BorderWidth = 2
					s.Color = Color.DarkRed
					s.BorderDashStyle = ChartDashStyle.Dash
					s.Name = "Downshift curve (generic)"
					MyChart.Series.Add(s)
				End If
				'End If
			End If

		End If


		a.Name = "main"

		a.AxisX.Title = "engine speed [1/min]"
		a.AxisX.TitleFont = New Font("Helvetica", 10)
		a.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
		a.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
		a.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

		a.AxisY.Title = "engine torque [Nm]"
		a.AxisY.TitleFont = New Font("Helvetica", 10)
		a.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
		a.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
		a.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

		a.AxisX.Minimum = 300
		a.BorderDashStyle = ChartDashStyle.Solid
		a.BorderWidth = 1

		a.BackColor = Color.GhostWhite

		MyChart.ChartAreas.Add(a)

		MyChart.Titles.Add("Gear " & Gear & " shift polygons")
		MyChart.Titles(0).Font = New Font("Helvetica", 12)

		MyChart.Update()

		img = New Bitmap(MyChart.Width, MyChart.Height, PixelFormat.Format32bppArgb)
		MyChart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))

		PicBox.Image = img
	End Sub


	Private Function GetShiftLines(engineFullLoadCurve As EngineFullLoadCurve, vehicle As Vehicle,
									gears As IList(Of ITransmissionInputData), gear As UInteger) As ShiftPolygon
		Dim engine As CombustionEngineData = ConvertToEngineData(engineFullLoadCurve, VectoJobForm.n_idle)
		If gears.Count <= 1 Then
			Return Nothing
		End If
		Dim rDyn As Meter = (vehicle.DynamicTyreRadius / 1000.0).SI(Of Meter)()
		If rDyn.IsEqual(0) Then
			If (vehicle.Axles.Count < 2) Then
				Return Nothing
			End If
			rDyn = DeclarationData.Wheels.Lookup(vehicle.Axles(1).Wheels).DynamicTyreRadius
		End If
		If (rDyn.IsEqual(0)) Then
			Return Nothing
		End If
		Dim shiftLines As ShiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(gear - 1, engine.FullLoadCurve, gears,
																					engine,
																					Double.Parse(LvGears.Items(0).SubItems(GearboxTbl.Ratio).Text, CultureInfo.InvariantCulture),
																					(rDyn))
		Return shiftLines
	End Function

	Private Function ConvertToGears(gbx As ListView.ListViewItemCollection) As IList(Of ITransmissionInputData)
		Dim retVal As List(Of ITransmissionInputData) = New List(Of ITransmissionInputData)
		Dim value As Double

		For i As Integer = 1 To gbx.Count - 1
			If _
				gbx(i).SubItems(GearboxTbl.Ratio).Text <> "" AndAlso Double.TryParse(gbx(i).SubItems(GearboxTbl.Ratio).Text, value) _
				Then
				retVal.Add(
					New TransmissionInputData() _
							With {.Ratio = Double.Parse(value, CultureInfo.InvariantCulture)})

			End If
		Next
		Return retVal
	End Function


#Region "Torque Converter"


	'Browse TC file
	Private Sub BtTCfileBrowse_Click(sender As Object, e As EventArgs) Handles BtTCfileBrowse.Click
		If TorqueConverterFileBrowser.OpenDialog(fFileRepl(TbTCfile.Text, GetPath(GbxFile))) Then
			TbTCfile.Text = GetFilenameWithoutDirectory(TorqueConverterFileBrowser.Files(0), GetPath(GbxFile))
		End If
	End Sub

	'Open TC file
	Private Sub BtTCfileOpen_Click(sender As Object, e As EventArgs) Handles BtTCfileOpen.Click
		OpenFiles(fFileRepl(TbTCfile.Text, GetPath(GbxFile)))
	End Sub


#End Region

	Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter
	End Sub

	Public Sub New()

		' Dieser Aufruf ist für den Designer erforderlich.
		InitializeComponent()

		' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
	End Sub

	Private Sub BtTCShiftFileBrowse_Click(sender As Object, e As EventArgs) Handles BtTCShiftFileBrowse.Click
		If fbTCCShift.OpenDialog(fFileRepl(TBTCShiftPolygon.Text, GetPath(GbxFile))) Then
			TBTCShiftPolygon.Text = GetFilenameWithoutDirectory(fbTCCShift.Files(0), GetPath(GbxFile))
		End If
	End Sub
End Class
