
Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC


Namespace UnitTests
	<TestFixture()>
	Public Class M1_AverageHVACLoadDemandTests
		Private Const _GOODMAP As String = "TestFiles\testAlternatorMap.aalt"
		Private Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
		Private Const _BusDatabase As String = "TestFiles\BusDatabase.abdb"

		Private signals As ISignals = New Signals With {.EngineSpeed = 2000.RPMtoRad()}
		Private powernetVoltage As Volt = 26.3.SI(of Volt)
		Private ssm As ISSMTOOL = New SSMTOOL(_SSMMAP, New HVACConstants())


		Private m0 As IM0_NonSmart_AlternatorsSetEfficiency
		Private alternatorMap As IAlternatorMap 
		Private alternatorGearEfficiency As Single = 0.8
		Private compressorGrearEfficiency As Single = 0.8

       
        Public Sub New()
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)
            alternatorMap = New AlternatorMap(_GOODMAP)
			alternatorMap.Initialise()

            CType(ssm.GenInputs, SSMGenInputs)._vehicle.Height = 0.SI(of Meter)

			ssm.Load(_SSMMAP)

			m0 = New M00Impl(New ElectricalConsumerList(powernetVoltage, 0.096, True),
														alternatorMap, powernetVoltage, signals, ssm)
		End Sub

		Private Function GETM1Instance() As IM1_AverageHVACLoadDemand

			ssm.Load(_SSMMAP)

			Return New M01Impl(m0, alternatorGearEfficiency,
												compressorGrearEfficiency,
												powernetVoltage,
												signals,
												ssm)
		End Function


		<Test()>
		Public Sub CreateNew()

			Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()

			Assert.NotNull(target)
		End Sub


		<Test()>
		Public Sub GetAveragePowerDemandAtCrankFromHVACMechanicsWattsTest()


			Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
			Dim expected As Single = 1580.276
			Dim actual As Watt = target.AveragePowerDemandAtCrankFromHVACMechanicalsWatts

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<Test()>
		Public Sub AveragePowerDemandAtCrankFromHVACElectricsWattsTest()


			Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
			Dim expected As Single = 0
			Dim actual As Watt = target.AveragePowerDemandAtCrankFromHVACElectricsWatts

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<Test()>
		Public Sub AveragePowerDemandAtAlternatorFromHVACElectricsWattsTest()


			Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
			Dim expected As Single = 0
			Dim actual As Watt = target.AveragePowerDemandAtAlternatorFromHVACElectricsWatts

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<Test()>
		Public Sub HVACFuelingLitresPerHourTest()


			Dim target As IM1_AverageHVACLoadDemand = GETM1Instance()
			Dim expected As Single = 0
			Dim actual As KilogramPerSecond = target.HVACFuelingLitresPerHour()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub
	End Class
End Namespace


