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
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
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
		private double _ratio;

		public Retarder(IVehicleContainer cockpit, RetarderLossMap lossMap, double ratio) : base(cockpit)
		{
			_lossMap = lossMap;
			_ratio = ratio;
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_ret_loss] = (PreviousState.InTorque - PreviousState.OutTorque) * avgAngularSpeed;
			container[ModalResultField.P_retarder_in] = CurrentState.InTorque * avgAngularSpeed;
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
			var avgAngularSpeed = (PreviousState.InAngularVelocity + angularVelocity) / 2.0;
			var retarderTorqueLoss =
				_lossMap.RetarderLoss(avgAngularSpeed * _ratio, DataBus.ExecutionMode != ExecutionMode.Declaration) / _ratio;
			CurrentState.SetState(torque + retarderTorqueLoss, angularVelocity, torque, angularVelocity);

			return NextComponent.Request(absTime, dt, torque + retarderTorqueLoss, angularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var retarderTorqueLoss =
				_lossMap.RetarderLoss(angularVelocity * _ratio, DataBus.ExecutionMode != ExecutionMode.Declaration) / _ratio;
			PreviousState.SetState(torque + retarderTorqueLoss, angularVelocity, torque, angularVelocity);

			return NextComponent.Initialize(torque + retarderTorqueLoss, angularVelocity);
		}
	}
}