using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	public class ShiftStrategyV2Test
	{
		public const string Class9Decl =
			@"TestData\Generic Vehicles\Declaration Mode\Class9_RigidTruck_6x2\Class9_RigidTruck_DECL.vecto";


		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(Class9Decl, 0),
		TestCase(@"E:\QUAM\Downloads\2018-07-13_Prototype-TCU-VECTO_R2016a\a_input_Data\Cl-5_tractor_directdr_axle_2p53.xml", 0),
		TestCase(@"E:\QUAM\Downloads\2018-07-13_Prototype-TCU-VECTO_R2016a\a_input_Data\Cl-5_tractor_directdr_axle_2p53.xml", 1),
		TestCase(@"E:\QUAM\Downloads\2018-07-13_Prototype-TCU-VECTO_R2016a\a_input_Data\Cl-5_tractor_directdr_axle_2p53.xml", 4)
		]
		public void TestSShiftStrategyV2(string jobFile, int i)
		{
			var fileWriter = new FileOutputWriter(jobFile);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var dataProvider = Path.GetExtension(jobFile) == ".vecto"
				? JSONInputDataFactory.ReadJsonJob(jobFile)
				: new XMLDeclarationInputDataProvider(jobFile, true);
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = true,
				ActualModalData = false,
				Validate = false,
				SumData = sumWriter,
			};

			var runs = runsFactory.SimulationRuns().ToArray();

			jobContainer.AddRun(runs[i]);

			jobContainer.Runs[0].Run.Run();

			Assert.IsTrue(jobContainer.Runs[0].Run.FinishedWithoutErrors);
			jobContainer.Execute();
			jobContainer.WaitFinished();
		}
	}

}
