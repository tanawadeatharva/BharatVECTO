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
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
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

    [TestCase()]
	public void FuelCellPostProcessing_DistanceWindow()
	{
		string jobFile = "TestData/H2_FCV/PostProcessing/FCHV_singleFc.vecto";
		int cycleIdx = 0;

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

		fcPostProcessor.CalculateFuelCellPowerDemand(10000.SI<Meter>(), fcData, rundata.BatteryData);
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
		SearchAlgorithm.BinarySearch(start.SI<Meter>(), end.SI(Unit.SI.Kilo.Meter).Cast<Meter>(),
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