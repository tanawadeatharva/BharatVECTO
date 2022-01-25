using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface IManufacturerReportFactory
	{
		IXMLManufacturerReport GetConventionalLorryManufacturerReport();

		IXMLManufacturerReport GetManufacturerReport(string vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		IMrfXmlType GetConventionalLorryVehicleType();
		IMrfXmlType GetHEV_Px_IHCP_LorryVehicleType();
		IMrfXmlType GetHEV_S2_LorryVehicleType();
		IMrfXmlType GetHEV_S3_LorryVehicleType();
		IMrfXmlType GetHEV_S4_LorryVehicleType();
		IMrfXmlType GetHEV_IEPC_S_LorryVehicleType();
		IMrfXmlType GetPEV_E2_LorryVehicleType();
		IMrfXmlType GetPEV_E3_LorryVehicleType();
		IMrfXmlType GetPEV_E4_LorryVehicleType();






		IMrfXmlType GetConventional_PrimaryBusVehicleType();
		IMrfXmlType GetHEV_Px_IHPC_PrimaryBusVehicleType();
		IMrfXmlType GetHEV_S2_PrimaryBusVehicleType();
		IMrfXmlType GetHEV_S3_PrimaryBusVehicleType();
		IMrfXmlType GetHEV_S4_PrimaryBusVehicleType();
		IMrfXmlType GetHEV_IEPC_S_PrimaryBusVehicleType();
		IMrfXmlType GetPEV_E2_PrimaryBusVehicleType();
		IMrfXmlType GetPEV_E3_PrimaryBusVehicleType();
		IMrfXmlType GetPEV_E4_PrimaryBusVehicleType();
		IMrfXmlType GetPEV_IEPC_PrimaryBusVehicleType();

		IMrfXmlType GetConventional_CompletedBusVehicleType();
		IMrfXmlType GetHEV_CompletedBusVehicleType();
		IMrfXmlType GetPEV_CompletedBusVehicleType();


		IMrfXmlType GetConventionalLorryComponentsType();
		IMrfXmlType GetHEV_Px_IHCP_LorryComponentsType();
		IMrfXmlType GetHEV_S2_LorryComponentsType();
		IMrfXmlType GetHEV_S3_LorryComponentsType();
		IMrfXmlType GetHEV_S4_LorryComponentsType();
		IMrfXmlType GetHEV_IEPC_S_LorryComponentsType();
		IMrfXmlType GetPEV_E2_LorryComponentsType();
		IMrfXmlType GetPEV_E3_LorryComponentsType();
		IMrfXmlType GetPEV_E4_LorryComponentsType();
		IMrfXmlType GetPEV_IEPC_S_LorryComponentsType();
		IMrfXmlType GetConventional_PrimaryBusComponentsType();
		IMrfXmlType GetHEV_Px_IHPC_PrimaryBusComponentsType();
		IMrfXmlType GetHEV_S2_PrimaryBusComponentsType();
		IMrfXmlType GetHEV_S3_PrimaryBusComponentsType();
		IMrfXmlType GetHEV_S4_PrimaryBusComponentsType();
		IMrfXmlType GetHEV_IEPC_S_PrimaryBusComponentsType();
		IMrfXmlType GetPEV_E2_PrimaryBusComponentsType();
		IMrfXmlType GetPEV_E3_PrimaryBusComponentsType();
		IMrfXmlType GetPEV_E4_PrimaryBusComponentsType();
		IMrfXmlType GetPEV_IEPC_PrimaryBusComponentsType();






		IMrfVehicleGroup GetGeneralVehicleOutputGroup();


		IMrfXmlGroup GetGeneralLorryVehicleOutputGroup();
		IMrfXmlGroup GetHEV_VehicleSequenceGroup();
		IMrfXmlGroup GetPEV_VehicleSequenceGroup();
		IMrfXmlGroup GetConventionalLorryVehicleOutputGroup();
		IMrfXmlGroup GetHEV_lorryVehicleOutputGroup();
		IMrfXmlGroup GetPEV_lorryVehicleOutputGroup();
		IMrfXmlGroup GetHEV_lorryVehicleOutputSequenceGroup();
		IMrfXmlGroup GetPrimaryBusGeneralVehicleOutputGroup();
		IMrfXmlGroup GetHEV_PrimaryBusVehicleOutputGroup();



		IMrfXmlType GetEngineTorqueLimitationsType();
		IMrfXmlType GetEngineType();
		IMrfXmlType GetRetarderType();
		IMrfXmlType GetTorqueConverterType();
		IMrfXmlType GetAngleDriveType();
		IMrfXmlType GetTransmissionType();
		IMrfXmlType GetElectricMachinesType();
		IMrfXmlType GetAxleGearType();
		IMrfXmlType GetAxleWheelsType();
		IMRFAdasType GetConventionalADASType();
		IMRFAdasType GetHEVADASType();
		IMRFAdasType GetPEVADASType();
		IMrfXmlType GetIEPCSpecifications();
		IMrfXmlType GetREESSSpecificationsType();
		IMrfXmlType GetAirdragType();


		IMRFLorryAuxiliariesType GetConventionalLorryAuxType();
		IMRFLorryAuxiliariesType GetHEV_LorryAuxiliariesType();


		IMRFBusAuxiliariesType GetPrimaryBusAuxType_Conventional();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_P();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_HEV_S();
		IMRFBusAuxiliariesType GetPrimaryBusAuxType_PEV();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_Conventional_HEV_Px();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_HEV_S();
		IMRFBusAuxiliariesType GetPrimaryBusPneumaticSystemType_PEV_IEPC();

		IMRFBusAuxiliariesType GetPrimaryBusElectricSystemType_Conventional_HEV();

		IMRFBusAuxiliariesType GetPrimaryBusElectricSystemType_PEV();
		IMRFBusAuxiliariesType GetPrimaryBusHVACSystemType_Conventional_HEV();
		IMRFBusAuxiliariesType GetPrimaryBusHVACSystemType_PEV();

		IMRFBusAuxiliariesType GetConventionalCompletedBusAuxType();
		IMRFBusAuxiliariesType GetConventionalCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetCompletedBusElectricSystemType();


		IMrfXmlGroup GetPEV_PrimaryBusVehicleOutputGroup();
		IMrfXmlGroup GetCompletedBusGeneralVehicleOutputGroup();
		IMrfVehicleGroup GetCompletedBusSequenceGroup();
		IMrfVehicleGroup GetCompletedBusDimensionSequenceGroup();
		IMrfBusAuxGroup GetCompletedBus_HVACSystemGroup();



	}


}
