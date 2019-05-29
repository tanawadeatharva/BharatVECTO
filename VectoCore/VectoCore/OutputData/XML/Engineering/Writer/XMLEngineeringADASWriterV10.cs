using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.Engineering.Interfaces;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.Engineering.Writer {
	internal class XMLEngineeringADASWriterV10 : AbstractXMLWriter,
		IXMLEngineeringADASWriter
	{

		public const string NAMESPACE_URI = XMLDefinitions.ENGINEERING_DEFINITONS_NAMESPACE_V10;

		public XMLEngineeringADASWriterV10() : base("AdvancedDriverAssistEngineeringType") { }

		#region Overrides of AbstractXMLWriter

		public override object[] WriteXML(IAdvancedDriverAssistantSystemsEngineering inputData)
		{
			var adas = inputData as IAdvancedDriverAssistantSystemsEngineering;
			if (adas == null) {
				return null;
			}
			// todo: write different ADAS options
			return null;
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace { get { return Writer.RegisterNamespace(NAMESPACE_URI); } }

		#endregion
	}
}