using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricComponents.ElectricMotor;


namespace TUGraz.VectoCore.Models.GenericModelData
{
	public class GenericBusIEPCData : GenericBusEMBase
	{
		#region Constant

		public const double GenericGearEfficiency = 0.96;
		public const double GenericAxleGearEfficiency = 0.96;

		#endregion

		private readonly GenericTransmissionComponentData _genericTransmission = new GenericTransmissionComponentData();

		private double _axleEfficiency;
		private Dictionary<int, double> _gearRatios;
		private double _axleRatio;
		private KeyValuePair<int, double> _gearRatioAtMeasurement;

		public GenericBusIEPCData()
		{
			GenericEfficiencyMap_ASM = 
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_IEPC_ASM_normalized.vmap";
			GenericEfficiencyMap_PSM = 
				$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EfficiencyMap_IEPC_PSM_normalized.vmap";
		}

		public IEPCElectricMotorData CreateIEPCElectricMotorData(IIEPCDeclarationInputData iepcData)
		{
			InitData(iepcData);

			var count = iepcData.DesignTypeWheelMotor && iepcData.NrOfDesignTypeWheelMotorMeasured == 1 ? 2 : 1; //? also for declaration mode valid
			
			var iepcEM = new IEPCElectricMotorData {
				IEPCDragCurves = GetIEPCDragCurves(iepcData, count),
				EfficiencyData = GetIEPCVoltageLevelData(iepcData.VoltageLevels, count, iepcData.ElectricMachineType),
				Inertia = iepcData.Inertia * count,
				RatioPerGear = _gearRatios.Select(x => x.Value).ToArray(),
				//iepcEM.RatioADC
                //iepcEM.TransmissionLossMap
                //iepcEM.EMDragCurve
                //iepcEM.Overload
            };
			
			return iepcEM;
		}

		private void InitData(IIEPCDeclarationInputData iepcData)
		{
			if (iepcData.DifferentialIncluded) {
				_axleEfficiency = GenericAxleGearEfficiency;
				_axleRatio = 1;
			} else {
				_axleEfficiency = 1;
				_axleRatio = 1;
			}

			_gearRatios = new Dictionary<int, double>();
			foreach (var gear in iepcData.Gears.OrderBy(x => x.GearNumber)) {
				_gearRatios.Add(gear.GearNumber, gear.Ratio);
			}

			_gearRatioAtMeasurement = GetGearRatioAtMeasurement();
		}

		private VoltageLevelData GetIEPCVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, ElectricMachineType electricMachineType)
		{
			return new VoltageLevelData {
				VoltageLevels = GetElectricMotorVoltageLevelData(voltageLevels, count, electricMachineType)
			};
		}

		private List<ElectricMotorVoltageLevelData> GetElectricMotorVoltageLevelData(IList<IElectricMotorVoltageLevel> voltageLevels, int count, ElectricMachineType electricMachineType)
		{
			var result = new List<ElectricMotorVoltageLevelData>();
			foreach (var entry in voltageLevels.OrderBy(x => x.VoltageLevel)) {
				var fld = GetElectricMotorFullLoadCurve(entry, count);
				var flds = GetElectricMotorFullLoadCurves(entry, count);

                var iepcVoltageLevel = new IEPCVoltageLevelData {
					EfficiencyMaps = GetEfficiencyMaps(entry, count, electricMachineType, (flds.Count > 0) ? flds : new Dictionary<uint, ElectricMotorFullLoadCurve>() { { 0, fld } }),
					Voltage = entry.VoltageLevel,
					FullLoadCurve = fld,
					FullLoadCurves = flds
				};
				result.Add(iepcVoltageLevel);
			}

			return result;
		}

		private ElectricMotorFullLoadCurve GetElectricMotorFullLoadCurve(IElectricMotorVoltageLevel voltageLevel, int count)
		{
			return (voltageLevel.FullLoadCurve.Count == 1) && (voltageLevel.FullLoadCurve.First().Gear == 0) 
				? IEPCFullLoadCurveReader.Create(voltageLevel.FullLoadCurve.First().LoadCurve, count, _gearRatioAtMeasurement.Value)
				: null;
		}

		private Dictionary<uint, ElectricMotorFullLoadCurve> GetElectricMotorFullLoadCurves(IElectricMotorVoltageLevel voltageLevel, int count)
		{
            var fldCurves = new Dictionary<uint, ElectricMotorFullLoadCurve>();

            foreach (var curve in voltageLevel.FullLoadCurve.Where(x => x.Gear > 0))
            {
                var ratio = _gearRatios.First(x => x.Key == curve.Gear).Value;
                fldCurves.Add((uint)curve.Gear, IEPCFullLoadCurveReader.Create(curve.LoadCurve, count, ratio));
            }

			return fldCurves;
        }

        private Dictionary<uint, EfficiencyMap> GetEfficiencyMaps(IElectricMotorVoltageLevel voltageLevel, int count,
			ElectricMachineType electricMachineType, Dictionary<uint, ElectricMotorFullLoadCurve> fullLoadCurves)
		{
			var result = new Dictionary<uint, EfficiencyMap>();

			foreach (var gearEntry in _gearRatios) {

				var gearRatio = gearEntry.Value;

				var loadCurve = voltageLevel.FullLoadCurve.FirstOrDefault(x => x.Gear == gearEntry.Key) ?? voltageLevel.FullLoadCurve.First();

                var ratedPoint = GenericRatedPointHelper.GetRatedPointOfFullLoadCurveAtIEPC(loadCurve.LoadCurve,
					_axleRatio, (loadCurve.Gear == 0) ? _gearRatioAtMeasurement.Value : gearRatio, GenericGearEfficiency, _axleEfficiency);

                var fullLoadCurve = fullLoadCurves.ContainsKey((uint)gearEntry.Key) ? fullLoadCurves[(uint)gearEntry.Key] : fullLoadCurves.First().Value;

                var deNormalizedMap = DeNormalizeData(GetNormalizedEfficiencyMap(electricMachineType), ratedPoint, gearRatio);
				result.Add((uint) gearEntry.Key, IEPCMapReader.Create(deNormalizedMap, count, gearRatio, fullLoadCurve, ExecutionMode.Declaration));
			}

			return result;
		}
		

		private Dictionary<uint, DragCurve> GetIEPCDragCurves(IIEPCDeclarationInputData iepcData, int count)
		{
			var dragCurves = new Dictionary<uint, DragCurve>();

			if (iepcData.DragCurves.Count > 1) {
				for (var i = 0u; i < iepcData.DragCurves.Count; i++) {
					var ratio = iepcData.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(iepcData.DragCurves[(int)i].DragCurve, count, ratio));
				}
			} else {
				var dragCurve = iepcData.DragCurves.First().DragCurve;
				for (var i = 0u; i < iepcData.Gears.Count; i++) {
					var ratio = iepcData.Gears.First(x => x.GearNumber == i + 1).Ratio;
					dragCurves.Add(i + 1, IEPCDragCurveReader.Create(dragCurve, count, ratio));
				}
			}

            return dragCurves;
		}


		private KeyValuePair<int, double> GetGearRatioAtMeasurement()
		{
			var gear = _gearRatios.Select(x => new {
				Gear = x.Key,
				Ratio = x.Value,
				Distance = Math.Abs(x.Value - 1)
			}).OrderBy(x => x.Distance).GroupBy(x => x.Distance).First().MaxBy(x => x.Ratio);

			return new KeyValuePair<int, double>(gear.Gear, gear.Ratio);
		}
		

		private DataTable DeNormalizeData(TableData normalizedMap, RatedPoint ratedPoint, double gearRatio)
		{
			var result = new DataTable();
			result.Columns.Add(ElectricMotorMapReader.Fields.MotorSpeed);
			result.Columns.Add(ElectricMotorMapReader.Fields.Torque);
			result.Columns.Add(ElectricMotorMapReader.Fields.PowerElectrical);

			foreach (DataRow row in normalizedMap.Rows) {
				var torqueNormValue = row.ParseDouble(TorqueNorm);
				var motorSpeed = row.ParseDouble(MotorSpeedNorm) * ratedPoint.NRated / gearRatio / _axleRatio;
				var torque = torqueNormValue * ratedPoint.TRated * gearRatio * _axleRatio * 
							( torqueNormValue > 0 ? GenericGearEfficiency * _axleEfficiency : 1 / (GenericGearEfficiency * _axleEfficiency));
				var powerElectrical = row.ParseDouble(PowerElectricalNorm) * ratedPoint.PRated;
				
				var newRow = result.NewRow();
				newRow[ElectricMotorMapReader.Fields.MotorSpeed] = Math.Round(motorSpeed.AsRPM, 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.Torque] = Math.Round(torque.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				newRow[ElectricMotorMapReader.Fields.PowerElectrical] = Math.Round(powerElectrical.Value(), 2, MidpointRounding.AwayFromZero).ToXMLFormat(2);
				result.Rows.Add(newRow);
			}

			return result;
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
                ContinuousTorqueGen = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousTorqueGen, ovlHi.ContinuousTorqueGen, averageVoltage),
                ContinuousPower = VectoMath.Interpolate(vLow.VoltageLevel, vHigh.VoltageLevel, ovlLo.ContinuousPower, ovlHi.ContinuousPower, averageVoltage),
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

            //ElectricMotorRatedSpeedHelper.GetRatedSpeed(voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque)
            var FullLoadCurve = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve;
            // FullLoadCurve express torque and speed at the rotor level, not the output shaft

            //var FullLoadEntries = voltageLevel.VoltageLevels.Where(x => x.Voltage == voltageEntry.VoltageLevel).First().FullLoadCurve.FullLoadEntries; 
            //var continuousPower = continuousTorque * continuousTorqueSpeed;

            var continuousTorqueSpeedRef = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullDriveTorque));
            var continuousTorqueSpeedRefGen = VectoMath.Min(continuousTorqueSpeed, ElectricMotorRatedSpeedHelper.GetRatedSpeed(FullLoadCurve.FullLoadEntries, row => row.MotorSpeed, row => row.FullGenerationTorque));

            //var continuousTorqueRef = VectoMath.Min(continuousPower / continuousTorqueSpeedRef, -FullLoadCurve.MaxDriveTorque);
            //var continuousTorqueRefGen = VectoMath.Min(continuousPower / continuousTorqueSpeedRefGen, FullLoadCurve.MaxGenerationTorque);

            var ovlElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    overloadTestSpeed,
                    -overloadTorque,
                    gear,
                    true)
                    .ElectricalPower;
            var ovlPwrLoss = -ovlElPwr - overloadTorque * overloadTestSpeed; // losses need to be positive

            var contElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                -continuousTorque, gear).ElectricalPower ??
                            voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel, continuousTorqueSpeed,
                                voltageLevel.FullLoadDriveTorque(voltageEntry.VoltageLevel, continuousTorqueSpeed, gear.Gear),
                                gear, true).ElectricalPower;
            var continuousPowerLoss = -contElPwr - continuousTorque * continuousTorqueSpeed; // loss needs to be positive

            var overloadBufferDirect = VectoMath.Max(ovlPwrLoss - continuousPowerLoss, continuousPowerLoss * 0.0001) * voltageEntry.OverloadTime;

            // min buffer based on max torque

            var maxTorqueDrvFldContSpeed = -FullLoadCurve.FullLoadDriveTorque(continuousTorqueSpeed);
            var maxTorqueGenFldContSpeed = FullLoadCurve.FullGenerationTorque(continuousTorqueSpeed);

            // peak losses considering maxTorque instead of OVL torque, but with a time of 3 seconds constraint instead of Tovl used in previous version

            var peakElPwr = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    continuousTorqueSpeed,
                    -maxTorqueDrvFldContSpeed,
                    gear,
                    true)
                    .ElectricalPower;
            var peakPwrLoss = -peakElPwr - continuousTorqueSpeed * maxTorqueDrvFldContSpeed; // losses need to be positive
            //var peakPwrLossEtaOVL = (1 / etaOvl.Value() - 1) * continuousTorqueSpeed * maxTorqueDrvFldContSpeed; // losses need to be positive

            // For case when Gen max Torque is higher than Drive Torque, The Buffer might need to be increased
            // Use Gen torque, to ensure reaching it during 3 seconds
            var peakElPwrGen = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                continuousTorqueSpeed,
                maxTorqueGenFldContSpeed,
                gear,
                true)
                .ElectricalPower;
            var peakPwrLossGen = continuousTorqueSpeed * maxTorqueGenFldContSpeed - peakElPwrGen; // losses need to be positive
            //var peakPwrLossGenEtaOVL = (1 - etaOvl.Value()) * continuousTorqueSpeed * maxTorqueGenFldContSpeed; // losses need to be positive

            // additional saturation taking into account all the operating points of the FullLoadCurve

            var peakElPwr_i = peakElPwr * 0.0;
            var peakPwrLossGen_i = peakElPwr * 0.0;
            var peakPwrLoss_i = peakElPwr * 0.0;

            foreach (var OP in FullLoadCurve.FullLoadEntries)
            {
                if (OP.MotorSpeed.IsGreater(0.0))
                {
                    peakElPwr_i = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                    OP.MotorSpeed,
                    OP.FullDriveTorque, // OP.FullGenerationTorque
                    gear,
                    true)
                    .ElectricalPower;

                    peakPwrLoss_i = OP.MotorSpeed * OP.FullDriveTorque - peakElPwr_i; // operation reversed to fit sign rule

                    peakElPwr_i = voltageLevel.LookupElectricPower(voltageEntry.VoltageLevel,
                        OP.MotorSpeed,
                        OP.FullGenerationTorque, // OP.FullGenerationTorque
                        gear,
                        true)
                        .ElectricalPower;

                    peakPwrLossGen_i = OP.MotorSpeed * OP.FullGenerationTorque - peakElPwr_i; // operation reversed to fit sign rule

                    peakPwrLoss = VectoMath.Max(peakPwrLoss, peakPwrLoss_i);
                    peakPwrLossGen = VectoMath.Max(peakPwrLossGen, peakPwrLossGen_i);
                }

            }

            // BUFFER
            // buffer is the difference of power losses between the OVL point (a Max Torque point for 3s) and the CONT point,
            // two computation methods:
            // - using losses computation on maps
            // - iso efficiency assumption for both leads to a difference of mechanical power in expression, which has to be positive

            var dPloss = VectoMath.Max(peakPwrLoss, peakPwrLossGen) - continuousPowerLoss;

            var OverloadTime = 3.SI<Second>();

            var overloadBufferTransf = VectoMath.Max(dPloss, continuousPowerLoss * 0.0001) * OverloadTime;

            var overloadBuffer = VectoMath.Max(overloadBufferTransf, overloadBufferDirect);

            // definition of continuousPower and continuousTorqueRef@ratedSpeed limits in derated mode

            var continuousPower = continuousTorque * continuousTorqueSpeed;
            var continuousTorqueRef = VectoMath.Min(continuousPower / continuousTorqueSpeedRef, -FullLoadCurve.MaxDriveTorque);
            var continuousTorqueRefGen = VectoMath.Min(continuousPower / continuousTorqueSpeedRefGen, FullLoadCurve.MaxGenerationTorque);

            return new OverloadData()
            {
                OverloadBuffer = overloadBuffer,
                ContinuousTorque = continuousTorqueRef,
                ContinuousTorqueGen = continuousTorqueRefGen,
                ContinuousPower = continuousPower,
                ContinuousPowerLoss = continuousPowerLoss
            };
        }
    }
}
