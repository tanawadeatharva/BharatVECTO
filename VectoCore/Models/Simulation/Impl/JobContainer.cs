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
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Container for simulation jobs.
	/// </summary>
	public class JobContainer : LoggingObject
	{
		internal readonly List<RunEntry> Runs = new List<RunEntry>();
		private readonly SummaryDataContainer _sumWriter;

		private static int _jobNumber;

		/// <summary>
		/// Initializes a new empty instance of the <see cref="JobContainer"/> class.
		/// </summary>
		/// <param name="sumWriter">The sum writer.</param>
		public JobContainer(SummaryDataContainer sumWriter)
		{
			_sumWriter = sumWriter;
		}

		public void AddRun(IVectoRun run)
		{
			Interlocked.Increment(ref _jobNumber);
			Runs.Add(new RunEntry { Run = run, JobContainer = this });
		}

		public struct CycleTypeDescription
		{
			public string Name;
			public CycleType CycleType;
		}

		public IEnumerable<CycleTypeDescription> GetCycleTypes()
		{
			return Runs.Select(r => new CycleTypeDescription {Name = r.Run.CycleName,CycleType = r.Run.GetContainer().RunData.Cycle.CycleType}).Distinct();
		}

		public IEnumerable<int> AddRuns(SimulatorFactory factory)
		{
			factory.SumData = _sumWriter;
			factory.JobNumber = Interlocked.Increment(ref _jobNumber);

			foreach (var run in factory.SimulationRuns()) {
				var entry = new RunEntry { Run = run, JobContainer = this };
				Runs.Add(entry);
				yield return entry.Run.RunIdentifier;
			}
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

		private static readonly AutoResetEvent ResetEvent = new AutoResetEvent(false);

		public void WaitFinished()
		{
			ResetEvent.WaitOne();
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
				ResetEvent.Set();
			}
		}

		public Dictionary<int, ProgressEntry> GetProgress()
		{
			return Runs.ToDictionary(jobEntry => jobEntry.Run.RunIdentifier, entry => new ProgressEntry {
				RunName = entry.Run.RunName,
				CycleName = entry.Run.CycleName,
				RunSuffix = entry.Run.RunSuffix,
				Progress = entry.Progress,
				Done = entry.Done,
				ExecTime = entry.ExecTime,
				Success = entry.Success,
				Canceled = entry.Canceled,
				Error = entry.ExecException
			});
		}

		public bool AllCompleted
		{
			get { return Runs.All(r => r.Done); }
		}

		public class ProgressEntry
		{
			public string RunName;
			public double Progress;
			public double ExecTime;
			public Exception Error;
			public bool Canceled;
			public bool Success;
			public bool Done;
			public string CycleName;
			public string RunSuffix;
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
					Run.Run(_worker, x => Progress = x);
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