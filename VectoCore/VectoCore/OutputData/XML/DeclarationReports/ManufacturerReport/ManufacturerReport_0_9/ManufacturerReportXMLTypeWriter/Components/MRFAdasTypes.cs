using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
	public interface IMRFAdasType
	{
		XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas);
	}

	internal class MRFConventionalAdasType : AbstractMrfXmlType, IMRFAdasType
    {
		public MRFConventionalAdasType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			return new XElement(_mrf + XMLNames.Vehicle_ADAS,
				new XElement(_mrf + XMLNames.Vehicle_ADAS_EngineStopStart, inputData.EngineStopStart),
				new XElement(_mrf + "EcoRollWithoutEngineStopStart",
					inputData.EcoRoll == EcoRollType.WithoutEngineStop),
				new XElement(_mrf + "EcoRollWithEngineStopStart",
					inputData.EcoRoll == EcoRollType.WithEngineStop),
				new XElement(_mrf + XMLNames.Vehicle_ADAS_PCC,
					inputData.PredictiveCruiseControl != PredictiveCruiseControlType.None));
		}

		#endregion

		
	}

	internal class MRFHevAdasType : AbstractMrfXmlType, IMRFAdasType
	{
		public MRFHevAdasType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			return new XElement(_mrf + XMLNames.Vehicle_ADAS, 
				new XElement(_mrf + XMLNames.Vehicle_ADAS_EngineStopStart, inputData.EngineStopStart),
				new XElement(_mrf + XMLNames.Vehicle_ADAS_PCC, inputData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
				);
		}

		#endregion
	}

	internal class MRFPevAdasType : AbstractMrfXmlType, IMRFAdasType
	{
		public MRFPevAdasType(IManufacturerReportFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			return new XElement(_mrf + XMLNames.Vehicle_ADAS,
				new XElement(_mrf + XMLNames.Vehicle_ADAS_PCC, inputData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			);
		}

		#endregion
	}
}
