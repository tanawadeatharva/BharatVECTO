using System.IO;
using System.Xml;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.InputData.FileIO.XML
{
	public interface IXMLInputDataReader
	{
		IInputDataProvider Create(string filename, bool verifyXML);

		IInputDataProvider Create(Stream inputData, bool verifyXML);

		IInputDataProvider Create(XmlReader inputData, bool verifyXML);

		IEngineeringInputDataProvider CreateEngineering(string filename);

		IEngineeringInputDataProvider CreateEngineering(Stream inputData);

		IEngineeringInputDataProvider CreateEngineering(XmlReader inputData);

		IDeclarationInputDataProvider CreateDeclaration(string filename);

		IDeclarationInputDataProvider CreateDeclaration(XmlReader inputData);
	}
}
