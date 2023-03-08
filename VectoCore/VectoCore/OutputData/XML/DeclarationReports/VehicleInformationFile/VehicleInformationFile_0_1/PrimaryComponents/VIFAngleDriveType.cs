using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAngleDriveType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFAngleDriveType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var angelDrive = inputData.JobInputData.Vehicle.Components.AngledriveInputData;
			if (angelDrive == null || angelDrive.Type != AngledriveType.SeparateAngledrive)
				return null;
			}

			return new XElement(_vif + XMLNames.Component_Angledrive,
					new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AngledriveDataVIFType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, angelDrive.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, angelDrive.Model),
					new XElement(_vif + XMLNames.Component_CertificationMethod, angelDrive.CertificationMethod.ToXMLFormat()),
					angelDrive.CertificationMethod == CertificationMethod.StandardValues
						? null
						: new XElement(_vif + XMLNames.Report_Component_CertificationNumber,
							angelDrive.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date,
						XmlConvert.ToString(angelDrive.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, angelDrive.AppVersion),
					new XElement(_vif + XMLNames.AngleDrive_Ratio, angelDrive.Ratio.ToXMLFormat(3))
				));
		}

		#endregion
	}
}
