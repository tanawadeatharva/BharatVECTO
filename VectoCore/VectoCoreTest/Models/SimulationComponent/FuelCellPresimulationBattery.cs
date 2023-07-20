using Moq;
using NUnit.Framework;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;

namespace TUGraz.VectoCore.Tests.Models.SimulationComponent;



[TestFixture]
public class FuelCellPresimulationBattery
{


	[Test]
	public void EquivalentBattery()
	{
		EngineeringDataAdapter engAdapter = new EngineeringDataAdapter();
		Mock<IElectricStorageSystemEngineeringInputData> batMock =
			new Mock<IElectricStorageSystemEngineeringInputData>();



		var pevBatSystem = engAdapter.CreateBatteryData(batMock.Object, 0.5);






	}


	
}