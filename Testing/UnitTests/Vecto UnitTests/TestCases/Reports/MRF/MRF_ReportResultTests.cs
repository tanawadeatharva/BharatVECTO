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
using TUGraz.VectoCore.Utils;
using Assert = NUnit.Framework.Assert;

namespace TUGraz.Vecto.UnitTests.TestCases.Reports.MRF;

public class MRF_ReportResultTests
{
	XNamespace MRF_NS = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");

	private StandardKernel _kernel;

	private IResultsWriterFactory _reportResultsFactory;

	[OneTimeSetUp]
	public void OneTimeSetup()
	{
		_kernel = new StandardKernel(new VectoNinjectModule());
		_reportResultsFactory = _kernel.Get<IResultsWriterFactory>();
	}


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

        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, "VectoOutputManufacturer.0.9", MRF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

        //WriteToFile("MRF", doc, runData, success, exempted);
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
        var ovcmode = ovc ? OvcHevMode.ChargeDepleting : OvcHevMode.NotApplicable;
        var runData = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, ovc, exempted, ovcmode, fuels);
        var modData = ReportResultTestUtils.GetMockModData(success ? VectoRun.Status.Success : VectoRun.Status.Aborted, fuels);

        var resultEntries = new List<IResultEntry>();

        var resultEntry = ReportResultTestUtils.GetResultEntry(runData);
        resultEntry.SetResultData(runData, modData, 1);

        if (jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.PureElectric) {
            resultEntry.AuxHeaterFuel = FuelData.Diesel;
            resultEntry.ZEV_FuelConsumption_AuxHtr = 1.SI<Kilogram>();
            resultEntry.ZEV_CO2 = resultEntry.ZEV_FuelConsumption_AuxHtr * resultEntry.AuxHeaterFuel.CO2PerFuelWeight;
        }

        resultEntries.Add(resultEntry);

        if (ovc && jobType.GetPowertrainArchitectureType() == VectoSimulationJobTypeHelper.Hybrid) {
            var run2 = ReportResultTestUtils.GetMockRunData(vehicleCategory, jobType, true, exempted, OvcHevMode.ChargeSustaining, fuels);
            var res2 = ReportResultTestUtils.GetResultEntry(run2);
            res2.SetResultData(run2, modData, 1);
            resultEntries.Add(res2);
        }

        var resultsWriter = _reportResultsFactory.GetMRFResultsWriter(
            runData.VehicleData.VehicleCategory.GetVehicleType(),
            runData.JobType, runData.VehicleData.OffVehicleCharging, runData.Exempted);

        var results = resultsWriter.GenerateResults(resultEntries);

        Assert.NotNull(results);

        var doc = ReportResultTestUtils.CreateXmlDocument(results, "VectoOutputManufacturer.0.9", MRF_NS);
        var validator = ReportResultTestUtils.GetValidator(doc);

		ReportResultTestUtils.WriteToConsole(doc);

        Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport), validator.ValidationError);

        //WriteToFile("MRF", doc, runData, success, exempted);
    }
}