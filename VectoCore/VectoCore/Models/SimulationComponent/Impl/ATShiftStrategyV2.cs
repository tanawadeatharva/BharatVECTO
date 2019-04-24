using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class ATShiftStrategyV2 : BaseShiftStrategy
	{

		private ATGearbox _gearbox;

		protected Dictionary<Second, HistoryEntry> HistoryBuffer = new Dictionary<Second, HistoryEntry>();
		protected Dictionary<Second, AccelerationEntry> AccelerationBuffer = new Dictionary<Second, AccelerationEntry>();

		private MeterPerSecond demandedSpeed = 0.SI<MeterPerSecond>();
		private MeterPerSquareSecond accRsv = 0.SI<MeterPerSquareSecond>();
		private MeterPerSquareSecond driverAccelerationAvg;
		private Radian gradient = 0.SI<Radian>();

		protected readonly VectoRunData PowertrainConfig;
		protected readonly ShiftStrategyParameters ShiftStrategyParameters;

		protected SimplePowertrainContainer TestContainer;
		protected readonly MaxGradabilityLookup MaxGradability;
		protected MaxCardanTorqueLookup MaxCardanTorqueLookup;
		protected AverageAccelerationTorqueLookup AverageAccelerationTorqueLookup;

		private Dictionary<int, GearRating> GearRatings = new Dictionary<int, GearRating>();


		private readonly NextGearState _nextGear = new NextGearState();

		public ATShiftStrategyV2(VectoRunData data, IVehicleContainer dataBus) : base(data.GearboxData, dataBus)
		{
			PowertrainConfig = data;
			ShiftStrategyParameters = data.GearshiftParameters;

			//GearShiftSequence = gearList.ToArray();

			// create a dummy powertrain for pre-processing and estimatins
			var modData = new ModalDataContainer(data, null, null, false);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(data);
			TestContainer.AbsTime = -double.MaxValue.SI<Second>();
			builder.BuildSimplePowertrain(data, TestContainer);


			MaxGradability = new MaxGradabilityLookup();
			dataBus.AddPreprocessor(new MaxGradabilityPreprocessor(MaxGradability, data, TestContainer));

			MaxCardanTorqueLookup = new MaxCardanTorqueLookup();
			dataBus.AddPreprocessor(new MaxCardanTorquePreprocessor(MaxCardanTorqueLookup, data, TestContainer));

			AverageAccelerationTorqueLookup = new AverageAccelerationTorqueLookup();
			dataBus.AddPreprocessor(
				new AverageAccelerationTorquePreprocessor(AverageAccelerationTorqueLookup, data, GetEngineSpeedLimitHighMin()));

		}


		public override IGearbox Gearbox
		{
			get { return _gearbox; }
			set {
				_gearbox = value as ATGearbox;
				if (_gearbox == null) {
					throw new VectoException("AT Shift strategy can only handle AT gearboxes, given: {0}", value.GetType());
				}
			}
		}

		#region Overrides of BaseShiftStrategy

		public override void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			// update own history
			var velocity = DataBus.VehicleSpeed + DataBus.DriverAcceleration * dt / 2.0;
			var cardanDemand = DataBus.CurrentAxleDemand;
			var currentCardanPower = cardanDemand.Item1 * cardanDemand.Item2;

			UpdateHistoryBuffer(absTime, dt, currentCardanPower, velocity);

			var currentVelocity = DataBus.VehicleSpeed;
			accRsv = CalcAccelerationReserve(currentVelocity, absTime + dt);

			driverAccelerationAvg = GetAverageAcceleration(absTime + dt);

			gradient = CalcGradientDuringGearshift(false, dt, currentVelocity);

		}

		public override bool ShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var cardanDemand = DataBus.CurrentAxleDemand;
			var currentCardanPower = cardanDemand.Item1 * cardanDemand.Item2;

			// no shift when vehicle stands
			if (DataBus.VehicleStopped) {
				return false;
			}

			// EMERGENCY SHIFTS ---------------------------------------
			if (CheckEmergencyShift(absTime, outTorque, outAngularVelocity, inAngularVelocity, gear)) {
				return true;
			}


			// TEST
			var currentVelocity = DataBus.VehicleSpeed;
			accRsv = CalcAccelerationReserve(currentVelocity, absTime + dt);

			var minimumShiftTimePassed = (lastShiftTime + ModelData.ShiftTime).IsSmallerOrEqual(absTime);
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

		private bool PropulsionShiftDecision(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gearboxGear, Second lastShiftTime)
		{
			var lookAheadDistance =
				DataBus.VehicleSpeed * ModelData.TractionInterruption; //ShiftStrategyParameters.GearResidenceTime;
			var roadGradient = DataBus.CycleLookAhead(lookAheadDistance).RoadGradient;

			var currentVelocity = DataBus.VehicleSpeed;
			gradient = CalcGradientDuringGearshift(false, dt, currentVelocity);
			//var estimatedVelocityPostShift = DataBus.VehicleSpeed; //VelocityDropData.Interpolate(currentVelocity, gradient);
			//var predictionVelocity = DataBus.VehicleSpeed; //CalcPredictionVelocity(currentVelocity, estimatedVelocityPostShift);

			if (driverAccelerationAvg <= ShiftStrategyParameters.DriverAccelerationThresholdLow) {
				driverAccelerationAvg = 0.SI<MeterPerSquareSecond>();
			}

			var gear = Gearbox.Gears.ToList().FindIndex(x => x.Gear == gearboxGear && x.TorqueConverterLocked == _gearbox.TCLocked);
			var gearLimitHigh = Math.Min(Gearbox.Gears.Count, gear + ShiftStrategyParameters.AllowedGearRangeUp);

			GearRatings.Clear();

			// calc rating for current gear
			var ratingCurrentGear = CalcGearRating(absTime, outAngularVelocity, gear, gear, roadGradient);
			if (ratingCurrentGear.RatingCase == GearRatingCase.A) {
				ratingCurrentGear = new GearRating(
					GearRatingCase.A, ratingCurrentGear.Rating * ShiftStrategyParameters.RatingFactorCurrentGear, ratingCurrentGear.MaxEngineSpeed);
			}
			GearRatings[gear] = ratingCurrentGear;

			// calc rating for lower gears (down to gear limit or rating exceeds engine speed)

			var loop = true;
			var gearLimitLow = (uint)Math.Max(0, gear - ShiftStrategyParameters.AllowedGearRangeDown);
			for (var i = gear - 1; loop && i >= gearLimitLow; i--) {
				var rating = CalcGearRating(absTime, outAngularVelocity, gear, i, roadGradient);

				if (rating.RatingCase == GearRatingCase.E) {
					loop = false;
				}

				GearRatings[i] = rating;
			}

			// calc rating for higher gears (up to gear limit or rating exceeds engine speed)

			loop = true;
			for (var i = gear + 1; loop && i < gearLimitHigh; i++) {
				var rating = CalcGearRating(absTime, outAngularVelocity, gear, i, roadGradient);

				if (rating.RatingCase == GearRatingCase.E) {
					loop = false;
				}

				GearRatings[i] = rating;
			}

			var selectedGear = GearRatings.OrderBy(x => x.Value).First().Key;

			var retVal = false;
			if (selectedGear < gear && (DownshiftAllowed(absTime, lastShiftTime) || inAngularVelocity < GetEngineSpeedLimitLow(false))) {
				_nextGear.SetState(Gearbox.Gears[selectedGear], absTime);
				retVal = true;
			}
			if (selectedGear > gear && (UpshiftAllowed(absTime, lastShiftTime) || inAngularVelocity > GearRatings[gear].MaxEngineSpeed)) {
				_nextGear.SetState(Gearbox.Gears[selectedGear], absTime);
				retVal = true;
			}

			return retVal;
		}

		private bool UpshiftAllowed(Second absTime, Second lastShiftTime)
		{
			return (absTime - lastShiftTime).IsGreaterOrEqual(ModelData.ShiftTime);
		}

		private bool DownshiftAllowed(Second absTime, Second lastShiftTime)
		{
			return (absTime - lastShiftTime).IsGreaterOrEqual(ModelData.ShiftTime);
		}

		private GearRating CalcGearRating(
			Second absTime, PerSecond outAngularVelocity, int gearIdx, int nextGearIdx, Radian roadGradient)
		{
			var nextGear = Gearbox.Gears[nextGearIdx].Gear;
			var gradientBelowMaxGrad = roadGradient < MaxGradability.GradabilityLimitedTorque(Gearbox.Gears[nextGearIdx]);
			var engineSpeedAboveMin =
				outAngularVelocity * ModelData.GearRatio(Gearbox.Gears[nextGearIdx]) > PowertrainConfig.EngineData.IdleSpeed;

			var engineSpeedBelowMax = outAngularVelocity * ModelData.GearRatio(Gearbox.Gears[nextGearIdx]) <
									PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed;

			if (!(gradientBelowMaxGrad && engineSpeedAboveMin && engineSpeedBelowMax)) {
				return new GearRating(GearRatingCase.E, 0, null);
			}

			return RatingGear(
				false, nextGearIdx, gearIdx, gradient, accRsv, driverAccelerationAvg);
		}


		private GearRating RatingGear(
			bool driveOff, int gearIdx, int currentGearIdx, Radian roadGradient, MeterPerSquareSecond accReserve, MeterPerSquareSecond driverAccelerationAvg)
		{
			var gear = Gearbox.Gears[gearIdx];
			TestContainer.GearboxCtl.SetGear = new GearshiftPosition(gear.Gear, gear.TorqueConverterLocked);
			TestContainer.VehiclePort.Initialize(DataBus.VehicleSpeed, roadGradient);

			var respAccRsv = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				accReserve, roadGradient, true);

			if (respAccRsv.EngineSpeed < PowertrainConfig.EngineData.IdleSpeed ||
				respAccRsv.EngineSpeed > PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed) {
				return new GearRating(GearRatingCase.E, 0, 0.RPMtoRad());
			}

			GearRating? retVal;
			var engineSpeedLowThreshold = GetEngineSpeedLimitLow(driveOff);

			var respConstVel = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				0.SI<MeterPerSquareSecond>(), roadGradient, true);
			var engineSpeedHighThreshold = GetEngineSpeedLimitHigh(
				driveOff, gear, respAccRsv.EngineSpeed, respConstVel.CardanTorque);
			if (respAccRsv.EngineSpeed < engineSpeedLowThreshold) {
				return new GearRating(
					GearRatingCase.D,
					(engineSpeedLowThreshold - respAccRsv.EngineSpeed).AsRPM, engineSpeedHighThreshold);
			}

			if (respAccRsv.EngineSpeed > engineSpeedHighThreshold) {
				return new GearRating(
					GearRatingCase.D,
					(respAccRsv.EngineSpeed - engineSpeedHighThreshold).AsRPM, engineSpeedHighThreshold);
			}

			ResponseDryRun respDriverDemand = null;
			if (respAccRsv.EngineTorqueDemandTotal <= respAccRsv.EngineDynamicFullLoadTorque) {
				respDriverDemand = DriverDemandResponse(roadGradient, driverAccelerationAvg);

				var fc = PowertrainConfig.EngineData.ConsumptionMap.GetFuelConsumption(
					respDriverDemand.EngineTorqueDemandTotal.LimitTo(
						PowertrainConfig.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(respAccRsv.EngineSpeed),
						PowertrainConfig.EngineData.FullLoadCurves[0].FullLoadStationaryTorque(respAccRsv.EngineSpeed)),
					respAccRsv.EngineSpeed);
				retVal = new GearRating(
					GearRatingCase.A,
					(fc.Value.ConvertToGrammPerHour().Value / VectoMath.Max(respDriverDemand.AxlegearPowerRequest, 1.SI<Watt>())).Value() *
					1e3,
					engineSpeedHighThreshold);
			} else {
				retVal = new GearRating(
					GearRatingCase.B, (respAccRsv.EnginePowerRequest - respAccRsv.DynamicFullLoadPower).Value(),
					engineSpeedHighThreshold);
			}

			if (gearIdx > currentGearIdx) {
				respDriverDemand = respDriverDemand ?? DriverDemandResponse(roadGradient, driverAccelerationAvg);
				var estimatedResidenceTime =
					EstimateResidenceTimeInGear(
						gear, respDriverDemand, engineSpeedHighThreshold, DataBus.VehicleSpeed, roadGradient, engineSpeedLowThreshold);
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

		private Second EstimateResidenceTimeInGear(
			GearshiftPosition gear, ResponseDryRun responseDriverDemand, PerSecond engineSpeedHighThreshold,
			MeterPerSecond velocityAfterGearshift, Radian estimatedGradient, PerSecond engineSpeedLowThreshold)
		{
			// get total 'transmission ratio' of powertrain
			var engineSpeed = responseDriverDemand.EngineSpeed;
			var vehicleSpeed = responseDriverDemand.VehicleSpeed;
			var ratio = (vehicleSpeed / engineSpeed).Cast<Meter>();

			var estimatedEngineSpeed = velocityAfterGearshift / ratio;
			if (estimatedEngineSpeed < engineSpeedLowThreshold || estimatedEngineSpeed > engineSpeedHighThreshold) {
				return null;
			}

			var averageAccelerationTorque = AverageAccelerationTorqueLookup.Interpolate(
				responseDriverDemand.EngineSpeed, responseDriverDemand.EngineTorqueDemand);

			TestContainer.GearboxCtl.SetGear = new GearshiftPosition(gear.Gear, gear.TorqueConverterLocked);

			var initResponse = TestContainer.VehiclePort.Initialize(vehicleSpeed, estimatedGradient);
			var delta = initResponse.EngineTorqueDemand - averageAccelerationTorque;
			var acceleration = SearchAlgorithm.Search(
				0.SI<MeterPerSquareSecond>(), delta, 0.1.SI<MeterPerSquareSecond>(),
				getYValue: r => { return (r as AbstractResponse).EngineTorqueDemand - averageAccelerationTorque; },
				evaluateFunction: a => {
					return TestContainer.VehiclePort.Request(
						0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, a, estimatedGradient, true);
				},
				criterion: r => { return ((r as AbstractResponse).EngineTorqueDemand - averageAccelerationTorque).Value(); }
			);

			var engineAcceleration = (acceleration / ratio).Cast<PerSquareSecond>();
			var deltaEngineSpeed = engineSpeedHighThreshold - engineSpeed;
			if (engineAcceleration.IsGreater(0) && deltaEngineSpeed > 0) {
				return (deltaEngineSpeed / engineAcceleration).Cast<Second>();
			}

			return null;
		}

		private PerSecond GetEngineSpeedLimitHigh(bool driveOff, GearshiftPosition gear, PerSecond engineSpeed, NewtonMeter cardanTorque)
		{
			if (driveOff) {
				return ShiftStrategyParameters.EngineSpeedHighDriveOffFactor *
						PowertrainConfig.EngineData.FullLoadCurves[0].NTq99hSpeed;
			}

			var maxCardanTorque = MaxCardanTorqueLookup.Lookup(gear, engineSpeed);
			var ratioTorqueCardan = cardanTorque / maxCardanTorque;
			var engineSpeedHighMin = GetEngineSpeedLimitHighMin();
			var ratioEngineSpeedCurrMax = ShiftStrategyParameters.ShareEngineHigh.Lookup(ratioTorqueCardan);
			return engineSpeedHighMin + ratioEngineSpeedCurrMax *
					(PowertrainConfig.EngineData.FullLoadCurves[0].NP99hSpeed - engineSpeedHighMin);
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

		private PerSecond GetEngineSpeedLimitLow(bool driveOff)
		{
			var shareIdleLowMax = driveOff
				? ShiftStrategyParameters.ShareIdleLow.MaxValue
				: ShiftStrategyParameters.ShareIdleLow.Lookup(DataBus.VehicleSpeed);
			return PowertrainConfig.EngineData.IdleSpeed + shareIdleLowMax *
					(PowertrainConfig.EngineData.FullLoadCurves[0].NP99hSpeed - PowertrainConfig.EngineData.IdleSpeed);
		}

		private bool CheckEmergencyShift(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity, PerSecond inAngularVelocity, uint gear)
		{
			// Emergency Downshift: if lower than engine idle speed
			if (inAngularVelocity.IsSmaller(DataBus.EngineIdleSpeed)) {
				Log.Debug("engine speed would fall below idle speed - shift down");
				Downshift(absTime, gear);
				return true;
			}
			// Emergency Upshift: if higher than engine rated speed
			if (inAngularVelocity.IsGreaterOrEqual(VectoMath.Min(ModelData.Gears[gear].MaxSpeed, DataBus.EngineRatedSpeed))) {
				// check if upshift is possible
				if (!ModelData.Gears.ContainsKey(gear + 1)) {
					return false;
				}

				PerSecond nextInAngularSpeed;
				NewtonMeter nextInTorque;
				if (ModelData.Gears[gear].HasLockedGear) {
					nextInAngularSpeed = outAngularVelocity * ModelData.Gears[gear].Ratio;
					nextInTorque = outTorque / ModelData.Gears[gear].Ratio;
				} else {
					nextInAngularSpeed = outAngularVelocity * ModelData.Gears[gear + 1].Ratio;
					nextInTorque = outTorque / ModelData.Gears[gear + 1].Ratio;
				}
				//if (!IsBelowDownShiftCurve(gear + 1, nextInTorque, nextInAngularSpeed)) {
				if (nextInAngularSpeed.IsGreater(DataBus.EngineRatedSpeed)) { 
					Log.Debug("engine speed would be above max speed / rated speed - shift up");
					Upshift(absTime, gear);
					return true;
				}
			}
			return false;
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

		private void Downshift(Second absTime, uint gear)
		{
			// L -> C
			if (_gearbox.TCLocked && ModelData.Gears[gear].HasTorqueConverter) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear, tcLocked: false);
				return;
			}

			// L -> L-1
			// C -> C-1
			if (ModelData.Gears.ContainsKey(gear - 1)) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear - 1, tcLocked: _gearbox.TCLocked);
				return;
			}

			// L -> 0 -- not allowed!!
			throw new VectoSimulationException(
				"ShiftStrategy wanted to shift down but current gear is locked (L) and has no torque converter (C) and disenganging directly from (L) is not allowed.");
		}

		private void Upshift(Second absTime, uint gear)
		{
			// C -> L: switch from torque converter to locked gear
			if (!_gearbox.TCLocked && ModelData.Gears[gear].HasLockedGear) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear, tcLocked: true);
				return;
			}

			// L -> L+1
			// C -> C+1
			if (ModelData.Gears.ContainsKey(gear + 1)) {
				_nextGear.SetState(absTime, disengaged: false, gear: gear + 1, tcLocked: _gearbox.TCLocked);
				return;
			}

			// C -> L+1 -- not allowed!!
			throw new VectoSimulationException(
				"ShiftStrategy wanted to shift up, but current gear has active torque converter (C) but no locked gear (no L) and shifting directly to (L) is not allowed.");
		}


		private bool CoastingBrakingShiftDecision(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gearboxGear, Second lastShiftTime)
		{
			var upperEngineSpeedLimit = (PowertrainConfig.EngineData.FullLoadCurves[0].NTq99lSpeed +
										PowertrainConfig.EngineData.FullLoadCurves[0].NTq99hSpeed) / 2.0;

			var currentVelocity = DataBus.VehicleSpeed;
			var gradient = CalcGradientDuringGearshift(false, dt, currentVelocity);
			//var estimatedVelocityPostShift = VelocityDropData.Interpolate(currentVelocity, gradient);
			var predictedVelocity = DataBus.DriverBehavior == DrivingBehavior.Braking
				? currentVelocity + PowertrainConfig.DriverData.AccelerationCurve.Lookup(currentVelocity).Deceleration *
				ModelData.TractionInterruption
				: DataBus.VehicleSpeed;

			var gearIdx = Gearbox.Gears.ToList()
								.FindIndex(x => x.Gear == gearboxGear && x.TorqueConverterLocked == _gearbox.TCLocked);
			var gear = Gearbox.Gears[gearIdx];

			if (inAngularVelocity < GetEngineSpeedLimitLow(false)) {
				for (var i = Math.Max(0, gearIdx - ShiftStrategyParameters.AllowedGearRangeDown);
					i < Math.Min(ModelData.Gears.Count, gearIdx + ShiftStrategyParameters.AllowedGearRangeUp);
					i++) {
					var nextGear = Gearbox.Gears[i];
					TestContainer.GearboxCtl.SetGear = new GearshiftPosition(nextGear.Gear, nextGear.TorqueConverterLocked);
					var init = TestContainer.VehiclePort.Initialize(predictedVelocity, gradient);
					if (init.EngineSpeed > GetEngineSpeedLimitLow(false) &&
						init.EngineSpeed < upperEngineSpeedLimit) {
						_nextGear.SetState(nextGear, absTime);
						return true;
					}
				}
			}

			if (inAngularVelocity >= upperEngineSpeedLimit) {
				for (var i = Math.Min(Gearbox.Gears.Count, gearIdx + ShiftStrategyParameters.AllowedGearRangeUp);
					i >= Math.Max(0, gearIdx + ShiftStrategyParameters.AllowedGearRangeDown);
					i--) {
					var nextGear = Gearbox.Gears[i];
					TestContainer.GearboxCtl.SetGear = new GearshiftPosition(nextGear.Gear, nextGear.TorqueConverterLocked);
					var init = TestContainer.VehiclePort.Initialize(predictedVelocity, gradient);
					if (init.EngineSpeed > GetEngineSpeedLimitLow(false) &&
						init.EngineSpeed < upperEngineSpeedLimit) {
						_nextGear.SetState(nextGear, absTime);
						return true;
					}
				}
			}

			return false;
		}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				// AT always starts in first gear and TC active!
				_gearbox.TCLocked = false;
				_gearbox.Disengaged = true;
				return 1;
			}
			
			for (var gearIdx = Gearbox.Gears.Count - 1; gearIdx >= 0; gearIdx--) {
				var gear = Gearbox.Gears[gearIdx];
				var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear.Gear].Ratio;
				if (DataBus.EngineSpeed < inAngularVelocity && inAngularVelocity < DataBus.EngineRatedSpeed) {
					_nextGear.SetState(gear, absTime);
					return _nextGear.Gear;
				}
			}
			// fallback: start with first gear;
			_gearbox.TCLocked = false;
			_gearbox.Disengaged = false;
			return 1;
		}

		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (_nextGear.AbsTime != null && _nextGear.AbsTime.IsEqual(absTime)) {
				_gearbox.TCLocked = _nextGear.TorqueConverterLocked;
				_gearbox.Disengaged = _nextGear.Disengaged;
				_nextGear.AbsTime = null;
				return _nextGear.Gear;
			}
			_nextGear.AbsTime = null;
			return _gearbox.Gear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed)
		{
			throw new System.NotImplementedException("AT Shift Strategy does not support disengaging.");
		}

		public override GearshiftPosition NextGear
		{
			get { return new GearshiftPosition(_nextGear.Gear, _nextGear.TorqueConverterLocked); }
		}


		#endregion

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
				Acceleration = DataBus.DriverAcceleration
			};

			var outdated = AccelerationBuffer
				.Where(x => x.Key + x.Value.dt < absTime - ShiftStrategyParameters.DriverAccelerationLookBackInterval)
				.Select(x => x.Key).ToArray();

			foreach (var entry in outdated) {
				AccelerationBuffer.Remove(entry);
			}
		}

		private MeterPerSquareSecond CalcAccelerationReserve(MeterPerSecond currentVelocity, Second absTime)
		{
			var lastTargetspeedChange = DataBus.LastTargetspeedChange;
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
				DataBus.CycleData.LeftSample.VehicleTargetSpeed);
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

		private Radian CalcGradientDuringGearshift(bool driveOff, Second dt, MeterPerSecond currentVelocity)
		{
			var lookaheadMidShift = driveOff
				? ShiftStrategyParameters.StartVelocity /
				ShiftStrategyParameters.StartAcceleration / 2.0 * ShiftStrategyParameters.StartVelocity
				: currentVelocity * dt / 2.0;
			if (lookaheadMidShift.IsEqual(0)) {
				return DataBus.RoadGradient;
			}
			var currentAltitude = DataBus.Altitude;
			var lookAheadPos = DataBus.CycleLookAhead(lookaheadMidShift);
			var gradient = VectoMath.InclinationToAngle((lookAheadPos.Altitude - currentAltitude) / lookaheadMidShift);
			return lookAheadPos.RoadGradient;
		}





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


		private class NextGearState
		{
			public Second AbsTime;
			public bool Disengaged;
			public uint Gear;
			public bool TorqueConverterLocked;

			public NextGearState() { }

			private NextGearState(NextGearState nextGearState)
			{
				AbsTime = nextGearState.AbsTime;
				Disengaged = nextGearState.Disengaged;
				Gear = nextGearState.Gear;
				TorqueConverterLocked = nextGearState.TorqueConverterLocked;
			}

			public NextGearState(Second absTime, ATGearbox gearbox)
			{
				SetState(absTime, gearbox);
			}

			public void SetState(Second absTime, bool disengaged, uint gear, bool tcLocked)
			{
				AbsTime = absTime;
				Disengaged = disengaged;
				Gear = gear;
				TorqueConverterLocked = tcLocked;
			}

			public void SetState(NextGearState state)
			{
				AbsTime = state.AbsTime;
				Disengaged = state.Disengaged;
				Gear = state.Gear;
				TorqueConverterLocked = state.TorqueConverterLocked;
			}

			public void SetState(Second absTime, ATGearbox gearbox)
			{
				AbsTime = absTime;
				Disengaged = gearbox.Disengaged;
				Gear = gearbox.Gear;
				TorqueConverterLocked = gearbox.TCLocked;
			}

			public NextGearState Clone()
			{
				return new NextGearState(this);
			}

			internal void SetState(GearshiftPosition gearshiftPosition, Second absTime)
			{
				AbsTime = absTime;
				Disengaged = false;
				Gear = gearshiftPosition.Gear;
				TorqueConverterLocked = gearshiftPosition.TorqueConverterLocked.Value;
			}
		}
	}
}
