using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public class NoSummaryWriter : IReportResultsSummaryWriter
	{
		#region Implementation of IReportResultsSummaryWriter

		public XElement GetElement(IList<IResultEntry> entries)
		{
			return null;
		}

		public XElement GetElement(IList<IOVCResultEntry> entries)
		{
			return null;
		}

		#endregion
	}
}