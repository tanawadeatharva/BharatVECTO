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

Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.Globalization
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Configuration
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox
Imports TUGraz.VectoCore.Models.SimulationComponent.Impl

''' <summary>
''' Job Editor. Create/Edit VECTO job files (.vecto)
''' </summary>
''' <remarks></remarks>
Public Class F_VECTO
	Public VECTOfile As String
	Private Changed As Boolean = False

	Private pgDriver As TabPage

	Private pgDriverON As Boolean = True

	Private AuxDlog As F_VEH_AuxDlog

	Public n_idle As Single
	Public FLDfile As String

	'AA-TB
	'Populate Advanced Auxiliaries
	Private Sub PopulateAdvancedAuxiliaries()
		'Scan the program directory for DLL's which are AdvancedAuxiliaries and display
		Dim aList As List(Of cAdvancedAuxiliary) = DiscoverAdvancedAuxiliaries()

		cboAdvancedAuxiliaries.DataSource = aList
		cboAdvancedAuxiliaries.DisplayMember = "AuxiliaryName"
	End Sub


	'Initialise form
	Private Sub F02_GEN_Load(sender As Object, e As EventArgs) Handles Me.Load
		Dim x As Int16

		n_idle = -1
		FLDfile = ""

		AuxDlog = New F_VEH_AuxDlog

		pgDriver = TabPgDriver

		For x = 0 To TabControl1.TabCount - 1
			TabControl1.TabPages(x).Show()
		Next

		LvAux.Columns(2).Width = -2

		'Declaration Mode
		If Cfg.DeclMode Then
			LvAux.Columns(2).Text = "Technology"
		Else
			LvAux.Columns(2).Text = "Input File"
		End If

		CbEngOnly.Enabled = Not Cfg.DeclMode
		GrCycles.Enabled = Not Cfg.DeclMode
		GrVACC.Enabled = Not Cfg.DeclMode
		PnStartStop.Enabled = Not Cfg.DeclMode
		RdOff.Enabled = Not Cfg.DeclMode
		GrLAC.Enabled = Not Cfg.DeclMode
		ButAuxAdd.Enabled = Not Cfg.DeclMode
		ButAuxRem.Enabled = Not Cfg.DeclMode
		PnEcoRoll.Enabled = Not Cfg.DeclMode

		Changed = False
		'AA-TB
		PopulateAdvancedAuxiliaries()

		'Attempt to select that found in Config
	End Sub

	'Close - Check for unsaved changes
	Private Sub F02_GEN_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Set generic values for Declaration mode
	Private Sub DeclInit()

		If Not Cfg.DeclMode Then Exit Sub

		LvCycles.Items.Clear()
		CbEngOnly.Checked = False
		TbDesMaxFile.Text = ""
		If Not RdEcoRoll.Checked Then RdOverspeed.Checked = True
		CbLookAhead.Checked = True

		TbSSspeed.Text = DeclarationData.Driver.StartStop.MaxSpeed.AsKmph()	'cDeclaration.SSspeed
		TbSStime.Text = DeclarationData.Driver.StartStop.MinTime.Value()   'cDeclaration.SStime
		TbSSdelay.Text = DeclarationData.Driver.StartStop.Delay.Value()	 ' cDeclaration.SSdelay

		tbLacPreviewFactor.Text = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor
		tbLacDfTargetSpeedFile.Text = ""
		tbLacDfVelocityDropFile.Text = ""

		TbOverspeed.Text = DeclarationData.Driver.OverSpeedEcoRoll.OverSpeed.AsKmph()  'cDeclaration.Overspeed
		TbUnderSpeed.Text = DeclarationData.Driver.OverSpeedEcoRoll.UnderSpeed.AsKmph()	' cDeclaration.Underspeed
		TbVmin.Text = DeclarationData.Driver.OverSpeedEcoRoll.MinSpeed.AsKmph()	 'cDeclaration.ECvmin

		If _
			LvAux.Items.Count <> 5 OrElse
			(LvAux.Items(0).Text <> sKey.AUX.Fan OrElse LvAux.Items(1).Text <> sKey.AUX.SteerPump OrElse
			LvAux.Items(2).Text <> sKey.AUX.HVAC OrElse LvAux.Items(3).Text <> sKey.AUX.ElecSys OrElse
			LvAux.Items(4).Text <> sKey.AUX.PneumSys) Then
			LvAux.Items.Clear()


			LvAux.Items.Add(GetTechListForAux(sKey.AUX.Fan, "Fan", DeclarationData.Fan))

			LvAux.Items.Add(GetTechListForAux(sKey.AUX.SteerPump, "Steering pump", DeclarationData.SteeringPump))

			LvAux.Items.Add(GetTechListForAux(sKey.AUX.HVAC, "HVAC", DeclarationData.HeatingVentilationAirConditioning))

			LvAux.Items.Add(GetTechListForAux(sKey.AUX.ElecSys, "Electric System", DeclarationData.ElectricSystem))

			LvAux.Items.Add(GetTechListForAux(sKey.AUX.PneumSys, "Pneymatic System", DeclarationData.PneumaticSystem))

		End If
	End Sub

	Protected Function GetTechListForAux(key As String, name As String, aux As IDeclarationAuxiliaryTable) As ListViewItem
		Dim LV0 As ListViewItem

		LV0 = New ListViewItem(key)
		LV0.SubItems.Add(name)
		Dim auxtech As String() = aux.GetTechnologies()
		If auxtech.Count > 1 Then
			LV0.SubItems.Add("")
		Else
			LV0.SubItems.Add(auxtech(0))
		End If
		Return LV0
	End Function


	'Show/Hide "Driver Assist" Tab
	Private Sub SetDrivertab(onOff As Boolean)
		If onOff Then
			If Not pgDriverON Then
				pgDriverON = True
				TabControl1.TabPages.Insert(1, pgDriver)
			End If
		Else
			If pgDriverON Then
				pgDriverON = False
				TabControl1.Controls.Remove(pgDriver)
			End If
		End If
	End Sub


#Region "Browse Buttons"

	Private Sub ButtonVEH_Click(sender As Object, e As EventArgs) Handles ButtonVEH.Click
		If fbVEH.OpenDialog(fFileRepl(TbVEH.Text, fPATH(VECTOfile))) Then
			TbVEH.Text = fFileWoDir(fbVEH.Files(0), fPATH(VECTOfile))
		End If
	End Sub

	Private Sub ButtonMAP_Click(sender As Object, e As EventArgs) Handles ButtonMAP.Click
		If fbENG.OpenDialog(fFileRepl(TbENG.Text, fPATH(VECTOfile))) Then
			TbENG.Text = fFileWoDir(fbENG.Files(0), fPATH(VECTOfile))
		End If
	End Sub

	Private Sub ButtonGBX_Click(sender As Object, e As EventArgs) Handles ButtonGBX.Click
		If fbGBX.OpenDialog(fFileRepl(TbGBX.Text, fPATH(VECTOfile))) Then
			TbGBX.Text = fFileWoDir(fbGBX.Files(0), fPATH(VECTOfile))
		End If
	End Sub

	Private Sub BtDesMaxBr_Click_1(sender As Object, e As EventArgs) Handles BtDesMaxBr.Click
		If fbACC.OpenDialog(fFileRepl(TbDesMaxFile.Text, fPATH(VECTOfile))) Then
			TbDesMaxFile.Text = fFileWoDir(fbACC.Files(0), fPATH(VECTOfile))
		End If
	End Sub

	Private Sub BtAccOpen_Click(sender As Object, e As EventArgs) Handles BtAccOpen.Click
		OpenFiles(fFileRepl(TbDesMaxFile.Text, fPATH(VECTOfile)))
	End Sub

#End Region

#Region "Open Buttons"

	'Open Vehicle Editor
	Private Sub ButOpenVEH_Click(sender As Object, e As EventArgs) Handles ButOpenVEH.Click
		Dim f As String
		f = fFileRepl(TbVEH.Text, fPATH(VECTOfile))

		'Thus Veh-file is returned
		F_VEH.JobDir = fPATH(VECTOfile)
		F_VEH.AutoSendTo = True

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not F_VEH.Visible Then
			F_VEH.Show()
		Else
			If F_VEH.WindowState = FormWindowState.Minimized Then F_VEH.WindowState = FormWindowState.Normal
			F_VEH.BringToFront()
		End If

		If Not Trim(f) = "" Then F_VEH.OpenVehicle(f)
	End Sub

	'Open Engine Editor
	Private Sub ButOpenENG_Click(sender As Object, e As EventArgs) Handles ButOpenENG.Click
		Dim f As String
		f = fFileRepl(TbENG.Text, fPATH(VECTOfile))

		'Thus Veh-file is returned
		F_ENG.JobDir = fPATH(VECTOfile)
		F_ENG.AutoSendTo = True

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not F_ENG.Visible Then
			F_ENG.Show()
		Else
			If F_ENG.WindowState = FormWindowState.Minimized Then F_ENG.WindowState = FormWindowState.Normal
			F_ENG.BringToFront()
		End If

		If Not Trim(f) = "" Then F_ENG.openENG(f)
	End Sub

	'Open Gearbox Editor
	Private Sub ButOpenGBX_Click(sender As Object, e As EventArgs) Handles ButOpenGBX.Click
		Dim f As String
		f = fFileRepl(TbGBX.Text, fPATH(VECTOfile))

		'Thus Veh-file is returned
		F_GBX.JobDir = fPATH(VECTOfile)
		F_GBX.AutoSendTo = True

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not F_GBX.Visible Then
			F_GBX.Show()
		Else
			If F_GBX.WindowState = FormWindowState.Minimized Then F_GBX.WindowState = FormWindowState.Normal
			F_GBX.BringToFront()
		End If

		If Not Trim(f) = "" Then F_GBX.openGBX(f)
	End Sub

#End Region

#Region "Toolbar"

	'New
	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		VECTOnew()
	End Sub

	'Open
	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If fbVECTO.OpenDialog(VECTOfile, False, "vecto") Then VECTOload2Form(fbVECTO.Files(0))
	End Sub

	'Save
	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		Save()
	End Sub

	'Save As
	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		If fbVECTO.SaveDialog(VECTOfile) Then Call VECTOsave(fbVECTO.Files(0))
	End Sub

	'Send to Job file list in main form
	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click
		If ChangeCheckCancel() Then Exit Sub
		If VECTOfile = "" Then
			MsgBox("File not found!" & ChrW(10) & ChrW(10) & "Save file and try again.")
		Else
			F_MAINForm.AddToJobListView(VECTOfile)
		End If
	End Sub

	'Help
	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim BrowserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim DefaultBrowserPath As String =
					Regex.Match(BrowserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(DefaultBrowserPath,
						String.Format("""{0}{1}""", MyAppPath, "User Manual\help.html#job-editor"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub


#End Region

	'Save ("Save" or "Save As" when new file)
	Private Function Save() As Boolean
		If VECTOfile = "" Then
			If fbVECTO.SaveDialog("") Then
				VECTOfile = fbVECTO.Files(0)
			Else
				Return False
			End If
		End If
		Return VECTOsave(VECTOfile)
	End Function

	'Open file
	Public Sub VECTOload2Form(file As String)
		If ChangeCheckCancel() Then Exit Sub

		VECTOnew()

		'Read GEN
		Dim VEC0 = New cVECTO
		VEC0.FilePath = file
		Try
			If Not VEC0.ReadFile() Then
				MsgBox("Failed to load " & fFILE(file, True) & "!")
				Exit Sub
			End If
		Catch ex As Exception
			MsgBox("Failed to load " & fFILE(file, True) & "!")
			Exit Sub
		End Try

		If Cfg.DeclMode <> VEC0.SavedInDeclMode Then
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


		'Update Form

		'Files -----------------------------
		TbVEH.Text = VEC0.PathVeh(True)
		TbENG.Text = VEC0.PathEng(True)
		TbGBX.Text = VEC0.PathGbx(True)

		'Start/Stop
		ChBStartStop.Checked = VEC0.StartStop
		TbSSspeed.Text = VEC0.StStV.ToString()
		TbSStime.Text = VEC0.StStT.ToString()
		TbSSdelay.Text = VEC0.StStDelay.ToString()

		'VACC
		TbDesMaxFile.Text = VEC0.DesMaxFile(True)


		'AA-TB
		'Try and Select any previously selected Auxiliary Type
		For Each item As cAdvancedAuxiliary In cboAdvancedAuxiliaries.Items
			If item.AssemblyName = VEC0.AuxiliaryAssembly AndAlso VEC0.AuxiliaryVersion = item.AuxiliaryVersion Then
				cboAdvancedAuxiliaries.SelectedItem = item
				Exit For
			End If
		Next
		'AA-TB
		'Assign any previously saved Axiliary FilePath
		txtAdvancedAuxiliaryFile.Text = VEC0.AdvancedAuxiliaryFilePath


		LvAux.Items.Clear()
		For Each AuxEntryKV In VEC0.AuxPaths
			Dim lv0 = New ListViewItem
			lv0.SubItems(0).Text = AuxEntryKV.Key
			lv0.SubItems.Add(AuxEntryKV.Value.Type)
			If Cfg.DeclMode Then
				lv0.SubItems.Add(AuxEntryKV.Value.TechStr)
			Else
				lv0.SubItems.Add(AuxEntryKV.Value.Path.OriginalPath)
			End If
			LvAux.Items.Add(lv0)
		Next

		For Each sb In VEC0.CycleFiles
			Dim lv0 = New ListViewItem
			lv0.Text = sb.OriginalPath
			LvCycles.Items.Add(lv0)
		Next

		CbEngOnly.Checked = VEC0.EngOnly

		If VEC0.EcoRollOn Then
			RdEcoRoll.Checked = True
		ElseIf VEC0.OverSpeedOn Then
			RdOverspeed.Checked = True
		Else
			RdOff.Checked = True
		End If
		TbOverspeed.Text = CStr(VEC0.OverSpeed)
		TbUnderSpeed.Text = CStr(VEC0.UnderSpeed)
		TbVmin.Text = CStr(VEC0.VMin)
		CbLookAhead.Checked = VEC0.LookAheadOn
		'TbAlookahead.Text = CStr(VEC0.ALookahead)
		'TbVminLA.Text = CStr(VEC0.VMinLa)
		tbLacPreviewFactor.Text = CStr(VEC0.LacPreviewFactor)
		tbDfCoastingOffset.Text = CStr(VEC0.LacDfOffset)
		tbDfCoastingScale.Text = CStr(VEC0.LacDfScale)

		tbLacDfTargetSpeedFile.Text = VEC0.LacDfTargetSpeedFile
		tbLacDfVelocityDropFile.Text = VEC0.LacDfVelocityDropFile

		'-------------------------------------------------------------

		DeclInit()


		F_ENG.AutoSendTo = False
		F_GBX.AutoSendTo = False
		F_VEH.AutoSendTo = False


		VECTOfile = file

		Dim x As Short = Len(file)
		While Mid(file, x, 1) <> "\" And x > 0
			x = x - 1
		End While
		Text = Mid(file, x + 1, Len(file) - x)
		Changed = False
		ToolStripStatusLabelGEN.Text = ""	'file & " opened."

		UpdatePic()

		'-------------------------------------------------------------
	End Sub

	'Save file
	Private Function VECTOsave(file As String) As Boolean
		Dim message As String = String.Empty

		'AA-TB
		'Validation of Auxiliary Types/Advanced Auxiliaries
		'if not classic, check the file is valid, if not fail the operation and alert user.
		If cboAdvancedAuxiliaries.SelectedIndex > 0 Then

			'resolve absolute path for auxiliary file.
			Dim absoluteAAUxFile = ResolveAAUXFilePath(fPATH(VECTOfile), txtAdvancedAuxiliaryFile.Text)

			Dim aaAssemblyName = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary).AssemblyName
			Dim aaAssemblyVersion = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary).AuxiliaryVersion


			If Not ValidateAAUXFile(absoluteAAUxFile, aaAssemblyName, aaAssemblyVersion, message) Then
				MessageBox.Show(
					String.Format("You have selected an advanced auxiliary *Auxiliary Type*, but the file specified is invalid :{0}",
								message))
				Return False
			End If

		End If


		Dim vec0 = New cVECTO
		vec0.FilePath = file

		'Files ------------------------------------------------- -----------------

		vec0.PathVeh = TbVEH.Text
		vec0.PathEng = TbENG.Text

		For Each lv0 As ListViewItem In LvCycles.Items
			Dim sb = New cSubPath
			sb.Init(fPATH(file), lv0.Text)
			vec0.CycleFiles.Add(sb)
		Next

		vec0.PathGbx = TbGBX.Text


		'Start/Stop
		vec0.StartStop = ChBStartStop.Checked
		vec0.StStV = CSng(fTextboxToNumString(TbSSspeed.Text))
		vec0.StStT = CSng(fTextboxToNumString(TbSStime.Text))
		vec0.StStDelay = CInt(fTextboxToNumString(TbSSdelay.Text))

		'a_DesMax
		vec0.DesMaxFile = TbDesMaxFile.Text

		'AA-TB
		vec0.AuxiliaryAssembly = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary).AssemblyName
		vec0.AuxiliaryVersion = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary).AuxiliaryVersion
		vec0.AdvancedAuxiliaryFilePath = txtAdvancedAuxiliaryFile.Text

		For Each lv0 As ListViewItem In LvAux.Items
			Dim auxEntry = New cVECTO.AuxEntry

			If Cfg.DeclMode Then
				auxEntry.TechStr = lv0.SubItems(2).Text
			Else
				auxEntry.Path.Init(fPATH(file), lv0.SubItems(2).Text)
			End If

			auxEntry.Type = lv0.SubItems(1).Text
			vec0.AuxPaths.Add(lv0.SubItems(0).Text, auxEntry)
		Next

		vec0.EngOnly = CbEngOnly.Checked

		vec0.EcoRollOn = RdEcoRoll.Checked
		vec0.OverSpeedOn = RdOverspeed.Checked
		vec0.OverSpeed = CSng(fTextboxToNumString(TbOverspeed.Text))
		vec0.UnderSpeed = CSng(fTextboxToNumString(TbUnderSpeed.Text))
		vec0.VMin = CSng(fTextboxToNumString(TbVmin.Text))
		vec0.LookAheadOn = CbLookAhead.Checked
		'vec0.ALookahead = CSng(fTextboxToNumString(TbAlookahead.Text))
		'vec0.VMinLa = CSng(fTextboxToNumString(TbVminLA.Text))

		vec0.LacPreviewFactor = CSng(fTextboxToNumString(tbLacPreviewFactor.Text))
		vec0.LacDfOffset = CSng(fTextboxToNumString(tbDfCoastingOffset.Text))
		vec0.LacDfScale = CSng(fTextboxToNumString(tbDfCoastingScale.Text))
		vec0.LacDfTargetSpeedFile = tbLacDfTargetSpeedFile.Text
		vec0.LacDfVelocityDropFile = tbLacDfVelocityDropFile.Text
		'------------------------------------------------------------

		'SAVE
		If Not vec0.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		VECTOfile = file

		file = fFILE(VECTOfile, True)

		Text = file
		ToolStripStatusLabelGEN.Text = ""

		F_MAINForm.AddToJobListView(VECTOfile)

		Changed = False

		Return True
	End Function

	'New file
	Public Sub VECTOnew()

		If ChangeCheckCancel() Then Exit Sub

		n_idle = -1
		FLDfile = ""

		'Files
		TbVEH.Text = ""
		TbENG.Text = ""
		LvCycles.Items.Clear()
		TbGBX.Text = ""
		TbDesMaxFile.Text = ""

		'Start/Stop
		TbSSspeed.Text = "5"
		TbSStime.Text = "5"
		ChBStartStop.Checked = False

		LvAux.Items.Clear()

		CbEngOnly.Checked = False

		RdOff.Checked = True
		CbLookAhead.Checked = True
		'TbAlookahead.Text = "-0.5"
		TbOverspeed.Text = ""
		TbUnderSpeed.Text = ""
		TbVmin.Text = ""
		'TbVminLA.Text = "50"
		tbLacPreviewFactor.Text = "10"
		tbDfCoastingOffset.Text = "2.5"
		tbDfCoastingScale.Text = "1.5"
		tbLacDfTargetSpeedFile.Text = ""
		tbLacDfVelocityDropFile.Text = ""

		'---------------------------------------------------

		DeclInit()

		F_ENG.AutoSendTo = False

		VECTOfile = ""
		Text = "Job Editor"
		ToolStripStatusLabelGEN.Text = ""
		Changed = False
		UpdatePic()
	End Sub


#Region "Track changes"

#Region "'Change' Events"

	Private Sub TextBoxVEH_TextChanged(sender As Object, e As EventArgs) _
		Handles TbVEH.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TextBoxMAP_TextChanged(sender As Object, e As EventArgs) _
		Handles TbENG.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TextBoxFLD_TextChanged(sender As Object, e As EventArgs) _
		Handles TbGBX.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TbDesMaxFile_TextChanged_1(sender As Object, e As EventArgs) Handles TbDesMaxFile.TextChanged
		Change()
	End Sub


	Private Sub TBSSspeed_TextChanged(sender As Object, e As EventArgs) Handles TbSSspeed.TextChanged
		Change()
	End Sub

	Private Sub TBSStime_TextChanged(sender As Object, e As EventArgs) _
		Handles TbSStime.TextChanged, TbSSdelay.TextChanged
		Change()
	End Sub

	Private Sub TbOverspeed_TextChanged(sender As Object, e As EventArgs) Handles TbOverspeed.TextChanged
		Change()
	End Sub

	Private Sub TbUnderSpeed_TextChanged(sender As Object, e As EventArgs) Handles TbUnderSpeed.TextChanged
		Change()
	End Sub

	Private Sub TbVmin_TextChanged(sender As Object, e As EventArgs) _
		Handles TbVmin.TextChanged
		Change()
	End Sub

	Private Sub LvCycles_AfterLabelEdit(sender As Object, e As LabelEditEventArgs) _
		Handles LvCycles.AfterLabelEdit
		Change()
	End Sub


#End Region

	Private Sub Change()
		If Not Changed Then
			ToolStripStatusLabelGEN.Text = "Unsaved changes in current file"
			Changed = True
		End If
	End Sub

	' "Save changes? "... Returns True if User aborts
	Private Function ChangeCheckCancel() As Boolean

		If Changed Then

			Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
				Case MsgBoxResult.Yes
					Return Not Save()
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

#End Region

#Region "Aux Listview"

	Private Sub ButAuxAdd_Click(sender As Object, e As EventArgs) Handles ButAuxAdd.Click
		Dim ID As String

		AuxDlog.VehPath = fPATH(VECTOfile)
		AuxDlog.TbPath.Text = ""
		AuxDlog.CbType.SelectedIndex = -1
		AuxDlog.CbType.Text = ""
		AuxDlog.TbID.Text = "" '!!! Set Type before ID, because changing the type will overwrite the id !!!

lbDlog:
		If AuxDlog.ShowDialog = DialogResult.OK Then

			ID = UCase(Trim(AuxDlog.TbID.Text))

			Dim lv0 As ListViewItem
			For Each lv0 In LvAux.Items
				If lv0.SubItems(0).Text = ID Then
					MsgBox("ID '" & ID & "' already defined!", MsgBoxStyle.Critical)
					AuxDlog.TbID.SelectAll()
					AuxDlog.TbID.Focus()
					GoTo lbDlog
				End If
			Next

			lv0 = New ListViewItem
			lv0.SubItems(0).Text = UCase(Trim(AuxDlog.TbID.Text))
			lv0.SubItems.Add(Trim(AuxDlog.CbType.Text))
			lv0.SubItems.Add(Trim(AuxDlog.TbPath.Text))
			LvAux.Items.Add(lv0)
			Change()
		End If
	End Sub

	Private Sub ButAuxRem_Click(sender As Object, e As EventArgs) Handles ButAuxRem.Click
		RemoveAuxItem()
	End Sub

	Private Sub LvAux_DoubleClick(sender As Object, e As EventArgs) Handles LvAux.DoubleClick
		EditAuxItem()
	End Sub

	Private Sub LvAux_KeyDown(sender As Object, e As KeyEventArgs) Handles LvAux.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				If Not Cfg.DeclMode Then RemoveAuxItem()
			Case Keys.Enter
				EditAuxItem()
		End Select
	End Sub

	Private Sub EditAuxItem()
		If LvAux.SelectedItems.Count = 0 Then
			Exit Sub
		End If

		Dim selItem = LvAux.SelectedItems(0)

		AuxDlog.VehPath = fPATH(VECTOfile)
		AuxDlog.CbType.SelectedIndex = -1
		AuxDlog.CbType.Text = selItem.SubItems(1).Text
		AuxDlog.TbID.Text = selItem.SubItems(0).Text	'After Type-set!

		If Cfg.DeclMode Then
			AuxDlog.CbTech.Text = selItem.SubItems(2).Text
			AuxDlog.TbPath.Text = ""
		Else
			AuxDlog.CbTech.SelectedIndex = -1
			AuxDlog.TbPath.Text = selItem.SubItems(2).Text
		End If

		If AuxDlog.ShowDialog = DialogResult.OK Then
			selItem.SubItems(0).Text = UCase(Trim(AuxDlog.TbID.Text))
			selItem.SubItems(1).Text = Trim(AuxDlog.CbType.Text)

			If Cfg.DeclMode Then
				selItem.SubItems(2).Text = Trim(AuxDlog.CbTech.Text)
			Else
				selItem.SubItems(2).Text = Trim(AuxDlog.TbPath.Text)
			End If

			Change()
		End If
	End Sub

	Private Sub RemoveAuxItem()
		Dim i As Integer

		If LvAux.SelectedItems.Count = 0 Then
			If LvAux.Items.Count = 0 Then
				Exit Sub
			Else
				LvAux.Items(LvAux.Items.Count - 1).Selected = True
			End If
		End If

		i = LvAux.SelectedItems(0).Index

		LvAux.SelectedItems(0).Remove()

		If LvAux.Items.Count > 0 Then
			If i < LvAux.Items.Count Then
				LvAux.Items(i).Selected = True
			Else
				LvAux.Items(LvAux.Items.Count - 1).Selected = True
			End If
			LvAux.Focus()
		End If

		Change()
	End Sub

#End Region

	'OK (Save & Close)
	Private Sub ButSave_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If Not Save() Then Exit Sub
		Close()
	End Sub

	'Cancel
	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub

#Region "Cycle list"

	Private Sub LvCycles_DoubleClick(sender As Object, e As EventArgs) Handles LvCycles.DoubleClick
		If LvCycles.SelectedItems.Count > 0 Then _
			OpenFiles(fFileRepl(LvCycles.SelectedItems(0).SubItems(0).Text, fPATH(VECTOfile)))
	End Sub

	Private Sub LvCycles_KeyDown(sender As Object, e As KeyEventArgs) Handles LvCycles.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				RemoveCycle()
			Case Keys.Enter
				If LvCycles.SelectedItems.Count > 0 Then LvCycles.SelectedItems(0).BeginEdit()
		End Select
	End Sub

	Private Sub BtDRIadd_Click(sender As Object, e As EventArgs) Handles BtDRIadd.Click
		Dim genDir As String = fPATH(VECTOfile)

		If fbDRI.OpenDialog("", True) Then
			For Each s In fbDRI.Files
				LvCycles.Items.Add(fFileWoDir(s, genDir))
			Next
			Change()
		End If
	End Sub

	Private Sub BtDRIrem_Click(sender As Object, e As EventArgs) Handles BtDRIrem.Click
		RemoveCycle()
	End Sub

	Private Sub RemoveCycle()
		Dim i As Integer

		If LvCycles.SelectedItems.Count = 0 Then
			If LvCycles.Items.Count = 0 Then
				Exit Sub
			Else
				LvCycles.Items(LvCycles.Items.Count - 1).Selected = True
			End If
		End If

		i = LvCycles.SelectedItems(0).Index

		LvCycles.SelectedItems(0).Remove()

		If LvCycles.Items.Count > 0 Then
			If i < LvCycles.Items.Count Then
				LvCycles.Items(i).Selected = True
			Else
				LvCycles.Items(LvCycles.Items.Count - 1).Selected = True
			End If

			LvCycles.Focus()
		End If

		Change()
	End Sub

#End Region

#Region "Enable/Disable GUI controls"

	'Engine only mode changed
	Private Sub CbEngOnly_CheckedChanged(sender As Object, e As EventArgs) Handles CbEngOnly.CheckedChanged
		CheckEngOnly()
		Change()
	End Sub

	Private Sub CheckEngOnly()
		Dim OnOff As Boolean

		OnOff = Not CbEngOnly.Checked

		SetDrivertab(OnOff)

		ButOpenVEH.Enabled = OnOff
		TbVEH.Enabled = OnOff
		ButtonVEH.Enabled = OnOff
		ButOpenGBX.Enabled = OnOff
		TbGBX.Enabled = OnOff
		ButtonGBX.Enabled = OnOff
		GrAux.Enabled = OnOff
	End Sub

	'Start/Stop changed 
	Private Sub ChBStartStop_CheckedChanged_1(sender As Object, e As EventArgs) _
		Handles ChBStartStop.CheckedChanged
		Change()
		If Not Cfg.DeclMode Then PnStartStop.Enabled = ChBStartStop.Checked
	End Sub

	'LAC changed
	Private Sub CbLookAhead_CheckedChanged(sender As Object, e As EventArgs) _
		Handles CbLookAhead.CheckedChanged
		Change()
	End Sub

	'EcoRoll / Overspeed changed
	Private Sub RdOff_CheckedChanged(sender As Object, e As EventArgs) _
		Handles RdOff.CheckedChanged, RdOverspeed.CheckedChanged, RdEcoRoll.CheckedChanged
		Dim EcoR As Boolean
		Dim Ovr As Boolean

		Change()

		EcoR = RdEcoRoll.Checked
		Ovr = RdOverspeed.Checked

		TbOverspeed.Enabled = Ovr Or EcoR
		Label13.Enabled = Ovr Or EcoR
		Label14.Enabled = Ovr Or EcoR

		TbUnderSpeed.Enabled = EcoR
		Label22.Enabled = EcoR
		Label20.Enabled = EcoR

		TbVmin.Enabled = Ovr Or EcoR
		Label23.Enabled = Ovr Or EcoR
		Label21.Enabled = Ovr Or EcoR
	End Sub

#End Region

	Public Sub UpdatePic()
		Dim VEH0 As New cVEH
		Dim i As Integer
		Dim pmax As Single

		Dim f As cFile_V3
		Dim lM As List(Of Single)
		Dim lup As List(Of Single)
		Dim ldown As List(Of Single)
		Dim line As String()


		Dim HDVclass As String
		'Dim m0 As 

		Dim s As Series
		Dim a As ChartArea
		Dim img As Image

		Dim EngOK = False

		TbHVCclass.Text = ""
		TbVehCat.Text = ""
		TbMass.Text = ""
		TbAxleConf.Text = ""
		TbEngTxt.Text = ""
		TbGbxTxt.Text = ""
		PicVehicle.Image = Nothing
		PicBox.Image = Nothing


		VEH0.FilePath = fFileRepl(TbVEH.Text, fPATH(VECTOfile))
		If VEH0.ReadFile(False) Then
			Dim maxMass = (VEH0.MassMax * 1000).SI(Of Kilogram)()		   'CSng(fTextboxToNumString(TbMassMass.Text))

			Dim s0 As Segment = Nothing
			Try
				s0 = DeclarationData.Segments.Lookup(VEH0.VehCat, VEH0.AxleConf, maxMass, 0.SI(Of Kilogram), True)
			Catch
			End Try
			If Not s0 Is Nothing Then
				HDVclass = s0.VehicleClass.GetClassNumber()

				If Cfg.DeclMode Then
					LvCycles.Items.Clear()
					For Each m0 In s0.Missions
						LvCycles.Items.Add(m0.MissionType.ToString())
					Next
				End If

			Else
				HDVclass = "-"
			End If

			PicVehicle.Image = ConvPicPath(HDVclass, False)	'Image.FromFile(cDeclaration.ConvPicPath(HDVclass, False))

			TbHVCclass.Text = "HDV Class " & HDVclass
			TbVehCat.Text = VEH0.VehCat.GetCategoryName()	'ConvVehCat(VEH0.VehCat, True)
			TbMass.Text = VEH0.MassMax & " t"
			TbAxleConf.Text = VEH0.AxleConf.GetName() 'ConvAxleConf(VEH0.AxleConf)

		End If


		Dim OkCount = 0

		Dim ENG0 = New cENG
		ENG0.FilePath = fFileRepl(TbENG.Text, fPATH(VECTOfile))

		'Create plot
		Dim MyChart = New Chart
		MyChart.Width = PicBox.Width
		MyChart.Height = PicBox.Height

		a = New ChartArea

		Dim FLD0 = New cFLD

		If ENG0.ReadFile(False) Then

			n_idle = ENG0.Nidle
			FLDfile = ENG0.PathFLD

			EngOK = True
			FLD0.FilePath = ENG0.PathFLD

			If FLD0.ReadFile(False, False) Then

				s = New Series
				s.Points.DataBindXY(FLD0.LnU, FLD0.LTq)
				s.ChartType = SeriesChartType.FastLine
				s.BorderWidth = 2
				s.Color = Color.DarkBlue
				s.Name = "Full load"
				MyChart.Series.Add(s)

				s = New Series
				s.Points.DataBindXY(FLD0.LnU, FLD0.LTqDrag)
				s.ChartType = SeriesChartType.FastLine
				s.BorderWidth = 2
				s.Color = Color.Blue
				s.Name = "Motoring"
				MyChart.Series.Add(s)

				OkCount += 1

				pmax = FLD0.Pfull(FLD0.fnUrated)

			End If

			TbEngTxt.Text = (ENG0.Displ / 1000).ToString("0.0") & " l " & pmax.ToString("#") & " kW  " & ENG0.ModelName


			Dim MAP0 = New cMAP
			MAP0.FilePath = ENG0.PathMAP

			If MAP0.ReadFile(False) Then

				s = New Series
				s.Points.DataBindXY(MAP0.nU, MAP0.Tq)
				s.ChartType = SeriesChartType.Point
				s.MarkerSize = 3
				s.Color = Color.Red
				s.Name = "Map"
				MyChart.Series.Add(s)

				OkCount += 1

			End If

		End If

		Dim GBX0 = New cGBX
		GBX0.FilePath = fFileRepl(TbGBX.Text, fPATH(VECTOfile))

		If GBX0.ReadFile(False) Then

			TbGbxTxt.Text = GBX0.GearCount & "-Speed " & GBX0.gs_Type.ShortName() & "  " & GBX0.ModelName

			If Cfg.DeclMode Then

				If EngOK Then

					For i = 1 To GBX0.GearCount

						FLD0.FilePath = ENG0.PathFLD

						If FLD0.ReadFile(True, False) Then

							If FLD0.Init(ENG0.Nidle) Then

								'Dim engine As CombustionEngineData = ConvertToEngineData(FLD0, F_VECTO.n_idle)
								'Dim shiftLines As ShiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(Gear - 1,
								'																			engine.FullLoadCurve, gears,
								'																			engine,
								'																			Double.Parse(LvGears.Items(0).SubItems(F_GBX.GearboxTbl.Ratio).Text,
								'																						CultureInfo.InvariantCulture),
								'																			(.rdyn / 1000.0).SI(Of Meter))

								's = New Series
								's.Points.DataBindXY(shiftLines.Upshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
								'					shiftLines.Upshift.Select(Function(pt) pt.Torque.Value()).ToList())
								's.ChartType = SeriesChartType.FastLine
								's.BorderWidth = 2
								's.Color = Color.DarkRed
								's.Name = "Upshift curve (" & i & ")"
								'MyChart.Series.Add(s)

								's = New Series
								's.Points.DataBindXY(
								'	shiftLines.Downshift.Select(Function(pt) pt.AngularSpeed.Value() / Constants.RPMToRad).ToList(),
								'	shiftLines.Downshift.Select(Function(pt) pt.Torque.Value()).ToList())
								's.ChartType = SeriesChartType.FastLine
								's.BorderWidth = 2
								's.Color = Color.DarkRed
								's.Name = "Downshift curve (" & i & ")"
								'MyChart.Series.Add(s)
							End If


							OkCount += 1

							pmax = FLD0.Pfull(FLD0.fnUrated)

						End If

					Next

				End If

			Else

				f = New cFile_V3
				For i = 1 To GBX0.GearCount

					lM = New List(Of Single)
					lup = New List(Of Single)
					ldown = New List(Of Single)

					If f.OpenRead(GBX0.gsFile(i)) Then

						f.ReadLine()

						Try

							Do While Not f.EndOfFile
								line = f.ReadLine
								lM.Add(CSng(line(0)))
								lup.Add(CSng(line(1)))
								ldown.Add(CSng(line(2)))
							Loop

							s = New Series
							s.Points.DataBindXY(lup, lM)
							s.ChartType = SeriesChartType.FastLine
							s.BorderWidth = 2
							s.Color = Color.DarkRed
							s.Name = "Upshift curve"
							' MyChart.Series.Add(s) 'MQ 2016-06-20: do not plot shift lines in engine dialog

							s = New Series
							s.Points.DataBindXY(ldown, lM)
							s.ChartType = SeriesChartType.FastLine
							s.BorderWidth = 2
							s.Color = Color.DarkRed
							s.Name = "Downshift curve"
							'MyChart.Series.Add(s) 'MQ 2016-06-20:do not plot shift lines in engine dialog

							OkCount += 1

							f.Close()

						Catch ex As Exception
							f.Close()
						End Try

					End If

				Next

			End If

		End If

		If OkCount > 0 Then

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

			MyChart.Update()

			img = New Bitmap(MyChart.Width, MyChart.Height, PixelFormat.Format32bppArgb)
			MyChart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))

			PicBox.Image = img


		End If
	End Sub


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


	'AA-TB
	Private Sub picAuxInfo_MouseEnter(sender As Object, e As EventArgs) Handles picAuxInfo.MouseEnter


		If cboAdvancedAuxiliaries.SelectedIndex = -1 Then Exit Sub

		'Get tooltip
		Dim item As cAdvancedAuxiliary

		item = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary)

		If item.AuxiliaryVersion = "CLASSIC" Then

			ToolTip1.ToolTipTitle = "Classic Vecto Auxiliaries"
			ToolTip1.SetToolTip(picAuxInfo, "Uses original basic auxiliaries calculation")

		Else

			ToolTip1.ToolTipTitle = "Advanced Auxiliary Information"
			ToolTip1.SetToolTip(picAuxInfo, item.AuxiliaryName & " : Version=" & item.AuxiliaryVersion)

		End If
	End Sub

	'AA-TB
	Private Sub btnBrowseAAUXFile_Click(sender As Object, e As EventArgs) Handles btnBrowseAAUXFile.Click

		If String.IsNullOrEmpty(VECTOfile) Then
			MessageBox.Show(
				"Please complete and save a valid new .vecto file before adding/configuring advanced bus auxiliaries.")
			Return
		End If

		Dim aauxFileValidated As Boolean = False
		Dim fbAux As New cFileBrowser(True, False)
		Dim message As String = String.Empty
		Dim absoluteAuxPath As String
		Dim assembly As cAdvancedAuxiliary

		'If Classic is selected, then bail
		If cboAdvancedAuxiliaries.SelectedIndex = 0 Then Return

		'Get Absolute Path for AAUX FILE.
		absoluteAuxPath = ResolveAAUXFilePath(fPATH(VECTOfile), txtAdvancedAuxiliaryFile.Text)

		'Set Extensions
		fbAux.Extensions = New String() {"AAUX"}

		Try

			assembly = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary)

			Dim validAAUXFile As Boolean = ValidateAAUXFile(absoluteAuxPath, assembly.AssemblyName,
															assembly.AuxiliaryVersion, message)
			Dim fileExists As Boolean = File.Exists(absoluteAuxPath)

			If fileExists AndAlso validAAUXFile Then
				ConfigureAdvancedAuxiliaries(assembly.AssemblyName, assembly.AuxiliaryVersion,
											txtAdvancedAuxiliaryFile.Text, VECTOfile)
			Else

				Dim needToFindOrCreateFile As Boolean = True

				While needToFindOrCreateFile

					'Find / Create  file and configure.
					If fbAux.CustomDialog(absoluteAuxPath, False, False, tFbExtMode.ForceExt, False, String.Empty) Then
						txtAdvancedAuxiliaryFile.Text = fFileWoDir(fbAux.Files(0), fPATH(VECTOfile))
						assembly = DirectCast(cboAdvancedAuxiliaries.SelectedItem, cAdvancedAuxiliary)

						If _
							File.Exists(ResolveAAUXFilePath(fPATH(VECTOfile), txtAdvancedAuxiliaryFile.Text)) OrElse
							MsgBox("Do you want to create a new .AAUX file?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
							needToFindOrCreateFile = False
							ConfigureAdvancedAuxiliaries(assembly.AssemblyName, assembly.AuxiliaryVersion,
														txtAdvancedAuxiliaryFile.Text, VECTOfile)
						End If
					Else
						needToFindOrCreateFile = False
					End If

				End While

			End If

		Catch ex As Exception
			MessageBox.Show("There was an error configuring your Advanced Auxiliary File")
		End Try
	End Sub

	'AA-TB
	Private Sub cboAdvancedAuxiliaries_SelectedIndexChanged(sender As Object, e As EventArgs) _
		Handles cboAdvancedAuxiliaries.SelectedIndexChanged

		'Enable or otherwise the text box and browser button associated with Advanced Axuiliaries
		If cboAdvancedAuxiliaries.SelectedIndex = 0 Then

			btnBrowseAAUXFile.Enabled = False
			txtAdvancedAuxiliaryFile.Enabled = False

		Else

			btnBrowseAAUXFile.Enabled = True
			txtAdvancedAuxiliaryFile.Enabled = True

		End If
	End Sub


	'AA-TB
	Private Sub btnAAUXOpen_Click(sender As Object, e As EventArgs) Handles btnAAUXOpen.Click

		OpenFiles(fFileRepl(txtAdvancedAuxiliaryFile.Text, fPATH(VECTOfile)))
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnDfTargetSpeed.Click
		If fbDfTargetSpeed.OpenDialog(fFileRepl(tbLacDfTargetSpeedFile.Text, fPATH(VECTOfile))) Then _
			tbLacDfTargetSpeedFile.Text = fFileWoDir(fbDfTargetSpeed.Files(0), fPATH(VECTOfile))
	End Sub

	Private Sub btnDfVelocityDrop_Click(sender As Object, e As EventArgs)
		If fbDfVelocityDrop.OpenDialog(fFileRepl(tbLacDfVelocityDropFile.Text, fPATH(VECTOfile))) Then _
			tbLacDfVelocityDropFile.Text = fFileWoDir(fbDfVelocityDrop.Files(0), fPATH(VECTOfile))
	End Sub
End Class


