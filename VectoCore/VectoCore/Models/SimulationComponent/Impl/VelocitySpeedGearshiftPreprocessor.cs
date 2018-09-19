using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class VelocitySpeedGearshiftPreprocessor : ISimulationPreprocessor
	{
		protected readonly Second TractionInterruption;
		private readonly VelocityRollingLookup VehicleVelocityDropLookup;

		public VelocitySpeedGearshiftPreprocessor(VelocityRollingLookup velocityDropData, Second tracktionInterruption, int minGradient = -24, int maxGradient = 24, int gradientStep = 2)
		{
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

		public void RunPreprocessing(VectoRun container)
		{
			VehicleVelocityDropLookup.Data = IterateVehicleSpeedAndGradient(container);
		}

		#endregion

		public int MinGradient { get; set; }

		public int MaxGradient { get; set; }
		public int GradientStep { get; set; }

		protected Entry[] IterateVehicleSpeedAndGradient(VectoRun run)
		{
			var container = run.GetContainer() as VehicleContainer;
			var vehicle = container?.Vehicle as Vehicle;

			if (vehicle == null) {
				throw new VectoException("no vehicle found...");
			}

			var modData = container.ModalData as ModalDataContainer;

			var tmp = new List<Entry>();
			foreach (var speed in Speeds) {
				for (var grad = MinGradient; grad <= MaxGradient; grad+=GradientStep) {
					var gradient = VectoMath.InclinationToAngle(grad / 100.0);
					vehicle.Initialize(speed, gradient);
					(container.Gearbox as Gearbox).Gear = 0;
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