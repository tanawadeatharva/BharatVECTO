using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.InputData.Reader.ComponentData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFTorqueConverterType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFTorqueConverterType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var torque = inputData.JobInputData.Vehicle.Components.TorqueConverterInputData;
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
					new XElement(_vif + XMLNames.Component_AppVersion, torque.AppVersion),
					GetCharacteristics(torque.TCData)
				));
		}


		private XElement GetCharacteristics(DataTable tcData)
		{
			if(tcData == null)
				return null;

			var entries = new List<XElement>();
			for (int r = 0; r < tcData.Rows.Count; r++) {

				var speedRatio = tcData.Rows[r][TorqueConverterDataReader.Fields.SpeedRatio];
				var torqueRatio = tcData.Rows[r][TorqueConverterDataReader.Fields.TorqueRatio];
				var inputTorqueRef = tcData.Rows[r][TorqueConverterDataReader.Fields.CharacteristicTorque];

				var xElement = new XElement(_vif + XMLNames.TorqueConverter_Characteristics_Entry,
					new XAttribute(XMLNames.TorqueConverterData_SpeedRatio_Attr, speedRatio),
					new XAttribute(XMLNames.TorqueConverterData_TorqueRatio_Attr, torqueRatio),
					new XAttribute(XMLNames.TorqueConverterDataMapping_InputTorqueRef_Attr, inputTorqueRef));

				entries.Add(xElement);
			}

			return new XElement(_vif + XMLNames.TorqueConverter_Characteristics,
				entries.Select(x => x));
		}

		#endregion
	}
}
