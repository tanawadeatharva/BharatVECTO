using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;

namespace TUGraz.VectoCore.Tests.XML
{

	[TestFixture]
	class XMLPrimaryVehicleReportBusReaderTest
	{

		private const string vehilcePIFExample =
			"TestData/XML/XMLReaderDeclaration/SchemaVersion2.6_Buses/PIF-heavyBus-sample.xml";

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
		public void TestPrimaryVehicleBusData()
		{

			var reader = XmlReader.Create(vehilcePIFExample);
			var inputDataProvider = xmlInputReader.Create(reader) as IPrimaryVehicleInformationInputDataProvider;

			var vehicle = inputDataProvider.Vehicle;

			Assert.AreEqual("Generic Truck Manufacturer", vehicle.Manufacturer);
			Assert.AreEqual("Street, ZIP City", vehicle.ManufacturerAddress);
			Assert.AreEqual("Generic Model", vehicle.Model);
			Assert.AreEqual("VEH-1234567890", vehicle.VIN);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), vehicle.Date);
			Assert.IsTrue(VehicleCategory.HeavyBusPrimaryVehicle == vehicle.VehicleCategory);
			Assert.IsTrue(AxleConfiguration.AxleConfig_4x2 == vehicle.AxleConfiguration);
			Assert.AreEqual(false, vehicle.Articulated);
			Assert.AreEqual(25000, vehicle.GrossVehicleMassRating.Value());
			Assert.AreEqual(600, vehicle.EngineIdleSpeed.Value());
			Assert.AreEqual("Transmission Output Retarder", vehicle.Components.RetarderInputData.Type.ToXMLFormat());
			Assert.AreEqual(1.000, vehicle.Components.RetarderInputData.Ratio);
			Assert.AreEqual("None", vehicle.Components.AngledriveInputData.Type.ToXMLFormat());
			Assert.IsFalse(vehicle.ZeroEmissionVehicle);

			Assert.IsFalse(vehicle.ADAS.EngineStopStart);
			Assert.IsTrue(EcoRollType.None == vehicle.ADAS.EcoRoll);
			Assert.IsTrue(PredictiveCruiseControlType.None == vehicle.ADAS.PredictiveCruiseControl);

			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.AreEqual(3, vehicle.TorqueLimits.Count);
			Assert.AreEqual(6, vehicle.TorqueLimits[0].Gear);
			Assert.AreEqual(1800, vehicle.TorqueLimits[0].MaxTorque.Value());
			Assert.AreEqual(1, vehicle.TorqueLimits[1].Gear);
			Assert.AreEqual(2500, vehicle.TorqueLimits[1].MaxTorque.Value());
			Assert.AreEqual(12, vehicle.TorqueLimits[2].Gear);
			Assert.AreEqual(1900, vehicle.TorqueLimits[2].MaxTorque.Value());

			var components = inputDataProvider.Vehicle.Components;

			TestEngineDataPIFType(components.EngineInputData);

			TestTransmissionDataPIFType(components.GearboxInputData);

			Assert.IsNull(components.TorqueConverterInputData);

			TestAngledrive(components.AngledriveInputData);

			TestRetarderInputData(components.RetarderInputData);

			TestAxlegear(components.AxleGearInputData);

			TestAxleWheels(components.AxleWheels);

			TestAuxiliaries(components.BusAuxiliaries);

			TestPneumaticSystem(components.BusAuxiliaries.PneumaticSupply,
				components.BusAuxiliaries.PneumaticConsumers);

			TestHVAC(components.BusAuxiliaries.HVACAux);

			TestResultDataSignature(inputDataProvider.ManufacturerRecordHash);

			TestResultData(inputDataProvider.ResultsInputData);

			TestApplicationInformation(inputDataProvider.ApplicationInformation);

			//TestSignature(inputDataProvider.ManufacturerHash);
		}

		
		private void TestEngineDataPIFType(IEngineDeclarationInputData engine)
		{
			Assert.IsNotNull(engine);
			Assert.AreEqual("Generic Engine Manufacturer", engine.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Engine", engine.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", engine.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-15T11:00:00Z").ToUniversalTime(), engine.Date);
			Assert.AreEqual("VectoEngine x.y", engine.AppVersion);
			Assert.AreEqual(12730.SI(Unit.SI.Cubic.Centi.Meter).Cast<CubicMeter>(), engine.Displacement);//12730
			Assert.AreEqual(2200, engine.RatedSpeedDeclared.AsRPM);
			Assert.AreEqual(380000, engine.RatedPowerDeclared.Value());
			Assert.AreEqual(2400, engine.MaxTorqueDeclared.Value());

			Assert.IsTrue(engine.WHRType == WHRType.None);

			Assert.IsNotNull(engine.EngineModes);
			Assert.AreEqual(1, engine.EngineModes.Count);
			Assert.AreEqual(560, engine.EngineModes[0].IdleSpeed.AsRPM);

			var loadCurve = engine.EngineModes[0].FullLoadCurve;
			Assert.AreEqual(10, loadCurve.Rows.Count);
			Assert.AreEqual(3, loadCurve.Columns.Count);
			Assert.AreEqual(FullLoadCurveReader.Fields.EngineSpeed, loadCurve.Columns[0].Caption);
			Assert.AreEqual(FullLoadCurveReader.Fields.TorqueFullLoad, loadCurve.Columns[1].Caption);
			Assert.AreEqual(FullLoadCurveReader.Fields.TorqueDrag, loadCurve.Columns[2].Caption);

			var startRow = 0;
			Assert.IsTrue(CheckLoadCurveEntry("560.00", "1180.00", "-149.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("600.00", "1282.00", "-148.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("800.00", "1791.00", "-149.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("1000.00", "2300.00", "-160.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("1200.00", "2300.00", "-179.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("1400.00", "2300.00", "-203.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("1600.00", "2079.00", "-235.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("1800.00", "1857.00", "-264.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("2000.00", "1352.00", "-301.00", loadCurve, ref startRow));
			Assert.IsTrue(CheckLoadCurveEntry("2100.00", "1100.00", "-320.00", loadCurve, ref startRow));

			Assert.AreEqual(1, engine.EngineModes[0].Fuels.Count);
			Assert.AreEqual(FuelType.DieselCI, engine.EngineModes[0].Fuels[0].FuelType);
		}
		
		private void TestTransmissionDataPIFType(IGearboxDeclarationInputData transmission)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", transmission.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", transmission.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, transmission.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", transmission.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), transmission.Date);
			Assert.AreEqual("3.0.1", transmission.AppVersion);
			Assert.AreEqual(GearboxType.AMT, transmission.Type);

			var gears = transmission.Gears;
			Assert.IsNotNull(gears);
			Assert.AreEqual(12, gears.Count);

			var currentGearEntry = 0;

			Assert.IsTrue(CheckGearEntry(14.930, 1900, 2000, 1, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(11.640, 1900, 2000, 2, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(9.020, null, 2000, 3, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(7.040, null, 2000, 4, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(5.640, null, 2000, 5, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(4.400, null, 2000, 6, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(3.390, null, 2000, 7, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(2.650, null, 2000, 8, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(2.050, null, 2000, 9, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(1.600, null, 2000, 10, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(1.280, null, 2000, 11, gears, ref currentGearEntry));
			Assert.IsTrue(CheckGearEntry(1.000, null, null, 12, gears, ref currentGearEntry));
		}

		private void TestAngledrive(IAngledriveInputData angeldrive)
		{
			Assert.IsNotNull(angeldrive);
			Assert.AreEqual("Generic Gearbox Manufacturer", angeldrive.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", angeldrive.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, angeldrive.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", angeldrive.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), angeldrive.Date);
			Assert.AreEqual("3.0.1", angeldrive.AppVersion);
			Assert.AreEqual(2.345, angeldrive.Ratio);

			Assert.IsNull(angeldrive.LossMap);
			Assert.That(() => angeldrive.Efficiency, Throws.TypeOf<VectoException>());
		}

		private void TestRetarderInputData(IRetarderInputData retarder)
		{
			Assert.IsNotNull(retarder);
			Assert.IsNull(retarder.LossMap);
			Assert.AreEqual("Transmission Output Retarder", retarder.Type.ToXMLFormat());
			Assert.AreEqual(1.000, retarder.Ratio);
		}
		
		private void TestAxlegear(IAxleGearInputData axelGear)
		{
			Assert.AreEqual("Generic Gearbox Manufacturer", axelGear.Manufacturer);
			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axelGear.Model);
			Assert.AreEqual(CertificationMethod.StandardValues, axelGear.CertificationMethod);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", axelGear.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T11:00:00Z").ToUniversalTime(), axelGear.Date);
			Assert.AreEqual("3.0.1", axelGear.AppVersion);
			Assert.AreEqual(AxleLineType.SinglePortalAxle, axelGear.LineType);
			Assert.AreEqual(2.590, axelGear.Ratio);

			Assert.IsNull(axelGear.LossMap);
			Assert.That(() => axelGear.Efficiency, Throws.TypeOf<VectoException>());
		}

		private void TestAxleWheels(IAxlesDeclarationInputData axles)
		{
			Assert.AreEqual(2, axles.AxlesDeclaration.Count);
			var axle1 = axles.AxlesDeclaration[0];
			var axle2 = axles.AxlesDeclaration[1];

			Assert.AreEqual(AxleType.VehicleNonDriven, axle1.AxleType);
			Assert.AreEqual(false, axle1.TwinTyres);
			//Data Id ??!?

			var tyre1 = axle1.Tyre;
			Assert.AreEqual("Generic Wheels Manufacturer", tyre1.Manufacturer);
			Assert.AreEqual("Generic Wheel", tyre1.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre1.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T14:00:00Z").ToUniversalTime(), tyre1.Date);
			Assert.AreEqual("Tyre Generation App 1.0", tyre1.AppVersion);
			Assert.AreEqual("315/70 R22.5", tyre1.Dimension);
			Assert.AreEqual(0.0055, tyre1.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre1.TyreTestLoad.Value());//85% of the maximum tyre payload

			Assert.AreEqual("#WHL-5432198760-315-70-R22.5", tyre1.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", tyre1.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", tyre1.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", tyre1.DigestValue.DigestMethod);
			Assert.AreEqual("4TkUGQTX8tevHOU9Cj9uyCFuI/aqcEYlo/gyVjVQmv0=", tyre1.DigestValue.DigestValue);

			var tyre2 = axle2.Tyre;
			Assert.AreEqual("Generic Wheels Manufacturer", tyre2.Manufacturer);
			Assert.AreEqual("Generic Wheel", tyre2.Model);
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre2.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-11T14:00:00Z").ToUniversalTime(), tyre2.Date);
			Assert.AreEqual("Tyre Generation App 1.0", tyre2.AppVersion);
			Assert.AreEqual("315/70 R22.5", tyre2.Dimension);
			Assert.AreEqual(0.0063, tyre2.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre2.TyreTestLoad.Value());

			Assert.AreEqual("#WHL-5432198760-315-70-R22.5", tyre2.DigestValue.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", tyre2.DigestValue.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", tyre2.DigestValue.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", tyre2.DigestValue.DigestMethod);
			Assert.AreEqual("KljvtvGUUQ/L7MiLVAqU+bckL5PNDNNwdeLH9kUVrfM=", tyre2.DigestValue.DigestValue);
		}

		private void TestAuxiliaries(IBusAuxiliariesDeclarationData auxiliaries)
		{
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", auxiliaries.FanTechnology);
			Assert.AreEqual(1, auxiliaries.SteeringPumpTechnology.Count);
			Assert.AreEqual("Variable displacement elec. controlled", auxiliaries.SteeringPumpTechnology[0]);
			
			Assert.AreEqual(1, auxiliaries.ElectricSupply.Alternators.Count);
			Assert.AreEqual("default", auxiliaries.ElectricSupply.Alternators[0].Technology);
			Assert.AreEqual(false, auxiliaries.ElectricSupply.SmartElectrics);
		}

		private void TestPneumaticSystem(IPneumaticSupplyDeclarationData supply,
			IPneumaticConsumersDeclarationData consumers)
		{
			Assert.AreEqual("Small", supply.CompressorSize);
			Assert.AreEqual("none", supply.Clutch);
			Assert.AreEqual(1.000, supply.Ratio);
			Assert.AreEqual(false, supply.SmartAirCompression);
			Assert.AreEqual(false, supply.SmartRegeneration);

			Assert.AreEqual(ConsumerTechnology.Pneumatically, consumers.AdBlueDosing);
			Assert.AreEqual(ConsumerTechnology.Mechanically, consumers.AirsuspensionControl);
			//Assert.AreEqual(ConsumerTechnology.Pneumatically, consumers.DoorDriveTechnology);
		}
		
		private void TestHVAC(IHVACBusAuxiliariesDeclarationData hvac)
		{
			Assert.AreEqual(true, hvac.AdjustableCoolantThermostat);
			Assert.AreEqual(true, hvac.EngineWasteGasHeatExchanger);
		}
		
		private void TestResultDataSignature(DigestData resultDataSignature)
		{
			Assert.AreEqual("#MRF-VEH-1234567890", resultDataSignature.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", resultDataSignature.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", resultDataSignature.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", resultDataSignature.DigestMethod);
			Assert.AreEqual("4TkUGQTX8tevHOU9Cj9uyCFuI/aqcEYlo/gyVjVQmv0=", resultDataSignature.DigestValue);
		}

		private void TestResultData(IResultsInputData resultsInputData)
		{
			Assert.AreEqual("success", resultsInputData.Status);
			Assert.AreEqual(4, resultsInputData.Results.Count);

			var result = resultsInputData.Results[0];
			Assert.AreEqual("success", result.ResultStatus);
			Assert.AreEqual("P31SD", result.VehicleGroup);
			Assert.AreEqual("Regional Delivery", result.Mission);

			TestSimulationParameter(8810, 920, 20, "single fuel mode", result.SimulationParameter);

			result = resultsInputData.Results[1];
			Assert.AreEqual("success", result.ResultStatus);
			Assert.AreEqual("P31SD", result.VehicleGroup);
			Assert.AreEqual("Regional Delivery", result.Mission);

			TestSimulationParameter(12490, 4600, 80, "single fuel mode", result.SimulationParameter);

			result = resultsInputData.Results[2];
			Assert.AreEqual("success", result.ResultStatus);
			Assert.AreEqual("P31DD", result.VehicleGroup);
			Assert.AreEqual("Urban Delivery", result.Mission);

			TestSimulationParameter(8810, 920, 20, "single fuel mode", result.SimulationParameter);

			result = resultsInputData.Results[3];
			Assert.AreEqual("success", result.ResultStatus);
			Assert.AreEqual("P31DD", result.VehicleGroup);
			Assert.AreEqual("Urban Delivery", result.Mission);

			TestSimulationParameter(12490, 4600, 80, "single fuel mode", result.SimulationParameter);
		}

		private void TestApplicationInformation(IApplicationInformation applicationInformation)
		{
			Assert.AreEqual("Sample File Generator", applicationInformation.SimulationToolVersion);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), applicationInformation.Date);
		}

		private void TestSignature(DigestData manufacturerSignature)
		{
			Assert.AreEqual("#PIFHB-VEH-1234567890", manufacturerSignature.Reference);
			Assert.AreEqual("urn:vecto:xml:2017:canonicalization", manufacturerSignature.CanonicalizationMethods[0]);
			Assert.AreEqual("http://www.w3.org/2001/10/xml-exc-c14n#", manufacturerSignature.CanonicalizationMethods[1]);
			Assert.AreEqual("http://www.w3.org/2001/04/xmlenc#sha256", manufacturerSignature.DigestMethod);
			Assert.AreEqual("4TkUGQTX8tevHOU9Cj9uyCFuI/aqcEYlo/gyVjVQmv0=", manufacturerSignature.DigestValue);
		}

		private void TestSimulationParameter(double totalVehicleMass, double payload, int passengerCount,
			string fuelMode, ISimulationParameter simulationParameter)
		{
			Assert.AreEqual(totalVehicleMass.SI<Kilogram>(), simulationParameter.TotalVehicleMass);
			Assert.AreEqual(payload.SI<Kilogram>(), simulationParameter.Payload);
			Assert.AreEqual(passengerCount, simulationParameter.PassengerCount);
			Assert.AreEqual(fuelMode, simulationParameter.FuelMode);
		}
		
		private bool CheckLoadCurveEntry(string engineSpeed, string maxTorque, string dragTorque, TableData loadCurve, ref int currentRow)
		{
			var result = engineSpeed == loadCurve.Rows[currentRow][0].ToString() &&
						 maxTorque == loadCurve.Rows[currentRow][1].ToString() &&
						 dragTorque == loadCurve.Rows[currentRow][2].ToString();

			currentRow++;
			return result;
		}
		

		private bool CheckGearEntry(double? ratio, double? maxTorque, double? maxSpeed, int gearNumber, IList<ITransmissionInputData> gears, ref int currentEntry)
		{
			var currentMaxTorque =  maxTorque?.SI<NewtonMeter>();
			var currentMaxSpeed = maxSpeed?.RPMtoRad();

			var result = gears[currentEntry].Gear == gearNumber &&
						 gears[currentEntry].Ratio == ratio &&
						 gears[currentEntry].MaxTorque == currentMaxTorque &&
						 gears[currentEntry].MaxInputSpeed == currentMaxSpeed;
			
			currentEntry++;
			return result;
		}
	}
}
