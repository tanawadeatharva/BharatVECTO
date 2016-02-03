/*
* Copyright 2015 European Union
*
* Licensed under the EUPL (the "Licence");
* You may not use this work except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* http://ec.europa.eu/idabc/eupl5
*
* Unless required by applicable law or agreed to in writing, software 
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and 
* limitations under the Licence.
*/

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

		public Wheels(IVehicleContainer cockpit, Meter rdyn)
			: base(cockpit)
		{
			_dynamicWheelRadius = rdyn;
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
			CurrentState.Torque  = force * _dynamicWheelRadius;
			CurrentState.AngularSpeed  = velocity / _dynamicWheelRadius;
			CurrentState.WheelsPowerRequest = CurrentState.Torque * CurrentState.AngularSpeed;
			var retVal = NextComponent.Request(absTime, dt, CurrentState.Torque, CurrentState.AngularSpeed, dryRun);
			retVal.WheelsPowerRequest = CurrentState.WheelsPowerRequest;
			return retVal;
		}


		public IResponse Initialize(Newton force, MeterPerSecond velocity)
		{
			PreviousState.Torque  = force * _dynamicWheelRadius;
			PreviousState.AngularSpeed = velocity / _dynamicWheelRadius;
			PreviousState.WheelsPowerRequest = PreviousState.Torque * PreviousState.AngularSpeed;

			return NextComponent.Initialize(PreviousState.Torque, PreviousState.AngularSpeed);
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
			//container[ModalResultField.Pwheel] = (CurrentState.WheelsPowerRequest + PreviousState.WheelsPowerRequest) / 2;
			container[ModalResultField.Pwheel] = (CurrentState.Torque *( CurrentState.AngularSpeed + PreviousState.AngularSpeed) / 2.0).Cast<Watt>();
		}

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}

		#endregion

		public class WheelsState
		{
			public Watt WheelsPowerRequest;

			public NewtonMeter Torque;
			public PerSecond AngularSpeed;
		}
	}
}