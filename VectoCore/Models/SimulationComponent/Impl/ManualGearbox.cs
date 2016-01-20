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

using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ManualGearbox : Gearbox
	{
		public ManualGearbox(IVehicleContainer container, GearboxData gearboxData = null) : base(container, gearboxData) {}

		#region ITnOutPort

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var dt = Constants.SimulationSettings.TargetTimeInterval;
			ShiftTime = double.NegativeInfinity.SI<Second>();
			PowerLoss = null;

			if (Disengaged) {
				Gear = DataBus.Gear;
			}

			var inAngularVelocity = outAngularVelocity * Data.Gears[Gear].Ratio;
			var inTorque = Data.Gears[Gear].LossMap.GetInTorque(inAngularVelocity, outTorque);

			var torqueLossInertia = outAngularVelocity.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(inAngularVelocity, PreviousInAngularSpeed, Data.Inertia, dt) / inAngularVelocity;

			inTorque += torqueLossInertia;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			if (response is ResponseSuccess) {
				PreviousInAngularSpeed = inAngularVelocity;
				Disengaged = false;
			}

			return response;
		}


		/// <summary>
		/// Requests the gearbox in engaged mode. Sets the gear if no gear was set previously.
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><term>ResponseGearShift</term><description>if a shift is needed.</description></item>
		/// <item><term>else</term><description>Response from NextComponent.</description></item>
		/// </list>
		/// </returns>
		protected override IResponse RequestGearEngaged(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity,
			bool dryRun)
		{
			// Set a Gear if no gear was set and engineSpeed is not zero
			if (Disengaged && !outAngularVelocity.IsEqual(0)) {
				Disengaged = false;
				Gear = DataBus.Gear;
				Log.Debug("Gearbox engaged gear {0}", Gear);
			}

			var inEngineSpeed = outAngularVelocity * Data.Gears[Gear].Ratio;
			var inTorque = outAngularVelocity.IsEqual(0)
				? outTorque / Data.Gears[Gear].Ratio
				: Data.Gears[Gear].LossMap.GetInTorque(inEngineSpeed, outTorque);

			PowerLoss = inTorque * inEngineSpeed - outTorque * outAngularVelocity;

			if (!inEngineSpeed.IsEqual(0)) {
				PowerLossInertia = Formulas.InertiaPower(inEngineSpeed, PreviousInAngularSpeed, Data.Inertia, dt);
				inTorque += PowerLossInertia / inEngineSpeed;
			} else {
				PowerLossInertia = 0.SI<Watt>();
			}

			var shiftRequired = Gear != DataBus.Gear;

			if (shiftRequired) {
				ShiftTime = absTime + Data.TractionInterruption;

				Log.Debug("Gearbox is shifting. absTime: {0}, dt: {1}, shiftTime: {2}, out: ({3}, {4}), in: ({5}, {6})", absTime, dt,
					ShiftTime, outTorque, outAngularVelocity, inTorque, inEngineSpeed);

				Disengaged = true;
				Log.Info("Gearbox disengaged");

				return new ResponseGearShift {
					Source = this,
					SimulationInterval = Data.TractionInterruption,
					GearboxPowerRequest = outTorque * outAngularVelocity
				};
			}

			var response = NextComponent.Request(absTime, dt, inTorque, inEngineSpeed);
			response.GearboxPowerRequest = outTorque * outAngularVelocity;

			PreviousInAngularSpeed = inEngineSpeed;
			return response;
		}

		#endregion
	}
}