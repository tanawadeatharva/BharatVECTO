using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Declaration;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile
{
	public interface IXMLCustomerReport
	{
		void Initialize(VectoRunData modelData);
		XDocument Report { get; }
		void WriteResult(IResultEntry resultValue);
		void GenerateReport(XElement resultSignature);
	}

	public interface IXMLCustomerReportCompletedBus
	{
		void WriteResult(IResultEntry genericResult,
			IResultEntry specificResult, IResult primaryResult);
	}
}