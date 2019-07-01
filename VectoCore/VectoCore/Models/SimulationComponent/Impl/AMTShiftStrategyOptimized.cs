using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyOptimized : AMTShiftStrategy
	{
		private FuelConsumptionMap fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;
		private SimplePowertrainContainer TestContainer;
		private Gearbox TestContainerGbx;

		public AMTShiftStrategyOptimized(VectoRunData runData, IDataBus dataBus) : base(runData, dataBus)
		{
			if (runData.EngineData == null) {
				return;
			}
			fcMap = runData.EngineData.ConsumptionMap;
			fld = runData.EngineData.FullLoadCurves;
			shiftStrategyParameters = runData.GearshiftParameters;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}

			var modData = new ModalDataContainer(runData, null, null, false);
			var builder = new PowertrainBuilder(modData);
			TestContainer = new SimplePowertrainContainer(runData);
			builder.BuildSimplePowertrain(runData, TestContainer);
			TestContainerGbx = TestContainer.GearboxCtl as Gearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}
		}

		#region Overrides of AMTShiftStrategy

		protected override uint CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			KilogramPerSecond fcCurrent = null;

			var fcUpshiftPossible = false;

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
				var tryNextGear = (uint)(currentGear + i);

				if (tryNextGear > ModelData.Gears.Keys.Max() || !(ModelData.Gears[tryNextGear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				fcUpshiftPossible = true;
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				var inTorque = response.ClutchPowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (fcCurrent == null) {
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
					fcCurrent = fcMap.GetFuelConsumption(
						responseCurrent.EngineTorqueDemandTotal.LimitTo(
							fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
							fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed))
						, responseCurrent.EngineSpeed).Value;
				}
				var fcNext = fcMap.GetFuelConsumption(
					response.EngineTorqueDemandTotal.LimitTo(
						fld[tryNextGear].DragLoadStationaryTorque(response.EngineSpeed),
						fld[tryNextGear].FullLoadStationaryTorque(response.EngineSpeed)), response.EngineSpeed).Value;

				if (reserve < ModelData.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext.Value();
			}

			if (currentGear != minFcGear) {
				return minFcGear;
			}

			return fcUpshiftPossible ? currentGear : base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear);
		}

		protected virtual uint OverdriveUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear)
		{
			var tryNextGear = currentGear + 1;
			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

			var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
			var inTorque = response.ClutchPowerRequest / inAngularVelocity;

			// if next gear supplied enough power reserve: take it
			// otherwise take
			if (!IsBelowDownShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
				var fcCurrent = fcMap.GetFuelConsumption(
					responseCurrent.EngineTorqueDemand.LimitTo(
						fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
						fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed))
					, responseCurrent.EngineSpeed);
				var fcNext = fcMap.GetFuelConsumption(
					response.EngineTorqueDemand.LimitTo(
						fld[tryNextGear].DragLoadStationaryTorque(response.EngineSpeed),
						fld[tryNextGear].FullLoadStationaryTorque(response.EngineSpeed)), response.EngineSpeed);

				if (reserve >= ModelData.TorqueReserve && fcNext.Value.IsSmaller(fcCurrent.Value * shiftStrategyParameters.RatingFactorCurrentGear)) {
					currentGear = tryNextGear;
				}
			}

			return currentGear;
		}

		protected override uint DoCheckDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque, PerSecond inAngularVelocity, uint currentGear)
		{
			var nextGear = base.DoCheckDownshift(
				absTime, dt, outTorque, outAngularVelocity, inTorque, inAngularVelocity, currentGear);

			if (nextGear == currentGear && currentGear > ModelData.Gears.Keys.Min()) {
				nextGear = CheckEarlyDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear);
			}
			return nextGear;
		}

		protected virtual uint CheckEarlyDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear)
		{
			var minFcGear = currentGear;
			var minFc = double.MaxValue;
			KilogramPerSecond fcCurrent = null;

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {
				var tryNextGear = (uint)(currentGear - i);

				if (tryNextGear <= 1 || !(ModelData.Gears[tryNextGear].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

				var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
				var inTorque = response.ClutchPowerRequest / inAngularVelocity;

				if (IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
					continue;
				}

				
				if (fcCurrent == null) {
					var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear);
					fcCurrent = fcMap.GetFuelConsumption(
						responseCurrent.EngineTorqueDemand.LimitTo(
							fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
							fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed))
						, responseCurrent.EngineSpeed).Value;
				}
				var fcNext = fcMap.GetFuelConsumption(
					response.EngineTorqueDemand.LimitTo(
						fld[tryNextGear].DragLoadStationaryTorque(response.EngineSpeed),
						fld[tryNextGear].FullLoadStationaryTorque(response.EngineSpeed)), response.EngineSpeed).Value;

				if (!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) ||
					!fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFcGear = tryNextGear;
				minFc = fcNext.Value();
			}

			return minFcGear;
		}


		#endregion

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

		public new static string Name { get { return "AMT shift strategy w early upshift (FC-based)"; } }

		#region Overrides of AMTShiftStrategy

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius)
		{
			return DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(i, engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);
		}

		#endregion
	}
}
