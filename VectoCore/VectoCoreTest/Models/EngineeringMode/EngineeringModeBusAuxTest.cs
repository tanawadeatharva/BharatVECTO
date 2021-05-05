using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Ninject;
using NUnit.Framework;
using NUnit.Framework.Internal;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.FileIO.XML;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.BusAuxiliaries;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Pneumatics;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.Electrics;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Models.SimulationComponent;
using TUGraz.VectoCore.Tests.Utils;
using TUGraz.VectoCore.Utils;
using Wheels = TUGraz.VectoCore.Models.SimulationComponent.Impl.Wheels;

namespace TUGraz.VectoCore.Tests.Models.EngineeringMode
{
	[TestFixture]
	public class EngineeringModeBusAuxTest
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

		const string JobFile = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux.vecto";
		const string JobFile_SmartES = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux_SmartES.vecto";
		const string JobFile_SmartPS = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux_SmartPS.vecto";
		const string JobFile_SmartES_SmartPS = @"TestData\Integration\Buses\EngineeringMode\InterurbanBus_ENG_BusAux_SmartES-SmartPS.vecto";

		const string JobFile_A = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_A.vecto";
		const string JobFile_A_ESS = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_A_ESS.vecto";
		const string JobFile_B = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_B.vecto";
		const string JobFile_C1 = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_C1.vecto";
		const string JobFile_C2a = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_C2a.vecto";
		const string JobFile_C2b = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_C2b.vecto";
		const string JobFile_C3a = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_C3a.vecto";
		const string JobFile_C3b = @"TestData\Hybrids\BusAuxEngineeringMode\InterurbanBus_ENG_BusAux_C3b.vecto";


		private const string JobRoeck_BusAux_B =
			@"J:\TE-Em\Emissionsmodelle\VECTO\Arbeitsordner\AAUX\Check bus aux electrical system configurations\System type B\Citybus_P0-APT-S-175kW-6.8l_B\Citybus_P0_B.vecto";
		[
		TestCase(JobFile, 0, TestName = "InterurbanBus ENG BusAux NonSmart Interurban"),
		TestCase(JobFile, 1, TestName = "InterurbanBus ENG BusAux NonSmart Coach"),
		TestCase(JobFile, 2, TestName = "InterurbanBus ENG BusAux NonSmart Urban"),
		TestCase(JobFile, 3, TestName = "InterurbanBus ENG BusAux NonSmart Suburban"),
		TestCase(JobFile, 4, TestName = "InterurbanBus ENG BusAux NonSmart HeavyUrban"),

		TestCase(JobFile_SmartES, 0, TestName = "InterurbanBus ENG BusAux Smart-ES Interurban"),
		TestCase(JobFile_SmartES, 1, TestName = "InterurbanBus ENG BusAux Smart-ES Coach"),
		TestCase(JobFile_SmartES, 2, TestName = "InterurbanBus ENG BusAux Smart-ES Urban"),
		TestCase(JobFile_SmartES, 3, TestName = "InterurbanBus ENG BusAux Smart-ES Suburban"),
		TestCase(JobFile_SmartES, 4, TestName = "InterurbanBus ENG BusAux Smart-ES HeavyUrban"),

		TestCase(JobFile_SmartPS, 0, TestName = "InterurbanBus ENG BusAux Smart-PS Interurban"),
		TestCase(JobFile_SmartPS, 1, TestName = "InterurbanBus ENG BusAux Smart-PS Coach"),
		TestCase(JobFile_SmartPS, 2, TestName = "InterurbanBus ENG BusAux Smart-PS Urban"),
		TestCase(JobFile_SmartPS, 3, TestName = "InterurbanBus ENG BusAux Smart-PS Suburban"),
		TestCase(JobFile_SmartPS, 4, TestName = "InterurbanBus ENG BusAux Smart-PS HeavyUrban"),

		TestCase(JobFile_SmartES_SmartPS, 0, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Interurban"),
		TestCase(JobFile_SmartES_SmartPS, 1, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Coach"),
		TestCase(JobFile_SmartES_SmartPS, 2, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Urban"),
		TestCase(JobFile_SmartES_SmartPS, 3, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS Suburban"),
		TestCase(JobFile_SmartES_SmartPS, 4, TestName = "InterurbanBus ENG BusAux Smart-ES Smart-PS HeavyUrban"),

		TestCase(JobFile_A, 2, TestName = "InterurbanBus ENG BusAux A Urban"),
		TestCase(JobFile_B, 2, TestName = "InterurbanBus ENG BusAux B Urban"),
		TestCase(JobFile_C1, 2, TestName = "InterurbanBus ENG BusAux C1 Urban"),
		TestCase(JobFile_C2a, 2, TestName = "InterurbanBus ENG BusAux C2a Urban"),
		TestCase(JobFile_C2b, 2, TestName = "InterurbanBus ENG BusAux C2b Urban"),
		TestCase(JobFile_C3a, 2, TestName = "InterurbanBus ENG BusAux C3a Urban"),
		TestCase(JobFile_C3b, 2, TestName = "InterurbanBus ENG BusAux C3b Urban"),

		TestCase(JobFile_A_ESS, 0, TestName = "InterurbanBus ENG BusAux A ESS Interurban"),
		TestCase(JobFile_A_ESS, 1, TestName = "InterurbanBus ENG BusAux A ESS Coach"),
		TestCase(JobFile_A_ESS, 2, TestName = "InterurbanBus ENG BusAux A ESS Urban"),

		TestCase(JobFile_C1, 0, TestName = "InterurbanBus ENG BusAux C1 Interurban"),

		TestCase(JobRoeck_BusAux_B, 0, "dev", TestName = "Roeck Citybus P0 Type B"),
		]
		public void InterurbanBus_BusAuxTest(string jobFile, int runIdx, string outPath = null)
		{
			var outFile = Path.Combine(Path.GetDirectoryName(jobFile), outPath, Path.GetFileName(jobFile));
			if (!Directory.Exists(Path.GetDirectoryName(outFile))) {
				Directory.CreateDirectory(Path.GetDirectoryName(outFile));
			}
			var writer = new FileOutputWriter(outFile);
			var inputData = Path.GetExtension(jobFile) == ".xml"
				? xmlInputReader.CreateDeclaration(jobFile)
				//? new XMLDeclarationInputDataProvider(relativeJobPath, true)
				: JSONInputDataFactory.ReadJsonJob(jobFile);

			var sumContainer = new SummaryDataContainer(writer);
			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputData, writer) {
				WriteModalResults = true,
                SumData = sumContainer,
                //ActualModalData = true,
                Validate = false
			};

			var jobContainer = new JobContainer(sumContainer);

			var run = factory.SimulationRuns().ToArray()[runIdx];

			Assert.NotNull(run);

			jobContainer.AddRun(run);

			var pt = run.GetContainer();
			Assert.NotNull(pt);

			run.Run();
			Assert.IsTrue(run.FinishedWithoutErrors);

			jobContainer.Execute();
			jobContainer.WaitFinished();
		}


		// ##########################################################

		// Case B: Smart ES

		// driving, ICE on, battery not empty
		// driving, ICE on, battery empty
		// driving, ICE off, battery not empty
		// driving, ICE off, battery empty
		// standstill, ICE on, battery not empty
		// standstill, ICE on, battery empty
		// standstill, ICE off, battery not empty
		// standstill, ICE off, battery empty
		// braking, battery not full
		// braking, battery full

		public const double AlternatorEfficiency = 0.7;
		public const double DCDCEfficiency = 0.97;

		public const double MaxAlternatorPower = 4000.0;

		public const double I_Base = 25.0;
		public const double I_ICEOff_dr = 20.0;
		public const double I_ICEOff_stop = 10.0;

		public const double P_aux_m_Base = 900;
		public const double P_aux_m_ICEOff_dr = 750;
		public const double P_aux_m_ICEOff_st = 300;

		public const double PowernetVoltage = 28.3;
		public const double P_ES_base = I_Base * PowernetVoltage;  // 707,5 W
		public const double P_ES_ICEOff_dr = I_ICEOff_dr * PowernetVoltage; // 566 W
		public const double P_ES_ICEOff_stop = I_ICEOff_stop * PowernetVoltage; // 283 W

		public const double P_PS_off_600 = 524.3 / 0.97; //540.5154639 W
		public const double P_PS_off_1000 = 775.75 / 0.97; // 799.742268 W

		public static WattSecond ElectricStorageCapacity = 28.3.SI(Unit.SI.Watt.Hour).Cast<WattSecond>();

		[TestCase(DrivingBehavior.Accelerating, true, 0,
			P_aux_m_Base + P_PS_off_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base, P_ES_base / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case A (1); driving, ICE on")]
		[TestCase(DrivingBehavior.Accelerating, false, 0,
			0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr + P_PS_off_600,
			P_aux_m_Base + P_PS_off_600 + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency,
			TestName = "BusAux Case A (2); driving, ICE off")]
		[TestCase(DrivingBehavior.Halted, true, 0,
			P_aux_m_Base + P_PS_off_600 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
			P_ES_base / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case A (3); standstill, ICE on")]
		[TestCase(DrivingBehavior.Halted, false, 0,
			0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st + P_PS_off_600,
			P_PS_off_600 + P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency,
			TestName = "BusAux Case A (4); standstill, ICE off")]
		[TestCase(DrivingBehavior.Braking, true, 0,
			P_aux_m_Base + P_PS_off_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
			P_ES_base / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case A (5); braking, ICE on")]
		public void TestBusAux_Case_A(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC,
			double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
			double P_busAux_ES_mech,
			double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected)
		{
			TestBusAux_Casees(AlternatorType.Conventional, drivingBehavior, iceOn, batterySoC, P_auxMech_expected, P_busAux_ES_gen_expected, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech, P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected);
		}


		[TestCase(DrivingBehavior.Accelerating, true, 0.5,
			P_aux_m_Base + P_PS_off_1000, 0, P_ES_base, 0, 0, 0,
			TestName = "BusAux Case B (1); driving, ICE on, battery not empty")]
		[TestCase(DrivingBehavior.Accelerating, true, 0,
			P_aux_m_Base + P_PS_off_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
			P_ES_base / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case B (2); driving, ICE on, battery empty")]
		[TestCase(DrivingBehavior.Accelerating, false, 0.5,
			0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr + P_PS_off_600,
			P_aux_m_Base + P_PS_off_600 + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency,
			TestName = "BusAux Case B (3); driving, ICE off, battery not empty")]
		[TestCase(DrivingBehavior.Accelerating, false, 0,
			0, 0, P_ES_ICEOff_dr, 0, P_aux_m_ICEOff_dr + P_PS_off_600,
			P_aux_m_Base + P_PS_off_600 + (P_ES_base - P_ES_ICEOff_dr) / AlternatorEfficiency,
			TestName = "BusAux Case B (4); driving, ICE off, battery empty")]
		[TestCase(DrivingBehavior.Halted, true, 0.5,
			P_aux_m_Base + P_PS_off_600, 0, P_ES_base, 0, 0, 0,
			TestName = "BusAux Case B (5); standstill, ICE on, battery not empty")]
		[TestCase(DrivingBehavior.Halted, true, 0,
			P_aux_m_Base + P_PS_off_600 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
			P_ES_base / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case B (6); standstill, ICE on, battery empty")]
		[TestCase(DrivingBehavior.Halted, false, 0.5,
			0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st + P_PS_off_600,
			P_PS_off_600 + P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency,
			TestName = "BusAux Case B (7); standstill, ICE off, battery not empty")]
		[TestCase(DrivingBehavior.Halted, false, 0,
			0, 0, P_ES_ICEOff_stop, 0, P_aux_m_ICEOff_st + P_PS_off_600,
			P_PS_off_600 + P_aux_m_Base + (P_ES_base - P_ES_ICEOff_stop) / AlternatorEfficiency,
			TestName = "BusAux Case B (8); standstill, ICE off, battery empty")]
		[TestCase(DrivingBehavior.Braking, true, 0.5,
			P_aux_m_Base + P_PS_off_1000 + MaxAlternatorPower / AlternatorEfficiency, MaxAlternatorPower, P_ES_base,
			MaxAlternatorPower / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case B (9); braking, ICE on, battery not full")]
		[TestCase(DrivingBehavior.Braking, true, 1,
			P_aux_m_Base + P_PS_off_1000 + P_ES_base / AlternatorEfficiency, P_ES_base, P_ES_base,
			P_ES_base / AlternatorEfficiency, 0, 0,
			TestName = "BusAux Case B (10); braking, ICE on, battery full")]
		public void TestBusAux_Case_B(DrivingBehavior drivingBehavior, bool iceOn, double batterySoC,
			double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
			double P_busAux_ES_mech,
			double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected)
		{
			TestBusAux_Casees(AlternatorType.Smart, drivingBehavior, iceOn, batterySoC, P_auxMech_expected, P_busAux_ES_gen_expected, P_busAux_ES_consumer_sum_expected, P_busAux_ES_mech, P_aux_ESS_mech_ICE_off_expected, P_aux_ESS_mech_ICE_on_expected);
		}

		public void TestBusAux_Casees(AlternatorType alternatorType, DrivingBehavior drivingBehavior, bool iceOn,
			double batterySoC,
			double P_auxMech_expected, double P_busAux_ES_gen_expected, double P_busAux_ES_consumer_sum_expected,
			double P_busAux_ES_mech,
			double P_aux_ESS_mech_ICE_off_expected, double P_aux_ESS_mech_ICE_on_expected) 
		{

			var container = CreatePowerTrain(batterySoC, alternatorType);

			container.VehicleStopped = drivingBehavior == DrivingBehavior.Halted;
			container.VehicleSpeed = drivingBehavior == DrivingBehavior.Halted
				? 0.KMPHtoMeterPerSecond()
				: 30.KMPHtoMeterPerSecond();
			container.DriverBehavior = drivingBehavior;
			container.DrivingAction = drivingBehavior == DrivingBehavior.Halted
				? DrivingAction.Halt
				: (drivingBehavior == DrivingBehavior.Braking ? DrivingAction.Brake : DrivingAction.Accelerate);
			container.EngineSpeed = drivingBehavior == DrivingBehavior.Halted ? 600.RPMtoRad() : 1000.RPMtoRad();
			container.EngineCtl.CombustionEngineOn = iceOn;


			var ice = container.Components.First(x => x is ICombustionEngine) as StopStartCombustionEngine;
			Assert.NotNull(ice);

			var modData = container.ModalData as ModalDataContainer;
			Assert.NotNull(modData);

			var absTime = 0.SI<Second>();
			var dt = 1.SI<Second>();
			IResponse response = null;
			switch (drivingBehavior) {
				case DrivingBehavior.Halted:
					container.Gear = new GearshiftPosition(0);
					ice.CombustionEngineOn = iceOn;
					response = ice.Request(absTime, dt, 0.SI<NewtonMeter>(), 600.RPMtoRad(), false);
					break;
				case DrivingBehavior.Accelerating:
				case DrivingBehavior.Driving:
					container.Gear = new GearshiftPosition(3);
					ice.CombustionEngineOn = iceOn;
					response = ice.Request(absTime, dt, 0.SI<NewtonMeter>(), iceOn ? 1000.RPMtoRad() : 600.RPMtoRad(), false);
					break;
				case DrivingBehavior.Braking:
					container.Gear = new GearshiftPosition(3);
					container.BrakePower = 50e3.SI<Watt>();
					ice.CombustionEngineOn = iceOn;
					response = ice.Request(absTime, dt, 0.SI<NewtonMeter>(), iceOn ? 1000.RPMtoRad() : 600.RPMtoRad(), false);
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(drivingBehavior), drivingBehavior, null);
			}

			Assert.NotNull(response);
			Assert.IsAssignableFrom<ResponseSuccess>(response);
			
			container.CommitSimulationStep(absTime, dt);
			modData.CommitSimulationStep();

			Assert.AreEqual(1, modData.Data.Rows.Count);

			var row = modData.Data.Rows[0];

			Assert.AreEqual(iceOn, row.Field<bool>(ModalResultField.ICEOn.GetName()), ModalResultField.ICEOn.GetName());
			Assert.AreEqual(P_auxMech_expected, row.Field<SI>(ModalResultField.P_aux_mech.GetName()).Value(), 1e-3, ModalResultField.P_aux_mech.GetName());
			Assert.AreEqual(P_busAux_ES_gen_expected, row.Field<SI>(ModalResultField.P_busAux_ES_generated.GetName()).Value(), 1e-3, ModalResultField.P_busAux_ES_generated.GetName());
			Assert.AreEqual(P_busAux_ES_consumer_sum_expected, row.Field<SI>(ModalResultField.P_busAux_ES_consumer_sum.GetName()).Value(), 1e-3, ModalResultField.P_busAux_ES_consumer_sum.GetName());
			Assert.AreEqual(P_busAux_ES_consumer_sum_expected, row.Field<SI>(ModalResultField.P_busAux_ES_other.GetName()).Value(), 1e-3, ModalResultField.P_busAux_ES_other.GetName());
			Assert.AreEqual(P_busAux_ES_mech, row.Field<SI>(ModalResultField.P_busAux_ES_sum_mech.GetName()).Value(), 1e-3, ModalResultField.P_busAux_ES_sum_mech.GetName());

			Assert.AreEqual(P_aux_ESS_mech_ICE_off_expected, row.Field<SI>(ModalResultField.P_aux_ESS_mech_ice_off.GetName()).Value(), 1e-3, ModalResultField.P_aux_ESS_mech_ice_off.GetName());
			Assert.AreEqual(P_aux_ESS_mech_ICE_on_expected, row.Field<SI>(ModalResultField.P_aux_ESS_mech_ice_on.GetName()).Value(), 1e-3, ModalResultField.P_aux_ESS_mech_ice_on.GetName());

		}

		public const string EngineFileHigh = @"TestData\Components\24t Coach_high.veng";

		public static MockVehicleContainer CreatePowerTrain(double initialSoC, AlternatorType alternatorType)
		{
			//var gearboxData = CreateGearboxData();
			var engineData = MockSimulationDataFactory.CreateEngineDataFromFile(EngineFileHigh, 6);
            //var axleGearData = CreateAxleGearData();
            var vehicleData = CreateVehicleData(3300.SI<Kilogram>());
            //var airdragData = CreateAirdragData();
            //var driverData = CreateDriverData(AccelerationFile);

            var cycleData = DrivingCycleDataReader.ReadFromStream("s,v,grad,stop\n0,0,0,10\n10,20,0,0\n20,21,0,0\n30,22,0,0\n40,23,0,0\n50,24,0,0\n60,25,0,0\n70,26,0,0\n80,27,0,0\n90,28,0,0\n100,29,0,0".ToStream(), CycleType.DistanceBased, "DummyCycle", false);

			var runData = new VectoRunData() {
				JobRunId = 0,
                VehicleData = vehicleData,
                EngineData = engineData,
				ElectricMachinesData = new List<Tuple<PowertrainPosition, ElectricMotorData>>(),
				SimulationType = SimulationType.DistanceCycle,
				Cycle = cycleData,
				BusAuxiliaries = CreateBusAuxData(alternatorType, vehicleData),
				Aux = CreateAuxiliaryData(P_aux_m_Base.SI<Watt>(), P_aux_m_ICEOff_dr.SI<Watt>(), P_aux_m_ICEOff_st.SI<Watt>())
			};
			
			var modData = new ModalDataContainer(runData, null, null) {
				WriteModalResults = false
			};

			var container = new MockVehicleContainer() {
				CycleData = new CycleData() { LeftSample = cycleData.Entries.First()},
				ModalData = modData, 
				HasCombustionEngine = true,
				HasElectricMotor = false,
			};
			var engine = new StopStartCombustionEngine(container, engineData);
			

			container.EngineInfo = engine;
			
			var conventionalAux = CreateAuxiliaries(runData.Aux, container);
			var aux = new BusAuxiliariesAdapter(container, runData.BusAuxiliaries, conventionalAux);

			var auxCfg = runData.BusAuxiliaries;
			var electricStorage = auxCfg.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
				? new SimpleBattery(container, auxCfg.ElectricalUserInputsConfig.ElectricStorageCapacity, auxCfg.ElectricalUserInputsConfig.StoredEnergyEfficiency, initialSoC)
				: (ISimpleBattery)new NoBattery(container);
			aux.ElectricStorage = electricStorage;
			engine.Connect(aux.Port());


			return container;
		}

		private static IAuxPort CreateAuxiliaries(IEnumerable<VectoRunData.AuxData> auxDataList,
			IVehicleContainer container)
		{
			var aux = new EngineAuxiliary(container);
			foreach (var auxData in auxDataList) {
				// id's in upper case
				var id = auxData.ID.ToUpper();

				switch (auxData.DemandType) {
					case AuxiliaryDemandType.Constant:
						aux.AddConstant(id, auxData.PowerDemand);
						break;
					default:
						throw new ArgumentOutOfRangeException("AuxiliaryDemandType", auxData.DemandType.ToString());
				}

				container.ModalData?.AddAuxiliary(id);
			}

			return aux;
		}

		private static IList<VectoRunData.AuxData> CreateAuxiliaryData(Watt pwrICEOn, Watt pwrICEOffDriving, Watt pwrICEOffStandstill)
		{
			var baseDemand = pwrICEOffStandstill;
			var stpDemand = pwrICEOffDriving - pwrICEOffStandstill;
			var fanDemand = pwrICEOn - pwrICEOffDriving;

			var auxList = new List<VectoRunData.AuxData>() {
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENG_AUX_MECH_BASE, DemandType = AuxiliaryDemandType.Constant, PowerDemand = baseDemand},
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENG_AUX_MECH_STP, DemandType = AuxiliaryDemandType.Constant, PowerDemand = stpDemand},
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.IDs.ENG_AUX_MECH_FAN, DemandType = AuxiliaryDemandType.Constant, PowerDemand = fanDemand},
			};

			return auxList;
		}

		private static AuxiliaryConfig CreateBusAuxData(AlternatorType alternatorType, VehicleData vehicleData)
		{
			var CompressorMap = "TestData/Integration/BusAuxiliaries/DEFAULT_2-Cylinder_1-Stage_650ccm.ACMP";
			var AverageAirDemand = 0.0.SI<NormLiterPerSecond>();
			var SmartAirCompression = false;
			var GearRatio = 1.0;

			var BatteryEfficiency = 1.0;
			var ESSupplyFromHEVREESS = false;
			//var AlternatorType = VectoCommon.Models.AlternatorType.Smart;
			var ElectricPowerDemand = 0.0.SI<Watt>();
			var MechanicalPowerDemand = 0.0.SI<Watt>();
			var AuxHeaterPower = 0.0.SI<Watt>();
			var AverageHeatingDemand = 0.0.SI<Joule>();

			var retVal = new AuxiliaryConfig() {
				//InputData = auxInputData.BusAuxiliariesData,
				ElectricalUserInputsConfig = new ElectricsUserInputsConfig() {
					PowerNetVoltage = Constants.BusAuxiliaries.ElectricSystem.PowernetVoltage,
					//StoredEnergyEfficiency = Constants.BusAuxiliaries.ElectricSystem.StoredEnergyEfficiency,
					ResultCardIdle = new DummyResultCard(),
					ResultCardOverrun = new DummyResultCard(),
					ResultCardTraction = new DummyResultCard(),
					AlternatorGearEfficiency = Constants.BusAuxiliaries.ElectricSystem.AlternatorGearEfficiency,
					DoorActuationTimeSecond = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond,
					AlternatorMap = new SimpleAlternator(AlternatorEfficiency) {
						Technologies = new List<string>() { "engineering mode" }
					},
					AlternatorType = alternatorType,
					ConnectESToREESS = ESSupplyFromHEVREESS,
					DCDCEfficiency = DCDCEfficiency,
					MaxAlternatorPower = MaxAlternatorPower.SI<Watt>(),
					ElectricStorageCapacity = ElectricStorageCapacity ?? 0.SI<WattSecond>(),
					StoredEnergyEfficiency = BatteryEfficiency,
					ElectricalConsumers = GetElectricConsumers(I_Base.SI<Ampere>(), I_ICEOff_dr.SI<Ampere>(), I_ICEOff_stop.SI<Ampere>()),
				},
				PneumaticAuxillariesConfig = new PneumaticsConsumersDemand() {
					AdBlueInjection = 0.SI<NormLiterPerSecond>(),
					AirControlledSuspension = AverageAirDemand,
					Braking = 0.SI<NormLiterPerKilogram>(),
					BreakingWithKneeling = 0.SI<NormLiterPerKilogramMeter>(),
					DeadVolBlowOuts = 0.SI<PerSecond>(),
					DeadVolume = 0.SI<NormLiter>(),
					NonSmartRegenFractionTotalAirDemand = 0,
					SmartRegenFractionTotalAirDemand = 0,
					OverrunUtilisationForCompressionFraction =
						Constants.BusAuxiliaries.PneumaticConsumersDemands.OverrunUtilisationForCompressionFraction,
					DoorOpening = 0.SI<NormLiter>(),
					StopBrakeActuation = 0.SI<NormLiterPerKilogram>(),
				},
				PneumaticUserInputsConfig = new PneumaticUserInputsConfig() {
					CompressorMap =
						new CompressorMap(CompressorMapReader.Create(VectoCSVFile.Read(CompressorMap), 1.0),
							"engineering mode", ""),
					CompressorGearEfficiency = Constants.BusAuxiliaries.PneumaticUserConfig.CompressorGearEfficiency,
					CompressorGearRatio = GearRatio,
					SmartAirCompression = SmartAirCompression,
					SmartRegeneration = false,
					KneelingHeight = 0.SI<Meter>(),
					AirSuspensionControl = ConsumerTechnology.Pneumatically,
					AdBlueDosing = ConsumerTechnology.Electrically,
					Doors = ConsumerTechnology.Electrically
				},
				Actuations = new Actuations() {
					Braking = 0,
					Kneeling = 0,
					ParkBrakeAndDoors = 0,
					CycleTime = 1.SI<Second>()
				},
				SSMInputs = new SSMEngineeringInputs() {
					MechanicalPower = MechanicalPowerDemand,
					ElectricPower = ElectricPowerDemand,
					AuxHeaterPower = AuxHeaterPower,
					HeatingDemand = AverageHeatingDemand,
					AuxHeaterEfficiency = Constants.BusAuxiliaries.SteadyStateModel.AuxHeaterEfficiency,
					FuelEnergyToHeatToCoolant = Constants.BusAuxiliaries.Heater.FuelEnergyToHeatToCoolant,
					CoolantHeatTransferredToAirCabinHeater =
						Constants.BusAuxiliaries.Heater.CoolantHeatTransferredToAirCabinHeater,
				},
				VehicleData = vehicleData,
			};

			return retVal;
		}

		private static Dictionary<string, ElectricConsumerEntry> GetElectricConsumers(Ampere currentDemand, Ampere currentDemandEngineOffDriving, Ampere currentDemandEngineOffStandstill)
		{
			var retVal = new Dictionary<string, ElectricConsumerEntry>();

			var iBase = currentDemandEngineOffStandstill;
			var iSP = currentDemandEngineOffDriving -
					currentDemandEngineOffStandstill;
			var iFan = currentDemand - currentDemandEngineOffDriving;

			retVal["BaseLoad"] = new ElectricConsumerEntry() {
				Current = iBase,
				BaseVehicle = true
			};
			retVal[Constants.Auxiliaries.IDs.SteeringPump] = new ElectricConsumerEntry() {
				Current = iSP,
				ActiveDuringEngineStopStandstill = false,
			};
			retVal[Constants.Auxiliaries.IDs.Fan] = new ElectricConsumerEntry() {
				Current = iFan,
				ActiveDuringEngineStopStandstill = false,
				ActiveDuringEngineStopDriving = false,
			};
			return retVal;
		}

		

		private static VehicleData CreateVehicleData(Kilogram loading)
		{
			var axles = new List<Axle> {
				new Axle {
					AxleWeightShare = 0.4375,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.375,
					Inertia = 10.83333.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0065,
					TwinTyres = true,
					TyreTestLoad = 52532.55.SI<Newton>()
				},
				new Axle {
					AxleWeightShare = 0.1875,
					Inertia = 21.66667.SI<KilogramSquareMeter>(),
					RollResistanceCoefficient = 0.0055,
					TwinTyres = false,
					TyreTestLoad = 62538.75.SI<Newton>()
				}
			};
			return new VehicleData {
				AirDensity = DeclarationData.AirDensity,
				AxleConfiguration = AxleConfiguration.AxleConfig_6x2,
				CurbMass = 15700.SI<Kilogram>(),
				Loading = loading,
				DynamicTyreRadius = 0.52.SI<Meter>(),
				AxleData = axles,
				SavedInDeclarationMode = false
			};
		}

	}
}