using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public interface IXMLJobDataReader {
		IEngineEngineeringInputData CreateEngineOnly { get; }

		IVehicleEngineeringInputData CreateVehicle { get; }

		IXMLCyclesDataProvider CreateCycles { get; }
	}
}