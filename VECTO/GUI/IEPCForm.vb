Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON

Public Class IEPCForm
    Public IEPCFilePath As String = ""
    Private _powerMapDlg As IEPCInputDialog
    Private _dragCurveDlg As IEPCInputDialog
    Private _gearDlg As IEPCGearInputDialog


    Private Sub IEPCForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _powerMapDlg = New IEPCInputDialog(IEPCDialogType.PowerMapDialog)
        _dragCurveDlg = New IEPCInputDialog(IEPCDialogType.DragCurveDialog)
        _gearDlg = New IEPCGearInputDialog()
    End Sub

    Public Sub ReadIEPCFile(file As String)
        Dim inputData = JSONInputDataFactory.ReadIEPCEngineeringInputData(file, True)

        tbModel.Text = inputData.Model
        tbInertia.Text = inputData.Inertia.ToGUIFormat()
        cbDifferentialIncluded.Checked = inputData.DifferentialIncluded
        cbDesignTypeWheelMotor.Checked = inputData.DesignTypeWheelMotor
        tbNumberOfDesignTypeWheelMotor.Text = inputData.NrOfDesignTypeWheelMotorMeasured.Value.ToGUIFormat()
        tbThermalOverload.Text = inputData.OverloadRecoveryFactor.ToGUIFormat()

        Dim voltageLevel = inputData.VoltageLevels.First()
        SetFirstVoltageLevel(voltageLevel)
        voltageLevel = inputData.VoltageLevels.Last()
        SetSecondaryVoltageLevel(voltageLevel)
        SetGearEntries(inputData.Gears)
        SetDragEntries(inputData.DragCurves)
    End Sub

    Private Sub SetFirstVoltageLevel(voltageLevel As IElectricMotorVoltageLevel)
        tbVoltage1.Text = voltageLevel.VoltageLevel.ToGUIFormat()
        tbContinousTorque1.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
        tbContinousTorqueSpeed1.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
        tbOverloadTime1.Text = voltageLevel.OverloadTime.ToGUIFormat()
        tbOverloadTorque1.Text = voltageLevel.OverloadTorque.ToGUIFormat()
        tboverloadTorqueSpeed1.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
        tbFLCurve1.Text = voltageLevel.FullLoadCurve.Source
        SetPowerMapEntries(_lvPowerMap1, voltageLevel.PowerMap)
    End Sub

    Private Sub SetSecondaryVoltageLevel(voltageLevel As IElectricMotorVoltageLevel)
        tbVoltage2.Text = voltageLevel.VoltageLevel.ToGUIFormat()
        tbContinousTorque2.Text = voltageLevel.ContinuousTorque.ToGUIFormat()
        tbContinousTorqueSpeed2.Text = voltageLevel.ContinuousTorqueSpeed.AsRPM.ToGUIFormat()
        tbOverloadTime2.Text = voltageLevel.OverloadTime.ToGUIFormat()
        tbOverloadTorque2.Text = voltageLevel.OverloadTorque.ToGUIFormat()
        tbOverloadTorqueSpeed2.Text = voltageLevel.OverloadTestSpeed.AsRPM.ToGUIFormat()
        tbFLCurve2.Text = voltageLevel.FullLoadCurve.Source
        SetPowerMapEntries(_lvPowerMap2, voltageLevel.PowerMap)
    End Sub

    Private Sub SetGearEntries(entries As IList(Of IGearEntry))
        For Each entry As IGearEntry In entries
            Dim listEntry = CreateListViewItem(entry.GearNumber, entry.Ratio, entry.MaxOutputShaftTorque, entry.MaxOutputShaftSpeed)
            _lvGear.Items.Add(listEntry)
        Next
    End Sub

    Private Sub SetDragEntries(entries As IList(Of IDragCurve))
        For Each entry As IDragCurve In entries
            Dim listEntry = CreateListViewItem(entry.Gear.Value, entry.DragCurve.Source)
            _lvDragCurve.Items.Add(listEntry)
        Next
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

    Private Function CreateListViewItem(gearNumber As Integer, ratio As Double, outputShaftTorque As NewtonMeter, outputShaftSpeed As PerSecond) As ListViewItem

        Dim retVal As New ListViewItem
        retVal.SubItems(0).Text = gearNumber.ToGUIFormat()
        retVal.SubItems.Add(ratio.ToGUIFormat())
        retVal.SubItems.Add(outputShaftTorque.ToGUIFormat())
        retVal.SubItems.Add(outputShaftSpeed.ToGUIFormat())
        Return retVal

    End Function

    Private Function CreateListViewItem(ratio As Double, outputShaftTorque As Double?, outputShaftSpeed As Double?) As ListViewItem

        Dim retVal As New ListViewItem
        retVal.SubItems(0).Text = ratio.ToGUIFormat()
        If outputShaftSpeed.HasValue Then
            retVal.SubItems.Add(outputShaftTorque.Value.ToGUIFormat())
        End If
        If outputShaftSpeed.HasValue Then
            retVal.SubItems.Add(outputShaftSpeed.Value.ToGUIFormat())
        End If
        Return retVal

    End Function

    Private Sub btAddPowerMap2_Click(sender As Object, e As EventArgs) Handles btAddPowerMap2.Click
        AddListViewItem(_powerMapDlg, _lvPowerMap2)
    End Sub

    Private Sub btAddPowerMap1_Click(sender As Object, e As EventArgs) Handles btAddPowerMap1.Click
        AddListViewItem(_powerMapDlg, _lvPowerMap1)
    End Sub

    Private Sub btAddDragCurve_Click(sender As Object, e As EventArgs) Handles btAddDragCurve.Click
        AddListViewItem(_dragCurveDlg, _lvDragCurve)
    End Sub

    Private Sub AddListViewItem(dialog As IEPCInputDialog, listView As ListView)

        If (dialog.ShowDialog() = DialogResult.OK) Then
            Dim gear = Convert.ToInt32(dialog.tbGear.Text)
            Dim filePath = dialog.tbInputFile.Text
            listView.Items.Add(CreateListViewItem(gear, filePath))
            dialog.Clear()
        End If
    End Sub

    Private Sub btRemovePowerMap1_Click(sender As Object, e As EventArgs) Handles btRemovePowerMap1.Click
        RemoveListEntry(_lvPowerMap1)
    End Sub

    Private Sub btRemoveDragCurve_Click(sender As Object, e As EventArgs) Handles btRemoveDragCurve.Click
        RemoveListEntry(_lvDragCurve)
    End Sub
    Private Sub btRemoveGear_Click(sender As Object, e As EventArgs) Handles btRemoveGear.Click
        RemoveListEntry(_lvGear)
    End Sub

    Private Sub btRemovePowerMap2_Click(sender As Object, e As EventArgs) Handles btRemovePowerMap2.Click
        RemoveListEntry(_lvPowerMap2)
    End Sub

    Private Sub RemoveListEntry(listView As ListView)
        If listView.SelectedItems.Count = 0 Then
            If listView.Items.Count = 0 Then
                Exit Sub
            Else
                listView.Items(listView.Items.Count - 1).Selected = True
            End If
        End If

        listView.SelectedItems(0).Remove()
    End Sub

    Private Sub btAddGear_Click(sender As Object, e As EventArgs) Handles btAddGear.Click

        If (_gearDlg.ShowDialog() = DialogResult.OK) Then
            Dim ratio = Convert.ToDouble(_gearDlg.tbRatio.Text)
            Dim outputShaftTorque As Double?
            Dim outputShaftSpeed As Double?
            If _gearDlg.tbMaxOutShaftTorque.Text.Length > 0 Then
                outputShaftTorque = Convert.ToDouble(_gearDlg.tbMaxOutShaftTorque.Text)
            End If
            If _gearDlg.tbMaxOutShaftSpeed.Text.Length > 0 Then
                outputShaftSpeed = Convert.ToDouble(_gearDlg.tbMaxOutShaftSpeed.Text)
            End If
            Dim entry = CreateListViewItem(ratio, outputShaftTorque, outputShaftSpeed)
            _lvGear.Items.Add(entry)
            _gearDlg.Clear()
        End If

    End Sub

    Private Sub lvGear_DoubleClick(sender As Object, e As EventArgs) Handles lvGear.DoubleClick

        If lvGear.SelectedItems.Count = 0 Then Exit Sub

        Dim entry As ListViewItem = lvGear.SelectedItems(0)

        _gearDlg.tbRatio.Text = entry.SubItems(0).Text
        _gearDlg.tbMaxOutShaftSpeed.Text = entry.SubItems(1).Text
        _gearDlg.tbMaxOutShaftTorque.Text = entry.SubItems(2).Text
        _gearDlg.tbRatio.Focus()

        If _gearDlg.ShowDialog() = DialogResult.OK Then
            entry.SubItems(0).Text = _gearDlg.tbRatio.Text
            entry.SubItems(1).Text = _gearDlg.tbMaxOutShaftSpeed.Text
            entry.SubItems(2).Text = _gearDlg.tbMaxOutShaftTorque.Text
        End If

    End Sub

    Private Sub lvDragCurve_DoubleClick(sender As Object, e As EventArgs) Handles lvDragCurve.DoubleClick
        EditEntry(_dragCurveDlg, lvDragCurve)

    End Sub

    Private Sub lvPowerMap1_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap1.DoubleClick
        EditEntry(_powerMapDlg, lvPowerMap1)
    End Sub

    Private Sub lvPowerMap2_DoubleClick(sender As Object, e As EventArgs) Handles lvPowerMap2.DoubleClick
        EditEntry(_powerMapDlg, lvPowerMap2)
    End Sub

    Private Sub EditEntry(dialog As IEPCInputDialog, listView As ListView)

        If listView.SelectedItems.Count = 0 Then Exit Sub

        Dim entry As ListViewItem = listView.SelectedItems(0)
        dialog.tbGear.Text = entry.SubItems(0).Text
        dialog.tbInputFile.Text = entry.SubItems(1).Text
        dialog.tbGear.Focus()

        If dialog.ShowDialog() = DialogResult.OK Then
            entry.SubItems(0).Text = dialog.tbGear.Text
            entry.SubItems(1).Text = dialog.tbInputFile.Text
        End If

    End Sub
End Class