using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLAuxiliariesDeclarationInputData : IAuxiliariesDeclarationInputData
	{
		IXMLAuxiliaryReader Reader { set; }
	}

	public interface IXMLAuxiliaryDeclarationInputData : IAuxiliaryDeclarationInputData
	{
		
	}
}
