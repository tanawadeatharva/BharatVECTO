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

using NLog.Fluent;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Wheels : StatefulVectoSimulationComponent<Wheels.WheelsState>, IWheels, IFvOutPort, ITnInPort
	{
		protected ITnOutPort NextComponent;

		private readonly Meter _dynamicWheelRadius;
		private readonly KilogramSquareMeter _totalWheelsInertia;

		public Wheels(IVehicleContainer cockpit, Meter rdyn, KilogramSquareMeter totalWheelsInertia)
			: base(cockpit)
		{
			_dynamicWheelRadius = rdyn;
			_totalWheelsInertia = totalWheelsInertia;
		}

		#region IFvOutProvider

		public IFvOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region ITnInProvider

		public ITnInPort InPort()
		{
			return this;
		}

		#endregion

		#region IFvOutPort

		public IResponse Request(Second absTime, Second dt, Newton force, MeterPerSecond velocity, bool dryRun)
		{
			Log.Debug("request: force: {0}, velocity: {1}", force, velocity);

			CurrentState.AngularVelocity = velocity / _dynamicWheelRadius;
			CurrentState.InertiaTorqueLoss =
				(_totalWheelsInertia * (CurrentState.AngularVelocity - PreviousState.AngularVelocity) / dt).Cast<NewtonMeter>();

			//WheelsPowerRequest = (torque + CurrentState.InertiaTorqueLoss) * CurrentState.AngularVelocity;
			CurrentState.Torque = force * _dynamicWheelRadius + CurrentState.InertiaTorqueLoss;
			var retVal = NextComponent.Request(absTime, dt, CurrentState.Torque, CurrentState.AngularVelocity,
				dryRun);
			retVal.WheelsPowerRequest = CurrentState.PowerRequest();
			return retVal;
		}


		public IResponse Initialize(Newton force, MeterPerSecond velocity)
		{
			PreviousState.Torque = force * _dynamicWheelRadius;
			PreviousState.AngularVelocity = velocity / _dynamicWheelRadius;
			PreviousState.InertiaTorqueLoss = 0.SI<NewtonMeter>();

			return NextComponent.Initialize(PreviousState.Torque, PreviousState.AngularVelocity);
		}

		#endregion

		#region ITnInPort

		void ITnInPort.Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		#endregion

		#region VectoSimulationComponent

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.P_wheel_in] =
				(CurrentState.Torque * (CurrentState.AngularVelocity + PreviousState.AngularVelocity) / 2.0).Cast<Watt>();
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		#endregion

		public class WheelsState
		{
			public PerSecond AngularVelocity;
			public NewtonMeter Torque;
			public NewtonMeter InertiaTorqueLoss;

			public Watt PowerRequest()
			{
				return AngularVelocity * Torque;
			}
		}
	}
}