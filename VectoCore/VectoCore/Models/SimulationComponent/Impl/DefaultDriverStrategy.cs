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
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class DefaultDriverStrategy : LoggingObject, IDriverStrategy
	{
		public static readonly SIBase<Meter> BrakingSafetyMargin = 0.1.SI<Meter>();

		protected internal DrivingBehaviorEntry NextDrivingAction;

		public enum DrivingMode
		{
			DrivingModeDrive,
			DrivingModeBrake,
		}

		protected internal DrivingMode CurrentDrivingMode;

		protected Dictionary<DrivingMode, IDriverMode> DrivingModes = new Dictionary<DrivingMode, IDriverMode>();

		public DefaultDriverStrategy()
		{
			DrivingModes.Add(DrivingMode.DrivingModeDrive, new DriverModeDrive() { DriverStrategy = this });
			DrivingModes.Add(DrivingMode.DrivingModeBrake, new DriverModeBrake() { DriverStrategy = this });
			CurrentDrivingMode = DrivingMode.DrivingModeDrive;
		}

		public IDriverActions Driver { get; set; }

		protected internal DrivingBehaviorEntry BrakeTrigger { get; set; }

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			if (CurrentDrivingMode == DrivingMode.DrivingModeBrake) {
				if (Driver.DataBus.Distance.IsGreaterOrEqual(BrakeTrigger.TriggerDistance, 1e-3.SI<Meter>())) {
					CurrentDrivingMode = DrivingMode.DrivingModeDrive;
					NextDrivingAction = null;
					DrivingModes[CurrentDrivingMode].ResetMode();
					Log.Debug("Switching to DrivingMode DRIVE");
				}
			}
			if (CurrentDrivingMode == DrivingMode.DrivingModeDrive) {
				var currentDistance = Driver.DataBus.Distance;

				//var coasting = LookAheadCoasting(ds);

				UpdateDrivingAction(currentDistance, ds);
				if (NextDrivingAction != null) {
					var remainingDistance = NextDrivingAction.ActionDistance - currentDistance;
					var estimatedNextTimestep = remainingDistance / Driver.DataBus.VehicleSpeed;
					if (remainingDistance.IsEqual(0.SI<Meter>(), Constants.SimulationSettings.DriverActionDistanceTolerance) ||
						estimatedNextTimestep.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval)) {
						CurrentDrivingMode = DrivingMode.DrivingModeBrake;
						DrivingModes[CurrentDrivingMode].ResetMode();
						Log.Debug("Switching to DrivingMode BRAKE");

						BrakeTrigger = NextDrivingAction;
						//break;
					} else if ((currentDistance + ds).IsGreater(NextDrivingAction.ActionDistance)) {
						Log.Debug("Current simulation interval exceeds next action distance at {0}. reducing maxDistance to {1}",
							NextDrivingAction.ActionDistance, NextDrivingAction.ActionDistance - currentDistance);
						return new ResponseDrivingCycleDistanceExceeded() {
							Source = this,
							MaxDistance = NextDrivingAction.ActionDistance - currentDistance
						};
					}
				}
			}

			var retVal = DrivingModes[CurrentDrivingMode].Request(absTime, ds, targetVelocity, gradient);

			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			Driver.DriverBehavior = DrivingBehavior.Halted;
			CurrentDrivingMode = DrivingMode.DrivingModeDrive;
			return Driver.DrivingActionHalt(absTime, dt, targetVelocity, gradient);
		}

		private void UpdateDrivingAction(Meter currentDistance, Meter ds)
		{
			var nextAction = GetNextDrivingAction(currentDistance, ds);
			if (NextDrivingAction == null) {
				if (nextAction != null) {
					// take the new action
					NextDrivingAction = nextAction;
				}
			} else {
				// update action distance for current 'next action'
				if (Driver.DataBus.VehicleSpeed > NextDrivingAction.NextTargetSpeed) {
					var brakingDistance = Driver.ComputeDecelerationDistance(NextDrivingAction.NextTargetSpeed) + BrakingSafetyMargin;
					switch (NextDrivingAction.Action) {
						case DrivingBehavior.Coasting:
							//var coastingDistance = ComputeCoastingDistance(Driver.DataBus.VehicleSpeed, NextDrivingAction.NextTargetSpeed);
							var coastingDistance = ComputeCoastingDistance(Driver.DataBus.VehicleSpeed, NextDrivingAction.CycleEntry);
							NextDrivingAction.CoastingStartDistance = NextDrivingAction.TriggerDistance - coastingDistance;
							NextDrivingAction.BrakingStartDistance = NextDrivingAction.TriggerDistance - brakingDistance;
							break;
						case DrivingBehavior.Braking:
							NextDrivingAction.BrakingStartDistance = NextDrivingAction.TriggerDistance - brakingDistance;
							NextDrivingAction.CoastingStartDistance = double.MaxValue.SI<Meter>();
							break;
						default:
							throw new ArgumentOutOfRangeException();
					}
				}

				if (nextAction != null) {
					if (nextAction.HasEqualTrigger(NextDrivingAction)) {
						// if the action changes and the vehicle has not yet exceeded the action distance => update the action
						// otherwise do nothing, NextDrivingAction's action distance has already been updated
						if (nextAction.Action != NextDrivingAction.Action && nextAction.ActionDistance > currentDistance) {
							NextDrivingAction = nextAction;
						}
					} else {
						// hmm, we've got a new action that is closer to what we got before?
						if (nextAction.ActionDistance < NextDrivingAction.ActionDistance) {
							NextDrivingAction = nextAction;
						}
					}
				} else {
					NextDrivingAction = null;
				}
			}
			Log.Debug("Next Driving Action: {0}", NextDrivingAction);
		}

		protected internal DrivingBehaviorEntry GetNextDrivingAction(Meter minDistance, Meter ds)
		{
			var currentSpeed = Driver.DataBus.VehicleSpeed;

			var lookaheadDistance =
				(currentSpeed.Value() * 3.6 * Driver.DriverData.LookAheadCoasting.LookAheadDistanceFactor).SI<Meter>();
			var stopDistance = Driver.ComputeDecelerationDistance(0.SI<MeterPerSecond>());
			lookaheadDistance = VectoMath.Max(2 * ds, lookaheadDistance, 1.2 * stopDistance);
			var lookaheadData = Driver.DataBus.LookAhead(lookaheadDistance);

			Log.Debug("Lookahead distance: {0} @ current speed {1}", lookaheadDistance, currentSpeed);
			var nextActions = new List<DrivingBehaviorEntry>();
			foreach (var entry in lookaheadData) {
				var nextTargetSpeed = OverspeedAllowed(entry.VehicleTargetSpeed)
					? entry.VehicleTargetSpeed + Driver.DriverData.OverSpeedEcoRoll.OverSpeed
					: entry.VehicleTargetSpeed;
				if (nextTargetSpeed < currentSpeed) {
					var action = DrivingBehavior.Braking;
					var coastingDistance = ComputeCoastingDistance(currentSpeed, entry);
					var brakingDistance = Driver.ComputeDecelerationDistance(nextTargetSpeed) + BrakingSafetyMargin;

					if (!Driver.DriverData.LookAheadCoasting.Enabled || coastingDistance < 0) {
						Log.Debug(
							"adding 'Braking' starting at distance {0}. brakingDistance: {1}, triggerDistance: {2}, nextTargetSpeed: {3}",
							entry.Distance - brakingDistance, brakingDistance, entry.Distance, nextTargetSpeed);
						coastingDistance = brakingDistance;
					} else {
						//var coastingDistance = ComputeCoastingDistance(currentSpeed, nextTargetSpeed);
						if (currentSpeed > Driver.DriverData.LookAheadCoasting.MinSpeed) {
							action = DrivingBehavior.Coasting;

							Log.Debug(
								"adding 'Coasting' starting at distance {0}. coastingDistance: {1}, triggerDistance: {2}, nextTargetSpeed: {3}",
								entry.Distance - coastingDistance, coastingDistance, entry.Distance, nextTargetSpeed);
						} else {
							coastingDistance = -1.SI<Meter>();
						}
					}
					nextActions.Add(
						new DrivingBehaviorEntry {
							Action = action,
							CoastingStartDistance = entry.Distance - coastingDistance,
							BrakingStartDistance = entry.Distance - brakingDistance,
							TriggerDistance = entry.Distance,
							NextTargetSpeed = nextTargetSpeed,
							CycleEntry = entry,
						});
				}
			}
			if (!nextActions.Any()) {
				return null;
			}
			var nextBrakingAction = nextActions.OrderBy(x => x.BrakingStartDistance).First();
			var nextCoastingAction = nextActions.OrderBy(x => x.CoastingStartDistance).First();
			if (nextBrakingAction.TriggerDistance.IsEqual(nextCoastingAction.TriggerDistance)) {
				// its the same trigger, use it
				return nextCoastingAction;
			}

			// MQ: 27.5.2016 remark: one could set the coasting distance to the closest coasting distance as found above to start coasting a little bit earlier.
			return nextBrakingAction;
		}

		protected internal virtual Meter ComputeCoastingDistance(MeterPerSecond vehicleSpeed,
			DrivingCycleData.DrivingCycleEntry actionEntry)
		{
			var targetSpeed = OverspeedAllowed(actionEntry.VehicleTargetSpeed)
				? actionEntry.VehicleTargetSpeed + Driver.DriverData.OverSpeedEcoRoll.OverSpeed
				: actionEntry.VehicleTargetSpeed;

			var vehicleMass = Driver.DataBus.TotalMass + Driver.DataBus.ReducedMassWheels;
			var targetAltitude = actionEntry.Altitude; //dec.Altitude;

			var vehicleAltitude = Driver.DataBus.Altitude;

			var targetEnergy = vehicleMass * Physics.GravityAccelleration * targetAltitude +
								vehicleMass * targetSpeed * targetSpeed / 2;
			var vehicleEnergy = vehicleMass * Physics.GravityAccelleration * vehicleAltitude +
								vehicleMass * vehicleSpeed * vehicleSpeed / 2;

			var energyDifference = vehicleEnergy - targetEnergy;

			var airDragForce = Driver.DataBus.AirDragResistance(vehicleSpeed, targetSpeed);
			var rollResistanceForce = Driver.DataBus.RollingResistance(
				((targetAltitude - vehicleAltitude) / (actionEntry.Distance - Driver.DataBus.Distance)).Value().SI<Radian>());
			var engineDragLoss = Driver.DataBus.EngineDragPower(Driver.DataBus.EngineSpeed);
			var gearboxLoss = Driver.DataBus.GearboxLoss();
			var axleLoss = Driver.DataBus.AxlegearLoss();

			var coastingResistanceForce = airDragForce + rollResistanceForce +
										(gearboxLoss + axleLoss - engineDragLoss) / vehicleSpeed;

			var coastingDecisionFactor = Driver.DriverData.LookAheadCoasting.LookAheadDecisionFactor.Lookup(targetSpeed,
				vehicleSpeed - targetSpeed);
			var coastingDistance = (energyDifference / (coastingDecisionFactor * coastingResistanceForce)).Cast<Meter>();
			return coastingDistance;
		}

		public bool OverspeedAllowed(MeterPerSecond velocity, bool prohibitOverspeed = false)
		{
			if (prohibitOverspeed) {
				return false;
			}
			return Driver.DriverData.OverSpeedEcoRoll.Mode == DriverMode.Overspeed &&
					velocity > Driver.DriverData.OverSpeedEcoRoll.MinSpeed;
		}
	}

	//=====================================

	public interface IDriverMode
	{
		DefaultDriverStrategy DriverStrategy { get; set; }

		IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient);

		void ResetMode();
	}

	public abstract class AbstractDriverMode : LoggingObject, IDriverMode
	{
		private IDriverActions _driver;
		private DriverData _driverData;
		private IDataBus _dataBus;

		public DefaultDriverStrategy DriverStrategy { get; set; }

		protected IDriverActions Driver
		{
			get { return _driver ?? (_driver = DriverStrategy.Driver); }
		}

		protected DriverData DriverData
		{
			get { return _driverData ?? (_driverData = Driver.DriverData); }
		}

		protected IDataBus DataBus
		{
			get { return _dataBus ?? (_dataBus = Driver.DataBus); }
		}

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			var response = DoHandleRequest(absTime, ds, targetVelocity, gradient);

			if (DriverStrategy.NextDrivingAction == null || !(response is ResponseSuccess)) {
				return response;
			}

			// if we accelerate in the current simulation interval the ActionDistance of the next action
			// changes and we might pass the ActionDistance - check again...
			if (response.Acceleration <= 0) {
				return response;
			}

			// if the speed at the end of the simulation interval is below the next target speed 
			// we are fine (no need to brake right now)
			var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
			if (v2 <= DriverStrategy.NextDrivingAction.NextTargetSpeed) {
				return response;
			}

			Meter newds;
			response = CheckRequestDoesNotExceedNextAction(absTime, ds, targetVelocity, gradient, response, out newds);

			if (ds.IsEqual(newds, 1e-3.SI<Meter>())) {
				return response;
			}
			if (newds.IsSmallerOrEqual(0, 1e-3)) {
				newds = ds / 2.0;
				//DriverStrategy.CurrentDrivingMode = DefaultDriverStrategy.DrivingMode.DrivingModeBrake;
				//DriverStrategy.BrakeTrigger = DriverStrategy.NextDrivingAction;
			}

			var newOperatingPoint = VectoMath.ComputeTimeInterval(DataBus.VehicleSpeed, response.Acceleration, DataBus.Distance,
				newds);
			if (newOperatingPoint.SimulationInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval)) {
				// the next time interval will be too short, this may lead to issues with inertia etc. 
				// instead of accelerating, drive at constant speed.
				response = DoHandleRequest(absTime, ds, Driver.DataBus.VehicleSpeed, gradient, true);
				return response;
			}
			Log.Debug("Exceeding next ActionDistance at {0}. Reducing max Distance from {2} to {1}",
				DriverStrategy.NextDrivingAction.ActionDistance, newds, ds);
			return new ResponseDrivingCycleDistanceExceeded() {
				Source = this,
				MaxDistance = newds,
			};
		}

		protected abstract IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed = false);

		protected abstract IResponse CheckRequestDoesNotExceedNextAction(Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newSimulationDistance);

		public abstract void ResetMode();
	}

	//=====================================

	public class DriverModeDrive : AbstractDriverMode
	{
		protected override IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed = false)
		{
			IResponse response;

			Driver.DriverBehavior = DrivingBehavior.Driving;
			var velocity = targetVelocity;
			if (DriverStrategy.OverspeedAllowed(targetVelocity, prohibitOverspeed)) {
				velocity += DriverData.OverSpeedEcoRoll.OverSpeed;
			}
			if (DataBus.ClutchClosed(absTime)) {
				// drive along
				if (DriverStrategy.OverspeedAllowed(targetVelocity, prohibitOverspeed) &&
					DataBus.VehicleSpeed.IsEqual(targetVelocity)) {
					response = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
					if (response is ResponseSuccess && response.Acceleration < 0) {
						response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
					}
				} else {
					response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
				}
				response.Switch().
					Case<ResponseUnderload>(r => {
						if (DriverStrategy.OverspeedAllowed(targetVelocity, prohibitOverspeed)) {
							response = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
							if (response is ResponseUnderload || response is ResponseSpeedLimitExceeded) {
								response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							}
						} else {
							response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
						}
					});

				response.Switch().
					Case<ResponseGearShift>(r => {
						response = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
						response.Switch().
							Case<ResponseUnderload>(() => {
								// overload may happen if driver limits acceleration when rolling downhill
								response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							}).
							Case<ResponseSpeedLimitExceeded>(() => { response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient); });
					}).
					Case<ResponseOverload>(r => { response = Driver.DrivingActionCoast(absTime, ds, velocity, gradient); });
			} else {
				if (DataBus.VehicleSpeed.IsSmallerOrEqual(0.SI<MeterPerSecond>())) {
					// the clutch is disengaged, and the vehicle stopped - we can't perform a roll action. wait for the clutch to be engaged
					// todo mk 2016-08-23: is this still needed?
					var remainingShiftTime = Constants.SimulationSettings.TargetTimeInterval;
					while (!DataBus.ClutchClosed(absTime + remainingShiftTime)) {
						remainingShiftTime += Constants.SimulationSettings.TargetTimeInterval;
					}
					return new ResponseFailTimeInterval() {
						Source = this,
						DeltaT = remainingShiftTime,
					};
				}
				response = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
				response.Switch().
					Case<ResponseUnderload>(r => { response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient, r); })
					.Case<ResponseSpeedLimitExceeded>(() => { response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient); });
			}
			return response;
		}

		protected override IResponse CheckRequestDoesNotExceedNextAction(Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newds)
		{
			var nextAction = DriverStrategy.NextDrivingAction;
			newds = ds;
			if (nextAction == null) {
				return response;
			}
			var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
			var newBrakingDistance = Driver.DriverData.AccelerationCurve.ComputeAccelerationDistance(v2,
				nextAction.NextTargetSpeed) + DefaultDriverStrategy.BrakingSafetyMargin;
			switch (DriverStrategy.NextDrivingAction.Action) {
				case DrivingBehavior.Coasting:
					var coastingDistance = DriverStrategy.ComputeCoastingDistance(v2, nextAction.CycleEntry);
					var newActionDistance = coastingDistance;
					var safetyFactor = 4.0;
					if (newBrakingDistance > coastingDistance) {
						newActionDistance = newBrakingDistance;
						safetyFactor = 0.5;
					}
					// if the distance at the end of the simulation interval is smaller than the new ActionDistance
					// we are safe - go ahead...
					if ((Driver.DataBus.Distance + ds).IsSmallerOrEqual(nextAction.TriggerDistance - newActionDistance,
						Constants.SimulationSettings.DriverActionDistanceTolerance * safetyFactor) &&
						(Driver.DataBus.Distance + ds).IsSmallerOrEqual(nextAction.TriggerDistance - newBrakingDistance)) {
						return response;
					}
					newds = ds / 2; //EstimateAccelerationDistanceBeforeBrake(response, nextAction) ?? ds;
					break;
				case DrivingBehavior.Braking:
					if ((Driver.DataBus.Distance + ds).IsSmaller(nextAction.TriggerDistance - newBrakingDistance)) {
						return response;
					}
					newds = nextAction.TriggerDistance - newBrakingDistance - Driver.DataBus.Distance -
							Constants.SimulationSettings.DriverActionDistanceTolerance / 2;
					break;
				default:
					return response;
			}
			return response;
		}

		public override void ResetMode() {}
	}

	//=====================================

	public class DriverModeBrake : AbstractDriverMode
	{
		protected enum BrakingPhase
		{
			Coast,
			Brake
		}

		protected BrakingPhase Phase;
		protected bool RetryDistanceExceeded;

		protected override IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient,
			bool prohibitOverspeed = false)
		{
			IResponse response = null;
			if (DataBus.VehicleSpeed <= DriverStrategy.BrakeTrigger.NextTargetSpeed) {
				if (DataBus.ClutchClosed(absTime)) {
					if (DataBus.VehicleSpeed.IsGreater(0)) {
						response = Driver.DrivingActionAccelerate(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
					} else {
						if (RetryDistanceExceeded) {
							response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
						} else {
							RetryDistanceExceeded = true;
							return new ResponseDrivingCycleDistanceExceeded() { MaxDistance = ds / 2 };
						}
					}
				} else {
					response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
				}
				response.Switch().
					Case<ResponseGearShift>(() => {
						response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
						response.Switch().
							Case<ResponseUnderload>(r => {
								// under-load may happen if driver limits acceleration when rolling downhill
								response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
									gradient, r);
							}).
							Case<ResponseSpeedLimitExceeded>(() => {
								response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleSpeed,
									gradient);
							});
					}).
					Case<ResponseSpeedLimitExceeded>(() => {
						response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleSpeed,
							gradient);
					}).
					Case<ResponseUnderload>(r => {
						response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
							gradient, r);
					});
				return response;
			}
			var currentDistance = DataBus.Distance;

			var brakingDistance = Driver.ComputeDecelerationDistance(DriverStrategy.BrakeTrigger.NextTargetSpeed) +
								DefaultDriverStrategy.BrakingSafetyMargin;
			DriverStrategy.BrakeTrigger.BrakingStartDistance = DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance;
			if (Phase == BrakingPhase.Coast) {
				var nextBrakeAction = DriverStrategy.GetNextDrivingAction(DataBus.Distance, ds);
				if (nextBrakeAction != null && !DriverStrategy.BrakeTrigger.TriggerDistance.IsEqual(nextBrakeAction.TriggerDistance) &&
					nextBrakeAction.BrakingStartDistance.IsSmaller(DriverStrategy.BrakeTrigger.BrakingStartDistance)) {
					DriverStrategy.BrakeTrigger = nextBrakeAction;
					Log.Debug("setting brake trigger to new trigger: trigger distance: {0}, start braking @ {1}",
						nextBrakeAction.TriggerDistance, nextBrakeAction.BrakingStartDistance);
				}

				Log.Debug("start braking @ {0}", DriverStrategy.BrakeTrigger.BrakingStartDistance);
				var remainingDistanceToBrake = DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance;
				var estimatedTimeInterval = remainingDistanceToBrake / DataBus.VehicleSpeed;
				if (estimatedTimeInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval) ||
					currentDistance + Constants.SimulationSettings.DriverActionDistanceTolerance >
					DriverStrategy.BrakeTrigger.BrakingStartDistance) {
					Phase = BrakingPhase.Brake;
					Log.Debug("Switching to BRAKE Phase. currentDistance: {0}", currentDistance);
				} else {
					if ((currentDistance + ds).IsGreater(DriverStrategy.BrakeTrigger.BrakingStartDistance)) {
						return new ResponseDrivingCycleDistanceExceeded() {
							//Source = this,
							MaxDistance = DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance
						};
					}
				}
				if (DataBus.VehicleSpeed < Constants.SimulationSettings.MinVelocityForCoast) {
					Phase = BrakingPhase.Brake;
					Log.Debug("Switching to BRAKE Phase. currentDistance: {0}  v: {1}", currentDistance,
						DataBus.VehicleSpeed);
				}
			}
			switch (Phase) {
				case BrakingPhase.Coast:
					Driver.DriverBehavior = DrivingBehavior.Coasting;
					response = DataBus.ClutchClosed(absTime)
						? Driver.DrivingActionCoast(absTime, ds, VectoMath.Max(targetVelocity, DataBus.VehicleSpeed), gradient)
						: Driver.DrivingActionRoll(absTime, ds, VectoMath.Max(targetVelocity, DataBus.VehicleSpeed), gradient);
					response.Switch().
						Case<ResponseUnderload>(r => {
							// coast would decelerate more than driver's max deceleration => issue brakes to decelerate with driver's max deceleration
							response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
								gradient, r);
							if ((DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance).IsSmallerOrEqual(
								Constants.SimulationSettings.DriverActionDistanceTolerance)) {
								Phase = BrakingPhase.Brake;
							}
						}).
						Case<ResponseOverload>(r => {
							// limiting deceleration while coast may result in an overload => issue brakes to decelerate with driver's max deceleration
							response = DataBus.ClutchClosed(absTime)
								? Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient)
								: Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
							//Phase = BrakingPhase.Brake;
						}).
						Case<ResponseDrivingCycleDistanceExceeded>(r => {
							if (!ds.IsEqual(r.MaxDistance)) {
								// distance has been reduced due to vehicle stop in coast/roll action => use brake action to get exactly to the stop-distance
								// TODO what if no gear is enaged (and we need driveline power to get to the stop-distance?
								response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
							}
						}).
						Case<ResponseGearShift>(r => { response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient); });
					// handle the SpeedLimitExceeded Response separately in case it occurs in one of the requests in the second try
					response.Switch().
						Case<ResponseSpeedLimitExceeded>(() => {
							response = Driver.DrivingActionBrake(absTime, ds, DataBus.VehicleSpeed,
								gradient);
							if (response is ResponseOverload && !DataBus.ClutchClosed(absTime)) {
								response = Driver.DrivingActionRoll(absTime, ds, DataBus.VehicleSpeed, gradient);
							}
						});
					break;
				case BrakingPhase.Brake:

					Log.Debug("Phase: BRAKE. breaking distance: {0} start braking @ {1}", brakingDistance,
						DriverStrategy.BrakeTrigger.BrakingStartDistance);
					if (DriverStrategy.BrakeTrigger.BrakingStartDistance < currentDistance) {
						Log.Info("Expected Braking Deceleration could not be reached! {0}",
							DriverStrategy.BrakeTrigger.BrakingStartDistance - currentDistance);
					}
					var targetDistance = DataBus.VehicleSpeed < Constants.SimulationSettings.MinVelocityForCoast
						? DriverStrategy.BrakeTrigger.TriggerDistance
						: null;
					if (targetDistance == null && DriverStrategy.BrakeTrigger.NextTargetSpeed.IsEqual(0.SI<MeterPerSecond>())) {
						targetDistance = DriverStrategy.BrakeTrigger.TriggerDistance - DefaultDriverStrategy.BrakingSafetyMargin;
					}
					Driver.DriverBehavior = DrivingBehavior.Braking;
					response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed,
						gradient, targetDistance: targetDistance);
					response.Switch().
						Case<ResponseOverload>(r => {
							Log.Info(
								"Brake -> Got OverloadResponse during brake action - desired deceleration could not be reached! response: {0}",
								r);
							if (!DataBus.ClutchClosed(absTime)) {
								Log.Info("Brake -> Overload -> Clutch is open - Trying roll action");
								response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
							} else {
								Log.Info("Brake -> Overload -> Clutch is closed - Trying brake action again");
								response = Driver.DrivingActionBrake(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient,
									targetDistance: targetDistance);
								response.Switch().
									Case<ResponseOverload>(r1 => {
										Log.Info("Brake -> Overload -> 2nd Brake -> Overload -> Trying accelerate action");
										response = Driver.DrivingActionAccelerate(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
										response.Switch().Case<ResponseGearShift>(
											rs => {
												Log.Info("Brake -> Overload -> 2nd Brake -> Accelerate -> Got GearShift response, performing roll action");
												response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
											});
									});
							}
						}).
						Case<ResponseGearShift>(r => {
							Log.Info("Brake -> Got GearShift response, performing roll action");
							response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
						});
					break;
			}

			return response;
		}

		protected override IResponse CheckRequestDoesNotExceedNextAction(Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newds)
		{
			var nextAction = DriverStrategy.BrakeTrigger;
			newds = ds;
			if (nextAction == null) {
				return response;
			}

			switch (nextAction.Action) {
				case DrivingBehavior.Coasting:
					var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
					var newBrakingDistance = Driver.DriverData.AccelerationCurve.ComputeAccelerationDistance(v2,
						nextAction.NextTargetSpeed);
					if ((Driver.DataBus.Distance + ds).IsSmaller(nextAction.TriggerDistance - newBrakingDistance)) {
						return response;
					}
					newds = nextAction.TriggerDistance - newBrakingDistance - Driver.DataBus.Distance -
							Constants.SimulationSettings.DriverActionDistanceTolerance / 2;
					break;
				default:
					return response;
			}
			return response;
		}

		public override void ResetMode()
		{
			RetryDistanceExceeded = false;
			Phase = BrakingPhase.Coast;
		}
	}

	//=====================================

	[DebuggerDisplay("ActionDistance: {ActionDistance}, TriggerDistance: {TriggerDistance}, Action: {Action}")]
	public class DrivingBehaviorEntry
	{
		public DrivingBehavior Action;
		public MeterPerSecond NextTargetSpeed;
		public Meter TriggerDistance;

		public Meter ActionDistance
		{
			get
			{
				return VectoMath.Min(CoastingStartDistance ?? double.MaxValue.SI<Meter>(),
					BrakingStartDistance ?? double.MaxValue.SI<Meter>());
			}
		}

		public Meter SelectActionDistance(Meter minDistance)
		{
			return
				new[] { BrakingStartDistance, CoastingStartDistance }.OrderBy(x => x.Value()).First(x => x >= minDistance);
		}

		public Meter CoastingStartDistance { get; set; }

		public Meter BrakingStartDistance { get; set; }

		public DrivingCycleData.DrivingCycleEntry CycleEntry;

		public bool HasEqualTrigger(DrivingBehaviorEntry other)
		{
			return TriggerDistance.IsEqual(other.TriggerDistance) && NextTargetSpeed.IsEqual(other.NextTargetSpeed);
		}

		public override string ToString()
		{
			return string.Format("action: {0} @ {1} / {2}. trigger: {3} targetSpeed: {4}", Action, CoastingStartDistance,
				BrakingStartDistance, TriggerDistance, NextTargetSpeed);
		}
	}
}