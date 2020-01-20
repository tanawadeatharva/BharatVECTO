using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M0_1Impl : IM0_1_AverageElectricLoadDemand
	{
		
		public M0_1Impl(IAuxiliaryConfig config)
		{

			TotalAverageDemandAmpsIncludingBaseLoad = config.ElectricalUserInputsConfig.AverageCurrentDemandInclBaseLoad;
			TotalAverageDemandAmpsWithoutBaseLoad = config.ElectricalUserInputsConfig.AverageCurrentDemandWithoutBaseLoad;
		}

		public Ampere TotalAverageDemandAmpsIncludingBaseLoad { get; }

		public Ampere TotalAverageDemandAmpsWithoutBaseLoad { get; }
	}
}
