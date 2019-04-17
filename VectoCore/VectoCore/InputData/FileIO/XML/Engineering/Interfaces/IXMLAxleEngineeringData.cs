using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLAxleEngineeringData : IAxleEngineeringInputData, IXMLResource
	{
		IXMLAxleReader Reader { set; }
	}
}
