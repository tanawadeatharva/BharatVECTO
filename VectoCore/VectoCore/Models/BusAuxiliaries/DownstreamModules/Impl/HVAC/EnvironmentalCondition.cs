using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalCondition : IEnvironmentalCondition
	{
		private Kelvin _temperature;
		private WattPerSquareMeter _solar;
		private double _weight;

		public EnvironmentalCondition(Kelvin temperature, WattPerSquareMeter solar, double weight)
		{
			_temperature = temperature;
			_solar = solar;
			_weight = weight;
		}

		public Kelvin GetTemperature()
		{
			return _temperature;
		}

		public WattPerSquareMeter GetSolar()
		{
			return _solar;
		}

		public double GetWeighting()
		{
			return _weight;
		}

		public double GetNormalisedWeighting(List<IEnvironmentalCondition> map)
		{
			return _weight / map.Sum(w => w.GetWeighting());
		}
	}
}
