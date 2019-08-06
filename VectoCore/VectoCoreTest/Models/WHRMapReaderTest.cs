using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.XPath;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models
{
	[TestFixture]
	public class WHRMapTest
	{
		public const string SingleFuelWHRVehicle = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.3\vehicle_sampleSingleModeSingleFuel_WHR.xml";

		public const string DualFuelWHRVehicle = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.3\vehicle_sampleSingleModeDualFuel_WHR.xml";

		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		[TestCase]
		public void TestWHRMapCSVData()
		{

			var data = "engine speed, torque, fuel consumption, whr power \n" +
						"600, -100, 0, 100\n" +
						"600, 500, 200, 400\n" +
						"2400, -100, 0, 100\n" +
						"2400, 500, 200, 400";

			var whrMap = WHRPowerReader.Create(VectoCSVFile.ReadStream(data.ToStream()));
			var result = whrMap.GetWHRPower(500.SI<NewtonMeter>(), 600.RPMtoRad(), true);

			Assert.IsFalse(result.Extrapolated);
			Assert.AreEqual(400, result.ElectricPower.Value());
		}

		[TestCase()]
		public void ReadXMLSingleFuelEngWithWHR()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);
			var inputDataProvider = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());

			var runs = dao.NextRun().ToArray();
			Assert.AreEqual(10, runs.Length);

			Assert.IsTrue(runs.All(x => x.EngineData.WHRData?.WHRMap != null));
		}


		[TestCase()]
		public void ReadXMLSingleFuelEngWHRWithoutCorrectionFactors()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrCfNode = nav.SelectSingleNode(".//*[local-name()='WHRCorrectionFactors']");
			whrCfNode.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			var runs = dao.NextRun().ToArray();

			Assert.IsTrue(runs.All(x => x.EngineData.WHRData == null));
		}

		[TestCase()]
		public void ReadXMLSingleFuelEngWHRMissingWHRPower()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");
			while (whrPwrEntries.MoveNext()) { 
				whrPwrEntries.Current.DeleteSelf();
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);

			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			var runs = dao.NextRun().ToArray();

			Assert.IsTrue(runs.All(x => x.EngineData.WHRData == null));
		}

		[TestCase()]
		public void ReadXMLSingleFuelEngWHRMissingWHREntries()
		{
			var reader = XmlReader.Create(SingleFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");

			for (var i = 0; i < 4; i++) {
				whrPwrEntries.MoveNext();
			}
			whrPwrEntries.Current.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			AssertHelper.Exception<VectoException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHRData has to be provided for every entry in the FC-Map! n: 560.00, T: 400.00");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngWithWHR()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);
			var inputDataProvider = xmlInputReader.Create(reader) as IDeclarationInputDataProvider;
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());

			var runs = dao.NextRun().ToArray();
			Assert.AreEqual(10, runs.Length);

			Assert.IsTrue(runs.All(x => x.EngineData.WHRData?.WHRMap != null));
		}

		[TestCase()]
		public void ReadXMLDualFuelEngWHRCorrectionFactorsBothFuels()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrCfNode = nav.SelectSingleNode(".//*[local-name()='WHRCorrectionFactors']");
			var fuel2 = nav.SelectSingleNode(".//*[local-name()='Fuel'][2]/*[local-name()='FuelConsumptionMap']");
			fuel2.InsertBefore(whrCfNode.Clone());

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			AssertHelper.Exception<VectoException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHRData (correction factors) can only be defined for one fuel!");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngWHRPowerBothFuels()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var fuel2 = nav.Select(".//*[local-name()='Fuel'][2]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']");
			while (fuel2.MoveNext()) {
				fuel2.Current.CreateAttribute("", "electricPower", "", "200.00");
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			AssertHelper.Exception<VectoException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHRData (electric power) can only be defined for one fuel!");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineWHRCorrectionAndMapForDifferentFuel()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var fuel1Entries = nav.Select(".//*[local-name()='Fuel'][1]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");
			while (fuel1Entries.MoveNext()) {
				fuel1Entries.Current.DeleteSelf();
			}

			var fuel2Entries = nav.Select(".//*[local-name()='Fuel'][2]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']");
			while (fuel2Entries.MoveNext()) {
				fuel2Entries.Current.CreateAttribute("", "electricPower", "", "200.00");
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			AssertHelper.Exception<VectoException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "Correction Factors and WHR-Map have to be defined for the same fuel!");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineWHRMissingWHREntries()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='Fuel'][1]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");

			for (var i = 0; i < 4; i++) {
				whrPwrEntries.MoveNext();
			}
			whrPwrEntries.Current.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			AssertHelper.Exception<VectoException>(
				() => {
					var runs = dao.NextRun().ToArray();
				}, "WHRData has to be provided for every entry in the FC-Map! n: 560.00, T: 400.00");
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineNoWHRCorrectionFactors()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrCfNode = nav.SelectSingleNode(".//*[local-name()='WHRCorrectionFactors']");
			whrCfNode.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			var runs = dao.NextRun().ToArray();

			Assert.IsTrue(runs.All(x => x.EngineData.WHRData == null));
		}

		[TestCase()]
		public void ReadXMLDualFuelEngineNoWHRPowerEntries()
		{
			var reader = XmlReader.Create(DualFuelWHRVehicle);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();

			var whrPwrEntries = nav.Select(".//*[local-name()='Fuel'][1]/*[local-name()='FuelConsumptionMap']/*[local-name()='Entry']/@electricPower");

			while (whrPwrEntries.MoveNext()) {
				whrPwrEntries.Current.DeleteSelf();
			}

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var inputDataProvider = xmlInputReader.CreateDeclaration(modified);
			var dao = new DeclarationModeVectoRunDataFactory(inputDataProvider, new NullDeclarationReport());
			var runs = dao.NextRun().ToArray();

			Assert.IsTrue(runs.All(x => x.EngineData.WHRData == null));
		}
	}
}
