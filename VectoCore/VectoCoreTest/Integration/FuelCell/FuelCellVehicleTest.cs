using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.SimulationComponentData;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Tests.Utils.RunDataHelper;
using TUGraz.VectoCore.Utils;
using ElectricSystem = TUGraz.VectoCore.Models.SimulationComponent.ElectricSystem;


namespace TUGraz.VectoCore.Tests.Integration.FuelCell
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class FuelCellVehicleTest
	{

		protected const string FCHV_E2_JOB = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";

		protected const string FCHV_E2_JOB_300kW = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc_300kW_fc.vecto";
        protected const string FCHV_E2_JOB_multipleFC = @"TestData/H2_FCV/GenericVehicleE2 - FCHV/FCHV_singleFc.vecto";



        [OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(FCHV_E2_JOB, 0, TestName = "FCHV E2 Job RD single FC")]
		
		[TestCase(FCHV_E2_JOB_multipleFC, 0, 
			TestName="FCHV E2 Job RD multiple FC", 
			IgnoreReason = "MultipleFC not implemented")]


		public void E2_FCHV_Job(string jobFile, int cycleIdx)
		{
			var run = GetRun(jobFile, cycleIdx, out var pt, out _);
			TestContext.Progress.WriteLine(
				$"Usable energy {pt.RunData.BatteryData.UseableStoredEnergy.ConvertToKiloWattHour()}");
            run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);
		}

		private static IVectoRun GetRun(string jobFile, int cycleIdx, out IVehicleContainer pt, out IEngineeringInputDataProvider inputProvider)
		{
			inputProvider = JSONInputDataFactory.ReadJsonJob(jobFile) as IEngineeringInputDataProvider;

			var writer = new FileOutputWriter(jobFile);
			var factory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Engineering, inputProvider, writer);
			factory.Validate = false;
			factory.WriteModalResults = true;

			var sumContainer = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			var run = factory.SimulationRuns().ToArray()[cycleIdx];

			Assert.NotNull(run);

			pt = run.GetContainer();

			Assert.NotNull(pt);
			return run;
		}


		[TestCase(FCHV_E2_JOB, 0, 100, 10, 100, TestName = "FCHV E2 Job RD single FC, 100kWh  , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 100, 10, 300, TestName = "FCHV E2 Job RD single FC, 100kWh  , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 100, 10, 500, TestName = "FCHV E2 Job RD single FC, 100kWh  , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 80,  10, 100, TestName = "FCHV E2 Job RD single FC, 80kWh   , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 80,  10, 300, TestName = "FCHV E2 Job RD single FC, 80kWh   , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 80,  10, 500, TestName = "FCHV E2 Job RD single FC, 80kWh   , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 50,  10, 100, TestName = "FCHV E2 Job RD single FC, 50kWh   , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 50,  10, 300, TestName = "FCHV E2 Job RD single FC, 50kWh   , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 50,  10, 500, TestName = "FCHV E2 Job RD single FC, 50kWh   , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 35,  10, 100, TestName = "FCHV E2 Job RD single FC, 35kWh   , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 35,  10, 300, TestName = "FCHV E2 Job RD single FC, 35kWh   , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 35,  10, 500, TestName = "FCHV E2 Job RD single FC, 35kWh   , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 20,  10, 100, TestName = "FCHV E2 Job RD single FC, 20kWh   , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 20,  10, 300, TestName = "FCHV E2 Job RD single FC, 20kWh   , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 20,  10, 500, TestName = "FCHV E2 Job RD single FC, 20kWh   , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 10,  10, 100, TestName = "FCHV E2 Job RD single FC, 10kWh   , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 10,  10, 300, TestName = "FCHV E2 Job RD single FC, 10kWh   , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 10,  10, 500, TestName = "FCHV E2 Job RD single FC, 10kWh   , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 5,   10, 100, TestName = "FCHV E2 Job RD single FC, 5kWh    , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 5,   10, 300, TestName = "FCHV E2 Job RD single FC, 5kWh    , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 5,   10, 500, TestName = "FCHV E2 Job RD single FC, 5kWh    , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 1,   10, 100, TestName = "FCHV E2 Job RD single FC, 1kWh    , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 1,   10, 300, TestName = "FCHV E2 Job RD single FC, 1kWh    , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 1,   10, 500, TestName = "FCHV E2 Job RD single FC, 1kWh    , 500 kW")]
		[TestCase(FCHV_E2_JOB, 0, 0.1, 10, 100, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 100 kW")]
		[TestCase(FCHV_E2_JOB, 0, 0.1, 10, 300, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 300 kW")]
		[TestCase(FCHV_E2_JOB, 0, 0.1, 10, 500, TestName = "FCHV E2 Job RD single FC, 0.1kWh  , 500 kW")]

		#region

		//[TestCase(FCHV_E2_JOB_300kW, 0, 100, TestName = "FCHV E2 Job RD single FC_300kW, 100kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 80,  TestName = "FCHV E2 Job RD single FC_300kW, 80kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 70,  TestName = "FCHV E2 Job RD single FC_300kW, 70kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 60,  TestName = "FCHV E2 Job RD single FC_300kW, 60kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 50,  TestName = "FCHV E2 Job RD single FC_300kW, 50kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 35,  TestName = "FCHV E2 Job RD single FC_300kW, 35kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 20,  TestName = "FCHV E2 Job RD single FC_300kW, 20kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 10,  TestName = "FCHV E2 Job RD single FC_300kW, 10kWh")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 5,   TestName = "FCHV E2 Job RD single FC_300kW, 5kWh  ")]
		//[TestCase(FCHV_E2_JOB_300kW, 0, 1,   TestName = "FCHV E2 Job RD single FC_300kW, 1kWh  ")]

		//[TestCase(FCHV_E2_JOB, 1, 100, TestName = "1 FCHV E2 Job RD single FC, 100kWh")]
		//[TestCase(FCHV_E2_JOB, 1, 80,  TestName = "1 FCHV E2 Job RD single FC, 80 kWh")]
		//[TestCase(FCHV_E2_JOB, 1, 20,  TestName = "1 FCHV E2 Job RD single FC, 20 kWh")]
		//[TestCase(FCHV_E2_JOB, 1, 10,  TestName = "1 FCHV E2 Job RD single FC, 10 kWh")]
		//[TestCase(FCHV_E2_JOB, 1, 5,   TestName = "1 FCHV E2 Job RD single FC, 5kWh")]
		//[TestCase(FCHV_E2_JOB, 1, 1,   TestName = "1 FCHV E2 Job RD single FC, 1kWh")]

		//[TestCase(FCHV_E2_JOB, 2, 100, TestName = "2 FCHV E2 Job RD single FC, 100kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 80,  TestName = "2 FCHV E2 Job RD single FC, 80 kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 20,  TestName = "2 FCHV E2 Job RD single FC, 20 kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 10,  TestName = "2 FCHV E2 Job RD single FC, 10 kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 5,   TestName = "2 FCHV E2 Job RD single FC, 5kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 1,   TestName = "2 FCHV E2 Job RD single FC, 1kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 0.1, TestName = "2 FCHV E2 Job RD single FC, 0.1kWh")]

		//[TestCase(FCHV_E2_JOB, 2, 100, TestName = "2 FCHV E2 Job RD single FC, 100kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 80, TestName = "2 FCHV E2 Job RD single FC, 80 kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 20, TestName = "2 FCHV E2 Job RD single FC, 20 kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 10, TestName = "2 FCHV E2 Job RD single FC, 10 kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 5, TestName = "2 FCHV E2 Job RD single FC, 5kWh")]
		//[TestCase(FCHV_E2_JOB, 2, 0.1, TestName = "2 FCHV E2 Job RD single FC, 0.1kWh")]

		//[TestCase(FCHV_E2_JOB, 3, 100, TestName = "3 FCHV E2 Job RD single FC, 100kWh")]
		//[TestCase(FCHV_E2_JOB, 3, 80,  TestName = "3 FCHV E2 Job RD single FC, 80 kWh")]
		//[TestCase(FCHV_E2_JOB, 3, 20,  TestName = "3 FCHV E2 Job RD single FC, 20 kWh")]
		//[TestCase(FCHV_E2_JOB, 3, 10,  TestName = "3 FCHV E2 Job RD single FC, 10 kWh")]
		//[TestCase(FCHV_E2_JOB, 3, 5,   TestName = "3 FCHV E2 Job RD single FC, 5kWh")]
		//[TestCase(FCHV_E2_JOB, 3, 1,   TestName = "3 FCHV E2 Job RD single FC, 1kWh")]
#endregion

		public void E2_FCHV_Job_var_capacity(string jobFile, int cycleIdx, double usable_energy_kWh, double min_fcPower_kW, double max_fcPower_kW)
		{
			var run = GetRun(jobFile, cycleIdx, out var pt, out var inputData);
			var components = inputData.JobInputData.Vehicle.Components;
			//components.FuelCellSystemInputData = new Mock<IFuelCellSystemEngineeringInputData>().Object;

			var fcProperty = components.GetType().GetField("_fuelCellSystem", System.Reflection.BindingFlags.NonPublic
																			| System.Reflection.BindingFlags.Instance);
			fcProperty.SetValue(components, GetFuelCellSystemInputData((max_fcPower_kW * 1E3).SI<Watt>(), (min_fcPower_kW * 1E3).SI<Watt>()));

			TestContext.Progress.WriteLine(
				$"FC ({components.FuelCellSystemInputData.FuelCellComponents.Single().FuelCellComponent.MinElectricPower}/" +
				$"{components.FuelCellSystemInputData.FuelCellComponents.Single().FuelCellComponent.MaxElectricPower}) ");

			
			var engineeringDao = new EngineeringDataAdapter();
			var vehicle = inputData.JobInputData.Vehicle;

		

			var batData = engineeringDao.CreateBatteryData(vehicle.Components.ElectricStorage, vehicle.InitialSOC);
			var rd = pt.RunData;

			var usableWs = usable_energy_kWh.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>();
			batData = batData.SetUsableCapacity(usableWs);

			rd.BatteryData =
				engineeringDao.CreateFuelCellPreProcessingBattery(vehicle.Components.FuelCellSystemInputData, batData.Clone());


			Assert.AreEqual(usable_energy_kWh, batData.UseableStoredEnergy.ConvertToKiloWattHour().Value, 1E-3);

			try {
				run.Run();
			} catch (Exception ex) {
				rd = run.GetContainer().RunData;
				var postProcessing = rd.FuelCellSystemData.PostProcessing;
				TestContext.Progress.WriteLine(ex.Message);

				throw;
			}
	
			Assert.IsTrue(run.FinishedWithoutErrors);
			Assert.AreEqual(max_fcPower_kW, run.GetContainer().RunData.FuelCellSystemData.FuelCells.First().MaxElectricPower.ConvertToKiloWatt().Value);

			rd = run.GetContainer().RunData;

			var fcs = rd.FuelCellSystemData;
			var pP = fcs.PostProcessing;

			TestContext.Progress.WriteLine($"Window Size {pP.WindowSize}," +
											$"found after {pP.BinarySearchIterations} iterations" +
											$"SoC {pP.SoC}");


		}

		



		IFuelCellSystemEngineeringInputData GetFuelCellSystemInputData(Watt maxPower, Watt minPower)
		{
			var massFlowMap = new TableData() { };
			massFlowMap.Columns.AddRange(new DataColumn[] {
				new DataColumn() {
					ColumnName = FuelCellMassFlowMapReader.Fields.ElectricPower,
					DataType = typeof(string),
                },
				new DataColumn() {
					ColumnName = FuelCellMassFlowMapReader.Fields.m_H2,
					DataType = typeof(string),
				}
			});

			var nrOfEntries = 10;
			var maxPower_kW = maxPower.Value() / 1000;
			var minPower_kW = minPower.Value() / 1000;
			var step = (maxPower_kW - minPower_kW) / nrOfEntries;


			for (int i = 0; i <= nrOfEntries; i++) {
				var row = massFlowMap.NewRow();
				var power = minPower_kW + step * i;
				row[0] = power.ToString(CultureInfo.InvariantCulture);
				row[1] = (power * 100).ToString(CultureInfo.InvariantCulture);


				massFlowMap.Rows.Add(row);
			}

			var fuelCellSystemMock = new Mock<IFuelCellSystemEngineeringInputData>();
			fuelCellSystemMock.Setup(fcs => fcs.GradientPowerChange).Returns(int.MaxValue.SI<WattPerSecond>());
			fuelCellSystemMock.Setup(fcs => fcs.OnOffHysteresis).Returns(0.SI<Second>());

			var fuelCellComponentMock = new Mock<IFuelCellComponentEngineeringInputData>();
			var fuelCellComponentEntry = new FuelCellComponentEntry<IFuelCellComponentEngineeringInputData>() {
				Count = 1,
				FuelCellComponent = fuelCellComponentMock.Object,
			};
			fuelCellSystemMock.Setup(fcs => fcs.FuelCellComponents).Returns(
				new List<FuelCellComponentEntry<IFuelCellComponentEngineeringInputData>>() {
					fuelCellComponentEntry
				});

			fuelCellComponentMock.Setup(fcc => fcc.MaxElectricPower)
				.Returns(maxPower);
			fuelCellComponentMock.Setup(fcc => fcc.MinElectricPower)
				.Returns(minPower);
			fuelCellComponentMock.Setup(fcc => fcc.MassFlowMap)
				.Returns(massFlowMap);

            return fuelCellSystemMock.Object;
		}
	}

}
