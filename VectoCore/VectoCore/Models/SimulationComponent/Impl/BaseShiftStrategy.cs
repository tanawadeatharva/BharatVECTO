using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public abstract class BaseShiftStrategy : LoggingObject, IShiftStrategy
	{
		protected readonly IDataBus DataBus;
		protected readonly GearboxData ModelData;

		protected BaseShiftStrategy(GearboxData data, IDataBus dataBus)
		{
			ModelData = data;
			DataBus = dataBus;
		}

		public abstract bool ShiftRequired(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity,
			NewtonMeter inTorque,
			PerSecond inAngularVelocity, uint gear, Second lastShiftTime);

		public abstract uint InitGear(Second absTime, Second dt, NewtonMeter torque, PerSecond outAngularVelocity);
		public abstract uint Engage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outAngularVelocity);
		public abstract void Disengage(Second absTime, Second dt, NewtonMeter outTorque, PerSecond outEngineSpeed);
		public abstract IGearbox Gearbox { get; set; }
		public abstract GearInfo NextGear { get; }

		protected MeterPerSquareSecond EstimateAccelerationForGear(uint gear, PerSecond gbxAngularVelocityOut)
		{
			if (gear == 0 || gear > ModelData.Gears.Count) {
				throw new VectoSimulationException("invalid gear: {0}", gear);
			}

			var vehicleSpeed = DataBus.VehicleSpeed;

			var nextEngineSpeed = gbxAngularVelocityOut * ModelData.Gears[gear].Ratio;
			var maxEnginePower = DataBus.EngineStationaryFullPower(nextEngineSpeed);

			var avgSlope =
			((DataBus.CycleLookAhead(Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Altitude -
			DataBus.Altitude) / Constants.SimulationSettings.GearboxLookaheadForAccelerationEstimation).Value().SI<Radian>();

			var airDragLoss = DataBus.AirDragResistance(vehicleSpeed, vehicleSpeed) * DataBus.VehicleSpeed;
			var rollResistanceLoss = DataBus.RollingResistance(avgSlope) * DataBus.VehicleSpeed;
			var gearboxLoss = ModelData.Gears[gear].LossMap.GetTorqueLoss(gbxAngularVelocityOut,
								maxEnginePower / nextEngineSpeed * ModelData.Gears[gear].Ratio).Value * nextEngineSpeed;
			//DataBus.GearboxLoss();
			var slopeLoss = DataBus.SlopeResistance(avgSlope) * DataBus.VehicleSpeed;
			var axleLoss = DataBus.AxlegearLoss();

			var accelerationPower = maxEnginePower - gearboxLoss - axleLoss - airDragLoss - rollResistanceLoss - slopeLoss;

			var acceleration = accelerationPower / DataBus.VehicleSpeed / (DataBus.TotalMass + DataBus.ReducedMassWheels);

			return acceleration.Cast<MeterPerSquareSecond>();
		}
	}
}