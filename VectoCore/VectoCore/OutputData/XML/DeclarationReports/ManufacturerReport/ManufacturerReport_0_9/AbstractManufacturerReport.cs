using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9
{
	public abstract class AbstractManufacturerReport : IXMLManufacturerReport
    {
        protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		public static XNamespace Mrf => XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput");

		public static XNamespace Mrf_0_9 => XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9");
		public static XNamespace _di => XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");


		protected readonly IManufacturerReportFactory _mRFReportFactory;

		protected bool _ovc = false;

		protected XElement Results { get; set; }
		protected XElement Vehicle { get; set; }

		protected XElement InputDataIntegrity { get; set; }

		protected XElement Signature { get; set; }

		public abstract string OutputDataType { get; } //also used as name for the mockup result element

		protected AbstractManufacturerReport(IManufacturerReportFactory MRFReportFactory)
		{
			_mRFReportFactory = MRFReportFactory;

			// TODO MQ: write dummy result element for testcases (2022-07-13), remove once result writing is implemented
			Results = new XElement(Mrf_0_9 + "Results",
				new XElement(Mrf_0_9 + "Status", "success"),
				new XElement(Mrf_0_9 + "ExemptedVehicle"));
		}

		#region Implementation of IXMLManufacturerReport

		protected abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

		public virtual void Initialize(VectoRunData modelData)
		{
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;
			
			//Results = new XElement(Mrf_0_9 + XMLNames.Report_Results);
			InputDataIntegrity = new XElement(Mrf_0_9 + XMLNames.Report_InputDataSignature,
				modelData.InputData.XMLHash == null ? XMLHelper.CreateDummySig(_di) : new XElement(modelData.InputData.XMLHash));
		}

		public XDocument Report { get; protected set; }

		private List<XMLDeclarationReport.ResultEntry> results = new List<XMLDeclarationReport.ResultEntry>();

		public void WriteResult(XMLDeclarationReport.ResultEntry resultValue)
		{
			

		}


		public void GenerateReport()
		{
			var retVal = new XDocument(new XElement(Mrf + "VectoOutput",
					new XAttribute(XNamespace.Xmlns + "xsi", xsi),
					new XAttribute(XNamespace.Xmlns + "mrf", Mrf),
					new XAttribute(XNamespace.Xmlns + "mrf0.9", Mrf_0_9),
					new XAttribute("xmlns", Mrf_0_9),
					new XAttribute(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") + "schemaLocation",
						$"{Mrf.NamespaceName} " + @"V:\VectoCore\VectoCore\Resources\XSD/VectoOutputManufacturer.xsd"),

					new XElement(Mrf + XMLNames.Report_DataWrap,
						new XAttribute(xsi + XMLNames.XSIType, $"{OutputDataType}"),
						GetContents()
					)
				)
			);

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(retVal);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			var h = VectoHash.Load(stream);
			Report = h.AddHash();
		}

		protected virtual IList<XElement> GetContents()
		{
			return new[] {
				Vehicle,
				InputDataIntegrity,
				Results,
				XMLHelper.GetApplicationInfo(Mrf_0_9)
			};
		}

		#endregion

	}

}
