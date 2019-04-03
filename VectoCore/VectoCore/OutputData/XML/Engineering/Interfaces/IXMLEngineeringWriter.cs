using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces {
	public interface IXMLEngineeringWriter
	{
		XNamespace RegisterNamespace(string namespaceUri);

		string GetNSPrefix(string xmlns);

		XDocument Write(IEngineeringInputDataProvider inputData);

		XDocument Write(IInputDataProvider inputData);

		XDocument WriteComponent<T>(T componentInputData) where T : class, IComponentInputData;

		WriterConfiguration Configuration { get; set; }

		string GetFilename<T>(T componentData, string suffix = null) where T:IComponentInputData;

		string RemoveInvalidFileCharacters(string filename);
	}

	public class WriterConfiguration
	{
		public bool SingleFile { get; set; }
		public string BasePath { get; set; }
	}
}