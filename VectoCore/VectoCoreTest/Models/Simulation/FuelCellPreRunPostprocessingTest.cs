using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
using TUGraz.VectoCore.Tests.Utils.RunDataHelper;
using TUGraz.VectoCore.OutputData.ModDataPostprocessing.Impl.FuelCell;
using TUGraz.VectoCore.Tests.Utils;

namespace TUGraz.VectoCore.Tests.Models.Simulation;


[TestFixture]
public class FuelCellPreRunPostprocessingT
{

	[TestCase()]
	public void FuelCellPreRunPostprocessingTest()
	{

	}

	internal class PostProcessingEntry
	{
		public Meter dist;
		public Meter simulationDistance;
		public MeterPerSecond v_act;
	}

	[TestCase(true)]
	[TestCase(false)]
	public void TestFuelCellWindowIterator_1(bool compareWithGeneric)
	{
		// apply a constant value - the sum of all windows has to be the same
		var modData = new ModalDataContainer(new VectoRunData() {
			Cycle = new DrivingCycleData() {
				CycleType = CycleType.DistanceBased,
			}
		}, null, null);
		modData.Data.CreateColumns(ModalResults.DistanceCycleSignals);
		modData.Data.CreateColumns(ModalResults.DriverSignals);



		var entries = new List<PostProcessingEntry>();

        for (var i = 1; i <= 100; i++) {
			modData[ModalResultField.dist] = i.SI<Meter>();
			modData[ModalResultField.simulationDistance] = 1.SI<Meter>();
			modData[ModalResultField.v_act] = 30.KMPHtoMeterPerSecond();
			modData[ModalResultField.acc] = 0.SI<MeterPerSquareSecond>();
			modData.CommitSimulationStep();

			if (compareWithGeneric) {
				entries.Add(new PostProcessingEntry() {
					dist = i.SI<Meter>(),
					simulationDistance = 1.SI<Meter>(),
					v_act = 30.KMPHtoMeterPerSecond(),
				});
			}


		}

		var windowSize = 6.SI<Meter>();
		var wIt = new ModDataWindowIterator(modData, windowSize);
		for (; !wIt.CycleEndReached; wIt.MoveNext()) {
			var sum = 0.0;
			for (; !wIt.WindowEndReached; wIt.NextEntry()) {
				sum += ((MeterPerSecond)modData.Data.Rows[wIt.Current][ModalResultField.v_act.GetShortCaption()]).AsKmph;
			}
			TestContext.Progress.WriteLine($"{wIt.Position}, {wIt.Current}: {wIt.Start} - {wIt.End}: {sum}");
			TestContext.Progress.WriteLine($"    {modData.Data.Rows[wIt.Position][ModalResultField.dist.GetShortCaption()]}, " +
										$"{modData.Data.Rows[wIt.Start][ModalResultField.dist.GetShortCaption()]} - " +
										$"{modData.Data.Rows[wIt.End][ModalResultField.dist.GetShortCaption()]}: {sum}");
			Assert.AreEqual(180, sum, 1e-6, $"at position: {wIt.Position}, {modData.Data.Rows[wIt.Position][ModalResultField.dist.GetShortCaption()]}");
		}

		if (compareWithGeneric) {
			TestContext.Progress.WriteLine("Test generic iterator");
			var gIt =
				new GeneralizedModDataWindowIterator<PostProcessingEntry, Meter>(entries, windowSize, entry => entry.dist, entry => entry.simulationDistance)
					{ };
			for (; !gIt.EndReached; gIt.MoveNext())
			{
				var sum = 0.0;
				for (; !gIt.WindowEndReached; gIt.NextEntry())
				{
					sum += entries[gIt.CurrentIndex].v_act.AsKmph;
				}
				TestContext.Progress.WriteLine($"{gIt.Position}, {gIt.CurrentIndex}: {gIt.Start} - {gIt.End}: {sum}");
				TestContext.Progress.WriteLine($"    {entries[gIt.Position].dist}, " +
												$"{entries[gIt.Start].dist} - " +
												$"{entries[gIt.End].dist}: {sum}");
                Assert.AreEqual(180, sum, 1e-6, $"at position: {gIt.Position}, {entries[gIt.Position]}");
			}
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


	[TestCase(0.4, 0.6, 0.5, 0.3, 0.5)]
	[TestCase(0.1, 0.9, 0.5, -0.6, 0.5)]
	[TestCase(0.1, 0.9, 0.5, 0.5, 1.5)]

	[TestCase(0.1, 0.9, 0.9, 0.7, 1.5)]
    
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

		var d_minSoc = batMinSoc - minTraceSoc;
		var d_maxSoc = maxTraceSoc - batMaxSoc;

		var preRunPostProcessor = new FuelCellPreRunPostprocessor(null);

		var canBeShifted = preRunPostProcessor.TryShiftInitialSoC(batMinSoc, batMaxSoc, initSoc, minTraceSoc, maxTraceSoc, out var new_initSoc);




		TestContext.Progress.WriteLine($"Bat {batMinSoc} to {batMaxSoc} , init {initSoc}");
		TestContext.Progress.WriteLine($"BatTrace {minTraceSoc} to {maxTraceSoc}");
		TestContext.Progress.WriteLine($"$D_neg {initSoc - minTraceSoc}, D_pos {maxTraceSoc - initSoc}");


        TestContext.Progress.WriteLine($"New initial Soc {new_initSoc}");

		Assert.IsTrue(new_initSoc.IsGreaterOrEqual(batMinSoc, 1E-04));
		Assert.IsTrue(new_initSoc.IsSmallerOrEqual(batMaxSoc, 1E-04));



        Assert.IsTrue(Math.Abs(new_initSoc - initSoc).IsGreaterOrEqual(d_minSoc, 1E-04));
		Assert.IsTrue(Math.Abs(new_initSoc - initSoc).IsGreaterOrEqual(d_maxSoc, 1E-04));

    }


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
		var infBatterySystem = new TracingInfinityBatterySystem(batteryData.Clone());
		batterySystem.Initialize(startSoc);
		infBatterySystem.Initialize(startSoc);

		var simpleContainer = new SimpleModDataContainer();
		var infBatteryContainer = new SimpleModDataContainer();
		

        var dt = d_dt.SI<Second>();
		var absTime = d_absTime.SI<Second>();
		var powerDemand = i_powerDemand.SI<Watt>();
		TestContext.Progress.WriteLine($"Powerdemand: {powerDemand}\n SoC: {startSoc} \n dt: {dt} \n absTime: {absTime}");

        var socStart = batterySystem.StateOfCharge;


		var response = batterySystem.Request(absTime, dt, -powerDemand, false);
		var response_inf = infBatterySystem.Request(absTime, dt, -powerDemand, false);


		Assume.That(response is RESSResponseSuccess, response.GetType().ToString());
		Assert.That(response_inf is RESSResponseSuccess, response_inf.GetType().ToString());
		batterySystem.CommitSimulationStep(absTime, dt, simpleContainer);
		infBatterySystem.CommitSimulationStep(absTime, dt, infBatteryContainer);

		var dSoc_response = batterySystem.StateOfCharge - socStart;
		var deltaSoc = DeltaSoc(batterySystem, response);

		var deltaSoc_infBatterySystem = infBatterySystem.StateOfCharge - socStart;



		Assert.AreEqual(deltaSoc.Value(), dSoc_response, 1E-08, "Comparison of delta soc from response, and delta SOC calculated with current and power");
		Assert.AreEqual(deltaSoc, deltaSoc_infBatterySystem, 1E-08);
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




	[Test]
	public void GetFuelCellRaw([Range(1, 1000)]double windowSize_m)
	{




	}




	//[TestCase(0)]
	[TestCase(1, true)]
	[TestCase(1, false)]
    public void FuelCellPostProcessing_DistanceWindow__SoCRange(int cycleIdx, bool tryShift)
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

		rundata.BatteryData.SetUsableCapacity(10.SI(Unit.SI.Kilo.Watt.Hour).Cast<WattSecond>());
		var tmpSystem = new BatterySystem(null, rundata.BatteryData);
		var minSoc = tmpSystem.MinSoC;
		var maxSoc = tmpSystem.MaxSoC;

		for (Meter d = modData.Distance; d > 2.SI<Meter>(); d /= 2) {

			var success = fcPostProcessor.CalculateFuelCellPowerDemandForSoC(d, fcData, rundata.BatteryData, out var entries, rundata.BatteryData.InitialSoC, out var minTrace, out var maxTrace);

			var range = maxTrace - minTrace;
			TestContext.Progress.WriteLine($"s:{d}, soc_range:{range}, $[{minTrace}|{maxTrace}], {(success ? "success" : "")}");

            if (!success && tryShift) {
				if(fcPostProcessor.TryShiftInitialSoC(minSoc,maxSoc,rundata.BatteryData.InitialSoC, minTrace, maxTrace, out var init_soc))
				{
					TestContext.Progress.WriteLine($"\t Shifted soc {init_soc}");
					success = fcPostProcessor.CalculateFuelCellPowerDemandForSoC(d, fcData, rundata.BatteryData, out entries, init_soc, out minTrace, out maxTrace);
					range = maxTrace - minTrace;

					TestContext.Progress.WriteLine($"\t s:{d}, soc_range:{range}, $[{minTrace}|{maxTrace}], {(success ? "success" : "")}");
                }
			}

			if (success) {
				Assert.Pass();
				break;
			}
		}
		Assert.Fail();

	}


	[TestCase(0, 10)]
	[TestCase(1, 10000)]
	public void FuelCellPostProcessing_DistanceWindow(int cycleIdx, int distance)
	{
		string jobFile = "TestData/H2_FCV/PostProcessing/FCHV_singleFc.vecto";

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

		fcPostProcessor.CalculateFuelCellPowerDemandForWindowSize(distance.SI<Meter>(), fcData, rundata.BatteryData, out _);
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

		var entries = fcPostProcessor.CalculateFuelCellPowerDemand(fcData, rundata.BatteryData);
	}

	[TestCase(0, 100, 63, 0.05, 62.5)]
	[TestCase(0, 100, 63, 0.02, 62.5)]
	[TestCase(0, 100, 33.4, 0.05, 32.8125)]
	[TestCase(0, 100, 33.4, 0.02, 33.203125)]
	[TestCase(0, 100, 74.9, 0.05, 71.8750)]
	[TestCase(0, 100, 74.9, 0.02, 74.21875)]
	[TestCase(0, 100, 24.9, 0.05, 24.21875)]
	[TestCase(0, 100, 24.9, 0.02, 24.609375)]
	[TestCase(0, 100, 4.9, 0.05, 4.8828125)]
	[TestCase(0, 100, 4.9, 0.02, 4.882812)]

    public void TestFuelCellWindowSearchAlgorithm(double start, double end, double actual, double searchThreshold, double expectedWnd)
	{
		Assert.IsTrue(Math.Abs((actual - expectedWnd) / (actual + expectedWnd) * 2) < searchThreshold, "invalid parameters for testcase");
		var accepted = new List<Meter>();
		// we can add the full distance already beforehand because the search starts only if the full distance is not 
		// feasible.
		var rejected = new List<Meter>() { end.SI(Unit.SI.Kilo.Meter).Cast<Meter>() };
		int iterationCount = 0;
		SearchAlgorithm.BinarySearch(
			xStart: start.SI<Meter>(), 
			xEnd:end.SI(Unit.SI.Kilo.Meter).Cast<Meter>(),
			evaluateFunction: (d) => d > actual.SI(Unit.SI.Kilo.Meter).Cast<Meter>() ? null : 0.5,
			acceptFunction: (d, o) => {
				if (o == null) {
					rejected.Add(d);
				} else {
					accepted.Add(d);
				}

				return o != null;
			},
			abortCriterion: (d, o) => {
				if (!accepted.Any() || !rejected.Any()) {
					return false;
				}
				var lastAccepted = accepted.Last();
				var lastRejected = rejected.Last();
				var deviation = Math.Abs((lastRejected - lastAccepted) / (lastRejected + lastAccepted) * 2);
				return deviation < searchThreshold;
			},
			ref iterationCount,
			searcher: this
			);
			var solution = accepted.Last();

            TestContext.WriteLine($"iteration count: {iterationCount} deviation: {(actual - solution.ConvertToKiloMeter()) / (actual + solution.ConvertToKiloMeter()) * 2}");
            Assert.AreEqual(expectedWnd * 1000, solution.Value(), 1e-3);
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
		run.GetContainer().RunData.BatteryData.Batteries.ForEach(x => x.Item2.ChargeDepletingBattery = true);



			
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


	//[TestCase]
	//public void TestWindowSizes()
	//{
	//	//var modaldataMock = new Mock<IModalDataContainer>(behavior:MockBehavior.Strict);
	//	//modaldataMock.SetupGet(m => m.Data).Returns(() => null);
	//	//MockModalDataContainer
	//	FuelCellPreRunPostprocessor.FcCalcEntries(windowSize, minFcPower, maxFcPower, new BatterySystem()
			
			
			
			
	//		)


	//}






}