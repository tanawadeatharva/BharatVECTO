using System;
using System.Collections.Generic;
using System.Data;
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

		public struct HistoryEntry
		{
			public Second dt;
			public MeterPerSecond AvgSpeed;
			public Watt AvgCardanPower;
		}

		protected IModalDataContainer ModalData;

		public AMTShiftStrategyV2(VectoRunData data, IVehicleContainer dataBus) : base(data.GearboxData, dataBus)
		{
			ModalData = dataBus.ModalData;
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

			var propulsion = HistoryBuffer.All(x => x.Value.AvgSpeed > 0) ||
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
			var minRating = new GearRating(GearRatingCase.D, double.MaxValue);
			var selectedGear = gear;
			for (var i = Math.Max(0, gear - ShiftStrategyParameters.AllowedGearRangeDown);
				i <= Math.Min(ModelData.Gears.Count, gear + ShiftStrategyParameters.AllowedGearRangeUp);
				i++) {
				var nextGear = (uint)i;
				if (nextGear > gear) {
					var gradientBelowMaxGrad = roadGradient < MaxGradability.GradabilityLimitedTorque(nextGear);
					var engineSpeedAboveMin = outAngularVelocity * ModelData.Gears[nextGear].Ratio > GetEngineSpeedLimitLow(false);
					var engineSpeedBelowMax = outAngularVelocity * ModelData.Gears[nextGear].Ratio < PowertrainConfig.EngineData.FullLoadCurves[0].N95hSpeed;
					if (!(gradientBelowMaxGrad && engineSpeedAboveMin && engineSpeedBelowMax)) {
						continue;
					}

					var rating = RatingGear(false, nextGear, dt);
					if (rating < minRating) {
						minRating = rating;
						selectedGear = nextGear;
						_nextGear = nextGear;
					}
				}
			}

			return selectedGear != gear;
		}

		private GearRating RatingGear(bool startGear, uint gear, Second dt)
		{
			var currentVelocity = DataBus.VehicleSpeed;
			var lookaheadMidShift = startGear
				? ShiftStrategyParameters.StartVelocity /
				ShiftStrategyParameters.StartAcceleration / 2.0 * ShiftStrategyParameters.StartVelocity
				: currentVelocity * dt / 2.0;
			var currentAltitude = DataBus.Altitude;
			var lookAheadPos = DataBus.CycleLookAhead(lookaheadMidShift);
			var gradient = VectoMath.InclinationToAngle((lookAheadPos.Altitude - currentAltitude) / lookaheadMidShift);

			var predictionVelocity = ShiftStrategyParameters.StartVelocity;
			var accRsv = ShiftStrategyParameters.StartAcceleration;
			if (!startGear) {
				var estimatedVelocityPostShift = VelocityDropData.Interpolate(currentVelocity, gradient);
				var ratioSpeedDrop = estimatedVelocityPostShift / VectoMath.Max(currentVelocity, 0.1.SI<MeterPerSecond>());
				var predictionIntervalRatio = ShiftStrategyParameters.PredictionDurationLookup.Lookup(ratioSpeedDrop.Value());
				var speedChange = estimatedVelocityPostShift - currentVelocity;
				predictionVelocity = currentVelocity + speedChange * predictionIntervalRatio;

				var targetSpeed = DataBus.CycleData.LeftSample.VehicleTargetSpeed;
				var accRsvLow = ShiftStrategyParameters.AccelerationReserveLookup.LookupLow(currentVelocity);
				var accRsvHigh = ShiftStrategyParameters.AccelerationReserveLookup.LookupHigh(currentVelocity);
				var targetSpeedDeviationLim = (targetSpeed - currentVelocity).LimitTo(
					0.KMPHtoMeterPerSecond(), targetSpeed * ShiftStrategyParameters.TargetSpeedDeviationFactor);
				accRsv = VectoMath.Interpolate(
					0.KMPHtoMeterPerSecond(),
					VectoMath.Max(targetSpeed * ShiftStrategyParameters.TargetSpeedDeviationFactor, 0.001.SI<MeterPerSecond>()),
					accRsvLow, accRsvHigh,
					targetSpeedDeviationLim);
			}

			TestContainer.GearboxCtl.Gear = gear;
			TestContainer.VehiclePort.Initialize(predictionVelocity, gradient);
			var response = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), Constants.SimulationSettings.TargetTimeInterval,
				accRsv, gradient, true);

			GearRating? retVal = null;
			if (response.EngineTorqueDemandTotal <= response.EngineDynamicFullLoadTorque) {
				var fc = PowertrainConfig.EngineData.ConsumptionMap.GetFuelConsumption(
					response.EngineTorqueDemandTotal, response.EngineSpeed);
				retVal = new GearRating(GearRatingCase.A, (fc.Value / response.AxlegearPowerRequest).Value());
			} else {
				retVal = new GearRating(GearRatingCase.B, (response.EnginePowerRequest - response.DynamicFullLoadPower).Value());
			}

			var estimatedResidenceTime = EstimateResidenceTimeInGear(gear, response);
			if (estimatedResidenceTime < ShiftStrategyParameters.GearResidenceTime) {
				retVal = new GearRating(
					GearRatingCase.C, (ShiftStrategyParameters.GearResidenceTime - estimatedResidenceTime).Value());
			}

			return retVal.Value;
		}

		private bool CoastingBrakingShiftDecision(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime)
		{
			var upperEngineSpeedLimit = (PowertrainConfig.EngineData.FullLoadCurves[0].NTq99lSpeed +
										PowertrainConfig.EngineData.FullLoadCurves[0].NTq99lSpeed) / 2.0;
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
			var minRating = new GearRating(GearRatingCase.D, double.MaxValue);
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


		private Second EstimateResidenceTimeInGear(uint gear, ResponseDryRun response)
		{
			var engineSpeed = response.EngineSpeed;
			var vehicleSpeed = response.VehicleSpeed;
			var ratio = (vehicleSpeed / engineSpeed).Cast<Meter>();
			var vehicleAcceleration = PowertrainConfig.DriverData.AccelerationCurve.Lookup(vehicleSpeed).Acceleration;
			var engineAcceleration = (vehicleAcceleration / ratio).Cast<PerSquareSecond>();
			var deltaEngineSpeed = GetEngineSpeedLimitHighDriveOff(gear) - engineSpeed;
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
		public GearRating(GearRatingCase ratingCase, double rating)
		{
			RatingCase = ratingCase;
			Rating = rating;
		}

		public double Rating { get; }

		public GearRatingCase RatingCase { get; }

		public static bool operator <(GearRating first, GearRating second)
		{
			return first.RatingCase < second.RatingCase && first.Rating < second.Rating;
		}

		public static bool operator >(GearRating first, GearRating second)
		{
			return first.RatingCase > second.RatingCase && first.Rating > second.Rating;
		}
	}

	internal enum GearRatingCase
	{
		A = 1,
		B,
		C,
		D
	}
}
