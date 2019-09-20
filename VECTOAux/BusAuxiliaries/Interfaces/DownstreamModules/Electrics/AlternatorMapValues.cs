namespace TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.Electrics
{
	// Originally was going to hold more than one value type.
	public struct AlternatorMapValues
	{
		public readonly double Efficiency;

		public AlternatorMapValues(double efficiency)
		{
			this.Efficiency = efficiency;
		}
	}
}
