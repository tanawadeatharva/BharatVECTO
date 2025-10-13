using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
	public abstract class AbstractMrfXmlType
	{
		protected XNamespace _mrf = AbstractManufacturerReport.Namespace;

        protected readonly IManufacturerReportFactory _mrfFactory;

		protected AbstractMrfXmlType(IManufacturerReportFactory mrfFactory)
		{
			_mrfFactory = mrfFactory;
		}

		protected virtual IVehicleDeclarationInputData GetVehicle(IDeclarationInputDataProvider inputData)
		{
			if (inputData is IMultistepBusInputDataProvider multistep) {
				return multistep.JobInputData.PrimaryVehicle.Vehicle;
			}

			return inputData.JobInputData.Vehicle;
		}
	}
}
