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
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	/// <summary>
	/// Simulator for one vecto simulation job.
	/// </summary>
	public abstract class VectoRun : LoggingObject, IVectoRun
	{
		private static uint _runIdCounter;

		protected Second AbsTime = 0.SI<Second>();
		// ReSharper disable once InconsistentNaming
		protected Second dt = 1.SI<Second>();
		protected SummaryDataContainer SumWriter { get; set; }
		protected string JobName { get; set; }
		protected ISimulationOutPort CyclePort { get; set; }

		[Required, ValidateObject]
		protected IVehicleContainer Container { get; set; }

		public bool FinishedWithoutErrors { get; protected set; }
		public uint RunIdentifier { get; protected set; }

		public string RunName
		{
			get { return Container.RunData.JobName; }
		}

		public string CycleName
		{
			get { return Container.RunData.Cycle.Name; }
		}

		public string RunSuffix
		{
			get { return Container.RunData.ModFileSuffix; }
		}

		protected VectoRun(IVehicleContainer container)
		{
			Container = container;
			RunIdentifier = _runIdCounter++;
			Container.RunStatus = Status.Pending;
			CyclePort = container.GetCycleOutPort();
		}

		public IVehicleContainer GetContainer()
		{
			return Container;
		}


		public void Run(BackgroundWorker worker = null, Action<double> reportProgressAction = null)
		{
			var debug = new DebugData();
			
			Log.Info("VectoJob started running.");

			Initialize();
			try {
				IResponse response;
				do {
					response = DoSimulationStep();
					debug.Add(response);
					if (response is ResponseSuccess) {
						Container.CommitSimulationStep(AbsTime, dt);
						AbsTime += dt;
						if (reportProgressAction != null) {
							reportProgressAction(CyclePort.Progress);
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
					AbsTime, Container.Distance, dt, Container.VehicleSpeed, TryCatch(() => Container.Gear),
					vse.Message, RunIdentifier);
			} catch (VectoException ve) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(ve);
				Container.RunStatus = Status.Aborted;
				Container.FinishSimulation();
				throw new VectoSimulationException("{6} - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}", ve,
					AbsTime, Container.Distance, dt, Container.VehicleSpeed, TryCatch(() => Container.Gear), ve.Message,
					RunIdentifier);
			} catch (Exception e) {
				Log.Error("SIMULATION RUN ABORTED! ========================");
				Log.Error(e);
				Container.RunStatus = Status.Aborted;
				Container.FinishSimulation();
				throw new VectoSimulationException("{6} - absTime: {0}, distance: {1}, dt: {2}, v: {3}, Gear: {4} | {5}", e, AbsTime,
					Container.Distance, dt, Container.VehicleSpeed, TryCatch(() => Container.Gear), e.Message,
					RunIdentifier);
			}
			Container.RunStatus = Status.Success;
			Container.FinishSimulation();
			Log.Info("VectoJob finished.");
		}

		private static object TryCatch(Func<object> action)
		{
			try {
				return action();
			} catch (VectoException e) {
				LogManager.GetLogger(typeof(VectoRun).FullName).Info(e);
				return null;
			}
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