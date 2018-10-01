using System.Collections.Generic;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Connector.Ports.Impl;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class MaxCardanTorquePreprocessor : ISimulationPreprocessor
	{
		protected readonly MaxCardanTorqueLookup MaxCardanTorqueLookup;
		protected readonly VectoRunData Data;
		protected readonly SimplePowertrainContainer TestContainer;

		public MaxCardanTorquePreprocessor(MaxCardanTorqueLookup maxCardanTorqueLookup, VectoRunData data, SimplePowertrainContainer testContainer)
		{
			MaxCardanTorqueLookup = maxCardanTorqueLookup;
			Data = data;
			TestContainer = testContainer;
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var retVal = new Dictionary<uint, List<KeyValuePair<PerSecond, NewtonMeter>>>(Data.GearboxData.Gears.Count);

			TestContainer.GearboxCtl.Gear = 1;
			var init = TestContainer.VehiclePort.Initialize(Data.GearshiftParameters.StartVelocity, 0.SI<Radian>());
			var powertrainRatioWOGearbox = (Data.GearshiftParameters.StartVelocity / init.EngineSpeed / Data.GearboxData.Gears[1].Ratio).Cast<Meter>();

			var engineSpeedSteps = (Data.EngineData.FullLoadCurves[0].N95hSpeed - Data.EngineData.IdleSpeed) / 250;

			foreach (var gearData in Data.GearboxData.Gears) {
				retVal[gearData.Key] = new List<KeyValuePair<PerSecond, NewtonMeter>>();
				TestContainer.GearboxCtl.Gear = gearData.Key;
				for (var engineSpeed = Data.EngineData.IdleSpeed;
					engineSpeed < Data.EngineData.FullLoadCurves[0].N95hSpeed;
					engineSpeed += engineSpeedSteps) {
					var maxTorque = Data.EngineData.FullLoadCurves[gearData.Key].FullLoadStationaryTorque(engineSpeed);
					var vehicleSpeed = engineSpeed * powertrainRatioWOGearbox * gearData.Value.Ratio;
					var first = TestContainer.VehiclePort.Initialize(vehicleSpeed, 0.SI<Radian>());
					var delta = first.EngineTorqueDemandTotal - maxTorque;
					var grad = SearchAlgorithm.Search(
						0.SI<Radian>(), delta, 0.1.SI<Radian>(),
						getYValue: r => {
							return ((r as AbstractResponse).EngineTorqueDemandTotal - maxTorque);
						},
						evaluateFunction: g => {
							return TestContainer.VehiclePort.Initialize(vehicleSpeed, g);
						},
						criterion: r => {
							return ((r as AbstractResponse).EngineTorqueDemandTotal - maxTorque).Value();
						}
					);
					var max = TestContainer.VehiclePort.Initialize(vehicleSpeed, grad);
					retVal[gearData.Key].Add(new KeyValuePair<PerSecond, NewtonMeter>(max.EngineSpeed, max.CardanTorque));
				}
			}

			MaxCardanTorqueLookup.Data = retVal;
		}

		#endregion
	}
}