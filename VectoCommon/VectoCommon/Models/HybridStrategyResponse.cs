using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models {
	public class HybridStrategyResponse
	{
		public Dictionary<PowertrainPosition, NewtonMeter> MechanicalAssistPower;
		public bool ShiftRequired { get; set; }
		public uint NextGear { get; set; }
		public bool GearboxInNeutral { get; set; }
		public bool CombustionEngineOn { get; set; }
	}
}