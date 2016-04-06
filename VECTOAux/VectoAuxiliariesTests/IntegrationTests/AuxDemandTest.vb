Imports System.IO
Imports NUnit.Framework
Imports VectoAuxiliaries

Namespace IntegrationTests
	<TestFixture>
	Public Class AuxDemandTest
		'<TestCase(12000, 1256, 148, 148, 4537.96826)> ' without HVAC demand
		'<TestCase(12000, 1256, -15, -50, 7405.0791)>  ' without HVAC demand<Test>
		<TestCase(12000, 1256, 148, 148, 6087.0317)>
		<TestCase(12000, 1256, -15, -50, 8954.1435)>
		<TestCase(15700, 1319, -35.79263, -144.0441, 9093.9511)>
		Public Sub AuxDemnadTest(vehicleWeight As Double, engineSpeed As Double, driveLinePower As Double,
								internalPower As Double,
								expectedPowerDemand As Double)
			Dim engineFCMapFilePath = "TestFiles\Integration\24t Coach.vmap"
			Dim auxFilePath = "TestFiles\Integration\AdvAuxTest.aaux"

			Dim aux As AdvancedAuxiliaries = New AdvancedAuxiliaries

			aux.VectoInputs.Cycle = "Coach"
			aux.VectoInputs.VehicleWeightKG = vehicleWeight
			aux.VectoInputs.FuelDensity = 0.832
			Dim fuelMap As cMAP = New cMAP()
			fuelMap.FilePath = engineFCMapFilePath
			fuelMap.ReadFile(False)
			fuelMap.Triangulate()

			aux.VectoInputs.FuelMap = fuelMap


			aux.Signals.TotalCycleTimeSeconds = 15000
			aux.Signals.EngineIdleSpeed = 560

			aux.Initialise(Path.GetFileName(auxFilePath), Path.GetDirectoryName(Path.GetFullPath(auxFilePath)) + "\")

			aux.Signals.ClutchEngaged = True
			aux.Signals.EngineDrivelinePower = driveLinePower 'kW
			aux.Signals.EngineSpeed = engineSpeed 'rpm
			aux.Signals.EngineDrivelineTorque = driveLinePower * 1000 / (engineSpeed * 2 * Math.PI / 60)
			aux.Signals.EngineMotoringPower = 24 'kW - has to be positive

			aux.Signals.PreExistingAuxPower = 6.1
			aux.Signals.Idle = False
			aux.Signals.InNeutral = False
			aux.Signals.RunningCalc = True
			aux.Signals.Internal_Engine_Power = internalPower	'kW

			Dim power As Single = aux.AuxiliaryPowerAtCrankWatts

			Assert.AreEqual(expectedPowerDemand, power, 0.0001)
		End Sub

		<Test>
		Public Sub AuxFCConsumtionTest()

			Dim driveLinePower As Double = 148
			Dim internalPower As Double = 148
			Dim engineSpeed As Double = 1256


			Dim engineFCMapFilePath = "TestFiles\Integration\24t Coach.vmap"
			Dim auxFilePath = "TestFiles\Integration\AdvAuxTest.aaux"

			Dim aux As AdvancedAuxiliaries = New AdvancedAuxiliaries

			aux.VectoInputs.Cycle = "Coach"
			aux.VectoInputs.VehicleWeightKG = 12000
			aux.VectoInputs.FuelDensity = 0.832
			Dim fuelMap As cMAP = New cMAP()
			fuelMap.FilePath = engineFCMapFilePath
			fuelMap.ReadFile(False)
			fuelMap.Triangulate()

			aux.VectoInputs.FuelMap = fuelMap


			aux.Signals.TotalCycleTimeSeconds = 15000
			aux.Signals.EngineIdleSpeed = 560

			aux.Initialise(Path.GetFileName(auxFilePath), Path.GetDirectoryName(Path.GetFullPath(auxFilePath)) + "\")

			aux.Signals.ClutchEngaged = True
			aux.Signals.EngineDrivelinePower = driveLinePower 'kW
			aux.Signals.EngineSpeed = engineSpeed 'rpm
			aux.Signals.EngineDrivelineTorque = driveLinePower * 1000 / (1256 * 2 * Math.PI / 60)
			aux.Signals.EngineMotoringPower = 24 'kW - has to be positive

			aux.Signals.PreExistingAuxPower = 0
			aux.Signals.Idle = False
			aux.Signals.InNeutral = False
			aux.Signals.RunningCalc = True
			aux.Signals.Internal_Engine_Power = internalPower	'kW

			Dim msg As String = String.Empty
			For i As Integer = 0 To 9
				Assert.AreEqual(6087.0317, aux.AuxiliaryPowerAtCrankWatts, 0.001)
				aux.CycleStep(1, msg)
			Next

			Assert.AreEqual(79.303, aux.AA_TotalCycleFC_Grams, 0.0001)

			aux.Signals.EngineDrivelinePower = -15
			aux.Signals.EngineDrivelineTorque = aux.Signals.EngineDrivelinePower * 1000 / (1256 * 2 * Math.PI / 60)
			aux.Signals.Internal_Engine_Power = -50

			For i As Integer = 0 To 9
				Assert.AreEqual(8954.1435, aux.AuxiliaryPowerAtCrankWatts, 0.001)
				aux.CycleStep(1, msg)
			Next

			Assert.AreEqual(82.5783, aux.AA_TotalCycleFC_Grams, 0.0001)

			aux.Signals.EngineDrivelinePower = driveLinePower
			aux.Signals.EngineDrivelineTorque = aux.Signals.EngineDrivelinePower * 1000 / (1256 * 2 * Math.PI / 60)
			aux.Signals.Internal_Engine_Power = internalPower

			For i As Integer = 0 To 9
				Assert.AreEqual(6087.0317, aux.AuxiliaryPowerAtCrankWatts, 0.001)
				aux.CycleStep(1, msg)
			Next

			Assert.AreEqual(162.4655, aux.AA_TotalCycleFC_Grams, 0.0001)
		End Sub
	End Class
End Namespace