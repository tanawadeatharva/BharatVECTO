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
using System.Data;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public class ModalDataContainer : IModalDataContainer
	{
		private readonly bool _writeEngineOnly;
		private readonly IModalDataFilter[] _filters;
		private readonly Action<ModalDataContainer> _addReportResult;
		internal ModalResults Data { get; set; }
		private DataRow CurrentRow { get; set; }
		//private readonly VectoRunData _runData;

		private readonly IModalDataWriter _writer;
		private readonly List<string> _additionalColumns = new List<string>();
		public string RunName { get; private set; }
		public string CycleName { get; private set; }
		public string RunSuffix { get; private set; }

		public bool WriteModalResults { get; set; }

		public VectoRun.Status RunStatus { get; protected set; }

		public bool WriteAdvancedAux { get; set; }

		public ModalDataContainer(string runName, IModalDataWriter writer, bool writeEngineOnly = false)
			: this(runName, "", "", writer, _ => { }, writeEngineOnly) {}

		public ModalDataContainer(VectoRunData runData, IModalDataWriter writer, Action<ModalDataContainer> addReportResult,
			bool writeEngineOnly, params IModalDataFilter[] filter)
			: this(runData.JobName, runData.Cycle.Name, runData.ModFileSuffix, writer, addReportResult, writeEngineOnly, filter) {}

		protected ModalDataContainer(string runName, string cycleName, string runSuffix, IModalDataWriter writer,
			Action<ModalDataContainer> addReportResult, bool writeEngineOnly, params IModalDataFilter[] filters)

		{
			HasTorqueConverter = false;
			RunName = runName;
			CycleName = cycleName;
			RunSuffix = runSuffix;
			_writer = writer;

			_writeEngineOnly = writeEngineOnly;
			_filters = filters;
			_addReportResult = addReportResult ?? (x => { });

			Data = new ModalResults();
			Auxiliaries = new Dictionary<string, DataColumn>();
			CurrentRow = Data.NewRow();
			WriteAdvancedAux = false;
		}

		public bool HasTorqueConverter { get; set; }

		public void CommitSimulationStep()
		{
			Data.Rows.Add(CurrentRow);
			CurrentRow = Data.NewRow();
		}

		public void Finish(VectoRun.Status runStatus)
		{
			var dataColumns = new List<ModalResultField> { ModalResultField.time };

			RunStatus = runStatus;

			if (!_writeEngineOnly) {
				dataColumns.AddRange(new[] {
					ModalResultField.simulationInterval,
					ModalResultField.dist,
					ModalResultField.v_act,
					ModalResultField.v_targ,
					ModalResultField.acc,
					ModalResultField.grad
				});
			}
			if (!_writeEngineOnly) {
				dataColumns.AddRange(new[] {
					ModalResultField.Gear,
				});
			}
			dataColumns.AddRange(new[] {
				ModalResultField.n_eng_avg,
				ModalResultField.T_eng_fcmap,
				ModalResultField.Tq_full,
				ModalResultField.Tq_drag,
				ModalResultField.P_eng_fcmap,
				ModalResultField.P_eng_full,
				ModalResultField.P_eng_drag,
				ModalResultField.P_eng_inertia,
				ModalResultField.P_eng_out,
				ModalResultField.P_clutch_loss,
				ModalResultField.P_clutch_out,
				ModalResultField.P_aux
			});

			if (!_writeEngineOnly) {
				dataColumns.AddRange(new[] {
					ModalResultField.P_gbx_in,
					ModalResultField.P_gbx_loss,
					ModalResultField.P_gbx_inertia,
					ModalResultField.P_retarder_in,
					ModalResultField.P_ret_loss,
					ModalResultField.P_angle_in,
					ModalResultField.P_angle_loss,
					ModalResultField.P_axle_in,
					ModalResultField.P_axle_loss,
					ModalResultField.P_brake_in,
					ModalResultField.P_brake_loss,
					ModalResultField.P_wheel_in,
					ModalResultField.P_wheel_inertia,
					ModalResultField.P_trac,
					ModalResultField.P_slope,
					ModalResultField.P_air,
					ModalResultField.P_roll,
					ModalResultField.P_veh_inertia,
				});

				if (HasTorqueConverter) {
					dataColumns.AddRange(new[] {
						ModalResultField.TCv,
						ModalResultField.P_tc_loss,
						ModalResultField.TCmu,
						ModalResultField.TC_M_Out,
						ModalResultField.TC_n_Out
					});
				}
			}
			if (!_writeEngineOnly && WriteAdvancedAux) {
				dataColumns.AddRange(new[] {
					ModalResultField.AA_NonSmartAlternatorsEfficiency,
					ModalResultField.AA_SmartIdleCurrent_Amps,
					ModalResultField.AA_SmartIdleAlternatorsEfficiency,
					ModalResultField.AA_SmartTractionCurrent_Amps,
					ModalResultField.AA_SmartTractionAlternatorEfficiency,
					ModalResultField.AA_SmartOverrunCurrent_Amps,
					ModalResultField.AA_SmartOverrunAlternatorEfficiency,
					ModalResultField.AA_CompressorFlowRate_LitrePerSec,
					ModalResultField.AA_OverrunFlag,
					ModalResultField.AA_EngineIdleFlag,
					ModalResultField.AA_CompressorFlag,
					ModalResultField.AA_TotalCycleFC_Grams,
					ModalResultField.AA_TotalCycleFC_Litres,
					ModalResultField.AA_AveragePowerDemandCrankHVACMechanicals,
					ModalResultField.AA_AveragePowerDemandCrankHVACElectricals,
					ModalResultField.AA_AveragePowerDemandCrankElectrics,
					ModalResultField.AA_AveragePowerDemandCrankPneumatics,
					ModalResultField.AA_TotalCycleFuelConsumptionCompressorOff,
					ModalResultField.AA_TotalCycleFuelConsumptionCompressorOn,
				});
			}

			var strCols = dataColumns.Select(x => x.GetName())
				.Concat(Auxiliaries.Values.Select(c => c.ColumnName))
				.Concat(
					new[] {
						ModalResultField.FCMap, ModalResultField.FCAUXc, ModalResultField.FCWHTCc,
						ModalResultField.FCAAUX, ModalResultField.FCFinal
					}.Select(x => x.GetName()));

			if (WriteModalResults) {
				var filteredData = Data;
				foreach (var filter in _filters) {
					RunSuffix += "_" + filter.ID;
					filteredData = filter.Filter(filteredData);
				}
				_writer.WriteModData(RunName, CycleName, RunSuffix, new DataView(filteredData).ToTable(false, strCols.ToArray()));
			}

			_addReportResult(this);
		}

		public IEnumerable<T> GetValues<T>(DataColumn col)
		{
			return Data.Rows.Cast<DataRow>().Select(x => x.Field<T>(col));
		}

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return GetValues<T>(Data.Columns[(int)key]);
		}

		public object this[ModalResultField key]
		{
			get { return CurrentRow[(int)key]; }
			set { CurrentRow[(int)key] = value; }
		}

		public object this[string auxId]
		{
			get { return CurrentRow[Auxiliaries[auxId]]; }
			set { CurrentRow[Auxiliaries[auxId]] = value; }
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void SetDataValue(string fieldName, object value)
		{
			if (!Data.Columns.Contains(fieldName)) {
				_additionalColumns.Add(fieldName);
				Data.Columns.Add(fieldName);
			}
			if (value is double) {
				CurrentRow[fieldName] = string.Format(CultureInfo.InvariantCulture, "{0}", value);
			} else {
				CurrentRow[fieldName] = value;
			}
		}

		public Dictionary<string, DataColumn> Auxiliaries { get; set; }

		public void AddAuxiliary(string id)
		{
			if (!string.IsNullOrWhiteSpace(id)) {
				if (!Auxiliaries.ContainsKey(id)) {
					var col = Data.Columns.Add(ModalResultField.P_aux_ + id, typeof(SI));
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
						ModalResultField.P_aux_.GetAttribute().Decimals;
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
						ModalResultField.P_aux_.GetAttribute().OutputFactor;
					col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
						ModalResultField.P_aux_.GetAttribute().ShowUnit;

					Auxiliaries[id] = col;
				}
			}
		}

		public class ModalData1HzFilter : IModalDataFilter
		{
			public ModalResults Filter(ModalResults data)
			{
				var absTime = 0.SI<Second>();
				var distance = 0.SI<Meter>();
				var results = (ModalResults)data.Clone();

				var remainingDt = 0.SI<Second>();

				object[] remainingRow = null;
				var gearsList = new Dictionary<object, Second>(3);
				var v_act = data.Rows.Cast<DataRow>().First().Field<MeterPerSecond>((int)ModalResultField.v_act);

				foreach (DataRow row in data.Rows) {
					var currentDt = row.Field<Second>((int)ModalResultField.simulationInterval);
					distance = row.Field<Meter>((int)ModalResultField.dist);

					// if current + remaining time >= 1 second: take remaining row and split up currentRow to fill up 1 second.
					if (remainingDt > 0 && remainingDt + currentDt >= 1) {
						var diffDt = 1.SI<Second>() - remainingDt;
						var r = results.NewRow();

						var gear = row[(int)ModalResultField.Gear];
						gearsList[gear] = gearsList.GetValueOrZero(gear) + diffDt;

						distance += diffDt * v_act + diffDt * diffDt * (MeterPerSquareSecond)row[(int)ModalResultField.acc] / 2;
						v_act += diffDt * (MeterPerSquareSecond)row[(int)ModalResultField.acc];
						r.ItemArray = AddRow(remainingRow, MultiplyRow(row.ItemArray, diffDt));
						absTime += diffDt;

						r[(int)ModalResultField.time] = absTime;
						r[(int)ModalResultField.simulationInterval] = 1.SI<Second>();
						r[(int)ModalResultField.Gear] = gearsList.MaxBy(kv => kv.Value).Key;
						r[(int)ModalResultField.dist] = distance;
						r[(int)ModalResultField.v_act] = v_act;

						gearsList.Clear();
						results.Rows.Add(r);
						currentDt -= diffDt;
						remainingDt = 0.SI<Second>();
						remainingRow = null;
					}

					// if current row still longer than 1 second: split it to 1 second slices until it is < 1 second
					while (currentDt >= 1) {
						currentDt = currentDt - 1.SI<Second>();
						var dt = 1.SI<Second>();
						var r = results.NewRow();
						r.ItemArray = row.ItemArray;
						absTime += dt;
						distance += dt * v_act + dt * dt * (MeterPerSquareSecond)row[(int)ModalResultField.acc] / 2;
						v_act += dt * (MeterPerSquareSecond)row[(int)ModalResultField.acc];

						r[(int)ModalResultField.time] = absTime;
						r[(int)ModalResultField.simulationInterval] = dt;
						r[(int)ModalResultField.dist] = distance;
						r[(int)ModalResultField.v_act] = v_act;
						results.Rows.Add(r);
					}

					// if the there still is something left in current row: add the weighted values to remainder-buffer
					if (currentDt > 0) {
						var gear = row[(int)ModalResultField.Gear];
						gearsList[gear] = gearsList.GetValueOrZero(gear) + currentDt;

						distance += currentDt * v_act + currentDt * currentDt * (MeterPerSquareSecond)row[(int)ModalResultField.acc] / 2;
						v_act += currentDt * (MeterPerSquareSecond)row[(int)ModalResultField.acc];
						remainingRow = AddRow(remainingRow, MultiplyRow(row.ItemArray, currentDt));
						remainingDt += currentDt;
						absTime += currentDt;
					} else {
						remainingRow = null;
						remainingDt = 0.SI<Second>();
						gearsList.Clear();
					}
				}

				// if last row was not enough to full second: take last row as whole second

				if (remainingDt > 0) {
					var last = data.Rows.Cast<DataRow>().Last();
					var r = results.NewRow();

					r.ItemArray = MultiplyRow(remainingRow, 1 / remainingDt).ToArray();
					distance += remainingDt * v_act +
								remainingDt * remainingDt * (MeterPerSquareSecond)last[(int)ModalResultField.acc] / 2;
					v_act += remainingDt * (MeterPerSquareSecond)last[(int)ModalResultField.acc];

					r[(int)ModalResultField.time] = VectoMath.Ceiling(absTime);
					r[(int)ModalResultField.simulationInterval] = 1.SI<Second>();
					r[(int)ModalResultField.Gear] = gearsList.MaxBy(kv => kv.Value).Key;
					r[(int)ModalResultField.dist] = distance;
					r[(int)ModalResultField.v_act] = v_act;
					results.Rows.Add(r);
				}

				return results;
			}

			private static IEnumerable<object> MultiplyRow(IEnumerable<object> row, SI dt)
			{
				return row.Select(val => {
					if (val is SI) {
						val = (SI)val * dt.Value();
					} else {
						val.Switch()
							.Case<int>(i => val = i * dt.Value())
							.Case<double>(d => val = d * dt.Value())
							.Case<float>(f => val = f * dt.Value())
							.Case<uint>(ui => val = ui * dt.Value());
					}
					return val;
				});
			}

			private static object[] AddRow(IEnumerable<object> row, IEnumerable<object> addRow)
			{
				if (row == null) {
					return addRow.ToArray();
				}
				if (addRow == null) {
					return row.ToArray();
				}

				return row.ZipAll(addRow, (val, addVal) => {
					if (val is SI || addVal is SI) {
						if (DBNull.Value == val) {
							val = addVal;
						} else if (DBNull.Value != addVal) {
							val = (SI)val + (SI)addVal;
						}
					} else {
						val.Switch()
							.Case<int>(i => val = i + (int)addVal)
							.Case<double>(d => val = d + (double)addVal)
							.Case<float>(f => val = f + (float)addVal)
							.Case<uint>(ui => val = ui + (uint)addVal);
					}
					return val;
				}).ToArray();
			}

			public string ID
			{
				get { return "1Hz"; }
			}
		}
	}
}