using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLEngineeringVehicleComponentsData : IVehicleComponentsEngineering, IXMLResource {
		IXMLComponentsReader ComponentReader { set; }
	}
}
