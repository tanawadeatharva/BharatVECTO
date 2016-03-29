using TUGraz.VectoCore.Models.SimulationComponent.Data.Engine;
using TUGraz.VectoCore.Utils;
using VectoAuxiliaries;

namespace TUGraz.VectoCore.Models.SimulationComponent.Impl
{
	public class BusAuxiliariesAdapter : IEngineAuxPort
	{
		protected IAdvancedAuxiliaries Auxiliaries;


		public BusAuxiliariesAdapter(string aauxFile, string cycleName, Kilogram vehicleWeight, FuelConsumptionMap fcMap,
			PerSecond engineIdleSpeed)
		{
			// 'Set Statics
			Auxiliaries.VectoInputs.Cycle = DetermineCycle(cycleName);
			Auxiliaries.VectoInputs.VehicleWeightKG = (float)vehicleWeight.Value();
			Auxiliaries.VectoInputs.FuelMap = new FuelConsumptionAdapter() { FcMap = fcMap };
			Auxiliaries.VectoInputs.FuelDensity = Physics.FuelDensity.Value();

			//'Set Signals
			Auxiliaries.Signals.TotalCycleTimeSeconds = 3600; // TODO MQ: get cycle time somehow!
			Auxiliaries.Signals.EngineIdleSpeed = (float)engineIdleSpeed.Value();
			Auxiliaries.RunStart(aauxFile, "");
		}

		private string DetermineCycle(string cycleName)
		{
			return "Coach";
			//			Public Function DetermineCycleNameFromCurrentFile() As String

			//	'Get DriveFile without path and without extension
			//	Dim driveFile As String = fFILE(CurrentCycleFile, False)

			//	Select Case (True)

			//		'DJN - update to make contains test case insensitive
			//		Case driveFile.ToLower().Contains("heavy_urban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Heavy urban"

			//		Case driveFile.ToLower().Contains("suburban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Suburban"

			//		Case driveFile.ToLower().Contains("urban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Urban"

			//		Case driveFile.ToLower().Contains("interurban") AndAlso driveFile.ToLower().Contains("bus")
			//			Return "Interurban"

			//		Case driveFile.ToLower().Contains("coach")
			//			Return "Coach"

			//		Case Else
			//			WorkerMsg(tMsgID.Warn,
			//					String.Format("UnServiced Cycle Name '{0}' in Pneumatics Actuations Map 0 Actuations returned", driveFile),
			//					"Advanced Auxiliaries")
			//			Return "UnknownCycleName"

			//	End Select


			//	Return "Urban"
			//End Function
		}

		public NewtonMeter Initialize(NewtonMeter torque, PerSecond angularSpeed)
		{
			throw new System.NotImplementedException();
		}

		public NewtonMeter PowerDemand(Second absTime, Second dt, NewtonMeter torque, PerSecond angularSpeed,
			bool dryRun = false)
		{
			throw new System.NotImplementedException();
		}

		protected class FuelConsumptionAdapter : IFuelConsumptionMap
		{
			protected internal FuelConsumptionMap FcMap;

			public double GetFuelConsumption(double torque, double angularVelocity)
			{
				return FcMap.GetFuelConsumption(torque.SI<NewtonMeter>(), angularVelocity.SI<PerSecond>()).Value();
			}
		}
	}
}