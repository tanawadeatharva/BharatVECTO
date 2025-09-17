using System;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data
{
	public class AxlePowertrainData
	{
		[Required, SIRange(0, 4)]
		public int AxleNumber { get; internal set; }

		[Required]
		public VectoSimulationJobType Type { get; internal set; }

		[ValidateObject]
		public GearboxData GearboxData { get; internal set; }

		[ValidateObject]
		public AxleGearData AxleGearData { get; internal set; }

		[ValidateObject]
		public AngledriveData AngledriveData { get; internal set; }

		[ValidateObject]
		public RetarderData Retarder { get; internal set; }

		[ValidateObject]
		public PTOData PTO { get; internal set; }

		public ShiftStrategyParameters GearshiftParameters { get; internal set; }

		public Tuple<PowertrainPosition, ElectricMotorData> ElectricMachineData { get; internal set; }
	}
}
