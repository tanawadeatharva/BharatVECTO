using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Ninject.Extensions.NamedScope;
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

		protected readonly List<GearshiftPosition> GearList;

		public new static string Name
		{
			get { return "AT - EffShift"; }
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
			// initialize vehicle so that vehicleStopped of the testcontainer is false (required for test-runs)
			TestContainer.VehiclePort.Initialize(10.KMPHtoMeterPerSecond(), 0.SI<Radian>());

			if (runData.Cycle.CycleType == CycleType.MeasuredSpeed) {
				try {
					TestContainer.GetCycleOutPort().Initialize();
					TestContainer.GetCycleOutPort().Request(0.SI<Second>(), 1.SI<Second>());
				} catch (Exception ) { }
			}

			if (shiftStrategyParameters.AllowedGearRangeFC > 2 || shiftStrategyParameters.AllowedGearRangeFC < 1) {
				Log.Warn("Gear-range for FC-based gearshift must be either 1 or 2!");
				shiftStrategyParameters.AllowedGearRangeFC = shiftStrategyParameters.AllowedGearRangeFC.LimitTo(1, 2);
			}


			GearList = new List<GearshiftPosition>();
			foreach (var gear in runData.GearboxData.Gears) {
				if (runData.GearboxData.Type.AutomaticTransmission()) {
					if (gear.Value.HasTorqueConverter) {
						GearList.Add(new GearshiftPosition(gear.Key, false));
					}
					if (gear.Value.HasLockedGear) {
						GearList.Add(new GearshiftPosition(gear.Key, true));
					}
				} else {
					GearList.Add(new GearshiftPosition(gear.Key));
				}
			}
		}

		#region Overrides of ATShiftStrategy

		protected override bool? CheckEarlyUpshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime)
		{
			if (outAngularVelocity.IsEqual(0)) {
				return null;
			}

			var minFcGear = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var minFc = double.MaxValue;
			KilogramPerSecond fcCurrent = null;

			var current = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var currentIdx = GearList.IndexOf(current);

			for (var i = 1; i <=  shiftStrategyParameters.AllowedGearRangeFC; i++) {

				if (currentIdx + i >= GearList.Count) {
					// no further gear
					continue;
				}

				var next = GearList[currentIdx + i];
				if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
					// upshift from C to L with skipping gear not allowed
					continue;
				}

				if (!(ModelData.Gears[next.Gear].Ratio < shiftStrategyParameters.RatioEarlyUpshiftFC)) {
					continue;
				}

				
				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, next);

				var inAngularVelocity = ModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var inTorque = response.EnginePowerRequest / inAngularVelocity;

				// if next gear supplied enough power reserve: take it
				// otherwise take
				if (ModelData.Gears[next.Gear].ShiftPolygon.IsBelowDownshiftCurve(inTorque, inAngularVelocity)) {
					continue;
				}

				var fullLoadPower = response.EnginePowerRequest - response.DeltaFullLoad;
				var reserve = 1 - response.EnginePowerRequest / fullLoadPower;

				if (fcCurrent == null) {
					var responseCurrent = RequestDryRunWithGear(
						absTime, dt, outTorque, outAngularVelocity, current);
					fcCurrent = fcMap.GetFuelConsumption(
						responseCurrent.EngineTorqueDemand.LimitTo(
							fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
							fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed))
						, responseCurrent.EngineSpeed).Value;
				}
				var fcNext = fcMap.GetFuelConsumption(
					response.EngineTorqueDemand.LimitTo(
						fld[next.Gear].DragLoadStationaryTorque(response.EngineSpeed),
						fld[next.Gear].FullLoadStationaryTorque(response.EngineSpeed)), response.EngineSpeed).Value;

				if (reserve < ModelData.TorqueReserve ||
					!fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) || !fcNext.IsSmaller(minFc)) {
					continue;
				}

				minFc = fcNext.Value();
				minFcGear = next;
			}

			if (!minFcGear.Equals(current)) {
				ShiftGear(absTime, current, minFcGear);
				return true;
			}

			return null;	
		}

		

		protected override bool? CheckEarlyDownshift(
			Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, NewtonMeter origInTorque,
			PerSecond origInAngularVelocity, uint currentGear, Second lastShiftTime)
		{
			var minFcGear = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked); 
			var minFc = double.MaxValue;
			KilogramPerSecond fcCurrent = null;

			var current = new GearshiftPosition(currentGear, _gearbox.TorqueConverterLocked);
			var currentIdx = GearList.IndexOf(current);

			for (var i = 1; i <= shiftStrategyParameters.AllowedGearRangeFC; i++) {

				if (currentIdx - i < 0) {
					// no further gear
					continue;
				}

				var next = GearList[currentIdx - i];
				if (!next.TorqueConverterLocked.Value) {
					continue;
				}
				if (current.TorqueConverterLocked != next.TorqueConverterLocked && current.Gear != next.Gear) {
					// downshift from C to L with skipping gear not allowed
					continue;
				}

				if (!(ModelData.Gears[next.Gear].Ratio < shiftStrategyParameters.RatioEarlyDownshiftFC)) {
					continue;
				}

				var response = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, next);

				var inAngularVelocity = ModelData.Gears[next.Gear].Ratio * outAngularVelocity;
				var inTorque = response.EnginePowerRequest / inAngularVelocity;

				if (!IsAboveUpShiftCurve(next.Gear, inTorque, inAngularVelocity, next.TorqueConverterLocked.Value)) {

					if (fcCurrent == null) {
						var responseCurrent = RequestDryRunWithGear(absTime, dt, outTorque, outAngularVelocity, current);
						fcCurrent = fcMap.GetFuelConsumption(
							responseCurrent.EngineTorqueDemand.LimitTo(
								fld[currentGear].DragLoadStationaryTorque(responseCurrent.EngineSpeed),
								fld[currentGear].FullLoadStationaryTorque(responseCurrent.EngineSpeed))
							, responseCurrent.EngineSpeed).Value;
					}
					var fcNext = fcMap.GetFuelConsumption(
						response.EngineTorqueDemand.LimitTo(
							fld[next.Gear].DragLoadStationaryTorque(response.EngineSpeed),
							fld[next.Gear].FullLoadStationaryTorque(response.EngineSpeed)), response.EngineSpeed).Value;

					if (fcNext.IsSmaller(fcCurrent * shiftStrategyParameters.RatingFactorCurrentGear) && fcNext.IsSmaller(minFc)) {
						minFcGear = next;
						minFc = fcNext.Value();

					}
				}
			}

			if (!current.Equals(minFcGear)) {
				ShiftGear(absTime, current, minFcGear);
				return true;
			}

			return null;
		}

		protected virtual void ShiftGear(Second absTime, GearshiftPosition currentGear, GearshiftPosition nextGear)
		{
			if (currentGear.TorqueConverterLocked != nextGear.TorqueConverterLocked && currentGear.Gear != nextGear.Gear) {
				throw new VectoException("skipping gear from converter to locked not allowed! {0} -> {1}", currentGear.Name, nextGear.Name);
			}
			_nextGear.SetState(absTime, disengaged: false, gear: nextGear.Gear, tcLocked: nextGear.TorqueConverterLocked.Value);
		}

		#endregion

		protected ResponseDryRun RequestDryRunWithGear(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity, GearshiftPosition gear)
		{
			TestContainerGbx.Disengaged = false;
			TestContainerGbx.Gear = gear.Gear;
			TestContainerGbx.TorqueConverterLocked = gear.TorqueConverterLocked.Value;

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

	[DebuggerDisplay("{Name}")]
	public class GearshiftPosition
	{
		public uint Gear { get; }
		public bool? TorqueConverterLocked { get; }

		public GearshiftPosition(uint gear, bool? torqueConverterLocked = null)
		{
			Gear = gear;
			TorqueConverterLocked = torqueConverterLocked;
		}

		public string Name
		{
			get {
				return string.Format(
					"{0}{1}", Gear,
					Gear == 0 ? "" : (TorqueConverterLocked.HasValue ? (TorqueConverterLocked.Value ? "L" : "C") : ""));
			}
		}

		public override bool Equals(object x)
		{
			var other = x as GearshiftPosition;
			if (other == null)
				return false;

			return other.Gear == Gear && other.TorqueConverterLocked == TorqueConverterLocked;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

	}
}
