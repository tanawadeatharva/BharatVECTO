/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System.Data;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Utils;
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
				"  0,  20, 0,    0",
				" 100, 60, 0,    0",
				"1000, 60, 0,    0",
				"1500, 40, 1,    0",
				"2000, 50,-1,    0",
				"2500,  0, 0,    2"
			};
			var cycle = SimpleDrivingCycles.CreateCycleData(cycleData);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck_ModDataIntegrity.vmod");

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(Truck40tPowerTrain.EngineFile);


			var modData = (ModalDataContainer)run.GetContainer().ModalData;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			var lastGear = 0u;
			foreach (DataRow row in modData.Data.Rows) {
				if (cycle.Entries.Last().Distance.IsEqual(((Meter)row[(int)ModalResultField.dist]))) {
					continue;
				}
				var gear = (uint)row[(int)ModalResultField.Gear];
				var time = (Second)row[(int)ModalResultField.time];

				if ((lastGear == 0 && gear != 0) || (lastGear != 0 && gear == 0)) {
					//skipNext = (uint)row[(int)ModalResultField.Gear] == 0;
					lastGear = gear;
					continue;
				}
				var distance = (Meter)row[(int)ModalResultField.dist];
				var tqEngFcmap = (NewtonMeter)row[(int)ModalResultField.T_eng_fcmap];
				var nEngFcMap = (PerSecond)row[(int)ModalResultField.n_eng_avg];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[(int)ModalResultField.FCMap];
				Assert.AreEqual(fuelConsumption.Value(),
					engineData.ConsumptionMap.GetFuelConsumption(tqEngFcmap, nEngFcMap).Value(), 1E-3, "time: {0}  distance: {1}",
					time, distance);

				// check P_eng_FCmap = T_eng_fcmap * n_eng
				var pEngFcmap = (SI)row[(int)ModalResultField.P_eng_fcmap];
				Assert.AreEqual(pEngFcmap.Value(), (tqEngFcmap * nEngFcMap).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);


				var pWheelIn = (Watt)row[(int)ModalResultField.P_wheel_in];
				var pAir = (Watt)row[(int)ModalResultField.P_air];
				var pRoll = (Watt)row[(int)ModalResultField.P_roll];
				var pGrad = (Watt)row[(int)ModalResultField.P_slope];
				var pVehInertia = (Watt)row[(int)ModalResultField.P_veh_inertia];
				var pTrac = (Watt)row[(int)ModalResultField.P_trac];


				// Pe_﻿eng = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux - P_brake_loss
				var pEngOut = (Watt)row[(int)ModalResultField.P_eng_out];
				var pLossGbx = (Watt)row[(int)ModalResultField.P_gbx_loss];
				var pGbxIn = (Watt)row[(int)ModalResultField.P_gbx_in];
				var pLossAxle = (Watt)row[(int)ModalResultField.P_axle_loss];
				var pAxleIn = (Watt)row[(int)ModalResultField.P_axle_in];
				var pLossRet = (Watt)row[(int)ModalResultField.P_ret_loss];
				var pRetIn = (Watt)row[(int)ModalResultField.P_retarder_in];
				var pGbxInertia = (Watt)row[(int)ModalResultField.P_gbx_inertia];
				var pEngInertia = (Watt)row[(int)ModalResultField.P_eng_inertia];
				var pAux = (Watt)row[(int)ModalResultField.P_aux];
				var pBrakeLoss = (Watt)row[(int)ModalResultField.P_brake_loss];
				var pBrakeIn = (Watt)row[(int)ModalResultField.P_brake_in];
				var pClutchLoss = (Watt)row[(int)ModalResultField.P_clutch_loss];
				var pClutchOut = (Watt)row[(int)ModalResultField.P_clutch_out];
				var pWheelInertia = (Watt)row[(int)ModalResultField.P_wheel_inertia];


				// P_trac = P_veh_inertia + P_roll + P_air + P_slope
				Assert.AreEqual(pTrac.Value(), (pAir + pRoll + pGrad + pVehInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// P_wheel_in = P_trac + P_wheel_inertia
				Assert.AreEqual(pWheelIn.Value(), (pTrac + pWheelInertia).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pBrakeIn.Value(), (pWheelIn + pBrakeLoss).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pAxleIn.Value(), (pBrakeIn + pLossAxle).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pRetIn.Value(), (pAxleIn + pLossRet).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), pClutchOut.Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngOut.Value(), (pClutchOut + pClutchLoss).Value(), 1E-3,
					"time: {0}  distance: {1}", time, distance);

				if (gear != 0) {
					Assert.AreEqual(pEngFcmap.Value(),
						(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
						pClutchLoss).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
				}
				lastGear = gear;
			}
		}
	}
}