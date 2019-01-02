using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class ExemptedVehicleTest
	{
		const string ExemptedVehicle = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(ExemptedVehicle, 1)]
		public void TestSimulationExemptedVehicle(string filename, int numRuns)
		{
			var writer = new FileOutputWriter(filename);

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var inputData = new XMLDeclarationInputDataProvider(filename, true); //.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(numRuns, runs.Count);
			foreach (var run in runs) {
				jobContainer.AddRun(run);
			}
			//jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));

			Assert.IsTrue(File.Exists(manufactuerFile));
			Assert.IsTrue(File.Exists(customerFile));

			var validator = new XMLValidator(XmlReader.Create(manufactuerFile));
			Assert.IsTrue(validator.ValidateXML(XMLValidator.XmlDocumentType.ManufacturerReport));

			var val2 = new XMLValidator(XmlReader.Create(customerFile));
			Assert.IsTrue(val2.ValidateXML(XMLValidator.XmlDocumentType.CustomerReport));

			var val3 = new XMLValidator(XmlReader.Create(customerFile));
			Assert.IsTrue(val3.ValidateXML(XMLValidator.XmlDocumentType.MonitoringReport));

		}

		[TestCase(ExemptedVehicle, true, true, true, "Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!"),
			TestCase(ExemptedVehicle, true, false, true, "Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!"),
			TestCase(ExemptedVehicle, false, false, false, "Invalid input: at least one option of ZE-HDV, He-HDV, and DualFuelVehicle has to be set for an exempted vehicle!")]
		public void TestInvalidExemptedCombination(string filename, bool zeroEmission, bool hybrid, bool dualFuel, string exMsg)
		{
			var writer = new FileOutputWriter(filename);

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			SetExemptedParameters(nav, zeroEmission, hybrid, dualFuel);

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputData = new XMLDeclarationInputDataProvider(modified, true);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			AssertHelper.Exception<VectoException>(
				() => {
					jobContainer.Runs[0].Run.Run();
				},
				messageContains: exMsg);
			Assert.IsFalse(File.Exists(customerFile));
			Assert.IsFalse(File.Exists(manufactuerFile));
			Assert.IsFalse(File.Exists(monitoringFile));
		}



		[TestCase(ExemptedVehicle, null, 10000),
		TestCase(ExemptedVehicle, 100000, null),
		TestCase(ExemptedVehicle, null, null)]
		public void TestHybridExemptedRequiresMaxNetPower(string filename, double? maxNetPower1, double? maxNetPower2)
		{
			var writer = new FileOutputWriter(filename);

			var customerFile = writer.XMLCustomerReportName;
			var manufactuerFile = writer.XMLFullReportName;
			var monitoringFile = writer.XMLMonitoringReportName;
			if (File.Exists(customerFile)) {
				File.Delete(customerFile);
			}
			if (File.Exists(manufactuerFile)) {
				File.Delete(manufactuerFile);
			}
			if (File.Exists(monitoringFile)) {
				File.Delete(monitoringFile);
			}

			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			SetExemptedParameters(nav, false, true, false);
			var maxPower1 = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_MaxNetPower1),
												manager);
			if (maxNetPower1.HasValue) {
				maxPower1.SetValue(maxNetPower1.Value.ToXMLFormat(0));
			} else {
				maxPower1.DeleteSelf();
			}
			var maxPower2 = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_MaxNetPower2),
												manager);
			if (maxNetPower2.HasValue) {
				maxPower2.SetValue(maxNetPower2.Value.ToXMLFormat(0));
			} else {
				maxPower2.DeleteSelf();
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputData = new XMLDeclarationInputDataProvider(modified, true);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			AssertHelper.Exception<VectoException>(
				() => {
					jobContainer.Runs[0].Run.Run();
				},
				messageContains: "For He-HDV both MaxNetPower1 and MaxNetPower2 have to be provided!");
			Assert.IsFalse(File.Exists(customerFile));
			Assert.IsFalse(File.Exists(manufactuerFile));
			Assert.IsFalse(File.Exists(monitoringFile));
		}


		private static void SetExemptedParameters(XPathNavigator nav, bool zeroEmission, bool hybrid, bool dualFuel)
		{
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var zeNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_ZeroEmissionVehicle),
				manager);
			zeNode.SetValue(zeroEmission.ToString().ToLowerInvariant());

			var dualfuelNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_DualFuelVehicle),
				manager);
			dualfuelNode.SetValue(dualFuel.ToString().ToLowerInvariant());

			var hybridNode = nav.SelectSingleNode(
				helper.QueryAbs(
					helper.NSPrefix(
						XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_HybridElectricHDV),
				manager);
			hybridNode.SetValue(hybrid.ToString().ToLowerInvariant());
		}
	}
}
