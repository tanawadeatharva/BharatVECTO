using System;
using System.IO;
using System.Linq;
using System.Threading;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.PrimaryBus;

public class PrimaryBusIgnoreResult
{
	private const string BASE_DIR = @"TestData/Integration/DeclarationMode/2nd_AmendmDeclMode/";

	private ThreadLocal<StandardKernel> _kernel;

	private StandardKernel Kernel => _kernel.Value;

	private IXMLInputDataReader _xmlReader;

    [OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new ThreadLocal<StandardKernel>(() => new StandardKernel(new VectoNinjectModule()));
		_xmlReader = Kernel.Get<IXMLInputDataReader>();
	}

    [SetUp]
	public void Setup()
	{
		Kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryVariant>().InSingletonScope();
		var cycleFactory = Kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryVariant;
		cycleFactory.Variant = "Short_10";

	}

	[
		TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", 0, TestName = "2nd Amendment PrimaryBus Conventional"),
		TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", 0, TestName = "2nd Amendment PrimaryBus PEV E2"),
	]
	public void PrimaryBusSimulationTest(string jobFile, int runIdx)
	{
		RunSimulationPrimary(jobFile, runIdx);
	}

	public void RunSimulationPrimary(string jobFile, int runIdx, params Action<VectoRunData>[] runDataModifier)
	{
		RunSimulationPrimary(jobFile, out var _);
	}

    public void RunSimulationPrimary(string jobFile, out string vifFile, params Action<VectoRunData>[] runDataModifier)
    {
		var filePath = Path.Combine(BASE_DIR, jobFile);
        var dataProvider = _xmlReader.CreateDeclaration(filePath);
        var fileWriter = new FileOutputWriter(filePath);
        var sumData = new SummaryDataContainer(fileWriter);
        var simFactory = Kernel.Get<ISimulatorFactoryFactory>();
        var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
        runsFactory.WriteModalResults = true;
        runsFactory.SerializeVectoRunData = true;
        runsFactory.SumData = sumData;
        var jobContainer = new JobContainer(sumData) { };
        //var jobContainer = new JobContainer(new MockSumWriter()) { };
        var runs = runsFactory.SimulationRuns().ToList();
        foreach (var vectoRun in runs) {
            foreach (var action in runDataModifier) {
                action(vectoRun.GetContainer().RunData);
            }
        }

		var abortedRunIdx = runs.FindIndex(x =>
			x.GetContainer().RunData.Mission.BusParameter.DoubleDecker &&
			x.GetContainer().RunData.Loading == LoadingType.ReferenceLoad &&
			x.GetContainer().RunData.Mission.MissionType == MissionType.Interurban);
		if (abortedRunIdx < 0) {
			throw new Exception("No such simulation run found!");
		}
		runs[abortedRunIdx] = new AbortingSimulationRun(runs[abortedRunIdx]);

		foreach (var run in runs) {
			jobContainer.AddRun(run);
		}

        jobContainer.Execute();
        jobContainer.WaitFinished();
        Assert.IsTrue(jobContainer.AllCompleted);
        Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
        vifFile = fileWriter.XMLMultistageReportFileName;
        PrintFiles(fileWriter);
    }

	public class AbortingSimulationRun : DistanceRun
	{
		private readonly IVectoRun _run;

		public AbortingSimulationRun(IVectoRun run) : base(run.GetContainer(), null, new IgnoreSimulationRun())
		{
			_run = run;
		}

        protected override IResponse DoSimulationStep()
        {
            IterationStatistics.StartIteration();

            // estimate distance to be traveled within the next TargetTimeInterval
            var ds = Container.VehicleInfo.VehicleSpeed.IsEqual(0.KMPHtoMeterPerSecond(), 0.01.SI<MeterPerSecond>())
                ? Constants.SimulationSettings.DriveOffDistance
                : VectoMath.Max(
                Constants.SimulationSettings.TargetTimeInterval * Container.VehicleInfo.VehicleSpeed,
                    Constants.SimulationSettings.DriveOffDistance);

            var loopCount = 0;
            IResponse response;
            var debug = new DebugData();
            do {
                IterationStatistics.Increment(this, "Iterations");

                Container.Brakes.BrakePower = 0.SI<Watt>();
                response = CyclePort.Request(AbsTime, ds);
                switch (response) {
                    case ResponseSuccess r:
                        dt = r.SimulationInterval;
                        break;
                    case ResponseDrivingCycleDistanceExceeded r:
                        if (r.MaxDistance.IsSmallerOrEqual(0)) {
                            throw new VectoSimulationException("DistanceExceeded, MaxDistance is invalid: {0}", r.MaxDistance);
                        }
                        ds = r.MaxDistance;
                        break;
                    case ResponseCycleFinished _:
                        FinishedWithoutErrors = true;
                        Log.Info("========= Driving Cycle Finished");
                        break;
                    case ResponseBatteryEmpty _:
                        FinishedWithoutErrors = true;
                        Log.Info("========= REESS empty");
                        break;
                    default:
                        throw new VectoException("DistanceRun got an unexpected response: {0}", response);
                }
                if (loopCount++ > Constants.SimulationSettings.MaximumIterationCountForSimulationStep) {
                    throw new VectoSimulationException("Maximum iteration count for a single simulation interval reached! Aborting!");
                }

				if (AbsTime > 100.SI<Second>()) {
					throw new VectoException("Intentionally aborting the simulation run...");
				}
                debug.Add($"[DR.DST-{loopCount}]", response);
            } while (!(response is ResponseSuccess || response is ResponseCycleFinished || response is ResponseBatteryEmpty));

            IterationStatistics.Increment(this, "Distance", Container.MileageCounter.Distance.Value());
            IterationStatistics.Increment(this, "Time", AbsTime.Value());
            IterationStatistics.FinishIteration(AbsTime);
            response.AbsTime = AbsTime;
            return response;
        }
    }

	public class IgnoreSimulationRun : IPostMortemAnalyzer
	{
		#region Implementation of IPostMortemAnalyzer

		public bool AbortSimulation(IVehicleContainer container, Exception ex)
		{
            // do not abort the simulation but ignore the result
			container.RunStatus = VectoRun.Status.PrimaryBusSimulationIgnore;
            return false;
		}

		#endregion
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
	}
}