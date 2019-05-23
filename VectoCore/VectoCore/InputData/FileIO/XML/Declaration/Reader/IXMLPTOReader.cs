using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader
{
	public interface IXMLPTOReader
	{
		IPTOTransmissionInputData PTOInputData { get; }
	}
}
