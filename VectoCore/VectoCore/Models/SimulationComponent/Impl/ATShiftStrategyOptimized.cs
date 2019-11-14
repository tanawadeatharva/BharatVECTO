using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ninject.Extensions.NamedScope;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{


	public class ATShiftStrategyOptimized : ATShiftStrategy
	{
		private FuelConsumptionMap fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;
		private SimplePowertrainContainer TestContainer;
		private ATGearbox TestContainerGbx;

		protected readonly List<GearshiftPosition> GearList;

		private Kilogram vehicleMass;
		private Kilogram MinMass;
		private Kilogram MaxMass;

		private List<SchmittTrigger> LoadStageSteps = new List<SchmittTrigger>();
		private ShiftLineSet UpshiftLineTCLocked = new ShiftLineSet();

		public const double DownhillSlope = -5;
		public const double UphillSlope = 5;

		public new static string Name
		{
			get { return "AT - EffShift"; }
		}

		public ATShiftStrategyOptimized(VectoRunData runData, IDataBus dataBus) : base(runData, dataBus)
		{
			if (runData.EngineData == null) {
				return;
			}
			fcMap = runData.EngineData.ConsumptionMap;
			fld = runData.EngineData.FullLoadCurves;
			vehicleMass = runData.VehicleData.TotalVehicleMass;
			shiftStrategyParameters = runData.GearshiftParameters;

			MinMass = runData.VehicleData.MinimumVehicleMass;
			MaxMass = runData.VehicleData.MaximumVehicleMass;

			var LoadStageThresoldsUp = "19.7;36.34;53.01;69.68;86.35";
			var LoadStageThresoldsDown = "13.7;30.34;47.01;63.68;80.35";
			foreach (var entry in LoadStageThresoldsUp.Split(';').Select(x => x.ToDouble()).Zip(LoadStageThresoldsDown.Split(';').Select(x => x.ToDouble()), Tuple.Create)) {
				LoadStageSteps.Add(new SchmittTrigger(entry));
			}

			var slopeDh = VectoMath.InclinationToAngle(DownhillSlope / 100.0);
			var slopeLevel = VectoMath.InclinationToAngle(0);
			var slopeUh = VectoMath.InclinationToAngle(UphillSlope / 100.0);

			var sl1 = new ShiftLines();
			sl1.entriesAMin.Add(Tuple.Create(slopeDh, 650.RPMtoRad()));
			sl1.entriesAMin.Add(Tuple.Create(slopeLevel, 680.RPMtoRad()));
			sl1.entriesAMin.Add(Tuple.Create(slopeUh, 725.RPMtoRad()));
			sl1.entriesAMax.Add(Tuple.Create(slopeDh, 650.RPMtoRad()));
			sl1.entriesAMax.Add(Tuple.Create(slopeLevel, 680.RPMtoRad()));
			sl1.entriesAMax.Add(Tuple.Create(slopeUh, 725.RPMtoRad()));
			UpshiftLineTCLocked.LoadStages[1] = sl1;
			UpshiftLineTCLocked.LoadStages[2] = sl1;
			UpshiftLineTCLocked.LoadStages[3] = sl1;

			var sl4 = new ShiftLines();
			sl4.entriesAMin.Add(Tuple.Create(slopeDh, 650.RPMtoRad()));
			sl4.entriesAMin.Add(Tuple.Create(slopeLevel, 680.RPMtoRad()));
			sl4.entriesAMin.Add(Tuple.Create(slopeUh, 725.RPMtoRad()));
			sl4.entriesAMax.Add(Tuple.Create(slopeDh, 670.RPMtoRad()));
			sl4.entriesAMax.Add(Tuple.Create(slopeLevel, 700.RPMtoRad()));
			sl4.entriesAMax.Add(Tuple.Create(slopeUh, 745.RPMtoRad()));
			UpshiftLineTCLocked.LoadStages[4] = sl4;

			var sl5 = new ShiftLines();
			sl5.entriesAMin.Add(Tuple.Create(slopeDh, 660.RPMtoRad()));
			sl5.entriesAMin.Add(Tuple.Create(slopeLevel, 690.RPMtoRad()));
			sl5.entriesAMin.Add(Tuple.Create(slopeUh, 735.RPMtoRad()));
			sl5.entriesAMax.Add(Tuple.Create(slopeDh, 680.RPMtoRad()));
			sl5.entriesAMax.Add(Tuple.Create(slopeLevel, 710.RPMtoRad()));
			sl5.entriesAMax.Add(Tuple.Create(slopeUh, 755.RPMtoRad()));
			UpshiftLineTCLocked.LoadStages[5] = sl5;

			var sl6 = new ShiftLines();
			sl6.entriesAMin.Add(Tuple.Create(slopeDh, 670.RPMtoRad()));
			sl6.entriesAMin.Add(Tuple.Create(slopeLevel, 700.RPMtoRad()));
			sl6.entriesAMin.Add(Tuple.Create(slopeUh, 745.RPMtoRad()));
			sl6.entriesAMax.Add(Tuple.Create(slopeDh, 690.RPMtoRad()));
			sl6.entriesAMax.Add(Tuple.Create(slopeLevel, 720.RPMtoRad()));
			sl6.entriesAMax.Add(Tuple.Create(slopeUh, 765.RPMtoRad()));
			UpshiftLineTCLocked.LoadStages[6] = sl6;

			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			var modData = new ModalDataContainer(runData, null, null, false);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(runData);
			
			builder.BuildSimplePowertrain(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as ATGearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}
			// initialize vehicle so that vehicleStopped of the testcontainer is false (required for test-runs)
			TestContainer.VehiclePort.Initialize(10.KMPHtoMeterPerSecond(), 0.SI<Radian>());

			if (runData.Cycle.CycleType == CycleType.MeasuredSpeed) {
				try {
					TestContainer.GetCycleOutPort().Initialize();
					TestContainer.GetCycleOutPort().Request(0.SI<Second>(), 1.SI<Second>());
				} catch (Exception ) { }
			}

			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}


			GearList = new List<GearshiftPosition>();
			foreach (var gear in runData.GearboxData.Gears) {
				if (runData.GearboxData.Type.AutomaticTransmission()) {
					if (gear.Value.HasTorqueConverter) {
						GearList.Add(new GearshiftPosition(gear.Key, false));
					}
					if (gear.Value.HasLockedGear) {
						GearList.Add(new GearshiftPosition(gear.Key, true));
					}
				} else {
					GearList.Add(new GearshiftPosition(gear.Key));
				}
			}
		}

		#region Overrides of ATShiftStrategy

		protected override bool? CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque, PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime, IResponse response)
		{
			var current = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var currentIdx = GearList.IndexOf(current);
			var next = GearList[currentIdx + 1];

			if (current.Gear == next.Gear && current.TorqueConverterLocked != next.TorqueConverterLocked) {
				return CheckUpshiftFromTC(absTime, dt, outTorque, outAngularVelocity, origInTorque, origInAngularVelocity, currentGear, lastShiftTime, response);
			}

			return CheckEarlyUpshiftFromLocked(
				absTime, dt, outTorque, outAngularVelocity, origInTorque, origInAngularVelocity, currentGear, lastShiftTime, response);
		}

		private bool? CheckUpshiftFromTC(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime, IResponse response)
		{
			var accPower = EstimateAccelerrationPower(outAngularVelocity, outTorque);

			var _accMin = (accPower / DataBus.VehicleSpeed / (MaxMass + DataBus.ReducedMassWheels)).Cast<MeterPerSquareSecond>();
			var _accMax = (accPower / DataBus.VehicleSpeed / (MinMass + DataBus.ReducedMassWheels)).Cast<MeterPerSquareSecond>();

			var engineLoadPercent = inTorque / response.EngineDynamicFullLoadTorque;
			var _loadStage = GetLoadStage(engineLoadPercent);

			var shiftSpeed = UpshiftLineTCLocked.LookupShiftSpeed(
				_loadStage, DataBus.RoadGradient, DataBus.DriverAcceleration, _accMin, _accMax);
			var shiftSpeedGbxOut = shiftSpeed / ModelData.Gears[currentGear].Ratio;
			if (outAngularVelocity > shiftSpeedGbxOut) {
				Upshift(absTime, currentGear);
				return true;
			}

			return false;
		}

		protected Watt EstimateAccelerrationPower(PerSecond gbxOutSpeed, NewtonMeter gbxOutTorque)
		{
			var vehicleSpeed = DataBus.VehicleSpeed;
			var avgSlope =
			((DataBus.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
			DataBus.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleSpeed;
			var rollResistanceLoss = DataBus.RollingResistance(avgSlope) * DataBus.VehicleSpeed;

			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.SlopeResistance(avgSlope) * DataBus.VehicleSpeed;
			var axleLoss = DataBus.AxlegearLoss();

			return gbxOutSpeed * gbxOutTorque - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;
		}

		private int GetLoadStage(double engineLoadPercent)
		{
			var sum = 1;
			foreach (var entry in LoadStageSteps) {
				sum += entry.GetOutput(engineLoadPercent * 100);
			}

			return sum;
		}

		protected bool? CheckEarlyUpshiftFromLocked(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque, PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime, IResponse response1)
		{
			if (outAngularVelocity.IsEqual(0)) {
				return null;
			}

			var minFcGear = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var minFc = double.MaxValue;
			KilogramPerSecond fcCurrent = null;

			var current = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var currentIdx = GearList.IndexOf(current);

			var vDrop = DataBus.DriverAcceleration * shiftStrategyParameters.ATLookAheadTime;
			var vehicleSpeedPostShift = (DataBus.VehicleSpeed + vDrop * shiftStrategyParameters.VelocityDropFactor).LimitTo(0.KMPHtoMeterPerSecond(), DataBus.CycleData.LeftSample.VehicleTargetSpeed);

			for (var i = 1; i <=  shiftStrategyParameters.AllowedGearRangeFC; i++) {

				if (currentIdx + i >= GearList.Count) {
					// no further gear
					continue;
				}

				var next = GearList[currentIdx + i];
				if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
					// upshift from C to L with skipping gear not allowed
					continue;
				}

				if (!(ModelData.Gears[next.Gear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				var inAngularVelocity = ModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var totalTransmissionRatio = inAngularVelocity / (DataBus.VehicleSpeed + DataBus.DriverAcceleration * dt);
				var estimatedEngineSpeed = (vehicleSpeedPostShift * totalTransmissionRatio).Cast<PerSecond>();
				if (estimatedEngineSpeed.IsSmaller(shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				var pNextGearMax = DataBus.EngineStationaryFullPower(estimatedEngineSpeed);
				
				var outAngularVelocityEst = (outAngularVelocity* vehicleSpeedPostShift / (DataBus.VehicleSpeed + DataBus.DriverAcceleration * dt)).Cast<PerSecond>() ;
				var outTorqueEst = outTorque * outAngularVelocity / outAngularVelocityEst;

				var response = RequestDryRunWithGear(absTime, dt, outTorqueEst, outAngularVelocityEst, next);
				//var response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration, next);

				if (!response.EnginePowerRequest.IsSmaller(pNextGearMax)) {
					continue;
				}
				
				var inTorque = response.EnginePowerRequest / inAngularVelocity;

				

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (ModelData.Gears[next.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inAngularVelocity)) {
					continue;
				}

				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (reserve < ModelData.TorqueReserve) {
					var accelerationFactor = outAngularVelocity * ModelData.Gears[currentGear].Ratio < fld[0].NTq98hSpeed
						? 1.0
						: VectoMath.Interpolate(
							fld[0].NTq98hSpeed, fld[0].NP98hSpeed, 1.0, shiftStrategyParameters.AccelerationFactor,
							outAngularVelocity * ModelData.Gears[currentGear].Ratio);
					if (accelerationFactor.IsEqual(1, 1e-9)) {
						continue;
					}
					var accelerationTorque = vehicleMass * DataBus.DriverAcceleration * DataBus.VehicleSpeed / outAngularVelocity;
					var reducedTorque = outTorque - accelerationTorque * (1 - accelerationFactor);

					response = RequestDryRunWithGear(absTime, dt, reducedTorque, outAngularVelocity, next);
					//response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration * accelerationFactor, next);
					fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
					reserve = 1 - response.EnginePowerRequest / fullLoadPower;
					if (reserve < ModelData.TorqueReserve) {
						continue;
					}
				}

				if (fcCurrent == null) {
					//var responseCurrent = RequestDryRunWithGear(
					//	absTime, dt, vehicleSpeedForGearRating, DataBus.DriverAcceleration, current);
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
					var tqCurrent = responseCurrent.EngineTorqueDemand.LimitTo(
						fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
						fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed));
					var fcCurrentRes = fcMap.GetFuelConsumption(tqCurrent, responseCurrent.EngineSpeed, true);
					if (fcCurrentRes.Extrapolated) {
						Log.Warn("EffShift Strategy: Extrapolation of fuel consumption for current gear!n: {1}, Tq: {2}", responseCurrent.EngineSpeed, tqCurrent);
					}
					fcCurrent = fcCurrentRes.Value;
				}
				var tqNext = response.EngineTorqueDemand.LimitTo(
					fld[next.Gear].DragLoadStationaryTorque(response.EngineSpeed),
					fld[next.Gear].FullLoadStationaryTorque(response.EngineSpeed));
				var fcNextRes = fcMap.GetFuelConsumption(tqNext, response.EngineSpeed, true);
				if (fcNextRes.Extrapolated) {
					Log.Warn("EffShift Strategy: Extrapolation of fuel consumption for gear {0}! n: {1}, Tq: {2}", next, response.EngineSpeed, tqNext);
				}
				var fcNext = fcNextRes.Value;

				if (reserve < ModelData.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFc = fcNext.Value();
				minFcGear = next;
			}

			if (!minFcGear.Equals(current)) {
				ShiftGear(absTime, current, minFcGear);
				return true;
			}

			return null;	
		}

		

		protected override bool? CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque, PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime, IResponse response1)
		{
			var minFcGear = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked); 
			var minFc = double.MaxValue;
			KilogramPerSecond fcCurrent = null;

			var current = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var currentIdx = GearList.IndexOf(current);

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {

				if (currentIdx - i < 0) {
					// no further gear
					continue;
				}

				var next = GearList[currentIdx - i];
				if (!next.TorqueConverterLocked.Value) {
					continue;
				}
				if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
					// downshift from C to L with skipping gear not allowed
					continue;
				}

				if (!(ModelData.Gears[next.Gear].Ratio < shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, next);

				var inAngularVelocity = ModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var inTorque = response.EnginePowerRequest / inAngularVelocity;

				if (!IsAboveUpShiftCurve(next.Gear, inTorque, inAngularVelocity, next.TorqueConverterLocked.Value)) {

					if (fcCurrent == null) {
						var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
						fcCurrent = fcMap.GetFuelConsumption(
							responseCurrent.EngineTorqueDemand.LimitTo(
								fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
								fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed))
							, responseCurrent.EngineSpeed, true).Value;
					}
					var fcNext = fcMap.GetFuelConsumption(
						response.EngineTorqueDemand.LimitTo(
							fld[next.Gear].DragLoadStationaryTorque(response.EngineSpeed),
							fld[next.Gear].FullLoadStationaryTorque(response.EngineSpeed)), response.EngineSpeed, true).Value;

					if (fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) && fcNext.IsSmaller(minFc)) {
						minFcGear = next;
						minFc = fcNext.Value();

					}
				}
			}

			if (!current.Equals(minFcGear)) {
				ShiftGear(absTime, current, minFcGear);
				return true;
			}

			return null;
		}

		protected virtual void ShiftGear(Second absTime, GearshiftPosition currentGear, GearshiftPosition nextGear)
		{
			if (currentGear.TorqueConverterLocked != nextGear.TorqueConverterLocked && currentGear.Gear != nextGear.Gear) {
				throw new VectoException("skipping gear from converter to locked not allowed! {0} -> {1}", currentGear.Name, nextGear.Name);
			}
			_nextGear.SetState(absTime, disengaged: false, gear: nextGear.Gear, tcLocked: nextGear.TorqueConverterLocked.Value);
		}

		#endregion

		protected ResponseDryRun RequestDryRunWithGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition gear)
		{
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = gear.Gear;
			TestContainerGbx.TorqueConverterLocked = gear.TorqueConverterLocked.Value;

			TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			return response;
		}

		protected ResponseDryRun RequestDryRunWithGear(Second absTime, Second dt, MeterPerSecond vehicleSpeed, MeterPerSquareSecond acceleration, GearshiftPosition gear)
		{
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = gear.Gear;
			TestContainerGbx.TorqueConverterLocked = gear.TorqueConverterLocked.Value;

			TestContainer.VehiclePort.Initialize(vehicleSpeed, DataBus.RoadGradient);
			var response = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), dt, acceleration, DataBus.RoadGradient, true);
			return response;
		}

		#region Overrides of ATShiftStrategy

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius)
		{
			var shiftLine = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
				Math.Max(i, 2), engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);

			var upshift = new List<ShiftPolygon.ShiftPolygonEntry>();

			if (i < gearboxGears.Count - 1) {

				var maxDragTorque = engineDataFullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = engineDataFullLoadCurve.MaxTorque * 1.1;

				var speed = engineData.FullLoadCurves[0].NP98hSpeed / gearboxGears[i].Ratio * gearboxGears[i + 1].Ratio;


				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, speed));
				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxTorque, speed));
			}
			return new ShiftPolygon(shiftLine.Downshift.ToList(), upshift);
		}

		#endregion
	}

	[DebuggerDisplay("{Name}")]
	public class GearshiftPosition
	{
		public uint Gear { get; }
		public bool? TorqueConverterLocked { get; }

		public GearshiftPosition(uint gear, bool? torqueConverterLocked = null)
		{
			Gear = gear;
			TorqueConverterLocked = torqueConverterLocked;
		}

		public string Name
		{
			get {
				return string.Format(
					"{0}{1}", Gear,
					Gear == 0 ? "" : (TorqueConverterLocked.HasValue ? (TorqueConverterLocked.Value ? "L" : "C") : ""));
			}
		}

		public override bool Equals(object x)
		{
			var other = x as GearshiftPosition;
			if (other == null)
				return false;

			return other.Gear == Gear && other.TorqueConverterLocked == TorqueConverterLocked;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

	}
}
