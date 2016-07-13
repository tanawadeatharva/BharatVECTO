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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter
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
				GrossVehicleWeight = data.GrossVehicleMassRating,
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
			try {
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
				switch (retarder.Type) {
					case RetarderType.Primary:
					case RetarderType.Secondary:
						retarder.LossMap = RetarderLossMap.Create(data.LossMap);
						retarder.Ratio = data.Ratio;
						break;
					case RetarderType.None:
					case RetarderType.LossesIncludedInTransmission:
						retarder.Ratio = 1;
						break;
					default:
						// ReSharper disable once NotResolvedInText
						// ReSharper disable once LocalizableElement
						throw new ArgumentOutOfRangeException("retarder.Type", "RetarderType unknown");
				}

				return retarder;
			} catch (Exception e) {
				throw new VectoException("Error while Reading Retarder Data: {0}", e.Message);
			}
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

		internal AxleGearData CreateAxleGearData(IAxleGearInputData data, bool useEfficiencyFallback)
		{
			TransmissionLossMap axleLossMap;
			try {
				axleLossMap = TransmissionLossMap.Create(data.LossMap, data.Ratio, "AxleGear");
			} catch (InvalidFileFormatException) {
				if (useEfficiencyFallback)
					axleLossMap = TransmissionLossMap.Create(data.Efficiency, data.Ratio, "AxleGear");
				else {
					throw;
				}
			}

			return new AxleGearData {
				SavedInDeclarationMode = data.SavedInDeclarationMode,
				Vendor = data.Vendor,
				ModelName = data.ModelName,
				Creator = data.Creator,
				Date = data.Date,
				TypeId = data.TypeId,
				DigestValue = data.DigestValue,
				IntegrityStatus = data.IntegrityStatus,
				AxleGear = new GearData { LossMap = axleLossMap, Ratio = data.Ratio, TorqueConverterActive = false }
			};
		}

		internal AngularGearData CreateAngularGearData(IAngularGearInputData data, bool useEfficiencyFallback)
		{
			try {
				var type = AngularGearType.None;
				try {
					type = data.Type;
				} catch (Exception) {
					Log.Info("AngularGear not found. Assuming None.");
					// ignored
				}

				var angularGear = new AngularGearData {
					SavedInDeclarationMode = data.SavedInDeclarationMode,
					Vendor = data.Vendor,
					ModelName = data.ModelName,
					Creator = data.Creator,
					Date = data.Date,
					TypeId = data.TypeId,
					DigestValue = data.DigestValue,
					IntegrityStatus = data.IntegrityStatus,
					Type = type,
					AngularGear = new TransmissionData()
				};

				switch (angularGear.Type) {
					case AngularGearType.SeparateAngularGear:
						try {
							angularGear.AngularGear.LossMap = TransmissionLossMap.Create(data.LossMap, data.Ratio, "AngularGear");
						} catch (VectoException) {
							if (useEfficiencyFallback)
								angularGear.AngularGear.LossMap = TransmissionLossMap.Create(data.Efficiency, data.Ratio, "AngularGear");
							else {
								throw;
							}
						}
						angularGear.AngularGear.Ratio = data.Ratio;
						break;

					case AngularGearType.LossesIncludedInGearbox:
					case AngularGearType.None:
						angularGear.AngularGear.Ratio = 1;
						// if no angular gear or already included: Create LossMap with 100% efficiency
						angularGear.AngularGear.LossMap = TransmissionLossMap.Create(1, angularGear.AngularGear.Ratio,
							"Zero Losses AngularGear");
						break;
					default:
						throw new ArgumentOutOfRangeException("data", "Unknown AngularGear Type.");
				}
				return angularGear;
			} catch (Exception e) {
				throw new VectoException("Error while reading AngularGear data: {0}", e.Message);
			}
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
			// TODO mk-2016-04-18: refactor when new gearbox full load is implemented: gearbox will then only have 1 constant value as full load.
			var entries =
				gearCurve.FullLoadEntries.Concat(engineCurve.FullLoadEntries)
					.OrderBy(x => x.EngineSpeed)
					.DistinctBy((x, y) => x.EngineSpeed.Value().IsEqual(y.EngineSpeed.Value()))
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
	}
}