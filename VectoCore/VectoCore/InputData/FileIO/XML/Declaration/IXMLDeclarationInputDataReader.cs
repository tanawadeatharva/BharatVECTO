using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration
{
	public interface IXMLDeclarationInputDataReader
	{
		IDeclarationJobInputData JobData { get; }
	}
}
