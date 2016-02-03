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

using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AxleGear : VectoSimulationComponent, IPowerTrainComponent, ITnInPort, ITnOutPort
	{
		protected ITnOutPort NextComponent;
		private readonly AxleGearData _gearData;

		protected Watt Loss;

		public AxleGear(VehicleContainer container, AxleGearData gearData) : base(container)
		{
			_gearData = gearData;
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

			var inAngularVelocity = angularVelocity * _gearData.AxleGear.Ratio;
			var inTorque = angularVelocity.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: _gearData.AxleGear.LossMap.GetInTorque(inAngularVelocity, torque);

			var outPower = torque * angularVelocity;
			var inPower = inTorque * inAngularVelocity;
			Loss = inPower - outPower;

			var retVal = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);

			retVal.AxlegearPowerRequest = torque * angularVelocity;
			return retVal;
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			var inAngularVelocity = angularVelocity * _gearData.AxleGear.Ratio;
			var inTorque = _gearData.AxleGear.LossMap.GetInTorque(inAngularVelocity, torque);

			return NextComponent.Initialize(inTorque, inAngularVelocity);
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.PlossDiff] = Loss;
		}

		protected override void DoCommitSimulationStep()
		{
			if (_gearData.AxleGear.LossMap.Extrapolated) {
				// todo (MK, 2015-12-14): should we throw an interpolation error in EngineOnly Mode also?
				if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
					throw new VectoException("AxleGear LossMap data was extrapolated: range for loss map is not sufficient.");
				} else {
					Log.Warn("AxleGear LossMap data was extrapolated.");
				}
			}
			Loss = null;
		}
	}
}