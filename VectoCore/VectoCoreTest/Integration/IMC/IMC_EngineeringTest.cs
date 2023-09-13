using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.IMC;

public class IMC_EngineeringTest
{
	private const string PEV_IMC_Grp5 = "TestData/IMC/BEV_E2_Group5_2030LH_rl_Electr_Gear/BEV_Group5LH_rl_Electr_Gear.vecto";

    [TestCase]
    public void RunJob_PHEV_IMC_Engineering()
    {
        var jobFile = "TestData/IMC/P2_PHEV_Group5_2030LH_rl_ENG/P2_Group5LH_rl.vecto";
        var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
        var writer = new FileOutputWriter(jobFile);
        var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
        factory.Validate = false;
        factory.WriteModalResults = true;
        factory.SumData = new SummaryDataContainer(writer);
        var run = factory.SimulationRuns().ToArray()[0];
        var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

        run.Run();
        Assert.IsTrue(run.FinishedWithoutErrors);
    }

	[TestCase (PEV_IMC_Grp5, 0, TestName = "PEV IMC Engineering Grp5 LH")]
	[TestCase(PEV_IMC_Grp5, 1, TestName = "PEV IMC Engineering Grp5 RD")]
	public void RunJob_PEV_IMC_Engineering(string jobFile, int runIdx)
	{
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);
		var writer = new FileOutputWriter(jobFile);
		var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;
		factory.SumData = new SummaryDataContainer(writer);

		var run = factory.SimulationRuns().ToArray()[runIdx];
		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);
	}
}