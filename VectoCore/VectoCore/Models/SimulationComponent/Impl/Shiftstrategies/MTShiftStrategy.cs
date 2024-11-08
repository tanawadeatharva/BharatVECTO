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
			if ((IsBelowDownShiftCurve(currentGear, inTorque, inAngularVelocity) && droppedSpeedRatio.IsSmallerOrEqual(2.0)) ||
				IsBelowExtendedDownShiftCurve(currentGear, inTorque, inAngularVelocity))
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