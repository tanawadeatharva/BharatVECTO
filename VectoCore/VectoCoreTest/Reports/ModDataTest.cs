/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration;
using TUGraz.VectoCore.Tests.Utils;
using System.IO;

namespace TUGraz.VectoCore.Tests.Reports
{
	[TestFixture]
	public class ModDataTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

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

			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(Truck40tPowerTrain.EngineFile, 0);

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

		[TestCase(@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2\Class2_RigidTruck_DECL.vecto")]
		public void TestVSUM_VMOD_FormatDecl(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Declaration);

			var tmpWriter = new FileOutputWriter(jobName);

			AssertModDataFormat(tmpWriter.GetModDataFileName(Path.GetFileNameWithoutExtension(jobName), "LongHaul", "ReferenceLoad"));
			AssertSumDataFormat(tmpWriter.SumFileName);
		}

		[TestCase(@"TestData\Integration\EngineeringMode\Class2_RigidTruck_4x2\Class2_RigidTruck_ENG.vecto"),
		TestCase(@"TestData\Integration\EngineeringMode\Class5_Tractor_4x2\Class5_Tractor_ENG.vecto"),
		TestCase(@"TestData\Integration\EngineeringMode\Class9_RigidTruck_6x2_PTO\Class9_RigidTruck_ENG_PTO.vecto"),]
		public void TestFullCycleModDataIntegrityMT(string jobName)
		{
			RunSimulation(jobName, ExecutionMode.Engineering);	
		}

		private void AssertModDataFormat(string modFilename)
		{
			var lineCnt = 0;
			var gearColumn = -1;
			foreach (var line in File.ReadLines(modFilename)) {
				lineCnt++;
				if (lineCnt == 2) {
					var header = line.Split(',').ToList();
					gearColumn = header.FindIndex(x => x.StartsWith("Gear"));
				}
				if (lineCnt <= 2) {
					continue;
				}
				var parts = line.Split(',');
				for (var i = 0; i < 53; i++) {
					if (i == gearColumn || i >= parts.Length || string.IsNullOrWhiteSpace(parts[i])) {
						continue;
					}
					var numParts = parts[i].Split('.');
					Assert.AreEqual(2, numParts.Length, string.Format("Line {0}: column {1}: value {2}", lineCnt, i, parts[i]));
					Assert.IsTrue(numParts[0].Length > 0);
					Assert.AreEqual(4, numParts[1].Length);
				} 
			}
		}

		private void AssertSumDataFormat(string sumFilename)
		{
			var first = 2;
			foreach (var line in File.ReadLines(sumFilename)) {
				if (first > 0) {
					first--;
					continue;
				}
				var parts = line.Split(',');
				for (var i = 56; i < 128; i++) {
					if (i >= parts.Length || string.IsNullOrWhiteSpace(parts[i])) {
						continue;
					}
					var numParts = parts[i].Split('.');
					Assert.AreEqual(2, numParts.Length);
					Assert.IsTrue(numParts[0].Length > 0);
					Assert.AreEqual(4, numParts[1].Length);
				}
			}
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
					((DistanceBasedDrivingCycle)((VehicleContainer)run.Run.GetContainer()).DrivingCycle).Data.Entries.Last()
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
					FuelConsumptionMapReader.Create(((IEngineeringInputDataProvider)inputData).JobInputData.Vehicle.EngineInputData.FuelConsumptionMap));
			}

			AssertSumDataIntegrity(sumData, mode);
		}

		private static void AssertSumDataIntegrity(SummaryDataContainer sumData, ExecutionMode mode)
		{
			Assert.IsTrue(sumData.Table.Rows.Count > 0);

			var ptoTransmissionColumn =
				sumData.Table.Columns.Contains(string.Format(SummaryDataContainer.E_FORMAT,
					Constants.Auxiliaries.IDs.PTOTransmission))
					? string.Format(SummaryDataContainer.E_FORMAT, Constants.Auxiliaries.IDs.PTOTransmission)
					: null;
			var ptoConsumerColumn =
				sumData.Table.Columns.Contains(string.Format(SummaryDataContainer.E_FORMAT, Constants.Auxiliaries.IDs.PTOConsumer))
					? string.Format(SummaryDataContainer.E_FORMAT, Constants.Auxiliaries.IDs.PTOConsumer)
					: null;

			foreach (DataRow row in sumData.Table.Rows) {
				var inputFile = row[SummaryDataContainer.INPUTFILE].ToString();
				var cycle = row[SummaryDataContainer.CYCLE].ToString();
				var loading = row[SummaryDataContainer.LOADING].ToString();
				var eFcMapPos = ((ConvertedSI)row[SummaryDataContainer.E_FCMAP_POS]);
				var eFcMapNeg = ((ConvertedSI)row[SummaryDataContainer.E_FCMAP_NEG]);
				var ePowertrainInertia = ((ConvertedSI)row[SummaryDataContainer.E_POWERTRAIN_INERTIA]);
				var eAux = ((ConvertedSI)row[SummaryDataContainer.E_AUX]);
				var eClutchLoss = ((ConvertedSI)row[SummaryDataContainer.E_CLUTCH_LOSS]);
				var eTcLoss = ((ConvertedSI)row[SummaryDataContainer.E_TC_LOSS]);
				//var eShiftLoss = ((SI)row[SummaryDataContainer.E_SHIFT_LOSS]);
				var eGbxLoss = ((ConvertedSI)row[SummaryDataContainer.E_GBX_LOSS]);
				var eRetLoss = ((ConvertedSI)row[SummaryDataContainer.E_RET_LOSS]);
				var eAngleLoss = ((ConvertedSI)row[SummaryDataContainer.E_ANGLE_LOSS]);
				var eAxlLoss = ((ConvertedSI)row[SummaryDataContainer.E_AXL_LOSS]);
				var eBrakeLoss = ((ConvertedSI)row[SummaryDataContainer.E_BRAKE]);
				var eVehInertia = ((ConvertedSI)row[SummaryDataContainer.E_VEHICLE_INERTIA]);
				var eAir = ((ConvertedSI)row[SummaryDataContainer.E_AIR]);
				var eRoll = ((ConvertedSI)row[SummaryDataContainer.E_ROLL]);
				var eGrad = ((ConvertedSI)row[SummaryDataContainer.E_GRAD]);
				var cargoVolume = mode == ExecutionMode.Engineering ? 0.0 : ((ConvertedSI)row[SummaryDataContainer.CARGO_VOLUME]);

				var loadingValue = ((ConvertedSI)row[SummaryDataContainer.LOADING]) / 1000;
				var fcPer100km = ((ConvertedSI)row[SummaryDataContainer.FCFINAL_LITERPER100KM]);
				var fcPerVolume = mode == ExecutionMode.Engineering
					? 0.0
					: ((ConvertedSI)row[SummaryDataContainer.FCFINAL_LiterPer100M3KM]);
				var fcPerLoad = loadingValue > 0 ? ((ConvertedSI)row[SummaryDataContainer.FCFINAL_LITERPER100TKM]) : 0.0;
				var co2PerKm = ((ConvertedSI)row[SummaryDataContainer.CO2_KM]);
				var co2PerVolume = mode == ExecutionMode.Engineering ? 0.0 : ((ConvertedSI)row[SummaryDataContainer.CO2_M3KM]);
				var co2PerLoad = loadingValue > 0 ? ((ConvertedSI)row[SummaryDataContainer.CO2_TKM]) : 0.0;

				var ePTOtransm = ptoTransmissionColumn != null ? ((ConvertedSI)row[ptoTransmissionColumn]) : 0.0;
				var ePTOconsumer = ptoConsumerColumn != null ? ((ConvertedSI)row[ptoConsumerColumn]) : 0.0;

				// E_fcmap_pos = E_fcmap_neg + E_powertrain_inertia + E_aux_xxx + E_aux_sum + E_clutch_loss + E_tc_loss + E_gbx_loss + E_shift_loss + E_ret_loss + E_angle_loss + E_axl_loss + E_brake + E_vehicle_inertia + E_air + E_roll + E_grad + E_PTO_CONSUM + E_PTO_TRANSM
				Assert.AreEqual(eFcMapPos,
					eFcMapNeg + ePowertrainInertia + eAux + eClutchLoss + eTcLoss + eGbxLoss + eRetLoss + eAngleLoss +
					eAxlLoss + eBrakeLoss + eVehInertia + eAir + eRoll + eGrad + ePTOconsumer + ePTOtransm, 1e-5,
					"input file: {0}  cycle: {1} loading: {2}",
					inputFile, cycle, loading);

				var pFcmapPos = ((ConvertedSI)row[SummaryDataContainer.P_FCMAP_POS]);
				var time = ((ConvertedSI)row[SummaryDataContainer.TIME]);

				// E_fcmap_pos = P_fcmap_pos * t
				Assert.AreEqual(eFcMapPos, pFcmapPos * (time / 3600), 1e-3, "input file: {0}  cycle: {1} loading: {2}", inputFile,
					cycle, loading);

				if (cargoVolume > 0) {
					Assert.AreEqual(fcPerVolume, fcPer100km / cargoVolume, 1e-3, "input file: {0}  cycle: {1} loading: {2}", inputFile,
						cycle, loading);

					Assert.AreEqual(co2PerVolume, co2PerKm / cargoVolume, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile,
						cycle, loading);
				}

				if (loadingValue > 0) {
					Assert.AreEqual(co2PerLoad, co2PerKm / loadingValue, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
					Assert.AreEqual(fcPerLoad, fcPer100km / loadingValue, 1e-3, "input file: {0}  cycle: {1} loading: {2}",
						inputFile, cycle, loading);
				}

				var stopTimeShare = ((ConvertedSI)row[SummaryDataContainer.STOP_TIMESHARE]);
				var accTimeShare = ((ConvertedSI)row[SummaryDataContainer.ACC_TIMESHARE]);
				var decTimeShare = ((ConvertedSI)row[SummaryDataContainer.DEC_TIMESHARE]);
				var cruiseTimeShare = ((ConvertedSI)row[SummaryDataContainer.CRUISE_TIMESHARE]);

				Assert.AreEqual(100, stopTimeShare + accTimeShare + decTimeShare + cruiseTimeShare, 1e-3,
					"input file: {0}  cycle: {1} loading: {2}", inputFile, cycle, loading);

				Assert.IsTrue(((ConvertedSI)row[SummaryDataContainer.ACC_POS]) > 0);
				Assert.IsTrue(((ConvertedSI)row[SummaryDataContainer.ACC_NEG]) < 0);

				var gearshifts = ((ConvertedSI)row[SummaryDataContainer.NUM_GEARSHIFTS]);
				Assert.IsTrue(gearshifts > 0);

				//var acc = ((SI)row[SummaryDataContainer.ACC]).Value();
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

				var pWheelInertia = (Watt)row[(int)ModalResultField.P_wheel_inertia];
				var pPTOconsumer = ptoConsumerColumn == null || row[ptoConsumerColumn.ColumnName] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoConsumerColumn.ColumnName];
				var pPTOtransm = ptoTransmissionColumn == null || row[ptoTransmissionColumn.ColumnName] is DBNull
					? 0.SI<Watt>()
					: (Watt)row[ptoTransmissionColumn.ColumnName];
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

				var pClutchLoss = (Watt)(row[(int)ModalResultField.P_clutch_loss] != DBNull.Value
					? row[(int)ModalResultField.P_clutch_loss]
					: 0.SI<Watt>());

				var pClutchOut = row[(int)ModalResultField.P_clutch_out];
				if (pClutchOut != DBNull.Value) {
					Assert.AreEqual(pGbxIn.Value(), (pClutchOut as Watt).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);
					Assert.AreEqual(pEngOut.Value(), (pClutchOut as Watt + pClutchLoss).Value(), 1E-3, "time: {0}  distance: {1}",
						time, distance);
				}

				var pTC_Loss = (Watt)(row[(int)ModalResultField.P_TC_loss] != DBNull.Value
					? row[(int)ModalResultField.P_TC_loss]
					: 0.SI<Watt>());

				var pTCOut = row[(int)ModalResultField.P_clutch_out];
				if (pTCOut != DBNull.Value) {
					Assert.AreEqual(pGbxIn.Value(), (pTCOut as Watt).Value(), 1E-3, "time: {0}  distance: {1}", time, distance);
					//Assert.AreEqual(pEngOut.Value(), (pTCOut as Watt + pTC_Loss).Value(), 1E-3, "time: {0}  distance: {1}",
					//	time, distance);
				}

				Assert.IsTrue(pLossGbx.IsGreaterOrEqual(pShiftLoss + pGbxInertia), "time: {0}  distance: {1}", time,
					distance);

				Assert.AreEqual(pGbxIn.Value(), (pRetIn + pLossGbx + pGbxInertia).Value(), gear != 0 ? 1E-3 : 0.5,
					"time: {0}  distance: {1}", time,
					distance);

				// P_eng_fcmap = P_eng_out + P_AUX + P_eng_inertia ( + P_PTO_Transm + P_PTO_Consumer )
				Assert.AreEqual(pEngFcmap.Value(), (pEngOut + pAux + pEngInertia + pPTOtransm + pPTOconsumer).Value(), 0.5,
					"time: {0}  distance: {1}", time, distance);

				// P_eng_fcmap = sum(Losses Powertrain)
				var pLossTot = pClutchLoss + pTC_Loss + pLossGbx + pLossRet + pGbxInertia + pLossAngle + pLossAxle + pBrakeLoss +
								pWheelInertia + pAir + pRoll + pGrad + pVehInertia + pPTOconsumer + pPTOtransm;
				var pEngFcmapCalc = (pLossTot + pEngInertia + pAux).Value();
				Assert.AreEqual(pEngFcmap.Value(), pEngFcmapCalc, 0.5, "time: {0}  distance: {1}", time, distance);

				Assert.AreEqual(pEngFcmap.Value(),
					(pTrac + pWheelInertia + pBrakeLoss + pLossAxle + pLossRet + pLossGbx + pGbxInertia + pEngInertia + pAux +
					pClutchLoss + pTC_Loss + pPTOtransm + pPTOconsumer).Value(), 0.5, "time: {0}  distance: {1}", time, distance);
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
					((DistanceBasedDrivingCycle)((VehicleContainer)run.Run.GetContainer()).DrivingCycle).Data.Entries.Last()
						.Distance));
			}
			var auxKeys =
				new Dictionary<string, DataColumn>(
					((ModalDataContainer)jobContainer.Runs.First().Run.GetContainer().ModalData).Auxiliaries);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			foreach (var modalResults in modData) {
				AssertModDataIntegrityAT(modalResults.Item1, auxKeys, modalResults.Item2,
					FuelConsumptionMapReader.Create(((IEngineeringInputDataProvider)inputData).JobInputData.Vehicle.EngineInputData.FuelConsumptionMap));
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
