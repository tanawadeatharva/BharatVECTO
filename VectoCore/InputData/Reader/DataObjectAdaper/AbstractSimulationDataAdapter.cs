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
				Vendor = data.Vendor,
				MakeAndModel = data.MakeAndModel,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
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
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				MakeAndModel = data.MakeAndModel,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
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
				Vendor = data.Vendor,
				MakeAndModel = data.MakeAndModel,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
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
				Vendor = data.Vendor,
				MakeAndModel = data.MakeAndModel,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
				Type = data.Type
			};
		}

		internal AxleGearData CreateAxleGearData(IAxleGearInputData data)
		{
			var axleLossMap = TransmissionLossMap.Create(data.LossMap, data.Ratio, "AxleGear");
			return new AxleGearData() {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				MakeAndModel = data.MakeAndModel,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
				AxleGear = new GearData() { LossMap = axleLossMap, Ratio = data.Ratio, TorqueConverterActive = false }
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