using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering
{
	public interface IXMLComponentsReader
	{
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

		ITyreEngineeringInputData Tyre { get; }

		IVehicleComponentsEngineering ComponentInputData { get; }

		IAxleEngineeringInputData CreateAxle(XmlNode axleNode);

		ITransmissionInputData CreateGear(XmlNode gearNode);

		IAuxiliaryEngineeringInputData Create(XmlNode auxNode);
	}
}
