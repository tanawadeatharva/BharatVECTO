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

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Org.BouncyCastle.Asn1;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestFixture]
	public class ModDataTest
	{
		[TestCase()]
		public void ModDataIntegritySimpleTest()
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
			var sumData = new SummaryDataContainer(null);
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck_ModDataIntegrity.vmod");

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(Truck40tPowerTrain.EngineFile);

			// get a reference to the mod-data because the modaldata container clears it after simulation
			var modData = ((ModalDataContainer)run.GetContainer().ModalData).Data;
			var auxKeys = ((ModalDataContainer)run.GetContainer().ModalData).Auxiliaries;

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			AssertModDataIntegrity(modData, auxKeys, cycle.Entries.Last().Distance, engineData.ConsumptionMap);
		}

		[TestCase(@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto")]
		public void TestFullCycleModDataIntegrityDeclMT(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Declaration);
		}

		[TestCase(@"TestData\Integration\EngineeringMode\Class2_RigidTruck_4x2\Class2_RigidTruck_ENG.vecto"),
		TestCase(@"TestData\Integration\EngineeringMode\Class5_Tractor_4x2\Class5_Tractor_ENG.vecto"),
		TestCase(@"TestData\Integration\EngineeringMode\Class9_RigidTruck_6x2_PTO\Class9_RigidTruck_ENG_PTO.vecto")]
		public void TestFullCycleModDataIntegrityMT(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Engineering);
		}

		private static void RunSimulation(string jobName, ExecutionMode mode)
		{
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory = new SimulatorFactory(mode, inputData, fileWriter) { WriteModalResults = true };

			jobContainer.AddRuns(runsFactory);
			var modData = new List<Tuple<ModalResults, Meter>>();
			foreach (var run in jobContainer.Runs) {
				modData.Add(Tuple.Create(((ModalDataContainer)run.Run.GetContainer().ModalData).Data,
					((DistanceBasedDrivingCycle)((VehicleContainer)run.Run.GetContainer()).DrivingCycle)._data.Entries.Last()
						.Distance));
			}
			var auxKeys =
				new Dictionary<string, DataColumn>(
					((ModalDataContainer)jobContainer.Runs.First().Run.GetContainer().ModalData).Auxiliaries);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			// mod files will be stored in e.g. 
			// VectoCoreTest\bin\Debug\TestData\Integration\EngineeringMode\Class2_RigidTruck_4x2\Class2_RigidTruck_ENG.vecto_00.vmod
			//fileWriter.WriteModData(Path.GetFileName(jobName), "0", "0", modData[0].Item1);
			//fileWriter.WriteModData(Path.GetFileName(jobName), "1", "1", modData[1].Item1);

			foreach (var modalResults in modData) {
				AssertModDataIntegrity(modalResults.Item1, auxKeys, modalResults.Item2,
					FuelConsumptionMapReader.Create(((IEngineeringInputDataProvider)inputData).EngineInputData.FuelConsumptionMap));
			}

			AssertSumDataIntegrity(sumData, mode);
		}

		private static void AssertSumDataIntegrity(SummaryDataContainer sumData, ExecutionMode mode)
		{
			Assert.IsTrue(sumData._table.Rows.Count > 0);

			var ptoTransmissionColumn =
				sumData._table.Columns.Contains(string.Format(SummaryDataContainer.E_FORMAT,
					Constants.Auxiliaries.IDs.PTOTransmission))
					? string.Format(SummaryDataContainer.E_FORMAT, Constants.Auxiliaries.IDs.PTOTransmission)
					: null;
			var ptoConsumerColumn =
				sumData._table.Columns.Contains(string.Format(SummaryDataContainer.E_FORMAT, Constants.Auxiliaries.IDs.PTOConsumer))
					? string.Format(SummaryDataContainer.E_FORMAT, Constants.Auxiliaries.IDs.PTOConsumer)
					: null;

			foreach (DataRow row in sumData._table.Rows) {
				var inputFile = row[SummaryDataContainer.INPUTFILE].ToString();
				var cycle = row[SummaryDataContainer.CYCLE].ToString();
				var loading = row[SummaryDataContainer.LOADING].ToString();
				var eFcMapPos = ((SI)row[SummaryDataContainer.E_FCMAP_POS]).Value();
				var eFcMapNeg = ((SI)row[SummaryDataContainer.E_FCMAP_NEG]).Value();
				var ePowertrainInertia = ((SI)row[SummaryDataContainer.E_POWERTRAIN_INERTIA]).Value();
				var eAux = ((SI)row[SummaryDataContainer.E_AUX]).Value();
				var eClutchLoss = ((SI)row[SummaryDataContainer.E_CLUTCH_LOSS]).Value();
				var eTcLoss = ((SI)row[SummaryDataContainer.E_TC_LOSS]).Value();
				var eShiftLoss = ((SI)row[SummaryDataContainer.E_SHIFT_LOSS]).Value();
				var eGbxLoss = ((SI)row[SummaryDataContainer.E_GBX_LOSS]).Value();
				var eRetLoss = ((SI)row[SummaryDataContainer.E_RET_LOSS]).Value();
				var eAngleLoss = ((SI)row[SummaryDataContainer.E_ANGLE_LOSS]).Value();
				var eAxlLoss = ((SI)row[SummaryDataContainer.E_AXL_LOSS]).Value();
				var eBrakeLoss = ((SI)row[SummaryDataContainer.E_BRAKE]).Value();
				var eVehInertia = ((SI)row[SummaryDataContainer.E_VEHICLE_INERTIA]).Value();
				var eAir = ((SI)row[SummaryDataContainer.E_AIR]).Value();
				var eRoll = ((SI)row[SummaryDataContainer.E_ROLL]).Value();
				var eGrad = ((SI)row[SummaryDataContainer.E_GRAD]).Value();
				var cargoVolume = mode == ExecutionMode.Engineering ? 0 : ((SI)row[SummaryDataContainer.VOLUME]).Value();

				var loadingValue = ((SI)row[SummaryDataContainer.LOADING]).Value() / 1000;
				var fcPer100km = ((SI)row[SummaryDataContainer.FCFINAL_LITERPER100KM]).Value();
				var fcPerVolume = mode == ExecutionMode.Engineering
					? 0
					: ((SI)row[SummaryDataContainer.FCFINAL_LiterPer100M3KM]).Value();
				var fcPerLoad = loadingValue > 0 ? ((SI)row[SummaryDataContainer.FCFINAL_LITERPER100TKM]).Value() : 0;
				var co2Per100km = ((SI)row[SummaryDataContainer.CO2_KM]).Value();
				var co2PerVolume = mode == ExecutionMode.Engineering ? 0 : ((SI)row[SummaryDataContainer.CO2_M3KM]).Value();
				var co2PerLoad = loadingValue > 0 ? ((SI)row[SummaryDataContainer.CO2_TKM]).Value() : 0;

				var ePTOtransm = ptoTransmissionColumn != null ? ((SI)row[ptoTransmissionColumn]).Value() : 0;
				var ePTOconsumer = ptoConsumerColumn != null ? ((SI)row[ptoConsumerColumn]).Value() : 0;

				// E_fcmap_pos = E_fcmap_neg + E_powertrain_inertia + E_aux_xxx + E_aux_sum + E_clutch_loss + E_tc_loss + E_gbx_loss + E_shift_loss + E_ret_loss + E_angle_loss + E_axl_loss + E_brake + E_vehicle_inertia + E_air + E_roll + E_grad + E_PTO_CONSUM + E_PTO_TRANSM
				Assert.AreEqual(eFcMapPos,
					eFcMapNeg + ePowertrainInertia + eAux + eClutchLoss + eTcLoss + eGbxLoss + eRetLoss + eAngleLoss +
					eAxlLoss + eBrakeLoss + eVehInertia + eAir + eRoll + eGrad + ePTOconsumer + ePTOtransm, 1e-3,
					"input file: {0}  cycle: {1} loading: {2}",
					inputFile, cycle, loading);

				var pFcmapPos = ((SI)row[SummaryDataContainer.P_FCMAP_POS]).Value();
				var time = ((SI)row[SummaryDataContainer.TIME]).Value();

				// E_fcmap_pos = P_fcmap_pos * t
				Assert.AreEqual(eFcMapPos, pFcmapPos * (time / 3600), 1e-3, "input file: {0}  cycle: {1} loading: {2}", inputFile,
					cycle, loading);

				if (cargoVolume > 0) {
					Assert.AreEqual(fcPerVolume, fcPer100km / cargoVolume, 1e-3, "input file: {0}  cycle: {1} loading: {2}", inputFile,
						cycle, loading);

					Assert.AreEqual(co2PerVolume, co2Per100km / cargoVolume, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile,
						cycle, loading);
				}

				if (loadingValue > 0) {
					Assert.AreEqual(co2PerLoad, co2Per100km / loadingValue, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
					Assert.AreEqual(fcPerLoad, fcPer100km / loadingValue, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
				}

				var stopTimeShare = ((SI)row[SummaryDataContainer.STOP_TIMESHARE]).Value();
				var accTimeShare = ((SI)row[SummaryDataContainer.ACC_TIMESHARE]).Value();
				var decTimeShare = ((SI)row[SummaryDataContainer.DEC_TIMESHARE]).Value();
				var cruiseTimeShare = ((SI)row[SummaryDataContainer.CRUISE_TIMESHARE]).Value();

				Assert.AreEqual(100, stopTimeShare + accTimeShare + decTimeShare + cruiseTimeShare, 1e-3,
					"input file: {0}  cycle: {1} loading: {2}", inputFile, cycle, loading);

				Assert.IsTrue(((SI)row[SummaryDataContainer.ACC_POS]).Value() > 0);
				Assert.IsTrue(((SI)row[SummaryDataContainer.ACC_NEG]).Value() < 0);
			}
		}

		private static void AssertModDataIntegrity(ModalResults modData, Dictionary<string, DataColumn> auxKeys,
			Meter totalDistance, FuelConsumptionMap consumptionMap)
		{
			Assert.IsTrue(modData.Rows.Count > 0);

			var ptoTransmissionColumn = auxKeys.ContainsKey(Constants.Auxiliaries.IDs.PTOTransmission)
				? auxKeys[Constants.Auxiliaries.IDs.PTOTransmission]
				: null;
			var ptoConsumerColumn = auxKeys.ContainsKey(Constants.Auxiliaries.IDs.PTOConsumer)
				? auxKeys[Constants.Auxiliaries.IDs.PTOConsumer]
				: null;
			foreach (DataRow row in modData.Rows) {
				if (totalDistance.IsEqual(((Meter)row[(int)ModalResultField.dist]))) {
					continue;
				}
				var gear = (uint)row[(int)ModalResultField.Gear];
				var time = (Second)row[(int)ModalResultField.time];

				var distance = (Meter)row[(int)ModalResultField.dist];
				var tqEngFcmap = (NewtonMeter)row[(int)ModalResultField.T_eng_fcmap];
				var nEngFcMap = (PerSecond)row[(int)ModalResultField.n_eng_avg];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[(int)ModalResultField.FCMap];
				Assert.AreEqual(fuelConsumption.Value(),
					consumptionMap.GetFuelConsumption(tqEngFcmap, nEngFcMap).Value.Value(), 1E-3, "time: {0}  distance: {1}",
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

				// P_﻿eng_out = P﻿_wheel + P_loss﻿gearbox + P_loss﻿axle + P_loss﻿retarder + P_a﻿gbx + Pa_﻿eng + P_aux - P_brake_loss
				var pEngOut = (Watt)row[(int)ModalResultField.P_eng_out];
				var pLossGbx = (Watt)row[(int)ModalResultField.P_gbx_loss];
				var pGbxIn = (Watt)row[(int)ModalResultField.P_gbx_in];
				var pLossAxle = (Watt)row[(int)ModalResultField.P_axle_loss];
				var pLossAngle = row[(int)ModalResultField.P_angle_loss] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[(int)ModalResultField.P_angle_loss];
				var pAxleIn = (Watt)row[(int)ModalResultField.P_axle_in];
				var pLossRet = (Watt)row[(int)ModalResultField.P_ret_loss];
				var pRetIn = (Watt)row[(int)ModalResultField.P_retarder_in];
				var pGbxInertia = (Watt)row[(int)ModalResultField.P_gbx_inertia];
				var pShiftLoss = row[(int)ModalResultField.P_gbx_shift_loss] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[(int)ModalResultField.P_gbx_shift_loss];
				var pEngInertia = (Watt)row[(int)ModalResultField.P_eng_inertia];
				var pAux =
					(Watt)(row[(int)ModalResultField.P_aux] != DBNull.Value ? row[(int)ModalResultField.P_aux] : 0.SI<Watt>());
				var pBrakeLoss = (Watt)row[(int)ModalResultField.P_brake_loss];
				var pBrakeIn = (Watt)row[(int)ModalResultField.P_brake_in];
				var pClutchLoss = (Watt)row[(int)ModalResultField.P_clutch_loss];
				var pClutchOut = (Watt)row[(int)ModalResultField.P_clutch_out];
				var pWheelInertia = (Watt)row[(int)ModalResultField.P_wheel_inertia];
				var pPTOconsumer = ptoConsumerColumn == null || row[ptoConsumerColumn] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoConsumerColumn];
				var pPTOtransm = ptoTransmissionColumn == null || row[ptoTransmissionColumn] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoTransmissionColumn];
				// P_trac = P_veh_inertia + P_roll + P_air + P_slope
				Assert.AreEqual(pTrac.Value(), (pAir + pRoll + pGrad + pVehInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// P_wheel_in = P_trac + P_wheel_inertia
				Assert.AreEqual(pWheelIn.Value(), (pTrac + pWheelInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pBrakeIn.Value(), (pWheelIn + pBrakeLoss).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pAxleIn.Value(), (pBrakeIn + pLossAxle).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pRetIn.Value(), (pAxleIn + pLossRet).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pGbxIn.Value(), pClutchOut.Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngOut.Value(), (pClutchOut + pClutchLoss).Value(), 1E-3,
					"time: {0}  distance: {1}", time, distance);

				Assert.IsTrue(pLossGbx.IsGreaterOrEqual(pShiftLoss + pGbxInertia), "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);

				// P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer )
				Assert.AreEqual(pEngFcmap.Value(), (pEngOut + pAux + pEngInertia + pPTOtransm + pPTOconsumer).Value(), 0.5,
					"time: {0}  distance: {1}", time, distance);

				// P_eng_fcmap = sum(Losses Powertrain)
				var pLossTot = pClutchLoss + pLossGbx + pLossRet + pGbxInertia + pLossAngle + pLossAxle + pBrakeLoss +
								pWheelInertia + pAir + pRoll + pGrad + pVehInertia + pPTOconsumer + pPTOtransm;
				var pEngFcmapCalc = (pLossTot + pEngInertia + pAux).Value();
				Assert.AreEqual(pEngFcmap.Value(), pEngFcmapCalc, 0.5, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngFcmap.Value(),
					(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
					pClutchLoss + pPTOtransm + pPTOconsumer).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
			}
		}

		[
			TestCase(@"TestData\Integration\EngineeringMode\CityBus_AT\CityBus_AT_Ser.vecto"),
			TestCase(@"TestData\Integration\EngineeringMode\CityBus_AT\CityBus_AT_PS.vecto")]
		public void TestFullCycleModDataIntegrityAT(string jobName)
		{
			var fileWriter = new FileOutputWriter(jobName);
			var sumData = new SummaryDataContainer(fileWriter);

			var jobContainer = new JobContainer(sumData);
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);

			var runsFactory =
				new SimulatorFactory(ExecutionMode.Engineering, inputData, fileWriter) { WriteModalResults = true };

			jobContainer.AddRuns(runsFactory);
			var modData = new List<Tuple<ModalResults, Meter>>();
			foreach (var run in jobContainer.Runs) {
				modData.Add(Tuple.Create(((ModalDataContainer)run.Run.GetContainer().ModalData).Data,
					((DistanceBasedDrivingCycle)((VehicleContainer)run.Run.GetContainer()).DrivingCycle)._data.Entries.Last()
						.Distance));
			}
			var auxKeys =
				new Dictionary<string, DataColumn>(
					((ModalDataContainer)jobContainer.Runs.First().Run.GetContainer().ModalData).Auxiliaries);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			foreach (var modalResults in modData) {
				AssertModDataIntegrityAT(modalResults.Item1, auxKeys, modalResults.Item2,
					FuelConsumptionMapReader.Create(((IEngineeringInputDataProvider)inputData).EngineInputData.FuelConsumptionMap));
			}

			AssertSumDataIntegrity(sumData, ExecutionMode.Engineering);
		}

		private static void AssertModDataIntegrityAT(ModalResults modData, Dictionary<string, DataColumn> auxKeys,
			Meter totalDistance, FuelConsumptionMap consumptionMap)
		{
			Assert.IsTrue(modData.Rows.Count > 0);

			var ptoTransmissionColumn = auxKeys.ContainsKey(Constants.Auxiliaries.IDs.PTOTransmission)
				? auxKeys[Constants.Auxiliaries.IDs.PTOTransmission]
				: null;
			var ptoConsumerColumn = auxKeys.ContainsKey(Constants.Auxiliaries.IDs.PTOConsumer)
				? auxKeys[Constants.Auxiliaries.IDs.PTOConsumer]
				: null;
			foreach (DataRow row in modData.Rows) {
				if (totalDistance.IsEqual(((Meter)row[(int)ModalResultField.dist]))) {
					continue;
				}
				var gear = (uint)row[(int)ModalResultField.Gear];
				var time = (Second)row[(int)ModalResultField.time];

				var distance = (Meter)row[(int)ModalResultField.dist];
				var tqEngFcmap = (NewtonMeter)row[(int)ModalResultField.T_eng_fcmap];
				var nEngFcMap = (PerSecond)row[(int)ModalResultField.n_eng_avg];

				// check fuel consumption interpolation
				var fuelConsumption = (SI)row[(int)ModalResultField.FCMap];
				Assert.AreEqual(fuelConsumption.Value(),
					consumptionMap.GetFuelConsumption(tqEngFcmap, nEngFcMap).Value.Value(), 1E-3, "time: {0}  distance: {1}",
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
				var pLossAngle = row[(int)ModalResultField.P_angle_loss] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[(int)ModalResultField.P_angle_loss];
				var pAxleIn = (Watt)row[(int)ModalResultField.P_axle_in];
				var pLossRet = (Watt)row[(int)ModalResultField.P_ret_loss];
				var pRetIn = (Watt)row[(int)ModalResultField.P_retarder_in];
				var pGbxInertia = (Watt)row[(int)ModalResultField.P_gbx_inertia];
				var pShiftLoss = row[(int)ModalResultField.P_gbx_shift_loss] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[(int)ModalResultField.P_gbx_shift_loss];
				var pEngInertia = (Watt)row[(int)ModalResultField.P_eng_inertia];
				var pAux =
					(Watt)(row[(int)ModalResultField.P_aux] != DBNull.Value ? row[(int)ModalResultField.P_aux] : 0.SI<Watt>());
				var pBrakeLoss = (Watt)row[(int)ModalResultField.P_brake_loss];
				var pBrakeIn = (Watt)row[(int)ModalResultField.P_brake_in];
				var pTcLoss = (Watt)row[(int)ModalResultField.P_TC_loss];
				var pTcOut = (Watt)row[(int)ModalResultField.P_TC_out];
				var pWheelInertia = (Watt)row[(int)ModalResultField.P_wheel_inertia];
				var pPTOconsumer = ptoConsumerColumn == null || row[ptoConsumerColumn] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoConsumerColumn];
				var pPTOtransm = ptoTransmissionColumn == null || row[ptoTransmissionColumn] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoTransmissionColumn];
				// P_trac = P_veh_inertia + P_roll + P_air + P_slope
				Assert.AreEqual(pTrac.Value(), (pAir + pRoll + pGrad + pVehInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				// P_wheel_in = P_trac + P_wheel_inertia
				Assert.AreEqual(pWheelIn.Value(), (pTrac + pWheelInertia).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pBrakeIn.Value(), (pWheelIn + pBrakeLoss).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pAxleIn.Value(), (pBrakeIn + pLossAxle).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pRetIn.Value(), (pAxleIn + pLossRet).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pGbxIn.Value(), pTcOut.Value(), 1E-3, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngOut.Value(), (pTcOut + pTcLoss).Value(), 1E-3,
					"time: {0}  distance: {1}", time, distance);

				// P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer )
				Assert.AreEqual(pEngFcmap.Value(), (pEngOut + pAux + pEngInertia + pPTOtransm + pPTOconsumer).Value(), 1E-3,
					"time: {0}  distance: {1}", time,
					distance);

				// P_eng_fcmap = sum(Losses Powertrain)
				var pLossTot = pTcLoss + pLossGbx + pLossRet + pGbxInertia + pLossAngle + pLossAxle + pBrakeLoss +
								pWheelInertia + pAir + pRoll + pGrad + pVehInertia + pPTOconsumer + pPTOtransm;

				Assert.AreEqual(pEngFcmap.Value(), (pLossTot + pEngInertia + pAux).Value(), 1E-3, "time: {0}  distance: {1}", time,
					distance);

				Assert.IsTrue(pLossGbx.IsGreaterOrEqual(pShiftLoss + pGbxInertia), "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);
				Assert.AreEqual(pEngFcmap.Value(),
					(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
					pTcLoss + pPTOtransm + pPTOconsumer).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
			}
		}
	}
}