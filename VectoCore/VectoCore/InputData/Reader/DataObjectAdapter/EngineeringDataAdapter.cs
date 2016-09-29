/*
* This file is part of VECTO.
*
* Copyright © 2012-2016 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class EngineeringDataAdapter : AbstractSimulationDataAdapter
	{
		internal VehicleData CreateVehicleData(IVehicleEngineeringInputData data)
		{
			if (data.SavedInDeclarationMode) {
				WarnEngineeringMode("VehicleData");
			}

			var retVal = SetCommonVehicleData(data);

			retVal.CurbWeight += data.CurbWeightExtra;
			retVal.TrailerGrossVehicleWeight = 0.SI<Kilogram>();
			retVal.Loading = data.Loading;
			retVal.DynamicTyreRadius = data.DynamicTyreRadius;
			retVal.CrossWindCorrectionMode = data.CrossWindCorrectionMode;
			switch (data.CrossWindCorrectionMode) {
				case CrossWindCorrectionMode.NoCorrection:
					retVal.CrossWindCorrectionCurve =
						new CrosswindCorrectionCdxALookup(CrossWindCorrectionCurveReader.GetNoCorrectionCurve(data.AirDragArea),
							CrossWindCorrectionMode.NoCorrection);
					break;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurve(data.CrosswindCorrectionMap,
							data.AirDragArea), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
					break;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionVAirBeta(data.AirDragArea,
						CrossWindCorrectionCurveReader.ReadCdxABetaTable(data.CrosswindCorrectionMap));
					break;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					retVal.CrossWindCorrectionCurve =
						new CrosswindCorrectionCdxALookup(DeclarationDataAdapter.GetDeclarationAirResistanceCurve(retVal.VehicleCategory,
							data.AirDragArea), CrossWindCorrectionMode.DeclarationModeCorrection);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			var axles = data.Axles;
			retVal.AxleData = axles.Select(axle => new Axle {
				WheelsDimension = axle.Wheels,
				Inertia = axle.Inertia,
				TwinTyres = axle.TwinTyres,
				RollResistanceCoefficient = axle.RollResistanceCoefficient,
				AxleWeightShare = axle.AxleWeightShare,
				TyreTestLoad = axle.TyreTestLoad,
				//Wheels = axle.WheelsStr
			}).ToList();
			return retVal;
		}

		private void WarnEngineeringMode(string msg)
		{
			Log.Error("{0} is in Declaration Mode but is used for Engineering Mode!", msg);
		}

		internal CombustionEngineData CreateEngineData(IEngineEngineeringInputData engine, IGearboxEngineeringInputData gbx)
		{
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine);
			retVal.Inertia = engine.Inertia +
							(gbx != null && gbx.Type.AutomaticTransmission() ? gbx.TorqueConverter.Inertia : 0.SI<KilogramSquareMeter>());
			retVal.FullLoadCurve = EngineFullLoadCurve.Create(engine.FullLoadCurve);
			retVal.FullLoadCurve.EngineData = retVal;
			retVal.WHTCCorrectionFactor = engine.WHTCEngineering;
			return retVal;
		}

		internal GearboxData CreateGearboxData(IGearboxEngineeringInputData gearbox, CombustionEngineData engineData,
			double axlegearRatio, Meter dynamicTyreRadius, bool useEfficiencyFallback)
		{
			if (gearbox.SavedInDeclarationMode) {
				WarnEngineeringMode("GearboxData");
			}

			var retVal = SetCommonGearboxData(gearbox);

			//var gears = gearbox.Gears;
			if (gearbox.Gears.Count < 2) {
				throw new VectoSimulationException(
					"At least two Gear-Entries must be defined in Gearbox!");
			}

			retVal.Inertia = gearbox.Inertia;
			retVal.TractionInterruption = gearbox.TractionInterruption;
			retVal.TorqueReserve = gearbox.TorqueReserve;
			retVal.StartTorqueReserve = gearbox.StartTorqueReserve;
			retVal.ShiftTime = gearbox.ShiftTime;
			retVal.StartSpeed = gearbox.StartSpeed;
			retVal.StartAcceleration = gearbox.StartAcceleration;

			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();

			for (uint i = 0; i < gearbox.Gears.Count; i++) {
				var gear = gearbox.Gears[(int)i];
				TransmissionLossMap lossMap;
				if (gear.LossMap != null) {
					lossMap = TransmissionLossMapReader.Create(gear.LossMap, gear.Ratio, string.Format("Gear {0}", i + 1));
				} else if (useEfficiencyFallback) {
					lossMap = TransmissionLossMapReader.Create(gear.Efficiency, gear.Ratio, string.Format("Gear {0}", i + 1));
				} else {
					throw new InvalidFileFormatException("Gear {0} LossMap or Efficiency missing.", i + 1);
				}

				var fullLoadCurve = IntersectFullLoadCurves(engineData.FullLoadCurve, gear.MaxTorque);
				if (gearbox.Type.AutomaticTransmission() && gear.ShiftPolygon == null) {
					throw new VectoException("Shiftpolygons are required for AT Gearboxes!");
				}
				var shiftPolygon = gear.ShiftPolygon != null
					? ShiftPolygonReader.Create(gear.ShiftPolygon)
					: DeclarationData.Gearbox.ComputeShiftPolygon((int)i, fullLoadCurve, gearbox.Gears, engineData, axlegearRatio,
						dynamicTyreRadius);
				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxTorque = gear.MaxTorque,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				if (gearbox.Type == GearboxType.ATPowerSplit) {
					if (i == 0) {
						// powersplit transmission: torque converter already contains ratio and losses
						gearData.TorqueConverterRatio = 1;
						gearData.TorqueConverterGearLossMap = TransmissionLossMapReader.Create(1, 1, string.Format("TCGear {0}", i + 1));
						gearData.TorqueConverterShiftPolygon = ShiftPolygonReader.Create(gearbox.TorqueConverter.ShiftPolygon);
					}
				}
				if (gearbox.Type == GearboxType.ATSerial) {
					if (i == 0) {
						// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
						gearData.TorqueConverterRatio = gearData.Ratio;
						gearData.TorqueConverterGearLossMap = gearData.LossMap;
						gearData.TorqueConverterShiftPolygon = ShiftPolygonReader.Create(gearbox.TorqueConverter.ShiftPolygon);
					}
					if (i == 1 && gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold) {
						// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
						// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
						gearData.TorqueConverterRatio = gearData.Ratio;
						gearData.TorqueConverterGearLossMap = gearData.LossMap;
						gearData.TorqueConverterShiftPolygon = ShiftPolygonReader.Create(gearbox.TorqueConverter.ShiftPolygon);
						// NOTE: the lower gear in 'gears' dictionary has index i !!
						gears[i].Ratio = double.NaN;
						gears[i].LossMap = null;
					}
				}
				gears.Add(i + 1, gearData);
			}
			retVal.Gears = gears;

			if (retVal.Gears.Any(g => g.Value.HasTorqueConverter)) {
				if (!retVal.Type.AutomaticTransmission()) {
					throw new VectoException("Torque Converter can only be used with AT gearbox model");
				}
				retVal.TorqueConverterData = TorqueConverterDataReader.Create(gearbox.TorqueConverter.TCData,
					gearbox.TorqueConverter.ReferenceRPM, DeclarationData.Gearbox.TorqueConverterSpeedLimit);
			} else {
				if (retVal.Type.AutomaticTransmission()) {
					throw new VectoException("AT gearbox model requires torque converter");
				}
			}

			retVal.DownshiftAfterUpshiftDelay = gearbox.DownshiftAferUpshiftDelay;
			retVal.UpshiftAfterDownshiftDelay = gearbox.UpshiftAfterDownshiftDelay;
			retVal.UpshiftMinAcceleration = gearbox.UpshiftMinAcceleration;
			return retVal;
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData)
		{
			return auxInputData.Auxiliaries.Select(a => {
				switch (a.AuxiliaryType) {
					case AuxiliaryDemandType.Mapping:
						return CreateMappingAuxiliary(a);
					case AuxiliaryDemandType.Constant:
						return CreateConstantAuxiliary(a);
					default:
						throw new VectoException("Auxiliary type {0} not supported!", a.AuxiliaryType);
				}
			}).Concat(new VectoRunData.AuxData { ID = "", DemandType = AuxiliaryDemandType.Direct }.ToEnumerable()).ToList();
		}

		private static VectoRunData.AuxData CreateMappingAuxiliary(IAuxiliaryEngineeringInputData a)
		{
			if (a.DemandMap == null) {
				throw new VectoSimulationException("Demand Map for auxiliary {0} required", a.ID);
			}
			if (a.DemandMap.Columns.Count != 3 || a.DemandMap.Rows.Count < 4) {
				throw new VectoSimulationException(
					"Demand Map for auxiliary {0} has to contain exactly 3 columns and at least 4 rows", a.ID);
			}
			return new VectoRunData.AuxData {
				ID = a.ID,
				DemandType = AuxiliaryDemandType.Mapping,
				Data = AuxiliaryDataReader.Create(a)
			};
		}

		private static VectoRunData.AuxData CreateConstantAuxiliary(IAuxiliaryEngineeringInputData a)
		{
			return new VectoRunData.AuxData {
				ID = a.ID,
				DemandType = AuxiliaryDemandType.Constant,
				PowerDemand = a.ConstantPowerDemand
			};
		}

		internal DriverData CreateDriverData(IDriverEngineeringInputData driver)
		{
			if (driver.SavedInDeclarationMode) {
				WarnEngineeringMode("DriverData");
			}

			AccelerationCurveData accelerationData = null;
			if (driver.AccelerationCurve != null) {
				accelerationData = AccelerationCurveReader.Create(driver.AccelerationCurve);
			}

			var lookAheadData = new DriverData.LACData {
				Enabled = driver.Lookahead.Enabled,
				//Deceleration = driver.Lookahead.Deceleration,
				MinSpeed = driver.Lookahead.MinSpeed,
				LookAheadDecisionFactor =
					new LACDecisionFactor(driver.Lookahead.CoastingDecisionFactorOffset, driver.Lookahead.CoastingDecisionFactorScaling,
						driver.Lookahead.CoastingDecisionFactorTargetSpeedLookup,
						driver.Lookahead.CoastingDecisionFactorVelocityDropLookup),
				LookAheadDistanceFactor = driver.Lookahead.LookaheadDistanceFactor
			};
			var overspeedData = new DriverData.OverSpeedEcoRollData {
				Mode = driver.OverSpeedEcoRoll.Mode,
				MinSpeed = driver.OverSpeedEcoRoll.MinSpeed,
				OverSpeed = driver.OverSpeedEcoRoll.OverSpeed,
				UnderSpeed = driver.OverSpeedEcoRoll.UnderSpeed,
			};
			var startstopData = new VectoRunData.StartStopData {
				Enabled = driver.StartStop.Enabled,
				Delay = driver.StartStop.Delay,
				MinTime = driver.StartStop.MinTime,
				MaxSpeed = driver.StartStop.MaxSpeed,
			};
			var retVal = new DriverData {
				AccelerationCurve = accelerationData,
				LookAheadCoasting = lookAheadData,
				OverSpeedEcoRoll = overspeedData,
				StartStop = startstopData,
			};
			return retVal;
		}

		//=================================
		public RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				return new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.Create(pto.PTOLossMap),
					PTOCycle = DrivingCycleDataReader.ReadFromDataTable(pto.PTOCycle, CycleType.PTO, "PTO", false)
				};
			}

			return null;
		}

		public AdvancedAuxData CreateAdvancedAuxData(IAuxiliariesEngineeringInputData auxInputData)
		{
			return new AdvancedAuxData() {
				AdvancedAuxiliaryFilePath = auxInputData.AdvancedAuxiliaryFilePath,
				AuxiliaryAssembly = auxInputData.AuxiliaryAssembly,
				AuxiliaryVersion = auxInputData.AuxiliaryVersion
			};
		}
	}
}