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

using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
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

		public virtual IResponse Request(Second absTime, Second dt)
		{
			// cycle finished (no more entries in cycle)
			if (LeftSample.Current == null) {
				return new ResponseCycleFinished { Source = this };
			}

			// interval exceeded
			if (RightSample.Current != null && (absTime + dt).IsGreater(RightSample.Current.Time)) {
				return new ResponseFailTimeInterval {
					AbsTime = absTime,
					Source = this,
					DeltaT = RightSample.Current.Time - absTime
				};
			}

			var response = NextComponent.Request(absTime, dt, LeftSample.Current.Torque,
				DataBus.ClutchClosed(absTime) ? LeftSample.Current.AngularVelocity : 0.SI<PerSecond>());

			AbsTime = absTime + dt;

			response.Switch()
				.Case<ResponseUnderload>(r => {
					Log.Warn("PowertrainDrivingCycle got an underload response. Using PDrag.");
					response = new ResponseSuccess();
				})
				.Case<ResponseOverload>(r => {
					Log.Warn("PowertrainDrivingCycle got an underload response. Using PFullLoad.");
					response = new ResponseSuccess();
				})
				.Case<ResponseSuccess>(() => { })
				.Default(
					r => { throw new UnexpectedResponseException("PowertrainDrivingCycle received an unexpected response.", r); });

			return response;
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
			if ((RightSample.Current == null) || AbsTime.IsGreaterOrEqual(RightSample.Current.Time)) {
				RightSample.MoveNext();
				LeftSample.MoveNext();
			}
		}

		#endregion

		public CycleData CycleData
		{
			get
			{
				return new CycleData {
					AbsTime = LeftSample.Current.Time,
					AbsDistance = null,
					LeftSample = LeftSample.Current,
					RightSample = RightSample.Current,
				};
			}
		}
	}

	/// <summary>
	/// Driving Cycle for the PWheel driving cycle.
	/// </summary>
	public class PWheelCycle : PowertrainDrivingCycle, IDriverInfo, IClutchInfo
	{
		public Gearbox Gearbox { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PWheelCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		/// <param name="axleRatio">The axle ratio.</param>
		/// <param name="gearbox"></param>
		public PWheelCycle(IVehicleContainer container, DrivingCycleData cycle, double axleRatio, Gearbox gearbox)
			: base(container, cycle)
		{
			Gearbox = gearbox;

			foreach (var entry in Data.Entries) {
				entry.AngularVelocity = entry.AngularVelocity /
										(axleRatio * (entry.Gear == 0 ? 1 : Gearbox.Data.Gears[entry.Gear].Ratio));
				entry.Torque = entry.PWheel / entry.AngularVelocity;
			}
		}

		public override IResponse Request(Second absTime, Second dt)
		{
			if (RightSample.Current == null) {
				return new ResponseCycleFinished { Source = this };
			}

			Gearbox.Gear = LeftSample.Current.Gear;
			Gearbox.Disengaged = LeftSample.Current.Gear == 0;
			return base.Request(absTime, dt);
		}


		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.Pwheel] = LeftSample.Current.PWheel;
			base.DoWriteModalResults(container);
		}

		#region IDriverInfo

		/// <summary>
		/// True if the angularVelocity at the wheels is 0.
		/// </summary>
		public bool VehicleStopped
		{
			get { return false; }
		}

		/// <summary>
		/// Always Driving.
		/// </summary>
		public DrivingBehavior DrivingBehavior
		{
			get { return DrivingBehavior.Driving; }
		}

		#endregion

		public bool ClutchClosed(Second absTime)
		{
			return LeftSample.Current.Gear != 0;
		}
	}


	/// <summary>
	/// Driving Cycle for the PWheel driving cycle.
	/// </summary>
	public class MeasuredSpeedCycle : VectoSimulationComponent, IDriverInfo, IDrivingCycleInfo, IDriverDemandInProvider,
		IDriverDemandInPort, ISimulationOutProvider, ISimulationOutPort
	{
		protected DrivingCycleData Data;
		protected IDriverDemandOutPort NextComponent;
		protected IEnumerator<DrivingCycleData.DrivingCycleEntry> RightSample { get; set; }
		protected IEnumerator<DrivingCycleData.DrivingCycleEntry> LeftSample { get; set; }
		protected Gearbox Gearbox;

		protected Second AbsTime { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PowertrainDrivingCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		/// <param name="gearbox">the gearbox.</param>
		public MeasuredSpeedCycle(IVehicleContainer container, DrivingCycleData cycle, Gearbox gearbox)
			: base(container)
		{
			Gearbox = gearbox;

			Data = cycle;
			LeftSample = Data.Entries.GetEnumerator();
			LeftSample.MoveNext();

			RightSample = Data.Entries.GetEnumerator();
			RightSample.MoveNext();
			RightSample.MoveNext();
		}

		#region IDriverDemandInProvider

		public IDriverDemandInPort InPort()
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
			throw new VectoSimulationException("MeasuredSpeed Cycle can not handle distance request.");
		}

		public virtual IResponse Request(Second absTime, Second dt)
		{
			// cycle finished (no more entries in cycle)
			if (RightSample.Current == null || LeftSample.Current == null) {
				return new ResponseCycleFinished { AbsTime = absTime, Source = this };
			}

			// interval exceeded
			if (RightSample.Current != null && (absTime + dt).IsGreater(RightSample.Current.Time)) {
				return new ResponseFailTimeInterval {
					AbsTime = absTime,
					Source = this,
					DeltaT = RightSample.Current.Time - absTime
				};
			}

			var delta_v = RightSample.Current.VehicleTargetSpeed - LeftSample.Current.VehicleTargetSpeed;
			var delta_t = RightSample.Current.Time - LeftSample.Current.Time;
			var acceleration = delta_v / delta_t;
			var gradient = LeftSample.Current.RoadGradient;

			Gearbox.Gear = LeftSample.Current.Gear;
			Gearbox.Disengaged = LeftSample.Current.Gear == 0;

			var response = NextComponent.Request(absTime, dt, acceleration, gradient);

			response.Switch()
				//.Case<ResponseUnderload>(r => {
				//	Log.Warn("PowertrainDrivingCycle got an underload response. Using PDrag.");
				//	response = new ResponseSuccess();
				//})
				//.Case<ResponseOverload>(r => {
				//	Log.Warn("PowertrainDrivingCycle got an underload response. Using PFullLoad.");
				//	response = new ResponseSuccess();
				//})
				.Case<ResponseSuccess>(() => { })
				.Default(
					r => { throw new UnexpectedResponseException("PowertrainDrivingCycle received an unexpected response.", r); });

			AbsTime = absTime + dt;
			return response;
		}

		public IResponse Initialize()
		{
			var first = Data.Entries.First();

			AbsTime = first.Time;
			var response = NextComponent.Initialize(first.VehicleTargetSpeed, first.RoadGradient);
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

		#region IDriverDemandInPort

		void IDriverDemandInPort.Connect(IDriverDemandOutPort other)
		{
			NextComponent = other;
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoCommitSimulationStep()
		{
			if ((RightSample.Current == null) || AbsTime.IsGreaterOrEqual(RightSample.Current.Time)) {
				RightSample.MoveNext();
				LeftSample.MoveNext();
			}
		}

		#endregion

		public CycleData CycleData
		{
			get
			{
				return new CycleData {
					AbsTime = LeftSample.Current.Time,
					AbsDistance = null,
					LeftSample = LeftSample.Current,
					RightSample = RightSample.Current,
				};
			}
		}

		protected override void DoWriteModalResults(IModalDataContainer container) {}

		public bool VehicleStopped
		{
			get { return false; }
		}

		public DrivingBehavior DrivingBehavior
		{
			get { return DrivingBehavior.Driving; }
		}
	}
}