Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports VectoAuxiliaries
Imports Moq
Imports TUGraz.VectoCommon.Utils


Namespace UnitTests
	<TestFixture()>
	Public Class M13Tests
		Private Const FUEL_DENSITY_percm3 As Single = 0.835


		'<TestCase(50,	60,	70,	TRUE,	TRUE ,100, 1,False,	 72287.5f , 86.57185629f )> _

		<Test()> _
		<TestCase(50, 60, 70, False, False, 100, 1, False, 60.0F)> _
		<TestCase(50, 60, 70, False, True, 100, 1, False, 110.0F)> _
		<TestCase(50, 60, 70, True, False, 100, 1, False, 0.0F)> _
		<TestCase(50, 60, 70, True, True, 100, 1, False, 110.0F)> _
		<TestCase(50, 60, 70, True, True, 100, 2, True, 220.0F)>
		Public Sub InputOutputValues(IP1 As Double,
									IP2 As Double,
									IP3 As Double,
									IP4 As Boolean,
									IP5 As Boolean,
									IP6 As Double,
									IP7 As Double,
									IP8 As Boolean,
									OUT1 As Double)

			'Arrange
			Dim m10 As New Mock(Of IM10)
			Dim m11 As New Mock(Of IM11)
			Dim m12 As New Mock(Of IM12)
			Dim Signals As New Mock(Of ISignals)

			m12.Setup(Function(x) x.FuelconsumptionwithsmartElectricsandAveragePneumaticPowerDemand()).Returns(IP1.SI(Of Gram))
			m12.Setup(Function(x) x.BaseFuelConsumptionWithTrueAuxiliaryLoads()).Returns(IP2.SI(Of Gram))
			m10.Setup(Function(x) x.AverageLoadsFuelConsumptionInterpolatedForPneumatics).Returns(0.SI(Of Gram))
			m10.Setup(Function(x) x.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand).Returns(IP3.SI(Of Gram))
			m11.Setup(Function(x) x.TotalCycleFuelConsuptionAverageLoads).Returns(0.SI(Of Gram))
			Signals.Setup(Function(x) x.SmartPneumatics).Returns(IP4)
			Signals.Setup(Function(x) x.SmartElectrics).Returns(IP5)
			Signals.Setup(Function(x) x.WHTC).Returns(IP7)
			Signals.Setup(Function(x) x.DeclarationMode).Returns(IP8)
			Signals.Setup(Function(x) x.TotalCycleTimeSeconds).Returns(3114)
			Signals.Setup(Function(x) x.CurrentCycleTimeInSeconds).Returns(3114)

			'Act
			Dim target = New M13(m10.Object, m11.Object, m12.Object, Signals.Object)

			'Assert
			Assert.AreEqual(OUT1, target.WHTCTotalCycleFuelConsumptionGrams.Value(), 0.001)
		End Sub
	End Class
End Namespace


