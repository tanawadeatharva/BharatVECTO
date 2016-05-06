using System.Diagnostics;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCommon.Models
{
	[DebuggerDisplay("a: {Acceleration}, dt: {SimulationInterval}, ds: {SimulationDistance}")]
	public struct OperatingPoint
	{
		public MeterPerSquareSecond Acceleration;
		public Meter SimulationDistance;
		public Second SimulationInterval;

		public override string ToString()
		{
			return string.Format("a: {0}, dt: {1}, ds: {2}", Acceleration, SimulationInterval, SimulationDistance);
		}
	}
}