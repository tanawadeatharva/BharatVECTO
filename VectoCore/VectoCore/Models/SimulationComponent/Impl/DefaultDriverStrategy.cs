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
using System.Windows.Markup;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.InputData.Impl;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using DriverData = TUGraz.VectoCore.Models.SimulationComponent.Data.DriverData;

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

		/// <summary>
		/// Checks if Look Ahead Coasting triggers
		/// </summary>
		public Dictionary<string, object> LookAheadCoasting(Meter ds)
		{
			var dict = new Dictionary<string, object>();

			// (1) & (2) : x_decelerationpoint - d_prev <= x_veh < x_decelerationpoint
			var v_veh = Driver.DataBus.CycleData.LeftSample.VehicleTargetSpeed; //Driver.DataBus.VehicleSpeed;
			var d_prev = 10 * Driver.DataBus.VehicleSpeed.ConvertTo().Kilo.Meter.Per.Hour.Value();
			var lookaheadData = Driver.DataBus.LookAhead(d_prev.SI<Meter>());
			var nextActions = new SortedDictionary<Meter, DrivingCycleData.DrivingCycleEntry>();
			foreach (var entry in lookaheadData.Where(e => e.VehicleTargetSpeed <= v_veh)) {
				var nextTargetSpeed = entry.VehicleTargetSpeed;
				if (nextTargetSpeed < Driver.DataBus.VehicleSpeed) {
					var coastingDistance = Formulas.DecelerationDistance(Driver.DataBus.VehicleSpeed, nextTargetSpeed,
						Driver.DriverData.LookAheadCoasting.Deceleration);
					nextActions.Add(entry.Distance - coastingDistance, entry);
				}
			}

			var dec = nextActions.FirstOrDefault().Value;

			// (4) v_veh < v_max_deceleration * 0.98
			if (dec != null) {
				var x_dec = dec.Distance;
				dict["x_dec"] = x_dec.Value();

				var x_delta = x_dec - Driver.DataBus.Distance;
				dict["x_delta"] = x_delta.Value();

				var v_target = dec.VehicleTargetSpeed;
				dict["v_target"] = v_target.Value();

				var x_max_deceleration = Driver.ComputeDecelerationDistance(v_target);
				dict["x_max_deceleration"] = x_max_deceleration.Value();
				var coastingPossible = x_delta >= x_max_deceleration * 0.98;
				dict["CoastingAllowed"] = coastingPossible ? 1 : 0;

				// 3. CDP > DF_coasting
				if (coastingPossible) {
					var m = Driver.DataBus.TotalMass;
					var g = Physics.GravityAccelleration;
					var h_target = dec.Altitude;
					dict["h_target"] = h_target.Value();

					var h_vehicle = Driver.DataBus.Altitude;
					dict["h_vehicle"] = h_vehicle.Value();

					var E_kin_veh = m * v_veh * v_veh / 2;
					var E_kin_target = m * v_target * v_target / 2;
					var E_pot_target = m * g * h_target;
					var E_pot_veh = m * g * h_vehicle;
					dict["E_kin_veh"] = E_kin_veh.Value();
					dict["E_kin_target"] = E_kin_target.Value();
					dict["E_pot_target"] = E_pot_target.Value();
					dict["E_pot_veh"] = E_pot_veh.Value();

					var delta_E = (E_kin_veh + E_pot_veh) - (E_kin_target + E_pot_target);
					dict["delta_E"] = delta_E.Value();

					var F_dec_average = delta_E / x_delta;
					dict["F_dec_average"] = F_dec_average.Value();

					//var avgVelocity = (v_veh + v_target) / 2;
					//var acc = (v_target - v_veh) * (avgVelocity / x_delta);
					var F_air = Driver.DataBus.AirDragResistance(v_veh, v_target);
					dict["F_air"] = F_air.Value();

					var F_roll = Driver.DataBus.RollingResistance(((h_target - h_vehicle) / x_delta).Value().SI<Radian>());
					dict["F_roll"] = F_roll.Value();

					var F_enginedrag = Driver.DataBus.EngineDragPower(Driver.DataBus.EngineSpeed) / v_veh;
					dict["F_enginedrag"] = F_enginedrag.Value();

					var F_loss_gb = Driver.DataBus.GearboxLoss(Driver.DataBus.EngineSpeed, Driver.DataBus.EngineTorque) / v_veh;
					dict["F_loss_gb"] = F_loss_gb.Value();

					// todo mk-2016-05-11 calculate ra loss
					var F_loss_ra = 0.SI<Newton>();
					dict["F_loss_ra"] = F_loss_ra.Value();

					var F_coasting = F_enginedrag - F_air - F_roll - F_loss_gb - F_loss_ra;
					dict["F_coasting"] = F_coasting.Value();

					var CDP = F_dec_average / -F_coasting;
					dict["CDP"] = CDP.Value();

					var DF_coasting = LACDecisionFactor.Lookup(v_target, v_veh - v_target);
					dict["DF_coasting"] = DF_coasting;

					dict["EnableCoasting"] = CDP > DF_coasting ? 1 : 0;
				}
			}
			return dict;
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
					switch (NextDrivingAction.Action) {
						case DrivingBehavior.Coasting:

							//var coastingDistance = ComputeCoastingDistance(Driver.DataBus.VehicleSpeed, NextDrivingAction.NextTargetSpeed);
							var coastingDistance = ComputeCoastingDistance(Driver.DataBus.VehicleSpeed, NextDrivingAction.CycleEntry);
							NextDrivingAction.ActionDistance = NextDrivingAction.TriggerDistance - coastingDistance;
							break;
						case DrivingBehavior.Braking:
							var brakingDistance = Driver.ComputeDecelerationDistance(NextDrivingAction.NextTargetSpeed) + BrakingSafetyMargin;
							NextDrivingAction.ActionDistance = NextDrivingAction.TriggerDistance - brakingDistance;
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

		protected DrivingBehaviorEntry GetNextDrivingAction(Meter minDistance, Meter ds)
		{
			var currentSpeed = Driver.DataBus.VehicleSpeed;

			// distance until halt
#if NEW_COASTING
   			var lookaheadDistance = (currentSpeed.Value() * 3.6 * 10).SI<Meter>();
#else
            var lookaheadDistance = Formulas.DecelerationDistance(currentSpeed, 0.SI<MeterPerSecond>(),
                Driver.DriverData.LookAheadCoasting.Deceleration);
#endif
			lookaheadDistance = VectoMath.Max(2 * ds, 1.2 * lookaheadDistance);
			var lookaheadData = Driver.DataBus.LookAhead(lookaheadDistance);

			Log.Debug("Lookahead distance: {0} @ current speed {1}", lookaheadDistance, currentSpeed);
			var nextActions = new List<DrivingBehaviorEntry>();
			foreach (var entry in lookaheadData) {
				var nextTargetSpeed = OverspeedAllowed(entry.RoadGradient, entry.VehicleTargetSpeed)
					? entry.VehicleTargetSpeed + Driver.DriverData.OverSpeedEcoRoll.OverSpeed
					: entry.VehicleTargetSpeed;
				if (nextTargetSpeed < currentSpeed) {
					var coastingDistance = ComputeCoastingDistance(currentSpeed, entry);
#if NEW_COASTING
                    if (coastingDistance < 0) {
#else
                    if ( !Driver.DriverData.LookAheadCoasting.Enabled ||
						currentSpeed < Driver.DriverData.LookAheadCoasting.MinSpeed || 
                        coastingDistance < 0)
                    {
#endif
						var brakingDistance = Driver.ComputeDecelerationDistance(nextTargetSpeed) + BrakingSafetyMargin;
						Log.Debug(
							"adding 'Braking' starting at distance {0}. brakingDistance: {1}, triggerDistance: {2}, nextTargetSpeed: {3}",
							entry.Distance - brakingDistance, brakingDistance, entry.Distance, nextTargetSpeed);
						nextActions.Add(new DrivingBehaviorEntry {
							Action = DrivingBehavior.Braking,
							ActionDistance = entry.Distance - brakingDistance,
							TriggerDistance = entry.Distance,
							NextTargetSpeed = nextTargetSpeed,
							CycleEntry = entry,
						});
					} else {
						//var coastingDistance = ComputeCoastingDistance(currentSpeed, nextTargetSpeed);

						Log.Debug(
							"adding 'Coasting' starting at distance {0}. coastingDistance: {1}, triggerDistance: {2}, nextTargetSpeed: {3}",
							entry.Distance - coastingDistance, coastingDistance, entry.Distance, nextTargetSpeed);
						nextActions.Add(
							new DrivingBehaviorEntry {
								Action = DrivingBehavior.Coasting,
								ActionDistance = entry.Distance - coastingDistance,
								TriggerDistance = entry.Distance,
								NextTargetSpeed = nextTargetSpeed,
								CycleEntry = entry,
							});
					}
				}
			}

			return nextActions.Count == 0 ? null : nextActions.OrderBy(x => x.ActionDistance).First();
		}



#if NEW_COASTING
		protected virtual Meter ComputeCoastingDistance(MeterPerSecond v_veh, DrivingCycleData.DrivingCycleEntry actionEntry)
		{
			var v_target = OverspeedAllowed(actionEntry.RoadGradient, actionEntry.VehicleTargetSpeed)
				? actionEntry.VehicleTargetSpeed + Driver.DriverData.OverSpeedEcoRoll.OverSpeed
				: actionEntry.VehicleTargetSpeed;

			var m = Driver.DataBus.TotalMass;
			var g = Physics.GravityAccelleration;
			var h_target = actionEntry.Altitude; //dec.Altitude;

			var h_vehicle = Driver.DataBus.Altitude;

			var E_kin_veh = m * v_veh * v_veh / 2;
			var E_kin_target = m * v_target * v_target / 2;
			var E_pot_target = m * g * h_target;
			var E_pot_veh = m * g * h_vehicle;

			var delta_E = (E_kin_veh + E_pot_veh) - (E_kin_target + E_pot_target);

			//var F_dec_average = delta_E / x_delta;

			var F_air = Driver.DataBus.AirDragResistance(v_veh, v_target);
			var F_roll =
				Driver.DataBus.RollingResistance(
					((h_target - h_vehicle) / (actionEntry.Distance - Driver.DataBus.Distance)).Value().SI<Radian>());
			var F_enginedrag = Driver.DataBus.EngineDragPower(Driver.DataBus.EngineSpeed) / v_veh;
			var F_loss_gb = Driver.DataBus.GearboxLoss(Driver.DataBus.EngineSpeed, Driver.DataBus.EngineTorque) / v_veh;

			// todo mk-2016-05-11 calculate ra loss
			var F_loss_ra = 0.SI<Newton>();

			var F_coasting = F_air + F_roll + F_loss_gb + F_loss_ra - F_enginedrag;

			//var CDP = F_dec_average / -F_coasting;

			var DF_coasting = LACDecisionFactor.Lookup(v_target, v_veh - v_target);

			var delta_x = (delta_E / (DF_coasting * F_coasting)).Cast<Meter>();

			return delta_x;
		}
#else
        protected virtual Meter ComputeCoastingDistance(MeterPerSecond currentSpeed, DrivingCycleData.DrivingCycleEntry actionEntry)
        {
            return Formulas.DecelerationDistance(currentSpeed, actionEntry.VehicleTargetSpeed,
                Driver.DriverData.LookAheadCoasting.Deceleration);
        }
#endif

		public bool OverspeedAllowed(Radian gradient, MeterPerSecond velocity)
		{
			return Driver.DriverData.OverSpeedEcoRoll.Mode == DriverMode.Overspeed &&
					gradient < 0 && velocity > Driver.DriverData.OverSpeedEcoRoll.MinSpeed;
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

			Log.Debug("Exceeding next ActionDistance at {0}. Reducing max Distance from {2} to {1}",
				DriverStrategy.NextDrivingAction.ActionDistance, newds, ds);
			return new ResponseDrivingCycleDistanceExceeded() {
				Source = this,
				MaxDistance = newds,
			};
		}

		protected abstract IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient);

		protected abstract IResponse CheckRequestDoesNotExceedNextAction(Second absTime, Meter ds,
			MeterPerSecond targetVelocity, Radian gradient, IResponse response, out Meter newSimulationDistance);

		public abstract void ResetMode();

		protected Meter EstimateAccelerationDistanceBeforeBrake(IResponse retVal, DrivingBehaviorEntry nextAction)
		{
			// estimate the distance to drive when accelerating with the current acceleration (taken from retVal) and then 
			// coasting with the dirver's LookaheadDeceleration. 
			// TriggerDistance - CurrentDistance = s_accelerate + s_lookahead
			// s_coast(dt) = - (currentSpeed + a * dt + nextTargetSpeed) * (nextTargetSpeed - (currentSpeed + a * dt))  / (2 * a_lookahead)
			// s_acc(dt) = currentSpeed * dt + a / 2 * dt^2
			// => solve for dt, compute ds = currentSpeed * dt + a / 2 * dt^2
			var dtList = VectoMath.QuadraticEquationSolver(
				(retVal.Acceleration / 2 -
				retVal.Acceleration * retVal.Acceleration / 2 / Driver.DriverData.LookAheadCoasting.Deceleration).Value(),
				(Driver.DataBus.VehicleSpeed -
				Driver.DataBus.VehicleSpeed * retVal.Acceleration / Driver.DriverData.LookAheadCoasting.Deceleration).Value(),
				(nextAction.NextTargetSpeed * nextAction.NextTargetSpeed / 2 / Driver.DriverData.LookAheadCoasting.Deceleration -
				Driver.DataBus.VehicleSpeed * Driver.DataBus.VehicleSpeed / 2 / Driver.DriverData.LookAheadCoasting.Deceleration -
				(nextAction.TriggerDistance - Driver.DataBus.Distance)).Value());
			dtList.Sort();
			if (!dtList.Any(x => x > 0)) {
				return null;
			}
			var dt = dtList.First(x => x > 0).SI<Second>();
			var newds = Driver.DataBus.VehicleSpeed * dt + (retVal.Acceleration / 2 * dt * dt);
			return newds;
		}
	}

	//=====================================

	public class DriverModeDrive : AbstractDriverMode
	{
		protected override IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			IResponse response = null;

			Driver.DriverBehavior = DrivingBehavior.Driving;
			var velocity = targetVelocity;
			if (DriverStrategy.OverspeedAllowed(gradient, targetVelocity)) {
				velocity += DriverData.OverSpeedEcoRoll.OverSpeed;
			}
			if (DataBus.ClutchClosed(absTime)) {
				// drive along
				if (DriverStrategy.OverspeedAllowed(gradient, targetVelocity) && DataBus.VehicleSpeed.IsEqual(targetVelocity)) {
					response = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
					if (response is ResponseSuccess && response.Acceleration < 0) {
						response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
					}
				} else {
					response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
				}
				response.Switch().
					Case<ResponseGearShift>(() => {
						response = Driver.DrivingActionRoll(absTime, ds, velocity, gradient);
						response.Switch().
							Case<ResponseUnderload>(() => {
								// overload may happen if driver limits acceleration when rolling downhill
								response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							}).
							Case<ResponseSpeedLimitExceeded>(() => { response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient); });
					}).
					Case<ResponseUnderload>(r => {
						if (DriverStrategy.OverspeedAllowed(gradient, targetVelocity)) {
							response = Driver.DrivingActionCoast(absTime, ds, velocity, gradient);
							if (response is ResponseUnderload || response is ResponseSpeedLimitExceeded) {
								response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
							}
						} else {
							response = Driver.DrivingActionBrake(absTime, ds, velocity, gradient);
						}
					});
			} else {
				if (DataBus.VehicleSpeed.IsSmallerOrEqual(0.SI<MeterPerSecond>())) {
					// the clutch is disengaged, and the vehicle stopped - we can't perform a roll action. wait for the clutch to be engaged
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
			switch (DriverStrategy.NextDrivingAction.Action) {
				case DrivingBehavior.Coasting:
					var coastingDistance = Formulas.DecelerationDistance(v2, nextAction.NextTargetSpeed,
						Driver.DriverData.LookAheadCoasting.Deceleration);

					// if the distance at the end of the simulation interval is smaller than the new ActionDistance
					// we are safe - go ahead...
					if ((Driver.DataBus.Distance + ds).IsSmallerOrEqual(nextAction.TriggerDistance - coastingDistance,
						Constants.SimulationSettings.DriverActionDistanceTolerance)) {
						return response;
					}
					newds = EstimateAccelerationDistanceBeforeBrake(response, nextAction) ?? ds;
					break;
				case DrivingBehavior.Braking:
					var brakingDistance = Driver.DriverData.AccelerationCurve.ComputeAccelerationDistance(v2,
						nextAction.NextTargetSpeed) + DefaultDriverStrategy.BrakingSafetyMargin;
					if ((Driver.DataBus.Distance + ds).IsSmaller(nextAction.TriggerDistance - brakingDistance)) {
						return response;
					}
					newds = (nextAction.TriggerDistance - brakingDistance) - Driver.DataBus.Distance;
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
		protected bool RetryDistanceExceeded = false;

		protected override IResponse DoHandleRequest(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			IResponse response = null;
			if (DataBus.VehicleSpeed <= DriverStrategy.BrakeTrigger.NextTargetSpeed) {
				if (DataBus.ClutchClosed(absTime)) {
					if (DataBus.VehicleSpeed.IsGreater(0.SI<MeterPerSecond>())) {
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

			if (Phase == BrakingPhase.Coast) {
				var brakingDistance = Driver.ComputeDecelerationDistance(DriverStrategy.BrakeTrigger.NextTargetSpeed) +
									DefaultDriverStrategy.BrakingSafetyMargin;
				Log.Debug("breaking distance: {0}, start braking @ {1}", brakingDistance,
					DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance);
				var remainingDistanceToBrake = DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance - currentDistance;
				var estimatedTimeInterval = remainingDistanceToBrake / DataBus.VehicleSpeed;
				if (estimatedTimeInterval.IsSmaller(Constants.SimulationSettings.LowerBoundTimeInterval) ||
					currentDistance + Constants.SimulationSettings.DriverActionDistanceTolerance >
					DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance) {
					Phase = BrakingPhase.Brake;
					Log.Debug("Switching to BRAKE Phase. currentDistance: {0}", currentDistance);
				} else {
					if ((currentDistance + ds).IsGreater(DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance)) {
						return new ResponseDrivingCycleDistanceExceeded() {
							//Source = this,
							MaxDistance = DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance - currentDistance
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
							Phase = BrakingPhase.Brake;
						}).
						Case<ResponseOverload>(r => {
							// limiting deceleration while coast may result in an overload => issue brakes to decelerate with driver's max deceleration
							if (DataBus.ClutchClosed(absTime)) {
								response = Driver.DrivingActionAccelerate(absTime, ds, targetVelocity, gradient);
							} else {
								response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
							}
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
					var brakingDistance = Driver.ComputeDecelerationDistance(DriverStrategy.BrakeTrigger.NextTargetSpeed) +
										DefaultDriverStrategy.BrakingSafetyMargin;
					Log.Debug("Phase: BRAKE. breaking distance: {0} start braking @ {1}", brakingDistance,
						DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance);
					if (DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance < currentDistance) {
						Log.Info("Expected Braking Deceleration could not be reached! {0}",
							DriverStrategy.BrakeTrigger.TriggerDistance - brakingDistance - currentDistance);
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
							Log.Info("Got OverloadResponse during brake action - desired deceleration could not be reached! response: {0}", r);
							if (!DataBus.ClutchClosed(absTime)) {
								Log.Info("Clutch is open - trying RollAction");
								response = Driver.DrivingActionRoll(absTime, ds, targetVelocity, gradient);
							} else {
								Log.Info("Clutch is closed - trying AccelerateAction");
								response = Driver.DrivingActionAccelerate(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
								response.Switch().Case<ResponseGearShift>(
									rs => {
										Log.Info("Got GearShift response, performing roll action...");
										response = Driver.DrivingActionRoll(absTime, ds, DriverStrategy.BrakeTrigger.NextTargetSpeed, gradient);
									}
									);
							}
						});
					break;
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

			switch (nextAction.Action) {
				case DrivingBehavior.Coasting:
					var v2 = Driver.DataBus.VehicleSpeed + response.Acceleration * response.SimulationInterval;
					var brakingDistance = Driver.DriverData.AccelerationCurve.ComputeAccelerationDistance(v2,
						nextAction.NextTargetSpeed);
					if ((Driver.DataBus.Distance + ds).IsSmaller(nextAction.TriggerDistance - brakingDistance)) {
						return response;
					}
					newds = (nextAction.TriggerDistance - brakingDistance) - Driver.DataBus.Distance;
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
		public Meter ActionDistance;
		public DrivingCycleData.DrivingCycleEntry CycleEntry;

		public bool HasEqualTrigger(DrivingBehaviorEntry other)
		{
			return TriggerDistance.IsEqual(other.TriggerDistance) && NextTargetSpeed.IsEqual(other.NextTargetSpeed);
		}

		public override string ToString()
		{
			return string.Format("action: {0} @ {1}. trigger: {2} targetSpeed: {3}", Action, ActionDistance, TriggerDistance,
				NextTargetSpeed);
		}
	}
}