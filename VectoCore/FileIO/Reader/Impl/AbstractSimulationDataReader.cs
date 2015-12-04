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
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.FileIO.Reader.Impl
{
	public abstract class AbstractSimulationDataReader : InputFileReader, ISimulationDataReader
	{
		//protected string JobBasePath = "";

		protected VectoJobFile Job { get; set; }

		protected VectoVehicleFile Vehicle { get; set; }

		protected VectoGearboxFile Gearbox { get; set; }

		protected VectoEngineFile Engine { get; set; }

		protected IList<VectoRunData.AuxData> Aux { get; set; }

		public void SetJobFile(string filename)
		{
			Job = ReadJobFile(filename);
			ProcessJob(Job);
		}

		public abstract bool IsEngineOnly { get; }

		public abstract IEnumerable<VectoRunData> NextRun();


		protected abstract void ProcessJob(VectoJobFile job);


		/// <summary>
		/// has to read the file string and create file-container
		/// </summary>
		protected abstract VectoJobFile ReadJobFile(string file);

		/// <summary>
		/// has to read the file string and create file-container
		/// </summary>
		protected abstract VectoVehicleFile ReadVehicle(string file);

		protected abstract VectoEngineFile ReadEngine(string file);

		protected abstract VectoGearboxFile ReadGearbox(string file);

		protected abstract IList<VectoRunData.AuxData> ReadAuxiliary(string basePath,
			IEnumerable<VectoAuxiliaryFile> auxiliaries);
	}
}