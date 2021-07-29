/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Container for simulation jobs.
	/// </summary>
	public class JobContainer : LoggingObject
	{
		private class RunContainer
		{
			private readonly ISimulatorFactory _simulatorFactory;
			

			private bool followUpSimulatorFactoryFetched = false;

			private readonly HashSet<int> _unfinishedRuns = new HashSet<int>();
			private readonly ReaderWriterLockSlim _unfinishedRunsRwLock = new ReaderWriterLockSlim();


			public RunContainer(ISimulatorFactory simulatorFactory, IList<int> runIds)
			{
				_simulatorFactory = simulatorFactory;
				foreach (var runId in runIds) {
					_unfinishedRuns.Add(runId);
				}
			}

			public void JobCompleted(int runId)
			{
				try {
					_unfinishedRunsRwLock.EnterWriteLock();
					_unfinishedRuns.Remove(runId);
				} finally {
					_unfinishedRunsRwLock.ExitWriteLock();
				}
			
			}
			private bool AllCompleted()
			{
				try {
					_unfinishedRunsRwLock.EnterReadLock();
					return _unfinishedRuns.Count == 0;
				} finally {
					_unfinishedRunsRwLock.ExitReadLock();
				}
			}

			[MethodImpl(MethodImplOptions.Synchronized)]
			public ISimulatorFactory GetFollowUpSimulatorFactory()
			{
				try {
					if (followUpSimulatorFactoryFetched || !AllCompleted()) {
						return null;
					}
					followUpSimulatorFactoryFetched = true;
					return _simulatorFactory.FollowUpSimulatorFactory;
				} finally {

				}

			}

		}

		

		private readonly SummaryDataContainer _sumWriter;

		internal readonly List<RunEntry> Runs = new List<RunEntry>();
		private readonly HashSet<int> _unfinishedRuns = new HashSet<int>();
		private ReaderWriterLockSlim _runsRwLock = new ReaderWriterLockSlim();
		private ConcurrentDictionary<int, RunContainer> _runContainerMap  = new ConcurrentDictionary<int, RunContainer>();

		private static int _jobNumber;
		private bool _multithreaded = true;
		private bool _canceled = false;

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

			try {
				_runsRwLock.EnterWriteLock();
				Runs.Add(new RunEntry(run, this));
				_unfinishedRuns.Add(run.RunIdentifier);
			} finally {
				_runsRwLock.ExitWriteLock();
			}

		}

		public struct CycleTypeDescription
		{
			public string Name;
			public CycleType CycleType;
		}

		public IEnumerable<CycleTypeDescription> GetCycleTypes(){
			try {
				_runsRwLock.EnterReadLock();
				return Runs.Select(
						r => new CycleTypeDescription {
							Name = r.Run.CycleName,
							CycleType = r.Run.GetContainer().RunData.Cycle?.CycleType ?? CycleType.None
						})
					.Distinct();
			} finally {
				_runsRwLock.ExitReadLock();
			}
		}

		/// <summary>
		/// Adds the runs from the factory to the job container.
		/// </summary>
		/// <returns>A List of Run-Identifiers (unique), int</returns>
		public List<int> AddRuns(ISimulatorFactory factory)
		{
			var runIDs = new List<int>();
			factory.SumData = _sumWriter;
			factory.JobNumber = Interlocked.Increment(ref _jobNumber);

			try {
				_runsRwLock.EnterWriteLock();
				foreach (var run in factory.SimulationRuns()) {
					var entry = new RunEntry(run, this, factory.JobNumber);
					Runs.Add(entry);
					_unfinishedRuns.Add(run.RunIdentifier);
					runIDs.Add(entry.RunId);
				}
			} finally {
				_runsRwLock.ExitWriteLock();
			}

			var added = _runContainerMap.TryAdd(factory.JobNumber, new RunContainer(factory, runIDs));
			System.Diagnostics.Debug.Assert(added);
			return runIDs;


		}

		/// <summary>
		/// Execute all runs, waits until finished.
		/// </summary>
		public void Execute(bool multithreaded = true)
		{
			
			_multithreaded = multithreaded;
			Log.Info("VectoRun started running. Executing Runs.");
			if (_canceled) {
					Log.Info("JobContainer already cancelled\n");
					return;
			}


			try {
				_runsRwLock.EnterWriteLock();
				if (multithreaded) {
					Runs.ForEach(r => { r.RunWorkerAsync(); });
				} else {
					var first = new Task(() => { });
					var task = first;
					// ReSharper disable once LoopCanBeConvertedToQuery
					foreach (var run in Runs) {
						var r = run;
						task = task.ContinueWith(t => r.RunWorkerAsync().Wait(),
							TaskContinuationOptions.OnlyOnRanToCompletion);
					}

					first.Start();
				}
			} finally {
				_runsRwLock.ExitWriteLock();
			}
		}

		public void Cancel()
		{

			foreach (var job in Runs) {
				job.CancelAsync();
			}
			WaitFinished();
		}

		public void CancelCurrent()
		{
			foreach (var job in Runs) {
				job.CancelAsync();
			}
		}

		public void WaitFinished()
		{
			Task.WaitAll(Runs.Select(r => r.RunTask).ToArray());
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		private void JobCompleted(int runId, int runContainerId)
		{
			_runContainerMap.TryGetValue(runContainerId, out var runContainer);
			runContainer?.JobCompleted(runId);
			AddFollowUpSimulatorFactories(runContainerId);

			try {
				_runsRwLock.EnterWriteLock();
				_unfinishedRuns.Remove(runId);
			} finally {
				_runsRwLock.ExitWriteLock();
			}




			if (AllCompleted) {
				_sumWriter.Finish();
			}
		}

		private void AddFollowUpSimulatorFactories(int runContainerId)
		{
			_runContainerMap.TryGetValue(runContainerId, out var runContainer);
			var additionalSimulatorFactory = runContainer?.GetFollowUpSimulatorFactory();
			if (additionalSimulatorFactory == null)
				return;

			AddRuns(additionalSimulatorFactory);
			Execute(_multithreaded);
		}

		public bool AllCompleted
		{
			get
			{
				try {
					_runsRwLock.EnterReadLock();
					return _unfinishedRuns.Count == 0;
				} finally {
					_runsRwLock.ExitReadLock();
				}
				
			}
		}

		public Dictionary<int, ProgressEntry> GetProgress()
		{
			return Runs.ToDictionary(
					r => r.Run.RunIdentifier,
					r => new ProgressEntry
					{
						RunId = r.Run.RunIdentifier,
						JobRunId = r.Run.JobRunIdentifier,
						RunName = r.Run.RunName,
						CycleName = r.Run.CycleName,
						RunSuffix = r.Run.RunSuffix,
						Progress = r.Run.Progress,
						Done = r.Done,
						ExecTime = r.ExecTime,
						Success = r.Success,
						Canceled = r.Canceled,
						Error = r.ExecException
					});

		}

		public class ProgressEntry
		{
			// unique identifier of the simulation run
			public int RunId;
			// job-local identifier of the simulation run
			public int JobRunId;

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

		[DebuggerDisplay("{Run.RunIdentifier}: {Run.RunName}, {Run.CycleName}")]
		internal class RunEntry : LoggingObject
		{
			public IVectoRun Run;
			public JobContainer JobContainer;
			private readonly ReaderWriterLockSlim _doneLock = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
			private bool _done = false;
			public bool Done => _done;

			public bool Running;
			public bool Success;
			public bool Canceled;
			public double ExecTime;
			public Exception ExecException;
			public readonly Task RunTask;
			private readonly int _runContainerId;

			public int RunId => Run.RunIdentifier;
			public int JobRunId => Run.JobRunIdentifier;

			public RunEntry(IVectoRun run, JobContainer jobContainer, int runContainerId = -1)
			{
				Run = run;
				JobContainer = jobContainer;
				_runContainerId = runContainerId;
				RunTask = new Task(() => {
					var stopWatch = Stopwatch.StartNew();
					try {
						Running = true;
						Run.Run();
					} catch (Exception ex) {
						Log.Error(ex, "Error during simulation run!");
						ExecException = ex;
						throw;
					} finally {
						stopWatch.Stop();
						Success = Run.FinishedWithoutErrors && ExecException == null;
						ExecTime = stopWatch.Elapsed.TotalMilliseconds;
						JobContainer.JobCompleted(RunId, _runContainerId);
						_done = true;

						//Notify which job has completed (ID)
					}
				});
			}

			public Task RunWorkerAsync()
			{
				if (Running == false) {
					RunTask.Start();
				}
				return RunTask;
			}

			public void CancelAsync()
			{
				Run.Cancel();
				Canceled = true;
			}
		}
	}
}