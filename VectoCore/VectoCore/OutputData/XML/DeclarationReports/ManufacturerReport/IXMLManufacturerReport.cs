using System;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Models.Simulation.Data;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport
{
	public interface IXMLManufacturerReport
	{
		//void InitializeVehicleData(IDeclarationInputDataProvider inputData);
		void Initialize(VectoRunData modelData);
		XDocument Report { get; }
		void WriteResult(IResultEntry resultValue);
		void GenerateReport();
	}

	public interface IXMLManufacturerReportCompletedBus
	{
        //Not supported in C# 7.3
        //delegate IResultEntry GetCompletedResult(IResultEntry generic,
        //	IResultEntry specific, IResult primary);

        /// <summary>
        /// Adds a result to the report, the functor getCompletedResult determines how the result is calculated
        /// </summary>
        /// <param name="genericResult"></param>
        /// <param name="specificResult"></param>
        /// <param name="primaryResult"></param>
		/// <param name="getCompletedResult">IResultEntry GetCompletedResult(IResultEntry generic,
		//	IResultEntry specific, IResult primary);</param>
        void WriteResult(IResultEntry genericResult,
			IResultEntry specificResult, IResult primaryResult, Func<IResultEntry, IResultEntry, IResult, IResultEntry> getCompletedResult);
	}
}