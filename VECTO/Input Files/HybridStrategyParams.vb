Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils

Public Class HybridStrategyParams
    Implements IHybridStrategyParameters

    Private _filePath As String
    Private _myPath As String
    Public MinimumIceOnTime As Double

    Public Sub New()
        _myPath = ""
        _filePath = ""




        SetDefault()
    End Sub

    Private Sub SetDefault()

    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save strategy parameters")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = New JSONFileWriter()
            writer.SaveStrategyParameters(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Faled to write Strategy Parameters file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function

    Public Property FilePath() As String
        Get
            Return _filePath
        End Get
        Set(ByVal value As String)
            _filePath = value
            If _filePath = "" Then
                _myPath = ""
            Else
                _myPath = Path.GetDirectoryName(_filePath) & "\"
            End If
        End Set
    End Property

    Public Property EquivalenceFactor As Double

    Public Property MinSoC As Double

    Public Property MaxSoC As Double

    Public Property TargetSoC As Double

    Public ReadOnly Property Source As String Implements IHybridStrategyParameters.Source
        Get
            Return FilePath
        End Get
    End Property

    Public ReadOnly Property MinICEOnTime As Second Implements IHybridStrategyParameters.MinimumICEOnTime
        Get
            Return MinimumICEOnTime.SI(of Second)
        End Get
    End Property

    Public ReadOnly Property AuxBufferTime As Second Implements IHybridStrategyParameters.AuxBufferTime
        Get
            Return AuxiliaryBufferTime.SI(Of Second)
        End Get
    End Property

    Public ReadOnly Property AuxBufferChargeTime As Second Implements IHybridStrategyParameters.AuxBufferChargeTime
        Get
            Return AuxiliaryBufferChgTime.SI(of Second)
        End Get
    End Property

    Public Property AuxiliaryBufferTime As Double

    Public Property AuxiliaryBufferChgTime As Double

    Private ReadOnly Property IHybridStrategyParameters_EquivalenceFactor As Double Implements IHybridStrategyParameters.EquivalenceFactor
        Get
            Return EquivalenceFactor
        End Get
    End Property

    Private ReadOnly Property IHybridStrategyParameters_MinSoC As Double Implements IHybridStrategyParameters.MinSoC
        Get
            Return MinSoC
        End Get
    End Property

    Private ReadOnly Property IHybridStrategyParameters_MaxSoC As Double Implements IHybridStrategyParameters.MaxSoC
        Get
            Return MaxSoC
        End Get
    End Property

    Private ReadOnly Property IHybridStrategyParameters_TargetSoC As Double Implements IHybridStrategyParameters.TargetSoC
        Get
            Return TargetSoC
        End Get
    End Property
End Class