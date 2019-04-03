using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLAuxiliairesData : IAuxiliariesEngineeringInputData
	{
		IXMLComponentsReader Reader { set; }
	}
}
