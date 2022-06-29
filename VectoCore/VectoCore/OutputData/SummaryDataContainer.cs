/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.BusAuxiliaries;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

// ReSharper disable MemberCanBePrivate.Global  -- used by API!

namespace TUGraz.VectoCore.OutputData
{
	//public delegate void WriteSumData(IModalDataContainer data);

	public interface ISumData
	{
		void Write(IModalDataContainer modData, VectoRunData runData);
		void RegisterComponent(VectoSimulationComponent component);
	}

	/// <summary>
	/// Class for the sum file in vecto.
	/// </summary>
	public class SummaryDataContainer : LoggingObject, ISumData, IDisposable
	{
		protected readonly string[] FcColumns = {
			SumDataFields.FCMAP_H, SumDataFields.FCMAP_KM,
			SumDataFields.FCNCVC_H, SumDataFields.FCNCVC_KM,
			SumDataFields.FCWHTCC_H, SumDataFields.FCWHTCC_KM,
			SumDataFields.FCESS_H, SumDataFields.FCESS_KM,
			SumDataFields.FCESS_H_CORR, SumDataFields.FCESS_KM_CORR,
			SumDataFields.FC_BusAux_PS_CORR_H, SumDataFields.FC_BusAux_PS_CORR_KM,
			SumDataFields.FC_BusAux_ES_CORR_H, SumDataFields.FC_BusAux_ES_CORR_KM,
			SumDataFields.FCWHR_H_CORR, SumDataFields.FCWHR_KM_CORR,
			SumDataFields.FC_HEV_SOC_H, SumDataFields.FC_HEV_SOC_KM,
			SumDataFields.FC_HEV_SOC_CORR_H, SumDataFields.FC_HEV_SOC_CORR_KM,
			SumDataFields.FC_AUXHTR_H, SumDataFields.FC_AUXHTR_KM,
			SumDataFields.FC_AUXHTR_H_CORR, SumDataFields.FC_AUXHTR_KM_CORR,
			SumDataFields.FCFINAL_H, SumDataFields.FCFINAL_KM, SumDataFields.FCFINAL_LITERPER100KM, SumDataFields.FCFINAL_LITERPER100TKM,
			SumDataFields.FCFINAL_LiterPer100M3KM, SumDataFields.FCFINAL_LiterPer100PassengerKM,
			SumDataFields.SPECIFIC_FC, SumDataFields.K_VEHLINE, SumDataFields.K_ENGLINE
		};

		private object _tableLock = new object();
		internal readonly DataTable Table;
		private readonly ISummaryWriter _sumWriter;

		protected SummaryDataContainer() { }

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryDataContainer"/> class.
		/// </summary>
		/// <param name="writer"></param>
		public SummaryDataContainer(ISummaryWriter writer)
		{
			_sumWriter = writer;
			Table = new DataTable();
			InitTableColumns();
		}

		public void RegisterComponent(VectoSimulationComponent component)
		{

		}

		private void InitTableColumns()
		{
			lock (Table) {
				Table.Columns.AddRange(
					new[] {
						Tuple.Create(SumDataFields.SORT, typeof(int)),
						Tuple.Create(SumDataFields.JOB, typeof(string)),
						Tuple.Create(SumDataFields.INPUTFILE, typeof(string)),
						Tuple.Create(SumDataFields.CYCLE, typeof(string)),
						Tuple.Create(SumDataFields.STATUS, typeof(string)),
						Tuple.Create(SumDataFields.VEHICLE_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.VIN_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.VEHICLE_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.HDV_CO2_VEHICLE_CLASS, typeof(string)),
						Tuple.Create(SumDataFields.CURB_MASS, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.LOADING, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.PassengerCount, typeof(double)),
						Tuple.Create(SumDataFields.TOTAL_VEHICLE_MASS, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.ENGINE_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_FUEL_TYPE, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_RATED_POWER, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.ENGINE_IDLING_SPEED, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.ENGINE_RATED_SPEED, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.ENGINE_DISPLACEMENT, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.ENGINE_WHTC_URBAN, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_WHTC_RURAL, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_WHTC_MOTORWAY, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_BF_COLD_HOT, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_CF_REG_PER, typeof(string)),
						Tuple.Create(SumDataFields.ENGINE_ACTUAL_CORRECTION_FACTOR, typeof(string)),
						Tuple.Create(SumDataFields.VEHICLE_FUEL_TYPE, typeof(string)),
						Tuple.Create(SumDataFields.AIRDRAG_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.CD_x_A_DECLARED, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.CD_x_A, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.SLEEPER_CAB, typeof(string)),
						Tuple.Create(SumDataFields.DECLARED_RRC_AXLE1, typeof(double)),
						Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE1, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.DECLARED_RRC_AXLE2, typeof(double)),
						Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE2, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.DECLARED_RRC_AXLE3, typeof(double)),
						Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE3, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.DECLARED_RRC_AXLE4, typeof(double)),
						Tuple.Create(SumDataFields.DECLARED_FZISO_AXLE4, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER, typeof(double)),
						Tuple.Create(SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER, typeof(double)),
						Tuple.Create(SumDataFields.R_DYN, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.NUM_AXLES_DRIVEN, typeof(int)),
						Tuple.Create(SumDataFields.NUM_AXLES_NON_DRIVEN, typeof(int)),
						Tuple.Create(SumDataFields.NUM_AXLES_TRAILER, typeof(int)),
						Tuple.Create(SumDataFields.GEARBOX_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.GEARBOX_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.GEARBOX_TYPE, typeof(string)),
						Tuple.Create(SumDataFields.GEAR_RATIO_FIRST_GEAR, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.GEAR_RATIO_LAST_GEAR, typeof(ConvertedSI)),
						Tuple.Create(SumDataFields.TORQUECONVERTER_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.TORQUECONVERTER_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.RETARDER_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.RETARDER_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.RETARDER_TYPE, typeof(string)),
						Tuple.Create(SumDataFields.ANGLEDRIVE_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.ANGLEDRIVE_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.ANGLEDRIVE_RATIO, typeof(string)),
						Tuple.Create(SumDataFields.AXLE_MANUFACTURER, typeof(string)),
						Tuple.Create(SumDataFields.AXLE_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.AXLE_RATIO, typeof(ConvertedSI)),
						Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.SteeringPump),
							typeof(string)),
						Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.Fan),
							typeof(string)),
						Tuple.Create(
							string.Format(SumDataFields.AUX_TECH_FORMAT,
								Constants.Auxiliaries.IDs.HeatingVentilationAirCondition),
							typeof(string)),
						Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.PneumaticSystem),
							typeof(string)),
						Tuple.Create(string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.ElectricSystem),
							typeof(string)),
						Tuple.Create(SumDataFields.TCU_MODEL, typeof(string)),
						Tuple.Create(SumDataFields.ADAS_TECHNOLOGY_COMBINATION, typeof(string)),
						Tuple.Create(SumDataFields.PTO_TECHNOLOGY, typeof(string)),
						Tuple.Create(SumDataFields.REESS_CAPACITY, typeof(string)),
						//Tuple.Create(PTO_OTHER_ELEMENTS, typeof(string)),
					}.Select(x => new DataColumn(x.Item1, x.Item2)).ToArray());

				Table.Columns.AddRange(
					new[] {
						SumDataFields.CARGO_VOLUME, SumDataFields.TIME, SumDataFields.DISTANCE, SumDataFields.SPEED, SumDataFields.ALTITUDE_DELTA,
					}.Select(x => new DataColumn(x, typeof(ConvertedSI))).ToArray());


				Table.Columns.AddRange(
					new[] {
						SumDataFields.CO2_KM, SumDataFields.CO2_TKM, SumDataFields.CO2_M3KM, SumDataFields.CO2_PKM, SumDataFields.P_WHEEL,
						SumDataFields.P_WHEEL_POS, SumDataFields.P_FCMAP, SumDataFields.P_FCMAP_POS,
						SumDataFields.E_FCMAP_POS, SumDataFields.E_FCMAP_NEG, SumDataFields.E_POWERTRAIN_INERTIA, SumDataFields.E_AUX,
						SumDataFields.E_AUX_EL_HV, SumDataFields.E_CLUTCH_LOSS,
						SumDataFields.E_TC_LOSS, SumDataFields.E_SHIFT_LOSS, SumDataFields.E_GBX_LOSS, SumDataFields.E_RET_LOSS,
						SumDataFields.E_ANGLE_LOSS,
						SumDataFields.E_AXL_LOSS, SumDataFields.E_BRAKE, SumDataFields.E_VEHICLE_INERTIA, SumDataFields.E_WHEEL, SumDataFields.E_AIR,
						SumDataFields.E_ROLL, SumDataFields.E_GRAD,
						SumDataFields.AirConsumed, SumDataFields.AirGenerated, SumDataFields.E_PS_CompressorOff, SumDataFields.E_PS_CompressorOn,
						SumDataFields.E_BusAux_ES_consumed, SumDataFields.E_BusAux_ES_generated, SumDataFields.Delta_E_BusAux_Battery,
						SumDataFields.E_BusAux_PS_corr, SumDataFields.E_BusAux_ES_mech_corr,
						SumDataFields.E_BusAux_HVAC_Mech, SumDataFields.E_BusAux_HVAC_El,
						SumDataFields.E_BusAux_AuxHeater,
						SumDataFields.E_WHR_EL, SumDataFields.E_WHR_MECH, SumDataFields.E_ICE_START, SumDataFields.E_AUX_ESS_missing,
						SumDataFields.NUM_ICE_STARTS, SumDataFields.ACC,
						SumDataFields.ACC_POS, SumDataFields.ACC_NEG, SumDataFields.ACC_TIMESHARE, SumDataFields.DEC_TIMESHARE,
						SumDataFields.CRUISE_TIMESHARE,
						SumDataFields.MAX_SPEED, SumDataFields.MAX_ACCELERATION, SumDataFields.MAX_DECELERATION, SumDataFields.AVG_ENGINE_SPEED,
						SumDataFields.MAX_ENGINE_SPEED, SumDataFields.NUM_GEARSHIFTS, SumDataFields.STOP_TIMESHARE,
						SumDataFields.ICE_FULL_LOAD_TIME_SHARE, SumDataFields.ICE_OFF_TIME_SHARE,
						SumDataFields.COASTING_TIME_SHARE, SumDataFields.BRAKING_TIME_SHARE, SumDataFields.AVERAGE_POS_ACC
					}.Select(x => new DataColumn(x, typeof(ConvertedSI))).ToArray());

				Table.Columns.AddRange(
					new[] {
						Tuple.Create(SumDataFields.ENGINE_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.AVERAGE_ENGINE_EFFICIENCY, typeof(double)),
						Tuple.Create(SumDataFields.TORQUE_CONVERTER_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(SumDataFields.TORQUE_CONVERTER_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP, typeof(double)),
						Tuple.Create(SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP, typeof(double)),
						Tuple.Create(SumDataFields.GEARBOX_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(SumDataFields.GEARBOX_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.AVERAGE_GEARBOX_EFFICIENCY, typeof(double)),
						Tuple.Create(SumDataFields.RETARDER_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(SumDataFields.RETARDER_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.ANGLEDRIVE_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(SumDataFields.ANGLEDRIVE_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.AVERAGE_ANGLEDRIVE_EFFICIENCY, typeof(double)),
						Tuple.Create(SumDataFields.AXLEGEAR_CERTIFICATION_METHOD, typeof(string)),
						Tuple.Create(SumDataFields.AXLEGEAR_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.AVERAGE_AXLEGEAR_EFFICIENCY, typeof(double)),
						Tuple.Create(SumDataFields.AIRDRAG_CERTIFICATION_NUMBER, typeof(string)),
						Tuple.Create(SumDataFields.AIRDRAG_CERTIFICATION_METHOD, typeof(string)),
					}.Select(x => new DataColumn(x.Item1, x.Item2)).ToArray());
			}
		}

		/// <summary>
		/// Finishes the summary data container (writes the data to the sumWriter).
		/// </summary>
		public virtual void Finish()
		{
			if (_sumWriter != null) {
				lock (Table) {
					var view = new DataView(Table, "", SumDataFields.SORT, DataViewRowState.CurrentRows).ToTable();

					var probablyEmptyCols = new[] { SumDataFields.E_WHEEL, SumDataFields.SPECIFIC_FC }.Select(x =>
						x.Contains("{") ? x.Substring(0, x.IndexOf("{", StringComparison.Ordinal)) : x).ToArray();
					var removeCandidates =
						view.Columns.Cast<DataColumn>()
							.Where(column => probablyEmptyCols.Any(x => column.ColumnName.StartsWith(x))).ToList();
					var toRemove = new List<string>();
					foreach (var column in removeCandidates) {
						//var column = view.Columns[colName];
						if (view.AsEnumerable().All(dr => dr.IsNull(column))) {
							toRemove.Add(column.ColumnName);
						}
					}

					toRemove = toRemove.Concat(
						view.Columns.Cast<DataColumn>()
							.Where(column => column.ColumnName.StartsWith(SumDataFields.INTERNAL_PREFIX))
							.Select(x => x.ColumnName)).ToList();

					foreach (var dataColumn in toRemove) {
						view.Columns.Remove(dataColumn);
					}

					try {
						_sumWriter.WriteSumData(view);
					} catch (Exception e) {
						LogManager.GetLogger(typeof(SummaryDataContainer).FullName).Error(e.Message);
					}
				}
			}
		}

		private void UpdateTableColumns(ICollection<IFuelProperties> modDataFuelData,
			bool engineDataMultipleEngineFuelModes)
		{
			foreach (var entry in modDataFuelData) {
				foreach (var column in FcColumns.Reverse()) {
					var colName = string.Format(column,
						modDataFuelData.Count <= 1 && !engineDataMultipleEngineFuelModes
							? ""
							: "_" + entry.FuelType.GetLabel());
					lock (Table) {
						if (!Table.Columns.Contains(colName)) {
							var col = new DataColumn(colName, typeof(ConvertedSI));
							Table.Columns.Add(col);
							col.SetOrdinal(Table.Columns.IndexOf(SumDataFields.ALTITUDE_DELTA) + 1);
						}
					}
				}
			}
		}

		/// <summary>
		/// Writes the result of one run into the summary data container.
		/// </summary>
		//[MethodImpl(MethodImplOptions.Synchronized)]
		protected DataRow GetResultRow(IModalDataContainer modData, VectoRunData runData)
		{
			lock (_tableLock) {
				if (modData.HasCombustionEngine) {
					UpdateTableColumns(modData.FuelData, runData.EngineData.MultipleEngineFuelModes);
				}
			}

			lock (Table) {
				var row = Table.NewRow();
				//Table.Rows.Add(row);
				return row;
			}
		}

		protected Dictionary<string, object> GetResultDictionary(IModalDataContainer modData, VectoRunData runData)
		{
			if (modData.HasCombustionEngine) {
				lock (_tableLock) {
					UpdateTableColumns(modData.FuelData, runData.EngineData.MultipleEngineFuelModes);
				}
			}

			return new Dictionary<string, object>();
		}

		protected void AddResultRow(DataRow row)
		{
			lock (_tableLock) {
				Table.Rows.Add(row);
			}
		}

		private void AddResultDictionary(Dictionary<string, object> row)
		{
			lock (_tableLock) {
				var tableRow = Table.NewRow();
				foreach (var keyValuePair in row) {
					tableRow[keyValuePair.Key] = keyValuePair.Value;
				}

				Table.Rows.Add(tableRow);
			}
		}
	

	//[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void Write(IModalDataContainer modData, VectoRunData runData)
		{
			//var row = GetResultRow(modData, runData); // Replace row with dictionary

			var row = GetResultDictionary(modData, runData);
			row[SumDataFields.SORT] = runData.JobNumber * 1000 + runData.RunNumber;
			row[SumDataFields.JOB] = $"{runData.JobNumber}-{runData.RunNumber}"; //ReplaceNotAllowedCharacters(current);
			row[SumDataFields.INPUTFILE] = ReplaceNotAllowedCharacters(runData.JobName);
			row[SumDataFields.CYCLE] = ReplaceNotAllowedCharacters(runData.Cycle.Name + Constants.FileExtensions.CycleFile);

			row[SumDataFields.STATUS] = modData.RunStatus;

			var vehicleLoading = 0.SI<Kilogram>();
			var cargoVolume = 0.SI<CubicMeter>();
			var gearCount = 0u;
			double? passengerCount = null;
			if (runData.Cycle.CycleType != CycleType.EngineOnly) {
				WriteFullPowertrain(runData, row);

				cargoVolume = runData.VehicleData.CargoVolume;
				vehicleLoading = runData.VehicleData.Loading;
				gearCount = (uint?)runData.GearboxData?.Gears.Count ?? 0u;
				passengerCount = runData.VehicleData.PassengerCount;
			}

			row[SumDataFields.VEHICLE_FUEL_TYPE] = modData.FuelData.Select(x => x.GetLabel()).Join();

			var totalTime = modData.Duration;
			row[SumDataFields.TIME] = (ConvertedSI)totalTime;

			var distance = modData.Distance;
			if (distance != null) {
				row[SumDataFields.DISTANCE] = distance.ConvertToKiloMeter();
			}

			var speed = modData.Speed();
			if (speed != null) {
				row[SumDataFields.SPEED] = speed.ConvertToKiloMeterPerHour();
			}

			row[SumDataFields.ALTITUDE_DELTA] = (ConvertedSI)modData.AltitudeDelta();

			if (modData.HasCombustionEngine) {
				WriteFuelConsumptionEntries(modData, row, vehicleLoading, cargoVolume, passengerCount, runData);
			} else {
				if (runData.ElectricMachinesData.Count > 0) {
					lock (Table) {
						if (!Table.Columns.Contains(SumDataFields.ElectricEnergyConsumptionPerKm)) {
							lock (_tableLock) {
								var col = Table.Columns.Add(SumDataFields.ElectricEnergyConsumptionPerKm, typeof(ConvertedSI));
								col.SetOrdinal(Table.Columns[SumDataFields.CO2_KM].Ordinal);
							}
						}
					}

					row[SumDataFields.ElectricEnergyConsumptionPerKm] =
						(-modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int) / modData.Distance).Cast<JoulePerMeter>().ConvertToKiloWattHourPerKiloMeter();
				}
			}

			if (runData.Mission?.MissionType == MissionType.VerificationTest) {
				var fuelsWhtc = runData.EngineData.Fuels.Select(
											fuel => modData.TimeIntegral<Kilogram>(modData.GetColumnName(fuel.FuelData, ModalResultField.FCWHTCc)) /
													modData.TimeIntegral<Kilogram>(modData.GetColumnName(fuel.FuelData, ModalResultField.FCMap)))
										.Select(dummy => (double)dummy).ToArray();
				row[SumDataFields.ENGINE_ACTUAL_CORRECTION_FACTOR] = fuelsWhtc.Join(" / ");
			}

			row[SumDataFields.P_WHEEL_POS] = modData.PowerWheelPositive().ConvertToKiloWatt();
			row[SumDataFields.P_WHEEL] = modData.PowerWheel().ConvertToKiloWatt();

			if (modData.HasCombustionEngine) {
				row[SumDataFields.P_FCMAP_POS] = modData.TotalPowerEnginePositiveAverage().ConvertToKiloWatt();
				row[SumDataFields.P_FCMAP] = modData.TotalPowerEngineAverage().ConvertToKiloWatt();
			}

			WriteAuxiliaries(modData, row, runData.BusAuxiliaries != null);

			WriteWorkEntries(modData, row, runData);

			WritePerformanceEntries(runData, modData, row);

			row[SumDataFields.COASTING_TIME_SHARE] = (ConvertedSI)modData.CoastingTimeShare();
			row[SumDataFields.BRAKING_TIME_SHARE] = (ConvertedSI)modData.BrakingTimeShare();

			if (runData.EngineData != null) {
				row[SumDataFields.ICE_FULL_LOAD_TIME_SHARE] = (ConvertedSI)modData.ICEMaxLoadTimeShare();
				row[SumDataFields.ICE_OFF_TIME_SHARE] = (ConvertedSI)modData.ICEOffTimeShare();
				row[SumDataFields.NUM_ICE_STARTS] = (ConvertedSI)modData.NumICEStarts().SI<Scalar>();
			}

			if (gearCount > 0) {
				WriteGearshiftStats(modData, row, gearCount);
			}

			//AddResultRow(row); //Add dictionary to datatable
			AddResultDictionary(row);
		}


		private void WriteFuelConsumptionEntries(
			IModalDataContainer modData, Dictionary<string, object> row, Kilogram vehicleLoading,
			CubicMeter cargoVolume, double? passengers, VectoRunData runData)
		{
			var multipleEngineModes = runData.EngineData.MultipleEngineFuelModes;
			var vtpCycle = runData.Cycle.CycleType == CycleType.VTP;


			row[SumDataFields.E_WHR_EL] = modData.CorrectedModalData.WorkWHREl.ConvertToKiloWattHour();
			row[SumDataFields.E_WHR_MECH] = modData.CorrectedModalData.WorkWHRMech.ConvertToKiloWattHour();

			row[SumDataFields.E_BusAux_PS_corr] = modData.CorrectedModalData.WorkBusAuxPSCorr.ConvertToKiloWattHour();
			row[SumDataFields.E_BusAux_ES_mech_corr] = modData.CorrectedModalData.WorkBusAuxESMech.ConvertToKiloWattHour();

			row[SumDataFields.E_BusAux_AuxHeater] = modData.CorrectedModalData.AuxHeaterDemand.Cast<WattSecond>().ConvertToKiloWattHour();

			foreach (var fuel in modData.FuelData) {
				var suffix = modData.FuelData.Count <= 1 && !multipleEngineModes ? "" : "_" + fuel.FuelType.GetLabel();

				row[FcCol(SumDataFields.FCMAP_H, suffix)] =
					modData.FuelConsumptionPerSecond(ModalResultField.FCMap, fuel)?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FCMAP_KM, suffix)] =
					modData.FuelConsumptionPerMeter(ModalResultField.FCMap, fuel)?.ConvertToGrammPerKiloMeter();


				row[FcCol(SumDataFields.FCNCVC_H, suffix)] =
					modData.FuelConsumptionPerSecond(ModalResultField.FCNCVc, fuel)?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FCNCVC_KM, suffix)] =
					modData.FuelConsumptionPerMeter(ModalResultField.FCNCVc, fuel)?.ConvertToGrammPerKiloMeter();

				row[FcCol(SumDataFields.FCWHTCC_H, suffix)] =
					modData.FuelConsumptionPerSecond(ModalResultField.FCWHTCc, fuel)?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FCWHTCC_KM, suffix)] =
					modData.FuelConsumptionPerMeter(ModalResultField.FCWHTCc, fuel)?.ConvertToGrammPerKiloMeter();

				//modData.FuelConsumptionPerSecond(ModalResultField.FCICEStopStart, fuel)
				//                                               ?.ConvertToGrammPerHour();
				//modData.FuelConsumptionPerMeter(ModalResultField.FCICEStopStart, fuel)
				//                                               ?.ConvertToGrammPerKiloMeter();

				var fuelConsumption = modData.CorrectedModalData.FuelConsumptionCorrection(fuel);

				row[FcCol(SumDataFields.K_ENGLINE, suffix)] = fuelConsumption.EngineLineCorrectionFactor.ConvertToGramPerKiloWattHour();
				row[FcCol(SumDataFields.K_VEHLINE, suffix)] = fuelConsumption.VehicleLine?.ConvertToGramPerKiloWattHour();

				var vehLine = modData.VehicleLineSlope(fuel);
				if (vehLine != null) {
					row[FcCol(SumDataFields.K_VEHLINE, suffix)] = vehLine.ConvertToGramPerKiloWattHour();
				}

				row[FcCol(SumDataFields.FCESS_H, suffix)] = fuelConsumption.FC_ESS_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FCESS_H_CORR, suffix)] = fuelConsumption.FC_ESS_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FC_BusAux_PS_CORR_H, suffix)] = fuelConsumption.FC_BusAux_PS_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FC_BusAux_ES_CORR_H, suffix)] = fuelConsumption.FC_BusAux_ES_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FCWHR_H_CORR, suffix)] = fuelConsumption.FC_WHR_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FC_HEV_SOC_CORR_H, suffix)] = fuelConsumption.FC_REESS_SOC_CORR_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FC_HEV_SOC_H, suffix)] = fuelConsumption.FC_REESS_SOC_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FC_AUXHTR_H, suffix)] = fuelConsumption.FC_AUXHTR_H?.ConvertToGrammPerHour();
				row[FcCol(SumDataFields.FC_AUXHTR_H_CORR, suffix)] = fuelConsumption.FC_AUXHTR_H_CORR?.ConvertToGrammPerHour();

				row[FcCol(SumDataFields.FCFINAL_H, suffix)] = fuelConsumption.FC_FINAL_H?.ConvertToGrammPerHour();

				row[FcCol(SumDataFields.FCWHR_KM_CORR, suffix)] = fuelConsumption.FC_WHR_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FC_BusAux_PS_CORR_KM, suffix)] = fuelConsumption.FC_BusAux_PS_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FC_BusAux_ES_CORR_KM, suffix)] = fuelConsumption.FC_BusAux_ES_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FC_HEV_SOC_CORR_KM, suffix)] = fuelConsumption.FC_REESS_SOC_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FC_HEV_SOC_KM, suffix)] = fuelConsumption.FC_REESS_SOC_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FC_AUXHTR_KM, suffix)] = fuelConsumption.FC_AUXHTR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FC_AUXHTR_KM_CORR, suffix)] = fuelConsumption.FC_AUXHTR_KM_CORR?.ConvertToGrammPerKiloMeter();

				row[FcCol(SumDataFields.FCESS_KM, suffix)] = fuelConsumption.FC_ESS_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FCESS_KM_CORR, suffix)] = fuelConsumption.FC_ESS_CORR_KM?.ConvertToGrammPerKiloMeter();
				row[FcCol(SumDataFields.FCFINAL_KM, suffix)] = fuelConsumption.FC_FINAL_KM?.ConvertToGrammPerKiloMeter();

				if (fuel.FuelDensity != null) {

					var fcVolumePerMeter = fuelConsumption.FuelVolumePerMeter;
					row[FcCol(SumDataFields.FCFINAL_LITERPER100KM, suffix)] = fcVolumePerMeter?.ConvertToLiterPer100Kilometer();

					if (vehicleLoading != null && !vehicleLoading.IsEqual(0) && fcVolumePerMeter != null) {
						row[FcCol(SumDataFields.FCFINAL_LITERPER100TKM, suffix)] =
							(fcVolumePerMeter / vehicleLoading).ConvertToLiterPer100TonKiloMeter();
					}
					if (cargoVolume > 0 && fcVolumePerMeter != null) {
						row[FcCol(SumDataFields.FCFINAL_LiterPer100M3KM, suffix)] =
							(fcVolumePerMeter / cargoVolume).ConvertToLiterPerCubicMeter100KiloMeter();
					}

					if (passengers != null && fcVolumePerMeter != null) {
						// subtract driver!
						row[FcCol(SumDataFields.FCFINAL_LiterPer100PassengerKM, suffix)] =
							(fcVolumePerMeter / passengers.Value).ConvertToLiterPer100Kilometer();
					}
				}

				if (vtpCycle) {
					row[FcCol(SumDataFields.SPECIFIC_FC, suffix)] = (modData.TotalFuelConsumption(ModalResultField.FCFinal, fuel) / modData.WorkWheelsPos())
						.ConvertToGramPerKiloWattHour();
				}
			}



			row[SumDataFields.CO2_KM] = modData.CorrectedModalData.KilogramCO2PerMeter.ConvertToGrammPerKiloMeter();
			if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
				row[SumDataFields.CO2_TKM] = (modData.CorrectedModalData.KilogramCO2PerMeter / vehicleLoading).ConvertToGrammPerTonKilometer();
			}
			if (cargoVolume > 0) {
				row[SumDataFields.CO2_M3KM] = (modData.CorrectedModalData.KilogramCO2PerMeter / cargoVolume).ConvertToGrammPerCubicMeterKiloMeter();
			}
			if (passengers != null) {
				row[SumDataFields.CO2_PKM] = (modData.CorrectedModalData.KilogramCO2PerMeter / passengers.Value).ConvertToGrammPerKiloMeter();
			}
		}

		private static string FcCol(string col, string suffix)
		{
			return string.Format(col, suffix);
		}

		private void WriteAuxiliaries(IModalDataContainer modData, Dictionary<string, object> row, bool writeBusAux)
		{
			foreach (var aux in modData.Auxiliaries) {
				string colName;
				if (aux.Key == Constants.Auxiliaries.IDs.PTOConsumer || aux.Key == Constants.Auxiliaries.IDs.PTOTransmission) {
					colName = string.Format(SumDataFields.E_FORMAT, aux.Key);
				} else {
					colName = string.Format(SumDataFields.E_AUX_FORMAT, aux.Key);
				}

				lock (Table)
					if (!Table.Columns.Contains(colName)) {
					lock (_tableLock) {
						var col = Table.Columns.Add(colName, typeof(ConvertedSI));

						// move the new column to correct position
						col.SetOrdinal(Table.Columns[SumDataFields.E_AUX].Ordinal);
					}
				}

				row[colName] = modData.AuxiliaryWork(aux.Value).ConvertToKiloWattHour();
			}

			if (writeBusAux) {
				row[SumDataFields.E_BusAux_HVAC_Mech] = modData.TimeIntegral<WattSecond>(ModalResultField.P_busAux_HVACmech_consumer).ConvertToKiloWattHour();
				row[SumDataFields.E_BusAux_HVAC_El] = modData.TimeIntegral<WattSecond>(ModalResultField.P_busAux_ES_HVAC).ConvertToKiloWattHour();
			}
		}

		private void WriteGearshiftStats(IModalDataContainer modData, Dictionary<string, object> row, uint gearCount)
		{
			row[SumDataFields.NUM_GEARSHIFTS] = gearCount == 1 ? 0.SI<Scalar>() : (ConvertedSI)modData.GearshiftCount();
			var timeSharePerGear = modData.TimeSharePerGear(gearCount);

			for (uint i = 0; i <= gearCount; i++) {
				var colName = string.Format(SumDataFields.TIME_SHARE_PER_GEAR_FORMAT, i);
				lock (Table)
					if (!Table.Columns.Contains(colName)) {
					lock (_tableLock) {
						Table.Columns.Add(colName, typeof(ConvertedSI));
					}
				}
				row[colName] = (ConvertedSI)timeSharePerGear[i];
			}
		}

		private void WritePerformanceEntries(VectoRunData runData, IModalDataContainer modData, Dictionary<string, object> row)
		{
			row[SumDataFields.ACC] = (ConvertedSI)modData.AccelerationAverage();
			row[SumDataFields.ACC_POS] = (ConvertedSI)modData.AccelerationsPositive();
			row[SumDataFields.ACC_NEG] = (ConvertedSI)modData.AccelerationsNegative();
			var accTimeShare = modData.AccelerationTimeShare();
			row[SumDataFields.ACC_TIMESHARE] = (ConvertedSI)accTimeShare;
			var decTimeShare = modData.DecelerationTimeShare();
			row[SumDataFields.DEC_TIMESHARE] = (ConvertedSI)decTimeShare;
			var cruiseTimeShare = modData.CruiseTimeShare();
			row[SumDataFields.CRUISE_TIMESHARE] = (ConvertedSI)cruiseTimeShare;
			var stopTimeShare = modData.StopTimeShare();
			row[SumDataFields.STOP_TIMESHARE] = (ConvertedSI)stopTimeShare;

			row[SumDataFields.MAX_SPEED] = (ConvertedSI)modData.MaxSpeed().AsKmph.SI<Scalar>();
			row[SumDataFields.MAX_ACCELERATION] = (ConvertedSI)modData.MaxAcceleration();
			row[SumDataFields.MAX_DECELERATION] = (ConvertedSI)modData.MaxDeceleration();
			if (runData.EngineData != null) {
				row[SumDataFields.AVG_ENGINE_SPEED] = (ConvertedSI)modData.AvgEngineSpeed().AsRPM.SI<Scalar>();
				row[SumDataFields.MAX_ENGINE_SPEED] = (ConvertedSI)modData.MaxEngineSpeed().AsRPM.SI<Scalar>();
			}

			row[SumDataFields.AVERAGE_POS_ACC] = (ConvertedSI)modData.AverageAccelerationBelowTargetSpeed();
			if (accTimeShare != null && decTimeShare != null && cruiseTimeShare != null) {
				var shareSum = accTimeShare + decTimeShare + cruiseTimeShare + stopTimeShare;
				if (!shareSum.IsEqual(100, 1e-2)) {
					Log.Warn(
						"Sumfile Error: driving behavior timeshares must sum up to 100%: acc: {0}%, dec: {1}%, cruise: {2}%, stop: {3}%, sum: {4}%",
						accTimeShare.ToOutputFormat(1, null, false), decTimeShare.ToOutputFormat(1, null, false),
						cruiseTimeShare.ToOutputFormat(1, null, false), stopTimeShare.ToOutputFormat(1, null, false),
						shareSum.ToOutputFormat(1, null, false));
				}
			}

			var eFC = 0.SI<Joule>();
			foreach (var fuel in modData.FuelData) {
				eFC += modData.TimeIntegral<Kilogram>(modData.GetColumnName(fuel, ModalResultField.FCFinal)) * fuel.LowerHeatingValueVecto;
			}
			var eIcePos = modData.TimeIntegral<WattSecond>(ModalResultField.P_ice_fcmap, x => x > 0);
			row[SumDataFields.AVERAGE_ENGINE_EFFICIENCY] = eFC.IsEqual(0, 1e-9) ? 0 : (eIcePos / eFC).Value();

			if (runData.SimulationType == SimulationType.EngineOnly) {
				return;
			}

			var gbxOutSignal = runData.Retarder != null && runData.Retarder.Type == RetarderType.TransmissionOutputRetarder
				? ModalResultField.P_retarder_in
				: (runData.AngledriveData == null ? ModalResultField.P_axle_in : ModalResultField.P_angle_in);
			var eGbxIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_gbx_in, x => x > 0);
			var eGbxOut = modData.TimeIntegral<WattSecond>(gbxOutSignal, x => x > 0);
			row[SumDataFields.AVERAGE_GEARBOX_EFFICIENCY] = eGbxIn.IsEqual(0, 1e-9) ? 0 : (eGbxOut / eGbxIn).Value();

			if (runData.GearboxData != null && runData.GearboxData.Type.AutomaticTransmission() && runData.GearboxData.Type != GearboxType.APTN && runData.GearboxData.Type != GearboxType.IHPC) {
				var eTcIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_TC_in, x => x > 0);
				var eTcOut = eGbxIn;
				row[SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITHOUT_LOCKUP] = eTcIn.IsEqual(0, 1e-9) ? 0 : (eTcOut / eTcIn).Value();

				var tcData = modData.GetValues(
					x => new {
						dt = x.Field<Second>(ModalResultField.simulationInterval.GetName()),
						locked = x.Field<int>(ModalResultField.TC_Locked.GetName()),
						P_TCin = x.Field<Watt>(ModalResultField.P_TC_in.GetName()),
						P_TCout = x.Field<Watt>(ModalResultField.P_TC_out.GetName())
					});
				eTcIn = 0.SI<WattSecond>();
				eTcOut = 0.SI<WattSecond>();
				foreach (var entry in tcData.Where(x => x.locked == 0)) {
					eTcIn += entry.dt * entry.P_TCin;
					eTcOut += entry.dt * entry.P_TCout;
				}

				row[SumDataFields.AVERAGE_TORQUE_CONVERTER_EFFICIENCY_WITH_LOCKUP] = eTcIn.IsEqual(0, 1e-9) ? 0 : (eTcOut / eTcIn).Value();
			}

			if (runData.AngledriveData != null) {
				var eAngleIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_angle_in, x => x > 0);
				var eAngleOut = modData.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);

				row[SumDataFields.AVERAGE_ANGLEDRIVE_EFFICIENCY] = (eAngleOut / eAngleIn).Value();
			}

			var eAxlIn = modData.TimeIntegral<WattSecond>(ModalResultField.P_axle_in, x => x > 0);
			var eAxlOut = modData.TimeIntegral<WattSecond>(ModalResultField.P_brake_in, x => x > 0);
			row[SumDataFields.AVERAGE_AXLEGEAR_EFFICIENCY] = eAxlIn.IsEqual(0, 1e-9) ? 0 : (eAxlOut / eAxlIn).Value();
		}

		private void WriteWorkEntries(IModalDataContainer modData, Dictionary<string, object> row, VectoRunData runData)
		{
			row[SumDataFields.E_FCMAP_POS] = modData.TotalEngineWorkPositive().ConvertToKiloWattHour();
			row[SumDataFields.E_FCMAP_NEG] = (-modData.TotalEngineWorkNegative()).ConvertToKiloWattHour();
			row[SumDataFields.E_POWERTRAIN_INERTIA] = modData.PowerAccelerations().ConvertToKiloWattHour();
			row[SumDataFields.E_AUX] = modData.WorkAuxiliaries().ConvertToKiloWattHour();
			row[SumDataFields.E_AUX_EL_HV] = modData.TimeIntegral<WattSecond>(ModalResultField.P_aux_el).ConvertToKiloWattHour();
			row[SumDataFields.E_CLUTCH_LOSS] = modData.WorkClutch().ConvertToKiloWattHour();
			row[SumDataFields.E_TC_LOSS] = modData.WorkTorqueConverter().ConvertToKiloWattHour();
			row[SumDataFields.E_SHIFT_LOSS] = modData.WorkGearshift().ConvertToKiloWattHour();
			row[SumDataFields.E_GBX_LOSS] = modData.WorkGearbox().ConvertToKiloWattHour();
			row[SumDataFields.E_RET_LOSS] = modData.WorkRetarder().ConvertToKiloWattHour();
			row[SumDataFields.E_AXL_LOSS] = modData.WorkAxlegear().ConvertToKiloWattHour();
			row[SumDataFields.E_ANGLE_LOSS] = modData.WorkAngledrive().ConvertToKiloWattHour();
			row[SumDataFields.E_BRAKE] = modData.WorkTotalMechanicalBrake().ConvertToKiloWattHour();
			row[SumDataFields.E_VEHICLE_INERTIA] = modData.WorkVehicleInertia().ConvertToKiloWattHour();
			row[SumDataFields.E_AIR] = modData.WorkAirResistance().ConvertToKiloWattHour();
			row[SumDataFields.E_ROLL] = modData.WorkRollingResistance().ConvertToKiloWattHour();
			row[SumDataFields.E_GRAD] = modData.WorkRoadGradientResistance().ConvertToKiloWattHour();
			row[SumDataFields.E_AUX_ESS_missing] = modData.CorrectedModalData.WorkESSMissing.ConvertToKiloWattHour();
			if (runData.Cycle.CycleType == CycleType.VTP) {
				row[SumDataFields.E_WHEEL] = modData.WorkWheels().ConvertToKiloWattHour();
			}

			if (runData.BusAuxiliaries != null) {
				row[SumDataFields.AirGenerated] = (ConvertedSI)modData.AirGenerated();
				row[SumDataFields.AirConsumed] = (ConvertedSI)modData.AirConsumed();
				row[SumDataFields.E_PS_CompressorOff] = modData.EnergyPneumaticCompressorPowerOff().ConvertToKiloWattHour();
				row[SumDataFields.E_PS_CompressorOn] = modData.EnergyPneumaticCompressorOn().ConvertToKiloWattHour();

				row[SumDataFields.E_BusAux_ES_generated] = modData.EnergyBusAuxESGenerated().ConvertToKiloWattHour();
				row[SumDataFields.E_BusAux_ES_consumed] = modData.EnergyBusAuxESConsumed().ConvertToKiloWattHour();
				row[SumDataFields.Delta_E_BusAux_Battery] = (runData.BusAuxiliaries.ElectricalUserInputsConfig.AlternatorType == AlternatorType.Smart
						? modData.DeltaSOCBusAuxBattery() * runData.BusAuxiliaries.ElectricalUserInputsConfig.ElectricStorageCapacity
						: 0.SI<WattSecond>())
					.ConvertToKiloWattHour();
			}

			row[SumDataFields.E_ICE_START] = modData.WorkEngineStart().ConvertToKiloWattHour();

			foreach (var em in runData.ElectricMachinesData) {
				var fields = em.Item1 == PowertrainPosition.IEPC
					? GetIEPCWorkEntries(modData, row, em)
					: GetElectricMachineWorkEntries(modData, row, em);
				fields.Reverse();
				foreach (var entry in fields) {
					var colName = string.Format(entry.Item1, em.Item1.GetName());
					lock (Table) {
						if (!Table.Columns.Contains(colName)) {
							lock (_tableLock) {
								var col = Table.Columns.Add(colName, typeof(ConvertedSI));
								col.SetOrdinal(Table.Columns[SumDataFields.E_GRAD].Ordinal + 1);
							}
						}
					}

					row[colName] = entry.Item2;
				}
			}

			if (runData.BatteryData != null || runData.SuperCapData != null) {
				foreach (var field in new[] { SumDataFields.REESS_StartSoC, SumDataFields.REESS_EndSoC }) {
					lock (Table) {
						if (Table.Columns.Contains(field)) {
							continue;
						}

						lock (_tableLock) {
							var col = Table.Columns.Add(field, typeof(double));
							col.SetOrdinal(Table.Columns[SumDataFields.P_WHEEL].Ordinal);
						}
					}
				}

				foreach (var field in new[] {
					SumDataFields.REESS_DeltaEnergy, SumDataFields.E_REESS_LOSS, SumDataFields.E_REESS_T_chg, SumDataFields.E_REESS_T_dischg,
					SumDataFields.E_REESS_int_chg, SumDataFields.E_REESS_int_dischg
				}) {
					lock (Table) {
						if (Table.Columns.Contains(field)) {
							continue;
						}

						lock (_tableLock) {
							var col = Table.Columns.Add(field, typeof(ConvertedSI));
							col.SetOrdinal(Table.Columns[SumDataFields.P_WHEEL].Ordinal);
						}
					}
				}
				row[SumDataFields.E_REESS_LOSS] = modData.REESSLoss().ConvertToKiloWattHour();
				row[SumDataFields.E_REESS_T_chg] = modData.WorkREESSChargeTerminal().ConvertToKiloWattHour();
				row[SumDataFields.E_REESS_T_dischg] = modData.WorkREESSDischargeTerminal().ConvertToKiloWattHour();
				row[SumDataFields.E_REESS_int_chg] = modData.WorkREESSChargeInternal().ConvertToKiloWattHour();
				row[SumDataFields.E_REESS_int_dischg] = modData.WorkREESSDischargeInternal().ConvertToKiloWattHour();
			}

			if (runData.BatteryData != null) {
				row[SumDataFields.REESS_StartSoC] = runData.BatteryData.InitialSoC * 100;
				row[SumDataFields.REESS_EndSoC] = modData.REESSEndSoC();
				row[SumDataFields.REESS_DeltaEnergy] = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName())
					.ConvertToKiloWattHour();

			}
			if (runData.SuperCapData != null) {
				row[SumDataFields.REESS_StartSoC] = runData.SuperCapData.InitialSoC * 100;
				row[SumDataFields.REESS_EndSoC] = modData.REESSEndSoC();
				row[SumDataFields.REESS_DeltaEnergy] = modData.TimeIntegral<WattSecond>(ModalResultField.P_reess_int.GetName())
					.ConvertToKiloWattHour();
			}
		}

		private List<Tuple<string, ConvertedSI>> GetIEPCWorkEntries(IModalDataContainer modData, Dictionary<string, object> row, Tuple<PowertrainPosition, ElectricMotorData> em)
		{
			var emColumns = new List<Tuple<string, ConvertedSI>>() {
				Tuple.Create(SumDataFields.IEPC_AVG_SPEED_FORMAT,
					modData.ElectricMotorAverageSpeed(em.Item1).ConvertToRoundsPerMinute()),

				Tuple.Create(SumDataFields.E_IEPC_DRIVE_FORMAT, modData.TotalElectricMotorWorkDrive(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(SumDataFields.E_IEPC_GENERATE_FORMAT,
					modData.TotalElectricMotorWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(SumDataFields.ETA_IEPC_DRIVE_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyDrive(em.Item1), "")),
				Tuple.Create(SumDataFields.ETA_IEPC_GEN_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyGenerate(em.Item1), "")),

				Tuple.Create(SumDataFields.E_IEPC_OFF_Loss_Format, modData.ElectricMotorOffLosses(em.Item1).ConvertToKiloWattHour()),
				
				Tuple.Create(SumDataFields.E_IEPC_LOSS_FORMAT, modData.ElectricMotorLosses(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(SumDataFields.E_IEPC_OFF_TIME_SHARE, (ConvertedSI)modData.ElectricMotorOffTimeShare(em.Item1))
			};
			return emColumns;
		}

		private List<Tuple<string, ConvertedSI>> GetElectricMachineWorkEntries(IModalDataContainer modData, Dictionary<string, object> row, Tuple<PowertrainPosition, ElectricMotorData> em)
		{
			var emColumns = new List<Tuple<string, ConvertedSI>>() {
				Tuple.Create(SumDataFields.EM_AVG_SPEED_FORMAT,
					modData.ElectricMotorAverageSpeed(em.Item1).ConvertToRoundsPerMinute()),

				Tuple.Create(SumDataFields.E_EM_Mot_DRIVE_FORMAT,
					modData.TotalElectricMotorMotWorkDrive(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(SumDataFields.E_EM_Mot_GENERATE_FORMAT,
					modData.TotalElectricMotorMotWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(SumDataFields.ETA_EM_Mot_DRIVE_FORMAT,
					new ConvertedSI(modData.ElectricMotorMotEfficiencyDrive(em.Item1), "")),
				Tuple.Create(SumDataFields.ETA_EM_Mot_GEN_FORMAT,
					new ConvertedSI(modData.ElectricMotorMotEfficiencyGenerate(em.Item1), "")),


				Tuple.Create(SumDataFields.E_EM_DRIVE_FORMAT, modData.TotalElectricMotorWorkDrive(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(SumDataFields.E_EM_GENERATE_FORMAT,
					modData.TotalElectricMotorWorkRecuperate(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(SumDataFields.ETA_EM_DRIVE_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyDrive(em.Item1), "")),
				Tuple.Create(SumDataFields.ETA_EM_GEN_FORMAT, new ConvertedSI(modData.ElectricMotorEfficiencyGenerate(em.Item1), "")),

				Tuple.Create(SumDataFields.E_EM_OFF_Loss_Format, modData.ElectricMotorOffLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(SumDataFields.E_EM_LOSS_TRANSM_FORMAT,
					modData.ElectricMotorTransmissionLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(SumDataFields.E_EM_Mot_LOSS_FORMAT, modData.ElectricMotorMotLosses(em.Item1).ConvertToKiloWattHour()),
				Tuple.Create(SumDataFields.E_EM_LOSS_FORMAT, modData.ElectricMotorLosses(em.Item1).ConvertToKiloWattHour()),

				Tuple.Create(SumDataFields.E_EM_OFF_TIME_SHARE, (ConvertedSI)modData.ElectricMotorOffTimeShare(em.Item1))
			};
			return emColumns;
			
		}

		private void WriteFullPowertrain(VectoRunData runData, Dictionary<string, object> row)
		{
			WriteVehicleData(runData, row);

			if (runData.BusAuxiliaries?.InputData != null) {
				// only write in declaration mode - if input data is set
				// subtract driver!
				row[SumDataFields.PassengerCount] = runData.VehicleData.PassengerCount;
			}

			row[SumDataFields.TCU_MODEL] = runData.ShiftStrategy;
			row[SumDataFields.PTO_TECHNOLOGY] = runData.PTO?.TransmissionType ?? "";

			WriteEngineData(runData.EngineData, row);

			WriteGearboxData(runData.GearboxData, row);

			WriteRetarderData(runData.Retarder, row);

			WriteAngledriveData(runData.AngledriveData, row);

			WriteAxlegearData(runData.AxleGearData, row);

			WriteAuxTechnologies(runData, row);

			WriteAxleWheelsData(runData.VehicleData.AxleData, row);

			WriteAirdragData(runData.AirdragData, row);

		}

		private static void WriteVehicleData(VectoRunData runData, Dictionary<string, object> row)
		{
			var data = runData.VehicleData;
			//if (runData.VehicleData.b)
			var gbxType = runData.GearboxData?.Type ?? GearboxType.NoGearbox;

			row[SumDataFields.VEHICLE_MANUFACTURER] = data.Manufacturer;
			row[SumDataFields.VIN_NUMBER] = data.VIN;
			row[SumDataFields.VEHICLE_MODEL] = data.ModelName;

			row[SumDataFields.HDV_CO2_VEHICLE_CLASS] = runData.Mission?.BusParameter?.BusGroup.GetClassNumber() ?? data.VehicleClass.GetClassNumber();
			row[SumDataFields.CURB_MASS] = (ConvertedSI)data.CurbMass;

			// - (data.BodyAndTrailerWeight ?? 0.SI<Kilogram>());
			row[SumDataFields.LOADING] = (ConvertedSI)data.Loading;
			row[SumDataFields.CARGO_VOLUME] = (ConvertedSI)data.CargoVolume;

			row[SumDataFields.TOTAL_VEHICLE_MASS] = (ConvertedSI)data.TotalVehicleMass;

			row[SumDataFields.SLEEPER_CAB] = data.SleeperCab.HasValue ? (data.SleeperCab.Value ? "yes" : "no") : "-";

			row[SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER] =
				data.RollResistanceCoefficientWithoutTrailer;
			row[SumDataFields.ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER] =
				data.TotalRollResistanceCoefficient;

			row[SumDataFields.R_DYN] = (ConvertedSI)data.DynamicTyreRadius;

			row[SumDataFields.ADAS_TECHNOLOGY_COMBINATION] = data.ADAS != null ? DeclarationData.ADASCombinations.Lookup(data.ADAS, gbxType).ID : "";

			var cap = "";
			if (runData.BatteryData?.Capacity != null) {
				cap = $"{runData.BatteryData.Capacity.AsAmpHour} Ah";
			}

			if (runData.SuperCapData?.Capacity != null) {
				cap = $"{runData.SuperCapData.Capacity} F";
			}

			row[SumDataFields.REESS_CAPACITY] = cap;
		}

		private static void WriteAirdragData(AirdragData data, Dictionary<string, object> row)
		{
			row[SumDataFields.AIRDRAG_MODEL] = data.ModelName;
			row[SumDataFields.AIRDRAG_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
			row[SumDataFields.AIRDRAG_CERTIFICATION_NUMBER] =
				data.CertificationMethod == CertificationMethod.StandardValues ? "" : data.CertificationNumber;
			row[SumDataFields.CD_x_A_DECLARED] = (ConvertedSI)data.DeclaredAirdragArea;
			row[SumDataFields.CD_x_A] = (ConvertedSI)data.CrossWindCorrectionCurve.AirDragArea;
		}

		private static void WriteEngineData(CombustionEngineData data, Dictionary<string, object> row)
		{
			if (data == null) {
				return;
			}
			row[SumDataFields.ENGINE_MANUFACTURER] = data.Manufacturer;
			row[SumDataFields.ENGINE_MODEL] = data.ModelName;
			row[SumDataFields.ENGINE_CERTIFICATION_NUMBER] = data.CertificationNumber;
			row[SumDataFields.ENGINE_FUEL_TYPE] = data.Fuels.Select(x => x.FuelData.GetLabel()).Join(" / ");
			row[SumDataFields.ENGINE_RATED_POWER] = data.RatedPowerDeclared != null && data.RatedPowerDeclared > 0
				? data.RatedPowerDeclared.ConvertToKiloWatt()
				: data.FullLoadCurves[0].MaxPower.ConvertToKiloWatt();
			row[SumDataFields.ENGINE_IDLING_SPEED] = (ConvertedSI)data.IdleSpeed.AsRPM.SI<Scalar>();
			row[SumDataFields.ENGINE_RATED_SPEED] = data.RatedSpeedDeclared != null && data.RatedSpeedDeclared > 0
				? (ConvertedSI)data.RatedSpeedDeclared.AsRPM.SI<Scalar>()
				: (ConvertedSI)data.FullLoadCurves[0].RatedSpeed.AsRPM.SI<Scalar>();
			row[SumDataFields.ENGINE_DISPLACEMENT] = data.Displacement.ConvertToCubicCentiMeter();

			row[SumDataFields.ENGINE_WHTC_URBAN] = data.Fuels.Select(x => x.WHTCUrban).Join(" / ");
			row[SumDataFields.ENGINE_WHTC_RURAL] = data.Fuels.Select(x => x.WHTCRural).Join(" / ");
			row[SumDataFields.ENGINE_WHTC_MOTORWAY] = data.Fuels.Select(x => x.WHTCMotorway).Join(" / ");
			row[SumDataFields.ENGINE_BF_COLD_HOT] = data.Fuels.Select(x => x.ColdHotCorrectionFactor).Join(" / ");
			row[SumDataFields.ENGINE_CF_REG_PER] = data.Fuels.Select(x => x.CorrectionFactorRegPer).Join(" / ");
			row[SumDataFields.ENGINE_ACTUAL_CORRECTION_FACTOR] = data.Fuels.Select(x => x.FuelConsumptionCorrectionFactor).Join(" / ");
		}

		private static void WriteAxleWheelsData(List<Axle> data, Dictionary<string, object> row)
		{
			var fields = new[] {
				Tuple.Create(SumDataFields.DECLARED_RRC_AXLE1, SumDataFields.DECLARED_FZISO_AXLE1),
				Tuple.Create(SumDataFields.DECLARED_RRC_AXLE2, SumDataFields.DECLARED_FZISO_AXLE2),
				Tuple.Create(SumDataFields.DECLARED_RRC_AXLE3, SumDataFields.DECLARED_FZISO_AXLE3),
				Tuple.Create(SumDataFields.DECLARED_RRC_AXLE4, SumDataFields.DECLARED_FZISO_AXLE4),
			};
			for (var i = 0; i < Math.Min(fields.Length, data.Count); i++) {
				if (data[i].AxleType == AxleType.Trailer) {
					continue;
				}

				row[fields[i].Item1] = data[i].RollResistanceCoefficient;
				row[fields[i].Item2] = (ConvertedSI)data[i].TyreTestLoad;
			}

			row[SumDataFields.NUM_AXLES_DRIVEN] = data.Count(x => x.AxleType == AxleType.VehicleDriven);
			row[SumDataFields.NUM_AXLES_NON_DRIVEN] = data.Count(x => x.AxleType == AxleType.VehicleNonDriven);
			row[SumDataFields.NUM_AXLES_TRAILER] = data.Count(x => x.AxleType == AxleType.Trailer);
		}

		private static void WriteAxlegearData(AxleGearData data, Dictionary<string, object> row)
		{
			if (data == null) {
				return;
			}
			row[SumDataFields.AXLE_MANUFACTURER] = data.Manufacturer;
			row[SumDataFields.AXLE_MODEL] = data.ModelName;
			row[SumDataFields.AXLE_RATIO] = (ConvertedSI)data.AxleGear.Ratio.SI<Scalar>();
			row[SumDataFields.AXLEGEAR_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
			row[SumDataFields.AXLEGEAR_CERTIFICATION_NUMBER] = data.CertificationMethod == CertificationMethod.StandardValues
				? ""
				: data.CertificationNumber;
		}

		private void WriteAuxTechnologies(VectoRunData runData, Dictionary<string, object> row)
		{
			var auxData = runData.Aux;
			var busAux = runData.BusAuxiliaries;
			foreach (var aux in auxData) {
				if (aux.ID == Constants.Auxiliaries.IDs.PTOConsumer || aux.ID == Constants.Auxiliaries.IDs.PTOTransmission) {
					continue;
				}

				var colName = string.Format(SumDataFields.AUX_TECH_FORMAT, aux.ID);

				lock (Table)
					if (!Table.Columns.Contains(colName)) {
					lock (_tableLock) {
						var col = Table.Columns.Add(colName, typeof(string));

						// move the new column to correct position
						col.SetOrdinal(Table.Columns[SumDataFields.CARGO_VOLUME].Ordinal);
					}
				}

				row[colName] = aux.Technology == null ? "" : aux.Technology.Join("; ");
			}

			if (busAux == null) {
				return;
			}

			row[string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition)] =
				busAux.SSMInputs is ISSMDeclarationInputs inputs ? inputs.HVACTechnology : "engineering mode";
			row[string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.ElectricSystem)] =
				busAux.ElectricalUserInputsConfig.AlternatorType.GetLabel();
			row[string.Format(SumDataFields.AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.PneumaticSystem)] = runData.JobType == VectoSimulationJobType.BatteryElectricVehicle ? "-" :
				busAux.PneumaticUserInputsConfig.CompressorMap.Technology;
		}

		private static void WriteAngledriveData(AngledriveData data, Dictionary<string, object> row)
		{
			if (data != null) {
				row[SumDataFields.ANGLEDRIVE_MANUFACTURER] = data.Manufacturer;
				row[SumDataFields.ANGLEDRIVE_MODEL] = data.ModelName;
				row[SumDataFields.ANGLEDRIVE_RATIO] = data.Angledrive.Ratio;
				row[SumDataFields.ANGLEDRIVE_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
				row[SumDataFields.ANGLEDRIVE_CERTIFICATION_NUMBER] =
					data.CertificationMethod == CertificationMethod.StandardValues
						? ""
						: data.CertificationNumber;
			} else {
				row[SumDataFields.ANGLEDRIVE_MANUFACTURER] = Constants.NOT_AVAILABLE;
				row[SumDataFields.ANGLEDRIVE_MODEL] = Constants.NOT_AVAILABLE;
				row[SumDataFields.ANGLEDRIVE_RATIO] = Constants.NOT_AVAILABLE;
				row[SumDataFields.ANGLEDRIVE_CERTIFICATION_METHOD] = "";
				row[SumDataFields.ANGLEDRIVE_CERTIFICATION_NUMBER] = "";
			}
		}

		private static void WriteRetarderData(RetarderData data, Dictionary<string, object> row)
		{
			row[SumDataFields.RETARDER_TYPE] = (data?.Type ?? RetarderType.None).GetLabel();
			if (data != null && data.Type.IsDedicatedComponent()) {
				row[SumDataFields.RETARDER_MANUFACTURER] = data.Manufacturer;
				row[SumDataFields.RETARDER_MODEL] = data.ModelName;
				row[SumDataFields.RETARDER_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
				row[SumDataFields.RETARDER_CERTIFICATION_NUMBER] = data.CertificationMethod == CertificationMethod.StandardValues
					? ""
					: data.CertificationNumber;
			} else {
				row[SumDataFields.RETARDER_MANUFACTURER] = Constants.NOT_AVAILABLE;
				row[SumDataFields.RETARDER_MODEL] = Constants.NOT_AVAILABLE;
				row[SumDataFields.RETARDER_CERTIFICATION_METHOD] = "";
				row[SumDataFields.RETARDER_CERTIFICATION_NUMBER] = "";
			}
		}

		private static void WriteGearboxData(GearboxData data, Dictionary<string, object> row)
		{
			if (data == null) {
				return;
			}
			row[SumDataFields.GEARBOX_MANUFACTURER] = data.Manufacturer;
			row[SumDataFields.GEARBOX_MODEL] = data.ModelName;
			row[SumDataFields.GEARBOX_TYPE] = data.Type;
			row[SumDataFields.GEARBOX_CERTIFICATION_NUMBER] = data.CertificationMethod == CertificationMethod.StandardValues
				? ""
				: data.CertificationNumber;
			row[SumDataFields.GEARBOX_CERTIFICATION_METHOD] = data.CertificationMethod.GetName();
			if (data.Type.AutomaticTransmission()) {
				row[SumDataFields.GEAR_RATIO_FIRST_GEAR] = data.Gears.Count > 0
					? (double.IsNaN(data.Gears.First().Value.Ratio)
						? (ConvertedSI)data.Gears.First().Value.TorqueConverterRatio.SI<Scalar>()
						: (ConvertedSI)data.Gears.First().Value.Ratio.SI<Scalar>())
					: 0.SI<Scalar>();
				row[SumDataFields.GEAR_RATIO_LAST_GEAR] = data.Gears.Count > 0
					? (ConvertedSI)data.Gears.Last().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				if (data.Type != GearboxType.APTN && data.Type != GearboxType.IHPC) {
					row[SumDataFields.TORQUECONVERTER_MANUFACTURER] = data.TorqueConverterData.Manufacturer;
					row[SumDataFields.TORQUECONVERTER_MODEL] = data.TorqueConverterData.ModelName;
					row[SumDataFields.TORQUE_CONVERTER_CERTIFICATION_NUMBER] =
						data.TorqueConverterData.CertificationMethod == CertificationMethod.StandardValues
							? ""
							: data.TorqueConverterData.CertificationNumber;
					row[SumDataFields.TORQUE_CONVERTER_CERTIFICATION_METHOD] =
						data.TorqueConverterData.CertificationMethod.GetName();
				}
			} else {
				row[SumDataFields.GEAR_RATIO_FIRST_GEAR] = data.Gears.Count > 0
					? (ConvertedSI)data.Gears.First().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[SumDataFields.GEAR_RATIO_LAST_GEAR] = data.Gears.Count > 0
					? (ConvertedSI)data.Gears.Last().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[SumDataFields.TORQUECONVERTER_MANUFACTURER] = Constants.NOT_AVAILABLE;
				row[SumDataFields.TORQUECONVERTER_MODEL] = Constants.NOT_AVAILABLE;
				row[SumDataFields.TORQUE_CONVERTER_CERTIFICATION_METHOD] = "";
				row[SumDataFields.TORQUE_CONVERTER_CERTIFICATION_NUMBER] = "";
			}
		}

		private static string ReplaceNotAllowedCharacters(string text)
		{
			return text.Replace('#', '_').Replace(',', '_').Replace('\n', '_').Replace('\r', '_');
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected void Dispose(bool disposing)
		{
			if (disposing) {
				lock (Table)
					Table.Dispose();
			}
		}
	}
}
