using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriter
	{
		XElement GenerateResults(List<IResultEntry> results);
	}


	public class ExemptedResultsWriter : IResultsWriter
	{
		protected XNamespace XmlNS => XNamespace.Get("urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9");

		#region Implementation of IResultsWriter

		public XElement GenerateResults(List<IResultEntry> results)
		{
			return new XElement(XmlNS + "Results",
				new XElement(XmlNS + "Status", "success"),
				new XElement(XmlNS + "ExemptedVehicle"));
		}

		#endregion
	}
}