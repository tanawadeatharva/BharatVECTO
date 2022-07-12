using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public abstract class AbstractVehicleInformationFile : IXMLVehicleInformationFile
	{
		private XDocument _report;
		protected XNamespace _tns;

		protected readonly IVIFReportFactory _vifFactory;


		public static XNamespace VIF => XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

		protected XNamespace _xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");
		protected XNamespace _di = "http://www.w3.org/2000/09/xmldsig#";
		protected XNamespace _v20 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.0";
		protected XNamespace _v21 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.1";
		protected XNamespace _v23 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.3";
		protected XNamespace _v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";
		protected XNamespace _v10 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v1.0";

		public abstract string OutputDataType { get; }


		protected XElement Vehicle { get; set; }
		protected XElement Results { get; set; }
	

		protected AbstractVehicleInformationFile(IVIFReportFactory vifFactory)
		{
			_vifFactory = vifFactory;
		}
		
		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);
		
		#region Implementation of IXMLPrimaryVehicleReport

		public void Initialize(VectoRunData modelData, List<List<FuelData.Entry>> fuelModes)
		{
			InitializeVehicleData(modelData.InputData);
			Results = new XElement(VIF + XMLNames.Report_Results);
		}

		private List<XMLDeclarationReport.ResultEntry> results = new List<XMLDeclarationReport.ResultEntry>();
		public void WriteResult(XMLDeclarationReport.ResultEntry result)
		{
			results.Add(result);
		}

		public void GenerateReport(XElement fullReportHash)
		{
			Report = new XDocument(new XElement(VIF + XMLNames.VectoOutputMultistep,
				new XAttribute(XNamespace.Xmlns + "di", _di),
				new XAttribute(XNamespace.Xmlns + "xsi", _xsi.NamespaceName),
				new XAttribute(XNamespace.Xmlns + "vif", VIF),
				new XAttribute(XNamespace.Xmlns + "v2.0", _v20),
				new XAttribute(XNamespace.Xmlns + "v2.1", _v21),
				new XAttribute(XNamespace.Xmlns + "v2.3", _v23),
				new XAttribute(XNamespace.Xmlns + "v2.4", _v24),
				new XAttribute(_xsi + "schemaLocation", $"{_tns.NamespaceName} " + @"V:\VectoCore\VectoCore\Resources\XSD/VectoOutputMultistep.0.1.xsd"),
				new XAttribute("xmlns", _tns),

				new XElement(VIF + XMLNames.Bus_PrimaryVehicle,
					new XElement(VIF + "Data", new XAttribute("id", "1234"),
						Vehicle,
						Results,
						GetApplicationInfo()
					)
				),

				new XElement(VIF + XMLNames.DI_Signature)
			));
		}

		public XDocument Report { get; protected set; }

		public XNamespace Tns => _tns;

		#endregion



		protected XElement GetApplicationInfo()
		{
			var versionNumber = VectoSimulationCore.VersionNumber;
#if CERTIFICATION_RELEASE
			// add nothing to version number
#else
			versionNumber += " !!NOT FOR CERTIFICATION!!";
#endif
			return new XElement(VIF + XMLNames.Report_ApplicationInfo_ApplicationInformation,
				new XElement(VIF + XMLNames.Report_ApplicationInfo_SimulationToolVersion, versionNumber),
				new XElement(VIF + XMLNames.Report_ApplicationInfo_Date,
					XmlConvert.ToString(DateTime.Now, XmlDateTimeSerializationMode.Utc)));
		}


		protected XElement GetSignature(DigestData digestData)
		{
			return new XElement(VIF + XMLNames.DI_Signature,
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
