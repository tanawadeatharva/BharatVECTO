using System.Xml;
using System.Xml.Schema;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationVehicleData : IVehicleDeclarationInputData, IXMLResource
	{
		XmlElement ComponentNode { get; }

		IXMLComponentReader ComponentReader { set; }

		XmlElement PTONode { get; }

		IXMLPTOReader PTOReader { set; }

		XmlElement ADASNode { get; }

		IXMLADASReader ADASReader { set; }

		AngledriveType AngledriveType { get; }

		RetarderType RetarderType { get; }

		double RetarderRatio { get; }
		IPTOTransmissionInputData PTOTransmissionInputData { get; }
	}
}
