using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.Simulation;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration.Declaration
{
	public class ADASVehicleTest
	{
		const string Class5ADAS = @"Testdata\Integration\DeclarationMode\Class5_ADAS\Tractor_4x2_vehicle-class-5_EURO6_2018.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}


		[TestCase(Class5ADAS, 1, false, false, false, PredictiveCruiseControlType.Option_1_2_3, 1 - 0.7/100.0),
		 TestCase(Class5ADAS, 5, true, true, false, PredictiveCruiseControlType.Option_1_2_3, 1 - 1.3 / 100.0)
			]
		public void ADASCorrectionFactorTest(string filename, int runIdx, bool engineStopStart, bool ecoRollWithoutEngineStop, bool ecoRollWithEngineStop, PredictiveCruiseControlType pcc, double expectedCorrectionFactor)
		{
			var reader = XmlReader.Create(filename);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			XNamespace ns = Constants.XML.VectoDeclarationDefinitionsNS;
			
			var newAdas = new XElement(ns + XMLNames.Vehicle_ADAS, 
				new XElement(ns + XMLNames.Vehicle_ADAS_EngineStopStart, engineStopStart),
				new XElement(ns + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, ecoRollWithoutEngineStop),
				new XElement(ns + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, ecoRollWithEngineStop),
				new XElement(ns + XMLNames.Vehicle_ADAS_PCC, pcc.ToXMLFormat()));
			var adas = nav.SelectSingleNode(helper.QueryAbs(
													helper.NSPrefix(XMLNames.VectoInputDeclaration,
																	Constants.XML.RootNSPrefix),
													XMLNames.Component_Vehicle,
													XMLNames.Vehicle_ADAS),
												manager);
			adas.ReplaceSelf(newAdas.ToString());//  .SetValue(fuel);
			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var writer = new FileOutputWriter(filename);
			var inputData = new XMLDeclarationInputDataProvider(modified, true); //.ReadJsonJob(relativeJobPath);
			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputData, writer) {
				WriteModalResults = true,
				ActualModalData = true
			};
			var jobContainer = new JobContainer(new MockSumWriter());

			jobContainer.AddRuns(factory);

			var run = jobContainer.Runs[runIdx].Run;
			var modContainer = ((ModalDataContainer)run.GetContainer().ModalData);
			var modData = modContainer.Data;
			run.Run();
			modContainer.Data = modData;
			var cf = modContainer.FuelConsumptionADASPerSecond() / modContainer.FuelConsumptionWHTCPerSecond();

			Assert.AreEqual(expectedCorrectionFactor, cf.Value(), 1e-6);

		}
	}
}
