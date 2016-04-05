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

using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCore.Models;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public delegate void WriteSumData(IModalDataContainer data, Kilogram vehicleMass, Kilogram loading);

	/// <summary>
	/// Class for the sum file in vecto.
	/// </summary>
	public class SummaryDataContainer : LoggingObject
	{
		// ReSharper disable InconsistentNaming
		private const string JOB = "Job [-]";
		private const string INPUTFILE = "Input File [-]";
		private const string CYCLE = "Cycle [-]";
		private const string MASS = "Mass [kg]";
		private const string LOADING = "Loading [kg]";
		private const string STATUS = "Status";
		private const string TIME = "time [s]";
		private const string DISTANCE = "distance [km]";
		private const string SPEED = "speed [km/h]";
		private const string ALTITUDE_DELTA = "altitudeDelta [m]";

		private const string FCMAP = "FC-Map [g/h]";
		private const string FCMAPKM = "FC-Map [g/km]";
		private const string FCAUXC = "FC-AUXc [g/h]";
		private const string FCAUXCKM = "FC-AUXc [g/km]";
		private const string FCWHTCC = "FC-WHTCc [g/h]";
		private const string FCWHTCCKM = "FC-WHTCc [g/km]";
		private const string FCFINAL = "FC-Final [g/km]";
		private const string FCFINAL_LITERPER100KM = "FC-Final [l/100km]";
		private const string FCFINAL_LITERPER100TKM = "FC-Final [l/100tkm]";

		private const string CO2KM = "CO2 [g/km]";
		private const string CO2TKM = "CO2 [g/tkm]";

		private const string PPOS = "Ppos [kW]";
		private const string PNEG = "Pneg [kW]";
		private const string PWHEELPOS = "PwheelPos [kW]";
		private const string PBRAKE = "P_brake_loss [kW]";

		private const string EPOSICE = "EposICE [kWh]";
		private const string ENEGICE = "EnegICE [kWh]";
		private const string EAIR = "Eair [kWh]";
		private const string EROLL = "Eroll [kWh]";
		private const string EGRAD = "Egrad [kWh]";
		private const string EACC = "Eacc [kWh]";
		private const string EAUX = "Eaux [kWh]";
		private const string EBRAKE = "Ebrake [kWh]";
		private const string ETRANSM = "Etransm [kWh]";
		private const string ERETARDER = "Eretarder [kWh]";
		private const string ETORQUECONV = "Etorqueconv [kWh]";

		private const string ACCELERATIONS = "a [m/s^2]";
		private const string APOS = "a_pos [m/s^2]";
		private const string ANEG = "a_neg [m/s^2]";

		private const string PACC = "pAcc [%]";
		private const string PDEC = "pDec [%]";
		private const string PCRUISE = "pCruise [%]";
		private const string PSTOP = "pStop [%]";
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
				TIME, DISTANCE, SPEED, ALTITUDE_DELTA, PPOS, PNEG, FCMAP, FCMAPKM, FCAUXC, FCAUXCKM, FCWHTCC, FCWHTCCKM, PWHEELPOS,
				PBRAKE, EPOSICE, ENEGICE, EAIR, EROLL, EGRAD, EACC, EAUX, EBRAKE, ETRANSM, ERETARDER, MASS, LOADING, ACCELERATIONS,
				APOS,
				ANEG, PACC, PDEC, PCRUISE, PSTOP, ETORQUECONV, CO2KM, CO2TKM, FCFINAL, FCFINAL_LITERPER100KM, FCFINAL_LITERPER100TKM
			}.Select(x => new DataColumn(x, typeof(SI))).ToArray());
		}

		public virtual void Write(bool isEngineOnly, IModalDataContainer data, string jobFileName, string jobName,
			string cycleFileName, Kilogram vehicleMass, Kilogram vehicleLoading)
		{
			var row = _table.NewRow();
			_table.Rows.Add(row);

			row[JOB] = ReplaceNotAllowedCharacters(jobName);
			row[INPUTFILE] = ReplaceNotAllowedCharacters(jobFileName);
			row[CYCLE] = ReplaceNotAllowedCharacters(cycleFileName);
			row[STATUS] = data.RunStatus;
			row[TIME] = data.Duration();

			if (isEngineOnly) {
				row[PPOS] = data.EnginePowerPositiveAverage().ConvertTo().Kilo.Watt;
				row[PNEG] = data.EnginePowerNegativeAverage().ConvertTo().Kilo.Watt;
				row[FCMAP] = data.FuelConsumptionPerSecond().ConvertTo().Gramm.Per.Hour;
				row[FCAUXC] = data.FuelConsumptionAuxStartStopCorrectedPerSecond().ConvertTo().Gramm.Per.Hour;
				row[FCWHTCC] = data.FuelConsumptionWHTCCorrectedPerSecond().ConvertTo().Gramm.Per.Hour;
				WriteAuxiliaries(data, row);
			} else {
				var distance = data.Distance();
				if (distance != null) {
					row[DISTANCE] = distance.ConvertTo().Kilo.Meter;
				}

				var speed = data.Speed();
				if (speed != null) {
					row[SPEED] = speed.ConvertTo().Kilo.Meter.Per.Hour;
				}

				row[ALTITUDE_DELTA] = data.AltitudeDelta();
				row[PPOS] = data.EnginePowerPositiveAverage().ConvertTo().Kilo.Watt;
				row[PNEG] = data.EnginePowerNegativeAverage().ConvertTo().Kilo.Watt;

				var fcfinal = data.FuelConsumptionFinal();
				if (fcfinal != null) {
					row[FCFINAL] = fcfinal.ConvertTo().Gramm.Per.Kilo.Meter;
				}
				row[FCFINAL_LITERPER100KM] = data.FuelConsumptionFinalLiterPer100Kilometer();
				if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
					row[FCFINAL_LITERPER100TKM] = data.FuelConsumptionFinalLiterPer100Kilometer() / vehicleLoading.ConvertTo().Ton;
				}
				row[FCMAP] = data.FuelConsumptionPerSecond().ConvertTo().Gramm.Per.Hour;

				var fuelConsumptionPerMeter = data.FuelConsumptionPerMeter();
				if (fuelConsumptionPerMeter != null) {
					row[FCMAPKM] = fuelConsumptionPerMeter.ConvertTo().Gramm.Per.Kilo.Meter;
				}
				row[FCAUXC] = data.FuelConsumptionAuxStartStopCorrectedPerSecond().ConvertTo().Gramm.Per.Hour;
				var fuelConsumptionAuxStartStopCorrected = data.FuelConsumptionAuxStartStopCorrected();
				if (fuelConsumptionAuxStartStopCorrected != null) {
					row[FCAUXCKM] = fuelConsumptionAuxStartStopCorrected.ConvertTo().Gramm.Per.Kilo.Meter;
				}
				row[FCWHTCC] = data.FuelConsumptionWHTCCorrectedPerSecond().ConvertTo().Gramm.Per.Hour;
				var fuelConsumptionWHTCCorrected = data.FuelConsumptionWHTCCorrected();
				if (fuelConsumptionWHTCCorrected != null) {
					row[FCWHTCCKM] = fuelConsumptionWHTCCorrected.ConvertTo().Gramm.Per.Kilo.Meter;
				}

				var kilogramPerMeter = data.CO2PerMeter();
				if (kilogramPerMeter != null) {
					row[CO2KM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter;
					if (vehicleLoading != null && !vehicleLoading.IsEqual(0)) {
						row[CO2TKM] = kilogramPerMeter.ConvertTo().Gramm.Per.Kilo.Meter / vehicleLoading.ConvertTo().Ton;
					}
				}

				row[PWHEELPOS] = data.PowerWheelPositive().ConvertTo().Kilo.Watt;
				row[PBRAKE] = data.PowerBrake().ConvertTo().Kilo.Watt;
				row[EPOSICE] = data.EngineWorkPositive().ConvertTo().Kilo.Watt.Hour;
				row[ENEGICE] = data.EngineWorkNegative().ConvertTo().Kilo.Watt.Hour;
				row[EAIR] = data.WorkAirResistance().ConvertTo().Kilo.Watt.Hour;
				row[EROLL] = data.WorkRollingResistance().ConvertTo().Kilo.Watt.Hour;
				row[EGRAD] = data.WorkRoadGradientResistance().ConvertTo().Kilo.Watt.Hour;
				row[EACC] = data.PowerAccelerations().ConvertTo().Kilo.Watt.Hour;
				row[EAUX] = data.WorkAuxiliaries().ConvertTo().Kilo.Watt.Hour;
				WriteAuxiliaries(data, row);
				row[EBRAKE] = data.WorkTotalMechanicalBrake().ConvertTo().Kilo.Watt.Hour;
				row[ETRANSM] = data.WorkTransmission().ConvertTo().Kilo.Watt.Hour;
				row[ERETARDER] = data.WorkRetarder().ConvertTo().Kilo.Watt.Hour;
				row[ETORQUECONV] = data.WorkTorqueConverter().ConvertTo().Kilo.Watt.Hour;
				row[MASS] = vehicleMass;
				row[LOADING] = vehicleLoading;
				row[ACCELERATIONS] = data.AccelerationAverage();
				row[APOS] = data.AccelerationsPositive3SecondAverage();
				row[ANEG] = data.AverageAccelerations3SecondNegative();
				row[PACC] = data.PercentAccelerationTime();
				row[PDEC] = data.PercentDecelerationTime();
				row[PCRUISE] = data.PercentCruiseTime();
				row[PSTOP] = data.PercentStopTime();
			}
		}

		private static string ReplaceNotAllowedCharacters(string text)
		{
			return text.Replace('#', '_').Replace(',', '_').Replace('\n', '_').Replace('\r', '_');
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void WriteAuxiliaries(IModalDataContainer data, DataRow row)
		{
			foreach (var aux in data.Auxiliaries) {
				var colName = "Eaux_" + aux.Key + " [kWh]";
				if (!_table.Columns.Contains(colName)) {
					_table.Columns.Add(colName, typeof(SI));
				}

				row[colName] = data.AuxiliaryWork(aux.Value).ConvertTo().Kilo.Watt.Hour;
			}
		}

		public virtual void Finish()
		{
			if (_sumWriter != null) {
				_sumWriter.WriteSumData(new DataView(_table, "", JOB, DataViewRowState.CurrentRows).ToTable());
			}
		}
	}
}