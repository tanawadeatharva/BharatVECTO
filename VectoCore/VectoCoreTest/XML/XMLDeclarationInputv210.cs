using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Xml;
using Ninject;
using NUnit.Framework;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData.FileIO;

namespace TUGraz.VectoCore.Tests.XML
{
	[TestFixture]
	public class XMLDeclarationInputv210
	{
		protected IXMLInputDataReader xmlInputReader;
		private IKernel _kernel;

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);

			_kernel = new StandardKernel(new VectoNinjectModule());
			xmlInputReader = _kernel.Get<IXMLInputDataReader>();
		}

		private const string BASE_DIR = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\Distributed\";
		private const string ADDITONAL_TESTS_DIR = @"TestData\XML\XMLReaderDeclaration\SchemaVersion2.10\";


		[TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_IEPC-S.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_S3.xml"),
		TestCase(@"HeavyLorry\HEV-S_heavyLorry_S4.xml"),
		TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px.xml"),
		TestCase(@"HeavyLorry\IEPC_heavyLorry.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_AMT_E2.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_APT-N_E2.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_E3.xml"),
		TestCase(@"HeavyLorry\PEV_heavyLorry_E4.xml"),
		TestCase(@"MediumLorry\Conventional_mediumLorry_AMT.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_AMT_S2.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_IEPC-S.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_S3.xml"),
		TestCase(@"MediumLorry\HEV-S_mediumLorry_S4.xml"),
		TestCase(@"MediumLorry\HEV_mediumLorry_AMT_Px.xml"),
		TestCase(@"MediumLorry\IEPC_mediumLorry.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_AMT_E2.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_APT-N_E2.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_E3.xml"),
		TestCase(@"MediumLorry\PEV_mediumLorry_E4.xml"),
		TestCase(@"PrimaryBus\Conventional_primaryBus_AMT.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_AMT_S2.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_IEPC-S.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_S3.xml"),
		TestCase(@"PrimaryBus\HEV-S_primaryBus_S4.xml"),
		TestCase(@"PrimaryBus\HEV_primaryBus_AMT_Px.xml"),
		TestCase(@"PrimaryBus\IEPC_primaryBus.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_AMT_E2.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_E3.xml"),
		TestCase(@"PrimaryBus\PEV_primaryBus_E4.xml"),
		//TestCase(@"CompletedBus\Conventional_completedBus_1.xml"),
		//TestCase(@"CompletedBus\HEV_completedBus_1.xml"),
		//TestCase(@"CompletedBus\IEPC_completedBus_1.xml"),
		//TestCase(@"CompletedBus\PEV_completedBus_1.xml"),
		TestCase(@"ExemptedVehicles\exempted_completedBus_input_full.xml"),
		TestCase(@"ExemptedVehicles\exempted_completedBus_input_only_mandatory_entries.xml"),
		TestCase(@"ExemptedVehicles\exempted_heavyLorry.xml"),
		TestCase(@"ExemptedVehicles\exempted_mediumLorry.xml"),
		TestCase(@"ExemptedVehicles\exempted_primaryBus.xml"),
		]
		public void TestReadingJobVersion_V210(string jobFile)
		{
			ReadDeclarationJob(jobFile);
		}

		[TestCase(@"CompletedBus\Conventional_completedBus_1.xml"),
		TestCase(@"CompletedBus\HEV_completedBus_1.xml"),
		TestCase(@"CompletedBus\IEPC_completedBus_1.xml"),
		TestCase(@"CompletedBus\PEV_completedBus_1.xml"),
		]
		public void TestReadingCompletedBus_V210(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);
			Assert.NotNull(dataProvider.JobInputData.Vehicle);

			var veh = dataProvider.JobInputData.Vehicle;
			Assert.AreEqual(1, veh.NumberPassengerSeatsLowerDeck);
			Assert.AreEqual(9.5, veh.Length.Value());

			Assert.NotNull(veh.Components);
			Assert.NotNull(veh.Components.BusAuxiliaries);

			var busAux = veh.Components.BusAuxiliaries;
			Assert.AreEqual(BusHVACSystemConfiguration.Configuration0, busAux.HVACAux.SystemConfiguration);
			Assert.AreEqual(HeatPumpType.non_R_744_3_stage, busAux.HVACAux.HeatPumpTypeCoolingPassengerCompartment);

			Assert.IsTrue(busAux.ElectricConsumers.BrakelightsLED);
		}

		public IVectoRun[] ReadDeclarationJob(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);

			var fileWriter = new FileOutputWriter(filename);
			//var sumWriter = new SummaryDataContainer(fileWriter);
			//var jobContainer = new JobContainer(sumWriter);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			var runsFactory = new SimulatorFactory(ExecutionMode.Declaration, dataProvider, fileWriter) {
				ModalResults1Hz = false,
				WriteModalResults = false,
				ActualModalData = false,
				Validate = false,
			};

			var runs = runsFactory.SimulationRuns().ToArray();
			Assert.IsTrue(runs.Length > 0);

			return runs;
		}

		[TestCase(@"HeavyLorry\Conventional_heavyLorry_AMT.xml")]
		public void TestConventionalHeavyLorry(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData); 
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limits
		}


		[TestCase(@"MediumLorry\Conventional_mediumLorry_AMT.xml")]
		public void TestConventionalMediumLorry(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));
			
			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.AreEqual(20.300.SI<CubicMeter>(), vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}

		[TestCase(@"PrimaryBus\Conventional_primaryBus_AMT.xml")]
		public void TestConventionalPrimaryHeavyBus(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);

			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}
		
		[TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px.xml", BASE_DIR)]
		[TestCase(@"HEV_heavyLorry_AMT_Px_Capacitor.xml", ADDITONAL_TESTS_DIR)]
		public void TestHEVHeaveyLorry(string jobfile, string dir)
		{
			var filename = Path.Combine(dir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);

			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
			TestElectricMachinesData(vehicle.Components.ElectricMachines.Entries.First());
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels); 
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);

			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			TestElectricStorageElements(vehicle.Components.ElectricStorage.ElectricStorageElements);
			
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			TestElectricMotorTorqueLimits(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			TestBoostingLimitations(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}

		#region Test Electric Machines Reader

		private void TestElectricMachinesData(ElectricMachineEntry<IElectricMotorDeclarationInputData> eMachineEntry)
		{
			Assert.IsNotNull(eMachineEntry);
			var eMachine = eMachineEntry.ElectricMachine;

			Assert.IsNotNull(eMachine);
			Assert.AreEqual("a", eMachine.Manufacturer);
			Assert.AreEqual("a", eMachine.Model);
			Assert.AreEqual("token", eMachine.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), eMachine.Date);
			Assert.AreEqual("aaaaa", eMachine.AppVersion);
			Assert.AreEqual(ElectricMachineType.ASM, eMachine.ElectricMachineType);
			Assert.AreEqual(CertificationMethod.Measured, eMachine.CertificationMethod);
			Assert.AreEqual(1.SI<Watt>(), eMachine.R85RatedPower);
			Assert.AreEqual(0.10.SI<KilogramSquareMeter>(), eMachine.Inertia);//RotationalInertia
			Assert.AreEqual(200.00.SI<NewtonMeter>(), eMachine.ContinuousTorque);
			Assert.AreEqual(2000.00.SI<PerSecond>(), eMachine.ContinuousTorqueSpeed);//TestSpeedContinuousTorque
			Assert.AreEqual(400.00.SI<NewtonMeter>(), eMachine.OverloadTorque);
			Assert.AreEqual(2000.00.SI<PerSecond>(), eMachine.OverloadTestSpeed);//TestSpeedOverloadTorque
			Assert.AreEqual(30.00.SI<Second>(), eMachine.OverloadTime);
			Assert.AreEqual(483.SI<Volt>(), eMachine.TestVoltageOverload);
			Assert.AreEqual(true, eMachine.DcDcConverterIncluded);
			Assert.AreEqual("None", eMachine.IHPCType);

			TestVoltageLevel(eMachine.VoltageLevels);
			TestDragCurve(eMachine.DragCurve);
		}

		private void TestVoltageLevel(IList<IElectricMotorVoltageLevel> voltageLevels)
		{
			Assert.AreEqual(2, voltageLevels.Count);
			var voltageLevel = voltageLevels[0];
			Assert.AreEqual(400.SI<Volt>(), voltageLevel.VoltageLevel);

			TestMaxTorqueCurveEntry("0.00","450.00","-450.00", voltageLevel.FullLoadCurve.Rows[0]);
			TestMaxTorqueCurveEntry("4000.00", "100.00", "-100.00", voltageLevel.FullLoadCurve.Rows[1]);

			TestPowerMapEntry("0.00", "400.00", "1000.00", voltageLevel.EfficiencyMap.Rows[0]);
			TestPowerMapEntry("0.00", "-400.00", "-1000.00", voltageLevel.EfficiencyMap.Rows[1]);
			TestPowerMapEntry("4000.00", "4000.00", "20000.00", voltageLevel.EfficiencyMap.Rows[2]);
			TestPowerMapEntry("4000.00", "-4000.00", "-20000.00", voltageLevel.EfficiencyMap.Rows[3]);

			voltageLevel = voltageLevels[1];
			Assert.AreEqual(600.SI<Volt>(), voltageLevel.VoltageLevel);

			TestMaxTorqueCurveEntry("0.00", "450.00", "-450.00", voltageLevel.FullLoadCurve.Rows[0]);
			TestMaxTorqueCurveEntry("4000.00", "100.00", "-100.00", voltageLevel.FullLoadCurve.Rows[1]);

			TestPowerMapEntry("0.00", "400.00", "1000.00", voltageLevel.EfficiencyMap.Rows[0]);
			TestPowerMapEntry("0.00", "-400.00", "-1000.00", voltageLevel.EfficiencyMap.Rows[1]);
			TestPowerMapEntry("4000.00", "4000.00", "20000.00", voltageLevel.EfficiencyMap.Rows[2]);
			TestPowerMapEntry("4000.00", "-4000.00", "-20000.00", voltageLevel.EfficiencyMap.Rows[3]);
		}
		
		private void TestMaxTorqueCurveEntry(string outShaftSpeed, string maxTorque, string minTorque, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row[XMLNames.MaxTorqueCurve_OutShaftSpeed]);
			Assert.AreEqual(maxTorque, row[XMLNames.MaxTorqueCurve_MaxTorque]);
			Assert.AreEqual(minTorque, row[XMLNames.MaxTorqueCurve_MinTorque]);
		}

		private void TestPowerMapEntry(string outShaftSpeed, string torque, string electricPower, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row[XMLNames.PowerMap_OutShaftSpeed]);
			Assert.AreEqual(torque, row[XMLNames.PowerMap_Torque]);
			Assert.AreEqual(electricPower, row[XMLNames.PowerMap_ElectricPower]);
		}

		private void TestDragCurve(TableData dragCurve)
		{
			TestDragCurveEntry("0.00", "10.00", dragCurve.Rows[0]);
			TestDragCurveEntry("4000.00", "30.00", dragCurve.Rows[1]);
		}

		private void TestDragCurveEntry(string outShaftSpeed, string dragTorque, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row[XMLNames.DragCurve_OutShaftSpeed]);
			Assert.AreEqual(dragTorque, row[XMLNames.DragCurve_DragTorque]);
		}

		#endregion

		#region Test ElectricStorage Element Reader

		private void TestElectricStorageElements(IList<IElectricStorageDeclarationInputData> elements)
		{
			Assert.IsNotNull(elements);
			switch (elements.Count) {
				case 1:
					TestSuperCapEntry(elements.First());
					break;
				case 2:
					Assert.AreEqual(2, elements.Count);
					TestFirstBatterySystemEntry(elements[0]);
					TestSecondBatterySystemEntry(elements[1]);
					break;
			}
		}

		private void TestSuperCapEntry(IElectricStorageDeclarationInputData entry)
		{
			Assert.AreEqual(REESSType.SuperCap, entry.REESSPack.StorageType);
			Assert.AreEqual("Capacitor Manufacturer", entry.REESSPack.Manufacturer);
			Assert.AreEqual("Capacitor Model", entry.REESSPack.Model);
			Assert.AreEqual("ccccccc", entry.REESSPack.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-03T00:00:00Z").ToUniversalTime(), entry.REESSPack.Date);
			Assert.AreEqual("ccccc", entry.REESSPack.AppVersion);
			Assert.AreEqual(CertificationMethod.Measured, entry.REESSPack.CertificationMethod);
			Assert.IsNotNull(entry.REESSPack.DigestValue);
			
			var supercap = (ISuperCapDeclarationInputData)entry.REESSPack;
			Assert.AreEqual(100.00.SI<Farad>(), supercap.Capacity);
			Assert.AreEqual(20.00.SI<Ohm>() / 1000, supercap.InternalResistance);
			Assert.AreEqual(12.00.SI<Volt>(), supercap.MinVoltage);
			Assert.AreEqual(100.00.SI<Volt>(), supercap.MaxVoltage);
			Assert.AreEqual(80.00.SI<Ampere>(), supercap.MaxCurrentCharge);
			Assert.AreEqual(20.00.SI<Ampere>(), supercap.MaxCurrentDischarge);
		}

		private void TestFirstBatterySystemEntry(IElectricStorageDeclarationInputData entry)
		{
			Assert.AreEqual(REESSType.Battery, entry.REESSPack.StorageType);
			Assert.AreEqual(0, entry.StringId);
			Assert.AreEqual("a", entry.REESSPack.Manufacturer);
			Assert.AreEqual("a", entry.REESSPack.Model);
			Assert.AreEqual("tokena", entry.REESSPack.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), entry.REESSPack.Date);
			Assert.AreEqual("aaaaa", entry.REESSPack.AppVersion);
			Assert.AreEqual(CertificationMethod.Measured, entry.REESSPack.CertificationMethod);
			Assert.IsNotNull(entry.REESSPack.DigestValue);

			var battery = (IBatteryPackDeclarationInputData)entry.REESSPack;
			Assert.AreEqual(20, battery.MinSOC);
			Assert.AreEqual(80, battery.MaxSOC);
			Assert.AreEqual(BatteryType.HPBS, battery.BatteryType);
			Assert.AreEqual(72.00.SI<AmpereSecond>() * 3600, battery.Capacity);
			Assert.AreEqual(true, battery.ConnectorsSubsystemsIncluded);
			Assert.AreEqual(true, battery.JunctionboxIncluded);
			Assert.AreEqual(20.0.DegCelsiusToKelvin(), battery.TestingTemperature);

			Assert.IsNotNull(battery.VoltageCurve);//OVC Data
			TestOCVTableRow("0", "620.00", battery.VoltageCurve.Rows[0]);
			TestOCVTableRow("100", "640.00", battery.VoltageCurve.Rows[1]);
			Assert.IsNotNull(battery.InternalResistanceCurve);
			TestInternalResistanceTableRow("0", "10.00", "11.00", "12.00", battery.InternalResistanceCurve.Rows[0]);
			TestInternalResistanceTableRow("100","12.00" ,"14.00","16.00", battery.InternalResistanceCurve.Rows[1]);
			Assert.IsNotNull(battery.MaxCurrentMap);//CurrentLimits Data
			TestCurrentLimitsTableRow("0", "50.00", "0.00", battery.MaxCurrentMap.Rows[0]);
			TestCurrentLimitsTableRow("100", "0.00", "50.00", battery.MaxCurrentMap.Rows[1]);
		}

		private void TestSecondBatterySystemEntry(IElectricStorageDeclarationInputData entry)
		{
			Assert.AreEqual(REESSType.Battery, entry.REESSPack.StorageType);
			Assert.AreEqual(1, entry.StringId);
			Assert.AreEqual("b", entry.REESSPack.Manufacturer);
			Assert.AreEqual("b", entry.REESSPack.Model);
			Assert.AreEqual("tokenb", entry.REESSPack.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-02-02T00:00:00Z").ToUniversalTime(), entry.REESSPack.Date);
			Assert.AreEqual("bbbbb", entry.REESSPack.AppVersion);
			Assert.AreEqual(CertificationMethod.Measured, entry.REESSPack.CertificationMethod);
			Assert.IsNotNull(entry.REESSPack.DigestValue);

			var battery = (IBatteryPackDeclarationInputData)entry.REESSPack;
			Assert.IsNull(battery.MinSOC);
			Assert.IsNull(battery.MaxSOC);
			Assert.AreEqual(BatteryType.HPBS, battery.BatteryType);
			Assert.AreEqual(73.00.SI<AmpereSecond>() * 3600, battery.Capacity);
			Assert.AreEqual(true, battery.ConnectorsSubsystemsIncluded);
			Assert.AreEqual(true, battery.JunctionboxIncluded);
			Assert.AreEqual(20.0.DegCelsiusToKelvin(), battery.TestingTemperature);

			Assert.IsNotNull(battery.VoltageCurve);//OVC Data
			TestOCVTableRow("0", "621.00", battery.VoltageCurve.Rows[0]);
			TestOCVTableRow("100", "641.00", battery.VoltageCurve.Rows[1]);
			Assert.IsNotNull(battery.InternalResistanceCurve);
			TestInternalResistanceTableRow("0", "11.00", "12.00", "13.00", battery.InternalResistanceCurve.Rows[0]);
			TestInternalResistanceTableRow("100", "12.00", "14.00", "16.00", battery.InternalResistanceCurve.Rows[1]);
			Assert.IsNotNull(battery.MaxCurrentMap);//CurrentLimits Data
			TestCurrentLimitsTableRow("0", "51.00", "0.00", battery.MaxCurrentMap.Rows[0]);
			TestCurrentLimitsTableRow("100", "0.00", "50.00", battery.MaxCurrentMap.Rows[1]);
		}

		private void TestOCVTableRow(string soc, string ocv, DataRow row)
		{
			Assert.AreEqual(soc, row[XMLNames.REESS_OCV_SoC]);
			Assert.AreEqual(ocv, row[XMLNames.REESS_OCV_OCV]);
		}

		private void TestInternalResistanceTableRow(string soc, string r2, string r10, string r20, DataRow row)
		{
			Assert.AreEqual(soc, row[XMLNames.REESS_InternalResistanceCurve_SoC]);
			Assert.AreEqual(r2, row[XMLNames.REESS_InternalResistanceCurve_R2]);
			Assert.AreEqual(r10, row[XMLNames.REESS_InternalResistanceCurve_R10]);
			Assert.AreEqual(r20, row[XMLNames.REESS_InternalResistanceCurve_R20]);
		}
		
		private void TestCurrentLimitsTableRow(string soc, string maxChargingCurrent, string maxDischargingCurrent,
			DataRow row)
		{
			Assert.AreEqual(soc, row[XMLNames.REESS_CurrentLimits_SoC]);
			Assert.AreEqual(maxChargingCurrent, row[XMLNames.REESS_CurrentLimits_MaxChargingCurrent]);
			Assert.AreEqual(maxDischargingCurrent, row[XMLNames.REESS_CurrentLimits_MaxDischargingCurrent]);
		}


		#endregion

		#region Test Electric Motor TorqueLimits Reader

		private void TestElectricMotorTorqueLimits(Dictionary<PowertrainPosition, List<Tuple<int, TableData>>> limits)
		{
			Assert.IsNotNull(limits);
			Assert.AreEqual(1, limits.Count);
			Assert.AreEqual(2, limits.First().Value.Count);
			Assert.AreEqual(PowertrainPosition.HybridP2, limits.First().Key);
			
			Assert.AreEqual(100, limits.First().Value[0].Item1);
			TestMaxTorqueCurveEntry("0.00", "200.00", "-200.00", limits.First().Value[0].Item2.Rows[0]);
			TestMaxTorqueCurveEntry("1000.00", "300.00", "-300.00", limits.First().Value[0].Item2.Rows[1]);

			Assert.AreEqual(500, limits.First().Value[1].Item1);
			TestMaxTorqueCurveEntry("0.00", "200.00", "-200.00", limits.First().Value[1].Item2.Rows[0]);
			TestMaxTorqueCurveEntry("1000.00", "300.00", "-300.00", limits.First().Value[1].Item2.Rows[1]);
		}

		#endregion
		
		#region Test Max Propulsion Torque Reader / BoostingLimitations

		private void TestBoostingLimitations(TableData boostingLimitations)
		{
			Assert.IsNotNull(boostingLimitations);
			Assert.AreEqual(2, boostingLimitations.Rows.Count);

			TestBoostingLimitationsEntry("0.00", "0.00", boostingLimitations.Rows[0]);
			TestBoostingLimitationsEntry("1000.00", "0.00", boostingLimitations.Rows[1]);
		}

		private void TestBoostingLimitationsEntry(string rotationalSpeed, string boostingTorque, DataRow row)
		{
			Assert.AreEqual(rotationalSpeed, row[XMLNames.BoostingLimitation_RotationalSpeed]);
			Assert.AreEqual(boostingTorque, row[XMLNames.BoostingLimitation_BoostingTorque]);
		}

		#endregion

		
		[TestCase(@"MediumLorry\HEV_mediumLorry_AMT_Px.xml")]
		public void TestHEVMediumLorry(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.AreEqual(20.300.SI<CubicMeter>(), vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNotNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}
		

		[TestCase(@"PrimaryBus\HEV_primaryBus_AMT_Px.xml")]
		public void TestHEVPrimaryBusAMTPx(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNotNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNotNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}

		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_AMT_S2.xml", BASE_DIR)]
		[TestCase(@"HEV-S_heavyLorry_AMT_S2_ADC.xml", ADDITONAL_TESTS_DIR)]
		public void TestHEVHeavyLorryAMTS2(string jobfile, string fileDir)
		{
			var filename = Path.Combine(fileDir, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			TestElectricMachines(vehicle.Components.ElectricMachines.Entries);
			Assert.IsNotNull(vehicle.Components.GearboxInputData);
			TestTorqueConverter(vehicle);
			Assert.IsNotNull(vehicle.Components.AngledriveInputData);//optional
			Assert.IsNotNull(vehicle.Components.RetarderInputData);//optional
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}

		private void TestElectricMachines(IList<ElectricMachineEntry<IElectricMotorDeclarationInputData>> eMachines)
		{
			foreach (var eMachine in eMachines) {
				switch (eMachine.Position) {
					case PowertrainPosition.GEN:
						TestElectricMachineGEN(eMachine);
						break;
					case PowertrainPosition.BatteryElectricE2:
						TestElectricMachine(eMachine);
						break;
				}
			}
		}

		private void TestElectricMachineGEN(ElectricMachineEntry<IElectricMotorDeclarationInputData> eMachine)
		{
			Assert.AreEqual(1, eMachine.Count);
			TestElectricMachinesData(eMachine);
			if (eMachine.ADC != null)
				TestADC(eMachine.ADC);
		}

		private void TestElectricMachine(ElectricMachineEntry<IElectricMotorDeclarationInputData> eMachine)
		{
			Assert.AreEqual(1, eMachine.Count);
			TestElectricMachinesData(eMachine);
			if(eMachine.ADC != null)
				TestADC(eMachine.ADC);
		}

		private void TestADC(IADCDeclarationInputData adcData)
		{
			Assert.AreEqual("ADC Manufacturer", adcData.Manufacturer);
			Assert.AreEqual("ADC Model", adcData.Model);
			Assert.AreEqual("adcadc", adcData.CertificationNumber);
			Assert.AreEqual(DateTime.Parse("2017-01-01T00:00:00Z").ToUniversalTime(), adcData.Date);
			Assert.AreEqual("adccda", adcData.AppVersion);
			Assert.AreEqual(12.123, adcData.Ratio);
			Assert.AreEqual(CertificationMethod.Option1, adcData.CertificationMethod);
			Assert.IsNotNull(adcData.DigestValue);

			TestTorqueLossMapEntry("10.00","40.00" ,"30.00", adcData.LossMap.Rows[0]);
			TestTorqueLossMapEntry("11.00", "41.00", "31.00", adcData.LossMap.Rows[1]);
			TestTorqueLossMapEntry("12.00", "41.00", "32.00", adcData.LossMap.Rows[2]);
			TestTorqueLossMapEntry("13.00", "42.00", "33.00", adcData.LossMap.Rows[3]);
		}

		private void TestTorqueLossMapEntry(string inputSpeed, string inputTorque, string torqueLoss, DataRow row)
		{
			Assert.AreEqual(inputSpeed, row[XMLNames.ADC_TorqueLossMap_InputSpeed]);
			Assert.AreEqual(inputTorque, row[XMLNames.ADC_TorqueLossMap_InputTorque]);
			Assert.AreEqual(torqueLoss, row[XMLNames.ADC_TorqueLossMap_TorqueLoss]);
		}


		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_S3.xml")]
		public void TestHeavyLorryS3(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2 ,vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN,vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.RetarderInputData);
			Assert.IsNotNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.MaxPropulsionTorque);
		}


		[TestCase(@"HeavyLorry\HEV-S_heavyLorry_S4.xml")]
		public void TestHeavyLorryS4(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider.JobInputData);
			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);
			Assert.IsNotNull(vehicle.Components.ElectricMachines);
			Assert.AreEqual(2, vehicle.Components.ElectricMachines.Entries.Count);
			Assert.AreEqual(PowertrainPosition.BatteryElectricE2, vehicle.Components.ElectricMachines.Entries[0].Position);
			Assert.AreEqual(PowertrainPosition.GEN, vehicle.Components.ElectricMachines.Entries[1].Position);
			Assert.IsNull(vehicle.Components.GearboxInputData);
			Assert.IsNull(vehicle.Components.TorqueConverterInputData);
			Assert.IsNull(vehicle.Components.AngledriveInputData);
			Assert.IsNotNull(vehicle.Components.RetarderInputData);
			Assert.IsNull(vehicle.Components.AxleGearInputData);
			Assert.IsNotNull(vehicle.Components.AxleWheels);
			Assert.IsNotNull(vehicle.Components.AuxiliaryInputData);
			Assert.IsNull(vehicle.Components.BusAuxiliaries);
			Assert.IsNotNull(vehicle.Components.AirdragInputData);
			Assert.IsNotNull(vehicle.Components.ElectricStorage);
			Assert.IsNotNull(vehicle.Components.PTOTransmissionInputData);
			Assert.IsNull(vehicle.CargoVolume);
			Assert.IsNull(vehicle.TorqueLimits);
			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);
			Assert.IsNull(vehicle.MaxPropulsionTorque);
		}


		#region Test existence of torque converter

		private void TestTorqueConverter(IVehicleDeclarationInputData vehicle)
		{
			var torqueConverter = vehicle.Components.TorqueConverterInputData;
			switch (vehicle.Components?.GearboxInputData?.Type)
			{
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					Assert.IsNotNull(torqueConverter);
					break;
			}
		}

		#endregion

	}
}