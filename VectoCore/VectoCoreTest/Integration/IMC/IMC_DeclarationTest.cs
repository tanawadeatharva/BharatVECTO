using System.Linq;
using System.Threading;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Integration.IMC;

public class IMC_DeclarationTest
{
	private ThreadLocal<StandardKernel> _kernel;
	private IXMLInputDataReader _xmlReader;


	public const string PEV_IMC_Grp5_all = "TestData/IMC/Declaration/BEV_E2_Group5_2030/BEV_Group5_IMC.vecto";
	public const string HEV_IMC_Grp5_all = "TestData/IMC/Declaration/P2_PHEV_Group5_2030/P2_Group5_IMC.vecto";

    public StandardKernel Kernel {
		get => _kernel.Value;
	}

    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		_xmlReader = Kernel.Get<IXMLInputDataReader>();
	}


    [TestCase(PEV_IMC_Grp5_all, 3, TestName = "PEV IMC All Declaration Grp5 RD RL")]
	public void RunJob_PEV_IMC_Declaration(string jobFile, int runIdx)
	{
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);
		var run = factory.SimulationRuns().ToArray()[runIdx];
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
	}

	[TestCase(HEV_IMC_Grp5_all, MissionType.RegionalDelivery, LoadingType.ReferenceLoad, TestName = "P-HEV IMC All Declaration Grp5 RD RL")]
	public void RunJob_PHEV_IMC_Declaration(string jobFile, MissionType? mission = null, LoadingType? loading = null)
	{
		if (mission != null && loading != null) {
			Kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
			var missionFilter = Kernel.Get<IMissionFilter>() as TestMissionFilter;
			missionFilter.SetMissions((mission.Value, loading.Value));
        }

		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = Kernel.Get<ISimulatorFactoryFactory>().Factory(ExecutionMode.Declaration, inputProvider, writer, null, null);
		//SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;
		var sumData = new SummaryDataContainer(writer);

		var jobContainer = new JobContainer(sumData);
		jobContainer.AddRuns(factory);

		jobContainer.Execute(false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
	}
}