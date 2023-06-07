using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
    public abstract class AbstractVIFGroupWriter : IReportOutputGroup
	{
		protected readonly IVIFReportFactory _vifReportFactory;
		protected XNamespace _vif = "urn:tugraz:ivt:VectoAPI:DeclarationOutput:VehicleInterimFile:v0.1";
		protected XNamespace _v24 = "urn:tugraz:ivt:VectoAPI:DeclarationDefinitions:v2.4";


		protected AbstractVIFGroupWriter(IVIFReportFactory vifReportFactory)
		{
			_vifReportFactory = vifReportFactory;
		}

		#region Implementation of IReportOutputGroup

		public abstract IList<XElement> GetElements(IDeclarationInputDataProvider inputData);

		#endregion
	}
}
