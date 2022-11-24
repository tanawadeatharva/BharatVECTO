using System;
using System.ComponentModel;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter
{

	public abstract class HybridStrategyDataAdapter{

	}

	public class ParallelHybridStrategyParameterDataAdapter : HybridStrategyParameters
	{
		public HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData, SuperCapData superCap)
		{
			if (batterySystemData == null && superCap == null) {
				return null;
			}
			if(batterySystemData != null && superCap != null) {
				throw new VectoException("Supercap AND Batteries not supported");
			}
			var result = new HybridStrategyParameters();
			if (batterySystemData != null) {
				var tmpBatterySystem = new BatterySystem(null, batterySystemData);
				result.MinSoC = tmpBatterySystem.MinSoC;
				result.MaxSoC = tmpBatterySystem.MaxSoC;
				result.TargetSoC = (result.MaxSoC - result.MinSoC) / 2;
			} else {
				result.MinSoC = superCap.MinVoltage / superCap.MaxVoltage;
				result.MaxSoC = 1;// superCap.MaxVoltage / superCap.MaxVoltage;
				
				result.TargetSoC = Math.Sqrt(Math.Pow(superCap.MaxVoltage.Value(), 2) - Math.Pow(superCap.MaxVoltage.Value(), 2)) /
									superCap.MaxVoltage.Value();
			}

			//TODO Move to DeclarationData.Hybridstrategy.Parallel
			result.AuxReserveTime = 0.SI<Second>();
			result.AuxReserveChargeTime = 0.SI<Second>();
			result.MinICEOnTime = 10.SI<Second>();
			result.ICEStartPenaltyFactor = 0.1;
			result.CostFactorSOCExponent = 1;

			

			return result;
		}
	}

	public class SerialHybridStrategyParameterDataAdapter : HybridStrategyParameters
	{
		public HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCap)
		{
			return new HybridStrategyParameters();
		}
	}
}