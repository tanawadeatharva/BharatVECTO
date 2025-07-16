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

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return (adas != null) 
				? new XElement(_vif + XMLNames.Vehicle_ADAS, 
					new XElement(_v27 + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
					new XElement(_v27 + XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adas.EcoRoll == EcoRollType.WithoutEngineStop),
					new XElement(_v27+ XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adas.EcoRoll == EcoRollType.WithEngineStop),
					new XElement(_v27 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()),
					adas.ATEcoRollReleaseLockupClutch.HasValue
						? new XElement(_v27 + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch, adas.ATEcoRollReleaseLockupClutch.Value)
						: null
					)
				: null; 
		}
	}

	public class VIFHEVAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFHEVAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return (adas != null)
				? new XElement(_vif + XMLNames.Vehicle_ADAS,
					new XElement(_v27 + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
					new XElement(_v27 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat()),
					adas.ATEcoRollReleaseLockupClutch.HasValue
						? new XElement(_v27 + XMLNames.Vehicle_ADAS_ATEcoRollReleaseLockupClutch, adas.ATEcoRollReleaseLockupClutch.Value)
						: null
					)
				: null;
		}
	}

	public class VIFPEVAdasType : AbstractVIFXmlType, IVIFFAdasType
	{
		public VIFPEVAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		public XElement GetXmlType(IAdvancedDriverAssistantSystemDeclarationInputData adas)
		{
			return (adas != null)
				? new XElement(_vif + XMLNames.Vehicle_ADAS,
					new XElement(_v27 + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat())
					)
				: null;
		}
	}
}
