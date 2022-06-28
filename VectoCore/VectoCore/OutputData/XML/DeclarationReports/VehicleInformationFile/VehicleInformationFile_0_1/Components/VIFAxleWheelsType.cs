using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components
{
	public class VIFAxleWheelsType : AbstractVIFXmlType, IXmlTypeWriter
	{
		public VIFAxleWheelsType(IVIFReportFactory vifFactory) : base(vifFactory) { }

		#region Implementation of IXmlTypeWriter

		public XElement GetElement(IDeclarationInputDataProvider inputData)
		{
			var axleWheels = inputData.JobInputData.Vehicle.Components.AxleWheels;
			return new XElement(_vif + XMLNames.Component_AxleWheels,
				new XElement(_vif + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + "type", "v2.0:AxleDataDeclarationType"),
						GetAxleData(axleWheels.AxlesDeclaration)
				));
		}
		
		private List<XElement> GetAxleData(IList<IAxleDeclarationInputData> axleInput)
		{
			var axles = new List<XElement>();
			
			var axleNumber = 1;
			foreach (var currentAxle in axleInput) {
				var axle = new XElement(_vif + XMLNames.AxleWheels_Axles_Axle,
					new XAttribute(XMLNames.AxleWheels_Axles_Axle_AxleNumber_Attr, axleNumber++),
					new XAttribute(XNamespace.Xmlns + "v2.0", _v20),
					new XAttribute(_xsi + "type", "v2.0:AxleDataDeclarationType"),
					GetTyre(currentAxle.Tyre));
				axles.Add(axle);
			}

			return axles;
		}

		private XElement GetTyre(ITyreDeclarationInputData tyre)
		{
			var currentTyre = 
				new XElement(_v20 + XMLNames.ComponentDataWrapper,
					new XAttribute(_xsi + "type", "TyreDataDeclarationType"),
					new XAttribute("id", tyre.DigestValue.Reference),
					new XElement(XMLNames.Component_Manufacturer, tyre.Manufacturer),
					new XElement(XMLNames.Component_Model, tyre.Model),
					new XElement(XMLNames.Component_CertificationNumber, tyre.CertificationNumber),
					new XElement(XMLNames.Component_Date, XmlConvert.ToString(tyre.Date, XmlDateTimeSerializationMode.Utc)),
					new XElement(XMLNames.Component_AppVersion, tyre.Date),
					new XElement(XMLNames.AxleWheels_Axles_Axle_Dimension, tyre.Dimension),
					new XElement(XMLNames.AxleWheels_Axles_Axle_RRCDeclared, tyre.RollResistanceCoefficient),
					new XElement(XMLNames.AxleWheels_Axles_Axle_FzISO, tyre.FuelEfficiencyClass)
			);

			return new XElement(_v20 + XMLNames.AxleWheels_Axles_Axle_Tyre,
				currentTyre,
				GetSignature(tyre.DigestValue)
			);
		}
		
		#endregion
	}
}
