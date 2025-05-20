using System;
using TUGraz.VectoCommon.Exceptions;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.Utils;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.Common
{
	public static class ResultWriterNamingHelper
	{
		public const string RESULT_WRITER_2nd_AMDM = "ResultWriter_v0.9";

		public const string RESULT_WRITER_3rd_AMDM = "ResultWriter_v1.0";

		public static string GetResultWriterName(IDeclarationInputDataProvider inputData)
		{
			switch (inputData.DataSource.SourceType) {
				case DataSourceType.XMLFile:
					return GetXMLResultWriterName(inputData);
				case DataSourceType.JSONFile:
					return GetJSONResultWriterName(inputData);
			}

			throw new VectoException("unhandled input data source type {0}", inputData.DataSource.SourceType);
		}

		private static string GetXMLResultWriterName(IDeclarationInputDataProvider inputData)
		{
				if (inputData is IMultistepBusInputDataProvider multistep) {
					switch (multistep.JobInputData.PrimaryVehicle.DataSource.TypeVersion) {
						case XMLDefinitions.DECLARATION_MULTISTAGE_BUS_VEHICLE_NAMESPACE_VO1:
							return RESULT_WRITER_2nd_AMDM;
						// todo MQ 20250514: add further cases for new schema version of VIF once defined
					}
					throw new VectoException("unknown XML type of multistep input data {0}",
						inputData.JobInputData.Vehicle.DataSource.TypeVersion);
				}
			switch (inputData.JobInputData.Vehicle.DataSource.TypeVersion) {
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V24:
					return RESULT_WRITER_2nd_AMDM;
				case XMLDefinitions.DECLARATION_DEFINITIONS_NAMESPACE_URI_V27:
					return RESULT_WRITER_3rd_AMDM;
			}

			throw new VectoException("unknown XML type of input data {0}",
				inputData.JobInputData.Vehicle.DataSource.TypeVersion);
		}

		private static string GetJSONResultWriterName(IDeclarationInputDataProvider inputData)
		{
			return RESULT_WRITER_2nd_AMDM;
		}

    }
}