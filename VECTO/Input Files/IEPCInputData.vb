Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils

Public Class IEPCInputData 
    Implements IIEPCEngineeringInputData

    Public FilePath As String
    Public ModelName As String
    Public InertiaValue As KilogramSquareMeter
    Public DifferentialIncludedValue As Boolean
    Public DesignTypeWheelMotorValue As Boolean
    Public NrOfDesignTypeWheelMotorMeasuredValue As Integer?
    Public OverloadRecoveryFactorValue As Double

    Public GearValues As IList(Of IGearEntry)
    Public VoltageLevelValues As IList(Of IElectricMotorVoltageLevel)
    Public DragCurveValues As  IList(Of IDragCurve)
    

    public Function SaveFile() As Boolean
        Return False
    End Function


    
    Public ReadOnly Property DataSource As DataSource Implements IComponentInputData.DataSource
        Get
            Dim retVal = New DataSource()
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
            Return Nothing
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
            Return VectoCore.Configuration.Constants.NOT_AVAILABLE
        End Get
    End Property

    Public ReadOnly Property DigestValue As DigestData Implements IComponentInputData.DigestValue
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property ElectricMachineType As ElectricMachineType Implements IIEPCDeclarationInputData.ElectricMachineType
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property R85RatedPower As Watt Implements IIEPCDeclarationInputData.R85RatedPower
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property Inertia As KilogramSquareMeter Implements IIEPCDeclarationInputData.Inertia
        Get
            Return InertiaValue
        End Get
    End Property

    Public ReadOnly Property DifferentialIncluded As Boolean Implements IIEPCDeclarationInputData.DifferentialIncluded
        Get
            Return DifferentialIncludedValue
        End Get
    End Property

    Public ReadOnly Property DesignTypeWheelMotor As Boolean Implements IIEPCDeclarationInputData.DesignTypeWheelMotor
        Get
            Return DesignTypeWheelMotorValue
        End Get
    End Property

    Public ReadOnly Property NrOfDesignTypeWheelMotorMeasured As Integer? Implements IIEPCDeclarationInputData.NrOfDesignTypeWheelMotorMeasured
        Get
            Return NrOfDesignTypeWheelMotorMeasuredValue
        End Get
    End Property

    Public ReadOnly Property Gears As IList(Of IGearEntry) Implements IIEPCDeclarationInputData.Gears
        Get
            Return GearValues
        End Get
    End Property

    Public ReadOnly Property VoltageLevels As IList(Of IElectricMotorVoltageLevel) Implements IIEPCDeclarationInputData.VoltageLevels
        Get
            Return VoltageLevelValues
        End Get
    End Property

    Public ReadOnly Property DragCurves As IList(Of IDragCurve) Implements IIEPCDeclarationInputData.DragCurves
        Get
            Return DragCurveValues
        End Get
    End Property

    Public ReadOnly Property Conditioning As TableData Implements IIEPCDeclarationInputData.Conditioning
        Get
            Return Nothing
        End Get
    End Property

    Public ReadOnly Property OverloadRecoveryFactor As Double Implements IIEPCEngineeringInputData.OverloadRecoveryFactor
        Get
            Return OverloadRecoveryFactorValue
        End Get
    End Property
End Class
