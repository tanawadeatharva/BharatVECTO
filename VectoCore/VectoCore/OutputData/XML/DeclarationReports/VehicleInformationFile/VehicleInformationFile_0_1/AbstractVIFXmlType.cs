using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class AbstractVIFXmlType
	{

		protected XNamespace _vif = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace _v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";
		protected XNamespace _v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3";
		protected XNamespace _v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace _di = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");
		


		protected readonly IVIFReportFactory _vifFactory;

		protected AbstractVIFXmlType(IVIFReportFactory vifFactory)
		{
			_vifFactory = vifFactory;
		}

		protected XElement GetSignature(DigestData digestData)
		{
			return new XElement(_v20 + XMLNames.DI_Signature,
				new XElement(_di + XMLNames.DI_Signature_Reference,
					new XAttribute(XMLNames.DI_Signature_Reference_URI_Attr, digestData.Reference),
					new XElement(_di + XMLNames.DI_Signature_Reference_Transforms,
						new XElement(_di + XMLNames.DI_Signature_Reference_Transforms_Transform,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, digestData.CanonicalizationMethods[0])),
						new XElement(_di + XMLNames.DI_Signature_Reference_Transforms_Transform,
							new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "http://www.w3.org/2001/10/xml-exc-c14n#"))),
					new XElement(_di + XMLNames.DI_Signature_Reference_DigestMethod,
						new XAttribute(XMLNames.DI_Signature_Algorithm_Attr, "http://www.w3.org/2001/04/xmlenc#sha256")),
					new XElement(_di + XMLNames.DI_Signature_Reference_DigestValue, digestData.DigestValue)));
		}
	}
}
