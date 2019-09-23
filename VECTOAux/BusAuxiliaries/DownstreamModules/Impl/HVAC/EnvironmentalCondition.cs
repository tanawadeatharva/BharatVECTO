using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class EnvironmentalCondition : IEnvironmentalCondition
	{
		private double _temperature;
		private double _solar;
		private double _weight;

		public EnvironmentalCondition(double temperature, double solar, double weight)
		{
			_temperature = temperature;
			_solar = solar;
			_weight = weight;
		}

		public double GetTemperature()
		{
			return _temperature;
		}

		public double GetSolar()
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
