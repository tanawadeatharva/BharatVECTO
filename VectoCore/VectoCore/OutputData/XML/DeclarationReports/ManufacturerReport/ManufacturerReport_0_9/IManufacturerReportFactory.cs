using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface IManufacturerReportFactory
	{
		IXMLManufacturerReport GetConventionalLorryManufacturerReport();

		IMrfXmlType GetConventionalLorryVehicleType();
		IMrfXmlType GetConventionalADASType();
		IMrfXmlType GetEngineTorqueLimitationsType();


		IMrfXmlType GetConventionalLorryComponentsType();

		IMrfXmlType GetEngineType();
		IMrfXmlType GetRetarderType();
		IMrfXmlType GetTorqueConverterType();
		IMrfXmlType GetAngleDriveType();
		IMrfXmlType GetTransmissionType();


		IMrfXmlGroup GetGeneralVehicleOutputGroup();

		IMrfXmlGroup GetConventionalLorryVehicleOutputGroup();


		IMrfXmlGroup GetGeneralLorryVehicleOutputGroup();


		IMrfXmlType GetAirdragType();
		IMrfXmlType GetAxleWheelsType();
		IMrfXmlType GetConventionalLorryAuxType();
	}
}
