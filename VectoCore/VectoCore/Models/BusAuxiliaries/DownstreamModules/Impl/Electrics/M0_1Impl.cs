using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.Electrics
{
	public class M0_1Impl : IM0_1_AverageElectricLoadDemand
	{
		protected IElectricalConsumerList Consumer;


		public M0_1Impl(IAuxiliaryConfig config) { 
			Consumer = config.ElectricalUserInputsConfig.ElectricalConsumers;
			
			var doorDutyCycleFraction = GetDoorActuationTimeFraction(config.ActuationsMap, config.Cycle);

			GetTotalAverageDemandAmpsIncludingBaseLoad = Consumer.Items.Sum(
				x => x.ConsumerName == Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer
					? x.NumberInActualVehicle * x.NominalConsumptionAmps * doorDutyCycleFraction
					: x.TotalAvgConumptionAmps);

			GetTotalAverageDemandAmpsWithoutBaseLoad = Consumer.Items.Where(x => !x.BaseVehicle).Sum(
				x => x.ConsumerName == Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer
					? x.NumberInActualVehicle * x.NominalConsumptionAmps * doorDutyCycleFraction
					: x.TotalAvgConumptionAmps);
			var sum = 0.0;
			foreach (var x in Consumer.Items) {
				if (x.BaseVehicle) {
					continue;
				}
				var current = x.ConsumerName == Constants.BusAuxiliaries.ElectricalConsumers.DoorsPerVehicleConsumer
					? x.NumberInActualVehicle * x.NominalConsumptionAmps * doorDutyCycleFraction
					: x.TotalAvgConumptionAmps;
				Debug.WriteLine(current);
				sum += current.Value();
			}
		}

		private double GetDoorActuationTimeFraction(IPneumaticActuationsMap actuations, string cycle)

		{
			var actuationsKey = new ActuationsKey(Constants.BusAuxiliaries.BrakeAndDoorsActuationKey , cycle);

			var numActuations = actuations.GetNumActuations(actuationsKey);
			var secondsPerActuation = Constants.BusAuxiliaries.ElectricalConsumers.DoorActuationTimeSecond;

			actuationsKey = new ActuationsKey(Constants.BusAuxiliaries.CycleTimeActuationKey, cycle);
			var doorDutyCycleFraction = (numActuations * secondsPerActuation) / actuations.GetNumActuations(actuationsKey).SI<Second>();

			return doorDutyCycleFraction;
		}

		public Ampere GetTotalAverageDemandAmpsIncludingBaseLoad { get; }

		public Ampere GetTotalAverageDemandAmpsWithoutBaseLoad { get; }
	}
}
