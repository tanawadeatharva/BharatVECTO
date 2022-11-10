using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalConditionMapEntry : IEnvironmentalConditionsMapEntry
	{
        public EnvironmentalConditionMapEntry(Kelvin temperature, WattPerSquareMeter solar, double weight) : this(-1,
            temperature, solar, weight, new Dictionary<HeatPumpType, double>(),
            new Dictionary<HeaterType, double>())
        { }

        public EnvironmentalConditionMapEntry(int id, Kelvin temperature, WattPerSquareMeter solar, double weight,
			Dictionary<HeatPumpType, double> heatPumpCoP, Dictionary<HeaterType, double> heaterEfficiency)
		{
			ID = id;
			Temperature = temperature;
			Solar = solar;
			Weighting = weight;
			HeatPumpCoP = heatPumpCoP;
			HeaterEfficiency = heaterEfficiency;
		}

		public int ID { get; }

		public IReadOnlyDictionary<HeaterType, double> HeaterEfficiency { get; }

		public IReadOnlyDictionary<HeatPumpType, double> HeatPumpCoP { get; }

		public Kelvin Temperature { get; }

		public WattPerSquareMeter Solar { get; }

		public double Weighting { get; internal set; }

		public override bool Equals(object other)
		{
			var myOther = other as EnvironmentalConditionMapEntry;
			if (myOther == null) {
				return false;
			}

			return Temperature.IsEqual(myOther.Temperature) && Solar.IsEqual(myOther.Solar) &&
					Weighting.IsEqual(myOther.Weighting);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
