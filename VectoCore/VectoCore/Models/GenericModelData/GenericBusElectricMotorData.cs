using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusElectricMotorData : GenericBusEMBase
	{
		public GenericBusElectricMotorData()
		{
			GenericEfficiencyMap_ASM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_ASM_normalized.vmap";
			GenericEfficiencyMap_PSM =
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_PSM_normalized.vmap";
		}


		public ElectricMotorData CreateGenericElectricMotorData(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry,
			IList<Tuple<Volt, TableData>> torqueLimits, Volt averageVoltage)
		{
			var electricMachineType = electricMachineEntry.ElectricMachine.ElectricMachineType;
			var efficiencyData = GetVoltageLevels(electricMachineEntry, electricMachineType, torqueLimits);
			var powertrainPosition = electricMachineEntry.Position;
			var adcLossMap = electricMachineEntry.MechanicalTransmissionLossMap;
			var adcRatio = electricMachineEntry.RatioADC;
			var lossMap = powertrainPosition == PowertrainPosition.IHPC
				? TransmissionLossMapReader.CreateEmADCLossMap(1.0, 1.0, "EM ADC IHPC LossMap Eff")
				: adcLossMap != null
					? TransmissionLossMapReader.CreateEmADCLossMap(adcLossMap, adcRatio, "EM ADC LossMap")
					: TransmissionLossMapReader.CreateEmADCLossMap(DeclarationData.ElectricMachineDefaultMechanicalTransmissionEfficiency, adcRatio, "EM ADC LossMap Eff");


            var electricMotorData = new ElectricMotorData {
				RatioPerGear = electricMachineEntry.RatioPerGear,
				EMDragCurve = ElectricMotorDragCurveReader.Create(electricMachineEntry.ElectricMachine.DragCurve,
					electricMachineEntry.Count),
				EfficiencyData = efficiencyData,
				Inertia = electricMachineEntry.ElectricMachine.Inertia * electricMachineEntry.Count,//??
				RatioADC = electricMachineEntry.RatioADC,
				Overload = CalculateOverloadData(electricMachineEntry.ElectricMachine, electricMachineEntry.Count, efficiencyData, averageVoltage ),
				TransmissionLossMap = lossMap,
				OverloadRecoveryFactor = DeclarationData.OverloadRecoveryFactor,
			};

			return electricMotorData;
		}




        private OverloadData CalculateOverloadData(IIEPCDeclarationInputData iepc, int count,
            VoltageLevelData voltageLevel, Volt averageVoltage, Tuple<uint, double> gearRatioUsedForMeasurement)
        {

            // if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
            if (averageVoltage < iepc.VoltageLevels.Min(x => x.VoltageLevel))
            {
                return CalculateOverloadBuffer(iepc.VoltageLevels.First(), count, voltageLevel, gearRatioUsedForMeasurement);
            }
            if (averageVoltage > iepc.VoltageLevels.Max(x => x.VoltageLevel))
            {
                return CalculateOverloadBuffer(iepc.VoltageLevels.Last(), count, voltageLevel, gearRatioUsedForMeasurement);
            }

            var (vLow, vHigh) = iepc.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
            var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevel, gearRatioUsedForMeasurement);
            var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevel, gearRatioUsedForMeasurement);

            var retVal = new OverloadData()
            {
                OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
                ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
            };
            return retVal;
        }


        private OverloadData CalculateOverloadData(IElectricMotorDeclarationInputData motorData, int count, VoltageLevelData voltageLevel, Volt averageVoltage)
        {

            // if average voltage is outside of the voltage-level range, do not extrapolate but take the min voltage entry, or max voltage entry
            if (averageVoltage < motorData.VoltageLevels.Min(x => x.VoltageLevel))
            {
                return CalculateOverloadBuffer(motorData.VoltageLevels.First(), count, voltageLevel);
            }
            if (averageVoltage > motorData.VoltageLevels.Max(x => x.VoltageLevel))
            {
                return CalculateOverloadBuffer(motorData.VoltageLevels.Last(), count, voltageLevel);
            }

            var (vLow, vHigh) = motorData.VoltageLevels.OrderBy(x => x.VoltageLevel).GetSection(x => x.VoltageLevel < averageVoltage);
            var ovlLo = CalculateOverloadBuffer(vLow, count, voltageLevel);
            var ovlHi = CalculateOverloadBuffer(vHigh, count, voltageLevel);

            var retVal = new OverloadData()
            {
                OverloadBuffer = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.OverloadBuffer, ovlHi.OverloadBuffer, averageVoltage),
                ContinuousTorque = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorque, ovlHi.ContinuousTorque, averageVoltage),
                ContinuousPowerLoss = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPowerLoss, ovlHi.ContinuousPowerLoss, averageVoltage)
            };
            return retVal;
        }

        private OverloadData CalculateOverloadBuffer(IElectricMotorVoltageLevel voltageEntry,
            int count, VoltageLevelData voltageLevel, Tuple<uint, double> gearUsedForMeasurement = null)
        {
            var gearRatioUsedForMeasurement = gearUsedForMeasurement?.Item2 ?? 1.0;
            var gear = new GearshiftPosition(gearUsedForMeasurement?.Item1 ?? 1);
            var continuousTorque = voltageEntry.ContinuousTorque * count / gearRatioUsedForMeasurement;
            var continuousTorqueSpeed = voltageEntry.ContinuousTorqueSpeed * gearRatioUsedForMeasurement;
            var overloadTorque = (voltageEntry.OverloadTorque ?? 0.SI<NewtonMeter>()) * count / gearRatioUsedForMeasurement;
            var overloadTestSpeed = (voltageEntry.OverloadTestSpeed ?? 0.RPMtoRad()) * gearRatioUsedForMeasurement;


            var peakElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    overloadTestSpeed,
                    -overloadTorque,
                    gear,
                    true)
                    .ElectricalPower;
            var peakPwrLoss = -peakElPwr - overloadTorque * overloadTestSpeed; // losses need to be positive

            var contElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                -continuousTorque, gear).ElectricalPower ??
                            voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed),
                                gear, true).ElectricalPower;
            var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive
            var overloadBuffer = (peakPwrLoss - continuousPowerLoss) * voltageEntry.OverloadTime;
            return new OverloadData()
            {
                OverloadBuffer = overloadBuffer,
                ContinuousTorque = continuousTorque,
                ContinuousPowerLoss = continuousPowerLoss
            };
        }

        private VoltageLevelData GetVoltageLevels(
			ElectricMachineEntry<IElectricMotorDeclarationInputData> electricMachineEntry,
			ElectricMachineType electricMachineType, IList<Tuple<Volt, TableData>> torqueLimits)
		{
			var voltageLevels = electricMachineEntry.ElectricMachine.VoltageLevels;
			var count = electricMachineEntry.Count;
			var normalizedMap = GetNormalizedEfficiencyMap(electricMachineType);


			 return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, normalizedMap, torqueLimits)
			};
		}
		

		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(
			IList<IElectricMotorVoltageLevel> voltageLevels, int count, TableData normalizedMap,
			IList<Tuple<Volt, TableData>> torqueLimits)
		{
			var result = new List<ElectricMotorVoltageLevelData>();

			foreach (var voltageLevel in voltageLevels) {

				var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtEM(voltageLevels[0].FullLoadCurve);
				var efficiencyMap = DeNormalizeData(normalizedMap, ratedPoint);
				
				var electricMotorVoltageLevel = new ElectricMotorVoltageLevelData {
					Voltage = voltageLevel.VoltageLevel,
					FullLoadCurve = GetElectricMotorFullLoadCurve(voltageLevel, count, torqueLimits),
					EfficiencyMap = ElectricMotorMapReader.Create(efficiencyMap, count)
				};

				result.Add(electricMotorVoltageLevel);
			}
			
			return result;
		}


		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(IElectricMotorVoltageLevel entry,
			int count,
			IList<Tuple<Volt, TableData>> torqueLimits)
		{
			//var entries = new List<ElectricMotorFullLoadCurve.FullLoadEntry>();
			var fullLoadCurve = ElectricFullLoadCurveReader.Create(entry.FullLoadCurve, count);
            var maxTorqueCurve = torqueLimits == null
				? null
				: ElectricFullLoadCurveReader.Create(
					torqueLimits.FirstOrDefault(x => x.Item1 == null || x.Item1.IsEqual(entry.VoltageLevel))?.Item2, count);

			var fullLoadCurveCombined = ElectricMachinesDataAdapter.IntersectEMFullLoadCurves(fullLoadCurve, maxTorqueCurve);

            //         foreach (DataRow row in entry.FullLoadCurve.Rows) {
            //	entries.Add(new ElectricMotorFullLoadCurve.FullLoadEntry {
            //		MotorSpeed = row.ParseDouble("outShaftSpeed").SI<PerSecond>(),
            //		FullGenerationTorque = row.ParseDouble("minTorque").SI<NewtonMeter>(),
            //		FullDriveTorque = row.ParseDouble("maxTorque").SI<NewtonMeter>()
            //	});
            //}

            return fullLoadCurveCombined;
		}
		

		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint)
		{
			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);
			
			foreach (DataRow row in normalizedMap.Rows) {
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated;
				var torque = row.ParseDouble(TorqueNorm) * ratedPoint.TRated;
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;

				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);
			}
			
			return result;
		}
	}
}
