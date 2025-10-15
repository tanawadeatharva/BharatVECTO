using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="genericResult"></param>
        /// <param name="specificResult"></param>
        /// <param name="primaryResult"></param>
		/// <param name="getCompletedResult">IResultEntry GetCompletedResult(IResultEntry generic,
		//	IResultEntry specific, IResult primary);</param></param>
        void WriteResult(IResultEntry genericResult,
			IResultEntry specificResult, IResult primaryResult, Func<IResultEntry, IResultEntry, IResult, IResultEntry> getCompletedResult);
	}
}