/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;
using DriverData = TUGraz.VectoCore.Models.SimulationComponent.Data.DriverData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Driver :
		StatefulProviderComponent<Driver.DriverState, IDrivingCycleOutPort, IDriverDemandInPort, IDriverDemandOutPort>,
		IDriver, IDrivingCycleOutPort, IDriverDemandInPort, IDriverActions, IDriverInfo
	{
		public DriverData DriverData { get; protected set; }

		protected readonly IDriverStrategy DriverStrategy;
		public string CurrentAction = "";

		public Driver(IVehicleContainer container, DriverData driverData, IDriverStrategy strategy) : base(container)
		{
			DriverData = driverData;
			DriverStrategy = strategy;
			strategy.Driver = this;
			DriverAcceleration = 0.SI<MeterPerSquareSecond>();
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			DriverBehavior = vehicleSpeed.IsEqual(0) ? DrivingBehavior.Halted : DrivingBehavior.Driving;
			return NextComponent.Initialize(vehicleSpeed, roadGradient);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient,
			MeterPerSquareSecond startAcceleration)
		{
			DriverBehavior = vehicleSpeed.IsEqual(0) ? DrivingBehavior.Halted : DrivingBehavior.Driving;
			var retVal = NextComponent.Initialize(vehicleSpeed, roadGradient, startAcceleration);

			return retVal;
		}

		public IResponse Request(Second absTime, Meter ds, MeterPerSecond targetVelocity, Radian gradient)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("==== DRIVER Request (distance) ====");
			Log.Debug(
				"Request: absTime: {0},  ds: {1}, targetVelocity: {2}, gradient: {3} | distance: {4}, velocity: {5}, vehicle stopped: {6}",
				absTime, ds, targetVelocity, gradient, DataBus.Distance, DataBus.VehicleSpeed, DataBus.VehicleStopped);

			var retVal = DriverStrategy.Request(absTime, ds, targetVelocity, gradient);

			CurrentState.Response = retVal;
			retVal.SimulationInterval = CurrentState.dt;
			retVal.Acceleration = CurrentState.Acceleration;

			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("==== DRIVER Request (time) ====");
			Log.Debug(
				"Request: absTime: {0},  dt: {1}, targetVelocity: {2}, gradient: {3} | distance: {4}, velocity: {5} gear: {6}: vehicle stopped: {7}",
				absTime, dt, targetVelocity, gradient, DataBus.Distance, DataBus.VehicleSpeed, DataBus.Gear,
				DataBus.VehicleStopped);

			var retVal = DriverStrategy.Request(absTime, dt, targetVelocity, gradient);

			CurrentState.Response = retVal;
			retVal.SimulationInterval = CurrentState.dt;
			retVal.Acceleration = CurrentState.Acceleration;

			return retVal;
		}

		public new IDataBus DataBus
		{
			get { return base.DataBus; }
		}

		/// <summary>
		/// see documentation of IDriverActions
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="ds"></param>
		/// <param name="targetVelocity"></param>
		/// <param name="gradient"></param>
		/// <param name="previousResponse"></param>
		/// <returns></returns>
		public IResponse DrivingActionAccelerate(Second absTime, Meter ds, MeterPerSecond targetVelocity,
			Radian gradient,
			IResponse previousResponse = null)
		{
			CurrentAction = "ACCELERATE";
			IterationStatistics.Increment(this, "Accelerate");
			Log.Debug("DrivingAction Accelerate");
			var operatingPoint = ComputeAcceleration(ds, targetVelocity);

			IResponse retVal = null;
			DriverAcceleration = operatingPoint.Acceleration;
			var response = previousResponse ??
							NextComponent.Request(absTime, operatingPoint.SimulationInterval, operatingPoint.Acceleration,
								gradient);
			response.Acceleration = operatingPoint.Acceleration;

			response.Switch().
				Case<ResponseSuccess>(r => { retVal = r; // => return
				}).
				Case<ResponseOverload>(). // do nothing, searchOperatingPoint is called later on
				Case<ResponseEngineSpeedTooHigh>(). // do nothing, searchOperatingPoint is called later on
				Case<ResponseUnderload>(r => {
					// Delta is negative we are already below the Drag-load curve. activate brakes
					retVal = r; // => return, strategy should brake
				}).
				Case<ResponseFailTimeInterval>(r => {
					// occurs only with AT gearboxes - extend time interval after gearshift!
					retVal = new ResponseDrivingCycleDistanceExceeded {
						Source = this,
						MaxDistance = r.Acceleration / 2 * r.DeltaT * r.DeltaT + DataBus.VehicleSpeed * r.DeltaT
					};
				}).
				Case<ResponseGearShift>(r => { retVal = r; }).
				Default(r => { throw new UnexpectedResponseException("DrivingAction Accelerate.", r); });

			if (retVal == null) {
				// unhandled response (overload, delta > 0) - we need to search for a valid operating point..	

				OperatingPoint nextOperatingPoint;
				try {
					nextOperatingPoint = SearchOperatingPoint(absTime, ds, gradient, operatingPoint.Acceleration,
						response);
				} catch (VectoEngineSpeedTooLowException) {
					// in case of an exception during search the engine-speed got too low - gear disengaged, try roll action.
					nextOperatingPoint = SearchOperatingPoint(absTime, ds, gradient, operatingPoint.Acceleration,
						response);
				}

				var limitedOperatingPoint = nextOperatingPoint;
				if (!(retVal is ResponseEngineSpeedTooHigh || DataBus.ClutchClosed(absTime))) {
					limitedOperatingPoint = LimitAccelerationByDriverModel(nextOperatingPoint,
						LimitationMode.LimitDecelerationDriver);
					Log.Debug("Found operating point for Drive/Accelerate. dt: {0}, acceleration: {1}",
						limitedOperatingPoint.SimulationInterval, limitedOperatingPoint.Acceleration);
				}
				DriverAcceleration = limitedOperatingPoint.Acceleration;
				retVal = NextComponent.Request(absTime, limitedOperatingPoint.SimulationInterval,
					limitedOperatingPoint.Acceleration,
					gradient);
				if (retVal != null) {
					retVal.Acceleration = limitedOperatingPoint.Acceleration;
				}
				retVal.Switch().
					Case<ResponseUnderload>(() => operatingPoint = limitedOperatingPoint)
					. // acceleration is limited by driver model, operating point moves below drag curve
					Case<ResponseOverload>(() => {
						// deceleration is limited by driver model, operating point moves above full load (e.g., steep uphill)
						// the vehicle/driver can't achieve an acceleration higher than deceleration curve, try again with higher deceleration
						if (DataBus.GearboxType.AutomaticTransmission()) {
							Log.Info("AT Gearbox - Operating point resulted in an overload, searching again...");
							// search again for operating point, transmission may have shifted inbetween
							nextOperatingPoint = SearchOperatingPoint(absTime, ds, gradient, operatingPoint.Acceleration,
								response);
							DriverAcceleration = nextOperatingPoint.Acceleration;
							retVal = NextComponent.Request(absTime, nextOperatingPoint.SimulationInterval,
								nextOperatingPoint.Acceleration, gradient);
						} else {
							if (absTime > 0 && DataBus.VehicleStopped) {
								Log.Info(
									"Operating point with limited acceleration resulted in an overload! Vehicle stopped! trying HALT action {0}",
									nextOperatingPoint.Acceleration);
								DataBus.BrakePower = 1.SI<Watt>();
								retVal = DrivingActionHalt(absTime, nextOperatingPoint.SimulationInterval, 0.SI<MeterPerSecond>(), gradient);
								ds = 0.SI<Meter>();
								//retVal.Acceleration = 0.SI<MeterPerSquareSecond>();
							} else {
								if (response is ResponseEngineSpeedTooHigh) {
									Log.Info(
										"Operating point with limited acceleration due to high engine speed resulted in an overload, searching again...");
									nextOperatingPoint = SearchOperatingPoint(absTime, ds, gradient, operatingPoint.Acceleration,
										retVal);
									DriverAcceleration = nextOperatingPoint.Acceleration;
									retVal = NextComponent.Request(absTime, nextOperatingPoint.SimulationInterval,
										nextOperatingPoint.Acceleration, gradient);
								} else {
									Log.Info(
										"Operating point with limited acceleration resulted in an overload! trying again with original acceleration {0}",
										nextOperatingPoint.Acceleration);
									DriverAcceleration = nextOperatingPoint.Acceleration;
									retVal = NextComponent.Request(absTime, nextOperatingPoint.SimulationInterval,
										nextOperatingPoint.Acceleration,
										gradient);
								}
							}
						}
						retVal.Switch().
							Case<ResponseSuccess>(() => operatingPoint = nextOperatingPoint).
							Case<ResponseGearShift>(() => operatingPoint = nextOperatingPoint).
							Default(
								r => { throw new UnexpectedResponseException("DrivingAction Accelerate after Overload", r); });
					}).
					Case<ResponseGearShift>(() => operatingPoint = limitedOperatingPoint).
					Case<ResponseFailTimeInterval>(r => {
						// occurs only with AT gearboxes - extend time interval after gearshift!
						retVal = new ResponseDrivingCycleDistanceExceeded {
							Source = this,
							MaxDistance = r.Acceleration / 2 * r.DeltaT * r.DeltaT + DataBus.VehicleSpeed * r.DeltaT
						};
					}).
					Case<ResponseSuccess>(() => operatingPoint = limitedOperatingPoint).
					Default(
						r => {
							throw new UnexpectedResponseException(
								"DrivingAction Accelerate after SearchOperatingPoint.", r);
						});
			}
			CurrentState.Acceleration = operatingPoint.Acceleration;
			CurrentState.dt = operatingPoint.SimulationInterval;
			CurrentState.Response = retVal;

			retVal.Acceleration = operatingPoint.Acceleration;
			retVal.SimulationInterval = operatingPoint.SimulationInterval;
			retVal.SimulationDistance = ds;
			retVal.OperatingPoint = operatingPoint;

			return retVal;
		}

		/// <summary>
		/// see documentation of IDriverActions
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="ds"></param>
		/// <param name="maxVelocity"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public IResponse DrivingActionCoast(Second absTime, Meter ds, MeterPerSecond maxVelocity, Radian gradient)
		{
			CurrentAction = "COAST";
			IterationStatistics.Increment(this, "Coast");
			Log.Debug("DrivingAction Coast");

			return CoastOrRollAction(absTime, ds, maxVelocity, gradient, false);
		}

		/// <summary>
		/// see documentation of IDriverActions
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="ds"></param>
		/// <param name="maxVelocity"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public IResponse DrivingActionRoll(Second absTime, Meter ds, MeterPerSecond maxVelocity, Radian gradient)
		{
			CurrentAction = "ROLL";
			IterationStatistics.Increment(this, "Roll");

			Log.Debug("DrivingAction Roll");

			var retVal = CoastOrRollAction(absTime, ds, maxVelocity, gradient, true);
			retVal.Switch().
				Case<ResponseGearShift>(
					() => {
						throw new UnexpectedResponseException("DrivingAction Roll: Gearshift during Roll action.",
							retVal);
					});

			return retVal;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="ds"></param>
		/// <param name="maxVelocity"></param>
		/// <param name="gradient"></param>
		/// <param name="rollAction"></param>
		/// <returns>
		/// * ResponseSuccess
		/// * ResponseDrivingCycleDistanceExceeded: vehicle is at low speed, coasting would lead to stop before ds is reached.
		/// * ResponseSpeedLimitExceeded: vehicle accelerates during coasting which would lead to exceeding the given maxVelocity (e.g., driving downhill, engine's drag load is not sufficient)
		/// * ResponseUnderload: engine's operating point is below drag curve (vehicle accelerates more than driver model allows; engine's drag load is not sufficient for limited acceleration
		/// * ResponseGearShift: gearbox needs to shift gears, vehicle can not accelerate (traction interruption)
		/// * ResponseFailTimeInterval: 
		/// </returns>
		protected IResponse CoastOrRollAction(Second absTime, Meter ds, MeterPerSecond maxVelocity, Radian gradient,
			bool rollAction)
		{
			var requestedOperatingPoint = ComputeAcceleration(ds, DataBus.VehicleSpeed);
			DriverAcceleration = requestedOperatingPoint.Acceleration;
			var initialResponse = NextComponent.Request(absTime, requestedOperatingPoint.SimulationInterval,
				requestedOperatingPoint.Acceleration, gradient, dryRun: true);

			OperatingPoint searchedOperatingPoint;
			try {
				searchedOperatingPoint = SearchOperatingPoint(absTime, requestedOperatingPoint.SimulationDistance,
					gradient,
					requestedOperatingPoint.Acceleration, initialResponse, coastingOrRoll: true);
			} catch (VectoEngineSpeedTooLowException) {
				// in case of an exception during search the engine-speed got too low - gear disengaged --> try again with disengaged gear.
				searchedOperatingPoint = SearchOperatingPoint(absTime, requestedOperatingPoint.SimulationDistance,
					gradient,
					requestedOperatingPoint.Acceleration, initialResponse, coastingOrRoll: true);
			}

			if (!ds.IsEqual(searchedOperatingPoint.SimulationDistance)) {
				// vehicle is at low speed, coasting would lead to stop before ds is reached: reduce simulated distance to stop distance.
				Log.Debug(
					"SearchOperatingPoint reduced the max. distance: {0} -> {1}. Issue new request from driving cycle!",
					searchedOperatingPoint.SimulationDistance, ds);
				CurrentState.Response = new ResponseDrivingCycleDistanceExceeded {
					Source = this,
					MaxDistance = searchedOperatingPoint.SimulationDistance,
					Acceleration = searchedOperatingPoint.Acceleration,
					SimulationInterval = searchedOperatingPoint.SimulationInterval,
					OperatingPoint = searchedOperatingPoint
				};
				return CurrentState.Response;
			}

			Log.Debug("Found operating point for {2}. dt: {0}, acceleration: {1}",
				searchedOperatingPoint.SimulationInterval,
				searchedOperatingPoint.Acceleration, rollAction ? "ROLL" : "COAST");

			var limitedOperatingPoint = LimitAccelerationByDriverModel(searchedOperatingPoint,
				rollAction ? LimitationMode.NoLimitation : LimitationMode.LimitDecelerationDriver);

			// compute speed at the end of the simulation interval. if it exceeds the limit -> return
			var v2 = DataBus.VehicleSpeed +
					limitedOperatingPoint.Acceleration * limitedOperatingPoint.SimulationInterval;
			if (v2 > maxVelocity && limitedOperatingPoint.Acceleration.IsGreaterOrEqual(0)) {
				Log.Debug("vehicle's velocity would exceed given max speed. v2: {0}, max speed: {1}", v2, maxVelocity);
				return new ResponseSpeedLimitExceeded() { Source = this };
			}

			DriverAcceleration = limitedOperatingPoint.Acceleration;
			var response = NextComponent.Request(absTime, limitedOperatingPoint.SimulationInterval,
				limitedOperatingPoint.Acceleration, gradient);

			response.SimulationInterval = limitedOperatingPoint.SimulationInterval;
			response.SimulationDistance = ds;
			response.Acceleration = limitedOperatingPoint.Acceleration;
			response.OperatingPoint = limitedOperatingPoint;

			response.Switch().
				Case<ResponseSuccess>().
				Case<ResponseUnderload>(). // driver limits acceleration, operating point may be below engine's 
				//drag load resp. below 0
				Case<ResponseOverload>(). // driver limits acceleration, operating point may be above 0 (GBX), use brakes
				Case<ResponseEngineSpeedTooHigh>(). // reduce acceleration/vehicle speed
				Case<ResponseGearShift>().
				Case<ResponseFailTimeInterval>(r => {
					response = new ResponseDrivingCycleDistanceExceeded {
						Source = this,
						MaxDistance = r.Acceleration / 2 * r.DeltaT * r.DeltaT + DataBus.VehicleSpeed * r.DeltaT
					};
				}).
				Default(
					() => {
						throw new UnexpectedResponseException(
							"CoastOrRoll Action: unhandled response from powertrain.", response);
					});

			CurrentState.Response = response;
			CurrentState.Acceleration = response.Acceleration;
			CurrentState.dt = response.SimulationInterval;
			return response;
		}

		public IResponse DrivingActionBrake(Second absTime, Meter ds, MeterPerSecond nextTargetSpeed, Radian gradient,
			IResponse previousResponse = null, Meter targetDistance = null)
		{
			CurrentAction = "BRAKE";
			IterationStatistics.Increment(this, "Brake");
			Log.Debug("DrivingAction Brake");

			IResponse retVal = null;

			var operatingPoint = ComputeAcceleration(ds, nextTargetSpeed);

			//if (operatingPoint.Acceleration.IsSmaller(0)) {
			// if we should brake with the max. deceleration and the deceleration changes within the current interval, take the larger deceleration...
			if (
				operatingPoint.Acceleration.IsEqual(
					DriverData.AccelerationCurve.Lookup(DataBus.VehicleSpeed).Deceleration)) {
				var v2 = DataBus.VehicleSpeed + operatingPoint.Acceleration * operatingPoint.SimulationInterval;
				var nextAcceleration = DriverData.AccelerationCurve.Lookup(v2).Deceleration;
				var tmp = ComputeTimeInterval(VectoMath.Min(operatingPoint.Acceleration, nextAcceleration),
					operatingPoint.SimulationDistance);
				if (!operatingPoint.Acceleration.IsEqual(nextAcceleration) &&
					operatingPoint.SimulationDistance.IsEqual(tmp.SimulationDistance)) {
					// only adjust operating point if the acceleration is different but the simulation distance is not modified
					// i.e., braking to the next sample point (but a little bit slower)
					Log.Debug("adjusting acceleration from {0} to {1}", operatingPoint.Acceleration, tmp.Acceleration);
					operatingPoint = tmp;
					// ComputeTimeInterval((operatingPoint.Acceleration + tmp.Acceleration) / 2, operatingPoint.SimulationDistance);
				}
			}
			if (targetDistance != null && targetDistance > DataBus.Distance) {
				var tmp = ComputeAcceleration(targetDistance - DataBus.Distance, nextTargetSpeed, false);
				if (tmp.Acceleration.IsGreater(operatingPoint.Acceleration)) {
					operatingPoint = ComputeTimeInterval(tmp.Acceleration, ds);
					if (!ds.IsEqual(operatingPoint.SimulationDistance)) {
						Log.Error(
							"Unexpected Condition: Distance has been adjusted from {0} to {1}, currentVelocity: {2} acceleration: {3}, targetVelocity: {4}",
							operatingPoint.SimulationDistance, ds, DataBus.VehicleSpeed, operatingPoint.Acceleration,
							nextTargetSpeed);
						throw new VectoSimulationException("Simulation distance unexpectedly adjusted! {0} -> {1}", ds,
							operatingPoint.SimulationDistance);
					}
				}
			}

			DriverAcceleration = operatingPoint.Acceleration;
			var response = previousResponse ??
							NextComponent.Request(absTime, operatingPoint.SimulationInterval, operatingPoint.Acceleration,
								gradient);

			var point = operatingPoint;
			response.Switch().
				Case<ResponseSuccess>(r => retVal = r).
				Case<ResponseOverload>(r => retVal = r)
				. // i.e., driving uphill, clutch open, deceleration higher than desired deceleration
				Case<ResponseUnderload>(). // will be handled in SearchBrakingPower
				Case<ResponseEngineSpeedTooHigh>(r => {
					Log.Debug("Engine speeed was too high, search for appropriate acceleration first.");
					operatingPoint = SearchOperatingPoint(absTime, ds, gradient, point.Acceleration,
						response);
				}). // will be handled in SearchBrakingPower
				Case<ResponseGearShift>(). // will be handled in SearchBrakingPower
				Case<ResponseFailTimeInterval>(r =>
					retVal = new ResponseDrivingCycleDistanceExceeded() {
						Source = this,
						MaxDistance = DataBus.VehicleSpeed * r.DeltaT + point.Acceleration / 2 * r.DeltaT * r.DeltaT
					}).
				Default(r => { throw new UnexpectedResponseException("DrivingAction Brake: first request.", r); });

			if (retVal != null) {
				CurrentState.Acceleration = operatingPoint.Acceleration;
				CurrentState.dt = operatingPoint.SimulationInterval;
				CurrentState.Response = retVal;
				retVal.Acceleration = operatingPoint.Acceleration;
				retVal.SimulationInterval = operatingPoint.SimulationInterval;
				retVal.SimulationDistance = ds;
				retVal.OperatingPoint = operatingPoint;
				return retVal;
			}

			operatingPoint = SearchBrakingPower(absTime, operatingPoint.SimulationDistance, gradient,
				operatingPoint.Acceleration, response);

			if (!ds.IsEqual(operatingPoint.SimulationDistance, 1E-15.SI<Meter>())) {
				Log.Info(
					"SearchOperatingPoint Braking reduced the max. distance: {0} -> {1}. Issue new request from driving cycle!",
					operatingPoint.SimulationDistance, ds);
				return new ResponseDrivingCycleDistanceExceeded {
					Source = this,
					MaxDistance = operatingPoint.SimulationDistance
				};
			}

			Log.Debug("Found operating point for braking. dt: {0}, acceleration: {1} brakingPower: {2}",
				operatingPoint.SimulationInterval,
				operatingPoint.Acceleration, DataBus.BrakePower);
			if (DataBus.BrakePower < 0) {
				var overload = new ResponseOverload {
					Source = this,
					BrakePower = DataBus.BrakePower,
					Acceleration = operatingPoint.Acceleration
				};
				DataBus.BrakePower = 0.SI<Watt>();
				return overload;
			}

			DriverAcceleration = operatingPoint.Acceleration;
			retVal = NextComponent.Request(absTime, operatingPoint.SimulationInterval, operatingPoint.Acceleration,
				gradient);

			retVal.Switch().
				Case<ResponseSuccess>().
				Case<ResponseGearShift>().
				Case<ResponseFailTimeInterval>(r =>
					retVal = new ResponseDrivingCycleDistanceExceeded() {
						Source = this,
						MaxDistance =
							DataBus.VehicleSpeed * r.DeltaT + operatingPoint.Acceleration / 2 * r.DeltaT * r.DeltaT
					}).
				Case<ResponseUnderload>(r => {
					if (DataBus.GearboxType.AutomaticTransmission()) {
						operatingPoint = SearchBrakingPower(absTime, operatingPoint.SimulationDistance, gradient,
							operatingPoint.Acceleration, response);
						DriverAcceleration = operatingPoint.Acceleration;
						retVal = NextComponent.Request(absTime, operatingPoint.SimulationInterval,
							operatingPoint.Acceleration, gradient);
					}
				}).
				Case<ResponseOverload>(r => {
					if (DataBus.GearboxType.AutomaticTransmission()) {
						// overload may happen because of gearshift between search and actual request, search again
						var i = 5;
						while (i-- > 0 && !(retVal is ResponseSuccess)) {
							DataBus.BrakePower = 0.SI<Watt>();
							operatingPoint = SearchBrakingPower(absTime, operatingPoint.SimulationDistance, gradient,
								operatingPoint.Acceleration, response);
							DriverAcceleration = operatingPoint.Acceleration;
							if (DataBus.BrakePower.IsSmaller(0)) {
								DataBus.BrakePower = 0.SI<Watt>();

								operatingPoint = SearchOperatingPoint(absTime, ds, gradient, 0.SI<MeterPerSquareSecond>(), r);
							}
							retVal = NextComponent.Request(absTime, operatingPoint.SimulationInterval,
								operatingPoint.Acceleration, gradient);
						}
					} else {
						throw new UnexpectedResponseException(
							"DrivingAction Brake: request failed after braking power was found.", r);
					}
				}).
				Default(
					r => {
						throw new UnexpectedResponseException(
							"DrivingAction Brake: request failed after braking power was found.", r);
					});
			CurrentState.Acceleration = operatingPoint.Acceleration;
			CurrentState.dt = operatingPoint.SimulationInterval;
			CurrentState.Response = retVal;
			retVal.Acceleration = operatingPoint.Acceleration;
			retVal.SimulationInterval = operatingPoint.SimulationInterval;
			retVal.SimulationDistance = ds;
			retVal.OperatingPoint = operatingPoint;

			return retVal;
		}

		// ================================================

		/// <summary>
		/// 
		/// </summary>
		/// <param name="operatingPoint"></param>
		/// <param name="limits"></param>
		/// <returns></returns>
		private OperatingPoint LimitAccelerationByDriverModel(OperatingPoint operatingPoint,
			LimitationMode limits)
		{
			var limitApplied = false;
			var originalAcceleration = operatingPoint.Acceleration;
			//if (((limits & LimitationMode.LimitDecelerationLookahead) != 0) &&
			//	operatingPoint.Acceleration < DriverData.LookAheadCoasting.Deceleration) {
			//	operatingPoint.Acceleration = DriverData.LookAheadCoasting.Deceleration;
			//	limitApplied = true;
			//}
			var accelerationLimits = DriverData.AccelerationCurve.Lookup(DataBus.VehicleSpeed);
			if (operatingPoint.Acceleration > accelerationLimits.Acceleration) {
				operatingPoint.Acceleration = accelerationLimits.Acceleration;
				limitApplied = true;
			}
			if (((limits & LimitationMode.LimitDecelerationDriver) != 0) &&
				operatingPoint.Acceleration < accelerationLimits.Deceleration) {
				operatingPoint.Acceleration = accelerationLimits.Deceleration;
				limitApplied = true;
			}
			if (limitApplied) {
				operatingPoint.SimulationInterval =
					ComputeTimeInterval(operatingPoint.Acceleration, operatingPoint.SimulationDistance)
						.SimulationInterval;
				Log.Debug("Limiting acceleration from {0} to {1}, dt: {2}", originalAcceleration,
					operatingPoint.Acceleration, operatingPoint.SimulationInterval);
			}
			return operatingPoint;
		}

		/// <summary>
		/// Performs a search for the required braking power such that the vehicle accelerates with the given acceleration.
		/// Returns a new operating point (a, ds, dt) where ds may be shorter due to vehicle stopping
		/// </summary>
		/// <returns>operating point (a, ds, dt) such that the vehicle accelerates with the given acceleration.</returns>
		private OperatingPoint SearchBrakingPower(Second absTime, Meter ds, Radian gradient,
			MeterPerSquareSecond acceleration, IResponse initialResponse)
		{
			IterationStatistics.Increment(this, "SearchBrakingPower", 0);

			var operatingPoint = new OperatingPoint { SimulationDistance = ds, Acceleration = acceleration };
			operatingPoint = ComputeTimeInterval(operatingPoint.Acceleration, ds);
			Watt deltaPower = null;
			initialResponse.Switch().
				Case<ResponseGearShift>(r => {
					IterationStatistics.Increment(this, "SearchBrakingPower");
					DriverAcceleration = operatingPoint.Acceleration;
					var nextResp = NextComponent.Request(absTime, operatingPoint.SimulationInterval,
						operatingPoint.Acceleration,
						gradient, true);
					deltaPower = nextResp.GearboxPowerRequest;
				}).
				Case<ResponseEngineSpeedTooHigh>(r => {
					IterationStatistics.Increment(this, "SearchBrakingPower");
					DriverAcceleration = operatingPoint.Acceleration;
					var nextResp = NextComponent.Request(absTime, operatingPoint.SimulationInterval,
						operatingPoint.Acceleration,
						gradient, true);
					deltaPower = nextResp.GearboxPowerRequest;
				}).
				Case<ResponseUnderload>(r =>
					deltaPower = DataBus.ClutchClosed(absTime) ? r.Delta : r.GearboxPowerRequest).
				Default(
					r => { throw new UnexpectedResponseException("cannot use response for searching braking power!", r); });

			try {
				DataBus.BrakePower = SearchAlgorithm.Search(DataBus.BrakePower, deltaPower,
					deltaPower.Abs() * (DataBus.GearboxType.AutomaticTransmission() ? 0.5 : 1),
					getYValue: result => {
						var response = (ResponseDryRun)result;
						return DataBus.ClutchClosed(absTime) ? response.DeltaDragLoad : response.GearboxPowerRequest;
					},
					evaluateFunction: x => {
						DataBus.BrakePower = x;
						operatingPoint = ComputeTimeInterval(operatingPoint.Acceleration, ds);

						IterationStatistics.Increment(this, "SearchBrakingPower");
						DriverAcceleration = operatingPoint.Acceleration;
						return NextComponent.Request(absTime, operatingPoint.SimulationInterval,
							operatingPoint.Acceleration, gradient,
							true);
					},
					criterion: result => {
						var response = (ResponseDryRun)result;
						var delta = DataBus.ClutchClosed(absTime)
							? response.DeltaDragLoad
							: response.GearboxPowerRequest;
						return delta.Value();
					});

				return operatingPoint;
			} catch (Exception) {
				Log.Error("Failed to find operating point for braking power! absTime: {0}", absTime);
				throw;
			}
		}

		protected OperatingPoint SearchOperatingPoint(Second absTime, Meter ds, Radian gradient,
			MeterPerSquareSecond acceleration, IResponse initialResponse, bool coastingOrRoll = false)
		{
			IterationStatistics.Increment(this, "SearchOperatingPoint", 0);

			var retVal = new OperatingPoint { Acceleration = acceleration, SimulationDistance = ds };

			var actionRoll = !DataBus.ClutchClosed(absTime);
			var searchEngineSpeed = false;

			Watt origDelta = null;
			if (actionRoll) {
				initialResponse.Switch().
					Case<ResponseDryRun>(r => origDelta = r.GearboxPowerRequest).
					Case<ResponseFailTimeInterval>(r => origDelta = r.GearboxPowerRequest).
					Default(r => { throw new UnexpectedResponseException("SearchOperatingPoint: Unknown response type.", r); });
			} else {
				initialResponse.Switch().
					Case<ResponseOverload>(r => origDelta = r.Delta).
					Case<ResponseEngineSpeedTooHigh>(r => {
						searchEngineSpeed = true;
						origDelta = r.DeltaEngineSpeed * 1.SI<NewtonMeter>();
					}). // search operating point in drive action after overload
					Case<ResponseDryRun>(r => origDelta = coastingOrRoll ? r.DeltaDragLoad : r.DeltaFullLoad).
					Default(r => { throw new UnexpectedResponseException("SearchOperatingPoint: Unknown response type.", r); });
			}
			var delta = origDelta;
			try {
				retVal.Acceleration = SearchAlgorithm.Search(acceleration, delta,
					Constants.SimulationSettings.OperatingPointInitialSearchIntervalAccelerating,
					getYValue: response => {
						var r = (ResponseDryRun)response;
						if (searchEngineSpeed) {
							return r.DeltaEngineSpeed * 1.SI<NewtonMeter>();
						}
						return actionRoll ? r.GearboxPowerRequest : (coastingOrRoll ? r.DeltaDragLoad : r.DeltaFullLoad);
					},
					evaluateFunction:
						acc => {
							// calculate new time interval only when vehiclespeed and acceleration are != 0
							// else: use same timeinterval as before.
							if (!(acc.IsEqual(0) && DataBus.VehicleSpeed.IsEqual(0))) {
								var tmp = ComputeTimeInterval(acc, ds);
								if (tmp.SimulationInterval.IsEqual(0.SI<Second>(), 1e-9.SI<Second>())) {
									throw new VectoSearchAbortedException(
										"next TimeInterval is 0. a: {0}, v: {1}, dt: {2}", acc,
										DataBus.VehicleSpeed, tmp.SimulationInterval);
								}
								retVal.Acceleration = tmp.Acceleration;
								retVal.SimulationInterval = tmp.SimulationInterval;
								retVal.SimulationDistance = tmp.SimulationDistance;
							}
							IterationStatistics.Increment(this, "SearchOperatingPoint");
							DriverAcceleration = acc;
							var response = NextComponent.Request(absTime, retVal.SimulationInterval, acc, gradient, true);
							response.OperatingPoint = retVal;
							return response;
						},
					criterion: response => {
						var r = (ResponseDryRun)response;
						if (searchEngineSpeed) {
							return r.DeltaEngineSpeed.Value();
						}
						delta = actionRoll
							? r.GearboxPowerRequest
							: (coastingOrRoll ? r.DeltaDragLoad : r.DeltaFullLoad);
						return delta.Value();
					},
					abortCriterion:
						(response, cnt) => {
							var r = (ResponseDryRun)response;
							if (r == null) {
								return false;
							}

							return !actionRoll && !ds.IsEqual(r.OperatingPoint.SimulationDistance);
						});
			} catch (VectoSearchAbortedException) {
				// search aborted, try to go ahead with the last acceleration
			} catch (Exception) {
				Log.Error("Failed to find operating point! absTime: {0}", absTime);
				throw;
			}

			if (!retVal.Acceleration.IsBetween(DriverData.AccelerationCurve.MaxDeceleration(),
				DriverData.AccelerationCurve.MaxAcceleration())) {
				Log.Info("Operating Point outside driver acceleration limits: a: {0}", retVal.Acceleration);
			}
			return ComputeTimeInterval(retVal.Acceleration, retVal.SimulationDistance);
		}

		/// <summary>
		/// compute the acceleration and time-interval such that the vehicle's velocity approaches the given target velocity
		/// - first compute the acceleration to reach the targetVelocity within the given distance
		/// - limit the acceleration/deceleration by the driver's acceleration curve
		/// - compute the time interval required to drive the given distance with the computed acceleration
		/// computed acceleration and time interval are stored in CurrentState!
		/// </summary>
		/// <param name="ds">distance to reach the next target speed</param>
		/// <param name="targetVelocity">next vehicle speed to decelerate to</param>
		/// <param name="limitByDriverModel">if set to false the required acceleration will be computed, regardless of the driver's acceleration curve</param>
		public OperatingPoint ComputeAcceleration(Meter ds, MeterPerSecond targetVelocity,
			bool limitByDriverModel = true)
		{
			var currentSpeed = DataBus.VehicleSpeed;
			var retVal = new OperatingPoint() { SimulationDistance = ds };

			// Δx = (v0+v1)/2 * Δt
			// => Δt = 2*Δx/(v0+v1) 
			var dt = 2 * ds / (currentSpeed + targetVelocity);

			// a = Δv / Δt
			var requiredAcceleration = (targetVelocity - currentSpeed) / dt;

			if (!limitByDriverModel) {
				return ComputeTimeInterval(requiredAcceleration, ds);
			}

			var maxAcceleration = DriverData.AccelerationCurve.Lookup(currentSpeed);

			if (requiredAcceleration > maxAcceleration.Acceleration) {
				requiredAcceleration = maxAcceleration.Acceleration;
			}
			if (requiredAcceleration < maxAcceleration.Deceleration) {
				requiredAcceleration = maxAcceleration.Deceleration;
			}

			retVal.Acceleration = requiredAcceleration;
			retVal = ComputeTimeInterval(retVal.Acceleration, ds);

			if (ds.IsEqual(retVal.SimulationDistance)) {
				return retVal;
			}

			// this case should not happen, acceleration has been computed such that the target speed
			// can be reached within ds.
			Log.Error(
				"Unexpected Condition: Distance has been adjusted from {0} to {1}, currentVelocity: {2} acceleration: {3}, targetVelocity: {4}",
				retVal.SimulationDistance, ds, currentSpeed, CurrentState.Acceleration, targetVelocity);
			throw new VectoSimulationException("Simulation distance unexpectedly adjusted! {0} -> {1}", ds,
				retVal.SimulationDistance);
		}

		/// <summary>
		/// computes the distance required to decelerate from the current velocity to the given target velocity considering
		/// the drivers acceleration/deceleration curve.
		/// </summary>
		/// <param name="targetSpeed"></param>
		/// <returns></returns>
		public Meter ComputeDecelerationDistance(MeterPerSecond targetSpeed)
		{
			return DriverData.AccelerationCurve.ComputeAccelerationDistance(DataBus.VehicleSpeed, targetSpeed);
		}

		/// <summary>
		/// Computes the time interval for driving the given distance ds with the vehicle's current speed and the given acceleration.
		/// If the distance ds can not be reached (i.e., the vehicle would halt before ds is reached) then the distance parameter is adjusted.
		/// Returns a new operating point (a, ds, dt)
		/// </summary>
		/// <param name="acceleration"></param>
		/// <param name="ds"></param>
		/// <returns>Operating point (a, ds, dt)</returns>
		private OperatingPoint ComputeTimeInterval(MeterPerSquareSecond acceleration, Meter ds)
		{
			return VectoMath.ComputeTimeInterval(DataBus.VehicleSpeed, acceleration, DataBus.Distance, ds);
		}

		/// <summary>
		/// simulate a certain time interval where the vehicle is stopped.
		/// </summary>
		/// <param name="absTime"></param>
		/// <param name="dt"></param>
		/// <param name="targetVelocity"></param>
		/// <param name="gradient"></param>
		/// <returns></returns>
		public IResponse DrivingActionHalt(Second absTime, Second dt, MeterPerSecond targetVelocity, Radian gradient)
		{
			CurrentAction = "HALT";
			if (!targetVelocity.IsEqual(0) || !DataBus.VehicleStopped) {
				Log.Error("TargetVelocity ({0}) and VehicleVelocity ({1}) must be zero when vehicle is halting!",
					targetVelocity,
					DataBus.VehicleSpeed);
				throw new VectoSimulationException(
					"TargetVelocity ({0}) and VehicleVelocity ({1}) must be zero when vehicle is halting!",
					targetVelocity,
					DataBus.VehicleSpeed);
			}

			DriverAcceleration = 0.SI<MeterPerSquareSecond>();
			var retVal = NextComponent.Request(absTime, dt, 0.SI<MeterPerSquareSecond>(), gradient);

			retVal.Switch().
				Case<ResponseGearShift>(r => {
					DriverAcceleration = 0.SI<MeterPerSquareSecond>();
					retVal = NextComponent.Request(absTime, dt, 0.SI<MeterPerSquareSecond>(), gradient);
				});
			CurrentState.dt = dt;
			CurrentState.Acceleration = 0.SI<MeterPerSquareSecond>();
			return retVal;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.acc] = CurrentState.Acceleration;
			container.SetDataValue("DriverAction", ActionToNumber(CurrentAction));
		}

		private int ActionToNumber(string currentAction)
		{
			switch (currentAction.ToUpper()) {
				case "HALT":
					return 0;
				case "ROLL":
					return 2;
				case "COAST":
					return 4;
				case "ACCELERATE":
					return 6;
				case "BRAKE":
					return -5;
				default:
					return -10;
			}
		}

		protected override void DoCommitSimulationStep()
		{
			if (!(CurrentState.Response is ResponseSuccess)) {
				throw new VectoSimulationException("Previous request did not succeed!");
			}
			CurrentState.Response = null;
		}

		public class DriverState
		{
			// ReSharper disable once InconsistentNaming
			public Second dt;
			public MeterPerSquareSecond Acceleration;
			public IResponse Response;
		}

		[Flags]
		protected enum LimitationMode
		{
			NoLimitation = 0x0,
			LimitDecelerationDriver = 0x2,
			//LimitDecelerationLookahead = 0x4
		}

		public DrivingBehavior DriverBehavior { get; set; }

		public MeterPerSquareSecond DriverAcceleration { get; protected set; }
	}
}