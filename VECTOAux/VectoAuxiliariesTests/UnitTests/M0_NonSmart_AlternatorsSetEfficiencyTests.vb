Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports System.IO
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCore.InputData.Reader.ComponentData
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
Imports Signals = TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.Signals
Imports TUGraz.VectoCore.InputData.FileIO.JSON
Imports TUGraz.VectoCore.Models.BusAuxiliaries
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics
Imports TUGraz.VectoCore.Models.Declaration
Imports TUGraz.VectoCore.Models.SimulationComponent.Data

Namespace UnitTests
    <TestFixture()>
    Public Class M0_NonSmart_AlternatorsSetEfficiencyTests
        Private Const cstrAlternatorsEfficiencyMapLocation As String = "tests\testAlternatorMap.aalt"
        Private Const cstrHVACMapLocation As String = "TestFiles\TestHvacMap.csv"
        Private Const cstrAlternatorMap As String = "TestFiles\testAlternatorMap.aalt"

        Private elecConsumers As IElectricalConsumerList

        Private alternatorMap As IAlternatorMap
        Private signals As Signals = New Signals
        Private powernetVoltage As Volt = 26.3.SI (Of Volt)()

        Private Function GetSSM() As ISSMTOOL

            Const _SSMMAP As String = "TestFiles\ssm.Ahsm"

            Dim auxconfig = Utils.GetAuxTestConfig()

            Dim ssm As SSMTOOL = New SSMTOOL(auxconfig.SSMInputs) _
            ', New HVACConstants())
            CType(CType(ssm.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            'ssm.Load(_SSMMAP)

            Return ssm
        End Function

        Public Sub New()

            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)

            signals.EngineSpeed = 2000.RPMtoRad()

            'Setup consumers and HVAC ( 1 Consumer in Test Category )

            Dim list = New List(Of IElectricalConsumer)()
            Dim consumer = New ElectricalConsumer(False, "TEST", "CONSUMER1", 20.SI (Of Ampere), 0.5,
                                                  26.3.SI (Of Volt), 1, "")
            list.Add(consumer)
            elecConsumers = CType(New ElectricalConsumerList(list), IElectricalConsumerList)

            'Alternator Map
            alternatorMap = AlternatorReader.ReadMap(cstrAlternatorMap)
            'alternatorMap.Initialise()
        End Sub

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        Public Sub CreateNewTest()

            Dim auxConfig = utils.GetAuxTestConfig()
            CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            Dim m01 As IM0_1_AverageElectricLoadDemand = New M0_1Impl(Utils.GetAuxTestConfig())
            Dim target As IM0_NonSmart_AlternatorsSetEfficiency = New M00Impl(m01, alternatorMap, powernetVoltage,
                                                                              signals,  New SSMTOOL(auxconfig.SSMInputs))
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub CreateNew_MissingElecConsumers_ThrowArgumentExceptionTest()

            Dim target As IM0_NonSmart_AlternatorsSetEfficiency
            Dim auxConfig = utils.GetAuxTestConfig()
            CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            Assert.That(Sub() target = New M00Impl(Nothing, alternatorMap, powernetVoltage, signals, New SSMTOOL(auxconfig.SSMInputs)),
                        Throws.InstanceOf (Of ArgumentException))
        End Sub

        <Test()>
        Public Sub CreateNew_MissingAlternatorMap_ThrowArgumentExceptionTest()
            Dim target As IM0_NonSmart_AlternatorsSetEfficiency
            Dim auxConfig = utils.GetAuxTestConfig()
            CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)

            Dim m01 As IM0_1_AverageElectricLoadDemand = New M0_1Impl(auxConfig)
            Assert.That(Sub() target = New M00Impl(m01, Nothing, powernetVoltage, signals, New SSMTOOL(auxconfig.SSMInputs)),
                        Throws.InstanceOf (Of ArgumentException))
        End Sub

        <Test()>
        Public Sub EfficiencyValueTest()

            Dim auxConfig = utils.GetAuxTestConfig()

            CType(auxConfig.ElectricalUserInputsConfig, ElectricsUserInputsConfig).ElectricalConsumers = elecConsumers
            CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)

            Dim m01 As IM0_1_AverageElectricLoadDemand = New M0_1Impl(auxConfig)

            Dim target As M00Impl = New M00Impl(m01, alternatorMap, powernetVoltage, signals, New SSMTOOL(auxconfig.SSMInputs))

            Dim actual As Double = target.AlternatorsEfficiency

            Dim expected As Single = 0.62

            Assert.AreEqual(expected, actual, 0.001)
        End Sub

        <Test()>
        Public Sub HVAC_PowerDemandAmpsTest()
            Dim auxConfig = utils.GetAuxTestConfig()
            CType(CType(auxConfig.SSMInputs, SSMInputs).Vehicle, VehicleData).Height=  0.SI (Of Meter)
            Dim m01 As IM0_1_AverageElectricLoadDemand = New M0_1Impl(auxConfig)

            Dim target As IM0_NonSmart_AlternatorsSetEfficiency = New M00Impl(m01, alternatorMap, powernetVoltage,
                                                                              signals, New SSMTOOL(auxconfig.SSMInputs))

            Dim actual As Ampere
            Dim expected As Single = 0

            actual = target.GetHVACElectricalPowerDemandAmps()

            Assert.AreEqual(expected, actual.Value(), 0.001)
        End Sub
    End Class
End Namespace


