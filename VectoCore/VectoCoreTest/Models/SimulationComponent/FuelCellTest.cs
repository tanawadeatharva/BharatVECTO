using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent;



[TestFixture]
public class FuelCellTest
{
	public TableData GetTableData(string csvContent)
	{
		return VectoCSVFile.ReadStream(csvContent.ToStream());
	}

	[TestCase(true, TestName="Connector Included")]
	//[TestCase(false, TestName="Connector not Included")] //Considered in ES
    public void EquivalentBattery(bool connectorIncluded)
	{
		EngineeringDataAdapter engAdapter = new EngineeringDataAdapter();
		Mock<IElectricStorageSystemEngineeringInputData> batMock =
			new Mock<IElectricStorageSystemEngineeringInputData>();

		var storageElement = new Mock<IElectricStorageEngineeringInputData>();
		var batPack = new Mock<IBatteryPackEngineeringInputData>();

		Mock<IFuelCellSystemEngineeringInputData> fuelCellSystemMock = new Mock<IFuelCellSystemEngineeringInputData>();
		Mock<IFuelCellComponentEngineeringInputData> fuelCellComponentMock = new Mock<IFuelCellComponentEngineeringInputData>();

        SetMockData(batMock, storageElement, batPack, fuelCellSystemMock, fuelCellComponentMock);
		batPack.SetupGet(b => b.ConnectorsSubsystemsIncluded).Returns(connectorIncluded);


		var pevBatSystemData = engAdapter.CreateBatteryData(batMock.Object, 0.5);

		var pevBatSystem = new BatterySystem(null, pevBatSystemData);
		var maxDischargePower = pevBatSystem.MaxDischargePower(1.SI<Second>());


		var preBatSystemData = engAdapter.CreateFuelCellPreProcessingBattery(fuelCellSystemMock.Object, pevBatSystemData);

		var preBatSystem = new BatterySystem(null, preBatSystemData);


		Assert.AreEqual(preBatSystem.MaxChargePower(1.SI<Second>()), pevBatSystem.MaxChargePower(1.SI<Second>())); //Fuel cell cannnot be charged

		preBatSystem.Initialize(0.5);
		pevBatSystem.Initialize(0.5);
		preBatSystem.PreviousState.PulseDuration = 1.SI<Second>();
		preBatSystem.PreviousState.PowerDemand = -1.SI<Watt>();
		pevBatSystem.PreviousState.PulseDuration = 1.SI<Second>();
		pevBatSystem.PreviousState.PowerDemand = -1.SI<Watt>();


		Assert.Less(pevBatSystem.MaxDischargePower(1.SI<Second>()), (0.SI<Watt>()));

		Assert.IsTrue(pevBatSystem.MaxDischargePower(1.SI<Second>()).IsRelativeEqual(preBatSystem.MaxDischargePower(1.SI<Second>()) + fuelCellComponentMock.Object.MaxElectricPower, 1E-06));

		var internalResistance = preBatSystem.Batteries.First(x => x.Key == FuelCellSystemData.FuelCellBatID).Value.InternalResistance(1.SI<Second>());
		Assert.IsTrue(internalResistance.IsEqual(0), $"Expected 0 was {internalResistance}");
	}

	private void SetMockData(Mock<IElectricStorageSystemEngineeringInputData> batMock, Mock<IElectricStorageEngineeringInputData> storageElement, Mock<IBatteryPackEngineeringInputData> batPack, Mock<IFuelCellSystemEngineeringInputData> fuelCellSystemMock,
		Mock<IFuelCellComponentEngineeringInputData> fuelCellComponentMock)
	{
		batMock.SetupGet(b => b.ElectricStorageElements).Returns(new List<IElectricStorageEngineeringInputData>()
			{ storageElement.Object });
		storageElement.SetupGet(s => s.Count).Returns(1);
		storageElement.SetupGet(s => s.StringId).Returns(1);
		storageElement.SetupGet(s => s.REESSPack).Returns(batPack.Object);


		batPack.SetupGet(r => r.MaxSOC).Returns(() => 0.8);
		batPack.SetupGet(r => r.MinSOC).Returns(() => 0.2);

		batPack.SetupGet(r => r.Capacity).Returns((110 * 3600).SI<AmpereSecond>());


		batPack.SetupGet(r => r.StorageType).Returns(REESSType.Battery);
		batPack.SetupGet(r => r.ConnectorsSubsystemsIncluded).Returns(true);

		//TODO FILL MAPS
		batPack.SetupGet(r => r.InternalResistanceCurve).Returns(
			GetTableData(
				"SoC, Ri\r\n" +
				"0,  0.04\r\n" +
				"100,  0.04"
			)
		);
		batPack.SetupGet(r => r.MaxCurrentMap).Returns(GetTableData(
			"SOC, I_charge, I_discharge\r\n" +
			"0, 1620, 1620\r\n" +
			"100, 1620, 1620"
		));
		batPack.SetupGet(r => r.VoltageCurve).Returns(GetTableData(
			"SOC, V\r\n" +
			"0, 673.5\r\n" +
			"10, 700.2\r\n" +
			"20, 715.4\r\n" +
			"30, 723.6\r\n" +
			"40, 727.7\r\n" +
			"50, 730.0\r\n" +
			"60, 731.6\r\n" +
			"70, 733.8\r\n" +
			"80, 737.1\r\n" +
			"90, 742.2\r\n" +
			"100, 750.2"
		));


		fuelCellSystemMock.SetupGet(fcs => fcs.FuelCellComponents).Returns(() =>
			new List<FuelCellComponentEntry<IFuelCellComponentEngineeringInputData>>() {
				new FuelCellComponentEntry<IFuelCellComponentEngineeringInputData>() {
					Count = 1,
					FuelCellComponent = fuelCellComponentMock.Object,
				}
			});

		fuelCellComponentMock.SetupGet(fcC => fcC.MaxElectricPower).Returns((100 * 1000).SI<Watt>());
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