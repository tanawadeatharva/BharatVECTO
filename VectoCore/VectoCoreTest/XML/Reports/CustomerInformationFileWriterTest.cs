using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData.XML;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_1_0.CustomerInformationFile;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.XML.Reports
{
	[TestFixture]
    internal class CustomerInformationFileWriterTest : MRF_CIF_WriterTestBase
    {

		private IXMLCustomerReport GetCustomerReport(string fileName,
			out IDeclarationInputDataProvider dataProvider)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			dataProvider = _xmlReader.CreateDeclaration(fileName);

			var arch = dataProvider.JobInputData.Vehicle.ArchitectureID;

			dataProvider.JobInputData.Vehicle.VehicleCategory.GetVehicleType(); // HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.Vehicle.Components?.ElectricMachines?.Entries)?.Count(electric =>
				electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.Vehicle.Components?.IEPC != null);
			var report = _cifFactory.GetCustomerReport(
				dataProvider.JobInputData.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				dataProvider.JobInputData.Vehicle.ArchitectureID,
				dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);
			return report;
		}

		private IXMLCustomerReport GetCompletedBusCustomerReport(string fileName,
	out IMultistepBusInputDataProvider dataProvider)
		{
			Assert.IsFalse(string.IsNullOrEmpty(fileName));
			var tmp = _xmlReader.CreateDeclaration(fileName);
			dataProvider = tmp as IMultistepBusInputDataProvider;

			var arch = dataProvider.JobInputData.PrimaryVehicle.Vehicle.ArchitectureID;

			dataProvider.JobInputData.PrimaryVehicle.Vehicle.VehicleCategory.GetVehicleType(); // HEV/PEV - Sx/Px
			var ihpc = (dataProvider.JobInputData.PrimaryVehicle.Vehicle.Components?.ElectricMachines?.Entries)?.Count(electric =>
				electric.ElectricMachine.IHPCType != "None") > 0;
			var iepc = (dataProvider.JobInputData.PrimaryVehicle.Vehicle.Components?.IEPC != null);
			var report = _cifFactory.GetCustomerReport(
				dataProvider.JobInputData.ConsolidateManufacturingStage.Vehicle.VehicleCategory,
				dataProvider.JobInputData.JobType,
				arch,
				dataProvider.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle,
				iepc,
				ihpc);
			return report;
		}


		[TestCase(ConventionalHeavyLorry)]
		public void ConventionalLorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as ConventionalLorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		private VectoRunData GetRunData(IDeclarationInputDataProvider dataProvider)
		{
			var multistage = dataProvider as IMultistepBusInputDataProvider;
			return new VectoRunData() {
				InputData = dataProvider,
				Exempted = multistage != null ? multistage.JobInputData.PrimaryVehicle.Vehicle.ExemptedVehicle : dataProvider.JobInputData.Vehicle.ExemptedVehicle,
				VehicleData = new VehicleData() {
					OffVehicleCharging = false,
					//VehicleCategory = multistage != null ? VehicleCategory.HeavyBusCompletedVehicle : dataProvider.JobInputData.Vehicle.VehicleCategory,
				}
			};
		}

		[TestCase(HEV_Px_HeavyLorry)]
		public void HEV_Px_IHPC_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as HEV_PxLorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_S2_HeavyLorry)]
		public void HEV_S2_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as HEV_S2_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport)); Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}


		[TestCase(HEV_S3_HeavyLorry)]
		public void HEV_S3_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as HEV_S3_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}



		[TestCase(HEV_S4_HeavyLorry)]
		public void HEV_S4_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as HEV_S4_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(HEV_IEPC_S_HeavyLorry)]
		public void HEV_IEPC_S_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as HEV_IEPC_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_E2_HeavyLorry)]
		public void PEV_E2_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as PEV_E2_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(PEV_E3_HeavyLorry)]
		public void PEV_E3_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as PEV_E3_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}
		[TestCase(PEV_E4_HeavyLorry)]
		public void PEV_E4_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as PEV_E4_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}
        [TestCase(PEV_IEPC_HeavyLorry)]
        public void PEV_IEPC_LorryCIFTest(string fileName)
        {
            var report = GetCustomerReport(fileName, out var dataProvider) as PEV_IEPC_Lorry_CIF;
            Assert.NotNull(report);
            report.Initialize(GetRunData(dataProvider));

			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

		[TestCase(Exempted_HeavyLorry)]
		public void Exempted_LorryCIFTest(string fileName)
		{
			var report = GetCustomerReport(fileName, out var dataProvider) as Exempted_Lorry_CIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}



		[TestCase(Conventional_CompletedBus)]
		public void Conventional_CompletedBus_CIFTest(string fileName)
		{
			var report = GetCompletedBusCustomerReport(fileName, out var dataProvider) as Conventional_CompletedBusCIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}



		[TestCase(Exempted_CompletedBus)]
		public void Exempted_CompletedBus_CIFTest(string fileName)
		{
			var report = GetCompletedBusCustomerReport(fileName, out var dataProvider) as Exempted_CompletedBusCIF;
			Assert.NotNull(report);
			report.Initialize(GetRunData(dataProvider));
			report.GenerateReport(XMLHelper.CreateDummySig(XNamespace.Get("http://www.w3.org/2000/09/xmldsig#")));

			Assert.IsTrue(ValidateAndPrint(report.Report, XmlDocumentType.CustomerReport));
			Assert.IsTrue(WriteToDisk(outputBasePath, TestContext.CurrentContext.Test.MethodName, report.Report));
		}

	}
}
