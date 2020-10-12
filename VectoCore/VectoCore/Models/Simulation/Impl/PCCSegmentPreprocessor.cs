using System;
using System.Collections.Generic;
using System.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.SimulationComponent.Data;
using TUGraz.VectoCore.Models.SimulationComponent.Impl;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.Simulation.Impl
{
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
					Container, slopes, PCCDriverData.MinSpeed,
					VectoMath.Min(Container.VehicleInfo.MaxVehicleSpeed, Container.RunData.Cycle.Entries.Max(x => x.VehicleTargetSpeed)))
				.RunPreprocessing();

			var runData = Container.RunData;
			var engineDrag = runData.EngineData.FullLoadCurves[0].FullLoadEntries
									.Average(x => (x.EngineSpeed * x.TorqueDrag).Value()).SI<Watt>();

			var slopeEngineDrag = 0.0;
			if (runData.GearboxData.Type.AutomaticTransmission()) {
				if ((runData.VehicleData.ADAS.EcoRoll != EcoRollType.None && runData.GearboxData.ATEcoRollReleaseLockupClutch) ||
					runData.VehicleData.ADAS.EcoRoll == EcoRollType.None) {
					slopeEngineDrag = (engineDrag / Physics.GravityAccelleration / runData.VehicleData.TotalVehicleMass).Value();
				}
			} else {
				if (runData.VehicleData.ADAS.EcoRoll == EcoRollType.None) {
					slopeEngineDrag = (engineDrag / Physics.GravityAccelleration / runData.VehicleData.TotalVehicleMass).Value();
				}
			}

			//runData.VehicleData.ADAS.EcoRoll != EcoRollType.None
			//	? 0
			//	: (engineDrag / Physics.GravityAccelleration / runData.VehicleData.TotalVehicleWeight).Value();

			PCCSegment pccSegment = null;
			var targetspeedChanged = 0.SI<Meter>();
			foreach (var tuple in Container.RunData.Cycle.Entries.Pairwise(Tuple.Create)) {
				if (!tuple.Item1.Highway) {
					pccSegment = null;
					continue;
				}

				if (!tuple.Item1.VehicleTargetSpeed.IsGreaterOrEqual(PCCDriverData.PCCEnableSpeed)) {
					// only consider pcc segments where the target speed is at least the pcc-enable speed
					targetspeedChanged = tuple.Item2.Distance;
					pccSegment = null;
					continue;
				}

				if (tuple.Item1.Distance.IsEqual(tuple.Item2.Distance)) {
					// can't calculate avg slope if difference between two entries is 0
					continue;
				}

				if (!tuple.Item1.VehicleTargetSpeed.IsEqual(tuple.Item2.VehicleTargetSpeed)) {
					// target speed must not change within PCC segment
					targetspeedChanged = tuple.Item2.Distance;
					pccSegment = null;
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

				if (pccSegment == null && slope < minSlope) {
					pccSegment = new PCCSegment() {
						DistanceMinSpeed = tuple.Item1.Distance,
						StartDistance =
							tuple.Item1.Distance - VectoMath.Min(
								tuple.Item1.Distance - targetspeedChanged - 1.SI<Meter>(),
								PCCDriverData.PreviewDistanceUseCase1), // PCCDriverData.PreviewDistanceUseCase1,
						TargetSpeed = tuple.Item1.VehicleTargetSpeed,
						Altitude = tuple.Item1.Altitude,
						EnergyMinSpeed = (runData.VehicleData.TotalVehicleMass * Physics.GravityAccelleration * tuple.Item1.Altitude)
										.Cast<Joule>() +
										runData.VehicleData.TotalVehicleMass * (tuple.Item1.VehicleTargetSpeed - PCCDriverData.UnderSpeed) *
										(tuple.Item1.VehicleTargetSpeed - PCCDriverData.UnderSpeed) / 2,
					};
				}

				if (pccSegment != null && slope > minSlope) {
					pccSegment.EndDistance = tuple.Item1.Distance;
					pccSegment.EnergyEnd =
						(runData.VehicleData.TotalVehicleMass * Physics.GravityAccelleration * tuple.Item1.Altitude).Cast<Joule>() +
						runData.VehicleData.TotalVehicleMass * tuple.Item1.VehicleTargetSpeed *
						tuple.Item1.VehicleTargetSpeed / 2;
					PCCSegments.Segments.Add(pccSegment);
					pccSegment = null;
				}
			}
		}

		#endregion
	}
}
