using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Resources;
using TUGraz.VectoCore.Models.Simulation.Data;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9;

namespace TUGraz.VectoMockup.Reports
{
	class MockupReportResultsFactory : IResultsWriterFactory
	{
		protected XNamespace CIF = AbstractCustomerReport.Namespace;
		protected XNamespace MRF = AbstractManufacturerReport.Namespace;

        protected XNamespace VIF = XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1");

		#region Implementation of IResultsWriterFactory

		public IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			if (exempted)
				return new MockupExemptedResultsWriter(CIF, MockupResultReader.ResultType.CIF);
			return new MockupDummyResultsWriter(CIF, MockupResultReader.ResultType.CIF);
		}

		public IResultsWriter GetMRFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			if (exempted)
				return new MockupExemptedResultsWriter(MRF, MockupResultReader.ResultType.CIF);
			return new MockupDummyResultsWriter(MRF, MockupResultReader.ResultType.MRF);
		}

		public IResultsWriter GetVIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			if (exempted)
				return new MockupExemptedResultsWriter(VIF, MockupResultReader.ResultType.CIF);
			return new MockupDummyResultsWriter(VIF, MockupResultReader.ResultType.VIF);
		}

		#endregion
	}

	internal class MockupExemptedResultsWriter : IResultsWriter
	{
		private readonly XNamespace TNS;
		public MockupResultReader.ResultType ResultType;

		public MockupExemptedResultsWriter(XNamespace ns, MockupResultReader.ResultType resultType)
		{
			TNS = ns;
			ResultType = resultType;
		}

		#region Implementation of IResultsWriter

		public XElement GenerateResults(List<IResultEntry> results)
		{
			var retVal = new XElement(TNS + XMLNames.Report_Results);
			retVal.Add(new XElement(TNS + "Status", "success"));
			retVal.Add(new XElement(TNS + "ExemptedVehicle"));
			return retVal;
		}

		#endregion
	}

	internal class MockupDummyResultsWriter : IResultsWriter
	{
		private readonly XNamespace TNS;

		public MockupDummyResultsWriter(XNamespace ns, MockupResultReader.ResultType resultType)
		{
			TNS = ns;
			ResultType = resultType;
		}

		public MockupResultReader.ResultType ResultType;

		#region Implementation of IResultsWriter

		public XElement GenerateResults(List<IResultEntry> results)
		{
			var retVal = new XElement(TNS + XMLNames.Report_Results);
			retVal.AddFirst(new XElement(TNS + "Status", "success"));
			retVal.AddFirst(new XComment("Always prints success at the moment"));

			foreach (var result in results) {
				switch (ResultType) {
					case MockupResultReader.ResultType.CIF:
						retVal.Add(MockupResultReader.GetCIFMockupResult(result, "Result", result.VectoRunData));
						break;
					case MockupResultReader.ResultType.MRF:
						retVal.Add(MockupResultReader.GetMRFMockupResult(result, "Result", result.VectoRunData));
						break;
					case MockupResultReader.ResultType.VIF:
						retVal.Add(MockupResultReader.GetVIFMockupResult(result, "Result", result.VectoRunData));
						break;
				}
				
			}

			if (ResultType == MockupResultReader.ResultType.CIF) {
				var result = results.First();
				if (!result.VectoRunData.VehicleData.VocationalVehicle) {
					retVal.Add(MockupResultReader.GetCIFMockupResult(result, TNS + "Summary",
						result.VectoRunData));
				}
			}

			return retVal;
		}

		#endregion
	}

}