using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.Simulation.Impl.SimulatorFactory;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.MonitoringReport;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing.Impl;

namespace TUGraz.VectoCore.Tests.XML.Reports
{
	[TestFixture]
	[NonParallelizable]
    public class MonitoringReportWriterTest : MRF_CIF_WriterTestBase
    {
		private IXMLManufacturerReport GetManufacturerReport(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			var dataProvider = _xmlReader.CreateDeclaration(fileName);
			
			var ihpc = (dataProvider.JobInputData.Vehicle.Components?.ElectricMachines?.Entries)?.Count(electric =>
				electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components?.IEPC != null);
			
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);

			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();
			
			return report;
		}

		private IXMLManufacturerReport GetCompletedBusManufacturerReport(string fileName)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			var dataProvider = _xmlReader.CreateDeclaration(fileName) as IMultistepBusInputDataProvider;
			
			var ihpc = (dataProvider.JobInputData.PrimaryVehicle.Vehicle.Components?.ElectricMachines?.Entries)?.Count(electric => electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.PrimaryVehicle.Vehicle.Components?.IEPC != null);
			
			var report = _mrfFactory.GetManufacturerReport(
				dataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID,
				dataProvider.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);

			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();
			
			return report;
		}

        private IXMLMonitoringReport GetMonitoringReport(string fileName,
			out IDeclarationInputDataProvider dataProvider)
        { 
			Assert.IsFalse(string.IsNullOrEmpty(fileName));

			dataProvider = _xmlReader.CreateDeclaration(fileName);

			var mrf = (dataProvider is IMultistepBusInputDataProvider)
				? GetCompletedBusManufacturerReport(fileName)
				: GetManufacturerReport(fileName);

			return new XMLMonitoringReport(mrf);
        }

        private static VectoRunData GetRunData(IDeclarationInputDataProvider dataProvider)
		{
			var axleData =  new List<Axle>();

			var axles = (dataProvider is IMultistepBusInputDataProvider)
				? new List<IAxleDeclarationInputData>()
				: (dataProvider.JobInputData.Vehicle.Components != null)
					? dataProvider.JobInputData.Vehicle.Components.AxleWheels.AxlesDeclaration
					: new List<IAxleDeclarationInputData>();

			foreach(var axle in axles) {
				axleData.Add(new Axle() { AxleType = axle.AxleType });
			}

			var axleGearData = (dataProvider is IMultistepBusInputDataProvider)
				? null
				: (dataProvider.JobInputData.Vehicle.Components != null)
					? (dataProvider.JobInputData.Vehicle.Components.AxleGearInputData != null)
						? new AxleGearData()
						: null
					: null;

			return new VectoRunData() {
				InputData = dataProvider,
				VehicleData = new VehicleData() {
					OffVehicleCharging = false,
					AxleData = axleData
				},
				AxleGearData = axleGearData
			};
		}

		public void ValidateMRFHash(string reportPath)
		{
			var xmlDoc = new XmlDocument();
			xmlDoc.Load(reportPath);

			XmlNamespaceManager namespaces = new XmlNamespaceManager(xmlDoc.NameTable);
			namespaces.AddNamespace("ns", XMLDefinitions.MONITORING_NAMESPACE_URI);
			
			var signatureNode = xmlDoc.SelectSingleNode("/ns:VectoMonitoring/ns:ManufacturerRecord/ns:Signature", namespaces);
			var signatureDigest = new DigestData(signatureNode);

			var hash = XMLHashProvider.ComputeHash(xmlDoc, signatureDigest.Reference.Remove(0, 1), signatureDigest.CanonicalizationMethods,
				signatureDigest.DigestMethod);
			
			Assert.IsTrue(hash.InnerText.Equals(signatureDigest.DigestValue), 
				$"Manufacturer Report hash: {signatureDigest.DigestValue} differs from calculated hash: {hash.InnerText}");
		}

        [TestCase(ConventionalHeavyLorry, TestName="MonitoringReport_ConventionalHeavyLorry")]
		[TestCase(Conventional_PrimaryBus, TestName="MonitoringReport_ConventionalPrimaryBus")]
		[TestCase(Conventional_CompletedBus, TestName="MonitoringReport_ConventionalCompletedBus")]
		[TestCase(HEV_Px_HeavyLorry, TestName="MonitoringReport_HEV_Px_HeavyLorry")]
		[TestCase(HEV_Px_IHPC_PrimaryBus, TestName="MonitoringReport_HEV_Px_IHPC_PrimaryBus")]
		[TestCase(HEV_S2_HeavyLorry, TestName="MonitoringReport_HEV_S2_HeavyLorry")]
		[TestCase(HEV_S2_PrimaryBus, TestName="MonitoringReport_HEV_S2_PrimaryBus")]
		[TestCase(HEV_S3_HeavyLorry, TestName="MonitoringReport_HEV_S3_HeavyLorry")]
		[TestCase(HEV_S3_PrimaryBus, TestName="MonitoringReport_HEV_S3_PrimaryBus")]
		[TestCase(HEV_S4_HeavyLorry, TestName="MonitoringReport_HEV_S4_HeavyLorry")]
		[TestCase(HEV_S4_PrimaryBus, TestName="MonitoringReport_HEV_S4_PrimaryBus")]
		[TestCase(HEV_IEPC_S_HeavyLorry, TestName="MonitoringReport_HEV_IEPC_S_HeavyLorry")]
		[TestCase(HEV_IEPC_S_NoAxlegear_HeavyLorry, TestName="MonitoringReport_HEV_IEPC_S_NoAxlegear_HeavyLorry")]
		[TestCase(HEV_IEPC_S_PrimaryBus, TestName="MonitoringReport_HEV_IEPC_S_PrimaryBus")]
		[TestCase(HEVCompletedBus, TestName="MonitoringReport_HEVCompletedBus")]
		[TestCase(PEV_E2_HeavyLorry, TestName="MonitoringReport_PEV_E2_HeavyLorry")]
		[TestCase(PEV_E2_PrimaryBus, TestName="MonitoringReport_PEV_E2_PrimaryBus")]
		[TestCase(PEV_E3_HeavyLorry, TestName="MonitoringReport_PEV_E3_HeavyLorry")]
		[TestCase(PEV_E3_PrimaryBus, TestName="MonitoringReport_PEV_E3_PrimaryBus")]
		[TestCase(PEV_E4_HeavyLorry, TestName="MonitoringReport_PEV_E4_HeavyLorry")]
		[TestCase(PEV_E4_PrimaryBus, TestName="MonitoringReport_PEV_E4_PrimaryBus")]
		[TestCase(PEV_IEPC_HeavyLorry, TestName="MonitoringReport_PEV_IEPC_HeavyLorry")]
		[TestCase(PEV_IEPC_NoAxlegear_HeavyLorry, TestName="MonitoringReport_PEV_IEPC_NoAxlegear_HeavyLorry")]
		[TestCase(PEV_IEPC_PrimaryBus, TestName="MonitoringReport_PEV_IEPC_PrimaryBus")]
		[TestCase(PEVCompletedBus, TestName="MonitoringReport_PEVCompletedBus")]
		[TestCase(Exempted_HeavyLorry, TestName="MonitoringReport_Exempted_HeavyLorry")]
		[TestCase(Exempted_PrimaryBus, TestName="MonitoringReport_Exempted_PrimaryBus")]
		[TestCase(Exempted_CompletedBus, TestName="MonitoringReport_Exempted_CompletedBus")]
        public void TestMonitoringReport(string fileName)
		{
			var report = GetMonitoringReport(fileName, out var dataProvider) as XMLMonitoringReport;

			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport();

			Assert.IsTrue(ValidateAndPrint(report.Report, VectoCore.Utils.XmlDocumentType.MonitoringReport));
            Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));

			var destPath = Path.Combine(outputBasePath, TestContext.CurrentContext.Test.MethodName+".xml");
			ValidateMRFHash(destPath);
		}

		private const string LORRY_JOB = @"TestData/Generic Vehicles/Declaration Mode/PEV_Lorry/Group5_ PEV_E4.xml";
		[
		TestCase(LORRY_JOB, TestName="MonitoringReport_RunJob_Lorry")
		]
		public void TestMonitoringReportWithJobRun(string fileName)
		{ 
			var fileWriter = new FileOutputWriter(fileName);
			var sumWriter = new SummaryDataContainer(fileWriter);
			var jobContainer = new JobContainer(sumWriter);
			var inputReader = _kernel.Get<IXMLInputDataReader>();
			var dataProvider = inputReader.Create(fileName);
			var runsFactory = SimulatorFactory.CreateSimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.WriteModalResults = false;
			runsFactory.Validate = false;
			runsFactory.ActualModalData = false;

			jobContainer.AddRuns(runsFactory);

			jobContainer.Execute();
			jobContainer.WaitFinished();

			Assert.AreEqual(true, jobContainer.AllCompleted);

			var validator = new XMLValidator(XmlReader.Create(fileWriter.XMLMonitoringReportName));
			validator.ValidateXML(VectoCore.Utils.XmlDocumentType.MonitoringReport);
			
			Assert.IsNull(validator.ValidationError);

			ValidateMRFHash(fileWriter.XMLMonitoringReportName);

			var validatorMRF = new XMLValidator(XmlReader.Create(fileWriter.XMLFullReportName));
			validatorMRF.ValidateXML(VectoCore.Utils.XmlDocumentType.ManufacturerReport);
			
			Assert.IsNull(validatorMRF.ValidationError);
		}

	}
}
