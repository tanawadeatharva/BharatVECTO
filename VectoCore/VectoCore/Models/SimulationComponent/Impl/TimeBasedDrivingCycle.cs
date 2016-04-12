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
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	///     Class representing one Time Based Driving Cycle
	/// </summary>
	public class TimeBasedDrivingCycle : VectoSimulationComponent, IDrivingCycle,
		IDrivingCycleInPort, ISimulationOutPort
	{
		protected DrivingCycleData Data;
		protected IDrivingCycleOutPort NextComponent;

		public TimeBasedDrivingCycle(IVehicleContainer container, DrivingCycleData cycle) : base(container)
		{
			Data = cycle;
		}

		#region ISimulationOutProvider

		public ISimulationOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region IDrivingCycleInProvider

		public IDrivingCycleInPort InPort()
		{
			return this;
		}

		#endregion

		#region ISimulationOutPort

		IResponse ISimulationOutPort.Request(Second absTime, Meter ds)
		{
			throw new NotImplementedException();
		}

		public IResponse Request(Second absTime, Second dt)
		{
			//todo: change to variable time steps
			var index = (int)Math.Floor(absTime.Value());
			if (index >= Data.Entries.Count) {
				return new ResponseCycleFinished();
			}

			// TODO: implement request and response handling!!
			var dx = 0.SI<Meter>();
			return NextComponent.Request(absTime, dt, Data.Entries[index].VehicleTargetSpeed,
				Data.Entries[index].RoadGradient);
		}

		public IResponse Initialize()
		{
			// nothing to initialize here...
			// TODO: _outPort.initialize();
			throw new NotImplementedException();
		}


		public double Progress
		{
			get { throw new NotImplementedException(); }
		}

		public Meter StartDistance
		{
			get { return 0.SI<Meter>(); }
		}

		#endregion

		#region IDrivingCycleInPort

		void IDrivingCycleInPort.
			Connect(IDrivingCycleOutPort
				other)
		{
			NextComponent = other;
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			// TODO: write data...
		}

		protected override void DoCommitSimulationStep()
		{
			// TODO: commit step
		}

		#endregion

		public CycleData CycleData
		{
			get
			{
				return new CycleData {
					AbsTime = 0.SI<Second>(),
					AbsDistance = 0.SI<Meter>(),
					LeftSample = null,
					RightSample = null
				};
			}
		}
	}
}