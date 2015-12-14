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

using System;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Wheels : VectoSimulationComponent, IWheels, IFvOutPort, ITnInPort
	{
		protected ITnOutPort NextComponent;
		private readonly Meter _dynamicWheelRadius;

		protected Watt WheelsPowerRequest { get; set; }

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
			var torque = force * _dynamicWheelRadius;
			var angularVelocity = velocity / _dynamicWheelRadius;
			WheelsPowerRequest = torque * angularVelocity;
			var retVal = NextComponent.Request(absTime, dt, torque, angularVelocity, dryRun);
			retVal.WheelsPowerRequest = WheelsPowerRequest;
			return retVal;
		}


		public IResponse Initialize(Newton force, MeterPerSecond velocity)
		{
			var torque = force * _dynamicWheelRadius;
			var angularVelocity = velocity / _dynamicWheelRadius;

			return NextComponent.Initialize(torque, angularVelocity);
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
			container[ModalResultField.Pwheel] = WheelsPowerRequest;
		}

		protected override void DoCommitSimulationStep()
		{
			// nothing to commit
		}

		#endregion
	}
}