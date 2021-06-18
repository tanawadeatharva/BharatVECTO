using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class ElectricMotorData
	{
		[SIRange(double.MinValue, double.MaxValue)]
		public KilogramSquareMeter Inertia { get; internal set; }

		[SIRange(double.MinValue, double.MaxValue)]
		public NewtonMeter ContinuousTorque { get; internal set; }

		[SIRange(0, double.MaxValue)]
		public PerSecond ContinuousTorqueSpeed { get; internal set; }

		[SIRange(double.MinValue, double.MaxValue)]
		public NewtonMeter OverloadTorque { get; set; }

		[SIRange(0, double.MaxValue)]
		public PerSecond OverloadTestSpeed { get; set; }


		[SIRange(0, double.MaxValue)]
		public Second OverloadTime { get; internal set; }
		
		[SIRange(0, 1)]
		public double OverloadRegenerationFactor { get; internal set; }

		public double RatioADC { get; internal set; }
		
		public TransmissionLossMap TransmissionLossMap { get; internal set; }
		
		public double[] RatioPerGear { get; set; }

		[ValidateObject]
		public VoltageLevelData EfficiencyData { get; internal set; }
	}

	public class VoltageLevelData
	{
		private PerSecond _maxSpeed;

		public IList<ElectricMotorVoltageLevelData> VoltageLevels { get; internal set; }


		public PerSecond MaxSpeed
		{
			get
			{
				return _maxSpeed ?? (_maxSpeed = VoltageLevels
					.Min(v => v.FullLoadCurve.FullLoadEntries.MaxBy(x => x.MotorSpeed).MotorSpeed));
			}
		}

		public NewtonMeter LookupDragTorque(Volt voltage, PerSecond avgSpeed)
		{
			return null;
		}

		public NewtonMeter EfficiencyMapLookupTorque(Volt voltage, Watt electricPower, PerSecond avgSpeed, NewtonMeter maxEmTorque)
		{
			return null;
		}

		public EfficiencyMap.EfficiencyResult LookupElectricPower(Volt voltage, PerSecond avgSpeed, NewtonMeter maxTorque, bool allowExtrapolation = false)
		{
			return null;
		}

		public NewtonMeter FullGenerationTorque(Volt voltage, PerSecond avgSpeed)
		{
			return null;
		}

		public NewtonMeter FullLoadDriveTorque(Volt voltage, PerSecond avgSpeed)
		{
			return null;
		}
	}

	public class ElectricMotorVoltageLevelData
	{
		[SIRange(0, double.MaxValue)]
		public Volt Voltage { get; internal set; }

		[ValidateObject]
		public ElectricMotorFullLoadCurve FullLoadCurve { get; internal set; }

		[ValidateObject]
		public DragCurve DragCurve { get; internal set; }

		[ValidateObject]
		public EfficiencyMap EfficiencyMap { get; internal set; }

		
	}
}