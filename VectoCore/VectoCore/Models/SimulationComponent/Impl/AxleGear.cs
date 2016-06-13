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

using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AxleGear : StatefulVectoSimulationComponent<AxleGear.AxleGearState>, IPowerTrainComponent, ITnInPort,
		ITnOutPort
	{
		protected ITnOutPort NextComponent;

		[ValidateObject] internal readonly AxleGearData ModelData;

		public AxleGear(IVehicleContainer container, AxleGearData modelData) : base(container)
		{
			ModelData = modelData;
		}

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

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			Log.Debug("request: torque: {0}, angularVelocity: {1}", torque, angularVelocity);

			var inAngularVelocity = angularVelocity * ModelData.AxleGear.Ratio;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + angularVelocity) / 2.0;

			var torqueLossResult = ModelData.AxleGear.LossMap.GetTorqueLoss(avgOutAngularVelocity, torque);
			var inTorque = torque / ModelData.AxleGear.Ratio + torqueLossResult.Value;

			CurrentState.SetState(inTorque, inAngularVelocity, torque, angularVelocity);
			CurrentState.TorqueLossResult = torqueLossResult;

			var retVal = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);

			retVal.AxlegearPowerRequest = torque * (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			return retVal;
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var inAngularVelocity = angularVelocity * ModelData.AxleGear.Ratio;
			var torqueLossResult = ModelData.AxleGear.LossMap.GetTorqueLoss(angularVelocity, torque);
			var inTorque = torque / ModelData.AxleGear.Ratio + torqueLossResult.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, torque, angularVelocity);
			PreviousState.TorqueLossResult = torqueLossResult;

			return NextComponent.Initialize(inTorque, inAngularVelocity);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgAngularVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			container[ModalResultField.P_axle_loss] = (CurrentState.InTorque - CurrentState.OutTorque / ModelData.AxleGear.Ratio) *
													avgAngularVelocity;
			container[ModalResultField.P_axle_in] = CurrentState.InTorque * avgAngularVelocity;
		}

		protected override void DoCommitSimulationStep()
		{
			if (CurrentState.TorqueLossResult.Extrapolated) {
				Log.Warn("AxleGear LossMap data was extrapolated: range for loss map is not sufficient: n:{0}, torque:{1}",
					CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque);

				if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
					throw new VectoException(
						"AxleGear LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{0}, torque:{1}",
						CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque);
				}
			}
			AdvanceState();
		}

		public class AxleGearState : SimpleComponentState
		{
			public TransmissionLossMap.LossMapResult TorqueLossResult;
		}
	}
}