using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class ElectricMotorData
	{
		[ValidateObject]
		public EfficiencyMap EfficiencyMap { get; internal set; }

		[ValidateObject]
		public ElectricMotorFullLoadCurve FullLoadCurve { get; internal set; }

		[SIRange(double.MinValue, double.MaxValue)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[ValidateObject]
		public DragCurve DragCurve { get; internal set; }

		[SIRange(double.MinValue, double.MaxValue)]
		public Watt ContinuousPower { get; internal set; }

		[SIRange(0, double.MaxValue)]
		public Second OverloadTime { get; internal set; }
		
		[SIRange(0, double.MaxValue)]
		public PerSecond ContinuousPowerSpeed { get; internal set; }

		[SIRange(0, 1)]
		public double OverloadRegenerationFactor { get; internal set; }

		public double RatioADC { get; internal set; }
		
		public TransmissionLossMap TransmissionLossMap { get; internal set; }
		
		public double[] RatioPerGear { get; set; }
	}
}