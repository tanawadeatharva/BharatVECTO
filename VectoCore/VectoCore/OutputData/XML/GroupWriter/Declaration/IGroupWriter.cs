using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.GroupWriter.Declaration
{
    public interface IVehicleDeclarationGroupWriter
	{
		XElement[] GetGroupElements(IVehicleDeclarationInputData vehicle);
	}

	public interface IBusAuxiliariesDeclarationGroupWriter
	{
		XElement[] GetGroupElements(IBusAuxiliariesDeclarationData aux);
	}
}
