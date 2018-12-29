using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration
{
	public class NaturalGasVehicles
	{
		const string Class5NG = @"Testdata\Integration\DeclarationMode\Class5_NG\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[
		 TestCase(Class5NG, 2, TankSystem.Liquefied, 253.7, 702.8, TestName = "Class5 LNG 2"),
		 TestCase(Class5NG, 2, TankSystem.Compressed, 259.5, 698.1, TestName = "Class5 CNG 2"),
		TestCase(Class5NG, 6, TankSystem.Liquefied, 252.8, 700.4, TestName = "Class5 LNG 6"),
		TestCase(Class5NG, 6, TankSystem.Compressed, 258.6, 695.7, TestName = "Class5 CNG 6"),
			]
		public void NaturalGasTankSystemTest(string filename, int runIdx, TankSystem tankSystem, double expectedFc, double expectedCo2)
		{
			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			XNamespace ns = Constants.XML.VectoDeclarationDefinitionsNS;
			var tankSystemNode = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_NgTankSystem),
												manager);
			tankSystemNode.SetValue(tankSystem.ToString());
			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var writer = new MockDeclarationWriter(filename);
			var inputData = new XMLDeclarationInputDataProvider(modified, true); //.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			jobContainer.Execute();
			jobContainer.WaitFinished();
			var manufacturerReport = writer.GetReport(ReportType.DeclarationReportManufacturerXML);

			var fuelTypeResultNodes = manufacturerReport.XPathSelectElements("//*[local-name()='Results']//*[local-name()='FuelType']");
			foreach (var node in fuelTypeResultNodes) {
				Assert.AreEqual(tankSystem == TankSystem.Liquefied ? "LNG PI" : "CNG PI", node.Value);
			}

			var fcNode = manufacturerReport.XPathSelectElement(
				string.Format("//*[local-name()='Results']/*[local-name()='Result'][{0}]//*[local-name()='FuelConsumption' and @unit='g/km']", runIdx));
			var co2Node = manufacturerReport.XPathSelectElement(
				string.Format("//*[local-name()='Results']/*[local-name()='Result'][{0}]//*[local-name()='CO2' and @unit='g/km']", runIdx));

			Console.WriteLine("fc: {0}  co2: {1}", fcNode.Value, co2Node.Value);

			Assert.AreEqual(expectedFc, fcNode.Value.ToDouble(), 0.1);
			Assert.AreEqual(expectedCo2, co2Node.Value.ToDouble(), 0.1);
		}
	}

	public class MockDeclarationWriter : IOutputDataWriter
	{
		public DataTable SumData { get; private set; }

		private readonly Dictionary<ReportType, XDocument> _reports = new Dictionary<ReportType, XDocument>();

		public MockDeclarationWriter(string filename)
		{
			
		}

		public XDocument GetReport(ReportType type)
		{
			return _reports[type];
		}

		#region Implementation of IModalDataWriter

		public void WriteModData(int jobRunId, string runName, string cycleName, string runSuffix, DataTable modData)
		{
			
		}

		#endregion

		#region Implementation of IReportWriter

		public void WriteReport(ReportType type, XDocument data)
		{
			_reports[type] = data;
		}

		public void WriteReport(ReportType type, Stream data)
		{
		}

		#endregion

		#region Implementation of ISummaryWriter

		public void WriteSumData(DataTable sortedAndFilteredTable)
		{
			SumData = sortedAndFilteredTable;
		}

		#endregion
	}
}
