
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
Public Class HybridStrategyParamsForm
    Private _strategyParamsFile As String = ""
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
        If EngineFileBrowser.OpenDialog(_strategyParamsFile) Then
            Try
                OpenHybridStrategyParametersFile(EngineFileBrowser.Files(0))
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

        If _strategyParamsFile = "" Then
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

        VectoJobForm.TbENG.Text = GetFilenameWithoutDirectory(_strategyParamsFile, JobDir)
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


        tbEquivalenceFactor.Text = ""
        tbMinSoC.Text = ""
        tbMaxSoC.Text = ""
        tbTargetSoC.Text = ""
        tbauxBufferTime.Text = ""
        tbAuxBufferChargeTime.Text = ""

        DeclInit()

        _strategyParamsFile = ""
        Text = "Hybrid Strategy Parameters Editor"
        LbStatus.Text = ""

        _changed = False
    End Sub

    'Open VENG file
    Public Sub OpenHybridStrategyParametersFile(file As String)
        Dim strategyParams As IHybridStrategyParameters

        If ChangeCheckCancel() Then Exit Sub

        Dim inputData As IEngineeringInputDataProvider = TryCast(JSONInputDataFactory.ReadComponentData(file),
                                                                IEngineeringInputDataProvider)

        strategyParams = inputData.JobInputData.HybridStrategyParameters

        If Not Cfg.DeclMode Then
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

        tbEquivalenceFactor.Text = strategyParams.EquivalenceFactor.ToGUIFormat()

        tbEquivalenceFactor.Text = strategyParams.EquivalenceFactor.ToGUIFormat()
        tbMinSoC.Text = (strategyParams.MinSoC * 100).ToGUIFormat()
        tbMaxSoC.Text = (strategyParams.MaxSoC * 100).ToGUIFormat()
        tbTargetSoC.Text = (strategyParams.MaxSoC * 100).ToGUIFormat()

        'tbAuxBufferChargeTime = strategyParams.
        'tbauxBufferTime = strategyParams.

        DeclInit()

        BatteryFileBrowser.UpdateHistory(file)
        Text = GetFilenameWithoutPath(file, True)
        LbStatus.Text = ""
        _strategyParamsFile = file
        Activate()

        _changed = False
    End Sub

    'Save or Save As function = true if file is saved
    Private Function SaveOrSaveAs(ByVal saveAs As Boolean) As Boolean
        If _strategyParamsFile = "" Or saveAs Then
            If HCUFileBrowser.SaveDialog(_strategyParamsFile) Then
                _strategyParamsFile = HCUFileBrowser.Files(0)
            Else
                Return False
            End If
        End If
        Return SaveParamsToFile(_strategyParamsFile)
    End Function

    'Save VENG file to given filepath. Called by SaveOrSaveAs. 
    Private Function SaveParamsToFile(ByVal file As String) As Boolean

        Dim strategyParams As HybridStrategyParams = New HybridStrategyParams
        strategyParams.FilePath = file


        strategyParams.EquivalenceFactor = tbEquivalenceFactor.Text.ToDouble(0)

        strategyParams.MinSoC = tbMinSoC.Text.ToDouble(0) / 100
        strategyParams.MaxSoC = tbMaxSoC.Text.ToDouble(0) / 100
        strategyParams.TargetSoC = tbTargetSoC.Text.ToDouble(0) / 100

        strategyParams.AuxBufferTime = tbauxBufferTime.Text.ToDouble(0)
        strategyParams.AuxBufferChgTime = tbAuxBufferChargeTime.Text.ToDouble(0)

        If Not strategyParams.SaveFile Then
            MsgBox("Cannot save to " & file, MsgBoxStyle.Critical)
            Return False
        End If

        HCUFileBrowser.UpdateHistory(file)
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






#End Region



    'Save and close
    Private Sub ButOK_Click(sender As Object, e As EventArgs) Handles ButOK.Click
        If SaveOrSaveAs(False) Then Close()
    End Sub

    'Close without saving (see FormClosing Event)
    Private Sub ButCancel_Click(sender As Object, e As EventArgs) Handles ButCancel.Click
        Close()
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

    Private Sub tbMinSoC_TextChanged(sender As Object, e As EventArgs) Handles tbMinSoC.TextChanged
        Change()
    End Sub

    Private Sub tbMaxSoC_TextChanged(sender As Object, e As EventArgs) Handles tbMaxSoC.TextChanged
        Change()
    End Sub

    Private Sub tbTargetSoC_TextChanged(sender As Object, e As EventArgs) Handles tbTargetSoC.TextChanged
        Change()
    End Sub

    Private Sub tbauxBufferTime_TextChanged(sender As Object, e As EventArgs) Handles tbauxBufferTime.TextChanged
        Change()
    End Sub

    Private Sub tbAuxBufferChargeTime_TextChanged(sender As Object, e As EventArgs) Handles tbAuxBufferChargeTime.TextChanged
        Change()
    End Sub

    Private Sub tbEquivalenceFactor_TextChanged(sender As Object, e As EventArgs) Handles tbEquivalenceFactor.TextChanged
        Change()
    End Sub

#End Region





End Class
