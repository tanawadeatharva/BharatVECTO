/*
* Copyright 2015, 2016 Graz University of Technology,
* Institute of Internal Combustion Engines and Thermodynamics,
* Institute of Technical Informatics
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData
{
	public class ModalDataContainer : IModalDataContainer
	{
		private readonly ExecutionMode _mode;
		private readonly Action<ModalDataContainer> _addReportResult;
		internal ModalResults Data { get; set; }
		private DataRow CurrentRow { get; set; }
		//private readonly VectoRunData _runData;

		private readonly IModalDataWriter _writer;
		public string RunName { get; private set; }
		public string CycleName { get; private set; }
		public string RunSuffix { get; private set; }

		public bool WriteModalResults { get; set; }

		public VectoRun.Status RunStatus { get; protected set; }

		public ModalDataContainer(string runName, IModalDataWriter writer,
			ExecutionMode mode = ExecutionMode.Engineering)
			: this(runName, "", "", writer, _ => { }, mode) {}

		public ModalDataContainer(VectoRunData runData, IModalDataWriter writer, Action<ModalDataContainer> addReportResult,
			ExecutionMode mode = ExecutionMode.Engineering)
			: this(runData.JobName, runData.Cycle.Name, runData.ModFileSuffix, writer, addReportResult, mode) {}

		protected ModalDataContainer(string runName, string cycleName, string runSuffix, IModalDataWriter writer,
			Action<ModalDataContainer> addReportResult, ExecutionMode mode)

		{
			HasTorqueConverter = false;
			RunName = runName;
			CycleName = cycleName;
			RunSuffix = runSuffix;
			_writer = writer;

			_mode = mode;
			_addReportResult = addReportResult;

			Data = new ModalResults();
			Auxiliaries = new Dictionary<string, DataColumn>();
			CurrentRow = Data.NewRow();
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

			if (_mode != ExecutionMode.EngineOnly) {
				dataColumns.AddRange(new[] {
					ModalResultField.simulationInterval,
					ModalResultField.dist,
					ModalResultField.v_act,
					ModalResultField.v_targ,
					ModalResultField.acc,
					ModalResultField.grad
				});
			}
			if (_mode != ExecutionMode.EngineOnly) {
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

			if (_mode != ExecutionMode.EngineOnly) {
				dataColumns.AddRange(new[] {
					ModalResultField.P_gbx_in,
					ModalResultField.P_gbx_loss,
					ModalResultField.P_gbx_inertia,
					ModalResultField.P_retarder_in,
					ModalResultField.P_ret_loss,
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
						ModalResultField.TCmu,
						ModalResultField.TC_M_Out,
						ModalResultField.TC_n_Out
					});
				}
			}

			var strCols = dataColumns.Select(x => x.GetName())
				.Concat((Auxiliaries.Values.Select(c => c.ColumnName)))
				.Concat(new[] { ModalResultField.FCMap, ModalResultField.FCAUXc, ModalResultField.FCWHTCc }.Select(x => x.GetName()));

			if (_mode != ExecutionMode.Declaration || WriteModalResults) {
				//VectoCSVFile.Write(_modWriter, new DataView(Data).ToTable(false, strCols.ToArray()));
				_writer.WriteModData(RunName, CycleName, RunSuffix,
					new DataView(Data).ToTable(false, strCols.ToArray()));
			}

			if (_mode == ExecutionMode.Declaration) {
				_addReportResult(this);
			}
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
	}
}