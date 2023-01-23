using System.Collections.Generic;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public class OvcResultEntry : IOVCResultEntry
	{
		#region Implementation of IOVCResultEntry

		public IResultEntry ChargeDepletingResult { get; internal set; }
		public IResultEntry ChargeSustainingResult { get; internal set; }

		public IWeightedResult Weighted { get; internal set; }

		#endregion
	}

	public class WeightedResult : IWeightedResult
	{
		public WeightedResult(IResultEntry cdResult)
		{
			ChargeDepletingResult = cdResult;
		}

		protected IResultEntry ChargeDepletingResult;

		#region Implementation of IWeightedResult

		public Meter Distance => ChargeDepletingResult.Distance;
		public Kilogram Payload => ChargeDepletingResult.Payload;
		public CubicMeter CargoVolume => ChargeDepletingResult.CargoVolume;
		public double? PassengerCount => ChargeDepletingResult.PassengerCount;
		public MeterPerSecond AverageSpeed { get; internal set; }
		public MeterPerSecond AverageDrivingSpeed { get; internal set; }

		public IDictionary<IFuelProperties, Kilogram> FuelConsumption { get; internal set; }
		public WattSecond ElectricEnergyConsumption { get; internal set; }
		public Kilogram CO2Total { get; internal set; }
		public Meter ActualChargeDepletingRange { get; internal set; }
		public Meter EquivalentAllElectricRange { get; internal set; }
		public Meter ZeroCO2EmissionsRange { get; internal set; }
		public double UtilityFactor { get; internal set; }
		public IFuelProperties AuxHeaterFuel { get; set; }
		public Kilogram ZEV_FuelConsumption_AuxHtr { get; set; }
		public Kilogram ZEV_CO2 { get; set; }

		#endregion
	}
}