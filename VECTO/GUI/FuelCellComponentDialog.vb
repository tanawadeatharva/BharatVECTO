Imports System.IO

Public Class FuelCellComponentDialog
    Public _vehFile As String

    Property VehFile As String
        Get
            Return _vehFile
        End Get
        Set(value As String)
            _vehFile = value
        End Set
    End Property

    'Save and close
    Private Sub OK_Button_Click(sender As Object, e As EventArgs) Handles OK_Button.Click
        If Not 0 < Convert.ToInt32(numFuelCellCount.Text) Then
            MsgBox("Fuel Cell Count has to be >= 1")
            numFuelCellCount.Focus()
            Return
        End If

        DialogResult = DialogResult.OK
        Close()
    End Sub

    'Cancel
    Private Sub Cancel_Button_Click(sender As Object, e As EventArgs) Handles Cancel_Button.Click
        DialogResult = DialogResult.Cancel
        Close()
    End Sub

    Private Sub btnBrowseFuelCellComponent_Click(sender As Object, e As EventArgs) Handles btnBrowseFuelCellComponent.Click
        If FuelCellComponentFileBrowser.OpenDialog(FileRepl(tbFuelCellComponent.Text, GetPath(_vehFile))) Then
            tbFuelCellComponent.Text = GetFilenameWithoutDirectory(FuelCellComponentFileBrowser.Files(0), GetPath(_vehFile))
        End If
    End Sub

    Private Sub btnOpenFuelCellComponent_Click(sender As Object, e As EventArgs) Handles btnOpenFuelCellComponent.Click
        Dim f As String
        f = FileRepl(tbFuelCellComponent.Text, GetPath(_vehFile))
        Dim fuelCellForm = FuelCellComponentForm
        'fuelCellForm.Parent = Me
        'Thus Veh-file is returned
        fuelCellForm.JobDir = GetPath(_vehFile)
        fuelCellForm.AutoSendTo = True

        If Not Trim(f) = "" Then
            If Not File.Exists(f) Then
                MsgBox("File not found!")
                Exit Sub
            End If
        End If


        If Not FuelCellComponentForm.Visible Then
            FuelCellComponentForm.Show()
        Else
            If FuelCellComponentForm.WindowState = FormWindowState.Minimized Then BatteryForm.WindowState = FormWindowState.Normal
            FuelCellComponentForm.BringToFront()
        End If

        If Not Trim(f) = "" Then
            Try
                fuelCellForm.OpenFuelCellFile(f)
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading FuelCell File")
            End Try
        End If
    End Sub

    Public Sub Clear()
        tbFuelCellComponent.Text = ""
        numFuelCellCount.Text = "1"
    End Sub
End Class