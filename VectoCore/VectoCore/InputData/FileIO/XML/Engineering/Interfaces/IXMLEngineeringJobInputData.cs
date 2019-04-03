using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLEngineeringJobInputData : IEngineeringJobInputData, IXMLResource
	{
		IXMLJobDataReader Reader { set; }

		IXMLEngineeringInputData InputData { get; }
	}
}
