

//#define singlethreaded

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.Schema;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
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
using TUGraz.VectoCore.Tests.TestUtils;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration.HeavyLorry;



[TestFixture]
//[Parallelizable(ParallelScope.Children)]
public class HeavyLorrySimulation
{
	private enum PTOState
	{
		VehicleDriving, // speed >= 0
		VehicleStopped, // speed == 0, pto might be activated
		PTOActive, // Pto is active 
	}

	private const string BASE_DIR = @"TestData\Integration\DeclarationMode\V24_DeclarationMode\";
	private const string Group5_HEV_P2_OVC = @"HeavyLorry\P-HEV\Group5_HEV_P2_ovc.xml";
	private const string Group5_HEV_P3_OVC = @"HeavyLorry\P-HEV\Group5_HEV_P3_ovc.xml";
	private const string Group5_HEV_P4_OVC = @"HeavyLorry\P-HEV\Group5_HEV_P4_ovc.xml";
	private const string Group5_HEV_P2_5_OVC = @"HeavyLorry\P-HEV\Group5_HEV_P2_5_ovc.xml";
	private const string Group5_HEV_S2_OVC = @"HeavyLorry\S-HEV\Group2_HEV_S2_ovc.xml";
	private const string Group5_HEV_S3_OVC = @"HeavyLorry\S-HEV\Group2_HEV_S3_ovc.xml";
	private const string Group5_HEV_S4_OVC = @"HeavyLorry\S-HEV\Group2_HEV_S4_ovc.xml";
	private const string Group5_HEV_S_IEPC = @"HeavyLorry\S-HEV\Group2_HEV_IEPC_S.xml";
	private const string Group5_HEV_S_IEPC_pto = @"HeavyLorry\S-HEV\Group2_HEV_IEPC_S_pto.xml";
	private const string Group5_HEV_S_IEPC_ovc = @"HeavyLorry\S-HEV\Group2_HEV_IEPC_S_ovc.xml";

	private const string Group5_PEV_E3 = @"HeavyLorry\PEV\Group5_ PEV_E3_ES_Standard.xml";
	private const string Group2_HEV_IEPC_S_StdVal = @"HeavyLorry\S-HEV\Group2_HEV_IEPC_S_standard_values.xml";


	private StandardKernel _kernel;
	private IXMLInputDataReader _xmlReader;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_xmlReader = _kernel.Get<IXMLInputDataReader>();
		
	}

	//Conventional


	//S-HEV
	[TestCase(Group5_HEV_S2_OVC)]

	//PEV
	[TestCase(Group5_PEV_E3)]
	///Runs a f
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
        var jobContainer = new JobContainer(summaryDataContainer) { };
        jobContainer.AddRuns(runsFactory);
        PrintRuns(jobContainer, null);

        jobContainer.Execute(multiThreaded);

        if (multiThreaded)
        {
            jobContainer.WaitFinished();
        }

        Assert.IsTrue(jobContainer.AllCompleted);
        Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
        PrintRuns(jobContainer, fileWriter);
        PrintFiles(fileWriter);

		var mrfPath = fileWriter.GetWrittenFiles()[ReportType.DeclarationReportManufacturerXML];
		var cifPath = fileWriter.GetWrittenFiles()[ReportType.DeclarationReportCustomerXML];
		var cifSchema = XMLValidator.GetXMLSchema(XmlDocumentType.CustomerReport);
		var mrfSchema = XMLValidator.GetXMLSchema(XmlDocumentType.ManufacturerReport);
		XDocument.Load(mrfPath).Validate(mrfSchema, (sender, args) => Assert.Fail(args.Message));
		XDocument.Load(cifPath).Validate(cifSchema, (sender, args) => Assert.Fail(args.Message));
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


    [TestCase(Group5_HEV_S2_OVC)]
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
		var jobContainer = GetJobContainer(jobFile, null, out var fileWriter, out var runs, out sumDataContainer, false);
	
		var run = runs.First();
		jobContainer.AddRun(run);

        run.GetContainer().ModalData[ModalResultField.time] = 1.SI<Second>(); //fake duration for run
        run.GetContainer().ModalData[ModalResultField.Gear] = 2;
        run.GetContainer().ModalData.CommitSimulationStep();
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
	[TestCase(@"ADD E3_PEV_HERE")]
	public void PTOWithoutTransmissionTest(string jobFile)
	{
		SummaryDataContainer sumDataContainer;
		var exception = Assert.Throws<VectoException>(() => GetJobContainer(jobFile, null, out var fileWriter, out var runs, out sumDataContainer));
		TestContext.WriteLine(exception.Message);
	}

	[TestCase(Group5_HEV_P2_OVC, 20)]
	[TestCase(Group5_HEV_P3_OVC, 20)]
	[TestCase(Group5_HEV_P4_OVC, 20)]
	[TestCase(Group5_HEV_P2_5_OVC, 20)]
	public void PHEV_ChargeSustainingIt(string jobFile, int nrRuns)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer);

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());
		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);
	}
	[TestCase(Group5_HEV_P2_OVC, 20)]
	[TestCase(Group5_HEV_P3_OVC, 20)]
	[TestCase(Group5_HEV_P4_OVC, 20)]
	[TestCase(Group5_HEV_P2_5_OVC, 20)]
	[TestCase(@"HeavyLorry\P-HEV\Group5_HEV_IHPC.xml", 20)]

	public void PHEV_ChargeDepleting(string jobFile, int nrRuns)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer);
		

		Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));

		runs = runs.Where(run => {
			var rd = run.GetContainer().RunData;
			return rd.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting &&
					rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		}).ToList();

		jobContainer.AddRun(runs.Single());
		var modData = ((ModalDataContainer)((VehicleContainer)runs.Single().GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);

	}

	[TestCase(Group5_HEV_S2_OVC, 12)]
	[TestCase(Group5_HEV_S3_OVC, 12)]
	[TestCase(Group5_HEV_S4_OVC, 12)]
	[TestCase(Group5_HEV_S_IEPC_ovc, 12)]
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

		
		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()); //init soc
		foreach (DataRow modDataRow in modData.Rows) {
			Assert.AreEqual(soc, modDataRow.Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()));
			Assert.IsFalse(modDataRow.Field<bool>(ModalResultField.ICEOn.GetName()));
			AssertSHEV_PEV_Conditioning(modDataRow, runs.Single());
		}
		Assert.IsTrue(modData.Rows.Count > 0);
	}

	[TestCase(Group5_HEV_S2_OVC, 12)]
	[TestCase(Group5_HEV_S3_OVC, 12)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S4.xml", 6)]
	[TestCase(Group5_HEV_S_IEPC, 6)]
	[TestCase(Group2_HEV_IEPC_S_StdVal, 6)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S2.xml", 6)]
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

	[TestCase(@"HeavyLorry\PEV\PEV_heavyLorry_AMT_E2.xml", 6)]
	[TestCase(Group5_PEV_E3,10)]
	[TestCase(@"HeavyLorry\PEV\Group5_ PEV_E4.xml",10)]
	[TestCase(@"HeavyLorry\PEV\PEV_heavyLorry_E4_standardValues.xml", 10)]
	[TestCase(@"HeavyLorry\PEV\Group5_ PEV_IEPC_E.xml",10)]
	public void PEV(string jobFile, int nrRuns)
	{
		SummaryDataContainer sumDataContainer;
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out sumDataContainer);

		Assert.IsTrue(runs.All(run => run.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));
		//Assert.AreEqual(0, runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.NotApplicable));


		var run = runs.Single(run => {
			var rd = run.GetContainer().RunData;
			return rd.Mission.MissionType == MissionType.UrbanDelivery && rd.Loading == LoadingType.ReferenceLoad;
		});
		jobContainer.AddRun(run);

	



		Assert.AreEqual(1, jobContainer.Runs.Count);

		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
		jobContainer.Execute(false);
		WaitAndAssertSuccess(jobContainer, fileWriter);


		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()); //init soc
		foreach (DataRow modDataRow in modData.Rows)
		{
			Assert.AreEqual(soc, modDataRow.Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()));
			//Assert.IsFalse(modDataRow.Field<bool>(ModalResultField.ICEOn.GetName()));
			if (!run.GetContainer().RunData.JobType.IsOneOf(VectoSimulationJobType.IEPC_S, VectoSimulationJobType.IEPC_E)) {
				AssertSHEV_PEV_Conditioning(modDataRow, run);
			}
		}

		//foreach (var vectoRun in runs.Where(r => r != run))
		//{
		//	var rd = vectoRun.GetContainer().RunData;
		//	rd.Report.AddResult(rd, modData);
		//}

		Assert.IsTrue(modData.Rows.Count > 0);
	}

	[Test]
	public void PEVPtoTransmission(string jobFile, int nrRuns)
	{
		Assert.Fail();
	}

	[TestCase(@"HeavyLorry\PEV\PEV_heavyLorry_E4_pto.xml", 8)]
	[TestCase(@"HeavyLorry\PEV\Group5_ PEV_IEPC_E_pto.xml", 8)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_S3_pto.xml", 8)]
	[TestCase(@"HeavyLorry\S-HEV\Group2_HEV_IEPC_S_pto.xml", 8)]
	public void EPTO(string jobFile, int nrRuns)
	{
		var jobContainer = GetJobContainer(jobFile, nrRuns, out var fileWriter, out var runs, out var sumDataContainer,
			true);
		Assert.IsTrue(runs.Any(run => run.GetContainer().RunData.Mission.MissionType == MissionType.MunicipalUtility));
		var run = runs.First(run => run.GetContainer().RunData.Mission.MissionType == MissionType.MunicipalUtility);
		jobContainer.AddRun(run);

		var modData = ((ModalDataContainer)((VehicleContainer)run.GetContainer()).ModData).Data;
		jobContainer.Execute();

		var ptoCol = "P_aux_PTO_CONSUM_el [kW]";
		var speedCol = "v_act";
		var timeCol = "time";

		WaitAndAssertSuccess(jobContainer, fileWriter);
		var soc = modData.Rows[0].Field<Scalar>(ModalResultField.REESSStateOfCharge.GetName()); //init soc
		var ptoState = PTOState.VehicleDriving;


		var eptoCSV =
			VectoCSVFile.ReadStream(RessourceHelper.ReadStream(DeclarationData.PTO.DefaultE_PTOActivationCycle));
		var eptoCurve = new LinearCurve();
		foreach (DataRow row in eptoCSV.Rows) {
			eptoCurve.AddPoint(row.ParseDouble(0), row.ParseDouble(1) * 1000);
		}

		double startTime = 0; 
		for (var i = 0; i < modData.Rows.Count - 1; i++) {
			switch (ptoState) {
				case PTOState.VehicleDriving:
					if (modData.Rows[i].Field<MeterPerSecond>(speedCol).Value() == 0) {
						ptoState = PTOState.VehicleStopped;
					}
					Assert.AreEqual(0, modData.Rows[i].Field<Watt>(ptoCol).Value());
					break;
				case PTOState.VehicleStopped:
					if (modData.Rows[i].Field<MeterPerSecond>(speedCol).Value() > 0) {
						ptoState = PTOState.VehicleDriving;
						continue;
					}

					if (modData.Rows[i].Field<Watt>(ptoCol).Value() > 0) {
						ptoState = PTOState.PTOActive;
						startTime = modData.Rows[i].Field<Second>(timeCol).Value();
						Assert.AreEqual(eptoCurve.Lookup(0), modData.Rows[i].Field<Watt>(ptoCol).Value(),1E-6 );
					}
					break;
				case PTOState.PTOActive:


					if (modData.Rows[i].Field<Watt>(ptoCol).Value() == 0) {
						ptoState = PTOState.VehicleStopped;
						continue;
					}

					Assert.AreEqual(eptoCurve.Lookup(modData.Rows[i].Field<Second>(timeCol).Value() - startTime), modData.Rows[i].Field<Watt>(ptoCol).Value(), 1E-6);

					break;
				default:
					throw new ArgumentOutOfRangeException();
			}




		}
	}

	[Test, TestCaseSource(nameof(GetJsonJobs))]
	public void JSONDeclarationSmokeTest(string path)
	{
		var writeReports = false;
		var inputData = JSONInputDataFactory.ReadJsonJob(path, false);
		var fileWriter = new FileOutputWriter(path);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, inputData, fileWriter, writeReports? null : new NullDeclarationReport());    //, writeReports ? null : new NullDeclarationReport());
		runsFactory.WriteModalResults = true;
		var sumWriter = new MockSumWriter();

		var jobContainer = new JobContainer(sumWriter);
		runsFactory.SumData = sumWriter;
		//var sumDataContainer = sumWriter;
		//var runs = runsFactory.SimulationRuns().ToList();
		jobContainer.AddRuns(runsFactory);
		jobContainer.Execute(true);
		WaitAndAssertSuccess(jobContainer, fileWriter);

	}

	public static string[] GetJsonJobs()
	{
		var dirPath = Path.Combine(BASE_DIR, "JSON");
		List<string> vectoJobs = new List<string>();
		foreach (var fileName in Directory.EnumerateFiles(dirPath, "*.vecto", SearchOption.AllDirectories))
		{
			vectoJobs.Add(fileName);
		};

		return vectoJobs.ToArray();
	}





	private void WaitAndAssertSuccess(JobContainer jobContainer, FileOutputWriter fileWriter)
	{
		jobContainer.WaitFinished();
		Assert.IsTrue(jobContainer.AllCompleted);
		Assert.IsTrue(jobContainer.Runs.TrueForAll(runEntry => runEntry.Success));
		PrintRuns(jobContainer, fileWriter);
		PrintFiles(fileWriter);
	}

	[MethodImpl(MethodImplOptions.Synchronized)]
	private JobContainer GetJobContainer(string jobFile, int? nrRuns, out FileOutputWriter fileWriter,
		out List<IVectoRun> runs, out SummaryDataContainer sumDataContainer, bool writeReports = true)
	{
		var filePath = Path.Combine(BASE_DIR, jobFile);
		Assert.IsTrue(File.Exists(filePath), "Testfile not found: " + filePath);
		var dataProvider = _xmlReader.CreateDeclaration(filePath);
		fileWriter = new FileOutputWriter(filePath);
		var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter, writeReports ? null : new NullDeclarationReport());
		runsFactory.WriteModalResults = true;
		var sumWriter = new MockSumWriter();
		
		var jobContainer = new JobContainer(sumWriter);
		runsFactory.SumData = sumWriter;
		sumDataContainer = sumWriter;
		runs = runsFactory.SimulationRuns().ToList();

		if (nrRuns.HasValue) {
			Assert.AreEqual(nrRuns, runs.Count, "Cycles: \n"  + string.Join("\n", runs.Select(run => run.CycleName + "_" +  run.RunSuffix)));
		}
		TestContext.WriteLine(string.Join("\n", runs.Select(r => r.CycleName + "_" + r.RunSuffix)));

		if (dataProvider.JobInputData.Vehicle.OvcHev) {
			Assert.AreEqual(runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeDepleting),
				runs.Count(r => r.GetContainer().RunData.OVCMode == VectoRunData.OvcHevMode.ChargeSustaining));
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

	//runs.First().GetContainer().PowertrainInfo.ElectricMotorPositions;
	public void AssertSHEV_PEV_Conditioning(DataRow modDataRow, IVectoRun run)
	{
		var electricMotorPositions = run.GetContainer().PowertrainInfo.ElectricMotorPositions;
		var position = electricMotorPositions.Single(e => e != PowertrainPosition.GEN);

		var hasGen = electricMotorPositions.Any(e => e == PowertrainPosition.GEN);

		var idx = modDataRow.Table.Rows.IndexOf(modDataRow);
		if (idx - 1 < 0)
		{
			return;
		}

		var prevRow = modDataRow.Table.Rows[idx - 1];

		var condData = run.GetContainer().RunData.Aux.Single(aux => aux.ID == Constants.Auxiliaries.IDs.Cond);
		Assert.IsTrue(condData.IsFullyElectric);
		Assert.IsTrue(condData.ConnectToREESS);
		var time = modDataRow.Field<Second>(ModalResultField.time.GetName());
		//modDataRow.Table.Rows[]
		if (EMOn(prevRow, position) || (hasGen && EMOn(prevRow, PowertrainPosition.GEN)))
		{
			var cond = DeclarationData.Conditioning.LookupPowerDemand(run.GetContainer().RunData.VehicleData.VehicleClass,
				run.GetContainer().RunData.Mission.MissionType);
			var condMod = modDataRow.Field<Watt>("P_aux_COND_el [kW]");
			Assert.IsTrue(cond.IsEqual(condMod), $"expected {cond} got {condMod} at {time}");
		}
		else
		{
			var condMod = modDataRow.Field<Watt>("P_aux_COND_el [kW]");
			Assert.IsTrue(0.SI<Watt>().IsEqual(condMod), $"expected {0} got {condMod} at {time}");
		}
	}

	private static bool EMOn(DataRow prevRow, PowertrainPosition position)
	{
		if (position == PowertrainPosition.IEPC) {
			return prevRow.Field<Scalar>(string.Format(ModalResultField.IEPC_Off_.GetCaption(), position.GetLabel())) == 0.SI<Scalar>();
		}
		return prevRow.Field<Scalar>(string.Format(ModalResultField.EM_Off_.GetCaption(), position.GetLabel())) == 0.SI<Scalar>();
	}
}
