using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.RoadSweepers
{
	[TestFixture]
	public class RoadSweeperTests
	{
		public const string RoadSweeperJob =
			@"TestData\RoadSweepers\Class9_RigidTruck_6x2_PTO\Class9_RigidTruck_ENG_PTO.vecto";

		public const string RoadSweeperVehicle =
				@"E:\QUAM\Workspace\VECTO-RoadSweepers\VectoCore\VectoCoreTest\TestData\RoadSweepers\Class9_RigidTruck_6x2_PTO\Class9_RigidTruck.vveh"
			;

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase()]
		public void RoadSweeper_StartDriveStop()
		{
			var cycle = "   0, 0, 0, 5, 0, 3, 0   \n" +
						"   1, 8, 0, 0, 2, 3, 10  \n" +
						"1000, 0, 0, 5, 0, 3, 0   \n" +
						"1001, 8, 0, 0, 0, 3, 0   \n" +
						"1200, 0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_Stop_PTO-active");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOffDuringDrive()
		{
			var cycle = "   0, 0, 0, 5, 0, 3, 0   \n" +
						"   1, 8, 0, 0, 2, 3, 10  \n" +
						" 800, 8, 0, 0, 0, 3, 10  \n" +
						"1000, 0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_off_during_drive");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOnDuringDrive()
		{
			var cycle = "   0, 0, 0, 5, 0, 3, 0   \n" +
						"   1, 8, 0, 0, 0, 3, 10  \n" +
						" 200, 8, 0, 0, 2, 3, 10  \n" +
						"1000, 0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_on_during_drive");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOnDuringDriveFromHigherSpeed()
		{
			var cycle = "   0,  0, 0, 5, 0, 3, 0   \n" +
						"   1, 20, 0, 0, 0, 3, 10  \n" +
						" 200,  8, 0, 0, 2, 3, 10  \n" +
						"1000,  0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_on_during_drive_from_higher_speed");
		}

		[TestCase()]
		public void RoadSweeper_SwitchPTOOnDuringDriveWithHigherSpeed()
		{
			var cycle = "   0,  0, 0, 5, 0, 3, 0   \n" +
						"   1, 20, 0, 0, 0, 3, 10  \n" +
						" 200, 20, 0, 0, 2, 3, 10  \n" +
						"1000,  0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_switch_PTO_on_during_drive_with_higher_speed");
		}


		[TestCase()]
		public void RoadSweeper_ChangePTOLoadDuringSweeping()
		{
			var cycle = "   0,  0, 0, 5, 0, 3, 0   \n" +
						"   1,  8, 0, 0, 2, 3, 10  \n" +
						" 400,  8, 0, 0, 2, 3, 20  \n" +
						"1000,  0, 0, 5, 0, 3, 0   \n";
			EngineeringRunWithCycle(RoadSweeperJob, cycle, "Accelerate_Drive_change_PTO_load_while_sweeping");
		}


		[TestCase(RoadSweeperJob, 1, TestName = "RoadSweeper 1")]
		public void RoadSweeperTest(string jobFile, int idx)
		{
			EngineeringSimulationRun(idx, jobFile);
		}

		public void EngineeringSimulationRun(int cycleIdx, string jobName)
		{
			var inputData = JSONInputDataFactory.ReadJsonJob(jobName);
			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobName), Path.GetFileName(jobName)));

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};

			factory.SumData = sumContainer;

			var runs = factory.SimulationRuns().ToArray();
			var run = runs[cycleIdx];

			jobContainer.AddRun(run);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
		}


		public void EngineeringRunWithCycle(string jobFile, string cycleData, string testName)
		{
			var job = (IEngineeringInputDataProvider)JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(Path.Combine(Path.GetDirectoryName(jobFile), Path.GetFileName(testName)));

			var cycle = (ICycleData)new CycleInputData() {
				Name = testName,
				CycleData = VectoCSVFile.ReadStream(InputDataHelper.InputDataAsStream("s,v,grad,stop,PTO, Padd, P_PTO", cycleData.Split('\n')), source: testName)
			};

			var inputData = new EngineeringJobInputData() {
				JobName = job.JobInputData.JobName,
				Vehicle = job.JobInputData.Vehicle,
				DriverInputData = job.DriverInputData,
				Cycles = (new[] {cycle}).ToList()
			};
			

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
				//ActualModalData = true,
				Validate = false
			};

			factory.SumData = sumContainer;

			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));
		}

		public class EngineeringJobInputData : IEngineeringInputDataProvider, IEngineeringJobInputData
		{
			private bool _savedInDeclarationMode = false;

			#region Implementation of IInputDataProvider

			public DataSource DataSource { get { return new DataSource(); } }

			#endregion

			#region Implementation of IEngineeringInputDataProvider

			public IEngineeringJobInputData JobInputData { get { return this; } }
			public IDriverEngineeringInputData DriverInputData { get; set; }

			#endregion

			#region Implementation of IDriverDeclarationInputData

			

			public IVehicleEngineeringInputData Vehicle { get; set; }
			public IList<ICycleData> Cycles { get; set; }
			public bool EngineOnlyMode { get { return false; } }
			public IEngineEngineeringInputData EngineOnly { get { return null; } }

			IVehicleDeclarationInputData IDeclarationJobInputData.Vehicle
			{
				get { return Vehicle; }
			}

			public string JobName { get; set; }

			#endregion

			#region Implementation of IDeclarationJobInputData

			bool IDeclarationJobInputData.SavedInDeclarationMode
			{
				get { return _savedInDeclarationMode; }
			}

			#endregion
		}
	}
}
