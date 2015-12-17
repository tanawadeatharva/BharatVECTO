/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Tests.Utils
{
	/// <summary>
	/// Fake Data Writer Class for Tests.
	/// </summary>
	internal class MockModalDataContainer : IModalDataContainer
	{
		public MockModalDataContainer()
		{
			Data = new ModalResults();
			CurrentRow = Data.NewRow();
			Auxiliaries = new Dictionary<string, DataColumn>();
		}

		public ModalResults Data { get; set; }
		public DataRow CurrentRow { get; set; }

		public string ModFileName
		{
			get { return ""; }
		}

		public object this[string auxId]
		{
			get { return CurrentRow[Auxiliaries[auxId]]; }
			set { CurrentRow[Auxiliaries[auxId]] = value; }
		}

		public bool HasTorqueConverter { get; set; }

		public void CommitSimulationStep()
		{
			Data.Rows.Add(CurrentRow);
			CurrentRow = Data.NewRow();
		}

		public VectoRun.Status RunStatus
		{
			get { return VectoRun.Status.Success; }
		}


		public void Finish(VectoRun.Status runStatus) {}

		public bool WriteModalResults { get; set; }

		public IEnumerable<T> GetValues<T>(ModalResultField key)
		{
			return Data.Rows.Cast<DataRow>().Select(x => x.Field<T>((int)key));
		}

		public IEnumerable<T> GetValues<T>(DataColumn col)
		{
			return Data.Rows.Cast<DataRow>().Select(x => x.Field<T>(col));
		}

		public Dictionary<string, DataColumn> Auxiliaries { get; set; }

		public void AddAuxiliary(string id)
		{
			var auxColName = ModalResultField.Paux_ + id;
			if (!Data.Columns.Contains(auxColName)) {
				Auxiliaries[id] = Data.Columns.Add(auxColName, typeof(Watt));
			}
			Auxiliaries[id] = Data.Columns[auxColName];
		}

		public string RunName { get; set; }
		public string CycleName { get; set; }
		public string RunSuffix { get; set; }

		public object this[ModalResultField key]
		{
			get { return CurrentRow[key.GetName()]; }
			set { CurrentRow[key.GetName()] = value; }
		}

		public void CommitSimulationStep(Second absTime, Second simulationInterval)
		{
			CurrentRow[ModalResultField.time.GetName()] = (absTime + simulationInterval / 2);
			CurrentRow[ModalResultField.simulationInterval.GetName()] = simulationInterval;
			CommitSimulationStep();
		}
	}
}