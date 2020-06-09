using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class ElectricMotorData
	{
		[ValidateObject]
		public EfficiencyMap EfficiencyMap { get; internal set; }

		public ElectricFullLoadCurve FullLoadCurve { get; internal set; }

		public KilogramSquareMeter Inertia { get; internal set; }

		public DragCurve DragCurve { get; internal set; }
	}
}