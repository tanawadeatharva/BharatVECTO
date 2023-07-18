Imports System.IO
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

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
