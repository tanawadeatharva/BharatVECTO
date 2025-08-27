using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface IXmlTypeWriter
	{
		XElement GetElement(IDeclarationInputDataProvider inputData);

	}

	public interface IXmlAxlePowertrainTypeWriter
	{
		XElement GetElement(IAxlePowertrainDeclarationInputData axlePt);
	}

}
