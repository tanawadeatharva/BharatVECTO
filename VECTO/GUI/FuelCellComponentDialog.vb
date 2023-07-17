Imports System.IO

Public Class FuelCellComponentDialog
    Private Sub btnOpenFuelCellComponent_Click(sender As Object, e As EventArgs) Handles btnOpenFuelCellComponent.Click
        'Dim f As String
        'f = FileRepl(tbFuelCellComponent.Text, GetPath(_vehFile))

        ''Thus Veh-file is returned
        'FuelCellForm.JobDir = GetPath(_vehFile)
        'BatteryForm.AutoSendTo = True

        'If Not Trim(f) = "" Then
        '    If Not File.Exists(f) Then
        '        MsgBox("File not found!")
        '        Exit Sub
        '    End If
        'End If

        'If Not BatteryForm.Visible Then
        '    BatteryForm.Show()
        'Else
        '    If BatteryForm.WindowState = FormWindowState.Minimized Then BatteryForm.WindowState = FormWindowState.Normal
        '    BatteryForm.BringToFront()
        'End If

        'If Not Trim(f) = "" Then
        '    Try
        '        BatteryForm.OpenBatteryFile(f)
        '    Catch ex As Exception
        '        MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading FuelCell File")
        '    End Try
        'End If
    End Sub
End Class