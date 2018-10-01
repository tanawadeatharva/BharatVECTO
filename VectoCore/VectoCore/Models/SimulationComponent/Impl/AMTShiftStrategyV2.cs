using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyV2 : ShiftStrategy
	{
		private uint _nextGear;
		protected readonly MaxGradabilityLookup MaxGradability;
		protected readonly VelocityRollingLookup VelocityDropData;
		protected readonly VectoRunData PowertrainConfig;
		protected readonly ShiftStrategyParameters ShiftStrategyParameters;
		protected Dictionary<uint, PerSecond> EngineSpeedAtDriveOff;
		protected SimplePowertrainContainer TestContainer;
		protected Dictionary<Second, HistoryEntry> HistoryBuffer = new Dictionary<Second, HistoryEntry>();
		protected AverageAccelerationTorqueLookup AverageAccelerationTorqueLookup;
		protected MaxCardanTorqueLookup MaxCardanTorqueLookup;

		protected DebugData DebugData = new DebugData();

		public struct HistoryEntry
		{
			public Second dt;
			public MeterPerSecond AvgSpeed;
			public Watt AvgCardanPower;
		}

		public AMTShiftStrategyV2(VectoRunData data, IVehicleContainer dataBus) : base(data.GearboxData, dataBus)
		{
			PowertrainConfig = data;
			ShiftStrategyParameters = data.GearshiftParameters;

			// create a dummy powertrain for pre-processing and estimatins
			var modData = new ModalDataContainer(data, null, null, false);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(data);
			builder.BuildSimplePowertrain(data, TestContainer);

			// register pre-processors
			VelocityDropData = new VelocityRollingLookup();
			dataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, data.GearboxData.TractionInterruption, TestContainer));

			MaxGradability = new MaxGradabilityLookup();
			dataBus.AddPreprocessor(new MaxGradabilityPreprocessor(MaxGradability, data, TestContainer));

			EngineSpeedAtDriveOff = new Dictionary<uint, PerSecond>(ModelData.Gears.Count);
			dataBus.AddPreprocessor(new EngineSpeedDriveOffPreprocessor(EngineSpeedAtDriveOff, data, TestContainer));

			AverageAccelerationTorqueLookup = new AverageAccelerationTorqueLookup();
			dataBus.AddPreprocessor(new AverageAccelerationTorquePreprocessor(AverageAccelerationTorqueLookup, data, GetEngineSpeedLimitHighMin()));

			MaxCardanTorqueLookup = new MaxCardanTorqueLookup();
			dataBus.AddPreprocessor(new MaxCardanTorquePreprocessor(MaxCardanTorqueLookup, data, TestContainer));
		}

		#region Overrides of BaseShiftStrategy

		public override bool ShiftRequired(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			// update own history
			var velocity = DataBus.VehicleSpeed + DataBus.DriverAcceleration * dt / 2.0;
			var cardanDemand = DataBus.CurrentAxleDemand;
			var currentCardanPower = cardanDemand.Item1 * cardanDemand.Item2;

			UpdateHistoryBuffer(absTime, dt, currentCardanPower, velocity);

			// normal shift when all requirements are fullfilled ------------------
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
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var lookAheadDistance = DataBus.VehicleSpeed * ShiftStrategyParameters.GearResidenceTime;
			var roadGradient = DataBus.CycleLookAhead(lookAheadDistance).RoadGradient;
			var minRating = new GearRating(GearRatingCase.D, double.MaxValue, 0.RPMtoRad());
			var selectedGear = gear;
			var debugData = new DebugData();
			for (var i = Math.Max(0, gear - ShiftStrategyParameters.AllowedGearRangeDown);
				i <= Math.Min(ModelData.Gears.Count, gear + ShiftStrategyParameters.AllowedGearRangeUp);
				i++) {
				var nextGear = (uint)i;

				var gradientBelowMaxGrad = roadGradient < MaxGradability.GradabilityLimitedTorque(nextGear);
				var engineSpeedAboveMin = outAngularVelocity * ModelData.Gears[nextGear].Ratio > GetEngineSpeedLimitLow(false);

				var engineSpeedBelowMax = outAngularVelocity * ModelData.Gears[nextGear].Ratio <
										PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed;
				if (nextGear > gear && !(gradientBelowMaxGrad && engineSpeedAboveMin && engineSpeedBelowMax)) {
					debugData.Add(
						string.Format(
							"{0} - gear {1} exceeds speed limits / gradability {2}", absTime, nextGear,
							outAngularVelocity * ModelData.Gears[nextGear].Ratio));
					continue;
				}

				var rating = RatingGear(false, nextGear, dt);
				if (nextGear == gear) {
					if (rating.RatingCase == GearRatingCase.A) {
						rating = new GearRating(GearRatingCase.A, rating.Rating * ShiftStrategyParameters.RatingFactorCurrentGear, rating.MaxEngineSpeed);
					}
				}
				debugData.Add(string.Format("{0} - gear {1}: rating {2}", absTime, nextGear, rating));
				if (rating < minRating) {
					minRating = rating;
					selectedGear = nextGear;
				}
			}
			debugData.Add(string.Format("selected gear: {0} - {1}", selectedGear, minRating));

			if (selectedGear < gear && (DownshiftAllowed(absTime) || inAngularVelocity < GetEngineSpeedLimitLow(false))) {
				_nextGear = selectedGear;
			}
			if (selectedGear > gear && (UpshiftAllowed(absTime) || inAngularVelocity > minRating.MaxEngineSpeed)) {
				_nextGear = selectedGear;
			}
			
			DebugData.Add(debugData);
			return _nextGear != gear;
		}

		private bool UpshiftAllowed(Second absTime)
		{
			return (absTime - _gearbox.LastDownshift).IsSmaller(_gearbox.ModelData.UpshiftAfterDownshiftDelay);
		}

		private bool DownshiftAllowed(Second absTime)
		{
			return (absTime - _gearbox.LastUpshift).IsSmaller(_gearbox.ModelData.DownshiftAfterUpshiftDelay);
		}

		private GearRating RatingGear(bool driveOff, uint gear, Second dt)
		{
			var currentVelocity = DataBus.VehicleSpeed;
			var gradient = CalcGradientDuringGearshift(driveOff, dt, currentVelocity);

			var predictionVelocity = ShiftStrategyParameters.StartVelocity;
			var accRsv = ShiftStrategyParameters.StartAcceleration;
			if (!driveOff) {
				predictionVelocity = CalcPredictionVelocity(currentVelocity, gradient);

				accRsv = CalcAccelerationReserve(currentVelocity);
			}

			TestContainer.GearboxCtl.Gear = gear;
			TestContainer.VehiclePort.Initialize(predictionVelocity, gradient);
			var respAccRsv = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				accRsv, gradient, true);
			var respConstVel = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				0.SI<MeterPerSquareSecond>(), gradient, true);

			if (respAccRsv.EngineSpeed < PowertrainConfig.EngineData.IdleSpeed ||
				respAccRsv.EngineSpeed > PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed) {
				return new GearRating(GearRatingCase.E, 0, 0.RPMtoRad());
			}


			GearRating? retVal;
			var engineSpeedLowThreshold = GetEngineSpeedLimitLow(driveOff);
			var engineSpeedHighThreshold = GetEngineSpeedLimitHigh(driveOff, gear, respAccRsv.EngineSpeed, respConstVel.CardanTorque);
			if (respAccRsv.EngineSpeed < engineSpeedLowThreshold) {
				return new GearRating(GearRatingCase.D, (engineSpeedLowThreshold - respAccRsv.EngineSpeed).Value(), engineSpeedHighThreshold);
			}

			if (respAccRsv.EngineSpeed > engineSpeedHighThreshold) {
				return new GearRating(GearRatingCase.D, (respAccRsv.EngineSpeed - engineSpeedHighThreshold).Value(), engineSpeedHighThreshold);
			}

			if (respAccRsv.EngineTorqueDemandTotal <= respAccRsv.EngineDynamicFullLoadTorque) {
				var fc = PowertrainConfig.EngineData.ConsumptionMap.GetFuelConsumption(
					VectoMath.Max(respAccRsv.EngineTorqueDemandTotal, PowertrainConfig.EngineData.FullLoadCurves[0].DragLoadStationaryTorque(respAccRsv.EngineSpeed)), respAccRsv.EngineSpeed);
				retVal = new GearRating(GearRatingCase.A, (fc.Value / respAccRsv.AxlegearPowerRequest).Value(), engineSpeedHighThreshold);
			} else {
				retVal = new GearRating(GearRatingCase.B, (respAccRsv.EnginePowerRequest - respAccRsv.DynamicFullLoadPower).Value(), engineSpeedHighThreshold);
			}

			var estimatedResidenceTime = EstimateResidenceTimeInGear(gear, respAccRsv, engineSpeedHighThreshold, gradient);
			if (estimatedResidenceTime < ShiftStrategyParameters.GearResidenceTime) {
				retVal = new GearRating(
					GearRatingCase.C, (ShiftStrategyParameters.GearResidenceTime - estimatedResidenceTime).Value(), engineSpeedHighThreshold);
			}

			return retVal.Value;
		}

		private Radian CalcGradientDuringGearshift(bool driveOff, Second dt, MeterPerSecond currentVelocity)
		{
			var lookaheadMidShift = driveOff
				? ShiftStrategyParameters.StartVelocity /
				ShiftStrategyParameters.StartAcceleration / 2.0 * ShiftStrategyParameters.StartVelocity
				: currentVelocity * dt / 2.0;
			var currentAltitude = DataBus.Altitude;
			var lookAheadPos = DataBus.CycleLookAhead(lookaheadMidShift);
			var gradient = VectoMath.InclinationToAngle((lookAheadPos.Altitude - currentAltitude) / lookaheadMidShift);
			return gradient;
		}

		private MeterPerSquareSecond CalcAccelerationReserve(MeterPerSecond currentVelocity)
		{
			var targetSpeed = DataBus.CycleData.LeftSample.VehicleTargetSpeed;
			var accRsvLow = ShiftStrategyParameters.AccelerationReserveLookup.LookupLow(currentVelocity);
			var accRsvHigh = ShiftStrategyParameters.AccelerationReserveLookup.LookupHigh(currentVelocity);
			var targetSpeedDeviationLim = (targetSpeed - currentVelocity).LimitTo(
				0.KMPHtoMeterPerSecond(), targetSpeed * ShiftStrategyParameters.TargetSpeedDeviationFactor);
			var accRsv = VectoMath.Interpolate(
				0.KMPHtoMeterPerSecond(),
				VectoMath.Max(targetSpeed * ShiftStrategyParameters.TargetSpeedDeviationFactor, 0.001.SI<MeterPerSecond>()),
				accRsvLow, accRsvHigh,
				targetSpeedDeviationLim);
			return accRsv;
		}

		private MeterPerSecond CalcPredictionVelocity(MeterPerSecond currentVelocity, Radian gradient)
		{
			var estimatedVelocityPostShift = VelocityDropData.Interpolate(currentVelocity, gradient);
			var ratioSpeedDrop = estimatedVelocityPostShift / VectoMath.Max(currentVelocity, 0.1.SI<MeterPerSecond>());
			var predictionIntervalRatio = ShiftStrategyParameters.PredictionDurationLookup.Lookup(ratioSpeedDrop.Value());
			var speedChange = estimatedVelocityPostShift - currentVelocity;
			return currentVelocity + speedChange * predictionIntervalRatio;
		}

		private PerSecond GetEngineSpeedLimitHigh(bool driveOff, uint gear, PerSecond engineSpeed, NewtonMeter cardanTorque)
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

		private bool CoastingBrakingShiftDecision(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var upperEngineSpeedLimit = (PowertrainConfig.EngineData.FullLoadCurves[0].NTq99lSpeed +
										PowertrainConfig.EngineData.FullLoadCurves[0].NTq99hSpeed) / 2.0;
			if (inAngularVelocity < GetEngineSpeedLimitLow(false)) {
				for (var i = Math.Max(0, gear - ShiftStrategyParameters.AllowedGearRangeDown);
					i <= Math.Min(ModelData.Gears.Count, gear + ShiftStrategyParameters.AllowedGearRangeUp);
					i++) {
					var nextGear = (uint)i;
					if (outAngularVelocity * ModelData.Gears[nextGear].Ratio > GetEngineSpeedLimitLow(false) &&
						outAngularVelocity * ModelData.Gears[nextGear].Ratio < upperEngineSpeedLimit) {
						_nextGear = nextGear;
						return true;
					}
				}
			}

			if (inAngularVelocity >= upperEngineSpeedLimit) {
				for (var i = Math.Min(ModelData.Gears.Count, gear + ShiftStrategyParameters.AllowedGearRangeUp);
					i >= Math.Max(0, gear + ShiftStrategyParameters.AllowedGearRangeDown);
					i--) {
					var nextGear = (uint)i;
					if (outAngularVelocity * ModelData.Gears[nextGear].Ratio > GetEngineSpeedLimitLow(false) &&
						outAngularVelocity * ModelData.Gears[nextGear].Ratio < upperEngineSpeedLimit) {
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
			var oldEntries = HistoryBuffer.Keys.Where(x => x < absTime - ShiftStrategyParameters.LookBackInterval).ToArray();
			foreach (var entry in oldEntries) {
				HistoryBuffer.Remove(entry);
			}
		}

		public override uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(torque, outAngularVelocity);
			}

			for (var gear = (uint)ModelData.Gears.Count; gear > 1; gear--) {
				var inAngularVelocity = outAngularVelocity * ModelData.Gears[gear].Ratio;
				if (DataBus.EngineSpeed < inAngularVelocity && inAngularVelocity < DataBus.EngineRatedSpeed) {
					_nextGear = gear;
					return gear;
				}
			}

			return 1;
		}

		private uint InitStartGear(NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var maxStartGear = (int)Math.Round(ModelData.Gears.Count / 2.0, MidpointRounding.AwayFromZero);

			var startGear = 1u;
			var minRating = new GearRating(GearRatingCase.D, double.MaxValue, 0.RPMtoRad());
			var roadGradient = DataBus.CycleData.LeftSample.RoadGradient; //CycleLookAhead(0.SI<Meter>()).RoadGradient;
			for (uint i = (uint)maxStartGear; i > 0; i--) {
				var gradientBelowMaxGrad = roadGradient < MaxGradability.GradabilityLimitedTorque(i);
				var engineSpeedLimitLow = GetEngineSpeedLimitLow(true);
				var engineSpeed = GetEngineSpeed(i);
				var engineSpeedAboveMin = engineSpeed > engineSpeedLimitLow;
				var engineSpeedBelowN95h = engineSpeed < PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed;
				if (gradientBelowMaxGrad && engineSpeedAboveMin && engineSpeedBelowN95h) {
					var rating = RatingGear(true, i, null);
					if (rating < minRating) {
						minRating = rating;
						startGear = i;
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
				: ShiftStrategyParameters.ShareIdleLow.Lookup(DataBus.VehicleSpeed);
			return PowertrainConfig.EngineData.IdleSpeed + shareIdleLowMax *
					(PowertrainConfig.EngineData.FullLoadCurves[0].NP99hSpeed - PowertrainConfig.EngineData.IdleSpeed);
		}


		private Second EstimateResidenceTimeInGear(uint gear, ResponseDryRun responseDriverDemand, PerSecond engineSpeedHighThreshold, Radian estimatedGradient)
		{
			// get total 'transmission ratio' of powertrain
			var engineSpeed = responseDriverDemand.EngineSpeed;
			var vehicleSpeed = responseDriverDemand.VehicleSpeed;
			var ratio = (vehicleSpeed / engineSpeed).Cast<Meter>();

			var averageAccelerationTorque = AverageAccelerationTorqueLookup.Interpolate(
				responseDriverDemand.EngineSpeed, responseDriverDemand.EngineTorqueDemand);

			var initResponse = TestContainer.VehiclePort.Initialize(vehicleSpeed, estimatedGradient);
			var delta = initResponse.EngineTorqueDemand - averageAccelerationTorque;
			var acceleration = SearchAlgorithm.Search(
				0.SI<MeterPerSquareSecond>(), delta, 0.1.SI<MeterPerSquareSecond>(),
				getYValue: r => {
					return (r as AbstractResponse).EngineTorqueDemand - averageAccelerationTorque;
				},
				evaluateFunction: a => {
					return TestContainer.VehiclePort.Request(
						0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval, a, estimatedGradient, true);
				},
				criterion: r => {
					return ((r as AbstractResponse).EngineTorqueDemand - averageAccelerationTorque).Value();
				}
			);
			
			var engineAcceleration = (acceleration / ratio).Cast<PerSquareSecond>();
			var deltaEngineSpeed = engineSpeedHighThreshold - engineSpeed;
			if (engineAcceleration > 0 && deltaEngineSpeed > 0) {
				return (deltaEngineSpeed / engineAcceleration).Cast<Second>();
			}

			// TODO: make similar to matlab implementtion
			return 0.SI<Second>();
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


		public override uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			return _nextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed) { }

		public override GearInfo NextGear
		{
			get { return new GearInfo(_nextGear, true); }
		}

		#endregion
	}

	internal struct GearRating
	{
		public GearRating(GearRatingCase ratingCase, double rating, PerSecond maxEngineSpeed)
		{
			RatingCase = ratingCase;
			Rating = rating;
			MaxEngineSpeed = maxEngineSpeed;
		}

		public double Rating { get; }

		public GearRatingCase RatingCase { get; }
		public PerSecond MaxEngineSpeed { get; set; }

		public static bool operator <(GearRating first, GearRating second)
		{
			return first.RatingCase < second.RatingCase && first.Rating < second.Rating;
		}

		public static bool operator >(GearRating first, GearRating second)
		{
			return first.RatingCase > second.RatingCase && first.Rating > second.Rating;
		}

		public override string ToString()
		{
			return string.Format("{0} / {1}", RatingCase, Rating);
		}
	}

	internal enum GearRatingCase
	{
		A = 1,	// inside preferred speed range and inside torque range
		B,		// inside engine speed limits, torque demand too high
		C,		// valid gear, gear residience time below threshold
		D,		// inside engine speed limits, outside preferred speed range
		E		// outside engine speed limits
	}
}
