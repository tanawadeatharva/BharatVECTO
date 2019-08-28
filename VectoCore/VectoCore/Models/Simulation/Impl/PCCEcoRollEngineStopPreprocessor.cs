using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Configuration;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
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
		private Dictionary<MeterPerSecond, Radian> SlopeData;

		public PCCEcoRollEngineStopPreprocessor(
			SimplePowertrainContainer simpleContainer, Dictionary<MeterPerSecond, Radian> slopeData, MeterPerSecond minSpeed,
			MeterPerSecond maxSpeed)
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
			SlopeData.Clear();

			for (var speed = MinSpeed; speed <= MaxSpeed; speed += SpeedStep) {
				var gear = FindLowestGearForSpeed(speed);
				gearbox.Gear = gear;
				gearbox.DisengageGearbox = true;
				gearbox._nextGear = new GearInfo(gear, true);
				vehicle.Initialize(speed, 0.SI<Radian>());

				var slope = SearchSlope(vehicle, Container);

				modData?.Reset();
				SlopeData[speed] = slope;
			}
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
					evaluateFunction: grad => { return vehicle.Request(absTime, simulationInterval, acceleration, grad, true); },
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

	public class PCCSegmentPreprocessor : ISimulationPreprocessor
	{
		protected SimplePowertrainContainer Container;
		protected PCCSegments PCCSegments;
		protected DriverData.PCCData PCCDriverData;

		public PCCSegmentPreprocessor(
			SimplePowertrainContainer simpleContainer, PCCSegments segments, DriverData.PCCData driverDataPCC)
		{
			Container = simpleContainer;
			PCCSegments = segments;
			PCCDriverData = driverDataPCC;
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var slopes = new Dictionary<MeterPerSecond, Radian>();
			new PCCEcoRollEngineStopPreprocessor(
				Container, slopes, PCCDriverData.MinSpeed, PCCDriverData.MaxSpeed).RunPreprocessing();

			var runData = Container.RunData;
			var engineDrag = runData.EngineData.FullLoadCurves[0].FullLoadEntries
									.Average(x => (x.EngineSpeed * x.TorqueDrag).Value()).SI<Watt>();

			var slopeEngineDrag = runData.VehicleData.ADAS.EcoRoll == EcoRollType.WithEngineStop
				? 0
				: (engineDrag / Physics.GravityAccelleration / runData.VehicleData.TotalVehicleWeight).Value();

			PCCSegment pccSegment = null;
			var targetspeedChanged = 0.SI<Meter>();
			foreach (var tuple in Container.RunData.Cycle.Entries.Pairwise(Tuple.Create)) {
				if (!tuple.Item1.Highway) {
					continue;
				}
				if (tuple.Item1.Distance.IsEqual(tuple.Item2.Distance)) {
					// can't calculate avg slope if difference between two entries is 0
					continue;
				}

				if (!tuple.Item1.VehicleTargetSpeed.IsEqual(tuple.Item2.VehicleTargetSpeed)) {
					// target speed must not change within PCC segment
					targetspeedChanged = tuple.Item2.Distance;
					continue;
				}

				if (pccSegment != null && !tuple.Item1.VehicleTargetSpeed.IsEqual(pccSegment.TargetSpeed)) {
					// target speed must not change within PCC segment
					pccSegment = null;
					continue;
				}

				var minSlope = (slopes.Interpolate(x => x.Key.Value(), y => y.Value.Value(), tuple.Item1.VehicleTargetSpeed.Value())
								+ slopeEngineDrag / tuple.Item1.VehicleTargetSpeed.Value()).SI<Radian>();

				var slope = VectoMath.InclinationToAngle(
					(tuple.Item2.Altitude - tuple.Item1.Altitude) / (tuple.Item2.Distance - tuple.Item1.Distance));

				if (pccSegment == null && slope < minSlope &&
					(tuple.Item1.Distance - targetspeedChanged).IsGreater(PCCDriverData.PreviewDistance)) {
					pccSegment = new PCCSegment() {
						DistanceMinSpeed = tuple.Item1.Distance,
						StartDistance = tuple.Item1.Distance - PCCDriverData.PreviewDistance,
						TargetSpeed = tuple.Item1.VehicleTargetSpeed,
						Altitude = tuple.Item1.Altitude,
						EnergyMinSpeed = (runData.VehicleData.TotalVehicleWeight * Physics.GravityAccelleration * tuple.Item1.Altitude)
										.Cast<Joule>() +
										runData.VehicleData.TotalVehicleWeight * (tuple.Item1.VehicleTargetSpeed - PCCDriverData.UnderSpeed) *
										(tuple.Item1.VehicleTargetSpeed - PCCDriverData.UnderSpeed) / 2,
					};
				}

				if (pccSegment != null && slope > minSlope) {
					pccSegment.EndDistance = tuple.Item1.Distance;
					pccSegment.EnergyEnd =
						(runData.VehicleData.TotalVehicleWeight * Physics.GravityAccelleration * tuple.Item1.Altitude).Cast<Joule>() +
						runData.VehicleData.TotalVehicleWeight * tuple.Item1.VehicleTargetSpeed *
						tuple.Item1.VehicleTargetSpeed / 2;
					PCCSegments.Segments.Add(pccSegment);
					pccSegment = null;
				}
			}
		}

		#endregion
	}

	public class PCCSegments
	{
		public PCCSegments()
		{
			Segments = new List<PCCSegment>();
			CurrentIdx = 0;
		}

		public void MoveNext()
		{
			CurrentIdx = CurrentIdx + 1;
			if (CurrentIdx >= Count) {
				CurrentIdx = Count - 1;
			}
		}

		public int CurrentIdx { get; private set; }

		public int Count {  get { return Segments.Count; } }

		public PCCSegment Current { get { return Segments.Any() ? Segments[CurrentIdx] : null; } }

		public List<PCCSegment> Segments { get; }
	}

	public class PCCSegment
	{
		public Meter StartDistance { get; set; }

		public Meter DistanceMinSpeed { get; set; }
		public Meter EndDistance { get; set; }
		public MeterPerSecond TargetSpeed { get; set; }
		public Meter Altitude { get; set; }

		public Joule EnergyMinSpeed { get; set; }
		public Joule EnergyEnd { get; set; }
	}
}
