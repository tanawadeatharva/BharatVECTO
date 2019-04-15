using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public interface IXMLComponentsReader
	{
		IVehicleComponentsEngineering ComponentInputData { get; }

		IAxleGearInputData AxleGearInputData { get; }

		IAngledriveInputData AngularGearInputData { get; }

		IEngineEngineeringInputData EngineInputData { get; }

		IRetarderInputData RetarderInputData { get; }

		IAuxiliariesEngineeringInputData AuxiliaryData { get; }

		IGearboxEngineeringInputData GearboxData { get; }

		ITorqueConverterEngineeringInputData TorqueConverter { get; }

		IPTOTransmissionInputData PTOData { get; }

		IAirdragEngineeringInputData AirdragInputData { get; }

		IAxlesEngineeringInputData AxlesEngineeringInputData { get; }

	}

	public interface IXMLAxlesReader
	{
		IAxleEngineeringInputData CreateAxle(XmlNode axleNode);
	}

	public interface IXMLAxleReader
	{
		ITyreEngineeringInputData Tyre { get; }
	}

	public interface IXMLGearboxReader
	{
		ITransmissionInputData CreateGear(XmlNode gearNode);
	}

	public interface IXMLAuxiliaryReader
	{
		IAuxiliaryEngineeringInputData CreateAuxiliary(XmlNode auxNode);
	}
}
