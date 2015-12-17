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

using System;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class DistanceRun : VectoRun
	{
		public DistanceRun(IVehicleContainer container) : base(container) {}

		protected override IResponse DoSimulationStep()
		{
			// estimate distance to be traveled within the next TargetTimeInterval
			var ds = Container.VehicleSpeed.IsEqual(0)
				? Constants.SimulationSettings.DriveOffDistance
				: Constants.SimulationSettings.TargetTimeInterval * Container.VehicleSpeed;

			var loopCount = 0;
			IResponse response;
			do {
				response = CyclePort.Request(AbsTime, ds);
				response.Switch().
					Case<ResponseSuccess>(r => { dt = r.SimulationInterval; }).
					Case<ResponseDrivingCycleDistanceExceeded>(r => {
						if (r.MaxDistance.IsSmallerOrEqual(0)) {
							throw new VectoSimulationException("DistanceExceeded, MaxDistance is invalid: {0}", r.MaxDistance);
						}
						ds = r.MaxDistance;
					}).
					Case<ResponseCycleFinished>(r => {
						FinishedWithoutErrors = true;
						Log.Info("========= Driving Cycle Finished");
					}).
					Default(r => { throw new VectoException("DistanceRun got an unexpected response: {0}", r); });
				if (loopCount++ > Constants.SimulationSettings.MaximumIterationCountForSimulationStep) {
					throw new VectoSimulationException("Maximum iteration count for a single simulation interval reached! Aborting!");
				}
			} while (!(response is ResponseSuccess || response is ResponseCycleFinished));

			return response;
		}

		protected override IResponse Initialize()
		{
			Log.Info("Starting {0}", RunIdentifier);
			return CyclePort.Initialize();
		}
	}
}