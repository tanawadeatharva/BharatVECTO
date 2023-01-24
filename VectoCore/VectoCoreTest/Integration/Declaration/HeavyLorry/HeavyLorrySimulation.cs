

//#define singlethreaded

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text.RegularExpressions;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.HeavyLorry;



[TestFixture]
public class HeavyLorrySimulation
{


	private const string BASE_DIR = @"TestData\Integration\DeclarationMode\V24_DeclarationMode\";


	private const string HeavylorryGroup2HevS2XML = @"HeavyLorry\S-HEV\Group2_HEV_S2.xml";
	private const string HeavylorryGroup2HevS4XML = @"HeavyLorry\S-HEV\Group2_HEV_S4.xml";
	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();
	}

	[TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2_realistic.xml"),
	TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic_TorqueLimits.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E3_realistic_municipal.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2_pto_transm.xml"),
	TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml"),
	TestCase(HeavylorryGroup2HevS2XML),
	TestCase(HeavylorryGroup2HevS4XML),
	TestCase(@"HeavyLorry\Group5_HEV_P2_.xml"),
	TestCase(@"HeavyLorry\Group5_HEV_P3_ovc.xml")]
	[TestCase(@"HeavyLorry\HEV_heavy_lorry_S4_ovc.xml")]
	public void HeavyLorrySimulationTest(string jobFile)
	{
#if singlethreaded
		RunSimulation(jobFile, false);
#else
		RunSimulation(jobFile, true);
#endif
	}

	public void RunSimulation(string jobFile, bool multiThreaded = true)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var runsFactory = GetSimulatorFactory(filePath, out var dataProvider, out var fileWriter, out var summaryDataContainer);
		runsFactory.WriteModalResults = true;
		var jobContainer = new JobContainer(summaryDataContainer){};
		jobContainer.AddRuns(runsFactory);
		PrintRuns(jobContainer, null);
		
		jobContainer.Execute(multiThreaded);

		if (multiThreaded) {
			jobContainer.WaitFinished();
		}

		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	private ISimulatorFactory GetSimulatorFactory(string filePath, out IDeclarationInputDataProvider dataProvider,
		out FileOutputWriter fileWriter, out SummaryDataContainer sumWriter)
	{
		dataProvider = _xmlReader.CreateDeclaration(filePath);
		fileWriter = new FileOutputWriter(filePath);
	
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		sumWriter = new MockSumWriter();
		runsFactory.SumData = sumWriter;
		return runsFactory;
	}


	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S2_ovc.xml")]
	public void VMOD_VSUMUnitTest(string jobFile)
	{
		var columnsWithoutUnit = new List<ModalResultField> {
			ModalResultField.ICEOn,
			ModalResultField.EM_Off_,
			ModalResultField.HybridStrategyScore, 
			ModalResultField.HybridStrategySolution,
		}.Select(m => m.GetCaption());


		//	.ToHashSet();

		var filePath = Path.Combine(BASE_DIR, jobFile);
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, null, out var fileWriter, out var runs, out sumDataContainer);
	
		var run = runs.First();
		jobContainer.AddRun(run);
		//run.GetContainer().ModalData.Finish(runStatus:VectoRun.Status.Success, null);
		run.GetContainer().FinishSingleSimulationRun(null);
		var modFileName = fileWriter.GetModDataFileName(run.RunName, run.CycleName, run.RunSuffix); 

		var sumFileName = fileWriter.SumFileName;
		Assert.IsTrue(File.Exists(modFileName));
		var mod = VectoCSVFile.Read(modFileName, false, true);

		var columnsWithoutUnitHashSet = new HashSet<string>();
        foreach (var col in columnsWithoutUnit) {
			var emPositions = run.GetContainer().PowertrainInfo.ElectricMotorPositions;
			foreach (var emPosition in emPositions) {
				columnsWithoutUnitHashSet.Add(string.Format(col, emPosition.GetLabel()));
			}
		}


        List<string> columnHeaders = new List<string>();
		foreach (DataColumn modColumn in mod.Columns) {
			columnHeaders.Add(modColumn.Caption);
		}


		var unitColumnHeaders = columnHeaders.Where(h => !columnsWithoutUnitHashSet.Contains(h));
		foreach (string header in unitColumnHeaders) {
			TestContext.WriteLine(header);
		}
		foreach (string header in unitColumnHeaders) {
			var regex = new Regex(@"\[\S+\]");
			Assert.AreEqual(1, regex.Matches(header).Count(), $"missing unit {header}");
		}


		var sumCols = new List<string>();
		TestContext.WriteLine("\n -----------------------SumFile captions----------------------------- \n");
		foreach (DataColumn col in sumDataContainer.Table.Columns) {
			var caption = col.Caption;
			sumCols.Add(caption);
			TestContext.WriteLine(caption);
		}
		foreach (string sumHeader in sumCols)
		{
			var regex = new Regex(@"\[\S+\]");
			Assert.IsTrue(regex.Matches(sumHeader).Count() <= 1, $"double units {sumHeader}");
		}



	}

	//[Test]
	//public void HEVS4()
	//{
	//	var jobContainer = GetJobContainer(HeavylorryGroup2HevS4XML, 6, out var fileWriter, out var runs, out var sumDataContainer);
	//	//var simFactory = GetSimulatorFactory(Path.Combine(BASE_DIR, HeavylorryGroup2HevS4XML), out var dataProvider,
	//	//	out var fileWriter, out var mockSumWriter);

	//	foreach (var vectoRun in runs) {
	//		var rd = vectoRun.GetContainer().RunData;
	//	}
	//	var container = runs.First().GetContainer();
	//	var runData = container.RunData;

	//	runs = runs.Where(run => {
	//		var rd = run.GetContainer().RunData;
	//		return rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
	//	}).ToList();

	//	jobContainer.AddRun(runs.Single());

	//	Assert.AreEqual(1, jobContainer.Runs.Count);

	//	var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
	//	jobContainer.Execute(false);
	//	WaitAndAssertSuccess(jobContainer, fileWriter);


	//	//Pneumatic system data test
	//	var ps = runData.Aux.Where(x => x.ID == Constants.Auxiliaries.IDs.PneumaticSystem).Single();
	//	Assume.That(ps.IsFullyElectric);
	//	Assert.That(ps.ConnectToREESS);
	//}


	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S4_invalid_pto.xml")]
	public void PTOWithoutTransmissionTest(string jobFile)
	{
		SummaryDataContainer sumDataContainer;
		var exception = Assert.Throws<VectoException>(() => GetJobContainer(jobFile, null, out var fileWriter, out var runs, out sumDataContainer));
		TestContext.WriteLine(exception.Message);
	}


	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S2_ovc.xml", 12)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S3_ovc.xml", 12)]
	//[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S4_ovc.xml",12)]
	public void SHEV_ChargeDepleting(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		
		Assert.AreEqual(runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting), 
			runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining));
		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));


		runs = runs.Where(run => { 
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());

		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);

		
		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName());
		foreach (DataRow modDataRow in modData.Rows) {
			Assert.AreEqual(soc, modDataRow.Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()));
			Assert.IsFalse(modDataRow.Field<bool>(ModalResultField.ICEOn.GetName()));
			AssertSHEV_PEV_Conditioning(modDataRow, runs.Single());
		}
		Assert.IsTrue(modData.Rows.Count > 0);
	}

	//runs.First().GetContainer().PowertrainInfo.ElectricMotorPositions;
	public void AssertSHEV_PEV_Conditioning(DataRow modDataRow, IVectoRun run)
	{
		var electricMotorPositions = run.GetContainer().PowertrainInfo.ElectricMotorPositions;
		var position = electricMotorPositions.Single(e => e != PowertrainPosition.GEN);

		var idx = modDataRow.Table.Rows.IndexOf(modDataRow);
		if (idx - 1 < 0) {
			return;
		}

		var prevRow = modDataRow.Table.Rows[idx - 1];

		var condData = run.GetContainer().RunData.Aux.Single(aux => aux.ID == Constants.Auxiliaries.IDs.Cond);
		Assert.IsTrue(condData.IsFullyElectric);
		Assert.IsTrue(condData.ConnectToREESS);
		var time = modDataRow.Field<Second>(ModalResultField.time.GetName());
		//modDataRow.Table.Rows[]
		if (EMOn(prevRow, position) || EMOn(prevRow, PowertrainPosition.GEN)) {
			var cond = DeclarationData.Conditioning.LookupPowerDemand(run.GetContainer().RunData.VehicleData.VehicleClass,
				run.GetContainer().RunData.Mission.MissionType);
			var condMod = modDataRow.Field<Watt>("P_aux_COND_el [kW]");
			Assert.IsTrue(cond.IsEqual(condMod), $"expected {cond} got {condMod} at {time}");
		} else {
			var condMod = modDataRow.Field<Watt>("P_aux_COND_el [kW]");
			Assert.IsTrue(0.SI<Watt>().IsEqual(condMod), $"expected {0} got {condMod} at {time}");
		}
	}

	private static bool EMOn(DataRow prevRow, PowertrainPosition position)
	{
		return prevRow.Field<Scalar>(string.Format(ModalResultField.EM_Off_.GetCaption(), position.GetLabel())) == 0.SI<Scalar>();
	}

	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S2_ovc.xml", 12)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S3_ovc.xml", 12)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S4.xml", 6)]
	public void SHEV_ChargeSustaining(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		if (runs.First().GetContainer().RunData.VehicleData.OffVehicleCharging) {
			Assert.AreEqual(runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting),
				runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining));
		}
		
		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());

		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);

		
		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName());
		foreach (DataRow modDataRow in modData.Rows)
		{
			AssertSHEV_PEV_Conditioning(modDataRow, runs.Single());
		}
		Assert.IsTrue(modData.Rows.Count > 0);
	}

	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S2_pto_transmission.xml", 6)]
	public void SHEV_PTO_Transmission(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));
		Assert.AreEqual(6, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining));
		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.LongHaul && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());

		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modDataContainer = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData);
		var modData = modDataContainer.Data;
		var aux = modDataContainer.Auxiliaries.ToDictionary(entry => entry.Key, entry => entry.Value);


		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);


		modDataContainer.Data = modData;
		modDataContainer.Auxiliaries = aux;
		//modDataContainer.GetValues<double>(ModalResultField

		

		var ptoTransm = modData.Rows[1][modDataContainer.Auxiliaries[Constants.Auxiliaries.IDs.PTOTransmission]];
		var ptoTransmCaption = modDataContainer.Auxiliaries[Constants.Auxiliaries.IDs.PTOTransmission];
		Assert.IsNotNull(ptoTransm);
		//modData.
		foreach (DataRow row in modData.Rows)
		{
			if (row.Field<uint>(ModalResultField.Gear.GetShortCaption()) != 0) {
				Assert.IsTrue(row.Field<Watt>(ptoTransmCaption).Value().IsGreater(0));
			}
		}
		Assert.IsTrue(modData.Rows.Count > 0);
	}

	private void WaitAndAssertSuccess(JobContainer jobContainer, FileOutputWriter fileWriter)
	{
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	private JobContainer GetJobContainer(string jobFile, int? nrRuns, out FileOutputWriter fileWriter,
		out List<IVectoRun> runs, out SummaryDataContainer sumDataContainer)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
		runsFactory.WriteModalResults = true;
		var sumWriter = new MockSumWriter();
		
		var jobContainer = new JobContainer(sumWriter);
		runsFactory.SumData = sumWriter;
		sumDataContainer = sumWriter;
		runs = runsFactory.SimulationRuns().ToList();
		if (nrRuns.HasValue) {
			Assert.AreEqual(nrRuns, runs.Count);
		}
		return jobContainer;
	}


	private void PrintRuns(JobContainer jobContainer, FileOutputWriter fileWriter = null)
	{
		foreach (var keyValuePair in jobContainer.GetProgress()) {
			TestContext.WriteLine($"{keyValuePair.Key}: {keyValuePair.Value.CycleName} {keyValuePair.Value.RunName} {keyValuePair.Value.Error?.Message}" );
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
