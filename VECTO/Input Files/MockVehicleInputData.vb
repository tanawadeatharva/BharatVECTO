Imports System.Collections.Generic
Imports TUGraz.VectoCommon.InputData
Imports TUGraz.VectoCommon.Models
Imports TUGraz.VectoCommon.Utils

Public Class MockVehicleInputData
    Implements IVehicleDeclarationInputData
    Public Property DataSource As DataSource Implements IComponentInputData.DataSource
    Public Property SavedInDeclarationMode As Boolean Implements IComponentInputData.SavedInDeclarationMode
    Public Property Manufacturer As String Implements IComponentInputData.Manufacturer
    Public Property Model As String Implements IComponentInputData.Model
    Public Property [Date] As String Implements IComponentInputData.[Date]
    Public Property CertificationMethod As CertificationMethod Implements IComponentInputData.CertificationMethod
    Public Property CertificationNumber As String Implements IComponentInputData.CertificationNumber
    Public Property DigestValue As DigestData Implements IComponentInputData.DigestValue
    Public Property Identifier As String Implements IVehicleDeclarationInputData.Identifier
    Public Property ExemptedVehicle As Boolean Implements IVehicleDeclarationInputData.ExemptedVehicle
    Public Property VIN As String Implements IVehicleDeclarationInputData.VIN
    Public Property LegislativeClass As LegislativeClass Implements IVehicleDeclarationInputData.LegislativeClass
    Public Property VehicleCategory As VehicleCategory Implements IVehicleDeclarationInputData.VehicleCategory
    Public Property AxleConfiguration As AxleConfiguration Implements IVehicleDeclarationInputData.AxleConfiguration
    Public Property CurbMassChassis As Kilogram Implements IVehicleDeclarationInputData.CurbMassChassis
    Public Property GrossVehicleMassRating As Kilogram Implements IVehicleDeclarationInputData.GrossVehicleMassRating
    Public Property TorqueLimits As IList(Of ITorqueLimitInputData) Implements IVehicleDeclarationInputData.TorqueLimits
    Public Property ManufacturerAddress As String Implements IVehicleDeclarationInputData.ManufacturerAddress
    Public Property EngineIdleSpeed As PerSecond Implements IVehicleDeclarationInputData.EngineIdleSpeed
    Public Property VocationalVehicle As Boolean Implements IVehicleDeclarationInputData.VocationalVehicle
    Public Property SleeperCab As Boolean Implements IVehicleDeclarationInputData.SleeperCab
    Public Property TankSystem As TankSystem? Implements IVehicleDeclarationInputData.TankSystem

    Public Property ADAS As IAdvancedDriverAssistantSystemDeclarationInputData _
        Implements IVehicleDeclarationInputData.ADAS

    Public Property ZeroEmissionVehicle As Boolean Implements IVehicleDeclarationInputData.ZeroEmissionVehicle
    Public Property HybridElectricHDV As Boolean Implements IVehicleDeclarationInputData.HybridElectricHDV
    Public Property DualFuelVehicle As Boolean Implements IVehicleDeclarationInputData.DualFuelVehicle
    Public Property MaxNetPower1 As Watt Implements IVehicleDeclarationInputData.MaxNetPower1
    Public Property MaxNetPower2 As Watt Implements IVehicleDeclarationInputData.MaxNetPower2
    Public Property Components As IVehicleComponentsDeclaration Implements IVehicleDeclarationInputData.Components
End Class