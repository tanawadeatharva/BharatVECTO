using System;
using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.Utils.Ninject;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9.ResultWriter
{
	public interface IResultsWriterFactory
	{
		IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc,
			bool exempted);
	}

	public interface IInternalResultWriterFactory
	{
		IResultsWriter GetCIFResultsWriter(
			VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification vehicleClasiClassification);
	}

	public class ResultWriterFactory : IResultsWriterFactory
	{
		private readonly IInternalResultWriterFactory _internalFactory;

		public ResultWriterFactory(IInternalResultWriterFactory internalFactory)
		{
			_internalFactory = internalFactory;
		}

		#region Implementation of IResultsWriterFactory

		public IResultsWriter GetCIFResultsWriter(string vehicleCategory, VectoSimulationJobType jobType, bool ovc, bool exempted)
		{
			try {
				return _internalFactory.GetCIFResultsWriter(
					new VehicleTypeAndArchitectureStringHelperResults.ResultsVehicleClassification(vehicleCategory,
						jobType, ovc, exempted));
			} catch (Exception e) {
				throw new Exception($"Could not create ResultsWriter for vehicle category {vehicleCategory}, {jobType}, ovc: {ovc}, exempted: {exempted}", e);
			}
		}

		#endregion
	}

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