using System.IO;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationInputv210VehicleDataTest
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;


		private const string Optional_TESTS_DIR =
			@"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\WithoutOptionalEntries";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private IXMLDeclarationVehicleData ReadVehicleData(string jobfile, string testDir)
		{
			var filename = Path.Combine(testDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			return (IXMLDeclarationVehicleData)dataProvider.JobInputData.Vehicle;
		}


		[TestCase(@"Conventional_heavyLorry_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalHeavyLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.NotNull(vehicle);
			Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(6000.SI<Kilogram>(), vehicle.CurbMassChassis);
			Assert.AreEqual(12000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			Assert.AreEqual(RetarderType.None, vehicle.RetarderType);
			Assert.AreEqual(2.000, vehicle.RetarderRatio);
			Assert.AreEqual(AngledriveType.None, vehicle.AngledriveType);
			Assert.IsNotNull(vehicle.PTOTransmissionInputData);
			Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(false, vehicle.VocationalVehicle);
			Assert.IsNull(vehicle.TankSystem);
			Assert.AreEqual(false, vehicle.SleeperCab);
			Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
		}

		[TestCase(@"Conventional_mediumLorry_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalMediumLorryVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.Tractor, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(3500.SI<Kilogram>(), vehicle.CurbMassChassis);
			Assert.AreEqual(7100.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			Assert.AreEqual(RetarderType.None, vehicle.RetarderType);
			Assert.AreEqual(10.000, vehicle.RetarderRatio);
			Assert.AreEqual(AngledriveType.None, vehicle.AngledriveType);
			Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
			Assert.IsNull(vehicle.TankSystem);
			Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
		}

		[TestCase(@"Conventional_primaryBus_AMT_n_opt.xml", Optional_TESTS_DIR)]
		public void TestConventionalPrimaryBusVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.NotNull(vehicle);
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(false, vehicle.Articulated);
			Assert.AreEqual(25000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
			Assert.AreEqual(600.RPMtoRad(), vehicle.EngineIdleSpeed);
			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.RetarderType);
			Assert.AreEqual(1.000, vehicle.RetarderRatio);
			Assert.AreEqual(AngledriveType.None, vehicle.AngledriveType);
			Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
		}


		[TestCase(@"HEV_heavyLorry_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVHeavyLorryPxVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.NotNull(vehicle);
			Assert.AreEqual(LegislativeClass.N3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.RigidTruck, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(6000.SI<Kilogram>(), vehicle.CurbMassChassis);
			Assert.AreEqual(12000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			Assert.AreEqual(RetarderType.None, vehicle.RetarderType);
			Assert.AreEqual(12.000, vehicle.RetarderRatio);
			Assert.AreEqual(AngledriveType.None, vehicle.AngledriveType);
			Assert.IsNotNull(vehicle.PTOTransmissionInputData);
			Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(false, vehicle.VocationalVehicle);
			Assert.IsNull(vehicle.TankSystem);
			Assert.AreEqual(false, vehicle.SleeperCab);
			Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
			Assert.AreEqual(ArchitectureID.P2_5, vehicle.ArchitectureID);
			Assert.AreEqual(false, vehicle.OvcHev);
			Assert.AreEqual(11.SI<Watt>(), vehicle.MaxChargingPower);
		}


		[TestCase(@"HEV_mediumLorry_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]
		public void TestHEVMediumLorryPxVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.NotNull(vehicle);
			Assert.AreEqual(LegislativeClass.N2, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.Van, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(6000.SI<Kilogram>(), vehicle.CurbMassChassis);
			Assert.AreEqual(12000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
			Assert.AreEqual(650.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			Assert.AreEqual(RetarderType.None, vehicle.RetarderType);
			Assert.AreEqual(13.000, vehicle.RetarderRatio);
			Assert.AreEqual(AngledriveType.None, vehicle.AngledriveType);
			Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual("ASDF", vehicle.VehicleTypeApprovalNumber);
			Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
			Assert.AreEqual(true, vehicle.OvcHev);
			Assert.AreEqual(12.SI<Watt>(), vehicle.MaxChargingPower);
		}


		[TestCase(@"HEV_primaryBus_AMT_Px_n_opt.xml", Optional_TESTS_DIR)]

		public void TestHEVPrimaryBusPxVehicleData(string jobfile, string testDir)
		{
			var vehicle = ReadVehicleData(jobfile, testDir);

			Assert.NotNull(vehicle);
			Assert.AreEqual(LegislativeClass.M3, vehicle.LegislativeClass);
			Assert.AreEqual(VehicleCategory.HeavyBusPrimaryVehicle, vehicle.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicle.AxleConfiguration);
			Assert.AreEqual(true, vehicle.Articulated);
			Assert.AreEqual(25000.SI<Kilogram>(), vehicle.GrossVehicleMassRating);
			Assert.AreEqual(600.00.RPMtoRad(), vehicle.EngineIdleSpeed);
			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, vehicle.RetarderType);
			Assert.AreEqual(1.000, vehicle.RetarderRatio);
			Assert.AreEqual(AngledriveType.SeparateAngledrive, vehicle.AngledriveType);
			Assert.AreEqual(false, vehicle.ZeroEmissionVehicle);
			Assert.AreEqual(ArchitectureID.P2, vehicle.ArchitectureID);
			Assert.AreEqual(true, vehicle.OvcHev);
			Assert.AreEqual(5.SI<Watt>(), vehicle.MaxChargingPower);
		}
	}
}
