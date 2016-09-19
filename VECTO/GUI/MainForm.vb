'
' This file is part of VECTO.
'
' Copyright © 2012-2016 European Union
'
' Developed by Graz University of Technology,
'              Institute of Internal Combustion Engines and Thermodynamics,
'              Institute of Technical Informatics
'
' VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
' by the European Commission - subsequent versions of the EUPL (the "Licence");
' You may not use VECTO except in compliance with the Licence.
' You may obtain a copy of the Licence at:
'
' https://joinup.ec.europa.eu/community/eupl/og_page/eupl
'
' Unless required by applicable law or agreed to in writing, VECTO
' distributed under the Licence is distributed on an "AS IS" basis,
' WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
' See the Licence for the specific language governing permissions and
' limitations under the Licence.
'
' Authors:
'   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
'   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
'   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
'   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
'   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
'   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
'

Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports System.Reflection
Imports TUGraz.VectoCore.Models.Simulation.Impl
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Threading
Imports Microsoft.VisualBasic.FileIO
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.OutputData
Imports TUGraz.VectoCore.OutputData.FileIO
Imports TUGraz.VectoCore.Utils

''' <summary>
''' Main application form. Loads at application start. Closing form ends application.
''' </summary>
''' <remarks></remarks>

Public Class MainForm
	Private JobListView As cFileListView
	Private CycleListView As cFileListView

	Private LastModeName As String
	Private ConMenTarget As ListView
	Private ConMenTarJob As Boolean

	Private GUIlocked As Boolean

	Private CheckLock As Boolean
	Private GENchecked As Integer
	Private GENcheckAllLock As Boolean

	Private CbDeclLock As Boolean = False

#Region "SLEEP Control - Prevent sleep while VECTO is running"

	Private Declare Function SetThreadExecutionState Lib "kernel32" (esFlags As Long) As Long

	Private Sub AllowSleepOFF()
#If Not PLATFORM = "x86" Then
		SetThreadExecutionState(tEXECUTION_STATE.ES_CONTINUOUS Or tEXECUTION_STATE.ES_SYSTEM_REQUIRED)
#End If
	End Sub

	Private Sub AllowSleepON()
#If Not PLATFORM = "x86" Then
		SetThreadExecutionState(tEXECUTION_STATE.ES_CONTINUOUS)
#End If
	End Sub

	Private Enum tEXECUTION_STATE As Integer
		''' Informs the system that the state being set should remain in effect until the next call that uses ES_CONTINUOUS and one of the other state flags is cleared.
		ES_CONTINUOUS = &H80000000
		''' Forces the display to be on by resetting the display idle timer.
		ES_DISPLAY_REQUIRED = &H2
		''' Forces the system to be in the working state by resetting the system idle timer.
		ES_SYSTEM_REQUIRED = &H1
	End Enum

#End Region

#Region "FileBrowser Init/Close"

	Private Sub FB_Initialize()
		FileBrowserFolderHistoryIninialized = False
		Try
			COREvers = Assembly.LoadFrom("VectoCore.dll").GetName().Version.ToString()
		Catch ex As Exception
			LogFile.WriteToLog(MessageType.Err, ex.StackTrace)
		End Try


		FolderFileBrowser = New FileBrowser("WorkDir", True)
		TextFileBrowser = New FileBrowser("FileLists")
		JobfileFileBrowser = New FileBrowser("vecto")
		VehicleFileBrowser = New FileBrowser("vveh")
		FuelConsumptionMapFileBrowser = New FileBrowser("vmap")
		DrivingCycleFileBrowser = New FileBrowser("vdri")
		FullLoadCurveFileBrowser = New FileBrowser("vfld")
		EngineFileBrowser = New FileBrowser("veng")
		GearboxFileBrowser = New FileBrowser("vgbx")
		DriverAccelerationFileBrowser = New FileBrowser("vacc")
		AuxFileBrowser = New FileBrowser("vaux")
		GearboxShiftPolygonFileBrowser = New FileBrowser("vgbs")
		RetarderLossMapFileBrowser = New FileBrowser("vrlm")
		TransmissionLossMapFileBrowser = New FileBrowser("vtlm")
		fbPTOLM = New FileBrowser("vptol")
		TorqueConverterFileBrowser = New FileBrowser("vtcc")
		fbTCCShift = New FileBrowser("vgbs")
		fbCDx = New FileBrowser("vcdx")
		DriverDecisionFactorVelocityDropFileBrowser = New FileBrowser("DfVelocityDrop")
		DriverDecisionFactorTargetSpeedFileBrowser = New FileBrowser("DfTargetSpeed")
		DriverDecisionFactorVelocityDropFileBrowser.Extensions = New String() {"csv"}
		DriverDecisionFactorTargetSpeedFileBrowser.Extensions = New String() {"csv"}

		ModalResultsFileBrowser = New FileBrowser("vmod")


		'-------------------------------------------------------
		TextFileBrowser.Extensions = New String() {"txt"}
		JobfileFileBrowser.Extensions = New String() {"vecto"}
		VehicleFileBrowser.Extensions = New String() {"vveh"}
		FuelConsumptionMapFileBrowser.Extensions = New String() {"vmap"}
		DrivingCycleFileBrowser.Extensions = New String() {"vdri"}
		FullLoadCurveFileBrowser.Extensions = New String() {"vfld"}
		EngineFileBrowser.Extensions = New String() {"veng"}
		GearboxFileBrowser.Extensions = New String() {"vgbx"}
		DriverAccelerationFileBrowser.Extensions = New String() {"vacc"}
		AuxFileBrowser.Extensions = New String() {"vaux"}
		GearboxShiftPolygonFileBrowser.Extensions = New String() {"vgbs"}
		RetarderLossMapFileBrowser.Extensions = New String() {"vrlm"}
		TransmissionLossMapFileBrowser.Extensions = New String() {"vtlm"}
		fbPTOLM.Extensions = New String() {"vptol"}
		TorqueConverterFileBrowser.Extensions = New String() {"vtcc"}
		fbTCCShift.Extensions = New String() {"vgbs"}
		fbCDx.Extensions = New String() {"vcdv", "vcdb"}

		ModalResultsFileBrowser.Extensions = New String() {"vmod"}
	End Sub

	Private Sub FB_Close()
		FolderFileBrowser.Close()
		TextFileBrowser.Close()
		JobfileFileBrowser.Close()
		VehicleFileBrowser.Close()
		FuelConsumptionMapFileBrowser.Close()
		DrivingCycleFileBrowser.Close()
		FullLoadCurveFileBrowser.Close()
		EngineFileBrowser.Close()
		GearboxFileBrowser.Close()
		DriverAccelerationFileBrowser.Close()
		AuxFileBrowser.Close()
		GearboxShiftPolygonFileBrowser.Close()
		RetarderLossMapFileBrowser.Close()
		TransmissionLossMapFileBrowser.Close()
		fbPTOLM.Close()
		TorqueConverterFileBrowser.Close()
		fbTCCShift.Close()
		fbCDx.Close()
		ModalResultsFileBrowser.Close()
	End Sub

#End Region

	'Lock certain GUI elements while VECTO is running
	Private Sub LockGUI(Lock As Boolean)
		GUIlocked = Lock

		PanelOptAllg.Enabled = Not Lock

		BtGENup.Enabled = Not Lock
		BtGENdown.Enabled = Not Lock
		ButtonGENadd.Enabled = Not Lock
		ButtonGENremove.Enabled = Not Lock
		LvGEN.LabelEdit = Not Lock
		ChBoxAllGEN.Enabled = Not Lock

		btStartV3.Enabled = Not Lock
	End Sub


#Region "Form Init/Close"

	'Initialise
	Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load
		Dim x As Integer

		GUIlocked = False
		CheckLock = False
		GENcheckAllLock = False
		GENchecked = 0


		'Load Tabs properly (otherwise problem with ListViews)
		For x = 0 To TabControl1.TabCount - 1
			TabControl1.TabPages(x).Show()
		Next

		LastModeName = ""

		FB_Initialize()

		Text = "VECTO " & VECTOvers & " / VectoCore " & COREvers


		'FileLists
		JobListView = New cFileListView(MyConfPath & "joblist.txt")
		JobListView.LVbox = LvGEN
		CycleListView = New cFileListView(MyConfPath & "cyclelist.txt")

		JobListView.LoadList()

		LoadOptions()

		'Resize columns ... after Loading the @file-lists
		LvGEN.Columns(1).Width = -2
		LvMsg.Columns(2).Width = -2

		'Initialize BackgroundWorker

		VectoWorkerV3 = New BackgroundWorker()
		AddHandler VectoWorkerV3.DoWork, AddressOf VectoWorkerV3_OnDoWork
		AddHandler VectoWorkerV3.ProgressChanged, AddressOf VectoWorkerV3_OnProgressChanged
		AddHandler VectoWorkerV3.RunWorkerCompleted, AddressOf VectoWorkerV3_OnRunWorkerCompleted

		VectoWorkerV3.WorkerReportsProgress = True
		VectoWorkerV3.WorkerSupportsCancellation = True


		'Set mode (Batch/Standard)
		ModeUpdate()

#If DEBUG Then
		Const LicCheck As Boolean = False
#Else
		Const LicCheck as Boolean = True
#End If

		'License check
		If LicCheck And Not Lic.LICcheck() Then
			MsgBox("License File invalid!" & vbCrLf & vbCrLf & Lic.FailMsg)
			If Lic.CreateActFile(MyAppPath & "ActivationCode.dat") Then
				MsgBox("Activation File created.")
			Else
				MsgBox("Failed to create Activation File! Is Directory Read-Only?")
			End If
			Close()
		Else
			GUIMsg(MessageType.Normal, "License File validated.")
			If Lic.TimeWarn Then GUIMsg(MessageType.Warn, "License expiring date (y/m/d): " & Lic.ExpTime)
		End If

		DeclOnOff()
	End Sub

	Public Shared Sub LogMethod(level As String, message As String)
		Try
			If level = "Warn" Then
				VectoWorkerV3.ReportProgress(100, New VectoProgress With {.Target = "ListBoxWarning", .Message = message})
			ElseIf level = "Error" Or level = "Fatal" Then
				VectoWorkerV3.ReportProgress(100, New VectoProgress With {.Target = "ListBoxError", .Message = message})

			End If
		Catch e As InvalidOperationException

		End Try
	End Sub

	'Declaration mode GUI settings
	Private Sub DeclOnOff()

		If Cfg.DeclMode Then
			Text = "VECTO " & COREvers & " - Declaration Mode"
			Cfg.DeclInit()
		Else
			Text = "VECTO " & COREvers
		End If

		If Cfg.DeclMode Then
			LastModeName = "Declaration"
		Else
			LastModeName = "Engineering"
		End If

		Status(LastModeName & " Mode")

		LoadOptions()

		LbDecl.Visible = Cfg.DeclMode
	End Sub

	'Shown Event (Form-Load finished) ... here StartUp Forms are loaded (DEV, GEN/ADV- Editor ..)
	Private Sub F01_MAINForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
		Dim fwelcome As WelcomeDialog

		If Cfg.FirstRun Then
			Cfg.FirstRun = False
			fwelcome = New WelcomeDialog
			fwelcome.ShowDialog()
		End If
		'End If
	End Sub

	'Open file

	'Close
	Private Sub F01_MAINForm_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

		'Save File-Lists
		SaveFileLists()

		'Close log
		LogFile.CloseLog()

		'Config save
		SetOptions()
		Cfg.Save()

		'File browser instances close
		FB_Close()
	End Sub

#End Region

	'Open file - Job, vehicle, engine, gearbox or signature file
	Public Sub OpenVectoFile(File As String)

		If Not IO.File.Exists(File) Then

			GUIMsg(MessageType.Err, "File not found! (" & File & ")")
			MsgBox("File not found! (" & File & ")", MsgBoxStyle.Critical)

		Else

			Select Case UCase(GetExtension(File))
				Case ".VGBX"
					If Not GearboxForm.Visible Then
						GearboxForm.Show()
					Else
						GearboxForm.JobDir = ""
						If GearboxForm.WindowState = FormWindowState.Minimized Then GearboxForm.WindowState = FormWindowState.Normal
						GearboxForm.BringToFront()
					End If
					GearboxForm.OpenGbx(File)
				Case ".VVEH"
					If Not VehicleForm.Visible Then
						VehicleForm.Show()
					Else
						VehicleForm.JobDir = ""
						If VehicleForm.WindowState = FormWindowState.Minimized Then VehicleForm.WindowState = FormWindowState.Normal
						VehicleForm.BringToFront()
					End If
					Try
						VehicleForm.OpenVehicle(File)
					Catch ex As Exception
						MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Vehicle File")
					End Try
				Case ".VENG"
					If Not EngineForm.Visible Then
						EngineForm.Show()
					Else
						EngineForm.JobDir = ""
						If EngineForm.WindowState = FormWindowState.Minimized Then EngineForm.WindowState = FormWindowState.Normal
						EngineForm.BringToFront()
					End If
					EngineForm.OpenEngineFile(File)
				Case ".VECTO"
					OpenVECTOeditor(File)
				Case ".VSIG"
					OpenSigFile(File)
				Case Else
					MsgBox("Type '" & GetExtension(File) & "' unknown!", MsgBoxStyle.Critical)
			End Select

		End If
	End Sub


#Region "Events"

	Private Sub ButtonGENremove_Click(sender As Object, e As EventArgs) _
		Handles ButtonGENremove.Click
		RemoveJobFile()
	End Sub

	Private Sub ButtonGENadd_Click(sender As Object, e As EventArgs) _
		Handles ButtonGENadd.Click
		AddJobFile()
	End Sub

	Private Sub ListViewGEN_KeyDown(sender As Object, e As KeyEventArgs) _
		Handles LvGEN.KeyDown
		Select Case e.KeyCode
			Case Keys.Delete, Keys.Back
				If Not GUIlocked Then RemoveJobFile()
			Case Keys.Enter
				OpenJobFile()
		End Select
	End Sub

	Private Sub ListViewGEN_DoubleClick(sender As Object, e As EventArgs) Handles LvGEN.DoubleClick
		If LvGEN.SelectedItems.Count > 0 Then
			LvGEN.SelectedItems(0).Checked = Not LvGEN.SelectedItems(0).Checked
			OpenJobFile()
		End If
	End Sub

	Private Sub LvGEN_ItemChecked(sender As Object, e As ItemCheckedEventArgs) _
		Handles LvGEN.ItemChecked

		If e.Item.Checked Then
			GENchecked += 1
		Else
			GENchecked -= 1
		End If

		If CheckLock Then Exit Sub
		UpdateJobTabText()
	End Sub

	Private Sub ChBoxAllGEN_CheckedChanged(sender As Object, e As EventArgs) _
		Handles ChBoxAllGEN.CheckedChanged

		If GENcheckAllLock And ChBoxAllGEN.CheckState = CheckState.Indeterminate Then Exit Sub

		CheckAllGEN(ChBoxAllGEN.Checked)
	End Sub

	Private Sub CheckAllGEN(Check As Boolean)
		Dim x As ListViewItem

		CheckLock = True
		LvGEN.BeginUpdate()

		For Each x In LvGEN.Items
			x.Checked = Check
		Next

		LvGEN.EndUpdate()
		CheckLock = False

		GENchecked = LvGEN.CheckedItems.Count
		UpdateJobTabText()
	End Sub

	Private Sub ListGEN_DragEnter(sender As Object, e As DragEventArgs) _
		Handles LvGEN.DragEnter
		If (e.Data.GetDataPresent(DataFormats.FileDrop)) Then
			e.Effect = DragDropEffects.Copy
		End If
	End Sub

	Private Sub ListGEN_DragDrop(sender As Object, e As DragEventArgs) _
		Handles LvGEN.DragDrop
		Dim f As String()
		f = CType(e.Data.GetData(DataFormats.FileDrop), String())
		AddToJobListView(f)
	End Sub

	Private Sub BtGENup_Click(sender As Object, e As EventArgs) Handles BtGENup.Click
		MoveItem(LvGEN, True)
	End Sub

	Private Sub BtGENdown_Click(sender As Object, e As EventArgs) Handles BtGENdown.Click
		MoveItem(LvGEN, False)
	End Sub

#End Region

	'Remove selected file(s) from job list
	Private Sub RemoveJobFile()
		Dim lastindx As Integer
		Dim SelIx() As Integer
		Dim i As Integer

		If LvGEN.SelectedItems.Count < 1 Then
			If LvGEN.Items.Count = 1 Then
				LvGEN.Items(0).Selected = True
			Else
				Exit Sub
			End If
		End If

		LvGEN.BeginUpdate()
		CheckLock = True

		ReDim SelIx(LvGEN.SelectedItems.Count - 1)
		LvGEN.SelectedIndices.CopyTo(SelIx, 0)

		lastindx = LvGEN.SelectedIndices(LvGEN.SelectedItems.Count - 1)

		For i = UBound(SelIx) To 0 Step -1
			LvGEN.Items.RemoveAt(SelIx(i))
		Next

		If lastindx < LvGEN.Items.Count Then
			LvGEN.Items(lastindx).Selected = True
		Else
			If LvGEN.Items.Count > 0 Then LvGEN.Items(LvGEN.Items.Count - 1).Selected = True
		End If

		LvGEN.EndUpdate()
		CheckLock = False

		GENchecked = LvGEN.CheckedItems.Count
		UpdateJobTabText()
	End Sub

	'Browse for job file(s) and add to job list with AddToJobListView
	Private Sub AddJobFile()
		Dim x As String()
		Dim Chck As Boolean = False

		x = New String() {""}

		'STANDARD/BATCH
		If JobfileFileBrowser.OpenDialog("", True, "vecto") Then
			Chck = True
			x = JobfileFileBrowser.Files
		End If

		If Chck Then AddToJobListView(x)
	End Sub

	'Open file in list
	Private Sub OpenJobFile()
		Dim f As String

		If LvGEN.SelectedItems.Count < 1 Then
			If LvGEN.Items.Count = 1 Then
				LvGEN.Items(0).Selected = True
			Else
				Exit Sub
			End If
		End If

		f = LvGEN.SelectedItems(0).SubItems(0).Text
		f = fFileRepl(f)
		If Not File.Exists(f) Then
			MsgBox(f & " not found!")
		Else
			OpenVECTOeditor(f)
		End If
	End Sub

	'Add File to job listview (multiple files)
	Private Sub AddToJobListView(Path As String(), Optional ByVal Txt As String = " ")
		Dim pDim As Integer
		Dim p As Integer
		Dim f As Integer
		Dim fList As String()
		Dim fListDim As Integer = -1
		Dim listViewItem As ListViewItem

		'If VECTO runs: Cancel operation (because Mode-change during calculation is not very clever)
		If VectoWorkerV3.IsBusy Then Exit Sub

		pDim = UBound(Path)
		ReDim fList(0)	   'um Nullverweisausnahme-Warnung zu verhindern

		'******************************************* Begin Update '*******************************************
		LvGEN.BeginUpdate()
		CheckLock = True

		LvGEN.SelectedIndices.Clear()

		If pDim = 0 Then
			fListDim = LvGEN.Items.Count - 1
			ReDim fList(fListDim)
			For f = 0 To fListDim
				fList(f) = fFileRepl(LvGEN.Items(f).SubItems(0).Text)
			Next
		End If

		For p = 0 To pDim

			If pDim = 0 Then

				For f = 0 To fListDim

					'If file already exists in the list: Do not append (only when a single file)
					If UCase(Path(p)) = UCase(fList(f)) Then

						'Status reset
						LvGEN.Items(f).SubItems(1).Text = Txt
						LvGEN.Items(f).BackColor = Color.FromKnownColor(KnownColor.Window)
						LvGEN.Items(f).ForeColor = Color.FromKnownColor(KnownColor.WindowText)

						'Element auswählen und anhaken |@@| Element selection and hook
						LvGEN.Items(f).Selected = True
						LvGEN.Items(f).Checked = True
						LvGEN.Items(f).EnsureVisible()

						GoTo lbFound
					End If
				Next

			End If

			'Otherwise: Add File (without WorkDir)
			listViewItem = New ListViewItem(Path(p))	'fFileWD(Path(p)))
			listViewItem.SubItems.Add(" ")
			listViewItem.Checked = True
			listViewItem.Selected = True
			LvGEN.Items.Add(listViewItem)
			listViewItem.EnsureVisible()
lbFound:
		Next

		LvGEN.EndUpdate()
		CheckLock = False
		'******************************************* End Update '*******************************************

		'Number update
		GENchecked = LvGEN.CheckedItems.Count
		UpdateJobTabText()
	End Sub

	'Add File to job listview (single file)
	Public Sub AddToJobListView(Path As String, Optional ByVal Txt As String = " ")
		Dim p(0) As String
		p(0) = Path
		AddToJobListView(p, Txt)
	End Sub

	'Update job files counter in tab titel
	Private Sub UpdateJobTabText()
		Dim c As Integer
		c = LvGEN.Items.Count

		TabPageGEN.Text = "Job Files ( " & GENchecked & " / " & c & " )"

		GENcheckAllLock = True

		If GENchecked = 0 Then
			ChBoxAllGEN.CheckState = CheckState.Unchecked
		ElseIf GENchecked = c Then
			ChBoxAllGEN.CheckState = CheckState.Checked
		Else
			ChBoxAllGEN.CheckState = CheckState.Indeterminate
		End If

		GENcheckAllLock = False
	End Sub


#Region "Toolstrip"

	'New Job file
	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		OpenVECTOeditor("<New>")
	End Sub

	'Open input file
	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click

		If JobfileFileBrowser.OpenDialog("", False, "vecto,vveh,vgbx,veng,vsig") Then
			OpenVectoFile(JobfileFileBrowser.Files(0))
		End If
	End Sub

	Private Sub GENEditorToolStripMenuItem1_Click(sender As Object, e As EventArgs) _
		Handles GENEditorToolStripMenuItem1.Click
		OpenVECTOeditor("<New>")
	End Sub

	Private Sub VEHEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles VEHEditorToolStripMenuItem.Click
		If Not VehicleForm.Visible Then
			VehicleForm.Show()
		Else
			If VehicleForm.WindowState = FormWindowState.Minimized Then VehicleForm.WindowState = FormWindowState.Normal
			VehicleForm.BringToFront()
		End If
	End Sub

	Private Sub EngineEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles EngineEditorToolStripMenuItem.Click
		If Not EngineForm.Visible Then
			EngineForm.Show()
		Else
			If EngineForm.WindowState = FormWindowState.Minimized Then EngineForm.WindowState = FormWindowState.Normal
			EngineForm.BringToFront()
		End If
	End Sub

	Private Sub GearboxEditorToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles GearboxEditorToolStripMenuItem.Click
		If Not GearboxForm.Visible Then
			GearboxForm.Show()
		Else
			If GearboxForm.WindowState = FormWindowState.Minimized Then GearboxForm.WindowState = FormWindowState.Normal
			GearboxForm.BringToFront()
		End If
	End Sub

	Private Sub GraphToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles GraphToolStripMenuItem.Click
		Dim FGraph As New GraphForm
		FGraph.Show()
	End Sub

	Private Sub SignOrVerifyFilesToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles SignOrVerifyFilesToolStripMenuItem.Click
		If Not FileSignDialog.Visible Then
			FileSignDialog.Show()
		Else
			If FileSignDialog.WindowState = FormWindowState.Minimized Then FileSignDialog.WindowState = FormWindowState.Normal
			FileSignDialog.BringToFront()
		End If
	End Sub

	Private Sub OpenLogToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles OpenLogToolStripMenuItem.Click
		Process.Start(MyAppPath & "log.txt")
	End Sub

	Private Sub SettingsToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles SettingsToolStripMenuItem.Click
		Settings.ShowDialog()
	End Sub

	Private Sub UserManualToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles UserManualToolStripMenuItem.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim BrowserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim DefaultBrowserPath As String =
					Regex.Match(BrowserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(DefaultBrowserPath, Uri.EscapeDataString(MyAppPath & "User Manual\help.html"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

	Private Sub UpdateNotesToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles UpdateNotesToolStripMenuItem.Click
		If File.Exists(MyAppPath & "User Manual\Release Notes.pdf") Then
			Process.Start(MyAppPath & "User Manual\Release Notes.pdf")
		Else
			MsgBox("Release Notes not found!", MsgBoxStyle.Critical)
		End If
	End Sub

	Private Sub ReportBugViaCITnetToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles ReportBugViaCITnetToolStripMenuItem.Click
		JiraDialog.ShowDialog()
	End Sub

	Private Sub CreateActivationFileToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles CreateActivationFileToolStripMenuItem.Click
		If MsgBox("Create Activation File ?", MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
			If Lic.CreateActFile(MyAppPath & "ActivationCode.dat") Then
				GUIMsg(MessageType.Normal, "Activation File created.")
			Else
				GUIMsg(MessageType.Err, "Failed to create Activation File!")
				MsgBox("ERROR! Failed to create Activation File!", MsgBoxStyle.Critical)
			End If
		End If
	End Sub

	Private Sub AboutVECTOToolStripMenuItem1_Click(sender As Object, e As EventArgs) _
		Handles AboutVECTOToolStripMenuItem1.Click
		AboutBox.ShowDialog()
	End Sub


#End Region

	'Move job/cycle file up or down in list view
	Private Sub MoveItem(ByRef ListV As ListView, MoveUp As Boolean)
		Dim x As Int32
		Dim y As Int32
		Dim y1 As Int32
		Dim items() As String
		Dim check() As Boolean
		Dim index() As Integer
		Dim ListViewItem0 As ListViewItem

		If GUIlocked Then Exit Sub

		'Cache Selected Items
		y1 = ListV.SelectedItems.Count - 1
		ReDim items(y1)
		ReDim check(y1)
		ReDim index(y1)
		y = 0
		For Each x In ListV.SelectedIndices
			items(y) = ListV.Items(x).SubItems(0).Text
			check(y) = ListV.Items(x).Checked
			If MoveUp Then
				If x = 0 Then Exit Sub
				index(y) = x - 1
			Else
				If x = ListV.Items.Count - 1 Then Exit Sub
				index(y) = x + 1
			End If
			y += 1
		Next

		ListV.BeginUpdate()

		'Delete Selected Items
		For Each ListViewItem0 In ListV.SelectedItems
			ListViewItem0.Remove()
		Next

		'Items select and Insert
		For y = 0 To y1
			If Not check(y) Then GENchecked += 1
			ListViewItem0 = ListV.Items.Insert(index(y), items(y))
			ListViewItem0.SubItems.Add(" ")
			ListViewItem0.Checked = check(y)
			ListV.SelectedIndices.Add(index(y))
		Next

		ListV.EndUpdate()
	End Sub


#Region "job/cycle file List - Context Menu"

	'Save List
	Private Sub SaveListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles SaveListToolStripMenuItem.Click
		If TextFileBrowser.SaveDialog("") Then
			If ConMenTarJob Then
				JobListView.SaveList(TextFileBrowser.Files(0))
			Else
				CycleListView.SaveList(TextFileBrowser.Files(0))
			End If
		End If
	End Sub

	'Load List
	Private Sub LoadListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles LoadListToolStripMenuItem.Click

		If GUIlocked Then Exit Sub

		If TextFileBrowser.OpenDialog("") Then

			If ConMenTarJob Then 'GEN
				JobListView.LoadList(TextFileBrowser.Files(0))
				GENchecked = LvGEN.CheckedItems.Count
				UpdateJobTabText()
			Else 'DRI
				'Mode toggle 
				CycleListView.LoadList(TextFileBrowser.Files(0))
			End If

		End If
	End Sub

	'Load Default List
	Private Sub LoadDefaultListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles LoadDefaultListToolStripMenuItem.Click

		If GUIlocked Then Exit Sub

		If ConMenTarJob Then

			JobListView.LoadList()

			GENchecked = LvGEN.CheckedItems.Count
			UpdateJobTabText()
		Else
			CycleListView.LoadList()

		End If
	End Sub

	'Clear List
	Private Sub ClearListToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles ClearListToolStripMenuItem.Click

		If GUIlocked Then Exit Sub

		ConMenTarget.Items.Clear()
		If ConMenTarJob Then
			GENchecked = LvGEN.CheckedItems.Count
			UpdateJobTabText()
		End If
	End Sub

#End Region

	'VECTO Start button - Calls VECTO_Launcher or aborts calculation

	Private Sub btStartV3_Click(sender As Object, e As EventArgs) Handles btStartV3.Click
		If Not VectoWorkerV3.IsBusy Then
			'Save Lists for Crash
			SaveFileLists()

			LvGEN.SelectedItems.Clear()

			If LvGEN.CheckedItems.Count = 0 Then
				GUIMsg(MessageType.Err, "No job file selected!")
				Exit Sub
			End If

			Status("Launching VECTO ...")
			JobFileList.Clear()
			JobFileList.AddRange(
				From listViewItem As ListViewItem In LvGEN.CheckedItems.Cast(Of ListViewItem)()
									Select fFileRepl(listViewItem.SubItems(0).Text))

			SetOptions()
			Cfg.Save()
			ClearMsg()

			LockGUI(True)
			btStartV3.Enabled = True
			btStartV3.Text = "STOP"
			btStartV3.Image = My.Resources.Stop_icon

			ToolStripProgBarOverall.Value = 0
			ToolStripProgBarOverall.Style = ProgressBarStyle.Continuous
			ToolStripProgBarOverall.Visible = True

			VectoWorkerV3.RunWorkerAsync()
		Else
			btStartV3.Enabled = False
			btStartV3.Text = "Aborting..."
			btStartV3.Image = My.Resources.Play_icon_gray
			VectoWorkerV3.CancelAsync()
		End If
	End Sub


	Private Sub VectoWorkerV3_OnDoWork(theSender As Object, e As DoWorkEventArgs)
		Dim sender As BackgroundWorker = TryCast(theSender, BackgroundWorker)
		If sender Is Nothing Then Exit Sub

		AllowSleepOFF()

		Dim sumFileWriter As FileOutputWriter = New FileOutputWriter(JobFileList(0))
		Dim sumWriter As SummaryDataContainer = New SummaryDataContainer(sumFileWriter)
		Dim jobContainer As JobContainer = New JobContainer(sumWriter)

		Dim mode As ExecutionMode
		If Cfg.DeclMode Then
			mode = ExecutionMode.Declaration
		Else
			mode = ExecutionMode.Engineering
			Physics.FuelDensity = New SI(Cfg.FuelDens).Kilo.Gramm.Per.Cubic.Dezi.Meter.Cast(Of KilogramPerCubicMeter)()
			Physics.AirDensity = New SI(Cfg.AirDensity).Kilo.Gramm.Per.Cubic.Meter.Cast(Of KilogramPerCubicMeter)()
			Physics.CO2PerFuelWeight = Cfg.CO2perFC
		End If

		'dictionary of run-identifiers to fileWriters (used for output directory of modfile)
		Dim fileWriters As Dictionary(Of Integer, FileOutputWriter) = New Dictionary(Of Integer, FileOutputWriter)

		'list of finished runs
		Dim finishedRuns As List(Of Integer) = New List(Of Integer)

		For Each jobFile As String In JobFileList
			Try
				sender.ReportProgress(0,
									New VectoProgress With {.Target = "ListBox", .Message = "Reading File " + jobFile, .Link = jobFile})

				Dim dataProvider As IInputDataProvider = JSONInputDataFactory.ReadJsonJob(jobFile)
				Dim fileWriter As FileOutputWriter = New FileOutputWriter(jobFile)

				Dim runsFactory As SimulatorFactory = New SimulatorFactory(mode, dataProvider, fileWriter)
				runsFactory.WriteModalResults = Cfg.ModOut
				runsFactory.ModalResults1Hz = Cfg.Mod1Hz

				For Each runId As Integer In jobContainer.AddRuns(runsFactory)
					fileWriters.Add(runId, fileWriter)
				Next

				sender.ReportProgress(0,
									New VectoProgress With {.Target = "ListBox", .Message = "Finished Reading Data for job: " + jobFile})

			Catch ex As Exception
				MsgBox(String.Format("ERROR running job {0}: {1}", jobFile, ex.Message), MsgBoxStyle.Critical)
				sender.ReportProgress(0, New VectoProgress With {.Target = "ListBoxError", .Message = ex.Message})
				Return
			End Try
		Next

		'print detected cycles
		For Each cycle As JobContainer.CycleTypeDescription In jobContainer.GetCycleTypes()
			sender.ReportProgress(0,
								New VectoProgress _
									With {.Target = "ListBox", .Message = String.Format("Detected Cycle {0}: {1}", cycle.Name, cycle.CycleType)})
		Next

		sender.ReportProgress(0, New VectoProgress With {.Target = "ListBox",
								.Message = _
								String.Format("Starting Simulation ({0} Jobs, {1} Runs)", JobFileList.Count, jobContainer.GetProgress().Count)})

		jobContainer.Execute(True)

		Dim start As DateTime = DateTime.Now()

		While Not jobContainer.AllCompleted
			'cancel the job if thread is interrupted (button "Stop" clicked)
			If sender.CancellationPending Then
				jobContainer.Cancel()
				Return
			End If

			Dim progress As Dictionary(Of Integer, JobContainer.ProgressEntry) = jobContainer.GetProgress()
			Dim sumProgress As Double = progress.Sum(Function(pair) pair.Value.Progress)
			Dim duration As Double = (DateTime.Now() - start).TotalSeconds

			sender.ReportProgress(Convert.ToInt32((sumProgress * 100.0) / progress.Count),
								New VectoProgress With {.Target = "Status",
									.Message = _
									String.Format("Duration: {0:0}s, Current Progress: {1:P} ({2})", duration, sumProgress / progress.Count,
												String.Join(", ", progress.Select(Function(pair) String.Format("{0,4:P}", pair.Value.Progress))))})

			Dim justFinished As Dictionary(Of Integer, JobContainer.ProgressEntry) =
					progress.Where(Function(proc) proc.Value.Done AndAlso Not finishedRuns.Contains(proc.Key)).ToDictionary(
						Function(pair) pair.Key, Function(pair) pair.Value)
			PrintRuns(justFinished, fileWriters)
			finishedRuns.AddRange(justFinished.Select(Function(pair) pair.Key))
			Thread.Sleep(100)
		End While

		Dim remainingRuns As Dictionary(Of Integer, JobContainer.ProgressEntry) =
				jobContainer.GetProgress().Where(Function(proc) proc.Value.Done AndAlso Not finishedRuns.Contains(proc.Key)).
				ToDictionary(Function(pair) pair.Key, Function(pair) pair.Value)
		PrintRuns(remainingRuns, fileWriters)

		finishedRuns.Clear()
		fileWriters.Clear()

		For Each progressEntry As KeyValuePair(Of Integer, JobContainer.ProgressEntry) In jobContainer.GetProgress()
			sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
									.Message = String.Format("{0,-60} {1,8:P} {2,10:F2}s - {3}",
															String.Format("{0} {1} {2}", progressEntry.Value.RunName, progressEntry.Value.CycleName,
																		progressEntry.Value.RunSuffix),
															progressEntry.Value.Progress, progressEntry.Value.ExecTime / 1000.0,
															IIf(progressEntry.Value.Success, "Success", "Aborted"))})
			If (Not progressEntry.Value.Success) Then
				sender.ReportProgress(100,
									New VectoProgress With {.Target = "ListBox", .Message = progressEntry.Value.Error.Message})
			End If

		Next

		For Each job As String In JobFileList
			Dim report As String = New FileOutputWriter(job).PDFReportName
			If File.Exists(report) Then
				sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
										.Message = String.Format("PDF-Report for '{0}' written to {1}", Path.GetFileName(job), report),
										.Link = "<RUN>" + report})
			End If
		Next

		If File.Exists(sumFileWriter.SumFileName) Then
			sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
									.Message = String.Format("Sum File written to {0}", sumFileWriter.SumFileName),
									.Link = sumFileWriter.SumFileName})
		End If

		sender.ReportProgress(100, New VectoProgress With {.Target = "ListBox",
								.Message = String.Format("Simulation Finished in {0:0}s", (DateTime.Now() - start).TotalSeconds)})
	End Sub


	Private Shared Sub PrintRuns(progress As Dictionary(Of Integer, JobContainer.ProgressEntry),
								fileWriters As Dictionary(Of Integer, FileOutputWriter))
		For Each p As KeyValuePair(Of Integer, JobContainer.ProgressEntry) In progress
			Dim modFilename As String = fileWriters(p.Key).GetModDataFileName(p.Value.RunName, p.Value.CycleName,
																			p.Value.RunSuffix + If(Cfg.Mod1Hz, "_1Hz", ""))

			Dim runName As String = String.Format("{0} {1} {2}", p.Value.RunName, p.Value.CycleName, p.Value.RunSuffix)

			If Not p.Value.Error Is Nothing Then
				VectoWorkerV3.ReportProgress(0, New VectoProgress With {.Target = "ListBoxError",
												.Message = String.Format("Finished Run {0} with ERROR: {1}", runName, p.Value.Error.Message),
												.Link = modFilename})
			Else
				VectoWorkerV3.ReportProgress(0,
											New VectoProgress _
												With {.Target = "ListBox", .Message = String.Format("Finished Run {0} successfully.", runName)})
			End If

			If (File.Exists(modFilename)) Then
				VectoWorkerV3.ReportProgress(0, New VectoProgress With {.Target = "ListBox",
												.Message = String.Format("Run {0}: Modal Results written to {1}", runName, modFilename), .Link = modFilename
												})
			End If
		Next
	End Sub

	Private Sub VectoWorkerV3_OnProgressChanged(sender As Object, e As ProgressChangedEventArgs)
		Dim progress As VectoProgress = TryCast(e.UserState, VectoProgress)
		If progress Is Nothing Then Exit Sub

		Select Case progress.Target
			Case "ListBox"
				If progress.Link Is Nothing Then
					MSGtoForm(MessageType.Normal, progress.Message, "", "")
				Else
					MSGtoForm(MessageType.Normal, progress.Message, "", progress.Link)
				End If
			Case "ListBoxWarning"
				MSGtoForm(MessageType.Warn, progress.Message, "", "")
				Return
			Case "ListBoxError"
				MSGtoForm(MessageType.Err, progress.Message, "", "")
				Return
			Case "Status"
				Status(progress.Message)
		End Select

		ToolStripProgBarOverall.Value = e.ProgressPercentage
	End Sub

	Private Sub VectoWorkerV3_OnRunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs)

		'Progbar reset
		ToolStripProgBarOverall.Visible = False
		ToolStripProgBarOverall.Style = ProgressBarStyle.Continuous
		ToolStripProgBarOverall.Value = 0
		ProgSecStop()

		LvGEN.SelectedIndices.Clear()

		'ShutDown when Unexpected Error
		If e.Error IsNot Nothing Then
			MsgBox("An Unexpected Error occurred!" & ChrW(10) & ChrW(10) &
					e.Error.Message.ToString, MsgBoxStyle.Critical, "Unexpected Error")
			LogFile.WriteToLog(MessageType.Err, ">>>Unexpected Error:" & e.Error.ToString())
		End If

		'Options enable / GUI reset
		LockGUI(False)
		btStartV3.Text = "START"
		btStartV3.Image = My.Resources.Play_icon
		Status(LastModeName & " Mode")

		'SLEEP reactivate
		AllowSleepON()
	End Sub


	Private Sub ModeUpdate()

		'Save lists
		JobListView.SaveList()

		'GUI changes according to current mode

		If Cfg.DeclMode Then
			LastModeName = "Declaration"
		Else
			LastModeName = "Engineering"
		End If

		'Update job counter
		GENchecked = LvGEN.CheckedItems.Count
		UpdateJobTabText()

		'Status label
		Status(LastModeName & " Mode")
	End Sub

	'Class for ListView control - Job and cycle lists
	Private Class cFileListView
		Private ReadOnly FilePath As String
		Private LoadedDefault As Boolean
		Public LVbox As ListView

		Public Sub New(Path As String)
			FilePath = Path
			LoadedDefault = False
		End Sub

		Public Sub SaveList(Optional ByVal Path As String = "")
			Dim x As Int32
			If Path = "" Then
				If Not LoadedDefault Then Exit Sub
				Path = FilePath
			End If
			Dim file As StreamWriter = My.Computer.FileSystem.OpenTextFileWriter(Path, False, Encoding.UTF8)
			For x = 1 To LVbox.Items.Count
				file.WriteLine(String.Join("?", LVbox.Items(x - 1).SubItems(0).Text, Math.Abs(CInt(LVbox.Items(x - 1).Checked))))
			Next
			file.Close()
		End Sub

		Public Sub LoadList(Optional ByVal Path As String = "")
			'Dim line As String()
			Dim NoCheck As Boolean
			'Dim file As CsvFile
			Dim ListViewItem0 As ListViewItem

			If Path = "" Then
				Path = FilePath
				LoadedDefault = True
			End If

			'file = New CsvFile

			If Not File.Exists(Path) Then
				If Not LoadedDefault Then GUIMsg(MessageType.Err, "Cannot open file (" & Path & ")!")
				Exit Sub
			End If

			MainForm.CheckLock = True
			LVbox.BeginUpdate()

			LVbox.Items.Clear()

			NoCheck = False
			Dim reader As TextFieldParser = New TextFieldParser(Path, Encoding.Default)
			reader.TextFieldType = FieldType.Delimited
			reader.Delimiters = New String() {"?"}

			Do While Not reader.EndOfData
				Dim line As String() = reader.ReadFields()
				If Strings.Left(Trim(line(0)), 1) = "#" Then Continue Do

				ListViewItem0 = New ListViewItem(line(0))
				ListViewItem0.SubItems.Add(" ")

				If NoCheck Then
					ListViewItem0.Checked = True
				Else
					If UBound(line) < 1 Then
						NoCheck = True
						ListViewItem0.Checked = True
					Else
						If IsNumeric(line(1)) Then
							ListViewItem0.Checked = CBool(line(1))
						Else
							ListViewItem0.Checked = True
						End If
					End If
				End If
				LVbox.Items.Add(ListViewItem0)
			Loop

			reader.Close()

			LVbox.EndUpdate()
			MainForm.CheckLock = False

			If LVbox.Items.Count > 0 Then LVbox.Items(LVbox.Items.Count - 1).EnsureVisible()
		End Sub
	End Class


	'Open Job Editor and open file (or new file)
	Friend Sub OpenVECTOeditor(x As String)

		If Not VectoJobForm.Visible Then
			VectoJobForm.Show()
		Else
			If VectoJobForm.WindowState = FormWindowState.Minimized Then VectoJobForm.WindowState = FormWindowState.Normal
			VectoJobForm.BringToFront()
		End If

		If x = "<New>" Then
			VectoJobForm.VECTOnew()
		Else
			VectoJobForm.VECTOload2Form(x)
		End If

		VectoJobForm.Activate()
	End Sub

	'Open signature file (.vsig)
	Friend Sub OpenSigFile(file As String)
		If Not FileSignDialog.Visible Then
			FileSignDialog.Show()

		End If
		FileSignDialog.WindowState = FormWindowState.Normal
		FileSignDialog.TbSigFile.Text = file
		FileSignDialog.VerifySigFile()
		FileSignDialog.Activate()
	End Sub

	'Save job and cycle file lists
	Private Sub SaveFileLists()
		JobListView.SaveList()
		'If Cfg.BatchMode Then CycleListView.SaveList()
	End Sub


#Region "Progressbar controls"

	'Initialise progress bar (Start of next job in calculation)

	'Stop - Hide progress bar
	Private Sub ProgSecStop()
		TmProgSec.Stop()
		ToolStripProgBarJob.Visible = False
		ToolStripProgBarJob.Value = 0
	End Sub

	'Timer to update progress bar regularly
	Private Sub TmProgSec_Tick(sender As Object, e As EventArgs) Handles TmProgSec.Tick
		If GUItest0.TestActive Then
			Call GUItest0.TestTick()
			Exit Sub
		Else
			If Not ProgBarCtrl.ProgLock Then ProgSecUpdate()
		End If
	End Sub

	'Update progress bar (timer controlled)
	Private Sub ProgSecUpdate()

		With ProgBarCtrl

			If .ProgJobInt > 0 AndAlso ToolStripProgBarJob.Style = ProgressBarStyle.Marquee Then
				ToolStripProgBarJob.Style = ProgressBarStyle.Continuous
			End If

			If .ProgJobInt < 0 Then
				.ProgJobInt = 0
			ElseIf .ProgJobInt > 100 Then
				.ProgJobInt = 100
			End If

			ToolStripProgBarJob.Value = .ProgJobInt

			If .ProgOverallStartInt > -1 Then
				ToolStripProgBarOverall.Value =
					CInt(.ProgOverallStartInt + (.PgroOverallEndInt - .ProgOverallStartInt) * .ProgJobInt / 100)
			End If

		End With
	End Sub


#End Region

#Region "Options Tab"

	'Load options from config class
	Public Sub LoadOptions()
		ChBoxModOut.Checked = Cfg.ModOut
		ChBoxMod1Hz.Checked = Cfg.Mod1Hz

		RbDecl.Checked = Cfg.DeclMode
	End Sub

	'Update config class from options in GUI, e.g. before running calculations 
	Private Sub SetOptions()
		Cfg.ModOut = ChBoxModOut.Checked
		Cfg.Mod1Hz = ChBoxMod1Hz.Checked
	End Sub

#End Region


	'Add message to message list
	Public Sub MSGtoForm(ID As MessageType, Msg As String, Source As String, Link As String)

		If (InvokeRequired) Then
			'Me.Invoke(New MsgToFormDelegate(AddressOf MSGtoForm), ID, Msg, Source, Link)
			Exit Sub
		End If
		Dim lv0 As ListViewItem

		lv0 = New ListViewItem
		lv0.Text = Msg
		lv0.SubItems.Add(Now.ToString("HH:mm:ss.ff"))
		lv0.SubItems.Add(Source)

		If LvMsg.Items.Count > 9999 Then LvMsg.Items.RemoveAt(0)

		LogFile.WriteToLog(ID, Msg & vbTab & Source)

		Select Case ID

			Case MessageType.Err

				lv0.BackColor = Color.Red
				lv0.ForeColor = Color.White

			Case MessageType.Warn

				lv0.BackColor = Color.Khaki				 'FromArgb(218, 125, 0) 'DarkOrange
				lv0.ForeColor = Color.Black

			Case Else

				If ID = MessageType.NewJob Then
					lv0.BackColor = Color.LightGray
					lv0.ForeColor = Color.DarkBlue
				End If

		End Select

		If Link <> "" Then
			If Not ID = MessageType.Err Then lv0.ForeColor = Color.Blue
			lv0.SubItems(0).Font = New Font(LvMsg.Font, FontStyle.Underline)
			lv0.Tag = Link
		End If


		LvMsg.Items.Add(lv0)

		lv0.EnsureVisible()
	End Sub


	'Open link in message list
	Private Sub LvMsg_MouseClick(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseClick
		Dim txt As String
		If LvMsg.SelectedIndices.Count > 0 Then
			If Not LvMsg.SelectedItems(0).Tag Is Nothing Then
				If _
					Len(CStr(LvMsg.SelectedItems(0).Tag)) > 4 AndAlso
					Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 4) = "<UM>" Then
					txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<UM>", MyAppPath & "User Manual")
					txt = txt.Replace(" ", "%20")
					txt = txt.Replace("\", "/")
					txt = "file:///" & txt
					Try
						Process.Start(txt)
					Catch ex As Exception
						MsgBox("Cannot open link! (-_-;)")
					End Try
				ElseIf _
					Len(CStr(LvMsg.SelectedItems(0).Tag)) > 5 AndAlso
					Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 5) = "<GUI>" Then
					txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<GUI>", "")
					OpenVectoFile(txt)
				ElseIf _
					Len(CStr(LvMsg.SelectedItems(0).Tag)) > 5 AndAlso
					Microsoft.VisualBasic.Left(CStr(LvMsg.SelectedItems(0).Tag), 5) = "<RUN>" Then
					txt = CStr(LvMsg.SelectedItems(0).Tag).Replace("<RUN>", "")
					Try
						Process.Start(txt)
					Catch ex As Exception
						GUIMsg(MessageType.Err, "Could not run '" & txt & "'!")
					End Try
				Else
					OpenFiles(CStr(LvMsg.SelectedItems(0).Tag))
				End If
			End If
		End If
	End Sub

	'Link-cursor (Hand) for links
	Private Sub LvMsg_MouseMove(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseMove
		Dim lv0 As ListViewItem
		lv0 = LvMsg.GetItemAt(e.Location.X, e.Location.Y)
		If lv0 Is Nothing OrElse lv0.Tag Is Nothing Then
			LvMsg.Cursor = Cursors.Arrow
		Else
			LvMsg.Cursor = Cursors.Hand
		End If
		If mouseDownOnListView Then
			Try
				LvMsg.HitTest(e.Location).Item.Selected = True
			Catch
			End Try
		End If
	End Sub

#Region "Open File Context Menu"

	Private CmFiles As String()

	'Initialise and open context menu
	Private Sub OpenFiles(ParamArray files() As String)

		If files.Length = 0 Then Exit Sub

		CmFiles = files

		OpenInGraphWindowToolStripMenuItem.Enabled = (UCase(GetExtension(CmFiles(0))) = ".VMOD")


		OpenWithToolStripMenuItem.Text = "Open with " & Cfg.OpenCmdName

		CmOpenFile.Show(Cursor.Position)
	End Sub

	'Open with tool defined in Settings
	Private Sub OpenWithToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles OpenWithToolStripMenuItem.Click
		If Not FileOpenAlt(CmFiles(0)) Then MsgBox("Failed to open file!")
	End Sub

	Private Sub OpenInGraphWindowToolStripMenuItem_Click(sender As Object, e As EventArgs) _
		Handles OpenInGraphWindowToolStripMenuItem.Click
		Dim FGraph As New GraphForm
		FGraph.Show()
		FGraph.LoadNewFile(CmFiles(0))
	End Sub

	'Show in folder
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

	'Change Declaraion Mode
	Private Sub RbDecl_CheckedChanged(sender As Object, e As EventArgs) Handles RbDecl.CheckedChanged
		If CbDeclLock Then Exit Sub

		If VectoJobForm.Visible Or VehicleForm.Visible Or GearboxForm.Visible Or EngineForm.Visible Then
			CbDeclLock = True
			RbDecl.Checked = Not RbDecl.Checked
			CbDeclLock = False
			MsgBox("Please close all dialog windows (e.g. Job Editor) before changing mode!")
		Else
			Cfg.DeclMode = RbDecl.Checked
			RbDev.Checked = Not RbDecl.Checked
			DeclOnOff()
		End If
	End Sub


#Region "GUI Tests"

	Private ReadOnly GUItest0 As New GUItest(Me)
	Private mouseDownOnListView As Boolean

	Private Class GUItest
		Private RowLim As Integer = 9
		Private ColLim As Integer = 45
		Public TestActive As Boolean = False
		Private TestAborted As Boolean
		Private xCtrl As Integer
		Private xPanel As Integer
		Private Scr As Integer
		Private PRbAlt As Boolean
		Private ReadOnly Ctrls(RowLim + 1) As Integer
		Private ReadOnly Pnls(RowLim + 1) As Integer
		Private CtrlC As Integer
		Private CtrlCL As Integer
		Private PnDir As Integer
		Private PnDirC As Integer
		Private PnDirCL As Integer
		Private PnDirRnd As Integer
		Private CtrlRnd As Integer
		Private DiffC As Integer
		Private DiffLvl As Integer
		Private bInit As Integer
		Private ReadOnly MyForm As MainForm
		Private ReadOnly KeyCode As List(Of Integer)

		Private Sub TestRun()

			Dim z As Integer

			xPanel = ColLim - 10
			xCtrl = ColLim - 10
			PRbAlt = False
			Scr = 0
			PnDir = 0
			PnDirCL = 10
			PnDirC = 0 ' StrDirCL
			CtrlCL = 5
			CtrlC = CtrlCL
			PnDirRnd = 5
			CtrlRnd = 8
			DiffC = 0
			DiffLvl = 1
			bInit = 0
			TestAborted = False
			Randomize()


			MyForm.LvMsg.Items.Clear()
			MyForm.ToolStripLbStatus.Text = "Score: 0000             Press <Esc> to Quit"

			For z = 1 To RowLim - 6
				PRbAlt = Not PRbAlt
				If Not PRbAlt Then
					MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|       |*")
				Else
					MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   |   |*")
				End If
			Next

			PRbAlt = False

			MyForm.LvMsg.Items.Add("  VECTO Interactive Mode" & Space(ColLim - 35) & "*|       |*")
			MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   |   |*")
			MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|       |*")
			MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   |   |*")
			MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|       |*")
			MyForm.LvMsg.Items.Add(Space(ColLim - 11) & "*|   ∆   |*")

			For z = 1 To RowLim + 1
				Pnls(z) = ColLim - 10
				Ctrls(z) = 0
			Next

			MyForm.TmProgSec.Interval = 200

			MyForm.LvMsg.Focus()

			MyForm.TmProgSec.Start()
		End Sub

		Public Sub TestStop()
			MyForm.TmProgSec.Stop()
			TestActive = False
			MyForm.LvMsg.Items.Clear()
			CtrlC = 0
			MyForm.ToolStripLbStatus.Text = MyForm.LastModeName & " Mode"
		End Sub

		Public Sub TestTick()

			If bInit = 24 Then GoTo LbRace
			bInit += 1

			Select Case bInit
				Case 10
					MyForm.LvMsg.Items.RemoveAt(RowLim - 6)
					MyForm.LvMsg.Items.RemoveAt(RowLim - 5)
					MyForm.LvMsg.Items.Insert(RowLim - 6, Space(ColLim - 11) & "*|       |*")
					MyForm.LvMsg.Items.Insert(RowLim - 4, Space(ColLim - 30) & "  3      " & Space(10) & "*|       |*")
				Case 14
					MyForm.LvMsg.Items.RemoveAt(RowLim - 4)
					MyForm.LvMsg.Items.Insert(RowLim - 4, Space(ColLim - 30) & "  2      " & Space(10) & "*|       |*")
				Case 18
					MyForm.LvMsg.Items.RemoveAt(RowLim - 4)
					MyForm.LvMsg.Items.Insert(RowLim - 4, Space(ColLim - 30) & "  1      " & Space(10) & "*|       |*")
				Case 22
					MyForm.LvMsg.Items.RemoveAt(RowLim - 4)
					MyForm.LvMsg.Items.Insert(RowLim - 4, Space(ColLim - 30) & " Go!     " & Space(10) & "*|       |*")
				Case 24
					MyForm.LvMsg.Items.RemoveAt(RowLim - 4)
					MyForm.LvMsg.Items.Insert(RowLim - 4, Space(ColLim - 30) & "         " & Space(10) & "*|       |*")
			End Select
			Exit Sub
LbRace:

			PRbAlt = Not PRbAlt

			MyForm.LvMsg.BeginUpdate()

			sLists()

			sAlign()

			sSetCtrl()

			sSetPanel()

			MyForm.LvMsg.Items.RemoveAt(RowLim)

			sUpdateCtrl()

			MyForm.LvMsg.EndUpdate()

			If Math.Abs(xCtrl - Pnls(2)) > 4 Then
				sAbort()
				Exit Sub
			ElseIf Ctrls(2) <> 0 Then
				If xCtrl = Pnls(2) + Ctrls(2) - 4 Then
					sAbort()
					Exit Sub
				End If
				Scr += 5 * DiffLvl
			End If

			Scr += DiffLvl
			DiffC += 1

			'Erhöhe Schwierigkeitsgrad
			If DiffC = (DiffLvl + 3) * 4 Then
				DiffC = 0
				DiffLvl += 1
				If DiffLvl > 2 And DiffLvl < 7 Then MyForm.TmProgSec.Interval = 300 - (DiffLvl) * 30
				Scr += 100
				Select Case DiffLvl
					Case 3
						PnDirCL = 3
						CtrlCL = 4
						CtrlRnd = 6
					Case 5
						PnDirCL = 2
						PnDirRnd = 4
					Case 8
						CtrlCL = 2
					Case 10
						CtrlRnd = 4
						PnDirRnd = 3
				End Select
			End If
		End Sub

		Public Sub TestKey(Key0 As Integer)

			If TestActive Then
				Select Case Key0
					Case Keys.Left
						xCtrl -= 1
						sUpdateCtrl()
					Case Keys.Right
						xCtrl += 1
						sUpdateCtrl()
					Case Keys.Escape
						TestStop()
				End Select
			Else

				If KeyCode(CtrlC) = Key0 Then
					CtrlC += 1
					If CtrlC = KeyCode.Count Then
						TestActive = True
						TestRun()
					End If
				Else
					CtrlC = 0
				End If

			End If
		End Sub

		Private Sub sAbort()

			Dim s As String, s1 As String

			If TestAborted Then Exit Sub

			TestAborted = True

			MyForm.TmProgSec.Stop()

			MyForm.LvMsg.BeginUpdate()

			s = MyForm.LvMsg.Items(0).Text
			MyForm.LvMsg.Items.RemoveAt(0)
			MyForm.LvMsg.Items.Insert(0, "You crashed!" & Microsoft.VisualBasic.Right(s, Len(s) - 12))

			s = MyForm.LvMsg.Items(1).Text
			s1 = "Score: " & Scr & " "
			MyForm.LvMsg.Items.RemoveAt(1)
			MyForm.LvMsg.Items.Insert(1, s1 & Microsoft.VisualBasic.Right(s, Len(s) - Len(s1)))

			MyForm.LvMsg.EndUpdate()

			LogFile.WriteToLog(MessageType.Normal, "*** Race Score: " & Scr.ToString("0000") & " ***")

			CtrlC = 0
			TestActive = False

			MyForm.ToolStripLbStatus.Text = MyForm.LastModeName & " Mode"
		End Sub

		Private Sub sSetCtrl()
			Dim x As Integer
			If Scr < 10 Then Exit Sub
			Ctrls(RowLim + 1) = 0
			CtrlC += 1
			If CtrlC < CtrlCL Then Exit Sub
			Select Case CInt(Int((CtrlRnd * Rnd()) + 1))
				Case 1, 2
					CtrlC = 0
					x = CInt(Int((7 * Rnd()) + 1))
					Ctrls(RowLim + 1) = x
				Case Else
			End Select
		End Sub

		Private Sub sUpdateCtrl()
			Dim s As String
			If bInit < 21 Then
				xCtrl = ColLim - 10
				Exit Sub
			End If
			If Math.Abs(xCtrl - Pnls(1)) > 5 Then
				sAbort()
				Exit Sub
			End If
			s = Replace(MyForm.LvMsg.Items(RowLim - 1).Text.ToString, "∆", " ") & "   "
			s = Microsoft.VisualBasic.Left(s, ColLim + 15)
			's = s.Remove(0, 20)
			's = "Press <Esc> to Quit " & s
			If Mid(s, xCtrl + 5, 1) = "X" Then
				sAbort()
				Exit Sub
			End If
			s = s.Remove(xCtrl + 4, 1)
			's = Trim(s.Insert(xCar + 4, "∆")) & Space(ColLim + 5 - Streets(2)) & "Pts: " & Pts & " Lv: " & DiffLvl
			s = Space(Pnls(2) - 1) & Trim(s.Insert(xCtrl + 4, "∆"))
			MyForm.LvMsg.Items.RemoveAt(RowLim - 1)
			MyForm.LvMsg.Items.Insert(RowLim - 1, s)
			MyForm.ToolStripLbStatus.Text = "Score: " & Scr.ToString("0000") & "             Press <Esc> to Quit"
		End Sub

		Private Sub sSetPanel()
			Dim s As String
			s = "*|   |   |*"
			If PRbAlt Then
				s = s.Remove(5, 1)
				s = s.Insert(5, " ")
			End If
			If Ctrls(RowLim + 1) <> 0 Then
				s = s.Remove(Ctrls(RowLim + 1) + 1, 1)
				s = s.Insert(Ctrls(RowLim + 1) + 1, "X")
			End If
			Select Case xPanel - Pnls(RowLim)
				Case -1
					s = Replace(s, "|", "\")
				Case 1
					s = Replace(s, "|", "/")
			End Select
			MyForm.LvMsg.Items.Insert(0, Space(xPanel - 1) & s)
		End Sub

		Private Sub sAlign()
			PnDirC += 1
			If PnDirC < PnDirCL Then GoTo Lb1
			PnDirC = 0
			Select Case CInt(Int((PnDirRnd * Rnd()) + 1))
				Case 1
					PnDir = 1
				Case 2
					PnDir = -1
				Case Else
					PnDir = 0
			End Select
Lb1:
			xPanel += PnDir
			If xPanel > ColLim Then
				xPanel = ColLim
			ElseIf xPanel < 22 Then
				xPanel = 22
			End If
			Pnls(RowLim + 1) = xPanel
		End Sub

		Private Sub sLists()
			Dim x As Integer
			For x = 2 To RowLim + 1
				Ctrls(x - 1) = Ctrls(x)
				Pnls(x - 1) = Pnls(x)
			Next
		End Sub

		Public Sub New(form As MainForm)
			MyForm = form
			KeyCode = New List(Of Integer)
			KeyCode.Add(Keys.Up)
			KeyCode.Add(Keys.Up)
			KeyCode.Add(Keys.Down)
			KeyCode.Add(Keys.Down)
			KeyCode.Add(Keys.Left)
			KeyCode.Add(Keys.Right)
			KeyCode.Add(Keys.Left)
			KeyCode.Add(Keys.Right)
			KeyCode.Add(Keys.B)
			KeyCode.Add(Keys.A)
			CtrlC = 0
		End Sub
	End Class

	Private Sub LvMsg_KeyDown(sender As Object, e As KeyEventArgs) Handles LvMsg.KeyDown
		GUItest0.TestKey(e.KeyValue)
		If GUItest0.TestActive Then e.SuppressKeyPress = True
	End Sub

	Private Sub LvMsg_LostFocus(sender As Object, e As EventArgs) Handles LvMsg.LostFocus
		If GUItest0.TestActive Then GUItest0.TestStop()
	End Sub

#End Region

	Private Sub LvMsg_KeyUp(sender As Object, e As KeyEventArgs) Handles LvMsg.KeyUp
		If (e.Control And e.KeyCode = Keys.C) Then
			Dim builder As StringBuilder = New StringBuilder()
			For Each selectedItem As ListViewItem In LvMsg.SelectedItems
				builder.AppendLine(String.Join(", ",
												selectedItem.SubItems.Cast(Of ListViewItem.ListViewSubItem).Select(
													Function(item) item.Text)))
			Next
			Clipboard.SetText(builder.ToString())
		End If
	End Sub

	Private Sub LvMsg_MouseDown(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseDown
		mouseDownOnListView = True
	End Sub

	Private Sub LvMsg_MouseUp(sender As Object, e As MouseEventArgs) Handles LvMsg.MouseUp
		mouseDownOnListView = False
	End Sub

	Private Sub LvGEN_MouseUp(sender As Object, e As MouseEventArgs) Handles LvGEN.MouseUp
		If e.Button = MouseButtons.Right Then
			ConMenTarget = LvGEN
			ConMenTarJob = True

			'Locked functions show/hide
			LoadListToolStripMenuItem.Enabled = Not GUIlocked
			LoadDefaultListToolStripMenuItem.Enabled = Not GUIlocked
			ClearListToolStripMenuItem.Enabled = Not GUIlocked

			ConMenFilelist.Show(MousePosition)
		End If
	End Sub

	Private Sub RbDev_CheckedChanged(sender As Object, e As EventArgs) Handles RbDev.CheckedChanged
	End Sub


	Private Class VectoProgress
		Public Target As String
		Public Message As String
		Public Link As String
	End Class
End Class
