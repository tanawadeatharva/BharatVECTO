Imports DownstreamModules.HVAC
Imports Electrics
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.Electrics
Imports TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Hvac
Imports ISignals = TUGraz.VectoCore.BusAuxiliaries.Interfaces.ISignals
Imports Signals = TUGraz.VectoCore.BusAuxiliaries.Interfaces.Signals

Namespace UnitTests
	<TestFixture()>
	Public Class M5_SmartAlternatorSetGenerationTests
		'Constants
		Private Const _powerNetVoltage As Single = 26.3
		Private Const _hvacMap As String = "testFiles\TestHvacMap.csv"
		Private Const _altMap As String = "testFiles\testAlternatormap.aalt"
		Private Const _rpm As Integer = 2000
		Private Const _altGearPullyEfficiency As Single = 0.8

		'Private fields
		Private _m05 As IM0_5_SmartAlternatorSetEfficiency
		Private _target As IM5_SmartAlternatorSetGeneration
		Private _signals As ISignals = New Signals
		Private ssmHVac As IHVACSteadyStateModel = New HVACSteadyStateModel(100, 100, 100)


		Private Function GetSSM() As ISSMTOOL


			Const _SSMMAP As String = "TestFiles\ssm.Ahsm"
			'Const _BusDatabase As String ="TestFiles\BusDatabase.abdb

			Dim ssm As ISSMTOOL = New SSMTOOL(_SSMMAP, New HVACConstants())


			ssm.Load(_SSMMAP)


			Return ssm
		End Function

		Private Sub Initialise()

			_signals.EngineSpeed = 2000.RPMtoRad()

			Dim elecConsumers As New ElectricalConsumerList(_powerNetVoltage, 0.096, True)

			Dim alternatoMap As New AlternatorMap(_altMap)
			alternatoMap.Initialise()
			Dim _
				m0 As _
					New M00Impl(elecConsumers, alternatoMap, _powerNetVoltage.SI(Of Volt), _signals,
															GetSSM())

			'Results Cards
			Dim readings = New List(Of SmartResult)
			readings.Add(New SmartResult(10, 8))
			readings.Add(New SmartResult(70, 63))

			Dim idleResult As New ResultCard(readings)
			Dim tractionResult As New ResultCard(readings)
			Dim overrunResult As New ResultCard(readings)

			Dim signals As ISignals = New Signals
			signals.EngineSpeed = 2000.RPMtoRad()

			_m05 = New M0_5Impl(m0, elecConsumers, alternatoMap, idleResult, tractionResult,
														overrunResult, signals)
		End Sub

		<TestCase()>
		Public Sub CreateNewTest()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage.SI(Of Volt), _altGearPullyEfficiency)
			Assert.IsNotNull(_target)
		End Sub

		<TestCase()>
		Public Sub PowerAtCrankIdleWatts()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage.SI(Of Volt), _altGearPullyEfficiency)
			Dim expected As Single = 1641.35791
			Dim actual As Watt = _target.AlternatorsGenerationPowerAtCrankIdleWatts()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<TestCase()>
		Public Sub PowerAtCrankTractionWatts()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage.SI(Of Volt), _altGearPullyEfficiency)
			Dim expected As Single = 1641.35791
			Dim actual As Watt = _target.AlternatorsGenerationPowerAtCrankTractionOnWatts()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub

		<TestCase()>
		Public Sub PowerAtCrankOverrunWatts()

			Initialise()
			_target = New M05Impl(_m05, _powerNetVoltage.SI(Of Volt), _altGearPullyEfficiency)
			Dim expected As Single = 1641.35791F

			Dim actual As Watt = _target.AlternatorsGenerationPowerAtCrankOverrunWatts()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub
	End Class
End Namespace


