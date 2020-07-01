using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Battery {

	public class BatteryData
	{
		[ValidateObject]
		public SOCMap SOCMap { get; internal set; }

		[Range(0, 1)]
		public double MinSOC { get; internal set; }

		[Range(0, 1)]
		public double MaxSOC { get; internal set; }

		[SIRange(0, 1e9)]
		public Ohm InternalResistance { get; internal set; }

		public AmpereSecond Capacity { get; internal set; }

		public Ampere MaxCurrent { get; internal set; }

		public double InitialSoC { get; internal set; }

		//public double TargetSoC { get; internal set; }
	}

	public class SOCMap
	{
		protected SOCMapEntry[] Entries;

		public SOCMap(SOCMapEntry[] entries)
		{
			Entries = entries;
		}

		public Volt Lookup(double soc)
		{
			var idx = FindIndex(soc);
			return VectoMath.Interpolate(Entries[idx - 1].SOC, Entries[idx].SOC, Entries[idx - 1].BatteryVolts,
				Entries[idx].BatteryVolts, soc);
		}

		protected int FindIndex(double soc)
		{
			if (soc < Entries.First().SOC)
			{
				return 1;
			}
			if (soc > Entries.Last().SOC)
			{
				return Entries.Length - 1;

			}
			for (var index = 1; index < Entries.Length; index++)
			{
				if (soc >= Entries[index - 1].SOC && soc <= Entries[index].SOC)
				{
					return index;
				}
			}
			throw new VectoException("soc {0} exceeds battery model data. min: {1} max: {2}", soc, Entries.First().SOC, Entries.Last().SOC);
		}

		public class SOCMapEntry
		{
			[Required, Range(0, 1)] public double SOC;
			[Required, SIRange(0, double.MaxValue)] public Volt BatteryVolts;
		}
	}
}