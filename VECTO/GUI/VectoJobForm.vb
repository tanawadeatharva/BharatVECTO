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
'Option Infer On

Imports System.Collections.Generic
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox

''' <summary>
''' Job Editor. Create/Edit VECTO job files (.vecto)
''' </summary>
''' <remarks></remarks>
Public Class VectoJobForm
	Public VectoFile As String
	Private _changed As Boolean = False

	Private _pgDriver As TabPage

	Private _pgDriverOn As Boolean = True

	Private _auxDialog As VehicleAuxiliariesDialog

	'AA-TB
	'Populate Advanced Auxiliaries
	Private Sub PopulateAdvancedAuxiliaries()
		'Scan the program directory for DLL's which are AdvancedAuxiliaries and display
		Dim aList As List(Of AdvancedAuxiliary) = DiscoverAdvancedAuxiliaries()

		cboAdvancedAuxiliaries.DataSource = aList
		cboAdvancedAuxiliaries.DisplayMember = "AuxiliaryName"
	End Sub


	'Initialise form
	Private Sub F02_GEN_Load(sender As Object, e As EventArgs) Handles Me.Load
		Dim x As Integer

		_auxDialog = New VehicleAuxiliariesDialog

		_pgDriver = TabPgDriver

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

		_changed = False
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

		TbSSspeed.Text = DeclarationData.Driver.StartStop.MaxSpeed.AsKmph().ToGUIFormat()	'cDeclaration.SSspeed
		TbSStime.Text = DeclarationData.Driver.StartStop.MinTime.ToGUIFormat()	 'cDeclaration.SStime
		TbSSdelay.Text = DeclarationData.Driver.StartStop.Delay.ToGUIFormat()	 ' cDeclaration.SSdelay
		tbLacPreviewFactor.Text = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor.ToGUIFormat()
		tbLacDfTargetSpeedFile.Text = ""
		tbLacDfVelocityDropFile.Text = ""

		TbOverspeed.Text = DeclarationData.Driver.OverSpeedEcoRoll.OverSpeed.AsKmph().ToGUIFormat()	 'cDeclaration.Overspeed
		TbUnderSpeed.Text = DeclarationData.Driver.OverSpeedEcoRoll.UnderSpeed.AsKmph().ToGUIFormat() _
		' cDeclaration.Underspeed
		TbVmin.Text = DeclarationData.Driver.OverSpeedEcoRoll.MinSpeed.AsKmph().ToGUIFormat()	 'cDeclaration.ECvmin

		If _
			LvAux.Items.Count <> 5 OrElse
			(LvAux.Items(0).Text <> VectoCore.Configuration.Constants.Auxiliaries.IDs.Fan OrElse
			LvAux.Items(1).Text <> VectoCore.Configuration.Constants.Auxiliaries.IDs.SteeringPump OrElse
			LvAux.Items(2).Text <> VectoCore.Configuration.Constants.Auxiliaries.IDs.HeatingVentilationAirCondition OrElse
			LvAux.Items(3).Text <> VectoCore.Configuration.Constants.Auxiliaries.IDs.ElectricSystem OrElse
			LvAux.Items(4).Text <> VectoCore.Configuration.Constants.Auxiliaries.IDs.PneumaticSystem) Then
			LvAux.Items.Clear()


			LvAux.Items.Add(GetTechListForAux(AuxiliaryType.Fan, DeclarationData.Fan))

			LvAux.Items.Add(GetTechListForAux(AuxiliaryType.SteeringPump, DeclarationData.SteeringPump))

			LvAux.Items.Add(GetTechListForAux(AuxiliaryType.HVAC, DeclarationData.HeatingVentilationAirConditioning))

			LvAux.Items.Add(GetTechListForAux(AuxiliaryType.ElectricSystem, DeclarationData.ElectricSystem))

			LvAux.Items.Add(GetTechListForAux(AuxiliaryType.PneumaticSystem, DeclarationData.PneumaticSystem))

		End If
	End Sub

	Protected Function GetTechListForAux(type As AuxiliaryType, aux As IDeclarationAuxiliaryTable) _
		As ListViewItem
		Dim listViewItem As ListViewItem

		listViewItem = New ListViewItem(type.Key())
		listViewItem.SubItems.Add(type.Name())
		Dim auxtech As String() = aux.GetTechnologies()
		If auxtech.Count > 1 Then
			listViewItem.SubItems.Add("")
		Else
			listViewItem.SubItems.Add(auxtech(0))
		End If
		Return listViewItem
	End Function


	'Show/Hide "Driver Assist" Tab
	Private Sub SetDrivertab(onOff As Boolean)
		If onOff Then
			If Not _pgDriverOn Then
				_pgDriverOn = True
				TabControl1.TabPages.Insert(1, _pgDriver)
			End If
		Else
			If _pgDriverOn Then
				_pgDriverOn = False
				TabControl1.Controls.Remove(_pgDriver)
			End If
		End If
	End Sub


#Region "Browse Buttons"

	Private Sub ButtonVEH_Click(sender As Object, e As EventArgs) Handles ButtonVEH.Click
		If VehicleFileBrowser.OpenDialog(FileRepl(TbVEH.Text, GetPath(VECTOfile))) Then
			TbVEH.Text = GetFilenameWithoutDirectory(VehicleFileBrowser.Files(0), GetPath(VECTOfile))
		End If
	End Sub

	Private Sub ButtonMAP_Click(sender As Object, e As EventArgs) Handles ButtonMAP.Click
		If EngineFileBrowser.OpenDialog(FileRepl(TbENG.Text, GetPath(VECTOfile))) Then
			TbENG.Text = GetFilenameWithoutDirectory(EngineFileBrowser.Files(0), GetPath(VECTOfile))
		End If
	End Sub

	Private Sub ButtonGBX_Click(sender As Object, e As EventArgs) Handles ButtonGBX.Click
		If GearboxFileBrowser.OpenDialog(FileRepl(TbGBX.Text, GetPath(VECTOfile))) Then
			TbGBX.Text = GetFilenameWithoutDirectory(GearboxFileBrowser.Files(0), GetPath(VECTOfile))
		End If
	End Sub

	Private Sub BtDesMaxBr_Click_1(sender As Object, e As EventArgs) Handles BtDesMaxBr.Click
		If DriverAccelerationFileBrowser.OpenDialog(FileRepl(TbDesMaxFile.Text, GetPath(VECTOfile))) Then
			TbDesMaxFile.Text = GetFilenameWithoutDirectory(DriverAccelerationFileBrowser.Files(0), GetPath(VECTOfile))
		End If
	End Sub

	Private Sub BtAccOpen_Click(sender As Object, e As EventArgs) Handles BtAccOpen.Click
		OpenFiles(FileRepl(TbDesMaxFile.Text, GetPath(VECTOfile)))
	End Sub

#End Region

#Region "Open Buttons"

	'Open Vehicle Editor
	Private Sub ButOpenVEH_Click(sender As Object, e As EventArgs) Handles ButOpenVEH.Click
		Dim f As String
		f = FileRepl(TbVEH.Text, GetPath(VECTOfile))

		'Thus Veh-file is returned
		VehicleForm.JobDir = GetPath(VECTOfile)
		VehicleForm.AutoSendTo = True

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not VehicleForm.Visible Then
			VehicleForm.Show()
		Else
			If VehicleForm.WindowState = FormWindowState.Minimized Then VehicleForm.WindowState = FormWindowState.Normal
			VehicleForm.BringToFront()
		End If

		If Not Trim(f) = "" Then
			Try
				VehicleForm.OpenVehicle(f)
			Catch ex As Exception
				MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
			End Try
		End If
	End Sub

	'Open Engine Editor
	Private Sub ButOpenENG_Click(sender As Object, e As EventArgs) Handles ButOpenENG.Click
		Dim f As String
		f = FileRepl(TbENG.Text, GetPath(VECTOfile))

		'Thus Veh-file is returned
		EngineForm.JobDir = GetPath(VECTOfile)
		EngineForm.AutoSendTo = True

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not EngineForm.Visible Then
			EngineForm.Show()
		Else
			If EngineForm.WindowState = FormWindowState.Minimized Then EngineForm.WindowState = FormWindowState.Normal
			EngineForm.BringToFront()
		End If

		If Not Trim(f) = "" Then EngineForm.OpenEngineFile(f)
	End Sub

	'Open Gearbox Editor
	Private Sub ButOpenGBX_Click(sender As Object, e As EventArgs) Handles ButOpenGBX.Click
		Dim f As String
		f = FileRepl(TbGBX.Text, GetPath(VECTOfile))

		'Thus Veh-file is returned
		GearboxForm.JobDir = GetPath(VECTOfile)
		GearboxForm.AutoSendTo = True

		If Not Trim(f) = "" Then
			If Not File.Exists(f) Then
				MsgBox("File not found!")
				Exit Sub
			End If
		End If

		If Not GearboxForm.Visible Then
			GearboxForm.Show()
		Else
			If GearboxForm.WindowState = FormWindowState.Minimized Then GearboxForm.WindowState = FormWindowState.Normal
			GearboxForm.BringToFront()
		End If

		If Not Trim(f) = "" Then GearboxForm.OpenGbx(f)
	End Sub

#End Region

#Region "Toolbar"

	'New
	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		VectoNew()
	End Sub

	'Open
	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If JobfileFileBrowser.OpenDialog(VECTOfile, False, "vecto") Then VECTOload2Form(JobfileFileBrowser.Files(0))
	End Sub

	'Save
	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		Save()
	End Sub

	'Save As
	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		If JobfileFileBrowser.SaveDialog(VECTOfile) Then Call VECTOsave(JobfileFileBrowser.Files(0))
	End Sub

	'Send to Job file list in main form
	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click
		If ChangeCheckCancel() Then Exit Sub
		If VECTOfile = "" Then
			MsgBox("File not found!" & ChrW(10) & ChrW(10) & "Save file and try again.")
		Else
			MainForm.AddToJobListView(VECTOfile)
		End If
	End Sub

	'Help
	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim browserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim defaultBrowserPath As String =
					Regex.Match(browserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(defaultBrowserPath,
						String.Format("""{0}{1}""", MyAppPath, "User Manual\help.html#job-editor"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub


#End Region

	'Save ("Save" or "Save As" when new file)
	Private Function Save() As Boolean
		If VECTOfile = "" Then
			If JobfileFileBrowser.SaveDialog("") Then
				VECTOfile = JobfileFileBrowser.Files(0)
			Else
				Return False
			End If
		End If
		Return VECTOsave(VECTOfile)
	End Function

	'Open file
	Public Sub VECTOload2Form(file As String)

		If ChangeCheckCancel() Then Exit Sub

		VectoNew()

		'Read GEN
		Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file), 
																IEngineeringInputDataProvider)
		Dim vectoJob As IEngineeringJobInputData = inputData.JobInputData()

		If Cfg.DeclMode <> vectoJob.SavedInDeclarationMode Then
			Select Case WrongMode()
				Case 1
					Close()
					MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
					MainForm.OpenVectoFile(file)
				Case -1
					Exit Sub
			End Select
		End If

		VECTOfile = file
		_basePath = Path.GetDirectoryName(file)
		'Update Form

		'Files -----------------------------
		TbVEH.Text = GetRelativePath(inputData.VehicleInputData.Source, _basePath)
		TbENG.Text = GetRelativePath(inputData.EngineInputData.Source, _basePath)
		TbGBX.Text = GetRelativePath(inputData.GearboxInputData.Source, _basePath)

		'Start/Stop
		Dim driver As IDriverEngineeringInputData = inputData.DriverInputData
		ChBStartStop.Checked = driver.StartStop.Enabled
		TbSSspeed.Text = driver.StartStop.MaxSpeed.AsKmph().ToGUIFormat()
		TbSStime.Text = driver.StartStop.MinTime.ToGUIFormat()
		TbSSdelay.Text = driver.StartStop.Delay.ToGUIFormat()

		If (Cfg.DeclMode) Then
			TbDesMaxFile.Text = ""
			'AA-TB
			'Try and Select any previously selected Auxiliary Type
			Dim declarationInput As IDeclarationInputDataProvider = CType(inputData, IDeclarationInputDataProvider)
			Dim auxInput As IAuxiliariesDeclarationInputData = declarationInput.AuxiliaryInputData()

			cboAdvancedAuxiliaries.SelectedIndex = 0

			LvAux.Items.Clear()
			Dim entry As IAuxiliaryDeclarationInputData
			For Each entry In auxInput.Auxiliaries
				Dim lv0 As ListViewItem = New ListViewItem
				lv0.SubItems(0).Text = AuxiliaryTypeHelper.GetAuxKey(entry.Type)
				lv0.SubItems.Add(AuxiliaryTypeHelper.ToString(entry.Type))
				lv0.SubItems.Add(String.Join(", ", entry.Technology))
				LvAux.Items.Add(lv0)
			Next
		Else
			'VACC
			TbDesMaxFile.Text =
				If(driver.AccelerationCurve Is Nothing, "", GetRelativePath(driver.AccelerationCurve.Source, _basePath))


			Dim auxInput As IAuxiliariesEngineeringInputData = inputData.AuxiliaryInputData()
			For Each item As AdvancedAuxiliary In cboAdvancedAuxiliaries.Items
				If _
					item.AssemblyName = auxInput.AuxiliaryAssembly.ToString() AndAlso auxInput.AuxiliaryVersion = item.AuxiliaryVersion _
					Then
					cboAdvancedAuxiliaries.SelectedItem = item
					Exit For
				End If
			Next
			'AA-TB
			'Assign any previously saved Axiliary FilePath
			txtAdvancedAuxiliaryFile.Text = auxInput.AdvancedAuxiliaryFilePath

			LvAux.Items.Clear()
			For Each entry As IAuxiliaryEngineeringInputData In auxInput.Auxiliaries
				Dim lv0 As ListViewItem = New ListViewItem
				lv0.SubItems(0).Text = entry.ID
				lv0.SubItems.Add(entry.AuxiliaryType.ToString())
				lv0.SubItems.Add(If(entry.DemandMap Is Nothing, "", entry.DemandMap.Source))
				LvAux.Items.Add(lv0)
			Next

		End If

		Dim sb As ICycleData
		For Each sb In vectoJob.Cycles
			Dim lv0 As ListViewItem = New ListViewItem
			lv0.Text = sb.Name
			LvCycles.Items.Add(lv0)
		Next

		CbEngOnly.Checked = vectoJob.EngineOnlyMode

		If driver.OverSpeedEcoRoll.Mode = DriverMode.EcoRoll Then
			RdEcoRoll.Checked = True
		ElseIf driver.OverSpeedEcoRoll.Mode = DriverMode.Overspeed Then
			RdOverspeed.Checked = True
		Else
			RdOff.Checked = True
		End If
		TbOverspeed.Text = driver.OverSpeedEcoRoll.OverSpeed.AsKmph().ToGUIFormat()
		TbUnderSpeed.Text = driver.OverSpeedEcoRoll.UnderSpeed.AsKmph().ToGUIFormat()
		TbVmin.Text = driver.OverSpeedEcoRoll.MinSpeed.AsKmph().ToGUIFormat()
		CbLookAhead.Checked = driver.Lookahead.Enabled
		'TbAlookahead.Text = CStr(VEC0.ALookahead)
		'TbVminLA.Text = CStr(VEC0.VMinLa)
		tbLacPreviewFactor.Text = driver.Lookahead.LookaheadDistanceFactor.ToGUIFormat()
		tbDfCoastingOffset.Text = driver.Lookahead.CoastingDecisionFactorOffset.ToGUIFormat()
		tbDfCoastingScale.Text = driver.Lookahead.CoastingDecisionFactorScaling.ToGUIFormat()

		tbLacDfTargetSpeedFile.Text = If(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup Is Nothing, "",
										GetRelativePath(driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup.Source, _basePath))
		tbLacDfVelocityDropFile.Text = If(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup Is Nothing, "",
										GetRelativePath(driver.Lookahead.CoastingDecisionFactorVelocityDropLookup.Source, _basePath))

		'-------------------------------------------------------------

		DeclInit()


		EngineForm.AutoSendTo = False
		GearboxForm.AutoSendTo = False
		VehicleForm.AutoSendTo = False


		Dim x As Integer = Len(file)
		While Mid(file, x, 1) <> "\" And x > 0
			x = x - 1
		End While
		Text = Mid(file, x + 1, Len(file) - x)
		_changed = False
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
			Dim absoluteAAUxFile As String = ResolveAAUXFilePath(GetPath(VECTOfile), txtAdvancedAuxiliaryFile.Text)

			Dim aaAssemblyName As String = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary).AssemblyName
			Dim aaAssemblyVersion As String = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary).AuxiliaryVersion


			If Not ValidateAAUXFile(absoluteAAUxFile, aaAssemblyName, aaAssemblyVersion, message) Then
				MessageBox.Show(
					String.Format("You have selected an advanced auxiliary *Auxiliary Type*, but the file specified is invalid :{0}",
								message))
				Return False
			End If

		End If


		Dim vec0 As VectoJob = New VectoJob
		vec0.FilePath = file

		'Files ------------------------------------------------- -----------------

		vec0.PathVeh = TbVEH.Text
		vec0.PathEng = TbENG.Text

		For Each lv0 As ListViewItem In LvCycles.Items
			Dim sb As SubPath = New SubPath
			sb.Init(GetPath(file), lv0.Text)
			vec0.CycleFiles.Add(sb)
		Next

		vec0.PathGbx = TbGBX.Text


		'Start/Stop
		vec0.StartStop = ChBStartStop.Checked
		vec0.StartStopMaxSpeed = TbSSspeed.Text.ToDouble()
		vec0.StartStopTime = TbSStime.Text.ToDouble()
		vec0.StartStopDelay = TbSSdelay.Text.ToDouble()

		'a_DesMax
		vec0.DesMaxFile = TbDesMaxFile.Text

		'AA-TB
		vec0.AuxiliaryAssembly = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary).AssemblyName
		vec0.AuxiliaryVersion = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary).AuxiliaryVersion
		vec0.AdvancedAuxiliaryFilePath = txtAdvancedAuxiliaryFile.Text

		For Each lv0 As ListViewItem In LvAux.Items
			Dim auxEntry As VectoJob.AuxEntry = New VectoJob.AuxEntry

			If Cfg.DeclMode Then
				auxEntry.TechnologyList.Clear()
				auxEntry.TechnologyList.Add(lv0.SubItems(2).Text)
			Else
				auxEntry.Path.Init(GetPath(file), lv0.SubItems(2).Text)
			End If

			auxEntry.Type = lv0.SubItems(1).Text
			vec0.AuxPaths.Add(lv0.SubItems(0).Text, auxEntry)
		Next

		vec0.EngineOnly = CbEngOnly.Checked

		vec0.EcoRollOn = RdEcoRoll.Checked
		vec0.OverSpeedOn = RdOverspeed.Checked
		vec0.OverSpeed = TbOverspeed.Text.ToDouble()
		vec0.UnderSpeed = TbUnderSpeed.Text.ToDouble()
		vec0.VMin = TbVmin.Text.ToDouble()
		vec0.LookAheadOn = CbLookAhead.Checked
		'vec0.ALookahead = CSng(fTextboxToNumString(TbAlookahead.Text))
		'vec0.VMinLa = CSng(fTextboxToNumString(TbVminLA.Text))

		vec0.LacPreviewFactor = tbLacPreviewFactor.Text.ToDouble()
		vec0.LacDfOffset = tbDfCoastingOffset.Text.ToDouble()
		vec0.LacDfScale = tbDfCoastingScale.Text.ToDouble()
		vec0.LacDfTargetSpeedFile = tbLacDfTargetSpeedFile.Text
		vec0.LacDfVelocityDropFile = tbLacDfVelocityDropFile.Text
		'------------------------------------------------------------

		'SAVE
		If Not vec0.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		VECTOfile = file

		file = GetFilenameWithoutPath(VECTOfile, True)

		Text = file
		ToolStripStatusLabelGEN.Text = ""

		MainForm.AddToJobListView(VECTOfile)

		_changed = False

		Return True
	End Function

	'New file
	Public Sub VectoNew()

		If ChangeCheckCancel() Then Exit Sub

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

		EngineForm.AutoSendTo = False

		VECTOfile = ""
		Text = "Job Editor"
		ToolStripStatusLabelGEN.Text = ""
		_changed = False
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
		If Not _changed Then
			ToolStripStatusLabelGEN.Text = "Unsaved changes in current file"
			_changed = True
		End If
	End Sub

	' "Save changes? "... Returns True if User aborts
	Private Function ChangeCheckCancel() As Boolean

		If _changed Then

			Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
				Case MsgBoxResult.Yes
					Return Not Save()
				Case MsgBoxResult.Cancel
					Return True
				Case Else 'MsgBoxResult.No
					_changed = False
					Return False
			End Select

		Else

			Return False

		End If
	End Function

#End Region

#Region "Aux Listview"

	Private Sub ButAuxAdd_Click(sender As Object, e As EventArgs) Handles ButAuxAdd.Click
		Dim id As String

		_auxDialog.VehPath = GetPath(VECTOfile)
		_auxDialog.TbPath.Text = ""
		_auxDialog.CbType.SelectedIndex = -1
		_auxDialog.CbType.Text = ""
		_auxDialog.TbID.Text = ""	'!!! Set Type before ID, because changing the type will overwrite the id !!!

lbDlog:
		If _auxDialog.ShowDialog = DialogResult.OK Then

			id = UCase(Trim(_auxDialog.TbID.Text))

			Dim lv0 As ListViewItem
			For Each lv0 In LvAux.Items
				If lv0.SubItems(0).Text = id Then
					MsgBox("ID '" & id & "' already defined!", MsgBoxStyle.Critical)
					_auxDialog.TbID.SelectAll()
					_auxDialog.TbID.Focus()
					GoTo lbDlog
				End If
			Next

			lv0 = New ListViewItem
			lv0.SubItems(0).Text = UCase(Trim(_auxDialog.TbID.Text))
			lv0.SubItems.Add(Trim(_auxDialog.CbType.Text))
			lv0.SubItems.Add(Trim(_auxDialog.TbPath.Text))
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

		Dim selItem As ListViewItem = LvAux.SelectedItems(0)

		_auxDialog.VehPath = GetPath(VECTOfile)
		_auxDialog.CbType.SelectedIndex = -1
		_auxDialog.CbType.Text = selItem.SubItems(1).Text
		_auxDialog.TbID.Text = selItem.SubItems(0).Text	'After Type-set!

		If Cfg.DeclMode Then
			_auxDialog.CbTech.Text = selItem.SubItems(2).Text
			_auxDialog.TbPath.Text = ""
		Else
			_auxDialog.CbTech.SelectedIndex = -1
			_auxDialog.TbPath.Text = selItem.SubItems(2).Text
		End If

		If _auxDialog.ShowDialog = DialogResult.OK Then
			selItem.SubItems(0).Text = UCase(Trim(_auxDialog.TbID.Text))
			selItem.SubItems(1).Text = Trim(_auxDialog.CbType.Text)

			If Cfg.DeclMode Then
				selItem.SubItems(2).Text = Trim(_auxDialog.CbTech.Text)
			Else
				selItem.SubItems(2).Text = Trim(_auxDialog.TbPath.Text)
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
			OpenFiles(FileRepl(LvCycles.SelectedItems(0).SubItems(0).Text, GetPath(VECTOfile)))
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
		Dim genDir As String = GetPath(VECTOfile)

		If DrivingCycleFileBrowser.OpenDialog("", True) Then
			Dim s As String
			For Each s In DrivingCycleFileBrowser.Files
				LvCycles.Items.Add(GetFilenameWithoutDirectory(s, genDir))
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
		Dim onOff As Boolean

		onOff = Not CbEngOnly.Checked

		SetDrivertab(onOff)

		ButOpenVEH.Enabled = onOff
		TbVEH.Enabled = onOff
		ButtonVEH.Enabled = onOff
		ButOpenGBX.Enabled = onOff
		TbGBX.Enabled = onOff
		ButtonGBX.Enabled = onOff
		GrAux.Enabled = onOff
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
		Dim ecoRoll As Boolean
		Dim overspeed As Boolean

		Change()

		ecoRoll = RdEcoRoll.Checked
		overspeed = RdOverspeed.Checked

		TbOverspeed.Enabled = overspeed Or ecoRoll
		Label13.Enabled = overspeed Or ecoRoll
		Label14.Enabled = overspeed Or ecoRoll

		TbUnderSpeed.Enabled = ecoRoll
		Label22.Enabled = ecoRoll
		Label20.Enabled = ecoRoll

		TbVmin.Enabled = overspeed Or ecoRoll
		Label23.Enabled = overspeed Or ecoRoll
		Label21.Enabled = overspeed Or ecoRoll
	End Sub

#End Region

	Public Sub UpdatePic()

		Dim i As Integer
		Dim pmax As Double

		Dim HDVclass As String

		Dim s As Series
		Dim a As ChartArea
		Dim img As Bitmap

		Dim engOk As Boolean = False

		TbHVCclass.Text = ""
		TbVehCat.Text = ""
		TbMass.Text = ""
		TbAxleConf.Text = ""
		TbEngTxt.Text = ""
		TbGbxTxt.Text = ""
		PicVehicle.Image = Nothing
		PicBox.Image = Nothing

		Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(VECTOfile), 
																IEngineeringInputDataProvider)
		Dim vehicle As IVehicleEngineeringInputData = inputData.VehicleInputData

		If Not vehicle Is Nothing Then
			Dim maxMass As Kilogram = vehicle.GrossVehicleMassRating					'CSng(fTextboxToNumString(TbMassMass.Text))

			Dim s0 As Segment = Nothing
			Try
				s0 = DeclarationData.Segments.Lookup(vehicle.VehicleCategory, vehicle.AxleConfiguration, maxMass, 0.SI(Of Kilogram),
													True)
			Catch
			End Try
			If Not s0 Is Nothing Then
				HDVclass = s0.VehicleClass.GetClassNumber()

				If Cfg.DeclMode Then
					LvCycles.Items.Clear()
					Dim m0 As Mission
					For Each m0 In s0.Missions
						LvCycles.Items.Add(m0.MissionType.ToString())
					Next
				End If

			Else
				HDVclass = "-"
			End If

			PicVehicle.Image = ConvPicPath(If(s0 Is Nothing, -1, HDVclass.ToInt()), False) _
			'Image.FromFile(cDeclaration.ConvPicPath(HDVclass, False))

			TbHVCclass.Text = "HDV Class " & HDVclass
			TbVehCat.Text = vehicle.VehicleCategory.GetCategoryName()	'ConvVehCat(VEH0.VehCat, True)
			TbMass.Text = (vehicle.GrossVehicleMassRating.Value() / 1000) & " t"
			TbAxleConf.Text = vehicle.AxleConfiguration.GetName()	'ConvAxleConf(VEH0.AxleConf)

		End If


		Dim okCount As Integer = 0

		Dim engine As IEngineEngineeringInputData = inputData.EngineInputData
		'engine.FilePath = fFileRepl(TbENG.Text, GetPath(VECTOfile))

		'Create plot
		Dim chart As Chart = New Chart
		chart.Width = PicBox.Width
		chart.Height = PicBox.Height

		a = New ChartArea

		'Dim FLD0 As EngineFullLoadCurve = New EngineFullLoadCurve

		If Not engine Is Nothing Then

			engine.IdleSpeed.Value()

			Dim fullLoadCurve As FullLoadCurve = EngineFullLoadCurve.Create(engine.FullLoadCurve)

			s = New Series
			s.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
								fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueFullLoad.Value()).ToArray())
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkBlue
			s.Name = "Full load"
			chart.Series.Add(s)

			s = New Series
			s.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
								fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueDrag.Value()).ToArray())
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.Blue
			s.Name = "Motoring"
			chart.Series.Add(s)

			okCount += 1

			pmax = fullLoadCurve.MaxPower.Value() / 1000 'FLD0.Pfull(FLD0.EngineRatedSpeed)


			TbEngTxt.Text = (engine.Displacement.Value() * 1000).ToString("0.0") & " l " & pmax.ToString("#") & " kW  " &
							engine.ModelName

			Dim fuelConsumptionMap As FuelConsumptionMap = FuelConsumptionMapReader.Create(engine.FuelConsumptionMap)

			s = New Series
			s.Points.DataBindXY(fuelConsumptionMap.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
								fuelConsumptionMap.Entries.Select(Function(x) x.Torque.Value()).ToArray())
			s.ChartType = SeriesChartType.Point
			s.MarkerSize = 3
			s.Color = Color.Red
			s.Name = "Map"
			chart.Series.Add(s)

			okCount += 1


		End If

		Dim gearbox As IGearboxEngineeringInputData = inputData.GearboxInputData

		If Not gearbox Is Nothing Then

			TbGbxTxt.Text = gearbox.Gears.Count & "-Speed " & gearbox.Type.ShortName() & "  " & gearbox.ModelName

			If Cfg.DeclMode Then

				If engOk Then

					For i = 1 To gearbox.Gears.Count


						'If FLD0.Init(ENG0.Nidle) Then '' use engine from below...

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
						'End If


						'	OkCount += 1

						'	pmax = FLD0.Pfull(FLD0.EngineRatedSpeed)

						'End If

					Next

				End If

			Else

				For Each gear As ITransmissionInputData In gearbox.Gears
					If gear.ShiftPolygon Is Nothing OrElse gear.ShiftPolygon.Rows.Count = 0 Then Continue For
					Dim shiftPolygon As ShiftPolygon = ShiftPolygonReader.Create(gear.ShiftPolygon)
					s = New Series
					s.Points.DataBindXY(shiftPolygon.Upshift.Select(Function(x) x.AngularSpeed),
										shiftPolygon.Upshift.Select(Function(x) x.Torque))
					s.ChartType = SeriesChartType.FastLine
					s.BorderWidth = 2
					s.Color = Color.DarkRed
					s.Name = "Upshift curve"
					' MyChart.Series.Add(s) 'MQ 2016-06-20: do not plot shift lines in engine dialog

					s = New Series
					s.Points.DataBindXY(shiftPolygon.Downshift.Select(Function(x) x.AngularSpeed),
										shiftPolygon.Downshift.Select(Function(x) x.Torque))
					s.ChartType = SeriesChartType.FastLine
					s.BorderWidth = 2
					s.Color = Color.DarkRed
					s.Name = "Downshift curve"
					'MyChart.Series.Add(s) 'MQ 2016-06-20:do not plot shift lines in engine dialog

					okCount += 1
				Next

			End If

		End If

		If okCount > 0 Then

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

			chart.ChartAreas.Add(a)

			chart.Update()

			img = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
			chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))

			PicBox.Image = img


		End If
	End Sub


#Region "Open File Context Menu"

	Private _contextMenuFiles As String()
	Private _basePath As String = ""

	Private Sub OpenFiles(ParamArray files() As String)
		If files.Length = 0 Then Exit Sub

		_contextMenuFiles = files
		OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName
		CmOpenFile.Show(Windows.Forms.Cursor.Position)
	End Sub

	Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles OpenWithToolStripMenuItem.Click
		If Not FileOpenAlt(_contextMenuFiles(0)) Then MsgBox("Failed to open file!")
	End Sub

	Private Sub ShowInFolderToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles ShowInFolderToolStripMenuItem.Click
		If File.Exists(_contextMenuFiles(0)) Then
			Try
				Process.Start("explorer", "/select,""" & _contextMenuFiles(0) & "")
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
		Dim item As AdvancedAuxiliary

		item = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary)

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
		Dim fbAux As New FileBrowser("aaux", True, False)
		Dim message As String = String.Empty
		Dim absoluteAuxPath As String
		Dim assembly As AdvancedAuxiliary

		'If Classic is selected, then bail
		If cboAdvancedAuxiliaries.SelectedIndex = 0 Then Return

		'Get Absolute Path for AAUX FILE.
		absoluteAuxPath = ResolveAAUXFilePath(GetPath(VECTOfile), txtAdvancedAuxiliaryFile.Text)

		'Set Extensions
		fbAux.Extensions = New String() {"AAUX"}

		Try

			assembly = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary)

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
					If fbAux.CustomDialog(absoluteAuxPath, False, False, FileBrowserFileExtensionMode.ForceExt, False, String.Empty) _
						Then
						txtAdvancedAuxiliaryFile.Text = GetFilenameWithoutDirectory(fbAux.Files(0), GetPath(VECTOfile))
						assembly = DirectCast(cboAdvancedAuxiliaries.SelectedItem, AdvancedAuxiliary)

						If _
							File.Exists(ResolveAAUXFilePath(GetPath(VECTOfile), txtAdvancedAuxiliaryFile.Text)) OrElse
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

		OpenFiles(FileRepl(txtAdvancedAuxiliaryFile.Text, GetPath(VECTOfile)))
	End Sub

	Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnDfTargetSpeed.Click
		If DriverDecisionFactorTargetSpeedFileBrowser.OpenDialog(FileRepl(tbLacDfTargetSpeedFile.Text, GetPath(VECTOfile))) _
			Then _
			tbLacDfTargetSpeedFile.Text = GetFilenameWithoutDirectory(DriverDecisionFactorTargetSpeedFileBrowser.Files(0),
																	GetPath(VECTOfile))
	End Sub
End Class





