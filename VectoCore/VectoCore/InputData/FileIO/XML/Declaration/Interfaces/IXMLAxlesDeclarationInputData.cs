using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces {
	public interface IXMLAxlesDeclarationInputData : IAxlesDeclarationInputData, IXMLResource
	{
		IXMLComponentReader Reader { set; }
	}
}