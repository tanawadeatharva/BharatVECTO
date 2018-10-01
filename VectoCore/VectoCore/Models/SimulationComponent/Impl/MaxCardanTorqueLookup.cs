using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class MaxCardanTorqueLookup 
	{
		public Dictionary<uint, List<KeyValuePair<PerSecond, NewtonMeter>>> Data { protected get; set; }

		public NewtonMeter Lookup(uint gear, PerSecond engineSpeed)
		{
			return Data[gear].Interpolate(x => x.Key.Value(), y => y.Value, engineSpeed.Value());
		}
	}
}