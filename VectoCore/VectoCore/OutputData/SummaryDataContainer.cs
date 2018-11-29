/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

// ReSharper disable MemberCanBePrivate.Global  -- used by API!

namespace TUGraz.VectoCore.OutputData
{
	public delegate void WriteSumData(IModalDataContainer data);

	/// <summary>
	/// Class for the sum file in vecto.
	/// </summary>
	public class SummaryDataContainer : LoggingObject, IDisposable
	{
		// ReSharper disable InconsistentNaming
		public const string INTERNAL_PREFIX = "INTERNAL";
		public const string SORT = INTERNAL_PREFIX + " Sorting";
		public const string JOB = "Job [-]";
		public const string INPUTFILE = "Input File [-]";
		public const string CYCLE = "Cycle [-]";
		public const string STATUS = "Status";
		public const string CURB_MASS = "Corrected Actual Curb Mass [kg]";
		public const string LOADING = "Loading [kg]";

		public const string VEHICLE_MANUFACTURER = "Vehicle manufacturer [-]";
		public const string VIN_NUMBER = "VIN number";
		public const string VEHICLE_MODEL = "Vehicle model [-]";

		public const string ENGINE_MANUFACTURER = "Engine manufacturer [-]";
		public const string ENGINE_MODEL = "Engine model [-]";
		public const string ENGINE_FUEL_TYPE = "Engine fuel type [-]";
		public const string ENGINE_WHTC_URBAN = "Engine WHTCUrban";
		public const string ENGINE_WHTC_RURAL = "Engine WHTCRural";
		public const string ENGINE_WHTC_MOTORWAY = "Engine WHTCMotorway";
		public const string ENGINE_BF_COLD_HOT = "Engine BFColdHot";
		public const string ENGINE_CF_REG_PER = "Engine CFRegPer";
		public const string ENGINE_ACTUAL_CORRECTION_FACTOR = "Engine actual CF";
		public const string ENGINE_RATED_POWER = "Engine rated power [kW]";
		public const string ENGINE_IDLING_SPEED = "Engine idling speed [rpm]";
		public const string ENGINE_RATED_SPEED = "Engine rated speed [rpm]";
		public const string ENGINE_DISPLACEMENT = "Engine displacement [ccm]";

		public const string ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER = "total RRC [-]";
		public const string ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER = "weighted RRC w/o trailer [-]";

		public const string GEARBOX_MANUFACTURER = "Gearbox manufacturer [-]";
		public const string GEARBOX_MODEL = "Gearbox model [-]";
		public const string GEARBOX_TYPE = "Gearbox type [-]";
		public const string GEAR_RATIO_FIRST_GEAR = "Gear ratio first gear [-]";
		public const string GEAR_RATIO_LAST_GEAR = "Gear ratio last gear [-]";

		public const string TORQUECONVERTER_MANUFACTURER = "Torque converter manufacturer [-]";
		public const string TORQUECONVERTER_MODEL = "Torque converter model [-]";

		public const string RETARDER_MANUFACTURER = "Retarder manufacturer [-]";
		public const string RETARDER_MODEL = "Retarder model [-]";
		public const string RETARDER_TYPE = "Retarder type [-]";

		public const string ANGLEDRIVE_MANUFACTURER = "Angledrive manufacturer [-]";
		public const string ANGLEDRIVE_MODEL = "Angledrive model [-]";
		public const string ANGLEDRIVE_RATIO = "Angledrive ratio [-]";

		public const string AXLE_MANUFACTURER = "Axle manufacturer [-]";
		public const string AXLE_MODEL = "Axle model [-]";
		public const string AXLE_RATIO = "Axle gear ratio [-]";

		public const string AUX_TECH_FORMAT = "Auxiliary technology {0} [-]";

		public const string HDV_CO2_VEHICLE_CLASS = "HDV CO2 vehicle class [-]";
		public const string TOTAL_VEHICLE_MASS = "Total vehicle mass [kg]";
		public const string CD_x_A_DECLARED = "Declared CdxA [m²]";
		public const string CD_x_A = "CdxA [m²]";
		//public const string ROLLING_RESISTANCE_COEFFICIENT = "weighed RRC [-]";
		public const string R_DYN = "r_dyn [m]";

		public const string CARGO_VOLUME = "Cargo Volume [m³]";
		public const string TIME = "time [s]";
		public const string DISTANCE = "distance [km]";
		public const string SPEED = "speed [km/h]";
		public const string ALTITUDE_DELTA = "altitudeDelta [m]";

		public const string FCMAP_H = "FC-Map [g/h]";
		public const string FCMAP_KM = "FC-Map [g/km]";
		public const string FCNCVC_H = "FC-NCVc [g/h]";
		public const string FCNCVC_KM = "FC-NCVc [g/km]";
		public const string FCWHTCC_H = "FC-WHTCc [g/h]";
		public const string FCWHTCC_KM = "FC-WHTCc [g/km]";
		public const string FCAAUX_H = "FC-AAUX [g/h]";
		public const string FCAAUX_KM = "FC-AAUX [g/km]";
		public const string FCADAS_H = "FC-ADAS [g/h]";
		public const string FCADAS_KM = "FC-ADAS [g/km]";

		public const string FCFINAL_H = "FC-Final [g/h]";
		public const string FCFINAL_KM = "FC-Final [g/km]";
		public const string FCFINAL_LITERPER100KM = "FC-Final [l/100km]";
		public const string FCFINAL_LITERPER100TKM = "FC-Final [l/100tkm]";
		public const string FCFINAL_LiterPer100M3KM = "FC-Final [l/100m³km]";

		public const string CO2_KM = "CO2 [g/km]";
		public const string CO2_TKM = "CO2 [g/tkm]";
		public const string CO2_M3KM = "CO2 [g/m³km]";

		public const string P_WHEEL_POS = "P_wheel_in_pos [kW]";
		public const string P_FCMAP_POS = "P_fcmap_pos [kW]";

		public const string E_FORMAT = "E_{0} [kWh]";
		public const string E_AUX_FORMAT = "E_aux_{0} [kWh]";
		public const string E_AUX = "E_aux_sum [kWh]";

		public const string E_AIR = "E_air [kWh]";
		public const string E_ROLL = "E_roll [kWh]";
		public const string E_GRAD = "E_grad [kWh]";
		public const string E_VEHICLE_INERTIA = "E_vehi_inertia [kWh]";
		public const string E_POWERTRAIN_INERTIA = "E_powertrain_inertia [kWh]";
		public const string E_WHEEL = "E_wheel [kWh]";
		public const string E_BRAKE = "E_brake [kWh]";
		public const string E_GBX_LOSS = "E_gbx_loss [kWh]";
		public const string E_SHIFT_LOSS = "E_shift_loss [kWh]";
		public const string E_AXL_LOSS = "E_axl_loss [kWh]";
		public const string E_RET_LOSS = "E_ret_loss [kWh]";
		public const string E_TC_LOSS = "E_tc_loss [kWh]";
		public const string E_ANGLE_LOSS = "E_angle_loss [kWh]";
		public const string E_CLUTCH_LOSS = "E_clutch_loss [kWh]";
		public const string E_FCMAP_POS = "E_fcmap_pos [kWh]";
		public const string E_FCMAP_NEG = "E_fcmap_neg [kWh]";

		public const string SPECIFIC_FC = "Specific FC [g/kWh] wheel pos.";

		public const string ACC = "a [m/s^2]";
		public const string ACC_POS = "a_pos [m/s^2]";
		public const string ACC_NEG = "a_neg [m/s^2]";

		public const string ACC_TIMESHARE = "AccelerationTimeShare [%]";
		public const string DEC_TIMESHARE = "DecelerationTimeShare [%]";
		public const string CRUISE_TIMESHARE = "CruiseTimeShare [%]";
		public const string STOP_TIMESHARE = "StopTimeShare [%]";

		public const string MAX_SPEED = "max. speed [km/h]";
		public const string MAX_ACCELERATION = "max. acc [m/s²]";
		public const string MAX_DECELERATION = "max. dec [m/s²]";
		public const string AVG_ENGINE_SPEED = "n_eng_avg [rpm]";
		public const string MAX_ENGINE_SPEED = "n_eng_max [rpm]";
		public const string NUM_GEARSHIFTS = "gear shifts [-]";
		public const string ENGINE_FULL_LOAD_TIME_SHARE = "Engine max. Load time share [%]";
		public const string COASTING_TIME_SHARE = "CoastingTimeShare [%]";
		public const string BRAKING_TIME_SHARE = "BrakingTImeShare [%]";

		public const string TIME_SHARE_PER_GEAR_FORMAT = "Gear {0} TimeShare [%]";

		public const string NUM_AXLES_DRIVEN = "Number axles vehicle driven [-]";
		public const string NUM_AXLES_NON_DRIVEN = "Number axles vehicle non-driven [-]";
		public const string NUM_AXLES_TRAILER = "Number axles trailer [-]";

		// ReSharper restore InconsistentNaming

		internal readonly DataTable Table;
		private readonly ISummaryWriter _sumWriter;

		protected SummaryDataContainer() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryDataContainer"/> class.
		/// </summary>
		/// <param name="writer"></param>
		
		public SummaryDataContainer(ISummaryWriter writer)
		{
			_sumWriter = writer;
			
			Table = new DataTable();

			Table.Columns.AddRange(new[] {
				Tuple.Create(SORT, typeof(int)),
				Tuple.Create(JOB, typeof(string)),
				Tuple.Create(INPUTFILE, typeof(string)),
				Tuple.Create(CYCLE, typeof(string)),
				Tuple.Create(STATUS, typeof(string)),
				Tuple.Create(VEHICLE_MANUFACTURER, typeof(string)),
				Tuple.Create(VIN_NUMBER, typeof(string)),
				Tuple.Create(VEHICLE_MODEL, typeof(string)),
				Tuple.Create(HDV_CO2_VEHICLE_CLASS, typeof(string)),
				Tuple.Create(CURB_MASS, typeof(ConvertedSI)),
				Tuple.Create(LOADING, typeof(ConvertedSI)),
				Tuple.Create(TOTAL_VEHICLE_MASS, typeof(ConvertedSI)),
				Tuple.Create(ENGINE_MANUFACTURER, typeof(string)),
				Tuple.Create(ENGINE_MODEL, typeof(string)),
				Tuple.Create(ENGINE_FUEL_TYPE, typeof(string)),
				Tuple.Create(ENGINE_RATED_POWER, typeof(ConvertedSI)),
				Tuple.Create(ENGINE_IDLING_SPEED, typeof(ConvertedSI)),
				Tuple.Create(ENGINE_RATED_SPEED, typeof(ConvertedSI)),
				Tuple.Create(ENGINE_DISPLACEMENT, typeof(ConvertedSI)),
				Tuple.Create(ENGINE_WHTC_URBAN, typeof(double)),
				Tuple.Create(ENGINE_WHTC_RURAL, typeof(double)),
				Tuple.Create(ENGINE_WHTC_MOTORWAY, typeof(double)),
				Tuple.Create(ENGINE_BF_COLD_HOT, typeof(double)),
				Tuple.Create(ENGINE_CF_REG_PER, typeof(double)),
				Tuple.Create(ENGINE_ACTUAL_CORRECTION_FACTOR, typeof(double)),
				Tuple.Create(CD_x_A_DECLARED, typeof(ConvertedSI)),
				Tuple.Create(CD_x_A, typeof(ConvertedSI)),
				Tuple.Create(ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER, typeof(double)),
				Tuple.Create(ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER, typeof(double)),
				Tuple.Create(R_DYN, typeof(ConvertedSI)),
				Tuple.Create(NUM_AXLES_DRIVEN, typeof(int)),
				Tuple.Create(NUM_AXLES_NON_DRIVEN, typeof(int)),
				Tuple.Create(NUM_AXLES_TRAILER, typeof(int)),
				Tuple.Create(GEARBOX_MANUFACTURER, typeof(string)),
				Tuple.Create(GEARBOX_MODEL, typeof(string)),
				Tuple.Create(GEARBOX_TYPE, typeof(string)),
				Tuple.Create(GEAR_RATIO_FIRST_GEAR, typeof(ConvertedSI)),
				Tuple.Create(GEAR_RATIO_LAST_GEAR, typeof(ConvertedSI)),
				Tuple.Create(TORQUECONVERTER_MANUFACTURER, typeof(string)),
				Tuple.Create(TORQUECONVERTER_MODEL, typeof(string)),
				Tuple.Create(RETARDER_MANUFACTURER, typeof(string)),
				Tuple.Create(RETARDER_MODEL, typeof(string)),
				Tuple.Create(RETARDER_TYPE, typeof(string)),
				Tuple.Create(ANGLEDRIVE_MANUFACTURER, typeof(string)),
				Tuple.Create(ANGLEDRIVE_MODEL, typeof(string)),
				Tuple.Create(ANGLEDRIVE_RATIO, typeof(string)),
				Tuple.Create(AXLE_MANUFACTURER, typeof(string)),
				Tuple.Create(AXLE_MODEL, typeof(string)),
				Tuple.Create(AXLE_RATIO, typeof(ConvertedSI)),
				Tuple.Create(string.Format(AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.SteeringPump), typeof(string)),
				Tuple.Create(string.Format(AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.Fan), typeof(string)),
				Tuple.Create(string.Format(AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.HeatingVentilationAirCondition),
					typeof(string)),
				Tuple.Create(string.Format(AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.PneumaticSystem), typeof(string)),
				Tuple.Create(string.Format(AUX_TECH_FORMAT, Constants.Auxiliaries.IDs.ElectricSystem), typeof(string)),
			}.Select(x => new DataColumn(x.Item1, x.Item2)).ToArray());

			Table.Columns.AddRange(new[] {
				CARGO_VOLUME,
				TIME, DISTANCE,
				SPEED, ALTITUDE_DELTA,
				FCMAP_H, FCMAP_KM,
				FCNCVC_H, FCNCVC_KM,
				FCWHTCC_H, FCWHTCC_KM,
				FCAAUX_H, FCAAUX_KM,
				FCADAS_H, FCADAS_KM,
				FCFINAL_H, FCFINAL_KM,
				FCFINAL_LITERPER100KM, FCFINAL_LITERPER100TKM, FCFINAL_LiterPer100M3KM,SPECIFIC_FC,
				CO2_KM, CO2_TKM, CO2_M3KM,
				P_WHEEL_POS, P_FCMAP_POS,
				E_FCMAP_POS, E_FCMAP_NEG, E_POWERTRAIN_INERTIA,
				E_AUX, E_CLUTCH_LOSS, E_TC_LOSS, E_SHIFT_LOSS, E_GBX_LOSS,
				E_RET_LOSS, E_ANGLE_LOSS, E_AXL_LOSS, E_BRAKE, E_VEHICLE_INERTIA, E_WHEEL , E_AIR, E_ROLL, E_GRAD ,
				ACC, ACC_POS, ACC_NEG, ACC_TIMESHARE, DEC_TIMESHARE, CRUISE_TIMESHARE,
				MAX_SPEED, MAX_ACCELERATION, MAX_DECELERATION, AVG_ENGINE_SPEED, MAX_ENGINE_SPEED, NUM_GEARSHIFTS,
				STOP_TIMESHARE, ENGINE_FULL_LOAD_TIME_SHARE, COASTING_TIME_SHARE, BRAKING_TIME_SHARE
			}.Select(x => new DataColumn(x, typeof(ConvertedSI))).ToArray());
		}

		/// <summary>
		/// Finishes the summary data container (writes the data to the sumWriter).
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void Finish()
		{
			if (_sumWriter != null) {
				var view = new DataView(Table, "", SORT, DataViewRowState.CurrentRows).ToTable();
				var toRemove =
					view.Columns.Cast<DataColumn>().Where(column => column.ColumnName.StartsWith(INTERNAL_PREFIX)).ToList();
				foreach (var colName in new[] { E_WHEEL, SPECIFIC_FC }) {
					var column = view.Columns[colName];
					if (view.AsEnumerable().All(dr => dr.IsNull(column))) {
						toRemove.Add(column);
					}
				}
				foreach (var dataColumn in toRemove) {
					view.Columns.Remove(dataColumn);
				}
				_sumWriter.WriteSumData(view);
			}
		}

		/// <summary>
		/// Writes the result of one run into the summary data container.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public virtual void Write(IModalDataContainer modData, int jobNr, int runNr, VectoRunData runData)
		{
			var row = Table.NewRow();
			Table.Rows.Add(row);

			row[SORT] = jobNr * 1000 + runNr;
			row[JOB] = string.Format("{0}-{1}", jobNr, runNr); //ReplaceNotAllowedCharacters(current);
			row[INPUTFILE] = ReplaceNotAllowedCharacters(runData.JobName);
			row[CYCLE] = ReplaceNotAllowedCharacters(runData.Cycle.Name + Constants.FileExtensions.CycleFile);

			row[STATUS] = modData.RunStatus;

			var vehicleLoading = 0.SI<Kilogram>();
			var cargoVolume = 0.SI<CubicMeter>();
			var gearCount = 0u;
			if (runData.Cycle.CycleType != CycleType.EngineOnly) {
				WriteFullPowertrain(runData, row);

				cargoVolume = runData.VehicleData.CargoVolume;
				vehicleLoading = runData.VehicleData.Loading;
				gearCount = (uint)runData.GearboxData.Gears.Count;
			}


			var totalTime = modData.Duration();
			row[TIME] = (ConvertedSI)totalTime;

			var distance = modData.Distance();
			if (distance != null) {
				row[DISTANCE] = distance.ConvertToKiloMeter();
			}

			var speed = modData.Speed();
			if (speed != null) {
				row[SPEED] = speed.ConvertToKiloMeterPerHour();
			}

			row[ALTITUDE_DELTA] = (ConvertedSI)modData.AltitudeDelta();

			WriteFuelconsumptionEntries(modData, row, vehicleLoading, cargoVolume, runData.Cycle.CycleType == CycleType.VTP);

			var kilogramPerMeter = modData.CO2PerMeter();
			if (kilogramPerMeter != null) {
				row[CO2_KM] = kilogramPerMeter.ConvertToGrammPerKiloMeter();
				if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
					row[CO2_TKM] = (kilogramPerMeter / vehicleLoading).ConvertToGrammPerTonKilometer();
				}
				if (cargoVolume > 0) {
					row[CO2_M3KM] = (kilogramPerMeter / cargoVolume).ConvertToGrammPerCubicMeterKiloMeter();
				}
			}

			row[P_WHEEL_POS] = modData.PowerWheelPositive().ConvertToKiloWatt();

			row[P_FCMAP_POS] = modData.TotalPowerEnginePositiveAverage().ConvertToKiloWatt();

			WriteAuxiliaries(modData, row);

			WriteWorkEntries(modData, row, runData.Cycle.CycleType == CycleType.VTP);

			WritePerformanceEntries(modData, row);

			row[ENGINE_FULL_LOAD_TIME_SHARE] = (ConvertedSI)modData.EngineMaxLoadTimeShare();
			row[COASTING_TIME_SHARE] = (ConvertedSI)modData.CoastingTimeShare();
			row[BRAKING_TIME_SHARE] = (ConvertedSI)modData.BrakingTimeShare();

			if (gearCount <= 0) {
				return;
			}

			WriteGearshiftStats(modData, row, gearCount);
		}

		private static void WriteFuelconsumptionEntries(IModalDataContainer modData, DataRow row, Kilogram vehicleLoading,
			CubicMeter cargoVolume, bool vtpCycle)
		{
			var tmp = modData.FCMapPerSecond();
			row[FCMAP_H] = tmp.ConvertToGrammPerHour();
			var fcMapPerMeter = modData.FCMapPerMeter();
			if (fcMapPerMeter != null) {
				row[FCMAP_KM] = fcMapPerMeter.ConvertToGrammPerKiloMeter();
			}

			row[FCNCVC_H] = modData.FuelConsumptionNCVCorrectedPerSecond().ConvertToGrammPerHour();
			var fuelConsumptionAuxStartStopCorrected = modData.FuelConsumptionNCVCorrected();
			row[FCNCVC_KM] = fuelConsumptionAuxStartStopCorrected.ConvertToGrammPerKiloMeter();

			row[FCWHTCC_H] = modData.FuelConsumptionWHTCPerSecond().ConvertToGrammPerHour();
			var fuelConsumptionWHTCCorrected = modData.FuelConsumptionWHTC();
			row[FCWHTCC_KM] = fuelConsumptionWHTCCorrected.ConvertToGrammPerKiloMeter();

			row[FCAAUX_H] = modData.FuelConsumptionAAUXPerSecond().ConvertToGrammPerHour();
			var fuelConsumptionAaux = modData.FuelConsumptionAAUX();
			row[FCAAUX_KM] = fuelConsumptionAaux.ConvertToGrammPerKiloMeter();

			row[FCADAS_H] = modData.FuelConsumptionADASPerSecond().ConvertToGrammPerHour();
			var fuelConsumptionAdas = modData.FuelConsumptionADAS();
			row[FCADAS_KM] = fuelConsumptionAdas.ConvertToGrammPerKiloMeter();

			row[FCFINAL_H] = modData.FuelConsumptionFinalPerSecond().ConvertToGrammPerHour();
			var fcfinal = modData.FuelConsumptionFinal();
			row[FCFINAL_KM] = fcfinal.ConvertToGrammPerKiloMeter();

			var fcFinal = modData.FuelConsumptionFinalVolumePerMeter();
			row[FCFINAL_LITERPER100KM] = fcFinal.ConvertToLiterPer100Kilometer();
			if (vehicleLoading != null && !vehicleLoading.IsEqual(0) && fcFinal != null) {
				row[FCFINAL_LITERPER100TKM] = (fcFinal / vehicleLoading).ConvertToLiterPer100TonKiloMeter();
			}
			if (cargoVolume > 0 && fcFinal != null) {
				row[FCFINAL_LiterPer100M3KM] = (fcFinal / cargoVolume).ConvertToLiterPerCubicMeter100KiloMeter();
			}

			if (vtpCycle) {
				row[SPECIFIC_FC] = (modData.TimeIntegral<Kilogram>(ModalResultField.FCFinal) / modData.WorkWheelsPos()).ConvertToGramPerKiloWattHour();
			}
		}


		private void WriteAuxiliaries(IModalDataContainer modData, DataRow row)
		{
			foreach (var aux in modData.Auxiliaries) {
				string colName;
				if (aux.Key == Constants.Auxiliaries.IDs.PTOConsumer || aux.Key == Constants.Auxiliaries.IDs.PTOTransmission) {
					colName = string.Format(E_FORMAT, aux.Key);
				} else {
					colName = string.Format(E_AUX_FORMAT, aux.Key);
				}

				if (!Table.Columns.Contains(colName)) {
					var col = Table.Columns.Add(colName, typeof(ConvertedSI));
					// move the new column to correct position
					col.SetOrdinal(Table.Columns[E_AUX].Ordinal);
				}

				row[colName] = modData.AuxiliaryWork(aux.Value).ConvertToKiloWattHour();
			}
		}

		private void WriteGearshiftStats(IModalDataContainer modData, DataRow row, uint gearCount)
		{
			row[NUM_GEARSHIFTS] = (ConvertedSI)modData.GearshiftCount();
			var timeSharePerGear = modData.TimeSharePerGear(gearCount);

			for (uint i = 0; i <= gearCount; i++) {
				var colName = string.Format(TIME_SHARE_PER_GEAR_FORMAT, i);
				if (!Table.Columns.Contains(colName)) {
					Table.Columns.Add(colName, typeof(ConvertedSI));
				}
				row[colName] = (ConvertedSI)timeSharePerGear[i];
			}
		}

		private void WritePerformanceEntries(IModalDataContainer modData, DataRow row)
		{
			row[ACC] = (ConvertedSI)modData.AccelerationAverage();
			row[ACC_POS] = (ConvertedSI)modData.AccelerationsPositive();
			row[ACC_NEG] = (ConvertedSI)modData.AccelerationsNegative();
			var accTimeShare = modData.AccelerationTimeShare();
			row[ACC_TIMESHARE] = (ConvertedSI)accTimeShare;
			var decTimeShare = modData.DecelerationTimeShare();
			row[DEC_TIMESHARE] = (ConvertedSI)decTimeShare;
			var cruiseTimeShare = modData.CruiseTimeShare();
			row[CRUISE_TIMESHARE] = (ConvertedSI)cruiseTimeShare;
			var stopTimeShare = modData.StopTimeShare();
			row[STOP_TIMESHARE] = (ConvertedSI)stopTimeShare;

			row[MAX_SPEED] = (ConvertedSI)modData.MaxSpeed().AsKmph.SI<Scalar>();
			row[MAX_ACCELERATION] = (ConvertedSI)modData.MaxAcceleration();
			row[MAX_DECELERATION] = (ConvertedSI)modData.MaxDeceleration();
			row[AVG_ENGINE_SPEED] = (ConvertedSI)modData.AvgEngineSpeed().AsRPM.SI<Scalar>();
			row[MAX_ENGINE_SPEED] = (ConvertedSI)modData.MaxEngineSpeed().AsRPM.SI<Scalar>();
			if (accTimeShare != null && decTimeShare != null && cruiseTimeShare != null) {
				var shareSum = accTimeShare + decTimeShare + cruiseTimeShare + stopTimeShare;
				if (!shareSum.IsEqual(100)) {
					Log.Error(
						"Sumfile Error: driving behavior timeshares must sum up to 100%: acc: {0}%, dec: {1}%, cruise: {2}%, stop: {3}%, sum: {4}%",
						accTimeShare.ToOutputFormat(1, null, false), decTimeShare.ToOutputFormat(1, null, false),
						cruiseTimeShare.ToOutputFormat(1, null, false), stopTimeShare.ToOutputFormat(1, null, false),
						shareSum.ToOutputFormat(1, null, false));
				}
			}
		}

		private static void WriteWorkEntries(IModalDataContainer modData, DataRow row, bool vtpMode)
		{
			row[E_FCMAP_POS] = modData.TotalEngineWorkPositive().ConvertToKiloWattHour();
			row[E_FCMAP_NEG] = (-modData.TotalEngineWorkNegative()).ConvertToKiloWattHour();
			row[E_POWERTRAIN_INERTIA] = modData.PowerAccelerations().ConvertToKiloWattHour();
			row[E_AUX] = modData.WorkAuxiliaries().ConvertToKiloWattHour();
			row[E_CLUTCH_LOSS] = modData.WorkClutch().ConvertToKiloWattHour();
			row[E_TC_LOSS] = modData.WorkTorqueConverter().ConvertToKiloWattHour();
			row[E_SHIFT_LOSS] = modData.WorkGearshift().ConvertToKiloWattHour();
			row[E_GBX_LOSS] = modData.WorkGearbox().ConvertToKiloWattHour();
			row[E_RET_LOSS] = modData.WorkRetarder().ConvertToKiloWattHour();
			row[E_AXL_LOSS] = modData.WorkAxlegear().ConvertToKiloWattHour();
			row[E_ANGLE_LOSS] = modData.WorkAngledrive().ConvertToKiloWattHour();
			row[E_BRAKE] = modData.WorkTotalMechanicalBrake().ConvertToKiloWattHour();
			row[E_VEHICLE_INERTIA] = modData.WorkVehicleInertia().ConvertToKiloWattHour();
			row[E_AIR] = modData.WorkAirResistance().ConvertToKiloWattHour();
			row[E_ROLL] = modData.WorkRollingResistance().ConvertToKiloWattHour();
			row[E_GRAD] = modData.WorkRoadGradientResistance().ConvertToKiloWattHour();
			if (vtpMode) {
				row[E_WHEEL] = modData.WorkWheels().ConvertToKiloWattHour();
			}
		}

		private void WriteFullPowertrain(VectoRunData runData, DataRow row)
		{
			row[VEHICLE_MANUFACTURER] = runData.VehicleData.Manufacturer;
			row[VIN_NUMBER] = runData.VehicleData.VIN;
			row[VEHICLE_MODEL] = runData.VehicleData.ModelName;

			row[HDV_CO2_VEHICLE_CLASS] = runData.VehicleData.VehicleClass.GetClassNumber();
			row[CURB_MASS] = (ConvertedSI)runData.VehicleData.CurbWeight;
			// - (runData.VehicleData.BodyAndTrailerWeight ?? 0.SI<Kilogram>());
			row[LOADING] = (ConvertedSI)runData.VehicleData.Loading;
			row[CARGO_VOLUME] = (ConvertedSI)runData.VehicleData.CargoVolume;

			row[TOTAL_VEHICLE_MASS] = (ConvertedSI)runData.VehicleData.TotalVehicleWeight;
			row[ENGINE_MANUFACTURER] = runData.EngineData.Manufacturer;
			row[ENGINE_MODEL] = runData.EngineData.ModelName;
			row[ENGINE_FUEL_TYPE] = runData.EngineData.FuelData.GetLabel();
			row[ENGINE_RATED_POWER] = runData.EngineData.RatedPowerDeclared != null && runData.EngineData.RatedPowerDeclared > 0
				? runData.EngineData.RatedPowerDeclared.ConvertToKiloWatt()
				: runData.EngineData.FullLoadCurves[0].MaxPower.ConvertToKiloWatt();
			row[ENGINE_IDLING_SPEED] = (ConvertedSI)runData.EngineData.IdleSpeed.AsRPM.SI<Scalar>();
			row[ENGINE_RATED_SPEED] = runData.EngineData.RatedSpeedDeclared != null && runData.EngineData.RatedSpeedDeclared > 0
				? (ConvertedSI)runData.EngineData.RatedSpeedDeclared.AsRPM.SI<Scalar>()
				: (ConvertedSI)runData.EngineData.FullLoadCurves[0].RatedSpeed.AsRPM.SI<Scalar>();
			row[ENGINE_DISPLACEMENT] = runData.EngineData.Displacement.ConvertToCubicCentiMeter();

			row[ENGINE_WHTC_URBAN] = runData.EngineData.WHTCUrban;
			row[ENGINE_WHTC_RURAL] = runData.EngineData.WHTCRural;
			row[ENGINE_WHTC_MOTORWAY] = runData.EngineData.WHTCMotorway;
			row[ENGINE_BF_COLD_HOT] = runData.EngineData.ColdHotCorrectionFactor;
			row[ENGINE_CF_REG_PER] = runData.EngineData.CorrectionFactorRegPer;
			row[ENGINE_ACTUAL_CORRECTION_FACTOR] = runData.EngineData.FuelConsumptionCorrectionFactor;

			row[CD_x_A_DECLARED] = (ConvertedSI)runData.AirdragData.DeclaredAirdragArea;
			row[CD_x_A] = (ConvertedSI)runData.AirdragData.CrossWindCorrectionCurve.AirDragArea;

			row[ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER] =
				runData.VehicleData.RollResistanceCoefficientWithoutTrailer;
			row[ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER] =
				runData.VehicleData.TotalRollResistanceCoefficient;

			row[R_DYN] = (ConvertedSI)runData.VehicleData.DynamicTyreRadius;

			row[NUM_AXLES_DRIVEN] = runData.VehicleData.AxleData.Count(x => x.AxleType == AxleType.VehicleDriven);
			row[NUM_AXLES_NON_DRIVEN] = runData.VehicleData.AxleData.Count(x => x.AxleType == AxleType.VehicleNonDriven);
			row[NUM_AXLES_TRAILER] = runData.VehicleData.AxleData.Count(x => x.AxleType == AxleType.Trailer);

			row[GEARBOX_MANUFACTURER] = runData.GearboxData.Manufacturer;
			row[GEARBOX_MODEL] = runData.GearboxData.ModelName;
			row[GEARBOX_TYPE] = runData.GearboxData.Type;
			WriteGearboxData(runData, row);

			row[RETARDER_TYPE] = runData.Retarder.Type.GetLabel();
			WriteRetarderData(runData, row);

			WriteAngledriveData(runData, row);

			row[AXLE_MANUFACTURER] = runData.AxleGearData.Manufacturer;
			row[AXLE_MODEL] = runData.AxleGearData.ModelName;
			row[AXLE_RATIO] = (ConvertedSI)runData.AxleGearData.AxleGear.Ratio.SI<Scalar>();

			WriteAuxTechnologies(runData, row);
		}

		private void WriteAuxTechnologies(VectoRunData runData, DataRow row)
		{
			foreach (var aux in runData.Aux) {
				if (aux.ID == Constants.Auxiliaries.IDs.PTOConsumer || aux.ID == Constants.Auxiliaries.IDs.PTOTransmission) {
					continue;
				}
				var colName = string.Format(AUX_TECH_FORMAT, aux.ID);

				if (!Table.Columns.Contains(colName)) {
					var col = Table.Columns.Add(colName, typeof(string));
					// move the new column to correct position
					col.SetOrdinal(Table.Columns[CARGO_VOLUME].Ordinal);
				}

				row[colName] = aux.Technology == null ? "" : string.Join("; ", aux.Technology);
			}
		}

		private static void WriteAngledriveData(VectoRunData runData, DataRow row)
		{
			if (runData.AngledriveData != null) {
				row[ANGLEDRIVE_MANUFACTURER] = runData.AngledriveData.Manufacturer;
				row[ANGLEDRIVE_MODEL] = runData.AngledriveData.ModelName;
				row[ANGLEDRIVE_RATIO] = runData.AngledriveData.Angledrive.Ratio;
			} else {
				row[ANGLEDRIVE_MANUFACTURER] = "n.a.";
				row[ANGLEDRIVE_MODEL] = "n.a.";
				row[ANGLEDRIVE_RATIO] = "n.a.";
			}
		}

		private static void WriteRetarderData(VectoRunData runData, DataRow row)
		{
			if (runData.Retarder.Type.IsDedicatedComponent()) {
				row[RETARDER_MANUFACTURER] = runData.Retarder.Manufacturer;
				row[RETARDER_MODEL] = runData.Retarder.ModelName;
			} else {
				row[RETARDER_MANUFACTURER] = "n.a.";
				row[RETARDER_MODEL] = "n.a.";
			}
		}

		private static void WriteGearboxData(VectoRunData runData, DataRow row)
		{
			if (runData.GearboxData.Type.AutomaticTransmission()) {
				row[GEAR_RATIO_FIRST_GEAR] = runData.GearboxData.Gears.Count > 0
					? (double.IsNaN(runData.GearboxData.Gears.First().Value.Ratio)
						? (ConvertedSI)runData.GearboxData.Gears.First().Value.TorqueConverterRatio.SI<Scalar>()
						: (ConvertedSI)runData.GearboxData.Gears.First().Value.Ratio.SI<Scalar>())
					: 0.SI<Scalar>();
				row[GEAR_RATIO_LAST_GEAR] = runData.GearboxData.Gears.Count > 0
					? (ConvertedSI)runData.GearboxData.Gears.Last().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[TORQUECONVERTER_MANUFACTURER] = runData.GearboxData.TorqueConverterData.Manufacturer;
				row[TORQUECONVERTER_MODEL] = runData.GearboxData.TorqueConverterData.ModelName;
			} else {
				row[GEAR_RATIO_FIRST_GEAR] = runData.GearboxData.Gears.Count > 0
					? (ConvertedSI)runData.GearboxData.Gears.First().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[GEAR_RATIO_LAST_GEAR] = runData.GearboxData.Gears.Count > 0
					? (ConvertedSI)runData.GearboxData.Gears.Last().Value.Ratio.SI<Scalar>()
					: (ConvertedSI)0.SI<Scalar>();
				row[TORQUECONVERTER_MANUFACTURER] = "n.a.";
				row[TORQUECONVERTER_MODEL] = "n.a.";
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
				Table.Dispose();
			}
		}
	}
}