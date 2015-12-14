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
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.PDF;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class SimulatorFactory : LoggingObject
	{
		private static int _jobNumberCounter;

		public enum FactoryMode
		{
			EngineeringMode,
			DeclarationMode,
			EngineOnlyMode,
		};

		private FactoryMode _mode;

		public SimulatorFactory(FactoryMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer)
		{
			Log.Fatal("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			_mode = mode;
			ModWriter = writer;
			switch (mode) {
				case FactoryMode.DeclarationMode:
					var report = new DeclarationReport(WindowsIdentity.GetCurrent().Name,
						dataProvider.JobInputData().JobName, writer);

					DataReader = new DeclarationModeVectoRunDataFactory(dataProvider, report);
					break;
				case FactoryMode.EngineeringMode:
					DataReader = new EngineeringModeVectoRunDataFactory(dataProvider);
					break;
				case FactoryMode.EngineOnlyMode:
					DataReader = new EngineOnlyVectoRunDataFactory(dataProvider);
					break;
				default:
					throw new VectoException("Unkown factory mode in SimulatorFactory: {0}", mode);
			}
			//DataReader.SetJobFile(jobFile);
		}

		public IVectoRunDataFactory DataReader { get; private set; }

		public SummaryDataContainer SumData { get; set; }

		public IOutputDataWriter ModWriter { get; private set; }


		public int JobNumber { get; set; }

		public bool WriteModalResults { get; set; }

		/// <summary>
		/// Creates powertrain and initializes it with the component's data.
		/// </summary>
		/// <returns>new VectoRun Instance</returns>
		public IEnumerable<IVectoRun> SimulationRuns()
		{
			var i = 0;
			foreach (var data in DataReader.NextRun()) {
				//var modFileName = Path.Combine(data.BasePath,
				//	data.JobName.Replace(Constants.FileExtensions.VectoJobFile, "") + "_{0}{1}" +
				//	Constants.FileExtensions.ModDataFile);
				// -> string.Format(modFileName, data.Cycle.Name, data.ModFileSuffix ?? "")
				var d = data;
				IModalDataContainer modContainer =
					new ModalDataContainer(data, ModWriter,
						writer => {
							if (d.Report != null) {
								d.Report.AddResult(d.Loading, d.Mission, writer);
							}
						}, _mode);
				modContainer.WriteModalResults = WriteModalResults;
				var builder = new PowertrainBuilder(modContainer,
					data.IsEngineOnly, (writer, mass, loading) =>
						SumData.Write(d.IsEngineOnly, modContainer, d.JobName, string.Format("{0}-{1}", JobNumber, i++),
							d.Cycle.Name + Constants.FileExtensions.CycleFile, mass, loading));

				VectoRun run;
				if (data.IsEngineOnly) {
					run = new TimeRun(builder.Build(data));
				} else {
					//var runCaption = string.Format("{0}-{1}-{2}",
					//	Path.GetFileNameWithoutExtension(data.JobName), data.Cycle.Name, data.ModFileSuffix);
					run = new DistanceRun(builder.Build(data));
				}

				yield return run;
			}
		}
	}
}