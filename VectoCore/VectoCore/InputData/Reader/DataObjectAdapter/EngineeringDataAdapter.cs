/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using TUGraz.VectoCore.Configuration;
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
			retVal.BodyAndTrailerWeight = data.CurbMassExtra;
			//retVal.CurbWeight += data.CurbMassExtra;
			retVal.TrailerGrossVehicleWeight = 0.SI<Kilogram>();
			retVal.Loading = data.Loading;
			retVal.DynamicTyreRadius = data.DynamicTyreRadius;
			var axles = data.Axles;

			retVal.AxleData = axles.Select(axle => new Axle {
				WheelsDimension = axle.Tyre.Dimension,
				Inertia = axle.Tyre.Inertia,
				TwinTyres = axle.TwinTyres,
				RollResistanceCoefficient = axle.Tyre.RollResistanceCoefficient,
				AxleWeightShare = axle.AxleWeightShare,
				TyreTestLoad = axle.Tyre.TyreTestLoad,
				AxleType = axle.AxleType,
				//Wheels = axle.WheelsStr
			}).ToList();
			return retVal;
		}

		public AirdragData CreateAirdragData(IAirdragEngineeringInputData airdragData, IVehicleEngineeringInputData data)
		{
			var retVal = SetCommonAirdragData(airdragData);
			retVal.CrossWindCorrectionMode = airdragData.CrossWindCorrectionMode;

			switch (airdragData.CrossWindCorrectionMode) {
				case CrossWindCorrectionMode.NoCorrection:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.GetNoCorrectionCurve(airdragData.AirDragArea),
						CrossWindCorrectionMode.NoCorrection);
					break;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurve(airdragData.CrosswindCorrectionMap,
							airdragData.AirDragArea), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
					break;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionVAirBeta(airdragData.AirDragArea,
						CrossWindCorrectionCurveReader.ReadCdxABetaTable(airdragData.CrosswindCorrectionMap));
					break;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					var airDragArea = airdragData.AirDragArea ??
									DeclarationData.Segments.LookupCdA(data.VehicleCategory, data.AxleConfiguration, data.GrossVehicleMassRating);
					var height = data.Height ?? DeclarationData.Segments.LookupHeight(data.VehicleCategory, data.AxleConfiguration,
						data.GrossVehicleMassRating);
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(airDragArea,
						DeclarationDataAdapter.GetDeclarationAirResistanceCurve(
							GetAirdragParameterSet(data.VehicleCategory, data.AxleConfiguration, data.Axles.Count), airDragArea, height),
						CrossWindCorrectionMode.DeclarationModeCorrection);
					break;
				default:
					throw new ArgumentOutOfRangeException("CrosswindCorrection", airdragData.CrossWindCorrectionMode.ToString());
			}
			return retVal;
		}

		private string GetAirdragParameterSet(VehicleCategory vehicleCategory, AxleConfiguration axles, int numAxles)
		{
			switch (vehicleCategory) {
				case VehicleCategory.RigidTruck:
					return numAxles > axles.NumAxles() ? "RigidTrailer" : "RigidSolo";
				case VehicleCategory.Tractor:
					return "TractorSemitrailer";
				case VehicleCategory.CityBus:
				case VehicleCategory.InterurbanBus:
				case VehicleCategory.Coach:
					return "CoachBus";
				default:
					throw new ArgumentOutOfRangeException("vehicleCategory", vehicleCategory, null);
			}
		}

		private void WarnEngineeringMode(string msg)
		{
			Log.Error("{0} is in Declaration Mode but is used for Engineering Mode!", msg);
		}

		internal CombustionEngineData CreateEngineData(IEngineEngineeringInputData engine, IGearboxEngineeringInputData gbx,
			IEnumerable<ITorqueLimitInputData> torqueLimits)
		{
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine);
			retVal.Inertia = engine.Inertia +
							(gbx != null && gbx.Type.AutomaticTransmission() ? gbx.TorqueConverter.Inertia : 0.SI<KilogramSquareMeter>());
			var limits = torqueLimits.ToDictionary(e => e.Gear);
			var numGears = gbx == null ? 0 : gbx.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(engine.FullLoadCurve);
			fullLoadCurves[0].EngineData = retVal;
			if (gbx != null) {
				foreach (var gear in gbx.Gears) {
					var maxTorque = VectoMath.Min(gear.MaxTorque, limits.ContainsKey(gear.Gear) ? limits[gear.Gear].MaxTorque : null);
					fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
				}
			}
			retVal.FullLoadCurves = fullLoadCurves;
			retVal.FuelConsumptionCorrectionFactor = engine.WHTCEngineering;
			return retVal;
		}

		internal GearboxData CreateGearboxData(IGearboxEngineeringInputData gearbox, CombustionEngineData engineData,
			double axlegearRatio, Meter dynamicTyreRadius, VehicleCategory vehicleCategory, bool useEfficiencyFallback)
		{
			if (gearbox.SavedInDeclarationMode) {
				WarnEngineeringMode("GearboxData");
			}

			var retVal = SetCommonGearboxData(gearbox);

			//var gears = gearbox.Gears;
			if (gearbox.Gears.Count < 2) {
				throw new VectoSimulationException("At least two Gear-Entries must be defined in Gearbox!");
			}

			SetEngineeringData(gearbox, retVal);

			var gearDifferenceRatio = gearbox.Type.AutomaticTransmission() && gearbox.Gears.Count > 2
				? gearbox.Gears[0].Ratio / gearbox.Gears[1].Ratio
				: 1.0;

			var gears = new Dictionary<uint, GearData>();
			ShiftPolygon tcShiftPolygon = null;
			if (gearbox.Type.AutomaticTransmission()) {
				tcShiftPolygon = gearbox.TorqueConverter.ShiftPolygon != null
					? ShiftPolygonReader.Create(gearbox.TorqueConverter.ShiftPolygon)
					: DeclarationData.TorqueConverter.ComputeShiftPolygon(engineData.FullLoadCurves[0]);
			}
			for (uint i = 0; i < gearbox.Gears.Count; i++) {
				var gear = gearbox.Gears[(int)i];
				var lossMap = CreateGearLossMap(gear, i, useEfficiencyFallback, false);

				var shiftPolygon = gear.ShiftPolygon != null && gear.ShiftPolygon.SourceType != DataSourceType.Missing
					? ShiftPolygonReader.Create(gear.ShiftPolygon)
					: DeclarationData.Gearbox.ComputeShiftPolygon(gearbox.Type, (int)i, engineData.FullLoadCurves[i + 1], gearbox.Gears,
						engineData,
						axlegearRatio, dynamicTyreRadius);
				var gearData = new GearData {
					ShiftPolygon = shiftPolygon,
					MaxSpeed = gear.MaxInputSpeed,
					Ratio = gear.Ratio,
					LossMap = lossMap,
				};

				CreateATGearData(gearbox, i, gearData, tcShiftPolygon, gearDifferenceRatio, gears, vehicleCategory);
				gears.Add(i + 1, gearData);
			}
			retVal.Gears = gears;

			if (retVal.Type.AutomaticTransmission()) {
				var ratio = double.IsNaN(retVal.Gears[1].Ratio) ? 1 : retVal.Gears[1].TorqueConverterRatio / retVal.Gears[1].Ratio;
				retVal.PowershiftShiftTime = gearbox.PowershiftShiftTime;
				retVal.TorqueConverterData = TorqueConverterDataReader.Create(gearbox.TorqueConverter.TCData,
					gearbox.TorqueConverter.ReferenceRPM, gearbox.TorqueConverter.MaxInputSpeed, ExecutionMode.Engineering, ratio,
					gearbox.TorqueConverter.CLUpshiftMinAcceleration, gearbox.TorqueConverter.CCUpshiftMinAcceleration);
			}


			return retVal;
		}

		private static void CreateATGearData(IGearboxEngineeringInputData gearbox, uint i, GearData gearData,
			ShiftPolygon tcShiftPolygon, double gearDifferenceRatio, Dictionary<uint, GearData> gears,
			VehicleCategory vehicleCategory)
		{
			if (gearbox.Type == GearboxType.ATPowerSplit && i == 0) {
				// powersplit transmission: torque converter already contains ratio and losses
				CretateTCFirstGearATPowerSplit(gearData, i, tcShiftPolygon);
			}
			if (gearbox.Type == GearboxType.ATSerial) {
				if (i == 0) {
					// torqueconverter is active in first gear - duplicate ratio and lossmap for torque converter mode
					CreateTCFirstGearATSerial(gearData, tcShiftPolygon);
				}
				if (i == 1 && gearDifferenceRatio >= DeclarationData.Gearbox.TorqueConverterSecondGearThreshold(vehicleCategory)) {
					// ratio between first and second gear is above threshold, torqueconverter is active in second gear as well
					// -> duplicate ratio and lossmap for torque converter mode, remove locked transmission for previous gear
					CreateTCSecondGearATSerial(gearData, tcShiftPolygon);
					// NOTE: the lower gear in 'gears' dictionary has index i !!
					gears[i].Ratio = double.NaN;
					gears[i].LossMap = null;
				}
			}
		}

		private static void SetEngineeringData(IGearboxEngineeringInputData gearbox, GearboxData retVal)
		{
			retVal.Inertia = gearbox.Type.ManualTransmission() ? gearbox.Inertia : 0.SI<KilogramSquareMeter>();
			retVal.TractionInterruption = gearbox.TractionInterruption;
			retVal.TorqueReserve = gearbox.TorqueReserve;
			retVal.StartTorqueReserve = gearbox.StartTorqueReserve;
			retVal.ShiftTime = gearbox.MinTimeBetweenGearshift;
			retVal.StartSpeed = gearbox.StartSpeed;
			retVal.StartAcceleration = gearbox.StartAcceleration;
			retVal.DownshiftAfterUpshiftDelay = gearbox.DownshiftAfterUpshiftDelay;
			retVal.UpshiftAfterDownshiftDelay = gearbox.UpshiftAfterDownshiftDelay;
			retVal.UpshiftMinAcceleration = gearbox.UpshiftMinAcceleration;
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData)
		{
			var auxList = new List<VectoRunData.AuxData>(auxInputData.Auxiliaries.Count + 1) {
				new VectoRunData.AuxData { ID = Constants.Auxiliaries.Cycle, DemandType = AuxiliaryDemandType.Direct }
			};

			foreach (var a in auxInputData.Auxiliaries) {
				switch (a.AuxiliaryType) {
					case AuxiliaryDemandType.Mapping:
						auxList.Add(CreateMappingAuxiliary(a));
						break;
					case AuxiliaryDemandType.Constant:
						auxList.Add(CreateConstantAuxiliary(a));
						break;
					default:
						throw new VectoException("Auxiliary type {0} not supported!", a.AuxiliaryType);
				}
			}
			return auxList;
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

			if (driver.Lookahead == null) {
				throw new VectoSimulationException("Error: Lookahead Data is missing.");
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
			var retVal = new DriverData {
				AccelerationCurve = accelerationData,
				LookAheadCoasting = lookAheadData,
				OverSpeedEcoRoll = overspeedData,
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
				var ptoData = new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.Create(pto.PTOLossMap),
				};
				if (pto.PTOCycle != null) {
					ptoData.PTOCycle = DrivingCycleDataReader.ReadFromDataTable(pto.PTOCycle, CycleType.PTO, "PTO", false);
				}
				return ptoData;
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