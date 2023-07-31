using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation;


[TestFixture]
public class FuelCellPreRunPostprocessingT
{

	[TestCase()]
	public void FuelCellPreRunPostprocessingTest()
	{

	}

	[TestCase()]
	public void FuelCellPostProcessing_DistanceWindow()
	{
		string jobFile = "TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";
		int cycleIdx = 0;

        var modData = RunFCHV_PEV_Simulation(jobFile, cycleIdx);

		var fcPostProcessor = new FuelCellPreRunPostprocessor(modData);
	}

	private static IModalDataContainer RunFCHV_PEV_Simulation(string jobFile, int cycleIdx)
	{
		var inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile);

		var writer = new FileOutputWriter(jobFile);
		var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
		factory.Validate = false;
		factory.WriteModalResults = true;

		var sumContainer = new SummaryDataContainer(writer);
		var jobContainer = new JobContainer(sumContainer);

		factory.SumData = sumContainer;

		var run = factory.SimulationRuns().ToArray()[cycleIdx];
		run.GetContainer().RunData.IterativeRunStrategy = null;

		Assert.NotNull(run);

		var pt = run.GetContainer();

		Assert.NotNull(pt);

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);

		return run.GetContainer().ModalData;
	}


}