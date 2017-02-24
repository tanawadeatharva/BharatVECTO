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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;

// ReSharper disable MemberCanBePrivate.Global  -- used by API!

namespace TUGraz.VectoCore.OutputData
{
	public delegate void WriteSumData(
		IModalDataContainer data, Kilogram vehicleMass, Kilogram loading, CubicMeter cargoVolume);

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
		public const string MASS = "Mass [kg]";
		public const string LOADING = "Loading [kg]";
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

			_table.Columns.AddRange(new[] {
				MASS, LOADING, VOLUME, TIME, DISTANCE, SPEED, ALTITUDE_DELTA, FCMAP_H, FCMAP_KM, FCAUXC_H, FCAUXC_KM, FCWHTCC_H,
				FCWHTCC_KM,
				FCAAUX_H, FCAAUX_KM, FCFINAL_H, FCFINAL_KM, FCFINAL_LITERPER100KM, FCFINAL_LITERPER100TKM, FCFINAL_LiterPer100M3KM,
				CO2_KM, CO2_TKM, CO2_M3KM,
				P_WHEEL_POS, P_FCMAP_POS,
				E_FCMAP_POS, E_FCMAP_NEG, E_POWERTRAIN_INERTIA, E_AUX, E_CLUTCH_LOSS, E_TC_LOSS, E_SHIFT_LOSS, E_GBX_LOSS,
				E_RET_LOSS, E_ANGLE_LOSS, E_AXL_LOSS, E_BRAKE, E_VEHICLE_INERTIA, E_AIR, E_ROLL, E_GRAD,
				ACC, ACC_POS, ACC_NEG, ACC_TIMESHARE, DEC_TIMESHARE, CRUISE_TIMESHARE, STOP_TIMESHARE
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
		public virtual void Write(IModalDataContainer modData, string jobFileName, string jobName, string cycleFileName,
			Kilogram vehicleMass, Kilogram vehicleLoading, CubicMeter cargoVolume)
		{
			var row = _table.NewRow();
			_table.Rows.Add(row);

			row[JOB] = ReplaceNotAllowedCharacters(jobName);
			row[INPUTFILE] = ReplaceNotAllowedCharacters(jobFileName);
			row[CYCLE] = ReplaceNotAllowedCharacters(cycleFileName);
			row[STATUS] = modData.RunStatus;

			row[MASS] = vehicleMass;
			row[LOADING] = vehicleLoading;
			row[VOLUME] = cargoVolume;

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
			//row[P_BRAKE_LOSS] = modData.PowerBrake().ConvertTo().Kilo.Watt;
			//row[P_ANGLE_LOSS] = modData.PowerAngle().ConvertTo().Kilo.Watt;
			//row[P_TC_LOSS] = modData.PowerTorqueConverter().ConvertTo().Kilo.Watt;
			//row[P_CLUTCH_POS] = modData.EnginePowerPositiveAverage().ConvertTo().Kilo.Watt;
			//row[P_CLUTCH_NEG] = modData.EnginePowerNegativeAverage().ConvertTo().Kilo.Watt;

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

			//row[E_CLUTCH_POS] = modData.EngineWorkPositive().ConvertTo().Kilo.Watt.Hour;
			//row[E_CLUTCH_NEG] = modData.EngineWorkNegative().ConvertTo().Kilo.Watt.Hour;
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