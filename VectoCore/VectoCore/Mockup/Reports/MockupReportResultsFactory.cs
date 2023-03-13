using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;

namespace TUGraz.VectoMockup.Reports
{
	class MockupReportResultsFactory : IResultsWriterFactory
	{
		#region Implementation of IResultsWriterFactory

		public IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			return new MockupDummyResultsWriter(XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9"));
		}

		public IResultsWriter GetMRFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			return new MockupDummyResultsWriter(XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9"));
		}

		public IResultsWriter GetVIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			return new MockupDummyResultsWriter(XNamespace.Get("urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1"));
		}

		#endregion
	}

	internal class MockupDummyResultsWriter : IResultsWriter
	{
		private readonly XNamespace TNS;

		public MockupDummyResultsWriter(XNamespace ns)
		{
			TNS = ns;
		}

		#region Implementation of IResultsWriter

		public XElement GenerateResults(List<IResultEntry> results)
		{
			// only return a single 'Results' element - will be replaced in Mockup Report
			return new XElement(TNS + "Results");
		}

		#endregion
	}

}