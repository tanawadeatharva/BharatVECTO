using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter.CompletedBus;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter
{
    public interface IManufacturerReportFactory
	{
		IXMLManufacturerReport GetConventionalLorryManufacturerReport();

		IXMLManufacturerReport GetManufacturerReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		IXmlTypeWriter GetConventionalLorryVehicleType();
		IXmlTypeWriter GetHEV_Px_IHCP_LorryVehicleType();
		IXmlTypeWriter GetHEV_S2_LorryVehicleType();
		IXmlTypeWriter GetHEV_S3_LorryVehicleType();
		IXmlTypeWriter GetHEV_S4_LorryVehicleType();
		IXmlTypeWriter GetHEV_IEPC_S_LorryVehicleType();
		IXmlTypeWriter GetPEV_E2_LorryVehicleType();
		IXmlTypeWriter GetPEV_E3_LorryVehicleType();
		IXmlTypeWriter GetPEV_E4_LorryVehicleType();
		IXmlTypeWriter GetPEV_IEPC_LorryVehicleType();

		IXmlTypeWriter GetExempted_LorryVehicleType();





		IXmlTypeWriter GetConventional_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_Px_IHPC_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_S2_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_S3_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_S4_PrimaryBusVehicleType();
		IXmlTypeWriter GetHEV_IEPC_S_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_E2_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_E3_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_E4_PrimaryBusVehicleType();
		IXmlTypeWriter GetPEV_IEPC_PrimaryBusVehicleType();

		IXmlTypeWriter GetExempted_PrimaryBusVehicleType();

		IXmlTypeWriter GetConventional_CompletedBusVehicleType();
		IXmlTypeWriter GetHEV_CompletedBusVehicleType();
		IXmlTypeWriter GetPEV_CompletedBusVehicleType();

		IXmlTypeWriter GetExempted_CompletedBusVehicleType();


		IXmlTypeWriter GetConventionalLorryComponentsType();
		IXmlTypeWriter GetHEV_Px_IHCP_LorryComponentsType();
		IXmlTypeWriter GetHEV_S2_LorryComponentsType();
		IXmlTypeWriter GetHEV_S3_LorryComponentsType();
		IXmlTypeWriter GetHEV_S4_LorryComponentsType();
		IXmlTypeWriter GetHEV_IEPC_S_LorryComponentsType();
		IXmlTypeWriter GetPEV_E2_LorryComponentsType();
		IXmlTypeWriter GetPEV_E3_LorryComponentsType();
		IXmlTypeWriter GetPEV_E4_LorryComponentsType();
		IXmlTypeWriter GetPEV_IEPC_S_LorryComponentsType();
		IXmlTypeWriter GetConventional_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_Px_IHPC_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_S2_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_S3_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_S4_PrimaryBusComponentsType();
		IXmlTypeWriter GetHEV_IEPC_S_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_E2_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_E3_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_E4_PrimaryBusComponentsType();
		IXmlTypeWriter GetPEV_IEPC_PrimaryBusComponentsType();
		IXmlTypeWriter GetConventional_CompletedBusComponentsType();
		IXmlTypeWriter GetHEV_CompletedBusComponentsType();
		IXmlTypeWriter GetPEV_CompletedBusComponentsType();


		IReportVehicleOutputGroup GetGeneralVehicleOutputGroup();


		IReportOutputGroup GetGeneralLorryVehicleOutputGroup();
		IReportOutputGroup GetHEV_VehicleSequenceGroup();
		IReportOutputGroup GetPEV_VehicleSequenceGroup();
		IReportOutputGroup GetConventionalLorryVehicleOutputGroup();
		IReportOutputGroup GetHEV_lorryVehicleOutputGroup();
		IReportOutputGroup GetPEV_lorryVehicleOutputGroup();
		IReportOutputGroup GetHEV_lorryVehicleOutputSequenceGroup();
		IReportOutputGroup GetPrimaryBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetExemptedPrimaryBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetHEV_PrimaryBusVehicleOutputGroup();


		IXmlTypeWriter GetEngineTorqueLimitationsType();
		IXmlTypeWriter GetEngineType();
		IXmlTypeWriter GetRetarderType();
		IXmlTypeWriter GetTorqueConverterType();
		IXmlTypeWriter GetAngleDriveType();
		IXmlTypeWriter GetTransmissionType();
		IXmlTypeWriter GetElectricMachinesType();
		IXmlTypeWriter GetAxleGearType();
		IXmlTypeWriter GetAxleWheelsType();
		IMRFAdasType GetConventionalADASType();
		IMRFAdasType GetHEVADASType();
		IMRFAdasType GetPEVADASType();
		IXmlTypeWriter GetIEPCSpecifications();
		IXmlTypeWriter GetREESSSpecificationsType();
		IMrfAirdragType GetAirdragType();


		IMRFLorryAuxiliariesType GetConventionalLorryAuxType();
		IMRFLorryAuxiliariesType GetHEV_LorryAuxiliariesType();

		IMRFLorryAuxiliariesType GetPEV_LorryAuxiliariesType();


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

		IMRFBusAuxiliariesType GetConventionalCompletedBusAuxType();
		IMRFBusAuxiliariesType GetConventionalCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetConventionalCompletedBusElectricSystemType();

		IMRFBusAuxiliariesType GetHEVCompletedBusAuxType();
		IMRFBusAuxiliariesType GetHEVCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetHEVCompletedBusElectricSystemType();

		IMRFBusAuxiliariesType GetPEVCompletedBusAuxType();
		IMRFBusAuxiliariesType GetPEVCompletedBus_HVACSystemType();
		IMRFBusAuxiliariesType GetPEVCompletedBusElectricSystemType();


		IReportOutputGroup GetPEV_PrimaryBusVehicleOutputGroup();
		IReportOutputGroup GetConventionalCompletedBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetHEVCompletedBusGeneralVehicleOutputGroup();
		IReportOutputGroup GetPEVCompletedBusGeneralVehicleOutputGroup();
		IReportVehicleOutputGroup GetCompletedBusSequenceGroup();
		IReportVehicleOutputGroup GetCompletedBusDimensionSequenceGroup();
		IMrfBusAuxGroup GetCompletedBus_HVACSystemGroup();
		IMrfBusAuxGroup GetCompletedBus_xEVHVACSystemGroup();
		IMrfVehicleType GetBoostingLimitationsType();

	}


}
