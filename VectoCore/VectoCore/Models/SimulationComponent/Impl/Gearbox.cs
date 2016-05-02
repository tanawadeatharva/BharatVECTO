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

using System.Diagnostics;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Gearbox : StatefulVectoSimulationComponent<Gearbox.GearboxState>, IGearbox, ITnOutPort, ITnInPort,
		IClutchInfo
	{
		/// <summary>
		/// The next port.
		/// </summary>
		protected ITnOutPort NextComponent;

		/// <summary>
		/// The data and settings for the gearbox.
		/// </summary>
		internal readonly GearboxData ModelData;

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

		public bool ClutchClosed(Second absTime)
		{
			return _engageTime.IsSmallerOrEqual(absTime);
		}

		public Gearbox(IVehicleContainer container, GearboxData gearboxModelData, IShiftStrategy strategy) : base(container)
		{
			ModelData = gearboxModelData;
			_strategy = strategy;
			_strategy.Gearbox = this;
		}

		#region ITnInProvider

		public ITnInPort InPort()
		{
			return this;
		}

		#endregion

		#region ITnOutProvider

		[DebuggerHidden]
		public ITnOutPort OutPort()
		{
			return this;
		}

		#endregion

		#region IGearboxCockpit

		/// <summary>
		/// The current gear.
		/// </summary>
		public uint Gear { get; set; }

		[DebuggerHidden]
		public MeterPerSecond StartSpeed
		{
			get { return ModelData.StartSpeed; }
		}

		[DebuggerHidden]
		public MeterPerSquareSecond StartAcceleration
		{
			get { return ModelData.StartAcceleration; }
		}

		public FullLoadCurve GearFullLoadCurve
		{
			get { return Gear == 0 ? null : ModelData.Gears[Gear].FullLoadCurve; }
		}

		#endregion

		#region ITnOutPort

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			// MK 2016-02-10: SI doesn't allow inifinity anymore -- therefore simply a very negative value is used.
			_engageTime = -1e10.SI<Second>(); //double.NegativeInfinity.SI<Second>();

			if (Disengaged) {
				Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			}

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;
			var torqueLoss = ModelData.Gears[Gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / ModelData.Gears[Gear].Ratio + torqueLoss;
			var torqueLossInertia = outAngularVelocity.IsEqual(0)
				? 0.SI<NewtonMeter>()
				: Formulas.InertiaPower(inAngularVelocity, PreviousState.InAngularVelocity, ModelData.Inertia, dt) /
				inAngularVelocity;

			inTorque += torqueLossInertia;

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			PreviousState.Gear = Gear;
			Disengaged = false;

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);

			return response;
		}

		internal ResponseDryRun Initialize(uint gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
			var torqueLoss = ModelData.Gears[gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / ModelData.Gears[gear].Ratio + torqueLoss;

			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = (ModelData.Inertia.IsEqual(0))
					? 0.SI<PerSquareSecond>()
					: outTorque / ModelData.Inertia;

				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, ModelData.Inertia,
					Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			response.Switch().
				Case<ResponseSuccess>().
				Case<ResponseOverload>().
				Case<ResponseUnderload>().
				Default(r => { throw new UnexpectedResponseException("Gearbox.Initialize", r); });

			var fullLoad = DataBus.EngineStationaryFullPower(inAngularVelocity);
			if (ModelData.Gears[gear].FullLoadCurve != null) {
				var fullLoadGearbox = ModelData.Gears[gear].FullLoadCurve.FullLoadStationaryTorque(inAngularVelocity) *
									inAngularVelocity;
				fullLoad = VectoMath.Min(fullLoadGearbox, fullLoad);
			}

			return new ResponseDryRun {
				Source = this,
				EnginePowerRequest = response.EnginePowerRequest,
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
		public IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun)
		{
			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", torque, angularVelocity);
			if (DataBus.VehicleStopped) {
				_engageTime = absTime;
			}
			IResponse retVal;
			// TODO: MQ 2016/03/10: investigate further the effects of having the condition angularvelocity != 0
			if (ClutchClosed(absTime) /* && !angularVelocity.IsEqual(0) */) {
				retVal = RequestGearEngaged(absTime, dt, torque, angularVelocity, dryRun);
			} else {
				retVal = RequestGearDisengaged(absTime, dt, torque, angularVelocity, dryRun);
			}

			return retVal;
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
		private IResponse RequestGearDisengaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			Log.Debug("Current Gear: Neutral");

			var avgAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

			if (dryRun) {
				// if gearbox is disengaged the 0[W]-line is the limit for drag and full load.
				return new ResponseDryRun {
					Source = this,
					GearboxPowerRequest = outTorque * avgAngularVelocity,
					DeltaDragLoad = outTorque * avgAngularVelocity,
					DeltaFullLoad = outTorque * avgAngularVelocity,
				};
			}

			var shiftTimeExceeded = absTime.IsSmaller(_engageTime) &&
									_engageTime.IsSmaller(absTime + dt, ModelData.TractionInterruption / 20.0); // allow 5% tolerance of shift time
			if (shiftTimeExceeded) {
				return new ResponseFailTimeInterval {
					Source = this,
					DeltaT = _engageTime - absTime,
					GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0
				};
			}

			if ((outTorque * avgAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseOverload {
					Source = this,
					Delta = outTorque * avgAngularVelocity,
					GearboxPowerRequest = outTorque * avgAngularVelocity
				};
			}

			if ((outTorque * avgAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.LineSearchTolerance)) {
				return new ResponseUnderload {
					Source = this,
					Delta = outTorque * avgAngularVelocity,
					GearboxPowerRequest = outTorque * avgAngularVelocity
				};
			}

			CurrentState.SetState(0.SI<NewtonMeter>(), outAngularVelocity * ModelData.Gears[PreviousState.Gear].Ratio, outTorque,
				outAngularVelocity);
			CurrentState.Gear = PreviousState.Gear;

			var response = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), null);
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
		private IResponse RequestGearEngaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			// Set a Gear if no gear was set and engineSpeed is not zero
			if (Disengaged && !outAngularVelocity.IsEqual(0)) {
				Disengaged = false;
				if (DataBus.VehicleStopped) {
					Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
				} else {
					Gear = _strategy.Engage(absTime, dt, outTorque, outAngularVelocity);
				}

				Log.Debug("Gearbox engaged gear {0}", Gear);
			}

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			var inTorqueLoss = ModelData.Gears[Gear].LossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);
			var inTorque = outTorque / ModelData.Gears[Gear].Ratio + inTorqueLoss;

			var inAngularVelocity = outAngularVelocity * ModelData.Gears[Gear].Ratio;

			if (dryRun) {
				if ((DataBus.DriverBehavior == DrivingBehavior.Braking || DataBus.DriverBehavior == DrivingBehavior.Coasting) &&
					inAngularVelocity < DataBus.EngineIdleSpeed && DataBus.VehicleSpeed < Constants.SimulationSettings.VehicleStopClutchDisengageSpeed) {
					Disengaged = true;
					_engageTime = absTime + dt;
					_strategy.Disengage(absTime, dt, outTorque, outAngularVelocity);
					Log.Debug("EngineSpeed is below IdleSpeed, Gearbox disengage!");
					return new ResponseEngineSpeedTooLow { Source = this, GearboxPowerRequest = outTorque * outAngularVelocity };
				}
				var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, true);
				dryRunResponse.GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
				return dryRunResponse;
			}

			var shiftAllowed = !inAngularVelocity.IsEqual(0) && !DataBus.VehicleSpeed.IsEqual(0);

			if (shiftAllowed) {
				var shiftRequired = _strategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity,
					Gear, _engageTime);

				if (shiftRequired) {
					_engageTime = absTime + ModelData.TractionInterruption;

					Log.Debug("Gearbox is shifting. absTime: {0}, dt: {1}, interuptionTime: {2}, out: ({3}, {4}), in: ({5}, {6})", absTime,
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
			CurrentState.TransmissionTorqueLoss = inTorque - (outTorque / ModelData.Gears[Gear].Ratio);
			if (!inAngularVelocity.IsEqual(0)) {
				// MQ 19.2.2016: check! inertia is related to output side, torque loss accounts to input side
				CurrentState.InertiaTorqueLossOut =
					Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
					avgOutAngularVelocity;
				inTorque += CurrentState.InertiaTorqueLossOut / ModelData.Gears[Gear].Ratio;
			} else {
				CurrentState.InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			}
			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;
			// end critical section

			var response = NextComponent.Request(absTime, dt, inTorque, inAngularVelocity);
			response.GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + CurrentState.OutAngularVelocity) / 2.0;

			return response;
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
			//var avgAngularSpeed = ((PreviousState.InAngularVelocity * ModelData.Gears[CurrentState.Gear].Ratio /
			//						ModelData.Gears[PreviousState.Gear].Ratio) +
			//						CurrentState.InAngularVelocity) / 2.0;
			var avgInAngularSpeed = (PreviousState.OutAngularVelocity +
									CurrentState.OutAngularVelocity) / 2.0 * ModelData.Gears[Gear].Ratio;

			container[ModalResultField.Gear] = Disengaged || DataBus.VehicleStopped ? 0 : Gear;
			container[ModalResultField.P_gbx_loss] = CurrentState.TransmissionTorqueLoss * avgInAngularSpeed;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgInAngularSpeed;
			container[ModalResultField.P_gbx_in] = CurrentState.InTorque * avgInAngularSpeed;
		}

		protected override void DoCommitSimulationStep()
		{
			if (!Disengaged) {
				if (ModelData.Gears[Gear].LossMap.Extrapolated) {
					Log.Warn(
						"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
						Gear, CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque,
						ModelData.Gears[Gear].Ratio);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
							Gear, CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque,
							ModelData.Gears[Gear].Ratio);
					}
				}
			}

			AdvanceState();
		}

		#endregion

		public class GearboxState : SimpleComponentState
		{
			public NewtonMeter InertiaTorqueLossOut = 0.SI<NewtonMeter>();
			public NewtonMeter TransmissionTorqueLoss = 0.SI<NewtonMeter>();
			public uint Gear;
		}
	}
}