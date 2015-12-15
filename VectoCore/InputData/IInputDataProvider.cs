using System.Collections.Generic;
using System.Deployment.Internal;

namespace TUGraz.VectoCore.InputData
{
	public interface IInputDataProvider
	{
		IJobInputData JobInputData();

		IVehicleInputData VehicleInputData { get; }

		IGearboxInputData GearboxInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IEngineInputData EngineInputData { get; }

		IAuxiliariesInputData AuxiliaryInputData();

		IRetarderInputData RetarderInputData { get; }

		IDriverInputData DriverInputData { get; }
	}
}