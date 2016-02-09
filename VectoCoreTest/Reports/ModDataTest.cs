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
				var torqueEngine = (NewtonMeter)row[(int)ModalResultField.T_eng_fcmap];
				var engineSpeed = (PerSecond)row[(int)ModalResultField.n_eng_avg];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[(int)ModalResultField.FCMap];
				Assert.AreEqual(fuelConsumption.Value(),
					engineData.ConsumptionMap.GetFuelConsumption(torqueEngine, engineSpeed).Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				// check P_eng_out = T_eng_fcmap * n_eng
				var enginePower = (SI)row[(int)ModalResultField.P_eng_out];
				Assert.AreEqual(enginePower.Value(), (torqueEngine * engineSpeed).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// P_wheel = P_air + P_roll + P_grad + Pa_veh
				var pWheel = (Watt)row[(int)ModalResultField.P_wheel_in];
				var pAir = (Watt)row[(int)ModalResultField.P_air];
				var pRoll = (Watt)row[(int)ModalResultField.P_roll];
				var pGrad = (Watt)row[(int)ModalResultField.P_slope];
				var paVeh = (Watt)row[(int)ModalResultField.P_veh_inertia];

				Assert.AreEqual(pWheel.Value(), (pAir + pRoll + pGrad + paVeh).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// Pe_﻿eng = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux - P_brake_loss
				var peEng = (Watt)row[(int)ModalResultField.P_eng_out];
				var pLossGbx = (Watt)row[(int)ModalResultField.P_gbx_loss];
				var pLossAxle = (Watt)row[(int)ModalResultField.P_axle_loss];
				var pLossRet = (Watt)row[(int)ModalResultField.P_ret_loss];
				var paGbx = (Watt)row[(int)ModalResultField.P_gbx_inertia];
				var paEng = (Watt)row[(int)ModalResultField.P_eng_inertia];
				var pAux = (Watt)row[(int)ModalResultField.P_aux];
				var pBrake = (Watt)row[(int)ModalResultField.P_brake_loss];

				var gear = (uint)row[(int)ModalResultField.Gear];

				if (gear != 0) {
					Assert.AreEqual(peEng.Value(), (pWheel + pLossGbx + pLossAxle + pLossRet + paGbx + paEng + pAux - pBrake).Value(),
						1E-3, "time: {0}  distance: {1}", time, distance);
				}
			}
		}
	}
}