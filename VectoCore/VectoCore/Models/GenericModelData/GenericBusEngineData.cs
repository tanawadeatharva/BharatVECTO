using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.BusAuxiliaries;
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

		private static GenericBusEngineData _instance;

		public static GenericBusEngineData Instance
		{
			get { return _instance ?? (_instance = new GenericBusEngineData()); }
		}

		#endregion

		protected GenericBusEngineData()
		{
			
		}

		public CombustionEngineData CreateGenericBusEngineData(IVehicleDeclarationInputData primaryVehicle)
		{
			var engineData = primaryVehicle.Components.EngineInputData;
			var gearbox = primaryVehicle.Components.GearboxInputData;

			var engine = new CombustionEngineData {
				IdleSpeed = engineData.EngineModes[0].IdleSpeed,
				Displacement = engineData.Displacement,
				WHRType = WHRType.None,
				Inertia = DeclarationData.Engine.EngineInertia(engineData.Displacement, gearbox.Type),
				EngineStartTime = DeclarationData.Engine.DefaultEngineStartTime,
				RatedPowerDeclared = engineData.RatedPowerDeclared,
				RatedSpeedDeclared = engineData.RatedSpeedDeclared,
				MaxTorqueDeclared = engineData.MaxTorqueDeclared,
			};

		var limits = primaryVehicle.TorqueLimits.ToDictionary(e => e.Gear);
			var numGears = gearbox.Gears.Count;
			var fullLoadCurves = new Dictionary<uint, EngineFullLoadCurve>(numGears + 1);
			fullLoadCurves[0] = FullLoadCurveReader.Create(engineData.EngineModes.First().FullLoadCurve, true);
			fullLoadCurves[0].EngineData = engine;

			foreach (var gear in gearbox.Gears) {
				var maxTorque = VectoMath.Min(
					DeclarationDataAdapterHeavyLorry.GbxMaxTorque(gear, numGears, fullLoadCurves[0].MaxTorque),
					DeclarationDataAdapterHeavyLorry.VehMaxTorque(gear, numGears, limits, fullLoadCurves[0].MaxTorque));
				fullLoadCurves[(uint)gear.Gear] = AbstractSimulationDataAdapter.IntersectFullLoadCurves(fullLoadCurves[0], maxTorque);
			}

			engine.FullLoadCurves = fullLoadCurves;
			

			var fuel = GetCombustionEngineFuelData(primaryVehicle.Components.EngineInputData.EngineModes.First(), fullLoadCurves[0]);
			
			

			engine.Fuels = new List<CombustionEngineFuelData> { fuel };

			
			return engine;
		}


		private string GetEngineRessourceId(IEngineModeDeclarationInputData engineMode)
		{
			var fuelType = engineMode.Fuels.First().FuelType;
			var isDualFuel = engineMode.Fuels.Count > 1;
			
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

		private IFuelProperties GetFuelData(IEngineModeDeclarationInputData engineMode)
		{
			var fuelType = engineMode.Fuels.First().FuelType;
			var isDualFuel = engineMode.Fuels.Count > 1;

			if (isDualFuel)
				return FuelData.Diesel;

			switch (fuelType) {
				case FuelType.DieselCI:
				case FuelType.EthanolCI:
				case FuelType.NGCI:
					return FuelData.Diesel;
				default:
					return FuelData.Instance().Lookup(FuelType.NGPI, TankSystem.Compressed);
			}
		}


		private CombustionEngineFuelData GetCombustionEngineFuelData(IEngineModeDeclarationInputData engineMode,
			EngineFullLoadCurve fullLoadCurve)
		{
			var ressourceId = GetEngineRessourceId(engineMode);

			var nIdle = engineMode.IdleSpeed.AsRPM;
			var ratedSpeed = fullLoadCurve.RatedSpeed.AsRPM;
			var maxTorque = fullLoadCurve.MaxTorque.Value();
			
			var denormalizedData = DenormalizeData(ressourceId, nIdle, ratedSpeed, maxTorque);
			
			var engineSpeed = denormalizedData.AsEnumerable().Select(r => 
				r.Field<string>(FuelConsumptionMapReader.Fields.EngineSpeed).ToDouble()).ToArray();

			var clusterResult = new MeanShiftClustering().FindClusters(engineSpeed, 1);

			foreach (var entry in clusterResult) {
				var dragTorque = fullLoadCurve.DragLoadStationaryTorque(entry.RPMtoRad()).Value();
				var newRow = denormalizedData.NewRow();
				newRow[FuelConsumptionMapReader.Fields.EngineSpeed] = entry;
				newRow[FuelConsumptionMapReader.Fields.Torque] = dragTorque;
				newRow[FuelConsumptionMapReader.Fields.FuelConsumption] = 0;
				denormalizedData.Rows.Add(newRow);
				var newRow2 = denormalizedData.NewRow();
				newRow2[FuelConsumptionMapReader.Fields.EngineSpeed] = entry;
				newRow2[FuelConsumptionMapReader.Fields.Torque] = dragTorque - 100;
				newRow2[FuelConsumptionMapReader.Fields.FuelConsumption] = 0;
				denormalizedData.Rows.Add(newRow2);
			}

			;
			var fcMap = FuelConsumptionMapReader.Create(denormalizedData.AsEnumerable().OrderBy(r => r.Field<string>(FuelConsumptionMapReader.Fields.EngineSpeed).ToDouble())
																		.ThenBy(r => r.Field<string>(FuelConsumptionMapReader.Fields.Torque).ToDouble()).CopyToDataTable());

			var fuel = new CombustionEngineFuelData
			{
				WHTCUrban = 1,
				WHTCRural = 1,
				WHTCMotorway = 1,
				ColdHotCorrectionFactor = 1,
				CorrectionFactorRegPer = 1,
				ConsumptionMap = fcMap,
				FuelData = GetFuelData(engineMode)
			};

			return fuel;
		}

		


		private DataTable DenormalizeData(string ressourceId, double nIdle, double ratedSpeed, double maxTorque)
		{
			var normalized = VectoCSVFile.ReadStream(RessourceHelper.ReadStream(ressourceId), source: ressourceId);

			var result = new DataTable();
			result.Columns.Add(FuelConsumptionMapReader.Fields.EngineSpeed);
			result.Columns.Add(FuelConsumptionMapReader.Fields.Torque);
			result.Columns.Add(FuelConsumptionMapReader.Fields.FuelConsumption);

			foreach (DataRow row in normalized.Rows) {
				var engineSpeed = DenormalizeEngineSpeed((string)row[FuelConsumptionMapReader.Fields.EngineSpeed],
					nIdle, ratedSpeed);
				var torque = DenormalizeTorque((string)row[FuelConsumptionMapReader.Fields.Torque], maxTorque);
				var fc = DenormalizeFC((string)row[FuelConsumptionMapReader.Fields.FuelConsumption], maxTorque);

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
