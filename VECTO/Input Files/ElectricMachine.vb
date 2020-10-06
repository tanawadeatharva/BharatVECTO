Imports System.Collections.Generic
Imports System.ComponentModel.DataAnnotations
Imports System.IO
Imports System.Linq
Imports TUGraz.VECTO.Input_Files
Imports TUGraz.VectoCommon.Exceptions
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent
Imports TUGraz.VectoCore.Models.SimulationComponent.Data
Imports TUGraz.VectoCore.Utils

<CustomValidation(GetType(ElectricMachine), "ValidateEngine")>
Public Class ElectricMachine
    Implements IElectricMotorEngineeringInputData

    Private ReadOnly _fullLoadCurvePath As SubPath
    Private ReadOnly _dragCurvePath As SubPath
    Private ReadOnly _efficiencyMap As SubPath

    ''' <summary>
    ''' Directory of engine file. Defined in FilePath property (Set)
    ''' </summary>
    ''' <remarks></remarks>
    Friend _myPath As String

    ''' <summary>
    ''' Full file path. Needs to be defined via FilePath property before calling ReadFile or SaveFile.
    ''' </summary>
    ''' <remarks></remarks>
    Private _filePath As String

    Public ModelName As String
    Public MotorInertia As Double
    Public PeakPowerTime As Double
    Public ContPwr As Double
    Public RatedSpeed As Double

    ''' <summary>
    ''' New instance. Initialise
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub New()
        _myPath = ""
        _filePath = ""

        _fullLoadCurvePath = New SubPath
        _dragCurvePath = New SubPath()
        _efficiencyMap = New SubPath()


        SetDefault()
    End Sub

    Private Sub SetDefault()
        ModelName = "Undefined"
        MotorInertia = 0


        _fullLoadCurvePath.Clear()

    End Sub

    Public Function SaveFile() As Boolean

        Dim validationResults As IList(Of ValidationResult) =
                Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), Nothing, False)

        If validationResults.Count > 0 Then
            Dim messages As IEnumerable(Of String) =
                    validationResults.Select(Function(r) r.ErrorMessage + String.Join(", ", r.MemberNames.Distinct()))
            MsgBox("Invalid input." + Environment.NewLine + String.Join(Environment.NewLine, messages), MsgBoxStyle.OkOnly,
                   "Failed to save engine")
            Return False
        End If

        Try
            Dim writer As JSONFileWriter = New JSONFileWriter()
            writer.SaveElectricMotor(Me, _filePath, Cfg.DeclMode)

        Catch ex As Exception
            MsgBox("Faled to write Engine file: " + ex.Message)
            Return False
        End Try
        Return True
    End Function


    Public Shared Function ValidateEngine(engine As ElectricMachine, validationContext As ValidationContext) As ValidationResult
        Dim engineData As ElectricMotorData


        Dim modeService As VectoValidationModeServiceContainer =
                TryCast(validationContext.GetService(GetType(VectoValidationModeServiceContainer)),
                        VectoValidationModeServiceContainer)
        Dim mode As ExecutionMode = If(modeService Is Nothing, ExecutionMode.Declaration, modeService.Mode)
        Dim emsCycle As Boolean = (modeService IsNot Nothing) AndAlso modeService.IsEMSCycle
        Dim gbxType As GearboxType? = If(modeService Is Nothing, GearboxType.MT, modeService.GearboxType)

        Try
            'If mode = ExecutionMode.Declaration Then
            '    Dim doa As DeclarationDataAdapterHeavyLorry = New DeclarationDataAdapterHeavyLorry()

            '    'engineData = doa.create(DummyVehicle, engine.EngineModes.First(), New Mission() With {.MissionType = MissionType.LongHaul})
            'Else
            '    Dim doa As EngineeringDataAdapter = New EngineeringDataAdapter()
            '    Dim dummyVehicle As IVehicleEngineeringInputData = New DummyVehicle() With {
            '            .IVehicleComponentsEngineering_EngineInputData = engine
            '            }
            '    engineData = doa.CreateElectricMachines(engine)
            'End If

            'Dim result As IList(Of ValidationResult) =
            '        engineData.Validate(If(Cfg.DeclMode, ExecutionMode.Declaration, ExecutionMode.Engineering), gbxType, emsCycle)

            'If Not result.Any() Then Return ValidationResult.Success

            'Return New ValidationResult("Engine Configuration is invalid. ",
            '                            result.Select(Function(r) r.ErrorMessage + String.Join(Environment.NewLine, r.MemberNames)).ToList())
            Return ValidationResult.Success
        Catch ex As Exception
            Return New ValidationResult(ex.Message)
        End Try
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

    Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
        Get
            Dim retVal As DataSource = New DataSource()
            retVal.SourceType = DataSourceType.JSONFile
            retVal.SourceFile = FilePath
            Return retVal
        End Get
    End Property

    Public ReadOnly Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
        Get
            Return Cfg.DeclMode
        End Get
    End Property

    Public ReadOnly Property Manufacturer As String Implements IComponentInputData.Manufacturer
        Get

        End Get
    End Property

    Public ReadOnly Property Model As String Implements IComponentInputData.Model
        Get
            Return ModelName
        End Get
    End Property

    Public ReadOnly Property [Date] As Date Implements IComponentInputData.[Date]
        Get
            Return Now.ToUniversalTime()
        End Get
    End Property

    Public ReadOnly Property AppVersion As String Implements IComponentInputData.AppVersion
        Get
            Return "VECTO-GUI"
        End Get
    End Property

    Public ReadOnly Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
        Get
            Return CertificationMethod.NotCertified
        End Get
    End Property
    Public ReadOnly Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
        Get
            Return TUGraz.VectoCore.Configuration.Constants.NOT_AVailABLE
        End Get
    End Property
    Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property FullLoadCurve As TableData Implements IElectricMotorDeclarationInputData.FullLoadCurve
        Get
            If Not File.Exists(_fullLoadCurvePath.FullPath) Then _
                Throw New VectoException("Full-Load Curve is missing or invalid")
            Return VectoCSVFile.Read(_fullLoadCurvePath.FullPath)
        End Get
    End Property
    Public ReadOnly Property DragCurve As TableData Implements IElectricMotorDeclarationInputData.DragCurve
        Get
            If Not File.Exists(_dragCurvePath.FullPath) Then _
                Throw New VectoException("Drag Curve is missing or invalid")
            Return VectoCSVFile.Read(_dragCurvePath.FullPath)
        End Get
    End Property
    Public ReadOnly Property EfficiencyMap As TableData Implements IElectricMotorDeclarationInputData.EfficiencyMap
        Get
            If Not File.Exists(_efficiencyMap.FullPath) Then _
                Throw New VectoException("Drag Curve is missing or invalid")
            Return VectoCSVFile.Read(_efficiencyMap.FullPath)
        End Get
    End Property
    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IElectricMotorDeclarationInputData.Inertia
        Get
            Return MotorInertia.SI(Of KilogramSquareMeter)
        End Get
    End Property

    Public ReadOnly Property OverloadTime As Second Implements IElectricMotorDeclarationInputData.OverloadTime
    get
            Return PeakPowerTime.SI(of Second)
    End Get
    End Property

    Public ReadOnly Property ConinuousPowerSpeed As PerSecond Implements IElectricMotorDeclarationInputData.ContinuousPowerSpeed
    get
            Return RatedSpeed.RPMtoRad()
    End Get
    End Property

    Public Property OverloadRecoveryFactor As Double Implements IElectricMotorDeclarationInputData.OverloadRecoveryFactor

    Public ReadOnly Property ContinuousPower As Watt Implements IElectricMotorDeclarationInputData.ContinuousPower
    get
        Return ContPwr.si(of Watt)
    End Get
    End Property

    Public Property PathMaxTorque(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _fullLoadCurvePath.OriginalPath
            Else
                Return _fullLoadCurvePath.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _fullLoadCurvePath.Init(_myPath, value)
        End Set
    End Property

    Public Property PathDrag(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _dragCurvePath.OriginalPath
            Else
                Return _dragCurvePath.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _dragCurvePath.Init(_myPath, value)
        End Set
    End Property

    Public Property PathMap(Optional ByVal original As Boolean = False) As String
        Get
            If original Then
                Return _efficiencyMap.OriginalPath
            Else
                Return _efficiencyMap.FullPath
            End If
        End Get
        Set(ByVal value As String)
            _efficiencyMap.Init(_myPath, value)
        End Set
    End Property

End Class