using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class AMTShiftStrategyOptimized : AMTShiftStrategy
	{
		private FuelConsumptionMap fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;

		public AMTShiftStrategyOptimized(VectoRunData runData, IDataBus dataBus) : base(runData, dataBus)
		{
			fcMap = runData.EngineData.ConsumptionMap;
			fld = runData.EngineData.FullLoadCurves;
			shiftStrategyParameters = runData.GearshiftParameters;
			if (shiftStrategyParameters == null) {
				throw new VectoException("Parameters for shift strategy missing!");
			}
		}

		#region Overrides of AMTShiftStrategy

		protected override uint CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear)
		{
			if (ModelData.Gears[currentGear + 1].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC) {
				return OverdriveUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear);
			}

			return base.CheckEarlyUpshift(absTime, dt, outTorque, outAngularVelocity, currentGear);
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
			if (ModelData.Gears[currentGear - 1].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC) {
				return OverdriveDownshift(absTime, dt, outTorque, outAngularVelocity, currentGear);
			}

			return currentGear;
		}

		private uint OverdriveDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint currentGear)
		{
			var tryNextGear = currentGear - 1;
			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear);

			var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
			var inTorque = response.ClutchPowerRequest / inAngularVelocity;

			if (!IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity)) {
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

				if (fcNext.Value.IsSmaller(fcCurrent.Value * shiftStrategyParameters.RatingFactorCurrentGear)) {
					currentGear = tryNextGear;
				}
			}

			return currentGear;
		}

		#endregion

		public new static string Name { get { return "AMT shift strategy w early upshift (FC-based)"; } }
	}
}
