using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.OutputData;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class PEVAMTShiftStrategy : IShiftStrategy
	{
		protected readonly IDataBus DataBus;
		protected readonly GearboxData ModelData;
		protected Gearbox _gearbox;
		protected uint _nextGear;

		public bool EarlyShiftUp { get; }

		public bool SkipGears { get; }

		public PEVAMTShiftStrategy(GearboxData data, IDataBus dataBus)
		{
			ModelData = data;
			DataBus = dataBus;

			EarlyShiftUp = true;
			SkipGears = true;
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

					inAngularVelocity = response.Engine.EngineSpeed; //ModelData.Gears[currentGear].Ratio * outAngularVelocity;
					inTorque = response.Clutch.PowerRequest / inAngularVelocity;

					var maxTorque = VectoMath.Min(response.Engine.DynamicFullLoadPower / ((DataBus.EngineInfo.EngineSpeed + response.Engine.EngineSpeed) / 2),
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

		protected  ResponseDryRun RequestDryRunWithGear(
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
			var maxEnginePower = DataBus.EngineInfo.EngineStationaryFullPower(nextEngineSpeed);

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