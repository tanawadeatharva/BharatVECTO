Imports System.Drawing.Imaging
Imports System.IO
Imports System.Security.Cryptography
Imports System.Windows.Forms.DataVisualization.Charting
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents
Imports TUGraz.VectoCore.Utils

Public Class FuelCellComponentForm
    Private _fuelCellComponentFile As String = ""
    Public AutoSendTo As Boolean = False
    Public JobDir As String = ""
    Private _changed As Boolean = False

    Public ReadOnly Property BatteryFile As String
        Get
            Return _fuelCellComponentFile
        End Get
    End Property


    Protected Overrides Sub OnFormClosing(e As FormClosingEventArgs)
        MyBase.OnFormClosing(e)
    End Sub


    Private Sub FuelCellFormLoad(sender As Object, e As EventArgs) Handles Me.Load
        _changed = False

        NewFuelCellComponent()
    End Sub

    Private Sub NewFuelCellComponent()
        If ChangeCheckCancel() Then Exit Sub

        tbMaxElectricPower.Text = ""
        tbManufacturer.Text = ""
        tbModel.Text = ""
        tbMassFlowMap.Text = ""
        tbMinElectricPower.Text = ""


        _fuelCellComponentFile = ""
        _changed = False

        LbStatus.Text = ""
        UpdatePic()



    End Sub

    Private Function FillFuelCellComponent(file As String) As FuelCellComponent
        Dim fcC = New FuelCellComponent

        fcC.FilePath = file
        fcC.Model = tbModel.Text
        fcC.Manufacturer = tbManufacturer.Text
        fcC.MinElectricPower = (tbMinElectricPower.Text.ToDouble(0) / 1000).SI(Of Watt)
        fcC.MaxElectricPower = (tbMaxElectricPower.Text.ToDouble(0) / 1000).SI(Of Watt)
        fcC.MassFlowMapFile = Path.Combine(Path.GetDirectoryName(_fuelCellComponentFile), tbMassFlowMap.Text) 'Absolute


        Return fcC
    End Function


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


    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _fuelCellComponentFile = "" Or saveAs Then
            If FuelCellComponentFileBrowser.SaveDialog(_fuelCellComponentFile) Then
                _fuelCellComponentFile = FuelCellComponentFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveFuelCellComponentToFile(_fuelCellComponentFile)
    End Function

    Private Function SaveFuelCellComponentToFile(file As String) As Boolean
        Dim fcC = FillFuelCellComponent(file)
        If Not fcC.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        If AutoSendTo Then
            If FuelCellComponentDialog.Visible Then
                If UCase(FileRepl(FuelCellComponentDialog.tbFuelCellComponent.Text, JobDir)) <> UCase(file) Then _
                    FuelCellComponentDialog.tbFuelCellComponent.Text = GetFilenameWithoutDirectory(file, JobDir)
                'VectoJobForm.UpdatePic()
            End If
        End If

        FuelCellComponentFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _changed = False
        Return True
    End Function

    Public Sub OpenFuelCellFile(file As String)
        If ChangeCheckCancel() Then Exit Sub


        Dim basePath As String = Path.GetDirectoryName(file)
        Dim fcComponent As IFuelCellComponentEngineeringInputData = JSONInputDataFactory.ReadFuelCellComponentEngineeringInputData(file, False)

        'Check if saved in declaration mode?

        tbModel.Text = fcComponent.Model
        tbManufacturer.Text = fcComponent.Manufacturer
        tbMaxElectricPower.Text = (fcComponent.MaxElectricPower * 0.001).ToGUIFormat()
        tbMinElectricPower.Text = (fcComponent.MinElectricPower * 0.001).ToGUIFormat()
        tbMassFlowMap.Text = GetRelativePath(fcComponent.MassFlowMap.Source, basePath)


        FuelCellComponentFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _fuelCellComponentFile = file
        Activate()
        _changed = False

        UpdatePic()

    End Sub

    Private Sub UpdatePic()
        Dim massFlowMap As FuelCellMassFlowMap = Nothing

        pcBoxMassFlowMap.Image = Nothing

        Try
            Dim massFlowMapFile As String =
                    If(Not String.IsNullOrWhiteSpace(_fuelCellComponentFile), Path.Combine(Path.GetDirectoryName(_fuelCellComponentFile),
                                                                                           tbMassFlowMap.Text), tbMassFlowMap.Text)
            If File.Exists(massFlowMapFile) Then _
                massFlowMap = FuelCellMassFlowMapReader.Create(VectoCSVFile.Read(massFlowMapFile))
        Catch ex As Exception

        End Try

        If (massFlowMap Is Nothing) Then Exit Sub

        Dim chart As Chart = New Chart
        chart.Width = pcBoxMassFlowMap.Width
        chart.Height = pcBoxMassFlowMap.Height

        Dim chartArea As ChartArea = New ChartArea

        Dim series = New Series
        series.Points.DataBindXY(massFlowMap.Entries.Select(Function(e) e.P_el_out.Value() / 1000).ToArray(),
                                 massFlowMap.Entries.Select(Function(x) x.H2.Value() * 3600 * 1000).ToArray())

        series.ChartType = SeriesChartType.FastLine
        series.BorderWidth = 2
        series.Color = Color.DarkCyan
        series.Name = "Massflow (" & tbMassFlowMap.Text & ")"
        chart.Series.Add(series)

        chartArea.Name = "main"

        chartArea.AxisX.Title = "Power [kW]"
        chartArea.AxisX.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisX.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisX.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisX.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.AxisY.Title = "H2 [g/h]"
        chartArea.AxisY.TitleFont = New Font("Helvetica", 10)
        chartArea.AxisY.LabelStyle.Font = New Font("Helvetica", 8)
        chartArea.AxisY.LabelAutoFitStyle = LabelAutoFitStyles.None
        chartArea.AxisY.MajorGrid.LineDashStyle = ChartDashStyle.Dot

        chartArea.BackColor = Color.GhostWhite
        chart.ChartAreas.Add(chartArea)
        chart.Update()

        Dim img As Bitmap = New Bitmap(chart.Width, chart.Height, PixelFormat.Format32bppArgb)
        chart.DrawToBitmap(img, New Rectangle(0, 0, pcBoxMassFlowMap.Width, pcBoxMassFlowMap.Height))


        pcBoxMassFlowMap.Image = img


    End Sub

    Private Sub btnSaveFuelCellComponent_Click(sender As Object, e As EventArgs) Handles btnSaveFuelCellComponent.Click
        If SaveOrSaveAs(False) Then
            DialogResult = DialogResult.OK
            Close()
        End If
    End Sub

    Private Sub btnCancel_Click(sender As Object, e As EventArgs) Handles btnCancel.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub btnBrowseMassFlowMap_Click(sender As Object, e As EventArgs) Handles btnBrowseMassFlowMap.Click
        If MassFlowMapFileBrowser.OpenDialog(FileRepl(tbMassFlowMap.Text, GetPath(_fuelCellComponentFile))) Then _
            tbMassFlowMap.Text = GetFilenameWithoutDirectory(MassFlowMapFileBrowser.Files(0), GetPath(_fuelCellComponentFile))
    End Sub

End Class
