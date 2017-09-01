using System.IO;
using System.Linq;
using System.Xml;
using NUnit.Framework;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace TUGraz.VectoCore.Tests.Models.Simulation
{
	[TestFixture]
	public class DeclarationSimulationFactoryTest
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";

		[TestCase("None", RetarderType.None),
		TestCase("Losses included in Gearbox", RetarderType.LossesIncludedInTransmission),
		TestCase("Engine Retarder", RetarderType.EngineRetarder),
		TestCase("Transmission Input Retarder", RetarderType.TransmissionInputRetarder),
		TestCase("Transmission Output Retarder", RetarderType.TransmissionOutputRetarder)]
		public void TestRetarderTypes(string xmlValue, RetarderType retarderType)
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderType), manager);
			xmlRetarderType.SetValue(xmlValue);

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, null) { Validate = false };

			var runs = factory.SimulationRuns().ToArray();
			Assert.AreEqual(8, runs.Length);
		}
	}
}
