using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLAxlesData : IAxlesEngineeringInputData
	{
		IXMLComponentsReader Reader { set; }
	}
}
