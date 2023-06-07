using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFRetarderType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFRetarderType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var retarder = inputData.JobInputData.Vehicle.Components.RetarderInputData;
			if (retarder == null || retarder.Type.IsOneOf(RetarderType.EngineRetarder, RetarderType.None,
					RetarderType.LossesIncludedInTransmission))
				return null;

			return new XElement(_vif + XMLNames.Component_Retarder,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "RetarderDataVIFType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, retarder.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, retarder.Model),
					new XElement(_vif + XMLNames.Component_CertificationMethod, retarder.CertificationMethod.ToXMLFormat()),
					retarder.CertificationMethod == CertificationMethod.StandardValues
						? null
						: new XElement(_vif + XMLNames.Report_Component_CertificationNumber,
							retarder.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date,
						XmlConvert.ToString(retarder.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, retarder.AppVersion),
					new XElement(_vif + XMLNames.Vehicle_RetarderRatio, retarder.Ratio.ToXMLFormat(3))
				));
		}

		#endregion
	}
}