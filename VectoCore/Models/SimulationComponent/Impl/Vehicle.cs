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
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Vehicle : StatefulVectoSimulationComponent<Vehicle.VehicleState>, IVehicle, IMileageCounter, IFvInPort, IDriverDemandOutPort
	{
		private readonly CrossWindCorrectionCurve _airResistanceCurve;
		private readonly VehicleData _data;

		protected IFvOutPort NextComponent;

		public Vehicle(IVehicleContainer container, VehicleData data) : base(container)
		{
			_data = data;
			PreviousState = new VehicleState { Distance = 0.SI<Meter>(), Velocity = 0.SI<MeterPerSecond>() };
			CurrentState = new VehicleState { Distance = 0.SI<Meter>(), Velocity = 0.SI<MeterPerSecond>() };

			var values = DeclarationData.AirDrag.Lookup(_data.VehicleCategory);
			_airResistanceCurve = data.CrossWindCorrectionCurve;
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			// ReSharper disable once UseObjectOrCollectionInitializer @@@ computation of AirDragResistance requires previousState.Velocity to be initialized!
			PreviousState = new VehicleState {
				Distance = DataBus.CycleStartDistance,
				Velocity = vehicleSpeed,
				RollingResistance = RollingResistance(roadGradient),
				SlopeResistance = SlopeResistance(roadGradient),
			};
			PreviousState.AirDragResistance = AirDragResistance(0.SI<MeterPerSquareSecond>(),
				Constants.SimulationSettings.TargetTimeInterval);
			PreviousState.VehicleTractionForce = PreviousState.RollingResistance
													+ PreviousState.AirDragResistance
													+ PreviousState.SlopeResistance;

			CurrentState = new VehicleState {
				Distance = DataBus.CycleStartDistance,
				Velocity = vehicleSpeed,
				AirDragResistance = PreviousState.AirDragResistance,
				RollingResistance = PreviousState.RollingResistance,
				SlopeResistance = PreviousState.SlopeResistance,
				VehicleTractionForce = PreviousState.VehicleTractionForce
			};


			return NextComponent.Initialize(CurrentState.VehicleTractionForce, vehicleSpeed);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, MeterPerSquareSecond startAcceleration,
			Radian roadGradient)
		{
			var tmp = PreviousState.Velocity;
			// set vehicle speed to get accurate airdrag resistance
			PreviousState.Velocity = vehicleSpeed;
			CurrentState.Velocity = vehicleSpeed + startAcceleration * Constants.SimulationSettings.TargetTimeInterval;
			var vehicleAccelerationForce = DriverAcceleration(startAcceleration) + RollingResistance(roadGradient) +
											AirDragResistance(startAcceleration,
												Constants.SimulationSettings.TargetTimeInterval) +
											SlopeResistance(roadGradient);

			var retVal = NextComponent.Initialize(vehicleAccelerationForce, vehicleSpeed);

			PreviousState.Velocity = tmp;
			CurrentState.Velocity = tmp;
			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSquareSecond acceleration, Radian gradient,
			bool dryRun = false)
		{
			Log.Debug("from Wheels: acceleration: {0}", acceleration);
			CurrentState.dt = dt;
			CurrentState.Acceleration = acceleration;
			CurrentState.Velocity = PreviousState.Velocity + acceleration * dt;
			if (CurrentState.Velocity.IsEqual(0.SI<MeterPerSecond>(),
				Constants.SimulationSettings.VehicleSpeedHaltTolerance)) {
				CurrentState.Velocity = 0.SI<MeterPerSecond>();
			}
			CurrentState.Distance = PreviousState.Distance + PreviousState.Velocity * dt + acceleration * dt * dt / 2;

			CurrentState.DriverAcceleration = DriverAcceleration(acceleration);
			CurrentState.RollingResistance = RollingResistance(gradient);
			CurrentState.AirDragResistance = AirDragResistance(acceleration, dt);
			CurrentState.SlopeResistance = SlopeResistance(gradient);

			// DriverAcceleration = vehicleTractionForce - RollingResistance - AirDragResistance - SlopeResistance
			CurrentState.VehicleTractionForce = CurrentState.DriverAcceleration
													+ CurrentState.RollingResistance
													+ CurrentState.AirDragResistance
													+ CurrentState.SlopeResistance;

			var retval = NextComponent.Request(absTime, dt, CurrentState.VehicleTractionForce,
				CurrentState.Velocity,
				dryRun);
			return retval;
		}

		public void Connect(IFvOutPort other)
		{
			NextComponent = other;
		}

		public Meter Distance
		{
			get { return PreviousState.Distance; }
		}

		public MeterPerSecond VehicleSpeed
		{
			get { return PreviousState.Velocity; }
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
			var averageVelocity = (PreviousState.Velocity + CurrentState.Velocity) / 2;

			container[ModalResultField.v_act] = averageVelocity;

			//container[ModalResultField.P_veh_inertia] = ((_previousState.VehicleTractiveForce * _previousState.Velocity +
			//									_currentState.VehicleTractiveForce * _currentState.Velocity) / 2.0).Cast<Watt>();
			container[ModalResultField.P_veh_inertia] =
				(CurrentState.DriverAcceleration * (CurrentState.Velocity + PreviousState.Velocity) / 2).Cast<Watt>();
			container[ModalResultField.P_slope] = ((PreviousState.SlopeResistance * PreviousState.Velocity +
												CurrentState.SlopeResistance * CurrentState.Velocity) / 2.0).Cast<Watt>
				();
			container[ModalResultField.P_roll] = ((PreviousState.RollingResistance * PreviousState.Velocity +
												CurrentState.RollingResistance * CurrentState.Velocity) / 2.0)
				.Cast<Watt>();

			container[ModalResultField.P_air] = ComputeAirDragPowerLoss(PreviousState.Velocity, CurrentState.Velocity,
				CurrentState.dt);


			// sanity check: is the vehicle in step with the cycle?
			if (container[ModalResultField.dist] == DBNull.Value) {
				Log.Warn("distance field is not set!");
			} else {
				var distance = (SI)container[ModalResultField.dist];
				if (!distance.IsEqual(CurrentState.Distance, 1e-12.SI<Meter>())) {
					Log.Warn("distance diverges: {0}, distance: {1}", (distance - CurrentState.Distance).Value(),
						distance);
				}
			}
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		protected Newton RollingResistance(Radian gradient)
		{
			var retVal = (Math.Cos(gradient.Value()) * _data.TotalVehicleWeight() *
						Physics.GravityAccelleration *
						_data.TotalRollResistanceCoefficient).Cast<Newton>();
			Log.Debug("RollingResistance: {0}", retVal);
			return retVal;
		}

		protected Newton DriverAcceleration(MeterPerSquareSecond accelleration)
		{
			var retVal = (_data.TotalVehicleWeight() * accelleration).Cast<Newton>();
			Log.Debug("DriverAcceleration: {0}", retVal);
			return retVal;
		}

		protected internal Newton SlopeResistance(Radian gradient)
		{
			var retVal =
				(_data.TotalVehicleWeight() * Physics.GravityAccelleration * Math.Sin(gradient.Value())).Cast<Newton>();
			Log.Debug("SlopeResistance: {0}", retVal);
			return retVal;
		}

		protected internal Newton AirDragResistance(MeterPerSquareSecond acceleration, Second dt)
		{
			var vAverage = PreviousState.Velocity + acceleration * dt / 2;
			if (vAverage.IsEqual(0)) {
				return 0.SI<Newton>();
			}
			var result =
				(ComputeAirDragPowerLoss(PreviousState.Velocity, PreviousState.Velocity + acceleration * dt, dt) /
				vAverage).Cast<Newton>();

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
			public Newton VehicleTractionForce;
			public MeterPerSecond Velocity;
			public MeterPerSquareSecond Acceleration { get; set; }
		}
	}
}