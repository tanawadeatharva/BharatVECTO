using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestClass]
	public class ModDataTest
	{
		[TestMethod]
		public void ModDataIntegrityTest()
		{
			var cycleData = new[] {
				// <s>,<v>,<grad>,<stop>
				"  0,  20, 0,     0",
				" 100, 60, 0,     0",
				"1000, 60, 0,     0"
			};
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck_ModDataIntegrity.vmod");

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(Truck40tPowerTrain.EngineFile);


			var modData = (ModalDataContainer)run.GetContainer().ModalData;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			foreach (DataRow row in modData.Data.Rows) {
				var time = (Second)row[(int)ModalResultField.time];
				var distance = (Meter)row[(int)ModalResultField.dist];
				var torqueEngine = (NewtonMeter)row[(int)ModalResultField.Tq_eng];
				var engineSpeed = (PerSecond)row[(int)ModalResultField.n];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[(int)ModalResultField.FCMap];
				Assert.AreEqual(fuelConsumption.Value(),
					engineData.ConsumptionMap.GetFuelConsumption(torqueEngine, engineSpeed).Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				// check Pe_eng = Tq_eng * n_eng
				var enginePower = (SI)row[(int)ModalResultField.Pe_eng];
				Assert.AreEqual(enginePower.Value(), (torqueEngine * engineSpeed).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// P_wheel = P_air + P_roll + P_grad + Pa_veh
				var pWheel = (Watt)row[(int)ModalResultField.Pwheel];
				var pAir = (Watt)row[(int)ModalResultField.Pair];
				var pRoll = (Watt)row[(int)ModalResultField.Proll];
				var pGrad = (Watt)row[(int)ModalResultField.Pgrad];
				var paVeh = (Watt)row[(int)ModalResultField.PaVeh];

				Assert.AreEqual(pWheel.Value(), (pAir + pRoll + pGrad + paVeh).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// Pe_﻿eng = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux - Pbrake
				var peEng = (Watt)row[(int)ModalResultField.Pe_eng];
				var pLossGbx = (Watt)row[(int)ModalResultField.PlossGB];
				var pLossAxle = (Watt)row[(int)ModalResultField.PlossDiff];
				var pLossRet = (Watt)row[(int)ModalResultField.PlossRetarder];
				var paGbx = (Watt)row[(int)ModalResultField.PaGB];
				var paEng = (Watt)row[(int)ModalResultField.PaEng];
				var pAux = (Watt)row[(int)ModalResultField.Paux];
				var pBrake = (Watt)row[(int)ModalResultField.Pbrake];

				var gear = (uint)row[(int)ModalResultField.Gear];

				if (gear != 0) {
					Assert.AreEqual(peEng.Value(), (pWheel + pLossGbx + pLossAxle + pLossRet + paGbx + paEng + pAux - pBrake).Value(),
						1E-3, "time: {0}  distance: {1}", time, distance);
				}
			}
		}
	}
}