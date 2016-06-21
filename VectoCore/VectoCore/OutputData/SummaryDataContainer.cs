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

namespace TUGraz.VectoCore.OutputData
{
	public delegate void WriteSumData(IModalDataContainer data, Kilogram vehicleMass, Kilogram loading);

	/// <summary>
	/// Class for the sum file in vecto.
	/// </summary>
	public class SummaryDataContainer : LoggingObject, IDisposable
	{
		// ReSharper disable InconsistentNaming
		private const string JOB = "Job [-]";
		private const string INPUTFILE = "Input File [-]";
		private const string CYCLE = "Cycle [-]";
		private const string STATUS = "Status";
		private const string MASS = "Mass [kg]";
		private const string LOADING = "Loading [kg]";
		private const string TIME = "time [s]";
		private const string DISTANCE = "distance [km]";
		private const string SPEED = "speed [km/h]";
		private const string ALTITUDE_DELTA = "altitudeDelta [m]";

		private const string FCMAP_H = "FC-Map [g/h]";
		private const string FCMAP_KM = "FC-Map [g/km]";
		private const string FCAUXC_H = "FC-AUXc [g/h]";
		private const string FCAUXC_KM = "FC-AUXc [g/km]";
		private const string FCWHTCC_H = "FC-WHTCc [g/h]";
		private const string FCWHTCC_KM = "FC-WHTCc [g/km]";
		private const string FCAAUX_H = "FC-AAUX [g/h]";
		private const string FCAAUX_KM = "FC-AAUX [g/km]";

		private const string FCFINAL_H = "FC-Final [g/h]";
		private const string FCFINAL_KM = "FC-Final [g/km]";
		private const string FCFINAL_LITERPER100KM = "FC-Final [l/100km]";
		private const string FCFINAL_LITERPER100TKM = "FC-Final [l/100tkm]";

		private const string CO2_KM = "CO2 [g/km]";
		private const string CO2_TKM = "CO2 [g/tkm]";

		private const string P_WHEEL_POS = "P_wheel_in_pos [kW]";
		private const string P_BRAKE_LOSS = "P_brake_loss [kW]";
		private const string P_ENG_POS = "P_eng_out_pos [kW]";
		private const string P_ENG_NEG = "P_eng_out_neg [kW]";

		private const string E_AUX_FORMAT = "E_aux_{0} [kWh]";
		private const string E_AUX = "E_aux_sum [kWh]";

		private const string E_AIR = "E_air [kWh]";
		private const string E_ROLL = "E_roll [kWh]";
		private const string E_GRAD = "E_grad [kWh]";
		private const string E_INERTIA = "E_inertia [kWh]";
		private const string E_BRAKE = "E_brake [kWh]";
		private const string E_GBX_AXL_LOSS = "E_gbx_axl_loss [kWh]";
		private const string E_RET_LOSS = "E_ret_loss [kWh]";
		private const string E_TC_LOSS = "E_tc_loss [kWh]";
		private const string E_ENG_POS = "E_eng_out_pos [kWh]";
		private const string E_ENG_NEG = "E_eng_out_neg [kWh]";

		private const string ACC = "a [m/s^2]";
		private const string ACC_POS = "a_pos [m/s^2]";
		private const string ACC_NEG = "a_neg [m/s^2]";

		private const string ACC_TIMESHARE = "AccelerationTimeShare [%]";
		private const string DEC_TIMESHARE = "DecelerationTimeShare [%]";
		private const string CRUISE_TIMESHARE = "CruiseTimeShare [%]";
		private const string STOP_TIMESHARE = "StopTimeShare [%]";
		// ReSharper restore InconsistentNaming

		private readonly DataTable _table;
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
				MASS, LOADING, TIME, DISTANCE, SPEED, ALTITUDE_DELTA, FCMAP_H, FCMAP_KM, FCAUXC_H, FCAUXC_KM, FCWHTCC_H, FCWHTCC_KM,
				FCAAUX_H, FCAAUX_KM, FCFINAL_H, FCFINAL_KM, FCFINAL_LITERPER100KM, FCFINAL_LITERPER100TKM, CO2_KM, CO2_TKM,
				P_WHEEL_POS, P_BRAKE_LOSS, P_ENG_POS, P_ENG_NEG, E_AUX, E_AIR, E_ROLL, E_GRAD, E_INERTIA, E_BRAKE, E_GBX_AXL_LOSS,
				E_RET_LOSS, E_TC_LOSS, E_ENG_POS, E_ENG_NEG, ACC, ACC_POS, ACC_NEG, ACC_TIMESHARE, DEC_TIMESHARE, CRUISE_TIMESHARE,
				STOP_TIMESHARE
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
			Kilogram vehicleMass, Kilogram vehicleLoading)
		{
			var row = _table.NewRow();
			_table.Rows.Add(row);

			row[JOB] = ReplaceNotAllowedCharacters(jobName);
			row[INPUTFILE] = ReplaceNotAllowedCharacters(jobFileName);
			row[CYCLE] = ReplaceNotAllowedCharacters(cycleFileName);
			row[STATUS] = modData.RunStatus;

			row[MASS] = vehicleMass;
			row[LOADING] = vehicleLoading;

			row[TIME] = modData.Duration();

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
			var fuelConsumptionAAUX = modData.FuelConsumptionAAUX();
			if (fuelConsumptionAAUX != null) {
				row[FCAAUX_KM] = fuelConsumptionAAUX.ConvertTo().Gramm.Per.Kilo.Meter;
			}

			row[FCFINAL_H] = modData.FuelConsumptionFinalPerSecond().ConvertTo().Gramm.Per.Hour;
			var fcfinal = modData.FuelConsumptionFinal();
			if (fcfinal != null) {
				row[FCFINAL_KM] = fcfinal.ConvertTo().Gramm.Per.Kilo.Meter;
			}
			row[FCFINAL_LITERPER100KM] = modData.FuelConsumptionFinalLiterPer100Kilometer();
			if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
				row[FCFINAL_LITERPER100TKM] = modData.FuelConsumptionFinalLiterPer100Kilometer() / vehicleLoading.ConvertTo().Ton;
			}

			var kilogramPerMeter = modData.CO2PerMeter();
			if (kilogramPerMeter != null) {
				row[CO2_KM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter;
				if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
					row[CO2_TKM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter / vehicleLoading.ConvertTo().Ton;
				}
			}

			row[P_WHEEL_POS] = modData.PowerWheelPositive().ConvertTo().Kilo.Watt;
			row[P_BRAKE_LOSS] = modData.PowerBrake().ConvertTo().Kilo.Watt;
			row[P_ENG_POS] = modData.EnginePowerPositiveAverage().ConvertTo().Kilo.Watt;
			row[P_ENG_NEG] = modData.EnginePowerNegativeAverage().ConvertTo().Kilo.Watt;

			foreach (var aux in modData.Auxiliaries) {
				var colName = string.Format(E_AUX_FORMAT, aux.Key);
				if (!_table.Columns.Contains(colName)) {
					var col = _table.Columns.Add(colName, typeof(SI));
					// move the new column to correct position
					col.SetOrdinal(_table.Columns[E_AUX].Ordinal);
				}
				row[colName] = modData.AuxiliaryWork(aux.Value).ConvertTo().Kilo.Watt.Hour;
			}
			row[E_AUX] = modData.WorkAuxiliaries().ConvertTo().Kilo.Watt.Hour;

			row[E_AIR] = modData.WorkAirResistance().ConvertTo().Kilo.Watt.Hour;
			row[E_ROLL] = modData.WorkRollingResistance().ConvertTo().Kilo.Watt.Hour;
			row[E_GRAD] = modData.WorkRoadGradientResistance().ConvertTo().Kilo.Watt.Hour;
			row[E_INERTIA] = modData.PowerAccelerations().ConvertTo().Kilo.Watt.Hour;
			row[E_BRAKE] = modData.WorkTotalMechanicalBrake().ConvertTo().Kilo.Watt.Hour;
			row[E_GBX_AXL_LOSS] = modData.WorkTransmission().ConvertTo().Kilo.Watt.Hour;
			row[E_RET_LOSS] = modData.WorkRetarder().ConvertTo().Kilo.Watt.Hour;
			row[E_TC_LOSS] = modData.WorkTorqueConverter().ConvertTo().Kilo.Watt.Hour;
			row[E_ENG_POS] = modData.EngineWorkPositive().ConvertTo().Kilo.Watt.Hour;
			row[E_ENG_NEG] = modData.EngineWorkNegative().ConvertTo().Kilo.Watt.Hour;

			row[ACC] = modData.AccelerationAverage();
			row[ACC_POS] = modData.AccelerationsPositive();
			row[ACC_NEG] = modData.AccelerationsNegative();
			row[ACC_TIMESHARE] = modData.AccelerationTimeShare();
			row[DEC_TIMESHARE] = modData.DecelerationTimeShare();
			row[CRUISE_TIMESHARE] = modData.CruiseTimeShare();
			row[STOP_TIMESHARE] = modData.StopTimeShare();
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

		protected virtual void Dispose(bool disposing)
		{
			if (disposing) {
				_table.Dispose();
			}
		}
	}
}