using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces
{
	public interface IXMLDeclarationVehicleData : IVehicleDeclarationInputData, IXMLResource
	{
		IXMLComponentReader ComponentReader { set; }

		IXMLPTOReader PTOReader { set; }

		IXMLADASReader ADASReader { set; }

		AngledriveType AngledriveType { get; }

		RetarderType RetarderType { get; }

		double RetarderRatio { get; }
		IPTOTransmissionInputData PTOTransmissionInputData { get; }
	}
}
