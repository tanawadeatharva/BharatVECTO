/*
* Copyright 2015 Graz University of Technology
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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Container for simulation jobs.
	/// </summary>
	public class JobContainer : LoggingObject
	{
		internal readonly List<RunEntry> Runs = new List<RunEntry>();
		private readonly SummaryFileWriter _sumWriter;

		private static int _jobNumber;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="JobContainer"/> class.
		/// </summary>
		/// <param name="sumWriter">The sum writer.</param>
		public JobContainer(SummaryFileWriter sumWriter)
		{
			_sumWriter = sumWriter;
		}

		public string SumFileName
		{
			get { return _sumWriter.SumFileName; }
		}

		public void AddRun(IVectoRun run)
		{
			_jobNumber++;
			Runs.Add(new RunEntry { Run = run, JobContainer = this });
		}

		public void AddRuns(IEnumerable<IVectoRun> runs)
		{
			_jobNumber++;
			//Runs.AddRange(runs);
			foreach (var run in runs) {
				Runs.Add(new RunEntry { Run = run, JobContainer = this });
			}
		}

		public void AddRuns(SimulatorFactory factory)
		{
			factory.SumWriter = _sumWriter;
			factory.JobNumber = _jobNumber++;
			AddRuns(factory.SimulationRuns());
		}

		/// <summary>
		/// Execute all runs, waits until finished.
		/// </summary>
		public void Execute(bool multithreaded = true)
		{
			Log.Info("VectoRun started running. Executing Runs.");

			foreach (var job in Runs) {
				if (multithreaded) {
					job.Started = true;
					job.RunWorkerAsync();
				}
			}
			if (!multithreaded) {
				var entry = Runs.First();
				entry.Started = true;
				entry.RunWorkerAsync();
			}
		}

		public void Cancel()
		{
			foreach (var job in Runs) {
				job.CancelAsync();
			}
		}

		public void CancelCurrent()
		{
			foreach (var job in Runs) {
				job.CancelAsync();
			}
		}

		private static readonly AutoResetEvent resetEvent = new AutoResetEvent(false);

		public void WaitFinished()
		{
			resetEvent.WaitOne();
		}


		private void JobCompleted()
		{
			var next = Runs.FirstOrDefault(x => x.Started == false);
			if (next != null) {
				next.Started = true;
				next.RunWorkerAsync();
			}
			if (AllCompleted) {
				_sumWriter.Finish();
				resetEvent.Set();
			}
		}

		public Dictionary<string, ProgressEntry> GetProgress()
		{
			return Runs.ToDictionary(jobEntry => jobEntry.Run.Name, entry => new ProgressEntry {
				Progress = entry.Progress,
				Done = entry.Done,
				ExecTime = entry.ExecTime,
				Success = entry.Success,
				Canceled = entry.Canceled,
				Error = entry.ExecException,
				ModFileName = entry.Run.GetContainer().ModFileName
			});
		}

		public bool AllCompleted
		{
			get { return Runs.All(x => x.Done); }
		}

		public class ProgressEntry
		{
			public double Progress;
			public double ExecTime;
			public Exception Error;
			public bool Canceled;
			public bool Success;
			public bool Done;
			public string ModFileName;
		}

		internal class RunEntry : LoggingObject
		{
			public IVectoRun Run;
			public JobContainer JobContainer;
			public double Progress;
			public bool Done;
			public bool Started;
			public bool Success;
			public bool Canceled;
			public double ExecTime;
			public Exception ExecException;

			private readonly BackgroundWorker _worker = new BackgroundWorker();

			public RunEntry()
			{
				_worker.DoWork += OnDoWork;
				_worker.RunWorkerCompleted += OnRunWorkerCompleted;
				_worker.WorkerSupportsCancellation = true;
			}

			public void RunWorkerAsync()
			{
				_worker.RunWorkerAsync();
			}

			public void CancelAsync()
			{
				_worker.CancelAsync();
			}

			private void OnDoWork(object sender, DoWorkEventArgs e)
			{
				var stopWatch = Stopwatch.StartNew();
				try {
					Run.Run(_worker, (x => Progress = x));
				} catch (Exception ex) {
					Log.Error(ex, "Error during simulation run!");
					ExecException = ex;
				}
				if (_worker.CancellationPending) {
					e.Cancel = true;
					Canceled = true;
				}
				stopWatch.Stop();
				Success = Run.FinishedWithoutErrors;
				Done = true;
				ExecTime = stopWatch.Elapsed.TotalMilliseconds;
				JobContainer.JobCompleted();
			}

			private void OnRunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
			{
				if (e.Error != null) {
					ExecException = e.Error;
				}
			}
		}
	}
}