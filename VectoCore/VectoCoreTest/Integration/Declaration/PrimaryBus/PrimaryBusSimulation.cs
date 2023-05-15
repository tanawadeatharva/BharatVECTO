using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Xml;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Newtonsoft.Json;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.Tests.Integration.CompletedBus;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using Formatting = Newtonsoft.Json.Formatting;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.PrimaryBus;

public class PrimaryBusSimulation
{

	private const string BASE_DIR = @"TestData\Integration\DeclarationMode\2nd_AmendmDeclMode\";

	private const string BASE_DIR_COMPLETED = @"TestData\Integration\DeclarationMode\2nd_AmendmDeclMode\CompletedBus";
	private const string BASE_DIR_VIF = @"TestData\Integration\DeclarationMode\2nd_AmendmDeclMode\CompletedBus\VIF";
	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();

		_kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryVariant>().InSingletonScope();
		var cycleFactory = _kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryVariant;
		cycleFactory.Variant = "Short_10";
	}


	[
	TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", 0, TestName = "2nd Amendment PrimaryBus Conventional"),
	TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", 0, TestName = "2nd Amendment PrimaryBus PEV E2"),

	TestCase(@"PrimaryBus/PEV/PrimaryCoach_E2_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus Coach PEV E2 Base"),
	TestCase(@"PrimaryBus/PEV/PrimaryCityBus_IEPC_Base.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus PEV IEPC Base"),

	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_BD_BCVC_stefan.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 stpr"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_NoAlt.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 no_alt stpr"),
	//TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_Notappl.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 notAppl stpr"), // invalid input for PS
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_CM_BCVC.xml", 7, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 AMT_CM_BCVC stpr, InterUrban_Ref_Load"),

	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_Conv.xml", 0, TestName="2nd Amendment PrimaryBus Coach P-HEV P2 AMT"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 Base AMT"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_OVC.xml", 0, TestName = "2nd Amendment PrimaryBus Coach P-HEV P2 AMT OVC"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus P-HEV P1 Base AT"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml", 7, TestName = "2nd Amendment PrimaryBus CityBus P-HEV P1 Base AT - Interurban"),
    TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_AT_BD_BCVC.xml", 0, TestName= "2nd Amendment PrimaryBus CityBus P-HEV P1 Base AT - no TC"),
	TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_IHPC.xml", -1, TestName = "2nd Amendment PrimaryBus CityBus P-HEV IHPC Base"),
    TestCase(@"PrimaryBus/S-HEV/PrimaryCoach_S2_Base_AMT.xml", 0, TestName = "2nd Amendment PrimaryBus Coach S-HEV S2 Base"),
	TestCase(@"PrimaryBus/S-HEV/PrimaryCityBus_IEPC-S_Base.xml", 0, TestName = "2nd Amendment PrimaryBus CityBus S-HEV IEPC Base"),

    TestCase(@"PrimaryBus/exempted/exempted_primary_heavyBus.xml", 0, TestName = "2nd Amendment PrimaryBus Exempted"),
	]
	public void PrimaryBusSimulationTest(string jobFile, int runIdx)
	{
		RunSimulationPrimary(jobFile, runIdx);
	}
	
	[
	TestCase(@"primary_heavyBus group41_nonSmart.RSLT_VIF.xml", @"Conventional_completedBus_2.xml", 1, TestName = "2nd Amendment CompletedBus Conventional"),
	TestCase(@"PEV_primaryBus_AMT_E2.RSLT_VIF.xml", @"PEV_completedBus_2.xml", -1,                      TestName = "2nd Amendment CompletedBus PEV E2"),
	TestCase(@"PrimaryCoach_E2_Base_AMT.RSLT_VIF.xml", @"PEV_completedBus_2.xml", 1,                   TestName = "2nd Amendment CompletedBus Coach PEV E2"),
	TestCase(@"PrimaryCityBus_IEPC_Base.RSLT_VIF.xml", @"PEV_completedBus_2.xml", 1,                   TestName = "2nd Amendment CompletedBus CityBus PEV IEPC"),
	TestCase(@"PrimaryCoach_P2_HEV_Base_AMT.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 1,               TestName = "2nd Amendment CompletedBus Coach HEV P2"),
	TestCase(@"PrimaryCoach_P2_HEV_AMT_OVC.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 1,                TestName = "2nd Amendment CompletedBus Coach HEV P2 OVC"),
	TestCase(@"PrimaryCityBus_P1_HEV_Base_AT.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 1, true,        TestName = "2nd Amendment CompletedBus CityBus HEV P1 - fails on complete cycle"),
	TestCase(@"PrimaryCoach_S2_Base_AMT.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 1,                   TestName = "2nd Amendment CompletedBus Coach HEV S2"),
	TestCase(@"PrimaryCityBus_IEPC-S_Base.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 1,                 TestName = "2nd Amendment CompletedBus CityBus HEV IEPC-S"),
	TestCase(@"exempted_primary_heavyBus.RSLT_VIF.xml", @"exempted_completedBus_input_full.xml", 1,    TestName = "2nd Amendment CompletedBus Exempted"),
	TestCase(@"IHPC", @"IHPC_completed", 1, TestName="2nd Amendment Completed Bus IHPC")
	]
    public void CompletedBusSimulationTest(string vifFile, string completed, int runIdx, bool full_sim = false)
	{
		if (full_sim) {
			_kernel.Rebind<IDeclarationCycleFactory>().To<DeclarationCycleFactory>().InSingletonScope();
        }
		var completedJob = GenerateJsonJobCompletedBus(Path.Combine(BASE_DIR_VIF, vifFile), Path.Combine(BASE_DIR_COMPLETED, completed));

		_kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
		var missionFilter = _kernel.Get<IMissionFilter>() as TestMissionFilter;
		missionFilter!.SetMissions((MissionType.Coach, LoadingType.ReferenceLoad));

        var finalVif = CreateCompletedVIF(completedJob);

		//RunSimulationPrimary(finalVif, runIdx);
	}

	[TestCase(@"PrimaryCityBus_P1_HEV_Base_AT.RSLT_VIF.xml", @"HEV_completedBus_2.xml", 53804,
		TestName = "2nd Amendment CompletedBus CityBus HEV P1 - failing cycle section")]
	public void CompletedBusCycleSection(string vifFile, string completed, double start,
		double? distance = null)
	{
		_kernel.Rebind<IDeclarationCycleFactory>().To<TestDeclarationCycleFactoryStartPoint>().InSingletonScope();
		var cycleFactory = _kernel.Get<IDeclarationCycleFactory>() as TestDeclarationCycleFactoryStartPoint;

		_kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
		var missionFilter = _kernel.Get<IMissionFilter>() as TestMissionFilter;
		missionFilter!.SetMissions((MissionType.Interurban, LoadingType.ReferenceLoad));
		cycleFactory!.SetStartPoint(MissionType.Interurban, start.SI<Meter>(), true, distance?.SI<Meter>());
		var completedJob = GenerateJsonJobCompletedBus(Path.Combine(BASE_DIR_VIF, vifFile),
			Path.Combine(BASE_DIR_COMPLETED, completed));

		var finalVif = CreateCompletedVIF(completedJob);
	}


	[
		TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", @"Conventional_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Conventional"),
		
		TestCase(@"PrimaryBus/PEV/PEV_primaryBus_AMT_E2.xml", @"PEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus PEV E2"),
		TestCase(@"PrimaryBus/PEV/PrimaryCoach_E2_Base_AMT.xml", @"PEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach PEV E2 Base"),
		TestCase(@"PrimaryBus/PEV/PrimaryCityBus_IEPC_Base.xml", @"PEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus PEV IEPC Base"),

		TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_Base_AMT.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach P-HEV P2 Base AMT"),
		TestCase(@"PrimaryBus/P-HEV/PrimaryCoach_P2_HEV_AMT_OVC.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach P-HEV P2 AMT OVC"),
		TestCase(@"PrimaryBus/P-HEV/PrimaryCityBus_P1_HEV_Base_AT.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus P-HEV P1 Base AT"),

		TestCase(@"PrimaryBus/S-HEV/PrimaryCoach_S2_Base_AMT.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus Coach S-HEV S2 Base"),
		TestCase(@"PrimaryBus/S-HEV/PrimaryCityBus_IEPC-S_Base.xml", @"HEV_completedBus_2.xml", 0, TestName = "2nd Amendment SingleBus CityBus S-HEV IEPC Base"),

		//TestCase(@"PrimaryBus/exempted/exempted_primary_heavyBus.xml", @"exempted_completedBus_input_full.xml", 0, TestName = "2nd Amendment SingleBus Exempted"), // exempted single run not supported!

	]
	public void SingleBusSimulationTest(string jobFile, string completed, int runIdx)
	{
		RunSimulationSingle(jobFile, completed, runIdx);
	}

	private const LoadingType RefL = LoadingType.ReferenceLoad;
	private const MissionType CycleCO = MissionType.Coach;

	[TestCase(@"PrimaryBus/Conventional/primary_heavyBus group41_nonSmart.xml", @"Conventional_completedBus_2.xml", @"primary_heavyBus group41_nonSmart.RSLT_VIF.xml", CycleCO, RefL, 
			TestName = "2nd Amendment FactorMethodRunData Conventional CO RL"),
	]
	public void TestFactorMethodRunData(string primary, string completed, string vifFile, MissionType mission, LoadingType loading)
	{
		_kernel.Rebind<IMissionFilter>().To<TestMissionFilter>().InSingletonScope();
		var missionFilter = _kernel.Get<IMissionFilter>() as TestMissionFilter;
		missionFilter!.SetMissions((mission, loading));

		var outputPath = Path.Combine("FactorMethod_xEV", TestContext.CurrentContext.Test.Name.Split(' ').Skip(3).Join("_"));
		if (!Directory.Exists(outputPath)) {
			Directory.CreateDirectory(outputPath);
		}

		var singleJob = GenerateJsonJobSingleBus(Path.Combine(BASE_DIR, primary), Path.Combine(BASE_DIR_COMPLETED, completed));
		var dataProviderSingle = JSONInputDataFactory.ReadJsonJob(singleJob);
		var fileWriterSingle = new FileOutputWriter(singleJob);
		var simFactorySingle = _kernel.Get<ISimulatorFactoryFactory>();
		var runsFactorySingle = simFactorySingle.Factory(ExecutionMode.Declaration, dataProviderSingle, fileWriterSingle, null, null);

		SerializeRunData(runsFactorySingle, outputPath);


		var filePathPrim = Path.Combine(BASE_DIR, primary);
		var dataProviderPrim = _xmlReader.CreateDeclaration(filePathPrim);
		var fileWriterPrim = new FileOutputWriter(filePathPrim);
		var simFactoryPrim = _kernel.Get<ISimulatorFactoryFactory>();
		var runsFactoryPrim = simFactoryPrim.Factory(ExecutionMode.Declaration, dataProviderPrim, fileWriterPrim, null, null);

		SerializeRunData(runsFactoryPrim, outputPath);


		var completedJob = GenerateJsonJobCompletedBus(Path.Combine(BASE_DIR_VIF, vifFile), Path.Combine(BASE_DIR_COMPLETED, completed), false);

		var dataProviderComleted = JSONInputDataFactory.ReadJsonJob(completedJob);
		var fileWriterCompleted = new FileOutputWriter(singleJob);
		var simFactoryCompleted = _kernel.Get<ISimulatorFactoryFactory>();
		var runsFactoryCompleted = simFactoryCompleted.Factory(ExecutionMode.Declaration, dataProviderComleted, fileWriterCompleted, null, null);
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriterCompleted)) { };
		jobContainer.AddRuns(runsFactoryCompleted);
		jobContainer.Execute();
		jobContainer.WaitFinished();

		var vif = fileWriterCompleted.GetWrittenFiles().FirstOrDefault(x => x.Key == ReportType.DeclarationReportMultistageVehicleXML);
		Assert.NotNull(vif);

		var dataProviderVif = _xmlReader.CreateDeclaration(vif.Value) as IMultistepBusInputDataProvider;
		var dataProviderFinal = new XMLDeclarationVIFInputData(dataProviderVif, null, true);
        var fileWriterFinal = new FileOutputWriter(filePathPrim);
		var simFactoryFinal = _kernel.Get<ISimulatorFactoryFactory>();
		var runsFactoryFinal = simFactoryFinal.Factory(ExecutionMode.Declaration, dataProviderFinal, fileWriterFinal, null, null);

		SerializeRunData(runsFactoryFinal, outputPath);
    }

    private static void SerializeRunData(ISimulatorFactory runsFactorySingle, string outputPath)
	{
		var jsonSerializerSettings = new JsonSerializerSettings();
		jsonSerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
		foreach (var run in runsFactorySingle.SimulationRuns()) {
			var data = run.GetContainer().RunData;
			File.WriteAllText(
				Path.Combine(
					outputPath,
					$"{data.JobName}_{data.Cycle.Name}{data.ModFileSuffix}.json"),
				JsonConvert.SerializeObject(data, Formatting.Indented, jsonSerializerSettings));
		}
	}

	private void RunSimulationSingle(string jobFile, string completed, int runIdx)
	{
		var singleJob = GenerateJsonJobSingleBus(Path.Combine(BASE_DIR, jobFile), Path.Combine(BASE_DIR_COMPLETED, completed));
		var dataProvider = JSONInputDataFactory.ReadJsonJob(singleJob);
		var fileWriter = new FileOutputWriter(singleJob);
		var simFactory = _kernel.Get<ISimulatorFactoryFactory>();

		var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		//runsFactory.WriteModalResults = true;
		runsFactory.SerializeVectoRunData = true;
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriter)) { };

		if (runIdx < 0) {
			jobContainer.AddRuns(runsFactory);
		} else {
			var run = runsFactory.SimulationRuns().Skip(runIdx).First();
			jobContainer.AddRun(run);
			//if (dataProvider.JobInputData.Vehicle.OvcHev) {
			//	var run2 = runsFactory.SimulationRuns().Skip(runIdx + 1).First();
			//	jobContainer.AddRun(run2);
			//}
		}

		PrintRuns(jobContainer, null);

		jobContainer.Execute();
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));

		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	private string GenerateJsonJobSingleBus(string primary, string completed)
	{
		var subDirectory = Path.GetDirectoryName(primary);

		var header = new Dictionary<string, object>() {
			{ "FileVersion", 6 }
		};
		var body = new Dictionary<string, object>() {
			{ "PrimaryVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(primary)) },
			{ "CompletedVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(completed)) },
			//{ "RunSimulation", true}
		};
		var json = new Dictionary<string, object>() {
			{"Header", header},
			{"Body", body}
		};

		Directory.CreateDirectory(Path.GetFullPath(subDirectory));
		var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), "Single_" + Path.GetFileNameWithoutExtension(primary) + ".vecto");
		var str = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
		File.WriteAllText(path, str);
		return path;
	}

	public string CreateCompletedVIF(string jobFile)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = JSONInputDataFactory.ReadJsonJob(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var simFactory = _kernel.Get<ISimulatorFactoryFactory>();
		
		var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		
		//runsFactory.WriteModalResults = true;
		runsFactory.SerializeVectoRunData = true;
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriter)) { };
        //var jobContainer = new JobContainer(new MockSumWriter()) { };

		jobContainer.AddRuns(runsFactory);
		//PrintRuns(jobContainer, null);

		jobContainer.Execute(multithreaded:false);
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));

		//PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
		return fileWriter.GetWrittenFiles()[ReportType.DeclarationReportMultistageVehicleXML];
	}

	public void RunSimulationPrimary(string jobFile, int runIdx)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		var fileWriter = new FileOutputWriter(filePath);
		var simFactory = _kernel.Get<ISimulatorFactoryFactory>();
		var runsFactory = simFactory.Factory(ExecutionMode.Declaration, dataProvider, fileWriter, null, null);
		runsFactory.WriteModalResults = true;
		runsFactory.SerializeVectoRunData = true;
		var jobContainer = new JobContainer(new SummaryDataContainer(fileWriter)) { };
		//var jobContainer = new JobContainer(new MockSumWriter()) { };

		if (runIdx < 0) {
			jobContainer.AddRuns(runsFactory);
		} else {
			var run = runsFactory.SimulationRuns().Skip(runIdx).First();
			jobContainer.AddRun(run);
			TestContext.Progress.WriteLine($"{run.CycleName} - {run.RunSuffix}");
			var expectedResults = 1;
			if (dataProvider.JobInputData.Vehicle.OvcHev && !dataProvider.JobInputData.Vehicle.VehicleType.IsOneOf(
					VectoSimulationJobType.BatteryElectricVehicle, 
					VectoSimulationJobType.IEPC_E)) {
				var run2 = runsFactory.SimulationRuns().Skip(runIdx + 1).First();

				jobContainer.AddRun(run2);
				TestContext.Progress.WriteLine($"{run2.CycleName} - {run2.RunSuffix}");

				CompareStrings(run.CycleName + run.RunSuffix, run2.CycleName + run2.RunSuffix, 2);
				expectedResults++;
			}
			
			SetResultCountInReport(expectedResults, run.GetContainer().RunData.Report);
		}

		PrintRuns(jobContainer, null);

		jobContainer.Execute();
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));

		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	private void CompareStrings(string expected, string actual, int ignoreEnd)
	{
		var len = expected.Length - ignoreEnd;

		var expectedWithoutSuffix = expected.Substring(0, len);
		var actualWithoutSuffix = actual.Substring(0, len);
		Assert.AreEqual(expectedWithoutSuffix, actualWithoutSuffix);
	}

	private void SetResultCountInReport(int count, IDeclarationReport report)
	{
		if (report == null)
		{
			return; //also used in engineering mode
		}
		if (report is XMLDeclarationReport rep09)
		{
			GetField("_resultCount", rep09.GetType()).SetValue(rep09, count);
			return;
		}
		Assert.Fail("Reflection failed");

	}

	private FieldInfo GetField(string name, Type type)
	{
		bool found = false;
		while (!found)
		{
			FieldInfo[] fields = type.GetFields(
				BindingFlags.NonPublic |
				BindingFlags.Instance);
			var field = fields.FirstOrDefault(f => f.Name == name);
			if (field == null)
			{
				type = type.BaseType;
				if (type == null)
				{
					Assert.Fail("Field not found");
				}
			}
			else
			{
				return field;
			}
		}

		return null;

	}


	private string GenerateJsonJobCompletedBus(string vif, string completeBusInput, bool? runSimulation = null)
	{
		var subDirectory = Path.GetDirectoryName(completeBusInput);

		var header = new Dictionary<string, object>() {
			{ "FileVersion", 7 }
		};
		var body = new Dictionary<string, object>() {
			{ "PrimaryVehicleResults", Path.GetRelativePath(subDirectory, Path.GetFullPath(vif)) },
			{ "CompletedVehicle", Path.GetRelativePath(subDirectory, Path.GetFullPath(completeBusInput)) },
			{ "RunSimulation", runSimulation ?? true }
		};
		var json = new Dictionary<string, object>() {
			{"Header", header},
			{"Body", body}
		};

		Directory.CreateDirectory(Path.GetFullPath(subDirectory));
		var path = Path.Combine(Path.Combine(Path.GetFullPath(subDirectory)), Path.GetFileNameWithoutExtension(vif) + ".vecto");
		var str = JsonConvert.SerializeObject(json, Newtonsoft.Json.Formatting.Indented);
		File.WriteAllText(path, str);
		return path;
	}

	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}");
			//if (fileWriter != null && keyValuePair.Value.Success) {
			//	TestContext.AddTestAttachment(fileWriter.GetModDataFileName(keyValuePair.Value.RunName, keyValuePair.Value.CycleName, keyValuePair.Value.RunSuffix), keyValuePair.Value.RunName);
			//         }

		}
	}

	private void PrintFiles(FileOutputWriter fileWriter)
	{
		//if (fileWriter.GetWrittenFiles().Count == 0) {
		//	Assert.Fail("No files written\n");
		//}
		foreach (var keyValuePair in fileWriter.GetWrittenFiles()) {
			TestContext.WriteLine($"{keyValuePair.Key} written to {keyValuePair.Value}");
			TestContext.AddTestAttachment(keyValuePair.Value, keyValuePair.Key.ToString());
		}
	}
}