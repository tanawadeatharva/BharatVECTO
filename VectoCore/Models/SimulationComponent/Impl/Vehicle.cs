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

using System;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Vehicle : VectoSimulationComponent, IVehicle, IMileageCounter, IFvInPort, IDriverDemandOutPort
	{
		private readonly CrossWindCorrectionCurve _airResistanceCurve;
		private readonly VehicleData _data;
		private VehicleState _currentState;
		private VehicleState _previousState;
		protected IFvOutPort NextComponent;

		public Vehicle(IVehicleContainer container, VehicleData data) : base(container)
		{
			_data = data;
			_previousState = new VehicleState { Distance = 0.SI<Meter>(), Velocity = 0.SI<MeterPerSecond>() };
			_currentState = new VehicleState { Distance = 0.SI<Meter>(), Velocity = 0.SI<MeterPerSecond>() };

			_airResistanceCurve = data.CrossWindCorrectionCurve;
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			// ReSharper disable once UseObjectOrCollectionInitializer @@@ computation of AirDragResistance (below) needs VecicleSpeed!
			_previousState = new VehicleState {
				Distance = DataBus.CycleStartDistance,
				Velocity = vehicleSpeed,
				RollingResistance = RollingResistance(roadGradient),
				SlopeResistance = SlopeResistance(roadGradient),
				AirDragResistance = AirDragResistance(0.SI<MeterPerSquareSecond>(), Constants.SimulationSettings.TargetTimeInterval)
			};
			_previousState.VehicleAccelerationForce = _previousState.RollingResistance
													+ _previousState.AirDragResistance
													+ _previousState.SlopeResistance;

			_currentState = new VehicleState {
				Distance = DataBus.CycleStartDistance,
				Velocity = vehicleSpeed,
				AirDragResistance = _previousState.AirDragResistance,
				RollingResistance = _previousState.RollingResistance,
				SlopeResistance = _previousState.SlopeResistance,
				VehicleAccelerationForce = _previousState.VehicleAccelerationForce
			};


			return NextComponent.Initialize(_currentState.VehicleAccelerationForce, vehicleSpeed);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration)
		{
			var tmp = _previousState.Velocity;
			// set vehicle speed to get accurate airdrag resistance
			_previousState.Velocity = vehicleSpeed;
			_currentState.Velocity = vehicleSpeed + startAcceleration * Constants.SimulationSettings.TargetTimeInterval;
			var vehicleAccelerationForce = DriverAcceleration(startAcceleration) + RollingResistance(roadGradient) +
											AirDragResistance(startAcceleration,
												Constants.SimulationSettings.TargetTimeInterval) +
											SlopeResistance(roadGradient);

			var retVal = NextComponent.Initialize(vehicleAccelerationForce, vehicleSpeed);

			_previousState.Velocity = tmp;
			_currentState.Velocity = tmp;
			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSquareSecond acceleration, Radian gradient,
			bool dryRun = false)
		{
			Log.Debug("from Wheels: acceleration: {0}", acceleration);
			_currentState.dt = dt;
			_currentState.Acceleration = acceleration;
			_currentState.Velocity = _previousState.Velocity + acceleration * dt;
			if (_currentState.Velocity.IsSmaller(0.SI<MeterPerSecond>(), Constants.SimulationSettings.VehicleSpeedHaltTolerance)) {
				_currentState.Velocity = 0.SI<MeterPerSecond>();
			}
			_currentState.Distance = _previousState.Distance + _previousState.Velocity * dt + acceleration * dt * dt / 2;

			_currentState.DriverAcceleration = DriverAcceleration(acceleration);
			_currentState.RollingResistance = RollingResistance(gradient);
			_currentState.AirDragResistance = AirDragResistance(acceleration, dt);
			_currentState.SlopeResistance = SlopeResistance(gradient);

			// DriverAcceleration = vehicleAccelerationForce - RollingResistance - AirDragResistance - SlopeResistance
			_currentState.VehicleAccelerationForce = _currentState.DriverAcceleration
													+ _currentState.RollingResistance
													+ _currentState.AirDragResistance
													+ _currentState.SlopeResistance;

			var retval = NextComponent.Request(absTime, dt, _currentState.VehicleAccelerationForce, _currentState.Velocity,
				dryRun);
			return retval;
		}

		public void Connect(IFvOutPort other)
		{
			NextComponent = other;
		}

		public Meter Distance
		{
			get { return _previousState.Distance; }
		}

		public MeterPerSecond VehicleSpeed
		{
			get { return _previousState.Velocity; }
		}

		public Kilogram VehicleMass
		{
			get { return _data.TotalCurbWeight(); }
		}

		public Kilogram VehicleLoading
		{
			get { return _data.Loading; }
		}

		public Kilogram TotalMass
		{
			get { return _data.TotalVehicleWeight(); }
		}

		public IFvInPort InPort()
		{
			return this;
		}

		public IDriverDemandOutPort OutPort()
		{
			return this;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var averageVelocity = (_previousState.Velocity + _currentState.Velocity) / 2;

			container[ModalResultField.v_act] = averageVelocity;
			container[ModalResultField.PaVeh] = (_previousState.VehicleAccelerationForce * _previousState.Velocity +
												_currentState.VehicleAccelerationForce * _currentState.Velocity) / 2.0;
			container[ModalResultField.Pgrad] = (_previousState.SlopeResistance * _previousState.Velocity +
												_currentState.SlopeResistance * _currentState.Velocity) / 2.0;
			container[ModalResultField.Proll] = (_previousState.RollingResistance * _previousState.Velocity +
												_currentState.RollingResistance * _currentState.Velocity) / 2.0;
			container[ModalResultField.Pair] = ComputeAirDragPowerLoss(_previousState.Velocity, _currentState.Velocity,
				_currentState.dt);

			// sanity check: is the vehicle in step with the cycle?
			if (container[ModalResultField.dist] == DBNull.Value) {
				Log.Warn("distance field is not set!");
			} else {
				var distance = (SI)container[ModalResultField.dist];
				if (!distance.IsEqual(_currentState.Distance, 1e-12.SI<Meter>())) {
					Log.Warn("distance diverges: {0}, distance: {1}", (distance - _currentState.Distance).Value(),
						distance);
				}
			}
		}

		protected override void DoCommitSimulationStep()
		{
			_previousState = _currentState;
			_currentState = new VehicleState();
		}

		protected Newton RollingResistance(Radian gradient)
		{
			var weight = _data.TotalVehicleWeight();
			var gravity = Physics.GravityAccelleration;
			var rollCoefficient = _data.TotalRollResistanceCoefficient;

			var retVal = Math.Cos(gradient.Value()) * weight * gravity * rollCoefficient;
			Log.Debug("RollingResistance: {0}", retVal);
			return retVal;
		}

		protected Newton DriverAcceleration(MeterPerSquareSecond accelleration)
		{
			var retVal = _data.TotalVehicleWeight() * accelleration;
			Log.Debug("DriverAcceleration: {0}", retVal);
			return retVal;
		}

		protected internal Newton SlopeResistance(Radian gradient)
		{
			var retVal = _data.TotalVehicleWeight() * Physics.GravityAccelleration * Math.Sin(gradient.Value());
			Log.Debug("SlopeResistance: {0}", retVal);
			return retVal;
		}

		protected internal Newton AirDragResistance(MeterPerSquareSecond acceleration, Second dt)
		{
			var vAverage = _previousState.Velocity + acceleration * dt / 2;
			if (vAverage.IsEqual(0)) {
				return 0.SI<Newton>();
			}
			var result = ComputeAirDragPowerLoss(_previousState.Velocity, _previousState.Velocity + acceleration * dt, dt) /
						vAverage;

			Log.Debug("AirDragResistance: {0}", result);
			return result;
		}

		private Watt ComputeAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt)
		{
			var vAverage = (v1 + v2) / 2;
			var CdA = _airResistanceCurve.EffectiveAirDragArea(vAverage);
			Watt averageAirDragPower;
			if (v1.IsEqual(v2)) {
				averageAirDragPower = (Physics.AirDensity / 2.0 * CdA * vAverage * vAverage * vAverage).Cast<Watt>();
			} else {
				// compute the average force within the current simulation interval
				// P(t) = k * v(t)^3  , v(t) = v0 + a * t  // a != 0
				// => P_avg = (CdA * rho/2)/(4*a * dt) * (v2^4 - v1^4)
				var acceleration = (v2 - v1) / dt;
				averageAirDragPower =
					(Physics.AirDensity / 2.0 * CdA * (v2 * v2 * v2 * v2 - v1 * v1 * v1 * v1) / (4 * acceleration * dt))
						.Cast<Watt>();
			}
			return averageAirDragPower;
		}

		public class VehicleState
		{
			public Newton AirDragResistance;
			public Meter Distance;
			public Newton DriverAcceleration;
			public Second dt;
			public Newton RollingResistance;
			public Newton SlopeResistance;
			public Newton VehicleAccelerationForce;
			public MeterPerSecond Velocity;
			public MeterPerSquareSecond Acceleration { get; set; }
		}
	}
}