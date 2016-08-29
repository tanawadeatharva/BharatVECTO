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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	/// <summary>
	///     Class representing one Distance Based Driving Cycle
	/// </summary>
	public sealed class DistanceBasedDrivingCycle : StatefulProviderComponent
		<DistanceBasedDrivingCycle.DrivingCycleState, ISimulationOutPort, IDrivingCycleInPort, IDrivingCycleOutPort>,
		IDrivingCycle, ISimulationOutPort, IDrivingCycleInPort, IDisposable
	{
		private const double LookaheadTimeSafetyMargin = 1.5;
		private readonly DrivingCycleData _data;
		internal readonly DrivingCycleEnumerator CycleIntervalIterator;
		private bool _intervalProlonged;

		public DistanceBasedDrivingCycle(IVehicleContainer container, DrivingCycleData cycle) : base(container)
		{
			_data = cycle;
			CycleIntervalIterator = new DrivingCycleEnumerator(_data);
			CycleStartDistance = _data.Entries.Count > 0 ? _data.Entries.First().Distance : 0.SI<Meter>();

			var first = _data.Entries.First();
			PreviousState = new DrivingCycleState {
				AbsTime = 0.SI<Second>(),
				WaitTime = 0.SI<Second>(),
				Distance = first.Distance,
				Altitude = first.Altitude,
			};
			CurrentState = PreviousState.Clone();
		}

		public IResponse Initialize()
		{
			if (CycleIntervalIterator.LeftSample.VehicleTargetSpeed.IsEqual(0)) {
				var retVal = NextComponent.Initialize(DataBus.StartSpeed,
					CycleIntervalIterator.LeftSample.RoadGradient, DataBus.StartAcceleration);
				if (!(retVal is ResponseSuccess)) {
					throw new UnexpectedResponseException("Couldn't find start gear.", retVal);
				}
			}

			return NextComponent.Initialize(CycleIntervalIterator.LeftSample.VehicleTargetSpeed,
				CycleIntervalIterator.LeftSample.RoadGradient);
		}

		public IResponse Request(Second absTime, Second dt)
		{
			throw new NotImplementedException("Distance Based Driving Cycle does not support time requests.");
		}

		public IResponse Request(Second absTime, Meter ds)
		{
			if (CycleIntervalIterator.LeftSample.Distance.IsEqual(PreviousState.Distance.Value())) {
				// exactly on an entry in the cycle...
				if (!CycleIntervalIterator.LeftSample.StoppingTime.IsEqual(0)
					&& CycleIntervalIterator.LeftSample.StoppingTime > PreviousState.WaitTime) {
					// stop for certain time unless we've already waited long enough...
					if (!CycleIntervalIterator.LeftSample.VehicleTargetSpeed.IsEqual(0)) {
						Log.Warn("Stopping Time requested in cycle but target-velocity not zero. distance: {0}, target speed: {1}",
							CycleIntervalIterator.LeftSample.StoppingTime, CycleIntervalIterator.LeftSample.VehicleTargetSpeed);
						throw new VectoSimulationException("Stopping Time only allowed when target speed is zero!");
					}
					var dt = CycleIntervalIterator.LeftSample.StoppingTime - PreviousState.WaitTime;
					if (CycleIntervalIterator.LeftSample.StoppingTime.IsGreater(3 * Constants.SimulationSettings.TargetTimeInterval)) {
						// split into 3 parts
						if (PreviousState.WaitTime.IsEqual(0)) {
							dt = Constants.SimulationSettings.TargetTimeInterval;
						} else {
							if (dt > Constants.SimulationSettings.TargetTimeInterval) {
								dt -= Constants.SimulationSettings.TargetTimeInterval;
							}
						}
					}

					CurrentState.Response = DriveTimeInterval(absTime, dt);
					return CurrentState.Response;
				}
			}
			if (CycleIntervalIterator.LastEntry && PreviousState.Distance.IsEqual(CycleIntervalIterator.RightSample.Distance)) {
				CurrentState.Response = new ResponseCycleFinished();
				return CurrentState.Response;
			}

			var nextSpeedChange = GetSpeedChangeWithinSimulationInterval(ds);
			if (nextSpeedChange == null || ds.IsSmallerOrEqual(nextSpeedChange - PreviousState.Distance)) {
				if (nextSpeedChange == null || DataBus.VehicleSpeed.IsEqual(0.SI<MeterPerSecond>())) {
					CurrentState.Response = DriveDistance(absTime, ds);
					return CurrentState.Response;
				}
				var remainingDistance = nextSpeedChange - PreviousState.Distance - ds;
				var estimatedRemainingTime = remainingDistance / DataBus.VehicleSpeed;
				if (_intervalProlonged || remainingDistance.IsEqual(0.SI<Meter>()) ||
					estimatedRemainingTime.IsGreater(Constants.SimulationSettings.LowerBoundTimeInterval)) {
					CurrentState.Response = DriveDistance(absTime, ds);
					return CurrentState.Response;
				}
				Log.Debug("Extending distance by {0} to next sample point. ds: {1} new ds: {2}", remainingDistance, ds,
					nextSpeedChange - PreviousState.Distance);
				_intervalProlonged = true;
				CurrentState.Response = new ResponseDrivingCycleDistanceExceeded {
					Source = this,
					MaxDistance = nextSpeedChange - PreviousState.Distance
				};
				return CurrentState.Response;
			}
			// only drive until next sample point in cycle with speed change
			Log.Debug("Limiting distance to next sample point {0}",
				CycleIntervalIterator.RightSample.Distance - PreviousState.Distance);
			CurrentState.Response = new ResponseDrivingCycleDistanceExceeded {
				Source = this,
				MaxDistance = nextSpeedChange - PreviousState.Distance
			};
			return CurrentState.Response;
		}

		private IResponse DriveTimeInterval(Second absTime, Second dt)
		{
			CurrentState.AbsTime = PreviousState.AbsTime + dt;
			CurrentState.WaitTime = PreviousState.WaitTime + dt;
			CurrentState.Gradient = ComputeGradient(0.SI<Meter>());
			CurrentState.VehicleTargetSpeed = CycleIntervalIterator.LeftSample.VehicleTargetSpeed;

			return NextComponent.Request(absTime, dt, CycleIntervalIterator.LeftSample.VehicleTargetSpeed, CurrentState.Gradient);
		}

		private IResponse DriveDistance(Second absTime, Meter ds)
		{
			var nextSpeedChanges = LookAhead(Constants.SimulationSettings.BrakeNextTargetDistance);
			if (nextSpeedChanges.Count > 0 && !CurrentState.RequestToNextSamplePointDone) {
				CurrentState.RequestToNextSamplePointDone = true;
				Log.Debug("current distance is close to the next speed change: {0}",
					nextSpeedChanges.First().Distance - PreviousState.Distance);
				return new ResponseDrivingCycleDistanceExceeded {
					Source = this,
					MaxDistance = Constants.SimulationSettings.BrakeNextTargetDistance
				};
			}

			CurrentState.Distance = PreviousState.Distance + ds;
			CurrentState.SimulationDistance = ds;
			CurrentState.VehicleTargetSpeed = CycleIntervalIterator.LeftSample.VehicleTargetSpeed;
			CurrentState.Gradient = ComputeGradient(ds);

			var retVal = NextComponent.Request(absTime, ds, CurrentState.VehicleTargetSpeed, CurrentState.Gradient);
			retVal.Switch()
				.Case<ResponseFailTimeInterval>(
					r => {
						retVal = NextComponent.Request(absTime, r.DeltaT, 0.SI<MeterPerSecond>(), CurrentState.Gradient);
						retVal = NextComponent.Request(absTime, ds, CurrentState.VehicleTargetSpeed, CurrentState.Gradient);
					});
			return retVal;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.dist] = CurrentState.Distance; // (CurrentState.Distance + PreviousState.Distance) / 2.0;
			container[ModalResultField.simulationDistance] = CurrentState.SimulationDistance;
			container[ModalResultField.v_targ] = CurrentState.VehicleTargetSpeed;
			container[ModalResultField.grad] = (Math.Tan(CurrentState.Gradient.Value()) * 100).SI<Scalar>();
			container[ModalResultField.altitude] = CurrentState.Altitude;
		}

		protected override void DoCommitSimulationStep()
		{
			if (!(CurrentState.Response is ResponseSuccess)) {
				throw new VectoSimulationException("Previous request did not succeed!");
			}

			PreviousState = CurrentState;
			CurrentState = CurrentState.Clone();
			_intervalProlonged = false;

			if (!CycleIntervalIterator.LeftSample.StoppingTime.IsEqual(0) &&
				CycleIntervalIterator.LeftSample.StoppingTime.IsEqual(PreviousState.WaitTime)) {
				// we needed to stop at the current interval in the cycle and have already waited enough time, move on..
				CycleIntervalIterator.MoveNext();
			}

			// separately test for equality and greater than to have tolerance for equality comparison
			if (CycleIntervalIterator.LeftSample.StoppingTime.IsEqual(0)) {
				while (CycleIntervalIterator.LeftSample.StoppingTime.IsEqual(0) &&
						CurrentState.Distance.IsGreaterOrEqual(CycleIntervalIterator.RightSample.Distance) &&
						!CycleIntervalIterator.LastEntry) {
					// we have reached the end of the current interval in the cycle, move on...
					CycleIntervalIterator.MoveNext();
				}
			} else {
				if (CycleIntervalIterator.LeftSample.StoppingTime.IsEqual(PreviousState.WaitTime)) {
					// we needed to stop at the current interval in the cycle and have already waited enough time, move on..
					CycleIntervalIterator.MoveNext();
				}
			}
		}

		private Radian ComputeGradient(Meter ds)
		{
			//var leftSamplePoint = CycleIntervalIterator.LeftSample;

			var cycleIterator = CycleIntervalIterator.Clone();
			while (cycleIterator.RightSample.Distance < PreviousState.Distance + ds && !cycleIterator.LastEntry) {
				cycleIterator.MoveNext();
			}
			var leftSamplePoint = cycleIterator.LeftSample;
			var rightSamplePoint = cycleIterator.RightSample;

			if (leftSamplePoint.Distance.IsEqual(rightSamplePoint.Distance)) {
				return leftSamplePoint.RoadGradient;
			}
			if (ds.IsEqual(0.SI<Meter>())) {
				return leftSamplePoint.RoadGradient;
			}
			CurrentState.Altitude = VectoMath.Interpolate(leftSamplePoint.Distance, rightSamplePoint.Distance,
				leftSamplePoint.Altitude, rightSamplePoint.Altitude, PreviousState.Distance + ds);

			var gradient = VectoMath.InclinationToAngle(((CurrentState.Altitude - PreviousState.Altitude) /
														(ds)).Value());
			//return 0.SI<Radian>();
			return gradient;
		}

		private Meter GetSpeedChangeWithinSimulationInterval(Meter ds)
		{
			var leftSamplePoint = CycleIntervalIterator.LeftSample;
			var cycleIterator = CycleIntervalIterator.Clone();

			do {
				if (!leftSamplePoint.VehicleTargetSpeed.IsEqual(cycleIterator.RightSample.VehicleTargetSpeed)) {
					return cycleIterator.RightSample.Distance;
				}
			} while (cycleIterator.RightSample.Distance < PreviousState.Distance + ds && cycleIterator.MoveNext());
			if (cycleIterator.LastEntry) {
				return cycleIterator.RightSample.Distance;
			}
			return null;
		}

		/// <summary>
		/// Progress of the distance in the driving cycle.
		/// </summary>
		public double Progress
		{
			get
			{
				return _data.Entries.Count > 0
					? (CurrentState.Distance.Value() - _data.Entries.First().Distance.Value()) /
					(_data.Entries.Last().Distance.Value() - _data.Entries.First().Distance.Value())
					: 0;
			}
		}

		public Meter CycleStartDistance { get; internal set; }

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			var retVal = new List<DrivingCycleData.DrivingCycleEntry>();

			var cycleIterator = CycleIntervalIterator.Clone();
			var velocity = cycleIterator.LeftSample.VehicleTargetSpeed;

			do {
				if (cycleIterator.RightSample.VehicleTargetSpeed.IsEqual(velocity)) {
					continue;
				}
				var lookaheadEntry = retVal.Find(x => x.Distance == cycleIterator.RightSample.Distance);
				if (lookaheadEntry != null) {
					// an entry may occur twice when vehicle stops (one entry with v=0 and the other with drive on after stop)
					// only use the one with min. speed
					if (cycleIterator.RightSample.VehicleTargetSpeed < lookaheadEntry.VehicleTargetSpeed) {
						retVal.Remove(lookaheadEntry);
						retVal.Add(cycleIterator.RightSample); // TODO: MQ 2016-05-13: use clone of iterator here?
					}
				} else {
					retVal.Add(cycleIterator.RightSample); // TODO: MQ 2016-05-13: use clone of iterator here?
				}
				velocity = cycleIterator.RightSample.VehicleTargetSpeed;
				if (velocity.IsEqual(0.KMPHtoMeterPerSecond())) {
					// do not look beyond vehicle stop
					break;
				}
			} while (cycleIterator.MoveNext() && cycleIterator.RightSample.Distance < PreviousState.Distance + lookaheadDistance);
			if (retVal.Count > 0) {
				retVal = retVal.Where(x => x.Distance <= PreviousState.Distance + lookaheadDistance).ToList();
				retVal.Sort((x, y) => x.Distance.CompareTo(y.Distance));
			}
			return retVal;
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return LookAhead(LookaheadTimeSafetyMargin * DataBus.VehicleSpeed * time);
		}

		public CycleData CycleData
		{
			get
			{
				return new CycleData {
					AbsTime = CurrentState.AbsTime,
					AbsDistance = CurrentState.Distance,
					LeftSample = CycleIntervalIterator.LeftSample,
					RightSample = CycleIntervalIterator.RightSample
				};
			}
		}

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			var absDistance = CurrentState.Distance + distance;
			var myIterator = CycleIntervalIterator.Clone();

			if (absDistance > _data.Entries.Last().Distance) {
				return ExtrapolateCycleEntry(absDistance, _data.Entries.Last());
			}
			while (myIterator.RightSample.Distance < absDistance) {
				myIterator.MoveNext();
			}

			return InterpolateCycleEntry(absDistance, myIterator.RightSample);
		}

		private DrivingCycleData.DrivingCycleEntry InterpolateCycleEntry(Meter absDistance,
			DrivingCycleData.DrivingCycleEntry lookahead)
		{
			var retVal = new DrivingCycleData.DrivingCycleEntry(lookahead) {
				Distance = absDistance,
				Altitude = VectoMath.Interpolate(CurrentState.Distance, lookahead.Distance, CurrentState.Altitude,
					lookahead.Altitude, absDistance)
			};

			retVal.RoadGradient =
				((retVal.Altitude - CurrentState.Altitude) / (absDistance - CurrentState.Distance)).Value().SI<Radian>();

			return retVal;
		}

		private DrivingCycleData.DrivingCycleEntry ExtrapolateCycleEntry(Meter absDistance,
			DrivingCycleData.DrivingCycleEntry lookahead)
		{
			var retVal = new DrivingCycleData.DrivingCycleEntry(lookahead) {
				Distance = absDistance,
				Altitude = lookahead.Altitude + lookahead.RoadGradient * (absDistance - lookahead.Distance),
			};

			retVal.RoadGradient =
				((retVal.Altitude - CurrentState.Altitude) / (absDistance - CurrentState.Distance)).Value().SI<Radian>();

			return retVal;
		}

		public Meter Altitude
		{
			get { return PreviousState.Altitude; }
		}

		internal void SetDriveOffDistance(Meter startDistance)
		{
			while (CycleIntervalIterator.MoveNext() && CycleIntervalIterator.RightSample.Distance < startDistance) {}
			PreviousState.Distance = startDistance;
			CycleStartDistance = startDistance;
		}

		public sealed class DrivingCycleEnumerator : IEnumerator<DrivingCycleData.DrivingCycleEntry>
		{
			private int _currentCycleIndex;
			private readonly DrivingCycleData _data;

			public DrivingCycleEnumerator(DrivingCycleData data)
			{
				_currentCycleIndex = 0;
				_data = data;
				LastEntry = false;
			}

			public DrivingCycleEnumerator Clone()
			{
				return new DrivingCycleEnumerator(_data) {
					_currentCycleIndex = _currentCycleIndex,
					LastEntry = LastEntry
				};
			}

			public DrivingCycleData.DrivingCycleEntry Current
			{
				get { return LeftSample; }
			}

			public DrivingCycleData.DrivingCycleEntry Next
			{
				get { return RightSample; }
			}

			public DrivingCycleData.DrivingCycleEntry LeftSample
			{
				get { return _data.Entries[_currentCycleIndex]; }
			}

			public DrivingCycleData.DrivingCycleEntry RightSample
			{
				get { return _currentCycleIndex + 1 >= _data.Entries.Count ? null : _data.Entries[_currentCycleIndex + 1]; }
			}

			public bool LastEntry { get; private set; }

			object System.Collections.IEnumerator.Current
			{
				get { return LeftSample; }
			}

			public bool MoveNext()
			{
				// cycleIndex has to be max. next to last (so that rightSample is still valid.
				if (_currentCycleIndex >= _data.Entries.Count - 2) {
					LastEntry = true;
					return false;
				}
				_currentCycleIndex++;
				if (_currentCycleIndex == _data.Entries.Count - 2) {
					LastEntry = true;
				}

				return true;
			}

			public void Reset()
			{
				_currentCycleIndex = 0;
			}

			public void Dispose() {}
		}

		public sealed class DrivingCycleState
		{
			public DrivingCycleState Clone()
			{
				return new DrivingCycleState {
					AbsTime = AbsTime,
					Distance = Distance,
					VehicleTargetSpeed = VehicleTargetSpeed,
					Altitude = Altitude,
					// WaitTime is not cloned on purpose!
					WaitTime = 0.SI<Second>(),
					Response = null
				};
			}

			public Second AbsTime;

			public Meter Distance;

			public Second WaitTime;

			public MeterPerSecond VehicleTargetSpeed;

			public Meter Altitude;

			public Radian Gradient;

			public IResponse Response;

			public bool RequestToNextSamplePointDone;

			public Meter SimulationDistance;
		}

		public void Dispose()
		{
			CycleIntervalIterator.Dispose();
		}
	}
}