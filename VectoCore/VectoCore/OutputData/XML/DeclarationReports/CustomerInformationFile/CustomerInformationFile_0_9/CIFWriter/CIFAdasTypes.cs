using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface ICIFAdasType
    {
        XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas);
    }

    internal class CIFConventionalAdasType : AbstractCifXmlType, ICIFAdasType
	{
		public CIFConventionalAdasType(ICustomerInformationFileFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMRFComponentWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			return new XElement(_cif + XMLNames.Vehicle_ADAS,
				new XElement(_cif + XMLNames.Vehicle_ADAS_EngineStopStart, inputData.EngineStopStart),
				new XElement(_cif + "EcoRollWithoutEngineStopStart",
					inputData.EcoRoll == EcoRollType.WithoutEngineStop),
				new XElement(_cif + "EcoRollWithEngineStopStart",
					inputData.EcoRoll == EcoRollType.WithEngineStop),
				new XElement(_cif + XMLNames.Vehicle_ADAS_PCC,
					inputData.PredictiveCruiseControl != PredictiveCruiseControlType.None));
		}

		#endregion

		
	}

	internal class CIFHevAdasType : AbstractCifXmlType, ICIFAdasType
	{
		public CIFHevAdasType(ICustomerInformationFileFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			return new XElement(_cif + XMLNames.Vehicle_ADAS, 
				new XElement(_cif + XMLNames.Vehicle_ADAS_EngineStopStart, inputData.EngineStopStart),
				new XElement(_cif + XMLNames.Vehicle_ADAS_PCC, inputData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
				);
		}

		#endregion
	}

	internal class CIFPevAdasType : AbstractCifXmlType, ICIFAdasType
	{
		public CIFPevAdasType(ICustomerInformationFileFactory mrfFactory) : base(mrfFactory) { }

		#region Overrides of AbstractMrfXmlType

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData inputData)
		{
			return new XElement(_cif + XMLNames.Vehicle_ADAS,
				new XElement(_cif + XMLNames.Vehicle_ADAS_PCC, inputData.PredictiveCruiseControl != PredictiveCruiseControlType.None)
			);
		}

		#endregion
	}
}
