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
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.ModFilter
{
	public class ActualModalDataFilter : IModalDataFilter
	{
		public ModalResults Filter(ModalResults data)
		{
			var results = (ModalResults)data.Clone();

			//var dt = 1e-12.SI<Second>();
			//var ds = 1e-12.SI<Meter>();

			var init = data.Rows[0];
			var v_act = init.Field<MeterPerSecond>((int)ModalResultField.v_act);
			var n_engine = init.Field<PerSecond>((int)ModalResultField.n_eng_avg);
			var dist = init.Field<Meter>((int)ModalResultField.dist);
			var n_gbx_out = init.Field<PerSecond>((int)ModalResultField.n_gbx_out_avg);

			for (var i = 1; i < data.Rows.Count; i++) {
				//var prev = data.Rows[i - 1];
				var current = data.Rows[i];
				var start = results.NewRow();
				var end = results.NewRow();

				start[(int)ModalResultField.time] = current.Field<Second>((int)ModalResultField.time) -
													current.Field<Second>((int)ModalResultField.simulationInterval) / 2.0;
				end[(int)ModalResultField.time] = current.Field<Second>((int)ModalResultField.time) +
												current.Field<Second>((int)ModalResultField.simulationInterval) / 2.0;

				SetConstantValues(current, start, end,
					ModalResultField.simulationInterval,
					ModalResultField.simulationDistance,
					ModalResultField.acc,
					ModalResultField.grad,
					ModalResultField.Gear,
					ModalResultField.TC_Locked);

				start[(int)ModalResultField.v_act] = v_act;
				v_act = 2 * current.Field<MeterPerSecond>((int)ModalResultField.v_act) - v_act;
				end[(int)ModalResultField.v_act] = v_act;

				SetConstantValues(current, start, end, ModalResultField.v_targ);

				start[(int)ModalResultField.dist] = dist;
				dist = current.Field<Meter>((int)ModalResultField.dist);
				end[(int)ModalResultField.dist] = dist;

				start[(int)ModalResultField.n_eng_avg] = n_engine;
				n_engine = 2 * current.Field<PerSecond>((int)ModalResultField.n_eng_avg) - n_engine;
				end[(int)ModalResultField.n_eng_avg] = n_engine;

				start[(int)ModalResultField.n_gbx_out_avg] = n_gbx_out;
				n_gbx_out = 2 * current.Field<PerSecond>((int)ModalResultField.n_gbx_out_avg) - n_gbx_out;
				end[(int)ModalResultField.n_gbx_out_avg] = n_gbx_out;

				SetConstantValues(current, start, end,
					ModalResultField.T_eng_fcmap,
					ModalResultField.Tq_full,
					ModalResultField.Tq_drag,
					ModalResultField.T_gbx_out
					);

				SetConstantValues(current, start, end,
					ModalResultField.P_eng_full,
					ModalResultField.P_eng_full_stat,
					ModalResultField.P_eng_out,
					ModalResultField.P_eng_drag,
					ModalResultField.P_eng_fcmap,
					ModalResultField.P_clutch_out,
					ModalResultField.P_clutch_loss,
					ModalResultField.P_aux,
					ModalResultField.P_eng_inertia,
					ModalResultField.P_gbx_in,
					ModalResultField.P_gbx_inertia,
					ModalResultField.P_gbx_loss,
					ModalResultField.P_angle_loss,
					ModalResultField.P_retarder_in,
					ModalResultField.P_ret_loss,
					ModalResultField.P_veh_inertia,
					ModalResultField.P_roll,
					ModalResultField.P_air,
					ModalResultField.P_slope,
					ModalResultField.P_wheel_in,
					ModalResultField.P_brake_in,
					ModalResultField.P_brake_loss,
					ModalResultField.P_wheel_inertia,
					ModalResultField.P_axle_in,
					ModalResultField.P_axle_loss,
					ModalResultField.P_angle_in,
					ModalResultField.P_angle_loss,
					ModalResultField.P_trac);

				SetConstantValues(current, start, end,
					ModalResultField.FCMap,
					ModalResultField.FCAUXc,
					ModalResultField.FCAAUX,
					ModalResultField.FCWHTCc,
					ModalResultField.FCFinal);

				//---
				results.Rows.Add(start);
				results.Rows.Add(end);
			}

			return results;
		}

		private void SetConstantValues(DataRow current, DataRow start, DataRow end, params ModalResultField[] fields)
		{
			foreach (var field in fields) {
				if (current[(int)field] == DBNull.Value) {
					continue;
				}
				if (field.GetDataType() == typeof(SI)) {
					start[(int)field] = current.Field<SI>((int)field);
					end[(int)field] = current.Field<SI>((int)field);
				} else if (field.GetDataType() == typeof(double)) {
					start[(int)field] = current.Field<double>((int)field);
					end[(int)field] = current.Field<double>((int)field);
				} else if (field.GetDataType() == typeof(int)) {
					start[(int)field] = current.Field<int>((int)field);
					end[(int)field] = current.Field<int>((int)field);
				} else if (field.GetDataType() == typeof(uint)) {
					start[(int)field] = current.Field<uint>((int)field);
					end[(int)field] = current.Field<uint>((int)field);
				}
			}
		}

		public string ID
		{
			get { return "sim"; }
		}
	}
}