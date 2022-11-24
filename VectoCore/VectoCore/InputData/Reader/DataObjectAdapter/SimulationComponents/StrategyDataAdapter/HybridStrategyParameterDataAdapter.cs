using System;
using System.Collections.Generic;
using System.ComponentModel;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.HeavyLorry;
using TUGraz.VectoCore.Models.Simulation.Data;
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
			SuperCapData superCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode)
		{

			
			if (batterySystemData == null && superCapData == null) {
				return null;
			}

			if (superCapData != null) {
				throw new VectoException("Super cap for serial hybrid is not implemented");
			}


		
			var result = new HybridStrategyParameters();
			
			result.AuxReserveTime = null;
			result.AuxReserveChargeTime = null;
			result.MinICEOnTime = null;
			result.GensetMinOptPowerFactor = 0;
			result.ICEStartPenaltyFactor = double.NaN;
			result.MaxPropulsionTorque = new Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque>();
			result.EquivalenceFactorCharge = double.NaN;
			result.EquivalenceFactorDischarge = double.NaN;
			result.CostFactorSOCExponent = 5;

			switch (ovcMode) {
				case VectoRunData.OvcHevMode.NotApplicable:
					SetGenericParameters(result, batterySystemData, superCapData, vehicleMass);
					break;
				case VectoRunData.OvcHevMode.ChargeSustaining:
					break;
				case VectoRunData.OvcHevMode.ChargeDepleting:
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ovcMode), ovcMode, null);
			}

			//TODO Move to DeclarationData.Hybridstrategy.Parallel



			return result;

		}

		private void SetGenericParameters(HybridStrategyParameters result, BatterySystemData batterySystemData,
			SuperCapData superCapData, Kilogram vehicleMass){
			var tmpSystem = new BatterySystem(null, batterySystemData);
			var deltaSoc = CalculatedDeltaSocSHev(vehicleMass, batterySystemData, tmpSystem);


			result.MinSoC = tmpSystem.MinSoC + 2 * deltaSoc;
			result.TargetSoC = tmpSystem.MaxSoC - 5 * deltaSoc;
			result.MaxSoC = tmpSystem.MaxSoC;

		}

		private double CalculatedDeltaSocSHev(Kilogram vehicleMass, BatterySystemData batterySystemData, BatterySystem tmpSystem)
		{
			
			

			var v_nom = tmpSystem.NominalVoltage;
			var c_nom = tmpSystem.Capacity;
			double speedInkmph = 100;

			var result = vehicleMass * speedInkmph.KMPHtoMeterPerSecond() * speedInkmph.KMPHtoMeterPerSecond() / v_nom / c_nom;

			
			return result.Value();
		}

	}
}