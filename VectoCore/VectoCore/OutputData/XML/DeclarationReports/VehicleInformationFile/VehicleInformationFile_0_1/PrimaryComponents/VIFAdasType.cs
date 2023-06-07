using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public interface IVIFFAdasType
	{
		XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas);
	}

	public class VIFConventionalAdasType :  AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFConventionalAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_vif + XMLNames.Vehicle_ADAS, 
				new XAttribute("xmlns", _v24),
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_Conventional_Type"),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(_v24 +  XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adas.EcoRoll == EcoRollType.WithoutEngineStop),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adas.EcoRoll == EcoRollType.WithEngineStop),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()),
				adas.ATEcoRollReleaseLockupClutch.HasValue
					? new XElement(_v24 + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch,
						adas.ATEcoRollReleaseLockupClutch.Value)
					: null); 
		}

		#endregion
	}

	public class VIFHEVAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFHEVAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_vif + XMLNames.Vehicle_ADAS,
				new XAttribute("xmlns", _v24),
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_HEV_Type"),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()),
				adas.ATEcoRollReleaseLockupClutch.HasValue
					? new XElement(_v24 + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch,
						adas.ATEcoRollReleaseLockupClutch.Value)
					: null);
		}

		#endregion
	}

	public class VIFPEVAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFPEVAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_vif + XMLNames.Vehicle_ADAS,
				new XAttribute("xmlns", _v24),
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_PEV_Type"),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()));
		}

		#endregion
	}

	public class VIFIEPCAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFIEPCAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			if (adas == null)
				return null;

			return new XElement(_vif + XMLNames.Vehicle_ADAS,
				new XAttribute("xmlns", _v24),
				new XAttribute(_xsi + XMLNames.XSIType, "ADAS_IEPC_Type"),
				new XElement(_v24 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()));
		}

		#endregion
	}
}
