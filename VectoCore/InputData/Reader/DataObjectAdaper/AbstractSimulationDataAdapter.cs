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

using System.Collections.Generic;
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

		internal VehicleData SetCommonVehicleData(IVehicleDeclarationInputData data)
		{
			var retVal = new VehicleData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				ModelName = data.ModelName,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
				VehicleCategory = data.VehicleCategory,
				AxleConfiguration = data.AxleConfiguration,
				CurbWeight = data.CurbWeightChassis,
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

		internal RetarderData SetCommonRetarderData(IRetarderInputData data, IVehicleDeclarationInputData vehicle)
		{
			var retarder = new RetarderData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				ModelName = data.ModelName,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
				Type = data.Type,
			};
			if (retarder.Type == RetarderData.RetarderType.Primary || retarder.Type == RetarderData.RetarderType.Secondary) {
				retarder.LossMap = RetarderLossMap.Create(data.LossMap);
				retarder.Ratio = vehicle.RetarderRatio;
			}
			return retarder;
		}

		internal CombustionEngineData SetCommonCombustionEngineData(IEngineDeclarationInputData data)
		{
			var retVal = new CombustionEngineData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				ModelName = data.ModelName,
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

		internal GearboxData SetCommonGearboxData(IGearboxDeclarationInputData data)
		{
			return new GearboxData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				ModelName = data.ModelName,
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
				ModelName = data.ModelName,
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
				.OrderBy(x => x.EngineSpeed)
				.Distinct(new FullLoadEntryEqualityComparer())
				.Select(x => new FullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = x.EngineSpeed,
					TorqueFullLoad =
						VectoMath.Min(engineCurve.FullLoadStationaryTorque(x.EngineSpeed),
							gearCurve.FullLoadStationaryTorque(x.EngineSpeed))
				});

			var flc = new EngineFullLoadCurve {
				FullLoadEntries = entries.ToList(),
				EngineData = engineCurve.EngineData,
				PT1Data = engineCurve.PT1Data
			};
			return flc;
		}

		internal class FullLoadEntryEqualityComparer : IEqualityComparer<FullLoadCurve.FullLoadCurveEntry>
		{
			public bool Equals(FullLoadCurve.FullLoadCurveEntry x, FullLoadCurve.FullLoadCurveEntry y)
			{
				return x.EngineSpeed.Value().IsEqual(y.EngineSpeed.Value());
			}

			public int GetHashCode(FullLoadCurve.FullLoadCurveEntry obj)
			{
				return obj.EngineSpeed.Value().GetHashCode();
			}
		}
	}
}