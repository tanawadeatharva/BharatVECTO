using System.Collections.Generic;
using System.Data;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class VelocitySpeedGearshiftPreprocessor : ISimulationPreprocessor
	{
		protected readonly Second TractionInterruption;
		protected readonly VelocityRollingLookup VehicleVelocityDropLookup;
		protected SimplePowertrainContainer Container;

		public VelocitySpeedGearshiftPreprocessor(
			VelocityRollingLookup velocityDropData, Second tracktionInterruption, SimplePowertrainContainer simpleContainer,
			int minGradient = -24, int maxGradient = 24, int gradientStep = 2)
		{
			Container = simpleContainer;
			VehicleVelocityDropLookup = velocityDropData;
			TractionInterruption = tracktionInterruption;
			MinGradient = minGradient;
			MaxGradient = maxGradient;
			GradientStep = gradientStep;

			var speeds = Enumerable.Range(1, 12).Select(x => (x * 10).KMPHtoMeterPerSecond()).ToList();
			speeds.Insert(0, 5.KMPHtoMeterPerSecond());
			Speeds = speeds;
		}


		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			VehicleVelocityDropLookup.Data = IterateVehicleSpeedAndGradient();
		}

		#endregion

		public int MinGradient { get; set; }

		public int MaxGradient { get; set; }
		public int GradientStep { get; set; }

		protected Entry[] IterateVehicleSpeedAndGradient()
		{
			var container = Container;
			var vehicle = container?.Vehicle as Vehicle;

			if (vehicle == null) {
				throw new VectoException("no vehicle found...");
			}

			var gearbox = container.Gearbox as Gearbox;
			if (gearbox == null) {
				throw new VectoException("no gearbox found...");
			}

			var modData = container.ModalData as ModalDataContainer;
			var runData = container.RunData;
			var ratio = 1.0 / runData.VehicleData.DynamicTyreRadius *
						runData.AxleGearData.AxleGear.Ratio *
						(runData.AngledriveData?.Angledrive.Ratio ?? 1.0);

			var tmp = new List<Entry>();
			foreach (var speed in Speeds) {
				var gearForSpeed = runData.GearboxData.Gears.FirstOrDefault(
					x => (speed * ratio * x.Value.Ratio).IsBetween(
						runData.EngineData.IdleSpeed, runData.EngineData.FullLoadCurves[0].RatedSpeed)).Key;
				for (var grad = MinGradient; grad <= MaxGradient; grad += GradientStep) {
					var gradient = VectoMath.InclinationToAngle(grad / 100.0);
					gearbox.Disengaged = false;
					gearbox.Gear = gearForSpeed;
					vehicle.Initialize(speed, gradient);
					gearbox.Gear = 0;
					gearbox.Disengaged = true;
					gearbox.EngageTime = 100.SI<Second>();
					gearbox._nextGear = new GearInfo(gearForSpeed, true);
					var vehicleSpeed = SimulateRollingVehicle(vehicle, gradient, container);
					modData?.Reset();
					tmp.Add(new Entry() { StartVelocity = speed, Gradient = gradient, EndVelocity = vehicleSpeed });
				}
			}

			return tmp.ToArray();
		}

		public IList<MeterPerSecond> Speeds { get; }

		protected MeterPerSecond SimulateRollingVehicle(
			Vehicle vehicle, Radian gradient, IVehicleContainer container)
		{
			var simulationInterval = Constants.SimulationSettings.TargetTimeInterval;

			var acceleration = 0.SI<MeterPerSquareSecond>();
			var absTime = 0.SI<Second>();
			while (absTime < TractionInterruption) {
				var initialResponse = vehicle.Request(absTime, simulationInterval, acceleration, gradient);
				var delta = initialResponse.GearboxPowerRequest;
				try {
					var time = absTime;
					acceleration = SearchAlgorithm.Search(
						acceleration, delta, Constants.SimulationSettings.OperatingPointInitialSearchIntervalAccelerating,
						getYValue: response => {
							var r = (ResponseDryRun)response;
							return r.GearboxPowerRequest;
						},
						evaluateFunction: acc => {
							var response = vehicle.Request(time, simulationInterval, acc, gradient, true);
							response.Acceleration = acc;
							return response;
						},
						criterion: response => {
							var r = (ResponseDryRun)response;
							return r.GearboxPowerRequest.Value();
						},
						abortCriterion: (response, cnt) => {
							var r = (ResponseDryRun)response;
							return r != null && (vehicle.VehicleSpeed + r.Acceleration * simulationInterval) < 0.KMPHtoMeterPerSecond();
						}
					);
					var step = vehicle.Request(absTime, simulationInterval, acceleration, gradient);
					if (!(step is ResponseSuccess)) {
						throw new VectoSimulationException("failed to find acceleration for rolling");
					}

					absTime += simulationInterval;
				} catch (VectoSearchAbortedException) {
					return 0.KMPHtoMeterPerSecond();
				}

				container.CommitSimulationStep(absTime, simulationInterval);
			}

			return vehicle.VehicleSpeed;
		}

		public class Entry
		{
			public MeterPerSecond StartVelocity;
			public Radian Gradient;
			public MeterPerSecond EndVelocity;
		}
	}
}
