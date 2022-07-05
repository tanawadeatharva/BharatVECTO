using TUGraz.VectoCommon.InputData;
using TUGraz.VectoCommon.Models;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportGroupWriter;
using TUGraz.VectoCore.OutputData.XML.DeclarationReports.ManufacturerReport.ManufacturerReport_0_9.ManufacturerReportXMLTypeWriter;

namespace TUGraz.VectoCore.OutputData.XML.DeclarationReports.VehicleInformationFile.VehicleInformationFile_0_1
{
	public interface IVIFReportFactory
	{
		IXMLPrimaryVehicleReport GetVIFReport(VehicleCategory vehicleType, VectoSimulationJobType jobType,
			ArchitectureID archId, bool exempted, bool iepc, bool ihpc);




		#region Vehicle

		IXmlTypeWriter GetConventionalLorryVehicleType();


		#endregion

		#region Componenet Group

		IXmlTypeWriter GetConventionalComponentType();
		IXmlTypeWriter GetHevIepcSComponentVIFType();

		#endregion



		#region Components

		IXmlTypeWriter GetAdasType();
		IXmlTypeWriter GetAngelDriveType();
		IXmlTypeWriter GetAuxiliaryType();
		IXmlTypeWriter GetAuxiliaryHevSType();
		IXmlTypeWriter GetAxlegearType();
		IXmlTypeWriter GetAxleWheelsType();
		IXmlTypeWriter GetBoostingLimitationsType();
		IXmlTypeWriter GetElectricEnergyStorageType();
		IXmlTypeWriter GetElectricMachineGENType();
		IXmlTypeWriter GetElectricMotorTorqueLimitsType();
		IXmlTypeWriter GetEngineType();
		IXmlTypeWriter GetTorqueConvertType();
		IXmlTypeWriter GetIepcType();
		IXmlTypeWriter GetTorqueLimitsType();
		IXmlTypeWriter GetTransmissionType();

		#endregion


		#region Groups

		IReportOutputGroup GetConventionalVehicleGroup();
		IReportOutputGroup GetPrimaryBusGeneralParameterGroup();
		IReportOutputGroup GetPrimaryBusChassisParameterGroup();
		IReportOutputGroup GetPrimaryBusRetarderParameterGroup();
		IReportOutputGroup GetPrimaryBusXeVParameterGroup();
		IReportOutputGroup GetHevIepcSVehicleGroup();
		IReportOutputGroup GetHevSxVehicleGroup();
		IReportOutputGroup GetIepcVehicleGroup();
		IReportOutputGroup GetPEVVehicleGroup();


		#endregion

	}
}
