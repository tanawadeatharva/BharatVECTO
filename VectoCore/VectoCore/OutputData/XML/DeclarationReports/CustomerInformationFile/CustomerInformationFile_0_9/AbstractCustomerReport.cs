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
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCommon.Utils;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.Models.SimulationComponent;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReport;
using TUGraz.VectoCore.Utils;
using TUGraz.VectoHashing;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9
{
    public abstract class AbstractCustomerReport : IXMLCustomerReport
	{
		protected readonly ICustomerInformationFileFactory _cifFactory;
		protected readonly IResultsWriterFactory _resultFactory;

		protected XNamespace xsi = XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance");

		public static XNamespace Cif => XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput");

		public static XNamespace Cif_0_9 => XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9");
		public static XNamespace _di => XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");


		protected XElement Vehicle { get; set; }

		protected IVehicleDeclarationInputData Input { get; set; }
		protected IResultsWriter Results { get; set; }

		protected XElement InputDataIntegrity { get; set; }

		public abstract string OutputDataType { get; }

		protected bool _ovc = false;
		protected AbstractCustomerReport(ICustomerInformationFileFactory cifFactory, IResultsWriterFactory resultFactory)
		{
			_cifFactory = cifFactory;
			_resultFactory = resultFactory;
		}


		public abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

		#region Implementation of IXMLCustomerReport

		public virtual void Initialize(VectoRunData modelData)
		{
			if (modelData.VehicleData.VehicleClass.IsBus()) {
				Input = modelData.InputData.PrimaryVehicleData.Vehicle;
			} else {
				Input = modelData.InputData.JobInputData.Vehicle;
			}
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;
			Results = _resultFactory.GetCIFResultsWriter(modelData.InputData, modelData.VehicleData.VehicleCategory.GetVehicleType(), 
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted); 
			InputDataIntegrity = new XElement(Cif_0_9 + XMLNames.Report_InputDataSignature,
				modelData.InputData.XMLHash == null ? XMLHelper.CreateDummySig(_di) : new XElement(modelData.InputData.XMLHash));
		}

		public XDocument Report { get; protected set; }

		protected List<IResultEntry> _results = new List<IResultEntry>();

		public void WriteResult(IResultEntry resultValue)
		{
			_results.Add(resultValue);
		}

		public void GenerateReport(XElement resultSignature)
		{
			var retVal = new XDocument(new XElement(Cif + "VectoCustomerInformation",
					new XAttribute(XNamespace.Xmlns + "xsi", xsi),
					new XAttribute(XNamespace.Xmlns + "cif", Cif.NamespaceName),
					new XAttribute(XNamespace.Xmlns + "cif0.9", Cif_0_9.NamespaceName),
					new XAttribute("xmlns", Cif_0_9),
					new XAttribute(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") + "schemaLocation",
						$"{Cif.NamespaceName} " + @"V:\VectoCore\VectoCore\Resources\XSD/VectoOutputCustomer.xsd"),

					new XElement(Cif + XMLNames.Report_DataWrap,
						new XAttribute(xsi + XMLNames.XSIType, $"{OutputDataType}"),
						GetReportContents(resultSignature)
					)
				)
			);

			var RDGroupEntry = _results.SingleOrDefault(e => DeclarationData.EvaluateLHSubgroupConditions(e));

			// ReSharper disable once PossibleNullReferenceException
			Vehicle.XPathSelectElement($"//*[local-name()='{XMLNames.VehicleGroupCO2}']").Value =
				DeclarationData.GetVehicleGroupCO2StandardsGroup(Input, RDGroupEntry != null ? RDGroupEntry.ActualChargeDepletingRange?.Value() : null).ToXMLFormat();

			var stream = new MemoryStream();
			var writer = new StreamWriter(stream);
			writer.Write(retVal);
			writer.Flush();
			stream.Seek(0, SeekOrigin.Begin);
			var h = VectoHash.Load(stream);
			Report = h.AddHash();
		}

		protected virtual IList<XElement> GetReportContents(XElement resultSignature)
		{
			return new[] {
				Vehicle,
				InputDataIntegrity,
				new XElement(Cif_0_9 + XMLNames.Report_ManufacturerRecord_Signature, resultSignature),
				Results.GenerateResults(_results),
				XMLHelper.GetApplicationInfo(Cif_0_9)
			};
		}

		#endregion


	}


}
