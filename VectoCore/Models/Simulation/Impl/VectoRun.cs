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
using System.ComponentModel;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Simulator for one vecto simulation job.
	/// </summary>
	public abstract class VectoRun : LoggingObject, IVectoRun
	{
		public string Name { get; protected set; }

		protected Second AbsTime = 0.SI<Second>();
		protected Second dt = 1.SI<Second>();
		protected SummaryFileWriter SumWriter { get; set; }
		protected string JobFileName { get; set; }
		protected string JobName { get; set; }
		protected ISimulationOutPort CyclePort { get; set; }
		//protected IModalDataWriter DataWriter { get; set; }
		protected IVehicleContainer Container { get; set; }

		public bool FinishedWithoutErrors { get; protected set; }

		protected VectoRun(IVehicleContainer container)
		{
			Container = container;
			Container.RunStatus = Status.Pending;
			CyclePort = container.GetCycleOutPort();
		}

		public IVehicleContainer GetContainer()
		{
			return Container;
		}


		public void Run(BackgroundWorker worker = null, Action<double> ReportProgress = null)
		{
			Log.Info("VectoJob started running.");

			Initialize();
			try {
				IResponse response;
				do {
					response = DoSimulationStep();
					if (response is ResponseSuccess) {
						Container.CommitSimulationStep(AbsTime, dt);
						AbsTime += dt;
						if (ReportProgress != null) {
							ReportProgress(CyclePort.Progress);
						}
						if (worker != null && worker.CancellationPending) {
							Log.Error("Background Task canceled!");
							Container.RunStatus = Status.Canceled;
							Container.FinishSimulation();
							return;
						}
					}
				} while (response is ResponseSuccess);
			} catch (VectoSimulationException vse) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(vse);
				Container.RunStatus = Status.Aborted;
				Container.FinishSimulation();
				throw new VectoSimulationException("{6} - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}", vse,
					AbsTime, Container.Distance, dt, Container.VehicleSpeed, Container.Gear, vse.Message, Name);
			} catch (VectoException ve) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(ve);
				Container.RunStatus = Status.Aborted;
				Container.FinishSimulation();
				throw new VectoSimulationException("{6} - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}", ve,
					AbsTime, Container.Distance, dt, Container.VehicleSpeed, Container.Gear, ve.Message, Name);
			} catch (Exception e) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(e);
				Container.RunStatus = Status.Aborted;
				Container.FinishSimulation();
				throw new VectoSimulationException("{6} - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}", e, AbsTime,
					Container.Distance, dt, Container.VehicleSpeed, Container.Gear, e.Message, Name);
			}
			Container.RunStatus = Status.Success;
			Container.FinishSimulation();
			Log.Info("VectoJob finished.");
		}

		protected abstract IResponse DoSimulationStep();

		protected abstract IResponse Initialize();

		public enum Status
		{
			Pending,
			Running,
			Success,
			Canceled,
			Aborted,
		}
	}
}