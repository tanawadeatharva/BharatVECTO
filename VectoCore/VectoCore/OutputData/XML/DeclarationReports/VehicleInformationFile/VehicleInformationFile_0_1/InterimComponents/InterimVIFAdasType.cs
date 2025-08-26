using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.InterimComponents
{
	public class VIFCompletedConventionalAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFCompletedConventionalAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_v27 + XMLNames.Vehicle_ADAS,
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_Conventional_Type"),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adas.EcoRoll == EcoRollType.WithoutEngineStop),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adas.EcoRoll == EcoRollType.WithEngineStop),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()),
				adas.ATEcoRollReleaseLockupClutch.HasValue
					? new XElement(_v27 + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch,
						adas.ATEcoRollReleaseLockupClutch.Value)
					: null);
		}

		#endregion
	}


	public class VIFCompletedHEVAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFCompletedHEVAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_v27 + XMLNames.Vehicle_ADAS,
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_HEV_Type"),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()),
				adas.ATEcoRollReleaseLockupClutch.HasValue
					? new XElement(_v27 + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch,
						adas.ATEcoRollReleaseLockupClutch.Value)
					: null);
		}

		#endregion
	}

	public class VIFCompletedPEVAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFCompletedPEVAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_v27 + XMLNames.Vehicle_ADAS,
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_PEV_Type"),
				new XElement(_v27 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()));
		}

		#endregion
	}
}