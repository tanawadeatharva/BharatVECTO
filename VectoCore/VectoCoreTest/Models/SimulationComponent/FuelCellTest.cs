using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent;



[TestFixture]
public class FuelCellTest
{


	[Test]
	public void EquivalentBattery()
	{
		EngineeringDataAdapter engAdapter = new EngineeringDataAdapter();
		Mock<IElectricStorageSystemEngineeringInputData> batMock =
			new Mock<IElectricStorageSystemEngineeringInputData>();

		Mock<IFuelCellSystemEngineeringInputData> fuelCellSystemMock = new Mock<IFuelCellSystemEngineeringInputData>();



		var pevBatSystem = engAdapter.CreateBatteryData(batMock.Object, 0.5);

		var preBatSystem = engAdapter.CreateFuelCellPreProcessingBattery(fuelCellSystemMock.Object, pevBatSystem);






	}


	[TestCase(100, 100, 1, 0, 100, TestName = "Constant")]
	[TestCase(100, 1, 1, 5, 95, TestName = "Reduce Power (Limited)")]
	[TestCase(100, 150, 1, 5, 105, TestName="Increasing (Limited)")]
	[TestCase(100, 110, 1, 50, 110, TestName="Increasing unlimited")]
	public void GradientPowerChange(double previous, double current, double dt, double gradientPowerChange, double expected)
	{
		var result = FuelCellSystem.GetLimitedPower(previous.SI<Watt>(), current.SI<Watt>(), dt.SI<Second>(),
			gradientPowerChange.SI<WattPerSecond>());

		Assert.AreEqual(expected, result.Value());
	}


	
}