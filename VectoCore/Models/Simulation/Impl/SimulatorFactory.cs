using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.InputData;
using TUGraz.VectoCore.InputData.FileIO.Reader;
using TUGraz.VectoCore.InputData.FileIO.Reader.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

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

		public SimulatorFactory(FactoryMode mode, string jobFile)
		{
			Log.Fatal("########## VectoCore Version {0} ##########", Assembly.GetExecutingAssembly().GetName().Version);
			JobNumber = Interlocked.Increment(ref _jobNumberCounter);
			_mode = mode;
			switch (mode) {
				case FactoryMode.DeclarationMode:
					var report = new DeclarationReport(WindowsIdentity.GetCurrent().Name,
						Path.GetDirectoryName(jobFile), Path.GetFileNameWithoutExtension(jobFile));

					DataReader = new DeclarationModeVectoRunDataFactory();
					break;
				case FactoryMode.EngineeringMode:
					DataReader = new EngineeringModeVectoRunDataFactory();
					break;
				case FactoryMode.EngineOnlyMode:
					DataReader = new EngineOnlyVectoRunDataFactory();
					break;
				default:
					throw new VectoException("Unkown factory mode in SimulatorFactory: {0}", mode);
			}
			DataReader.SetJobFile(jobFile);
		}

		public IVectoRunDataFactory DataReader { get; private set; }

		public SummaryFileWriter SumWriter { get; set; }

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
				var modFileName = Path.Combine(data.BasePath,
					data.JobName.Replace(Constants.FileExtensions.VectoJobFile, "") + "_{0}{1}" +
					Constants.FileExtensions.ModDataFile);
				var d = data;
				IModalDataWriter modWriter =
					new ModalDataWriter(string.Format(modFileName, data.Cycle.Name, data.ModFileSuffix ?? ""),
						writer => d.Report.AddResult(d.Loading, d.Mission, writer), _mode);
				modWriter.WriteModalResults = WriteModalResults;
				var builder = new PowertrainBuilder(modWriter,
					DataReader.IsEngineOnly, (writer, mass, loading) =>
						SumWriter.Write(d.IsEngineOnly, modWriter, d.JobName, string.Format("{0}-{1}", JobNumber, i++),
							d.Cycle.Name + ".vdri",
							mass, loading));

				VectoRun run;
				if (data.IsEngineOnly) {
					run = new TimeRun(builder.Build(data));
				} else {
					var runCaption = string.Format("{0}-{1}-{2}",
						Path.GetFileNameWithoutExtension(data.JobName), data.Cycle.Name, data.ModFileSuffix);
					run = new DistanceRun(runCaption, builder.Build(data));
				}

				yield return run;
			}
		}
	}
}