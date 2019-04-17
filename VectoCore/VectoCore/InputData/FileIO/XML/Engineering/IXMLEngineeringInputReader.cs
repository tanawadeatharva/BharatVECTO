using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public interface IXMLEngineeringInputReader
	{
		IEngineeringJobInputData JobData { get; }

		IDriverEngineeringInputData DriverModel { get; }

	}
}