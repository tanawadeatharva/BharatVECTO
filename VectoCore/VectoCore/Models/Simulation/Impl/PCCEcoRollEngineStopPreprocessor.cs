using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
	public class PCCEcoRollEngineStopPreprocessor : ISimulationPreprocessor
	{
		protected SimplePowertrainContainer Container;
		private MeterPerSecond MaxSpeed;
		private MeterPerSecond MinSpeed;
		private EcoRollSlopeData SlopeData;

		public PCCEcoRollEngineStopPreprocessor(SimplePowertrainContainer simpleContainer, EcoRollSlopeData slopeData, MeterPerSecond minSpeed, MeterPerSecond maxSpeed)
		{
			Container = simpleContainer;
			MinSpeed = minSpeed;
			MaxSpeed = maxSpeed;
			SlopeData = slopeData;
			SpeedStep = 5.KMPHtoMeterPerSecond();
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var vehicle = Container?.Vehicle as Vehicle;

			if (vehicle == null) {
				throw new VectoException("no vehicle found...");
			}

			var gearbox = Container.Gearbox as Gearbox;
			if (gearbox == null) {
				throw new VectoException("no gearbox found...");
			}

			var modData = Container.ModalData as ModalDataContainer;
			var runData = Container.RunData;
			var tmp = new Dictionary<MeterPerSecond,Radian>();

			for (var speed = MinSpeed; speed <= MaxSpeed; speed += SpeedStep) {
				var gear = FindLowestGearForSpeed(speed);
				gearbox.Gear = gear;
				gearbox.DisengageGearbox = true;
				gearbox._nextGear = new GearInfo(gear, true);
				vehicle.Initialize(speed, 0.SI<Radian>());

				var slope = SearchSlope(vehicle, Container);

				modData?.Reset();
				tmp[speed] = slope;
			}

			SlopeData.Data = tmp;
		}

		private uint FindLowestGearForSpeed(MeterPerSecond speed)
		{
			var data = Container.RunData;
			var ratio = data.AxleGearData.AxleGear.Ratio * (data.AngledriveData?.Angledrive.Ratio ?? 1.0) /
						data.VehicleData.DynamicTyreRadius;
			return Container.RunData.GearboxData.Gears.Select(
				x => {
					var n = speed * ratio * x.Value.Ratio;
					return n < data.EngineData.IdleSpeed ? 0 : x.Key;
				}).Max();
		}

		private Radian SearchSlope(Vehicle vehicle, SimplePowertrainContainer container)
		{
			var simulationInterval = Constants.SimulationSettings.TargetTimeInterval;
			var acceleration = 0.SI<MeterPerSquareSecond>();
			var absTime = 0.SI<Second>();
			var gradient = 0.SI<Radian>();
			var initialResponse = vehicle.Request(absTime, simulationInterval, acceleration, gradient);
			var delta = initialResponse.GearboxPowerRequest;

			try {
				gradient = SearchAlgorithm.Search(
					gradient, delta, 0.1.SI<Radian>(),
					getYValue: response => {
						var r = (ResponseDryRun)response;
						return r.GearboxPowerRequest;
					},
					evaluateFunction: grad => {
						return vehicle.Request(absTime, simulationInterval, acceleration, grad, true);
					},
					criterion: response => {
						var r = (ResponseDryRun)response;
						return r.GearboxPowerRequest.Value();
					}
					);
			} catch (VectoSearchAbortedException) {
				return gradient;
			}

			return gradient;
		}

		public MeterPerSecond SpeedStep { get; set; }

		#endregion
	}
}
