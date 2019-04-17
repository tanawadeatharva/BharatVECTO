using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces
{
	public interface IXMLEngineeringVehicleData : IVehicleEngineeringInputData, IPTOTransmissionInputData, IXMLResource
	{
		IXMLComponentsReader ComponentReader { set; }

		XmlElement ComponentNode { get; }

		AngledriveType AngledriveType { get; }

		RetarderType RetarderType { get; }

		double RetarderRatio { get; }

		IXMLEngineeringJobInputData Job { get; }
		
	}
}
