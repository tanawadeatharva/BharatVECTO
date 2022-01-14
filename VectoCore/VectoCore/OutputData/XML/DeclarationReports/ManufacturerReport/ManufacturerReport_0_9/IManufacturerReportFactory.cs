using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface IManufacturerReportFactory
	{
		IXMLManufacturerReport GetConventionalLorryManufacturerReport();

		IXMLManufacturerReport GetManufacturerReport(string vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		IMrfXmlType GetConventionalLorryVehicleType();

		IMrfXmlType GetHEV_S2_VehicleType();
		IMrfXmlType GetHEV_S3_VehicleType();
		IMrfXmlType GetHEV_S4_VehicleType();

		IMrfXmlType GetHEV_IEPC_S_VehicleType();

		IMrfXmlType GetPEV_E2_VehicleType();
		IMrfXmlType GetPEV_E3_VehicleType();
		IMrfXmlType GetPEV_E4_VehicleType();



		IMrfXmlType GetHEV_S2_ComponentsType();
		IMrfXmlType GetHEV_S3_ComponentsType();
		IMrfXmlType GetHEV_S4_ComponentsType();
		IMrfXmlType GetHEV_IEPC_S_ComponentsType();


		IMrfXmlType GetConventionalADASType();

		IMrfXmlGroup GetHEVVehicleSequenceGroup();
		IMrfXmlType GetHEVADASType();
		IMrfXmlType GetEngineTorqueLimitationsType();


		IMrfXmlType GetConventionalLorryComponentsType();

		IMrfXmlType GetEngineType();
		IMrfXmlType GetRetarderType();
		IMrfXmlType GetTorqueConverterType();
		IMrfXmlType GetAngleDriveType();
		IMrfXmlType GetTransmissionType();
		IMrfXmlType GetElectricMachinesType();
		IMrfXmlType GetAxleGearType();

		IMrfXmlGroup GetGeneralVehicleOutputGroup();

		IMrfXmlGroup GetConventionalLorryVehicleOutputGroup();


		IMrfXmlGroup GetGeneralLorryVehicleOutputGroup();

		IMrfXmlType GetREESSSpecificationsType();
		IMrfXmlType GetAirdragType();
		IMrfXmlType GetAxleWheelsType();
		IMrfXmlType GetConventionalLorryAuxType();
		IMrfXmlType GetHEV_Px_IHCP_LorryVehicleType();
		IMrfXmlGroup GetHEV_lorryVehicleOutputGroup();

		IMrfXmlType GetHEV_Px_IHCP_ComponentsType();
		IMrfXmlGroup GetHEV_lorryVehicleOutputSequenceGroup();
		IMrfXmlType GetHEV_LorryAuxiliariesType();

	}
}
