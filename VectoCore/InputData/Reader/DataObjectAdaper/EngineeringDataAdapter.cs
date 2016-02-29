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
using System.IO;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdaper
{
	public class EngineeringDataAdapter : AbstractSimulationDataAdapter
	{
		internal VehicleData CreateVehicleData(IVehicleEngineeringInputData data)
		{
			if (data.SavedInDeclarationMode) {
				WarnEngineeringMode("VehicleData");
			}

			var retVal = SetCommonVehicleData(data);

			retVal.CurbWeigthExtra = data.CurbWeightExtra;
			retVal.Loading = data.Loading;
			retVal.DynamicTyreRadius = data.DynamicTyreRadius;
			retVal.CrossWindCorrectionMode = data.CrossWindCorrectionMode;
			switch (data.CrossWindCorrectionMode) {
				case CrossWindCorrectionMode.NoCorrection:
					retVal.CrossWindCorrectionCurve =
						new CrosswindCorrectionCdxALookup(CrossWindCorrectionCurveReader.GetNoCorrectionCurve(data.AirDragArea),
							CrossWindCorrectionMode.NoCorrection);
					break;
				case CrossWindCorrectionMode.SpeedDependentCorrectionFactor:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionCdxALookup(
						CrossWindCorrectionCurveReader.ReadSpeedDependentCorrectionCurve(data.CrosswindCorrectionMap,
							data.AirDragArea), CrossWindCorrectionMode.SpeedDependentCorrectionFactor);
					break;
				case CrossWindCorrectionMode.VAirBetaLookupTable:
					retVal.CrossWindCorrectionCurve = new CrosswindCorrectionVAirBeta(data.AirDragArea,
						CrossWindCorrectionCurveReader.ReadCdxABetaTable(data.CrosswindCorrectionMap));
					break;
				case CrossWindCorrectionMode.DeclarationModeCorrection:
					retVal.CrossWindCorrectionCurve =
						new CrosswindCorrectionCdxALookup(DeclarationDataAdapter.GetDeclarationAirResistanceCurve(retVal.VehicleCategory,
							data.AirDragArea), CrossWindCorrectionMode.DeclarationModeCorrection);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}


			var axles = data.Axles;
			retVal.AxleData = axles.Select(axle => new Axle {
				WheelsDimension = axle.Wheels,
				Inertia = axle.Inertia,
				TwinTyres = axle.TwinTyres,
				RollResistanceCoefficient = axle.RollResistanceCoefficient,
				AxleWeightShare = axle.AxleWeightShare,
				TyreTestLoad = axle.TyreTestLoad,
				//Wheels = axle.WheelsStr
			}).ToList();
			return retVal;
		}

		private void WarnEngineeringMode(string msg)
		{
			Log.Warn("{0} is in Declaration Mode but is used for Engineering Mode!", msg);
		}

		internal CombustionEngineData CreateEngineData(IEngineEngineeringInputData engine)
		{
			if (engine.SavedInDeclarationMode) {
				WarnEngineeringMode("EngineData");
			}

			var retVal = SetCommonCombustionEngineData(engine);
			retVal.Inertia = engine.Inertia;
			retVal.FullLoadCurve = EngineFullLoadCurve.Create(engine.FullLoadCurve);
			retVal.FullLoadCurve.EngineData = retVal;
			return retVal;
		}

		internal GearboxData CreateGearboxData(IGearboxEngineeringInputData gearbox, CombustionEngineData engineData)
		{
			if (gearbox.SavedInDeclarationMode) {
				WarnEngineeringMode("GearboxData");
			}

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
					FullLoadCurve = gearFullLoad,
					Ratio = gear.Ratio,
					TorqueConverterActive = gear.TorqueConverterActive
				});
			}).ToDictionary(kv => kv.Key, kv => kv.Value);
			return retVal;
		}

		public IList<VectoRunData.AuxData> CreateAuxiliaryData(IAuxiliariesEngineeringInputData auxInputData)
		{
			if (auxInputData.SavedInDeclarationMode) {
				WarnEngineeringMode("AuxData");
			}

			return auxInputData.Auxiliaries.Select(a => new VectoRunData.AuxData {
				ID = a.ID,
				Technology = a.Technology,
				TechList = a.TechList.DefaultIfNull(Enumerable.Empty<string>()).ToArray(),
				DemandType = AuxiliaryDemandType.Mapping,
				Data = new AuxiliaryData(a) //AuxiliaryData.Create(a.DemandMap)
			}).Concat(new VectoRunData.AuxData { ID = "", DemandType = AuxiliaryDemandType.Direct }.ToEnumerable()).ToList();
		}

		internal DriverData CreateDriverData(IDriverEngineeringInputData driver)
		{
			if (driver.SavedInDeclarationMode) {
				WarnEngineeringMode("DriverData");
			}

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
				Mode = driver.OverSpeedEcoRoll.Mode,
				MinSpeed = driver.OverSpeedEcoRoll.MinSpeed,
				OverSpeed = driver.OverSpeedEcoRoll.OverSpeed,
				UnderSpeed = driver.OverSpeedEcoRoll.UnderSpeed,
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
		public RetarderData CreateRetarderData(IRetarderInputData retarder, IVehicleEngineeringInputData vehicle)
		{
			return SetCommonRetarderData(retarder, vehicle);
		}
	}
}