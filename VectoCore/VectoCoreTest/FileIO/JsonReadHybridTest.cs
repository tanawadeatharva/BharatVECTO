using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.FileIO;
using TUGraz.VectoCore.Tests.Integration.Declaration;

namespace TUGraz.VectoCore.Tests.FileIO
{
	[TestFixture]
	public class JsonReadHybridTest
	{

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[TestCase()]
		public void TestReadBatteryPack()
		{
			var inputProvider = JSONInputDataFactory.ReadBatteryData(@"TestData\Hybrids\Battery\GenericBattery.vbat", false);

			Assert.AreEqual(7.5.SI(Unit.SI.Ampere.Hour), inputProvider.Capacity);
			
			var soc = inputProvider.VoltageCurve;
			Assert.AreEqual("0", soc.Rows[0][BatterySOCReader.Fields.StateOfCharge]);
			Assert.AreEqual("590", soc.Rows[0][BatterySOCReader.Fields.BatteryVoltage]);

			Assert.AreEqual(0.20, inputProvider.MinSOC);
			Assert.AreEqual(0.80, inputProvider.MaxSOC);
			Assert.AreEqual(50, inputProvider.MaxCurrentFactor);

			var socMap = BatterySOCReader.Create(soc);

			Assert.AreEqual(590, socMap.Lookup(0).Value());
			Assert.AreEqual(658, socMap.Lookup(1).Value());
			Assert.AreEqual(640, socMap.Lookup(0.5).Value());

			Assert.AreEqual(639, socMap.Lookup(0.45).Value());
		}

		[TestCase()]
		public void TestReadElectricMotor()
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			Assert.AreEqual(0.15, inputProvider.Inertia.Value(), 1e-6);

			var fld = inputProvider.FullLoadCurve;
			Assert.AreEqual("0", fld.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("401.07", fld.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-401.07", fld.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMap = ElectricFullLoadCurveReader.Create(fld, 1, 1, 1);
			Assert.AreEqual(-401.07, fldMap.FullLoadDriveTorque(0.RPMtoRad()).Value());
			Assert.AreEqual(401.07, fldMap.FullGenerationTorque(0.RPMtoRad()).Value());

			var pwr = inputProvider.EfficiencyMap;
			Assert.AreEqual("0", pwr.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwr.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("9.8449", pwr.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMap = ElectricMotorMapReader.Create(pwr, 1, 1, 1);
			Assert.AreEqual(-10171.0, pwrMap.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value());

			Assert.AreEqual(-20430.186, pwrMap.LookupElectricPower(120.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

        }

        [TestCase()]
		public void TestReadElectricMotorAggregation()
		{
			var inputProvider =
				JSONInputDataFactory.ReadElectricMotorData(@"TestData\Hybrids\ElectricMotor\GenericEMotor.vem", false);

			Assert.AreEqual(0.15, inputProvider.Inertia.Value(), 1e-6);

			var fld = inputProvider.FullLoadCurve;
			Assert.AreEqual("0", fld.Rows[0][ElectricFullLoadCurveReader.Fields.MotorSpeed]);
			Assert.AreEqual("401.07", fld.Rows[0][ElectricFullLoadCurveReader.Fields.DrivingTorque]);
			Assert.AreEqual("-401.07", fld.Rows[0][ElectricFullLoadCurveReader.Fields.GenerationTorque]);

			var fldMap = ElectricFullLoadCurveReader.Create(fld, 22.6, 2, 0.97);
			Assert.AreEqual(-17584.51308 , fldMap.FullLoadDriveTorque(0.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(18689.03505, fldMap.FullGenerationTorque(0.RPMtoRad()).Value(), 1e-3);

			Assert.AreEqual(-17584.51308, fldMap.FullLoadDriveTorque(50.RPMtoRad()).Value(), 1e-3);
			Assert.AreEqual(18689.03505, fldMap.FullGenerationTorque(50.RPMtoRad()).Value(), 1e-3);

            var pwr = inputProvider.EfficiencyMap;
			Assert.AreEqual("0", pwr.Rows[0][ElectricMotorMapReader.Fields.MotorSpeed]);
			Assert.AreEqual("-800", pwr.Rows[0][ElectricMotorMapReader.Fields.Torque]);
			Assert.AreEqual("9.8449", pwr.Rows[0][ElectricMotorMapReader.Fields.PowerElectrical]);

			var pwrMap = ElectricMotorMapReader.Create(pwr, 22.6, 2, 0.97);
			Assert.AreEqual(-146.469, pwrMap.LookupElectricPower(-0.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

			Assert.AreEqual(-20773.997, pwrMap.LookupElectricPower(120.RPMtoRad(), -800.SI<NewtonMeter>()).ElectricalPower.Value(), 1e-3);

        }

        [TestCase()]
		public void TestReadHybridVehicle()
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(@"TestData\Hybrids\GenericVehicle_Group2_P2\Class2_RigidTruck_ParHyb_ENG.vecto");

			var engineering = inputProvider as IEngineeringInputDataProvider;

			Assert.NotNull(engineering);
			Assert.AreEqual(0.8, engineering.JobInputData.Vehicle.InitialSOC);

			var bat = engineering.JobInputData.Vehicle.Components.ElectricStorage;

			var ri = BatteryInternalResistanceReader.Create(bat.BatteryPack.InternalResistanceCurve, 1);
			Assert.NotNull(bat);
			Assert.AreEqual(2, bat.Count);
			Assert.AreEqual(50, bat.BatteryPack.MaxCurrentFactor);
			Assert.AreEqual(0.04, ri.Lookup(0.5).Value());

			var em = engineering.JobInputData.Vehicle.Components.ElectricMachines;

			Assert.NotNull(em);
			Assert.AreEqual(1, em.Entries.Count);

			Assert.AreEqual(PowertrainPosition.HybridP2, em.Entries[0].Position);

			Assert.AreEqual(0.2, em.Entries[0].ElectricMachine.Inertia.Value());

		}

		[TestCase()]
		public void TestCreateHybridPowertrain()
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(@"TestData\Hybrids\GenericVehicle_Group2_P2\Class2_RigidTruck_ParHyb_ENG.vecto");

			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, null);

			var sumContainer = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			//var run = factory.SimulationRuns().ToArray()[0];

			//Assert.NotNull(run);

			//var pt = run.GetContainer();

			//Assert.NotNull(pt);

			//var port = run.GetContainer().GetCycleOutPort();
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}

		[TestCase()]
		public void TestCreateBatteryElectricPowertrain()
		{
			var inputProvider = JSONInputDataFactory.ReadJsonJob(@"TestData\BatteryElectric\GenericVehicleB4\BEV_ENG.vecto");

			var factory = new SimulatorFactory(ExecutionMode.Engineering, inputProvider, null);

			var sumContainer = new SummaryDataContainer(null);
			var jobContainer = new JobContainer(sumContainer);

			factory.SumData = sumContainer;

			//var run = factory.SimulationRuns().ToArray()[0];

			//Assert.NotNull(run);

			//var pt = run.GetContainer();

			//Assert.NotNull(pt);

			//var port = run.GetContainer().GetCycleOutPort();
			jobContainer.AddRuns(factory);
			jobContainer.Execute();
			jobContainer.WaitFinished();
			Assert.IsTrue(jobContainer.GetProgress().All(x => x.Value.Success));
		}
    }
}