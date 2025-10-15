using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAngleDriveType : AbstractVIFXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
    {
		public VIFAngleDriveType(IVIFReportFactory vifFactory) : base(vifFactory) { }

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
        {
            return GetElement(axlePt.AngledriveInputData);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            return GetElement(inputData.JobInputData.Vehicle.Components.AngledriveInputData);
        }

        private XElement GetElement(IAngledriveInputData angleDrive)
		{
			if (angleDrive == null || angleDrive.Type != AngledriveType.SeparateAngledrive) {
				return null;
			}

			return new XElement(_vif + XMLNames.Component_Angledrive,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AngledriveDataVIFType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, angleDrive.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, angleDrive.Model),
					new XElement(_vif + XMLNames.Component_CertificationMethod, angleDrive.CertificationMethod.ToXMLFormat()),
					angleDrive.CertificationMethod == CertificationMethod.StandardValues
						? null
						: new XElement(_vif + XMLNames.Report_Component_CertificationNumber,
							angleDrive.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date,
						XmlConvert.ToString(angleDrive.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, angleDrive.AppVersion),
					new XElement(_vif + XMLNames.AngleDrive_Ratio, angleDrive.Ratio.ToXMLFormat(3))
				));
		}

	}
}
