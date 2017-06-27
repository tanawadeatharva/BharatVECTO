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
		public const string CURB_MASS = "Chassis curb mass [kg]";
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
		public const string FCAUXC_H = "FC-AUXc [g/h]";
		public const string FCAUXC_KM = "FC-AUXc [g/km]";
		public const string FCWHTCC_H = "FC-WHTCc [g/h]";
		public const string FCWHTCC_KM = "FC-WHTCc [g/km]";
		public const string FCAAUX_H = "FC-AAUX [g/h]";
		public const string FCAAUX_KM = "FC-AAUX [g/km]";

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

		public const string ACC = "a [m/s^2]";
		public const string ACC_POS = "a_pos [m/s^2]";
		public const string ACC_NEG = "a_neg [m/s^2]";

		public const string ACC_TIMESHARE = "AccelerationTimeShare [%]";
		public const string DEC_TIMESHARE = "DecelerationTimeShare [%]";
		public const string CRUISE_TIMESHARE = "CruiseTimeShare [%]";
		public const string STOP_TIMESHARE = "StopTimeShare [%]";

		public const string MAX_SPEED = "max. speed [km/h";
		public const string MAX_ACCELERATION = "max. acc [m/s²]";
		public const string MAX_DECELERATION = "max. dec [m/s²]";
		public const string AVG_ENGINE_SPEED = "n_eng_avg [rpm]";
		public const string MAX_ENGINE_SPEED = "n_eng_max [rpm]";
		public const string NUM_GEARSHIFTS = "gear shifts [-]";
		public const string ENGINE_FULL_LOAD_TIME_SHARE = "Engine max. Load time share [%]";
		public const string COASTING_TIME_SHARE = "CoastingTimeShare [%]";
		public const string BRAKING_TIME_SHARE = "BrakingTImeShare [%]";

		public const string TIME_SHARE_PER_GEAR_FORMAT = "Gear {0} TimeShare [%]";

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
				Tuple.Create(CURB_MASS, typeof(SI)),
				Tuple.Create(LOADING, typeof(SI)),
				Tuple.Create(TOTAL_VEHICLE_MASS, typeof(SI)),
				Tuple.Create(ENGINE_MANUFACTURER, typeof(string)),
				Tuple.Create(ENGINE_MODEL, typeof(string)),
				Tuple.Create(ENGINE_FUEL_TYPE, typeof(string)),
				Tuple.Create(ENGINE_RATED_POWER, typeof(SI)),
				Tuple.Create(ENGINE_IDLING_SPEED, typeof(SI)),
				Tuple.Create(ENGINE_RATED_SPEED, typeof(SI)),
				Tuple.Create(ENGINE_DISPLACEMENT, typeof(SI)),
				Tuple.Create(ENGINE_WHTC_URBAN, typeof(double)),
				Tuple.Create(ENGINE_WHTC_RURAL, typeof(double)),
				Tuple.Create(ENGINE_WHTC_MOTORWAY, typeof(double)),
				Tuple.Create(ENGINE_BF_COLD_HOT, typeof(double)),
				Tuple.Create(ENGINE_CF_REG_PER, typeof(double)),
				Tuple.Create(ENGINE_ACTUAL_CORRECTION_FACTOR, typeof(double)),
				Tuple.Create(CD_x_A, typeof(SI)),
				Tuple.Create(ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER, typeof(double)),
				Tuple.Create(ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER, typeof(double)),
				Tuple.Create(R_DYN, typeof(SI)),
				Tuple.Create(GEARBOX_MANUFACTURER, typeof(string)),
				Tuple.Create(GEARBOX_MODEL, typeof(string)),
				Tuple.Create(GEARBOX_TYPE, typeof(string)),
				Tuple.Create(GEAR_RATIO_FIRST_GEAR, typeof(SI)),
				Tuple.Create(GEAR_RATIO_LAST_GEAR, typeof(SI)),
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
				Tuple.Create(AXLE_RATIO, typeof(SI)),
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
				FCAUXC_H, FCAUXC_KM,
				FCWHTCC_H, FCWHTCC_KM,
				FCAAUX_H, FCAAUX_KM,
				FCFINAL_H, FCFINAL_KM,
				FCFINAL_LITERPER100KM, FCFINAL_LITERPER100TKM, FCFINAL_LiterPer100M3KM,
				CO2_KM, CO2_TKM, CO2_M3KM,
				P_WHEEL_POS, P_FCMAP_POS,
				E_FCMAP_POS, E_FCMAP_NEG, E_POWERTRAIN_INERTIA,
				E_AUX, E_CLUTCH_LOSS, E_TC_LOSS, E_SHIFT_LOSS, E_GBX_LOSS,
				E_RET_LOSS, E_ANGLE_LOSS, E_AXL_LOSS, E_BRAKE, E_VEHICLE_INERTIA, E_AIR, E_ROLL, E_GRAD,
				ACC, ACC_POS, ACC_NEG, ACC_TIMESHARE, DEC_TIMESHARE, CRUISE_TIMESHARE, STOP_TIMESHARE,
				MAX_SPEED, MAX_ACCELERATION, MAX_DECELERATION, AVG_ENGINE_SPEED, MAX_ENGINE_SPEED, NUM_GEARSHIFTS,
				ENGINE_FULL_LOAD_TIME_SHARE, COASTING_TIME_SHARE, BRAKING_TIME_SHARE
			}.Select(x => new DataColumn(x, typeof(SI))).ToArray());
		}

		/// <summary>
		/// Finishes the summary data container (writes the data to the sumWriter).
		/// </summary>
		public virtual void Finish()
		{
			if (_sumWriter != null) {
				var view = new DataView(Table, "", SORT, DataViewRowState.CurrentRows).ToTable();
				var toRemove =
					view.Columns.Cast<DataColumn>().Where(column => column.ColumnName.StartsWith(INTERNAL_PREFIX)).ToList();
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
		//public virtual void Write(IModalDataContainer modData, string jobFileName, string jobName, string cycleFileName,
		//	Kilogram vehicleMass, Kilogram vehicleLoading, CubicMeter cargoVolume, uint gearCount)
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
			row[TIME] = totalTime;

			var distance = modData.Distance();
			if (distance != null) {
				row[DISTANCE] = distance.ConvertTo().Kilo.Meter;
			}

			var speed = modData.Speed();
			if (speed != null) {
				row[SPEED] = speed.ConvertTo().Kilo.Meter.Per.Hour;
			}

			row[ALTITUDE_DELTA] = modData.AltitudeDelta();

			WriteFuelconsumptionEntries(modData, row, vehicleLoading, cargoVolume);

			var kilogramPerMeter = modData.CO2PerMeter();
			if (kilogramPerMeter != null) {
				row[CO2_KM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter;
				if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
					row[CO2_TKM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter / vehicleLoading.ConvertTo().Ton;
				}
				if (cargoVolume > 0) {
					row[CO2_M3KM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter / cargoVolume;
				}
			}

			row[P_WHEEL_POS] = modData.PowerWheelPositive().ConvertTo().Kilo.Watt;

			row[P_FCMAP_POS] = modData.TotalPowerEnginePositiveAverage().ConvertTo().Kilo.Watt;

			WriteAuxiliaries(modData, row);

			WriteWorkEntries(modData, row);

			//var acc = modData.AccelerationPer3Seconds();


			WritePerformanceEntries(modData, row);

			row[ENGINE_FULL_LOAD_TIME_SHARE] = modData.EngineMaxLoadTimeShare();
			row[COASTING_TIME_SHARE] = modData.CoastingTimeShare();
			row[BRAKING_TIME_SHARE] = modData.BrakingTimeShare();


			if (gearCount <= 0) {
				return;
			}

			WriteGearshiftStats(modData, row, gearCount);
		}

		private static void WriteFuelconsumptionEntries(IModalDataContainer modData, DataRow row, Kilogram vehicleLoading,
			CubicMeter cargoVolume)
		{
			row[FCMAP_H] = modData.FCMapPerSecond().ConvertTo().Gramm.Per.Hour;
			var fcMapPerMeter = modData.FCMapPerMeter();
			if (fcMapPerMeter != null) {
				row[FCMAP_KM] = fcMapPerMeter.ConvertTo().Gramm.Per.Kilo.Meter;
			}

			row[FCAUXC_H] = modData.FuelConsumptionAuxStartStopPerSecond().ConvertTo().Gramm.Per.Hour;
			var fuelConsumptionAuxStartStopCorrected = modData.FuelConsumptionAuxStartStop();
			if (fuelConsumptionAuxStartStopCorrected != null) {
				row[FCAUXC_KM] = fuelConsumptionAuxStartStopCorrected.ConvertTo().Gramm.Per.Kilo.Meter;
			}

			row[FCWHTCC_H] = modData.FuelConsumptionWHTCPerSecond().ConvertTo().Gramm.Per.Hour;
			var fuelConsumptionWHTCCorrected = modData.FuelConsumptionWHTC();
			if (fuelConsumptionWHTCCorrected != null) {
				row[FCWHTCC_KM] = fuelConsumptionWHTCCorrected.ConvertTo().Gramm.Per.Kilo.Meter;
			}

			row[FCAAUX_H] = modData.FuelConsumptionAAUXPerSecond().ConvertTo().Gramm.Per.Hour;
			var fuelConsumptionAaux = modData.FuelConsumptionAAUX();
			if (fuelConsumptionAaux != null) {
				row[FCAAUX_KM] = fuelConsumptionAaux.ConvertTo().Gramm.Per.Kilo.Meter;
			}

			row[FCFINAL_H] = modData.FuelConsumptionFinalPerSecond().ConvertTo().Gramm.Per.Hour;
			var fcfinal = modData.FuelConsumptionFinal();
			if (fcfinal != null) {
				row[FCFINAL_KM] = fcfinal.ConvertTo().Gramm.Per.Kilo.Meter;
			}

			var fcPer100lkm = modData.FuelConsumptionFinalLiterPer100Kilometer();
			row[FCFINAL_LITERPER100KM] = fcPer100lkm;
			if (vehicleLoading != null && !vehicleLoading.IsEqual(0) && fcPer100lkm != null) {
				row[FCFINAL_LITERPER100TKM] = fcPer100lkm /
											vehicleLoading.ConvertTo().Ton;
			}
			if (cargoVolume > 0 && fcPer100lkm != null) {
				row[FCFINAL_LiterPer100M3KM] = fcPer100lkm / cargoVolume;
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
					var col = Table.Columns.Add(colName, typeof(SI));
					// move the new column to correct position
					col.SetOrdinal(Table.Columns[E_AUX].Ordinal);
				}

				row[colName] = modData.AuxiliaryWork(aux.Value).ConvertTo().Kilo.Watt.Hour;
			}
		}

		private void WriteGearshiftStats(IModalDataContainer modData, DataRow row, uint gearCount)
		{
			row[NUM_GEARSHIFTS] = modData.GearshiftCount();
			var timeSharePerGear = modData.TimeSharePerGear(gearCount);

			for (uint i = 0; i <= gearCount; i++) {
				var colName = string.Format(TIME_SHARE_PER_GEAR_FORMAT, i);
				if (!Table.Columns.Contains(colName)) {
					Table.Columns.Add(colName, typeof(SI));
				}
				row[colName] = timeSharePerGear[i];
			}
		}

		private void WritePerformanceEntries(IModalDataContainer modData, DataRow row)
		{
			row[ACC] = modData.AccelerationAverage();
			row[ACC_POS] = modData.AccelerationsPositive();
			row[ACC_NEG] = modData.AccelerationsNegative();
			var accTimeShare = modData.AccelerationTimeShare();
			row[ACC_TIMESHARE] = accTimeShare;
			var decTimeShare = modData.DecelerationTimeShare();
			row[DEC_TIMESHARE] = decTimeShare;
			var cruiseTimeShare = modData.CruiseTimeShare();
			row[CRUISE_TIMESHARE] = cruiseTimeShare;
			var stopTimeShare = modData.StopTimeShare();
			row[STOP_TIMESHARE] = stopTimeShare;

			row[MAX_SPEED] = modData.MaxSpeed().AsKmph.SI<Scalar>();
			row[MAX_ACCELERATION] = modData.MaxAcceleration();
			row[MAX_DECELERATION] = modData.MaxDeceleration();
			row[AVG_ENGINE_SPEED] = modData.AvgEngineSpeed().AsRPM.SI<Scalar>();
			row[MAX_ENGINE_SPEED] = modData.MaxEngineSpeed().AsRPM.SI<Scalar>();
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

		private static void WriteWorkEntries(IModalDataContainer modData, DataRow row)
		{
			row[E_FCMAP_POS] = modData.TotalEngineWorkPositive().ConvertTo().Kilo.Watt.Hour;
			row[E_FCMAP_NEG] = -modData.TotalEngineWorkNegative().ConvertTo().Kilo.Watt.Hour;
			row[E_POWERTRAIN_INERTIA] = modData.PowerAccelerations().ConvertTo().Kilo.Watt.Hour;
			row[E_AUX] = modData.WorkAuxiliaries().ConvertTo().Kilo.Watt.Hour;
			row[E_CLUTCH_LOSS] = modData.WorkClutch().ConvertTo().Kilo.Watt.Hour;
			row[E_TC_LOSS] = modData.WorkTorqueConverter().ConvertTo().Kilo.Watt.Hour;
			row[E_SHIFT_LOSS] = modData.WorkGearshift().ConvertTo().Kilo.Watt.Hour;
			row[E_GBX_LOSS] = modData.WorkGearbox().ConvertTo().Kilo.Watt.Hour;
			row[E_RET_LOSS] = modData.WorkRetarder().ConvertTo().Kilo.Watt.Hour;
			row[E_AXL_LOSS] = modData.WorkAxlegear().ConvertTo().Kilo.Watt.Hour;
			row[E_ANGLE_LOSS] = modData.WorkAngledrive().ConvertTo().Kilo.Watt.Hour;
			row[E_BRAKE] = modData.WorkTotalMechanicalBrake().ConvertTo().Kilo.Watt.Hour;
			row[E_VEHICLE_INERTIA] = modData.WorkVehicleInertia().ConvertTo().Kilo.Watt.Hour;
			row[E_AIR] = modData.WorkAirResistance().ConvertTo().Kilo.Watt.Hour;
			row[E_ROLL] = modData.WorkRollingResistance().ConvertTo().Kilo.Watt.Hour;
			row[E_GRAD] = modData.WorkRoadGradientResistance().ConvertTo().Kilo.Watt.Hour;
		}

		private void WriteFullPowertrain(VectoRunData runData, DataRow row)
		{
			row[VEHICLE_MANUFACTURER] = runData.VehicleData.Manufacturer;
			row[VIN_NUMBER] = runData.VehicleData.VIN;
			row[VEHICLE_MODEL] = runData.VehicleData.ModelName;

			row[HDV_CO2_VEHICLE_CLASS] = runData.VehicleData.VehicleClass.GetClassNumber();
			row[CURB_MASS] = runData.VehicleData.CurbWeight;
			// - (runData.VehicleData.BodyAndTrailerWeight ?? 0.SI<Kilogram>());
			row[LOADING] = runData.VehicleData.Loading;
			row[CARGO_VOLUME] = runData.VehicleData.CargoVolume;

			row[TOTAL_VEHICLE_MASS] = runData.VehicleData.TotalVehicleWeight;
			row[ENGINE_MANUFACTURER] = runData.EngineData.Manufacturer;
			row[ENGINE_MODEL] = runData.EngineData.ModelName;
			row[ENGINE_FUEL_TYPE] = runData.EngineData.FuelType.GetLabel();
			row[ENGINE_RATED_POWER] = runData.EngineData.RatedPowerDeclared != null
				? runData.EngineData.RatedPowerDeclared.ConvertTo().Kilo.Watt
				: runData.EngineData.FullLoadCurves[0].MaxPower.ConvertTo().Kilo.Watt;
			row[ENGINE_IDLING_SPEED] = runData.EngineData.IdleSpeed.AsRPM.SI<Scalar>();
			row[ENGINE_RATED_SPEED] = runData.EngineData.RatedSpeedDeclared != null
				? runData.EngineData.RatedSpeedDeclared.AsRPM.SI<Scalar>()
				: runData.EngineData.FullLoadCurves[0].RatedSpeed.AsRPM.SI<Scalar>();
			row[ENGINE_DISPLACEMENT] = runData.EngineData.Displacement.ConvertTo().Cubic.Centi.Meter;

			row[ENGINE_WHTC_URBAN] = runData.EngineData.WHTCUrban;
			row[ENGINE_WHTC_RURAL] = runData.EngineData.WHTCRural;
			row[ENGINE_WHTC_MOTORWAY] = runData.EngineData.WHTCMotorway;
			row[ENGINE_BF_COLD_HOT] = runData.EngineData.ColdHotCorrectionFactor;
			row[ENGINE_CF_REG_PER] = runData.EngineData.CorrectionFactorRegPer;
			row[ENGINE_ACTUAL_CORRECTION_FACTOR] = runData.EngineData.FuelConsumptionCorrectionFactor;

			row[CD_x_A] = runData.AirdragData.CrossWindCorrectionCurve.AirDragArea;

			row[ROLLING_RESISTANCE_COEFFICIENT_WO_TRAILER] =
				runData.VehicleData.RollResistanceCoefficientWithoutTrailer;
			row[ROLLING_RESISTANCE_COEFFICIENT_W_TRAILER] =
				runData.VehicleData.TotalRollResistanceCoefficient;

			row[R_DYN] = runData.VehicleData.DynamicTyreRadius;

			row[GEARBOX_MANUFACTURER] = runData.GearboxData.Manufacturer;
			row[GEARBOX_MODEL] = runData.GearboxData.ModelName;
			row[GEARBOX_TYPE] = runData.GearboxData.Type;
			if (runData.GearboxData.Type.AutomaticTransmission()) {
				row[GEAR_RATIO_FIRST_GEAR] = runData.GearboxData.Gears.Count > 0
					? (double.IsNaN(runData.GearboxData.Gears.First().Value.Ratio)
						? runData.GearboxData.Gears.First().Value.TorqueConverterRatio.SI<Scalar>()
						: runData.GearboxData.Gears.First().Value.Ratio.SI<Scalar>())
					: 0.SI<Scalar>();
				row[GEAR_RATIO_LAST_GEAR] = runData.GearboxData.Gears.Count > 0
					? runData.GearboxData.Gears.Last().Value.Ratio.SI<Scalar>()
					: 0.SI<Scalar>();
				row[TORQUECONVERTER_MANUFACTURER] = runData.GearboxData.TorqueConverterData.Manufacturer;
				row[TORQUECONVERTER_MODEL] = runData.GearboxData.TorqueConverterData.ModelName;
			} else {
				row[GEAR_RATIO_FIRST_GEAR] = runData.GearboxData.Gears.Count > 0
					? runData.GearboxData.Gears.First().Value.Ratio.SI<Scalar>()
					: 0.SI<Scalar>();
				row[GEAR_RATIO_LAST_GEAR] = runData.GearboxData.Gears.Count > 0
					? runData.GearboxData.Gears.Last().Value.Ratio.SI<Scalar>()
					: 0.SI<Scalar>();
				row[TORQUECONVERTER_MANUFACTURER] = "n.a.";
				row[TORQUECONVERTER_MODEL] = "n.a.";
			}
			row[RETARDER_TYPE] = runData.Retarder.Type.GetLabel();
			if (runData.Retarder.Type.IsDedicatedComponent()) {
				row[RETARDER_MANUFACTURER] = runData.Retarder.Manufacturer;
				row[RETARDER_MODEL] = runData.Retarder.ModelName;
			} else {
				row[RETARDER_MANUFACTURER] = "n.a.";
				row[RETARDER_MODEL] = "n.a.";
			}

			if (runData.AngledriveData != null) {
				row[ANGLEDRIVE_MANUFACTURER] = runData.AngledriveData.Manufacturer;
				row[ANGLEDRIVE_MODEL] = runData.AngledriveData.ModelName;
				row[ANGLEDRIVE_RATIO] = runData.AngledriveData.Angledrive.Ratio;
			} else {
				row[ANGLEDRIVE_MANUFACTURER] = "n.a.";
				row[ANGLEDRIVE_MODEL] = "n.a.";
				row[ANGLEDRIVE_RATIO] = "n.a.";
			}

			row[AXLE_MANUFACTURER] = runData.AxleGearData.Manufacturer;
			row[AXLE_MODEL] = runData.AxleGearData.ModelName;
			row[AXLE_RATIO] = runData.AxleGearData.AxleGear.Ratio.SI<Scalar>();

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