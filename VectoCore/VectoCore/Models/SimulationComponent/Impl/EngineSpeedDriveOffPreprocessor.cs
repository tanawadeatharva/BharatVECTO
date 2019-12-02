using System.Collections.Generic;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Simulation;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl {
	public class EngineSpeedDriveOffPreprocessor : ISimulationPreprocessor
	{
		protected VectoRunData Data;
		protected SimplePowertrainContainer Container;
		protected Dictionary<uint, PerSecond> engineSpeeds;

		public EngineSpeedDriveOffPreprocessor(Dictionary<uint, PerSecond> engineSpeedAtDriveOff, VectoRunData runData, SimplePowertrainContainer testContainer)
		{
			Data = runData;
			Container = testContainer;
			engineSpeeds = engineSpeedAtDriveOff;
		}

		#region Implementation of ISimulationPreprocessor

		public void RunPreprocessing()
		{
			var vehicle = Container.Vehicle as Vehicle;

			if (vehicle == null) {
				throw new VectoException("no vehicle found...");
			}

			var gearbox = Container.Gearbox as Gearbox;
			if (gearbox == null) {
				throw new VectoException("no gearbox found...");
			}

			// issue a initialize and request call in order to avoid a slipping clutch when looping over all gears
			gearbox.Disengaged = false;
			gearbox.Gear = 1;
			vehicle.Initialize(Data.GearshiftParameters.StartVelocity, VectoMath.InclinationToAngle(0));
			vehicle.Request(0.SI<Second>(), 0.5.SI<Second>(), 0.SI<MeterPerSquareSecond>(), 0.SI<Radian>());
			foreach (var gearData in Data.GearboxData.Gears) {
				gearbox.Gear = gearData.Key;
				gearbox.Disengaged = false;
				var response = vehicle.Initialize(Data.GearshiftParameters.StartVelocity, VectoMath.InclinationToAngle(0));
				engineSpeeds[gearData.Key] = response.EngineSpeed;
			}
		}

		#endregion
	}
}