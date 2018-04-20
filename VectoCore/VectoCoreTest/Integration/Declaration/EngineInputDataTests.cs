using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration
{
	[TestFixture]
	public class EngineInputDataTests
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase(null, 1.0, 45.5323),
		TestCase("CFRegPer", 1.2, 45.5323 * 1.2),
		TestCase("BFColdHot", 1.2, 45.5323 * 1.2),
		TestCase("CFNCV", 1.2, 45.5323)  // has no influence - only for documentation purpose
			]
		public void TestEngineCorrectionFactors(string correctionFactor, double value, double expectedFc)
		{
			var reader = XmlReader.Create(SampleVehicleDecl);
			var modified = reader;
			if (correctionFactor != null) {
				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var firstAxle = nav.SelectSingleNode("//"+
					helper.Query(
						//helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
						correctionFactor
					), manager);
				firstAxle.InnerXml = value.ToXMLFormat(4);

				 modified = XmlReader.Create(new StringReader(nav.OuterXml));

			}
			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null, validate: false);
			var first = factory.SimulationRuns().First();

			var modData = ((ModalDataContainer)first.GetContainer().ModalData).Data;
			first.Run();

			Assert.AreEqual(expectedFc, modData.AsEnumerable().Sum(r => r.Field<SI>((int)ModalResultField.FCFinal).Value()), 1e-3);

		}
	}
}
