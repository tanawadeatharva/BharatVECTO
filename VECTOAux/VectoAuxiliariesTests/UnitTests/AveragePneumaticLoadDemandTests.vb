
Imports VectoAuxiliaries.Pneumatics
Imports NUnit.Framework
Imports TUGraz.VectoCommon.Utils
Imports VectoAuxiliariesTests.Mocks
Imports VectoAuxiliaries


Namespace UnitTests
	<TestFixture>
	Public Class M3_AveragePneumaticLoadDemandTests
		Private _pneumaticUserInputsConfig As IPneumaticUserInputsConfig
		Private _pneumaticAuxillariesConfig As IPneumaticsAuxilliariesConfig
		Private _pneumaticsActuationsMap As IPneumaticActuationsMAP
		Private _pneumaticsCompressorFlowRateMap As ICompressorMap
		Private _vehicleMassKG As Single = 16500
		Private _cycleName As String = "Urban"
		Private _cycleDurationMinutes As Single = 51.9
		Private _totalAirDemand As Single

		Private _actuationsMapPath As String = "Testfiles\testPneumaticActuationsMap_GOODMAP.apac"
		Private _compressorMapPath As String = "Testfiles\testCompressorMap.acmp"

		Private _defaultInputConfig As IPneumaticUserInputsConfig
		Private _Signals As ISignals = New Signals


		'Constructors
		Public Sub New()

			initialise()
		End Sub

		Private Sub initialise()

			_defaultInputConfig = New PneumaticUserInputsConfig()

			_defaultInputConfig.CompressorGearRatio = 1.3
			_defaultInputConfig.CompressorGearEfficiency = 0.8
			_defaultInputConfig.SmartRegeneration = True
			_defaultInputConfig.RetarderBrake = True
			_defaultInputConfig.KneelingHeightMillimeters = 80
			_defaultInputConfig.AirSuspensionControl = "Electrically"
			_defaultInputConfig.AdBlueDosing = "Pneumatic"
			_defaultInputConfig.Doors = "Pneumatic"
			_defaultInputConfig.SmartAirCompression = True

			_Signals.TotalCycleTimeSeconds = 3114

			_Signals.EngineSpeed = 3000
		End Sub


		<Test>
		Public Sub CreateNewtest()

			Dim psUserInputsConfig = CType(New PneumaticUserInputsConfig(), IPneumaticUserInputsConfig)
			psUserInputsConfig.AirSuspensionControl = "Mechanically"
			psUserInputsConfig.Doors = "Pneumatic"
			psUserInputsConfig.AdBlueDosing = "Pneumatic"


			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)


			psCompressorMap.Initialise()
			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(psUserInputsConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)


			Assert.IsNotNull(target)
		End Sub

		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValuesTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 7947.684
			Dim actual As NormLiter = target.TotalAirDemand()

			Assert.AreEqual(expected, actual.Value(), 0.000001)
		End Sub

		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrankTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Single = 5832.091
			Dim actual As Watt = target.GetAveragePowerDemandAtCrankFromPneumatics()

			Assert.AreEqual(expected, actual.Value(), 0.001)
		End Sub


		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_AveragePowerAtTheCrank_0_80EFTest()

			initialise()

			_defaultInputConfig.CompressorGearEfficiency = 0.8

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Single = 5832.091

			Assert.AreEqual(expected, target.GetAveragePowerDemandAtCrankFromPneumatics().Value(), 0.001)
		End Sub

		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRateTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Single = 7947.55127 / _Signals.TotalCycleTimeSeconds

			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub

		'SmartRegeneration = False
		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_SmartRegenOffTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			_defaultInputConfig.SmartRegeneration = False

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 8863.378 / _Signals.TotalCycleTimeSeconds

			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub

		'RetarderBrake = False
		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_RetarderBrakeOffTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			_defaultInputConfig.RetarderBrake = False

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 8541.45 / _Signals.TotalCycleTimeSeconds


			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub

		'KneelingHeightMilimeters = 100
		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Kneeling100mmTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			_defaultInputConfig.KneelingHeightMillimeters = 100

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 8557.524 / _Signals.TotalCycleTimeSeconds

			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub

		'AirSuspensionControl = "mechanically"
		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_AirSuspension_mechanicallyTest()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			_defaultInputConfig.AirSuspensionControl = "Mechanically"

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 7947.68457 / _Signals.TotalCycleTimeSeconds

			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub

		'AdBlueDosing = "electric"
		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_AdBlueDosing_electric_Test()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			_defaultInputConfig.AdBlueDosing = "Pneumatic"

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 7947.68457 / _Signals.TotalCycleTimeSeconds

			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub

		'Doors = "Electric"
		<Test()>
		Public Sub AverageLoadValueUsingDefaultAuxValues_TotalRequiredAirDeliveryRate_Doors_electric_Test()

			initialise()

			Dim psAuxConfig = CType(New PneumaticsAuxilliariesConfig(True), IPneumaticsAuxilliariesConfig)
			Dim psActuationsMap = CType(New PneumaticActuationsMAP(_actuationsMapPath), IPneumaticActuationsMAP)
			Dim psCompressorMap = CType(New CompressorMap(_compressorMapPath), ICompressorMap)

			psCompressorMap.Initialise()

			_defaultInputConfig.Doors = "Electric"

			Dim _
				target As _
					New M3_AveragePneumaticLoadDemand(_defaultInputConfig, psAuxConfig, psActuationsMap, psCompressorMap,
													_vehicleMassKG.SI(Of Kilogram), "Urban", _Signals)

			Dim expected As Double = 6880.88428 / _Signals.TotalCycleTimeSeconds

			Assert.AreEqual(expected, target.AverageAirConsumedPerSecondLitre().Value(), 0.001)
		End Sub
	End Class
End Namespace


