using System.IO;
using System.Linq;
using NUnit.Framework;
using TUGraz.VECTO;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.JSON;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Tests.Utils;
using Battery = TUGraz.VectoCore.Models.SimulationComponent.Impl.Battery;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent
{
	[TestFixture]
	public class BatteryTest
	{
		public const string componentFile = @"TestData\Hybrids\Battery\GenericBattery.vbat";

		[OneTimeSetUp]
		public void RunBeforeAnyTests()
		{
			Directory.SetCurrentDirectory(TestContext.CurrentContext.TestDirectory);
		}

		[
		TestCase(0.5, 0.5, -500, 0.499985523),
		TestCase(0.5, 1, -14000, 0.499175514),
		TestCase(0.5, 1, 7000, 0.5004016980),
		TestCase(0.35, 0.5, -200, 0.349994175),
		TestCase(0.35, 0.5, -16000, 0.3495245545),
		TestCase(0.75, 0.5, -300, 0.7499913702),
		TestCase(0.75, 0.5, -14500, 0.7495755113),
			]

		public void BatteryRequestTest(double initialSoC, double simInterval, double powerDemand, double expectedSoC)
		{
			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false) ;
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			var modData = new MockModalDataContainer();
			bat.Initialize(initialSoC);

			//Assert.AreEqual(3.2*200, bat.Voltage.Value());

			var absTime = 0.SI<Second>();
			var dt = simInterval.SI<Second>();
			var response = bat.Request(absTime, dt, powerDemand.SI<Watt>());
			Assert.IsInstanceOf<RESSResponseSuccess>(response);
			bat.CommitSimulationStep(absTime, dt, modData);

			Assert.AreEqual(expectedSoC, bat.StateOfCharge, 1e-9);
		}

		[TestCase(0.5, 0.5, -500000, -169875, 70125),
		 TestCase(0.35, 0.5, -500000, -168375, 70125),
		 TestCase(0.75, 0.5, -500000, -171375, 70125),
		 TestCase(0.2001, 3, -500000, -563.00328, 0.40392),
		 TestCase(0.2001, 1, -500000, -1686.58632, 3.63528),]
		public void BatteryOverloadTestNoAux(double initialSoC, double dt, double powerDemand,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);

			bat.Initialize(initialSoC);

			var response = bat.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<RESSUnderloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.MaxDischargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.LossPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand, response.PowerDemand.Value(), 1e-2);
		}

		[TestCase(0.5, 0.5, -500000, 10000, -169875, 70125),
		 TestCase(0.35, 0.5, -500000, 10000, -168375, 70125),
		 TestCase(0.75, 0.5, -500000, 10000, -171375, 70125),
		 TestCase(0.2001, 3, -500000, 10000, -563.00328, 0.40392),
		 TestCase(0.2001, 1, -500000, 10000, -1686.58632, 3.63528),]
		public void BatteryOverloadTestAux(double initialSoC, double dt, double powerDemand, double auxPower,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var es = new ElectricSystem(container);
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(auxPower.SI<Watt>()));
			bat.Initialize(initialSoC);

			var response = es.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<ElectricSystemUnderloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.RESSResponse.MaxDischargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.RESSResponse.LossPower.Value(), 1e-2);
			Assert.AreEqual(auxPower, response.AuxPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand - auxPower, response.RESSResponse.PowerDemand.Value(), 1e-2);
		}

		[TestCase(0.5, 0.5, 500000, 310125, 70125),
		 TestCase(0.35, 0.5, 500000, 308625, 70125),
		 TestCase(0.75, 0.5, 500000, 311625, 70125),
		 TestCase(0.2001, 3, 500000, 304878, 70125),
		 TestCase(0.7999, 1, 500000, 1747.82448, 3.63528),
		 TestCase(0.7999, 3, 500000, 581.80032, 0.40392)]
		public void BatteryUnderloadTestNoAux(double initialSoC, double dt, double powerDemand,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);

			bat.Initialize(initialSoC);

			var response = bat.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<RESSOverloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.MaxChargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.LossPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand, response.PowerDemand.Value(), 1e-2);
		}

		[TestCase(0.5, 0.5, 500000, 10000, 310125, 70125),
		 TestCase(0.35, 0.5, 500000, 10000, 308625, 70125),
		 TestCase(0.75, 0.5, 500000, 10000, 311625, 70125),
		 TestCase(0.2001, 3, 500000, 10000, 304878, 70125),
		 TestCase(0.7999, 1, 500000, 10000, 1747.82448, 3.63528),
		 TestCase(0.7999, 3, 500000, 10000, 581.80032, 0.40392)]
		public void BatteryUnderloadTestAux(double initialSoC, double dt, double powerDemand, double auxPower,
			double maxPowerDischarge, double battLoss)
		{

			var inputData = JSONInputDataFactory.ReadREESSData(componentFile, false);
			Assert.NotNull(inputData);

			var dao = new EngineeringDataAdapter();
			var tmp = new MockBatteryInputData()
			{
				REESSPack = inputData,
			};
			var batteryData = dao.CreateBatteryData(tmp, 0.8);

			var container = new MockVehicleContainer();
			var bat = new Battery(container, batteryData.Batteries.First().Item2);
			var es = new ElectricSystem(container);
			es.Connect(bat);
			es.Connect(new MockElectricConsumer(auxPower.SI<Watt>()));
			bat.Initialize(initialSoC);

			var response = es.Request(0.SI<Second>(), dt.SI<Second>(), powerDemand.SI<Watt>());
			Assert.IsInstanceOf<ElectricSystemOverloadResponse>(response);

			Assert.AreEqual(maxPowerDischarge, response.RESSResponse.MaxChargePower.Value(), 1e-2);
			Assert.AreEqual(battLoss, response.RESSResponse.LossPower.Value(), 1e-2);
			Assert.AreEqual(auxPower, response.AuxPower.Value(), 1e-2);
			Assert.AreEqual(powerDemand - auxPower, response.RESSResponse.PowerDemand.Value(), 1e-2);
		}
	}
}