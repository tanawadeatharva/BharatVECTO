using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ShiftStrategy;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

namespace TUGraz.VectoCore.Models.Simulation.Data {
	public class ShiftStrategyParameters
	{
		public MeterPerSecond StartVelocity { get; internal set; }

		public MeterPerSquareSecond StartAcceleration { get; internal set; }

		public Second GearResidenceTime { get; internal set; }


		public PredictionDurationLookup PredictionDurationLookup { get; internal set; }
		public ShareTorque99lLookup ShareTorque99L { get; internal set; }

		public ShareIdleLowLookup ShareIdleLow { get; internal set; }

		public EngineSpeedHighFactorLookup ShareEngineHigh { get; internal set; }

		public AccelerationReserveLookup AccelerationReserveLookup { get; internal set; }

		//% Max.acceptable engine speed for current gear
		//% Low limit, if demanded cardan torque for /constant/ velocity is not above
		//% the max.available cardan torque
		//% Min.ratio of distance between
		//% min.speed where 99 % of the max.engine torque are reached, and
		//% max.speed where 99 % of the max.engine power are reached.
		public double DnT99L_highMin1 { get; internal set; }

		//% Max.acceptable engine speed for current gear
		//% Low limit, if demanded cardan torque for /constant/ velocity is not above
		//% the max.available cardan torque
		//% Max.ratio of distance between
		//% min.speed where 99 % of the max.engine torque are reached, and
		//% max.speed where 99 % of the max.engine power are reached.
		public double DnT99L_highMin2 { get; internal set; }

		public double EngineSpeedHighDriveOffFactor { get; set; }
		public int AllowedGearRangeDown { get; set; }
		public int AllowedGearRangeUp { get; set; }
		public Second LookBackInterval { get; set; }
		public Watt AverageCardanPowerThresholdPropulsion { get; set; }
		public Watt CurrentCardanPowerThresholdPropulsion { get; set; }
		public double TargetSpeedDeviationFactor { get; set; }
		public double RatingFactorCurrentGear { get; set; }
	}
}