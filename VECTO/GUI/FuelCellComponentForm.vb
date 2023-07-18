Imports System.IO
Imports TUGraz.VectoCommon.InputData
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




    Private Sub NewFuelCellComponent()
        If ChangeCheckCancel() Then Exit Sub






    End Sub

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
        Throw New NotImplementedException()
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

        UpdatePic()

    End Sub

    Private Sub UpdatePic()



    End Sub
End Class
