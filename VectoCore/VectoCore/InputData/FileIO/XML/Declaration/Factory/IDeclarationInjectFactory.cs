using System.Xml;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Interfaces;
using TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Reader;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Declaration.Factory
{
	public interface IDeclarationInjectFactory
	{
		/*
		 * Ninject automatically creates a factory class for this interface
		 * 
		 * the first argument is used to lookup the named binding for the required return type, the remaining
		 * parameters are the constructor arguments
		 * 
		 */

		IXMLDeclarationInputData CreateInputProvider(string version, XmlDocument xmlDoc, string fileName);

		IXMLDeclarationJobInputData CreateJobData(
			string version, XmlNode node, IXMLDeclarationInputData inputProvider, string fileName);

		IXMLDeclarationVehicleData CreateVehicleData(
			string version, IXMLDeclarationJobInputData jobData, XmlNode xmlNode, string sourceFile);

		IXMLVehicleComponentsDeclaration CreateComponentData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);


		IXMLAirdragDeclarationInputData CreateAirdragData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLGearboxDeclarationInputData CreateGearboxData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLGearData CreateGearData(string version, XmlNode gearNode, string sourceFile);


		IXMLTorqueConverterDeclarationInputData CreateTorqueconverterData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAxleGearInputData CreateAxlegearData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAngledriveInputData CreateAngledriveData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLEngineDeclarationInputData CreateEngineData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLRetarderInputData CreateRetarderData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAuxiliariesDeclarationInputData CreateAuxiliariesData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLPTOTransmissionInputData CreatePTOData(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAxlesDeclarationInputData CreateAxleWheels(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAxleDeclarationInputData CreateAxleData(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode,  string sourceFile);

		IXMLTyreDeclarationInputData CreateTyre(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);

		IXMLAuxiliaryDeclarationInputData CreateAuxiliaryData(string version, XmlNode auxNode, IXMLDeclarationVehicleData vehicle);

		IXMLAdvancedDriverAssistantSystemDeclarationInputData CreateADASData(string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, string sourceFile);


		// ---------------------------------------------------------------------------------------------

		IXMLDeclarationInputDataReader CreateInputReader(
			string version, IXMLDeclarationInputData inputData, XmlNode baseNode, bool verifyXML);

		IXMLJobDataReader CreateJobReader(
			string version, IXMLDeclarationJobInputData jobData, XmlNode jobNode, bool verifyXML);

		IXMLComponentReader CreateComponentReader(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentsNode, bool verifyXML);

		IXMLADASReader CreateADASReader(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode vehicleNode, bool verifyXML);

		IXMLPTOReader CreatePTOReader(
			string version, IXMLDeclarationVehicleData vehicle, XmlNode componentNode, bool verifyXML);
	}

}
