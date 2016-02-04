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

using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Clutch : VectoSimulationComponent, IClutch, ITnOutPort, ITnInPort
	{
		private readonly PerSecond _idleSpeed;
		private readonly PerSecond _ratedSpeed;
		protected ITnOutPort NextComponent;
		private const double ClutchEff = 1;
		private ClutchState _clutchState = ClutchState.ClutchSlipping;

		protected ICombustionEngineIdleController IdleController;
		private Watt _requiredPower;
		private NewtonMeter _requiredTorque;


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

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			container[ModalResultField.Pe_clutch] = _requiredPower;
			container[ModalResultField.Tq_clutch] = _requiredTorque;
		}

		protected override void DoCommitSimulationStep()
		{
			_requiredPower = 0.SI<Watt>();
			_requiredTorque = 0.SI<NewtonMeter>();
		}

		public ITnInPort InPort()
		{
			return this;
		}

		public ITnOutPort OutPort()
		{
			return this;
		}

		public ITnOutPort IdleControlPort
		{
			get { return NextComponent; }
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun = false)
		{
			if (angularVelocity == null) {
				Log.Debug("Invoking IdleController...");

				_requiredPower = 0.SI<Watt>();
				_requiredTorque = 0.SI<NewtonMeter>();
				var retval = IdleController.Request(absTime, dt, torque, null, dryRun);
				retval.ClutchPowerRequest = _requiredPower;
				return retval;
			}
			if (IdleController != null) {
				IdleController.Reset();
			}
			NewtonMeter torqueIn;
			PerSecond angularVelocityIn;
			AddClutchLoss(torque, angularVelocity, out torqueIn, out angularVelocityIn);
			_requiredPower = torqueIn * angularVelocityIn;
			_requiredTorque = torqueIn;

			var retVal = NextComponent.Request(absTime, dt, torqueIn, angularVelocityIn, dryRun);
			retVal.ClutchPowerRequest = torque * angularVelocity;
			return retVal;
		}

		public IResponse Initialize(NewtonMeter torque, PerSecond angularVelocity)
		{
			NewtonMeter torqueIn;
			PerSecond engineSpeedIn;
			AddClutchLoss(torque, angularVelocity, out torqueIn, out engineSpeedIn);

			var retVal = NextComponent.Initialize(torqueIn, engineSpeedIn);
			retVal.ClutchPowerRequest = torque * angularVelocity;
			return retVal;
		}

		public void Connect(ITnOutPort other)
		{
			NextComponent = other;
		}

		protected virtual void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, out NewtonMeter torqueIn,
			out PerSecond engineSpeedIn)
		{
			Log.Debug("from Wheels: torque: {0}, angularVelocity: {1}, power {2}", torque, angularVelocity,
				Formulas.TorqueToPower(torque, angularVelocity));
			torqueIn = torque;
			engineSpeedIn = angularVelocity;

			if (DataBus.VehicleStopped) {
				_clutchState = ClutchState.ClutchOpened;
				engineSpeedIn = _idleSpeed;
				torqueIn = 0.SI<NewtonMeter>();
			} else {
				var engineSpeedNorm = (angularVelocity - _idleSpeed) / (_ratedSpeed - _idleSpeed);
				if (engineSpeedNorm < Constants.SimulationSettings.CluchNormSpeed) {
					_clutchState = ClutchState.ClutchSlipping;

					var engineSpeed0 = VectoMath.Max(_idleSpeed, angularVelocity);
					var clutchSpeedNorm = Constants.SimulationSettings.CluchNormSpeed /
										((_idleSpeed + Constants.SimulationSettings.CluchNormSpeed * (_ratedSpeed - _idleSpeed)) / _ratedSpeed);
					engineSpeedIn =
						((clutchSpeedNorm * engineSpeed0 / _ratedSpeed) * (_ratedSpeed - _idleSpeed) + _idleSpeed).Radian.Cast<PerSecond>();

					torqueIn = (torque * angularVelocity) / ClutchEff / engineSpeedIn;
				} else {
					_clutchState = ClutchState.ClutchClosed;
				}
			}
			Log.Debug("to Engine:   torque: {0}, angularVelocity: {1}, power {2}", torqueIn, engineSpeedIn,
				Formulas.TorqueToPower(torqueIn, engineSpeedIn));
		}
	}

	/// <summary>
	/// Clutch without losses and slipping behaviour for PWheel driving cycle.
	/// </summary>
	public class PWheelClutch : Clutch
	{
		public PWheelClutch(IVehicleContainer container, ICombustionEngineIdleController idleController) : base(container)
		{
			IdleController = idleController;
		}

		protected override void AddClutchLoss(NewtonMeter torque, PerSecond angularVelocity, out NewtonMeter torqueIn,
			out PerSecond engineSpeedIn)
		{
			torqueIn = torque;
			engineSpeedIn = angularVelocity;
		}
	}
}