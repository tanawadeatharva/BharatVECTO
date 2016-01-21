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
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	/// Represents a driving cycle which directly is connected to the powertrain (e.g. engine, or axle gear).
	/// </summary>
	public class PowertrainDrivingCycle : VectoSimulationComponent, IPowertrainSimulation, ITnInPort,
		ISimulationOutPort
	{
		protected DrivingCycleData Data;
		protected ITnOutPort NextComponent;
		private IEnumerator<DrivingCycleData.DrivingCycleEntry> RightSample { get; set; }
		private IEnumerator<DrivingCycleData.DrivingCycleEntry> LeftSample { get; set; }

		protected Second AbsTime { get; set; }

		public PowertrainDrivingCycle(IVehicleContainer container, DrivingCycleData cycle) : base(container)
		{
			Data = cycle;
			LeftSample = Data.Entries.GetEnumerator();
			LeftSample.MoveNext();

			RightSample = Data.Entries.GetEnumerator();
			RightSample.MoveNext();
			RightSample.MoveNext();
		}

		#region ITnInProvider

		public ITnInPort InPort()
		{
			return this;
		}

		#endregion

		#region ISimulationOutProvider

		public ISimulationOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region ISimulationOutPort

		public IResponse Request(Second absTime, Meter ds)
		{
			throw new VectoSimulationException("Powertrain Only Simulation can not handle distance request.");
		}

		IResponse ISimulationOutPort.Request(Second absTime, Second dt)
		{
			//todo: change to variable time steps
			var index = (int)Math.Floor(absTime.Value());
			if (index >= Data.Entries.Count) {
				return new ResponseCycleFinished();
			}
			AbsTime = absTime;
			return NextComponent.Request(absTime, dt, Data.Entries[index].Torque, Data.Entries[index].AngularVelocity);
		}

		public IResponse Initialize()
		{
			var index = 0;
			return NextComponent.Initialize(Data.Entries[index].Torque, Data.Entries[index].AngularVelocity);
		}

		public string CycleName
		{
			get { return Data.Name; }
		}

		public double Progress
		{
			get { return AbsTime.Value() / Data.Entries.Last().Time.Value(); }
		}


		public Meter StartDistance
		{
			get { return 0.SI<Meter>(); }
		}

		#endregion

		#region ITnInPort

		void ITnInPort.Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container) {}

		protected override void DoCommitSimulationStep()
		{
			LeftSample.MoveNext();
			RightSample.MoveNext();
		}

		#endregion

		public CycleData CycleData()
		{
			return new CycleData {
				AbsTime = LeftSample.Current.Time,
				AbsDistance = null,
				LeftSample = LeftSample.Current,
				RightSample = RightSample.Current,
			};
		}
	}

	public class PWheelCycle : PowertrainDrivingCycle
	{
		public PWheelCycle(IVehicleContainer container, DrivingCycleData cycle, double axleRatio,
			IDictionary<uint, double> gears) : base(container, cycle)
		{
			gears[0] = 1;
			foreach (var entry in Data.Entries) {
				entry.AngularVelocity *= axleRatio * gears[entry.Gear];
				entry.Torque = entry.PWheel / entry.AngularVelocity;
			}
		}
	}
}