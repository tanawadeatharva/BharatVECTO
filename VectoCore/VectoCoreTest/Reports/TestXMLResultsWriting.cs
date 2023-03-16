using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Moq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.ResultWriter;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Reports;

[TestFixture]
public class TestXMLResultsWriting
{
	private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;
	//private IXMLInputDataReader _xmlReader;

	XNamespace CIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9");
	XNamespace MRF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
	XNamespace VIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		//_xmlReader = _kernel.Get<IXMLInputDataReader>();
		_reportResultsFactory = _kernel.Get<IResultsWriterFactory>();
	}


	[
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(CIFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.Tractor, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(CIFResultsWriter.HEVNonOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(CIFResultsWriter.HEVOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(CIFResultsWriter.PEVLorry)),

		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(CIFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(CIFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(CIFResultsWriter.HEVNonOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(CIFResultsWriter.HEVOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(CIFResultsWriter.PEVBus)),

	]
	public void Test_CIF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}



	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: Lorry Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: Lorry Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, true, TestName = "CIF_ReportResult_WritingResults: Lorry PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, false, TestName = "CIF_ReportResult_WritingResults: Lorry PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, true, true, TestName = "CIF_ReportResult_WritingResults: Lorry HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, true, true, true, TestName = "CIF_ReportResult_WritingResults: Lorry Conv exempted"),
	]
	public void Test_CIF_ReportResult_WritingResults_Lorry(VectoSimulationJobType jobType, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = ovc ? VectoRunData.OvcHevMode.ChargeDepleting : VectoRunData.OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);
		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, VectoRunData.OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}

		var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputCustomer.0.9", CIF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

		WriteToFile("CIF", doc, runData, success, exempted);
	}


	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, FuelType.NGPI, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, false, TestName = "CIF_ReportResult_WritingResults: CompletedBus PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, true, true, TestName = "CIF_ReportResult_WritingResults: CompletedBus HEV exempted"),
	]
	public void TestReportResult_WritingResults_CompletedBus(VectoSimulationJobType jobType, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
		var ovcmode = ovc ? VectoRunData.OvcHevMode.ChargeDepleting : VectoRunData.OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);
		resultEntries.Add(resultEntry);

		if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric) {
			resultEntry.AuxHeaterFuel = FuelData.Diesel;
			resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
			resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
		}

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, VectoRunData.OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}

		var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputCustomer.0.9", CIF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

		WriteToFile("CIF", doc, runData, success, exempted);
	}


	// ---------------------------
	// MRF Tests

	[
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(MRFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(MRFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.Tractor, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(MRFResultsWriter.ConventionalLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(MRFResultsWriter.HEVNonOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(MRFResultsWriter.HEVOVCLorry)),
		TestCase(VehicleCategory.RigidTruck, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(MRFResultsWriter.PEVLorry)),

		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(MRFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(MRFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(MRFResultsWriter.HEVNonOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(MRFResultsWriter.HEVOVCBus)),
		TestCase(VehicleCategory.HeavyBusCompletedVehicle, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(MRFResultsWriter.PEVBus)),
	]
	public void Test_MRF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}


	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: Lorry Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: Lorry Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, true, TestName = "MRF_ReportResult_WritingResults: Lorry PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, false, TestName = "MRF_ReportResult_WritingResults: Lorry PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, true, true, TestName = "MRF_ReportResult_WritingResults: Lorry HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, true, true, true, TestName = "MRF_ReportResult_WritingResults: Lorry Conv exempted"),
	]
	public void Test_MRF_ReportResult_WritingResults_Lorry(VectoSimulationJobType jobType, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = ovc ? VectoRunData.OvcHevMode.ChargeDepleting : VectoRunData.OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);

		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, VectoRunData.OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}

		var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputManufacturer.0.9", MRF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

		WriteToFile("MRF", doc, runData, success, exempted);
	}

	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, true, false, true, FuelType.NGPI, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, false, TestName = "MRF_ReportResult_WritingResults: CompletedBus PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, true, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, true, true, true, TestName = "MRF_ReportResult_WritingResults: CompletedBus Conv exempted"),
	]
	public void Test_MRF_ReportResult_WritingResults_Bus(VectoSimulationJobType jobType, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.HeavyBusCompletedVehicle;
		var ovcmode = ovc ? VectoRunData.OvcHevMode.ChargeDepleting : VectoRunData.OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);

		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, VectoRunData.OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}

		var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputManufacturer.0.9", MRF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

		WriteToFile("MRF", doc, runData, success, exempted);
	}

	// ------------
	// VIF Tests

	[
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ConventionalVehicle, false, true, typeof(VIFResultsWriter.ExemptedVehicle)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ConventionalVehicle, false, false, typeof(VIFResultsWriter.ConventionalBus)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.ParallelHybridVehicle, false, false, typeof(VIFResultsWriter.HEVNonOVCBus)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.SerialHybridVehicle, true, false, typeof(VIFResultsWriter.HEVOVCBus)),
		TestCase(VehicleCategory.HeavyBusPrimaryVehicle, VectoSimulationJobType.BatteryElectricVehicle, true, false, typeof(VIFResultsWriter.PEVBus)),
	]
	public void Test_VIF_ReportResultInstance(VehicleCategory vehicleCategory, VectoSimulationJobType jobType, bool ovc,
		bool exempted, Type expectedResultWriterType)
	{
		var resultsWriter = _reportResultsFactory.GetVIFResultsWriter(vehicleCategory.GetVehicleType(), jobType, ovc, exempted);

		Assert.AreEqual(expectedResultWriterType, resultsWriter.GetType());
	}

	[
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, true, FuelType.NGPI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, false, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC SUCCESS"),
		TestCase(VectoSimulationJobType.SerialHybridVehicle, true, false, true, FuelType.NGCI, FuelType.DieselCI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC DualFuel SUCCESS"),
		TestCase(VectoSimulationJobType.IEPC_S, true, false, true, FuelType.NGPI, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC LNG SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV OVC ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV non-OVC SUCCESS"),
		TestCase(VectoSimulationJobType.ParallelHybridVehicle, false, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV non-OVC ERROR"),

		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus PEV SUCCESS"),
		TestCase(VectoSimulationJobType.BatteryElectricVehicle, true, false, false, TestName = "VIF_ReportResult_WritingResults: PrimaryBus PEV ERROR"),

		TestCase(VectoSimulationJobType.ParallelHybridVehicle, true, true, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus HEV exempted"),
		TestCase(VectoSimulationJobType.ConventionalVehicle, true, true, true, TestName = "VIF_ReportResult_WritingResults: PrimaryBus Conv exempted"),
	]
	public void Test_VIF_ReportResult_WritingResults_Bus(VectoSimulationJobType jobType, bool ovc, bool exempted, bool success, params FuelType[] fuels)
	{
		var vehicleCategory = VehicleCategory.HeavyBusPrimaryVehicle;
		var ovcmode = ovc ? VectoRunData.OvcHevMode.ChargeDepleting : VectoRunData.OvcHevMode.NotApplicable;
		var runData = GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
		var modData = GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

		var resultEntries = new List<IResultEntry>();

		var resultEntry = GetResultEntry(runData);
		resultEntry.SetResultData(runData, modData, 1);

		resultEntries.Add(resultEntry);

		if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
			var run2 = GetMockRunData(vehicleCategory, jobType, true, exempted, VectoRunData.OvcHevMode.ChargeSustaining, fuels);
			var res2 = GetResultEntry(run2);
			res2.SetResultData(run2, modData, 1);
			resultEntries.Add(res2);
		}

		var resultsWriter = _reportResultsFactory.GetVIFResultsWriter(
			runData.VehicleData.VehicleCategory.GetVehicleType(),
			runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

		var results = resultsWriter.GenerateResults(resultEntries);

		Assert.NotNull(results);

		var doc = CreateXmlDocument(results, "VectoOutputMultistep.0.1", VIF_NS);
		var validator = GetValidator(doc);

		WriteToConsole(doc);

		Assert.IsTrue(validator.ValidateXML(XmlDocumentType.MultistepOutputData), validator.ValidationError);

		WriteToFile("VIF", doc, runData, success, exempted);
	}

	// ---------

	[TestCase()]
	public void TestCalculateOVCWeightedResult(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = VectoRunData.OvcHevMode.ChargeDepleting;
		var runData = GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels);
		var modData = GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		var cdResult = GetResultEntry(runData);
		cdResult.SetResultData(runData, modData, 1);

		var run2 = GetMockRunData(vehicleCategory, jobType, true, false, VectoRunData.OvcHevMode.ChargeSustaining, fuels);
		var modData2 = GetMockModData(VectoRun.Status.Success, fuels, VectoRunData.OvcHevMode.ChargeSustaining);
		var csResult = GetResultEntry(run2);
		csResult.SetResultData(run2, modData2, 1);

		var weighted = DeclarationData.CalculateWeightedResult(cdResult, csResult);

		Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)} {weighted.UtilityFactor.ToXMLFormat(3)}" +
						$" {weighted.ElectricEnergyConsumption.Value().ToXMLFormat(3)} {weighted.FuelConsumption[FuelData.Diesel].Value().ToXMLFormat(3)} {weighted.CO2Total.Value().ToXMLFormat(3)}");

		//1518.750 1366.875 1366.875 0.004 795230.237 30.890 20.000

		Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
		Assert.AreEqual(1366.875, weighted.EquivalentAllElectricRange.Value(), 1e-3);
		Assert.AreEqual(1366.875, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);
		Assert.AreEqual(0.004, weighted.UtilityFactor, 1e-3);
		Assert.AreEqual(795230.237, weighted.ElectricEnergyConsumption.Value(), 1e-3);
		Assert.AreEqual(30.890, weighted.FuelConsumption[FuelData.Diesel].Value(), 1e-3);
		Assert.AreEqual(20.000, weighted.CO2Total.Value(), 1e-3);

	}

	// ---------

	[TestCase()]
	public void TestCalculatePEVRanges(params FuelType[] fuels)
	{
		var jobType = VectoSimulationJobType.ParallelHybridVehicle;
		var vehicleCategory = VehicleCategory.RigidTruck;
		var ovcmode = VectoRunData.OvcHevMode.ChargeDepleting;
		var runData = GetMockRunData(vehicleCategory, jobType, true, false, ovcmode, fuels);
		var modData = GetMockModData(VectoRun.Status.Success, fuels, ovcmode);

		//var result = GetResultEntry(runData);
		//result.SetResultData(runData, modData, 1);


		var weighted = DeclarationData.CalculateElectricRangesPEV(runData, modData);

		Console.WriteLine($"{weighted.ActualChargeDepletingRange.Value().ToXMLFormat(3)} {weighted.EquivalentAllElectricRange.Value().ToXMLFormat(3)} {weighted.ZeroCO2EmissionsRange.Value().ToXMLFormat(3)}");

		//1518.750 1366.875 1366.875 0.004 797877.345 30.890 20.000

		Assert.AreEqual(1518.750, weighted.ActualChargeDepletingRange.Value(), 1e-3);
		Assert.AreEqual(1518.750, weighted.EquivalentAllElectricRange.Value(), 1e-3);
		Assert.AreEqual(1518.750, weighted.ZeroCO2EmissionsRange.Value(), 1e-3);

	}

	// ===================================

	private static void WriteToConsole(XDocument doc)
	{
		var m = new MemoryStream();
		var writer = new XmlTextWriter(m, Encoding.UTF8) { Formatting = Formatting.Indented };
		doc.WriteTo(writer);
		writer.Flush();
		m.Flush();
		m.Seek(0, SeekOrigin.Begin);
		Console.WriteLine(new StreamReader(m).ReadToEnd());
	}

    
	private void WriteToFile(string prefix, XDocument doc, VectoRunData runData, bool success, bool exempted)
	{
		lock (this) {
			var fileName = GetFilename(prefix, runData, success, exempted);
			var filePath = Path.Combine("TestDummyResults", fileName);
			if (!Directory.Exists(Path.GetDirectoryName(filePath))) {
				Directory.CreateDirectory(Path.GetDirectoryName(filePath));
			}

			if (File.Exists(filePath)) {
				File.Delete(filePath);
			}

			var writer = new XmlTextWriter(filePath, Encoding.UTF8) { Formatting = Formatting.Indented };
			doc.WriteTo(writer);
			writer.Flush();
		}
	}

	private string GetFilename(string prefix, VectoRunData runData, bool success, bool exempted)
	{
		var arch = string.Empty;
		switch (runData.JobType.GetPowertrainArchitectureType()) {
			case VectoSimulationJobTypeHelper.Hybrid:
				arch = (runData.VehicleData.OffVehicleCharging ? "OVC" : "non-OVC") + "-HEV";
				break;
			case VectoSimulationJobTypeHelper.Conventional:
				arch = "Conv";
				break;
			case VectoSimulationJobTypeHelper.PureElectric:
				arch = "PEV";
				break;
		}

		var category = runData.VehicleData.VehicleCategory.IsLorry() ? "Lorry" : "Bus";
		var suffix = success ? null : "_ERR";
		var exept = exempted ? "_exempted" : null;

		var fuelSuffix = "";
		var fuels = runData.EngineData.Fuels;
		if (fuels.Any(x => x.FuelData.FuelType != FuelType.DieselCI)) {
			fuelSuffix = "_" + fuels.Select(x => x.FuelData.FuelType.ToXMLFormat().Replace(' ', '-')).Join("_");
		}
		return $"{prefix}_MockupResults_{arch}_{category}{fuelSuffix}{suffix}{exept}.xml";
	}


	private static XMLDeclarationReport.ResultEntry GetResultEntry(VectoRunData runData)
	{
		var resultEntry = new XMLDeclarationReport.ResultEntry();
		resultEntry.Initialize(runData);
		return resultEntry;
	}

	private XMLValidator GetValidator(XDocument doc)
	{
		var ms = new MemoryStream();
		var writer = new XmlTextWriter(ms, Encoding.UTF8);
		doc.WriteTo(writer);
		writer.Flush();
		ms.Flush();
		ms.Seek(0, SeekOrigin.Begin);
		return new XMLValidator(new XmlTextReader(ms));
	}

	private XDocument CreateXmlDocument(XElement results, string reportType, XNamespace ns)
	{
		var xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		
		var doc = new XDocument();
		doc.Add(new XElement(ns + "VectoMockResults",
			new XAttribute(XNamespace.Xmlns + "xsi", xsi.NamespaceName),
			new XAttribute(xsi + "schemaLocation", $@"{ns.NamespaceName} V:\VectoCore\VectoCore\Resources\XSD\{reportType}.xsd"),
			results));

		return doc;
	}

	private IModalDataContainer GetMockModData(VectoRun.Status runStatus, FuelType[] fuelTypes, VectoRunData.OvcHevMode ovcMode = VectoRunData.OvcHevMode.NotApplicable)
	{
		var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new[] { FuelType.DieselCI } : fuelTypes;

		var modData = new Mock<IModalDataContainer>();
		modData.Setup(x => x.RunStatus).Returns(runStatus);
		modData.Setup(x => x.Duration).Returns(3600.SI<Second>());
		modData.Setup(x => x.Distance).Returns(30000.SI<Meter>());
		modData.Setup(x => x.GetValues<MeterPerSecond>(ModalResultField.v_act)).Returns(new[] { 0.KMPHtoMeterPerSecond(), 50.KMPHtoMeterPerSecond() });
		modData.Setup(x => x.GetValues<MeterPerSquareSecond>(ModalResultField.acc)).Returns(new[] { -1.SI<MeterPerSquareSecond>(), 0.SI<MeterPerSquareSecond>(), 1.SI<MeterPerSquareSecond>()});
		modData.Setup(x => x.GetValues<uint>(ModalResultField.Gear)).Returns(new[] { 0u, 2u, 0u, 3u, 0u });

		var e_gbxIn = 1000.SI<WattSecond>();
		var gbxEff = 0.98;
		var axlEff = 0.97;
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn);
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff);
		modData.Setup(x => x.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, It.IsNotNull<Func<SI, bool>>())).Returns(e_gbxIn * gbxEff * axlEff);

		if (runStatus != VectoRun.Status.Success) {
			modData.Setup(x => x.Error).Returns("TestCase Error!");
			modData.Setup(x => x.StackTrace).Returns("Testcase Stacktrace");
		}

		var mc = new Mock<ICorrectedModalData>();
		modData.Setup(x => x.CorrectedModalData).Returns(mc.Object);

		var fcCorrected = new Dictionary<FuelType, IFuelConsumptionCorrection>();
		var ovcFactor = ovcMode == VectoRunData.OvcHevMode.ChargeDepleting ? 0.1 : 1.0;
		foreach (var fuelType in fuels) {
			var factor = fcCorrected.Count == 0 ? 1 : 0.1;
			var fc = new Mock<IFuelConsumptionCorrection>();
			fc.Setup(x => x.Fuel).Returns(DeclarationData.FuelData.Lookup(fuelType, TankSystem.Liquefied));
			fc.Setup(x => x.TotalFuelConsumptionCorrected).Returns(31.SI<Kilogram>() * factor * ovcFactor);
			fc.Setup(x => x.EnergyDemand).Returns(31.SI<Kilogram>() * factor * ovcFactor * FuelData.Diesel.LowerHeatingValueVecto);
			fcCorrected.Add(fuelType, fc.Object);
		}
		mc.Setup(x => x.FuelCorrection).Returns(fcCorrected);

		mc.Setup(x => x.CO2Total).Returns(20.SI<Kilogram>());
		mc.Setup(x => x.FuelEnergyConsumptionTotal).Returns(1e9.SI<Joule>());

		var elOvcFactor = ovcMode == VectoRunData.OvcHevMode.ChargeSustaining ? 0 : 1.0;
		mc.Setup(x => x.ElectricEnergyConsumption_SoC).Returns(200.SI(Unit.SI.Mega.Joule).Cast<WattSecond>() * elOvcFactor);

		return modData.Object;
	}

	private VectoRunData GetMockRunData(VehicleCategory vehicleCategory, VectoSimulationJobType jobType,
		bool offVehicleCharging, bool exempted, VectoRunData.OvcHevMode ovcMode, FuelType[] fuelTypes)
	{
		var fuels = fuelTypes == null || fuelTypes.Length == 0 ? new [] { FuelType.DieselCI } : fuelTypes;
		return new VectoRunData() {
			Mission = new Mission() {
				MissionType = MissionType.LongHaul
			},
			OVCMode = ovcMode,
			Exempted = exempted,
			JobType = jobType,
			Loading = LoadingType.LowLoading,
			MaxChargingPower = 250.SI(Unit.SI.Kilo.Watt).Cast<Watt>(),
			VehicleData = new VehicleData() {
				CurbMass = 7600.SI<Kilogram>(),
				Loading = 5000.SI<Kilogram>(),
				CargoVolume = 20.SI<CubicMeter>(),
				PassengerCount = 20,
				VehicleClass = VehicleClass.Class5,
				VehicleCategory = vehicleCategory,
				OffVehicleCharging = offVehicleCharging,
			},
			EngineData = new CombustionEngineData() {
				FuelMode = 0,
				Fuels = fuels.Select(x => new CombustionEngineFuelData()
					{ FuelData = DeclarationData.FuelData.Lookup(x, TankSystem.Liquefied) }).ToList(),
			},
			Retarder = new RetarderData() {
				Type = RetarderType.None,
			},
			BatteryData = new BatterySystemData() {
				Batteries = new List<Tuple<int, BatteryData>>() {
					Tuple.Create(1, new BatteryData() {
						BatteryId = 0,
						Capacity = 7.5.SI(Unit.SI.Ampere.Hour).Cast<AmpereSecond>(),
						ChargeSustainingBattery = true,
						MinSOC = 0.2,
						MaxSOC = 0.8,
						SOCMap = BatterySOCReader.Create("SoC, V\n0, 600\n100, 650\n".ToStream()),
						InternalResistance = BatteryInternalResistanceReader.Create("SoC, Ri-2, Ri-10, Ri-20\n0, 20, 20, 20\n100, 20, 20, 20\n".ToStream(), true),
						MaxCurrent = BatteryMaxCurrentReader.Create("SoC, I_charge, I_discharge\n0, 300, 300\n100, 500, 500\n".ToStream())
						
					})
				}
			}
		};
	}
}