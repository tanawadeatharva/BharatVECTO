Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils

Namespace UnitTests
	<TestFixture()>
	Public Class M0_5_SmartAlternatorSetEfficiencyTests
		Private target As M0_5_SmartAlternatorSetEfficiency
		Private signals = New Signals

		Public Sub New()

			Initialise()
		End Sub


		Private Function GetSSM() As ISSMTOOL


			Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
			'Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

			Dim ssm As ISSMTOOL = New SSMTOOL(_SSMMAP, New HVACConstants())


			ssm.Load(_SSMMAP)


			Return ssm
		End Function

		Private Sub Initialise()


			Dim ssm As ISSMTOOL = GetSSM()
			Dim elecConsumers As New ElectricalConsumerList(26.3, 0.096, True)

			'Dim  hvacMap As New HVACMap("testFiles\TestHvacMap.csv")
			'hvacMap.Initialise()
			Dim alternatoMap As New AlternatorMap("testFiles\testAlternatormap.aalt")
			alternatoMap.Initialise()

			Dim signals = New Signals()
			signals.EngineSpeed = 2000.RPMtoRad()

			Dim m0 As New M0_NonSmart_AlternatorsSetEfficiency(elecConsumers, alternatoMap, 26.3.SI(Of Volt), signals, ssm)

			'Results Cards
			Dim readings = New List(Of SmartResult)
			readings.Add(New SmartResult(10, 8))
			readings.Add(New SmartResult(70, 63))

			Dim idleResult As New ResultCard(readings)
			Dim tractionResult As New ResultCard(readings)
			Dim overrunResult As New ResultCard(readings)


			signals.EngineSpeed = 2000.RPMtoRad()
			target = New M0_5_SmartAlternatorSetEfficiency(m0, elecConsumers, alternatoMap, idleResult, tractionResult,
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


