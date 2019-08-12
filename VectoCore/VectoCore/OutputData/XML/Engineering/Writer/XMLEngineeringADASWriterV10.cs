using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
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

			var ns = ComponentDataNamespace;
			return new object[] { new XElement(ns + XMLNames.Vehicle_AdvancedDriverAssist, 
				new XElement(ns + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(ns + XMLNames.Vehicle_ADAS_EcoRoll, adas.EcoRoll.ToXMLFormat()),
				new XElement(ns + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()))};
		}

		#endregion

		#region Overrides of AbstractXMLWriter

		public override XNamespace ComponentDataNamespace { get { return Writer.RegisterNamespace(NAMESPACE_URI); } }

		#endregion
	}
}