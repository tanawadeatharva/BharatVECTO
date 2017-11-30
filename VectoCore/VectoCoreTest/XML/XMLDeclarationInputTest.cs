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

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration;
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
	public class XMLDeclarationInputTest
	{
		const string SampleVehicleDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample.xml";
		const string SampleVehicleDeclNoAirdrag = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_noAirdrag.xml";
		const string SampleVehicleFullDecl = "TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_FULL.xml";

		const string SampleVehicleFullDeclCertificationOptions =
			"TestData/XML/XMLReaderDeclaration/vecto_vehicle-sample_certificationOptions.xml";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase]
		public void TestXMLInputEng()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);

			var engineDataProvider = inputDataProvider.JobInputData.Vehicle.EngineInputData;

			Assert.IsTrue(engineDataProvider.SavedInDeclarationMode);

			Assert.AreEqual("Generic 40t Long Haul Truck Engine", engineDataProvider.Model);
			Assert.AreEqual(0.012730, engineDataProvider.Displacement.Value());
			Assert.AreEqual(1.0097, engineDataProvider.WHTCUrban);
			//AssertHelper.Exception<VectoException>(() => { var tmp = engineDataProvider.Inertia; });

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
		public void TestXMLInputGbx()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
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

			Assert.AreEqual(5000, gears.First().MaxTorque.Value());
		}

		[TestCase]
		public void TestXMLInputAxlG()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var axlegearDataProvider = inputDataProvider.JobInputData.Vehicle.AxleGearInputData;

			Assert.AreEqual("Generic 40t Long Haul Truck AxleGear", axlegearDataProvider.Model);

			var lossMapData = axlegearDataProvider.LossMap;
			Assert.AreEqual(2.59, axlegearDataProvider.Ratio);
			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("-5000.00", lossMapData.Rows[0][1]);
			Assert.AreEqual("115.00", lossMapData.Rows[0][2]);

			var lossMap = TransmissionLossMapReader.Create(lossMapData, axlegearDataProvider.Ratio, "AxleGear");
			Assert.IsNotNull(lossMap);

			AssertHelper.Exception<VectoException>(() => { var tmp = axlegearDataProvider.Efficiency; });
		}

		[TestCase]
		public void TestXMLInputRetarder()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var retarderDataProvider = inputDataProvider.JobInputData.Vehicle.RetarderInputData;

			Assert.AreEqual("Generic Retarder", retarderDataProvider.Model);

			var lossMapData = retarderDataProvider.LossMap;

			Assert.AreEqual(RetarderType.TransmissionOutputRetarder, retarderDataProvider.Type);

			Assert.AreEqual("0.00", lossMapData.Rows[0][0]);
			Assert.AreEqual("10.00", lossMapData.Rows[0][1]);

			var lossMap = RetarderLossMapReader.Create(lossMapData);
			Assert.IsNotNull(lossMap);
		}

		[TestCase]
		public void TestXMLInputAxleWheels()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			var axles = vehicleDataProvider.Axles;

			var tyre = axles[0].Tyre;
			Assert.AreEqual("315/70 R22.5",tyre.Dimension);
			Assert.AreEqual(0.0055, tyre.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre.TyreTestLoad.Value());
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre.CertificationNumber);

			tyre = axles[1].Tyre;
			Assert.AreEqual("315/70 R22.5", tyre.Dimension);
			Assert.AreEqual(0.0063, tyre.RollResistanceCoefficient);
			Assert.AreEqual(31300, tyre.TyreTestLoad.Value());
			Assert.AreEqual("e12*0815/8051*2017/05E0000*00", tyre.CertificationNumber);
		}

		[TestCase]
		public void TestXMLInputAxleWheelsDuplicates()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
				), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(2);


			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			AssertHelper.Exception<VectoException>(() => { var axles = vehicleDataProvider.Axles; });
		}

		[TestCase]
		public void TestXMLInputAxleWheelsAxleNumTooLow()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
				), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(0);


			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			AssertHelper.Exception<VectoException>(
				() => { var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true); });
		}

		[TestCase]
		public void TestXMLInputAxleWheelsAxleNumTooHigh()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var firstAxle = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_AxleWheels,
				XMLNames.ComponentDataWrapper,
				XMLNames.AxleWheels_Axles,
				helper.QueryConstraint(XMLNames.AxleWheels_Axles_Axle, "1", null, string.Empty)
				), manager);
			firstAxle.MoveToAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, string.Empty);
			firstAxle.SetTypedValue(3);


			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			AssertHelper.Exception<VectoException>(() => { var axles = vehicleDataProvider.Axles; });
		}

		[TestCase]
		public void TestXMLInputAuxiliaries()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var auxDataProvider = inputDataProvider.JobInputData.Vehicle.AuxiliaryInputData();

			var aux = auxDataProvider.Auxiliaries;
			var aux1 = aux[0];

			Assert.AreEqual(AuxiliaryType.Fan, aux1.Type);
			Assert.AreEqual("Hydraulic driven - Constant displacement pump", aux1.Technology.First());

			var aux3 = aux[2];
			Assert.AreEqual(AuxiliaryType.ElectricSystem, aux3.Type);
			Assert.AreEqual("Standard technology - LED headlights, all", aux3.Technology.First());
		}

		
		[TestCase]
		public void TestVehicleInput()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);

			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual(VehicleCategory.Tractor, vehicleDataProvider.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleDataProvider.AxleConfiguration);

			Assert.AreEqual(7100.0, vehicleDataProvider.CurbMassChassis.Value());
			Assert.AreEqual(40000.0, vehicleDataProvider.GrossVehicleMassRating.Value());
			Assert.AreEqual(6.34, inputDataProvider.JobInputData.Vehicle.AirdragInputData.AirDragArea.Value());

			Assert.AreEqual(1.0, inputDataProvider.JobInputData.Vehicle.RetarderInputData.Ratio);
		}

		[TestCase]
		public void TestVehicleInputNoAirdrag()
		{
			var reader = XmlReader.Create(SampleVehicleDeclNoAirdrag);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);

			Assert.IsNull(inputDataProvider.JobInputData.Vehicle.AirdragInputData.AirDragArea);
		}

		[TestCase]
		public void TestXMLPowertrainGeneration()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var fileWriter = new FileOutputWriter("foo");
			var sumWriter = new FileOutputWriter("vecto_vehicle-sample_xml");
			var sumData = new SummaryDataContainer(sumWriter);
			var jobContainer = new JobContainer(sumData);
			var dataProvider = new XMLDeclarationInputDataProvider(reader, true);

			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter);
			runsFactory.WriteModalResults = true;

			jobContainer.AddRuns(runsFactory);

			Assert.AreEqual(8, jobContainer.Runs.Count);
		}

		[TestCase]
		public void TestFullFeaturedXMEngineering_TorqueConverter()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);

			var tcDataProvider = inputDataProvider.JobInputData.Vehicle.TorqueConverterInputData;


			Assert.AreEqual(3, tcDataProvider.TCData.Rows.Count);
			Assert.AreEqual("300.00", tcDataProvider.TCData.Rows[0][2]);
			Assert.AreEqual("0.90", tcDataProvider.TCData.Rows[2][1]);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_AngleDrive()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);


			var angledriveDataProvider = inputDataProvider.JobInputData.Vehicle.AngledriveInputData;

			Assert.AreEqual(2.345, angledriveDataProvider.Ratio);
			Assert.AreEqual(6, angledriveDataProvider.LossMap.Rows.Count);
			Assert.AreEqual("-10000.00", angledriveDataProvider.LossMap.Rows[0][1]);

			AssertHelper.Exception<VectoException>(() => { var tmp = angledriveDataProvider.Efficiency; });
		}

		[TestCase]
		public void TestVehicleInputData()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			Assert.AreEqual(VehicleCategory.Tractor, vehicleDataProvider.VehicleCategory);
			Assert.AreEqual(AxleConfiguration.AxleConfig_4x2, vehicleDataProvider.AxleConfiguration);

			Assert.AreEqual(7100.0, vehicleDataProvider.CurbMassChassis.Value());
			Assert.AreEqual(40000.0, vehicleDataProvider.GrossVehicleMassRating.Value());
			Assert.AreEqual(6.34, inputDataProvider.JobInputData.Vehicle.AirdragInputData.AirDragArea.Value());

			Assert.AreEqual(1.0, inputDataProvider.JobInputData.Vehicle.RetarderInputData.Ratio);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_TorqueLimits()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var vehicleDataProvider = inputDataProvider.JobInputData.Vehicle;

			var torqueLimits = vehicleDataProvider.TorqueLimits.OrderBy(x => x.Gear).ToList();
			Assert.AreEqual(3, torqueLimits.Count);
			Assert.AreEqual(1, torqueLimits[0].Gear);
			Assert.AreEqual(2500, torqueLimits[0].MaxTorque.Value());
			Assert.AreEqual(12, torqueLimits[2].Gear);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_GbxTorqueLimits()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.GearboxInputData;
			var gears = gearboxDataProvider.Gears;

			Assert.AreEqual(12, gears.Count);
			Assert.AreEqual(1900, gears[0].MaxTorque.Value());
			Assert.AreEqual(1900, gears[1].MaxTorque.Value());
			Assert.IsNull(gears[11].MaxTorque);
		}

		[TestCase]
		public void TestFullFeaturedXMLDeclaration_GbxSpeedLimits()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var gearboxDataProvider = inputDataProvider.JobInputData.Vehicle.GearboxInputData;
			var gears = gearboxDataProvider.Gears;

			Assert.AreEqual(12, gears.Count);
			Assert.AreEqual(2000, gears[0].MaxInputSpeed.AsRPM, 1e-6);
			Assert.AreEqual(2000, gears[1].MaxInputSpeed.AsRPM, 1e-6);
			Assert.IsNull(gears[11].MaxInputSpeed);
		}

		[TestCase]
		public void TestElementNotAvailable()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var retarderRatio = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderRatio), manager);
			retarderRatio.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

			AssertHelper.Exception<VectoException>(() => { var tmp = inputDataProvider.JobInputData.Vehicle.RetarderInputData.Ratio; });
		}

		[TestCase]
		public void TestRetarderTypeNone()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var doc = new XmlDocument();
			doc.Load(reader);
			var nav = doc.CreateNavigator();
			var manager = new XmlNamespaceManager(nav.NameTable);
			var helper = new XPathHelper(ExecutionMode.Declaration);
			helper.AddNamespaces(manager);

			var retarderType = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_RetarderType), manager);
			retarderType.SetValue("None");

			var retarder = nav.SelectSingleNode(helper.QueryAbs(
				helper.NSPrefix(XMLNames.VectoInputDeclaration, Constants.XML.RootNSPrefix),
				XMLNames.Component_Vehicle,
				XMLNames.Vehicle_Components,
				XMLNames.Component_Retarder), manager);
			retarder.DeleteSelf();

			var modified = XmlReader.Create(new StringReader(nav.OuterXml));

			var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

			var factory = new SimulatorFactory(ExecutionMode.Declaration, inputDataProvider, new FileOutputWriter("dummy"));
			var jobContainer = new JobContainer(null);
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
		}

		[TestCase]
		public void TestRetarderTypes()
		{
			var retarderTypes = new Dictionary<string, RetarderType>() {
				{ "None", RetarderType.None },
				{ "Losses included in Gearbox", RetarderType.LossesIncludedInTransmission },
				{ "Engine Retarder", RetarderType.EngineRetarder },
				{ "Transmission Input Retarder", RetarderType.TransmissionInputRetarder },
				{ "Transmission Output Retarder", RetarderType.TransmissionOutputRetarder }
			}
				;
			foreach (var retarderType in retarderTypes) {
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
				xmlRetarderType.SetValue(retarderType.Key);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

				Assert.AreEqual(retarderType.Value, inputDataProvider.JobInputData.Vehicle.RetarderInputData.Type);
			}
		}

		[TestCase]
		public void TestAxleConfigurationTypes()
		{
			var axleConfigurations = new Dictionary<string, AxleConfiguration>() {
				{ "4x2", AxleConfiguration.AxleConfig_4x2 },
				//{ "4x4", AxleConfiguration.AxleConfig_4x4 },
				{ "6x2", AxleConfiguration.AxleConfig_6x2 },
				{ "6x4", AxleConfiguration.AxleConfig_6x4 },
				//{ "6x6", AxleConfiguration.AxleConfig_6x6 },
				//{ "8x2", AxleConfiguration.AxleConfig_8x2 },
				{ "8x4", AxleConfiguration.AxleConfig_8x4 },
				//{ "8x6", AxleConfiguration.AxleConfig_8x6 },
				//{ "8x8", AxleConfiguration.AxleConfig_8x8 }
			};
			foreach (var axleConfiguration in axleConfigurations) {
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
					XMLNames.Vehicle_AxleConfiguration), manager);
				xmlRetarderType.SetValue(axleConfiguration.Key);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

				Assert.AreEqual(axleConfiguration.Value, inputDataProvider.JobInputData.Vehicle.AxleConfiguration);
			}
		}

		[TestCase]
		public void TestVehicleCategoryTypes()
		{
			var vehicleCategories = new Dictionary<string, VehicleCategory>() {
				{ "Rigid Truck", VehicleCategory.RigidTruck },
				{ "Tractor", VehicleCategory.Tractor },
				//{ "City Bus", VehicleCategory.CityBus },
				//{ "Interurban Bus", VehicleCategory.InterurbanBus },
				//{ "Coach", VehicleCategory.Coach }
			};
			foreach (var vehicleCategory in vehicleCategories) {
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
					XMLNames.Vehicle_VehicleCategory), manager);
				xmlRetarderType.SetValue(vehicleCategory.Key);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified, true);

				Assert.AreEqual(vehicleCategory.Value, inputDataProvider.JobInputData.Vehicle.VehicleCategory);
			}
		}


		[TestCase]
		public void TestWheelsTypes()
		{
			var retarderTypes = new Dictionary<string, RetarderType>() { };
			foreach (var retarderType in retarderTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_RetarderType),
					manager);
				xmlRetarderType.SetValue(retarderType.Key);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified,
					true);

				Assert.AreEqual(retarderType.Value,
					inputDataProvider.JobInputData.Vehicle.RetarderInputData.Type);
			}
		}

		[TestCase]
		public void TestPTOTypeTypes()
		{
			var ptoTypes = new string[][] {
				new[] { "none", "none" },
				new[] { "only one engaged gearwheel above oil level", "none" },
				new[] { "only the drive shaft of the PTO", "shift claw, synchronizer, sliding gearwheel" },
				new[] { "only the drive shaft of the PTO", "multi-disc clutch" },
				new[] { "only the drive shaft of the PTO", "multi-disc clutch, oil pump" },
				new[] { "drive shaft and/or up to 2 gear wheels", "shift claw, synchronizer, sliding gearwheel" },
				new[] { "drive shaft and/or up to 2 gear wheels", "multi-disc clutch" },
				new[] { "drive shaft and/or up to 2 gear wheels", "multi-disc clutch, oil pump" },
				new[] { "drive shaft and/or more than 2 gear wheels", "shift claw, synchronizer, sliding gearwheel" },
				new[] { "drive shaft and/or more than 2 gear wheels", "multi-disc clutch" },
				new[] { "drive shaft and/or more than 2 gear wheels", "multi-disc clutch, oil pump" },
			};
			foreach (var ptoType in ptoTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var ptoGearWheels = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_PTO,
					XMLNames.Vehicle_PTO_ShaftsGearWheels),
					manager);
				ptoGearWheels.SetValue(ptoType[0]);
				var ptoOther = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_PTO,
					XMLNames.Vehicle_PTO_OtherElements),
					manager);
				ptoOther.SetValue(ptoType[1]);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified,
					true);

				if (ptoType[0] == "none") {
					Assert.AreEqual("None",
						inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData.PTOTransmissionType);
				} else if (ptoType[0] == "only one engaged gearwheel above oil level") {
					Assert.AreEqual(ptoType[0],
						inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData.PTOTransmissionType);
				} else {
					Assert.AreEqual(string.Format("{0} - {1}", ptoType[0], ptoType[1]),
						inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData.PTOTransmissionType);
				}
				DeclarationData.PTOTransmission.Lookup(inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData.PTOTransmissionType);
			}
		}

		[TestCase]
		public void TestAngledriveTypes()
		{
			var angledriveTypes = new Dictionary<string, AngledriveType>() {
				{ "None", AngledriveType.None },
				{ "Losses included in Gearbox", AngledriveType.LossesIncludedInGearbox },
				{ "Separate Angledrive", AngledriveType.SeparateAngledrive }
			};
			foreach (var angleDrive in angledriveTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_AngledriveType),
					manager);
				xmlRetarderType.SetValue(angleDrive.Key);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified,
					true);

				Assert.AreEqual(angleDrive.Value, inputDataProvider.JobInputData.Vehicle.AngledriveInputData.Type);
			}
		}

		[TestCase]
		public void TestGearboxTypes()
		{
			var gearboxTypes = new Dictionary<string, GearboxType>() {
				{ "SMT", GearboxType.MT },
				{ "AMT", GearboxType.AMT },
				{ "APT-S", GearboxType.ATSerial },
				{ "APT-P", GearboxType.ATPowerSplit }
			};
			foreach (var gearboxType in gearboxTypes) {
				var reader = XmlReader.Create(SampleVehicleDecl);

				var doc = new XmlDocument();
				doc.Load(reader);
				var nav = doc.CreateNavigator();
				var manager = new XmlNamespaceManager(nav.NameTable);
				var helper = new XPathHelper(ExecutionMode.Declaration);
				helper.AddNamespaces(manager);

				var xmlRetarderType = nav.SelectSingleNode(helper.QueryAbs(
					helper.NSPrefix(XMLNames.VectoInputDeclaration,
						Constants.XML.RootNSPrefix),
					XMLNames.Component_Vehicle,
					XMLNames.Vehicle_Components,
					XMLNames.Component_Gearbox,
					XMLNames.ComponentDataWrapper,
					XMLNames.Gearbox_TransmissionType),
					manager);
				xmlRetarderType.SetValue(gearboxType.Key);

				var modified = XmlReader.Create(new StringReader(nav.OuterXml));

				var inputDataProvider = new XMLDeclarationInputDataProvider(modified,
					true);

				Assert.AreEqual(gearboxType.Value, inputDataProvider.JobInputData.Vehicle.GearboxInputData.Type);
			}
		}

		[TestCase]
		public void TestPTOInputNone()
		{
			var reader = XmlReader.Create(SampleVehicleDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var ptoDataProvider = inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData;

			Assert.AreEqual("None", ptoDataProvider.PTOTransmissionType);
		}

		[TestCase]
		public void TestPTOInput()
		{
			var reader = XmlReader.Create(SampleVehicleFullDecl);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);
			var ptoDataProvider = inputDataProvider.JobInputData.Vehicle.PTOTransmissionInputData;
			var lookup = DeclarationData.PTOTransmission.Lookup(ptoDataProvider.PTOTransmissionType);

			Assert.AreEqual("only the drive shaft of the PTO - multi-disc clutch", ptoDataProvider.PTOTransmissionType);
			Assert.AreEqual(1000, lookup.PowerDemand.Value());
		}

		[TestCase]
		public void TestCertificationMethodInput()
		{
			var reader = XmlReader.Create(SampleVehicleFullDeclCertificationOptions);

			var inputDataProvider = new XMLDeclarationInputDataProvider(reader, true);

			Assert.AreEqual(CertificationMethod.Option2, inputDataProvider.JobInputData.Vehicle.GearboxInputData.CertificationMethod);
			Assert.AreEqual(CertificationMethod.Measured, inputDataProvider.JobInputData.Vehicle.AxleGearInputData.CertificationMethod);
			Assert.AreEqual(CertificationMethod.Measured, inputDataProvider.JobInputData.Vehicle.RetarderInputData.CertificationMethod);
			Assert.AreEqual(CertificationMethod.Measured, inputDataProvider.JobInputData.Vehicle.AirdragInputData.CertificationMethod);
		}
	}
}
