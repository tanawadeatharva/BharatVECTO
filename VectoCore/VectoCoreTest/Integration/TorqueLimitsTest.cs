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

using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class TorqueLimitsTest
	{
		const string GearboxLimitJobDecl_865 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxTorqueLimits\Class2_RigidTruck_gbxTqLimit-865_DECL.vecto";

		const string GearboxLimitJobDecl_800 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxTorqueLimits\Class2_RigidTruck_gbxTqLimit-800_DECL.vecto";


		const string VehicleLimitJobDecl_910 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-910_DECL.vecto";

		const string VehicleLimitJobDecl_850 =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_VehTorqueLimits\Class2_RigidTruck_vehTqLimit-850_DECL.vecto";

		const string GearboxSpeedLimitJobDecl =
			@"Testdata\Integration\DeclarationMode\Class2_RigidTruck_4x2_GbxSpeedLimits\Class2_RigidTruck_DECL.vecto";

		const string EngineSpeedLimitJobDecl =
			@"TestData\Integration\DeclarationMode\Class2_RigidTruck_4x2_engineSpeedlimit\Class2_RigidTruck_DECL.vecto";

		[TestCase()]
		public void TestGearboxTorqueLimitsAbove90FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(GearboxLimitJobDecl_865);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by gbx
			Assert.AreEqual(865, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by gbx but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by gbx but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestGearboxTorqueLimitsBelow90FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(GearboxLimitJobDecl_800);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by gbx
			Assert.AreEqual(800, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestVehicleTorqueLimitsAbove95FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(VehicleLimitJobDecl_910);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase()]
		public void TestVehicleTorqueLimitsBelow95FLD()
		{
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(VehicleLimitJobDecl_850);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null);

			var run = factory.DataReader.NextRun().First();

			var engineData = run.EngineData;

			// check default FLD
			Assert.AreEqual(956, engineData.FullLoadCurves[0].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[0].MaxDragTorque.Value());

			// check first gear - limited by vehicle but not applicaple
			Assert.AreEqual(956, engineData.FullLoadCurves[1].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[1].MaxDragTorque.Value());

			// check fourth gear - limited by vehicle
			Assert.AreEqual(850, engineData.FullLoadCurves[4].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[4].MaxDragTorque.Value());

			// check last gear - limited by vehicle 
			Assert.AreEqual(850, engineData.FullLoadCurves[6].MaxTorque.Value());
			Assert.AreEqual(-115, engineData.FullLoadCurves[6].MaxDragTorque.Value());
		}

		[TestCase(GearboxLimitJobDecl_800),
		TestCase(GearboxLimitJobDecl_865),
		TestCase(VehicleLimitJobDecl_850),
		TestCase(VehicleLimitJobDecl_910)]
		public void TestRunTorqueLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter) {
				WriteModalResults = true
			};


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCategory("LongRunning"), TestCase(GearboxSpeedLimitJobDecl)]
		public void TestRunGbxSpeedLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter) {
				WriteModalResults = true,
				ActualModalData = true
			};


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}

		[TestCase(EngineSpeedLimitJobDecl)]
		public void TestRunEngineSpeedLimitedSimulations(string file)
		{
			var fileWriter = new FileOutputWriter(file);
			var sumData = new SummaryDataContainer(fileWriter);
			var inputDataProvider = JSONInputDataFactory.ReadJsonJob(file);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, fileWriter) {
				WriteModalResults = true,
				//ActualModalData = true
			};


			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			//jobContainer.Runs[1].RunWorkerAsync().Wait();

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.IsTrue(jobContainer.Runs.All(r => r.Success), string.Concat(jobContainer.Runs.Select(r => r.ExecException)));
		}
	}
}