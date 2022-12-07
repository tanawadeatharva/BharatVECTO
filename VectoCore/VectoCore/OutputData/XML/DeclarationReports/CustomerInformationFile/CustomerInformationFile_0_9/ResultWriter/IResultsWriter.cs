using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.Models;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriter
	{
		XElement GenerateResults(List<IResultEntry> results);
	}

	public abstract class AbstractResultsWriter : IResultsWriter
	{
		#region Implementation of IResultsWriter

		//public abstract XElement GenerateResults(List<IResultEntry> results);

		public virtual XElement GenerateResults(List<IResultEntry> results)
		{
			return null;
		}

		#endregion
	}

	public class CIFResultsWriter
	{
		public class ConventionalLorry : AbstractResultsWriter {}

		public class HEVNonOVCLorry : AbstractResultsWriter {}

		public class HEVOVCLorry : AbstractResultsWriter {}

		public class PEVLorry : AbstractResultsWriter {}

		public class ConventionalBus : AbstractResultsWriter {}

		public class HEVNonOVCBus : AbstractResultsWriter {}

		public class HEVOVCBus : AbstractResultsWriter {}

		public class PEVBus : AbstractResultsWriter {}

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
}