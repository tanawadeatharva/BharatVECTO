using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;


namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFTransmissionType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFTransmissionType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var transmission = inputData.JobInputData.Vehicle.Components.GearboxInputData;
			if (transmission == null)
				return null;

			return new XElement(_vif + XMLNames.Component_Transmission,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + XMLNames.XSIType, "TransmissionDataVIFType"),
					new XElement(_vif + XMLNames.Component_Manufacturer, transmission.Manufacturer),
					new XElement(_vif + XMLNames.Component_Model, transmission.Model),
					new XElement(_vif + XMLNames.Component_Gearbox_CertificationMethod,
						transmission.CertificationMethod.ToXMLFormat()),
					transmission.CertificationMethod == CertificationMethod.StandardValues
						? null
						: new XElement(_vif + XMLNames.Component_CertificationNumber, transmission.CertificationNumber),
					new XElement(_vif + XMLNames.Component_Date,
						XmlConvert.ToString(transmission.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(_vif + XMLNames.Component_AppVersion, transmission.AppVersion),
					new XElement(_vif + XMLNames.Gearbox_TransmissionType, transmission.Type.ToXMLFormat()),
					new XElement(_vif + XMLNames.Gearbox_Gears,
						new XAttribute(_xsi + XMLNames.XSIType, "TransmissionGearsVIFType"),
						transmission.Gears.Select(
							x => new XElement(
								_vif + XMLNames.Gearbox_Gears_Gear,
								new XAttribute(XMLNames.Gearbox_Gear_GearNumber_Attr, x.Gear),
								new XElement(_vif + XMLNames.Gearbox_Gear_Ratio, x.Ratio.ToXMLFormat(3)),
								x.MaxTorque != null
									? new XElement(_vif + XMLNames.Gearbox_Gears_MaxTorque, x.MaxTorque.ToXMLFormat(0))
									: null,
								x.MaxInputSpeed != null
									? new XElement(_vif + XMLNames.Gearbox_Gear_MaxSpeed,
										x.MaxInputSpeed.AsRPM.ToXMLFormat(0))
									: null)
						)
					)
				)
			);
		}

		#endregion
	}


}
