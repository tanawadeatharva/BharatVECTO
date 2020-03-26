using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class HybridController :
		StatefulProviderComponent<HybridController.HybridControllerState, ITnOutPort, ITnInPort, ITnOutPort>,
		IHybridController, ITnInPort, ITnOutPort
	{
		protected readonly Dictionary<PowertrainPosition, ElectricMotorController> _electricMotorCtl;
		protected readonly HybridCtlShiftStrategy _shiftStrategy;
		protected readonly IHybridControlStrategy _hybridStrategy;

		

		public HybridController(IVehicleContainer container, IHybridControlStrategy strategy, IElectricSystem es,
			SwitchableClutch clutch) : base(container)
		{
			_electricMotorCtl = new Dictionary<PowertrainPosition, ElectricMotorController>();
			_shiftStrategy = new HybridCtlShiftStrategy(this, container);
			_hybridStrategy = strategy;
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

		public virtual IElectricMotorControl ElectricMotorControl(PowertrainPosition pos)
		{
			return _electricMotorCtl[pos];
		}

		public virtual IShiftStrategy ShiftStrategy
		{
			get { return _shiftStrategy; }
		}

		public IResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			bool dryRun = false)
		{
			CurrentState.StrategyResponse = Strategy.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
			Gearbox.SwitchToNeutral = CurrentState.StrategyResponse.GearboxInNeutral;
			Engine.CombustionEngineOn = CurrentState.StrategyResponse.CombustionEngineOn;

			return NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, dryRun);
		}

		public IResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			CurrentState.StrategyResponse = Strategy.Initialize(outTorque, outAngularVelocity);
			return NextComponent.Initialize(outTorque, outAngularVelocity);
		}

		protected override void DoCommitSimulationStep()
		{
			base.DoCommitSimulationStep();
			Strategy.CommitSimulationStep();
		}

		protected override void DoWriteModalResults(Second time, Second simulationInterval,
			IModalDataContainer container) { }

		private NewtonMeter MechanicalAssistPower(PowertrainPosition pos, Second absTime, Second dt,
			NewtonMeter outTorque, PerSecond prevOutAngularVelocity, PerSecond currOutAngularVelocity, bool dryRun)
		{
			return CurrentState.StrategyResponse.MechanicalAssistPower[pos];
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
		public class HybridControllerState
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
				PerSecond prevOutAngularVelocity,
				PerSecond currOutAngularVelocity, PowertrainPosition position, bool dryRun)
			{
				return _controller.MechanicalAssistPower(position, absTime, dt, outTorque, prevOutAngularVelocity,
					currOutAngularVelocity, dryRun);
			}

			public NewtonMeter MaxDriveTorque(PerSecond avgSpeed, Second dt)
			{
				var driveTorque = ElectricMotorData.FullLoadCurve.FullLoadDriveTorque(avgSpeed);
				var drivePowerElectric = ElectricMotorData.EfficiencyMap.LookupElectricPower(avgSpeed, driveTorque).ElectricalPower;
				if (drivePowerElectric >= _controller.ElectricSystem.MaxDischargePower(dt))
				{
					return driveTorque;
				}

				drivePowerElectric = _controller.ElectricSystem.MaxDischargePower(dt);
				driveTorque = ElectricMotorData.EfficiencyMap.SearchMechanicalPower(drivePowerElectric, avgSpeed).Torque;
				return driveTorque;
			}

			public NewtonMeter MaxDragTorque(PerSecond avgSpeed, Second dt)
			{
				return 0.SI<NewtonMeter>();
			}
		}


		///=======================================================================================
		public class HybridCtlShiftStrategy : ShiftStrategy
		{
			protected HybridController _controller;


			protected readonly uint MaxStartGear;
			protected uint _nextGear;

			public HybridCtlShiftStrategy(HybridController hybridController, IVehicleContainer container) : base(
				container.RunData.GearboxData, container)
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
				foreach (var gearData in ModelData.Gears.Reverse()) {
					if (ModelData.StartSpeed * transmissionRatio * gearData.Value.Ratio <= minEngineSpeed)
						continue;
					MaxStartGear = gearData.Key;
					break;
				}
			}

			public override ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i,
				EngineFullLoadCurve engineDataFullLoadCurve,
				IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio,
				Meter dynamicTyreRadius)
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
				if (DataBus.VehicleSpeed.IsEqual(0)) {
					return InitStartGear(outTorque, outAngularVelocity);
				}

				for (var gear = (uint)ModelData.Gears.Count; gear > 1; gear--) {
					var response = _gearbox.Initialize(gear, outTorque, outAngularVelocity);

					var inAngularSpeed = outAngularVelocity * ModelData.Gears[gear].Ratio;
					var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

					// if in shift curve and torque reserve is provided: return the current gear
					if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) &&
						!IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
						reserve >= ModelData.StartTorqueReserve) {
						if ((inAngularSpeed - DataBus.EngineIdleSpeed) /
							(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
							Constants.SimulationSettings.ClutchClosingSpeedNorm && gear > 1) {
							gear--;
						}

						_nextGear = gear;
						return gear;
					}

					// if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
					if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && gear < ModelData.Gears.Count) {
						_nextGear = gear;
						return gear + 1;
					}
				}

				// fallback: return first gear
				_nextGear = 1;
				return 1;
			}

			protected uint InitStartGear(NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				for (var gear = MaxStartGear; gear > 1; gear--) {
					var inAngularSpeed = outAngularVelocity * ModelData.Gears[gear].Ratio;

					var ratedSpeed = DataBus.EngineRatedSpeed;
					if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
						continue;
					}

					var response = _gearbox.Initialize(gear, outTorque, outAngularVelocity);

					var fullLoadPower =
						response.Engine.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

					if (response.Engine.EngineSpeed > DataBus.EngineIdleSpeed &&
						reserve >= ModelData.StartTorqueReserve) {
						_nextGear = gear;
						return gear;
					}
				}

				_nextGear = 1;
				return 1;
			}


			private bool SpeedTooLowForEngine(uint gear, PerSecond outAngularSpeed)
			{
				return (outAngularSpeed * ModelData.Gears[gear].Ratio).IsSmaller(DataBus.EngineIdleSpeed);
			}

			private bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
			{
				return
					(outAngularSpeed * ModelData.Gears[gear].Ratio).IsGreaterOrEqual(VectoMath.Min(
						ModelData.Gears[gear].MaxSpeed,
						DataBus.EngineN95hSpeed));
			}

			public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
			{
				while (_nextGear > 1 && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
					_nextGear--;
				}

				while (_nextGear < ModelData.Gears.Count && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
					_nextGear++;
				}

				return _nextGear;
			}

			public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque,
				PerSecond outEngineSpeed) { }

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
		}

		
	}
}