using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ShiftStrategy;

namespace TUGraz.VectoCore.Models.Simulation.Data {
	public class ShiftStrategyParameters
	{
		public MeterPerSecond StartVelocity { get; internal set; }

		public MeterPerSquareSecond StartAcceleration { get; internal set; }

		public Second GearResidenceTime { get; internal set; }


		public IPredictionDurationLookup PredictionDurationLookup { get; internal set; }
		public IShareTorque99lLookup ShareTorque99L { get; internal set; }

		public IShareIdleLowLookup ShareIdleLow { get; internal set; }

		public IEngineSpeedHighFactorLookup ShareEngineHigh { get; internal set; }

		public IAccelerationReserveLookup AccelerationReserveLookup { get; internal set; }

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
		public Second DriverAccelerationLookBackInterval { get; set; }
		public Watt AverageCardanPowerThresholdPropulsion { get; set; }
		public Watt CurrentCardanPowerThresholdPropulsion { get; set; }
		public double TargetSpeedDeviationFactor { get; set; }
		public double RatingFactorCurrentGear { get; set; }
		public MeterPerSquareSecond DriverAccelerationThresholdLow { get; set; }
		public double RatioEarlyDownshiftFC { get; set; }
		public double RatioEarlyUpshiftFC { get; set; }

		public int AllowedGearRangeFC { get; set; }

		public double AccelerationFactor { get; set; }

		public double VelocityDropFactor { get; internal set; }

		// Shift Lines for Voith proposed shift strategy
		public TableData GearshiftLines { get; set; }

		public IEnumerable<Tuple<double, double>> LoadstageThresholds { get; set; }

		public PerSecond MinEngineSpeedPostUpshift { get; set; }
		public Second ATLookAheadTime { get; set; }
		public double[] LoadStageThresoldsUp { get; set; }
		public double[] LoadStageThresoldsDown { get; set; }
		public double[][] ShiftSpeedsTCToLocked { get; set; }
	}
}