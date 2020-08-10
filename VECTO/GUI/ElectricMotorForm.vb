
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Linq
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Engine
Imports TUGraz.VectoCore.Utils
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

''' <summary>
''' Engine Editor. Open and save .VENG files.
''' </summary>
''' <remarks></remarks>
Public Class ElectricMotorForm
    Private _emFile As String = ""
    Public AutoSendTo As Boolean = False
    Public JobDir As String = ""
    Private _changed As Boolean = False

    Private _contextMenuFiles As String()




    'Before closing Editor: Check if file was changed and ask to save.
    Private Sub F_ENG_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If e.CloseReason <> CloseReason.ApplicationExitCall And e.CloseReason <> CloseReason.WindowsShutDown Then
            e.Cancel = ChangeCheckCancel()
        End If
    End Sub

    'Initialise.
    Private Sub EngineFormLoad(sender As Object, e As EventArgs) Handles Me.Load

        ' initialize form on load - nothng to do right now

        'pnInertia.Enabled = Not Cfg.DeclMode


        _changed = False


        NewEngine()
    End Sub

    'Set generic values for Declaration mode.
    Private Sub DeclInit()

        If Not Cfg.DeclMode Then Exit Sub



    End Sub


#Region "Toolbar"

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        NewEngine()
    End Sub

    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If EngineFileBrowser.OpenDialog(_emFile) Then
            Try
                OpenElectricMachineFile(EngineFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Engine File")
            End Try
        End If
    End Sub

    Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
        SaveOrSaveAs(False)
    End Sub

    Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
        SaveOrSaveAs(True)
    End Sub

    Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click

        If ChangeCheckCancel() Then Exit Sub

        If _emFile = "" Then
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

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_emFile, JobDir)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,
                        String.Format("""file://{0}""", Path.Combine(MyAppPath, "User Manual\help.html#engine-editor")))
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub

#End Region

    'Create new empty Engine file.
    Private Sub NewEngine()

        If ChangeCheckCancel() Then Exit Sub

        tbMakeModel.Text = ""
        tbInertia.Text = ""
        tbMap.Text = ""
        tbDragTorque.Text = ""
        tbMaxTorque.Text = ""

        DeclInit()

        _emFile = ""
        Text = "Electric Motor Editor"
        LbStatus.Text = ""

        _changed = False

        UpdatePic()
    End Sub

    'Open VENG file
    Public Sub OpenElectricMachineFile(file As String)
        Dim engine As IElectricMotorEngineeringInputData

        If ChangeCheckCancel() Then Exit Sub

        Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file),
                                                                IEngineeringInputDataProvider)

        engine = inputData.JobInputData.Vehicle.Components.ElectricMachines.Entries.First().ElectricMachine

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
        tbMakeModel.Text = engine.Model
        tbInertia.Text = engine.Inertia.ToGUIFormat()


        tbDragTorque.Text = GetRelativePath(engine.DragCurve.Source, basePath)
        tbMaxTorque.Text = GetRelativePath(engine.FullLoadCurve.Source, basePath)
        tbMap.Text = GetRelativePath(engine.EfficiencyMap.Source, basePath)
        DeclInit()

        ElectricMotorFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _emFile = file
        Activate()

        _changed = False
        UpdatePic()
    End Sub

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _emFile = "" Or saveAs Then
            If ElectricMotorFileBrowser.SaveDialog(_emFile) Then
                _emFile = ElectricMotorFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveElectricMotorToFile(_emFile)
    End Function

    'Save VENG file to given filepath. Called by SaveOrSaveAs. 
    Private Function SaveElectricMotorToFile(ByVal file As String) As Boolean

        Dim em As ElectricMachine = New ElectricMachine
        em.FilePath = file

        em.ModelName = tbMakeModel.Text
        If Trim(em.ModelName) = "" Then em.ModelName = "Undefined"
        em.MotorInertia = tbInertia.Text.ToDouble(0)

        em.PathMaxTorque = tbMaxTorque.Text
        em.PathDrag = tbDragTorque.Text
        em.PathMap = tbMap.Text

        If Not em.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        'If AutoSendTo Then
        '    If VectoJobForm.Visible Then
        '        If UCase(FileRepl(VectoJobForm.TbENG.Text, JobDir)) <> UCase(file) Then _
        '            VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(file, JobDir)
        '        VectoJobForm.UpdatePic()
        '    End If
        'End If

        ElectricMotorFileBrowser.UpdateHistory(file)
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


    Private Sub TbName_TextChanged(sender As Object, e As EventArgs) Handles tbMakeModel.TextChanged
        Change()
    End Sub

    Private Sub TbDispl_TextChanged(sender As Object, e As EventArgs)
        Change()
        DeclInit()
    End Sub

    Private Sub TbInertia_TextChanged(sender As Object, e As EventArgs) Handles tbInertia.TextChanged
        Change()
    End Sub

    Private Sub TbNleerl_TextChanged(sender As Object, e As EventArgs)
        UpdatePic()
        Change()
    End Sub

    Private Sub TbMAP_TextChanged(sender As Object, e As EventArgs) _
        Handles tbDragTorque.TextChanged
        UpdatePic()
        Change()
    End Sub

    Private Sub TbWHTCurban_TextChanged(sender As Object, e As EventArgs)
        Change()
    End Sub

    Private Sub TbWHTCrural_TextChanged(sender As Object, e As EventArgs)
        Change()
    End Sub

    Private Sub TbWHTCmw_TextChanged(sender As Object, e As EventArgs)
        Change()
    End Sub


#End Region


    'Open VMAP file
    Private Sub BtMAPopen_Click(sender As Object, e As EventArgs)
        Dim fldfile As String

        fldfile = FileRepl(tbDragTorque.Text, GetPath(_emFile))

        If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
            OpenFiles(FileRepl(tbMap.Text, GetPath(_emFile)), fldfile)
        Else
            OpenFiles(FileRepl(tbMap.Text, GetPath(_emFile)))
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
        Dim fullLoadCurve As ElectricFullLoadCurve = Nothing
        Dim dragCurve As DragCurve = Nothing
        Dim fcMap As EfficiencyMap = Nothing

        'Dim engineCharacteristics As String = ""

        PicBox.Image = Nothing

        'If Not File.Exists(_engFile) Then Exit Sub

        Try
            Dim fldFile As String =
                    If(Not String.IsNullOrWhiteSpace(_emFile), Path.Combine(Path.GetDirectoryName(_emFile), tbMaxTorque.Text), tbDragTorque.Text)
            If File.Exists(fldFile) Then _
                fullLoadCurve = ElectricFullLoadCurveReader.Create(VectoCSVFile.Read(fldFile), 1.0, 1, 1.0)
        Catch ex As Exception
        End Try

        Try
            Dim fcFile As String =
                    If(Not String.IsNullOrWhiteSpace(_emFile), Path.Combine(Path.GetDirectoryName(_emFile), tbMap.Text), tbMap.Text)
            If File.Exists(fcFile) Then fcMap = ElectricMotorMapReader.Create(VectoCSVFile.Read(fcFile), 1.0, 1, 1.0)
        Catch ex As Exception
        End Try

        Try
            Dim dragFile As String =
                    If(Not String.IsNullOrWhiteSpace(_emFile), Path.Combine(Path.GetDirectoryName(_emFile), tbDragTorque.Text), tbMap.Text)
            If File.Exists(dragFile) Then dragCurve = ElectricMotorDragCurveReader.Create(VectoCSVFile.Read(dragFile), 1.0, 1, 1.0)
        Catch ex As Exception
        End Try

        If fullLoadCurve Is Nothing AndAlso fcMap Is Nothing AndAlso dragCurve Is Nothing Then Exit Sub

        'Create plot
        Dim chart As Chart = New Chart
        chart.Width = PicBox.Width
        chart.Height = PicBox.Height

        Dim chartArea As ChartArea = New ChartArea

        If Not fullLoadCurve Is Nothing Then
            Dim series As Series = New Series
            series.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.MotorSpeed.AsRPM).ToArray(),
                                    fullLoadCurve.FullLoadEntries.Select(Function(x) x.FullDriveTorque.Value()).ToArray())
            series.ChartType = SeriesChartType.FastLine
            series.BorderWidth = 2
            series.Color = Color.DarkBlue
            series.Name = "Max drive torque (" & tbDragTorque.Text & ")"
            chart.Series.Add(series)

            series = New Series
            series.Points.DataBindXY(fullLoadCurve.FullLoadEntries.Select(Function(x) x.MotorSpeed.AsRPM).ToArray(),
                                    fullLoadCurve.FullLoadEntries.Select(Function(x) x.FullGenerationTorque.Value()).ToArray())
            series.ChartType = SeriesChartType.FastLine
            series.BorderWidth = 2
            series.Color = Color.Blue
            series.Name = "Max generation torque (" & Path.GetFileNameWithoutExtension(tbMap.Text) & ")"
            chart.Series.Add(series)

            'engineCharacteristics +=
            '    String.Format("Max. Torque: {0:F0} Nm; Max. Power: {1:F1} kW; n_rated: {2:F0} rpm; n_95h: {3:F0} rpm",
            '                fullLoadCurve.MaxTorque.Value(), fullLoadCurve.MaxPower.Value() / 1000, fullLoadCurve.RatedSpeed.AsRPM,
            '                fullLoadCurve.N95hSpeed.AsRPM)
        End If

        If Not fcMap Is Nothing Then
            Dim series As Series = New Series
            series.Points.DataBindXY(fcMap.Entries.Select(Function(x) x.MotorSpeed.AsRPM).ToArray(),
                                    fcMap.Entries.Select(Function(x) x.Torque.Value()).ToArray())
            series.ChartType = SeriesChartType.Point
            series.MarkerSize = 3
            series.Color = Color.Red
            series.Name = "Map"
            chart.Series.Add(series)
        End If

        If Not dragCurve Is Nothing Then
            Dim series As Series = New Series
            series.Points.DataBindXY(dragCurve.Entries.Select(Function(x) x.MotorSpeed.AsRPM).ToArray(),
                                     dragCurve.Entries.Select(Function(x) x.DragTorque.Value()).ToArray())
            series.ChartType = SeriesChartType.FastLine
            series.BorderWidth = 2
            series.Color = Color.Green
            series.Name = "Drag torque"
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

        chartArea.AxisX.Minimum = 0
        chartArea.BorderDashStyle = ChartDashStyle.Solid
        chartArea.BorderWidth = 1

        chartArea.BackColor = Color.GhostWhite

        chart.ChartAreas.Add(chartArea)

        chart.Update()

        Dim img As Bitmap = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
        chart.DrawToBitmap(img, New Rectangle(0, 0, PicBox.Width, PicBox.Height))


        PicBox.Image = img
        'lblEngineCharacteristics.Text = engineCharacteristics
    End Sub


#Region "Open File Context Menu"


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

    Private Sub btnMaxTorqueCurveOpen_Click(sender As Object, e As EventArgs) Handles btnMaxTorqueCurveOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbMaxTorque.Text, GetPath(_emFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbMaxTorque.Text, GetPath(_emFile)), theFile)
        Else
            OpenFiles(FileRepl(tbMaxTorque.Text, GetPath(_emFile)))
        End If
    End Sub

    Private Sub btnDragCurveOpen_Click(sender As Object, e As EventArgs) Handles btnDragCurveOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbDragTorque.Text, GetPath(_emFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbDragTorque.Text, GetPath(_emFile)), theFile)
        Else
            OpenFiles(FileRepl(tbDragTorque.Text, GetPath(_emFile)))
        End If
    End Sub

    Private Sub btnEmMapOpen_Click(sender As Object, e As EventArgs) Handles btnEmMapOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbMap.Text, GetPath(_emFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbMap.Text, GetPath(_emFile)), theFile)
        Else
            OpenFiles(FileRepl(tbMap.Text, GetPath(_emFile)))
        End If
    End Sub

    Private Sub btnBrowseMaxTorque_Click(sender As Object, e As EventArgs) Handles btnBrowseMaxTorque.Click
        If ElectricMachineMaxTorqueFileBrowser.OpenDialog(FileRepl(tbMaxTorque.Text, GetPath(_emFile))) Then _
            tbMaxTorque.Text = GetFilenameWithoutDirectory(ElectricMachineMaxTorqueFileBrowser.Files(0), GetPath(_emFile))
    End Sub

    Private Sub btnBrowseDragCurve_Click(sender As Object, e As EventArgs) Handles btnBrowseDragCurve.Click
        If ElectricMachineDragTorqueFileBrowser.OpenDialog(FileRepl(tbDragTorque.Text, GetPath(_emFile))) Then _
            tbDragTorque.Text = GetFilenameWithoutDirectory(ElectricMachineDragTorqueFileBrowser.Files(0), GetPath(_emFile))
    End Sub

    Private Sub btnBrowseEmMap_Click(sender As Object, e As EventArgs) Handles btnBrowseEmMap.Click
        If ElectricMachineEfficiencyMapFileBrowser.OpenDialog(FileRepl(tbMap.Text, GetPath(_emFile))) Then _
            tbMap.Text = GetFilenameWithoutDirectory(ElectricMachineEfficiencyMapFileBrowser.Files(0), GetPath(_emFile))
    End Sub
End Class
