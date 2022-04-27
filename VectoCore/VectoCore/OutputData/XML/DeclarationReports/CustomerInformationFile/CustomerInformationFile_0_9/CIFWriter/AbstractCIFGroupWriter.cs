using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.CustomerInformationFile.CustomerInformationFile_0_9;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public abstract class AbstractCIFGroupWriter : IReportOutputGroup
    {
		protected XNamespace _mrf = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:v0.9";
		protected XNamespace _cif = "urn:tugraz:ivt:VectoAPI:CustomerOutput:v0.9";
		protected readonly ICustomerInformationFileFactory _cifFactory;

		protected AbstractCIFGroupWriter(ICustomerInformationFileFactory cifFactory)
		{
			_cifFactory = cifFactory;
		}

		#region Implementation of IMRFGroupWriter

		public abstract IList<XElement> GetElements(IDeclarationInputDataProvider inputData);

		#endregion
	}
}
