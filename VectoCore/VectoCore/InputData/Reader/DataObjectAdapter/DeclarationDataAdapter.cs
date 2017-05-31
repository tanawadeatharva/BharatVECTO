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
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
{
	public class DeclarationDataAdapter : AbstractSimulationDataAdapter
	{
		public DriverData CreateDriverData(IDriverDeclarationInputData data)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("DriverData");
			}
			var lookAheadData = new DriverData.LACData {
				Enabled = DeclarationData.Driver.LookAhead.Enabled,
				//Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
				MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed,
				LookAheadDecisionFactor = new LACDecisionFactor(),
				LookAheadDistanceFactor = DeclarationData.Driver.LookAhead.LookAheadDistanceFactor,
			};
			var overspeedData = new DriverData.OverSpeedEcoRollData {
				Mode = data.OverSpeedEcoRoll.Mode,
				MinSpeed = DeclarationData.Driver.OverSpeedEcoRoll.MinSpeed,
				OverSpeed = DeclarationData.Driver.OverSpeedEcoRoll.OverSpeed,
				UnderSpeed = DeclarationData.Driver.OverSpeedEcoRoll.UnderSpeed
			};
			if (!DeclarationData.Driver.OverSpeedEcoRoll.AllowedModes.Contains(overspeedData.Mode)) {
				throw new VectoSimulationException(
					"Specified Overspeed/EcoRoll Mode not allowed in declaration mode! {0}",
					overspeedData.Mode);
			}
			var retVal = new DriverData {
				LookAheadCoasting = lookAheadData,
				OverSpeedEcoRoll = overspeedData,
			};
			return retVal;
		}

		internal VehicleData CreateVehicleData(IVehicleDeclarationInputData data, Mission mission, Kilogram loading,
			Kilogram municipalBodyWeight)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("VehicleData");
			}

			var retVal = SetCommonVehicleData(data);
			retVal.TrailerGrossVehicleWeight = mission.Trailer.Sum(t => t.TrailerGrossVehicleWeight).DefaultIfNull(0);

			retVal.BodyAndTrailerWeight = (mission.MissionType == MissionType.MunicipalUtility
				? municipalBodyWeight
				: mission.BodyCurbWeight) + mission.Trailer.Sum(t => t.TrailerCurbWeight).DefaultIfNull(0);
			//retVal.CurbWeight += retVal.BodyAndTrailerWeight;

			retVal.Loading = loading;
			var drivenIndex = DrivenAxleIndex(data.Axles);
			retVal.DynamicTyreRadius =
				DeclarationData.Wheels.Lookup(data.Axles[drivenIndex].Wheels).DynamicTyreRadius;
			retVal.CargoVolume = mission.MissionType != MissionType.Construction ? mission.TotalCargoVolume : 0.SI<CubicMeter>();


			var axles = data.Axles;
			if (axles.Count < mission.AxleWeightDistribution.Length) {
				throw new VectoException("Vehicle does not contain sufficient axles. {0} axles defined, {1} axles required",
					data.Axles.Count, mission.AxleWeightDistribution.Length);
			}
			var axleData = new List<Axle>();
			for (var i = 0; i < mission.AxleWeightDistribution.Length; i++) {
				var axleInput = axles[i];
				var axle = new Axle {
					WheelsDimension = axleInput.Wheels,
					AxleType = axleInput.AxleType,
					AxleWeightShare = mission.AxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.RollResistanceCoefficient,
					TyreTestLoad = axleInput.TyreTestLoad,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Wheels.RemoveWhitespace()).Inertia,
				};
				axleData.Add(axle);
			}

			foreach (var trailer in mission.Trailer) {
				axleData.AddRange(trailer.TrailerWheels.Select(trailerWheel => new Axle {
					AxleType = AxleType.Trailer,
					AxleWeightShare = trailer.TrailerAxleWeightShare / trailer.TrailerWheels.Count,
					TwinTyres = DeclarationData.Trailer.TwinTyres,
					RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
					TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
					Inertia = trailerWheel.Inertia
				}));
			}
			retVal.AxleData = axleData;
			return retVal;
		}

		private static int DrivenAxleIndex(IList<IAxleDeclarationInputData> axles)
		{
			for (var i = 0; i < axles.Count; i++) {
				if (axles[i].AxleType != AxleType.VehicleDriven) {
					continue;
				}
				return i;
			}
			return DeclarationData.PoweredAxle();
		}

		internal CombustionEngineData CreateEngineData(IEngineDeclarationInputData engine,
			IGearboxDeclarationInputData gearbox, IEnumerable<ITorqueLimitInputData> torqueLimits)
		{
			if (!engine.SavedInDeclarationMode) {
				WarnDeclarationMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine);
			retVal.WHTCUrban = engine.WHTCUrban;
			retVal.WHTCMotorway = engine.WHTCMotorway;
			retVal.WHTCRural = engine.WHTCRural;
			retVal.ColdHotCorrectionFactor = engine.ColdHotBalancingFactor;
			retVal.Inertia = DeclarationData.Engine.EngineInertia(retVal.Displacement, gearbox.Type);
			var limits = torqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = EngineFullLoadCurve.Create(engine.FullLoadCurve, true);
			fullLoadCurves[0].EngineData = retVal;
			foreach (var gear in gearbox.Gears) {
				var maxTorque = VectoMath.Min(
					GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}
			retVal.FullLoadCurves = fullLoadCurves;
			return retVal;
		}

		private static NewtonMeter VehMaxTorque(ITransmissionInputData gear, int numGears,
			Dictionary<int, ITorqueLimitInputData> limits,
			NewtonMeter maxEngineTorque)
		{
			if (gear.Gear - 1 >= numGears / 2) {
				// only upper half of gears can limit if max-torque <= 0.95 of engine max torque
				if (limits.ContainsKey(gear.Gear) &&
					limits[gear.Gear].MaxTorque <= DeclarationData.Engine.TorqueLimitVehicleFactor * maxEngineTorque) {
					return limits[gear.Gear].MaxTorque;
				}
			}
			return null;
		}

		private static NewtonMeter GbxMaxTorque(ITransmissionInputData gear, int numGears, NewtonMeter maxEngineTorque
			)
		{
			if (gear.Gear - 1 < numGears / 2) {
				// gears count from 1 to n, -> n entries, lower half can always limit
				if (gear.MaxTorque != null) {
					return gear.MaxTorque;
				}
			} else {
				// upper half can only limit if max-torque <= 90% of engine max torque
				if (gear.MaxTorque != null && gear.MaxTorque <= DeclarationData.Engine.TorqueLimitGearboxFactor * maxEngineTorque) {
					return gear.MaxTorque;
				}
			}
			return null;
		}

		internal GearboxData CreateGearboxData(IGearboxDeclarationInputData gearbox, CombustionEngineData engine,
			double axlegearRatio, Meter dynamicTyreRadius, bool useEfficiencyFallback)
		{
			if (!gearbox.SavedInDeclarationMode) {
				WarnDeclarationMode("GearboxData");
			}
			var retVal = SetCommonGearboxData(gearbox);
			switch (retVal.Type) {
				case GearboxType.ATPowerSplit:
				case GearboxType.ATSerial:
					throw new VectoSimulationException(
						"Automatic Transmission currently not supported in DeclarationMode!");
				//case GearboxType.Custom:
				//	throw new VectoSimulationException("Custom Transmission not supported in DeclarationMode!");
			}
			var gears = gearbox.Gears;
			if (gears.Count < 1) {
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}

			retVal.Inertia = DeclarationData.Gearbox.Inertia;
			retVal.TractionInterruption = retVal.Type.TractionInterruption();

			retVal.TorqueReserve = DeclarationData.Gearbox.TorqueReserve;
			retVal.StartTorqueReserve = DeclarationData.Gearbox.TorqueReserveStart;
			retVal.ShiftTime = DeclarationData.Gearbox.MinTimeBetweenGearshifts;
			retVal.StartSpeed = DeclarationData.Gearbox.StartSpeed;
			retVal.StartAcceleration = DeclarationData.Gearbox.StartAcceleration;

			TransmissionLossMap gearLossMap;
			retVal.Gears = gears.Select((gear, i) => {
				uint gearNbr = (uint)(i + 1);
				try {
					if (gear.LossMap == null) {
						throw new InvalidFileFormatException(string.Format("LossMap for Gear {0} is missing.", i + 1));
					}
					gearLossMap = TransmissionLossMapReader.Create(gear.LossMap, gear.Ratio, string.Format("Gear {0}", i + 1));
				} catch (InvalidFileFormatException) {
					if (useEfficiencyFallback) {
						gearLossMap = TransmissionLossMapReader.Create(gear.Efficiency, gear.Ratio, string.Format("Gear {0}", i + 1));
					} else {
						throw;
					}
				}

				var shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(i, engine.FullLoadCurves[gearNbr], gears, engine,
					axlegearRatio, dynamicTyreRadius);

				return new KeyValuePair<uint, GearData>(gearNbr,
					new GearData {
						LossMap = gearLossMap,
						ShiftPolygon = shiftPolygon,
						MaxSpeed = gear.MaxInputSpeed,
						Ratio = gear.Ratio,
					});
			}).ToDictionary(kv => kv.Key, kv => kv.Value);

			retVal.DownshiftAfterUpshiftDelay = DeclarationData.Gearbox.DownshiftAfterUpshiftDelay;
			retVal.UpshiftAfterDownshiftDelay = DeclarationData.Gearbox.UpshiftAfterDownshiftDelay;
			retVal.UpshiftMinAcceleration = DeclarationData.Gearbox.UpshiftMinAcceleration;
			return retVal;
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesDeclarationInputData auxInputData,
			MissionType mission, VehicleClass hvdClass)
		{
			if (!auxInputData.SavedInDeclarationMode) {
				WarnDeclarationMode("AuxiliariesData");
			}
			var retVal = new List<VectoRunData.AuxData>();

			if (auxInputData.Auxiliaries.Count != 5) {
				Log.Error(
					"In Declaration Mode exactly 5 Auxiliaries must be defined: Fan, Steering pump, HVAC, Electric System, Pneumatic System.");
				throw new VectoException(
					"In Declaration Mode exactly 5 Auxiliaries must be defined: Fan, Steering pump, HVAC, Electric System, Pneumatic System.");
			}

			foreach (var auxType in EnumHelper.GetValues<AuxiliaryType>()) {
				var auxData = auxInputData.Auxiliaries.FirstOrDefault(a => a.Type == auxType);
				if (auxData == null) {
					throw new VectoException("Auxiliary {0} not found.", auxType);
				}
				var aux = new VectoRunData.AuxData {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = auxData.Technology
				};

				mission = mission.GetNonEMSMissionType();
				switch (auxType) {
					case AuxiliaryType.Fan:
						aux.PowerDemand = DeclarationData.Fan.Lookup(mission, auxData.Technology.FirstOrDefault());
						aux.ID = Constants.Auxiliaries.IDs.Fan;
						break;
					case AuxiliaryType.SteeringPump:
						aux.PowerDemand = DeclarationData.SteeringPump.Lookup(mission, hvdClass, auxData.Technology);
						aux.ID = Constants.Auxiliaries.IDs.SteeringPump;
						break;
					case AuxiliaryType.HVAC:
						aux.PowerDemand = DeclarationData.HeatingVentilationAirConditioning.Lookup(mission, hvdClass);
						aux.ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition;
						break;
					case AuxiliaryType.PneumaticSystem:
						aux.PowerDemand = DeclarationData.PneumaticSystem.Lookup(mission, auxData.Technology.FirstOrDefault());
						aux.ID = Constants.Auxiliaries.IDs.PneumaticSystem;
						break;
					case AuxiliaryType.ElectricSystem:
						aux.PowerDemand = DeclarationData.ElectricSystem.Lookup(mission, auxData.Technology.FirstOrDefault());
						aux.ID = Constants.Auxiliaries.IDs.ElectricSystem;
						break;
					default:
						continue;
				}
				retVal.Add(aux);
			}
			return retVal;
		}

		private void WarnDeclarationMode(string inputData)
		{
			Log.Warn("{0} not in Declaration Mode!", inputData);
		}

		public RetarderData CreateRetarderData(IRetarderInputData retarder)
		{
			return SetCommonRetarderData(retarder);
		}

		public static List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> GetDeclarationAirResistanceCurve(
			string crosswindCorrectionParameters, SquareMeter aerodynamicDragAera, Meter vehicleHeight)
		{
			const int startSpeed = 60;
			const int maxSpeed = 130;
			const int speedStep = 5;

			const int maxAlpha = 180;
			const int alphaStep = 10;

			const int startHeightPercent = 5;
			const int maxHeightPercent = 100;
			const int heightPercentStep = 10;
			const double heightShare = (double)heightPercentStep / maxHeightPercent;

			var values = DeclarationData.AirDrag.Lookup(crosswindCorrectionParameters);

			// first entry (0m/s) will get CdxA of second entry.
			var points = new List<CrossWindCorrectionCurveReader.CrossWindCorrectionEntry> {
				new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
					Velocity = 0.SI<MeterPerSecond>(),
					EffectiveCrossSectionArea = 0.SI<SquareMeter>()
				}
			};

			for (var speed = startSpeed; speed <= maxSpeed; speed += speedStep) {
				var vVeh = speed.KMPHtoMeterPerSecond();

				var cdASum = 0.SI<SquareMeter>();

				for (var heightPercent = startHeightPercent; heightPercent < maxHeightPercent; heightPercent += heightPercentStep) {
					var height = (heightPercent / 100.0) * vehicleHeight;
					var vWind = Physics.BaseWindSpeed * Math.Pow(height / Physics.BaseWindHeight, Physics.HellmannExponent);

					for (var alpha = 0; alpha <= maxAlpha; alpha += alphaStep) {
						var vAirX = vVeh + vWind * Math.Cos(alpha.ToRadian());
						var vAirY = vWind * Math.Sin(alpha.ToRadian());

						var beta = Math.Atan(vAirY / vAirX).ToDegree();

						// ΔCdxA = A1β + A2β² + A3β³
						var deltaCdA = values.A1 * beta + values.A2 * beta * beta + values.A3 * beta * beta * beta;

						// CdxA(β) = CdxA(0) + ΔCdxA(β)
						var cdA = aerodynamicDragAera + deltaCdA;

						var share = (alpha == 0 || alpha == maxAlpha ? alphaStep / 2.0 : alphaStep) / maxAlpha;

						// v_air = sqrt(v_airX²+vAirY²)
						// cdASum = CdxA(β) * v_air²/v_veh²
						cdASum += heightShare * share * cdA * (vAirX * vAirX + vAirY * vAirY) / (vVeh * vVeh);
					}
				}
				points.Add(new CrossWindCorrectionCurveReader.CrossWindCorrectionEntry {
					Velocity = vVeh,
					EffectiveCrossSectionArea = cdASum
				});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return points;
		}

		public PTOData CreatePTOTransmissionData(IPTOTransmissionInputData pto)
		{
			if (pto.PTOTransmissionType != "None") {
				return new PTOData {
					TransmissionType = pto.PTOTransmissionType,
					LossMap = PTOIdleLossMapReader.GetZeroLossMap(),
				};
			}

			return null;
		}

		public AirdragData CreateAirdragData(IAirdragDeclarationInputData airdragInputData, Mission mission,
			Segment segment)
		{
			if (airdragInputData == null) {
				return DefaultAirdragData(mission, segment);
			}

			var retVal = SetCommonAirdragData(airdragInputData);
			var aerodynamicDragArea = airdragInputData.AirDragArea + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			retVal.DeclaredAirdragArea = airdragInputData.AirDragArea;
			retVal.CrossWindCorrectionCurve =
				new CrosswindCorrectionCdxALookup(aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(mission.CrossWindCorrectionParameters, aerodynamicDragArea, segment.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection);
			return retVal;
		}

		private AirdragData DefaultAirdragData(Mission mission, Segment segment)
		{
			var aerodynamicDragArea = mission.MissionType == MissionType.Construction
				? segment.CdAConstruction
				: segment.CdADefault + mission.Trailer.Sum(t => t.DeltaCdA).DefaultIfNull(0);

			return new AirdragData() {
				CertificationMethod = CertificationMethod.StandardValues,
				DeclaredAirdragArea = mission.MissionType == MissionType.Construction ? segment.CdAConstruction : segment.CdADefault,
				CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(aerodynamicDragArea,
					GetDeclarationAirResistanceCurve(mission.CrossWindCorrectionParameters, aerodynamicDragArea, segment.VehicleHeight),
					CrossWindCorrectionMode.DeclarationModeCorrection)
			};
		}
	}
}