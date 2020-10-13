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
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class PEVAMTShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected readonly IDataBus DataBus;
		protected readonly GearboxData ModelData;

		protected Gearbox _gearbox;
		protected uint _nextGear;

		private ShiftStrategyParameters shiftStrategyParameters;
		protected readonly VelocityRollingLookup VelocityDropData = new VelocityRollingLookup();
		private SimplePowertrainContainer TestContainer;
		private Gearbox TestContainerGbx;
		private Kilogram vehicleMass;
		private EfficiencyMap PowerMap;
		private ElectricMotorFullLoadCurve FullLoadCurve;

		public bool EarlyShiftUp { get; }

		public bool SkipGears { get; }


		public PEVAMTShiftStrategy(IVehicleContainer dataBus)
		{
			var runData = dataBus.RunData;
			ModelData = dataBus.RunData.GearboxData;
			PowerMap = dataBus.RunData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricB2)?.Item2.EfficiencyMap;
			FullLoadCurve = dataBus.RunData.ElectricMachinesData
				.FirstOrDefault(x => x.Item1 == PowertrainPosition.BatteryElectricB2)?.Item2.FullLoadCurve;
			DataBus = dataBus;

			EarlyShiftUp = true;
			SkipGears = true;

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
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime, IResponse response)
		{
			CheckGearshiftRequired = true;
			var retVal = DoCheckShiftRequired(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity,
				gear, lastShiftTime, response);
			CheckGearshiftRequired = false;
			return retVal;
		}

		private bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint gear,
			Second lastShiftTime, IResponse response)
		{
			// no shift when vehicle stands
			if (DataBus.VehicleInfo.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			while (_nextGear < ModelData.Gears.Count &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / ModelData.Gears[gear].Ratio)) {
				_nextGear++;
			}
			if (_nextGear != gear) {
				return true;
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed = (lastShiftTime + ModelData.ShiftTime).IsSmallerOrEqual(absTime);
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

		protected virtual uint CheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear, IResponse response)
		{
			// if the driver's intention is _not_ to accelerate or drive along then don't upshift
			if (DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Accelerating && DataBus.DriverInfo.DriverBehavior != DrivingBehavior.Driving) {
				return currentGear;
			}
			if ((absTime - _gearbox.LastDownshift).IsSmaller(_gearbox.ModelData.UpshiftAfterDownshiftDelay)) {
				return currentGear;
			}
			var nextGear = DoCheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
			if (nextGear == currentGear) {
				return nextGear;
			}

			// estimate acceleration for selected gear
			if (EstimateAccelerationForGear(nextGear, outAngularVelocity).IsSmaller(_gearbox.ModelData.UpshiftMinAcceleration)) {
				// if less than 0.1 for next gear, don't shift
				if (nextGear - currentGear == 1) {
					return currentGear;
				}
				// if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
				if (nextGear > currentGear &&
					EstimateAccelerationForGear(currentGear + 1, outAngularVelocity)
						.IsSmaller(_gearbox.ModelData.UpshiftMinAcceleration)) {
					return currentGear;
				}
				nextGear = currentGear + 1;
			}

			return nextGear;
		}

		protected virtual uint DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear++;

				while (SkipGears && currentGear < ModelData.Gears.Count) {
					currentGear++;
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity = response.ElectricMotor.AngularVelocity; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(response.ElectricMotor.MaxDriveTorque,
						currentGear > 1
							? ModelData.Gears[currentGear].ShiftPolygon.InterpolateDownshift(response.Engine.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= 0 /*ModelData.TorqueReserve */ && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear--;
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (EarlyShiftUp && currentGear < ModelData.Gears.Count) {
				currentGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
			}
			return currentGear;
		}

		protected virtual uint CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			IResponse minFCResponse = null;
			var fcCurrent = double.NaN;

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

			var totalTransmissionRatio = DataBus.ElectricMotorInfo(PowertrainPosition.BatteryElectricB2).ElectricMotorSpeed / DataBus.VehicleInfo.VehicleSpeed;
			//var totalTransmissionRatio = outAngularVelocity / DataBus.VehicleSpeed;

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
				var tryNextGear = (uint)(currentGear + i);

				if (tryNextGear > ModelData.Gears.Keys.Max() ||
					!(ModelData.Gears[tryNextGear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				//fcUpshiftPossible = true;

				//var response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration, tryNextGear);
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				var estimatedEngineSpeed = (vehicleSpeedPostShift * (totalTransmissionRatio / ModelData.Gears[currentGear].Ratio * ModelData.Gears[tryNextGear].Ratio)).Cast<PerSecond>();
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

				
				if (double.IsNaN(fcCurrent)) {
					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
					fcCurrent = GetFCRating(responseCurrent);
				}
				
				var fcNext = GetFCRating(response);

				if (reserve < ModelData.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
				minFCResponse = response;
			}

			if (currentGear != minFcGear) {
				return minFcGear;
			}

			return currentGear;
			//return fcUpshiftPossible
			//	? currentGear
			//	: base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
		}

		
		private double GetFCRating(ResponseDryRun response)//PerSecond engineSpeed, NewtonMeter tqCurrent)
		{
			var currentGear = response.Gearbox.Gear;

			var maxGenTorque = VectoMath.Min(ModelData.Gears[currentGear].MaxTorque, response.ElectricMotor.MaxRecuperationTorque);
			var maxDriveTorque = ModelData.Gears[currentGear].MaxTorque != null
				? VectoMath.Max(-ModelData.Gears[currentGear].MaxTorque, response.ElectricMotor.MaxDriveTorque)
				: response.ElectricMotor.MaxDriveTorque;

			var tqCurrent = (response.ElectricMotor.PowerRequest / response.ElectricMotor.AngularVelocity).LimitTo(maxDriveTorque, maxGenTorque);
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
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint tryNextGear)
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

		protected MeterPerSquareSecond EstimateAccelerationForGear(uint gear, PerSecond gbxAngularVelocityOut)
		{
			if (gear == 0 || gear > ModelData.Gears.Count) {
				throw new VectoSimulationException("EstimateAccelerationForGear: invalid gear: {0}", gear);
			}

			var vehicleSpeed = DataBus.VehicleInfo.VehicleSpeed;

			var nextEngineSpeed = gbxAngularVelocityOut * ModelData.Gears[gear].Ratio;
			var maxEnginePower = -(FullLoadCurve.FullLoadDriveTorque(nextEngineSpeed) * nextEngineSpeed);
			
			var avgSlope =
				((DataBus.DrivingCycleInfo.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
				DataBus.DrivingCycleInfo.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.VehicleInfo.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleInfo.VehicleSpeed;
			var rollResistanceLoss = DataBus.VehicleInfo.RollingResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var gearboxLoss = ModelData.Gears[gear].LossMap.GetTorqueLoss(gbxAngularVelocityOut,
				maxEnginePower / nextEngineSpeed * ModelData.Gears[gear].Ratio).Value * nextEngineSpeed;
			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.VehicleInfo.SlopeResistance(avgSlope) * DataBus.VehicleInfo.VehicleSpeed;
			var axleLoss = DataBus.AxlegearInfo.AxlegearLoss();

			var accelerationPower = maxEnginePower - gearboxLoss - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleInfo.VehicleSpeed / (DataBus.VehicleInfo.TotalMass + DataBus.WheelsInfo.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}

		protected virtual uint CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear, IResponse response)
		{
			if ((absTime - _gearbox.LastUpshift).IsSmaller(_gearbox.ModelData.DownshiftAfterUpshiftDelay)) {
				return currentGear;
			}
			return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
		}

		protected virtual uint DoCheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear, IResponse response)
		{
			var nextGear = BaseDoCheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);

			if (nextGear == currentGear && currentGear > ModelData.Gears.Keys.Min()) {
				nextGear = CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
			}
			return nextGear;
		}

		protected virtual uint BaseDoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear, IResponse response)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear--;
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


		protected virtual uint CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			// no eff-shift if torque demand is close to ICE drag load
			//if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
			//	return currentGear;
			//}

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
				var tryNextGear = (uint)(currentGear - i);

				if (tryNextGear < 1 || !(ModelData.Gears[tryNextGear].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				//var response = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, tryNextGear);

				var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				if (double.IsNaN(fcCurrent)) {
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					fcCurrent = GetFCRating(responseCurrent);
				}
				var fcNext = GetFCRating(response);

				if (!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) ||
					!fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
			}

			return minFcGear;
		}


		public uint InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
				_nextGear = 1;
				return 1;
			}

			for (var gear = (uint)ModelData.Gears.Count; gear > 1; gear--) {
				var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

				var inAngularSpeed = outAngularVelocity * ModelData.Gears[gear].Ratio;
				var inTorque = response.ElectricMotor.PowerRequest / inAngularSpeed;

				// if in shift curve and torque reserve is provided: return the current gear
				if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed)) {
					_nextGear = gear;
					return gear;
				}
			}
			// fallback: return first gear
			_nextGear = 1;
			return 1;
		}

		

		protected bool IsBelowDownShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear <= 1) {
				return false;
			}
			return ModelData.Gears[gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inEngineSpeed);
		}

		protected bool IsAboveDownShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear <= 1) {
				return true;
			}
			return ModelData.Gears[gear].ShiftPolygon.IsAboveDownshiftCurve(inTorque, inEngineSpeed);
		}

		protected bool IsAboveUpShiftCurve(uint gear, NewtonMeter inTorque, PerSecond inEngineSpeed)
		{
			if (gear >= ModelData.Gears.Count) {
				return false;
			}
			return ModelData.Gears[gear].ShiftPolygon.IsAboveUpshiftCurve(inTorque, inEngineSpeed);
		}

		public uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			
			while (_nextGear < ModelData.Gears.Count && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear++;
			}

			return _nextGear;
		}

		
		protected bool SpeedTooHighForEngine(uint gear, PerSecond outAngularSpeed)
		{
			return
				(outAngularSpeed * ModelData.Gears[gear].Ratio).IsGreaterOrEqual(VectoMath.Min(ModelData.Gears[gear].MaxSpeed,
					DataBus.ElectricMotorInfo(PowertrainPosition.BatteryElectricB2).MaxSpeed));
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

		public GearInfo NextGear {
			get { return new GearInfo(_nextGear, false); }
		}


		public bool CheckGearshiftRequired { get; protected set; }

		public void Request(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

		public void WriteModalResults(IModalDataContainer container) { }

		public ShiftPolygon ComputeDeclarationShiftPolygon(GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears, CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius, ElectricMotorData electricMotorData)
		{
			return DeclarationData.Gearbox.ComputeElectricMotorShiftPolygon(i, electricMotorData.FullLoadCurve, gearboxGears, axlegearRatio, dynamicTyreRadius);
		}

	}
}