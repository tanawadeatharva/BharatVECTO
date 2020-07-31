
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
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.Battery
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
Public Class BatteryForm
    Private _batteryFile As String = ""
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
        If EngineFileBrowser.OpenDialog(_batteryFile) Then
            Try
                OpenBatteryFile(EngineFileBrowser.Files(0))
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

        If _batteryFile = "" Then
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

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_batteryFile, JobDir)
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
        tbCapacity.Text = ""
        tbRiCurve.Text = ""
        tbSoCCurve.Text = ""

        DeclInit()

        _batteryFile = ""
        Text = "Electric Energy Storage Editor"
        LbStatus.Text = ""

        _changed = False

        UpdatePic()
    End Sub

    'Open VENG file
    Public Sub OpenBatteryFile(file As String)
        Dim battery As IBatteryPackEngineeringInputData

        If ChangeCheckCancel() Then Exit Sub

        Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file),
                                                                IEngineeringInputDataProvider)

        battery = inputData.JobInputData.Vehicle.Components.ElectricStorage.BatteryPack

        If Cfg.DeclMode <> battery.SavedInDeclarationMode Then
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
        tbMakeModel.Text = battery.Model
        tbCapacity.Text = battery.Capacity.ToGUIFormat()

        tbCFactor.Text = battery.MaxCurrentFactor.ToGUIFormat()
        tbSoCMin.Text = (battery.MinSOC * 100).ToGUIFormat()
        tbSoCMax.Text = (battery.MaxSOC * 100).ToGUIFormat()

        tbSoCCurve.Text = GetRelativePath(battery.VoltageCurve.Source, basePath)
        tbRiCurve.Text = GetRelativePath(battery.InternalResistanceCurve.Source, basePath)
        DeclInit()

        BatteryFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _batteryFile = file
        Activate()

        _changed = False
        UpdatePic()
    End Sub

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _batteryFile = "" Or saveAs Then
            If BatteryFileBrowser.SaveDialog(_batteryFile) Then
                _batteryFile = BatteryFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveEngineToFile(_batteryFile)
    End Function

    'Save VENG file to given filepath. Called by SaveOrSaveAs. 
    Private Function SaveEngineToFile(ByVal file As String) As Boolean

        Dim battery As Battery = New Battery
        battery.FilePath = file

        battery.ModelName = tbMakeModel.Text
        If Trim(battery.ModelName) = "" Then battery.ModelName = "Undefined"
        battery.BatCapacity = tbCapacity.Text.ToDouble(0)

        battery.PathSoCCurve = tbSoCCurve.Text
        battery.PathRiCurve = tbRiCurve.Text

        battery.BatMinSoc = tbSoCMin.Text.ToDouble(0)
        battery.BatMaxSoc = tbSoCMax.Text.ToDouble(0)

        battery.BatCFactor = tbCFactor.Text.ToDouble(0)

        If Not battery.SaveFile Then
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

        BatteryFileBrowser.UpdateHistory(file)
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

    Private Sub TbInertia_TextChanged(sender As Object, e As EventArgs) Handles tbCapacity.TextChanged
        Change()
    End Sub

    Private Sub TbNleerl_TextChanged(sender As Object, e As EventArgs)
        UpdatePic()
        Change()
    End Sub

    Private Sub TbMAP_TextChanged(sender As Object, e As EventArgs) _
        Handles tbSoCCurve.TextChanged
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

        fldfile = FileRepl(tbSoCCurve.Text, GetPath(_batteryFile))

        If fldfile <> NoFile AndAlso File.Exists(fldfile) Then
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)), fldfile)
        Else
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)))
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
        Dim socCurve As SOCMap = Nothing
        Dim riCurve As InternalResistanceMap = Nothing

        'Dim engineCharacteristics As String = ""

        PicBox.Image = Nothing

        'If Not File.Exists(_engFile) Then Exit Sub

        Try
            Dim socFile As String =
                    If(Not String.IsNullOrWhiteSpace(_batteryFile), Path.Combine(Path.GetDirectoryName(_batteryFile), tbSoCCurve.Text), tbSoCCurve.Text)
            If File.Exists(socFile) Then _
                socCurve = BatterySOCReader.Create(VectoCSVFile.Read(socFile))
        Catch ex As Exception
        End Try

        Try
            Dim riFile As String =
                    If(Not String.IsNullOrWhiteSpace(_batteryFile), Path.Combine(Path.GetDirectoryName(_batteryFile), tbRiCurve.Text), tbRiCurve.Text)
            If File.Exists(riFile) Then riCurve = BatteryInternalResistanceReader.Create(VectoCSVFile.Read(riFile), 1)
        Catch ex As Exception
        End Try

        If socCurve Is Nothing AndAlso riCurve Is Nothing Then Exit Sub

        'Create plot
        Dim chart As Chart = New Chart
        chart.Width = PicBox.Width
        chart.Height = PicBox.Height

        Dim chartArea As ChartArea = New ChartArea

        If Not socCurve Is Nothing Then
            Dim series As Series = New Series
            series.Points.DataBindXY(socCurve.Entries.Select(Function(x) x.SOC * 100).ToArray(),
                                    socCurve.Entries.Select(Function(x) x.BatteryVolts.Value()).ToArray())
            series.ChartType = SeriesChartType.FastLine
            series.BorderWidth = 2
            series.Color = Color.DarkBlue
            series.Name = "Battery Voltage (" & tbSoCCurve.Text & ")"
            chart.Series.Add(series)
        End If

        If Not riCurve Is Nothing Then
            Dim series As Series = New Series
            series.Points.DataBindXY(riCurve.Entries.Select(Function(x) x.SoC * 100).ToArray(),
                                     riCurve.Entries.Select(Function(x) x.Resistance.Value()).ToArray())
            series.ChartType = SeriesChartType.FastLine
            series.MarkerSize = 3
            series.Color = Color.Red
            series.Name = "Internal Resistance"
            series.YAxisType = AxisType.Secondary
            chart.Series.Add(series)
        End If


        chartArea.Name = "main"

        chartArea.AxisX.Title = "SoC [%]"
        chartArea.AxisX.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.AxisY.Title = "Voltage [V]"
        chartArea.AxisY.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.AxisY2.Title = "Internal Resistance [Ω]"
        chartArea.AxisY2.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisY2.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisY2.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisY2.MajorGrid.LineDashStyle = ChartDashStyle.Dot

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


    Private Sub btnBrowseSoCCurve_Click(sender As Object, e As EventArgs) Handles btnBrowseSoCCurve.Click
        If BatterySoCCurveFileBrowser.OpenDialog(FileRepl(tbSoCCurve.Text, GetPath(_batteryFile))) Then _
            tbSoCCurve.Text = GetFilenameWithoutDirectory(BatterySoCCurveFileBrowser.Files(0), GetPath(_batteryFile))
    End Sub

    Private Sub btnBrowseRiMap_Click(sender As Object, e As EventArgs) Handles btnBrowseRiMap.Click
        If BatteryInternalResistanceCurveFileBrowser.OpenDialog(FileRepl(tbRiCurve.Text, GetPath(_batteryFile))) Then _
            tbRiCurve.Text = GetFilenameWithoutDirectory(BatteryInternalResistanceCurveFileBrowser.Files(0), GetPath(_batteryFile))
    End Sub

    Private Sub btnSoCCurveOpen_Click(sender As Object, e As EventArgs) Handles btnSoCCurveOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbSoCCurve.Text, GetPath(_batteryFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbSoCCurve.Text, GetPath(_batteryFile)), theFile)
        Else
            OpenFiles(FileRepl(tbSoCCurve.Text, GetPath(_batteryFile)))
        End If
    End Sub

    Private Sub btnRiMapOpen_Click(sender As Object, e As EventArgs) Handles btnRiMapOpen.Click
        Dim theFile As String

        theFile = FileRepl(tbRiCurve.Text, GetPath(_batteryFile))

        If theFile <> NoFile AndAlso File.Exists(theFile) Then
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)), theFile)
        Else
            OpenFiles(FileRepl(tbRiCurve.Text, GetPath(_batteryFile)))
        End If
    End Sub
End Class
