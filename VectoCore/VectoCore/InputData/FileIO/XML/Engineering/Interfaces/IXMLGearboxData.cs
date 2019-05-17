using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLGearboxData : IGearboxEngineeringInputData
	{
		IXMLGearboxReader Reader { set; }
	}
}
