using System.CodeDom;
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
	public class ATGearbox : AbstractGearbox<ATGearbox.ATGearboxState>, IGearbox, ITnOutPort, ITnInPort,
		IClutchInfo
	{
		protected internal bool Disengaged = true;

		protected internal readonly IShiftStrategy _strategy;

		protected TorqueConverter TorqueConverter;

		public Second LastShift { get; private set; }

		public ATGearbox(IVehicleContainer container, GearboxData gearboxModelData, IShiftStrategy strategy)
			: base(container, gearboxModelData)
		{
			_strategy = strategy;
			_strategy.Gearbox = this;
			LastShift = -double.MaxValue.SI<Second>();
			TorqueConverter = new TorqueConverter(this, _strategy, container, gearboxModelData.TorqueConverterData);
		}


		private ICombustionEngineIdleController _idleController;

		public bool TorqueConverterLocked { get; protected internal set; }

		public ICombustionEngineIdleController IdleController
		{
			get { return _idleController; }
			set
			{
				_idleController = value;
				_idleController.RequestPort = NextComponent;
			}
		}

		public new void Connect(ITnOutPort other)
		{
			NextComponent = other;
			TorqueConverter.NextComponent = NextComponent;
		}

		public override IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (Disengaged) {
				Gear = _strategy.InitGear(0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, outTorque,
					outAngularVelocity);
			}
			var inAngularVelocity = 0.SI<PerSecond>();
			var inTorque = 0.SI<NewtonMeter>();
			var effectiveRatio = ModelData.Gears[Gear].Ratio;
			var effectiveLossMap = ModelData.Gears[Gear].LossMap;
			if (!TorqueConverterLocked) {
				effectiveRatio = ModelData.Gears[Gear].TorqueConverterRatio;
				effectiveLossMap = ModelData.Gears[Gear].TorqueConverterGearLossMap;
			}
			if (!DataBus.VehicleStopped) {
				inAngularVelocity = outAngularVelocity * effectiveRatio;
				var torqueLossResult = effectiveLossMap.GetTorqueLoss(outAngularVelocity, outTorque);
				CurrentState.TorqueLossResult = torqueLossResult;
				inTorque = outTorque / effectiveRatio + torqueLossResult.Value;
			}
			if (Disengaged) {
				return NextComponent.Initialize(0.SI<NewtonMeter>(), null);
			}

			if (!TorqueConverterLocked && !ModelData.Gears[Gear].HasTorqueConverter) {
				throw new VectoSimulationException("Torque converter requested by strategy for gear without torque converter!");
			}
			var response = TorqueConverterLocked
				? NextComponent.Initialize(inTorque, inAngularVelocity)
				: TorqueConverter.Initialize(inTorque, inAngularVelocity);

			PreviousState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			PreviousState.Gear = Gear;
			PreviousState.TorqueConverterLocked = TorqueConverterLocked;
			return response;
		}

		internal ResponseDryRun Initialize(uint gear, bool torqueConverterLocked, NewtonMeter outTorque,
			PerSecond outAngularVelocity)
		{
			var effectiveRatio = torqueConverterLocked ? ModelData.Gears[gear].Ratio : ModelData.Gears[gear].TorqueConverterRatio;

			var inAngularVelocity = outAngularVelocity * effectiveRatio;
			var torqueLossResult = torqueConverterLocked
				? ModelData.Gears[gear].LossMap.GetTorqueLoss(outAngularVelocity, outTorque)
				: ModelData.Gears[Gear].TorqueConverterGearLossMap.GetTorqueLoss(outAngularVelocity, outTorque);
			var inTorque = outTorque / effectiveRatio + torqueLossResult.Value;

			IResponse response;
			if (torqueConverterLocked) {
				response = NextComponent.Initialize(inTorque, inAngularVelocity);
			} else {
				if (!ModelData.Gears[gear].HasTorqueConverter) {
					throw new VectoSimulationException("Torque converter requested by strategy for gear without torque converter!");
				}
				response = TorqueConverter.Initialize(inTorque, inAngularVelocity);
			}

			response.Switch().
				Case<ResponseSuccess>().
				Case<ResponseUnderload>().
				Case<ResponseOverload>().
				Default(r => { throw new UnexpectedResponseException("AT-Gearbox.Initialize", r); });

			return new ResponseDryRun() {
				Source = this,
				EngineSpeed = response.EngineSpeed,
				EnginePowerRequest = response.EnginePowerRequest,
				GearboxPowerRequest = outTorque * outAngularVelocity,
			};
		}


		public override IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			IterationStatistics.Increment(this, "Requests");

			Log.Debug("AT-Gearbox Power Request: torque: {0}, angularVelocity: {1}", outTorque, outAngularVelocity);

			if (DataBus.VehicleStopped && outAngularVelocity > 0) {
				Gear = _strategy.InitGear(absTime, dt, outTorque, outAngularVelocity);
			}

			IResponse retVal;
			var count = 0;
			var loop = false;
			do {
				if (Disengaged || (DataBus.DriverBehavior == DrivingBehavior.Halted)) {
					// only when vehicle is halted or close before halting
					retVal = RequestDisengaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
				} else {
					Disengaged = false;
					retVal = RequestEngaged(absTime, dt, outTorque, outAngularVelocity, dryRun);
				}
				retVal.Switch()
					.Case<ResponseGearShift>(r => {
						loop = true;
						Gear = _strategy.Engage(absTime, dt, outTorque, outAngularVelocity);
					});
			} while (loop && ++count < 2);

			retVal.GearboxPowerRequest = outTorque * (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;
			return retVal;
		}

		private IResponse RequestEngaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
			var effectiveRatio = ModelData.Gears[Gear].Ratio;
			var effectiveLossMap = ModelData.Gears[Gear].LossMap;
			if (!TorqueConverterLocked) {
				effectiveRatio = ModelData.Gears[Gear].TorqueConverterRatio;
				effectiveLossMap = ModelData.Gears[Gear].TorqueConverterGearLossMap;
			}

			var avgOutAngularVelocity = (PreviousState.OutAngularVelocity + outAngularVelocity) / 2.0;

			var inTorqueLossResult = effectiveLossMap.GetTorqueLoss(avgOutAngularVelocity, outTorque);

			CurrentState.TorqueLossResult = inTorqueLossResult;
			var inTorque = outTorque / effectiveRatio + inTorqueLossResult.Value;

			if (!TorqueConverterLocked && !ModelData.Gears[Gear].HasTorqueConverter) {
				throw new VectoSimulationException("Torque converter requested by strategy for gear without torque converter!");
			}
			var inAngularVelocity = outAngularVelocity * effectiveRatio;

			CurrentState.InertiaTorqueLossOut = !inAngularVelocity.IsEqual(0)
				? Formulas.InertiaPower(outAngularVelocity, PreviousState.OutAngularVelocity, ModelData.Inertia, dt) /
				avgOutAngularVelocity
				: 0.SI<NewtonMeter>();
			inTorque += CurrentState.InertiaTorqueLossOut / effectiveRatio;

			CurrentState.SetState(inTorque, inAngularVelocity, outTorque, outAngularVelocity);
			CurrentState.Gear = Gear;
			CurrentState.TorqueConverterLocked = TorqueConverterLocked;
			CurrentState.TransmissionTorqueLoss = inTorque - outTorque / effectiveRatio;

			if (!TorqueConverterLocked) {
				return TorqueConverter.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);
			}
			if (_strategy.ShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, Gear, LastShift)) {
				return new ResponseGearShift() {
					Source = this
				};
			}
			return NextComponent.Request(absTime, dt, inTorque, inAngularVelocity, dryRun);
		}


		private IResponse RequestDisengaged(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun)
		{
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

			Log.Debug("Invoking IdleController...");

			var retval = IdleController.Request(absTime, dt, 0.SI<NewtonMeter>(), null);
			retval.ClutchPowerRequest = 0.SI<Watt>();
			CurrentState.SetState(0.SI<NewtonMeter>(), retval.EngineSpeed, outTorque, outAngularVelocity);
			CurrentState.Gear = 0;
			return retval;
		}


		protected override void DoWriteModalResults(IModalDataContainer container)
		{
			var avgInAngularSpeed = (PreviousState.OutAngularVelocity +
									CurrentState.OutAngularVelocity) / 2.0 * ModelData.Gears[Gear].Ratio;

			container[ModalResultField.Gear] = Disengaged || DataBus.VehicleStopped ? 0 : Gear;
			container[ModalResultField.TC_Locked] = TorqueConverterLocked;
			//container[ModalResultField.TCv] =; 
			//container[ModalResultField.TCmu] =;
			//container[ModalResultField.TC_n_Out] =;
			//container[ModalResultField.TC_M_Out] =;
			container[ModalResultField.P_gbx_loss] = CurrentState.TransmissionTorqueLoss * avgInAngularSpeed;
			container[ModalResultField.P_gbx_inertia] = CurrentState.InertiaTorqueLossOut * avgInAngularSpeed;
			container[ModalResultField.P_gbx_in] = CurrentState.InTorque * avgInAngularSpeed;
		}

		protected override void DoCommitSimulationStep()
		{
			if (!Disengaged) {
				if (CurrentState.TorqueLossResult != null && CurrentState.TorqueLossResult.Extrapolated) {
					Log.Warn(
						"Gear {0} LossMap data was extrapolated: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
						Gear, CurrentState.OutAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.OutTorque,
						ModelData.Gears[Gear].Ratio);
					if (DataBus.ExecutionMode == ExecutionMode.Declaration) {
						throw new VectoException(
							"Gear {0} LossMap data was extrapolated in Declaration Mode: range for loss map is not sufficient: n:{1}, torque:{2}, ratio:{3}",
							Gear, CurrentState.InAngularVelocity.ConvertTo().Rounds.Per.Minute, CurrentState.InTorque,
							ModelData.Gears[Gear].Ratio);
					}
				}
			}
			if (DataBus.VehicleStopped) {
				Disengaged = true;
			}
			AdvanceState();
		}


		public bool ClutchClosed(Second absTime)
		{
			return true;
		}


		public class ATGearboxState : GearboxState
		{
			public bool TorqueConverterLocked;

			//public PerSecond TorqueConverterTorqueOut;
			//public PerSecond TorqueConverterAngularSpeedOut;

			//public double TorqueConverterSpeedRatio;
			//public double TorqueConverterTorqueRatio;
		}
	}
}