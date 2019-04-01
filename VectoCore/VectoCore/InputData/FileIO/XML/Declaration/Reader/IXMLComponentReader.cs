using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader
{
	public interface IXMLComponentReader
	{
		IVehicleComponentsDeclaration ComponentInputData { get; }

		IAirdragDeclarationInputData AirdragInputData { get; }
		IGearboxDeclarationInputData GearboxInputData { get; }
		IAxleGearInputData AxleGearInputData { get; }
		IAngledriveInputData AngledriveInputData { get; }
		IEngineDeclarationInputData EngineInputData { get; }
		IAuxiliariesDeclarationInputData AuxiliaryData { get; }
		IRetarderInputData RetarderInputData { get; }
		IAxlesDeclarationInputData AxlesDeclarationInputData { get; }

		ITorqueConverterDeclarationInputData TorqueConverterInputData { get; }
		ITransmissionInputData CreateGear(XmlNode gearNode);
		IAuxiliaryDeclarationInputData CreateAuxiliary(XmlNode auxNode);
		IAxleDeclarationInputData CreateAxle(XmlNode axleNode);

		ITyreDeclarationInputData Tyre { get; }
	}
}
