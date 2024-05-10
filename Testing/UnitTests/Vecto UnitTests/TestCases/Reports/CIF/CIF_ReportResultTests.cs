using System.Xml.Linq;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter;
using Assert = NUnit.Framework.Assert;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.CIF;

public class CIF_ReportResultTests
{
	XNamespace CIF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9");

    private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
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
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
        var modData = ReportResultTestUtils.GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

        var resultEntries = new List<IResultEntry>();

        var resultEntry = ReportResultTestUtils.GetResultEntry(runData);
        resultEntry.SetResultData(runData, modData, 1);
        resultEntries.Add(resultEntry);

        if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
            var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
            var res2 = ReportResultTestUtils.GetResultEntry(run2);
            res2.SetResultData(run2, modData, 1);
            resultEntries.Add(res2);
        }

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, "VectoOutputCustomer.0.9", CIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

        //WriteToFile("CIF", doc, runData, success, exempted);
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
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
        var modData = ReportResultTestUtils.GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

        var resultEntries = new List<IResultEntry>();

        var resultEntry = ReportResultTestUtils.GetResultEntry(runData);
        resultEntry.SetResultData(runData, modData, 1);
        resultEntries.Add(resultEntry);

        if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric) {
            resultEntry.AuxHeaterFuel = FuelData.Diesel;
            resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
            resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
        }

        if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
            var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
            var res2 = ReportResultTestUtils.GetResultEntry(run2);
            res2.SetResultData(run2, modData, 1);
            resultEntries.Add(res2);
        }

        var resultsWriter = _reportResultsFactory.GetCIFResultsWriter(
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, "VectoOutputCustomer.0.9", CIF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.CustomerReport), validator.ValidationError);

        // WriteToFile("CIF", doc, runData, success, exempted);
    }

	
}