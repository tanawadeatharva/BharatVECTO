using System;
using System.Collections.Generic;
using NLog.Targets;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.Battery;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.StrategyDataAdapter
{

    public abstract class HybridStrategyDataAdapter{

	}

	public class ParallelHybridStrategyParameterDataAdapter : HybridStrategyDataAdapter
	{
		public HybridStrategyParameters CreateHybridStrategyParameters(BatterySystemData batterySystemData,
			SuperCapData superCap, VectoRunData.OvcHevMode ovcMode, LoadingType loading, VehicleClass vehicleClass, MissionType missionType)
		{
			if (batterySystemData == null && superCap == null) {
				return null;
			}

			if (batterySystemData != null && superCap != null) {
				throw new VectoException("Supercap AND Batteries not supported");
			}

			var result = new HybridStrategyParameters();
			
			if (batterySystemData != null) {
				var tmpBatterySystem = new BatterySystem(null, batterySystemData);
				result.MinSoC = tmpBatterySystem.MinSoC;
				result.MaxSoC = tmpBatterySystem.MaxSoC;
				result.TargetSoC = (result.MaxSoC - result.MinSoC) / 2;
				result.InitialSoc = batterySystemData.InitialSoC;
			} else {
				result.MinSoC = superCap.MinVoltage / superCap.MaxVoltage;
				result.MaxSoC = 1;// superCap.MaxVoltage / superCap.MaxVoltage;
				
				result.TargetSoC = Math.Sqrt(Math.Pow(superCap.MaxVoltage.Value(), 2) - Math.Pow(superCap.MinVoltage.Value(), 2)) /
									superCap.MaxVoltage.Value();
				result.InitialSoc = superCap.InitialSoC;
			}



			//var equivalenceFactor = DeclarationData.HevStrategyParameters.

			result.AuxReserveTime = 0.SI<Second>();
			result.AuxReserveChargeTime = 0.SI<Second>();
			result.MinICEOnTime = 10.SI<Second>();
			result.ICEStartPenaltyFactor = 0.1;
			result.CostFactorSOCExponent = 1;

			if (ovcMode == VectoRunData.OvcHevMode.ChargeSustaining) {
				result.EquivalenceFactor =
					DeclarationData.HEVStrategyParameters.LookupEquivalenceFactor(missionType,
						vehicleClass, loading, result.MaxSoC - result.MinSoC);
			} else {
				result.EquivalenceFactor = DeclarationData.HEVStrategyParameters.PHEVChargeDepletingEquivalenceFactor;
			}


			result.EquivalenceFactorCharge = result.EquivalenceFactor * 0.85;
			result.EquivalenceFactorDischarge = result.EquivalenceFactor / 0.85;
			

			return result;
		}
	}

	public class SerialHybridStrategyParameterDataAdapter : HybridStrategyDataAdapter
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
			result.GensetMinOptPowerFactor = 0.2;

			result.ICEStartPenaltyFactor = double.NaN;
			result.MaxPropulsionTorque = new Dictionary<GearshiftPosition, VehicleMaxPropulsionTorque>();
			result.EquivalenceFactorCharge = double.NaN;
			result.EquivalenceFactorDischarge = double.NaN;
			result.CostFactorSOCExponent = double.NaN;

	

			SetGenericParameters(result, batterySystemData, superCapData, vehicleMass, out var deltaSoc, out var tmpSystem);

			switch (ovcMode) {
				case VectoRunData.OvcHevMode.ChargeSustaining:
					result.InitialSoc = result.MinSoC + deltaSoc;
					break;
				case VectoRunData.OvcHevMode.ChargeDepleting:
					result.InitialSoc = (tmpSystem.MaxSoC + tmpSystem.MinSoC) / 2;
					result.TargetSoC = result.InitialSoc - 1;
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(ovcMode), ovcMode, null);
			}

			return result;
		}

		private void SetGenericParameters(HybridStrategyParameters result, BatterySystemData batterySystemData,
			SuperCapData superCapData, Kilogram vehicleMass, out double deltaSoc, out BatterySystem tmpSystem)
		{
			tmpSystem = null;
			if (batterySystemData != null) {
				tmpSystem = new BatterySystem(null, batterySystemData);
			}

			deltaSoc = CalculatedDeltaSocSHev(vehicleMass, tmpSystem, superCapData);

			var reessMinSoc = tmpSystem?.MinSoC ?? (superCapData.MinVoltage / superCapData.MaxVoltage);
			var reessMaxSoc = tmpSystem?.MaxSoC ?? 1;



			result.MinSoC = reessMinSoc + 2 * deltaSoc;
			result.TargetSoC = tmpSystem.MaxSoC - 5 * deltaSoc;
			result.MaxSoC = double.NaN;


			if (result.MinSoC >= result.TargetSoC) {
				deltaSoc = CalculatedDeltaSocSHev(vehicleMass, tmpSystem, superCapData, 50);
				//Small battery
				result.TargetSoC = reessMaxSoc - 1 * deltaSoc;
				result.MinSoC = reessMinSoc + 2 * deltaSoc;
				if (reessMinSoc >= result.TargetSoC) {
					throw new VectoException("Min SOC higher than Target SOC");
				}
			}
		}

		private double CalculatedDeltaSocSHev(Kilogram vehicleMass, 
			BatterySystem tmpSystem, SuperCapData superCap, double kmph = 100)
		{

			var v_nom = tmpSystem?.NominalVoltage ?? (superCap.MaxVoltage + superCap.MinVoltage)/2;
			var c_nom = tmpSystem.Capacity.AsAmpHour;
			var v = kmph.KMPHtoMeterPerSecond();
			var result = ((vehicleMass / 2 * v * v) / 3600) * (1 / v_nom) * (1 / c_nom);
			return result.Value();
		}

	}
}