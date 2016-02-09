/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NLog;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestClass]
	public class FullCycleDeclarationTest
	{
		public const string TruckDeclarationJob =
			@"TestData\Integration\DeclarationMode\40t Truck\40t_Long_Haul_Truck.vecto";


		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_LongHaulCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("LongHaul");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_LongHaulCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_RegionalDeliveryCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("RegionalDelivery");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_RegionalDeliveryCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>(), true);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}


		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_UrbanDeliveryCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("UrbanDelivery");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_UrbanDeliveryCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_MunicipalCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("MunicipalUtility");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_MunicipalCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_ConstructionCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Construction");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_ConstructionCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_HeavyUrbanCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("HeavyUrban");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_HeavyUrbanCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_SubUrbanCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Suburban");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_SubUrbanCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_InterUrbanCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Interurban");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_InterUrbanCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40t_CoachCycle_RefLoad()
		{
			var cycle = SimpleDrivingCycles.ReadDeclarationCycle("Coach");
			var run = Truck40tPowerTrain.CreateEngineeringRun(cycle, "Truck40t_CoachCycle_RefLoad.vmod",
				7500.SI<Kilogram>(), 12900.SI<Kilogram>());

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, TestCategory("LongRunning")]
		public void Truck40tDeclarationTest()
		{
			LogManager.DisableLogging();

			var inputData = JSONInputDataFactory.ReadJsonJob(TruckDeclarationJob);
			var fileWriter = new FileOutputWriter(Path.GetFileNameWithoutExtension(TruckDeclarationJob),
				Path.GetDirectoryName(TruckDeclarationJob));
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter) {
				WriteModalResults = true
			};
			var sumData = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumData);
			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			foreach (var run in jobContainer.Runs) {
				Assert.IsTrue(run.Run.FinishedWithoutErrors);
			}
		}

		[TestMethod, Ignore]
		public void Truck40t_RegionalDeliveryCycle_RefLoad_Declaration()
		{
			const string jobFile = @"c:\Users\Technik\Downloads\40t Long Haul Truck\40t_Long_Haul_Truck.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var fileWriter = new FileOutputWriter(jobFile);
			var factory = new SimulatorFactory(ExecutionMode.Declaration,
				inputData, fileWriter) {
					WriteModalResults = true,
					SumData = new SummaryDataContainer(fileWriter)
				};
			var runs = factory.SimulationRuns().ToArray();

			var run = runs[4];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, Ignore]
		public void Truck12t_LongHaulCycle_RefLoad_Declaration()
		{
			const string jobFile = @"c:\Users\Technik\Downloads\12t Delivery Truck\12t Delivery Truck.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var fileWriter = new FileOutputWriter(jobFile);

			// TODO: fails due to interpolaion failure in Gear 4
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter) {
				WriteModalResults = true,
				SumData = new SummaryDataContainer(fileWriter)
			};
			var runs = factory.SimulationRuns().ToArray();

			var run = runs[1];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		[TestMethod, Ignore]
		public
		void Truck12t_UrbanDeliveryCycle_RefLoad_Declaration()
		{
			var jobFile = @"c:\Users\Technik\Downloads\12t Delivery Truck\12t Delivery Truck.vecto";
			var inputData = JSONInputDataFactory.ReadJsonJob(jobFile);
			var fileWriter = new FileOutputWriter(jobFile);

			// TODO: fails due to interpolaion failure in Gear 4
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter) {
				WriteModalResults = true,
				SumData = new SummaryDataContainer(fileWriter)
			};
			var runs = factory.SimulationRuns().ToArray();
			var run = runs[7];
			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}
}