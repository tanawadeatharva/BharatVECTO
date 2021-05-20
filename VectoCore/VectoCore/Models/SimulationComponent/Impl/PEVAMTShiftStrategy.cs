using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.ElectricMotor;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{

	public class PEVAMTShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected readonly IDataBus DataBus;
		protected readonly GearboxData GearboxModelData;

		protected Gearbox _gearbox;
		protected GearshiftPosition _nextGear;

		private ShiftStrategyParameters shiftStrategyParameters;
		protected readonly VelocityRollingLookup VelocityDropData = new VelocityRollingLookup();
		private SimplePowertrainContainer TestContainer;
		private Gearbox TestContainerGbx;
		private Kilogram vehicleMass;
		private EfficiencyMap PowerMap;
		private ElectricMotorFullLoadCurve FullLoadCurve;
		private SI TransmissionRatio;
		private ShiftStrategyParameters GearshiftParams;
		private GearList GearList;

		public bool EarlyShiftUp { get; }

		public bool SkipGears { get; }

		public static string Name {
			get { return "AMT - EffShift (BEV)"; }
		}


		public PEVAMTShiftStrategy(IVehicleContainer dataBus)
		{
			var runData = dataBus.RunData;
			if (runData.VehicleData == null) {
				return;
			}
			GearboxModelData = dataBus.RunData.GearboxData;
			GearshiftParams = dataBus.RunData.GearshiftParameters;
			GearList = GearboxModelData.GearList;
			MaxStartGear = GearList.Reverse().First();

			PowerMap = dataBus.RunData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricE2)?.Item2.EfficiencyMap;
			FullLoadCurve = dataBus.RunData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricE2)?.Item2.FullLoadCurve;
			DataBus = dataBus;

			EarlyShiftUp = true;
			SkipGears = true;

			TransmissionRatio = runData.AxleGearData.AxleGear.Ratio *
									(runData.AngledriveData == null ? 1.0 : runData.AngledriveData.Angledrive.Ratio) /
									runData.VehicleData.DynamicTyreRadius;
			//var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
			//	Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;

			shiftStrategyParameters = runData.GearshiftParameters;
			vehicleMass = runData.VehicleData.TotalVehicleMass;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}
			SetupVelocityDropPreprocessor(dataBus);
		}

		private void SetupVelocityDropPreprocessor(IVehicleContainer dataBus)
		{
			var runData = dataBus.RunData;
			// MQ: 2019-11-29 - fuel used here has no effect as this is the modDatacontainer for the test-powertrain only!
			var modData = new ModalDataContainer(runData, null, null);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimplePowertrainE2(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			// register pre-processors
			var maxG = runData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var grad = Convert.ToInt32(maxG / 2) * 2;
			if (grad == 0) {
				grad = 2;
			}

			dataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessorE2(VelocityDropData, runData.GearboxData.TractionInterruption, TestContainer, -grad, grad, 2));

		}

		public bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque,
			PerSecond inAngularVelocity, GearshiftPosition gear, Second lastShiftTime, IResponse response)
		{
			CheckGearshiftRequired = true;
			var retVal = DoCheckShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity,
				gear, lastShiftTime, response);
			CheckGearshiftRequired = false;
			return retVal;
		}

		private bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			// no shift when vehicle stands
			if (DataBus.VehicleInfo.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			while (GearList.HasSuccessor(_nextGear) &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = GearList.Successor(_nextGear);
			}
			if (_nextGear != gear) {
				return true;
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed = (lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, response);
			if (_nextGear != gear) {
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear, response);

			//if ((ModelData.Gears[_nextGear].Ratio * outAngularVelocity - DataBus.EngineIdleSpeed) /
			//	(DataBus.EngineRatedSpeed - DataBus.EngineIdleSpeed) <
			//	Constants.SimulationSettings.ClutchClosingSpeedNorm && _nextGear > 1) {
			//	_nextGear--;
			//}

			return _nextGear != gear;
		}

		protected virtual GearshiftPosition CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(GearshiftParams.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
			if (nextGear.Equals(currentGear)) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(GearshiftParams.UpshiftMinAcceleration)) {
				// if less than 0.1 for next gear, don't shift
				if (GearList.Distance(nextGear, currentGear) == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear > currentGear &&
					EstimateAccelerationForGear(GearList.Successor(currentGear), outAngularVelocity)
						.IsSmaller(GearshiftParams.UpshiftMinAcceleration)) {
					return currentGear;
				}
				nextGear = GearList.Successor(currentGear);
			}

			return nextGear;
		}

		protected virtual GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity,
			GearshiftPosition currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = GearList.Successor(currentGear);

				while (SkipGears && GearList.HasSuccessor(currentGear)) {
					currentGear = GearList.Successor(currentGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity =
						response.ElectricMotor
							.AngularVelocity; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(-response.ElectricMotor.MaxDriveTorque,
						!currentGear.Equals(GearList.First())
							? GearboxModelData.Gears[currentGear.Gear].ShiftPolygon
								.InterpolateDownshift(response.Engine.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ &&
						IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear = GearList.Predecessor(currentGear);
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (EarlyShiftUp && GearList.HasSuccessor(currentGear)) {
				currentGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
			}

			return currentGear;
		}

		protected virtual GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			//var minFcGear = currentGear;
			//var minFc = double.MaxValue;
			//IResponse minFCResponse = null;
			//var fcCurrent = double.NaN;

			//var fcUpshiftPossible = true;

			// no eff-shift if torque demand is close to ICE drag load
			//if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
			//	return currentGear;
			//}

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
			var vehicleSpeedPostShift = DataBus.VehicleInfo.VehicleSpeed - vDrop * shiftStrategyParameters.VelocityDropFactor;

			var totalTransmissionRatio = DataBus.ElectricMotorInfo(PowertrainPosition.BatteryElectricE2).ElectricMotorSpeed / DataBus.VehicleInfo.VehicleSpeed;
			//var totalTransmissionRatio = outAngularVelocity / DataBus.VehicleSpeed;

			var results = new List<Tuple<GearshiftPosition, double>>();
			foreach (var tryNextGear in GearList.IterateGears(GearList.Successor(currentGear), GearList.Successor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				
				//if (tryNextGear > GearboxModelData.Gears.Keys.Max() 
				//	/*|| !(ModelData.Gears[tryNextGear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)*/) {
				//	continue;
				//}

				//fcUpshiftPossible = true;

				//var response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration, tryNextGear);
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				var estimatedEngineSpeed = (vehicleSpeedPostShift * (totalTransmissionRatio / GearboxModelData.Gears[currentGear.Gear].Ratio * GearboxModelData.Gears[tryNextGear.Gear].Ratio)).Cast<PerSecond>();
				if (estimatedEngineSpeed.IsSmaller(shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				//var pNextGearMax = DataBus.Engine Info.EngineStationaryFullPower(estimatedEngineSpeed);

				//if (!response.Engine.PowerRequest.IsSmaller(pNextGearMax)) {
				//	continue;
				//}

				var fullLoadPower = -response.ElectricMotor.MaxDriveTorque * response.ElectricMotor.AngularVelocity;
				var reserve = 1 - response.ElectricMotor.PowerRequest / fullLoadPower;

				//var reserve = 1 - response.EngineTorqueDemandTotal / response.EngineStationaryFullLoadTorque;

				
				//if (double.IsNaN(fcCurrent)) {
				//	//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
				//	var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
				//	fcCurrent = GetFCRating(responseCurrent);
				//}
				
				var fcNext = GetFCRating(response);
				results.Add(Tuple.Create(tryNextGear, fcNext));

				//if (reserve < GearshiftParams.TorqueReserve ||
				//	!fcNext.IsGreater(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
				//	continue;
				//}

				//minFcGear = tryNextGear;
				//minFc = fcNext;
				//minFCResponse = response;
			}

			if (results.Count == 0) {
				return currentGear;
			}

			var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
			var fcCurrent = GetFCRating(responseCurrent);

			var minFc = results.MinBy(x => x.Item2);

			var ratingFactor = outTorque < 0
				? 1 / shiftStrategyParameters.RatingFactorCurrentGear
				: shiftStrategyParameters.RatingFactorCurrentGear;

			if (minFc.Item2.IsGreater(fcCurrent * ratingFactor)) {
				return minFc.Item1;
			}

			return currentGear;
			//return fcUpshiftPossible
			//	? currentGear
			//	: base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
		}

		
		private double GetFCRating(ResponseDryRun response)//PerSecond engineSpeed, NewtonMeter tqCurrent)
		{
			var currentGear = response.Gearbox.Gear;

			var maxGenTorque = VectoMath.Min(GearboxModelData.Gears[currentGear.Gear].MaxTorque, response.ElectricMotor.MaxRecuperationTorque);
			var maxDriveTorque = GearboxModelData.Gears[currentGear.Gear].MaxTorque != null
				? VectoMath.Max(-GearboxModelData.Gears[currentGear.Gear].MaxTorque, response.ElectricMotor.MaxDriveTorque)
				: response.ElectricMotor.MaxDriveTorque;

			var tqCurrent = (response.ElectricMotor.ElectricMotorPowerMech / response.ElectricMotor.AngularVelocity);
			if (!tqCurrent.IsBetween(maxDriveTorque, maxGenTorque)) {
				return double.NaN;
			}
			var engineSpeed = response.ElectricMotor.AngularVelocity;

			
			var fcCurRes = PowerMap.LookupElectricPower(engineSpeed, tqCurrent, true);
			if (fcCurRes.Extrapolated) {
				Log.Warn(
					"EffShift Strategy: Extrapolation of power consumption for current gear! n: {0}, Tq: {1}",
					engineSpeed, tqCurrent);
			}
			return fcCurRes.ElectricalPower.Value();
		}




		protected ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition tryNextGear)
		{
			LogEnabled = false;
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = tryNextGear;

			TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			LogEnabled = true;
			return response;
		}

		protected MeterPerSquareSecond EstimateAccelerationForGear(GearshiftPosition gear, PerSecond gbxAngularVelocityOut)
		{
			if (!gear.Engaged || !GearList.Contains(gear)) {
				throw new VectoSimulationException("EstimateAccelerationForGear: invalid gear: {0}", gear);
			}

			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;

			var nextEngineSpeed = gbxAngularVelocityOut * GearboxModelData.Gears[gear.Gear].Ratio;
			var maxEnginePower = -(FullLoadCurve.FullLoadDriveTorque(nextEngineSpeed) * nextEngineSpeed);
			
			var avgSlope =
				((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
				DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var gearboxLoss = GearboxModelData.Gears[gear.Gear].LossMap.GetTorqueLoss(gbxAngularVelocityOut,
				maxEnginePower / nextEngineSpeed * GearboxModelData.Gears[gear.Gear].Ratio).Value * nextEngineSpeed;
			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			var accelerationPower = maxEnginePower - gearboxLoss - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleInfo.VehicleSpeed / (DataBus.VehicleInfo.TotalMass + DataBus.WheelsInfo.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}

		protected virtual GearshiftPosition CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(GearshiftParams.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
		}

		protected virtual GearshiftPosition DoCheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			var nextGear = BaseDoCheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);

			if (nextGear.Equals(currentGear) && !currentGear.Equals(GearList.First())) {
				nextGear = CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
			}
			return nextGear;
		}

		protected virtual GearshiftPosition BaseDoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = GearList.Predecessor(currentGear);
				//while (SkipGears && currentGear > 1) {
				//	currentGear--;
				//	var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

				//	inAngularVelocity = ModelData.Gears[currentGear].Ratio * outAngularVelocity;
				//	inTorque = response.ClutchPowerRequest / inAngularVelocity;
				//	var maxTorque = VectoMath.Min(response.DynamicFullLoadPower / ((DataBus.EngineSpeed + response.EngineSpeed) / 2),
				//		currentGear > 1
				//			? ModelData.Gears[currentGear].ShiftPolygon.InterpolateDownshift(response.EngineSpeed)
				//			: double.MaxValue.SI<NewtonMeter>());
				//	var reserve = maxTorque.IsEqual(0) ? -1 : (1 - inTorque / maxTorque).Value();
				//	if (reserve >= ModelData.TorqueReserve && IsBelowUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				//		continue;
				//	}
				//	currentGear++;
				//	break;
				//}
			}
			return currentGear;
		}


		protected virtual GearshiftPosition CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			//var minFcGear = currentGear;
			//var minFc = double.MaxValue * Math.Sign(outTorque.Value()) ;
			//var fcCurrent = double.NaN;

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			// no eff-shift if torque demand is close to ICE drag load
			//if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
			//	return currentGear;
			//}

			var results = new List<Tuple<GearshiftPosition, double>>();
			foreach (var tryNextGear in GearList.IterateGears(GearList.Predecessor(currentGear), GearList.Predecessor(currentGear, (uint)shiftStrategyParameters.AllowedGearRangeFC))) {
				
				//if (tryNextGear < 1 /*|| !(ModelData.Gears[tryNextGear].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC)*/) {
				//	continue;
				//}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				//var response = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, tryNextGear);

				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				//if (double.IsNaN(fcCurrent)) {
				//	var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

				//	//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
				//	fcCurrent = GetFCRating(responseCurrent);
				//}
				var fcNext = GetFCRating(response);
				results.Add(Tuple.Create(tryNextGear,fcNext));

				//if (!fcNext.IsGreater(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) ||
				//	!fcNext.IsGreater(minFc)) {
				//	continue;
				//}

				//minFcGear = tryNextGear;
				//minFc = fcNext;
			}

			if (results.Count == 0) {
				return currentGear;
			}

			var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
			var fcCurrent = GetFCRating(responseCurrent);

			var minFc = results.MinBy(x => x.Item2);

			var ratingFactor = outTorque < 0
				? 1 / shiftStrategyParameters.RatingFactorCurrentGear
				: shiftStrategyParameters.RatingFactorCurrentGear;


			if (minFc.Item2.IsGreater(fcCurrent * ratingFactor)) {
				return minFc.Item1;
			}
			
			return currentGear;
		}


		public GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				return InitStartGear(absTime, outTorque, outAngularVelocity);
			}

			foreach (var gear in GearList.Reverse()) {
				//for (var gear = (uint)GearboxModelData.Gears.Count; gear > 1; gear--) {
				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed)) {
					_nextGear = gear;
					return gear;
				}
			}
			// fallback: return first gear
			_nextGear = GearList.First();
			return _nextGear;
		}

		private GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			var emSpeeds = new Dictionary<GearshiftPosition, Tuple<PerSecond, PerSecond>>();

			foreach (var gear in GearList.Reverse()) {
				//for (var gear = (uint)GearboxModelData.Gears.Count; gear >= 1; gear--) {
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = FullLoadCurve.MaxSpeed * 0.9;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var fullLoadPower = -(response.ElectricMotor.MaxDriveTorque * response.ElectricMotor.AngularVelocity);
					//.DynamicFullLoadPower; //EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.ElectricMotor.PowerRequest / fullLoadPower;

				if (reserve >= GearshiftParams.StartTorqueReserve) {
					//_nextGear = gear;
					//return gear;
					emSpeeds[gear] = Tuple.Create(response.ElectricMotor.AngularVelocity,
						(GearshiftParams.StartSpeed * TransmissionRatio * GearboxModelData.Gears[gear.Gear].Ratio)
						.Cast<PerSecond>());
				}
			}

			if (emSpeeds.Any()) {
				var optimum = emSpeeds.MaxBy(x => x.Key.Gear); //x => VectoMath.Abs(x.Value.Item2 - FullLoadCurve.MaxSpeed * 0.5));
				_nextGear = optimum.Key;
				return _nextGear;
			}
			_nextGear = GearList.First();
			return _nextGear;
		}

		protected bool IsBelowDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!GearList.HasPredecessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		protected bool IsAboveDownShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!GearList.HasPredecessor(gear)) {
				return true;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsAboveDownshiftCurve(inTorque, inEngineSpeed);
		}

		protected bool IsAboveUpShiftCurve(GearshiftPosition gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (!GearList.HasSuccessor(gear)) {
				return false;
			}
			return GearboxModelData.Gears[gear.Gear].ShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		public GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			
			while (GearList.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = GearList.Successor(_nextGear);
			}

			return _nextGear;
		}

		
		protected bool SpeedTooHighForEngine(GearshiftPosition gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * GearboxModelData.Gears[gear.Gear].Ratio).IsGreaterOrEqual(VectoMath.Min(GearboxModelData.Gears[gear.Gear].MaxSpeed,
					DataBus.ElectricMotorInfo(PowertrainPosition.BatteryElectricE2).MaxSpeed));
		}

		public void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public IGearbox Gearbox {
			get { return _gearbox; }
			set {
				var myGearbox = value as Gearbox;
				if (myGearbox == null) {
					throw new VectoException("This shift strategy can't handle gearbox of type {0}", value.GetType());
				}
				_gearbox = myGearbox;
			}
		}

		public GearshiftPosition NextGear {
			get { return _nextGear; }
		}


		public bool CheckGearshiftRequired { get; protected set; }
		public GearshiftPosition MaxStartGear { get; }

		public void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public void WriteModalResults(IModalDataContainer container) { }

		public ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, 
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData)
		{
			return DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i, electricMotorData.FullLoadCurve, electricMotorData.RatioADC, gearboxGears, axlegearRatio, dynamicTyreRadius);
		}

	}
}