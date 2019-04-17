using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationJobInputData : IDeclarationJobInputData, IXMLResource
	{
		IXMLJobDataReader Reader { set; }

		IXMLDeclarationInputData InputData { get; }
	}
}
