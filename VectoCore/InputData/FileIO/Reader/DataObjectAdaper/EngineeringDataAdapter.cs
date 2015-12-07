using System.Collections.Generic;
using System.IO;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData.FileIO.EngineeringFile;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.FileIO.Reader.DataObjectAdaper
{
	public class EngineeringDataAdapter : AbstractSimulationDataAdapter
	{
		internal VehicleData CreateVehicleData(IVehicleInputData data, Mission mission,
			Kilogram loading)
		{
			var retVal = SetCommonVehicleData(data);

			retVal.CurbWeigthExtra = data.CurbWeightExtra;
			retVal.Loading = data.Loading;
			retVal.DynamicTyreRadius = data.DynamicTyreRadius;

			retVal.CrossWindCorrectionMode = data.CrossWindCorrectionMode;
			retVal.AerodynamicDragAera = data.DragCoefficient;

			var axles = data.Axles;
			retVal.AxleData = axles.Select(axle => new Axle {
				Inertia = axle.Inertia,
				TwinTyres = axle.TwinTyres,
				RollResistanceCoefficient = axle.RollResistanceCoefficient,
				AxleWeightShare = axle.AxleWeightShare,
				TyreTestLoad = axle.TyreTestLoad,
				//Wheels = axle.WheelsStr
			}).ToList();
			return retVal;
		}

		internal CombustionEngineData CreateEngineData(IEngineInputData engine)
		{
			var retVal = SetCommonCombustionEngineData(engine);
			retVal.Inertia = engine.Inertia;
			retVal.FullLoadCurve = EngineFullLoadCurve.Create(engine.FullLoadCurve);
			retVal.FullLoadCurve.EngineData = retVal;
			return retVal;
		}

		internal GearboxData CreateGearboxData(IGearboxInputData gearbox, CombustionEngineData engineData)
		{
			var retVal = SetCommonGearboxData(gearbox);

			var gears = gearbox.Gears;
			if (gears.Count < 1) {
				throw new VectoSimulationException(
					"At least one Gear-Entry must be defined in Gearbox!");
			}

			retVal.Inertia = gearbox.Inertia;
			retVal.TractionInterruption = gearbox.TractionInterruption;
			retVal.SkipGears = gearbox.SkipGears;
			retVal.EarlyShiftUp = gearbox.EarlyShiftUp;
			retVal.TorqueReserve = gearbox.TorqueReserve;
			retVal.StartTorqueReserve = gearbox.StartTorqueReserve;
			retVal.ShiftTime = gearbox.ShiftTime;
			retVal.StartSpeed = gearbox.StartSpeed;
			retVal.StartAcceleration = gearbox.StartAcceleration;

			retVal.HasTorqueConverter = gearbox.TorqueConverter.Enabled;

			//var engineFullLoadCurve = (engineData != null) ? engineData.FullLoadCurve : null;

			retVal.Gears = gears.Select((gear, i) => {
				var lossMap = TransmissionLossMap.Create(gear.LossMap, gear.Ratio, string.Format("Gear {0}", i + 1));
				var gearFullLoad = gear.FullLoadCurve != null
					? FullLoadCurve.Create(gear.FullLoadCurve)
					: null;
				var fullLoadCurve = IntersectFullLoadCurves(engineData.FullLoadCurve, gearFullLoad);
				var shiftPolygon = gear.ShiftPolygon != null
					? ShiftPolygon.Create(gear.ShiftPolygon)
					: DeclarationData.Gearbox.ComputeShiftPolygon(fullLoadCurve, engineData.IdleSpeed);

				return new KeyValuePair<uint, GearData>((uint)(i + 1), new GearData {
					LossMap = lossMap,
					ShiftPolygon = shiftPolygon,
					FullLoadCurve = gearFullLoad ?? engineData.FullLoadCurve,
					Ratio = gear.Ratio,
					TorqueConverterActive = gear.TorqueConverterActive
				});
			}).ToDictionary(kv => kv.Key, kv => kv.Value);
			return retVal;
		}

		internal DriverData CreateDriverData(IDriverInputData driver)
		{
			AccelerationCurveData accelerationData = null;
			if (driver.AccelerationCurve != null) {
				accelerationData = AccelerationCurveData.Create(driver.AccelerationCurve);
			}

			var lookAheadData = new DriverData.LACData {
				Enabled = driver.Lookahead.Enabled,
				Deceleration = driver.Lookahead.Deceleration,
				MinSpeed = driver.Lookahead.MinSpeed,
			};
			var overspeedData = new DriverData.OverSpeedEcoRollData {
				Mode = driver.OverspeedEcoRoll.Mode,
				MinSpeed = driver.OverspeedEcoRoll.MinSpeed,
				OverSpeed = driver.OverspeedEcoRoll.OverSpeed,
				UnderSpeed = driver.OverspeedEcoRoll.UnderSpeed,
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
	}
}