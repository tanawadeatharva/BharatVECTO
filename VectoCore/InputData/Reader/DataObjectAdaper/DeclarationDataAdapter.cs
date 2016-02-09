/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdaper
{
	public class DeclarationDataAdapter : AbstractSimulationDataAdapter
	{
		public DriverData CreateDriverData(IDriverInputData data)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("DriverData");
			}
			var lookAheadData = new DriverData.LACData {
				Enabled = DeclarationData.Driver.LookAhead.Enabled,
				Deceleration = DeclarationData.Driver.LookAhead.Deceleration,
				MinSpeed = DeclarationData.Driver.LookAhead.MinimumSpeed
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
			var startstopData = new VectoRunData.StartStopData {
				Enabled = data.StartStop.Enabled,
				Delay = DeclarationData.Driver.StartStop.Delay,
				MinTime = DeclarationData.Driver.StartStop.MinTime,
				MaxSpeed = DeclarationData.Driver.StartStop.MaxSpeed
			};
			var retVal = new DriverData {
				LookAheadCoasting = lookAheadData,
				OverSpeedEcoRoll = overspeedData,
				StartStop = startstopData
			};
			return retVal;
		}

		internal VehicleData CreateVehicleData(IVehicleInputData data, Mission mission, Kilogram loading)
		{
			if (!data.SavedInDeclarationMode) {
				WarnDeclarationMode("VehicleData");
			}

			var retVal = SetCommonVehicleData(data);

			retVal.GrossVehicleMassRating = data.GrossVehicleMassRating;

			retVal.CurbWeigthExtra = mission.MassExtra;
			retVal.Loading = loading;
			retVal.DynamicTyreRadius =
				DeclarationData.DynamicTyreRadius(data.Axles[DeclarationData.PoweredAxle()].Wheels, data.Rim);

			var aerodynamicDragAera = mission.UseCdA2
				? data.AirDragAreaRigidTruck
				: data.AirDragArea;

			retVal.CrossWindCorrectionCurve = GetDeclarationAirResistanceCurve(retVal.VehicleCategory, aerodynamicDragAera);
			var axles = data.Axles;
			if (axles.Count < mission.AxleWeightDistribution.Length) {
				throw new VectoException("Vehicle does not contain sufficient axles. {0} axles defined, {1} axles required",
					data.Axles.Count, mission.AxleWeightDistribution.Count());
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

			axleData.AddRange(mission.TrailerAxleWeightDistribution.Select(tmp => new Axle {
				AxleType = AxleType.Trailer,
				AxleWeightShare = tmp,
				TwinTyres = DeclarationData.Trailer.TwinTyres,
				RollResistanceCoefficient = DeclarationData.Trailer.RollResistanceCoefficient,
				TyreTestLoad = DeclarationData.Trailer.TyreTestLoad.SI<Newton>(),
				Inertia = DeclarationData.Wheels.Lookup(DeclarationData.Trailer.WheelsType).Inertia
			}));
			retVal.AxleData = axleData;
			return retVal;
		}

		internal CombustionEngineData CreateEngineData(IEngineInputData engine)
		{
			if (!engine.SavedInDeclarationMode) {
				WarnDeclarationMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine);
			retVal.Inertia = DeclarationData.Engine.EngineInertia(retVal.Displacement);
			retVal.FullLoadCurve = EngineFullLoadCurve.Create(engine.FullLoadCurve, true);
			retVal.FullLoadCurve.EngineData = retVal;
			return retVal;
		}

		internal GearboxData CreateGearboxData(IGearboxInputData gearbox, CombustionEngineData engine)
		{
			if (!gearbox.SavedInDeclarationMode) {
				WarnDeclarationMode("GearboxData");
			}
			var retVal = SetCommonGearboxData(gearbox);
			switch (retVal.Type) {
				case GearboxType.AT:
					throw new VectoSimulationException(
						"Automatic Transmission currently not supported in DeclarationMode!");
				case GearboxType.Custom:
					throw new VectoSimulationException("Custom Transmission not supported in DeclarationMode!");
			}
			var gears = gearbox.Gears;
			if (gears.Count < 1) {
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}

			retVal.Inertia = DeclarationData.Gearbox.Inertia.SI<KilogramSquareMeter>();
			retVal.TractionInterruption = retVal.Type.TractionInterruption();
			retVal.SkipGears = retVal.Type.SkipGears();
			retVal.EarlyShiftUp = retVal.Type.EarlyShiftGears();

			retVal.TorqueReserve = DeclarationData.Gearbox.TorqueReserve;
			retVal.StartTorqueReserve = DeclarationData.Gearbox.TorqueReserveStart;
			retVal.ShiftTime = DeclarationData.Gearbox.MinTimeBetweenGearshifts.SI<Second>();
			retVal.StartSpeed = DeclarationData.Gearbox.StartSpeed.SI<MeterPerSecond>();
			retVal.StartAcceleration = DeclarationData.Gearbox.StartAcceleration.SI<MeterPerSquareSecond>();

			retVal.HasTorqueConverter = false;

			retVal.Gears = gears.Select((gear, i) => {
				var gearLossMap = TransmissionLossMap.Create(gear.LossMap, gear.Ratio, string.Format("Gear {0}", i + 1));
				var gearFullLoad = gear.FullLoadCurve == null
					? engine.FullLoadCurve
					: FullLoadCurve.Create(gear.FullLoadCurve, true);

				var fullLoadCurve = IntersectFullLoadCurves(engine.FullLoadCurve, gearFullLoad);
				var shiftPolygon = DeclarationData.Gearbox.ComputeShiftPolygon(fullLoadCurve, engine.IdleSpeed);
				return new KeyValuePair<uint, GearData>((uint)i + 1,
					new GearData {
						LossMap = gearLossMap,
						ShiftPolygon = shiftPolygon,
						FullLoadCurve = gearFullLoad ?? engine.FullLoadCurve,
						Ratio = gear.Ratio,
						TorqueConverterActive = false
					});
			}).ToDictionary(kv => kv.Key, kv => kv.Value);
			return retVal;
		}


		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesInputData auxInputData,
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
				var auxData = auxInputData.Auxiliaries.FirstOrDefault(a => AuxiliaryTypeHelper.Parse(a.Type) == auxType);
				if (auxData == null) {
					throw new VectoException("Auxiliary {0} not found.", auxType);
				}
				var aux = new VectoRunData.AuxData {
					DemandType = AuxiliaryDemandType.Constant,
					Technology = auxData.Technology
				};
				switch (auxType) {
					case AuxiliaryType.Fan:
						aux.PowerDemand = DeclarationData.Fan.Lookup(mission, auxData.Technology);
						aux.ID = Constants.Auxiliaries.IDs.Fan;
						break;
					case AuxiliaryType.SteeringPump:
						aux.PowerDemand = DeclarationData.SteeringPump.Lookup(mission, hvdClass, auxData.Technology);
						aux.ID = Constants.Auxiliaries.IDs.SteeringPump;
						break;
					case AuxiliaryType.HeatingVentilationAirCondition:
						aux.PowerDemand = DeclarationData.HeatingVentilationAirConditioning.Lookup(mission, hvdClass);
						aux.ID = Constants.Auxiliaries.IDs.HeatingVentilationAirCondition;
						break;
					case AuxiliaryType.PneumaticSystem:
						aux.PowerDemand = DeclarationData.PneumaticSystem.Lookup(mission, hvdClass);
						aux.ID = Constants.Auxiliaries.IDs.PneumaticSystem;
						break;
					case AuxiliaryType.ElectricSystem:
						aux.PowerDemand = DeclarationData.ElectricSystem.Lookup(mission,
							auxData.TechList.DefaultIfNull(Enumerable.Empty<string>()).ToArray());
						aux.ID = Constants.Auxiliaries.IDs.ElectricSystem;
						aux.TechList = auxData.TechList.DefaultIfNull(Enumerable.Empty<string>()).ToArray();
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


		public static CrossWindCorrectionCurve GetDeclarationAirResistanceCurve(VehicleCategory vehicleCategory,
			SquareMeter aerodynamicDragAera)
		{
			var values = DeclarationData.AirDrag.Lookup(vehicleCategory);
			var points = new List<CrossWindCorrectionCurve.CrossWindCorrectionEntry> {
				new CrossWindCorrectionCurve.CrossWindCorrectionEntry {
					Velocity = 0.SI<MeterPerSecond>(),
					EffectiveCrossSectionArea = 0.SI<SquareMeter>()
				}
			};
			for (var speed = 60; speed <= 100; speed += 5) {
				var vVeh = speed.KMPHtoMeterPerSecond();
				var cdASum = 0.0.SI<SquareMeter>();
				for (var alpha = 0; alpha <= 180; alpha += 10) {
					var vWindX = Physics.BaseWindSpeed * Math.Cos(alpha.ToRadian());
					var vWindY = Physics.BaseWindSpeed * Math.Sin(alpha.ToRadian());
					var vAirX = vVeh + vWindX;
					var vAirY = vWindY;
//					var vAir = VectoMath.Sqrt<MeterPerSecond>(vAirX * vAirX + vAirY * vAirY);
					var beta = Math.Atan((vAirY / vAirX).Value()).ToDegree();
					var deltaCdA = ComputeDeltaCd(beta, values);
					var cdA = aerodynamicDragAera + deltaCdA;

					var degreeShare = ((alpha != 0 && alpha != 180) ? 10.0 / 180.0 : 5.0 / 180.0);

//					cdASum += degreeShare * cdA * (vAir * vAir / (vVeh * vVeh)).Cast<Scalar>();
					cdASum += degreeShare * cdA * ((vAirX * vAirX + vAirY * vAirY) / (vVeh * vVeh)).Cast<Scalar>();
				}
				points.Add(new CrossWindCorrectionCurve.CrossWindCorrectionEntry {
					Velocity = vVeh,
					EffectiveCrossSectionArea = cdASum
				});
			}

			points[0].EffectiveCrossSectionArea = points[1].EffectiveCrossSectionArea;
			return new CrossWindCorrectionCurve(points, CrossWindCorrectionMode.DeclarationModeCorrection);
		}

		protected static SquareMeter ComputeDeltaCd(double beta, AirDrag.AirDragEntry values)
		{
			return (values.A1 * beta + values.A2 * beta * beta + values.A3 * beta * beta * beta).SI<SquareMeter>();
		}
	}
}