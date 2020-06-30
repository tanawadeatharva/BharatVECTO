using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Strategies
{
	public class TestPowertrain
	{
		public SimplePowertrainContainer Container;
		public Gearbox Gearbox;
		public SimpleHybridController HybridController;
		public Battery Battery;
		public Clutch Clutch;
		public IBrakes Brakes;

		public IDriverInfo Driver;
		public IDrivingCycleInfo DrivingCycle;

		public StopStartCombustionEngine CombustionEngine;

		public TestPowertrain(SimplePowertrainContainer container, IDataBus realContainer)
		{
			Container = container;
			Gearbox = Container.GearboxCtl as Gearbox;
			HybridController = Container.HybridController as SimpleHybridController;
			Battery = Container.BatteryInfo as Battery;
			Clutch = Container.ClutchInfo as Clutch;
			CombustionEngine = Container.EngineInfo as StopStartCombustionEngine;
			if (Gearbox == null) {
				throw new VectoException("Unknown gearboxtype in TestContainer: {0}", Container.GearboxCtl.GetType().FullName);
			}

			if (HybridController == null) {
				throw new VectoException("Unknown HybridController in TestContainer: {0}", Container.HybridController.GetType().FullName);
			}

			Driver = new MockDriver(container, realContainer);
			DrivingCycle = new MockDrivingCycle(container, realContainer);
			Brakes = new MockBrakes(container);
		}
	}

	public class MockBrakes : VectoSimulationComponent, IBrakes
	{
		public MockBrakes(IVehicleContainer container) : base(container)
		{
			BrakePower = 0.SI<Watt>();
		}

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion

		#region Implementation of IBrakes

		public Watt BrakePower { get; set; }

		#endregion
	}

	public class MockDrivingCycle : VectoSimulationComponent, IDrivingCycleInfo
	{
		private IDataBus realContainer;

		public MockDrivingCycle(VehicleContainer container, IDataBus rcontainer) : base(container)
		{
			realContainer = rcontainer;
		}

		#region Implementation of IDrivingCycleInfo

		public CycleData CycleData
		{
			get { return realContainer.DrivingCycleInfo.CycleData; }
		}

		public bool PTOActive
		{
			get { return realContainer.DrivingCycleInfo.PTOActive; }
		}

		public DrivingCycleData.DrivingCycleEntry CycleLookAhead(Meter distance)
		{
			return realContainer.DrivingCycleInfo.CycleLookAhead(distance);
		}

		public Meter Altitude
		{
			get { return realContainer.DrivingCycleInfo.Altitude; }
		}

		public Radian RoadGradient
		{
			get { return realContainer.DrivingCycleInfo.RoadGradient; }
		}

		public MeterPerSecond TargetSpeed
		{
			get { return realContainer.DrivingCycleInfo.TargetSpeed; }
		}

		public Second StopTime
		{
			get { return realContainer.DrivingCycleInfo.StopTime; }
		}

		public Meter CycleStartDistance
		{
			get { return realContainer?.DrivingCycleInfo?.CycleStartDistance ?? 0.SI<Meter>(); }
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Meter lookaheadDistance)
		{
			return realContainer.DrivingCycleInfo.LookAhead(lookaheadDistance);
		}

		public IReadOnlyList<DrivingCycleData.DrivingCycleEntry> LookAhead(Second time)
		{
			return realContainer.DrivingCycleInfo.LookAhead(time);
		}

		public SpeedChangeEntry LastTargetspeedChange
		{
			get { return realContainer.DrivingCycleInfo.LastTargetspeedChange; }
		}

		public void FinishSimulation()
		{
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion
	}

	public class MockDriver : VectoSimulationComponent, IDriverInfo
	{
		private IDataBus realContainer;

		public MockDriver(VehicleContainer container, IDataBus rcontainer) : base(container)
		{
			realContainer = rcontainer;
		}

		#region Implementation of IDriverInfo

		public DrivingBehavior DriverBehavior
		{
			get { return realContainer?.DriverInfo?.DriverBehavior ?? DrivingBehavior.Accelerating; }
		}

		public DrivingAction DrivingAction
		{
			get { return realContainer?.DriverInfo?.DrivingAction ?? DrivingAction.Accelerate; }
		}

		public MeterPerSquareSecond DriverAcceleration
		{
			get { return realContainer?.DriverInfo.DriverAcceleration; }
		}

		#endregion

		#region Overrides of VectoSimulationComponent

		protected override void DoWriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			
		}

		protected override void DoCommitSimulationStep(Second time, Second simulationInterval)
		{
			
		}

		#endregion
	}

	public class HybridStrategy : LoggingObject, IHybridControlStrategy
	{
		public static readonly Second MIN_ICE_ON_TIME = 3.SI<Second>();

		public class StrategyState
		{
			public HybridStrategyResponse Response { get; set; }
			public List<HybridResultEntry> Evaluations;
			public HybridResultEntry Solution { get; set; }

			public bool GearboxEngaged;

			public Second ICEStartTStmp { get; set; }
		}

		private VectoRunData ModelData;
		private IDataBus DataBus;

		protected Dictionary<PowertrainPosition, NewtonMeter> ElectricMotorsOff;

		private bool ElectricMotorCanPropellDuringTractionInterruption;

		//private Second lastShiftTime;
		

		private TestPowertrain TestPoweretrain;

		protected readonly VelocityRollingLookup VelocityDropData;


		protected StrategyState CurrentState = new StrategyState();
		protected StrategyState PreviousState = new StrategyState();
		private double IceRampUpCosts;
		private double IceIdlingCosts;

		public HybridStrategy(VectoRunData runData, IVehicleContainer vehicleContainer)
		{
			DataBus = vehicleContainer;
			ModelData = runData;
			if (ModelData.ElectricMachinesData.Select(x => x.Item1).Distinct().Count() > 1) {
				throw new VectoException("More than one electric motors are currently not supported");
			}

			ElectricMotorsOff = ModelData.ElectricMachinesData
										.Select(x => new KeyValuePair<PowertrainPosition, NewtonMeter>(x.Item1, null))
										.ToDictionary(x => x.Key, x => x.Value);
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			ElectricMotorCanPropellDuringTractionInterruption =
				emPos == PowertrainPosition.HybridP4 || emPos == PowertrainPosition.HybridP3;

			var engineRampUpEnergy = Formulas.InertiaPower(ModelData.EngineData.IdleSpeed, 0.RPMtoRad(), ModelData.EngineData.Inertia, ModelData.EngineData.EngineStartTime) * ModelData.EngineData.EngineStartTime;
			var engineDragEnergy = VectoMath.Abs(ModelData.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(ModelData.EngineData.IdleSpeed)) *
									ModelData.EngineData.IdleSpeed / 2.0 * ModelData.EngineData.EngineStartTime;

			IceRampUpCosts = (engineRampUpEnergy + engineDragEnergy).Value() / DeclarationData.AlternaterEfficiency / DeclarationData.AlternaterEfficiency;

			IceIdlingCosts = ModelData.EngineData.Fuels.Sum(
				x => (x.ConsumptionMap.GetFuelConsumptionValue(0.SI<NewtonMeter>(), ModelData.EngineData.IdleSpeed)
					* x.FuelData.LowerHeatingValueVecto * MIN_ICE_ON_TIME).Value());

			// create testcontainer
			var modData = new ModalDataContainer(runData, null, new[] { FuelData.Diesel }, null, false);
			var builder = new PowertrainBuilder(modData);
			var testContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimpleHybridPowertrain(runData, testContainer);

			TestPoweretrain = new TestPowertrain(testContainer, DataBus);
			
			// register pre-processors
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			VelocityDropData = new VelocityRollingLookup();
			vehicleContainer.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption, testContainer, -grad, grad, 2));

			var shiftStrategyParameters = runData.GearshiftParameters;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}
			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		
		public virtual IHybridController Controller { protected get; set; }
		

		public virtual HybridStrategyResponse Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Halt) {
				return HandleHaltAction(absTime, dt, outTorque, outAngularVelocity, dryRun);
			}

			
			var eval = new List<HybridResultEntry>();
			
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate || DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) {
				if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
					//if (dryRun) {
					//	eval.Add(CurrentState.Solution);
					//} else {
						eval = FindSolution(absTime, dt, outTorque, outAngularVelocity, dryRun);
					//}
				} else {
					eval.Add(new HybridResultEntry {
						U = double.NaN,
						Response = null,
						Setting = new HybridStrategyResponse() {
							GearboxInNeutral = false,
							CombustionEngineOn = true,
							MechanicalAssistPower = ElectricMotorsOff
						},
						FuelCosts = double.NaN,
						Gear = 0,
					});
				}
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Roll || DataBus.DriverInfo.DrivingAction == DrivingAction.Coast) {
				eval.Add(new HybridResultEntry {
					U = double.NaN,
					Response = null,
					Setting = new HybridStrategyResponse() {
						GearboxInNeutral = false,
						CombustionEngineOn = DataBus.EngineInfo.EngineOn,
						MechanicalAssistPower = ElectricMotorsOff
					},
					FuelCosts = double.NaN,
					ICEOff = !DataBus.EngineInfo.EngineOn,
					Gear = 0 ,
				});
			}
			if (DataBus.DriverInfo.DrivingAction == DrivingAction.Brake && (eval.Count  == 0 || eval.All(x => double.IsNaN(x.Score)))) {
				eval.Add(MaxRecuperationSetting(absTime, dt, outTorque, outAngularVelocity));
			}
			//if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && eval.Count > 0 && eval.All(x => double.IsNaN(x.Score))) {
			//	if (eval.All(x => x.Response.Engine.TotalTorqueDemand.IsGreater(x.Response.Engine.DynamicFullLoadTorque))) {
			//		// overload for all situations -> use max EM power
					
			//	}

			//}

			var best = eval.Where(x => !double.IsNaN(x.Score)).OrderBy(x => x.Score).FirstOrDefault(); // ?? eval.FirstOrDefault();
			if (best == null) {
				best = eval.FirstOrDefault();
				var allOverload = eval.All(
					x => (x.IgnoreReason & HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh) != 0);
				var allUnderload = eval.All(
					x => (x.IgnoreReason & HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow) != 0);
				if (DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate && allOverload) {
					if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
						// overload, EM can support - use solution with max EM power
						best = eval.MinBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value ?? 0.SI<NewtonMeter>()));
					}
				}
				if ((DataBus.DriverInfo.DrivingAction == DrivingAction.Accelerate || DataBus.DriverInfo.DrivingAction == DrivingAction.Brake) && allUnderload) {
					if (ElectricMotorCanPropellDuringTractionInterruption || DataBus.GearboxInfo.GearEngaged(absTime)) {
						best = eval.MaxBy(x => x.Setting.MechanicalAssistPower.Sum(e => e.Value ?? 0.SI<NewtonMeter>()));
					}
				}
			}

			//Tuple<bool, uint> gs = null;
			//if (best?.Response != null && !best.ICEOff) {
			//	gs = HandleGearshift(absTime, best);
			//}
			var currentGear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear.Gear;

			var retVal = new HybridStrategyResponse() {
				CombustionEngineOn = !best.ICEOff,
				GearboxInNeutral = best.Setting.GearboxInNeutral,
				MechanicalAssistPower = best.Setting.MechanicalAssistPower,
				ShiftRequired = best.Gear != 0 && best.Gear != currentGear, //  gs?.Item1 ?? false,
				NextGear = best.Gear // gs?.Item2 ?? 0,
			};
			if (!DataBus.EngineInfo.EngineOn && !best.ICEOff && retVal.ShiftRequired) {
				CurrentState.ICEStartTStmp = absTime + dt;
			}
			CurrentState.Response = dryRun ? null : retVal;
			if (!dryRun) {
				CurrentState.Solution = best;
				CurrentState.Evaluations = eval;
				CurrentState.GearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime);
				if (!DataBus.EngineCtl.CombustionEngineOn && !best.ICEOff && !retVal.ShiftRequired) {
					CurrentState.ICEStartTStmp = absTime;
				}
			}
			return retVal;
		}

		

		private HybridResultEntry MaxRecuperationSetting(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var first = new HybridStrategyResponse() {
				CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
				GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
				MechanicalAssistPower = ElectricMotorsOff
			};
			var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, DataBus.GearboxInfo.Gear, first);

			var allowICEOff = PreviousState.ICEStartTStmp == null ||
							PreviousState.ICEStartTStmp.IsSmaller(absTime + MIN_ICE_ON_TIME);


			var emPos = ModelData.ElectricMachinesData.First().Item1;
			var emTorque = !ElectricMotorCanPropellDuringTractionInterruption && (firstResponse.Gearbox.Gear == 0 || !DataBus.GearboxInfo.GearEngaged(absTime)) ? null : firstResponse.ElectricMotor.MaxRecuperationTorque;
			return TryConfiguration(absTime, dt, outTorque, outAngularVelocity, DataBus.GearboxInfo.Gear, emPos, emTorque, double.NaN, allowICEOff);
		}

		private List<HybridResultEntry> FindSolution(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			//var first = new HybridStrategyResponse() {
			//	CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
			//	GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
			//	MechanicalAssistPower = ElectricMotorsOff
			//};
			//var firstResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, DataBus.GearboxInfo.Gear, first);

			var allowICEOff = PreviousState.ICEStartTStmp == null ||
							PreviousState.ICEStartTStmp.IsSmaller(absTime + MIN_ICE_ON_TIME);

			//var gearboxEngaged = DataBus.GearboxInfo.GearEngaged(absTime);
			var emPos = ModelData.ElectricMachinesData.First().Item1;
			//var emTqReq = (firstResponse.ElectricMotor.PowerRequest + firstResponse.ElectricMotor.InertiaPowerDemand)  / firstResponse.ElectricMotor.AngularVelocity;
			var responses = new List<HybridResultEntry>();

			//var entry = new HybridResultEntry() {
			//	U = double.NaN,
			//	Response = firstResponse,
			//	Setting = first,
			//	Gear = DataBus.GearboxInfo.Gear,
			//	//Score = CalcualteCosts(firstResponse, dt)
			//};
			//CalcualteCosts(firstResponse, dt, entry, allowICEOff);
			//responses.Add(entry);

			//if (firstResponse.Gearbox.Gear == 0 && !ElectricMotorCanPropellDuringTractionInterruption) {
			//	return responses;
			//}

			var minimumShiftTimePassed = (DataBus.GearboxInfo.LastShift + ModelData.GearboxData.ShiftTime).IsSmallerOrEqual(absTime);
			var gearRangeUpshift = ModelData.GearshiftParameters.AllowedGearRangeUp;
			var gearRangeDownshift = ModelData.GearshiftParameters.AllowedGearRangeDown;
			if (dryRun || !minimumShiftTimePassed || (absTime - DataBus.GearboxInfo.LastUpshift).IsSmaller(ModelData.GearboxData.DownshiftAfterUpshiftDelay)) {
				gearRangeDownshift = 0;
			}
			if (dryRun || !minimumShiftTimePassed || (absTime - DataBus.GearboxInfo.LastDownshift).IsSmaller(ModelData.GearboxData.UpshiftAfterDownshiftDelay)) {
				gearRangeUpshift = 0;
			}

			var gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear.Gear; // DataBus.GearboxInfo.Gear;
			var numGears = ModelData.GearboxData.Gears.Count;
			for (uint nextGear = (uint)Math.Max(1, gear - gearRangeDownshift);
				nextGear <= Math.Min(numGears, gear + gearRangeUpshift);
				nextGear++) {


				var emOffSetting = new HybridStrategyResponse() {
					CombustionEngineOn = true, //DataBus.EngineCtl.CombustionEngineOn,
					GearboxInNeutral = false, // DataBus.GearboxInfo.GearEngaged(absTime),
					MechanicalAssistPower = ElectricMotorsOff
				};
				var emOffResponse = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, emOffSetting);
				
				if (emOffResponse == null) {
					continue;
				}
				var emTqReq = (emOffResponse.ElectricMotor.PowerRequest + emOffResponse.ElectricMotor.InertiaPowerDemand) / emOffResponse.ElectricMotor.AngularVelocity;

				var entry = new HybridResultEntry() {
					U = double.NaN,
					Response = emOffResponse,
					Setting = emOffSetting,
					Gear = nextGear
				};
				CalcualteCosts(emOffResponse, dt, entry, allowICEOff);
				responses.Add(entry);
				IterateEMTorque(absTime, dt, outTorque, outAngularVelocity, nextGear, allowICEOff, emOffResponse, emTqReq, emPos, responses);
			}

			return responses;
		}

		private void IterateEMTorque(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint nextGear, bool allowIceOff, ResponseDryRun firstResponse, NewtonMeter emTqReq, PowertrainPosition emPos, List<HybridResultEntry> responses)
		{
			const double stepSize = 0.1;

			// iterate over 'EM provides torque'. allow EM to provide more torque in order to overcome ICE inertia
			var maxEmTorque = firstResponse.ElectricMotor.MaxDriveTorque ?? 0.SI<NewtonMeter>();
			var maxU = allowIceOff
				? -1.0
				: Math.Min((maxEmTorque) / emTqReq, -1.0);
			if (firstResponse.ElectricMotor.MaxDriveTorque != null && (!ElectricMotorCanPropellDuringTractionInterruption && firstResponse.Gearbox.Gear != 0)) {
				for (var u = 0.0; u >= maxU; u -= stepSize * (u < -4 ? 10 : (u < -2 ? 5 : 1))) {
					var emTorque = emTqReq.Abs() * u;
					if (!emTorque.IsBetween(
						0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, emTorque, u, allowIceOff);
					responses.Add(tmp);
				}

				// make sure the max drive point is also covered.
				var emTorqueM = emTqReq * maxU;
				if (!responses.Any(x => x.Gear == nextGear && x.U.IsEqual(maxU)) && emTorqueM.IsBetween(
					0.SI<NewtonMeter>(), firstResponse.ElectricMotor.MaxDriveTorque)) {
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, emTorqueM, maxU, allowIceOff);
					responses.Add(tmp);
				}
				if (maxEmTorque.IsSmaller(0) && emTqReq.IsGreater(-maxEmTorque)) {
					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, maxEmTorque, maxEmTorque / emTqReq, allowIceOff);
					responses.Add(tmp);
				}
			}
			
			// iterate over 'EM recuperates' up to max available recuperation potential
			if (firstResponse.ElectricMotor.MaxRecuperationTorque != null && (!ElectricMotorCanPropellDuringTractionInterruption && firstResponse.Gearbox.Gear != 0)) {
				for (var u = stepSize; u <= 1.0; u += stepSize) {
					var emTorque = firstResponse.ElectricMotor.MaxRecuperationTorque * u;
					if (!(emTorque).IsBetween(
						firstResponse.ElectricMotor.MaxRecuperationTorque, 0.SI<NewtonMeter>())) {
						continue;
					}

					var tmp = TryConfiguration(absTime, dt, outTorque, outAngularVelocity, nextGear, emPos, emTorque, u, allowIceOff);
					responses.Add(tmp);
				}
			}
		}

		private HybridResultEntry TryConfiguration(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint nextGear, PowertrainPosition emPos, NewtonMeter emTorque, double u, bool allowIceOff)
		{
			var cfg = new HybridStrategyResponse() {
				CombustionEngineOn = true,
				GearboxInNeutral = false,
				MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>() {
					{ emPos, emTorque }
				}
			};
			var resp = RequestDryRun(absTime, dt, outTorque, outAngularVelocity, nextGear, cfg);
			
			var tmp = new HybridResultEntry {
				U = u,
				Setting = cfg,
				Response = resp,
				Gear = nextGear,
			};
			CalcualteCosts(resp, dt, tmp, allowIceOff);
			return tmp;
		}

		private ResponseDryRun RequestDryRun(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint nextGear, HybridStrategyResponse cfg)
		{
			TestPoweretrain.Gearbox.Gear = PreviousState.GearboxEngaged ? DataBus.GearboxInfo.Gear : Controller.ShiftStrategy.NextGear.Gear;
			TestPoweretrain.Container.VehiclePort.Initialize(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			TestPoweretrain.HybridController.ApplyStrategySettings(cfg);
			TestPoweretrain.HybridController.Initialize(Controller.PreviousState.OutTorque, Controller.PreviousState.OutAngularVelocity);
			TestPoweretrain.Clutch.Initialize(DataBus.ClutchInfo.ClutchLosses);
			TestPoweretrain.Battery.Initialize(DataBus.BatteryInfo.StateOfCharge);

			//TestPoweretrain.CombustionEngine.PreviousState.EngineOn = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
			//TestPoweretrain.CombustionEngine.PreviousState.EnginePower = (DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
			//TestPoweretrain.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
			//TestPoweretrain.CombustionEngine.PreviousState.EngineSpeed = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
			//TestPoweretrain.CombustionEngine.PreviousState.EngineTorque = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
			//TestPoweretrain.CombustionEngine.PreviousState.EngineTorqueOut = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
			//TestPoweretrain.CombustionEngine.PreviousState.DynamicFullLoadTorque = (DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;
			
			if (nextGear != DataBus.GearboxInfo.Gear) {
				if (ModelData.GearboxData.Gears[nextGear].Ratio > ModelData.GearshiftParameters.RatioEarlyUpshiftFC) {
					return null;
				}

				if (ModelData.GearboxData.Gears[nextGear].Ratio >= ModelData.GearshiftParameters.RatioEarlyDownshiftFC) {
					return null;
				}

				var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
				if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
					return null;
				}


				var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
				var vehicleSpeedPostShift = DataBus.VehicleInfo.VehicleSpeed - vDrop * ModelData.GearshiftParameters.VelocityDropFactor;
				TestPoweretrain.Gearbox.Gear = nextGear;
				TestPoweretrain.Container.VehiclePort.Initialize(
					vehicleSpeedPostShift, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			}

			TestPoweretrain.CombustionEngine.PreviousState.EngineOn = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineOn;
			TestPoweretrain.CombustionEngine.PreviousState.EnginePower = (DataBus.EngineInfo as CombustionEngine).PreviousState.EnginePower;
			TestPoweretrain.CombustionEngine.PreviousState.dt = (DataBus.EngineInfo as CombustionEngine).PreviousState.dt;
			TestPoweretrain.CombustionEngine.PreviousState.EngineSpeed = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineSpeed;
			TestPoweretrain.CombustionEngine.PreviousState.EngineTorque = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorque;
			TestPoweretrain.CombustionEngine.PreviousState.EngineTorqueOut = (DataBus.EngineInfo as CombustionEngine).PreviousState.EngineTorqueOut;
			TestPoweretrain.CombustionEngine.PreviousState.DynamicFullLoadTorque = (DataBus.EngineInfo as CombustionEngine).PreviousState.DynamicFullLoadTorque;

			// AMT EffShift: estimatedVelocityPostShift < MIN_SPEED => no shift

			// AMT EffShift: engine torqueOut close to dragCurve => no shift

			// AMT EffShift: Ratio EarlyUpshift / Ratio RealyDownshift

			// initialize with new vehicle speed
			// set gear

			var retVal = TestPoweretrain.HybridController.NextComponent.Request(absTime, dt, outTorque, outAngularVelocity, true);

			//if (nextGear != DataBus.GearboxInfo.Gear) {
			//	if (retVal.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * retVal.Engine.DragTorque)) {
			//		return null;
			//	}
			//}

			//var retVal2 = Controller.RequestDryRun(absTime, dt, outTorque, outAngularVelocity, cfg);
			return retVal as ResponseDryRun;
		}

		private void CalcualteCosts(ResponseDryRun resp, Second dt, HybridResultEntry tmp, bool allowIceOff)
		{
			tmp.IgnoreReason = 0;
			if (resp == null) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.NoResponseAvailable;
				return;
			}

			//if (resp.Gearbox.Gear == 0) {
			//	tmp.FuelCosts = Math.Abs((int)tmp.Gear - DataBus.GearboxInfo.Gear);
			//	tmp.GearshiftPenalty = 1;
			//	return;
			//}
			if (!resp.Engine.TotalTorqueDemand.IsBetween(
				resp.Engine.DragTorque, resp.Engine.DynamicFullLoadTorque)) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= resp.Engine.TotalTorqueDemand.IsGreater(resp.Engine.DynamicFullLoadTorque)
					? HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh
					: HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow;
			}

			if (resp.Gearbox.Gear != 0 &&resp.Engine.EngineSpeed.IsGreaterOrEqual(
					VectoMath.Min(
						ModelData.GearboxData.Gears[resp.Gearbox.Gear].MaxSpeed,
						DataBus.EngineInfo.EngineN95hSpeed)) ) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooHigh;
			}
			if (resp.Engine.EngineSpeed.IsSmallerOrEqual(ModelData.EngineData.IdleSpeed)) {
				tmp.FuelCosts = double.NaN;
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedTooLow;
			}

			if (resp.Engine.EngineSpeed != null && resp.Gearbox.Gear != 0 && resp.Gearbox.Gear < ModelData.GearboxData.Gears.Count && ModelData.GearboxData.Gears[resp.Gearbox.Gear].ShiftPolygon.IsAboveUpshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				tmp.FuelCosts = double.NaN; // Tuple.Create(true, response.Gearbox.Gear + 1);
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
			}
			if (resp.Engine.EngineSpeed != null && resp.Gearbox.Gear > 0 && ModelData.GearboxData.Gears[resp.Gearbox.Gear].ShiftPolygon.IsBelowDownshiftCurve(resp.Engine.TorqueOutDemand, resp.Engine.EngineSpeed)) {
				//lastShiftTime = absTime;
				tmp.FuelCosts = double.NaN; // = Tuple.Create(true, response.Gearbox.Gear - 1);
				tmp.IgnoreReason |= HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift;
			}

			if (!double.IsNaN(tmp.FuelCosts)) {
				if (allowIceOff && resp.Engine.TorqueOutDemand.IsEqual(0)) {
					// no torque from ICE requested, ICE could be turned off
					tmp.FuelCosts = 0;
					tmp.ICEOff = true;
				} else {
					tmp.FuelCosts = ModelData.EngineData.Fuels.Sum(
						x => (x.ConsumptionMap.GetFuelConsumptionValue(resp.Engine.TotalTorqueDemand, resp.Engine.EngineSpeed)
							* x.FuelData.LowerHeatingValueVecto * dt).Value());
				}
			}
			tmp.BatCosts = -(resp.ElectricSystem.ConsumerPower * dt).Value();
			tmp.SoCPenalty = 1 - Math.Pow((DataBus.BatteryInfo.StateOfCharge - ModelData.BatteryData.TargetSoC) / (0.5 * (ModelData.BatteryData.MaxSOC - ModelData.BatteryData.MinSOC)), 5);

			tmp.EqualityFactor = 2.5;
			tmp.GearshiftPenalty = resp.Gearbox.Gear != 0 && resp.Gearbox.Gear != DataBus.GearboxInfo.Gear
				? ModelData.GearshiftParameters.RatingFactorCurrentGear
				: 1;

			if (!DataBus.EngineCtl.CombustionEngineOn && !tmp.ICEOff) {
				tmp.ICEStartPenalty1 = IceRampUpCosts;
				tmp.ICEStartPenalty2 = IceIdlingCosts;
			} else {
				tmp.ICEStartPenalty1 = 0;
				tmp.ICEStartPenalty2 = 0;
			}
		}

		private Tuple<bool, uint> HandleGearshift(Second absTime, HybridResultEntry config)
		{
			var minimumShiftTimePassed = (DataBus.GearboxInfo.LastShift + ModelData.GearboxData.ShiftTime).IsSmallerOrEqual(absTime);
			
			// search for EM operating point already selected another gear
			if (minimumShiftTimePassed && config.Gear != DataBus.GearboxInfo.Gear) {
				return Tuple.Create(true, config.Gear);
			}
			var response = config.Response;
			var retVal = Tuple.Create(false, response.Gearbox.Gear);

			//var gear = DataBus.GearboxInfo.Gear;
			//var _nextGear = gear;
			//var outSpeed = (response.Gearbox.GearboxInputSpeed ?? response.Engine.EngineSpeed ) / ModelData.GearboxData.Gears[gear].Ratio;
			// emergency shift to not stall the engine ------------------------
			//if (gear == 1 && SpeedTooLowForEngine(_nextGear, outSpeed)) {
			//	retVal = Tuple.Create(true, 0u);
			//}
			//_nextGear = gear;
			//while (_nextGear > 1 && SpeedTooLowForEngine(_nextGear, outSpeed)) {
			//	_nextGear--;
			//}
			//while (_nextGear < ModelData.GearboxData.Gears.Count &&
			//		SpeedTooHighForEngine(_nextGear, outSpeed)) {
			//	_nextGear++;
			//}
			//if (_nextGear != gear) {
			//	return Tuple.Create(true, _nextGear);
			//}

			//// normal shift when all requirements are fullfilled ------------------
			//if (!minimumShiftTimePassed) {
			//	return retVal;
			//}

			//if (response.Engine.EngineSpeed != null && gear < ModelData.GearboxData.Gears.Count  && ModelData.GearboxData.Gears[gear].ShiftPolygon.IsAboveUpshiftCurve(response.Engine.TorqueOutDemand, response.Engine.EngineSpeed)) {
			//	//lastShiftTime = absTime;
			//	retVal = Tuple.Create(true, response.Gearbox.Gear + 1);
			//}
			//if (response.Engine.EngineSpeed != null && gear > 0  && ModelData.GearboxData.Gears[gear].ShiftPolygon.IsBelowDownshiftCurve(response.Engine.TorqueOutDemand, response.Engine.EngineSpeed)) {
			//	//lastShiftTime = absTime;
			//	retVal = Tuple.Create(true, response.Gearbox.Gear - 1);
			//}
			return retVal;
		}


		private bool SpeedTooLowForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * ModelData.GearboxData.Gears[gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);
		}

		private bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * ModelData.GearboxData.Gears[gear].Ratio).IsGreaterOrEqual(VectoMath.Min(ModelData.GearboxData.Gears[gear].MaxSpeed,
																								DataBus.EngineInfo.EngineN95hSpeed));
		}

		private HybridStrategyResponse HandleHaltAction(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, bool dryRun)
		{
			var tmp = new HybridStrategyResponse() {
				CombustionEngineOn = false,
				GearboxInNeutral = false,
				MechanicalAssistPower = ElectricMotorsOff
			};
			CurrentState.Response = dryRun ? null : tmp;
			return tmp;
		}

		public virtual HybridStrategyResponse Initialize(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var retVal = new HybridStrategyResponse()
				{ MechanicalAssistPower = new Dictionary<PowertrainPosition, NewtonMeter>() };

			foreach (var em in ModelData.ElectricMachinesData) {
				retVal.MechanicalAssistPower[em.Item1] = null;
			}

			return retVal;
		}

		public virtual void CommitSimulationStep(Second time, Second simulationInterval)
		{
			PreviousState = CurrentState;
			CurrentState = new StrategyState();
			CurrentState.ICEStartTStmp = PreviousState.ICEStartTStmp;
		}

		public void WriteModalResults(Second time, Second simulationInterval, IModalDataContainer container)
		{
			container[ModalResultField.HybridStrategyScore] = CurrentState.Solution?.Score ?? 0;
			container[ModalResultField.HybridStrategySolution] = CurrentState.Solution?.U ?? -100;

			if (CurrentState.Evaluations != null) {
				container.SetDataValue(
					"HybridStrategyEvaluation",
					string.Join(
						" | ", CurrentState.Evaluations.Select(
							x => {
								var foo = string.Join(" ",  x.Setting.MechanicalAssistPower.Select(e => $"{e.Key.GetName()} - {e.Value}"));
								var ice = "====";
								if (x.Response != null) {
									ice =
										$"{x.Response.Engine.TorqueOutDemand}, {x.Response.Engine.TotalTorqueDemand}, {x.Response.Engine.DynamicFullLoadTorque}";
								}
								return
									$"{x.U:F2}: {x.Score:F2}; G{x.Gear}; ({x.FuelCosts:F2} + {x.EqualityFactor:F2} * ({x.BatCosts:F2} + {x.ICEStartPenalty1:F2}) * {x.SoCPenalty:F2} + {x.ICEStartPenalty2:F2}) / {x.GearshiftPenalty:F2} = {x.Score:F2} ({foo} ICE: {ice}); {x.IgnoreReason.HumanReadable()}";
							})
						)
					);
			}
		}

		[DebuggerDisplay("{U}: {Score} - G{Gear}")]
		public class HybridResultEntry
		{
			public double U { get; set; }

			public HybridStrategyResponse Setting { get; set; }

			public ResponseDryRun Response { get; set; }

			public double Score { get { return (FuelCosts + EqualityFactor * (BatCosts + ICEStartPenalty1) * SoCPenalty + ICEStartPenalty2) / GearshiftPenalty; } }

			public double FuelCosts { get; set; }

			public double BatCosts { get; set; }

			public double SoCPenalty { get; set; }

			public double EqualityFactor { get; set; }

			public double GearshiftPenalty { get; set; }

			public double ICEStartPenalty1 { get; set; }

			public double ICEStartPenalty2 { get; set; }

			public uint Gear { get; set; }

			public bool ICEOff { get; set; }

			public HybridConfigurationIgnoreReason IgnoreReason { get; set; }
		}

		[Flags]
		public enum HybridConfigurationIgnoreReason
		{
			NotEvaluated = 0,
			EngineSpeedTooLow = 1<<2,
			EngineSpeedTooHigh = 1<<3,
			EngineTorqueDemandTooHigh = 1<<4,
			EngineTorqueDemandTooLow = 1<<5,
			EngineSpeedAboveUpshift = 1<<6,
			EngineSpeedBelowDownshift = 1<<7,
			NoResponseAvailable = 1<<8,
			Evaluated = 1<<9,
		}



	}

	public static class HybridConfigurationIgnoreReasonHelper
	{
		public static string HumanReadable(this HybridStrategy.HybridConfigurationIgnoreReason x)
		{
			var retVal = new List<string>();
			foreach (var entry in EnumHelper.GetValues<HybridStrategy.HybridConfigurationIgnoreReason>()) {

				var tmp = x & entry;
				switch (tmp) {
					
					case HybridStrategy.HybridConfigurationIgnoreReason.Evaluated: break;
					case HybridStrategy.HybridConfigurationIgnoreReason.NotEvaluated: retVal.Add("not evaluated");
						break;
					case HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedTooLow: retVal.Add("engine speed too low");
						break;
					case HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedTooHigh: retVal.Add("engine speed too high");
						break;
					case HybridStrategy.HybridConfigurationIgnoreReason.EngineTorqueDemandTooHigh:
						retVal.Add("engine torque demand too high");
						break;
					case HybridStrategy.HybridConfigurationIgnoreReason.EngineTorqueDemandTooLow:
						retVal.Add("engine torque demand too low");
						break;
					case HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift: retVal.Add("engine speed above upshift"); break;
					case HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift:
						retVal.Add("engine speed below downshift");
						break;
					case HybridStrategy.HybridConfigurationIgnoreReason.NoResponseAvailable: return "no response available";
					default: throw new ArgumentOutOfRangeException(nameof(x), x, null);
				}
			}

			return string.Join("/", retVal);
		}

		public static bool InvalidEngineSpeed(this HybridStrategy.HybridConfigurationIgnoreReason x)
		{
			return x == HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedTooLow ||
					x == HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedTooHigh ||
					x == HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedBelowDownshift ||
					x == HybridStrategy.HybridConfigurationIgnoreReason.EngineSpeedAboveUpshift;
		}
	}
}