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

		IEngineeringInputDataProvider CreateEngineering(string filename, bool verifyXML);

		IEngineeringInputDataProvider CreateEngineering(Stream inputData, bool verifyXML);

		IEngineeringInputDataProvider CreateEngineering(XmlReader inputData, bool verifyXML);

		IDeclarationInputDataProvider CreateDeclaration(string filename, bool verifyXML);

		IDeclarationInputDataProvider CreateDeclaration(XmlReader inputData, bool verifyXML);
	}
}
