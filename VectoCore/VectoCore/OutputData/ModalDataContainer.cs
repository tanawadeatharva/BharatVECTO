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
using TUGraz.VectoCommon.Exceptions;
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
			_filters = filters ?? new IModalDataFilter[0];
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
				if (HasTorqueConverter) {
					dataColumns.AddRange(new[] { ModalResultField.TC_Locked });
				}
			}
			dataColumns.AddRange(new[] {
				ModalResultField.n_eng_avg,
				ModalResultField.T_eng_fcmap,
				ModalResultField.Tq_full,
				ModalResultField.Tq_drag,
				ModalResultField.P_eng_fcmap,
				ModalResultField.P_eng_full,
				ModalResultField.P_eng_full_stat,
				ModalResultField.P_eng_drag,
				ModalResultField.P_eng_inertia,
				ModalResultField.P_eng_out,
			});
			if (HasTorqueConverter) {
				dataColumns.AddRange(new[] {
					ModalResultField.P_TC_loss,
					ModalResultField.P_TC_out,
				});
			} else {
				dataColumns.AddRange(new[] {
					ModalResultField.P_clutch_loss,
					ModalResultField.P_clutch_out,
				});
			}
			dataColumns.AddRange(new[] {
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
						ModalResultField.TorqueConverterSpeedRatio,
						ModalResultField.TorqueConverterTorqueRatio,
						ModalResultField.TC_TorqueOut,
						ModalResultField.TC_angularSpeedOut,
						ModalResultField.TC_TorqueIn,
						ModalResultField.TC_angularSpeedIn,
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
#if TRACE
			strCols = strCols.Concat(_additionalColumns);
#endif
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

		public T TimeIntegral<T>(ModalResultField field, Func<SI, bool> filter = null)
			where T : SIBase<T>
		{
			var result = 0.0;
			for (var i = 0; i < Data.Rows.Count; i++) {
				var value = Data.Rows[i][(int)field];
				if (value != null && value != DBNull.Value) {
					var siValue = (SI)value;
					if (filter == null || filter(siValue)) {
						result += siValue.Value() * ((Second)Data.Rows[i][(int)ModalResultField.simulationInterval]).Value();
					}
				}
			}

			return result.SI<T>();
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

		/// <summary>
		/// Adds a new auxiliary column into the mod data.
		/// </summary>
		/// <param name="id">The Aux-ID. This is the internal identification for the auxiliary.</param>
		/// <param name="columnName">(Optional) The column name in the mod file. Default: "P_aux_" + id</param>
		public void AddAuxiliary(string id, string columnName = null)
		{
			if (!string.IsNullOrWhiteSpace(id) && !Auxiliaries.ContainsKey(id)) {
				var col = Data.Columns.Add(columnName ?? ModalResultField.P_aux_ + id, typeof(SI));
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.Decimals] =
					ModalResultField.P_aux_.GetAttribute().Decimals;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.OutputFactor] =
					ModalResultField.P_aux_.GetAttribute().OutputFactor;
				col.ExtendedProperties[ModalResults.ExtendedPropertyNames.ShowUnit] =
					ModalResultField.P_aux_.GetAttribute().ShowUnit;

				Auxiliaries[id] = col;
			}
		}

		public class ModalData1HzFilter : IModalDataFilter
		{
			public ModalResults Filter(ModalResults data)
			{
				var results = (ModalResults)data.Clone();

				object[] remainingRow = null;
				var gearsList = new Dictionary<object, Second>(3);

				var absTime = 0.SI<Second>();
				var distance = 0.SI<Meter>();
				var v = data.Rows[0].Field<MeterPerSecond>((int)ModalResultField.v_act);
				var remainingDt = 0.SI<Second>();
				var vPrevious = v;

				for (var i = 0; i < data.Rows.Count; i++) {
					var row = data.Rows[i];

					var currentDt = row.Field<Second>((int)ModalResultField.simulationInterval);

					if (row.Field<Meter>((int)ModalResultField.dist).IsSmaller(distance, 1e-3)) {
						LogManager.GetLogger(typeof(ModalData1HzFilter).FullName).Error("1Hz-Filter: distance must always be increasing.");
					}

					// if a remainder and currentDt would exceed 1 second: take remainder and take current row to fill up to 1 second.
					if (remainingDt > 0 && remainingDt + currentDt >= 1) {
						// calculate values
						var dt = 1.SI<Second>() - remainingDt;
						var gear = row[(int)ModalResultField.Gear];
						gearsList[gear] = gearsList.GetValueOrZero(gear) + dt;
						var a = (MeterPerSquareSecond)row[(int)ModalResultField.acc];
						var ds = dt * v + a / 2 * dt * dt;
						if (ds.IsSmaller(0)) {
							throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative.");
						}
						absTime += dt;
						v += dt * a;
						distance += ds;

						// write a new row for the combined 1 second
						var r = results.NewRow();
						r.ItemArray = AddRow(remainingRow, MultiplyRow(row.ItemArray, dt));
						r[(int)ModalResultField.time] = absTime;
						r[(int)ModalResultField.simulationInterval] = 1.SI<Second>();
						r[(int)ModalResultField.Gear] = gearsList.MaxBy(kv => kv.Value).Key;
						r[(int)ModalResultField.dist] = distance;
						r[(int)ModalResultField.v_act] = (v + vPrevious) / 2;
						vPrevious = v;
						results.Rows.Add(r);

						// reset remainder
						// reduce current dt by already taken diff
						gearsList.Clear();
						currentDt -= dt;
						remainingDt = 0.SI<Second>();
						remainingRow = null;
					}

					// if current row still longer than 1 second: split it to 1 second slices until it is < 1 second
					while (currentDt >= 1) {
						// calculate values
						var dt = 1.SI<Second>();
						currentDt = currentDt - 1.SI<Second>();
						var a = (MeterPerSquareSecond)row[(int)ModalResultField.acc];
						var ds = v * dt + a / 2 * dt * dt;
						if (ds.IsSmaller(0)) {
							throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative.");
						}
						absTime += dt;
						v += a * dt;
						distance += ds;

						// write a new row for the sliced 1 second
						var r = results.NewRow();
						r.ItemArray = row.ItemArray;
						r[(int)ModalResultField.time] = absTime;
						r[(int)ModalResultField.simulationInterval] = dt;
						r[(int)ModalResultField.dist] = distance;
						r[(int)ModalResultField.v_act] = (v + vPrevious) / 2;
						vPrevious = v;
						results.Rows.Add(r);
					}

					// if there still is something left in current row: add to weighted values to remainder
					if (currentDt > 0) {
						// calculate values
						var dt = currentDt;
						var gear = row[(int)ModalResultField.Gear];
						gearsList[gear] = gearsList.GetValueOrZero(gear) + dt;
						var a = (MeterPerSquareSecond)row[(int)ModalResultField.acc];
						var ds = v * dt + a / 2 * dt * dt;
						if (ds.IsSmaller(0)) {
							throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative.");
						}
						absTime += dt;
						v += a * dt;
						distance += ds;

						// add to remainder
						remainingRow = AddRow(remainingRow, MultiplyRow(row.ItemArray, dt));
						remainingDt += dt;
					} else {
						// reset remainder (just to be sure!)
						remainingRow = null;
						remainingDt = 0.SI<Second>();
						gearsList.Clear();
					}
				}

				// if last row was not enough to full second: take last row as whole second
				if (remainingDt > 0) {
					// calculate values
					var last = data.Rows.Cast<DataRow>().Last();
					var dt = remainingDt;
					var a = (MeterPerSquareSecond)last[(int)ModalResultField.acc];
					var ds = v * dt + a / 2 * dt * dt;
					if (ds.IsSmaller(0)) {
						throw new VectoSimulationException("1Hz-Filter: simulation distance must not be negative.");
					}
					v += a * dt;
					distance += ds;

					// write a new row for the last second
					var r = results.NewRow();
					r.ItemArray = MultiplyRow(remainingRow, 1 / dt).ToArray();
					r[(int)ModalResultField.time] = VectoMath.Ceiling(absTime);
					r[(int)ModalResultField.simulationInterval] = 1.SI<Second>();
					r[(int)ModalResultField.Gear] = gearsList.MaxBy(kv => kv.Value).Key;
					r[(int)ModalResultField.dist] = distance;
					r[(int)ModalResultField.v_act] = (v + vPrevious) / 2;
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