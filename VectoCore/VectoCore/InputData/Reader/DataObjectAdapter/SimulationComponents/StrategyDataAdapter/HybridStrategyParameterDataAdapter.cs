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
using TUGraz.VectoCore.Utils;

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
				throw new VectoException("Either Battery or SuperCap must be set");
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



			if (batterySystemData != null) {
				SetGenericParameters(ref result, batterySystemData, vehicleMass, ovcMode);
			}
			if (superCapData != null) {
				SetGenericParameters(ref result, superCapData, vehicleMass, ovcMode);
			}


			return result;
		}

		private void SetGenericParameters(ref HybridStrategyParameters result, SuperCapData superCapData, Kilogram vehicleMass, VectoRunData.OvcHevMode ovcMode)
		{

			
			SetSuperCapParameters(result, superCapData, vehicleMass, 70.KMPHtoMeterPerSecond());
			if (result.MinSoC >= result.TargetSoC) {
				SetSuperCapParameters(result, superCapData, vehicleMass, 30.KMPHtoMeterPerSecond());
			}









			if (result.MinSoC >= result.TargetSoC || result.TargetSoC >= 1) {
				throw new VectoException("Min SOC higher than Target SOC");
			}
		}

		private void SetSuperCapParameters(HybridStrategyParameters result, SuperCapData superCapData, Kilogram vehicleMass, MeterPerSecond velocity)
		{
			var min_sc_energy = SC_Energy(superCapData.MinVoltage, superCapData.Capacity);
			var max_sc_energy = SC_Energy(superCapData.MaxVoltage, superCapData.Capacity);

			var kin_energy = KineticEnergy(vehicleMass, velocity);

			var min_hs_energy = min_sc_energy + kin_energy;
			var target_hs_energy = max_sc_energy - kin_energy;

			var u_min = SC_Voltage(min_hs_energy, superCapData.Capacity);
			var u_target = SC_Voltage(target_hs_energy, superCapData.Capacity);

			result.MinSoC = u_min / superCapData.MaxVoltage;
			result.TargetSoC = u_target / superCapData.MaxVoltage;
			result.InitialSoc =
				Math.Sqrt(((Math.Pow(superCapData.MaxVoltage.Value(),2) + Math.Pow(superCapData.MinVoltage.Value(),2)) / 2.0)) /
				superCapData.MaxVoltage.Value();
			result.MaxSoC = 1;
		}

		private void SetGenericParameters(ref HybridStrategyParameters result,
			BatterySystemData batterySystemData, 
			Kilogram vehicleMass,
			VectoRunData.OvcHevMode ovcMode)
		{
			var tmpSystem = new BatterySystem(null, batterySystemData);
			var deltaSoc = CalculateDeltaSocSHev(vehicleMass, tmpSystem, 100.KMPHtoMeterPerSecond());

			var reessMinSoc = tmpSystem.MinSoC;
			var reessMaxSoc = tmpSystem.MaxSoC;

			result.MinSoC = reessMinSoc + 2 * deltaSoc;
			result.TargetSoC = tmpSystem.MaxSoC - 5 * deltaSoc;
			result.MaxSoC = double.NaN;

			if (result.MinSoC >= result.TargetSoC) {
				deltaSoc = CalculateDeltaSocSHev(vehicleMass, tmpSystem, 50.KMPHtoMeterPerSecond());
				//Small battery
				result.TargetSoC = reessMaxSoc - 1 * deltaSoc;
				result.MinSoC = reessMinSoc + 2 * deltaSoc;
				if (reessMinSoc >= result.TargetSoC) {
					throw new VectoException("Min SOC higher than Target SOC");
				}
			}

			switch (ovcMode)
			{
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
		}

		private double CalculateDeltaSocSHev(Kilogram vehicleMass, 
			BatterySystem tmpSystem, MeterPerSecond v)
		{

			var v_nom = tmpSystem.NominalVoltage;
			var c_nom = tmpSystem.Capacity.AsAmpHour;
			var result = KineticEnergy(vehicleMass, v).ConvertToWattHour()  / v_nom / c_nom;
			return result.Value();
		}

		private static WattSecond KineticEnergy(Kilogram vehicleMass, MeterPerSecond v)
		{
			return ((vehicleMass / 2.0) * v * v).Cast<WattSecond>();
		}

		private static Volt SC_Voltage(WattSecond energy, Farad capacity)
		{
			return Math.Sqrt((2.0 * energy / capacity).Value()).SI<Volt>();
		}

		private static Joule SC_Energy(Volt voltage, Farad capacity)
		{
			return 0.5 * capacity * voltage * voltage;
		}
	}
}