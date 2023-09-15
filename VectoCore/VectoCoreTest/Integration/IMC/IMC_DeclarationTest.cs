using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.Integration.IMC;

public class IMC_DeclarationTest
{

	public const string PEV_IMC_Grp5_all = "TestData/IMC/Declaration/BEV_E2_Group5_2030/BEV_Group5_IMC.vecto";

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
}