using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class HybridController :
		StatefulProviderComponent<HybridController.HybridControllerState, ITnOutPort, ITnInPort, ITnOutPort>,
		IHybridController, ITnInPort, ITnOutPort
	{
		protected readonly Dictionary<PowertrainPosition, ElectricMotorController> _electricMotorCtl;
		protected readonly HybridCtlShiftStrategy _shiftStrategy;
		protected readonly IHybridControlStrategy _hybridStrategy;

		private Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>> _electricMotorTorque =
			new Dictionary<PowertrainPosition, Tuple<PerSecond, NewtonMeter>>();

		private HybridStrategyResponse CurrentStrategySettings;

		protected DebugData DebugData = new DebugData();


		public HybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es) : base(container)
		{
			_electricMotorCtl = new Dictionary<PowertrainPosition, ElectricMotorController>();
			_shiftStrategy = container.RunData.GearboxData.Type.AutomaticTransmission()
				? new HybridCtlATShiftStrategy(this, container)
				: new HybridCtlShiftStrategy(this, container);
			_hybridStrategy = strategy;
			strategy.Controller = this;

			ElectricSystem = es;
		}

		public IHybridControlStrategy Strategy
		{
			get { return _hybridStrategy; }
		}

		public IElectricSystem ElectricSystem { get; }

		public virtual void AddElectricMotor(PowertrainPosition pos, ElectricMotorData motorData)
		{
			if (_electricMotorCtl.ContainsKey(pos)) {
				throw new VectoException("Electric motor already registered as position {0}", pos);
			}

			_electricMotorCtl[pos] = new ElectricMotorController(this, motorData);
		}

		//public ResponseDryRun RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, HybridStrategyResponse strategySettings)
		//{
		//	ApplyStrategySettings(strategySettings);
		//	var retVal = NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, true);

		//	return retVal as ResponseDryRun;
		//}

		private void ApplyStrategySettings(HybridStrategyResponse strategySettings)
		{
			Gearbox.SwitchToNeutral = strategySettings.GearboxInNeutral;
			Engine.CombustionEngineOn = strategySettings.CombustionEngineOn;
			_electricMotorTorque = strategySettings.MechanicalAssistPower;
			//if (strategySettings.ShiftRequired) {
			//	_shiftStrategy.SetNextGear(strategySettings.NextGear);
			//}
		}

		SimpleComponentState IHybridController.PreviousState
		{
			get { return PreviousState; }
		}

		public virtual IElectricMotorControl ElectricMotorControl(PowertrainPosition pos)
		{
			return _electricMotorCtl[pos];
		}

		public virtual IShiftStrategy ShiftStrategy
		{
			get { return _shiftStrategy; }
		}

		public GearInfo SelectedGear { get; protected set; }

		public bool GearboxEngaged
		{
			get { return CurrentStrategySettings.GearboxEngaged; }
		}

		public PerSecond ElectricMotorSpeed(PowertrainPosition pos)
		{
			return CurrentStrategySettings.MechanicalAssistPower[pos].Item1;
		}

		public PerSecond ICESpeed
		{
			get { return CurrentStrategySettings.EvaluatedSolution.Response?.Engine.EngineSpeed; }
		}


		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			var retry = false;
			var retryCount = 0;
			IResponse retVal;
			do {
				if (retryCount > 10) {
					throw new VectoException("HybridStrategy: retry count exceeded! {0}", DebugData);
				}

				retry = false;
				var strategyResponse = Strategy.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				if (strategyResponse is HybridStrategyLimitedResponse) {
					var ovl = strategyResponse as HybridStrategyLimitedResponse;
					if (dryRun) {
						return new ResponseDryRun(this) {
							DeltaDragLoad = ovl.Delta,
							DeltaFullLoad = ovl.Delta,
							DeltaEngineSpeed = ovl.DeltaEngineSpeed
						};
					}

					return new ResponseOverload(this) {
						Delta = ovl.Delta
					};
				}

				var strategySettings = strategyResponse as HybridStrategyResponse;
				ApplyStrategySettings(strategySettings);
				if (!dryRun) {
					CurrentState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
					CurrentState.StrategyResponse = strategySettings;
				}

				//SelectedGear = new GearInfo(strategySettings.NextGear, true);
				if (!dryRun && /*!DataBus.EngineInfo.EngineOn &&*/ strategySettings.ShiftRequired) {
					DataBus.GearboxCtl.TriggerGearshift(absTime, dt);
					_shiftStrategy.SetNextGear(strategySettings.NextGear);
					SelectedGear = new GearInfo(strategySettings.NextGear, true);
					return new ResponseGearShift(this);
				}

				if (!dryRun /*&& DataBus.VehicleInfo.VehicleStopped*/) {
					SelectedGear = new GearInfo(strategySettings.NextGear, true);
				}

				CurrentStrategySettings = strategySettings;
				retVal = NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
				DebugData.Add(new {
					DrivingAction = DataBus.DriverInfo.DrivingAction, StrategySettings = strategySettings,
					Response = retVal, DryRun = dryRun
				});

				if (!dryRun && strategySettings.CombustionEngineOn && retVal is ResponseSuccess &&
					retVal.Engine.EngineSpeed.IsSmaller(Strategy.MinICESpeed)) {
					Strategy.AllowEmergencyShift = true;
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}

				if (retVal is ResponseDifferentGearEngaged) {
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}

				if (retVal is ResponseInvalidOperatingPoint) {
					retryCount++;
					retry = true;
					Strategy.OperatingpointChangedDuringRequest(absTime, dt, outTorque, outAngularVelocity, dryRun,
						retVal);
					continue;
				}

				retVal.HybridController.StrategySettings = strategySettings;
				if (!(retVal is ResponseSuccess) && strategySettings.EvaluatedSolution.Gear != 0 &&
					retVal.Gearbox.Gear != strategySettings.EvaluatedSolution.Gear && retryCount < 3) {
					retryCount++;
					retry = true;
				}
			} while (retry);

			var modifiedResponse = Strategy.AmendResponse(retVal, absTime, dt, outTorque, outAngularVelocity, dryRun);

			return modifiedResponse;
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			PreviousState.SetState(outTorque, outAngularVelocity, outTorque, outAngularVelocity);
			var strategyResponse = Strategy.Initialize(outTorque, outAngularVelocity);
			PreviousState.StrategyResponse = strategyResponse as HybridStrategyResponse;
			_electricMotorTorque = PreviousState.StrategyResponse.MechanicalAssistPower;
			var retVal = NextComponent.Initialize(outTorque, outAngularVelocity);
			SelectedGear = new GearInfo(DataBus.GearboxInfo.Gear, DataBus.GearboxInfo.TCLocked);
			return retVal;
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			base.DoCommitSimulationStep(time, simulationInterval);
			Strategy.CommitSimulationStep(time, simulationInterval);
			DebugData = new DebugData();
		}

		protected override void DoWriteModalResults(
			Second time, Second simulationInterval,
			IModalDataContainer container)
		{
			Strategy.WriteModalResults(time, simulationInterval, container);
		}

		private NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt,
			NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, bool dryRun)
		{
			return _electricMotorTorque[pos]?.Item2;

			//return CurrentState.StrategyResponse.MechanicalAssistPower[pos];
		}

		public uint NextGear
		{
			get { return CurrentState.StrategyResponse.NextGear; }
		}

		public bool ShiftRequired
		{
			get { return CurrentState.StrategyResponse.ShiftRequired; }
		}

		public IHybridControlledGearbox Gearbox { protected get; set; }
		public ICombustionEngine Engine { protected get; set; }

		///=======================================================================================
		public class HybridControllerState : SimpleComponentState
		{
			public HybridStrategyResponse StrategyResponse;
		}

		///=======================================================================================
		public class ElectricMotorController : IElectricMotorControl
		{
			protected HybridController _controller;
			protected ElectricMotorData ElectricMotorData;

			public ElectricMotorController(HybridController hybridController, ElectricMotorData motorData)
			{
				_controller = hybridController;
				ElectricMotorData = motorData;
			}

			public NewtonMeter MechanicalAssistPower(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity,
				NewtonMeter maxDriveTorque, NewtonMeter maxRecuperationTorque, PowertrainPosition position, bool dryRun)
			{
				return _controller.MechanicalAssistPower(position, absTime, dt, outTorque, prevOutAngularVelocity,
					currOutAngularVelocity, dryRun);
			}
		}


		///=======================================================================================
		
		public class HybridCtlShiftStrategy : ShiftStrategy
		{
			protected HybridController _controller;


			protected readonly uint MaxStartGear;
			protected uint _nextGear { get; set; }

			public HybridCtlShiftStrategy(HybridController hybridController, IVehicleContainer container) : base(
				container)
			{
				_controller = hybridController;

				var runData = container.RunData;
				if (runData == null || runData.EngineData == null) {
					return;
				}

				var transmissionRatio = runData.AxleGearData.AxleGear.Ratio *
										(runData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
										runData.VehicleData.DynamicTyreRadius;
				var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
					Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;
				foreach (var gearData in GearboxModelData.Gears.Reverse()) {
					if (GearshiftParams.StartSpeed * transmissionRatio * gearData.Value.Ratio <= minEngineSpeed)
						continue;
					MaxStartGear = gearData.Key;
					break;
				}
			}

			public override ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i,
				EngineFullLoadCurve engineDataFullLoadCurve,
				IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
				Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
			{
				return DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
					i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
			}

			protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity,
				NewtonMeter inTorque,
				PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IResponse response)
			{
				if (_controller.ShiftRequired) {
					_nextGear = _controller.NextGear;
				}

				return _controller.ShiftRequired;
			}

			public override uint InitGear(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity)
			{
				if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
					return InitStartGear(absTime, outTorque, outAngularVelocity);
				}

				for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {
					var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

					var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear].Ratio;
					var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

					// if in shift curve and torque reserve is provided: return the current gear
					if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) &&
						!IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
						reserve >= GearshiftParams.StartTorqueReserve) {
						if ((inAngularSpeed - DataBus.EngineInfo.EngineIdleSpeed) /
							(DataBus.EngineInfo.EngineRatedSpeed - DataBus.EngineInfo.EngineIdleSpeed) <
							Constants.SimulationSettings.ClutchClosingSpeedNorm && gear > 1) {
							gear--;
						}

						_nextGear = gear;
						return gear;
					}

					// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
					if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && gear < GearboxModelData.Gears.Count) {
						_nextGear = gear;
						return gear + 1;
					}
				}

				// fallback: return first gear
				_nextGear = 1;
				return 1;
			}

			protected uint InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				if (!DataBus.EngineCtl.CombustionEngineOn) {
					return _nextGear;
				}

				for (var gear = MaxStartGear; gear > 1; gear--) {
					var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear].Ratio;

					var ratedSpeed = DataBus.EngineInfo.EngineRatedSpeed;
					if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
						continue;
					}

					var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

					var fullLoadPower =
						response.Engine.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

					if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineIdleSpeed &&
						reserve >= GearshiftParams.StartTorqueReserve) {
						_nextGear = gear;
						return gear;
					}
				}

				_nextGear = 1;
				return 1;
			}


			private bool SpeedTooLowForEngine(uint gear, PerSecond outAngularSpeed)
			{
				return (outAngularSpeed * GearboxModelData.Gears[gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);
			}

			private bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
			{
				return
					(outAngularSpeed * GearboxModelData.Gears[gear].Ratio).IsGreaterOrEqual(VectoMath.Min(
						GearboxModelData.Gears[gear].MaxSpeed,
						DataBus.EngineInfo.EngineN95hSpeed));
			}

			public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				if (DataBus.EngineCtl.CombustionEngineOn) {
					while (_nextGear > 1 && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
						_nextGear--;
					}

					while (_nextGear < GearboxModelData.Gears.Count && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
						_nextGear++;
					}
				}

				return _nextGear;
			}

			public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outAngularVelocity)
			{
				if (!_controller.ShiftRequired && DataBus.DriverInfo.DrivingAction != DrivingAction.Halt) {
					// gearbox disengaged on its own! set next gear!
					var gear = _nextGear;
					while (gear > 1 && SpeedTooLowForEngine(gear, outAngularVelocity)) {
						gear--;
					}

					while (gear < GearboxModelData.Gears.Count && SpeedTooHighForEngine(gear, outAngularVelocity)) {
						gear++;
					}

					_nextGear = gear;
				}
			}

			public override IGearbox Gearbox
			{
				get { return _gearbox; }
				set
				{
					var myGearbox = value as Gearbox;
					if (myGearbox == null) {
						throw new VectoException("This shift strategy can't handle gearbox of type {0}",
							value.GetType());
					}

					_gearbox = myGearbox;
				}
			}

			public override GearInfo NextGear
			{
				get { return new GearInfo(_nextGear, false); }
			}

			public void SetNextGear(uint nextGear)
			{
				_nextGear = nextGear;
			}
		}

		public void RepeatDrivingAction(Second absTime)
		{
			Strategy.RepeatDrivingAction(absTime);
		}


		///=======================================================================================

		public class HybridCtlATShiftStrategy : HybridCtlShiftStrategy
		{
			protected new ATGearbox _gearbox;

			public HybridCtlATShiftStrategy(HybridController hybridController, IVehicleContainer container) : base(
				hybridController, container) { }

			public override IGearbox Gearbox
			{
				get { return _gearbox; }
				set
				{
					var myGearbox = value as ATGearbox;
					if (myGearbox == null) {
						throw new VectoException("This shift strategy can't handle gearbox of type {0}",
							value.GetType());
					}

					_gearbox = myGearbox;
				}
			}
		}

	}
}