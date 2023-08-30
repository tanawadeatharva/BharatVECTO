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
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl;
using TUGraz.VectoCore.Utils;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Models.Simulation;


[TestFixture]
public class FuelCellPreRunPostprocessingT
{

	[TestCase()]
	public void FuelCellPreRunPostprocessingTest()
	{

	}

	[TestCase()]
	public void TestFuelCellWindowIterator_1()
	{
		// apply a constant value - the sum of all windows has to be the same
		var modData = new ModalDataContainer(new VectoRunData() {
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased,
			}
		}, null, null);
		modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
		modData.Data.CreateColumns(ModalResults.DriverSignals);
		for (var i = 1; i <= 100; i++) {
			modData[ModalResultField.dist] = i.SI<Meter>();
			modData[ModalResultField.simulationDistance] = 1.SI<Meter>();
			modData[ModalResultField.v_act] = 30.KMPHtoMeterPerSecond();
			modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
			modData.CommitSimulationStep();
		}

		var windowSize = 6.SI<Meter>();
		var wIt = new ModDataWindowIterator(modData, windowSize);
		for (; !wIt.CycleEndReached; wIt.MoveNext()) {
			var sum = 0.0;
			for (; !wIt.WindowEndReached; wIt.NextEntry()) {
				sum += ((MeterPerSecond)modData.Data.Rows[wIt.Current][ModalResultField.v_act.GetShortCaption()]).AsKmph;
			}
			Console.WriteLine($"{wIt.Position}, {wIt.Current}: {wIt.Start} - {wIt.End}: {sum}");
			Console.WriteLine($"    {modData.Data.Rows[wIt.Position][ModalResultField.dist.GetShortCaption()]}, " +
							$"{modData.Data.Rows[wIt.Start][ModalResultField.dist.GetShortCaption()]} - " +
							$"{modData.Data.Rows[wIt.End][ModalResultField.dist.GetShortCaption()]}: {sum}");
			Assert.AreEqual(180, sum, 1e-6, $"at position: {wIt.Position}, {modData.Data.Rows[wIt.Position][ModalResultField.dist.GetShortCaption()]}");
		}
    }

	[TestCase()]
	public void TestFuelCellWindowIterator_2()
	{
		// apply a sine wave (4 full waves) and the window size matches the period of the sine wave
		// the sum has to be 0. window size shall be an even number
		var modData = new ModalDataContainer(new VectoRunData() {
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased,
			}
		}, null, null);
		modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
		modData.Data.CreateColumns(ModalResults.DriverSignals);
		for (var i = 1; i <= 104; i++) {
			modData[ModalResultField.dist] = i.SI<Meter>();
			modData[ModalResultField.simulationDistance] = 1.SI<Meter>();
			modData[ModalResultField.v_act] = 30.KMPHtoMeterPerSecond() * Math.Sin(i / 104.0 * 4.0 * 2 * Math.PI);
			modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
			modData.CommitSimulationStep();
		}

		var windowSize = 26.SI<Meter>();
		var wIt = new ModDataWindowIterator(modData, windowSize);
		for (; !wIt.CycleEndReached; wIt.MoveNext()) {
			var sum = 0.0;
			for (; !wIt.WindowEndReached; wIt.NextEntry()) {
				sum += ((MeterPerSecond)modData.Data.Rows[wIt.Current][ModalResultField.v_act.GetShortCaption()]).AsKmph;
			}
			Console.WriteLine($"{wIt.Position}, {wIt.Current}: {wIt.Start} - {wIt.End}: {sum}");
			Console.WriteLine($"    {modData.Data.Rows[wIt.Position][ModalResultField.dist.GetShortCaption()]}, " +
							$"{modData.Data.Rows[wIt.Start][ModalResultField.dist.GetShortCaption()]} - " +
							$"{modData.Data.Rows[wIt.End][ModalResultField.dist.GetShortCaption()]}: {sum}");
			Assert.AreEqual(0, sum, 1e-6, $"at position: {wIt.Position}, {modData.Data.Rows[wIt.Position][ModalResultField.dist.GetShortCaption()]}");
		}
	}



	[TestCase(0.1, 0.9, 0.5, -0.2, 0.6)]
	[TestCase(0.1, 0.9, 0.5, 0.5, 1.2)]
	[TestCase(0, 1, 0.5, -0.5, 0.5)]


	[TestCase(0.1, 0.9, 0.5, -2, 0.5)]
	[TestCase(0.1, 0.9, 0.5, -0.6, 0.5)]
	[TestCase(0.1, 0.9, 0.5, 0.5, 1.5)]
    //[Test]
    public void ShiftInitSoc(
		double batMinSoc,
		double batMaxSoc,
		double initSoc,

		double minTraceSoc,
		 double maxTraceSoc)
	{

		Assume.That(batMinSoc < batMaxSoc);
		Assume.That((batMaxSoc - batMinSoc).IsGreaterOrEqual(maxTraceSoc - minTraceSoc));

		//initSoc = batMinSoc + ((batMaxSoc - batMinSoc) * initSoc);

		var d_minSoc = initSoc - minTraceSoc;
		var d_maxSoc = maxTraceSoc - maxTraceSoc;

		var preRunPostProcessor = new FuelCellPreRunPostprocessor(null);

		var new_initSoc = preRunPostProcessor.ShiftInitialSoc(batMinSoc, batMaxSoc, initSoc, minTraceSoc, maxTraceSoc);


		Assert.GreaterOrEqual(new_initSoc, batMinSoc);
		Assert.LessOrEqual(new_initSoc, batMaxSoc);

		Assert.GreaterOrEqual(new_initSoc - d_minSoc, batMinSoc);
		Assert.LessOrEqual(new_initSoc + d_maxSoc, batMaxSoc);

		TestContext.Progress.WriteLine($"New initial Soc {new_initSoc}");



	}

	//[Test]
	//public void InfinityTraceBatTest()
	//{


	//	////Powerdemand in Watt Assume fixed time steps of 60s
	//	//List<double> PowerDemand = new List<double>() {
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,
	//	//	100e3,

	//	//};

	//	//List<double> Soc = new List<double>();




	//	//var infinityBatData = batteryData.Clone();
	//	//infinityBatData.ChargeSustainingBatterySystem = true;

	//	//var infinityBattery = new BatterySystem(null, infinityBatData );

	//	//infinityBattery.Initialize(0.5);


	//	//var dt = 60.SI<Second>();
	//	//var absTime = 0.SI<Second>();

	//	//var infSoc_accumulated = 0.5;
	//	//var batSoc_accumulated = 0.5;
	//	//foreach (var powerDemand in PowerDemand.Select(pd => pd.SI<Watt>())) {
	//	//	TestContext.Progress.WriteLine(batterySystem.StateOfCharge);








	// // //         //Infinity battery
	// // //         var infResponse = infinityBattery.Request(absTime, dt, -powerDemand, false);
	//	//	//infinityBattery.CommitSimulationStep(absTime, dt, simpleContainer);
	//	//	//deltaSoc = DeltaSoc(infinityBattery, infResponse);


	//	//	//infSoc_accumulated += deltaSoc;

	//	//	//Assert.AreEqual(dSoc_response, deltaSoc.Value(), 1E-08, "Delta SOC");
	//	//	//Soc.Add(batterySystem.StateOfCharge); //real battery system




	//	//	////Assert.AreEqual(Soc.Last(), infSoc, 1E-4, "Accumulated SOC");








	//	//	absTime += dt;
	//	//}
	//}


	//TODO Reduce number of tests.
	[Test, Pairwise]
	public void CalculateSOCWithCurrent(
		[Range(-400E3, +400E3, 10E3)] double i_powerDemand, 
		[Range(0.1, 0.9, 0.1)]        double startSoc, 
		[Range(0.4, 0.6, 0.1)]        double d_dt, 
		[Values(1)]                          double d_absTime)
	{
		
		EngineeringDataAdapter dao = new EngineeringDataAdapter();
		var inputData = JSONInputDataFactory.ReadJsonJob(@"TestData/H2_FCV/PostProcessing/FCHV_singleFc.vecto") as IEngineeringInputDataProvider;
		var batteryData = dao.CreateBatteryData(inputData!.JobInputData.Vehicle.Components.ElectricStorage, 0.5);

		var batterySystem = new BatterySystem(null, batteryData);
		batterySystem.Initialize(startSoc);
		var simpleContainer = new SimpleModDataContainer();

		

        var dt = d_dt.SI<Second>();
		var absTime = d_absTime.SI<Second>();
		var powerDemand = i_powerDemand.SI<Watt>();
		TestContext.Progress.WriteLine($"Powerdemand: {powerDemand}\n SoC: {startSoc} \n dt: {dt} \n absTime: {absTime}");

        var socStart = batterySystem.StateOfCharge;
		var response = batterySystem.Request(absTime, dt, -powerDemand, false);
		Assume.That(response is RESSResponseSuccess, response.GetType().ToString());
		batterySystem.CommitSimulationStep(absTime, dt, simpleContainer);
		var dSoc_response = batterySystem.StateOfCharge - socStart;
		var deltaSoc = DeltaSoc(batterySystem, response);


		Assert.AreEqual(deltaSoc.Value(), dSoc_response, 1E-08, "Comparison of delta soc from response, and delta SOC calculated with current and power");
	}


    private static Scalar DeltaSoc(BatterySystem battery, IRESSResponse reponse)
	{
		var totalCapacity = battery.Capacity;


		var current = (reponse.PowerDemand - reponse.LossPower) / reponse.InternalVoltage;
		var energy = current * reponse.SimulationInterval;

		var energy_before = totalCapacity * battery.StateOfCharge;

		var remaining_energy = energy_before + energy;

		var soc_after = remaining_energy / battery.Capacity;
		var deltaSoc = soc_after - battery.StateOfCharge;
		return deltaSoc;
	}


	[TestCase(0, 10)]
	[TestCase(1, 10000)]
    public void FuelCellPostProcessing_DistanceWindow(int cycleIdx, int distance)
	{
		string jobFile = "TestData/H2_FCV/PostProcessing/FCHV_singleFc.vecto";

		var (modData, rundata) = RunFCHV_PEV_Simulation(jobFile, cycleIdx);

		var fcPostProcessor = new FuelCellPreRunPostprocessor(modData);
		var fcData = new FuelCellSystemData() {
			FuelCells = new List<FuelCellData>() {
				new FuelCellData() {
					MinElectricPower = 60.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					MaxElectricPower = 300.SI(Unit.SI.Kilo.Watt).Cast<Watt>()
				}
			}
		};

		fcPostProcessor.CalculateFuelCellPowerDemand(distance.SI<Meter>(), fcData, rundata.BatteryData);
	}

	[TestCase(0, 10)]
	[TestCase(1, 10000)]

	[TestCase(2, 10000)]

	[TestCase(3, 10000)]
    public void FuelCellPostProcessing_DistanceWindow_small_bat(int cycleIdx, int distance)
	{
		string jobFile = "TestData/H2_FCV/PostProcessing/FCHV_singleFc_smallBat.vecto";

		var (modData, rundata) = RunFCHV_PEV_Simulation(jobFile, cycleIdx);

		var fcPostProcessor = new FuelCellPreRunPostprocessor(modData);
		var fcData = new FuelCellSystemData()
		{
			FuelCells = new List<FuelCellData>() {
				new FuelCellData() {
					MinElectricPower = 60.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
					MaxElectricPower = 300.SI(Unit.SI.Kilo.Watt).Cast<Watt>()
				}
			}
		};

		fcPostProcessor.CalculateFuelCellPowerDemand(distance.SI<Meter>(), fcData, rundata.BatteryData);
	}



    private static (ModalDataContainer modData, VectoRunData RunData) RunFCHV_PEV_Simulation(string jobFile, int cycleIdx)
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
		//run.GetContainer().RunData.IterativeRunStrategy = null;
		run.GetContainer().RunData.BatteryData.Batteries.ForEach(x => x.Item2.ChargeSustainingBattery = true);
			
        Assert.NotNull(run);
		var modData = run.GetContainer().ModalData as ModalDataContainer;
		var modDataData = modData.Data;
		var pt = run.GetContainer();

		Assert.NotNull(pt);

		run.Run();
		Assert.IsTrue(run.FinishedWithoutErrors);

		modData.Data = modDataData;
		return (modData, run.GetContainer().RunData);
	}


}