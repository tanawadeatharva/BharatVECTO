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
			Assert.AreEqual(0.SI<CubicMeter>() , vehicle.CargoVolume);
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
			Assert.AreNotEqual(0.SI<CubicMeter>(), vehicle.CargoVolume);
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
			Assert.AreEqual(0.SI<CubicMeter>(), vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
			Assert.IsNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
			Assert.IsNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}
		
		[TestCase(@"HeavyLorry\HEV_heavyLorry_AMT_Px.xml")]

		public void TestHEVHeaveyLorry(string jobfile)
		{
			var filename = Path.Combine(BASE_DIR, jobfile);
			var dataProvider = xmlInputReader.CreateDeclaration(XmlReader.Create(filename));

			Assert.NotNull(dataProvider);
			Assert.NotNull(dataProvider.JobInputData);

			var vehicle = dataProvider.JobInputData.Vehicle;
			Assert.NotNull(vehicle);
			Assert.IsNotNull(vehicle.Components);
			Assert.IsNotNull(vehicle.Components.EngineInputData);

			TestElectricMachines(vehicle.Components.ElectricMachines);
			Assert.AreEqual(1, vehicle.Components.ElectricMachines.Entries.Count);
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
			Assert.AreEqual(0.SI<CubicMeter>(), vehicle.CargoVolume);
			Assert.IsNotNull(vehicle.TorqueLimits);
//			Assert.IsNotNull(vehicle.ElectricMotorTorqueLimits);//Vehicle EM Drive Limits
//			Assert.IsNotNull(vehicle.MaxPropulsionTorque);//Vehicle Max Prop. Limit
		}




		#region Test Electric Machines Reader

		private void TestElectricMachines(IElectricMachinesDeclarationInputData electricMachines)
		{
			Assert.IsNotNull(electricMachines);
			Assert.AreEqual(1, electricMachines.Entries.Count);
			var eMachine = electricMachines.Entries.First().ElectricMachine;

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
			Assert.AreEqual(outShaftSpeed, row["outShaftSpeed"].ToString());
			Assert.AreEqual(maxTorque, row["maxTorque"].ToString());
			Assert.AreEqual(minTorque, row["minTorque"].ToString());
		}

		private void TestPowerMapEntry(string outShaftSpeed, string torque, string electricPower, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row["outShaftSpeed"].ToString());
			Assert.AreEqual(torque, row["torque"].ToString());
			Assert.AreEqual(electricPower, row["electricPower"].ToString());
		}

		private void TestDragCurve(TableData dragCurve)
		{
			TestDragCurveEntry("0.00", "10.00", dragCurve.Rows[0]);
			TestDragCurveEntry("4000.00", "30.00", dragCurve.Rows[1]);
		}

		private void TestDragCurveEntry(string outShaftSpeed, string dragTorque, DataRow row)
		{
			Assert.AreEqual(outShaftSpeed, row["outShaftSpeed"].ToString());
			Assert.AreEqual(dragTorque, row["dragTorque"].ToString());
		}

		#endregion

		#region Test ElectricStorage Element Reader

		private void TestElectricStorageElements(IList<IElectricStorageDeclarationInputData> elements)
		{
			Assert.IsNotNull(elements);
			Assert.AreEqual(2, elements.Count);

			foreach (var entry in elements) {
				TestREESS(entry);
			}



		}

		private void TestREESS(IElectricStorageDeclarationInputData storage)
		{
			Assert.AreEqual(1, storage.StringId);
		}


		

		#endregion




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