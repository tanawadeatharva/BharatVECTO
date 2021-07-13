using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Data.Battery {

	public class BatterySystemData
	{
		public BatterySystemData()
		{
			Batteries = new List<Tuple<int, BatteryData>>();
		}

		public List<Tuple<int, BatteryData>> Batteries { get; internal set; }

		public double InitialSoC { get; internal set; }
		public AmpereSecond Capacity {
			get { throw new NotImplementedException();}
		}
	}

	public class BatteryData
	{
		[ValidateObject]
		public SOCMap SOCMap { get; internal set; }

		[Range(0, 1)]
		public double MinSOC { get; internal set; }

		[Range(0, 1)]
		public double MaxSOC { get; internal set; }

		[SIRange(0, 1e9)]
		public InternalResistanceMap InternalResistance { get; internal set; }

		public AmpereSecond Capacity { get; internal set; }

		public MaxCurrentMap MaxCurrent { get; internal set; }

	}

	public class SuperCapData
	{
		public Farad Capacity { get; internal set; }

		public Ohm InternalResistance { get; internal set; }

		public Volt MinVoltage { get; internal set; }

		public Volt MaxVoltage { get; internal set; }
		public double InitialSoC { get; internal set; }
		public Ampere MaxCurrentCharge { get; internal set; }

		public Ampere MaxCurrentDischarge { get; internal set; }
	}

	public class SOCMap
	{
		protected internal SOCMapEntry[] Entries;

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

	public class InternalResistanceMap
	{
		protected internal InternalResistanceMapEntry[] Entries;

		public InternalResistanceMap(InternalResistanceMapEntry[] entries)
		{
			Entries = entries;
		}

		public Ohm Lookup(double SoC)
		{
			var idx = FindIndex(SoC);
			return VectoMath.Interpolate(Entries[idx - 1].SoC, Entries[idx].SoC, Entries[idx - 1].Resistance,
				Entries[idx].Resistance, SoC);
        }

		protected int FindIndex(double soc)
		{
			if (soc < Entries.First().SoC) {
				return 1;
			}

			if (soc > Entries.Last().SoC) {
				return Entries.Length - 1;

			}

			for (var index = 1; index < Entries.Length; index++) {
				if (soc >= Entries[index - 1].SoC && soc <= Entries[index].SoC) {
					return index;
				}
			}

			throw new VectoException("soc {0} exceeds battery model data. min: {1} max: {2}", soc, Entries.First().SoC,
				Entries.Last().SoC);
		}

		public class InternalResistanceMapEntry
		{
			[Required, Range(0, 1)] public double SoC;
			[Required, SIRange(0, 1e6)] public Ohm Resistance;
		}

	}


	public class MaxCurrentMap
	{
		protected internal MaxCurrentEntry[] Entries;

		public MaxCurrentMap(MaxCurrentEntry[] entries)
		{
			Entries = entries;
		}

		public Ampere LookupMaxChargeCurrent(double soc)
		{
			var idx = FindIndex(soc);
			return VectoMath.Interpolate(Entries[idx - 1].SoC, Entries[idx].SoC, Entries[idx - 1].MaxChargeCurrent,
				Entries[idx].MaxChargeCurrent, soc);
		}

		public Ampere LookupMaxDischargeCurrent(double soc)
		{
			var idx = FindIndex(soc);
			return VectoMath.Interpolate(Entries[idx - 1].SoC, Entries[idx].SoC, Entries[idx - 1].MaxDischargeCurrent,
				Entries[idx].MaxDischargeCurrent, soc);
		}

		protected int FindIndex(double soc)
		{
			if (soc < Entries.First().SoC) {
				return 1;
			}

			if (soc > Entries.Last().SoC) {
				return Entries.Length - 1;

			}

			for (var index = 1; index < Entries.Length; index++) {
				if (soc >= Entries[index - 1].SoC && soc <= Entries[index].SoC) {
					return index;
				}
			}

			throw new VectoException("soc {0} exceeds battery model data. min: {1} max: {2}", soc, Entries.First().SoC,
				Entries.Last().SoC);
		}

		public class MaxCurrentEntry
		{
			[Required, SIRange(0, 1)] public double SoC;

			[Required, SIRange(0, double.MaxValue)]
			public Ampere MaxChargeCurrent;

			[Required, SIRange(-double.MaxValue, 0)]
			public Ampere MaxDischargeCurrent;
		}
	}
}