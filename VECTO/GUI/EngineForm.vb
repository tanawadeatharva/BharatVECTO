
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Text.RegularExpressions
Imports System.Windows.Forms.DataVisualization.Charting
Imports System.Xml.Linq
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Utils
Imports VectoAuxiliaries
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
		PnWhtcDeclaration.Enabled = Cfg.DeclMode
		PnWhtcEngineering.Enabled = Not Cfg.DeclMode

		_changed = False
		NewEngine()
	End Sub

	'Set generic values for Declaration mode.
	Private Sub DeclInit()

		If Not Cfg.DeclMode Then Exit Sub

		TbInertia.Text = DeclarationData.Engine.EngineInertia((TbDispl.Text.ToDouble(0.0) / 1000.0 / 1000.0).SI(Of CubicMeter),
															GearboxType.AMT).ToGUIFormat()
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
			VectoJobForm.VectoNew()
		Else
			VectoJobForm.WindowState = FormWindowState.Normal
		End If

		VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_engFile, JobDir)
	End Sub

	Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
		If File.Exists(MyAppPath & "User Manual\help.html") Then
			Dim browserRegistryString As String =
					My.Computer.Registry.ClassesRoot.OpenSubKey("\http\shell\open\command\").GetValue("").ToString
			Dim defaultBrowserPath As String =
					Regex.Match(browserRegistryString, "(\"".*?\"")").Captures(0).ToString
			Process.Start(defaultBrowserPath,
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
	Public Sub OpenEngineFile(file As String)
		Dim engine As IEngineEngineeringInputData

		If ChangeCheckCancel() Then Exit Sub

		Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file), 
																IEngineeringInputDataProvider)

		engine = inputData.EngineInputData


		If Cfg.DeclMode <> engine.SavedInDeclarationMode Then
			Select Case WrongMode()
				Case 1
					Close()
					MainForm.RbDecl.Checked = Not MainForm.RbDecl.Checked
					MainForm.OpenVectoFile(file)
				Case -1
					Exit Sub
			End Select
		End If

		Dim basePath As String = Path.GetDirectoryName(file)
		TbName.Text = engine.ModelName
		TbDispl.Text = (engine.Displacement * 1000 * 1000).ToGUIFormat()
		TbInertia.Text = engine.Inertia.ToGUIFormat()
		TbNleerl.Text = engine.IdleSpeed.AsRPM.ToGUIFormat()

		TbMAP.Text = GetRelativePath(engine.FuelConsumptionMap.Source, basePath)
		TbFLD.Text = GetRelativePath(engine.FullLoadCurve.Source, basePath)
		TbWHTCurban.Text = engine.WHTCUrban.ToGUIFormat()
		TbWHTCrural.Text = engine.WHTCRural.ToGUIFormat()
		TbWHTCmw.Text = engine.WHTCMotorway.ToGUIFormat()
		TbWHTCEngineering.Text = engine.WHTCEngineering.ToGUIFormat()
		TbColdHotFactor.Text = engine.ColdHotBalancingFactor.ToGUIFormat()

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
	Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
		If _engFile = "" Or saveAs Then
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
		engine.Displacement = TbDispl.Text.ToDouble(0)
		engine.EngineInertia = TbInertia.Text.ToDouble(0)
		engine.IdleSpeed = TbNleerl.Text.ToDouble(0)

		engine.PathFld = TbFLD.Text
		engine.PathMap = TbMAP.Text


		engine.WHTCUrbanInput = TbWHTCurban.Text.ToDouble(0)
		engine.WHTCRuralInput = TbWHTCrural.Text.ToDouble(0)
		engine.WHTCMotorwayInput = TbWHTCmw.Text.ToDouble(0)
		engine.WHTCEngineeringInput = TbWHTCEngineering.Text.ToDouble(0)

		engine.ColdHotBalancingFactorInput = TbColdHotFactor.Text.ToDouble(0)


		If Not engine.SaveFile Then
			MsgBox("Cannot safe to " & file, MsgBoxStyle.Critical)
			Return False
		End If

		If AutoSendTo Then
			If VectoJobForm.Visible Then
				If UCase(FileRepl(VectoJobForm.TbENG.Text, JobDir)) <> UCase(file) Then _
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
		If FuelConsumptionMapFileBrowser.OpenDialog(FileRepl(TbMAP.Text, GetPath(_engFile))) Then _
			TbMAP.Text = GetFilenameWithoutDirectory(FuelConsumptionMapFileBrowser.Files(0), GetPath(_engFile))
	End Sub


	'Open VMAP file
	Private Sub BtMAPopen_Click(sender As Object, e As EventArgs) Handles BtMAPopen.Click
		Dim fldfile As String

		fldfile = FileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
			OpenFiles(FileRepl(TbMAP.Text, GetPath(_engFile)), fldfile)
		Else
			OpenFiles(FileRepl(TbMAP.Text, GetPath(_engFile)))
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
		Dim fullLoadCurve As FullLoadCurve = Nothing
		Dim fcMap As FuelConsumptionMap = Nothing


		PicBox.Image = Nothing

		'If Not File.Exists(_engFile) Then Exit Sub

		Try
			Dim fldFile As String =
					If(Not String.IsNullOrWhiteSpace(_engFile), Path.Combine(Path.GetDirectoryName(_engFile), TbFLD.Text), TbFLD.Text)
			fullLoadCurve = FullLoadCurveReader.Create(VectoCSVFile.Read(fldFile), engineFld:=True)
		Catch ex As Exception
		End Try

		Try
			Dim fcFile As String =
					If(Not String.IsNullOrWhiteSpace(_engFile), Path.Combine(Path.GetDirectoryName(_engFile), TbMAP.Text), TbMAP.Text)
			fcMap = FuelConsumptionMapReader.Create(VectoCSVFile.Read(fcFile))
		Catch ex As Exception
		End Try

		If fullLoadCurve Is Nothing AndAlso fcMap Is Nothing Then Exit Sub


		'Create plot
		Dim chart As Chart = New Chart
		chart.Width = PicBox.Width
		chart.Height = PicBox.Height

		Dim chartArea As ChartArea = New ChartArea

		If Not fullLoadCurve Is Nothing Then
			Dim series As Series = New Series
			series.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
									fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueFullLoad.Value()).ToArray())
			series.ChartType = SeriesChartType.FastLine
			series.BorderWidth = 2
			series.Color = Color.DarkBlue
			series.Name = "Full load (" & TbFLD.Text & ")"
			chart.Series.Add(series)

			series = New Series
			series.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
									fullLoadCurve.FullLoadEntries.Select(Function(x) x.TorqueDrag.Value()).ToArray())
			series.ChartType = SeriesChartType.FastLine
			series.BorderWidth = 2
			series.Color = Color.Blue
			series.Name = "Motoring (" & Path.GetFileNameWithoutExtension(TbMAP.Text) & ")"
			chart.Series.Add(series)
		End If

		If Not fcMap Is Nothing Then
			Dim series As Series = New Series
			series.Points.DataBindXY(fcMap.Entries.Select(Function(x) x.EngineSpeed.AsRPM).ToArray(),
									fcMap.Entries.Select(Function(x) x.Torque.Value()).ToArray())
			series.ChartType = SeriesChartType.Point
			series.MarkerSize = 3
			series.Color = Color.Red
			series.Name = "Map"
			chart.Series.Add(series)
		End If

		chartArea.Name = "main"

		chartArea.AxisX.Title = "engine speed [1/min]"
		chartArea.AxisX.TitleFont = New Font("Helvetica", 10)
		chartArea.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
		chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
		chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

		chartArea.AxisY.Title = "engine torque [Nm]"
		chartArea.AxisY.TitleFont = New Font("Helvetica", 10)
		chartArea.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
		chartArea.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
		chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

		chartArea.AxisX.Minimum = 300
		chartArea.BorderDashStyle = ChartDashStyle.Solid
		chartArea.BorderWidth = 1

		chartArea.BackColor = Color.GhostWhite

		chart.ChartAreas.Add(chartArea)

		chart.Update()

		Dim img As Bitmap = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
		chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))


		PicBox.Image = img
	End Sub


#Region "Open File Context Menu"

	Private _contextMenuFiles As String()

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


	Private Sub BtFLD_Click(sender As Object, e As EventArgs) Handles BtFLD.Click
		If FullLoadCurveFileBrowser.OpenDialog(FileRepl(TbFLD.Text, GetPath(_engFile))) Then _
			TbFLD.Text = GetFilenameWithoutDirectory(FullLoadCurveFileBrowser.Files(0), GetPath(_engFile))
	End Sub

	Private Sub BtFLDopen_Click(sender As Object, e As EventArgs) Handles BtFLDopen.Click
		Dim fldfile As String

		fldfile = FileRepl(TbFLD.Text, GetPath(_engFile))

		If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
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

	Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TbWHTCEngineering.TextChanged
	End Sub

	Private Sub Label9_Click(sender As Object, e As EventArgs) Handles lblWhtcEngineering.Click
	End Sub
End Class
