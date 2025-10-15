using System.Xml.Linq;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class AbstractVIFXmlType
	{

		protected XNamespace _vif = XMLDefinitions.VEHICLE_INTERIM_FILE_TARGET_VERSION;
        protected XNamespace _v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";
		protected XNamespace _v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3";
        protected XNamespace _v26 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.6";
        protected XNamespace _v27 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.7";
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
