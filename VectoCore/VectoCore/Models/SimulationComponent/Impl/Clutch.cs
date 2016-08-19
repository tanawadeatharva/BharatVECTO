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

using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
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
	public class Clutch : StatefulProviderComponent<SimpleComponentState, ITnOutPort, ITnInPort, ITnOutPort>, IClutch,
		ITnOutPort, ITnInPort
	{
		private readonly PerSecond _idleSpeed;
		private readonly PerSecond _ratedSpeed;
		private const double ClutchEff = 1;
		private ClutchState _clutchState = ClutchState.ClutchSlipping;

		public ICombustionEngineIdleController IdleController
		{
			get { return _idleController; }
			set
			{
				_idleController = value;
				_idleController.RequestPort = NextComponent;
			}
		}

		private readonly SI _clutchSpeedSlippingFactor;
		private ICombustionEngineIdleController _idleController;

		protected Clutch(IVehicleContainer container) : base(container) {}

		public Clutch(IVehicleContainer container, CombustionEngineData engineData) : base(container)
		{
			_idleSpeed = engineData.IdleSpeed;
			_ratedSpeed = engineData.FullLoadCurve.RatedSpeed;
			_clutchSpeedSlippingFactor = Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed) /
										(_idleSpeed + Constants.SimulationSettings.ClutchClosingSpeedNorm * (_ratedSpeed - _idleSpeed));
		}

		public ClutchState State()
		{
			return _clutchState;
		}

		//public ITnOutPort IdleControlPort
		//{
		//	get { return NextComponent; }
		//}

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			NewtonMeter torqueIn;
			PerSecond engineSpeedIn;
			if (DataBus.DriverBehavior == DrivingBehavior.Halted /*DataBus.VehicleStopped*/) {
				_clutchState = ClutchState.ClutchOpened;
				engineSpeedIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
			} else {
				AddClutchLoss(outTorque, outAngularVelocity, out torqueIn, out engineSpeedIn);
			}
			PreviousState.SetState(torqueIn, outAngularVelocity, outTorque, outAngularVelocity);

			var retVal = NextComponent.Initialize(torqueIn, engineSpeedIn);
			retVal.ClutchPowerRequest = outTorque * outAngularVelocity;
			return retVal;
		}

		public virtual IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			if (outAngularVelocity == null) {
				Log.Debug("Invoking IdleController...");

				var retval = IdleController.Request(absTime, dt, outTorque, null, dryRun);
				retval.ClutchPowerRequest = 0.SI<Watt>();
				CurrentState.SetState(0.SI<NewtonMeter>(), retval.EngineSpeed, outTorque, retval.EngineSpeed);
				return retval;
			}
			if (IdleController != null) {
				IdleController.Reset();
			}

			Log.Debug("from Wheels: torque: {0}, angularVelocity: {1}, power {2}", outTorque, outAngularVelocity,
				Formulas.TorqueToPower(outTorque, outAngularVelocity));

			NewtonMeter torqueIn;
			PerSecond angularVelocityIn;
			if (DataBus.DriverBehavior == DrivingBehavior.Halted /*DataBus.VehicleStopped*/) {
				_clutchState = ClutchState.ClutchOpened;
				angularVelocityIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
			} else {
				AddClutchLoss(outTorque, outAngularVelocity, out torqueIn, out angularVelocityIn);
			}
			Log.Debug("to Engine:   torque: {0}, angularVelocity: {1}, power {2}", torqueIn, angularVelocityIn,
				Formulas.TorqueToPower(torqueIn, angularVelocityIn));
			CurrentState.SetState(torqueIn, angularVelocityIn, outTorque, outAngularVelocity);

			var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);

			retVal.ClutchPowerRequest = outTorque *
										((PreviousState.OutAngularVelocity ?? 0.SI<PerSecond>()) + CurrentState.OutAngularVelocity) / 2.0;
			return retVal;
		}

		private void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, out NewtonMeter torqueIn,
			out PerSecond angularVelocityIn)
		{
			torqueIn = torque;
			angularVelocityIn = angularVelocity;

			var engineSpeedNorm = (angularVelocity - _idleSpeed) / (_ratedSpeed - _idleSpeed);
			if (engineSpeedNorm < Constants.SimulationSettings.ClutchClosingSpeedNorm) {
				_clutchState = ClutchState.ClutchSlipping;
				// MQ: 27.5.2016: when angularVelocity is 0 (at the end of the simulation interval) don't use the 
				//     angularVelocity but average angular velocity
				//     Reason: if angularVelocity = 0 also the power (torque * angularVelocity) is 0 and then
				//             the torque demand for the engine is 0. no drag torque although vehicle has to decelerate
				//             "the clutch" eats up the whole torque
				var effectiveAngularVelocity = angularVelocity.IsEqual(0.SI<PerSecond>())
					? (PreviousState.OutAngularVelocity + angularVelocity) / 2
					: angularVelocity;
				var engineSpeed = VectoMath.Max(_idleSpeed, angularVelocity);

				angularVelocityIn = _clutchSpeedSlippingFactor * engineSpeed + _idleSpeed;
				torqueIn = torque * effectiveAngularVelocity / ClutchEff / ((angularVelocityIn));
			} else {
				_clutchState = ClutchState.ClutchClosed;
			}
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			if (PreviousState.InAngularVelocity == null || CurrentState.InAngularVelocity == null) {
				container[ModalResultField.P_clutch_out] = 0.SI<Watt>();
				container[ModalResultField.P_clutch_loss] = 0.SI<Watt>();
			} else {
				var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
				var avgInAngularVelocity = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
				container[ModalResultField.P_clutch_out] = CurrentState.OutTorque * avgOutAngularVelocity;
				container[ModalResultField.P_clutch_loss] = CurrentState.InTorque * avgInAngularVelocity -
															CurrentState.OutTorque * avgOutAngularVelocity;
				//(CurrentState.InTorque - CurrentState.OutTorque) * avgInAngularVelocity;
			}
		}
	}
}