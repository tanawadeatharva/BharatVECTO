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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Retarder : StatefulVectoSimulationComponent<SimpleComponentState>, IPowerTrainComponent, ITnInPort,
		ITnOutPort
	{
		protected ITnOutPort NextComponent;

		private readonly RetarderLossMap _lossMap;

		public Retarder(IVehicleContainer cockpit, RetarderLossMap lossMap) : base(cockpit)
		{
			_lossMap = lossMap;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.PlossRetarder] = (PreviousState.PowerLoss() + CurrentState.PowerLoss()) / 2.0;
		}

		protected override void DoCommitSimulationStep() {}

		public ITnInPort InPort()
		{
			return this;
		}

		public ITnOutPort OutPort()
		{
			return this;
		}

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			if (angularVelocity == null) {
				return NextComponent.Request(absTime, dt, torque, null, dryRun);
			}
			var retarderTorqueLoss = _lossMap.RetarderLoss(angularVelocity);
			//_retarderLoss = retarderTorqueLoss * angularVelocity;
			CurrentState.SetState(torque + retarderTorqueLoss, angularVelocity, torque, angularVelocity);

			return NextComponent.Request(absTime, dt, torque + retarderTorqueLoss, angularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var retarderTorqueLoss = _lossMap.RetarderLoss(angularVelocity);
			PreviousState.SetState(torque + retarderTorqueLoss, angularVelocity, torque, angularVelocity);

			return NextComponent.Initialize(torque + retarderTorqueLoss, angularVelocity);
		}
	}
}