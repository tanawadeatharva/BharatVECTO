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

	}
}
