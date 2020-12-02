using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
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
	public class AMTShiftStrategyACEA : ShiftStrategy
	{
		private GearshiftPosition _nextGear;
		protected readonly MaxGradabilityLookup MaxGradability;
		protected readonly VelocityRollingLookup VelocityDropData;
		protected readonly VectoRunData PowertrainConfig;
		protected readonly ShiftStrategyParameters ShiftStrategyParameters;
		protected Dictionary<uint, PerSecond> EngineSpeedAtDriveOff;
		protected SimplePowertrainContainer TestContainer;
		protected Dictionary<Second, HistoryEntry> HistoryBuffer = new Dictionary<Second, HistoryEntry>();
		protected Dictionary<Second, AccelerationEntry> AccelerationBuffer = new Dictionary<Second, AccelerationEntry>();
		protected AverageAccelerationTorqueLookup AverageAccelerationTorqueLookup;
		protected MaxCardanTorqueLookup MaxCardanTorqueLookup;

		protected DebugData DebugData = new DebugData();
		private Dictionary<uint, GearRating> GearRatings = new Dictionary<uint, GearRating>();
		private MeterPerSquareSecond accRsv = 0.SI<MeterPerSquareSecond>();
		private MeterPerSecond demandedSpeed = 0.SI<MeterPerSecond>();
		private MeterPerSquareSecond driverAccelerationAvg;
		private Radian gradient = 0.SI<Radian>();
		private Gearbox TestContainerGbx;

		public struct HistoryEntry
		{
			public Second dt;
			public MeterPerSecond AvgSpeed;
			public Watt AvgCardanPower;
		}

		[DebuggerDisplay("dt: {dt}, acc: {Acceleration}")]
		public struct AccelerationEntry
		{
			public Second dt;
			public MeterPerSquareSecond Acceleration;

			public override string ToString()
			{
				return string.Format("dt: {0} acc: {1}", dt, Acceleration);
			}
		}

		public AMTShiftStrategyACEA(IVehicleContainer dataBus) : base(dataBus)
		{
			var data = dataBus.RunData;
			if (data.EngineData == null) {
				return;
			}

			if (data.EngineData.Fuels.Count > 1) {
				throw new VectoException("ACEA TCU only supports single-fuel engines");
			}

			PowertrainConfig = data;
			ShiftStrategyParameters = data.GearshiftParameters;
			if (ShiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			// create a dummy powertrain for pre-processing and estimatins
			var modData = new ModalDataContainer(data, null, null);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			// register pre-processors
			var maxG = data.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;

			VelocityDropData = new VelocityRollingLookup();
			dataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, data.GearboxData.TractionInterruption, TestContainer, -grad, grad, 2));

			MaxGradability = new MaxGradabilityLookup();
			dataBus.AddPreprocessor(new MaxGradabilityPreprocessor(MaxGradability, data, TestContainer));

			EngineSpeedAtDriveOff = new Dictionary<uint, PerSecond>(GearboxModelData.Gears.Count);
			dataBus.AddPreprocessor(new EngineSpeedDriveOffPreprocessor(EngineSpeedAtDriveOff, data, TestContainer));

			AverageAccelerationTorqueLookup = new AverageAccelerationTorqueLookup();
			dataBus.AddPreprocessor(
				new AverageAccelerationTorquePreprocessor(AverageAccelerationTorqueLookup, data, GetEngineSpeedLimitHighMin()));

			MaxCardanTorqueLookup = new MaxCardanTorqueLookup();
			dataBus.AddPreprocessor(new MaxCardanTorquePreprocessor(MaxCardanTorqueLookup, data, TestContainer));
		}

		private bool SpeedTooLowForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return (outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsSmaller(DataBus.EngineInfo.EngineIdleSpeed);
		}

		private bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(
					VectoMath.Min(
						GearboxModelData.Gears[gear.Gear].MaxSpeed,
						DataBus.EngineInfo.EngineN95hSpeed));
		}

		#region Overrides of BaseShiftStrategy

		public override void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			// update own history
			var velocity = DataBus.VehicleInfo.VehicleSpeed + DataBus.DriverInfo.DriverAcceleration * dt / 2.0;
			var cardanDemand = DataBus.AxlegearInfo.CurrentAxleDemand;
			var currentCardanPower = cardanDemand.Item1 * cardanDemand.Item2;

			UpdateHistoryBuffer(absTime, dt, currentCardanPower, velocity);

			var currentVelocity = DataBus.VehicleInfo.VehicleSpeed;
			accRsv = CalcAccelerationReserve(currentVelocity, absTime + dt);

			driverAccelerationAvg = GetAverageAcceleration(absTime + dt);

			gradient = CalcGradientDuringGearshift(false, dt, currentVelocity);

		}


		protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			var cardanDemand = DataBus.AxlegearInfo.CurrentAxleDemand;
			var currentCardanPower = cardanDemand.Item1 * cardanDemand.Item2;

			// no shift when vehicle stands
			if (DataBus.VehicleInfo.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			if (gear.Equals(Gears.First()) && SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				return true;
			}

			_nextGear = gear;
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}
			while (Gears.HasSuccessor(_nextGear) &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Successor(_nextGear);
			}

			if (_nextGear != gear) {
				return true;
			}

			// TEST
			var currentVelocity = DataBus.VehicleInfo.VehicleSpeed;
			accRsv = CalcAccelerationReserve(currentVelocity, absTime + dt);

			var minimumShiftTimePassed = (lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			var averageCardanPower = CalcAverageCardanPower();

			var propulsion = HistoryBuffer.Min(x => x.Value.AvgSpeed).IsEqual(0) ||
							averageCardanPower > ShiftStrategyParameters.AverageCardanPowerThresholdPropulsion ||
							currentCardanPower > ShiftStrategyParameters.CurrentCardanPowerThresholdPropulsion;

			return propulsion
				? PropulsionShiftDecision(
					absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime)
				: CoastingBrakingShiftDecision(
					absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, lastShiftTime);
		}

		private Watt CalcAverageCardanPower()
		{
			var sumCardanPower = 0.SI<WattSecond>();
			var sumDt = 0.SI<Second>();
			foreach (var entry in HistoryBuffer) {
				sumDt += entry.Value.dt;
				sumCardanPower += entry.Value.AvgCardanPower * entry.Value.dt;
			}

			return sumCardanPower / sumDt;
		}

		private bool PropulsionShiftDecision(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime)
		{
			var lookAheadDistance =
				DataBus.VehicleInfo.VehicleSpeed * GearboxModelData.TractionInterruption; //ShiftStrategyParameters.GearResidenceTime;
			var roadGradient = DataBus.DrivingCycleInfo.CycleLookAhead(lookAheadDistance).RoadGradient;
			
			var currentVelocity = DataBus.VehicleInfo.VehicleSpeed;
			gradient = CalcGradientDuringGearshift(false, dt, currentVelocity);
			var estimatedVelocityPostShift = VelocityDropData.Interpolate(currentVelocity, gradient);
			var predictionVelocity = CalcPredictionVelocity(currentVelocity, estimatedVelocityPostShift);

			if (driverAccelerationAvg <= ShiftStrategyParameters.DriverAccelerationThresholdLow) {
				driverAccelerationAvg = 0.SI<MeterPerSquareSecond>();
			}

			
			var gearLimitHigh = Math.Min(GearboxModelData.Gears.Count, gear.Gear + ShiftStrategyParameters.AllowedGearRangeUp);

			GearRatings.Clear();

			// calc rating for current gear
			var ratingCurrentGear = CalcGearRating(absTime, outAngularVelocity, gear, gear, roadGradient, predictionVelocity, estimatedVelocityPostShift);
			if (ratingCurrentGear.RatingCase == GearRatingCase.A) {
				ratingCurrentGear = new GearRating(
					GearRatingCase.A, ratingCurrentGear.Rating * ShiftStrategyParameters.RatingFactorCurrentGear, ratingCurrentGear.MaxEngineSpeed);
			}
			GearRatings[gear.Gear] = ratingCurrentGear;

			// calc rating for lower gears (down to gear limit or rating exceeds engine speed)

			var loop = true;
			var gearLimitLow = (uint)Math.Max(1, gear.Gear - ShiftStrategyParameters.AllowedGearRangeDown);
			for (var i = gear.Gear - 1; loop && i >= gearLimitLow; i--) {
				var rating = CalcGearRating(absTime, outAngularVelocity, gear, new GearshiftPosition(i), roadGradient, predictionVelocity, estimatedVelocityPostShift);

				if (rating.RatingCase == GearRatingCase.E) {
					loop = false;
				}

				GearRatings[i] = rating;
			}

			// calc rating for higher gears (up to gear limit or rating exceeds engine speed)

			loop = true;
			for (var i = gear.Gear + 1; loop && i <= gearLimitHigh; i++) {
				var rating = CalcGearRating(absTime, outAngularVelocity, gear, new GearshiftPosition(i), roadGradient, predictionVelocity, estimatedVelocityPostShift);

				if (rating.RatingCase == GearRatingCase.E) {
					loop = false;
				}

				GearRatings[i] = rating;
			}

			var selectedGear = GearRatings.OrderBy(x => x.Value).First().Key;

			if (selectedGear < gear.Gear && (DownshiftAllowed(absTime) || inAngularVelocity < GetEngineSpeedLimitLow(false))) {
				_nextGear = new GearshiftPosition(selectedGear);
			}
			if (selectedGear > gear.Gear && (UpshiftAllowed(absTime) || inAngularVelocity > GearRatings[gear.Gear].MaxEngineSpeed)) {
				_nextGear = new GearshiftPosition(selectedGear);
			}

			return _nextGear != gear;
		}

		private GearRating CalcGearRating(
			Second absTime, PerSecond outAngularVelocity, GearshiftPosition gear, GearshiftPosition nextGear, Radian roadGradient, 
			MeterPerSecond predictionVelocity, MeterPerSecond estimatedVelocityPostShift)
		{
			var gradientBelowMaxGrad = roadGradient < MaxGradability.GradabilityLimitedTorque(nextGear.Gear);
			var engineSpeedAboveMin =
				outAngularVelocity * GearboxModelData.Gears[nextGear.Gear].Ratio > PowertrainConfig.EngineData.IdleSpeed;

			var engineSpeedBelowMax = outAngularVelocity * GearboxModelData.Gears[nextGear.Gear].Ratio <
									PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed;

			if (!(gradientBelowMaxGrad && engineSpeedAboveMin && engineSpeedBelowMax)) {
				return new GearRating(GearRatingCase.E, 0, null); 
			}

			return RatingGear(
				false, nextGear, gear, gradient, predictionVelocity, estimatedVelocityPostShift, accRsv, driverAccelerationAvg);
		}

		private MeterPerSquareSecond GetAverageAcceleration(Second absTime)
		{
			if (!AccelerationBuffer.Any()) {
				return 0.SI<MeterPerSquareSecond>();
			}

			var sumTime = 0.SI<Second>();
			var sumAcc = 0.SI<MeterPerSecond>();
			var start = absTime - ShiftStrategyParameters.DriverAccelerationLookBackInterval;

			foreach (var entry in AccelerationBuffer) {
				var time = VectoMath.Max(VectoMath.Min(entry.Key - start, 0.SI<Second>()) + entry.Value.dt, 0.SI<Second>());
				var acc = entry.Value.Acceleration * time;
				sumTime += time;
				sumAcc += acc;
			}

			var avgAcc = sumAcc / VectoMath.Max(sumTime, 10.SI<Second>());

			return avgAcc > 0.1.SI<MeterPerSquareSecond>() ? avgAcc : 0.SI<MeterPerSquareSecond>();
		}

		private bool UpshiftAllowed(Second absTime)
		{
			return (absTime - _gearbox.LastDownshift).IsGreaterOrEqual(GearshiftParams.UpshiftAfterDownshiftDelay);
		}

		private bool DownshiftAllowed(Second absTime)
		{
			return (absTime - _gearbox.LastUpshift).IsGreaterOrEqual(GearshiftParams.DownshiftAfterUpshiftDelay);
		}

		private GearRating RatingGear(
			bool driveOff, GearshiftPosition gear, GearshiftPosition currentGear, Radian gradient, MeterPerSecond predictionVelocity,
			MeterPerSecond velocityAfterGearshift, MeterPerSquareSecond accRsv, MeterPerSquareSecond driverAccelerationAvg)
		{
			TestContainerGbx.Gear = gear;
			TestContainer.VehiclePort.Initialize(predictionVelocity, gradient);
			
			var respAccRsv = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				accRsv, gradient, true);

			if (respAccRsv.Engine.EngineSpeed < PowertrainConfig.EngineData.IdleSpeed ||
				respAccRsv.Engine.EngineSpeed > PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed) {
				return new GearRating(GearRatingCase.E, 0, 0.RPMtoRad());
			}

			GearRating? retVal;
			var engineSpeedLowThreshold = GetEngineSpeedLimitLow(driveOff);

			var respConstVel = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				0.SI<MeterPerSquareSecond>(), gradient, true);
			var engineSpeedHighThreshold = GetEngineSpeedLimitHigh(
				driveOff, gear, respAccRsv.Engine.EngineSpeed, respConstVel.Axlegear.CardanTorque);
			if (respAccRsv.Engine.EngineSpeed < engineSpeedLowThreshold) {
				return new GearRating(
					GearRatingCase.D,
					(engineSpeedLowThreshold - respAccRsv.Engine.EngineSpeed).AsRPM, engineSpeedHighThreshold);
			}

			if (respAccRsv.Engine.EngineSpeed > engineSpeedHighThreshold) {
				return new GearRating(
					GearRatingCase.D,
					(respAccRsv.Engine.EngineSpeed - engineSpeedHighThreshold).AsRPM, engineSpeedHighThreshold);
			}

			ResponseDryRun respDriverDemand = null;
			if (respAccRsv.Engine.TotalTorqueDemand <= respAccRsv.Engine.DynamicFullLoadTorque) {
				respDriverDemand = DriverDemandResponse(gradient, driverAccelerationAvg);
				
				var fc = PowertrainConfig.EngineData.Fuels.First().ConsumptionMap.GetFuelConsumption(
					respDriverDemand.Engine.TotalTorqueDemand.LimitTo(
						PowertrainConfig.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(respAccRsv.Engine.EngineSpeed),
						PowertrainConfig.EngineData.FullLoadCurves[0].FullLoadStationaryTorque(respAccRsv.Engine.EngineSpeed)),
					respAccRsv.Engine.EngineSpeed);
				retVal = new GearRating(
					GearRatingCase.A,
					(fc.Value.ConvertToGrammPerHour().Value / VectoMath.Max(respDriverDemand.Axlegear.PowerRequest, 1.SI<Watt>())).Value() *
					1e3,
					engineSpeedHighThreshold);
			} else {
				retVal = new GearRating(
					GearRatingCase.B, (respAccRsv.Engine.PowerRequest - respAccRsv.Engine.DynamicFullLoadPower).Value(),
					engineSpeedHighThreshold);
			}

			if (gear.Gear > currentGear.Gear) {
				respDriverDemand = respDriverDemand ?? DriverDemandResponse(gradient, driverAccelerationAvg);
				var estimatedResidenceTime =
					EstimateResidenceTimeInGear(
						gear, respDriverDemand, engineSpeedHighThreshold, velocityAfterGearshift, gradient, engineSpeedLowThreshold);
				if (estimatedResidenceTime != null && estimatedResidenceTime < ShiftStrategyParameters.GearResidenceTime) {
					retVal = new GearRating(
						GearRatingCase.C, (ShiftStrategyParameters.GearResidenceTime - estimatedResidenceTime).Value(),
						engineSpeedHighThreshold);
				}
			}
			return retVal.Value;
		}

		private ResponseDryRun DriverDemandResponse(Radian gradient, MeterPerSquareSecond driverAccelerationAvg)
		{
			var respDriverDemand = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, driverAccelerationAvg, gradient,
				true);

			if (respDriverDemand.DeltaFullLoad.IsGreater(0)) {
				driverAccelerationAvg = SearchAlgorithm.Search(
					driverAccelerationAvg, respDriverDemand.DeltaFullLoad,
					Constants.SimulationSettings.OperatingPointInitialSearchIntervalAccelerating,
					getYValue: response => {
						var r = (ResponseDryRun)response;
						return r.DeltaFullLoad;
					},
					evaluateFunction:
					acc => {
						var response = TestContainer.VehiclePort.Request(
							0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, acc, gradient, true);
						return response;
					},
					criterion: response => {
						var r = (ResponseDryRun)response;
						return r.DeltaFullLoad.Value() / 1e5;
					});
				respDriverDemand = (ResponseDryRun)TestContainer.VehiclePort.Request(
					0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, driverAccelerationAvg, gradient,
					true);
			}
			return respDriverDemand;
		}

		private Radian CalcGradientDuringGearshift(bool driveOff, Second dt, MeterPerSecond currentVelocity)
		{
			var lookaheadMidShift = driveOff
				? ShiftStrategyParameters.StartVelocity /
				ShiftStrategyParameters.StartAcceleration / 2.0 * ShiftStrategyParameters.StartVelocity
				: currentVelocity * dt / 2.0;
			if (lookaheadMidShift.IsEqual(0)) {
				return DataBus.DrivingCycleInfo.RoadGradient;
			}
			var currentAltitude = DataBus.DrivingCycleInfo.Altitude;
			var lookAheadPos = DataBus.DrivingCycleInfo.CycleLookAhead(lookaheadMidShift);
			var gradient = VectoMath.InclinationToAngle((lookAheadPos.Altitude - currentAltitude) / lookaheadMidShift);
			return lookAheadPos.RoadGradient;
		}

		private MeterPerSquareSecond CalcAccelerationReserve(MeterPerSecond currentVelocity, Second absTime)
		{
			var lastTargetspeedChange = DataBus.DrivingCycleInfo.LastTargetspeedChange;
			demandedSpeed = ComputeDemandedSpeed(lastTargetspeedChange, absTime);
			var accRsvLow = ShiftStrategyParameters.AccelerationReserveLookup.LookupLow(currentVelocity);
			var accRsvHigh = ShiftStrategyParameters.AccelerationReserveLookup.LookupHigh(currentVelocity);
			var targetSpeedDeviationLim = (demandedSpeed - currentVelocity).LimitTo(
				0.KMPHtoMeterPerSecond(), demandedSpeed * ShiftStrategyParameters.TargetSpeedDeviationFactor);
			var accr = VectoMath.Interpolate(
				0.KMPHtoMeterPerSecond(),
				VectoMath.Max(demandedSpeed * ShiftStrategyParameters.TargetSpeedDeviationFactor, 0.001.SI<MeterPerSecond>()),
				accRsvLow, accRsvHigh,
				targetSpeedDeviationLim);
			return accr;
		}

		protected MeterPerSecond ComputeDemandedSpeed(SpeedChangeEntry lastTargetspeedChange, Second absTime)
		{
			var accelerationTime = absTime - lastTargetspeedChange.AbsTime;
			return VectoMath.Min(
				PowertrainConfig.DriverData.AccelerationCurve.ComputeEndVelocityAccelerate(
					lastTargetspeedChange.PreviousTargetSpeed, accelerationTime),
				DataBus.DrivingCycleInfo.CycleData.LeftSample.VehicleTargetSpeed);
		}

		private MeterPerSecond CalcPredictionVelocity(
			MeterPerSecond currentVelocity, MeterPerSecond estimatedVelocityPostShift)
		{
			var ratioSpeedDrop = estimatedVelocityPostShift / VectoMath.Max(currentVelocity, 0.1.SI<MeterPerSecond>());
			var predictionIntervalRatio = ShiftStrategyParameters.PredictionDurationLookup.Lookup(ratioSpeedDrop.Value());
			var speedChange = estimatedVelocityPostShift - currentVelocity;
			return currentVelocity + speedChange * predictionIntervalRatio;
		}

		private PerSecond GetEngineSpeedLimitHigh(bool driveOff, GearshiftPosition gear, PerSecond engineSpeed, NewtonMeter cardanTorque)
		{
			if (driveOff) {
				return ShiftStrategyParameters.EngineSpeedHighDriveOffFactor *
						PowertrainConfig.EngineData.FullLoadCurves[0].NTq99hSpeed;
			}

			var maxCardanTorque = MaxCardanTorqueLookup.Lookup(gear.Gear, engineSpeed);
			var ratioTorqueCardan = cardanTorque / maxCardanTorque;
			var engineSpeedHighMin = GetEngineSpeedLimitHighMin();
			var ratioEngineSpeedCurrMax = ShiftStrategyParameters.ShareEngineHigh.Lookup(ratioTorqueCardan);
			return engineSpeedHighMin + ratioEngineSpeedCurrMax *
					(PowertrainConfig.EngineData.FullLoadCurves[0].NP99hSpeed - engineSpeedHighMin);
		}

		private bool CoastingBrakingShiftDecision(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime)
		{
			var upperEngineSpeedLimit = (PowertrainConfig.EngineData.FullLoadCurves[0].NTq99lSpeed +
										PowertrainConfig.EngineData.FullLoadCurves[0].NTq99hSpeed) / 2.0;

			var currentVelocity = DataBus.VehicleInfo.VehicleSpeed;
			var gradient = CalcGradientDuringGearshift(false, dt, currentVelocity);
			var estimatedVelocityPostShift = VelocityDropData.Interpolate(currentVelocity, gradient);
			var predictedVelocity = DataBus.DriverInfo.DriverBehavior == DrivingBehavior.Braking
				? currentVelocity + PowertrainConfig.DriverData.AccelerationCurve.Lookup(currentVelocity).Deceleration *
				GearboxModelData.TractionInterruption
				: CalcPredictionVelocity(currentVelocity, estimatedVelocityPostShift);

			if (inAngularVelocity < GetEngineSpeedLimitLow(false)) {
				for (var i = Math.Max(1, gear.Gear - ShiftStrategyParameters.AllowedGearRangeDown);
					i <= Math.Min(GearboxModelData.Gears.Count, gear.Gear + ShiftStrategyParameters.AllowedGearRangeUp);
					i++) {
					var nextGear = new GearshiftPosition((uint)i);
					TestContainerGbx.Gear = nextGear;
					var init = TestContainer.VehiclePort.Initialize(predictedVelocity, gradient);
					if (init.Engine.EngineSpeed > GetEngineSpeedLimitLow(false) &&
						init.Engine.EngineSpeed < upperEngineSpeedLimit) {
						_nextGear = nextGear;
						return true;
					}
				}
			}

			if (inAngularVelocity >= upperEngineSpeedLimit) {
				for (var i = Math.Min(GearboxModelData.Gears.Count, gear.Gear + ShiftStrategyParameters.AllowedGearRangeUp);
					i >= Math.Max(1, gear.Gear + ShiftStrategyParameters.AllowedGearRangeDown);
					i--) {
					var nextGear = new GearshiftPosition((uint)i);
					TestContainerGbx.Gear = nextGear;
					var init = TestContainer.VehiclePort.Initialize(predictedVelocity, gradient);
					if (init.Engine.EngineSpeed > GetEngineSpeedLimitLow(false) &&
						init.Engine.EngineSpeed < upperEngineSpeedLimit) {
						_nextGear = nextGear;
						return true;
					}
				}
			}

			return false;
		}


		private void UpdateHistoryBuffer(Second absTime, Second dt, Watt currentCardanPower, MeterPerSecond velocity)
		{
			HistoryBuffer[absTime] = new HistoryEntry() {
				dt = dt,
				AvgCardanPower = currentCardanPower,
				AvgSpeed = velocity,
			};
			var oldEntries = HistoryBuffer.Keys.Where(x => x < absTime + dt - ShiftStrategyParameters.LookBackInterval)
										.ToArray();
			foreach (var entry in oldEntries) {
				HistoryBuffer.Remove(entry);
			}

			//MeterPerSquareSecond aDemanded;
			//var aLimit = PowertrainConfig.DriverData.AccelerationCurve.Lookup(DataBus.VehicleSpeed);
			//if (DataBus.DriverBehavior == DrivingBehavior.Braking) {
			//	aDemanded = aLimit.Deceleration;
			//} else if (DataBus.DriverBehavior == DrivingBehavior.Coasting) {
			//	aDemanded = 0.SI<MeterPerSquareSecond>();
			//} else {
			//	var lastTargetspeedChange = DataBus.LastTargetspeedChange;
			//	var vDemanded = ComputeDemandedSpeed(lastTargetspeedChange, absTime);
			//	aDemanded = ((vDemanded - DataBus.VehicleSpeed) / dt).LimitTo(aLimit.Deceleration, aLimit.Acceleration);
			//}
			AccelerationBuffer[absTime] = new AccelerationEntry() {
				dt = dt,
				//Acceleration = VectoMath.Max(aDemanded, 0.SI<MeterPerSquareSecond>())
				Acceleration = DataBus.DriverInfo.DriverAcceleration
			};

			var outdated = AccelerationBuffer
				.Where(x => x.Key + x.Value.dt < absTime - ShiftStrategyParameters.DriverAccelerationLookBackInterval)
				.Select(x => x.Key).ToArray();

			foreach (var entry in outdated) {
				AccelerationBuffer.Remove(entry);
			}
		}

		public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(torque, outAngularVelocity);
			}

			for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {
				var inAngularVelocity = outAngularVelocity * GearboxModelData.Gears[gear].Ratio;
				if (DataBus.EngineInfo.EngineSpeed < inAngularVelocity && inAngularVelocity < DataBus.EngineInfo.EngineRatedSpeed) {
					_nextGear = new GearshiftPosition(gear);
					return _nextGear;
				}
			}

			return Gears.First();
		}

		private GearshiftPosition InitStartGear(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var maxStartGear = (int)Math.Round(GearboxModelData.Gears.Count / 2.0, MidpointRounding.AwayFromZero);

			var currentVelocity = DataBus.VehicleInfo.VehicleSpeed;
			var gradient = CalcGradientDuringGearshift(true, null, null);
			var estimatedVelocityPostShift = VelocityDropData.Interpolate(currentVelocity, gradient);
			var predictionVelocity = ShiftStrategyParameters.StartVelocity;
			accRsv = ShiftStrategyParameters.StartAcceleration;

			var startGear = Gears.First();
			var minRating = new GearRating(GearRatingCase.E, double.MaxValue, 0.RPMtoRad());
			var roadGradient = DataBus.DrivingCycleInfo.CycleData.LeftSample.RoadGradient; //CycleLookAhead(0.SI<Meter>()).RoadGradient;
			GearRatings.Clear();
			for (var i = (uint)maxStartGear; i > 0; i--) {
				var gradientBelowMaxGrad = roadGradient < MaxGradability.GradabilityLimitedTorque(i);
				var engineSpeedLimitLow = GetEngineSpeedLimitLow(true);
				var engineSpeed = GetEngineSpeed(i);
				var engineSpeedAboveMin = engineSpeed > engineSpeedLimitLow;
				var engineSpeedBelowN95h = engineSpeed < PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed;
				if (gradientBelowMaxGrad && engineSpeedAboveMin && engineSpeedBelowN95h) {
					var rating = RatingGear(true, new GearshiftPosition(i), new GearshiftPosition(0), roadGradient, predictionVelocity, estimatedVelocityPostShift, accRsv, accRsv);
					GearRatings[i] = rating;
					if (rating < minRating) {
						minRating = rating;
						startGear = new GearshiftPosition(i);
					}
				}
			}

			_nextGear = startGear;
			return startGear;
		}

		private PerSecond GetEngineSpeed(uint gear)
		{
			return EngineSpeedAtDriveOff[gear];
		}

		private PerSecond GetEngineSpeedLimitLow(bool driveOff)
		{
			var shareIdleLowMax = driveOff
				? ShiftStrategyParameters.ShareIdleLow.MaxValue
				: ShiftStrategyParameters.ShareIdleLow.Lookup(DataBus.VehicleInfo.VehicleSpeed);
			return PowertrainConfig.EngineData.IdleSpeed + shareIdleLowMax *
					(PowertrainConfig.EngineData.FullLoadCurves[0].NP99hSpeed - PowertrainConfig.EngineData.IdleSpeed);
		}


		private Second EstimateResidenceTimeInGear(
			GearshiftPosition gear, ResponseDryRun responseDriverDemand, PerSecond engineSpeedHighThreshold,
			MeterPerSecond velocityAfterGearshift, Radian estimatedGradient, PerSecond engineSpeedLowThreshold)
		{
			// get total 'transmission ratio' of powertrain
			var engineSpeed = responseDriverDemand.Engine.EngineSpeed;
			var vehicleSpeed = responseDriverDemand.Vehicle.VehicleSpeed;
			var ratio = (vehicleSpeed / engineSpeed).Cast<Meter>();

			var estimatedEngineSpeed = velocityAfterGearshift / ratio;
			if (estimatedEngineSpeed < engineSpeedLowThreshold || estimatedEngineSpeed > engineSpeedHighThreshold) {
				return null;
			}

			var averageAccelerationTorque = AverageAccelerationTorqueLookup.Interpolate(
				responseDriverDemand.Engine.EngineSpeed, responseDriverDemand.Engine.TorqueOutDemand);

			TestContainerGbx.Gear = gear;
			var initResponse = TestContainer.VehiclePort.Initialize(vehicleSpeed, estimatedGradient);
			var delta = initResponse.Engine.TorqueOutDemand - averageAccelerationTorque;
			var acceleration = SearchAlgorithm.Search(
				0.SI<MeterPerSquareSecond>(), delta, 0.1.SI<MeterPerSquareSecond>(),
				getYValue: r => { return (r as AbstractResponse).Engine.TorqueOutDemand - averageAccelerationTorque; },
				evaluateFunction: a => {
					return TestContainer.VehiclePort.Request(
						0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, a, estimatedGradient, true);
				},
				criterion: r => { return ((r as AbstractResponse).Engine.TorqueOutDemand - averageAccelerationTorque).Value(); }
			);

			var engineAcceleration = (acceleration / ratio).Cast<PerSquareSecond>();
			var deltaEngineSpeed = engineSpeedHighThreshold - engineSpeed;
			if (engineAcceleration.IsGreater(0) && deltaEngineSpeed > 0) {
				return (deltaEngineSpeed / engineAcceleration).Cast<Second>();
			}

			return null;
		}

		private PerSecond GetEngineSpeedLimitHighDriveOff(uint gear)
		{
			return ShiftStrategyParameters.EngineSpeedHighDriveOffFactor *
					PowertrainConfig.EngineData.FullLoadCurves[0].NTq99hSpeed;
		}

		private PerSecond GetEngineSpeedLimitHighMin()
		{
			var fld = PowertrainConfig.EngineData.FullLoadCurves[0];
			var wT99l = fld.NTq99lSpeed;
			var wT99h = fld.NTq99hSpeed;
			var wP99h = fld.NP99hSpeed;
			var max1 = wT99l + ShiftStrategyParameters.DnT99L_highMin1 * (wP99h - wT99l);
			var max2 = wT99l + ShiftStrategyParameters.DnT99L_highMin2 * (wP99h - wT99l);

			return VectoMath.Max(max1, VectoMath.Min(max2, wT99h));
		}


		public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			//_nextGear = gear;
			while (_nextGear.Gear > 1 && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}
			while (_nextGear.Gear < GearboxModelData.Gears.Count &&
					SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = Gears.Successor(_nextGear);
			}

			return _nextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public override GearshiftPosition NextGear
		{
			get { return _nextGear; }
		}

		public static string Name { get { return "AMT - ACEA TCU"; } }

		#endregion

		public override void WriteModalResults(IModalDataContainer container)
		{
			foreach (var gear in GearboxModelData.Gears.Keys) {
				container.SetDataValue(
					string.Format("Gear{0}-Rating", gear),
					GearRatings.ContainsKey(gear)
						? GearRatings[gear].NumericValue
						: new GearRating(GearRatingCase.Z, 0, null).NumericValue);
			}

			container.SetDataValue("acc_rsv", accRsv?.Value() ?? 0);
			container.SetDataValue("v_dem", demandedSpeed?.AsKmph ?? 0);
			container.SetDataValue("acc_driver_avg", driverAccelerationAvg?.Value() ?? 0);
			container.SetDataValue("grad_shift", Math.Tan(gradient.Value()) * 100);
			GearRatings.Clear();
			accRsv = null;
		}

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData = null)
		{
			return null;
		}
	}
}
