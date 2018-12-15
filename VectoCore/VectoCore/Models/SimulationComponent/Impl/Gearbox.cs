/*
* This file is part of VECTO.
*
* Copyright © 2012-2017 European Union
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
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Gearbox : AbstractGearbox<GearboxState>
	{
		/// <summary>
		/// The shift strategy.
		/// </summary>
		private readonly IShiftStrategy _strategy;

		/// <summary>
		/// Time when a gearbox shift engages a new gear (shift is finished). Is set when shifting is needed.
		/// </summary>
		private Second _engageTime = 0.SI<Second>();

		/// <summary>
		/// True if gearbox is disengaged (no gear is set).
		/// </summary>
		protected internal bool Disengaged = true;

		public Second LastUpshift { get; protected internal set; }

		public Second LastDownshift { get; protected internal set; }

		public override GearInfo NextGear
		{
			get { return _strategy.NextGear; }
		}

		public override bool ClutchClosed(Second absTime)
		{
			return _engageTime.IsSmallerOrEqual(absTime);
		}

		public Gearbox(IVehicleContainer container, IShiftStrategy strategy, VectoRunData runData) : base(container, runData)
		{
			_strategy = strategy;
			_strategy.Gearbox = this;

			LastDownshift = -double.MaxValue.SI<Second>();
			LastUpshift = -double.MaxValue.SI<Second>();
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			_engageTime = -double.MaxValue.SI<Second>();

			if (Disengaged) {
				Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;
			var gearboxTorqueLoss = ModelData.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = gearboxTorqueLoss;

			var inTorque = outTorque / ModelData.Gears[Gear].Ratio
							+ gearboxTorqueLoss.Value;

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;
			Disengaged = false;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);

			return response;
		}

		public override bool TCLocked { get { return true; } }

		internal ResponseDryRun Initialize(uint gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var oldGear = Gear;
			Gear = gear;
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var torqueLossResult = ModelData.Gears[gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			CurrentState.TorqueLossResult = torqueLossResult;
			var inTorque = outTorque / ModelData.Gears[gear].Ratio + torqueLossResult.Value;

			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = ModelData.Inertia.IsEqual(0)
					? 0.SI<PerSquareSecond>()
					: outTorque / ModelData.Inertia;

				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, ModelData.Inertia,
					Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response =
				(ResponseDryRun)
					NextComponent.Request(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, inTorque,
						inAngularVelocity, true); //NextComponent.Initialize(inTorque, inAngularVelocity);
			//response.Switch().
			//	Case<ResponseSuccess>().
			//	Case<ResponseOverload>().
			//	Case<ResponseUnderload>().
			//	Default(r => { throw new UnexpectedResponseException("Gearbox.Initialize", r); });

			var fullLoad = DataBus.EngineStationaryFullPower(inAngularVelocity);

			Gear = oldGear;
			return new ResponseDryRun {
				Source = this,
				EnginePowerRequest = response.EnginePowerRequest,
				EngineSpeed = response.EngineSpeed,
				DynamicFullLoadPower = response.DynamicFullLoadPower,
				ClutchPowerRequest = response.ClutchPowerRequest,
				GearboxPowerRequest = outTorque * outAngularVelocity,
				DeltaFullLoad = response.EnginePowerRequest - fullLoad
			};
		}

		/// <summary>
		/// Requests the Gearbox to deliver torque and angularVelocity
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><description>ResponseDryRun</description></item>
		/// <item><description>ResponseOverload</description></item>
		/// <item><description>ResponseGearshift</description></item>
		/// </list>
		/// </returns>
		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);
			if (DataBus.VehicleStopped) {
				_engageTime = absTime;
				LastDownshift = -double.MaxValue.SI<Second>();
				LastUpshift = -double.MaxValue.SI<Second>();
			}
			if (DataBus.DriverBehavior == DrivingBehavior.Halted) {
				_engageTime = absTime + dt;
			}

			var gear = NextGear.Gear;
			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLossResult = ModelData.Gears[gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			if (avgOutAngularVelocity.IsEqual(0, 1e-9)) {
				inTorqueLossResult.Value = 0.SI<NewtonMeter>();
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var inTorque = outTorque / ModelData.Gears[gear].Ratio + inTorqueLossResult.Value;
			var inertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += inertiaTorqueLossOut / ModelData.Gears[gear].Ratio;

			var halted = DataBus.DrivingAction == DrivingAction.Halt;
			var driverDeceleratingNegTorque = DataBus.DriverBehavior == DrivingBehavior.Braking &&
											(DataBus.BrakePower.IsGreater(0) || inTorque < 0);
			var vehiclespeedBelowThreshold =
				DataBus.VehicleSpeed.IsSmaller(Constants.SimulationSettings.ClutchDisengageWhenHaltingSpeed);
			if (halted || (driverDeceleratingNegTorque && vehiclespeedBelowThreshold)) {
				_engageTime = VectoMath.Max(_engageTime, absTime + dt);

				return RequestGearDisengaged(absTime, dt, outTorque, outAngularVelocity, inTorque, dryRun);
			}

			return ClutchClosed(absTime)
				? RequestGearEngaged(absTime, dt, outTorque, outAngularVelocity, inTorque, inTorqueLossResult, inertiaTorqueLossOut, dryRun)
				: RequestGearDisengaged(absTime, dt, outTorque, outAngularVelocity, inTorque, dryRun);
		}

		/// <summary>
		/// Requests the Gearbox in Disengaged mode
		/// </summary>
		/// <returns>
		/// <list type="bullet">
		/// <item><term>ResponseDryRun</term><description>if dryRun, immediate return!</description></item>
		/// <item><term>ResponseFailTimeInterval</term><description>if shiftTime would be exceeded by current step</description></item>
		/// <item><term>ResponseOverload</term><description>if torque &gt; 0</description></item>
		/// <item><term>ResponseUnderload</term><description>if torque &lt; 0</description></item>
		/// <item><term>else</term><description>Response from NextComponent</description></item>
		/// </list>
		/// </returns>
		private IResponse RequestGearDisengaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			bool dryRun)
		{
			Disengaged = true;
			Log.Debug("Current Gear: Neutral");

			var avgAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

			var gear = NextGear.Gear;



			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;

			var avgInAngularVelocity = (PreviousState.InAngularVelocity + inAngularVelocity) / 2.0;

			if (dryRun) {
				// if gearbox is disengaged the 0[W]-line is the limit for drag and full load.
				var delta = inTorque * avgInAngularVelocity;
				return new ResponseDryRun {
					Source = this,
					GearboxPowerRequest = delta,
					DeltaDragLoad = delta,
					DeltaFullLoad = delta,
				};
			}

			var shiftTimeExceeded = absTime.IsSmaller(_engageTime) &&
									_engageTime.IsSmaller(absTime + dt, Constants.SimulationSettings.LowerBoundTimeInterval);
			// allow 5% tolerance of shift time
			if (shiftTimeExceeded && _engageTime - absTime > Constants.SimulationSettings.LowerBoundTimeInterval / 2) {
				return new ResponseFailTimeInterval {
					Source = this,
					DeltaT = _engageTime - absTime,
					GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0
				};
			}

			if ((inTorque * avgInAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseOverload {
					Source = this,
					Delta = inTorque * avgInAngularVelocity,
					GearboxPowerRequest = inTorque * avgInAngularVelocity
				};
			}

			if ((inTorque * avgInAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseUnderload {
					Source = this,
					Delta = inTorque * avgInAngularVelocity,
					GearboxPowerRequest = inTorque * avgInAngularVelocity
				};
			}

			//var inTorque = 0.SI<NewtonMeter>();
			if (avgInAngularVelocity.Equals(0.SI<PerSecond>())) {
				inTorque = 0.SI<NewtonMeter>();
			}

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque,
				outAngularVelocity);
			CurrentState.Gear = gear;
			CurrentState.TransmissionTorqueLoss = inTorque * ModelData.Gears[gear].Ratio - outTorque;

			var response = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), inAngularVelocity);

			//CurrentState.InAngularVelocity = response.EngineSpeed;

			response.GearboxPowerRequest = outTorque * avgAngularVelocity;

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
		private IResponse RequestGearEngaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, TransmissionLossMap.LossMapResult inTorqueLossResult, NewtonMeter inertiaTorqueLossOut, bool dryRun)
		{
			// Set a Gear if no gear was set and engineSpeed is not zero
			//if (!Disengaged && DataBus.VehicleStopped && !outAngularVelocity.IsEqual(0))
			//{
			//	Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			//}
			if (Disengaged && !outAngularVelocity.IsEqual(0)) {
				ReEngageGear(absTime, dt, outTorque, outAngularVelocity);
				Log.Debug("Gearbox engaged gear {0}", Gear);
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;

			if (dryRun) {
				var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
				dryRunResponse.GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
				return dryRunResponse;
			}

			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity);
			var shiftAllowed = !inAngularVelocity.IsEqual(0) && !DataBus.VehicleSpeed.IsEqual(0);

			if (response is ResponseSuccess && shiftAllowed) {
				var shiftRequired = _strategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque,
					response.EngineSpeed, Gear, _engageTime);

				if (shiftRequired) {
					_engageTime = absTime + ModelData.TractionInterruption;

					Log.Debug("Gearbox is shifting. absTime: {0}, dt: {1}, interuptionTime: {2}, out: ({3}, {4}), in: ({5}, {6})",
						absTime,
						dt, _engageTime, outTorque, outAngularVelocity, inTorque, inAngularVelocity);

					Disengaged = true;
					_strategy.Disengage(absTime, dt, outTorque, outAngularVelocity);
					Log.Info("Gearbox disengaged");

					return new ResponseGearShift {
						Source = this,
						SimulationInterval = ModelData.TractionInterruption,
						GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0
					};
				}
			}

			// this code has to be _after_ the check for a potential gear-shift!
			// (the above block issues dry-run requests and thus may update the CurrentState!)
			// begin critical section
			CurrentState.TransmissionTorqueLoss = inTorque * ModelData.Gears[Gear].Ratio - outTorque;
			// MQ 19.2.2016: check! inertia is related to output side, torque loss accounts to input side
			CurrentState.InertiaTorqueLossOut = inertiaTorqueLossOut;


			CurrentState.TorqueLossResult = inTorqueLossResult;
			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;
			// end critical section


			response.GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;

			return response;
		}

		private void ReEngageGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			Disengaged = false;
			var lastGear = Gear;
			Gear = DataBus.VehicleStopped
				? _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity)
				: _strategy.Engage(absTime, dt, outTorque, outAngularVelocity);
			if (!DataBus.VehicleStopped) {
				if (Gear > lastGear) {
					LastUpshift = absTime;
				}
				if (Gear < lastGear) {
					LastDownshift = absTime;
				}
			}
		}

		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.InAngularVelocity + CurrentState.InAngularVelocity) / 2.0;
			var avgOutAngularSpeed = (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;
			var inPower = CurrentState.InTorque * avgInAngularSpeed;
			var outPower = CurrentState.OutTorque * avgOutAngularSpeed;
			container[ModalResultField.Gear] = Disengaged || DataBus.VehicleStopped ? 0 : Gear;
			container[ModalResultField.P_gbx_loss] = inPower - outPower;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgOutAngularSpeed;
			container[ModalResultField.P_gbx_in] = inPower;
			container[ModalResultField.n_gbx_out_avg] = (PreviousState.OutAngularVelocity +
														CurrentState.OutAngularVelocity) / 2.0;
			container[ModalResultField.T_gbx_out] = CurrentState.OutTorque;
		}

		protected override void DoCommitSimulationStep()
		{
			if (!Disengaged) {
				if (CurrentState.TorqueLossResult != null && CurrentState.TorqueLossResult.Extrapolated) {
					Log.Warn(
						"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
						Gear, CurrentState.OutAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.OutTorque,
						ModelData.Gears[Gear].Ratio);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
							Gear, CurrentState.InAngularVelocity.ConvertToRoundsPerMinute(), CurrentState.InTorque,
							ModelData.Gears[Gear].Ratio);
					}
				}
			}
			if (DataBus.VehicleStopped) {
				Disengaged = true;
				_engageTime = -double.MaxValue.SI<Second>();
			}
			base.DoCommitSimulationStep();
		}
	}
}
