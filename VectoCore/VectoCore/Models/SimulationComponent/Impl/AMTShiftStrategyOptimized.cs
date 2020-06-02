using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyOptimized : AMTShiftStrategy
	{
		private List<CombustionEngineFuelData> fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;
		private SimplePowertrainContainer TestContainer;
		private Gearbox TestContainerGbx;

		protected readonly VelocityRollingLookup VelocityDropData;
		private AccelerationCurveData accCurve;

		private Kilogram vehicleMass;
		protected internal ResponseDryRun minFCResponse;

		public static readonly MeterPerSecond MIN_SPEED_AFTER_TRACTION_INTERRUPTION = 5.KMPHtoMeterPerSecond();

		public AMTShiftStrategyOptimized(VectoRunData runData, IVehicleContainer dataBus) : base(runData, dataBus)
		{
			if (runData.EngineData == null) {
				return;
			}

			fcMap = runData.EngineData.Fuels;
			fld = runData.EngineData.FullLoadCurves;
			shiftStrategyParameters = runData.GearshiftParameters;
			accCurve = runData.DriverData.AccelerationCurve;
			vehicleMass = runData.VehicleData.TotalVehicleMass;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			// MQ: 2019-11-29 - fuel used here has no effect as this is the modDatacontainer for the test-powertrain only!
			var modData = new ModalDataContainer(runData, null, new[] {FuelData.Diesel}, null, false);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimplePowertrain(runData, TestContainer);
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

			VelocityDropData = new VelocityRollingLookup();
			dataBus.AddPreprocessor(
				new VelocitySpeedGearshiftPreprocessor(VelocityDropData, runData.GearboxData.TractionInterruption, TestContainer, -grad, grad, 2));

			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		#region Overrides of AMTShiftStrategy

		protected override uint CheckEarlyUpshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			minFCResponse = null;
			var fcCurrent = double.NaN;

			var fcUpshiftPossible = true;

			if (response1.Engine.EngineTorqueDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return currentGear;
			}

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleSpeed, DataBus.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			var vDrop = DataBus.VehicleSpeed - estimatedVelocityPostShift;
			var vehicleSpeedPostShift = DataBus.VehicleSpeed - vDrop * shiftStrategyParameters.VelocityDropFactor;

			var totalTransmissionRatio = DataBus.EngineSpeed / DataBus.VehicleSpeed;
			//var totalTransmissionRatio = outAngularVelocity / DataBus.VehicleSpeed;

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
				var tryNextGear = (uint)(currentGear + i);

				if (tryNextGear > ModelData.Gears.Keys.Max() ||
					!(ModelData.Gears[tryNextGear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				fcUpshiftPossible = true;

				//var response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, DataBus.DriverAcceleration, tryNextGear);
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				var estimatedEngineSpeed = (vehicleSpeedPostShift * (totalTransmissionRatio / ModelData.Gears[currentGear].Ratio * ModelData.Gears[tryNextGear].Ratio)).Cast<PerSecond>();
				if (estimatedEngineSpeed.IsSmaller(shiftStrategyParameters.MinEngineSpeedPostUpshift)) {
					continue;
				}

				var pNextGearMax = DataBus.EngineStationaryFullPower(estimatedEngineSpeed);

				if (!response.Engine.PowerRequest.IsSmaller(pNextGearMax)) {
					continue;
				}

				var fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.Engine.PowerRequest / fullLoadPower;

				//var reserve = 1 - response.EngineTorqueDemandTotal / response.EngineStationaryFullLoadTorque;

				if (reserve < ModelData.TorqueReserve /* && reserve > -0.1*/) {
					//var acc = EstimateAcceleration(outAngularVelocity, outTorque);

					var accelerationFactor = outAngularVelocity * ModelData.Gears[currentGear].Ratio < fld[0].NTq98hSpeed
						? 1.0
						: VectoMath.Interpolate(
							fld[0].NTq98hSpeed, fld[0].NP98hSpeed, 1.0, shiftStrategyParameters.AccelerationFactor,
							outAngularVelocity * ModelData.Gears[currentGear].Ratio);
					if (accelerationFactor.IsEqual(1, 1e-9)) {
						continue;
					}
					//var minAcc = VectoMath.Min(DataBus.DriverAcceleration, accCurve.Lookup(DataBus.VehicleSpeed).Acceleration * accelerationFactor);
					//var minAcc = DataBus.DriverAcceleration * accelerationFactor;
					//response = RequestDryRunWithGear(absTime, dt, vehicleSpeedPostShift, minAcc, tryNextGear);
					var accelerationTorque = vehicleMass * DataBus.DriverAcceleration * DataBus.VehicleSpeed / outAngularVelocity;
					var reducedTorque = outTorque - accelerationTorque * (1 - accelerationFactor);

					response = RequestDryRunWithGear(absTime, dt, reducedTorque, outAngularVelocity, tryNextGear);
					fullLoadPower = response.Engine.PowerRequest - response.DeltaFullLoad;
					reserve = 1 - response.Engine.PowerRequest / fullLoadPower;
					if (reserve < ModelData.TorqueReserve) {
						continue;
					} else {
						//Log.Error("foo");
					}
				}

				if (double.IsNaN(fcCurrent)) {
					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
					var tqCurrent = responseCurrent.Engine.EngineTorqueDemandTotal.LimitTo(
						fld[currentGear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
						fld[currentGear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed));
					fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, tqCurrent);
				}
				var tqNext = response.Engine.EngineTorqueDemandTotal.LimitTo(
					fld[tryNextGear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
					fld[tryNextGear].FullLoadStationaryTorque(response.Engine.EngineSpeed));
				var fcNext = GetFCRating(response.Engine.EngineSpeed, tqNext);
				
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

			return fcUpshiftPossible
				? currentGear
				: base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response1);
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

		
		protected override uint DoCheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear, IResponse response)
		{
			var nextGear = base.DoCheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear, response);

			if (nextGear == currentGear && currentGear > ModelData.Gears.Keys.Min()) {
				nextGear = CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear, response);
			}
			return nextGear;
		}

		protected virtual uint CheckEarlyDownshift(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear, IResponse response1)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			var fcCurrent = double.NaN;

			var estimatedVelocityPostShift = VelocityDropData.Interpolate(DataBus.VehicleSpeed, DataBus.RoadGradient ?? 0.SI<Radian>());
			if (!estimatedVelocityPostShift.IsGreater(MIN_SPEED_AFTER_TRACTION_INTERRUPTION)) {
				return currentGear;
			}

			if (response1.Engine.EngineTorqueDemand.IsSmaller(DeclarationData.GearboxTCU.DragMarginFactor * fld[currentGear].DragLoadStationaryTorque(response1.Engine.EngineSpeed))) {
				return currentGear;
			}

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
				var tryNextGear = (uint)(currentGear - i);

				if (tryNextGear <= 1 || !(ModelData.Gears[tryNextGear].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				//var response = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, tryNextGear);

				var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				var inTorque = response.Clutch.PowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				if (double.IsNaN(fcCurrent)) {
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);

					//var responseCurrent = RequestDryRunWithGear(absTime, dt, DataBus.VehicleSpeed, DataBus.DriverAcceleration, currentGear);
					fcCurrent = GetFCRating(responseCurrent.Engine.EngineSpeed, responseCurrent.Engine.EngineTorqueDemand.LimitTo(
							fld[currentGear].DragLoadStationaryTorque(responseCurrent.Engine.EngineSpeed),
							fld[currentGear].FullLoadStationaryTorque(responseCurrent.Engine.EngineSpeed)));
				}
				var fcNext = GetFCRating(response.Engine.EngineSpeed, response.Engine.EngineTorqueDemand.LimitTo(
						fld[tryNextGear].DragLoadStationaryTorque(response.Engine.EngineSpeed),
						fld[tryNextGear].FullLoadStationaryTorque(response.Engine.EngineSpeed)));

				if (!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) ||
					!fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext;
			}

			return minFcGear;
		}

		#endregion

		protected ResponseDryRun RequestDryRunWithGear(
			Second absTime, Second dt, MeterPerSecond vehicleSpeed, MeterPerSquareSecond acceleration, uint tryNextGear)
		{
			LogEnabled = false;
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = tryNextGear;

			//TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			TestContainer.VehiclePort.Initialize(vehicleSpeed, DataBus.RoadGradient);
			var response = (ResponseDryRun)TestContainer.VehiclePort.Request(
				0.SI<Second>(), dt, acceleration, DataBus.RoadGradient, true);

			//var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
			//	0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			LogEnabled = true;
			return response;
		}

		protected override ResponseDryRun RequestDryRunWithGear(
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

		public new static string Name
		{
			get { return "AMT - EffShift"; }
		}

		#region Overrides of AMTShiftStrategy

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve,
			IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius)
		{
			return DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
				i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
		}

		#endregion
	}
}
