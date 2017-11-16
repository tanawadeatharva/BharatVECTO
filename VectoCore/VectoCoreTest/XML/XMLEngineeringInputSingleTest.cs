/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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

using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering;
using TUGraz.VectoCore.InputData.Reader;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using NUnit.Framework;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLEngineeringInputSingleTest
	{
		public const string EngineeringSampleFile = "TestData/XML/XMLReaderEngineering/engineering_job-sample.xml";

		public const string EngineeringSampleFileFull = "TestData/XML/XMLReaderEngineering/engineering_job-sample_FULL.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestXMLInputEngSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var engineDataProvider = inputDataProvider.JobInputData.Vehicle.EngineInputData;

			Assert.IsFalse(engineDataProvider.SavedInDeclarationMode);

			Assert.AreEqual("Generic 40t Long Haul Truck Engine", engineDataProvider.Model);
			Assert.AreEqual(0.012730, engineDataProvider.Displacement.Value());
			Assert.AreEqual(0.77, engineDataProvider.Inertia.Value());

			AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.WHTCMotorway; });
			AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.WHTCRural; });
			AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.WHTCUrban; });

			var fcMapTable = engineDataProvider.FuelConsumptionMap;
			Assert.AreEqual(112, fcMapTable.Rows.Count);
			Assert.AreEqual("engine speed", fcMapTable.Columns[0].Caption);
			Assert.AreEqual("torque", fcMapTable.Columns[1].Caption);
			Assert.AreEqual("fuel consumption", fcMapTable.Columns[2].Caption);

			Assert.AreEqual("560.00", fcMapTable.Rows[0][0]);
			var fcMap = FuelConsumptionMapReader.Create(fcMapTable);
            Assert.AreEqual(1256.SI(Unit.SI.Gramm.Per.Hour).Value(), 
				fcMap.GetFuelConsumption(0.SI<NewtonMeter>(), 560.RPMtoRad()).Value.Value());

			var fldTable = engineDataProvider.FullLoadCurve;
			Assert.AreEqual(10, fldTable.Rows.Count);
			Assert.AreEqual("engine speed", fldTable.Columns[0].Caption);
			Assert.AreEqual("full load torque", fldTable.Columns[1].Caption);
			Assert.AreEqual("motoring torque", fldTable.Columns[2].Caption);
			var fldMap = FullLoadCurveReader.Create(fldTable, true);
		}

		[TestCase]
		public void TestXMLInputGbxSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.GearboxInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck Gearbox", gearboxDataProvider.Model);
			Assert.AreEqual(GearboxType.AMT, gearboxDataProvider.Type);
			var gears = gearboxDataProvider.Gears;
			Assert.AreEqual(12, gears.Count);

			Assert.AreEqual(1, gears.First().Gear);
			Assert.AreEqual(14.93, gears.First().Ratio);
			Assert.AreEqual("0.00", gears.First().LossMap.Rows[0][0]);
			Assert.AreEqual("-350.00", gears.First().LossMap.Rows[0][1]);
			Assert.AreEqual("12.06", gears.First().LossMap.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(gears.First().LossMap, gears.First().Ratio,
				gears.First().Gear.ToString());

			//Assert.AreEqual("560", gears.First().FullLoadCurve.Rows[0][0]);
			//Assert.AreEqual("2500", gears.First().FullLoadCurve.Rows[0][1]);

			//var fldMap = FullLoadCurveReader.Create(gears.First().FullLoadCurve, true);
		}


		[TestCase]
		public void TestXMLInputAxlGSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);
			var axlegearDataProvider = inputDataProvider.JobInputData.Vehicle.AxleGearInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axlegearDataProvider.Model);

			var lossMapData = axlegearDataProvider.LossMap;
			Assert.AreEqual(2.59, axlegearDataProvider.Ratio);
			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("-5000.00", lossMapData.Rows[0][1]);
			Assert.AreEqual("115.00", lossMapData.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(lossMapData, axlegearDataProvider.Ratio, "AxleGear");
		}

		[TestCase]
		public void TestXMLInputAxlGSingleFileEfficiency()
		{
			var reader = XmlReader.Create(EngineeringSampleFile);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var axleglosses = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Axlegear,
				XMLNames.ComponentDataWrapper, XMLNames.Axlegear_TorqueLossMap), manager);
			//accData.DeleteSelf();
			axleglosses.ReplaceSelf(
				new XElement(XMLNames.Axlegear_TorqueLossMap, new XElement(XMLNames.Axlegear_Efficiency, "0.9123")).ToString());

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);

			var axleGear = inputDataProvider.JobInputData.Vehicle.AxleGearInputData;
			Assert.AreEqual(0.9123, axleGear.Efficiency);
		}

		[TestCase]
		public void TestXMLInputRetarderSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);
			var retarderDataProvider = inputDataProvider.JobInputData.Vehicle.RetarderInputData;

			Assert.AreEqual("Generic Retarder", retarderDataProvider.Model);

			var lossMapData = retarderDataProvider.LossMap;

			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, retarderDataProvider.Type);

			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("10.00", lossMapData.Rows[0][1]);

			var lossMap = RetarderLossMapReader.Create(lossMapData);
		}

		[TestCase]
		public void TestXMLInputAxleWheelsSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);
			var vehicleDataProvider = inputDataProvider.VehicleInputData;

			var axles = vehicleDataProvider.Axles;

			Assert.AreEqual("315/70 R22.5", axles[0].Wheels);
			Assert.AreEqual(0.0055, axles[0].RollResistanceCoefficient);
			Assert.AreEqual(31300, axles[0].TyreTestLoad.Value());

			Assert.AreEqual("315/70 R22.5", axles[1].Wheels);
			Assert.AreEqual(0.0063, axles[1].RollResistanceCoefficient);
			Assert.AreEqual(31300, axles[1].TyreTestLoad.Value());

			//AssertHelper.Exception<VectoException>(() => { var tmp = vehicleDataProvider.Rim; });
			Assert.AreEqual(0.488822, vehicleDataProvider.DynamicTyreRadius.Value());
		}

		[TestCase]
		public void TestXMLInputAxleWheelsDuplicates()
		{
			var reader = XmlReader.Create(EngineeringSampleFile);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
				), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(2);


			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);

			var vehicleDataProvider = inputDataProvider.VehicleInputData;

			AssertHelper.Exception<VectoException>(() => { var axles = vehicleDataProvider.Axles; });
		}

		[TestCase]
		public void TestXMLInputAxleWheelsAxleNumTooLow()
		{
			var reader = XmlReader.Create(EngineeringSampleFile);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
				), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(0);

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			AssertHelper.Exception<VectoException>(
				() => {
					var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);
					var axles = inputDataProvider.VehicleInputData.Axles;
				});
		}

		[TestCase]
		public void TestXMLInputAxleWheelsAxleNumTooHigh()
		{
			var reader = XmlReader.Create(EngineeringSampleFile);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
				), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(3);


			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			AssertHelper.Exception<VectoException>(
				() => {
					var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);
					var axles = inputDataProvider.VehicleInputData.Axles;
				});
		}

		[TestCase]
		public void TestXMLInputAuxiliariesSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);
			var auxDataProvider = inputDataProvider.JobInputData.Vehicle.AuxiliaryInputData();

			var aux = auxDataProvider.Auxiliaries;
			var aux1 = aux[0];

			Assert.AreEqual("ES", aux1.ID);

			Assert.AreEqual(70, aux1.DemandMap.Rows[0].ParseDouble(AuxiliaryDataReader.Fields.MechPower));
			Assert.AreEqual(640, aux1.DemandMap.Rows[2].ParseDouble(AuxiliaryDataReader.Fields.SupplyPower));

			var aux2 = aux[1];

			Assert.AreEqual("FAN", aux2.ID);
		}

		[TestCase]
		public void TestXMLInputADASSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var adas = inputDataProvider.DriverInputData;

			Assert.AreEqual(DriverMode.Overspeed, adas.OverSpeedEcoRoll.Mode);
		}

		[TestCase]
		public void TestVehicleInputSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var vehicleDataProvider = inputDataProvider.VehicleInputData;

			Assert.AreEqual(VehicleCategory.Tractor, vehicleDataProvider.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleDataProvider.AxleConfiguration);

			Assert.AreEqual(7100.0, vehicleDataProvider.CurbMassChassis.Value());
			Assert.AreEqual(40000.0, vehicleDataProvider.GrossVehicleMassRating.Value());
			Assert.AreEqual(6.29, inputDataProvider.JobInputData.Vehicle.AirdragInputData.AirDragArea.Value());

			Assert.AreEqual(1500, vehicleDataProvider.Loading.Value());
			Assert.AreEqual(500, vehicleDataProvider.CurbMassExtra.Value());

			Assert.AreEqual(1.0, inputDataProvider.JobInputData.Vehicle.RetarderInputData.Ratio);
		}

		[TestCase]
		public void TestXMEngineering_DriverModel()
		{
			var reader = File.OpenRead(EngineeringSampleFile);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var driverDataProvider = inputDataProvider.DriverInputData;

			var lac = driverDataProvider.Lookahead;
			Assert.IsTrue(lac.Enabled);
			Assert.AreEqual(DeclarationData.Driver.LookAhead.MinimumSpeed.AsKmph, lac.MinSpeed.AsKmph, 1e-6);

			var overspeed = driverDataProvider.OverSpeedEcoRoll;
			Assert.AreEqual(DriverMode.Overspeed, overspeed.Mode);
			Assert.AreEqual(50, overspeed.MinSpeed.AsKmph, 1e-6);
			Assert.AreEqual(5, overspeed.OverSpeed.AsKmph, 1e-6);
			Assert.AreEqual(5, overspeed.UnderSpeed.AsKmph, 1e-6);

			var driverAcc = driverDataProvider.AccelerationCurve;
			Assert.AreEqual(2, driverAcc.Rows.Count);
			Assert.AreEqual("100", driverAcc.Rows[1][0]);
			Assert.AreEqual("1", driverAcc.Rows[1][1]);
			Assert.AreEqual("-1", driverAcc.Rows[1][2]);

			var declarationDriverDataProvider = (IDriverDeclarationInputData)inputDataProvider.DriverInputData;


			var shiftStrategy = inputDataProvider.JobInputData.Vehicle.GearboxInputData;

			Assert.AreEqual(DeclarationData.Gearbox.UpshiftMinAcceleration.Value(), shiftStrategy.UpshiftMinAcceleration.Value(),
				1e-6);
			Assert.AreEqual(DeclarationData.Gearbox.DownshiftAfterUpshiftDelay.Value(),
				shiftStrategy.DownshiftAfterUpshiftDelay.Value(), 1e-6);
			Assert.AreEqual(DeclarationData.Gearbox.UpshiftAfterDownshiftDelay.Value(),
				shiftStrategy.UpshiftAfterDownshiftDelay.Value(), 1e-6);

			Assert.AreEqual(DeclarationData.Gearbox.TorqueReserve, shiftStrategy.TorqueReserve, 1e-6);
			Assert.AreEqual(DeclarationData.Gearbox.MinTimeBetweenGearshifts.Value(),
				shiftStrategy.MinTimeBetweenGearshift.Value(), 1e-6);
			Assert.AreEqual(DeclarationData.Gearbox.StartSpeed.Value(), shiftStrategy.StartSpeed.Value(), 1e-6);
			Assert.AreEqual(DeclarationData.Gearbox.StartAcceleration.Value(), shiftStrategy.StartAcceleration.Value(), 1e-6);
			Assert.AreEqual(DeclarationData.Gearbox.TorqueReserveStart, shiftStrategy.StartTorqueReserve, 1e-6);

			AssertHelper.AreRelativeEqual(Constants.DefaultPowerShiftTime, shiftStrategy.PowershiftShiftTime);

			var tcShiftStrategy = inputDataProvider.JobInputData.Vehicle.GearboxInputData.TorqueConverter;

			AssertHelper.AreRelativeEqual(DeclarationData.TorqueConverter.CCUpshiftMinAcceleration,
				tcShiftStrategy.CCUpshiftMinAcceleration);
			AssertHelper.AreRelativeEqual(DeclarationData.TorqueConverter.CLUpshiftMinAcceleration,
				tcShiftStrategy.CLUpshiftMinAcceleration);
		}

		[TestCase]
		public void TestXMEngineering_DriverModelNoAcc()
		{
			var reader = XmlReader.Create(EngineeringSampleFile);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var accData = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_DriverModel,
				XMLNames.DriverModel_DriverAccelerationCurve), manager);
			accData.DeleteSelf();

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);

			var driverDataProvider = inputDataProvider.DriverInputData;

			var driverAcc = driverDataProvider.AccelerationCurve;
			Assert.AreEqual("TUGraz.VectoCore.Resources.Declaration.VACC.Truck.vacc", driverAcc.Source);
			Assert.AreEqual(5, driverAcc.Rows.Count);
		}


		[TestCase]
		public void TestXMLPowertrainGenerationSingleFile()
		{
			var fileWriter = new FileOutputWriter("foo");
			var sumWriter = new FileOutputWriter("vecto_vehicle-sample_xml");
			var sumData = new SummaryDataContainer(sumWriter);
			var jobContainer = new JobContainer(sumData);
			var dataProvider = new XMLEngineeringInputDataProvider(EngineeringSampleFile, true);

			var runsFactory = new SimulatorFactory(ExecutionMode.Engineering, dataProvider, fileWriter);
			runsFactory.WriteModalResults = true;

			jobContainer.AddRuns(runsFactory);

			//Assert.AreEqual(6, jobContainer.Runs.Count);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_TorqueConverter()
		{
			var reader = File.OpenRead(EngineeringSampleFileFull);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var tcDataProvider = inputDataProvider.JobInputData.Vehicle.TorqueConverterInputData;

			Assert.AreEqual(1000, tcDataProvider.ReferenceRPM.AsRPM, 1e-6);
			Assert.AreEqual(1.1, tcDataProvider.Inertia.Value());

			Assert.AreEqual(3, tcDataProvider.TCData.Rows.Count);
			Assert.AreEqual("300.00", tcDataProvider.TCData.Rows[0][2]);
			Assert.AreEqual("0.90", tcDataProvider.TCData.Rows[2][1]);

			Assert.IsNotNull(tcDataProvider.ShiftPolygon);
			Assert.AreEqual("700", tcDataProvider.ShiftPolygon.Rows[0][1]);
			Assert.AreEqual("800", tcDataProvider.ShiftPolygon.Rows[1][2]);

			Assert.AreEqual(1700, tcDataProvider.MaxInputSpeed.AsRPM, 1e-6);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_AngleDrive()
		{
			var reader = File.OpenRead(EngineeringSampleFileFull);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var angledriveDataProvider = inputDataProvider.JobInputData.Vehicle.AngledriveInputData;

			Assert.AreEqual(1.2, angledriveDataProvider.Ratio);
			Assert.AreEqual(6, angledriveDataProvider.LossMap.Rows.Count);
			Assert.AreEqual("-10000.00", angledriveDataProvider.LossMap.Rows[0][1]);
			Assert.AreEqual("100.00", angledriveDataProvider.LossMap.Rows[4][2]);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_DriverModel()
		{
			var reader = File.OpenRead(EngineeringSampleFileFull);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var driverDataProvider = inputDataProvider.DriverInputData;

			var lac = driverDataProvider.Lookahead;
			Assert.IsTrue(lac.Enabled);
			Assert.AreEqual(60, lac.MinSpeed.AsKmph, 1e-6);
			Assert.AreEqual(2, lac.CoastingDecisionFactorTargetSpeedLookup.Rows.Count);
			Assert.AreEqual("100", lac.CoastingDecisionFactorTargetSpeedLookup.Rows[1][0]);
			Assert.AreEqual("2", lac.CoastingDecisionFactorTargetSpeedLookup.Rows[1][1]);

			Assert.AreEqual(2, lac.CoastingDecisionFactorVelocityDropLookup.Rows.Count);
			Assert.AreEqual("100", lac.CoastingDecisionFactorVelocityDropLookup.Rows[1][0]);
			Assert.AreEqual("1", lac.CoastingDecisionFactorVelocityDropLookup.Rows[1][1]);

			var overspeed = driverDataProvider.OverSpeedEcoRoll;
			Assert.AreEqual(DriverMode.Overspeed, overspeed.Mode);
			Assert.AreEqual(50, overspeed.MinSpeed.AsKmph, 1e-6);
			Assert.AreEqual(5, overspeed.OverSpeed.AsKmph, 1e-6);
			Assert.AreEqual(5, overspeed.UnderSpeed.AsKmph, 1e-6);

			var driverAcc = driverDataProvider.AccelerationCurve;
			Assert.AreEqual(2, driverAcc.Rows.Count);
			Assert.AreEqual("100", driverAcc.Rows[1][0]);
			Assert.AreEqual("1", driverAcc.Rows[1][1]);
			Assert.AreEqual("-1", driverAcc.Rows[1][2]);

			var shiftStrategy = inputDataProvider.JobInputData.Vehicle.GearboxInputData;

			Assert.AreEqual(0.133, shiftStrategy.UpshiftMinAcceleration.Value(), 1e-6);
			Assert.AreEqual(12, shiftStrategy.DownshiftAfterUpshiftDelay.Value(), 1e-6);
			Assert.AreEqual(13, shiftStrategy.UpshiftAfterDownshiftDelay.Value(), 1e-6);

			Assert.AreEqual(0.213, shiftStrategy.TorqueReserve, 1e-6);
			Assert.AreEqual(2.33, shiftStrategy.MinTimeBetweenGearshift.Value(), 1e-6);
			Assert.AreEqual(2.11, shiftStrategy.StartSpeed.Value(), 1e-6);
			Assert.AreEqual(0.211, shiftStrategy.StartAcceleration.Value(), 1e-6);
			Assert.AreEqual(0.212, shiftStrategy.StartTorqueReserve, 1e-6);

			Assert.AreEqual(0.811, shiftStrategy.PowershiftShiftTime.Value(), 1e-6);

			var tcShiftStrategy = inputDataProvider.JobInputData.Vehicle.GearboxInputData.TorqueConverter;

			Assert.AreEqual(0.134, tcShiftStrategy.CCUpshiftMinAcceleration.Value(), 1e-6);
			Assert.AreEqual(0.133, tcShiftStrategy.CLUpshiftMinAcceleration.Value(), 1e-6);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_CrosswindCorrection()
		{
			var reader = File.OpenRead(EngineeringSampleFileFull);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var airdragData = inputDataProvider.JobInputData.Vehicle.AirdragInputData;
			Assert.AreEqual(CrossWindCorrectionMode.SpeedDependentCorrectionFactor, airdragData.CrossWindCorrectionMode);
			Assert.AreEqual(2, airdragData.CrosswindCorrectionMap.Rows.Count);
			Assert.AreEqual("100", airdragData.CrosswindCorrectionMap.Rows[1][0]);
			Assert.AreEqual("1.8", airdragData.CrosswindCorrectionMap.Rows[1][1]);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_PTO()
		{
			var reader = File.OpenRead(EngineeringSampleFileFull);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);

			var ptoData = inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData;

			Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", ptoData.PTOTransmissionType);
			Assert.AreEqual(2, ptoData.PTOLossMap.Rows.Count);
			Assert.AreEqual("2800.00", ptoData.PTOLossMap.Rows[1][0]);
			Assert.AreEqual("100.00", ptoData.PTOLossMap.Rows[1][1]);

			Assert.AreEqual(4, ptoData.PTOCycle.Rows.Count);
			Assert.AreEqual("3", ptoData.PTOCycle.Rows[3][0]);
			Assert.AreEqual("1200.00", ptoData.PTOCycle.Rows[3][1]);
			Assert.AreEqual("100.00", ptoData.PTOCycle.Rows[3][2]);
		}


		[TestCase]
		public void TestXMLInputAngledriveGSingleFile()
		{
			var reader = File.OpenRead(EngineeringSampleFileFull);

			var inputDataProvider = new XMLEngineeringInputDataProvider(reader, true);
			var angledriveInputData = inputDataProvider.JobInputData.Vehicle.AngledriveInputData;

			Assert.AreEqual("Generic Angledrive", angledriveInputData.Model);

			var lossMapData = angledriveInputData.LossMap;
			Assert.AreEqual(1.2, angledriveInputData.Ratio);
			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("-10000.00", lossMapData.Rows[0][1]);
			Assert.AreEqual("100.00", lossMapData.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(lossMapData, angledriveInputData.Ratio, "Angledrive");
		}

		[TestCase]
		public void TestXMLInputAngledriveSingleFileEfficiency()
		{
			var reader = XmlReader.Create(EngineeringSampleFileFull);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var angledrivelosses = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Angledrive,
				XMLNames.ComponentDataWrapper, XMLNames.AngleDrive_TorqueLossMap), manager);
			//accData.DeleteSelf();
			angledrivelosses.ReplaceSelf(
				new XElement(XMLNames.AngleDrive_TorqueLossMap, new XElement(XMLNames.AngleDrive_Efficiency, "0.9124")).ToString());

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);

			var angledrive = inputDataProvider.JobInputData.Vehicle.AngledriveInputData;
			Assert.AreEqual(0.9124, angledrive.Efficiency);
		}

		[TestCase]
		public void TestXMLInputConstantAuxSingleFile()
		{
			var reader = XmlReader.Create(EngineeringSampleFileFull);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var aux = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Auxiliaries,
				XMLNames.ComponentDataWrapper), manager);
			//accData.DeleteSelf();
			//angledrivelosses.ReplaceSelf(new XElement(XMLNames.AngleDrive_Efficiency, "0.9124").ToString());
			aux.InnerXml =
				new XElement(XMLNames.Auxiliaries_Auxiliary, new XAttribute(XMLNames.Auxiliaries_Auxiliary_ID_Attr, "const"),
					new XElement(XMLNames.Auxiliaries_Auxiliary_ConstantAuxLoad, "5000")).ToString();

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);

			var auxInput = inputDataProvider.JobInputData.Vehicle.AuxiliaryInputData().Auxiliaries;

			Assert.AreEqual(1, auxInput.Count);
			Assert.AreEqual(AuxiliaryDemandType.Constant, auxInput[0].AuxiliaryType);
			Assert.AreEqual(5000, auxInput[0].ConstantPowerDemand.Value(), 1e-6);
		}

		[TestCase]
		public void TestRetarderTypeNone()
		{
			var reader = XmlReader.Create(EngineeringSampleFile);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);

			var retarderType = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderType), manager);
			retarderType.SetValue("None");

			var retarder = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Retarder), manager);
			retarder.DeleteSelf();

			//modify cycle & remove AUX to make simulation valid
			var cycle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.VectoJob_MissionCycles), manager);
			cycle.InnerXml =
				new XElement(XMLNames.Missions_Cycle,
					new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
					new XAttribute(XMLNames.ExtResource_File_Attr, "LongHaul")).ToString();
			var aux = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Auxiliaries,
				XMLNames.ComponentDataWrapper), manager);
			aux.InnerXml = "";

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);

			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputDataProvider, new FileOutputWriter("dummy"));
			var jobContainer = new JobContainer(null);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
		}

		[TestCase]
		public void TestXMLInputInvalidXML()
		{
			var reader = XmlReader.Create(EngineeringSampleFileFull);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var engine = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Engine), manager);
			//accData.DeleteSelf();
			engine.DeleteSelf();

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			AssertHelper.Exception<VectoException>(
				() => { var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true); });
		}


		[TestCase]
		public void TestXMLInputInvalidCycle()
		{
			var reader = XmlReader.Create(EngineeringSampleFileFull);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var cycles = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.VectoJob_MissionCycles), manager);
			//accData.DeleteSelf();
			cycles.InnerXml =
				new XElement(XMLNames.Missions_Cycle,
					new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
					new XAttribute(XMLNames.ExtResource_File_Attr, "invalid_cycle.vdri")).ToString();

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			AssertHelper.Exception<VectoException>(
				() => {
					var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);
					var cyclesList = inputDataProvider.JobInputData.Cycles;
				});
		}

		[TestCase]
		public void TestXMLInputInvalidDriverAcceleration()
		{
			var reader = XmlReader.Create(EngineeringSampleFileFull);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var driverAcceleration = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_DriverModel, XMLNames.DriverModel_DriverAccelerationCurve), manager);
			//accData.DeleteSelf();
			driverAcceleration.InnerXml =
				new XElement(XMLNames.ExternalResource,
					new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
					new XAttribute(XMLNames.ExtResource_File_Attr, "invalid_acceleration.vacc")).ToString();

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			AssertHelper.Exception<VectoException>(
				() => {
					var inputDataProvider = new XMLEngineeringInputDataProvider(stream, true);
					var cyclesList = inputDataProvider.DriverInputData.AccelerationCurve;
				});
		}

		[TestCase]
		public void TestXMLInputExtResourceMissingTag()
		{
			var reader = XmlReader.Create(EngineeringSampleFileFull);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Engineering);
			helper.AddNamespaces(manager);


			var axlegearLossMap = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputEngineering, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle, XMLNames.Vehicle_Components, XMLNames.Component_Axlegear,
				XMLNames.ComponentDataWrapper, XMLNames.Axlegear_TorqueLossMap), manager);
			//accData.DeleteSelf();
			axlegearLossMap.InnerXml = "";
			//InnerXml =
			//new XElement(XMLNames.ExternalResource,
			//	new XAttribute(XMLNames.ExtResource_Type_Attr, XMLNames.ExtResource_Type_Value_CSV),
			//	new XAttribute(XMLNames.ExtResource_File_Attr, "invalid_acceleration.vacc")).ToString();

			//var modified = XmlReader.Create(new StringReader(nav.OuterXml));
			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(nav.OuterXml);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);

			AssertHelper.Exception<VectoException>(
				() => {
					var inputDataProvider = new XMLEngineeringInputDataProvider(stream, false);
					var lossmap = inputDataProvider.JobInputData.Vehicle.AxleGearInputData.LossMap;
				}, "Failed to read TorqueLossMap resource");
		}
	}
}
