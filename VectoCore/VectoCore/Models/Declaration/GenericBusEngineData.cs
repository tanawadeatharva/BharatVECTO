using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.InputData.Reader.DataObjectAdapter;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Declaration
{
	public class GenericBusEngineData
	{
		#region Constans

		private static string GenericEngineCM_Normed_CI =
			$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EngineConsumptionMap_CI_Normed.vmap";

		private static string GenericEngineCM_Normed_PI =
			$"{DeclarationData.DeclarationDataResourcePrefix}.GenericBusData.EngineConsumptionMap_PI_Normed.vmap";

		#endregion

		public CombustionEngineData CreateGenericBusEngineData(IVehicleDeclarationInputData pifVehicle)
		{
			var enginePif = pifVehicle.Components.EngineInputData;
			var gearbox = pifVehicle.Components.GearboxInputData;

			var engine = new CombustionEngineData();

			var limits = pifVehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(enginePif.EngineModes.First().FullLoadCurve, true);
			fullLoadCurves[0].EngineData = engine;

			foreach (var gear in gearbox.Gears)
			{
				var maxTorque = VectoMath.Min(
					DeclarationDataAdapterHeavyLorry.GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					DeclarationDataAdapterHeavyLorry.VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			engine.FullLoadCurves = fullLoadCurves;
			engine.IdleSpeed = enginePif.EngineModes[0].IdleSpeed;
			engine.Displacement = enginePif.Displacement;

			var fuel = GetCombustionEngineFuelData(pifVehicle, fullLoadCurves[0]);
			
			engine.WHRType = WHRType.None;

			engine.Fuels = new List<CombustionEngineFuelData> { fuel };

			engine.Inertia = DeclarationData.Engine.EngineInertia(engine.Displacement, gearbox.Type);
			return engine;
		}


		private string GetEngineRessourceId(IVehicleDeclarationInputData vehiclePif)
		{
			var fuelType = vehiclePif.Components.EngineInputData.EngineModes.First().Fuels.First().FuelType;
			var isDualFuel = vehiclePif.DualFuelVehicle;
			
			if (isDualFuel)
				return GenericEngineCM_Normed_CI;

			switch (fuelType)
			{
				case FuelType.DieselCI:
				case FuelType.EthanolCI:
				case FuelType.NGCI:
					return GenericEngineCM_Normed_CI;
				default:
					return GenericEngineCM_Normed_PI;
			}
		}


		private CombustionEngineFuelData GetCombustionEngineFuelData(IVehicleDeclarationInputData vehiclePif,
			EngineFullLoadCurve fullLoadCurve)
		{
			var ressourceId = GetEngineRessourceId(vehiclePif);

			var nIdle = vehiclePif.Components.EngineInputData.RatedSpeedDeclared.AsRPM;
			var nRated = fullLoadCurve.RatedSpeed.Value();
			var mRated = fullLoadCurve.MaxTorque.Value();
			
			var denormalizedData = DenormalizeData(ressourceId, nIdle, nRated, mRated);
			
			var engineSpeed = denormalizedData.AsEnumerable().Select(r => 
				r.Field<string>(FuelConsumptionMapReader.Fields.EngineSpeed).ToDouble()).ToArray();

			var clusterResult = new MeanShiftClustering().FindClusters(engineSpeed, 1);
			
			for (int i = 0; i < clusterResult.Length; i++) {
				var currentTorque = fullLoadCurve.DragLoadStationaryTorque(clusterResult[i].RPMtoRad()).Value();
				SetDragLoadFuelConsumption(denormalizedData, clusterResult[i], currentTorque);
			}
			
			var fcMap = FuelConsumptionMapReader.Create(denormalizedData);

			var fuel = new CombustionEngineFuelData
			{
				WHTCUrban = 1,
				WHTCRural = 1,
				WHTCMotorway = 1,
				ColdHotCorrectionFactor = 1,
				CorrectionFactorRegPer = 1,
				ConsumptionMap = fcMap
			};

			return fuel;
		}

		private void SetDragLoadFuelConsumption(DataTable currentDataTable, double engineSpeed, double torque)
		{
			for (int i = 0; i < currentDataTable.Rows.Count; i++) {
				var currentRowSpeed = currentDataTable.Rows[i]
					[FuelConsumptionMapReader.Fields.EngineSpeed].ToString().ToDouble();

				if (currentRowSpeed.IsEqual(engineSpeed)) {
					var newRow = currentDataTable.NewRow();
					newRow[FuelConsumptionMapReader.Fields.EngineSpeed] = engineSpeed;
					newRow[FuelConsumptionMapReader.Fields.Torque] = torque;
					newRow[FuelConsumptionMapReader.Fields.FuelConsumption] = 0;
					currentDataTable.Rows.InsertAt(newRow, i);
					break;
				}
			}
		}
		
		private DataTable DenormalizeData(string ressourceId, double nIdle, double nRated, double mRated)
		{
			var normedData = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);

			var result = new DataTable();
			result.Columns.Add(FuelConsumptionMapReader.Fields.EngineSpeed);
			result.Columns.Add(FuelConsumptionMapReader.Fields.Torque);
			result.Columns.Add(FuelConsumptionMapReader.Fields.FuelConsumption);

			foreach (DataRow row in normedData.Rows)
			{
				var engineSpeed = DenormalizeEngineSpeed((string)row[FuelConsumptionMapReader.Fields.EngineSpeed],
					nIdle, nRated);
				var torque = DenormalizeTorque((string)row[FuelConsumptionMapReader.Fields.Torque], mRated);
				var fc = DenormalizeFC((string)row[FuelConsumptionMapReader.Fields.FuelConsumption], mRated);

				var newRow = result.NewRow();
				newRow[FuelConsumptionMapReader.Fields.EngineSpeed] = engineSpeed;
				newRow[FuelConsumptionMapReader.Fields.Torque] = torque;
				newRow[FuelConsumptionMapReader.Fields.FuelConsumption] = fc;
				result.Rows.Add(newRow);
			}

			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double DenormalizeFC(string fc, double mRated)
		{
			return fc.ToDouble() * mRated;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double DenormalizeTorque(string torque, double mRated)
		{
			return torque.ToDouble() * mRated;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private double DenormalizeEngineSpeed(string engineSpeed, double nIdle, double nRated)
		{
			return engineSpeed.ToDouble() * (nRated - nIdle) + nIdle;
		}

	}
}
