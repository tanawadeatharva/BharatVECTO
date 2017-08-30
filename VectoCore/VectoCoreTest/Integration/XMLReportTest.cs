using System.IO;
using System.Xml;
using System.Xml.XPath;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.OutputData.XML;


namespace TUGraz.VectoCore.Tests.Integration
{
	[TestFixture]
	public class XMLReportTest
	{
		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestXMLReportMetaInformation()
		{
			var jobfile = @"Testdata\XML\XMLReaderDeclaration\vecto_vehicle-sample.xml";
			var dataProvider = new XMLDeclarationInputDataProvider(XmlReader.Create(jobfile), true);
			var writer = new FileOutputWriter(jobfile);
			var xmlReport = new XMLDeclarationReport(writer);
			var sumData = new SummaryDataContainer(writer);
			var jobContainer = new JobContainer(sumData);

			if (File.Exists(writer.SumFileName)) {
				File.Delete(writer.SumFileName);
			}

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, writer, xmlReport) {
				WriteModalResults = false,
				Validate = false,
			};
			jobContainer.AddRuns(runsFactory);
			jobContainer.Execute();
			jobContainer.WaitFinished();

			var manufacturerReport = xmlReport.FullReport;

			Assert.AreEqual(5, manufacturerReport.XPathSelectElement("//*[local-name()='VehicleGroup']").Value.ToInt());
		}
	}
}
