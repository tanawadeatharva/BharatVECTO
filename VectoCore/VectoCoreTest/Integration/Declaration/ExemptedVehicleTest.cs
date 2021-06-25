/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration.Declaration;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using XmlDocumentType = TUGraz.VectoCore.Utils.XmlDocumentType;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	[Parallelizable(ParallelScope.All)]
	public class ExemptedVehicleTest
	{
		const string ExemptedVehicle = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted.xml";
		const string ExemptedVehicleNoHEV = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted_nonHEV.xml";
		const string ExemptedVehicleV2 = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted_v2.xml";
		const string ExemptedVehicleV2NoHEV = @"Testdata\Integration\DeclarationMode\ExemptedVehicle\vecto_vehicle-sample_exempted_v2_nonHEV.xml";

		private const string ExemptedMin = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted.xml";
		private const string ExemptedAxl = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_axl.xml";
		private const string ExemptedAxlSleeperT = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_axl+sleeperT.xml";
		private const string ExemptedAxlSleeperF = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_axl+sleeperF.xml";
		private const string ExemptedSleeperT = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_sleeperT.xml";
		private const string ExemptedSleeperF = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_sleeperF.xml";
		private const string ExemptedPEVMaxNetPower = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_PEV.xml";
		private const string ExemptedPEVMin = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_PEV_2.xml";

		private const string ExemptedMin_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1.xml";

		private const string ExemptedAxl_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_axl.xml";
		private const string ExemptedAxlSleeperT_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_axl+SleeperT.xml";
		private const string ExemptedAxlSleeperF_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_axl+SleeperF.xml";
		private const string ExemptedSleeperT_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_sleeperT.xml";
		private const string ExemptedSleeperF_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_sleeperF.xml";
		private const string ExemptedPEVMaxNetPower_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_PEV.xml";
		private const string ExemptedPEVMin_v2 = @"TestData\Integration\DeclarationMode\ExemptedVehicle\exempted_v2.2.1_PEV_2.xml";



		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase(ExemptedVehicle, 1),
		TestCase(ExemptedVehicleNoHEV, 1),
		TestCase(ExemptedVehicleV2, 1),
		TestCase(ExemptedVehicleV2NoHEV, 1)
		]
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

			var inputData = xmlInputReader.CreateDeclaration(filename);
			
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
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport));

			var val2 = new XMLValidator(XmlReader.Create(customerFile));
			Assert.IsTrue(val2.ValidateXML(XmlDocumentType.CustomerReport));

			var val3 = new XMLValidator(XmlReader.Create(monitoringFile));
			Assert.IsTrue(val3.ValidateXML(XmlDocumentType.MonitoringReport));

		}

		[TestCase(ExemptedVehicle, true, true, true, "Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!"),
			TestCase(ExemptedVehicle, true, false, true, "Invalid input: ZE-HDV and DualFuelVehicle are mutually exclusive!"),
			TestCase(ExemptedVehicle, false, false, false, "Invalid input: at least one option of ZE-HDV, He-HDV, and DualFuelVehicle has to be set for an exempted vehicle!")]
		public void TestInvalidExemptedCombination(string filename, bool zeroEmission, bool hybrid, bool dualFuel, string exMsg)
		{
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(filename));

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

			var inputData = xmlInputReader.CreateDeclaration(modified);
			
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
			var writer = new FileOutputWriter(InputDataHelper.GetRandomFilename(filename));

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

			var inputData = xmlInputReader.CreateDeclaration(modified);
			
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



		[
		TestCase(ExemptedMin, null, null, null, null, false),
		TestCase(ExemptedAxl, AxleConfiguration.AxleConfig_4x2, null, 30000, 20000, false),
		TestCase(ExemptedAxlSleeperT, AxleConfiguration.AxleConfig_4x2, true, 30000, 20000, false),
		TestCase(ExemptedAxlSleeperF, AxleConfiguration.AxleConfig_4x2, false, 30000, 20000, false),
		TestCase(ExemptedSleeperT, null, true, 30000, 20000, false),
		TestCase(ExemptedSleeperF, null, false, 30000, 20000, false),
		TestCase(ExemptedPEVMaxNetPower, AxleConfiguration.AxleConfig_4x2, true, 30000, 20000, true),
		TestCase(ExemptedPEVMin, null, null, null, null, true),

		TestCase(ExemptedMin_v2, null, null, null, null, false),
		TestCase(ExemptedAxl_v2, AxleConfiguration.AxleConfig_4x2, null, 30000, 20000, false),
		TestCase(ExemptedAxlSleeperT_v2, AxleConfiguration.AxleConfig_4x2, true, 30000, 20000, false),
		TestCase(ExemptedAxlSleeperF_v2, AxleConfiguration.AxleConfig_4x2, false, 30000, 20000, false),
		TestCase(ExemptedSleeperT_v2, null, true, 30000, 20000, false),
		TestCase(ExemptedSleeperF_v2, null, false, 30000, 20000, false),
		TestCase(ExemptedPEVMaxNetPower_v2, AxleConfiguration.AxleConfig_4x2, true, 30000, 20000, true),
		TestCase(ExemptedPEVMin_v2, null, null, null, null, true),
		]
		public void TestExemptedVehiclesAxleConfSleeperCabMRF(string filename, AxleConfiguration? expectedMrfAxleConf,
			bool? expectedMrfSleeperCab, double? expectedMaxNetPower1, double? expectedMaxNetPower2, bool zeHDV)
		{
			var writer = new MockDeclarationWriter(filename);


			var inputData = xmlInputReader.CreateDeclaration(filename);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			var runs = factory.SimulationRuns().ToList();
			Assert.AreEqual(1, runs.Count);
			foreach (var run in runs) {
				jobContainer.AddRun(run);
			}
			//jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var progress = jobContainer.GetProgress();
			Assert.IsTrue(progress.All(r => r.Value.Success), string.Concat<Exception>(progress.Select(r => r.Value.Error)));


			var validator = new XMLValidator(writer.GetReport(ReportType.DeclarationReportManufacturerXML).CreateReader());
			Assert.IsTrue(validator.ValidateXML(XmlDocumentType.ManufacturerReport));

			var val2 = new XMLValidator(writer.GetReport(ReportType.DeclarationReportCustomerXML).CreateReader());
			Assert.IsTrue(val2.ValidateXML(XmlDocumentType.CustomerReport));

			var mrf = writer.GetReport(ReportType.DeclarationReportManufacturerXML).Document;
			Assert.NotNull(mrf);

			var axleConfNode = mrf.XPathSelectElements(XMLHelper.QueryLocalName(XMLNames.Vehicle_AxleConfiguration))
				.ToArray();
			if (expectedMrfAxleConf == null) {
				Assert.AreEqual(0, axleConfNode.Length);
			} else {
				Assert.AreEqual(1, axleConfNode.Length, "axleconfiguration missing in mrf");
				Assert.AreEqual(expectedMrfAxleConf.Value, AxleConfigurationHelper.Parse(axleConfNode.First().Value), "axleconfiguration: incorrect value");
			}

			var sleeperCabNode = mrf.XPathSelectElements(XMLHelper.QueryLocalName(XMLNames.Vehicle_SleeperCab))
				.ToArray();
			if (expectedMrfSleeperCab == null) {
				Assert.AreEqual(0, sleeperCabNode.Length);
			} else {
				Assert.AreEqual(1, sleeperCabNode.Length);
				Assert.AreEqual(expectedMrfSleeperCab.Value, XmlConvert.ToBoolean(sleeperCabNode.First().Value));
			}

			var maxNetPower1Node = mrf.XPathSelectElements(XMLHelper.QueryLocalName(XMLNames.Vehicle_MaxNetPower1))
				.ToArray();
			if (expectedMaxNetPower1 == null) {
				Assert.AreEqual(0, maxNetPower1Node.Length);
			} else {
				Assert.AreEqual(1, maxNetPower1Node.Length);
				Assert.AreEqual(expectedMaxNetPower1.Value, maxNetPower1Node.First().Value.ToDouble());
			}

			var maxNetPower2Node = mrf.XPathSelectElements(XMLHelper.QueryLocalName(XMLNames.Vehicle_MaxNetPower2))
				.ToArray();
			if (expectedMaxNetPower2 == null) {
				Assert.AreEqual(0, maxNetPower2Node.Length);
			} else {
				Assert.AreEqual(1, maxNetPower2Node.Length);
				Assert.AreEqual(expectedMaxNetPower2.Value, maxNetPower2Node.First().Value.ToDouble());
			}

			var zeNode = mrf.XPathSelectElement(XMLHelper.QueryLocalName(XMLNames.Vehicle_ZeroEmissionVehicle));
			Assert.NotNull(zeNode);
			Assert.AreEqual(zeHDV, XmlConvert.ToBoolean(zeNode.Value));
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
