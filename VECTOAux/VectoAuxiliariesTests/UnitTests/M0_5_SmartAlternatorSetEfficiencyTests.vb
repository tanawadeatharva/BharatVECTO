Imports System.IO
Imports NUnit.Framework
Imports TUGraz.VectoCommon.BusAuxiliaries
Imports TUGraz.VectoCommon.Utils

Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules
Imports TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC


Namespace UnitTests


	<TestFixture()>
	Public Class M0_5_SmartAlternatorSetEfficiencyTests
		Private target As IM0_5_SmartAlternatorSetEfficiency
		Private signals = New Signals

		Public Sub New()

			
		End Sub

        <OneTimeSetUp>
        Sub RunBeforeAnyTests()    
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory)

            Initialise()
        end Sub

		Private Function GetSSM() As ISSMTOOL


			Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
			'Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

			Dim ssm As ISSMTOOL = New SSMTOOL(_SSMMAP, New HVACConstants())
		    CType(ssm.SSMInputs, SSMInputs)._vehicle.Height = 0.SI(of Meter)

			ssm.Load(_SSMMAP)


			Return ssm
		End Function

		Private Sub Initialise()


			Dim ssm As ISSMTOOL = GetSSM()
			Dim elecConsumers As New ElectricalConsumerList(26.3.SI(of Volt), 0.096, True)

			'Dim  hvacMap As New HVACMap("testFiles\TestHvacMap.csv")
			'hvacMap.Initialise()
			Dim alternatoMap As New AlternatorMap("testFiles\testAlternatormap.aalt")
			alternatoMap.Initialise()

			Dim signals = New Signals()
			signals.EngineSpeed = 2000.RPMtoRad()

			Dim m0 As New M00Impl(elecConsumers, alternatoMap, 26.3.SI(Of Volt), signals, ssm)

			'Results Cards
			Dim readings = New List(Of SmartResult)
			readings.Add(New SmartResult(10.SI(of Ampere), 8.SI(of Ampere)))
			readings.Add(New SmartResult(70.SI(of Ampere), 63.SI(of Ampere)))

			Dim idleResult As New ResultCard(readings)
			Dim tractionResult As New ResultCard(readings)
			Dim overrunResult As New ResultCard(readings)


			signals.EngineSpeed = 2000.RPMtoRad()
			target = New M0_5Impl(m0, elecConsumers, alternatoMap, idleResult, tractionResult,
															overrunResult, signals)
		End Sub

		<Test()>
		Public Sub CreateNewTest()
			Initialise()
			Assert.IsNotNull(target)
		End Sub

		<Test()>
		Public Sub SmartIdleCurrentTest()
			Initialise()
			Assert.IsNotNull(target)
		End Sub

		<Test()>
		Public Sub SmartTractionCurrentTest()
			Initialise()
			Assert.IsNotNull(target)
		End Sub

		<Test()>
		Public Sub SmartOverrunCurrentTest()
			Initialise()
			Assert.IsNotNull(target)
		End Sub

		<Test()>
		Public Sub AlternatorsEfficiencyIdle2000rpmTest()
			Initialise()

			Dim expected As Double = 0.6308339
			Dim actual As Double = target.AlternatorsEfficiencyIdleResultCard()

			Assert.AreEqual(expected, actual, 0.000001)
		End Sub


		<Test()>
		Public Sub AlternatorsEfficiencyTraction2000rpmTest()
			Initialise()

			Dim expected As Double = 0.6308339
			Dim actual As Double = target.AlternatorsEfficiencyTractionOnResultCard()

			Assert.AreEqual(expected, actual, 0.000001)
		End Sub


		<Test()>
		Public Sub AlternatorsEfficiencyOverrun2000rpmTest()
			Initialise()

			Dim expected As Double = 0.6308339
			Dim actual As Double = target.AlternatorsEfficiencyOverrunResultCard()

			Assert.AreEqual(expected, actual, 0.000001)
		End Sub

       
	End Class
End Namespace


