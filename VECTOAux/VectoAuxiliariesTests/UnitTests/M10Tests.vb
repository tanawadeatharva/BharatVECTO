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
	Public Class M10Test
		<Test()> _
		<TestCase(8, 8, 0, 2, 4, 2, 5, 5.75F, 3.5F)>
		Public Sub InterpolationTests(x1 As Double, y1 As Double, x2 As Double, y2 As Double, x3 As Double, y3 As Double,
									xTAir As Double, out1 As Double, out2 As Double)

			Dim m3 As New Mock(Of IM3_AveragePneumaticLoadDemand)
			Dim m9 As New Mock(Of IM9)
			Dim signals As New Signals() ' Not required , here for expansion only.

			m3.Setup(Function(x) x.AverageAirConsumedPerSecondLitre).Returns(xTAir.SI(Of NormLiterPerSecond))
			m9.Setup(Function(x) x.LitresOfAirCompressorOnContinually).Returns(x1.SI(Of NormLiter))
			m9.Setup(Function(x) x.TotalCycleFuelConsumptionCompressorOnContinuously).Returns(y1.SI(Of Gram))
			'x2 is not an output of m9, an is allways zero, but to keep in line with schematic, is represented anyway although it is a constant.
			m9.Setup(Function(x) x.TotalCycleFuelConsumptionCompressorOffContinuously).Returns(y2.SI(Of Gram))
			m9.Setup(Function(x) x.LitresOfAirCompressorOnOnlyInOverrun).Returns(x3.SI(Of NormLiter))
			m9.Setup(Function(x) x.TotalCycleFuelConsumptionCompressorOffContinuously).Returns(y3.SI(Of Gram))


			Dim target As IM10 = New M10(m3.Object, m9.Object, signals)

			target.CycleStep(1.SI(Of Second))

			Assert.AreEqual(out1, target.AverageLoadsFuelConsumptionInterpolatedForPneumatics.Value(), 0.001)
			Assert.AreEqual(out2, target.FuelConsumptionSmartPneumaticsAndAverageElectricalPowerDemand.Value(), 0.001)
		End Sub
	End Class
End Namespace


