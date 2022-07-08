using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1.Components;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public interface IVIFReportFactory
	{
		IXMLPrimaryVehicleReport GetVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);


		#region Vehicle

		IXmlTypeWriter GetConventionalVehicleType();
		IXmlTypeWriter GetHevIepcSVehicleType();
		IXmlTypeWriter GetHevPxVehicleType();
		IXmlTypeWriter GetHevS2VehicleType();
		IXmlTypeWriter GetHevS3VehicleType();
		IXmlTypeWriter GetHevS4VehicleType();
		IXmlTypeWriter GetIepcVehicleType();
		IXmlTypeWriter GetPevE2VehicleType();
		IXmlTypeWriter GetPevE3VehicleType();
		IXmlTypeWriter GetPevE4VehicleType();


		#endregion

		#region Componenet Group

		IXmlTypeWriter GetConventionalComponentType();
		IXmlTypeWriter GetHevIepcSComponentVIFType();
		IXmlTypeWriter GetHevPxComponentVIFType();
		IXmlTypeWriter GetHevS2ComponentVIFType();
		IXmlTypeWriter GetHevS3ComponentVIFType();
		IXmlTypeWriter GetHevS4ComponentVIFType();
		IXmlTypeWriter GetPevE2ComponentVIFType();
		IXmlTypeWriter GetPevE3ComponentVIFType();
		IXmlTypeWriter GetPevE4ComponentVIFType();
		IXmlTypeWriter GetIepcComponentVIFType();

		#endregion


		#region Components

		IXmlTypeWriter GetAdasType();
		IXmlTypeWriter GetAngelDriveType();
		IXmlTypeWriter GetAuxiliaryType();
		IXmlTypeWriter GetAuxiliaryHevSType();
		IXmlTypeWriter GetAuxiliaryHevPType();
		IXmlTypeWriter GetAuxiliaryIEPCType();
		IXmlTypeWriter GetAuxiliaryPEVType();
		IXmlTypeWriter GetAxlegearType();
		IXmlTypeWriter GetAxleWheelsType();
		IXmlTypeWriter GetBoostingLimitationsType();
		IXmlTypeWriter GetElectricEnergyStorageType();
		IXmlTypeWriter GetElectricMachineGENType();
		IXmlElectricMachineSystemType GetElectricMachineSystemType();
		IXmlTypeWriter GetElectricMachineType();
		IXmlTypeWriter GetElectricMotorTorqueLimitsType();
		IXmlTypeWriter GetEngineType();
		IXmlTypeWriter GetTorqueConvertType();
		IXmlTypeWriter GetIepcType();
		IXmlTypeWriter GetTorqueLimitsType();
		IXmlTypeWriter GetTransmissionType();

		#endregion


		#region Parameter Groups

		IReportOutputGroup GetConventionalVehicleGroup();
		IReportOutputGroup GetPrimaryBusGeneralParameterGroup();
		IReportOutputGroup GetPrimaryBusChassisParameterGroup();
		IReportOutputGroup GetPrimaryBusRetarderParameterGroup();
		IReportOutputGroup GetPrimaryBusXevParameterGroup();

		IReportOutputGroup GetHevIepcSVehicleParameterGroup();
		IReportOutputGroup GetHevSxVehicleParameterGroup();
		IReportOutputGroup GetIepcVehicleParameterGroup();
		IReportOutputGroup GetPevExVehicleParmeterGroup();
		IReportOutputGroup GetHevPxVehicleParameterGroup();

		#endregion

	}
}
