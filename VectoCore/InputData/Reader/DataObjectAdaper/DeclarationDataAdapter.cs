using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
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
				Mode = data.OverspeedEcoRoll.Mode,
				MinSpeed = DeclarationData.Driver.OverSpeedEcoRoll.MinSpeed,
				OverSpeed = DeclarationData.Driver.OverSpeedEcoRoll.OverSpeed,
				UnderSpeed = DeclarationData.Driver.OverSpeedEcoRoll.UnderSpeed
			};
			if (!DeclarationData.Driver.OverSpeedEcoRoll.AllowedModes.Contains(overspeedData.Mode)) {
				throw new VectoSimulationException("Specified Overspeed/EcoRoll Mode not allowed in declaration mode! {0}",
					overspeedData.Mode);
			}
			var startstopData = new VectoRunData.StartStopData {
				Enabled = data.StartStop.Enabled,
				Delay = DeclarationData.Driver.StartStop.Delay,
				MinTime = DeclarationData.Driver.StartStop.MinTime,
				MaxSpeed = DeclarationData.Driver.StartStop.MaxSpeed,
			};
			var retVal = new DriverData {
				LookAheadCoasting = lookAheadData,
				OverSpeedEcoRoll = overspeedData,
				StartStop = startstopData,
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

			retVal.CrossWindCorrectionMode = CrossWindCorrectionMode.DeclarationModeCorrection;
			retVal.AerodynamicDragAera = mission.UseCdA2
				? data.DragCoefficientRigidTruck
				: data.DragCoefficient;

			var axles = data.Axles;
			if (axles.Count < mission.AxleWeightDistribution.Length) {
				throw new VectoException(
					string.Format("Vehicle does not contain sufficient axles. {0} axles defined, {1} axles required",
						axles.Count, mission.AxleWeightDistribution.Count()));
			}
			var axleData = new List<Axle>();
			for (var i = 0; i < mission.AxleWeightDistribution.Length; i++) {
				var axleInput = axles[i];
				var axle = new Axle {
					AxleWeightShare = mission.AxleWeightDistribution[i],
					TwinTyres = axleInput.TwinTyres,
					RollResistanceCoefficient = axleInput.RollResistanceCoefficient,
					TyreTestLoad = axleInput.TyreTestLoad,
					Inertia = DeclarationData.Wheels.Lookup(axleInput.Wheels.Replace(" ", "")).Inertia,
				};
				axleData.Add(axle);
			}

			axleData.AddRange(mission.TrailerAxleWeightDistribution.Select(tmp => new Axle {
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
					throw new VectoSimulationException("Automatic Transmission currently not supported in DeclarationMode!");
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
					: FullLoadCurve.Create(gear.FullLoadCurve);

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
			foreach (var auxData in auxInputData.Auxiliaries) {
				var aux = new VectoRunData.AuxData { DemandType = AuxiliaryDemandType.Constant };

				switch (AuxiliaryTypeHelper.Parse(auxData.Type)) {
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
						aux.PowerDemand = DeclarationData.ElectricSystem.Lookup(mission, auxData.TechList.ToArray());
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
	}
}