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
	public class Vehicle : StatefulVectoSimulationComponent<Vehicle.VehicleState>, IVehicle, IMileageCounter, IFvInPort,
		IDriverDemandOutPort
	{
		//private readonly CrossWindCorrectionCurve _airResistanceCurve;
		internal readonly VehicleData ModelData;

		protected IFvOutPort NextComponent;

		public Vehicle(IVehicleContainer container, VehicleData modelData) : base(container)
		{
			ModelData = modelData;
			modelData.CrossWindCorrectionCurve.SetDataBus(container);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient)
		{
			PreviousState = new VehicleState {
				Distance = DataBus.CycleStartDistance,
				Velocity = vehicleSpeed,
				RollingResistance = RollingResistance(roadGradient),
				SlopeResistance = SlopeResistance(roadGradient),
				AirDragResistance = AirDragResistance(vehicleSpeed, 0.SI<MeterPerSquareSecond>(),
					Constants.SimulationSettings.TargetTimeInterval),
			};
			PreviousState.VehicleTractionForce = PreviousState.RollingResistance
												+ PreviousState.AirDragResistance
												+ PreviousState.SlopeResistance;

			return NextComponent.Initialize(PreviousState.VehicleTractionForce, vehicleSpeed);
		}

		public IResponse Initialize(MeterPerSecond vehicleSpeed, Radian roadGradient, MeterPerSquareSecond startAcceleration)
		{
			//CurrentState.Velocity = vehicleSpeed + startAcceleration * Constants.SimulationSettings.TargetTimeInterval;
			var vehicleAccelerationForce = DriverAcceleration(startAcceleration)
											+ RollingResistance(roadGradient)
											+ AirDragResistance(vehicleSpeed, startAcceleration, Constants.SimulationSettings.TargetTimeInterval)
											+ SlopeResistance(roadGradient);

			var retVal = NextComponent.Initialize(vehicleAccelerationForce, vehicleSpeed);
			return retVal;
		}

		public IResponse Request(Second absTime, Second dt, MeterPerSquareSecond acceleration, Radian gradient,
			bool dryRun = false)
		{
			Log.Debug("Vehicle: acceleration: {0}", acceleration);
			CurrentState.SimulationInterval = dt;
			CurrentState.Acceleration = acceleration;
			CurrentState.Velocity = PreviousState.Velocity + acceleration * dt;
			if (CurrentState.Velocity.IsSmallerOrEqual(0.SI<MeterPerSecond>(),
				Constants.SimulationSettings.VehicleSpeedHaltTolerance)) {
				CurrentState.Velocity = 0.SI<MeterPerSecond>();
			}
			CurrentState.Distance = PreviousState.Distance + PreviousState.Velocity * dt + acceleration * dt * dt / 2;

			CurrentState.DriverAcceleration = DriverAcceleration(acceleration);
			CurrentState.RollingResistance = RollingResistance(gradient);
			CurrentState.AirDragResistance = AirDragResistance(PreviousState.Velocity, acceleration, dt);
			CurrentState.SlopeResistance = SlopeResistance(gradient);

			// DriverAcceleration = vehicleTractionForce - RollingResistance - AirDragResistance - SlopeResistance
			CurrentState.VehicleTractionForce = CurrentState.DriverAcceleration
												+ CurrentState.RollingResistance
												+ CurrentState.AirDragResistance
												+ CurrentState.SlopeResistance;

			var retval = NextComponent.Request(absTime, dt, CurrentState.VehicleTractionForce,
				CurrentState.Velocity, dryRun);
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
			get { return ModelData.TotalCurbWeight(); }
		}

		public Kilogram VehicleLoading
		{
			get { return ModelData.Loading; }
		}

		public Kilogram TotalMass
		{
			get { return ModelData.TotalVehicleWeight(); }
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
			var averageVelocity = (PreviousState.Velocity + CurrentState.Velocity) / 2.0;

			container[ModalResultField.v_act] = averageVelocity;

			container[ModalResultField.P_veh_inertia] = CurrentState.DriverAcceleration * averageVelocity;
			container[ModalResultField.P_roll] = CurrentState.RollingResistance * averageVelocity;
			container[ModalResultField.P_air] = CurrentState.AirDragResistance * averageVelocity;
			container[ModalResultField.P_slope] = CurrentState.SlopeResistance * averageVelocity;
			container[ModalResultField.P_trac] = CurrentState.VehicleTractionForce * averageVelocity;
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

		protected internal Newton RollingResistance(Radian gradient)
		{
			var weight = ModelData.TotalVehicleWeight();
			var gravity = Physics.GravityAccelleration;
			var rollCoefficient = ModelData.TotalRollResistanceCoefficient;

			var retVal = Math.Cos(gradient.Value()) * weight * gravity * rollCoefficient;
			Log.Debug("RollingResistance: {0}", retVal);
			return retVal;
		}

		protected internal Newton DriverAcceleration(MeterPerSquareSecond accelleration)
		{
			var retVal = ModelData.TotalVehicleWeight() * accelleration;
			Log.Debug("DriverAcceleration: {0}", retVal);
			return retVal;
		}

		protected internal Newton SlopeResistance(Radian gradient)
		{
			var retVal = ModelData.TotalVehicleWeight() * Physics.GravityAccelleration * Math.Sin(gradient.Value());
			Log.Debug("SlopeResistance: {0}", retVal);
			return retVal;
		}

		protected internal Newton AirDragResistance(MeterPerSecond previousVelocity, MeterPerSquareSecond acceleration,
			Second dt)
		{
			var vAverage = previousVelocity + acceleration * dt / 2;
			if (vAverage.IsEqual(0)) {
				return 0.SI<Newton>();
			}
			var result = ComputeAirDragPowerLoss(previousVelocity, previousVelocity + acceleration * dt, dt) /
						vAverage;

			Log.Debug("AirDragResistance: {0}", result);
			return result;
		}

		private Watt ComputeAirDragPowerLoss(MeterPerSecond v1, MeterPerSecond v2, Second dt)
		{
			return ModelData.CrossWindCorrectionCurve.AverageAirDragPowerLoss(v1, v2, dt);
		}

		public class VehicleState
		{
			public Meter Distance = 0.SI<Meter>();
			public Second SimulationInterval = 0.SI<Second>();
			public Newton AirDragResistance = 0.SI<Newton>();
			public Newton DriverAcceleration = 0.SI<Newton>();
			public Newton RollingResistance = 0.SI<Newton>();
			public Newton SlopeResistance = 0.SI<Newton>();
			public Newton VehicleTractionForce = 0.SI<Newton>();
			public MeterPerSecond Velocity = 0.SI<MeterPerSecond>();
			public MeterPerSquareSecond Acceleration = 0.SI<MeterPerSquareSecond>();

			public override string ToString()
			{
				return
					string.Format(
						"v: {0}  a: {1}, dt: {2}, driver_acc: {3}, roll_res: {4}, slope_res: {5}, air_drag: {6}, traction force: {7}",
						Velocity, Acceleration, SimulationInterval, DriverAcceleration, RollingResistance, SlopeResistance,
						AirDragResistance,
						VehicleTractionForce);
			}
		}
	}
}