
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Xml.Linq
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.Models.Declaration
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
''' Engine Editor. Open and save .VENG files.
''' </summary>
''' <remarks></remarks>
Public Class EngineForm
	Private _engFile As String = ""
	Public AutoSendTo As Boolean = False
	Public JobDir As String = ""
	Private _changed As Boolean = False


	'Before closing Editor: Check if file was changed and ask to save.
	Private Sub F_ENG_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
		If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
			e.Cancel = ChangeCheckCancel()
		End If
	End Sub

	'Initialise.
	Private Sub EngineFormLoad(sender As Object, e As EventArgs) Handles Me.Load

		PnInertia.Enabled = Not Cfg.DeclMode
		GrWHTC.Enabled = Cfg.DeclMode


		_changed = False
		NewEngine()
	End Sub

	'Set generic values for Declaration mode.
	Private Sub DeclInit()

		If Not Cfg.DeclMode Then Exit Sub

		TbInertia.Text =
			CStr(
				DeclarationData.Engine.EngineInertia((fTextboxToNumString(TbDispl.Text) / 1000.0).SI(Of CubicMeter),
													GearboxType.AMT).Value())
	End Sub


#Region "Toolbar"

	Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
		NewEngine()
	End Sub

	Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
		If EngineFileBrowser.OpenDialog(_engFile) Then OpenEngineFile(EngineFileBrowser.Files(0))
	End Sub

	Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
		SaveOrSaveAs(False)
	End Sub

	Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
		SaveOrSaveAs(True)
	End Sub

	Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click

		If ChangeCheckCancel() Then Exit Sub

		If _engFile = "" Then
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

		VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_engFile, JobDir)
	End Sub

	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim BrowserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim DefaultBrowserPath As String =
					Regex.Match(BrowserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(DefaultBrowserPath,
						String.Format("""{0}{1}""", MyAppPath, "User Manual\help.html#engine-editor"))
		Else
			MsgBox("User Manual not found!", MsgBoxStyle.Critical)
		End If
	End Sub

#End Region

	'Create new empty Engine file.
	Private Sub NewEngine()

		If ChangeCheckCancel() Then Exit Sub

		TbName.Text = ""
		TbDispl.Text = ""
		TbInertia.Text = ""
		TbNleerl.Text = ""
		TbMAP.Text = ""
		TbFLD.Text = ""
		TbWHTCurban.Text = ""
		TbWHTCrural.Text = ""
		TbWHTCmw.Text = ""

		DeclInit()

		_engFile = ""
		Text = "ENG Editor"
		LbStatus.Text = ""

		_changed = False

		UpdatePic()
	End Sub

	'Open VENG file
	Public Sub OpenEngineFile(ByVal file As String)
		Dim ENG0 As Engine

		If ChangeCheckCancel() Then Exit Sub

		ENG0 = New Engine

		ENG0.FilePath = file

		If Not ENG0.ReadFile Then
			MsgBox("Cannot read " & file & "!")
			Exit Sub
		End If

		If Cfg.DeclMode <> ENG0.SavedInDeclMode Then
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

		TbName.Text = ENG0.ModelName
		TbDispl.Text = ENG0.Displacement.ToString
		TbInertia.Text = ENG0.EngineInertia.ToString
		TbNleerl.Text = ENG0.IdleSpeed.ToString

		TbMAP.Text = ENG0.PathMAP(True)
		TbFLD.Text = ENG0.PathFLD(True)
		TbWHTCurban.Text = ENG0.WHTCurban
		TbWHTCrural.Text = ENG0.WHTCrural
		TbWHTCmw.Text = ENG0.WHTCmw

		DeclInit()

		EngineFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""
		_engFile = file
		Activate()

		_changed = False
		UpdatePic()
	End Sub

	'Save or Save As function = true if file is saved
	Private Function SaveOrSaveAs(ByVal SaveAs As Boolean) As Boolean
		If _engFile = "" Or SaveAs Then
			If EngineFileBrowser.SaveDialog(_engFile) Then
				_engFile = EngineFileBrowser.Files(0)
			Else
				Return False
			End If
		End If
		Return SaveEngineToFile(_engFile)
	End Function

	'Save VENG file to given filepath. Called by SaveOrSaveAs. 
	Private Function SaveEngineToFile(ByVal file As String) As Boolean

		Dim engine As Engine = New Engine
		engine.FilePath = file

		engine.ModelName = TbName.Text
		If Trim(engine.ModelName) = "" Then engine.ModelName = "Undefined"
		engine.Displacement = CSng(fTextboxToNumString(TbDispl.Text))
		engine.EngineInertia = CSng(fTextboxToNumString(TbInertia.Text))
		engine.IdleSpeed = CSng(fTextboxToNumString(TbNleerl.Text))

		engine.PathFLD = TbFLD.Text
		engine.PathMAP = TbMAP.Text


		engine.WHTCurban = CSng(fTextboxToNumString(TbWHTCurban.Text))
		engine.WHTCrural = CSng(fTextboxToNumString(TbWHTCrural.Text))
		engine.WHTCmw = CSng(fTextboxToNumString(TbWHTCmw.Text))


		If Not engine.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		If AutoSendTo Then
			If VectoJobForm.Visible Then
				If UCase(fFileRepl(VectoJobForm.TbENG.Text, JobDir)) <> UCase(file) Then _
					VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(file, JobDir)
				VectoJobForm.UpdatePic()
			End If
		End If

		EngineFileBrowser.UpdateHistory(file)
		Text = GetFilenameWithoutPath(file, True)
		LbStatus.Text = ""

		_changed = False

		Return True
	End Function


#Region "Track changes"

	'Flags current file as modified.
	Private Sub Change()
		If Not _changed Then
			LbStatus.Text = "Unsaved changes in current file"
			_changed = True
		End If
	End Sub

	' "Save changes ?" .... Returns True if User aborts
	Private Function ChangeCheckCancel() As Boolean

		If _changed Then
			Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
				Case MsgBoxResult.Yes
					Return Not SaveOrSaveAs(False)
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


	Private Sub TbName_TextChanged(sender As Object, e As EventArgs) Handles TbName.TextChanged
		Change()
	End Sub

	Private Sub TbDispl_TextChanged(sender As Object, e As EventArgs) Handles TbDispl.TextChanged
		Change()
		DeclInit()
	End Sub

	Private Sub TbInertia_TextChanged(sender As Object, e As EventArgs) Handles TbInertia.TextChanged
		Change()
	End Sub

	Private Sub TbNleerl_TextChanged(sender As Object, e As EventArgs) Handles TbNleerl.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TbMAP_TextChanged(sender As Object, e As EventArgs) _
		Handles TbMAP.TextChanged, TbFLD.TextChanged
		UpdatePic()
		Change()
	End Sub

	Private Sub TbWHTCurban_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCurban.TextChanged
		Change()
	End Sub

	Private Sub TbWHTCrural_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCrural.TextChanged
		Change()
	End Sub

	Private Sub TbWHTCmw_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCmw.TextChanged
		Change()
	End Sub


#End Region

	'Browse for VMAP file
	Private Sub BtMAP_Click(sender As Object, e As EventArgs) Handles BtMAP.Click
		If FuelConsumptionMapFileBrowser.OpenDialog(fFileRepl(TbMAP.Text, GetPath(_engFile))) Then _
			TbMAP.Text = GetFilenameWithoutDirectory(FuelConsumptionMapFileBrowser.Files(0), GetPath(_engFile))
	End Sub


	'Open VMAP file
	Private Sub BtMAPopen_Click(sender As Object, e As EventArgs) Handles BtMAPopen.Click
		Dim fldfile As String

		fldfile = fFileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> sKey.NoFile AndAlso File.Exists(fldfile) Then
			OpenFiles(fFileRepl(TbMAP.Text, GetPath(_engFile)), fldfile)
		Else
			OpenFiles(fFileRepl(TbMAP.Text, GetPath(_engFile)))
		End If
	End Sub


	'Save and close
	Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
		If SaveOrSaveAs(False) Then Close()
	End Sub

	'Close without saving (see FormClosing Event)
	Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
		Close()
	End Sub

	Private Sub UpdatePic()

		Dim fldOK As Boolean = False
		Dim mapOK As Boolean = False
		Dim fullLoadCurve As New EngineFullLoadCurve
		Dim fcMap As New FuelconsumptionMap
		Dim chart As Chart
		Dim s As Series
		Dim a As ChartArea
		Dim img As Image

		PicBox.Image = Nothing

		Try

			'Read Files
			fullLoadCurve.FilePath = fFileRepl(TbFLD.Text, GetPath(_engFile))
			fldOK = fullLoadCurve.ReadFile(False, False)

			fcMap.FilePath = fFileRepl(TbMAP.Text, GetPath(_engFile))
			mapOK = fcMap.ReadFile(False)

		Catch ex As Exception

		End Try

		If Not fldOK And Not mapOK Then Exit Sub


		'Create plot
		chart = New Chart
		chart.Width = PicBox.Width
		chart.Height = PicBox.Height

		a = New ChartArea

		If fldOK Then

			s = New Series
			s.Points.DataBindXY(fullLoadCurve.EngineSpeedList, fullLoadCurve.MaxTorqueList)
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.DarkBlue
			s.Name = "Full load (" & GetFilenameWithoutPath(fullLoadCurve.FilePath, True) & ")"
			chart.Series.Add(s)

			s = New Series
			s.Points.DataBindXY(fullLoadCurve.EngineSpeedList, fullLoadCurve.DragTorqueList)
			s.ChartType = SeriesChartType.FastLine
			s.BorderWidth = 2
			s.Color = Color.Blue
			s.Name = "Motoring (" & GetFilenameWithoutPath(fullLoadCurve.FilePath, True) & ")"
			chart.Series.Add(s)

		End If

		If mapOK Then
			s = New Series
			s.Points.DataBindXY(fcMap.nU, fcMap.Tq)
			s.ChartType = SeriesChartType.Point
			s.MarkerSize = 3
			s.Color = Color.Red
			s.Name = "Map"
			chart.Series.Add(s)
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

		chart.ChartAreas.Add(a)

		chart.Update()

		img = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
		chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))


		PicBox.Image = img
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


	Private Sub BtFLD_Click(sender As Object, e As EventArgs) Handles BtFLD.Click
		If FullLoadCurveFileBrowser.OpenDialog(fFileRepl(TbFLD.Text, GetPath(_engFile))) Then _
			TbFLD.Text = GetFilenameWithoutDirectory(FullLoadCurveFileBrowser.Files(0), GetPath(_engFile))
	End Sub

	Private Sub BtFLDopen_Click(sender As Object, e As EventArgs) Handles BtFLDopen.Click
		Dim fldfile As String

		fldfile = fFileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> sKey.NoFile AndAlso File.Exists(fldfile) Then
			OpenFiles(fldfile)
		End If
	End Sub

	Private Sub BtWHTCimport_Click(sender As Object, e As EventArgs) Handles BtWHTCimport.Click
		Dim xml As XDocument

		Dim dlog As New FileBrowser("XML", False, True)
		dlog.Extensions = New String() {"xml"}

		If Not dlog.OpenDialog("") Then Exit Sub

		Try
			xml = XDocument.Load(dlog.Files(0))

			TbWHTCurban.Text = xml.<VECTO-Engine-TransferFile>.<WHTCCorrectionFactors>.<Urban>.Value
			TbWHTCrural.Text = xml.<VECTO-Engine-TransferFile>.<WHTCCorrectionFactors>.<Rural>.Value
			TbWHTCmw.Text = xml.<VECTO-Engine-TransferFile>.<WHTCCorrectionFactors>.<Motorway>.Value

		Catch ex As Exception
			MsgBox("Failed to load file! " & ex.Message, MsgBoxStyle.Critical)
		End Try
	End Sub
End Class
