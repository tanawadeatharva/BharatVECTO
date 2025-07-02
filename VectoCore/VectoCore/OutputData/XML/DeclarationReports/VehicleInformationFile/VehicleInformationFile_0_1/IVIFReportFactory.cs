using System.Collections.Generic;
using System.Xml.Linq;
using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public interface IXmlMultistepTypeWriter
	{
		XElement GetElement(IMultistageVIFInputData inputData);
	}

	public interface IReportMultistepCompletedBusOutputGroup
	{
		IList<XElement> GetElements(IMultistageVIFInputData multiStageInputDataProvider);
	}

	public interface IReportMultistepCompletedBusTypeWriter
	{
		XElement GetElement(IMultistageVIFInputData inputData);

	}

	public interface IVIFReportInterimFactory
	{
		IXMLMultistepIntermediateReport GetInterimVIFReport(VehicleCategory vehicleType,
			VectoSimulationJobType jobType, ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		#region Vehicle

		IXmlMultistepTypeWriter GetConventionalVehicleType();

		IXmlMultistepTypeWriter GetHEVVehicleType();

		IXmlMultistepTypeWriter GetPEVVehicleType();

        IXmlMultistepTypeWriter Get_FCHV_VehicleType();

        IXmlMultistepTypeWriter GetExemptedVehicleType();

		#endregion

		IReportMultistepCompletedBusOutputGroup GetCompletedBusGeneralParametersGroup();
		IReportMultistepCompletedBusOutputGroup GetCompletedBusParametersGroup();
		IReportMultistepCompletedBusOutputGroup GetCompletedBusPassengerCountGroup();
		IReportMultistepCompletedBusOutputGroup GetCompletedBusDimensionsGroup();

		IReportMultistepCompletedBusTypeWriter GetConventionalInterimComponentsType();

		IReportMultistepCompletedBusTypeWriter GetxEVInterimComponentsType();


		IReportMultistepCompletedBusTypeWriter GetInterimAirdragType();
		IReportMultistepCompletedBusTypeWriter GetInterimConventionalAuxiliariesType();

		IReportMultistepCompletedBusTypeWriter GetInterimxEVAuxiliariesType();

		IVIFFAdasType GetConventionalInterimADASType();

		IVIFFAdasType GetHEVInterimADASType();

		IVIFFAdasType GetPEVInterimADASType();
	}


	public interface IVIFReportFactory
	{
		IXMLVehicleInformationFile GetVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);

		
		#region Vehicle

		IXmlTypeWriter GetConventionalVehicleType();
		IXmlTypeWriter GetHevIepcSVehicleType();
		IXmlTypeWriter GetHevPxVehicleType();
		IXmlTypeWriter GetHevS2VehicleType();
		IXmlTypeWriter GetHevS3VehicleType();
		IXmlTypeWriter GetHevS4VehicleType();
		IXmlTypeWriter Get_FCHV_IEPC_VehicleType();
        IXmlTypeWriter GetMultipleFCHVVehicleType();
        IXmlTypeWriter GetMultiplePEVVehicleType();
        IXmlTypeWriter GetMultipleSHEVVehicleType();
        IXmlTypeWriter Get_FCHV_F2_VehicleType();
		IXmlTypeWriter Get_FCHV_F3_VehicleType();
		IXmlTypeWriter Get_FCHV_F4_VehicleType();
		IXmlTypeWriter GetIepcVehicleType();
		IXmlTypeWriter GetPevE2VehicleType();
		IXmlTypeWriter GetPevE3VehicleType();
		IXmlTypeWriter GetPevE4VehicleType();

		IXmlTypeWriter GetPevIEPCVehicleType();

		IXmlTypeWriter GetExemptedVehicleType();


		#endregion

		#region Componenet Group

		IXmlTypeWriter GetConventionalComponentType();
		IXmlTypeWriter GetHevIepcSComponentVIFType();
		IXmlTypeWriter GetHevPxComponentVIFType();
		IXmlTypeWriter GetHevS2ComponentVIFType();
		IXmlTypeWriter GetHevS3ComponentVIFType();
		IXmlTypeWriter GetHevS4ComponentVIFType();
        IXmlTypeWriter Get_Multiple_PEV_ComponentVIFType();
        IXmlTypeWriter Get_Multiple_SHEV_ComponentVIFType();
        IXmlTypeWriter Get_Multiple_FCHV_ComponentVIFType();
        IXmlTypeWriter Get_FCHV_F2_ComponentVIFType();
		IXmlTypeWriter Get_FCHV_F3_ComponentVIFType();
		IXmlTypeWriter Get_FCHV_F4_ComponentVIFType();
		IXmlTypeWriter GetPevE2ComponentVIFType();
		IXmlTypeWriter GetPevE3ComponentVIFType();
		IXmlTypeWriter GetPevE4ComponentVIFType();
		IXmlTypeWriter GetPevIEPCComponentVIFType();
		IXmlTypeWriter GetHevIepcFComponentVIFType();

		#endregion


		#region Components

		IVIFFAdasType GetConventionalADASType();
		IVIFFAdasType GetHEVADASType();
		IVIFFAdasType GetPEVADASType();
		
		IXmlTypeWriter GetAngelDriveType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainAngleDriveType();

        IXmlTypeWriter GetRetarderType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainRetarderType();

        IXmlTypeWriter GetAuxiliaryType();
		IXmlTypeWriter GetAuxiliaryHevSType();
		IXmlTypeWriter GetAuxiliaryFCHVType();

		IXmlTypeWriter GetAuxiliaryIEPC_SType();

		IXmlTypeWriter GetAuxiliaryHevPType();
		IXmlTypeWriter GetAuxiliaryIEPCType();
		IXmlTypeWriter GetAuxiliaryPEVType();
		IXmlTypeWriter GetAxlegearType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainAxleGearType();
        IXmlTypeWriter GetAxleWheelsType();
		IXmlTypeWriter GetBoostingLimitationsType();
		IXmlTypeWriter GetElectricEnergyStorageType();
		IXmlTypeWriter GetElectricMachineGENType();
		IXmlElectricMachineSystemType GetElectricMachineSystemType();
		IXmlTypeWriter GetElectricMachineType();
        IXmlTypeWriter GetGeneratorType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainElectricMachineType();
        IXmlTypeWriter GetElectricMotorTorqueLimitsType();
		IXmlTypeWriter GetEngineType();
		IXmlTypeWriter GetTorqueConvertType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainTorqueConverterType();
        IXmlTypeWriter GetIepcType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainIEPCType();

        IXmlTypeWriter GetTorqueLimitsType();
		IXmlTypeWriter GetTransmissionType();
        IXmlAxlePowertrainTypeWriter GetAxlePowertrainTransmissionType();
        IXmlTypeWriter GetFuelCellType();

		#endregion


		#region Parameter Groups

		IReportOutputGroup GetConventionalVehicleGroup();
		IReportOutputGroup GetPrimaryBusGeneralParameterGroup();
		IReportOutputGroup GetPrimaryBusChassisParameterGroup();
		IReportOutputGroup GetPrimaryBusRetarderParameterGroup();
		IReportOutputGroup GetPrimaryBusXevParameterGroup();

		IReportOutputGroup GetHevIepcSVehicleParameterGroup();
		IReportOutputGroup GetHevIepcFVehicleParameterGroup();
		IReportOutputGroup GetHevSxVehicleParameterGroup();
		IReportOutputGroup GetIepcVehicleParameterGroup();
		IReportOutputGroup GetPevExVehicleParmeterGroup();
        IReportOutputGroup Get_Multiple_PEV_VehicleParmeterGroup();
        IReportOutputGroup Get_Multiple_SHEV_VehicleParmeterGroup();
        IReportOutputGroup Get_Multiple_FCHV_VehicleParmeterGroup();
        IReportOutputGroup Get_FCHV_Fx_VehicleParmeterGroup();
        IReportOutputGroup Get_FCHV_IEPC_VehicleParmeterGroup();
        IReportOutputGroup GetPevIEPCVehicleParmeterGroup();
		IReportOutputGroup GetHevPxVehicleParameterGroup();

		IReportOutputGroup GetExemptedVehicleParameterGroup();

		#endregion

	}
}
