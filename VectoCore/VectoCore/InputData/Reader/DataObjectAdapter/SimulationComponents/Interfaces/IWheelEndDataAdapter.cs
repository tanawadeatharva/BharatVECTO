using System.Collections.Generic;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.SimulationComponent.Data;

namespace TUGraz.VectoCore.InputData.Reader.DataObjectAdapter.SimulationComponents.Interfaces
{
	public interface IWheelEndDataAdapter
	{
		WheelEndData CreateWheelEndData(VehicleClass vehicleClass, IList<IAxleDeclarationInputData> axles);
	}
}
