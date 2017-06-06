using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Integration
{
	[TestClass]
	public class TestVehicleIdlingSpeed
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";

		[TestMethod]
		public void VehicleIdlingSpeedTest()
		{
			var VehicleEngineIdleSpeed = 900.RPMtoRad();

			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var idlingSpeed = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_IdlingSpeed), manager);
			idlingSpeed.SetValue(VehicleEngineIdleSpeed.AsRPM.ToXMLFormat(0));

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, new FileOutputWriter("Idle900"));
			//factory.WriteModalResults = true;

			var jobContainer = new JobContainer(null);
			jobContainer.AddRuns(factory);

			var runIdx = 0;
			var run = jobContainer.Runs[runIdx];
			var modContainer = (ModalDataContainer)run.Run.GetContainer().ModalData;
			var modData = modContainer.Data;

			run.Run.Run();

			modContainer.Data = modData;
			Assert.IsTrue(modContainer.Min<PerSecond>(ModalResultField.n_eng_avg).IsGreaterOrEqual(VehicleEngineIdleSpeed));


			//jobContainer.Execute();
			//jobContainer.WaitFinished();
		}
	}
}