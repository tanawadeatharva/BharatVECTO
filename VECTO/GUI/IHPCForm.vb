Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class IHPCForm

    Private _changed as Boolean
    Private _dragCurveFilePath as String
    Private _flCurveFilePath1 as String
    Private _flCurveFilePath2 as String

#Region "Set JSON Data"

    Public Sub ReadIHPCFile(file As String)

        Dim inputProvider = New JSONComponentInputData(file, Nothing)
        Dim ihpcData = inputProvider.ElectricMachines.Entries.First().ElectricMachine

        tbModel.Text = ihpcData.Model
        tbInertia.Text = ihpcData.Inertia.ToGUIFormat()
        tbThermalOverload.Text = ihpcData.OverloadRecoveryFactor.ToGUIFormat()
        tbDragCurve.Text = ihpcData.DragCurve.Source

        SetVoltageLevelLow(ihpcData.VoltageLevels.First()) 
        SetVoltageLevelHigh(ihpcData.VoltageLevels.Last())
    End Sub

    Private Sub SetVoltageLevelLow(voltageLevel as IElectricMotorVoltageLevel)

        tbVoltage1.Text = voltageLevel.VoltageLevel.ToGUIFormat()
        tbContinousTorque1.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
        tbContinousTorqueSpeed1.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
        tbOverloadTime1.Text = voltageLevel.OverloadTime.ToGUIFormat()
        tbOverloadTorque1.Text = voltageLevel.OverloadTorque.ToGUIFormat()
        tboverloadTorqueSpeed1.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
        tbFLCurve1.Text = voltageLevel.FullLoadCurve.Source
        SetPowerMapEntries(_lvPowerMap1, voltageLevel.PowerMap)

    End Sub

    Private Sub SetVoltageLevelHigh(voltageLevel as IElectricMotorVoltageLevel)

        tbVoltage2.Text = voltageLevel.VoltageLevel.ToGUIFormat()
        tbContinousTorque2.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
        tbContinousTorqueSpeed2.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
        tbOverloadTime2.Text = voltageLevel.OverloadTime.ToGUIFormat()
        tbOverloadTorque2.Text = voltageLevel.OverloadTorque.ToGUIFormat()
        tboverloadTorqueSpeed1.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
        tbFLCurve2.Text = voltageLevel.FullLoadCurve.Source
        SetPowerMapEntries(_lvPowerMap2, voltageLevel.PowerMap)

    End Sub

    Private Sub SetPowerMapEntries(powerMapListView As ListView, entries As IList(Of IElectricMotorPowerMap))
        For Each entry As IElectricMotorPowerMap In entries
            Dim listEntry = CreateListViewItem(entry.Gear, entry.PowerMap.Source)
            powerMapListView.Items.Add(listEntry)
        Next
    End Sub

    Private Function CreateListViewItem(axleNumber As Integer, filepath As String) As ListViewItem
        Dim retVal As New ListViewItem
        retVal.SubItems(0).Text = axleNumber.ToGUIFormat()
        retVal.SubItems.Add(filepath)
        Return retVal
    End Function
    
#End Region

    Public Sub ClearIHPC()

        tbModel.Text = ""
        tbInertia.Text = ""
        tbThermalOverload.Text = ""
        tbDragCurve.Text = ""

        tbVoltage1.Text = ""
        tbContinousTorque1.Text = ""
        tbContinousTorqueSpeed1.Text = ""
        tbOverloadTime1.Text = ""
        tbOverloadTorque1.Text = ""
        tboverloadTorqueSpeed1.Text = ""
        tbFLCurve1.Text = ""
        RemoveAllListViewItems(_lvPowerMap1)

        tbVoltage2.Text = ""
        tbContinousTorque2.Text = ""
        tbContinousTorqueSpeed2.Text = ""
        tbOverloadTime2.Text = ""
        tbOverloadTorque2.Text = ""
        tboverloadTorqueSpeed1.Text = ""
        tbFLCurve2.Text = ""
        RemoveAllListViewItems(_lvPowerMap2)
        
        LbStatus.Text = ""
        _changed = False

    End Sub

    Private Sub RemoveAllListViewItems(listView As ListView)
        If listView.Items.Count = 0 Then
            Exit Sub
        Else
            For Each listItem As ListViewItem In listView.Items
                listItem.Remove()
            Next
        End If
    End Sub

#Region "Events"

    Private Sub btDragCurve_Click(sender As Object, e As EventArgs) Handles btDragCurve.Click
        If IHPCDragCurveFileBrowser.OpenDialog(FileRepl(tbDragCurve.Text, GetPath(_dragCurveFilePath)))
            _tbDragCurve.Text = GetFilenameWithoutDirectory(IHPCDragCurveFileBrowser.Files(0), GetPath(_dragCurveFilePath))
        End If
    End Sub
    
    Private Sub btFLCurveFile1_Click(sender As Object, e As EventArgs) Handles btFLCurveFile1.Click
        If IHPCFullLoadCurveFileBrowser.OpenDialog(FileRepl(tbFLCurve1.Text, GetPath(_flCurveFilePath1)))
            _tbFLCurve1.Text = GetFilenameWithoutDirectory(IHPCFullLoadCurveFileBrowser.Files(0), GetPath(_flCurveFilePath1))
        End If
    End Sub
    
    Private Sub btFLCurveFile2_Click(sender As Object, e As EventArgs) Handles btFLCurveFile2.Click
        If IHPCFullLoadCurveFileBrowser.OpenDialog(FileRepl(tbFLCurve2.Text, GetPath(_flCurveFilePath2)))
            _tbFLCurve2.Text = GetFilenameWithoutDirectory(IHPCFullLoadCurveFileBrowser.Files(0), GetPath(_flCurveFilePath2))
        End If
    End Sub

    Private Sub btAddPowerMap1_Click(sender As Object, e As EventArgs) Handles btAddPowerMap1.Click
        AddListViewItem(lvPowerMap1)
    End Sub
    
    Private Sub btAddPowerMap2_Click(sender As Object, e As EventArgs) Handles btAddPowerMap2.Click
        AddListViewItem(lvPowerMap2)
    End Sub
    
    Private Sub AddListViewItem(listView As ListView)
        If IHPCPowerMapInputDialog.ShowDialog() = DialogResult.OK Then
            Dim gear = Convert.ToInt32(IHPCPowerMapInputDialog.tbGear.Text)
            Dim filePath = IHPCPowerMapInputDialog.tbInputFile.Text
            listView.Items.Add(CreateListViewItem(gear, filePath))
        End If
    End Sub

    Private Sub btRemovePowerMap1_Click(sender As Object, e As EventArgs) Handles btRemovePowerMap1.Click
        RemoveListEntry(lvPowerMap1)
    End Sub
    
    Private Sub btRemovePowerMap2_Click(sender As Object, e As EventArgs) Handles btRemovePowerMap2.Click
        RemoveListEntry(lvPowerMap2)
    End Sub

    Private Sub RemoveListEntry(listView As ListView)
        If listView.Items.Count = 0 Then
            Exit Sub
        Else
            listView.Items(listView.Items.Count - 1).Remove()
        End If
    End Sub

    Private Sub lvPowerMap1_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap1.DoubleClick
        EditEntry(lvPowerMap1)
    End Sub

    Private Sub lvPowerMap2_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap2.DoubleClick
        EditEntry(lvPowerMap2)
    End Sub


    Private Sub EditEntry( listView As ListView)
        If listView.SelectedItems.Count = 0 Then Exit Sub
        Dim entry As ListViewItem = listView.SelectedItems(0)
        
        IHPCPowerMapInputDialog.tbGear.Text = entry.SubItems(0).Text
        If entry.SubItems.Count = 2 Then
            IHPCPowerMapInputDialog.tbInputFile.Text = entry.SubItems(1).Text
        End If
        IHPCPowerMapInputDialog.tbGear.Focus()

        If IHPCPowerMapInputDialog.ShowDialog() = DialogResult.OK Then
            entry.SubItems(0).Text = IHPCPowerMapInputDialog.tbGear.Text
            entry.SubItems(1).Text = IHPCPowerMapInputDialog.tbInputFile.Text
        End If

    End Sub
    
    Private Sub btSave_Click(sender As Object, e As EventArgs) Handles btSave.Click

    End Sub

    Private Sub btCancel_Click(sender As Object, e As EventArgs) Handles btCancel.Click

    End Sub
    
#End Region

End Class