using System.Xml;
using TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Interfaces;

namespace TUGraz.VectoCore.InputData.FileIO.XML.Engineering.Factory
{
	public interface IEngineeringInjectFactory
	{
		IXMLEngineeringInputData CreateInputProvider(string version, XmlDocument xmldoc, string fileName);

		IXMLEngineeringInputReader CreateInputReader(
			string version, IXMLEngineeringInputData inputData, XmlNode documentElement, bool verifyXml);
	}
}
