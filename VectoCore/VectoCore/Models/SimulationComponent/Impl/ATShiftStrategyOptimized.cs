using System;
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


	public class ATShiftStrategyOptimized : ATShiftStrategy
	{
		private FuelConsumptionMap fcMap;
		private Dictionary<uint, EngineFullLoadCurve> fld;
		private ShiftStrategyParameters shiftStrategyParameters;
		private SimplePowertrainContainer TestContainer;
		private ATGearbox TestContainerGbx;


		public new static string Name
		{
			get { return "AT shift strategy w early upshift (FC-based)"; }
		}

		public ATShiftStrategyOptimized(VectoRunData runData, IDataBus dataBus) : base(runData, dataBus)
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
			TestContainerGbx = TestContainer.GearboxCtl as ATGearbox;
			if (TestContainerGbx == null) {
				throw new VectoException("Unknown gearboxtype: {0}", TestContainer.GearboxCtl.GetType().FullName);
			}

			if (runData.Cycle.CycleType == CycleType.MeasuredSpeed) {
				try {
					TestContainer.GetCycleOutPort().Initialize();
					TestContainer.GetCycleOutPort().Request(0.SI<Second>(), 1.SI<Second>());
				} catch (Exception ) { }
			}

			
		}

		#region Overrides of ATShiftStrategy

		protected override bool? CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime)
		{
			var tryNextGear = _gearbox.TorqueConverterLocked ? currentGear + 1 : currentGear;
			if (!(ModelData.Gears[tryNextGear].Ratio <= shiftStrategyParameters.RatioEarlyDownshiftFC)) {
				return null;
			}

			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear, true);

			var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
			var inTorque = response.EnginePowerRequest / inAngularVelocity;

			// if next gear supplied enough power reserve: take it
			// otherwise take
			if (!ModelData.Gears[tryNextGear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inAngularVelocity)) {
				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear, _gearbox.TorqueConverterLocked);
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
					Upshift(absTime, currentGear);
					return true;
				}
			}
			return null;
		}

		protected override bool? CheckEarlyDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime)
		{
			var tryNextGear = _gearbox.TorqueConverterLocked && currentGear == 1 ? currentGear : currentGear - 1;
			var tryNextTc = currentGear == 1 && _gearbox.TorqueConverterLocked ? false : true;
			if (!(ModelData.Gears[tryNextGear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
				return null;
			}

			var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, tryNextGear, tryNextTc);

			var inAngularVelocity = ModelData.Gears[tryNextGear].Ratio * outAngularVelocity;
			var inTorque = response.EnginePowerRequest / inAngularVelocity;

			if (!IsAboveUpShiftCurve(tryNextGear, inTorque, inAngularVelocity, tryNextTc)) {
				var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, currentGear, _gearbox.TorqueConverterLocked);
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
					Downshift(absTime, currentGear);
					return true;
				}
			}

			return null;
		}

		#endregion

		protected ResponseDryRun RequestDryRunWithGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, uint tryNextGear, bool tcLocked)
		{
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = tryNextGear;
			TestContainerGbx.TorqueConverterLocked = tcLocked;

			TestContainer.GearboxOutPort.Initialize(outTorque, outAngularVelocity);
			var response = (ResponseDryRun)TestContainer.GearboxOutPort.Request(
				0.SI<Second>(), dt, outTorque, outAngularVelocity, true);
			return response;
		}

		#region Overrides of ATShiftStrategy

		public override ShiftPolygon ComputeDeclarationShiftPolygon(
			GearboxType gearboxType, int i, EngineFullLoadCurve engineDataFullLoadCurve, IList<ITransmissionInputData> gearboxGears,
			CombustionEngineData engineData, double axlegearRatio, Meter dynamicTyreRadius)
		{
			var shiftLine = DeclarationData.Gearbox.ComputeEfficiencyShiftPolygon(
				Math.Max(i, 2), engineDataFullLoadCurve, gearboxGears, engineData, axlegearRatio, dynamicTyreRadius);

			var upshift = new List<ShiftPolygon.ShiftPolygonEntry>();

			if (i < gearboxGears.Count - 1) {

				var maxDragTorque = engineDataFullLoadCurve.MaxDragTorque * 1.1;
				var maxTorque = engineDataFullLoadCurve.MaxTorque * 1.1;

				var speed = engineData.FullLoadCurves[0].RatedSpeed / gearboxGears[i].Ratio * gearboxGears[i + 1].Ratio;


				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxDragTorque, speed));
				upshift.Add(new ShiftPolygon.ShiftPolygonEntry(maxTorque, speed));
			}
			return new ShiftPolygon(shiftLine.Downshift.ToList(), upshift);
		}

		#endregion
	}
}
