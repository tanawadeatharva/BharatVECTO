using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAxlegearType : AbstractVIFXmlType, IXmlTypeWriter, IXmlAxlePowertrainTypeWriter
    {
		public VIFAxlegearType(IVIFReportFactory vifFactory) : base(vifFactory) { }

        public XElement GetElement(IAxlePowertrainDeclarationInputData axlePt)
        {
            return GetElement(axlePt.AxleGearInputData);
        }

        public XElement GetElement(IDeclarationInputDataProvider inputData)
        {
            return GetElement(inputData.JobInputData.Vehicle.Components.AxleGearInputData);
        }

        private XElement GetElement(IAxleGearInputData axleGear)
		{
			if (axleGear == null)
				return null;
			
			return new XElement(_vif + XMLNames.Component_Axlegear,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "AxlegearDataVIFType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, axleGear.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, axleGear.Model),
					new XElement(_vif + XMLNames.Component_CertificationMethod, axleGear.CertificationMethod.ToXMLFormat()),
					axleGear.CertificationMethod == CertificationMethod.StandardValues
						? null
						: new XElement(_vif + XMLNames.Report_Component_CertificationNumber,
							axleGear.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date,
						XmlConvert.ToString(axleGear.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, axleGear.AppVersion),
					new XElement(_vif + XMLNames.Axlegear_LineType, axleGear.LineType.ToXMLFormat()),
					new XElement(_vif + XMLNames.Axlegear_Ratio, axleGear.Ratio.ToXMLFormat(3))));
		}
	}
}
