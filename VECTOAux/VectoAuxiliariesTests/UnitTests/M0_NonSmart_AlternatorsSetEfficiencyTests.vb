Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports System.IO
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC
Imports Signals = TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.Signals

Namespace UnitTests
    <TestFixture()>
    Public Class M0_NonSmart_AlternatorsSetEfficiencyTests
        Private Const cstrAlternatorsEfficiencyMapLocation As String = "tests\testAlternatorMap.aalt"
        Private Const cstrHVACMapLocation As String = "TestFiles\TestHvacMap.csv"
        Private Const cstrAlternatorMap As String = "TestFiles\testAlternatorMap.aalt"

        Private elecConsumers As IElectricalConsumerList

        Private alternatorMap As IAlternatorMap
        Private signals As Signals = New Signals
        Private powernetVoltage As Volt = 26.3.SI(Of Volt)()
        Private ssm As IHVACSteadyStateModel = New HVACSteadyStateModel(100, 100, 100)

        Private Function GetSSM() As ISSMTOOL

            Const _SSMMAP As String = "TestFiles\ssm.Ahsm"

            Dim ssm As SSMTOOL = New SSMTOOL(_SSMMAP, New HVACConstants())
            CType(ssm.SSMInputs, SSMInputs)._vehicle.Height = 0.SI(of Meter)
            ssm.Load(_SSMMAP)

            Return ssm
        End Function

        Public Sub New()

            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)

            signals.EngineSpeed = 2000.RPMtoRad()

            'Setup consumers and HVAC ( 1 Consumer in Test Category )
            elecConsumers = CType(New ElectricalConsumerList(0.096.SI(of Volt), 26.3), IElectricalConsumerList)
            elecConsumers.AddConsumer(New ElectricalConsumer(False, "TEST", "CONSUMER1", 20.SI(of Ampere), 0.5,
                                                            26.3.SI(of Volt), 1, ""))

            'Alternator Map
            alternatorMap = CType(New AlternatorMap(cstrAlternatorMap), IAlternatorMap)
            alternatorMap.Initialise()
        End Sub

        <OneTimeSetUp>
        Public Sub RunBeforeAnyTests()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
        End Sub

        <Test()>
        Public Sub CreateNewTest()
            Dim target As IM0_NonSmart_AlternatorsSetEfficiency = New M00Impl(elecConsumers,alternatorMap, powernetVoltage, signals, GetSSM())
            Assert.IsNotNull(target)
        End Sub

        <Test()>
        Public Sub CreateNew_MissingElecConsumers_ThrowArgumentExceptionTest()

            Dim target As IM0_NonSmart_AlternatorsSetEfficiency
            Assert.That(Sub() target = New M00Impl(Nothing, alternatorMap, powernetVoltage, signals, GetSSM()), Throws.InstanceOf(Of ArgumentException))
        End Sub

        <Test()>
        Public Sub CreateNew_MissingAlternatorMap_ThrowArgumentExceptionTest()
            Dim target As IM0_NonSmart_AlternatorsSetEfficiency
            Assert.That(Sub() target = New M00Impl(elecConsumers, Nothing, powernetVoltage, signals, GetSSM()), Throws.InstanceOf(Of ArgumentException))
        End Sub

        <Test()>
        Public Sub EfficiencyValueTest()
            Dim target As M00Impl = New M00Impl(elecConsumers,
                                                                                                        alternatorMap, powernetVoltage, signals, GetSSM())

            Dim actual As Single = target.AlternatorsEfficiency

            Dim expected As Single = 0.62

            Assert.AreEqual(expected, actual, 0.001)
        End Sub

        <Test()>
        Public Sub HVAC_PowerDemandAmpsTest()

            Dim target As IM0_NonSmart_AlternatorsSetEfficiency = New M00Impl(elecConsumers,
                                                                                                        alternatorMap, powernetVoltage, signals, GetSSM())

            Dim actual As Ampere
            Dim expected As Single = 0

            actual = target.GetHVACElectricalPowerDemandAmps()

            Assert.AreEqual(expected, actual.Value(), 0.001)
        End Sub
    End Class
End Namespace


