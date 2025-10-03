using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public abstract class AbstractCIFGroupWriter : IReportOutputGroup
    {
		protected XNamespace _cif => AbstractCustomerReport.Namespace;
		protected readonly ICustomerInformationFileFactory _cifFactory;

		protected AbstractCIFGroupWriter(ICustomerInformationFileFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}

		#region Implementation of IMRFGroupWriter

		public abstract IList<XElement> GetElements(IDeclarationInputDataProvider inputData);

		#endregion

		protected virtual IVehicleDeclarationInputData GetVehicle(IDeclarationInputDataProvider inputData)
		{
			if (inputData is IMultistepBusInputDataProvider multistep) {
				return multistep.JobInputData.PrimaryVehicle.Vehicle;
			}

			return inputData.JobInputData.Vehicle;
		}
	}

    public abstract class AbstractCifXmlType
	{
		protected XNamespace _cif => AbstractCustomerReport.Namespace;
		protected readonly ICustomerInformationFileFactory _cifFactory;

		protected AbstractCifXmlType(ICustomerInformationFileFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}
	}
}
