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

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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
		protected IEnumerator<DrivingCycleData.DrivingCycleEntry> RightSample { get; set; }
		protected IEnumerator<DrivingCycleData.DrivingCycleEntry> LeftSample { get; set; }

		protected Second AbsTime { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PowertrainDrivingCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
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
			// cycle finished (no more entries in cycle)
			if (RightSample.Current == null) {
				return new ResponseCycleFinished { Source = this };
			}

			// interval exceeded
			if ((absTime + dt).IsGreater(RightSample.Current.Time)) {
				return new ResponseFailTimeInterval {
					Source = this,
					DeltaT = (absTime + dt) - RightSample.Current.Time
				};
			}

			var request = NextComponent.Request(absTime, dt, LeftSample.Current.Torque, LeftSample.Current.AngularVelocity);
			request.Switch()
				.Case<ResponseSuccess>(() => {
					// if response successfull update internal AbsTime for DoCommit
					AbsTime = absTime + dt;
				})
				.Default(r => {
					throw new UnexpectedResponseException("PowertrainDrivingCycle received an unexpected response.", r);
				});

			return request;
		}

		public IResponse Initialize()
		{
			var first = Data.Entries.First();

			AbsTime = first.Time;
			var response = NextComponent.Initialize(first.Torque, first.AngularVelocity);
			response.AbsTime = AbsTime;
			return response;
		}

		public string CycleName
		{
			get { return Data.Name; }
		}

		public double Progress
		{
			get { return AbsTime.Value() / Data.Entries.Last().Time.Value(); }
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
			if (AbsTime.IsGreater(RightSample.Current.Time)) {
				RightSample.MoveNext();
				LeftSample.MoveNext();
			}
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

	/// <summary>
	/// Driving Cycle for the PWheel driving cycle.
	/// </summary>
	public class PWheelCycle : PowertrainDrivingCycle, IDriverInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PWheelCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		/// <param name="axleRatio">The axle ratio.</param>
		/// <param name="gears">The gears.</param>
		public PWheelCycle(IVehicleContainer container, DrivingCycleData cycle, double axleRatio,
			IDictionary<uint, double> gears) : base(container, cycle)
		{
			foreach (var entry in Data.Entries) {
				entry.AngularVelocity = entry.AngularVelocity / (axleRatio * gears[entry.Gear]);
				entry.Torque = entry.PWheel / entry.AngularVelocity;
			}
		}

		/// <summary>
		/// True if the angularVelocity at the wheels is 0.
		/// </summary>
		public bool VehicleStopped
		{
			get { return LeftSample.Current.AngularVelocity.IsEqual(0); }
		}

		/// <summary>
		/// Always Driving.
		/// </summary>
		public DrivingBehavior DrivingBehavior
		{
			get { return DrivingBehavior.Driving; }
		}
	}
}