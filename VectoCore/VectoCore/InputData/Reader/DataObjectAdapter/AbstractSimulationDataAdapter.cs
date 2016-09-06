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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;

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
				GrossVehicleWeight = data.GrossVehicleMassRating,
			};

			return retVal;
		}

		internal RetarderData SetCommonRetarderData(IRetarderInputData data)
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
					//case RetarderType.EngineRetarder:
					case RetarderType.TransmissionInputRetarder:
					case RetarderType.TransmissionOutputRetarder:
						retarder.LossMap = RetarderLossMapReader.Create(data.LossMap);
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
				ConsumptionMap = FuelConsumptionMapReader.Create(data.FuelConsumptionMap),
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
				axleLossMap = TransmissionLossMapReader.Create(data.LossMap, data.Ratio, "AxleGear");
			} catch (InvalidFileFormatException) {
				if (useEfficiencyFallback) {
					axleLossMap = TransmissionLossMapReader.Create(data.Efficiency, data.Ratio, "AxleGear");
				} else {
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
				AxleGear = new GearData { LossMap = axleLossMap, Ratio = data.Ratio }
			};
		}

		/// <summary>
		/// Creates an AngularGearData or returns null if there is no angular gear.
		/// </summary>
		/// <param name="data"></param>
		/// <param name="useEfficiencyFallback">if true, the Efficiency value is used if no LossMap is found.</param>
		/// <returns></returns>
		internal AngularGearData CreateAngularGearData(IAngularGearInputData data, bool useEfficiencyFallback)
		{
			try {
				var type = AngularGearType.None;
				try {
					type = data.Type;
				} catch (InvalidFileFormatException) {
					Log.Info("AngularGear not found. Assuming None.");
				}

				switch (type) {
					case AngularGearType.LossesIncludedInGearbox:
					case AngularGearType.None:
						return null;
					case AngularGearType.SeparateAngularGear:
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
							AngularGear = new TransmissionData { Ratio = data.Ratio }
						};
						try {
							angularGear.AngularGear.LossMap = TransmissionLossMapReader.Create(data.LossMap, data.Ratio, "AngularGear");
						} catch (VectoException ex) {
							Log.Info("AngularGear Loss Map not found.");
							if (useEfficiencyFallback) {
								Log.Info("AngularGear Trying with Efficiency instead of Loss Map.");
								angularGear.AngularGear.LossMap = TransmissionLossMapReader.Create(data.Efficiency, data.Ratio, "AngularGear");
							} else {
								throw new VectoException("AngularGear: LossMap not found.", ex);
							}
						}
						return angularGear;
					default:
						throw new ArgumentOutOfRangeException("data", "Unknown Angulargear Type.");
				}
			} catch (Exception e) {
				throw new VectoException("Error while reading AngularGear data: {0}", e.Message);
			}
		}

		/// <summary>
		/// Intersects full load curves.
		/// </summary>
		/// <param name="engineCurve"></param>
		/// <param name="maxTorque"></param>
		/// <returns>A combined EngineFullLoadCurve with the minimum full load torque over all inputs curves.</returns>
		internal static EngineFullLoadCurve IntersectFullLoadCurves(EngineFullLoadCurve engineCurve, NewtonMeter maxTorque)
		{
			if (maxTorque == null) {
				return engineCurve;
			}

			var entries = new List<FullLoadCurve.FullLoadCurveEntry>();
			var firstEntry = engineCurve.FullLoadEntries.First();
			if (firstEntry.TorqueFullLoad < maxTorque) {
				entries.Add(engineCurve.FullLoadEntries.First());
			} else {
				entries.Add(new FullLoadCurve.FullLoadCurveEntry {
					EngineSpeed = firstEntry.EngineSpeed,
					TorqueFullLoad = maxTorque,
					TorqueDrag = firstEntry.TorqueDrag
				});
			}
			foreach (var entry in engineCurve.FullLoadEntries.Pairwise(Tuple.Create)) {
				if (entry.Item1.TorqueFullLoad <= maxTorque && entry.Item2.TorqueFullLoad <= maxTorque) {
					// segment is below maxTorque line -> use directly
					entries.Add(entry.Item2);
				} else if (entry.Item1.TorqueFullLoad > maxTorque && entry.Item2.TorqueFullLoad > maxTorque) {
					// segment is above maxTorque line -> add limited entry
					entries.Add(new FullLoadCurve.FullLoadCurveEntry {
						EngineSpeed = entry.Item2.EngineSpeed,
						TorqueFullLoad = maxTorque,
						TorqueDrag = entry.Item2.TorqueDrag
					});
				} else {
					// segment intersects maxTorque line -> add new entry at intersection
					var edgeFull = Edge.Create(new Point(entry.Item1.EngineSpeed.Value(), entry.Item1.TorqueFullLoad.Value()),
						new Point(entry.Item2.EngineSpeed.Value(), entry.Item2.TorqueFullLoad.Value()));
					var edgeDrag = Edge.Create(new Point(entry.Item1.EngineSpeed.Value(), entry.Item1.TorqueDrag.Value()),
						new Point(entry.Item2.EngineSpeed.Value(), entry.Item2.TorqueDrag.Value()));
					var intersectionX = (maxTorque.Value() - edgeFull.OffsetXY) / edgeFull.SlopeXY;
					entries.Add(new FullLoadCurve.FullLoadCurveEntry {
						EngineSpeed = intersectionX.SI<PerSecond>(),
						TorqueFullLoad = maxTorque,
						TorqueDrag = VectoMath.Interpolate(edgeDrag.P1, edgeDrag.P2, intersectionX).SI<NewtonMeter>()
					});
					entries.Add(new FullLoadCurve.FullLoadCurveEntry {
						EngineSpeed = entry.Item2.EngineSpeed,
						TorqueFullLoad = entry.Item2.TorqueFullLoad > maxTorque ? maxTorque : entry.Item2.TorqueFullLoad,
						TorqueDrag = entry.Item2.TorqueDrag
					});
				}
			}

			var flc = new EngineFullLoadCurve {
				FullLoadEntries = entries.ToList(),
				EngineData = engineCurve.EngineData,
				PT1Data = engineCurve.PT1Data
			};
			return flc;
		}
	}
}