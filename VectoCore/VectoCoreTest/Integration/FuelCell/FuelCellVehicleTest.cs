using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.SimulationComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.ElectricSystem;


namespace TUGraz.VectoCore.Tests.Integration.FuelCell
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FuelCellVehicleTest
	{

		protected const string FCHV_E2_JOB = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";
		protected const string FCHV_E2_JOB_multipleFC = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";
        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		private GraphWriter GetGraphWriter(ModalResultField[] yFields)
		{
			return new GraphWriter();
			var graphWriter = new GraphWriter();
			//#if TRACE
			graphWriter.Enable();

			//#else
			//graphWriter.Disable();
			//#endif


			var Yfields = new[] {
				ModalResultField.v_act, ModalResultField.altitude, ModalResultField.acc, ModalResultField.Gear,
				ModalResultField.REESSStateOfCharge,
			}.Concat(yFields).ToArray();
			graphWriter.Xfields = new[] { ModalResultField.dist };
			graphWriter.Yfields = yFields;
			graphWriter.Series1Label = "BEV";
			graphWriter.PlotIgnitionState = true;

			return graphWriter;
		}


		[
			TestCase(FCHV_E2_JOB, 0, TestName = "FCHV E2 Job RD single FC"),
			TestCase(FCHV_E2_JOB_multipleFC, 0, TestName="FCHV E2 Job RD multiple FC"),
        ]
		public void E2_FCHV_Job(string jobFile, int cycleIdx)
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

			Assert.NotNull(run);

			var pt = run.GetContainer();

			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}
	}

}
