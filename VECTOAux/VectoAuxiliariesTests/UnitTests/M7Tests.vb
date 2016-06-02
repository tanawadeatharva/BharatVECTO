Imports VectoAuxiliaries.Electrics
Imports VectoAuxiliaries.Pneumatics
Imports VectoAuxiliaries.Hvac
Imports VectoAuxiliaries.DownstreamModules
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliaries

Namespace UnitTests
	<TestFixture()>
	Public Class M7Test
		Private M5 As M5_Mock
		Private M6 As M6_Mock
		Private Signals As ISignals

		Public Sub New()

			M5 = New M5_Mock(100, 110, 120)
			M6 = New M6_Mock(100, 0, 0, 110, 120, 0, 130, 140, 150)
			Signals = New Signals()
		End Sub

		<Test()>
		Public Sub CreateNew_M7InstanceTest()
			Dim target As IM7 = New M7(M5, M6, Signals)
			Assert.IsNotNull(target)
		End Sub

		'IP1  M5 : Alternators Generation Power At Crank (Traction ) (Single )
		'IP2  M5 : Alternators Generation Power At Crank (Idle )     (Single )
		'IP3  Signals : IDLE            (Boolean )
		'IP4  M6      :Overrun Flag     (Integer )
		'IP5  Signals : Clutch engaged  (Boolean )
		'IP6  Signals : InNuetral       (Boolean )
		'IP7  M6      : SmartElectricalAndPneumatic:AlternatorPowerGen@Crank
		'IP8  M6      : SmartElectricalAndPneumatic:AirCompPowerGen@Crank
		'IP9  M6      : SmartElectricalOnly:AlternatorPowerGen@Crank
		'IP10 M6      : AveragePowerDemand@CrankFromPneumatics
		'IP11 M6      : SmartPneumaticsOnly:AirComprPowerGen@Crank
		'OP1  Op1     :Smart Electrical & Pneumatic Aux : Alternator power gen @ Crank
		'OP2  OP2     :Smart Electrical & Pneumatic Aux : Air comp   power gen @ Crank
		'OP3  OP3     :Smart Electrical Aux : Alternator             Power Gen @ Crank
		'OP4  OP4     :Smart Electrical Aux : Ait Compressor         Power Gen @ Crank 
		<Test()> _
		<TestCase(100, 200, False, 0, False, True, 300, 400, 500, 600, 700, 200, 600, 200, 600)> _
		<TestCase(100, 200, True, 0, False, True, 300, 400, 500, 600, 700, 200, 600, 200, 600)> _
		<TestCase(100, 200, False, 1, True, False, 300, 400, 500, 600, 700, 300, 400, 500, 700)>
		Public Sub InputOutputTests(ByVal IP1 As Double,
									ByVal IP2 As Double,
									ByVal IP3 As Boolean,
									ByVal IP4 As Double,
									ByVal IP5 As Boolean,
									ByVal IP6 As Boolean,
									ByVal IP7 As Double,
									ByVal IP8 As Double,
									ByVal IP9 As Double,
									ByVal IP10 As Double,
									ByVal IP11 As Double,
									ByVal OP1 As Double,
									ByVal OP2 As Double,
									ByVal OP3 As Double,
									ByVal OP4 As Double)

			'Instantiate new mocks.
			M5 = New M5_Mock()
			M6 = New M6_Mock()
			Signals = New Signals()

			'Assign from TestCaseValues
			M5._AlternatorsGenerationPowerAtCrankTractionOnWatts = IP1.SI(Of Watt)()
			M5._AlternatorsGenerationPowerAtCrankIdleWatts = IP2.SI(Of Watt)()
			Signals.Idle = IP3
			M6._OverrunFlag = IP4
			Signals.ClutchEngaged = IP5
			Signals.InNeutral = IP6
			M6._SmartElecAndPneumaticAltPowerGenAtCrank = IP7.SI(Of Watt)()
			M6._SmartElecAndPneumaticAirCompPowerGenAtCrank = IP8.SI(Of Watt)()
			M6._SmartElecOnlyAltPowerGenAtCrank = IP9.SI(Of Watt)()
			M6._AveragePowerDemandAtCrankFromPneumatics = IP10.SI(Of Watt)()
			M6._SmartPneumaticOnlyAirCompPowerGenAtCrank = IP11.SI(Of Watt)()


			'Create Instance of M7 from 
			Dim target As IM7 = New M7(M5, M6, Signals)

			Dim OP1act As Double = target.SmartElectricalAndPneumaticAuxAltPowerGenAtCrank().Value()
			Dim OP2act As Double = target.SmartElectricalAndPneumaticAuxAirCompPowerGenAtCrank().Value()
			Dim OP3act As Double = target.SmartElectricalOnlyAuxAltPowerGenAtCrank().Value()
			Dim OP4act As Double = target.SmartPneumaticOnlyAuxAirCompPowerGenAtCrank.Value()

			Assert.AreEqual(OP1, OP1act, 0.001)
			Assert.AreEqual(OP2, OP2act, 0.001)
			Assert.AreEqual(OP3, OP3act, 0.001)
			Assert.AreEqual(OP4, OP4act, 0.001)


			Assert.IsNotNull(target)
		End Sub
	End Class
End Namespace


