using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFTorqueConverterType : AbstractVIFXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
    {
		public VIFTorqueConverterType(IVIFReportFactory vifFactory) : base(vifFactory) { }

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
        {
            return GetElement(axlePt.TorqueConverterInputData);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            return GetElement(inputData.JobInputData.Vehicle.Components.TorqueConverterInputData);
        }

        private XElement GetElement(ITorqueConverterDeclarationInputData torque)
		{
			if (torque == null)
				return null;

			return new XElement(_vif + XMLNames.Component_TorqueConverter,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "TorqueConverterDataVIFType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, torque.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, torque.Model),
					new XElement(_vif + XMLNames.Component_CertificationMethod,
						torque.CertificationMethod.ToXMLFormat()),
					torque.CertificationMethod == CertificationMethod.StandardValues
						? null
						: new XElement(_vif + XMLNames.Report_Component_CertificationNumber,
							torque.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date,
						XmlConvert.ToString(torque.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, torque.AppVersion)
				));
		}

		
	}
}
