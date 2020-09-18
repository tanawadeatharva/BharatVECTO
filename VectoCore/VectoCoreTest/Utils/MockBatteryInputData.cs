using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.Tests.Utils {
	public class MockBatteryInputData : IElectricStorageEngineeringInputData
	{
		public IREESSPackInputData REESSPack { get; set; }
		public int Count { get; set; }
	}
}