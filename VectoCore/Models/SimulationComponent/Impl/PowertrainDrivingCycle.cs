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
using TUGraz.VectoCore.Configuration;
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

			return DoHandleRequest(absTime, dt, LeftSample.Current.AngularVelocity);
		}

		protected IResponse DoHandleRequest(Second absTime, Second dt, PerSecond angularVelocity)
		{
			var response = NextComponent.Request(absTime, dt, LeftSample.Current.Torque, angularVelocity);

			if (response is ResponseGearShift) {
				response = NextComponent.Request(absTime, dt, LeftSample.Current.Torque, angularVelocity);
			}

			response.Switch()
				.Case<ResponseUnderload>(r => {
					var torqueInterval = -r.Delta / (angularVelocity.IsEqual(0) ? 10.RPMtoRad() : angularVelocity);
					var torque = SearchAlgorithm.Search(LeftSample.Current.Torque, r.Delta, torqueInterval,
						getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
						evaluateFunction: t => NextComponent.Request(absTime, dt, t, angularVelocity, true),
						criterion: y =>
							((ResponseDryRun)y).DeltaDragLoad.IsEqual(0.SI<Watt>(), Constants.SimulationSettings.EnginePowerSearchTolerance));
					response = NextComponent.Request(absTime, dt, torque, angularVelocity);
				})
				.Case<ResponseOverload>(r => {
					angularVelocity = SearchAlgorithm.Search(angularVelocity, r.Delta, 50.RPMtoRad(),
						getYValue: result => ((ResponseDryRun)result).DeltaFullLoad,
						evaluateFunction: n => NextComponent.Request(absTime, dt, LeftSample.Current.Torque, n, true),
						criterion: y => ((ResponseDryRun)y).DeltaFullLoad.Abs() < Constants.SimulationSettings.EnginePowerSearchTolerance);
					response = NextComponent.Request(absTime, dt, LeftSample.Current.Torque, angularVelocity);
				})
				.Case<ResponseSuccess>(() => { })
				.Default(
					r => { throw new UnexpectedResponseException("MeasuredSpeedDrivingCycle received an unexpected response.", r); });

			if (!(response is ResponseSuccess)) {
				throw new UnexpectedResponseException("MeasuredSpeedDrivingCycle received an unexpected response.", response);
			}

			AbsTime = absTime + dt;
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
	public class PWheelCycle : PowertrainDrivingCycle, IDriverInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="PWheelCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		/// <param name="axleRatio">The axle ratio.</param>
		/// <param name="gearRatios"></param>
		public PWheelCycle(IVehicleContainer container, DrivingCycleData cycle, double axleRatio,
			IDictionary<uint, double> gearRatios) : base(container, cycle)
		{
			// just to ensure that null-gear has ratio 1
			gearRatios[0] = 1;

			foreach (var entry in Data.Entries) {
				entry.WheelAngularVelocity = entry.AngularVelocity / (axleRatio * gearRatios[entry.Gear]);
				entry.Torque = entry.PWheel / entry.WheelAngularVelocity;
			}
		}

		public override IResponse Request(Second absTime, Second dt)
		{
			if (RightSample.Current == null) {
				return new ResponseCycleFinished { Source = this };
			}

			// interval exceeded
			if ((absTime + dt).IsGreater(RightSample.Current.Time)) {
				return new ResponseFailTimeInterval {
					AbsTime = absTime,
					Source = this,
					DeltaT = RightSample.Current.Time - absTime
				};
			}

			return DoHandleRequest(absTime, dt, LeftSample.Current.WheelAngularVelocity);
		}


		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.P_wheel_in] = LeftSample.Current.PWheel;
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
	}

	/// <summary>
	/// Driving Cycle for the Measured Speed Gear driving cycle.
	/// </summary>
	public class MeasuredSpeedDrivingCycle : VectoSimulationComponent, IDriverInfo, IDrivingCycleInfo,
		IDriverDemandInProvider, IDriverDemandInPort, ISimulationOutProvider, ISimulationOutPort
	{
		protected DrivingCycleData Data;
		protected IDriverDemandOutPort NextComponent;
		private bool _isInitializing;
		protected IEnumerator<DrivingCycleData.DrivingCycleEntry> RightSample { get; set; }
		protected IEnumerator<DrivingCycleData.DrivingCycleEntry> LeftSample { get; set; }

		protected Second AbsTime { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="PowertrainDrivingCycle"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		/// <param name="cycle">The cycle.</param>
		public MeasuredSpeedDrivingCycle(IVehicleContainer container, DrivingCycleData cycle)
			: base(container)
		{
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
			Log.Fatal("MeasuredSpeed Cycle can not handle distance request.");
			throw new VectoSimulationException("MeasuredSpeed Cycle can not handle distance request.");
		}

		public virtual IResponse Request(Second absTime, Second dt)
		{
			// cycle finished
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

			// calc acceleration from speed diff vehicle to cycle
			var deltaV = RightSample.Current.VehicleTargetSpeed - DataBus.VehicleSpeed;
			var deltaT = RightSample.Current.Time - LeftSample.Current.Time;
			var acceleration = deltaV / deltaT;
			var gradient = LeftSample.Current.RoadGradient;

			var response = NextComponent.Request(absTime, dt, acceleration, gradient);
			if (response is ResponseGearShift) {
				response = NextComponent.Request(absTime, dt, acceleration, gradient);
			}

			response.Switch()
				.Case<ResponseUnderload>(r => {
					DataBus.BrakePower = SearchAlgorithm.Search(DataBus.BrakePower, r.Delta, -r.Delta,
						getYValue: result => ((ResponseDryRun)result).DeltaDragLoad,
						evaluateFunction: x => {
							DataBus.BrakePower = x;
							return NextComponent.Request(absTime, dt, acceleration, gradient, true);
						},
						criterion: y =>
							((ResponseDryRun)y).DeltaDragLoad.IsEqual(0.SI<Watt>(), Constants.SimulationSettings.EnginePowerSearchTolerance));
					response = NextComponent.Request(absTime, dt, acceleration, gradient);
				})
				.Case<ResponseOverload>(r => {
					acceleration = SearchAlgorithm.Search(acceleration, r.Delta,
						Constants.SimulationSettings.OperatingPointInitialSearchIntervalAccelerating,
						getYValue: result => ((ResponseDryRun)result).DeltaFullLoad,
						evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
						criterion: y => ((ResponseDryRun)y).DeltaFullLoad.Abs() < Constants.SimulationSettings.EnginePowerSearchTolerance);
					response = NextComponent.Request(absTime, dt, acceleration, gradient);
				})
				.Case<ResponseSuccess>(() => { })
				.Default(
					r => { throw new UnexpectedResponseException("MeasuredSpeedDrivingCycle received an unexpected response.", r); });

			if (!(response is ResponseSuccess)) {
				throw new UnexpectedResponseException("MeasuredSpeedDrivingCycle received an unexpected response.", response);
			}

			AbsTime = absTime + dt;
			return response;
		}

		public IResponse Initialize()
		{
			var first = Data.Entries.First();

			AbsTime = first.Time;

			_isInitializing = true;

			IResponse response;
			response = NextComponent.Initialize(first.VehicleTargetSpeed, first.RoadGradient);
			if (!(response is ResponseSuccess)) {
				throw new UnexpectedResponseException("Couldn't find start gear.", response);
			}

			_isInitializing = false;

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
			get { return !_isInitializing && LeftSample.Current.VehicleTargetSpeed.IsEqual(0); }
		}

		public DrivingBehavior DrivingBehavior
		{
			get { return DrivingBehavior.Driving; }
		}
	}
}