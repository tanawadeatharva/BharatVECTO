using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Castle.Core.Resource;
using Moq;
using NLog.LayoutRenderers;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.DataProvider;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;

namespace TUGraz.VectoCore.Tests.Models.Declaration.DataAdapter;

public class BatteryDataAdapterTest
{
	private ElectricStorageAdapter _electricStorageAdapter = new ElectricStorageAdapter();



	[OneTimeSetUp]
	public void OneTimeSetup()
	{

	}

	[TestCase(null, null, 0.0725, 0.9275, 0.8550)]
	public void PEVBatteryDataAdapterTest(
		double inputMinSoc,
		double inputMaxSoc,
		double expectedMinSoc,
		double expectedMaxSoc,
		double usableSocRange)
	{
		var elStorage = CreateElectricStorage(null, null);
		var inputData = CreateElectricStorageSystem(elStorage.Object);


		var batteryData = _electricStorageAdapter.CreateBatteryData(inputData.Object);
		Assert.AreEqual(1, batteryData.Batteries.Count);
		
		var battery = batteryData.Batteries.FirstOrDefault().Item2;
		
		Assert.IsTrue(battery.MinSOC.IsEqual(expectedMinSoc));
		Assert.IsTrue(battery.MaxSOC.IsEqual(expectedMaxSoc ));

		Assert.IsTrue(usableSocRange.IsEqual(battery.GetUsableSocRange()), $"Invalid {nameof(usableSocRange)} expected {usableSocRange} got {battery.GetUsableSocRange()}");

	}

	private static Mock<IElectricStorageDeclarationInputData> CreateElectricStorage(double? minSoc, double? maxSoc)
	{
		var elStorage = new Mock<IElectricStorageDeclarationInputData>();
		var ressPack = new Mock<IBatteryPackDeclarationInputData>();

		ressPack.Setup(m => m.Capacity).Returns((1000).SI<AmpereSecond>());

		ressPack.Setup(m => m.MinSOC).Returns(() => minSoc);
		ressPack.Setup(m => m.MaxSOC).Returns(() => maxSoc);
		ressPack.Setup(m => m.MaxCurrentMap).Returns(
			GetMockTableData(new[] {
				new[]{"0.0", "0.0", "0.0"}, 
				new[]{"0.0", "0.0", "0.0"},
				new[]{"0.0", "0.0", "0.0"}
			}));

		ressPack.Setup(m => m.InternalResistanceCurve).Returns(
			GetMockTableData(new[] {
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"}
			}));
		ressPack.Setup(m => m.VoltageCurve).Returns(
			GetMockTableData(new[] {
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"},
				new[]{"0.0", "0.0"}
			}));

		elStorage.Setup(m => m.REESSPack).Returns(() => ressPack.Object);
		return elStorage;
	}

	[TestCase]
	public void OVCHevBatteryDataAdapterTest()
	{

	}

	[TestCase]
	public void NonOVCHevBatteryDataAdapterTest()
	{

	}

	Mock<IElectricStorageSystemDeclarationInputData> CreateElectricStorageSystem(params IElectricStorageDeclarationInputData[] elStorageInputData)
	{
		var result = new Mock<IElectricStorageSystemDeclarationInputData>();
		result.Setup((m) => m.ElectricStorageElements)
			.Returns(new List<IElectricStorageDeclarationInputData>(elStorageInputData));

		return result;
	}

	static TableData GetMockTableData(string[][] values)
	{
		var result = new TableData();


		foreach(var col in values.First()) {
			result.Columns.Add(new DataColumn());
		}

		foreach (var row in values) {
			result.Rows.Add(result.NewRow().ItemArray = row);
		}
		return result;

	}


}

internal static class BatteryDataExtension
{
	public static double GetUsableSocRange(this BatteryData battery)
	{
		return battery.MaxSOC - battery.MinSOC;
	}
}
