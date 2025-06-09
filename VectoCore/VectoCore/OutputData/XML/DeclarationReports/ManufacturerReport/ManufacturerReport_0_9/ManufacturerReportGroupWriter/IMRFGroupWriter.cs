using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter
{
    public interface IReportOutputGroup
	{
		IList<XElement> GetElements(IDeclarationInputDataProvider inputData);
    }

	public interface IReportVehicleOutputGroup
	{
		IList<XElement> GetElements(IVehicleDeclarationInputData vehicleData);
	}

	public interface IReportCompletedBusOutputGroup
	{
		IList<XElement> GetElements(IMultistepBusInputDataProvider multiStageInputDataProvider);
	}

	public interface IAxlePowertrainReportOutputGroup
	{
        IList<XElement> GetElements(IAxlePowertrainDeclarationInputData axlePt);
    }
}
