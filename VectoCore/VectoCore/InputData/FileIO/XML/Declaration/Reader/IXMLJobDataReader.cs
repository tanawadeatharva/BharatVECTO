using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader
{
	public interface IXMLJobDataReader
	{
		IVehicleDeclarationInputData CreateVehicle { get; }
	}
}
