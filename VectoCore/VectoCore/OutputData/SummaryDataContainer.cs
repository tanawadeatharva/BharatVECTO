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
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using Org.BouncyCastle.Asn1;
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
		public const string JOB = "Job [-]";
		public const string INPUTFILE = "Input File [-]";
		public const string CYCLE = "Cycle [-]";
		public const string STATUS = "Status";
		public const string CURB_MASS = "Chassis curb mass [kg]";
		public const string LOADING = "Loading [kg]";

		public const string HDV_CO2_VEHICLE_CLASS = "HDV CO2 vehicle class [-]";
		public const string TOTAL_VEHICLE_MASS = "Total vehicle mass [kg]";
		public const string ENGINE_RATED_POWER = "Engine rated power [kW]";
		public const string ENGINE_IDLING_SPEED = "Engine idling speed [rpm]";
		public const string ENGINE_RATED_SPEED = "Engine rated speed [rpm]";
		public const string ENGINE_DISPLACEMENT = "Engine displacement [ccm]";
		public const string CD_x_A = "CdxA [m²]";
		public const string ROLLING_RESISTANCE_COEFFICIENT = "weighed RRC [-]";
		public const string TRANSMISSION_TYPE = "Transmission type [-]";
		public const string GEAR_RATIO_FIRST_GEAR = "Gear ratio first gear [-]";
		public const string GEAR_RATIO_LAST_GEAR = "Gear ratio last gear [-]";
		public const string AXLE_GEAR_RATIO = "Axle gear ratio [-]";
		public const string R_DYN = "r_dyn [m]";
		public const string RETARDER_TYPE = "Retarder type [-]";

		public const string VOLUME = "Cargo Volume [m³]";
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

		internal readonly DataTable _table;
		private readonly ISummaryWriter _sumWriter;

		protected SummaryDataContainer() {}

		/// <summary>
		/// Initializes a new instance of the <see cref="SummaryDataContainer"/> class.
		/// </summary>
		/// <param name="writer"></param>
		public SummaryDataContainer(ISummaryWriter writer)
		{
			_sumWriter = writer;

			_table = new DataTable();
			_table.Columns.Add(JOB, typeof(string));
			_table.Columns.Add(INPUTFILE, typeof(string));
			_table.Columns.Add(CYCLE, typeof(string));
			_table.Columns.Add(STATUS, typeof(string));
			_table.Columns.Add(HDV_CO2_VEHICLE_CLASS, typeof(string));

			_table.Columns.AddRange(new[] {
				CURB_MASS, LOADING, TOTAL_VEHICLE_MASS, ENGINE_RATED_POWER, ENGINE_IDLING_SPEED, ENGINE_RATED_SPEED,
				ENGINE_DISPLACEMENT, CD_x_A,
				ROLLING_RESISTANCE_COEFFICIENT
			}.Select(x => new DataColumn(x, typeof(SI))).ToArray());
			_table.Columns.Add(TRANSMISSION_TYPE, typeof(string));
			_table.Columns.AddRange(new[] {
				GEAR_RATIO_FIRST_GEAR, GEAR_RATIO_LAST_GEAR, AXLE_GEAR_RATIO, R_DYN
			}.Select(x => new DataColumn(x, typeof(SI))).ToArray());
			_table.Columns.Add(RETARDER_TYPE, typeof(string));
			_table.Columns.AddRange(new[] {
				VOLUME, TIME, DISTANCE, SPEED, ALTITUDE_DELTA, FCMAP_H, FCMAP_KM, FCAUXC_H, FCAUXC_KM,
				FCWHTCC_H,
				FCWHTCC_KM,
				FCAAUX_H, FCAAUX_KM, FCFINAL_H, FCFINAL_KM, FCFINAL_LITERPER100KM, FCFINAL_LITERPER100TKM, FCFINAL_LiterPer100M3KM,
				CO2_KM, CO2_TKM, CO2_M3KM,
				P_WHEEL_POS, P_FCMAP_POS,
				E_FCMAP_POS, E_FCMAP_NEG, E_POWERTRAIN_INERTIA, E_AUX, E_CLUTCH_LOSS, E_TC_LOSS, E_SHIFT_LOSS, E_GBX_LOSS,
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
				_sumWriter.WriteSumData(new DataView(_table, "", JOB, DataViewRowState.CurrentRows).ToTable());
			}
		}

		/// <summary>
		/// Writes the result of one run into the summary data container.
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		//public virtual void Write(IModalDataContainer modData, string jobFileName, string jobName, string cycleFileName,
		//	Kilogram vehicleMass, Kilogram vehicleLoading, CubicMeter cargoVolume, uint gearCount)
		public virtual void Write(IModalDataContainer modData, string current, VectoRunData runData)
		{
			var row = _table.NewRow();
			_table.Rows.Add(row);

			row[JOB] = ReplaceNotAllowedCharacters(current);
			row[INPUTFILE] = ReplaceNotAllowedCharacters(runData.JobName);
			row[CYCLE] = ReplaceNotAllowedCharacters(runData.Cycle.Name + Constants.FileExtensions.CycleFile);
			row[STATUS] = modData.RunStatus;

			var vehicleLoading = 0.SI<Kilogram>();
			var cargoVolume = 0.SI<CubicMeter>();
			uint gearCount = 0u;
			if (runData.Cycle.CycleType != CycleType.EngineOnly) {
				row[HDV_CO2_VEHICLE_CLASS] = runData.VehicleData.VehicleClass.GetClassNumber();
				row[CURB_MASS] = runData.VehicleData.CurbWeight - (runData.VehicleData.BodyAndTrailerWeight ?? 0.SI<Kilogram>());
				row[LOADING] = runData.VehicleData.Loading;
				row[VOLUME] = runData.VehicleData.CargoVolume;

				row[TOTAL_VEHICLE_MASS] = runData.VehicleData.TotalVehicleWeight;
				row[ENGINE_RATED_POWER] =
					runData.EngineData.FullLoadCurve.FullLoadStationaryPower(runData.EngineData.FullLoadCurve.RatedSpeed)
						.ConvertTo().Kilo.Watt;
				row[ENGINE_IDLING_SPEED] = runData.EngineData.IdleSpeed.AsRPM.SI<Scalar>();
				row[ENGINE_RATED_SPEED] = runData.EngineData.FullLoadCurve.RatedSpeed.AsRPM.SI<Scalar>();
				row[ENGINE_DISPLACEMENT] = runData.EngineData.Displacement.ConvertTo().Cubic.Centi.Meter;
				row[CD_x_A] = runData.VehicleData.CrossWindCorrectionCurve.AirDragArea;
				row[ROLLING_RESISTANCE_COEFFICIENT] = runData.VehicleData.TotalRollResistanceCoefficient.SI<Scalar>();
				row[TRANSMISSION_TYPE] = runData.GearboxData.Type;
				row[GEAR_RATIO_FIRST_GEAR] = runData.GearboxData.Gears.Count > 0
					? runData.GearboxData.Gears.First().Value.Ratio.SI<Scalar>()
					: 0.SI<Scalar>();
				row[GEAR_RATIO_LAST_GEAR] = runData.GearboxData.Gears.Count > 0
					? runData.GearboxData.Gears.Last().Value.Ratio.SI<Scalar>()
					: 0.SI<Scalar>();
				row[AXLE_GEAR_RATIO] = runData.AxleGearData.AxleGear.Ratio.SI<Scalar>();
				row[R_DYN] = runData.VehicleData.DynamicTyreRadius;
				row[RETARDER_TYPE] = runData.Retarder.Type;
				vehicleLoading = runData.VehicleData.Loading;
				cargoVolume = runData.VehicleData.CargoVolume;
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
			row[FCFINAL_LITERPER100KM] = modData.FuelConsumptionFinalLiterPer100Kilometer();
			if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
				row[FCFINAL_LITERPER100TKM] = (modData.FuelConsumptionFinalLiterPer100Kilometer() ?? 0.SI()) /
											vehicleLoading.ConvertTo().Ton;
			}
			if (cargoVolume > 0) {
				row[FCFINAL_LiterPer100M3KM] = (modData.FuelConsumptionFinalLiterPer100Kilometer() ?? 0.SI()) / cargoVolume;
			}

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

			foreach (var aux in modData.Auxiliaries) {
				string colName;
				if (aux.Key == Constants.Auxiliaries.IDs.PTOConsumer || aux.Key == Constants.Auxiliaries.IDs.PTOTransmission) {
					colName = string.Format(E_FORMAT, aux.Key);
				} else {
					colName = string.Format(E_AUX_FORMAT, aux.Key);
				}

				if (!_table.Columns.Contains(colName)) {
					var col = _table.Columns.Add(colName, typeof(SI));
					// move the new column to correct position
					col.SetOrdinal(_table.Columns[E_AUX].Ordinal);
				}

				row[colName] = modData.AuxiliaryWork(aux.Value).ConvertTo().Kilo.Watt.Hour;
			}

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

			//var acc = modData.AccelerationPer3Seconds();


			row[ACC] = modData.AccelerationAverage();
			var modal = modData as ModalDataContainer;
			if (modal == null) {
				Log.Error("unknown modal data container!");
				return;
			}
			row[ACC_POS] = modal.AccelerationsPositive();
			row[ACC_NEG] = modal.AccelerationsNegative();
			var accTimeShare = modal.AccelerationTimeShare();
			row[ACC_TIMESHARE] = accTimeShare;
			var decTimeShare = modal.DecelerationTimeShare();
			row[DEC_TIMESHARE] = decTimeShare;
			var cruiseTimeShare = modal.CruiseTimeShare();
			row[CRUISE_TIMESHARE] = cruiseTimeShare;
			var stopTimeShare = modal.StopTimeShare();
			row[STOP_TIMESHARE] = stopTimeShare;

			row[MAX_SPEED] = modal.MaxSpeed().AsKmph.SI<Scalar>();
			row[MAX_ACCELERATION] = modal.MaxAcceleration();
			row[MAX_DECELERATION] = modal.MaxDeceleration();
			row[AVG_ENGINE_SPEED] = modal.AvgEngineSpeed().AsRPM.SI<Scalar>();
			row[MAX_ENGINE_SPEED] = modData.MaxEngineSpeed().AsRPM.SI<Scalar>();

			row[ENGINE_FULL_LOAD_TIME_SHARE] = modal.EngineMaxLoadTimeShare();
			row[COASTING_TIME_SHARE] = modal.CoastingTimeShare();
			row[BRAKING_TIME_SHARE] = modal.BrakingTimeShare();

			if (gearCount > 0) {
				row[NUM_GEARSHIFTS] = modal.GearshiftCount();
				var timeSharePerGear = modal.TimeSharePerGear(gearCount);

				for (uint i = 0; i <= gearCount; i++) {
					var colName = string.Format(TIME_SHARE_PER_GEAR_FORMAT, i);
					if (!_table.Columns.Contains(colName)) {
						_table.Columns.Add(colName, typeof(SI));
					}
					row[colName] = timeSharePerGear[i];
				}
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
				_table.Dispose();
			}
		}
	}
}