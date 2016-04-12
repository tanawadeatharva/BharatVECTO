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
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Clutch : StatefulVectoSimulationComponent<SimpleComponentState>, IClutch, ITnOutPort, ITnInPort
	{
		private readonly PerSecond _idleSpeed;
		private readonly PerSecond _ratedSpeed;
		protected ITnOutPort NextComponent;
		private const double ClutchEff = 1;
		private ClutchState _clutchState = ClutchState.ClutchSlipping;

		protected ICombustionEngineIdleController IdleController;

		protected Clutch(IVehicleContainer container) : base(container) {}


		public Clutch(IVehicleContainer container, CombustionEngineData engineData,
			ICombustionEngineIdleController idleController)
			: base(container)
		{
			_idleSpeed = engineData.IdleSpeed;
			_ratedSpeed = engineData.FullLoadCurve.RatedSpeed;
			IdleController = idleController;
		}

		public ClutchState State()
		{
			return _clutchState;
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

		public ITnOutPort IdleControlPort
		{
			get { return NextComponent; }
		}


		public virtual IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			NewtonMeter torqueIn;
			PerSecond engineSpeedIn;
			AddClutchLoss(torque, angularVelocity, out torqueIn, out engineSpeedIn);
			PreviousState.SetState(torqueIn, angularVelocity, torque, angularVelocity);

			var retVal = NextComponent.Initialize(torqueIn, engineSpeedIn);
			retVal.ClutchPowerRequest = torque * angularVelocity;
			return retVal;
		}

		public virtual IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity,
			bool dryRun = false)
		{
			if (angularVelocity == null) {
				Log.Debug("Invoking IdleController...");

				var retval = IdleController.Request(absTime, dt, torque, null, dryRun);
				retval.ClutchPowerRequest = 0.SI<Watt>();
				CurrentState.SetState(0.SI<NewtonMeter>(), retval.EngineSpeed, torque, retval.EngineSpeed);
				return retval;
			}
			if (IdleController != null) {
				IdleController.Reset();
			}
			NewtonMeter torqueIn;
			PerSecond angularVelocityIn;
			AddClutchLoss(torque, angularVelocity, out torqueIn, out angularVelocityIn);

			CurrentState.SetState(torqueIn, angularVelocityIn, torque, angularVelocity);

			var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);

			retVal.ClutchPowerRequest = torque *
										((PreviousState.OutAngularVelocity ?? 0.SI<PerSecond>()) + CurrentState.OutAngularVelocity) / 2.0;
			return retVal;
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

		protected override void DoCommitSimulationStep()
		{
			AdvanceState();
		}


		private void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, out NewtonMeter torqueIn,
			out PerSecond angularVelocityIn)
		{
			Log.Debug("from Wheels: torque: {0}, angularVelocity: {1}, power {2}", torque, angularVelocity,
				Formulas.TorqueToPower(torque, angularVelocity));
			torqueIn = torque;
			angularVelocityIn = angularVelocity;

			if (DataBus.VehicleStopped) {
				_clutchState = ClutchState.ClutchOpened;
				angularVelocityIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
			} else {
				var engineSpeedNorm = (angularVelocity - _idleSpeed) / (_ratedSpeed - _idleSpeed);
				if (engineSpeedNorm < Constants.SimulationSettings.ClutchNormSpeed) {
					_clutchState = ClutchState.ClutchSlipping;

					var engineSpeed0 = VectoMath.Max(_idleSpeed, angularVelocity);
					var clutchSpeedNorm = Constants.SimulationSettings.ClutchNormSpeed /
										((_idleSpeed + Constants.SimulationSettings.ClutchNormSpeed * (_ratedSpeed - _idleSpeed)) / _ratedSpeed);
					angularVelocityIn =
						((clutchSpeedNorm * engineSpeed0 / _ratedSpeed) * (_ratedSpeed - _idleSpeed) + _idleSpeed).Radian.Cast<PerSecond>();

					torqueIn = (torque * angularVelocity) / ClutchEff / angularVelocityIn;
				} else {
					_clutchState = ClutchState.ClutchClosed;
				}
			}
			Log.Debug("to Engine:   torque: {0}, angularVelocity: {1}, power {2}", torqueIn, angularVelocityIn,
				Formulas.TorqueToPower(torqueIn, angularVelocityIn));
		}
	}
}