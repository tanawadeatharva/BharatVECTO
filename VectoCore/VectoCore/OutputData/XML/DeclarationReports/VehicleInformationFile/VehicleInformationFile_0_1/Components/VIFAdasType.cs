using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAdasType :  AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFAdasType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var adas = inputData.JobInputData.Vehicle.ADAS;
			return new XElement(_vif + XMLNames.Vehicle_ADAS,
				new XElement(_vif + XMLNames.Vehicle_ADAS_EngineStopStart, adas.EngineStopStart),
				new XElement(_vif +  XMLNames.Vehicle_ADAS_EcoRollWithoutEngineStop, adas.EcoRoll == EcoRollType.WithoutEngineStop),
				new XElement(_vif + XMLNames.Vehicle_ADAS_EcoRollWithEngineStopStart, adas.EcoRoll == EcoRollType.WithEngineStop),
				new XElement(_vif + XMLNames.Vehicle_ADAS_PCC, adas.PredictiveCruiseControl.ToXMLFormat())); 
		}

		#endregion
	}
}
