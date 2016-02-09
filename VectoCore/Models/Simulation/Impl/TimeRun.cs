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

using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class TimeRun : VectoRun
	{
		public TimeRun(IVehicleContainer container) : base(container) {}

		protected override IResponse DoSimulationStep()
		{
			var loopCount = 0;
			IResponse response;
			do {
				response = CyclePort.Request(AbsTime, dt);
				response.Switch().
					Case<ResponseSuccess>().
					Case<ResponseFailTimeInterval>(r => {
						dt = r.DeltaT;
					}).
					Case<ResponseCycleFinished>(r => {
						FinishedWithoutErrors = true;
						Log.Info("========= Driving Cycle Finished");
					}).
					Default(r => {
						throw new VectoException("TimeRun got an unexpected response: {0}", r);
					});
				if (loopCount++ > Constants.SimulationSettings.MaximumIterationCountForSimulationStep) {
					throw new VectoSimulationException("Maximum iteration count for a single simulation interval reached! Aborting!");
				}
			} while (!(response is ResponseSuccess || response is ResponseCycleFinished));

			return response;
		}

		protected override IResponse Initialize()
		{
			Log.Info("Starting {0}", RunIdentifier);
			var response = CyclePort.Initialize();
			AbsTime = response.AbsTime;
			return response;
		}
	}
}