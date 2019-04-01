using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLVehicleComponentsDeclaration : IVehicleComponentsDeclaration, IXMLResource
	{
		IXMLComponentReader ComponentReader { set; }

		//IXMLADASReader ADASReader { set; }

		//IXMLPTOReader PTOReader { set; }
	}

	public interface IXMLAdvancedDriverAssistantSystemDeclarationInputData :
		IAdvancedDriverAssistantSystemDeclarationInputData,
		IXMLResource { }
}
