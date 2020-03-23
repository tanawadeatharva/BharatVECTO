using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockBatteryInputData : IElectricStorageEngineeringInputData
	{
		IBatteryPackDeclarationInputData IElectricStorageDeclarationInputData.BatteryPack => BatteryPack;

		public IBatteryPackEngineeringInputData BatteryPack { get; set; }
		public int Count { get; set; }
	}
}