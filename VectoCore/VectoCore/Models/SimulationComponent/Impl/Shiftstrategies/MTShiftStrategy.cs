/*
* This file is part of VECTO.
*
* Copyright © 2012-2019 European Union
*
* Developed by Graz University of Technology,
*              Institute of Internal Combustion Engines and Thermodynamics,
*              Institute of Technical Informatics
*
* VECTO is licensed under the EUPL, Version 1.1 or - as soon they will be approved
* by the European Commission - subsequent versions of the EUPL (the "Licence");
* You may not use VECTO except in compliance with the Licence.
* You may obtain a copy of the Licence at:
*
* https://joinup.ec.europa.eu/community/eupl/og_page/eupl
*
* Unless required by applicable law or agreed to in writing, VECTO
* distributed under the Licence is distributed on an "AS IS" basis,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the Licence for the specific language governing permissions and
* limitations under the Licence.
*
* Authors:
*   Stefan Hausberger, hausberger@ivt.tugraz.at, IVT, Graz University of Technology
*   Christian Kreiner, christian.kreiner@tugraz.at, ITI, Graz University of Technology
*   Michael Krisper, michael.krisper@tugraz.at, ITI, Graz University of Technology
*   Raphael Luz, luz@ivt.tugraz.at, IVT, Graz University of Technology
*   Markus Quaritsch, markus.quaritsch@tugraz.at, IVT, Graz University of Technology
*   Martin Rexeis, rexeis@ivt.tugraz.at, IVT, Graz University of Technology
*/

using System;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl.Shiftstrategies
{
	public class MTShiftStrategy : AMTShiftStrategy
	{
		VelocitySpeedGearshiftPreprocessor PreprocessorSpeed;
		VelocityRollingLookup velocityDropData = new VelocityRollingLookup();

		public MTShiftStrategy(IVehicleContainer bus) : base(bus)
		{
			EarlyShiftUp = false;
			SkipGears = true;

			PreprocessorSpeed = ConfigureSpeedPreprocessor(bus);
			bus.AddPreprocessor(PreprocessorSpeed);
		}

		public new static string Name => "MT Shift Strategy";

		protected override GearshiftPosition DoCheckUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// upshift
			if (IsAboveUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
				currentGear = Gears.Successor(currentGear);

				while (SkipGears && currentGear.Gear < GearboxModelData.Gears.Count) {
					currentGear = Gears.Successor(currentGear);
					var tmpGear = Gearbox.Gear;
					_gearbox.Gear = currentGear;
					var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
					_gearbox.Gear = tmpGear;

					inAngularVelocity = response.Engine.EngineSpeed; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.Clutch.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(response.Engine.DynamicFullLoadPower / ((DataBus.EngineInfo.EngineSpeed + response.Engine.EngineSpeed) / 2),
						currentGear.Gear > 1
							? GearboxModelData.Gears[currentGear.Gear].ShiftPolygon.InterpolateDownshift(response.Engine.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = 1 - inTorque / maxTorque;

					if (reserve >= GearshiftParams.TorqueReserve && IsAboveDownShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}

					currentGear = Gears.Predecessor(currentGear);
					break;
				}
			}

			// early up shift to higher gear ---------------------------------------
			if (EarlyShiftUp && currentGear.Gear < GearboxModelData.Gears.Count) {
				// try if next gear would provide enough torque reserve
				var tryNextGear = Gears.Successor(currentGear);
				var tmpGear = Gearbox.Gear;
				_gearbox.Gear = tryNextGear;
				var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
				_gearbox.Gear = tmpGear;

				inAngularVelocity = GearboxModelData.Gears[tryNextGear.Gear].Ratio * outAngularVelocity;
				inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

					if (reserve >= GearshiftParams.TorqueReserve) {
						currentGear = tryNextGear;
					}
				}
			}
			return currentGear;
		}

		protected override GearshiftPosition DoCheckDownshift(Second absTime, Second dt, NewtonMeter outTorque,
			PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition currentGear, IResponse response1)
		{
			// down shift
			var interpolatedDroppedSpeed = velocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
			var droppedSpeed = interpolatedDroppedSpeed == 0.SI<MeterPerSecond>() || interpolatedDroppedSpeed == null
				? DataBus.VehicleInfo.VehicleSpeed : interpolatedDroppedSpeed;

			double droppedSpeedRatio = DataBus.VehicleInfo.VehicleSpeed / droppedSpeed;
			var UphillBrakingLowSpeed = inTorque.IsSmaller(0.0) && interpolatedDroppedSpeed.IsSmaller(GearboxModelData.DisengageWhenHaltingSpeed) && DataBus.DrivingCycleInfo.RoadGradient.IsGreater(0.0);

			if ((((IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity) && droppedSpeedRatio.IsSmallerOrEqual(2.0)) || IsBelowExtendedDownShiftCurve(currentGear, inTorque, inAngularVelocity)) && !(UphillBrakingLowSpeed)) ||
				(inAngularVelocity.IsSmaller(DataBus.EngineInfo.EngineIdleSpeed) && UphillBrakingLowSpeed))
			{
				currentGear = Gears.Predecessor(currentGear);
				while (SkipGears && currentGear.Gear > 1) {
					currentGear = Gears.Predecessor(currentGear);
					var tmpGear = Gearbox.Gear;
					_gearbox.Gear = currentGear;
					var response = (ResponseDryRun)_gearbox.Request(absTime, dt, outTorque, outAngularVelocity, true);
					_gearbox.Gear = tmpGear;

					inAngularVelocity = GearboxModelData.Gears[currentGear.Gear].Ratio * outAngularVelocity;
					inTorque = response.Clutch.PowerRequest / inAngularVelocity;
					var maxTorque = VectoMath.Min(response.Engine.DynamicFullLoadPower / ((DataBus.EngineInfo.EngineSpeed + response.Engine.EngineSpeed) / 2),
						currentGear.Gear > 1
							? GearboxModelData.Gears[currentGear.Gear].ShiftPolygon.InterpolateDownshift(response.Engine.EngineSpeed)
							: double.MaxValue.SI<NewtonMeter>());
					var reserve = maxTorque.IsEqual(0) ? -1 : (1 - inTorque / maxTorque).Value();
					if (reserve >= GearshiftParams.TorqueReserve && IsBelowUpShiftCurve(currentGear, inTorque, inAngularVelocity)) {
						continue;
					}
					currentGear = Gears.Successor(currentGear);
					break;
				}
			}
			return currentGear;
		}

        protected override bool DoCheckShiftRequired(Second absTime, Second dt, NewtonMeter outTorque,
            PerSecond outAngularVelocity, NewtonMeter inTorque, PerSecond inAngularVelocity, GearshiftPosition gear,
            Second lastShiftTime, IResponse response)
        {
            // no shift when vehicle stands
            if (DataBus.VehicleInfo.VehicleStopped)
            {
                return false;
            }

            // emergency shift to not stall the engine ------------------------
            if (Gears.First().Equals(gear) &&
                SpeedTooLowForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio))
            {
                return true;
            }

            _nextGear = gear;
            while (Gears.HasPredecessor(_nextGear) && SpeedTooLowForEngine(_nextGear,
                inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio))
            {
                _nextGear = Gears.Predecessor(_nextGear);
            }

            // upshift if max speed is exceeded 
            // except if upshift results in immediate downshift
            var interpolatedDroppedSpeed = velocityDropData.Interpolate(DataBus.VehicleInfo.VehicleSpeed, DataBus.DrivingCycleInfo.RoadGradient ?? 0.SI<Radian>());
            var droppedSpeed = interpolatedDroppedSpeed == 0.SI<MeterPerSecond>() || interpolatedDroppedSpeed == null
                ? DataBus.VehicleInfo.VehicleSpeed : interpolatedDroppedSpeed;

            var ShiftSpeedDropRatio = Math.Max(droppedSpeed / DataBus.VehicleInfo.VehicleSpeed, 0.10);

            while (Gears.HasSuccessor(_nextGear) &&
                    SpeedTooHighForEngine(_nextGear, inAngularVelocity / GearboxModelData.Gears[gear.Gear].Ratio) &&
                    IsAboveDownShiftCurve(Gears.Successor(_nextGear), outTorque / GearboxModelData.Gears[Gears.Successor(_nextGear).Gear].Ratio, ShiftSpeedDropRatio * outAngularVelocity * GearboxModelData.Gears[Gears.Successor(_nextGear).Gear].Ratio))
            {
                _nextGear = Gears.Successor(_nextGear);
            }

            if (!_nextGear.Equals(gear))
            {
                return true;
            }

            // PTO Active while drive (roadsweeping) shift rules
            if (DataBus.DrivingCycleInfo.CycleData.LeftSample.PTOActive == PTOActivity.PTOActivityRoadSweeping)
            {
                if (gear.Equals(DesiredGearRoadsweeping))
                {
                    return false;
                }

                if (gear > DesiredGearRoadsweeping)
                {
                    if (IsAboveDownShiftCurve(DesiredGearRoadsweeping, inTorque, inAngularVelocity))
                    {
                        _nextGear = DesiredGearRoadsweeping;
                        return true;
                    }
                }

                if (gear < DesiredGearRoadsweeping)
                {
                    if (!SpeedTooHighForEngine(
                        DesiredGearRoadsweeping, inAngularVelocity / GearboxModelData.Gears[DesiredGearRoadsweeping.Gear].Ratio))
                    {
                        _nextGear = DesiredGearRoadsweeping;
                        return true;
                    }
                }
            }

            // normal shift when all requirements are fullfilled ------------------
            var minimumShiftTimePassed =
                (lastShiftTime + GearshiftParams.TimeBetweenGearshifts).IsSmallerOrEqual(absTime);
            if (!minimumShiftTimePassed)
            {
                return false;
            }

            _nextGear = CheckDownshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
                response);
            if (!_nextGear.Equals(gear))
            {
                return true;
            }

            _nextGear = CheckUpshift(absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, gear,
                response);

            return !_nextGear.Equals(gear);
        }

		private VelocitySpeedGearshiftPreprocessor ConfigureSpeedPreprocessor(IVehicleContainer bus)
		{
			var TestContainer = PowertrainBuilder.BuildSimplePowertrain(bus.RunData);
			var TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			if (TestContainerGbx == null)
			{
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			var maxGradient = bus.RunData.Cycle.Entries.Max(x => Math.Abs(x.RoadGradientPercent.Value())) + 1;
			var gradient = Convert.ToInt32(maxGradient / 2) * 2;
			if (gradient == 0)
			{
				gradient = 2;
			}

			return new VelocitySpeedGearshiftPreprocessor(
					velocityDropData,
					bus.RunData.GearboxData.TractionInterruption,
					TestContainer,
					-gradient,
					gradient,
					2);
		}
	}
}