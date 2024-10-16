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
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
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
		protected readonly IResultsWriterFactory _resultFactory;

		protected bool _ovc = false;

		protected XElement Vehicle { get; set; }

		protected IVehicleDeclarationInputData Input { get; set; }
		protected IResultsWriter Results { get; set; }

		protected XElement InputDataIntegrity { get; set; }

		public abstract string OutputDataType { get; } //also used as name for the mockup result element

		protected AbstractManufacturerReport(IManufacturerReportFactory MRFReportFactory, IResultsWriterFactory resultFactory)
		{
			_mRFReportFactory = MRFReportFactory;
			_resultFactory = resultFactory;
		}

		#region Implementation of IXMLManufacturerReport

		protected abstract void InitializeVehicleData(IDeclarationInputDataProvider inputData);

		public virtual void Initialize(VectoRunData modelData)
		{
			if (modelData.VehicleData.VehicleClass.IsBus())
			{
				switch (modelData.InputData) {
					case ISingleBusInputDataProvider single:
						Input = single.PrimaryVehicle;
						break;
					case IMultistepBusInputDataProvider multistep:
						Input = multistep.JobInputData.PrimaryVehicle.Vehicle;
						break;
					case IDeclarationInputDataProvider declaration:
						Input = declaration.JobInputData.Vehicle;
						break;
				}
			}
			else
			{
				Input = modelData.InputData.JobInputData.Vehicle;
			}
			InitializeVehicleData(modelData.InputData);
			_ovc = modelData.VehicleData.OffVehicleCharging;
			
			Results = _resultFactory.GetMRFResultsWriter(modelData.VehicleData.VehicleCategory.GetVehicleType(),
				modelData.JobType, modelData.VehicleData.OffVehicleCharging, modelData.Exempted);
			InputDataIntegrity = new XElement(Mrf_0_9 + XMLNames.Report_InputDataSignature,
				modelData.InputData.XMLHash == null ? XMLHelper.CreateDummySig(_di) : new XElement(modelData.InputData.XMLHash));
		}

		public XDocument Report { get; protected set; }

		protected List<IResultEntry> _results = new List<IResultEntry>();

		public void WriteResult(IResultEntry resultValue)
		{
			_results.Add(resultValue);

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

			var RDGroupEntry = _results.SingleOrDefault(e => DeclarationData.EvaluateLHSubgroupConditions(e));
			double? LHOperationalRange = RDGroupEntry != null ? RDGroupEntry.ActualChargeDepletingRange?.Value() : null;

			Vehicle.XPathSelectElement($"//*[local-name()='{XMLNames.VehicleGroupCO2}']").Value =
				DeclarationData.GetVehicleGroupCO2StandardsGroup(Input, LHOperationalRange).ToXMLFormat();

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
				Results.GenerateResults(_results),
				XMLHelper.GetApplicationInfo(Mrf_0_9)
			};
		}

		#endregion

	}

}
