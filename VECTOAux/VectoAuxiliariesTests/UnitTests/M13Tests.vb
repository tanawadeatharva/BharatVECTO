Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq


Namespace UnitTests
    <TestFixture()> _
    Public Class M13Tests

        Private Const FUEL_DENSITY_percm3 As Single = 0.835


        '<TestCase(50,	60,	70,	TRUE,	TRUE ,100, 1,False,	 72287.5f , 86.57185629f )> _

        <Test()> _
        <TestCase(50, 60, 70, False, False, 100, 1, False, 60.0F, 86.57185F)> _
        <TestCase(50, 60, 70, False, True, 100, 1, False, 50.0F, 86.55988F)> _
        <TestCase(50, 60, 70, True, False, 100, 1, False, 70.0F, 86.58383F)> _
        <TestCase(50, 60, 70, True, True, 100, 1, False, 60.0F, 86.57185F)> _
        <TestCase(50, 60, 70, True, True, 100, 2, True, 120.0F, 173.1437F)> _
        Public Sub InputOutputValues(IP1 As Single,
                              IP2 As Single,
                              IP3 As Single,
                              IP4 As Boolean,
                              IP5 As Boolean,
                              IP6 As Single,
                              IP7 As Single,
                              IP8 As Boolean,
                              OUT1 As Single,
                              OUT2 As Single)

            'Arrange
            Dim m10 As New Mock(Of IM10)
            Dim m11 As New Mock(Of IM11)
            Dim m12 As New Mock(Of IM12)
            Dim Signals As New Mock(Of ISignals)

            m12.Setup(Function(x) x.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand).Returns(IP1)
            m12.Setup(Function(x) x.BaseFuelConsumptionWithTrueAuxiliaryLoads).Returns(IP2)
            m10.Setup(Function(x) x.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand).Returns(IP3)
            Signals.Setup(Function(x) x.SmartPneumatics).Returns(IP4)
            Signals.Setup(Function(x) x.SmartElectrics).Returns(IP5)
            Signals.Setup(Function(x) x.WHTC).Returns(IP7)
            Signals.Setup(Function(x) x.DeclarationMode).Returns(IP8)
            Signals.Setup(Function(x) x.TotalCycleTimeSeconds).Returns(3114)
            Signals.Setup(Function(x) x.CurrentCycleTimeInSeconds).Returns(3114)

            'Act
            Dim target = New M13(m10.Object, m11.Object, m12.Object, Signals.Object)

            'Assert
            Assert.AreEqual(OUT1, target.WHTCTotalCycleFuelConsumptionGrams)

        End Sub

    End Class

End Namespace



