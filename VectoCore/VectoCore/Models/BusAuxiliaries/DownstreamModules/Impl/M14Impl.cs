using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl.HVAC;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules;
using TUGraz.VectoCore.Models.BusAuxiliaries.Interfaces.DownstreamModules.HVAC;
using TUGraz.VectoCore.Models.Declaration;

namespace TUGraz.VectoCore.Models.BusAuxiliaries.DownstreamModules.Impl
{
	//public class M14Impl : AbstractModule, IM14
	//{
	//	protected readonly IM13 M13;
	//	protected readonly ISignals Signals;
	//	protected readonly ISSMTOOL SSM;

	//	protected Kilogram _totalCycleFcGrams;
	//	private readonly IFuelProperties EngineFuel;


	//	public M14Impl(IM13 m13, ISSMTOOL ssm, IFuelProperties engineFuel, ISignals signals)
	//	{
	//		M13 = m13;
	//		SSM = ssm;
	//		EngineFuel = engineFuel;
	//		Signals = signals;
	//	}


	//	protected override void DoCalculate()
	//	{
	//		var s1 = M13.WHTCTotalCycleFuelConsumption * EngineFuel.LowerHeatingValueVectoEngine;
	//		var s2 = SSM.SSMInputs.AuxHeater.FuelEnergyToHeatToCoolant * s1;
	//		var s3 = s2 * SSM.SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;
	//		var s4 = s3 / Signals.CurrentCycleTimeInSeconds.SI<Second>();
	//		var s5 = Signals.CurrentCycleTimeInSeconds.SI<Second>(); 
	//		var s6 = s5 * SSM.FuelPerHBaseAsjusted(s4); 
	//		var s7 = M13.WHTCTotalCycleFuelConsumption + s6;
	//		_totalCycleFcGrams = s7;
	//	}


	//	#region Implementation of IM14

	//	public Kilogram TotalCycleFC
	//	{
	//		get {
	//			if (!calculationValid) {
	//				Calculate();
	//			}
	//			return _totalCycleFcGrams;
	//		}
	//	}

		

	//	#endregion
	//}

	public class M14aImpl
	{
		//private Signals _signals;
		private SSMTOOL _ssmTool;

		public M14aImpl(SSMTOOL ssmTool)
		{
			_ssmTool = ssmTool;
		}

		public HeaterDemandResult AuxHeaterDemand(Second cycleTime, Joule engineWasteHeat,
			Joule electricMotorWasteHeatTotal)
		{
			var averageEngineWasteHeatPwr = engineWasteHeat / cycleTime;
			var iceUsableWasteHeat = averageEngineWasteHeatPwr *
									_ssmTool.SSMInputs.AuxHeater.FuelEnergyToHeatToCoolant *
									_ssmTool.SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;
			var emUsableWasteHeat = electricMotorWasteHeatTotal / cycleTime *
									_ssmTool.SSMInputs.ElectricWasteHeatToCoolant *
									_ssmTool.SSMInputs.AuxHeater.CoolantHeatTransferredToAirCabinHeater;
			var averageUsableEngineWasteHeat = iceUsableWasteHeat + emUsableWasteHeat;
			return new HeaterDemandResult(_ssmTool.AverageHeaterPower(averageUsableEngineWasteHeat), cycleTime);
		}
	}

	public class M14bImpl
	{
		private ISSMEngineeringInputs _auxConfig;

		public M14bImpl(ISSMEngineeringInputs auxConfigSsmInputs)
		{
			
			_auxConfig = auxConfigSsmInputs;
		}

		public HeaterDemandResult AuxHeaterDemand(Second cycleTime, Joule engineWasteHeat,
			Joule electricMotorWasteHeatTotal)
		{
			var averageEngineWasteHeatPwr = engineWasteHeat;
			var iceUsableWasteHeat = averageEngineWasteHeatPwr *
									_auxConfig.FuelEnergyToHeatToCoolant *
									_auxConfig.CoolantHeatTransferredToAirCabinHeater;
			var emUsableWasteHeat = electricMotorWasteHeatTotal *
									_auxConfig.ElectricWasteHeatToCoolant *
									_auxConfig.CoolantHeatTransferredToAirCabinHeater;
			var averageUsableEngineWasteHeat = iceUsableWasteHeat + emUsableWasteHeat;

			var heatingDiff = VectoMath.Max(0.SI<Joule>(), _auxConfig.HeatingDemand - averageUsableEngineWasteHeat);
			var auxHeaterEnergy = VectoMath.Min(heatingDiff , (_auxConfig.AuxHeaterPower * cycleTime).Cast<Joule>()) / _auxConfig.AuxHeaterEfficiency;

			return new HeaterDemandResult(new HeaterPower() {
				AuxHeaterPower = auxHeaterEnergy / cycleTime,
				ElectricHeaterPowerEl = 0.SI<Watt>(),
				HeatPumpPowerEl = 0.SI<Watt>(),
				HeatPumpPowerMech = 0.SI<Watt>(),
				RequiredHeatingPower = _auxConfig.HeatingDemand / cycleTime
			}, cycleTime);
		}
	}
}
