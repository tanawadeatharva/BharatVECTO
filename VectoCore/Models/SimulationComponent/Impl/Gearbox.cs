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

using System.Diagnostics;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Exceptions;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class Gearbox : VectoSimulationComponent, IGearbox, ITnOutPort, ITnInPort, IClutchInfo
	{
		/// <summary>
		/// The next port.
		/// </summary>
		protected ITnOutPort NextComponent;

		/// <summary>
		/// The data and settings for the gearbox.
		/// </summary>
		internal GearboxData Data;

		/// <summary>
		/// The shift strategy.
		/// </summary>
		protected readonly IShiftStrategy Strategy;

		/// <summary>
		/// Time when a gearbox shift engages a new gear (shift is finished). Is set when shifting is needed.
		/// </summary>
		protected Second ShiftTime = 0.SI<Second>();

		/// <summary>
		/// True if gearbox is disengaged (no gear is set).
		/// </summary>
		protected internal bool Disengaged = true;

		/// <summary>
		/// The power loss for the mod data.
		/// </summary>
		protected Watt PowerLoss;

		/// <summary>
		/// The inertia power loss for the mod data.
		/// </summary>
		protected Watt PowerLossInertia;

		/// <summary>
		/// The previous enginespeed for inertia calculation
		/// </summary>
		protected PerSecond PreviousInAngularSpeed = 0.SI<PerSecond>();

		public bool ClutchClosed(Second absTime)
		{
			return ShiftTime.IsSmallerOrEqual(absTime);
		}

		public Gearbox(IVehicleContainer container, GearboxData gearboxData, IShiftStrategy strategy) : base(container)
		{
			Data = gearboxData;
			Strategy = strategy;
			Strategy.Gearbox = this;
		}

		protected Gearbox(IVehicleContainer container, GearboxData gearboxData) : base(container)
		{
			Data = gearboxData;
		}

		#region ITnInProvider

		[DebuggerHidden]
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

		#region IGearboxInfo

		/// <summary>
		/// The current gear.
		/// </summary>
		public uint Gear { get; set; }

		public MeterPerSecond StartSpeed
		{
			get { return Data.StartSpeed; }
		}

		public MeterPerSquareSecond StartAcceleration
		{
			get { return Data.StartAcceleration; }
		}

		public FullLoadCurve GearFullLoadCurve
		{
			get { return Gear == 0 ? null : Data.Gears[Gear].FullLoadCurve; }
		}

		#endregion

		#region ITnOutPort

		public virtual IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var absTime = 0.SI<Second>();
			var dt = Constants.SimulationSettings.TargetTimeInterval;

			// MK 2016-02-10: SI doesn't allow inifinity anymore -- therefore simply a very negative value is used.
			ShiftTime = -1e10.SI<Second>(); //double.NegativeInfinity.SI<Second>();
			PowerLoss = null;
			VehicleStopped = DataBus.VehicleStopped;

				Gear = Strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);

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

		internal ResponseDryRun Initialize(uint gear, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var inAngularVelocity = outAngularVelocity * Data.Gears[gear].Ratio;
			var inTorque = Data.Gears[gear].LossMap.GetInTorque(inAngularVelocity, outTorque);
			VehicleStopped = DataBus.VehicleStopped;
			if (!inAngularVelocity.IsEqual(0)) {
				var alpha = Data.Inertia.IsEqual(0)
					? 0.SI<PerSquareSecond>()
					: outTorque / Data.Inertia;

				var inertiaPowerLoss = Formulas.InertiaPower(inAngularVelocity, alpha, Data.Inertia,
					Constants.SimulationSettings.TargetTimeInterval);
				inTorque += inertiaPowerLoss / inAngularVelocity;
			}

			var response = NextComponent.Initialize(inTorque, inAngularVelocity);
			response.Switch().
				Case<ResponseSuccess>().
				Case<ResponseOverload>().
				Case<ResponseUnderload>().
				Default(r => { throw new UnexpectedResponseException("Gearbox.Initialize", r); });

			var fullLoadGearbox = Data.Gears[gear].FullLoadCurve.FullLoadStationaryTorque(inAngularVelocity) * inAngularVelocity;
			var fullLoadEngine = DataBus.EngineStationaryFullPower(inAngularVelocity);

			var fullLoad = VectoMath.Min(fullLoadGearbox, fullLoadEngine);

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
		public virtual IResponse Request(Second absTime, Second dt, NewtonMeter torque, PerSecond angularVelocity, bool dryRun)
		{
			Log.Debug("Gearbox Power Request: torque: {0}, angularVelocity: {1}", torque, angularVelocity);
			VehicleStopped = DataBus.VehicleStopped;

			if (VehicleStopped) {
				ShiftTime = absTime;
			}
			IResponse retVal;
			if (DataBus.ClutchClosed(absTime)) {
				retVal = RequestGearEngaged(absTime, dt, torque, angularVelocity, dryRun);
			} else {
				retVal = RequestGearDisengaged(absTime, dt, torque, angularVelocity, dryRun);
			}

			return retVal;
		}

		protected bool VehicleStopped { get; set; }

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
		protected virtual IResponse RequestGearDisengaged(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			Log.Debug("Current Gear: Neutral");

			if (dryRun) {
				return new ResponseDryRun {
					Source = this,
					GearboxPowerRequest = outTorque * outAngularVelocity,
					DeltaDragLoad = outTorque * outAngularVelocity,
					DeltaFullLoad = outTorque * outAngularVelocity,
				};
			}

			var shiftTimeExceeded = absTime.IsSmaller(ShiftTime) &&
									ShiftTime.IsSmaller(absTime + dt, Data.TractionInterruption / 20.0);
			if (shiftTimeExceeded) {
				return new ResponseFailTimeInterval {
					Source = this,
					DeltaT = ShiftTime - absTime,
					GearboxPowerRequest = outTorque * outAngularVelocity
				};
			}

			if ((outTorque * outAngularVelocity).IsGreater(0.SI<Watt>(), Constants.SimulationSettings.EnginePowerSearchTolerance)) {
				return new ResponseOverload {
					AbsTime = absTime,
					Source = this,
					Delta = outTorque * outAngularVelocity,
					GearboxPowerRequest = outTorque * outAngularVelocity
				};
			}

			if ((outTorque * outAngularVelocity).IsSmaller(0.SI<Watt>(), Constants.SimulationSettings.EnginePowerSearchTolerance)) {
				return new ResponseUnderload {
					AbsTime = absTime,
					Source = this,
					Delta = outTorque * outAngularVelocity,
					GearboxPowerRequest = outTorque * outAngularVelocity
				};
			}

			var response = NextComponent.Request(absTime, dt, 0.SI<NewtonMeter>(), null);
			response.GearboxPowerRequest = outTorque * outAngularVelocity;

			PreviousInAngularSpeed = DataBus.EngineIdleSpeed;

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
		protected virtual IResponse RequestGearEngaged(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, bool dryRun)
		{
			// Set a Gear if no gear was set and engineSpeed is not zero
			if (Disengaged && !outAngularVelocity.IsEqual(0)) {
				Disengaged = false;
				if (VehicleStopped) {
					Gear = Strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
				} else {
					Gear = Strategy.Engage(absTime, dt, outTorque, outAngularVelocity);
				}

				Log.Debug("Gearbox engaged gear {0}", Gear);
			}

			var inEngineSpeed = outAngularVelocity * Data.Gears[Gear].Ratio;
			var inTorque = outAngularVelocity.IsEqual(0)
				? outTorque / Data.Gears[Gear].Ratio
				: Data.Gears[Gear].LossMap.GetInTorque(inEngineSpeed, outTorque);

			var inPower = inTorque * inEngineSpeed;
			var outPower = outTorque * outAngularVelocity;


			PowerLoss = inPower - outPower;

			if (!inEngineSpeed.IsEqual(0)) {
				PowerLossInertia = Formulas.InertiaPower(inEngineSpeed, PreviousInAngularSpeed, Data.Inertia, dt);
				inTorque += PowerLossInertia / inEngineSpeed;
			} else {
				PowerLossInertia = 0.SI<Watt>();
			}

			if (dryRun) {
				if ((DataBus.DrivingBehavior == DrivingBehavior.Braking || DataBus.DrivingBehavior == DrivingBehavior.Coasting) &&
					inEngineSpeed < DataBus.EngineIdleSpeed &&
					DataBus.VehicleSpeed < Constants.SimulationSettings.VehicleStopClutchDisengageSpeed) {
					Disengaged = true;
					ShiftTime = absTime + dt;
					Strategy.Disengage(absTime, dt, outTorque, outAngularVelocity);
					Log.Debug("EngineSpeed is below IdleSpeed, Gearbox disengage!");
					return new ResponseEngineSpeedTooLow {
						AbsTime = absTime,
						Source = this,
						GearboxPowerRequest = outTorque * outAngularVelocity
					};
				}
				var dryRunResponse = NextComponent.Request(absTime, dt, inTorque, inEngineSpeed, true);
				dryRunResponse.GearboxPowerRequest = outTorque * outAngularVelocity;
				return dryRunResponse;
			}

			var shiftAllowed = !inEngineSpeed.IsEqual(0) && !DataBus.VehicleSpeed.IsEqual(0);

			if (shiftAllowed) {
				var shiftRequired = Strategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inEngineSpeed,
					Gear, ShiftTime + Data.TractionInterruption);

				if (shiftRequired) {
					ShiftTime = absTime + Data.TractionInterruption;

					Log.Debug("Gearbox is shifting. absTime: {0}, dt: {1}, shiftTime: {2}, out: ({3}, {4}), in: ({5}, {6})", absTime,
						dt, ShiftTime, outTorque, outAngularVelocity, inTorque, inEngineSpeed);

					Disengaged = true;
					Strategy.Disengage(absTime, dt, outTorque, outAngularVelocity);
					Log.Info("Gearbox disengaged");

					return new ResponseGearShift {
						Source = this,
						SimulationInterval = Data.TractionInterruption,
						GearboxPowerRequest = outTorque * outAngularVelocity
					};
				}
			}

			var response = NextComponent.Request(absTime, dt, inTorque, inEngineSpeed);
			response.GearboxPowerRequest = outTorque * outAngularVelocity;

			PreviousInAngularSpeed = inEngineSpeed;
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
			container[ModalResultField.Gear] = Disengaged || VehicleStopped ? 0 : Gear;
			container[ModalResultField.PlossGB] = PowerLoss;
			container[ModalResultField.PaGB] = PowerLossInertia;
		}

		protected override void DoCommitSimulationStep()
		{
			if (!Disengaged) {
				if (Data.Gears[Gear].LossMap.Extrapolated) {
					// todo (MK, 2015-12-14): should we throw an interpolation error in EngineOnly Mode also?
					Log.Warn("Gear {0} LossMap data was extrapolated: range for loss map is not sufficient.", Gear);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						// todo (MK, 2016-01-07): add operating point and loss values for easier debugging
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient.", Gear);
					}
				}
			}

			PowerLoss = null;
			PowerLossInertia = null;
		}

		#endregion
	}
}