using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
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
	/// Driving Cycle for the Measured Speed Gear driving cycle.
	/// </summary>
	public class MeasuredSpeedDrivingCycle :
		StatefulProviderComponent
			<MeasuredSpeedDrivingCycle.DrivingCycleState, ISimulationOutPort, IDriverDemandInPort, IDriverDemandOutPort>,
		IDriverInfo, IDrivingCycleInfo, IMileageCounter, IDriverDemandInProvider, IDriverDemandInPort, ISimulationOutProvider,
		ISimulationOutPort
	{
		public class DrivingCycleState
		{
			public DrivingCycleState Clone()
			{
				return new DrivingCycleState {
					Distance = Distance,
					VehicleSpeed = VehicleSpeed,
				};
			}

			public Meter Distance;
			public MeterPerSecond VehicleSpeed;
			public Meter SimulationDistance;
			public MeterPerSquareSecond Acceleration;
		}

		protected DrivingCycleData Data;
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

			var first = LeftSample.Current;
			PreviousState = new DrivingCycleState {
				Distance = 0.SI<Meter>(),
				VehicleSpeed = first.VehicleTargetSpeed,
			};
			CurrentState = PreviousState.Clone();
		}

		public IResponse Initialize()
		{
			var first = Data.Entries.First();

			AbsTime = first.Time;

			_isInitializing = true;

			var response = NextComponent.Initialize(first.VehicleTargetSpeed, first.RoadGradient);
			if (!(response is ResponseSuccess)) {
				throw new UnexpectedResponseException("Couldn't find start gear.", response);
			}

			_isInitializing = false;

			response.AbsTime = AbsTime;
			return response;
		}

		public IResponse Request(Second absTime, Meter ds)
		{
			Log.Fatal("MeasuredSpeed Cycle can not handle distance request.");
			throw new VectoSimulationException("MeasuredSpeed Cycle can not handle distance request.");
		}

		public virtual IResponse Request(Second absTime, Second dt)
		{
			var debug = new DebugData();

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
			DriverAcceleration = acceleration;
			DriverBehavior = acceleration < 0
				? DriverBehavior = DrivingBehavior.Braking
				: DriverBehavior = DrivingBehavior.Driving;

			IResponse response;
			var responseCount = 0;
			do {
				response = NextComponent.Request(absTime, dt, acceleration, gradient);
				debug.Add(response);
				response.Switch()
					.Case<ResponseGearShift>(() => response = NextComponent.Request(absTime, dt, acceleration, gradient))
					.Case<ResponseUnderload>(r => {
						DataBus.BrakePower = SearchAlgorithm.Search(DataBus.BrakePower, r.Delta, -r.Delta,
							getYValue: result => RightSample.Current.Gear == 0
								? ((ResponseDryRun)result).GearboxPowerRequest
								: ((ResponseDryRun)result).DeltaDragLoad,
							evaluateFunction: x => {
								DataBus.BrakePower = x;
								return NextComponent.Request(absTime, dt, acceleration, gradient, true);
							},
							criterion: y => RightSample.Current.Gear == 0
								? ((ResponseDryRun)y).GearboxPowerRequest.Value()
								: ((ResponseDryRun)y).DeltaDragLoad.Value());
						Log.Info(
							"Found operating point for braking. absTime: {0}, dt: {1}, acceleration: {2}, gradient: {3}, BrakePower: {4}",
							absTime, dt, acceleration, gradient, DataBus.BrakePower);
						response = NextComponent.Request(absTime, dt, acceleration, gradient);
					})
					.Case<ResponseOverload>(r => {
						acceleration = SearchAlgorithm.Search(acceleration, r.Delta,
							Constants.SimulationSettings.OperatingPointInitialSearchIntervalAccelerating,
							getYValue: result => ((ResponseDryRun)result).DeltaFullLoad,
							evaluateFunction: x => NextComponent.Request(absTime, dt, x, gradient, true),
							criterion:
								y => ((ResponseDryRun)y).DeltaFullLoad.Value());
						Log.Info(
							"Found operating point for driver acceleration. absTime: {0}, dt: {1}, acceleration: {2}, gradient: {3}",
							absTime,
							dt, acceleration, gradient);
						response = NextComponent.Request(absTime, dt, acceleration, gradient);
					})
					.Case<ResponseFailTimeInterval>(r => { dt = r.DeltaT; })
					.Case<ResponseSuccess>()
					.Default(
						r => { throw new UnexpectedResponseException("MeasuredSpeedDrivingCycle received an unexpected response.", r); });
			} while (!(response is ResponseSuccess || response is ResponseFailTimeInterval) && (++responseCount < 10));

			AbsTime = absTime + dt;

			response.SimulationInterval = dt;
			response.Acceleration = acceleration;
			debug.Add(response);

			CurrentState.SimulationDistance = acceleration / 2 * dt * dt + PreviousState.VehicleSpeed * dt;
			CurrentState.Distance = CurrentState.SimulationDistance + PreviousState.Distance;
			CurrentState.VehicleSpeed = acceleration * dt + PreviousState.VehicleSpeed;
			CurrentState.Acceleration = acceleration;

			return response;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.dist] = CurrentState.Distance;
			container[ModalResultField.simulationDistance] = CurrentState.SimulationDistance;
			container[ModalResultField.v_targ] = LeftSample.Current.VehicleTargetSpeed;
			container[ModalResultField.grad] = LeftSample.Current.RoadGradientPercent;
			container[ModalResultField.altitude] = LeftSample.Current.Altitude;
			container[ModalResultField.acc] = CurrentState.Acceleration;
		}

		protected override void DoCommitSimulationStep()
		{
			if ((RightSample.Current == null) || AbsTime.IsGreaterOrEqual(RightSample.Current.Time)) {
				RightSample.MoveNext();
				LeftSample.MoveNext();
			}

			PreviousState = CurrentState;
			CurrentState = CurrentState.Clone();
		}

		public string CycleName
		{
			get { return Data.Name; }
		}

		public double Progress
		{
			get { return AbsTime == null ? 0 : AbsTime.Value() / Data.Entries.Last().Time.Value(); }
		}

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

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return new DrivingCycleData.DrivingCycleEntry(RightSample.Current);
			//throw new System.NotImplementedException();
		}

		public Meter Altitude
		{
			get { return LeftSample.Current.Altitude; }
		}

		public Meter CycleStartDistance
		{
			get { return 0.SI<Meter>(); }
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			throw new NotImplementedException();
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			throw new NotImplementedException();
		}

		public bool VehicleStopped
		{
			get { return !_isInitializing && LeftSample.Current.VehicleTargetSpeed.IsEqual(0); }
		}

		public DrivingBehavior DriverBehavior { get; internal set; }

		public MeterPerSquareSecond DriverAcceleration { get; protected set; }

		public Meter Distance
		{
			get { return CurrentState.Distance; }
		}
	}
}