using System;
using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.Simulation.DataBus;
using TUGraz.VectoCore.Models.Simulation.Impl;
using TUGraz.VectoCore.Models.SimulationComponent.Data.Gearbox;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class MaxGradabilityPreprocessor : ISimulationPreprocessor
	{
		protected MaxGradabilityLookup GradabilityLookup;
		protected VectoRunData ModelData;

		public MaxGradabilityPreprocessor(MaxGradabilityLookup maxGradability, VectoRunData modelData)
		{
			GradabilityLookup = maxGradability;
			ModelData = modelData;
		}


		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing(VectoRun container)
		{
			GradabilityLookup.Data = SearchMaxRoadGradient(container);
		}

		private Dictionary<uint, Tuple<Radian, Radian>> SearchMaxRoadGradient(VectoRun run)
		{
			var container = run.GetContainer() as VehicleContainer;
			var vehicle = container?.Vehicle as Vehicle;
			if (vehicle == null) {
				throw new VectoException("no vehicle found...");
			}

			var gearbox = container.Gearbox as Gearbox;
			if (gearbox == null) {
				throw new VectoException("no gearbox found...");
			}

			var driver = container.Driver as Driver;
			if (driver == null) {
				throw new VectoException("no driver found...");
			}
			driver.DriverBehavior = DrivingBehavior.Driving;

			var retVal = new Dictionary<uint, Tuple<Radian, Radian>>();
			foreach (var gearData in ModelData.GearboxData.Gears) {
				var engineSpeed = ModelData.EngineData.FullLoadCurves[0].NTq99lSpeed;
				var vehicleSpeed = CalcVehicleSpeed(engineSpeed, gearData);
				gearbox.Gear = gearData.Key;
				var gradMaxTorque = SearchGradient(vehicle, vehicleSpeed, ModelData.EngineData.FullLoadCurves[0].MaxTorque * 0.99);

				engineSpeed = (ModelData.EngineData.FullLoadCurves[0].NTq99lSpeed +
								ModelData.EngineData.FullLoadCurves[0].NTq99hSpeed) / 2.0;
				vehicleSpeed = CalcVehicleSpeed(engineSpeed, gearData);
				var torqueFactor = ModelData.GearshiftParameters.ShareTorque99L.Lookup(vehicleSpeed);
				var gradRedTorque = SearchGradient(vehicle, vehicleSpeed, ModelData.EngineData.FullLoadCurves[0].FullLoadStationaryTorque(engineSpeed) * torqueFactor);

				retVal.Add(gearData.Key, Tuple.Create(gradMaxTorque, gradRedTorque));
			}

			return retVal;
		}

		private Radian SearchGradient(Vehicle vehicle, MeterPerSecond vehicleSpeed, NewtonMeter maxTorque)
		{
			var gradient = VectoMath.InclinationToAngle(0);
			var response = vehicle.Initialize(vehicleSpeed, gradient);

			var delta = response.EnginePowerRequest / response.EngineSpeed - maxTorque;
			gradient = SearchAlgorithm.Search(
				gradient, delta, VectoMath.InclinationToAngle(1),
				getYValue: r => {
					var rs = r as ResponseSuccess;
					if (rs != null) {
						return rs.EnginePowerRequest / rs.EngineSpeed - maxTorque;
					}

					return 0.SI<NewtonMeter>();
				},
				evaluateFunction: g => {
					return vehicle.Initialize(vehicleSpeed, g);
				},
				criterion: r => {
					var rs = r as ResponseSuccess;
					if (rs != null) {
						return (rs.EnginePowerRequest / rs.EngineSpeed - maxTorque).Value();
					}
					return 0;
				}

			);

			return gradient;
		}

		private MeterPerSecond CalcVehicleSpeed(PerSecond engineSpeed, KeyValuePair<uint, GearData> gearData)
		{
			return engineSpeed / gearData.Value.Ratio / ModelData.AxleGearData.AxleGear.Ratio /
					(ModelData.AngledriveData?.Angledrive.Ratio ?? 1.0) * ModelData.VehicleData.DynamicTyreRadius;
		}

		#endregion
	}
}