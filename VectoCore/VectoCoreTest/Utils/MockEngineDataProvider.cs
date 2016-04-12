using System.Data;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	public class MockEngineDataProvider : IEngineEngineeringInputData
	{
		public bool SavedInDeclarationMode { get; set; }
		public string Vendor { get; set; }
		public string ModelName { get; set; }
		public string Creator { get; set; }
		public string Date { get; set; }
		public string TypeId { get; set; }
		public string DigestValue { get; set; }
		public IntegrityStatus IntegrityStatus { get; set; }
		public CubicMeter Displacement { get; set; }
		public PerSecond IdleSpeed { get; set; }
		public double WHTCMotorway { get; set; }
		public double WHTCRural { get; set; }
		public double WHTCUrban { get; set; }
		public DataTable FuelConsumptionMap { get; set; }
		public DataTable FullLoadCurve { get; set; }
		public KilogramSquareMeter Inertia { get; set; }
	}
}