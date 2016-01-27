using System.Linq;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdaper
{
	public abstract class AbstractSimulationDataAdapter : LoggingObject
	{
		// =========================

		internal VehicleData SetCommonVehicleData(IVehicleInputData data)
		{
			var retVal = new VehicleData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				VehicleCategory = data.VehicleCategory,
				AxleConfiguration = data.AxleConfiguration,
				CurbWeight = data.CurbWeight,
				//CurbWeigthExtra = data.CurbWeightExtra.SI<Kilogram>(),
				//Loading = data.Loading.SI<Kilogram>(),
				GrossVehicleMassRating = data.GrossVehicleMassRating,
				//DragCoefficient = data.DragCoefficient,
				//CrossSectionArea = data.CrossSectionArea.SI<SquareMeter>(),
				//DragCoefficientRigidTruck = data.DragCoefficientRigidTruck,
				//CrossSectionAreaRigidTruck = data.CrossSectionAreaRigidTruck.SI<SquareMeter>(),
				//TyreRadius = data.TyreRadius.SI().Milli.Meter.Cast<Meter>(),
				Rim = data.Rim,
			};

			return retVal;
		}

		internal RetarderData SetCommonRetarderData(IRetarderInputData data)
		{
			var retarder = new RetarderData {
				Type = data.Type,
			};
			if (retarder.Type == RetarderData.RetarderType.Primary || retarder.Type == RetarderData.RetarderType.Secondary) {
				retarder.LossMap = RetarderLossMap.Create(data.LossMap);
				retarder.Ratio = data.Ratio;
			}
			return retarder;
		}

		internal CombustionEngineData SetCommonCombustionEngineData(IEngineInputData data)
		{
			var retVal = new CombustionEngineData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				ModelName = data.ModelName,
				Displacement = data.Displacement,
				IdleSpeed = data.IdleSpeed,
				ConsumptionMap = FuelConsumptionMap.Create(data.FuelConsumptionMap),
				WHTCUrban = data.WHTCUrban,
				WHTCMotorway = data.WHTCMotorway,
				WHTCRural = data.WHTCRural,
			};
			return retVal;
		}

		internal GearboxData SetCommonGearboxData(IGearboxInputData data)
		{
			return new GearboxData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				ModelName = data.ModelName,
				Type = data.Type
			};
		}

		internal AxleGearData CreateAxleGearData(IAxleGearInputData axleGear)
		{
			var axleLossMap = TransmissionLossMap.Create(axleGear.LossMap, axleGear.Ratio, "AxleGear");
			return new AxleGearData() {
				AxleGear = new GearData() { LossMap = axleLossMap, Ratio = axleGear.Ratio, TorqueConverterActive = false }
			};
		}

		/// <summary>
		/// Intersects full load curves.
		/// </summary>
		/// <param name="engineCurve"></param>
		/// <param name="gearCurve"></param>
		/// <returns>A combined EngineFullLoadCurve with the minimum full load torque over all inputs curves.</returns>
		internal static EngineFullLoadCurve IntersectFullLoadCurves(EngineFullLoadCurve engineCurve, FullLoadCurve gearCurve)
		{
			if (gearCurve == null) {
				return engineCurve;
			}
			var entries = gearCurve.FullLoadEntries.Concat(engineCurve.FullLoadEntries)
				.Select(entry => entry.EngineSpeed)
				.OrderBy(engineSpeed => engineSpeed)
				.Distinct()
				.Select(engineSpeed => new FullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = engineSpeed,
					TorqueFullLoad =
						VectoMath.Min(engineCurve.FullLoadStationaryTorque(engineSpeed), gearCurve.FullLoadStationaryTorque(engineSpeed))
				});

			var flc = new EngineFullLoadCurve {
				FullLoadEntries = entries.ToList(),
				EngineData = engineCurve.EngineData,
				PT1Data = engineCurve.PT1Data
			};
			return flc;
		}
	}
}