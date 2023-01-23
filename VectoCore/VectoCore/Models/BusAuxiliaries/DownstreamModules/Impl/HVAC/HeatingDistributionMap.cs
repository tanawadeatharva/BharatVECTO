using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC
{
	public class HeatingDistributionMap
	{
		protected Dictionary<Tuple<HeatingDistributionCase, int>, Entry> HeatingDistribution;

		public HeatingDistributionMap(Dictionary<Tuple<HeatingDistributionCase, int>, Entry> map)
		{
			HeatingDistribution = map;
		}

		public Entry Lookup(HeatingDistributionCase hdCase, int environmentalId)
		{
			var key = Tuple.Create(hdCase, environmentalId);
			if (HeatingDistribution.ContainsKey(key)) {
				return HeatingDistribution[key];
			}
			return null;
		}

		public class Entry
		{
			protected int EnvironmentID;
			protected Dictionary<HeatPumpType, double> HeatpumpContribution;
			protected double ElectricHeaterContribution;
			protected double FuelHeaterContribution;

			public Entry(int environmentID, Dictionary<HeatPumpType, double> heatpumpContribution, double electricHeaterContribution, double fuelHeaterContribution)
			{
				EnvironmentID = environmentID;
				HeatpumpContribution = heatpumpContribution;
				ElectricHeaterContribution = electricHeaterContribution;
				FuelHeaterContribution = fuelHeaterContribution;
			}

			public double GetHeatpumpContribution(HeatPumpType heatPump)
			{
				if (HeatpumpContribution.ContainsKey(heatPump)) {
					return HeatpumpContribution[heatPump];
				}

				return 0;
			}

			public double GetElectricHeaterContribution(HeaterType heater)
			{
				return double.IsNaN(ElectricHeaterContribution) ? 0 : ElectricHeaterContribution;
			}

			public double GetFuelHeaterContribution()
			{
				return double.IsNaN(FuelHeaterContribution) ? 0 : FuelHeaterContribution;
			}
		}
	}
}