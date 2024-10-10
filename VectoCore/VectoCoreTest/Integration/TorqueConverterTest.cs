using NUnit.Framework;
using System;
using System.IO;
using System.Linq;
using Ninject;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Models.Simulation;

namespace TUGraz.VectoCore.Tests.Integration
{
    [TestFixture]
	[Parallelizable(ParallelScope.All)]
    public class TorqueConverterTest
    {
        public const String P1SerialJob = @"TestData/Integration/EngineeringMode/CityBus_AT/CityBus_AT_Ser-TC_all_gears.vecto";

		protected IPowertrainBuilder PowertrainBuilder;
		private IModalDataFactory ModDataFactory;

		[OneTimeSetUp]
        public void Init()
        {
            Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
			var kernel = new StandardKernel(new VectoNinjectModule());
			PowertrainBuilder = kernel.Get<IPowertrainBuilder>();
			ModDataFactory = kernel.Get<IModalDataFactory>();
		}

        [Category("Integration")]
        [
        TestCase(P1SerialJob, 0, TestName = "Torque Converter active for all gears")
        ]
        public void RunCycle(string jobFile, int cycleIdx)
        {
            var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

			var writer = new FileOutputWriter(jobFile);

			var factory = new SimulatorFactoryEngineering(inputProvider, writer, false, PowertrainBuilder, ModDataFactory) { WriteModalResults = false };
			factory.SumData = new SummaryDataContainer(writer);

			var run = factory.SimulationRuns().ToArray()[cycleIdx];
			run.Run();

			Assert.IsTrue(run.FinishedWithoutErrors);
        }
    }
}
