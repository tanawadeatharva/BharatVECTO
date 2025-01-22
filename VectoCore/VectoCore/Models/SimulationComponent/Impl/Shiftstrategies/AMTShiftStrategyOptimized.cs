using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
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
using TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies;
using TUGraz.VectoCore.Models.SimulationComponent.Strategies;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyOptimized : BaseShiftStrategy<Gearbox> //AMTShiftStrategy
	{
		public const string Name = "AMT - EffShift";

		private List<CombustionEngineFuelData> fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters _shiftStrategyParameters;
		private ISimpleVehicleContainer TestContainer;
		private Gearbox TestContainerGbx;
		//private AccelerationCurveData accCurve;

		private Kilogram vehicleMass;


		protected GearshiftPosition _nextGear;
		protected Gearbox _gearbox;
        protected GearshiftPosition DesiredGearRoadsweeping;
		protected ITestPowertrain<Gearbox> TestPowertrain;

        public AMTShiftStrategyOptimized(IVehicleContainer container) : base(container)
		{
			var runData = container.RunData;

            _shiftStrategyParameters = runData.GearshiftParameters;
			if (runData.EngineData == null) {
				return;
			}

			DesiredGearRoadsweeping = runData.DriverData?.PTODriveRoadsweepingGear;

			var transmissionRatio = runData.AxleGearData.AxleGear.Ratio *
									(runData.AngledriveData?.Angledrive.Ratio ?? 1.0) /
									runData.VehicleData.DynamicTyreRadius;
			var minEngineSpeed = (runData.EngineData.FullLoadCurves[0].RatedSpeed - runData.EngineData.IdleSpeed) *
				Constants.SimulationSettings.ClutchClosingSpeedNorm + runData.EngineData.IdleSpeed;

            MaxStartGear = GearboxModelData.GearList.First();
			foreach (var gear in GearboxModelData.GearList.Reverse()) {
				var gearData = GearboxModelData.Gears[gear.Gear];
				if (GearshiftParams.StartSpeed * transmissionRatio * gearData.Ratio > minEngineSpeed) {
					MaxStartGear = gear;
					break;
				}
			}

            // create testcontainer
            var testContainer = PowertrainBuilder.BuildSimplePowertrain(runData);
			TestPowertrain = PowertrainBuilder.CreateTestPowertrain<Gearbox>(testContainer, DataBus);

            fcMap = runData.EngineData.Fuels;
			fld = runData.EngineData.FullLoadCurves;

			
			//accCurve = runData.DriverData.AccelerationCurve;
			vehicleMass = runData.VehicleData.TotalVehicleMass;
			if (_shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			SetupVelocityDropPreprocessor(container);

			if (_shiftStrategyParameters.AllowedGearRangeFC > 2 || _shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				_shiftStrategyParameters.AllowedGearRangeFC = _shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		private void SetupVelocityDropPreprocessor(IVehicleContainer dataBus)
		{
			var runData = dataBus.RunData;
			// MQ: 2019-11-29 - fuel used here has no effect as this is the modDatacontainer for the test-powertrain only!
			TestContainer = PowertrainBuilder.BuildSimplePowertrain(runData);
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
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption,
					TestContainer, -grad, grad, 2));
		}

		
		public override GearshiftPosition NextGear => _nextGear;

        public override GearshiftPosition InitGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
        {
            if (DataBus.VehicleInfo.VehicleSpeed.IsEqual(0)) {
                return InitStartGear(absTime, outTorque, outAngularVelocity);
            }

            foreach (var gear in Gears.Reverse()) {
                var selected = gear;
                //var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);

                TestPowertrain.UpdateComponents();
                TestPowertrain.Gearbox.Gear = gear;
                TestPowertrain.Gearbox._nextGear = gear;

                var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
                response = TestPowertrain.Gearbox.Request(absTime,
                    Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
                    true);

                var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;
                var fullLoadPower = TestPowertrain.CombustionEngine.EngineStationaryFullPower(response.Engine.EngineSpeed);
                var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
                var inTorque = response.Clutch.PowerRequest / inAngularSpeed;

                // if in shift curve and torque reserve is provided: return the current gear
                if (!IsBelowDownShiftCurve(gear, inTorque, inAngularSpeed) && !IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) &&
                    reserve >= GearshiftParams.StartTorqueReserve) {
                    if ((inAngularSpeed - DataBus.EngineInfo.EngineIdleSpeed) / (DataBus.EngineInfo.EngineRatedSpeed - DataBus.EngineInfo.EngineIdleSpeed) <
                        Constants.SimulationSettings.ClutchClosingSpeedNorm && Gears.HasPredecessor(gear)) {
                        selected = Gears.Predecessor(gear);
                    }
                    _nextGear = selected;
                    return selected;
                }

                // if over the up shift curve: return the previous gear (even thou it did not provide the required torque reserve)
                if (IsAboveUpShiftCurve(gear, inTorque, inAngularSpeed) && Gears.HasSuccessor(gear)) {
                    selected = Gears.Successor(gear);
                    _nextGear = selected;
                    return selected;
                }
            }

            // fallback: return first gear
            _nextGear = Gears.First();
            return _nextGear;
        }

		private GearshiftPosition InitStartGear(Second absTime, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			foreach (var gear in Gears.IterateGears(MaxStartGear, Gears.First())) {
				var inAngularSpeed = outAngularVelocity * GearboxModelData.Gears[gear.Gear].Ratio;

				var ratedSpeed = DataBus.EngineInfo.EngineRatedSpeed;
				if (inAngularSpeed > ratedSpeed || inAngularSpeed.IsEqual(0)) {
					continue;
				}

				//var response = _gearbox.Initialize(absTime, gear, outTorque, outAngularVelocity);
				TestPowertrain.UpdateComponents();
				TestPowertrain.Gearbox.Gear = gear;
				TestPowertrain.Gearbox._nextGear = gear;

				var response = TestPowertrain.Gearbox.Initialize(outTorque, outAngularVelocity);
				response = TestPowertrain.Gearbox.Request(absTime,
					Constants.SimulationSettings.MeasuredSpeedTargetTimeInterval, outTorque, outAngularVelocity,
					true);

				var reserve = 1 - response.Engine.TotalTorqueDemand / response.Engine.DynamicFullLoadTorque; //response.Engine.PowerRequest/response.Engine.DynamicFullLoadPower does not contain auxiliary power

				if (response.Engine.EngineSpeed > DataBus.EngineInfo.EngineIdleSpeed && reserve >= GearshiftParams.StartTorqueReserve) {
					_nextGear = gear;
					return gear;
				}
			}
			_nextGear = Gears.First();
			return _nextGear;
		}


        public override GearshiftPosition Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity)
		{
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}
			while (Gears.HasSuccessor(_nextGear) && SpeedTooHighForEngine(_nextGear, outAngularVelocity)) {
				_nextGear = Gears.Successor(_nextGear);
			}

			return _nextGear;
		}

		public override void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity) { }

        protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
			Second lastShiftTime, IResponse response)
		{
			// no shift when vehicle stands
			if (DataBus.VehicleInfo.VehicleStopped) {
				return false;
			}

			// emergency shift to not stall the engine ------------------------
			if (Gears.First().Equals(gear) &&
				SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				return true;
			}

			_nextGear = gear;
			while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear,
						inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Predecessor(_nextGear);
			}

			while (Gears.HasSuccessor(_nextGear) &&
					SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio)) {
				_nextGear = Gears.Successor(_nextGear);
			}

			if (!_nextGear.Equals(gear)) {
				return true;
			}

			// PTO Active while drive (roadsweeping) shift rules
			if (DataBus.DrivingCycleInfo.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping) {
				if (gear.Equals(DesiredGearRoadsweeping)) {
					return false;
				}

				if (gear > DesiredGearRoadsweeping) {
					if (IsAboveDownShiftCurve(DesiredGearRoadsweeping, inTorque, inAngularVelocity)) {
						_nextGear = DesiredGearRoadsweeping;
						return true;
					}
				}

				if (gear < DesiredGearRoadsweeping) {
					if (!SpeedTooHighForEngine(
							DesiredGearRoadsweeping,
							inAngularVelocity / GearboxModelData.Gears[DesiredGearRoadsweeping.Gear].Ratio)) {
						_nextGear = DesiredGearRoadsweeping;
						return true;
					}
				}
			}

			// normal shift when all requirements are fullfilled ------------------
			var minimumShiftTimePassed =
				(lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
			if (!minimumShiftTimePassed) {
				return false;
			}

			_nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				response);
			if (!_nextGear.Equals(gear)) {
				return true;
			}

			_nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
				response);

			return !_nextGear.Equals(gear);
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
                if (nextGear.Gear - currentGear.Gear == 1) {
                    return currentGear;
                }
                // if a gear is skipped but acceleration is less than 0.1, try for next gear. if acceleration is still below 0.1 don't shift!
                if (nextGear.Gear > currentGear.Gear &&
                    EstimateAccelerationForGear(Gears.Successor(currentGear), outAngularVelocity)
                        .IsSmaller(GearshiftParams.UpshiftMinAcceleration)) {
                    return currentGear;
                }
                nextGear = Gears.Successor(currentGear);
            }

            return nextGear;
        }

		protected virtual GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Successor(currentGear);

				while (Gears.HasSuccessor(currentGear)) {
					currentGear = Gears.Successor(currentGear);
					var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					inAngularVelocity = response.Engine.EngineSpeed; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.Clutch.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(
						response.Engine.DynamicFullLoadPower /
						((DataBus.EngineInfo.EngineSpeed + response.Engine.EngineSpeed) / 2),
						currentGear.Equals(Gears.First())
							? double.MaxValue.SI<NewtonMeter>()
							: GearboxModelData.Gears[currentGear.Gear].ShiftPolygon
								.InterpolateDownshift(response.Engine.EngineSpeed));
					var reserve = 1 - inTorque.Value() / maxTorque.Value();

					if (reserve >= 0 /*ModelData.TorqueReserve */ && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear = Gears.Predecessor(currentGear);
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (Gears.HasSuccessor(currentGear)) {
				currentGear = CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
			}
			return currentGear;
		}


        protected virtual GearshiftPosition CheckDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
        {
            if ((absTime - _gearbox.LastUpshift).IsSmaller(GearshiftParams.DownshiftAfterUpshiftDelay)) {
                return currentGear;
            }
            return DoCheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);
        }




        #region Overrides of AMTShiftStrategy

        protected virtual GearshiftPosition CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			IResponse minFCResponse = null;
			var fcCurrent = double.NaN;

			var fcUpshiftPossible = true;

			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear.Gear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return currentGear;
			}

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			var vDrop = DataBus.VehicleInfo.VehicleSpeed - estimatedVelocityPostShift;
			var vehicleSpeedPostShift = DataBus.VehicleInfo.VehicleSpeed - vDrop * _shiftStrategyParameters.VelocityDropFactor;

			var totalTransmissionRatio = DataBus.EngineInfo.EngineSpeed / DataBus.VehicleInfo.VehicleSpeed;

			//for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
			foreach (var tryNextGear in Gears.IterateGears(Gears.Successor(currentGear),
				Gears.Successor(currentGear, (uint)_shiftStrategyParameters.AllowedGearRangeFC))) {
				//var tryNextGear = (uint)(currentGear.Gear + i);

				if (tryNextGear == null ||
					!(GearboxModelData.Gears[tryNextGear.Gear].Ratio < _shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				fcUpshiftPossible = true;

				//var response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration, tryNextGear);
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity,
					tryNextGear);
				
				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				var estimatedEngineSpeed = vehicleSpeedPostShift * (totalTransmissionRatio /
						GearboxModelData.Gears[currentGear.Gear].Ratio * GearboxModelData.Gears[tryNextGear.Gear].Ratio);
				if (estimatedEngineSpeed.IsSmaller(_shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				var pNextGearMax = DataBus.EngineInfo.EngineStationaryFullPower(estimatedEngineSpeed);

				if (!response.Engine.PowerRequest.IsSmaller(pNextGearMax)) {
					continue;
				}

				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				//var reserve = 1 - response.EngineTorqueDemandTotal / response.EngineStationaryFullLoadTorque;

				if (reserve < GearshiftParams.TorqueReserve /* && reserve > -0.1*/) {
					//var acc = EstimateAcceleration(outAngularVelocity, outTorque);

					var accelerationFactor = outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio < fld[0].NTq98hSpeed
						? 1.0
						: VectoMath.Interpolate(
							fld[0].NTq98hSpeed, fld[0].NP98hSpeed, 1.0, _shiftStrategyParameters.AccelerationFactor,
							outAngularVelocity * GearboxModelData.Gears[currentGear.Gear].Ratio);
					if (accelerationFactor.IsEqual(1, 1e-9)) {
						continue;
					}
					//var minAcc = VectoMath.Min(DataBus.DriverAcceleration, accCurve.Lookup(DataBus.VehicleSpeed).Acceleration * accelerationFactor);
					//var minAcc = DataBus.DriverAcceleration * accelerationFactor;
					//response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, minAcc, tryNextGear);
					var accelerationTorque = vehicleMass * DataBus.DriverInfo.DriverAcceleration * DataBus.VehicleInfo.VehicleSpeed / outAngularVelocity;
					var reducedTorque = outTorque - accelerationTorque * (1 - accelerationFactor);

					response = RequestDryRunWithGear(absTime, dt, reducedTorque, outAngularVelocity, tryNextGear);
					fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					if (reserve < GearshiftParams.TorqueReserve) {
						continue;
					} else {
						//Log.Error("foo");
					}
				}

				if (double.IsNaN(fcCurrent)) {
					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
					var tqCurrent = responseCurrent.Engine.TotalTorqueDemand.LimitTo(
						fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
						fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed));
					fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, tqCurrent);
				}
				var tqNext = response.Engine.TotalTorqueDemand.LimitTo(
					fld[tryNextGear.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
					fld[tryNextGear.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed));
				var fcNext = GetFCRating(response.Engine.EngineSpeed, tqNext);
				
				if (reserve < GearshiftParams.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * _shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
				minFCResponse = response;
			}

			if (!currentGear.Equals(minFcGear)) {
				return minFcGear;
			}

			//todo mk20210618 fcUpshiftPossible is always true! Maybe this statement can be simplified?
			return fcUpshiftPossible
				? currentGear
				: CheckEarlyUpshiftBase(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
		}

		protected virtual GearshiftPosition CheckEarlyUpshiftBase(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// try if next gear would provide enough torque reserve
			var tryNextGear = Gears.Successor(currentGear);
			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

			var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
			var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

			// if next gear supplied enough power reserve: take it
			// otherwise take
			if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				if (reserve >= GearshiftParams.TorqueReserve) {
					currentGear = tryNextGear;
				}
			}
			return currentGear;
		}

        private double GetFCRating(PerSecond engineSpeed, NewtonMeter tqCurrent)
		{
			var fcCurrent = 0.0;
			foreach (var fuel in fcMap) {
				var fcCurRes = fuel.ConsumptionMap.GetFuelConsumption(tqCurrent, engineSpeed, true);
				if (fcCurRes.Extrapolated) {
					Log.Warn(
						"EffShift Strategy: Extrapolation of fuel consumption for current gear!n: {0}, Tq: {1}",
						engineSpeed, tqCurrent);
				}
				fcCurrent += fcCurRes.Value.Value() * fuel.FuelData.LowerHeatingValueVecto.Value();
			}
			return fcCurrent;
		}

		
		protected virtual GearshiftPosition DoCheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			var nextGear = DoCheckDownshiftBase(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);

			if (Equals(nextGear, currentGear) && currentGear.Gear > GearboxModelData.Gears.Keys.Min()) {
				nextGear = CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
			}
			return nextGear;
		}

		protected virtual GearshiftPosition DoCheckDownshiftBase(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response)
		{
			// down shift
			if (IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Predecessor(currentGear);
			}
			return currentGear;
		}

        protected virtual GearshiftPosition CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(DeclarationData.GearboxTCU.MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			if (response1.Engine.TorqueOutDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear.Gear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return currentGear;
			}

			//for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
			foreach (var tryNextGear in Gears.IterateGears(Gears.Predecessor(currentGear),
				Gears.Predecessor(currentGear, (uint)_shiftStrategyParameters.AllowedGearRangeFC))) {
				//var tryNextGear = (uint)(currentGear.Gear - i);

				if (tryNextGear == null || !(GearboxModelData.Gears[tryNextGear.Gear].Ratio <= _shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				//var response = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, tryNextGear);

				var inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				if (double.IsNaN(fcCurrent)) {
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, responseCurrent.Engine.TorqueOutDemand.LimitTo(
							fld[currentGear.Gear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
							fld[currentGear.Gear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed)));
				}
				var fcNext = GetFCRating(response.Engine.EngineSpeed, response.Engine.TorqueOutDemand.LimitTo(
						fld[tryNextGear.Gear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
						fld[tryNextGear.Gear].FullLoadStationaryTorque(response.Engine.EngineSpeed)));

				if (!fcNext.IsSmaller(fcCurrent * _shiftStrategyParameters.RatingFactorCurrentGear) ||
					!fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
			}

			return minFcGear;
		}

		#endregion


		protected virtual ResponseDryRun RequestDryRunWithGear(
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

	}
}
