Imports System.IO
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.InputData.Impl

Public Class IEPCForm

    Public JobDir As String = ""
    Private _iepcFilePath as String = ""
    Private _powerMapDlg As IEPCInputDialog
    Private _dragCurveDlg As IEPCInputDialog
    Private _gearDlg As IEPCGearInputDialog
    Private _flcFilePath1 as String
    Private _flcFilePath2 as String
    Private _changed as Boolean
    
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
        _iepcFilePath = file

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
            Dim listEntry = CreateListViewItem(entry.Ratio, entry.MaxOutputShaftTorque, entry.MaxOutputShaftSpeed)
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

    Private Function CreateListViewItem(ratio As Double, outputShaftTorque As NewtonMeter, outputShaftSpeed As PerSecond) As ListViewItem

        Dim retVal As New ListViewItem
        retVal.SubItems(0).Text = ratio.ToGUIFormat()
        retVal.SubItems.Add(outputShaftTorque?.ToGUIFormat())
        retVal.SubItems.Add(outputShaftSpeed?.ToGUIFormat())
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

    Private Sub btFLCurveFile1_Click(sender As Object, e As EventArgs) Handles btFLCurveFile1.Click
        If IEPCFLCFileBrowser.OpenDialog(FileRepl(tbFLCurve1.Text, GetPath(_flcFilePath1))) Then
            tbFLCurve1.Text = GetFilenameWithoutDirectory(IEPCFLCFileBrowser.Files(0), GetPath(_flcFilePath1))
        End If
    End Sub

    Private Sub btFLCurveFile2_Click(sender As Object, e As EventArgs) Handles btFLCurveFile2.Click
        If IEPCFLCFileBrowser.OpenDialog(FileRepl(tbFLCurve2.Text, GetPath(_flcFilePath2))) Then
            tbFLCurve2.Text = GetFilenameWithoutDirectory(IEPCFLCFileBrowser.Files(0), GetPath(_flcFilePath2))
        End If
    End Sub

    Private Sub ToolStripBtNew_Click(sender As Object, e As EventArgs) Handles ToolStripBtNew.Click
        NewIEPC()
    End Sub

    Private Sub ToolStripBtOpen_Click(sender As Object, e As EventArgs) Handles ToolStripBtOpen.Click
        If IEPCFileBrowser.OpenDialog(_iepcFilePath) Then
            Try
                ReadIEPCFile(IEPCFileBrowser.Files(0))
            Catch ex As Exception
                MsgBox(ex.Message, MsgBoxStyle.OkOnly, "Error loading Engine File")
            End Try
        End If
    End Sub
    
    #Region "Toolbar"

    Private Sub NewIEPC()
        tbModel.Text = ""
        tbInertia.Text = ""
        cbDifferentialIncluded.Checked = False
        cbDifferentialIncluded.Checked = False
        tbNumberOfDesignTypeWheelMotor.Text = ""
        tbThermalOverload.Text = ""

        tbVoltage1.Text = ""
        tbContinousTorque1.Text = ""
        tbContinousTorqueSpeed1.Text = ""
        tbOverloadTime1.Text = ""
        tbOverloadTorque1.Text = ""
        tboverloadTorqueSpeed1.Text = ""
        tbFLCurve1.Text = ""
        _flcFilePath1 = ""
        RemoveAllListViewItems(lvPowerMap1)
        
        tbVoltage2.Text = ""
        tbContinousTorque2.Text = ""
        tbContinousTorqueSpeed2.Text = ""
        tbOverloadTime2.Text = ""
        tbOverloadTorque2.Text = ""
        tboverloadTorqueSpeed2.Text = ""
        tbFLCurve2.Text = ""
        _flcFilePath2 = ""
        RemoveAllListViewItems(lvPowerMap2)
        
        RemoveAllListViewItems(lvDragCurve)
        RemoveAllListViewItems(lvGear)

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

    Private Sub ToolStripBtSave_Click(sender As Object, e As EventArgs) Handles ToolStripBtSave.Click
        SaveOrSaveAs(False)
    End Sub

    Private Sub ToolStripBtSaveAs_Click(sender As Object, e As EventArgs) Handles ToolStripBtSaveAs.Click
        SaveOrSaveAs(True)
    End Sub

    Private Sub ToolStripBtSendTo_Click(sender As Object, e As EventArgs) Handles ToolStripBtSendTo.Click
        If ChangeCheckCancel() Then Exit Sub

        If _iepcFilePath = "" Then
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

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_iepcFilePath, JobDir)
    End Sub

    Private Sub ToolStripButton1_Click(sender As Object, e As EventArgs) Handles ToolStripButton1.Click
        If File.Exists(Path.Combine(MyAppPath, "User Manual\help.html")) Then
            Dim defaultBrowserPath As String = BrowserUtils.GetDefaultBrowserPath()
            Process.Start(defaultBrowserPath,
                          $"""file://{Path.Combine(MyAppPath, "User Manual\help.html#engine-editor")}""")
        Else
            MsgBox("User Manual not found!", MsgBoxStyle.Critical)
        End If
    End Sub
    
    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _iepcFilePath = "" Or saveAs Then
            If IEPCFileBrowser.SaveDialog(_iepcFilePath) Then
                _iepcFilePath = IEPCFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveIEPCToFile(_iepcFilePath)
    End Function
    
    Private Function SaveIEPCToFile(ByVal file As String) As Boolean

        Dim iepc  = new IEPCInputData
        iepc.FilePath = file
        iepc.ModelName = tbModel.Text
        iepc.InertiaValue = tbInertia.Text.ToDouble().SI(of KilogramSquareMeter)
        iepc.DifferentialIncludedValue = cbDifferentialIncluded.Checked
        iepc.DesignTypeWheelMotorValue = cbDesignTypeWheelMotor.Checked
        If tbNumberOfDesignTypeWheelMotor.Text = "" Then
            iepc.NrOfDesignTypeWheelMotorMeasuredValue = Nothing
        Else if IsNumeric(tbNumberOfDesignTypeWheelMotor.Text)
            iepc.NrOfDesignTypeWheelMotorMeasuredValue = tbNumberOfDesignTypeWheelMotor.Text.ToInt()
        End If
        iepc.OverloadRecoveryFactorValue = tbThermalOverload.Text.ToDouble()


        If Not iepc.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If


        _changed = False

        Return True
    End Function

    Private Function GetFirstVoltageLevel() As IElectricMotorVoltageLevel
        Dim voltageLevel = new ElectricMotorVoltageLevel()

        voltageLevel.VoltageLevel = tbVoltage1.Text.ToDouble().SI(Of Volt)
        voltageLevel.ContinuousTorque = tbContinousTorque1.Text.ToDouble().SI(of NewtonMeter)
        voltageLevel.ContinuousTorqueSpeed = tbContinousTorque1.Text.ToDouble().RPMtoRad()
        voltageLevel.OverloadTime = tbOverloadTime1.Text.ToDouble().SI(Of Second)
        voltageLevel.OverloadTorque = tbOverloadTorque1.Text.ToDouble().SI(of NewtonMeter)
        voltageLevel.OverloadTestSpeed = tboverloadTorqueSpeed1.Text.ToDouble().RPMtoRad()

        Return Nothing
        
    End Function

    
#End Region


    Private Function ChangeCheckCancel() As Boolean

        If _changed Then
            Select Case MsgBox("Save changes ?", MsgBoxStyle.YesNoCancel)
                Case MsgBoxResult.Yes
                    'Return Not SaveOrSaveAs(False)
                Case MsgBoxResult.Cancel
                    Return True
                Case Else
                    _changed = False
                    Return False
            End Select
        Else
            Return False
        End If

    End Function


    Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click

    End Sub

    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click

    End Sub
End Class