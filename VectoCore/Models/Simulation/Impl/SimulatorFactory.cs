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
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.Reader.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.PDF;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public enum ExecutionMode
	{
		Engineering,
		Declaration,
		EngineOnly,
	}

	public class SimulatorFactory : LoggingObject
	{
		private static int _jobNumberCounter;

		private readonly ExecutionMode _mode;

		public SimulatorFactory(ExecutionMode mode, IInputDataProvider dataProvider, IOutputDataWriter writer,
			DeclarationReport report = null)
		{
			Log.Fatal("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			_mode = mode;
			ModWriter = writer;
			switch (mode) {
				case ExecutionMode.Declaration:
					report = report ?? new PDFDeclarationReport(writer);
					var windowsIdentity = WindowsIdentity.GetCurrent();
					report.Creator = windowsIdentity != null ? windowsIdentity.Name : "N/A";
					report.JobName = dataProvider.JobInputData().JobName;
					DataReader = new DeclarationModeVectoRunDataFactory(dataProvider, report);
					break;
				case ExecutionMode.Engineering:
					DataReader = new EngineeringModeVectoRunDataFactory(dataProvider);
					break;
				case ExecutionMode.EngineOnly:
					DataReader = new EngineOnlyVectoRunDataFactory(dataProvider);
					break;
				default:
					throw new VectoException("Unkown factory mode in SimulatorFactory: {0}", mode);
			}
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
				var d = data;
				IModalDataContainer modContainer =
					new ModalDataContainer(data, ModWriter,
						writer => {
							if (d.Report != null) {
								d.Report.AddResult(d.Loading, d.Mission, writer);
							}
						}, _mode);
				modContainer.WriteModalResults = WriteModalResults;
				var current = i++;
				var builder = new PowertrainBuilder(modContainer, (writer, mass, loading) =>
					SumData.Write(d.IsEngineOnly, modContainer, d.JobName, string.Format("{0}-{1}", JobNumber, current),
						d.Cycle.Name + Constants.FileExtensions.CycleFile, mass, loading));

				VectoRun run;

				switch (data.Cycle.CycleType) {
					case CycleType.DistanceBased:
						run = new DistanceRun(builder.Build(data));
						break;
					case CycleType.EngineOnly:
						run = new TimeRun(builder.Build(data));
						break;
					case CycleType.TimeBased:
						run = new TimeRun(builder.Build(data));
						break;
					case CycleType.PWheel:
						run = new TimeRun(builder.Build(data));
						break;
					case CycleType.MeasuredSpeedDyno:
						run = new TimeRun(builder.Build(data));
						break;
					default:
						throw new ArgumentOutOfRangeException("CycleType unknown:" + data.Cycle.CycleType);
				}

				var validationErrors = run.Validate();
				if (validationErrors.Any()) {
					throw new VectoException("Validation of Run-Data Failed: " +
											"; ".Join(validationErrors.Select(r => r.ErrorMessage)));
				}


				yield return run;
			}
		}
	}
}